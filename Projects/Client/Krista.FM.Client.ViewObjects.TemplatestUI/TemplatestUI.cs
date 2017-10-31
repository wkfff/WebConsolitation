using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Common;
using Krista.FM.Common.FileUtils;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;

using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
    public class TemplatesViewObj : BaseViewObj
    {
        // список состояний документа
        private enum TemplateDocumentState : int
        {
            NoEdit = 0, 
			New = 1, 
			InEdit = 2
        }

        private const string imageColumn = "templateTypeImg";
        private const string blockColumn = "templateBlockImg";

        private TemplatesView vo;

        private ITemplatesRepository repository;

		// таблица, содержащая всю информацию о шаблоне кроме самого документа
        private DataTable dtTemplatesData;
        private DataSet hierarchyDs;

        private TemplateStruct activeTemplate = new TemplateStruct();

        private List<int> childDeleteRows = new List<int>();

        private int currentUser;

        internal Dictionary<int, ITemplate> editedTemplates = new Dictionary<int, ITemplate>();

        IWin32Window parentWindow;

        private const string reportTemplates = "Шаблоны отчетов";
        private const string finSources = "Источники финансирования";

        protected override void SetViewCtrl()
        {
            fViewCtrl = new TemplatesView();
            vo = (TemplatesView)fViewCtrl;
        }

        public TemplatesViewObj(string key)
            : base(key)
        {
            Caption = "Репозиторий отчетов";
        }

        public override void Initialize()
        {
            base.Initialize();

            repository = this.Workplace.ActiveScheme.TemplatesService.Repository;

            vo.Text = Caption;

            vo.tbDocumentName.TextChanged += new EventHandler(tbDocumentName_TextChanged);
            vo.utbmDocumentActions.ToolClick += new ToolClickEventHandler(utbmDocumentActions_ToolClick);
            vo.tbDescription.TextChanged += new EventHandler(tbDescription_TextChanged);

            #region настройка основного грида
            vo.ugeTemplates.OnGetGridColumnsState += new GetGridColumnsState(ugeTemplates_OnGetGridColumnsState);
            vo.ugeTemplates.ToolClick += new ToolBarToolsClick(ugeTemplates_ToolClick);
            vo.ugeTemplates.OnCancelChanges += new DataWorking(ugeTemplates_OnCancelChanges);
            vo.ugeTemplates.OnRefreshData += new RefreshData(ugeTemplates_OnRefreshData);
            vo.ugeTemplates.OnClearCurrentTable += new DataWorking(ugeTemplates_OnClearCurrentTable);
            vo.ugeTemplates.OnSaveChanges += new SaveChanges(ugeTemplates_OnSaveChanges);
            vo.ugeTemplates.OnAfterRowActivate += new AfterRowActivate(ugeTemplates_OnAfterRowActivate);
            vo.ugeTemplates.OnAfterRowInsert += new AfterRowInsert(ugeTemplates_OnAfterRowInsert);
            vo.ugeTemplates.OnGridInitializeLayout += new GridInitializeLayout(ugeTemplates_OnGridInitializeLayout);
            vo.ugeTemplates.OnInitializeRow += new InitializeRow(ugeTemplates_OnInitializeRow);
            vo.ugeTemplates.OnBeforeRowsDelete += new BeforeRowsDelete(ugeTemplates_OnBeforeRowsDelete);
            vo.ugeTemplates._ugData.CellListSelect += new CellEventHandler(_ugData_CellListSelect);
            vo.ugeTemplates.OnCellChange += new CellChange(ugeTemplates_OnCellChange);

            vo.ugeTemplates.OnSaveToXML += new SaveLoadXML(ugeTemplates_OnSaveToXML);
            vo.ugeTemplates.OnLoadFromXML += new SaveLoadXML(ugeTemplates_OnLoadFromXML);

            vo.ugeTemplates.GridDragDrop += new MainMouseEvents(ugeTemplates_GridDragDrop);
            vo.ugeTemplates.GridDragEnter += new MainMouseEvents(ugeTemplates_GridDragEnter);
            vo.ugeTemplates.GridDragOver += new MainMouseEvents(ugeTemplates_GridDragOver);

            vo.ugeTemplates.StateRowEnable = true;
            vo.ugeTemplates._ugData.SyncWithCurrencyManager = true;
            vo.ugeTemplates.SaveLoadFileName = "Репозиторий отчетов";
            #endregion
            
			// добавляем кнопочки на основной тулбар
            UpdateGridToolBar();

            currentUser = this.Workplace.CurrentUserID;
            // получаем даннные с сервера
            RefreshData();

            parentWindow = (IWin32Window)(Control)this.Workplace;
        }

        void ugeTemplates_OnCellChange(object sender, CellEventArgs e)
        {
            BlockRow(UltraGridHelper.GetActiveID(vo.ugeTemplates.ugData));
        }

        private void BlockRow(int id)
        {
            DataRow row = GetDocumentRow(id);
            SetTemplateState(id);
        }

        bool ugeTemplates_OnLoadFromXML(object sender)
        {
            string tmpFileName = this.vo.ugeTemplates.SaveLoadFileName;
            bool addToTopLevel = !(this.vo.ugeTemplates.ugData.Rows.Count > 0 && this.vo.ugeTemplates.ugData.ActiveRow != null);
            if (ImportParams.ShowForm(ref addToTopLevel))
            {
                if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, false, ref tmpFileName))
                {
                    FileStream stream = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read);
                    if (addToTopLevel)
                        repository.RepositoryImport(stream);
                    else
                        repository.RepositoryImport(stream, UltraGridHelper.GetActiveID(this.vo.ugeTemplates.ugData));
                    RefreshData();
                }
            }
            return false;
        }

        bool ugeTemplates_OnSaveToXML(object sender)
        {
            bool exportAllRows = true;
            bool exportHiaerarchy = true;
            if (FormImportParameters.ShowImportParams(parentWindow, ref exportAllRows, ref exportHiaerarchy))
            {

                string tmpFileName = this.vo.ugeTemplates.SaveLoadFileName;
                if (ExportImportHelper.GetFileName(tmpFileName, ExportImportHelper.fileExtensions.xml, true, ref tmpFileName))
                {
                    this.Workplace.OperationObj.Text = "Сохранение данных";
                    this.Workplace.OperationObj.StartOperation();
                    FileStream stream = null;
                    try
                    {
                        stream = new FileStream(tmpFileName, FileMode.Create);
                        if (!exportAllRows)
                        {
                            HierarchyInfo hi = new HierarchyInfo();
                            hi.loadMode = LoadMode.AllRows;
                            List<int> selectedIds = new List<int>();
                            UltraGridHelper.GetSelectedIds(this.vo.ugeTemplates, hi, out selectedIds, exportHiaerarchy);
                            repository.RepositoryExport(stream, selectedIds);
                        }
                        else
                            repository.RepositoryExport(stream);
                        return true;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                        this.Workplace.OperationObj.StopOperation();
                    }
                }
            }
            return false;
        }

        void ugeTemplates_GridDragOver(object sender, DragEventArgs e)
        {
            UltraGridRow tmpRow = UltraGridHelper.GetRowFromPos(e.X, e.Y, vo.ugeTemplates.ugData);
            if (e.Data.GetDataPresent("FileDrop"))
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                List<string> dropFiles = new List<string>();
                bool isFolder = false;
                foreach (string file in files)
                {
                    FileAttributes attr = File.GetAttributes(file);
                    // если это директория 
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        isFolder = true;
                    }
                }

                if (tmpRow == null)
                    e.Effect = DragDropEffects.Copy;
                else
                {
                    tmpRow.Activate();
                    TemplateTypes tmpRowType = (TemplateTypes)Convert.ToInt32(tmpRow.Cells["Type"].Value);
                    
                    if (tmpRowType == TemplateTypes.Group)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        if (!isFolder)
                            e.Effect = DragDropEffects.Move;
                        else
                            e.Effect = DragDropEffects.None;
                    }
                }
            }
        }

        private bool IsSameTemplateType(string fileName, TemplateTypes templateType)
        {
            string fileExt = Path.GetExtension(fileName).ToLower();
            if (fileExt == ".xls" && (templateType == TemplateTypes.MSExcel || templateType == TemplateTypes.MSExcelPlaning))
                return true;
            if (fileExt == ".doc" && (templateType == TemplateTypes.MSWord || templateType == TemplateTypes.MSWordPlaning))
                return true;
            if (fileExt == ".dot" && templateType == TemplateTypes.MSWordTemplate)
                return true;
            if (fileExt == ".xlt" && templateType == TemplateTypes.MSExcelTemplate)
                return true;
            if (templateType == TemplateTypes.Arbitrary)
                return true;
            return false;
        }

        void ugeTemplates_GridDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        void ugeTemplates_GridDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                string folderName = string.Empty;
                List<string> dropFiles = new List<string>();
                foreach (string file in files)
                {
                    FileAttributes attr = File.GetAttributes(file);
                    // если это директория 
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        folderName = files[0];
                        dropFiles.AddRange(Directory.GetFiles(file));
                    }
                    else
                        dropFiles.Add(file);
                }
                // добавляем файлы в задачу
                if (e.Effect != DragDropEffects.Copy && e.Effect != DragDropEffects.Move)
                    return;
                UltraGridRow tmpRow = UltraGridHelper.GetRowFromPos(e.X, e.Y, vo.ugeTemplates.ugData);
                if (tmpRow == null)
                {
                    // кидаем документ или каталог в пустое место на гриде
                    if (!string.IsNullOrEmpty(folderName))
                    {
                        AddFolderRow(folderName, tmpRow);
                        tmpRow = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
                    }
                    foreach (string fileName in dropFiles)
                    {
                        AddDocumentRow(fileName, tmpRow);
                    }
                }
                else
                {
                    // кидаем в некую запись в гриде
                    if (activeTemplate.TemplateType == TemplateTypes.Group)
                    {
                        if (!string.IsNullOrEmpty(folderName))
                        {
                            AddFolderRow(folderName, tmpRow);
                            tmpRow = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
                        }
                        foreach (string fileName in dropFiles)
                        {
                            AddDocumentRow(fileName, tmpRow);
                        }
                    }
                    else
                    {
                        if (e.Effect == DragDropEffects.Copy)
                        {
                            foreach (string file in dropFiles)
                                AddDocumentRow(file, false);
                        }
                        else if (e.Effect == DragDropEffects.Move)
                        {
                            if (dropFiles.Count == 0)
                                return;
                            bool addDocument = IsSameTemplateType(dropFiles[0], activeTemplate.TemplateType);
                            if (ImportParams.ShowForm(ref addDocument))
                            {
                                if (addDocument)
                                {
                                    if (ReplaceDocument())
                                        AddDocumentToRow(dropFiles[0]);
                                }
                                else
                                {
                                    AddDocumentRow(dropFiles[0], true);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region добавление документов в записи и записей с документами

        /// <summary>
        /// добавляет документ в уже существующую запись
        /// </summary>
        /// <param name="fileName"></param>
        private void AddDocumentToRow(string fileName)
        {
            if (ReplaceDocument())
            {
                activeTemplate.DocumentFileName = fileName;
                activeTemplate.DocumentName = Path.GetFileName(activeTemplate.DocumentFileName);
                DataRow row = GetDocumentRow(activeTemplate.ID);

                FileInfo fi = new FileInfo(activeTemplate.DocumentFileName);
                activeTemplate.Document = FileHelper.ReadFileData(activeTemplate.DocumentFileName);

                SetRowParams();

                if (string.IsNullOrEmpty(activeTemplate.Name))
                {
                    vo.tbDocumentName.Text = activeTemplate.Name = Path.GetFileNameWithoutExtension(activeTemplate.DocumentFileName);
                }
                row["PresentDocument"] = activeTemplate.PresentDocument = true;
                ShowTemplateDetail(activeTemplate.TemplateType);
            }
        }

        /// <summary>
        /// добавляет новую запись с документом
        /// </summary>
        /// <param name="fileName"></param>
        private void AddDocumentRow(string fileName, bool addChild)
        {
            AddDocumentRow(fileName, UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData));
        }

        private void AddDocumentRow(string fileName, UltraGridRow parentRow)
        {
            if (parentRow != null)
                AddNewTemplate(parentRow, false);
            else
                AddNewTemplate(null, true);
            if (!activeTemplate.PresentDocument)
            {
                activeTemplate.DocumentFileName = fileName;
                activeTemplate.DocumentName = Path.GetFileName(activeTemplate.DocumentFileName);
                DataRow row = GetDocumentRow(activeTemplate.ID);
                row["Type"] = activeTemplate.TemplateType = GetTemplateType(Path.GetExtension(fileName));
                FileInfo fi = new FileInfo(activeTemplate.DocumentFileName);
                activeTemplate.Document = FileHelper.ReadFileData(activeTemplate.DocumentFileName);

                SetRowParams();

                vo.tbDocumentName.Text = activeTemplate.Name = Path.GetFileNameWithoutExtension(activeTemplate.DocumentFileName);

                row["PresentDocument"] = activeTemplate.PresentDocument = true;
                ShowTemplateDetail(activeTemplate.TemplateType);
            }
        }

        private void AddFolderRow(string folderName, bool addChild)
        {
            AddFolderRow(folderName, UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData));
        }

        private void AddFolderRow(string folderName, UltraGridRow parentRow)
        {
            if (parentRow != null)
                AddNewTemplate(parentRow, false);
            else
                AddNewTemplate(null, true);
            string[] temp = folderName.Split('\\');
            DataRow row = GetDocumentRow(activeTemplate.ID);
            row["Name"] = activeTemplate.Name = temp[temp.Length - 1];
            row["PresentDocument"] = activeTemplate.PresentDocument = false;
            SetRowParams();
            ShowTemplateDetail(activeTemplate.TemplateType);
        }

        #endregion

        #region взятие шаблонов на редактирование

        private void SetTemplateState(int id)
        {
            // для удаленных и добавленных записей ничего не делаем
            DataRow row = GetDocumentRow(id);
            if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Added)
                return;

            //ButtonTool btnSetTemplateState = (ButtonTool)vo.ugeTemplates.utmMain.Tools["btnSetTemplateState"];
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
            ITemplate template = null;
            if (editedTemplates.ContainsKey(id))
                template = editedTemplates[id];
            else
            {
                template = repository[id];
                editedTemplates.Add(id, template);
            }
            TemplateState templateState = template.GetTemplateState(currentUser);
            switch (templateState)
            {
                case TemplateState.EasyAccessState:
                    template.SetTemplateState(currentUser, TemplateState.EditingState);
                    //activeRow.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[9];
                    row["Editor"] = currentUser;
                    row["TemplateState"] = (int)TemplateState.EditingByCurrent;
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[8];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Завершить редактирование отчета";
                    break;
                case TemplateState.EditingByCurrent:
                    template.SetTemplateState(currentUser, TemplateState.EditingByCurrent);
                    //activeRow.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[8];
                    row["Editor"] = currentUser;
                    row["TemplateState"] = (int)TemplateState.EditingByCurrent;
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[7];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Взять отчет на редактирование";
                    break;
                case TemplateState.EditingState:
                    return;
            }
        }

        private void SetTemplateState(ITemplate template)
        {
            // для удаленных и добавленных записей ничего не делаем
            TemplateState templateState = template.GetTemplateState(currentUser);
            DataRow row = GetDocumentRow(template.ID);
            switch (templateState)
            {
                case TemplateState.EasyAccessState:
                    template.SetTemplateState(currentUser, TemplateState.EditingState);
                    //activeRow.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[9];
                    row["Editor"] = currentUser;
                    row["TemplateState"] = (int)TemplateState.EditingByCurrent;
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[8];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Завершить редактирование отчета";
                    break;
                case TemplateState.EditingByCurrent:
                    template.SetTemplateState(currentUser, TemplateState.EasyAccessState);
                    //activeRow.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[8];
                    row["Editor"] = -1;
                    row["TemplateState"] = (int)TemplateState.EasyAccessState;
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[7];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Взять отчет на редактирование";
                    break;
                case TemplateState.EditingState:
                    return;
            }
        }

        #endregion

        private TemplateTypes GetTemplateType(string fileExt)
        {
            string str = fileExt.ToLower();
            switch (str)
            {
                case ".xls":
                    return TemplateTypes.MSExcel;
                case ".doc":
                    return TemplateTypes.MSWord;
                case ".dot":
                    return TemplateTypes.MSWordTemplate;
                case ".xlt":
                    return TemplateTypes.MSExcelTemplate;
                default:
                    return TemplateTypes.Arbitrary;
            }
        }

        void ugeTemplates_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (UltraGridRow delRow in e.Rows)
            {
                DeleteChildRows(delRow);
            }
        }

        private void DeleteChildRows(UltraGridRow delRow)
        {
            foreach (UltraGridChildBand band in delRow.ChildBands)
            {
                foreach (UltraGridRow row in band.Rows)
                {
                    DeleteChildRows(row);
                    int id = Convert.ToInt32(row.Cells["ID"].Value);
                    if (!childDeleteRows.Contains(id))
                        childDeleteRows.Add(id);
                    row.Delete();
                }
            }
        }

        #region события грида

        #region основные действия с данными на тулбаре

        bool ugeTemplates_OnSaveChanges(object sender)
        {
            return SaveData();
        }

        void ugeTemplates_OnClearCurrentTable(object sender)
        {
            repository.Clear(currentUser);
            RefreshData();
            editedTemplates.Clear();
        }

        bool ugeTemplates_OnRefreshData(object sender)
        {
            RefreshData();
            return true;
        }

        internal void ugeTemplates_OnCancelChanges(object sender)
        {
            this.Workplace.OperationObj.Text = "Отмена изменений";
            try
            {
                this.Workplace.OperationObj.StartOperation();
                this.RefreshData();
                foreach (ITemplate template in editedTemplates.Values)
                {
                    template.SetTemplateState(currentUser, TemplateState.EasyAccessState); 
                }
                editedTemplates.Clear();
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
        }

        #endregion

        void utbmDocumentActions_ToolClick(object sender, ToolClickEventArgs e)
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "btnCreateNew":
                    if (ReplaceDocument())
                        AddNewDocument();
                    break;
                case "btnAddExist":
                    if (ReplaceDocument())
                        AddDocument();
                    break;
                case "btnOpenDocument":
                    OpenDocument(activeTemplate.ID);
                    break;
                case "btnSaveDocument":
                    SaveDocument(activeTemplate.ID);
                    break;
            }
            row.Activate();
        }

        private bool ReplaceDocument()
        {
            if (!activeTemplate.PresentDocument)
                return true;
            if (MessageBox.Show("Документ уже был добавлен. Заменить?", "Документ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                return true;
            return false;
        }

        void tbDocumentName_TextChanged(object sender, EventArgs e)
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates._ugData);
            string name = row.Cells["Name"].Value.ToString();
            if (name != vo.tbDocumentName.Text)
            {
                row.Cells["Name"].Value = vo.tbDocumentName.Text;
                activeTemplate.DocumentName = vo.tbDocumentName.Text;
                BlockRow(UltraGridHelper.GetActiveID(vo.ugeTemplates.ugData));
            }
        }

        void tbDescription_TextChanged(object sender, EventArgs e)
        {
            UltraGridRow row = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates._ugData);
            string description = row.Cells["Description"].Value.ToString();
            if (description != vo.tbDescription.Text)
            {
                row.Cells["Description"].Value = vo.tbDescription.Text;
                activeTemplate.Description = vo.tbDescription.Text;
                BlockRow(UltraGridHelper.GetActiveID(vo.ugeTemplates.ugData));
            }
        }

        void ugeTemplates_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            
            ValueList list = null;
            if (!vo.ugeTemplates.ugData.DisplayLayout.ValueLists.Exists("TemplatesTypes"))
            {
                list = vo.ugeTemplates.ugData.DisplayLayout.ValueLists.Add("TemplatesTypes");
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "Группа отчетов (0)";
                item.DataValue = 0;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "Документ MS Word (1)";
                item.DataValue = 1;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "Документ MS Excel (2)";
                item.DataValue = 2;

                item = list.ValueListItems.Add("item3");
                item.DisplayText = "Документ MDX Эксперт (3)";
                item.DataValue = 3;

                item = list.ValueListItems.Add("item4");
                item.DisplayText = "Документ MDX Эксперт 3 (4)";
                item.DataValue = 4;

                item = list.ValueListItems.Add("item5");
                item.DisplayText = "Документ надстройки MS Word (5)";
                item.DataValue = 5;

                item = list.ValueListItems.Add("item6");
                item.DisplayText = "Документ надстройки MS Excel (6)";
                item.DataValue = 6;

                item = list.ValueListItems.Add("item7");
                item.DisplayText = "Произвольный документ (7)";
                item.DataValue = 7;

                item = list.ValueListItems.Add("item8");
                item.DisplayText = "Шаблон документа MS Word (8)";
                item.DataValue = 8;

                item = list.ValueListItems.Add("item9");
                item.DisplayText = "Шаблон документа MS Excel (9)";
                item.DataValue = 9;

				item = list.ValueListItems.Add("item10");
				item.DisplayText = "Веб-отчет (10)";
				item.DataValue = 10;
			}
            else
                list = vo.ugeTemplates.ugData.DisplayLayout.ValueLists["TemplatesTypes"];

            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                band.Columns["Type"].ValueList = list;
                band.Columns["Type"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

                UltraGridColumn uImageColumn = band.Columns.Add(imageColumn);
                UltraGridHelper.SetLikelyImageColumnsStyle(uImageColumn, -1);
                uImageColumn.Header.VisiblePosition = 1;
            } 
        }

        void ugeTemplates_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            UltraGridCell imageCell = row.Cells[imageColumn];
            if (row.Cells["Type"].Value == null || row.Cells["Type"].Value == DBNull.Value || string.IsNullOrEmpty(row.Cells["Type"].Value.ToString()))
                return;

            TemplateTypes templateType = (TemplateTypes)Convert.ToInt32(row.Cells["Type"].Value);
            SetTemplateImage(templateType, imageCell);
            if (row.Cells["TemplateState"].Value == DBNull.Value)
                return;

            TemplateState state = (TemplateState)Convert.ToInt32(row.Cells["TemplateState"].Value);
            switch (state)
            {
                case TemplateState.EasyAccessState:
                    //row.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[8];
                    break;
                case TemplateState.EditingByCurrent:
                    //row.Cells[blockColumn].Appearance.Image = vo.ilTools.Images[9];
                    break;
                case TemplateState.EditingState:
                    row.Cells[UltraGridEx.StateColumnName].Appearance.Image = vo.ilTools.Images[7];
                    row.Activation = Activation.NoEdit;
                    break;
            }
        }

        private void SetTemplateImage(TemplateTypes templateType, UltraGridCell imageCell)
        {
            switch (templateType)
            {
                case TemplateTypes.Arbitrary:
                    imageCell.Appearance.Image = vo.ilTemplatesType.Images[3];
                    return;
                case TemplateTypes.Group:
                    imageCell.Appearance.Image = vo.ilTemplatesType.Images[0];
                    return;
                case TemplateTypes.MDXExpert:
                case TemplateTypes.MDXExpert2:
                    imageCell.Appearance.Image = null;
                    return;
                case TemplateTypes.MSExcel:
                case TemplateTypes.MSExcelPlaning:
                    imageCell.Appearance.Image = vo.ilTemplatesType.Images[2];
                    return;
                case TemplateTypes.MSWord:
                case TemplateTypes.MSWordPlaning:
                    imageCell.Appearance.Image = vo.ilTemplatesType.Images[1];
                    return;
            }
        }

        void ugeTemplates_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            UltraGridRow insertedRow = row;
            UltraGrid ug = (UltraGrid)sender;
            if (row.Cells["ID"].Value.ToString() != string.Empty)
            {
                if (ug.ActiveRow.Cells["ID"].Value.ToString() == string.Empty)
                    insertedRow = ug.ActiveRow;
                else
                    return;
            }

            ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            try
            {
                activeTemplate.ID = repository.NewTemplateID();

                row.Cells["Name"].Value = string.Empty;
                row.Cells["Type"].Value = (int)TemplateTypes.Group;
                row.Cells["TemplateDocumentState"].Value = (int)TemplateDocumentState.New;
                row.Cells["PresentDocument"].Value = false;
                row.Cells["ID"].Value = activeTemplate.ID;
            }
            finally
            {
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
            }
        }

        void ugeTemplates_OnAfterRowActivate(object sender, EventArgs e)
        {
            if (vo.ugeTemplates.ugData.ActiveRow == null)
                return;
            if (vo.ugeTemplates.ugData.ActiveRow.IsAddRow)
            {
                vo.tbDocumentName.Text = string.Empty;
                vo.tbDescription.Text = string.Empty;
                return;
            }

            if (vo.ugeTemplates.ugData.ActiveRow.Cells["ID"].Value is DBNull)
            {
                vo.tbDocumentName.Text = string.Empty;
                vo.tbDescription.Text = string.Empty;
                return;
            }

            int activeRowID = UltraGridHelper.GetActiveID(vo.ugeTemplates.ugData);
            UltraGridRow activeRow =  UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
            DataRow row = GetDocumentRow(activeRowID);
            activeTemplate.ID = activeRowID;
            activeTemplate.Name = row["Name"].ToString();
            activeTemplate.Description = row["Description"].ToString();
            activeTemplate.TemplateType = (TemplateTypes)Convert.ToInt32(row["Type"]);
            activeTemplate.PresentDocument = Convert.ToBoolean(row["PresentDocument"]);

            if (row["TemplateState"] == DBNull.Value)
                return;

            TemplateState state = (TemplateState)Convert.ToInt32(row["TemplateState"]);
            //ButtonTool btnSetTemplateState = (ButtonTool)vo.ugeTemplates.utmMain.Tools["btnSetTemplateState"];
            switch (state)
            {
                case TemplateState.EasyAccessState:
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[7];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Взять отчет на редактирование";
                    SetLockRow(false);
                    break;
                case TemplateState.EditingByCurrent:
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance. Image = vo.ilTools.Images[8];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Завершить редактирование отчета";
                    SetLockRow(false);
                    break;
                case TemplateState.EditingState:
                    //btnSetTemplateState.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[7];
                    //btnSetTemplateState.SharedProps.ToolTipText = "Отчет редактируется другим пользователем";
                    SetLockRow(true);
                    break;
            }
            ShowTemplateDetail(activeTemplate.TemplateType);

            int docState = Convert.ToInt32(row["TemplateDocumentState"]);
            vo.tbDocumentName.Text = activeTemplate.DocumentName = row["Name"].ToString();
            vo.tbDescription.Text = activeTemplate.Description = row["Description"].ToString();

            if (activeTemplate.DocumentName == finSources || activeTemplate.DocumentName == reportTemplates)
            {
                vo.ugeTemplates.AllowDeleteRows = false;
            }
            else
            {
                vo.ugeTemplates.AllowDeleteRows = true;
            }

            SetdetailVisible();
        }

        private void SetLockRow(bool locked)
        {
            if (locked)
            {
                //button.SharedProps.AppearancesSmall.Appearance.Image = vo.ilImages.Images[12];
                //button.SharedProps.ToolTipText = "Открыть вариант для изменений";
                vo.ugeTemplates.AllowDeleteRows = false;
                vo.ugeTemplates.AllowEditRows = false;
                vo.tbDocumentName.ReadOnly = true;
                vo.tbDescription.ReadOnly = true;
            }
            else
            {
                //button.SharedProps.AppearancesSmall.Appearance.Image = vo.ilImages.Images[13];
                //button.SharedProps.ToolTipText = "Закрыть вариант от изменений";
                vo.ugeTemplates.AllowDeleteRows = true;
                vo.ugeTemplates.AllowEditRows = true;
                vo.tbDocumentName.ReadOnly = false;
                vo.tbDescription.ReadOnly = false;
            }
        }

        private void ShowTemplateDetail(TemplateTypes templateType)
        {
            vo.utcDataCls.Tabs[0].Visible = true;
            vo.utcDataCls.Tabs[0].Selected = true;
            if (templateType != TemplateTypes.Group)
            {
                vo.utcDataCls.Tabs[1].Visible = false;
                vo.utcDataCls.Tabs[2].Visible = false;
                vo.utbmDocumentActions.Visible = true;
            }
            else
            {
                vo.utcDataCls.Tabs[1].Visible = true;
                vo.utcDataCls.Tabs[2].Visible = true;
                vo.utbmDocumentActions.Visible = false;
            }

            vo.utbmDocumentActions.Tools["btnOpenDocument"].SharedProps.Enabled = activeTemplate.PresentDocument;
            vo.utbmDocumentActions.Tools["btnSaveDocument"].SharedProps.Enabled = activeTemplate.PresentDocument;
            vo.utbmDocumentActions.Tools["btnCreateNew"].SharedProps.Enabled = 
                activeTemplate.TemplateType != TemplateTypes.Arbitrary &&
                activeTemplate.TemplateType != TemplateTypes.MSWordTemplate &&
                activeTemplate.TemplateType != TemplateTypes.MSExcelTemplate;

            vo.utcDataCls.ActiveTab = vo.utcDataCls.Tabs[0];
        }


        void _ugData_CellListSelect(object sender, CellEventArgs e)
        {
            TemplateTypes templateType = (TemplateTypes)e.Cell.ValueListResolved.SelectedItemIndex;
            activeTemplate.TemplateType = templateType;
            ShowTemplateDetail(templateType);
            SetTemplateImage(templateType, e.Cell.Row.Cells[imageColumn]);
        }

        void ugeTemplates_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            UltraGridRow activeRow = vo.ugeTemplates.ugData.ActiveRow;
            switch (toolKey)
            {
                case "btnTopLevelTemplate":
                    AddNewTemplate(activeRow, true);
                    break;
                case "btnChildTemplate":
                    AddNewTemplate(activeRow, false);
                    break;
                case "btnOpenDocument":
                    if (activeTemplate.TemplateType != TemplateTypes.Group) 
                        OpenDocument(activeTemplate.ID);
                    break;
                case "btnSaveDocument": 
                    if (activeTemplate.TemplateType != TemplateTypes.Group)
                        SaveDocument(activeTemplate.ID);
                    break;
                case "btnSetTemplateState":
                    //SetTemplateState();
                    break;
            }
        }

        GridColumnsStates ugeTemplates_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsReadOnly = true;
			state.IsHiden = true;
			states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Наименование";
            state.ColumnWidth = 200;
            states.Add("Name", state);

			state = new GridColumnState();
			state.ColumnName = "Code";
			state.ColumnCaption = "Код";
			state.ColumnWidth = 100;
			state.IsHiden = false;
			states.Add("Code", state);

			state = new GridColumnState();
            state.ColumnName = "Type";
            state.ColumnCaption = "Тип отчета репозитория";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            state.ColumnWidth = 250;
            states.Add("Type", state);

            state = new GridColumnState();
            state.ColumnName = "Description";
            state.ColumnCaption = "Описание";
            state.ColumnWidth = 200;
            state.IsHiden = true;
            states.Add("Description", state);

            state = new GridColumnState();
            state.ColumnName = "DocumentFileName";
            state.ColumnCaption = string.Empty;
            state.IsHiden = true; 
            states.Add("DocumentFileName", state);
            
            state = new GridColumnState();
            state.ColumnName = "ParentID";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("ParentID", state);

            state = new GridColumnState();
            state.ColumnName = "TemplateDocumentState";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("TemplateDocumentState", state);

            state = new GridColumnState();
            state.ColumnName = "Document";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("Document", state);

            state = new GridColumnState();
            state.ColumnName = "LastOpenData";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("LastOpenData", state);

            state = new GridColumnState();
            state.ColumnName = "PresentDocument";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("PresentDocument", state);

            state = new GridColumnState();
            state.ColumnName = "Editor";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("Editor", state);

            state = new GridColumnState();
            state.ColumnName = "TemplateState";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("TemplateState", state);
            
            
            state = new GridColumnState();
            state.ColumnName = "FileName";
            state.ColumnCaption = "Наименование файла";
            state.ColumnWidth = 200;
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add("FileName", state);

            state = new GridColumnState();
            state.ColumnName = "DocumentExt";
            state.ColumnCaption = "Тип файла";
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add("DocumentExt", state);

            return states;
        }

        #endregion

        /// <summary>
        /// добавление различных кнопок на тулбар грида
        /// </summary>
        private void UpdateGridToolBar()
        {
            //
            UltraToolbar tb = vo.ugeTemplates.utmMain.Toolbars["utbColumns"];

            ButtonTool btnTopLevelTemplate = null;
            if (!vo.ugeTemplates.utmMain.Tools.Exists("btnTopLevelTemplate"))
            {
                btnTopLevelTemplate = new ButtonTool("btnTopLevelTemplate");
                btnTopLevelTemplate.SharedProps.ToolTipText = "Добавить отчет верхнего уровня";
                btnTopLevelTemplate.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[4];
                vo.ugeTemplates.utmMain.Tools.Add(btnTopLevelTemplate);
                tb.Tools.AddTool("btnTopLevelTemplate");
            }
            else
                btnTopLevelTemplate = (ButtonTool)vo.ugeTemplates.utmMain.Tools["btnTopLevelTemplate"];

            ButtonTool btnChildTemplate = null;
            if (!vo.ugeTemplates.utmMain.Tools.Exists("btnChildTemplate"))
            {
                btnChildTemplate = new ButtonTool("btnChildTemplate");
                btnChildTemplate.SharedProps.ToolTipText = "Добавить подчиненный отчет";
                btnChildTemplate.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[5];
                vo.ugeTemplates.utmMain.Tools.Add(btnChildTemplate);
                tb.Tools.AddTool("btnChildTemplate");
            }
            else
                btnChildTemplate = (ButtonTool)vo.ugeTemplates.utmMain.Tools["btnChildTemplate"];

            ButtonTool btnCheckOut = null;
            if (!vo.ugeTemplates.utmMain.Tools.Exists("btnCheckOut"))
            {
                btnCheckOut = new ButtonTool("btnCheckOut");
                btnCheckOut.SharedProps.ToolTipText = "Взять отчет на редактирование";
                btnCheckOut.SharedProps.AppearancesSmall.Appearance.Image = vo.ilTools.Images[7];
                vo.ugeTemplates.utmMain.Tools.Add(btnCheckOut);
                tb.Tools.AddTool("btnCheckOut");
            }
            else
                btnCheckOut = (ButtonTool)vo.ugeTemplates.utmMain.Tools["btnCheckOut"];

            vo.ugeTemplates.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
            vo.ugeTemplates.utmMain.Tools["btnVisibleAddButtons"].SharedProps.Visible = false;
            vo.ugeTemplates.utmMain.Tools["excelImport"].SharedProps.Visible = false;
            vo.ugeTemplates.utmMain.Tools["excelExport"].SharedProps.Visible = false;

            btnChildTemplate.SharedProps.Visible = true;
        }

        /// <summary>
        /// добавление нового элемента в репозиторий
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="parentElementKey"></param>
        private void AddNewTemplate(UltraGridRow activeRow, bool toTopLevel)
        {
            activeTemplate = new TemplateStruct();
            UltraGridRow row = null;

            if (!toTopLevel && activeRow != null)
            {
                row = activeRow.ChildBands[0].Band.AddNew();
            }
            else
            {
                row = vo.ugeTemplates.ugData.DisplayLayout.Bands[0].AddNew();
            }
        }

        /// <summary>
        /// получение документа по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private byte[] GetDocument(int id)
        {
            DataRow[] docRows = hierarchyDs.Tables[0].Select(string.Format("ID = {0}", id));
            byte[] document = null;
            if (docRows.Length == 0)
            {
                DataRow row = hierarchyDs.Tables[0].NewRow();
                row["ID"] = id;
                document = DocumentsHelper.DecompressFile(repository[id].Document);
                row["Document"] = document;
            }
            else
            {
                if (docRows[0]["Document"] == DBNull.Value)
                    document = DocumentsHelper.DecompressFile(repository[id].Document);
                else
                    document = (byte[])docRows[0]["Document"];
            }
            hierarchyDs.Tables[0].AcceptChanges();
            return document;
        }

        

        /// <summary>
        /// открываем документ шаблона 
        /// </summary>
        /// <param name="id"></param>
        private void OpenDocument(int id)
        {
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);
            byte[] document = GetDocument(id);
            // сохраняем документ во временный каталог с документами. Запоминаем время сохранения
            // при закрытии всего воркплейса проверяем, отличается ли дата последнего изменения от даты создания
            // если да, сохраняем документ в базу
            string fileName = TemplatesDocumentsHelper.GetFullFileName(id, GetDocumentFileName(id));
            // сохраняем документ во временный каталог
            TemplatesDocumentsHelper.SaveDocument(fileName, document);

            System.Diagnostics.Process.Start(fileName);
            FileInfo fi = new FileInfo(fileName);
            DataRow row = GetDocumentRow(id);
            if (row != null)
            {
                row["LastOpenData"] = fi.LastAccessTimeUtc;
                row["DocumentFileName"] = fileName;
            }
            activeRow.Cells["TemplateDocumentState"].Value = (int)TemplateDocumentState.InEdit;
            activeRow.Update();
        }

        

        /// <summary>
        /// создает по таблице иерархический DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="parentColumnName"></param>
        /// <param name="refColumnName"></param>
        /// <param name="relationName"></param>
        /// <returns></returns>
        private DataSet CreateHierarchyDataSet(DataTable dataTable, string parentColumnName, string refColumnName, string relationName)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable.Copy());
            ds.Relations.Add(relationName, ds.Tables[0].Columns[parentColumnName], ds.Tables[0].Columns[refColumnName]);
            return ds;
        }

        /// <summary>
        /// сохранение всех изменений
        /// </summary>
        internal bool SaveData()
        {
            List<string> listErrors = new List<string>();
            if (vo.ugeTemplates.ugData.ActiveRow != null)
                vo.ugeTemplates.ugData.ActiveRow.Update();

            if (!CheckNotNillFields(ref listErrors))
            {
                StringBuilder sb = new StringBuilder();
                foreach (string str in listErrors)
                {
                    sb.AppendLine(str);
                }
                MessageBox.Show(sb.ToString(), "Предупреждение");
                return false;
            }

            DataTable addTable = hierarchyDs.Tables[0].GetChanges(DataRowState.Added);
            DataTable editTable = hierarchyDs.Tables[0].GetChanges(DataRowState.Modified);
            DataTable deleteTable = hierarchyDs.Tables[0].GetChanges(DataRowState.Deleted);
            // добавляем новые шаблоны
            if (addTable != null)
            {
                foreach (DataRow row in addTable.Rows)
                {
                    ITemplate template = repository.AddNew(Convert.ToInt32(row["ID"]));
					template.Code = row["Code"].ToString();
					template.Name = row["Name"].ToString();
                    template.TemplateType = (TemplateTypes)Convert.ToInt32(row["Type"]);
                    template.Description = row["Description"].ToString();
                    template.DocumentFileName = row["DocumentFileName"].ToString();
                    if (row["ParentID"] != DBNull.Value)
                        template.ParentID = Convert.ToInt32(row["ParentID"]);
                }
            }

            // сохраняем измененные шаблоны
            if (editTable != null)
            {
                foreach (DataRow row in editTable.Rows)
                {
                    ITemplate template = repository[Convert.ToInt32(row["ID"])];
					template.Code = row["Code"].ToString();
					template.Name = row["Name"].ToString();
                    template.Description = row["Description"].ToString();
                    template.TemplateType = (TemplateTypes)Convert.ToInt32(row["Type"]);
                    template.DocumentFileName = row["DocumentFileName"].ToString();
                }
            }

            // удаляем шаблоны
            if (deleteTable != null)
            {
                foreach (DataRow row in deleteTable.Rows)
                {
                    int id = Convert.ToInt32(row["ID", DataRowVersion.Original]);
                    if (!childDeleteRows.Contains(id))
                        repository.Remove(id);
                }
            }

            foreach (ITemplate template in editedTemplates.Values)
            {
                template.SetTemplateState(currentUser, TemplateState.EasyAccessState);
            }
            editedTemplates.Clear();

            bool isChanged = false;
            //применим изменения
            if (addTable != null || editTable != null || deleteTable != null)
            {
                isChanged = true;
                repository.ApplyChanges();
            }
            // все изменения сливаем в одну таблицу
            DataTable dt = hierarchyDs.Tables[0].Clone();
            if (addTable != null)
                dt.Merge(addTable);
            if (editTable != null)
                dt.Merge(editTable);
            // для всех добавленных или открытых на редактирование проверим, надо ли сохранять изменения
            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;
                TemplateTypes templateType = (TemplateTypes)Convert.ToInt32(row["Type"]);
                bool presentDocument = Convert.ToBoolean(row["PresentDocument"]);
                if (presentDocument)
                {
                    int id = Convert.ToInt32(row["ID"]);
                    string documentFullName = row["DocumentFileName"].ToString();
                    FileInfo fi = new FileInfo(documentFullName);
                    object test = row["LastOpenData"];
                    if (test == null || test == DBNull.Value)
                        continue;
                    if ((Convert.ToInt32(row["TemplateDocumentState"]) == (int)TemplateDocumentState.New) ||
                        (fi.LastWriteTimeUtc != Convert.ToDateTime(row["LastOpenData"])))
                    {
                        // документ был открыт, закрыт и там были какие то изменения... записываем его в базу
                        byte[] fileData = FileHelper.ReadFileData(documentFullName);
                        ITemplate template = repository[id];
                        template.Document = DocumentsHelper.CompressFile(fileData);
                        row["PresentDocument"] = true;
                    }
                }
            }
            if (isChanged)
            {
                foreach (DataRow row in hierarchyDs.Tables[0].Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        row["TemplateDocumentState"] = (int)TemplateDocumentState.NoEdit;
                        if (row["TemplateState"] is DBNull)
                            row["TemplateState"] = TemplateState.EasyAccessState;
                    }
                }
                // сохраняем изменения в документах
                hierarchyDs.Tables[0].AcceptChanges();
            }

            return true;
        }

        /// <summary>
        /// получение данных по репозиторию
        /// </summary>
        private void RefreshData()
        {
            dtTemplatesData = repository.GetTemplatesInfo();
            // формируем таблицу под документы (добавляем новые вспомогательные поля)
            dtTemplatesData.Columns.Add("TemplateDocumentState", typeof(Int32));
            dtTemplatesData.Columns.Add("Document", typeof(byte[]));
            dtTemplatesData.Columns.Add("LastOpenData", typeof(DateTime));
            dtTemplatesData.Columns.Add("PresentDocument", typeof(bool));
            dtTemplatesData.Columns.Add("FileName", typeof(string));
            dtTemplatesData.Columns.Add("DocumentExt", typeof(string));
            dtTemplatesData.Columns.Add("TemplateState", typeof(Int32));

            foreach (DataRow row in dtTemplatesData.Rows)
            {
                row["TemplateDocumentState"] = (int)TemplateDocumentState.NoEdit;
                // состояние отчета
                int templateUserEditor = Convert.ToInt32(row["Editor"]);
                if (templateUserEditor == -1)
                    row["TemplateState"] = (int)TemplateState.EasyAccessState;
                else if (templateUserEditor == currentUser)
                    row["TemplateState"] = (int)TemplateState.EditingByCurrent;
                else
                    row["TemplateState"] = (int)TemplateState.EditingState;

                if (row["DocumentFileName"] == null || row["DocumentFileName"] == DBNull.Value)
                    row["PresentDocument"] = false;
                else
                {
                    row["PresentDocument"] = true;
                    string fullFileName = row["DocumentFileName"].ToString();
                    row["DocumentExt"] = Path.GetExtension(fullFileName).ToLower();
                    row["FileName"] = Path.GetFileNameWithoutExtension(fullFileName);
                }
            }

            dtTemplatesData.AcceptChanges();
            // создаем объект для хранения иерархических данных
            hierarchyDs = CreateHierarchyDataSet(dtTemplatesData, "ID", "ParentID", "TempateRelation");
            hierarchyDs.Tables[0].Constraints.Clear();
            vo.ugeTemplates.DataSource = hierarchyDs;
            vo.ugeTemplates.ugData.DisplayLayout.AddNewBox.Hidden = true;
            vo.ugeTemplates.ugData.DisplayLayout.Bands[0].ColumnFilters["ParentID"].FilterConditions.Add(FilterComparisionOperator.Equals, DBNull.Value);
            // делаем активной первую строку в гриде
            
            if (vo.ugeTemplates.ugData.Rows.Count > 0)
            {
                foreach (UltraGridRow row in vo.ugeTemplates.ugData.Rows)
                {
                    if (row.Band.Index == 0 && row.VisibleIndex >= 0)
                    {
                        row.Activate();
                        break;
                    }
                }
            }
            SetdetailVisible();
        }

        private void SetdetailVisible()
        {
            vo.sc1.Panel2Collapsed = vo.ugeTemplates.ugData.Rows.Count <= 0;
        }

        #region проверки на заполнение всех обязательных полей

        /// <summary>
        /// проверяем, все ли основные поля в записях заполнены
        /// </summary>
        /// <returns></returns>
        private bool CheckNotNillFields(ref List<string> errorsList)
        {
            foreach (DataRow row in hierarchyDs.Tables[0].Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;
                object id = row["ID"];
                if (row.IsNull("Type"))
                {
                    errorsList.Add(string.Format("Запись с ID = {0}. Поле '{1}' не заполнено", id, "Тип элемента репозитория"));
                }
                if (row.IsNull("Name") || row["Name"].ToString() == string.Empty)
                {
                    errorsList.Add(string.Format("Запись с ID = {0}. Поле '{1}' не заполнено", id, "Наименование"));
                }
            }
            if (errorsList.Count == 0)
                return true;
            return false;
        }

        #endregion

        #region методы работы с документами. Добавление, открытие и сохранение

        /// <summary>
        /// добавление нового документа
        /// </summary>
        private void AddNewDocument()
        {
            string folder = TemplatesDocumentsHelper.GetDocsFolder();
            string docName = vo.tbDocumentName.Text;
            if (string.IsNullOrEmpty(docName))
                docName = "Новый документ";
            docName = folder + Path.DirectorySeparatorChar + activeTemplate.ID.ToString() + "_" + docName;
            object obj = null;
            try
            {
                switch (activeTemplate.TemplateType)
                {
                    case TemplateTypes.MSExcel:
                    case TemplateTypes.MSExcelPlaning:
                        ExcelHelper exlhlp = new ExcelHelper(false);
                        docName = docName + exlhlp.GetExtension();
                        obj = exlhlp.CreateEmptyDocument(docName);
                        OfficeHelper.SetObjectVisible(obj, true);
                        break;
                    case TemplateTypes.MSWord:
                    case TemplateTypes.MSWordPlaning:
                        WordHelper wrdhlp = new WordHelper(false);
                        docName = docName + wrdhlp.GetExtension();
                        obj = wrdhlp.CreateEmptyDocument(docName);
                        OfficeHelper.SetObjectVisible(obj, true);
                        break;
                    case TemplateTypes.MDXExpert:
                        break;
                }
                
                DataRow row = GetDocumentRow(activeTemplate.ID);
                FileInfo fi = new FileInfo(docName);
                activeTemplate.DocumentFileName = docName;
                activeTemplate.LastOpenData = fi.LastAccessTime;
                activeTemplate.DocumentName = Path.GetFileName(activeTemplate.DocumentFileName);
                SetRowParams();

                if (string.IsNullOrEmpty(vo.tbDocumentName.Text))
                {
                    vo.tbDocumentName.Text = activeTemplate.Name = Path.GetFileNameWithoutExtension(activeTemplate.DocumentFileName);
                }
                row["PresentDocument"] = activeTemplate.PresentDocument = true;
                ShowTemplateDetail(activeTemplate.TemplateType);
            }
            finally
            {
                if ((obj != null) && (Marshal.IsComObject(obj)))
                {
                    Marshal.ReleaseComObject(obj);
                    GC.GetTotalMemory(true);
                }
            }
        }

        /// <summary>
        /// добавление существующего документа
        /// </summary>
        private void AddDocument()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;

            switch (activeTemplate.TemplateType)
            {
                case TemplateTypes.Arbitrary:
                    openDialog.Filter = "Произвольные документы|*.*";
                    break;
                case TemplateTypes.MSExcel:
                case TemplateTypes.MSExcelPlaning:
                    openDialog.Filter = "Документы MS Excel *.xls|*.xls";
                    break;
                case TemplateTypes.MDXExpert:
                    openDialog.Filter = "Отчеты MDX Эксперт *.exd|*.exd";
                    break;
                case TemplateTypes.MSWord:
                case TemplateTypes.MSWordPlaning:
                    openDialog.Filter = "Документы MS Word *.doc|*.doc";
                    break;
                case TemplateTypes.MSWordTemplate:
                    openDialog.Filter = "Шаблоны MS Word *.dot|*.dot";
                    break;
                case TemplateTypes.MSExcelTemplate:
                    openDialog.Filter = "Шаблоны MS Excel *.xlt|*.xlt";
                    break;
            }
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                activeTemplate.DocumentFileName = openDialog.FileName;
                activeTemplate.DocumentName = Path.GetFileName(activeTemplate.DocumentFileName);
                DataRow row = GetDocumentRow(activeTemplate.ID);

                FileInfo fi = new FileInfo(activeTemplate.DocumentFileName);
                activeTemplate.Document = FileHelper.ReadFileData(activeTemplate.DocumentFileName);

                SetRowParams();

                if (string.IsNullOrEmpty(activeTemplate.Name))
                {
                    vo.tbDocumentName.Text = activeTemplate.Name = Path.GetFileNameWithoutExtension(activeTemplate.DocumentFileName);
                }
                row["PresentDocument"] = activeTemplate.PresentDocument = true;
                ShowTemplateDetail(activeTemplate.TemplateType);
            }
        }

        /// <summary>
        /// установака данных для записи
        /// </summary>
        private void SetRowParams()
        {
            DataRow row = GetDocumentRow(activeTemplate.ID);
            row["LastOpenData"] = activeTemplate.LastOpenData;
            row["Document"] = activeTemplate.Document;

            if (activeTemplate.TemplateType != TemplateTypes.Group)
            {
                UltraGridRow gridRow = UltraGridHelper.GetActiveRowCells(vo.ugeTemplates.ugData);

                gridRow.Cells["DocumentFileName"].Value = activeTemplate.DocumentFileName;
                gridRow.Cells["FileName"].Value = Path.GetFileNameWithoutExtension(activeTemplate.DocumentFileName);
                gridRow.Cells["DocumentExt"].Value = Path.GetExtension(activeTemplate.DocumentFileName).ToLower();
            }
        }

        private void SaveDocument(int id)
        {
            byte[] document = GetDocument(id);
            // сохраняем документ во временный каталог с документами. Запоминаем время сохранения
            // при закрытии всего воркплейса проверяем, отличается ли дата последнего изменения от даты создания
            // если да, сохраняем документ в базу
            string fileName = GetDocumentFileName(id);
            if (TemplatesDocumentsHelper.ShowSaveDialog(ref fileName))
                TemplatesDocumentsHelper.SaveDocument(fileName, document);
        }

        /// <summary>
        /// проверяет, нужно ли записывать в базу открытый документ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckDocumentData(int id, string fileName)
        {
            DataRow row = GetDocumentRow(id);
            if (row == null)
                return false;
            FileInfo fi = new FileInfo(fileName);
            DateTime docLastWrite = fi.LastWriteTimeUtc;
            DateTime docLastOpen = Convert.ToDateTime(row["LastOpenData"]);
            if (docLastOpen != docLastWrite)
                return true;
            return false;
        }

        /// <summary>
        /// получение записи документа по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataRow GetDocumentRow(int id)
        {
            DataRow[] rows = hierarchyDs.Tables[0].Select(string.Format("ID = {0}", id));
            if (rows.Length > 0)
                return rows[0];
            return null;
        }

        private string GetDocumentFileName(int id)
        {
            DataRow row = GetDocumentRow(id);
            if (row != null)
            {
                string docName = row["Name"].ToString();
                docName += Path.GetExtension(row["DocumentFileName"].ToString());
                return docName;
            }
            return string.Empty;
        }

        #endregion
    }

    internal struct TemplateStruct
    {
        internal int ID;

        internal string Name;

        internal TemplateTypes TemplateType;

        internal string Description;

        internal int ParentID;

        internal string DocumentName;

        internal string DocumentFileName;

        internal byte[] Document;

        internal bool PresentDocument;

        internal DateTime LastOpenData;

        internal TemplateState TemplateState;

        internal void ClearParams()
        {
            ID = -1;

            Name = string.Empty;

            Description = string.Empty;

            ParentID = -1;

            DocumentName = string.Empty;

            DocumentFileName = string.Empty;

            Document = null;

            PresentDocument = false;

            TemplateState = TemplateState.EasyAccessState;
        }
    }
}
