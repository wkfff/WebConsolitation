using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    internal struct Form2pParam
    {
        ////string id;
        public string Expression;
        public Dictionary<string, UsedData> UsedParams;
    }

    internal struct UsedData
    {
        public string Id;
        public int TimeShift;
    }
    
    public class Form2pFieldCalcService
    {
        public const string Doc =
@"<Form2pCalc>
	<Param id=""06DMGx02"">
		<UsedData id=""06DMGx01_I1"" name=""A"" time=""-1"" />
		<UsedData id=""06DMGx01_I1"" name=""B"" time=""0"" />
		<Expression>B/A*100</Expression>
	</Param>
	<Param id=""06DMGx0201"">
		<UsedData id=""06DMGx0101_I1"" name=""A"" time=""-1"" />
		<UsedData id=""06DMGx0101_I1"" name=""B"" time=""0"" />
		<Expression>B/A*100</Expression>
	</Param>
	<Param id=""06DMGx0202"">
		<UsedData id=""06DMGx0102"" name=""A"" time=""-1"" />
		<UsedData id=""06DMGx0102"" name=""B"" time=""0"" />
		<Expression>B/A*100</Expression>
	</Param>
	<Param id=""06PTUc030302"">
		<UsedData id=""06PTUc030301_I1"" name=""A"" time=""-1"" />
		<UsedData id=""06PTUc030301_I1"" name=""B"" time=""0"" />
		<UsedData id=""06PTUc030303"" name=""C"" time=""0"" />
		<Expression>((B/A)/C*100)*100</Expression>
	</Param>	
</Form2pCalc>";

        private readonly IForecastForma2pVarRepository varRepository;
        private readonly IForecastForma2pValueRepository valueRepository;

        private Dictionary<string, Form2pParam> form2pParam = new Dictionary<string, Form2pParam>();

        public Form2pFieldCalcService(
                                      IForecastForma2pVarRepository varRepository,
                                      IForecastForma2pValueRepository valueRepository)
        {
            this.varRepository = varRepository;
            this.valueRepository = valueRepository;
        }

        public void LoadXml(string doc)
        {
            XDocument xDoc = XDocument.Parse(doc);
            XElement xRoot = xDoc.Root;

            foreach (XElement xParam in xRoot.Elements("Param"))
            {
                string id = xParam.Attribute("id").Value;

                string expression = xParam.Element("Expression").Value;

                Form2pParam param = new Form2pParam
                {
                    UsedParams = new Dictionary<string, UsedData>(),
                    Expression = expression
                };

                foreach (XElement xUsedData in xParam.Elements("UsedData"))
                {
                    var name = xUsedData.Attribute("name").Value;
                    
                    var time = Convert.ToInt32(xUsedData.Attribute("time").Value);

                    UsedData usedData = new UsedData
                    {
                        Id = xUsedData.Attribute("id").Value,
                        TimeShift = time
                    };

                    param.UsedParams.Add(name, usedData);
                }

                form2pParam.Add(id, param);
            }
        }

        public Dictionary<int, double> CalcParam(string calcSig, string inSig, int year, int varid, DataTable changedData)
        {
            if (form2pParam.ContainsKey(calcSig))
            {
                Form2pParam param = form2pParam[calcSig];

                Dictionary<int, double> calcedValue = new Dictionary<int, double>(); 
                
                var refVar = (from f in varRepository.GetAllVariants()
                             where f.ID == varid
                             select f).First();

                /*var list = (from f in valueRepository.GetAllValues()
                           where (f.YearOf == year) && (f.RefVarF2P == refVar)
                           select new
                           {
                               f.Value,
                               paramId = f.RefParametrs.ID
                           }).ToDictionary(x => x.paramId);*/
                
                var i = 0;
                List<int> listFormulas = new List<int>();

                foreach (var usedParam in param.UsedParams)
                {
                    if (usedParam.Value.Id == inSig)
                    {
                        listFormulas.Add(usedParam.Value.TimeShift * -1);
                        i++;
                    }
                }

                foreach (int formulaTimeShift in listFormulas)
                {
                    Dictionary<string, double> paramData = new Dictionary<string, double>();
                    
                    bool allFound = true;

                    foreach (var usedParam in param.UsedParams)
                    {
                        string key = usedParam.Key;
                        UsedData usedData = usedParam.Value;

                        var dataRow = changedData.Rows.Find(usedData.Id);

                        object value = DBNull.Value;

                        if (dataRow != null)
                        {
                            string colname = (year + usedData.TimeShift + formulaTimeShift).ToString();
                            if (dataRow.Table.Columns.Contains(colname))
                            {
                                value = dataRow[colname];
                                paramData.Add(key, Convert.ToDouble(value));
                            }
                        }

                        if (value == DBNull.Value)
                        {
                            var list = (from f in valueRepository.GetAllValues()
                                        where (f.YearOf == year + usedData.TimeShift + formulaTimeShift) &&
                                              (f.RefVarF2P == refVar) &&
                                              (f.RefParametrs.Signat == usedData.Id)
                                        select new
                                        {
                                            f.Value,
                                            paramId = f.RefParametrs.ID
                                        }).ToList();

                            if (list.Count == 0)
                            {
                                allFound = false;
                                break;
                            }

                            paramData.Add(key, Convert.ToDouble(list.First().Value));
                        }
                    }

                    if ((paramData.Count > 0) && allFound)
                    {
                        calcedValue.Add(year + formulaTimeShift, Equation.MathCalc(paramData, param.Expression));
                    }
                }

                if (calcedValue.Count() > 0)
                {
                    return calcedValue;
                }
            }

            return null;
        }

        public List<string> GetNeeded(string signature)
        {
            var usedParams = form2pParam[signature].UsedParams;

            List<string> list = new List<string>();

            foreach (KeyValuePair<string, UsedData> keyValuePair in usedParams)
            {
                list.Add(keyValuePair.Value.Id);
            }

            return list;
        }

        public List<string> GetDependent(string signature)
        {
            List<string> list = new List<string>();

            foreach (var param in form2pParam)
            {
                var usedParam = param.Value.UsedParams;

                foreach (var usedData in usedParam)
                {
                    if (usedData.Value.Id == signature)
                    {
                        list.Add(param.Key);
                        break;
                    }
                }
            }

            return list;
        }
    }
}
