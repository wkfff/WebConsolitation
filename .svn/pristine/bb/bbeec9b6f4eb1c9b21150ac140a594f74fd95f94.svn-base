using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using System.Data;

namespace Krista.FM.Server.FinSourcePlanning
{
    // делаем интерфейс с методами, позволяющими получать информацию о том, какие права даны на источники.
    // в классе, реализующим интерфейс добавить методы, устанавливающие доступ к данным, которые могут быть использованы в источниках


    /// <summary>
    /// класс, отвечающий за права доступа ко всем
    /// классификаторам и таблицам фактов, использующимся в источниках финансирования
    /// </summary>
    internal class FinSourcePlaningPermissionsHelper
    {
        private readonly IScheme scheme;
        private DataTable schemeObjects;
        private readonly int currentUserID;
        // список id объектов, для которых уже проводилась установка прав на видимость
        private List<int> chekedObjects;

        internal FinSourcePlaningPermissionsHelper(IScheme scheme)
        {
            this.scheme = scheme;
            schemeObjects = scheme.UsersManager.GetObjects();
            currentUserID = scheme.UsersManager.GetCurrentUserID();
            chekedObjects = new List<int>();
        }

        /// <summary>
        /// получение возможности отображения модулей источников финансирования
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <returns></returns>
        internal Boolean GetVisibleUIModule(string moduleKey)
        {
            // проверяем видимость модуля
            bool isVisible = scheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.View, false);
            if (!isVisible)
            {
                switch (moduleKey)
                {
                    case "Capitals":
                    case "CreditIncomes":
                    case "CreditIssued":
                    case "GuaranteeIssued":
                    case "ReferenceBooks":
                    case "CapitalOperations":
                        isVisible = scheme.UsersManager.CheckPermissionForSystemObject(moduleKey, (int)FinSourcePlaningUIModuleOperations.View, false);
                        break;
                    default:
                        isVisible = scheme.UsersManager.CheckPermissionForSystemObject(moduleKey, (int)FinSourcePlaningCalculateUIModuleOperations.View, false);
                        break;
                }
            }
            // если интерфейс видимый, то установим видимость классификаторов и таблиц фактов для текущего пользователя
            if (isVisible)
            {
                switch (moduleKey)
                {
                    case "Capitals":
                        SetPermissions2Capitals();
                        break;
                    case "CreditIncomes":
                        SetPermissions2CreditsIncomes();
                        break;
                    case "CreditIssued":
                        SetPermissions2CreditsIssued();
                        break;
                    case "GuaranteeIssued":
                        SetPermissions2Guarantee();
                        break;
                    case "ReferenceBooks":
                        SetPermissions2ReferenceBooks();
                        break;
                    case "DDE":
                        break;
                    case "BBK":
                        break;
                    case "VolumeHoldings":
                        SetPermissions2VolumeHoldings();
                        break;
                    case "RemainsDesign":
                        SetPermissions2RemainsDesign();
                        break;
                }
            }
            return isVisible;
        }

        /// <summary>
        /// настройка прав доступа к гарантиям предоставленным и всем классификаторам, что там используются
        /// </summary>
        private void SetPermissions2Guarantee()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_Guarantissued_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Organizations_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_OKV_Currency_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_TypeContract_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_KIF_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_EKR_PlanOutcomes_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_R_Plan_Key);
        }

        /// <summary>
        /// настройка прав доступа к кредитам полученным и всем классификаторам, что там используются
        /// </summary>
        private void SetPermissions2CreditsIncomes()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_Сreditincome_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Organizations_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_OKV_Currency_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_TypeContract_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Regions_Plan);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_KindBorrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_KIF_Plan_Key);
        }

        /// <summary>
        /// настройка прав доступа к ценным бумагам и всем классификаторам, что там используются
        /// </summary>
        private void SetPermissions2Capitals()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_Capital_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Organizations_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_Capital_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_OKV_Currency_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Regions_Plan);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_R_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_KIF_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_EKR_PlanOutcomes_Key);
        }

        /// <summary>
        /// настройка прав доступа к кредитам предоставленным и всем классификаторам, что там используются
        /// </summary>
        private void SetPermissions2CreditsIssued()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_Creditissued_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Organizations_Plan_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_OKV_Currency_Key);
            //SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_TypeContract_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Regions_Plan);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_KindBorrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_KIF_Plan_Key);
        }

        /// <summary>
        /// настройка прав доступа к справочникам и всем классификаторам, что там используются
        /// </summary>
        private void SetPermissions2ReferenceBooks()
        {
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_ExchangeRate_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Regions_Plan);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_JournalCB_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_KindBorrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_Constant_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_S_ViewContract_Key);
        }

        private void SetPermissions2VolumeHoldings()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_VolumeHoldings_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_PlanIncomes_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_PlanOutcomes_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_IF_Key);
        }

        private void SetPermissions2RemainsDesign()
        {
            SetVisiblePermissionForDactData(SchemeObjectsKeys.f_S_RemainsDesign);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_Borrow_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_PlanIncomes_Key);
            SetVisiblePermissionForDataClassifier(SchemeObjectsKeys.d_Variant_PlanOutcomes_Key);
        }

        private void SetVisiblePermissionForDataClassifier(string objectKey)
        {
            SetPermissionForObject(objectKey, "5001");
        }

        private void SetVisiblePermissionForDactData(string objectKey)
        {
            SetPermissionForObject(objectKey, "9001");
        }

        private void SetPermissionForObject(string schemeObjectKey, string permissionKey)
        {
            DataRow[] rows = schemeObjects.Select(string.Format("ObjectKey = '{0}'", schemeObjectKey));
            foreach (DataRow row in rows)
            {
                int objectID = Convert.ToInt32(row["ID"]);
                int objectType = Convert.ToInt32(row["ObjectType"]);
                if (chekedObjects.Contains(objectID))
                    continue;
                chekedObjects.Add(objectID);
                DataTable dtPermissions = scheme.UsersManager.GetUsersPermissionsForObject(objectID, objectType);
                DataRow[] permissions = dtPermissions.Select(string.Format("ID = {0}", currentUserID));
                foreach (DataRow permission in permissions)
                {
                    if (Convert.ToBoolean(permission[permissionKey]) || Convert.ToBoolean(permission["grp_" + permissionKey]))
                        continue;
                    permission[permissionKey] = true;
                }
                DataTable dtChanges = dtPermissions.GetChanges();
                if (dtChanges != null)
                    scheme.UsersManager.ApplayUsersPermissionsChanges(objectID, objectType, dtChanges);
            }
        }
    }
}
