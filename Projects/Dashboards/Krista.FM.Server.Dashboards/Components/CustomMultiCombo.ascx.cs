using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.WebUI.UltraWebNavigator;
using System.ComponentModel;

namespace Krista.FM.Server.Dashboards.Components
{
    public enum MultipleSelectionType
    {
        SimpleMultiple,
        CascadeMultiple
    }

    public enum AllowedSelectionType
    {
        AllNodes,
        LeafNodes
    }

    public enum TooltipVisibilityMode
    {
        Auto,
        Shown
    }

    /// <summary>
    /// Комбобокс с множественным выбором
    /// </summary>
    public partial class CustomMultiCombo : UserControl
    {
        #region Поля

        protected Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
        private bool panelExpanded = false;

        #endregion

        #region Свойства

        /// <summary>
        /// Доступен ли множественный выбор
        /// </summary>
        public bool MultiSelect
        {
            get
            {
                return ViewState["MultiSelect"] == null ? false : (bool)ViewState["MultiSelect"];
            }
            set
            {
                ViewState["MultiSelect"] = value;
            }
        }

        /// <summary>
        /// Вызывать ли постбэк после выбора элемента
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                return ViewState["AutoPostBack"] == null ? false : (bool)ViewState["AutoPostBack"];
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        /// <summary>
        /// Тип выделения узла (для множественного выбора)
        /// </summary>
        public MultipleSelectionType MultipleSelectionType
        {
            get
            {
                return ViewState["MultipleSelectionType"] == null ? MultipleSelectionType.CascadeMultiple : (MultipleSelectionType)ViewState["MultipleSelectionType"];
            }
            set
            {
                ViewState["MultipleSelectionType"] = value;
            }
        }

        /// <summary>
        /// Тип выделения узла (для одиночного выбора)
        /// </summary>
        public AllowedSelectionType AllowSelectionType
        {
            get
            {
                return ViewState["AllowSelectionType"] == null ? AllowedSelectionType.AllNodes : (AllowedSelectionType)ViewState["AllowSelectionType"];
            }
            set
            {
                ViewState["AllowSelectionType"] = value;
            }
        }

        /// <summary>
        /// Показывать выбранные элементы
        /// </summary>
        public bool ShowSelectedValue
        {
            get
            {
                // по умолчанию true
                return ViewState["ShowSelectedValue"] == null ? true : (bool)ViewState["ShowSelectedValue"];
            }
            set
            {
                ViewState["ShowSelectedValue"] = value;
            }
        }

        /// <summary>
        /// Режим показа подсказок к элементу
        /// </summary>
        public TooltipVisibilityMode TooltipVisibility
        {
            get
            {
                // по умолчанию true


                return ViewState["TooltipVisibility"] == null ? TooltipVisibilityMode.Auto : (TooltipVisibilityMode)ViewState["TooltipVisibility"];
            }
            set
            {
                ViewState["TooltipVisibility"] = value;
            }
        }

        /// <summary>
        /// Выбранный узел для одиночного выбора
        /// </summary>
        public Node SelectedNode
        {
            get
            {
                return GetSelected(treeView);
            }
        }

        Collection<Node> selectedNodes = new Collection<Node>();

        /// <summary>
        /// Выбранные узлы для множественного выбора
        /// </summary>
        public Collection<Node> SelectedNodes
        {
            get
            {
                selectedNodes = new Collection<Node>();
                GetMultiSelected(treeView);
                return selectedNodes;
            }
        }

        /// <summary>
        /// Выбранное значение для одиночного выбора
        /// </summary>
        public string SelectedValue
        {
            get
            {
                Node node = GetSelected(treeView);
                return (node != null) ? node.Text : string.Empty;
            }
        }

        /// <summary>
        /// Выбранные значения для множественного выбора
        /// </summary>
        public Collection<string> SelectedValues
        {
            get
            {
                selectedNodes = new Collection<Node>();
                GetMultiSelected(treeView);
                Collection<string> result = new Collection<string>();
                foreach (Node node in selectedNodes)
                {
                    result.Add(node.Text);
                }
                return result;
            }
        }

        /// <summary>
        /// Выбранные значения для множественного выбора в виде строки
        /// </summary>
        public string SelectedValuesString
        {
            get
            {
                string result = string.Empty;
                Collection<string> selectedValue = SelectedValues;
                if (selectedValue.Count != 0)
                {
                    foreach (string s in selectedValue)
                    {
                        result += string.Format(" {0},", s);
                    }

                    if (result != string.Empty && result[result.Length - 1] == ',')
                    {
                        result = result.Trim(',');
                    }

                    result = result.TrimStart(' ');
                }
                return result;
            }
        }

        /// <summary>
        /// Индекс выбранного элемента
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (!MultiSelect)
                {
                    Node node = SelectedNode;
                    return node == null ? -1 : node.Index;
                }
                else
                {
                    Collection<Node> nodes = SelectedNodes;
                    return nodes.Count == 0 ? -1 : nodes[0].Index;
                }
            }
        }

        /// <summary>
        /// Заголовок (значение хранится во вьюстейте компонента)
        /// </summary>
        private string PanelTitle
        {
            get
            {
                return ViewState["PanelTitle"] == null ? string.Empty : ViewState["PanelTitle"].ToString();
            }
            set
            {
                ViewState["PanelTitle"] = value;
            }
        }

        /// <summary>
        /// Отображение панели в развернутом виде.
        /// В IE не работает.
        /// </summary>
        public bool PanelExpanded
        {
            get
            {
                return panelExpanded;
            }
            set
            {
                panelExpanded = value;
            }
        }

        /// <summary>
        /// Заголовок панели
        /// </summary>
        public string PanelHeaderTitle
        {
            get
            {
                return webPanel.Header.Text;
            }
            set
            {
                webPanel.Header.Text = value;
            }
        }

        /// <summary>
        /// Хинт к заголовку панели
        /// </summary>
        public string PanelHeaderToolTip
        {
            get
            {
                return webPanel.ToolTip;
            }
            set
            {
                webPanel.ToolTip = value;
            }
        }

        /// <summary>
        /// Ширина (значение хранится во вьюстейте компонента)
        /// </summary>
        private int PanelWidth
        {
            get
            {
                return ViewState["PanelWidth"] == null ? 0 : Convert.ToInt32(ViewState["PanelWidth"]);
            }
            set
            {
                ViewState["PanelWidth"] = value;
            }
        }

        /// <summary>
        /// Высота (значение хранится во вьюстейте компонента)
        /// </summary>
        private int PanelHeight
        {
            get
            {
                return ViewState["PanelHeight"] == null ? 0 : Convert.ToInt32(ViewState["PanelHeight"]);
            }
            set
            {
                ViewState["PanelHeight"] = value;
            }
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title
        {
            get
            {
                return PanelTitle;
            }
            set
            {
                PanelTitle = value;
            }
        }

        /// <summary>
        /// Ширина контрола
        /// </summary>
        public double Width
        {
            get
            {
                return PanelWidth;
            }
            set
            {
                PanelWidth = Convert.ToInt32(value);
                webPanel.Width = Unit.Parse(value.ToString());
            }
        }

        /// <summary>
        /// Высота контрола
        /// </summary> 
        [DefaultValue(null)]
        public double? Height
        {
            get
            {
                return PanelHeight;
            }
            set
            {
                if (value != null)
                {
                    PanelHeight = Convert.ToInt32(value);
                    //webPanel.Height = Unit.Parse(value.ToString());
                }
                else
                {                    
                    PanelHeight = 0;
                }
            }
        }

        /// <summary>
        /// Контрол панели
        /// </summary>
        public Object PanelHeader
        {
            get
            {
                return webPanel.Header;
            }
        }

        /// <summary>
        /// Можно ли выбирать предков
        /// </summary>
        public bool ParentSelect
        {
            get
            {
                return ViewState["ParentSelect"] == null ? false : (bool)ViewState["ParentSelect"];
            }
            set
            {
                ViewState["ParentSelect"] = value;
            }
        }

        public string SelectedNodeParent
        {
            get
            {
                if (treeView == null || treeView.Nodes.Count == 0)
                {
                    return null;
                }
                if (treeView.SelectedNode.Parent != null)
                {
                    return treeView.SelectedNode.Parent.Text;
                }
                else
                {
                    // пользователь ткнул в родителя, вернем его.
                    return treeView.SelectedNode.Text;
                }
            }
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                treeView.ClearAll();
            }
        }

        private static string ClipText(string source, int width)
        {
            //return string.Format("<div style='float:left;'><img width='15px' height='15px' src='../../images/starYellowBB.png'/></div><div style='height:15px;width:{1}px;text-overflow:ellipsis;overflow: hidden;white-space: nowrap;'>{0}</div>", source, width - 35);
            return string.Format("<div style='height:15px;width:{1}px;text-overflow:ellipsis;overflow: hidden;white-space: nowrap;'>{0}</div>", source, width - 20);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PanelWidth == 0)
            {
                PanelWidth = 100;
            }
            // Будем различать версию для печати и скрытие параметров
            if ((Session["PrintVersion"] != null && (bool)Session["PrintVersion"]) ||
                Session["ShowParams"] != null && !(bool)Session["ShowParams"])
            {
                this.Visible = false;
            }

            RefreshWebTitle();

            string browser = HttpContext.Current.Request.Browser.Browser;
            if (browser.Contains("IE"))
            {
                webPanel.Expanded = false;
            }
            else
            {
                webPanel.Expanded = panelExpanded;
            }
            
            treeView.Selectable = !MultiSelect;            
            webPanel.Header.CollapsedAppearance.Style.CssClass = "WebPanelComboHeader";
            webPanel.Header.ExpandedAppearance.Style.CssClass = "WebPanelComboHeader";

            webPanel.ClientSideEvents.ExpandedStateChanging = string.Format("{0}_ExpandedStateChanging", this.ID);

            webPanel.PanelStyle.CustomRules =
               string.Format("position: absolute; z-index: 50000; overflow: auto; clip: rect(0px, {0}px, {1}px, 0px); *padding-top: 22px; width: {0}px; height: {1}px;", 
                PanelWidth + 2,
                PanelHeight != 0 ? PanelHeight + 2 : 700);

            webPanel.PanelStyle.BackColor = Color.Transparent;
            treeView.BorderStyle = BorderStyle.Solid;
            treeView.BorderWidth = 1;
            treeView.BorderColor = Color.FromArgb(171, 193, 222);

            treeView.AutoPostBack = AutoPostBack;
            treeView.AutoPostBackFlags.NodeChanged = AutoPostBack;
        }

        /// <summary>
        /// Выбран ли узел
        /// </summary>
        /// <param name="node">узел</param>
        /// <returns>true, если выбран</returns>
        private bool IsSelectedNode(Node node)
        {
            return (!MultiSelect && node.Selected || MultiSelect && node.Checked);
        }

        /// <summary>
        /// Установить выделение элемента (для одиночного выбора)
        /// </summary>
        /// <param name="tree">дерево</param>
        /// <param name="value">значение выделяемого узла</param>
        /// <param name="check">выделенность</param>
        private void SetSelected(UltraWebTree tree, string value, bool check)
        {
            if (tree == null)
            {
                return;
            }

            foreach (Node node in tree.Nodes)
            {
                if (SetSelectedNode(node, value, check))
                {
                    return;
                }
            }
        }


        /// <summary>
        /// Установить выделение элемента (для одиночного выбора)
        /// </summary>
        /// <param name="node">узел</param>
        /// <param name="value">значение выделяемого узла</param>
        /// <param name="check">выделенность</param>
        private bool SetSelectedNode(Node node, string value, bool check)
        {
            if (node == null)
                return false;

            if (node.Text == value)
            {
                if (MultiSelect)
                {
                    node.Checked = check;
                    if (MultipleSelectionType == MultipleSelectionType.CascadeMultiple)
                    {
                        SetMultiSelect(node);
                    }
                }
                else
                {
                    node.Selected = check;
                }
                return true;
            }

            foreach (Node n in node.Nodes)
            {
                if (SetSelectedNode(n, value, check))
                {
                    return true;
                }
            }

            return false;
        }

        private void SetMultiSelect(Node node)
        {
            foreach (Node childNode in node.Nodes)
            {
                childNode.Checked = true;
                SetMultiSelect(childNode);
            }
        }


        /// <summary>
        /// Заполнение дерева элементами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void treeView_DataBinding(object sender, EventArgs e)
        {
            foreach (string key in valuesDictionary.Keys)
            {
                Node node = new Node();
                node.Text = key;
                node.DataKey = valuesDictionary[key];
                node.CheckBox = MultiSelect ? CheckBoxes.True : CheckBoxes.False;

                switch (TooltipVisibility)
                {
                    case TooltipVisibilityMode.Auto:
                        {
                            node.ToolTip = String.Empty;
                            break;
                        }
                    case TooltipVisibilityMode.Shown:
                        {
                            node.ToolTip = key;
                            break;
                        }
                }

                int level = valuesDictionary[key];
                if (level == 0)
                {
                    treeView.Nodes.Add(node);
                }
                else
                {
                    Node lastNode = GetLastNode(level - 1);
                    if (lastNode != null)
                    {
                        lastNode.Nodes.Add(node);
                    }
                }
            }
            SetCheckedDefault();
        }

        /// <summary>
        /// Установить состояние
        /// </summary>
        public void SetСheckedState(string value, bool check)
        {
            SetSelected(treeView, value, check);
            RefreshWebTitle();
        }

        /// <summary>
        /// Установить состояние множественному параметру
        /// </summary>
        public void SetMultipleСheckedState(string values, bool check)
        {
            string[] seletedValues = values.Split(',');
            foreach (string value in seletedValues)
            {
                SetSelected(treeView, value.TrimStart(' '), check);
            }
            
            RefreshWebTitle();
        }

        /// <summary>
        /// Установить состояние всем узлам
        /// </summary>
        public void SetAllСheckedState(bool check, bool onlyLeafs)
        {
            foreach (Node node in treeView.Nodes)
            {
                SetAllСheckedState(node, check, onlyLeafs);
            }
            RefreshWebTitle();
        }
        
        /// <summary>
        /// Установить состояние всем потомкам Root
        /// </summary>
        private void SetAllСheckedState(Node root, bool check, bool onlyLeafs)
        {
            if (!onlyLeafs || IsLeaf(root))
            {
                SetSelectedNode(root, check);
            }

            foreach (Node node in root.Nodes)
            {
                SetAllСheckedState(node, check, onlyLeafs);
            }
        }

        private static bool IsLeaf(Node node)
        {
            return node.Nodes.Count == 0;
        }

        public void SetSelectedNode(Node node, bool check)
        {
            if (MultiSelect)
            {
                node.Checked = check;
            }
            else
            {
                node.Selected = check;
            }
        }

        /// <summary>
        /// Обновление заголовка веб-панели
        /// </summary>
        public void RefreshWebTitle()
        {
            treeView.ClientSideEvents.InitializeTree = string.Format("{0}_InitTree", this.ID);

            if (!MultiSelect)
            {
                if (ShowSelectedValue)
                {
                    webPanel.Header.Text = ClipText(string.Format("{0}: {1}", PanelTitle, SelectedValue), PanelWidth);
                    webPanel.ToolTip = SelectedValue;
                }
                treeView.ClientSideEvents.NodeClick = string.Format("{0}_NodeClick", this.ID);
            }
            else
            {
                if (ShowSelectedValue)
                {
                    webPanel.Header.Text = ClipText(string.Format("{0}: {1}", PanelTitle, SelectedValuesString), PanelWidth);
                    webPanel.ToolTip = SelectedValuesString;
                }
                treeView.ClientSideEvents.NodeChecked = string.Format("{0}_NodeChecked", this.ID);
            }
        }

        /// <summary>
        /// Установить стиль элемента
        /// </summary>
        /// <param name="value">значение элемента</param>
        /// <param name="CSSClass">css-класс</param>
        public void SetNodeStyle(string value, string CSSClass)
        {
            foreach (Node node in treeView.Nodes)
            {
                if (node.Text == value)
                {
                    node.Style.CssClass = CSSClass;
                    return;
                }
            }
        }

        #region Рендеринг

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            StringBuilder scriptString = new StringBuilder();
            scriptString.Append(@"
                <script id='Infragistics' type='text/javascript'>
                <!--");

            string methodName = string.Format("{0}_InitTree", this.ID);
            scriptString.AppendFormat(
                @"
                    function {0}(treeId)
                    {{
                        var tree = igtree_getTreeById(treeId);
                        var element = tree.treeElement;
                        var style = element.style;
                        style.width = '{1}px';                                            
                    }}                      
                    ",
                 methodName, PanelWidth > 2 ? PanelWidth - 17: 102);

            if (!MultiSelect)
            {
                methodName = string.Format("{0}_NodeClick", this.ID);

                scriptString.AppendFormat(
                    @"
                    function {0}(treeId, nodeId, button)
                    {{
                        var node = igtree_getNodeById(nodeId);
                        var level = node.getLevel(); 
                        var parentSelect = {3};
                        var showSelectedValue = {4};
                        if ({5} && node.hasChildren()) 
                        {{ return; }}
                        if (!parentSelect && level == 0 && node.hasChildren())
                        {{
                            return;
                        }}
                        var panel = igpnl_getPanelById('{1}');
                        if (showSelectedValue)
                        {{
                            panel.getHeader()._element.lastChild.lastChild.data = '{2}: ' + node.getText();
                            panel.getHeader()._element.title = node.getText();
                        }}
                        panel.setExpanded(!panel._expanded);
                        
                        EnableSubmitButton();
                    }}                      
                    ",
                     methodName, webPanel.ClientID, PanelTitle, ParentSelect ? "true" : "false", ShowSelectedValue ? "true" : "false",
                     AllowSelectionType == AllowedSelectionType.LeafNodes ? "true" : "false");
            }
            else if (MultipleSelectionType == MultipleSelectionType.CascadeMultiple)
            {
                methodName = string.Format("{0}_NodeChecked", this.ID);
                scriptString.AppendFormat(
                    @"
                        var selectedValues = '';

                        function {0}(treeId, nodeId, bChecked)
                        {{
	                        var node = igtree_getNodeById(nodeId);

                            selectedValues = '';

	                        {1}_CascadeChecked(node, bChecked);
	                        {1}_CheckUpdate(treeId);

							if (selectedValues.length > 3)
							{{
								selectedValues = selectedValues.substring(2, selectedValues.length);
							}}

                            var panel = igpnl_getPanelById('{2}');
                            panel.getHeader()._element.title = selectedValues;
                            panel.getHeader()._element.lastChild.lastChild.data = '{3}: ' + selectedValues;							

	                        EnableSubmitButton();	
                        }}

                        function {1}_CascadeChecked(node, bChecked)
                        {{
                            node.setChecked(bChecked, false);
	                        if (node.hasChildren())
	                        {{
		                        var nodes = node.getChildNodes();
		                        for (var i = 0; i < nodes.length; i++)
		                        {{                                
                                    {1}_CascadeChecked(nodes[i], bChecked);
		                        }}
	                        }}
                        }}

                        function {1}_CheckUpdate(treeId)
                        {{
                            var tree = igtree_getTreeById(treeId);
                        	
                            var nodes = tree.getNodes(false);
	                        for (var i = 0; i < nodes.length; i++)
	                        {{
		                        checked = {1}_GetCheckStatus(nodes[i]);
                                if (checked)
								{{
									selectedValues +=  ', ' + nodes[i].getText();
								}}
	                        }}
                        }}

                        function {1}_GetCheckStatus(node)
                        {{
	                        var allChecked = true;                            
	                        var allUnChecked = true; 
                        	var indeterminateExists = false;
							var selectedChilds = '';

	                        if (node.hasChildren())
	                        {{
		                        var nodes = node.getChildNodes();
                                for (var i = 0; i < nodes.length; i++)
		                        {{
			                        checked = {1}_GetCheckStatus(nodes[i]);

                                    if (checked)
									{{
										selectedChilds += ', ' + nodes[i].getText();
									}}                        			

									childNodeCount = nodes[i].getElement().childNodes.length;
									indeterminated = nodes[i].getElement().childNodes[childNodeCount - 2].indeterminate;
									
			                        allChecked = allChecked && checked;
			                        allUnChecked = allUnChecked && !checked;
									indeterminateExists = indeterminateExists || indeterminated;
		                        }}
                        		
								{1}_SetCheckedStatus(node, allChecked && !indeterminateExists, allUnChecked && !indeterminateExists);

                                if (!allChecked)
								{{
									selectedValues += selectedChilds;
								}}		                        

								return allChecked;
	                        }}
	                        else
	                        {{
                                return node.getChecked();
	                        }}
                        }}
					
                        function {1}_SetCheckedStatus(node, allChecked, allUnChecked)
                        {{
	                        childNodeCount = node.getElement().childNodes.length;
							if (allChecked)
	                        {{
		                        node.setChecked(true, false);
		                        node.getElement().childNodes[childNodeCount - 2].indeterminate = false;
	                        }}
	                        else if (allUnChecked)
		                         {{
			                          node.setChecked(false, false);
			                          node.getElement().childNodes[childNodeCount - 2].indeterminate = false;
		                         }}
		                         else
		                         {{
									  node.setChecked(true, false);
			                          node.getElement().childNodes[childNodeCount - 2].indeterminate = true;
		                         }}
                        }}
                    ",
                    methodName, this.ID, webPanel.ClientID, PanelTitle);
            }
            else 
            {
                methodName = string.Format("{0}_NodeChecked", this.ID);
                scriptString.AppendFormat(
                    @"
                        var selectedValues = '';

                        function {0}(treeId, nodeId, bChecked)
                        {{
	                        var node = igtree_getNodeById(nodeId);

                            selectedValues = '';

	                        node.setChecked(bChecked, false);
	                        {1}_CheckUpdate(treeId);

							if (selectedValues.length > 3)
							{{
								selectedValues = selectedValues.substring(2, selectedValues.length);
							}}

                            var panel = igpnl_getPanelById('{2}');
                            panel.getHeader()._element.title = selectedValues;
                            panel.getHeader()._element.lastChild.lastChild.data = '{3}: ' + selectedValues;							

	                        EnableSubmitButton();	
                        }}

                        function {1}_CheckUpdate(treeId)
                        {{
                            var tree = igtree_getTreeById(treeId);
                        	
                            var nodes = tree.getNodes(false);
	                        for (var i = 0; i < nodes.length; i++)
	                        {{
		                        checked = {1}_GetCheckStatus(nodes[i]);
                                if (checked)
								{{
									selectedValues +=  ', ' + nodes[i].getText();
								}}
	                        }}
                        }}

                        function {1}_GetCheckStatus(node)
                        {{
							var selectedChilds = '';

	                        if (node.hasChildren())
	                        {{
		                        var nodes = node.getChildNodes();
                                for (var i = 0; i < nodes.length; i++)
		                        {{
			                        checked = {1}_GetCheckStatus(nodes[i]);

                                    if (checked)
									{{
										selectedValues += ', ' + nodes[i].getText();
									}}                        			
		                        }}
	                        }}

                            return node.getChecked();
                        }}
                    ",
                    methodName, this.ID, webPanel.ClientID, PanelTitle);
            }
            methodName = string.Format("{0}_ExpandedStateChanging", this.ID);
            scriptString.AppendFormat(
                    @"
                    function {0}(oWebPanel, oEvent)
                    {{
                        oWebPanel._contentPanelElement.style.width = '{1}px';
                        oWebPanel._contentPanelElement.style.height = '{2}px';
                    }}
                    ",
                    methodName, 
                    PanelWidth != 0 ? PanelWidth + 2 : 102,
                    PanelHeight != 0 ? PanelHeight : 700);
            scriptString.Append(@"
                -->
            </script>");
            writer.Write(scriptString.ToString());
        }


        #endregion

        #region Методы выделения узлов

        /// <summary>
        /// Получить выделенные узлы
        /// </summary>
        /// <param name="tree">дерево</param>
        private void GetMultiSelected(UltraWebTree tree)
        {
            if (tree == null || selectedNodes == null)
            {
                return;
            }

            foreach (Node node in tree.Nodes)
            {
                if (MultipleSelectionType == MultipleSelectionType.CascadeMultiple && AllChecked(node))
                {
                    selectedNodes.Add(node);
                }
                else
                {
                    GetMultiSelectedNode(node);
                }
            }
        }

        private bool AllChecked(Node root)
        {
            bool allChecked = IsSelectedNode(root);
            foreach (Node node in root.Nodes)
            {
                allChecked = allChecked && IsSelectedNode(node) && AllChecked(node);
            }
            return allChecked;
        }

        /// <summary>
        /// Получить выделенные узлы
        /// </summary>
        /// <param name="root">корневой узел</param>
        private void GetMultiSelectedNode(Node root)
        {
            if (root == null || selectedNodes == null)
            {
                return;
            }

            if (MultipleSelectionType == MultipleSelectionType.CascadeMultiple)
            {
                if (IsSelectedNode(root))
                {
                    bool allChecked = true;

                    // Проходим по верхнему уровню
                    foreach (Node node in root.Nodes)
                    {
                        // Если нет потомков, могли попасть сюда, только если есть невыбранные соседи (не чекнули родителя раньше)
                        if (node.Nodes.Count == 0 && IsSelectedNode(node))
                        {
                            // Добавляем узел
                            selectedNodes.Add(node);
                        }
                        // проверяем выбор дочерних 
                        allChecked = AllChecked(node);


                        // Если выбраны все дочерние и они существуют
                        if (allChecked && node.Nodes.Count > 0)
                        {
                            // Добавляем узел
                            selectedNodes.Add(node);
                        }
                        else
                        {
                            GetMultiSelectedNode(node);
                        }
                    }
                }
            }
            else
            {
                if (IsSelectedNode(root))
                {
                    selectedNodes.Add(root);
                }

                foreach (Node node in root.Nodes)
                {
                    GetMultiSelectedNode(node);
                }
            }
        }

        /// <summary>
        /// Получить выделенный узел
        /// </summary>
        /// <param name="tree">дерево</param>
        /// <returns>выделенный узел</returns>
        private Node GetSelected(UltraWebTree tree)
        {
            if (tree == null)
            {
                return null;
            }

            foreach (Node node in tree.Nodes)
            {
                Node result = GetSelectedNode(node);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Получить выделенный узел
        /// </summary>
        /// <param name="root">корневой узел</param>
        /// <returns>выделенный узел</returns>
        private Node GetSelectedNode(Node root)
        {
            if (IsSelectedNode(root))
            {
                if (AllowSelectionType == AllowedSelectionType.LeafNodes)
                {
                    while (root.Nodes.Count != 0)
                    {
                        root = root.Nodes[0];
                    }
                    return root;
                }
                // для выделенной группы узлов берем первого потомка, пока только для 0-го уровня
                if (root.Level == 0 && root.Nodes.Count != 0 && !ParentSelect)
                {
                    return root.Nodes[0];
                }
                else
                {
                    return root;
                }
            }

            foreach (Node node in root.Nodes)
            {
                Node result = GetSelectedNode(node);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        #endregion

        #region Методы заполнения данными

        /// <summary>
        /// Заполнить список значениями из коллекции
        /// </summary>
        public void FillValues(Collection<string> nameCollection)
        {
            valuesDictionary.Clear();
            for (int i = 0; i < nameCollection.Count; i++)
            {
                valuesDictionary.Add(nameCollection[i], 0);
            }
            treeView.DataBind();
        }

        /// <summary>
        /// Заполнить список значениями из словаря
        /// </summary>
        public void FillDictionaryValues(Dictionary<string, int> nameCollection)
        {
            valuesDictionary.Clear();
            valuesDictionary = nameCollection;
            treeView.DataBind();
        }

        private FillComboMode fillMode;

        /// <summary>
        /// Заполнить список значениями из xml
        /// </summary>
        public void FillXmlValues(XmlDocument xmlDoc)
        {
            treeView.ReadXmlDoc(xmlDoc, true, false);
            fillMode = FillComboMode.XmlDocument;
            treeView.DataBind();
        }

        #endregion

        public string GetRootNodesName(int index)
        {
            return treeView.Nodes[index].Text;
        }

        public int GetRootNodesCount()
        {
            return treeView.Nodes.Count;
        }

        public void ClearNodes()
        {
            treeView.Nodes.Clear();
        }

        private void SetCheckedDefault()
        {
            // Если не удалось выбрать узел
            if (treeView.SelectedNode == null && treeView.Nodes.Count != 0)
            {
                // Если дерево иерархическое
                if (treeView.Nodes[0].Nodes != null &&
                    treeView.Nodes[0].Nodes.Count > 0)
                {
                    treeView.Nodes[0].Nodes[0].Selected = true;
                }
                else
                {
                    treeView.Nodes[0].Selected = true;
                }
            }
        }

        /// <summary>
        /// Выделение последнего узла
        /// </summary>
        public void SelectLastNode()
        {
            if (treeView.Nodes.Count != 0)
            {
                SelectLastNode(treeView.Nodes[treeView.Nodes.Count - 1]);
            }
        }

        private void SelectLastNode(Node node)
        {
            if (node.Nodes.Count == 0)
            {
                SetSelectedNode(node, true);
                node.Checked = true;
            }
            else
            {
                SelectLastNode(node.Nodes[node.Nodes.Count - 1]);
            }
        }

        /// <summary>
        /// Получение последнего узла заданного уровня
        /// </summary>
        public Node GetLastNode(int level)
        {
            if (treeView.Nodes.Count != 0)
            {
                return GetLastNode(treeView.Nodes[treeView.Nodes.Count - 1], level);
            }

            return null;
        }

        /// <summary>
        /// Получение последнего узла заданного уровня
        /// </summary>
        /// <param name="node">родительский узел</param>
        /// <param name="level">уровень</param>
        /// <returns>узел</returns>
        private static Node GetLastNode(Node node, int level)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Level == level)
            {
                return node;
            }
            else
            {
                return GetLastNode(node.Nodes[node.Nodes.Count - 1], level);
            }
        }

        /// <summary>
        /// Получение самого последнего узла
        /// </summary>
        /// <param name="node">родительский узел</param>
        /// <returns></returns>
        private static Node GetLastNode(Node node)
        {
            if (node.Nodes.Count == 0)
            {
                return node;
            }
            else
            {
                return GetLastNode(node.Nodes[node.Nodes.Count - 1]);
            }
        }

        public string GetSelectedNodePath()
        {
            Node node = GetSelected(treeView);
            return GetNodePath(node);
        }

        public string GetNodePath(Node node)
        {
            string path = string.Empty;

            while (node != null)
            {
                path = string.Format("{0}|{1}", node.Text, path);
                node = node.Parent;
            }

            return path;
        }

        public Node GetPreviousSublingNode(Node node)
        {
            if (node.PrevNode != null)
            {
                return node.PrevNode;
            }
            Node parentNode = node.Parent;
            if ((parentNode.PrevNode == null) || (parentNode.PrevNode.Nodes.Count == 0))
            {
                Node parentParentNode = parentNode.Parent;
                if ((parentParentNode.PrevNode != null) && (parentParentNode.PrevNode.Nodes.Count != 0))
                {
                    return parentParentNode.PrevNode.Nodes[parentParentNode.PrevNode.Nodes.Count - 1];
                }
                return null;
            }
            return parentNode.PrevNode.Nodes[parentNode.PrevNode.Nodes.Count - 1];
        }



        public string GetPreviousSublingNodePath(Node node)
        {
            if (node.PrevNode != null)
            {
                return GetNodePath(node.PrevNode);
            }
            else
            {
                Node parentNode = node.Parent;

                if (parentNode.PrevNode == null || parentNode.PrevNode.Nodes.Count == 0)
                {
                    Node parentParentNode = parentNode.Parent;
                    if (parentParentNode.PrevNode != null && parentParentNode.PrevNode.Nodes.Count != 0)
                    {
                        return GetNodeLastChild(parentParentNode.PrevNode.Nodes[parentParentNode.PrevNode.Nodes.Count - 1]);

                    }
                    return string.Empty;
                }

                return GetNodePath(parentNode.PrevNode.Nodes[parentNode.PrevNode.Nodes.Count - 1]);
            }
        }

        // Получение последнего потомка узла
        public string GetNodeLastChild(Node parent)
        {
            if (parent.Nodes.Count == 0)
            {
                return GetNodePath(parent);
            }
            else
            {
                return GetNodePath(parent.Nodes[parent.Nodes.Count - 1]);
            }
        }

        // Получение последнего потомка узла
        public Node GetLastChild(Node parent)
        {
            if (parent.Nodes.Count == 0)
            {
                return parent;
            }
            else
            {
                return parent.Nodes[parent.Nodes.Count - 1];
            }
        }

        /// <summary>
        /// Удаление узла по имени
        /// </summary>
        public void RemoveTreeNodeByName(string nodeName)
        {
            foreach (Node node in treeView.Nodes)
            {
                // удаляем, если это лист
                if (node.Text.ToLower() == nodeName.ToLower() && node.Nodes.Count == 0)
                {
                    treeView.Nodes.Remove(node);

                    return;
                }
                else
                {
                    RemoveNodeByName(node, nodeName);
                }
            }
        }

        private static void RemoveNodeByName(Node root, string nodeName)
        {
            foreach (Node node in root.Nodes)
            {
                // удаляем, если это лист
                if (node.Text.ToLower() == nodeName.ToLower() && node.Nodes.Count == 0)
                {
                    root.Nodes.Remove(node);
                    return;
                }
                else
                {
                    RemoveNodeByName(node, nodeName);
                }
            }
        }

        /// <summary>
        /// Переименование узла
        /// </summary>
        public void RenameTreeNodeByName(string oldName, string newName)
        {
            foreach (Node node in treeView.Nodes)
            {
                if (node.Text.ToLower() == oldName.ToLower())
                {
                    node.Text = newName;
                }
                else
                {
                    RenameNodeByName(node, oldName, newName);
                }
            }
        }

        private static void RenameNodeByName(Node root, string oldName, string newName)
        {
            foreach (Node node in root.Nodes)
            {
                if (node.Text.ToLower() == oldName.ToLower())
                {
                    root.Nodes.Remove(node);
                }
                else
                {
                    RenameNodeByName(node, oldName, newName);
                }
            }
        }
    }

    public enum FillComboMode
    {
        Collection = 0,
        Dictionary,
        XmlDocument
    }
}
