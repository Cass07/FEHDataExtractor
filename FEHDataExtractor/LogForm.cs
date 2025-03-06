using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FEHDataExtractor
{
    public partial class LogForm : Form
    {
        private ListBox logListBox;
        private Button closeButton;
        private List<LogEntry> logEntries = new List<LogEntry>();

        public LogForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.logListBox = new System.Windows.Forms.ListBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logListBox
            // 
            this.logListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListBox.FormattingEnabled = true;
            this.logListBox.Location = new System.Drawing.Point(12, 12);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(558, 329);
            this.logListBox.TabIndex = 0;
            this.logListBox.DoubleClick += new System.EventHandler(this.logListBox_DoubleClick);

            // DrawMode�� OwnerDrawVariable�� �����ϰ� DrawItem �̺�Ʈ �ڵ鷯 �߰�
            this.logListBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.logListBox.DrawItem += new DrawItemEventHandler(this.logListBox_DrawItem);

            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(495, 347);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // LogForm
            // 
            this.ClientSize = new System.Drawing.Size(582, 382);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.logListBox);
            this.Name = "LogForm";
            this.Text = "Processing Log";
            this.ResumeLayout(false);

            // ���ؽ�Ʈ �޴� �߰�
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Log Entry");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);

            ToolStripMenuItem viewDetailsMenuItem = new ToolStripMenuItem("View Details");
            viewDetailsMenuItem.Click += ViewDetailsMenuItem_Click;
            contextMenu.Items.Add(viewDetailsMenuItem);

            logListBox.ContextMenuStrip = contextMenu;
        }

        // ListBox�� DrawItem �̺�Ʈ �ڵ鷯
        private void logListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // DrawItem �̺�Ʈ�� ��ȿ���� Ȯ��
            if (e.Index < 0 || e.Index >= logEntries.Count)
                return;

            // �������� ���õǾ����� ���ο� ���� ���� ����
            e.DrawBackground();

            // �ؽ�Ʈ ���� ���� - ������ ������, �Ϲ� �޽����� ������
            Brush brush = logEntries[e.Index].HasError ? Brushes.Red : Brushes.Black;

            // �ؽ�Ʈ �׸���
            e.Graphics.DrawString(
                logListBox.Items[e.Index].ToString(),
                e.Font,
                brush,
                e.Bounds);

            // ��Ŀ���� ������ ��Ŀ�� �簢�� �׸���
            e.DrawFocusRectangle();
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            if (logListBox.SelectedIndex >= 0 && logListBox.SelectedIndex < logEntries.Count)
            {
                LogEntry entry = logEntries[logListBox.SelectedIndex];
                string textToCopy = entry.DisplayText;

                if (!string.IsNullOrEmpty(entry.StackTrace))
                {
                    textToCopy += Environment.NewLine + Environment.NewLine + entry.StackTrace;
                }

                try
                {
                    Clipboard.SetText(textToCopy);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ViewDetailsMenuItem_Click(object sender, EventArgs e)
        {
            ShowDetailsDialog();
        }

        private void logListBox_DoubleClick(object sender, EventArgs e)
        {
            ShowDetailsDialog();
        }

        private void ShowDetailsDialog()
        {
            if (logListBox.SelectedIndex < 0 || logListBox.SelectedIndex >= logEntries.Count)
                return;

            LogEntry entry = logEntries[logListBox.SelectedIndex];
            Form detailsForm = new Form
            {
                Text = "Log Details",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = true,
                ShowIcon = false
            };

            TextBox detailsTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9.0f),
                WordWrap = false
            };

            Button copyButton = new Button
            {
                Text = "Copy All",
                Dock = DockStyle.Bottom,
                Height = 30
            };

            copyButton.Click += (s, e) =>
            {
                try
                {
                    Clipboard.SetText(detailsTextBox.Text);
                    MessageBox.Show("Copied to clipboard.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            string details = entry.DisplayText;

            if (!string.IsNullOrEmpty(entry.StackTrace))
            {
                details += Environment.NewLine + Environment.NewLine +
                    "--- Stack Trace ---" + Environment.NewLine + entry.StackTrace;
            }

            detailsTextBox.Text = details;
            detailsTextBox.ForeColor = Color.DarkSalmon;

            // ���� �޽����� ��� �ؽ�Ʈ�ڽ� �ؽ�Ʈ ���� ���������� ����
            if (entry.HasError)
            {
                detailsTextBox.ForeColor = Color.Red;
            }

            detailsForm.Controls.Add(detailsTextBox);
            detailsForm.Controls.Add(copyButton);

            detailsForm.ShowDialog(this);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // �α� �޽��� ������ Ŭ����
        private class LogEntry
        {
            public string DisplayText { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public DateTime Timestamp { get; set; }
            public bool HasError { get; set; }
        }

        public void AddLog(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AddLog), message);
                return;
            }

            string displayText = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}";

            LogEntry entry = new LogEntry
            {
                DisplayText = displayText,
                Message = message,
                StackTrace = null,
                Timestamp = DateTime.Now,
                HasError = false
            };

            logEntries.Add(entry);
            logListBox.Items.Add(displayText);
            logListBox.SelectedIndex = logListBox.Items.Count - 1;
        }

        public void AddLog(string message, string stackTrace)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string>(AddLog), message, stackTrace);
                return;
            }

            string displayText = $"[{DateTime.Now.ToString("HH:mm:ss")}] [ERROR] {message}";

            LogEntry entry = new LogEntry
            {
                DisplayText = displayText,
                Message = message,
                StackTrace = stackTrace,
                Timestamp = DateTime.Now,
                HasError = true
            };

            logEntries.Add(entry);
            logListBox.Items.Add(displayText);
            logListBox.SelectedIndex = logListBox.Items.Count - 1;
        }

        // ���� Form1.cs������ ȣ��� ȣȯ�� ������ ���� �޼���
        public void AddLog(string message, Exception ex)
        {
            AddLog(message, ex?.StackTrace);
        }
    }
}
