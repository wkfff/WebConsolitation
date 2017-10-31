using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinListView;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using AutoFitColumns=Infragistics.Win.UltraWinListView.AutoFitColumns;
using ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator;

//using ADOMD;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class MemberList : Form
    {
        private XmlNode memberNamesXML = null;
        //если редактор находится в режиме только для чтения
        private bool isReadOnly;
        //выставляется в true, когда идет загрузка данных
        private bool isLoadMembers;

        private MemberCollection mbrs = null;
        private List<string> memberProperties = new List<string>();

        //флаг, выставляется в true, когда идет процесс поиска элементов
        private bool findingMembers = false;

        private List<string> exceptedMembers;
        private FieldSet fieldSet = null;

        //текущий элемент в дереве
        private UltraTreeNode currentNode = null;

        private bool isUpdating = false;

        private PivotData pivotData;

        private bool isLookupCubeMembers;

        //максимальное количество элементов, загружаемых за 1 раз из базы
        private const int maxMembersCount = 10000;

        private MemberFilterCollection _memberFilters;

        private bool _isFiltering = false;

        private string _currLoadedMember;

        private int _loadedMembersCount;

        private MemberFilter _curMemberFilter;

        private bool _fieldSetIsFiltered;

        private Hierarchy currHierarchy;

        public bool FieldSetIsFiltered
        {
            get { return this._fieldSetIsFiltered; }
            set { this._fieldSetIsFiltered = value; }
        }

        /// <summary>
        /// Флаг - идет фильтрация элементов
        /// </summary>
        public bool IsFiltering
        {
            get { return _isFiltering; }
            set { _isFiltering = value; }
        }

        /// <summary>
        /// Текущий загружаемый элемент
        /// </summary>
        public string CurrLoadedMember
        {
            get { return _currLoadedMember; }
            set { _currLoadedMember = value; }
        }

        /// <summary>
        /// Количество загруженных элементов
        /// </summary>
        public int LoadedMembersCount
        {
            get { return _loadedMembersCount; }
        }

        public MemberList()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            splitContainer1.IsSplitterFixed = !gbMemberProperties.Expanded;
        }

        /// <summary>
        /// Тип дочерних элементов
        /// </summary>
        public enum XmlMemberChildrenType
        {
            ctIncluded,
            ctExcluded,
            ctNone
        }

        /// <summary>
        /// Структура для быстрого доступа к аттрибутам xml - узла
        /// </summary>
        public struct MemberTreeHelper
        {
            public static XmlMemberChildrenType GetChildrenType(XmlNode node)
            {
                string tmpStr = XmlHelper.GetStringAttrValue(node, "childrentype", "");

                XmlMemberChildrenType result;

                switch (tmpStr)
                {
                    case "included":
                        result = XmlMemberChildrenType.ctIncluded;
                        break;
                    case "excluded":
                        result = XmlMemberChildrenType.ctExcluded;
                        break;
                    default:
                        result = XmlMemberChildrenType.ctNone;
                        break;
                }
                return result;
            }
           
            public static XmlMemberChildrenType GetChildrenType(XmlNode rootXmlNode, UltraTreeNode node)
            {
                return GetChildrenType(GetXmlNodeByTreeNode(rootXmlNode, node)); 
            }

            public static string GetUniqueName(XmlNode node)
            {
                return XmlHelper.GetStringAttrValue(node, "uname", "");
            }

            public static CheckState GetCheckState(XmlNode node)
            {
                if (GetChildrenType(node) == XmlMemberChildrenType.ctNone)
                {
                    if (GetChildrenType(node.ParentNode) == XmlMemberChildrenType.ctIncluded)
                    {
                        return CheckState.Checked;
                    }
                    else
                    {
                        return CheckState.Unchecked;
                    }
                }
                else
                {
                    return CheckState.Indeterminate;
                }
            }
     
            public static CheckState GetCheckState(XmlNode rootXmlNode, UltraTreeNode node)
            {

                if (GetXmlNodeByTreeNode(rootXmlNode, node) != null)
                {
                    return GetCheckState(GetXmlNodeByTreeNode(rootXmlNode, node));
                }
                else
                {
                    return CheckState.Checked;
                }

            }

            public static string NormalizeUName(string uname)
            {
                uname = uname.Replace("'", "&apos;");
                uname = uname.Replace("\"", "&quot;"); 
                uname = uname.Replace("\r", "&gt;");
                uname = uname.Replace("\n", "&lt;");
                return uname;
            }

            public static void NormalizeMemberNames(ref XmlNode root)
            {
                if (root == null)
                {
                    return;
                }

                //XmlNodeList nl = root.SelectNodes(".//*[(@uname) and not(*)]");
                XmlNodeList nl = root.SelectNodes(".//*[(@uname)]");
                string uname = "";
                foreach (XmlNode node in nl)
                {
                    uname = XmlHelper.GetStringAttrValue(node, "uname", "");
                    uname = NormalizeUName(uname);
                    XmlHelper.SetAttribute(node, "uname", uname);
                }
            }
            
            public static XmlNode GetXmlNodeByTreeNode(XmlNode rootXmlNode, UltraTreeNode node)
            {
                if (node.Tag == null)
                {
                    return rootXmlNode;
                }

                if (rootXmlNode != null)
                {
                    //string uname = NormalizeUName(((Member)node.Tag).UniqueName);
                    string uname = NormalizeUName(((string)node.Tag));
                    return rootXmlNode.SelectSingleNode("//member[@uname = '" + uname + "']");
                }
                else
                {
                    return null;
                }
            }

            public static XmlNode GetXmlNodeByUniqueName(XmlNode rootXmlNode, string uniqueName)
            {
                try
                {
                    return rootXmlNode.SelectSingleNode("//member[@uname = '" + NormalizeUName(uniqueName) + "']");
                }
                catch (Exception exc)
                {
                    CommonUtils.ProcessException(exc);
                    return null;                
                }
            }
        }

        private Member GetMemberByUniqueName(string uniqueName)
        {
            if (this.fieldSet != null)
            {
                return PivotData.GetMemberByUniqueName(this.fieldSet.AdomdHierarchy, uniqueName);
            }
            else
                if (this.currHierarchy != null)
                {
                    return PivotData.GetMemberByUniqueName(this.currHierarchy, uniqueName);
                }

            return null;
        }

        /// <summary>
        /// Добавление фиктивного корня
        /// </summary>
        /// <returns>созданный корень</returns>
        private UltraTreeNode AddDummyRoot()
        {
            UltraTreeNode node = tvMembers.Nodes.Add("(Все)");
            node.Tag = null;

            if ((memberNamesXML == null)||(!memberNamesXML.HasChildNodes))
            {
                node.CheckedState = CheckState.Checked;
            }
            else
            {
                curCheckedNode = node;
                node.CheckedState = CheckState.Indeterminate;
                DoCheckParent(node);
                curCheckedNode = null;
            }

            return node;

        }

        /// <summary>
        /// Добавление корня дерева
        /// </summary>
        /// <param name="member">null - если узел фиктивный, иначе allmember</param>
        /// <returns>созданный корень</returns>
        private UltraTreeNode AddRoot(Member member)
        {
            UltraTreeNode node;

            if (member == null)
            {
                node = AddDummyRoot();
                node.Tag = null;
            }
            else
            {
                node = tvMembers.Nodes.Add(member.UniqueName);

                node.Text = member.Caption;
                //node.Tag = member;
                node.Tag = member.UniqueName;


                if ((this.fieldSet != null)&&(this.fieldSet.ParentPivotData.PivotDataType == PivotDataType.Chart))
                    node.Override.NodeAppearance.Image = this.exceptedMembers.Contains(member.UniqueName) ? 1 : 0;


                curCheckedNode = node;
                node.CheckedState = MemberTreeHelper.GetCheckState(memberNamesXML, node);
                curCheckedNode = null;
            }
            ceSearchRoot.Items.Add(node, node.FullPath);
            ceSearchRoot.SelectedIndex = 0;
            return node;
        }

        /// <summary>
        /// Обновление чекстейтов для узлов дерева
        /// </summary>
        /// <param name="root">родительский узел</param>
        private void RefreshCheckStates(UltraTreeNode root)
        {
            CheckState tmpCS = CheckState.Checked;

            if (MemberTreeHelper.GetChildrenType(memberNamesXML, root) == XmlMemberChildrenType.ctIncluded)
            {
                root.CheckedState = CheckState.Unchecked;
                tmpCS = CheckState.Checked;
            }
            else 
                if (MemberTreeHelper.GetChildrenType(memberNamesXML, root) == XmlMemberChildrenType.ctExcluded)
                {
                    root.CheckedState = CheckState.Checked;
                    tmpCS = CheckState.Unchecked;
                }

            foreach (UltraTreeNode node in root.Nodes)
            {
                //if (MemberTreeHelper.GetXmlNodeByUniqueName(memberNamesXML, ((Member)node.Tag).UniqueName) != null)
                if (MemberTreeHelper.GetXmlNodeByUniqueName(memberNamesXML, ((string)node.Tag)) != null)
                    {
                    if (MemberTreeHelper.GetChildrenType(memberNamesXML, node) == XmlMemberChildrenType.ctNone)
                    {
                        node.CheckedState = tmpCS;
                    }
                    else
                    {
                        curCheckedNode = node;
                        node.CheckedState = CheckState.Indeterminate;
                        DoCheckParent(node);
                        curCheckedNode = null;
                    }
                }
            }
        }

        private void BuildTreeByXml(XmlNode root, TreeNodesCollection parentNodes)
        {
            string uniqueName = MemberTreeHelper.GetUniqueName(root);
            if (uniqueName != "")
            {
                PivotData.GetNameFromUniqueName(uniqueName);
                UltraTreeNode node = new UltraTreeNode(String.Format("{0}{1}", uniqueName, root.GetHashCode()));
                node.Text = PivotData.GetNameFromUniqueName(uniqueName);
                node.CheckedState = CheckState.Checked;
                node.Tag = uniqueName;
                parentNodes.Add(node);
                parentNodes = node.Nodes;
            }

            foreach(XmlNode xmlNode in root.ChildNodes)
            {
                BuildTreeByXml(xmlNode, parentNodes);
            }
        }

        /// <summary>
        /// Инициализируем все элементы измерения, одновременно выставляя признаки включенности
        /// </summary>
        /// <param name="root"></param>
        /// <param name="mems"></param>
        /// <param name="dimInfo"></param>
        public void InitAllTreeNode(UltraTreeNode root, MemberCollection mems, DimensionInfo dimInfo)
        {
            //создаем корневой узел
            if (root == null)
            {
                if (mems.Count > 1)
                {
                    root = AddRoot(null);
                }
                else
                {
                    root = AddRoot(mems[0]);
                    mems = mems[0].GetChildren();
                }
                root.CheckedState = CheckState.Unchecked;
            }
            this.AddMemberToTree(root, mems);
            foreach (UltraTreeNode child in root.Nodes)
            {
                //Member member = (Member)child.Tag; 
                Member member = PivotData.GetMemberByUniqueName(mems[0].ParentLevel.ParentHierarchy, (string)child.Tag);
                if (member.ChildCount > 0)
                {
                    try
                    {
                        MemberCollection memCollection = member.GetChildren();
                        this.InitAllTreeNode(child, memCollection, dimInfo);
                    }
                    catch
                    {
                    }
                }

                if (dimInfo.ContainMember(member.UniqueName))
                {
                    //выставлять будем только листьям, если у элемента состояние отличается от Unchecked,
                    //значи у него есть дочерние элементы, включенность которых уже устанавливали
                    if (child.CheckedState == CheckState.Unchecked)
                    {
                        child.CheckedState = CheckState.Checked;
                        this.DoCheckParent(child);
                    }
                }
            }
        }

        /// <summary>
        /// Инициализировать и вернуть список включенных/выключенных элементов
        /// </summary>
        /// <param name="mems"></param>
        /// <param name="dimInfo"></param>
        /// <returns></returns>
        public XmlNode InitMemberList(MemberCollection mems, DimensionInfo dimInfo)
        {
            XmlNode result = null;
            //Загружаем дерево, устанавливаем чекбоксы с учетом нач значений
            tvMembers.Nodes.Clear();
            //Инициализируем дерево элементов
            this.InitAllTreeNode(null, mems, dimInfo);
            //Сохранаяем мембер лист
            this.SaveMemberNamesToXML(tvMembers.Nodes[0], ref result);
            return result;
        }

        
        /// <summary>
        /// Загрузка элементов дерева
        /// </summary>
        /// <param name="root">корень</param>
        /// <param name="mems">дочерние элементы для корня</param>
        private void LoadMembers(UltraTreeNode root, MemberCollection mems)
        {
            UltraTreeNode node = null;

            if (mems == null)
                return;

            if ((root == null) && (tvMembers.Nodes.Count > 0))
                return;

            try
            {
                isLoadMembers = true;
                //создаем корневой узел
                if (root == null)
                {
                    if (mems.Count > 1)
                    {
                        root = AddRoot(null);
                    }
                    else
                    {
                        root = AddRoot(mems[0]);
                        mems = mems[0].GetChildren();
                    }
                    LoadMembers(root, mems);
                    return;
                }

                CheckState rootCS = root.CheckedState;
                this.AddMemberToTree(root, mems);

                //устанавливаем чекстейты для только что загруженных узлов
                if (rootCS == CheckState.Indeterminate)
                {
                    RefreshCheckStates(root);
                }
                else
                {
                    root.CheckedState = CheckState.Indeterminate;
                    root.CheckedState = rootCS;
                   
                }
            }
            finally
            {
                isLoadMembers = false;
            }
        }
        
        /*
        /// <summary>
        /// Загрузка элементов дерева
        /// </summary>
        /// <param name="root">корень</param>
        /// <param name="mems">дочерние элементы для корня</param>
        private void LoadMembers(UltraTreeNode root, Member rootMember)
        {
            UltraTreeNode node = null;

            if (rootMember == null)
                return;
            
            if ((root == null) && (tvMembers.Nodes.Count > 0))
                return;

            try
            {
                isLoadMembers = true;
                //создаем корневой узел
                if (root == null)
                {
                    node = AddRoot(rootMember);
                    LoadMembers(node, rootMember);
                    return;
                }

                CheckState rootCS = root.CheckedState;
                this.AddMemberToTree(root, rootMember);

                //устанавливаем чекстейты для только что загруженных узлов
                if (rootCS == CheckState.Indeterminate)
                {
                    RefreshCheckStates(root);
                }
                else
                {
                    root.CheckedState = CheckState.Indeterminate;
                    root.CheckedState = rootCS;
                }
            }
            finally
            {
                isLoadMembers = false;
            }
        }
        */

        private void AddMemberToTree(UltraTreeNode root, MemberCollection members)
        {
            List<UltraTreeNode> nodes = new List<UltraTreeNode>();
            int i = 0;
            //добавляем элементы в дерево
            if (members != null)
                foreach (Member memb in members)
                {
                    try
                    {
                        i++;
                        //иногда бывает неск мемберов с одинаковыми юникнеймами(поетому ключ делаем составной)
                        UltraTreeNode node = new UltraTreeNode(String.Format("{0}{1}{2}", memb.UniqueName, memb.GetHashCode(), i));
                        nodes.Add(node);

                        node.Text = memb.Caption == "" ? "(Пусто)" : memb.Caption;
                        //node.Tag = memb;
                        node.Tag = memb.UniqueName;

                        if ((this.fieldSet != null) &&
                            (this.fieldSet.ParentPivotData.PivotDataType == PivotDataType.Chart))
                            node.Override.NodeAppearance.Image = this.exceptedMembers.Contains(memb.UniqueName)
                                                                     ? 1
                                                                     : 0;

                        if (memb.ChildCount > 0)
                        {
                            node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
                        }
                    }
                    catch
                    {

                    }

                }
            root.Nodes.AddRange(nodes.ToArray());

            // root.Nodes.AddRange(nodes);
        }

        /// <summary>
        /// Соответствует ли элемент фильтру измерения
        /// </summary>
        /// <param name="mbr"></param>
        /// <returns></returns>
        private bool MatchMember(Member mbr)
        {
            
            foreach(MemberFilter filter in this.fieldSet.MemberFilters)
            {
                bool result = false;

                result = (String.IsNullOrEmpty(filter.MemberName))
                             ? true
                             : mbr.Name.ToUpper().Contains(filter.MemberName.ToUpper());

                if (result)
                    result = CheckMemberProperties(mbr, filter.Properties);

                if (result)
                    return true;
            }
            return false;
        }

        private string[] GetPropertyNames(Level level)
        {
            List<string> props = new List<string>();

            foreach (MemberFilter filter in this.fieldSet.MemberFilters)
            {
                foreach (KeyValuePair<string, string> prop in filter.Properties)
                {
                    if (level.LevelProperties.Find(prop.Key) != null)
                    {
                        string propUName = level.LevelProperties[prop.Key].UniqueName;

                        if (!props.Contains(propUName))
                        {
                            props.Add(propUName);
                        }
                    }
                }
            }
            return props.ToArray();
        }
        /*
        private Microsoft.AnalysisServices.AdomdClient.MemberFilter[] GetMemberFilters()
        {
            List<string> props = new List<string>();

            foreach (MemberFilter filter in this.fieldSet.MemberFilters)
            {
                foreach (KeyValuePair<string, string> prop in filter.Properties)
                {
                    if (!props.Contains(prop.Key))
                    {
                        props.Add(prop.Key);
                    }
                }
            }
            return props.ToArray();
        }*/


        private bool CheckMemberProperties(Member member, Dictionary<string, string> properties)
        {
            if (properties.Count == 0)
                return true;

            foreach (KeyValuePair<string, string> prop in properties)
            {
                if (!CheckMemberPropertyValue(member, prop.Key, prop.Value))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Функция проверки, что лучше сохранять включенные или выключенные элементы
        /// </summary>
        /// <param name="node">начальный узел</param>
        /// <returns>true - если сохраняем включенные</returns>
        private bool IsSaveIncluded(UltraTreeNode node)
        {
            bool result = true;

            if (node.Parent != null)
            {
                int includedCount = 0;
                int excludedCount = 0;

                for (int i = 0; i < node.Parent.Nodes.Count; i++)
                {
                    if (node.Parent.Nodes[i].CheckedState == CheckState.Checked)
                    {
                        includedCount++;
                    }
                    else if (node.Parent.Nodes[i].CheckedState == CheckState.Unchecked)
                    {
                        excludedCount++;
                    }
                }
                if (includedCount == node.Parent.Nodes.Count)
                {
                    result = true;
                }
                else
                {
                    result = (includedCount <= excludedCount);
                }
            }

            return result;
        }

        private void tvMembers_AfterExpand(object sender, NodeEventArgs e)
        {
            InitNodeChildren(e.TreeNode);
        }

        private void InitNodeChildren(UltraTreeNode node)
        {
            if (this.FieldSetIsFiltered)
                return;

            if (node.HasNodes && (node.Nodes.Count != 0))
            {
                return;
            }

            //tvMembers.BeginUpdate();

            //Member member = (Member)e.TreeNode.Tag;
            Member member = GetMemberByUniqueName((string)node.Tag);

            if (member == null)
            {
                return;
            }

            if (member.ChildCount > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                LoadMembers(node, member.GetChildren());
            }
            else
            {
                node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
            }

            //tvMembers.EndUpdate();
            Cursor.Current = Cursors.Default;
        }


        #region  работа с чекбоксами 

        private UltraTreeNode curCheckedNode = null;

        private void tvMembers_AfterCheck(object sender, NodeEventArgs e)
        {
            if (curCheckedNode != null)
            {
                return;
            }

            curCheckedNode = e.TreeNode;

            if (curCheckedNode.CheckedState == CheckState.Indeterminate)
            {
                curCheckedNode.CheckedState = CheckState.Unchecked;
            }

            tvMembers.BeginUpdate();
            DoCheckChildren(e.TreeNode);
            DoCheckParent(e.TreeNode);
            tvMembers.EndUpdate();
            RefreshFindingResultsCheckStates();

            curCheckedNode = null;

            btOK.Enabled = this.GetEnabledBtOK();

            //CheckFoundedItem(e.TreeNode, lvSearchResult);
        }


        /// <summary>
        /// Чекаем элемент в списке найденных, который соответствует данному узлу
        /// </summary>
        /// <param name="node"></param>
        private void CheckFoundedItem(UltraTreeNode node, UltraListView lv)
        {
            if ((isLoadMembers) || (this._isFiltering) || (lv.Items.Count == 0) || (node == null))
                return;

            findingMembers = true;

            //Member mbr = (Member)node.Tag; 

            Member mbr = node.Tag != null ? GetMemberByUniqueName((string)node.Tag) : null;
            if (mbr != null)
            {
                string currNodePath = GetMemberPath(mbr);
                foreach (UltraListViewItem foundedItem in lv.Items)
                {
                    if (foundedItem.Text == currNodePath)
                    {
                        foundedItem.CheckState = node.CheckedState;
                        break;
                    }
                }
            }
            findingMembers = false;
        }

        /// <summary>
        /// Обновляем чекстейты результатов поиска
        /// </summary>
        private void RefreshFindingResultsCheckStates()
        {
            if ((isLoadMembers) || (this._isFiltering) || (lvSearchResult.Items.Count == 0))
                return;

            findingMembers = true;
            foreach(UltraListViewItem item in lvSearchResult.Items)
            {
                Member mbr = (Member) item.Tag;
                UltraTreeNode treeNode = GetTreeNodeByMember(mbr);
                if (treeNode != null)
                    item.CheckState = treeNode.CheckedState;
            }
            findingMembers = false;
        }

        private void DoCheckParent(UltraTreeNode node)
        {
            int uncheckedCnt = 0;
            int checkedCnt = 0;

            if (node.Parent != null)
            {
                for (int i = 0; i < node.Parent.Nodes.Count; i++ )
                {
                    switch (node.Parent.Nodes[i].CheckedState)
                    {
                        case CheckState.Unchecked:
                            uncheckedCnt++;
                            break;
                        case CheckState.Checked:
                            checkedCnt++;
                            break;
                    }
                }

                if ((uncheckedCnt == node.Parent.Nodes.Count) && (checkedCnt == 0))
                {
                    node.Parent.CheckedState = CheckState.Unchecked;
                }
                else if ((uncheckedCnt == 0) && (checkedCnt == node.Parent.Nodes.Count))
                {
                    node.Parent.CheckedState = CheckState.Checked;
                }
                else
                {
                    node.Parent.CheckedState = CheckState.Indeterminate;
                }
                /*
                if (this.FieldSetIsFiltered)
                {
                    //CheckFoundedItem(node.Parent, lvFilterResult);
                }
                else
                {
                    CheckFoundedItem(node.Parent, lvSearchResult);
                }*/
                DoCheckParent(node.Parent);
            }
        }

        private void DoCheckChildren(UltraTreeNode node)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckedState = node.CheckedState;
                /*
                if (this.FieldSetIsFiltered)
                {
                    //CheckFoundedItem(node.Parent, lvFilterResult);
                }
                else
                {
                    CheckFoundedItem(node.Nodes[i], lvSearchResult);
                }*/
                DoCheckChildren(node.Nodes[i]);
            }
        }

        #endregion

        private void btOK_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            if ((fieldSet != null)&&(!this.isLookupCubeMembers))
            {
                fieldSet.MemberNames = this.memberNamesXML.Clone();
            }
        }

        private void ApplyChanges()
        {
            if (tvMembers.Nodes.Count > 0)
            {
                XmlNode xmlNode = null;
                if (!FieldSetIsFiltered)
                {
                    SaveMemberNamesToXML(tvMembers.Nodes[0], ref xmlNode);
                }
                else
                {
                    SaveFilteredMemberNamesToXML(tvMembers.Nodes[0], ref xmlNode);
                }
                this.memberNamesXML = xmlNode;
                //SaveMembersXmlToFile(xmlNode, "666.xml");
            }
        }

        public void SetTopIsVisible()
        {
            if (this.Visible)
            {
                this.TopMost = true;
                this.TopMost = false;
                this.Focus();
                this.Activate();
                Application.DoEvents();
            }
        }

        private void MemberList_Shown(object sender, EventArgs e)
        {
            if (tvMembers.Nodes.Count > 0)
                tvMembers.Nodes[0].Expanded = true;
        }

        /// <summary>
        /// Установка координат формы в позиции курсора
        /// </summary>
        private void SetFormPosition()
        {
            int borderWidth = 5;
            int x = Cursor.Position.X - this.Width;
            int y = Cursor.Position.Y;

            if ((x + this.Width + borderWidth) > Screen.PrimaryScreen.WorkingArea.Width)
            {
                x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - borderWidth;
            }

            if ((y + this.Height + borderWidth) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                y = Screen.PrimaryScreen.WorkingArea.Height - this.Height - borderWidth;
            }

            if (x < borderWidth)
            {
                x = borderWidth;
            }

            this.Left = x;
            this.Top = y;
        }

        /// <summary>
        /// Получение списка элементов
        /// </summary>
        public void GetMemberNames(MemberCollection members, XmlNode mbrNames, List<string> included, 
            List<string> excluded, PivotData pivotData, FieldSet fs)
        {
            this.FieldSetIsFiltered = false;

            this.pivotData = pivotData;
            this.fieldSet = fs;

            ultraToolbarsManager.Toolbars[0].Tools["ClearMembers"].SharedProps.Enabled = false;


            this.isLookupCubeMembers = false;

            if (!this.isUpdating)
            {
                this.mbrs = members;

                //Загружаем начальные значения
                memberNamesXML = mbrNames.Clone();
                tvMembers.BeginUpdate();
                //Загружаем дерево, устанавливаем чекбоксы с учетом нач значений
                tvMembers.Nodes.Clear();
                MemberTreeHelper.NormalizeMemberNames(ref memberNamesXML);
                this.LoadMembers(null, members);
                tvMembers.EndUpdate();
            }

            if ((this.mbrs != null)&&(this.mbrs.Count > 0)&&(members != null)&&(members.Count > 0))
            {
                if (this.mbrs[0].ParentLevel.ParentHierarchy.UniqueName != members[0].ParentLevel.ParentHierarchy.UniqueName)
                    return;
            }
            included.Clear();
            excluded.Clear();
            this.SaveMemberNamesToString(tvMembers.Nodes[0], included, excluded, string.Empty);
        }

        /// <summary>
        /// Получение списка элементов
        /// </summary>
        /// <param name="members">Все элементы поля</param>
        /// <param name="mbrNames">Выбранные элементы поля</param>
        /// <param name="isSilentMode">если true - показывать форму для выбора мемберов, иначе просто получить список мемберов</param>
        /// <param name="isReadOnly">если true - не будет возможности применить изменения</param>
        /// <returns>true - если нажали ОК</returns>
        public bool GetMemberNames(MemberCollection members, ref FieldSet fs, bool isSilentMode, bool isReadOnly)
        {
            this.FieldSetIsFiltered = false;
            lvSearchResult.Items.Clear();
            ceSearchRoot.Items.Clear();
            this.pivotData = fs.ParentPivotData;
            this.isLookupCubeMembers = false;

            ultraToolbarsManager.Toolbars[0].Tools["ClearMembers"].SharedProps.Enabled = false;

            ultraTabControl.Tabs["SearchTab"].Visible = true;
            ultraTabControl.Tabs["FilterTab"].Visible = false;
            ultraTabControl.SelectedTab = ultraTabControl.Tabs["Members"];

            if ((members != null)&&(members.Count > 0))
            {
                GetMemberPropertyList(members[0]);
            }
            InitFilters(gbSearchOptions, gFilters);
            this.fieldSet = fs;
            this.exceptedMembers = fs.ExceptedMembers;

            this.mbrs = members;
            //Загружаем начальные значения
            memberNamesXML = fs.MemberNames.Clone();

            //Загружаем дерево, устанавливаем чекбоксы с учетом нач значений
            tvMembers.BeginUpdate();
            tvMembers.Nodes.Clear();
            MemberTreeHelper.NormalizeMemberNames(ref memberNamesXML);
            LoadMembers(null, members);

            tvMembers.EndUpdate();

            this.isReadOnly = isReadOnly;
            btOK.Enabled = this.GetEnabledBtOK();

            try
            {
                if (isSilentMode)
                {
                    ApplyChanges();
                    fs.MemberNames = memberNamesXML.Clone();
                    memberNamesXML = null;
                    return true;
                }
                else
                {
                    this.SetFormPosition();
                    btUpdate.Visible = true;
                    bool result = false;
                    if (this.ShowDialog() == DialogResult.OK)
                    {
                        fs.MemberNames = memberNamesXML.Clone();
                        memberNamesXML = null;
                        result = true;
                    }
                    tvMembers.Nodes.Clear();
                    return result;
                }
            }
            finally
            {
                this.isReadOnly = false;
            }
        }


        public bool GetMemberNames(Hierarchy h, ref XmlNode memberNames)
        {
            this.FieldSetIsFiltered = false;

            lvSearchResult.Items.Clear();
            ceSearchRoot.Items.Clear();
            btUpdate.Visible = false;
            this.isLookupCubeMembers = true;

            ultraToolbarsManager.Toolbars[0].Tools["ClearMembers"].SharedProps.Enabled = false;

            ultraTabControl.Tabs["SearchTab"].Visible = true;
            ultraTabControl.Tabs["FilterTab"].Visible = false;
            ultraTabControl.SelectedTab = ultraTabControl.Tabs["Members"];

            this.currHierarchy = h;

            MemberCollection members = h.Levels[0].GetMembers();

            if ((members != null) && (members.Count > 0))
            {
                GetMemberPropertyList(members[0]);
            }
            InitFilters(gbSearchOptions, gFilters);

            this.mbrs = members;
            
            //Загружаем начальные значения
            memberNamesXML = memberNames.Clone();

            //Загружаем дерево, устанавливаем чекбоксы с учетом нач значений
            tvMembers.BeginUpdate();
            tvMembers.Nodes.Clear();
            MemberTreeHelper.NormalizeMemberNames(ref memberNamesXML);
            LoadMembers(null, members);

            tvMembers.EndUpdate();

            this.isReadOnly = isReadOnly;
            btOK.Enabled = this.GetEnabledBtOK();

            try
            {
                this.SetFormPosition();
                bool result = false;
                if (this.ShowDialog() == DialogResult.OK)
                {
                    memberNames = memberNamesXML.Clone();
                    memberNamesXML = null;
                    result = true;
                }
                tvMembers.Nodes.Clear();
                return result;
            }
            finally
            {
                this.isLookupCubeMembers = false;
            }
        }

        public bool GetFilterMemberNames(MemberCollection members, ref FieldSet fs)
        {
            this.FieldSetIsFiltered = true;
            lvFilterResult.Items.Clear();
            btUpdate.Visible = false;
            this.isLookupCubeMembers = true;
            this.pivotData = fs.ParentPivotData;

            ultraToolbarsManager.Toolbars[0].Tools["ClearMembers"].SharedProps.Enabled = true;

            ultraTabControl.Tabs["SearchTab"].Visible = false;
            ultraTabControl.Tabs["FilterTab"].Visible = true;
            ultraTabControl.SelectedTab = ultraTabControl.Tabs["FilterTab"];

            if ((members != null) && (members.Count > 0))
            {
                GetMemberPropertyList(members[0]);
            }
            InitFilters(gbPropertiesFilter, gProperties);

            this.mbrs = members;

            this.fieldSet = fs;
            //Загружаем начальные значения
            memberNamesXML = fs.MemberNames.Clone();

            //Загружаем дерево, устанавливаем чекбоксы с учетом нач значений
            tvMembers.BeginUpdate();
            tvMembers.Nodes.Clear();
            MemberTreeHelper.NormalizeMemberNames(ref memberNamesXML);

            BuildTreeByXml(memberNamesXML, tvMembers.Nodes);
            tvMembers.EndUpdate();

            this.isReadOnly = isReadOnly;
            btOK.Enabled = this.GetEnabledBtOK();
            pFilterProperties.Visible = false;
            InitMemberFilters();

            try
            {
                this.SetFormPosition();
                bool result = false;
                if (this.ShowDialog() == DialogResult.OK)
                {
                    fs.MemberNames = memberNamesXML.Clone();
                    memberNamesXML = null;
                    result = true;
                }
                tvMembers.Nodes.Clear();
                return result;
            }
            finally
            {
                this.isLookupCubeMembers = false;

            }
        }


        private bool GetEnabledBtOK()
        {
            return (tvMembers.Nodes.Count > 0)&&(!this.isReadOnly && (tvMembers.Nodes[0].CheckedState != CheckState.Unchecked));
        }

        private void SaveMemberNamesToString(UltraTreeNode node, List<string> included, 
            List<string> excluded, string parentName)
        {
            while (node != null)
            {
                string localParent = string.Empty;
                if (node.CheckedState == CheckState.Indeterminate)
                {
                    node.Expanded = true;
                    localParent = parentName;

                    if (localParent != string.Empty)
                        localParent += Consts.memberNameSeparate;

                    if (node.Parent != null)
                        localParent += node.Text;
                    
                    if (node.Nodes.Count > 0)
                        this.SaveMemberNamesToString(node.Nodes[0], included, excluded, localParent);
                }

                string nodeName = parentName + ((parentName != string.Empty) ? 
                    Consts.memberNameSeparate + node.Text : node.Text);

                if (node.CheckedState == CheckState.Checked)
                    included.Add(nodeName);
                else
                    if (node.CheckedState == CheckState.Unchecked)
                        excluded.Add(nodeName);

                node = node.GetSibling(NodePosition.Next);
            }
        }

        /// <summary>
        /// Сохранение настроек дерева отфильтрованных элементов в узел
        /// </summary>
        /// <param name="node">корень дерева</param>
        /// <param name="xmlRoot">xml-узел, в который сохраняются настройки</param>
        private void SaveFilteredMemberNamesToXML(UltraTreeNode node, ref XmlNode xmlRoot)
        {
            XmlNode xmlNode = null;

            if (node == null)
            {
                return;
            }

            if (xmlRoot == null)
            {
                xmlRoot = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
                SaveFilteredMemberNamesToXML(node, ref xmlRoot);
                return;
            }

            if (node.Tag == null)
            {
                SaveFilteredMemberNamesToXML(node.Nodes[0], ref xmlRoot);
                return;
            }

            XmlHelper.SetAttribute(xmlRoot, "childrentype", "included");

            while (node != null)
            {
                if ((node.HasNodes) && ((node.CheckedState == CheckState.Checked) || (node.CheckedState == CheckState.Indeterminate)))
                {
                    xmlNode = XmlHelper.AddChildNode(xmlRoot, "member",
                                                     new string[] {"uname", ((string)node.Tag)});
                    SaveFilteredMemberNamesToXML(node.Nodes[0], ref xmlNode);
                }
                else
                if (node.CheckedState == CheckState.Checked)
                {
                    XmlHelper.AddChildNode(xmlRoot, "member", new string[] { "uname", ((string)node.Tag)});
                }

                node = node.GetSibling(NodePosition.Next);
            }
        }

        /// <summary>
        /// Сохранение настроек дерева элементов в узел
        /// </summary>
        /// <param name="node">корень дерева</param>
        /// <param name="xmlRoot">xml-узел, в который сохраняются настройки</param>
        private void SaveMemberNamesToXML(UltraTreeNode node, ref XmlNode xmlRoot)
        {
            XmlNode xmlNode = null;

            if (node == null)
            {
                return;
            }

            if (xmlRoot == null)
            {
                xmlRoot = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
                SaveMemberNamesToXML(node, ref xmlRoot);
                return;
            }

            if (node.Tag == null)
            {
                SaveMemberNamesToXML(node.Nodes[0], ref xmlRoot);
                return;
            }

            bool isMembersIncluded = false;

            string hierarchyName = GetMemberHierarchyName((string)node.Tag);
            if (hierarchyName == "[Measures]")
            //if (((Member)node.Tag).ParentLevel.ParentHierarchy.UniqueName == "[Measures]")
            {
                isMembersIncluded = true;
            }
            else
            {
                isMembersIncluded = IsSaveIncluded(node);
            }

            if (isMembersIncluded)
            {
                XmlHelper.SetAttribute(xmlRoot, "childrentype", "included");
            }
            else
            {
                XmlHelper.SetAttribute(xmlRoot, "childrentype", "excluded");
            }

            while (node != null)
            {
                if (node.CheckedState == CheckState.Indeterminate)
                {
                    node.Expanded = true;
                    xmlNode = XmlHelper.AddChildNode(xmlRoot, "member",
                                                     new string[] { "uname", ((string)node.Tag) });
                    SaveMemberNamesToXML(node.Nodes[0], ref xmlNode);
                }

                if (((node.CheckedState == CheckState.Checked) && (isMembersIncluded)) ||
                    ((node.CheckedState == CheckState.Unchecked) && (!isMembersIncluded)))
                {
                    xmlNode = XmlHelper.AddChildNode(xmlRoot, "member",
                                                     new string[] { "uname", ((string)node.Tag) });
                }

                node = node.GetSibling(NodePosition.Next);
            }
        }



        public void SaveMembersXmlToFile(XmlNode xmlNode, string fileName)
        {
            string fullfileName = string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath),
                fileName);
            FileStream stream = null;

            stream = new FileStream(fullfileName, FileMode.Create, FileAccess.Write, FileShare.None);

            XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));
            xmlFormat.Serialize(stream, xmlNode);
            stream.Close();
        }

        private void MemberList_Resize(object sender, EventArgs e)
        {
            //Refresh();
            //Update();
        }

        private void tvMembers_BeforeCheck(object sender, BeforeCheckEventArgs e)
        {
            if (this.isReadOnly && !this.isLoadMembers)
                e.Cancel = true;
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            this.lvSearchResult.ViewSettingsDetails.AutoFitColumns = AutoFitColumns.ResizeAllColumns;  
            FindMembers();
        }

        private void btStopSearch_Click(object sender, EventArgs e)
        {
            StopSearch();
        }


        #region Поиск элементов

        /// <summary>
        /// Начать поиск элементов
        /// </summary>
        private void FindMembers()
        {
            btSearch.Visible = false;
            this.findingMembers = true;

            lvSearchResult.Items.Clear();
            this.lvSearchResult.ViewSettingsDetails.CheckBoxStyle = CheckBoxStyle.CheckBox;

            DataTable dt = (DataTable)gFilters.DataSource;

            UltraTreeNode searchRootNode = (UltraTreeNode)ceSearchRoot.SelectedItem.DataValue;
            MemberCollection members = this.mbrs;
            /*
            if (searchRootNode.Tag != null)
            {
                //Member mbr = GetMemberByUniqueName((string) searchRootNode.Tag);
                 members = ((Member) searchRootNode.Tag).GetChildren();
                
            }*/

            FindMembers(members, teSearchStr.Text, GetPropertiesFromTable(dt));
            this.findingMembers = false;

            lCurrMember.Text = "";
            btSearch.Visible = true;
            if (this.lvSearchResult.Items.Count == 0)
            {
                this.lvSearchResult.ViewSettingsDetails.CheckBoxStyle = CheckBoxStyle.None;
                this.lvSearchResult.Items.Add("members not found", "< Не найдено ни одного элемента >");
            }
            SetSearchResultColumnWidth(lvSearchResult);
        }

        /// <summary>
        /// Остановить поиск
        /// </summary>
        private void StopSearch()
        {
            this.findingMembers = false;
            lCurrMember.Text = "";
            btSearch.Visible = true;
        }

        /// <summary>
        /// поиск элементов по подстроке
        /// </summary>
        /// <param name="members">элементы, в которых ищем (включая все дочерние)</param>
        /// <param name="searchStr">подстрока в загловке элемента, по которой ищем</param>
        private void FindMembers(MemberCollection members, string searchStr, List<string[]> properties)
        {
            if ((members == null)||(members.Count == 0))
            {
                return;
            }

            try
            {
                foreach (Member mbr in members)
                {
                    if (!this.findingMembers)
                        return;

                    string currMemberPath = GetMemberPath(mbr);
                    lCurrMember.Text = currMemberPath;
                    Application.DoEvents();
                    //поиск элемента по имени
                    //если имя в поиске задано пустое, то будут попадать все элементы
                    if (Regex.Match(mbr.Caption.ToUpper(), searchStr.ToUpper()).Success)
                    {
                        //поиск по значениям свойств
                        //если фильтров по значениям свойств нет, то будут попадать все элементы, 
                        //которые нашлись по имени
                        if (CheckMemberProperties(mbr, properties))
                        {
                            UltraTreeNode treeNode = GetTreeNodeByMember(mbr);
                            if (treeNode != null)
                            {
                                UltraListViewItem item =
                                    lvSearchResult.Items.Add(mbr.UniqueName + lvSearchResult.Items.Count.ToString(),
                                                             currMemberPath);
                                item.Tag = mbr;
                                item.CheckState = treeNode.CheckedState;
                            }
                        }
                    }
                    int i = 0;
                    int childCount = 0;
                    do
                    {


                        MemberCollection mbrs = mbr.GetChildren(i, maxMembersCount);

                        FindMembers(mbrs, searchStr, properties);

                        childCount = mbrs.Count;
                        i = +childCount;

                    } while (childCount == maxMembersCount);
                }
            }
            catch(Exception exc)
            {

                
            }
            
    }


        private void LoadFilteredMembers(Level level)
        {
            try
            {
                int i = 0;
                int k = 0;
                int childCount = 0;
                List<UltraTreeNode> filteredNodes = new List<UltraTreeNode>();
                string parentName = "";
                UltraTreeNode parentNode = null;
                MemberCollection mbrs;
                string[] props = GetPropertyNames(level);
                Microsoft.AnalysisServices.AdomdClient.MemberFilter[] memFilters =
                    new Microsoft.AnalysisServices.AdomdClient.MemberFilter[0];

                do
                {
                    mbrs = level.GetMembers(i, maxMembersCount, props, memFilters);

                    filteredNodes.Clear();
                    Application.DoEvents();

                    foreach (Member mbr in mbrs)
                    {
                        this._currLoadedMember = mbr.Name;
                        this.lCurFiltMember.Text = mbr.UniqueName;
                        k++;
                        if ((k%1000) == 0)
                            Application.DoEvents();

                        if (!this._isFiltering)
                        {
                            AddNodeList(parentNode, filteredNodes, level);
                            return;
                        }

                        if (MatchMember(mbr))
                        {
                            this._loadedMembersCount++;

                            UltraTreeNode treeNode;
                            if (parentName == GetParentMemberUniqueName(mbr.UniqueName))
                            {
                                treeNode = GetNodeForMember(mbr, true, k);

                                if (treeNode != null)
                                    filteredNodes.Add(treeNode);
                            }
                            else
                            {
                                parentName = GetParentMemberUniqueName(mbr.UniqueName);
                                AddNodeList(parentNode, filteredNodes, level);
                                filteredNodes.Clear();

                                treeNode = GetNodeForMember(mbr, false, k);
                                if (treeNode != null)
                                {
                                    parentNode = treeNode.Parent;
                                }
                                else
                                {
                                    parentNode = null;
                                }
                            }

                        }
                    }

                    AddNodeList(parentNode, filteredNodes, level);

                    childCount = mbrs.Count;
                    i += childCount;
                } while (childCount == maxMembersCount);

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }

        private void AddNodeList(UltraTreeNode parentNode, List<UltraTreeNode> nodes, Level lev)
        {
            if ((nodes != null) && (nodes.Count > 0))
            {
                if (parentNode != null)
                {
                    parentNode.Nodes.AddRange(nodes.ToArray());
                }
                else
                {
                    int mbrLevel = (lev.ParentHierarchy.Levels[0].LevelType == LevelTypeEnum.All)
                        ? lev.LevelNumber : lev.LevelNumber + 1;
    
                    if (GetTreeNodeLevel(parentNode) != mbrLevel)
                        return;

                    tvMembers.Nodes.AddRange(nodes.ToArray());
                }
            }
        }

        /*
        private void LoadFilteredMembers(MemberCollection members)
        {
            int i = 0;
            int childCount = 0;
            List<UltraTreeNode> filteredNodes = new List<UltraTreeNode>();
            UltraTreeNode parentNode = null;

            foreach (Member mbr in members)
            {

                if (MatchMember(mbr))
                {
                    filteredNodes.Add(GetNodeForMember(mbr, false, i, null));
                }

                do
                {
                    //MemberCollection mbrs = mbr.GetChildren() (i, maxMembersCount);
                    LoadFilteredMembers(mbrs);
                    Application.DoEvents();
                    filteredNodes.Clear();
                    childCount = mbrs.Count;
                    i += childCount;

                } while (childCount == maxMembersCount);

            }
            tvMembers.Nodes.AddRange(filteredNodes.ToArray());

        }
        */
    

        private UltraTreeNode GetNodeForMember(Member mbr, bool parentSameAsPrevious, int nodeNumber)
        {
            UltraTreeNode node = null;
            if (parentSameAsPrevious)
            {
                node = new UltraTreeNode(String.Format("{0}{1}{2}", mbr.UniqueName, mbr.GetHashCode(), nodeNumber));
                node.Text = mbr.Caption == "" ? "(Пусто)" : mbr.Caption;
                node.Tag = mbr.UniqueName;
                this.curCheckedNode = node;
                node.CheckedState = CheckState.Checked;
                this.curCheckedNode = null;
                return node;
            }


            List<Member> parentMembers = new List<Member>();
            while (mbr != null)
            {
                parentMembers.Add(mbr);
                mbr = mbr.Parent;
            }
            parentMembers.Reverse();
            
            TreeNodesCollection nodes = tvMembers.Nodes;
            foreach (Member member in parentMembers)
            {
                node = GetNodeByUniqueName(nodes, member.UniqueName);
                if (node == null)
                {
                    int mbrLevel = (member.ParentLevel.ParentHierarchy.Levels[0].LevelType == LevelTypeEnum.All)
                        ? member.ParentLevel.LevelNumber : member.ParentLevel.LevelNumber + 1;
                    //int mbrLevel = member.ParentLevel.LevelNumber;
                    if (GetTreeNodesLevel(nodes) != mbrLevel)
                        return null;

                    node = new UltraTreeNode(String.Format("{0}{1}{2}", member.UniqueName, member.GetHashCode(), nodeNumber));
                    node.Text = member.Caption == "" ? "(Пусто)" : member.Caption;
                    node.Tag = member.UniqueName;
                    nodes.Add(node);
                    this.curCheckedNode = node;
                    node.CheckedState = CheckState.Checked;

                    this.curCheckedNode = null;
                }
                nodes = node.Nodes;
            }
            return node;
        }

        /// <summary>
        /// Возвращает уровень коллекции узлов
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private int GetTreeNodesLevel(TreeNodesCollection nodes)
        {
            return (nodes.ParentNode == null) ? 0 : nodes.ParentNode.Level + 1;
        }

        /// <summary>
        /// Возвращает уровень узла
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private int GetTreeNodeLevel(UltraTreeNode node)
        {
            return (node == null) ? 0 : node.Level + 1;
        }


        private UltraTreeNode GetNodeByUniqueName(TreeNodesCollection nodes, string uniqueName)
        {
            foreach (UltraTreeNode node in nodes)
            {
                if ((string)node.Tag == uniqueName)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Проверка значения свойства элемента
        /// </summary>
        private bool CheckMemberPropertyValue(Member member, string propertyName, string propertyValue)
        {
            if ((member.MemberProperties.Find(propertyName) == null) ||
                (member.MemberProperties[propertyName].Value == null))
                return false;

            string pValue = member.MemberProperties[propertyName].Value.ToString();

            return Regex.Match(pValue.ToUpper(), propertyValue.ToUpper()).Success;
            //return (pValue.ToUpper() == propertyValue.ToUpper());
        }

        /// <summary>
        /// Проверка значения свойства элемента
        /// </summary>
        private bool CheckMemberPropertyValue2(Member member, string propertyName, string propertyValue)
        {
            if ((member.Properties.Find(propertyName) == null) ||
                (member.Properties[propertyName].Value == null))
                return false;

            string pValue = member.Properties[propertyName].Value.ToString();

            return Regex.Match(pValue.ToUpper(), propertyValue.ToUpper()).Success;
            //return (pValue.ToUpper() == propertyValue.ToUpper());
        }


        /// <summary>
        /// Проверяем проходит элемент под все фильтры свойств или нет
        /// </summary>
        /// <param name="member">элемент</param>
        /// <param name="properties">список свойств. Свойство в форме массива, первый элемент массива - имя свойства, второй - значение</param>
        /// <returns></returns>
        private bool CheckMemberProperties(Member member, List<string[]>properties)
        {
            if (properties.Count == 0)
                return true;

            member.FetchAllProperties();
            foreach(string[] prop in properties)
            {
                if (!CheckMemberPropertyValue2(member, prop[0], prop[1]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Получение списка свойств из DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<string[]> GetPropertiesFromTable(DataTable table)
        {
            List<string[]> properties = new List<string[]>();
            foreach(DataRow row in table.Rows)
            {
                if ((row["Свойство"] == DBNull.Value) || (row["Значение"] == DBNull.Value))
                    continue;

                string[] prop = new string[2];
                prop[0] = (string)row["Свойство"];
                prop[1] = (string)row["Значение"];
                properties.Add(prop);
            }
            return properties;
        }

        /// <summary>
        /// Получение списка свойств из DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDictPropertiesFromTable(DataTable table)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (DataRow row in table.Rows)
            {
                if ((row["Свойство"] == DBNull.Value) || (row["Значение"] == DBNull.Value))
                    continue;
                if (!properties.ContainsKey((string)row["Свойство"]))
                    properties.Add((string)row["Свойство"], (string)row["Значение"]);
            }
            return properties;
        }



        /// <summary>
        /// получение полного пути элемента
        /// </summary>
        /// <param name="mbr">элемент, путь которого надо получить</param>
        /// <returns>путь к элементу</returns>
        private string GetMemberPath(Member mbr)
        {
            string result = "";
            try
            {
                if (mbr != null)
                {
                    result = mbr.Caption;
                    mbr = mbr.Parent;
                }

                while (mbr != null)
                {
                    result = mbr.Caption + "\\" + result;
                    mbr = mbr.Parent;
                }
            }
            catch (Exception exc)
            {
                if (this.pivotData != null)
                {
                    this.pivotData.DoAdomdExceptionReceived(exc);
                }
                else
                {
                    CommonUtils.ProcessException(exc);

                }
            }


            return result;
        }

        /// <summary>
        /// получение полного пути элемента 
        /// </summary>
        /// <param name="mbr">элемент, путь которого надо получить</param>
        /// <returns>путь к элементу</returns>
        private List<string> GetSplittedMemberPath(Member mbr)
        {
            List<string> result = new List<string>();
            try
            {
                while (mbr != null)
                {
                    result.Insert(0, mbr.Caption);
                    mbr = mbr.Parent;
                }
            }
            catch (Exception exc)
            {
                if (this.pivotData != null)
                {
                    this.pivotData.DoAdomdExceptionReceived(exc);
                }
            }

            return result;
        }

        /// <summary>
        /// Получение имени иерархии из имени мембера
        /// </summary>
        /// <returns></returns>
        private string GetMemberHierarchyName(string uniqueName)
        {
            string[] splitName = uniqueName.Split('.');
            return splitName[0];
        }

        /// <summary>
        /// Получение имени иерархии из имени мембера
        /// </summary>
        /// <returns></returns>
        private string GetParentMemberUniqueName(string uniqueName)
        {
            int lastDotPos = uniqueName.LastIndexOf("].[");
            if (lastDotPos > 0)
            {
                return uniqueName.Substring(0, lastDotPos + 1);
            }
            return "";
        }


        private UltraTreeNode GetTreeNodeByCaption(TreeNodesCollection nodes, string caption)
        {
            foreach(UltraTreeNode node in nodes)
            {
                if (node.Text == caption)
                    return node;
            }
            return null;
        }

        private UltraTreeNode GetTreeNodeByMember(Member member)
        {
            List<string> captions = GetSplittedMemberPath(member);

            TreeNodesCollection nodes = tvMembers.Nodes;

            if ((nodes.Count > 0) && (nodes[0].Tag == null))
                captions.Insert(0, "(Все)");


            UltraTreeNode node = null;

            foreach(string caption in captions)
            {
                node = GetTreeNodeByCaption(nodes, caption);
                if (node != null)
                {
                    if (!node.HasNodes)
                    {
                        tvMembers.BeginUpdate();
                        //Member mbr = (Member)node.Tag;
                        Member mbr = GetMemberByUniqueName((string)node.Tag);
                        if ((mbr != null)&&(!this.FieldSetIsFiltered))
                            LoadMembers(node, mbr.GetChildren());
                        tvMembers.EndUpdate();
                    }

                    nodes = node.Nodes;
                }
                else
                {
                    return null;
                }
            }

            return node;

        }

        /// <summary>
        /// Установить ширину столбца в результатах поиска по самому длинному элементу 
        /// </summary>
        private void SetSearchResultColumnWidth(UltraListView lv)
        {
            lv.ViewSettingsDetails.AutoFitColumns = AutoFitColumns.Default;  

            float maxWidth = this.lvSearchResult.Width - 4;

            foreach (UltraListViewItem item in lv.Items)
            {
                SizeF size = lv.CreateGraphics().MeasureString(item.Text, lv.Font);
                if (size.Width + 35 > maxWidth)
                {
                    maxWidth = size.Width + 35;
                }
            }
            lv.MainColumn.Width = (int)maxWidth;

        }
        
        private void lvSearchResult_ItemCheckStateChanged(object sender, ItemCheckStateChangedEventArgs e)
        {
            if ((findingMembers)||(this._isFiltering))
                return;

            Member mbr = (Member) e.Item.Tag;// GetMemberByUniqueName((string)e.Item.Tag);
            if (mbr != null)
            {
                UltraTreeNode node = GetTreeNodeByMember(mbr);
                if (node != null)
                {
                    node.CheckedState = e.Item.CheckState;
                }
            }
        }

        #endregion


        private void gbSearchOptions_ExpandedStateChanged(object sender, EventArgs e)
        {
            lvSearchResult.Top = gbSearchOptions.Top + gbSearchOptions.Height + 10;
            lvSearchResult.Height = searchTab.Height - lvSearchResult.Top - 20;
        }

        private void gbMemberProperties_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (gbMemberProperties.Expanded)
            {
                int sDistance = splitContainer1.SplitterDistance;
                this.Width = 350 + ultraTabControl.Width + 5;
                splitContainer1.SplitterDistance = sDistance;
                splitContainer1.IsSplitterFixed = false;
            }
            else
            {
                splitContainer1.IsSplitterFixed = true;
                this.Width = splitContainer1.SplitterDistance + gbMemberProperties.Width + 10;
            }
        }

        private void tvMembers_AfterSelect(object sender, SelectEventArgs e)
        {
            if(e.NewSelections.Count > 0)
            {
                if (e.NewSelections[0].Tag != null)
                {
                    //Member mbr = (Member)e.NewSelections[0].Tag;
                    Member mbr = GetMemberByUniqueName((string)e.NewSelections[0].Tag);
                    FillMemberProperties(mbr);
                }
            }
        }

        /// <summary>
        /// Заполнение свойств элемента
        /// </summary>
        /// <param name="member">элемент, свойства которого будем отображать</param>
        private void FillMemberProperties(Member member)
        {
            lvProperties.Items.Clear();
            //member.FetchAllProperties();
            if (member == null)
                return;

            CubeDef cube = member.ParentLevel.ParentHierarchy.ParentDimension.ParentCube;
            Member mbr = (Member) cube.GetSchemaObject(SchemaObjectType.ObjectTypeMember, member.UniqueName);

            foreach (Property prop in mbr.Properties)
            {
                string[] propValue = new string[1];
                if (prop.Value != null)
                    propValue[0] = prop.Value.ToString();
                UltraListViewItem item = new UltraListViewItem(prop.Name, propValue);
                lvProperties.Items.Add(item);
            }
            gbMemberProperties.Text = String.Format("Свойства элемента '{0}'", mbr.Caption);
        }

        private void lvSearchResult_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                if(e.SelectedItems[0].Tag != null)
                {
                    Member mbr = (Member) e.SelectedItems[0].Tag;// GetMemberByUniqueName((string)e.SelectedItems[0].Tag);
                    FillMemberProperties(mbr);
                }
            }
        }

        private void GetMemberPropertyList(Member member)
        {
            //чтобы отображались все свойства, получим элемент через схему
            CubeDef cube = member.ParentLevel.ParentHierarchy.ParentDimension.ParentCube;
            //member.FetchAllProperties();
            this.memberProperties.Clear();
            Hierarchy h = member.ParentLevel.ParentHierarchy;

            foreach (Level lev in h.Levels)
            {
                try
                {
                    string uName = lev.GetMembers(0, 1)[0].UniqueName;
                    
                    //Символы переноса в уникальных именах не воспринимаются. Если встретился такой элемент, обращаться к нему не будем, т.к. возникает исключение
                    if (uName.Contains("\r\n"))
                        continue;
                    
                    Member mbr =
                        (Member) cube.GetSchemaObject(SchemaObjectType.ObjectTypeMember, uName);
                    
                    foreach (Property prop in mbr.Properties)
                    {
                        if (!memberProperties.Contains(prop.Name))
                            memberProperties.Add(prop.Name);
                    }
                }
                catch(Exception exc)
                {
                    if (this.pivotData != null)
                        this.pivotData.DoAdomdExceptionReceived(exc);
                }
            }
        }

        /// <summary>
        /// Инвертирование всех включенных элементов
        /// </summary>
        /// <param name="node">корневой элемент</param>
        private void InvertSelectedMembers(UltraTreeNode node)
        {
            while(node != null)
            {
                switch(node.CheckedState)
                {
                    case CheckState.Checked:
                        node.CheckedState = CheckState.Unchecked;
                        break;
                    case CheckState.Unchecked:
                        node.CheckedState = CheckState.Checked;
                        break;
                    case CheckState.Indeterminate:
                        InitNodeChildren(node);
                        if (node.Nodes.Count > 0)
                            InvertSelectedMembers(node.Nodes[0]);
                        break;
                }
                node = node.GetSibling(NodePosition.Next);
            }
        }

        /// <summary>
        /// Раскрыть все включенные элементы
        /// </summary>
        /// <param name="node">корневой элемент</param>
        private void ExpandAllIncludedMembers(UltraTreeNode node)
        {
            while (node != null)
            {
                if (node.CheckedState == CheckState.Indeterminate)
                {
                    node.Expanded = true;
                    ExpandAllIncludedMembers(node.Nodes[0]);
                }
                node = node.GetSibling(NodePosition.Next);
            }
        }

        /// <summary>
        /// Перейти к предыдущему включенному элементу
        /// </summary>
        private void ShowPrevIncludedMember()
        {
            //Находим текущий выделенный элемент, если такого нет, то берем корневой
            UltraTreeNode currNode = null;
            if (tvMembers.SelectedNodes.Count > 0)
            {
                currNode = tvMembers.SelectedNodes[0];
            }
            else
            {
                if (tvMembers.Nodes.Count > 0)
                {
                    currNode = tvMembers.Nodes[0];
                    currNode.Selected = true;
                }
                else
                {
                    return;
                }
            }
            ShowPrevIncludedMember(tvMembers.Nodes[0], false);

        }

        /// <summary>
        /// Перейти к предыдущему включенному элементу
        /// </summary>
        private void ShowPrevIncludedMember(UltraTreeNode node, bool isSelectedFound)
        {
            while (node != null)
            {
                if (isSelectedFound)
                {
                    if (node.CheckedState == CheckState.Checked)
                    {
                        node.Selected = true;
                        tvMembers.TopNode = node;
                        return;
                    }
                }
                if (node.Selected)
                    isSelectedFound = true;

                if (node.HasNodes)
                {
                    ShowPrevIncludedMember(node.Nodes[0].GetSibling(NodePosition.Last), isSelectedFound);
                }
                node = node.GetSibling(NodePosition.Previous);
            }

        }

        /// <summary>
        /// Перейти к следующему включенному элементу
        /// </summary>
        private void ShowNextIncludedMember()
        {
            //Находим текущий выделенный элемент, если такого нет, то берем корневой
            UltraTreeNode currNode = (tvMembers.SelectedNodes.Count > 0)
                                         ? tvMembers.SelectedNodes[0]
                                         :
                                             (tvMembers.Nodes.Count > 0) ? tvMembers.Nodes[0] : null;
            ShowNextIncludedMember(currNode, false);

        }

        /// <summary>
        /// Перейти к следующему включенному элементу
        /// </summary>
        private void ShowNextIncludedMember(UltraTreeNode node, bool isSelectedFound)
        {
            while (node != null)
            {
                if (isSelectedFound)
                {
                    if (node.CheckedState == CheckState.Checked)
                    {
                        node.Selected = true;
                        tvMembers.TopNode = node;
                        return;
                    }
                }
                if (node.Selected)
                    isSelectedFound = true;

                if (node.HasNodes)
                {
                    ShowPrevIncludedMember(node.Nodes[0], isSelectedFound);
                }
                node = node.GetSibling(NodePosition.Next);
            }

        }

        #region фильтр по свойствам

        private void CreatePropertiesTable(UltraGrid grid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Свойство");
            dt.Columns.Add("Значение");
            grid.DataSource = dt;
        }

        private void InitFilters(UltraExpandableGroupBox groupBox, UltraGrid grid)
        {
            ultraTabControl.ActiveTab = ultraTabControl.Tabs[0];
            groupBox.Expanded = false;

            CreatePropertiesTable(grid);

            ValueList vList = null;
            if (!grid.DisplayLayout.ValueLists.Exists("Properties"))
            {
                vList = grid.DisplayLayout.ValueLists.Add("Properties");
            }
            else
            {
                vList = grid.DisplayLayout.ValueLists["Properties"];
                vList.ValueListItems.Clear();
            }

            foreach(string prop in this.memberProperties)
            {
                vList.ValueListItems.Add(prop, prop);
            }


            grid.DisplayLayout.Bands[0].Columns["Свойство"].ValueList = vList;

            grid.DisplayLayout.Bands[0].Columns["Свойство"].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            grid.DisplayLayout.Bands[0].Columns["Свойство"].ButtonDisplayStyle =
                Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable) gFilters.DataSource;
            dt.Rows.Add(new object[2]);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (gFilters.ActiveRow == null)
            {
                MessageBox.Show("Не выбрано ни одного фильтра",
                                "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                gFilters.ActiveRow.Delete(false);
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)gFilters.DataSource;
            if (dt != null)
                dt.Rows.Clear();
        }

        #endregion

        private void tvMembers_MouseClick(object sender, MouseEventArgs e)
        {
            UltraTreeNode node = tvMembers.GetNodeFromPoint(e.X, e.Y);
            if (node == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                this.currentNode = node;
                this.contextMenuStrip.Show(this.tvMembers, e.X, e.Y);
                return;
            }


            if ((this.fieldSet == null)||(this.fieldSet.ParentPivotData.PivotDataType != PivotDataType.Chart))
            {
                return;
            }


            if(node.Tag == null)
                return;

            if (((node.Bounds.Left + 50) > e.X) && ((node.Bounds.Left + 34) < e.X))
            {
                //string uName = ((Member)node.Tag).UniqueName;
                string uName = ((string)node.Tag);
                if (this.exceptedMembers.Contains(uName))
                {
                    this.exceptedMembers.Remove(uName);
                    node.Override.NodeAppearance.Image = 0;
                }
                else
                {
                    this.exceptedMembers.Add(uName);
                    node.Override.NodeAppearance.Image = 1;

                }
            }

        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            this.isUpdating = true;
            ApplyChanges();
            if (!this.isLookupCubeMembers)
                this.fieldSet.MemberNames = this.memberNamesXML;

            bool fl = this.fieldSet.ParentPivotData.IsDeferDataUpdating;
            this.fieldSet.ParentPivotData.IsDeferDataUpdating = false;
            this.fieldSet.ParentPivotData.DoDataChanged();
            this.fieldSet.ParentPivotData.IsDeferDataUpdating = fl;

            this.Activate();
            this.isUpdating = false;
        }

        private void miSearchElement_Click(object sender, EventArgs e)
        {
            ValueListItem item = ceSearchRoot.Items.ValueList.FindByDataValue(this.currentNode);
            if (item == null)
            {
                item = ceSearchRoot.Items.Add(this.currentNode,  this.currentNode.FullPath);
            }

            ceSearchRoot.SelectedItem = item;
            ultraTabControl.SelectedTab = ultraTabControl.Tabs[1];
        }

        private void teSearchStr_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyData)
            {
                case Keys.Enter:
                    FindMembers();
                    break;
                case Keys.Escape:
                    StopSearch();
                    break;
            }
        }

        private void lvSearchResult_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            if ((findingMembers)||(this._isFiltering))
                return;

            Member mbr = (Member) e.Item.Tag;// GetMemberByUniqueName((string)e.Item.Tag);
            if (mbr != null)
            {
                UltraTreeNode node = GetTreeNodeByMember(mbr);
                ultraTabControl.SelectedTab = ultraTabControl.Tabs[0];
                
                UltraTreeNode parentNode = node.Parent;
                while (parentNode != null)
                {
                    parentNode.Expanded = true;
                    parentNode = parentNode.Parent;
                }

                tvMembers.TopNode = node;
                node.Selected = true;
                tvMembers.Select();

            }

        }

        private void ClearMembers()
        {
            if (tvMembers.Nodes.Count > 0)
            {
                tvMembers.Nodes[0].Nodes.Clear();
                tvMembers.Nodes[0].CheckedState = CheckState.Checked;
            }
        }

        private void ultraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            UltraTreeNode topNode = (tvMembers.Nodes.Count > 0) ? tvMembers.Nodes[0] : null;
            if (topNode == null)
                return;

            switch(e.Tool.Key)
            {
                case "InvertButton":
                    InvertSelectedMembers(topNode);
                    break;
                case "ExpandAllIncluded":
                    ExpandAllIncludedMembers(topNode);
                    break;
                case "ClearMembers":
                    ClearMembers();
                    break;
                case "PrevIncluded":
                    ShowPrevIncludedMember();
                    break;
                case "NextIncluded":
                    ShowNextIncludedMember();
                    break;

            }
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            if (!gbMemberProperties.Expanded)
                splitContainer1.SplitterDistance = this.Width - gbMemberProperties.Width - 10;
        }

        private void btAddFilter_Click(object sender, EventArgs e)
        {
            if (this.fieldSet != null)
            {
                MemberFilter mFilter = new MemberFilter();
                this.fieldSet.MemberFilters.Add(mFilter);
                InitMemberFilters();
            }
        }

        private void InitMemberFilters()
        {
            this.pFilterProperties.Visible = false;

            if (this.fieldSet != null)
            {
                lvFilters.Items.Clear();
                int i = 0;
                foreach(MemberFilter mFilter in this.fieldSet.MemberFilters)
                {
                    i++;
                    UltraListViewItem item = lvFilters.Items.Add(String.Format("Фильтр {0}", i), String.Format("Фильтр {0}", i));
                    item.Tag = mFilter;
                }
                
                if (lvFilters.Items.Count > 0)
                {
                    lvFilters.SelectedItems.Add(lvFilters.Items[0]);
                }
            }

        }

        private void ShowFilterProperties(MemberFilter mFilter)
        {
            if (mFilter != null)
            {
                this.isUpdating = true;
                this.teFilterStr.Text = mFilter.MemberName;
                DataTable dt = (DataTable)this.gProperties.DataSource;
                dt.Rows.Clear();
                foreach (KeyValuePair<string, string> prop in mFilter.Properties)
                {
                    dt.Rows.Add(new string[2] {prop.Key, prop.Value});
                }
                this.isUpdating = false;
                
            }
        }

        private void InitCurMemberFilter()
        {
            if (this._isFiltering)
                return;

            if (this.isUpdating)
                return;

            if (this._curMemberFilter != null)
            {
                this._curMemberFilter.MemberName = this.teFilterStr.Text;
                DataTable dt = (DataTable) this.gProperties.DataSource;
                this._curMemberFilter.Properties = GetDictPropertiesFromTable(dt);
            }
        }
        

        private void lvFilters_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            this.pFilterProperties.Visible = this.lvFilters.SelectedItems.Count > 0;

            if (this.lvFilters.SelectedItems.Count > 0)
            {
                this._curMemberFilter = (MemberFilter)this.lvFilters.SelectedItems[0].Tag;
                ShowFilterProperties(this._curMemberFilter);
            }
        }

        private void teFilterStr_ValueChanged(object sender, EventArgs e)
        {
            InitCurMemberFilter();
        }


        private void btAddPropFilter_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)gProperties.DataSource;
            dt.Rows.Add(new object[2]);

        }

        private void btDelPropFilter_Click(object sender, EventArgs e)
        {
            if (gProperties.ActiveRow == null)
            {
                MessageBox.Show("Не выбрано ни одного фильтра",
                                "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                gProperties.ActiveRow.Delete(false);
            }
            InitCurMemberFilter();
        }

        private void btClearPropFilters_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)gProperties.DataSource;
            if (dt != null)
                dt.Rows.Clear();
            InitCurMemberFilter();

        }

        private void gProperties_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
        }

        private void StopFiltration()
        {
            SetFilterPropertiesEnabled(true);
            this._isFiltering = false;
            btStartFiltration.Visible = true;
            lCurFiltMember.Text = "";
            btOK.Enabled = this.GetEnabledBtOK();
        }

        /// <summary>
        /// Проверить есть ли фильтры в измерении
        /// </summary>
        /// <returns></returns>
        private bool IsFiltersExists()
        {
            bool result = false;
            if(this.fieldSet != null)
            {
                foreach (MemberFilter mFilter in this.fieldSet.MemberFilters)
                {
                    if(mFilter.MemberName.Trim() != "")
                    {
                        return true;
                    }

                    foreach (KeyValuePair<string, string> prop in mFilter.Properties)
                    {
                        if (prop.Value.Trim() != "")
                        {
                            return true;
                        }
                    }

                }
            }

            return result;
        }

        private void btStartFiltration_Click(object sender, EventArgs e)
        {
            if (!IsFiltersExists())
            {
                MessageBox.Show("Нет ни одного фильта с заданным условием.",
                    "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.lvFilterResult.ViewSettingsDetails.AutoFitColumns = AutoFitColumns.ResizeAllColumns;
            this.lvFilterResult.Items.Clear();
            this.lvFilterResult.ViewSettingsDetails.CheckBoxStyle = CheckBoxStyle.CheckBox;
            if (this.tvMembers.Nodes.Count > 0)
                this.tvMembers.Nodes[0].Nodes.Clear();

            SetFilterPropertiesEnabled(false);
            btStartFiltration.Visible = false;
            this._loadedMembersCount = 0;
            this._isFiltering = true;


            foreach (Level lev in this.mbrs[0].ParentLevel.ParentHierarchy.Levels)
                LoadFilteredMembers(lev);
            StopFiltration();

            if (this.lvFilterResult.Items.Count == 0)
            {
                this.lvFilterResult.ViewSettingsDetails.CheckBoxStyle = CheckBoxStyle.None;
                this.lvFilterResult.Items.Add("members not found", "< Не найдено ни одного элемента >");
            }
            SetSearchResultColumnWidth(lvFilterResult);

        }

        private void SetFilterPropertiesEnabled(bool value)
        {
            teFilterStr.Enabled = value;
            gbPropertiesFilter.Enabled = value;
        }

        private void btStopFilter_Click(object sender, EventArgs e)
        {
            StopFiltration();
        }

        private void btDelFilter_Click(object sender, EventArgs e)
        {
            if (this.lvFilters.SelectedItems.Count > 0)
            {
                this.fieldSet.MemberFilters.Remove((MemberFilter) this.lvFilters.SelectedItems[0].Tag);
                InitMemberFilters();
            }
        }

        private void gbPropertiesFilter_ExpandedStateChanged(object sender, EventArgs e)
        {
            lvFilterResult.Top = gbPropertiesFilter.Top + gbPropertiesFilter.Height + 10;
            lvFilterResult.Height = pFilterProperties.Height - lvFilterResult.Top - 20;
        }

        private void gProperties_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            InitCurMemberFilter();
        }

    }
}