using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamExistCredit : ParamExistRegNum
    {
        public ParamExistCredit()
        {
            Description = "Кредит";
        }

        protected override List<ParamBaseTable> GetTableList()
        {
            var filter = String.Format("{0} in ({1})", f_S_Creditincome.RefVariant, ReportConsts.FixedVariantsID);
            var tableList = new List<ParamBaseTable>
                                {
                                    new ParamBaseTable
                                        {
                                            FieldList = new List<string>
                                                            {
                                                                f_S_Creditincome.Num,
                                                                f_S_Creditincome.ContractDate,
                                                                f_S_Creditincome.RegNum
                                                            },
                                            Filter = filter,
                                            TableKey = f_S_Creditincome.internalKey
                                        }
                                };
            return tableList;
        }
    }
}
