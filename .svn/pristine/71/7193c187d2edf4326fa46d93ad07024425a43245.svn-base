using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls
{
    public class UsersGridControl : Control
    {
        private readonly ILinqRepository<Users> userRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository;
        private Store store;
        
        public UsersGridControl(
            ILinqRepository<Users> userRepository, 
            ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository)
        {
            this.userRepository = userRepository;
            this.oivUserRepository = oivUserRepository;
        }

        public Store Store { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Panel p = new Panel
                          {
                              ID = "panelS",
                              Height = 200,
                              Layout = "fit"
                          };
            
            Store = CreateStore("dsUsers");
            page.Controls.Add(Store);

            p.Items.Add(CreateGridPanel("gpUsers", "dsUsers", page));

            return new List<Component> { p };
        }

        public GridPanel CreateGridPanel(string gridId, string storeId, ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = gridId,
                StoreID = storeId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = 600,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };

            gp.ColumnModel.AddColumn("Name", "Name", "Логин", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            gp.ColumnModel.AddColumn("FirstName", "FirstName", "Имя", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            gp.ColumnModel.AddColumn("LastName", "LastName", "Фамилия", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            gp.ColumnModel.AddColumn("Patronymic", "Patronymic", "Отчество", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }

        public Store CreateStore(string storeId)
        {
            store = new Store { ID = storeId };

            JsonReader reader = new JsonReader { IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("FirstName");
            reader.Fields.Add("LastName");
            reader.Fields.Add("Patronymic");
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter(
                "id",
                "Ext.getCmp('gpOIV')===undefined ? -1 : gpOIV.getSelectionModel().hasSelection() ? gpOIV.getSelectionModel().getSelected().id : -1",
                ParameterMode.Raw));
            store.RefreshData += StoreRefreshData;

            return store;
        }

        private void StoreRefreshData(object sender, StoreRefreshDataEventArgs e)
        {
            int id = Convert.ToInt32(e.Parameters["id"]);

            var responsOivUser = oivUserRepository.FindAll();

            store.DataSource = userRepository.FindAll()
                .Where(x => responsOivUser.Where(p => p.RefResponsOIV.ID == id).Any(r => r.RefUser == x.ID)).ToList();
            store.DataBind();
        }
    }
}
