using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms
{
    public interface IFormActivatorService
    {
        void Activate(int formId);

        void RebuildSession(D_CD_Templates form);
    }
}