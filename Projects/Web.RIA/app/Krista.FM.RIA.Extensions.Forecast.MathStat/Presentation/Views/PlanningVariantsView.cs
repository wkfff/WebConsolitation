using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningVariantsView : View
    {
        ////private readonly PlanningVariantsParamsGridControl paramsGrid;
        private readonly PlanningVariantsGridControl variantsGrid;
        private readonly IForecastExtension extension;

        /*private readonly IRepository<D_Forecast_PlanningVariants> varsRepository;
        private readonly IRepository<D_Forecast_PlanningParams> paramsRepository;*/

        public PlanningVariantsView(IForecastExtension extension/*IRepository<D_Forecast_PlanningVariants> varsRepository, IRepository<D_Forecast_PlanningParams> paramsRepository*/)
        {
            /*this.varsRepository = varsRepository;
            this.paramsRepository = paramsRepository;*/
            this.extension = extension;

            ////paramsGrid = new PlanningVariantsParamsGridControl("VarCont");
            variantsGrid = new PlanningVariantsGridControl(extension);
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            /*layout.West.Collapsible = true;
            layout.West.Split = true;
            layout.West.Items.Add(paramsGrid.Build(page));*/

            layout.North.Items.Add(CreateToolBar());

            Panel panelCenter = new Panel
            {
                AutoScroll = true,
                Border = false,
                Layout = "fit"
                ////AutoWidth = true,
                ////Width = layout.Width,
                ////Height = layout.Height
                ////AutoHeight = true
            };

            panelCenter.Items.Add(variantsGrid.Build(page));

            /*panelCenter.Width = layout.Width;
            panelCenter.Height = layout.Height;*/

            layout.Center.Items.Add(panelCenter);
            ////((RowSelectionModel)paramsGrid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);

           //// paramsGrid.Store.DataBinding += StoreDataBinding;

            return new List<Component> { viewport };
        }

        private Component CreateToolBar()
        {
            FormPanel toolbarPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 0,
                ////Width = 400,
                Collapsible = false,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "form"
            };

            Toolbar toolbar = new Toolbar
            {
                ID = "toolbar"
            };

            toolbarPanel.TopBar.Add(toolbar);

            Button btnAddNew = new Button
            {
                ID = "btnAddNew",
                Text = "Добавить прогноз",
                Icon = Icon.Add
            };

            btnAddNew.Listeners.Click.Handler = "wndAddForecast.show();";

            toolbar.Items.Add(btnAddNew);

            return toolbarPanel;
        }

       /* private void StoreDataBinding(object sender, EventArgs e)
        {
            if (paramsGrid.Store.DataSource == null)
            {
                paramsGrid.Store.DataSource = paramsRepository.GetAll();
                paramsGrid.Store.DataBind();
            }
        }*/
    }
}
