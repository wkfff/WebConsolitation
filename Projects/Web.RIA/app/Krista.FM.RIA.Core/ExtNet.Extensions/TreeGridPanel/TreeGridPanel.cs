using System.ComponentModel;
using System.Web.UI;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core.ExtNet.Extensions.TreeGridPanel
{
    /* GridPanel для иерархического представления
     - необходимо указать MasterColumnId
     - при работе не в Restful режиме использовать функции, не вызывающие обращения к контроллеру
         - reloadHandler() обновление, сброс текущего элемента
         - saveHandler() сохранение
         - addHandler(record) добавление записи 
             для добавления в корень параметр record = null
             для получения выделенной записи getSelectedRecord()
         - deleteHandler() удаление выделенной записи
    */
    public class TreeGridPanel : GridPanel
    {
        private string masterColumnId = string.Empty;

        public TreeGridPanel(Page page)
        {
            Ext.Net.ResourceManager.GetInstance(page).RegisterStyle("MaximGB.TreeGrid", "/Krista.FM.RIA.Core/ExtNet.Extensions/TreeGridPanel/css/TreeGrid.css/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript("MaximGB.TreeGrid", "/Krista.FM.RIA.Core/ExtNet.Extensions/TreeGridPanel/js/TreeGrid.js/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript("MaximGB.TreeGrid.Patch", "/Krista.FM.RIA.Core/ExtNet.Extensions/TreeGridPanel/js/TreeGridPatch.js/extention.axd");
        }

        public string MasterColumnId
        {
            get
            {
                return masterColumnId;
            }

            set
            {
                masterColumnId = value;
            }
        }

        public override string InstanceOf
        {
            get
            {
                return XType;
            }
        }

        public override string XType
        {
            get
            {
                return "Ext.ux.maximgb.tg.EditorGridPanel";
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        [JsonIgnore]
        public override ConfigOptionsCollection ConfigOptions
        {
            get
            {
                var list = base.ConfigOptions;
                var optionMasterColumnId = new ConfigOption(
                    "master_column_id",
                    null,
                    string.Empty,
                    MasterColumnId);
                list.Add("master_column_id", optionMasterColumnId);
                return list;
            }
        }

        public new abstract class Config : GridPanel.Config
        {
            public virtual object MasterColumnId { get; set; }
        }
    }
}
