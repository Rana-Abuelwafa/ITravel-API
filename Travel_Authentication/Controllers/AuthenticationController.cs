using Mails_App;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, TwoFactorEnabled = true,sendOffers=model.sendOffers };
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
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessLogin"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                role = model.Role,
                                completeprofile = user.completeprofile
                            }
                        });
                    }
                    //genertae otp code and send to user by email to verify email
                    var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    string fileName = "OTPMail_" + model.lang + ".html";
                    MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);
                    Mail_Service.SendMail(mailData);
                    //generate response without token until user verify email
                    return Ok(new ResponseCls
                    {
                        isSuccessed = result.Succeeded,
                        message = _localizer["SuccessRegister"],
                        errors = null,
                        user = new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            GoogleId = user.GoogleId,
                            AccessToken = null,
                            RefreshToken = null,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            role = model.Role,
                            completeprofile = user.completeprofile

                        }
                    });
                }
                else
                {
                    List<IdentityError> errorList = result.Errors.ToList();
                    var errors = string.Join(", ", errorList.Select(e => e.Description));
                    //_logger.LogError(errors);
                    return Ok(new ResponseCls
                    {
                        isSuccessed = result.Succeeded,
                        message = errors,
                        errors = errors,
                        user = null
                    }

                    );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new ResponseCls
                     {
                         isSuccessed = false,
                         message = _localizer["CheckAdmin"],

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
                    var roles = await _userManager.GetRolesAsync(user);
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (isAdmin)
                    {
                        //generate response with token to admin
                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessLogin"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                role = roles.FirstOrDefault(),
                                completeprofile = user.completeprofile

                            }
                        });
                   
                    }
                    if (user.EmailConfirmed == false)
                    {
                        //generate otp and send it to user's email to verify email
                        var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                        MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);

                        Mail_Service.SendMail(mailData);

                        //generate response without token until user verify email

                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["OTPMSG"] + user.Email,
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = null,
                                RefreshToken = null,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                role = roles.FirstOrDefault(),
                                completeprofile = user.completeprofile

                            }
                        });
                      
                    }
                    else
                    {
                        //generate response with token if user's email is verified
                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessLogin"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                role = roles.FirstOrDefault(),
                                completeprofile = user.completeprofile,

                            }
                        });
                       
                    }


                }
                else
                    return Unauthorized(new ResponseCls
                    {
                        isSuccessed = false,
                        message = _localizer["MailPasswordIncorrect"],

                    });


            }
            catch (Exception e)
            {
                return Unauthorized(new ResponseCls
                {

                    isSuccessed = false,
                    message = _localizer["MailPasswordIncorrect"],

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
                    var roles = await _userManager.GetRolesAsync(user);
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessPassChange"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                role = roles.FirstOrDefault()

                            }
                        });
                       
                    }
                    else
                    {
                        List<IdentityError> errorList = result.Errors.ToList();
                        var errors = string.Join(", ", errorList.Select(e => e.Description));
                        // _logger.LogError(errors);
                        return BadRequest(new ResponseCls
                        {
                            isSuccessed = false,
                            message = errors,

                        });
                    }


                }
                else
                {
                    return Unauthorized(new ResponseCls
                    {

                        isSuccessed = false,
                        message = _localizer["UserNotFound"],

                    });
                }


            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Unauthorized(new ResponseCls
                {

                    isSuccessed = false,
                    message = _localizer["CheckAdmin"],

                });
            }
        }

        //used in gmail register
        [HttpPost("ExternalRegister")]
        public async Task<IActionResult> ExternalRegister([FromBody] AppsRegisterModel model)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, GoogleId = "1", TwoFactorEnabled = true, sendOffers = model.sendOffers };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //add rule to user
                    await _userManager.AddToRoleAsync(user, model.Role);
                    //generate otp and send it to user's email to verify email
                    var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);

                    Mail_Service.SendMail(mailData);
                    //generate response without token until user verify email
                    return Ok(new ResponseCls
                    {
                        isSuccessed = result.Succeeded,
                        message = _localizer["SuccessRegister"],
                        errors = null,
                        user = new User
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                           GoogleId=user.GoogleId,
                            AccessToken = null,
                            RefreshToken = null,
                            Id = user.Id,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            role = model.Role,
                            completeprofile = user.completeprofile

                        }
                    });
                    
                }
                else
                {
                    List<IdentityError> errorList = result.Errors.ToList();
                    var errors = string.Join(", ", errorList.Select(e => e.Description));
                    //_logger.LogError(errors);
                    return Ok(new ResponseCls
                    {
                        isSuccessed = result.Succeeded,
                        message = errors,
                        errors = errors,
                        user = null
                    });
                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                      new ResponseCls
                      {
                          isSuccessed = false,
                          message = _localizer["CheckAdmin"],

                      });
            }


        }



        [HttpPost("LoginGmail")]
        public async Task<IActionResult> LoginGmail([FromBody] AppsLoginModel model)
        {
            //check if user exist or not first
            var user = await _userManager.FindByEmailAsync(model.Email);
            try
            {
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (user.EmailConfirmed == false)
                    {
                        //generate otp and send it to user's email to verify email
                        var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                        MailData mailData = Utils.GetOTPMailData(model.lang, user.FirstName + " " + user.LastName, otp, model.Email);

                        Mail_Service.SendMail(mailData);
                        //generate response without token until user verify email
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["OTPMSG"] + user.Email,
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = null,
                                RefreshToken = null,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                completeprofile = user.completeprofile,
                                role = roles.FirstOrDefault()

                            }
                        });
                        
                    }
                    else
                    {
                        //generate response with token if user verify email
                        await _signInManager.SignInAsync(user, false);
                        var token = await GenerateJwtTokenAsync(user);
                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessLogin"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                GoogleId = user.GoogleId,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                completeprofile = user.completeprofile,
                                role = roles.FirstOrDefault()

                            }
                        });
                   
                    }
                }
                else
                    return StatusCode(StatusCodes.Status401Unauthorized,
                      new ResponseCls
                      {
                          isSuccessed = false,
                          message = _localizer["UserNotFound"],

                      });

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                      new ResponseCls
                      {
                          isSuccessed = false,
                          message = _localizer["CheckAdmin"],

                      });
            }
        }


        [HttpPost("ConfirmOTP")]
        public async Task<IActionResult> confirmOTP([FromBody] OTPConfirmCls model)
        {
            try
            {
                //check if user exist or not
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    //verify otp 
                    var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.otp);

                    if (isCodeValid)
                    {
                        //update user EmailConfirmed = true;
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        var token = await GenerateJwtTokenAsync(user);

                        return Ok(new ResponseCls
                        {
                            isSuccessed = true,
                            message = _localizer["SuccessLogin"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                AccessToken = token,
                                RefreshToken = token,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                GoogleId = user.GoogleId,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                completeprofile = user.completeprofile,
                                role = roles.FirstOrDefault()

                            }
                        });
                    }
                    else
                    {

                        return Ok(new ResponseCls
                        {
                            isSuccessed = false,
                            message = _localizer["InvalidCode"],
                            errors = null,
                            user = new User
                            {
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                Id = user.Id,
                                EmailConfirmed = user.EmailConfirmed,
                                GoogleId = user.GoogleId,
                                TwoFactorEnabled = user.TwoFactorEnabled,
                                
                                AccessToken = "",
                                RefreshToken = "",
                                completeprofile = user.completeprofile,
                                role = roles.FirstOrDefault()

                            }
                        });
                        

                    }
                }
                return Unauthorized(new ResponseCls
                {
                    isSuccessed = false,
                    message = _localizer["UserNotFound"],
                    user=null
                });


            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Ok(new ResponseCls
                {
                    
                    isSuccessed = false,
                    message = _localizer["CheckAdmin"],
                    user=null

                });
            }
        }

    }
}
