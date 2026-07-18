using System;
using System.Drawing;
using System.Windows.Forms;

namespace SEMI_FINAL
{
    public partial class Form1 : Form
    {
        // Khởi tạo service kết nối Ollama
        private OllamaService _ollamaService = new OllamaService();
        private bool _dangChoAI = false;  // Tránh spam gửi khi AI chưa trả lời
        private int _nextMessageY = 15;   // Theo dõi vị trí Y của tin nhắn tiếp theo

        public Form1()
        {
            InitializeComponent();
            this.Text = "AI Chat Assistant";

            // ✅ Thêm dòng này — cho phép nhấn Enter trong ô nhập
            txtInput.KeyDown += txtInput_KeyDown;

            KiemTraKetNoiOllama();
        }

        /// <summary>
        /// Kiểm tra và hiển thị trạng thái kết nối Ollama
        /// </summary>
        private async void KiemTraKetNoiOllama()
        {
            bool ketNoi = await _ollamaService.KiemTraKetNoi();

            // TODO: cập nhật lblStatus nếu bạn có label đó
            // lblStatus.Text    = ketNoi ? "● Đã kết nối" : "● Chưa kết nối";
            // lblStatus.ForeColor = ketNoi ? Color.LimeGreen : Color.Red;

            if (!ketNoi)
            {
                MessageBox.Show(
                    "Không thể kết nối Ollama!\n\n" +
                    "Hãy chắc chắn Ollama đang chạy:\n" +
                    "1. Mở Terminal\n" +
                    "2. Gõ: ollama serve\n" +
                    "3. Khởi động lại ứng dụng",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Tạo và hiển thị bong bóng tin nhắn trong pnlMain
        /// </summary>
        private void ThemTinNhan(string noiDung, bool laNguoiDung)
        {
            int maxWidth = pnlMain.Width - 80;
            var font     = new Font("Segoe UI", 10);
            int padH     = 24; // padding trái + phải (12 + 12)
            int padV     = 24; // padding trên + dưới (12 + 12)

            // Dùng TextRenderer — cùng engine với Label, đo chính xác hơn Graphics.MeasureString
            Size textSize = TextRenderer.MeasureText(
                noiDung,
                font,
                new Size(maxWidth - padH, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl
            );

            int bubbleWidth  = Math.Min(textSize.Width  + padH + 4, maxWidth);
            int bubbleHeight = textSize.Height + padV + 8; // +8 buffer tránh bị cắt chữ

            // Tạo Label với kích thước đã tính chính xác
            Label lblChat = new Label();
            lblChat.Text      = noiDung;
            lblChat.AutoSize  = false;
            lblChat.Size      = new Size(bubbleWidth, bubbleHeight);
            lblChat.Padding   = new Padding(12);
            lblChat.Font      = font;
            lblChat.BackColor = laNguoiDung ? Color.DodgerBlue : Color.FromArgb(235, 235, 235);
            lblChat.ForeColor = laNguoiDung ? Color.White : Color.Black;

            // Tính vị trí X (phải = người dùng, trái = AI)
            int toaDoX = laNguoiDung ? (pnlMain.Width - bubbleWidth - 25) : 15;
            lblChat.Location = new Point(toaDoX, _nextMessageY);

            pnlMain.Controls.Add(lblChat);

            // Cập nhật Y cho tin nhắn tiếp theo
            _nextMessageY += bubbleHeight + 15;

            pnlMain.Invalidate();
            pnlMain.ScrollControlIntoView(lblChat);
        }

        /// <summary>
        /// Xử lý khi nhấn nút GỬI
        /// </summary>
        private async void btnSend_Click(object sender, EventArgs e)
        {
            string cauHoi = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(cauHoi)) return;
            if (_dangChoAI) return; // Đang chờ AI trả lời thì không gửi thêm

            // Hiển thị tin nhắn người dùng
            ThemTinNhan(cauHoi, true);
            txtInput.Clear();

            // Khóa nút, hiển thị trạng thái chờ
            _dangChoAI = true;
            btnSend.Enabled = false;
            btnSend.Text = "...";

            try
            {
                // Gọi Ollama API thật (async, không block UI)
                string traLoi = await _ollamaService.GuiTinNhan(cauHoi);
                ThemTinNhan(traLoi, false);
            }
            catch (Exception ex)
            {
                ThemTinNhan($"⚠ Lỗi: {ex.Message}", false);
            }
            finally
            {
                // Mở khóa nút sau khi xong
                _dangChoAI = false;
                btnSend.Enabled = true;
                btnSend.Text = "GỬI";
            }
        }

        private void btnSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(this, new EventArgs());
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
        private void pnlMain_Paint(object sender, PaintEventArgs e) { }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(this, new EventArgs());
            }
        }
    }
}