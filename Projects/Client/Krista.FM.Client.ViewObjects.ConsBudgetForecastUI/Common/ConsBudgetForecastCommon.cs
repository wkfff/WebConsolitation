using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common
{
    /// <summary>
    /// индекс значений строки данных
    /// </summary>
    public enum ValuesIndex
    {
        /// <summary>
        /// прогноз
        /// </summary>
        Forecast = 0,
        /// <summary>
        /// Реструктуризация
        /// </summary>
        Restructuring = 1, 
        /// <summary>
        /// Недоимка
        /// </summary>
        Arrears = 2,
        /// <summary>
        /// Доначисленные
        /// </summary>
        Priorcharge = 3,
        /// <summary>
        /// норматив по БК
        /// </summary>
        NormBK = 4,
        /// <summary>
        /// Отчисления по БК
        /// </summary>
        BK = 5,
        /// <summary>
        /// норматив по СБ
        /// </summary>
        NormRF = 6,
        /// <summary>
        /// Отчисления по СБ
        /// </summary>
        RF = 7,
        /// <summary>
        /// норматив по МР
        /// </summary>
        NormMR = 8,
        /// <summary>
        /// Отчисления по МР
        /// </summary>
        MR = 9,
        /// <summary>
        /// норматив по диф СБ
        /// </summary>
        NormDifRF = 10,
        /// <summary>
        /// Отчисления по Диф.СБ
        /// </summary>
        DifRF = 11,
        /// <summary>
        /// норматив по диф МР
        /// </summary>
        NormDifMR = 12,
        /// <summary>
        /// 
        /// </summary>
        DifMR = 13,
        /// <summary>
        /// по всем нормативам
        /// </summary>
        AllNormatives = 14,
        /// <summary>
        /// Всего отчисления
        /// </summary>
        Result = 15
    }
}
