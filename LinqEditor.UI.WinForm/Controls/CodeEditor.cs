﻿using LinqEditor.Core.Session;
using LinqEditor.Core.Models.Analysis;
using LinqEditor.UI.WinForm.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using LinqEditor.Core.Helpers;
using System.Drawing;

// Margin width auto adjustable (line numbs)
// https://scintillanet.codeplex.com/discussions/267720

namespace LinqEditor.UI.WinForm.Controls
{
    public class CodeEditor : UserControl, IDisposable
    {
        const string Zws = "\u200B";
        const int TimerTickMs = 100;
        const int AnalyzeTickMs = 1000;
        const int NotReadyTickMs = 50;
        Font EditorFont = new Font("Consolas", 10);

        public readonly char[] StopCharacters = new[] { ' ', '=', '(', ')', ';', '\n' };

        ScintillaNET.Scintilla _editor;
        ScintillaNET.INativeScintilla _editorNative;
        IAsyncSession _session;
        IAsyncSessionFactory _sessionFactory;
        ToolTip2 _tooltip;
        Timer _tipTimer;
        int _tipWord;
        int _textLineHeight;
        bool _textUpdated = true; // sets analyser timer going
        Timer _analyzeTimer;

        public string SourceCode { get {

            return _editor.Text.Replace(Zws, "");
        }
        }

        public CodeEditor(IAsyncSessionFactory sessionFactory, ToolTip2 tooltip)
        {
            _sessionFactory = sessionFactory;
            _tooltip = tooltip;
            InitializeComponent();
        }

        public void Session(Guid id)
        {
            if (_session != null)
            {
                _sessionFactory.Release(_session);
            }

            _session = _sessionFactory.Create(id);
        }

        bool CursorInside
        {
            get
            {
                var pos = _editor.PointToClient(MousePosition);
                return pos.X >= 0 && pos.X < _editor.Width &&
                    pos.Y >= 0 && pos.Y < _editor.Height - 4; // splitter seems included?
            }
        }

        private void InitializeComponent()
        {
            // set editor focus on tab changes
            VisibleChanged += delegate 
            {
                if (Visible)
                {
                    _tipTimer.Start();
                    _analyzeTimer.Start();
                    _tooltip.CurrentOwner = this;
                    _editor.Focus();
                }
                else
                {
                    _tipTimer.Stop();
                    _analyzeTimer.Stop();
                }
            };

            _editor = new ScintillaNET.Scintilla();
            _editorNative = _editor as ScintillaNET.INativeScintilla;
            var initEditor = _editor as ISupportInitialize;
            initEditor.BeginInit();
            _editor.Dock = DockStyle.Fill;
            _editor.ConfigurationManager.Language = "C#";
            _editor.Margins[1].Width = 0; // not sure what this is
            _editor.MatchBraces = true;
            _editor.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
            _editor.Caret.Width = 2;
            _editor.Font = EditorFont;
            _editor.Name = "_scintilla";
            _editor.TabIndex = 0;
            _editor.CharAdded += _editor_CharAdded;
            
            _editor.TextChanged += delegate
            {
                _textUpdated = true;
            };

            // https://scintillanet.codeplex.com/discussions/538501
            _editorNative.SetMouseDwellTime(TimerTickMs);
            _editor.DwellStart += delegate(object sender, ScintillaNET.ScintillaMouseEventArgs e)
            {
                var mousePos = _editor.PointToClient(MousePosition);
                var charPos = _editorNative.PositionFromPointClose(mousePos.X, mousePos.Y);
                
                if (_editor.CharAt(charPos) == default(char) ||
                    charPos >= 0 && StopCharacters.Contains(_editor.CharAt(charPos)))
                {
                    charPos = -1;
                }
                else
                {
                    charPos = _editorNative.WordStartPosition(charPos, true);
                }

                _tipWord = charPos;
            };

            initEditor.EndInit();

            SuspendLayout();
            Controls.Add(_editor);
            ResumeLayout();

            // https://scintillanet.codeplex.com/discussions/75490
            var imageList = new ImageList();
            // same order as enum order
            imageList.Images.Add(Resources.Types.FieldIcon);
            imageList.Images.Add(Resources.Types.Property_501);
            imageList.Images.Add(Resources.Types.Method_636);
            imageList.Images.Add(Resources.Types.ExtensionMethod_9571);
            
            _editor.AutoComplete.RegisterImages(imageList, System.Drawing.Color.Magenta);
            _editor.AutoComplete.MaxHeight = 10;
            _editor.ConfigurationManager.Language = "cs";

            Load += delegate
            {
                // todo: runtime font-size changes
                _textLineHeight = _editorNative.TextHeight(0);
                foreach (var key in _editor.Lexing.StyleNameMap)
                {
                    _editor.Styles[key.Value].Font = EditorFont;
                    _editor.Styles[key.Value].FontName = EditorFont.Name;
                }
            };

            // timer handles reading changes to the currently hovered word (defined by scintilla.)
            // when the word changes, the timer attempts to get tooltip info for the new word.
            _tipTimer = new Timer();
            _tipTimer.Interval = TimerTickMs;
            _tipTimer.Tick += tipTick;

            _analyzeTimer = new Timer();
            _analyzeTimer.Interval = NotReadyTickMs;
            _analyzeTimer.Tick += async delegate
            {
                _analyzeTimer.Stop();
                if (_textUpdated && _session != null)
                {
                    _textUpdated = false;
                    var result = await _session.AnalyzeAsync(_editor.Text);
                    if (result.Context == UserContext.NotReady) 
                    {
                        _textUpdated = true; // retry
                    }
                    else
                    {
                        // first time we hit this branch we completed the analysis
                        _analyzeTimer.Interval = AnalyzeTickMs;
                    }

                    DrawErrors(result.Errors);
                }
                _analyzeTimer.Start();
            };
        }

        void DrawErrors(IEnumerable<Error> errors)
        {
            
            _editor.Indicators[0].Style = ScintillaNET.IndicatorStyle.Squiggle;
            _editor.Indicators[0].Color = Color.Red;
            _editor.GetRange().ClearIndicator(0); 
            foreach (var err in errors)
            {
                var offset = err.Location.StartIndex == err.Location.EndIndex ? 1 : 0;
                var range = _editor.GetRange(err.Location.StartIndex - offset, err.Location.EndIndex);
                range.SetIndicator(0);
            }
        }

        async void _editor_CharAdded(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            if (_session == null) return;

            if (e.Ch == '.')
            {
                var result = await _session.AnalyzeAsync(SourceCode, _editor.CurrentPos - 1);
                if (result.Context == UserContext.MemberCompletion)
                {
                    _editor.AutoComplete.FillUpCharacters = "";
                    _editor.AutoComplete.List = GetAutoCompleteList(result.MemberCompletions);
                    _editor.AutoComplete.Show();
                }
            }

            // scintilla keeps refiring dwell events, so the tooltip will not reshow untill 
            // the mouse has been moved from the location the tip was killed.
            _tooltip.HideTip(); 
        }

        async void tipTick(object sender, EventArgs e)
        {
            _tipTimer.Stop();
            // handle tooltip logic
            var inside = CursorInside;
            var current = _tipWord;
            if (_session != null && current > -1 && inside && !_tooltip.Showing)
            {
                var end = _editorNative.WordEndPosition(current, true);
                var startX = _editorNative.PointXFromPosition(current);
                var startY = _editorNative.PointYFromPosition(current);
                var endX = _editorNative.PointXFromPosition(end);
                var endY = _editorNative.PointYFromPosition(end);

                var analysisResult = await _session.AnalyzeAsync(SourceCode, current);

                if (current == _tipWord && analysisResult.Context == UserContext.ToolTip)
                {
                    var startPos = _editor.PointToScreen(new System.Drawing.Point(startX, startY));
                    var endPos = _editor.PointToScreen(new System.Drawing.Point(endX, endY));
                    _tooltip.PlaceTip(startPos, endPos, _textLineHeight, analysisResult.ToolTip);
                }
            }
            else if (!inside)
            {
                _tipWord = -1;
            }
            _tipTimer.Start();
        }

        private List<string> GetAutoCompleteList(IEnumerable<CompletionEntry> suggestions)
        {
            return suggestions.Select(x => string.Format("{0}?{1}", x.Value, (int)x.Kind)).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            _tipTimer.Dispose();
            _analyzeTimer.Dispose();
            if (_session != null)
            {
                _sessionFactory.Release(_session);
            }
            base.Dispose(disposing);
        }
    }
}
