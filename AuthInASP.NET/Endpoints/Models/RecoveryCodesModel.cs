using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthInASP.NET.Endpoints.Models
{
    public class RecoveryCodesModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }
    }
}
