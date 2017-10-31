using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pParamsController : SchemeBoundController
    {
        private readonly IForecastForma2pRepository paramsRepository;
        
        public Form2pParamsController(IForecastForma2pRepository paramsRepository)
        {
            this.paramsRepository = paramsRepository;
        }

        public ActionResult Load(int sourceId)
        {
            var groups = (from p in paramsRepository.GetAllParams() 
                         where (p.Code % 100000000 == 0) &&
                         (p.SourceID == sourceId)
                         select new { p.Code, p.Name }).ToList();

            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            foreach (var group in groups)
            {
                dictionary.Add(group.Code / 100000000, group.Name);
            }

            dictionary.Add(0, "Нет группы");

            var view = from f in paramsRepository.GetAllParams()
                       where (f.ParentID != null) && 
                             (f.SourceID == sourceId) &&
                             (f.Code % 100000000 != 0) &&
                             (f.Code / 100000000 != 0)
                       select new
                       {
                           f.Code,
                           f.Name,
                           f.Note,
                           RefOKEI = f.RefUnits.Symbol,
                           f.Signat,
                           Groups = dictionary[f.Code / 100000000]
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult ComboStoreLoad()
        {
/*            var view = paramsRepository.GetAllParams().GroupBy(f => new
                                                                        {
                                                                            ID = f.SourceID,
                                                                            Name =
                                                                        (from d in datasourceRepository.FindAll()
                                                                         where f.SourceID == d.ID
                                                                         select new
                                                                                    {
                                                                                        d.DataSourceName
                                                                                    }).First()
                                                                        }).OrderBy(v => v.Key.Name);*/
            string selectSQL = String.Format("select f.sourceid as ID, d.datasourcename as Name from d_forecast_forma2p f left join datasources d on d.id = f.sourceid group by f.sourceid, d.datasourcename order by d.datasourcename");
            
            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<ListItem> list = new List<ListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new ListItem { Text = row[1].ToString(), Value = row[0].ToString() });
            }
            
            return new AjaxStoreResult(list, list.Count);
        }
    }
}
