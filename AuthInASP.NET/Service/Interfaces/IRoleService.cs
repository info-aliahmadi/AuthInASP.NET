﻿

using AuthInASP.NET.Endpoints.Models;

namespace AuthInASP.NET.Service.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Result<List<RoleModel>>> GetList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<RoleModel>> GetById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        Task<Result<RoleModel>> Add(RoleModel roleModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<Result> AssignPermissionToRoleAsync(int permissionId, string roleName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<Result> AssignPermissionToRoleAsync(string permissionName, string roleName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Result> AssignPermissionToRoleAsync(int permissionId, int roleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        Task<Result<RoleModel>> Update(RoleModel roleModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> Delete(int id);


    }
}
