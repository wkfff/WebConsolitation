using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.CapitalOperations;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Capital
{
    class ParamCapitalBondBuybackDetail : ParamCapitalBondBuyback
    {
        public ParamCapitalBondBuybackDetail()
        {
            MultiSelect = true;
            BookInfo.DefaultSort = f_S_BondBuyback.OfficialNumber;
            Columns = new List<string>
                          {
                              f_S_BondBuyback.RedemptionDate.ToUpper(), 
                              f_S_BondBuyback.OfficialNumber.ToUpper()
                          };
        }
    }
}
