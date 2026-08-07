using System; // Sử dụng namespace cơ bản của hệ thống
using System.Drawing; // Sử dụng namespace hỗ trợ xử lý hình ảnh (Point, Size, Rectangle,...)
using Tesseract; // Sử dụng thư viện Tesseract OCR để nhận diện chữ
using System.IO; // Sử dụng namespace xử lý tệp tin và luồng (File, Path,...)

namespace SEMI_FINAL // Khai báo không gian tên SEMI_FINAL cho toàn bộ dự án
{
    public class OcrService // Khai báo lớp OcrService ở mức truy cập public
    {
        private readonly string _tessDataPath; // Khai báo biến chứa đường dẫn dữ liệu ngôn ngữ của Tesseract (chỉ đọc)

        public OcrService() // Hàm khởi tạo của lớp OcrService
        {
            _tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"); // Gán đường dẫn tuyệt đối đến thư mục 'tessdata' nằm trong thư mục gốc của chương trình
        }

        public string ExtractTextFromImage(string imagePath) // Hàm nhận đường dẫn ảnh, thực thi nhận diện và trả về văn bản
        {
            if (!File.Exists(imagePath)) // Kiểm tra xem file ảnh có tồn tại trên đĩa hay không
                throw new FileNotFoundException("Không tìm thấy file ảnh.", imagePath); // Quăng lỗi nếu không tìm thấy file

            using (var engine = new TesseractEngine(_tessDataPath, "vie+eng", EngineMode.Default)) // Khởi tạo bộ nhận diện Tesseract cho tiếng Việt và Anh, dùng 'using' để tự giải phóng tài nguyên
            {
                using (var img = Pix.LoadFromFile(imagePath)) // Tải hình ảnh vào đối tượng Pix (của thư viện Tesseract)
                {
                    using (var page = engine.Process(img)) // Chạy thuật toán nhận diện chữ trên đối tượng hình ảnh
                    {
                        return page.GetText().Trim(); // Lấy văn bản kết quả, cắt bỏ khoảng trắng hai đầu rồi trả về
                    } // Tự động dọn dẹp biến page
                } // Tự động dọn dẹp biến img
            } // Tự động dọn dẹp biến engine
        }
    }
}
