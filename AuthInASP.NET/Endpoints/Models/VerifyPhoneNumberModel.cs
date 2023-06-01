﻿using System.ComponentModel.DataAnnotations;

namespace AuthInASP.NET.Endpoints.Models
{
    public class VerifyPhoneNumberModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
