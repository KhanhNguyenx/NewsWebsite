namespace NewsWebsite.Helpers
{
    public class ResponseData
    {
        public bool success { get; set; } = false;
        public string message { get; set; } = string.Empty;
        public long time { get; set; }
        public dynamic data { get; set; } = null!;

        //public int statusCode { get; set; }
        public ResponseData()
        {
            success = false;
            message = "Response is failed or empty";
        }

        public ResponseData(dynamic data)
        {
            this.data = data;
        }

        public ResponseData(bool success, dynamic data)
        {
            this.success = success;
            this.data = data;
            message = "";
        }

        public ResponseData(bool success, string message, dynamic data)
        {
            this.success = success;
            this.message = message;
            this.data = data;
        }
    }
}
