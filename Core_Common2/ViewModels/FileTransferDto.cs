namespace Core_Common2.ViewModels
{
    public class FileTransferDto
    {
        public string FileName { get; set; }

        public string FileContent { get; set; } 

        public string FileUrl { get; set; }
    }

    public class FileTransferViewModel
    {
        public string FileName { get; set; }

        public string FileContent { get; set; }

        public Stream FileStream { get; set; }
    }
}
