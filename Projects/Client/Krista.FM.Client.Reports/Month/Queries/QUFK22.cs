using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.UFK;

namespace Krista.FM.Client.Reports.Month.Queries
{
    class QUFK22Params
    {
        public string Period {get; set; }
        public string KD { get; set; }
        public string KVSR { get; set; }
        public string Region { get; set; }
        public string Mark { get; set; }

        public QUFK22Params()
        {
            Period = String.Empty;
            KD = String.Empty;
            KVSR = String.Empty;
            Region = String.Empty;
            Mark = String.Empty;
        }
    }

    public enum QUFK22Group
    {
        Region,
        Kd,
        Mark,
        Period
    }

    class QUFK22
    {
        public static string GroupBy(QUFK22Params qParams, List<QUFK22Group> groupList)
        {
            const string sptGroups = ", ";

            var infoFact = new GroupHelper()
                               {
                                   Prefix = QFilterHelper.fltPrefixName,
                                   EntityKey = f_D_UFK22.InternalKey
                               };

            var infoKD = new GroupHelper
            {
                Prefix = "kd",
                EntityKey = d_KD_UFK.InternalKey
            };

            var infoOkato = new GroupHelper
            {
                Prefix = "okt",
                EntityKey = d_OKATO_UFK.InternalKey
            };

            var fltKVSR = String.Empty;
            if (qParams.KVSR.Length != 0)
            {
                fltKVSR = String.Format(
                    " and {2}{0} in ({1})", 
                    d_KD_UFK.RefKVSRBridge,
                    qParams.KVSR, 
                    infoKD.FullPrefix);
            }

            var fltRegion = String.Empty;
            if (qParams.Region.Length != 0)
            {
                fltRegion = String.Format(
                    " and {2}{0} in ({1})",
                    d_OKATO_UFK.RefRegionsBridge,
                    qParams.Region,
                    infoOkato.FullPrefix);
            }

            var strGroup = String.Empty;

            foreach (var group in groupList)
            {
                var tblPrefix = QFilterHelper.fltPrefix;
                var tblFieldName = String.Empty;

                switch (group)
                {
                    case QUFK22Group.Kd :
                        tblPrefix = infoKD.FullPrefix;
                        tblFieldName = d_KD_UFK.RefKDBridge;
                        break;
                    case QUFK22Group.Period:
                        tblFieldName = f_D_UFK22.RefYearDayUNV;
                        break;
                    case QUFK22Group.Mark:
                        tblFieldName = f_D_UFK22.RefMarks;
                        break;
                    case QUFK22Group.Region:
                        tblPrefix = infoOkato.FullPrefix;
                        tblFieldName = d_OKATO_UFK.RefRegionsBridge;
                        break;
                }

                strGroup = String.Join(sptGroups, new[] { strGroup, String.Format("{0}{1}", tblPrefix, tblFieldName) });
            }

            strGroup = ReportDataServer.Trim(strGroup, sptGroups);

            var queryText =
                String.Format(
                    @"
                    select 
                        Sum({31}.{1}) as {1}, Sum({31}.{2}) as {2}, {0}
                    from 
                        {16} {31}, {17} {32}, {18} {33}
                    where 
                        {31}.{4} in ({23}) and 
                        ({21}) and 
                        {31}.{6} = {33}.id and 
                        {31}.{3} = {32}.id and
                        {32}.{5} in ({22})
                        {24}
                        {25}
                    group by 
                        {0}",
                    strGroup, // 0
                    f_D_UFK22.ForPeriod, // 1
                    f_D_UFK22.FromBeginYear, // 2
                    f_D_UFK22.RefKD, // 3
                    f_D_UFK22.RefMarks, // 4
                    d_KD_UFK.RefKDBridge, // 5
                    f_D_UFK22.RefOKATO, // 6
                    d_OKATO_UFK.RefRegionsBridge, // 7
                    f_D_UFK22.RefYearDayUNV, // 8
                    String.Empty, // 09
                    String.Empty, // 10
                    String.Empty, // 11
                    String.Empty, // 12
                    String.Empty, // 13
                    String.Empty, // 14
                    String.Empty, // 15
                    infoFact.Entity.FullDBName, // 16
                    infoKD.Entity.FullDBName, // 17
                    infoOkato.Entity.FullDBName, // 18
                    String.Empty, // 19
                    String.Empty, // 20
                    qParams.Period, // 21
                    qParams.KD, // 22
                    qParams.Mark, // 23
                    fltKVSR, // 24
                    fltRegion, // 25
                    String.Empty, // 26
                    String.Empty, // 27
                    String.Empty, // 28
                    String.Empty, // 29
                    String.Empty, // 30
                    infoFact.Prefix, // 31
                    infoKD.Prefix, // 32
                    infoOkato.Prefix // 33
                    );

            return queryText;            
        }
    }
}
