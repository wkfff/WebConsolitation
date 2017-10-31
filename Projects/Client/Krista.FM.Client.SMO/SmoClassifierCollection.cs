using System;
using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoClassifierCollection : SmoDictionaryBase<string, IClassifier>, IEntityCollection<IClassifier>
    {
        public SmoClassifierCollection(IDictionaryBase<string, IClassifier> serverObject)
            : base(serverObject)
        {
        }

        public SmoClassifierCollection(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            if (obj is IBridgeClassifier)
                return typeof(SmoBridgeClassifier);
            else if (obj is IVariantDataClassifier)
                return typeof(SmoVariantDataClassifier);
            else 
                return typeof(SmoClassifier);
        }

        #region IEntityCollection<IEntity> Members

        public IClassifier CreateItem(ClassTypes classType, SubClassTypes subClassType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDictionaryBase<string,IEntity> Members

        public new IClassifier New(string key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDictionary<string,IEntity> Members


/*        public new ICollection<IEntity> Values
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
        */
       /* public new IClassifier this[string key]
        {
            get
            {
                if (cached)
                {
                    IClassifier serverObject = (IClassifier)GetCachedValue(key);
                    Type objectType = serverObject is IBridgeClassifier
                                          ? typeof (SmoBridgeClassifier)
                                          : typeof (SmoClassifier);
                    return (IClassifier)SMOObjectsCache.GetSmoObject(objectType, serverObject);
                }
                else
                {
                    return ServerControl[key];
                }
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }*/

        #endregion

        #region ICollection<KeyValuePair<string,IEntity>> Members

        public void Add(KeyValuePair<string, IEntity> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(KeyValuePair<string, IEntity> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(KeyValuePair<string, IEntity>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Remove(KeyValuePair<string, IEntity> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
                
        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ICollection2DataTable Members

        public System.Data.DataTable GetDataTable()
        {
            return ((ICollection2DataTable)ServerControl).GetDataTable();
        }

        #endregion
    }
}
