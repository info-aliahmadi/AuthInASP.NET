﻿namespace AuthInASP.NET.Domian
{
    public class PermissionRole
    {
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
}
