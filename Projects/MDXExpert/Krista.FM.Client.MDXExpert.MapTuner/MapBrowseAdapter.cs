using System;
using System.Collections.Generic;
using System.Text;
using Dundas.Maps.WinControl;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    class MapBrowseAdapter : FilterablePropertyBase
    {
        private MapControl map;
        private TunerForm TunerForm;

        public MapBrowseAdapter(TunerForm TunerForm, MapControl mapControl)
        {
            this.TunerForm = TunerForm;
            this.map = mapControl;
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
                MapLayerCollectionEditor.Map = this.map;
                return this.map.Layers;
            }
        }

        
        [Category("Репозиторий")]
        [Description("Путь к репозиторию")]
        [DisplayName("Путь к репозиторию")]
        [Editor(typeof(MapFolderNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string RepositoryPath
        {
            get { return TunerForm.MapRepositoryPath; }
            set
            {
                TunerForm.MapRepositoryPath = value;
            }
        }
        
        [Category("Репозиторий")]
        [Description("Шаблон")]
        [DisplayName("Шаблон")]
        [Editor(typeof(MapTemplateEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string TemplateName
        {
            get 
            {
                return this.TunerForm.TemplateName; 
            }
            set 
            {
                this.TunerForm.TemplateName = value;
            }
        }

        [Browsable(false)]
        public bool ShowExtendedSettings
        {
            get { return this.TunerForm.showExtendedSettings; }
        }

        
        [Category("Дополнительные настройки")]
        [Description("Карта")]
        [DisplayName("Карта")]
        [DynamicPropertyFilter("ShowExtendedSettings", "True")]
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
        
        
        #endregion
        
        public class MapFolderNameEditor : FolderNameEditor
        {
            protected override void InitializeDialog(FolderNameEditor.FolderBrowser folderBrowser)
            {
                folderBrowser.Description = "Выберите расположение репозитория";
            }
        }
        

    }
}
