using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoModificationItem : ServerManagedObject<IModificationItem>, IModificationItem
    {
        public SmoModificationItem(IModificationItem serverControl)
            : base(serverControl)
        {
        }

        #region IModificationItem Members

        public Dictionary<string, IModificationItem> Items
        {
            get { return ServerControl.Items; }
        }

        public string Key
        {
            get { return ServerControl.Key; }
        }

        public string Name
        {
            get { return ServerControl.Name; }
        }

        public ModificationTypes Type
        {
            get { return ServerControl.Type; }
        }

        public ModificationStates State
        {
            get { return ServerControl.State; }
        }

        public Exception Exception
        {
            get { return ServerControl.Exception; }
        }

        public int ImageIndex
        {
            get { return ServerControl.ImageIndex; }
        }

        public void Applay(IModificationContext context, out bool isAppliedPartially)
        {
            ServerControl.Applay(context, out isAppliedPartially);
        }

        #endregion

        public override void Dispose()
        {
            ServerControl.Dispose();
        }
    }
}
