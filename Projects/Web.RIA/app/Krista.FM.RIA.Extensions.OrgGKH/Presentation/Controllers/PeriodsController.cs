using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с периодами
    /// </summary>
    public class PeriodsController : SchemeBoundController
    {
        /// <summary>
        /// Список месяцев
        /// </summary>
        public static readonly List<string> Months = new List<string>(12)
                                                   {
                                                       "январь", 
                                                       "февраль", 
                                                       "март", 
                                                       "апрель", 
                                                       "май", 
                                                       "июнь", 
                                                       "июль", 
                                                       "август", 
                                                       "сентябрь", 
                                                       "октябрь", 
                                                       "ноябрь", 
                                                       "декабрь"
                                                   };

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по месяцам) в иерархическом виде</returns>
        public ActionResult LookupMonthPeriod()
        {
            try
            {
                var data = new List<Period>();
                for (var i = DateTime.Today.Year + 1; i > 2009; i--)
                {
                    for (var j = 12; j > 0; j--)
                    {
                        data.Add(new Period
                        {
                            ID = (i * 10000) + (j * 100),
                            Name = " {0} {1}".FormatWith(Months[j - 1], i),
                            Year = i,
                            Month = j,
                            Parent = (i * 10000) + 1
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

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список периодов (по неделям) в иерархическом виде</returns>
        public ActionResult LookupWeekPeriod()
        {
            try
            {
                var data = new List<Period>();

                var date = DateTime.Today;

                date = date.AddDays(03 - date.Day);
                date = date.AddMonths(01 - date.Month);
                date = date.AddYears(2011 - date.Year);
                
                var today = DateTime.Today;

                while (date <= today)
                {
                    var period = new Period
                                     {
                                         Year = date.Year,
                                         Month = date.Month,
                                         Parent = -1
                                     };

                    var sdate = date.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
                    date = date.AddDays(4);
                    period.ID = (date.Year * 10000) + (date.Month * 100) + date.Day;
                    var fdate = date.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
                    date = date.AddDays(3);

                    period.Name = "{0}-{1}".FormatWith(sdate, fdate);

                    data.Add(period);
                }

                data.Reverse();

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
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
