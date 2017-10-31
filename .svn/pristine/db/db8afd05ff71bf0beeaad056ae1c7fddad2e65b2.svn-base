using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class OivUsersView : View
    {
        private readonly IRepository<D_OMSU_ResponsOIV> oivRepository;
        private readonly ResponsOivGridControl grid;
        private readonly UsersGridControl usersGrid;

        public OivUsersView(IRepository<D_OMSU_ResponsOIV> oivRepository, UsersGridControl gridControl)
        {
            this.oivRepository = oivRepository;
            
            grid = new ResponsOivGridControl();
            usersGrid = gridControl;
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.Center.Items.Add(grid.Build(page));
            layout.South.Items.Add(usersGrid.Build(page));

            ((RowSelectionModel)grid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain" };
            viewport.Items.Add(layout);

            grid.Store.DataBinding += StoreDataBinding;

            return new List<Component> { viewport };
        }

        private void StoreDataBinding(object sender, EventArgs e)
        {
            if (grid.Store.DataSource == null)
            {
                grid.Store.DataSource = oivRepository.GetAll();
                grid.Store.DataBind();
            }
        }
    }
}
