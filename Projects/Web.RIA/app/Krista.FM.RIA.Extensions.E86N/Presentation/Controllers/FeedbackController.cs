using System;
using System.Net.Mail;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.SendMailServices;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public class FeedbackController : SchemeBoundController
    {
        private readonly ISendMailService mail;

        public FeedbackController(ISendMailService mail)
        {
            this.mail = mail;
        }

        public AjaxFormResult CheckSmtpConnection()
        {
            if (mail.CheckSmtpParams())
            {
                return new AjaxFormResult { Success = true };
            }

            return new AjaxFormResult
                       {
                           Success = false,
                           ExtraParams =
                               {
                                   new Parameter("responseText", "Не настроен SMTP-сервер, сейчас произойдет перенаправление на " + mail.GetMailTo()),
                                   new Parameter("redirectTo", mail.GetMailTo())
                               }
                       };
        }

        public ActionResult SendFeedback(string caption, string message)
        {
            try
            {
                mail.SendMail(caption, message);
            }
            catch (SmtpException e)
            {
                Trace.TraceError("Исключение при отправке сообщения в техподдержку: " + e.Message);
                return new AjaxFormResult
                           {
                               Success = false,
                               ExtraParams =
                                   {
                                       new Parameter("redirectTo", mail.GetMailTo()),
                                       new Parameter("responseText", "Не настроен SMTP-сервер, сейчас произойдет перенаправление на " + mail.GetMailTo())
                                   },
                           };
            }
            catch (Exception e)
            {
                Trace.TraceError("Исключение при отправке сообщения в техподдержку: " + e.Message);
                return new AjaxFormResult
                           {
                               Success = false,
                               ExtraParams =
                                   {
                                       new Parameter("redirectTo", mail.GetMailTo()),
                                       new Parameter("responseText", e.Message)
                                   }
                           };
            }

            return new AjaxFormResult
            {
                Success = true
            };
        }
    }
}
