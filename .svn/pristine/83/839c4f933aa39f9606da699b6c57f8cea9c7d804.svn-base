using System;
using System.Web.Mvc;

using Krista.FM.RIA.Core.Progress;

namespace Krista.FM.RIA.Core.Controllers
{
    [ControllerSessionState(ControllerSessionState.Disabled)]
    public class ProgressController : Controller
    {
        private readonly IProgressManager progressManager;

        public ProgressController(IProgressManager progressManager)
        {
            this.progressManager = progressManager;
        }

        public JsonResult Get()
        {
            var state = progressManager.GetStatus(GetTaskId());
            return Json(new { state.Text, state.Percentage });
        }

        /// <summary>
        /// Извлекает ID текущей задачи из Http-заголовка запроса.
        /// </summary>
        /// <returns>ID задачи.</returns>
        private string GetTaskId()
        {
            var id = Request.Headers[ProgressManager.HeaderNameTaskId];
            return id ?? String.Empty;
        }
    }
}
