﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Mails_App;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Cryptography;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Travel_Authentication.Models;
using Travel_Authentication.Services;
using static System.Net.WebRequestMethods;

namespace Travel_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        IMailService Mail_Service = null;
        public AuthenticationController(IStringLocalizer<Messages> localizer, RoleManager<IdentityRole>? roleManager, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IMailService _MailService, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _localizer = localizer;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            Mail_Service = _MailService;

        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel? roleModel)
        {
            if (roleModel == null)
            {
                return BadRequest($"{nameof(roleModel)} cannot be null.");
            }

            var role = new IdentityRole();
            role.Name = roleModel.role;

            IdentityResult result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return BadRequest("Unable to create a role.");
            }

            return Ok();
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseCls { isSuccessed = false, message = "Invalid data" });

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TwoFactorEnabled = true,
                    sendOffers = model.sendOffers
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return HandleIdentityErrors(result);

                await _userManager.AddToRoleAsync(user, model.Role);

                // Admin: issue token immediately
                if (model.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    return await HandleAdminRegistration(user, model.Role);

                // Normal user → Send OTP
                return await HandleUnverifiedEmailLogin(user, model.lang, _localizer["SuccessRegister"]);
            }
            catch (Exception ex)
            {
                // Ideally log ex
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseCls
                    {
                        isSuccessed = false,
                        message = _localizer["CheckAdmin"]
                    });
            }
        }

        //used for normal login (email & password)
        [HttpPost("LoginUser")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(BuildErrorResponse(_localizer["MailPasswordIncorrect"]));

            try
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return Unauthorized(BuildErrorResponse(_localizer["MailPasswordIncorrect"]));

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return await HandleAdminRegistration(user, role);

                // Not confirmed → send OTP
                if (!user.EmailConfirmed)
                    return await HandleUnverifiedEmailLogin(user, model.lang, $"{_localizer["OTPMSG"]} {user.Email}");

                return await HandleVerifiedUserLogin(user, role);
            }
            catch (Exception ex)
            {
                // Ideally log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BuildErrorResponse(_localizer["UnexpectedError"] ?? "Unexpected error occurred."));
            }
        }


        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordCls model)
        {
            if (string.IsNullOrWhiteSpace(model.userId))
                return BadRequest(BuildErrorResponse(_localizer["UserNotFound"]));

            try
            {
                var user = await _userManager.FindByIdAsync(model.userId);
                if (user == null)
                    return Unauthorized(BuildErrorResponse(_localizer["UserNotFound"]));

                var result = await ChangeUserPasswordAsync(user, model);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Password change failed for user {model.userId}: {errors}");
                    return BadRequest(BuildErrorResponse(errors));
                }

                // Update refresh token and generate JWT
                var roles = await _userManager.GetRolesAsync(user);
                var token = await GenerateJwtTokenAsync(user);
                var refreshToken = GenerateRefreshToken();

                await UpdateRefreshToken(user, refreshToken);

                return Ok(new ResponseCls
                {
                    isSuccessed = true,
                    message = _localizer["SuccessPassChange"],
                    user = CreateUserResponse(user, roles.FirstOrDefault(), token)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password for user {UserId}", model.userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BuildErrorResponse(_localizer["CheckAdmin"] ?? "Internal server error"));
            }
        }
   
        //used in gmail register
        [HttpPost("ExternalRegister")]
        public async Task<IActionResult> ExternalRegister([FromBody] AppsRegisterModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest(new ResponseCls { isSuccessed = false, message = "Invalid data" });

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    TwoFactorEnabled = true,
                    sendOffers = model.sendOffers,
                    GoogleId="1",
                    
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return HandleIdentityErrors(result);

                await _userManager.AddToRoleAsync(user, model.Role);
                return await HandleUnverifiedEmailLogin(user, model.lang, _localizer["SuccessRegister"]);
            }
            catch (Exception ex)
            {
                // Ideally log ex
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseCls
                    {
                        isSuccessed = false,
                        message = _localizer["CheckAdmin"]
                    });
            }

        }

        [HttpPost("LoginGmail")]
        public async Task<IActionResult> LoginGmail([FromBody] AppsLoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return Unauthorized(BuildErrorResponse(_localizer["UserNotFound"]));

                var roles = await _userManager.GetRolesAsync(user);

                if (!user.EmailConfirmed)
                    return await HandleUnverifiedEmailLogin(user, model.lang, roles.FirstOrDefault());

                return await HandleVerifiedUserLogin(user, roles.FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Gmail login for {Email}", model.Email);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BuildErrorResponse(_localizer["CheckAdmin"] ?? "Internal server error"));
            }
        }
        [HttpPost("ConfirmOTP")]
        public async Task<IActionResult> ConfirmOTP([FromBody] OTPConfirmCls model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return Unauthorized(BuildErrorResponse(_localizer["UserNotFound"]));

                var roles = await _userManager.GetRolesAsync(user);
                var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.otp);

                if (!isCodeValid)
                    return Ok(BuildErrorResponse(_localizer["InvalidCode"]));

                // Mark email as confirmed and issue tokens
                user.EmailConfirmed = true;
                var token = await GenerateJwtTokenAsync(user);
                var refreshToken = GenerateRefreshToken();

                await UpdateRefreshToken(user, refreshToken);
                SetRefreshTokenCookie(refreshToken);

                return Ok(new ResponseCls
                {
                    isSuccessed = true,
                    message = _localizer["SuccessLogin"],
                    user = CreateUserResponse(user, roles.FirstOrDefault(), token)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming OTP for {Email}", model.Email);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BuildErrorResponse(_localizer["CheckAdmin"]));
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
                return Unauthorized();

            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized();

            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            SetRefreshTokenCookie(newRefreshToken);
          
            return Ok(new ResponseCls
            {
                isSuccessed = true,
                message = _localizer["SuccessLogin"],
                errors = null,
                user = CreateUserResponse(user, roles.FirstOrDefault(), newAccessToken)
               
            });
            //return Ok(new { AccessToken = newAccessToken });
        }

        #region "helper methods"

        private async Task<IdentityResult> ChangeUserPasswordAsync(ApplicationUser user, PasswordCls model)
        {
            var hasPassword = await _userManager.HasPasswordAsync(user);

            return hasPassword
                // Regular flow (old password required)
                ? await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword)
                :
                // External login user (no password yet) → set new password directly
                await _userManager.AddPasswordAsync(user, model.NewPassword);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            DateTime timestamp = DateTime.Now;
            string fullName = user.FirstName + " " + user.LastName;
            // Get User roles and add them to claims
            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("ClientId", user.Id.ToString()),
                        new Claim("FullName", fullName),
                        new Claim("Email", user.Email),
                        //new Claim("ClientId", user.Id.ToString()),
                        new Claim("TimeStamp",timestamp.ToString()),
                        new Claim("ActivtationTokenExpiredAt",timestamp.AddMinutes(5).ToString()),
                    };
            AddRolesToClaims(authClaims, roles);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
        private User CreateUserResponse(ApplicationUser user, string role, string token)
        {
            return new User
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                GoogleId = user.GoogleId,
                AccessToken = token,
                Id = user.Id,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                role = role,
                completeprofile = user.completeprofile,
                RefreshToken=user.RefreshToken,
                sendOffers=user.sendOffers,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            };
        }

        private IActionResult HandleIdentityErrors(IdentityResult result)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Ok(new ResponseCls
            {
                isSuccessed = false,
                message = errors,
                errors = errors
            });
        }
        private async Task<IActionResult> HandleAdminRegistration(ApplicationUser user, string role)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);

            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.EmailConfirmed = true;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            SetRefreshTokenCookie(refreshToken);
            return Ok(new ResponseCls
            {
                isSuccessed = true,
                message = _localizer["SuccessLogin"],
                user = CreateUserResponse(user, role, token)
            });
        }

        private ResponseCls BuildErrorResponse(string message)
        {
            return new ResponseCls
            {
                isSuccessed = false,
                message = message
            };
        }
        private async Task<IActionResult> HandleUnverifiedEmailLogin(ApplicationUser user, string lang,string message)
        {
            var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var mailData = Utils.GetOTPMailData(lang, $"{user.FirstName} {user.LastName}", otp, user.Email);
            Mail_Service.SendMail(mailData);

            return Ok(new ResponseCls
            {
                isSuccessed = true,
                message = message,
                user = CreateUserResponse(user, "User", null)
            });
        }
        private async Task<IActionResult> HandleVerifiedUserLogin(ApplicationUser user, string role)
        {
            await _signInManager.SignInAsync(user, false);

            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            await UpdateRefreshToken(user, refreshToken);
            SetRefreshTokenCookie(refreshToken);

            return Ok(new ResponseCls
            {
                isSuccessed = true,
                message = _localizer["SuccessLogin"],
                user = CreateUserResponse(user, role, token)
            });
        }
        private async Task UpdateRefreshToken(ApplicationUser user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
        }
  
        #endregion


    }
}
