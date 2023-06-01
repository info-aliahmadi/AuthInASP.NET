
using AuthInASP.NET.Domian;

namespace AuthInASP.NET.Endpoints.Models
{
    public class UserPermissionModel
    {
        public User User { get; set; }

        public IList<RoleModel> Roles { get; set; }

        public IList<PermissionModel> Permissions { get; set; }
    }
}
