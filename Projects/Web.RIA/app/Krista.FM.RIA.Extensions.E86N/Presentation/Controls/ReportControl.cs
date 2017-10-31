using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Services.Documents;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public class ReportControl : Control
    {
        private readonly IDocuments documents;
        private readonly IAuthService auth;

        public ReportControl(bool isToNavigationPanel = false)
        {
            documents = Resolver.Get<IDocuments>();
            auth = Resolver.Get<IAuthService>();
            IsToNavigationPanel = isToNavigationPanel;
        }

        public bool IsToNavigationPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            var comboBox = new ComboBox { ID = "cb" };

            foreach (var item in documents.GetDocTypesList())
            {
                comboBox.Items.Add(new ListItem(item.Name, item.ID.ToString(CultureInfo.InvariantCulture)));
            }

            comboBox.Width = 250;
            comboBox.SelectedIndex = 0;
            comboBox.FieldLabel = @"Тип документа";
            comboBox.On(
                "beforerender",
                "function() { if (this.lastSelectionText == 'Паспорт') Ext.getCmp('cbYear').setDisabled(true); }");
            comboBox.On(
                "select",
                "function(combo, records) { Ext.getCmp('cbYear').setDisabled(records.data.text == 'Паспорт'); }");

            var comboBoxYear = new ComboBox { ID = "cbYear" };

            foreach (var item in documents.GetYears())
            {
                comboBoxYear.Items.Add(new ListItem(item));
            }

            comboBoxYear.Width = 250;
            comboBoxYear.SelectedIndex = comboBoxYear.Items.Count - 1;
            comboBoxYear.FieldLabel = @"Год документа";

            var reportDate = new DateField
                {
                    ID = "reportDate",
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
                    ID = "ReportWindow",
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
                                    ID = "form",
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
                                            comboBox,
                                            comboBoxYear,
                                            reportDate,
                                            btnGrbs,
                                            btnPpo
                                        }
                                }
                        },
                };

            page.Controls.Add(win);

            const Icon ReportIcon = Icon.ReportGo;
            const string ToolTip = @"Отчет по состояниям документов";
            var hidden = !(auth.IsAdmin() || auth.IsGrbsUser() || auth.IsPpoUser());
            const string Handler = @"Ext.getCmp('ReportWindow').show()";
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
                    ID = "ReportMenuItem",
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
                    ID = "ReportBtn",
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

        private Button GetButton(bool isPPO)
        {
            var btn = new Button
            {
                ID = isPPO ? "btnPpo" : "btnGrbs",
                Icon = Icon.ReportGo,
                ToolTip = isPPO ? @"Отчет по ппо" : @"Отчет по грбс",
                Text = isPPO ? @"Отчет по ппо" : @"Отчет по грбс",
            };

            btn.DirectEvents.Click.Url = UiBuilders.GetUrl<ReportsController>("GetDocReport");
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("typeDoc", "cb.getValue()", ParameterMode.Raw));
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("reportDate", "reportDate.getValue()", ParameterMode.Raw));
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("isPPO", isPPO.ToString().ToLower(), ParameterMode.Raw));
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("docYear", "cbYear.getValue()", ParameterMode.Raw));
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
