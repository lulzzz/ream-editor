import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Message, EventName } from './api';
import { QueryStream, InputStream, EditorStream, ResultStream, OmnisharpStream } from './index';
import { Connection, TextUpdate, AutocompletionQuery } from '../models/index';
const ipc = electronRequire('electron').ipcRenderer;

@Injectable()
export class StreamManager {
    public ready: Promise<boolean>;
    constructor(
        input: InputStream,
        private query: QueryStream,
        private omnisharp: OmnisharpStream
    ) {
        let queryResolver = null;
        let omnisharpResolver = null;
        let readyResolver = null;
        this.ready = new Promise(done => readyResolver = done);
        const queryPromise = new Promise(done => queryResolver = done);
        const omnisharpPromise = new Promise(done => omnisharpResolver = done);
        query.once(msg => msg.name === EventName.ProcessReady, () => queryResolver());
        omnisharp.once(msg => msg.name === EventName.ProcessReady, () => omnisharpResolver());
        queryPromise.then(() => omnisharpPromise.then(() => {
            input.connect();
            readyResolver(true);
        }));
        ipc.on('application-event', this.applicationEventHandler);
    }

    public close() : Promise<boolean> {
        return new Promise<boolean>(done => {
            this.query.once(msg => msg.name === EventName.ProcessClosed, () => {
                ipc.send('application-event', 'close-query-engine');
                new Promise<boolean>(done2 => {
                    this.omnisharp.once(msg => msg.name === EventName.ProcessClosed, () => {
                        ipc.send('application-event', 'close-omnisharp');
                        done2(true);
                    });
                    this.omnisharp.stopServer();
                }).then(res => done(res));
            });
            this.query.stopServer();
        });
    }

    private applicationEventHandler = (event: any, msg: string) => {
        if (msg === 'close') {
            this.close();
        }
    }
}
