using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    public partial class AddNewUser : Form
    {

        private string selectedDomainName;
        private IWorkplace currentWorkplace;

        DomainItemProperties[] SelectedUsers;

        private static Guid IID_IADsContainer = new Guid("{001677D0-FD16-11CE-ABC4-02608C9E7553}");

        private static Guid IID_IADsDomain = new Guid("{00E4C220-FD16-11CE-ABC4-02608C9E7553}");

        private static Guid IID_IADsUser = new Guid("{3E37E320-17E2-11CF-ABC4-02608C9E7553}");

        [DllImport("ActiveDS.dll", EntryPoint = "ADsOpenObject", PreserveSig = false, CharSet = CharSet.Unicode)]
        extern public static void ADsOpenObject(string lpszPath, string UserName, string Password,
            int dwReserved, ref Guid refIID, [MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

        enum ADsObjects { unknown, group, user };

        public DomainItemProperties[] GetSelectedUsers()
        {
            return SelectedUsers;
        }

        public DomainItemProperties[] GetSelectedUsers(DomainItemProperties[] users)
        {
            return SelectedUsers;
        }

        public AddNewUser(IWorkplace workplace)
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugUsers);
            currentWorkplace = workplace;
            GetObjects();
        }

        public static bool GetUsersFromDomain(IWorkplace workplace, out DomainItemProperties[] addUsers)
        {
            bool returnValue = false;
            addUsers = null;
            AddNewUser frmUsers = new AddNewUser(workplace);
            if (frmUsers.ShowDialog() == DialogResult.OK)
            {
                addUsers = frmUsers.GetSelectedUsers();
                if (addUsers.Length > 0)
                    returnValue = true;
            }
            frmUsers.Dispose();
            return returnValue;
        }

        /// <summary>
        /// разбивает полное имя на ФИО
        /// </summary>
        /// <param name="user"></param>
        private static void GetNamesFromFullName(ref DomainItemProperties user)
        {
            string userFullName = user.name;
            user.surname = userFullName.Split(' ')[0];
            if (userFullName.Split(' ').Length > 1)
                user.firstName = userFullName.Split(' ')[1];
            if (userFullName.Split(' ').Length > 2)
                user.patronymicName = userFullName.Split(' ')[2];
        }

        /// <summary>
        /// получает логин пользователя по его настройкам в AD
        /// </summary>
        /// <param name="ADsPath"></param>
        /// <returns></returns>
        private static string GetLogin(string ADsPath)
        {
            object user;
            Guid userGuid = IID_IADsUser;
            string userLogin = string.Empty;

            ADsOpenObject(ADsPath, null, null, 1, ref userGuid, out user);

            if (user != null)
            {
                userLogin = ((IADsUser)user).Get("sAMAccountName").ToString();
            }
            user = null;
            return userLogin;
        }

        /// <summary>
        /// получение доменов WinNT
        /// </summary>
        void GetObjects()
        {
            Object pADObject;
            Guid domain = IID_IADsDomain;
            Guid container = IID_IADsContainer;
            IADsContainer cont;
            try
            {
                // получаем список всех объектов Active Directory через протокол WinNT
                ADsOpenObject("WinNT:", null, null, 1, ref container, out pADObject);
                if (pADObject != null)
                {
                    cont = (IADsContainer)pADObject;
                    IEnumerator enumer = cont.GetEnumerator();
                    enumer.MoveNext();
                    dsUsers.Tables[0].BeginLoadData();
                    // выбираем домены
                    while (enumer.Current != null)
                    {
                        if (enumer.Current is IADsDomain)
                        {
                            Object dom = enumer.Current;
                            DataRow row = dsUsers.Tables[0].Rows.Add(null, null);
                            row["Name"] = ((IADsDomain)dom).Name;
                            row["ID"] = dsUsers.Tables[0].Rows.Count - 1;
                        }
                        enumer.MoveNext();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dsUsers.Tables[0].EndLoadData();
            }
        }

        /// <summary>
        /// получение всех групп указанного домена или пользователей группы
        /// </summary>
        /// <param name="DomainName"></param>
        /// <returns></returns>
        private void GetADSObjects(string ADsPath, string objectFilter, int parentRowID, ADsObjects AdsObject)
        {
            currentWorkplace.OperationObj.Text = "Получение и обработка данных";
            currentWorkplace.OperationObj.StartOperation();
            DirectoryEntry entry = null;
            try
            {
                dsUsers.Tables[(int)AdsObject].BeginLoadData();
                if (AdsObject == ADsObjects.user)
                {
                    DirectoryEntry groupEntry = new DirectoryEntry(ADsPath);
                    // создаем объект поиска в Active Directory 
                    DirectorySearcher groupSearch = new DirectorySearcher(groupEntry);
                    // ставим фильтр на эту группу для получения из нее пользователей
                    groupSearch.Filter = "(&(objectClass=organizationalUnit))";
                    // ищем группу
                    SearchResult groupResult = groupSearch.FindOne();
                    if (groupResult != null)
                        entry = groupResult.GetDirectoryEntry();
                }
                else
                {
                    // получаем объект DirectoryEntry по данному пути
                    entry = new DirectoryEntry(string.Format("LDAP://{0}", ADsPath));
                }
                // создаем объект поиска в Active Directory
                DirectorySearcher search = new DirectorySearcher(entry);
                // устанавливаем фильтр на группы
                search.Filter = objectFilter;
                // указываем, какие параметры групп нам нужны
                search.PropertiesToLoad.Add("Name");
                search.PropertiesToLoad.Add("Description");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("title");
                search.PropertiesToLoad.Add("ADsPath");
                SearchResultCollection resultCol = null;
                // осуществляем поиск
                try
                {
                    resultCol = search.FindAll();
                }
                catch
                {
                    // ничего не получилось
                    dsUsers.Tables[(int)AdsObject].EndLoadData();
                    currentWorkplace.OperationObj.StopOperation();
                    string strMessage = string.Empty;
                    UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(ugUsers);
                    if (AdsObject == ADsObjects.group)
                    {
                        strMessage = string.Format("Нет прав для получения списка групп домена '{0}'", activeRow.Cells["Name"].Value);
                    }
                    else
                        strMessage = string.Format("Нет прав для получения списка пользователей группы '{}'", activeRow.Cells["Name"].Value);
                    MessageBox.Show(strMessage, "Импорт пользователей", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ugUsers.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                // если нашли
                if (resultCol != null)
                {
                    // добавляем данные по группам в грид
                    foreach (SearchResult result in resultCol)
                    {
                        DataRow childRow = dsUsers.Tables[(int)AdsObject].Rows.Add(null, null);
                        childRow.BeginEdit();
                        if (result.Properties.Contains("Name"))
                            childRow["Name"] = ((String)result.Properties["Name"][0]);
                        if (result.Properties.Contains("Description"))
                            childRow["Description"] = ((String)result.Properties["Description"][0]);
                        if (result.Properties.Contains("mail"))
                            childRow["Mail"] = ((String)result.Properties["mail"][0]);
                        if (result.Properties.Contains("ADsPath"))
                            childRow["ADsPath"] = ((String)result.Properties["ADsPath"][0]);

                        childRow["ID"] = dsUsers.Tables[(int)AdsObject].Rows.Count - 1;
                        childRow["refID"] = parentRowID;
                        childRow.EndEdit();
                    }
                }
            }
            finally
            {
                dsUsers.Tables[(int)AdsObject].EndLoadData();
                ugUsers.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                //parentRow.Activate();
                currentWorkplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// получает всех пользователей из домена и формирует список пользователей с параметрами
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="users"></param>
        private void GetUsersFromDomain(string Domain, out DomainItemProperties[] users)
        {
            currentWorkplace.OperationObj.Text = "Получение и обработка данных";
            currentWorkplace.OperationObj.StartOperation();
            users = null;
            try
            {
                // получаем объект DirectoryEntry для выбранного домена
                DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", Domain));
                // создаем объект поиска в Active Directory
                DirectorySearcher search = new DirectorySearcher(entry);
                // устанавливаем фильтр на группы
                search.Filter = "(&(objectClass=user)(objectCategory=Person))";
                // указываем, какие параметры групп нам нужны
                search.PropertiesToLoad.Add("Name");
                search.PropertiesToLoad.Add("Description");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("title");
                search.PropertiesToLoad.Add("ADsPath");
                // осуществляем поиск
                SearchResultCollection resultCol = search.FindAll();
                ugUsers.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                // если нашли
                if (resultCol != null)
                {
                    users = new DomainItemProperties[resultCol.Count];
                    // добавляем данные по группам или пользователям в грид
                    int index = 0;
                    foreach (SearchResult result in resultCol)
                    {
                        if (result.Properties.Contains("Name"))
                        {
                            users[index].name = ((String)result.Properties["Name"][0]);
                            GetNamesFromFullName(ref users[index]);
                        }
                        if (result.Properties.Contains("Description"))
                            users[index].description = ((String)result.Properties["Description"][0]);
                        if (result.Properties.Contains("mail"))
                            users[index].mail = ((String)result.Properties["mail"][0]);
                        if (result.Properties.Contains("title"))
                            users[index].title = ((String)result.Properties["title"][0]);
                        if (result.Properties.Contains("ADsPath"))
                        {
                            string ADsPath = ((String)result.Properties["ADsPath"][0]);
                            users[index].Login = Domain + "\\" + GetLogin(ADsPath);
                        }
                        index++;
                    }
                }
            }
            finally
            {
                ugUsers.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                currentWorkplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// добавление пользователей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUsers_Click(object sender, EventArgs e)
        {
            if (ugUsers.ActiveRow != null)
            {
                Guid userGuid = IID_IADsUser;
                //string ADsPath = string.Empty;
                switch (ugUsers.ActiveRow.Band.Index)
                {
                    // добавляем пользователей всего домена
                    case 0:
                        GetUsersFromDomain(ugUsers.ActiveRow.Cells["NAME"].Value.ToString(), out SelectedUsers);
                        break;
                    // добавляем выбранную группу
                    case 1:
                        // создаем список пользователей на число их в группе
                        SelectedUsers = new DomainItemProperties[ugUsers.ActiveRow.ChildBands[0].Rows.Count];
                        // добавляем этих самых пользователей
                        foreach (UltraGridRow row in ugUsers.ActiveRow.ChildBands[0].Rows)
                        {
                            SetUserParamsFromUltraRow(row, ref SelectedUsers[row.Index]);
                        }
                        break;
                    // добавляем отдельного пользователя
                    case 2:
                        // добавляем конкретного выбранного пользователя
                        SelectedUsers = new DomainItemProperties[1];
                        SetUserParamsFromUltraRow(ugUsers.ActiveRow, ref SelectedUsers[0]);
                        break;
                }
            }
            dsUsers.AcceptChanges();
        }

        private void SetUserParamsFromUltraRow(UltraGridRow row, ref DomainItemProperties user)
        {
            user.name = row.Cells["name"].Value.ToString();
            GetNamesFromFullName(ref user);
            user.mail = row.Cells["mail"].Value.ToString();
            user.description = row.Cells["description"].Value.ToString();
            // нужно получить логин из пользователя
            string ADsPath = row.Cells["ADsPath"].Value.ToString();
            user.Login = selectedDomainName + "\\" + GetLogin(ADsPath);
        }

        /// <summary>
        /// действия при выборе домена или группы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugUsers_AfterRowActivate(object sender, EventArgs e)
        {
            
        }

        //********************************************************************************//

        private void ugUsers_AfterRowExpanded(object sender, RowEventArgs e)
        {
            //UltraGridRow row = e.Row;
            e.Row.Activate();

            UltraGridRow row = UltraGridHelper.GetRowCells(e.Row);
            if (row.Cells.Exists("wasSelected"))
                if (row.Cells["wasSelected"].Value == DBNull.Value)
                {
                    //ugUsers.BeginUpdate();
                    switch (row.Band.Index)
                    {
                        // загружаем все группы домена
                        case 0:
                            GetADSObjects(row.Cells["NAME"].Value.ToString(), "(&(objectClass=organizationalUnit))", Convert.ToInt32(row.Cells["ID"].Value), ADsObjects.group);
                            selectedDomainName = row.Cells["NAME"].Value.ToString();
                            break;
                        // загружаем всех пользователей группы
                        case 1:
                            GetADSObjects(row.Cells["ADsPath"].Value.ToString(), "(&(objectClass=user)(objectCategory=Person))", Convert.ToInt32(row.Cells["ID"].Value), ADsObjects.user);
                            break;
                    }
                    row.Cells["wasSelected"].Value = 1;
                    //ugUsers.EndUpdate();
                }
            //row.Activate();
            row.Selected = true;
            //row.Expanded = true;

            btnAddUsers.Enabled = true;
            row.Update();
        }
    }

    /// <summary>
    /// структура для передачи параметров, полученных из AD
    /// </summary>
    public struct DomainItemProperties
    {
        public string name;

        public string firstName;

        public string surname;

        public string patronymicName;

        public string description;

        public string mail;

        public string title;

        public string ADsPath;

        public string Login;
    }
}