using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // Модуль закачки ежемесячных отчетов формата XML

    /// <summary>
    /// ФО_0002_Ежемесячные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Функции закачки внешнего шаблона

        /// <summary>
        /// Закачивает данные классификаторов блока Доходы из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpIncomesClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year >= 2005)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form649, XmlForm.Form428, XmlForm.Form650, XmlForm.Form428v },
                    new int[] { 1 }, dsKD.Tables[0], clsKD, new string[] { "CODESTR", "-1" }, true,
                    kdCache, new string[] { "998*;999*" }, null, ClsProcessModifier.Standard);
            }
            else
            {
                PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form649, XmlForm.Form428, XmlForm.Form650, XmlForm.Form428v },
                    new int[] { 1 }, dsKD.Tables[0], clsKD, kdCache);
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники внешнего финансирования из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpSrcOutFinClsFromExtPatternXML(XmlDocument xdPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form428, XmlForm.Form428v },
                        new int[] { 3 }, dsSrcOutFin.Tables[0], clsSrcOutFin, srcOutFinCache);
                    break;

                default:
                    PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form649, XmlForm.Form650 },
                        new int[] { 5 }, dsSrcOutFin.Tables[0], clsSrcOutFin, srcOutFinCache);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники внутреннего финансирования из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpSrcInFinClsFromExtPatternXML(XmlDocument xdPattern)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form428, XmlForm.Form428v },
                        new int[] { 3 }, dsSrcInFin.Tables[0], clsSrcInFin, srcInFinCache);
                    break;

                default:
                    PumpClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form649, XmlForm.Form650 },
                        new int[] { 4 }, dsSrcInFin.Tables[0], clsSrcInFin, srcInFinCache);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Расходы из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpOutcomesClsFromExtPatternXML(XmlDocument xdPattern)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form649, XmlForm.Form428, XmlForm.Form650, XmlForm.Form428v },
                new int[] { 2 }, dsFKR.Tables[0], clsFKR, new string[] { "CODE", "-1" }, true, fkrCache,
                new string[] { "9990..9998" }, null, ClsProcessModifier.Standard);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправВнешнийДолг из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpOutDebtBooksClsFromExtPatternXML(XmlDocument xdPattern)
        {
            XmlForm[] xmlForms;
            string[] attrsValuesMapping;
            int[] sections = null;
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                xmlForms = new XmlForm[] { XmlForm.Form487, XmlForm.Form651 };
                sections = new int[] { 3 };
                attrsValuesMapping = new string[] { "SRCOUTFIN", "0..20", "GVRMOUTDEBT", "-1..6" };
            }
            else if (this.DataSource.Year >= 2005)
            {
                xmlForms = new XmlForm[] { XmlForm.Form414, XmlForm.Form651 };
                sections = new int[] { 5 };
                attrsValuesMapping = new string[] { "SRCOUTFIN", "0..20", "GVRMOUTDEBT", "-1..6" };
            }
            else
            {
                xmlForms = new XmlForm[] { XmlForm.Form414, XmlForm.Form651 };
                sections = new int[] { 5 };
                attrsValuesMapping = new string[] { "SRCOUTFIN", "0..4", "GVRMOUTDEBT", "6..-1" };
            }
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForms, sections, dsMarksOutDebt.Tables[0],
                clsMarksOutDebt, attrsValuesMapping, true, scrOutFinSourcesRefCache, null, null, ClsProcessModifier.Special);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправВнутреннийДолг из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpInDebtBooksClsFromExtPatternXML(XmlDocument xdPattern)
        {
            XmlForm[] xmlForms = null;
            string[] attrsValuesMapping = null;
            int[] sections = null;
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                xmlForms = new XmlForm[] { XmlForm.Form487, XmlForm.Form651 };
                sections = new int[] { 2 };
                attrsValuesMapping = new string[] { "SRCINFIN", "0..20", "GVRMINDEBT", "-1..3" };
            }
            else if (this.DataSource.Year >= 2005)
            {
                xmlForms = new XmlForm[] { XmlForm.Form414, XmlForm.Form651 };
                sections = new int[] { 4 };
                attrsValuesMapping = new string[] { "SRCINFIN", "0..20", "GVRMINDEBT", "-1..3" };
            }
            else if (this.DataSource.Year >= 2003)
            {
                xmlForms = new XmlForm[] { XmlForm.Form651 };
                sections = new int[] { 4 };
                attrsValuesMapping = new string[] { "SRCINFIN", "0..5", "GVRMINDEBT", "5..7" };
            }
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForms, sections, dsMarksInDebt.Tables[0],
                clsMarksInDebt, attrsValuesMapping, true, scrInFinSourcesRefCache, null, null, ClsProcessModifier.Special);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправДоходы из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpIncomesBooksClsFromExtPatternXML(XmlDocument xdPattern)
        {
            // доходы грузим из 414 (в 487 - нет)
            PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                new int[] { 1 }, dsKVSR.Tables[0], clsKVSR, new string[] { "CODE", "-1..3" }, true, kvsrCache,
                null, null, ClsProcessModifier.Special);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправЗадолженность из внешнего шаблона
        /// </summary>
        /// <param name="xdPattern">Шаблон</param>
        private void PumpArrearsBooksClsFromExtPatternXML(XmlDocument xdPattern)
        {
            XmlForm[] xmlForms;
            string[] attrsValuesMapping;
            int[] sections = null;
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                xmlForms = new XmlForm[] { XmlForm.Form487, XmlForm.Form651 };
                sections = new int[] { 4 };
                attrsValuesMapping = new string[] { "FKR", "0..4", "EKR", "-1..3" };
            }
            else if (this.DataSource.Year >= 2005)
            {
                xmlForms = new XmlForm[] { XmlForm.Form414, XmlForm.Form651 };
                sections = new int[] { 6 };
                attrsValuesMapping = new string[] { "FKR", "0..4", "EKR", "-1..3" };
            }
            else
            {
                xmlForms = new XmlForm[] { XmlForm.Form414, XmlForm.Form651 };
                sections = new int[] { 6 };
                attrsValuesMapping = new string[] { "FKR", "0..4", "EKR", "-1..6" };
            }
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForms, sections, dsMarksArrears.Tables[0],
                clsMarksArrears, attrsValuesMapping, false, null, null, null, ClsProcessModifier.Special);
        }

        private void PumpOutcomesBooksClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form487, XmlForm.Form651 },
                    new int[] { 1, 4 }, dsFKRBook.Tables[0], clsFKRBook, new string[] { "CODE", "0..4" }, true, fkrBookCache,
                    null, new string[] { "2..16" }, ClsProcessModifier.FKRBook);
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form487, XmlForm.Form651 },
                    new int[] { 1, 4 }, dsEKRBook.Tables[0], clsEKRBook, new string[] { "CODE", "-1..3" }, true,
                    ekrBookCache, null, new string[] { "2..16" }, ClsProcessModifier.EKRBook);
            }
            else if (this.DataSource.Year >= 2005)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsFKRBook.Tables[0], clsFKRBook, new string[] { "CODE", "0..4" }, true, fkrBookCache,
                    null, new string[] { "2..16" }, ClsProcessModifier.FKRBook);
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsEKRBook.Tables[0], clsEKRBook, new string[] { "CODE", "-1..3" }, true,
                    ekrBookCache, null, new string[] { "2..16" }, ClsProcessModifier.EKRBook);
            }
            else
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsFKRBook.Tables[0], clsFKRBook, new string[] { "CODE", "0..4" }, true, fkrBookCache,
                    null, new string[] { "5..20;44;51" }, ClsProcessModifier.FKRBook);
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsEKRBook.Tables[0], clsEKRBook, new string[] { "CODE", "-1..6" }, true, ekrBookCache,
                    null, new string[] { "5..20;44;51" }, ClsProcessModifier.EKRBook);
            }
        }

        private void PumpOutcomesBooksAddClsFromExtPatternXML(XmlDocument xdPattern)
        {
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form487, XmlForm.Form651 },
                    new int[] { 1, 4 }, dsMarksOutcomes.Tables[0], clsMarksOutcomes,
                    new string[] { "FKR", "0..4", "EKR", "-1..3" }, false, marksOutcomesCache,
                    null, new string[] { "17..75" }, ClsProcessModifier.MarksOutcomes);
            }
            else if (this.DataSource.Year >= 2005)
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsMarksOutcomes.Tables[0], clsMarksOutcomes,
                    new string[] { "FKR", "0..4", "EKR", "-1..3" }, false, marksOutcomesCache,
                    null, new string[] { "17..75" }, ClsProcessModifier.MarksOutcomes);
            }
            else
            {
                PumpComplexClsFromExternalPatternXML(xdPattern, new XmlForm[] { XmlForm.Form414, XmlForm.Form651 },
                    new int[] { 2 }, dsMarksOutcomes.Tables[0], clsMarksOutcomes,
                    new string[] { "FKR", "0..4", "EKR", "-1..6" }, true, marksOutcomesCache,
                    null, new string[] { "21..43;45..50;52..84" }, ClsProcessModifier.MarksOutcomes);
            }
        }

        /// <summary>
        /// Закачивает шаблон старого формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected override void PumpExternalXMLPattern(XmlDocument xdPattern)
        {
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bInnerFinSources))
                PumpSrcInFinClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bOuterFinSources))
                PumpSrcOutFinClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bIncomesRefs))
                PumpIncomesBooksClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bOutcomesRefs))
                PumpOutcomesBooksClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
                PumpOutcomesBooksAddClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
                PumpInDebtBooksClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
                PumpOutDebtBooksClsFromExtPatternXML(xdPattern);
            if (ToPumpBlock(Block.bArrearsRefs))
                PumpArrearsBooksClsFromExtPatternXML(xdPattern);
        }

        #endregion Функции закачки внешнего шаблона

        #region Функции закачки внутреннего шаблона

        /// <summary>
        /// Закачивает данные классификаторов блока Доходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpIncomesClsFromIntPatternXML(XmlNode xnPattern)
        {
            if (this.DataSource.Year >= 2005)
            {
                XmlForm[] forms = GetXmlFormByFormatXML();
                forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form628r });
                if (this.DataSource.Year < 2008)
                    forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form128, XmlForm.Form128v });

                PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 1 }, dsKD.Tables[0], clsKD,
                    new string[] { "CODESTR", "ВД;АдмВД" }, false, kdCache, new string[] { "998*;999*" }, null, ClsProcessModifier.Standard);
                if (this.DataSource.Year >= 2008)
                {
                    forms = new XmlForm[] { XmlForm.Form117 };
                    PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 1 }, dsKD.Tables[0], clsKD,
                        new string[] { "CODESTR", "ВД" }, false, kdCache, null, null, ClsProcessModifier.Standard);
                }

                // форма 127
                forms = new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v };
                PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 1 }, dsKD.Tables[0], clsKD,
                    new string[] { "CODESTR", "ВД" }, false, kdCache, null, null, ClsProcessModifier.Standard);

            }
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, GetXmlFormByFormatXML(), new int[] { 1 }, dsKD.Tables[0], clsKD,
                    new string[] { "CODESTR", "ВД;АдмВД" }, false, kdCache, null, null, ClsProcessModifier.Standard);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники внешнего финансирования из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpSrcOutFinClsFromIntPatternXML(XmlNode xnPattern)
        {
            int[] sectNo = null;
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                case XmlFormat.Format2005: sectNo = new int[1] { 5 };
                    break;
                default: sectNo = new int[1] { 3 };
                    break;
            }
            if (this.DataSource.Year >= 2005)
            {
                XmlForm[] forms = GetXmlFormByFormatXML();
                forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form628r });
                if (this.DataSource.Year < 2008)
                    forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form128, XmlForm.Form128v });

                PumpComplexClsFromInternalPatternXML(xnPattern, forms, sectNo, dsSrcOutFin.Tables[0],
                    clsSrcOutFin, new string[] { "CODESTR", "КИВнФ;АдмКИВнФ" }, true, srcOutFinCache, null, null, ClsProcessModifier.SrcOutFin);
                if (this.DataSource.Year >= 2008)
                {
                    forms = new XmlForm[] { XmlForm.Form117 };
                    sectNo = new int[] { 3 };
                    PumpComplexClsFromInternalPatternXML(xnPattern, forms, sectNo, dsSrcOutFin.Tables[0],
                        clsSrcOutFin, new string[] { "CODESTR", "КИВнФ" }, true, srcOutFinCache, null, null, ClsProcessModifier.SrcOutFin);
                }

                // форма 127
                forms = new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v };
                PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 3 }, dsSrcOutFin.Tables[0],
                    clsSrcOutFin, new string[] { "CODESTR", "КИВнФ" }, true, srcOutFinCache, null, null, ClsProcessModifier.SrcOutFin);

            }
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, GetXmlFormByFormatXML(), sectNo, dsSrcOutFin.Tables[0],
                    clsSrcOutFin, new string[] { "CODEStr", "ВнешнИФ;ТипИстФин+ОбщИФ" }, true, srcOutFinCache, null, null, ClsProcessModifier.SrcOutFin);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока Источники внутреннего финансирования из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpSrcInFinClsFromIntPatternXML(XmlNode xnPattern)
        {
            int[] sectNo = null;
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                case XmlFormat.Format2005: sectNo = new int[1] { 4 };
                    break;

                default: sectNo = new int[1] { 3 };
                    break;
            }
            if (this.DataSource.Year >= 2005)
            {
                XmlForm[] forms = GetXmlFormByFormatXML();
                forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form628r });
                if (this.DataSource.Year < 2008)
                    forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form128, XmlForm.Form128v });

                PumpComplexClsFromInternalPatternXML(xnPattern, forms, sectNo, dsSrcInFin.Tables[0],
                    clsSrcInFin, new string[] { "CODESTR", "КИВФ;АдмКИВФ" }, true, srcInFinCache, null, null, ClsProcessModifier.SrcInFin);
                if (this.DataSource.Year >= 2008)
                {
                    forms = new XmlForm[] { XmlForm.Form117 };
                    sectNo = new int[] { 3 };
                    PumpComplexClsFromInternalPatternXML(xnPattern, forms, sectNo, dsSrcInFin.Tables[0],
                        clsSrcInFin, new string[] { "CODESTR", "КИВФ" }, true, srcInFinCache, null, null, ClsProcessModifier.SrcInFin);
                }

                // форма 127
                forms = new XmlForm[] { XmlForm.Form127, XmlForm.Form127g, XmlForm.Form127v };
                PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 3 }, dsSrcInFin.Tables[0],
                    clsSrcInFin, new string[] { "CODESTR", "КИВФ" }, true, srcInFinCache, null, null, ClsProcessModifier.SrcInFin);

            }
            else if (this.DataSource.Year >= 2003)
                PumpComplexClsFromInternalPatternXML(xnPattern, GetXmlFormByFormatXML(), sectNo, dsSrcInFin.Tables[0],
                    clsSrcInFin, new string[] { "CODE", "ВнутрИФ" }, true, srcInFinCache, null, null, ClsProcessModifier.SrcInFin);
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, GetXmlFormByFormatXML(), sectNo, dsSrcInFin.Tables[0],
                    clsSrcInFin, new string[] { "CODESTR", "КИВФ;АдмКИВФ" }, true, srcInFinCache, null, null, ClsProcessModifier.SrcInFin);
        }

        #region расходы клс

        private void FormAuxClsCaches(XmlNode xnPattern, ref Dictionary<int, string> cache, string[] nsiAttrName)
        {
            XmlNodeList xnlNSIData = xnPattern.SelectNodes(string.Format(
                "//NSI/Catalogs/Catalog{0}", GetXPathConstrByAttr("Code", nsiAttrName)));
            for (int i = 0; i < xnlNSIData.Count; i++)
            {
                XmlNodeList xnlCatalogItems = xnlNSIData[i].SelectNodes("./CatalogItem");
                for (int j = 0; j < xnlCatalogItems.Count; j++)
                {
                    string codeStr = xnlCatalogItems[j].Attributes["Code"].Value.Trim();
                    if (codeStr == string.Empty)
                        continue;
                    if (CommonRoutines.TrimNumbers(codeStr) != string.Empty)
                        continue;
                    int code = Convert.ToInt32(codeStr);
                    string name = xnlCatalogItems[j].Attributes["Name"].Value;
                    if (!cache.ContainsKey(code))
                        cache.Add(code, name);
                }
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
                case XmlFormat.October2005:
                case XmlFormat.Skif3:
                    XmlForm[] forms = GetXmlFormByFormatXML();
                    forms = (XmlForm[])CommonRoutines.ConcatArrays(forms, new XmlForm[] { XmlForm.Form628r });
                    PumpComplexClsFromInternalPatternXML(dsFKR.Tables[0], xnPattern, forms, new int[] { 2 },
                        clsFKR, new string[] { "CODE", "3..6" }, true, fkrCache, new string[] { "0000;7900", "3..6", "!000", "17..19" }, null, ClsProcessModifier.FKR);
                    PumpComplexClsFromInternalPatternXML(dsEKR.Tables[0], xnPattern, forms, new int[] { 2 },
                        clsEKR, new string[] { "CODE", "17..19" }, true, ekrCache, new string[] { "000", "17..19" }, null, ClsProcessModifier.EKR);

                    // 128
                    if (this.DataSource.Year < 2008)
                    {
                        forms = new XmlForm[] { XmlForm.Form128, XmlForm.Form128v };
                        // PumpComplexClsFromInternalPatternXML(dsFKR.Tables[0], xnPattern, forms, new int[] { 12 },
                        //     clsFKR, new string[] { "CODE", "3..6" }, true, fkrCache, new string[] { "0000;7900", "3..6", "!000", "17..19" }, null, ClsProcessModifier.FKR);
                        PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 12 }, dsEKR.Tables[0],
                            clsEKR, new string[] { "CODE", "ЭКР" }, true, ekrCache, null, null, ClsProcessModifier.EKR);
                    }


                    if (this.DataSource.Year >= 2008)
                    {
                        forms = new XmlForm[] { XmlForm.Form117 };
                        PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR, new string[] { "РзПр" },
                            new string[] { "РзПр", "Code;-1" }, fkrCache, new string[] { "7900"});
                        PumpComplexClsFromInternalPatternXML(xnPattern, forms, new int[] { 12 }, dsEKR.Tables[0],
                            clsEKR, new string[] { "CODE", "ЭКР" }, true, ekrCache, null, null, ClsProcessModifier.EKR);
                    }

                    // форма 127
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsFKR.Tables[0], clsFKR, new string[] { "РзПр" },
                        new string[] { "РзПр", "Code;-1" }, fkrCache, new string[] { "7900" });
                    PumpClsFromInternalNSIPatternXML(xnPattern, dsEKR.Tables[0], clsEKR, new string[] { "ЭКР" },
                        new string[] { "ЭКР", "Code;-1" }, ekrCache, new string[] { "790" });

                    // формируем вспомогательные кэши по квр, кцср, фкр (код - наименование).
                    // нужно при формировании клс расходы (форма 117, 127) (как обычно полная ебанутость, но всем похуй)

                    FormAuxClsCaches(xnPattern, ref kvrAuxCache, new string[] { "ВР" });
                    FormAuxClsCaches(xnPattern, ref kcsrAuxCache, new string[] { "ЦСР" });
                    FormAuxClsCaches(xnPattern, ref fkrAuxCache, new string[] { "РзПр" });

                    break;
                default:
                    PumpComplexClsFromInternalPatternXML(xnPattern, GetXmlFormByFormatXML(), new int[] { 2 }, dsFKR.Tables[0],
                        clsFKR, new string[] { "CODE", "Рз+Пр;РзПр" }, true, fkrCache, new string[] { "9990..9998;7900", "0..4" }, null, ClsProcessModifier.CacheSubCode);
                    break;
            }
        }

        #endregion расходы клс

        /// <summary>
        /// Закачивает данные классификаторов блока СправВнешнийДолг из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpOutDebtBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            int[] sections = null;
            if (xmlForm[0] == XmlForm.Form487)
                sections = new int[] { 3 };
            else
                sections = new int[] { 5 };
            if (this.DataSource.Year >= 2010)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { };
                PumpComplexClsFromInternalPatternXML(dsMarksOutDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutDebt, new string[] { "SRCOUTFIN", "0..1", "GVRMOUTDEBT", "0..1" }, false,
                    scrOutFinSourcesRefCache, null, null, ClsProcessModifier.MarksOutDebt);
            }
            else if (this.DataSource.Year >= 2008)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { 2 };
                PumpComplexClsFromInternalPatternXML(dsMarksOutDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutDebt, new string[] { "SRCOUTFIN", "0..1", "GVRMOUTDEBT", "0..1" }, false,
                    scrOutFinSourcesRefCache, null, null, ClsProcessModifier.MarksOutDebt);
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                PumpComplexClsFromInternalPatternXML(dsMarksOutDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutDebt, new string[] { "SRCOUTFIN", "0..1", "GVRMOUTDEBT", "0..1" }, false,
                    scrOutFinSourcesRefCache, null, null, ClsProcessModifier.MarksOutDebt);
            }
            else if (this.DataSource.Year >= 2005)
                PumpComplexClsFromInternalPatternXML(dsMarksOutDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutDebt, new string[] { "SRCOUTFIN", "0..20", "GVRMOUTDEBT", "20..25" }, false,
                    scrOutFinSourcesRefCache, null, null, ClsProcessModifier.Special);
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sections, dsMarksOutDebt.Tables[0], clsMarksOutDebt,
                    new string[] { "SRCOUTFIN", "КИВнФ;АдмКИВнФ;ВнешнИФ", "GVRMOUTDEBT", "ГВнешД" }, false,
                    scrOutFinSourcesRefCache, null, null, ClsProcessModifier.MarksOutDebt);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправВнутреннийДолг из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpInDebtBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            int[] sections = null;
            if (xmlForm[0] == XmlForm.Form487)
                sections = new int[] { 2 };
            else
                sections = new int[] { 4 };
            if (this.DataSource.Year >= 2010)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { };
                PumpComplexClsFromInternalPatternXML(dsMarksInDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksInDebt, new string[] { "SRCINFIN", "0..1", "GVRMINDEBT", "0..1" }, false,
                    scrInFinSourcesRefCache, null, null, ClsProcessModifier.MarksInDebt);
            }
            else if (this.DataSource.Year >= 2008)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { 2 };
                PumpComplexClsFromInternalPatternXML(dsMarksInDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksInDebt, new string[] { "SRCINFIN", "0..1", "GVRMINDEBT", "0..1" }, false,
                    scrInFinSourcesRefCache, null, null, ClsProcessModifier.MarksInDebt);
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                PumpComplexClsFromInternalPatternXML(dsMarksInDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksInDebt, new string[] { "SRCINFIN", "0..1", "GVRMINDEBT", "0..1" }, false,
                    scrInFinSourcesRefCache, null, null, ClsProcessModifier.MarksInDebt);
            }
            else if (this.DataSource.Year >= 2005)
                PumpComplexClsFromInternalPatternXML(dsMarksInDebt.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksInDebt, new string[] { "SRCINFIN", "0..20", "GVRMINDEBT", "20..22" }, false,
                    scrInFinSourcesRefCache, null, null, ClsProcessModifier.Special);
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sections, dsMarksInDebt.Tables[0], clsMarksInDebt,
                    new string[] { "SRCINFIN", "КИВФ;АдмКИВФ;ВнутрИФ", "GVRMINDEBT", "ГВнутрД" }, false,
                    scrInFinSourcesRefCache, null, null, ClsProcessModifier.MarksInDebt);
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправДоходы из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpIncomesBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            // доходы всегда качаем из 414
            if (xmlForm[0] == XmlForm.Form487)
                xmlForm[0] = XmlForm.Form414;
            int[] sections;
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                sections = null;
            else
                sections = new int[] { 1 };
            if (this.DataSource.Year >= 2005)
                PumpComplexClsFromInternalPatternXML(dsKVSR.Tables[0], xnPattern, xmlForm, sections, clsKVSR,
                    new string[] { "CODE", "20..22" }, true, kvsrCache, null, null, ClsProcessModifier.CacheSubCode);
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sections, dsKVSR.Tables[0], clsKVSR,
                    new string[] { "CODE", "ППП" }, true, kvsrCache, null, null, ClsProcessModifier.CacheSubCode);

            // форма 127
            PumpClsFromInternalNSIPatternXML(xnPattern, dsKVSR.Tables[0], clsKVSR, new string[] { "Адм" },
                new string[] { "Адм", "Code;-1" }, kvsrCache, new string[] { "000" });
        }

        /// <summary>
        /// Закачивает данные классификаторов блока СправЗадолженность из внутреннего шаблона
        /// </summary>
        /// <param name="xnPattern">Шаблон</param>
        private void PumpArrearsBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            int[] sections = null;
            if (xmlForm[0] == XmlForm.Form487)
                sections = new int[] { 4 };
            else
                sections = new int[] { 6 };
            if (this.DataSource.Year >= 2010)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { };
                PumpComplexClsFromInternalPatternXML(dsMarksArrears.Tables[0], xnPattern, xmlForm, sections, clsMarksArrears,
                    new string[] { "FKR", "8..11", "EKR", "22..24", "KCSR", "12..18", "KVR", "19..21" },
                    false, arrearsCache, null, null, ClsProcessModifier.Arrears);
            }
            else if (this.DataSource.Year >= 2008)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { 2 };
                PumpComplexClsFromInternalPatternXML(dsMarksArrears.Tables[0], xnPattern, xmlForm, sections, clsMarksArrears,
                    new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, arrearsCache, null, null, ClsProcessModifier.Arrears);
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { 4 };
                PumpComplexClsFromInternalPatternXML(dsMarksArrears.Tables[0], xnPattern, xmlForm, sections, clsMarksArrears,
                    new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, arrearsCache, null, null, ClsProcessModifier.Arrears);
            }
            else if (this.DataSource.Year >= 2005)
                PumpComplexClsFromInternalPatternXML(dsMarksArrears.Tables[0], xnPattern, xmlForm, sections, clsMarksArrears,
                    new string[] { "FKR", "0..4", "EKR", "4..6" }, false, arrearsCache, null, null, ClsProcessModifier.Arrears);
            else
                PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sections, dsMarksArrears.Tables[0], clsMarksArrears,
                    new string[] { "FKR", "РзПр;Рз+Пр", "EKR", "ПСР" }, false, arrearsCache, null, null, ClsProcessModifier.Arrears);
        }

        private void PumpOutcomesBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            int[] sections = null;
            if (xmlForm[0] == XmlForm.Form487)
                sections = new int[] { 1, 4 };
            else
                sections = new int[] { 2 };
            if (this.DataSource.Year >= 2005)
            {
                PumpComplexClsFromInternalPatternXML(
                    dsFKRBook.Tables[0], xnPattern, xmlForm, sections,
                    clsFKRBook, new string[] { "CODE", "0..4" }, true, fkrBookCache,
                    new string[] { "0000", "0..4" }, new string[] { "2..16" }, ClsProcessModifier.FKRBook);
                PumpComplexClsFromInternalPatternXML(
                    dsEKRBook.Tables[0], xnPattern, xmlForm, sections,
                    clsEKRBook, new string[] { "CODE", "4..6:000000" }, true, ekrBookCache,
                    new string[] { "!0000", "0..4", "000", "4..6" }, new string[] { "2..16" }, ClsProcessModifier.EKRBook);
            }
            else
            {
                PumpComplexClsFromInternalPatternXML(
                    dsFKRBook.Tables[0], xnPattern, xmlForm, sections,
                    clsFKRBook, new string[] { "CODE", "0..4" }, true, fkrBookCache,
                    new string[] { "0000", "0..4" }, new string[] { "5..20;44;51" }, ClsProcessModifier.FKRBook);
                PumpComplexClsFromInternalPatternXML(
                    dsEKRBook.Tables[0], xnPattern, xmlForm, sections,
                    clsEKRBook, new string[] { "CODE", "-1..6" }, true, ekrBookCache,
                    null, new string[] { "5..20;44;51" }, ClsProcessModifier.EKRBook);
            }
        }

        private void PumpOutcomesBooksAddClsFromIntPatternXML(XmlNode xnPattern)
        {
            XmlForm[] xmlForm = GetXmlFormBooksByFormatXML();
            int[] sections = null;
            if (xmlForm[0] == XmlForm.Form487)
                sections = new int[] { 1, 4 };
            else
                sections = new int[] { 2 };
            if (this.DataSource.Year >= 2010)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { };
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "8..11", "EKR", "22..24", "KCSR", "12..18", "KVR", "19..21" },
                    false, marksOutcomesCache, null, null, ClsProcessModifier.MarksOutcomes);
            }
            else if (this.DataSource.Year >= 2008)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487};
                sections = new int[] { 1 };
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, marksOutcomesCache, null, null, ClsProcessModifier.MarksOutcomes);
                sections = new int[] { 2 };
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, marksOutcomesCache, null, null, ClsProcessModifier.MarksOutcomes);
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                xmlForm = new XmlForm[] { XmlForm.Form487 };
                sections = new int[] { 1 };
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, marksOutcomesCache, null, null, ClsProcessModifier.MarksOutcomes);
                sections = new int[] { 4 };
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, marksOutcomesCache, null, null, ClsProcessModifier.MarksOutcomes);
            }
            else if (this.DataSource.Year >= 2005)
                PumpComplexClsFromInternalPatternXML(
                    dsMarksOutcomes.Tables[0], xnPattern, xmlForm, sections,
                    clsMarksOutcomes, new string[] { "FKR", "0..4", "EKR", "4..6" }, false,
                    marksOutcomesCache, null, new string[] { "17..75" }, ClsProcessModifier.MarksOutcomes);
            else
                PumpComplexClsFromInternalPatternXML(
                    xnPattern, xmlForm, sections, dsMarksOutcomes.Tables[0],
                    clsMarksOutcomes, new string[] { "FKR", "Рз+Пр", "EKR", "ПСР" }, false,
                    marksOutcomesCache, null, new string[] { "21..43;45..50;52..84" }, ClsProcessModifier.MarksOutcomes);
        }

        private void PumpExcessBooksClsFromIntPatternXML(XmlNode xnPattern)
        {
            if (this.DataSource.Year < 2008)
                return;
            XmlForm[] xmlForm = new XmlForm[] { XmlForm.Form487 };
            int[] sections = new int[] { 2 };
            if (this.DataSource.Year >= 2010)
            {
                sections = new int[] { };
                PumpComplexClsFromInternalPatternXML(dsMarksExcess.Tables[0], xnPattern, xmlForm, sections, clsMarksExcess,
                    new string[] { "FKR", "8..11", "EKR", "22..24", "KCSR", "12..18", "KVR", "19..21" },
                    false, excessCache, null, null, ClsProcessModifier.Excess);
            }
            else
            {
                PumpComplexClsFromInternalPatternXML(dsMarksExcess.Tables[0], xnPattern, xmlForm, sections, clsMarksExcess,
                    new string[] { "FKR", "3..6", "EKR", "17..19", "KCSR", "7..13", "KVR", "14..16" },
                    false, excessCache, null, null, ClsProcessModifier.Excess);
            }
        }

        private void PumpAccountClsFromIntPatternXML(XmlNode xnPattern)
        {
            if (this.DataSource.Year < 2011)
                return;
            XmlForm[] xmlForm = new XmlForm[] { XmlForm.Form428};
            int[] sections = new int[] { 4 };
            PumpComplexClsFromInternalPatternXML(dsMarksAccount.Tables[0], xnPattern, xmlForm, sections, clsMarksAccount,
                new string[] { "Code", "-1" },
                false, marksAccountCache, null, null, ClsProcessModifier.Account);
        }

        /// <summary>
        /// Закачивает шаблон нового формата
        /// </summary>
        /// <param name="xdReport">Шаблон</param>
        protected override void PumpInternalXMLPattern(XmlNode xnPattern)
        {
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bInnerFinSources))
                PumpSrcInFinClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bOuterFinSources))
                PumpSrcOutFinClsFromIntPatternXML(xnPattern);
            if ((ToPumpBlock(Block.bIncomesRefs)) || (ToPumpBlock(Block.bOutcomes)))
                PumpIncomesBooksClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bOutcomesRefs))
                PumpOutcomesBooksClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
                PumpOutcomesBooksAddClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
                PumpInDebtBooksClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
                PumpOutDebtBooksClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bArrearsRefs))
                PumpArrearsBooksClsFromIntPatternXML(xnPattern);
            if (ToPumpBlock(Block.bExcessRefs))
                PumpExcessBooksClsFromIntPatternXML(xnPattern);

            if (ToPumpBlock(Block.bAccount))
                PumpAccountClsFromIntPatternXML(xnPattern);

            UpdateData();
        }

        #endregion Функции закачки внутреннего шаблона

        #region Функции общей организации закачки блоков

        /// <summary>
        /// Возвращает массив форм хмл в зависимотси от формата отчета
        /// </summary>
        /// <param name="sectNo">Номер секциии</param>
        /// <param name="sectNo128">Номер секциии для 128 форм</param>
        private string[] GetXmlFormByFormatXML(string sectNo, string sectNo128)
        {
            string[] xmlForm;
            if (this.DataSource.Year >= 2008)
                sectNo128 = string.Empty;

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                case XmlFormat.October2005:
                    if (sectNo128 == string.Empty)
                    {
                        xmlForm = new string[] { "428;428v", sectNo };
                    }
                    else
                    {
                        xmlForm = new string[] { "428;428v", sectNo, "128;128v", sectNo128 };
                    }
                    break;

                default:
                    xmlForm = new string[] { "649;650", sectNo };
                    break;
            }

            return xmlForm;
        }

        /// <summary>
        /// Возвращает массив форм хмл в зависимотси от формата отчета
        /// </summary>
        /// <param name="sectNo">Номер секциии</param>
        private string[] GetXmlFormByFormatXML(string sectNo)
        {
            return GetXmlFormByFormatXML(sectNo, string.Empty);
        }

        /// <summary>
        /// Возвращает массив форм хмл в зависимотси от формата отчета
        /// </summary>
        private XmlForm[] GetXmlFormByFormatXML()
        {
            XmlForm[] xmlForm;

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                case XmlFormat.October2005:
                    xmlForm = new XmlForm[] { XmlForm.Form428, XmlForm.Form428v };
                    break;

                default:
                    xmlForm = new XmlForm[] { XmlForm.Form649, XmlForm.Form650 };
                    break;
            }

            return xmlForm;
        }

        /// <summary>
        /// Закачивает блок "Дефицит Профицит"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepDefProfXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);

            Dictionary<XmlForm, string[]> codeExclusion4XmlForm = new Dictionary<XmlForm, string[]>(3);
            string clsCodeAttr = string.Empty;

            // форма 127
            forcePumpForm127 = false;
            string[] formSection = new string[] { "127;127g;127v", "22" };
            PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, formSection,
                daMonthRepDefProf, dsMonthRepDefProf.Tables[0], fctMonthRepDefProf,
                null, null, null, null, null, progressMsg, null, codeExclusion4XmlForm,
                BlockProcessModifier.MRDefProf, clsCodeAttr, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 127, качаем из 428, 628

            string sectNo;

            // Code узла Form = 42802 или 428v02, закачиваем только те значения, для которых
            // значение в поле РзПр = 7900
            clsCodeAttr = "РзПр";
            string[] codeExclusions = new string[] { "!7900", "-1" };

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                case XmlFormat.Format2005:
                    sectNo = "3";
                    if (this.XmlReportFormat == XmlFormat.Format2004)
                    {
                        clsCodeAttr = string.Empty;
                        codeExclusions = null;
                    }
                    break;

                default:
                    sectNo = "2";
                    if (this.XmlReportFormat == XmlFormat.Skif3)
                    {
                        clsCodeAttr = string.Empty;
                        // Code узла Form = 42802 или 428v02, закачиваем только те значения, для которых
                        // значение 4-7 символов из ПППРзПрЦСРВРЭКР = 7900
                        codeExclusions = new string[] { "!7900", "3..6" };
                    }
                    break;
            }

            // Code узла Form = 12822 или 12822v, закачиваем только те значения, для которых РзПрЦСРВРЭКР = 00000000000000790
            codeExclusion4XmlForm.Add(XmlForm.Form128, new string[] { "!*790", "-1" });
            codeExclusion4XmlForm.Add(XmlForm.Form128v, new string[] { "!*790", "-1" });
            codeExclusion4XmlForm.Add(XmlForm.UnknownForm, codeExclusions);

            formSection = GetXmlFormByFormatXML(sectNo, "22");
            formSection = (string[])CommonRoutines.ConcatArrays(formSection, new string[] { "628r", "2" });
            PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, formSection,
                daMonthRepDefProf, dsMonthRepDefProf.Tables[0], fctMonthRepDefProf,
                null, null, null, null, null, progressMsg, null, codeExclusion4XmlForm,
                BlockProcessModifier.MRDefProf, clsCodeAttr, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 428, качаем из 117
            if (this.DataSource.Year >= 2008)
            {
                codeExclusion4XmlForm.Clear();
                formSection = new string[] { "117", "22" };
                PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, formSection,
                    daMonthRepDefProf, dsMonthRepDefProf.Tables[0], fctMonthRepDefProf,
                    null, null, null, null, null, progressMsg, null, codeExclusion4XmlForm,
                    BlockProcessModifier.MRDefProf, clsCodeAttr, regionCache, nullRegions, null,
                    region4PumpCache);
            }

            // форма 127
            forcePumpForm127 = true;
            codeExclusion4XmlForm = new Dictionary<XmlForm, string[]>(3);
            clsCodeAttr = string.Empty;
            formSection = new string[] { "127;127g;127v", "22" };
            PumpXMLReportBlock("Блок \"ДефицитПрофицит\"", xnReport, formSection,
            daMonthRepDefProf, dsMonthRepDefProf.Tables[0], fctMonthRepDefProf,
            null, null, null, null, null, progressMsg, null, codeExclusion4XmlForm,
            BlockProcessModifier.MRDefProf, clsCodeAttr, regionCache, nullRegions, null,
            region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepDefProf, ref dsMonthRepDefProf);

            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Доходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepIncomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);

            // форма 127
            forcePumpForm127 = false;
            string[] formSection = new string[] { "127;127g;127v", "1" };
            PumpXMLReportBlock("Блок \"Доходы\"", xnReport, formSection,
                daMonthRepIncomes, dsMonthRepIncomes.Tables[0], fctMonthRepIncomes,
                new DataTable[] { dsKD.Tables[0] },
                new IClassifier[] { clsKD },
                new int[] { },
                new string[] { "REFKD" },
                new int[] { nullKD },
                progressMsg,
                new Dictionary<string, int>[] { kdCache },
                new string[] { },
                BlockProcessModifier.MRIncomes, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 127 качаем в 428, 628
            formSection = GetXmlFormByFormatXML("1", "1");
            formSection = (string[])CommonRoutines.ConcatArrays(formSection, new string[] { "628r", "1" });
            PumpXMLReportBlock("Блок \"Доходы\"", xnReport, formSection,
                daMonthRepIncomes, dsMonthRepIncomes.Tables[0], fctMonthRepIncomes,
                new DataTable[] { dsKD.Tables[0] },
                new IClassifier[] { clsKD },
                new int[] { },
                new string[] { "REFKD" },
                new int[] { nullKD },
                progressMsg,
                new Dictionary<string, int>[] { kdCache },
                new string[] { "998*;999*", "-1" },
                BlockProcessModifier.MRIncomes, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 428, качаем из 117
            if (this.DataSource.Year >= 2008)
            {
                formSection = new string[] { "117", "1" };
                PumpXMLReportBlock("Блок \"Доходы\"", xnReport, formSection,
                    daMonthRepIncomes, dsMonthRepIncomes.Tables[0], fctMonthRepIncomes,
                    new DataTable[] { dsKD.Tables[0] },
                    new IClassifier[] { clsKD },
                    new int[] { },
                    new string[] { "REFKD" },
                    new int[] { nullKD },
                    progressMsg,
                    new Dictionary<string, int>[] { kdCache },
                    new string[] { },
                    BlockProcessModifier.MRIncomes, string.Empty, regionCache, nullRegions, null,
                    region4PumpCache);
            }

            // форма 127 - второй раз
            forcePumpForm127 = true;
            formSection = new string[] { "127;127g;127v", "1" };
            PumpXMLReportBlock("Блок \"Доходы\"", xnReport, formSection,
                daMonthRepIncomes, dsMonthRepIncomes.Tables[0], fctMonthRepIncomes,
                new DataTable[] { dsKD.Tables[0] },
                new IClassifier[] { clsKD },
                new int[] { },
                new string[] { "REFKD" },
                new int[] { nullKD },
                progressMsg,
                new Dictionary<string, int>[] { kdCache },
                new string[] { },
                BlockProcessModifier.MRIncomes, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepIncomes, ref dsMonthRepIncomes);

            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники внешнего финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepOutFinXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"ИсточникиВнешФин\".", TraceMessageKind.Information);

            // форма 127
            forcePumpForm127 = false;
            string[] formSection = new string[] { "127;127g;127v", "3" };
            PumpXMLReportBlock("Блок \"ИсточникиВнешФин\"", xnReport, formSection,
                daMonthRepOutFin, dsMonthRepOutFin.Tables[0], fctMonthRepOutFin,
                new DataTable[] { dsSrcOutFin.Tables[0] },
                new IClassifier[] { clsSrcOutFin },
                new int[] { },
                new string[] { "RefSOF" },
                new int[] { nullSrcOutFin },
                progressMsg,
                new Dictionary<string, int>[] { srcOutFinCache },
                BlockProcessModifier.MRSrcOutFin, "КИВнФ", regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в форме 127, качаем из 428, 628
            string sectNo;
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                case XmlFormat.Format2005:
                    sectNo = "5";
                    break;

                default:
                    sectNo = "3";
                    break;
            }

            formSection = GetXmlFormByFormatXML(sectNo, "03");
            formSection = (string[])CommonRoutines.ConcatArrays(formSection, new string[] { "628r", "3" });

            PumpXMLReportBlock("Блок \"ИсточникиВнешФин\"", xnReport, formSection,
                daMonthRepOutFin, dsMonthRepOutFin.Tables[0], fctMonthRepOutFin,
                new DataTable[] { dsSrcOutFin.Tables[0] },
                new IClassifier[] { clsSrcOutFin },
                new int[] { },
                new string[] { "RefSOF" },
                new int[] { nullSrcOutFin },
                progressMsg,
                new Dictionary<string, int>[] { srcOutFinCache },
                BlockProcessModifier.MRSrcOutFin, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 428, качаем из 117
            if (this.DataSource.Year >= 2008)
            {
                formSection = new string[] { "117", "3" };
                PumpXMLReportBlock("Блок \"ИсточникиВнешФин\"", xnReport, formSection,
                    daMonthRepOutFin, dsMonthRepOutFin.Tables[0], fctMonthRepOutFin,
                    new DataTable[] { dsSrcOutFin.Tables[0] },
                    new IClassifier[] { clsSrcOutFin },
                    new int[] { },
                    new string[] { "RefSOF" },
                    new int[] { nullSrcOutFin },
                    progressMsg,
                    new Dictionary<string, int>[] { srcOutFinCache },
                    BlockProcessModifier.MRSrcOutFin, "КИВнФ", regionCache, nullRegions, null,
                    region4PumpCache);
            }

            // форма 127 - второй раз
            forcePumpForm127 = true;
            formSection = new string[] { "127;127g;127v", "3" };
            PumpXMLReportBlock("Блок \"ИсточникиВнешФин\"", xnReport, formSection,
                daMonthRepOutFin, dsMonthRepOutFin.Tables[0], fctMonthRepOutFin,
                new DataTable[] { dsSrcOutFin.Tables[0] },
                new IClassifier[] { clsSrcOutFin },
                new int[] { },
                new string[] { "RefSOF" },
                new int[] { nullSrcOutFin },
                progressMsg,
                new Dictionary<string, int>[] { srcOutFinCache },
                BlockProcessModifier.MRSrcOutFin, "КИВнФ", regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutFin, ref dsMonthRepOutFin);

            WriteToTrace("Закачка Блок \"ИсточникиВнешФин\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники внутреннего финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepInFinXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"ИсточникиВнутрФин\".", TraceMessageKind.Information);

            // форма 127
            forcePumpForm127 = false;
            string[] formSection = new string[] { "127;127g;127v", "3" };
            PumpXMLReportBlock("Блок \"ИсточникиВнутрФин\"", xnReport, formSection,
                daMonthRepInFin, dsMonthRepInFin.Tables[0], fctMonthRepInFin,
                new DataTable[] { dsSrcInFin.Tables[0] },
                new IClassifier[] { clsSrcInFin },
                new int[] { },
                new string[] { "RefSIF" },
                new int[] { nullSrcInFin },
                progressMsg,
                new Dictionary<string, int>[] { srcInFinCache },
                BlockProcessModifier.MRSrcInFin, "КИВФ", regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 127, качаем из 428, 628
            string sectNo;
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                case XmlFormat.Format2005:
                    sectNo = "4";
                    break;

                default:
                    sectNo = "3";
                    break;
            }

            formSection = GetXmlFormByFormatXML(sectNo, "03");
            formSection = (string[])CommonRoutines.ConcatArrays(formSection, new string[] { "628r", "3" });

            PumpXMLReportBlock("Блок \"ИсточникиВнутрФин\"", xnReport, formSection,
                daMonthRepInFin, dsMonthRepInFin.Tables[0], fctMonthRepInFin,
                new DataTable[] { dsSrcInFin.Tables[0] },
                new IClassifier[] { clsSrcInFin },
                new int[] { },
                new string[] { "RefSIF" },
                new int[] { nullSrcInFin },
                progressMsg,
                new Dictionary<string, int>[] { srcInFinCache },
                BlockProcessModifier.MRSrcInFin, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // если нет в 428, качаем из 117
            if (this.DataSource.Year >= 2008)
            {
                formSection = new string[] { "117", "3" };
                PumpXMLReportBlock("Блок \"ИсточникиВнутрФин\"", xnReport, formSection,
                    daMonthRepInFin, dsMonthRepInFin.Tables[0], fctMonthRepInFin,
                    new DataTable[] { dsSrcInFin.Tables[0] },
                    new IClassifier[] { clsSrcInFin },
                    new int[] { },
                    new string[] { "RefSIF" },
                    new int[] { nullSrcInFin },
                    progressMsg,
                    new Dictionary<string, int>[] { srcInFinCache },
                    BlockProcessModifier.MRSrcInFin, "КИВФ", regionCache, nullRegions, null,
                    region4PumpCache);
            }

            // форма 127 - второй раз
            forcePumpForm127 = true;
            formSection = new string[] { "127;127g;127v", "3" };
            PumpXMLReportBlock("Блок \"ИсточникиВнутрФин\"", xnReport, formSection,
                daMonthRepInFin, dsMonthRepInFin.Tables[0], fctMonthRepInFin,
                new DataTable[] { dsSrcInFin.Tables[0] },
                new IClassifier[] { clsSrcInFin },
                new int[] { },
                new string[] { "RefSIF" },
                new int[] { nullSrcInFin },
                progressMsg,
                new Dictionary<string, int>[] { srcInFinCache },
                BlockProcessModifier.MRSrcInFin, "КИВФ", regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepInFin, ref dsMonthRepInFin);

            WriteToTrace("Закачка Блок \"ИсточникиВнутрФин\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Расходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepOutcomesXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);

            Dictionary<XmlForm, string[]> codeExclusion4XmlForm = new Dictionary<XmlForm, string[]>(3);
            // Code узла Form = 12812, 12812v. Code узла Form = 12822 или 12822v, не закачиваются значения,
            // где РзПрЦСРВРЭКР = 00000000000000790
            codeExclusion4XmlForm.Add(XmlForm.Form128, new string[] { "*790", "-1" });
            codeExclusion4XmlForm.Add(XmlForm.Form128v, new string[] { "*790", "-1" });
            // Code узла Form = 42802 или 428v02, не закачиваются значения, где РзПр=7900
            codeExclusion4XmlForm.Add(XmlForm.UnknownForm,
                new string[] { "#!9600;#!9800", "3..6", "7900", "3..6", "000", "17..19" });

            codeExclusion4XmlForm.Add(XmlForm.Form127, new string[] { "7900", "-1" });
            codeExclusion4XmlForm.Add(XmlForm.Form127g, new string[] { "7900", "-1" });
            codeExclusion4XmlForm.Add(XmlForm.Form127v, new string[] { "7900", "-1" });

            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                case XmlFormat.Skif3:

                    // форма 127
                    forcePumpForm127 = false;
                    string[] formSection = new string[] { "127;127g;127v", "12" };
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, formSection,
                        daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                        new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0], dsKVSR.Tables[0] },
                        new IClassifier[] { clsFKR, clsEKR, clsKVSR },
                        new int[] { 2005 },
                        new string[] { "REFFKR", "REFEKR", "RefKVSR" },
                        new int[] { nullFKR, nullEKR, nullKVSR },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrCache, ekrCache, kvsrCache },
                        codeExclusion4XmlForm, BlockProcessModifier.MROutcomes, string.Empty, regionCache, nullRegions,
                        new string[] { "0..1", "0..1", "0..1" },
                        region4PumpCache);

                    // если нет в 127 качаем из 428, 628
                    formSection = GetXmlFormByFormatXML("2", "12;22");
                    formSection = (string[])CommonRoutines.ConcatArrays(formSection, new string[] { "628r", "2" });

                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, formSection,
                        daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                        new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0] },
                        new IClassifier[] { clsFKR, clsEKR },
                        new int[] { 2005 },
                        new string[] { "REFFKR", "REFEKR" },
                        new int[] { nullFKR, nullEKR },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrCache, ekrCache },
                        codeExclusion4XmlForm, BlockProcessModifier.MROutcomes, string.Empty, regionCache, nullRegions,
                        new string[] { "3..6", "17..19" },
                        region4PumpCache);

                    // если нет в 428, качаем из 117
                    if (this.DataSource.Year >= 2008)
                    {
                        formSection = new string[] { "117", "12" };
                        codeExclusion4XmlForm.Clear();

                        PumpXMLReportBlock("Блок \"Расходы\"", xnReport, formSection,
                            daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                            new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0] },
                            new IClassifier[] { clsFKR, clsEKR },
                            new int[] { 2005 },
                            new string[] { "REFFKR", "REFEKR" },
                            new int[] { nullFKR, nullEKR },
                            progressMsg,
                            new Dictionary<string, int>[] { fkrCache, ekrCache },
                            codeExclusion4XmlForm, BlockProcessModifier.MROutcomes, string.Empty, regionCache, nullRegions,
                            new string[] { "0..1", "0..1" },
                            region4PumpCache);
                    }

                    // форма 127
                    forcePumpForm127 = true;
                    formSection = new string[] { "127;127g;127v", "12" };
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, formSection,
                        daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                        new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0], dsKVSR.Tables[0] },
                        new IClassifier[] { clsFKR, clsEKR, clsKVSR },
                        new int[] { 2005 },
                        new string[] { "REFFKR", "REFEKR", "RefKVSR" },
                        new int[] { nullFKR, nullEKR, nullKVSR },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrCache, ekrCache, kvsrCache },
                        codeExclusion4XmlForm, BlockProcessModifier.MROutcomes, string.Empty, regionCache, nullRegions,
                        new string[] { "0..1", "0..1", "0..1" },
                        region4PumpCache);

                    break;

                default:
                    PumpXMLReportBlock("Блок \"Расходы\"", xnReport, GetXmlFormByFormatXML("2"),
                        daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                        new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0] },
                        new IClassifier[] { clsFKR, clsEKR },
                        new int[] { 2005 },
                        new string[] { "REFFKR", "REFEKR" },
                        new int[] { nullFKR, nullEKR },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrCache, ekrCache },
                        BlockProcessModifier.MROutcomes, string.Empty, regionCache, nullRegions,
                        new string[] { "0..4", "0..0" },
                        region4PumpCache);
                    break;
            }

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomes, ref dsMonthRepOutcomes);

            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает массив форм хмл в зависимотси от формата отчета
        /// </summary>
        private string[] GetXmlFormBooksByFormatXML(string sectNo)
        {
            string[] xmlForm;

            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                case XmlFormat.October2005:
                    if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                        xmlForm = new string[] { "487", sectNo };
                    else
                        xmlForm = new string[] { "414", sectNo };
                    break;
                default:
                    xmlForm = new string[] { "651", sectNo };
                    break;
            }
            return xmlForm;
        }

        /// <summary>
        /// Возвращает массив форм хмл в зависимотси от формата отчета
        /// </summary>
        private XmlForm[] GetXmlFormBooksByFormatXML()
        {
            XmlForm[] xmlForm;
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Skif3:
                case XmlFormat.October2005:
                    if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                        xmlForm = new XmlForm[] { XmlForm.Form487 };
                    else
                        xmlForm = new XmlForm[] { XmlForm.Form414 };
                    break;
                default:
                    xmlForm = new XmlForm[] { XmlForm.Form651 };
                    break;
            }
            return xmlForm;
        }

        /// <summary>
        /// Закачивает блок "СправВнешнийДолг"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepOutDebtBooksXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"СправВнешнийДолг\".", TraceMessageKind.Information);
            string section = string.Empty;
            string[] formSection = null;
            if (this.DataSource.Year >= 2010)
                formSection = new string[] { "487", string.Empty };
            else
            {
                if (this.DataSource.Year >= 2008)
                    section = "2";
                else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    section = "3";
                else
                    section = "5";
                formSection = GetXmlFormBooksByFormatXML(section);
            }

            PumpXMLReportBlock("Блок \"СправВнешнийДолг\"", xnReport, formSection,
                daMonthRepOutDebtBooks, dsMonthRepOutDebtBooks.Tables[0], fctMonthRepOutDebtBooks,
                new DataTable[] { dsMarksOutDebt.Tables[0] },
                new IClassifier[] { clsMarksOutDebt },
                new int[] { 2005 },
                new string[] { "REFMARKSOUTDEBT" },
                new int[] { nullMarksOutDebt },
                progressMsg, new Dictionary<string, int>[] { scrOutFinSourcesRefCache },
                BlockProcessModifier.MRCommonBooks, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutDebtBooks, ref dsMonthRepOutDebtBooks);

            WriteToTrace("Закачка Блок \"СправВнешнийДолг\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправВнутреннийДолг"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepInDebtBooksXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"СправВнутреннийДолг\".", TraceMessageKind.Information);
            string section = string.Empty;
            string[] formSection = null;
            if (this.DataSource.Year >= 2010)
                formSection = new string[] { "487", string.Empty };
            else
            {
                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    section = "2";
                else
                    section = "4";
                formSection = GetXmlFormBooksByFormatXML(section);
            }
            PumpXMLReportBlock("Блок \"СправВнутреннийДолг\"", xnReport, formSection,
                daMonthRepInDebtBooks, dsMonthRepInDebtBooks.Tables[0], fctMonthRepInDebtBooks,
                new DataTable[] { dsMarksInDebt.Tables[0] },
                new IClassifier[] { clsMarksInDebt },
                new int[] { 2005 },
                new string[] { "REFMARKSINDEBT" },
                new int[] { nullMarksInDebt },
                progressMsg, new Dictionary<string, int>[] { scrInFinSourcesRefCache },
                BlockProcessModifier.MRCommonBooks, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepInDebtBooks, ref dsMonthRepInDebtBooks);

            WriteToTrace("Закачка Блок \"СправВнутреннийДолг\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправДоходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepIncomesBooksXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"СправДоходы\".", TraceMessageKind.Information);
            string [] formSection = null;
            if (this.DataSource.Year * 100 + this.DataSource.Month < 200510)
                formSection = new string[] { "651", "1" };
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                formSection = new string[] { "414", string.Empty };
            else
                formSection = new string[] { "414", "1" };
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                    PumpXMLReportBlock("Блок \"СправДоходы\"", xnReport, formSection,
                        daMonthRepIncomesBooks, dsMonthRepIncomesBooks.Tables[0], fctMonthRepIncomesBooks,
                        new DataTable[] { dsKVSR.Tables[0] },
                        new IClassifier[] { clsKVSR },
                        new int[] { 2004 },
                        new string[] { "REFKVSR" },
                        new int[] { nullKVSR },
                        progressMsg,
                        new Dictionary<string, int>[] { kvsrCache },
                        BlockProcessModifier.MRCommonBooks, string.Empty, regionCache, nullRegions, null,
                        region4PumpCache);
                    break;

                default:
                    PumpXMLReportBlock("Блок \"СправДоходы\"", xnReport, formSection,
                        daMonthRepIncomesBooks, dsMonthRepIncomesBooks.Tables[0], fctMonthRepIncomesBooks,
                        new DataTable[] { dsKVSR.Tables[0] },
                        new IClassifier[] { clsKVSR },
                        new int[] { 2005 },
                        new string[] { "REFKVSR" },
                        new int[] { nullKVSR },
                        progressMsg,
                        new Dictionary<string, int>[] { kvsrCache },
                        BlockProcessModifier.MRIncomesBooks, string.Empty, regionCache, nullRegions,
                        new string[] { "20..22" },
                        region4PumpCache);
                    break;
            }

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepIncomesBooks, ref dsMonthRepIncomesBooks);

            WriteToTrace("Закачка Блок \"СправДоходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправЗадолженность"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepArrearsBooksXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"СправЗадолженность\".", TraceMessageKind.Information);
            string section = string.Empty;
            string[] formSection = null;
            if (this.DataSource.Year >= 2010)
                formSection = new string[] { "487", string.Empty };
            else
            {
                if (this.DataSource.Year >= 2008)
                    section = "2";
                else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    section = "4";
                else
                    section = "6";
                formSection = GetXmlFormBooksByFormatXML(section);
            }
            PumpXMLReportBlock("Блок \"СправЗадолженность\"", xnReport, formSection,
                daMonthRepArrearsBooks, dsMonthRepArrearsBooks.Tables[0], fctMonthRepArrearsBooks,
                new DataTable[] { dsMarksArrears.Tables[0] },
                new IClassifier[] { clsMarksArrears },
                new int[] { 2005 },
                new string[] { "REFMARKSARREARS" },
                new int[] { nullMarksArrears },
                progressMsg, new Dictionary<string, int>[] { arrearsCache },
                BlockProcessModifier.MRCommonBooks, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepArrearsBooks, ref dsMonthRepArrearsBooks);

            WriteToTrace("Закачка Блок \"СправЗадолженность\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправРасходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepOutcomesBooksXML(string progressMsg, XmlNode xnReport)
        {
            // с февраля 2007 года секцию закачивать не надо
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                return;
            WriteToTrace("Старт закачки Блок \"СправРасходы\".", TraceMessageKind.Information);
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                    PumpXMLReportBlock("Блок \"СправРасходы\"", xnReport, GetXmlFormBooksByFormatXML("2"),
                        daMonthRepOutcomesBooks, dsMonthRepOutcomesBooks.Tables[0], fctMonthRepOutcomesBooks,
                        new DataTable[] { dsFKRBook.Tables[0], dsEKRBook.Tables[0] },
                        new IClassifier[] { clsFKRBook, clsEKRBook },
                        null,
                        new string[] { "REFFKR", "REFEKR" },
                        new int[] { nullFKRBook, nullEKRBook },
                        progressMsg,
                        new Dictionary<string, int>[] { fkrBookCache, ekrBookCache },
                        new string[] { "000", "4..6", "!005..020;044;051", "-1..3" },
                        BlockProcessModifier.MROutcomesBooks, string.Empty, regionCache, nullRegions,
                        new string[] { "0..4", "-1..6" },
                        region4PumpCache);
                    break;
                default:
                    if (this.DataSource.Year >= 2005)
                        PumpXMLReportBlock("Блок \"СправРасходы\"", xnReport, GetXmlFormBooksByFormatXML("2"),
                            daMonthRepOutcomesBooks, dsMonthRepOutcomesBooks.Tables[0], fctMonthRepOutcomesBooks,
                            new DataTable[] { dsFKRBook.Tables[0], dsEKRBook.Tables[0] },
                            new IClassifier[] { clsFKRBook, clsEKRBook },
                            null,
                            new string[] { "REFFKR", "REFEKR" },
                            new int[] { nullFKRBook, nullEKRBook },
                            progressMsg,
                            new Dictionary<string, int>[] { fkrBookCache, ekrBookCache },
                            new string[] { "000", "4..6", "!002..016", "-1..3" },
                            BlockProcessModifier.MROutcomesBooks, string.Empty, regionCache, nullRegions,
                            new string[] { "0..4", "4..6:000000" },
                            region4PumpCache);
                    else
                        PumpXMLReportBlock("Блок \"СправРасходы\"", xnReport, GetXmlFormBooksByFormatXML("2"),
                            daMonthRepOutcomesBooks, dsMonthRepOutcomesBooks.Tables[0], fctMonthRepOutcomesBooks,
                            new DataTable[] { dsFKRBook.Tables[0], dsEKRBook.Tables[0] },
                            new IClassifier[] { clsFKRBook, clsEKRBook },
                            null,
                            new string[] { "REFFKR", "REFEKR" },
                            new int[] { nullFKRBook, nullEKRBook },
                            progressMsg,
                            new Dictionary<string, int>[] { fkrBookCache, ekrBookCache },
                            new string[] { },
                            BlockProcessModifier.MROutcomesBooks, "Рз+Пр+ПСР", regionCache, nullRegions,
                            new string[] { "0..4", "4..6:000000" },
                            region4PumpCache);
                    break;
            }

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomesBooks, ref dsMonthRepOutcomesBooks);

            WriteToTrace("Закачка Блок \"СправРасходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправРасходыДоп"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="xnReport">Элемент хмл с данными</param>
        private void PumpMonthRepOutcomesBooksExXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"СправРасходыДоп\".", TraceMessageKind.Information);
            string section = string.Empty;
            if (this.DataSource.Year >= 2010)
            {
                PumpXMLReportBlock("Блок \"СправРасходыДоп\"", xnReport, new string[] { "487", string.Empty },
                    daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx.Tables[0], fctMonthRepOutcomesBooksEx,
                    new DataTable[] { dsMarksOutcomes.Tables[0] },
                    new IClassifier[] { clsMarksOutcomes },
                    new int[] { 2005 },
                    new string[] { "REFMARKSOUTCOMES" },
                    new int[] { nullMarksOutcomes },
                    progressMsg,
                    new Dictionary<string, int>[] { marksOutcomesCache },
                    new string[] { },
                    BlockProcessModifier.MROutcomesBooksEx, string.Empty, regionCache, nullRegions, null,
                    region4PumpCache);
            }
            else if (this.DataSource.Year >= 2008)
            {
                section = "1";
                PumpXMLReportBlock("Блок \"СправРасходыДоп\"", xnReport, GetXmlFormBooksByFormatXML(section),
                    daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx.Tables[0], fctMonthRepOutcomesBooksEx,
                    new DataTable[] { dsMarksOutcomes.Tables[0] },
                    new IClassifier[] { clsMarksOutcomes },
                    new int[] { 2005 },
                    new string[] { "REFMARKSOUTCOMES" },
                    new int[] { nullMarksOutcomes },
                    progressMsg,
                    new Dictionary<string, int>[] { marksOutcomesCache },
                    new string[] { },
                    BlockProcessModifier.MROutcomesBooksEx, string.Empty, regionCache, nullRegions, null,
                    region4PumpCache);
                UpdateData();
                section = "2";
                PumpXMLReportBlock("Блок \"СправРасходыДоп\"", xnReport, GetXmlFormBooksByFormatXML(section),
                    daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx.Tables[0], fctMonthRepOutcomesBooksEx,
                    new DataTable[] { dsMarksOutcomes.Tables[0] },
                    new IClassifier[] { clsMarksOutcomes },
                    new int[] { 2005 },
                    new string[] { "REFMARKSOUTCOMES" },
                    new int[] { nullMarksOutcomes },
                    progressMsg,
                    new Dictionary<string, int>[] { marksOutcomesCache },
                    new string[] { },
                    BlockProcessModifier.MROutcomesBooksEx, string.Empty, regionCache, nullRegions, null,
                    region4PumpCache);
                UpdateData();
            }
            else
            {
                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    section = "1;4";
                else
                    section = "2";
                PumpXMLReportBlock("Блок \"СправРасходыДоп\"", xnReport, GetXmlFormBooksByFormatXML(section),
                    daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx.Tables[0], fctMonthRepOutcomesBooksEx,
                    new DataTable[] { dsMarksOutcomes.Tables[0] },
                    new IClassifier[] { clsMarksOutcomes },
                    new int[] { 2005 },
                    new string[] { "REFMARKSOUTCOMES" },
                    new int[] { nullMarksOutcomes },
                    progressMsg,
                    new Dictionary<string, int>[] { marksOutcomesCache },
                    new string[] { },
                    BlockProcessModifier.MROutcomesBooksEx, string.Empty, regionCache, nullRegions, null,
                    region4PumpCache);
            }

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomesBooksEx, ref dsMonthRepOutcomesBooksEx);

            WriteToTrace("Закачка Блок \"СправРасходыДоп\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает районы из отчета формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected override bool PumpRegionsFromXMLReport(XmlNode xnReport)
        {
            return PumpRegionsXML(xnReport, dsRegions.Tables[0], clsRegions, regionCache,
                dsRegions4Pump.Tables[0], clsRegions4Pump, region4PumpCache);
        }

        private void PumpMonthRepExcessBooksXML(string progressMsg, XmlNode xnReport)
        {
            if (this.DataSource.Year < 2008)
                return;

            WriteToTrace("Старт закачки Блок \"СправОстатки\".", TraceMessageKind.Information);
            string section = string.Empty;
            if (this.DataSource.Year < 2010)
                section = "2";
            string[] formSection = new string[] { "487", section };
            PumpXMLReportBlock("Блок \"СправОстатки\"", xnReport, formSection,
                daMonthRepExcessBooks, dsMonthRepExcessBooks.Tables[0], fctMonthRepExcessBooks,
                new DataTable[] { dsMarksExcess.Tables[0] },
                new IClassifier[] { clsMarksExcess },
                new int[] { 2005 },
                new string[] { "RefMarks" },
                new int[] { nullMarksExcess },
                progressMsg, new Dictionary<string, int>[] { excessCache },
                BlockProcessModifier.MRExcessBooks, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepExcessBooks, ref dsMonthRepExcessBooks);

            WriteToTrace("Закачка Блок \"СправОстатки\" закончена.", TraceMessageKind.Information);
        }

        private void PumpMonthRepAccountXML(string progressMsg, XmlNode xnReport)
        {
            if (this.DataSource.Year < 2011)
                return;
            WriteToTrace("Старт закачки Блок \"КонсРасходы\".", TraceMessageKind.Information);
            string section = "4";
            string[] formSection = new string[] { "428", section };
            PumpXMLReportBlock("Блок \"КонсРасходы\"", xnReport, formSection,
                daMonthRepAccount, dsMonthRepAccount.Tables[0], fctMonthRepAccount,
                new DataTable[] { dsMarksAccount.Tables[0] },
                new IClassifier[] { clsMarksAccount },
                new int[] { 2005 },
                new string[] { "RefAccount" },
                new int[] { nullMarksAccount },
                progressMsg, new Dictionary<string, int>[] { marksAccountCache },
                BlockProcessModifier.MRAccount, string.Empty, regionCache, nullRegions, null,
                region4PumpCache);
            UpdateData();
            ClearDataSet(daMonthRepAccount, ref dsMonthRepAccount);

            WriteToTrace("Закачка Блок \"КонсРасходы\" закончена.", TraceMessageKind.Information);
        }

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
                            "Code", code, "Name", name, "KL", kl, "KST", kst, "SourceID", yearSourceID });
                    }
                }
            }
        }

        private void PumpMonthRepArrearsXML(string progressMsg, XmlNode xnReport)
        {
            WriteToTrace("Старт закачки Блок \"Задолженность\"", TraceMessageKind.Information);

            string[] formSections = new string[] { "159;169;159V;169V;469;459;469V;459V", string.Empty };
            PumpMonthRepAccount(xnReport, formSections[0]);
            PumpXMLReportBlock("Блок \"Задолженность\"", xnReport, formSections,
                daMonthRepArrears, dsMonthRepArrears.Tables[0], fctMonthRepArrears, new DataTable[] { dsAccount.Tables[0] },
                new IClassifier[] { clsAccount }, new int[] { }, new string[] { "RefAccount" },
                new int[] { nullAccount }, progressMsg, new Dictionary<string, int>[] { accountCache },
                BlockProcessModifier.MRArrears, "ПлСч;ПлСч11", regionCache, nullRegions, null, null);
            UpdateData();
            ClearDataSet(daMonthRepArrears, ref dsMonthRepArrears);

            WriteToTrace("Закачка Блок \"Задолженность\" закончена", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает отчет формата хмл
        /// </summary>
        /// <param name="xnReport">Элемент с данными отчета</param>
        protected override void PumpXMLReport(XmlNode xnReport, string progressMsg)
        {
            if (xnReport == null)
                return;
            if (ToPumpBlock(Block.bIncomes))
                PumpMonthRepIncomesXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bOutcomes))
                PumpMonthRepOutcomesXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bDefProf))
                PumpMonthRepDefProfXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bInnerFinSources))
                PumpMonthRepInFinXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bOuterFinSources))
                PumpMonthRepOutFinXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bIncomesRefs))
                PumpMonthRepIncomesBooksXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bOutcomesRefs))
                PumpMonthRepOutcomesBooksXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
                PumpMonthRepOutcomesBooksExXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
                PumpMonthRepInDebtBooksXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
                PumpMonthRepOutDebtBooksXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bArrearsRefs))
                PumpMonthRepArrearsBooksXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bExcessRefs))
                PumpMonthRepExcessBooksXML(progressMsg, xnReport);

            if (ToPumpBlock(Block.bAccount))
                PumpMonthRepAccountXML(progressMsg, xnReport);
            if (ToPumpBlock(Block.bArrears))
                PumpMonthRepArrearsXML(progressMsg, xnReport);
        }

        #endregion Функции общей организации закачки блоков
    }
}