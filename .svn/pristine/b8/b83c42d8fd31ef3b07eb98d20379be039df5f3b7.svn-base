using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;

namespace Krista.FM.Common.TaskParamEditors
{
    partial class MembersTree : UserControl
    {
        private XmlDocument dom;

        /// <summary>
        /// Список всех узлов дерева в порядке добавления.
        /// </summary>
        private List<MembersTreeNode> treeNodes = new List<MembersTreeNode>();

        private bool isLoading;
        internal bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set 
            {
                isLoading = value; 
                if (isLoading)
                    treeView1.BeginUpdate();
                else
                    treeView1.EndUpdate();
            }
        }

        internal bool IsLocked 
        { 
            get; 
            set;
        }

        internal string LastError { get; set; }

        private MembersTreeNode hitNode;

        private void SetToolTip(Control ctrl, string text)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(ctrl, text);
        }

        private string CodeToShow { get; set; }

        internal MembersTree()
        {
            InitializeComponent();
            SetToolTip(SelectAllButton, "Выделить все");
            SetToolTip(DeselectAllButton, "Снять выделение");
            SetToolTip(InvertButton, "Обратить выделение");
            SetToolTip(ExpandButton, "Развернуть все уровни");
            SetToolTip(CollapseButton, "Свернуть все уровни");
            SetToolTip(ExpandCheckedButton, "Развернуть до выделенных элементов");
            SetToolTip(PrevCheckedButton, "Перейти к предыдущему выделенному элементу");
            SetToolTip(NextCheckedButton, "Перейти к следующему выделенному элементу");

        }

        internal new bool Load(IScheme scheme, string fullDimName, string oldValue)
        {
            LastError = string.Empty;

            XmlDocument document = LoadMetadata(scheme);
            if (document == null)
                return false;

            XmlDocument oldMembers = null;
            if (oldValue != string.Empty)
            {
                oldMembers = new XmlDocument();
                try
                {
                    oldMembers.LoadXml(oldValue);
                }
                catch (Exception e)
                {
                    oldMembers = null;
                }
            }

            string dimName;
            string hierName;
            Utils.ParseDimensionName(fullDimName, out dimName, out hierName, ".");

            CodeToShow = GetCodeToShow(document, dimName, hierName);
            string levelNames = GetLevelNames(document, dimName, hierName);
            if (levelNames == String.Empty)
                return false;

            string members = scheme.PlaningProvider.GetMembers("0", "", dimName, hierName, levelNames, "");
            XmlDocument newMembers = new XmlDocument();
            try
            {
                newMembers.LoadXml(members);
            }
            catch (Exception e)
            {
                LastError = Diagnostics.KristaDiagnostics.ExpandException(e);
                return false;
            }

            if (oldMembers != null)
                Utils.CopyMembersState(oldMembers, newMembers);

            return LoadTree(newMembers, levelNames, String.Empty);
        }

        /// <summary>
        /// Загружает дерево элементов измерения
        /// </summary>
        /// <param name="xmlDom">xml измерения</param>
        /// <param name="allLevels">список всех уровней измерения через разделитель</param>
        /// <param name="code">Название свойства элемента для формирования выводимого имени элемента</param>
        /// <returns></returns>
        internal bool LoadTree(XmlDocument xmlDom, string allLevels, string code)
        {
            IsLoading = true;

            dom = xmlDom;
            LoadLevels(allLevels);

            treeView1.Nodes.Clear();
            treeNodes.Clear();
            XmlNodeList nl = xmlDom.SelectNodes("function_result/Members/Member[@name]");
            if (nl == null)
                return false;
            foreach (XmlNode xmlNode in nl)
                LoadMemberNode(xmlNode, null);
            
            UpdateNodeImages();
            if (treeView1.Nodes.Count > 0)
                treeView1.Nodes[0].Expand();
            IsLoading = false;
            return treeView1.Nodes.Count > 0;
        }

        /// <summary>
        /// Переписывает в исходный документ состояние всех узлов
        /// </summary>
        internal void SaveTreeToXmlDocument()
        {
            foreach (Level level in levels)
            {
                XmlNode levelNode = GetLevelNode(level.Name);
                XmlHelper.SetAttribute(levelNode, Attr.LevelState, ((int) level.State).ToString());
            }

            foreach (MembersTreeNode treeNode in treeNodes)
            {
                XmlHelper.SetAttribute(treeNode.DomNode, Attr.Checked, treeNode.Checked.ToString().ToLower());
                XmlHelper.SetAttribute(treeNode.DomNode, Attr.Influence, ((int) treeNode.Influence).ToString());
                XmlHelper.SetAttribute(treeNode.DomNode, Attr.UnderInfluence, ((int)treeNode.UnderInfluence).ToString());
            }
        }

        internal static string GetMembersText(string xmlValue)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(xmlValue);
            }
            catch
            {
            }
            return GetMembersText(document);
        }

        internal static string GetMembersText(XmlDocument document)
        {
            if (document == null)
                return String.Empty;
            XmlNode root = document.SelectSingleNode("function_result/Members");
            string text = GetMemberText(root, 0);
            text = text.TrimStart('\r', '\n');
            return text;
        }

        private static string GetMemberText(XmlNode memberNode, int level)
        {
            if (memberNode == null)
                return String.Empty;
            XmlNodeList children = memberNode.SelectNodes("Member");
            if (children == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (XmlNode child in children)
            {
                string childName = XmlHelper.GetStringAttrValue(child, Attr.Name, String.Empty);
                if (childName == String.Empty)
                    continue;
                bool check = XmlHelper.GetBoolAttrValue(child, Attr.Checked, false);
                NodeInfluence inf = (NodeInfluence) XmlHelper.GetIntAttrValue(child, Attr.Influence, 0);
                if (check || (inf == NodeInfluence.Children) || (inf == NodeInfluence.Descendants))
                {
                   // if (sb.ToString() != String.Empty)
                        sb.Append("\r\n");
                    for (int i = 0; i < level; i++)
                        sb.Append("         ");
                    sb.Append(childName);
                    sb.Append(GetMemberText(child, level + 1));
                }
                else
                    sb.Append(GetMemberText(child, level));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Рекурсивно загружает узел документа в дерево 
        /// </summary>
        /// <param name="xmlNode">Загружаемый узел документа</param>
        /// <param name="treeNode">Родительский узел дерева. Null для корня.</param>
        private void LoadMemberNode(XmlNode xmlNode, TreeNode treeNode)
        {
            string nodeName = XmlHelper.GetStringAttrValue(xmlNode, Attr.Name, String.Empty);
            if (nodeName == String.Empty)
                return;
            if (CodeToShow != string.Empty)
            {
                string codeValue = XmlHelper.GetStringAttrValue(xmlNode, CodeToShow, string.Empty);
                if (codeValue != string.Empty)
                    nodeName = string.Format("[{0}] {1}", codeValue, nodeName);
            }

            MembersTreeNode newNode = new MembersTreeNode(nodeName, xmlNode);
            if (treeNode == null)
                treeView1.Nodes.Add(newNode);
            else
                treeNode.Nodes.Add(newNode);
            treeNodes.Add(newNode);
            newNode.ContextMenuStrip = treeMenu;
            newNode.MembersTree = this;

            newNode.Checked = XmlHelper.GetBoolAttrValue(xmlNode, Attr.Checked, false);
            newNode.Influence = (NodeInfluence)XmlHelper.GetIntAttrValue(xmlNode, Attr.Influence, 0);
            newNode.InCheckedLevel = levels[newNode.Level].State != LevelState.Disabled;

            foreach (XmlNode childNode in xmlNode.SelectNodes("Member[@name]"))
                LoadMemberNode(childNode, newNode);
        }

        private void UpdateNodeImages()
        {
            foreach (MembersTreeNode node in treeView1.Nodes)
                node.UpdateImage();
        }

        /// <summary>
        /// Обрабатывает изменение состояния уровня
        /// </summary>
        /// <param name="levelIndex">Порядковый номер уровня в коллекции</param>
        private void OnLevelStateChanged(int levelIndex)
        {
            treeView1.BeginUpdate();

            Level level = levels[levelIndex];
            foreach (MembersTreeNode node in treeNodes.Cast<MembersTreeNode>().Where(node => node.Level == levelIndex))
            {
                node.InCheckedLevel = level.State != LevelState.Disabled; // !!! or not MayDisableLevels
                if (level.State != LevelState.Forced)
                    node.UpdateImage();
            }

            if (level.State == LevelState.Forced)
                OnMembersCheck(levelIndex, true);
            //SetDomLevelState(level);

            treeView1.EndUpdate();
        }

        /// <summary>
        /// Обрабатывает установку или снятие выделения с элементов уровня
        /// </summary>
        /// <param name="levelIndex">Порядковый номер уровня в коллекции</param>
        /// <param name="isChecked">Состояние выделения</param>
        private void OnMembersCheck(int levelIndex, bool isChecked)
        {
            if ((levels[levelIndex].State == LevelState.Forced) && !isChecked)
                levels[levelIndex].State = LevelState.Enabled;

            foreach (MembersTreeNode node in treeNodes)
            {
                if (node.Level == levelIndex)
                    node.Checked = isChecked;
                else
                    if (((node.Level < levelIndex) && (node.Influence == NodeInfluence.Descendants)) ||
                        ((node.Level - levelIndex == -1) && (node.Influence == NodeInfluence.Children)))
                        node.Influence = NodeInfluence.None;
            }
            UpdateNodeImages();
        }

        private const string itemNameNone = "itemNone";
        private const string itemNameChildren = "itemChildren";
        private const string itemNameDescendants = "itemDescendants";
        private const string itemNameExclude = "itemExclude";

        private void treeMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (hitNode == null)
                return;

            switch (e.ClickedItem.Name)
            {
                case itemNameNone:
                    hitNode.Influence = NodeInfluence.None;
                    break;

                case itemNameChildren:
                    hitNode.Influence = NodeInfluence.Children;
                    break;

                case itemNameDescendants:
                    hitNode.Influence = NodeInfluence.Descendants;
                    break;

                case itemNameExclude:
                    hitNode.Influence = NodeInfluence.Exclude;
                    break;

                case "itemSelectChildren":
                    hitNode.CheckChildren(true, false);
                    break;

                case "itemSelectDescendants":
                    hitNode.CheckChildren(true, true);
                    break;

                case "itemDeselectChidlren":
                    hitNode.CheckChildren(false, false);
                    break;

                case "itemDeselectDescendants":
                    hitNode.CheckChildren(false, true);
                    break;
            }
        }

        private void treeMenu_Opening(object sender, CancelEventArgs e)
        {
            if (hitNode == null)
                return;

            foreach (var item in treeMenu.Items)
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                    menuItem.Checked = false;

                    // Не даем устанавливать узлу влияние на потомков, если он сам уже под влиянием
                    bool notUnderDescendants = !((hitNode.UnderInfluence == NodeInfluence.Descendants)
                        && ((menuItem.Name == itemNameChildren) || (menuItem.Name == itemNameDescendants)));

                    // При запрете редактирования элементов доступны только "шарики"
                    bool notLocked = !IsLocked
                                     || (menuItem.Name == itemNameNone)
                                     || (menuItem.Name == itemNameChildren)
                                     || (menuItem.Name == itemNameDescendants)
                                     || (menuItem.Name == itemNameExclude);

                    // Потомкам отключенных элементов недоступна ни одна опция
                    bool notUnderExcluded = hitNode.UnderInfluence != NodeInfluence.Exclude;

                    menuItem.Enabled = notLocked && notUnderExcluded && notUnderDescendants;

                    /*if (FPopup.Items[i].Name = 'pmiSetDefault') and FPopup.Items[i].Enabled then
                        FPopup.Items[i].Enabled := Node.Checked;*/

                }

            switch (hitNode.Influence)
            {
                case NodeInfluence.Children:
                    (treeMenu.Items[itemNameChildren] as ToolStripMenuItem).Checked = true;
                    break;

                case NodeInfluence.Descendants:
                    (treeMenu.Items[itemNameDescendants] as ToolStripMenuItem).Checked = true;
                    break;

                case NodeInfluence.Exclude:
                    (treeMenu.Items[itemNameExclude] as ToolStripMenuItem).Checked = true;
                    break;

                case NodeInfluence.None:
                    (treeMenu.Items[itemNameNone] as ToolStripMenuItem).Checked = true;
                    break;
            }
        }
        
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo info = treeView1.HitTest(e.X, e.Y);
            if (info.Node != null)
                hitNode = (MembersTreeNode)info.Node;
            if ((e.Button == MouseButtons.Left) && (info.Location == TreeViewHitTestLocations.StateImage) && (e.Clicks == 1))
            {
                if (IsLocked)
                    return;
                if ((hitNode.Influence == NodeInfluence.Exclude) || (hitNode.UnderInfluence == NodeInfluence.Exclude))
                    return; 
                if (RequestOnParentInfluence(hitNode) && RequestOnForcedLevel(hitNode))
                {
                    hitNode.ToggleCheck();
                }
            }
        }

        private bool RequestOnParentInfluence(MembersTreeNode node)
        {

            /* Здесь зашита обработка ситуации, когда пользователь снимает галку с потомка "красного или зеленого шарика". 
                Чтобы исключить возможность случайного, ненамеренного сброса признака влияния, спросим у пользователя,
                уверен ли он в желании снять галку и отключить "шарик"*/

            if (IsLoading)
                return true;
            if (node.Influence == NodeInfluence.Exclude)
                return true;
            if (!node.Checked)
                return true;
            if (!node.IsUnderInfluence)
                return true;

            return MessageBox.Show(@"Снятие выделения с данного элемента приведет к сбросу атрибута влияния у его предков. Вы уверены, что хотите продолжить?", 
                @"Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

        }

        private bool RequestOnForcedLevel(MembersTreeNode node)
        {
            if (levels[node.Level].State != LevelState.Forced)
                return true;

            bool result = MessageBox.Show(@"Снятие выделения с данного элемента приведет к сбросу безусловного выделения уровня. Продолжить?",
                @"Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            if (result)
                levels[node.Level].State = LevelState.Enabled;
            return result;
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
                e.Cancel = true;
        }

        #region Обработчики кнопок
        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            foreach (MembersTreeNode treeNode in treeNodes)
            {
                treeNode.Checked = true;
                if (treeNode.Influence == NodeInfluence.Exclude)
                    treeNode.Influence = NodeInfluence.None;
            }
            UpdateNodeImages();
            IsLoading = false;
            

        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            foreach (MembersTreeNode treeNode in treeNodes)
            {
                treeNode.Checked = false;
                if (treeNode.Influence != NodeInfluence.Exclude)
                    treeNode.Influence = NodeInfluence.None;
            }
            foreach (Level level in levels)
            {
                if (level.State == LevelState.Forced)
                    level.State = LevelState.Enabled;
            }
            UpdateNodeImages();
            IsLoading = false;
        }

        private void InvertButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            foreach (MembersTreeNode treeNode in treeNodes)
            {
                if (treeNode.Influence != NodeInfluence.Exclude)
                    treeNode.Influence = NodeInfluence.None;
                treeNode.ToggleCheck();
            }
            foreach (Level level in levels)
            {
                if (level.State == LevelState.Forced)
                    level.State = LevelState.Enabled;
            }
            UpdateNodeImages();
            IsLoading = false;
        }

        private void ExpandButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            treeView1.ExpandAll();
            IsLoading = false;
        }

        private void CollapseButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            treeView1.CollapseAll();
            IsLoading = false;
        }

        private void ExpandCheckedButton_Click(object sender, EventArgs e)
        {
            IsLoading = true;
            foreach (MembersTreeNode treeNode in treeNodes)
                if (treeNode.GetCheckedCount(true) > 0)
                    treeNode.Expand();
            IsLoading = false;
        }

        private void NextCheckedButton_Click(object sender, EventArgs e)
        {
            PrevNextButtonHandler(true);
        }

        private void PrevCheckedButton_Click(object sender, EventArgs e)
        {
            PrevNextButtonHandler(false);
        }

        private int GetNextOrPrevIndex(int index, bool isNext)
        {
            if (isNext)
            {
                if (++index == treeNodes.Count)
                    index = 0;
            }
            else
            {
                if (--index == -1)
                    index = treeNodes.Count - 1;
            }
            return index;
        }

        private void PrevNextButtonHandler(bool isNext)
        {
            // Нечего делать, если нет выбранных элементов
            if (treeNodes.Where(tNode => tNode.Checked).Count() == 0)
                return;

            IsLoading = true;

            int index = treeNodes.IndexOf(((treeView1.SelectedNode ?? treeNodes[0]) as MembersTreeNode));
            do
            {
                index = GetNextOrPrevIndex(index, isNext);
                if (treeNodes[index].Checked)
                {
                    treeView1.SelectedNode = treeNodes[index];
                    treeView1.SelectedNode.EnsureVisible();
                }
            } while (!treeNodes[index].Checked);
            
            IsLoading = false;
        }

        private void ButtonEnterHandler(object sender, EventArgs e)
        {
            treeView1.Focus();
        }
        #endregion

        

        private XmlDocument LoadMetadata(IScheme scheme)
        {
            if (scheme == null)
                return null;

            string metadata = scheme.PlaningProvider.GetMetaData();
            if (metadata == String.Empty)
                return null;
            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(metadata);
            }
            catch (Exception e)
            {
                LastError = Diagnostics.KristaDiagnostics.ExpandException(e);
                return null;
            }
            return document;
        }

        private string GetLevelNames(XmlDocument document, string dimName, string hierName)
        {
            XmlNode hierNode = GetHierarchyNode(document, dimName, hierName);
            if (hierNode == null)
                return String.Empty;

            XmlNodeList levelsList = hierNode.SelectNodes("Levels/Level");
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode levelNode in levelsList)
            {
                string levelName = XmlHelper.GetStringAttrValue(levelNode, Attr.Name, String.Empty);
                if (sb.ToString() != String.Empty)
                    sb.Append("$$$");
                sb.Append(levelName);
            }
            return sb.ToString();
        }

        private XmlNode GetHierarchyNode(XmlDocument document, string dimName, string hierName)
        {
            string xpath = String.Format("function_result/SharedDimensions/Dimension[@{0}='{1}']", Attr.Name, dimName);
            XmlNode dimNode = document.SelectSingleNode(xpath);
            if (dimNode == null)
                return null;

            xpath = String.Format("Hierarchy[@{0}='{1}']", Attr.Name, hierName);
            XmlNode hierNode = dimNode.SelectSingleNode(xpath);
            return hierNode;
        }

        private string GetCodeToShow(XmlDocument document, string dimName, string hierName)
        {
            XmlNode hierNode = GetHierarchyNode(document, dimName, hierName);
            if (hierNode == null)
                return String.Empty;

            XmlNode node = hierNode.SelectSingleNode("Properties/Property[@name = 'Код']");
            if (node != null)
                return "Код";
            node = hierNode.SelectSingleNode("Properties/Property[@name = 'ИНН']");
            if (node != null)
                return "ИНН";
            return string.Empty;
        }


        private void FilterMembers(XmlNode oldRoot, XmlNode newRoot, int levelIndex)
        {
            if ((oldRoot == null) || (!oldRoot.HasChildNodes))
                return;
            if (levels[levelIndex].State == LevelState.Disabled)
            {
                foreach (XmlNode childNode in oldRoot.ChildNodes)
                {
                    FilterMembers(childNode, newRoot, levelIndex + 1);
                }
            }
            else
            {
                foreach (XmlNode childNode in oldRoot.ChildNodes)
                {
                    XmlNode node = newRoot.OwnerDocument.ImportNode(childNode.CloneNode(false), false);
                    newRoot.AppendChild(node);
                    FilterMembers(childNode, node, levelIndex + 1);
                }
            }
        }

        private void FilterLevels(XmlNode root)
        {
            if (root == null)
                return;
            for (int i = root.ChildNodes.Count - 1; i >= 0; i--)
            {
                if (levels[i].State == LevelState.Disabled)
                {
                    root.RemoveChild(root.ChildNodes[i]);
                    continue;
                }
                XmlHelper.SetAttribute(root.ChildNodes[i], Attr.LevelState, ((int) levels[i].State).ToString());
            }

        }

        private void FilterMembersDom(XmlDocument document)
        {
            Utils.CopyInfluences(document);
            XmlNode root = document.SelectSingleNode("function_result/Members");
            
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(document.OuterXml);
            XmlNode newRoot = doc2.SelectSingleNode("function_result/Members");
            newRoot.RemoveAll();
            FilterMembers(root, newRoot, 0);
            
            document.LoadXml(doc2.OuterXml);
            root = document.SelectSingleNode("function_result/Levels");
            FilterLevels(root);
        }

        private void PrepareXml()
        {
            SaveTreeToXmlDocument();
            Utils.SetCheckedIndication(dom);
            FilterMembersDom(dom);
            Utils.CutAllInvisible(dom, true);
        }

        internal string GetXmlValue()
        {
            if (dom == null)
                return string.Empty;
            PrepareXml();
            return dom.OuterXml;
        }


        private const int levelsInitialHeight = 20;
        private int levelsMaxHeight = 20;

        private bool levelsExposed;

        private readonly List<Level> levels = new List<Level>();


        /// <summary>
        /// Изначальная загрузка уровней
        /// </summary>
        /// <param name="allLevels"></param>
        private void LoadLevels(string allLevels)
        {
            string[] splitLevels = allLevels.Split(new[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);

            int boxTop = levelsInitialHeight + 0;
            int boxLeft = 28;

            foreach (string levelName in splitLevels)
            {
                LevelState levelState = LoadInitialLevelState(levelName);
                Level level = new Level(levelName, levelState);
                levels.Add(level);
                CheckBox box = new CheckBox
                {
                    Text = levelName,
                    Parent = pnLevels,
                    Top = boxTop,
                    Left = boxLeft,
                    AutoSize = true,
                    Checked = (levelState == LevelState.Enabled) || (levelState == LevelState.Forced),
                    ContextMenuStrip = levelMenu,
                    Tag = level
                };
                box.CheckedChanged += LevelCheckBoxCheckedChanged;
                boxTop += box.Height;
                boxLeft += 20;
            }

            levelsMaxHeight = boxTop;
        }

        private XmlNode GetLevelNode(string levelName)
        {
            return dom.SelectSingleNode(String.Format("function_result/Levels/Level[@{0}='{1}']", Attr.Name, levelName));
        }

        private LevelState LoadInitialLevelState(string levelName)
        {
            LevelState levelState = LevelState.Disabled;
            XmlNode levelNode = GetLevelNode(levelName);
            if (levelNode != null)
            {
                int levelStateInt = XmlHelper.GetIntAttrValue(levelNode, Attr.LevelState, -1);
                if (levelStateInt == -1)
                    levelStateInt = 1;
                levelState = (LevelState)levelStateInt;
            }
            return levelState;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            levelsExposed = !levelsExposed;
            pnLevels.Height = levelsExposed ? levelsMaxHeight : levelsInitialHeight;
            linkLabel1.Text = levelsExposed ? "Скрыть уровни" : "Показать уровни";
        }

        private void UncheckAllMenuItems()
        {
            foreach (var item in levelMenu.Items)
            {
                if (item is ToolStripMenuItem)
                    ((ToolStripMenuItem)item).Checked = false;
            }
        }

        private void CheckClickedMenuItem(ToolStripMenuItem clickedItem)
        {
            UncheckAllMenuItems();
            clickedItem.Checked = true;
        }

        /// <summary>
        /// Обработчик пунктов контекстного меню чекбоксов уровней
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            CheckBox box = ((sender as ContextMenuStrip).SourceControl as CheckBox);
            Level level = (Level) box.Tag;

            switch (e.ClickedItem.Name)
            {
                case "itemDisable":
                    box.Checked = false;
                    CheckClickedMenuItem(itemDisable);
                    level.State = LevelState.Disabled;
                    box.ForeColor = Color.Black;
                    OnLevelStateChanged(GetLevelIndex(level));
                    break;

                case "itemEnable":
                    box.Checked = true;
                    CheckClickedMenuItem(itemEnable);
                    level.State = LevelState.Enabled;
                    box.ForeColor = Color.Black;
                    OnLevelStateChanged(GetLevelIndex(level));
                    break;

                case "itemForce":
                    box.Checked = true;
                    CheckClickedMenuItem(itemForce);
                    level.State = LevelState.Forced;
                    box.ForeColor = Color.Red;
                    OnLevelStateChanged(GetLevelIndex(level));
                    break;

                case "itemCheck":
                    OnMembersCheck(GetLevelIndex(level), true);
                    break;

                case "itemUncheck":
                    OnMembersCheck(GetLevelIndex(level), false);
                    break;
            }
        }

        private void levelMenu_Opening(object sender, CancelEventArgs e)
        {
            UncheckAllMenuItems();
            CheckBox box = (sender as ContextMenuStrip).SourceControl as CheckBox;
            Level level = (Level) box.Tag;
            ToolStripMenuItem menuItem = levelMenu.Items[(int)level.State] as ToolStripMenuItem;
            menuItem.Checked = true;
        }     
   
        private int GetLevelIndex(Level level)
        {
            return levels.IndexOf(level);
        }

        private void LevelCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = sender as CheckBox;
            Level level = (Level) box.Tag;
            if (box.Checked)
            {
                CheckClickedMenuItem(itemEnable);
                level.State = LevelState.Enabled;
            }
            else
            {
                CheckClickedMenuItem(itemDisable);
                level.State = LevelState.Disabled;
            }
            
            box.ForeColor = Color.Black;
            OnLevelStateChanged(GetLevelIndex(level));
        }

    }



    internal class Level
    {
        internal Level(string name, LevelState state)
        {
            this.name = name;
            this.State = state;
        }

        private readonly string name;

        internal string Name
        {
            get { return name; }
        }

        internal LevelState State { get; set; }
    }

    /// <summary>
    /// Влияние узла на потомков
    /// </summary>
    internal enum NodeInfluence
    {
        None,
        Children,
        Descendants,
        Exclude
    };

    internal enum LevelState
    {
        Disabled,
        Enabled,
        Forced
    };

    internal struct Attr
    {
        internal const string Name = "name";
        internal const string Checked = "checked";
        internal const string Influence = "influence";
        internal const string UnderInfluence = "underinfluence";
        internal const string LevelState = "levelstate";
        internal const string UniqueName = "unique_name";
        internal const string DefaultValue = "defaultvalue";
    }
}