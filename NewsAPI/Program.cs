using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsAPI.Data;
using NewsAPI.Helpers;
using NewsAPI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DI_AddService.MyServiceRegister(builder);
DI_AddService.myServiceRegister(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts(); //Browser only use Https
app.UseRouting();

//CORS single
app.UseCors("Policy1");
//CORS all
app.UseCors();
//app.UseCors("AllowAll");
app.UseRateLimiter();
app.MapControllers();
//app.UseMiddleware<AntiXSSMiddleware>();
//ConfigAuthen
app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<JwtExpirationMiddleware>();

app.MapControllers();

app.Run();
