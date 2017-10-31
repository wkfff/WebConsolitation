using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    internal struct ViewForm2pValuesStruct
    {
        public int ID;
        public int RefParametrs;
        public int RefForecastType;
        public double? Est;
        public double? Y1;
        public double? Y2;
        public double? Y3;
        public double? R1;
        public double? R2;
        ////public int YearOf;
        public string Units;
        public string Signat;
        public int Code;
        public string GroupName;
        public string ParamName;
    }

    internal struct ViewForm2pValuesChartStruct
    {
        public int? Year;
        public double? Xv1;
        public double? Xv2;
    }

    internal struct VariantsValue
    {
        public int Value;
        public string Text;
        public string Group;
        public string Year;
    }

    public class Form2pValuesController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";
        
        private readonly IForecastExtension extension;
        private readonly IForecastForma2pVarRepository form2pVarRepository;
        private readonly IForecastForma2pRepository forma2pRepository;
        private readonly IForecastVariantsRepository variantRepository;
        private readonly IForecastValuesRepository valuesRepository;

        private readonly IForecastRegulatorsValueRepository regsValueRepository;
        private readonly IForecastRegulatorsRepository regsRepository;

        private readonly ILinqRepository<T_Forecast_ParamValues> form2pValueRepository;
        private readonly ILinqRepository<FX_FX_KindOfForecasts> forecastType;
        private readonly ILinqRepository<DataSources> datasourceRepository;

        public Form2pValuesController(
                                      IForecastExtension extension, 
                                      IForecastForma2pVarRepository form2pVarRepository,
                                      IForecastForma2pRepository forma2pRepository,
                                      IForecastVariantsRepository variantRepository,
                                      IForecastValuesRepository valuesRepository,
                                      ILinqRepository<T_Forecast_ParamValues> form2pValueRepository,
                                      ILinqRepository<FX_FX_KindOfForecasts> forecastType,
                                      ILinqRepository<DataSources> datasourceRepository,
                                      IForecastRegulatorsValueRepository regsValueRepository,
                                      IForecastRegulatorsRepository regsRepository)
        {
            this.extension = extension;
            this.form2pVarRepository = form2pVarRepository;
            this.forma2pRepository = forma2pRepository;
            this.form2pValueRepository = form2pValueRepository;
            this.forecastType = forecastType;
            this.datasourceRepository = datasourceRepository;
            this.variantRepository = variantRepository;
            this.valuesRepository = valuesRepository;
            this.regsValueRepository = regsValueRepository;
        }
        
        public ActionResult ShowExist(int id)
        {
            var varid = id;
            string key = String.Format("form2pForm_{0}", varid);

            if (extension.Forms.ContainsKey(key))
            {
                extension.Forms.Remove(key);
            }

            extension.Forms.Add(key, new UserFormsControls());

            var viewControl = Resolver.Get<Form2pValuesView>();
            
            UserFormsControls ufc = this.extension.Forms[key];

            ufc.AddObject("varId", varid);

            int year = (from f in form2pVarRepository.GetAllVariants()
                        where f.ID == varid
                        select new { f.RefYear.ID }).First().ID;

            ufc.AddObject("year", year);

            viewControl.Initialize(key);
            
            return View(ViewRoot + "View.aspx", viewControl);
        }
        
        public ActionResult Load(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];
            var obj = ufc.GetObject("varId");
            if (obj != null)
            {
                int varId = Convert.ToInt32(obj);

                string selectSQL;

                List<string> attributesList = new List<string>
                {
                   "v.ID",
                    "RefParametrs",
                    "RefForecastType",
                    "Est",
                    "Y1",
                    "Y2",
                    "Y3",
                    "R1",
                    "R2"
                    ////"YearOf"
                };

                string filtr = String.Format("(RefVarf2p = {0}) and (v.code not like '%00000000')", varId);
                
                string includedQuery1 = "(select o.designation from d_units_okei o where v.refunits=o.id) as units";
                string includedQuery2 = "v.signat, f.name as paramname, v.code as code";
                string includedQuery3 = "v.groupname";

                selectSQL = String.Format("Select {0}, {1}, {2}, {3} from {4} left join d_forecast_forma2p f on f.id = v.refparametrs where {5}", String.Join(", ", attributesList.ToArray()), includedQuery1, includedQuery2, includedQuery3, "v_forecast_val_form2p v", filtr);

                var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

                List<ViewForm2pValuesStruct> list = new List<ViewForm2pValuesStruct>();
                foreach (object[] row in queryResult)
                {
                    list.Add(new ViewForm2pValuesStruct
                        {
                            ID = Convert.ToInt32(row[0]),
                            RefParametrs = Convert.ToInt32(row[1]),
                            RefForecastType = Convert.ToInt32(row[2]),
                            Est = row[3] != null ? Convert.ToDouble(row[3]) : (double?)null,
                            Y1 = row[4] != null ? Convert.ToDouble(row[4]) : (double?)null,
                            Y2 = row[5] != null ? Convert.ToDouble(row[5]) : (double?)null,
                            Y3 = row[6] != null ? Convert.ToDouble(row[6]) : (double?)null,
                            R1 = row[7] != null ? Convert.ToDouble(row[7]) : (double?)null,
                            R2 = row[8] != null ? Convert.ToDouble(row[8]) : (double?)null,
                            ////YearOf = Convert.ToInt32(row[9]),
                            Units = Convert.ToString(row[9]),
                            Signat = Convert.ToString(row[10]),
                            ParamName = Convert.ToString(row[11]),
                            Code = Convert.ToInt32(row[12]),
                            GroupName = Convert.ToString(row[13])
                        });
                }

                var view = from t in list
                           select new
                           {
                               t.ID,
                               t.RefParametrs,
                               t.RefForecastType,
                               t.R1,
                               t.R2,
                               t.Est,
                               t.Y1,
                               t.Y2,
                               t.Y3,
                               ////t.YearOf,
                               t.Units,
                               t.Signat,
                               t.GroupName,
                               t.ParamName,
                               t.Code
                           };

                return new AjaxStoreResult(view, view.Count());
            }

            return new AjaxResult();
        }

        public ActionResult Save(string savedData, int estYear, int varId)
        {
            AjaxStoreResult ar = new AjaxStoreResult(StoreResponseFormat.Save);
            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", savedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";
            
            form2pValueRepository.DbContext.BeginTransaction();

            try
            {
                DataSources dataSources = (from f in datasourceRepository.FindAll()
                                           where (f.DataCode == 2) && (f.SupplierCode == "ЭО") && (f.DataName == "Прогноз") && (f.Year == estYear.ToString())
                                           select f).First();

                var refForecastType = (from f in forecastType.FindAll()
                                       where f.ID == -1
                                       select f).First();

                var refVarF2p = (from f in form2pVarRepository.GetAllVariants()
                                 where f.ID == varId
                                 select f).First();

                Dictionary<int, D_Forecast_Forma2p> paramList = (from f in forma2pRepository.GetAllParams()
                                    where (f.SourceID == dataSources.ID)
                                    select f).ToDictionary(x => x.ID);

                foreach (var updated in data.Updated)
                {
                    int paramid = Convert.ToInt32(updated["RefParametrs"]);

                    decimal? yearData = null;

                    Dictionary<string, T_Forecast_ParamValues> list = (from f in form2pValueRepository.FindAll()
                               where (f.RefParametrs.ID == paramid) &&
                                     (f.RefVarF2P.ID == varId)
                               select f).ToDictionary(x => String.Format("{0}_{1}", x.YearOf, x.ParamType));

                    /*var refParam = (from f in forma2pRepository.GetAllParams()
                                    where (f.ID == paramid) && (f.SourceID == dataSources.ID)
                                    select f).First();*/
                    
                    for (int i = 0; i < 6; i++)
                    {
                        int curYear = estYear - 2 + i;

                        int paramType = 0;
                        
                        switch (i)
                        {
                            case 0:
                                if (updated["R1"] != null)
                                {
                                    paramType = 1;
                                    string val = updated["R1"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                            case 1:
                                if (updated["R2"] != null)
                                {
                                    paramType = 1;
                                    string val = updated["R2"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                            case 2:
                                if (updated["Est"] != null)
                                {
                                    paramType = 2;
                                    string val = updated["Est"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                            case 3:
                                if (updated["Y1"] != null)
                                {
                                    paramType = 3;
                                    string val = updated["Y1"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                            case 4:
                                if (updated["Y2"] != null)
                                {
                                    paramType = 3;
                                    string val = updated["Y2"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                            case 5:
                                if (updated["Y3"] != null)
                                {
                                    paramType = 3;
                                    string val = updated["Y3"].Replace(".", ",");
                                    yearData = Decimal.Parse(val, ci);
                                }

                                break;
                        }
                        
                        if (paramType != 0)
                        {
                            T_Forecast_ParamValues paramValues;

                            /*var list2 = from f in list
                                        where (f.ParamType == paramType) && (f.YearOf == curYear)
                                        select f;*/

                            string key = String.Format("{0}_{1}", curYear, paramType);
                            if (!list.ContainsKey(key))
                            {
                                paramValues = new T_Forecast_ParamValues
                                              {
                                                  Value = yearData,
                                                  ParamType = paramType,
                                                  YearOf = curYear,
                                                  RefParametrs = paramList[paramid],
                                                  RefForecastType = refForecastType,
                                                  RefVarF2P = refVarF2p
                                              };
                            }
                            else
                            {
                                paramValues = list[key];
                                paramValues.Value = yearData;
                            }

                            form2pValueRepository.Save(paramValues);
                        }
                    }
                }

                form2pValueRepository.DbContext.CommitTransaction();
                ar.SaveResponse.Success = true;
            }
            catch (Exception e)
            {
                form2pValueRepository.DbContext.RollbackTransaction();
                ar.SaveResponse.Success = false;
                throw new Exception(e.Message, e);
            }
            
            return ar;
        }

        public ActionResult LoadComboScenVar(int estYear)
        {
            IForecastScenarioVarsRepository repo = Core.Resolver.Get<IForecastScenarioVarsRepository>();

            var list = (from f in repo.GetAllVars()
                       where (f.ReadyToCalc == 2) && (f.RefYear.ID == estYear)
                       select new ListItem
                       {
                           Value = f.ID.ToString(),
                           Text = f.Name
                       }).ToList();

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult LoadComboPlanVar(int id, int estYear, string signat)
        {
            //// estYear - 1 < (f.RefVarDate.ID / 10000) проверяем чтобы в статистику не влез прогноз
            ////////  &&
            var list = from f in variantRepository.GetAllVariants()
                       where (f.RefParam.Signat == signat) && (estYear - 1 < (f.RefDate.ID / 10000))
                       select new
                       {
                           Value = f.ID,
                           Text = f.Name,
                           Group = f.RefParam.Name,
                           Year = DataService.GetDate(f.RefDate.ID, f.Period),
                           f.XMLString
                       };
            
            List<VariantsValue> listOut = new List<VariantsValue>();
            
            foreach (var item in list)
            {
                XDocument xDoc = XDocument.Parse(item.XMLString);
                
                var usedYears = xDoc.Root.Element("UsedYears");

                var dataFrom = Convert.ToInt32(usedYears.Attribute("from").Value);
                ////var dataTo = Convert.ToInt32(usedYears.Attribute("to").Value);

                if (estYear > dataFrom)
                {
                    listOut.Add(new VariantsValue
                    {
                        Value = item.Value,
                        Text = item.Text,
                        Group = item.Group,
                        Year = item.Year
                    });
                }
            }

            return new AjaxStoreResult(listOut, listOut.Count());
        }

        public ActionResult LoadComboForm2pVar2(int estYear)
        {
            var list = from f in form2pVarRepository.GetAllVariants()
                       where (f.RefYear.ID == estYear) 
                       select new ListItem
                       {
                           Value = f.ID.ToString(),
                           Text = f.Name
                       };
            
            var ar = new AjaxStoreResult(list, list.Count());

            return ar;
        }

        public ActionResult InsertFromPlan(int varid, int estYear)
        {
            AjaxResult ar = new AjaxResult();

            int paramid = (from f in variantRepository.GetAllVariants()
                           where f.ID == varid
                          select f.RefParam.ID).First();

            var list = from f in valuesRepository.GetAllValues()
                       where (f.RefVar.ID == varid) && (f.RefParam.ID == paramid)
                       select new
                       {
                           Year = f.RefDate.ID / 10000,
                           f.Value,
                           Stat = f.RefStat.ID == 1
                       };

            string valR1 = "txtfR1.clear();";
            string valR2 = "txtfR2.clear();";
            string valEst = "txtfEst.clear();";
            string valY1 = "txtfY1.clear();";
            string valY2 = "txtfY2.clear();";
            string valY3 = "txtfY3.clear();";

            foreach (var item in list)
            {
                switch (item.Year - estYear)
                {
                    case -2:
                        if (item.Stat)
                        {
                            valR1 = "txtfR1.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        }

                        break;
                    case -1:
                        if (item.Stat)
                        {
                            valR2 = "txtfR2.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        }

                        break;
                    case 0:
                        valEst = "txtfEst.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        break;
                    case +1:
                        if (!item.Stat)
                        {
                            valY1 = "txtfY1.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        }

                        break;
                    case +2:
                        if (!item.Stat)
                        {
                            valY2 = "txtfY2.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        }

                        break;
                    case +3:
                        if (!item.Stat)
                        {
                            valY3 = "txtfY3.setValue({0});".FormatWith(item.Value.ToString().Replace(",", "."));
                        }

                        break;
                }
            }

            ar.Script = String.Concat(valR1, valR2, valEst, valY1, valY2, valY3);
            
            return ar;
        }

        public ActionResult InsertFromVar(int paramid, string r1, string r2, string est, string y1, string y2, string y3)
        {
            AjaxResult ar = new AjaxResult();

            StringBuilder script = new StringBuilder();
            
            script.AppendFormat("var v = dsForm2pValues.getById({0});", paramid); // String.Empty;

            if (r1 != String.Empty)
            {
                script.AppendFormat("v.set('R1',{0});", r1.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('R1',{1});".FormatWith(paramid, r1.Replace(",", "."));
            }

            if (r2 != String.Empty)
            {
                script.AppendFormat("v.set('R2',{0});", r2.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('R2',{1});".FormatWith(paramid, r2.Replace(",", "."));
            }

            if (est != String.Empty)
            {
                script.AppendFormat("v.set('Est',{0});", est.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('Est',{1});".FormatWith(paramid, est.Replace(",", "."));
            }

            if (y1 != String.Empty)
            {
                script.AppendFormat("v.set('Y1',{0});", y1.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('Y1',{1});".FormatWith(paramid, y1.Replace(",", "."));
            }

            if (y2 != String.Empty)
            {
                script.AppendFormat("v.set('Y2',{0});", y2.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('Y2',{1});".FormatWith(paramid, y2.Replace(",", "."));
            }

            if (y3 != String.Empty)
            {
                script.AppendFormat("v.set('Y3',{0});", y3.Replace(",", "."));
                ////script += "dsForm2pValues.getById({0}).set('Y3',{1});".FormatWith(paramid, y3.Replace(",", "."));
            }

            script.Append("insertWindow.hide();");
            ar.Script = script.ToString(); ////script + "insertWindow.hide();";

            return ar;
        }

        public ActionResult LoadChart(string key, int paramId, int varId2)
        {
            UserFormsControls ufc = this.extension.Forms[key];
            var objVar = ufc.GetObject("varId");
            var objYear = ufc.GetObject("year");

            List<ViewForm2pValuesChartStruct> list = new List<ViewForm2pValuesChartStruct>();
            
            if ((objVar != null) && (objYear != null))
            {
                string selectSQL = String.Empty;

                int varId = Convert.ToInt32(objVar);

                int year = Convert.ToInt32(objYear);
                
                List<string> attributesList = new List<string>
                {
                    "v.ID",
                    "RefParametrs",
                    "RefForecastType",
                    "Est",
                    "Y1",
                    "Y2",
                    "Y3",
                    "R1",
                    "R2"
                    ////"YearOf"
                };

                string filtr;

                if (varId2 != -1)
                {
                    filtr = String.Format("(v.refparametrs = {0})  and ((v.refvarf2p = {1}) or (v.refvarf2p = {2}))", paramId, varId, varId2);
                }
                else
                {
                    filtr = String.Format("(v.refparametrs = {0})  and (v.refvarf2p = {1})", paramId, varId);
                }

                string includedQuery1 = "(select o.designation from d_units_okei o where v.refunits=o.id) as units";
                string includedQuery2 = "v.signat, f.name as paramname";

                selectSQL = String.Format("Select {0}, {1}, {2} from {3} left join d_forecast_forma2p f on f.id = v.refparametrs where {4}", String.Join(", ", attributesList.ToArray()), includedQuery1, includedQuery2, "v_forecast_val_form2p v", filtr);

                var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

                ViewForm2pValuesChartStruct[] arr = new ViewForm2pValuesChartStruct[6];

                int j = 0;
                foreach (object[] row in queryResult)
                {
                    var est = row[3] != null ? Convert.ToDouble(row[3]) : (double?)null;
                    var y1 = row[4] != null ? Convert.ToDouble(row[4]) : (double?)null;
                    var y2 = row[5] != null ? Convert.ToDouble(row[5]) : (double?)null;
                    var y3 = row[6] != null ? Convert.ToDouble(row[6]) : (double?)null;
                    var r1 = row[7] != null ? Convert.ToDouble(row[7]) : (double?)null;
                    var r2 = row[8] != null ? Convert.ToDouble(row[8]) : (double?)null;

                    if (j == 0)
                    {
                        arr[0].Xv1 = r1;
                        arr[0].Year = year - 2;
                        arr[1].Xv1 = r2;
                        arr[1].Year = year - 1;
                        arr[2].Xv1 = est;
                        arr[2].Year = year;
                        arr[3].Xv1 = y1;
                        arr[3].Year = year + 1;
                        arr[4].Xv1 = y2;
                        arr[4].Year = year + 2;
                        arr[5].Xv1 = y3;
                        arr[5].Year = year + 3;
                    }

                    if (j == 1)
                    {
                        arr[0].Xv2 = r1;
                        arr[1].Xv2 = r2;
                        arr[2].Xv2 = est;
                        arr[3].Xv2 = y1;
                        arr[4].Xv2 = y2;
                        arr[5].Xv2 = y3;
                    }

                    j++;
                }

                list = arr.ToList();
            }

            double? min = null;
            double? max = null;
            double delta;
            
            foreach (var item in list)
            {
                if (!min.HasValue)
                {
                    min = item.Xv1;
                }

                if (!max.HasValue)
                {
                    max = item.Xv1;
                }

                if (min > item.Xv1)
                {
                    min = item.Xv1;
                }

                if (min > item.Xv2)
                {
                    min = item.Xv2;
                }

                if (max < item.Xv1)
                {
                    max = item.Xv1;
                }

                if (max < item.Xv2)
                {
                    max = item.Xv2;
                }
            }

            var script = String.Empty;

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

            ////store.Listeners.DataChanged.AddAfter(script); 

            var ar = new AjaxStoreExtraResult(list, list.Count, script);

            return ar;
        }

        public ActionResult CalcInGridParam(int baseYear, int varId, string sig, string column, string changedData)
        {
            AjaxResult ar = new AjaxResult();

            var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            ci.NumberFormat.NumberDecimalSeparator = ",";

            StoreDataHandler dataHandler = new StoreDataHandler(String.Format("{{{0}}}", changedData));
            ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();
            
            DataTable tempTable = new DataTable();
            DataColumn col = tempTable.Columns.Add("sig", typeof(string));
            tempTable.Columns.Add("{0}".FormatWith(baseYear - 2), typeof(double));
            tempTable.Columns.Add("{0}".FormatWith(baseYear - 1), typeof(double));
            tempTable.Columns.Add("{0}".FormatWith(baseYear), typeof(double));
            tempTable.Columns.Add("{0}".FormatWith(baseYear + 1), typeof(double));
            tempTable.Columns.Add("{0}".FormatWith(baseYear + 2), typeof(double));
            tempTable.Columns.Add("{0}".FormatWith(baseYear + 3), typeof(double));

            tempTable.PrimaryKey = new DataColumn[] { col };

            foreach (Dictionary<string, string> dicrow in data.Updated)
            {
                DataRow row = tempTable.NewRow();

                var s = dicrow["R1"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[(baseYear - 2).ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                s = dicrow["R2"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[(baseYear - 1).ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                s = dicrow["Est"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[baseYear.ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                s = dicrow["Y1"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[(baseYear + 1).ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                s = dicrow["Y2"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[(baseYear + 2).ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                s = dicrow["Y3"];
                if (!String.IsNullOrEmpty(s))
                {
                    row[(baseYear + 3).ToString()] = Decimal.Parse(s.Replace(".", ","), ci);
                }

                row["sig"] = dicrow["Signat"];

                tempTable.Rows.Add(row);
            }

            int year = 0;

            switch (column)
            {
                case "R1":
                    year = baseYear - 2;
                    break;
                case "R2":
                    year = baseYear - 1;
                    break;
                case "Est":
                    year = baseYear;
                    break;
                case "Y1":
                    year = baseYear + 1;
                    break;
                case "Y2":
                    year = baseYear + 2;
                    break;
                case "Y3":
                    year = baseYear + 3;
                    break;
            }
            
            Form2pFieldCalcService calcService = new Form2pFieldCalcService(Core.Resolver.Get<IForecastForma2pVarRepository>(), Core.Resolver.Get<IForecastForma2pValueRepository>());
            calcService.LoadXml(Form2pFieldCalcService.Doc);
            var listDep = calcService.GetDependent(sig);
            
            StringBuilder script = new StringBuilder();

            foreach (string depSig in listDep)
            {
                ////var list = calcService.GetNeeded(depSig);
                script.AppendFormat("var v = dsForm2pValues.getAt(dsForm2pValues.find(\"Signat\",\"{0}\"));", depSig);
                
                if (true)
                {
                    var calcedData = calcService.CalcParam(depSig, sig, year, varId, tempTable);

                    foreach (var yearData in calcedData)
                    {
                        string colName = "Est";
                        if (yearData.Key == baseYear + 1)
                        {
                            colName = "Y1";
                        }

                        if (yearData.Key == baseYear + 2)
                        {
                            colName = "Y2";
                        }

                        if (yearData.Key == baseYear + 3)
                        {
                            colName = "Y3";
                        }

                        if (yearData.Key == baseYear - 2)
                        {
                            colName = "R1";
                        }

                        if (yearData.Key == baseYear - 1)
                        {
                            colName = "R2";
                        }

                        script.AppendFormat("v.set('{0}',{1});", colName, yearData.Value.ToString().Replace(",", "."));
                    }
                }
            }

            ar.Script = script.ToString();

            return ar;
        }

        public ActionResult InsertRegs(int varid, int baseYear)
        {
            AjaxResult ar = new AjaxResult();

            StringBuilder script = new StringBuilder();

            /*var list = (from f in regsRepository.GetAllRegulators()
                       where !String.IsNullOrEmpty(f.Signat)
                       select new
                       {
                           f.ID
                       }).ToList();*/

            var list = (from f in regsValueRepository.GetAllValues()
                       where !String.IsNullOrEmpty(f.RefRegs.Signat)
                       select new 
                       {
                           f.Value,
                           f.RefDate,
                           f.RefVSC,
                           Sig = f.RefRegs.Signat
                       }).ToList();

            foreach (var regValue in list)
            {
                var curYear = regValue.RefDate.ID / 10000;
                
                string colName = String.Empty;

                if (curYear == baseYear + 1)
                {
                    colName = "Y1";
                }

                if (curYear == baseYear + 2)
                {
                    colName = "Y2";
                }

                if (curYear == baseYear + 3)
                {
                    colName = "Y3";
                }

                if (curYear == baseYear)
                {
                    colName = "Est";
                }

                if (curYear == baseYear - 2)
                {
                    colName = "R1";
                }

                if (curYear == baseYear - 1)
                {
                    colName = "R2";
                }

                if (colName != String.Empty)
                {
                    script.AppendFormat("var v = dsForm2pValues.getAt(dsForm2pValues.find(\"Signat\",\"{0}\"));", regValue.Sig);
                    script.AppendFormat("v.set('{0}',{1});", colName, regValue.Value.ToString().Replace(",", "."));
                }
            }

            ar.Script = script.ToString();

            return ar;
        }

        public ActionResult FillFromScen(int scenId, int id2p)
        {
            AjaxResult ar = new AjaxResult();

            try
            {
                using (new ServerContext())
                {
                    Scheme.Form2pService.FillFromScen(scenId, id2p);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            ar.Result = "success";

            return ar;
        }
    }
}
