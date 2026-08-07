# DÀN Ý THUYẾT TRÌNH BÁO CÁO: AI CHAT ASSISTANT
*Tích hợp Phân tích Đa tài liệu & OCR*

---

## 1. Giới thiệu bài toán & Mục tiêu (1-2 Slide)

**Slide 1: Tiêu đề & Đặt vấn đề**
* **Nội dung:** 
  * **Vấn đề:** Các phần mềm quản lý truyền thống thường khô khan, thao tác nhập liệu thủ công tốn thời gian, khó tổng hợp thông tin từ nhiều nguồn tài liệu đa dạng (PDF, Word, Excel, Hình ảnh).
  * **Nhu cầu:** Cần một trợ lý ảo thông minh (AI Assistant) có khả năng đọc hiểu đa định dạng và tương tác bằng ngôn ngữ tự nhiên ngay trong các phần mềm nội bộ, đảm bảo tính bảo mật.
* **Hình ảnh gợi ý:** Hình ảnh minh họa một nhân viên đang chật vật nhập liệu từ một đống giấy tờ/hóa đơn, bên cạnh là icon của một con AI đang giải quyết vấn đề.

**Slide 2: Mục tiêu dự án**
* **Nội dung:**
  * Xây dựng ứng dụng Desktop C# WinForms làm **AI Chatbot**.
  * Tích hợp khả năng đọc hiểu tài liệu: `Word, Excel, PDF, Markdown`.
  * Tích hợp **OCR (Nhận dạng ký tự quang học)** để bóc tách dữ liệu từ hình ảnh (CCCD, Hóa đơn).
  * Sử dụng mô hình AI chạy local (Ollama) đảm bảo 100% bảo mật dữ liệu, không tốn phí API, kết hợp tùy chọn Gemini AI.
* **Hình ảnh gợi ý:** Sơ đồ tư duy (Mindmap) ngắn gọn chỉ ra 3 mục tiêu chính: Chatbot AI + Xử lý đa tài liệu + Local AI & Bảo mật.

---

## 2. Công nghệ sử dụng & Nguyên lý hoạt động (2 Slide)

**Slide 3: Công nghệ sử dụng (Tech Stack)**
* **Nội dung:**
  * **Ngôn ngữ & Nền tảng:** C# & .NET Framework (Windows Forms).
  * **Giao diện (UI/UX):** Guna UI2 / Custom WinForms Controls.
  * **Xử lý AI (LLM):** 
    * **Ollama:** Chạy các mô hình ngôn ngữ lớn (Llama 3.2, Qwen...) ngay trên máy local hoặc mạng LAN.
    * **Gemini API:** Tùy chọn sử dụng cloud AI cho tốc độ cao.
  * **Xử lý dữ liệu & OCR:**
    * `Tesseract OCR`: Nhận dạng chữ từ ảnh.
    * `EPPlus` (Excel), `iTextSharp`/`Pdfium` (PDF)...
* **Hình ảnh gợi ý:** Các logo công nghệ (C#, .NET, Ollama, Meta Llama, Tesseract OCR, Google Gemini).

**Slide 4: Nguyên lý hoạt động (Kiến trúc hệ thống)**
* **Nội dung:** 
  1. Người dùng đính kèm file (Ảnh/PDF/Word) hoặc nhập text.
  2. `OcrService` / `DocumentReaderService` trích xuất thành Text thô.
  3. Ứng dụng gửi prompt + Text thô qua REST API tới mô hình AI (Ollama/Gemini).
  4. AI phân tích, trả về kết quả (Stream/JSON) và hiển thị lên giao diện (Render Markdown).
* **Hình ảnh gợi ý:** Sử dụng sơ đồ Mermaid trong file `DE_XUAT_TICH_HOP.md` (Mục 4). Sơ đồ này thể hiện rất rõ kiến trúc module hóa của ứng dụng.

---

## 3. Thiết kế giao diện & Trải nghiệm người dùng (2 Slide)

**Slide 5: Triết lý thiết kế (UI/UX)**
* **Nội dung:**
  * Giao diện theo phong cách **Modern, Dark Mode** thân thiện mắt.
  * Thiết kế dạng "Bong bóng chat" (Chat bubbles) giống các app hiện đại (Zalo, Messenger, ChatGPT).
  * Hỗ trợ hiển thị định dạng phong phú: **Markdown**, Code block, bôi đen, copy.
* **Hình ảnh gợi ý:** Chụp một góc giao diện lúc đang chat, khoanh đỏ các tính năng UI (khung chat, bo góc, nút đổi model).

**Slide 6: Trải nghiệm người dùng (Tiện ích tích hợp)**
* **Nội dung:**
  * Tự động phát hiện và khởi chạy ngầm máy chủ Ollama nếu chưa bật.
  * Phím tắt thao tác nhanh: Nhấn `Enter` để gửi, menu chuột phải (Context Menu) sao chép văn bản, nút Copy tiện lợi ở mỗi tin nhắn AI.
  * Khóa chặn spam (Validation): Không cho gửi khi AI đang trả lời hoặc ô chat trống.
* **Hình ảnh gợi ý:** Ảnh chụp chức năng click chuột phải chọn "Sao chép" hoặc trạng thái UI (Đang tải model...).

---

## 4. Cấu trúc chương trình & Module chính (1-2 Slide)

**Slide 7: Cấu trúc mã nguồn (Code Structure)**
* **Nội dung:** Chương trình được thiết kế hướng đối tượng (OOP), chia các dịch vụ (Service) độc lập với UI để dễ bảo trì:
  * `OllamaService.cs`: Quản lý kết nối, tự động fetch model, gửi request tới Local LLM.
  * `GeminiService.cs`: Xử lý giao tiếp với API của Google.
  * `OcrService.cs`: Gọi Tesseract nhận dạng chữ từ ảnh (CCCD, Hóa đơn).
  * `DocumentReaderService.cs`: Trích xuất text từ các file văn phòng.
* **Hình ảnh gợi ý:** Ảnh chụp thư mục Solution Explorer trong Visual Studio hiển thị các file `.cs` này một cách gọn gàng.

---

## 5. Đề xuất khả năng tích hợp (2 Slide)

**Slide 8: Khả năng tích hợp (Micro-module)**
* **Nội dung:** Ứng dụng không chỉ chạy độc lập mà còn là các module (DLL/Class) dễ dàng nhúng vào các ERP/CRM:
  * **Quản lý sinh viên / Y tế:** Tự động điền form (OCR + AI bóc tách JSON) từ ảnh CCCD/Bệnh án thẻ giấy.
  * **Phần mềm kế toán:** Tự động đọc tổng tiền, MST từ ảnh chụp hóa đơn.
  * **Văn phòng điện tử:** Tóm tắt hợp đồng/văn bản hàng chục trang trong vài giây.
* **Hình ảnh gợi ý:** Vẽ một biểu đồ luồng: `Ảnh CCCD -> [OCR Module] -> [AI Module] -> JSON -> Form nhập liệu sinh viên tự động`.

**Slide 9: Giá trị mang lại**
* **Nội dung:**
  * **Bảo mật:** Local LLM & OCR offline -> 100% dữ liệu (hóa đơn, CCCD) không bị đưa lên mạng.
  * **Chi phí:** Zero API Cost (Không tốn tiền thuê bao LLM).
  * **Hiệu suất:** Tiết kiệm 80% thời gian nhập liệu thủ công, giảm lỗi con người.
* **Hình ảnh gợi ý:** Icon cái khiên (Bảo mật), Icon đồng tiền gạch chéo (Zero Cost), Icon đồng hồ cát (Tiết kiệm thời gian).

---

## 6. Demo giao diện (1 Slide + Live Demo)

**Slide 10: Demo Chức năng**
* **Nội dung:** (Để 1 slide giữ chỗ, trên slide ghi chú các luồng sẽ thao tác trực tiếp)
  * Demo 1: Chat hỏi đáp bình thường với Ollama/Gemini.
  * Demo 2: Đính kèm một file Word/PDF và yêu cầu AI tóm tắt.
  * Demo 3: Đính kèm hình ảnh để demo tính năng OCR bóc tách thông tin (VD: file `Nguyen_Van_Muoi_images.jpg`).
* **Hình ảnh gợi ý:** 1 ảnh chụp toàn màn hình ứng dụng lúc đang phân tích thành công tấm ảnh CCCD hoặc hóa đơn. Dưới slide ghi chữ "Chuyển sang màn hình Live Demo".

---

## 7. Kết luận & Q&A (1-2 Slide)

**Slide 11: Kết luận**
* **Nội dung:**
  * Đã hoàn thiện ứng dụng AI Assistant với giao diện hiện đại, dễ dùng.
  * Ứng dụng thành công các công nghệ mã nguồn mở (Ollama, Tesseract) vào bài toán thực tế của doanh nghiệp.
  * Mã nguồn mở và module hóa, có sẵn "Developer Guide" để các nhóm khác tái sử dụng cho bài tập lớn cuối kỳ.
* **Hình ảnh gợi ý:** Hình ảnh cả nhóm (nếu có) hoặc một icon checkmark hoàn thành nhiệm vụ lớn.

**Slide 12: Q&A**
* **Nội dung:** Lời cảm ơn Thầy/Cô và các bạn đã lắng nghe. Mời đặt câu hỏi.
