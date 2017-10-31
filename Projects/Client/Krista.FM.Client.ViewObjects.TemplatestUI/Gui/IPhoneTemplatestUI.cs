using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Common.Services;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Infragistics.Win;

using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Gui
{
    internal class IPhoneTemplatestUI : WebTemplatestUIBase
    {
        private HashSet<int> expandedRowsId; 

        //Имена колонок в таблице
        const string cnSubjectDepended = "SubjectDepended";
        const string cnIsNotScrollable = "IsNotScrollable";
        const string cnLastDeployDate = "LastDeployDate";
        const string cnMobileTemplateType = "TemplateType";
        const string cnIcon = "IconByte";
        const string cnForumDiscussionID = "ForumDiscussionID";
        const string cnTerritoryRF = "TerritoryRF";
        const string cnTerritoryRFName = "TerritoryRFName";
        const string cnThemeSection = "ThemeSection";
        const string cnThemeSectionName = "ThemeSectionName";

        public IPhoneTemplatestUI()
            : base(typeof(IPhoneTemplatestUI).FullName, TemplateTypes.IPhone)
        {
            Caption = "Mobile-отчеты";
            expandedRowsId = new HashSet<int>();
        }

        public override System.Drawing.Icon Icon
        {
            get { return ResourceService.GetIcon("PdaBlack"); }
        }

        public override void Initialize()
        {
            base.Initialize();
            ViewObject.ugeTemplates.OnGridInitializeLayout += new GridInitializeLayout(ugeTemplates_OnGridInitializeLayout);
            ViewObject.ugeTemplates.OnInitializeRow += new InitializeRow(ugeTemplates_OnInitializeRow);
            ViewObject.ugeTemplates.OnClickCellButton += new ClickCellButton(ugeTemplates_OnClickCellButton);
            ViewObject.ugeTemplates.ugData.BeforeRowExpanded += ugData_BeforeRowExpanded;
        }
        
        public override void ReloadData()
        {
            base.ReloadData();

            expandedRowsId.Clear();
        }

        /// <summary>
        /// Возвращает тип документа используемый по умолчанию.
        /// </summary>
        internal override TemplateDocumentTypes DefaultDocumentTypes
        {
            get { return TemplateDocumentTypes.WebReport; }
        }

        protected override DataTable GetTemplatesInfo()
        {
            IList<TemplateDTO> templates = Repository.GetTemplatesInfo(TemplateTypes.IPhone, null);

            return  GetDataTable(templates);
        }

        private DataTable GetDataTable(IList<TemplateDTO> templates)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(TemplateFields.ID, typeof (int));
            DataColumn c = new DataColumn(TemplateFields.ParentID, typeof (int));
            c.AllowDBNull = true;
            dt.Columns.Add(c);
            dt.Columns.Add(TemplateFields.Type, typeof (int));
            dt.Columns.Add(TemplateFields.Name, typeof (string));
            dt.Columns.Add(TemplateFields.Description, typeof (string));
            dt.Columns.Add(TemplateFields.DocumentFileName, typeof (string));
            dt.Columns.Add(TemplateFields.Document, typeof (byte[]));
            dt.Columns.Add(TemplateFields.Editor, typeof (int));
            dt.Columns.Add(TemplateFields.Code, typeof (string));
            c = new DataColumn(TemplateFields.SortIndex, typeof (int));
            c.AllowDBNull = true;
            dt.Columns.Add(c);
            dt.Columns.Add(TemplateFields.LastEditData, typeof(DateTime));
            dt.Columns.Add(TemplateFields.Flags, typeof (int));
            dt.Columns.Add(TemplateFields.RefTemplatesTypes, typeof (int));
            dt.Columns.Add(cnSubjectDepended, typeof (bool));
            dt.Columns.Add(cnIsNotScrollable, typeof (bool));
            dt.Columns.Add(cnLastDeployDate, typeof (DateTime));
            dt.Columns.Add(cnMobileTemplateType, typeof (int));
            dt.Columns.Add(cnIcon, typeof (byte[]));
            dt.Columns.Add(cnForumDiscussionID, typeof (int));
            dt.Columns.Add(TemplateFields.IsVisible, typeof (int));
            dt.Columns.Add(cnTerritoryRF, typeof (string));
            dt.Columns.Add(cnThemeSection, typeof(string));
            dt.Columns.Add(cnTerritoryRFName, typeof(string));
            dt.Columns.Add(cnThemeSectionName, typeof(string));

            foreach (var template in templates)
            {
                DataRow row = dt.NewRow();

                row.BeginEdit();

                row[TemplateFields.ID] = template.ID;
                row[TemplateFields.ParentID] = template.ParentID ?? (object) DBNull.Value;
                row[TemplateFields.Type] = template.Type;
                row[TemplateFields.Name] = template.Name;
                row[TemplateFields.Description] = template.Description;
                row[TemplateFields.DocumentFileName] = template.DocumentFileName;
                row[TemplateFields.Editor] = template.Editor;
                row[TemplateFields.Code] = template.Code;
                row[TemplateFields.SortIndex] = template.SortIndex ?? (object) DBNull.Value;
                row[TemplateFields.Flags] = template.Flags;
                row[TemplateFields.RefTemplatesTypes] = template.RefTemplatesTypes;
                row[cnSubjectDepended] = template.SubjectDepended;
                row[cnIsNotScrollable] = template.IsNotScrollable;
                row[cnLastDeployDate] = template.LastDeployDate;
                row[cnMobileTemplateType] = template.TemplateType;
                row[cnIcon] = template.IconByte;
                row[cnForumDiscussionID] = template.ForumDiscussionID;
                row[cnTerritoryRF] = template.TerritoryRF;
                row[cnThemeSection] = template.ThemeSection;
                row[TemplateFields.IsVisible] = true;

                row[cnTerritoryRFName] = GetTerritoriesNames(template.TerritoryRF);
                row[cnThemeSectionName] = GetTemeSectionNames(template.ThemeSection);

                row.EndEdit();

                dt.Rows.Add(row);
            }

            dt.AcceptChanges();

            return dt;
        }

        void ugData_BeforeRowExpanded(object sender, CancelableRowEventArgs e)
        {
            int id = (int) e.Row.Cells["ID"].Value;

            if (expandedRowsId.Contains(id))
            {
                return;
            }

            IList<TemplateDTO> templates = Repository.GetTemplatesInfo(TemplateTypes.IPhone, id);

            DataTable table = GetDataTable(templates);

            foreach (DataRow row in table.Rows)
            {
                dtTemplates.Rows.Add(row.ItemArray).AcceptChanges();
            }

            expandedRowsId.Add(id);
        }

        protected override GridColumnsStates OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = base.OnGetGridColumnsState(sender);

            GridColumnState state = new GridColumnState();
            state.ColumnName = cnSubjectDepended;
            state.ColumnCaption = "Субъектно зависимый";
            state.IsHiden = true;
            state.ColumnPosition = 27;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnIsNotScrollable;
            state.ColumnCaption = "Не скроллируемый";
            state.IsHiden = true;
            state.ColumnPosition = 28;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnMobileTemplateType;
            state.ColumnCaption = "Тип мобильного отчета";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            state.IsHiden = false;
            state.ColumnPosition = 29;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnIcon;
            state.ColumnCaption = "Иконка";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            state.IsHiden = false;
            state.ColumnPosition = 30;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnLastDeployDate;
            state.ColumnCaption = "Дата выкладывания на сервер";
            state.IsHiden = false;
            state.IsReadOnly = true;
            state.IsSystem = true;
            state.ColumnPosition = 31;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnForumDiscussionID;
            state.ColumnCaption = "ID дискуссии форума";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerPositive;
            state.IsHiden = false;
            state.IsReadOnly = false;
            state.ColumnPosition = 32;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnTerritoryRF;
            state.IsHiden = true;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnTerritoryRFName;
            state.ColumnCaption = "Тип территории";
            state.IsReference = true;
            state.IsHiden = false;
            state.IsReadOnly = true;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnThemeSection;
            state.IsHiden = true;
            states.Add(state.ColumnName, state);

            state = new GridColumnState();
            state.ColumnName = cnThemeSectionName;
            state.ColumnCaption = "Тематические разделы";
            state.IsReference = true;
            state.IsReadOnly = true;
            state.IsHiden = false;
            states.Add(state.ColumnName, state);

            return states;
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        private Image ByteArrayToBitmap(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Bitmap.FromStream(ms);
            return returnImage;
        }

        private string GetImagePath()
        {
            string result = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Выберите изображение для данного элемента";
            dialog.Filter = "PNG (*.PNG)|*.PNG";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!dialog.CheckFileExists)
                {
                    DialogResult dialogResult = MessageBox.Show("Указан несуществующий файл", "Предупреждение",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Retry)
                        result = this.GetImagePath();   
                }
                result = dialog.FileName;
            }
            return result;
        }
        
        /// <summary>
        /// Сохраняем в базе данных документ с настройками ячейки
        /// </summary>
        /// <param name="cell"></param>
        private void SaveCellDocument(UltraGridCell cell)
        {
            // получаем дескриптор
            string xmlDescriptor = string.Empty;
            if (cell.Row.Cells[TemplateFields.Document].Value != DBNull.Value)
                xmlDescriptor = Encoding.UTF8.GetString((byte[])(cell.Row.Cells[TemplateFields.Document].Value));
            IPhoteTemplateDescriptor descriptor = XmlHelper.XmlStr2Obj<IPhoteTemplateDescriptor>(xmlDescriptor);

            //субъектно зависимость
            object cellValue = cell.Row.Cells[cnSubjectDepended].Value;
            descriptor.SubjectDepended = (cellValue is DBNull) ? false : Convert.ToBoolean(cell.Row.Cells[cnSubjectDepended].Value);
            //не скроллируемость
            cellValue = cell.Row.Cells[cnIsNotScrollable].Value;
            descriptor.IsNotScrollable = (cellValue is DBNull) ? false : Convert.ToBoolean(cell.Row.Cells[cnIsNotScrollable].Value);
            //тип мобильного отчета
            descriptor.TemplateType = (MobileTemplateTypes)cell.Row.Cells[cnMobileTemplateType].Value;
            descriptor.LastDeployDate = (DateTime) cell.Row.Cells[cnLastDeployDate].Value;
            //массив байт иконки
            cellValue = cell.Row.Cells[cnIcon].Value;
            descriptor.IconByte = (cellValue is DBNull) ? null : (byte[])cell.Row.Cells[cnIcon].Value;
            descriptor.ForumDiscussionID = (int)cell.Row.Cells[cnForumDiscussionID].Value;
            descriptor.TerritoryRF = (cell.Row.Cells[cnTerritoryRF].Value is DBNull)
                ? String.Empty
                : (string)cell.Row.Cells[cnTerritoryRF].Value;
            descriptor.ThemeSection = (cell.Row.Cells[cnThemeSection].Value is DBNull)
                ? String.Empty
                : (string)cell.Row.Cells[cnThemeSection].Value;

            // сохраняем значение дескриптора
            xmlDescriptor = XmlHelper.Obj2XmlStr(descriptor);

            cell.Row.Cells[TemplateFields.Document].Value = Encoding.UTF8.GetBytes(xmlDescriptor);
        }

        void ugeTemplates_OnClickCellButton(object sender, CellEventArgs e)
        {
            const string bTerritoryRFObjectKey = "3d6959a5-3ce6-439d-a5f1-b17aeeedfa37";
            const string bterritoryRFColumnName = "Code";

            const string fx_FX_ThemSectionObjectKey = "2f688506-bd3d-4761-af00-2c8ed283071c";
            const string fx_FX_ThemSectionColumnName = "Code";

            switch (e.Cell.Column.Key)
            {
                case cnTerritoryRFName:
                    {
                        OnCellClicked(e, bterritoryRFColumnName, bTerritoryRFObjectKey, cnTerritoryRF, true);
                        break;
                    }
                case cnThemeSectionName:
                    {
                        OnCellClicked(e, fx_FX_ThemSectionColumnName, fx_FX_ThemSectionObjectKey, cnThemeSection, false);
                        break;
                    }
                default:
                    {
                        string bitmapFileName = this.GetImagePath();
                        if (bitmapFileName != string.Empty)
                        {
                            Bitmap bitMap = new Bitmap(bitmapFileName);
                            e.Cell.Value = this.BitmapToByteArray(bitMap);
                        }
                        break;
                    }
            }
        }

        private void OnCellClicked(CellEventArgs e, string codeColumnName, string viewColumnObjectKey, string realColumnName, bool isTerritory)
        {
            HashSet<int> codesTerritory = new HashSet<int>();

            frmModalTemplate form = null;
            UltraGrid ultraGrid;

            IEntity territoryRFEntity =
                    Workplace.ActiveScheme.RootPackage.FindEntityByName(
                        viewColumnObjectKey);

            if (territoryRFEntity == null)
            {
                return;
            }

            codesTerritory.Clear();
            if (!String.IsNullOrEmpty(e.Cell.Row.Cells[realColumnName].Value.ToString()))
            {
                string[] codes = e.Cell.Row.Cells[realColumnName].Value.ToString().Split(';');
                foreach (string code in codes)
                {
                    codesTerritory.Add(Convert.ToInt32(code));
                }
            }

            DataClsUI territoryCls = new SelectRowsClsUI(territoryRFEntity, codesTerritory, codeColumnName);
            // получаем нужный классификатор
            // создаем объект просмотра классификаторов нужного типа
            territoryCls.Workplace = Workplace;
            territoryCls.RestoreDataSet = false;
            territoryCls.Initialize();
            territoryCls.InitModalCls(-1);
            // создаем форму
            form = new frmModalTemplate();
            form.AttachCls(territoryCls);
            territoryCls.RefreshAttachedData();
            ComponentCustomizer.CustomizeInfragisticsControls(form);
            ultraGrid = territoryCls.UltraGridExComponent.ugData;

            if (form.ShowDialog((Form) Workplace) == DialogResult.OK)
            {
                if (ultraGrid.ActiveRow != null)
                    ultraGrid.ActiveRow.Update();

                HashSet<int> hashSet = ((SelectRowsClsUI) territoryCls).Codes;
                string codes = null;
                foreach (int code in hashSet)
                {
                    codes += String.Format("{0};", code);
                }

                if(!String.IsNullOrEmpty(codes))
                {
                    codes = codes.Remove(codes.Length - 1, 1);
                }

                if (!e.Cell.Row.Cells[realColumnName].Value.Equals(codes))
                {
                    e.Cell.Row.Cells[realColumnName].Value = codes;
                    if (isTerritory)
                        e.Cell.Row.Cells[cnTerritoryRFName].Value = GetTerritoriesNames(codes);
                    else
                    {
                        e.Cell.Row.Cells[cnThemeSectionName].Value = GetTemeSectionNames(codes);
                    }
                }
            }
        }

        void ugeTemplates_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if ((e.Row.Cells[cnIcon].Value != DBNull.Value) && (e.Row.Cells[cnIcon].Value != null))
            {
                byte[] arrayByte = (byte[])e.Row.Cells[cnIcon].Value;   
                e.Row.Cells[cnIcon].Appearance.Image = this.ByteArrayToBitmap(arrayByte);
            }
        }

        void ugeTemplates_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;

            ValueList list;
            if (!ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists.Exists(cnMobileTemplateType))
            {
                list = ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists.Add(cnMobileTemplateType);
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "None";
                item.DataValue = 0;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "iPad";
                item.DataValue = 1;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "iPhone";
                item.DataValue = 2;

                item = list.ValueListItems.Add("item3");
                item.DisplayText = "WM240x320";
                item.DataValue = 3;

                item = list.ValueListItems.Add("item4");
                item.DisplayText = "WM480x640";
                item.DataValue = 4;
            }
            else
                list = ViewObject.ugeTemplates.ugData.DisplayLayout.ValueLists[cnMobileTemplateType];

            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                band.Columns[cnMobileTemplateType].ValueList = list;
                band.Columns[cnMobileTemplateType].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            }
        }

        protected override void OnAfterCellUpdate(object sender, CellEventArgs e)
        {
            Trace.TraceVerbose("OnAfterCellUpdate: {0} = {1}", e.Cell.Column.Key, e.Cell.Value);

            base.OnAfterCellUpdate(sender, e);

            if ((e.Cell.Column.Key.ToUpper() == cnSubjectDepended.ToUpper()) ||
                (e.Cell.Column.Key.ToUpper() == cnIsNotScrollable.ToUpper()) ||
                (e.Cell.Column.Key.ToUpper() == cnMobileTemplateType.ToUpper()) || 
                (e.Cell.Column.Key.ToUpper() == cnIcon.ToUpper()) ||
                (e.Cell.Column.Key.ToUpper() == cnForumDiscussionID.ToUpper()) ||
                (e.Cell.Column.Key.ToUpper() == cnTerritoryRFName.ToUpper()) ||
                (e.Cell.Column.Key.ToUpper() == cnThemeSectionName.ToUpper()))
            {
                this.SaveCellDocument(e.Cell);
            }
        }

        private object GetTemeSectionNames(string themeSection)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(themeSection))
            {
                string themes = themeSection;
                if (!String.IsNullOrEmpty(themes))
                {
                    string[] themesArray = themes.Split(';');
                    foreach (string s in themesArray)
                    {
                        name += string.Format("{0};", Repository.GetThemeSectionName(Convert.ToInt32(s)));
                    }
                }
            }

            return name;
        }

        private string GetTerritoriesNames(string territoryRf)
        {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(territoryRf))
            {
                string territories = territoryRf;
                if (!String.IsNullOrEmpty(territories))
                {
                    string[] territoriesArray = territories.Split(';');
                    foreach (string s in territoriesArray)
                    {
                        name += string.Format("{0};", Repository.GetTerritoryName(Convert.ToInt32(s)));
                    }
                }
            }

            return name;
        }
    }
}
