using System;
using System.Collections.Generic;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    /// <summary>
    /// Элемент управления коллекцией поставщиков данных
    /// </summary>
    public class SchemeSupplierListControl : CustomListControl<IDataSupplierCollection, IDataSupplier>
    {
        /// <summary>
        /// Элемент управления коллекцией поставщиков данных
        /// </summary>
        /// <param name="controlObject"></param>
        public SchemeSupplierListControl(IDataSupplierCollection controlObject, CustomTreeNodeControl parent)
            : base("DataSupplierCollection", "Поставщики данных", new SmoSupplierCollectionDesign(controlObject), parent, 0)
        {
            ExpandNode();
        }

        /// <summary>
        /// Добовляет дочерние элементы
        /// </summary>
        protected override void ExpandNode()
        {
            foreach (KeyValuePair<string, IDataSupplier> item in ((SmoSupplierCollectionDesign)this.ControlObject).ServerControl)
            {
                Nodes.Add(new SchemeSupplierControl(new SmoSupplierDesign(item.Value), this));
            }
        }
    }
}
