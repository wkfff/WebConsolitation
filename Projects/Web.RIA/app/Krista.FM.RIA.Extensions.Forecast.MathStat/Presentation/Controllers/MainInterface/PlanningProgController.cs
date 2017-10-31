using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningProgController : SchemeBoundController
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        private readonly IForecastExtension extension;

        public PlanningProgController(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult Show(int id)
        {
            var viewControl = Resolver.Get<PlanningProgView>();

            viewControl.Initialize(id);

            return View(ViewRoot + "View.aspx", viewControl);
        }

        public ActionResult LoadProg(string key)
        {
            UserFormsControls ufc = this.extension.Forms[key];

            DataTable datProg = ufc.DataService.GetProgData();
            DataTable datStatic = ufc.DataService.GetStaticData();

            DataTable tempTable = datProg.Clone();

            DataRow rowStatic = (from row in datStatic.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();
            DataRow rowProg = (from row in datProg.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();

            tempTable.Columns.Add("image", typeof(string));

            DataRow newRow = tempTable.NewRow();
            
            foreach (DataColumn col in datProg.Columns)
            {
                if (col.ColumnName != "id")
                {
                    newRow[col.ColumnName] = rowProg[col.ColumnName];
                }
            }
            
            newRow["image"] = "<img src='/Krista.FM.RIA.Extensions.Forecast.MathStat/Presentation/Content/prog.png/extention.axd'></img>";
            
            /*Image img = new Image
            {
                ID = "imgLogo",
                AutoWidth = true,
                AutoHeight = true,
                ImageUrl = "/Krista.FM.RIA.Extensions.Forecast.MathStat/Presentation/Content/Formulas/exp.png/extention.axd"
            };*/

            ////string s = img.GetGeneratedScripts();

            tempTable.Rows.Add(newRow);
            
            newRow = tempTable.NewRow();

            foreach (DataColumn col in datStatic.Columns)
            {
                if (col.ColumnName != "id")
                {
                    newRow[col.ColumnName] = rowStatic[col.ColumnName];
                }
            }

            newRow["image"] = "<img src='/Krista.FM.RIA.Extensions.Forecast.MathStat/Presentation/Content/stat.png/extention.axd'></img>";
            tempTable.Rows.Add(newRow);
            
            if (datProg.Rows.Count > 1)
            {
                List<DataRow> otherRowsProg = (from row in datProg.AsEnumerable()
                                   where Convert.ToInt32(row["id"]) != ufc.ParamId
                                   select row).ToList();

                foreach (DataRow row in otherRowsProg)
                {
                    newRow = tempTable.NewRow();

                    foreach (DataColumn col in datProg.Columns)
                    {
                        if (col.ColumnName != "id")
                        {
                            newRow[col.ColumnName] = row[col.ColumnName];
                        }
                    }
                    
                    tempTable.Rows.Add(newRow);
                }
            }

            return new AjaxStoreResult(tempTable, tempTable.Rows.Count);
        }
    }
}
