using System;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers
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
                return (int)(entity.ID / 10000);
            }
        }

        public static int GetMonth(this FX_Date_YearDayUNV entity)
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
                int month = (entity.ID / 100) % 100;
                return month > 12 ? 0 : month;
            }
        }

        public static int GetDay(this FX_Date_YearDayUNV entity)
        {
            if (entity == null)
            {
                return 0;
            }

            if (entity.ID < 0)
            {
                return 0;
            }
            else
            {
                int day = entity.ID % 100;
                if (day >= 1 && day <= 31)
                {
                    return day;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
