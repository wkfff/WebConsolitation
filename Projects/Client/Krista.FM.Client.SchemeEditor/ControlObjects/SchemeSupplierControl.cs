using System;
using System.Collections.Generic;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class SchemeSupplierControl : CustomListControl<IDataSupplier, IDataKind>
    {
        public SchemeSupplierControl(IDataSupplier controlObject, CustomTreeNodeControl parent)
            : base(controlObject.Name, controlObject.Name, controlObject, parent, 0)
        {
            ExpandNode();
        }

        /// <summary>
        /// Добовляет дочерние элементы
        /// </summary>
        protected override void ExpandNode()
        {
            foreach (IDataKind item in ((IDataSupplier)ControlObject).DataKinds.Values)
            {
                Nodes.Add(new SampleControl(item.Code, String.Format("{0} {1}", item.Code, item.Name), new SmoDataKindDesign(item), this, 0));
            }
        }
    }
}
