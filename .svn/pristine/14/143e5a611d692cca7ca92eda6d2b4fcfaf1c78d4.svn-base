using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoAssociateMapping : SmoKeyIdentifiedObject<IAssociateMapping>, IAssociateMapping
    {
        private SmoMappingValue dataValueSmo;
        private SmoMappingValue bridgeValueSmo;

        public SmoAssociateMapping(IAssociateMapping serverObject)
            : base(serverObject)
        {
        }

        public SmoMappingValue DataValueSmo
        {
            get 
            {
                if (dataValueSmo == null)
                {
                    if (ServerControl.OwnerObject is IAssociateRule)
                    {
                        dataValueSmo = new SmoAssociateMappingValue(ServerControl.DataValue, false);
                    }
                    else
                    {
                        dataValueSmo = new SmoMappingValue(ServerControl.DataValue, false);
                    }
                    dataValueSmo.OnChange += this.CallOnChange;
                }
                return dataValueSmo;
            }
        }

        public SmoMappingValue BridgeValueSmo
        {
            get 
            {
                if (bridgeValueSmo == null)
                {
                    if (ServerControl.OwnerObject is IAssociateRule)
                    {
                        bridgeValueSmo = new SmoAssociateMappingValue(ServerControl.BridgeValue, true);
                    }
                    else
                    {
                        bridgeValueSmo = new SmoMappingValue(ServerControl.BridgeValue, true);
                    }
                    bridgeValueSmo.OnChange += this.CallOnChange;
                }
                return bridgeValueSmo;
            }
        }

        #region IAssociateMapping Members

        public IMappingValue DataValue
        {
            get { return cached ? (IMappingValue)GetCachedObject("DataValue") : ServerControl.DataValue; }
        }

        public IMappingValue BridgeValue
        {
            get { return cached ? (IMappingValue)GetCachedObject("BridgeValue") : ServerControl.BridgeValue; }
        }

        #endregion
    }
}
