using System;
using System.Collections.Generic;
using System.Xml;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Common.Xml;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Ось итогов
    /// </summary>
    public class TotalAxis : Axis
    {
        //Еденицы измерения форматов (если количество типов изменится, то их прийдется подредактировать)
        private string[] formatUnit = new string[] {"", "", "", "р.", "тыс.р.", "тыс.р.", "млн.р.", "млн.р.", 
            "млрд.р.", "млрд.р.", "%", "", "тыс.", "млн.", "млрд.", "", "", "", "", "", "", ""};

        private List<PivotTotal> totals;

        public List<PivotTotal> Totals
        {
            get { return totals; }
            set { totals = value; }
        }

        /// <summary>
        /// Меры, которые отображаются в отчете
        /// </summary>
        public List<PivotTotal> VisibleTotals
        {
            get { return GetVisibleTotals(); }
        }

        public override bool IsEmpty
        {
            get
            {
                if (this.ParentPivotData.PivotDataType == PivotDataType.Map)
                {
                    return (this.FieldSets.Count == 0);
                }

                return (this.VisibleTotals.Count == 0); 
            }
        }

        /// <summary>
        /// Существует ли хотя бы у одного показателя сортировка.
        /// </summary>
        public bool IsExistsSort
        {
            get
            {
                foreach (PivotTotal total in this.Totals)
                {
                    if (total.SortType != SortType.None)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Получить еденицы измерения указанного формата
        /// </summary>
        /// <param name="type">формат</param>
        /// <returns></returns>
        public string GetFormatUnit(FormatType type)
        {
            try
            {
                return this.formatUnit[(int)type];
            }
            catch
            {
                return string.Empty;
            }
        }

        public TotalAxis(Client.MDXExpert.Data.PivotData pivotData)
        {
            this.axisType = AxisType.atTotals;
            this.totals = new List<PivotTotal>();
            this.FieldSets = new FieldSetCollection(pivotData);
            this._parentPivotData = pivotData;
            this.objectType = PivotObjectType.poAxis;
        }

        public override void Clear()
        {
            Totals.Clear();
            FieldSets.Clear();
        }

        private List<PivotTotal> GetVisibleTotals()
        {
            List<PivotTotal> result = new List<PivotTotal>();
            if (this.Totals != null)
            {
                foreach(PivotTotal total in this.Totals)
                {
                    if (total.IsVisible)
                        result.Add(total);
                }
            }
            return result;
        }

        /// <summary>
        /// Создаем показатели, по информации из селсета
        /// </summary>
        /// <param name="measuresSectionInfo"></param>
        public void InitialByClsInfo(List<MeasuresSectionInfo> measuresSectionInfo)
        {
            List<PivotTotal> newTotals = new List<PivotTotal>();
            foreach (MeasuresSectionInfo sectionInfo in measuresSectionInfo)
            {
                foreach (MeasureInfo measureInfo in sectionInfo.MeasuresInfo)
                {
                    bool isExists = false;
                    foreach (PivotTotal total in newTotals)
                    {
                        if (total.UniqueName == measureInfo.UniqueName)
                        {
                            isExists = true;
                            break;
                        }
                    }
                    if (isExists)
                        continue;

                    PivotTotal newTotal = this.GetPivotTotal(measureInfo.UniqueName);
                    if (newTotal == null)
                    {
                        newTotal = new PivotTotal(this.ParentPivotData, measureInfo.UniqueName, 
                            measureInfo.Caption, false, string.Empty, string.Empty, MeasureFormulaType.Custom);
                    }
                    newTotals.Add(newTotal);
                }
            }
            this.totals = newTotals;
            this.RefreshMemberNames();
        }

        public bool TotalIsPresent(string totalName)
        {
            return (this.GetTotalByName(totalName) != null);
        }

        /// <summary>
        /// Получение итога по юникнейму
        /// </summary>
        /// <param name="uniqueName">юникнейм итога</param>
        /// <returns>итог, если такой есть, иначе - null</returns>
        public PivotTotal GetTotalByName(string uniqueName)
        {
            foreach (PivotTotal total in totals)
            {
                if (total.UniqueName == uniqueName)
                {
                    return total;
                }
            }

            return null;
        }

        /// <summary>
        /// Получение пользовательской меры по юникнейму
        /// </summary>
        /// <param name="uniqueName">юникнейм итога</param>
        /// <returns>итог, если такой есть, иначе - null</returns>
        public PivotTotal GetCustomTotalByName(string uniqueName)
        {
            foreach (PivotTotal total in totals)
            {
                if ((total.UniqueName == uniqueName) && (total.IsCustomTotal))
                {
                    return total;
                }
            }

            return null;
        }


        /*
        public override bool ObjectIsPresent(string objectName)
        {
            return TotalIsPresent(objectName);
        }
        */
        public override PivotObject GetPivotObject(string objectName)
        {
            PivotTotal total = GetTotalByName(objectName);
            return total;
        }

        public PivotTotal GetPivotTotal(string totalName)
        {
            PivotObject pivotObject = this.GetPivotObject(totalName);

            return (pivotObject != null) ? (PivotTotal)pivotObject : null;            
        }

        public void AddCalcTotal()
        {
            if (!this.ParentPivotData.CheckConnection())
                return;

            TotalExpressionForm cTotalFrm = new TotalExpressionForm(GetNewTotalCaption(), "0", this.ParentPivotData);

            if (cTotalFrm.ShowDialog() == DialogResult.OK)
            {
                PivotTotal total = new PivotTotal(this.ParentPivotData, cTotalFrm.TotalCaption, cTotalFrm.Expression);

                total.IsLookupMeasure = cTotalFrm.IsLookupMeasure;
                total.LookupCubeName = cTotalFrm.LookupCubeName;

                total.Filters.InnerXml = (cTotalFrm.Filters != null) ? cTotalFrm.Filters.InnerXml : null;

                total.MeasureSource = cTotalFrm.MeasureSource;
                total.FormulaType = cTotalFrm.FormulaType;

                this.Totals.Add(total);
                RefreshMemberNames();

                this.ParentPivotData.DoDataChanged();
            }
        }

        public void DeleteCalcTotal(string uniqueName)
        {
            foreach(PivotTotal total in this.Totals)
            {
                if((total.UniqueName == uniqueName)&&(total.IsCustomTotal))
                {
                    this.Totals.Remove(total);
                    RefreshMemberNames();
                    this.ParentPivotData.DoDataChanged();
                    return;
                }
            }
        }

        public void EditCalcTotal(PivotTotal total)
        {
            if (!this.ParentPivotData.CheckConnection())
                return;

            TotalExpressionForm expressionFrm = new TotalExpressionForm(total);
            if (expressionFrm.ShowDialog() == DialogResult.OK)
            {
                bool isDeferDataUpdating = this.ParentPivotData.IsDeferDataUpdating;
                this.ParentPivotData.IsDeferDataUpdating = true;
                total.MeasureSource = expressionFrm.MeasureSource;
                total.IsLookupMeasure = expressionFrm.IsLookupMeasure;
                total.LookupCubeName = expressionFrm.LookupCubeName;
                total.Filters.InnerXml = (expressionFrm.Filters != null) ? expressionFrm.Filters.InnerXml : null;
                total.FormulaType = expressionFrm.FormulaType;
                total.Caption = expressionFrm.TotalCaption;
                total.Expression = expressionFrm.Expression;
                this.ParentPivotData.IsDeferDataUpdating = isDeferDataUpdating;
                this.ParentPivotData.DoDataChanged();

                total.ParentPivotData.SetSelection(SelectionType.SingleObject, total.UniqueName);
            }

        }

        public string GetNewTotalCaption()
        {
            if (this.GetPivotTotal("[Measures].[" + Consts.defaultTotalCaption + "]") == null)
            {
                return Consts.defaultTotalCaption;
            }

            int i = 0;
            string newTotalCaption;

            do
            {
                newTotalCaption = String.Format("{0}({1})", Consts.defaultTotalCaption, i);
                i++;
            }
            while (this.GetPivotTotal("[Measures].[" + newTotalCaption + "]") != null);

            return newTotalCaption;
        }


        /// <summary>
        /// Добавление списка итогов в список узлов
        /// </summary>
        /// <param name="root">родительский узел</param>
        /// <param name="totalList">список узлов</param>
        protected void AddTotalListToNodes(XmlNode root, List<PivotTotal> totalList)
        {
            XmlNode totalNode;
            foreach (PivotTotal total in totalList)
            {
                totalNode = XmlHelper.AddChildNode(root, "total",
                                        new string[2] { "uname", total.UniqueName },
                                        new string[2] { "caption", total.Caption },
                                        new string[2] { "isCustomTotal", total.IsCustomTotal.ToString() },
                                        new string[2] { "isVisible", total.IsVisible.ToString() },
                                        new string[2] { "expression", total.Expression },
                                        new string[2] { "sortType", total.SortType.ToString() },
                                        new string[2] { "sorteredTupleUN", total.SortedTupleUN },
                                        new string[2] { "measureSource", total.MeasureSource },
                                        new string[2] { "formulaType", total.FormulaType.ToString() },
                                        new string[2] { "isLookupMeasure", total.IsLookupMeasure.ToString() }
                                        );

                if ((total.FormulaType != MeasureFormulaType.Custom) && (total.IsLookupMeasure))
                {
                    XmlHelper.SetAttribute(totalNode, "lookupCubeName", total.LookupCubeName);
                    if (total.Filters != null)
                    {
                        XmlNode filtersNode = XmlHelper.AddChildNode(totalNode, "filters", "", null);
                        filtersNode.InnerXml = total.Filters.InnerXml;
                    }


                }


                AddFormatToXmlNode(total.Format, totalNode);
            }
        }

        private void AddFormatToXmlNode(ValueFormat format, XmlNode node)
        {
            XmlHelper.AddChildNode(node, "format",
                                    new string[2] { "type", format.FormatType.ToString() },
                                    new string[2] { "digitcnt", format.DigitCount.ToString() },
                                    new string[2] { "thsdel", format.ThousandDelimiter.ToString() },
                                    new string[2] { "unitdisplay", format.UnitDisplayType.ToString() },
                                    new string[2] { "valuealign", format.ValueAlignment.ToString() }
                                    );
        }

        /// <summary>
        /// Получение списка итогов из узлов
        /// </summary>
        /// <param name="root">родительский узел для узлов, в кот. записаны юникнеймы</param>
        /// <returns>список юникнеймов</returns>
        protected List<PivotTotal> GetTotalListFromNodes(XmlNode root)
        {
            List<PivotTotal> result = new List<PivotTotal>();
            PivotTotal total;
            XmlNode formatNode;


            if (root != null)
            {
                foreach (XmlNode node in root.SelectNodes("total"))
                {
                    total = new PivotTotal(this.ParentPivotData, 
                                            XmlHelper.GetStringAttrValue(node, "uname", ""),
                                            XmlHelper.GetStringAttrValue(node, "caption", ""),
                                            XmlHelper.GetBoolAttrValue(node, "isCustomTotal", false),
                                            XmlHelper.GetStringAttrValue(node, "expression", ""),
                                            XmlHelper.GetStringAttrValue(node, "measureSource", ""),
                                            (MeasureFormulaType)Enum.Parse(typeof(MeasureFormulaType),
                                                XmlHelper.GetStringAttrValue(node, "formulaType", "Custom"))
                                            );

                    total.IsLookupMeasure = XmlHelper.GetBoolAttrValue(node, "isLookupMeasure", false);
                    total.IsVisible = XmlHelper.GetBoolAttrValue(node, "isVisible", true);

                    if ((total.IsLookupMeasure)&&(total.FormulaType != MeasureFormulaType.Custom))
                    {
                        total.LookupCubeName = XmlHelper.GetStringAttrValue(node, "lookupCubeName", "");
                        total.Filters = node.SelectSingleNode("filters");
                    }

                    //если в кубе есть мера с юникнеймом к у вычислимой меры, то берем меру из куба 
                    if ((total.IsCustomTotal) && this.ParentPivotData.CheckCubeObject(total))
                    {
                        total.IsCustomTotal = false;
                    }

                    //тип сортировки
                    total.SetSortTypeWithoutRefresh((SortType)Enum.Parse(typeof(SortType),
                        XmlHelper.GetStringAttrValue(node, "sortType", "None")));
                    total.SortedTupleUN = XmlHelper.GetStringAttrValue(node, "sorteredTupleUN", string.Empty);

                    formatNode = node.SelectSingleNode("format");
                    total.Format.IsMayHook = true;
                    total.Format.FormatType = (FormatType)Enum.Parse(typeof(FormatType), XmlHelper.GetStringAttrValue(formatNode, "type", "Auto"));
                    total.Format.DigitCount = (byte)XmlHelper.GetIntAttrValue(formatNode, "digitcnt", 2);
                    total.Format.ThousandDelimiter = XmlHelper.GetBoolAttrValue(formatNode, "thsdel", true);
                    total.Format.UnitDisplayType = (UnitDisplayType)Enum.Parse(typeof(UnitDisplayType), XmlHelper.GetStringAttrValue(formatNode, "unitdisplay", "DisplayAtValue"));
                    total.Format.ValueAlignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), XmlHelper.GetStringAttrValue(formatNode, "valuealign", "Far"));
                    total.Format.IsMayHook = false;

                    result.Add(total);
                }
            }

            return result;
        }

        public override XmlNode SaveSettingsXml(XmlNode parentNode)
        {
            if (this.ParentPivotData.PivotDataType == PivotDataType.Map)
            {
                SaveMapAxisXml(parentNode);
            }

            XmlNode root = XmlHelper.AddChildNode(parentNode, "totals", "", null);
            AddTotalListToNodes(root, Totals);
            return root;
        }

        private void SaveMapAxisXml(XmlNode parentNode)
        {
            XmlNode root = XmlHelper.AddChildNode(parentNode, "fieldsets", "", null); 
            XmlNode fieldSetNode, nameListNode, fieldListNode, fieldNode;

            foreach (FieldSet fieldSet in FieldSets)
            {
                fieldSetNode = XmlHelper.AddChildNode(root, "fieldset", 
                                                        new string[2] {"uname", fieldSet.UniqueName},
                                                        new string[2] {"caption", fieldSet.Caption});

                if (fieldSet.MemberNames != null)
                {
                    nameListNode = XmlHelper.AddChildNode(fieldSetNode, "membernames", "", null);
                    nameListNode.InnerXml = fieldSet.MemberNames.OuterXml;
                }

                fieldListNode = XmlHelper.AddChildNode(fieldSetNode, "fields", "", null);

                foreach (PivotField field in fieldSet.Fields)
                {
                    fieldNode = XmlHelper.AddChildNode(fieldListNode, "field", 
                                            new string[2] {"uname", field.UniqueName},
                                            new string[2] {"caption", field.Caption});
                    //Видимость итога у уровня
                }
            }
            
        }

        public override void SetXml(XmlNode node)
        {
            Totals = GetTotalListFromNodes(node.SelectSingleNode("totals"));

            if (this.ParentPivotData.PivotDataType == PivotDataType.Map)
            {
                SetMapAxisXml(node);
                return;
            }
            RefreshMemberNames();
        }

        public void SetMapAxisXml(XmlNode node)
        {
            FieldSet fieldSet;

            node = node.SelectSingleNode("fieldsets");
            //режим скрытия пустых
            if (node == null)
            {
                return;
            }

            XmlNodeList fieldSetNodeList = node.SelectNodes("fieldset");
            XmlNodeList fieldNodeList;
            PivotField f;
            string[] tmpStrList;

            foreach (XmlNode fieldSetNode in fieldSetNodeList)
            {
                fieldSet = new FieldSet(this.AxisType, this.ParentPivotData,
                                        XmlHelper.GetStringAttrValue(fieldSetNode, "uname", ""),
                                        XmlHelper.GetStringAttrValue(fieldSetNode, "caption", "")
                                        );

                fieldSet.MemberNames = fieldSetNode.SelectSingleNode("membernames/dummy");
                fieldNodeList = fieldSetNode.SelectNodes("fields/field");

                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    f = new PivotField(fieldSet, fieldSet.ParentPivotData,
                                        XmlHelper.GetStringAttrValue(fieldNode, "uname", ""),
                                        XmlHelper.GetStringAttrValue(fieldNode, "caption", ""));

                    fieldSet.Fields.Add(f);
                }

                FieldSets.Add(fieldSet);
            }
        }

        public void RefreshMemberNames()
        {
            //if (this.VisibleTotals.Count > 0)
            {
                FieldSet fs = FieldSets.GetFieldSetByName("[Measures]");

                if (this.VisibleTotals.Count == 0)
                {
                    if (fs != null)
                    {
                        this.FieldSets.Remove(fs);
                    }
                    return;
                }


                if (fs == null)
                {
                    fs = new FieldSet(this.AxisType, ParentPivotData, "[Measures]", "Меры");
                    FieldSets.Add(fs);
                }

                fs.Fields.Clear();
                PivotField f = new PivotField(fs, ParentPivotData, "[Measures].[MeasuresLevel]", "");
                fs.Fields.Add(f);

                XmlNode mbrNames = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
                XmlHelper.SetAttribute(mbrNames, "childrentype", "included");

                foreach (PivotTotal total in this.VisibleTotals)
                {
                    XmlHelper.AddChildNode(mbrNames, "member", new string[] { "uname", total.UniqueName });
                }
                fs.MemberNames = mbrNames;
            }
        }

        public void RefreshTypeSort(PivotObject sender)
        {
            foreach (PivotTotal total in this.Totals)
            {
                if (!total.Equals(sender))
                {
                    total.SetSortTypeWithoutRefresh(SortType.None);
                    total.SortedTupleUN = string.Empty;
                }
            }
        }

        public PivotTotal CopyTotal(PivotTotal total)
        {
            PivotTotal newTotal = new PivotTotal(this.ParentPivotData, total.UniqueName, total.Caption,
                                                 total.IsCustomTotal, total.Expression, total.MeasureSource, total.FormulaType);
            
            if ((total.IsLookupMeasure)&&(total.FormulaType != MeasureFormulaType.Custom))
            {
                newTotal.IsLookupMeasure = total.IsLookupMeasure;
                newTotal.LookupCubeName = total.LookupCubeName;
                newTotal.Filters = total.Filters.Clone();
            }
            newTotal.SetIsVisibleWithoutRefresh(total.IsVisible);

            this.Totals.Add(newTotal);
            return newTotal;
        }
    }
}
