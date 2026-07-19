using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;        // ← cần dòng này cho JsonConvert
using Newtonsoft.Json.Linq;   // ← cần dòng này cho JObject



namespace SEMI_FINAL
{
    internal class OllamaService
    {
        // HttpClient dùng chung, không new mỗi lần (best practice)
        private static readonly HttpClient _httpClient = new HttpClient();

        // URL mặc định của Ollama khi chạy local
        private string _baseUrl;
        private string _model;

        // Giới hạn ký tự tối đa gửi lên AI — tránh tràn context window của model nhỏ
        private const int MAX_CONTENT_LENGTH = 8000;

        public OllamaService(string baseUrl = "http://localhost:11434", string model = "llama3.2")
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _model = model;

            // Tăng timeout lên 10 phút — model nhỏ như deepseek-r1:1.5b cần thời gian load và xử lý
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        /// <summary>
        /// Đổi địa chỉ máy chủ Ollama (Dùng khi kết nối Ollama trên máy khác trong mạng LAN)
        /// Ví dụ: SetBaseUrl("http://192.168.1.100:11434")
        /// </summary>
        public void SetBaseUrl(string baseUrl) => _baseUrl = baseUrl.TrimEnd('/');

        /// <summary>
        /// Đổi model đang dùng (ví dụ: llama3, mistral, gemma...)
        /// </summary>
        public void SetModel(string model) => _model = model;

        /// <summary>
        /// Lấy danh sách tất cả model đã cài trong Ollama (GET /api/tags)
        /// </summary>
        public async Task<List<string>> LayDanhSachModel()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
                if (!response.IsSuccessStatusCode) return new List<string>();

                string json = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);

                var models = obj["models"]?
                    .Select(m => m["name"]?.ToString())
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

                return models ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }


        /// <summary>
        /// Kiểm tra Ollama có đang chạy không
        /// </summary>
        public async Task<bool> KiemTraKetNoi()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gửi tin nhắn đến AI và nhận phản hồi (không streaming)
        /// </summary>
        /// <param name="tinNhan">Nội dung người dùng nhập</param>
        /// <returns>Chuỗi trả lời của AI</returns>
        public async Task<string> GuiTinNhan(string tinNhan)
        {
            // Cắt ngắn nếu quá dài — tránh tràn context window của model nhỏ (deepseek-r1:1.5b ~4096 tokens)
            string noiDungGuiDi = tinNhan;
            if (noiDungGuiDi.Length > MAX_CONTENT_LENGTH)
            {
                noiDungGuiDi = noiDungGuiDi.Substring(0, MAX_CONTENT_LENGTH)
                    + $"\n\n[... Nội dung đã bị cắt bớt do quá dài ({tinNhan.Length} ký tự, giới hạn {MAX_CONTENT_LENGTH}) ...]";
            }

            // Tạo body JSON theo chuẩn Ollama API
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = noiDungGuiDi }
                },
                stream = false  // false = trả về 1 lần, không stream từng chữ
            };

            try
            {
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // POST đến endpoint /api/chat
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);

                // Hiển thị lỗi HTTP rõ ràng thay vì thông báo chung chung
                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ollama trả về lỗi {(int)response.StatusCode}: {errorBody}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse JSON lấy nội dung trả lời
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
    }
}
