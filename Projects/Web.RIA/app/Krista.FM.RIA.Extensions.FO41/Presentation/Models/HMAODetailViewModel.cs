using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Models
{
    public class HMAODetailViewModel : DetailsViewModel
    {
        public long? INN { get; set; }

        public string KPP { get; set; }

        public int BridgeRegionId { get; set; }

        public string BridgeRegionName { get; set; }

        public string BridgeRegionShortName { get; set; }

        public string LegalAddress { get; set; }

        public string Address { get; set; }
        
        public int? Unit { get; set; }

        public long? OKATO { get; set; }

        public FX_FX_TypeTax TypeTax { get; set; }
    }
}
