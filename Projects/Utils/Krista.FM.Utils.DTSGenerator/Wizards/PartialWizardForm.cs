using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using Krista.FM.Client.Components;
using System.Data.Common;
using System.Data.OracleClient;
using System.Collections;

namespace Krista.FM.Utils.DTSGenerator.Wizards
{
    public partial class PartialWizardForm : Form
    {
        #region - Поля -
        /// <summary>
        /// Пакет для переноса данных
        /// </summary>
        private IPackage package;
        /// <summary>
        /// Родительская форма
        /// </summary>
        private SSISMainForm parentForm;
        /// <summary>
        /// Иконки для грида
        /// </summary>
        private ImageList imageList;
        /// <summary>
        /// Коллекция объектов для переноса
        /// </summary>
        private static Dictionary<string, SSISEntitiesObject> entitiesCollection = new Dictionary<string, SSISEntitiesObject>();
        /// <summary>
        /// Итог проверки
        /// </summary>
        private bool checkValidate = true;
        /// <summary>
        /// Условие для переноса источников
        /// </summary>
        private Dictionary<int, string> sourceIDDict = new Dictionary<int, string>();

        #endregion - Поля -

        #region - Конструктор -

        public PartialWizardForm(IPackage package, SSISMainForm parentForm)
        {
            this.package = package;
            this.parentForm = parentForm;

            this.imageList = new ImageList();

            InitializeImageList();
            InitializeComponent();
            InitializeEntitiesCollection();

            this.partialWizard.Back += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(partialWizard_Back);
            this.partialWizard.Next += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(partialWizard_Next);
            this.partialWizard.WizardClosed += new EventHandler(partialWizard_WizardClosed);
            this.partialWizard.Finish += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(partialWizard_Finish);

            this.wizardPageTables.Load += new EventHandler(wizardPageTables_Load);
            this.wizardPageValidate.Load += new EventHandler(wizardPageValidate_Load);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridExTables.ugData);
            this.ultraGridExTables.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ultraGridExTables_OnGridInitializeLayout);
            this.ultraGridExTables.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ultraGridExTables_OnInitializeRow);

            this.ultraGridExValidate.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(ultraGridExValidate_OnGridInitializeLayout);
            this.ultraGridExValidate.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(ultraGridExValidate_OnInitializeRow);
            this.ultraGridExValidate.OnGetHierarchyInfo += new Krista.FM.Client.Components.GetHierarchyInfo(ultraGridExValidate_OnGetHierarchyInfo);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridExValidate.ugData);

            this.wizardPageDataSource.Load += new EventHandler(wizardPageDataSource_Load);
            this.ultraGridExDataSorce.OnGridInitializeLayout += new GridInitializeLayout(ultraGridExDataSorce_OnGridInitializeLayout);
            this.ultraGridExDataSorce.OnInitializeRow += new InitializeRow(ultraGridExDataSorce_OnInitializeRow);
            this.ultraGridExDataSorce.OnCancelChanges += new DataWorking(ultraGridExDataSorce_OnCancelChanges);
            this.ultraGridExDataSorce.OnSaveChanges += new SaveChanges(ultraGridExDataSorce_OnSaveChanges);
            this.ultraGridExDataSorce.OnBeforeCellActivate += new BeforeCellActivate(ultraGridExDataSorce_OnBeforeCellActivate);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridExDataSorce.ugData);

            this.wizardFinalPage.Load += new EventHandler(wizardFinalPage_Load);
        }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public PartialWizardForm()
        {
            
        }
         
        
        #endregion - Конструктор -

        #region - Инициализация коллекции икнок

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

        #endregion - Инициализация коллекции икнок

        #region - Инициализация объектов для переноса -

        /// <summary>
        /// Инициализация объектов для переноса
        /// </summary>
        private void InitializeEntitiesCollection()
        {
            entitiesCollection.Clear();
            
            foreach (IEntity entity in package.Classes.Values)
            {
                HandleEntity(entity);
            }
        }

        private static void HandleEntity(IEntity entity)
        {
            if (entity.ClassType != ClassTypes.clsFixedClassifier)
            {
                foreach (IEntityAssociation entityAssociation in entity.Associations.Values)
                {
                    HandleEntity(entityAssociation.RoleBridge);
                }

                SSISEntitiesObject entityObject = new SSISEntitiesObject(new Guid(entity.ObjectKey), entity.FullCaption, entity);
                if (!entitiesCollection.ContainsKey(entity.ObjectKey) && (((IDataSourceDividedClass)entity).IsDivided || entity is IBridgeClassifier))
                    entitiesCollection.Add(entity.ObjectKey, entityObject);
                
            }
        }

        #endregion - Инициализация объектов для переноса -

        #region - Инициализация гридов -

        void ultraGridExTables_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            UltraGridColumn clmn = e.Layout.Bands[0].Columns["clmnTable"];
            clmn.Header.Caption = "Таблица";
            clmn.Width = 300;
            clmn.Header.Enabled = true;

            clmn = e.Layout.Bands[0].Columns.Insert(0, "clmnImage");
            clmn.Style = ColumnStyle.Image;
            clmn.Header.Caption = String.Empty;
            clmn.Width = 20;

            clmn = e.Layout.Bands[0].Columns["clmnPackage"];
            clmn.Header.Caption = "Пакет";
            clmn.Header.Enabled = true;
            clmn.Width = 350;

            clmn = e.Layout.Bands[0].Columns["clmnClassType"];
            clmn.Hidden = true;
        }

        void ultraGridExTables_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            switch (e.Row.Cells["clmnClassType"].Value.ToString())
            {
                case "clsBridgeClassifier":
                    e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[0];
                    break;
                case "clsDataClassifier":
                    e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[3];
                    break;
                case "clsFactData":
                    e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[2];
                    break;
                case "Table":
                    e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[1];
                    break; 
            }
        }

        void ultraGridExValidate_OnInitializeRow(object sender, InitializeRowEventArgs e)
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

        void ultraGridExValidate_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                band.ColHeadersVisible = false;
                band.GroupHeadersVisible = false;

                UltraGridColumn clmn = band.Columns["clmnID"];
                clmn.Hidden = true;

                clmn = band.Columns.Insert(0, "clmnImage");
                clmn.Style = ColumnStyle.Image;
                clmn.Header.Caption = String.Empty;
                clmn.Width = 20;

                clmn = band.Columns.Insert(1, "clmnImageType");
                clmn.Style = ColumnStyle.Image;
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

        /// <summary>
        /// Настройка иерархии
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        Krista.FM.Client.Components.HierarchyInfo ultraGridExValidate_OnGetHierarchyInfo(object sender)
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

        void ultraGridExDataSorce_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["clmnChecked"].Value == null)
                e.Row.Cells["clmnChecked"].Value = false;

            if (!String.IsNullOrEmpty(e.Row.Cells["state"].Value.ToString()))
            {
                e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[7];
                e.Row.Cells["clmnChecked"].Column.CellActivation = Activation.Disabled;
            }
            else
            {
                e.Row.Cells["clmnImage"].Appearance.ImageBackground = imageList.Images[8];
                e.Row.Cells["clmnChecked"].Column.CellActivation = Activation.AllowEdit;
            }
        }

        void ultraGridExDataSorce_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridColumn clmn = e.Layout.Bands[0].Columns["sourceID"];
            clmn.Header.Caption = "ID источника";
            clmn.Width = 100;
            clmn.Header.Enabled = true;
            clmn.CellActivation = Activation.ActivateOnly;

            clmn = e.Layout.Bands[0].Columns.Insert(0, "clmnImage");
            clmn.Style = ColumnStyle.Image;
            clmn.Header.Caption = String.Empty;
            clmn.Width = 20;

            clmn = e.Layout.Bands[0].Columns.Insert(1, "clmnChecked");
            clmn.Style = ColumnStyle.CheckBox;
            clmn.Header.Caption = String.Empty;
            clmn.Width = 20;

            clmn = e.Layout.Bands[0].Columns["datasourcename"];
            clmn.Header.Caption = "Имя источника";
            clmn.Header.Enabled = true;
            clmn.Width = 450;
            clmn.CellActivation = Activation.ActivateOnly;

            clmn = e.Layout.Bands[0].Columns["state"];
            clmn.Hidden = false;
        }

        bool ultraGridExDataSorce_OnSaveChanges(object sender)
        {
            return true;
        }

        void ultraGridExDataSorce_OnCancelChanges(object sender)
        {

        }

        void ultraGridExDataSorce_OnBeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            e.Cancel = false;
        }

        #endregion - Инициализация гридов -

        #region - Инициализация страниц мастера -

        /// <summary>
        /// Загрузка страницы с таблицами для переноса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardPageTables_Load(object sender, EventArgs e)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    DataColumn clmnTable = new DataColumn("clmnTable");
                    DataColumn clmnPackage = new DataColumn("clmnPackage");
                    DataColumn clmnClassType = new DataColumn("clmnClassType");
                    dt.Columns.AddRange(new DataColumn[] { clmnTable, clmnPackage, clmnClassType });
                    foreach (SSISEntitiesObject o in entitiesCollection.Values)
                        dt.Rows.Add(new object[] { o.Name, ((IEntity)o.ControlObject).ParentPackage.Name, ((IEntity)o.ControlObject).ClassType });
                    ultraGridExTables.DataSource = dt;
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Инициализация страницы валидации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wizardPageValidate_Load(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();

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

                SSISPackageObject packageObject = new SSISPackageObject(new Guid(package.ObjectKey), package.Name, package);
                packageObject.SsisEntities = entitiesCollection;

                checkValidate = parentForm.HandlePackageNode(packageObject, table);

                ds.Tables.Add(table);
                ds.Relations.Add(clmnID, clmnParendID);

                ultraGridExValidate.DataSource = ds;

                ultraGridExValidate.ugData.Rows[0].Expanded = true;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("При проверке переносимых объектов возникла ошибка: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Инициализация страницы с источниками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wizardPageDataSource_Load(object sender, EventArgs e)
        {
            try
            {
                string connectionStringSource = parentForm.SourceScheme.SchemeDWH.ConnectionString;
                string connectionStringDest = parentForm.DestinationScheme.SchemeDWH.ConnectionString;

                DataTable dsSource = new DataTable();
                DataTable dsDest = new DataTable();
                foreach (SSISEntitiesObject entitiesObject in entitiesCollection.Values)
                {
                   string queryString =
                   String.Format(
                       "select distinct t.sourceID, v.datasourcename from {0} t, datasources v where t.sourceID = v.ID",
                       ((IEntity)entitiesObject.ControlObject).FullDBName);
                    string queryStringDest =
                        String.Format(
                            "select distinct v.sourceID from {0} v",
                            ((IEntity)entitiesObject.ControlObject).FullDBName);

                    using (DataTable sourceTable = new DataTable())
                    {
                        using (DataTable destTable = new DataTable())
                        {
                            using (OracleDataAdapter daSource = new OracleDataAdapter(queryString, connectionStringSource))
                            {
                                using (OracleDataAdapter daDest = new OracleDataAdapter(queryStringDest, connectionStringDest))
                                {
                                    daSource.Fill(sourceTable);
                                    daDest.Fill(destTable);
                                }
                            }
                            dsSource.Merge(sourceTable);
                            dsDest.Merge(destTable);
                        }
                    }
                }

                dsSource = RemoveDuplicateRows(dsSource, "sourceID");
                dsDest = RemoveDuplicateRows(dsDest, "sourceID");

                using (DataColumn clmn = new DataColumn("state"))
                {
                    dsSource.Columns.Add(clmn);
                }
                
                foreach (DataRow row in dsSource.Rows)
                {
                    if (dsDest.Select(String.Format("sourceID = '{0}'", row["sourceID"])).Length > 0)
                        row["state"] = "Exist";
                }

                dsSource.AcceptChanges();

                ultraGridExDataSorce.DataSource = dsSource;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString());
            }
        }

        /// <summary>
        /// Удаляем дубликаты
        /// </summary>
        /// <param name="dTable"> Таблица, в которой удаляем дубликаты</param>
        /// <param name="colName"> Имя столбца, по которому определяем уникальность</param>
        /// <returns> Таблица без дубликатов</returns>
        public static DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            return dTable;
        }

        /// <summary>
        /// Получаем таблицу фактов
        /// </summary>
        /// <returns></returns>
        private IFactTable GetFactTable()
        {
            foreach (SSISEntitiesObject entitiesObject in entitiesCollection.Values)
            {
                if (entitiesObject.ControlObject is IFactTable)
                    return (IFactTable) entitiesObject.ControlObject;
            }

            return null;
        }

        void wizardFinalPage_Load(object sender, EventArgs e)
        {
            this.wizardFinalPage.Description2 += "\nПереносимые источники";

            foreach (string value in sourceIDDict.Values)
                wizardFinalPage.Description2 += String.Format("\n    {0}", value);

            this.wizardFinalPage.Description2 += "\nПереносимые объекты";

            foreach (SSISEntitiesObject o in entitiesCollection.Values)
                wizardFinalPage.Description2 += String.Format("\n    {0}", o.Name);
        }

        #endregion - Инициализация страниц мастера -

        #region - Навигация по мастеру - 

        void partialWizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage.Name == "WizardPageValidate" && !checkValidate)
            {
                if (MessageBox.Show("При валидации обнаружены ошибки", "Ошибки валидации!", MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                    e.Step = 0;
            }

            // добавляем условие на переносимые данные
            // сами новые источники тоже переносим 
            if (e.CurrentPage.Name == "WizardPageDataSource")
            {
                sourceIDDict.Clear();
                string expression = " where sourceID in (";

                foreach (UltraGridRow row in ultraGridExDataSorce.ugData.Rows)
                    if (row.Cells["clmnChecked"].Value.ToString() == "True")
                    {
                        expression += String.Format("{0}, ", row.Cells["sourceID"].Value);
                        sourceIDDict.Add(Convert.ToInt32(row.Cells["sourceID"].Value),
                                                         row.Cells["datasourcename"].Value.ToString());
                    }

                expression = expression.Substring(0, expression.Length - 2);
                expression += ")";

                foreach (SSISEntitiesObject o in entitiesCollection.Values)
                {
                    o.SqlExpession += expression;
                }
            }
        }

        void partialWizard_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
        }

        void partialWizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        void partialWizard_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
        }

        #endregion - Навигация по мастеру -

        #region - Точка входа -

        public static void ShowPartialWizardForm(IPackage package, SSISMainForm parentForm, ref SSISPackageObject packageObject)
        {
            using (PartialWizardForm form = new PartialWizardForm(package, parentForm))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SSISPackageObject packageObjectLoc = new SSISPackageObject(new Guid(package.ObjectKey), package.Name, package);
                    packageObjectLoc.SsisEntities = entitiesCollection;
                    packageObject = packageObjectLoc;
                }
            }
        }
        #endregion
    }
}