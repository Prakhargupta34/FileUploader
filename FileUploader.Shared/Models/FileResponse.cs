namespace FileUploader.Shared.Models;

public class FileResponse
{
    public string FileName { get; set; }
    public Stream ResponseStream { get; set; }
    public string ContentType { get; set; }
}