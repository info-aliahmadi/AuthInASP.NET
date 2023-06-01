using System.ComponentModel.DataAnnotations;

namespace AuthInASP.NET.Endpoints.Models
{
    public class ExternalLoginConfirmationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
