using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital
{
    /// <summary>
    /// класс по ценным бумагам
    /// </summary>
    public class Stock
    {
        private DataRow stockRow;

        internal Stock(DataRow stockRow)
        {
            this.stockRow = stockRow;
        }

        internal int ID
        {
            get { return Convert.ToInt32(stockRow["ID"]); }
        }

        /// <summary>
        /// ссылка на валюту
        /// </summary>
        internal int RefOkv
        {
            get { return Convert.ToInt32(stockRow["RefOkv"]); }
        }

        /// <summary>
        /// дата начала размещения
        /// </summary>
        internal DateTime? StartDate
        {
            get
            {
                if (stockRow.IsNull("StartDate"))
                    //return null;
                    throw new FinSourcePlanningException(
                        "Поле 'Дата начала размещения' текущего выпуска ценных бумаг не заполнено");
                return Convert.ToDateTime(stockRow["StartDate"]);
            }
        }

        /// <summary>
        /// дата окончания размещения
        /// </summary>
        internal DateTime? EndDate
        {
            get
            {
                if (stockRow.IsNull("EndDate"))
                    return null;
                return Convert.ToDateTime(stockRow["EndDate"]);
            }
        }

        internal DateTime? DischargeDate
        {
            get
            {
                if (stockRow.IsNull("DateDischarge"))
                    return null;
                return Convert.ToDateTime(stockRow["DateDischarge"]);
            }
        }

        /// <summary>
        /// Объем выпуска
        /// </summary>
        internal decimal Sum
        {
            get
            {
                if (stockRow.IsNull("Sum"))
                    //return 0;
                    throw new FinSourcePlanningException(
                        "Поле 'Объем выпуска' текущего выпуска ценных бумаг не заполнено");
                return Convert.ToDecimal(stockRow["Sum"]);
            }
        }

        /// <summary>
        /// Объем выпуска в валюте
        /// </summary>
        internal decimal CurrencySum
        {
            get
            {
                if (stockRow.IsNull("CurrencySum"))
                    return 0;
                return Convert.ToDecimal(stockRow["CurrencySum"]);
            }
        }

        internal decimal ExchangeRate
        {
            get
            {
                if (stockRow.IsNull("ExchangeRate"))
                    return 0;
                return Convert.ToDecimal(stockRow["ExchangeRate"]);
            }
        }

        internal decimal Nominal
        {
            get
            {
                if (stockRow.IsNull("Nominal"))
                    return 0;
                return Convert.ToDecimal(stockRow["Nominal"]);
            }
        }

        internal int Quantity
        {
            get
            {
                if (stockRow.IsNull("Count"))
                    //return 0;
                    throw new FinSourcePlanningException(
                        "Поле 'Количество ценных бумаг' текущего выпуска ценных бумаг не заполнено");
                return Convert.ToInt32(stockRow["Count"]);
            }
        }
    }
}
