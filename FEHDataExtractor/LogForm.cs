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

            // DrawMode를 OwnerDrawVariable로 설정하고 DrawItem 이벤트 핸들러 추가
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

            // 컨텍스트 메뉴 추가
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Log Entry");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);

            ToolStripMenuItem viewDetailsMenuItem = new ToolStripMenuItem("View Details");
            viewDetailsMenuItem.Click += ViewDetailsMenuItem_Click;
            contextMenu.Items.Add(viewDetailsMenuItem);

            logListBox.ContextMenuStrip = contextMenu;
        }

        // ListBox의 DrawItem 이벤트 핸들러
        private void logListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // DrawItem 이벤트가 유효한지 확인
            if (e.Index < 0 || e.Index >= logEntries.Count)
                return;

            // 아이템이 선택되었는지 여부에 따라 배경색 설정
            e.DrawBackground();

            // 텍스트 색상 설정 - 오류는 빨간색, 일반 메시지는 검은색
            Brush brush = logEntries[e.Index].HasError ? Brushes.Red : Brushes.Black;

            // 텍스트 그리기
            e.Graphics.DrawString(
                logListBox.Items[e.Index].ToString(),
                e.Font,
                brush,
                e.Bounds);

            // 포커스가 있으면 포커스 사각형 그리기
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

            // 오류 메시지인 경우 텍스트박스 텍스트 색상도 빨간색으로 설정
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

        // 로그 메시지 데이터 클래스
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

        // 기존 Form1.cs에서의 호출과 호환성 유지를 위한 메서드
        public void AddLog(string message, Exception ex)
        {
            AddLog(message, ex?.StackTrace);
        }
    }
}
