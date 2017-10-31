using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Views
{
    public class ReportFormView : View
    {
        private readonly int measureId;

        public ReportFormView(int measureId)
        {
            this.measureId = measureId;
        }
        
        public override List<Component> Build(ViewPage page)
        {
            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportReportForm",
                        Items = { new BorderLayout { Center = { Items = { CreateReportGrid() } } } }
                    }
                };
        }

        private IEnumerable<Component> CreateReportGrid()
        {
            var formPanel = new FormPanel
                                {
                                    ID = "ReportForm",
                                    Border = false,
                                    CssClass = "x-window-mc",
                                    BodyCssClass = "x-window-mc",
                                    StyleSpec = "margin: 5px 5px 5px 5px;",
                                    Layout = "RowLayout",
                                    AutoHeight = true,
                                    Url = "/Measures/SaveReport?measureId={0}".FormatWith(measureId)
                                };

            formPanel.Attributes.Add("measureId", measureId.ToString());

            var fields = new FieldSet
            {
                ID = "reportFields",
                Collapsible = false,
                Collapsed = false,
                Layout = "form",
                Border = false,
                LabelSeparator = String.Empty,
                LabelAlign = LabelAlign.Left,
                LabelWidth = 100,
                MaxWidth = 500,
                Padding = 2,
                StyleSpec = "margin-top: 2px; margin-bottom: 2px;",
                DefaultAnchor = "0",
                Height = 300
            };

            fields.Add(new DateField
                           {
                               ID = "RefPeriod",
                               AllowBlank = false,
                               FieldLabel = @"Дата*"
                           });
            fields.Add(new TextArea
                           {
                               ID = "Report",
                               FieldLabel = @"Текст отчета",
                               Height = 150
                           });
            fields.Add(new DisplayField { Text = @"* - обязательное поле для заполнения" });

            formPanel.Add(fields);

            return new List<Component> { formPanel };
        }
    }
}
