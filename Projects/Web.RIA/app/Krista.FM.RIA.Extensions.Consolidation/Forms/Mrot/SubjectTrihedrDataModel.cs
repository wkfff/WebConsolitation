namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class SubjectTrihedrDataModel
    {
        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public string RegionCode { get; set; }

        public string RegionTypeName { get; set; }

        public string OrgName { get; set; }

        public int OrgType { get; set; }

        /// <summary>
        /// Организации внебюджетного сектора экономики
        /// </summary>
        public decimal? PrincipalCountOffBudget { get; set; }

        public decimal? WorkerCountOffBudget { get; set; }

        // участники трехстороннего соглашения и работодатели, присоединившиеся к нему
        public decimal? PrincipalCountJoined { get; set; }

        public decimal? WorkerCountJoined { get; set; }

        // выполняющие условия в части установленного минимального размера заработной платы
        public decimal? PrincipalCountMinSalary { get; set; }

        public decimal? WorkerCountMinSalary { get; set; }

        // выполняющие условия в части среднего размера заработной платы
        public decimal? PrincipalCountAvgSalary { get; set; }

        public decimal? WorkerCountAvgSalary { get; set; }

        // справочные
        public decimal? MinSalary { get; set; }

        public decimal? AvgSalary { get; set; }
    }
}
