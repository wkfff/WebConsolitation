using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Models.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class ChangeLogView : View
    {
        private readonly ChangeLogModel model = new ChangeLogModel();

        public override List<Component> Build(ViewPage page)
        {
            var gp = new ModelGridView
                {
                    AutoLoad = true, 
                    Model = model, 
                    ActionController = typeof(ChangeLogController), 
                    PageSize = 100, 
                    ReadOnly = true
                };
            
            gp.Grid.AddRefreshButton();

            var excelBtn = new Button
            {
                ID = "excel",
                Icon = Icon.PageExcel,
                ToolTip = @"Выгрузить в excel"
            };
            excelBtn.DirectEvents.Click.Url = UiBuilders.GetUrl<ReportsController>("ImportChangeLogGrid");
            excelBtn.DirectEvents.Click.CleanRequest = true;
            excelBtn.DirectEvents.Click.IsUpload = true;
            excelBtn.DirectEvents.Click.FormID = "Form1";
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("state", string.Concat(gp.Grid.ID, ".getState()"), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("gridFilters", string.Format("{0}.filters.buildQuery({0}.filters.getFilterData())", gp.Grid.ID), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: result.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";

            gp.Grid.Toolbar().Add(excelBtn);

            var coponent = gp.Build(page);

            GridFilterCollection filters = ((GridFilters)((GridPanel)coponent[0]).Plugins[0]).Filters;

            var filter = filters.First(x => x.DataIndex.Equals(UiBuilders.NameOf(() => model.Time)));

            filters.Remove(filter);

            return new List<Component> { new Viewport { Items = { new FitLayout { Items = { coponent } } } } };
        }
    }
}
