import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { LogService } from './log.service';
import config from '../config';

const child_process = electronRequire('child_process');
const ipc = electronRequire('electron').ipcRenderer;
const path = electronRequire('path');
const fs = electronRequire('fs');

const isProduction = MODE !== 'DEVELOPMENT';
// __dirname doesn't seem to work in bundle mode
const dirname = (isProduction ? path.normalize(process.resourcesPath + '/app') : path.dirname(path.dirname(__dirname)))
    .replace(/%20/g, ' '); // otherwise file fetch files on windows

@Injectable()
export class MonitorService {
    public omnisharpReady: Promise<boolean>;
    public queryReady: Promise<boolean>;
    
    private omnisharpResolver;
    private queryResolver;
    private checker;
        
    constructor(
        private http: Http,
        private logService: LogService
    ) {
        ipc.on('application-event', this.applicationEventHandler.bind(this));
        this.omnisharpReady = new Promise((res, err) => {
            this.omnisharpResolver = (arg) => {
                // some buffer time
                setTimeout(() => { res(arg); }, 500); 
            };
        });
        
        this.queryReady = new Promise((res, err) => {
            this.queryResolver = res;
        });

        this.start();
    }
    
    private applicationEventHandler(event: any, msg: string) {
        if (msg === 'close') {
            let queryCb = () => ipc.send('application-event', 'close-query-engine');
            let omniCb = () => ipc.send('application-event', 'close-omnisharp');
            this.http.get(this.action(config.omnisharpPort, 'stopserver')).subscribe(omniCb, omniCb);
            this.http.get(this.action(config.queryEnginePort, 'stopserver')).subscribe(queryCb, queryCb);
        }
    }
        
    private start() {
        console.log('monitor.service:starting');
        let queryParams = this.queryCmd();
        let omnisharpCmd = this.omnisharpCmd().cmd;
        
        this.http.get(this.action(config.omnisharpPort, 'checkreadystatus'))
            .subscribe(
                ok => {
                    this.omnisharpResolver(true);
                },
                error => {
                    this.startProcess(omnisharpCmd, { });
                    this.checkBackends(config.omnisharpPort);
                }
            );

        this.http.get(this.action(config.queryEnginePort, 'checkreadystatus'))
            .subscribe(
                ok => {
                    this.queryResolver(true);
                }, error => {
                    this.startProcess(queryParams.cmd, { cwd: queryParams.dir });
                    this.checkBackends(config.queryEnginePort);
                }
            );
    }
    
    private checkBackends(port: number) {
        this.http.get(this.action(port, 'checkreadystatus'))
            .subscribe(ok => {
                if (port === config.omnisharpPort) { this.omnisharpResolver(true); }
                if (port === config.queryEnginePort) { this.queryResolver(true); }
            }, error => {
                let lbl = port === config.omnisharpPort ? 'omnisharp' : 'query-engine';
                setTimeout(() => this.checkBackends(port), 250);
            });
    }
    
    private startProcess(cmd: string, options: any) {
        child_process.exec(cmd, options, (error: string, stdout: string, stderr: string) => {
            this.logService.log('stdout', stdout);
            this.logService.log('stderr', stderr);
            if (error !== null) {
                this.logService.log('error', error);
            }
        });
    }
    
    private action(port: number, name: string) {
        return `http://localhost:${port}/${name}`;
    }
    
    private queryCmd(): { dir: string, cmd: string } {
        let dir = isProduction ? `${dirname}/query` : dirname;
        let cmd = isProduction ? `"${dir}/linq-editor.exe"` :
            `"${config.dotnetDebugPath}" run `;
        return { dir, cmd };
    }
    
    private omnisharpCmd(): { dir: string, cmd: string } {
        let slnPath = `${process.env.LOCALAPPDATA}/LinqEditor/omnisharp`;
        let exePath = `"${dirname}/omnisharp/OmniSharp.exe"`;
        slnPath = slnPath.replace(/\//g, '\\');
        let cmd = `${exePath} -s ${slnPath} -p ${config.omnisharpPort}`;
        return { dir: dirname, cmd };
    }
}
