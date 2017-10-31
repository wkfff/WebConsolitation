using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinDock;
using Dundas.Maps.WinControl;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    class MapReportElementBrowseAdapter : CustomReportElementBrowseAdapter
    {
        private MapControl map;
        private MapReportElement mapElement;
        private DistanceScalePanelBrowseAdapter distanceScaleBrowse;
        private ZoomPanelBrowseAdapter zoomPanelBrowse;
        private ColorSwatchPanelBrowseAdapter colorSwatchPanelBrowse;
        private NavigationPanelBrowseAdapter navigationPanelBrowse;

        public MapReportElementBrowseAdapter(DockableControlPane dcPane)
            : base(dcPane)
        {
            CustomReportElement reportElement = (CustomReportElement)dcPane.Control;
            
            if (!(reportElement is MapReportElement))
                return;

            this.mapElement = ((MapReportElement)reportElement);
            this.map = mapElement.Map;

            distanceScaleBrowse = new DistanceScalePanelBrowseAdapter(map.DistanceScalePanel);
            zoomPanelBrowse = new ZoomPanelBrowseAdapter(map.ZoomPanel);
            colorSwatchPanelBrowse = new ColorSwatchPanelBrowseAdapter(map.ColorSwatchPanel);
            navigationPanelBrowse = new NavigationPanelBrowseAdapter(map.NavigationPanel);
        }

        #region Свойства
        
        
        [Category("Внешний вид")]
        [Description("Слои карты")]
        [DisplayName("Слои")]
        [Editor(typeof(MapLayerCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public LayerCollection Layers 
        {
            get
            {
                MapLayerCollectionEditor.MainForm = this.mapElement.MainForm;
                return this.map.Layers;
            }
        }

        [Category("Внешний вид")]
        [Description("Подписи объектов")]
        [DisplayName("Подписи объектов")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public ShapeCaptionType ShapeCaptionType
        {
            get { return this.mapElement.ShapeCaptionType; }
            set { this.mapElement.ShapeCaptionType = value; }
        }
          

        [Category("Дополнительные панели")]
        [Description("Цветовая шкала")]
        [DisplayName("Цветовая шкала")]
        [Browsable(true)]
        public ColorSwatchPanelBrowseAdapter ColorSwatch
        {
            get { return colorSwatchPanelBrowse; }
            set { colorSwatchPanelBrowse = value; }
        }
        

        [Category("Дополнительные панели")]
        [Description("Масштаб")]
        [DisplayName("Масштаб")]
        [Browsable(true)]
        public DistanceScalePanelBrowseAdapter DistanceScale
        {
            get { return distanceScaleBrowse; }
            set { distanceScaleBrowse = value; }
        }


        
        [Category("Дополнительные панели")]
        [Description("Легенды")]
        [DisplayName("Легенды")]
        [Editor(typeof(MapLegendCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public LegendCollection Legends
        {
            get
            {
                MapLegendCollectionEditor.MainForm = this.mapElement.MainForm;
                return this.map.Legends;
            }
        }
        

        [Category("Дополнительные панели")]
        [Description("Навигация")]
        [DisplayName("Навигация")]
        [Browsable(true)]
        public NavigationPanelBrowseAdapter Navigation
        {
            get { return navigationPanelBrowse; }
            set { navigationPanelBrowse = value; }
        }

        [Category("Дополнительные панели")]
        [Description("Увеличение")]
        [DisplayName("Увеличение")]
        [Browsable(true)]
        public ZoomPanelBrowseAdapter ZoomPanel
        {
            get { return zoomPanelBrowse; }
            set { zoomPanelBrowse = value; }
        }

        [Category("Репозиторий карт")]
        [Description("Расположение репозитория")]
        [DisplayName("Расположение репозитория")]
        [Editor(typeof(MapFolderNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string RepositoryPath
        {
            get { return MapReportElement.MapRepositoryPath; }
            set
            {
                MapReportElement.MapRepositoryPath = value;
                this.mapElement.SetTemplateNameWithoutRefresh("");

            }
        }

        [Category("Репозиторий карт")]
        [Description("Шаблон карты")]
        [DisplayName("Шаблон карты")]
        [Editor(typeof(MapTemplateEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string TemplateName
        {
            get 
            {
                return this.mapElement.TemplateName; 
            }
            set 
            {
                this.mapElement.TemplateName = value;
                foreach (Shape shape in this.Map.Shapes)
                {
                    shape.TextVisibility = TextVisibility.Shown;
                }
                this.mapElement.LoadPreset();
                this.mapElement.RefreshMapAppearance();
            }
        }

        [Category("Управление данными")]
        [Description("Серии")]
        [DisplayName("Серии")]
        [Editor(typeof(MapSeriesCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public MapSerieCollection Series
        {
            get
            {
                return this.mapElement.Series;
            }
            set
            {
                this.mapElement.Series = value;
            }
        }

        [Category("Управление данными")]
        [Description("Тип источника данных")]
        [DisplayName("Источник данных")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public DataSourceType DataSourceType
        {
            get
            {
                return this.mapElement.DataSourceType;
            }
            set
            {
                if (((value == MDXExpert.DataSourceType.Cube) && CheckCube()) || (value != MDXExpert.DataSourceType.Cube))
                {
                    this.mapElement.DataSourceType = value;
                    this.mapElement.PivotData.DoDataChanged();
                    this.mapElement.MainForm.FieldListEditor.PivotDataContainer.Refresh();
                    //this.mapElement.InitialByCellSet();
                }
            }
        }

        /*
        [Category("Данные")]
        [Description("Пропорциональный размер значков по всем сериям")]
        [DisplayName("Пропорциональный размер значков")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool IsProportionalSymbolSize
        {
            get
            {
                return this.mapElement.Series.IsProportionalSymbolSize;
            }
            set
            {
                this.mapElement.Series.IsProportionalSymbolSize = value;
            }
        }
        */
        [Category("Дополнительные настройки")]
        [Description("Карта")]
        [DisplayName("Карта")]
        [Browsable(true)]
        public MapControl Map
        {
            get
            {
                return this.map;
            }
            set
            {
                this.map = value;
            }
        }


        [Category("Управление данными")]
        [DisplayName("Синхронизация")]
        [Description("Синхронизация")]
        [Editor(typeof(MapSyncEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public MapSynchronization Synchronization
        {
            get
            {
                return this.mapElement.Synchronization;
            }
            set
            {
                this.mapElement.Synchronization = value;
            }
        }



        #endregion

        public class MapFolderNameEditor : FolderNameEditor
        {
            protected override void InitializeDialog(FolderNameEditor.FolderBrowser folderBrowser)
            {
                folderBrowser.Description = "Выберите расположение репозитория";
                //folderBrowser.DirectoryPath = MapReportElement.MapRepositoryPath;
            }
        }

        /// <summary>
        /// проверяем, если куб не выбран, то даем возможность его выбрать
        /// </summary>
        private bool CheckCube()
        {
            if (MessageBox.Show("В случае выбора куба в качестве источника данных для карты пользовательские данные будут утеряны. Продолжить?",
                    "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            if (this.mapElement.PivotData.Cube != null)
                return true;
            /*
            if (MessageBox.Show("Куб не выбран. Выбрать в качестве источника данных для карты один из имеющихся гиперкубов?",
                            "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;
            */

            CubeDef cube = this.mapElement.MainForm.ChooseCube();
            if (cube != null)
            {
                this.mapElement.PivotData.CubeName = cube.Name;
                this.mapElement.MainForm.RefreshUserInterface(this.mapElement);
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
