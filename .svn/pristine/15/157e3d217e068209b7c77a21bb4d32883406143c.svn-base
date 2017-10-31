using System;
using Krista.FM.Common;

namespace Krista.FM.Domain.Reporitory.NHibernate.IoC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TransactionAttribute : Attribute, ITransactionAttributeSettings
    {
        private TransactionAttributeSettings settings;

        public TransactionAttribute()
        {
            settings = new TransactionAttributeSettings();
        }

        public bool IsExceptionSilent
        {
            get { return Settings.IsExceptionSilent; }
            set { Settings.IsExceptionSilent = value; }
        }

        public object ReturnValue
        {
            get { return Settings.ReturnValue; }
            set { Settings.ReturnValue = value; }
        }

        #region ITransactionAttributeSettings Members

        public TransactionAttributeSettings Settings
        {
            get { return settings; }
            set
            {
                if (value == null)
                {
                    throw new PreconditionException("Свойство Settings не должно быть null");
                }
                settings = value;
            }
        }

        #endregion
    }
}
