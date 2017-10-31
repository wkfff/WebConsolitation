using System.Collections.Generic;

namespace Krista.FM.Client.ViewObjects.MessagesUI.AddressBook
{
    public interface IAddressBook
    {
        List<AddressBookElement> GetUsers();
        List<AddressBookElement> GetGroups();
    }
}
