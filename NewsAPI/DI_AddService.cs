using NewsAPI.Data;
using NewsAPI.Services;
using System.Net;

namespace NewsAPI
{
    public class DI_AddService
    {
        public static void myServiceRegister(IServiceCollection Services)
        {

            Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            Services.AddScoped<IAppSettingService, AppSettingService>();
            //Services.AddTransient<IOTPService, IOTPService>();
            Services.AddScoped(typeof(IGenericServive<>), typeof(GenericService<>));
        }
    }
}
