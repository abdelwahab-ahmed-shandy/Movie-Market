namespace MovieMart.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? OriginalPath { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
