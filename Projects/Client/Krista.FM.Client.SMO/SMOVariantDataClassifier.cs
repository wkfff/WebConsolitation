using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoVariantDataClassifier : SmoClassifier, IVariantDataClassifier
    {
        public SmoVariantDataClassifier(IClassifier serverObject)
            : base(serverObject)
        {
        }

		public SmoVariantDataClassifier(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region IVariantDataClassifier Members

        public int CopyVariant(int variantID)
        {
            return ((IVariantDataClassifier)ServerControl).CopyVariant(variantID);
        }

        public int CopyVariant(int variantID, VariantListenerDelegate listener)
        {
            return ((IVariantDataClassifier)ServerControl).CopyVariant(variantID, listener);
        }

        public void DeleteVariant(int variantID)
        {
            ((IVariantDataClassifier)ServerControl).DeleteVariant(variantID);
        }

        public void DeleteVariant(int variantID, VariantListenerDelegate listener)
        {
            ((IVariantDataClassifier)ServerControl).DeleteVariant(variantID, listener);
        }

        public void LockVariant(int variantID)
        {
            ((IVariantDataClassifier)ServerControl).LockVariant(variantID);
        }

        public void UnlockVariant(int variantID)
        {
            ((IVariantDataClassifier)ServerControl).UnlockVariant(variantID);
        }

        #endregion
    }
}
