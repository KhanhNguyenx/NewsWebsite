using NewsAPI.Data;
using System.Net;

namespace NewsAPI
{
    public class DI_AddService
    {
        public static void myServiceRegister(IServiceCollection Services)
        {
            Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        }
    }
}
