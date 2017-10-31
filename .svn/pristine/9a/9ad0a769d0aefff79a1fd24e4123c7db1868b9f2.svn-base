using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Helpers;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    /// <summary>
    /// Контроллер для работы с периодами
    /// </summary>
    public class FO41HMAOPeriodsController : SchemeBoundController
    {
        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по месяцам) в иерархическом виде</returns>
        public ActionResult LookupPeriods(int taxTypeId)
        {
            try
            {
                var data = new List<Period>();
                var curYear = DateTime.Today.Year;
                var curMonth = DateTime.Today.Month;
                var curQuarter = ((curMonth - 1) / 3)  + 1;
                for (var quarter = curQuarter - 1; quarter >= 1; quarter--)
                {
                    data.Add(new Period
                    {
                        ID = (curYear * 10000) + (9990 + quarter),
                        Name = "{0} квартал {1} года".FormatWith(quarter, curYear),
                        Year = curYear,
                        Month = quarter * 3,
                        Parent = (curYear * 10000) + 1
                    });
                }

                for (var year = DateTime.Today.Year - 1; year > 2009; year--)
                {
                    for (int quarter = 4; quarter >= 1; quarter--)
                    {
                        data.Add(new Period
                                     {
                                         ID = (year * 10000) + (9990 + quarter),
                                         Name = "{0} квартал {1} года".FormatWith(quarter, year),
                                         Year = year,
                                         Month = quarter * 3,
                                         Parent = (year * 10000) + 1
                                     });
                    }
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
