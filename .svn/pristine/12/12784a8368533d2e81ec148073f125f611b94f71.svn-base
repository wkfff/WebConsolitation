using System;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsTasksFilter
    {
        public static bool FilterStoreData(Domain.ReportsTree x, bool[] filters)
        {
            return FilterStoreData(x, filters, DateTime.Now);
        }

        public static bool FilterStoreData(Domain.ReportsTree x, bool[] filters, DateTime now)
        {
            bool result = FilterStatus(filters, x);

            result &= FilterTimeline(filters, x, now);

            return result;
        }

        private static bool FilterTimeline(bool[] filters, Domain.ReportsTree x, DateTime now)
        {
            bool result = false;

            // По дате сдачи - просроченные
            if (filters[3])
            {
                result |= now > x.Deadline;
            }

            // По дате сдачи и кончу периода - к исполнению
            if (filters[4])
            {
                result |= now >= x.EndDate && now <= x.Deadline;
            }

            // По периоду - текущий период
            if (filters[5])
            {
                result |= now >= x.BeginDate && now < x.EndDate;
            }

            // По периоду - текущий год
            if (filters[6])
            {
                result |= now.Year == x.BeginDate.Year;
            }

            return result;
        }

        private static bool FilterStatus(bool[] filters, Domain.ReportsTree x)
        {
            bool result = false;

            // По статусу
            if (filters[0])
            {
                result |= x.StatusId == 1;
            }

            if (filters[1])
            {
                result |= x.StatusId == 2;
            }

            if (filters[2])
            {
                result |= x.StatusId == 3;
            }

            return result;
        }
    }
}
