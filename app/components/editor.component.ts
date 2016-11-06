import { Component, AfterViewInit, AfterViewChecked, ElementRef, Input } from '@angular/core';
import { TabService } from '../services/index';
import { InputStream } from '../streams/index';
import { TextUpdate } from '../models/index';
import * as CodeMirror from 'codemirror';
import 'codemirror/mode/clike/clike';

@Component({
    selector: 'rm-editor',
    template: `
<div class="rm-editor">
    <textarea></textarea>
</div>
`
})
export class EditorComponent implements AfterViewInit, AfterViewChecked {
    documents: any = {};
    sessionId: string;
    @Input('editor-props') public editorProps: any;
    private mirror: CodeMirror.Editor;
    private prevHeight: number;
    private firstLoad = true;
    constructor(
        private tabs: TabService,
        private input: InputStream,
        private elm: ElementRef
    ) {
        tabs.tabDragging.subscribe(dragging => {
            this.mirror.setOption('dragDrop', !dragging);
        });
    }

    ngAfterViewInit() {
        const txtElm = this.elm.nativeElement.querySelector('textarea');
        this.mirror = CodeMirror.fromTextArea(txtElm, this.editorOptions());
        this.mirror.on('change', this.changeHandler);

        this.tabs.currentSessionId.subscribe(id => {
            let doc = null;
            if (!this.documents[id]) {
                doc = CodeMirror.Doc('');
                this.documents[id] = doc;
            }
            doc = this.documents[id];
            const oldDoc = this.mirror.swapDoc(doc);
            this.mirror.setOption('mode', 'text/x-csharp');
            this.mirror.setOption('sessionId', id);
            this.documents[this.sessionId] = oldDoc;
            this.sessionId = id;
            requestAnimationFrame(() => {
                if (this.firstLoad) {
                    this.mirror.refresh();
                    this.firstLoad = false;
                    requestAnimationFrame(() => this.mirror.focus());
                } else {
                    this.mirror.focus()
                }
            });
        });
    }

    ngAfterViewChecked() {
        const elm = this.elm.nativeElement.querySelector('.CodeMirror');
        if (elm) {
            if (this.editorProps.height !== this.prevHeight) {
                elm.style.height = this.editorProps.height + 'px';
                this.prevHeight = this.editorProps.height;
                this.mirror.refresh();
            }
        }
    }

    private changeHandler = (mirror: CodeMirror.Editor, event: CodeMirror.EditorChange): void => {
        const sessionId = mirror.getOption('sessionId');
        this.input.edit(sessionId, <TextUpdate> {
            text: event.text,
            removed: event.removed,
            from: event.from,
            to: event.to
        });
    };


    private editorOptions() {
        return {
            lineNumbers: true,
            gutters: ['CodeMirror-lint-markers'],
            lint: true,
            smartIndent: false,
            showCursorWhenSelecting: true,
            mode: 'text/x-csharp'
        };
    }
}
