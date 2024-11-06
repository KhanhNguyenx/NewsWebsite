using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "*")]
        //[RegularExpression(@"^[a-zA-Z0-9_]{3,10}$", ErrorMessage = "*")]
        public string Username { get; set; } = null!;


        [Required(ErrorMessage = "*")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.[A-Z])(?=.*\d)(?=.[\W])[\S]{8,}$", ErrorMessage = "*")]
        public string PasswordHash { get; set; } = null!;
    }
    public class RegisterModel
    {
        [Required(ErrorMessage = "*")]
        [MaxLength(20)]
        //[RegularExpression(@"^[a-zA-Z0-9_]{3,10}$", ErrorMessage = "*")]
        public string Username { get; set; }
        [Required(ErrorMessage = "*")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.[A-Z])(?=.*\d)(?=.[\W])[\S]{8,}$", ErrorMessage = "*")]
        public string PasswordHash { get; set; }
        
        public string Email { get; set; }
        [MaxLength(30)]
        public string Fullname { get; set; }
        public bool IsAuthor { get; set; }= false;
        public int Status { get; set; } = 1;
    }
}
