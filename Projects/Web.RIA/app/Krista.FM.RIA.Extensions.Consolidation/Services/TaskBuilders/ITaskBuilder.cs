using System;
using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface ITaskBuilder
    {
        IList<D_CD_Task> Build(D_CD_Reglaments reglament, DateTime start, DateTime end);
    }
}
