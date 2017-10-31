using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Data;

namespace Krista.FM.Common.Consolidation.Calculations
{
    public interface IPrimaryDataProvider
    {
        IList<IRecord> GetSectionRows();

        IRecord CreateRecord(IRecord template);

        void AppendRecord(IRecord record);

        IList<IRecord> GetMultipliesRowsTemplates();
    }
}
