using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.CapitalOperations;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Capital
{
    class ParamCapitalReplaceIssFoLnDetail : ParamCapitalReplaceIssFoLn
    {
        public ParamCapitalReplaceIssFoLnDetail()
        {
            MultiSelect = true;
            BookInfo.DefaultSort = f_S_ReplaceIssForLn.OfficialNumber;
            Columns = new List<string>
                          {
                              f_S_ReplaceIssForLn.RedemptionDate.ToUpper(), 
                              f_S_ReplaceIssForLn.OfficialNumber.ToUpper()
                          };
        }
    }
}
