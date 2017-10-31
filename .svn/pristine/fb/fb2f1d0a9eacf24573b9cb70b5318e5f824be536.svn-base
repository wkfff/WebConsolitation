using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    ///   Субъекты консолидации
    /// </summary>
    public class SubjectsConsolidationView : View
    {
        private const string ObjectKey = "eda1248c-475d-487a-95e0-1a363b068408";
        
        private readonly IScheme scheme;

        public SubjectsConsolidationView(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public override List<Component> Build(ViewPage page)
        {
            var entity = scheme.RootPackage.FindEntityByName(ObjectKey);

            var gridView = new GridModelControl
            {
                Entity = entity,
                Readonly = true,
                Id = "{0}_Grid".FormatWith(entity.FullDBName),
                Title = entity.FullCaption
            }.Build(page)[0] as GridPanel;
            
            var importBtn = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Закачка субъектов с консолидации",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = UiBuilders.GetUrl<ImportsController>("ImportsConsolidationSubjects")
                }.Build(page)[0] as FormPanel;

            if (importBtn != null)
            {
                var button = importBtn.Items[0] as Button;
                if (button != null)
                {
                    button.DirectEvents.Click.Before = "Ext.Msg.wait('Выполняется процесс...', 'Подождите');";
                }
            }

            gridView.Toolbar().Add(importBtn);
            
            return new List<Component> { new Viewport { Items = { new FitLayout { Items = { gridView } } } } };
        }
    }
}
