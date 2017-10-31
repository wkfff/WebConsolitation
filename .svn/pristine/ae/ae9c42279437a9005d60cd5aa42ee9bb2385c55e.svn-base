using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.RIA.Core.Controllers.Binders;

namespace Krista.FM.RIA.Core.Controllers
{
    public class UserController : SchemeBoundController
    {
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        public AjaxStoreResult DataPaging(
            [DefaultBinder(0)] string start,
            [DefaultBinder(10)] string limit, 
            string sort, 
            string dir,
            string query)
        {
            int startValue = Convert.ToInt32(start);
            int limitValue = Convert.ToInt32(limit);

            DataTable dt = Scheme.UsersManager.GetUsers();

            List<DataRow> plants = new List<DataRow>();
            if (!string.IsNullOrEmpty(query) && query != "*")
            {
                plants.AddRange(dt.Select(String.Format("Name like '{0}%'", query.ToUpper())));
            }
            else
            {
                plants.AddRange(dt.Select());
            }

            if ((startValue + limitValue) > plants.Count)
            {
                limitValue = plants.Count - startValue;
            }

            List<DataRow> rangePlants = (startValue < 0 || limitValue < 0) ? plants : plants.GetRange(startValue, limitValue);

            return new AjaxStoreExtraResult(rangePlants, plants.Count, String.Empty);
        }
    }
}
