using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Forecast.ExcelAddin;
using Krista.FM.ServerLibrary;
using Microsoft.Office.Interop.Excel;
using DataTable=System.Data.DataTable;

namespace Krista.FM.Server.Forecast
{

	public class ExcelForm : IDisposable
	{
		private Microsoft.Office.Interop.Excel.Application excelApp;
		private IModelWrapper comClient;
		private Microsoft.Office.Core.COMAddIn comAddIn;

		private const String PluginGUID = "{F713A5B4-75EA-46D2-A589-120E3A1B40AB}";
		private const String DocumentGUID = "{630D536D-8602-46cb-BB57-673C9B041864}";
		public const String DefFileName = "forma_2P.xls";

		private Process excelProcess;
		private String fileName;

		private StartExcel se;
		private String modelDir = null;
				
		private IExModel forecastModel;

		public ExcelForm(IDatabase db, Int32 year)
		{
			Trace.TraceVerbose("Запуск EXCEL...");

			se = new StartExcel();
			excelApp = se.ExcelApp;
			excelProcess = se.ExcelProcess;
			if (excelProcess == null)
				throw new ForecastException("Процесс Excel не найден в памяти");

			Trace.TraceVerbose("Открытие листа планирования...");
			fileName = ExtractPlanningList(db, year.ToString());
			if (fileName != String.Empty)
			{
				OpenForm2pPlanningList(fileName);
			}
			else throw new ForecastException("Не удалось извлечь файл шаблона");
		}
		
		public Application ExcelApp
		{
			get { return excelApp; }
		}

		public string FileName
		{
			get { return fileName; }
		}

		public IExModel ForecastModel
		{
			get { return forecastModel; }
		}

		public void Dispose()
		{
			if (ExcelApp != null)
			{
				ExcelApp.Workbooks.Close();

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

				ExcelApp.Quit();
				if (Marshal.IsComObject(ExcelApp))
					Marshal.FinalReleaseComObject(ExcelApp);

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
			DeletePlanningList();
		}

		public void OpenForm2pPlanningList(String fileName)
		{
			
			Trace.TraceVerbose("Подключение плагина...");
			Int32 pcount = 0;
			foreach (Microsoft.Office.Core.COMAddIn ca in ExcelApp.COMAddIns)
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

			comClient.InitModel(Path.GetDirectoryName(fileName), ExcelApp.Visible, false);
			forecastModel = comClient.ExMod as IExModel;

			if (forecastModel == null)
				throw new ForecastException("Не удалось получить доступ к интерфейсу плагина");

			//ForecastModel.InitExModel(Path.GetDirectoryName(fileName), false);
			
			/*wb = new WorkbookOfModel(ExcelApp, Path.GetDirectoryName(fileName));
			wb.OpenWorkbook(Path.GetFileName(fileName));*/
			try
			{
				ForecastModel.OpenFile(Path.GetFileName(fileName));
			}
			catch (ForecastException e)
			{
				throw new ForecastException(String.Format("Не найден шаблон формы {0}",fileName), e);
			}
		}
				
		public String ExtractPlanningList(IDatabase db, String year)
		{
			FileStream fs = null;

			// Извлекаем документ из базы в буфер
			Byte[] documentData = new Byte[0];

			String queryText = String.Format("select d.document from documents d where d.sourcefilename = 'forma_2P_{0}.xls' and d.name = 'Форма 2П-{0}' and d.description = '{1}'", year, DocumentGUID);
			DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable);
				
			// исключение возникнет если документ пуст (еще не загружен)
			try
			{
				documentData = (Byte[])dt.Rows[0]["Document"];
			}
			catch { };
			
			if (documentData.Length != 0)
			{
				// Создем временную дирректорию
				modelDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), SessionContext.SessionId);
				if (!Directory.Exists(modelDir))
					Directory.CreateDirectory(modelDir);

				String localDocumentName = Path.Combine(modelDir, DefFileName);

				try
				{
					fs = new FileStream(localDocumentName, FileMode.CreateNew, FileAccess.Write);
					fs.Write(documentData, 0, documentData.Length);
				}
				finally
				{
					if (fs != null)
					{
						fs.Flush();
						fs.Close();
					}
				}
				return localDocumentName;
			}

			return String.Empty;
		}

		private void DeletePlanningList()
		{
			if (modelDir != null)
			{
				try
				{
					String fileName = Path.Combine(modelDir, DefFileName);
					if (File.Exists(fileName))
					{
						File.SetAttributes(fileName, FileAttributes.Archive);
						File.Delete(fileName);
					}

					String[] s = Directory.GetFiles(modelDir);
					if (s.Length == 0)
					{
						Directory.Delete(modelDir);
					}
				}
				catch (Exception e)
				{
					modelDir = null;
				}

			}
		}
		
	}
}