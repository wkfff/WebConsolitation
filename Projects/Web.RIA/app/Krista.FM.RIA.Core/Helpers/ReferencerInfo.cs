using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Core.Helpers
{
    public class ReferencerInfo
    {
        private readonly string ObjectKey;
        private readonly List<FieldInfo> PrimaryFields;
        private readonly List<FieldInfo> SecondaryFields;

        public ReferencerInfo(string objectKey, List<FieldInfo> primaryFields, List<FieldInfo> secondaryFields)
        {
            ObjectKey = objectKey;
            PrimaryFields = primaryFields;
            SecondaryFields = secondaryFields;
        }

        public string GetObjectKey()
        {
            return ObjectKey;
        }

        public List<FieldInfo> GetPrimaryFields()
        {
            return PrimaryFields;   
        }

        public List<FieldInfo> GetSecondaryFields()
        {
            return SecondaryFields;
        }

        public class FieldInfo
        {
            public string Caption { get; set; }

            public string Name { get; set; }
        }
    }
}
