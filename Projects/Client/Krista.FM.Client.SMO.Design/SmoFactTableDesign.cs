using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoFactTableDesign : SmoDataSourceDividedClassDesign, IFactTable
    {
        public SmoFactTableDesign(IEntity serverControl)
            : base(serverControl)
        {
        }

        [Browsable(false)]
        public new IFactTable ServerControl
        {
            get { return (IFactTable)serverControl; }
        }

        #region IFactTable Members

        public Dictionary<int, string> GetDataSourcesNames()
        {
            return ServerControl.GetDataSourcesNames();
        }

        public List<string> GetPartitionsNameBySourceID(int sourceID)
        {
            return ServerControl.GetPartitionsNameBySourceID(sourceID);
        }

        #endregion
    }
}
