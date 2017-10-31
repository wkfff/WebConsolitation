using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using Ext.Net;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Extensions.Consolidation.Models
{
    public class LayoutMarkupViewModel : ICustomConfigSerialization
    {
        public LayoutMarkupViewModel()
        {
            Rows = new Dictionary<int, List<string>>();
            Cells = new List<LayoutMarkupCellViewModel>();
            Styles = new Dictionary<short, Dictionary<string, string>>();
            StylesInnerCell = new Dictionary<short, Dictionary<string, string>>();
        }

        public int Height { get; set; }
        
        public int TotalColumns { get; set; }

        public int FirstRow { get; set; }
        
        public int LastRow { get; set; }

        public Dictionary<int, List<string>> Rows { get; set; }

        public List<LayoutMarkupCellViewModel> Cells { get; set; }

        public Dictionary<short, Dictionary<string, string>> Styles { get; set; }

        public Dictionary<short, Dictionary<string, string>> StylesInnerCell { get; set; }
        
        public string ToScript(Control owner)
        {
            var stringBuilder = new StringBuilder();
            JsonSerializer.Create(new JsonSerializerSettings())
                .Serialize(new JsonTextWriter(new StringWriter(stringBuilder)), this);
            return stringBuilder.ToString();
        }
    }
}
