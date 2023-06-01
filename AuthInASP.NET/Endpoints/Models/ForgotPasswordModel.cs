using System.ComponentModel.DataAnnotations;

namespace AuthInASP.NET.Endpoints.Models
{
    public record ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
