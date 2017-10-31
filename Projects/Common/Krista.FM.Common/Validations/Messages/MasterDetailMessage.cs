using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Validations.Messages
{
    public class MasterDetailMessage : ValidationMessage
    {
        private readonly IEntityAssociation entityAssociation;

        private IEntityAssociation secondEntityAssociation;

        private String masterObjectKey = String.Empty;

		public MasterDetailMessage(String masterObjectKey)
		{
			this.masterObjectKey = masterObjectKey;
		}

		public MasterDetailMessage(String masterObjectKey, IEntityAssociation entityAssociation)
        {
            this.masterObjectKey = masterObjectKey;
			this.entityAssociation = entityAssociation;
        }

        public MasterDetailMessage(IEntityAssociation entityAssociation)
        {
            this.entityAssociation = entityAssociation;
        }

        public MasterDetailMessage(IEntityAssociation entityAssociation, IEntityAssociation secondEntityAssociation)
        {
            this.entityAssociation = entityAssociation;
            this.secondEntityAssociation = secondEntityAssociation;
        }

        public string MasterObjectKey
        {
            get 
            {
                if (string.IsNullOrEmpty(masterObjectKey))
                    masterObjectKey = entityAssociation.RoleBridge.ObjectKey;
                return masterObjectKey;
            }
        }

        public string DetailObjectKey
        {
            get
            {
                if (entityAssociation == null)
                    return string.Empty;
                return entityAssociation.RoleData.ObjectKey;
            }
        }

        public string SecondDetailObjectKey
        {
            get
            {
                if (secondEntityAssociation == null)
                    return string.Empty;
                return secondEntityAssociation.RoleData.ObjectKey;
            }
        }
    }
}