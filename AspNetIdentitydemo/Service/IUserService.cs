using AspNetIdentityDemo.Api.Controllers;
using AspNetIdentityDemo.Api.Model;
using AspNetIdentityDemo.shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AspNetIdentityDemo.Api.Service
{
    public interface IUserService
    {
        Task<UserManagrerRespons> RegisterUserAysnc(RegisterViewModel model);
        Task<UserManagrerRespons> LogInUserAysnc(LogInViewModel model);
        Task<UserManagrerRespons> ConfirmEmailAysnc(string userId, string token);
        Task<UserManagrerRespons> ForgetPasswordAysnc(string email);
        Task<UserManagrerRespons> ResetPasswordAysnc(ResetPasswordViewModel model);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ImailService _mailService;

        public UserService(UserManager<IdentityUser> userManager,IConfiguration configuration,ImailService mailService)
        {
            this._userManager = userManager;
            this._configuration = configuration;
            this._mailService = mailService;
        }

        public async Task<UserManagrerRespons> ConfirmEmailAysnc(string userId, string token)
        {
            var user=await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagrerRespons 
                { 
                  IsSuccessful=false,
                Message="User notfound"
                };
            var decodedToken=WebEncoders.Base64UrlDecode(token);    
            var normalToken=Encoding.UTF8.GetString(decodedToken);
             var result=await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
                return new UserManagrerRespons
                {
                    IsSuccessful = true,
                    Message="Email confirmed sucessfully!"
                };
            return new UserManagrerRespons
            { 
                IsSuccessful = false,
                Message="Email did not confirm",
                Error=result.Errors.Select(e=>e.Description)
            };
        }

        public async Task<UserManagrerRespons> ForgetPasswordAysnc(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagrerRespons
                {
                    IsSuccessful = false,
                    Message = "No user associated with this Email"
                };
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";
            await _mailService.SendEmailAsync(email,"Reset password", "<h1>Follow the instruction to reset our password<h1>" +
                $"<p>To reset your assord<a href='{url}'>Click here</a></p>");
            return new UserManagrerRespons
            {
                IsSuccessful = true,
                Message = "Reset password url has been sent to the mail successfully"
            };

        }
        public async Task<UserManagrerRespons> LogInUserAysnc(LogInViewModel model)
        {
            var user=await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagrerRespons
                {
                    Message = "There is no user with that email address",
                    IsSuccessful = false,
                };
            }
             var result= await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return new UserManagrerRespons
                {
                    Message = "invalid password",
                    IsSuccessful = false,
                };
            var claim = new[]
            {
                new Claim("Email",model.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claim,
                expires: DateTime.Now.AddDays(30),
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString=new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagrerRespons
            {
                Message = tokenAsString,
                IsSuccessful = true,
                ExpireDate=token.ValidTo,
            };
        }

        public async Task<UserManagrerRespons> RegisterUserAysnc(RegisterViewModel model)
        { if (model == null)
           
               throw new NullReferenceException("Register model is null");

            if (model.Passord != model.ConfirmPassord)
                return new UserManagrerRespons()
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccessful = false,
                };

                var identityUser = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                };
            var result=await _userManager.CreateAsync(identityUser, model.Passord);
            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken=Encoding.UTF8.GetBytes(confirmEmailToken);
                 var validEmailToken=WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmEmail?UserId={identityUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<h1>welcome to Auth Demo</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here </a></p>");
                return new UserManagrerRespons()
                {
                    Message = "User created successfully",
                    IsSuccessful=true,
                };
            }
            return new UserManagrerRespons()
            {
                Message = "User did ot create",
                IsSuccessful = false,
                Error = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagrerRespons> ResetPasswordAysnc(ResetPasswordViewModel model)
        {
           var user=await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagrerRespons()
                {
                    IsSuccessful = false,
                    Message = "No user is associated with the email",
                };
            if (model.Newpassword != model.Confirmpassword)
                return new UserManagrerRespons()
                {
                    IsSuccessful = false,
                    Message = "Passord does not match",
                };
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result=await _userManager.ResetPasswordAsync(user,normalToken, model.Newpassword);
            if (result.Succeeded)
            return new UserManagrerRespons()
            {
                IsSuccessful = true,
                Message = "Passord reset successfully"
            };
            return new UserManagrerRespons()
            {
                IsSuccessful = false,
                Message = "Something went wrong",
                Error=result.Errors.Select(e => e.Description),
            };


        }
    }
}
