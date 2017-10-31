using System.Collections.Generic;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public sealed class ConsolidationPumpBtn : Control
    {
        public const string Scope = "E86n.View.AnnualBalanceView";

        public ConsolidationPumpBtn(int recId)
        {
            RecId = recId;
        }

        public int RecId { get; set; }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var consPumpBtn = new Button
            {
                ID = "consPumpBtn",
                Icon = Icon.Wand,
                ToolTip = @"Закачка с консолидации",
                Text = @"Импорт из консолидации",
                DirectEvents =
                {
                    Click =
                    {
                        Url = UiBuilders.GetUrl<ImportsController>(
                            "ImportsConsolidation",
                            new Dictionary<string, object>
                                    {
                                        { "docId", RecId }
                                    }),
                        CleanRequest = true,
                        Timeout = 600000,
                        IsUpload = true,
                        Before = string.Concat(Scope, ".PumpMask.show();"),
                        Success = Scope + @".PumpMask.hide();
                                    Ext.net.Notification.show({
                                                                iconCls : 'icon-information',
                                                                html : 'Выполнено',
                                                                title : 'Импорт',
                                                                hideDelay  : 2000
                                                            });",

                        Failure = string.Concat(Scope, ".PumpMask.hide(); Ext.Msg.alert('Ошибка закачки', result.extraParams.responseText);")
                    }
                }
            };

            return new List<Component> { consPumpBtn };
        }
    }
}
