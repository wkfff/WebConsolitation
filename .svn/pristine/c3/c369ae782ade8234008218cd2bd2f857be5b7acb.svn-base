using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseDataController : SchemeBoundController
    {
        private readonly IForecastExtension extension;

        private readonly IForecastReverseValuesRepository valuesRepository;
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastReverseRepository variantsRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;
        
        public ReverseDataController(
            IForecastExtension extension,
            IForecastReverseValuesRepository valuesRepository,
            IForecastParamsRepository paramsRepository,
            IForecastReverseRepository variantsRepository,
            IRepository<FX_Date_YearDayUNV> yearRepository)
        {
            this.extension = extension;
            this.valuesRepository = valuesRepository;
            this.paramsRepository = paramsRepository;
            this.variantsRepository = variantsRepository;
            this.yearRepository = yearRepository;
        }

        public ActionResult LoadY(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datY = ufc.DataService.GetProgData();
            DataTable datFactors = ufc.DataService.GetStaticData();

            DataTable tempTable = datY.Clone();

            DataRow rowFactors = (from row in datFactors.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();
            DataRow rowY = (from row in datY.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == ufc.ParamId
                               select row).First();

            DataRow newRow = tempTable.NewRow();
            foreach (DataColumn col in datY.Columns)
            {
                ////if (col.ColumnName != "id")
                {
                    newRow[col.ColumnName] = rowY[col.ColumnName];
                }
            }

            tempTable.Rows.Add(newRow);

            newRow = tempTable.Rows[0]; ////tempTable.NewRow();
            foreach (DataColumn col in datFactors.Columns)
            {
                if (col.ColumnName != "id")
                {
                    newRow[col.ColumnName] = rowFactors[col.ColumnName];
                }
            }
            
            ////tempTable.Rows.Add(newRow);

            return new AjaxStoreResult(tempTable, tempTable.Rows.Count);
        }

        public ActionResult LoadFactors(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datFactors = ufc.DataService.GetStaticData();

            DataTable tempTable = datFactors.Copy(); ////.Clone();
            tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["id"] };
            
            if (ufc.Contains("ResultTable"))
            {
                DataTable resultTable = ufc.GetObject("ResultTable") as DataTable;
                
                foreach (DataColumn column in resultTable.Columns)
                {
                    if (!tempTable.Columns.Contains(column.ColumnName))
                    {
                        tempTable.Columns.Add(column.ColumnName, column.DataType);
                    }
                }

                foreach (DataRow row in resultTable.Rows)
                {
                    var tempRow = tempTable.Rows.Find(row["id"]);

                    foreach (DataColumn column in resultTable.Columns)
                    {
                        tempRow[column.ColumnName] = row[column.ColumnName];
                    }
                }
            }

            return new AjaxStoreResult(tempTable, tempTable.Rows.Count);
        }

        public ActionResult LoadRegs(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datRegs = ufc.DataService.GetRegulatorData();

            return new AjaxStoreResult(datRegs, datRegs.Rows.Count);
        }

        public ActionResult LoadChart(string key, int paramId, int expYear)
        {
            List<ChartData> lst = new List<ChartData>();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datY = ufc.DataService.GetProgData();
            DataTable datFactors = ufc.DataService.GetStaticData();

            var listY = (from row in datY.AsEnumerable()
                                   where Convert.ToInt32(row["id"]) == paramId
                                   select row).ToList();
            var listFactors = (from row in datFactors.AsEnumerable()
                                     where Convert.ToInt32(row["id"]) == paramId
                                     select row).ToList(); 

            DataRow dataRowY = null;
            DataRow dataRowFactors = null;
            DataRow dataRowResult = null;

            if (listY.Count > 0)
            {
                dataRowY = listY.First();
            }

            if (listFactors.Count > 0)
            {
                dataRowFactors = listFactors.First();
            }

            if (ufc.Contains("ResultTable"))
            {
                DataTable resultTable = ufc.GetObject("ResultTable") as DataTable;
                var listResult = (from row in resultTable.AsEnumerable()
                                   where Convert.ToInt32(row["id"]) == paramId
                                   select row).ToList();

                if (listResult.Count > 0)
                {
                    dataRowResult = listResult.First();
                }

                if (dataRowY != null)
                {
                    for (int i = 0; i < datY.Columns.Count; i++)
                    {
                        DataColumn col = datY.Columns[i];
                        string colName = col.ColumnName;

                        double? xp = null; 
                        double? xs = null;

                        if (colName.Contains("year_"))
                        {
                            int year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                            if (year >= expYear)
                            {
                                object op = dataRowY[colName];
                                if (op != DBNull.Value)
                                {
                                    xp = Convert.ToDouble(op);
                                }
                            }

                            if (year <= expYear)
                            {
                                if (datY.Columns.Contains(colName))
                                {
                                    object os = dataRowY[colName];
                                    if (os != DBNull.Value)
                                    {
                                        xs = Convert.ToDouble(os);
                                    }
                                }    
                            }
                            
                            lst.Add(new ChartData { Year = year, Xp = xp, Xs = xs });
                        }
                    }
                }
                else
                {
                    if (dataRowFactors != null)
                    {
                        for (int i = 0; i < datY.Columns.Count; i++)
                        {
                            DataColumn col = datY.Columns[i];
                            string colName = col.ColumnName;

                            double? xp = null;
                            double? xs = null;
                            
                            int? year = null;

                            if (colName.Contains("year_"))
                            {
                                if (datFactors.Columns.Contains(colName))
                                {
                                    year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                                    object op = dataRowFactors[colName];
                                    if (op != DBNull.Value)
                                    {
                                        xp = Convert.ToDouble(op);
                                    }

                                    if (year == expYear)
                                    {
                                        object os = dataRowFactors[colName];

                                        if (os != DBNull.Value)
                                        {
                                            xs = Convert.ToDouble(os);
                                        }
                                    }
                                }

                                if (resultTable.Columns.Contains(colName))
                                {
                                    year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                                    object os = dataRowResult[colName];

                                    if (os != DBNull.Value)
                                    {
                                        xs = Convert.ToDouble(os);
                                    }
                                }

                                if (year.HasValue)
                                {
                                    lst.Add(new ChartData { Year = year.Value, Xp = xp, Xs = xs });
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Выбран не известный показатель");
                    }
                }
            }

            var script = String.Empty;
            double? min = null;
            double? max = null;
            double delta;

            foreach (ChartData item in lst)
            {
                double? value = null;

                if (item.Xp.HasValue)
                {
                    value = item.Xp.Value;

                    if (!min.HasValue)
                    {
                        min = value;
                    }

                    if (!max.HasValue)
                    {
                        max = value;
                    }

                    if (value < min.Value)
                    {
                        min = value;
                    }

                    if (value > max.Value)
                    {
                        max = value;
                    }
                }

                if (item.Xs.HasValue)
                {
                    value = item.Xs.Value;

                    if (!min.HasValue)
                    {
                        min = value;
                    }

                    if (!max.HasValue)
                    {
                        max = value;
                    }

                    if (value < min.Value)
                    {
                        min = value;
                    }

                    if (value > max.Value)
                    {
                        max = value;
                    }
                }
            }

            if (min.HasValue && max.HasValue)
            {
                delta = (max.Value - min.Value) / 10;

                min -= delta / 2;
                max += delta / 2;

                int dig;
                if (delta > 1)
                {
                    var tmp = Convert.ToInt32(Math.Round(delta)).ToString();
                    dig = tmp.Length;
                    if (dig > 3)
                    {
                        min = Math.Round(min.Value);
                        max = Math.Round(max.Value);
                        delta = Math.Round(delta);
                    }
                    else
                    {
                        min = Math.Round(min.Value, 4 - dig);
                        max = Math.Round(max.Value, 4 - dig);
                        delta = Math.Round(delta, 4 - dig);
                    }
                }
                else
                {
                    dig = 4;
                    min = Math.Round(min.Value, dig);
                    max = Math.Round(max.Value, dig);
                    delta = Math.Round(delta, dig);
                }

                script = String.Format(
@"if (chart1.isVisible()) {{
    chart1.setYAxis(new Ext.chart.NumericAxis({{
        minimum: {0},
        maximum: {1},
        majorUnit: {2},
        orientation: 'vertical'
    }}))
}}",
                        min.Value.ToString().Replace(",", "."),
                        max.Value.ToString().Replace(",", "."),
                        delta.ToString().Replace(",", "."));
            }

            return new AjaxStoreExtraResult(lst, lst.Count, script);
        }

        public ActionResult Calc(string key, int year)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable ydata = ufc.DataService.GetProgData();
            DataTable factorsData = ufc.DataService.GetStaticData();

            SortedList<string, double> coeffsList = ufc.DataService.GetCoeff();
            
            int numOfYear = 8;
            int numOfFactors = 3;

            double[] previousYear; //// = new double[] { 10, 5, 1 }; ////numOfFactors
            double[] y; //// = new double[] { 1, 2, 2, 4, 3, 2, 4, 5 }; ////numOfYear

            SortedList<int, double> ydataList = new SortedList<int, double>();

            foreach (DataColumn col in ydata.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    int colYear = Convert.ToInt32(colName.Replace("year_", String.Empty));
                    if (colYear > year)
                    {
                        double data = Convert.ToDouble(ydata.Rows[0][colName]);

                        ydataList.Add(colYear, data);
                    }
                }
            }

            y = ydataList.Values.ToArray();

            Dictionary<int, double> estValues = new Dictionary<int, double>();

            foreach (DataRow row in factorsData.Rows)
            {
                double data = Convert.ToDouble(row["year_{0}".FormatWith(year)]);
                int rowid = Convert.ToInt32(row["id"]);
                if (rowid != Convert.ToInt32(ydata.Rows[0]["id"]))
                {
                    estValues.Add(rowid, data);
                }
            }

            previousYear = estValues.Values.ToArray();

            double[] coeff; //// = new double[] { 1, 0.3, -0.4, 0.2 }; ////numOfFactors + 1

            coeff = coeffsList.Values.ToArray();
            
            ReverseRegression revRegression = new ReverseRegression(previousYear, y, coeff);
            double[,] res = revRegression.Compute();

            DataTable resultTable = ufc.GetObject("ResultTable") as DataTable;
            resultTable.PrimaryKey = new DataColumn[] { resultTable.Columns["id"] };

            int j = 0;
            foreach (DataRow row in factorsData.Rows)
            {
                if (Convert.ToInt32(row["id"]) != ufc.ParamId)
                {
                    foreach (DataColumn col in ydata.Columns)
                    {
                        string colName = col.ColumnName;

                        if (colName.Contains("year_"))
                        {
                            int colYear = Convert.ToInt32(colName.Replace("year_", String.Empty));

                            if (colYear > year)
                            {
                                if (!resultTable.Columns.Contains(colName))
                                {
                                    resultTable.Columns.Add(colName, typeof(double));
                                }

                                if (!resultTable.Rows.Contains(row["id"]))
                                {
                                    var newRow = resultTable.NewRow();
                                    newRow["id"] = row["id"];
                                    newRow["Param"] = row["Param"];

                                    resultTable.Rows.Add(newRow);
                                }

                                resultTable.Rows.Find(row["id"])[colName] = res[j, colYear - year - 1];
                            }
                        }
                    }
                    
                    j++;
                }
            }

            return ar;
        }

        public ActionResult Save(string key, int year)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable ydata = ufc.DataService.GetProgData();
            DataTable resultTable = ufc.GetObject("ResultTable") as DataTable;

            int varid = Convert.ToInt32(ufc.GetObject("VarId"));
            
            var extVar = varid == -1 ? null : variantsRepository.FindOne(varid);
            
            foreach (DataRow row in ydata.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);
                var refparam = paramsRepository.FindOne(paramid);

                foreach (DataColumn col in ydata.Columns)
                {
                    string colName = col.ColumnName;
                    if (colName.Contains("year_"))
                    {
                        var colYear = Convert.ToInt32(colName.Replace("year_", String.Empty));

                        if (colYear > year)
                        {
                            if (row[colName] != DBNull.Value)
                            {
                                D_Forecast_RevValues value = new D_Forecast_RevValues()
                                {
                                    Value = Convert.ToDecimal(row[colName]),
                                    RefParam = refparam,
                                    RefRev = extVar,
                                    RefDate = yearRepository.Get((colYear * 10000) + 1)
                                };

                                valuesRepository.Save(value);

                                valuesRepository.DbContext.CommitChanges();
                            }
                        }
                    }
                }
            }

            foreach (DataRow row in resultTable.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);
                var refparam = paramsRepository.FindOne(paramid);

                foreach (DataColumn col in resultTable.Columns)
                {
                    string colName = col.ColumnName;
                    if (colName.Contains("year_"))
                    {
                        var colYear = Convert.ToInt32(colName.Replace("year_", String.Empty));

                        if (row[colName] != DBNull.Value)
                        {
                            D_Forecast_RevValues value = new D_Forecast_RevValues()
                            {
                                Value = Convert.ToDecimal(row[colName]),
                                RefParam = refparam,
                                RefRev = extVar,
                                RefDate = yearRepository.Get((colYear * 10000) + 1)
                            };

                            valuesRepository.Save(value);

                            valuesRepository.DbContext.CommitChanges();
                        }
                    }
                }
            }
            
            return ar;
        }

        public ActionResult ChangeData(int rowid, string col, double newVal, string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datY = ufc.DataService.GetProgData();

            datY.PrimaryKey = new DataColumn[] { datY.Columns["id"] };

            DataRow row = datY.Rows.Find(rowid);

            if (row != null)
            {
                if (Convert.ToInt32(row["id"]) == rowid)
                {
                    row[col] = newVal;
                }
            }

            return ajaxResult;
        }
    }
}
