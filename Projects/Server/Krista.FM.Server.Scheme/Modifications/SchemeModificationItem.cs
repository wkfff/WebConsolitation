using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.Scheme.Modifications
{
    internal class SchemeModificationItem : MajorObjectModificationItem
    {
        public SchemeModificationItem(string name, object fromObject)
            : base(Krista.FM.ServerLibrary.ModificationTypes.Modify, name, fromObject, null, null)
        {
        }

        public override int ImageIndex
        {
            get { return 53; }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            base.OnAfterChildApplay(context);
        }
    }
}
