using Microsoft.IdentityModel.Logging;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using MedContactCore.DataTransferObjects;
using System.Net.Http;
using System.ComponentModel;
using System.Diagnostics;

namespace MedContactApp.Helpers
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;
        private int _mode = 0;
        private int _port = 587;
        private string? _emailRecipient;
        private Result? _result = default;
        private Dictionary<string, string> _emailCredentials = new Dictionary<string, string>();
        public EmailSender (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        internal  async Task<Result> SendEmailAsync(string? email, string subj, string mailbody)
        {
            bool result = int.TryParse(_configuration["EmailSendMode:Mode"], out var mode);
            if (result) _mode = mode;

            bool resPort = int.TryParse(_configuration["EmailSendMode:SendPort"], out var port);
            if (resPort) _port = port;


            var emailCredentials = _configuration.GetSection("EmailSendCredentials")
                           .Get<IDictionary<string, string>>();
            if (emailCredentials == null || 
                (!emailCredentials.ContainsKey("ServiceMailField") && !emailCredentials.ContainsKey("ServiceMailFrom") &&
                 !emailCredentials.ContainsKey("TestMailRecipient") && !emailCredentials.ContainsKey("ServiceMail") &&
                 !emailCredentials.ContainsKey("ServicePwd") && !emailCredentials.ContainsKey("SmtpServer")))
                return new Result() { IntResult = 0, Name = $"<br/><b>EmailSendCredentials have not been found</b>" };
            else
                _emailCredentials = (Dictionary<string, string>)emailCredentials;

            if (_mode > 0 && !string.IsNullOrEmpty(email))
                _emailRecipient = email;
            else
                _emailRecipient = _emailCredentials["TestMailRecipient"];

            string str = string.Empty;

            MailAddress from = new MailAddress(_emailCredentials["ServiceMailField"], _emailCredentials["ServiceMailFrom"]);
            MailAddress to = new MailAddress(_emailRecipient);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subj;
            message.Body = mailbody;

            SmtpClient client = new SmtpClient(_emailCredentials["SmtpServer"], _port);
            client.Credentials = new NetworkCredential(_emailCredentials["ServiceMail"], _emailCredentials["ServicePwd"]);
            client.EnableSsl = true;

            await client.SendMailAsync(message);
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            
            if(_result==default)
                return new Result { IntResult = 1, Name = $"Email to {_emailRecipient} has been sent" };
           else
                return _result;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _result =  new Result { IntResult = 0, Name = $"Email send to {_emailRecipient} has been cancelled" };
            }
            if (e.Error != null)
            {
                string error =$"Error {_emailRecipient} :" + e.Error.ToString() + " in SendCompletedHandlerEvent";
                _result =  new Result { IntResult = 0, Name = error };
            }

        }
    }
}
