import { Http, Response } from '@angular/http';
import { Subject, Observable } from 'rxjs/Rx';
import { EventName, Message } from './api';
const child_process = electronRequire('child_process');
const ipc = electronRequire('electron').ipcRenderer;
const path = electronRequire('path');

const REAMQUERY_BASEDIR = path.normalize(`${process.cwd()}/query/query/src/ReamQuery`);
type ProcessType = 'omnisharp' | 'query';

export class ProcessStream {
    public status: Subject<Message> = new Subject<Message>();
    private options: any;
    private command: string;
    private directory: string;
    private httpPort: number;
    private cancelCheck: boolean = false;
    constructor(private http: Http) { }

    public start(processType: ProcessType, command: string, directory: string, httpPort: number) {
        this.command = command;
        this.directory = directory;
        this.httpPort = httpPort;
        // this.options = processType === 'query' ? { cwd: directory } : { };
        this.options = processType === 'omnisharp' ? {} : { cwd: directory };
        if (processType === 'query') {
            process.env['REAMQUERY_BASEDIR'] = REAMQUERY_BASEDIR;
        }
        this.status.next(new Message(EventName.ProcessStarting));
        let start = new Date().getTime();
        let statusSub = this.status.subscribe(msg => {
            if (msg.name === EventName.ProcessFailed) {
                this.cancelCheck = true;
            }
        });
        child_process.exec(this.command, this.options, (error: string, stdout: string, stderr: string) => {
            if (error) { // expected when starting command fails, otherwise only stdout/err should be filled
                console.error('process.stream error', error);
                console.error('stdout', stdout);
                console.error('stderr', stderr);
                this.status.next(new Message(EventName.ProcessFailed));
            } else {
                this.status.next(new Message(EventName.ProcessClosed));
            }
            setTimeout(() => {
                statusSub.unsubscribe();
            }, 100);
        });
        this.checkreadystatus();
    }

    public close() {
        this.status.next(new Message(EventName.ProcessClosing));
        this.http.get(this.action('stopserver')).subscribe();
    }

    private checkreadystatus() {
        this.http.get(this.action('checkreadystatus'))
            .subscribe(ok => {
                this.cancelCheck = true;
                this.status.next(new Message(EventName.ProcessReady));
            }, error => {
                if (!this.cancelCheck) {
                    setTimeout(() => { this.checkreadystatus(); }, 500);
                }
            });
    }
    
    private action(name: string) {
        return `http://localhost:${this.httpPort}/${name}`;
    }
}
