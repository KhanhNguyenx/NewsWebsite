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
}
