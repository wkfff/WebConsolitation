using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamExistCapital : ParamExistRegNum
    {
        public ParamExistCapital()
        {
            Description = "Областные займы";
        }

        protected override List<ParamBaseTable> GetTableList()
        {
            var filter = String.Format("{0} in ({1})", f_S_Capital.RefVariant, ReportConsts.FixedVariantsID);
            var tableList = new List<ParamBaseTable>
                                {
                                    new ParamBaseTable
                                        {
                                            TableKey = f_S_Capital.InternalKey,
                                            FieldList = new List<string>
                                                            {
                                                                f_S_Capital.OfficialNumber,
                                                                f_S_Capital.RegEmissionDate,
                                                                f_S_Capital.RegNumber
                                                            },
                                            Filter = filter,
                                        }
                                };
            return tableList;
        }
    }
}
