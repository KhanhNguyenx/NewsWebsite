namespace NewsAPI.Services
{
    public interface IOTPService
    {
        string GenerateOTP();
    }
    public class OTPService : IOTPService
    {
        public string GenerateOTP()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
