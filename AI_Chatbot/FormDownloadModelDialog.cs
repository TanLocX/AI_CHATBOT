using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace SEMI_FINAL
{
    public class FormDownloadModelDialog : Form
    {
        private Guna2ComboBox cbPresetModels;
        private Guna2TextBox txtCustomModel;
        private Guna2Button btnDownload;
        private Guna2Button btnCancel;
        private Guna2ProgressBar progressBar;
        private Label lblTitle;
        private Label lblPreset;
        private Label lblCustom;
        private Label lblStatus;
        private Label lblDetail;

        private OllamaService _ollamaService;
        private CancellationTokenSource _cts;

        public string DownloadedModelName { get; private set; }

        public FormDownloadModelDialog(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(520, 360);
            this.Text = "Tải Model AI Local (Ollama)";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(24, 24, 27);
            this.ForeColor = Color.White;

            lblTitle = new Label
            {
                Text = "📥 Tải Model AI Local về máy",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(163, 201, 255),
                Location = new Point(20, 18),
                AutoSize = true
            };

            lblPreset = new Label
            {
                Text = "Chọn Model phổ biến:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(22, 52),
                AutoSize = true
            };

            cbPresetModels = new Guna2ComboBox
            {
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.FromArgb(39, 39, 42),
                ForeColor = Color.FromArgb(228, 228, 231),
                BorderColor = Color.FromArgb(63, 63, 70),
                FocusedState = { BorderColor = Color.FromArgb(0, 120, 212) },
                BorderRadius = 8,
                Location = new Point(22, 76),
                Size = new Size(460, 36),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cbPresetModels.Items.Add("llama3.2 (3B - ~2.0 GB - Nhẹ & Nhanh - Khuyên dùng)");
            cbPresetModels.Items.Add("qwen2.5 (3B - ~2.2 GB - Tiếng Việt & Code chuẩn)");
            cbPresetModels.Items.Add("mistral (7B - ~4.1 GB - Thông minh, tư duy sâu)");
            cbPresetModels.Items.Add("codellama (7B - ~3.8 GB - Chuyên lập trình)");
            cbPresetModels.SelectedIndex = 0;

            lblCustom = new Label
            {
                Text = "Hoặc nhập tên Model khác trên Ollama Library:",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(160, 160, 160),
                Location = new Point(22, 124),
                AutoSize = true
            };

            txtCustomModel = new Guna2TextBox
            {
                PlaceholderText = "Ví dụ: gemma2:2b, phi3, llava...",
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.FromArgb(39, 39, 42),
                ForeColor = Color.FromArgb(228, 228, 231),
                BorderColor = Color.FromArgb(63, 63, 70),
                FocusedState = { BorderColor = Color.FromArgb(0, 120, 212) },
                BorderRadius = 8,
                Location = new Point(22, 146),
                Size = new Size(460, 38)
            };

            progressBar = new Guna2ProgressBar
            {
                Location = new Point(22, 200),
                Size = new Size(460, 20),
                BorderRadius = 6,
                FillColor = Color.FromArgb(45, 45, 50),
                ProgressColor = Color.FromArgb(0, 120, 212),
                ProgressColor2 = Color.FromArgb(163, 201, 255),
                Value = 0,
                Visible = false
            };

            lblStatus = new Label
            {
                Text = "Sẵn sàng tải.",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(22, 228),
                Size = new Size(460, 22),
                AutoEllipsis = true
            };

            lblDetail = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140),
                Location = new Point(22, 250),
                Size = new Size(460, 18),
                AutoEllipsis = true
            };

            btnDownload = new Guna2Button
            {
                Text = "📥 Bắt đầu Tải",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FillColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                BorderRadius = 8,
                Location = new Point(352, 275),
                Size = new Size(130, 38)
            };
            btnDownload.Click += BtnDownload_Click;

            btnCancel = new Guna2Button
            {
                Text = "Hủy",
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.FromArgb(52, 52, 58),
                ForeColor = Color.White,
                BorderRadius = 8,
                Location = new Point(260, 275),
                Size = new Size(80, 38)
            };
            btnCancel.Click += (s, e) =>
            {
                _cts?.Cancel();
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPreset);
            this.Controls.Add(cbPresetModels);
            this.Controls.Add(lblCustom);
            this.Controls.Add(txtCustomModel);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblDetail);
            this.Controls.Add(btnDownload);
            this.Controls.Add(btnCancel);
        }

        private async void BtnDownload_Click(object sender, EventArgs e)
        {
            string targetModel = txtCustomModel.Text.Trim();
            if (string.IsNullOrEmpty(targetModel))
            {
                string selectedPreset = cbPresetModels.SelectedItem?.ToString() ?? "";
                int spaceIdx = selectedPreset.IndexOf(' ');
                targetModel = spaceIdx > 0 ? selectedPreset.Substring(0, spaceIdx) : selectedPreset;
            }

            if (string.IsNullOrEmpty(targetModel))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập tên Model hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnDownload.Enabled = false;
            cbPresetModels.Enabled = false;
            txtCustomModel.Enabled = false;
            progressBar.Visible = true;
            progressBar.Value = 0;
            lblStatus.Text = $"⏳ Đang kết nối tới Ollama để tải model '{targetModel}'...";

            _cts = new CancellationTokenSource();

            try
            {
                await _ollamaService.PullModel(targetModel, (completed, total, status) =>
                {
                    if (this.IsDisposed || !this.IsHandleCreated) return;

                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = string.IsNullOrEmpty(status) ? "Đang xử lý..." : status;

                        if (total > 0)
                        {
                            int pct = (int)((double)completed / total * 100);
                            progressBar.Value = Math.Min(Math.Max(pct, 0), 100);

                            double compMB = completed / 1024.0 / 1024.0;
                            double totMB = total / 1024.0 / 1024.0;
                            lblDetail.Text = $"Tiến độ: {compMB:F1} MB / {totMB:F1} MB ({pct}%)";
                        }
                    }));
                }, _cts.Token);

                _cts.Token.ThrowIfCancellationRequested();

                DownloadedModelName = targetModel;
                MessageBox.Show($"Tải thành công model '{targetModel}'! Bạn có thể sử dụng ngay bây giờ.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "❌ Đã hủy quá trình tải về.";
                lblDetail.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải Model: {ex.Message}", "Lỗi tải về", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Tải về thất bại!";
            }
            finally
            {
                btnDownload.Enabled = true;
                cbPresetModels.Enabled = true;
                txtCustomModel.Enabled = true;
            }
        }
    }
}
