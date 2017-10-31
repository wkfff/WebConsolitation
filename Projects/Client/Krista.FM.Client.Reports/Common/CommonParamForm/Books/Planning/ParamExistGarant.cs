using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamExistGarant : ParamExistRegNum
    {
        public ParamExistGarant()
        {
            Description = "Гарантия";
        }

        protected override List<ParamBaseTable> GetTableList()
        {
            var filter = String.Format("{0} in ({1})", f_S_Guarantissued.RefVariant, ReportConsts.FixedVariantsID);
            var tableList = new List<ParamBaseTable>
                                {
                                    new ParamBaseTable
                                        {
                                            TableKey = f_S_Guarantissued.InternalKey,
                                            FieldList = new List<string>
                                                            {
                                                                f_S_Guarantissued.Num,
                                                                f_S_Guarantissued.StartDate,
                                                                f_S_Guarantissued.RegNum
                                                            },
                                            Filter = filter
                                        }
                                };
            return tableList;
        }
    }
}
