using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningParamsView : View
    {
        private readonly IRepository<D_Forecast_PParams> paramsRepository;
        private readonly PlanningParamsGridControl grid;

        public PlanningParamsView(IRepository<D_Forecast_PParams> paramsRepository)
        {
            this.paramsRepository = paramsRepository;
            
            grid = new PlanningParamsGridControl();
        }
        
        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.Center.Items.Add(grid.Build(page));
            
            ////((RowSelectionModel)grid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain" };
            viewport.Items.Add(layout);

            ////grid.Store.DataBinding += StoreDataBinding;

            return new List<Component> { viewport };
        }

        /*private void StoreDataBinding(object sender, EventArgs e)
        {
            if (grid.Store.DataSource == null)
            {
                grid.Store.DataSource = paramsRepository.GetAll();
                grid.Store.DataBind();
            }
        }*/
    }
}
