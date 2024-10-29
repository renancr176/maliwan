namespace Maliwan.Domain.Core.Data;

public class FileDownload
{
    public string FileBase64 { get; set; }
    public string MimeType { get; set; }
    public string FileName { get; set; }

    public FileDownload(string fileBase64, string mimeType, string fileName = null)
    {
        FileBase64 = fileBase64;
        MimeType = mimeType;
        FileName = fileName;
    }
}