using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace WebShop.Utility {
    public class EmailSender : IEmailSender {
        //public string SendGridSecret { get; set; }

        //public EmailSender(IConfiguration _config) {
        //    SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
        //}

        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            //logic to send email

            //var client = new SendGridClient(SendGridSecret);

            
            //var to = new EmailAddress(email);
            //var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);


            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("stryhaliouyauheni@gmail.com", "ejjqyssmdgbtqrxq");
            smtpClient.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.Subject = subject;
            message.From = new MailAddress("support@dagerondev.com");
            message.Body = $"<html><body> {htmlMessage}</body></html>";
            message.IsBodyHtml = true;
            return smtpClient.SendMailAsync(message);


        }
    }
}

