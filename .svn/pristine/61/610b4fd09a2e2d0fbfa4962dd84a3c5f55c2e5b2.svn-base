using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;

namespace Krista.FM.Client.Reports.UFK.ReportQueries
{
    public class QUFK14Params
    {
        public string Period {get; set; }
        public string KD { get; set; }
        public string Mark { get; set; }
        public string KVSR { get; set; }
        public string Region { get; set; }
        public string Lvl { get; set; }
        public string KIF { get; set; }
        public string OrganizationPayers { get; set; }

        public QUFK14Params()
        {
            Period = String.Empty;
            KD = String.Empty;
            Mark = String.Empty;
            KVSR = String.Empty;
            Region = String.Empty;
            Lvl = String.Empty;
            KIF = String.Empty;
            OrganizationPayers = String.Empty;
        }
    }

    public enum QUFK14Group
    {
        Region,
        Kd,
        Period,
        Lvl,
        Payer
    }

    public class QUFK14
    {
        public static string GroupBy(QUFK14Params qParams, List<QUFK14Group> groupList)
        {
            const string sptGroups = ", ";
            const string templateFilter = " and {2}.{0} in ({1})";

            var infoFact = new GroupHelper()
            {
                Prefix = QFilterHelper.fltPrefixName,
                EntityKey = f_D_UFK14.InternalKey
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

            var infoOrg = new GroupHelper
            {
                Prefix = "org",
                EntityKey = d_Org_UFKPayers.InternalKey
            };

            var fltKVSR = String.Empty;
            if (qParams.KVSR.Length != 0)
            {
                fltKVSR = String.Format(templateFilter, d_KD_UFK.RefKVSRBridge, qParams.KVSR, infoKD.Prefix);
            }

            var fltRegion = String.Empty;
            if (qParams.Region.Length != 0)
            {
                fltRegion = String.Format(templateFilter, d_OKATO_UFK.RefRegionsBridge, qParams.Region, infoOkato.Prefix);
            }

            var fltLvl = String.Empty;
            if (qParams.Lvl.Length != 0)
            {
                fltLvl = String.Format(templateFilter, f_D_UFK14.RefFX, qParams.Lvl, infoFact.Prefix);
            }

            var fltKIF = String.Empty;
            if (qParams.KIF.Length != 0)
            {
                fltKIF = String.Format(templateFilter, d_KD_UFK.RefKIFBridge, qParams.KIF, infoKD.Prefix);
            }

            var fltOrg = String.Empty;
            if (qParams.OrganizationPayers.Length != 0)
            {
                fltOrg = String.Format(templateFilter, d_Org_UFKPayers.RefOrgPayersBridge, qParams.OrganizationPayers, infoOrg.Prefix);
            }

            var fltKBK = String.Empty;
            if (qParams.KD.Length != 0)
            {
                fltKBK = String.Format(templateFilter, d_KD_UFK.RefKDBridge, qParams.KD, infoKD.Prefix);
            }

            var strGroup = String.Empty;

            foreach (var group in groupList)
            {
                var tblPrefix = QFilterHelper.fltPrefix;
                var tblFieldName = String.Empty;

                switch (group)
                {
                    case QUFK14Group.Kd:
                        tblPrefix = infoKD.FullPrefix;
                        tblFieldName = d_KD_UFK.RefKDBridge;
                        break;
                    case QUFK14Group.Period:
                        tblFieldName = f_D_UFK14.RefYearDayUNV;
                        break;
                    case QUFK14Group.Region:
                        tblPrefix = infoOkato.FullPrefix;
                        tblFieldName = d_OKATO_UFK.RefRegionsBridge;
                        break;
                    case QUFK14Group.Lvl:
                        tblFieldName = f_D_UFK14.RefFX;
                        break;
                    case QUFK14Group.Payer:
                        tblPrefix = infoOrg.FullPrefix;
                        tblFieldName = d_Org_UFKPayers.RefOrgPayersBridge;
                        break;
                }

                strGroup = String.Join(sptGroups, new[] { strGroup, String.Format("{0}{1}", tblPrefix, tblFieldName) });
            }

            strGroup = ReportDataServer.Trim(strGroup, sptGroups);

            var queryText =
                String.Format(
                    @"
                    select 
                        Sum({31}.{1}) as {1}, {0}
                    from 
                        {16} {31}, {17} {32}, {18} {33}, {19} {34}
                    where 
                        {31}.{1} <> 0 and
                        ({21}) and 
                        {31}.{9} = {34}.id and
                        {31}.{6} = {33}.id and 
                        {31}.{3} = {32}.id
                        {22}
                        {24}
                        {25}
                        {23}
                        {26}
                        {27}
                    group by 
                        {0}",
                    strGroup, // 0
                    f_D_UFK14.Credit, // 1
                    String.Empty, // 2
                    f_D_UFK14.RefKD, // 3
                    String.Empty, // 4
                    d_KD_UFK.RefKDBridge, // 5
                    f_D_UFK14.RefOKATO, // 6
                    d_OKATO_UFK.RefRegionsBridge, // 7
                    f_D_UFK14.RefYearDayUNV, // 8
                    f_D_UFK14.RefOrg, // 09
                    String.Empty, // 10
                    String.Empty, // 11
                    String.Empty, // 12
                    String.Empty, // 13
                    String.Empty, // 14
                    String.Empty, // 15
                    infoFact.Entity.FullDBName, // 16
                    infoKD.Entity.FullDBName, // 17
                    infoOkato.Entity.FullDBName, // 18
                    infoOrg.Entity.FullDBName, // 19
                    String.Empty, // 20
                    qParams.Period, // 21
                    fltKBK, // 22
                    fltLvl, // 23
                    fltKVSR, // 24
                    fltRegion, // 25
                    fltKIF, // 26
                    fltOrg, // 27
                    String.Empty, // 28
                    String.Empty, // 29
                    String.Empty, // 30
                    infoFact.Prefix, // 31
                    infoKD.Prefix, // 32
                    infoOkato.Prefix, // 33
                    infoOrg.Prefix // 34
                    );

            return queryText; 
        }
    }
}