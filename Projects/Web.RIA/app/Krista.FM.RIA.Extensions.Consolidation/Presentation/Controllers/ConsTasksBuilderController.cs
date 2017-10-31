using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsTasksBuilderController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Reglaments> reglRepository;
        private readonly TaskBuilderService taskBuilderService;

        public ConsTasksBuilderController(
            ILinqRepository<D_CD_Reglaments> reglRepository,
            TaskBuilderService taskBuilderService)
        {
            this.reglRepository = reglRepository;
            this.taskBuilderService = taskBuilderService;
        }

        [HttpPost]
        [Transaction]
        public ActionResult Build(DateTime startDate, DateTime endDate, object reglaments)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<int[]>(Convert.ToString(reglaments));

                List<D_CD_Reglaments> reglamentses = data.Select(reglamentId => reglRepository.FindOne(reglamentId)).ToList();

                taskBuilderService.BuildTasks(startDate, endDate.AddDays(1), reglamentses);

                return new AjaxResult();
            }
            catch (Exception e)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Ошибка",
                    Text = e.ExpandException(new HtmlExceptionFormatter())
                };

                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }
        }
    }
}
