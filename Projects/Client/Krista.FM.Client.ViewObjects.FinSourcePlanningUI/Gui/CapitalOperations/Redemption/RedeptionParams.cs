using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption
{
    public class RedeptionParams
    {
        public Int64 CapitalId
        {
            get; set;
        }

        /// <summary>
        /// Государственный регистрационный номер выпуска ценных бумаг
        /// </summary>
        public string OfficialNumber
        {
            get; set;
        }

        /// <summary>
        /// Дата расчета
        /// </summary>
        public DateTime CalcDate
        {
            get; set;
        }

        /// <summary>
        /// Комментарий к расчету
        /// </summary>
        public string Name
        {
            get; set;
        }

        public int PaimentId
        {
            get; set;
        }

        /// <summary>
        /// Дата начала купонного периода, предшествующая дате выкупа
        /// </summary>
        public DateTime StartCpnDate
        {
            get; set;
        }

        /// <summary>
        /// Номер купонного периода
        /// </summary>
        public int NumCpn
        {
            get; set;
        }

        /// <summary>
        /// Дата окончания купонного периода
        /// </summary>
        public DateTime EndCpnDate
        {
            get; set;
        }

        /// <summary>
        /// Дата выкупа
        /// </summary>
        public DateTime RedemptionDate
        {
            get; set;
        }

        /// <summary>
        /// Ставка купонного дохода, % годовых
        /// </summary>
        public decimal CouponRate
        {
            get; set;
        }

        /// <summary>
        /// Номинальная стоимость (непогашенная часть номинальной стоимости) 1 облигации, руб.
        /// </summary>
        public decimal Nom
        {
            get; set;
        }

        /// <summary>
        /// Доходность к погашению эффективная, % годовых
        /// </summary>
        public decimal YTM
        {
            get; set;
        }

        /// <summary>
        /// Цена выкупа на 1 облигацию без НКД в рублях
        /// </summary>
        public decimal CPRub
        {
            get; set;
        }

        /// <summary>
        /// Цена выкупа на 1 облигацию без НКД в процентах от номинала
        /// </summary>
        public decimal CP
        {
            get; set;
        }

        /// <summary>
        /// Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости 1 облигации, руб. ( +/- )
        /// </summary>
        public decimal DiffPCNom
        {
            get; set;
        }

        /// <summary>
        /// НКД на дату выкупа на 1 облигацию в рублях
        /// </summary>
        public decimal AI
        {
            get; set;
        }

        /// <summary>
        /// Итого расходы на обслуживание при выкупе 1 облигации в рублях
        /// </summary>
        public decimal CostServLn
        {
            get; set;
        }

        /// <summary>
        /// Запланированные расходы на выплату купона за купонный период в котором осуществляется выкуп на 1 облигацию
        /// </summary>
        public decimal Cpn
        {
            get; set;
        }

        /// <summary>
        /// Количество выкупаемых облигаций
        /// </summary>
        public long TotalCount
        {
            get; set;
        }

        /// <summary>
        /// Объем временно свободных средств, руб.
        /// </summary>
        public decimal TotalSum
        {
            get; set;
        }

        /// <summary>
        /// Номинальная стоимость (непогашенная часть номинальной стоимости) облигаций, руб.
        /// </summary>
        public decimal TotalNom
        {
            get; set;
        }

        /// <summary>
        /// Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости облигаций, руб. ( +/- )
        /// </summary>
        public decimal TotalDiffPCNom
        {
            get; set;
        }

        /// <summary>
        /// НКД на дату выкупа, руб.
        /// </summary>
        public decimal TotalAI
        {
            get; set;
        }

        /// <summary>
        /// Итого расходы на обслуживание при выкупе облигаций, руб.
        /// </summary>
        public decimal TotalCostServLn
        {
            get; set;
        }

        /// <summary>
        /// Запланированные расходы на выплату купона на выкупаемый объем за купонный период в котором осуществляется выкуп.
        /// </summary>
        public decimal TotalCpn
        {
            get; set;
        }

        /// <summary>
        /// Экономия
        /// </summary>
        public decimal Economy
        {
            get; set;
        }
        
        /// <summary>
        /// максимальное количество выпускаемых облигаций
        /// </summary>
        public long MaxCapitalsCount
        {
            get; set;
        }

        public bool IsCalcCP
        {
            get; set;
        }
    }
}
