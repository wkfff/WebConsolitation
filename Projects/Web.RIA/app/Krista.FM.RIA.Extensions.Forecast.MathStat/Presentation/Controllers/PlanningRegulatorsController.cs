using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningRegulatorsController : SchemeBoundController
    {
        private IForecastRegulatorsRepository regulatorsRepository;
        private IForecastRegulatorsValueRepository regulatorsValueRepository;
        private ILinqRepository<D_Forecast_VarScenCond> varScenCondRepository;

        public PlanningRegulatorsController(IForecastRegulatorsRepository regulatorsRepository, IForecastRegulatorsValueRepository regulatorsValueRepository, ILinqRepository<D_Forecast_VarScenCond> varScenCondRepository)
        {
            this.regulatorsRepository = regulatorsRepository;
            this.regulatorsValueRepository = regulatorsValueRepository;
            this.varScenCondRepository = varScenCondRepository;
        }
        
        public ActionResult Load()
        {
            var view = from f in regulatorsRepository.GetAllRegulators()
                       select new
                       {
                           f.ID,
                           f.Name,
                           f.Descr,
                           Units = f.RefUnits.Symbol
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult LoadListRegs()
        {
            var view = from f in regulatorsRepository.GetAllRegulators()
                       select new
                       {
                           f.ID,
                           f.Name
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        public ActionResult ValuesLoad(int regId)
        {
            /*var listScenCond = regulatorsValueRepository.GetAllValues().Select(f => new
                                                                                {
                                                                                    ID = f.RefVarScenCond.ID,
                                                                                    Code = f.RefVarScenCond.Code,
                                                                                    Symbol = f.RefVarScenCond.Symbol
                                                                                }).GroupBy(p => p.ID).ToList();

            for (int i = 0; i < listScenCond.Count; i++)
            {
                var item = listScenCond[i];
            }*/

          /*  string selectSQL = @"select r.refvarscencond, v.code, v.symbol
from dv.d_forecast_regulatorsvalue r
left join dv.d_forecast_varscencond v on v.id = r.refvarscencond
where refregulators = {0}
group by refvarscencond, v.code, v.symbol 
order by refvarscencond, v.code, v.symbol".FormatWith(regId);

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();*/

            var list = from f in regulatorsValueRepository.GetAllValues()
                       where f.RefRegs.ID == regId
                       orderby f.RefVSC.ID 
                       select new
                       {
                           f.ID,
                           ScenCondId = f.RefVSC.ID,
                           ScenCondCode = f.RefVSC.Code,
                           ScenCondSymbol = f.RefVSC.Symbol,
                           Year = f.RefDate.ID / 10000,
                           f.Value
                       };

            DataTable dt = new DataTable();
            var col = dt.Columns.Add("id", typeof(int));
            dt.PrimaryKey = new[] { col };

            dt.Columns.Add("scenCondName", typeof(string));

            foreach (var item in list)
            {
                string yearName = "year_{0}".FormatWith(item.Year);
                if (!dt.Columns.Contains(yearName))
                {
                    dt.Columns.Add(yearName, typeof(double));
                }

                var row = dt.Rows.Find(item.ScenCondId);

                if (row == null)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["id"] = item.ScenCondId;
                    newRow["scenCondName"] = item.ScenCondSymbol;
                    dt.Rows.Add(newRow);

                    row = newRow;
                }

                row[yearName] = item.Value;
            }
            
            return new AjaxStoreResult(dt, dt.Rows.Count);
        }
    }
}
