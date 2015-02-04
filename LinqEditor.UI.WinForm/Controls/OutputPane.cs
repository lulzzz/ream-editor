﻿using LinqEditor.Core.Models.Editor;
using LinqEditor.Core.Models.Analysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LinqEditor.UI.WinForm.Controls
{
    public class OutputPane : UserControl
    {
        RichTextBox _console;
        DataGridView _diagnosticsList;
        TabControl _tabControl;
        TabPage _consoleTab;
        TabPage _diagnosticsTab;

        // owned by Main form
        ToolStripStatusLabel _rowCountLabel;
        
        DataTable _diagnostics;
        List<TabPage> _cachedResultTabs;

        public OutputPane()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;
            _consoleTab = new TabPage("Console");
            _diagnostics = new DataTable();
            _diagnostics.Columns.AddRange(new[]{
                new DataColumn("Category", typeof(string)),
                new DataColumn("Location", typeof(string)),
                new DataColumn("Message", typeof(string))
            });
            _diagnosticsTab = new TabPage("Errors");
            _diagnosticsTab.ClientSizeChanged += TabClientSizeChangedHandler;
            _diagnosticsList = new DataGridView();
            _diagnosticsList.ReadOnly = true;
            _diagnosticsList.AllowUserToAddRows = false;
            _diagnosticsList.DataBindingComplete += DiagnosticsListDataBindingCompleteHandler;
            _diagnosticsList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _diagnosticsList.DataSource = _diagnostics;
            _diagnosticsTab.Controls.Add(_diagnosticsList);
            _tabControl.TabPages.AddRange(new[] { _consoleTab, _diagnosticsTab });
            _console = new RichTextBox();
            _console.Dock = DockStyle.Fill;
            _console.ReadOnly = true;
            _console.BackColor = System.Drawing.Color.White;
            _consoleTab.Controls.Add(_console);

            _cachedResultTabs = new List<TabPage>();

            SuspendLayout();
            Controls.Add(_tabControl);
            ResumeLayout();
        }

        public event Action<int> DisplayedRowCountUpdated;

        public void BindOutput(ExecuteResult result)
        {
            SuspendLayout();
            _tabControl.Enabled = false;
            
            if (result.Success)
            {
                _console.AppendText(string.Format("Query executed in {0} ms\n", result.Duration));
                _console.AppendText(result.QueryText.Trim() + "\n\n");
                BindResults(result.Tables);
                BindDiagnostics(new Error[]{}, result.Warnings);
                if (result.Tables.Count() > 0)
                {
                    _tabControl.SelectedTab = _tabControl.TabPages[1 + result.Tables.Count()];
                }
            }
            else
            {
                BindDiagnostics(result.Errors, result.Warnings);
                if (result.Warnings != null && result.Warnings.Count() > 0 ||
                    result.Errors != null && result.Errors.Count() > 0)
                {
                    _tabControl.SelectedTab = _tabControl.TabPages[1];
                }
                if (result.Exception != null)
                {
                    _console.AppendText("Exception:\n" + result.Exception.ToString() + "\n");
                }
                if (result.InternalException != null)
                {
                    _console.AppendText("InternalException:\n" + result.InternalException.ToString() + "\n");
                }
            }
            _tabControl.Enabled = true;
            _console.SelectionStart = _console.TextLength;
            _console.ScrollToCaret();
            ResumeLayout(false);
            PerformLayout();
        }

        private void BindResults(IEnumerable<DataTable> tables)
        {
            var prevCount = _tabControl.TabPages.Count - 2; // 2 permanent tabs
            var index = 0;

            foreach (var table in tables)
            {
                if (index + 1 > prevCount)
                {
                    // either make a new tab or reuse previous
                    TabPage newTab = null;
                    if (_cachedResultTabs.Count > 0)
                    {
                        newTab = _cachedResultTabs[0];
                        _cachedResultTabs.RemoveAt(0);
                    }
                    else
                    {
                        newTab = new TabPage();
                        newTab.ClientSizeChanged += TabClientSizeChangedHandler;
                        newTab.Enter += TabEnterHandler;
                        var grid = new DataGridView();
                        grid.ReadOnly = true;
                        grid.AllowDrop = false;
                        grid.AllowUserToAddRows = false;
                        grid.AllowUserToDeleteRows = false;
                        grid.AllowUserToOrderColumns = true;
                        grid.AllowUserToResizeColumns = true;
                        grid.AllowUserToResizeRows = false; // less dragging action going on
                        grid.DefaultCellStyle.NullValue = "null";
                        
                        grid.DataBindingComplete += GridDataBindingCompleteAutoSizeHandler;
                        grid.CellFormatting += GridCellFormatting;
                        newTab.Controls.Add(grid);
                    }
                    newTab.Text = table.TableName;
                    _tabControl.TabPages.Add(newTab);
                }
                var tab = _tabControl.TabPages[2 + index++];
                var tabGrid = tab.Controls[0] as DataGridView;
                tabGrid.DataSource = table;
            }
            for (var i = index; i < prevCount; i++)
            {
                _cachedResultTabs.Add(_tabControl.TabPages[2 + i]);
            }
            for (var i = index; i < prevCount; i++)
            {
                _tabControl.TabPages.Remove(_tabControl.TabPages[2 + i]);
            }
            
        }

        void GridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value == DBNull.Value)
            {
                e.CellStyle.ForeColor = Color.Silver;

            }
            e.CellStyle.SelectionBackColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(SystemColors.Highlight))); // jeez
            e.CellStyle.SelectionForeColor = SystemColors.ControlText;
            e.CellStyle.Font = new System.Drawing.Font("Verdana", 8);
        }

        private void BindDiagnostics(IEnumerable<Error> errors, IEnumerable<Warning> warnings)
        {
            _diagnostics.Rows.Clear();

            var content = (errors ?? new Error[]{}).Select(x => new
            {
                Category = "Error",
                Line = x.Location.StartLine,
                Column = x.Location.StartColumn,
                Location = string.Format("{0},{1}", x.Location.StartLine, x.Location.StartColumn),
                Message = x.Message
            }).Concat((warnings ?? new Warning[]{}).Select(z => new
            {
                Category = "Warning",
                Line = z.Location.StartLine,
                Column = z.Location.StartColumn,
                Location = string.Format("{0},{1}", z.Location.StartLine, z.Location.StartColumn),
                Message = z.Message
            })).OrderByDescending(y => y.Line).ThenByDescending(u => u.Column);

            foreach (var item in content)
            {
                _diagnostics.Rows.Add(new[] { item.Category, item.Location, item.Message });
            }
        }

        private void BindRowCount(int rows)
        {
            if (DisplayedRowCountUpdated != null)
            {
                DisplayedRowCountUpdated(rows);
            }
        }
        
        void TabClientSizeChangedHandler(object sender, EventArgs e)
        {
            var tab = sender as TabPage;
            var grid = tab.Controls[0] as DataGridView;
            grid.Height = tab.ClientSize.Height;
            grid.Width = tab.ClientSize.Width;
        }

        void TabEnterHandler(object sender, EventArgs e)
        {
            var tab = sender as TabPage;
            var grid = tab.Controls[0] as DataGridView;
            BindRowCount(grid.RowCount);
        }

        void DiagnosticsListDataBindingCompleteHandler(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var view = sender as DataGridView;
            view.Columns[0].Width = 80;
            view.Columns[1].Width = 65;
            view.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        void GridDataBindingCompleteAutoSizeHandler(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var grid = sender as DataGridView;
            var tab = grid.Parent as TabPage;
            var tabControl = tab.Parent as TabControl;
            var gridWidth = grid.Width;

            for (var i = 0; i < grid.Columns.Count; i++)
            {
                //var mode = DataGridViewAutoSizeColumnMode.AllCells; 
                //if (i == grid.Columns.Count - 1) {
                //    var previousColWidth = 0;
                //    for (var j = 0; j < grid.Columns.Count - 1; j++ ) 
                //    {
                //        previousColWidth += grid.Columns[j].Width;
                //    }
                //    // assume the last cell needs on average the same as the rest.
                //    if (previousColWidth + (previousColWidth / (grid.Columns.Count - 1)) < gridWidth)
                //    {
                //        mode = DataGridViewAutoSizeColumnMode.Fill;
                //    }
                //}
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (var i = 0; i < grid.Columns.Count; i++)
            {
                var width = grid.Columns[i].Width;
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns[i].Width = width;
            }

            // if the tab shown doesn't change, no enter event is fired, so check and update here
            if (tabControl.SelectedTab == tab)
            {
                BindRowCount(grid.RowCount);
            }
        }
    }
}
