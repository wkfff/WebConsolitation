using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsData.EGRUL;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL
{
    class ParamKLADR : ParamInfo
    {
        public ParamKLADR()
        {
            Description = "Код субъекта по КЛАДР";
            Table = GetValuesList();
            Columns = new List<string> { ID };
        }

        public DataTable GetValuesList()
        {
            var orgAddressEntity = ConvertorSchemeLink.GetEntity(d_Org_Adress.InternalKey);
            var selectStr = String.Format("select distinct {0} as id, {1} as name from {2} order by {0}, {1}",
                d_Org_Adress.KodKLRegion,
                d_Org_Adress.NameRegion,
                orgAddressEntity.FullDBName);
            return dbHelper.GetTableData(selectStr);
        }
    }
}
