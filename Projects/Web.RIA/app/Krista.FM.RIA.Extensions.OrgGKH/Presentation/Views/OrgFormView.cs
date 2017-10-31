using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.OrgGKH.Params;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    public class OrgFormView : View
    {
        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;
        private readonly IOrgGkhExtension extension;
        private readonly string comboRegionsId = "Region";
        private readonly int parentRegion;

        public OrgFormView(
            IOrgGkhExtension extension, 
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            int parentRegion)
        {
            this.extension = extension;
            this.orgRepository = orgRepository;
            this.parentRegion = parentRegion;
        }

        public int? OrgID { get; set; }

        public List<KeyValuePair<int, string>> Regions { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportOrgForm",
                        Items = { new BorderLayout { Center = { Items = { CreateOrgGrid(page) } } } }
                    }
                };
        }

        private IEnumerable<Component> CreateOrgGrid(ViewPage page)
        {
            var formPanel = new FormPanel
                                {
                                    ID = "OrgForm",
                                    Border = false,
                                    CssClass = "x-window-mc",
                                    BodyCssClass = "x-window-mc",
                                    StyleSpec = "margin: 5px 5px 5px 5px;",
                                    Layout = "RowLayout",
                                    AutoHeight = true,
                                    Url = "/Organization/Save?orgId={0}".FormatWith(OrgID)
                                };

            var fields = new FieldSet
            {
                ID = "orgFields",
                Collapsible = false,
                Collapsed = false,
                Layout = "form",
                Border = false,
                LabelSeparator = String.Empty,
                LabelAlign = LabelAlign.Left,
                LabelWidth = 200,
                MaxWidth = 750,
                Padding = 2,
                StyleSpec = "margin-top: 2px; margin-bottom: 2px;",
                DefaultAnchor = "0",
                Height = 300
            };

            var orgParams = (OrgID != null) ? orgRepository.FindOne((int)OrgID) : new D_Org_RegistrOrg();

            var storeRegions = CreateStoreRegions();
            page.Controls.Add(storeRegions);

            fields.Add(new TextField
                           {
                               ID = "NameOrg",
                               AllowBlank = false,
                               FieldLabel = @"Наименование организации*",
                               Value = orgParams.NameOrg
                           });
            fields.Add(new TextField
                           {
                               ID = "ShName",
                               FieldLabel = @"Краткое наименование",
                               Value = orgParams.ShName
                           });
            fields.Add(new TextField
                           {
                               ID = "LegalAddress",
                               FieldLabel = @"Юридический адрес",
                               Value = orgParams.LegalAddress
                           });
            fields.Add(new TextField
                           {
                               ID = "FactAddress",
                               FieldLabel = @"Фактический адрес",
                               Value = orgParams.FactAddress                              
                           });
            fields.Add(new TextField
                           {
                               ID = "Phone",
                               FieldLabel = @"Телефон",
                               Value = orgParams.Phone
                           });

            fields.Add(new TextField
                           {
                               ID = "INN",
                               AllowBlank = false,
                               FieldLabel = @"ИНН*",
                               Value = orgParams.INN
                           });

            fields.Add(new TextField
                           {
                               ID = "KPP",
                               AllowBlank = false,
                               FieldLabel = @"КПП*",
                               Value = orgParams.KPP
                           });

            fields.Add(new ComboBox
                           {
                               TriggerAction = TriggerAction.All,
                               ValueField = "ID",
                               Width = 300,
                               Disabled = false,
                               DisplayField = "Name",
                               ID = comboRegionsId,
                               AllowBlank = false,
                               Editable = false,
                               FieldLabel = @"МО*",
                               StoreID = storeRegions.ID,
                               Value = orgParams.RefRegionAn == null ? Regions[0].Key : orgParams.RefRegionAn.ID
                           });

            fields.Add(new TextField
            {
                ID = "Login",
                FieldLabel = @"Логин",
                Disabled = !User.IsInRole(OrgGKHConsts.GroupAuditName),
                Value = orgParams.Login
            });

            fields.Add(new DisplayField { Text = @"* - обязательное поле для заполнения" });

            formPanel.Add(fields);

            return new List<Component> { formPanel };
        }

        private Store CreateStoreRegions()
        {
            var ds = new Store
                         {
                             ID = "RegionsStore",
                             AutoLoad = true
                         };

            if (User.IsInRole(OrgGKHConsts.GroupAuditName))
            {
                ds.Listeners.BeforeLoad.AddAfter(@"
                Ext.apply({0}.baseParams, {{ regionId: {1} }}); ".FormatWith(ds.ID, parentRegion));
            }

            ds.SetHttpProxy("/Organization/LookupRegions");

            ds.SetJsonReader()
            .AddField("ID")
            .AddField("Name");

            return ds;
        }
    }
}
