using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ProgDataController : SchemeBoundController
    {
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastValuesRepository valuesRepository;
        
        private readonly IForecastForma2pVarRepository form2pVarRepository;
        private readonly IForecastForma2pValueRepository form2pValuesRepository;
        private readonly IForecastForma2pRepository form2pParamRepository;

        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;
        private readonly IRepository<FX_Forecast_PType> ptypeRepository;
        
        private readonly ILinqRepository<FX_FX_KindOfForecasts> forecastType;
        ////private UserFormsControls ufc;
       /* private DataTable datProg; //// = new DataTable();
        private DataTable datStatic; //// = new DataTable();
        private List<Criteria> lstCrit;*/

        private readonly IExportService ExportService;
        
        private IForecastExtension extension;
        
        public ProgDataController(
            IForecastExtension extension,
            IExportService exportService,
            IForecastVariantsRepository variantsRepository,
            IForecastParamsRepository paramsRepository, 
            IForecastValuesRepository valuesRepository,
            IForecastForma2pVarRepository form2pVarRepository,
            IForecastForma2pValueRepository form2pValuesRepository,
            IForecastForma2pRepository form2pParamRepository,
            IRepository<FX_Date_YearDayUNV> yearRepository,
            IRepository<FX_Forecast_PType> ptypeRepository,
            ILinqRepository<FX_FX_KindOfForecasts> forecastType)
        {
            this.extension = extension;
            this.ExportService = exportService;
            
            /*datProg = ufc.GetObject("dtProg") as DataTable;
            datStatic = ufc.GetObject("dtStatic") as DataTable;
            lstCrit = ufc.GetObject("lstCrit") as List<Criteria>;*/
            /*datProg = ufc.DataService.GetProgData();
            datStatic = ufc.DataService.GetStaticData();
            lstCrit = ufc.DataService.GetCriteria();*/
            
            this.variantsRepository = variantsRepository;
            this.paramsRepository = paramsRepository;
            this.valuesRepository = valuesRepository;
            this.yearRepository = yearRepository;
            this.ptypeRepository = ptypeRepository;
            this.form2pVarRepository = form2pVarRepository;
            this.form2pValuesRepository = form2pValuesRepository;
            this.form2pParamRepository = form2pParamRepository;
            this.forecastType = forecastType;
        }
        
        public ActionResult ChangeFormula(int group, int method, string key)
        {
            string imgpath = String.Format("/PlanningFormulaCoeff/FormulaImage/{0}", (group * 100) + method);
            
            UserFormsControls ufc = this.extension.Forms[key];

            List<ForecastStruct> forecastList = ufc.DataService.GetForecastList();

            if (forecastList.Count == 1)
            {
                ForecastStruct fstruct = forecastList.First();
                fstruct.Group = Convert.ToInt32(group);
                fstruct.Method = method;
                forecastList.Clear();
                forecastList.Add(fstruct);
            }

            var mathGroup = extension.LoadedMathGroups.GetGroupByCode(group);

            var txtAreascript = String.Empty;

            if (mathGroup.HasValue)
            {
                var mathMethod = mathGroup.Value.Methods.GetMethodByCode(method);

                if (mathMethod.HasValue)
                {
                    txtAreascript = String.Format("textArea.setValue(\"{0}\");", mathMethod.Value.Description);
                }
            }

            string imgScript = String.Format("imgFormula.setImageUrl('{0}');", imgpath);

            return new AjaxResult { Script = String.Concat(txtAreascript, imgScript) };
        }

        public ActionResult ChangeFStruct(string group, int method, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            List<ForecastStruct> forecastList = ufc.DataService.GetForecastList();

            if (forecastList.Count == 1)
            {
                ForecastStruct fstruct = forecastList.First();
                fstruct.Group = Convert.ToInt32(group);
                fstruct.Method = method;
                forecastList.Clear();
                forecastList.Add(fstruct);
            }

            ar.Result = "success";

            return ar;
        }
        
        public ActionResult Calc(int predCount, string group, int method, string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();
            DataTable datStatic = ufc.DataService.GetStaticData();
            datStatic.PrimaryKey = new DataColumn[] { datStatic.Columns["id"] };

            DataTable datReg = ufc.DataService.GetRegulatorData();
            List<Criteria> lstCrit = ufc.DataService.GetCriteria();
            SortedList<string, double> coeff = ufc.DataService.GetCoeff();
            SortedList<int, bool> arrYear = ufc.DataService.GetArrYears();
            List<ForecastStruct> foreStruct = ufc.DataService.GetForecastList();

            int yearCount = (from f in arrYear
                    where f.Value
                    select f).ToList().Count();

            if (yearCount < 3)
            {
                ar.Result = "failure";
                ar.Script = @"Ext.MessageBox.show({
                                                title: 'Ошибка',
                                                msg: 'Количество выбранных для прогноза статистических данными должно быть более чем за 3 года.',
                                                buttons: Ext.MessageBox.OK,
                                                icon: Ext.MessageBox.ERROR
                                            });";
                return ar;
            }

            int colNum = 0;
            foreach (DataColumn col in datStatic.Columns)
            {
                if (col.ColumnName.Contains("year_"))
                {
                    colNum++;
                }
            }
            
            double[] pred = new double[] { };
            double[] data = new double[colNum];

            Dictionary<string, double[]> calculatedDatas = new Dictionary<string, double[]>();
            
            foreach (ForecastStruct fstruct in foreStruct)
            {
                int fparamId = fstruct.ForecastingParam.ParamId;
                string fparamName = fstruct.ForecastingParam.Name;
                
                Dictionary<int, double[]> inData = new Dictionary<int, double[]>();
                Dictionary<int, double[]> inRegs = new Dictionary<int, double[]>();
                
                ////int k = 0;
                foreach (ForecastParameter param in fstruct.UsedParams)
                {
                    int paramid = param.ParamId;
                    var rows = from row in datStatic.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == paramid
                               select row;

                    if (rows.Count() > 0)
                    {
                        DataRow drow = rows.First();

                        // Итератор столбцов
                        int j = 0;

                        double[] tmpData = new double[colNum];

                        foreach (DataColumn col in datStatic.Columns)
                        {
                            string colName = col.ColumnName;
                            if (colName.Contains("year_"))
                            {
                                double val;
                                if (drow[colName] != DBNull.Value)
                                {
                                   val = Convert.ToDouble(drow[colName]);
                                }
                                else
                                {
                                    val = 0;
                                }

                                tmpData[j] = val;
                                j++;
                            }
                        }
                        
                        inData.Add(paramid, tmpData);

                  ////      k++;
                    }
                }

                foreach (ForecastRegulator regulator in fstruct.UsedRegs)
                {
                    int regid = regulator.RegId;
                    var rows = from row in datReg.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == regid
                               select row;

                    if (rows.Count() > 0)
                    {
                        DataRow drow = rows.First();

                        // Итератор столбцов
                        int j = 0;

                        double[] tmpData = new double[colNum];

                        foreach (DataColumn col in datStatic.Columns)
                        {
                            string colName = col.ColumnName;
                            if (colName.Contains("year_"))
                            {
                                double val;
                                if (drow[colName] != DBNull.Value)
                                {
                                    val = Convert.ToDouble(drow[colName]);
                                }
                                else
                                {
                                    val = 0;
                                }

                                tmpData[j] = val;
                                j++;
                            }
                        }

                        inRegs.Add(regid, tmpData);

                        ////k++;
                    }
                }

                ////int rowCount = k;

                List<EquationError> equationErrors;
                
                double[] tmpPred = Equation.Calc(inData, inRegs, calculatedDatas, fstruct, fparamId, fstruct.Group, fstruct.Method, arrYear, predCount, coeff, extension.LoadedMathGroups, out equationErrors);

                if (equationErrors.Count > 0)
                {
                    foreach (EquationError error in equationErrors)
                    {
                        switch (error.Group)
                        {
                            case FixedMathGroups.MultiRegression:
                                int paramId = error.Data is int ? (int)error.Data : 0;

                                string item = "\"{0}\"".FormatWith((from f in paramsRepository.GetAllParams()
                                           where f.ID == paramId
                                           select f.Name).ToList().First());

                                ar.Script += @"Ext.MessageBox.show({{
                                                title: 'Ошибка',
                                                msg: '{0}',
                                                buttons: Ext.MessageBox.OK,
                                                icon: Ext.MessageBox.ERROR
                                            }});".FormatWith(error.Text.FormatWith(item));
                                break;
                        }
                    }
                }

                calculatedDatas.Add(fparamName, tmpPred);

                if (fparamId == ufc.ParamId)
                {
                    Array.Resize(ref pred, tmpPred.Length);
                    
                    tmpPred.CopyTo(pred, 0);
                    inData[fparamId].CopyTo(data, 0);
                }
            }
            
            lstCrit.Clear();

            lstCrit.Add(new Criteria { Name = "MSE", Value = Criterias.MSE(data, pred), Text = Criterias.Descript_MSE(data, pred) });
            lstCrit.Add(new Criteria { Name = "MAE", Value = Criterias.MAE(data, pred), Text = Criterias.Descript_MAE(data, pred) });
            lstCrit.Add(new Criteria { Name = "DW", Value = Criterias.DW(data, pred), Text = Criterias.Descript_DW(data, pred) });
            lstCrit.Add(new Criteria { Name = "R2", Value = Criterias.R2(data, pred), Text = Criterias.Descript_R2(data, pred) });
            lstCrit.Add(new Criteria { Name = "F", Value = Criterias.Fcrit(data, pred, 4), Text = Criterias.Descript_F(data, pred, 4) });

            /////double[] cr = Criterias.Criterii(data, pred);
            
            /*i = 0;
            foreach (DataColumn col in datProg.Columns)
            {
                if (col.ColumnName.Contains("year_"))
                {
                    datProg.Rows[0][col.ColumnName] = Math.Round(pred[i], 6);
                    i++;
                }
            }*/

            int rowNum;
            for (rowNum = 0; rowNum < datProg.Rows.Count; rowNum++)
            {
                if (Convert.ToInt32(datProg.Rows[rowNum]["id"]) == ufc.ParamId)
                {
                    break;
                }
            }
            
            int i = 0;
            foreach (DataColumn column in datProg.Columns)
            {
                if (column.ColumnName.Contains("year_"))
                {
                    if (i < pred.Length)
                    {
                        datProg.Rows[rowNum][column.ColumnName] = Math.Round(pred[i], 6);
                    }

                    i++;
                }
            }

            datProg.AcceptChanges();
            
            double? min;
            double? max;
            double? delta;

            DataRow rowStatic = (from row in datStatic.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();
            DataRow rowProg = (from row in datProg.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == ufc.ParamId
                               select row).First();

            string paramName = Convert.ToString(rowProg["Param"]);

            PlanningChartControl.ChartRange(rowStatic, rowProg, out min, out max, out delta);

            if (delta.HasValue)
            {
                var script = String.Format(
@"if (chart1.isVisible()) {{
    chart1.setYAxis(new Ext.chart.NumericAxis({{
        title: '{0}',
        minimum: {1},
        maximum: {2},
        majorUnit: {3},
        orientation: 'vertical'
    }}))
}};",
                    paramName,
                    min.Value.ToString().Replace(",", "."),
                    max.Value.ToString().Replace(",", "."),
                    delta.Value.ToString().Replace(",", "."));

                ar.Script += script;
            }

            return ar;
        }
        
        public ActionResult GetMethod(int item)
        {
            List<ListItem> list = new List<ListItem>();

            var group = extension.LoadedMathGroups.GetGroupByCode(item);

            if (group.HasValue)
            {
                foreach (Method method in group.Value.Methods)
                {
                    list.Add(new ListItem { Text = method.TextName, Value = method.Code.ToString() });
                }
            }
            
            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult InsertToForm2p(int varForm2p, string key)
        {
            AjaxResult ar = new AjaxResult();

            var list = (from f in form2pValuesRepository.GetAllValues()
                       where (f.RefVarF2P.ID == varForm2p)
                       select f).ToList();

            var form2p = form2pVarRepository.FindOne(varForm2p);

            var refForecastType = (from f in forecastType.FindAll()
                                   where f.ID == -1
                                   select f).First();

            if (form2p == null)
            {
                throw new Exception("Ошибка! Варинат с заданный ID не существует");
            }

            var baseYear = form2p.RefYear.ID;

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();

            try
            {
                form2pValuesRepository.DbContext.BeginTransaction();

                foreach (DataRow row in datProg.Rows)
                {
                    var paramid = Convert.ToInt32(row["id"]);

                    string signat = (from f in paramsRepository.GetAllParams()
                                  where f.ID == paramid
                                  select f.Signat).First();

                    if (!String.IsNullOrEmpty(signat))
                    {
                        var form2pParam = (from f in form2pParamRepository.GetAllParams()
                                           where f.Signat == signat
                                          select f).First();
                        int form2pParamID = form2pParam.ID;

                        if (form2pParamID != null)
                        {
                            foreach (DataColumn column in datProg.Columns)
                            {
                                var colName = column.ColumnName;
                                if (colName.Contains("year_"))
                                {
                                    var year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                                    T_Forecast_ParamValues data = null;

                                    if ((year >= baseYear - 2) && (year <= baseYear + 3))
                                    {
                                        int paramType = 0;
                                        if (year < baseYear)
                                        {
                                            var tmpList = from f in list
                                                          where (f.RefParametrs.ID == form2pParamID) && (f.YearOf == year) && (f.ParamType == 1)
                                                          select f;
                                            paramType = 1;

                                            if (tmpList.Count() > 0)
                                            {
                                                data = tmpList.First();
                                            }
                                        }

                                        if (year == baseYear)
                                        {
                                            var tmpList = from f in list
                                                          where (f.RefParametrs.ID == form2pParamID) && (f.YearOf == year) && (f.ParamType == 2)
                                                          select f;
                                            paramType = 2;
                                            if (tmpList.Count() > 0)
                                            {
                                                data = tmpList.First();
                                            }
                                        }

                                        if (year > baseYear)
                                        {
                                            var tmpList = from f in list
                                                          where (f.RefParametrs.ID == form2pParamID) && (f.YearOf == year) && (f.ParamType == 3)
                                                          select f;
                                            paramType = 3;

                                            if (tmpList.Count() > 0)
                                            {
                                                data = tmpList.First();
                                            }
                                        }

                                        if (data != null)
                                        {
                                            data.Value = Convert.ToDecimal(row[colName]);
                                            form2pValuesRepository.Save(data);
                                        }
                                        else
                                        {
                                            T_Forecast_ParamValues tfpv = new T_Forecast_ParamValues
                                            {
                                                Value = Convert.ToDecimal(row[colName]),
                                                RefVarF2P = form2p,
                                                RefForecastType = refForecastType,
                                                RefParametrs = form2pParam,
                                                YearOf = year,
                                                ParamType = paramType
                                            };

                                            form2pValuesRepository.Save(tfpv);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                form2pValuesRepository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                form2pValuesRepository.DbContext.RollbackTransaction();
                ar.Result = false;
                throw new Exception(e.Message, e);
            }

            return ar;
        }

        public ActionResult VarForm2pStore()
        {
            var list = (from f in form2pVarRepository.GetAllVariants()
                       select new
                       {
                           Text = f.Name,
                           Value = f.ID,
                           Year = f.RefYear.ID
                       }).ToList();

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult Save(int paramId, string name, int dataFrom, int dataTo, int progFrom, int progTo, string group, string method, int status/*, bool[] usedYears*/, string key, int varform2p)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();
            DataTable datStatic = ufc.DataService.GetStaticData();
            ////DataTable datRegulators = ufc.DataService.GetRegulatorData();

            List<ForecastStruct> foreStruct = ufc.DataService.GetForecastList();

            var paramlist = ufc.DataService.GetCoeff();
            var critlist = ufc.DataService.GetCriteria();

            SortedList<int, bool> usedYears = ufc.DataService.GetArrYears(); //// { true, false, true, false };
            
            int varid = ufc.VarID;

            int period = progTo - progFrom + 1;
            ////int cntUsedYears = dataTo - dataFrom + 1;
            
            /*for (int i = 0; i < cntUsedYears; i++)
            {
                usedYears[i] = true;
            }*/

            XDocument xDoc = new XDocument();

            XElement xEl = new XElement(
                "Forecast", 
                new XAttribute("from", progFrom),
                new XAttribute("to", progTo),
                new XAttribute("varform2p", varform2p));

            XElement xElUsedDatas = new XElement(
                "UsedDatas",
                new XAttribute("fparamid", ufc.ParamId),
                new XAttribute("group", group),
                new XAttribute("method", method));

            foreach (ForecastStruct forecastStruct in foreStruct)
            {
                XElement xElUsedData = new XElement(
                    "UsedData",
                    new XAttribute("fparamid", forecastStruct.ForecastingParam.ParamId),
                    new XAttribute("name", forecastStruct.ForecastingParam.Name),
                    new XAttribute("group", forecastStruct.Group),
                    new XAttribute("method", forecastStruct.Method));

                if (!String.IsNullOrEmpty(forecastStruct.Expression))
                {
                    XElement xElExp = new XElement("Expression", forecastStruct.Expression);
                    xElUsedData.Add(xElExp);
                }

                foreach (ForecastParameter parameter in forecastStruct.UsedParams)
                {
                    XElement xElParams = new XElement(
                        "Data", 
                        new XAttribute("paramid", parameter.ParamId),
                        new XAttribute("name", parameter.Name));
                    xElUsedData.Add(xElParams);
                }

                foreach (ForecastRegulator parameter in forecastStruct.UsedRegs)
                {
                    XElement xElParams = new XElement(
                        "Regulator",
                        new XAttribute("regid", parameter.RegId),
                        new XAttribute("name", parameter.Name),
                        new XAttribute("fvar", parameter.FVar));
                    xElUsedData.Add(xElParams);
                }

                xElUsedDatas.Add(xElUsedData);
            }
            
            xEl.Add(xElUsedDatas);

            XElement xElUsedYears = new XElement("UsedYears", new XAttribute("from", dataFrom), new XAttribute("to", dataTo));

            foreach (KeyValuePair<int, bool> usedYear in usedYears)
            {
                if (usedYear.Value)
                {
                    XElement xElUsedYear = new XElement("Year", usedYear.Key);
                    xElUsedYears.Add(xElUsedYear);
                }    
            }
            
            xEl.Add(xElUsedYears);

            XElement xElMethodParams = new XElement("MethodParams", new XAttribute("group", group), new XAttribute("method", method));

            foreach (KeyValuePair<string, double> valuePair in paramlist)
            {
                string value = Convert.ToString(valuePair.Value);
                XElement xElParams = new XElement("Param", new XAttribute("name", valuePair.Key), value);
                xElMethodParams.Add(xElParams);
            }
            
            xEl.Add(xElMethodParams);
            
            XElement xElMethodStat = new XElement("MethodStat", new XAttribute("group", group), new XAttribute("method", method));

            foreach (Criteria crit in critlist)
            {
                string value = Convert.ToString(crit.Value);
                XElement xElParams = new XElement("Crit", new XAttribute("name", crit.Name), new XAttribute("text", crit.Text), value);
                xElMethodStat.Add(xElParams);
            }

            xEl.Add(xElMethodStat);

            xDoc.Add(xEl);

            var extVar = varid == -1 ? null : variantsRepository.FindOne(varid);

            var param = paramsRepository.FindOne(paramId);
                        
            /*select new
                        {
                            t.ID,
                            t.ParamName,
                            t.ParentID,
                            t.Prognoseable,
                            t.RefOKEI,
                            t.RowType,
                            t.XMLString,
                            t.Form2P
                        }.;*/

            if (extVar != null)
            {
                extVar.Name = name;
                ////extVar.RefParam = param;
                extVar.XMLString = xDoc.ToString();
                extVar.RefDate = yearRepository.Get((progFrom * 10000) + 1);
                extVar.Period = period;
                extVar.Status = status;
                extVar.Method = (Convert.ToInt32(group) * 100) + Convert.ToInt16(method);

                variantsRepository.Save(extVar);

                variantsRepository.DbContext.CommitChanges();
            }
            else
            {
                D_Forecast_PVars planVar = new D_Forecast_PVars
                {
                    Name = name,
                    UserID = extension.UserID,
                    XMLString = xDoc.ToString(),
                    RefParam = param,
                    RefDate = yearRepository.Get((progFrom * 10000) + 1),
                    Period = period,
                    Status = status,
                    Method = (Convert.ToInt32(group) * 100) + Convert.ToInt16(method)
                };

                variantsRepository.Save(planVar);

                variantsRepository.DbContext.CommitChanges();

                extVar = planVar;
            }

            var existValues = valuesRepository.FindAll().Where(t => t.RefVar.ID == varid).ToList(); /*from t in valuesRepository.GetAllValue()
                              where t.RefVar.ID == varid
                              select t;*/

            valuesRepository.DbContext.BeginTransaction();

            if (existValues.Count() > 0)
            {
                foreach (var value in existValues)
                {
                    valuesRepository.Delete(value);
                }
                
                /////valuesRepository.DbContext.CommitChanges();  // may be in transcation?

                ////valuesRepository.Delete(existValues.First());
            }

            foreach (DataRow row in datProg.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);
                var refparam = paramsRepository.FindOne(paramid);

                foreach (DataColumn col in datProg.Columns)
                {
                    string colName = col.ColumnName;
                    if (colName.Contains("year_"))
                    {
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                        if (row[colName] != DBNull.Value)
                        {
                            FX_Forecast_PType fxstat = (from f in ptypeRepository.GetAll()
                                                       where (f.ID == 0)
                                                       select f).ToList().First();

                            D_Forecast_PValues value = new D_Forecast_PValues
                            {
                                Value = Convert.ToDecimal(row[colName]),
                                RefParam = refparam,
                                RefVar = extVar,
                                RefDate = yearRepository.Get((year * 10000) + 1),
                                RefStat = fxstat
                            };

                            valuesRepository.Save(value);
                        }
                    }
                }
            }

            foreach (DataRow row in datStatic.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);
                var refparam = paramsRepository.FindOne(paramid);

                foreach (DataColumn col in datStatic.Columns)
                {
                    string colName = col.ColumnName;
                    if (colName.Contains("year_"))
                    {
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                        if (row[colName] != DBNull.Value)
                        {
                            FX_Forecast_PType fxprog = (from f in ptypeRepository.GetAll()
                                                        where (f.ID == 1)
                                                        select f).ToList().First();

                            D_Forecast_PValues value = new D_Forecast_PValues
                            {
                                Value = Convert.ToDecimal(row[colName]),
                                RefParam = refparam,
                                RefVar = extVar,
                                RefDate = yearRepository.Get((year * 10000) + 1),
                                RefStat = fxprog
                            };

                            valuesRepository.Save(value);
                        }
                    }
                }
            }

            valuesRepository.DbContext.CommitTransaction();
            ////valuesRepository.DbContext.CommitChanges();

            ////variantsRepository.Save();))
            
            return ajaxResult;
        }

        /// <summary>
        /// Загружает DataTable'ы
        /// </summary>
        /// <param name="key">ключь вкладки</param>
        /// <returns>результат успешности</returns>
        public ActionResult Load(string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();
            DataTable datStatic = ufc.DataService.GetStaticData();

            int varid = ufc.VarID;

            ////var existValues = valuesRepository.FindAll().Where(t => t.RefVar.ID == varid).ToList();

            foreach (DataRow row in datProg.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);

                foreach (DataColumn column in datProg.Columns)
                {
                    if (column.ColumnName.Contains("year_"))
                    {
                        var colName = column.ColumnName;
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                        var yearRef = yearRepository.Get((year * 10000) + 1);
                        var varRef = variantsRepository.FindOne(varid);
                        var paramRef = paramsRepository.FindOne(paramid);
                        ////var existValue = valuesRepository.FindAll().Where(t => t.RefVar.ID == varid).ToList();

                        /*var list = (from t in valuesRepository.FindAll()
                                         where (t.RefVar == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t).ToList();*/
                        var existValue = from t in valuesRepository.FindAll()
                                         where (t.RefStat.ID == 0) && (t.RefVar == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t.Value;
                        var lst  = existValue.ToList();
                        if (lst.Count() > 0)
                        {
                            row[colName] = existValue.First();
                        }
                    }
                }
            }

            foreach (DataRow row in datStatic.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);

                foreach (DataColumn column in datStatic.Columns)
                {
                    if (column.ColumnName.Contains("year_"))
                    {
                        var colName = column.ColumnName;
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                        var yearRef = yearRepository.Get((year * 10000) + 1);
                        var varRef = variantsRepository.FindOne(varid);
                        var paramRef = paramsRepository.FindOne(paramid);
                        ////var existValue = valuesRepository.FindAll().Where(t => t.RefVar.ID == varid).ToList();
                        var existValue = from t in valuesRepository.FindAll()
                                         where (t.RefVar == varRef) && (t.RefDate == yearRef) && (t.RefStat.ID == 1) && (t.RefParam == paramRef)
                                         select t.Value;
                        if (existValue.ToList().Count() > 0)
                        {
                            row[colName] = existValue.First();
                        }
                    }
                }
            }

            ajaxResult.Result = "success";

            return ajaxResult;
        }
        
        public ActionResult ExportVariant(int varid)
        {
            var stream = ExportService.ExportPlanResult(varid);

            ////var file = File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(variantsRepository.FindOne(varid).Name));

            ////var s = file.ContentType;

            var s2 = "{0}.xls".FormatWith(variantsRepository.FindOne(varid).Name); ////file.FileDownloadName;

            byte[] buff = new byte[stream.Length];

            stream.Read(buff, 0, buff.Length);

            FileContentResult fcr = new FileContentResult(buff, "application/vnd.ms-excel");

            fcr.FileDownloadName = s2;

            /* ActionResult ar = file;

            Response.Clear();

            StringBuilder builder = new StringBuilder(255);
            builder.Append("attachment;filename=\"");
            builder.Append(s2);
            builder.Append('_');
            builder.Append(FileHelper.GetDownloadableFileName(s2));
            builder.Append('"');

            Response.ContentType = s;
            Response.AddHeader("Content-Disposition", builder.ToString());
            ////Response.OutputStream.Write(stream, 0, data.GetLength(0));
            
            byte[] buff = new byte[stream.Length];

            stream.Read(buff, 0, buff.Length);

            Response.OutputStream.Write(buff, 0, buff.Length);

            var s3 = FileHelper.GetDownloadableFileName(s2);*/
            
            return fcr;
        }

        public ActionResult PumpData(string key)
        {
            AjaxResult ar = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];
            
            DataTable datStatic = ufc.DataService.GetStaticData();

            DataTable datProg = ufc.DataService.GetProgData();

            ////var variant = variantsRepository.FindOne(varId);

            /*XDocument xDoc = XDocument.Parse(variant.XMLString);

            var usedDatas = xDoc.Root.Element("UsedDatas");
            var usedData = usedDatas.Element("UsedData");

            var usedYears = xDoc.Root.Element("UsedYears");

            int dataFrom = Convert.ToInt32(usedYears.Attribute("From").Value);
            int dataTo = Convert.ToInt32(usedYears.Attribute("To").Value);*/

            foreach (DataRow dataRow in datStatic.Rows)
            {
                int paramId = Convert.ToInt32(dataRow["id"]);

                var param = paramsRepository.FindOne(paramId);

                XDocument xParamDoc = XDocument.Parse(param.XMLString);

                var root = xParamDoc.Root;

                ////string paramName = root.Attribute("name").Value;
                double unitsTo = Convert.ToDouble(root.Attribute("units").Value);

                var table = root.Element("table");

                string fromTable = table.Attribute("name").Value;

                var dataEl = table.Element("data");
                string fieldValue = dataEl.Value;
                double unitsFrom = Convert.ToDouble(dataEl.Attribute("units").Value);

                var yearEl = table.Element("year");

                string yearFormat = yearEl.Attribute("format").Value;
                string filedYear = yearEl.Value;

                XElement filterEls = table.Element("filters");

                ////Dictionary<string, string > flist = new Dictionary<string, string>();

                var elements = filterEls.Elements();

                string filter = String.Empty;
                if (elements.Count() > 0)
                {
                    int i = 0;

                    string[] filters = new string[elements.Count()];

                    foreach (var element in elements)
                    {
                        string fname = element.Attribute("name").Value;
                        string fvalue = element.Value;
                        /////flist.Add(fname, fvalue);
                        filters[i] = String.Format("({0} = {1})", fname, fvalue);
                        i++;
                    }

                    filter = String.Format(" and {0}", String.Join(" and ", filters));
                }
                ////Core.Resolver.

                /*var repo = Core.Resolver.Get<NHibernateLinqRepository<D_Forecast_PlanningParams>>();

                repo.FindAll().Where(*/

                ////double k = unitsFrom / unitsTo;

                ////for (int curyear = dataFrom; curyear <= dataTo; curyear++)
                foreach (DataColumn col in datStatic.Columns)
                {
                    string colName = col.ColumnName;
                    if (colName.Contains("year_"))
                    {
                        int curyear = Convert.ToInt32(colName.Replace("year_", String.Empty));
                        int year;

                        switch (yearFormat)
                        {
                            case "unv":
                                year = (curyear * 10000) + 0001;
                                break;
                            case "year":
                            default:
                                year = curyear;
                                break;
                        }

                        string selectSQL = String.Format("select {0}*{1}/{2} from {3} where ({4}={5}) {6}", fieldValue, unitsFrom, unitsTo, fromTable, filedYear, year, filter);
                        var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

                        if (queryResult.Count > 0)
                        {
                            dataRow[colName] = queryResult[0];
                        }
                        else
                        {
                            dataRow[colName] = DBNull.Value;
                        }
                    }
                }
            }

            double? min;
            double? max;
            double? delta;

            DataRow rowStatic = (from row in datStatic.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();
            DataRow rowProg = (from row in datProg.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == ufc.ParamId
                               select row).First();

            string paramName = Convert.ToString(rowProg["Param"]);

            PlanningChartControl.ChartRange(rowStatic, rowProg, out min, out max, out delta);

            if (delta.HasValue)
            {
                var script = String.Format(
@"if (chart1.isVisible()) {{
    chart1.setYAxis(new Ext.chart.NumericAxis({{
        title: '{0}',
        minimum: {1},
        maximum: {2},
        majorUnit: {3},
        orientation: 'vertical'
    }}))
}}",
                    paramName,
                    min.Value.ToString().Replace(",", "."),
                    max.Value.ToString().Replace(",", "."),
                    delta.Value.ToString().Replace(",", "."));

                ar.Script = script;
            }

            ar.Result = "success";
            return ar;
        }

        public ActionResult BeginChangeYears()
        {
            AjaxResult ar = new AjaxResult();

            string script = @"
btnEdit.setVisible(false); 
btnRefresh.setVisible(true); 
panelStat.setDisabled(true); 
panelProg.setDisabled(true);
sfProgFromYear.setDisabled(false);
sfProgToYear.setDisabled(false);
sfDataFromYear.setDisabled(false);
sfDataToYear.setDisabled(false);
";

            ar.Script = script;

            return ar;
        }

        public ActionResult ApplyChangeYears(string key, int dataFrom, int dataTo, int progFrom, int progTo)
        {
            AjaxResult ar = new AjaxResult();

            if ((dataTo < dataFrom) || (progTo < progFrom) || (dataTo > progFrom))
            {
                ar.Result = "failure";
                ar.Script = @"Ext.MessageBox.show({
                        title: 'Ошибка',
                        msg: 'Не правильно заданы интервалы',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });";

                return ar;
            }

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datStatic = ufc.DataService.GetStaticData();
            DataTable datProg = ufc.DataService.GetProgData();
            SortedList<int, bool> years = ufc.DataService.GetArrYears();

            for (int i = 0; i < datStatic.Columns.Count; i++)
            {
                string colName = datStatic.Columns[i].ColumnName;
                if (colName.Contains("year_"))
                {
                    int year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                    if ((year < dataFrom) || (year > dataTo))
                    {
                        years.Remove(year);
                    }
                }
            }

            for (int i = dataFrom; i <= dataTo; i++)
            {
                if (!years.ContainsKey(i))
                {
                    years.Add(i, true);
                }
            }
            
            ResampleTable(datStatic, dataFrom, dataTo);
            ResampleTable(datProg, dataFrom, progTo);
            
/*            List<string> removeLst = new List<string>();

            for (int i = 0; i < datStatic.Columns.Count; i++)
            {
                DataColumn dataColumn = datStatic.Columns[i];
                string colName = dataColumn.ColumnName;

                if (colName.Contains("year_"))
                {
                    int year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                    if ((year < dataFrom) || (year > dataTo))
                    {
                        removeLst.Add(colName);
                        years.Remove(year);
                    }
                }
            }

            foreach (string name in removeLst)
            {
                datStatic.Columns.Remove(datStatic.Columns[name]);
            }

            removeLst.Clear();

            for (int i = 0; i < datProg.Columns.Count; i++)
            {
                DataColumn dataColumn = datProg.Columns[i];
                string colName = dataColumn.ColumnName;

                if (colName.Contains("year_"))
                {
                    int year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                    if ((year < dataFrom) || (year > progTo))
                    {
                        removeLst.Add(colName);
                        ////datProg.Columns.Remove(dataColumn);
                    }
                }
            }

            foreach (string name in removeLst)
            {
                datProg.Columns.Remove(datProg.Columns[name]);
            }

            for (int i = dataFrom; i <= dataTo; i++)
            {
                string yearName = String.Format("year_{0}", i);
                if (!datStatic.Columns.Contains(yearName))
                {
                    datStatic.Columns.Add(yearName, typeof(double));
                    
                    foreach (DataRow dataRow in datStatic.Rows)
                    {
                        dataRow[yearName] = 0.0;
                    }

                    years.Add(i, true);
                }
                
                if (!datProg.Columns.Contains(yearName))
                {
                    datProg.Columns.Add(yearName, typeof(double));

                    foreach (DataRow dataRow in datStatic.Rows)
                    {
                        dataRow[yearName] = 0.0;
                    }
                }
            }

            for (int i = progFrom; i <= progTo; i++)
            {
                string yearName = String.Format("year_{0}", i);
                if (!datProg.Columns.Contains(yearName))
                {
                    datProg.Columns.Add(yearName, typeof(double));

                    foreach (DataRow dataRow in datProg.Rows)
                    {
                        dataRow[yearName] = 0.0;
                    }
                }
            }*/
            
            string script = @"
btnEdit.setVisible(true); 
btnRefresh.setVisible(false); 
panelStat.setDisabled(false); 
panelProg.setDisabled(false);
sfProgFromYear.setDisabled(true);
sfProgToYear.setDisabled(true);
sfDataFromYear.setDisabled(true);
sfDataToYear.setDisabled(true);
";
            ar.Script = script;

            ar.Result = "success";
            
            return ar;
        }

        private void ResampleTable(DataTable datTable, int dataFrom, int dataTo)
        {
            DataTable tempTable = datTable.Copy();
            
            List<string> removeLst = new List<string>();
            
            for (int i = 0; i < datTable.Columns.Count; i++)
            {
                if (datTable.Columns[i].ColumnName.Contains("year_"))
                {
                    removeLst.Add(datTable.Columns[i].ColumnName);
                }
            }

            foreach (string name in removeLst)
            {
                datTable.Columns.Remove(datTable.Columns[name]);
            }
            
            for (int i = dataFrom; i <= dataTo; i++)
            {
                string yearName = String.Format("year_{0}", i);
                if (!datTable.Columns.Contains(yearName))
                {
                    datTable.Columns.Add(yearName, typeof(double));

                    for (int j = 0; j < datTable.Rows.Count; j++)
                    {
                        if (tempTable.Columns.Contains(yearName))
                        {
                            datTable.Rows[j][yearName] = tempTable.Rows[j][yearName];
                        }
                    }
                }
            }
        }
    }
}
