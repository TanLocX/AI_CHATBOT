using System;
using System.Drawing;
using System.IO;
using Tesseract;

namespace SEMI_FINAL
{
    public class OcrService
    {
        private string _tessDataPath;

        public OcrService()
        {
            // Tìm thư mục tessdata trong thư mục chạy (bin\Debug\tessdata hoặc ./tessdata)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _tessDataPath = Path.Combine(baseDir, "tessdata");

            if (!Directory.Exists(_tessDataPath))
            {
                // Thử tìm ở thư mục gốc dự án nếu chạy debug chưa copy
                string projectTessData = Path.Combine(baseDir, @"..\..\tessdata");
                if (Directory.Exists(projectTessData))
                {
                    _tessDataPath = Path.GetFullPath(projectTessData);
                }
            }
        }

        /// <summary>
        /// Đọc nội dung văn bản từ ảnh using Tesseract OCR (hỗ trợ Tiếng Việt + Tiếng Anh)
        /// </summary>
        public string DocChuTuAnh(string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Không tìm thấy file ảnh: " + imagePath);

            if (!Directory.Exists(_tessDataPath))
                throw new DirectoryNotFoundException("Không tìm thấy dữ liệu Tesseract tại: " + _tessDataPath + "\nHãy đảm bảo thư mục tessdata chứa vie.traineddata và eng.traineddata");

            try
            {
                using (var engine = new TesseractEngine(_tessDataPath, "vie+eng", EngineMode.Default))
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string text = page.GetText();
                        return string.IsNullOrWhiteSpace(text) ? "(Không nhận diện được chữ nào trong ảnh)" : text.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đọc chữ từ ảnh (OCR): " + ex.Message, ex);
            }
        }
    }
}
