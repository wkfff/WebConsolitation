using System;
using System.IO;
using Krista.FM.Common.TaskDocuments;
using Krista.FM.Common.Tasks;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Tasks.TaskUtils
{
    public class TaskUtils
    {
        public static void SetTaskContext(string fileName, ITask task, int documentId, IScheme scheme)
        {
            try
            {
                TaskDocumentHelper.SetTaskContext(
                    fileName,
                    (int)TaskDocumentType.dtPlanningSheet, 
                    false,
                    false, 
                    task.InEdit, 
                    scheme, 
                    task.Headline, 
                    task.ID,
                    Path.GetFileNameWithoutExtension(fileName), 
                    documentId,
                    scheme.UsersManager.GetUserNameByID(task.Doer), 
                    new TaskContext(task),
                    DocumentActionType.None
                );
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибки передачи контекста задачи в документ: {0}",
                        Diagnostics.KristaDiagnostics.ExpandException(e));

                throw new Exception("Ошибки передачи контекста задачи в документ", e);
            }
        }
    }
}
