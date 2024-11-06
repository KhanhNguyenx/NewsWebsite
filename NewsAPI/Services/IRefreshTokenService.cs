using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.Models;
using System.Security.Cryptography;

namespace NewsAPI.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(User account);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
    }
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly NewsWebDbContext _dbContext;
        public RefreshTokenService(NewsWebDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
        }
        public async Task<RefreshToken> GenerateRefreshTokenAsync(User account)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                Created = DateTime.UtcNow,
                UserId = account.Id
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            return refreshToken;
        }

        // Get a refresh token by its value
        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(a => a.RoleUsers)
                .ThenInclude(ru => ru.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        // Revoke a refresh token
        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            if (refreshToken == null || refreshToken.IsExpired)
            {
                return false;
            }

            refreshToken.Revoked = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
