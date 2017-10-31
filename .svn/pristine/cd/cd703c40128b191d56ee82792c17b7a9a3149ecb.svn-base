using System;
using System.Collections.Generic;
using System.Text;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.DebtBook.Services.DAL;
using Krista.FM.RIA.Extensions.DebtBook.Services.Note;
using Microsoft.Practices.Unity;
using Icon = Ext.Net.Icon;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class DebtBookExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private DebtBookExtension extension;

        public DebtBookExtensionInstaller()
            : base(typeof(DebtBookExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.DebtBook.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            Navigation navigation = base.ConfigureNavigation(parametersService);

            Panel wrapperPanel = new Panel { Height = 160, Border = false, Layout = "Anchor" };

            // Для субъекта панель управления состоянием сбора не показываем
            if (extension.UserRegionType != UserRegionType.Subject)
            {
                // Панель управления состоянием сбора
                wrapperPanel.Items.Add(CreateRegionVariantStatePanel());
            }

            // Панель варианта
            wrapperPanel.Items.Add(CreateVariantSelectPanel());
            
            navigation.ButtomBar = new List<Component> { wrapperPanel };

            navigationService.AddNavigation(navigation);
        }

        public override void ConfigureWindows(WindowService windowService)
        {
            windowService.AddWindow(GetSelectVariantWindow());

            windowService.AddWindow(GetReportParamsWindow());

            if (extension.UserRegionType == UserRegionType.Subject)
            {
                windowService.AddWindow(GetMinfinExportParamsWindow());
            }
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            var protocolRepository = new NHibernateLinqRepository<T_S_ProtocolTransfer>();

            extension = new DebtBookExtension(
                                                SchemeAccessor.GetScheme(), 
                                                new RegionsAccordanceService(),
                                                new VariantService(), 
                                                new RegionsService(), 
                                                new DataSourceService(), 
                                                protocolRepository);

            Resolver.RegisterInstance<IDebtBookExtension>(extension, LifetimeManagerType.Session);

            if (extension.Initialize())
            {
                StringBuilder clientScript = new StringBuilder();
                clientScript.AppendLine(@"
Workbench.extensions.DebtBook = {{
    name:'Долговая книга',
    version:'{0}',
    selectedVariantId: {1},
    currentSourceId: {2},
    currentAnalysisSourceId: {3},
    userRegionId: {4},
    userRegionName: '{5}',
    userRegionType: {6},
    userYear: {7},
    statusSchbId: {8},
    setVariantId: function(record){{
        Workbench.extensions.DebtBook.selectedVariantId = record.id;
        lblVariantCode.setText('Код ' + record.data.CODE);
        lblVariantDate.setText('Отчетная дата ' + Date.parseDate(record.data.REPORTDATE, 'd.m.Y').format('d.m.Y'));
        lblVariantCaption.setText(record.data.NAME);
        if (window.CopyVariantVologda != undefined){{
            window.CopyVariantVologda.setDisabled(record.get('VARIANTCOMPLETED') == 1);
        }}
    }}
}};"
                                            .FormatWith(
                                                "1.0",
                                                extension.Variant.Id,
                                                extension.CurrentSourceId,
                                                extension.CurrentAnalysisSourceId,
                                                extension.UserRegionId,
                                                extension.UserRegionName,
                                                (int)extension.UserRegionType,
                                                extension.UserYear,
                                                extension.StatusSchb.RefStatusSchb.ID));

                clientScript.AppendLine("lblVariantCode.setText('Код {0}');"
                                            .FormatWith(extension.Variant.Code));

                clientScript.AppendLine("lblVariantDate.setText('Отчетная дата {0:dd.MM.yyyy}');"
                                            .FormatWith(extension.Variant.ReportDate));

                clientScript.AppendLine("lblVariantCaption.setText('{0}');"
                                            .FormatWith(extension.Variant.Name));

                clientExtensionService.AddClientExtension(clientScript.ToString());
            }
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            container.RegisterType<IDebtBookNoteService, DebtBookNoteService>();
            container.RegisterType<IObjectRepository, ObjectRepository>();
        }

        /// <summary>
        /// Окно параметров выгрузки данных в Минфин.
        /// </summary>
        private static Window GetMinfinExportParamsWindow()
        {
            var minfinExportParamsWindow = new Window
                                               {
                                                   ID = "DebtBookMinfinExportParamsWindow",
                                                   Title = "Параметры выгрузки",
                                                   Width = 240,
                                                   Height = 105,
                                                   Hidden = true,
                                                   Icon = Icon.ApplicationFormEdit,
                                               };

            FormPanel panel = new FormPanel
                                  {
                                      ID = "DebtBookMinfinExportForm",
                                      Padding = 10,
                                      Border = false
                                  };

            panel.Items.Add(new DateField
                                {
                                    ID = "DebtBookMinfinExportDate",
                                    Value = DateTime.Today,
                                    FieldLabel = "Дата",
                                });

            minfinExportParamsWindow.Items.Add(panel);

            var btnExport = new Button
                                {
                                    ID = "DebtBookMinfinExport",
                                    Text = "Экспор",
                                };
            btnExport.DirectEvents.Click.Url = "/BebtBookReports/MinfinExport/";
            btnExport.DirectEvents.Click.IsUpload = true;
            btnExport.DirectEvents.Click.CleanRequest = true;
            btnExport.DirectEvents.Click.Success = "DebtBookMinfinExportParamsWindow.hide();";
            btnExport.DirectEvents.Click.Failure = @"
Ext.Msg.show({
   title:'Ошибка',
   msg: response.responseText,
   buttons: Ext.Msg.OK,
   icon: Ext.MessageBox.ERROR,
   maxWidth: 1000
});
DebtBookMinfinExportParamsWindow.hide();";
            btnExport.DirectEvents.Click.ExtraParams.Add(new Parameter
                                                             {
                                                                 Name = "exportDate",
                                                                 Value = "DebtBookMinfinExportForm.getForm().getValues().DebtBookMinfinExportDate",
                                                                 Mode = ParameterMode.Raw
                                                             });
            minfinExportParamsWindow.Buttons.Add(btnExport);

            var btnCancelExport = new Button
                                      {
                                          ID = "DebtBookCancelExport",
                                          Text = "Отмена",
                                          Handler = "function() { DebtBookMinfinExportParamsWindow.hide(); }"
                                      };
            minfinExportParamsWindow.Buttons.Add(btnCancelExport);
            return minfinExportParamsWindow;
        }

        /// <summary>
        /// Окно выбора параметров отчета.
        /// </summary>
        private static Window GetReportParamsWindow()
        {
            var reportParamsWindow = new Window 
            {
                ID = "DebtBookReportParamsWindow",
                Title = "Параметры отчета",
                Width = 600,
                Height = 480,
                Hidden = true,
                Icon = Icon.ApplicationFormEdit,
            };

            reportParamsWindow.AutoLoad.ShowMask = true;
            reportParamsWindow.AutoLoad.ReloadOnEvent = true;
            reportParamsWindow.AutoLoad.TriggerEvent = "show";
            reportParamsWindow.AutoLoad.Url = "/BebtBookReports/ShowParams?report={0}";
            reportParamsWindow.AutoLoad.Mode = LoadMode.IFrame;

            var btnCreateReport = new Button 
            {
                ID = "DebtBookCreateReport",
                Text = "Создать",
            };
            btnCreateReport.DirectEvents.Click.Url = "/BebtBookReports/Create/";
            btnCreateReport.DirectEvents.Click.FormID = "form1";
            btnCreateReport.DirectEvents.Click.IsUpload = true;
            btnCreateReport.DirectEvents.Click.CleanRequest = true;
            btnCreateReport.DirectEvents.Click.Success = "DebtBookReportParamsWindow.hide();";
            btnCreateReport.DirectEvents.Click.Failure = @"//response, result, el, type, action, extraParams
DebtBookReportParamsWindow.hide();
Ext.Msg.show({
			    title:'Ошибка',
			    msg: response.responseText,
			    buttons: Ext.Msg.OK,
			    icon: Ext.MessageBox.ERROR,
			    maxWidth: 1000
		    });
";
            btnCreateReport.DirectEvents.Click.ExtraParams.Add(
                new Parameter 
                {
                    Name = "reportType",
                    Value = "DebtBookReportParamsWindow.getBody().ReportParamsForm.getForm().getValues().reportType",
                    Mode = ParameterMode.Raw
                });
            btnCreateReport.DirectEvents.Click.ExtraParams.Add(
                new Parameter 
                {
                    Name = "userId",
                    Value = "DebtBookReportParamsWindow.getBody().ReportParamsForm.getForm().getValues().userId",
                    Mode = ParameterMode.Raw
                });
            reportParamsWindow.Buttons.Add(btnCreateReport);

            var btnCancelReport = new Button 
            {
                ID = "DebtBookCancelReport",
                Text = "Отмена",
                Handler = "function() { DebtBookReportParamsWindow.hide(); }"
            };
            reportParamsWindow.Buttons.Add(btnCancelReport);
            return reportParamsWindow;
        }

        /// <summary>
        /// Окно выбора варианта.
        /// </summary>
        private static Window GetSelectVariantWindow()
        {
            var w = new Window();

            w.ID = "DebtBookVariantWindow";
            w.Title = "Выбор варианта";
            w.Width = 600;
            w.Height = 400;
            w.Hidden = true;

            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.Url = "/View/DebtBookVariantSelect";
            w.AutoLoad.Mode = LoadMode.IFrame;

            var btn = new Button { ID = "DebtBookVariantSelectButton", Text = "Выбрать", Disabled = true };
            btn.Listeners.Click.Handler = @"
Workbench.extensions.DebtBook.setVariantId(DebtBookVariantWindow.getBody().Extension.entityBook.selectedRecord);
Ext.net.DirectEvent.confirmRequest({
		cleanRequest: true,
		url: '/BebtBookVariant/SetCurrentVariant/',
		extraParams: {variantId:Workbench.extensions.DebtBook.selectedVariantId},
		eventMask: {showMask: true, msg: 'Установка текущего варианта'},
		userFailure: function(response, result, el, type, action, extraParams){
			if (result.result){
				alert('Ошибка: ' + result.result);
			} else {
				Ext.Msg.show({
					title:'Ошибка',
					msg: result.errorMessage,
					buttons: Ext.Msg.OK,
					icon: Ext.MessageBox.ERROR,
					maxWidth: 1000
				});
			}
		},
		control:this
	});
#{DebtBookVariantWindow}.hide();";
            w.Buttons.Add(btn);

            // Установка обработчика выбора варианта
            w.Listeners.Update.Handler = @"
function rowSelected(record) { 
    DebtBookVariantWindow.getBody().Extension.entityBook.selectedRecord = record;
    DebtBookVariantSelectButton.setDisabled(false); 
}
DebtBookVariantWindow.getBody().Extension.entityBook.onRowSelect = rowSelected;
";

            w.AddAfterClientInitScript("DebtBookVariantWindow.MetaData = {};");
            return w;
        }

        private static Panel CreateVariantSelectPanel()
        {
            Panel variantPanel = new Panel
            {
                ID = "DebtBookVariantPanel",
                Border = false,
                Height = 100,
                AnchorVertical = "50%"
            };

            var toolbar = new Toolbar();
            toolbar.Items.Add(new Label("Текущий вариант") { Cls = "toolbar-label" });

            variantPanel.TopBar.Add(toolbar);

            variantPanel.Items.Add(new Label { ID = "lblVariantCode", Cls = "label" });
            variantPanel.Items.Add(new Label { ID = "lblVariantDate", Cls = "label" });
            variantPanel.Items.Add(new Label { ID = "lblVariantCaption", Cls = "label" });

            variantPanel.Items.Add(new Button
            {
                ID = "btnSelectVariant",
                Text = "Выбрать вариант",
                Icon = Icon.ApplicationFormEdit,
                Handler = "function(){DebtBookVariantWindow.show();}"
            });

            variantPanel.Listeners.Resize.Handler =
                "if (btnSelectVariant.getEl() != undefined){ btnSelectVariant.getEl().setWidth(DebtBookVariantPanel.getWidth());}";
            return variantPanel;
        }

        private Panel CreateRegionVariantStatePanel()
        {
            var cls = extension.HighlightColor ? "greenlabel" : "label";

            Panel statePanel = new Panel
            {
                ID = "DebtBookStatePanel",
                Border = false,
                Height = 58,
                AnchorVertical = "50%"
            };

            if (extension.HighlightColor)
            {
                statePanel.BodyStyle = "background-color: #98FB98";
            }

            var stateToolbar = new Toolbar();
            stateToolbar.Items.Add(new Label("Статус сбора") { Cls = "toolbar-label" });
            statePanel.TopBar.Add(stateToolbar);

            statePanel.Items.Add(new Label
            {
                ID = "lblState",
                Cls = cls,
                Html = extension.StatusSchbText
            });

            // Если вариант на редактировании, то для района показываем кнопку действия
            if (extension.StatusSchb.RefStatusSchb.ID == 1)
            {
                Button button = new Button
                {
                    ID = "btnEndInput",
                    Text = "Завершить ввод",
                    Icon = Icon.StopRed
                };
                button.DirectEvents.Click.Url = "/BebtBookStatus/ToTest";
                button.DirectEvents.Click.CleanRequest = true;

                statePanel.Items.Add(button);
                statePanel.Listeners.Resize.Handler =
                    "if (btnEndInput.getEl() != undefined){ btnEndInput.getEl().setWidth(DebtBookStatePanel.getWidth());}";
            }

            return statePanel;
        }
    }
}
