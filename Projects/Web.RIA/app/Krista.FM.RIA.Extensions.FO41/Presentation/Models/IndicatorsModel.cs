namespace Krista.FM.RIA.Extensions.FO41.Presentation.Models
{
    public class IndicatorsModel
    {
        public int TempID { get; set; }

        public int Id { get; set; }
        
        public int RefApplication { get; set; }
        
        public decimal? Fact { get; set; }
        
        public decimal? PreviousFact { get; set; }
        
        public int RowType { get; set; }
        
        public decimal? Estimate { get; set; }
        
        public decimal? Forecast { get; set; }
        
        public int RefMarks { get; set; }
        
        public int RefMarksCode { get; set; }
        
        public string Symbol { get; set; }
        
        public string RefName { get; set; }
        
        public string RefNumberString { get; set; }
        
        public int OKEI { get; set; }
        
        public string OKEIName { get; set; }
        
        public bool IsFormula { get; set; }
        
        public string PrevFactFormula { get; set; }
        
        public string FactFormula { get; set; }
        
        public string EstimateFormula { get; set; }
        
        public string ForecastFormula { get; set; }
        
        public bool HasDetail { get; set; }
        
        public int DetailMark { get; set; }
    }
}
