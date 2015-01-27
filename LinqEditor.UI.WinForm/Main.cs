﻿using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Installer;
using LinqEditor.Core.Backend.Models;
using LinqEditor.Core.Backend.Repository;
using LinqEditor.Core.CodeAnalysis.Editor;
using LinqEditor.Core.CodeAnalysis.Models;
using LinqEditor.UI.WinForm.Controls;
using ScintillaNET;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqEditor.UI.WinForm
{
    public class Main : Form
    {
        public static string TestConnectionString = "Data Source=.\\sqlexpress;Integrated Security=True;Initial Catalog=Opera18500DB";

        ToolStrip Toolbar;
        SplitContainer MainContainer;
        ToolStripButton executeButton;
        TabControl TabControl;
        TabPage ConsoleTab;
        TabPage DiagnosticsTab;
        DataGridView DiagnosticsDataGrid;
        DataTable DiagnosticsDataTable;
        StatusStrip StatusBar;
        ToolStripStatusLabel EditorStatusBarLabel;

        TextBox ConnectionTextBox;
        List<TabPage> ResultTabs;
        CodeEditor Editor;
        RichTextBox Console;

        IBackgroundSession ConnectionSession;
        IBackgroundCompletion CompletionHelper;
        IWindsorContainer IOCContainer;

        private void InitializeContainer()
        {
            IOCContainer = new WindsorContainer();
            IOCContainer.Install(FromAssembly.This());

            ConnectionSession = IOCContainer.Resolve<IBackgroundSession>(new Arguments(new { connectionString = ConnectionTextBox.Text }));
            CompletionHelper = IOCContainer.Resolve<IBackgroundCompletion>();
        }

        public static Main Create()
        {
            return new Main();
        }

        public Main()
        {
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Text = "Linq Editor";
            Width = 800;
            Height = 500;
            FormClosed += Main_FormClosed;

            ResultTabs = new List<TabPage>();

            this.Load += Main_Load;
            
            MainContainer = new SplitContainer();
            MainContainer.Orientation = Orientation.Horizontal;
            MainContainer.Dock = DockStyle.Fill;
            MainContainer.TabStop = false;
            MainContainer.SizeChanged += MainContainer_SizeChanged;

            // status
            StatusBar = new StatusStrip();
            StatusBar.Dock = DockStyle.Bottom;
            StatusBar.GripStyle = ToolStripGripStyle.Hidden;
            StatusBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            EditorStatusBarLabel = new ToolStripStatusLabel();
            
            // connection
            ConnectionTextBox = new TextBox();
            ConnectionTextBox.Dock = DockStyle.Top;
            ConnectionTextBox.ReadOnly = true;
            ConnectionTextBox.Text = TestConnectionString;

            // scintilla
            Editor = new CodeEditor();
            Editor.Dock = DockStyle.Fill;

            // toolbar
            Toolbar = new ToolStrip();
            Toolbar.Dock = DockStyle.Top;
            
            // play button
            executeButton = new ToolStripButton();
            executeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            executeButton.Image = Icons.play;
            executeButton.Click += executeButton_Click;
            executeButton.Enabled = false;

            Toolbar.Items.Add(executeButton);

            // tabControl
            TabControl = new TabControl();
            TabControl.Dock = DockStyle.Fill;
            ConsoleTab = new TabPage("Console");
            DiagnosticsDataTable = new DataTable();
            DiagnosticsDataTable.Columns.AddRange(new []{
                new DataColumn("Category", typeof(string)),
                new DataColumn("Location", typeof(string)),
                new DataColumn("Message", typeof(string))
            });

            DiagnosticsTab = new TabPage("Errors");
            DiagnosticsTab.ClientSizeChanged += TabClientSizeChanged;
            DiagnosticsDataGrid = new DataGridView();
            DiagnosticsDataGrid.ReadOnly = true;
            DiagnosticsDataGrid.AllowUserToAddRows = false;
            DiagnosticsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DiagnosticsDataGrid.DataBindingComplete += DiagnosticsDataGrid_DataBindingComplete;
            DiagnosticsDataGrid.DataSource = DiagnosticsDataTable;
            DiagnosticsTab.Controls.Add(DiagnosticsDataGrid);
            TabControl.TabPages.AddRange(new[] { ConsoleTab, DiagnosticsTab });

            Console = new RichTextBox();
            Console.Dock = DockStyle.Fill;
            Console.ReadOnly = true;
            Console.BackColor = Color.White;
            ConsoleTab.Controls.Add(Console);

            // add controls
            StatusBar.SuspendLayout();
            SuspendLayout();
            MainContainer.Panel1.Controls.Add(Editor);
            MainContainer.Panel1.Controls.Add(ConnectionTextBox);
            MainContainer.Panel2.Controls.Add(TabControl);

            Controls.Add(MainContainer);
            Controls.Add(Toolbar);
            Controls.Add(StatusBar);

            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();

            ResumeLayout();

            InitializeContainer();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                executeButton.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async Task<ExecuteResult> Execute()
        {
            return await ConnectionSession.ExecuteAsync(Editor.SourceCode());
        }

        private void BindResults(IEnumerable<DataTable> tables)
        {
            SuspendLayout();
            foreach (var tab in ResultTabs)
            {
                TabControl.TabPages.Remove(tab);
            }
            var newTabs = new List<TabPage>();
            foreach (var table in tables)
            {
                var tab = new TabPage(table.TableName);
                tab.ClientSizeChanged += TabClientSizeChanged;

                var grid = new DataGridView();
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.DataBindingComplete += GridDataBindingCompleteAutoSize;
                grid.DataSource = table;
                tab.Controls.Add(grid);
                newTabs.Add(tab);
            }
            ResultTabs = newTabs;
            TabControl.TabPages.AddRange(ResultTabs.ToArray());
            ResumeLayout();
        }

        private void BindDiagnosticsDataTable(IEnumerable<Error> errors, IEnumerable<Warning> warnings)
        {
            DiagnosticsDataTable.Rows.Clear();

            var content = errors.Select(x => new
            {
                Category = "Error",
                Line = x.Location.StartLine,
                Column = x.Location.StartColumn,
                Location = string.Format("{0},{1}", x.Location.StartLine, x.Location.StartColumn),
                Message = x.Message
            }).Concat(warnings.Select(z => new
            {
                Category = "Warning",
                Line = z.Location.StartLine,
                Column = z.Location.StartColumn,
                Location = string.Format("{0},{1}", z.Location.StartLine, z.Location.StartColumn),
                Message = z.Message
            })).OrderByDescending(y => y.Line).ThenByDescending(u => u.Column);

            foreach (var item in content)
            {
                DiagnosticsDataTable.Rows.Add(new[] { item.Category, item.Location, item.Message });
            }
        }

        void DiagnosticsDataGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var view = sender as DataGridView;
            view.Columns[0].Width = 80;
            view.Columns[1].Width = 65;
            view.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        void GridDataBindingCompleteAutoSize(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var view = sender as DataGridView;
            for (var i = 0; i < view.Columns.Count; i++)
            {
                view.Columns[i].AutoSizeMode = i < view.Columns.Count - 1 ?
                    DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        void TabClientSizeChanged(object sender, EventArgs e)
        {
            var tab = sender as TabPage;
            var grid = tab.Controls[0] as DataGridView;
            grid.Height = tab.ClientSize.Height;
            grid.Width = tab.ClientSize.Width;
        }

        void MainContainer_SizeChanged(object sender, EventArgs e)
        {
            var container = sender as SplitContainer;
            ConnectionTextBox.Width = container.ClientSize.Width;
        }

        void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            IOCContainer.Release(ConnectionSession);
            IOCContainer.Release(CompletionHelper);
            IOCContainer.Dispose();
        }

        async void Main_Load(object sender, EventArgs e)
        {
            var result = await ConnectionSession.InitializeAsync(ConnectionTextBox.Text);
            // init the completion helper with schema data
            await CompletionHelper.InitializeAsync(result.AssemblyPath, result.SchemaNamespace);
            Console.AppendText("Completion Initialized\n");
            // passes helper to editor control
            Editor.CompletionHelper = CompletionHelper; 
            executeButton.Enabled = true;
        }

        async void executeButton_Click(object sender, EventArgs e)
        {
            var btn = sender as ToolStripButton;
            btn.Enabled = false;
            var result = await Execute();
            if (result.Success)
            {
                Console.AppendText("Query Text\n");
                Console.AppendText(result.QueryText);
                BindResults(result.Tables);
                BindDiagnosticsDataTable(new Error[]{}, result.Warnings);

                if (ResultTabs.Count > 0)
                {
                    // todo: fix this
                    TabControl.SelectedTab = TabControl.TabPages[TabControl.TabPages.Count - 1];
                }
            }
            else
            {
                BindDiagnosticsDataTable(result.Errors, result.Warnings);
                if (result.Warnings != null && result.Warnings.Count() > 0 ||
                    result.Errors != null && result.Errors.Count() > 0)
                {
                    TabControl.SelectedTab = TabControl.TabPages[1];
                }
            }
            btn.Enabled = true;
        }
    }
}
