using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    public enum CalculatedValue
    {
        CouponR,
        YTM,
        CurrPriceRur,
        CurrPricePercent
    }

    public class FinancialCalculationParams
    {
        /// <summary>
        /// дата расчета
        /// </summary>
        public DateTime CalculationDate
        {
            get; set;
        }
        /// <summary>
        /// наименование выпуска
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// Принимаемое для расчета число дней в году
        /// </summary>
        public int Basis
        {
            get; set;
        }
        /// <summary>
        /// Номинальная стоимость, руб.
        /// </summary>
        public decimal Nominal
        {
            get; set;
        }
        /// <summary>
        /// Амортизация долга
        /// </summary>
        public bool IsDepreciation
        {
            get; set;
        }
        /// <summary>
        /// Срок обращения
        /// </summary>
        public int CurrencyBorrow
        {
            get; set;
        }
        /// <summary>
        /// Дата начала первого купонного периода
        /// </summary>
        public DateTime CouponStartDate
        {
            get; set;
        }
        /// <summary>
        /// Погашение номинальной стоимости
        /// </summary>
        public NominalCost[] PaymentsNominalCost
        {
            get; set;
        }
        /// <summary>
        /// Купоны
        /// </summary>
        public Coupon[] Coupons
        {
            get; set;
        }
        /// <summary>
        /// Ставка купонного дохода, % годовых
        /// </summary>
        public decimal CouponR
        {
            get; set;
        }
        /// <summary>
        /// Эффективная доходность к погашению, % годовых
        /// </summary>
        public decimal YTM
        {
            get; set;
        }
        /// <summary>
        /// Текущая рыночная цена (полная), руб.
        /// </summary>
        public decimal CurrentPriceRur
        {
            get; set;
        }
        /// <summary>
        /// Текущая рыночная цена (полная), %
        /// </summary>
        public decimal CurrentPricePercent
        {
            get; set;
        }
        /// <summary>
        /// Количество облигаций размещаемого займа, шт.
        /// </summary>
        public long TotalCount
        {
            get; set;
        }
        /// <summary>
        /// Объем средств от размещения займа, руб. 
        /// </summary>
        public decimal TotalSum
        {
            get; set;
        }

        public CalculatedValue CalculatedValue
        {
            get; set;
        }
        /// <summary>
        /// Погашение в конце срока
        /// </summary>
        public bool IsEndDateRepay
        {
            get; set;
        }
        /// <summary>
        /// Постоянная ставка
        /// </summary>
        public bool IsConstRate
        {
            get; set;
        }
        /// <summary>
        /// количество купонных периодов
        /// </summary>
        public int CouponsCount
        {
            get; set;
        }
    }

    public class NominalCost
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Num
        {
            get; set;
        }
        /// <summary>
        /// Количество дней
        /// </summary>
        public int DayCount
        {
            get; set;
        }
        /// <summary>
        /// Выплата номинальной стоимости
        /// </summary>
        public decimal NominalSum
        {
            get; set;
        }
    }

    public class Coupon
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Num
        {
            get; set;
        }
        /// <summary>
        /// Количество дней
        /// </summary>
        public int DayCount
        {
            get; set;
        }
        /// <summary>
        /// Ставка купонного дохода
        /// </summary>
        public decimal CouponRate
        {
            get; set;
        }
        /// <summary>
        /// Непогашенная часть номинальной стоимости 
        /// </summary>
        public decimal Nominal
        {
            get; set;
        }
    }

    public class CalculationUniqueParams
    {
        public string Name
        {
            get; set;
        }

        public DateTime CalculationDate
        {
            get; set;
        }

        public CalculationUniqueParams(string name, DateTime calcDate)
        {
            Name = name;
            CalculationDate = calcDate;
        }

        public CalculationUniqueParams(string dateName)
        {
            int dateIndex = dateName.Split('_').Length;
            string date = dateName.Split('_')[dateIndex - 1];
            Name = dateName.Replace(date, string.Empty).TrimEnd('_');
            CalculationDate = Convert.ToDateTime(date);
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name, CalculationDate.ToShortDateString());
        }
    }
}
