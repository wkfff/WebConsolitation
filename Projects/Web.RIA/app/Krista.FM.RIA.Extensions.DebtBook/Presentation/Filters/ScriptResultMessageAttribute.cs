using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Filters
{
    public class ScriptResultMessageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                StringBuilder sb = new StringBuilder()
                    .Append(ExtNet.Msg.Alert("Уведомление", "Вариант успешно скопирован.").ToScript());

                filterContext.Result = new AjaxResult { Script = sb.ToString() };
            }
            else
            {
                Trace.TraceError(filterContext.Exception.ExpandException());

                filterContext.Result = new AjaxResult
                {
                    Script = ExtNet.Msg.Alert("Ошибка", filterContext.Exception.Message).ToScript()
                };
            }
        }
    }
}
