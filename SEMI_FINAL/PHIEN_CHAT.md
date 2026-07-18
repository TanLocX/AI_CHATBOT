# 📝 Phiên Chat Hướng Dẫn — AI Chat Assistant
**Ngày:** 16/07/2026 | **Dự án:** SEMI_FINAL — Bài Tập Giữa Kỳ .NET

---

## 🎯 Mục tiêu dự án
Xây dựng ứng dụng **AI Chat Assistant** bằng C# Windows Forms kết nối **Ollama API** (local).
- Chủ đề: Số 24 — Nhóm G (Tiện ích AI)
- Framework: .NET Framework 4.7.2
- UI Library: Guna.UI2.WinForms

---

## 📦 Cài đặt & Công nghệ

### NuGet Packages
| Package | Version | Mục đích |
|---|---|---|
| `Guna.UI2.WinForms` | 2.0.4.8 | UI controls đẹp |
| `Newtonsoft.Json` | 13.0.4 | Parse JSON từ Ollama |

### Ollama
- Tải tại: https://ollama.com/download
- Model đã cài: `llama3.2:latest` (2.0 GB)
- Ollama tự chạy ngầm sau khi cài — **không cần** `ollama serve`
- Kiểm tra: mở browser vào `http://localhost:11434`

---

## 🗂 Cấu trúc File Project

```
SEMI_FINAL/
├── Form1.cs              # Logic chính: ThemTinNhan, btnSend_Click...
├── Form1.Designer.cs     # UI Designer (Guna controls)
├── OllamaService.cs      # Kết nối Ollama API
├── Program.cs            # Entry point
├── packages.config       # Danh sách NuGet packages
└── PHIEN_CHAT.md         # File này
```

---

## 🔧 OllamaService.cs — Kết nối AI

```csharp
// Endpoint: POST http://localhost:11434/api/chat
// Request body:
{
    "model": "llama3.2",
    "messages": [{ "role": "user", "content": "..." }],
    "stream": false
}

// Response:
{
    "message": { "content": "Câu trả lời của AI..." }
}
```

**Các hàm chính:**
- `KiemTraKetNoi()` — GET http://localhost:11434 → trả về bool
- `GuiTinNhan(string tinNhan)` — POST /api/chat → trả về string
- `SetModel(string model)` — đổi model đang dùng

---

## 🖥 Form1.cs — Logic Giao Diện

### Biến quan trọng
```csharp
private OllamaService _ollamaService = new OllamaService();
private bool _dangChoAI = false;  // Chặn spam gửi khi AI đang trả lời
private int _nextMessageY = 15;   // Theo dõi vị trí Y tin nhắn tiếp theo
```

### Hàm ThemTinNhan — Render bong bóng chat
- Dùng `TextRenderer.MeasureText()` (không dùng `MeasureString` — sai engine)
- `TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl`
- Thêm buffer `+8` vào height để tránh bị cắt chữ
- Người dùng: căn phải (màu xanh DodgerBlue)
- AI: căn trái (màu xám 235,235,235)

### Luồng gửi tin nhắn
```
btnSend_Click()
  → ThemTinNhan(cauHoi, true)      // Hiện bong bóng người dùng
  → btnSend.Enabled = false        // Khóa nút
  → await _ollamaService.GuiTinNhan()  // Gọi API (async)
  → ThemTinNhan(traLoi, false)     // Hiện bong bóng AI
  → btnSend.Enabled = true         // Mở khóa nút
```

### Enter để gửi
```csharp
// Trong constructor Form1():
txtInput.KeyDown += txtInput_KeyDown;

private void txtInput_KeyDown(object sender, KeyEventArgs e)
{
    if (e.KeyCode == Keys.Enter && !e.Shift)
    {
        e.SuppressKeyPress = true;
        btnSend_Click(this, new EventArgs());
    }
}
```

---

## 🐛 Lỗi đã gặp & Cách fix

| Lỗi | Nguyên nhân | Fix |
|---|---|---|
| `JsonConvert` gạch đỏ | File tên `OllamaService.cs.cs` (thừa `.cs`) | Đổi tên file đúng |
| Gửi tin không thấy gì | `Guna2Panel` không render đúng | Thêm `Invalidate()` |
| Enter không gửi được | Event `KeyDown` gắn vào `btnSend` thay vì `txtInput` | Thêm event vào `txtInput` |
| `TxtInput_KeyDown` gạch đỏ | Tên hàm không khớp chữ hoa/thường | Đổi thành `txtInput_KeyDown` |
| `_nextMessageY` gạch đỏ | Quên khai báo biến | Thêm `private int _nextMessageY = 15;` |
| Tin nhắn bị mất nửa | `MeasureString` (GDI+) ≠ engine của Label (GDI) | Dùng `TextRenderer.MeasureText()` |
| Bong bóng vị trí nhảy lung tung | `AutoSize=true` → Height=0 khi chưa render | Dùng `_nextMessageY` tăng dần |

---

## ✅ Trạng thái hiện tại
- [x] Giao diện cơ bản (Guna UI)
- [x] Kết nối Ollama API
- [x] Gửi/nhận tin nhắn hoạt động
- [x] Chat bubble hiển thị đúng vị trí
- [x] Nhấn Enter để gửi
- [ ] Lưu lịch sử chat
- [ ] Nút xóa lịch sử
- [ ] Hiển thị trạng thái kết nối (lblStatus)
- [ ] Chọn model (ComboBox)

---

## 📌 Việc cần làm tiếp

1. **Lưu lịch sử chat** vào file `.json` (không dùng database)
2. **Nút Clear** để xóa màn hình chat và reset `_nextMessageY = 15`
3. **Hiển thị "AI đang trả lời..."** trong lúc chờ
4. **Quản lý context** — gửi cả lịch sử hội thoại cho AI nhớ ngữ cảnh
5. **Tài liệu báo cáo** (PDF 5-10 trang) + Developer Guide

---

## 💡 Gợi ý tích hợp (Mục 3.4 — Điểm cộng)

| Phần mềm | Tích hợp | Lợi ích |
|---|---|---|
| Quản lý sinh viên | Chatbot hỏi đáp quy định, điểm số | Giảm tải bộ phận hỗ trợ |
| Thư viện sách | Tư vấn sách, tra cứu tài liệu | Tìm kiếm nhanh hơn |
| E-learning | Giải bài tập, giải thích khái niệm | Học 24/7 không cần giáo viên |
| Dashboard | Phân tích số liệu bằng ngôn ngữ tự nhiên | Hiểu dữ liệu không cần chuyên môn |
