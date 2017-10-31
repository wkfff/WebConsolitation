using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.ViewObjects.MessagesUI.AddressBook;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    class SchemeAddressBook : IAddressBook
    {
        private readonly IScheme scheme;

        public SchemeAddressBook(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public List<AddressBookElement> GetUsers()
        {
            List<AddressBookElement> list = new List<AddressBookElement>();
            DataTable table = scheme.UsersManager.GetUsers();
            foreach (DataRow row in table.Rows)
            {
                string fullName =
                    string.Format(
                        "{0} {1} {2} {3}",
                        row["LASTNAME"],
                        row["FIRSTNAME"],
                        row["PATRONYMIC"],
                        row["NAME"]).Trim();
                UserWrapper userWrapper = new UserWrapper(Convert.ToInt32(row["ID"]), fullName);
                list.Add(userWrapper);
            }

            list.Sort();

            return list;
        }

        public List<AddressBookElement> GetGroups()
        {
            List<AddressBookElement> list = new List<AddressBookElement>();
            DataTable table = scheme.UsersManager.GetGroups();
            foreach (DataRow row in table.Rows)
            {
                string fullName =
                    string.Format(
                        "{0} {1}",
                        "Группа",
                        row["NAME"]).Trim();
                GroupWrapper groupWrapper = new GroupWrapper(Convert.ToInt32(row["ID"]), fullName);
                list.Add(groupWrapper);
            }

            list.Sort();

            return list;
        }
    }
}
