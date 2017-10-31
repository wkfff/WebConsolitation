using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    public class MarksView : View
    {
        private readonly string GridId = "MeasuresGrid";
        private readonly string PeriodComboId = "PeriodCombo";
        private readonly string RegionComboId = "RegionCombo";

        public override List<Component> Build(ViewPage page)
        {
            var periodsStore = CreatePeriodsStore();
            page.Controls.Add(periodsStore);

            var regionsStore = CreateRegionsStore();
            page.Controls.Add(regionsStore);

            var marksStore = CreateMarksStore();
            page.Controls.Add(marksStore);
            
            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMarksGKH",
                    AutoScroll = true,
                    Items =
                    {
                        new BorderLayout
                        {
                            Center =
                                {
                                    Items =
                                        {
                                            CreateMarksGridPanel(page, marksStore.ID, regionsStore.ID, periodsStore.ID)
                                        }
                                },
                            North =
                            {
                                Items =
                                {
                                    new DisplayField
                                    {
                                        Text = @"Форма ввода общих показателей для мониторинга деятельности организаций ЖКХ",
                                        StyleSpec = "font-size: 14px; padding-bottom: 5px; padding-top: 5px; font-weight: bold; text-align: left;"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private Store CreateMarksStore()
        {
            var ds = new Store
            {
                ID = "MarksStore",
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/Marks/Read")
                .SetJsonReader()
                .AddField("NameOrg")
                .AddField("OrgId")
                .AddField("Number")
                .AddField("RegionName")
                .AddField("INN")
                .AddField("Mark50000Id")
                .AddField("Mark60000Id")
                .AddField("Mark50000Value")
                .AddField("Mark60000Value");

            ds.Listeners.BeforeLoad.AddAfter(@"
                var period = {0}.value || {1}; 
                var region = {2}.value || {3}
                Ext.apply({4}.store.baseParams, {{ periodId: period, regionId: region }}); "
            .FormatWith(PeriodComboId, -1, RegionComboId, -1, GridId));

            var saveMarksUrl = "/Marks/Save";
            ds.Listeners.BeforeSave.AddAfter(@"
                var period = {0}.value || {1}; 
                var region = {2}.value || {3}
                {4}.store.updateProxy.conn.url = '{5}?regionId=' + region + '&periodId=' + period;"
            .FormatWith(PeriodComboId, -1, RegionComboId, -1, GridId, saveMarksUrl));

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = saveMarksUrl,
                Method = HttpMethod.POST
            });

            return ds;
        }

        private Store CreatePeriodsStore()
        {
            var ds = new Store
            {
                ID = "PeriodsStore",
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            ds.SetHttpProxy("/Periods/LookupMonthPeriod")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private Store CreateRegionsStore()
        {
            var ds = new Store
            {
                ID = "regionsStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy("/PlanProceeds/LookupRegions")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private GridPanel CreateMarksGridPanel(
            ViewPage page, 
            string marksStoreId, 
            string regionStoreId, 
            string periodStoreId)
        {
            var gp = new GridPanel
            {
                ID = GridId,
                Icon = Icon.Table,
                Closable = false,
                Frame = true,
                StoreID = marksStoreId,
                EnableColumnMove = false,
                SelectionModel = { new ExcelLikeSelectionModel() },
                ColumnLines = true,
                View = { new GridView() }
            };

            gp.AddRefreshButton();
            gp.AddSaveButton();

            gp.Toolbar().Add(new ToolbarSeparator());
            gp.Toolbar().Add(new ComboBox
                                 {
                                     ID = RegionComboId,
                                     AllowBlank = false,
                                     Editable = false,
                                     Width = 300,
                                     TriggerAction = TriggerAction.All,
                                     StoreID = regionStoreId,
                                     ValueField = "ID",
                                     EmptyText = @"Выберите территорию",
                                     DisplayField = "Name",
                                     FieldLabel = @"Территория",
                                     LabelWidth = 70,
                                     StyleSpec = "margin: 0px 0px 5px 0px;",
                                     Listeners =
                                         {
                                             Select =
                                                 {
                                                     Handler =
                                                         @"{0}.store.load();".
                                                         FormatWith(GridId)
                                                 }
                                         }
                                 });
            gp.Toolbar().Add(new ToolbarSeparator());
            gp.Toolbar().Add(new ComboBox
                                 {
                                     ID = PeriodComboId,
                                     AllowBlank = false,
                                     Width = 300,
                                     TriggerAction = TriggerAction.All,
                                     StoreID = periodStoreId,
                                     ValueField = "ID",
                                     Editable = false,
                                     EmptyText = @"Выберите период",
                                     DisplayField = "Name",
                                     FieldLabel = @"Период",
                                     StyleSpec = "margin: 0px 0px 5px 0px;",
                                     LabelWidth = 50,
                                     Value = (DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100),
                                     Listeners =
                                     {
                                         Select =
                                         {
                                             Handler =
                                                 @"{0}.store.load();".
                                                 FormatWith(GridId)
                                         }
                                     }
                                 });

            var columnN = gp.ColumnModel.AddColumn("Number", "№", DataAttributeTypes.dtString);
            columnN.SetWidth(50);
            columnN.Sortable = true;
            columnN.Hideable = false;

            var columnName = gp.ColumnModel
                .AddColumn("NameOrg", "Наименование организации ЖКХ", DataAttributeTypes.dtString);
            columnName.SetWidth(300);
            columnName.Sortable = true;
            columnName.Hideable = false;

            var columnINN = gp.ColumnModel
                .AddColumn("INN", "ИНН", DataAttributeTypes.dtString);
            columnINN.SetWidth(150);
            columnINN.Sortable = true;
            columnINN.Hideable = false;

            var columnRegion = gp.ColumnModel
                .AddColumn("RegionName", "Муниципальное образование", DataAttributeTypes.dtString);
            columnRegion.SetWidth(200);
            columnRegion.Sortable = true;
            columnRegion.Hideable = false;

            var column5 = gp.ColumnModel
                .AddColumn("Mark50000Value", "Установленный тариф, рублей/1 Гкал", DataAttributeTypes.dtString);
            column5.SetWidth(170).SetEditableDouble(2);
            column5.Sortable = true;
            column5.Hideable = false;

            var column6 = gp.ColumnModel
                .AddColumn("Mark60000Value", "Топливная составляющая в тарифе, %", DataAttributeTypes.dtString);
            column6.SetWidth(170).SetEditableDouble(2);
            column6.Sortable = true;
            column6.Hideable = false;
            
            gp.AddColumnsWrapStylesToPage(page);
            gp.AddColumnsHeaderAlignStylesToPage(page);
            return gp;
        }
    }
}
