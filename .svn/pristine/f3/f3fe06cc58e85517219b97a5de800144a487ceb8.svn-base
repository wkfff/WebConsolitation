using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Forecast.ExcelAddin;
//using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;
using Microsoft.Office.Interop.Excel;
using System.IO;


namespace Krista.FM.Server.Forecast
{
	internal class TraceMess : IPrintMes
	{
		public void PrintMess(String s)
		{
			Trace.TraceVerbose(s);
		}
	}
	
	public class StartExcel
	{
		private readonly Microsoft.Office.Interop.Excel.Application excelApp;
		private readonly Process excelProcess;

		public StartExcel()
		{
			//Получаем список процессов до запуска екселя
			Process[] excelProcesses1 = Process.GetProcessesByName("EXCEL");

			try
			{
				excelApp = new Microsoft.Office.Interop.Excel.Application();
				excelApp.DisplayAlerts = false;
				excelApp.Visible = false;

			}
			catch (Exception e)
			{
				throw new ForecastException("Не удалось создать объект Excel", e);
			}

			//excelApp = (Microsoft.Office.Interop.Excel.Application)Marshal.GetActiveObject("Excel.Application");

			//Получаем список процессов после запуска екселя
			Process[] excelProcesses2 = Process.GetProcessesByName("EXCEL");

			Boolean isInProcessList;
			excelProcess = null;

			//Сравниваем два списка и получаем процесс экселя
			foreach (Process proc2 in excelProcesses2)
			{
				isInProcessList = false;
				foreach (Process proc1 in excelProcesses1)
				{
					if (proc1.Id == proc2.Id)
					{
						isInProcessList = true;
						break;
					}
				}
				if (!isInProcessList)
				{
					excelProcess = proc2;
					break;
				}
			}
		}

		public Application ExcelApp
		{
			get { return excelApp; }
		}

		public Process ExcelProcess
		{
			get { return excelProcess; }
		}
	}

	public class ExcelModel: IDisposable
	{
		private Microsoft.Office.Interop.Excel.Application excelApp;
		private IModelWrapper comClient;
		private IExModel forecastModel;
		private Microsoft.Office.Core.COMAddIn comAddIn;

		private readonly String modelPath;

		private const String PluginGUID = "{F713A5B4-75EA-46D2-A589-120E3A1B40AB}";

		private Process excelProcess;

		private Workbook workBook;
		private StartExcel se;
		
		/// <summary>
		/// Создает экземпляр Excel. Открывает лист "main.xls". Ищет наш плагин.
		/// Если найден открывает все книги модели.
		/// </summary>
		public ExcelModel()
		{
			Trace.TraceVerbose("Загрузка модели...");

			se = new StartExcel();
			excelApp = se.ExcelApp;
			excelProcess = se.ExcelProcess;
			if (excelProcess == null)
				new ForecastException("Процесс Excel не найден в памяти");

			modelPath = GetModelDirectory();
			
			try
			{
				workBook = excelApp.Workbooks.Open(Path.Combine(modelPath, "main.xls"), XlUpdateLinks.xlUpdateLinksAlways, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing);
			}
			catch (Exception e)
			{
				CloseModelDir();
				throw new ForecastException("Не удалось открыть главную книгу", e);
			}
			
			////////////
			
			Trace.TraceVerbose("Подключение плагина...");
			Int32 pcount = 0;
			foreach (Microsoft.Office.Core.COMAddIn ca in excelApp.COMAddIns)
			{
				if (ca.Guid == PluginGUID)
				{
					if (!ca.Connect)
					{
						ca.Connect = true;
					}
					comAddIn = ca;
					comClient = ca.Object as IModelWrapper;
					pcount++;
				}
			}
			
			if (comClient == null)
				throw new ForecastException("Не удалось найти и подключить плагин Excel GUID=" + PluginGUID);

			if (pcount > 1)
				throw new ForecastException("Найдено несколько версий плагина Excel GUID=" + PluginGUID);
						
			TraceMess tm = new TraceMess();
			comClient.IPm = tm;

			Trace.TraceVerbose("Открытие модели...");
			try
			{
				comClient.OpenModelFiles(modelPath, excelApp.Visible);
				forecastModel = comClient.ExMod as IExModel;
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка открытия модели", e);
			}

			Decimal ver;
			Boolean check;
			Int32 count;

			Object o = null;
			try
			{
				o = excelApp.Run("main.xls!Module1.GetVers", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
				count = Convert.ToInt32(o) - 1;
				Worksheet sheet = (Worksheet)workBook.Sheets["main"];
				ver = Convert.ToDecimal(sheet.get_Range("ver", Type.Missing).Value2);
				check = Convert.ToBoolean(sheet.get_Range("check", Type.Missing).Value2);
			}
			catch (Exception e)
			{
				CloseModelDir();
				throw new ForecastException("Не удалось выполнить проверку версии модели", e);
			}
			finally
			{
				CloseComObject(ref o);
			}

			if (check && (ModelBooks.Length == count)) 
			{
				Trace.TraceVerbose("Версия модели:{0}", ver);
			}
			else
				throw new ForecastException("Версия Файлов модели различна");

			try
			{
				excelApp.Run("main.xls!Module1.DisableCalc", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			}
			catch (Exception e)
			{
				CloseModelDir();
				throw new ForecastException("Не удалось выполнить макрос блокировки расчета", e);
			}
		}
		
		public IExModel ForecastModel
		{
			get { return forecastModel; }
		}

		public static string RepoDir
		{
			get { return repoDir; }
			set { repoDir = value; }
		}
				
		public void Dispose()
		{
			if (workBook != null)
			{
				workBook.Close(false, null, false);
				try
				{
					if (Marshal.IsComObject(workBook))
						Marshal.ReleaseComObject(workBook);
				}
				catch (Exception e) { }
				workBook = null;
			}

			if (excelApp != null)
			{
				excelApp.Workbooks.Close();
								
				try
				{
					if (Marshal.IsComObject(forecastModel))
						Marshal.ReleaseComObject(forecastModel);
				}
				catch (Exception e) { }
				forecastModel = null;
							
				try
				{
					if (Marshal.IsComObject(comClient))
						Marshal.ReleaseComObject(comClient);
				}
				catch (Exception e) { }
				comClient = null;
				
				try
				{
					if (Marshal.IsComObject(comAddIn))
						Marshal.ReleaseComObject(comAddIn);
				}
				catch (Exception e) { }
				comAddIn = null;
				
				excelApp.Quit();
				if (Marshal.IsComObject(excelApp))
					Marshal.FinalReleaseComObject(excelApp);

				excelApp = null;
				se = null;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();

				excelProcess.Refresh();

				if ((excelProcess != null) && !excelProcess.HasExited)
				{
					Trace.TraceError("Процесс Excel найден в памяти! Удаление процесса.");
					excelProcess.Kill();
				}

			}
			CloseModelDir();
		}

		private void CloseComObject(ref Object obj)
		{
			try
			{
				if (Marshal.IsComObject(obj))
					Marshal.ReleaseComObject(obj);
			}
			catch (Exception e) { }
			workBook = null;
		}

		/// <summary>
		/// Количество пользователей работающих с моделью
		/// </summary>
		private static Int32 userCount = 0;

		/// <summary>
		/// Директория для файлов модели
		/// </summary>
		private static String modelDir = String.Empty;

		private static String repoDir = String.Empty;

		/// <summary>
		/// Список файлов модели
		/// </summary>
		readonly static String[] ModelBooks = new String[] { "Внебюджетные фонды.xls", "Индикаторы.xls",
			"Инфляция и рост.xls", "Капиталовложения.xls", "КБ.xls", "Население.xls",
			"Настройка.xls", "Основной капитал.xls", "Регуляторы.xls", "Сектор населения.xls",
			"Сектор СНУ.xls", "Сектор СПТРУ.xls", "Социальные показатели.xls", "Сценарий.xls",
			"Трудовые ресурсы.xls", "ФБ.xls", "main.xls", "Форма 2п-ОКВЭД.xls", "Макробалансы.xls" };

		
		/// <summary>
		/// Метод создает временную директорию и копирует в нее файлы модели.
		/// В случае если с моделью уже работают пользователи, то возвращает
		/// директорию с моделью.
		/// </summary>
		/// <returns>Директория с моделью</returns>
		private static String GetModelDirectory()
		{
			if (userCount == 0)
			{
				userCount = 1;
				
				String rootDirectory = Environment.CurrentDirectory;
				if (RepoDir != String.Empty) 
					rootDirectory = RepoDir;
				String copyFromDir = Path.Combine(rootDirectory,"Forecast\\Model");
				modelDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"),SessionContext.SessionId);
				if (!Directory.Exists(modelDir)) Directory.CreateDirectory(modelDir);
				try
				{
					foreach (String fileName in ModelBooks)
					{
						File.Copy(Path.Combine(copyFromDir, fileName), Path.Combine(modelDir, fileName), true);
					}
				}
				catch (Exception e)
				{
					CloseModelDir();
					throw new ForecastException(String.Format("Не удалось скопировать файлы модели: {0}, {1}, {2}",copyFromDir, modelDir, e.Message), e);
				}
			}
			else userCount++;
			return modelDir;
		}

		/// <summary>
		/// Уменьшает счетчик количества пользователей работающих с директорией.
		/// Если пользователь последний то уничтожает директорию 
		/// </summary>
		private static void CloseModelDir()
		{
			if (userCount == 1)
			{
				try
				{
					if (modelDir != String.Empty)
					{
						//Directory.Delete(modelDir, true);
						foreach (String fileName in ModelBooks)
						{
							if (File.Exists(Path.Combine(modelDir, fileName)))
							{
								File.SetAttributes(Path.Combine(modelDir, fileName), FileAttributes.Archive);
								File.Delete(Path.Combine(modelDir, fileName));
							}
						}
						String[] s = Directory.GetFiles(modelDir);
						if (s.Length == 0)
						{
							Directory.Delete(modelDir);
						}
					}
				}
				catch (Exception e)
				{
					throw new ForecastException("Ошибка при удалении директории:"+e.Message,e);
				}
				finally
				{
					modelDir = String.Empty;
					userCount = 0;
				}
			}
			else 
				userCount--;
		}
	}
}
