using Mails_App;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Travel_Authentication.Models;
using Travel_Authentication.Services;

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
            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, TwoFactorEnabled = true };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //add rule to user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                    if (model.Role == "Admin")
                    {

                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            isSuccessed = true,
                            msg = _localizer["SuccessLogin"],
                            AccessToken = token,
                            RefreshToken = token,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled
                        });
                    }
                    //genertae otp code and send to user by email to verify email
                    var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    string fileName = "OTPMail_" + model.lang + ".html";
                    MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);
                    Mail_Service.SendMail(mailData);
                    //generate response without token until user verify email
                    return Ok(new User
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        isSuccessed = result.Succeeded,
                        msg = _localizer["SuccessRegister"],
                        AccessToken = null,
                        RefreshToken = null,
                        Id = user.Id,
                        EmailConfirmed = user.EmailConfirmed,
                        TwoFactorEnabled = user.TwoFactorEnabled

                    });
                }
                else
                {
                    List<IdentityError> errorList = result.Errors.ToList();
                    var errors = string.Join(", ", errorList.Select(e => e.Description));
                    //_logger.LogError(errors);
                    return Ok(new User
                    {
                        UserName = "",
                        Email = "",
                        FirstName = "",
                        LastName = "",
                        isSuccessed = result.Succeeded,
                        msg = errors,
                        AccessToken = "",
                        RefreshToken = "",
                        Id = null,
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new User
                     {
                         isSuccessed = false,
                         msg = _localizer["CheckAdmin"],

                     });
            }


        }



        //used for normal login (email & password)
        [HttpPost("LoginUser")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var lang = Request.Headers["Accept-Language"].ToString();
            var user = await _userManager.FindByEmailAsync(model.Email);
            try
            {
                var isAuth = await _userManager.CheckPasswordAsync(user, model.Password);
                if (user != null && isAuth)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (isAdmin)
                    {
                        //generate response with token to admin
                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            isSuccessed = true,
                            msg = _localizer["SuccessLogin"],
                            AccessToken = token,
                            RefreshToken = token,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                        });
                    }
                    if (user.EmailConfirmed == false)
                    {
                        //generate otp and send it to user's email to verify email
                        var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                        MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);

                        Mail_Service.SendMail(mailData);

                        //generate response without token until user verify email
                        return Ok(new User
                        {

                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            isSuccessed = true,
                            msg = _localizer["OTPMSG"] + user.Email,
                            AccessToken = null,
                            RefreshToken = null,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                        });
                    }
                    else
                    {
                        //generate response with token if user's email is verified
                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            isSuccessed = true,
                            msg = _localizer["SuccessLogin"],
                            AccessToken = token,
                            RefreshToken = token,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                        });
                    }


                }
                else
                    return Unauthorized(new User
                    {
                        isSuccessed = false,
                        msg = _localizer["MailPasswordIncorrect"],

                    });


            }
            catch (Exception e)
            {
                return Unauthorized(new User
                {

                    isSuccessed = false,
                    msg = _localizer["MailPasswordIncorrect"],

                });
            }
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
                        new Claim("ActivtationTokenExpiredAt",timestamp.AddMinutes(30).ToString()),
                    };
            AddRolesToClaims(authClaims, roles);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(30),
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

        [HttpPost("changePassword")]
        public async Task<IActionResult> changePassword([FromBody] PasswordCls model)
        {
            try
            {
                //check if user exist or not first
                var user = await _userManager.FindByIdAsync(model.userId);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            isSuccessed = true,
                            msg = _localizer["SuccessPassChange"],
                            AccessToken = token,
                            RefreshToken = token,
                            Id = user.Id,
                        });
                    }
                    else
                    {
                        List<IdentityError> errorList = result.Errors.ToList();
                        var errors = string.Join(", ", errorList.Select(e => e.Description));
                        // _logger.LogError(errors);
                        return BadRequest(new User
                        {

                            isSuccessed = false,
                            msg = errors,

                        });
                    }


                }
                else
                {
                    return Unauthorized(new User
                    {

                        isSuccessed = false,
                        msg = _localizer["UserNotFound"],

                    });
                }


            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Unauthorized(new User
                {

                    isSuccessed = false,
                    msg = _localizer["CheckAdmin"],

                });
            }
        }
    }
}
