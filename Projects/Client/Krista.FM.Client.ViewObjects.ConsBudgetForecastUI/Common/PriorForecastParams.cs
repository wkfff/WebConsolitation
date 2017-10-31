using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common
{
    public enum PriorForecastFormParams
    {
        Contingent = 0,
        ContingentSplit = 1,
        BudLevelsVariant = 2,
        Unknown = -1
    }

    public enum TerrType
    {
        SB = 3,
        MR = 4,
        GO = 7
    }

    public class PriorForecastParams
    {
        public PriorForecastFormParams PriorForecastFormParams
        {
            get; set;
        }

        public TerrType TerrType
        {
            get; set;
        }

        public long ContingentVariant
        {
            get; set;
        }

        public long BudLevelVariant
        { 
            get; set;
        }

        public bool Estimate
        {
            get; set;
        }

        public bool Forecast
        {
            get; set;
        }

        public int Year
        {
            get; set;
        }

        public long SourceId
        {
            get; set;
        }
    }
}
