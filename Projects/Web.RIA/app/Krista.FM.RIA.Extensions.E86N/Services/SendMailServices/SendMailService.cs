using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.E86N.Services.SendMailServices
{
    public class SendMailService : ISendMailService
    {
        public SmtpClient Client { get; private set; }

        public string From { get; private set; }

        public string Mailto { get; private set; }

        public SendMailService()
        {
            try
            {
                Client = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SmtpServer"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]),
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"]),
                };

                From = ConfigurationManager.AppSettings["SmtpFrom"];
                var password = ConfigurationManager.AppSettings["SmtpPassword"];
                Mailto = ConfigurationManager.AppSettings["SmtpTo"];

                Client.Credentials = new NetworkCredential(From.Split('@')[0], password);
            }
            catch (Exception e)
            {
                // Неправильная настройка будет обнаружена при попытке отправить письмо
                Trace.TraceError("Исключение при инициализации сервиса отправки почты: " + e.Message);
            }
        }

        public string GetMailTo()
        {
            if (ConfigurationManager.AppSettings["SmtpTo"].IsNullOrEmpty())
                throw new InvalidDataException("Не указан адрес почты назначения.");
            return ConfigurationManager.AppSettings["SmtpTo"];
        }

        public bool CheckSmtpParams()
        {
            return (ConfigurationManager.AppSettings["SmtpServer"].IsNotNullOrEmpty() &&
                    ConfigurationManager.AppSettings["SmtpPort"].IsNotNullOrEmpty() &&
                    ConfigurationManager.AppSettings["SmtpEnableSsl"].IsNotNullOrEmpty() &&
                    ConfigurationManager.AppSettings["SmtpFrom"].IsNotNullOrEmpty() &&
                    ConfigurationManager.AppSettings["SmtpPassword"].IsNotNullOrEmpty() &&
                    ConfigurationManager.AppSettings["SmtpTo"].IsNotNullOrEmpty());
        }

        public void SendMail(string caption, string message)
        {
            var mail = new MailMessage(From, Mailto, caption, message);
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            using (mail)
            {
                Client.Send(mail);
            }
        }
    }
}
