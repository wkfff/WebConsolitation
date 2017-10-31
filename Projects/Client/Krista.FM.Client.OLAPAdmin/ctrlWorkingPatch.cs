using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.OLAPStructures;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using CommandType=Krista.FM.Client.OLAPStructures.CommandType;
using Resources=Krista.FM.Client.OLAPAdmin.Properties.Resources;

namespace Krista.FM.Client.OLAPAdmin
{
    /// <summary>
    /// Тип обновления объекта
    /// </summary>
    public enum UpdateType
    {
        Create = 0,
        Alter = 1,
        Delete = 2
    }

    /// <summary>
    /// Контрол для работы с патчами
    /// </summary>
    public partial class ctrlWorkingPatch : UserControl
    {
        /// <summary>
        /// Событие возникающее при добавлении новых объектов для патча
        /// </summary>
        public event EventHandler AddObjectsEvent;

        Dictionary<string, ObjectForScript> objForScriptCollection = new Dictionary<string, ObjectForScript>();

        #region DataBase

        /// <summary>
        /// таблица
        /// </summary>
        private DataTable table;
        /// <summary>
        /// Поле - автоинкрементное поле (ключ)
        /// </summary>
        private DataColumn ID;
        /// <summary>
        /// Поле - имя объекта
        /// </summary>
        private DataColumn clmnname;
        /// <summary>
        /// Поле - тип объекта
        /// </summary>
        private DataColumn clmnobjType;
        /// <summary>
        /// Поле - метод обновления
        /// </summary>
        private DataColumn clmnmethod;

        #endregion DataBase

        /// <summary>
        /// Активный сервер
        /// </summary>
        private Microsoft.AnalysisServices.Server server = null;

        public ctrlWorkingPatch()
        {
            table = new DataTable();

            ID = new DataColumn();
            clmnname = new DataColumn();
            clmnobjType = new DataColumn();
            clmnmethod = new DataColumn();

            ID.ColumnName = "ID";
            ID.AutoIncrement = true;
            ID.DataType = typeof(Int32);
            ID.AutoIncrementStep = 1;
            ID.AutoIncrementSeed = 1;

            clmnname.ColumnName = "Name";
            clmnobjType.ColumnName = "ObjectType";
            clmnmethod.ColumnName = "Method";

            table.Columns.AddRange(new DataColumn[] { ID, clmnname, clmnobjType, clmnmethod });

            table.PrimaryKey = new DataColumn[] { ID, clmnname };

            InitializeComponent();

            ConfigUserToolBar();

            ultraGrid.OnGridInitializeLayout += new GridInitializeLayout(ultraGrid_OnGridInitializeLayout);
            ultraGrid.OnRefreshData += new RefreshData(ultraGrid_OnRefreshData);
            ultraGrid.OnCancelChanges += new DataWorking(ultraGrid_OnCancelChanges);
            ultraGrid.OnSaveChanges += new SaveChanges(ultraGrid_OnSaveChanges);
            ultraGrid.OnClearCurrentTable += new DataWorking(ultraGrid_OnClearCurrentTable);

            ultraGrid.utmMain.ToolClick += new ToolClickEventHandler(utmMain_ToolClick);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGrid._ugData);

            ultraGrid.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ultraGrid.ugData.DisplayLayout.AddNewBox.Hidden = true;
            ultraGrid.AllowAddNewRecords = false;

            ultraGrid.utmMain.Toolbars[1].Visible = false;
            ultraGrid.utmMain.Toolbars[2].Visible = false;


            ultraGrid.DataSource = table;
        }

        void utmMain_ToolClick(object sender, ToolClickEventArgs e)
        {
            ToolBase bt = e.Tool;
            if (bt != null)
            {
                switch(bt.Key)
                {
                    case "addButtonTool":
                        // вот оно и произошло :)
                        OnAddNewObject();
                        break;
                    case "createpatchButton":
                        CreatePatch();
                        break;
                    case "applayPatchButton":
                        if (server == null)
                            throw new Exception("Не активно подключение к серверу");

                        ApplayPatchForm formApplay = new ApplayPatchForm(server);
                        formApplay.ShowDialog();
                        break;
                }
            }
        }

        private void CreatePatch()
        {
            WizardCreatePatch wizard = new WizardCreatePatch(server, objForScriptCollection);
            wizard.ShowDialog();
        }

        /// <summary>
        /// Пользовательский тулбар
        /// </summary>
        private void ConfigUserToolBar()
        {
            UltraToolbar tb = ultraGrid.utmMain.Toolbars["userToolBar"];
            tb.Visible = true;

            // Добавление объектов для патча
            ButtonTool addButtonTool = new ButtonTool("addButtonTool");
            addButtonTool.SharedProps.ToolTipText = @"Добавить выделенные объекты";
            addButtonTool.SharedProps.AppearancesSmall.Appearance.Image = Resources.add;

            // Создание патча
            ButtonTool createpatchButton = new ButtonTool("createpatchButton");
            createpatchButton.SharedProps.ToolTipText = @"Создать патч";
            createpatchButton.SharedProps.AppearancesSmall.Appearance.Image = Resources.scriptXMLA;

            // Применить патч
            ButtonTool applayPatchButton = new ButtonTool("applayPatchButton");
            applayPatchButton.SharedProps.ToolTipText = @"Применить патч";
            applayPatchButton.SharedProps.AppearancesSmall.Appearance.Image = Resources.run;

            ultraGrid.utmMain.Tools.AddRange(new ToolBase[] { addButtonTool, createpatchButton, applayPatchButton });
            ultraGrid.utmMain.Toolbars["userToolBar"].Tools.AddRange(new ToolBase[] { addButtonTool, createpatchButton, applayPatchButton });
        }

        /// <summary>
        /// Активный сервер
        /// </summary>
        public Microsoft.AnalysisServices.Server Server
        {
            get { return server; }
            set { server = value; }
        }

        void ultraGrid_OnClearCurrentTable(object sender)
        {
            foreach (UltraGridRow row in ultraGrid.ugData.Rows)
            {
                row.Delete();
            }

            SaveChanges();

            RefreshScriptObject();
        }

        bool ultraGrid_OnSaveChanges(object sender)
        {
            SaveChanges();
            RefreshScriptObject();

            return true;
        }

        void ultraGrid_OnCancelChanges(object sender)
        {
            RefreshScriptObject();
        }

        bool ultraGrid_OnRefreshData(object sender)
        {
            RefreshScriptObject();
            return true;
        }

        static void ultraGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand _band = e.Layout.Bands[0];

            UltraGridColumn clmn = _band.Columns["ID"];
            clmn.Hidden = true;

            clmn = _band.Columns["Name"];
            clmn.Header.Caption = @"Имя объекта";
            clmn.Width = 350;

            clmn = _band.Columns["ObjectType"];
            clmn.Header.Caption = @"Тип объекта";
            clmn.Width = 200;

            clmn = _band.Columns["Method"];
            clmn.Header.Caption = @"Метод обновления";
            clmn.Style = ColumnStyle.DropDownList;
            clmn.Width = 200;
        }

        /// <summary>
        /// Сохраняет сделанные изменения
        /// </summary>
        internal void SaveChanges()
        {
            DataTable dt = table.GetChanges();
            if (dt == null)
                return;

            foreach (DataRow row in dt.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        break;
                    case DataRowState.Deleted:
                        if (objForScriptCollection.ContainsKey(Convert.ToString(row[1, DataRowVersion.Original])))
                        {
                            objForScriptCollection.Remove(Convert.ToString(row[1, DataRowVersion.Original]));
                        }
                        break;
                    case DataRowState.Modified:
                        if (objForScriptCollection.ContainsKey(Convert.ToString(row[1, DataRowVersion.Original])))
                        {
                            objForScriptCollection[Convert.ToString(row[1, DataRowVersion.Original])].CommandType = GetCommandType(Convert.ToString(row[3]));
                        }
                        break;
                }
            }
            dt.AcceptChanges();
        }

        public void RefreshScriptObject()
        {
            table.Rows.Clear();

            ultraGrid.ugData.DisplayLayout.ValueLists.Clear();

            ValueList takeMethodList = ultraGrid.ugData.DisplayLayout.ValueLists.Add("MethodUpdate");
            foreach (UpdateType method in Enum.GetValues(typeof(UpdateType)))
                takeMethodList.ValueListItems.Add(method.ToString());
            UltraGridColumn methodClmn = ultraGrid.ugData.DisplayLayout.Bands[0].Columns["Method"];
            methodClmn.ValueList = takeMethodList;

            foreach (ObjectForScript obj in objForScriptCollection.Values)
            {
                DataRow row = table.NewRow();
                row[1] = obj.Obj.Name;
                row[2] = obj.ObjectType;
                row[3] = obj.CommandType.ToString();

                table.Rows.Add(row);
            }

            table.AcceptChanges();
        }


        public UltraGridEx UltraGrid
        {
            get { return ultraGrid; }
        }


        public DataTable Table
        {
            get { return table; }
        }

        public bool IsChange()
        {
            return table.GetChanges() != null;
        }

        private static CommandType GetCommandType(string type)
        {
            switch (type)
            {
                case "Create":
                    return CommandType.create;
                case "Alter":
                    return CommandType.alter;
                case "Delete":
                    return CommandType.delete;
                default:
                    throw new Exception("");
            }
        }
        
        /// <summary>
        /// Обработчик события
        /// </summary>
        void OnAddNewObject()
        {
            if (AddObjectsEvent != null)
                AddObjectsEvent(this, new EventArgs());
        }

       
        public Dictionary<string, ObjectForScript> ObjForScriptCollection
        {
            get { return objForScriptCollection; }
            set { objForScriptCollection = value; }
        }
    }
}
