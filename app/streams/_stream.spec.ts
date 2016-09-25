import * as chai from 'chai';
import { expect } from 'chai';
import * as sinon from 'sinon';
import * as sinonChai from 'sinon-chai';
import { ReflectiveInjector, enableProdMode } from '@angular/core';
import { Http, XHRBackend, ConnectionBackend, BrowserXhr, ResponseOptions, 
    BaseResponseOptions, RequestOptions, BaseRequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { QueryMessage, OmnisharpMessage } from '../messages/index';
import { QueryStream, SessionStream, EditorStream, ResultStream, OmnisharpStream } from './index';
import config from '../config';
import { CodeCheckResult } from '../models';
import XSRFStrategyMock from '../test/xsrf-strategy-mock';
import { cSharpTestData, cSharpTestDataExpectedResult,
    cSharpTestDataExpectedCodeChecks, codecheckEditorTestData } from '../test/editor-testdata';
import replaySteps from '../test/replay-steps';
import * as uuid from 'node-uuid';
const http = electronRequire('http');
const backendTimeout = config.unitTestData.backendTimeout;

describe('[int-test] streams', function() {
    this.timeout(backendTimeout * 3000);
    let session: SessionStream = null;
    let editor: EditorStream = null;
    let result: ResultStream = null;
    let injector: ReflectiveInjector = null;
    let query: QueryStream = null;
    let omnisharp: OmnisharpStream = null;
    
    before(function() {
        chai.expect();
        chai.use(sinonChai);
        injector = ReflectiveInjector.resolveAndCreate([
            Http, BrowserXhr, XSRFStrategyMock,
            { provide: ConnectionBackend, useClass: XHRBackend },
            { provide: ResponseOptions, useClass: BaseResponseOptions },
            { provide: RequestOptions, useClass: BaseRequestOptions },
            QueryStream,
            SessionStream,
            EditorStream,
            ResultStream,
            OmnisharpStream
        ]);
        session = injector.get(SessionStream);
        editor = injector.get(EditorStream);
        result = injector.get(ResultStream);
        query = injector.get(QueryStream);
        omnisharp = injector.get(OmnisharpStream);
    });

    it('waits for backends to be ready', function(done) {
        this.timeout(backendTimeout);
        let queryReady = false;
        let omnisharpReady = false;
        query.once(msg => msg.type === 'ready', () => {
            queryReady = true;
            if (omnisharpReady) {
                done();
            }
        });
        omnisharp.once(msg => msg.type === 'ready', () => {
            omnisharpReady = true;
            if (queryReady) {
                done();
            }
        });
    });

    it('emits result and template messages for simple value expressions', function(done) {
        this.timeout(backendTimeout * (cSharpTestData.length + 1));
        let verifyCount = 0;
        cSharpTestData.forEach((testData, idx: number) => {
            // only a single page
            const expectedPage = cSharpTestDataExpectedResult[idx][0]; 
            const id = uuid.v4();
            let sawRunCodeResponse = false;
            let sawCodeTemplateResponse = false;
            // todo, if events isn't hot, nothing will happen in query, so we need to also
            // listen for something, so we check that we get no errors from the request
            query.once(msg => msg.type === 'run-code-response' && msg.id === id, msg => {
                expect(msg.response.code).to.equal(0);
                sawRunCodeResponse = true;
            });
            query.once(msg => msg.type === 'code-template-response' && msg.id === id, msg => {
                expect(msg.codeTemplate.namespace).to.not.be.empty;
                expect(msg.codeTemplate.lineOffset).to.not.be.undefined;
                expect(msg.codeTemplate.columnOffset).to.not.be.undefined;
                expect(msg.codeTemplate.template).to.contain(`namespace ${msg.codeTemplate.namespace}`);
                sawCodeTemplateResponse = true;
            });
            const resultSub = result.events
                .filter(msg => msg.id === id)
                .subscribe(msg => {
                    if (msg.type === 'done') {
                        resultSub.unsubscribe();
                        if (idx === cSharpTestData.length - 1) {
                            expect(verifyCount).to.equal(cSharpTestData.length);
                            expect(sawRunCodeResponse).to.be.true;
                            expect(sawCodeTemplateResponse).to.be.true;
                            done();
                        }
                    } else if (msg.type === 'update') {
                        expect(msg.data.id).to.equal(id);
                        expect(msg.data.title).to.equal(expectedPage.title);
                        expect(msg.data.columns).to.deep.equal(expectedPage.columns);
                        expect(msg.data.columnTypes).to.deep.equal(expectedPage.columnTypes);
                        expect(msg.data.rows).to.deep.equal(expectedPage.rows);
                        verifyCount++;
                    }
                }); 
            
            replaySteps([
                100, () => session.new(id),
                { 
                    for: testData.events,
                    wait: 100,
                    fn: (evt) => editor.edit(id, evt)
                },
                500, () => session.runCode(id)
            ]);
        });
    });

    it('emits codecheck messages for simple statement', function(done) {
        this.timeout(backendTimeout * 2);
        const firstEdits = codecheckEditorTestData[0].events.filter(x => x.time < 6000);
        const secondEdits = codecheckEditorTestData[0].events.filter(x => x.time >= 6000);
        const codechecks: OmnisharpMessage[] = [];
        const codecheckSub = omnisharp.events.filter(msg => msg.type === 'codecheck').subscribe(msg => {
            if (codechecks.length === 0) {
                // first check should cause an error, with the incomplete statement
                expect(msg.checks.length).to.equal(1);
                expect(msg.checks[0].logLevel).to.equal('Error');
            }
            if (codechecks.length === 1) {
                // second check should cause a warning about unused variables
                expect(msg.checks.length).to.equal(1);
                expect(msg.checks[0].logLevel).to.equal('Warning');
            }
            codechecks.push(msg);
            if (codechecks.length === 2) {
                done();
            }
        });

        const id = uuid.v4();
        const codecheckResults = [];
        replaySteps([
            100, () => session.new(id),
            {
                for: firstEdits,
                wait: 100,
                fn: (evt) => editor.edit(id, evt)
            },
            100, () => session.codeCheck(id),
            // give omnisharp some time since this is a first real op
            3000, () => { },
            {
                for: secondEdits,
                wait: 100,
                fn: (evt) => editor.edit(id, evt)
            },
            500, () => session.codeCheck(id)
        ]);
    });

    it('stops query process when stopServer is called', function(done) {
        this.timeout(backendTimeout);
        query.once(msg => msg.type === 'closed', () => {
            setTimeout(() => {
                let url = `http://localhost:${config.queryEnginePort}/checkreadystate`;
                http.get(url, res => { done(new Error('response received')); })
                    .on('error', () => { done(); });
            }, 500);
        });
        replaySteps([
            500, () => query.stopServer()
        ]);
    });

    it('stops omnisharp process when stopServer is called', function(done) {
        this.timeout(backendTimeout);
        omnisharp.once(msg => msg.type === 'closed', () => {
            setTimeout(() => {
                let url = `http://localhost:${config.omnisharpPort}/checkreadystate`;
                http.get(url, res => { done(new Error('response received')); })
                    .on('error', () => { done(); });
            }, 500);
        });
        replaySteps([
            500, () => omnisharp.stopServer()
        ]);
    });
});