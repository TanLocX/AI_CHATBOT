# BÁO CÁO NGHIÊN CỨU & KẾ HOẠCH TRIỂN KHAI: ĐỔI NỐI DỮ LIỆU DỰ PHÒNG CHUYỂN VÙNG OLLAMA (REMOTE SERVER VS LOCALHOST)

> **Cập nhật ngày:** 21/07/2026  
> **Dự án:** AI Chat Assistant (C# .NET WinForms)  
> **Trạng thái:** Đã triển khai tự động kích hoạt `ollama serve` local trong `Form1.cs`, đang hoàn thiện bộ Failover Nối kép trong `OllamaService.cs`.

---

## 1. ĐẶT VẤN ĐỀ VÀ MỤC TIÊU

### Thực trạng:
- Trong mã nguồn `Form1.cs` và `OllamaService.cs`, ứng dụng đã từng cấu hình IP ZeroTier của máy chủ Remote (`http://192.168.193.10:11434`), sau đó được chuyển về `http://localhost:11434` do Remote Server tạm đóng hoặc thay đổi môi trường mạng.
- Việc thay đổi thủ công URL trong code dẫn tới thiếu linh hoạt khi máy chủ Remote mở lại hoặc khi người dùng di chuyển giữa các mạng (LAN công ty vs Mạng cá nhân).

### Mục tiêu:
Xây dựng **Cơ chế Chuyển đổi Nối kép (Dynamic Dual-Connection Fallback)** với 2 đường chạy:
1. **Đường 1 (Ưu tiên - Remote Server)**: Kết nối tới máy chủ GPU tập trung (`http://192.168.193.10:11434` hoặc IP LAN chỉ định).
2. **Đường 2 (Dự phòng - Localhost)**: Tự động chuyển về máy cục bộ (`http://localhost:11434`) nếu Remote Server đóng/không kết nối được, đồng thời tự kích hoạt tiến trình `ollama serve` ngầm không hiện cửa sổ CMD.

---

## 2. KIẾN TRÚC VÀ LUỒNG XỬ LÝ NỐI KÉP (DUAL-CONNECTION FLOW)

```
                       ┌──────────────────────────────┐
                       │   Khởi động Ứng dụng C#     │
                       └──────────────┬───────────────┘
                                      │
                                      ▼
                       ┌──────────────────────────────┐
                       │  Kiểm tra kết nối Server     │
                       │ (http://192.168.193.10:11434)│
                       └──────────────┬───────────────┘
                                      │
                      ┌───────────────┴───────────────┐
                      │                               │
             (Thành công <= 2s)                (Thất bại / Timeout)
                      │                               │
                      ▼                               ▼
       ┌─────────────────────────────┐ ┌─────────────────────────────┐
       │   Dùng ĐƯỜNG 1: REMOTE      │ │  Chuyển sang ĐƯỜNG 2: LOCAL │
       │  (Chạy model lớn, GPU mạnh) │ │ (http://localhost:11434)    │
       └──────────────┬──────────────┘ └──────────────┬──────────────┘
                      │                               │
                      │                               ▼
                      │                ┌─────────────────────────────┐
                      │                │  Kiểm tra Ollama Localhost  │
                      │                └──────────────┬──────────────┘
                      │                               │
                      │                      ┌────────┴────────┐
                      │                 (Chưa chạy)        (Đã chạy)
                      │                      │                 │
                      │                      ▼                 │
                      │         ┌──────────────────────────┐   │
                      │         │ Chạy ngầm `ollama serve`  │   │
                      │         └────────────┬─────────────┘   │
                      │                      │                 │
                      ▼                      ▼                 ▼
       ┌─────────────────────────────────────────────────────────────┐
       │    Cập nhật UI Status Bar & Tải Danh sách Models tương ứng │
       └─────────────────────────────────────────────────────────────┘
```

---

## 3. THIẾT KẾ CHI TIẾT TRONG NGUỒN C#

### 3.1 Nâng cấp `OllamaService.cs`
Bổ sung các thuộc tính và phương thức quản lý đa máy chủ:
- `private List<string> _serverCandidates`: Danh sách URL các server (đứng đầu là Remote IP, tiếp theo là Localhost).
- `private string _activeBaseUrl`: Địa chỉ Server đang hoạt động thực tế.
- `public async Task<string> TimServerKhaDungAsync()`:
  - Lần lượt gửi request siêu nhẹ (`GET /api/tags` hoặc `GET /`) với timeout ngắn (**2 giây**).
  - Trả về Server đầu tiên phản hồi thành công (`IsSuccessStatusCode == true`).

### 3.2 Tự động Chuyển đổi & Kích hoạt Localhost trong `Form1.cs`
- Trong `Form1.cs`, hàm `TuDongKhoiDongOllama()` đã triển khai cơ chế kiểm tra tiến trình `ollama.exe` và tự động khởi động tiến trình ngầm `ollama serve` (không hiện cửa sổ CMD) nếu dịch vụ local chưa sẵn sàng.

---

## 4. DANH SÁCH TASK TRIỂN KHAI & TIẾN ĐỘ (IMPLEMENTATION TASKS)

- [x] **Task 1 (Đã làm trong Form1.cs)**: Tự động phát hiện và khởi động ngầm tiến trình `ollama serve` khi Ollama Local chưa chạy.
- [x] **Task 2 (Đã làm trong OllamaService.cs)**: Xây dựng cơ chế đổi URL linh hoạt (`SetBaseUrl`) và kiểm tra sức khỏe máy chủ (`KiemTraKetNoi()`).
- [ ] **Task 3**: Cập nhật `OllamaService.cs` hỗ trợ danh sách `_serverCandidates` tự động quét danh sách IP Remote -> Localhost với Timeout 2000ms.
- [ ] **Task 4**: Cập nhật `Form1.cs` hiển thị chỉ báo trạng thái máy chủ (Remote vs Localhost) trên thanh tiêu đề/status bar.
- [ ] **Task 5**: Thêm nút "Tải lại kết nối / Kiểm tra lại Máy chủ Remote" trên Sidebar để người dùng thử lại thủ công khi Remote Server mở lại.

---

## 5. MÃ NGUỒN THAM CHIẾU C# CHO BỘ FAILOVER

### Đoạn mã C# mẫu kiểm tra và tự chuyển vùng Server (`OllamaService.cs`):

```csharp
private readonly List<string> _serverCandidates = new List<string>
{
    "http://192.168.193.10:11434", // Đường 1: Remote Server (Ưu tiên)
    "http://localhost:11434"        // Đường 2: Localhost (Dự phòng)
};

/// <summary>
/// Quét danh sách Server và tự chọn Server khả dụng đầu tiên
/// </summary>
public async Task<string> TimServerKhaDungAsync()
{
    foreach (var url in _serverCandidates)
    {
        try
        {
            using (var cts = new System.Threading.CancellationTokenSource(2000)) // Timeout 2s
            {
                var response = await _httpClient.GetAsync(url, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    _baseUrl = url;
                    return url; // Trả về Server phản hồi đầu tiên
                }
            }
        }
        catch
        {
            // Tiếp tục thử Server tiếp theo trong danh sách
        }
    }
    
    // Nếu tất cả đều thất bại, mặc định về Localhost
    _baseUrl = "http://localhost:11434";
    return _baseUrl;
}
```

---

## 6. ĐÁNH GIÁ VÀ LỢI ÍCH

| Tiêu chí | Cấu hình Cứng (Ban đầu) | Kiến trúc Nối kép (Mới) |
| :--- | :--- | :--- |
| **Tính linh hoạt** | Phải sửa code và biên dịch lại khi Server bật/tắt | Tự động phát hiện và chuyển đổi trong 2 giây |
| **Trải nghiệm người dùng** | Gặp lỗi crash/freeze khi Server đóng | Hoạt động liên tục không gián đoạn (Fallback Local) |
| **Tận dụng tài nguyên** | Chỉ dùng được 1 môi trường cố định | Ưu tiên GPU mạnh của Server, sẵn sàng chạy Offline |
| **Tự động hóa** | Người dùng phải tự chạy CMD `ollama serve` | Ứng dụng tự kích hoạt tiến trình ngầm nếu cần |
