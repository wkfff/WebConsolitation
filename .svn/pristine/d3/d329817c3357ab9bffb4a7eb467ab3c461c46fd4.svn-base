using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Services.Documents;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    using Krista.FM.Extensions;

    // todo можно зарефакторить данный контрол, вынести часть в общий предоки наследовать для каждого отчета
    public class MonitoringPlacementInfoReportControl : Control
    {
        public const string Title = @"Мониторинг размещения сведений";
        public const string ID = "MonitoringPlacementInfoReport";

        private readonly IDocuments documents;
        private readonly IAuthService auth;
       
        public MonitoringPlacementInfoReportControl()
        {
            documents = Resolver.Get<IDocuments>();
            auth = Resolver.Get<IAuthService>();
        }

        public override List<Component> Build(ViewPage page)
        {
            var comboBoxYear = new ComboBox { ID = "cbYear{0}".FormatWith(ID) };

            foreach (var item in documents.GetYears())
            {
                comboBoxYear.Items.Add(new ListItem(item));
            }

            comboBoxYear.Width = 250;
            comboBoxYear.SelectedIndex = comboBoxYear.Items.Count - 1;
            comboBoxYear.FieldLabel = @"Год документа";

            var reportDate = new DateField
                {
                    ID = "reportDate{0}".FormatWith(ID),
                    FieldLabel = @"Дата отчета",
                    Width = 250,
                    MaxDate = DateTime.Now,
                    SelectedDate = DateTime.Now
                };
            
            var win = new Window
                {
                    AutoRender = false,
                    ID = "{0}Window".FormatWith(ID),
                    Hidden = true,
                    Width = 320,
                    Height = 320,
                    Modal = true,
                    Title = Title,
                    BodyCssClass = "x-window-mc",
                    CssClass = "x-window-mc",
                    Border = false,
                    Resizable = false,
                    MonitorResize = true,
                    Padding = 10,
                    Layout = LayoutType.Fit.ToString(),
                    AutoHeight = true,
                    Items =
                        {
                            new FormPanel
                                {
                                    ID = "form{0}".FormatWith(ID),
                                    MonitorValid = true,
                                    Layout = LayoutType.Form.ToString(),
                                    ButtonAlign = Alignment.Right,
                                    LabelAlign = LabelAlign.Top,
                                    Border = false,
                                    BodyCssClass = "x-window-mc",
                                    CssClass = "x-window-mc",
                                    LabelWidth = 200,
                                    DefaultAnchor = "99%",
                                    AutoHeight = true,
                                    BodyStyle = "padding:10px",
                                    Items =
                                        {
                                            comboBoxYear,
                                            reportDate,
                                            GetButton()
                                        }
                                }
                        }
                };

            page.Controls.Add(win);

            const Icon ReportIcon = Icon.ReportGo;
            const string ToolTip = Title;
            var hidden = !(auth.IsAdmin() || auth.IsGrbsUser() || auth.IsPpoUser());
            string handler = "Ext.getCmp('{0}Window').show()".FormatWith(ID);
            const string FormID = "Form1";
            const bool CleanRequest = true;
            const bool IsUpload = true;
            const string Failure = @"Ext.Msg.show({
                                                      title:'Ошибка',
                                                      msg: response.responseText,
                                                      buttons: Ext.Msg.OK,
                                                      icon: Ext.MessageBox.ERROR,
                                                      maxWidth: 1000
                                                   });";
           
            var reportMenuItem = new MenuItem
            {
                ID = "{0}MenuItem".FormatWith(ID),
                Icon = ReportIcon,
                ToolTip = ToolTip,
                Hidden = hidden,
                Text = ToolTip
            };
            reportMenuItem.Listeners.Click.Handler = handler;
            reportMenuItem.DirectEvents.Click.CleanRequest = CleanRequest;
            reportMenuItem.DirectEvents.Click.IsUpload = IsUpload;
            reportMenuItem.DirectEvents.Click.FormID = FormID;
            reportMenuItem.DirectEvents.Click.Failure = Failure;
            return new List<Component> { reportMenuItem };
        }

        public Component BuildComponent(ViewPage page)
        {
            return Build(page)[0];
        }

        private Button GetButton()
        {
            var btn = new Button
            {
                ID = "btnGo{0}".FormatWith(ID),
                Icon = Icon.ReportGo,
                ToolTip = @"Мониторинг размещения сведений",
                Text = @"Построить отчет"
            };

            btn.DirectEvents.Click.Url = UiBuilders.GetUrl<ReportsController>("GetMonitoringPlacementInfoReport");
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportDate", "reportDate{0}.getValue()".FormatWith(ID), ParameterMode.Raw));
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("docYear", "cbYear{0}.getValue()".FormatWith(ID), ParameterMode.Raw));
            
            // todo стоило бы сделать общий обработчик ошибок для всех кнопок или всех контролов
            btn.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                        title:'Ошибка',
                                                        msg: response.responseText,
                                                        buttons: Ext.Msg.OK,
                                                        icon: Ext.MessageBox.ERROR,
                                                        maxWidth: 1000
                                                    });";

            return btn;
        }
    }
}
