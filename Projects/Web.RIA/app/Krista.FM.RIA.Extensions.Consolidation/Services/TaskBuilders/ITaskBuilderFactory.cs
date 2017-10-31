using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface ITaskBuilderFactory
    {
        ITaskBuilder CreateBuilder(FX_FX_Periodicity periodicity);
    }
}