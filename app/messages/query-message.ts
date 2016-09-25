import { WebSocketMessage } from './web-socket-message';
import { BaseResponse, TemplateResponse } from '../streams/interfaces';

type eventType = 'starting' | 'failed' | 'ready' | 'closing' | 'closed' | 'message' | 'run-code-response' | 'code-template-response';
export class QueryMessage {
    constructor(
        public type: eventType,
        public id: string = null,
        public socket: WebSocketMessage = null,
        public response: BaseResponse = null,
        public template: TemplateResponse = null
    ) {}
}
