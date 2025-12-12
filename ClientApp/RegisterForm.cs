using System;
using System.Drawing;
using System.Windows.Forms;
using ClientApp.Services;

namespace ClientApp
{
    public partial class RegisterForm : Form
    {
        private Panel mainPanel;
        private Label titleLabel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label emailLabel;
        private TextBox emailTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Label confirmPasswordLabel;
        private TextBox confirmPasswordTextBox;
        private Button registerButton;
        private LinkLabel loginLinkLabel;

        public RegisterForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RegisterForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(450, 600);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng ký tài khoản";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.ResumeLayout(false);

        }

        private void InitializeCustomComponents()
        {
            mainPanel = new Panel
            {
                Location = new Point(50, 30),
                Size = new Size(350, 540),
                BackColor = Color.White
            };

            titleLabel = new Label
            {
                Text = "Tạo tài khoản mới",
                Location = new Point(50, 30),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 41, 46),
                TextAlign = ContentAlignment.MiddleCenter
            };

            usernameLabel = new Label
            {
                Text = "Tên đăng nhập",
                Location = new Point(30, 80),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            usernameTextBox = new TextBox
            {
                Location = new Point(30, 110),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 252)
            };

            emailLabel = new Label
            {
                Text = "Email",
                Location = new Point(30, 160),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            emailTextBox = new TextBox
            {
                Location = new Point(30, 190),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 252)
            };

            passwordLabel = new Label
            {
                Text = "Mật khẩu",
                Location = new Point(30, 240),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(30, 270),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 252),
                PasswordChar = '•',
                UseSystemPasswordChar = true
            };

            confirmPasswordLabel = new Label
            {
                Text = "Xác nhận mật khẩu",
                Location = new Point(30, 320),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            confirmPasswordTextBox = new TextBox
            {
                Location = new Point(30, 350),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 252),
                PasswordChar = '•',
                UseSystemPasswordChar = true
            };

            registerButton = new Button
            {
                Text = "Đăng ký",
                Location = new Point(30, 410),
                Size = new Size(290, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 160, 67),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Click += RegisterButton_Click;

            loginLinkLabel = new LinkLabel
            {
                Text = "Đã có tài khoản? Đăng nhập ngay",
                Location = new Point(70, 470),
                Size = new Size(210, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                LinkColor = Color.FromArgb(9, 105, 218),
                ActiveLinkColor = Color.FromArgb(9, 105, 218),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            loginLinkLabel.LinkClicked += LoginLinkLabel_LinkClicked;

            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(usernameLabel);
            mainPanel.Controls.Add(usernameTextBox);
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(confirmPasswordLabel);
            mainPanel.Controls.Add(confirmPasswordTextBox);
            mainPanel.Controls.Add(registerButton);
            mainPanel.Controls.Add(loginLinkLabel);

            mainPanel.BorderStyle = BorderStyle.FixedSingle;
            
            this.Controls.Add(mainPanel);
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string email = emailTextBox.Text.Trim();
            string password = passwordTextBox.Text;
            string confirmPassword = confirmPasswordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var authService = new AuthService();
                var result = authService.Register(username, password, email);
                
                if (result.Success)
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể kết nối đến server!\n{ex.Message}", "Lỗi kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
