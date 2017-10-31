using System.Collections.Generic;

namespace Krista.FM.RIA.Core.ViewModel
{
    /// <summary>
    /// Задает поведение интерфейса.
    /// </summary>
    public interface IViewService
    {
        List<ActionDescriptor> Actions { get; }

        string GetDataFilter();
        
        string GetClientScript();
    }
}
