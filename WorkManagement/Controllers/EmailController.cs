using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;

namespace WorkManagement.Controllers
{
    public class email
    {
        public string titleMessage { get; set; }
        public string emailTo { get; set; }
        public int uid { get; set; }
        public string bodyMessage { get; set; }
        public string domainEmail { get; set; }
        public int portDomainEmail { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] email email)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Big Pro Tech company", "wmemailservice@gmail.com"));
                message.To.Add(new MailboxAddress(email.emailTo));
                message.Subject = email.titleMessage;
                message.Body = new TextPart("plain")
                {
                    Text = email.bodyMessage
                };
                using (var client = new SmtpClient())
                {
                    client.Connect(email.domainEmail, email.portDomainEmail, false);
                    client.Authenticate("wmemailservice@gmail.com","bigprotech");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                var result = JsonConvert.SerializeObject(new { result = "Gửi email thành công: "+email.emailTo+" ,"+email.domainEmail+" ,"+email.portDomainEmail });
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
    }
}