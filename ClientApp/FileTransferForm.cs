using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ClientApp.Services;

namespace ClientApp
{
    public partial class FileTransferForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Label serverLabel;
        private TextBox serverTextBox;
        private Label portLabel;
        private TextBox portTextBox;
        private Button connectButton;
        private Label statusLabel;
        
        private Label fileLabel;
        private TextBox filePathTextBox;
        private Button browseButton;
        private Label fileSizeLabel;
        
        private ProgressBar progressBar;
        private Label progressLabel;
        private Button sendButton;
        private TextBox logTextBox;

        private FileTransferService _fileTransferService;
        private bool isConnected = false;
        private Thread sendThread;

        public FileTransferForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeServices();
        }
        
        private void InitializeServices()
        {
            _fileTransferService = new FileTransferService();
            _fileTransferService.OnProgressChanged += (percent, sent, total) => UpdateProgress(percent, sent, total);
            _fileTransferService.OnLogMessage += (message) => AddLog(message);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FileTransferForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(600, 550);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FileTransferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Truyền File qua mạng TCP";
            this.Load += new System.EventHandler(this.FileTransferForm_Load);
            this.ResumeLayout(false);

        }

        private void InitializeCustomComponents()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(600, 60),
                BackColor = Color.FromArgb(36, 41, 46)
            };

            titleLabel = new Label
            {
                Text = "TCP File Transfer Client",
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White
            };
            headerPanel.Controls.Add(titleLabel);

            // Connection Panel
            Panel connectionPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(560, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            serverLabel = new Label
            {
                Text = "Server IP:",
                Location = new Point(15, 15),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            serverTextBox = new TextBox
            {
                Text = "127.0.0.1",
                Location = new Point(90, 15),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };

            portLabel = new Label
            {
                Text = "Port:",
                Location = new Point(260, 15),
                Size = new Size(40, 25),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            portTextBox = new TextBox
            {
                Text = "12345",
                Location = new Point(305, 15),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };

            connectButton = new Button
            {
                Text = "Kết nối",
                Location = new Point(400, 12),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 160, 67),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            connectButton.FlatAppearance.BorderSize = 0;
            connectButton.Click += ConnectButton_Click;

            statusLabel = new Label
            {
                Text = "● Chưa kết nối",
                Location = new Point(15, 50),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(200, 50, 50)
            };

            connectionPanel.Controls.Add(serverLabel);
            connectionPanel.Controls.Add(serverTextBox);
            connectionPanel.Controls.Add(portLabel);
            connectionPanel.Controls.Add(portTextBox);
            connectionPanel.Controls.Add(connectButton);
            connectionPanel.Controls.Add(statusLabel);

            // File Selection Panel
            Panel filePanel = new Panel
            {
                Location = new Point(20, 170),
                Size = new Size(560, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            fileLabel = new Label
            {
                Text = "Chọn file để gửi:",
                Location = new Point(15, 15),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            filePathTextBox = new TextBox
            {
                Location = new Point(15, 45),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.FromArgb(250, 251, 252)
            };

            browseButton = new Button
            {
                Text = "Duyệt...",
                Location = new Point(445, 43),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(9, 105, 218),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            browseButton.FlatAppearance.BorderSize = 0;
            browseButton.Click += BrowseButton_Click;

            fileSizeLabel = new Label
            {
                Text = "",
                Location = new Point(15, 75),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };

            filePanel.Controls.Add(fileLabel);
            filePanel.Controls.Add(filePathTextBox);
            filePanel.Controls.Add(browseButton);
            filePanel.Controls.Add(fileSizeLabel);

            // Progress Panel - CLO3.2
            Panel progressPanel = new Panel
            {
                Location = new Point(20, 280),
                Size = new Size(560, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            progressLabel = new Label
            {
                Text = "Tiến trình: 0%",
                Location = new Point(15, 15),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10)
            };

            progressBar = new ProgressBar
            {
                Location = new Point(15, 40),
                Size = new Size(420, 25),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            sendButton = new Button
            {
                Text = "Gửi File",
                Location = new Point(445, 15),
                Size = new Size(100, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 160, 67),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += SendButton_Click;

            progressPanel.Controls.Add(progressLabel);
            progressPanel.Controls.Add(progressBar);
            progressPanel.Controls.Add(sendButton);

            // Log Panel
            Label logLabel = new Label
            {
                Text = "Nhật ký hoạt động:",
                Location = new Point(20, 370),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            logTextBox = new TextBox
            {
                Location = new Point(20, 395),
                Size = new Size(560, 140),
                Font = new Font("Consolas", 9),
                BackColor = Color.FromArgb(250, 251, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            // Add all controls
            this.Controls.Add(headerPanel);
            this.Controls.Add(connectionPanel);
            this.Controls.Add(filePanel);
            this.Controls.Add(progressPanel);
            this.Controls.Add(logLabel);
            this.Controls.Add(logTextBox);

            AddLog("Sẵn sàng. Vui lòng kết nối đến server.");
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                try
                {
                    string server = serverTextBox.Text.Trim();
                    int port = int.Parse(portTextBox.Text.Trim());

                    if (_fileTransferService.Connect(server, port))
                    {
                        isConnected = true;
                        connectButton.Text = "Ngắt kết nối";
                        connectButton.BackColor = Color.FromArgb(218, 54, 51);
                        statusLabel.Text = $"● Đã kết nối đến {server}:{port}";
                        statusLabel.ForeColor = Color.FromArgb(46, 160, 67);
                        sendButton.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Không thể kết nối đến server!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddLog($"LỖI: Không thể kết nối - {ex.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            _fileTransferService.Disconnect();

            isConnected = false;
            connectButton.Text = "Kết nối";
            connectButton.BackColor = Color.FromArgb(46, 160, 67);
            statusLabel.Text = "● Chưa kết nối";
            statusLabel.ForeColor = Color.FromArgb(200, 50, 50);
            sendButton.Enabled = false;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file để gửi";
                ofd.Filter = "Tất cả file (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = ofd.FileName;
                    FileInfo fi = new FileInfo(ofd.FileName);
                    fileSizeLabel.Text = $"Kích thước: {FormatFileSize(fi.Length)} | Định dạng: {fi.Extension}";
                    AddLog($"Đã chọn file: {fi.Name} ({FormatFileSize(fi.Length)})");
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show("Vui lòng chọn file để gửi!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(filePathTextBox.Text))
            {
                MessageBox.Show("File không tồn tại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sendButton.Enabled = false;
            browseButton.Enabled = false;

            sendThread = new Thread(() => SendFile(filePathTextBox.Text));
            sendThread.IsBackground = true;
            sendThread.Start();
        }

        private void SendFile(string filePath)
        {
            var result = _fileTransferService.SendFile(filePath);
            
            if (result.Success)
            {
                MessageBox.Show("Gửi file thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Lỗi gửi file: {result.ErrorMessage}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            ResetUI();
        }

        private void UpdateProgress(int percent, long sent, long total)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => UpdateProgress(percent, sent, total)));
                return;
            }

            progressBar.Value = percent;
            progressLabel.Text = $"Tiến trình: {percent}% ({FormatFileSize(sent)} / {FormatFileSize(total)})";
        }

        private void ResetUI()
        {
            if (sendButton.InvokeRequired)
            {
                sendButton.Invoke(new Action(ResetUI));
                return;
            }

            sendButton.Enabled = isConnected;
            browseButton.Enabled = true;
        }

        private void AddLog(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action(() => AddLog(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            logTextBox.AppendText($"[{timestamp}] {message}\r\n");
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_fileTransferService.IsSending)
            {
                var result = MessageBox.Show("Đang gửi file. Bạn có chắc muốn hủy?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Disconnect();
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileTransferService?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FileTransferForm_Load(object sender, EventArgs e)
        {

        }
    }
}
