using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Consolidated
{
    public class FormView : View
    {
        private int TaskId
        {
            get
            {
                return Convert.ToInt32(Params["taskId"]);
            }
        }

        public override List<Component> Build(ViewPage page)
        {
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FitLayout { Items = { CreateFormPanel() } }
                }
            };

            return new List<Component> { view };
        }

        private List<Component> CreateFormPanel()
        {
            var form = new FormPanel { Border = false, AutoScroll = true, Layout = LayoutType.Anchor.ToString() };

            form.Items.Add(new Button 
            {
                ID = "btReportGas",
                Icon = Icon.PageExcel,
                Text = "Отчет по ГСМ",
                AnchorHorizontal = "100%",
                Height = 128,
                DirectEvents =
                {
                    Click =
                    {
                        Url = String.Format("/Org3GasolineCons/ExportGas"),
                        IsUpload = true,
                        CleanRequest = true,
                        ExtraParams = { new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value) }
                    }
                }
            });

            form.Items.Add(new Button
            {
                ID = "btReportFood",
                Icon = Icon.PageExcel,
                Text = "Отчет по продуктам питания",
                AnchorHorizontal = "100%",
                Height = 128,
                DirectEvents =
                {
                    Click =
                    {
                        Url = String.Format("/Org3GasolineCons/ExportFood"),
                        IsUpload = true,
                        CleanRequest = true,
                        ExtraParams = { new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value) }
                    }
                }
            });
            
            return new List<Component> { form };
        }
    }
}
