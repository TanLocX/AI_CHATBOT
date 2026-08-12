using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace SEMI_FINAL
{
    public class FormApiKeyDialog : Form
    {
        private Guna2TextBox txtApiKey;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        private Label lblTitle;
        private Label lblGuide;
        private LinkLabel lnkGetApiKey;

        public string ApiKey { get; private set; }

        public FormApiKeyDialog(string currentApiKey = "")
        {
            InitializeComponent();
            txtApiKey.Text = currentApiKey ?? "";
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 260);
            this.Text = "Cấu hình Gemini API Key";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(24, 24, 27);
            this.ForeColor = Color.White;

            lblTitle = new Label
            {
                Text = "🔑 Nhập Google Gemini API Key của bạn",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(163, 201, 255),
                Location = new Point(20, 20),
                AutoSize = true
            };

            lblGuide = new Label
            {
                Text = "Để sử dụng mô hình Gemini, bạn cần có API Key từ Google AI Studio:",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(22, 52),
                AutoSize = true
            };

            lnkGetApiKey = new LinkLabel
            {
                Text = "👉 Bấm vào đây để lấy API Key miễn phí từ Google AI Studio",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                LinkColor = Color.FromArgb(99, 179, 237),
                ActiveLinkColor = Color.FromArgb(144, 205, 244),
                Location = new Point(22, 76),
                AutoSize = true
            };
            lnkGetApiKey.LinkClicked += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("https://aistudio.google.com/apikey");
                }
                catch { }
            };

            txtApiKey = new Guna2TextBox
            {
                PlaceholderText = "Dán Gemini API Key tại đây (vd: AIzaSy...)",
                Font = new Font("Segoe UI", 10f),
                FillColor = Color.FromArgb(39, 39, 42),
                ForeColor = Color.FromArgb(228, 228, 231),
                BorderColor = Color.FromArgb(63, 63, 70),
                FocusedState = { BorderColor = Color.FromArgb(0, 120, 212) },
                BorderRadius = 8,
                Location = new Point(22, 110),
                Size = new Size(440, 42),
                PasswordChar = '•'
            };

            btnSave = new Guna2Button
            {
                Text = "Lưu API Key",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FillColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                BorderRadius = 8,
                Location = new Point(342, 168),
                Size = new Size(120, 38)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Guna2Button
            {
                Text = "Hủy",
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.FromArgb(52, 52, 58),
                ForeColor = Color.White,
                BorderRadius = 8,
                Location = new Point(250, 168),
                Size = new Size(80, 38)
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblGuide);
            this.Controls.Add(lnkGetApiKey);
            this.Controls.Add(txtApiKey);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string key = txtApiKey.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Vui lòng nhập API Key hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.ApiKey = key;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
