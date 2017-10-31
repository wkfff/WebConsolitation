using System;
using System.Data;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.RIA.Extensions.Entity
{
    [Authorize]
    public class EntityNavController : SchemeBoundController
    {
        private const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Extensions.Entity.dll/Krista.FM.RIA.Extensions.Entity/Presentation/Views/EntityNav/";

        //
        // GET: /EntityNav/
        public ActionResult Index(int classType)
        {
            ViewData.Model = classType;
            return View(ViewRoot + "Index.aspx");
        }

        public AjaxStoreResult Data(int classType, int limit, int start, string dir, string sort, [FiltersBinder]FilterConditions filters)
        {
            List<string> selectFilter = new List<string>();
            int i = 0;

                // Ќакладываем фильтры грида
                foreach (FilterCondition filter in filters.Conditions)
                {
                    string filterComparison = filter.Comparison == Comparison.Gt ? ">" : filter.Comparison == Comparison.Lt ? "<" : "=";
                    if (filter.FilterType == FilterType.String)
                    {
                        filterComparison = "like";
                    }
                    string filterStr = "({0} {1} '%{2}%')".FormatWith(filter.Name, filterComparison, filter.Value);
                    selectFilter.Add(filterStr);
                    i++;
                }
                string queryFilter = String.Join(" and ", selectFilter.ToArray());
                if (i > 0)
                    queryFilter = " and " + queryFilter;
                return GetDataInternal(classType, start, limit, dir, sort, queryFilter);
        }

        private AjaxStoreResult GetDataInternal(int classType , int start, int limit, string dir, string sort, string filter)
        {

            // формируем таблицу: если classtype = 3, загружаем факты, иначе - классификаторы.
            DataTable table = classType == 3 ? Scheme.FactTables.GetDataTable() : Scheme.Classifiers.GetDataTable();
            // делаем копию таблицы
            DataTable dest = table.Clone();
            int indx = 1;
            int count = 0;
            // дл€ каждой записи устанавливаем значение classtype, фильтры
            DataRow[] rows = table.Select(String.Format("ClassType = {0}", classType) + filter, sort + " " + dir);
            foreach (var dataRow in rows)
            {
                if (indx > start && count < limit)
                {
                    dest.Rows.Add(dataRow.ItemArray);
                    count++;
                }
                if (count == limit)
                    break;
                indx++;
            }
            // возвращаем набор данных фактов или классификаторов
            return new AjaxStoreResult(dest, rows.GetLength(0));
        }
                
    }
}
