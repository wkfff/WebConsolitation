using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.Workplace.Gui
{
    public static class WorkplaceSingleton
    {
        private static Workplace workplace = null;

        public static Form MainForm
        {
            get { return workplace; }
        }

        public static Workplace Workplace
        {
			[System.Diagnostics.DebuggerStepThrough]
            get { return workplace; }
        }

        public static bool InitializeWorkplace()
        {
        	Trace.TraceInformation("InitializeWorkplace...");

            StatusBarService.Initialize();

            workplace = new Workplace();

            bool initialized = workplace.InitializeWorkplace();
            
            workplace.WorkplaceLayout = new WorkplaceLayout();

            OnWorkplaceCreated();
            return initialized;
        }

        private static void OnWorkplaceCreated()
        {
            if (WorkplaceCreated != null)
            {
                WorkplaceCreated(null, EventArgs.Empty);
            }
        }

        public static event EventHandler WorkplaceCreated;

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        [STAThread]
        public static void Main()
        {
			// Назначаем обработчик загрузки отсутствующих сборок
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
#if DEBUG
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
#endif

			// Назначаем обработчик необработанных исключений для домена приложения
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        	Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			// Назначаем обработчик необработанных исключений нити Workplace
            Application.ThreadException += UnhandledExceptionHandler.OnThreadException;

#if DEBUG
        	SetupConsoleWindow();
#endif

            // русифицируем используемые компоненты Infragistics
            InfragisticsRusification.LocalizeAll();

            /*string exe = Assembly.GetExecutingAssembly().Location;
            string configDirectory = Path.GetDirectoryName(exe);
            configDirectory = Path.Combine(configDirectory, "Options");

            PropertyService.InitializeService(configDirectory, configDirectory, "WorkplaceProperties");
            PropertyService.Load();
            
            ResourceService.RegisterNeutralStrings(new ResourceManager("Krista.FM.Client.Workplace.Properties.Resources", Assembly.GetExecutingAssembly()));*/

            ResourceService.InitializeService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Krista.FM.Client.Common.Resources.Loader.Initialize();

            // Если воркплейс проинициализирован
            if (InitializeWorkplace())
            {
                // запускаем воркплэйс 
				Trace.TraceInformation("Workplace started");
#if DEBUG
				workplace.WindowState = FormWindowState.Normal;
				workplace.Location = new Point(400, 0);
				workplace.Width = 1024 - 450;
#endif
				Application.Run(workplace);
            }
#if DEBUG
        	Trace.TraceWarning("Для зактытия консоли нажмите любую клавишу...");
        	Console.Read();
#endif
		}

#if DEBUG
		/// <summary>
		/// Настройка котсоли приложения.
		/// </summary>
		private static void SetupConsoleWindow()
		{
			Console.Title = "Консоль трассировочних сообщений приложения";

			IntPtr handle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "ConsoleWindowClass", Console.Title);
			// Установить позицию окна
			//SendMessage(handle, WM_MOVING, (int)0x7028 + (int)folderViewMode, 0);
			Console.SetBufferSize(1000, 3000);
			Console.SetWindowSize(140, 79);
			Console.SetWindowPosition(0, 0);
		}

		[DllImport("User32.dll", EntryPoint = "SendMessageW")]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, int lparam);

		[DllImport("User32.dll", EntryPoint = "FindWindowEx")]
		private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr child, string className, string window);

		[DllImport("User32.dll", EntryPoint = "GetWindow")]
		private static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

#endif

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string message = String.Format("Не удалось найти сборку: {0}", args.Name);
#if DEBUG
			Trace.TraceError(message);
#else
			//MessageBox.Show(message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
#endif
			return null;
		}

		private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Trace.TraceVerbose("Загружена сборка: {0}", args.LoadedAssembly.FullName);
		}

		/// <summary>
		/// Обработчик критических ошибок домена приложения.
		/// </summary>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string message = String.Format(CultureInfo.CurrentUICulture,
				"Сообщение: {0}\nТип исключения: {1}\nIsTerminating={2}",
				((Exception)e.ExceptionObject).Message,
				e.ExceptionObject.GetType().FullName,
				e.IsTerminating);
			Trace.TraceCritical(message);
			MessageBox.Show(message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
    }
}
