namespace SEMI_FINAL
{
    public class ChatMessage
    {
        public string Role { get; set; } // "user" hoặc "assistant" / "model"
        public string Content { get; set; } // Nội dung tin nhắn
        public string ImagePath { get; set; } // Đường dẫn ảnh đính kèm (nếu có)
    }
}
