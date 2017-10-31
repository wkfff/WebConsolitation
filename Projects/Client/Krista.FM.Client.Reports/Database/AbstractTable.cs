namespace Krista.FM.Client.Reports.Database
{
    public class AbstractTable
    {
        private readonly string tableKey = "";

        public virtual string InternalKey 
        {
            get { return tableKey; }
        }

        public AbstractTable()
        {
        }

        public AbstractTable(string objectKey)
        {
            tableKey = objectKey;
        }
    }
}
