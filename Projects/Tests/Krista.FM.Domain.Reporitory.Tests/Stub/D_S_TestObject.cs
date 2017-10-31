using System;

namespace Krista.FM.Domain.Reporitory.Tests
{
    public class D_S_TestObject : DomainObject
    {
        public static readonly string Key = "4d192956-aced-4718-a87c-b2e5519c022a";

        private int rowType;
        public int RowType
        {
            get { return rowType; }
            set { rowType = value; }
        }

        private int code;
        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private int? intNullable;
        public int? IntNullable
        {
            get { return intNullable; }
            set { intNullable = value; }
        }

        private Decimal numeric;
        public Decimal Numeric
        {
            get { return numeric; }
            set { numeric = value; }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private int refRegion;
        public int RefRegion
        {
            get { return refRegion; }
            set { refRegion = value; }
        }

    }
}
