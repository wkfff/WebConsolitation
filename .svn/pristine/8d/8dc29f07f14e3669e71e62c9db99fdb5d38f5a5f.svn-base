using System;
using Ext.Net.MVC;
using Krista.FM.Common;

namespace Krista.FM.RIA.Core.Controllers
{
    public class SessionController : SchemeBoundController
    {
        public AjaxResult KeepAlive()
        {
            AjaxResult response = new AjaxResult();
            try
            {
                Scheme.SessionManager.ClientSessionIsAlive(ClientAuthentication.SessionID);
                response.Result = "success";
            }
            catch (Exception e)
            {
                response.ErrorMessage = String.Format(
                    "Потеряно соединение с сервером, попробуйте подключиться позже. {0}", 
                    e.Message);
            }

            return response;
        }
    }
}
