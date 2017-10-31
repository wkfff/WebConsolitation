using System;
using System.Web.Mvc;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class StateTask2016ViewController : SchemeBoundController
    {
        private readonly IAuthService auth;
        private readonly INewRestService newRestService;

        public StateTask2016ViewController()
        {
            auth = Resolver.Get<IAuthService>();
            newRestService = Resolver.Get<INewRestService>();
        }

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportStateTask2016Service.Serialize(auth, newRestService.Load<F_F_ParameterDoc>(recId)),
                "application/xml",
                "stateTask" + DateTime.Now.ToString("yyyymmddhhmmss") + ".xml");
        }
    }
}
