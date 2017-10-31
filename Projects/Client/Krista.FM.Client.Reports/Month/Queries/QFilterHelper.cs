using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.FactTables;

namespace Krista.FM.Client.Reports.Month.Queries
{
    class QFilterHelper
    {
        public const string fltSplitter = " or ";
        public const string fltMultiSplitter = " and ";
        public const string fltPrefix = "f.";
        public const string fltPrefixName = "f";

        private string GetPrefix(bool usePrefix)
        {
            var prefix = String.Empty;

            if (usePrefix)
            {
                prefix = fltPrefix;
            }

            return prefix;
        }

        public string PeriodFilter(string fieldName, string loValue, string hiValue, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{3}{0} >= {1} and {3}{0} <= {2}", fieldName, loValue, hiValue, prefix);
        }

        public string MoreFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} > {1}", fieldName, value, prefix);
        }

        public string MoreEqualFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} >= {1}", fieldName, value, prefix);
        }

        public string LessFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} < {1}", fieldName, value, prefix);
        }

        public string EqualIntFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} = {1}", fieldName, value, prefix);
        }

        public string NotEqualIntFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} <> {1}", fieldName, value, prefix);
        }

        public string RangeFilter(string fieldName, object value, bool usePrefix = false)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{2}{0} in ({1})", fieldName, value, prefix);
        }

        public string GetYearFilter(int year, bool usePrefix = true)
        {
            var yearLoBound = ReportDataServer.GetUNVYearLoBound(year);
            var yearHiBound = ReportDataServer.GetUNVYearEnd(year);
            return GetYearFilter(yearLoBound, yearHiBound, usePrefix);
        }

        public string GetAbsYearFilter(int year, bool usePrefix = true)
        {
            var yearLoBound = ReportDataServer.GetUNVYearLoBound(year);
            var yearHiBound = ReportDataServer.GetUNVAbsYearEnd(year);
            return GetYearFilter(yearLoBound, yearHiBound, usePrefix);
        }

        public string GetYearFilter(string yearLoDate, string yearHiDate, bool usePrefix = true)
        {
            var prefix = GetPrefix(usePrefix);
            return String.Format("{3}{0}>={1} and {3}{0}<={2}", 
                f_F_MonthRepIncomes.RefYearDayUNV, 
                yearLoDate, 
                yearHiDate,
                prefix);            
        }

        public string GetMultiYearFilter(List<int> years)
        {
            var filterPeriod = String.Empty;

            foreach (var year in years)
            {
                var yearPeriod = GetYearFilter(year); 
                filterPeriod = ReportDataServer.Combine(filterPeriod, yearPeriod, fltSplitter);
            }

            filterPeriod = ReportDataServer.Trim(filterPeriod, fltSplitter);

            return filterPeriod;
        }

        public string GetAbsMultiYearFilter(List<int> years)
        {
            var filterPeriod = String.Empty;

            foreach (var year in years)
            {
                var yearPeriod = GetAbsYearFilter(year);
                filterPeriod = ReportDataServer.Combine(filterPeriod, yearPeriod, fltSplitter);
            }

            filterPeriod = ReportDataServer.Trim(filterPeriod, fltSplitter);

            return filterPeriod;
        }
    }
}
