using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class LoginForm : Form
    {
        private Panel mainPanel;
        private Label titleLabel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Button loginButton;
        private LinkLabel registerLinkLabel;

        public LoginForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.ClientSize = new Size(450, 500);
            this.Text = "Đăng nhập";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            mainPanel = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(350, 400),
                BackColor = Color.White
            };

            titleLabel = new Label
            {
                Text = "Đăng nhập vào hệ thống",
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

            passwordLabel = new Label
            {
                Text = "Mật khẩu",
                Location = new Point(30, 160),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(36, 41, 46)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(30, 190),
                Size = new Size(290, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 252),
                PasswordChar = '•',
                UseSystemPasswordChar = true
            };

            loginButton = new Button
            {
                Text = "Đăng nhập",
                Location = new Point(30, 250),
                Size = new Size(290, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 160, 67),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += LoginButton_Click;

            registerLinkLabel = new LinkLabel
            {
                Text = "Chưa có tài khoản? Đăng ký ngay",
                Location = new Point(70, 310),
                Size = new Size(210, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                LinkColor = Color.FromArgb(9, 105, 218),
                ActiveLinkColor = Color.FromArgb(9, 105, 218),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            registerLinkLabel.LinkClicked += RegisterLinkLabel_LinkClicked;

            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(usernameLabel);
            mainPanel.Controls.Add(usernameTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(loginButton);
            mainPanel.Controls.Add(registerLinkLabel);

            mainPanel.BorderStyle = BorderStyle.FixedSingle;
            
            this.Controls.Add(mainPanel);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: Implement login logic here
            MessageBox.Show($"Đăng nhập với username: {username}", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RegisterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
