using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using Ext.Net;
using Krista.FM.Common.Constants;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.DebtBook.Params;
using Krista.FM.ServerLibrary;
using GridView = Krista.FM.RIA.Core.Gui.GridView;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class ProtocolTransferView : GridView
    {
        private readonly IScheme scheme;

        public ProtocolTransferView(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var rm = page.Controls.OfType<HtmlForm>().First().Controls.OfType<ResourceManager>().First();
            rm.RegisterScript("Hack", "/Content/js/Ext.net.CommandColumn.Hack.js");

            var components = base.Build(page);

            GridPanel gridPanel = components.OfType<Viewport>().First()
                .Items.OfType<BorderLayout>().First()
                .Center.Items.OfType<GridPanel>().First();

            // Настраиваем столбцы
            var column = gridPanel.ColumnModel.GetColumnById("RefStatusSchb");
            column.Hidden = false;
            column.Width = 150;
            column.Groupable = false;
            int colIndex = gridPanel.ColumnModel.Columns.IndexOf(column);
            gridPanel.ColumnModel.MoveColumn(colIndex, 0);

            column = gridPanel.ColumnModel.GetColumnById("RefRegion");
            column.Hidden = false;
            column.Width = 100;

            column = gridPanel.ColumnModel.GetColumnById("CheckTransfer");
            column.Hidden = true;
            column.Groupable = false;

            column = gridPanel.ColumnModel.GetColumnById("Commentary");
            column.Hidden = false;
            column.Width = 400;
            column.Groupable = false;

            column = gridPanel.ColumnModel.GetColumnById("DataTransfer");
            column.Width = 140;
            column.Groupable = false;

            column = gridPanel.ColumnModel.GetColumnById("RefRegion");
            column.GroupRenderer.Handler = @"//function(value,metadata,record,rowIndex,colIndex,store)
var lastRow;
store.data.each(function(row){
    if (row.data.REFREGION == record.data.REFREGION){
        if (lastRow == undefined) lastRow = row;
        if (row.data.ID > lastRow.data.ID) lastRow = row;
    }
});
var icon;
if (lastRow.data.REFSTATUSSCHB == 1) icon = 'icon-flagwhite';
if (lastRow.data.REFSTATUSSCHB == 2) icon = 'icon-flagorange';
if (lastRow.data.REFSTATUSSCHB == 3) icon = 'icon-flaggreen';
return '<span style=""visibility: visible;"" class=""x-label""><img src=""/extjs/resources/images/default/s-gif/ext.axd"" class=""x-label-icon ' + icon + '""><span class=""x-label-value"">' + value + '</span></span>';
";

            CreateGroupCommands(gridPanel);

            // Задаем группировку по районам
            GroupingView groupingView = new GroupingView 
            { 
                ID = gridPanel.ID + "View", 
                HideGroupedColumn = true, 
                EnableNoGroups = true,
                StartCollapsed = true
            };

            // Задаем сортировку и поле группировки
            Store store = page.Controls.OfType<Store>().First(
                x => x.ID == "{0}Store".FormatWith(gridPanel.ID));
            store.GroupField = "LP_REFREGION";
            store.Sort("LP_REFREGION", SortDirection.ASC);
            
            // Размер страницы делаем заведомо большой, чтобы отключить пагинацию
            // т.к. будет некорректно работать группировка
            store.BaseParams["limit"] = "10000";
            ((PagingToolbar)gridPanel.TopBar[0]).PageSize = 10000;
            
            // Фильтруем данные по текущему варианту
            store.Listeners.BeforeLoad.Handler = @"options.params.systemGridFilters='{""f_100_field"":""REFVARIANT"",""f_100_data_type"":""numeric"",""f_100_data_comparison"":""eq"",""f_100_data_value"":""' + parent.Workbench.extensions.DebtBook.selectedVariantId + '""}'";

            gridPanel.View.Clear();
            gridPanel.View.Add(groupingView);

            // Удаляем с тулбара лишние кнопки
            var button = gridPanel.TopBar[0].Items.OfType<Button>().FirstOrDefault(
                x => x.ID == "{0}AddNewRowButton".FormatWith(gridPanel.ID));
            if (button != null)
            {
                button.Hidden = true;
            }

            button = gridPanel.TopBar[0].Items.OfType<Button>().FirstOrDefault(
                x => x.ID == "{0}DeleteRowButton".FormatWith(gridPanel.ID));
            if (button != null)
            {
                button.Hidden = true;
            }

            button = gridPanel.TopBar[0].Items.OfType<Button>().FirstOrDefault(
                x => x.ID == "{0}SaveButton".FormatWith(gridPanel.ID));
            if (button != null)
            {
                button.Hidden = true;
            } 

            // Добавляем кнопки действий "Утвердить/Отклонить"
            CreateActionButtons(gridPanel);
            
            return components;
        }

        private void CreateGroupCommands(GridPanel gridPanel)
        {
            CommandColumn commandColumn = new CommandColumn();

            commandColumn.GroupCommands.Add(new GridCommand
            {
                Icon = Icon.Report,
                CommandName = "Report",
                Text = "Отчет"
            });

            commandColumn.GroupCommands.Add(new GridCommand
            { 
                Icon = Icon.Accept, 
                CommandName = "Accept",
                Text = "Утвердить"
            });

            commandColumn.GroupCommands.Add(new GridCommand 
            {
                Icon = Icon.Decline,
                CommandName = "Reject",
                Text = "Отклонить"
            });

            commandColumn.PrepareGroupToolbar.Handler = @"
var lastRow = records[0];
for (var i = 0; i < records.length; i++){
    var row = records[i];
    if (lastRow == undefined) lastRow = row;
    if (row.data.ID > lastRow.data.ID) lastRow = row;
}
var icon;
try
{
if (lastRow.data.REFSTATUSSCHB == 1) {
    icon = 'icon-flagwhite';
}
if (lastRow.data.REFSTATUSSCHB == 2) {
    icon = 'icon-flagorange';
}
if (lastRow.data.REFSTATUSSCHB == 3) {
    icon = 'icon-flaggreen';
}

if (lastRow.data.REFSTATUSSCHB == 1) {
    toolbar.items.items[1].setVisible(false);
    toolbar.items.items[2].setVisible(false);
}
if (lastRow.data.REFSTATUSSCHB == 3) {
    toolbar.items.items[1].setVisible(false);
}
toolbar.doLayout();
}
catch(err)
{
}
";

            // Создаем обработчик нажания кнопки на тулбаре группы
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"
if(command === 'Accept' || command === 'Reject'){
    var regionId = records[0].data.REFREGION;
    Ext.net.DirectEvent.confirmRequest({
	    cleanRequest: true,
	    isUpload: false,
	    url: '/BebtBookStatus/' + command + '?regionId=' + regionId,
	    formProxyArg: 'Form1',
	    userSuccess: function(response, result, el, type, action, extraParams){
            ProtocolTransferStore.load();
        },
	    userFailure: function(response, result, el, type, action, extraParams){
              Ext.net.DirectEvent.showFailure(response);
	    },
	    control:this
    });
}");

            // Если нужно, то добавляем реакцию на нажатие кнопки отчета
            string reportId = String.Empty;
            var value = new OKTMOValueProvider(scheme).GetValue();
            if (value == OKTMO.Yaroslavl)
            {
                reportId = "Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtorBookYarReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports";
            }
            else if (value == OKTMO.Orenburg)
            {
                reportId = "Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtorBookOrenburgReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports";
            }
            else if (value == OKTMO.Vologda)
            {
                reportId = "Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtBookVologdaReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports";
            }
            else if (value == OKTMO.Moscow)
            {
                reportId = "Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtBookMoscowReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports";
            }
            else if (value == OKTMO.Stavropol)
            {
                reportId = "Krista.FM.Domain.Services.FinSourceDebtorBook.Reports.DebtBookStavropolReport, Krista.FM.Domain.Services.FinSourceDebtorBook.Reports";
            }

            sb.AppendLine(@"
if(command === 'Report'){{ 
    var regionId = records[0].data.REFREGION;
    var report = '{0}';
    if (report != ''){{
        Ext.net.DirectEvent.confirmRequest({{
	        cleanRequest: true,
	        isUpload: true,
	        url: '/BebtBookReports/Create?reportType=' + report + '&userId=-1&userRegion=' + regionId,
	        formProxyArg: 'Form1',
	        userSuccess: function(response, result, el, type, action, extraParams){{}},
	        userFailure: function(response, result, el, type, action, extraParams){{
                  Ext.net.DirectEvent.showFailure(response);
	        }},
	        control:this
        }});
   }} else {{
       Ext.net.Notification.show({{iconCls: 'icon-information', html: 'В соответствии с ОКТМО отчет не настроен.', title: 'Уведомление', hideDelay: 2500}}); 
   }}
   
}}".FormatWith(reportId));
            
            gridPanel.ColumnModel.Columns.Add(commandColumn);
            gridPanel.Listeners.GroupCommand.AddBefore(sb.ToString());
        }

        private void CreateActionButtons(GridPanel gridPanel)
        {
            gridPanel.TopBar[0].Items.Add(new Label { Icon = Icon.FlagWhite, Hidden = true });
            gridPanel.TopBar[0].Items.Add(new Label { Icon = Icon.FlagOrange, Hidden = true });
            gridPanel.TopBar[0].Items.Add(new Label { Icon = Icon.FlagGreen, Hidden = true });
        }
    }
}
