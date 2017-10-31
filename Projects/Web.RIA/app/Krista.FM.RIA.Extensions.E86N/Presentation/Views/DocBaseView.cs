using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    ///  Базовый класс для документов
    /// </summary>
    public class DocBaseView : View
    {
        [Dependency]
        public IAuthService Auth { get; set; }

        [Dependency]
        public INewRestService NewRestService { get; set; }

        public string ViewController { get; set; }

        public string ReadOnlyDocHandler { get; set; }

        public ViewPage Page { get; set; }

        /// <summary>
        /// Идентификатор документа
        /// </summary>
        public int DocId { get; set; }

        /// <summary>
        /// Идентификатор документа
        /// </summary>
        public string DocIdStr { get; set; }

        /// <summary>
        /// Детализация документа
        /// </summary>
        public Component Detail
        {
            get { return ((BorderLayout)View.Items[0]).Center.Items[0]; }

            set { ((BorderLayout)View.Items[0]).Center.Items.Add(value); }
        }

        /// <summary>
        /// Верхний тулбар документа
        /// </summary>
        public Toolbar TopToolBar { get; set; }

        /// <summary>
        /// Вьюпорт документа
        /// </summary>
        public Viewport View { get; set; }

        /// <summary>
        /// Менеджер ресурсов этого представления
        /// </summary>
        public ResourceManager ResourceManager { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            DocId = Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]);
            DocIdStr = DocId.ToString(CultureInfo.InvariantCulture);

            Page = page;

            ResourceManager = ResourceManager.GetInstance(page);

            ResourceManager.RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            ResourceManager.RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;
            
            TopToolBar = new NewStateToolBarControl(Convert.ToInt32(DocId.ToString(CultureInfo.InvariantCulture))).BuildComponent(page);

            TopToolBar.Add(new VersioningControl(Convert.ToInt32(DocId.ToString(CultureInfo.InvariantCulture)), ViewController, ReadOnlyDocHandler).Build(page));

            TopToolBar.Add(new ToolbarSeparator());
            
            if (Auth.IsAdmin())
            {
                var export = new UpLoadFileBtnControl
                                 {
                                     Id = "btnExport",
                                     Name = "Экспорт в XML",
                                     Icon = Icon.DiskDownload,
                                     Upload = false,
                                     UploadController = "/{0}/ExportToXml".FormatWith(ViewController),
                                     Params = { { "recId", DocId.ToString(CultureInfo.InvariantCulture) } }
                                 };

                TopToolBar.Add(export.Build(Page));

                TopToolBar.Add(new ToolbarSeparator());

                TopToolBar.Add(new SetDocStateBtn(DocId).Build(Page));
            }
            
            View = new Viewport
                           {
                               Items =
                                   {
                                       new BorderLayout
                                           {
                                               North = { Items = { new ParamDocPanelControl(DocId, TopToolBar).BuildComponent(page) } },
                                           }
                                   }
                           };

            return new List<Component> { View };
        }

        public Store GetStoreByModel(ViewModelBase model, string id)
        {
            var store = StoreExtensions.StoreCreateDefault(
                string.Concat(id, "Store"),
                false,
                ViewController,
                string.Concat(id, "Read"),
                string.Concat(id, "Save"),
                string.Concat(id, "Save"),
                string.Concat(id, "Delete"));
            store.SetBaseParams("docId", DocIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocIdStr, ParameterMode.Raw);
            store.AddFieldsByClass(model);
            Page.Controls.Add(store);
            return store;
        }
    }
}
