namespace NewsAPI.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
}
