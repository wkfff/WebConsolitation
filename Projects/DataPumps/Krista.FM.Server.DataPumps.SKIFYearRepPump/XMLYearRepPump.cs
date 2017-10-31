using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // Модуль функций закачки отчетов формата xml

    /// <summary>
    /// ФО_0005_Ежегодные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Функции закачки внешнего шаблона

        /// <summary>
        /// Закачивает данные классификаторов блока Доходы из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
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
        /// Закачивает данные классификаторов блока Источники финансирования из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
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
        /// Закачивает данные классификаторов блока Расходы из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
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
        /// Закачивает данные классификаторов блока Сеть Штаты Контингент из внешнего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpNetClsFromExtPatternXML(XmlDocument xdPattern)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form625 }, new int[] { 1 },
                dsMarksNet.Tables[0], clsMarksNet, new string[] {
                    "FKR", "0..4", "KCSR", "7..9", "SUBKCSR", "10..10", "KVR", "11..13", "SUBKVR", "14..15", "KNEC", "16..18" },
                false, marksNetCache, null, null, ClsProcessModifier.Standard);
        }

        /// <summary>
        /// Закачивает шаблон старого формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected override void PumpExternalXMLPattern(XmlDocument xdPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    throw new Exception("Закачка из внешнего шаблона в формате Skif3 не поддерживается");
            }

            // Доходы
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromExtPatternXML(xdPattern);

            // Источники финансирования
            if (ToPumpBlock(Block.bFinSources))
                PumpSrcFinClsFromExtPatternXML(xdPattern);

            // Расходы
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromExtPatternXML(xdPattern);

            // Сеть Штаты Контингент
            if (ToPumpBlock(Block.bNet))
                PumpNetClsFromExtPatternXML(xdPattern);
        }

        #endregion Функции закачки внешнего шаблона

        #region Функции закачки внутреннего шаблона

        /// <summary>
        /// Закачивает данные классификаторов блока Доходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpIncomesClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623, XmlForm.Form428g, XmlForm.Form428Vg },
                        new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "АдмВД" },
                        false, kdCache, null, null, ClsProcessModifier.Standard);
                    // форма 127, 128
                    PumpComplexClsFromInternalPatternXML(xnPattern,
                        new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v, XmlForm.Form128, XmlForm.Form128v },
                        new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "ВД" },
                        false, kdCache, null, null, ClsProcessModifier.Standard);
                    // форма 117
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form117 }, new int[] { 1 },
                        dsKD.Tables[0], clsKD, new string[] { "CODESTR", "ВД" }, false, kdCache, null, null, ClsProcessModifier.Standard);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "ВД" },
                            false, kdCache, null, null, ClsProcessModifier.Standard);
                    }
                    else
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "ВД" },
                            false, kdCache, null, null, ClsProcessModifier.Standard);
                    }
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники финансирования из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpSrcFinClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623, XmlForm.Form428g, XmlForm.Form428Vg },
                        new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005,
                        new string[] { "CODESTR", "ВнутрИФ;ВнешИФ;ТипИстФин+ОбщИФ" }, false, kifCache,
                        null, null, ClsProcessModifier.Standard);
                    // форма 127, 128
                    PumpComplexClsFromInternalPatternXML(xnPattern,
                        new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v, XmlForm.Form128, XmlForm.Form128v },
                        new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "КИВнФ" },
                        false, kifCache, null, null, ClsProcessModifier.Standard);
                    // форма 117
                    PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form117 }, new int[] { 3 },
                        dsKIF2005.Tables[0], clsKIF2005, new string[] { "CODESTR", "КИВФ;КИВнФ" }, false, kifCache, null, null, ClsProcessModifier.Standard);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 3 }, dsKIF2005.Tables[0], clsKIF2005,
                            new string[] { "CODESTR", "ВнутрИФ;ВнешИФ;ТипИстФин+ОбщИФ" }, false, kifCache,
                            null, null, ClsProcessModifier.Standard);
                    }
                    else
                    {
                        PumpComplexClsFromInternalPatternXML(xnPattern, new XmlForm[] { XmlForm.Form623 },
                            new int[] { 3 }, dsKIF2004.Tables[0], clsKIF2004,
                            new string[] { "CODESTR", "ВнутрИФ;ВнешИФ;ТипИстФин+ОбщИФ" }, false, kifCache,
                            null, null, ClsProcessModifier.Standard);
                    }
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Расходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpOutcomesClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKVR.Tables[0], clsKVR,
                        new string[] { "ВР", "ПППРзПрЦСРВРЭКР" },
                        new string[] { "ВР", "CODE;-1", "ПППРзПрЦСРВРЭКР", "CODE;14..16" }, kvrCache,
                        new string[] { "ПППРзПрЦСРВРЭКР", "!*000;-1", "ПППРзПрЦСРВРЭКР", "000;14..16", "ВР", "000;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKCSR.Tables[0], clsKCSR,
                        new string[] { "ЦСР", "ПППРзПрЦСРВРЭКР" },
                        new string[] { "ЦСР", "CODE;-1", "ПППРзПрЦСРВРЭКР", "CODE;7..13" }, kcsrCache,
                        new string[] { "ПППРзПрЦСРВРЭКР", "!*000000;-1", "ПППРзПрЦСРВРЭКР", "0000000;7..13", "ЦСР", "0000000;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR,
                        new string[] { "РзПр", "ПППРзПрЦСРВРЭКР" },
                        new string[] { "РзПр", "CODE;-1", "ПППРзПрЦСРВРЭКР", "CODE;3..6" }, fkrCache,
                        new string[] { "ПППРзПрЦСРВРЭКР", "!*0000000000000;-1", "ПППРзПрЦСРВРЭКР", "0000;3..6",
                            "ПППРзПрЦСРВРЭКР", "7900;3..6", "РзПр", "0000;-1", "РзПр", "7900;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsEKR.Tables[0], clsEKR,
                        new string[] { "ЭКР", "ПППРзПрЦСРВРЭКР" },
                        new string[] { "ЭКР", "CODE;-1", "ПППРзПрЦСРВРЭКР", "CODE;17..19" }, ekrCache,
                        new string[] { "ПППРзПрЦСРВРЭКР", "000;17..19", "ЭКР", "000;-1", "ЭКР", "790;-1" });
                    // форма 127 и 117
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKVR.Tables[0], clsKVR, new string[] { "ВР" },
                        new string[] { "ВР", "CODE;-1" }, kvrCache, new string[] { });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKCSR.Tables[0], clsKCSR, new string[] { "ЦСР" },
                        new string[] { "ЦСР", "CODE;-1" }, kcsrCache, new string[] { });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR, new string[] { "РзПр" },
                        new string[] { "РзПр", "CODE;-1" }, fkrCache, new string[] { "РзПр", "7900;-1" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsEKR.Tables[0], clsEKR, new string[] { "ЭКР" },
                        new string[] { "ЭКР", "CODE;-1" }, ekrCache, new string[] { "ЭКР", "790;-1" });

                    // kvsr
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsKvsr.Tables[0], clsKvsr, new string[] { "Адм" },
                        new string[] { "Адм", "CODE;-1" }, kvsrCache, new string[] { });

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
        /// Закачивает данные классификаторов блока Сеть Штаты Контингент из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpNetClsFromIntPatternXML(XmlNode xnPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsMarksNet.Tables[0], clsMarksNet, new string[] { "РзПрЦСРВРСШ" },
                        new string[] { "РзПрЦСРВРСШ", "CODE;-1..3" }, marksNetCache, new string[] { }, false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Закачивает шаблон нового формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected override void PumpInternalXMLPattern(XmlNode xnPattern)
        {
            // Доходы
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromIntPatternXML(xnPattern);
            // Источники финансирования
            if (ToPumpBlock(Block.bFinSources))
                PumpSrcFinClsFromIntPatternXML(xnPattern);
            // Расходы
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromIntPatternXML(xnPattern);
            // Сеть Штаты Контингент
            if (ToPumpBlock(Block.bNet))
                PumpNetClsFromIntPatternXML(xnPattern);
        }

        #endregion Функции закачки внутреннего шаблона

        #region Функции общей организации закачки блоков

        /// <summary>
        /// Закачивает блок "Дефицит Профицит"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepDefProfXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "623", "26" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!7980", "-1" },
                        BlockProcessModifier.YRDefProf, "Рз+Пр", regionCache, nullRegions, null, region4PumpCache);
                    break;

                case XmlFormat.Skif3:
                    // форма 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "127;127g;127v", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "РзПрЦСРВРЭКР", regionCache, nullRegions, null, region4PumpCache);
                    //  форма 428
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "428g;428Vg", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00079000000000000000", "-1" },
                        BlockProcessModifier.YRDefProf, "ПППРзПрЦСРВРЭКР", regionCache, nullRegions, null, region4PumpCache);
                    //  форма 128
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "128;128V", "22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { },
                        BlockProcessModifier.YRDefProf, "Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions, null, region4PumpCache);
                    //  форма 117
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "117", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "РзПрЦСРВРЭКР", regionCache, nullRegions, null, region4PumpCache);
                    // форма 127
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "127;127g;127v", "12;22" },
                        daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                        null, null, null, null, null, progressMsg, null, new string[] { "!00000000000000790", "-1" },
                        BlockProcessModifier.YRDefProf, "РзПрЦСРВРЭКР", regionCache, nullRegions, null, region4PumpCache);
                    break;
                default:
                    if (this.DataSource.Year <= 2004)
                    {
                        PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "623", "26" },
                            daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                            null, null, null, null, null, progressMsg, null, new string[] { "!7980000000000000000", "-1" },
                            BlockProcessModifier.YRDefProf, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    }
                    else
                    {
                        PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, new string[] { "623", "26" },
                            daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf,
                            null, null, null, null, null, progressMsg, null, new string[] { "!79800000000000000000", "-1" },
                            BlockProcessModifier.YRDefProf, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    }
                    break;
            }

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRDefProf, ref dsFOYRDefProf);

            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Доходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepIncomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // форма 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("Блок \"Доходы\"", xnReport, new string[] { "127;127g;127v", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 428, 128
                    PumpXMLReportBlock("Блок \"Доходы\"", xnReport, new string[] { "428g;428Vg;128;128V", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 117
                    PumpXMLReportBlock("Блок \"Доходы\"", xnReport, new string[] { "117", "1" },
                        daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                        new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                        new IClassifier[] { clsKD, clsKD },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKD", "REFKD" },
                        new int[] { nullKD, nullKD },
                        progressMsg,
                        new Dictionary<string, int>[] { kdCache },
                        BlockProcessModifier.YRIncomes, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 127 второй раз
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("Блок \"Доходы\"", xnReport, new string[] { "127;127g;127v", "1" },
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
                    PumpXMLReportBlock("Блок \"Доходы\"", xnReport, new string[] { "623", "1" },
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

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRIncomes, ref dsFOYRIncomes);

            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepSrcFinXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Источники финансирования\".", TraceMessageKind.Information);

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // форма 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("Блок \"Источники финансирования\"", xnReport, new string[] { "127;127g;127v", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 428, 128
                    PumpXMLReportBlock("Блок \"Источники финансирования\"", xnReport, new string[] { "428g;428Vg;128;128V", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 117
                    PumpXMLReportBlock("Блок \"Источники финансирования\"", xnReport, new string[] { "117", "3" },
                        daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                        new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                        new IClassifier[] { clsKIF2004, clsKIF2005 },
                        new int[] { 2004, 2005 },
                        new string[] { "REFKIF2004", "REFKIF2005" },
                        new int[] { nullKIF2004, nullKIF2005 },
                        progressMsg,
                        new Dictionary<string, int>[] { kifCache },
                        BlockProcessModifier.YRSrcFin, string.Empty, regionCache, nullRegions, null, region4PumpCache);
                    // форма 127 - второй раз
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("Блок \"Источники финансирования\"", xnReport, new string[] { "127;127g;127v", "3" },
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
                    PumpXMLReportBlock("Блок \"Источники финансирования\"", xnReport, new string[] { "428g;428Vg", "3" },
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

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRSrcFin, ref dsFOYRSrcFin);

            WriteToTrace("Закачка Блок \"Источники финансирования\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Недостачи Хищения"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepEmbezzlesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Недостачи Хищения\".", TraceMessageKind.Information);

            PumpXMLReportBlock("Блок \"Недостачи Хищения\"", xnReport, new string[] { "630", "3" },
                daFOYREmbezzles, dsFOYREmbezzles.Tables[0], fctFOYREmbezzles,
                new DataTable[] { dsMarksEmbezzles.Tables[0], dsMeansType.Tables[0] },
                new IClassifier[] { fxcMarksEmbezzles, fxcMeansType },
                new int[] { 2005 },
                new string[] { "REFMARKS", "REFMEANSTYPE" },
                new int[] { 0, 0 },
                progressMsg,
                new Dictionary<string, int>[] { marksEmbezzlesCache, meansTypeCache },
                BlockProcessModifier.YREmbezzles, string.Empty, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYREmbezzles, ref dsFOYREmbezzles);

            WriteToTrace("Закачка Блок \"Недостачи Хищения\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Расходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepOutcomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    // форма 127
                    forcePumpForm127 = false;
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "127;127g;127v", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "ПППРзПрЦСРВРЭКР;Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // форма 428
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "428g;428Vg", "12;22" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "ПППРзПрЦСРВРЭКР;Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // форма 128
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "128;128V", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "ПППРзПрЦСРВРЭКР;Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // форма 117
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "117", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "ПППРзПрЦСРВРЭКР;Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    // форма 127 - второй раз
                    forcePumpForm127 = true;
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "127;127g;127v", "12" },
                        daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                        new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], null, dsEKR.Tables[0], dsKvsr.Tables[0] },
                        new IClassifier[] { clsKVR, clsKCSR, clsFKR, null, clsEKR, clsKvsr },
                        new int[] { 2005 },
                        new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR", "RefKVSRYR" },
                        new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR, nullKvsr },
                        progressMsg,
                        new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, null, ekrCache, kvsrCache },
                        new string[] { "#!9600;#!9800;7900", "3..6", "000", "17..19" },
                        BlockProcessModifier.YROutcomes, "ПППРзПрЦСРВРЭКР;Адм+РзПр+ЦСР+ВР+ЭКР", regionCache, nullRegions,
                        new string[] { "14..16", "7..13", "3..6", "0..0", "17..19", "0..3" }, region4PumpCache);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                    {
                        PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "623", "4..26" },
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
                        PumpXMLReportBlock("Блок \"Расходы\"", xnReport, new string[] { "623", "4..26" },
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

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYROutcomes, ref dsFOYROutcomes);

            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Сеть Штаты Контингент"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepNetXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Сеть Штаты Контингент\".", TraceMessageKind.Information);

            if (marksSubKvrCache.Count < 2)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "Не заполнен классификатор 'Показатели.ФО_ГодОтч_СубКВР', форма 625 закачана не будет.");
                return;
            }

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                    string[] codeExclusions = new string[] { };
                    if (this.DataSource.Year <= 2005)
                        codeExclusions = new string[] { "0000000", "5..11", "000", "12..14" };

                    PumpXMLReportBlock("Блок \"Сеть Штаты Контингент\"", xnReport,
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
                    /*PumpXMLReportBlock("Блок \"Сеть Штаты Контингент\"", xnReport,
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

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRNet, ref dsFOYRNet);

            WriteToTrace("Закачка Блок \"Сеть Штаты Контингент\" закончена.", TraceMessageKind.Information);
        }

        #region Блок "Баланс"

        // закачивает классификатор ПланСчетов.МесОтч
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
                        string code = GetAttrValueByName(row.Attributes, "ПлСч", "ПлСч11");
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
        /// Закачивает блок "Баланс"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepBalancXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Баланс\".", TraceMessageKind.Information);

            string[] formSections = new string[] { "12001;13001;43001", string.Empty };
            PumpMonthRepAccount(xnReport, formSections[0]);

            PumpXMLReportBlock("Блок \"Баланс\"", xnReport,
                formSections, daFOYRBalanc, dsFOYRBalanc.Tables[0], fctFOYRBalanc,
                new DataTable[] { dsAccount.Tables[0] }, new IClassifier[] { clsAccount }, new int[] { },
                new string[] { "RefAccount" }, new int[] { nullAccount }, progressMsg,
                new Dictionary<string, int>[] { accountCache }, BlockProcessModifier.YRBalanc,
                "ПлСч;ПлСч11", regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRBalanc, ref dsFOYRBalanc);

            WriteToTrace("Закачка Блок \"Баланс\" закончена.", TraceMessageKind.Information);
        }

        // закачивает классификатор Показатели.ФО_Забалансовые счета
        private void PumpMarksFOYRBalOff(XmlNode xnReport)
        {
            XmlNodeList catalogs = xnReport.ParentNode.SelectNodes("NSI/Catalogs/Catalog");
            foreach (XmlNode catalog in catalogs)
            {
                string catalogCode = GetAttrValueByName(catalog.Attributes, "Code");
                if (catalogCode.Trim().ToUpper() == "ЗБСЧСТРОКА")
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
        /// Закачивает блок "Баланс Справка"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpYearRepBalOffXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Баланс Справка\".", TraceMessageKind.Information);

            PumpMarksFOYRBalOff(xnReport);

            PumpXMLReportBlock("Блок \"Баланс Справка\"", xnReport,
                new string[] { "12002;13002;43002", string.Empty }, daFOYRBalOff, dsFOYRBalOff.Tables[0], fctFOYRBalOff,
                new DataTable[] { dsMarksBallOff.Tables[0] }, new IClassifier[] { clsMarksBallOff }, new int[] { },
                new string[] { "RefMarks" }, new int[] { nullMarksBallOff }, progressMsg,
                new Dictionary<string, int>[] { marksBallOffCache }, BlockProcessModifier.YRBalanc,
                "ЗбСчСтрока", regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRBalOff, ref dsFOYRBalOff);

            WriteToTrace("Закачка Блок \"Баланс Справка\" закончена.", TraceMessageKind.Information);
        }

        #endregion

        /// <summary>
        /// Закачивает районы из отчета формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected override bool PumpRegionsFromXMLReport(XmlNode xnReport)
        {
            return PumpRegionsXML(xnReport, dsRegions.Tables[0], clsRegions, regionCache,
                dsRegions4Pump.Tables[0], clsRegions4Pump, region4PumpCache);
        }

        /// <summary>
        /// Закачивает отчет формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected override void PumpXMLReport(XmlNode xnReport, string progressMsg)
        {
            if (xnReport == null)
                return;
            // Блок "Дефицит Профицит"
            if (ToPumpBlock(Block.bDefProf))
                PumpYearRepDefProfXML(progressMsg, xnReport);
            // Блок "Доходы"
            if (ToPumpBlock(Block.bIncomes))
                PumpYearRepIncomesXML(progressMsg, xnReport);
            // Блок "Источники финансирования"
            if (ToPumpBlock(Block.bFinSources))
                PumpYearRepSrcFinXML(progressMsg, xnReport);
            // Блок "Недостачи Хищения"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepEmbezzlesXML(progressMsg, xnReport);
            // Блок "Расходы"
            if (ToPumpBlock(Block.bOutcomes))
                PumpYearRepOutcomesXML(progressMsg, xnReport);
            // Блок "Сеть Штаты Контингент"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepNetXML(progressMsg, xnReport);
            // Блок "Баланс"
            if (ToPumpBlock(Block.bBalanc) && hasBalancBlock)
            {
                PumpYearRepBalancXML(progressMsg, xnReport);
                PumpYearRepBalOffXML(progressMsg, xnReport);
            }
        }

        #endregion Функции общей организации закачки блоков
    }
}
