using System.Reflection;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Infrastructure
{
    public interface IRebuildMappingService
    {
        void Rebuild(Assembly[] domainAssemblies);
    }
}