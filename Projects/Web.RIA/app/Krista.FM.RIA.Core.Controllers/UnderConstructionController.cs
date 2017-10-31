using System.Web.Mvc;

namespace Krista.FM.RIA.Core.Controllers
{
    public class UnderConstructionController : Controller
    {
        public const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/";

        public ActionResult Show()
        {
            return View(ViewRoot + "UnderConstruction.aspx");
        }
    }
}
