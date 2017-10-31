using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Controllers
{
    /// <summary>
    /// Контролле для работы с периодами
    /// </summary>
    public class FO51PeriodsController : SchemeBoundController
    {
        /// <summary>
        /// Список месяцев
        /// </summary>
        private readonly List<string> months = new List<string>(12) { " январь", " февраль", " март", " апрель", " май", " июнь", " июль", " август", " сентябрь", " октябрь", " ноябрь", " декабрь" };

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по месяцам) в иерархическом виде</returns>
        public ActionResult LookupMonthPeriod(int year)
        {
            try
            {
                var data = new List<Period>();
                var curYear = DateTime.Today.Year;
                var month = year == curYear ? DateTime.Today.Month - 1 : 12;
                if (month == 0)
                {
                    month = 1;
                }

                while (month != 0)
                {
                        data.Add(new Period
                        {
                            ID = (year * 10000) + (month * 100),
                            Name = "{0} {1}".FormatWith(months[month - 1], year),
                            Year = year,
                            Month = month,
                            Parent = (year * 10000) + 1
                        });

                        month--;
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по годам)</returns>
        public ActionResult LookupYearPeriod()
        {
            try
            {
                var year = DateTime.Today.Year;
                if (DateTime.Today.Month == 1)
                {
                    year--;
                }

                var data = new List<object>();
                while (year != 2010)
                {
                    data.Add(new
                                 {
                                     ID = year,
                                     Name = "{0} год".FormatWith(year)
                                 });
                    year--;
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult ShowPeriodView()
        {
            var periodView = new ChoosePeriodView();

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", periodView);
        }

        /// <summary>
        /// Класс - период
        /// </summary>
        private class Period
        {
            /// <summary>
            /// Идентификатор периода
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Наименование периода
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// Год периода
            /// </summary>
            public int Year { get; set; }
            
            /// <summary>
            /// Месяц периода или 0
            /// </summary>
            public int Month { get; set; }
            
            /// <summary>
            /// Родительский период (для периодов по месяцам) или 0 (по годам)
            /// </summary>
            public int Parent { get; set; }
        }
    }
}
