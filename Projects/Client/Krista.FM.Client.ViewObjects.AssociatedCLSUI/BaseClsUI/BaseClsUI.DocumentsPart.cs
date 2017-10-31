using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Common.FileUtils;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {

        private UIElement element;
        // наименования полей, отвечающих за работу с документами
        private string activeDocumentColumnName;
        // полное имя документа
        private string activeDocumentFullNameColumnName;
        // тип документа
        private string activeDocumentTypeColumnName;
        // имя документа
        private string activeDocumentNameColumnName;

        private const string openDocument = "_OpenDoc";

        private const string saveDocument = "_SaveDoc";

        private const string createNewDoc = "_CreateDoc";

        private const string documentType = "_DocumentType";

        private const string documentExtension = "_DocumentExtension";

        private const string headerMultiColumns = "_HeaderMultiColumns";

        /// <summary>
        /// Название папки документов
        /// </summary>
        private const string DocsFolderName = "Documents";


        /// <summary>
        /// удаление документа (не из базы)
        /// </summary>
        private void DeleteCurrentDocument()
        {
            UltraGridRow dataRow = UltraGridHelper.GetActiveRowCells(activeGrid.ugData);
            // наименование не очищаем
            //dataRow.Cells[activeDocumentNameColumnName].Value = null;
            dataRow.Cells[activeDocumentFullNameColumnName].Value = null;
            dataRow.Cells[activeDocumentColumnName].Value = DBNull.Value;
            dataRow.Cells[GetDocumentTypeColumnName(activeDocumentColumnName)].Value = null;
            dataRow.Cells[GetDocumentExtColumnName(activeDocumentColumnName)].Value = string.Empty;
            dataRow.Update();
            dataRow.Refresh(RefreshRow.FireInitializeRow);
        }

        private void OpenDocument(DataRow row)
        {
            // открытие документа из базы.
            // создаем локальную копию и ее уже открываем
            Workplace.OperationObj.Text = "Поиск документа";
            Workplace.OperationObj.StartOperation();
            try
            {
                UltraGridRow dataRow = UltraGridHelper.GetActiveRowCells(activeGrid.ugData);
                if (dataRow.Cells[activeDocumentColumnName].Value == null || dataRow.Cells[activeDocumentColumnName].Value == DBNull.Value)
                    return;
                
                string fileName = dataRow.Cells[activeDocumentNameColumnName].Value.ToString();
                string fullName = dataRow.Cells[activeDocumentFullNameColumnName].Value.ToString();
                string fileExtension = Path.GetExtension(fullName);
                // получаем имя файла относительно того, где установлен клиент
                fileName = GetFullFileName(fileName + fileExtension, activeDocumentColumnName, Convert.ToInt32(dataRow.Cells["ID"].Value));
                if (!File.Exists(fileName))
                {
                    // если файл не существует, создаем новый
                    FileStream fs = null;
                    try
                    {
                        // Попытаемся зааполнить ячейку данными из БД.
                        IDocumentAttribute attr = GetDocumentAttribute();
                        FillDocumentCell(row, ((IDocumentAttribute)attr).GetDocumentDataFromDataBase(activeObject.FullDBName, UltraGridHelper.GetActiveID(activeGrid.ugData)));
                        // если никаких даннх нету выходим
                        byte[] compressData = (byte[])dataRow.Cells[activeDocumentColumnName].Value;
                        byte[] decompressData = DecompressFile(compressData);
                        fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
                        fs.Write(decompressData, 0, decompressData.Length);
                        compressData = null;
                        decompressData = null;
                    }
                    finally
                    {
                        if (fs != null) fs.Close();
                        GC.GetTotalMemory(true);
                    }
                }
                else
                {
                    // если существует, открываем его... 
                    // Но сначала заполним из него ячейку.
                    byte[] fileData = CompressFile(FileHelper.ReadFileData(fileName));
                    FillDocumentCell(row, fileData);
                }
                System.Diagnostics.Process.Start(fileName);
                activeGrid.BurnChangesDataButtons(true);
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        // Для получения атрибута документа ничего умнее не придумал.
        private IDocumentAttribute GetDocumentAttribute()
        {
            foreach (KeyValuePair<string, IDataAttribute> attribute in activeObject.Attributes)
            {
                if (attribute.Value is IDocumentAttribute)
                {
                    return (IDocumentAttribute) attribute.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Заполняет ячейку документа данными.
        /// </summary>
        /// <param name="dataRow">Строка с ячейкой.</param>
        /// <param name="documentData">Данные.</param>
        private void FillDocumentCell(DataRow row, byte[] documentData)
        {
            // Получаем данные, пишем в ячейку.
            row[activeDocumentColumnName] = documentData;
        }

        private void SaveDocument(DataRow row)
        {
            // сохранение документа
            // создаем диалог с сохранением. Имя файла выбираем исходя из имени документа. 
            try
            {
                UltraGridRow dataRow = UltraGridHelper.GetActiveRowCells(activeGrid.ugData);
                if (dataRow.Cells[activeDocumentFullNameColumnName].Value == null ||
                    dataRow.Cells[activeDocumentFullNameColumnName].Value == DBNull.Value)
                    return;

                string fileName = dataRow.Cells[activeDocumentNameColumnName].Value.ToString();
                string fullName = dataRow.Cells[activeDocumentFullNameColumnName].Value.ToString();
                string fileExtension = Path.GetExtension(fullName);

                if (ExportImportHelper.GetFileName(fileName, fileExtension, true, ref fileName))
                {
                    FileStream fs = null;
                    byte[] compressData = new byte[0];
                    byte[] fileData = new byte[0];
                    try
                    {
                        // Попытаемся зааполнить ячейку данными из БД.
                        IDocumentAttribute attr = GetDocumentAttribute();
                        FillDocumentCell(row, ((IDocumentAttribute)attr).GetDocumentDataFromDataBase(activeObject.FullDBName, Convert.ToInt32(row["ID"])));
                        dataRow.Cells[string.Format("{0}{1}", activeDocumentColumnName, saveDocument)].Activation = Activation.AllowEdit;
                        compressData = (byte[])dataRow.Cells[activeDocumentColumnName].Value;
                        fileData = DecompressFile(compressData);
                        fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                        fs.Write(fileData, 0, fileData.Length);
                    }
                    finally
                    {
                        fileData = null;
                        fileData = null;
                        if (fs != null) fs.Close();
                    }
                }
            }
            finally
            {
                GC.GetTotalMemory(true);
            }
        }

        private string GetFullFileName(string fileName, string documentColumnName, int documentRowID)
        {
            string documentsDirectory = AppDomain.CurrentDomain.BaseDirectory + DocsFolderName;;
            if (!Directory.Exists(documentsDirectory))
                Directory.CreateDirectory(documentsDirectory);
            // добавим к названию файла наименование колонки и ID записи
            // для получения уникального имени каждого файла
            int blobFieldsCount = 0;
            foreach (GridColumnState state in activeGrid.CurrentStates.Values)
            {
                if (state.IsBLOB)
                    blobFieldsCount++;
            }

            string oldPrefixName = string.Format("{0}_{1}_{2}", activeObject.ID, documentRowID, documentColumnName);
            string newPrefixName = string.Format("{0}_{1}", activeObject.ID, documentRowID);
            if (blobFieldsCount < 2)
            {
                if (fileName.Contains(oldPrefixName))
                    fileName = fileName.Replace(oldPrefixName, string.Empty);

                if (!fileName.Contains(newPrefixName))
                    fileName = string.Format("{0}_{1}_{2}", activeObject.ID, documentRowID, fileName);
            }
            else
            {
                if (!fileName.Contains(oldPrefixName))
                    fileName = string.Format("{0}_{1}_{2}_{3}", activeObject.ID, documentRowID, documentColumnName, fileName);
            }
            return documentsDirectory+ "\\" + fileName;
        }

        private void AddNewDocument(string fillDocumentPath, string documentName)
        {
            if (File.Exists(fillDocumentPath))
            {
                byte[] fileData = new byte[0];
                byte[] compressData = new byte[0];
                try
                {
                    // сохраняем пожатые данные
                    fileData = FileHelper.ReadFileData(fillDocumentPath);
                    compressData = CompressFile(fileData);
                    UltraGridRow dataRow = UltraGridHelper.GetActiveRowCells(activeGrid.ugData);
                    dataRow.Cells[activeDocumentColumnName].Value = compressData;
                    // заполняем дополнительные для документа поля
                    string currentDocementName = documentName == string.Empty ? Path.GetFileNameWithoutExtension(fillDocumentPath) : documentName;
                    if (dataRow.Cells[activeDocumentNameColumnName].Value.ToString() == string.Empty)
                        dataRow.Cells[activeDocumentNameColumnName].Value = Path.GetFileNameWithoutExtension(currentDocementName);
                    dataRow.Cells[activeDocumentFullNameColumnName].Value = Path.GetFileName(fillDocumentPath);
                    string fileExtension = Path.GetExtension(fillDocumentPath);
                    dataRow.Cells[GetDocumentExtColumnName(activeDocumentColumnName)].Value = fileExtension;
                    dataRow.Cells[activeDocumentTypeColumnName].Value = (int)ResolveDocumentType(fillDocumentPath);
                }
                finally
                {
                    fileData = null;
                    compressData = null;
                    GC.GetTotalMemory(true);
                }
            }
        }

        void cmsCreateDocument_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            vo.cmsCreateDocument.Hide();
            string newDocumentName = String.Empty;
            switch (e.ClickedItem.Name)
            {
                case "tsmiSelectDocument":
                    string openDocumentName = string.Empty;
                    if (ExportImportHelper.GetFileName(openDocumentName, ExportImportHelper.fileExtensions.Unknown, false, ref openDocumentName))
                    {
                        // типа выбрали документ произвольный, будем его добавлять
                        AddNewDocument(openDocumentName, string.Empty);
                    }
                    break;
                case "tsmiCreateNewExcel":
                    // создаем новый экземпляр екселя
                    string newExcelFileName;
                    using (ExcelApplication excel = OfficeHelper.CreateExcelApplication())
                    {
                        newDocumentName = GetNewDocumentName(excel.GetExtension());
                        newExcelFileName = GetFullFileName(newDocumentName, activeDocumentColumnName, UltraGridHelper.GetActiveID(activeGrid.ugData));
                        excel.CreateEmptyDocument(newExcelFileName);
                        excel.Visible = true;
                    }
                    AddNewDocument(newExcelFileName, newDocumentName);
                    break;
                case "tsmiCreateNewWord":
                    // создаем новый экземпляр ворда
                    string newWordFileName;
                    using (WordApplication word = OfficeHelper.CreateWordApplication())
                    {
                        newDocumentName = GetNewDocumentName(word.GetExtension());
                        newWordFileName = GetFullFileName(newDocumentName, activeDocumentColumnName, UltraGridHelper.GetActiveID(activeGrid.ugData));
                        word.CreateEmptyDocument(newWordFileName);
                        word.Visible = true;
                    }
                    AddNewDocument(newWordFileName, newDocumentName);
                    break;
                case "tsmiDeleteDocument":
                    // удаляем документ, что в данной ячейке. Как же мочим документ в кеше
                    DeleteCurrentDocument();
                    break;
            }
        }

        private string GetNewDocumentName(string documentExt)
        {
            UltraGridRow dataRow = UltraGridHelper.GetActiveRowCells(activeGrid.ugData);
            string fileName = dataRow.Cells[activeDocumentNameColumnName].Value.ToString();
            if (fileName == string.Empty)
                return "Новый документ" + documentExt;
            return fileName + documentExt;
        }


        private void GetActiveBlobCellParams()
        {
            // при нажатии любой из кнопок, относящихся к документу получаем названия колонок, относящихся к документу
            if (activeGrid.ugData.ActiveRow == null)
                return;
            if (activeGrid.ugData.ActiveCell == null)
                return;

            string activeCellKey = activeGrid.ugData.ActiveCell.Column.Key;

            string blobCellKey = activeCellKey.Replace(openDocument, string.Empty);
            blobCellKey = blobCellKey.Replace(saveDocument, string.Empty);
            activeDocumentColumnName = blobCellKey.Replace(createNewDoc, string.Empty);
            activeDocumentFullNameColumnName = GetFullDocumentColumnName(activeDocumentColumnName);
            activeDocumentTypeColumnName = GetDocumentTypeColumnName(activeDocumentColumnName);
            activeDocumentNameColumnName = GetDocumentNameColumnName(activeDocumentColumnName);
        }


        private ClassifierDocumentType ResolveDocumentType(string fileName)
        {
            ClassifierDocumentType dt = ClassifierDocumentType.dtArbitraryDocument;
            string fileExt = Path.GetExtension(fileName);
            switch (fileExt)
            {
                case ".doc":
                    dt = ClassifierDocumentType.dtArbitraryWordDocument;
                    // если плагин доступен - пытаемся получить характеристики документа
                    //if (wordHelper.PlaginInstalled)
                    //    GetDocumentTypeFromHelper(wordHelper, fileName, ref dt);
                    
                    break;
                case ".exd":
                    dt = ClassifierDocumentType.dtMDXExpertDocument;
                    break;
                // файлы Excel, в свою очередь, могут являться документами планирования
                case ".xls":
                    // пока не делаем ничего для документов планирования
                    dt = ClassifierDocumentType.dtArbitraryExcelDocument;
                    // если плагин доступен - пытаемся получить характеристики документа
                    //if (excelHelper.PlaginInstalled)
                    //    GetDocumentTypeFromHelper(excelHelper, fileName, ref dt);
                    break;
            }
            return dt;
        }


        internal static void AddDocumentsTypeListToGrid(UltraGrid ugData)
        {
            ValueList list = null;
            if (!ugData.DisplayLayout.ValueLists.Exists("DocumentsTypes"))
            {
                list = ugData.DisplayLayout.ValueLists.Add("DocumentsTypes");
               
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "Произвольный документ";
                item.DataValue = (int)ClassifierDocumentType.dtArbitraryDocument;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "Надстройка MS Excel - расчетный лист";
                item.DataValue = (int)ClassifierDocumentType.dtCalcList;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "Надстройка MS Excel - форма ввода";
                item.DataValue = (int)ClassifierDocumentType.dtInputForm;

                item = list.ValueListItems.Add("item3");
                item.DisplayText = "Надстройка MS Excel - форма сбора данных";
                item.DataValue = (int)ClassifierDocumentType.dtDataCaptureList;

                item = list.ValueListItems.Add("item4");
                item.DisplayText = "Надстройка MS Excel - отчет";
                item.DataValue = (int)ClassifierDocumentType.dtReport;

                item = list.ValueListItems.Add("item5");
                item.DisplayText = "Надстройка MS Word – отчет";
                item.DataValue = (int)ClassifierDocumentType.dtWordReport;

                item = list.ValueListItems.Add("item6");
                item.DisplayText = "Документ MDX Expert";
                item.DataValue = (int)ClassifierDocumentType.dtMDXExpertDocument;

                /*item = list.ValueListItems.Add("item7");
                item.DisplayText = "Лист планирования";
                item.DataValue = (int)ClassifierDocumentType.dtPlaningSheet;*/

                item = list.ValueListItems.Add("item8");
                item.DisplayText = "Документ MS Excel";
                item.DataValue = (int)ClassifierDocumentType.dtArbitraryExcelDocument;

                item = list.ValueListItems.Add("item9");
                item.DisplayText = "Документ MS Word";
                item.DataValue = (int)ClassifierDocumentType.dtArbitraryWordDocument;

                item = list.ValueListItems.Add("item10");
                item.DisplayText = "Задача (XML файл)";
                item.DataValue = (int)ClassifierDocumentType.dtTaskXMLDocument;

                item = list.ValueListItems.Add("item11");
                item.DisplayText = "Объект системы (XML файл)";
                item.DataValue = (int)ClassifierDocumentType.dtSchemeObjectXMLDocument;
            }
            else
                list = ugData.DisplayLayout.ValueLists["DocumentsTypes"];
        }



        private static void SetValueListToColumn(ValueList list, UltraGridColumn column)
        {
            column.ValueList = list;
            column.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
        }


        protected void SetDocumentRow(UltraGridRow row, GridColumnsStates gridStates)
        {
            // настройка картинок, кнопочек для работы с документами
            foreach (UltraGridCell cell in row.Cells)
            {
                string keyCellColumn = cell.Column.Key;

                if (!row.Band.Columns.Exists(GetFullDocumentColumnName(keyCellColumn)))
                    continue;

                if (gridStates.ContainsKey(keyCellColumn) && gridStates[keyCellColumn].IsBLOB)
                {
                    string fullDocumentName = row.Cells[GetFullDocumentColumnName(keyCellColumn)].Value.ToString();
                    // Если имени документа нет, то гасим обе кнопки.
                    if (string.IsNullOrEmpty(fullDocumentName))
                    {
                        row.Cells[string.Format("{0}{1}", keyCellColumn, openDocument)].Activation = Activation.Disabled;
                        row.Cells[string.Format("{0}{1}", keyCellColumn, saveDocument)].Activation = Activation.Disabled;
                    }
                    else
                    {
                        // если есть полное название документа, то это значит, что документ так или иначе есть либо в базе либо
                        // в самой записи, если он еще не был сохранен.
                        row.Cells[string.Format("{0}{1}", keyCellColumn, openDocument)].Activation = Activation.AllowEdit;
                        row.Cells[string.Format("{0}{1}", keyCellColumn, saveDocument)].Activation = Activation.AllowEdit;
                        row.Cells[GetDocumentExtColumnName(keyCellColumn)].Value = Path.GetExtension(fullDocumentName);
                        row.Update();
                    }
                }

                if (keyCellColumn.Contains(openDocument))
                {
                    cell.ToolTipText = "Открыть документ";
					cell.ButtonAppearance.Image = this.vo.ilImages.Images[18];
                }
                if (keyCellColumn.Contains(saveDocument))
                {
                    cell.ToolTipText = "Сохранить документ";
					cell.ButtonAppearance.Image = this.vo.ilImages.Images[17];
                }

                if (keyCellColumn.Contains(createNewDoc))
                {
                    cell.ToolTipText = "Файл";
					cell.ButtonAppearance.Image = this.vo.ilImages.Images[19];
                }
            }
        }


        internal static void AddDocumentButtons(UltraGridLayout layout, GridColumnsStates states)
        {
            foreach (UltraGridBand band in layout.Bands)
            {
                foreach (UltraGridColumn column in band.Columns)
                {
                    if (states.ContainsKey(UltraGridEx.GetSourceColumnName(column.Key)))
                        if (states[UltraGridEx.GetSourceColumnName(column.Key)].IsBLOB/* && IsDocumentColumn(column.Key)*/)
                        {
                            if (band.Columns.Exists(column.Key + createNewDoc))
                                continue;

                            if (!band.Columns.Exists(GetDocumentNameColumnName(column.Key)))
                                continue;

                            UltraGridColumn keyColumn = band.Columns.Add(column.Key + createNewDoc);
                            keyColumn.Header.VisiblePosition = column.Header.VisiblePosition;
                            UltraGridHelper.SetLikelyButtonColumnsStyle(keyColumn, -1);
                            keyColumn.Header.Enabled = false;
                            keyColumn.RowLayoutColumnInfo.LabelPosition = LabelPosition.None;

                            keyColumn = band.Columns.Add(column.Key + openDocument);
                            keyColumn.Header.VisiblePosition = column.Header.VisiblePosition + 1;
                            UltraGridHelper.SetLikelyButtonColumnsStyle(keyColumn, -1);
                            keyColumn.Header.Enabled = false;
                            keyColumn.RowLayoutColumnInfo.LabelPosition = LabelPosition.None;

                            keyColumn = band.Columns.Add(column.Key + saveDocument);
                            keyColumn.Header.VisiblePosition = column.Header.VisiblePosition + 2;
                            UltraGridHelper.SetLikelyButtonColumnsStyle(keyColumn, -1);
                            keyColumn.Header.Enabled = false;
                            keyColumn.RowLayoutColumnInfo.LabelPosition = LabelPosition.None;

                            UltraGridColumn documentNameColumn = band.Columns[GetDocumentNameColumnName(column.Key)];
                            documentNameColumn.HiddenWhenGroupBy = DefaultableBoolean.False;

                            // добавляем список типов документов в колонку 
                            UltraGridColumn documentTypeColumn = band.Columns[GetDocumentTypeColumnName(column.Key)];
                            SetValueListToColumn(layout.ValueLists["DocumentsTypes"], documentTypeColumn);

                            // создаем дополнительные колонки для отображения типа файла и его расширения
                            UltraGridColumn documentExtensionColumn = band.Columns.Add(GetDocumentExtColumnName(column.Key), string.Format("{0} Тип файла", documentNameColumn.Header.Caption));
                            documentExtensionColumn.CellActivation = Activation.ActivateOnly;
                            documentExtensionColumn.Header.VisiblePosition = documentTypeColumn.Header.VisiblePosition + 1;
                            documentExtensionColumn.Hidden = true;

                            band.Override.AllowColMoving = AllowColMoving.NotAllowed;
                        }
                }
            }
        }

        private byte[] CompressFile(byte[] compressedFileBufer)
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
            compressStream.Write(compressedFileBufer, 0, compressedFileBufer.Length);
            compressStream.Close();
            return ms.ToArray();
        }


        private byte[] GetBuferFromStream(MemoryStream stream)
        {
            return stream.ToArray();
        }


        public int ReadAllBytesFromStream(Stream stream, List<byte> list)
        {
            try
            {
                int offset = 0;
                int totalCount = 0;
                while (true)
                {
                    int byteRead = stream.ReadByte();

                    if (byteRead == -1)
                        break;
                    list.Add((byte)byteRead);
                    offset++;
                    totalCount++;
                }
                return totalCount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        } 


        private byte[] DecompressFile(byte[] compressedFileBufer)
        {
            MemoryStream ms = null;
            DeflateStream decompressedzipStream = null;
            try
            {
                ms = new MemoryStream(compressedFileBufer);
                ms.Write(compressedFileBufer, 0, compressedFileBufer.Length);
                decompressedzipStream = new DeflateStream(ms, CompressionMode.Decompress, true);
                ms.Position = 0;
                List<byte> list = new List<byte>();
                ReadAllBytesFromStream(decompressedzipStream, list);
                return list.ToArray();
            }
            finally
            {
                decompressedzipStream.Close();
            }
        }


        /// <summary>
        /// Проверка и сохранение всех локальных копий документов текущего объекта.
        /// </summary>
        protected void SaveAllDocuments(DataSet ds, GridColumnsStates states, string objectFullDBName)
        {
            SaveAllDocuments(ds, states, objectFullDBName, false);
        }

        /// <summary>
        /// Проверка и сохранение всех локальных копий документов текущего объекта.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="states"></param>
        /// <param name="objectFullDBName"></param>
        /// <param name="needSave">Признак необходимости сохранения в БД.</param>
        protected void SaveAllDocuments(DataSet ds, GridColumnsStates states, string objectFullDBName, bool needSave)
        {
            // сохраняем в том случае, если есть локальная копия и она чем то отличается от той, что в базе
            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                        SaveDocumentsDataToRow(row, states, objectFullDBName, needSave);
                }
            }
        }

        /// <summary>
        /// Проверка и сохранение документа текущей записи.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="states"></param>
        /// <param name="objectFullDBName"></param>
        /// <param name="needSave">Признак необходимости сохранения в БД.</param>
        private void SaveDocumentsDataToRow(DataRow row, GridColumnsStates states, string objectFullDBName, bool needSave)
        {
            if (row.RowState == DataRowState.Deleted)
                return;
            foreach (GridColumnState state in states.Values)
            {
                if (state.IsBLOB)
                {
                    // берем текущую версию документа из временной копии и сохраняем ее в базу
                    // получаем полное текущее имя файла
                    if (!row.Table.Columns.Contains(GetFullDocumentColumnName(state.ColumnName)))
                        continue;
                    string fullFileName = Convert.ToString(row[GetFullDocumentColumnName(state.ColumnName)]);
                    string fileName = Convert.ToString(row[GetDocumentNameColumnName(state.ColumnName)]);
                    string fileExtension = Path.GetExtension(fullFileName);
                    fullFileName = GetFullFileName(fileName + fileExtension, state.ColumnName, Convert.ToInt32(row["ID"]));
                    // Если данные получены и файл есть
                    if (row[state.ColumnName] != null && row[state.ColumnName] != DBNull.Value)
                    {
                        byte[] currentFileData = (byte[])row[state.ColumnName];
                        if (File.Exists(fullFileName))
                        {
                            byte[] fileData = CompressFile(FileHelper.ReadFileData(fullFileName));
                            if (!CompareData(currentFileData, fileData))
                            {
                                if (needSave)
                                {
                                    GetDocumentAttribute().
                                        SaveDocumentDataToDataBase(fileData, objectFullDBName, Convert.ToInt32(row["ID"]));
                                }
                                fileData = null;
                            }
                        }
                        else
                        {
                            GetDocumentAttribute().
                                SaveDocumentDataToDataBase(currentFileData, objectFullDBName, Convert.ToInt32(row["ID"]));
                        }
                        currentFileData = null;
                    }
                }
            }
        }

        /// <summary>
        /// проверка двух массивов
        /// </summary>
        /// <param name="buf1"></param>
        /// <param name="buf2"></param>
        /// <returns></returns>
        public bool CompareData(byte[] buf1, byte[] buf2)
        {
            if (buf1.Length != buf2.Length)
                return false;
            for (int i = 0; i < buf1.Length; i++)
                if (buf1[i] != buf2[i])
                    return false;
            return true;
        }

        protected void AddDocumentRow(string fileName, UltraGridEx gridEx)
        {
            UltraGridRow row = gridEx.ugData.DisplayLayout.Bands[0].AddNew();
            foreach (GridColumnState state in gridEx.CurrentStates.Values)
            {
                if (state.IsBLOB)
                {
                    byte[] fileData = FileHelper.ReadFileData(fileName);
                    string documentColumnName = state.ColumnName;
                    row.Cells[documentColumnName].Value = CompressFile(fileData);
                    row.Cells[GetFullDocumentColumnName(documentColumnName)].Value = Path.GetFileName(fileName);
                    if (row.Cells[GetDocumentNameColumnName(documentColumnName)].Value == null || row.Cells[documentColumnName + "Name"].Value == DBNull.Value)
                        row.Cells[GetDocumentNameColumnName(documentColumnName)].Value = Path.GetFileNameWithoutExtension(fileName);
                    string fileExtension = Path.GetExtension(fileName);
                    row.Cells[GetDocumentTypeColumnName(documentColumnName)].Value = (int)ResolveDocumentType(fileName);
                    row.Cells[GetDocumentExtColumnName(documentColumnName)].Value = fileExtension;
                    break;
                }
            }
        }

        void ugeCls_OnAfterColumnHideShow(object sender, string columnName, bool isColumnHide)
        {
            UltraGridEx uexGrid = (UltraGridEx)sender;
            bool isBlobColumn = false;
            string tmpBLOBColumn = columnName.ToUpper().Replace("NAME", string.Empty);
            if (uexGrid.CurrentStates.ContainsKey(tmpBLOBColumn))
                if (uexGrid.CurrentStates[tmpBLOBColumn].IsBLOB)
                    isBlobColumn = true;

            if (!isBlobColumn) return;

            foreach (UltraGridBand band in uexGrid.ugData.DisplayLayout.Bands)
            {
                band.Columns[tmpBLOBColumn + createNewDoc].Hidden = isColumnHide;
                band.Columns[tmpBLOBColumn + openDocument].Hidden = isColumnHide;
                band.Columns[tmpBLOBColumn + saveDocument].Hidden = isColumnHide;
            }
        }

        #region работа с полями, связанных с документом

		private static string GetFullDocumentColumnName(string documentColumnName)
        {
            return documentColumnName + "FileName";
        }

        private static string GetDocumentNameColumnName(string documentColumnName)
        {
            return documentColumnName + "Name";
        }

		private static string GetDocumentTypeColumnName(string documentColumnName)
        {
            return documentColumnName + "Type";
        }

		private static string GetDocumentExtColumnName(string documentColumnName)
        {
            return documentColumnName + "FileName" + documentExtension;
        }

        #endregion

    }
}
