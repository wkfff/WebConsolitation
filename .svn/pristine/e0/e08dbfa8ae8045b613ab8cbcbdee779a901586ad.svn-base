using System;
using System.Web.Mvc;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Filters;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Entity
{
    public class NewEntityController : SchemeBoundController
    {
        [ViewEntityAuthorizationFilter]
        public ActionResult ShowBook(string objectKey, string sourceId, string filter)
        {
            var entity = Scheme.RootPackage.FindEntityByName(objectKey);

            if (entity == null)
            {
                throw new Exception("Объект не найден.");
            }

            var classifier = entity as IClassifier;

            var gridView = new EntityGridView
            {
                Entity = entity,
                Id = "gridView{0}".FormatWith(entity.FullDBName),
                Title = entity.FullCaption,
                SourceId = -1,
                Readonly = true,
                IsBook = true,
                ShowMode = EntityBookViewModel.ShowModeType.Normal,
                ViewService = new DefaultViewService(filter)
            };

            if (classifier != null)
            {
                if (classifier.Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    gridView.ParentId = "PARENTID";
                    return View(
                        // ReSharper disable Mvc.ViewNotResolved
                        "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx",
                        // ReSharper restore Mvc.ViewNotResolved
                        gridView);
                }
            }

            return View(
                // ReSharper disable Mvc.ViewNotResolved
                "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx",
                // ReSharper restore Mvc.ViewNotResolved
                gridView);
        }
    }
}
