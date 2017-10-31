using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Server.GlobalConsts;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal class GlobalConstantsModificationItem : CollectionModificationItem
    {
        internal GlobalConstantsModificationItem(string name, IGlobalConstsManager fromObject, object toObject)
            : base (name, fromObject, toObject)
        {
        }

        public override int ImageIndex
        {
            get { return 51; }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            ((GlobalConstsManager)FromObject).ResetCache();

            base.OnAfterChildApplay(context);
        }
    }
}