using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAssociateMappingDesign : SmoKeyIdentifiedObjectDesign<IAssociateMapping>, IAssociateMapping
    {
        private SmoMappingValueDesign dataValueSmo;
        private SmoMappingValueDesign bridgeValueSmo;

        public SmoAssociateMappingDesign(IAssociateMapping serverControl)
            : base(serverControl)
        {
        }


        [DisplayName(@"Классификатор данных")]
        [Description("Определение атрибута со стороны классификатора данных, из которого будет браться значение при формировании сопоставимого.")]
        public SmoMappingValueDesign DataValueSmo
        {
            get
            {
                if (dataValueSmo == null)
                {
                    if (serverControl.OwnerObject is IAssociateRule)
                    {
                        dataValueSmo = new SmoAssociateMappingValueDesign(serverControl.DataValue, false);
                    }
                    else
                    {
                        dataValueSmo = new SmoMappingValueDesign(serverControl.DataValue, false);
                    }
                    dataValueSmo.OnChange += this.CallOnChange;
                }
                return dataValueSmo;
            }
        }

        [DisplayName(@"Сопоставимый")]
        [Description("Определение атрибута со стороны сопоставимого классификатора, в который будет записываться значение при формировании сопоставимого.")]
        public SmoMappingValueDesign BridgeValueSmo
        {
            get
            {
                if (bridgeValueSmo == null)
                {
                    if (serverControl.OwnerObject is IAssociateRule)
                    {
                        bridgeValueSmo = new SmoAssociateMappingValueDesign(serverControl.BridgeValue, true);
                    }
                    else
                    {
                        bridgeValueSmo = new SmoMappingValueDesign(serverControl.BridgeValue, true);
                    }
                    bridgeValueSmo.OnChange += this.CallOnChange;
                }
                return bridgeValueSmo;
            }
        }

        #region IAssociateMapping Members

        [Browsable(false)]
        public IMappingValue DataValue
        {
            get { return serverControl.DataValue; }
        }

        [Browsable(false)]
        public IMappingValue BridgeValue
        {
            get { return serverControl.BridgeValue; }
        }

        #endregion
    }
}
