using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsTasksController : SchemeBoundController
    {
        private readonly ReportTreeService reportTreeService;
        private readonly IUserSessionState sessionState;

        public ConsTasksController(
            ReportTreeService reportTreeService, 
            IUserSessionState sessionState)
        {
            this.reportTreeService = reportTreeService;
            this.sessionState = sessionState;
        }

        [HttpGet]
        public RestResult Load(int subjectId, bool[] filters, bool showChilds)
        {
            // TODO: [Безопасность] Ограничить выбор subjectId: нельзя указывать subjectId находящийся выше или в другой ветке
            try
            {
                IList<ReportsTree> reportsTrees;
                if (subjectId == -1)
                {
                    IEnumerable<ReportsTree> list = new List<ReportsTree>();
                    list = sessionState.Subjects
                        .Aggregate(
                            list,
                            (current, subject) => current.Union(reportTreeService.GetReportsTree(subject.ID, showChilds)
                                .Where(x => x.SubjectId == subject.ID)));
                    
                    reportsTrees = list.ToList();
                }
                else
                {
                    reportsTrees = reportTreeService.GetReportsTree(subjectId, showChilds);
                }

                var data = from t in reportsTrees.Where(x => ConsTasksFilter.FilterStoreData(x, filters))
                           orderby t.TemplateGroup, t.Deadline
                           select new
                            {
                                t.ID,
                                t.Year,
                                t.Period,
                                t.FormName,
                                t.TemplateName,
                                t.TemplateShortName,
                                t.TemplateClass,
                                t.TemplateGroup,
                                t.Role,
                                t.ReportLevel,
                                t.Subject, 
                                t.SubjectShortName,
                                t.BeginDate, t.EndDate, t.Deadline, t.Status,
                                t.LastChangeDate,
                                t.LastChangeUser
                            };

                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
