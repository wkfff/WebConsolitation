using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class BebtBookGridView : GridModelControl
    {
        private readonly IDebtBookExtension extension;
        private readonly IVariantProtocolService protocolService;

        public BebtBookGridView(IDebtBookExtension extension, IVariantProtocolService protocolService)
        {
            this.extension = extension;
            this.protocolService = protocolService;
            ControlRelationships = new List<ControlRelationship>();
        }

        /// <summary>
        /// Набор контрольных соотношений
        /// </summary>
        public IList<ControlRelationship> ControlRelationships { get; set; }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var variantId = extension.Variant.Id;
            var regionId = extension.UserRegionId;
            T_S_ProtocolTransfer status = protocolService.GetStatus(variantId, regionId);

            Readonly = status.RefStatusSchb.ID != 1
                && extension.UserRegionType != UserRegionType.Subject;

            AutoSaveOnDelete = true;
            ShowDataSourcesBox = false;

            var components = base.Build(page);

            // Добавляем ресурсы
            ResourceManager.GetInstance(page).RegisterStyle(
                "Show", "/Content/css/DebtBook/Show.css");
            ResourceManager.GetInstance(page).RegisterScript(
                "Extension.View", "/Content/js/Extension.View.js");
            ResourceManager.GetInstance(page).RegisterScript(
                "DebtBook.Show", "/Content/js/DebtBook.Show.js");
            
            // Добавляем окно создания отчетов
            var reportParamsWindow = CreateReportParamsWindow(Id);
            if (!page.Controls.OfType<Window>().Any(x => x.ID == reportParamsWindow.ID))
            {
                page.Controls.Add(reportParamsWindow);
            }

            var gridPanel = components.OfType<GridPanel>().First();

            // Удаляем с тулбара лишние кнопки
            var toolBar = gridPanel.TopBar[0]; 
            var button = toolBar.Items.OfType<Button>()
                .FirstOrDefault(x => x.ID == "{0}SaveButton".FormatWith(gridPanel.ID));
            if (button != null)
            {
                button.Hidden = true;
            }

            // Добавляем кнопку проверки контрольных соотношений при их наличии
            if (this.ControlRelationships.Count > 0)
            {
                var buttonCheckRelationships = new Button
                {
                    ID = "{0}CheckControlRelationshipsButton".FormatWith(gridPanel.ID),
                    Icon = Icon.Lightbulb,
                    Hidden = false,
                    ToolTip = "Проверить контрольные соотношения",
                    DirectEvents =
                    {
                        Click =
                        {
                            Url = "/BebtBookData/CheckControlRelationshipsForAll",
                            CleanRequest = true
                        }
                    }
                };

                var dataStore = page.Controls.OfType<Store>().Where(x => x.ID == "{0}Store".FormatWith(this.Id)).First();
                var parameter = dataStore.AutoLoadParams.GetParameter("serverFilter");
                if (parameter != null)
                {
                    parameter.Mode = ParameterMode.Raw;
                    buttonCheckRelationships.DirectEvents.Click.ExtraParams.Add(parameter);
                }

                buttonCheckRelationships.DirectEvents.Click.ExtraParams.Add(new Parameter("variantID", "parent.Workbench.extensions.DebtBook.selectedVariantId", ParameterMode.Raw));
                buttonCheckRelationships.DirectEvents.Click.ExtraParams.Add(new Parameter("sourceID", "parent.Workbench.extensions.DebtBook.currentSourceId", ParameterMode.Raw));
                buttonCheckRelationships.DirectEvents.Click.ExtraParams.Add(new Parameter("viewId", "document.location.pathname.split('/')[2]", ParameterMode.Raw));
                buttonCheckRelationships.DirectEvents.Click.ExtraParams.Add(new Parameter("tabId", this.Id, ParameterMode.Value));
                toolBar.Items.Add(buttonCheckRelationships);
            }

            return components;
        }

        protected override void CreateColumnModel(IEnumerable<IDataAttribute> attributes, ICollection<IEntityAssociation> associations)
        {
            // Системные и фиксированные поля не отображаются
            foreach (var attribute in attributes.Where(x =>
                x.Class != DataAttributeClassTypes.System
                && x.Class != DataAttributeClassTypes.Fixed))
            {
                var column = AddColumn(attribute);

                // В гриде видны поля у которых установлено свойство LookupType,
                // остальные скрыты
                column.Hidden = attribute.LookupType == LookupAttributeTypes.None;

                column.Hideable = attribute.Visible || !column.Hidden;

                if (attribute.Class == DataAttributeClassTypes.Reference)
                {
                    column.DataIndex = "LP_{0}".FormatWith(column.DataIndex);
                }

                Columns.Add(column);
            }
        }

        private static Window CreateReportParamsWindow(string ownerId)
        {
            Window window = new Window 
            {
                ID = "ReportParamsWindow",
                Icon = Icon.ApplicationFormEdit,
                Title = "Параметры отчета",
                Width = 600,
                Height = 480,
                Hidden = true,
                Modal = true,
                Constrain = true
            };

            window.AutoLoad.Url = "/";
            window.AutoLoad.Mode = LoadMode.IFrame;
            window.AutoLoad.TriggerEvent = "show";
            window.AutoLoad.ReloadOnEvent = true;
            window.AutoLoad.ShowMask = true;
            window.AutoLoad.MaskMsg = "Загрузка формы...";

            Button btnCreateReport = new Button
            {
                ID = "{0}btnCreateReport".FormatWith(ownerId),
                Text = "Создать",
                Enabled = false
            };

            btnCreateReport.DirectEvents.Click.Url = "/BebtBookReports/Create/";
            btnCreateReport.DirectEvents.Click.FormID = "Form1";
            btnCreateReport.DirectEvents.Click.IsUpload = true;
            btnCreateReport.DirectEvents.Click.CleanRequest = true;
            btnCreateReport.DirectEvents.Click.Success = "#{ReportParamsWindow}.hide();";
            btnCreateReport.DirectEvents.Click.Failure = @"
#{ReportParamsWindow}.hide();
Ext.Msg.show({
			    title:'Ошибка',
			    msg: response.responseText,
			    buttons: Ext.Msg.OK,
			    icon: Ext.MessageBox.ERROR,
			    maxWidth: 1000
		    });
";
            btnCreateReport.DirectEvents.Click.ExtraParams.Add(new Parameter(
                "reportType", 
                "#{ReportParamsWindow}.getBody().ReportParamsForm.getForm().getValues().reportType", 
                ParameterMode.Raw));
            btnCreateReport.DirectEvents.Click.ExtraParams.Add(new Parameter(
                "userId",
                "#{ReportParamsWindow}.getBody().ReportParamsForm.getForm().getValues().userId", 
                ParameterMode.Raw));
            btnCreateReport.DirectEvents.Click.ExtraParams.Add(new Parameter(
                "userRegion",
                "#{ReportParamsWindow}.getBody().ReportParamsForm.getForm().getValues().userRegion", 
                ParameterMode.Raw));

            Button btnCancel = new Button
            {
                ID = "{0}btnCancel".FormatWith(ownerId),
                Text = "Отмена",
                Enabled = false
            };
            btnCancel.Listeners.Click.Handler = "#{ReportParamsWindow}.hide();";

            window.Buttons.Add(btnCreateReport);
            window.Buttons.Add(btnCancel);

            return window;
        }
    }
}
