using System;
using System.Collections.Generic;
using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
    public class Users : DomainObject
    {
        private IList<Memberships> memberships;

        public Users()
        {
            this.memberships = new List<Memberships>();
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int UserType { get; set; }
        public virtual bool Blocked { get; set; }
        public virtual string DNSName { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Patronymic { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual bool AllowDomainAuth { get; set; }
        public virtual bool AllowPwdAuth { get; set; }
        public virtual int? RefDepartments { get; set; }
        public virtual int? RefOrganizations { get; set; }
        public virtual int? RefRegion { get; set; }
	    public virtual string Email { get; set; }
        public virtual string PwdHashSHA { get; set; }

        [ReferenceField("RefUsers")]
        public virtual IList<Memberships> Memberships
        {
            get { return memberships; }
            set { memberships = value; }
        }
    }
}
