﻿using LinqEditor.Core.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqEditor.UI.WinForm.Forms
{
    public class ConnectionManager : Form
    {
        class BasicComboBoxItem
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }

            public override string ToString()
            {
                return string.Format("{0} {1}", Name, ConnectionString);
            }
        }

        // http://stackoverflow.com/questions/11873378/adding-placeholder-text-to-textbox
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        
        ISchemaStore _schemaStore;

        public ConnectionManager(ISchemaStore schemaStore)
        {
            _schemaStore = schemaStore;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var width = 380;
            var height = 480;
            var font = new Font("Consolas", 10);

            var layoutEdit = new FlowLayoutPanel();
            var editSelector = new ComboBox();
            var editRuler = new Label();
            var editHeader = new Label();
            var editConnStr = new TextBox();
            var editTitle = new TextBox();
            var editType = new TextBox();
            var deleteBtn = new Button();
            var saveBtn = new Button();

            var layoutAdd = new FlowLayoutPanel();
            var addRuler = new Label();
            var addHeader = new Label();
            var addDescription = new Label();
            var addConnStr = new TextBox();
            var addTitle = new TextBox();
            var addType = new ComboBox();
            var addButton = new Button();

            Action bindConnections = () =>
            {
                editSelector.Items.Clear();
                foreach (string connStr in _schemaStore.ConnectionNames.Keys)
                {
                    editSelector.Items.Add(new BasicComboBoxItem { Name = _schemaStore.ConnectionNames[connStr], ConnectionString = connStr });
                }
                editSelector.Enabled = editSelector.Items.Count > 0;
            };

            layoutEdit.SuspendLayout();
            layoutAdd.SuspendLayout();
            SuspendLayout();

            Width = width;
            Height = height;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new System.Windows.Forms.Padding() { All = 10 };
            Text = "Connection manager";


            editRuler.AutoSize = false;
            editRuler.Height = 2;
            editRuler.Text = "";
            editRuler.BorderStyle = BorderStyle.Fixed3D;
            editRuler.Location = new Point(0, 20);
            
            editHeader.Text = "Existing connections";
            editHeader.Width = 105;
            editHeader.Location = new Point(20, 14);

            
            editType.Enabled = false;
            editType.Width = width / 4;
            editTitle.Enabled = false;
            editConnStr.Enabled = false;
            editConnStr.Height = 75;
            editConnStr.Multiline = true;
            editConnStr.WordWrap = true;
            editConnStr.Font = font;

            deleteBtn.Text = "Delete";
            deleteBtn.Enabled = false;
            deleteBtn.Click += delegate 
            {
                var obj = editSelector.SelectedItem as BasicComboBoxItem;
                _schemaStore.DeleteConnectionString(obj.ConnectionString);
                editConnStr.Text = "";
                editConnStr.Enabled = false;
                editType.Text = "";
                editType.Enabled = false;
                editTitle.Text = "";
                editTitle.Enabled = false;
                deleteBtn.Enabled = false;
                saveBtn.Enabled = false;
                bindConnections();
            };
            
            saveBtn.Text = "Save";
            saveBtn.Enabled = false;

            editSelector.Dock = DockStyle.Top;
            editSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            editSelector.SelectedValueChanged += delegate(object sender, EventArgs args)
            {
                var list = sender as ComboBox;
                var selected = list.SelectedItem as BasicComboBoxItem;
                if (selected != null)
                {
                    editConnStr.Text = selected.ConnectionString;
                    editConnStr.Enabled = true;
                    editTitle.Text = selected.Name;
                    editTitle.Enabled = true;
                    editType.Text = "SQL Server";
                    saveBtn.Enabled = true;
                    deleteBtn.Enabled = true;
                } 
            };

            layoutEdit.BackColor = Color.Transparent;
            layoutEdit.FlowDirection = FlowDirection.LeftToRight;
            layoutEdit.Location = new Point(15, 35);
            layoutEdit.Height = 165;

            /////////////////////////////////////////////////////////////

            var addOffset = 220;

            addRuler.AutoSize = false;
            addRuler.Height = 2;
            addRuler.Text = "";
            addRuler.BorderStyle = BorderStyle.Fixed3D;
            addRuler.Location = new Point(0, addOffset);
            
            addHeader.Text = "Add connection";
            addHeader.Width = 85;
            addHeader.Location = new Point(20, addOffset - 7);

            
            addType.Width = width / 4;
            addType.DropDownStyle = ComboBoxStyle.DropDownList;
            addType.Items.Add("SQL Server");
            addType.SelectedItem = addType.Items[0];
            addType.Enabled = false;

            layoutAdd.BackColor = Color.Transparent;
            layoutAdd.FlowDirection = FlowDirection.LeftToRight;
            layoutAdd.Location = new Point(15, addOffset + 15);
            layoutAdd.Height = 160;

            
            addDescription.Text = "Enter a connection string and an optional display title";

            addConnStr.Multiline = true;
            addConnStr.WordWrap = true;
            addConnStr.Height = 75;
            addConnStr.Font = font;

            addButton.Text = "Add";
            addButton.Enabled = false;
            addButton.Click += delegate
            {
                _schemaStore.AddConnectionString(addConnStr.Text, addTitle.Text);
                addConnStr.Text = string.Empty;
                addTitle.Text = string.Empty;
                addButton.Enabled = false;
                bindConnections();
            };

            addConnStr.TextChanged += delegate 
            {
                addButton.Enabled = addConnStr.Text.Length > 0;
            };

            ///////////////////////////////////////////////////////////////

            var lastOffset = 405;
            var lastRuler = new Label();
            lastRuler.AutoSize = false;
            lastRuler.Height = 2;
            lastRuler.Text = "";
            lastRuler.BorderStyle = BorderStyle.Fixed3D;
            lastRuler.Location = new Point(0, lastOffset);

            var closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Location = new Point(width - 95, lastOffset + 15);
            closeButton.Click += delegate { Hide(); };
            AcceptButton = closeButton;

            layoutEdit.ClientSizeChanged += delegate
            {
                editSelector.Width = layoutEdit.ClientSize.Width - 15;
                editConnStr.Width = layoutEdit.ClientSize.Width - 15;
                editTitle.Width = layoutEdit.ClientSize.Width - 15;
            };
            layoutEdit.Width = 200;

            layoutAdd.ClientSizeChanged += delegate
            {
                addDescription.Width = layoutAdd.ClientSize.Width - 15;
                addConnStr.Width = layoutAdd.ClientSize.Width - 15;
                addTitle.Width = layoutAdd.ClientSize.Width - 15;
            };
            
            ClientSizeChanged += delegate 
            { 
                editRuler.Width = ClientSize.Width + 20;
                addRuler.Width = ClientSize.Width + 20;
                lastRuler.Width = ClientSize.Width + 20;
                layoutAdd.Width = ClientSize.Width - 20;
                layoutEdit.Width = ClientSize.Width - 20;
            };

            layoutEdit.Controls.AddRange(new Control[] { editSelector, editConnStr, editTitle, editType, deleteBtn, saveBtn });
            layoutAdd.Controls.AddRange(new Control[] { addDescription, addConnStr, addTitle, addType, addButton });
            Controls.AddRange(new Control[] { editHeader, editRuler, layoutEdit, addHeader, addRuler, layoutAdd, lastRuler, closeButton });

            layoutAdd.ResumeLayout(false);
            layoutAdd.PerformLayout();
            layoutEdit.ResumeLayout(false);
            layoutEdit.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

            bindConnections();
        }

    }
}