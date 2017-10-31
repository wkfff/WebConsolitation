using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamExistContractDate : ParamExistRegNum
    {
        public ParamExistContractDate()
        {
            Description = "Договор";
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
                                                                f_S_Creditincome.ContractDate
                                                            },
                                            Filter = filter,
                                            TableKey = f_S_Creditincome.internalKey
                                        },
                                    new ParamBaseTable
                                        {
                                            TableKey = f_S_Guarantissued.InternalKey,
                                            FieldList = new List<string>
                                                            {
                                                                f_S_Guarantissued.Num,
                                                                f_S_Guarantissued.StartDate
                                                            },
                                            Filter = filter
                                        },
                                    new ParamBaseTable
                                        {
                                            TableKey = f_S_Capital.InternalKey,
                                            FieldList = new List<string>
                                                            {
                                                                f_S_Capital.OfficialNumber,
                                                                f_S_Capital.StartDate
                                                            },
                                            Filter = filter,
                                        }
                                };
            return tableList;
        }
    }
}
