using System; // Cung cấp các hàm cơ sở, ngoại lệ và hệ thống kiểu dữ liệu
using System.Net.Http; // Hỗ trợ khởi tạo HttpClient để gọi các API HTTP/HTTPS
using System.Text; // Hỗ trợ xử lý bảng mã ký tự, đặc biệt dùng khi đẩy JSON lên server
using System.Threading.Tasks; // Cung cấp lớp Task hỗ trợ lập trình bất đồng bộ (async/await)
using Newtonsoft.Json; // Thư viện xử lý JSON giúp chuyển object C# sang chuỗi JSON
using Newtonsoft.Json.Linq; // Thư viện giúp truy xuất các thuộc tính trong cây JSON một cách linh hoạt
using System.Collections.Generic;
using System.IO; // Hỗ trợ đọc các file hệ thống, cụ thể là để lấy byte hình ảnh

namespace SEMI_FINAL // Phạm vi chung của toàn bộ class trong dự án
{
    public class GeminiService // Định nghĩa lớp công khai để giao tiếp với AI Gemini
    {
        private static readonly HttpClient _httpClient = new HttpClient(); // Khởi tạo một thể hiện tĩnh HttpClient để chia sẻ kết nối, tăng tốc độ gọi mạng
        private string _apiKey; // Khai báo chuỗi lưu trữ API Key để xác thực tài khoản Google

        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }

        public bool HasKey => !string.IsNullOrWhiteSpace(_apiKey);

        public GeminiService(string apiKey) // Hàm tạo có nhận tham số là khóa API
        {
            _apiKey = apiKey; // Gán khóa được truyền vào cho trường _apiKey nội bộ
            _httpClient.Timeout = TimeSpan.FromMinutes(2); // Thiết lập thời gian chờ phản hồi tối đa của HTTP client lên 2 phút
            ClearHistory();
        }

        public void SetApiKey(string newKey)
        {
            _apiKey = newKey;
        }

        private static List<object> _chatHistory = new List<object>();
        private string _historyFilePath = "chat_history.json";

        public void ClearHistory()
        {
            _chatHistory.Clear();
            if (File.Exists(_historyFilePath))
            {
                try { File.Delete(_historyFilePath); } catch { }
            }
        }

        public async Task<string> GuiTinNhan(string tinNhan, string imagePath = null) // Phương thức gửi câu hỏi dạng chuỗi và ảnh tùy chọn
        {
            if (tinNhan.Length > 30000) // Kiểm tra nếu độ dài tin nhắn vượt quá 30.000 ký tự (tránh lỗi quá dung lượng)
            {
                tinNhan = tinNhan.Substring(0, 30000) + "\n\n[... Nội dung đã bị cắt bớt do quá dài ...]"; // Trích xuất và giới hạn lại số lượng ký tự tối đa
            }

            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent"; // Cấu trúc đường dẫn URL API Gemini gắn với khóa bí mật
            
            object userMessage;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath)) // Trường hợp người dùng có đính kèm file và file tồn tại thực sự
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath); // Đọc nội dung file ảnh vào mảng byte
                string base64Image = Convert.ToBase64String(imageBytes); // Chuyển đổi dữ liệu byte của ảnh sang dạng chuỗi Base64
                string mimeType = GetMimeType(imagePath); // Gọi hàm để xác định định dạng Mime-Type (ví dụ image/png)

                userMessage = new
                {
                    role = "user",
                    parts = new object[]
                    {
                        new { text = tinNhan },
                        new { inline_data = new { mime_type = mimeType, data = base64Image } }
                    }
                };
            }
            else // Trường hợp chỉ chat văn bản thuần, không có ảnh
            {
                userMessage = new
                {
                    role = "user",
                    parts = new[] { new { text = tinNhan } }
                };
            }

            // Chỉ lưu lịch sử nếu không phải là lệnh OCR ẩn
            bool isOcr = tinNhan.Contains("Hãy phân tích hình ảnh này. Tìm các bảng dữ liệu");
            
            var currentContents = new List<object>();

            if (!isOcr)
            {
                currentContents.AddRange(_chatHistory);
                currentContents.Add(userMessage);
            }
            else
            {
                currentContents.Add(userMessage);
            }

            object requestBody = new { contents = currentContents };

            string jsonBody = JsonConvert.SerializeObject(requestBody); // Biến object vô danh C# trở thành chuỗi JSON tiêu chuẩn
            File.WriteAllText("gemini_log.txt", jsonBody);

            int maxRetries = 3;
            int attempt = 0;
            int delayMs = 1000;

            while (attempt < maxRetries)
            {
                try // Khối lệnh dễ sinh ra lỗi mạng
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    using (var content = new StringContent(jsonBody, Encoding.UTF8, "application/json")) // Đóng gói chuỗi JSON đó thành HTTP Body có định danh utf-8
                    {
                        request.Content = content;
                        request.Headers.Add("x-goog-api-key", _apiKey);

                        var response = await _httpClient.SendAsync(request); // Gửi Request POST bất đồng bộ đến endpoint của Gemini

                        if (!response.IsSuccessStatusCode) // Nếu HTTP Code trả về không ở nhóm 2xx (ví dụ 400, 500)
                        {
                            string errorBody = await response.Content.ReadAsStringAsync(); // Đọc nội dung thông báo lỗi từ server
                            
                            // Nếu là lỗi 503 hoặc 429 và chưa quá số lần thử lại
                            if ((response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || 
                                 (int)response.StatusCode == 429) && attempt < maxRetries - 1)
                            {
                                await Task.Delay(delayMs);
                                delayMs *= 2; // Tăng gấp đôi thời gian chờ cho lần thử sau
                                attempt++;
                                continue;
                            }

                            throw new Exception($"Gemini API Error {(int)response.StatusCode}: {errorBody}"); // Ném ngoại lệ cho hàm gọi bên ngoài biết lỗi chi tiết
                        }

                        string jsonResponse = await response.Content.ReadAsStringAsync(); // Lấy toàn bộ phản hồi từ Gemini dưới dạng JSON chữ
                        var obj = JObject.Parse(jsonResponse); // Phân tích chuỗi JSON trả về thành đối tượng DOM dạng cây JObject
                        string answerText = obj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "Không có phản hồi từ Gemini."; // Duyệt sâu vào cấu trúc JSON để lấy text, nếu null thì trả về chuỗi thay thế
                        
                        if (!isOcr)
                        {
                            _chatHistory.Add(userMessage);
                            _chatHistory.Add(new
                            {
                                role = "model",
                                parts = new[] { new { text = answerText } }
                            });
                            
                            try
                            {
                                string updatedHistoryJson = JsonConvert.SerializeObject(_chatHistory, Formatting.Indented);
                                File.WriteAllText(_historyFilePath, updatedHistoryJson);
                            }
                            catch { }
                        }
                        return answerText;
                    }
                }
                catch (Exception ex) // Bắt lại ngoại lệ 
                {
                    if (attempt == maxRetries - 1) throw new Exception("Lỗi khi kết nối Gemini API: " + ex.Message); // Đóng gói lỗi gốc thành chuỗi lỗi có thông tin tiếng Việt
                    attempt++;
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
            }
            throw new Exception("Lỗi không xác định khi gọi Gemini API.");
        }
        
        private string GetMimeType(string path) // Hàm riêng hỗ trợ phân loại Mime type của file ảnh
        {
            string ext = Path.GetExtension(path).ToLowerInvariant(); // Trích phần mở rộng file (ví dụ: .png) và hạ xuống chữ thường
            switch (ext) // Dùng Switch để đối chiếu các loại ảnh phổ biến
            {
                case ".jpg": // Định dạng jpg
                case ".jpeg": return "image/jpeg"; // Trả về Mime type của hệ JPEG
                case ".png": return "image/png"; // Trả về Mime type của hệ PNG
                case ".gif": return "image/gif"; // Trả về Mime type của hệ GIF
                case ".webp": return "image/webp"; // Trả về Mime type của hệ WEBP
                default: return "image/jpeg"; // Mặc định trả về định dạng JPEG nếu không biết
            }
        }
    }
}
