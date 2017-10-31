using System;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json.Serialization;

using NHibernate.Proxy;

namespace Krista.FM.RIA.Core.Helpers
{
    public class NHibernateJsonContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
            {
                return base.GetSerializableMembers(objectType.BaseType);
            }

            return base.GetSerializableMembers(objectType);
        }
    }
}
