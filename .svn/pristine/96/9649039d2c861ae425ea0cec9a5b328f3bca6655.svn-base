using System;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	public abstract partial class BaseForecastUI : BaseClsUI
	{


		virtual public void SetPermissions()
		{

		}

	}

	public partial class ForecastUI : BaseForecastUI
	{
		protected String callClassName = "";

		protected Boolean canAssignParam = false;
		protected Boolean canCalculate = false;
		protected Boolean canEdit = false;

		public override void SetPermissions()
		{
			base.SetPermissions();
			
			IUsersManager um = Workplace.ActiveScheme.UsersManager;
			Int32 userID = ClientAuthentication.UserID;
			
			//Разрешение на просмотр
			if (!um.CheckAllForecastPermission(userID, ForecastOperations.ViewData))
				throw new ForecastException("Пользователь не имеет прав на доступ к пакету: 'Прогноза развития региона'. Обратитесь к администратору.");
						
			//RootToolsCollection tools = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools;
			
			/*if (tools.Exists("btnCalcScenario") && !um.CheckScenForecastPermission(userID, "Scenario", ScenForecastOperations.Calculate))
				tools["btnCalcScenario"].SharedProps.Visible = false;*/

			//Разрешение на расчет
			if (um.CheckScenForecastPermission(userID, callClassName, ScenForecastOperations.Calculate))
				canCalculate = true;
			else
				canCalculate = false;

			//Разрешение на создание новых
			if (!um.CheckScenForecastPermission(userID, callClassName, ScenForecastOperations.CreateNew))
				((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = false;

			//Разрешение на назначение ответствененных лиц
			if (um.CheckScenForecastPermission(userID, callClassName, ScenForecastOperations.AssignParam))
				canAssignParam = true;
			else
				canAssignParam = false;

			//РАзрешение на редактирование параметров для которых пользователь
			//не является ответственным
			if (um.CheckAllForecastPermission(userID, ForecastOperations.AllowEdit))
				canEdit = true;
			else
				canEdit = false;
		}
	}

	public partial class Form2pUI : BaseForecastUI
	{
		protected Boolean canCalculate = false;
		protected Boolean canCreateNew = false;

		public override void SetPermissions()
		{
			base.SetPermissions();

			IUsersManager um = Workplace.ActiveScheme.UsersManager;
			Int32 userID = ClientAuthentication.UserID;

			//Разрешение на просмотр
			if (!um.CheckForm2pForecastPermission(userID, Form2pForecastOperations.ViewData))
				throw new ForecastException("Пользователь не имеет прав на доступ к пакету: 'Прогноза развития региона'. Обратитесь к администратору.");

			//Разрешение на создание новых
			if (!um.CheckForm2pForecastPermission(userID, Form2pForecastOperations.CreateNew))
			{
				((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = false;
				canCreateNew = false;
			}
			else
			{
				((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = true;
				canCreateNew = true;
			}

			//Разрешение на расчет
			if (um.CheckForm2pForecastPermission(userID, Form2pForecastOperations.Calculate))
				canCalculate = true;
			else
				canCalculate = false;

		}
	}
	
}