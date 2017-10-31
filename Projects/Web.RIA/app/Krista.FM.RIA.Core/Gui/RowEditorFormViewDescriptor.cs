using System.Collections.Generic;

namespace Krista.FM.RIA.Core.Gui
{
    public class RowEditorFormViewDescriptor
    {
        public RowEditorFormViewDescriptor()
        {
            Params = new List<RowEditorFormViewParameterDescriptor>();
        }

        public string Id { get; set; }

        public string Url { get; set; }
        
        public List<RowEditorFormViewParameterDescriptor> Params { get; set; }
    }
}