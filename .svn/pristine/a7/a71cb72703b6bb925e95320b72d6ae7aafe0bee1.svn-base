using System;
using System.Collections.Generic;
using Infragistics.Win;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Infragistics.Win.UltraWinToolbars;
using System.Xml;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Margins = System.Drawing.Printing.Margins;

namespace Krista.FM.Client.MDXExpert
{
    class ReportElementToolbar
    {
        #region Ключи
        //вкладок
        //вкладка cвойств элемента отчета
        public const string elementTabKey = "ElementTab";

        //групп
        //вид элемента
        public const string appearGroupKey = "AppearGroup";

        //инструментов
        public const string captionGalleryKey = "ElementCaptionGallery";
        public const string commentGalleryKey = "ElementCommentGallery";
        public const string captionVisibleKey = "ElementCaptionVisible";
        public const string commentVisibleKey = "ElementCommentVisible";
        public const string directMDXKey = "DirectMDX";
        public const string printElementKey = "PrintChart";
        public const string copyImageToClipboardKey = "CopyImageToClipboard";
        public const string saveImageToFileKey = "SaveImageToFile";
        public const string saveMapImageToFileKey = "SaveMapImageToFile";
        public const string exportElementToExcelKey = "ExportElementToExcel";
        public const string commnetPlaceKey = "ElementCommentPlace";
        public const string activeElementKey = "ActiveElement";
        public const string showTotalsKey = "ShowTotals";
        public const string hideTotalsKey = "HideTotals";

        #endregion

        #region контролы элемента отчета
        //вкладки
        /// <summary>
        /// Вкладка со свойствами элемента
        /// </summary>
        private RibbonTab elementTab;

        //группы
        /// <summary>
        /// Вид
        /// </summary>
        private RibbonGroup appearGroup;

        /// <summary>
        /// Показывать заголовок элемента отчета
        /// </summary>
        private StateButtonTool captionVisible;
        /// <summary>
        /// Показывать комментарий элемента отчета
        /// </summary>
        private StateButtonTool commentVisible;
        /// <summary>
        /// Стили заголовка элемента отчета
        /// </summary>
        private PopupGalleryTool captionGallery;
        /// <summary>
        /// Стили комментария элемента отчета
        /// </summary>
        private PopupGalleryTool commentGallery;
        /// <summary>
        /// Ввести MDX
        /// </summary>
        private ButtonTool directMDX;
        /// <summary>
        /// Печать элемента отчета
        /// </summary>
        private ButtonTool printElement;
        /// <summary>
        /// Копировать изображение элемента в буфер
        /// </summary>
        private ButtonTool copyImageToClipboard;
        /// <summary>
        /// Сохранить изображение элемента в файл
        /// </summary>
        private ButtonTool saveImageToFile;
        /// <summary>
        /// Сохранить полное изображение карты в файл
        /// </summary>
        private ButtonTool saveMapImageToFile;
        /// <summary>
        /// Экспорт в excel активного элемента отчета
        /// </summary>
        private ButtonTool exportElementToExcel;
        /// <summary>
        /// Расположение комментария элемента отчета
        /// </summary>
        private ComboBoxTool commentPlace;
        /// <summary>
        /// Активный элемент отчета
        /// </summary>
        private ComboBoxTool activeElementTool;
        /// <summary>
        /// Показать все итоги
        /// </summary>
        private ButtonTool showTotals;
        /// <summary>
        /// Скрыть все итоги
        /// </summary>
        private ButtonTool hideTotals;

        #endregion

        private CustomReportElement _activeElement;
        private ToolbarsManage _toolbarsManager;
        //стиль заголовка/комментария до редактирования
        private CellStyle befoEditStyle = null;
        //Если true - то события контролов не выполняются 
        private bool isMayHook = false;
        /// <summary>
        /// Настройки печати
        /// </summary>
        private PageSettings pageSettings;

        public ReportElementToolbar(ToolbarsManage toolbarManager)
        {
            this.ToolbarsManager = toolbarManager;

            //Инициализируем настройки печатной страницы
            this.pageSettings = new PageSettings();
            this.pageSettings.Margins = new Margins(40, 40, 40, 40);
        }

        public void Initialize()
        {
            //Устанавливаем ссылки на контролы
            this.SetLinkOnTools();
            //Устанавливаем заголовкам вкладок русские имена
            this.SetRusTabCaption();
            //Устанавливаем контролам таблицы, требуемые обработчики
            this.SetEventHandlers();
        }

        /// <summary>
        /// Устанавливаем ссылки на контролы
        /// </summary>
        private void SetLinkOnTools()
        {
            //Вкладки
            //Cвойства элемента
            this.elementTab = this.Toolbars.Ribbon.Tabs[elementTabKey];

            //Группы 
            this.appearGroup = this.elementTab.Groups[appearGroupKey];

            //Инструменты
            this.captionGallery = (PopupGalleryTool)this.Toolbars.Tools[captionGalleryKey];
            this.commentGallery = (PopupGalleryTool)this.Toolbars.Tools[commentGalleryKey];
            this.captionVisible = (StateButtonTool)this.Toolbars.Tools[captionVisibleKey];
            this.commentVisible = (StateButtonTool)this.Toolbars.Tools[commentVisibleKey];
            this.commentPlace = (ComboBoxTool)this.Toolbars.Tools[commnetPlaceKey];
            this.activeElementTool = (ComboBoxTool)this.Toolbars.Tools[activeElementKey];
            this.directMDX = (ButtonTool)this.Toolbars.Tools[directMDXKey];
            this.printElement = (ButtonTool)this.Toolbars.Tools[printElementKey];
            this.copyImageToClipboard = (ButtonTool)this.Toolbars.Tools[copyImageToClipboardKey];
            this.saveImageToFile = (ButtonTool)this.Toolbars.Tools[saveImageToFileKey];
            this.saveMapImageToFile = (ButtonTool)this.Toolbars.Tools[saveMapImageToFileKey];
            this.exportElementToExcel = (ButtonTool)this.Toolbars.Tools[exportElementToExcelKey];
            this.showTotals = (ButtonTool)this.Toolbars.Tools[showTotalsKey];
            this.hideTotals = (ButtonTool)this.Toolbars.Tools[hideTotalsKey];
        }

        /// <summary>
        /// Установить обработчики событий
        /// </summary>
        private void SetEventHandlers()
        {
            this.captionGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(captionGallery_GalleryToolActiveItemChange);
            this.captionGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(captionGallery_GalleryToolItemClick);

            this.commentGallery.GalleryToolActiveItemChange += new GalleryToolItemEventHandler(commentGallery_GalleryToolActiveItemChange);
            this.commentGallery.GalleryToolItemClick += new GalleryToolItemEventHandler(commentGallery_GalleryToolItemClick);

            this.captionVisible.ToolClick += new ToolClickEventHandler(captionVisible_ToolClick);
            this.commentVisible.ToolClick += new ToolClickEventHandler(commentVisible_ToolClick);
            this.commentPlace.ToolValueChanged += new ToolEventHandler(commentPlace_ToolValueChanged);
            this.activeElementTool.ToolValueChanged += new ToolEventHandler(activeElement_ToolValueChanged);

            this.directMDX.ToolClick += new ToolClickEventHandler(directMDX_ToolClick);
            this.printElement.ToolClick += new ToolClickEventHandler(printElement_ToolClick);
            this.copyImageToClipboard.ToolClick += new ToolClickEventHandler(copyImageToClipboard_ToolClick);
            this.saveImageToFile.ToolClick += new ToolClickEventHandler(saveImageToFile_ToolClick);
            this.saveMapImageToFile.ToolClick += new ToolClickEventHandler(saveMapImageToFile_ToolClick);
            this.exportElementToExcel.ToolClick += new ToolClickEventHandler(exportElementToExcel_ToolClick);
            this.showTotals.ToolClick += new ToolClickEventHandler(showTotals_ToolClick);
            this.hideTotals.ToolClick += new ToolClickEventHandler(hideTotals_ToolClick);
        }


        /// <summary>
        /// Инициализация списка элементов отчета
        /// </summary>
        public void InitReportElementList()
        {
            this.isMayHook = true;
            this.activeElementTool.ValueList.ValueListItems.Clear(); 
            List<CustomReportElement> elements = this.MainForm.GetReportElementList();
            foreach(CustomReportElement elem in elements)
            {
                string elemCaption = (elem.PivotData.CubeName != "")
                                         ? String.Format("{0} ({1})", elem.Title, elem.PivotData.CubeName)
                                         : elem.Title;

                ValueListItem item = this.activeElementTool.ValueList.ValueListItems.Add(elem, elemCaption);
                switch(elem.ElementType)
                {
                    case ReportElementType.eChart:
                        item.Appearance.Image = 37;
                        break;
                    case ReportElementType.eTable:
                        item.Appearance.Image = 38;
                        break;
                    case ReportElementType.eMap:
                        item.Appearance.Image = 50;
                        break;
                    case ReportElementType.eGauge:
                        item.Appearance.Image = 62;
                        break;
                    case ReportElementType.eMultiGauge:
                        item.Appearance.Image = 62;
                        break;
                }
            }

            if (this.ActiveElement != null)
            {
                for (int i = 0; i < this.activeElementTool.ValueList.ValueListItems.Count; i++ )
                {
                    if (this.ActiveElement.UniqueName == ((CustomReportElement)this.activeElementTool.ValueList.ValueListItems[i].DataValue).UniqueName)
                    {
                        this.activeElementTool.SelectedIndex = i;
                        break;
                    }
                }

            }
            this.isMayHook = false;
        }

        /// <summary>
        /// Обновляем значения контролов в соотвествии с данными активной таблицы
        /// </summary>
        /// <param name="activeElement"></param>
        public void RefreshTabsTools(CustomReportElement activeElement)
        {
            this.ActiveElement = activeElement;
            this.InitReportElementList();

            if ((this.ActiveElement != null) && (this.ActiveElement.PivotData != null))
            {
                try
                {
                    this.isMayHook = true;
                    this.RefreshPropertiesTab();
                    this.SetControlsState();
                }
                finally
                {
                    this.isMayHook = false;
                }
            }
        }

        /// <summary>
        /// Выставляем сосояние контролам
        /// </summary>
        private void SetControlsState()
        {
            //Метод печати досутпен для диаграмм и карты
            this.printElement.SharedProps.Enabled = this.MainForm.IsExistsActiveChart || this.MainForm.IsExistsActiveMap;
            this.saveMapImageToFile.SharedProps.Visible = this.MainForm.IsExistsActiveMap;
        }

        private void RefreshPropertiesTab()
        {
            this.captionVisible.Checked = this.ActiveElement.Caption.Visible;
            this.commentVisible.Checked = this.ActiveElement.Comment.Visible;
            this.commentPlace.SelectedIndex = (int)this.ActiveElement.Comment.Place;
        }

        /// <summary>
        /// Инициализирует панель инструментов используя данные сохранненые в XML
        /// </summary>
        public void Load(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return;
            try
            {
                ToolbarUtils.LoadStyleGallery(this.captionGallery, xmlNode.SelectSingleNode(ToolbarUtils.elementCaptionStylesNodeName));
                ToolbarUtils.LoadStyleGallery(this.commentGallery, xmlNode.SelectSingleNode(ToolbarUtils.elementCommentStylesNodeName));
            }
            catch
            {
                FormException.ShowErrorForm(new Exception("При загрузке данных в панель инструментов элемента отчета, произошла ошибка"));
            }
        }

        /// <summary>
        /// Устанавливаем заголовкам вкладок русские имена, непонятно почему, но они очень 
        /// часто меняются на ключи
        /// </summary>
        private void SetRusTabCaption()
        {
            this.elementTab.Caption = "Элемент";
        }

        /// <summary>
        /// Должно вызываться после изменения любых свойств элемента отчета
        /// </summary>
        private void AfterEdited()
        {
            this.MainForm.Saved = false;
            this.MainForm.PropertyGrid.Refresh();
        }

        #region Обработчики событий
        void captionGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.CaptionGalleryItemChange(e.Item);
        }

        void captionGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.befoEditStyle = null;
            this.AfterEdited();
        }

        void commentGallery_GalleryToolActiveItemChange(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.CommentGalleryItemChange(e.Item);
        }

        void commentGallery_GalleryToolItemClick(object sender, GalleryToolItemEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.befoEditStyle = null;
            this.AfterEdited();
        }

        void captionVisible_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.ActiveElement.Caption.Visible = this.captionVisible.Checked;
            this.AfterEdited();
        }

        void commentVisible_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.ActiveElement.Comment.Visible = this.commentVisible.Checked;
            this.AfterEdited();
        }

        void commentPlace_ToolValueChanged(object sender, ToolEventArgs e)
        {
            CommentPlace place = (CommentPlace)this.commentPlace.SelectedIndex;
            if (place != this.ActiveElement.Comment.Place)
            {
                this.ActiveElement.Comment.Place = place;
                this.AfterEdited();
            }
        }

        void activeElement_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (this.isMayHook)
                return;
            if(this.activeElementTool.Value != null)
            {
                this.MainForm.RefreshUserInterface((CustomReportElement)this.activeElementTool.Value);
            }
        }


        void directMDX_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.MainForm.ShowMdxQueryForm();
        }

        void printElement_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            this.MainForm.PrintPreviewDialog.Document = this.ActiveElement.PrintDocumet;
            //загрузим настройки страницы
            this.MainForm.PrintPreviewDialog.Document.DefaultPageSettings = this.pageSettings;

            this.MainForm.PrintPreviewDialog.ShowDialog();

            //сохраним настройки страницы
            this.pageSettings = this.MainForm.PrintPreviewDialog.Document.DefaultPageSettings;
            this.MainForm.PrintPreviewDialog.Document.Dispose();
        }

        void copyImageToClipboard_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            //Копируем в буфер обмена изображение элемента
            Clipboard.SetImage(this.ActiveElement.GetBitmap());
        }

        void saveImageToFile_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            SaveFileDialog safeDialog = new SaveFileDialog();
            safeDialog.DefaultExt = ".jpg";
            safeDialog.Filter = "JPEG (*.jpg)|*.jpg |BMP (*.bmp)|*.bmp |PNG (*.png)|*.png |GIF (*.gif)|*.gif";
            safeDialog.Title = "Сохранить изображение элемента очета";
            safeDialog.FileName = this.ActiveElement.Title;

            if (safeDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap elementBitmap = this.ActiveElement.GetBitmap();

                System.Drawing.Imaging.ImageFormat imageFormat = this.GetImageFormat(safeDialog.FilterIndex);
                elementBitmap.Save(safeDialog.FileName, imageFormat);
                elementBitmap.Dispose();
            }
            safeDialog.Dispose();
        }

        private System.Drawing.Imaging.ImageFormat GetImageFormat(int filterIndex)
        {
            switch (filterIndex)
            {
                case 1: return System.Drawing.Imaging.ImageFormat.Jpeg;
                case 2: return System.Drawing.Imaging.ImageFormat.Bmp;
                case 3: return System.Drawing.Imaging.ImageFormat.Png;
                case 4: return System.Drawing.Imaging.ImageFormat.Gif;
            }
            return System.Drawing.Imaging.ImageFormat.Jpeg;
        }

        void saveMapImageToFile_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;

            SaveFileDialog safeDialog = new SaveFileDialog();
            safeDialog.DefaultExt = ".jpg";
            safeDialog.Filter = "JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|PNG (*.png)|*.png|TIFF (*.tiff)|*.tiff";
            safeDialog.Title = "Сохранить изображение элемента очета";
            safeDialog.FileName = this.ActiveElement.Title;

            if (safeDialog.ShowDialog() == DialogResult.OK)
            {
                if (this.ActiveElement.ElementType == ReportElementType.eMap)
                {
                    string imageFormat = "JPEG";

                    switch (safeDialog.FilterIndex)
                    {
                        case 0:
                            imageFormat = "JPEG";
                            break;
                        case 1:
                            imageFormat = "BMP";
                            break;
                        case 2:
                            imageFormat = "GIF";
                            break;
                        case 3:
                            imageFormat = "PNG";
                            break;
                        case 4:
                            imageFormat = "TIFF";
                            break;
                    }
                    
                    ((MapReportElement)this.ActiveElement).SaveFullMapAsImage(safeDialog.FileName, imageFormat);

                }
            }
            safeDialog.Dispose();
        }

        void exportElementToExcel_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            this.MainForm.ExportForm.ShowDialog(true);
        }

        private void SetTotalsVisible(bool isVisible)
        {
            if (this.ActiveElement != null)
            {
                if (this.ActiveElement.PivotData != null)
                {
                    bool isDeferDataUpdating = this.ActiveElement.PivotData.IsDeferDataUpdating;
                    this.ActiveElement.PivotData.IsDeferDataUpdating = true;

                    foreach (Axis axis in this.ActiveElement.PivotData.Axes)
                    {
                        if (axis is PivotAxis)
                        {
                            ((PivotAxis)axis).GrandTotalVisible = isVisible;
                            foreach (FieldSet fs in axis.FieldSets)
                            {
                                fs.IsVisibleTotals = isVisible;
                            }
                        }
                    }
                    this.ActiveElement.PivotData.IsDeferDataUpdating = isDeferDataUpdating;

                    this.ActiveElement.PivotData.DoDataChanged();
                    this.AfterEdited();
                }
            }
        }

        void showTotals_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            SetTotalsVisible(true);
        }

        void hideTotals_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (this.isMayHook)
                return;
            SetTotalsVisible(false);
        }


        #endregion 

        #region Вспомогательные методы для установки правильных значений контролам
        private void CaptionGalleryItemChange(GalleryToolItem item)
        {
            if (item != null)
            {
                //если активация элемента происходи впервые, запомним стиль 
                if (this.befoEditStyle == null)
                    this.befoEditStyle = this.ActiveElement.Caption.Style;
                //установим новый стиль
                this.ActiveElement.Caption.Style = (CellStyle)item.Tag;
            }
            else
            {
                //сюда попадаем при деактивации галереии стилей, если ничего не выбрали,
                //значит в befoEditStyle должен сохраниться стиль запомненый перед
                //началом выбора его и выставляем, если выбор все даки был сделан
                //(кликали по элементам) то ничего не происходит, т.к. это значение очищалось
                this.ActiveElement.Caption.Style = this.befoEditStyle;
                this.befoEditStyle = null;
            }
        }

        private void CommentGalleryItemChange(GalleryToolItem item)
        {
            if (item != null)
            {
                //если активация элемента происходи впервые, запомним стиль 
                if (this.befoEditStyle == null)
                    this.befoEditStyle = this.ActiveElement.Comment.Style;
                //установим новый стиль
                this.ActiveElement.Comment.Style = (CellStyle)item.Tag;
            }
            else
            {
                //сюда попадаем при деактивации галереии стилей, если ничего не выбрали,
                //значит в befoEditStyle должен сохраниться стиль запомненый перед
                //началом выбора его и выставляем, если выбор все даки был сделан
                //(кликали по элементам) то ничего не происходит, т.к. это значение очищалось
                this.ActiveElement.Comment.Style = this.befoEditStyle;
                this.befoEditStyle = null;
            }
        }
        #endregion

        #region Свойства
        public ToolbarsManage ToolbarsManager
        {
            get { return _toolbarsManager; }
            set { _toolbarsManager = value; }
        }

        /// <summary>
        /// Ссылка на главную форму
        /// </summary>
        public MainForm MainForm
        {
            get { return this.ToolbarsManager.MainForm; }
            set { this.ToolbarsManager.MainForm = value; }
        }

        /// <summary>
        /// Лента
        /// </summary>
        public UltraToolbarsManager Toolbars
        {
            get { return this.ToolbarsManager.Toolbars; }
            set { this.ToolbarsManager.Toolbars = value; }
        }

        /// <summary>
        /// Активный элемент отчета по которому инициализирован вкладку
        /// </summary>
        public CustomReportElement ActiveElement
        {
            get { return _activeElement; }
            set { _activeElement = value; }
        }

        /// <summary>
        /// Существует ли активный элемент
        /// </summary>
        private bool IsExistActiveElement
        {
            get { return this.ActiveElement != null; }
        }

        #endregion
    }
}
