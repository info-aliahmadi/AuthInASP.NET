﻿using AuthInASP.NET.Endpoints.Handler;
using AuthInASP.NET.Extensions.Permission;
using AuthInASP.NET.Service;
using System.Security.Claims;

namespace AuthInASP.NET.Endpoints
{
    public static class AuthModule
    {
        private const string API_SCHEMA = "/Auth";

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {

            endpoints.MapGet(API_SCHEMA + "/initialize", AccountHandler.InitializeHandler).AllowAnonymous();

            endpoints.MapGet(API_SCHEMA + "/login", AccountHandler.LoginHandler).AllowAnonymous();
            endpoints.MapPost(API_SCHEMA + "/Register", AccountHandler.RegisterHandler).AllowAnonymous();
            endpoints.MapPost(API_SCHEMA + "/SignOut", AccountHandler.SignOutHandler).RequirePermission("AUTH_SIGNOUT");
            endpoints.MapPost(API_SCHEMA + "/ExternalLoginCallback", AccountHandler.ExternalLoginCallbackHandler).RequirePermission("AUTH_GET.EXTERNAL.LOGIN.CALLBACK");
            endpoints.MapGet(API_SCHEMA + "/ExternalLoginConfirmation", AccountHandler.ExternalLoginConfirmationHandler).RequirePermission("AUTH_EXTERNAL.LOGIN.CONFIRMATION");

            endpoints.MapGet(API_SCHEMA + "/ConfirmEmail", AccountHandler.ConfirmEmailHandler).RequirePermission("AUTH_CONFIRM.EMAIL");
            endpoints.MapPost(API_SCHEMA + "/ForgotPassword", AccountHandler.ForgotPasswordHandler).RequirePermission("AUTH_FORGOT.PASSWORD");
            endpoints.MapPost(API_SCHEMA + "/ResetPassword", AccountHandler.ResetPasswordHandler).RequirePermission("AUTH_RESET.PASSWORD");
            endpoints.MapGet(API_SCHEMA + "/GetTwoFactorProvidersAsync", AccountHandler.GetTwoFactorProvidersAsyncHandler).RequirePermission("AUTH_GET.TWO.FACTOR.PROVIDERS.ASYNC");
            endpoints.MapPost(API_SCHEMA + "/SendCode", AccountHandler.SendCodeHandler).RequirePermission("AUTH_SEND.CODE");
            endpoints.MapPost(API_SCHEMA + "/VerifyAuthenticatorCode", AccountHandler.VerifyAuthenticatorCodeHandler).RequirePermission("AUTH_VERIFY.AUTHENTICATOR.CODE");
            endpoints.MapPost(API_SCHEMA + "/VerifyCode", AccountHandler.VerifyCodeHandler).RequirePermission("AUTH_VERIFY.CODE");
            endpoints.MapPost(API_SCHEMA + "/UseRecoveryCode", AccountHandler.UseRecoveryCodeHandler).RequirePermission("AUTH_USE.RECOVERY.CODE");


            endpoints.MapGet(API_SCHEMA + "/AssignRoleToUserByRoleName", UserHandler.AssignRoleToUserByRoleName).RequirePermission("AUTH_ASSIGN.ROLE.TO.USER.BY.ROLE.NAME");
            endpoints.MapGet(API_SCHEMA + "/AssignRoleToUserByRoleId", UserHandler.AssignRoleToUserByRoleId).RequirePermission("AUTH_ASSIGN.ROLE.TO.USER.BY.ROLE.ID");

            endpoints.MapGet(API_SCHEMA + "/AssignPermissionToRoleByRoleName", RoleHandler.AssignPermissionToRoleByRoleName).RequirePermission("AUTH_ASSIGN.PERMISSION.TO.ROLE.BY.ROLE.NAME");
            endpoints.MapGet(API_SCHEMA + "/AssignPermissionToRoleByRoleId", RoleHandler.AssignPermissionToRoleByRoleId).RequirePermission("AUTH_ASSIGN.PERMISSION.TO.ROLE.BY.ROLE.ID");


            endpoints.MapGet(API_SCHEMA + "/GetRoleList", RoleHandler.GetList).RequirePermission("AUTH_GET.ROLE.LIST");
            endpoints.MapGet(API_SCHEMA + "/GetRoleById", RoleHandler.GetRoleById).RequirePermission("AUTH_GET.ROLE.BY.ID");
            endpoints.MapPost(API_SCHEMA + "/AddRole", RoleHandler.AddRole).RequirePermission("AUTH_ADD.ROLE");
            endpoints.MapPost(API_SCHEMA + "/UpdateRole", RoleHandler.UpdateRole).RequirePermission("AUTH_UPDATE.ROLE");
            endpoints.MapGet(API_SCHEMA + "/DeleteRole", RoleHandler.DeleteRole).RequirePermission("AUTH_DELETE.ROLE");


            endpoints.MapGet(API_SCHEMA + "/GetPermissionList", PermissionHandler.GetList).RequirePermission("AUTH_GET.PERMISSION.LIST");
            endpoints.MapGet(API_SCHEMA + "/GetPermissionById", PermissionHandler.GetById).RequirePermission("AUTH_GET.PERMISSION.BY.ID");
            endpoints.MapPost(API_SCHEMA + "/AddPermission", PermissionHandler.AddPermission).RequirePermission("AUTH_ADD.PERMISSION");
            endpoints.MapPost(API_SCHEMA + "/UpdatePermission", PermissionHandler.UpdatePermission).RequirePermission("AUTH_UPDATE.PERMISSION");
            endpoints.MapGet(API_SCHEMA + "/DeletePermission", PermissionHandler.DeletePermission).RequirePermission("AUTH_DELETE.PERMISSION");


            endpoints.MapGet(API_SCHEMA + "/username", async (ClaimsPrincipal user, HttpContext context) =>
            {
                return user.Identity.Name;

            }).AllowAnonymous();


            return endpoints;
        }

    }
}
