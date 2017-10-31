using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public struct ValuationListItem
    {
        public int ID;
        ////public int BaseScenario;
        public string ParamName;
        public decimal? ValueEst;
        public decimal? ValueEstB;
        public decimal? ValueY1;
        public decimal? ValueY1B;
        public decimal? ValueY2;
        public decimal? ValueY2B;
        public decimal? ValueY3;
        public decimal? ValueY3B;
        public decimal? ValueY4;
        public decimal? ValueY4B;
        public decimal? ValueY5;
        public decimal? ValueY5B;
        public decimal MaxBound;
        public decimal MinBound;
        /*public decimal IndexDef;
        public string UserName;
        public bool Finished;
        public string GroupName;*/
        public string Units;
    }
    
    /*public struct IndsValListItem
    {
        public int ID;
        public string ParamName;
        public decimal? ValueEst;
        public decimal? ValueEstB;
        public decimal? ValueY1;
        public decimal? ValueY1B;
        public decimal? ValueY2;
        public decimal? ValueY2B;
        public decimal? ValueY3;
        public decimal? ValueY3B;
        public decimal? ValueY4;
        public decimal? ValueY4B;
        public decimal? ValueY5;
        public decimal? ValueY5B;
        public decimal MaxBound;
        public decimal MinBound;
        public string Units;
    }*/

    public class ValuationParamController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private IForecastExtension extension;
        private IForecastScenarioVarsRepository varsRepository;
        private IForecastScenarioAdjsRepository adjsRepository;
        private IForecastScenarioIndsRepository indsRepository;

        public ValuationParamController(
                                        IForecastExtension extension, 
                                        IForecastScenarioVarsRepository varsRepository,
                                        IForecastScenarioAdjsRepository adjsRepository,
                                        IForecastScenarioIndsRepository indsRepository)
        {
            this.extension = extension;
            this.varsRepository = varsRepository;
            this.adjsRepository = adjsRepository;
            this.indsRepository = indsRepository;
        }

        public ActionResult ShowExist(int id)
        {
            var scenId = id;
            var viewControl = Resolver.Get<ValuationParamView>();

            var list = (from f in varsRepository.GetAllVars()
                       where f.ID == scenId
                       select new
                       {
                           f.Parent,
                           f.RefYear
                       }).ToList().First();

            int baseId = list.Parent.Value;
            int baseYear = list.RefYear.ID;

            viewControl.Initialize(scenId, baseId, baseYear);
            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult AdjsLoadDT(string key, string activeTab)
        {
            var ufc = extension.Forms[key];
            DataTable dt = ufc.GetObject("AdjsTable") as DataTable;

            string code = activeTab.Split('_').Last();

            var list = (from f in dt.AsEnumerable()
                        where Convert.ToString(f["Code"]).Substring(0, 3) == code
                        select new ValuationListItem
                        {
                            ID = Convert.ToInt32(f["ID"]),
                            ParamName = Convert.ToString(f["ParamName"]),
                            ValueEst = f["ValueEst"] != DBNull.Value ? Convert.ToDecimal(f["ValueEst"]) : (decimal?)null,
                            ValueEstB = f["ValueEstB"] != DBNull.Value ? Convert.ToDecimal(f["ValueEstB"]) : (decimal?)null,
                            ValueY1 = f["ValueY1"] != DBNull.Value ? Convert.ToDecimal(f["ValueY1"]) : (decimal?)null,
                            ValueY1B = f["ValueY1B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY1B"]) : (decimal?)null,
                            ValueY2 = f["ValueY2"] != DBNull.Value ? Convert.ToDecimal(f["ValueY2"]) : (decimal?)null,
                            ValueY2B = f["ValueY2B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY2B"]) : (decimal?)null,
                            ValueY3 = f["ValueY3"] != DBNull.Value ? Convert.ToDecimal(f["ValueY3"]) : (decimal?)null,
                            ValueY3B = f["ValueY3B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY3B"]) : (decimal?)null,
                            ValueY4 = f["ValueY4"] != DBNull.Value ? Convert.ToDecimal(f["ValueY4"]) : (decimal?)null,
                            ValueY4B = f["ValueY4B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY4B"]) : (decimal?)null,
                            ValueY5 = f["ValueY5"] != DBNull.Value ? Convert.ToDecimal(f["ValueY5"]) : (decimal?)null,
                            ValueY5B = f["ValueY5B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY5B"]) : (decimal?)null,
                            MaxBound = Convert.ToDecimal(f["MaxBound"]),
                            MinBound = Convert.ToDecimal(f["MinBound"]),
                            Units = Convert.ToString(f["Units"])
                        }).ToList();

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult IndsLoadDT(string key, string activeTab)
        {
            var ufc = extension.Forms[key];
            DataTable dt = ufc.GetObject("IndsTable") as DataTable;

            string code = activeTab.Split('_').Last();

            var list = (from f in dt.AsEnumerable()
                        where Convert.ToString(f["Code"]).Substring(0, 3) == code
                        select new ValuationListItem
                        {
                            ID = Convert.ToInt32(f["ID"]),
                            ParamName = Convert.ToString(f["ParamName"]),
                            ValueEst = f["ValueEst"] != DBNull.Value ? Convert.ToDecimal(f["ValueEst"]) : (decimal?)null,
                            ValueEstB = f["ValueEstB"] != DBNull.Value ? Convert.ToDecimal(f["ValueEstB"]) : (decimal?)null,
                            ValueY1 = f["ValueY1"] != DBNull.Value ? Convert.ToDecimal(f["ValueY1"]) : (decimal?)null,
                            ValueY1B = f["ValueY1B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY1B"]) : (decimal?)null,
                            ValueY2 = f["ValueY2"] != DBNull.Value ? Convert.ToDecimal(f["ValueY2"]) : (decimal?)null,
                            ValueY2B = f["ValueY2B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY2B"]) : (decimal?)null,
                            ValueY3 = f["ValueY3"] != DBNull.Value ? Convert.ToDecimal(f["ValueY3"]) : (decimal?)null,
                            ValueY3B = f["ValueY3B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY3B"]) : (decimal?)null,
                            ValueY4 = f["ValueY4"] != DBNull.Value ? Convert.ToDecimal(f["ValueY4"]) : (decimal?)null,
                            ValueY4B = f["ValueY4B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY4B"]) : (decimal?)null,
                            ValueY5 = f["ValueY5"] != DBNull.Value ? Convert.ToDecimal(f["ValueY5"]) : (decimal?)null,
                            ValueY5B = f["ValueY5B"] != DBNull.Value ? Convert.ToDecimal(f["ValueY5B"]) : (decimal?)null,
                            MaxBound = Convert.ToDecimal(f["MaxBound"]),
                            MinBound = Convert.ToDecimal(f["MinBound"]),
                            Units = Convert.ToString(f["Units"])
                        }).ToList();

            return new AjaxStoreResult(list, list.Count());
        }
        
        public ActionResult AdjsLoad(int scenId, string activeTab)
        {
            string selectSQL;

            string filter = " p.code like '{0}%' ".FormatWith(activeTab.Split('_').Last());

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0} and {1}".FormatWith(scenId, filter);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0} and {1}".FormatWith(scenId, filter);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<ValuationListItem> list = new List<ValuationListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new ValuationListItem
                {
                    ID = Convert.ToInt32(row[0]),
                    ParamName = Convert.ToString(row[1]),
                    ValueEst = row[2] != null ? Convert.ToDecimal(row[2]) : (decimal?)null,
                    ValueEstB = row[3] != null ? Convert.ToDecimal(row[3]) : (decimal?)null,
                    ValueY1 = row[4] != null ? Convert.ToDecimal(row[4]) : (decimal?)null,
                    ValueY1B = row[5] != null ? Convert.ToDecimal(row[5]) : (decimal?)null,
                    ValueY2 = row[6] != null ? Convert.ToDecimal(row[6]) : (decimal?)null,
                    ValueY2B = row[7] != null ? Convert.ToDecimal(row[7]) : (decimal?)null,
                    ValueY3 = row[8] != null ? Convert.ToDecimal(row[8]) : (decimal?)null,
                    ValueY3B = row[9] != null ? Convert.ToDecimal(row[9]) : (decimal?)null,
                    ValueY4 = row[10] != null ? Convert.ToDecimal(row[10]) : (decimal?)null,
                    ValueY4B = row[11] != null ? Convert.ToDecimal(row[11]) : (decimal?)null,
                    ValueY5 = row[12] != null ? Convert.ToDecimal(row[12]) : (decimal?)null,
                    ValueY5B = row[13] != null ? Convert.ToDecimal(row[13]) : (decimal?)null,
                    MinBound = Convert.ToDecimal(row[14]),
                    MaxBound = Convert.ToDecimal(row[15]),
                    Units = Convert.ToString(row[16])
                });
            }
            
            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult IndsLoad(int scenId, string activeTab)
        {
            string selectSQL;

            string filter = " p.code like '{0}%' ".FormatWith(activeTab.Split('_').Last());

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0} and {1}".FormatWith(scenId, filter);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0} and {1}".FormatWith(scenId, filter);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            List<ValuationListItem> list = new List<ValuationListItem>();

            foreach (object[] row in queryResult)
            {
                list.Add(new ValuationListItem
                {
                    ID = Convert.ToInt32(row[0]),
                    ParamName = Convert.ToString(row[1]),
                    ValueEst = row[2] != null ? Convert.ToDecimal(row[2]) : (decimal?)null,
                    ValueEstB = row[3] != null ? Convert.ToDecimal(row[3]) : (decimal?)null,
                    ValueY1 = row[4] != null ? Convert.ToDecimal(row[4]) : (decimal?)null,
                    ValueY1B = row[5] != null ? Convert.ToDecimal(row[5]) : (decimal?)null,
                    ValueY2 = row[6] != null ? Convert.ToDecimal(row[6]) : (decimal?)null,
                    ValueY2B = row[7] != null ? Convert.ToDecimal(row[7]) : (decimal?)null,
                    ValueY3 = row[8] != null ? Convert.ToDecimal(row[8]) : (decimal?)null,
                    ValueY3B = row[9] != null ? Convert.ToDecimal(row[9]) : (decimal?)null,
                    ValueY4 = row[10] != null ? Convert.ToDecimal(row[10]) : (decimal?)null,
                    ValueY4B = row[11] != null ? Convert.ToDecimal(row[11]) : (decimal?)null,
                    ValueY5 = row[12] != null ? Convert.ToDecimal(row[12]) : (decimal?)null,
                    ValueY5B = row[13] != null ? Convert.ToDecimal(row[13]) : (decimal?)null,
                    MinBound = Convert.ToDecimal(row[14]),
                    MaxBound = Convert.ToDecimal(row[15]),
                    ////GroupName = Convert.ToString(row[10]),
                    Units = Convert.ToString(row[16])
                });
            }

            return new AjaxStoreResult(list, list.Count());
        }

        public ActionResult IndsLoadTable(int scenId, string key)
        {
            var ar = new AjaxResult();

            string selectSQL;

            var ufc = extension.Forms[key];

            DataTable dt = ufc.GetObject("IndsTable") as DataTable;

            ////string filter = " p.code like '{0}%' ".FormatWith(activeTab.Split('_').Last());)

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0}".FormatWith(scenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();
            
            foreach (object[] row in queryResult)
            {
                var datarow = dt.NewRow();
                
                datarow["ID"] = Convert.ToInt32(row[0]);
                datarow["ParamName"] = Convert.ToString(row[1]);
                datarow["ValueEst"] = row[2] != null ? Convert.ToDecimal(row[2]) : (decimal?)null;
                datarow["ValueEstB"] = row[3] != null ? Convert.ToDecimal(row[3]) : (decimal?)null;
                datarow["ValueY1"] = row[4] != null ? Convert.ToDecimal(row[4]) : (decimal?)null;
                datarow["ValueY1B"] = row[5] != null ? Convert.ToDecimal(row[5]) : (decimal?)null;
                datarow["ValueY2"] = row[6] != null ? Convert.ToDecimal(row[6]) : (decimal?)null;
                datarow["ValueY2B"] = row[7] != null ? Convert.ToDecimal(row[7]) : (decimal?)null;
                datarow["ValueY3"] = row[8] != null ? Convert.ToDecimal(row[8]) : (decimal?)null;
                datarow["ValueY3B"] = row[9] != null ? Convert.ToDecimal(row[9]) : (decimal?)null;
                datarow["ValueY4"] = row[10] != null ? Convert.ToDecimal(row[10]) : (decimal?)null;
                datarow["ValueY4B"] = row[11] != null ? Convert.ToDecimal(row[11]) : (decimal?)null;
                datarow["ValueY5"] = row[12] != null ? Convert.ToDecimal(row[12]) : (decimal?)null;
                datarow["ValueY5B"] = row[13] != null ? Convert.ToDecimal(row[13]) : (decimal?)null;
                datarow["MinBound"] = Convert.ToDecimal(row[14]);
                datarow["MaxBound"] = Convert.ToDecimal(row[15]);
                datarow["Units"] = Convert.ToString(row[16]);

                dt.Rows.Add(datarow);
            }

            return ar;
        }

        public ActionResult SaveChange(string key)
        {
            var ar = new AjaxResult();

            var ufc = extension.Forms[key];

            DataTable adjTable = ufc.GetObject("AdjsTable") as DataTable;
            DataTable indTable = ufc.GetObject("IndsTable") as DataTable;

            DataTable adjChanges = adjTable.GetChanges();
            DataTable indChanges = indTable.GetChanges();

            ar.Result = "success";

            if ((adjChanges != null) && (adjChanges.Rows.Count > 0))
            {
                try
                {
                    adjsRepository.DbContext.BeginTransaction();

                    foreach (DataRow row in adjChanges.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        var data = adjsRepository.FindOne(id);

                        data.ValueEstimate = row["ValueEstB"] != null ? Convert.ToDecimal(row["ValueEstB"]) : (decimal?)null;
                        data.ValueY1 = row["ValueY1"] != null ? Convert.ToDecimal(row["ValueY1"]) : (decimal?)null;
                        data.ValueY2 = row["ValueY2"] != null ? Convert.ToDecimal(row["ValueY2"]) : (decimal?)null;
                        data.ValueY3 = row["ValueY3"] != null ? Convert.ToDecimal(row["ValueY3"]) : (decimal?)null;
                        data.ValueY4 = row["ValueY4"] != null ? Convert.ToDecimal(row["ValueY4"]) : (decimal?)null;
                        data.ValueY5 = row["ValueY5"] != null ? Convert.ToDecimal(row["ValueY5"]) : (decimal?)null;

                        adjsRepository.Save(data);
                    }

                    adjsRepository.DbContext.CommitTransaction();

                    adjChanges.AcceptChanges();
                }
                catch (Exception e)
                {
                    adjsRepository.DbContext.RollbackTransaction();
                    ar.Result = "failure";
                    throw new Exception(e.Message, e);
                }
            }

            if ((indChanges != null) && (indChanges.Rows.Count > 0))
            {
                try
                {
                    indsRepository.DbContext.BeginTransaction();

                    foreach (DataRow row in indChanges.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        var data = indsRepository.FindOne(id);

                        data.ValueEstimate = row["ValueEstB"] != null ? Convert.ToDecimal(row["ValueEstB"]) : (decimal?)null;
                        data.ValueY1 = row["ValueY1"] != null ? Convert.ToDecimal(row["ValueY1"]) : (decimal?)null;
                        data.ValueY2 = row["ValueY2"] != null ? Convert.ToDecimal(row["ValueY2"]) : (decimal?)null;
                        data.ValueY3 = row["ValueY3"] != null ? Convert.ToDecimal(row["ValueY3"]) : (decimal?)null;
                        data.ValueY4 = row["ValueY4"] != null ? Convert.ToDecimal(row["ValueY4"]) : (decimal?)null;
                        data.ValueY5 = row["ValueY5"] != null ? Convert.ToDecimal(row["ValueY5"]) : (decimal?)null;

                        indsRepository.Save(data);
                    }

                    indsRepository.DbContext.CommitTransaction();

                    indChanges.AcceptChanges();
                }
                catch (Exception e)
                {
                    indsRepository.DbContext.RollbackTransaction();
                    ar.Result = "failure";
                    throw new Exception(e.Message, e);
                }
            }

            return ar;
        }

        public ActionResult AdjsLoadTable(int scenId, string key)
        {
            var ar = new AjaxResult();
            
            string selectSQL;

            var ufc = extension.Forms[key];

            DataTable dt = ufc.GetObject("IndsTable") as DataTable;
            
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0}".FormatWith(scenId);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0}".FormatWith(scenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();
            
            foreach (object[] row in queryResult)
            {
                var datarow = dt.NewRow();
                
                datarow["ID"] = Convert.ToInt32(row[0]);
                datarow["ParamName"] = Convert.ToString(row[1]);
                datarow["ValueEst"] = row[2] != null ? Convert.ToDecimal(row[2]) : (decimal?)null;
                datarow["ValueEstB"] = row[3] != null ? Convert.ToDecimal(row[3]) : (decimal?)null;
                datarow["ValueY1"] = row[4] != null ? Convert.ToDecimal(row[4]) : (decimal?)null;
                datarow["ValueY1B"] = row[5] != null ? Convert.ToDecimal(row[5]) : (decimal?)null;
                datarow["ValueY2"] = row[6] != null ? Convert.ToDecimal(row[6]) : (decimal?)null;
                datarow["ValueY2B"] = row[7] != null ? Convert.ToDecimal(row[7]) : (decimal?)null;
                datarow["ValueY3"] = row[8] != null ? Convert.ToDecimal(row[8]) : (decimal?)null;
                datarow["ValueY3B"] = row[9] != null ? Convert.ToDecimal(row[9]) : (decimal?)null;
                datarow["ValueY4"] = row[10] != null ? Convert.ToDecimal(row[10]) : (decimal?)null;
                datarow["ValueY4B"] = row[11] != null ? Convert.ToDecimal(row[11]) : (decimal?)null;
                datarow["ValueY5"] = row[12] != null ? Convert.ToDecimal(row[12]) : (decimal?)null;
                datarow["ValueY5B"] = row[13] != null ? Convert.ToDecimal(row[13]) : (decimal?)null;
                datarow["MinBound"] = Convert.ToDecimal(row[14]);
                datarow["MaxBound"] = Convert.ToDecimal(row[15]);
                datarow["Units"] = Convert.ToString(row[16]);

                dt.Rows.Add(datarow);
            }

            return ar;
        }

        public ActionResult ChangeAdjData(int rowid, string col, double newVal, string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable dt = ufc.GetObject("AdjsTable") as DataTable;

            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["id"]) == rowid)
                {
                    row[col] = newVal;
                    break;
                }
            }

            /*dt.AcceptChanges();

            ufc.UpdateObject(key, dt);*/

            return ajaxResult;
        }

        public ActionResult ChangeIndData(int rowid, string col, double newVal, string key)
        {
            AjaxResult ajaxResult = new AjaxResult();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable dt = ufc.GetObject("IndsTable") as DataTable;

            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["id"]) == rowid)
                {
                    row[col] = newVal;
                    break;
                }
            }

            return ajaxResult;
        }

        public ActionResult Calc(int varid)
        {
            AjaxResult ar = new AjaxResult();

            ar.Result = "failure";
            
            if (varid != -1)
            {
                using (new ServerContext())
                {
                    Scheme.ForecastService.CalcModel(varid);
                }

                ar.Result = "success";
            }
            
            return ar;
        }
    }
}
