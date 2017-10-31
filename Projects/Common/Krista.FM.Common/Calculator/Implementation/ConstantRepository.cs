using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class ConstantRepository : IConstantRepository
    {
        private readonly ICollection<IConstant> data;

        public ConstantRepository()
        {
            data = new Collection<IConstant>();
        }

        #region IConstantRepository

        public IEnumerable<IConstant> FindAll()
        {
            return data.AsEnumerable();
        }

        public IConstant FindOne(string name)
        {
            return data.FirstOrDefault(x => x.Name == name);
        }

        #endregion

        public void Add(IConstant constant)
        {
            if (FindOne(constant.Name) != null)
            {
                throw new DuplicateIdentifierException(constant.Name);
            }
            data.Add(constant);
        }

        public void Drop(IConstant constant)
        {
            if (FindOne(constant.Name) == null)
            {
                throw new UndefinedIdentifierException(constant.Name);
            }
            data.Add(constant);
        }
    }
}
