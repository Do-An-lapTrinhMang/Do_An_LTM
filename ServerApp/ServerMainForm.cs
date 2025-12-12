using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerApp.Services;

namespace ServerApp
{
    public partial class ServerMainForm : Form
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Label statusLabel;
        private Label portLabel;
        private Label connectedClientsLabel;
        private Button startServerButton;
        private Button stopServerButton;
        private Button viewUsersButton;
        private Button viewHistoryButton;
        private TextBox logTextBox;
        private Label logLabel;
        
        private TcpListener tcpListener;
        private Thread listenerThread;
        private bool isServerRunning = false;
        private int connectedClients = 0;
        private const int SERVER_PORT = 12345;

        public ServerMainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ServerMainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(700, 600);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ServerMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCP File Transfer Server";
            this.Load += new System.EventHandler(this.ServerMainForm_Load);
            this.ResumeLayout(false);

        }

        private void InitializeCustomComponents()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(700, 80),
                BackColor = Color.FromArgb(36, 41, 46)
            };

            titleLabel = new Label
            {
                Text = "TCP File Transfer Server",
                Location = new Point(20, 15),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White
            };

            statusLabel = new Label
            {
                Text = "● Stopped",
                Location = new Point(20, 50),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(255, 100, 100)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statusLabel);

            // Server Info Panel
            Panel infoPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(660, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            portLabel = new Label
            {
                Text = $"Port: {SERVER_PORT}",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            connectedClientsLabel = new Label
            {
                Text = "Connected Clients: 0",
                Location = new Point(20, 50),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            infoPanel.Controls.Add(portLabel);
            infoPanel.Controls.Add(connectedClientsLabel);

            // Control Buttons Panel
            Panel controlPanel = new Panel
            {
                Location = new Point(20, 220),
                Size = new Size(660, 60),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            startServerButton = new Button
            {
                Text = "Start Server",
                Location = new Point(20, 15),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 160, 67),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            startServerButton.FlatAppearance.BorderSize = 0;
            startServerButton.Click += StartServerButton_Click;

            stopServerButton = new Button
            {
                Text = "Stop Server",
                Location = new Point(180, 15),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(218, 54, 51),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            stopServerButton.FlatAppearance.BorderSize = 0;
            stopServerButton.Click += StopServerButton_Click;

            viewUsersButton = new Button
            {
                Text = "View Users",
                Location = new Point(350, 15),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(9, 105, 218),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            viewUsersButton.FlatAppearance.BorderSize = 0;
            viewUsersButton.Click += ViewUsersButton_Click;

            viewHistoryButton = new Button
            {
                Text = "View History",
                Location = new Point(500, 15),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(9, 105, 218),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            viewHistoryButton.FlatAppearance.BorderSize = 0;
            viewHistoryButton.Click += ViewHistoryButton_Click;

            controlPanel.Controls.Add(startServerButton);
            controlPanel.Controls.Add(stopServerButton);
            controlPanel.Controls.Add(viewUsersButton);
            controlPanel.Controls.Add(viewHistoryButton);

            // Activity Log
            logLabel = new Label
            {
                Text = "Activity Log:",
                Location = new Point(20, 295),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            logTextBox = new TextBox
            {
                Location = new Point(20, 325),
                Size = new Size(660, 250),
                Font = new Font("Consolas", 9),
                BackColor = Color.FromArgb(250, 251, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            // Add all controls to form
            this.Controls.Add(headerPanel);
            this.Controls.Add(infoPanel);
            this.Controls.Add(controlPanel);
            this.Controls.Add(logLabel);
            this.Controls.Add(logTextBox);

            // Add initial log
            AddLog("Server initialized. Ready to start.");
        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                listenerThread = new Thread(StartListening);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                isServerRunning = true;
                startServerButton.Enabled = false;
                stopServerButton.Enabled = true;

                UpdateStatusLabel("● Running", Color.FromArgb(100, 255, 100));
                AddLog($"Server started on port {SERVER_PORT}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"ERROR: Failed to start server - {ex.Message}");
            }
        }

        private void StopServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                isServerRunning = false;
                
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }

                startServerButton.Enabled = true;
                stopServerButton.Enabled = false;

                UpdateStatusLabel("● Stopped", Color.FromArgb(255, 100, 100));
                AddLog("Server stopped");
                
                connectedClients = 0;
                UpdateConnectedClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping server: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"ERROR: Failed to stop server - {ex.Message}");
            }
        }

        private void StartListening()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, SERVER_PORT);
                tcpListener.Start();

                AddLog("Waiting for client connections...");

                while (isServerRunning)
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        connectedClients++;
                        
                        string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                        AddLog($"Client connected from {clientIP}");
                        UpdateConnectedClients();

                        // TODO: Handle client communication in separate thread
                        Thread clientThread = new Thread(() => HandleClient(client));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (isServerRunning)
                {
                    AddLog($"ERROR: Listener exception - {ex.Message}");
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[8192];
                
                // Đọc request từ client
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                
                string[] parts = request.Split('|');
                string command = parts[0];
                
                switch (command)
                {
                    case "REGISTER":
                        HandleRegister(stream, parts, clientIP);
                        break;
                    case "LOGIN":
                        HandleLogin(stream, parts, clientIP);
                        break;
                    case "FILE":
                        HandleFileTransfer(stream, parts, clientIP, buffer);
                        break;
                    default:
                        AddLog($"[{clientIP}] Unknown command: {command}");
                        byte[] errorResponse = Encoding.UTF8.GetBytes("ERROR|Unknown command\n");
                        stream.Write(errorResponse, 0, errorResponse.Length);
                        break;
                }
            }
            catch (Exception ex)
            {
                AddLog($"ERROR [{clientIP}]: {ex.Message}");
            }
            finally
            {
                client.Close();
                connectedClients--;
                UpdateConnectedClients();
                AddLog($"[{clientIP}] Disconnected");
            }
        }
        
        private void HandleRegister(NetworkStream stream, string[] parts, string clientIP)
        {
            // Format: REGISTER|username|password|fullname
            if (parts.Length != 4)
            {
                byte[] response = Encoding.UTF8.GetBytes("ERROR|Invalid format\n");
                stream.Write(response, 0, response.Length);
                return;
            }
            
            string username = parts[1];
            string password = parts[2];
            string fullName = parts[3];
            
            var authService = new AuthService();
            var result = authService.Register(username, password, fullName);
            
            if (result.Success)
            {
                byte[] response = Encoding.UTF8.GetBytes("OK|Registration successful\n");
                stream.Write(response, 0, response.Length);
                AddLog($"[{clientIP}] User registered: {username}");
            }
            else
            {
                byte[] response = Encoding.UTF8.GetBytes($"ERROR|{result.ErrorMessage}\n");
                stream.Write(response, 0, response.Length);
                AddLog($"[{clientIP}] Register failed: {result.ErrorMessage}");
            }
        }
        
        private void HandleLogin(NetworkStream stream, string[] parts, string clientIP)
        {
            // Format: LOGIN|username|password
            if (parts.Length != 3)
            {
                byte[] response = Encoding.UTF8.GetBytes("ERROR|Invalid format\n");
                stream.Write(response, 0, response.Length);
                return;
            }
            
            string username = parts[1];
            string password = parts[2];
            
            var authService = new AuthService();
            var result = authService.Login(username, password);
            
            if (result.Success)
            {
                byte[] response = Encoding.UTF8.GetBytes($"OK|{result.UserId}|{result.FullName}\n");
                stream.Write(response, 0, response.Length);
                AddLog($"[{clientIP}] Login successful: {username}");
            }
            else
            {
                byte[] response = Encoding.UTF8.GetBytes($"ERROR|{result.ErrorMessage}\n");
                stream.Write(response, 0, response.Length);
                AddLog($"[{clientIP}] Login failed: {username}");
            }
        }
        
        private void HandleFileTransfer(NetworkStream stream, string[] parts, string clientIP, byte[] buffer)
        {
            // Format: FILE|filename|filesize
            if (parts.Length != 3)
            {
                AddLog($"[{clientIP}] Invalid file metadata format");
                return;
            }
            
            string fileName = parts[1];
            long fileSize = long.Parse(parts[2]);
            
            var fileService = new FileService();
            fileService.OnLogMessage += (message) => AddLog(message);
            
            var result = fileService.ReceiveFile(stream, fileName, fileSize, clientIP);
            
            if (!result.Success)
            {
                AddLog($"[{clientIP}] File transfer failed: {result.ErrorMessage}");
            }
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

        private void ViewUsersButton_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Open form to view users from database
                AddLog("Opening users list...");
                MessageBox.Show("Feature coming soon: View all registered users", "Users", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AddLog($"ERROR: Failed to view users - {ex.Message}");
            }
        }

        private void ViewHistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Open form to view transfer history from database
                AddLog("Opening transfer history...");
                MessageBox.Show("Feature coming soon: View file transfer history", "History", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AddLog($"ERROR: Failed to view history - {ex.Message}");
            }
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

        private void UpdateStatusLabel(string text, Color color)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => UpdateStatusLabel(text, color)));
                return;
            }

            statusLabel.Text = text;
            statusLabel.ForeColor = color;
        }

        private void UpdateConnectedClients()
        {
            if (connectedClientsLabel.InvokeRequired)
            {
                connectedClientsLabel.Invoke(new Action(UpdateConnectedClients));
                return;
            }

            connectedClientsLabel.Text = $"Connected Clients: {connectedClients}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isServerRunning)
            {
                var result = MessageBox.Show("Server is still running. Stop and exit?", "Confirm Exit", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    isServerRunning = false;
                    if (tcpListener != null)
                    {
                        tcpListener.Stop();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }
            }
            base.Dispose(disposing);
        }

        private void ServerMainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
