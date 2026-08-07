using System; // Cung cấp các chức năng cốt lõi (kiểu dữ liệu, ngoại lệ)
using System.Collections.Generic; // Cung cấp thư viện danh sách generic (ví dụ: List<T>)
using System.Linq; // Hỗ trợ cú pháp truy vấn trên các tập hợp như List, Array
using System.Net.Http; // Cung cấp HttpClient gửi yêu cầu mạng HTTP/REST
using System.Text; // Hỗ trợ định dạng chuỗi (Encoding.UTF8)
using System.Threading.Tasks; // Cung cấp khả năng lập trình xử lý đa luồng bất đồng bộ (Task, async/await)
using Newtonsoft.Json; // Thư viện bên thứ ba giúp tuần tự hóa (serialize) dữ liệu ra JSON
using Newtonsoft.Json.Linq; // Hỗ trợ thao tác truy vấn động (parse) chuỗi JSON
using System.IO; // Cung cấp truy cập I/O hệ thống file

namespace SEMI_FINAL // Namespace nhóm các class trong project lại
{
    internal class OllamaService // Khai báo class OllamaService ở mức nội bộ project
    {
        private static readonly HttpClient _httpClient = new HttpClient(); // Dùng chung 1 HttpClient (Best practice để tránh tràn socket)

        private string _baseUrl; // Lưu chuỗi URL trỏ đến máy chủ chứa Ollama
        private string _model; // Lưu chuỗi tên model đang dùng để gọi (llama3.2, mistral,...)

        private const int MAX_CONTENT_LENGTH = 8000; // Giới hạn số lượng ký tự chữ nhằm tránh tràn ngưỡng xử lý của model nhỏ

        public OllamaService(string baseUrl = "http://localhost:11434", string model = "llama3.2") // Khởi tạo với URL và tên model mặc định
        {
            _baseUrl = baseUrl.TrimEnd('/'); // Xóa dấu gạch chéo cuối nếu người dùng lỡ tay thêm
            _model = model; // Lưu lại model vào biến

            _httpClient.Timeout = TimeSpan.FromMinutes(10); // Cấu hình thời gian tối đa để chờ phản hồi lên tới 10 phút
        }

        public void SetBaseUrl(string baseUrl) => _baseUrl = baseUrl.TrimEnd('/'); // Hàm public để thay đổi địa chỉ IP của server (khi đổi mạng)

        public void SetModel(string model) => _model = model; // Hàm public cho phép chuyển đổi model AI đang sử dụng

        public async Task<List<string>> LayDanhSachModel() // Hàm tải toàn bộ các model đã cài trên Ollama bằng phương thức GET
        {
            try // Thử thực hiện kết nối HTTP
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags"); // Request API lấy danh sách tags từ Ollama
                if (!response.IsSuccessStatusCode) return new List<string>(); // Nếu phản hồi sai, lập tức trả về danh sách trống

                string json = await response.Content.ReadAsStringAsync(); // Đọc dữ liệu dạng text của phản hồi
                var obj = JObject.Parse(json); // Phân tích chuỗi thu được thành dạng JObject

                var models = obj["models"]? // Đi vào node "models"
                    .Select(m => m["name"]?.ToString()) // Biến danh sách node thành danh sách chuỗi 'name' (Tên model)
                    .Where(n => !string.IsNullOrEmpty(n)) // Lọc loại bỏ những giá trị rỗng/null
                    .ToList(); // Ép ngược lại thành kiểu List<string>

                return models ?? new List<string>(); // Trả lại danh sách tên các model, nếu List null thì trả mảng rỗng
            }
            catch // Bắt ngoại lệ nếu không thể tải hay server tịt
            {
                return new List<string>(); // Trả về rỗng để chương trình chạy tiếp
            }
        }

        public async Task<bool> KiemTraKetNoi() // Hàm kiểm tra tín hiệu máy chủ xem Ollama có online không
        {
            try // Thử chạy lệnh request Get
            {
                var response = await _httpClient.GetAsync(_baseUrl); // Gõ GET tới root endpoint
                return response.IsSuccessStatusCode; // Nếu trả về 200 OK thì coi như kết nối tốt
            }
            catch // Bắt các lỗi không tìm thấy host
            {
                return false; // Nếu gặp lỗi nghĩa là kết nối thất bại
            }
        }

        public async Task<string> GuiTinNhan(string tinNhan, string imagePath = null) // Hàm để gửi chat và hình ảnh (nếu có) lên Ollama AI
        {
            string noiDungGuiDi = tinNhan; // Tạo bản sao cho tin nhắn đầu vào
            if (noiDungGuiDi.Length > MAX_CONTENT_LENGTH) // Nếu độ dài vượt ngưỡng cho phép 8000 ký tự
            {
                noiDungGuiDi = noiDungGuiDi.Substring(0, MAX_CONTENT_LENGTH) // Cắt chuỗi, giữ 8000 ký tự đầu
                    + $"\n\n[... Nội dung đã bị cắt bớt do quá dài ({tinNhan.Length} ký tự, giới hạn {MAX_CONTENT_LENGTH}) ...]"; // Gắn thêm thông báo bị cắt bớt ở đuôi
            }

            object messageObject; // Biến vô danh chứa cấu trúc tin nhắn để truyền json
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath)) // Trường hợp người dùng có truyền ảnh
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath); // Load toàn bộ byte ảnh
                string base64Image = Convert.ToBase64String(imageBytes); // Đổi byte thành chuỗi Base64
                messageObject = new { role = "user", content = noiDungGuiDi, images = new[] { base64Image } }; // Khởi tạo payload chuẩn của Ollama vision có kèm biến image
            }
            else // Trường hợp không có hình ảnh
            {
                messageObject = new { role = "user", content = noiDungGuiDi }; // Khởi tạo payload Ollama chuẩn nhưng loại bỏ hình ảnh
            }

            var requestBody = new // Bao bọc nội dung vào body JSON 
            {
                model = _model, // Tên mô hình sẽ xử lý
                messages = new object[] // Tạo mảng tin nhắn
                { 
                    new { role = "system", content = "You are a helpful AI assistant. You MUST ALWAYS respond entirely in Vietnamese language (Tiếng Việt). Never use English in your responses, even if the user's prompt is in English. Translate any analysis to Vietnamese." }, // Hệ thống tự nhúng thêm System Prompt chỉ định tiếng Việt để tránh AI xài tiếng Anh
                    messageObject // Tin nhắn của người dùng
                },
                stream = false // Vô hiệu hóa stream để AI nhận đủ response rồi mới nhả JSON
            };

            try // Bọc các xử lý liên quan kết nối HTTP
            {
                string jsonBody = JsonConvert.SerializeObject(requestBody); // Đóng gói dữ liệu đối tượng sang dạng string JSON
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json"); // Chuẩn bị StringContent UTF8 với type JSON

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content); // Thực thi HTTP Post lên URL của API chat Ollama

                if (!response.IsSuccessStatusCode) // Xử lý nếu mã trạng thái khác 200
                {
                    string errorBody = await response.Content.ReadAsStringAsync(); // Lấy message chi tiết từ server
                    throw new Exception($"Ollama trả về lỗi {(int)response.StatusCode}: {errorBody}"); // Đẩy exception báo lỗi
                }

                string jsonResponse = await response.Content.ReadAsStringAsync(); // Trích xuất chuỗi trả lời sau khi thành công

                var obj = JObject.Parse(jsonResponse); // Deserialize JSON thành cây dạng từ điển
                return obj["message"]?["content"]?.ToString() ?? "Không có phản hồi."; // Tìm đường dẫn tới nội dung văn bản và xử lý nếu nó rỗng
            }
            catch (TaskCanceledException) // Bắt lỗi HTTP timeout
            {
                throw new Exception($"Hết thời gian chờ (timeout 10 phút). Model '{_model}' xử lý quá lâu — thử dùng model lớn hơn hoặc rút ngắn nội dung."); // Thông báo dễ đọc cho người dùng về timeout
            }
            catch (HttpRequestException ex) // Bắt lỗi mạng hoặc server đóng kết nối
            {
                throw new Exception($"Không thể kết nối Ollama ({_baseUrl}).\nNguyên nhân: {ex.Message}\n\nGợi ý: Ollama đang load model mới — hãy chờ 10-30 giây rồi thử lại."); // Thông báo và gợi ý
            }
        }
    }
}
