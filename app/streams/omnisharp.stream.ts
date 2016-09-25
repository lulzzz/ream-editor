import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable, Observer, Subscription, Subject } from 'rxjs/Rx';
import { QueryMessage, OmnisharpMessage, WebSocketMessage, SessionMessage } from '../messages/index';
import { ProcessStream, EditorStream, QueryStream, SessionStream } from './index';
import { ProcessHelper } from '../utils/process-helper';
import { CodeRequest, UpdateBufferRequest } from './interfaces';
import { CodeCheckResult, EditorChange, AutocompletionQuery } from '../models/index';
import * as uuid from 'node-uuid';
import config from '../config';

class SessionTemplateMap {
    constructor(
        public columnOffset: number,
        public lineOffset: number,
        public fileName: string,
        public sessionId: string,
        public templateRequest: UpdateBufferRequest = null,
        public edits: EditorChange[] = null,
        public timestamp: number = null,
    ) {}
}

class SessionUpdated {
    constructor(
        public sessionId: string,
        public timestamp: number
    ) {}    
}

class SessionReadyState {
    constructor(
        public sessionId: string,
        public ready: boolean
    ) {}
}

class SessionAutocompleteMap {
    constructor(
        public sessionId: string,
        public timestamp: number,
        public request: AutocompletionQuery
    ) {}
}

@Injectable()
export class OmnisharpStream {
    public events: Observable<OmnisharpMessage>;
    private process: ProcessStream;
    private templateMap: any = {};
    constructor(
        private editor: EditorStream,
        private query: QueryStream,
        private session: SessionStream,
        private http: Http
    ) {
        this.process = new ProcessStream(http);

        const sessionMaps = query.events
            .filter(msg => msg.type === 'code-template-response')
            .map(msg => {
                const update: UpdateBufferRequest = {
                    SessionId: msg.id,
                    FileName: `${config.omnisharpProjectPath}\\b${msg.id.replace(/\-/g, '')}.cs`,
                    FromDisk: false,
                    Buffer: msg.codeTemplate.template
                };
                return new SessionTemplateMap(
                    msg.codeTemplate.columnOffset,
                    msg.codeTemplate.lineOffset,
                    update.FileName,
                    msg.id,
                    update
                );
            })
            .flatMap(sessionMap => 
                this.http
                    .post(this.action('updatebuffer'), JSON.stringify(sessionMap.templateRequest))
                    .map(res => sessionMap)) // todo check result
            .publish();

        const sessionReady = session.events
            .filter(msg => msg.type === 'create')
            .flatMap(msg => {
                // one time delay until session is ready
                let sessionMap: SessionTemplateMap = null;
                const ready = new Promise((done) => {
                    const sessionSub = sessionMaps.filter(x => x.sessionId === msg.id).subscribe(x => {
                        sessionMap = x;
                        done();
                        sessionSub.unsubscribe();
                    });
                });
                return editor.events
                    .filter(x => x.type === 'edit' && x.id === msg.id)
                    .delayWhen(x => Observable.fromPromise(ready))
                    .map(x => {
                        return <EditorChange> {
                            newText: x.data.text.join('\n'),
                            startLine: x.data.from.line + sessionMap.lineOffset + 1,
                            startColumn: x.data.from.ch + (x.data.from.line === 0 ? sessionMap.columnOffset : 0) + 1,
                            endLine: x.data.to.line + sessionMap.lineOffset + 1,
                            endColumn: x.data.to.ch + (x.data.to.line === 0 ? sessionMap.columnOffset : 0) + 1,
                            timestamp: x.data.timestamp
                        };
                    })
                    .bufferTime(500)
                    .filter(edits => edits.length > 0)
                    .map(edits => new SessionTemplateMap(
                        sessionMap.columnOffset,
                        sessionMap.lineOffset,
                        sessionMap.fileName,
                        sessionMap.sessionId,
                        null,
                        edits
                    ));
            })
            .flatMap(sessionMap => 
                this.http
                    .post(this.action('updatebuffer'), JSON.stringify({
                        FromDisk: false,
                        FileName: sessionMap.fileName,
                        Changes: sessionMap.edits
                    }))
                    .map(res =>
                        new SessionUpdated(
                            sessionMap.sessionId,
                            sessionMap.edits[sessionMap.edits.length - 1].timestamp)))
            .publish();

        sessionReady.connect();
        
        sessionMaps.subscribe(map => {
            if (!this.templateMap[map.sessionId]) {
                this.templateMap[map.sessionId] = new SessionTemplateMap(map.columnOffset, map.lineOffset, map.fileName, null);
            }
        });

        const sessionStatus = session.events.filter(msg => msg.type === 'create')
            .flatMap(msg => {
                return new Observable<SessionReadyState>((obs: Observer<SessionReadyState>) => {
                    let lastEdit = 0;
                    let lastUpdate = 1;
                    let lastState = false;
                    editor.events.filter(x => x.type === 'edit' && x.id === msg.id).subscribe(x => {
                        Assert(lastEdit < x.data.timestamp, 'edit timestamp did not increase');
                        const wasFirst = lastEdit === 0;
                        lastEdit = x.data.timestamp;
                        if (lastState || wasFirst) {
                            lastState = false;
                            obs.next(new SessionReadyState(msg.id, false));
                        }
                    });
                    sessionReady.filter(x => x.sessionId === msg.id).subscribe(x => {
                        Assert(lastUpdate < x.timestamp, 'update timestamp did not increase');
                        lastUpdate = x.timestamp;
                        if (lastState !== (lastEdit <= lastUpdate)) {
                            lastState = lastEdit <= lastUpdate;
                            obs.next(new SessionReadyState(msg.id, lastState));
                        }
                    });
                });
            })
            .scan((validSessions: string[], value: SessionReadyState) => {
                const without = validSessions.filter(x => x !== value.sessionId);
                if (value.ready) {
                    return [value.sessionId].concat(validSessions);
                }
                return without;
            }, [])
            .publishReplay();

        sessionStatus
            .connect();

        const waitForSession = (msg: SessionMessage) : Observable<number> => {
            return new Observable<number>((obs: Observer<number>) => {
                let done = false;
                const sub = sessionStatus.filter(s => s.indexOf(msg.id) !== -1).subscribe(() => {
                    if (!done) {
                        done = true;
                        obs.next(1);
                        obs.complete();
                        // timing sensitive, if we dont wait, sub might be undefined
                        setTimeout(() => {
                            sub.unsubscribe();
                        }, 200);
                    }
                });
            });
        };

        const codeChecks = session.events
            .filter(msg => msg.type === 'codecheck')
            .delayWhen(waitForSession)
            .map(msg => {
                return new SessionTemplateMap(null, null, this.templateMap[msg.id].fileName, msg.id, null, null, performance.now());
            })
            .flatMap(msg => 
                this.http
                    .post(this.action('codecheck'), JSON.stringify({ FileName: msg.fileName }))
                    .map(res => res.json())
                    .map(res => this.mapQuickFixes(res, msg.sessionId, this.templateMap))
                    .map(checks => {
                        return new OmnisharpMessage('codecheck', msg.sessionId, null, null, this.filterCodeChecks(checks));
                    }));

        const autoCompletions = session.events
            .filter(msg => msg.type === 'autocomplete')
            .delayWhen(waitForSession)
            .map(msg => {
                const tmpl: SessionTemplateMap = this.templateMap[msg.id];
                const request = msg.autoComplete;
                request.fileName = tmpl.fileName;
                request.column += (request.line === 0 ? tmpl.columnOffset : 0) + 1;
                request.line += (request.line === 0 ? tmpl.lineOffset : 0) + 1;
                return new SessionAutocompleteMap(
                    msg.id,
                    performance.now(),
                    request
                );
            })
            .flatMap(msg => 
                this.http
                    .post(this.action('autocomplete'), JSON.stringify({ FileName: msg.request.fileName }))
                    .map(res => res.json())
                    .map((completions: any[]) => {
                        return new OmnisharpMessage('autocompletion', msg.sessionId, null, completions, null);
                    }));

        this.events = this.process
            .status
            .map(msg => new OmnisharpMessage(msg.type))
            .merge(codeChecks)
            .merge(autoCompletions);

        let helper = new ProcessHelper();
        let cmd = helper.omnisharp(config.omnisharpPort);
        this.process.start('omnisharp', cmd.command, cmd.directory, config.omnisharpPort);
        this.once(msg => msg.type === 'ready', () => {
            sessionMaps.connect();
        });
    }

    public once(pred: (msg: OmnisharpMessage) => boolean, handler: (msg: OmnisharpMessage) => void) {
        const sub = this.events.filter(msg => pred(msg)).subscribe(msg => {
            sub.unsubscribe();
            handler(msg);
        });
    }

    public stopServer() {
        this.process.close();
    }

    private mapQuickFixes = (result: any, mapKey: string, maps: any): CodeCheckResult[] => {
        let fixes: any[] = result.QuickFixes;
        const map = maps[mapKey];
        Assert(map != null, 'Did not find template map');
        return fixes.map(x => {
            const line = x.Line - 1 - (map.lineOffset);
            const column = x.Column - 1 - (line === 0 ? map.columnOffset : 0);
            const endLine = x.EndLine - 1 - (map.lineOffset);
            const endColumn = x.EndColumn - 1 - (endLine === 0 ? map.columnOffset : 0);
            return <CodeCheckResult> {
                text: this.cleanupMessage(x.Text),
                logLevel: x.LogLevel,
                fileName: x.FileName,
                line,
                column,
                endLine,
                endColumn
            };
        });
    }
    
    private filterCodeChecks(checks: CodeCheckResult[]): CodeCheckResult[] {
        let filt = checks
            .filter(c => {
                const isMissingSemicolon = c.text === '; expected';
                const isHidden = c.logLevel === 'Hidden';
                const isBeforeUserStub = c.line < 0;
                return !(isHidden || isMissingSemicolon || isBeforeUserStub);
            });
        return filt;
    }
    
    private cleanupMessage(text: string) {
        let fluf = [
            / \(are you missing a using directive or an assembly reference\?\)/
        ];
        let s = text;
        fluf.forEach(r => {
            s = s.replace(r, '');
        });
        return s;
    }

    private action(name: string) {
        return `http://localhost:${config.omnisharpPort}/${name}`;
    }
}
