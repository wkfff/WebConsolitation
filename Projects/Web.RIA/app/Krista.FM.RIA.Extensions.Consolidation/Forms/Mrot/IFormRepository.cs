using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public interface IFormRepository
    {
        IList<FormModel> GetFormData(int taskId);
    }
}