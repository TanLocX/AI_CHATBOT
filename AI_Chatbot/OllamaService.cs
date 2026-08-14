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
    public class OllamaService // Khai báo class OllamaService public
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

        public async Task<string> GuiTinNhan(string tinNhan, string imagePath = null, List<ChatMessage> history = null) // Hàm để gửi chat và hình ảnh (nếu có) lên Ollama AI
        {
            var messagesList = new List<object>();
            
            // 1. Thêm System Prompt cố định chỉ định Tiếng Việt
            messagesList.Add(new { role = "system", content = "You are a helpful AI assistant. You MUST ALWAYS respond entirely in Vietnamese language (Tiếng Việt). Never use English in your responses, even if the user's prompt is in English. Translate any analysis to Vietnamese." });

            // 2. Thêm lịch sử hội thoại trước đó (nếu có)
            if (history != null && history.Count > 0)
            {
                foreach (var msg in history)
                {
                    string contentMsg = msg.Content;
                    if (contentMsg.Length > MAX_CONTENT_LENGTH)
                    {
                        contentMsg = contentMsg.Substring(0, MAX_CONTENT_LENGTH) + "\n[... cắt bớt ...]";
                    }

                    if (!string.IsNullOrEmpty(msg.ImagePath) && File.Exists(msg.ImagePath))
                    {
                        byte[] imgBytes = File.ReadAllBytes(msg.ImagePath);
                        string base64 = Convert.ToBase64String(imgBytes);
                        messagesList.Add(new { role = msg.Role, content = contentMsg, images = new[] { base64 } });
                    }
                    else
                    {
                        messagesList.Add(new { role = msg.Role, content = contentMsg });
                    }
                }
            }

            // 3. Thêm tin nhắn hiện tại của người dùng
            string noiDungGuiDi = tinNhan;
            if (noiDungGuiDi.Length > MAX_CONTENT_LENGTH)
            {
                noiDungGuiDi = noiDungGuiDi.Substring(0, MAX_CONTENT_LENGTH) 
                    + $"\n\n[... Nội dung đã bị cắt bớt do quá dài ({tinNhan.Length} ký tự, giới hạn {MAX_CONTENT_LENGTH}) ...]";
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);
                messagesList.Add(new { role = "user", content = noiDungGuiDi, images = new[] { base64Image } });
            }
            else
            {
                messagesList.Add(new { role = "user", content = noiDungGuiDi });
            }

            var requestBody = new 
            {
                model = _model,
                messages = messagesList,
                stream = false
            };

            try
            {
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ollama trả về lỗi {(int)response.StatusCode}: {errorBody}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);
                return obj["message"]?["content"]?.ToString() ?? "Không có phản hồi.";
            }
            catch (TaskCanceledException)
            {
                throw new Exception($"Hết thời gian chờ (timeout 10 phút). Model '{_model}' xử lý quá lâu — thử dùng model lớn hơn hoặc rút ngắn nội dung.");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Không thể kết nối Ollama ({_baseUrl}).\nNguyên nhân: {ex.Message}\n\nGợi ý: Ollama đang load model mới — hãy chờ 10-30 giây rồi thử lại.");
            }
        }

        public async Task PullModel(string modelName, Action<long, long, string> onProgress, System.Threading.CancellationToken cancellationToken = default)
        {
            var requestBody = new { name = modelName, stream = true };
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/pull")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string err = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Không thể tải model '{modelName}': {err}");
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new System.IO.StreamReader(stream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        try
                        {
                            var obj = JObject.Parse(line);
                            string status = obj["status"]?.ToString() ?? "";
                            long completed = (long?)obj["completed"] ?? 0L;
                            long total = (long?)obj["total"] ?? 0L;

                            onProgress?.Invoke(completed, total, status);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
