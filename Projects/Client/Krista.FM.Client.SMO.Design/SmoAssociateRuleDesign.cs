using System;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAssociateRuleDesign : SmoKeyIdentifiedObjectDesign<IAssociateRule>, IAssociateRule
    {
        public SmoAssociateRuleDesign(IAssociateRule serverControl)
            : base(serverControl)
        {
        }

        #region IAssociateRule Members

        [DisplayName(@"Наименование")]
        [Description("Наименование правила сопоставления.")]
        public string Name
        {
            get { return serverControl.Name; }
            set
            {
                if (ReservedWordsClass.CheckName(value))
                {
                    serverControl.Name = value;
                    CallOnChange();
                }
            }
        }

        public bool UseConversionTable
        {
            get { return serverControl.UseConversionTable; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool UseFieldCoincidence
        {
            get { return serverControl.UseFieldCoincidence; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool AddNewRecords
        {
            get { return serverControl.AddNewRecords; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool ReAssociate
        {
            get { return serverControl.ReAssociate; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool ReadOnly
        {
            get { return serverControl.ReadOnly; }
        }

        public StringElephanterSettings Settings
        {
            get { return serverControl.Settings; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IAssociateMappingCollection Mappings
        {
            get
            {
                return
                    serverControl.Mappings;
            }
        }

        public IAssociateRule CloneRule()
        {
            return serverControl.CloneRule();
        }

        #endregion

        #region IMinorModifiable Members

        public void Update(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IModifiable Members

        public IModificationItem GetChanges(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
