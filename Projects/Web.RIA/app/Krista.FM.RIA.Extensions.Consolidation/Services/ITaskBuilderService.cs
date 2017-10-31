using System;
using System.Collections.Generic;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface ITaskBuilderService
    {
        void BuildTasks(DateTime startDate, DateTime endDate, IList<D_CD_Reglaments> reglamentses);

        void BuildTasks(DateTime startDate, DateTime endDate, D_CD_Reglaments reglament);

        void BuildCollectingTask(D_CD_CollectTask task);
    }
}