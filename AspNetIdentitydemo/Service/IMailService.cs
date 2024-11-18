using AspNetIdentityDemo.Api.Controllers;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AspNetIdentityDemo.Api.Service
{
    public interface ImailService
    {

        Task SendEmailAsync(string toEmail,string subject,string content);
    }
    public class Mailservice : ImailService
    {
        private  IConfiguration _configuration;
        
        public Mailservice(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendGripApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@authdemo.com", "JWT Auth Demo");
            var to = new EmailAddress(toEmail);
            var msg=MailHelper.CreateSingleEmail(from,to,subject, content,content);
            var response=await client.SendEmailAsync(msg);
          
        }
    }
}
