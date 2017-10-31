using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Filters;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.ViewServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Entity
{
    public class EntityExtController : SchemeBoundController
    {
        [ViewEntityAuthorizationFilter]
        public ActionResult ShowBook(string objectKey, string sourceId, string filter)
        {
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

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
                        ViewService = new HiddenColumnsViewService(entity, filter)
                    };

            if (classifier != null)
            {
                if (classifier.Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    gridView.ParentId = "PARENTID";
                    return
                        View(
// ReSharper disable Mvc.ViewNotResolved
                            "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx",
// ReSharper restore Mvc.ViewNotResolved
                            gridView);
                }
            }

            return
                View(
// ReSharper disable Mvc.ViewNotResolved
                    "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx",
// ReSharper restore Mvc.ViewNotResolved
                    gridView);
        }

        public ActionResult DataWithCustomSearch(
                                                string objectKey,
                                                string query,
                                                IEnumerable<string> fields,
                                                int limit = 10,
                                                int start = 0)
        {
            IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

            // TODO: пока не понял как обойтись без строгой спецификации, возможно позже запилю фабрику
            if (D_OKVED_OKVED.Key.Equals(objectKey))
            {
                return DataWithCustomSearch<D_OKVED_OKVED>(query, fields, limit, start, entity);
            }

            if (D_Fin_nsiBudget.Key.Equals(objectKey))
            {
                return DataWithCustomSearch<D_Fin_nsiBudget>(query, fields, limit, start, entity);
            }

            if (D_Line_Indicators.Key.Equals(objectKey))
            {
                return DataWithCustomSearch<D_Line_Indicators>(query, fields, limit, start, entity);
            }

            // return (ActionResult)
            // typeof (EntityExtController)
            // .GetMethod("DataWithCustomSearch")
            // .MakeGenericMethod(GetDomainByKey(objectKey))
            // .Invoke(null, new object[] {query, fields, limit, start, entity});
            throw new NotImplementedException("Загляни в EntityExt\\DataWithCustomSearch!");
        }

        /*//private static Type GetDomainByKey(string objectKey)
        //{
        //    if (D_OKVED_OKVED.Key.Equals(objectKey))
        //    {
        //        return typeof(D_OKVED_OKVED);
        //    }
        //    else if (D_Fin_nsiBudget.Key.Equals(objectKey))
        //    {
        //        return typeof(D_Fin_nsiBudget);
        //    }
        //    throw new NotImplementedException();
        //}

        //[ViewEntityAuthorizationFilter]
        //public ActionResult Create(string objectKey)
        //{
        //    IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);

        //    try
        //    {
        //        if (D_Services_CPotr.Key.Equals(objectKey))
        //        {
        //            var record = JavaScriptDomainConverter<D_Services_CPotr>
        //                .DeserializeSingle(new StoreDataHandler(HttpContext.Request["data"]).JsonData);

        //            Resolver.Get<ILinqRepository<D_Services_CPotr>>().Save(record);
        //            return new AjaxStoreResult(StoreResponseFormat.Save)
        //                .Do(storeResult => storeResult.SaveResponse.Message = string.Format("{{newId:{0}}}", record.ID));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return new AjaxStoreResult(StoreResponseFormat.Save)
        //            .Do(
        //                result => result.SaveResponse
        //                              .Do(response => response.Message = e.ExpandException())
        //                              .Do(response => response.Success = false));
        //    }

        //    throw new NotImplementedException();
        //}*/

        private static ActionResult DataWithCustomSearch<T>(
                                                            string query,
                                                            IEnumerable<string> fields,
                                                            int limit,
                                                            int start,
                                                            IEntity entity) where T : DomainObject
        {
            IQueryable<T> data = Resolver.Get<ILinqRepository<T>>().FindAll().ApplyQuery(query, fields, entity);
            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }
    }
}
