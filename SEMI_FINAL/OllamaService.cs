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

        public OllamaService(string baseUrl = "http://localhost:11434", string model = "llama3.2")
        {
            _baseUrl = baseUrl;
            _model = model;

            // Timeout 3 phút — AI đôi khi trả lời lâu
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
        }

        /// <summary>
        /// Đổi model đang dùng (ví dụ: llama3, mistral, gemma...)
        /// </summary>
        public void SetModel(string model) => _model = model;

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
            // Tạo body JSON theo chuẩn Ollama API
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                        new { role = "user", content = tinNhan }
                    },
                stream = false  // false = trả về 1 lần, không stream từng chữ
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // POST đến endpoint /api/chat
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Parse JSON lấy nội dung trả lời
            var obj = JObject.Parse(jsonResponse);
            return obj["message"]?["content"]?.ToString() ?? "Không có phản hồi.";
        }
    }
}
