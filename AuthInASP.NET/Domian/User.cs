﻿using Microsoft.AspNetCore.Identity;

namespace AuthInASP.NET.Domian
{
    public class User : IdentityUser<int>
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public DateTime DOB { get; set; }
    }
}
