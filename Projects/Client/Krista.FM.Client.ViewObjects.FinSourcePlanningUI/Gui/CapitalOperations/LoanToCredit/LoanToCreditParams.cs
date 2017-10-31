using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.LoanToCredit
{
    public class LoanToCreditParams
    {
        /// <summary>
        /// id выбранного выпуска ценных бумаг
        /// </summary>
        public object CapitalId
        {
            get; set;
        }

        /// <summary>
        /// Дата расчета. По умолчанию – текущая дата.  
        /// </summary>
        public DateTime CalcDate
        {
            get; set;
        }

        /// <summary>
        /// Для связки с другими таблицами.
        /// </summary>
        public string Name
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
        /// Дата начала купонного периода, предшествующая дате выкупа
        /// </summary>
        public DateTime StartCpnDate
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
        /// ID купонного периода
        /// </summary>
        public int PaimentId
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
        /// Дата погашения займа
        /// </summary>
        public DateTime DateDischarge
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
        /// Купонный доход
        /// </summary>
        public decimal Cpn
        {
            get; set;
        }

        /// <summary>
        /// НКД
        /// </summary>
        public decimal AI
        {
            get; set;
        }

        /// <summary>
        /// Непогашенная часть номинальной стоимости 1 облигации, руб.
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
        /// Цена выкупа на 1 облигацию (без НКД), руб.
        /// </summary>
        public decimal CPRub
        {
            get; set;
        }

        /// <summary>
        /// Цена выкупа на 1 облигацию (без НКД), % от номинала 
        /// </summary>
        public decimal CP
        {
            get; set;
        }

        /// <summary>
        /// Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости 1 облигации, руб. 
        /// </summary>
        public decimal DiffPCNom
        {
            get; set;
        }

        /// <summary>
        /// Привлечение кредита
        /// </summary>
        public int CreditAttraction
        {
            get; set;
        }

        /// <summary>
        /// Дата привлечения кредита
        /// </summary>
        public DateTime StartCrdtDate
        {
            get; set;
        }

        /// <summary>
        /// Дата погашения кредита
        /// </summary>
        public DateTime EndCrdtDate
        {
            get; set;
        }

        /// <summary>
        /// Рассчитываем ставку по итоговым расходам на обслуживание при выкупе 1 облигации?
        /// </summary>
        public bool IsCalcRate
        {
            get; set;
        }

        /// <summary>
        /// Ставка по кредиту, % годовых
        /// </summary>
        public decimal CrdtRate
        {
            get; set;
        }

        /// <summary>
        /// Расходы на обслуживание кредита для выкупа 1 облигации, руб.
        /// </summary>
        public decimal ServCrdt
        {
            get; set;
        }

        /// <summary>
        /// Займ 
        /// </summary>
        public decimal CostServLn
        {
            get; set;
        }

        /// <summary>
        /// Кредит
        /// </summary>
        public decimal CostServCrdt
        {
            get; set;
        }

        /// <summary>
        /// Рассчитывать количество облигаций от объема денежных средств на выкуп
        /// </summary>
        public bool IsCalcFrmRSum
        {
            get; set;
        }

        /// <summary>
        /// Количество выкупаемых облигаций (n), шт.
        /// </summary>
        public long Count
        {
            get; set;
        }

        /// <summary>
        /// Размер кредита, руб.
        /// </summary>
        public decimal CrdtSum
        {
            get; set;
        }

        /// <summary>
        /// Денежные средства на выкуп облигаций, руб.
        /// </summary>
        public decimal RepaymentSum
        {
            get; set;
        }

        /// <summary>
        /// Номинальная стоимость n облигации, руб.
        /// </summary>
        public decimal TotalNom
        {
            get; set;
        }

        /// <summary>
        /// Расходы на выплату купонного дохода, рассчитанный на n облигаций, руб. 
        /// </summary>
        public decimal TotalCpnInc
        {
            get; set;
        }

        /// <summary>
        /// Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости n облигаций, руб
        /// </summary>
        public decimal TotalDiffPCNom
        {
            get; set;
        }

        /// <summary>
        /// Расходы на обслуживание кредита для выкупа n облигаций, руб.
        /// </summary>
        public decimal TotalServCrdt
        {
            get; set;
        }

        /// <summary>
        /// Итого расходы на обслуживание при выкупе n облигаций, руб.
        /// </summary>
        public decimal TotalCostServ
        {
            get; set;
        }

        /// <summary>
        /// максимальное количество выпускаемых облигаций
        /// </summary>
        public long MaxCapitalsCount
        {
            get;
            set;
        }
    }
}
