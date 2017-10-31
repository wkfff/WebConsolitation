using System;

namespace Krista.FM.Client.ViewObjects.MessagesUI.AddressBook
{
    public class AddressBookElement : IComparable<AddressBookElement>
    {
        private readonly int id;
        private readonly string fullName;
        
        public AddressBookElement(int id, string fullName)
        {
            this.id = id;
            this.fullName = fullName;
        }

        public int Id
        {
            get { return id; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        #region ICompareable

        public int CompareTo(AddressBookElement other)
        {
            if (other == null)
            {
                return 1;
            }

            return FullName.CompareTo(other.FullName);
        }

        #endregion

        public override string ToString()
        {
            return FullName;
        }
    }
}
