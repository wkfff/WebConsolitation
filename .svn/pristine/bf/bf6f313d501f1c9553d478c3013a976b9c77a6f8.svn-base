using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoFactTable : SmoDataSourceDividedClass, IFactTable
	{
        public SmoFactTable(IEntity serverObject) 
            : base(serverObject)
        {
        }


        public SmoFactTable(SMOSerializationInfo cache) 
            : base(cache)
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
