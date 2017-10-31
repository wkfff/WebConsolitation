#region USING

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Infragistics.Win.AppStyling;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Utils.DTSGenerator.SMOObjects;
using Krista.FM.Utils.DTSGenerator.TreeObjects;
using Resources=Krista.FM.Utils.DTSGenerator.Properties.Resources;
using Krista.FM.Utils.DTSGenerator.Wizards;
using Krista.FM.Client.Components;
using System.Drawing;

#endregion

namespace Krista.FM.Utils.DTSGenerator
{
    public partial class SSISMainForm : Form
    {
        #region Fields

        /// <summary>
        /// Схема - источник
        /// </summary>
        private IScheme sourceScheme = null;

        private IDatabase destdatabase = null;

        /// <summary>
        /// Схема - преемник
        /// </summary>
        private IScheme destinationScheme = null;
        private ImageList imageList;

        private LogicalCallContextData context;

        #endregion

        #region Constructor

        public SSISMainForm()
        {
            InitializeComponent();

            InitializeUltraComponents();

            this.imageList = new ImageList();
            InitializeImageList();

            // Настройка среды .NET Remoting
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.FriendlyName + ".config", false);

            ultraTreePackages.MouseClick += new MouseEventHandler(ultraTreePackages_MouseClick);
            ultraGrid.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ultraGrid_OnInitializeRow);
            ultraGrid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ultraGrid_OnGridInitializeLayout);
            ultraGrid.OnGetHierarchyInfo += new Krista.FM.Client.Components.GetHierarchyInfo(ultraGrid_OnGetHierarchyInfo);
            // Зарузка стиля
            StyleManager.Load(@"RedPlanet.isl");
        }

        private void InitializeImageList()
        {
            imageList.TransparentColor = Color.Lime;

            imageList.Images.AddRange(new Image[] { Properties.Resources.ClassGreen,
                                                    Properties.Resources.Class,
                                                    Properties.Resources.ClassBlue, 
                                                    Properties.Resources.ClassYellow, 
                                                    Properties.Resources.Folder,
                                                    Properties.Resources.Attribute,
                                                    Properties.Resources.Добавить_зависимые_пакеты,
                                                    Properties.Resources.Ошибка,
                                                    Properties.Resources.Применить_изменения_16х16_Вариант4});
        }

        Krista.FM.Client.Components.HierarchyInfo ultraGrid_OnGetHierarchyInfo(object sender)
        {
            HierarchyInfo hi = GetHierarchyInfo();
            return hi;
        }

        private static HierarchyInfo GetHierarchyInfo()
        {
            HierarchyInfo newHrInfo = new HierarchyInfo();

            newHrInfo.ParentRefClmnName = "clmnParentID";
            newHrInfo.ParentClmnName = "clmnID";

            newHrInfo.LevelsCount = 3;
            newHrInfo.CurViewState = ViewState.Hierarchy;

            return newHrInfo;
        }

        void ultraGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                band.ColHeadersVisible = false;
                band.GroupHeadersVisible = false;

                UltraGridColumn clmn = band.Columns["clmnID"];
                clmn.Hidden = true;

                clmn = band.Columns.Insert(0, "clmnImage");
                clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                clmn.Header.Caption = String.Empty;
                clmn.Width = 20;

                clmn = band.Columns.Insert(1, "clmnImageType");
                clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                clmn.Header.Caption = String.Empty;
                clmn.Width = 20;

                clmn = band.Columns["clmnObject"];
                clmn.Header.Caption = "Объект";
                clmn.Header.Enabled = true;
                clmn.Width = 250;

                clmn = band.Columns["clmnObjectType"];
                clmn.Hidden = true;

                clmn = band.Columns["clmnError"];
                clmn.Header.Caption = "Ошибка";
                clmn.Header.Enabled = true;
                clmn.Width = 400;

                clmn = band.Columns["clmnParentID"];
                clmn.Hidden = true;
            }
        }

        void ultraGrid_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Row.Cells["clmnError"].Value.ToString()))
                e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[6];
            else
            {
                e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[7];
            }

            switch (e.Row.Cells["clmnObjectType"].Value.ToString())
            {
                case "Package":
                    e.Row.Cells["clmnImageType"].Appearance.ImageBackground = imageList.Images[4];
                    break;
                case "Entity":
                    e.Row.Cells["clmnImageType"].Appearance.ImageBackground = imageList.Images[1];
                    break;
                case "Attribute":
                    e.Row.Cells["clmnImageType"].Appearance.ImageBackground = imageList.Images[5];
                    break;
            }
        }

        /// <summary>
        /// Схема - источник
        /// </summary>
        public IScheme SourceScheme
        {
            get { return sourceScheme; }
        }

        /// <summary>
        /// Схема - преемник
        /// </summary>
        public IScheme DestinationScheme
        {
            get { return destinationScheme; }
        }

        #endregion

        #region Инициализация компонентов Infragistics

        /// <summary>
        /// Инициализация статусбара
        /// </summary>
        private void InitializeUltraComponents()
        {
            //InfragisticComponentsCustomize.CustomizeInfragisticsControl(ultraGrid);
            //InfragisticComponentsCustomize.CustomizeUltraTabControl(ultraTabControl);
            //InfragisticComponentsCustomize.CustomizeUltraToolbarsManager(ultraToolbarsManager);

            ultraGrid.ugData.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

            ssisStatusBar.CreateUltraStatusPanels();

            ssisStatusBar.PanelSourceScheme.Text = "Источник: нет подключения";
            ssisStatusBar.PanelDestinationScheme.Text = "Преемник: нет подключения";
            ssisStatusBar.PanelUserName.Text = string.Format("Пользователь: {0}", Environment.UserName);
        }

        #endregion

        #region Connect To Scheme

        public static IScheme ConnectToScheme()
        {
            string ServerName = "";
            string SchemeName = "";
            string Login = "";
            string Password = "";
            IScheme ProxyScheme = null;
            bool offlineMode = false;
            AuthenticationType _authType = AuthenticationType.atWindows;
            
            while (true)
            {
                // показываем окно с диалогом подключения к схеме
                if (frmLogon.ShowLogonForm(ref ServerName, ref SchemeName, ref Login, ref Password,
                                           ref ProxyScheme, ref offlineMode, ref _authType))
                {
                    if (ProxyScheme != null || offlineMode)
                    {
                        // если успешно подключились - создаем воркплайс
                        return ProxyScheme;
                        // ... и выходим из цикла
                    }
                    else
                    {
                        // если нет - предлагаем поключится заново
                        if (MessageBox.Show("Не удалось подключиться к схеме. Повторить?", "Ошибка подключения",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) break;
                    }
                }
                    // если пользователь не захотел подключаться - выходим из цикла
                else break;
            }
            // в текущей конфигурации без схемы мы работать не можем
            return null;
        }

        #endregion

        #region Инициализация дерева объектов

        private void TreeViewInitialize()
        {
            Operation operation = new Operation();
            try
            {
                ultraTreePackages.Nodes.Clear();

                operation.Text = "Инициализация...";
                operation.StartOperation();

                SSISPackageObject obj =
                    new SSISPackageObject(Guid.NewGuid(), SourceScheme.RootPackage.Name, SourceScheme.RootPackage);
                SSISPackageNode packageNodeRoot = new SSISPackageNode(obj, new SSISSMOPackage(obj));
                TreeViewInitialize(SourceScheme.RootPackage, packageNodeRoot);

                packageNodeRoot.Expanded = true;
                ultraTreePackages.Nodes.Add(packageNodeRoot);
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        private static void TreeViewInitialize(IPackage rootPackage, SSISBaseTreeNode node)
        {
            foreach (KeyValuePair<string, IPackage> pair in rootPackage.Packages)
            {
                SSISPackageObject obj = new SSISPackageObject(Guid.NewGuid(), pair.Value.Name, pair.Value);

                SSISEntitiesObject objE = null;
                foreach (IEntity entity in pair.Value.GetSortEntitiesByPackage())
                {
                    objE = new SSISEntitiesObject(Guid.NewGuid(), entity.FullCaption, entity);
                    obj.SsisEntities.Add(entity.ObjectKey, objE);
                }

                SSISPackageNode packageNode = new SSISPackageNode(obj, new SSISSMOPackage(obj));

                TreeViewInitialize(pair.Value, packageNode);

                node.Nodes.Add(packageNode);
            }
        }

        #endregion

        #region Обработчик тулбара дерева объектов

        private void ultraToolbarsManagerTree_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "SelectAll":
                    {
                        SelectAllNodes(true);
                        break;
                    }
                case "unchecked":
                    {
                        SelectAllNodes(false);
                        break;
                    }
                case "CheckPackages":
                    {
                        CheckPackages();

                        break;
                    }
                case "AddDependentPackages":
                    {
                        AddDependentPackages();
                        break;
                    }
                case "Refresh":
                    {
                        RefreshStatusTree();
                        break;
                    }
                case "Create":
                    {
                        CreateRackagesCollection();
                        break;
                    }
                default:
                    throw new Exception("Обработчик не реализован");
            }
        }
               
        #endregion

        #region Создание коллекции пакетов для переноса данных

        /// <summary>
        /// 
        /// </summary>
        private void CreateRackagesCollection()
        {
            listBox.Items.Clear();

            foreach (UltraTreeNode node in ultraTreePackages.Nodes)
            {
                Create(node);
            }
        }

        private void Create(UltraTreeNode node)
        {
            if (node.CheckedState == CheckState.Checked)
            {
                listBox.Items.Add(node.Text);

                foreach (
                    KeyValuePair<string, IEntity> pair in
                        ((IPackage) ((SSISPackageNode) node).ControlOblect.ControlObject).Classes)
                {
                    if (pair.Value.ClassType != ClassTypes.clsFixedClassifier)
                        listBox.Items.Add('\t' + pair.Value.FullCaption);
                }
            }

            foreach (UltraTreeNode treeNode in node.Nodes)
            {
                Create(treeNode);
            }
        }

        #endregion

        #region Добавление зависимых пакетов

        private void AddDependentPackages()
        {
            Operation operation = new Operation();
            try 
            {
                operation.Text = "Добавление зависимых пакетов";
                operation.StartOperation();
                // коллекция уже выделенных объектов
                List<string> selectedNames = DefineSelectedNodesInTree();

                foreach (UltraTreeNode node in ultraTreePackages.Nodes)
                {
                    AddDependentPackages(node, selectedNames);
                }

                RefreshStatusTree();
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        #region Сформировать кооллекцию уже выделенных объектов

        /// <summary>
        /// Сразу определим, какие узлы уже выделены
        /// </summary>
        /// <returns></returns>
        private List<string> DefineSelectedNodesInTree()
        {
            List<string> list = new List<string>();
            foreach (UltraTreeNode node in ultraTreePackages.Nodes)
            {
                DefineSelectedNodesInTree(node, list);
            }

            return list;
        }

        private void DefineSelectedNodesInTree(UltraTreeNode node, List<string> list)
        {
            if (node.CheckedState == CheckState.Checked)
            {
                list.Add(node.Text);

                if (node.Parent != null)
                    CheckRootNode(node.Parent);
            }

            foreach (UltraTreeNode treeNode in node.Nodes)
            {
                DefineSelectedNodesInTree(treeNode, list);
            }
        }

        private void CheckRootNode(UltraTreeNode node)
        {
            if (node.CheckedState == CheckState.Unchecked)
                node.CheckedState = CheckState.Indeterminate;

            if (node.Parent != null)
                CheckRootNode(node.Parent);
        }

        #endregion


        private void AddDependentPackages(UltraTreeNode node, List<string> selectedNames)
        {
            if (node.CheckedState == CheckState.Checked && node.Text != "Корневой пакет")
            {
                List<string> collection =
                    ((IPackage) ((SSISPackageNode) node).ControlOblect.ControlObject).GetDependentsByPackage();
                foreach (string s in collection)
                {
                    if (!selectedNames.Contains(s))
                    {
                        // надо добавить узел
                        FindAndCheckNode(s, selectedNames);
                    }
                }
            }

            foreach (UltraTreeNode treeNode in node.Nodes)
            {
                AddDependentPackages(treeNode, selectedNames);
            }
        }

        #region найти и выделить узел по его имени

        private void FindAndCheckNode(string s, List<string > selectedNames)
        {
            foreach (UltraTreeNode node in ultraTreePackages.Nodes)
            {
                FindAndCheckNode(node, s, selectedNames);
            }
        }

        private void FindAndCheckNode(UltraTreeNode node, string s, List<string> selectedNames)
        {
            if (node.Text == s)
            {
                node.CheckedState = CheckState.Checked;

                if (node.Parent != null)
                    CheckRootNode(node.Parent);
                selectedNames.Add(s);
            }

            foreach (UltraTreeNode treeNode in node.Nodes)
            {
                FindAndCheckNode(treeNode, s, selectedNames);
            }
        }

        #endregion


        #endregion

        #region Работа с поиском потенциальных ошибок

        private void CheckPackages()
        {
            Operation opeation = new Operation();
            try
            {
                opeation.Text = "Поиск потенциальных ошибок";
                opeation.StartOperation();

                using (DataTable table = CheckSelectedPackages())
                {
                    using (DataSet ds = new DataSet())
                    {
                        ds.Tables.Add(table);
                        ds.Relations.Add(table.Columns["clmnID"], table.Columns["clmnParentID"]);

                        ultraGrid.DataSource = ds;
                    }
                }

                ultraTabControl.SelectedTab = ultraTabControl.Tabs[3];
            }
            finally
            {
                opeation.StopOperation();
                opeation.ReleaseThread();
            }
        }

       

        /// <summary>
        /// таблица с потенциальными ошибками, которые могут возникнуть при переносе данных
        /// </summary>
        /// <returns></returns>
        public DataTable CheckSelectedPackages()
        {
            if (DestinationScheme == null)
                throw new Exception("Отсутствует подключение к преемнику.");

            DataTable table = new DataTable();

            // Инициализация столбцов таблицы
            DataColumn clmnID = new DataColumn("clmnID");
            clmnID.DataType = System.Type.GetType("System.Int32");
            clmnID.AutoIncrement = true;
            clmnID.AutoIncrementSeed = 1;
            clmnID.AutoIncrementStep = 1;
            clmnID.ReadOnly = true;

            clmnID.AutoIncrement = true;
            DataColumn clmnObject = new DataColumn("clmnObject");
            DataColumn clmnObjectType = new DataColumn("clmnObjectType");
            DataColumn clmnError = new DataColumn("clmnError");
            DataColumn clmnParendID = new DataColumn("clmnParentID");
            clmnParendID.DataType = System.Type.GetType("System.Int32");
            
            table.Columns.AddRange(new DataColumn[]
                                       {clmnID, clmnObject, clmnObjectType, clmnError, clmnParendID});
            
            ValidatePackages(table);

            return table;
        }

        /// <summary>
        /// Обработка дерева объектов
        /// </summary>
        /// <param name="table"></param>
        private void ValidatePackages(DataTable table)
        {
            foreach (UltraTreeNode node in ultraTreePackages.Nodes)
            {
                ValidateRootNode(node, table);
            }
        }

        /// <summary>
        /// Обработка конкретного узла
        /// </summary>
        /// <param name="node"></param>
        /// <param name="table"></param>
        private void ValidateRootNode(UltraTreeNode node, DataTable table)
        {
            if(node.CheckedState == CheckState.Checked)
            {
                SSISPackageNode packageNode = node as SSISPackageNode;
    
                if (packageNode != null)
                {
                    HandlePackageNode(packageNode.ControlOblect, table);
                }
            }

            foreach (UltraTreeNode treeNode in node.Nodes)
            {
                ValidateRootNode(treeNode, table);
            }
        }

        /// <summary>
        /// обработка узла-пакета
        /// </summary>
        /// <param name="packageNode"></param>
        /// <param name="table"></param>
        public bool HandlePackageNode(SSISPackageObject packageObject, DataTable table)
        {
            bool checkValidate = true;

            try
            {
                // ошибка при валидации
                string errorSring = string.Empty;
                // ID родительской записи
                int parentID = -1;

                IPackage packageInDestinationScheme = FindPackageByName(packageObject.Name);
                if (packageInDestinationScheme == null)
                {
                    errorSring = String.Format("В схеме {0} не найден пакет {1}", DestinationScheme.Name,
                                               packageObject.Name);
                }

                DataRow row = table.NewRow();
                row["clmnObject"] = packageObject.Name;
                row["clmnObjectType"] = "Package";
                row["clmnError"] = errorSring;
                table.Rows.Add(row);

                parentID = Convert.ToInt32(row[0]);
                if (!String.IsNullOrEmpty(errorSring))
                    return false;

                errorSring = string.Empty;

                foreach (SSISEntitiesObject entityObject in packageObject.SsisEntities.Values)
                {
                    IEntity entity = entityObject.ControlObject as IEntity;
                    
                    if (entity.ClassType != ClassTypes.clsFixedClassifier)
                    {
                        IEntity en = DestinationScheme.RootPackage.FindEntityByName(entity.ObjectKey);
                        if (en == null)
                            errorSring = String.Format("В пакете {0} не найден класс {1}",
                                                       packageInDestinationScheme.Name,
                                                       entity.FullCaption);
                        row = table.NewRow();
                        row["clmnObject"] = entity.FullCaption;
                        row["clmnObjectType"] = "Entity";
                        row["clmnError"] = errorSring;
                        row["clmnParentID"] = parentID;
                        table.Rows.Add(row);

                        int parentEntityID = Convert.ToInt32(row[0]);
                        if (!String.IsNullOrEmpty(errorSring))
                        {
                            errorSring = String.Empty;
                            UpdateErrorMessage(table, "При валидации пакета возникли ошибки", parentID);

                            checkValidate = false;

                            continue;
                        }

                        try
                        {
                            if (entity.Presentations.Count != 0)
                            {
                                continue;
                            }

                            foreach (KeyValuePair<string, IDataAttribute> attribute in entity.Attributes)
                            {
                                if (!en.Attributes.ContainsKey(attribute.Key) && attribute.Value.Name != "ID")
                                    errorSring = String.Format("В классе {0} не найден атрибут {1}", en.FullCaption,
                                                               attribute.Value.Name);
                                row = table.NewRow();
                                row["clmnObject"] = attribute.Value.Name;
                                row["clmnObjectType"] = "Attribute";
                                row["clmnError"] = errorSring;
                                row["clmnParentID"] = parentEntityID;
                                table.Rows.Add(row);

                                if (!String.IsNullOrEmpty(errorSring))
                                {
                                    UpdateErrorMessage(table, "При валидации объекта возникли ошибки", parentEntityID);
                                    checkValidate = false;
                                }
                                errorSring = string.Empty;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }

                    table.AcceptChanges();
                }

                return checkValidate;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Обновлят сообщение об ошибке у родительских записей
        /// </summary>
        /// <param name="table"></param>
        /// <param name="errorSring"></param>
        /// <param name="parentID"></param>
        private void UpdateErrorMessage(DataTable table, string errorSring, int parentID)
        {
            DataRow[] rows = table.Select(String.Format("clmnID = {0}", parentID));
            foreach (DataRow dataRow in rows)
            {
                dataRow["clmnError"] = errorSring;
            }
        }

        /// <summary>
        /// Поиск пакета в схеме-преемнике
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private IPackage FindPackageByName(string name)
        {
            return Find(DestinationScheme.RootPackage, name);
        }

        private IPackage Find(IPackage iPackage, string  name)
        {
            if (iPackage.Name == name)
                return iPackage;
            else
            {
                IPackage package = null;
                foreach (KeyValuePair<string, IPackage> pair in iPackage.Packages)
                {
                    package = Find(pair.Value, name);

                    if (package != null)
                        break;
                }

                return package;
            }
        }

        #endregion

        #region Выделение/снятие выделения у объектов дерева

        private void SelectAllNodes(bool selected)
        {
            foreach (UltraTreeNode node in ultraTreePackages.Nodes)
            {
                SelectAllNodes(node, selected);
            }

            RefreshStatusTree();
        }

        private void SelectAllNodes(UltraTreeNode node, bool selected)
        {
            foreach (UltraTreeNode treeNode in node.Nodes)
                SelectAllNodes(treeNode, selected);

            if (selected)
                node.CheckedState = CheckState.Checked;
            else
                node.CheckedState = CheckState.Unchecked;
        }

        #endregion

        #region Работа в вклаками

        private void ultraTabControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if(ultraTabControl.SelectedTab.Index == 1)
            {
                HandleTabEntities();
            }

            if (ultraTabControl.SelectedTab.Index == 2)
            {
                HandleTabAttributes();
            }
        }

        private void HandleTabAttributes()
        {
            ultraTreeAttributes.Nodes.Clear();

            if (ultraTreeEntities.SelectedNodes.Count == 1)
            {
                SSISEntitiesNode node = ultraTreeEntities.SelectedNodes[0] as SSISEntitiesNode;
                if (node != null)
                {
                    SSISEntitiesObject obj =
                        new SSISEntitiesObject(Guid.NewGuid(), node.ControlOblect.Name,
                                               (IEntity) node.ControlOblect.ControlObject);
                    SSISEntitiesNode n = new SSISEntitiesNode(obj, new SSISSMOEntity(obj, SourceScheme.SchemeDWH.ConnectionString));

                    foreach (KeyValuePair<string, IDataAttribute> pair in ((IEntity)node.ControlOblect.ControlObject).Attributes)
                    {
                        SSISAttributeObject objA = new SSISAttributeObject(Guid.NewGuid(), pair.Value.Caption, pair.Value);
                        SSISAttributeNode nodeAtt = new SSISAttributeNode(objA, new SSISSMOAttribute(objA));
                        if (node.CheckedState == CheckState.Checked)
                            nodeAtt.CheckedState = CheckState.Checked;
                        n.Nodes.Add(nodeAtt);
                    }

                    n.Expanded = true;
                    ultraTreeAttributes.Nodes.Add(n);
                }
            }
        }

        private void HandleTabEntities()
        {
            ultraTreeEntities.Nodes.Clear();

            if (ultraTreePackages.SelectedNodes.Count == 1)
            {
                SSISPackageNode node = ultraTreePackages.SelectedNodes[0] as SSISPackageNode;
                if (node != null)
                {
                    foreach (SSISEntitiesObject ssisEntitiesObject in node.ControlOblect.SsisEntities.Values)
                    {
                        SSISEntitiesNode nodeEn =
                                new SSISEntitiesNode(ssisEntitiesObject
                                                     , new SSISSMOEntity(ssisEntitiesObject, SourceScheme.SchemeDWH.ConnectionString));
                        if (node.CheckedState == CheckState.Checked)
                            nodeEn.CheckedState = CheckState.Checked;
                        ultraTreeEntities.Nodes.Add(nodeEn);
                    }
                }
            }
        }

        #endregion

        #region Обработчики нажатия мыши

         /// <summary>
        /// Обработчик нажатия клавиши на дереве, для вызова контекстного меню объекта дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ultraTreePackages_MouseClick(object sender, MouseEventArgs e)
        {
            SSISPackageNode node;

            node = ultraTreePackages.GetNodeFromPoint(e.X, e.Y) as SSISPackageNode;

            if (node != null)
            {
                // контекстное меню появись!
                if (e.Button == MouseButtons.Right)
                {
                    System.Windows.Forms.ContextMenuStrip cms = new ContextMenuStrip();
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Name = "PartialTransfer";
                    item.Text = "Перенести данные по новым источникам для блока";
                    item.Click += new EventHandler(item_Click);
                    item.Tag = node.ControlOblect.ControlObject;
                    cms.Items.Add(item);

                    cms.Show(ultraTreePackages, e.X, e.Y);
                }
                if (e.Button == MouseButtons.Left)
                {
                    propertyGrid.SelectedObject = node.SmoObject;
                }
            }
        }

        void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            switch (item.Name)
            {
                case "PartialTransfer":
                    SSISPackageObject obj = new SSISPackageObject(new Guid(), "Test", null);
                    PartialWizardForm.ShowPartialWizardForm((IPackage)item.Tag, this, ref obj);
                    if (obj.Name != "Test")
                    {
                        CreateDTSXPForm form = new CreateDTSXPForm(context, obj, SourceScheme, DestinationScheme, destdatabase);
                        form.ShowDialog();
                    }
                    break;
            }
        }


        private void ultraTreeEntities_MouseClick(object sender, MouseEventArgs e)
        {
            SSISEntitiesNode node;

            node = ultraTreeEntities.GetNodeFromPoint(e.X, e.Y) as SSISEntitiesNode;

            if (node != null)
            {
                // контекстное меню появись!
                if (e.Button == MouseButtons.Right)
                {
                }
                if (e.Button == MouseButtons.Left)
                {
                    propertyGrid.SelectedObject = node.SmoObject;
                }
            }
        }

        private void ultraTreeAttributes_MouseClick(object sender, MouseEventArgs e)
        {
            SSISAttributeNode node;

            node = ultraTreeAttributes.GetNodeFromPoint(e.X, e.Y) as SSISAttributeNode;

            if (node != null)
            {
                // контекстное меню появись!
                if (e.Button == MouseButtons.Right)
                {
                }
                if (e.Button == MouseButtons.Left)
                {
                    propertyGrid.SelectedObject = node.SmoObject;
                }
            }
        }

        #endregion

        #region Обработчик основного тулбара 

        private void ultraToolbarsManager_ToolClick_1(object sender, ToolClickEventArgs e)
        {
            LogicalCallContextData context1 = LogicalCallContextData.GetContext();

            switch (e.Tool.Key)
            {
                case "sourceConnect":
                case "sourceConnectBt":
                    {
                        sourceScheme = ConnectToScheme();
                        if (SourceScheme != null)
                        {
                            ssisStatusBar.PanelSourceScheme.Text =
                                String.Format("Источник: {0}\\{1}\\{2}", SourceScheme.Server.Machine, SourceScheme.Name, SourceScheme.SchemeDWH.DataBaseName);

                            TreeViewInitialize();

                            e.Tool.SharedProps.AppearancesSmall.Appearance.Image =
                                Resources.Connect;
                            context = LogicalCallContextData.GetContext();
                            LogicalCallContextData.SetContext(context1);
                        }
                        break;
                    }
                case "destinationConnect":
                case "destinationConnectBt":
                    {
                        destinationScheme = ConnectToScheme();
                        if (DestinationScheme != null)
                        {
                            destdatabase = DestinationScheme.SchemeDWH.DB;

                            ssisStatusBar.PanelDestinationScheme.Text =
                                String.Format("Преемник: {0}\\{1}\\{2}", DestinationScheme.Server.Machine,
                                              DestinationScheme.Name, DestinationScheme.SchemeDWH.DataBaseName);
                            e.Tool.SharedProps.AppearancesSmall.Appearance.Image =
                                Resources.Connect;
                            LogicalCallContextData.SetContext(context1);
                        }
                        break;
                    }
                case "setStyle":
                    {
                        Setstyle();
                        break;
                    }

                default:
                    throw new Exception("Обработчик не реализован");
            }
        }

        #endregion

        #region Вспомагательные функции

        /// <summary>
        /// Настройка единого стиля
        /// </summary>
        private void Setstyle()
        {
            appStylistRuntime.ShowRuntimeApplicationStylingEditor(this, "Выбор оформления");
        }

        /// <summary>
        /// Обновляет на статус баре количество выделенных объектов
        /// </summary>
        private void RefreshStatusTree()
        {
            ultraStatusBar.Text = String.Format("Выделено объектов: {0}", DefineSelectedNodesInTree().Count);
        }

        #endregion

        #region Обработчики меню списка объектов для переноса

        private void ultraToolbarsManagerTransfer_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "Transfer":
                    {
                        CreateDTSXPForm form = new CreateDTSXPForm(context,(SSISPackageNode)ultraTreePackages.Nodes[0], SourceScheme, DestinationScheme, destdatabase);
                        form.ShowDialog();
                        break;
                    }
                default:
                    throw new Exception("Обработчик не реализован");
            }
        }

        #endregion

    }
}