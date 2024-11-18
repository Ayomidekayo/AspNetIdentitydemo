using AspNetIdentityDemo.Api.Model;
using AspNetIdentityDemo.Api.Service;
using AspNetIdentityDemo.shared;
using Microsoft.AspNetCore.Mvc;

namespace AspNetIdentityDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ImailService _mailService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService,ImailService mailService,IConfiguration configuration)
        {
            this._userService = userService;
            this._mailService = mailService;
            this._configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAysnc(model);

                //Todo Send a confirmation email
                if(result.IsSuccessful)
                return Ok(result);//200 status code

                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");// 400 satus code
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LogInAsync([FromBody] LogInViewModel model)
        {
            if (ModelState.IsValid)
            {  
                var result=await _userService.LogInUserAysnc(model);
                if (result.IsSuccessful)
                { await _mailService.SendEmailAsync(model.Email, "New login", "<h1>Hey!, new login to your account noticed</h1><p>Ne login to your account at" + DateTime.Now + "</p>");
                    return Ok(result);
                 }
                ;
                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
           
        }
        // /api/auth/ConfirmEmail?UserId&token
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string UserId, string token)
        {
            if(string.IsNullOrWhiteSpace(UserId) | string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }
            var result=await _userService.ConfirmEmailAysnc(UserId, token);

            if (result.IsSuccessful)
            {
                return Redirect($"{_configuration["AppUrl"]}/ ConfrmEmail.html");
            }
            return BadRequest(result);
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();
            var result=await _userService.ForgetPasswordAysnc(email);
            if (result.IsSuccessful)
                return Ok(result);
            return BadRequest(result);
        }
        // api/auth/resetpassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resut=await _userService.ResetPasswordAysnc(model);
                if (resut.IsSuccessful) 
                    return Ok(resut);
               return BadRequest(resut);   
            }
            return BadRequest("some properties are t valid");
        }
    }
}
