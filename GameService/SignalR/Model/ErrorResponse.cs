namespace SignalR.Model
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
