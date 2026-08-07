using System; // Cung cấp các kiểu dữ liệu và ngoại lệ cơ bản
using System.IO; // Cung cấp các lớp để đọc và ghi tệp, luồng dữ liệu
using System.IO.Compression; // Hỗ trợ đọc/ghi nội dung tệp nén (như file .zip hoặc cấu trúc bên trong .docx)
using System.Linq; // Cung cấp các phương thức truy vấn dữ liệu (LINQ)
using System.Text; // Chứa các lớp xử lý và biểu diễn bảng mã chuỗi ký tự (như UTF-8)
using System.Xml.Linq; // Cung cấp khả năng phân tích và làm việc với cấu trúc XML
using UglyToad.PdfPig; // Thư viện của bên thứ 3 dùng để đọc và trích xuất nội dung từ tệp PDF

namespace SEMI_FINAL // Bọc các lớp bên trong không gian tên SEMI_FINAL
{
    public class DocumentReaderService // Khai báo lớp công khai chịu trách nhiệm đọc tài liệu
    {
        public string ReadDocument(string filePath) // Hàm chính để xử lý đọc tài liệu từ đường dẫn được cung cấp
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) // Kiểm tra nếu đường dẫn truyền vào bị trống hoặc file không tồn tại
            {
                throw new FileNotFoundException("Không tìm thấy file tài liệu hoặc đường dẫn không hợp lệ.", filePath); // Ném ngoại lệ báo thiếu tệp
            }

            string ext = Path.GetExtension(filePath).ToLowerInvariant(); // Lấy đuôi mở rộng của tệp tin và chuyển đổi thành chữ thường để chuẩn hóa

            switch (ext) // Dùng câu lệnh switch để xử lý từng loại đuôi tệp khác nhau
            {
                case ".txt": // Xử lý cho định dạng text thuần
                case ".md": // Xử lý cho định dạng Markdown
                case ".csv": // Xử lý cho định dạng dữ liệu bảng phân cách bằng dấu phẩy
                case ".json": // Xử lý cho định dạng cấu trúc dữ liệu JSON
                case ".xml": // Xử lý cho định dạng cấu trúc thẻ XML
                case ".log": // Xử lý cho định dạng lưu vết log
                case ".sql": // Xử lý cho định dạng truy vấn cơ sở dữ liệu
                case ".cs": // Xử lý cho file mã nguồn ngôn ngữ C#
                case ".py": // Xử lý cho file mã nguồn ngôn ngữ Python
                case ".js": // Xử lý cho file mã nguồn JavaScript
                case ".html": // Xử lý cho cấu trúc website
                case ".css": // Xử lý cho file kiểu dáng CSS
                    return File.ReadAllText(filePath, Encoding.UTF8); // Đọc toàn bộ nội dung tệp với bảng mã UTF-8 và trả về trực tiếp

                case ".pdf": // Xử lý trường hợp đuôi file là .pdf
                    return ReadPdf(filePath); // Chuyển luồng thực thi sang phương thức ReadPdf

                case ".doc": // Xử lý trường hợp đuôi file word định dạng cũ
                case ".docx": // Xử lý trường hợp đuôi file word định dạng mới
                    return ReadWord(filePath); // Chuyển luồng thực thi sang phương thức ReadWord

                default: // Trường hợp không rơi vào các đuôi đã biết
                    try // Thử chạy khối lệnh bên trong
                    {
                        return File.ReadAllText(filePath, Encoding.UTF8); // Cố gắng đọc tệp tin dưới dạng text thuần
                    }
                    catch // Bắt bất kỳ lỗi nào xảy ra khi việc đọc thất bại
                    {
                        throw new NotSupportedException($"Định dạng file ({ext}) hiện chưa được hỗ trợ."); // Ném ra lỗi định dạng không hỗ trợ
                    }
            }
        }

        private string ReadPdf(string filePath) // Hàm riêng tư phụ trách đọc file .pdf
        {
            var sb = new StringBuilder(); // Khởi tạo đối tượng StringBuilder để gom văn bản một cách hiệu quả
            using (var pdf = PdfDocument.Open(filePath)) // Mở tệp pdf qua UglyToad.PdfPig và giải phóng sau khi xong
            {
                foreach (var page in pdf.GetPages()) // Vòng lặp duyệt qua tất cả các trang của tài liệu pdf
                {
                    sb.AppendLine(page.Text); // Lấy văn bản thô trên trang đó và nối thêm một dòng mới vào StringBuilder
                }
            }
            return sb.ToString().Trim(); // Biến đổi StringBuilder thành chuỗi kết quả, xóa khoảng trắng ở 2 đầu và trả về
        }

        private string ReadWord(string filePath) // Hàm riêng tư phụ trách đọc file .doc/.docx
        {
            var sb = new StringBuilder(); // Khởi tạo StringBuilder để chứa dữ liệu văn bản

            try // Bọc khối lệnh có thể gây lỗi khi đọc file Zip
            {
                using (var archive = ZipFile.OpenRead(filePath)) // Mở file docx như là một file nén zip thông thường
                {
                    var entry = archive.GetEntry("word/document.xml"); // Tìm file có tên 'word/document.xml' chứa toàn bộ text
                    if (entry != null) // Đảm bảo file cấu trúc có tồn tại bên trong file zip
                    {
                        using (var stream = entry.Open()) // Mở luồng để đọc nội dung của document.xml
                        {
                            var xdoc = XDocument.Load(stream); // Dùng thư viện XML nạp luồng vừa tạo vào cấu trúc cây XDocument
                            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"; // Định nghĩa không gian tên mặc định cho Word
                            var paras = xdoc.Descendants(w + "p"); // Lấy ra toàn bộ danh sách các thẻ đoạn văn (thẻ 'p')
                            foreach (var p in paras) // Duyệt tuần tự qua mỗi thẻ đoạn văn
                            {
                                var texts = p.Descendants(w + "t").Select(t => t.Value); // Lấy giá trị của tất cả thẻ văn bản 't' nằm bên trong đoạn văn đó
                                string line = string.Join("", texts).Trim(); // Ghép các chuỗi con lại với nhau thành câu liền mạch và cắt khoảng trống dư
                                if (!string.IsNullOrEmpty(line)) // Chỉ lấy những câu thực sự có dữ liệu
                                {
                                    sb.AppendLine(line); // Lưu câu đó xuống một dòng mới của kết quả
                                }
                            }
                        }
                    }
                }
                string res = sb.ToString().Trim(); // Lấy tất cả văn bản trích xuất được và bỏ dấu xuống dòng thừa cuối
                if (!string.IsNullOrEmpty(res)) return res; // Trả về kết quả nếu đã đọc thành công dữ liệu
            }
            catch // Bắt những lỗi ném ra do file không phải file nén zip (.docx) chuẩn
            {
                // Im lặng bắt ngoại lệ, cho phép tiếp tục fallback xuống cách thức xử lý file dưới
            }

            try // Bước cuối, cố gắng xử lý như file .doc thuần cũ
            {
                return File.ReadAllText(filePath, Encoding.UTF8); // Thử nghiệm lấy toàn bộ byte và giải mã theo cấu trúc utf8
            }
            catch // Nắm bắt mọi lỗi do giải mã thất bại
            {
                return "Không thể trích xuất nội dung văn bản từ file Word này."; // Trả về thông điệp lỗi tự cấu hình thay vì ném exception cho app văng
            }
        }
    }
}
