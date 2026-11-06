# FluentChat AI Studio 🤖
*(AI Desktop Assistant - C# WinForms)*

Một ứng dụng Desktop Chatbot AI hỗ trợ Tiếng Việt được viết bằng **C# WinForms (.NET Framework 4.7.2)**, kết hợp sức mạnh của **Local AI (Ollama)** và **Cloud AI (Google Gemini)**.

---

## ⚡ LỰA CHỌN 1: Dành Cho Người Dùng ("Tải Về Là Dùng Ngay")

Nếu bạn chỉ muốn sử dụng phần mềm mà không cần cài Visual Studio hay biên dịch code:

1. **Tải bản Portable:**
   - Tải file nén **`AI_Chatbot_Portable_v1.0.zip`** (tại mục [Releases](../../releases) của repository hoặc thư mục `dist/`).
2. **Giải nén:**
   - Giải nén file `.zip` vào bất kỳ thư mục nào trên máy tính.
3. **Chạy ứng dụng:**
   - Nhấp đúp vào file **`AI_Chatbot.exe`** để mở ứng dụng ngay lập tức!

> [!TIP]
> - **Dùng Cloud AI (Google Gemini):** Nhấn vào nút **[🔑 Cấu hình API Key]** ở góc dưới bên trái màn hình, dán API Key miễn phí từ [Google AI Studio](https://aistudio.google.com/apikey) và bấm Lưu.
> - **Dùng Local AI (Ollama):** Cài đặt [Ollama](https://ollama.com/download) và chạy `ollama run llama3.2`. Ứng dụng sẽ tự động kết nối ngầm.
> - **OCR Nhận diện chữ:** Thư mục `tessdata` (tiếng Việt + tiếng Anh) đã được tích hợp sẵn, bạn chỉ việc bấm nút `➕ File` và chọn ảnh để quét chữ offline.

---

## 🛠️ LỰA CHỌN 2: Dành Cho Lập Trình Viên (Mã Nguồn & Phát Triển)

Nếu bạn muốn chỉnh sửa, phát triển thêm tính năng hoặc tự build:

### 1. Yêu cầu môi trường
- Hệ điều hành: Windows 10/11
- Công cụ: **Visual Studio 2019 hoặc 2022** (.NET Desktop Development)
- .NET Framework 4.7.2 SDK

### 2. Mở và Biên dịch dự án
1. Clone hoặc tải mã nguồn về máy:
   ```bash
   git clone <URL_REPO>
   ```
2. Mở file **`AI_Chatbot.sln`** (hoặc `AI_Chatbot.slnx` nếu dùng VS 2022 v17.10+) bằng Visual Studio.
3. Chuột phải vào Solution -> chọn **Restore NuGet Packages** (hoặc dùng lệnh `nuget restore AI_Chatbot.sln`).
4. Nhấn **F5** (hoặc Ctrl + F5) để chạy.

### 3. Đóng gói bản Portable tự động
Dự án đã tích hợp sẵn script tự động hóa toàn bộ việc Restore -> Build Release -> Tải dữ liệu OCR -> Nén file ZIP:
```powershell
powershell -ExecutionPolicy Bypass -File scripts\build_portable.ps1
```
File ZIP đầu ra sẽ nằm tại: `dist\AI_Chatbot_Portable_v1.0.zip`.

---

## 🌟 Các Tính Năng Nổi Bật

- 💬 **Đa mô hình AI linh hoạt:** Chuyển đổi mượt mà giữa AI Offline (Ollama - Llama 3.2, Mistral, Qwen...) và AI Online (Google Gemini 2.5 Flash).
- 📎 **Đọc hiểu tài liệu đa định dạng:** Word (`.docx`), PDF (`.pdf`), Excel (`.xlsx`), Text (`.txt`), CSV, JSON, Markdown.
- 🖼️ **OCR Offline (Tesseract 5.2):** Trích xuất chữ tiếng Việt và tiếng Anh từ hình ảnh không cần kết nối mạng.
- 📊 **Vision AI & Xuất Excel:** Tự động phát hiện cấu trúc bảng biểu trong ảnh và xuất ra file Excel định dạng chuẩn qua EPPlus.
- 🎨 **Giao diện hiện đại (Fluent Dark UI):** Thiết kế tối ưu bằng thư viện Guna.UI2, hỗ trợ hiệu ứng hiển thị Markdown, code block đẹp mắt.
