# Mô Tả Chi Tiết Dự Án: FluentChat AI Studio - Desktop Assistant

## 1. Tổng Quan Dự Án
**FluentChat AI Studio** là một ứng dụng trợ lý ảo (AI Chatbot) dành cho Desktop được xây dựng trên nền tảng **C# Windows Forms (WinForms)**. Dự án mang đến một giao diện trò chuyện hiện đại (Fluent Design) và tích hợp các công nghệ Trí tuệ Nhân tạo tiên tiến, cho phép người dùng giao tiếp với cả các mô hình AI chạy cục bộ (Local LLM) và mô hình AI đám mây (Cloud LLM) để xử lý văn bản, hình ảnh, và tài liệu.

## 2. Các Tính Năng Cốt Lõi

### a. Tích Hợp Đa Mô Hình AI (Multi-Model AI)
- **Ollama (Local AI):** Hỗ trợ kết nối và chạy ngầm máy chủ [Ollama](https://ollama.com/) trực tiếp trên máy tính. Ứng dụng có thể tự động bật server Ollama, tải danh sách các mô hình đang có sẵn (ví dụ: `llama3.2`), và cung cấp giao diện để người dùng tải xuống (`pull`) các mô hình mới trực tiếp từ giao diện UI.
- **Google Gemini (Cloud AI):** Tích hợp Google Gemini (cụ thể là `Gemini 2.5 Flash`) thông qua API Key. Cho phép người dùng linh hoạt chuyển đổi qua lại giữa AI nội bộ (Ollama) bảo mật và sức mạnh của AI đám mây.

### b. Đọc & Trích Xuất Dữ Liệu Tài Liệu Tự Động (Document Parsing)
Ứng dụng có khả năng "đọc" và hiểu nội dung từ rất nhiều định dạng tệp tin văn phòng và văn bản mã nguồn nhờ vào `DocumentReaderService`:
- **Đọc PDF:** Sử dụng thư viện bên thứ ba `UglyToad.PdfPig` để trích xuất text thuần từ tài liệu PDF.
- **Đọc Word (`.doc`, `.docx`):** Có khả năng giải nén nội dung cấu trúc XML (`word/document.xml`) trực tiếp từ file docx để trích xuất chữ một cách mượt mà mà không cần cài đặt Microsoft Office.
- **Văn bản thuần & Code:** Hỗ trợ đọc trực tiếp `.txt`, `.md`, `.csv`, `.json`, `.xml`, `.sql`, `.cs`, `.py`, `.js`, `.html`,...
- Dữ liệu sau khi trích xuất sẽ được tự động đính kèm vào "ngữ cảnh" của tin nhắn để AI phân tích và tóm tắt theo yêu cầu.

### c. Nhận Diện Ký Tự Quang Học & Thị Giác Máy Tính (OCR & Vision)
- **Tesseract OCR (Offline):** Tích hợp engine mã nguồn mở Tesseract OCR (thông qua `OcrService`) và bộ ngôn ngữ `tessdata` (hỗ trợ Tiếng Việt & Anh). Tính năng này cho phép trích xuất chữ trực tiếp từ hình ảnh tải lên ngay trên máy tính mà không cần kết nối internet.
- **AI Vision:** Nếu sử dụng các mô hình AI có khả năng thị giác (Vision), hệ thống sẽ mã hóa ảnh sang `Base64` và gửi kèm truy vấn để AI phân tích mô tả hoặc trích xuất bảng biểu ra Excel.

### d. Giao Diện Người Dùng Hiện Đại (Fluent UI)
- Xóa bỏ giao diện cứng nhắc truyền thống của WinForms bằng các thư viện hiện đại (`Guna.UI2.WinForms`).
- Hỗ trợ **hiển thị Markdown (Markdown Rendering)** ngay trong khung chat: Các thẻ in đậm, code block, hay headers đều được hệ thống tự động phân tích (`ParseInlineMarkdown`) và tô màu/định dạng trong `RichTextBox`.
- Bong bóng chat (Chat Bubbles) có kích thước co giãn tự động (Dynamic Auto-size), bo góc mượt mà, màu sắc phân biệt giữa tin nhắn của Người dùng và Trợ lý AI. Có sẵn tính năng "Copy" văn bản với 1 click.

## 3. Cấu Trúc Mã Nguồn Chính

- **`Form1.cs`:** Giao diện chính của ứng dụng. Nơi xử lý các sự kiện click, gõ phím, logic giao diện (thêm bong bóng chat), parse markdown hiển thị, cũng như điều phối luồng gọi đến các AI Service.
- **`OllamaService.cs`:** Lớp chịu trách nhiệm giao tiếp với API cục bộ của máy chủ Ollama (port mặc định `11434`). Xử lý gọi chat, ping kiểm tra server, lấy danh sách mô hình và luồng tải mô hình mới.
- **`GeminiService.cs`:** Lớp xử lý gọi REST API đến endpoint của hệ thống Google Generative AI (Gemini). Xử lý việc đính kèm ảnh (Base64 Mime/Type) và chat văn bản đa phương thức.
- **`DocumentReaderService.cs`:** Lớp Service đảm nhiệm việc đọc và bóc tách chữ từ các định dạng tài liệu, PDF (bằng `PdfPig`), DOCX (bằng `System.IO.Compression` giải nén file xml nội bộ), và mã nguồn.
- **`OcrService.cs`:** Lớp bao bọc thư viện `TesseractEngine` để load ảnh cục bộ và quét chữ (sử dụng model data trong thư mục `/tessdata`).
- **`FormDownloadModelDialog.cs` & `FormApiKeyDialog.cs`:** Các cửa sổ Pop-up phụ giúp người dùng nhập API Key hoặc quản lý việc tải AI Models.

## 4. Công Nghệ & Thư Viện (Dependencies)
- **Framework:** .NET Framework (C# Windows Forms).
- **Giao diện:** `Guna.UI2.WinForms` (Tạo UI/UX hiện đại).
- **Thao tác API/JSON:** `Newtonsoft.Json`, `System.Net.Http`.
- **Đọc PDF:** `UglyToad.PdfPig`.
- **Nhận diện OCR:** `Tesseract`.
- **Tiện ích Office:** `EPPlus` (Thao tác xuất file Excel).

---
*Tài liệu này được phân tích và tổng hợp tự động từ các tệp mã nguồn bên trong thư mục dự án.*
