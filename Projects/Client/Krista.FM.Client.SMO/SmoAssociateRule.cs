using System;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoAssociateRule : SmoKeyIdentifiedObject<IAssociateRule>, IAssociateRule
    {
        public SmoAssociateRule(IAssociateRule serverObject)
            : base(serverObject)
        {
        }


        public SmoAssociateRule(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        #region IAssociateRule Members

        public string Name
        {
            get { return cached ? (string)GetCachedValue("Name") : serverControl.Name; }
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
            get { return cached ? (bool)GetCachedValue("UseConversionTable") : serverControl.UseConversionTable; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool UseFieldCoincidence
        {
            get { return cached ? (bool)GetCachedValue("UseFieldCoincidence") : serverControl.UseFieldCoincidence; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool AddNewRecords
        {
            get { return cached ? (bool)GetCachedValue("AddNewRecords") : serverControl.AddNewRecords; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool ReAssociate
        {
            get { return cached ? (bool)GetCachedValue("ReAssociate") : serverControl.ReAssociate; }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool ReadOnly
        {
            get { return cached ? (bool)GetCachedValue("ReadOnly") : serverControl.ReadOnly; }
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
            get {
                return
                    cached
                        ? (IAssociateMappingCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SMOAssociateMappingCollection),
                                                       GetCachedObject("Mappings"))
                        : serverControl.Mappings; }
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
