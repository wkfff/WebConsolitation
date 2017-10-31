using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public abstract class SmoCommonDBObject : SmoCommonObject, ICommonDBObject, IMajorModifiable
    {
        public SmoCommonDBObject(ICommonDBObject serverObject)
            : base(serverObject)
        {
        }

        public SmoCommonDBObject(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region ICommonDBObject Members

        public string Key
        {
            get
            {
                return cached ? (string)GetCachedValue("Key") : ((ICommonDBObject)serverControl).Key;
            }
        }

        [Browsable(false)]
        public IPackage ParentPackage
        {
            get { return ((ICommonDBObject)serverControl).ParentPackage; }
        }

        public int ID
        {
            get { return cached ? (int)GetCachedValue("ID") : ((ICommonDBObject)serverControl).ID; }
        }

        public DBObjectStateTypes DbObjectState
        {
            get { return cached ? (DBObjectStateTypes)GetCachedValue("DbObjectState") : ((ICommonDBObject)serverControl).DbObjectState; }
        }

        public string DeveloperDescription
        {
            get { return cached ? (string)GetCachedValue("DeveloperDescription") : ((ICommonDBObject)serverControl).DeveloperDescription; }
            set 
            { 
                bool callOnChangeEvent = !(this.IsClone || this.IsLocked);
                ((ICommonDBObject)serverControl).DeveloperDescription = value;
                if (callOnChangeEvent)
                    CallOnChange(); 
            }
        }

        public void EndEdit()
        {
            ((ICommonDBObject)serverControl).EndEdit();
        }

        public void EndEdit(string comments)
        {
            ((ICommonDBObject)serverControl).EndEdit(comments);
        }

        public void CancelEdit()
        {
            ((ICommonDBObject)serverControl).CancelEdit();
        }

        public Dictionary<string, string> GetSQLMetadataDictionary()
        {
            return ((ICommonDBObject) serverControl).GetSQLMetadataDictionary();
        }

        public List<string> GetSQLDefinitionScript()
        {
            return ((ICommonDBObject) serverControl).GetSQLDefinitionScript();
        }

		public virtual string Semantic
		{
			get
			{
				return
					cached
						? String.Format("{0} ({1})", GetCachedValue("SemanticCaption"), GetCachedValue("Semantic"))
						: String.Format("{0} ({1})", ((ICommonDBObject)serverControl).SemanticCaption, ((ICommonDBObject)serverControl).Semantic);
			}
			set
			{
				((ICommonDBObject)serverControl).Semantic = value.Split('(', ')')[1];
				CallOnChange();
			}
		}

		public string SemanticCaption
		{
			get { return cached ? (string)GetCachedValue("SemanticCaption") : ((ICommonDBObject)serverControl).SemanticCaption; }
		}

		#endregion

        #region IMajorModifiable Members

        public IModificationItem GetChanges()
        {
            return ((ICommonDBObject)serverControl).GetChanges();
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
