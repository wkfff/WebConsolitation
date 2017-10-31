using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamExistRegNum : ParamInfo
    {
        private const string DisplayColumn = "CALC_NAME";

        protected class ParamBaseTable
        {
            public string TableKey { get; set; }
            public List<string> FieldList { get; set; }
            public string Filter { get; set; }
        }

        public ParamExistRegNum()
        {
            Description = "Номер договора";
            MultiSelect = false;
            Table = CreateTable();
            Columns = new List<string>();

            for (var i = 2; i < Table.Columns.Count - 1; i++)
            {
                Columns.Add(Table.Columns[i].ColumnName);
            }

            DisplayFieldName = DisplayColumn;
        }

        private DataTable CreateTable()
        {
            var lstInfo = GetTableList();
            var columnCount = lstInfo.Max(f => f.FieldList.Count) + 1;
            var tblResult = new DataTable();

            for (var i = 0; i < columnCount + 1; i++)
            {
                tblResult.Columns.Add(Convert.ToString(i), typeof(string));
            }

            tblResult.Columns[0].ColumnName = ID;
            tblResult.Columns[1].ColumnName = NAME;
            tblResult.Columns[tblResult.Columns.Count - 1].ColumnName = DisplayColumn;

            foreach (var info in lstInfo)
            {
                var tblEntity = dbHelper.GetEntityData(info.TableKey, info.Filter);
                
                foreach (DataRow rowData in tblEntity.Rows)
                {
                    var cntColumn = 0;
                    var rowInsert = tblResult.Rows.Add();
                    rowInsert[cntColumn++] = rowData[info.FieldList[0]];
 
                    foreach(var fName in info.FieldList)
                    {
                        var value = rowData[fName];

                        if (tblEntity.Columns[fName].DataType == typeof(DateTime))
                        {
                            value = Convert.ToDateTime(value).ToShortDateString();
                        }

                        rowInsert[cntColumn++] = value;
                    }

                    rowInsert[DisplayColumn] = String.Format("{0} {1}", rowInsert[1], rowInsert[2]);
                }
            }

            return tblResult;
        }

        protected virtual List<ParamBaseTable> GetTableList()
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
                                        },
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
                                        },
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
