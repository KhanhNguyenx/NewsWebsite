//using NewsAPI.Data;
//using NewsAPI.Models;
//using System;

//namespace NewsAPI.Services.SimpleService
//{
//    public interface IUserService
//    {
//        Account Authenticate(string username, string password);
//        // Các phương thức khác như Register, GetById, v.v.
//    }
//    public class UserService : IUserService
//    {
//        private readonly NewsWebDbContext _context; // Inject your DbContext

//        public UserService(NewsWebDbContext context)
//        {
//            _context = context; // Initialize the DbContext
//        }

//        public Account ValidateUser(string email, string password)
//        {
//            // Find user by email
//            var user = _context.Users.SingleOrDefault(u => u.Email == email);

//            if (user == null)
//                return null; // User not found

//            // Validate the password (hash comparison if the password is stored hashed)
//            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
//            return isPasswordValid ? user : null;
//        }
//    }

//}
