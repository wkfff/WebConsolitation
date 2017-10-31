using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.UFK;

namespace Krista.FM.Client.Reports.Month.Queries
{
    class QFOYRParams
    {
        public string Period { get; set; }
        public string KD { get; set; }
        public string Lvl { get; set; }
        public string DocType { get; set; }

        public QFOYRParams()
        {
            Period = String.Empty;
            KD = String.Empty;
            Lvl = String.Empty;
            DocType = String.Empty;
        }
    }

    public enum QFOYRGroup
    {
        Region,
        Kd,
        Period,
        Lvl,
        DocType
    }

    class QFOYRIncomes
    {
        public static string GroupBy(QFOYRParams qParams, List<QFOYRGroup> groupList)
        {
            const string templateFilter = " and {2}.{0} in ({1})";
            const string sptGroups = ", ";
            var fltKBK = String.Empty;
            var fltDocType = String.Empty;

            var infoFact = new GroupHelper()
            {
                Prefix = QFilterHelper.fltPrefixName,
                EntityKey = f_D_FOYRIncomes.InternalKey
            };

            var infoKD = new GroupHelper
            {
                Prefix = "kd",
                EntityKey = d_KD_FOYR.InternalKey
            };

            var infoRgn = new GroupHelper
            {
                Prefix = "rgn",
                EntityKey = d_Regions_FOYR.InternalKey
            };

            if (qParams.KD.Length != 0)
            {
                fltKBK = String.Format(templateFilter, d_KD_FOYR.RefKDBridge, qParams.KD, infoKD.Prefix);
            }

            if (qParams.DocType.Length != 0)
            {
                fltDocType = String.Format(templateFilter, d_Regions_FOYR.RefDocType, qParams.DocType, infoRgn.Prefix);
            }

            var strGroup = String.Empty;

            foreach (var group in groupList)
            {
                var tblPrefix = QFilterHelper.fltPrefix;
                var tblFieldName = String.Empty;

                switch (group)
                {
                    case QFOYRGroup.Kd:
                        tblPrefix = infoKD.FullPrefix;
                        tblFieldName = d_KD_FOYR.RefKDBridge;
                        break;
                    case QFOYRGroup.Period:
                        tblFieldName = f_D_FOYRIncomes.RefYearDayUNV;
                        break;
                    case QFOYRGroup.Lvl:
                        tblFieldName = f_D_FOYRIncomes.RefBdgtLevels;
                        break;
                    case QFOYRGroup.Region:
                        tblPrefix = infoRgn.FullPrefix;
                        tblFieldName = d_Regions_FOYR.RefRegionsBridge;
                        break;
                    case QFOYRGroup.DocType:
                        tblPrefix = infoRgn.FullPrefix;
                        tblFieldName = d_Regions_FOYR.RefDocType;
                        break;
                }

                strGroup = String.Join(sptGroups, new[] { strGroup, String.Format("{0}{1}", tblPrefix, tblFieldName) });
            }

            strGroup = ReportDataServer.Trim(strGroup, sptGroups);

            var queryText =
                String.Format(
                    @"
                    select 
                        {0},
                        Sum({31}.{1}) as {1}, 
                        Sum({31}.{8}) as {8}
                    from 
                        {16} {31}, {17} {32}, {18} {33}
                    where 
                        {31}.{5} = 1 and
                        {31}.{2} in ({23}) and 
                        ({21}) and 
                        {31}.{4} = {33}.id and 
                        {31}.{6} = {32}.id
                        {22}
                        {24}
                    group by 
                        {0}",
                    strGroup, // 0
                    f_D_FOYRIncomes.Assigned, // 1
                    f_D_FOYRIncomes.RefBdgtLevels, // 2
                    f_D_FOYRIncomes.RefYearDayUNV, // 3
                    f_D_FOYRIncomes.RefRegions, // 4
                    f_D_FOYRIncomes.RefMeansType, // 5
                    f_D_FOYRIncomes.RefKD, // 6
                    d_KD_FOYR.RefKDBridge, // 7
                    f_D_FOYRIncomes.Performed, // 8
                    d_Regions_FOYR.RefRegionsBridge, // 9
                    String.Empty, // 10
                    String.Empty, // 11
                    String.Empty, // 12
                    String.Empty, // 13
                    String.Empty, // 14
                    String.Empty, // 15
                    infoFact.Entity.FullDBName, // 16
                    infoKD.Entity.FullDBName, // 17
                    infoRgn.Entity.FullDBName, // 18
                    String.Empty, // 19
                    String.Empty, // 20
                    qParams.Period, // 21
                    fltKBK, // 22
                    qParams.Lvl, // 23
                    fltDocType, // 24
                    String.Empty, // 25
                    String.Empty, // 26
                    String.Empty, // 27
                    String.Empty, // 28
                    String.Empty, // 29
                    String.Empty, // 30
                    infoFact.Prefix, // 31
                    infoKD.Prefix, // 32
                    infoRgn.Prefix // 33
                    );

            return queryText;
        }
    }

}
