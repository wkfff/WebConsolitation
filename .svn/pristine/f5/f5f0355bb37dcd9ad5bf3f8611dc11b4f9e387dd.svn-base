using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public abstract class SmoCommonDBObjectDesign : SmoCommonObjectDesign, ICommonDBObject, IMajorModifiable
    {
        protected SmoCommonDBObjectDesign(ICommonDBObject serverControl)
            : base(serverControl)
        {
        }
        
        #region ICommonDBObject Members

        [Browsable(false)]
        public string Key
        {
            get { return ((ICommonDBObject)serverControl).Key; }
        }

        [Browsable(false)]
        public IPackage ParentPackage
        {
            get { return ((ICommonDBObject)serverControl).ParentPackage; }
        }

        [Category("CommonObject")]
        [DisplayName(@"ID объекта (ID)")]
        [Description("ID объекта в БД")]
        [Browsable(false)]
        public int ID
        {
            get { return ((ICommonDBObject)serverControl).ID; }
        }

        [Category("CommonObject")]
        [DisplayName(@"Состояние объекта")]
        [Description("Состояние структуры объекта")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        public DBObjectStateTypes DbObjectState
        {
            get { return ((ICommonDBObject)serverControl).DbObjectState; }
        }

        [Browsable(false)]
        public string DeveloperDescription
        {
            get { return ((ICommonDBObject)serverControl).DeveloperDescription; }
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
            return ((ICommonDBObject)serverControl).GetSQLMetadataDictionary();
        }

        public List<string> GetSQLDefinitionScript()
        {
            return ((ICommonDBObject)serverControl).GetSQLDefinitionScript();
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Семантика (Semantic)")]
        [Description("Семантическая принадлежность объекта")]
        [Browsable(true)]
        [TypeConverter(typeof(RelationConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Semantic
        {
            get
            {
                return
                    String.Format("{0} ({1})", ((ICommonDBObject) serverControl).SemanticCaption,
                                  ((ICommonDBObject) serverControl).Semantic);
            }
            set
            {
                ((ICommonDBObject)serverControl).Semantic = value.Split('(', ')')[1];
                CallOnChange();
            }
        }

        [Browsable(false)]
        public string SemanticCaption
        {
            get { return ((ICommonDBObject)serverControl).SemanticCaption; }
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
