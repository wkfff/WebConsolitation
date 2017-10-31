using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;

namespace Krista.FM.Client.Reports.UFNS.ReportQueries
{
    public class QFilter
    {
        internal Enum Key { get; set; }
        internal string Filter { get; set; }
        internal string Not { get; set; }

        public QFilter(Enum key, string filter, bool isNegative = false)
        {
            Key = key;
            Filter = filter;
            Not = isNegative ? "NOT" : String.Empty;
        }

        public QFilter(Enum key, int filter, bool isNegative = false)
        {
            Key = key;
            Filter = Convert.ToString(filter);
            Not = isNegative ? "NOT" : String.Empty;
        }
    }

    public class QFactTable
    {
        protected class QParam
        {
            internal string FactField { get; set; }
            internal string SourceField { get; set; }
            internal string Alias { get; set; }
        }

        private const string AliasSplitter = " as ";

        protected GroupHelper InfoFact;
        protected Dictionary<Enum, GroupHelper> Sources = new Dictionary<Enum, GroupHelper>();
        protected Dictionary<Enum, QParam> Params = new Dictionary<Enum, QParam>();
        public string Result { get; set; }

        public static string Having(string field, int k = 1)
        {
            return k == 1
                ? String.Format("HAVING Sum({0}.{1})", QFilterHelper.fltPrefixName, field)
                : String.Format("HAVING {2}*Sum({0}.{1})", QFilterHelper.fltPrefixName, field, k);
        }

        public QFactTable(string factTableKey)
        {
            InfoFact = new GroupHelper
            {
                Prefix = QFilterHelper.fltPrefixName,
                EntityKey = factTableKey
            };

            Result = "(*)";
        }

        protected void AddSource(Enum param, string entityKey)
        {
            var info = new GroupHelper { Prefix = param.ToString(), EntityKey = entityKey };

            foreach (var source in Sources.Where(source => source.Value.EntityKey == entityKey))
            {
                info = source.Value;
                break;
            }

            Sources.Add(param, info);
        }

        protected void AddParam(Enum param, string factField, string sourceField, string alias = null)
        {
            var repParam = new QParam { FactField = factField, SourceField = sourceField, Alias = alias};
            Params.Add(param, repParam);
        }

        protected bool AddTable(List<string> tables, GroupHelper info)
        {
            var table = String.Format("{0} {1}", info.Entity.FullDBName, info.Prefix);
            if (!tables.Contains(table))
            {
                tables.Add(table);
                return true;
            }
            
            return false;
        }

        private void AddFactFilter(List<string> filters, string filter)
        {
            if (filter.Length > 0)
            {
                filters.Add(filter);
            }
        }

        private string GetInFilter(string prefix, string field, QFilter filter)
        {
            var word = String.Format("{0} in", filter.Not).Trim();
            return String.Format("{0}.{1} {2} ({3})", prefix, field, word, filter.Filter);
        }

        private void AddFactFilter(List<string> filters, string field, QFilter filter)
        {
            if (filter.Filter.Length > 0)
            {
                filters.Add(GetInFilter(InfoFact.Prefix, field, filter));
            }
        }

        private string GetIdFilter(string factField, string prefix)
        {
            return String.Format("{0}.{1} = {2}.id", InfoFact.Prefix, factField, prefix);
        }

        private void AddFilter(List<string> filters, List<string> tables, QParam filterParam, QFilter filter)
        {
            if (filter.Filter.Length > 0)
            {
                if (!Sources.ContainsKey(filter.Key))
                {
                    return;
                }
                var info = Sources[filter.Key];
                var flt = GetInFilter(info.Prefix, filterParam.SourceField, filter);
                if (AddTable(tables, info))
                {
                    filters.Add(String.Format("{0} and {1}", GetIdFilter(filterParam.FactField, info.Prefix), flt));
                }
                else
                {
                    filters.Add(flt);
                }
            }
        }

        private void AddGroups(List<string> groups, List<string> filters, List<string> tables, IEnumerable<Enum> keys)
        {
            foreach (var key in keys)
            {
                if (!Params.ContainsKey(key))
                {
                    continue;
                }

                var groupParams = Params[key];
                var alias = String.IsNullOrEmpty(groupParams.Alias)
                                ? String.Empty
                                : String.Format("{0}{1}", AliasSplitter, groupParams.Alias);

                if (groupParams.SourceField != null)
                {
                    if (!Sources.ContainsKey(key))
                    {
                        continue;
                    }

                    var info = Sources[key];

                    if (AddTable(tables, info))
                    {
                        filters.Add(GetIdFilter(groupParams.FactField, info.Prefix));
                    }

                    groups.Add(String.Format("{0}.{1}{2}", info.Prefix, groupParams.SourceField, alias));
                    continue;
                }

                if (groupParams.FactField != null)
                {
                    groups.Add(String.Format("{0}.{1}{2}", InfoFact.Prefix, groupParams.FactField, alias));
                }
            }
        }

        private void AddFilters(List<string> filters, List<string> tables, IEnumerable<QFilter> filterList)
        {
            foreach (var filter in filterList)
            {
                var key = filter.Key;
                if (!Params.ContainsKey(key))
                {
                    continue;
                }

                var filterParams = Params[key];
                if (filterParams.SourceField != null)
                {
                    AddFilter(filters, tables, filterParams, filter);
                }
                else
                {
                    if (filterParams.FactField != null)
                    {
                        AddFactFilter(filters, filterParams.FactField, filter);
                    }
                    else
                    {
                        AddFactFilter(filters, filter.Filter);
                    }
                }
            }
        }

        public string GetQueryText(List<QFilter> filterList, List<Enum> groupList, string havingFilter)
        {
            var tables = new List<string>();
            var filters = new List<string>();
            var groups = new List<string>();
            AddTable(tables, InfoFact);
            AddFilters(filters, tables, filterList);
            if (groupList != null)
            {
                AddGroups(groups, filters, tables, groupList);
            }
            var strSelect = Result;
            var strTables = String.Join(", ", tables.ToArray());
            var strFilters = String.Empty;
            var strGroups = String.Empty;

            if (filters.Count > 0)
            {
                strFilters = String.Format("where\r\n\t{0}", String.Join(" and\r\n\t", filters.ToArray()));
            }

            if (groups.Count > 0)
            {
                const StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
                var groupsWithAlias = groups.ToArray();
                var groupsWithoutAlias = groups.Select(e => e.Split(new[] {AliasSplitter}, option)[0]).ToArray();
                strGroups = String.Format("group by\r\n\t{0}", String.Join(", ", groupsWithoutAlias));
                strSelect = String.Format("{0}, {1}", strSelect, String.Join(", ", groupsWithAlias));
            }

            if (havingFilter != String.Empty)
            {
                havingFilter = String.Format("\r\n{0}", havingFilter);
            }

            var queryText = String.Format("select\r\n\t{0}\r\nfrom\r\n\t{1}\r\n{2}\r\n{3}{4}",
                                             strSelect,     // 0
                                             strTables,     // 1
                                             strFilters,    // 2
                                             strGroups,     // 3
                                             havingFilter   // 4
                                         );                           
                                 
            return queryText;
        }

        public string GetQueryText(List<QFilter> filterList, List<Enum> groupList)
        {
            return GetQueryText(filterList, groupList, String.Empty);
        }

        public string GetQueryText(List<QFilter> filterList)
        {
            return GetQueryText(filterList, null);
        }
    }


    public class QDirtyUMNS28n : QFactTable
    {
        public enum Keys { Period, Day, KD, Okved, Mark, Okato };

        public QDirtyUMNS28n()
            : base(f_F_DirtyUMNS28n.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum", InfoFact.Prefix, f_F_DirtyUMNS28n.Summe);

            AddSource(Keys.KD, d_KD_A28N.InternalKey);
            AddSource(Keys.Okato, d_OKATO_A28N.InternalKey);
            AddSource(Keys.Okved, d_OKVED_A28N.InternalKey);
            AddSource(Keys.Mark, fx_FX_DataMarks65n.InternalKey);

            AddParam(Keys.KD, f_F_DirtyUMNS28n.RefKDA28N, d_KD_A28N.RefBridgeKDA28N);
            AddParam(Keys.Okato, f_F_DirtyUMNS28n.RefOKATOA28N, d_OKATO_A28N.RefRegionsBridge);
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Mark, f_F_DirtyUMNS28n.RefDataMarks65n, null);
            AddParam(Keys.Okved, f_F_DirtyUMNS28n.RefOKVEDA28N, d_OKVED_A28N.RefBridgeOKVEDA28N);
            AddParam(Keys.Day, f_F_DirtyUMNS28n.RefYearDayUNV, null);
        }
    }

    public class QFNS3Cons : QFactTable
    {
        public enum Keys { Period, KD, Lvl, Day };

        public QFNS3Cons()
            : base(f_D_FNS3Cons.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum0, Sum({0}.{2}) as Sum1",
                                    InfoFact.Prefix,
                                    f_D_FNS3Cons.Earned,
                                    f_D_FNS3Cons.Inpayments);

            AddSource(Keys.KD, d_KD_FNS3.InternalKey);

            AddParam(Keys.KD, f_D_FNS3Cons.RefKD, d_KD_FNS3.RefKDBridge);
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Lvl, f_D_FNS3Cons.RefBudgetLevels, null);
            AddParam(Keys.Day, f_D_FNS3Cons.RefYearDayUNV, null);
        }
    }


    public class QFNS4NMTotal : QFactTable
    {
        public enum Keys { Period, Day, Arrears, RefD };

        public QFNS4NMTotal()
            : base(f_D_FNS4NMTotal.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum", InfoFact.Prefix, f_D_FNS4NMTotal.Value);

            AddSource(Keys.Arrears, d_Arrears_FNS.InternalKey);
            AddSource(Keys.RefD, d_D_GroupFNS.InternalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS4NMTotal.RefYearDayUNV, null);
            AddParam(Keys.Arrears, f_D_FNS4NMTotal.RefArrears, d_Arrears_FNS.RefArrearsFNSBridge);
            AddParam(Keys.RefD, f_D_FNS4NMTotal.RefD, d_D_GroupFNS.RefDGroup);
        }
    }


    public class QMonthRep : QFactTable
    {
        public enum Keys { Period, Day, KD, Lvl, Okato, DocTyp, Means };

        public QMonthRep()
            : base(f_F_MonthRepIncomes.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum, Sum({0}.{2}) as Sum1",
                                    InfoFact.Prefix,
                                    f_F_MonthRepIncomes.Fact,
                                    f_F_MonthRepIncomes.YearPlan);

            AddSource(Keys.KD, d_KD_MonthRep.InternalKey);
            AddSource(Keys.Okato, d_Regions_MonthRep.InternalKey);
            AddSource(Keys.DocTyp, d_Regions_MonthRep.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_F_MonthRepIncomes.RefYearDayUNV, null);
            AddParam(Keys.KD, f_F_MonthRepIncomes.RefKD, d_KD_MonthRep.RefKDBridge);
            AddParam(Keys.Lvl, f_F_MonthRepIncomes.RefBdgtLevels, null);
            AddParam(Keys.Okato, f_F_MonthRepIncomes.RefRegions, d_Regions_MonthRep.RefRegionsBridge);
            AddParam(Keys.DocTyp, f_F_MonthRepIncomes.RefRegions, d_Regions_MonthRep.RefDocType);
            AddParam(Keys.Means, f_F_MonthRepIncomes.RefMeansType, null);
        }
    }

    public class QFNS5MNRegions : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Okato };

        public QFNS5MNRegions()
            : base(f_D_FNS5MNRegions.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum", InfoFact.Prefix, f_D_FNS5MNRegions.ValueReport);

            AddSource(Keys.Okato, d_Regions_FNS.InternalKey);
            AddSource(Keys.MarkBridge, d_Marks_FNS5MN.InternalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5MNRegions.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FNS5MNRegions.RefRegions, d_Regions_FNS.RefBridge);
            AddParam(Keys.Mark, f_D_FNS5MNRegions.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5MNRegions.RefMarks, d_Marks_FNS5MN.RefMarksFNS5MNBridge);
        }
    }

    public class QFNS5NDFLRegions : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Okato };

        public QFNS5NDFLRegions()
            : base(f_D_FNS5NDFLRegions.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}", InfoFact.Prefix, f_D_FNS5NDFLRegions.ValueReport);

            AddSource(Keys.Okato, d_Regions_FNS.InternalKey);
            AddSource(Keys.MarkBridge, d_Marks_FNS5NDFL.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5NDFLRegions.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FNS5NDFLRegions.RefRegions, d_Regions_FNS.RefBridge);
            AddParam(Keys.Mark, f_D_FNS5NDFLRegions.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5NDFLRegions.RefMarks, d_Marks_FNS5NDFL.RefBridge,
                     f_D_FNS5NDFLRegions.RefMarks);
        }
    }

    public class QFNS5NDFLTotal : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge };

        public QFNS5NDFLTotal()
            : base(f_D_FNS5NDFLTotal.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_D_FNS5NDFLTotal.ValueReport,
                                    f_D_FNS5NDFLTotal.TaxpayersNumberReport);

            AddSource(Keys.MarkBridge, d_Marks_FNS5NDFL.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5NDFLTotal.RefYearDayUNV, null);
            AddParam(Keys.Mark, f_D_FNS5NDFLTotal.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5NDFLTotal.RefMarks, d_Marks_FNS5NDFL.RefBridge, f_D_FNS5NDFLTotal.RefMarks);
        }
    }

    public class QFNS5DDKRegions : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Okato, Person, Income };

        public QFNS5DDKRegions()
            : base(f_D_FNS5DDKRegions.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}", InfoFact.Prefix, f_D_FNS5DDKRegions.Value);

            AddSource(Keys.Okato, d_Regions_FNS.InternalKey);
            AddSource(Keys.MarkBridge, d_Marks_FNS5DDK.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5DDKRegions.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FNS5DDKRegions.RefRegions, d_Regions_FNS.RefBridge);
            AddParam(Keys.Mark, f_D_FNS5DDKRegions.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5DDKRegions.RefMarks, d_Marks_FNS5DDK.RefMarksFNS5DDKBridge,
                     f_D_FNS5DDKRegions.RefMarks);
            AddParam(Keys.Person, f_D_FNS5DDKRegions.RefTypes, null);
            AddParam(Keys.Income, f_D_FNS5DDKRegions.RefTypesIncomes, null);
        }
    }

    public class QFNS5DDKTotal : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Person, Income };

        public QFNS5DDKTotal()
            : base(f_D_FNS5DDKTotal.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}", InfoFact.Prefix, f_D_FNS5DDKTotal.Value);

            AddSource(Keys.MarkBridge, d_Marks_FNS5DDK.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5DDKTotal.RefYearDayUNV, null);
            AddParam(Keys.Mark, f_D_FNS5DDKTotal.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5DDKTotal.RefMarks, d_Marks_FNS5DDK.RefMarksFNS5DDKBridge,
                     f_D_FNS5DDKTotal.RefMarks);
            AddParam(Keys.Person, f_D_FNS5DDKTotal.RefTypes, null);
            AddParam(Keys.Income, f_D_FNS5DDKTotal.RefTypesIncomes, null);
        }
    }

    public class QFNS5YSNRegions : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Okato, Person };

        public QFNS5YSNRegions()
            : base(f_D_FNS5YSNRegions.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_D_FNS5YSNRegions.Value,
                                    f_D_FNS5YSNRegions.ValueReport);

            AddSource(Keys.Okato, d_Regions_FNS.InternalKey);
            AddSource(Keys.MarkBridge, d_Marks_FNS5YSN.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5YSNRegions.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FNS5YSNRegions.RefRegions, d_Regions_FNS.RefBridge);
            AddParam(Keys.Mark, f_D_FNS5YSNRegions.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5YSNRegions.RefMarks, d_Marks_FNS5YSN.RefBridge,
                     f_D_FNS5YSNRegions.RefMarks);
            AddParam(Keys.Person, f_D_FNS5YSNRegions.RefTypes, null);
        }
    }

    public class QFNS5YSNTotal : QFactTable
    {
        public enum Keys { Period, Day, Mark, MarkBridge, Okato, Person };

        public QFNS5YSNTotal()
            : base(f_D_FNS5YSNTotal.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_D_FNS5YSNTotal.Value,
                                    f_D_FNS5YSNTotal.ValueReport);

            AddSource(Keys.Okato, d_Regions_FNS.InternalKey);
            AddSource(Keys.MarkBridge, d_Marks_FNS5YSN.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FNS5YSNTotal.RefYearDayUNV, null);
            AddParam(Keys.Mark, f_D_FNS5YSNTotal.RefMarks, null);
            AddParam(Keys.MarkBridge, f_D_FNS5YSNTotal.RefMarks, d_Marks_FNS5YSN.RefBridge,
                     f_D_FNS5YSNTotal.RefMarks);
            AddParam(Keys.Person, f_D_FNS5YSNTotal.RefTypes, null);
        }
    }

    public class QUFK14 : QFactTable
    {
        public enum Keys { Period, Day, DayFK, Okato, KD, KVSR, Lvl, Org, Struc, Oper };

        public QUFK14()
            : base(f_D_UFK14.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_D_UFK14.Credit,
                                    f_D_UFK14.Debit);
            
            AddSource(Keys.KD, d_KD_UFK.InternalKey);
            AddSource(Keys.KVSR, d_KD_UFK.InternalKey);
            AddSource(Keys.Okato, d_OKATO_UFK.InternalKey);
            AddSource(Keys.Org, d_Org_UFKPayers.InternalKey);
            AddSource(Keys.Struc, d_Org_UFKPayers.InternalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_UFK14.RefYearDayUNV, null);
            AddParam(Keys.DayFK, f_D_UFK14.RefFKDay, null);
            AddParam(Keys.Okato, f_D_UFK14.RefOKATO, d_OKATO_UFK.RefRegionsBridge);
            AddParam(Keys.KD, f_D_UFK14.RefKD, d_KD_UFK.RefKDBridge);
            AddParam(Keys.KVSR, f_D_UFK14.RefKD, d_KD_UFK.RefKVSRBridge);
            AddParam(Keys.Lvl, f_D_UFK14.RefFX, null);
            AddParam(Keys.Org, f_D_UFK14.RefOrg, d_Org_UFKPayers.RefOrgPayersBridge);
            AddParam(Keys.Struc, f_D_UFK14.RefOrg, d_Org_UFKPayers.RefFX);
            AddParam(Keys.Oper, f_D_UFK14.RefOpertnTypes, null);
        }
    }

    public class QUFK14dirty : QFactTable
    {
        public enum Keys { Period, Day, DayFK, Okato, KD, KVSR, Org, Struc };

        public QUFK14dirty()
            : base(f_D_UFK14dirty.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}, Sum({0}.{2}) as {2}",
                                    InfoFact.Prefix,
                                    f_D_UFK14dirty.Credit,
                                    f_D_UFK14dirty.Debit);

            AddSource(Keys.KD, d_KD_UFK.InternalKey);
            AddSource(Keys.KVSR, d_KD_UFK.InternalKey);
            AddSource(Keys.Okato, d_OKATO_UFK.InternalKey);
            AddSource(Keys.Org, d_Org_UFKPayers.InternalKey);
            AddSource(Keys.Struc, d_Org_UFKPayers.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_UFK14dirty.RefYearDayUNV, null);
            AddParam(Keys.DayFK, f_D_UFK14dirty.RefFKDay, null);
            AddParam(Keys.Okato, f_D_UFK14dirty.RefOKATO, d_OKATO_UFK.RefRegionsBridge);
            AddParam(Keys.KD, f_D_UFK14dirty.RefKD, d_KD_UFK.RefKDBridge);
            AddParam(Keys.KVSR, f_D_UFK14dirty.RefKD, d_KD_UFK.RefKVSRBridge);
            AddParam(Keys.Org, f_D_UFK14dirty.RefOrg, d_Org_UFKPayers.RefOrgPayersBridge);
            AddParam(Keys.Struc, f_D_UFK14dirty.RefOrg, d_Org_UFKPayers.RefFX);
        }
    }

    public class QdUFK22 : QFactTable
    {
        public enum Keys { Period, Day, Okato, KD, KVSR, Marks };

        public QdUFK22()
            : base(f_D_UFK22.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as Sum, Sum({0}.{2}) as Sum1",
                                    InfoFact.Prefix,
                                    f_D_UFK22.ForPeriod,
                                    f_D_UFK22.FromBeginYear);

            AddSource(Keys.KD, d_KD_UFK.InternalKey);
            AddSource(Keys.KVSR, d_KD_UFK.InternalKey);
            AddSource(Keys.Okato, d_OKATO_UFK.InternalKey);

            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_UFK22.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_UFK22.RefOKATO, d_OKATO_UFK.RefRegionsBridge);
            AddParam(Keys.KD, f_D_UFK22.RefKD, d_KD_UFK.RefKDBridge);
            AddParam(Keys.KVSR, f_D_UFK22.RefKD, d_KD_UFK.RefKVSRBridge);
            AddParam(Keys.Marks, f_D_UFK22.RefMarks, null);
        }
    }

    public class QWorkingDays : QFactTable
    {
        public enum Keys { Period };

        public QWorkingDays()
            : base(d_Date_WorkingDays.InternalKey)
        {
            Result = String.Format("{0}.{1}", InfoFact.Prefix, d_Date_WorkingDays.DaysWorking);
            AddParam(Keys.Period, null, null);
        }
    }

    public class QdFOYRIncomes : QFactTable
    {
        public enum Keys { Period, Day, Okato, KD, Lvl, Means, DocTyp };

        public QdFOYRIncomes()
            : base(f_D_FOYRIncomes.InternalKey)
        {
            Result = String.Format("Sum({0}.{1}) as {1}",
                                    InfoFact.Prefix,
                                    f_D_FOYRIncomes.Performed);

            AddSource(Keys.KD, d_KD_FOYR.InternalKey);
            AddSource(Keys.Okato, d_Regions_FOYR.InternalKey);
            AddSource(Keys.DocTyp, d_Regions_FOYR.InternalKey);
            
            AddParam(Keys.Period, null, null);
            AddParam(Keys.Day, f_D_FOYRIncomes.RefYearDayUNV, null);
            AddParam(Keys.Okato, f_D_FOYRIncomes.RefRegions, d_Regions_FOYR.RefRegionsBridge);
            AddParam(Keys.Lvl, f_D_FOYRIncomes.RefBdgtLevels, null);
            AddParam(Keys.KD, f_D_FOYRIncomes.RefKD, d_KD_FOYR.RefKDBridge);
            AddParam(Keys.Means, f_D_FOYRIncomes.RefMeansType, null);
            AddParam(Keys.DocTyp, f_D_FOYRIncomes.RefRegions, d_Regions_FOYR.RefDocType);
        }
    }
}