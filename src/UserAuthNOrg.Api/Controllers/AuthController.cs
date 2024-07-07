using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthNOrg.Core.Context;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Infrastructure.Interfaces;
using UserAuthNOrg.Utilities.CoreConstants;
using UserAuthNOrg.Utilities.Extensions;

namespace UserAuthNOrg.Api.Controllers
{
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IContextAccessor _contextAccessor;
        private readonly UserAuthNOrgDbContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IContextAccessor contextAccessor,
            UserAuthNOrgDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _contextAccessor = contextAccessor;
            _context = context;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO model)
        {
            var (status, errors) = ValidatePayload(model);

            if (status)
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists is not null)
                    return BadRequest(new ApiResponse<string>(
                        "Email belongs to an existing customer",
                        Utilities.Enums.StatusCode.BadRequest));

                var newUser = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    EmailConfirmed = true,
                    PhoneNumber = model.Phone,
                    NormalizedEmail = model.Email
                };

                var isCreated = await _userManager.CreateAsync(newUser, model.Password);
                if (isCreated.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(ConstantsString.OrgUser).Result)
                    {
                        var role = new IdentityRole()
                        {
                            Name = ConstantsString.OrgUser
                        };
                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;

                        if (roleResult.Succeeded)
                            _userManager.AddToRoleAsync(newUser, ConstantsString.OrgUser).Wait();
                    }
                    else
                    {
                        _userManager.AddToRoleAsync(newUser, ConstantsString.OrgUser).Wait();
                    }

                    var organization = new Organization()
                    {
                        Name = $"{model.FirstName}'s Organization",
                        Description = "Default Organization"
                    };

                    await _context.AddAsync(organization);

                    var userOrganization = new UserOrganization()
                    {
                        OrgId = organization.OrgId,
                        Id = newUser.Id
                    };

                    await _context.AddAsync(userOrganization);

                    await _context.SaveChangesAsync();

                    var jwtToken = await GenerateJWT(newUser);

                    var response = new ApiResponse<SuccessfulUserCreation>(
                        new SuccessfulUserCreation()
                        {
                            AccessToken = jwtToken,
                            User = new ViewUser()
                            {
                                Email = newUser.Email,
                                FirstName = newUser.FirstName,
                                LastName = newUser.LastName,
                                Phone = newUser.PhoneNumber,
                                UserId = newUser.Id
                            }
                        }, 
                        "Registration successful", 
                        Utilities.Enums.StatusCode.Created);

                    return StatusCode(201, response);
                }
            }

            var result = new ApiResponse<List<Error>>(errors);
            return StatusCode(422, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO model)
        {
            var (status, errors) = ValidatePayload(model);

            if (ModelState.IsValid && status)
            {
                // check if the user with the same email exist
                var user = await _userManager.FindByEmailAsync(model.Email.Trim());

                if (user == null)
                {
                    // We don't want to give to much information on why the request has failed for security reasons
                    return StatusCode(401, new ApiResponse<string>("Authentication failed", Utilities.Enums.StatusCode.UnAuthorize));
                }

                var isCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

                if (isCorrect)
                {
                    if (user.LockoutEnabled)
                    {
                        return StatusCode(401, new ApiResponse<string>("Your account has been suspended! Contact Admin.", Utilities.Enums.StatusCode.UnAuthorize));
                    }
                    var jwtToken = await GenerateJWT(user);

                    return Ok(
                        new ApiResponse<SuccessfulUserCreation>(
                        new SuccessfulUserCreation()
                        {
                            AccessToken = jwtToken,
                            User = new ViewUser()
                            {
                                Email = user.Email.Trim(),
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Phone = user.PhoneNumber,
                                UserId = user.Id
                            }
                        },"Login successful"));

                }
                else
                {
                    // We don't want to give too much information on why the request has failed for security reasons
                    return StatusCode(401, new ApiResponse<string>("Authentication failed", Utilities.Enums.StatusCode.UnAuthorize));
                }
            }

            var result = new ApiResponse<List<Error>>(errors);
            return StatusCode(422, result);
        }

        [HttpPut("update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateAccount([FromBody] EditDTO model)
        {
            var userId = _contextAccessor.GetCurrentUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return BadRequest(new ApiResponse<string>("User doesn't Exist", Utilities.Enums.StatusCode.BadRequest));

            user.FirstName = model.FirstName is null ? user.FirstName : model.FirstName;
            user.LastName = model.LastName is null ? user.LastName : model.LastName;
            user.PhoneNumber = model.PhoneNumber is null ? user.PhoneNumber : model.PhoneNumber;

            await _userManager.UpdateAsync(user);
            return Ok(new ApiResponse<string>("Updated Successfully!", Utilities.Enums.StatusCode.OK));
        }

        //[Authorize(AuthenticationSchemes = "Bearer")]
        //[HttpPost("ChangePassword")]
        //public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        //{
        //    if (string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.OldPassword))
        //    {
        //        return BadRequest(new ResultModel<bool> { Message = "All fields are required!", Data = false });
        //    }
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        if (user is null)
        //        {
        //            return BadRequest(new ResultModel<bool> { Message = "User Not Found!", Data = false });
        //        }

        //        var result = await _userManager.ChangePasswordAsync(user,
        //            model.OldPassword, model.NewPassword);

        //        if (!result.Succeeded)
        //        {
        //            return BadRequest(new ResultModel<bool> { Message = result.Errors.FirstOrDefault().Description, Data = false });
        //        }
        //        await _signInManager.RefreshSignInAsync(user);
        //        return Ok(new ResultModel<bool> { Message = "Password changed successfully!", Data = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleError(ex);
        //    }
        //}

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
       
        private async Task<string> GenerateJWT(User user)
        {
            //get the user's roles
            var roles = await _userManager.GetRolesAsync(user);
            //Generate Token
            var expirationDay = DateTime.UtcNow.AddHours(1);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"].ToString()));
            List<Claim> subjectClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Expires = expirationDay
            };
            tokenDescriptor.Subject.AddClaims(subjectClaims);
            //create the token 
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static (bool status, List<Error> errors) ValidatePayload(SignUpDTO model)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(model.FirstName))
                errors.Add(new() { Field = "FirstName", Message = "First Name cannot be null!" });

            if (string.IsNullOrWhiteSpace(model.LastName))
                errors.Add(new() { Field = "LastName", Message = "Last Name cannot be null!" });

            if (string.IsNullOrWhiteSpace(model.Password))
                errors.Add(new() { Field = "Password", Message = "Password cannot be null!" });

            if (string.IsNullOrWhiteSpace(model.Email))
                errors.Add(new() { Field = "Email", Message = "Email cannot be null!" });

            if(!string.IsNullOrWhiteSpace(model.Email))
            {
                var emailValid = new EmailAddressAttribute().IsValid(model.Email);
                if (!emailValid) errors.Add(new() { Field = "Email", Message = "Email address is not valid" });
            }

            if (!errors.Any())
                return (true, errors);

            return (false, errors);
        }

        private static (bool status, List<Error> errors) ValidatePayload(SignInDTO model)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(model.Password))
                errors.Add(new() { Field = "Password", Message = "Password cannot be null!" });

            if (string.IsNullOrWhiteSpace(model.Email))
                errors.Add(new() { Field = "Email", Message = "Email cannot be null!" });

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var emailValid = new EmailAddressAttribute().IsValid(model.Email);
                if (!emailValid) errors.Add(new() { Field = "Email", Message = "Email address is not valid" });
            }

            if (!errors.Any())
                return (true, errors);

            return (false, errors);
        }
    }
}
