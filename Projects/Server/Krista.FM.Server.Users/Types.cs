using System;
using System.Reflection;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Users
{
    public struct TypesHelper
    {
        public static string GetCaptionForObjectType(SysObjectsTypes tp)
        {
        	return EnumHelper.GetEnumItemDescription(typeof(SysObjectsTypes), tp);
        }

		private static Type GetOperationsEnumForType(SysObjectsTypes tp)
		{
			return AssociateEnumTypeAttribute.GetEnumItemAssociateType(typeof(SysObjectsTypes), tp);
		}

		public static void GetOperationInfo(int operation, ref string operationCaption, ref Type operationType, 
            ref int? parentOperation)
        {
            operationCaption = "����������� ��������";
            operationType = null;
            parentOperation = null;

            switch (operation)
            {
                // �������� �� �����������������
                case (int)AdministrationOperations.PermissionsManagement:
                    operationType = typeof(AdministrationOperations);
                    operationCaption = "���������� �������";
                    break;

                // �������� ��� ���� ���������������� �����������
                case (int)AllUIModulesOperations.DisplayAll:
                    operationType = typeof(AllUIModulesOperations);
                    operationCaption = "����������� ���� ������ � ����������";
                    break;

                // �������� ��� ����������������� ����������
                case (int)UIModuleOperations.Display:
                    operationType = typeof(UIModuleOperations);
                    parentOperation = (int)AllUIModulesOperations.DisplayAll;
                    operationCaption = "����������� ����� � ����������";
                    break;
                // �������� ��� ���������� "�������������� � �������"
                case (int)EntityNavigationListUI.Display:
                    operationType = typeof(EntityNavigationListUI);
                    parentOperation = (int)AllUIModulesOperations.DisplayAll;
                    operationCaption = "����������� ����� � ����������";
                    break;
                // �������� ��� ��������� ���������� "�������������� � �������"
                case (int)UIClassifiersSubmoduleOperation.Display:
                    operationType = typeof(UIClassifiersSubmoduleOperation);
                    parentOperation = (int)EntityNavigationListUI.Display;
                    operationCaption = "����������� ����� � ����������";
                    break;

                // �������� ��� ���� ��������������� ������
                case (int)AllDataClassifiersOperations.ViewClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "�������� ��������������";
                    break;
                case (int)AllDataClassifiersOperations.AddRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "���������� ������ � �������������";
                    break;
                case (int)AllDataClassifiersOperations.DelRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "�������� ������ ��������������";
                    break;
                case (int)AllDataClassifiersOperations.EditRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "�������������� ������ ��������������";
                    break;
                case (int)AllDataClassifiersOperations.AddClassifierForNewDataSource:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "���������� �������������� �� ������ ���������";
                    break;
                case (int)AllDataClassifiersOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "��������� �������� � ��������������";
                    break;
                case (int)AllDataClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "��������� �������� � �������������� � ����������� ����";
                    break;
                case (int)AllDataClassifiersOperations.ClearClassifierData:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "������� ��������������";
                    break;
                case (int)AllDataClassifiersOperations.ImportClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "������ ��������������";
                    break;

                // �������� ��� �������������� ������
                case (int)DataClassifiesOperations.ViewClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "�������� ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.ViewClassifier;
                    break;
                case (int)DataClassifiesOperations.AddRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "���������� ������ � �������������";
                    parentOperation = (int)AllDataClassifiersOperations.AddRecord;
                    break;
                case (int)DataClassifiesOperations.DelRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "�������� ������ ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.DelRecord;
                    break;
                case (int)DataClassifiesOperations.EditRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "�������������� ������ ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.EditRecord;
                    break;
                case (int)DataClassifiesOperations.AddClassifierForNewDataSource:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "���������� �������������� �� ������ ���������";
                    parentOperation = (int)AllDataClassifiersOperations.AddClassifierForNewDataSource;
                    break;
                case (int)DataClassifiesOperations.ChangeClassifierHierarchy:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "��������� �������� � ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.ChangeClassifierHierarchy;
                    break;
                case (int)DataClassifiesOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "��������� �������� � �������������� � ����������� ����";
                    parentOperation = (int)AllDataClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier;
                    break;
                case (int)DataClassifiesOperations.ClearClassifierData:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "������� ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.ClearClassifierData;
                    break;
                case (int)DataClassifiesOperations.ImportClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "������ ��������������";
                    parentOperation = (int)AllDataClassifiersOperations.ImportClassifier;
                    break;

                // �������� ��� ���� ������������ ���������������
                case (int)AllAssociatedClassifiersOperations.ViewClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "�������� ��������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.AddRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "���������� ������ � �������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.DelRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "�������� ������ ��������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.EditRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "�������������� ������ ��������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "��������� �������� � ��������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "��������� �������� � �������������� � ����������� ����";
                    break;
                case (int)AllAssociatedClassifiersOperations.ClearClassifierData:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "������� ��������������";
                    break;
                case (int)AllAssociatedClassifiersOperations.ImportClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "������ ��������������";
                    break;

                // �������� ��� ������������� ��������������
                case (int)AssociatedClassifierOperations.ViewClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "�������� ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ViewClassifier;
                    break;
                case (int)AssociatedClassifierOperations.AddRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "���������� ������ � �������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.AddRecord;
                    break;
                case (int)AssociatedClassifierOperations.DelRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "�������� ������ ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.DelRecord;
                    break;
                case (int)AssociatedClassifierOperations.EditRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "�������������� ������ ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.EditRecord;
                    break;
                case (int)AssociatedClassifierOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "��������� �������� � ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ChangeClassifierHierarchy;
                    break;
                case (int)AssociatedClassifierOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "��������� �������� � �������������� � ����������� ����";
                    parentOperation = (int)AllAssociatedClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier;
                    break;
                case (int)AssociatedClassifierOperations.ClearClassifierData:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "������� ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ClearClassifierData;
                    break;
                case (int)AssociatedClassifierOperations.ImportClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "������ ��������������";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ImportClassifier;
                    break;

                // �������� ��� ���� ������ ������
                case (int)AllFactTablesOperations.ViewClassifier:
                    operationType = typeof(AllFactTablesOperations);
                    operationCaption = "�������� ������";
                    break;
                case (int)AllFactTablesOperations.EditRecord:
                    operationType = typeof(AllFactTablesOperations);
                    operationCaption = "�������������� ������";
                    break;

                // �������� ��� ������� ������
                case (int)FactTableOperations.ViewClassifier:
                    operationType = typeof(FactTableOperations);
                    operationCaption = "�������� ������";
                    parentOperation = (int)AllFactTablesOperations.ViewClassifier;
                    break;
                case (int)FactTableOperations.EditRecord:
                    operationType = typeof(FactTableOperations);
                    operationCaption = "�������������� ������";
                    parentOperation = (int)AllFactTablesOperations.EditRecord;
                    break;

                // �������� ��� ������������� ���� ���������������
                case (int)AssociateForAllClassifiersOperations.Associate:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "�������������";
                    break;
                case (int)AssociateForAllClassifiersOperations.ClearAssociate:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "������� �������������";
                    break;
                case (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "���������� ������ � ������� �������������";
                    break;
                case (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "�������� ������ �� ������� �������������";
                    break;

                // �������� ��� �������������
                case (int)AssociateOperations.Associate:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "�������������";
                    parentOperation = (int)AssociateForAllClassifiersOperations.Associate;
                    break;
                case (int)AssociateOperations.ClearAssociate:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "������� �������������";
                    parentOperation = (int)AssociateForAllClassifiersOperations.ClearAssociate;
                    break;
                case (int)AssociateOperations.AddRecordIntoBridgeTable:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "���������� ������ � ������� �������������";
                    parentOperation = (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable;
                    break;
                case (int)AssociateOperations.DelRecordFromBridgeTable:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "�������� ������ �� ������� �������������";
                    parentOperation = (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable;
                    break;

                // �������� ��� ���� ���������� ������
                case (int)AllDataSourcesOperation.AddDataSource:
                    operationType = typeof(AllDataSourcesOperation);
                    operationCaption = "���������� ��������� ������";
                    break;
                case (int)AllDataSourcesOperation.DelDataSource:
                    operationType = typeof (AllDataSourcesOperation);
                    operationCaption = "�������� ��������� ������";
                    break;

                // �������� ��� ���� ������� ������
                case (int)AllDataPumpsOperations.StartPump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "������ ������� ������";
                    break;
                case (int)AllDataPumpsOperations.StopPump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "��������� ������� ������";
                    break;
                case (int)AllDataPumpsOperations.DeletePump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "�������� ������� ������";
                    break;
                case (int)AllDataPumpsOperations.PreviewPumpData:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "�������� ������ �������";
                    break;

                // �������� ��� ������� ������
                case (int)DataPumpOperations.StartPump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "������ ������� ������";
                    parentOperation = (int)AllDataPumpsOperations.StartPump;
                    break;
                case (int)DataPumpOperations.StopPump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "��������� ������� ������";
                    parentOperation = (int)AllDataPumpsOperations.StopPump;
                    break;
                case (int)DataPumpOperations.DeletePump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "�������� ������� ������";
                    parentOperation = (int)AllDataPumpsOperations.DeletePump;
                    break;
                case (int)DataPumpOperations.PreviewPumpData:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "�������� ������ �������";
                    break;

                // �������� ��� ����� ������������
                case (int)PlanningSheetOperations.Construction:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "���������������";
                    break;
                case (int)PlanningSheetOperations.ChangeFilters:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "��������� ��������";
                    break;
                case (int)PlanningSheetOperations.UpdateData:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "���������� ������";
                    break;

                // �������� ��� ���� �����
                case (int)AllTasksOperations.CreateTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "�������� �����";
                    break;
                case (int)AllTasksOperations.ChangeTaskHierarchyOrder:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "����������� ����� �� ��������";
                    break;
                case (int)AllTasksOperations.DelTaskAction:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "��������: �������";
                    break;
                case (int)AllTasksOperations.EditTaskAction:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "��������: �������������";
                    break;
                case (int)AllTasksOperations.ViewAllUsersTasks:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "�������� ����� ���� �������������";
                    break;
                case (int)AllTasksOperations.AssignTaskViewPermission:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "������������� ���� �� �������� �����";
                    break;
                case (int)AllTasksOperations.CanCancelEdit:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "����� ����������";
                    break;
                case (int)AllTasksOperations.ExportTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "������� �����";
                    break;
                case (int)AllTasksOperations.ImportTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "������ �����";
                    break;

                // �������� ��� ���� ������
                case (int)TaskTypeOperations.CreateTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "�������� ������";
                    parentOperation = (int)AllTasksOperations.CreateTask;
                    break;
                case (int)TaskTypeOperations.ChangeTaskHierarchyOrder:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "����������� ����� �� ��������";
                    parentOperation = (int)AllTasksOperations.ChangeTaskHierarchyOrder;
                    break;
                case (int)TaskTypeOperations.DelTaskAction:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "��������: �������";
                    parentOperation = (int)AllTasksOperations.DelTaskAction;
                    break;
                case (int)TaskTypeOperations.EditTaskAction:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "��������: �������������";
                    parentOperation = (int)AllTasksOperations.EditTaskAction;
                    break;
                case (int)TaskTypeOperations.ViewAllUsersTasks:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "�������� ����� ���� �������������";
                    parentOperation = (int)AllTasksOperations.ViewAllUsersTasks;
                    break;
                case (int)TaskTypeOperations.AssignTaskViewPermission:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "������������� ���� �� ������";
                    parentOperation = (int)AllTasksOperations.AssignTaskViewPermission;
                    break;
                case (int)TaskTypeOperations.CanCancelEdit:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "����� ����������";
                    break;
                    /*
                case (int)TaskTypeOperations.ExportTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "������� ������";
                    parentOperation = (int)AllTasksOperations.ExportTask;
                    break;
                case (int)TaskTypeOperations.ImportTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "������ ������";
                    parentOperation = (int)AllTasksOperations.ImportTask;
                    break;
                    */

                // �������� ��� ��������� ������
                case (int)TaskDocumentOperations.View:
                    operationType = typeof(TaskDocumentOperations);
                    operationCaption = "��������";
                    break;

                // �������� ��� ���� ������
                case (int)TaskOperations.View:
                    operationType = typeof(TaskOperations);
                    operationCaption = "�������� ������";
                    break;

                case (int)TaskOperations.Perform:
                    operationType = typeof(TaskOperations);
                    operationCaption = "���������� ������";
                    break;

                // �������� ��� ������������ �������
                case (int)WebReportsOperations.View:
                    operationType = typeof(WebReportsOperations);
                    operationCaption = "�������� �������";
                    break;

				// �������� ��� ���� ��������
				case (int)AllTemplatesOperations.CreateTemplate:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "�������� ��������";
					break;
				case (int)AllTemplatesOperations.ChangeTemplateHierarchyOrder:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "����������� �������� �� ��������";
					break;
				case (int)AllTemplatesOperations.EditTemplateAction:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "��������: �������������";
					break;
				case (int)AllTemplatesOperations.ViewAllUsersTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "�������� �������� ���� �������������";
					break;
				case (int)AllTemplatesOperations.AssignTemplateViewPermission:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "������������� ���� �� �������� ��������";
					break;
				case (int)AllTemplatesOperations.ExportTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "������� ��������";
					break;
				case (int)AllTemplatesOperations.ImportTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "������ ��������";
					break;

				// �������� ��� ���� �������
				case (int)TemplateTypeOperations.CreateTemplate:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "�������� �������";
					parentOperation = (int)AllTemplatesOperations.CreateTemplate;
					break;
				case (int)TemplateTypeOperations.ChangeTemplateHierarchyOrder:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "����������� ������� �� ��������";
					parentOperation = (int)AllTemplatesOperations.ChangeTemplateHierarchyOrder;
					break;
				case (int)TemplateTypeOperations.EditTemplateAction:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "��������: �������������";
					parentOperation = (int)AllTemplatesOperations.EditTemplateAction;
					break;
				case (int)TemplateTypeOperations.ViewAllUsersTemplates:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "�������� �������� ���� �������������";
					parentOperation = (int)AllTemplatesOperations.ViewAllUsersTemplates;
					break;
				case (int)TemplateTypeOperations.AssignTemplateViewPermission:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "������������� ���� �� �������� �������";
					parentOperation = (int)AllTemplatesOperations.AssignTemplateViewPermission;
					break;

				// �������� ��� �������
				case (int)TemplateOperations.ViewTemplate:
					operationType = typeof(TemplateOperations);
					operationCaption = "�������� �������";
					break;
				case (int)TemplateOperations.EditTemplateAction:
					operationType = typeof(TemplateOperations);
					operationCaption = "�������������� �������";
					break;

				//�������� ��� ��������
				case (Int32)ForecastOperations.ViewData:
					operationType = typeof(ForecastOperations);
					operationCaption = "�������� ������";
					break;
				case (Int32)ForecastOperations.AssignParam:
					operationType = typeof(ForecastOperations);
					operationCaption = "������������� ���������� ����������";
					break;
				case (Int32)ForecastOperations.Calculate:
					operationType = typeof(ForecastOperations);
					operationCaption = "������";
					break;
				case (Int32)ForecastOperations.CreateNew:
					operationType = typeof(ForecastOperations);
					operationCaption = "�������� ����� ��������";
					break;
				case (Int32)ForecastOperations.AllowEdit:
					operationType = typeof(ForecastOperations);
					operationCaption = "������ ���������";
					break;

				case (Int32)ScenForecastOperations.AssignParam: //��� ��������� � ���������
					operationType = typeof(ScenForecastOperations);
					operationCaption = "������������� ���������� ����������";
					break;
				case (Int32)ScenForecastOperations.Calculate:  //��� ��������� � ���������
					operationType = typeof(ScenForecastOperations);
					operationCaption = "������";
					break;
				case (Int32)ScenForecastOperations.CreateNew:  //��� ��������� � ���������
					operationType = typeof(ScenForecastOperations);
					operationCaption = "�������� ����� ��������";
					break;

				case (Int32)Form2pForecastOperations.Calculate:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "������";
					break;
				case (Int32)Form2pForecastOperations.CreateNew:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "�������� ����� ��������";
					break;
				case (Int32)Form2pForecastOperations.ViewData:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "�������� ������";
					break;

                // ����� ��� ���������� ��������������
                case (Int32)FinSourcePlaningOperations.View:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "����������� ������� ���������� ��������������";
                    break;
                case (Int32)FinSourcePlaningOperations.AddRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "���������� �������";
                    break;
                case (Int32)FinSourcePlaningOperations.DelRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "�������� �������";
                    break;
                case (Int32)FinSourcePlaningOperations.EditRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "�������������� �������";
                    break;
                case (Int32)FinSourcePlaningOperations.ImportData:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "������ ������";
                    break;
                case (Int32)FinSourcePlaningOperations.ClearData:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "������� ������";
                    break;
                case (Int32)FinSourcePlaningOperations.Calculate:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "������ ������";
                    break;

                // ����� ��� ���������� �������������� �� �������� ��������� ������
                case (Int32)FinSourcePlaningUIModuleOperations.View:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "����������� ������� ���������� ��������������";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.AddRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "���������� �������";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.DelRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "�������� �������";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.EditRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "�������������� �������";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.ImportData:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "������ ������";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.ClearData:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "������� ������";
                    break;

                // ����� ��� ���������� �������������� ��� ��������
                case (Int32)FinSourcePlaningCalculateUIModuleOperations.View:
                    operationType = typeof(FinSourcePlaningCalculateUIModuleOperations);
                    operationCaption = "����������� ������";
                    break;
                case (Int32)FinSourcePlaningCalculateUIModuleOperations.Calculate:
                    operationType = typeof(FinSourcePlaningCalculateUIModuleOperations);
                    operationCaption = "������ ������";
                    break;

                 // ��������� �������
                case (int)AllMessageOperations.Subscribe:
                    operationType = typeof (AllMessageOperations);
                    operationCaption = "����������� �� ��������� �� �������";
                    break;
                // ��������� �����������������
                case (int)MessageOperations.Subscribe:
                    operationType = typeof(MessageOperations);
                    operationCaption = "����������� �� ��������� �� �������";
                    break;
                // ����� ������������ �������
                case (int) IncomesPlaningOperations.ViewPlaningOperations:
                    operationType = typeof(IncomesPlaningOperations);
                    operationCaption = "����������� ���� �������� ���������� ������������ �������";
                    break;
                case (int) IncomesPlaningModuleOperations.ViewPlaningOperationsModule:
                    operationType = typeof(IncomesPlaningModuleOperations);
                    operationCaption = "����������� ������� ���������� ������������ �������";
                    break;
			}
        }

        public static string[] GetOperationsForType(SysObjectsTypes tp)
        {
            Type operationsEnumType = GetOperationsEnumForType(tp);
            string[] result = null;
            if (operationsEnumType != null)
            {
                FieldInfo[] fiArr = operationsEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                result = new string[fiArr.Length];
                for (int i = 0; i < fiArr.Length; i ++ )
                {
                    FieldInfo fi = fiArr[i];
                    int intVal = Convert.ToInt32(fi.GetValue(null));
                    result[i] = Convert.ToString(intVal);
                }
            }
            return result;
        }

        public static int[] GetOperationsForTypeInt(SysObjectsTypes tp)
        {
            Type operationsEnumType = GetOperationsEnumForType(tp);
            int[] result = null;
            if (operationsEnumType != null)
            {
                FieldInfo[] fiArr = operationsEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                result = new int[fiArr.Length];
                for (int i = 0; i < fiArr.Length; i++)
                {
                    result[i] = Convert.ToInt32(fiArr[i].GetValue(null));
                }
            }
            return result;
        }

        public static int? GetParentTypeInt(int tp)
        {
            switch ((SysObjectsTypes)tp)
            {
                case SysObjectsTypes.Unknown:
                    return null;
                case SysObjectsTypes.Administration:
                    return null;
                case SysObjectsTypes.AllUIModules:
                    return null;
                case SysObjectsTypes.UIModule:
                    return (int)SysObjectsTypes.AllUIModules;
                case SysObjectsTypes.EntityNavigationListUI:
                    return (int)SysObjectsTypes.AllUIModules;
                case SysObjectsTypes.UIClassifiersSubmodule:
                    return (Int32)SysObjectsTypes.EntityNavigationListUI;
                case SysObjectsTypes.AllDataClassifiers:
                    return null;
                case SysObjectsTypes.DataClassifier:
                    return (int)SysObjectsTypes.AllDataClassifiers;
                case SysObjectsTypes.AllAssociatedClassifiers:
                    return null;
                case SysObjectsTypes.AssociatedClassifier:
                    return (int)SysObjectsTypes.AllAssociatedClassifiers;
                case SysObjectsTypes.AllFactTables:
                    return null;
                case SysObjectsTypes.FactTable:
                    return (int)SysObjectsTypes.AllFactTables;
                case SysObjectsTypes.AssociateForAllClassifiers:
                    return null;
                case SysObjectsTypes.Associate:
                    return (int)SysObjectsTypes.AssociateForAllClassifiers;
                case SysObjectsTypes.AllDataSources:
                    return null;
                case SysObjectsTypes.AllDataPumps:
                    return null;
                case SysObjectsTypes.DataPump:
                    return (int)SysObjectsTypes.AllDataPumps;
                case SysObjectsTypes.PlanningSheet:
                    return null;
                case SysObjectsTypes.AllTasks:
                    return null;
                case SysObjectsTypes.TaskDocument:
                    return (int)SysObjectsTypes.AllTasks;
                case SysObjectsTypes.Task:
                    return (int)SysObjectsTypes.TaskType;
                case SysObjectsTypes.TaskType:
                    return (int)SysObjectsTypes.AllTasks;
				case SysObjectsTypes.AllTemplates:
					return null;
				case SysObjectsTypes.TemplateType:
					return (int)SysObjectsTypes.AllTemplates;
				case SysObjectsTypes.Template:
					return (int)SysObjectsTypes.TemplateType;

				///������� �������� �������
				case SysObjectsTypes.AllForecast:
					return null;
				case SysObjectsTypes.ScenarioForecast:
					return (Int32)SysObjectsTypes.AllForecast;
				case SysObjectsTypes.Form2pForecast:
					return null;
				// ��������� ��������������
				case SysObjectsTypes.FinSourcePlaningUIModule:
                    return (Int32)SysObjectsTypes.FinSourcePlaning;
                case SysObjectsTypes.FinSourcePlaningCalculateUIModule:
                    return (Int32)SysObjectsTypes.FinSourcePlaning;
                // message
                case SysObjectsTypes.Message:
                    return (int) SysObjectsTypes.AllMessages;
                // ������������ �������
                case SysObjectsTypes.IncomesPlaningModule:
                    return (int)SysObjectsTypes.IncomesPlaning;
				default:
                    return null;

            }
        }
    }
}