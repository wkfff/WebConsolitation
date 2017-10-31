using System;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Office.Core;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the Krista.FM.Server.Forecast.ExcelAddinSetup project, 
	// right click the project in the Solution Explorer, then choose install.
	#endregion

	/// <summary>
	/// Обертка для модели обеспечиывающая управлениие 
	/// </summary>
	[Guid("9CF1DFC8-F652-4e1c-8984-0B7709F2CD74")]
	[ComVisible(true)]
	public interface IModelWrapper
	{
		/// <summary>
		/// Открывает файлы входящие в модель
		/// </summary>
		void OpenModelFiles(String path, Boolean show);

		/// <summary>
		/// Метод инициализирует модель и если нужно открывает книги модели
		/// </summary>
		void InitModel(String path, Boolean show, Boolean open);

		/// <summary>
		/// Интерфейс к Excel'евской модели
		/// </summary>
		IExModel ExMod
		{
			get;
		}

		/// <summary>
		/// Закрывает файлы
		/// </summary>
		void CloseModelFiles();
		
		/// <summary>
		/// Задание исполняющей процедуры вывода в лог сообщений.
		/// </summary>
		IPrintMes IPm { set; }

	}

	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("F713A5B4-75EA-46D2-A589-120E3A1B40AB"), ProgId("ExcelAddin.Connect")]
	[ComVisible(true)]
	public class Connect : Object, Extensibility.IDTExtensibility2, IModelWrapper
	{

		private CommandBarButton myButton;
		
		private IPrintMes ipm; //Интерфейc на логгер сообщений
		
		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
			
		}

		private Microsoft.Office.Interop.Excel.Application exapp;

		/// <summary>
		/// Получает указатель на объект ExcelModel
		/// </summary>
		public IExModel ExMod
		{
			get { return exMod; }
		}
				
		public IPrintMes IPm
		{
			set 
			{ 
				ipm = value;
				Trace.pm = ipm.PrintMess;
			}
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		[ComVisible(true)]
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
			//applicationObject = application;
			exapp = (Microsoft.Office.Interop.Excel.Application)application;

			((COMAddIn)addInInst).Object = this;
			if (connectMode != Extensibility.ext_ConnectMode.ext_cm_Startup)
			{
				OnStartupComplete(ref custom);
			}
		}


		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		[ComVisible(true)]
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
			if (disconnectMode != Extensibility.ext_DisconnectMode.ext_dm_HostShutdown)
			{
				OnBeginShutdown(ref custom);
			}
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		[ComVisible(true)]
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' /
		[ComVisible(true)]
		public void OnStartupComplete(ref System.Array custom)
		{
			CommandBars oCommandBars;
			CommandBar oStandardBar;

			//System.Windows.Forms.MessageBox.Show("ExcelAddin Startup complete.");

			oCommandBars = (CommandBars)exapp.GetType().InvokeMember("CommandBars", BindingFlags.GetProperty, null, exapp, null);
			oStandardBar = oCommandBars["Standard"];
			
			/*try
			{
				oCommandBars = (CommandBars)exapp.GetType().InvokeMember("CommandBars", BindingFlags.GetProperty, null, exapp, null);
			}
			catch (Exception)
			{
				// Outlook имеет набор CommandBars в объекте Explorer.
				object oActiveExplorer;
				oActiveExplorer = exapp.GetType().InvokeMember("ActiveExplorer", BindingFlags.GetProperty, null, exapp, null);
				oCommandBars = (CommandBars)oActiveExplorer.GetType().InvokeMember("CommandBars", BindingFlags.GetProperty, null, oActiveExplorer, null);
			}

			// Настройка пользовательской кнопки в  панели команд Стандартная.
			try
			{
				oStandardBar = oCommandBars["Standard"];
			}
			catch (Exception)
			{
				// В Access основная панель задач называется Database.
				oStandardBar = oCommandBars["Database"];
			}*/

			// Если кнопка не была удалена, используется существующая.
			try
			{
				myButton = (CommandBarButton)oStandardBar.Controls["О плагине"];	//["My Custom Button"];
			}
			catch (Exception)
			{
				Object omissing = System.Reflection.Missing.Value;
				myButton = (CommandBarButton)oStandardBar.Controls.Add(1, omissing, omissing, omissing, omissing);
				myButton.Caption = "О плагине";
				myButton.Style = MsoButtonStyle.msoButtonCaption;
				omissing = null;
			}

			// Следующие элементы необязательные, но рекомендуемые. 
			//Свойство Tag помогает быстро найти орган управления
			//и помогает MSO отслеживать его, если видимо 
			//более одного окна приложения. Это свойство требуется
			//для некоторых приложений Office и должно быть предоставлено.
			myButton.Caption = "О плагине";

			// Свойство OnAction необязательное, но рекомендуемое. 
			//Его необходимо установить на ProgID надстройки, поэтому если даже 
			//надстройка не загружается при нажатии кнопки пользователем,
			//MSO загружает надстройку автоматически и затем вызывает
			//событие Click для надстройки, чтобы обработать его. 
			myButton.OnAction = "!<ExcelAddin.Connect>";

			myButton.Visible = true;
			myButton.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(this.MyButton_Click);
			
			oStandardBar = null;
			oCommandBars = null;

		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		[ComVisible(true)]
		public void OnBeginShutdown(ref System.Array custom)
		{
			Object omissing = System.Reflection.Missing.Value;

			myButton.Delete(omissing);

			try
			{
				if (Marshal.IsComObject(exapp))
					Marshal.ReleaseComObject(exapp);
			}
			catch (Exception e) { }

			try
			{
				if (Marshal.IsComObject(ipm))
					Marshal.ReleaseComObject(ipm);
			}
			catch (Exception e) { }

			try
			{
				if (Marshal.IsComObject(exMod))
					Marshal.ReleaseComObject(exMod);
			}
			catch (Exception e) { }

			try
			{
				if ((Trace.pm != null) && Marshal.IsComObject(Trace.pm))
					Marshal.ReleaseComObject(Trace.pm);
			}
			catch (Exception e) { }

			try
			{
				if (Marshal.IsComObject(myButton))
					Marshal.ReleaseComObject(myButton);
			}
			catch (Exception e) { }

			exMod = null;
			ipm = null;
			exapp = null;
			Trace.pm = null;
			myButton = null;
			omissing = null;

			GC.Collect();
		}
		
		private ExModel exMod;

		/// <summary>
		/// Метод инициализирует модель и открывает книги модели. Равносилен InitModel
		/// </summary>
		public void OpenModelFiles(String path, Boolean show)
		{
			InitModel(path, show, true);
		}

		/// <summary>
		/// Метод инициализирует модель и если нужно открывает книги модели
		/// </summary>
		public void InitModel(String path, Boolean show, Boolean open)
		{
			if (exMod == null)
			{
				exMod = new ExModel();
				exMod.ExcelApp = exapp;
				exMod.InitExModel(path, show);
				if (open)
					exMod.OpenModel();
			}
		}

		/// <summary>
		/// Метод закрывает открытые книги модели
		/// </summary>
		[ComVisible(true)]
		public void CloseModelFiles()
		{
			if (exMod != null)
			{
				exMod.CloseModel();
				exMod.Dispose();
			}
		}

		private void MyButton_Click(CommandBarButton cmdBarbutton, ref bool cancel)
		{
			Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
			String name = "";
			
			foreach (Assembly assembly in ass)
			{
				if (assembly.FullName.Contains("Krista.FM.Server.Forecast.ExcelAddin"))
				{
					name = assembly.FullName;
					break; 
				}
			}
			
			System.Windows.Forms.MessageBox.Show(String.Format("Плагин установлен совместо с программным продуктом АИС ФМ и подлежит удалению только вместе с программным продуктом ООО 'Криста'. Cборка: {0}",name), "ExcelAddin");

			ass = null;
			name = null;
		}
	}
}
