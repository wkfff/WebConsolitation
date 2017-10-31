using Krista.FM.Client.Reports.Database.FactTables.CapitalOperations;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Capital
{
    class ParamCapitalBondBuyback : ParamInfo
    {
        public ParamCapitalBondBuyback()
        {
            Description = "Расчет";
            BookInfo = new ParamBookInfo
                           {
                               EntityKey = f_S_BondBuyback.internalKey, 
                               MultiSelect = true
                           };
        }
    }
}
