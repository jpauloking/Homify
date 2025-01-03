using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace Homify.Services
{
    public class EmailServices
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _fromEmail;
        private readonly string _password;
        private readonly bool _enableSsl;

        public  EmailServices()
        {
            _smtpServer = "smtp.gmail.com";
            _port = 587;
            _fromEmail = "asenlawrence05@gmail.com";
            _password = "cnfp bsmy fyfr baiw";
            _enableSsl = true;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    using (var smtpServer = new SmtpClient(_smtpServer))
                    {
                        smtpServer.Port = _port;
                        smtpServer.Credentials = new NetworkCredential(_fromEmail, _password);
                        smtpServer.EnableSsl = _enableSsl;
                        smtpServer.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Send email failed: " + ex.ToString());
            }
        }
    }
}