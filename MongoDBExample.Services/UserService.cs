using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;
using MongoDBExample.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MongoDBExample.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IOptions<MongoDbExampleDatabaseSettings> _options;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailSender _emailSender;

    public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<MongoDbExampleDatabaseSettings> options, ILogger<UserService> logger , IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<LoginDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        var response = new LoginDtoResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("User doesn't exists");
            }

            var result = await _userManager.CheckPasswordAsync(userIdentity, request.Password);
            if (!result)
                throw new ApplicationException("Incorrect password");


            var expiredDate = DateTime.Now.AddHours(1);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{userIdentity.FirstName} {userIdentity.LastName}"),
                new Claim(ClaimTypes.Email, userIdentity.Email),
                new Claim(ClaimTypes.Expiration, expiredDate.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            var roles = await _userManager.GetRolesAsync(userIdentity);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Creacion de token JWT
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Jwt.Secret));

            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(
                _options.Value.Jwt.Issuer, 
                _options.Value.Jwt.Audience,
                claims, 
                DateTime.Now, 
                expiredDate);

            var token = new JwtSecurityToken(header, payload);

            response.Token = new JwtSecurityTokenHandler().WriteToken(token);
            response.Success = true;

            _logger.LogInformation("User {User} logged in at {DateTime}", userIdentity.Email, DateTime.Now);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.Success = false;
            _logger.LogError(ex, "Error while loggin: {Message}", ex.Message);
        }


        return await Task.FromResult(response);
    }

    public async Task<BaseResponseGeneric<string>> RegisterAsync(RegisterUserDtoRequest request)
    {
        var response = new BaseResponseGeneric<string>();
        try
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {

                var userIdentity = await _userManager.FindByEmailAsync(request.Email);

                if (userIdentity != null)
                {
                    if (!await _roleManager.RoleExistsAsync("Administrador"))
                        await _roleManager.CreateAsync(new Role { Name = "Administrador" });

                    if (!await _roleManager.RoleExistsAsync("Client"))
                        await _roleManager.CreateAsync(new Role { Name = "Client" });

                    if (_userManager.Users.Count() == 1)
                        await _userManager.AddToRoleAsync(userIdentity, "Administrador");
                    else
                        await _userManager.AddToRoleAsync(userIdentity, "Client");

                    // Enviar un correo

                    await _emailSender.SendEmailAsync(new EmailMessageInfo(user.Email, 
                    $"{userIdentity.FirstName} {userIdentity.LastName}", "User Registry", $"The user {userIdentity.Email} was succesfully created!"));
                }

                response.Success = true;
                response.Data = user.Id.ToString();
            }
            else
            {
                response.Success = false;
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "An error occurred while creating the user";
            _logger.LogError(ex, "Error al registrar {Message}", ex.Message);
        }

        return response;
    }

    public async Task<BaseResponse> RequestTokenToResetPasswordAsync(DtoRequestPassword request)
    {
        var response = new BaseResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("User doesn't exists");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(userIdentity);

            // Enviar un correo
            await _emailSender.SendEmailAsync(new EmailMessageInfo(userIdentity.Email, $"{userIdentity.FirstName} {userIdentity.LastName}", "Password reset", $"to reset your password copy the following token: {token}"));
            
            _logger.LogInformation("An email was sent to {Email} to reset your password", request.Email);

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "The request to reset the password failed";
            _logger.LogError(ex, "The request to reset the password failed {Message}", ex.Message);
        }
        
        return response;
    }

    public async Task<BaseResponse> ResetPasswordAsync(DtoResetPassword request)
    {
         var response = new BaseResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("Usuario no existe");
            }

            var result = await _userManager.ResetPasswordAsync(userIdentity, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }
            else
            {
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "An error occurred while reseting the password";
            _logger.LogError(ex, "Error while resetting {Message}", ex.Message);
        }

        return response;
    }

    public async Task<BaseResponse> ChangePasswordAsync(DtoChangePassword request)
    {
        var response = new BaseResponse();

        try
        {
            var userIdentity = await _userManager.FindByEmailAsync(request.Email);

            if (userIdentity == null)
            {
                throw new ApplicationException("User doesn't exists");
            }

            var result = await _userManager.ChangePasswordAsync(userIdentity, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Length = 0;
            }
            else
            {
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "An error ocurred while changing password";
            _logger.LogError(ex, "Error while changing password {Message}", ex.Message);
        }

        return response;
    }
}