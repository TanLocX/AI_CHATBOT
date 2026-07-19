using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEMI_FINAL
{
    public partial class Form1 : Form
    {
        // Khởi tạo service kết nối Ollama.
        // Đổi lại localhost vì IP 192.168.193.10 là máy của bạn khác (ZeroTier), không dùng được trên máy này
        private OllamaService _ollamaService = new OllamaService("http://localhost:11434", "llama3.2");
        private OcrService _ocrService = new OcrService();
        private DocumentReaderService _docService;
        private string _fileDinhKemTen = null;
        private string _fileDinhKemNoiDung = null;
        private bool _dangChoAI = false;  // Tránh spam gửi khi AI chưa trả lời

        public Form1()
        {
            InitializeComponent();
            this.Text = "AI Chat Assistant";
            _docService = new DocumentReaderService(_ocrService);

            // ✅ Cho phép nhấn Enter trong ô nhập
            txtInput.KeyDown += txtInput_KeyDown;

            TuDongKhoiDongOllama();  // Tự khởi động Ollama nếu chưa chạy
        }

        /// <summary>
        /// Tự động khởi động Ollama nếu chưa chạy, sau đó kiểm tra kết nối và load models
        /// </summary>
        private async void TuDongKhoiDongOllama()
        {
            // Kiểm tra Ollama đã chạy chưa
            bool dangChay = await _ollamaService.KiemTraKetNoi();

            if (!dangChay)
            {
                // Tìm tiến trình ollama đang chạy không
                var processes = Process.GetProcessesByName("ollama");
                if (processes.Length == 0)
                {
                    try
                    {
                        // Khởi động ollama serve ngầm (không hiện cửa sổ CMD)
                        var psi = new ProcessStartInfo()
                        {
                            FileName = "ollama",
                            Arguments = "serve",
                            UseShellExecute = false,
                            CreateNoWindow = true,   // Ẩn hoàn toàn, không hiện CMD
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        Process.Start(psi);

                        // Chờ Ollama khởi động xong (~3 giây)
                        await Task.Delay(3000);
                    }
                    catch
                    {
                        // Nếu không tự start được, hiện hướng dẫn thủ công
                        MessageBox.Show(
                            "Không thể tự khởi động Ollama.\n\n" +
                            "Hãy mở Command Prompt và chạy:\n" +
                            "   ollama serve\n\n" +
                            "Sau đó khởi động lại ứng dụng.",
                            "Ollama chưa chạy",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    // Tiến trình ollama tồn tại nhưng chưa sẵn sàng, chờ thêm
                    await Task.Delay(2000);
                }
            }

            // Sau khi Ollama đã chạy, kiểm tra kết nối và load models
            KiemTraKetNoiOllama();
            TaiDanhSachModel();
        }

        /// <summary>
        /// Tự động tải danh sách model từ Ollama và đổ vào ComboBox
        /// </summary>
        private async void TaiDanhSachModel()
        {
            guna2ComboBox1.Enabled = false;
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("⏳ Đang tải...");
            guna2ComboBox1.SelectedIndex = 0;

            var models = await _ollamaService.LayDanhSachModel();

            guna2ComboBox1.Items.Clear();

            if (models.Count == 0)
            {
                guna2ComboBox1.Items.Add("(Không tìm thấy model)");
                guna2ComboBox1.SelectedIndex = 0;
                guna2ComboBox1.Enabled = false;
                return;
            }

            foreach (var m in models)
                guna2ComboBox1.Items.Add(m);

            // Chọn model đang dùng nếu có trong danh sách
            string currentModel = "llama3.2";
            int idx = models.FindIndex(m => m.StartsWith(currentModel));
            guna2ComboBox1.SelectedIndex = idx >= 0 ? idx : 0;

            guna2ComboBox1.Enabled = true;
        }

        /// <summary>
        /// Xử lý khi người dùng chọn model khác trong ComboBox
        /// </summary>
        private void cboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = guna2ComboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected) || selected.StartsWith("(") || selected.StartsWith("⏳"))
                return;

            _ollamaService.SetModel(selected);
            ThemTinNhan($"🤖 Đã chuyển sang model: {selected}", false);
        }

        private async void KiemTraKetNoiOllama()
        {
            bool ketNoi = await _ollamaService.KiemTraKetNoi();

            // TODO: cập nhật lblStatus nếu bạn có label đó
            // lblStatus.Text    = ketNoi ? "● Đã kết nối" : "● Chưa kết nối";
            // lblStatus.ForeColor = ketNoi ? Color.LimeGreen : Color.Red;

            if (!ketNoi)
            {
                MessageBox.Show(
                    "Không thể kết nối máy chủ Ollama!\n\n" +
                    "Hãy kiểm tra các bước sau:\n" +
                    "1. Nếu chạy máy local: gõ 'ollama serve' trong Terminal.\n" +
                    "2. Nếu kết nối máy khác trong mạng LAN:\n" +
                    "   - Đảm bảo đã nhập đúng IP máy đó trong Form1.cs\n" +
                    "   - Đảm bảo máy chủ Ollama đã cấu hình biến môi trường OLLAMA_HOST=0.0.0.0\n" +
                    "   - Kiểm tra tường lửa (Firewall) máy chủ đã mở cổng 11434 chưa.",
                    "Lỗi kết nối Ollama",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Tính toán tọa độ Y cho tin nhắn tiếp theo dựa vào phần tử thấp nhất trong pnlMain, tránh hoàn toàn lỗi chồng chéo khi cuộn (AutoScroll)
        /// </summary>
        private int GetNextMessageY()
        {
            int bottomMost = 15;
            foreach (Control c in pnlMain.Controls)
            {
                if (c.Visible && c.Bottom + 15 > bottomMost)
                {
                    bottomMost = c.Bottom + 15;
                }
            }
            return bottomMost;
        }

        /// <summary>
        /// Tạo và hiển thị bong bóng tin nhắn trong pnlMain, trả về control vừa tạo (Guna2Panel chứa RichTextBox và nút sao chép)
        /// </summary>
        private Control ThemTinNhan(string noiDung, bool laNguoiDung)
        {
            int maxWidth = pnlMain.Width - 80;
            var font     = new Font("Google Sans", 12);
            int padH     = 24; // padding trái + phải (12 + 12)

            // Dùng TextRenderer để đo sơ bộ kích thước
            Size textSize = TextRenderer.MeasureText(
                noiDung,
                font,
                new Size(maxWidth - padH - 10, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl
            );

            int bubbleWidth  = Math.Min(textSize.Width + padH + 16, maxWidth);
            int rtbWidth     = bubbleWidth - padH;
            int rtbHeight    = textSize.Height + 10;

            // Tạo Panel chứa bong bóng tin nhắn
            Guna.UI2.WinForms.Guna2Panel bubblePanel = new Guna.UI2.WinForms.Guna2Panel();
            bubblePanel.BorderRadius = 12;
            Color bgColor = laNguoiDung ? Color.FromArgb(99, 102, 241) : Color.FromArgb(39, 39, 42);
            Color fgColor = laNguoiDung ? Color.White : Color.FromArgb(228, 228, 231);
            bubblePanel.FillColor = bgColor;
            bubblePanel.BackColor = Color.Transparent;

            // Tạo RichTextBox để người dùng chọn và bôi đen chữ thoải mái
            RichTextBox rtb = new RichTextBox();
            rtb.Text = noiDung;
            rtb.Font = font;
            rtb.ReadOnly = true;
            rtb.BorderStyle = BorderStyle.None;
            rtb.BackColor = bgColor;
            rtb.ForeColor = fgColor;
            rtb.ScrollBars = RichTextBoxScrollBars.None;
            rtb.Location = new Point(12, 12);
            rtb.Size = new Size(rtbWidth, rtbHeight);

            // Nút sao chép cho tin nhắn của AI
            Button btnCopy = null;
            if (!laNguoiDung)
            {
                btnCopy = new Button();
                btnCopy.Text = "📋 Sao chép";
                btnCopy.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                btnCopy.Size = new Size(95, 26);
                btnCopy.FlatStyle = FlatStyle.Flat;
                btnCopy.FlatAppearance.BorderSize = 0;
                btnCopy.BackColor = Color.FromArgb(215, 215, 215);
                btnCopy.ForeColor = Color.FromArgb(50, 50, 50);
                btnCopy.Cursor = Cursors.Hand;
                btnCopy.Location = new Point(12, rtb.Bottom + 6);

                btnCopy.Click += (s, e) => {
                    try {
                        if (!string.IsNullOrEmpty(noiDung))
                        {
                            Clipboard.SetText(noiDung);
                            btnCopy.Text = "✔ Đã chép!";
                            Timer t = new Timer { Interval = 2000 };
                            t.Tick += (ts, te) => {
                                btnCopy.Text = "📋 Sao chép";
                                t.Stop();
                                t.Dispose();
                            };
                            t.Start();
                        }
                    } catch { }
                };
            }

            // Đặt chiều cao ban đầu của bong bóng
            bubblePanel.Size = new Size(bubbleWidth, (btnCopy != null ? btnCopy.Bottom : rtb.Bottom) + 12);

            // Menu chuột phải sao chép tiện lợi
            ContextMenuStrip menuSaoChep = new ContextMenuStrip();
            ToolStripMenuItem itemCopyAll = new ToolStripMenuItem("📋 Sao chép toàn bộ tin nhắn");
            itemCopyAll.Click += (s, e) => {
                try { if (!string.IsNullOrEmpty(noiDung)) Clipboard.SetText(noiDung); } catch { }
            };
            ToolStripMenuItem itemCopySel = new ToolStripMenuItem("✂ Sao chép đoạn văn bản đã chọn");
            itemCopySel.Click += (s, e) => {
                try {
                    if (!string.IsNullOrEmpty(rtb.SelectedText))
                        Clipboard.SetText(rtb.SelectedText);
                    else if (!string.IsNullOrEmpty(noiDung))
                        Clipboard.SetText(noiDung);
                } catch { }
            };
            menuSaoChep.Items.Add(itemCopyAll);
            menuSaoChep.Items.Add(itemCopySel);

            rtb.ContextMenuStrip = menuSaoChep;
            bubblePanel.ContextMenuStrip = menuSaoChep;

            // Tự động điều chỉnh kích thước chính xác khi RichTextBox render xong
            rtb.ContentsResized += (s, e) => {
                int neededHeight = e.NewRectangle.Height + 6;
                if (Math.Abs(rtb.Height - neededHeight) > 2)
                {
                    rtb.Height = neededHeight;
                    if (btnCopy != null)
                    {
                        btnCopy.Location = new Point(12, rtb.Bottom + 6);
                        bubblePanel.Height = btnCopy.Bottom + 12;
                    }
                    else
                    {
                        bubblePanel.Height = rtb.Bottom + 12;
                    }
                }
            };

            bubblePanel.Controls.Add(rtb);
            if (btnCopy != null) bubblePanel.Controls.Add(btnCopy);

            // Tính vị trí X và Y
            int toaDoX = laNguoiDung ? (pnlMain.Width - bubbleWidth - 25) : 15;
            int toaDoY = GetNextMessageY();
            bubblePanel.Location = new Point(toaDoX, toaDoY);

            pnlMain.Controls.Add(bubblePanel);
            pnlMain.Invalidate();
            pnlMain.ScrollControlIntoView(bubblePanel);

            return bubblePanel;
        }

        /// <summary>
        /// Xử lý khi nhấn nút GỬI
        /// </summary>
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_dangChoAI) return;

            string cauHoi = txtInput.Text.Trim();

            // Nếu không có file đính kèm và ô nhập trống thì không làm gì
            if (_fileDinhKemNoiDung == null && string.IsNullOrEmpty(cauHoi)) return;

            if (_fileDinhKemNoiDung != null)
            {
                string userPrompt = string.IsNullOrEmpty(cauHoi)
                    ? "Hãy phân tích, tóm tắt ý chính và giải thích chi tiết nội dung quan trọng từ tài liệu đính kèm bằng tiếng Việt rõ ràng."
                    : cauHoi;

                string cauHoiHienThi = string.IsNullOrEmpty(cauHoi)
                    ? $"[📎 Đính kèm: {_fileDinhKemTen}]\n(Yêu cầu: Phân tích & tóm tắt tài liệu)"
                    : $"[📎 Đính kèm: {_fileDinhKemTen}]\n{userPrompt}";

                string cauHoiGuiAI = $"Dưới đây là dữ liệu được trích xuất từ tài liệu \"{_fileDinhKemTen}\":\n" +
                                     $"--------------------------------------------\n" +
                                     $"{_fileDinhKemNoiDung}\n" +
                                     $"--------------------------------------------\n\n" +
                                     $"Yêu cầu/chú thích của người dùng đối với tài liệu trên: \"{userPrompt}\"";

                // Reset trạng thái đính kèm
                _fileDinhKemTen = null;
                _fileDinhKemNoiDung = null;
                if (btnOcr != null)
                {
                    btnOcr.Text = "➕ File";
                    btnOcr.FillColor = Color.FromArgb(64, 64, 64);
                }

                txtInput.Clear();
                GuiTinNhanChoAI(cauHoiHienThi, cauHoiGuiAI);
            }
            else
            {
                txtInput.Clear();
                GuiTinNhanChoAI(cauHoi, cauHoi);
            }
        }

        /// <summary>
        /// Xử lý khi nhấn nút Đính kèm (+) — đọc file và lưu vào bộ nhớ đệm, chờ người dùng nhập chú thích rồi nhấn GỬI
        /// </summary>
        private void btnOcr_Click(object sender, EventArgs e)
        {
            if (_dangChoAI) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn tài liệu (Word, Excel, PDF, Markdown, Ảnh...) để đính kèm";
                ofd.Filter = "Tất cả các định dạng hỗ trợ|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.md;*.txt;*.csv;*.json;*.xml|" +
                             "Tài liệu Word (*.doc;*.docx)|*.doc;*.docx|" +
                             "Tài liệu Excel (*.xls;*.xlsx)|*.xls;*.xlsx|" +
                             "Tài liệu PDF (*.pdf)|*.pdf|" +
                             "Văn bản & Markdown (*.md;*.txt;*.csv;*.json)|*.md;*.txt;*.csv;*.json|" +
                             "Hình ảnh & OCR (*.jpg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|" +
                             "Tất cả các file (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = ofd.FileName;
                        string fileName = System.IO.Path.GetFileName(filePath);
                        
                        // Đọc toàn bộ nội dung từ file theo định dạng bằng DocumentReaderService
                        string extractedText = _docService.ReadDocument(filePath);

                        if (string.IsNullOrWhiteSpace(extractedText))
                        {
                            MessageBox.Show("File không chứa nội dung văn bản hoặc trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Lưu vào bộ nhớ đệm, chưa gửi ngay
                        _fileDinhKemTen = fileName;
                        _fileDinhKemNoiDung = extractedText;

                        // Đổi màu và chữ nút để thông báo đã đính kèm
                        if (btnOcr != null)
                        {
                            btnOcr.Text = "📎 " + (fileName.Length > 8 ? fileName.Substring(0, 8) + "..." : fileName);
                            btnOcr.FillColor = Color.MediumSeaGreen;
                        }

                        // Hiển thị thông báo hướng dẫn cho người dùng
                        ThemTinNhan($"📎 Đã tải lên tài liệu: \"{fileName}\" ({extractedText.Length} ký tự).\n👉 Hãy nhập chú thích hoặc câu hỏi của bạn vào ô bên dưới rồi nhấn GỬI (Nếu để trống và nhấn GỬI, AI sẽ tự động phân tích và tóm tắt file).", false);
                        txtInput.Focus();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi đọc tài liệu: " + ex.Message, "Lỗi Đính Kèm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Hàm chung để hiển thị tin nhắn người dùng và gửi request cho AI
        /// </summary>
        private async void GuiTinNhanChoAI(string cauHoiHienThi, string cauHoiGuiAI)
        {
            // Hiển thị tin nhắn người dùng
            ThemTinNhan(cauHoiHienThi, true);

            // Khóa nút, hiển thị trạng thái chờ
            _dangChoAI = true;
            btnSend.Enabled = false;
            if (btnOcr != null) btnOcr.Enabled = false;
            btnSend.Text = "...";

            // Tạo bong bóng tạm hiển thị hiệu ứng "AI đang suy nghĩ..."
            Control lblIndicator = ThemTinNhan("⏳ AI đang suy nghĩ...", false);

            try
            {
                // Gọi Ollama API thật (async, không block UI)
                string traLoi = await _ollamaService.GuiTinNhan(cauHoiGuiAI);

                // Xóa bong bóng tạm trước khi hiển thị câu trả lời thật
                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator))
                {
                    pnlMain.Controls.Remove(lblIndicator);
                    lblIndicator.Dispose();
                }

                ThemTinNhan(traLoi, false);
            }
            catch (Exception ex)
            {
                // Xóa bong bóng tạm nếu xảy ra lỗi
                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator))
                {
                    pnlMain.Controls.Remove(lblIndicator);
                    lblIndicator.Dispose();
                }

                ThemTinNhan($"⚠ Lỗi: {ex.Message}", false);
            }
            finally
            {
                // Mở khóa nút sau khi xong
                _dangChoAI = false;
                btnSend.Enabled = true;
                if (btnOcr != null) btnOcr.Enabled = true;
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

        private void pnlSizebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}