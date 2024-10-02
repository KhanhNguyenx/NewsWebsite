using NewsAPI.Models;

namespace NewsAPI.Services
{
    public interface IAppSettingService
    {
        string GetValuesSetting(string key);
        GoogleSetting GetGoogleSetting(string key);
    }
    public class AppSettingService : IAppSettingService
    {
        private readonly IConfiguration _configuration;
        public AppSettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetValuesSetting(string key)
        {
            return _configuration[key];
        }
        public GoogleSetting GetGoogleSetting(string key)
        {
            var googleObject = new GoogleSetting();
            _configuration.GetSection(key).Bind(googleObject);
            return googleObject;
        }
    }
}
