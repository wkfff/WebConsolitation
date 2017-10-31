
namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Models
{
    public class FO51ControlModel
    {
        public int MarkId { get; set; }

        public string MarkName { get; set; }

        public decimal? PassportMOPlanPeriod { get; set; }

        public decimal? MesOtchPlanPeriod { get; set; }

        public decimal? PassportMOFactPeriod { get; set; }

        public decimal? MesOtchFactPeriod { get; set; }

        public decimal? DefectPlanPeriod { get; set; }

        public decimal? DefectFactPeriod { get; set; }
    }
}
