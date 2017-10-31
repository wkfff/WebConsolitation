using System;
using System.Data;

namespace Krista.FM.Domain.Reporitory
{
    public class DataSetDataService : IDomainDataService
    {
        private readonly DataSet domainDataSet;

        public DataSetDataService(DataSet domainDataSet)
        {
            this.domainDataSet = domainDataSet;
        }

        public DataRow[] GetObjectData(Type objectType, string selectFilter)
        {
            return domainDataSet.Tables[objectType.Name].Select(selectFilter);
        }

        public void Create(DomainObject obj)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
