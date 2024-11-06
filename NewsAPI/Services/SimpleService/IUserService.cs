using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewsAPI.Data;
using NewsAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace NewsAPI.Services.SimpleService
{
    public interface IUserService :IService<User>
    {
        Task<User> GetAccountByUsernameAsync(string username);
        Task<string> GenerateAccessTokenAsync(User account);
        Task<bool> VerifyPasswordAsync(User account, string password);
        // Các phương thức khác như Register, GetById, v.v.
    }
    public class UserService  : IUserService
    {
        private readonly NewsWebDbContext _dbConTect;
        private readonly IConfiguration _config;
        public UserService(NewsWebDbContext dbConTect, IConfiguration config)
        {
            _dbConTect = dbConTect;
            _config = config;
        }
        public async Task<User> GetAsync(int id, bool useDTO = true)
        {
            if (useDTO)
                return await _dbConTect.Users.FindAsync(id);
            else
            {
                var entity = await _dbConTect.Users
                                .FirstOrDefaultAsync(a => a.Id == id);
                return entity!;
            }
        }

        public async Task<IEnumerable<User>> GetListAsync()
        {
            return await _dbConTect.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchAsync(Expression<Func<User, bool>> expression, bool useDTO = true)
        {
            if (useDTO)
                return await _dbConTect.Users.Where(expression).ToListAsync();
            else
            {
                var list = await _dbConTect.Users
                        .Where(expression)
                        .Select(a => new User
                        {
                            Id = a.Id,
                            Username = a.Username,
                            Email = a.Email,
                            Status = a.Status,

                        })
                        .ToListAsync();
                return list;
            }
        }

        public async Task<User> CreateAsync(User entity)
        {
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
            _dbConTect.Users.Add(entity);
            if (await _dbConTect.SaveChangesAsync() > 0)
                return entity;

            return null!;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            // Kiểm tra tài khoản có tồn tại
            if (!CheckExists(entity.Id))
            {
                return null!;
            }

            // Nếu người dùng muốn thay đổi mật khẩu, hash mật khẩu mới
            if (!string.IsNullOrEmpty(entity.PasswordHash))
            {
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
            }

            // Cập nhật các thông tin khác
            _dbConTect.Entry(entity).State = EntityState.Modified;

            // Lưu thay đổi vào cơ sở dữ liệu
            if (await _dbConTect.SaveChangesAsync() > 0)
            {
                return entity;
            }
            return null!;
        }

        public int Delete(int id)
        {
            var entity = _dbConTect.Users.Find(id);
            if (entity != null)
            {
                _dbConTect.Remove(entity);
                if (_dbConTect.SaveChanges() > 0)
                    return 1;
            }
            return 0;
        }
        public bool CheckExists(int id)
        {
            return _dbConTect.Users.Any(e => e.Id == id);
        }

        public async Task<int> MaxIdAsync(int id)
        {
            return await _dbConTect.Users.AnyAsync() ? await _dbConTect.Users.MaxAsync(x => x.Id) : 0;
        }

        public async Task<int> MinIdAsync(int id)
        {
            return await _dbConTect.Users.MinAsync(x => x.Id);
        }

        // Lấy tài khoản theo tên người dùng
        public async Task<User> GetAccountByUsernameAsync(string username)
        {
            return await _dbConTect.Users
                .Include(a => a.RoleUsers) // Bao gồm thông tin về RoleUsers
                .ThenInclude(ru => ru.Role) // Bao gồm Role liên kết với RoleUsers
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        // Kiểm tra mật khẩu (dùng plain-text, nên được cải thiện sau này)
        public async Task<bool> VerifyPasswordAsync(User account, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
        }

        public async Task<string> GenerateAccessTokenAsync(User account)
        {
            var roles = account.RoleUsers.Select(ru => ru.Role.Name).ToList();

            var authoClaims = new List<Claim>
            {
                 new Claim("Id", account.Id.ToString()),
                 new Claim(ClaimTypes.Name, account.Username),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Email, account.Email),
            };

            // Thêm vai trò của người dùng vào claim
            foreach (var role in roles)
            {
                authoClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = /*DataEncryption.Decrypt(*/jwtSettings["SecretKey"]/*)*/;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var lengthbyte = secretKeyBytes.Length;
            var securityKey = new SymmetricSecurityKey(secretKeyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: authoClaims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);  //accessToken  
        }
    }
}

