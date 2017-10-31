using System.ComponentModel;
using System.Xml.Serialization;
using Ext.Net;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core.ExtNet.Extensions.TreeGridPanel
{
    /* Store для иерархического представления
     - нужно указать ParentIdFieldName и LeafFieldName
     - функция проверки наличия изменений isDirty
     - параметры события сохранения Save (refStore, countOfRecords, recordsAll):
         - refStore - собственно Store;
         - countOfRecords - количество измененных записей;
         - recordsAll - структура переданная контроллеру { Created: [], Updated: [], Deleted: [] }; 
     - параметры события сохранения Exception (store, options, response):
         - store - собственно Store;
         - options, response - соотвествующие объекты ajax-ответа контроллера;
     - для работы с композитными идентификаторами необходимо перекрыть метод сортировки для коррекции сортировки внутри узла
                    new RecordField("ID")
                    {
                        CustomSortType =
                        {
                            Handler = @"
var ids = value.split('.');
return parseInt(ids[ids.length - 1]);
"
                        }
                    }
    */
    public class TreeGridPanelStore : Store
    {
        private string leafFieldName = string.Empty;
        private string parentIdFieldName = string.Empty;

        public string ParentIdFieldName
        {
            get
            {
                return parentIdFieldName;
            }

            set
            {
                parentIdFieldName = value;
            }
        }

        public virtual string LeafFieldName
        {
            get
            {
                return leafFieldName;
            }

            set
            {
                leafFieldName = value;
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
            get { return "Ext.ux.maximgb.tg.AdjacencyListStore"; }
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
                var optionLeafFieldName = new ConfigOption(
                    "leaf_field_name", 
                    null, 
                    "_is_leaf", 
                    LeafFieldName);
                list.Add("leaf_field_name", optionLeafFieldName);
                var optionParentIdFieldName = new ConfigOption(
                    "parent_id_field_name",
                    null,
                    "_parent",
                    ParentIdFieldName);
                list.Add("parent_id_field_name", optionParentIdFieldName);
                return list;
            }
        }
        
        public new abstract class Config : Store.Config
        {
            public virtual object LeafFieldName { get; set; }

            public virtual object ParentIdFieldName { get; set; }
        }
    }
}
