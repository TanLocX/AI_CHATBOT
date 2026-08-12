# AI Chat Assistant 🤖

Một ứng dụng Desktop Chatbot AI hỗ trợ Tiếng Việt được viết bằng C# (WinForms), kết hợp sức mạnh của **Local AI (Ollama)** và **Cloud AI (Google Gemini)**. 

Ngoài ra, ứng dụng còn tích hợp:
- 📎 **Đọc hiểu tài liệu**: Word, PDF, Text, CSV, JSON, Markdown...
- 🖼️ **OCR & Vision AI**: Trích xuất chữ từ hình ảnh (offline qua Tesseract) hoặc phân tích/xuất dữ liệu bảng biểu ra file Excel tự động (qua Gemini).

---

## 🚀 Hướng dẫn cài đặt cho người mới tải về

Khi bạn clone (tải) repository này về máy, hãy làm theo các bước cực kỳ đơn giản sau để thiết lập:

### Bước 1: Cấu hình API Key (Để dùng Google Gemini)
Do lý do bảo mật, API Key không được đẩy lên GitHub. Bạn cần tự điền key của riêng mình:

1. Trong thư mục `AI_Chatbot`, bạn sẽ thấy một file tên là **`secrets.config.example`**.
2. **Copy** file đó và đổi tên bản copy thành **`secrets.config`**.
3. Mở file `secrets.config` vừa tạo.
4. Đăng nhập [Google AI Studio](https://aistudio.google.com/apikey) để lấy API Key miễn phí.
5. Thay thế dòng `DIEN_API_KEY_CUA_BAN_VAO_DAY` bằng API Key của bạn:
   ```xml
   <add key="GeminiApiKey" value="AIzaSyB_Ví_Dụ_Mã_Key_Của_Bạn..." />
   ```

*(Lưu ý: File `secrets.config` đã được đưa vào `.gitignore` nên API Key của bạn được an toàn tuyệt đối).*

### Bước 2: Cài đặt Ollama (Để dùng AI Offline tại máy)
1. Tải và cài đặt [Ollama](https://ollama.com/download) cho Windows.
2. Mở Command Prompt (cmd) hoặc PowerShell và gõ lệnh tải mô hình (ví dụ Llama 3.2):
   ```bash
   ollama run llama3.2
   ```
*(Khi bạn mở ứng dụng C#, phần mềm sẽ tự động bật Ollama ngầm ở background).*

### Bước 3: Build và Chạy
1. Mở file `SEMI_FINAL.sln` bằng **Visual Studio** (khuyên dùng 2019 trở lên).
2. Khi mở lần đầu, Visual Studio có thể yêu cầu **Restore NuGet Packages** (tải các thư viện như Guna UI2, PdfPig, EPPlus...). Hãy nhấn đồng ý (hoặc click chuột phải vào Solution -> chọn *Restore NuGet Packages*).
3. Nhấn nút **Start (F5)** để chạy.

---

## 🛠️ Công nghệ sử dụng
- **.NET Framework 4.7.2** & C# WinForms
- **Guna UI2**: Thiết kế giao diện hiện đại (Dark Mode)
- **Tesseract OCR**: Quét chữ offline
- **EPPlus**: Thao tác và xuất file Excel
- **Newtonsoft.Json**: Phân tích dữ liệu JSON
