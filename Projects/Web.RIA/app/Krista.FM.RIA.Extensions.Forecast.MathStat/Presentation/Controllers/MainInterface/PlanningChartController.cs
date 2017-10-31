using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningChartController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;

        public PlanningChartController(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult Show(int id)
        {
            var viewControl = Resolver.Get<PlanningChartView>();

            viewControl.Initialize(id);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult LoadProgChart(string key)
        {
            List<ChartData> lst = new List<ChartData>();

            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();
            DataTable datStatic = ufc.DataService.GetStaticData();

            /*foreach (DataColumn c in datProg.Columns)
            {
                string colName = c.ColumnName;
                if (colName.Contains("year_"))
                {
                    int year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                    double xp = Convert.ToDouble(datProg.Rows[0][colName]);
                    double? xs = null;
                    if (datStatic.Columns.Contains(colName))
                    {
                        xs = Convert.ToDouble(datStatic.Rows[0][colName]);
                    }

                    lst.Add(new ChartData { Year = year, Xp = xp, Xs = xs });
                }
            }*/

            ////for (int i = 0; i < datStatic.Columns.Count + pyears; i++)

            DataRow progDataRow = (from row in datProg.AsEnumerable()
                                   where Convert.ToInt32(row["id"]) == ufc.ParamId
                                   select row).First();

            DataRow staticDataRow = (from row in datStatic.AsEnumerable()
                                     where Convert.ToInt32(row["id"]) == ufc.ParamId
                                     select row).First();

            for (int i = 0; i < datProg.Columns.Count; i++)
            {
                DataColumn col = datProg.Columns[i];
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    int year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                    double? xp = null;
                    object op = progDataRow[colName];
                    if (op != DBNull.Value)
                    {
                        xp = Convert.ToDouble(op);
                    }

                    double? xs = null;
                    if (datStatic.Columns.Contains(colName))
                    {
                        object os = staticDataRow[colName];
                        if (os != DBNull.Value)
                        {
                            xs = Convert.ToDouble(os);
                        }
                    }

                    lst.Add(new ChartData { Year = year, Xp = xp, Xs = xs });
                }
            }

            /*lst.Add(new C { Year = 2000, Xp = Convert.ToDouble(datProg.Rows[0]["year_2000"]), Xs = Convert.ToDouble(datStatic.Rows[0]["year_2000"]) });
            lst.Add(new C { Year = 2001, Xp = Convert.ToDouble(datProg.Rows[0]["year_2001"]), Xs = Convert.ToDouble(datStatic.Rows[0]["year_2001"]) });
            lst.Add(new C { Year = 2002, Xp = Convert.ToDouble(datProg.Rows[0]["year_2002"]), Xs = Convert.ToDouble(datStatic.Rows[0]["year_2002"]) });
            lst.Add(new C { Year = 2003, Xp = Convert.ToDouble(datProg.Rows[0]["year_2003"]), Xs = Convert.ToDouble(datStatic.Rows[0]["year_2003"]) });
            lst.Add(new C { Year = 2004, Xp = Convert.ToDouble(datProg.Rows[0]["year_2004"]), Xs = Convert.ToDouble(datStatic.Rows[0]["year_2004"]) });
            lst.Add(new C { Year = 2005, Xp = Convert.ToDouble(datProg.Rows[0]["year_2005"]), Xs = null });
            lst.Add(new C { Year = 2006, Xp = Convert.ToDouble(datProg.Rows[0]["year_2006"]), Xs = null });*/

            ////return new AjaxStoreResult(view, view.Count());););
            return new AjaxStoreResult(lst, lst.Count);
        }
    }
}
