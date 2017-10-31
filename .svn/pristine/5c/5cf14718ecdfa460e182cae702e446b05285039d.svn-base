using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Helpers;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с периодами
    /// </summary>
    public class FO41PeriodsController : SchemeBoundController
    {
        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по месяцам) в иерархическом виде</returns>
        public ActionResult LookupPeriods()
        {
            try
            {
                var data = new List<Period>();
                for (var year = DateTime.Today.Year - 1; year > 2009; year--)
                {
                    data.Add(new Period
                    {
                        ID = (year * 10000) + 1,
                        Name = "{0} год".FormatWith(year),
                        Year = year,
                        Month = 12,
                        Parent = (year * 10000) + 1
                    });
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
