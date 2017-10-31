using System;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL
{
    class ParamOrgFoundation : ParamInfo
    {
        public ParamOrgFoundation()
        {
            Description = "Учредители";
            
            BookInfo = new ParamBookInfo
                           {
                               InfoField = t_Org_FounderOrg.NameP,
                               EntityKey = t_Org_FounderOrg.InternalKey,
                               FullScreen = true,
                               MultiSelect = true,
                               DefaultSort = t_Org_FounderOrg.INN,
                               ItemTemplate = String.Format("{0} {1}", t_Org_FounderOrg.INN, t_Org_FounderOrg.NameP),
                               ItemFilter = String.Format("{0} = 1", t_Org_FounderOrg.Last)
                           };
        }
    }
}
