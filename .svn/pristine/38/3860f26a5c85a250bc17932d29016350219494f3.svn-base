using System;
using System.Text;
using System.Web.Mvc;

using Krista.FM.DigitalSignature;

namespace Krista.FM.RIA.Core.Controllers
{
    public class SignedDocumentController : SchemeBoundController
    {
        [HttpPost]
        public ActionResult Save(FormCollection form)
        {
            var document = Convert.FromBase64String(form["document"]);
            var dsign = Convert.FromBase64String(form["dsign"]);

            var result = DSign.Verify(dsign, document);

            if (result.Count == 0)
            {
                var guid = Guid.NewGuid();
                return new JsonResult { Data = new { success = true, guid } };
            }

            StringBuilder sb = new StringBuilder();
            foreach (var errors in result)
            {
                sb.AppendLine(errors);
                sb.AppendLine();
            }

            return new JsonResult { Data = new { success = false, message = sb.ToString() } };
        }
    }
}