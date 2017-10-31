using System;
using System.Collections.Generic;
using System.Linq;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public class Factor
    {
        private Boolean clsFeatureRF = true; // классфикационный признак по РФ
        private List<UseHandBooks> useHandBooks; //используемые справочники
        private List<CrossHandBooks> crossHandBooks; //пересечения справочников
        private List<string> territorySet; 

        public Factor()
        {
            ClsFeatureSubject = false;
            ClsFeatureFO = false;
            useHandBooks = new List<UseHandBooks>();
            crossHandBooks = new List<CrossHandBooks>();
            territorySet = new List<string>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string CubeName { get; set; }

        public string Unit { get; set; }

        public string ServiceCubeName { get; set; }

        public string WorkYear { get; set; }

        public string FrequencyWorks { get; set; }

        public string TimeProvision { get; set; }

        public string DepthTimeSet { get; set; }

        public bool ClsFeatureRF
        {
            get { return clsFeatureRF; }
            set { clsFeatureRF = value; }
        }

        public bool ClsFeatureFO { get; set; }

        public bool ClsFeatureSubject { get; set; }

        public void SetUseHandBooks(List<UseHandBooks> handBooks)
        {
            useHandBooks = handBooks;
        }

        public int GetCountUseHandBooks()
        {
            return useHandBooks.Count();
        }

        public List<UseHandBooks> GetUseHandBooks()
        {
            return useHandBooks;
        }

        public void SetCrossHandBooks(List<CrossHandBooks> handBooks)
        {
            crossHandBooks = handBooks;
        }

        public int GetCountCrossHandBooks()
        {
            return crossHandBooks.Count();
        }

        public List<CrossHandBooks> GetCrossHandBooks()
        {
            return crossHandBooks;
        }

        public void AddValueInTerritorySet(string value)
        {
            territorySet.Add(value);
        }

        public List<string> GetTerritorySet()
        {
            return territorySet;
        }
    }
}
