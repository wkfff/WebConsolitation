using System;
using System.Data;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Tasks
{
    public class TaskNavController : SchemeBoundController
    {
        private const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/TaskNav/";

        public ActionResult Index()
        {
            return View(ViewRoot + "Index.aspx");
        }

        public AjaxStoreResult Data()
        {
            DataTable dt = Scheme.TaskManager.Tasks.GetTasksInfo();

            DataTable dest = dt.Clone();

            int start = Convert.ToInt32(Request.Form["start"]);
            int limit = Convert.ToInt32(Request.Form["limit"]);
            int? anode = Request.Form["anode"].IsNullOrEmpty() ? null : (int?)Convert.ToInt32(Request.Form["anode"]);

            string filter = "REFTASKS is null";
            if (anode != null)
                filter = "REFTASKS = " + anode;

            int indx = 1;
            int count = 0;
            DataRow[] rows = dt.Select(filter, "ID" + " " + "ASC");
            foreach (var row in rows)
            {
                if (indx > start && count < limit)
                {
                    dest.Rows.Add(row.ItemArray);
                    count++;
                }
                if (count == limit)
                    break;
                indx++;
            }

            return new AjaxStoreExtraResult(dest, rows.GetLength(0), User.Identity.Name);
        }
    }
}
