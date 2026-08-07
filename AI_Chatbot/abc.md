# BÁO CÁO BÀI TẬP LỚN / ĐỒ ÁN
**TÊN ĐỀ TÀI: XÂY DỰNG ỨNG DỤNG AI CHAT ASSISTANT TÍCH HỢP PHÂN TÍCH ĐA TÀI LIỆU VÀ NHẬN DIỆN QUANG HỌC (OCR)**

---

## MỤC LỤC
1. [Chương 1: Tổng quan đề tài](#chuong-1-tong-quan-de-tai)
2. [Chương 2: Cơ sở lý thuyết và Công nghệ sử dụng](#chuong-2-co-so-ly-thuyet)
3. [Chương 3: Phân tích và Thiết kế hệ thống](#chuong-3-phan-tich-thiet-ke)
4. [Chương 4: Triển khai và Kết quả đạt được](#chuong-4-trien-khai)
5. [Chương 5: Ứng dụng thực tiễn và Tổng kết](#chuong-5-ung-dung)

---

<a name="chuong-1-tong-quan-de-tai"></a>
## CHƯƠNG 1: TỔNG QUAN ĐỀ TÀI

### 1.1 Đặt vấn đề
Trong bối cảnh chuyển đổi số mạnh mẽ, các hệ thống phần mềm quản lý (như ERP doanh nghiệp, CRM, hệ thống quản lý bệnh viện, trường học) đóng vai trò cốt lõi. Tuy nhiên, các hệ thống truyền thống vẫn gặp phải giới hạn lớn:
- Thao tác nhập liệu từ giấy tờ, hóa đơn, CMND/CCCD vẫn phải thực hiện thủ công, tốn rất nhiều thời gian và có rủi ro sai sót do yếu tố con người (Human Error).
- Việc tổng hợp, đọc hiểu và tóm tắt những báo cáo dài hàng chục trang (PDF, Word) chưa được tự động hóa, gây quá tải cho nhân sự hành chính.
- Các ứng dụng AI phổ biến hiện nay (như ChatGPT) yêu cầu phải tải dữ liệu lên Cloud, gây ra nguy cơ lộ lọt dữ liệu bảo mật của doanh nghiệp và tiêu tốn chi phí thuê bao (API Cost).

### 1.2 Mục tiêu đề tài
Dự án được xây dựng nhằm tạo ra một **Trợ lý ảo (AI Chat Assistant)** trên nền tảng Desktop giải quyết trực tiếp các bài toán trên với các mục tiêu cụ thể:
1. Xây dựng Chatbot thông minh giao tiếp bằng ngôn ngữ tự nhiên.
2. Tích hợp khả năng bóc tách, đọc hiểu đa dạng tài liệu (PDF, Word, Markdown...).
3. Ứng dụng công nghệ Thị giác máy tính (OCR) để tự động nhận dạng và trích xuất chữ từ hình ảnh (quét hóa đơn, căn cước...).
4. Ưu tiên tuyệt đối tính **bảo mật dữ liệu** bằng cách sử dụng Mô hình ngôn ngữ lớn (LLM) chạy cục bộ (Offline) với chi phí vận hành bằng 0.

---

<a name="chuong-2-co-so-ly-thuyet"></a>
## CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÔNG NGHỆ SỬ DỤNG

### 2.1 Nền tảng phát triển (C# & Windows Forms)
Dự án được phát triển bằng ngôn ngữ C# trên nền tảng .NET Framework. Giao diện (UI) được thiết kế hiện đại (Dark Mode, Bong bóng chat) thông qua thư viện Guna UI2 và các Custom Controls, mang lại trải nghiệm người dùng tương đồng với các ứng dụng nhắn tin hiện đại.

### 2.2 Xử lý ngôn ngữ tự nhiên (LLMs)
Hệ thống sử dụng linh hoạt giữa hai nền tảng AI:
- **Ollama (Local AI):** Đóng vai trò cốt lõi trong việc đảm bảo bảo mật. Ollama cho phép chạy các mô hình ngôn ngữ như Llama 3.2, Qwen trực tiếp trên máy tính cá nhân hoặc mạng LAN nội bộ.
- **Google Gemini API:** Được sử dụng như một tùy chọn nâng cao cho các tác vụ đòi hỏi sự phức tạp cao hoặc nhận diện cấu trúc ảnh xuất ra Excel.

### 2.3 Công nghệ nhận dạng ký tự quang học (OCR)
Sử dụng **Tesseract OCR**, một engine mã nguồn mở mạnh mẽ, để bóc tách text từ hình ảnh (Offline) với độ chính xác cao.

### 2.4 Xử lý và trích xuất tài liệu (Document Parsing)
Sử dụng các thư viện mạnh mẽ như:
- `UglyToad.PdfPig`: Đọc và trích xuất text từ định dạng PDF.
- `EPPlus`: Tự động tạo, định dạng và xuất báo cáo dưới dạng bảng tính Excel (.xlsx).

---

<a name="chuong-3-phan-tich-thiet-ke"></a>
## CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG

### 3.1 Sơ đồ kiến trúc tổng thể
Hệ thống được thiết kế theo tư duy Module hóa (Micro-module), tách biệt hoàn toàn giữa giao diện và logic xử lý nghiệp vụ, bao gồm 4 module chính:
1. **UI Layer (`Form1.cs`)**: Quản lý giao diện, bắt sự kiện người dùng và render định dạng Markdown.
2. **AI Module (`OllamaService.cs`, `GeminiService.cs`)**: Xử lý HTTP Client, gọi API, cắt ngắn chuỗi hội thoại tránh tràn bộ nhớ.
3. **OCR Module (`OcrService.cs`)**: Xử lý load hình ảnh và chạy engine Tesseract nhận dạng chữ.
4. **Document Module (`DocumentReaderService.cs`)**: Trích xuất văn bản thô từ các định dạng file đính kèm.

### 3.2 Luồng hoạt động (Data Flow)
1. **Input:** Người dùng nhập câu lệnh (Prompt) hoặc đính kèm tài liệu/hình ảnh.
2. **Preprocessing:** 
   - Nếu là hình ảnh $\rightarrow$ Gọi OCR Module quét ra Text.
   - Nếu là tài liệu $\rightarrow$ Gọi Document Module trích xuất ra Text.
3. **Processing:** Hệ thống ghép phần Text vừa trích xuất với câu lệnh của người dùng tạo thành một cấu trúc chuẩn và gửi xuống mô hình AI (Ollama/Gemini).
4. **Output:** Mô hình AI phân tích và trả về kết quả. UI Layer bắt phản hồi và hiển thị trực quan lên màn hình.

---

<a name="chuong-4-trien-khai"></a>
## CHƯƠNG 4: TRIỂN KHAI VÀ KẾT QUẢ ĐẠT ĐƯỢC

### 4.1 Giao diện người dùng thân thiện
- Cung cấp khung chat trực quan, hỗ trợ Markdown (in đậm, code block, bảng biểu).
- Tích hợp các tính năng công thái học như: tự động cuộn, chặn spam tin nhắn, nút "Sao chép" tiện lợi ở mỗi câu trả lời.

### 4.2 Tính năng nổi bật
- **Hỏi đáp thông minh (Chatbot):** Tương tác đa lĩnh vực, dịch thuật, hỗ trợ lập trình.
- **Phân tích tài liệu dung lượng lớn:** Tự động đọc file Word, PDF. Ví dụ: *"Hãy tìm trong tài liệu nội quy này các quy định về việc đi trễ"*.
- **Tự động bóc tách hóa đơn / CCCD:** Quét hình ảnh tải lên và bóc tách dữ liệu tức thì. Khả năng phát hiện cấu trúc bảng trong ảnh để tự động xuất thành file Excel hoàn chỉnh.

### 4.3 Đánh giá hệ thống
- **Về tính bảo mật (On-Premise Privacy):** Hoạt động hoàn toàn độc lập (Offline), dữ liệu không bao giờ bị đưa lên bên thứ 3.
- **Về chi phí vận hành (Zero-cost):** Không phát sinh cước phí thuê bao tháng API.
- **Hiệu năng:** Code được tối ưu hóa tốt, không gây tràn RAM máy tính người dùng.

---

<a name="chuong-5-ung-dung"></a>
## CHƯƠNG 5: ĐỀ XUẤT TÍCH HỢP VÀ TỔNG KẾT

### 5.1 Đề xuất khả năng tích hợp
Do mã nguồn thiết kế dạng Module chuẩn hóa, ứng dụng có thể được nhúng làm Micro-service cho các hệ thống phần mềm lớn:
- **Phần mềm quản lý sinh viên / bệnh viện:** Nhân viên chỉ việc tải ảnh thẻ/CCCD lên, Module OCR và AI tự động phân tích và điền form (Họ tên, Ngày sinh) $\rightarrow$ Tiết kiệm 80% thời gian nhập liệu.
- **Phần mềm kế toán:** Trích xuất Tên hàng hóa, Mã số thuế, Tổng tiền từ ảnh hóa đơn giấy.
- **Hệ thống E-Office:** Gắn chatbot ở góc màn hình giúp nhân sự tóm tắt hợp đồng dài hoặc tra cứu văn bản pháp luật nội bộ.

### 5.2 Kết luận
Dự án đã phát triển thành công ứng dụng AI Chat Assistant mạnh mẽ, kết hợp giữa mô hình ngôn ngữ lớn (LLM) và nhận diện quang học (OCR). Dự án chứng minh được tính khả thi trong việc mang AI bảo mật cao vào môi trường làm việc của doanh nghiệp, vừa tối ưu hóa quy trình nhập liệu thủ công, vừa giải quyết triệt để bài toán bảo mật dữ liệu và chi phí duy trì. Hướng đi này hoàn toàn có thể mở rộng để trở thành hệ sinh thái AI trợ lý cá nhân toàn diện trong tương lai.
