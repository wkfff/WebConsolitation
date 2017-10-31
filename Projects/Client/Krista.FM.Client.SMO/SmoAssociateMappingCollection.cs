using System;
using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SMOAssociateMappingCollection : SmoDictionaryBase<string, IAssociateMapping>, IAssociateMappingCollection
    {
        public SMOAssociateMappingCollection(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoAssociateMapping);
        }

        #region IAssociateMappingCollection Members

        public IAssociateMapping CreateItem()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IList<IAssociateMapping> Members

        public int IndexOf(IAssociateMapping item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IAssociateMapping item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IAssociateMapping this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<IAssociateMapping> Members

        public void Add(IAssociateMapping item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IAssociateMapping item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IAssociateMapping[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IAssociateMapping item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IAssociateMapping> Members

        public new IEnumerator<IAssociateMapping> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
