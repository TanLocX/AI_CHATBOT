using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UglyToad.PdfPig;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;

namespace SEMI_FINAL
{
    public class DocumentReaderService
    {
        private readonly OcrService _ocrService;

        public DocumentReaderService(OcrService ocrService)
        {
            _ocrService = ocrService;
        }

        public string ReadDocument(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy file tài liệu hoặc đường dẫn không hợp lệ.", filePath);
            }

            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                // 1. Nhóm hình ảnh -> dùng OCR (Tesseract)
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                case ".tiff":
                    return _ocrService.DocChuTuAnh(filePath);

                // 2. Nhóm văn bản thuần & Markdown & Data
                case ".txt":
                case ".md":
                case ".csv":
                case ".json":
                case ".xml":
                case ".log":
                case ".sql":
                case ".cs":
                case ".py":
                case ".js":
                case ".html":
                case ".css":
                    return File.ReadAllText(filePath, Encoding.UTF8);

                // 3. Nhóm PDF
                case ".pdf":
                    return ReadPdf(filePath);

                // 4. Nhóm Word (DOC / DOCX)
                case ".doc":
                case ".docx":
                    return ReadWord(filePath);

                // 5. Nhóm Excel (XLS / XLSX)
                case ".xls":
                case ".xlsx":
                    return ReadExcel(filePath);

                default:
                    // Thử đọc dạng text thuần nếu không xác định được đuôi file
                    try
                    {
                        return File.ReadAllText(filePath, Encoding.UTF8);
                    }
                    catch
                    {
                        throw new NotSupportedException($"Định dạng file ({ext}) hiện chưa được hỗ trợ.");
                    }
            }
        }

        private string ReadPdf(string filePath)
        {
            var sb = new StringBuilder();
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            return sb.ToString().Trim();
        }

        private string ReadWord(string filePath)
        {
            var sb = new StringBuilder();

            // Thử dùng NPOI XWPF trước cho Word (.docx / .doc)
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var docx = new XWPFDocument(stream);
                    foreach (var para in docx.Paragraphs)
                    {
                        if (!string.IsNullOrWhiteSpace(para.ParagraphText))
                        {
                            sb.AppendLine(para.ParagraphText);
                        }
                    }
                    foreach (var table in docx.Tables)
                    {
                        foreach (var row in table.Rows)
                        {
                            var cells = row.GetTableCells().Select(c => c.GetText().Trim());
                            sb.AppendLine(string.Join(" | ", cells));
                        }
                    }
                }
                string result = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(result)) return result;
            }
            catch
            {
                // Bỏ qua lỗi để chuyển sang phương án đọc Zip/XML trực tiếp
            }

            // Fallback đọc trực tiếp word/document.xml từ ZipArchive (rất nhanh và chuẩn xác với file .docx)
            sb.Clear();
            try
            {
                using (var archive = ZipFile.OpenRead(filePath))
                {
                    var entry = archive.GetEntry("word/document.xml");
                    if (entry != null)
                    {
                        using (var stream = entry.Open())
                        {
                            var xdoc = XDocument.Load(stream);
                            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                            var paras = xdoc.Descendants(w + "p");
                            foreach (var p in paras)
                            {
                                var texts = p.Descendants(w + "t").Select(t => t.Value);
                                string line = string.Join("", texts).Trim();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    sb.AppendLine(line);
                                }
                            }
                        }
                    }
                }
                string res = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(res)) return res;
            }
            catch
            {
                // Bỏ qua nếu không phải zip
            }

            // Nếu vẫn không được (hoặc là file .doc text thuần legacy), đọc như text
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch
            {
                return "Không thể trích xuất nội dung văn bản từ file Word này.";
            }
        }

        private string ReadExcel(string filePath)
        {
            var sb = new StringBuilder();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    if (sheet == null) continue;

                    sb.AppendLine($"--- [Sheet: {sheet.SheetName}] ---");
                    for (int r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
                    {
                        IRow row = sheet.GetRow(r);
                        if (row == null) continue;

                        var cellValues = new List<string>();
                        for (int c = row.FirstCellNum; c < row.LastCellNum; c++)
                        {
                            if (c < 0) continue;
                            NPOI.SS.UserModel.ICell cell = row.GetCell(c);
                            if (cell == null)
                            {
                                cellValues.Add("");
                            }
                            else
                            {
                                cellValues.Add(GetCellValueAsString(cell));
                            }
                        }
                        string rowStr = string.Join(" | ", cellValues).Trim(' ', '|');
                        if (!string.IsNullOrWhiteSpace(rowStr))
                        {
                            sb.AppendLine(rowStr);
                        }
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString().Trim();
        }

        private string GetCellValueAsString(NPOI.SS.UserModel.ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue != null ? cell.DateCellValue.Value.ToString("dd/MM/yyyy") : cell.NumericCellValue.ToString();
                    }
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    try
                    {
                        return cell.StringCellValue;
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Blank:
                default:
                    return "";
            }
        }
    }
}
