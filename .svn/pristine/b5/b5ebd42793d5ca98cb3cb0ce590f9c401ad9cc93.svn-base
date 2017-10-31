using System;
using System.Collections;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AdministrationUI;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal partial class TemplatestUIBase
	{
		private IInplaceTasksPermissionsView permissions;

		private void SetPermissions()
		{
			ArrayList allowedTemplateTypes;

			// Проверяем права на распределение прав
			allowedTemplateTypes = Workplace.ActiveScheme.UsersManager.GetTemplateTypesIdsWithAllowedOperation(
				TemplateTypeOperations.AssignTemplateViewPermission, ClientAuthentication.UserID);

			// Присоединяем детали с правами
			permissions = TemplatesNavigation.Instance.GetTasksPermissions();
			if (permissions != null && allowedTemplateTypes != null && allowedTemplateTypes.Contains((int)templateType))
				permissions.AttachViewObject(ViewObject.GroupsTabPage, ViewObject.UsersTabPage);
			else
				ViewObject.sc1.Panel2Collapsed = true;

			// Настраиваем права на команды создания отчетов
			CommandList[typeof(Commands.CreateChildTemplateCommand).Name].IsEnabled =
			CommandList[typeof(Commands.CreateRootTemplateCommand).Name].IsEnabled =
				CheckCanCreateTemplates();

			// Настраиваем права на команды редактирования отчетов
			allowedTemplateTypes = Workplace.ActiveScheme.UsersManager.GetTemplateTypesIdsWithAllowedOperation(
				TemplateTypeOperations.EditTemplateAction, ClientAuthentication.UserID);

			if (CommandList.ContainsKey(typeof(Commands.AddDocumentTemplateCommand).Name))
				CommandList[typeof(Commands.AddDocumentTemplateCommand).Name].IsEnabled =
					allowedTemplateTypes != null && allowedTemplateTypes.Contains((int)templateType);

			if (CommandList.ContainsKey(typeof(Commands.EditTemplateCommand).Name))
				CommandList[typeof(Commands.EditTemplateCommand).Name].IsEnabled =
					allowedTemplateTypes != null && allowedTemplateTypes.Contains((int)templateType);

			// Настраиваем права на экспорт\импорт
			bool allowExport =
				Workplace.ActiveScheme.UsersManager.CheckAllTemplatesPermissionForTemplate(
					ClientAuthentication.UserID, AllTemplatesOperations.ExportTemplates);

			bool allowImport =
				Workplace.ActiveScheme.UsersManager.CheckAllTemplatesPermissionForTemplate(
					ClientAuthentication.UserID, AllTemplatesOperations.ImportTemplates);

			ViewObject.ugeTemplates.SaveMenuVisible = allowExport;
			ViewObject.ugeTemplates.LoadMenuVisible = allowImport;
			ViewObject.ugeTemplates.AllowImportFromXML = allowImport;

            foreach (Control control in ViewObject.GroupsTabPage.Controls)
            {
                UltraGridEx groupGrid = control as UltraGridEx;
                if (groupGrid != null)
                {
                    groupGrid.OnRefreshData += new RefreshData(groupGrid_OnRefreshData);
                    break;
                }
            }

            foreach (Control control in ViewObject.UsersTabPage.Controls)
            {
                UltraGridEx userGrid = control as UltraGridEx;
                if (userGrid != null)
                {
                    userGrid.OnRefreshData += new RefreshData(groupGrid_OnRefreshData);
                    break;
                }
            }
		}

        bool groupGrid_OnRefreshData(object sender)
        {
            UltraGridRow activeRow = ViewObject.ugeTemplates.ugData.ActiveRow;
            if (activeRow == null)
                return false;

            int activeId = UltraGridHelper.GetActiveID(ViewObject.ugeTemplates.ugData);
            
            SetActiveTemplatePermissions(activeId, (TemplateDocumentTypes)Convert.ToInt32(activeRow.Cells[TemplateFields.Type].Value));
            return true;
        }

		internal void SetActiveTemplatePermissions(int templateID, TemplateDocumentTypes documentTypes)
		{
			if (permissions != null)
			{
				permissions.RefreshAttachedData(templateID, SysObjectsTypes.Template, false);
				permissions.RefreshAttachedData(templateID, SysObjectsTypes.Template, true);
			}

			bool allowView = CheckCanViewTemplate(templateID) && documentTypes != TemplateDocumentTypes.Group;
			bool allowEdit = CheckCanEditTemplate(templateID) && documentTypes != TemplateDocumentTypes.Group;

			CommandList[typeof(Commands.OpenTemplateCommand).Name].IsEnabled = allowView;
			CommandList[typeof(Commands.SaveTemplateCommand).Name].IsEnabled = allowView;

			CommandList[typeof(Commands.EditTemplateCommand).Name].IsEnabled = allowEdit;
			CommandList[typeof(Commands.AddDocumentTemplateCommand).Name].IsEnabled = allowEdit;
		}

		/// <summary>
		/// Проверка на наличие прав просмотра шаблона.
		/// </summary>
		/// <returns></returns>
		private bool CheckCanViewTemplate(int templateID)
		{
			return Workplace.ActiveScheme.UsersManager.CheckPermissionForTemplate(
				templateID, (int)templateType, (int)TemplateOperations.ViewTemplate, false);
		}

		/// <summary>
		/// Проверка на наличие прав создания шаблона.
		/// </summary>
		/// <returns></returns>
		internal bool CheckCanCreateTemplates()
		{
			ArrayList allowedTemplateTypes = Workplace.ActiveScheme.UsersManager.GetTemplateTypesIdsWithAllowedOperation(
				TemplateTypeOperations.CreateTemplate, ClientAuthentication.UserID);
			return allowedTemplateTypes != null && allowedTemplateTypes.Contains((int)templateType);
		}

		/// <summary>
		/// Проверка на наличие прав редактирования (удаления) шаблона.
		/// </summary>
		/// <returns></returns>
		internal bool CheckCanEditTemplate(int templateID)
		{
			return Workplace.ActiveScheme.UsersManager.CheckPermissionForTemplate(
				templateID, (int)templateType, (int)TemplateOperations.EditTemplateAction, false);
		}
	}
}
