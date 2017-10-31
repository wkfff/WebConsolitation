using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;


namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Различные проверки вызываемые при загрузке объекта
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        public int RegisterSystemObject(IDatabase externalDb, string name, string caption, SysObjectsTypes objectType)
        {
            if ((name == null) || (name == String.Empty))
                throw new Exception("Должно быть задано имя объекта");

            int newID = 0;
            lock (syncObjects)
            {
                // ищем объект во внутренней таблице
                string filter = String.Format("NAME = '{0}'", name);
                DataRow[] sysObj = ObjectsTable.Select(filter);
                switch (sysObj.Length)
                {
                    case 0:
                        using (Database db = (Database) _scheme.SchemeDWH.DB)
                        {
                            newID = db.GetGenerator("g_OBJECTS");
                        }
                        DataRow newObj = ObjectsTable.Rows.Add();
                        newObj["ID"] = newID;
                        newObj["OBJECTKEY"] = name;
                        newObj["NAME"] = name;
                        newObj["CAPTION"] = caption;
                        newObj["OBJECTTYPE"] = objectType;
                        SetParentType(newObj);
                        break;
                        // если поменялся заголовок или тип объекта - тоже меняем
                    case 1:
                        if (Convert.ToString(sysObj[0]["CAPTION"]) != caption)
                            sysObj[0]["CAPTION"] = caption;
                        if (Convert.ToInt32(sysObj[0]["OBJECTTYPE"]) != (int) objectType)
                            sysObj[0]["OBJECTTYPE"] = (int) objectType;
                        break;
                    default:
                        throw new ServerException(String.Format("Обнаружено {0} объекта с одинаковым именем {1}",
                                                                sysObj.Length, name));
                }

                // сохраняем изменения в базе
                DataTable objectsChanges = ObjectsTable.GetChanges();
                if ((objectsChanges != null) && (objectsChanges.Rows.Count > 0))
                {
                    using (DataUpdater upd = GetObjectsUpdater(externalDb))
                    {
                        try
                        {
                            upd.Update(ref objectsChanges);
                            ObjectsTable.AcceptChanges();
                        }
                        catch (Exception e)
                        {
                            ObjectsTable.RejectChanges();
                            throw new ServerException(e.Message, e);
                        }
                    }
                }
            }
            return newID;
        }

        /// <summary>
        /// Зарегистрировать объект системы в списке объектов
        /// </summary>
        /// <param name="name">Уникальное в пределах схемы имя объекта</param>
        /// <param name="caption">Описание объекта</param>
        /// <param name="objectType">Тип объекта</param>
        public int RegisterSystemObject(string name, string caption, SysObjectsTypes objectType)
        {
            return RegisterSystemObject(null, name, caption, objectType);
        }

        /// <summary>
        /// Удалить описание объекта из списка объектов системы
        /// </summary>
        /// <param name="name">Уникальное в пределах схемы имя объекта</param>
        public void UnregisterSystemObject(string name)
        {
            if ((name == null) || (name == String.Empty))
                throw new Exception("Должно быть задано имя объекта");

            lock (syncObjects)
            {
                // ищем объект во внутренней таблице
                string nameUp = name.ToUpper();
                string filter = String.Format("NAME = '{0}'", nameUp);
                DataRow[] sysObj = ObjectsTable.Select(filter);
                if (sysObj.Length == 1)
                {
                    sysObj[0].Delete();
                    // сохраняем изменения в базе
                    DataTable objectsChanges = ObjectsTable.GetChanges();
                    if ((objectsChanges != null) && (objectsChanges.Rows.Count > 0))
                    {
                        using (DataUpdater upd = GetObjectsUpdater(null))
                        {
                            try
                            {
                                upd.Update(ref objectsChanges);
                                ObjectsTable.AcceptChanges();
                            }
                            catch (Exception e)
                            {
                                ObjectsTable.RejectChanges();
                                throw new ServerException(e.Message, e);
                            }
                        }
                    }
                }
            }
        }

        private void RegisterGeneralObjectsTypes()
        {
            SysObjectsTypes tp = SysObjectsTypes.AllUIModules;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllDataClassifiers;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllAssociatedClassifiers;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AssociateForAllClassifiers;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllFactTables;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllDataPumps;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllDataSources;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllTasks;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
			tp = SysObjectsTypes.AllTemplates;
			RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
			tp = SysObjectsTypes.AllForecast;
			RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
			tp = SysObjectsTypes.Form2pForecast;
			RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.FinSourcePlaning;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.AllMessages;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);
            tp = SysObjectsTypes.IncomesPlaning;
            RegisterSystemObject(tp.ToString(), TypesHelper.GetCaptionForObjectType(tp), tp);

            #region регистрация объектов классификаторов и таблиц;
            // перерегистрируем блок классификаторов с новым типом
            tp = SysObjectsTypes.EntityNavigationListUI;
            RegisterSystemObject("EntityNavigationListUI", "Классификаторы и таблицы", tp);
            tp = SysObjectsTypes.UIClassifiersSubmodule;
            RegisterSystemObject("FixedClassifiers", "Фиксированный классификаторы", tp);
            RegisterSystemObject("DataClassifiers", "Классификаторы данных", tp);
            RegisterSystemObject("AssociatedClassifiers", "Сопоставимые классификаторы", tp);
            RegisterSystemObject("FactTables", "Таблицы фактов", tp);
            RegisterSystemObject("Associations", "Сопоставление классификаторов", tp);
            RegisterSystemObject("TranslationTables", "Таблицы перекодировок", tp);
            #endregion

            #region регистрация объектов источников финансирования
            // регистрируем объекты источников финансирования
            RegisterSystemObject("Capitals", "Ценные бумаги", SysObjectsTypes.FinSourcePlaningUIModule);
            RegisterSystemObject("CreditIssued", "Кредиты предоставленные", SysObjectsTypes.FinSourcePlaningUIModule);
            RegisterSystemObject("CreditIncomes", "Кредиты полученные", SysObjectsTypes.FinSourcePlaningUIModule);
            RegisterSystemObject("GuaranteeIssued", "Гарантии", SysObjectsTypes.FinSourcePlaningUIModule);
            RegisterSystemObject("VolumeHoldings", "Определение объема заимствований", SysObjectsTypes.FinSourcePlaningCalculateUIModule);
            RegisterSystemObject("RemainsDesign", "Расчет остатков средств бюджета", SysObjectsTypes.FinSourcePlaningCalculateUIModule);
            RegisterSystemObject("DDE", "ДДЕ", SysObjectsTypes.FinSourcePlaningCalculateUIModule);
            RegisterSystemObject("BBK", "Оценка проекта бюджета", SysObjectsTypes.FinSourcePlaningCalculateUIModule);
            RegisterSystemObject("ReferenceBooks", "Справочники", SysObjectsTypes.FinSourcePlaningUIModule);
            RegisterSystemObject("CapitalOperations", "Планирование операций с ЦБ", SysObjectsTypes.FinSourcePlaningCalculateUIModule);
            #endregion

            #region регистрация типов сообщений

            RegisterMessagesType();

            #endregion

            #region регистрация планирования доходов

            RegisterSystemObject("IncomesSplit", "Расщепление доходов", SysObjectsTypes.IncomesPlaningModule);
            RegisterSystemObject("IncomesYearPlan", "Годовой план по доходам", SysObjectsTypes.IncomesPlaningModule);
            RegisterSystemObject("Balance", "Балансировка", SysObjectsTypes.IncomesPlaningModule);
            RegisterSystemObject("IncomesEvalPlan", "Оценка и прогноз по доходам", SysObjectsTypes.IncomesPlaningModule);
            RegisterSystemObject("TaxpayersSum", "Суммы НП к доплате (уменьшению)", SysObjectsTypes.IncomesPlaningModule);
            RegisterSystemObject("PriorForecast", "Предварительный прогноз по доходам", SysObjectsTypes.IncomesPlaningModule);

            #endregion
        }

        private void RegisterMessagesType()
        {
            foreach (var value in Enum.GetValues(typeof(MessageType)))
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
                MessageTypeObjectKey[] messageTypeObjectKeys =
                    (MessageTypeObjectKey[]) fi.GetCustomAttributes(typeof (MessageTypeObjectKey),
                                                                     false);
                string enumDescription = attributes.Length > 0 ? attributes[0].Description : value.ToString();
                string enumObjectKey = messageTypeObjectKeys.Length > 0 ? messageTypeObjectKeys[0].ObjectKey : value.ToString();

                RegisterSystemObject(enumObjectKey, enumDescription, SysObjectsTypes.Message);
            }
        }

        /// <summary>
        /// Возвращает список зарегистрированных модулей в системе.
        /// </summary>
        /// <param name="getFullNames">
        /// Если true, то в качестве значения возвращает полное имя класса, 
        /// иначе возвращает описание.</param>
        /// <returns>[Уникальное имя, Описание или полное имя класса]</returns>
        private Dictionary<string, string> GetUIModulesList(bool getFullNames)
        {
            Dictionary<string, string> _uiModulesList = new Dictionary<string, string>();

            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery("select ID, FullName, Name, Description from RegisteredUIModules order by ID", QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    _uiModulesList.Add(
                        Convert.ToString(row["Name"]),
                        getFullNames ? Convert.ToString(row["FullName"]) : Convert.ToString(row["Description"]));
                }
            }
            return _uiModulesList;
        }

        private void RegisterUIModules()
        {
            Dictionary<string, string> uiModules = this.GetUIModulesList(false);
            foreach (string uiModule in uiModules.Keys)
            {
                string uiModuleCaption = uiModules[uiModule];
                RegisterSystemObject(uiModule, uiModuleCaption, SysObjectsTypes.UIModule);
            }
        }

        private const string ADMINISTRATORS_GROUP_NAME = "Администраторы";
        private const string ADMINISTRATORS_GROUP_CAPTION = "Администраторы системы";

        private const string WEBADMINISTRATORS_GROUP_NAME = "Web-администраторы";
        private const string WEBADMINISTRATORS_GROUP_CAPTION = "Web-администраторы системы";

        private int? administratorsGroupsObjID = null;
        private int? webAdministratorsGroupsObjID = null;

        private void CheckAdministrators()
        {
			using (Database db = (Database)_scheme.SchemeDWH.DB)
            {
                #region проверяем наличие группы администраторов
                // .. получаем все группы
				DataTable dt = new DataTable();
				using (DataUpdater du = GetGroupsUpdater())
                {
					du.Fill(ref dt);
                }
                // .. фильтруем по константному имени
				string filter = String.Format("(NAME='{0}')", ADMINISTRATORS_GROUP_NAME);
				DataRow[] filtered = dt.Select(filter);
                // .. если не нашли - добавляем
                if (filtered.Length == 0)
                {
                    administratorsGroupsObjID = db.GetGenerator("g_Groups");
                    dt.Rows.Add(administratorsGroupsObjID, ADMINISTRATORS_GROUP_NAME, ADMINISTRATORS_GROUP_CAPTION, 0, String.Empty);
                    ApplayGroupsChanges(dt.GetChanges());
                    dt.AcceptChanges();
                }
                // .. если нашли - запоминаем его ID
                else
                {
                    administratorsGroupsObjID = Convert.ToInt32(filtered[0]["ID"]);
                }

                // web-администраторы
                // .. фильтруем по константному имени
                filter = String.Format("(NAME='{0}')", WEBADMINISTRATORS_GROUP_NAME);
                DataRow[] webFiltered = dt.Select(filter);
                // .. если не нашли - добавляем
                if (webFiltered.Length == 0)
                {
                    webAdministratorsGroupsObjID = db.GetGenerator("g_Groups");
                    dt.Rows.Add(webAdministratorsGroupsObjID, WEBADMINISTRATORS_GROUP_NAME,
                                WEBADMINISTRATORS_GROUP_CAPTION, 0, String.Empty);
                    ApplayGroupsChanges(dt.GetChanges());
                    dt.AcceptChanges();
                }
                // .. если нашли - запоминаем его ID
                else
                {
                    webAdministratorsGroupsObjID = Convert.ToInt32(webFiltered[0]["ID"]);
                }

                #endregion
            }
        }

        private void AddOrUpdateAllTypePermissions(IDatabase db, Type AllOperationsType, int userID, int objID)
        {
            Array allOperations = Enum.GetValues(AllOperationsType);
            foreach (int op in allOperations)
            {
                if (!CheckPermission(userID, objID, op, true))
                {
                    DataRow newPermission = PermissionsTable.Rows.Add();
                    newPermission["ID"] = db.GetGenerator("G_PERMISSIONS");
                    newPermission["REFOBJECTS"] = objID;
                    newPermission["ALLOWEDACTION"] = op;
                    newPermission["REFUSERS"] = userID;
                }
            }
        }

        private void CheckFixedUsers()
        {
            // проверяем назначены ли пользователю DataPump права на закачки
            SysUser user = Users[(int)FixedUsers.FixedUsersIds.DataPump];

            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                // добавляем права на все операции с закачками
                int allPumpsObjID = GetSystemObjectIDByName(SysObjectsTypes.AllDataPumps.ToString());
                AddOrUpdateAllTypePermissions(db, typeof(AllDataPumpsOperations), user.ID, allPumpsObjID);
                // .. классификаторы данных
                int allDataClassifiersObjID = GetSystemObjectIDByName(SysObjectsTypes.AllDataClassifiers.ToString());
                AddOrUpdateAllTypePermissions(db, typeof(AllDataClassifiersOperations), user.ID, allDataClassifiersObjID);
                // .. сопоставимые классификаторы
                int allAssociatedClassifiersObjID = GetSystemObjectIDByName(SysObjectsTypes.AllAssociatedClassifiers.ToString());
                AddOrUpdateAllTypePermissions(db, typeof(AllAssociatedClassifiersOperations), user.ID, allAssociatedClassifiersObjID);
                // .. таблицы фактов
                int allFactTablesObjID = GetSystemObjectIDByName(SysObjectsTypes.AllFactTables.ToString());
                AddOrUpdateAllTypePermissions(db, typeof(AllFactTablesOperations), user.ID, allFactTablesObjID);
                // .. сопоставление для всех классификаторов
                int allAssociateObjID = GetSystemObjectIDByName(SysObjectsTypes.AssociateForAllClassifiers.ToString());
                AddOrUpdateAllTypePermissions(db, typeof(AssociateForAllClassifiersOperations), user.ID, allAssociateObjID);
            }
            FlushPermissionsChangesToDB();
        }
    }
}