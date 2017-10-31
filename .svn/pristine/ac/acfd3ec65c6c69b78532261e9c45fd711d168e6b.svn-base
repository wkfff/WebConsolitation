using System;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public static class FX_Date_YearDayUNVExtensions
    {
        public static int GetYear(this FX_Date_YearDayUNV entity)
        {
            if (entity == null)
            {
                return 0;
            }

            if (entity.ID < 0)
            {
                return entity.ID;
            }
            else
            {
                return Convert.ToInt32(entity.ID.ToString().Substring(0, 4));    
            }
        }

        public static bool IsQuarter(this FX_Date_YearDayUNV entity)
        {
            if (entity == null)
            {
                return false;
            }

            if (entity.ID < 0)
            {
                return false;
            }
            else
            {
                return entity.ID.ToString().Substring(4, 3) == "999" 
                       && entity.ID % 10 >= 1
                       && entity.ID % 10 <= 4;
            }
        }
    }
}
