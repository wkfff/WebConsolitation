using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public struct ForecastParameter
    {
        public string Name;
        public int ParamId;
    }

    public struct ForecastRegulator
    {
        public string Name;
        public int RegId;
        public int FVar;
    }

    public struct ForecastStruct
    {
        public ForecastParameter ForecastingParam;
        public List<ForecastParameter> UsedParams;
        public List<ForecastRegulator> UsedRegs;
        public int Method;
        public int Group;
        public string Expression;
    }

    public struct ChartData
    {
        public int Year { get; set; }

        public double? Xp { get; set; }

        public double? Xs { get; set; }
    }

    public class DataService : IDataService
    {
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastRegulatorsRepository regsRepository;
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;
        private readonly IForecastValuesRepository valuesRepository;
        private readonly IForecastRegulatorsValueRepository regsValuesRepository;
        private readonly IRepository<D_Forecast_VarScenCond> varScenCond;

        private DataTable datStatic;
        private DataTable datProg;
        private DataTable datRegulator;
        ////private List<Criteria> lstCrit;
        private SortedList<string, double> lstCoeff;
        private SortedList<int, bool> lstYears;
        private List<ForecastStruct> foreList;

        public DataService(
            IForecastParamsRepository paramsRepository, 
            IForecastRegulatorsRepository regsRepository,
            IForecastVariantsRepository variantsRepository,
            IForecastValuesRepository valuesRepository,
            IRepository<FX_Date_YearDayUNV> yearRepository,
            IForecastRegulatorsValueRepository regsValuesRepository,
            IRepository<D_Forecast_VarScenCond> varScenCond)
        {
            this.paramsRepository = paramsRepository;
            this.regsRepository = regsRepository;
            this.variantsRepository = variantsRepository;
            this.yearRepository = yearRepository;
            this.valuesRepository = valuesRepository;
            this.regsValuesRepository = regsValuesRepository;
            this.varScenCond = varScenCond;
        }

        public static string GetDate(int date, int? period)
        {
            if (date % 10000 == 0)
            {
                if (period.HasValue)
                {
                    return String.Format("{0} - {1} г.", date / 10000, (date / 10000) + period);
                }
            }

            return String.Empty;
        }

        public void Initialize()
        {
            datStatic = new DataTable();

            datStatic.Columns.Add("id", typeof(int));
            datStatic.Columns.Add("Param", typeof(string));

            datProg = new DataTable();

            datProg.Columns.Add("id", typeof(int));
            datProg.Columns.Add("Param", typeof(string));

            datRegulator = new DataTable();

            datRegulator.Columns.Add("id", typeof(int));
            datRegulator.Columns.Add("Param", typeof(string));
            datRegulator.Columns.Add("fvarcode", typeof(string));

            ////lstCrit = new List<Criteria>();

            lstCoeff = new SortedList<string, double>();

            lstYears = new SortedList<int, bool>();
            
            foreList = new List<ForecastStruct>();
        }

        public void FillData(int variantId)
        {
            var variant = variantsRepository.FindOne(variantId);
            
            XDocument xDoc = XDocument.Parse(variant.XMLString);

            int progFrom = Convert.ToInt32(xDoc.Root.Attribute("from").Value);
            int progTo = Convert.ToInt32(xDoc.Root.Attribute("to").Value);

            var usedDatas = xDoc.Root.Element("UsedDatas");

            int paramId = Convert.ToInt32(usedDatas.Attribute("fparamid").Value);

            ////var usedData = usedDatas.Element("UsedData");
            
            /*method = Convert.ToInt32(usedData.Attribute("method").Value);
            group = Convert.ToInt32(usedData.Attribute("group").Value);*/

            var usedYears = xDoc.Root.Element("UsedYears");

            int dataFrom = Convert.ToInt32(usedYears.Attribute("from").Value);
            int dataTo = Convert.ToInt32(usedYears.Attribute("to").Value);

            var methodParams = xDoc.Root.Element("MethodParams");
            var methodStat = xDoc.Root.Element("MethodStat");

            lstYears.Clear(); 

            for (int i = dataFrom; i <= dataTo; i++)
            {
                lstYears.Add(i, false);
            }
            
            foreach (XElement element in usedYears.Elements("Year"))
            {
                int year = Convert.ToInt32(element.Value);
                lstYears[year] = true;
            }

            for (int i = dataFrom; i <= dataTo; i++)
            {
                string colName = String.Format("year_{0}", i);
                datStatic.Columns.Add(colName, typeof(double));
                datProg.Columns.Add(colName, typeof(double));
                datRegulator.Columns.Add(colName, typeof(double));
            }
            
            for (int i = progFrom; i <= progTo; i++)
            {
                string colName = String.Format("year_{0}", i);
                if (!datProg.Columns.Contains(colName))
                {
                    datProg.Columns.Add(colName, typeof(double));
                    datRegulator.Columns.Add(colName, typeof(double));
                }
            }

            DataRow dr = datProg.NewRow();
            dr["id"] = paramId;

            var param = paramsRepository.FindOne(paramId);
            dr["Param"] = String.Format("{0}, {1}", param.Name, param.RefOKEI.Designation);
            datProg.Rows.Add(dr);
            
            foreList.Clear();

            ExtractUsedData(usedDatas);

            if (methodParams != null)
            {
                foreach (XElement methodParam in methodParams.Elements("Param"))
                {
                    if (methodParam != null)
                    {
                        string name = methodParam.Attribute("name").Value;
                        double value = Convert.ToDouble(methodParam.Value);
                        lstCoeff.Add(name, value);
                    }
                }
            }

            if (methodStat != null)
            {
                foreach (XElement crit in methodStat.Elements("Crit"))
                {
                    if (crit != null)
                    {
                        string name = crit.Attribute("name").Value;
                        string text = crit.Attribute("text").Value; 
                        double value = Convert.ToDouble(crit.Value);

                        ////lstCrit.Add(new Criteria { Name = name, Text = text, Value = value });
                    }
                }
            }
        }

        public void FillDataForNewComplexMethod(string xmlString)
        {
            XDocument xDoc = XDocument.Parse(xmlString);

            var usedDatas = xDoc.Root; ////.Element("UsedDatas");

            int paramId = Convert.ToInt32(usedDatas.Attribute("fparamid").Value);
            
            var param = paramsRepository.FindOne(paramId);

            DataRow dr = datProg.NewRow();
            dr["id"] = paramId;
            dr["Param"] = String.Format("{0}, {1}", param.Name, param.RefOKEI.Designation);
            datProg.Rows.Add(dr);
            
            ExtractUsedData(usedDatas);
        }
        
        public void LoadData(int variantId)
        {
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
                        var varRef = variantsRepository.FindOne(variantId);
                        var paramRef = paramsRepository.FindOne(paramid);
                        ////var existValue = valuesRepository.FindAll().Where(t => t.RefVar.ID == varid).ToList();

                        /*var list = (from t in valuesRepository.FindAll()
                                         where (t.RefVar == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t).ToList();*/
                        var existValue = from t in valuesRepository.FindAll()
                                         where (t.RefStat.ID == 0) && (t.RefVar == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t.Value;
                        var lst = existValue.ToList();
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
                        var varRef = variantsRepository.FindOne(variantId);
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

            LoadRegulators();
        }

        public void LoadRegulators()
        {
            Dictionary<int, int> dicRegs = new Dictionary<int, int>();

            foreach (ForecastStruct forecastStruct in foreList)
            {
                List<ForecastRegulator> usedRegs = forecastStruct.UsedRegs;

                foreach (ForecastRegulator forecastRegulator in usedRegs)
                {
                    if (!dicRegs.ContainsKey(forecastRegulator.RegId))
                    {
                        dicRegs.Add(forecastRegulator.RegId, forecastRegulator.FVar);
                    }
                }
            }
            
            foreach (DataRow row in datRegulator.Rows)
            {
                int regid = Convert.ToInt32(row["id"]);

                ////int fvarId = row["fvarcode"] == DBNull.Value ? 0 : Convert.ToInt32(row["fvar"]);
                int fvarId = dicRegs[regid];

                foreach (DataColumn column in datRegulator.Columns)
                {
                    if (column.ColumnName.Contains("year_"))
                    {
                        var colName = column.ColumnName;
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                        var yearRef = yearRepository.Get((year * 10000) + 1);
                        var regRef = regsRepository.FindOne(regid);
                        
                        var existValue = from t in regsValuesRepository.FindAll()
                                         where (t.RefDate == yearRef) && (t.RefRegs == regRef)
                                         select t;
                        Dictionary<int, D_Forecast_RegValues> dict = existValue.ToDictionary(x => x.RefVSC.ID);

                        if (dict.Count() > 0)
                        {
                            if (dict.ContainsKey(fvarId))
                            {
                                row[colName] = dict[fvarId].Value;
                            }
                            else
                            {
                                if (dict.ContainsKey(-1))
                                {
                                    row[colName] = dict[-1].Value;
                                }
                                else
                                {
                                    row[colName] = dict.Values.First().Value;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void NewData(int paramId)
        {
            var param = paramsRepository.FindOne(paramId);
            ////string paramName = paramsRepository.FindOne(paramId).ParamName;

            ForecastParameter parameter = new ForecastParameter
            {
                Name = param.Name,
                ParamId = paramId
            };

            ForecastStruct forecastStruct = new ForecastStruct
            {
                ForecastingParam = parameter,
                Group = -1,
                Method = -1,
                UsedParams = new List<ForecastParameter>
                    {
                        new ForecastParameter { Name = param.Name, ParamId = paramId }
                    },
                UsedRegs = new List<ForecastRegulator>()
            };
            
            foreList.Add(forecastStruct);
            
            DataRow rowStat = datStatic.NewRow();
            rowStat["id"] = paramId;
            rowStat["Param"] = param.Name;
            datStatic.Rows.Add(rowStat);

            DataRow dr = datProg.NewRow();
            dr["id"] = paramId;
            dr["Param"] = String.Format("{0}, {1}", param.Name, param.RefOKEI.Designation);
            datProg.Rows.Add(dr);
        }

        public DataTable GetProgData()
        {
            return datProg;
        }

        public DataTable GetStaticData()
        {
            return datStatic;
        }

        public DataTable GetRegulatorData()
        {
            return datRegulator;
        }

        public SortedList<string, double> GetCoeff()
        {
            return lstCoeff;
        }

        /*public List<Criteria> GetCriteria()
        {
            return lstCrit;
        }*/

        public SortedList<int, bool> GetArrYears()
        {
            return lstYears;
        }

        public List<ForecastStruct> GetForecastList()
        {
            return foreList;
        }

        private void ExtractUsedData(XElement usedDatas)
        {
            foreach (XElement usedData in usedDatas.Elements("UsedData"))
            {
                int fparamId = Convert.ToInt32(usedData.Attribute("fparamid").Value);
                string paramName = Convert.ToString(usedData.Attribute("name").Value);
                int paramMethod = Convert.ToInt32(usedData.Attribute("method").Value);
                int paramGroup = Convert.ToInt32(usedData.Attribute("group").Value);

                ForecastParameter parameter = new ForecastParameter
                {
                    Name = paramName,
                    ParamId = fparamId
                };

                ForecastStruct forecastStruct = new ForecastStruct
                {
                    ForecastingParam = parameter,
                    Group = paramGroup,
                    Method = paramMethod,
                    UsedParams = new List<ForecastParameter>(),
                    UsedRegs = new List<ForecastRegulator>()
                };

                foreach (XElement data in usedData.Elements("Data"))
                {
                    if (data != null)
                    {
                        DataRow rowStat = datStatic.NewRow();

                        int id = Convert.ToInt32(data.Attribute("paramid").Value);
                        string name = Convert.ToString(data.Attribute("name").Value);

                        ForecastParameter usedParam = new ForecastParameter
                        {
                            Name = name,
                            ParamId = id
                        };

                        forecastStruct.UsedParams.Add(usedParam);

                        rowStat["id"] = id;
                        var statParam = paramsRepository.FindOne(id);
                        rowStat["Param"] = String.Format("{0}, {1}", statParam.Name, statParam.RefOKEI.Designation);

                        datStatic.Rows.Add(rowStat);
                    }
                }

                foreach (XElement data in usedData.Elements("Regulator"))
                {
                    if (data != null)
                    {
                        DataRow rowReg = datRegulator.NewRow();

                        int id = Convert.ToInt32(data.Attribute("regid").Value);
                        string name = Convert.ToString(data.Attribute("name").Value);
                        
                        int fvar = -1;

                        if (data.Attribute("fvar") != null)
                        {
                            fvar = Convert.ToInt32(data.Attribute("fvar").Value);
                        }

                        ForecastRegulator usedReg = new ForecastRegulator()
                        {
                            Name = name,
                            RegId = id,
                            FVar = fvar
                        };

                        forecastStruct.UsedRegs.Add(usedReg);

                        string fvarcode = (from f in varScenCond.GetAll()
                                           where f.ID == fvar
                                           select f.Symbol).First();

                        rowReg["id"] = id;
                        var regParam = regsRepository.FindOne(id);
                        rowReg["Param"] = String.Format("{0}, {1}", regParam.Name, regParam.RefUnits.Designation);
                        rowReg["fvarcode"] = fvarcode;

                        datRegulator.Rows.Add(rowReg);
                    }
                }

                var exps = usedData.Elements("Expression");
                if (exps.Count() > 0)
                {
                    forecastStruct.Expression = exps.First().Value;
                }

                foreList.Add(forecastStruct);
            }
        }
    }
}