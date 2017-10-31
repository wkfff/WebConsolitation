using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PumpService
    {
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastParamsRepository paramsRepository;

        public PumpService(
                           IForecastVariantsRepository variantsRepository, 
                           IForecastParamsRepository paramsRepository)
        {
            this.variantsRepository = variantsRepository;
        }

        public void PumpData(int varId)
        {
            var variant = variantsRepository.FindOne(varId);

            XDocument xDoc = XDocument.Parse(variant.XMLString);
            
            var usedDatas = xDoc.Root.Element("UsedDatas");
            var usedData = usedDatas.Element("UsedData");

            var usedYears = xDoc.Root.Element("UsedYears");

            int dataFrom = Convert.ToInt32(usedYears.Attribute("From").Value);
            int dataTo = Convert.ToInt32(usedYears.Attribute("To").Value);

            foreach (XElement data in usedData.Elements("Data"))
            {
                int paramId = Convert.ToInt32(data.Attribute("fparam").Value);

                var param = paramsRepository.FindOne(paramId);

                XDocument xParamDoc = XDocument.Parse(param.XMLString);

                var root = xParamDoc.Root;

                string paramName = root.Attribute("name").Value;
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
                string[] filters = new string[elements.Count()];
                
                int i = 0;
                foreach (var element in elements)
                {
                    string fname = element.Attribute("name").Value;
                    string fvalue = element.Value;
                    /////flist.Add(fname, fvalue);
                    filters[i] = String.Format("({0} = {1})", fname, fvalue);
                }

                string filter = String.Join(" and ", filters); 

                ////Core.Resolver.

                /*var repo = Core.Resolver.Get<NHibernateLinqRepository<D_Forecast_PlanningParams>>();

                repo.FindAll().Where(*/

                double k = unitsFrom / unitsTo;
                
                for (int year = dataFrom; year <= dataTo; year++)
                {
                    int yearRow;

                    switch (yearFormat)
                    {
                        case "unv":
                            yearRow = (year * 10000) + 0001;
                            break;
                        case "year":
                        default:
                            yearRow = year;
                            break;
                    }

                    string selectSQL = String.Format("select {0}*{1} from {2} where ({3}={4}) {5}", fieldValue, k, fromTable, yearRow, year, filter);
                    var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();
                }
            }
        }
    }
}
