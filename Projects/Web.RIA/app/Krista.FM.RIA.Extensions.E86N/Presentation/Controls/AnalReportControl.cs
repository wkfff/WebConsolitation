using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class AnalReportControl : Control
    {
        private readonly IAuthService auth;

        public AnalReportControl(bool isToNavigationPanel = false)
        {
            auth = Resolver.Get<IAuthService>();
            IsToNavigationPanel = isToNavigationPanel;
        }

        public bool IsToNavigationPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            var reportDate = new DateField
                {
                    ID = "analReportDate",
                    FieldLabel = @"Дата отчета",
                    Width = 250,
                    MaxDate = DateTime.Now,
                    SelectedDate = DateTime.Now
                };

            var btn = new Button
            {
                ID = "analReportStartBtn",
                Icon = Icon.ReportGo,
                ToolTip = @"Сформировать отчет",
                Text = @"Сформировать отчет"
            };

            btn.DirectEvents.Click.Url = "/Reports/GetAnalReport";
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportDate", "analReportDate.getValue()", ParameterMode.Raw));
            btn.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                        title:'Ошибка',
                                                        msg: response.responseText,
                                                        buttons: Ext.Msg.OK,
                                                        icon: Ext.MessageBox.ERROR,
                                                        maxWidth: 1000
                                                    });";

            var win = new Window
                {
                    AutoRender = false,
                    ID = "AnalReportWindow",
                    Hidden = true,
                    Width = 320,
                    Height = 320,
                    Modal = true,
                    Title = @"Отчет",
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
                                    ID = "AnalForm",
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
                                            reportDate,
                                            btn
                                        }
                                }
                        },
                };

            page.Controls.Add(win);

            const Icon ReportIcon = Icon.Report;
            const string ToolTip = @"Аналитический отчет в разрезе видов деятельности и ППО";
            var hidden = !auth.IsAdmin();
            const string Handler = @"Ext.getCmp('AnalReportWindow').show()";
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

            if (IsToNavigationPanel)
            {
                var reportMenuItem = new MenuItem
                {
                    ID = "AnalReportMenuItem",
                    Icon = ReportIcon,
                    ToolTip = ToolTip,
                    Hidden = hidden,
                    Text = ToolTip
                };
                reportMenuItem.Listeners.Click.Handler = Handler;
                reportMenuItem.DirectEvents.Click.CleanRequest = CleanRequest;
                reportMenuItem.DirectEvents.Click.IsUpload = IsUpload;
                reportMenuItem.DirectEvents.Click.FormID = FormID;
                reportMenuItem.DirectEvents.Click.Failure = Failure;

                return new List<Component> { reportMenuItem };
            }

            var reportBtn = new Button
                {
                    ID = "AnalReportBtn",
                    Icon = ReportIcon,
                    ToolTip = ToolTip,
                    Hidden = hidden
                };
            reportBtn.Listeners.Click.Handler = Handler;
            reportBtn.DirectEvents.Click.CleanRequest = CleanRequest;
            reportBtn.DirectEvents.Click.IsUpload = IsUpload;
            reportBtn.DirectEvents.Click.FormID = FormID;
            reportBtn.DirectEvents.Click.Failure = Failure;

            return new List<Component> { reportBtn };
        }

        public Component BuildComponent(ViewPage page)
        {
            return Build(page)[0];
        }
    }
}
