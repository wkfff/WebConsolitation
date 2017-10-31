using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinTabs;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    public partial class AdministrationUI : BaseViewObj, IInplaceTasksPermissionsView
    {
        #region обработчики для разименовки гридов

        #region Пользователи
        public static CC.GridColumnsStates GetUsersColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = false;
            state.IsSystem = true;
            state.ColumnPosition = 1;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Заблокирован";
            state.ColumnName = "BLOCKED";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            state.ColumnPosition = 2;
            state.ColumnWidth = 90;
            states.Add("BLOCKED", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Разрешена доменная аутентификация";
            state.ColumnName = "AllowDomainAuth";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            state.ColumnWidth = 90;
            state.ColumnPosition = 3;
            states.Add("AllowDomainAuth", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Имя DNS";
            state.ColumnName = "DNSNAME";
            state.IsHiden = false;
            state.ColumnWidth = 120;
            state.ColumnPosition = 4;
            states.Add("DNSNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Разрешена аутентификация логин/пароль";
            state.ColumnName = "AllowPwdAuth";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            state.ColumnWidth = 90;
            state.ColumnPosition = 5;
            states.Add("AllowPwdAuth", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Логин";
            state.ColumnName = "NAME";
            state.ColumnWidth = 120;
            state.IsReadOnly = false;
            state.ColumnPosition = 6;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "Caption";
            state.ColumnWidth = 250;
            state.IsHiden = true;
            state.ColumnPosition = 8;
            states.Add("Caption", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "Description";
            state.ColumnWidth = 300;
            state.ColumnPosition = 16;
            state.IsReadOnly = false;
            state.ColumnPosition = 9;
            states.Add("Description", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "USERTYPE";
            state.IsHiden = true;
            state.ColumnPosition = 10;
            states.Add("USERTYPE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Последний вход в систему";
            state.ColumnName = "LASTLOGIN";
            state.IsSystem = true;
            state.ColumnWidth = 160;
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
            state.ColumnPosition = 11;
            states.Add("LASTLOGIN", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Имя";
            state.ColumnName = "FIRSTNAME";
            state.ColumnWidth = 150;
            state.ColumnPosition = 12;
            states.Add("FIRSTNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Фамилия";
            state.ColumnName = "LASTNAME";
            state.ColumnWidth = 150;
            state.ColumnPosition = 13;
            states.Add("LASTNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Отчество";
            state.ColumnName = "PATRONYMIC";
            state.ColumnWidth = 150;
            state.ColumnPosition = 14;
            states.Add("PATRONYMIC", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Должность";
            state.ColumnName = "JOBTITLE";
            state.ColumnWidth = 150;
            state.ColumnPosition = 15;
            states.Add("JOBTITLE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "REFDEPARTMENTS";
            state.IsHiden = true;
            state.ColumnPosition = 16;
            states.Add("REFDEPARTMENTS", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "REFORGANIZATIONS";
            state.IsHiden = true;
            state.ColumnPosition = 17;
            states.Add("REFORGANIZATIONS", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Районы";
            state.Mask = "9999999999";
            state.ColumnName = "REFREGION";
            state.IsLookUp = true;
            state.IsReference = true;
            state.ColumnPosition = 18;
            states.Add("REFREGION", state);

            return states;
        }
        #endregion

        #region Группы
        public static CC.GridColumnsStates GetGroupsColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = false;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Заблокирована";
            state.ColumnName = "BLOCKED";
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            state.ColumnPosition = 2;
            states.Add("BLOCKED", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Наименование";
            state.ColumnPosition = 3;
            state.ColumnName = "NAME";
            state.ColumnWidth = 400;
            state.IsReadOnly = false;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "Caption";
            state.ColumnWidth = 250;
            state.IsHiden = true;
            state.ColumnPosition = 3;
            states.Add("Caption", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "Description";
            state.ColumnWidth = 300;
            state.ColumnPosition = 16;
            state.IsReadOnly = false;
            states.Add("Description", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Имя DNS";
            state.ColumnName = "DNSNAME";
            state.IsHiden = true;
            states.Add("DNSNAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип группы";
            state.ColumnName = "GROUPTYPE";
            state.ColumnPosition = 5;
            state.IsHiden = true;
            states.Add("GROUPTYPE", state);

            return states;
        }
        #endregion

        #region Объекты
        public static CC.GridColumnsStates GetObjectsColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = true;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Системный код";
            state.ColumnName = "OBJECTTYPE";
            state.ColumnWidth = 80;
            state.ColumnPosition = 1;
            state.IsSystem = true;
            states.Add("OBJECTTYPE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип объекта";
            state.ColumnName = "RUSOBJECTTYPE";
            state.ColumnWidth = 200;
            state.ColumnPosition = 2;
            state.IsSystem = true;
            states.Add("RUSOBJECTTYPE", state);

            state = new CC.GridColumnState();
            state.ColumnName = "REFOBJECTTYPE";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("REFOBJECTTYPE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Идентификатор";
            state.IsSystem = true;
            state.ColumnPosition = 3;
            state.ColumnName = "NAME";
            state.ColumnWidth = 400;
            state.IsReadOnly = false;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "Caption";
            state.ColumnWidth = 250;
            state.IsHiden = false;
            state.ColumnCaption = "Наименование";
            state.ColumnPosition = 3;
            state.IsSystem = true;
            states.Add("Caption", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "Description";
            state.ColumnWidth = 300;
            state.ColumnPosition = 16;
            state.IsReadOnly = false;
            states.Add("Description", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "";
            state.ColumnName = "OBJECTKEY";
            state.IsHiden = true;
            state.ColumnPosition = 18;
            states.Add("OBJECTKEY", state);

            return states;
        }
        #endregion

        #region Сессии
        public static CC.GridColumnsStates GetSessionsColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = true;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Логин";
            state.ColumnName = "SessionName";
            state.ColumnWidth = 200;
            states.Add("SessionName", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Время подключения";
            state.ColumnName = "LogonTime";
            state.ColumnWidth = 140;
            states.Add("LogonTime", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Машина";
            state.ColumnName = "Host";
            state.ColumnWidth = 100;
            states.Add("Host", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Приложение";
            state.ColumnName = "Application";
            state.ColumnWidth = 400;
            states.Add("Application", state);

            state = new CC.GridColumnState();
            state.IsHiden = true;
            state.ColumnCaption = "Количество выделеных ресурсов";
            state.ColumnName = "ResourcesCount";
            state.ColumnWidth = 200;
            states.Add("ResourcesCount", state);

            state = new CC.GridColumnState();
            state.IsHiden = true;
            state.ColumnCaption = "Тип сессии";
            state.ColumnName = "SessionType";
            state.ColumnWidth = 200;
            states.Add("SessionType", state);

            state = new CC.GridColumnState();
            state.IsHiden = true;
            state.ColumnCaption = "";
            state.ColumnName = "isBlocked";
            state.ColumnWidth = 20;
            states.Add("isBlocked", state);

            return states;
        }
        #endregion

        #region Отделы и организации
        public static CC.GridColumnsStates GetDivisionsAndOrganizationsColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = false;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Наименование";
            state.ColumnName = "NAME";
            state.ColumnWidth = 200;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "Description";
            state.ColumnWidth = 250;
            states.Add("Description", state);

            return states;
        }
        #endregion

        #region Типы задач
        public static CC.GridColumnsStates GetTasksTypesColumns()
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnCaption = "ID";
            state.ColumnName = "ID";
            state.ColumnWidth = 40;
            state.IsHiden = false;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Код";
            state.ColumnName = "CODE";
            state.ColumnWidth = 100;
            state.Mask = "9999999999";
            states.Add("CODE", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Описание";
            state.ColumnName = "Description";
            state.ColumnWidth = 250;
            states.Add("Description", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Наименование";
            state.ColumnName = "NAME";
            state.ColumnWidth = 250;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnCaption = "Тип задачи";
            state.ColumnName = "TASKTYPE";
            state.ColumnWidth = 150;
            state.ColumnPosition = 4;
            state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            states.Add("TASKTYPE", state);

            return states;
        }
        #endregion

        CC.GridColumnsStates ugeAllList_OnGetGridColumnsState(object sender)
        {
            // надо будет сделать вообще для всех одно 
            NavigationNodeKind nk = CurrentNavigationNode;

            if (nk == NavigationNodeKind.ndAllDirectoryes)
                return null;

            switch (nk)
            {
                case NavigationNodeKind.ndAllGroups:
                    return GetGroupsColumns();
                case NavigationNodeKind.ndAllObjects:
                    return GetObjectsColumns();
                case NavigationNodeKind.ndAllUsers:
                    return GetUsersColumns();
                case NavigationNodeKind.ndDivisions:
                    return GetDivisionsAndOrganizationsColumns();
                case NavigationNodeKind.ndOrganizations:
                    return GetDivisionsAndOrganizationsColumns();
                case NavigationNodeKind.ndSessions:
                    return GetSessionsColumns();
                case NavigationNodeKind.ndTasksTypes:
                    return GetTasksTypesColumns();
                default:
                    return null;
            }
        }

        CC.GridColumnsStates ugeUsersPermissions_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsSystem = true;
            state.ColumnWidth = 40;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "NAME";
            state.ColumnCaption = "Наименование";
            state.IsSystem = true;
            state.ColumnWidth = 150;
            states.Add("NAME", state);

            return states;
        }

        CC.GridColumnsStates ugeMembership_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsSystem = true;
            state.ColumnWidth = 40;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "NAME";
            state.ColumnCaption = "Наименование";
            state.IsSystem = true;
            state.ColumnWidth = 150;
            states.Add("NAME", state);

            return states;
        }

        CC.GridColumnsStates ugeGroupsPermissions_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsSystem = true;
            state.ColumnWidth = 40;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "NAME";
            state.ColumnCaption = "Наименование";
            state.IsSystem = true;
            state.ColumnWidth = 150;
            states.Add("NAME", state);

            return states;
        }

        CC.GridColumnsStates ugeAssignedPermissions_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsSystem = true;
            state.ColumnWidth = 40;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "NAME";
            state.ColumnCaption = "Наименование";
            state.IsSystem = true;
            state.ColumnWidth = 150;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnName = "ObjectID";
            state.ColumnCaption = String.Empty;
            states.Add("ObjectID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "AllowedAction";
            state.ColumnCaption = String.Empty;
            states.Add("AllowedAction", state);

            return states;
        }
        #endregion

    }
}
