using System; // Import thư viện System cơ bản
using System.Diagnostics; // Import để dùng Process (khởi chạy process ngầm)
using System.Drawing; // Import để dùng Color, Font, Point, Size...
using System.Threading.Tasks; // Import để xử lý bất đồng bộ async/await
using System.Windows.Forms; // Import thư viện UI Windows Forms
using OfficeOpenXml; // Import thư viện EPPlus để thao tác file Excel
using Newtonsoft.Json; // Import Newtonsoft để parse JSON
using System.Collections.Generic; // Import Generic collections (List)
using System.IO; // Import để thao tác file, path
using System.Configuration; // Import để đọc cấu hình từ App.config

namespace SEMI_FINAL // Bắt đầu namespace của ứng dụng
{
    public partial class Form1 : System.Windows.Forms.Form // Khai báo class Form1 kế thừa từ Form
    {
        private OllamaService _ollamaService = new OllamaService(
            ConfigurationManager.AppSettings["OllamaBaseUrl"] ?? "http://localhost:11434",
            ConfigurationManager.AppSettings["OllamaModel"] ?? "llama3.2"
        ); // Đọc URL và Model từ App.config (có thể ghi đè bằng secrets.config)
        
        private GeminiService _geminiService = new GeminiService(
            ConfigurationManager.AppSettings["GeminiApiKey"] ?? ""
        ); // Đọc API Key từ App.config (ghi đè bằng secrets.config)
        private bool _isGeminiSelected = false; // Biến cờ đánh dấu xem người dùng đang chọn Gemini hay Ollama
        private DocumentReaderService _docService; // Khai báo biến trỏ đến service đọc file văn bản
        private OcrService _ocrService; // Khai báo biến trỏ đến service nhận diện chữ OCR
        private string _fileDinhKemTen = null; // Biến lưu tên file đính kèm tạm thời
        private string _fileDinhKemNoiDung = null; // Biến lưu nội dung chữ trích xuất từ file đính kèm
        private string _fileDinhKemPath = null; // Biến lưu đường dẫn vật lý của file đính kèm
        private bool _dangChoAI = false; // Cờ khóa trạng thái để tránh spam gửi nhiều tin nhắn cùng lúc khi AI đang xử lý

        public Form1() // Hàm khởi tạo UI của Form1
        {
            InitializeComponent(); // Gọi hàm tự sinh của WinForms để khởi tạo các control giao diện
            this.Text = "AI Chat Assistant"; // Set tiêu đề cho cửa sổ ứng dụng
            _docService = new DocumentReaderService(); // Cấp phát bộ nhớ khởi tạo DocumentReaderService
            _ocrService = new OcrService(); // Cấp phát bộ nhớ khởi tạo OcrService

            txtInput.KeyDown += txtInput_KeyDown; // Gắn sự kiện nhấn phím (KeyDown) cho ô nhập liệu txtInput

            // Load ảnh logo cho PictureBox bằng code (tránh lỗi Designer)
            try
            {
                string imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "images.png"); // Đường dẫn ảnh trong thư mục Resources
                if (File.Exists(imgPath)) // Kiểm tra file ảnh có tồn tại không
                    guna2CirclePictureBox1.Image = Image.FromFile(imgPath); // Load ảnh từ file
            }
            catch { } // Bỏ qua nếu không tìm thấy ảnh - app vẫn chạy bình thường

            TuDongKhoiDongOllama(); // Gọi hàm kiểm tra và bật ngầm server Ollama nếu nó chưa chạy
        }

        private async void TuDongKhoiDongOllama() // Hàm bất đồng bộ xử lý việc tự bật Ollama
        {
            bool dangChay = await _ollamaService.KiemTraKetNoi(); // Gọi service để ping đến server Ollama xem có phản hồi không

            if (!dangChay) // Nếu ping thất bại (Ollama chưa chạy)
            {
                var processes = Process.GetProcessesByName("ollama"); // Tìm trong hệ điều hành xem có process nào tên "ollama" không
                if (processes.Length == 0) // Nếu hoàn toàn không có tiến trình nào
                {
                    try // Bắt đầu khối lệnh thử nghiệm bắt lỗi
                    {
                        var psi = new ProcessStartInfo() // Tạo cấu hình khởi chạy process mới
                        {
                            FileName = "ollama", // Tên file thực thi là ollama
                            Arguments = "serve", // Tham số truyền vào là serve để bật server
                            UseShellExecute = false, // Không sử dụng shell của OS để chạy (để ẩn cửa sổ)
                            CreateNoWindow = true, // Cài đặt không tạo cửa sổ CMD đen
                            WindowStyle = ProcessWindowStyle.Hidden // Ép ẩn hoàn toàn cửa sổ
                        };
                        Process.Start(psi); // Yêu cầu hệ điều hành khởi chạy tiến trình dựa trên cấu hình psi

                        await Task.Delay(3000); // Tạm dừng luồng hiện tại 3 giây để chờ server Ollama kịp load lên
                    }
                    catch // Bắt mọi lỗi xảy ra khi gọi Process.Start
                    {
                        MessageBox.Show( // Hiển thị popup thông báo lỗi cho người dùng
                            "Không thể tự khởi động Ollama.\n\n" + // Thông báo dòng 1
                            "Hãy mở Command Prompt và chạy:\n" + // Hướng dẫn dòng 2
                            "   ollama serve\n\n" + // Lệnh chạy thủ công
                            "Sau đó khởi động lại ứng dụng.", // Hướng dẫn bước cuối
                            "Ollama chưa chạy", // Tiêu đề popup
                            MessageBoxButtons.OK, // Nút OK
                            MessageBoxIcon.Warning); // Icon cảnh báo màu vàng
                        return; // Kết thúc hàm sớm nếu lỗi
                    }
                }
                else // Nếu có tiến trình ollama nhưng chưa ping được
                {
                    await Task.Delay(2000); // Chờ thêm 2 giây cho nó khởi động xong
                }
            }

            KiemTraKetNoiOllama(); // Gọi hàm kiểm tra lại một lần nữa
            TaiDanhSachModel(); // Gọi hàm tự động lấy các model đang có trong Ollama
        }

        private async void TaiDanhSachModel() // Hàm tải danh sách các LLM từ Ollama về combobox
        {
            cb_model.Enabled = false; // Tạm khóa combobox model để người dùng không click lúc đang tải
            cb_model.Items.Clear(); // Xóa toàn bộ dữ liệu cũ trong combobox
            cb_model.Items.Add("⏳ Đang tải..."); // Thêm mục "Đang tải" làm placeholder
            cb_model.SelectedIndex = 0; // Chọn mục đầu tiên vừa thêm

            var models = await _ollamaService.LayDanhSachModel(); // Gọi API lấy list models từ service

            cb_model.Items.Clear(); // Xóa placeholder "Đang tải" đi

            if (models.Count == 0) // Nếu không tìm thấy model nào
            {
                cb_model.Items.Add("(Chưa có model Local - Click để tải)"); // Báo chưa có model
                ThemTinNhan("⚠️ Hệ thống chưa tìm thấy Model Local nào trên Ollama.\n👉 Bấm vào nút [📥 Tải Model Local] ở góc dưới bên trái để tải mô hình về máy (ví dụ: llama3.2).", false);
            }
            else // Nếu có model
            {
                foreach (var m in models) // Duyệt qua từng model lấy được
                    cb_model.Items.Add(m); // Đẩy tên model đó vào combobox
            }
            
            cb_model.Items.Add("Gemini 2.5 Flash"); // Thêm thủ công tùy chọn model Gemini của Google

            string currentModel = "llama3.2"; // Đặt model mặc định mong muốn là llama3.2
            int idx = models.FindIndex(m => m.StartsWith(currentModel)); // Tìm index của model này trong danh sách tải về
            cb_model.SelectedIndex = idx >= 0 ? idx : 0; // Nếu tìm thấy thì set chọn, không thì chọn mục đầu tiên (index 0)

            cb_model.Enabled = true; // Mở khóa lại combobox cho người dùng chọn
        }

        private void cboModel_SelectedIndexChanged(object sender, EventArgs e) // Sự kiện khi người dùng đổi model trong combobox
        {
            string selected = cb_model.SelectedItem?.ToString(); // Lấy tên model vừa được chọn
            if (string.IsNullOrEmpty(selected) || selected.StartsWith("⏳")) // Bỏ qua nếu là placeholder
                return; // Thoát hàm

            if (selected.StartsWith("(Chưa có model"))
            {
                HienThiDialogTaiModel();
                return;
            }

            if (selected == "Gemini 2.5 Flash" || selected.StartsWith("Gemini")) // Nếu người dùng chọn model Gemini
            {
                _isGeminiSelected = true; // Đổi cờ Gemini thành true
                
                if (!_geminiService.HasKey)
                {
                    ThemTinNhan("⚠️ Bạn chưa cấu hình Gemini API Key! Bấm vào nút [🔑 Cấu hình API Key] ở góc dưới bên trái để nhập Key.", false);
                    HienThiDialogNhapApiKey(true);
                }
                else
                {
                    ThemTinNhan($"🤖 Đã chuyển sang model: {selected}", false); // In thông báo hệ thống ra màn hình chat
                }
            }
            else // Nếu chọn model khác (thuộc Ollama)
            {
                _isGeminiSelected = false; // Đổi cờ Gemini thành false
                _ollamaService.SetModel(selected); // Cập nhật tên model vào cấu hình của Ollama Service
                ThemTinNhan($"🤖 Đã chuyển sang model: {selected}", false); // In thông báo chuyển model thành công
            }
        }

        private void btnDownloadModel_Click(object sender, EventArgs e)
        {
            HienThiDialogTaiModel();
        }

        private void HienThiDialogTaiModel()
        {
            using (FormDownloadModelDialog dlg = new FormDownloadModelDialog(_ollamaService))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    TaiDanhSachModel(); // Refresh lại danh sách model sau khi tải xong
                }
            }
        }

        private void btnApiKey_Click(object sender, EventArgs e)
        {
            HienThiDialogNhapApiKey(false);
        }

        private void HienThiDialogNhapApiKey(bool fromAutoPrompt)
        {
            using (FormApiKeyDialog dlg = new FormApiKeyDialog(_geminiService.ApiKey))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _geminiService.SetApiKey(dlg.ApiKey);
                    ThemTinNhan("✅ Đã cập nhật Gemini API Key thành công!", false);
                }
            }
        }

        private async void KiemTraKetNoiOllama() // Hàm kiểm tra lại kết nối lần cuối sau khi chạy
        {
            bool ketNoi = await _ollamaService.KiemTraKetNoi(); // Ping server
            
            if (!ketNoi) // Nếu vẫn không kết nối được
            {
                MessageBox.Show( // Bật thông báo lỗi
                    "Không thể kết nối máy chủ Ollama!\n\n" + // Báo lỗi
                    "Hãy kiểm tra các bước sau:\n" + // Gợi ý xử lý
                    "1. Nếu chạy máy local: gõ 'ollama serve' trong Terminal.\n" + // Cách xử lý local
                    "2. Nếu kết nối máy khác trong mạng LAN:\n" + // Cách xử lý mạng LAN
                    "   - Đảm bảo đã nhập đúng IP máy đó trong Form1.cs\n" + // Check IP
                    "   - Đảm bảo máy chủ Ollama đã cấu hình biến môi trường OLLAMA_HOST=0.0.0.0\n" + // Check biến môi trường
                    "   - Kiểm tra tường lửa (Firewall) máy chủ đã mở cổng 11434 chưa.", // Check firewall
                    "Lỗi kết nối Ollama", // Tiêu đề
                    MessageBoxButtons.OK, // Nút OK
                    MessageBoxIcon.Warning); // Icon
            }
        }

        private int GetNextMessageY() // Hàm tính tọa độ trục Y cho tin nhắn tiếp theo trong khung chat
        {
            int bottomMost = 15; // Mặc định tọa độ đầu tiên là 15 (cách viền trên 15px)
            foreach (Control c in pnlMain.Controls) // Duyệt qua mọi bong bóng chat (Control) đang có trong panel chính
            {
                if (c.Visible && c.Bottom + 15 > bottomMost) // Nếu control đó đang hiển thị và đáy của nó thấp hơn giá trị lớn nhất hiện tại
                {
                    bottomMost = c.Bottom + 15; // Cập nhật lại giá trị tọa độ thấp nhất (cộng thêm 15px margin)
                }
            }
            return bottomMost; // Trả về tọa độ Y an toàn để vẽ tin nhắn mới không đè lên cái cũ
        }

        private Control ThemTinNhan(string noiDung, bool laNguoiDung) // Hàm tạo và render bong bóng chat
        {
            int maxWidth = pnlMain.Width - 80; // Giới hạn chiều rộng tối đa của tin nhắn để không sát viền
            var font = new Font("Inter", 11.5f, FontStyle.Regular); // Định nghĩa Font chữ cho tin nhắn (Inter - font UI hiện đại từ Google Fonts)
            int padH = 24; // Tính padding ngang trái + phải (12px * 2)

            Size textSize = TextRenderer.MeasureText( // Tính toán kích thước thật của chuỗi dựa trên font
                noiDung, // Chuỗi cần tính
                font, // Font sử dụng
                new Size(maxWidth - padH - 10, int.MaxValue), // Khung hình chữ nhật tối đa để bọc chữ
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl // Cờ cho phép tự xuống dòng theo từ
            );

            int bubbleWidth = Math.Min(textSize.Width + padH + 16, maxWidth); // Tính chiều rộng bong bóng, chốt mức tối đa
            if (!laNguoiDung) bubbleWidth = Math.Max(bubbleWidth, 250); // Nếu là AI thì rộng tối thiểu 250 để chứa được các nút bấm
            int rtbWidth = bubbleWidth - padH; // Chiều rộng cho phần TextBox bên trong
            int rtbHeight = textSize.Height + 10; // Chiều cao TextBox bên trong

            Guna.UI2.WinForms.Guna2Panel bubblePanel = new Guna.UI2.WinForms.Guna2Panel(); // Khởi tạo Panel bong bóng sử dụng GunaUI
            bubblePanel.BorderRadius = 14; // Bo tròn góc bong bóng 14px Fluent style
            Color bgColor = laNguoiDung ? Color.FromArgb(0, 132, 255) : Color.FromArgb(24, 26, 34); // Nền: Gradient Xanh Stitch (#0084FF) hoặc Slate Dark (#181A22)
            Color fgColor = laNguoiDung ? Color.White : Color.FromArgb(235, 238, 245); // Màu chữ tương ứng
            bubblePanel.FillColor = bgColor; // Đổ màu nền cho Guna Panel
            bubblePanel.BackColor = Color.Transparent; // Đặt nền viền trong suốt
            if (!laNguoiDung)
            {
                bubblePanel.BorderColor = Color.FromArgb(45, 50, 65);
                bubblePanel.BorderThickness = 1;
            }

            RichTextBox rtb = new RichTextBox(); // Khởi tạo RichTextBox để chứa chữ
            rtb.Font = font; // Set font
            rtb.ReadOnly = true; // Chặn người dùng sửa nội dung
            rtb.BorderStyle = BorderStyle.None; // Xóa viền của RichTextBox
            rtb.BackColor = bgColor; // Đồng bộ màu nền
            rtb.ForeColor = fgColor; // Đồng bộ màu chữ
            rtb.ScrollBars = RichTextBoxScrollBars.None; // Tắt thanh cuộn
            rtb.Location = new Point(12, 12); // Đặt vị trí text bên trong panel
            rtb.Size = new Size(rtbWidth, rtbHeight);
            
            ApplyMarkdown(rtb, noiDung); // Parse định dạng Markdown

            Button btnCopy = null; // Khởi tạo nút Copy
            if (!laNguoiDung) // Chỉ tạo nút Copy nếu là tin nhắn của AI
            {
                btnCopy = new Button(); // Tạo Button mới
                btnCopy.Text = "📋 Sao chép"; // Gán nhãn cho nút
                btnCopy.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold); // Set font nhỏ và bold
                btnCopy.Size = new Size(95, 26); // Kích thước nút
                btnCopy.FlatStyle = FlatStyle.Flat; // Set kiểu nút phẳng
                btnCopy.FlatAppearance.BorderSize = 0; // Bỏ viền nút
                btnCopy.BackColor = Color.FromArgb(50, 50, 50); // Màu nền nút xám Stitch
                btnCopy.ForeColor = Color.FromArgb(220, 220, 220); // Màu chữ xám sáng
                btnCopy.Cursor = Cursors.Hand; // Con trỏ chuột hình bàn tay
                btnCopy.Location = new Point(12, rtb.Bottom + 6); // Đặt vị trí ngay dưới RichTextBox

                btnCopy.Click += (s, e) => { // Định nghĩa sự kiện khi click vào nút Copy
                    try { // Bọc try catch tránh lỗi Clipboard
                        if (!string.IsNullOrEmpty(noiDung)) // Nếu nội dung không rỗng
                        {
                            Clipboard.SetText(noiDung); // Lưu nội dung thô vào Clipboard của Windows
                            btnCopy.Text = "✔ Đã chép!"; // Đổi nhãn thông báo thành công
                            Timer t = new Timer { Interval = 2000 }; // Khởi tạo bộ đếm 2 giây
                            t.Tick += (ts, te) => { // Sự kiện khi hết 2 giây
                                btnCopy.Text = "📋 Sao chép"; // Đổi lại nhãn ban đầu
                                t.Stop(); // Dừng đếm
                                t.Dispose(); // Hủy biến timer
                            };
                            t.Start(); // Bắt đầu đếm
                        }
                    } catch { } // Bỏ qua lỗi nếu có
                };
            }

            bubblePanel.Size = new Size(bubbleWidth, (btnCopy != null ? btnCopy.Bottom : rtb.Bottom) + 12); // Set kích thước cuối cùng của Panel

            ContextMenuStrip menuSaoChep = new ContextMenuStrip(); // Khởi tạo menu chuột phải
            ToolStripMenuItem itemCopyAll = new ToolStripMenuItem("📋 Sao chép toàn bộ tin nhắn"); // Nút copy toàn bộ
            itemCopyAll.Click += (s, e) => { // Khi click nút
                try { if (!string.IsNullOrEmpty(noiDung)) Clipboard.SetText(noiDung); } catch { } // Lưu toàn bộ text vào clipboard
            };
            ToolStripMenuItem itemCopySel = new ToolStripMenuItem("✂ Sao chép đoạn văn bản đã chọn"); // Nút copy chỗ đang bôi đen
            itemCopySel.Click += (s, e) => { // Khi click nút
                try { // Bắt lỗi
                    if (!string.IsNullOrEmpty(rtb.SelectedText)) // Nếu có bôi đen
                        Clipboard.SetText(rtb.SelectedText); // Copy phần đó
                    else if (!string.IsNullOrEmpty(noiDung)) // Nếu không bôi đen gì
                        Clipboard.SetText(noiDung); // Copy tất cả
                } catch { } // Bỏ qua lỗi
            };
            menuSaoChep.Items.Add(itemCopyAll); // Thêm mục 1 vào menu
            menuSaoChep.Items.Add(itemCopySel); // Thêm mục 2 vào menu

            rtb.ContextMenuStrip = menuSaoChep; // Gắn menu cho rtb
            bubblePanel.ContextMenuStrip = menuSaoChep; // Gắn menu cho panel

            rtb.ContentsResized += (s, e) => { // Bắt sự kiện mỗi khi kích thước nội dung của RichTextBox tự động thay đổi
                int neededHeight = e.NewRectangle.Height + 6; // Tính chiều cao mới cần thiết
                if (Math.Abs(rtb.Height - neededHeight) > 2) // Nếu sai lệch quá 2 pixel
                {
                    rtb.Height = neededHeight; // Cập nhật chiều cao rtb
                    if (btnCopy != null) // Nếu có nút copy
                    {
                        btnCopy.Location = new Point(12, rtb.Bottom + 6); // Dịch chuyển nút copy xuống dưới theo
                        bubblePanel.Height = btnCopy.Bottom + 12; // Mở rộng panel theo
                    }
                    else // Nếu không có nút
                    {
                        bubblePanel.Height = rtb.Bottom + 12; // Mở rộng panel vừa đủ với text
                    }
                }
            };

            bubblePanel.Controls.Add(rtb); // Nhét RichTextBox vào trong Panel
            if (btnCopy != null) bubblePanel.Controls.Add(btnCopy); // Nhét nút Copy vào trong Panel

            int toaDoX = laNguoiDung ? (pnlMain.Width - bubbleWidth - 25) : 15; // Nếu là người dùng thì canh phải, AI thì canh trái
            int toaDoY = GetNextMessageY(); // Lấy tọa độ trục Y tiếp theo
            bubblePanel.Location = new Point(toaDoX, toaDoY); // Chốt tọa độ cho bong bóng

            pnlMain.Controls.Add(bubblePanel); // Nhét panel bong bóng vào khung cuộn chat chính
            pnlMain.Invalidate(); // Ép giao diện vẽ lại
            pnlMain.ScrollControlIntoView(bubblePanel); // Tự động cuộn chuột xuống tin nhắn mới nhất vừa thêm

            return bubblePanel; // Trả về đối tượng panel để hàm gọi có thể thao tác (như xóa khi đang loading)
        }

        private void ApplyMarkdown(RichTextBox rtb, string text) // Hàm hỗ trợ render các cấu trúc thẻ markdown thành chữ có định dạng (màu, bold)
        {
            rtb.Text = ""; // Xóa trắng rtb trước khi render
            bool isCode = false; // Biến cờ kiểm tra xem dòng hiện tại có nằm trong vùng thẻ 3 dấu backtick hay không
            
            string[] lines = text.Split('\n'); // Chia toàn bộ văn bản ra thành từng dòng
            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++) // Vòng lặp duyệt qua tất cả các dòng
            {
                string line = lines[lineIdx]; // Gán dòng hiện tại vào biến
                string lineBreak = (lineIdx < lines.Length - 1) ? "\n" : ""; // Xác định xem dòng này có cần dấu xuống dòng không (nếu k phải dòng cuối)

                if (line.Trim().StartsWith("```")) // Kiểm tra nếu dòng bắt đầu bằng 3 dấu backtick
                {
                    isCode = !isCode; // Đảo ngược cờ isCode (Đóng/Mở block code)
                    continue; // Bỏ qua dòng chứa dấu ``` này (không render ra rtb)
                }

                if (isCode) // Nếu đang nằm trong block code
                {
                    AppendText(rtb, line + lineBreak, new Font("Cascadia Mono", 10.5f, FontStyle.Regular), Color.FromArgb(235, 179, 52)); // Chèn text với font code Cascadia Mono, màu vàng
                }
                else // Nếu là văn bản thường
                {
                    if (line.StartsWith("### ")) // Nếu là Header H3
                    {
                        AppendText(rtb, line.Substring(4) + lineBreak, new Font(rtb.Font.FontFamily, 13, FontStyle.Bold), rtb.ForeColor); // Chữ to và in đậm
                    }
                    else if (line.StartsWith("## ")) // Nếu là Header H2
                    {
                        AppendText(rtb, line.Substring(3) + lineBreak, new Font(rtb.Font.FontFamily, 14, FontStyle.Bold), rtb.ForeColor); // Chữ to hơn và in đậm
                    }
                    else if (line.StartsWith("# ")) // Nếu là Header H1
                    {
                        AppendText(rtb, line.Substring(2) + lineBreak, new Font(rtb.Font.FontFamily, 16, FontStyle.Bold), rtb.ForeColor); // Chữ to nhất và in đậm
                    }
                    else // Các dòng văn bản bình thường khác
                    {
                        ParseInlineMarkdown(rtb, line + lineBreak); // Gọi hàm parse thẻ inline cho từng dòng này
                    }
                }
            }
        }

        private void AppendText(RichTextBox rtb, string text, Font font, Color color) // Hàm tiện ích để chèn text có màu và font vào cuối Rtb
        {
            rtb.SelectionStart = rtb.TextLength; // Di chuyển con trỏ bôi đen xuống cuối văn bản
            rtb.SelectionLength = 0; // Đảm bảo không bôi đen xóa đè bất kỳ kí tự nào
            rtb.SelectionFont = font; // Cài đặt font chữ cho đoạn chuẩn bị chèn
            rtb.SelectionColor = color; // Cài đặt màu chữ cho đoạn chuẩn bị chèn
            rtb.AppendText(text); // Nối chuỗi vào đuôi rtb
            rtb.SelectionColor = rtb.ForeColor; // Reset màu lại theo rtb mặc định
            rtb.SelectionFont = rtb.Font; // Reset font lại mặc định
        }

        private void ParseInlineMarkdown(RichTextBox rtb, string line) // Hàm xử lý thẻ bold (**) và inline code (`) giữa dòng văn bản
        {
            int i = 0; // Khởi tạo con trỏ chạy quét ký tự
            int lastIndex = 0; // Khởi tạo vị trí đánh dấu từ cuối cùng đã parse xong
            while (i < line.Length) // Vòng lặp duyệt từng ký tự
            {
                if (i <= line.Length - 2 && line.Substring(i, 2) == "**") // Kiểm tra xem có bắt gặp cụm '**' không
                {
                    int end = line.IndexOf("**", i + 2); // Tìm vị trí của cụm '**' đóng tiếp theo
                    if (end != -1) // Nếu tìm thấy cụm đóng
                    {
                        if (i > lastIndex) AppendText(rtb, line.Substring(lastIndex, i - lastIndex), rtb.Font, rtb.ForeColor); // Thêm phần chữ bình thường trước thẻ bold
                        string boldText = line.Substring(i + 2, end - i - 2); // Trích xuất vùng text nằm kẹp giữa 2 thẻ bold
                        AppendText(rtb, boldText, new Font(rtb.Font, FontStyle.Bold), rtb.ForeColor); // Chèn text đó dưới dạng in đậm
                        i = end + 2; // Đẩy con trỏ chạy vượt qua thẻ bold đóng
                        lastIndex = i; // Đánh dấu lại mốc đã xử lý
                        continue; // Tiếp tục vòng lặp
                    }
                }
                if (line[i] == '`') // Kiểm tra xem có gặp dấu tick '`' (inline code)
                {
                    int end = line.IndexOf('`', i + 1); // Tìm vị trí đóng '`'
                    if (end != -1) // Nếu có đóng
                    {
                        if (i > lastIndex) AppendText(rtb, line.Substring(lastIndex, i - lastIndex), rtb.Font, rtb.ForeColor); // Nhét text thường phía trước
                        string codeText = line.Substring(i + 1, end - i - 1); // Lấy chữ bên trong thẻ '`'
                        AppendText(rtb, codeText, new Font("Cascadia Mono", rtb.Font.Size), Color.FromArgb(235, 179, 52)); // Chèn với font code Cascadia Mono và màu cam
                        i = end + 1; // Vượt trỏ qua thẻ
                        lastIndex = i; // Reset mốc
                        continue; // Bỏ qua lệnh ở dưới đi tiếp
                    }
                }
                i++; // Tiến con trỏ lên 1 nếu ko rơi vào case nào
            }
            if (lastIndex < line.Length) // Xử lý nốt phần râu ria văn bản thường từ dấu mốc cuối cùng đến hết dòng
            {
                AppendText(rtb, line.Substring(lastIndex), rtb.Font, rtb.ForeColor); // Chèn text thường
            }
        }

        private void btnSend_Click(object sender, EventArgs e) // Sự kiện khi người dùng click vào nút GỬI
        {
            if (_dangChoAI) return; // Nếu hệ thống đang gọi API và chưa phản hồi thì chặn click

            string cauHoi = txtInput.Text.Trim(); // Lấy chữ trong ô nhập liệu và xóa khoảng trắng 2 đầu

            if (_fileDinhKemPath == null && string.IsNullOrEmpty(cauHoi)) return; // Nếu ko có cả file lẫn chữ thì không làm gì

            string currentFilePath = _fileDinhKemPath; // Lưu lại file đính kèm vào biến local an toàn
            bool isImage = currentFilePath != null && (currentFilePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || // Check xem file đính kèm có phải là ảnh jpg không
                                                       currentFilePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || // Check jpeg
                                                       currentFilePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || // Check png
                                                       currentFilePath.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)); // Check webp

            if (_fileDinhKemPath != null) // Trường hợp nếu người dùng có đính kèm file
            {
                string userPrompt = string.IsNullOrEmpty(cauHoi) // Kiểm tra xem người dùng có gõ thêm chữ nào không
                    ? (isImage ? "Hãy mô tả và phân tích hình ảnh này chi tiết bằng tiếng Việt." : "Hãy phân tích, tóm tắt ý chính và giải thích chi tiết nội dung quan trọng từ tài liệu đính kèm bằng tiếng Việt rõ ràng.") // Tự gen câu lệnh mặc định nếu người dùng ko nhập gì
                    : cauHoi; // Nếu người dùng nhập thì lấy câu của họ

                string cauHoiHienThi = string.IsNullOrEmpty(cauHoi) // Xây dựng chuỗi để hiển thị lên UI chatbox (ko kèm data rác)
                    ? $"[{(isImage ? "🖼" : "📎")} Đính kèm: {_fileDinhKemTen}]\n(Yêu cầu: {(isImage ? "Phân tích ảnh" : "Phân tích tài liệu")})" // Hàng hiển thị
                    : $"[{(isImage ? "🖼" : "📎")} Đính kèm: {_fileDinhKemTen}]\n{userPrompt}"; // Hàng hiển thị

                string systemInstruction = "\n\n[LƯU Ý QUAN TRỌNG: Bạn là một trợ lý ảo người Việt Nam. Bạn BẮT BUỘC phải trả lời bằng Tiếng Việt trong mọi tình huống. Tuyệt đối không được dùng Tiếng Anh.]"; // Chèn mưu hèn kế bẩn ép AI xài tiếng Việt
                
                string cauHoiGuiAI = userPrompt + systemInstruction; // Trộn câu hỏi và lệnh ngầm gửi qua mạng
                
                if (!isImage && _fileDinhKemNoiDung != null) // Trường hợp file là PDF/Word/Text (đã có nội dung text trích xuất)
                {
                    cauHoiGuiAI = $"Dưới đây là dữ liệu được trích xuất từ tài liệu \"{_fileDinhKemTen}\":\n" + // Mở đầu prompt data
                                  $"--------------------------------------------\n" + // Kẻ dòng
                                  $"{_fileDinhKemNoiDung}\n" + // Nhồi nội dung text hàng ngàn chữ của file vào
                                  $"--------------------------------------------\n\n" + // Kẻ dòng đóng
                                  $"Yêu cầu/chú thích của người dùng: \"{userPrompt}\"" + systemInstruction; // Dán câu hỏi
                }

                _fileDinhKemTen = null; // Xóa biến đính kèm tên (Reset trạng thái đính kèm)
                _fileDinhKemNoiDung = null; // Xóa nội dung
                _fileDinhKemPath = null; // Xóa đường dẫn
                if (btnOcr != null) // Nếu nút đính kèm (btnOcr) tồn tại trên form
                {
                    btnOcr.Text = "➕ File"; // Trả về nhãn mặc định
                    btnOcr.FillColor = Color.FromArgb(64, 64, 64); // Đổi về màu xám đen
                }

                txtInput.Clear(); // Xóa khung nhập chữ
                GuiTinNhanChoAI(cauHoiHienThi, cauHoiGuiAI, isImage ? currentFilePath : null); // Truyền dữ liệu sang hàm gọi AI
            }
            else // Trường hợp người dùng ko tải file nào cả, chỉ chat suông
            {
                txtInput.Clear(); // Xóa khung nhập
                string cauHoiThongThuong = cauHoi + "\n\n[LƯU Ý: Bạn BẮT BUỘC phải trả lời bằng Tiếng Việt, không dùng Tiếng Anh.]"; // Ép tiếng việt
                GuiTinNhanChoAI(cauHoi, cauHoiThongThuong, null); // Gửi sang AI xử lý, không có file
            }
        }

        private void btnOcr_Click(object sender, EventArgs e) // Sự kiện khi người dùng nhấn nút Đính kèm (có logo dấu +)
        {
            if (_dangChoAI) return; // Chặn nếu AI đang bận suy nghĩ thì ko cho up file

            using (OpenFileDialog ofd = new OpenFileDialog()) // Khởi tạo hộp thoại mở file của windows
            {
                ofd.Title = "Chọn tài liệu (Word, PDF, Markdown...) để đính kèm"; // Gắn tên cho cửa sổ bật lên
                ofd.Filter = "Tất cả định dạng (Ảnh, PDF, Word...)|*.pdf;*.doc;*.docx;*.md;*.txt;*.csv;*.json;*.xml;*.jpg;*.jpeg;*.png;*.webp|" + // Định dạng tổng hợp
                             "Tài liệu Word (*.doc;*.docx)|*.doc;*.docx|" + // Bộ lọc word
                             "Tài liệu PDF (*.pdf)|*.pdf|" + // Bộ lọc pdf
                             "Văn bản & Markdown (*.md;*.txt...)|*.md;*.txt;*.csv;*.json|" + // Bộ lọc text
                             "Ảnh (Vision AI) (*.jpg;*.png...)|*.jpg;*.jpeg;*.png;*.webp|" + // Bộ lọc hình ảnh
                             "Tất cả các file (*.*)|*.*"; // Cho phép chọn all

                if (ofd.ShowDialog() == DialogResult.OK) // Gọi hộp thoại hiện lên, kiểm tra xem người dùng có bấm OK hay ko
                {
                    try // Bọc thử nghiệm để bắt lỗi quá trình đọc file hỏng
                    {
                        string filePath = ofd.FileName; // Lấy full đường dẫn của file vừa chọn
                        string fileName = System.IO.Path.GetFileName(filePath); // Tách lấy tên file ngắn
                        string ext = System.IO.Path.GetExtension(filePath).ToLowerInvariant(); // Tách đuôi file, hạ chữ thường
                        
                        string extractedText = ""; // Biến chứa chuỗi văn bản trích xuất 
                        bool isImage = ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp"; // Xác định cờ nếu là ảnh
                        
                        if (isImage) // Nếu người dùng tải ảnh lên
                        {
                            var result = MessageBox.Show("Bạn muốn xuất bảng trong ảnh này ra file Excel bằng AI không?\n\nChọn [Yes] để xuất Excel tự động.\nChọn [No] để nhận diện chữ thông thường (OCR local).", "Xử lý ảnh", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question); // Hỏi xem mục đích xài ảnh làm gì
                            
                            if (result == DialogResult.Cancel) return; // Nếu chọn Hủy (Cancel) thì đóng và không làm gì
                            
                            if (result == DialogResult.Yes) // Nếu chọn Yes -> Export ra Excel qua mạng
                            {
                                _ = XuatExcelTuAnh(filePath, fileName); // Gọi hàm export bất đồng bộ chạy ngầm (fire and forget)
                                return; // Kết thúc hàm
                            }
                            
                            try // Nhánh nếu chọn NO (Dùng Tesseract OCR offline nhận diện chữ)
                            {
                                ThemTinNhan($"⏳ Đang quét ảnh để nhận diện chữ (OCR local): \"{fileName}\"...", false); // In thông báo hệ thống bắt đầu quyét
                                string textFromImage = _ocrService.ExtractTextFromImage(filePath); // Khởi chạy service Tesseract đọc ảnh
                                if (string.IsNullOrWhiteSpace(textFromImage)) // Nếu quét xong chả ra chữ nào
                                {
                                    ThemTinNhan("⚠ Không nhận dạng được chữ nào từ bức ảnh này.", false); // In thông báo lỗi
                                }
                                else // Nếu quét ra chữ
                                {
                                    ThemTinNhan($"📝 Đã trích xuất chữ từ ảnh '{fileName}':\n\n{textFromImage}", false); // In kết quả cho người dùng copy
                                }
                            }
                            catch (Exception ex) // Bắt lỗi OCR thư viện (thiếu ngôn ngữ, lỗi hình ảnh)
                            {
                                ThemTinNhan($"⚠ Lỗi khi OCR ảnh: {ex.Message}", false); // In báo lỗi
                            }
                            return; // Ngắt hàm (ảnh chỉ dùng đọc chữ tại chỗ chứ không đưa lên chatbox)
                        }
                        
                        extractedText = _docService.ReadDocument(filePath); // Trường hợp file là PDF/Word, gọi Document Service lấy text
                        if (string.IsNullOrWhiteSpace(extractedText)) // Nếu file Word hay PDF chỉ là rỗng hoặc hỏng
                        {
                            MessageBox.Show("File không chứa nội dung văn bản hoặc trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Bật popup báo lỗi
                            return; // Dừng lại
                        }

                        _fileDinhKemTen = fileName; // Lưu tên file vào bộ đệm của form
                        _fileDinhKemNoiDung = extractedText; // Lưu phần text trích xuất vào bộ nhớ form (hàng ngàn chữ)
                        _fileDinhKemPath = filePath; // Lưu đường dẫn tạm

                        if (btnOcr != null) // Nếu nút up file không null
                        {
                            btnOcr.Text = "📎 " + (fileName.Length > 8 ? fileName.Substring(0, 8) + "..." : fileName); // Đổi chữ nút thành tên file cắt ngắn cho gọn
                            btnOcr.FillColor = Color.MediumSeaGreen; // Đổi nền nút sang màu xanh lá cây để báo là đã dính file
                        }

                        string msg = $"📎 Đã tải lên tài liệu: \"{fileName}\" ({extractedText.Length} ký tự).\n👉 Hãy nhập chú thích hoặc câu hỏi của bạn vào ô bên dưới."; // Xây chuỗi thông báo
                        ThemTinNhan(msg, false); // Bắn câu thông báo lên khung chat để người dùng biết file up thành công
                        txtInput.Focus(); // Chuyển con trỏ chuột thẳng vào vùng nhập text
                    }
                    catch (Exception ex) // Nếu lỗi văng ra trong lúc đọc PDF/Word
                    {
                        MessageBox.Show("Lỗi khi đọc tài liệu: " + ex.Message, "Lỗi Đính Kèm", MessageBoxButtons.OK, MessageBoxIcon.Error); // Bật popup đỏ
                    }
                }
            }
        }

        private async void GuiTinNhanChoAI(string cauHoiHienThi, string cauHoiGuiAI, string imagePath = null) // Hàm tổng hợp xử lý điều hướng call API AI
        {
            ThemTinNhan(cauHoiHienThi, true); // Render tin nhắn của người dùng lên UI cho họ đọc

            _dangChoAI = true; // Bật cờ khóa UI chặn spam click gửi
            btnSend.Enabled = false; // Disable nút gửi
            if (btnOcr != null) btnOcr.Enabled = false; // Disable nút upload file
            btnSend.Text = "..."; // Đổi nhãn nút sang trạng thái loading

            Control lblIndicator = ThemTinNhan("⏳ AI đang suy nghĩ...", false); // Nhét vào một bong bóng loading giả của AI chờ

            try // Bọc thử nghiệm để gọi API
            {
                string traLoi = ""; // Biến chứa hồi đáp của AI
                if (_isGeminiSelected) // Nếu model chọn là Google Gemini
                {
                    if (!_geminiService.HasKey)
                    {
                        if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator))
                        {
                            pnlMain.Controls.Remove(lblIndicator);
                            lblIndicator.Dispose();
                        }
                        ThemTinNhan("⚠️ Vui lòng nhập Gemini API Key để gửi tin nhắn!", false);
                        HienThiDialogNhapApiKey(true);
                        return;
                    }
                    traLoi = await _geminiService.GuiTinNhan(cauHoiGuiAI, imagePath); // Đẩy yêu cầu qua internet tới Google API
                }
                else // Nếu model là Local (Llama, mistral...)
                {
                    traLoi = await _ollamaService.GuiTinNhan(cauHoiGuiAI, imagePath); // Đẩy yêu cầu tới port 11434 của localhost Ollama
                }

                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator)) // Kiểm tra xem cái bong bóng giả kia còn trên màn ko
                {
                    pnlMain.Controls.Remove(lblIndicator); // Nhổ cái bong bóng giả đi
                    lblIndicator.Dispose(); // Tiêu hủy nó trong ram
                }

                ThemTinNhan(traLoi, false); // Nhét câu trả lời thật sự từ AI vào UI
            }
            catch (Exception ex) // Bắt lỗi mạng hoặc lỗi API server
            {
                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator)) // Tìm bong bóng loading
                {
                    pnlMain.Controls.Remove(lblIndicator); // Bỏ bong bóng loading
                    lblIndicator.Dispose(); // Hủy ram
                }

                ThemTinNhan($"⚠ Lỗi: {ex.Message}", false); // Trả về thông báo lỗi dạng bong bóng chat đỏ
            }
            finally // Dù lỗi hay ko thì vẫn phải chạy qua bước này
            {
                _dangChoAI = false; // Mở khóa chặn spam
                btnSend.Enabled = true; // Mở lại nút gửi
                if (btnOcr != null) btnOcr.Enabled = true; // Mở lại nút up file
                btnSend.Text = "GỬI"; // Đặt lại nhãn gửi
            }
        }

        private void btnSend_KeyDown(object sender, KeyEventArgs e) // Sự kiện ấn phím khi đang focus vào Nút Gửi (Thừa, ít dùng)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift) // Kiểm tra nếu nhấn Enter mà ko có đè Shift
            {
                e.SuppressKeyPress = true; // Chặn tiếng Beep khó chịu của Windows
                btnSend_Click(this, new EventArgs()); // Kích hoạt sự kiện bấm gửi
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e) // Sự kiện nhấn phím Enter khi đang gõ phím ở textbox chat
        {
            if (e.KeyCode == Keys.Enter && !e.Shift) // Phát hiện thao tác ấn Enter để nộp bài
            {
                e.SuppressKeyPress = true; // Triệt tiêu phím Enter gốc để ngăn việc nhảy xuống dòng
                btnSend_Click(this, new EventArgs()); // Gọi hàm bấm nút Gửi thay cho việc bấm bằng chuột
            }
        }

        private async Task XuatExcelTuAnh(string filePath, string fileName) // Tính năng đặc biệt: Nhờ AI Gemini đọc bảng trong ảnh và ghi Excel
        {
            ThemTinNhan($"⏳ Đang phân tích cấu trúc bảng trong ảnh bằng AI (Gemini): \"{fileName}\"...", true); // Báo trạng thái lên khung chat
            
            _dangChoAI = true; // Khóa UI ko cho nhấn gửi lung tung
            btnSend.Enabled = false; // Disable nút chat
            if (btnOcr != null) btnOcr.Enabled = false; // Disable tải ảnh khác
            Control lblIndicator = ThemTinNhan("⏳ Trí tuệ nhân tạo đang xử lý ảnh và bóc tách dữ liệu...", false); // Hiện bong bóng chờ loading
            
            try // Thử gọi mạng và gọi file IO
            {
                string prompt = "Hãy phân tích hình ảnh này. Tìm các bảng dữ liệu có trong ảnh và trích xuất chúng. Trả về kết quả CHỈ DƯỚI ĐỊNH DẠNG JSON mảng 2 chiều (Array of Arrays) chứa các chuỗi văn bản của từng ô. Chỉ trả về chuỗi JSON thô, KHÔNG BỌC TRONG ```json, không thêm bất kỳ dòng giải thích nào. Ví dụ: [[\"STT\", \"Môn\"], [\"1\", \"Toán\"]]"; // Ép Gemini xuất JSON thuần túy format Array 2D
                
                string jsonResponse = await _geminiService.GuiTinNhan(prompt, filePath); // Gọi service lấy chuỗi JSON mảng
                
                jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim(); // Khử cặn bã markdown nếu AI ngu không vâng lời (Xóa nháy)
                
                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator)) // Tắt thông báo Loading
                {
                    pnlMain.Controls.Remove(lblIndicator); // Xóa
                    lblIndicator.Dispose(); // Hủy ram
                }

                var tableData = JsonConvert.DeserializeObject<List<List<string>>>(jsonResponse); // Deserialize JSON 2 chiều thành List bên trong List C#
                
                if (tableData == null || tableData.Count == 0) // Nếu decode hụt hoặc AI trả rỗng
                {
                    ThemTinNhan("⚠ Không tìm thấy bảng dữ liệu nào trong ảnh hoặc AI không thể nhận diện định dạng bảng.", false); // Báo lỗi
                    return; // Ngắt sớm
                }
                
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Đọc đường dẫn ra màn hình Desktop của OS Windows
                string outPath = Path.Combine(desktopPath, "BangDuLieu_AI_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx"); // Nối tạo file xlsx đính kèm mốc thời gian
                
                FileInfo newFile = new FileInfo(outPath); // Sinh thể hiện cấu trúc file vật lý
                using (ExcelPackage package = new ExcelPackage(newFile)) // Cấp thư viện EPPlus vào file đó
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Dữ liệu xuất"); // Bật 1 trang sheet trắng mới đặt tên Dữ liệu xuất
                    
                    for (int row = 0; row < tableData.Count; row++) // Duyệt qua list cha (hàng ngang)
                    {
                        var rowData = tableData[row]; // Trích danh sách cột của dòng đó
                        for (int col = 0; col < rowData.Count; col++) // Duyệt qua từng phần tử (cột dọc)
                        {
                            worksheet.Cells[row + 1, col + 1].Value = rowData[col]; // Lưu từng text của JSON vào tọa độ excel (Excel index từ 1)
                        }
                    }
                    
                    var range = worksheet.Cells[1, 1, tableData.Count, tableData[0].Count]; // Xác định khung hình chữ nhật bọc trọn vùng dữ liệu vừa ghi
                    var excelTable = worksheet.Tables.Add(range, "Table1"); // Chuyển range đó thành đối tượng Format As Table của Excel
                    excelTable.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9; // Phủ lên đó mẫu màu TableStyles đẹp mắt (Xanh lá)
                    
                    worksheet.Cells.AutoFitColumns(); // Ép hệ thống tự giãn nở độ rộng cột theo chữ cho đẹp
                    
                    package.Save(); // Thực thi thao tác write byte xuống đĩa
                }
                
                ThemTinNhan($"✅ Trích xuất và tạo file Excel thành công!\nFile được lưu tại Desktop: {outPath}", false); // In thông báo chốt sổ trên Chatbox
                
                var dialogResult = MessageBox.Show("Xuất file Excel thành công!\nBạn có muốn mở thư mục chứa file không?", "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Information); // Hiện thông báo nổi, hỏi có muốn đi tắt tới chỗ chứa không
                if (dialogResult == DialogResult.Yes) // Nếu ấn Yes
                {
                    Process.Start("explorer.exe", $"/select,\"{outPath}\""); // Mở Window Explorer và tự highlight bôi đen trúng vào file excel vừa tạo
                }
            }
            catch (Exception ex) // Hứng lỗi nếu API Google sập hoặc file excel đang bị mở bới app khác nên ko đè đc
            {
                if (lblIndicator != null && pnlMain.Controls.Contains(lblIndicator)) // Dọn cục loading
                {
                    pnlMain.Controls.Remove(lblIndicator); // Xóa control
                    lblIndicator.Dispose(); // Hủy object
                }
                ThemTinNhan($"⚠ Lỗi khi phân tích AI hoặc xuất Excel: {ex.Message}", false); // In lỗi đỏ lên Chat
            }
            finally // Cuối cùng giải phóng cờ chờ
            {
                _dangChoAI = false; // Bỏ trạng thái bận
                btnSend.Enabled = true; // Mở khóa GỬI
                if (btnOcr != null) btnOcr.Enabled = true; // Mở khóa File
            }
        }
    }
}
