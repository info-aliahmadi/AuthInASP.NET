﻿using AuthInASP.NET;
using AuthInASP.NET.Domian;
using AuthInASP.NET.Endpoints.Models;
using AuthInASP.NET.Infrastructure;
using AuthInASP.NET.Infrastructure.Repository;
using AuthInASP.NET.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniValidation;
using System.Security.Claims;

namespace AuthInASP.NET.Endpoints.Handler
{
    public class AccountHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_repository"></param>
        /// <param name="_emailSender"></param>
        /// <param name="_userManager"></param>
        /// <param name="_roleManager"></param>
        /// <param name="_signInManager"></param>
        /// <param name="_sharedlocalizer"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        public static async Task<IResult> InitializeHandler(
            IQueryRepository _repository,
            UserManager<User> _userManager,
            RoleManager<Role> _roleManager,
            SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
            ILogger<AccountHandler> _logger
            )
        {
            try
            {
                var result = new AccountResult();

                var user = new User
                { DOB = DateTime.Now, Name = "admin", UserName = "admin", Email = "admin@admin.com" };


                if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                    await _roleManager.CreateAsync(new Role() { Name = "SuperAdmin" });


                var isExist = _repository.Table<User>().Any(x => x.UserName == "admin");
                if (!isExist)
                {
                    var identityResult = await _userManager.CreateAsync(user, "admin");
                    await _userManager.AddToRoleAsync(user, "SuperAdmin");

                    if (identityResult.Succeeded)
                    {
                        return Results.Ok(result);
                    }
                    else
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            _logger.LogError(_sharedlocalizer["{0}; Requested By: {1}"], error.Description,
                                "admin@admin.com");
                            result.Errors.Add(error.Description);
                        }

                        _logger.LogError(_sharedlocalizer["The user could not create a new account.; Requested By: {0}"],
                            "admin@admin.com");
                        result.Status = AccountStatusEnum.Failed;

                        return Results.BadRequest(result);

                    }
                }
                return Results.Ok(result);

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static async Task<IResult> RegisterHandler(
             IQueryRepository _repository,
             UserManager<User> _userManager,
             RoleManager<Role> _roleManager,
             SignInManager<User> _signInManager,
             IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger,
             [FromBody] RegisterModel registerModel
             )
        {
            try
            {
                var result = new AccountResult();

                var user = new User
                { DOB = DateTime.Now, Name = registerModel.Name, UserName = registerModel.UserName, Email = registerModel.Email };


                if (!await _roleManager.RoleExistsAsync("user"))
                    await _roleManager.CreateAsync(new Role() { Name = "user" });


                var isExist = _repository.Table<User>().Any(x => x.UserName == registerModel.UserName || x.Email == registerModel.Email);
                if (!isExist)
                {
                    var identityResult = await _userManager.CreateAsync(user, registerModel.Password);


                    if (identityResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        if (_userManager.Options.SignIn.RequireConfirmedEmail)
                        {
                            result.Status = AccountStatusEnum.RequireConfirmedEmail;
                            //var emailRequest = new EmailRequestRecord();
                            ////For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                            ////Send an email with this link
                            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            //var callbackUrl = string.Format("ConfirmEmail/Account/{0}/{1}/{2}", user.Id, code, "");

                            //emailRequest.Subject = _sharedlocalizer["ConfirmEmail"];
                            //emailRequest.Body =
                            //    string.Format(
                            //        _sharedlocalizer[
                            //            "Please confirm your account by clicking this link: <a href='{0}'>link</a>"],
                            //        callbackUrl);
                            //emailRequest.ToEmail = registerModel.Email;
                            //try
                            //{
                            //    await _emailSender.SendEmailAsync(emailRequest);

                            //    result.Status = AccountStatusEnum.Succeeded;
                            //    return Results.Ok(result);
                            //}
                            //catch (Exception e)
                            //{
                            //    _logger.LogError(e.InnerException + "_" + e.Message);
                            //    result.Errors.Add(string.Format(_sharedlocalizer["{0} action throws an error"]));
                            //    return Results.BadRequest(result);
                            //}
                        }
                    }
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            _logger.LogError(_sharedlocalizer["{0}; Requested By: {1}"], error.Description,
                                registerModel.Email);
                            result.Errors.Add(error.Description);
                        }

                        _logger.LogError(_sharedlocalizer["The user could not create a new account.; Requested By: {0}"],
                            registerModel.Email);
                        result.Status = AccountStatusEnum.Failed;

                        return Results.BadRequest(result);

                    }
                    else
                    {
                        return Results.Ok(result);
                    }
                }
                else
                {

                    _logger.LogError(_sharedlocalizer["The username or email already exist.; Requested By: {0}"],
                        registerModel.Email);
                    result.Status = AccountStatusEnum.Failed;

                    return Results.BadRequest(result);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenService"></param>
        /// <param name="_userManager"></param>
        /// <param name="_signInManager"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        public static async Task<IResult> LoginHandler(
            ITokenService tokenService,
            UserManager<User> _userManager,
            SignInManager<User> _signInManager,
            string userName,
            string password,
            bool rememberMe)
        {
            try
            {
                var result = new AccountResult();
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var user = await _userManager.FindByNameAsync(userName);

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, true);
                if (signInResult.Succeeded)
                {
                    var token = tokenService.CreateToken(user, rememberMe);
                    result.Status = AccountStatusEnum.Succeeded;


                    if (signInResult.RequiresTwoFactor)
                    {
                        result.Status = AccountStatusEnum.RequiresTwoFactor;
                        return Results.Ok(result);
                    }

                    if (signInResult.IsLockedOut)
                    {
                        result.Status = AccountStatusEnum.IsLockedOut;
                        return Results.Ok(result);
                    }

                    return Results.Ok(token);
                }
                else
                {
                    return Results.BadRequest("BadRequest");
                }

            }
            catch (Exception e)
            {
                return Results.BadRequest("BadRequest");
            }
        }

        public static async Task<IResult> SignOutHandler(
            SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger)
        {
            var result = new AccountResult();
            await _signInManager.SignOutAsync();
            _logger.LogInformation(_sharedlocalizer["The user logged out."]);
            result.Status = AccountStatusEnum.Succeeded;
            return Results.Ok(result);
        }


        public static IResult ExternalLogin(SignInManager<User> _signInManager, HttpContext context, string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.

            var redirectUrl = HydraHelper.GetCurrentDomain(context) + $"ExternalLoginCallback/Account?ReturnUrl={returnUrl}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Results.Challenge(properties);
        }


        public static async Task<IResult> ExternalLoginCallbackHandler(SignInManager<User> _signInManager,
            HttpContext context,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, string? returnUrl = null, string? remoteError = null)
        {
            var result = new AccountResult();
            if (remoteError != null)
            {
                result.Status = AccountStatusEnum.ErrorExternalProvider;
                _logger.LogError(_sharedlocalizer["Error from external provider: {0}"], remoteError);
                result.Errors.Add(string.Format(_sharedlocalizer["Error from external provider: {0}"], remoteError));
                return Results.BadRequest(result);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                result.Status = AccountStatusEnum.NullExternalLoginInfo;
                _logger.LogError(_sharedlocalizer["Could not get info from an external provider"]);
                result.Errors.Add(_sharedlocalizer["Could not get info from an external provider"]);
                return Results.BadRequest(result);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var externalLoginResult =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                    isPersistent: false);
            if (externalLoginResult.Succeeded)
            {
                // Update any authentication tokens if login succeeded
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                _logger.LogInformation(_sharedlocalizer["User logged in with {0} provider."], info.LoginProvider);
                result.Status = AccountStatusEnum.Succeeded;
                return Results.Ok(result);
            }

            if (externalLoginResult.RequiresTwoFactor)
            {
                result.Status = AccountStatusEnum.RequiresTwoFactor;
                return Results.Ok(result);
            }

            if (externalLoginResult.IsLockedOut)
            {
                result.Status = AccountStatusEnum.IsLockedOut;
                return Results.Ok(result);
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.e;

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                _logger.LogWarning(_sharedlocalizer["Redirect the user {0} to ExternalLoginConfirmation"], email);

                var redirectUrl = HydraHelper.GetCurrentDomain(context) + $"ExternalLoginConfirmation/Account?email={email}";

                return Results.Redirect(redirectUrl, true);
            }
        }

        public static async Task<IResult> ExternalLoginConfirmationHandler(UserManager<User> _userManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, SignInManager<User> _signInManager, [FromBody] ExternalLoginConfirmationModel model)
        {
            var result = new AccountResult();
            ;
            if (MiniValidator.TryValidate(model, out var errors))
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    result.Status = AccountStatusEnum.ExternalLoginFailure;
                    _logger.LogError(_sharedlocalizer["External Login Failure for user: {0}"], model.Email);
                    return Results.BadRequest(result);
                }

                var user = new User { UserName = model.Email, Email = model.Email };
                var userManagerResult = await _userManager.CreateAsync(user);
                if (userManagerResult.Succeeded)
                {
                    userManagerResult = await _userManager.AddLoginAsync(user, info);
                    if (userManagerResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6,
                            _sharedlocalizer["User created an account using {0} provider; Requested By: {1}"], info.LoginProvider,
                            model.Email);
                        result.Status = AccountStatusEnum.Succeeded;
                        // Update any authentication tokens as well
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        return Results.Ok(result);
                    }
                }

                _logger.LogError(_sharedlocalizer["The user could not create a new account after external login.;"] + model.Email);
                result.Status = AccountStatusEnum.Failed;
                foreach (var error in userManagerResult.Errors)
                {
                    _logger.LogError(error.Description + "; Requested By: " + model.Email);
                    result.Errors.Add(error.Description);
                }

                return Results.BadRequest(result);
            }

            _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: {0} "], model.Email);

            return Results.BadRequest(errors);
        }


        public static async Task<IResult> ConfirmEmailHandler(UserManager<User> _userManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, string userId, string code, string returnUrl)
        {
            if (userId == null || code == null)
            {
                _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + userId);
                return Results.Redirect(returnUrl + "&status=error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + userId);
                return Results.Redirect(returnUrl + "&status=error");
            }
            _logger.LogInformation(_sharedlocalizer["User confirmed the code.; Requested By: {0}"], userId);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return Results.Redirect(returnUrl + "&status" + (result.Succeeded ? "succeeded" : "error"));
        }


        public static async Task<IResult> ForgotPasswordHandler(UserManager<User> _userManager,
            HttpContext context,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, [FromBody] ForgotPasswordModel model)
        {
            var result = new AccountResult();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    result.Status = AccountStatusEnum.Failed;
                    return Results.BadRequest(result);
                }

                result.Status = AccountStatusEnum.RequireConfirmedEmail;

                //var emailRequest = new EmailRequestRecord();
                ////For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                ////Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var callbackUrl = HydraHelper.GetCurrentDomain(context) + $"ConfirmEmail/Account?userId={user.Id}&code={code}&returnUrl=";

                //emailRequest.Subject = _sharedlocalizer["ConfirmEmail"];
                //emailRequest.Body = $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>";
                //emailRequest.ToEmail = model.Email;
                //try
                //{
                //    await _emailSender.SendEmailAsync(emailRequest);

                //    result.Status = AccountStatusEnum.Succeeded;
                //    return Results.Ok(result);
                //}
                //catch (Exception e)
                //{
                //    _logger.LogError(e.InnerException + "_" + e.Message);
                //    result.Errors.Add(_sharedlocalizer["RequireConfirmedEmail action throw an error"]);
                //    return Results.BadRequest(result);
                //}


            }

            // If we got this far, something failed, redisplay form
            _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + model.Email);

            return Results.BadRequest(errors);
        }


        public static async Task<IResult> ResetPasswordHandler(UserManager<User> _userManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, [FromBody] ResetPasswordModel model)
        {
            var result = new AccountResult();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + model.Email);

                return Results.BadRequest(errors);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + model.Email);
                result.Status = AccountStatusEnum.Failed;
                return Results.BadRequest(result);
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (resetPasswordResult.Succeeded)
            {
                result.Status = AccountStatusEnum.Succeeded;
                return Results.Ok(result);
            }

            _logger.LogError(_sharedlocalizer["{0} action does not Succeeded"], "ResetPasswordAsync");
            result.Status = AccountStatusEnum.Failed;
            result.Errors.Add(string.Format(_sharedlocalizer["{0} action does not Succeeded"], "ResetPasswordAsync"));
            return Results.BadRequest(result);
        }


        public static async Task<IResult> GetTwoFactorProvidersAsyncHandler(UserManager<User> _userManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, SignInManager<User> _signInManager)
        {
            var result = new AccountResult();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogWarning(_sharedlocalizer["Input data are invalid.; Requested By: "] + user.Email);
                result.Status = AccountStatusEnum.Failed;
                return Results.BadRequest(result);
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
                .ToList();
            return Results.Ok(new SendCodeModel
            { Providers = factorOptions });
        }


        public static async Task<IResult> SendCodeHandler(UserManager<User> _userManager, SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, [FromBody] SendCodeModel model)
        {
            var result = new AccountResult();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                _logger.LogError(_sharedlocalizer["Input data are invalid.; Requested By: "] + user ?? user.Email);
                result.Status = AccountStatusEnum.Invalid;
                result.Errors.Add("");

                return Results.BadRequest(result);
            }

            if (user == null)
            {
                _logger.LogError(_sharedlocalizer["User not found.;"]);
                result.Status = AccountStatusEnum.Failed;
                result.Errors.Add(_sharedlocalizer["User not found.; "]);

                return Results.NotFound(result);
            }

            if (model.SelectedProvider == "Authenticator")
            {
                // The following code protects for brute force attacks against the two factor codes.
                // If a user enters incorrect codes for a specified amount of time then the user account
                // will be locked out for a specified amount of time.
                result.Status = AccountStatusEnum.RequiresTwoFactor;
                return Results.Ok(result);
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogError(_sharedlocalizer["Token generator returned null.; Requested By: {0}"], user.Email);
                result.Status = AccountStatusEnum.Failed;
                result.Errors.Add(string.Format(_sharedlocalizer["Token generator returned null.; Requested By: {0}"], user.Email));

                return Results.BadRequest(result);
            }

            var message = _sharedlocalizer["Your security code is: "] + code;
            if (model.SelectedProvider == "Email")
            {
                //var emailRequest = new EmailRequestRecord()
                //{
                //    ToEmail = await _userManager.GetEmailAsync(user),
                //    Subject = _sharedlocalizer["Security Code"],
                //    Body = message
                //};

                //try
                //{
                //    await _emailSender.SendEmailAsync(emailRequest);

                //    result.Status = AccountStatusEnum.Succeeded;
                //    return Results.Ok(result);
                //}
                //catch (Exception e)
                //{
                //    _logger.LogError(e.InnerException + "_" + e.Message);
                //    result.Errors.Add(_sharedlocalizer["Send email action throws an error"]);
                //    return Results.BadRequest(result);
                //}
            }
            else if (model.SelectedProvider == "Phone")
            {
                //var smsRequest = new SmsRequestRecord()
                //{
                //    ToNumber = await _userManager.GetPhoneNumberAsync(user),
                //    Message = message
                //};
                //try
                //{
                //    await _smsSender.SendSmsAsync(smsRequest);

                //    result.Status = AccountStatusEnum.Succeeded;
                //    return Results.Ok(result);
                //}
                //catch (Exception e)
                //{
                //    _logger.LogError(e.InnerException + "_" + e.Message);
                //    result.Errors.Add(_sharedlocalizer["Send sms action throws an error"]);
                //    return Results.BadRequest(result);
                //}
            }

            _logger.LogError(_sharedlocalizer["The operation failed.; Requested By: {0}"], user.Email);
            result.Status = AccountStatusEnum.Failed;
            result.Errors.Add(string.Format(_sharedlocalizer["The operation failed.; Requested By: {0}"], user.Email));

            return Results.BadRequest(result);
        }

        /// <summary>
        /// If Two Factor Authenticator is enabled, the user have to enter 6 digits code by authenticator app
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public static async Task<IResult> VerifyAuthenticatorCodeHandler(SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, ClaimsPrincipal user, [FromBody] VerifyAuthenticatorCodeModel model)
        {
            var result = new AccountResult();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                _logger.LogError(_sharedlocalizer["Input data are invalid.; Requested By: "] + user.Identity?.Name);
                result.Status = AccountStatusEnum.Invalid;
                result.Errors.Add("");

                return Results.BadRequest(result);
            }
            var signInResult =
              await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe,
                  model.RememberBrowser);
            if (signInResult.Succeeded)
            {
                result.Status = AccountStatusEnum.Succeeded;
                return Results.Ok(result);
            }

            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning(7, _sharedlocalizer["User account locked out..; Requested By: {0}"], user.Identity?.Name);

                result.Status = AccountStatusEnum.IsLockedOut;
                return Results.Ok(result);
            }
            else
            {
                _logger.LogWarning(_sharedlocalizer["Code is invalid.; Requested By: {0}"], user.Identity?.Name);
                result.Status = AccountStatusEnum.InvalidCode;

                return Results.BadRequest(result);
            }
        }

        /// <summary>
        /// If Two Factor is enabled, the user have to enter code which was sent ti them by email or sms
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public static async Task<IResult> VerifyCodeHandler(SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, [FromBody] VerifyCodeModel model)
        {
            var result = new AccountResult();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                _logger.LogError(_sharedlocalizer["Input data are invalid.;"]);
                result.Status = AccountStatusEnum.Invalid;
                result.Errors.Add("");

                return Results.BadRequest(result);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var signInResult = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                model.RememberBrowser);
            if (signInResult.Succeeded)
            {
                result.Status = AccountStatusEnum.Succeeded;
                return Results.Ok(result);
            }

            if (signInResult.IsLockedOut)
            {
                result.Status = AccountStatusEnum.IsLockedOut;
                return Results.Ok(result);
            }
            else
            {
                result.Status = AccountStatusEnum.InvalidCode;
                return Results.Ok(result);
            }
        }

        /// <summary>
        /// The user can login by Two Factor Recovery code 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public static async Task<IResult> UseRecoveryCodeHandler(SignInManager<User> _signInManager,
            IStringLocalizer<SharedResource> _sharedlocalizer,
             ILogger<AccountHandler> _logger, [FromBody] UseRecoveryCodeModel model)
        {
            var result = new AccountResult();
            if (MiniValidator.TryValidate(model, out var errors))
            {
                _logger.LogError(_sharedlocalizer["Input data are invalid.;"]);
                result.Status = AccountStatusEnum.Invalid;
                result.Errors.Add("");

                return Results.BadRequest(result);
            }

            var signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);
            if (signInResult.Succeeded)
            {
                result.Status = AccountStatusEnum.Succeeded;
                return Results.Ok(result);
            }
            else
            {
                result.Status = AccountStatusEnum.InvalidCode;
                return Results.Ok(result);
            }
        }
    }
}
