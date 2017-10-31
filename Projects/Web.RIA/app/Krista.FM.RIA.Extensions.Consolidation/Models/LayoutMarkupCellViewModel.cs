using Newtonsoft.Json;

namespace Krista.FM.RIA.Extensions.Consolidation.Models
{
    public class LayoutMarkupCellViewModel
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "height")]
        public short Height { get; set; }

        [JsonProperty(PropertyName = "colspan")]
        public int Colspan { get; set; }

        [JsonProperty(PropertyName = "rowspan")]
        public int Rowspan { get; set; }

        [JsonProperty(PropertyName = "style")]
        public short Style { get; set; }

        [JsonProperty(PropertyName = "readonly")]
        public bool ReadOnly { get; set; }

        [JsonProperty(PropertyName = "required")]
        public bool Required { get; set; }

        [JsonProperty(PropertyName = "colId")]
        public string ColumnId { get; set; }
    }
}
