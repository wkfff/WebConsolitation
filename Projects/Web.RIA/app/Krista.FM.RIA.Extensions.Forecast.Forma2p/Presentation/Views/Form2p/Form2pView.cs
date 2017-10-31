using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pView : View
    {
        private readonly IRepository<D_Forecast_Forma2p> paramsRepository;
        private readonly Form2pParamsGridControl grid;

        public Form2pView(IRepository<D_Forecast_Forma2p> paramsRepository)
        {
            this.paramsRepository = paramsRepository;
            grid = new Form2pParamsGridControl();
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            layout.North.Items.Add(grid.TopPanel(page)); // panel inside

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false,
                ////AutoWidth = true,
                Width = layout.Width,
                Height = layout.Height,
                ////AutoHeight = true,
                ////AutoWidth = true, 
                Layout = "fit"
            };

            panel.Items.Add(grid.Build(page));
            
            layout.Center.Items.Add(panel);
            
            ////((RowSelectionModel)grid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);

           //// grid.Store.DataBinding += StoreDataBinding;

            return new List<Component> { viewport };
        }

       /* private void StoreDataBinding(object sender, EventArgs e)
        {
            if (grid.Store.DataSource == null)
            {
                grid.Store.DataSource = paramsRepository.GetAll();
                grid.Store.DataBind();
            }
        }*/
    }
}
