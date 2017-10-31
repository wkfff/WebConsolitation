using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Dundas.Maps.WinControl;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Path=System.IO.Path;
using Dundas.Utilities;
using LegendItem = Dundas.Maps.WinControl.LegendItem;

namespace Krista.FM.Client.MDXExpert
{
    public class MapReportElement : CustomReportElement
    {
        #region Поля

        private static string mapRepositoryPath = "";
        private MapControl map;
        private PropertyGrid propertyGrid;

        private List<string> objectNames = new List<string>();
        private List<string> seriesNames = new List<string>();
        private List<string> valuesNames = new List<string>();
        private List<Symbol> symbols = new List<Symbol>();
        private List<string> colorIntervalNames = new List<string>();

        private CellSet cls;

        private MapSerieCollection mapSeries;
        private Shape selectedShape;
        private DataSet sourceDS;
        private string templateName;
        //private bool displayEmptyShapeNames = true;
        private bool canRefreshMapAppearance = true;

        private Legend draggedLegend;
        private bool canDragPanel;
        private bool canLegendHResize;
        private bool canLegendVResize;
        private bool canLegendDiagResize;
        private Point beginDragPoint;
        //текущий край/угол легенды со списком объектов, над которым находится мышка
        private LegendBorder curLegendBorder = LegendBorder.None;
        //признаки изменения расположения и размера легенды со списком объектов
        private bool isLegendLocationChanged = false;
        private bool isLegendSizeChanged = false;
        //можно создавать легенду со списком объектов или нет
        private bool canCreateLegend = true;

        //Запоминаем в поле координаты центра карты, для определения события перетаскивания карты
        private PointF mapCenter;
        private float mapZoom;

        private DataSourceType dataSourceType = DataSourceType.Cube;
        private ShapeCaptionType shapeCaptionType = ShapeCaptionType.Name;

        private MapSynchronization synchronization;

        private LegendContentCollection legendContents;

        #endregion

        #region Свойства

        public MapControl Map
        {
            get { return map; }
        }

        public DataSet SourceDS
        {
            get { return sourceDS; }
            set
            {
                sourceDS = value;
                SetMapDataSource(value);
            }
        }

        public MapSerieCollection Series
        {
            get { return mapSeries; }
            set { mapSeries = value; }
        }

        public string TemplateName
        {
            get
            {
                return templateName; //System.IO.Path.GetFileName(this.templateName); 
            }
            set
            {
                if (templateName == value)
                {
                    return;
                }
                templateName = value;


                LoadMapTemplate(MapRepositoryPath + value);

                if (!String.IsNullOrEmpty(value))
                {
                    Utils regUtils = new Utils(typeof(MapReportElement), true);
                    regUtils.SetKeyValue(Consts.mapTemplateNameRegKey, value);

                    PivotData.DoDataChanged();
                }
            }
        }

        public static string MapRepositoryPath
        {
            get { return mapRepositoryPath; }
            set
            {
                mapRepositoryPath = value;
                Utils regUtils = new Utils(typeof(MapReportElement), true);
                regUtils.SetKeyValue(Consts.mapRepositoryPathRegKey, value);
                //templateName = "";
            }
        }

        public PropertyGrid PropertyGrid
        {
            get { return propertyGrid; }
        }

        public override IGridUserInterface GridUserInterface
        {
            get { return null; }
        }

        public override bool IsShowErrorMessage
        {
            get { return (base.IsCubeNotFond); }
        }

        /*
        public bool DisplayEmptyShapeNames
        {
            get { return displayEmptyShapeNames; }
            set
            {
                displayEmptyShapeNames = value;
                RefreshDisplayEmptyShapeNames(value);
            }
        }*/

        public bool CanRefreshMapAppearance
        {
            get { return canRefreshMapAppearance; }
            set { canRefreshMapAppearance = value; }
        }

        public DataSourceType DataSourceType
        {
            get { return dataSourceType; }
            set { dataSourceType = value; }
        }

        public ShapeCaptionType ShapeCaptionType
        {
            get { return shapeCaptionType; }
            set
            {
                SetShapeCaptionsType(value);
                RefreshMapAppearance();
            }
        }

        public MapSynchronization Synchronization
        {
            get { return synchronization; }
            set { synchronization = value; }
        }

        /// <summary>
        /// Вид отображения данных в легенде
        /// </summary>
        public LegendContentCollection LegendContents
        {
            get { return this.legendContents; }
            set { this.legendContents = value; }
        }

        /// <summary>
        /// Имена цветовых интервалов
        /// </summary>
        public List<string> ColorIntervalNames
        {
            get { return this.colorIntervalNames; }
            set { this.colorIntervalNames = value; }
        }

        #endregion

        public MapReportElement(MainForm mainForm)
            : base(mainForm, ReportElementType.eMap)
        {
            this.InitMap();

            propertyGrid = mainForm.PropertyGrid;

            PivotData.ColumnAxis.Caption = "Серии";
            PivotData.RowAxis.Caption = "Объекты";
            PivotData.TotalAxis.Caption = "Показатели";

            ElementType = ReportElementType.eMap;

            PivotData.DataChanged += OnPivotDataChange;
            PivotData.AppearanceChanged += OnPivotDataAppearanceChange;

            /*
            this.PivotData.StructureChanged += new PivotDataEventHandler(PivotData_StructureChanged);
            */

            LoadRegSettings();

            SymbolSize.SymbolMaxSize = 50;
            SymbolSize.SymbolMinSize = 2;

            this.synchronization = new MapSynchronization(this);
        }

        private void InitMap()
        {
            if (this.map != null)
            {
                this.map.Dispose();
                this.map = null;
            }

            map = new MapControl();

            //Приостоновим отрисовку
            ElementPlace.SuspendLayout();
            ElementPlace.AutoScroll = true;

            map.Parent = ElementPlace;
            map.Dock = DockStyle.Fill;
            map.MinimumSize = new Size(10, 10);

            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            map.Viewport.EnablePanning = true;
            map.Viewport.OptimizeForPanning = true;
            map.PostPaint += map_PostPaint;
            map.MouseLeave += map_MouseLeave;
            map.MouseEnter += map_MouseEnter;
            map.MouseMove += new MouseEventHandler(map_MouseMove);
            map.MouseDown += new MouseEventHandler(map_MouseDown);
            map.MouseUp += new MouseEventHandler(map_MouseUp);
            map.MouseClick += new MouseEventHandler(map_MouseClick);
            map.MouseDoubleClick += new MouseEventHandler(map_MouseDoubleClick);
            map.AllRulesApplied += new EventHandler(map_AllRulesApplied);

            if (mapSeries != null)
            {
                mapSeries.Clear();
                mapSeries = null;
            }
            mapSeries = new MapSerieCollection(map.Shapes);
            mapSeries.Element = this;
            
            if (objectNames != null)
            {
                objectNames.Clear();
                objectNames = null;
            }
            objectNames = new List<string>();

            if (seriesNames != null)
            {
                seriesNames.Clear();
                seriesNames = null;
            }
            seriesNames = new List<string>();

            if (valuesNames != null)
            {
                valuesNames.Clear();
                valuesNames = null;
            }
            valuesNames = new List<string>();

            legendContents = new LegendContentCollection();
            
            //Востановим отрисовку
            ElementPlace.ResumeLayout();

        }

        /// <summary>
        /// Выбор шаблона карты
        /// </summary>
        /// <returns></returns>
        public string SelectTemplateName()
        {
            Utils regUtils = new Utils(typeof(MapReportElement), true);
            string repositoryPath = regUtils.GetKeyValue(Consts.mapRepositoryPathRegKey);
            string templateName = regUtils.GetKeyValue(Consts.mapTemplateNameRegKey);

            repositoryPath = repositoryPath != String.Empty ? repositoryPath : Application.StartupPath + "\\ESRIMaps";
            MapTemplateChooser mapChooser = new MapTemplateChooser(repositoryPath, templateName);

            if (mapChooser.ShowDialog() == DialogResult.OK)
            {
                MapRepositoryPath = mapChooser.RepositoryPath;
                templateName = mapChooser.TemplateName;
                this.TemplateName = templateName;
                LoadPreset();
            }
            else
            {
                templateName = String.Empty;
            }

            return templateName;
        }

        /// <summary>
        /// загрузка преднастройки шаблона
        /// </summary>
        public void LoadPreset()
        {
            string fileName = String.Format("{0}{1}\\{2}.xml", MapRepositoryPath, this.TemplateName, GetSettingsFileName());
            if (!File.Exists(fileName))
                return;

            FileStream stream = null;
            XmlNode presetNode = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));
                presetNode = (XmlNode)xmlFormat.Deserialize(stream);
                stream.Close();
            }
            catch (Exception e)
            {
                CommonUtils.ProcessException(e);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }



            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader strReader = new StringReader(reportElementPreset))
                {
                    this.map.Serializer.Content = SerializationContent.All;
                    this.map.Serializer.ResetWhenLoading = false;
                    this.map.Serializer.Load(strReader);
                }
            }
        }

        private string GetSettingsFileName()
        {
            int beginPos = this.TemplateName.LastIndexOf("\\");
            return this.TemplateName.Substring(beginPos + 1);
        }


        void map_AllRulesApplied(object sender, EventArgs e)
        {
            RefreshLegendContent();
        }

        void map_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Dundas.Maps.WinControl.HitTestResult result = this.Map.HitTest(e.X, e.Y);
            if (result != null && result.Object != null)
            {
                Shape shape = null;

                if (result.Object is Shape)
                {
                    shape = (Shape) result.Object;
                }
                else
                if (result.Object is Symbol)
                {
                    string shName = ((Symbol) result.Object).ParentShape;
                    shape = (Shape)this.Map.Shapes.GetByName(shName);
                }

                if (shape != null)
                {
                    ShapePropertiesForm spForm = new ShapePropertiesForm(this, shape);
                    spForm.ShowDialog();
                }
                

            }
        }

        void map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Dundas.Maps.WinControl.HitTestResult result = this.Map.HitTest(e.X, e.Y);
                if (result != null && result.Object != null)
                {
                    Shape shape = null;

                    if (result.Object is Shape)
                    {
                        shape = (Shape)result.Object;
                    }
                    else
                        if (result.Object is Symbol)
                        {
                            string shName = ((Symbol)result.Object).ParentShape;
                            shape = (Shape)this.Map.Shapes.GetByName(shName);
                        }

                    if (shape != null)
                    {
                        ShapePropertiesForm spForm = new ShapePropertiesForm(this, shape);
                        spForm.ShowDialog();
                    }

                }
            }
        }


        void map_MouseUp(object sender, MouseEventArgs e)
        {
            this.canDragPanel = false;
            this.canLegendHResize = false;
            this.canLegendVResize = false;
            this.canLegendDiagResize = false;
            this.draggedLegend = null;
            this.Map.Viewport.EnablePanning = true;

            this.curLegendBorder = LegendBorder.None;

            //иногда глючит отрисовка,
            this.Map.Viewport.Zoom --;
            this.Map.Viewport.Zoom ++;

            if (this.isLegendLocationChanged || this.isLegendSizeChanged)
                this.MainForm.Saved = false;

            this.isLegendLocationChanged = false;
            this.isLegendSizeChanged = false;

        }

        void map_MouseDown(object sender, MouseEventArgs e)
        {
            Legend objNumberLegend = (Legend)this.Map.Legends.GetByName("Список объектов");
            if (objNumberLegend == null)
                return;

            //int legendWidth = this.Map.Width * (int)objNumberLegend.Size.Width / 100;
            //int legendHeight = this.Map.Height * (int)objNumberLegend.Size.Height / 100;
            int legendWidth = (int)objNumberLegend.Size.Width;
            int legendHeight = (int)objNumberLegend.Size.Height;

            Rectangle legendRect = new Rectangle((int)objNumberLegend.Location.X + 20,
                                                 (int)objNumberLegend.Location.Y + 20,
                                                 legendWidth - 40, legendHeight - 40);


            if (legendRect.Contains(e.Location))
            {
                if (objNumberLegend.Dock == PanelDockStyle.None)
                    this.canDragPanel = true;
                this.draggedLegend = objNumberLegend;
                this.beginDragPoint = new Point(e.Location.X - (int)objNumberLegend.Location.X,
                                                e.Location.Y - (int)objNumberLegend.Location.Y);
            }

            Rectangle legendRectWithBorders = new Rectangle((int)objNumberLegend.Location.X,
                                                 (int)objNumberLegend.Location.Y,
                                                 legendWidth, legendHeight);

            if (legendRectWithBorders.Contains(e.Location))
            {
                this.Map.Viewport.EnablePanning = false;
            }

            this.curLegendBorder = GetCurrentLegendBorder(e.Location, objNumberLegend);
            this.isLegendSizeChanged = (this.curLegendBorder != LegendBorder.None);
        }


        private LegendBorder GetCurrentLegendBorder(Point curPoint, Legend legend)
        {
            if (legend.AutoSize)
                return LegendBorder.None;

            int borderSize = 15,
                left = (int) legend.Location.X,
                top = (int)legend.Location.Y,
                width = (int)legend.Size.Width,
                height = (int)legend.Size.Height;


            //обозначим края легенды
            Rectangle leftBorder = new Rectangle(left, top + borderSize, borderSize, height - borderSize * 2);
            Rectangle rightBorder = new Rectangle(left + width - borderSize, top + borderSize, borderSize, height - borderSize * 2);
            Rectangle topBorder = new Rectangle(left + borderSize, top, width - borderSize * 2, borderSize);
            Rectangle bottomBorder = new Rectangle(left + borderSize, top + height - borderSize, width - borderSize * 2, borderSize);

            Rectangle legendBRGripper = new Rectangle(left + width - borderSize, top + height - borderSize, borderSize, borderSize);
            Rectangle legendBLGripper = new Rectangle(left, top + height - borderSize, borderSize, borderSize);
            Rectangle legendTRGripper = new Rectangle(left + width - borderSize, top, borderSize, borderSize);
            Rectangle legendTLGripper = new Rectangle(left, top, borderSize, borderSize);

            switch (legend.Dock)
            {
                case PanelDockStyle.Top:
                    if (bottomBorder.Contains(curPoint))
                        return LegendBorder.Bottom;
                    break;
                case PanelDockStyle.Bottom:
                    if (topBorder.Contains(curPoint))
                        return LegendBorder.Top;
                    break;
                case PanelDockStyle.Left:
                    if (bottomBorder.Contains(curPoint))
                        return LegendBorder.Bottom;
                    if (rightBorder.Contains(curPoint))
                        return LegendBorder.Right;
                    if (legendBRGripper.Contains(curPoint))
                        return LegendBorder.BottomRight;
                    break;

                case PanelDockStyle.Right:
                    if (bottomBorder.Contains(curPoint))
                        return LegendBorder.Bottom;
                    if (leftBorder.Contains(curPoint))
                        return LegendBorder.Left;
                    if (legendBLGripper.Contains(curPoint))
                        return LegendBorder.BottomLeft;
                    break;
                case PanelDockStyle.None:
                    if (bottomBorder.Contains(curPoint))
                        return LegendBorder.Bottom;
                    if (topBorder.Contains(curPoint))
                        return LegendBorder.Top;
                    if (leftBorder.Contains(curPoint))
                        return LegendBorder.Left;
                    if (rightBorder.Contains(curPoint))
                        return LegendBorder.Right;
                    if (legendBRGripper.Contains(curPoint))
                        return LegendBorder.BottomRight;
                    if (legendBLGripper.Contains(curPoint))
                        return LegendBorder.BottomLeft;
                    if (legendTLGripper.Contains(curPoint))
                        return LegendBorder.TopLeft;
                    if (legendTRGripper.Contains(curPoint))
                        return LegendBorder.TopRight;
                    break;
            }

            return LegendBorder.None;
        }

        void map_MouseMove(object sender, MouseEventArgs e)
        {
            Legend objNumberLegend = (Legend)this.Map.Legends.GetByName("Список объектов");
            if (objNumberLegend == null)
                return;

            int legendWidth = (int)objNumberLegend.Size.Width;
            int legendHeight = (int)objNumberLegend.Size.Height;

            Rectangle legendRect = new Rectangle((int)objNumberLegend.Location.X,
                                     (int)objNumberLegend.Location.Y,
                                     legendWidth, legendHeight);

            this.Map.Cursor = Cursors.Default;

            switch (GetCurrentLegendBorder(e.Location, objNumberLegend))
            {
                case LegendBorder.Top:
                case LegendBorder.Bottom:
                    this.Map.Cursor = Cursors.SizeNS;
                    break;
                case LegendBorder.Left:
                case LegendBorder.Right:
                    this.Map.Cursor = Cursors.SizeWE;
                    break;
                case LegendBorder.TopLeft:
                case LegendBorder.BottomRight:
                    this.Map.Cursor = Cursors.SizeNWSE;
                    break;
                case LegendBorder.TopRight:
                case LegendBorder.BottomLeft:
                    this.Map.Cursor = Cursors.SizeNESW;
                    break;
            }

            switch(this.curLegendBorder)
            {
                case LegendBorder.Top:
                    objNumberLegend.Size.Height -= e.Y - objNumberLegend.Location.Y - 10;
                    objNumberLegend.Location.Y = e.Y - 10;
                    break;
                case LegendBorder.Bottom:
                    objNumberLegend.Size.Height = e.Y - objNumberLegend.Location.Y + 10;
                    break;
                case LegendBorder.Left:
                    objNumberLegend.Size.Width -= e.X - objNumberLegend.Location.X - 10;
                    objNumberLegend.Location.X = e.X - 10;
                    break;
                case LegendBorder.Right:
                    objNumberLegend.Size.Width = e.X - objNumberLegend.Location.X + 10;
                    break;
                case LegendBorder.TopLeft:
                    objNumberLegend.Size.Width -= e.X - objNumberLegend.Location.X - 10;
                    objNumberLegend.Size.Height -= e.Y - objNumberLegend.Location.Y - 10;
                    objNumberLegend.Location.X = e.X - 10;
                    objNumberLegend.Location.Y = e.Y - 10;
                    break;
                case LegendBorder.TopRight:
                    objNumberLegend.Size.Width = e.X - objNumberLegend.Location.X + 10;
                    objNumberLegend.Size.Height -= e.Y - objNumberLegend.Location.Y - 10;
                    objNumberLegend.Location.Y = e.Y - 10;
                    break;
                case LegendBorder.BottomLeft:
                    objNumberLegend.Size.Width -= e.X - objNumberLegend.Location.X - 10;
                    objNumberLegend.Size.Height = e.Y - objNumberLegend.Location.Y + 10;
                    objNumberLegend.Location.X = e.X - 10;
                    break;
                case LegendBorder.BottomRight:
                    objNumberLegend.Size.Width = e.X - objNumberLegend.Location.X + 10;
                    objNumberLegend.Size.Height = e.Y - objNumberLegend.Location.Y + 10;
                    break;

            }

            if (objNumberLegend.Size.Width < 75)
                objNumberLegend.Size.Width = 75;
            if (objNumberLegend.Size.Height < 50)
                objNumberLegend.Size.Height = 50;

            if ((this.canDragPanel) && (this.draggedLegend != null))
            {
                this.draggedLegend.Location.X = e.X - this.beginDragPoint.X;
                this.draggedLegend.Location.Y = e.Y - this.beginDragPoint.Y;
                this.isLegendLocationChanged = true;
            }
        }

        private void LoadRegSettings()
        {
            Utils regUtils = new Utils(GetType(), true);
            string repositoryPath = regUtils.GetKeyValue(Consts.mapRepositoryPathRegKey);
            //string templateName = regUtils.GetKeyValue(Consts.mapTemplateNameRegKey);

            MapRepositoryPath = repositoryPath != String.Empty ? repositoryPath : Application.StartupPath + "\\ESRIMaps";


            //this.SetTemplateNameWithoutRefresh(templateName);
        }

        /// <summary>
        /// Получение структуры из таблицы, если с ней синхронизирована карта
        /// </summary>
        /// <returns>true - если структура получена</returns>
        private bool GetSyncronizedPivotData(bool isForceDataUpdate)
        {
            if (this.Synchronization.BoundTo != String.Empty)
            {
                TableReportElement tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                    this.PivotData.IsDeferDataUpdating = true;
                    Synchronize(tableElement.PivotData, false, !isForceDataUpdate);
                    this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
                    return true;
                }

            }
            return false;
        }

        public void OnPivotDataChange()
        {
            GetSyncronizedPivotData(false);
            //Получаем данные с сервера
            RefreshData();
        }

        protected override void RefreshData()
        {
            if (this.DataSourceType == DataSourceType.Cube)
            {
                base.RefreshData();
            }
        }

        private void OnPivotDataAppearanceChange(bool isNeedRecalculateGrid)
        {
            if (this.DataSourceType == DataSourceType.Cube)
                this.InitialByCellSet();
        }

        void map_MouseEnter(object sender, EventArgs e)
        {
            //запомним коэфициент зума
            this.mapZoom = this.map.Viewport.Zoom;
            //запоминим координаты центра карты
            this.mapCenter = this.map.Viewport.ViewCenter.ToPoint();
        }

        void map_MouseLeave(object sender, EventArgs e)
        {
            if (this.IsChangedMapZoom() || this.IsChangedMapCenter())
            {
                this.MainForm.Saved = false;
                this.mapZoom = float.Epsilon;
                this.mapCenter = PointF.Empty;
            }
        }

        private bool IsChangedMapZoom()
        {
            return (this.mapZoom != float.Epsilon) && (this.mapZoom != this.map.Viewport.Zoom);
        }

        private bool IsChangedMapCenter()
        {
            return !this.mapCenter.IsEmpty &&
                (!this.mapCenter.Equals(this.map.Viewport.ViewCenter.ToPoint()));
        }

        private void ClearSelectedShape()
        {
            if (selectedShape != null)
            {
                selectedShape.Selected = false;
            }
        }

        public override Bitmap GetPrintableImage(Rectangle pageBounds)
        {
            return (Map.Dock == DockStyle.Fill) ? GetBitmap(pageBounds) : GetBitmap();
        }

        public void SaveFullMapAsImage(string imagePath, string imageFormat)
        {
            ElementPlace.Update();

            MainForm.Operation.StartOperation();
            MainForm.Operation.Text = "Сохранение изображения карты...";
            ElementPlace.SuspendLayout();
            Map.Dock = DockStyle.None;

            float currentZoom = Map.Viewport.Zoom;
            float viewCenterX = Map.Viewport.ViewCenter.X;
            float viewCenterY = Map.Viewport.ViewCenter.Y;

            float newWidth = (Size.Width) * (currentZoom) / 100;
            float newHeight = (Size.Height) * (currentZoom)/ 100;

            Map.Width = (int) newWidth;
            Map.Height = (int) newHeight;
            Map.Viewport.Zoom = 95;
            Map.Viewport.ViewCenter.X = 50;
            Map.Viewport.ViewCenter.Y = 50;

            /*
            MemoryStream stream = new MemoryStream();
            this.map.SaveAsImage(stream, MapImageFormat.Bmp);
            Bitmap bm = new Bitmap(stream);*/

            MapImageFormat format = MapImageFormat.Jpeg;
            switch (imageFormat)
            {
                case "JPEG":
                    format = MapImageFormat.Jpeg;
                    break;
                case "BMP":
                    format = MapImageFormat.Bmp;
                    break;
                case "GIF":
                    format = MapImageFormat.Gif;
                    break;
                case "PNG":
                    format = MapImageFormat.Png;
                    break;
                case "TIFF":
                    format = MapImageFormat.Tiff;
                    break;
            }


            map.SaveAsImage(imagePath, format);

            Map.Viewport.Zoom = currentZoom;
            Map.Viewport.ViewCenter.X = viewCenterX;
            Map.Viewport.ViewCenter.Y = viewCenterY;
            Map.Dock = DockStyle.Fill;
            ElementPlace.ResumeLayout();
            MainForm.Operation.StopOperation();
        }

        public override Bitmap GetBitmap()
        {
            Rectangle fullElementBounds = ClientRectangle;

            fullElementBounds.Width -= ElementPlace.Width;
            fullElementBounds.Height -= ElementPlace.Height;

            fullElementBounds.Width += Map.Size.Width;
            fullElementBounds.Height += Map.Size.Height;

            fullElementBounds.Width = Math.Max(ClientRectangle.Width, fullElementBounds.Width);
            fullElementBounds.Height = Math.Max(ClientRectangle.Height, fullElementBounds.Height);


            return base.GetBitmap(fullElementBounds);
        }

        public override void SetElementVisible(bool value)
        {
            if (map.Visible != value)
            {
                map.Visible = value;
                Application.DoEvents();
            }
        }

        public void LoadMapTemplate(string mapDirectory)
        {
            map.Layers.Clear();
            map.Groups.Clear();
            map.Shapes.Clear();
            if (!Directory.Exists(mapDirectory))
            {
                FormException.ShowErrorForm(new Exception("Шаблон \"" + mapDirectory + "\" не найден."),
                                            ErrorFormButtons.WithoutTerminate);
                return;
            }
            
            string[] files = Directory.GetFiles(mapDirectory, "*.emt", SearchOption.TopDirectoryOnly);

           // DefineLayerOrder(ref files);

            if (files.Length > 0)
            {
                //сохраним правила отображения
                ShapeRule shRule = null;
                List<SymbolRule> symRules = new List<SymbolRule>();

                if (this.Map.ShapeRules.Count > 0)
                    shRule = this.Map.ShapeRules[0];
                foreach (SymbolRule symRule in this.Map.SymbolRules)
                {
                    symRules.Add(symRule);
                }

                this.map.Serializer.Reset();
                this.map.Serializer.SerializableContent = "*.*";
                this.map.Serializer.Content = SerializationContent.All;
                this.map.Serializer.Load(files[0]);

                //после смены шаблона восстановим правила отображения
                if (shRule != null)
                {
                    this.Map.ShapeRules.Clear();
                    this.Map.ShapeRules.Add(shRule);
                }
                this.Map.SymbolRules.Clear();
                foreach (SymbolRule symRule in symRules)
                {
                    this.Map.SymbolRules.Add(symRule);
                }
            }

            ClearNullShapeNames();


            int i = 0;
            while (i < map.Layers.Count)
            {
                Layer layer = map.Layers[i];
                layer.Tag = true;
                if (!LayerHasShapes(layer))
                {
                    map.Layers.Remove(layer);
                }
                else
                {
                    i++;
                }
            }

            RefreshObjectNamesInCustomDS();

            Legend legend = (Legend)this.Map.Legends.GetByName(Consts.objectList);
            if (legend != null)
                this.Map.Legends.Remove(legend);


            CheckObjCodeDuplicates();
             
        }

        //если имеются объекты с одинаковыми кодами - выдаем предупреждение
        private void CheckObjCodeDuplicates()
        {
            List<int> codes = new List<int>();

            foreach (Shape sh in this.Map.Shapes)
            {
                string objCode = (string)sh["CODE"];
                if (String.IsNullOrEmpty(objCode))
                    continue;

                int code = 0;
                Int32.TryParse(objCode, out code);

                if (!codes.Contains(code))
                {
                    codes.Add(code);
                }
                else
                {
                    MessageBox.Show("Некоторые объекты имеют одинаковые коды. Для корректного отображения данных коды объектов в шаблоне должны быть уникальны.",
                        "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        /// <summary>
        /// установить очередь загрузки слоев
        /// </summary>
        /// <param name="layerFiles"></param>
        private void DefineLayerOrder(ref string[] layerFiles)
        {
            //загружаем слой с городами в последнюю очередь
            for (int k = 0; k < layerFiles.Length; k++)
            {
                string layerName = Path.GetFileNameWithoutExtension(layerFiles[k]);
                if (layerName.ToUpper() == "ГОРОДА")
                {
                    string lastLayerName = layerFiles[layerFiles.Length - 1];
                    layerFiles[layerFiles.Length - 1] = layerFiles[k];
                    layerFiles[k] = lastLayerName;
                    break;
                }
            }
        }

        /// <summary>
        /// обновляем имена объектов в пользовательских данных
        /// </summary>
        private void RefreshObjectNamesInCustomDS()
        {
            DataSet ds = this.SourceDS;
            if (ds == null)
                return;

            if (this.DataSourceType == MDXExpert.DataSourceType.Custom)
            {
                List<string[]> objectNames = this.GetObjectCodesWithNames();



                //удаляем лишние таблицы и столбцы
                foreach (DataTable table in ds.Tables)
                {
                    if (!table.Columns.Contains(Consts.objCodeColumn))
                    {
                        table.Columns.Add(Consts.objCodeColumn, typeof (string));
                        table.Columns[Consts.objCodeColumn].SetOrdinal(0);
                    }
                    if (!table.Columns.Contains(Consts.mapObjectShortName))
                    {
                        table.Columns.Add(Consts.mapObjectShortName, typeof (string));
                        table.Columns[Consts.mapObjectShortName].SetOrdinal(2);
                    }

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        bool objContains = false;
                        for (int k = 0; k < objectNames.Count; k++)
                        {
                            if (((table.Rows[i][Consts.objCodeColumn] == DBNull.Value) || 
                                ((string)table.Rows[i][Consts.objCodeColumn] == objectNames[k][0])) &&
                                    ((table.Rows[i][Consts.objectsColumn] != DBNull.Value) && ((string)table.Rows[i][Consts.objectsColumn] == objectNames[k][1])))
                            {
                                objContains = true;
                                if (table.Rows[i][Consts.objCodeColumn] == DBNull.Value)
                                    table.Rows[i][Consts.objCodeColumn] = objectNames[k][0];
                                break;
                            }
                        }
                        if (!objContains)
                        {
                            table.Rows.Remove(table.Rows[i]);
                            i--;
                        }
                    }

                    //добавляем новые, если есть
                    for (int j = 0; j < objectNames.Count; j++)
                    {
                        if (MapHelper.GetTableRow(table, objectNames[j][0], objectNames[j][1]) == null)
                        {
                            if (objectNames[j][0] == null)
                                continue;

                            table.Rows.Add();
                            table.Rows[table.Rows.Count - 1][Consts.objCodeColumn] = objectNames[j][0];
                            table.Rows[table.Rows.Count - 1][Consts.objectsColumn] = objectNames[j][1];
                            table.Rows[table.Rows.Count - 1][Consts.mapObjectShortName] = objectNames[j][2];
                        }
                    }

                }

            }
        }

        private void CheckShapes(List<Shape> shapes)
        {
            for (int i = 0; i < this.Map.Shapes.Count; i++)
            {
                Shape sh = GetShapeFromList(shapes, this.Map.Shapes[i]);
                if(sh == null)
                {
                    this.Map.Shapes.RemoveAt(i);
                    i--;
                }
                else
                {
                    this.Map.Shapes[i].Layer = sh.Layer;
                    this.Map.Shapes[i].ParentGroup = sh.ParentGroup;
                    this.Map.Shapes[i].FieldData = sh.FieldData;
                }
            }
        }

        private Shape GetShapeFromList(List<Shape> shapes, Shape item)
        {
            foreach (Shape sh in shapes)
            {
                if (sh.Name == item.Name)
                {

                    return sh;
                }
            }
            return null;
        }

        private bool CheckLayers(string mapDirectory)
        {
            bool result = true;

            if (!Directory.Exists(mapDirectory))
            {
                Map.Shapes.Clear();
                Map.Layers.Clear();
                Map.Groups.Clear();
                return false;
            }
            
            for (int i = 0; i < Map.Layers.Count; i++)
            {
                Layer layer = Map.Layers[i];
                string errMsg = "";
                string layerName = layer.Name;

                if (!File.Exists(string.Format("{0}\\{1}.shp", mapDirectory, layerName)))
                {
                    errMsg = string.Format("Слой \"{0}\" не найден. ", layerName);
                }

                if (!File.Exists(mapDirectory + "\\" + layerName + ".dbf"))
                {
                    errMsg += string.Format("Не найдены данные для слоя \"{0}\".", layerName);
                }

                if (errMsg != "")
                {
                    Map.Layers.Remove(layer);
                    Group group = (Group)Map.Groups.GetByName(layerName);
                    if (group != null)
                    {
                        Map.Groups.Remove(group);
                    }
                    FormException.ShowErrorForm(new Exception(errMsg),
                                                ErrorFormButtons.WithoutTerminate);
                    result = false;
                }
            }
            return result;
        }

        public void ClearNullShapeNames()
        {
            //текущее поле, из которого берем подпись для объекта(код или наименование) 
            string shapeCaptionField = GetShapeFieldCaption();

            foreach (Shape sh in map.Shapes)
            {
                if (sh[shapeCaptionField] == null)
                    sh.Text = "";
                /*
                if (sh.Name.Contains("Shape") || sh.Name.Contains("(no data)"))
                {
                    sh.Text = "";
                }*/
            }
        }

        /// <summary>
        /// возвращает имя поля, используемого для подписи объектов(код или наименование)
        /// </summary>
        /// <returns></returns>
        public string GetShapeFieldCaption()
        {
            switch(this.ShapeCaptionType)
            {
                case MDXExpert.ShapeCaptionType.Name:
                    return "NAME";
                case MDXExpert.ShapeCaptionType.ShortName:
                    return "SHORTNAME";
                case MDXExpert.ShapeCaptionType.Code:
                    return "CODE";
            }
            return "NAME";
        }

        protected override CellSet SetMDXQuery(string mdxQuery)
        {
            CellSet cls = null;
            
            if (this.DataSourceType == DataSourceType.Custom)
            {
                this.SetMapDataSource(this.SourceDS);
                return cls;
            }
            
            try
            {
                cls = base.SetMDXQuery(mdxQuery);
                InitialByCellSet(cls);
            }
            catch (Exception e)
            {
                map.DataSource = null;
                CommonUtils.ProcessException(e);
            }
            return cls;
        }

        public override void InitialByCellSet(CellSet cls)
        {
            this.cls = cls;
            DataSet ds = PopulateMapDataFromCellSet(cls);
            SourceDS = ds;

            RefreshMapAppearance();
        }

        public void RefreshMapAppearance()
        {
            if(!this.CanRefreshMapAppearance)
            {
                return;
            }
            AddRules(seriesNames, valuesNames, SourceDS);
            RefreshDisplayEmptyShapeNames();
        }

        private void SetMapDataSource(DataSet ds)
        {
            map.DataSource = ds;
        }

        private void RefreshDisplayEmptyShapeNames()
        {
            foreach(Layer layer in this.Map.Layers)
            {
                MapLayerBrowseClass layerBrowse = new MapLayerBrowseClass(layer, this);
                layerBrowse.DisplayEmptyShapeNames = (bool) layer.Tag;
            }
        }

        public bool IsNullShape(string shapeName)
        {
            if (SourceDS == null)
            {
                return true;                
            }

            foreach(DataTable dt in SourceDS.Tables)
            {
                DataRow row = MapHelper.GetTableRow(dt, shapeName);
                if (row != null)
                {
                    for (int i = 3; i < dt.Columns.Count; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void AddRules(List<string> seriesNames, List<string> valueNames, DataSet ds)
        {
            Map.SuspendLayout();

            List<SymbolRule> symbolRules = new List<SymbolRule>();
            List<MapSerie> newMapSeries = new List<MapSerie>();

            symbols.Clear();
            foreach(Symbol symbol in this.Map.Symbols)
            {
                symbols.Add(symbol);
            }

            Series.Items.Clear();

            int offset = 0;
            Map.Symbols.Clear();

            ShapeRule shapeRule = null;

            foreach (Shape sh in Map.Shapes)
            {
                sh.ParentGroup = sh.Layer;
            }

            foreach (string serieName in seriesNames)
            {
                MapSerie mapSerie = new MapSerie(Series, serieName);
                DataTable table = ds.Tables[serieName];
                foreach (string value in valueNames)
                {
                    AddRule(serieName, value, offset, mapSerie, table, ref shapeRule, ref symbolRules);
                }
                offset += 10;
                mapSerie.RefreshDataView(this. symbols);
                newMapSeries.Add(mapSerie);
            }
            
            Map.SymbolRules.Clear();
            foreach (SymbolRule rule in symbolRules)
            {
                rule.Name = "";
                Map.SymbolRules.Add(rule);
                Series.AddSymbolRule(rule);
            }


            Map.ShapeRules.Clear();
            if (shapeRule != null)
            {
                Map.ShapeRules.Add(shapeRule);
                Series.AddShapeRule(shapeRule);
            }

            Series.AddRange(newMapSeries);
            Series.InitRules();
            this.Series.SetProportionalSymbolSize();

            RemoveUnusedLegends();
            this.Map.ResumeLayout();

            this.Map.ApplyRules();
            

            //задаем размеры значков в соответствии с размерами предустановленных значков правил
            //сейчас, если не задавать эти размеры, значки накладываются друг на друга
            foreach (SymbolRule rule in Map.SymbolRules)
            {
                foreach (PredefinedSymbol prSymbol in rule.PredefinedSymbols)
                {
                    foreach (Symbol symbol in prSymbol.GetAffectedSymbols())
                    {
                        Symbol affectedSymbol = GetMapSymbol(symbol.ParentShape, symbol.Category, (string)symbol.Tag);
                        if (affectedSymbol != null)
                        {
                            affectedSymbol.Width =  prSymbol.Width;
                            affectedSymbol.Height = prSymbol.Height;
                        }
                    }
                }
            }

            //Если значок не входит ни в одно правило делаем его невидимым
            foreach (Symbol symbol in this.Map.Symbols)
            {
                symbol.Visible = IsRuleAffectedSymbol(symbol);
            }



            //восстанавливаем видимость секторных диаграмм
            foreach (MapSerie mapSerie in this.Series)
            {
                foreach (MapPieChartSymbol symbol in mapSerie.PieChartSymbols)
                {
                    symbol.Visible = true;
                }
            }

            AddObjectNumbersLegend();
            AddShapeRuleLegend();
            //RefreshLegendContent();
        }

        /// <summary>
        /// установка настроек по умолчанию для объектов, на которые действует заливка
        /// </summary>
        public void SetFillShapesByDefault()
        {
            //для того чтобы подпись и формат подписи объекта брались от заливки, 
            //у объекта по умолчанию подпись должна быть "#NAME";
            foreach (ShapeRule shapeRule in this.Map.ShapeRules)
            {
                switch(this.ShapeCaptionType)
                {
                    case ShapeCaptionType.Name:
                        shapeRule.Text = shapeRule.Text.Replace("#CODE", "#NAME");
                        shapeRule.Text = shapeRule.Text.Replace("#SHORTNAME", "#NAME");
                        break;
                    case ShapeCaptionType.ShortName:
                        shapeRule.Text = shapeRule.Text.Replace("#CODE", "#SHORTNAME");
                        shapeRule.Text = shapeRule.Text.Replace("#NAME", "#SHORTNAME");
                        break;
                    case ShapeCaptionType.Code:
                        shapeRule.Text = shapeRule.Text.Replace("#NAME", "#CODE");
                        shapeRule.Text = shapeRule.Text.Replace("#SHORTNAME", "#CODE");
                        break;
                }

                foreach (CustomColor color in shapeRule.CustomColors)
                {
                    color.Text = shapeRule.Text;
                    foreach (Shape sh in color.GetAffectedElements())
                    {
                        sh.Text = "#NAME";
                    }
                }
            }
        }

        private SymbolRule GetSymbolRule(string serieName, string measure)
        {
            foreach (SymbolRule rule in this.Map.SymbolRules)
            {
                if ((rule.SymbolField == MapHelper.CorrectFieldName(measure)) && (rule.Category == serieName))
                {
                    return rule;
                }
            }
            return null;
        }

        /// <summary>
        /// проверка, применяется ли к значку правило отображения(symbolRule)
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool IsRuleAffectedSymbol(Symbol symbol)
        {
            SymbolRule rule = GetSymbolRule(symbol.Category, (string)symbol.Tag);
            if (rule == null)
                return false;

            foreach (PredefinedSymbol prSymbol in rule.PredefinedSymbols)
            {
                foreach (Symbol affectedSymbol in prSymbol.GetAffectedSymbols())
                {
                    if (symbol.ParentShape == affectedSymbol.ParentShape)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление правила отображения данных
        /// </summary>
        /// <param name="serieName">имя серии</param>
        /// <param name="valueName">имя значения</param>
        /// <param name="offset">смещение значка</param>
        /// <param name="mapSerie">серия, для которой добавляется правило отображения</param>
        /// <param name="table">таблица с данными серии</param>
        /// <param name="shapeRule">правило заливки</param>
        /// <param name="symbolRules">правила для значков</param>
        private void AddRule(string serieName, string valueName, int offset, MapSerie mapSerie, DataTable table, ref ShapeRule shapeRule, ref List<SymbolRule> symbolRules)
        {
            string shapeRuleCategory = "";
            string shapeField = "";
            if (Map.ShapeRules.Count > 0)
            {
                shapeRuleCategory = Map.ShapeRules[0].Category;
                shapeField = Map.ShapeRules[0].ShapeField;
            }

            mapSerie.SetShowCharts(Series.PieChartSeriesContains(serieName));

            if ((serieName != shapeRuleCategory)||(MapHelper.CorrectFieldName(valueName) != shapeField))
            {

                if (!Series.PieChartSeriesContains(serieName))
                {
                    AddLegend(serieName, valueName);
                    //значки
                    symbolRules.Add(Series.GetSymbolRule(serieName, valueName));
                    mapSerie.AddSymbolSerieRule(GetSymbolList(table, serieName, valueName, offset), valueName);
                }
                else
                {
                    //секторные диаграммы
                    mapSerie.AddPieChartPart(valueName);
                    mapSerie.PieChartOffset = offset;
                }
            }
            else
            {
                AddLegend(serieName, valueName);

                //заливка
                shapeRule = Series.GetShapeRule(shapeRuleCategory, valueName);
                mapSerie.AddShapeSerieRule(GetSymbolList(table, serieName, valueName, offset), valueName);
            }

        }

        /// <summary>
        /// получение существующего значка
        /// </summary>
        /// <param name="objectName">имя объекта</param>
        /// <param name="serieName">имя серии</param>
        /// <param name="valueName">имя показателя</param>
        /// <returns>значок, если найден, иначе - null</returns>
        private Symbol GetSymbol(string objectName, string serieName, string valueName)
        {
            List<Symbol> ruleSymbols = null;
           
            //ищем значек с такими параметрами, может раньше был такой
            MapSerie mapSerie = Series.GetPieChartSerie(serieName);
            if (mapSerie != null)
            {
                SerieRule serieRule = mapSerie.GetSerieRule(valueName);
                if (serieRule != null)
                    ruleSymbols = serieRule.Symbols;
            }
            
            if ((ruleSymbols == null)||(ruleSymbols.Count == 0))
            {
                ruleSymbols = this.symbols;
            }
           // ruleSymbols = this.symbols;
            foreach (Symbol symbol in ruleSymbols)
            {
                if ((symbol.ParentShape == objectName) && (symbol.Category == serieName))
                {
                    if (symbol.Tag == null)
                    {
                        if (symbol[MapHelper.CorrectFieldName(valueName)] != null)
                        {
                            symbol.Tag = valueName;
                            return symbol;
                        }
                    }
                    else if (((string) symbol.Tag) == valueName)
                    {
                        return symbol;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// получение существующего значка
        /// </summary>
        /// <param name="objectName">имя объекта</param>
        /// <param name="serieName">имя серии</param>
        /// <param name="valueName">имя показателя</param>
        /// <returns>значок, если найден, иначе - null</returns>
        private Symbol GetMapSymbol(string objectName, string serieName, string valueName)
        {
            foreach (Symbol symbol in this.Map.Symbols)
            {
                if ((symbol.ParentShape == objectName) && (symbol.Category == serieName))
                {
                    if (symbol.Tag == null)
                    {
                        if (symbol[MapHelper.CorrectFieldName(valueName)] != null)
                        {
                            symbol.Tag = valueName;
                            return symbol;
                        }
                    }
                    else if (((string)symbol.Tag) == valueName)
                    {
                        return symbol;
                    }
                }
            }
            return null;
        }


        private List<Symbol> GetSymbolList(DataTable dt, string category, string value, int offset)
        {
            List<Symbol> result = new List<Symbol>();

            if (!dt.Columns.Contains(Consts.objCodeColumn))
                return result;

            foreach (DataRow row in dt.Rows)
            {
                string objCode = String.Empty;
                if  (row[Consts.objCodeColumn] != DBNull.Value)
                    objCode = (string) row[Consts.objCodeColumn];
                Shape sh = null;
                if(objCode != String.Empty)
                    sh = GetShapeByPropertyValue("CODE", objCode);

                if (sh == null)
                    continue;

                string shapeName = sh.Name;//(string) row[Consts.objectsColumn];
                /*
                if (!ShapeExists(shapeName))
                {
                    continue;
                }*/

                if (row[value] == DBNull.Value)
                {
                    continue;
                }

                Symbol symbol = GetSymbol(shapeName, category, value);

                if (symbol != null)
                {
                    
                    string symbolName = String.Format("{0} {1} {2}", shapeName, category, value);
                    if (this.map.Symbols.GetByName(symbolName) == null)
                    {
                        symbol.Name = symbolName; 
                        map.Symbols.Add(symbol);
                    }
                }
                else
                {
                    symbol = new Symbol();
                    map.Symbols.Add(symbol);
                    symbol.Offset.X = offset;
                    symbol.ParentShape = shapeName;
                    symbol.Category = category;
                    symbol.Tag = value;
                }

                //if (ShapeExists(symbol.ParentShape))
                {
                    //Shape sh = (Shape) map.Shapes.GetByName(symbol.ParentShape);
                    symbol.Layer = sh.Layer; 
                }

                if (row[value] != DBNull.Value)
                {
                    if (this.Map.SymbolFields.GetByName(MapHelper.CorrectFieldName(value)) != null)
                        symbol[MapHelper.CorrectFieldName(value)] = row[value];
                }

                result.Add(symbol);
            }
            SetSymbolsOffset(result);
            return result;
        }

        public Shape GetShapeByName(string shapeName)
        {
            foreach (Shape sh in this.Map.Shapes)
            {
                if (sh["NAME"] == DBNull.Value)
                {
                    continue;
                }

                if ((string)sh["NAME"] == shapeName)
                //if (sh.Name == shapeName)
                {
                    return sh;
                }
            }
            return null;
        }


        public bool ShapeExists(string shapeName)
        {
            return (GetShapeByName(shapeName) != null);

            //return (map.Shapes.GetByName(shapeName) != null);
        }

        /// <summary>
        /// получение типа данных для колонки в таблице данных для серии
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public Type GetFieldType(string fieldName)
        {
            Field field = (Field)Map.SymbolFields.GetByName(MapHelper.CorrectFieldName(fieldName));
            if (field != null)
            {
                return field.Type;
            }
            return typeof (Double);
        }
        
        private void SetSymbolsOffset(List<Symbol> symbols)
        {
            foreach (Symbol sym in symbols)
            {
                if (GetSymbol(sym.ParentShape, sym.Category, (string)sym.Tag) == null)
                    sym.Offset.Y += sym.Height / 2 + 20;
            }
        }

        //установка типа подписей для объектов
        private void SetShapeCaptionsType(ShapeCaptionType value)
        {
            this.shapeCaptionType = value;
            //для подписей в виде кода задаем жирный шрифт
            switch(value)
            {
                case ShapeCaptionType.Code:
                    foreach (Shape sh in this.Map.Shapes)
                    {
                        sh.Font = new Font(sh.Font, FontStyle.Bold);
                    }
                    break;
                case ShapeCaptionType.Name:
                case ShapeCaptionType.ShortName:
                    foreach (Shape sh in this.Map.Shapes)
                        sh.Font = new Font(sh.Font, FontStyle.Regular);
                    break;
            }
        }

        #region секторные диаграммы

        private void map_PostPaint(object sender, MapPaintEventArgs e)
        {
            Symbol symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                MapSerie mapSerie = this.Series[symbol.Category];

                if (mapSerie == null)
                {
                    return;
                }

                if (!mapSerie.ShowCharts)
                {
                    return;
                }

                MapPieChartSymbol pieChartSymbol = mapSerie.GetPieChartSymbol(symbol.ParentShape, symbol.Category);
                if (pieChartSymbol == null)
                {
                    return;
                }

                if (pieChartSymbol.IsEmpty)
                    return;

                // Размер секторной диаграммы
                int width = symbol.Width;
                int height = symbol.Height;

                // Получаем координаты значка
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;

                DrawPieChart(mg.Graphics, x, y, width, height, pieChartSymbol.Values, mapSerie.PieChartColors, pieChartSymbol.ChartBase, pieChartSymbol.BaseColor);
            }
            
        }

        private void DrawPieChart(Graphics g, int x, int y, int width, int height, List<double> valueList, List<Color> colorList, double baseValue, Color baseColor)
        {
            if (valueList.Count == 0)
                return;

            int startAngle = 0;
            int sweepAngle = 0;
            bool isCorrectChart = true;

            double valuesSum = 0;
            foreach(double value in valueList)
            {
                valuesSum += value;
            }

            isCorrectChart = (valuesSum <= baseValue);

            //Признак - нужно ли рисовать границу для секторной диаграммы(если диаграмма пустая, то не рисуем)
            //bool isNeedDrawBorder = false;

            for (int i = 0; i < valueList.Count; i++ )
            {
                if (valueList[i] < 0)
                {
                    isCorrectChart = false;
                    continue;
                }
                if (valueList[i] == 0)
                {
                    continue;
                }

                //isNeedDrawBorder = true;
                sweepAngle = (int)(valueList[i] / baseValue * 360);
                g.FillPie(new SolidBrush(colorList[i]), x, y, width, height, startAngle, sweepAngle);
                g.DrawPie(new Pen(Color.Gray, 1), x, y, width, height, startAngle, sweepAngle);
                startAngle += sweepAngle;
            }

            //if (isNeedDrawBorder)
                g.DrawEllipse(new Pen(baseColor, 1), x, y, width, height);

            if (!isCorrectChart)
            {
                g.DrawLine(new Pen(Color.Red, 3), x, y, x + width, y + height);
                g.DrawLine(new Pen(Color.Red, 3), x, y + height, x + width, y);
            }
        }

        #endregion

        #region Данные
        /*
        //заполнение списка названий объектов
        private void FillObjectNames(CellSet cellset)
        {
            objectNames = GetObjectNames();
        }*/

        private void FillObjectNames(CellSet cellset)
        {
            objectNames.Clear();
            for (int i = 0; i < cellset.Axes[1].Positions.Count; i++)
            {
                //фильтруем данные от оллмемберов
                if (IsAllMember(cellset.Axes[1].Positions[i].Members[0]))
                    continue;

                //дата мемберы не берем в качестве объектов карты
                if (cellset.Axes[1].Positions[i].Members[0].UniqueName.EndsWith(".DATAMEMBER"))
                    continue;

                string objCode = GetMemberPropertyValue(cellset.Axes[1].Positions[i].Members[0],
                                                        Consts.mapObjectCode);
                string objName = GetMemberPropertyValue(cellset.Axes[1].Positions[i].Members[0],
                                                        Consts.mapObjectName);

                string fullName = cellset.Axes[1].Positions[i].Members[0].Caption;

                Shape sh = GetShapeByPropertyValue("CODE", objCode);

                if ((sh != null) && (objName != ""))
                {
                    sh["SHORTNAME"] = objName;
                    sh["NAME"] = fullName;
                    //sh.Name = objName;
                    objectNames.Add(fullName);
                }
            }
        }


        //заполнение списка названий значений
        private void FillValueNames(CellSet cellset)
        {
            List<Field> fieldList = new List<Field>();
            //bool hasDublicateCaptions = HasDuplicateCaptions(cellset.Axes[0].Positions);
            List<string> duplicateCaptions = GetDuplicateCaptions(cellset.Axes[0].Positions);

            foreach (Position pos in cellset.Axes[0].Positions)
            {
                //фильтруем данные от оллмемберов
                if (IsAllMember(pos.Members[0]))
                    continue;

                Member curMember = pos.Members[0];
                string memCaption = CommonUtils.GetMemberCaptionWithoutID(curMember);

                string valueName = duplicateCaptions.Contains(memCaption) ? GetCorrectMemberName(curMember) : memCaption;
                valueName = GetNameForCollectionItem(valuesNames, valueName);
                valuesNames.Add(valueName);
                string fieldName = MapHelper.CorrectFieldName(valueName); 

                Field field = (Field)Map.SymbolFields.GetByName(fieldName);
                if (field == null)
                {
                    field = new Field();
                    field.Name = fieldName; 
                    field.Type = typeof(Double);
                }

                fieldList.Add(field);
            }

            Map.SymbolFields.Clear();
            ////Map.ShapeFields.Clear();

            foreach (Field symbolField in fieldList)
            {
                Map.SymbolFields.Add(symbolField);
                if (Map.ShapeFields.GetByName(symbolField.Name) == null)
                    Map.ShapeFields.Add(symbolField);
            }
            if (Map.ShapeFields.GetByName("SHORTNAME") == null)
                Map.ShapeFields.Add("SHORTNAME").Type = typeof(string);
        }

        /// <summary>
        /// Есть ли на оси дубликаты заголовков у мемберов
        /// </summary>
        /// <returns></returns>
        private bool HasDuplicateCaptions(PositionCollection positions)
        {
            List<string> captionList = new List<string>();

            foreach (Position pos in positions)
            {
                string caption = pos.Members[0].Caption;
                if (captionList.Contains(caption))
                {
                    return true;
                }
                captionList.Add(caption);
            }

            return false;
        }

        /// <summary>
        /// Получение списка заголовков дубликатов
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        private List<string> GetDuplicateCaptions(PositionCollection positions)
        {
            List<string> captionList = new List<string>();
            List<string> duplicates = new List<string>();

            foreach (Position pos in positions)
            {
                string caption = CommonUtils.GetMemberCaptionWithoutID(pos.Members[0]);
                if ((captionList.Contains(caption)) && (!duplicates.Contains(caption)))
                {
                    duplicates.Add(caption);
                }
                captionList.Add(caption);
            }

            return duplicates;
        }

        /// <summary>
        /// Находится ли элемент на уровне all
        /// </summary>
        /// <param name="mbr"></param>
        /// <returns></returns>
        private bool IsAllMember(Member mbr)
        {
            return (mbr.ParentLevel.Name == "(All)");
        }

        /// <summary>
        /// Получение нормального имени для мембера. Нужно для того чтобы не было дубликатов в датасете
        /// </summary>
        /// <returns></returns>
        private string GetCorrectMemberName(Member mbr)
        {
            string realCaption = CommonUtils.GetMemberCaptionWithoutID(mbr);
            string extension = "";
            mbr = mbr.Parent;
            while ((mbr != null)&&(mbr.Type != MemberTypeEnum.All))
            {
                extension = CommonUtils.GetMemberCaptionWithoutID(mbr) + " " + extension;
                mbr = mbr.Parent;
            }

            if (extension != "")
            {
                extension = String.Format(" ({0})", extension.Trim()); 
            }
            return realCaption + extension;
        }

        /// <summary>
        /// Проверить, используется мембер для формирования итога или нет
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        private bool IsGrandMember(PivotAxis axis, string uniqueName)
        {
            foreach(FieldSet fs in axis.FieldSets)
            {
                if (fs.GrandMemberUN == uniqueName)
                    return true;
            }
            return false;
        }

        //заполнение названий серий
        private void FillSerieNames(CellSet cellset)
        {
            if (cellset.Axes.Count == 3)
            {
                List<string> duplicateCaptions = GetDuplicateCaptions(cellset.Axes[2].Positions);

                foreach (Position pos in cellset.Axes[2].Positions)
                {
                    //фильтруем данные от оллмемберов
                    if (IsAllMember(pos.Members[0]))
                        continue;

                    Member curMember = pos.Members[0];
                    string memberCaption = CommonUtils.GetMemberCaptionWithoutID(curMember);

                    //даем имя, если дубликат - добавляем заголовки родительских мемберов
                    string serieName = duplicateCaptions.Contains(memberCaption) ? GetCorrectMemberName(curMember) : memberCaption;
                    
                    //если и с родительскими элементами - дубликат, то в имя серии подставляем индекс дубликата
                    serieName = GetNameForCollectionItem(seriesNames, serieName);
                    seriesNames.Add(serieName);

                }
            }
            else
            {
                seriesNames.Add("Серия");
            }
        }

        /// <summary>
        /// создаем и инициализируем датасет пустыми таблицами
        /// </summary>
        /// <param name="seriesNames">имена таблиц = имена серий</param>
        /// <param name="objectNames">строки таблиц = имена объектов</param>
        /// <param name="valuesNames">столбцы таблиц = имена значений(мер)</param>
        /// <returns>датасет с пустыми таблицами</returns>
        private DataSet InitMapData(List<string> seriesNames, List<string> objectNames, List<string> valuesNames)
        {
            DataSet res = new DataSet();

            for (int i = 0; i < seriesNames.Count; i++)
            {
                res.Tables.Add(seriesNames[i]);

                res.Tables[seriesNames[i]].Columns.Add(Consts.objCodeColumn, typeof(String));
                res.Tables[seriesNames[i]].Columns.Add(Consts.objectsColumn, typeof(String));
                res.Tables[seriesNames[i]].Columns.Add(Consts.mapObjectShortName, typeof(String));


                for (int k = 0; k < valuesNames.Count; k++)
                {
                    res.Tables[seriesNames[i]].Columns.Add(valuesNames[k],
                                                           GetFieldType(MapHelper.CorrectFieldName(valuesNames[k])));
                }


                for (int j = 0; j < this.Map.Shapes.Count; j++)
                {
                    string objCode = (string)this.Map.Shapes[j]["CODE"];
                    string fullName = (string)this.Map.Shapes[j]["NAME"];
                    string shortName = (string)this.Map.Shapes[j]["SHORTNAME"];
                    if (String.IsNullOrEmpty(objCode))
                        continue;

                    res.Tables[seriesNames[i]].Rows.Add(new string[] { objCode, fullName, shortName });
                }

            }
            return res;
        }

        public void InitMapSerieNames(List<string> seriesNames, List<string> objectNames, List<string> valuesNames)
        {
            this.seriesNames = seriesNames;
            this.objectNames = objectNames;
            this.valuesNames = valuesNames;
        }

        public void AddFields(List<string> measureNames)
        {
            List<Field> fieldList = new List<Field>();

            foreach (string measureName in measureNames)
            {
                string fieldName = MapHelper.CorrectFieldName(measureName);

                Field field = (Field)Map.SymbolFields.GetByName(fieldName);
                if (field == null)
                {
                    field = new Field();
                    field.Name = fieldName;
                    field.Type = typeof(Double);
                }

                fieldList.Add(field);
            }

            this.Map.SymbolFields.Clear();
            ////this.Map.ShapeFields.Clear();

            foreach (Field symbolField in fieldList)
            {
                this.Map.SymbolFields.Add(symbolField);
                if (Map.ShapeFields.GetByName(symbolField.Name) == null)
                    this.Map.ShapeFields.Add(symbolField);
            }
            //поля NAME и CODE добавляются автоматически из dbf-ки шаблона. 
            //SHORTNAME добавляем сами
            if (Map.ShapeFields.GetByName("SHORTNAME") == null)
                this.Map.ShapeFields.Add("SHORTNAME").Type = typeof(string);

        }

        /// <summary>
        /// инициализация списков имен серий, объектов и показателей
        /// </summary>
        public void InitSerieNamesByDataset()
        {
            this.objectNames = GetObjectNames();

            DataSet ds = this.SourceDS;
            if (ds == null)
                return;

            this.seriesNames.Clear();
            foreach(DataTable table in ds.Tables)
                seriesNames.Add(table.TableName);

            this.valuesNames.Clear();
            if (ds.Tables.Count > 0)
            {
                foreach(DataColumn column in ds.Tables[0].Columns)
                {
                    if ((column.ColumnName != Consts.objectsColumn) &&
                         (column.ColumnName != Consts.objCodeColumn) &&
                         (column.ColumnName != Consts.mapObjectShortName))
                        this.valuesNames.Add(column.ColumnName);
                }
            }
            AddFields(valuesNames);
        }

        private string GetMemberPropertyValue(Member mbr, string propName)
        {
            MemberProperty prop = null;
            LevelProperty levProp = mbr.ParentLevel.LevelProperties.Find(propName);
            
            if (levProp != null)
            {
                foreach(MemberProperty mbrProp in mbr.MemberProperties)
                {
                    if (mbrProp.UniqueName == levProp.UniqueName)
                    {
                        prop = mbrProp;
                        break;
                    }
                }
            }

            if (prop != null)
            {
                if (prop.Value != null)
                {
                    if (propName == Consts.mapObjectCode)
                        return GetMemberPropertyValue(prop);

                    return prop.Value.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// При работе с MASS2005 все числовые значения в member propertys отображаются в 
        /// экспоненциальном виде, будем форматировать их в стандартный вид
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <returns></returns>
        public static string GetMemberPropertyValue(MemberProperty memberProperty)
        {
            //if(memberProperty.Value is Double)
            string result = memberProperty.Value.ToString();
            if (((PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2005) ||
                    (PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2008))
                && ((result.Contains("E")) || (result.Contains("."))))
            {
                string temp = result.Replace('.', ',');
                double nValue;
                if (double.TryParse(temp, out nValue))
                {
                    result = ((int)nValue).ToString();
                }
            }
            return result;
        }


        public Shape GetShapeByPropertyValue(string propertyName, string value)
        {
            if (this.Map.ShapeFields.GetByName(propertyName) == null)
                return null;

            foreach (Shape sh in this.Map.Shapes)
            {
                if ((string)sh[propertyName] == value)
                {
                    return sh;
                }
            }
            return null;
        }

        //заполнение датасета данными
        private void FillSeriesData(CellSet cellset, DataSet ds)
        {
            if (ds.Tables.Count == 0)
            {
                return;
            }

            List<Type> valueTypes = new List<Type>();

            foreach (DataColumn column in ds.Tables[0].Columns)
            {
                valueTypes.Add(column.DataType);
            }

            List<string> objCodes = new List<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                objCodes.Add(row[Consts.objCodeColumn].ToString());
            }

            int iVal = 0, iSer = 0;

            //районы 
            for (int i = 0; i < cellset.Axes[1].Positions.Count; i++)
            {
                //дата мемберы не берем в качестве объектов карты
                if (cellset.Axes[1].Positions[i].Members[0].UniqueName.EndsWith(".DATAMEMBER"))
                    continue;

                string objCode = GetMemberPropertyValue(cellset.Axes[1].Positions[i].Members[0],
                                        Consts.mapObjectCode);

                int objIndex = objCodes.IndexOf(objCode);
                if (objIndex < 0)
                    continue;

                //значения
                iVal = 0;
                for (int j = 0; j < cellset.Axes[0].Positions.Count; j++)
                {
                    
                    //фильтруем данные от оллмемберов
                    if (IsAllMember(cellset.Axes[0].Positions[j].Members[0]))
                        continue;

                    //для трех осей - дополнительный цикл по сериям
                    object tmpVar;
                    if (cellset.Axes.Count == 3)
                    {
                        //серии
                        iSer = 0;
                        for (int k = 0; k < cellset.Axes[2].Positions.Count; k++)
                        {
                            if (IsAllMember(cellset.Axes[2].Positions[k].Members[0]))
                                continue;

                            tmpVar = GetValueForTableCell(cls.Cells[j, i, k], valueTypes[iVal + 3]);
                            if ((valueTypes[iVal + 3] != typeof (DateTime)) || (tmpVar != DBNull.Value))
                            {
                                    ds.Tables[iSer].Rows[objIndex][iVal + 3] = tmpVar;
                            }
                            iSer++;
                        }
                    }
                        //для двух осей
                    else
                    {
                        tmpVar = GetValueForTableCell(cls.Cells[j, i], valueTypes[iVal + 3]);
                        if ((valueTypes[iVal + 3] != typeof (DateTime)) || (tmpVar != DBNull.Value))
                        {
                                ds.Tables[0].Rows[objIndex][iVal + 3] = tmpVar;
                        }
                    }
                    iVal++;
                }
                
            }
        }

        private void PrepareDataForFormatting(string valueName, Type newType, bool needDataCorrect, double correctKoef)
        {
            Field field = (Field)Map.SymbolFields.GetByName(MapHelper.CorrectFieldName(valueName));
            if (field != null)
            {
                field.Type = newType;
            }

            field = (Field)Map.ShapeFields.GetByName(MapHelper.CorrectFieldName(valueName));
            if (field != null)
            {
                if (field.Type != newType)
                {
                    Map.ShapeFields.Remove(field);
                    field = Map.ShapeFields.Add(MapHelper.CorrectFieldName(valueName));
                    field.Type = newType;
                }
            }

            DataSet ds = SourceDS;

            if (ds == null)
            {
                return;
            }

            int j = ds.Tables[0].Columns[valueName].Ordinal - 1;

            if (newType != ds.Tables[0].Columns[j + 1].DataType)
            {
                foreach (DataTable table in ds.Tables)
                {
                    table.Columns.Remove(valueName);
                    DataColumn column = table.Columns.Add(valueName, newType);
                    column.SetOrdinal(j + 1);
                }
            }

            for (int i = 0; i < cls.Axes[1].Positions.Count; i++)
            {
                object tmpVar;
                if (cls.Axes.Count == 3)
                {
                    //серии
                    for (int k = 0; k < cls.Axes[2].Positions.Count; k++)
                    {
                        tmpVar = GetValueForTableCell(cls.Cells[j, i, k], newType);
                        if ((newType != typeof (DateTime)) || (tmpVar != DBNull.Value))
                        {
                            if (needDataCorrect)
                            {
                                tmpVar = (Double) tmpVar*correctKoef;
                            }
                            ds.Tables[k].Rows[i][j + 1] = tmpVar;
                        }
                    }
                }
                else
                {
                    tmpVar = GetValueForTableCell(cls.Cells[j, i], newType);
                    if ((newType != typeof (DateTime)) || (tmpVar != DBNull.Value))
                    {
                        if (needDataCorrect)
                        {
                            tmpVar = (Double) tmpVar*correctKoef;
                        }
                        ds.Tables[0].Rows[i][j + 1] = tmpVar;
                    }
                }
            }
            SourceDS = ds;
        }

        private static object GetValueForTableCell(Cell cell, Type dataType)
        {
            object value = null;

            try
            {
                value = cell.Value;
            }
            catch
            {
            }

            if (value == null)
            {
                return DBNull.Value;
            }

            object cellValue = DBNull.Value;
            double doubleValue;

            if (dataType == typeof (Double))
            {
                try
                {
                    Double.TryParse(value.ToString(), out doubleValue);
                    cellValue = doubleValue; // (Double)value;
                }
                catch
                {
                }
            }
            else if (dataType == typeof (DateTime))
            {
                try
                {
                    Double.TryParse(value.ToString(), out doubleValue);
                    cellValue = DateTime.FromOADate(doubleValue);
                }
                catch
                {
                    cellValue = DateTime.MinValue;
                }
            }

            return cellValue;
        }

        private DataSet PopulateMapDataFromCellSet(CellSet cellset)
        {
            DataSet series = null;
            objectNames.Clear();
            valuesNames.Clear();
            seriesNames.Clear();
            mapSeries.Clear();
            try
            {
                if ((cellset == null) || !((cellset.Axes.Count == 2) || (cellset.Axes.Count == 3)))
                {
                    return null;
                }
                //получаем параметры для таблиц(имена таблиц, строк и столбцов)
                FillValueNames(cellset);
                FillObjectNames(cellset);
                FillSerieNames(cellset);
                //инициализируем датасет пустыми таблицами по полученным параметрам 
                series = InitMapData(seriesNames, objectNames, valuesNames);
                //заполняем датасет данными
                FillSeriesData(cellset, series);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return series;
        }

        private static string GetNameForCollectionItem(List<string> collection, string itemName)
        {
            string result = itemName;
            if (collection == null)
            {
                return result;
            }

            bool isNameExist;
            int i = 1;
            do
            {
                isNameExist = (collection.IndexOf(result) != -1);
                if (isNameExist)
                {
                    result = string.Format("{0} ({1})", itemName, i);
                    i++;
                }
            } while (isNameExist);
            return result;
        }

        public List<string> GetObjectNames()
        {
            List<string> objectNames = new List<string>();
            foreach (Shape sh in this.map.Shapes)
            {
                objectNames.Add((string)sh["NAME"]);
            }

            return objectNames;
        }

        public List<string[]> GetObjectCodesWithNames()
        {
            List<string[]> objectNames = new List<string[]>();
            foreach (Shape sh in this.map.Shapes)
            {
                string[]value = new string[3];
                value[0] = (string)sh["CODE"];
                value[1] = (string)sh["NAME"];
                value[2] = (string)sh["SHORTNAME"];
                objectNames.Add(value);
            }

            return objectNames;
        }


        #endregion

        #region Сохранение/загрузка

        private void SaveLayerSettings(XmlNode root)
        {
            XmlNode layersNode = XmlHelper.AddChildNode(root, Consts.mapLayers, "", null);
            foreach (Layer layer in Map.Layers)
            {
                MapLayerBrowseClass layerBrowse = new MapLayerBrowseClass(layer, this);
                double offsetX = layerBrowse.CentralPointOffset.X;
                double offsetY = layerBrowse.CentralPointOffset.Y;

                XmlHelper.AddChildNode(layersNode, "layer",
                                       new string[2] {"name", layer.Name},
                                       //new string[2] { "offsetX", offsetX.ToString() },
                                       //new string[2] { "offsetY", offsetY.ToString() },
                                       new string[2] {"displayEmptyShapeNames", layer.Tag.ToString() }
                                       );
            }
        }

        private void LoadLayerSettings(XmlNode root)
        {
            if (root == null)
            {
                return;
            }

            foreach (XmlNode layerNode in root.ChildNodes)
            {
                string layerName = XmlHelper.GetStringAttrValue(layerNode, "name", "");
                Layer layer = (Layer) Map.Layers.GetByName(layerName);
                if (layer == null)
                {
                    continue;
                }
                layer.Tag = XmlHelper.GetBoolAttrValue(layerNode, "displayEmptyShapeNames", true);

                //double offsetX = XmlHelper.GetFloatAttrValue(layerNode, "offsetX", 0);
                //double offsetY = XmlHelper.GetFloatAttrValue(layerNode, "offsetY", 0);
                /*
                MapLayerBrowseClass layerBrowse = new MapLayerBrowseClass(layer, this.Map);
                layerBrowse.DisplayEmptyShapeNames = (bool)layer.Tag;
                layerBrowse.CentralPointOffset = new MapPoint(offsetX, offsetY);
                */
            }
        }

        public override XmlNode Save()
        {
            XmlNode result = base.Save();

            //Размер карты
            SizeConverter sizeConverter = new SizeConverter();
            string elementSize = sizeConverter.ConvertToString(Map.Size);
            XmlHelper.SetAttribute(result, Consts.elementSize, elementSize);
            XmlHelper.SetAttribute(result, Consts.elementDock, Map.Dock.ToString());
            XmlHelper.SetAttribute(result, Consts.symMaxSize, SymbolSize.SymbolMaxSize.ToString());
            XmlHelper.SetAttribute(result, Consts.symMinSize, SymbolSize.SymbolMinSize.ToString());
            XmlHelper.SetAttribute(result, Consts.isPropSymbolSize, this.Series.IsProportionalSymbolSize.ToString());
            Series.AddNotPropMeasuresNode(result);

            XmlHelper.SetAttribute(result, Consts.shapeCaptionType, this.ShapeCaptionType.ToString());

            SaveLayerSettings(result);
            Series.AddPieChartsNode(result);

            XmlHelper.SetAttribute(result, Consts.templateName, templateName);
            XmlHelper.SetAttribute(result, Consts.dataSourceType, this.DataSourceType.ToString());
            SaveSourceDT(XmlHelper.AddChildNode(result, Common.Consts.sourceDT));
            XmlHelper.AddChildNode(result, Consts.synchronization,
                            new string[2] { Consts.boundTo, this.Synchronization.BoundTo },
                            new string[2] { Consts.objectsInRows, this.Synchronization.ObjectsInRows.ToString() });

            this.LegendContents.Save(XmlHelper.AddChildNode(result, Consts.legendContents));
            XmlHelper.AddStringListNode(result, Consts.intervalName, this.ColorIntervalNames);

            SavePreset(XmlHelper.AddChildNode(result, Consts.presets));

            return result;
        }

        public void SetTemplateNameWithoutRefresh(string name)
        {
            this.templateName = name;
            LoadMapTemplate(MapRepositoryPath + name);
        }

        public override void Load(XmlNode reportElement, bool isForceDataUpdate)
        {
            this.InitMap();
            base.Load(reportElement, isForceDataUpdate);

            if (reportElement == null)
            {
                return;
            }

            this.canCreateLegend = false;

            string templateName = XmlHelper.GetStringAttrValue(reportElement, Consts.templateName, string.Empty);
            this.SetTemplateNameWithoutRefresh(templateName); 
            SymbolSize.SymbolMaxSize = XmlHelper.GetIntAttrValue(reportElement, Consts.symMaxSize, 50);
            SymbolSize.SymbolMinSize = XmlHelper.GetIntAttrValue(reportElement, Consts.symMinSize, 2);
            this.Series.IsProportionalSymbolSize = XmlHelper.GetBoolAttrValue(reportElement, Consts.isPropSymbolSize, false);
            this.Series.LoadNotPropMeasuresNode(reportElement.SelectSingleNode(Consts.notProportionalMeasures));

            this.ShapeCaptionType = (ShapeCaptionType)Enum.Parse(typeof(ShapeCaptionType),
                    XmlHelper.GetStringAttrValue(reportElement, Consts.shapeCaptionType, ShapeCaptionType.Name.ToString()));

            List<Shape> shapes = new List<Shape>();
            foreach(Shape sh in this.Map.Shapes)
            {
                shapes.Add((Shape)sh.Clone());
            }

            this.DataSourceType = (DataSourceType)Enum.Parse(typeof(DataSourceType),
                    XmlHelper.GetStringAttrValue(reportElement, Consts.dataSourceType, DataSourceType.Cube.ToString()));
            LoadSourceDT(reportElement.SelectSingleNode(Common.Consts.sourceDT));

            this.canCreateLegend = true;
            LoadPreset(reportElement.SelectSingleNode(Consts.presets));
            CheckShapes(shapes);
            
            //CheckLayers(MapRepositoryPath + TemplateName);
            XmlNode layersNode = reportElement.SelectSingleNode(Consts.mapLayers);
            LoadLayerSettings(layersNode);

            Map.Dock = (DockStyle)Enum.Parse(typeof(DockStyle),
                XmlHelper.GetStringAttrValue(reportElement, Consts.elementDock, DockStyle.Fill.ToString()));

            //Размер карты
            string elementSize = XmlHelper.GetStringAttrValue(reportElement, Consts.elementSize,
                string.Empty);
            if (elementSize != string.Empty)
            {
                SizeConverter sizeConverter = new SizeConverter();
                Map.Size = (Size)sizeConverter.ConvertFromString(elementSize);
            }

            //кешируем настройки для значков и заливки
            foreach (SymbolRule symbolRule in this.Map.SymbolRules)
            {
                this.Series.AddSymbolRule(symbolRule);
            }
            foreach (ShapeRule shapeRule in this.Map.ShapeRules)
            {
                this.Series.AddShapeRule(shapeRule);
            }

            this.CanRefreshMapAppearance = false;
            XmlNode pieChartsNode = reportElement.SelectSingleNode(Consts.mapPieCharts);
            Series.LoadPieChartsXml(pieChartsNode);
            this.CanRefreshMapAppearance = true;


            XmlNode syncNode = reportElement.SelectSingleNode(Consts.synchronization);
            if (syncNode != null)
            {
                this.Synchronization.BoundTo = XmlHelper.GetStringAttrValue(syncNode, Consts.boundTo, "");
                this.Synchronization.ObjectsInRows = XmlHelper.GetBoolAttrValue(syncNode, Consts.objectsInRows, true);
            }

            this.ColorIntervalNames = XmlHelper.GetStringListFromXmlNode(reportElement.SelectSingleNode(Consts.intervalName));
            this.LegendContents.Load(reportElement.SelectSingleNode(Consts.legendContents));

            //пивот дата
            if (!GetSyncronizedPivotData(true))
                this.PivotData.Load(reportElement.SelectSingleNode(Consts.pivotData), isForceDataUpdate);

            this.PivotData.DoForceDataChanged(); //DataChanged();

            if (this.DataSourceType == DataSourceType.Custom)
            {
                InitSerieNamesByDataset();
                RefreshObjectNamesInCustomDS();
                RefreshMapAppearance();
            }

            if (!IsShowErrorMessage)
                Map.Show();
        }

        private void SaveSourceDT(XmlNode sourceDSNode)
        {
            if ((this.SourceDS != null) && (this.DataSourceType == DataSourceType.Custom))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    this.SourceDS.DataSetName = "Данные серий";
                    this.SourceDS.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Flush();

                    stream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        XmlHelper.AppendCDataSection(sourceDSNode, streamReader.ReadToEnd());
                    }
                }
            }
            else
            {
                XmlHelper.AppendCDataSection(sourceDSNode, string.Empty);
            }
        }

        private void LoadSourceDT(XmlNode sourceDSNode)
        {
            if ((sourceDSNode == null) || (sourceDSNode.FirstChild.Value == string.Empty))
                return;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(sourceDSNode.FirstChild.Value);
                    streamWriter.Flush();

                    stream.Position = 0;
                    DataSet ds = new DataSet();
                    ds.ReadXml(stream);
                    this.SourceDS = ds;
                }
            }
        }

        private void SavePreset(XmlNode presetNode)
        {
            using (StringWriter strWriter = new StringWriter())
            {
                Map.Serializer.SerializableContent = "*.*,Shape.Tag";
                Map.Serializer.NonSerializableContent = "Shape.EncodedShapeData";
               // Map.Serializer.NonSerializableContent = "Symbol.*,Shape.EncodedShapeData";
                Map.Serializer.Save(strWriter);
                XmlHelper.AppendCDataSection(presetNode, strWriter.ToString());
            }
        }

        private void LoadPreset(XmlNode presetNode)
        {
            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader strReader = new StringReader(reportElementPreset))
                {
                    //Map.Serializer.Content = SerializationContent.All;
                    Map.Serializer.SerializableContent = "*.*";
                    Map.Serializer.NonSerializableContent = "Shape.FieldData";
                    Map.Serializer.ResetWhenLoading = false;
                    Map.Serializer.Load(strReader);
                }
            }
        }

        #endregion

        #region Легенда

        private void AddLegend(string serieName, string valueName)
        {
            string legendName = serieName + " " + valueName;
            Legend legend = (Legend) Map.Legends.GetByName(legendName);

            bool serieHasData = SerieHasData(serieName, valueName);

            if ((legend == null) && serieHasData)
            {
                legend = Map.Legends.Add(legendName);
                legend.LegendStyle = LegendStyle.Column;
                legend.TextWrapThreshold = 30;

                legend.Title = (serieName == "Серия") ? "" : serieName;

                legend.CellColumns.Add("");
                LegendCellColumn column = new LegendCellColumn(valueName);
                legend.CellColumns.Add(column);
                column.ToolTip = valueName;
            }
            else
            {
                if ((legend != null) && !serieHasData)
                {
                    Map.Legends.Remove(legend);
                }
            }

            if ((legend != null)&&(this.legendContents[legend.Name] == null))
            {
                this.legendContents.Add(legendName, LegendContentType.Values);
            }

        }

        public void RefreshLegendContent()
        {
            foreach(SymbolRule rule in this.Map.SymbolRules)
            {
                string legendName = rule.ShowInLegend;
                Legend legend = (Legend)this.Map.Legends.GetByName(legendName);
                LegendContent content = this.LegendContents[legendName];
                if (content == null)
                    continue;
                switch (content.ContentType)
                {
                    case LegendContentType.Name:
                        for (int i = 0; i < legend.Items.Count; i++)
                            legend.Items[i].Text = rule.PredefinedSymbols[i].LegendText;
                        break;
                    case LegendContentType.NameAndValues:
                        for (int i = 0; i < legend.Items.Count; i++)
                            legend.Items[i].Text = String.Format("{0} ({1})", rule.PredefinedSymbols[i].LegendText,
                                                                 legend.Items[i].Text);
                        break;
                }
            }

            foreach (ShapeRule rule in this.Map.ShapeRules)
            {
                string legendName = rule.ShowInLegend;
                Legend legend = (Legend)this.Map.Legends.GetByName(legendName);
                LegendContent content = this.LegendContents[legendName];
                if (content == null)
                    continue;
                switch (content.ContentType)
                {
                    case LegendContentType.Name:
                        for (int i = 0; i < legend.Items.Count; i++)
                        {
                            if (i < this.ColorIntervalNames.Count)
                                legend.Items[i].Text = this.ColorIntervalNames[i]; //rule.CustomColors[i].LegendText;
                        }
                        break;
                    case LegendContentType.NameAndValues:
                        for (int i = 0; i < legend.Items.Count; i++)
                        {
                            if (i < this.ColorIntervalNames.Count)
                                legend.Items[i].Text = String.Format("{0} ({1})", this.ColorIntervalNames[i], //rule.CustomColors[i].LegendText,
                                                                 legend.Items[i].Text);
                        }
                        break;
                }
            }

        }

        private void RemoveUnusedLegends()
        {
            int i = 0;
            while (i < Map.Legends.Count)
            {
                Legend legend = Map.Legends[i];
                if (!(SymbolsUsedLegend(legend) || 
                        ShapesUsedLegend(legend) || 
                        PieChartUsedLegend(legend) ||
                        (legend.Name == Consts.objectList) ||
                        (legend.Name == "Заливка")
                        ))
                {
                    this.LegendContents.Remove(legend.Name);
                    Map.Legends.Remove(legend);
                }
                else
                {
                    i++;
                }
            }
        }

        private bool SymbolsUsedLegend(Legend legend)
        {
            foreach (SymbolRule rule in Map.SymbolRules)
            {
                if (rule.ShowInLegend == legend.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShapesUsedLegend(Legend legend)
        {
            foreach (ShapeRule rule in Map.ShapeRules)
            {
                if (rule.ShowInLegend == legend.Name)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool PieChartUsedLegend(Legend legend)
        {
            foreach (MapSerie mapSerie in Series.Items)
            {
                if ((mapSerie.Name == legend.Name)&&(mapSerie.ShowCharts))
                {
                    return true;
                }
            }
            return false;
        }

        private bool SerieHasData(string serieName, string valueName)
        {
            DataSet ds = SourceDS;
            DataTable dt = ds.Tables[serieName];
            foreach (DataRow row in dt.Rows)
            {
                if (row[Consts.objectsColumn] == DBNull.Value)
                    continue;

                if (!ShapeExists((string) row[Consts.objectsColumn]))
                {
                    continue;
                }
                if (row[valueName] != DBNull.Value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление подробной легенды для правила заливки
        /// </summary>
        private void AddShapeRuleLegend()
        {
            ShapeRule shRule = this.Map.ShapeRules.Count > 0 ? this.Map.ShapeRules[0] : null;
            Legend legend = (Legend)Map.Legends.GetByName("Заливка");
            if (shRule == null)
            {
                if (legend != null)
                    Map.Legends.Remove(legend);
                return;
            }

            if (legend != null)
            {

                RefreshShapeRuleLegend(legend);
                return;
            }

            legend = Map.Legends.Add("Заливка");
            legend.Visible = false;
            legend.Title = (shRule.Category == "Серия") ? "" : shRule.Category;
           
            legend.CellColumns.Add("color", "");
            legend.CellColumns.Add("name", shRule.ShapeField);
            legend.CellColumns.Add("value", "");
            legend.LegendStyle = LegendStyle.Column;

            RefreshShapeRuleLegend(legend);
        }

        private void AddObjectNumbersLegend()
        {
            if (!this.canCreateLegend)
                return;

            Legend legend = (Legend) Map.Legends.GetByName(Consts.objectList);
            if (legend != null)
            {
                RefreshObjectsLegend(legend);
                return;
            }

            legend = Map.Legends.Add(Consts.objectList);
            legend.Visible = false;

            legend.LegendStyle = LegendStyle.Column;
            legend.TextWrapThreshold = 30;
            legend.HeaderSeparator = LegendSeparatorType.Line;
            legend.HeaderSeparatorColor = Color.Gray;

            legend.Title = "";

            legend.CellColumns.Add("Код");
            legend.CellColumns.Add("  ");
            legend.CellColumns.Add("Наименование");
            

            RefreshObjectsLegend(legend);

            legend.SizeUnit = CoordinateUnit.Pixel;
            legend.LocationUnit = CoordinateUnit.Pixel;

            legend.AutoSize = true;
            legend.Dock = PanelDockStyle.None;
            legend.LegendStyle = LegendStyle.Table;
            legend.TableStyle = LegendTableStyle.Tall;
            legend.DockedInsideViewport = true;
            //Application.DoEvents();
            legend.AutoSize = false;
            
            legend.SizeUnit = CoordinateUnit.Pixel;
            legend.LocationUnit = CoordinateUnit.Pixel;
            legend.Size.Width = 200;
            legend.Size.Height = legend.Items.Count*19;
            legend.Location.X = 5;
            legend.Location.Y = 5;
            legend.ItemColumnSpacing = 300;
            legend.Visible = true;

        }

        //обновление списка объектов в легенде
        private void RefreshObjectsLegend(Legend legend)
        {
            legend.Items.Clear();

            //делаем сортировку по коду объекта
            List<int> codes = new List<int>();

            foreach (Shape sh in this.Map.Shapes)
            {
                string objCode = (string)sh["CODE"];
                if (String.IsNullOrEmpty(objCode))
                    continue;

                int code = 0;
                Int32.TryParse(objCode, out code);

                if (!codes.Contains(code))
                    codes.Add(code);
            }

            codes.Sort();


            foreach (int code in codes)
            {
                Shape sh = GetShapeByPropertyValue("CODE", code.ToString());
                if (sh == null)
                    continue;

                string objName = (string)sh["NAME"];
                string objCode = (string)sh["CODE"];
                if ((objName == objCode) || String.IsNullOrEmpty(objName) || String.IsNullOrEmpty(objCode))
                {
                    continue;
                }
                LegendItem legItem = legend.Items.Add(sh.Name);
                
                legItem.Cells.Add(objCode, LegendCellType.Text, objCode, ContentAlignment.MiddleRight);
                legItem.Cells.Add(LegendCellType.Text, "   ", ContentAlignment.MiddleRight);
                legItem.Cells.Add(objName, LegendCellType.Text, objName, ContentAlignment.MiddleLeft);
            }

        }


        /// <summary>
        /// Получение маски формата для легенды из текста
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string GetMaskFromText(string text)
        {
            int beginMask = text.IndexOf("{");
            if (beginMask > -1)
            {
                int endMask = text.IndexOf("}");
                if (beginMask < endMask)
                {
                    return text.Substring(beginMask + 1, endMask - beginMask - 1);
                }
            }
            return "";
        }

        //обновление списка объектов в легенде для заливки объектов
        private void RefreshShapeRuleLegend(Legend legend)
        {
            ShapeRule shRule = this.Map.ShapeRules.Count > 0 ? this.Map.ShapeRules[0] : null;
            if (shRule == null)
                return;

            legend.Items.Clear();

            DataTable legendValues = new DataTable();
            legendValues.Columns.Add("uName", typeof(string));
            legendValues.Columns.Add("caption", typeof(string));
            legendValues.Columns.Add("value", typeof(Double));
            legendValues.Columns.Add("color", typeof(Color));

            foreach (CustomColor customColor in shRule.CustomColors)
            {
                ArrayList aElements = customColor.GetAffectedElements();
                foreach (Shape sh in aElements)
                {
                    string objName = (string)sh["NAME"];

                    double value = 0;
                    if (sh[shRule.ShapeField] != null)
                        value = (double)sh[shRule.ShapeField];


                    if (String.IsNullOrEmpty(objName))
                    {
                        continue;
                    }

                    legendValues.Rows.Add(new object[4] { sh.Name, objName, value, customColor.Color });
                }
            }

            legendValues = SortTable(legendValues, "value", "ASC");
            string mask = GetMaskFromText(shRule.LegendText);

            foreach (DataRow row in legendValues.Rows)
            {
                LegendItem legItem = legend.Items.Add((string)row[0]);

                legItem.Cells.Add(LegendCellType.Symbol, (string)row[1], ContentAlignment.MiddleCenter);
                legItem.Cells.Add(LegendCellType.Text, (string)row[1], ContentAlignment.MiddleLeft);
                
                string objValue = ((Double)row[2]).ToString(mask);
                legItem.Cells.Add(LegendCellType.Text, objValue, ContentAlignment.MiddleRight);

                legItem.Color = (Color)row[3];
            }
        }


        /// <summary>
        /// Сортировка таблицы по значениям в конкретном столбце
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="order">ASC или DESC</param>
        /// <returns></returns>
        private DataTable SortTable(DataTable table, string columnName, string order)
        {
            if (!table.Columns.Contains(columnName))
            {
                return table;
            }
            table.DefaultView.Sort = String.Format("{0} {1}", columnName, order);

            return table.DefaultView.ToTable(); 
        }


        #endregion

        #region Форматы

        /*
        public void RefreshFormats()
        {
            foreach (PivotTotal total in PivotData.TotalAxis.Totals)
            {
                SetFormat(total.Caption, total.Format);
            }
        }
        */
        private static string GetMask(string digitsLabel, string unit, bool useThousandDelimiter, bool displayUnits)
        {
            string result = digitsLabel;
            if (useThousandDelimiter)
            {
                result = "#,##" + result;
            }
            if (displayUnits)
            {
                result += unit;
            }
            return result;
        }

        private static string GetDigitsLabel(byte digits)
        {
            string result = "";

            for (int i = 0; i < digits; i++)
            {
                result += "0";
            }

            if (result != "")
            {
                result = "." + result;
            }
            else
            {
                result = "";
            }

            return result;
        }

        private static string GetFormatStr(FormatType formatType, byte digits, bool useThousandDelimiter,
                                           bool displayUnits)
        {
            switch (formatType)
            {
                case FormatType.Auto:
                    return "#,##0.00"; // GetAutoFormatStr();
                case FormatType.DateTime:
                    return "f";
                case FormatType.Exponential:
                    return "0E+00";
                case FormatType.LongDate:
                    return "D";
                case FormatType.LongTime:
                    return "T";
                case FormatType.Percent:
                    if (displayUnits)
                    {
                        return GetMask("0" + GetDigitsLabel(digits), "%", useThousandDelimiter, displayUnits);
                    }
                    return GetMask("0" + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits);
                case FormatType.ShortDate:
                    return "d";
                case FormatType.ShortTime:
                    return "t";
                case FormatType.TrueFalse:
                    return "\"Истина\";\"Истина\";\"Ложь\"";
                case FormatType.YesNo:
                    return "\"Да\";\"Да\";\"Нет\"";
                case FormatType.None:
                    return "g";
                case FormatType.Currency:
                    return GetMask("0" + GetDigitsLabel(digits), @" \р\.", useThousandDelimiter, displayUnits);
                case FormatType.MilliardsCurrency:
                    return GetMask("0,,," + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter,
                                   displayUnits);
                case FormatType.MilliardsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter, displayUnits);
                case FormatType.MilliardsNumeric:
                    return GetMask("0,,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits);
                case FormatType.MillionsCurrency:
                    return GetMask("0,," + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits);
                case FormatType.MillionsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits);
                case FormatType.MillionsNumeric:
                    return GetMask("0,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits);
                case FormatType.ThousandsCurrency:
                    return GetMask("0," + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits);
                case FormatType.ThousandsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits);
                case FormatType.ThousandsNumeric:
                    return GetMask("0," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits);
                case FormatType.Numeric:
                    return GetMask("0" + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits);
            }


            return "";
        }
        /*
        private void SetFormat(string value, ValueFormat format)
        {
            bool displayUnits = (format.UnitDisplayType != UnitDisplayType.None);
            string formatString = GetFormatStr(format.FormatType, format.DigitCount, format.ThousandDelimiter,
                                               displayUnits);
            switch (format.FormatType)
            {
                case FormatType.DateTime:
                case FormatType.LongDate:
                case FormatType.LongTime:
                case FormatType.ShortDate:
                case FormatType.ShortTime:
                    PrepareDataForFormatting(value, typeof (DateTime), false, 0);
                    break;
                default:
                    if ((format.FormatType == FormatType.Percent) && (format.UnitDisplayType == UnitDisplayType.None))
                    {
                        PrepareDataForFormatting(value, typeof(Double), true, 100);
                    }
                    else
                    {
                        PrepareDataForFormatting(value, typeof(Double), false, 0);
                    }
                    break;
            }

            RefreshMapAppearance();
            SetFormat(value, formatString);
        }
        */
        private void SetFormat(string value, string formatString)
        {
            string fieldName = MapHelper.CorrectFieldName(value);

            foreach (SymbolRule rule in map.SymbolRules)
            {
                if (rule.SymbolField == fieldName)
                {
                    foreach (PredefinedSymbol symbol in rule.PredefinedSymbols)
                    {
                        symbol.Text = "#" + fieldName.ToUpper() + "{" + formatString + "}";
                        symbol.ToolTip = "#" + fieldName.ToUpper() + "{" + formatString + "}";
                    }
                    rule.LegendText = "#FROMVALUE{" + formatString + "} - #TOVALUE{" + formatString + "}";
                }
            }

            foreach (ShapeRule rule in map.ShapeRules)
            {
                if (rule.ShapeField == fieldName)
                {
                    rule.Text = "#" + GetShapeFieldCaption() + "\n#" + fieldName.ToUpper() + "{" + formatString + "}";
                    rule.ToolTip = "#NAME\n#" + fieldName.ToUpper() + "{" + formatString + "}";
                    rule.LegendText = "#FROMVALUE{" + formatString + "} - #TOVALUE{" + formatString + "}";
                }
            }
        }

        private static string GetTotalFormatString(PivotTotal total)
        {
            bool displayUnits = (total.Format.UnitDisplayType != UnitDisplayType.None);
            return GetFormatStr(total.Format.FormatType, total.Format.DigitCount, total.Format.ThousandDelimiter,
                                displayUnits);
        }

        public string GetTotalFormatString(string valueName)
        {
            foreach (PivotTotal total in PivotData.TotalAxis.Totals)
            {
                if (total.Caption == valueName)
                {
                    return GetTotalFormatString(total);
                }
            }
            return String.Empty;
        }

        public void RefreshFormatString(string valueName)
        {
            foreach (PivotTotal total in PivotData.TotalAxis.Totals)
            {
                if (total.Caption == valueName)
                {
                    SetFormat(total.Caption, GetTotalFormatString(total));
                }
            }
        }

        #endregion

        #region Слои

        private void AddLayer(string layerFileName)
        {
            string layerName = Path.GetFileNameWithoutExtension(layerFileName);
            int oldShapesCount = map.Shapes.Count;

            bool objectsHasNames = ObjectsHasNames(layerFileName);
            map.LoadFromShapeFile(layerFileName, objectsHasNames ? "NAME" : "", true);
            Layer layer = map.Layers.Add(layerName);
            layer.Tag = true;
 
            Group group = map.Groups.Add(layerName);
            group.Text = "";

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                map.Shapes[i].Layer = layerName;
                map.Shapes[i].ParentGroup = layerName;
            }

            RestoreLayerForShapes(layerName);
        }

        //имя слоя для объектов может сбиться, если загружаем слои с одинаковыми объектами... воcстановим его 
        private void RestoreLayerForShapes(string layerName)
        {
            foreach (Shape sh in map.Shapes)
            {
                if (sh.Layer == "(none)")
                {
                    sh.Layer = layerName;
                    sh.ParentGroup = layerName;
                }
            }
        }

        private bool LayerHasShapes(Layer layer)
        {
            foreach (Shape sh in map.Shapes)
            {
                if (sh.Layer == layer.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ObjectsHasNames(string layerFileName)
        {
            string dataBase = Path.GetDirectoryName(layerFileName);
            string tableName = Path.GetFileNameWithoutExtension(layerFileName);

            if (!File.Exists(dataBase + "\\" + tableName + ".dbf"))
            {
               // FormException.ShowErrorForm(new Exception(string.Format("Не найдены данные для слоя \"{0}\".", tableName)),
               //             ErrorFormButtons.WithoutTerminate);
                return false;
            }
            DataTable table = ShapeFileReader.ReadDBF(dataBase + "\\" + tableName + ".dbf");
            if (table == null)
            {
                return false;
            }
            return table.Columns.Contains("NAME");
        }

        #endregion

        #region Синхронизация

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pivotData"></param>
        /// <param name="refreshFieldList"></param>
        /// <param name="silentMode"></param>
        public void Synchronize(PivotData pivotData, bool refreshFieldList, bool silentMode)
        {
            bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
            this.PivotData.IsDeferDataUpdating = true;

            this.PivotData.Clear();

            this.PivotData.CubeName = pivotData.CubeName;

            foreach (FieldSet fs in pivotData.FilterAxis.FieldSets)
                this.PivotData.FilterAxis.FieldSets.CopyFieldSet(fs, AxisType.atFilters);

            if (this.Synchronization.ObjectsInRows)
            {
                foreach (FieldSet fs in pivotData.ColumnAxis.FieldSets)
                    this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(fs, AxisType.atColumns);

                this.PivotData.ColumnAxis.GrandTotalVisible = pivotData.ColumnAxis.GrandTotalVisible;


                foreach (FieldSet fs in pivotData.RowAxis.FieldSets)
                    this.PivotData.RowAxis.FieldSets.CopyFieldSet(fs, AxisType.atRows);

                this.PivotData.RowAxis.GrandTotalVisible = pivotData.RowAxis.GrandTotalVisible;

            }
            else
            {
                foreach (FieldSet fs in pivotData.ColumnAxis.FieldSets)
                    this.PivotData.RowAxis.FieldSets.CopyFieldSet(fs, AxisType.atRows);

                this.PivotData.RowAxis.GrandTotalVisible = pivotData.ColumnAxis.GrandTotalVisible;

                foreach (FieldSet fs in pivotData.RowAxis.FieldSets)
                    this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(fs, AxisType.atColumns);

                this.PivotData.ColumnAxis.GrandTotalVisible = pivotData.RowAxis.GrandTotalVisible;

            }

            pivotData.TotalAxis.RefreshMemberNames();

            foreach (FieldSet fs in pivotData.TotalAxis.FieldSets)
                this.PivotData.TotalAxis.FieldSets.CopyFieldSet(fs, AxisType.atTotals);

            foreach (PivotTotal total in pivotData.TotalAxis.Totals)
                this.PivotData.TotalAxis.CopyTotal(total);

            if (refreshFieldList)
            {
                this.MainForm.FieldListEditor.PivotData = this.PivotData;
                this.MainForm.FieldListEditor.InitEditor(this);
            }

            this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;

            if (!silentMode)
                this.RefreshData();
                //this.PivotData.DoDataChanged();

        }

        public void Synchronize()
        {
            PivotData pivotData = null;
            TableReportElement tableElement = null;

            if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
            {
                tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    pivotData = tableElement.PivotData;
                }
            }
            if (pivotData == null)
            {
                this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);
                return;
            }

            Synchronize(pivotData, true, false);
            if (tableElement != null)
                this.MainForm.UndoRedoManager.AddEvent(tableElement, UndoRedoEventType.DataChange);

        }

        /// <summary>
        /// Синхронизация с возможностью обхода отложенного обновления данных
        /// </summary>
        /// <param name="ignoreDeferDataUpdating"></param>
        public void Synchronize(bool forceDataUpdating)
        {
            if (forceDataUpdating)
            {
                bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                this.PivotData.IsDeferDataUpdating = false;
                Synchronize();
                this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
            }
            else
            {
                Synchronize();
            }
        }

        #endregion
    }

    public static class SymbolSize
    {
        private static int symbolMaxSize = 50;
        private static int symbolMinSize = 2;

        public static int SymbolMaxSize
        {
            get { return symbolMaxSize; }
            set { symbolMaxSize = value; }
        }

        public static int SymbolMinSize
        {
            get { return symbolMinSize; }
            set { symbolMinSize = value; }
        }
    }

    /// <summary>
    /// тип источника данных
    /// </summary>
    public enum DataSourceType
    {
        [Description("Куб")]
        Cube,
        [Description("Данные пользователя")]
        Custom
    }

    /// <summary>
    /// тип подписи объектов
    /// </summary>
    public enum ShapeCaptionType
    {
        [Description("Код")]
        Code,
        [Description("Наименование")]
        Name,
        [Description("Краткое наименование")]
        ShortName
    }
    
    /// <summary>
    /// край/угол легенды
    /// </summary>
    public enum LegendBorder
    {
        Top,
        Left,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        None
    }
}