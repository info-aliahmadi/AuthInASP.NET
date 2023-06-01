namespace AuthInASP.NET.Service.Interfaces
{
    public interface IPermissionChecker
    {
        bool IsAuthorized(int userId, string permissionName);
    }
}
