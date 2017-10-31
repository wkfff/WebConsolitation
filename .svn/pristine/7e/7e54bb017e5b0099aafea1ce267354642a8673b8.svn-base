using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers
{
    public class BebtBookStatusController : SchemeBoundController
    {
        private readonly IDebtBookExtension extension;
        private readonly VariantProtocolService protocolService;

        public BebtBookStatusController(IDebtBookExtension extension, VariantProtocolService protocolService)
        {
            this.extension = extension;
            this.protocolService = protocolService;
        }

        public ActionResult GetStatus()
        {
            var variantId = extension.Variant.Id;
            var regionId = extension.UserRegionId;

            T_S_ProtocolTransfer status = protocolService.GetStatus(variantId, regionId);

            string json = JSON.Serialize(status);

            return new AjaxResult { Script = "var status = {0}; alert(json);".FormatWith(json) };
        }

        [Transaction]
        public ActionResult ToTest(string comment)
        {
            var variantId = extension.Variant.Id;
            var regionId = extension.UserRegionId;

            try
            {
                extension.StatusSchb = protocolService.ToTest(
                    variantId, regionId, comment);

                StringBuilder sb = new StringBuilder()
                    .Append("Ext.getCmp('btnEndInput').setVisible(false);")
                    .Append("Ext.getCmp('lblState').setText('На рассмотрении');")
                    .Append(ExtNet.Msg.Alert("Уведомление", "Вариант успешно отправлен на рассмотрение.").ToScript());

                return new AjaxResult { Script = sb.ToString() };
            }
            catch (VariantProtocolException e)
            {
                return new AjaxResult
                {
                    Script = ExtNet.Msg.Alert("Ошибка", e.Message).ToScript()
                };
            }
        }

        [Transaction]
        public ActionResult Reject(int regionId, string comment)
        {
            var variantId = extension.Variant.Id;

            try
            {
                protocolService.Reject(variantId, regionId, comment);

                return new AjaxResult 
                {
                    Script = ExtNet.Msg.Alert("Уведомление", "Вариант успешно отклонен.").ToScript()
                };
            }
            catch (VariantProtocolException e)
            {
                return new AjaxResult { Script = ExtNet.Msg.Alert("Ошибка", e.Message).ToScript() };
            }
        }

        [Transaction]
        public ActionResult Accept(int regionId, string comment)
        {
            var variantId = extension.Variant.Id;

            try
            {
                protocolService.Accept(variantId, regionId, comment);

                return new AjaxResult { Script = ExtNet.Msg.Alert("Уведомление", "Вариант успешно утвержден.").ToScript() };
            }
            catch (VariantProtocolException e)
            {
                return new AjaxResult { Script = ExtNet.Msg.Alert("Уведомление", e.Message).ToScript() };
            }
        }
    }
}
