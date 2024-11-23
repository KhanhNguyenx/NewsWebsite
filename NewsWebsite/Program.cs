using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsWebsite.Helpers;
using NewsAPI.Services;
using NewsAPI.Services.SimpleService;
using NewsWebsite;
using NewsWebsite.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
var getConnectionStr = builder.Configuration.GetConnectionString("MyConnectString");
builder.Services.AddDbContext<NewsWebDbContext>(option => option.UseSqlServer(getConnectionStr));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped(typeof(IGenericServive<>), typeof(GenericService<>));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IConsumeApi, ConsumeApi>();

// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie = new CookieBuilder
    {
        //Domain = "domain.vn", //Releases in active
        Name = "Authentication",
        HttpOnly = true,
        Path = "/",
        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.Always
    };
    options.LoginPath = new PathString("/Authorize/Login");
    options.LogoutPath = new PathString("/Authorize/Logout");
    options.AccessDeniedPath = new PathString("/Error/403");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddSingleton<IConsumeApi, ConsumeApi>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); //Init auto mappper

builder.Services.AddSession(options =>
{
    //options.Cookie.Domain = "domain"; //Releases in active
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddHttpClient();

// Add services to the container.
//builder.Services.AddControllersWithViews();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CheckCookieExpirationMiddleware>();
app.UseSession();

app.MapControllerRoute(
    name: "PostDetails",
    pattern: "{slug}.html",
    defaults: new { controller = "Posts", action = "Details" },
    constraints: new { slug = @"[\w\-]+" } // Ràng buộc chỉ chấp nhận các ký tự, số và dấu gạch ngang
);

app.MapControllerRoute(
    name: "PostSlug",
    pattern: "posts/{slug}",
    defaults: new { controller = "Posts", action = "Details" }
);

app.MapControllerRoute(
    name: "PostsByCategory",
    pattern: "Category/{id}",
    defaults: new { controller = "Posts", action = "PostsByCategory" },
    constraints: new { id = @"\d+" } // Ràng buộc chỉ chấp nhận số
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();
