using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // ������ ������� ������� ������� ������� xml

    /// <summary>
    /// ��_0005_��������� ������.
    /// ������� ������ ����
    /// </summary>
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {
        #region ������� ������� �������� �������

        /// <summary>
        /// ���������� ������ ��������������� ����� ������ �� �������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpIncomesClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year >= 2005)
            {
                PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, new int[] { 1 },
                    dsKD.Tables[0], clsKD, kdCache);
            }
            else
            {
                PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, new int[] { 1 },
                    dsKD.Tables[0], clsKD, kdCache);
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ��������� �������������� �� �������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpSrcFinClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year >= 2005)
            {
                PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, new int[] { 3 },
                    dsKIF2005.Tables[0], clsKIF2005, kifCache);
            }
            else
            {
                PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, new int[] { 3 },
                    dsKIF2004.Tables[0], clsKIF2004, kifCache);
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ������� �� �������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpOutcomesClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year >= 2005)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsKVR.Tables[0], clsKVR, new string[] { "CODE", "14..16" }, true, kvrCache,
                    new string[] { "!*000", "-1", "000", "14..16" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsKCSR.Tables[0], clsKCSR, new string[] { "CODE", "7..13" }, true, kcsrCache,
                    new string[] { "!*000000", "-1", "0000000", "7..13" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsFKR.Tables[0], clsFKR, new string[] { "CODE", "0..4" }, true, fkrCache,
                    new string[] { "!*0000000000000000", "-1", "0000", "0..4" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsEKR.Tables[0], clsEKR, new string[] { "CODE", "17..18" }, true, ekrCache,
                    new string[] { "00", "17..18" }, null, ClsProcessModifier.CacheSubCode);
            }
            else
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsKVR.Tables[0], clsKVR, new string[] { "CODE", "10..12" }, true, kvrCache,
                    new string[] { "!*000000", "-1", "000", "10..12" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsKCSR.Tables[0], clsKCSR, new string[] { "CODE", "7..9:0000000" }, true, kcsrCache,
                    new string[] { "!*000000000", "-1", "000", "7..9" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsFKR.Tables[0], clsFKR, new string[] { "CODE", "0..4" }, true, fkrCache, new string[] {
                        "!*000000000000000", "-1", "0000", "0..4", "7980000000000000000", "-1" }, null, ClsProcessModifier.CacheSubCode);

                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form623 }, "4..26",
                    dsEKR.Tables[0], clsEKR, new string[] { "CODE", "13..18" }, true, ekrCache,
                    new string[] { "000000", "13..18" }, null, ClsProcessModifier.CacheSubCode);
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ���� ����� ���������� �� �������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpNetClsFromExtPatternXML(XmlDocument xdPattern)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form625 }, new int[] { 1 },
                dsMarksNet.Tables[0], clsMarksNet, new string[] {
                    "FKR", "0..4", "KCSR", "7..9", "SUBKCSR", "10..10", "KVR", "11..13", "SUBKVR", "14..15", "KNEC", "16..18" },
                false, marksNetCache, null, null, ClsProcessModifier.Standard);
        }

        /// <summary>
        /// ���������� ������ ������� �������
        /// </summary>
        /// <param name="xdReport">������</param>
        protected override void PumpExternalXMLPattern(XmlDocument xdPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    throw new Exception("������� �� �������� ������� � ������� Skif3 �� ��������������");
            }

            // ������
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromExtPatternXML(xdPattern);

            // ��������� ��������������
            if (ToPumpBlock(Block.bFinSources))
                PumpSrcFinClsFromExtPatternXML(xdPattern);

            // �������
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromExtPatternXML(xdPattern);

            // ���� ����� ����������
            if (ToPumpBlock(Block.bNet))
                PumpNetClsFromExtPatternXML(xdPattern);
        }

        #endregion ������� ������� �������� �������

        #region ������� ������� ����������� �������

        /// <summary>
        /// ���������� ������ ��������������� ����� ������ �� ����������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpIncomesClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623, XmlForm.Form428g, XmlForm.Form428Vg },
                        new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "�����" },
                        false, kdCache, null, null, ClsProcessModifier.Standard);
                    // ����� 127, 128
                    PumpComplexClsFromInternalPatternXML(xnPattern,
                        new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v, XmlForm.Form128, XmlForm.Form128v },
                        new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "��" },
                        false, kdCache, null, null, ClsProcessModifier.Standard);
                    // ����� 117
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form117 }, new int[] { 1 },
                        dsKD.Tables[0], clsKD, new string[] { "CODESTR", "��" }, false, kdCache, null, null, ClsProcessModifier.Standard);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "��" },
                            false, kdCache, null, null, ClsProcessModifier.Standard);
                    }
                    else
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "��" },
                            false, kdCache, null, null, ClsProcessModifier.Standard);
                    }
                    break;
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ��������� �������������� �� ����������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpSrcFinClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623, XmlForm.Form428g, XmlForm.Form428Vg },
                        new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005,
                        new string[] { "CODESTR", "�������;������;���������+�����" }, false, kifCache,
                        null, null, ClsProcessModifier.Standard);
                    // ����� 127, 128
                    PumpComplexClsFromInternalPatternXML(xnPattern,
                        new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v, XmlForm.Form128, XmlForm.Form128v },
                        new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "�����" },
                        false, kifCache, null, null, ClsProcessModifier.Standard);
                    // ����� 117
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form117 }, new int[] { 3 },
                        dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "����;�����" }, false, kifCache, null, null, ClsProcessModifier.Standard);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005,
                            new string[] { "CODESTR", "�������;������;���������+�����" }, false, kifCache,
                            null, null, ClsProcessModifier.Standard);
                    }
                    else
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 3 }, dsKIF2004.Tables[0], clsKIF2004,
                            new string[] { "CODESTR", "�������;������;���������+�����" }, false, kifCache,
                            null, null, ClsProcessModifier.Standard);
                    }
                    break;
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ������� �� ����������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpOutcomesClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKVR.Tables[0], clsKVR,
                        new string[] { "��", "���������������" },
                        new string[] { "��", "CODE;-1", "���������������", "CODE;14..16" }, kvrCache,
                        new string[] { "���������������", "!*000;-1", "���������������", "000;14..16", "��", "000;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKCSR.Tables[0], clsKCSR,
                        new string[] { "���", "���������������" },
                        new string[] { "���", "CODE;-1", "���������������", "CODE;7..13" }, kcsrCache,
                        new string[] { "���������������", "!*000000;-1", "���������������", "0000000;7..13", "���", "0000000;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR,
                        new string[] { "����", "���������������" },
                        new string[] { "����", "CODE;-1", "���������������", "CODE;3..6" }, fkrCache,
                        new string[] { "���������������", "!*0000000000000;-1", "���������������", "0000;3..6",
                            "���������������", "7900;3..6", "����", "0000;-1", "����", "7900;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsEKR.Tables[0], clsEKR,
                        new string[] { "���", "���������������" },
                        new string[] { "���", "CODE;-1", "���������������", "CODE;17..19" }, ekrCache,
                        new string[] { "���������������", "000;17..19", "���", "000;-1", "���", "790;-1" });
                    // ����� 127 � 117
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKVR.Tables[0], clsKVR, new string[] { "��" },
                        new string[] { "��", "CODE;-1" }, kvrCache, new string[] { });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKCSR.Tables[0], clsKCSR, new string[] { "���" },
                        new string[] { "���", "CODE;-1" }, kcsrCache, new string[] { });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR, new string[] { "����" },
                        new string[] { "����", "CODE;-1" }, fkrCache, new string[] { "����", "7900;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsEKR.Tables[0], clsEKR, new string[] { "���" },
                        new string[] { "���", "CODE;-1" }, ekrCache, new string[] { "���", "790;-1" });

                    // kvsr
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKvsr.Tables[0], clsKvsr, new string[] { "���" },
                        new string[] { "���", "CODE;-1" }, kvsrCache, new string[] { });

                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpComplexClsFromInternalPatternXML(dsKVR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsKVR, new string[] { "CODE", "14..16" }, true, kvrCache,
                            new string[] { "!*000", "-1", "000", "14..16" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsKCSR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsKCSR, new string[] { "CODE", "7..13" }, true, kcsrCache,
                            new string[] { "!*000000", "-1", "0000000", "7..13" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsFKR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsFKR, new string[] { "CODE", "0..4" }, true, fkrCache,
                            new string[] { "!*0000000000000000", "-1", "0000", "0..4" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsEKR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsEKR, new string[] { "CODE", "17..19" }, true, ekrCache,
                            new string[] { "000", "17..19" }, null, ClsProcessModifier.CacheSubCode);
                    }
                    else
                    {
                        PumpComplexClsFromInternalPatternXML(dsKVR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsKVR, new string[] { "CODE", "10..12" }, true, kvrCache,
                            new string[] { "!*000000", "-1", "000", "10..12" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsKCSR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsKCSR, new string[] { "CODE", "7..9:0000000" }, true, kcsrCache,
                            new string[] { "!*000000000", "-1", "000", "7..9" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsFKR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsFKR, new string[] { "CODE", "0..4" }, true, fkrCache,
                            new string[] { "!*000000000000000", "-1", "0000", "0..4" }, null, ClsProcessModifier.CacheSubCode);

                        PumpComplexClsFromInternalPatternXML(dsEKR.Tables[0], xnPattern, new XmlForm[] { XmlForm.Form623 },
                            "4..26", clsEKR, new string[] { "CODE", "13..18" }, true, ekrCache,
                            new string[] { "000000", "13..18" }, null, ClsProcessModifier.CacheSubCode);
                    }
                    break;
            }
        }

        /// <summary>
        /// ���������� ������ ��������������� ����� ���� ����� ���������� �� ����������� �������
        /// </summary>
        /// <param name="xnPattern">������</param>
        private void PumpNetClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsMarksNet.Tables[0], clsMarksNet, new string[] { "�����������" },
                        new string[] { "�����������", "CODE;-1..3" }, marksNetCache, new string[] { }, false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// ���������� ������ ������ �������
        /// </summary>
        /// <param name="xdReport">������</param>
        protected override void PumpInternalXMLPattern(XmlNode xnPattern)
        {
            // ������
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromIntPatternXML(xnPattern);
            // ��������� ��������������
            if (ToPumpBlock(Block.bFinSources))
                PumpSrcFinClsFromIntPatternXML(xnPattern);
            // �������
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromIntPatternXML(xnPattern);
            // ���� ����� ����������
            if (ToPumpBlock(Block.bNet))
                PumpNetClsFromIntPatternXML(xnPattern);
        }

        #endregion ������� ������� ����������� �������

        #region ������� ����� ����������� ������� ������

        /// <summary>
        /// ���������� ���� "������� ��������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepDefProfXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"���������������\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "623", "26" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!7980", "-1" },
                        BlockProcessModifier.YRDefProf, "��+��", regionCache, nullRegions, null, region4PumpCache);
                    break;

                case XmlFormat.Skif3:
                    // ����� 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "127;127g;127v", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "������������", regionCache, nullRegions, null, region4PumpCache);
                    //  ����� 428
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "428g;428Vg", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00079000000000000000", "-1" },
                        BlockProcessModifier.YRDefProf, "���������������", regionCache, nullRegions, null, region4PumpCache);
                    //  ����� 128
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "128;128V", "22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { },
                        BlockProcessModifier.YRDefProf, "���+����+���+��+���", regionCache, nullRegions, null, region4PumpCache);
                    //  ����� 117
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "117", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "������������", regionCache, nullRegions, null, region4PumpCache);
                    // ����� 127
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "127;127g;127v", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "������������", regionCache, nullRegions, null, region4PumpCache);
                    break;
                default:
                    if (this.DataSource.Year <= 2004)
                    {
                        PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "623", "26" },
                            daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                            null, null, null, null, null, progressMsg, null, new string[] { "!7980000000000000000", "-1" },
                            BlockProcessModifier.YRDefProf, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    }
                    else
                    {
                        PumpXMLReportBlock("���� \"���������������\"", xnReport, new string[] { "623", "26" },
                            daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                            null, null, null, null, null, progressMsg, null, new string[] { "!79800000000000000000", "-1" },
                            BlockProcessModifier.YRDefProf, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    }
                    break;
            }

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRDefProf, ref dsFOYRDefProf);

            WriteToTrace("������� ���� \"���������������\" ���������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ���� "������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepIncomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"������\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // ����� 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("���� \"������\"", xnReport, new string[] { "127;127g;127v", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 428, 128
                    PumpXMLReportBlock("���� \"������\"", xnReport, new string[] { "428g;428Vg;128;128V", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 117
                    PumpXMLReportBlock("���� \"������\"", xnReport, new string[] { "117", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 127 ������ ���
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("���� \"������\"", xnReport, new string[] { "127;127g;127v", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    break;
                default:
                    PumpXMLReportBlock("���� \"������\"", xnReport, new string[] { "623", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0] },
                        new IClassifier[] { clsKD },
                        new int[] { },
                        new string[] { "REFKD" },
                        new int[] { nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    break;
            }

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRIncomes, ref dsFOYRIncomes);

            WriteToTrace("������� ���� \"������\" ���������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ���� "��������� ��������������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepSrcFinXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"��������� ��������������\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // ����� 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("���� \"��������� ��������������\"", xnReport, new string[] { "127;127g;127v", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 428, 128
                    PumpXMLReportBlock("���� \"��������� ��������������\"", xnReport, new string[] { "428g;428Vg;128;128V", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 117
                    PumpXMLReportBlock("���� \"��������� ��������������\"", xnReport, new string[] { "117", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // ����� 127 - ������ ���
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("���� \"��������� ��������������\"", xnReport, new string[] { "127;127g;127v", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    break;
                default:
                    PumpXMLReportBlock("���� \"��������� ��������������\"", xnReport, new string[] { "428g;428Vg", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    break;
            }

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRSrcFin, ref dsFOYRSrcFin);

            WriteToTrace("������� ���� \"��������� ��������������\" ���������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ���� "��������� �������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepEmbezzlesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"��������� �������\".", TraceMessageKind.Information);

            PumpXMLReportBlock("���� \"��������� �������\"", xnReport, new string[] { "630", "3" },
                daFOYREmbezzles, dsFOYREmbezzles.Tables[0], fctFOYREmbezzles,
                new DataTable[] { dsMarksEmbezzles.Tables[0], dsMeansType.Tables[0] },
                new IClassifier[] { fxcMarksEmbezzles, fxcMeansType },
                new int[] { 2005 },
                new string[] { "REFMARKS", "REFMEANSTYPE" },
                new int[] { 0, 0 },
                progressMsg,
                new Dictionary<string, int>[] { marksEmbezzlesCache, meansTypeCache },
                BlockProcessModifier.YREmbezzles, string.Empty, regionCache, nullRegions, null, region4PumpCache);

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYREmbezzles, ref dsFOYREmbezzles);

            WriteToTrace("������� ���� \"��������� �������\" ���������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ���� "�������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepOutcomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"�������\".", TraceMessageKind.Information);
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // ����� 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "127;127g;127v", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "���������������;���+����+���+��+���", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // ����� 428
                    PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "428g;428Vg", "12;22" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "���������������;���+����+���+��+���", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // ����� 128
                    PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "128;128V", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "���������������;���+����+���+��+���", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // ����� 117
                    PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "117", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "���������������;���+����+���+��+���", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // ����� 127 - ������ ���
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "127;127g;127v", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "���������������;���+����+���+��+���", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "623", "4..26" },
                            daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                            new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0] },
                            new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR },
                            new int[] { 2005 },
                            new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR" },
                            new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR },
                            progressMsg,
                            new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache },
                            new string[] { "#!9600;#!9800", "0..4", "00", "2..3", "000", "17..19", "0000000", "7..13", "000", "14..16" },
                            BlockProcessModifier.YROutcomes, string.Empty, regionCache, nullRegions,
                            new string[] { "14..16", "7..13", "0..4", "0..0", "17..19" }, region4PumpCache);
                    }
                    else
                    {
                        PumpXMLReportBlock("���� \"�������\"", xnReport, new string[] { "623", "4..26" },
                            daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                            new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsEKR.Tables[0], null },
                            new IClassifier[] { clsKVR, clsKCSR, clsFKR, clsEKR },
                            new int[] { 2004 },
                            new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR" },
                            new int[] { nullKVR, nullKCSR, nullFKR, nullEKR },
                            progressMsg,
                            new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, ekrCache },
                            new string[] { "#!9600;#!9800", "0..4", "00", "2..3", "000000", "13..18", "000", "7..9",
                                "000", "10..12", "7980000000000000000", "-1" },
                            BlockProcessModifier.YROutcomes, string.Empty, regionCache, nullRegions,
                            new string[] { "10..12", "7..9:0000000", "0..4", "13..18", "0..0" }, region4PumpCache);
                    }
                    break;
            }

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYROutcomes, ref dsFOYROutcomes);

            WriteToTrace("������� ���� \"�������\" ���������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ���� "���� ����� ����������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepNetXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"���� ����� ����������\".", TraceMessageKind.Information);

            if (marksSubKvrCache.Count < 2)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "�� �������� ������������� '����������.��_������_������', ����� 625 �������� �� �����.");
                return;
            }

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    string[] codeExclusions = new string[] { };
                    if (this.DataSource.Year <= 2005)
                        codeExclusions = new string[] { "0000000", "5..11", "000", "12..14" };

                    PumpXMLReportBlock("���� \"���� ����� ����������\"", xnReport,
                        new string[] { "625;624", string.Empty },
                        daFOYRNet, dsFOYRNet.Tables[0], fctFOYRNet,
                        new DataTable[] { dsFKR.Tables[0], dsKCSR.Tables[0], dsKVR.Tables[0], dsMarksNet.Tables[0], dsMarksSubKvr.Tables[0] },
                        new IClassifier[] { clsFKR, clsKCSR, clsKVR, clsMarksNet, clsMarksSubKvr },
                        new int[] { 2005 },
                        new string[] { "RefFKR", "RefKCSR", "RefKVR", "RefKSSHK", "RefMarks" },
                        new int[] { nullFKR, nullKCSR, nullKVR, nullMarksNet, nullMarksSubKvr },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrCache, kcsrCache, kvrCache, marksNetCache, marksSubKvrCache },
                        codeExclusions, BlockProcessModifier.YRNet, string.Empty, regionCache, nullRegions,
                        new string[] { "0..4", "5..11", "12..14", "17..19" }, region4PumpCache);
                    break;

                default:
                    /*PumpXMLReportBlock("���� \"���� ����� ����������\"", xnReport,
                        new string[] { "625", "1" },
                        daFOYRNet, dsFOYRNet.Tables[0], fctFOYRNet,
                        new DataTable[] { dsMarksNet.Tables[0] },
                        new IClassifier[] { clsMarksNet },
                        new int[] { 2005 },
                        new string[] { "REFMARKS" },
                        new int[] { nullMarksNet },
                        progressMsg,
                        new Dictionary<string, int>[] { marksNetCache },
                        new string[] { "000", "7..9", "000", "11..13" },
                        BlockProcessModifier.YRNet, string.Empty, regionCache, nullRegions, null, null);*/

                    break;
            }

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRNet, ref dsFOYRNet);

            WriteToTrace("������� ���� \"���� ����� ����������\" ���������.", TraceMessageKind.Information);
        }

        #region ���� "������"

        // ���������� ������������� ����������.������
        private void PumpMonthRepAccount(XmlNode xnReport, string forms)
        {
            XmlNodeList formTemplates = xnReport.ParentNode.SelectNodes("Task/FormTemplates/FormTemplate");
            foreach (XmlNode formTemplate in formTemplates)
            {
                string formCode = GetAttrValueByName(formTemplate.Attributes, "Code");
                if (forms.Contains(formCode))
                {
                    XmlNodeList rows = formTemplate.SelectNodes("FormRows/Rows/Row");
                    foreach (XmlNode row in rows)
                    {
                        string code = GetAttrValueByName(row.Attributes, "����", "����11");
                        string name = GetAttrValueByName(row.ChildNodes[0].Attributes, "Name");
                        string kl = GetAttrValueByName(row.ChildNodes[0].Attributes, "Page");
                        string kst = GetAttrValueByName(row.ChildNodes[0].Attributes, "Row");

                        PumpCachedRow(accountCache, dsAccount.Tables[0], clsAccount, code, new object[] {
                            "Code", code, "Name", name, "KL", kl, "KST", kst });
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ���� "������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepBalancXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"������\".", TraceMessageKind.Information);

            string[] formSections = new string[] { "12001;13001;43001", string.Empty };
            PumpMonthRepAccount(xnReport, formSections[0]);

            PumpXMLReportBlock("���� \"������\"", xnReport,
                formSections, daFOYRBalanc, dsFOYRBalanc.Tables[0], fctFOYRBalanc,
                new DataTable[] { dsAccount.Tables[0] }, new IClassifier[] { clsAccount }, new int[] { },
                new string[] { "RefAccount" }, new int[] { nullAccount }, progressMsg,
                new Dictionary<string, int>[] { accountCache }, BlockProcessModifier.YRBalanc,
                "����;����11", regionCache, nullRegions, null, null);

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRBalanc, ref dsFOYRBalanc);

            WriteToTrace("������� ���� \"������\" ���������.", TraceMessageKind.Information);
        }

        // ���������� ������������� ����������.��_������������ �����
        private void PumpMarksFOYRBalOff(XmlNode xnReport)
        {
            XmlNodeList catalogs = xnReport.ParentNode.SelectNodes("NSI/Catalogs/Catalog");
            foreach (XmlNode catalog in catalogs)
            {
                string catalogCode = GetAttrValueByName(catalog.Attributes, "Code");
                if (catalogCode.Trim().ToUpper() == "����������")
                {
                    XmlNodeList catalogItems = catalog.SelectNodes("CatalogItem");
                    foreach (XmlNode catalogItem in catalogItems)
                    {
                        string name = GetAttrValueByName(catalogItem.Attributes, "Name");
                        string codeStr = GetAttrValueByName(catalogItem.Attributes, "Code");
                        int code = Convert.ToInt32(codeStr.Trim().PadLeft(1, '0'));

                        PumpCachedRow(marksBallOffCache, dsMarksBallOff.Tables[0], clsMarksBallOff,
                            code.ToString(), new object[] { "Code", code, "Name", name });
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ���� "������ �������"
        /// </summary>
        /// <param name="progressMsg">������ ���������</param>
        /// <param name="xnReport">������� ��� � �������</param>
        private void PumpYearRepBalOffXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("����� ������� ���� \"������ �������\".", TraceMessageKind.Information);

            PumpMarksFOYRBalOff(xnReport);

            PumpXMLReportBlock("���� \"������ �������\"", xnReport,
                new string[] { "12002;13002;43002", string.Empty }, daFOYRBalOff, dsFOYRBalOff.Tables[0], fctFOYRBalOff,
                new DataTable[] { dsMarksBallOff.Tables[0] }, new IClassifier[] { clsMarksBallOff }, new int[] { },
                new string[] { "RefMarks" }, new int[] { nullMarksBallOff }, progressMsg,
                new Dictionary<string, int>[] { marksBallOffCache }, BlockProcessModifier.YRBalanc,
                "����������", regionCache, nullRegions, null, null);

            // ���������� ������
            UpdateData();
            ClearDataSet(daFOYRBalOff, ref dsFOYRBalOff);

            WriteToTrace("������� ���� \"������ �������\" ���������.", TraceMessageKind.Information);
        }

        #endregion

        /// <summary>
        /// ���������� ������ �� ������ ������� ���
        /// </summary>
        /// <param name="xnReport">������� � ������� ������</param>
        protected override bool PumpRegionsFromXMLReport(XmlNode xnReport)
        {
            return PumpRegionsXML(xnReport, dsRegions.Tables[0], clsRegions, regionCache,
                dsRegions4Pump.Tables[0], clsRegions4Pump, region4PumpCache);
        }

        /// <summary>
        /// ���������� ����� ������� ���
        /// </summary>
        /// <param name="xnReport">������� � ������� ������</param>
        protected override void PumpXMLReport(XmlNode xnReport, string progressMsg)
        {
            if (xnReport == null)
                return;
            // ���� "������� ��������"
            if (ToPumpBlock(Block.bDefProf))
                PumpYearRepDefProfXML(progressMsg, xnReport);
            // ���� "������"
            if (ToPumpBlock(Block.bIncomes))
                PumpYearRepIncomesXML(progressMsg, xnReport);
            // ���� "��������� ��������������"
            if (ToPumpBlock(Block.bFinSources))
                PumpYearRepSrcFinXML(progressMsg, xnReport);
            // ���� "��������� �������"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepEmbezzlesXML(progressMsg, xnReport);
            // ���� "�������"
            if (ToPumpBlock(Block.bOutcomes))
                PumpYearRepOutcomesXML(progressMsg, xnReport);
            // ���� "���� ����� ����������"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepNetXML(progressMsg, xnReport);
            // ���� "������"
            if (ToPumpBlock(Block.bBalanc) && hasBalancBlock)
            {
                PumpYearRepBalancXML(progressMsg, xnReport);
                PumpYearRepBalOffXML(progressMsg, xnReport);
            }
        }

        #endregion ������� ����� ����������� ������� ������
    }
}
