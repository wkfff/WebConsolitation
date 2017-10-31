using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.MrotSummary
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
            FormPanel centerPanel = new FormPanel { Border = false, AutoScroll = true, Layout = "Anchor" };
            
            centerPanel.Items.Add(CreateReportButton(
                "UnactedRegions", "Список АТЕ, от которых не поступили данные"));

            centerPanel.Items.Add(CreateReportButton(
                "Executers", "Перечень исполнителей"));

            centerPanel.Items.Add(CreateReportButton(
                "SubjectTrihedrData", "Отчет по всем муниципальным образованиям"));

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportMain", 
                                   Layout = "Center", 
                                   Items =
                                       {
                                           new BorderLayout
                                               {
                                                   ID = "borderLayoutMain",
                                                   Center = { Items = { centerPanel } }
                                               }
                                       }
                               }
                       };
        }

        private Button CreateReportButton(string reportCode, string reportCaption)
        {
            return new Button
            {
                ID = String.Format("export{0}Button", reportCode),
                Icon = Icon.PageExcel,
                Text = reportCaption,
                AnchorHorizontal = "100%",
                Height = 128,
                DirectEvents =
                {
                    Click =
                    {
                        Url = String.Format("/ExportMrot/{0}", reportCode),
                        IsUpload = true,
                        CleanRequest = true,
                        ExtraParams = { new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value) }
                    }
                }
            };
        }
    }
}
