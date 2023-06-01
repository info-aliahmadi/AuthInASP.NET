using AuthInASP.NET.Endpoints.Models;

namespace AuthInASP.NET.Service.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Result<List<PermissionModel>>> GetList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<PermissionModel>> GetById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionModel"></param>
        /// <returns></returns>
        Task<Result<PermissionModel>> Add(PermissionModel permissionModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionModel"></param>
        /// <returns></returns>
        Task<Result<PermissionModel>> Update(PermissionModel permissionModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> Delete(int id);


    }
}
