using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface ITaskService
    {
        IList GetTasksTable(int programId);

        IList GetTasksTableForLookup(int programId); 
        
        void Create(int programId, D_ExcCosts_Goals target, string taskName, string taskNote);

        void Update(int programId, D_ExcCosts_Goals target, int taskId, string taskName, string taskNote);

        void Delete(int taskId, int programId);

        D_ExcCosts_Tasks GetTask(int id);

        void DeleteAllTask(int targetId);
    }
}