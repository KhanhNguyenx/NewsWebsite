using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsAPI.Data;
using NewsAPI.Services;
using System.Net;
using System.Text;
using EncrypDecryp;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace NewsAPI.Helpers
{
    public class DI_AddService
    {
        public static void MyServiceRegister(WebApplicationBuilder builder)
        {
            Decryption decryption = new Decryption();
            //StringConnect
            // Get the encrypted connection string from appsettings.json
            string encryptedConnectionString = builder.Configuration.GetConnectionString("MyConnectString");
            // Decrypt the connection string
            string decryptedConnectionString = decryption.Decrypt(encryptedConnectionString);
            builder.Services.AddDbContext<NewsWebDbContext>(option => option.UseSqlServer(decryptedConnectionString));

            //JwtSetting
            var secretkey = /*decryption.Decrypt(*/builder.Configuration["JwtSettings:SecretKey"]/*)*/;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretkey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ClockSkew = TimeSpan.FromHours(1),
                };
            }).AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = decryption.Decrypt(builder.Configuration["Google:ClientId"]);
                googleOptions.ClientSecret = decryption.Decrypt(builder.Configuration["Google:ClientSecret"]);
                googleOptions.CallbackPath = "/api/auth/google-callback";
            });


            //Roles After Authentication
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin")); // Nếu cả User và Admin đều được phép
            });


            //CORS Single
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("Policy1",
                    policy =>
                    {
                        policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
                    });
            });

            //CORS ALl
            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
                    });
            });


            //Rate Limited
            builder.Services.AddMemoryCache();
            builder.Services.AddRateLimiter(option =>
            {
                option.AddFixedWindowLimiter(policyName: "Fixed", opt =>
                {
                    opt.PermitLimit = 5; //limited 5 request
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2; //Limit number of request in queue
                });
            });

            //RateLimitedAll
            //builder.Services.AddMemoryCache();
            //builder.Services.AddRateLimiter(option =>
            //{
            //    option.RejectionStatusCode = 429;
            //    option.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(HttpContext => RateLimitPartition.GetFixedWindowLimiter(partitionKey: HttpContext.Request.Headers.Host.ToString(),
            //        factory: partition => new FixedWindowRateLimiterOptions
            //        {
            //            AutoReplenishment = true,
            //            PermitLimit = 10,
            //            QueueLimit = 0,
            //            Window = TimeSpan.FromMinutes(1)
            //        }));
            //});

        }
        public static void myServiceRegister(IServiceCollection Services)
        {

            Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            Services.AddScoped<IAppSettingService, AppSettingService>();
            //Services.AddTransient<IOTPService, IOTPService>();
            Services.AddScoped(typeof(IGenericServive<>), typeof(GenericService<>));
            Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
        }
    }
}
