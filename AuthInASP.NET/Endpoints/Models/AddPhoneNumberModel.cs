using System.ComponentModel.DataAnnotations;

namespace AuthInASP.NET.Endpoints.Models
{
    public class AddPhoneNumberModel
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
