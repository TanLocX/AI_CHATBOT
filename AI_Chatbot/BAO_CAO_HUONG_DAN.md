# BÁO CÁO VÀ HƯỚNG DẪN TRIỂN KHAI DỰ ÁN: AI CHAT ASSISTANT
*(Tích hợp Phân tích Đa tài liệu & Nhận diện quang học OCR)*

---

## MỤC LỤC
1. [Lời mở đầu & Giới thiệu chung](#1-lời-mở-đầu--giới-thiệu-chung)
2. [Tổng quan về Công nghệ và Kiến trúc](#2-tổng-quan-về-công-nghệ-và-kiến-trúc)
3. [Chi tiết Tính năng & Hướng dẫn Sử dụng](#3-chi-tiết-tính-năng--hướng-dẫn-sử-dụng)
4. [Tài liệu Dành cho Nhà phát triển (Developer Guide)](#4-tài-liệu-dành-cho-nhà-phát-triển-developer-guide)
5. [Đề xuất Khả năng Tích hợp hệ thống](#5-đề-xuất-khả-năng-tích-hợp-hệ-thống)
6. [Kết luận](#6-kết-luận)

---

## 1. LỜI MỞ ĐẦU & GIỚI THIỆU CHUNG

### 1.1. Đặt vấn đề
Trong bối cảnh chuyển đổi số, các phần mềm quản lý (ERP, CRM, phần mềm y tế, trường học) đang đóng vai trò thiết yếu. Tuy nhiên, một hạn chế lớn là các tác vụ nhập liệu thủ công (từ giấy tờ, hóa đơn, CMND/CCCD) vẫn tốn rất nhiều thời gian và dễ xảy ra sai sót. Ngoài ra, việc đọc và tóm tắt những báo cáo dài hàng chục trang cũng là một trở ngại đối với nhân sự. 

### 1.2. Giải pháp và Mục tiêu
Dự án **AI Chat Assistant** ra đời nhằm giải quyết vấn đề trên thông qua một ứng dụng Desktop xây dựng bằng C#. Đây không chỉ là một chatbot giao tiếp thông thường, mà còn là một **"trợ lý ảo đắc lực"** được trang bị các module nâng cao:
* **Đọc hiểu đa tài liệu**: Hỗ trợ đọc tự động các tệp Word (.docx), PDF, Text (.txt, .md, .csv).
* **Thị giác máy tính (Vision & OCR)**: Tích hợp công nghệ nhận dạng ký tự quang học để bóc tách thông tin từ hình ảnh.
* **Quyền riêng tư & Tiết kiệm chi phí**: Hệ thống sử dụng mô hình AI nguồn mở (Ollama) chạy trực tiếp trên máy tính local, đảm bảo dữ liệu (như hồ sơ bệnh án, hóa đơn bảo mật) 100% không bị rò rỉ lên Internet, đồng thời không tốn phí API hàng tháng.

---

## 2. TỔNG QUAN VỀ CÔNG NGHỆ VÀ KIẾN TRÚC

### 2.1. Công nghệ cốt lõi
* **Ngôn ngữ lập trình**: C# (Nền tảng .NET Framework / Windows Forms).
* **Giao diện (UI)**: Custom Controls và Guna UI2 mang lại trải nghiệm hiện đại (Dark Mode, Chat Bubbles giống Zalo, Messenger).
* **Trí tuệ nhân tạo (LLM)**: 
  * **Ollama (Llama 3.2, Qwen...)**: Xử lý ngôn ngữ tự nhiên Offline, bảo mật dữ liệu.
  * **Google Gemini API**: Tùy chọn xử lý trực tuyến tốc độ cao và thao tác phức tạp (chuyển ảnh sang Excel).
* **Xử lý tài liệu & Hình ảnh**:
  * `Tesseract OCR`: Nhận dạng ký tự từ ảnh offline.
  * `UglyToad.PdfPig`: Đọc và trích xuất text từ PDF.
  * `EPPlus`: Tự động tạo và format bảng tính Excel.

### 2.2. Sơ đồ luồng hoạt động
1. Người dùng nhập văn bản hoặc đính kèm tài liệu (Ảnh, PDF, Word).
2. Hệ thống định tuyến:
   - Nếu là ảnh -> Quét OCR hoặc Vision AI.
   - Nếu là File -> Document Reader trích xuất ra chữ (Text).
3. Ghép Text trích xuất được với "Câu lệnh (Prompt)" của người dùng.
4. Gửi cấu trúc này xuống cho Local AI (Ollama) hoặc Cloud AI (Gemini).
5. AI trả về câu trả lời. Giao diện (Form1) parse Markdown và hiển thị lên màn hình.

---

## 3. CHI TIẾT TÍNH NĂNG & HƯỚNG DẪN SỬ DỤNG

### 3.1. Giao tiếp cơ bản (Chatbot)
- Ứng dụng cung cấp khung chat trực quan. Người dùng có thể đặt câu hỏi về mọi lĩnh vực, từ tra cứu thông tin, dịch thuật, cho đến viết mã nguồn (code).
- Các tin nhắn được định dạng bằng Markdown (in đậm, code block) rõ ràng. Có sẵn nút "Sao chép" ở mọi câu trả lời của AI.

### 3.2. Đính kèm và Phân tích tài liệu (Word/PDF/JSON)
- Click vào nút `[+ File]`, chọn tài liệu. 
- Chương trình sẽ tự động đọc hiểu. Người dùng có thể yêu cầu: *"Hãy tóm tắt 3 ý chính của tài liệu này"* hoặc *"Hãy tìm trong tài liệu xem có quy định nào về việc đi trễ không?"*.

### 3.3. Nhận dạng ảnh & Tự động xuất Excel
Khi tải lên một hình ảnh (ví dụ: ảnh chụp hóa đơn, bảng điểm), hệ thống cung cấp 2 lựa chọn:
1. **Quét OCR Local**: Bóc tách toàn bộ chữ trong ảnh ra màn hình (không cần Internet).
2. **Xuất Excel tự động (AI Vision)**: AI sẽ phân tích cấu trúc của hình ảnh, phát hiện các hàng/cột và tự động sinh ra một file Excel đẹp mắt (.xlsx) lưu thẳng ra ngoài màn hình Desktop.

---

## 4. TÀI LIÊU DÀNH CHO NHÀ PHÁT TRIỂN (DEVELOPER GUIDE)

*(Phần này hướng dẫn một sinh viên/lập trình viên khác cách để thiết lập, đọc code và biên dịch lại ứng dụng này từ đầu).*

### 4.1. Môi trường Yêu cầu
* **IDE**: Microsoft Visual Studio 2019 hoặc mới hơn.
* **SDK**: .NET Framework 4.7.2 trở lên.
* **Phần mềm AI**: Cài đặt phần mềm **Ollama** tại trang chủ (ollama.com). Sau khi cài, mở Terminal gõ lệnh: `ollama run llama3.2` để tải mô hình ngôn ngữ về máy.

### 4.2. Khôi phục thư viện (NuGet Packages)
Để dự án chạy được, cần đảm bảo đã cài đặt các Packages sau (Click chuột phải vào Solution -> Manage NuGet Packages):
- `Newtonsoft.Json`: Dùng để thao tác mảng JSON.
- `Tesseract` (phiên bản phù hợp): Dùng cho OCR. *Lưu ý: Phải có thư mục `tessdata` (chứa file ngôn ngữ .traineddata) đặt cùng cấp với file `.exe`.*
- `UglyToad.PdfPig`: Dùng để đọc PDF.
- `EPPlus`: Dùng để tạo file Excel (Lưu ý set cờ NonCommercial).

### 4.3. Cấu trúc Source Code
Mã nguồn được thiết kế theo tư duy Module hóa cực cao (Hướng đối tượng - OOP), chia thành 5 file chính để dễ bảo trì:
1. `Form1.cs`: Quản lý 100% giao diện (UI). Xử lý sự kiện click chuột, render bong bóng chat, hiệu ứng cuộn, và gọi các Service.
2. `DocumentReaderService.cs`: Chứa thuật toán đọc file (ZipArchive cho Word, PdfPig cho PDF).
3. `OcrService.cs`: Đóng gói logic khởi tạo Engine Tesseract và Load Pix hình ảnh.
4. `OllamaService.cs`: Xử lý HTTP Client để POST dữ liệu xuống máy chủ LLM Local. Có thuật toán tự động cắt ngắn chuỗi để không làm tràn RAM của mô hình.
5. `GeminiService.cs`: Quản lý mã API Key, gói data thành chuỗi JSON Base64 ảnh theo chuẩn Google API.

---

## 5. ĐỀ XUẤT KHẢ NĂNG TÍCH HỢP HỆ THỐNG

Ứng dụng không chỉ là một app chạy độc lập mà còn là các **Module (Micro-service)** có thể nhúng thẳng vào Bài tập lớn / Đồ án cuối kỳ (ví dụ như các phần mềm quản lý):

1. **Hệ thống Quản lý Sinh viên / Bệnh viện**: Khi nhập hồ sơ người mới, thay vì gõ tay, nhân viên chỉ việc tải ảnh CCCD/BHYT lên. `OcrService` và `OllamaService` sẽ tự động phân tích và điền tự động dữ liệu `(Tên, Ngày sinh, Mã số)` vào Form C# có sẵn.
2. **Phần mềm Kế toán / Kho**: Cho phép đọc ảnh hóa đơn, phân tích bằng AI để trích xuất thẳng `(Tổng tiền, Tên hàng, MST)` rồi Insert vào Database.
3. **Trợ lý nhân sự (E-Office)**: Gắn module AI Chatbox vào góc phải phần mềm. Nhân viên có thể chat hỏi đáp về nội quy công ty, hoặc nhờ tóm tắt một hợp đồng 20 trang chỉ trong vài giây.

---

## 6. KẾT LUẬN

Dự án đã hoàn thiện thành công một AI Assistant mạnh mẽ nhưng cực kỳ nhẹ và độc lập. Điểm sáng lớn nhất của dự án là việc giải quyết được bài toán **Bảo mật dữ liệu** nhờ triển khai Local AI thay vì phụ thuộc 100% vào Cloud (như ChatGPT), đồng thời giúp doanh nghiệp đạt chi phí duy trì bằng 0 (Zero-Cost Operation). 

Mã nguồn được viết tường minh, chia tách thành các lớp dịch vụ rõ ràng và được chú thích 100% tiếng Việt trên từng dòng lệnh, tạo điều kiện thuận lợi nhất để kế thừa và phát triển cho các dự án quy mô lớn hơn trong tương lai.
