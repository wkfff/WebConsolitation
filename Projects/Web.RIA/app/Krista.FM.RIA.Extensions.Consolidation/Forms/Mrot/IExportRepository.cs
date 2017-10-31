using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public interface IExportRepository
    {
        IList<D_CD_Subjects> GetIncompleteRegions(int taskId);

        D_CD_Task GetTaskInfo(int taskId);

        IList<D_CD_Subjects> GetCompleteRegions(int taskId);

        IList<SubjectTrihedrDataModel> GetSubjectTrihedrData(int taskId);

        IList<D_CD_Task> GetChildTasks(int taskId);
    }
}
