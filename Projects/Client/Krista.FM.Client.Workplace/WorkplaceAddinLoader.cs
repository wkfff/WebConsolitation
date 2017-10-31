using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinExplorerBar;

using Krista.FM.Common;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;


namespace Krista.FM.Client.Workplace
{
    internal static class WorkplaceAddinLoader
    {
        #region - Загрузка модулей расширений -

        /// <summary>
        /// Загрузка одного объекта просмотра с типом viewObjectTypeName
        /// </summary>
        /// <param name="loadingErrors"></param>
        /// <param name="viewObjectTypeName"></param>
		private static void LoadViewObject(IWorkbench workplace, List<BaseNavigationCtrl> viewObjects, UltraExplorerBar navigationControl, StringBuilder loadingErrors, object viewObjectTypeName, string baseVersion)
        {
            if (((string[])viewObjectTypeName)[0].StartsWith("Krista.FM.RIA.Extensions."))
            {
                // Пропускаем все модули относящиеся к web-интерфейсу RIA
                return;
            }

			string fullName = ((string[])viewObjectTypeName)[0];

			Trace.TraceInformation("Загрузка {0}", fullName);
			
			if (((string[])viewObjectTypeName).GetLength(0) != 2)
            {
				loadingErrors.AppendLine(String.Format("Неверный формат имени объекта просмотра: \"{0}\".", String.Join(" ", (string[])viewObjectTypeName)));
				return;
			}
        	
			Assembly parentAssembly;
			try
			{
				parentAssembly = Assembly.Load(((string[]) viewObjectTypeName)[1]);
			}
			catch (BadImageFormatException)
			{
				loadingErrors.AppendLine(String.Format("Не удалось загрузить поврежденную сборку: {0}.", ((string[])viewObjectTypeName)[1]));
				return;
			}
			catch (FileLoadException)
			{
				loadingErrors.AppendLine(String.Format("Не удалось загрузить сборку: {0}.", ((string[]) viewObjectTypeName)[1]));
				return;
			}
			catch (FileNotFoundException)
			{
				loadingErrors.AppendLine(String.Format("Не найдена сборка: {0}.", ((string[])viewObjectTypeName)[1]));
				return;
			}

			// если нужно - проверим сборку на предмет соответствия базовой версии
            bool versionCheckingPassed = true;
            if (!String.IsNullOrEmpty(baseVersion))
            {
                string assVersion = AppVersionControl.GetAssemblyVersion(parentAssembly);
                string assBaseVersion = AppVersionControl.GetAssemblyBaseVersion(assVersion);
                if (assBaseVersion != baseVersion)
                {
                    string errStr = String.Format(String.Format("Не удалось создать объект {0}: версия содержащей его сборки {1} [{2}] не соответствует базовой версии [{3}].",
                        fullName, parentAssembly.ManifestModule.Name, assVersion, baseVersion));
                    loadingErrors.AppendLine(errStr);
                    loadingErrors.AppendLine();
                    versionCheckingPassed = false;
                }
            }

            if (versionCheckingPassed)
            {
                BaseNavigationCtrl navigationObject;
                try
                {
                    navigationObject = (BaseNavigationCtrl)parentAssembly.CreateInstance(fullName, true);

                    navigationObject.Workplace = workplace;
                    viewObjects.Add(navigationObject);
                    UltraExplorerBarGroup newGroup = navigationControl.Groups.Add(fullName, navigationObject.Caption);
                    if (navigationObject.TypeImage16 != null)
                        newGroup.Settings.AppearancesSmall.HeaderAppearance.Image = navigationObject.TypeImage16;
                    if (navigationObject.TypeImage24 != null)
                        newGroup.Settings.AppearancesLarge.HeaderAppearance.Image = navigationObject.TypeImage24;
                }
                catch (MissingMethodException)
                {
                    loadingErrors.AppendLine(String.Format("Не удалось создать объект {0}, т.к. отсутствует конструктор класса.", fullName));
                }
            }
            else
            {
                loadingErrors.AppendLine(String.Format("Отсутствует объект просмотра '{0}'.", viewObjectTypeName));
            }
        }

        /// <summary>
        /// Загрузка объектов в режиме online, с учетом прав доступа.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="viewObjects"></param>
        /// <param name="navigationControl"></param>
        /// <param name="loadingErrors"></param>
		private static void LoadOnlineViewObjects(IWorkbench workplace, List<BaseNavigationCtrl> viewObjects, UltraExplorerBar navigationControl, StringBuilder loadingErrors)
        {
        	object[] allowedObjects;
			try
			{
				workplace.OperationObj.Text = "Построение списка объектов";
				Application.DoEvents();
				Trace.TraceInformation("Построение списка объектов");
				allowedObjects = workplace.ActiveScheme.UsersManager.GetViewObjectsNamesAllowedForCurrentUser();
			}
			catch (Exception e)
			{
				Trace.TraceError(Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
				loadingErrors.AppendLine(e.Message);
				return;
			}
        	if ((allowedObjects == null) || (allowedObjects.Length == 0))
            {
                workplace.OperationObj.StopOperation();
                string msgStr = String.Format("В соответствии с настройками прав доступа пользователю {0} не разрешена загрузка ни одного рабочего места", workplace.ActiveScheme.UsersManager.GetCurrentUserName());
                MessageBox.Show(msgStr, "Нет доступных рабочих мест", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                // если нужно - получаем базовую версию системы для проверок
                string baseVersion = String.Empty;
                if (!AppVersionControl.IgnoreVersionsModeOn())
                {
                    baseVersion = AppVersionControl.GetServerLibraryVersion();
                    baseVersion = AppVersionControl.GetAssemblyBaseVersion(baseVersion);
                }

				foreach (object item in allowedObjects)
                {
                    string[] parts = ((string)item).Split(',');
                    LoadViewObject(workplace, viewObjects, navigationControl, loadingErrors, parts, baseVersion);
                }
            }
        }

		internal static void LoadViewObjectsEx(IWorkbench workplace, List<BaseNavigationCtrl> viewObjects, UltraExplorerBar navigationControl)
        {
            StringBuilder loadingErrors = new StringBuilder();

			Trace.Indent();
            LoadOnlineViewObjects(workplace, viewObjects, navigationControl, loadingErrors);
			Trace.Unindent();

            if (loadingErrors.Length > 0)
            {
                workplace.OperationObj.StopOperation();
                MessageBox.Show(
                    String.Concat("При загрузке объектов просмотра возникли следующие ошибки:", Environment.NewLine, Environment.NewLine, loadingErrors.ToString()),
                    "Ошибка загрузки объектов просмотра", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion - Загрузка модулей расширений -
    }
}
