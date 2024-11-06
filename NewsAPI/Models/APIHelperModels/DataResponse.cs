namespace NewsAPI.Models.APIHelperModels
{
    public class DataResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public long Time { get; set; }
        public dynamic Data { get; set; } = null!;

        public DataResponse()
        {
            Success = false;
            Message = "Response is failed or empty";
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public DataResponse(dynamic data)
        {
            Data = data;
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public DataResponse(bool success, dynamic data)
        {
            Success = success;
            Data = data;
            Message = "";
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public DataResponse(bool success, dynamic data, int totalPages)
        {
            Success = success;
            Data = data;
            Message = "";
            Time = totalPages;
        }

        public DataResponse(bool success, string message, dynamic data)
        {
            Success = success;
            Message = message;
            Data = data;
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
