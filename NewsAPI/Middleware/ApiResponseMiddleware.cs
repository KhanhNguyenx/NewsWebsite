//using System.Text.Json;

//namespace NewsAPI.Middleware
//{
//    public class ApiResponseMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public ApiResponseMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }
//        public async Task InvokeAsync(HttpContext context)
//        {
//            var originalBodyStream = context.Response.Body;

//            try
//            {
//                using (var responseBody = new MemoryStream())
//                {
//                    context.Response.Body = responseBody;
//                    await _next(context);

//                    context.Response.Body = originalBodyStream;

//                    var statusCode = context.Response.StatusCode;
//                    var isSuccess = statusCode >= 200 && statusCode < 300;
//                    string message = isSuccess ? "Request successful" : "Request failed";

//                    responseBody.Seek(0, SeekOrigin.Begin);
//                    var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
//                    var responseData = string.IsNullOrWhiteSpace(responseBodyText) ? null : JsonSerializer.Deserialize<object>(responseBodyText);
//                    var apiResponse = new ResponseAPI<object>(isSuccess, message, responseData);
//                    var jsonResponse = JsonSerializer.Serialize(apiResponse);
//                    context.Response.ContentType = "application/json";
//                    await context.Response.WriteAsync(jsonResponse);
//                }
//            }
//            catch (Exception ex)
//            {
//                var errorResponse = new ResponseAPI<string>(false, ex.Message, null);
//                var jsonResponse = JsonSerializer.Serialize(errorResponse);
//                context.Response.ContentType = "application/json";
//                context.Response.StatusCode = 500;
//                await context.Response.WriteAsync(jsonResponse);
//            }
//        }
//    }
//}
