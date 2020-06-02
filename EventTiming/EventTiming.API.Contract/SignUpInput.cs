using System.ComponentModel.DataAnnotations;

namespace EventTiming.API.Contract
{
    public class SignUpInput
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
