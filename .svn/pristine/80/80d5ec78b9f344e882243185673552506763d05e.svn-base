using System;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Common;

namespace Krista.FM.RIA.Core.Controllers
{
    public class MessageController : SchemeBoundController
    {
        public AjaxResult NewMessageCount()
        {
            var response = new AjaxResult();

            try
            {
                int count = Scheme.MessageManager.GetNewMessageCount(ClientAuthentication.UserID);
                response.ExtraParamsResponse.Add(new Parameter("messageCount", count.ToString()));
                response.Result = "success";
            }
            catch (Exception e)
            {
                response.ErrorMessage = String.Format("Не удалось получить количество непрочитанных сообщений. {0}", e.Message);
            }

            return response;
        }
    }
}
