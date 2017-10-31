using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Набор полей (иерархия).
    /// </summary>
    public class FieldSet : PivotObject, IEnumerable 
    {
        #region поля

        private AxisType _axisType;
        private string uniqueName;
        private string caption;
        private string _defaultMemberUN;
        private string _allMemberUN;
        private string _allLevelUN;
        private bool _usedInChartLabels = true;
        private FieldSetCollection parentCollection;
        private List<PivotField> fields;
        private XmlNode memberNames;
        private List<string> includedMemberNames;
        private List<string> excludedMembersNames;
        private DisplayMemberType _displayMemberType;
        private SortType _sortType;
        //private Hierarchy _adomdHierarchy;

        private List<string> exceptedMembers = new List<string>();
        //private MemberCaptionDictionary _memberCaptions;

        private MemberFilterCollection _memberFilters;

        #endregion

        #region Свойства

        public AxisType AxisType
        {
            get { return _axisType; }
            set { _axisType = value; }
        }

        /// <summary>
        /// Для диаграммы. Упоминать ли это измерение при формировании меток диаграммы
        /// </summary>
        public bool UsedInChartLabels
        {
            get { return _usedInChartLabels; }
            set { _usedInChartLabels = value; }
        }

        public FieldSetCollection ParentCollection
        {
            get { return parentCollection; }
            set { parentCollection = value; }
        }

        public string UniqueName
        {
            get 
            { 
                return uniqueName; 
            }
        }

        public string Caption
        {
            get 
            { 
                return caption; 
            }
            set 
            { 
                caption = value;
                this.ParentPivotData.DoAppearanceChanged(false);
            }
        }

        public List<PivotField> Fields
        {
            get { return fields; }
            set { fields = value; }
        }
        
        public XmlNode MemberNames
        {
            get { return memberNames; }
            set { memberNames = value; }
        }

        public bool ExistLevelAll
        {
            get { return this.AllMemberUN != string.Empty; }
        }
        
        /// <summary>
        /// Уникальное имя DefaultMember
        /// </summary>
        public string DefaultMemberUN
        {
            get { return _defaultMemberUN; }
            set { _defaultMemberUN = value; }
        }

        /// <summary>
        /// Уникальное имя элемента all
        /// </summary>
        public string AllMemberUN
        {
            get { return _allMemberUN; }
            set { _allMemberUN = value; }
        }

        /// <summary>
        /// Уникальное имя уровня all
        /// </summary>
        public string AllLevelUN
        {
            get { return _allLevelUN; }
            set { _allLevelUN = value; }
        }

        /// <summary>
        /// Если у измерения есть member All вернет его, иначе вернет DefaultMember
        /// </summary>
        public string GrandMemberUN
        {
            get { return this.AllMemberUN != string.Empty ? this.AllMemberUN : this.DefaultMemberUN; }
        }

        /// <summary>
        /// Видимость итогов
        /// </summary>
        public bool IsVisibleTotals
        {
            get { return this.GetVisibleTotals(); }
            set { this.SetVisibleTotals(value); }
        }

        /// <summary>
        /// Отображать дата мемберы
        /// </summary>
        public bool IsVisibleDataMembers
        {
            get { return this.GetVisibleDataMembers(); }
            set { this.SetVisibleDataMembers(value); }
        }

        /// <summary>
        /// Получить иереахию измерения
        /// </summary>
        public Hierarchy AdomdHierarchy
        {
            get { return this.ParentPivotData.GetAdomdHierarchy(this.UniqueName); }
        }

        /// <summary>
        /// Свойство влияет на результат метода GetMemberNames, имено от него завися какие 
        /// включенные или исключенные элементы станут результатом этого метода
        /// </summary>
        public DisplayMemberType DisplayMemberType
        {
            get { return _displayMemberType; }
            set 
            {
                if (value != _displayMemberType)
                {
                    _displayMemberType = value;
                    this.ParentPivotData.DoAppearanceChanged(true);
                }
            }
        }

        public SortType SortType
        {
            get { return _sortType; }
            set
            {
                if (_sortType != value)
                {
                    if ((this.PivotDataType == PivotDataType.Table)
                        && ((value == SortType.BDESC) || (value == SortType.BASC)))
                    {
                        DialogResult dialogResult = MessageBox.Show(Common.Consts.sortWarning, "MDXExpert ", System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.No)
                            return;
                    }

                    _sortType = value;
                    this.ParentPivotData.RefreshTypeSort(this);
                    if (!this.ParentPivotData.IsDeferDataUpdating)
                        this.ParentPivotData.DoDataChanged();
                }
            }
        }

        /// <summary>
        /// Отстортировано ли измерение, или один из его уровней
        /// </summary>
        public bool IsSorted
        {
            get
            {
                if (this.SortType != SortType.None)
                    return true;
                foreach (PivotField field in this.Fields)
                {
                    if (field.SortType != SortType.None)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Является ли измерение последним в оси
        /// </summary>
        public bool IsLastSetInAxis
        {
            get
            {
                return (this.ParentCollection.IndexOf(this) == this.ParentCollection.Count - 1);
            }
        }

        /// <summary>
        /// список исключенных элементов(точнее элементов, участвующих в выборке, но не предназначенных для отображения)
        /// </summary>
        public List<string> ExceptedMembers
        {
            get { return exceptedMembers; }
            set { exceptedMembers = value; }
        }

        public MemberFilterCollection MemberFilters
        {
            get { return _memberFilters; }
            set { _memberFilters = value; }
        }

        /*
        /// <summary>
        /// Пользовательские заголовки для элементов
        /// </summary>
        public MemberCaptionDictionary MemberCaptions
        {
            get { return _memberCaptions; }
            set { _memberCaptions = value; }
        }*/


        #endregion

        public FieldSet(Client.MDXExpert.Data.PivotData pivotData, string uniqueName, string caption)
            : this(AxisType.atNone, pivotData, uniqueName, caption)
        {
        }

        public FieldSet(AxisType axisType, Client.MDXExpert.Data.PivotData pivotData, string uniqueName, string caption)
        {
            fields = new List<PivotField>();
            this.uniqueName = uniqueName;
            this.caption = caption;
            this.AxisType = axisType;

            this._parentPivotData = pivotData;
            this.objectType = PivotObjectType.poFieldSet;
            //this._adomdHierarchy = this.ParentPivotData.GetAdomdHierarchy(this.UniqueName);

            this.DefaultMemberUN = this.GetDefaultMemberUN();
            this.AllLevelUN = this.GetAllLevelUN();
            this.AllMemberUN = this.GetAllMemberUN();
            this.includedMemberNames = new List<string>();
            this.excludedMembersNames = new List<string>();
            this.DisplayMemberType = DisplayMemberType.Auto;
            //this.MemberCaptions = new MemberCaptionDictionary();
            this._memberFilters = new MemberFilterCollection();
        }

        /// <summary>
        /// Устанавливает у всех уровней измерения видимость итогов
        /// </summary>
        /// <param name="value"></param>
        private void SetVisibleTotals(bool value)
        {
            bool isDeferUpdating = this.ParentPivotData.IsDeferDataUpdating;
            this.ParentPivotData.IsDeferDataUpdating = true;
            foreach (PivotField field in this.Fields)
            {
                field.IsVisibleTotal = value;
            }
            this.ParentPivotData.IsDeferDataUpdating = isDeferUpdating;
            this.ParentPivotData.DoDataChanged();
        }

        /// <summary>
        /// Устанавливает у всех уровней измерения видимость дата мемберов
        /// </summary>
        /// <param name="value"></param>
        private void SetVisibleDataMembers(bool value)
        {
            bool isDeferUpdating = this.ParentPivotData.IsDeferDataUpdating;
            this.ParentPivotData.IsDeferDataUpdating = true;
            foreach (PivotField field in this.Fields)
            {
                field.IsVisibleDataMember = value;
            }
            this.ParentPivotData.IsDeferDataUpdating = isDeferUpdating;
            this.ParentPivotData.DoDataChanged();
        }

        /// <summary>
        /// Получить список включеных/исключен элементов
        /// </summary>
        /// <param name="isOnlyName">выбираем возвращать с именами предков или только имена 
        /// самих элементов</param>
        /// <param name="maxCount">максимальное количество элементов</param>
        /// <returns></returns>
        private string GetMemberNames(bool isOnlyName, int maxCount)
        {
            string result = string.Empty;
            List<string> commentList = null;

            switch (this.DisplayMemberType)
            {
                case DisplayMemberType.Auto:
                    {
                        if (this.includedMemberNames.Count == 0)
                            commentList = this.excludedMembersNames;
                        else
                            if (this.excludedMembersNames.Count == 0)
                                commentList = this.includedMemberNames;
                            else
                                commentList = this.includedMemberNames.Count <= this.excludedMembersNames.Count ?
                                    this.includedMemberNames : this.excludedMembersNames;
                        break;
                    }
                case DisplayMemberType.Included:
                    commentList = this.includedMemberNames;
                    break;
                case DisplayMemberType.Excluded:
                    commentList = this.excludedMembersNames;
                    break;
            }

            for (int i = 0; (i <= maxCount) && (i < commentList.Count); i++)
            {
                if (result != string.Empty)
                    result += "\n";

                if (i == maxCount)
                    result += "...";
                else
                {
                    if (commentList.Equals(this.excludedMembersNames))
                        result += "Исключая: ";
                    result += isOnlyName ? this.GetLastMemberName(commentList[i]) : commentList[i];
                }
            }
            return (result == string.Empty) ? "(пусто)" : result;
        }

        /// <summary>
        /// Получить описание включенных элементов измерения
        /// </summary>
        /// <param name="maxCount">Максимальное количество элементов отображаемых в комментарии</param>
        /// <returns></returns>
        public string GetMembersComment(int maxMemberCount)
        {
            return this.GetMemberNames(false, maxMemberCount);
        }

        /// <summary>
        /// Если в измерении:
        ///     1)выбран один элемент, вернет его имя
        ///     2)выбранны все элементы, вернет имя AllМember, если его нет то вернет "(Все)"
        ///     3)если выбрано несколько элементов (больше одного) то вернет "Несколько элементов"
        ///     4)если выбраны все элементы кроме одного, вернет "Исключая: "имя исключенного элемента""
        /// </summary>
        /// <returns></returns>
        public string GetMembersDefinition(bool isCaptionIncludeParents, int maxMemberCount)
        {
            string result = "(несколько элементов)";
            if (this.MemberNames != null)
            {
                if ((this.includedMemberNames.Count == 1) && 
                    ((this.DisplayMemberType == DisplayMemberType.Included) || (this.DisplayMemberType == DisplayMemberType.Auto)))
                    result = isCaptionIncludeParents ? this.includedMemberNames[0] : this.GetLastMemberName(this.includedMemberNames[0]);
                else
                    if ((this.excludedMembersNames.Count == 1) &&
                    ((this.DisplayMemberType == DisplayMemberType.Excluded) || (this.DisplayMemberType == DisplayMemberType.Auto)))
                    {
                        result = "Исключая: ";
                        result += isCaptionIncludeParents ? this.excludedMembersNames[0] : this.GetLastMemberName(this.excludedMembersNames[0]);
                    }

                if (maxMemberCount != 1)
                {
                    result = this.GetMemberNames(!isCaptionIncludeParents, maxMemberCount);
                }
            }
            return result;
        }

        /// <summary>
        /// Получить ID первого включеного элемента
        /// </summary>
        /// <returns></returns>
        public string GetFirstIncludeMemberID()
        {
            XmlNode node = this.MemberNames.SelectSingleNode("member");
            return Krista.FM.Common.Xml.XmlHelper.GetStringAttrValue(node, "uname", string.Empty);
        }

        /// <summary>
        /// Получить ID влюченых элементов
        /// </summary>
        /// <returns></returns>
        public List<string> GetIncludeMembersID()
        {
            List<string> result = new List<string>();

            XmlNodeList nodes = this.MemberNames.SelectNodes("member");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    result.Add(Krista.FM.Common.Xml.XmlHelper.GetStringAttrValue(node, "uname", string.Empty));
                }
            }

            return result;
        }

        /// <summary>
        /// Инициализируем список включеных и выключеных элементов измерения.
        /// </summary>
        public void InitializeMemberNames()
        {
            //this.includedMemberNames.Clear();
            //this.excludedMembersNames.Clear();

            //Получаем имена включеных элементов
            this.ParentPivotData.GetMemberNames(this.AdomdHierarchy, this.MemberNames,
                                                this.includedMemberNames, this.excludedMembersNames);
        }

        /// <summary>
        /// Из цепоцки (родительских имен) возвращает последний кусок, тоесть имя самого элемента
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetLastMemberName(string source)
        {
            string result = source;
            if (result != string.Empty)
            {
                int start = result.LastIndexOf(Consts.memberNameSeparate);
                if (start != -1)
                {
                    start += Consts.memberNameSeparate.Length;
                    result = result.Substring(start, result.Length - start);
                }
            }
            return result;
        }

        /// <summary>
        /// Если хотя бы у одного уровня включена видимость итога вернет true, иначе false
        /// </summary>
        /// <returns></returns>
        private bool GetVisibleTotals()
        {
            foreach (PivotField field in this.Fields)
            {
                if (field.IsVisibleTotal)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Если хотя бы у одного уровня включена видимость дата мемберов вернет true, иначе false
        /// </summary>
        /// <returns></returns>
        private bool GetVisibleDataMembers()
        {
            foreach (PivotField field in this.Fields)
            {
                if (field.IsVisibleDataMember)
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            fields.Clear();
            memberNames = null;
        }

        public bool FieldIsPresent(string uniqueName)
        {
            foreach (PivotField f in Fields)
            {
                if (f.UniqueName == uniqueName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получение уровня по юникнейму
        /// </summary>
        /// <param name="uniqueName">юникнейм уровня</param>
        /// <returns>уровень, если такой есть, иначе - null</returns>
        public PivotField GetFieldByName(string uniqueName)
        {
            foreach (PivotField f in Fields)
            {
                if (f.UniqueName == uniqueName)
                {
                    return f;
                }
            }
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        /// <summary>
        /// Получить уникальное имя уровня All
        /// </summary>
        /// <returns></returns>
        private string GetAllLevelUN()
        {
            string result = string.Empty;
            try
            {
                Hierarchy h = this.ParentPivotData.GetAdomdHierarchy(this.UniqueName);
                if (h != null)
                {
                    if (h.Levels[0].Name == "(All)")
                    {
                        result = h.Levels[0].UniqueName;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Возвращает последний выбранный уровень в измерении
        /// </summary>
        /// <returns></returns>
        public PivotField GetLastField()
        {
            return (this.Fields.Count > 0) ? this.Fields[this.Fields.Count - 1] : null;
        }

        /// <summary>
        /// Вернет мембер по умолчанию
        /// </summary>
        /// <returns></returns>
        private string GetDefaultMemberUN()
        {
            string result = string.Format("{0}.DefaultMember", this.UniqueName);
            try
            {
                Hierarchy h = this.ParentPivotData.GetAdomdHierarchy(this.UniqueName);
                if (h != null)
                {
                    result = h.DefaultMember;
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Если в измерении есть уровень All то вернет UN этого мембера
        /// </summary>
        /// <returns></returns>
        private string GetAllMemberUN()
        {
            string result = string.Empty;
            try
            {
                Hierarchy h = this.ParentPivotData.GetAdomdHierarchy(this.UniqueName);
                if (h != null)
                {
                    if (h.Levels[0].Name == "(All)")
                    {
                        try
                        {
                            Level level = h.Levels[0];
                            result = level.GetMembers()[0].UniqueName;
                        }
                        catch(Exception exc)
                        {
                            this.ParentPivotData.DoAdomdExceptionReceived(exc);
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Если в измерении есть уровень All то вернет UN этого мембера, иначе 
        /// вернет мембер по умолчанию
        /// </summary>
        /// <returns></returns>
        private string GetGrandMemberUN()
        {
            string result = string.Format("{0}.DefaultMember", this.UniqueName);
            try
            {
                Hierarchy h = this.ParentPivotData.GetAdomdHierarchy(this.UniqueName);
                if (h != null)
                {
                    if (h.Levels[0].Name == "(All)")
                    {
                        try
                        {
                            Level level = h.Levels[0];
                            result = level.GetMembers()[0].UniqueName;
                        }
                        catch(Exception exc)
                        {
                            this.ParentPivotData.DoAdomdExceptionReceived(exc);
                        }
                    }
                    else
                    {
                        result = h.DefaultMember;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Множественный ли выбор у измерения
        /// (т.е выбрано ли более одного элемента)
        /// </summary>
        public bool IsMultipleChoise()
        {
            if (this.MemberNames == null)
                return false;

            //если есть упоминание исключаемых элементов - считаем что множественный
            string xPath = ".//*[@childrentype=\"excluded\"]";

            XmlNodeList nl = this.MemberNames.SelectNodes(xPath);
            if (nl.Count > 0)
            {
                return true;
            }
            //если есть упоминание исключаемых элементов - считаем что множественный
            foreach (XmlAttribute attribute in this.MemberNames.Attributes)
            {
                if (attribute.Value == "excluded")
                    return true;
            }

            //Если только списки включенных, тогда смотрим на кол-во листовых элементов
            nl = this.MemberNames.SelectNodes(".//*[(@uname) and not(*)]");
            //nl = this.MemberNames.SelectNodes(".//*[(@uname)]");
            return (nl.Count > 1);
        }

        /// <summary>
        /// Обновить сортировку у всех уровней измерения
        /// </summary>
        public void RefreshFieldSortType(PivotObject sender)
        {
            foreach (PivotField field in this.Fields)
            {
                if (field != sender)
                    field.SetSortTypeWithoutRefresh(SortType.None);
            }
        }

        /// <summary>
        /// Установка типа сортировки измерения без последующего возбуждения события об этом
        /// </summary>
        /// <param name="sortType"></param>
        public void SetSortTypeWithoutRefresh(SortType sortType)
        {
            this._sortType = sortType;
        }
    }

    public enum DisplayMemberType
    {
        [Description("Которых меньше")]
        Auto,
        [Description("Включенные")]
        Included,
        [Description("Выключенные")]
        Excluded
    }
}
