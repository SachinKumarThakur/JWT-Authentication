using System.ComponentModel.DataAnnotations;

namespace OrganizationAPI.ViewModel
{
    public class SignInViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
