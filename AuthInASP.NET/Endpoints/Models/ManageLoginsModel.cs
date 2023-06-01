using Microsoft.AspNetCore.Identity;

namespace AuthInASP.NET.Endpoints.Models
{
    public class ManageLoginsModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        //public IList<AuthenticationScheme> OtherLogins { get; set; }
    }
}
