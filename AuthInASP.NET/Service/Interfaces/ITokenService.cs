using AuthInASP.NET.Domian;

namespace AuthInASP.NET.Service.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string CreateToken(User user, bool rememberMe = false);


    }
}
