using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Server.DataPumpManagement;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Modifications
{
    internal class PumpRegistryModificationItem : CollectionModificationItem
    {
        internal PumpRegistryModificationItem(string name, IDataPumpInfo fromObject, object toObject)
            : base(name, fromObject, toObject)
        {
            
        }

        public override int ImageIndex
        {
            get
            {
                return 55;
            }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            ((DataPumpInfo)FromObject).ResetCache();

            base.OnAfterChildApplay(context);
        }
    }
}
