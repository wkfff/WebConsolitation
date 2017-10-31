using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class NewAnalReportControl : Control
    {
        private readonly IAuthService auth;

        public NewAnalReportControl(bool isToNavigationPanel = false)
        {
            auth = Resolver.Get<IAuthService>();
            Id = this.GetType().Name;
            IsToNavigationPanel = isToNavigationPanel;
        }

        public bool IsToNavigationPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            var reportDate = new DateField
                {
                    ID = Id + "reportDate",
                    FieldLabel = @"Дата отчета",
                    Width = 250,
                    MaxDate = DateTime.Now,
                    SelectedDate = DateTime.Now
                };

            var btnGrbs = GetButton(false);
            var btnPpo = GetButton(true);

            var win = new Window
                {
                    AutoRender = false,
                    ID = Id + "ReportWindow",
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
                                    ID = Id + "form",
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
                                            btnGrbs,
                                            btnPpo
                                        }
                                }
                        },
                };

            page.Controls.Add(win);

            const Icon ReportIcon = Icon.ReportPicture;
            const string ToolTip = @"Отчет в разрезе ППО и типов учреждений";
            var hidden = !(auth.IsAdmin() || auth.IsGrbsUser() || auth.IsPpoUser());
            var handler = "Ext.getCmp('{0}').show()".FormatWith(Id + "ReportWindow");
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
                    ID = Id + "ReportMenuItem",
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

            var reportBtn = new Button
                {
                    ID = Id + "ReportBtn",
                    Icon = ReportIcon,
                    ToolTip = ToolTip,
                    Hidden = hidden
                };
            reportBtn.Listeners.Click.Handler = handler;
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

        private Button GetButton(bool isPPO)
        {
            var btn = new Button
            {
                ID = isPPO ? Id + "btnPpo" : Id + "btnGrbs",
                Icon = Icon.ReportPicture,
                ToolTip = isPPO ? @"Отчет по ппо" : @"Отчет по грбс",
                Text = isPPO ? @"Отчет по ппо" : @"Отчет по грбс",
            };

            btn.DirectEvents.Click.Url = "/Reports/GetNewAnalReport";
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportDate", "{0}.getValue()".FormatWith(Id + "reportDate"), ParameterMode.Raw));
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("isPPO", isPPO.ToString().ToLower(), ParameterMode.Raw));
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
