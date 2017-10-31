using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Models
{
    public class EstimateModel
    {
        public int ID { get; set; }

        public int Level { get; set; }

        public string CritName { get; set; }

        public decimal? Weight { get; set; }

        public string Point { get; set; }

        public decimal? Estimate { get; set; }

        public int? SelectedId { get; set; }

        public string Comment { get; set; }
    }
}
