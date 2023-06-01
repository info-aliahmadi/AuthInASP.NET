﻿using AuthInASP.NET.Domian;
using AuthInASP.NET.Endpoints.Models;
using AuthInASP.NET.Infrastructure.Repository;
using AuthInASP.NET.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthInASP.NET.Service
{
    public class RoleService : IRoleService
    {
        private readonly IQueryRepository _queryRepository;
        private readonly ICommandRepository _commandRepository;

        public RoleService(IQueryRepository queryRepository, ICommandRepository commandRepository)
        {
            _queryRepository = queryRepository;
            _commandRepository = commandRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<RoleModel>>> GetList()
        {
            var result = new Result<List<RoleModel>>();

            var list = await _queryRepository.Table<Role>().ToListAsync();

            result.DataResult = list.Select(x => new RoleModel()
            {
                Id = x.Id,
                Name = x.Name,
                ConcurrencyStamp = x.ConcurrencyStamp,
                NormalizedName = x.NormalizedName
            }).ToList();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<RoleModel>> GetById(int id)
        {
            var result = new Result<RoleModel>();

            var record = await _queryRepository.Table<Role>().Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            var role = new RoleModel();
            if (record != null)
            {
                role.Id = record!.Id;
                role.Name = record.Name;
                role.ConcurrencyStamp = record.ConcurrencyStamp;
                role.NormalizedName = record.NormalizedName;
            }
            result.DataResult = role;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        public async Task<Result<RoleModel>> Add(RoleModel roleModel)
        {
            var result = new Result<RoleModel>();

            var isExist = await _queryRepository.Table<Role>().AnyAsync(x => x.Name == roleModel.Name || x.NormalizedName == roleModel.NormalizedName && x.NormalizedName != null);
            if (isExist)
            {
                result.Status = ResultStatusEnum.ItsDuplicate;
                result.Message = "The role existed";
                return result;
            }

            var role = new Role()
            {
                Name = roleModel.Name,
                ConcurrencyStamp = roleModel.ConcurrencyStamp,
                NormalizedName = roleModel.NormalizedName
            };
            await _commandRepository.InsertAsync(role);
            await _commandRepository.SaveChangesAsync();

            roleModel.Id = role.Id;
            result.DataResult = roleModel;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Result> AssignPermissionToRoleAsync(int permissionId, string roleName)
        {
            var result = new Result();

            var permission = await _queryRepository.Table<Permission>().FirstOrDefaultAsync(x => x.Id == permissionId);
            if (permission is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The permission Not Found";
                return result;
            }

            var role = await _queryRepository.Table<Role>().FirstOrDefaultAsync(x => x.Name == roleName);
            if (role is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The role Not Found";
                return result;
            }

            return await AssignPermissionToRoleAsync(permissionId, role.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<Result> AssignPermissionToRoleAsync(string permissionName, string roleName)
        {
            var result = new Result();

            var permission = await _queryRepository.Table<Permission>().FirstOrDefaultAsync(x => x.Name == permissionName);
            if (permission is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The permission Not Found";
                return result;
            }

            var role = await _queryRepository.Table<Role>().FirstOrDefaultAsync(x => x.Name == roleName);
            if (role is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The role Not Found";
                return result;
            }

            return await AssignPermissionToRoleAsync(permission.Id, role.Id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Result> AssignPermissionToRoleAsync(int permissionId, int roleId)
        {
            var result = new Result();

            var isExist = await _queryRepository.Table<PermissionRole>().AnyAsync(x => x.PermissionId == permissionId && x.RoleId == roleId);
            if (isExist)
            {
                result.Status = ResultStatusEnum.ItsDuplicate;
                result.Message = "The role and permission assigned already";
                return result;
            }

            var permissionRole = new PermissionRole()
            {
                PermissionId = permissionId,
                RoleId = roleId
            };
            await _commandRepository.InsertAsync(permissionRole);

            await _commandRepository.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        public async Task<Result<RoleModel>> Update(RoleModel roleModel)
        {
            var result = new Result<RoleModel>();
            var role = await _queryRepository.Table<Role>().FirstOrDefaultAsync(x => x.Id == roleModel.Id);
            if (role is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The role not found";
                return result;
            }

            role.Name = roleModel.Name;
            role.ConcurrencyStamp = roleModel.ConcurrencyStamp;
            role.NormalizedName = roleModel.NormalizedName;

            _commandRepository.UpdateAsync(role);

            await _commandRepository.SaveChangesAsync();

            result.DataResult = roleModel;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Result> Delete(int id)
        {
            var result = new Result();
            var role = _queryRepository.Table<Role>().FirstOrDefaultAsync(x => x.Id == id);
            if (role is null)
            {
                result.Status = ResultStatusEnum.NotFound;
                result.Message = "The role not found";
                return result;
            }

            var havePermission = await _queryRepository.Table<PermissionRole>().AnyAsync(x => x.RoleId == id);
            if (havePermission)
            {
                result.Status = ResultStatusEnum.IsNotAllowed;
                result.Message = "Is Not Allowed. because this role have permission";
                return result;
            }

            var haveUser = await _queryRepository.Table<UserRole>().AnyAsync(x => x.RoleId == id);
            if (haveUser)
            {
                result.Status = ResultStatusEnum.IsNotAllowed;
                result.Message = "Is Not Allowed. because this role have User";
                return result;
            }

            _commandRepository.DeleteAsync(role);

            await _commandRepository.SaveChangesAsync();

            return result;
        }
    }
}