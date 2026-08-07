# BÁO CÁO ĐỒ ÁN VÀ HƯỚNG DẪN TRIỂN KHAI
**TÊN ĐỀ TÀI: XÂY DỰNG ỨNG DỤNG AI CHAT ASSISTANT TÍCH HỢP PHÂN TÍCH ĐA TÀI LIỆU VÀ NHẬN DIỆN QUANG HỌC (OCR)**

---

## MỤC LỤC
1. [Chương 1: Tổng quan đề tài](#chuong-1-tong-quan-de-tai)
2. [Chương 2: Cơ sở lý thuyết và Công nghệ sử dụng](#chuong-2-co-so-ly-thuyet-va-cong-nghe-su-dung)
3. [Chương 3: Phân tích và Thiết kế hệ thống](#chuong-3-phan-tich-va-thiet-ke-he-thong)
4. [Chương 4: Chi tiết tính năng và Hướng dẫn sử dụng](#chuong-4-chi-tiet-tinh-nang-va-huong-dan-su-dung)
5. [Chương 5: Tài liệu dành cho Nhà phát triển (Developer Guide)](#chuong-5-tai-lieu-danh-cho-nha-phat-trien-developer-guide)
6. [Chương 6: Ứng dụng thực tiễn, Đề xuất tích hợp và Tổng kết](#chuong-6-ung-dung-thuc-tien-de-xuat-tich-hop-va-tong-ket)

---

<a name="chuong-1-tong-quan-de-tai"></a>
## CHƯƠNG 1: TỔNG QUAN ĐỀ TÀI

### 1.1 Đặt vấn đề
Trong bối cảnh chuyển đổi số mạnh mẽ, các hệ thống phần mềm quản lý (như ERP doanh nghiệp, CRM, hệ thống quản lý y tế, trường học) đang đóng vai trò cốt lõi. Tuy nhiên, các hệ thống truyền thống vẫn gặp phải giới hạn lớn:
- Thao tác nhập liệu từ giấy tờ, hóa đơn, CMND/CCCD vẫn phải thực hiện thủ công, tốn rất nhiều thời gian và có rủi ro sai sót do yếu tố con người (Human Error).
- Việc tổng hợp, đọc hiểu và tóm tắt những báo cáo dài hàng chục trang (PDF, Word) chưa được tự động hóa, gây quá tải cho nhân sự.
- Các ứng dụng AI phổ biến hiện nay (như ChatGPT) yêu cầu phải tải dữ liệu lên Cloud, gây ra nguy cơ lộ lọt dữ liệu bảo mật của doanh nghiệp và tiêu tốn chi phí thuê bao (API Cost).

### 1.2 Giải pháp và Mục tiêu
Dự án được xây dựng nhằm tạo ra một **Trợ lý ảo (AI Chat Assistant)** trên nền tảng Desktop giải quyết trực tiếp các bài toán trên với các mục tiêu cụ thể:
1. **Giao tiếp thông minh:** Xây dựng Chatbot giao tiếp bằng ngôn ngữ tự nhiên.
2. **Đọc hiểu đa tài liệu:** Tích hợp khả năng bóc tách, đọc tự động các tệp Word (.docx), PDF, Text (.txt, .md, .csv).
3. **Thị giác máy tính (Vision & OCR):** Ứng dụng công nghệ nhận dạng ký tự quang học để tự động trích xuất chữ từ hình ảnh.
4. **Quyền riêng tư & Tiết kiệm chi phí:** Ưu tiên tuyệt đối tính bảo mật dữ liệu bằng cách sử dụng Mô hình ngôn ngữ lớn (LLM) chạy cục bộ (Offline) với chi phí vận hành bằng 0.

---

<a name="chuong-2-co-so-ly-thuyet-va-cong-nghe-su-dung"></a>
## CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÔNG NGHỆ SỬ DỤNG

### 2.1 Nền tảng phát triển (C# & Windows Forms)
Dự án được phát triển bằng ngôn ngữ C# trên nền tảng .NET Framework. Giao diện (UI) được thiết kế hiện đại (Dark Mode, Bong bóng chat) thông qua thư viện Guna UI2 và các Custom Controls, mang lại trải nghiệm người dùng tương đồng với các ứng dụng nhắn tin hiện đại như Zalo, Messenger.

### 2.2 Xử lý ngôn ngữ tự nhiên (LLMs)
Hệ thống sử dụng linh hoạt giữa hai nền tảng AI:
- **Ollama (Local AI):** Đóng vai trò cốt lõi trong việc đảm bảo bảo mật. Ollama cho phép chạy các mô hình ngôn ngữ như Llama 3.2, Qwen trực tiếp trên máy tính cá nhân hoặc mạng LAN nội bộ.
- **Google Gemini API:** Được sử dụng như một tùy chọn nâng cao cho các tác vụ đòi hỏi sự phức tạp cao (ví dụ: chuyển cấu trúc ảnh sang Excel) với tốc độ phản hồi nhanh.

### 2.3 Công nghệ nhận dạng và Xử lý tài liệu
Sử dụng các thư viện mạnh mẽ để bóc tách dữ liệu:
- **Tesseract OCR:** Engine mã nguồn mở mạnh mẽ để bóc tách text từ hình ảnh (Offline) với độ chính xác cao.
- **UglyToad.PdfPig:** Thư viện đọc và trích xuất text từ định dạng PDF.
- **EPPlus:** Thư viện tự động tạo, định dạng và xuất báo cáo dưới dạng bảng tính Excel (.xlsx).

---

<a name="chuong-3-phan-tich-va-thiet-ke-he-thong"></a>
## CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG

### 3.1 Sơ đồ kiến trúc tổng thể
Hệ thống được thiết kế theo tư duy Module hóa (Micro-module), tách biệt hoàn toàn giữa giao diện và logic xử lý nghiệp vụ, bao gồm 4 module chính:
1. **UI Layer (`Form1.cs`)**: Quản lý giao diện, bắt sự kiện người dùng và render định dạng Markdown.
2. **AI Module (`OllamaService.cs`, `GeminiService.cs`)**: Xử lý HTTP Client, gọi API, cắt ngắn chuỗi hội thoại tránh tràn bộ nhớ.
3. **OCR Module (`OcrService.cs`)**: Xử lý load hình ảnh và chạy engine Tesseract nhận dạng chữ.
4. **Document Module (`DocumentReaderService.cs`)**: Trích xuất văn bản thô từ các định dạng file đính kèm.

### 3.2 Luồng hoạt động (Data Flow)
1. **Input:** Người dùng nhập câu lệnh (Prompt) hoặc đính kèm tài liệu/hình ảnh.
2. **Preprocessing (Định tuyến):** 
   - Nếu là hình ảnh $\rightarrow$ Gọi OCR Module hoặc Vision AI quét ra Text.
   - Nếu là tài liệu $\rightarrow$ Gọi Document Reader trích xuất ra Text.
3. **Processing:** Hệ thống ghép phần Text vừa trích xuất với câu lệnh của người dùng tạo thành một cấu trúc chuẩn và gửi xuống mô hình AI (Ollama/Gemini).
4. **Output:** Mô hình AI phân tích và trả về kết quả. UI Layer bắt phản hồi và hiển thị trực quan lên màn hình.

---

<a name="chuong-4-chi-tiet-tinh-nang-va-huong-dan-su-dung"></a>
## CHƯƠNG 4: CHI TIẾT TÍNH NĂNG VÀ HƯỚNG DẪN SỬ DỤNG

### 4.1 Giao tiếp cơ bản (Chatbot)
- Ứng dụng cung cấp khung chat trực quan. Người dùng có thể đặt câu hỏi về mọi lĩnh vực, từ tra cứu thông tin, dịch thuật, cho đến viết mã nguồn (code).
- Các tin nhắn được định dạng bằng Markdown (in đậm, code block, bảng biểu) rõ ràng. Có sẵn nút "Sao chép" ở mọi câu trả lời của AI và menu chuột phải tiện dụng.

### 4.2 Đính kèm và Phân tích tài liệu (Word/PDF/JSON)
- **Sử dụng:** Click vào nút `[+ File]`, chọn tài liệu. 
- Chương trình sẽ tự động đọc hiểu dung lượng lớn tài liệu. Người dùng có thể yêu cầu: *"Hãy tóm tắt 3 ý chính của tài liệu này"* hoặc *"Hãy tìm trong tài liệu nội quy này các quy định về việc đi trễ"*.

### 4.3 Nhận dạng ảnh & Tự động xuất Excel
Khi tải lên một hình ảnh (ví dụ: ảnh chụp hóa đơn, bảng điểm), hệ thống cung cấp 2 lựa chọn xử lý:
1. **Quét OCR Local:** Bóc tách toàn bộ chữ trong ảnh ra màn hình (không cần Internet).
2. **Xuất Excel tự động (AI Vision):** AI sẽ phân tích cấu trúc của hình ảnh, phát hiện các hàng/cột và tự động sinh ra một file Excel đẹp mắt (.xlsx) lưu thẳng ra ngoài màn hình Desktop.

---

<a name="chuong-5-tai-lieu-danh-cho-nha-phat-trien-developer-guide"></a>
## CHƯƠNG 5: TÀI LIỆU DÀNH CHO NHÀ PHÁT TRIỂN (DEVELOPER GUIDE)

*(Phần này hướng dẫn cách thiết lập, đọc code và biên dịch lại ứng dụng).*

### 5.1 Môi trường Yêu cầu
* **IDE**: Microsoft Visual Studio 2019 hoặc mới hơn.
* **SDK**: .NET Framework 4.7.2 trở lên.
* **Phần mềm AI**: Cài đặt phần mềm **Ollama** (ollama.com). Mở Terminal và chạy lệnh `ollama run llama3.2` để tải mô hình ngôn ngữ về máy.

### 5.2 Khôi phục thư viện (NuGet Packages)
Đảm bảo cài đặt các thư viện sau qua NuGet Package Manager:
- `Newtonsoft.Json`: Dùng để thao tác JSON.
- `Tesseract` (phiên bản phù hợp): Dùng cho OCR. *Lưu ý: Phải có thư mục `tessdata` (chứa data ngôn ngữ) đặt cùng cấp với file `.exe`.*
- `UglyToad.PdfPig`: Đọc PDF.
- `EPPlus`: Tạo file Excel (Lưu ý set cờ NonCommercial).

### 5.3 Cấu trúc Source Code
Mã nguồn tuân thủ nguyên lý OOP, chia tách rõ ràng:
1. `Form1.cs`: Quản lý 100% giao diện (UI), hiệu ứng cuộn, và gọi các Service.
2. `DocumentReaderService.cs`: Chứa thuật toán giải nén ZipArchive cho Word, PdfPig cho PDF.
3. `OcrService.cs`: Khởi tạo Engine Tesseract và Load hình ảnh xử lý offline.
4. `OllamaService.cs`: Quản lý HTTP Client POST dữ liệu xuống máy chủ LLM Local. Có thuật toán tự động cắt chuỗi tránh tràn RAM.
5. `GeminiService.cs`: Quản lý API Key, gói data JSON Base64 gửi Google API.

Mọi dòng code trong các file trên đều được chú thích (comment) chi tiết bằng tiếng Việt để dễ dàng bảo trì và tái sử dụng.

---

<a name="chuong-6-ung-dung-thuc-tien-de-xuat-tich-hop-va-tong-ket"></a>
## CHƯƠNG 6: ỨNG DỤNG THỰC TIỄN, ĐỀ XUẤT TÍCH HỢP VÀ TỔNG KẾT

### 6.1 Đề xuất khả năng tích hợp
Do mã nguồn thiết kế dạng Module, ứng dụng có thể được nhúng làm Micro-service cho các hệ thống phần mềm lớn:
- **Phần mềm quản lý sinh viên / y tế:** Tải ảnh thẻ/CCCD lên, Module OCR và AI tự động phân tích và điền form (Họ tên, Ngày sinh) $\rightarrow$ Tiết kiệm 80% thời gian nhập liệu.
- **Phần mềm kế toán:** Trích xuất tự động "Tên hàng hóa", "Mã số thuế", "Tổng tiền" từ ảnh hóa đơn để Insert vào Database.
- **Hệ thống E-Office:** Gắn chatbot ở góc màn hình giúp nhân sự tóm tắt hợp đồng dài hoặc tra cứu luật nội bộ một cách bảo mật.

### 6.2 Kết luận
Dự án đã phát triển thành công ứng dụng AI Chat Assistant mạnh mẽ, kết hợp xuất sắc giữa mô hình ngôn ngữ lớn (LLM) và nhận diện quang học (OCR). 

Dự án không chỉ tối ưu hóa quy trình nhập liệu thủ công mà còn giải quyết triệt để bài toán **Bảo mật dữ liệu** (nhờ AI Offline) và **Chi phí duy trì bằng 0** (Zero-cost). Hướng đi này chứng minh tính khả thi cao để ứng dụng AI vào thực tiễn doanh nghiệp và hoàn toàn có thể mở rộng thành một hệ sinh thái trợ lý ảo toàn diện trong tương lai.
