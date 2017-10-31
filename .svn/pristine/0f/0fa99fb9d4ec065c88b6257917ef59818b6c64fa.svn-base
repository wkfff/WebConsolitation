using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.FNS;

namespace Krista.FM.Client.Reports
{
    // базовый класс подтяжки новых значение по зависимым параметрам
    public class UndercutParamBase
    {
        protected object oldValue = String.Empty;
        protected object parentValue = String.Empty;

        // старое значение свойства
        public object OldValue
        {
            get { return oldValue; }
            set { oldValue = value; }
        }

        // значение свойства от которого зависим
        public object ParentValue
        {
            get { return parentValue; }
            set { parentValue = value; }
        }

        // новое вычисленное значение
        public object NewValue
        {
            get { return CalcNewValue(); }
        }

        protected virtual object CalcNewValue()
        {
            return oldValue;
        }

        public ParamInfo settings { get; set; }
    }

    public class UndercutExchangeRate : UndercutParamBase
    {
        private object actualDate;

        // фактическая дата курса
        public object ActualDate
        {
            get { return actualDate; }
            set { actualDate = value; }
        }

        protected virtual object GetDateStr()
        {
            return ParentValue;
        }

        protected virtual int OkvCode()
        {
            return -1;
        }

        protected virtual int GetDateOffset()
        {
            return 0;
        }

        protected override object CalcNewValue()
        {
            var filter = String.Format("{0} = {2} and {1} = 0",
                    d_S_ExchangeRate.RefOkv,
                    d_S_ExchangeRate.IsPrognozExchRate,
                    OkvCode());

            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var dtItems = dbHelper.GetEntityData(d_S_ExchangeRate.internalKey, filter);
            var strDate = GetDateStr();
            var daysOffset = GetDateOffset();

            if (daysOffset != 0)
            {
                strDate = Convert.ToDateTime(strDate).AddDays(daysOffset).ToShortDateString();
            }

            dtItems = DataTableUtils.FilterDataSet(dtItems,
                String.Format("{0} <= '{1}'", d_S_ExchangeRate.DateFixing, strDate));

            dtItems = DataTableUtils.SortDataSet(dtItems, d_S_ExchangeRate.DateFixing);

            if (dtItems.Rows.Count > 0)
            {
                var rowExchange = dtItems.Rows[dtItems.Rows.Count - 1];
                actualDate = rowExchange[d_S_ExchangeRate.DateFixing];
                return rowExchange[d_S_ExchangeRate.ExchangeRate].ToString();
            }

            return oldValue;
        }
    }

    public class UndercutExchangeUniEuroRate : UndercutExchangeRate
    {
        protected virtual string GetOkvName()
        {
            return "EUR";
        }

        protected override int OkvCode()
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var tblOkvBook = dbHelper.GetEntityData(d_OKV_Currency.internalKey);
            var rowsEuro = tblOkvBook.Select(String.Format("{0} = '{1}'", d_OKV_Currency.CodeLetter, GetOkvName()));
            return rowsEuro.Length > 0 ? Convert.ToInt32(rowsEuro[0][d_OKV_Currency.ID]) : ReportConsts.codeEUR;
        }
    }

    public class UndercutExchangeUniUsdRate : UndercutExchangeUniEuroRate
    {
        protected override string GetOkvName()
        {
            return "USD";
        }
    }

    public class UndercutExchangeUniUsdPrevRate : UndercutExchangeUniUsdRate
    {
        protected override int GetDateOffset()
        {
            return -1;
        }
    }

    public class UndercutPlanServiceStart : UndercutParamBase
    {
        protected virtual int GetSelectedIndex()
        {
            return 0;
        }

        protected DataTable GetTable()
        {
            var planObj = new ParamPlanService { num = ParentValue };
            return planObj.CreateTable();  
        }

        protected override object CalcNewValue()
        {
            var tbl = GetTable();
            settings.Table = tbl;
            return tbl.Rows.Count > 0 ? tbl.Rows[0][3] : null;
        }
    }

    public class UndercutPlanServiceEnd : UndercutPlanServiceStart
    {
        protected override object CalcNewValue()
        {
            var tbl = GetTable();
            settings.Table = tbl;
            return tbl.Rows.Count > 0 ? tbl.Rows[tbl.Rows.Count - 1][3] : null;
        }
    }

    public class UndercutMarkYear : UndercutParamBase
    {
        private readonly ParamBookInfo _markBridgeBookInfo;
        private readonly ParamBookInfo _markBookInfo;
        private bool isFirstRun = true;

        public UndercutMarkYear(ParamBookInfo markBridgeBookInfo, ParamBookInfo markBookInfo)
        {
            _markBridgeBookInfo = markBridgeBookInfo;
            _markBookInfo = markBookInfo;
        }

        protected override object CalcNewValue()
        {
            var container = ParamStore.container;
            var paramYear = Convert.ToString(parentValue);
            var paramContainer = container[ReportConsts.ParamMark];
            paramContainer.BookInfo = _markBookInfo;
            paramContainer.BookInfo.ClearItemsList();
            paramContainer.BookInfo.SourceYear = paramYear;
            if (paramContainer.BookInfo.CreateSourceList().Rows.Count > 1 && _markBridgeBookInfo != null)
            {
                paramContainer.BookInfo = _markBridgeBookInfo;
                paramContainer.BookInfo.ClearItemsList();
            }

            object value = container.GetParams()[ReportConsts.ParamMark];
            if (isFirstRun)
            {
                value = paramContainer.DefaultValue;
                isFirstRun = false;
            }
            
            return paramContainer.BookInfo.CheckValue(value);
        }
    }
}
