using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;
using Microsoft.Office.Interop.Excel;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	/// <summary>
	/// Интерфейс excel'евской модели
	/// </summary>
	[Guid("2D851755-81AA-4a2a-B954-74BD8040108B")]
	[ComVisible(true)]
	public interface IExModel : IDisposable
	{
		/// <summary>
		/// Закрывает модель, Ексель, уничтожает объекты.
		/// </summary>
		void CloseModel();

		/// <summary>
		/// Инициализирует модель
		/// </summary>
		/// <param name="path">Путь к модели</param>
		/// <param name="show">Показывать Excel</param>
		//void InitExModel(String path, Boolean show);

		/// <summary>
		/// Устанавливает путь к модели
		/// </summary>
		String BasePath { get; set; }
				
		/// <summary>
		/// Открывает модель
		/// </summary>
		/// <returns></returns>
		Boolean OpenModel();

		/// <summary>
		/// Открывает файл из директории модели и добавляет его к списку открытых
		/// </summary>
		/// <param name="fileName">имя файла</param>
		/// <returns></returns>
		Boolean OpenFile(String fileName);

		/// <summary>
		/// Возвращает интерфейс доступа к книге
		/// </summary>
		/// <param name="name">Имя книги</param>
		/// <returns></returns>
		IWorkbookOfModel GetWorkBook(String name);

		/// <summary>
		/// Пересчитывает всю модель
		/// </summary>
		void RecalcAll();

		/// <summary>
		/// Вызвает метод пересчета модели CalculateFullRebuild()
		/// </summary>
		void RecalcAllRebuild();

		/// <summary>
		/// Вызывает макрос без параметров
		/// </summary>
		/// <param name="name">Имя макроса</param>
		/// <returns>Возвращаемое макросом значение</returns>
		object CallMacros(String name);

		/// <summary>
		/// Задание исполняющей процедуры вывода в лог сообщений.
		/// </summary>
		//PrintMes Pm { set; }

		/// <summary>
		/// Запрещает расчет ячеек для уже открытых книг (перебором по листам)
		/// </summary>
		void DisableCalcOfWorkbooks();

		/// <summary>
		/// Разрешает расчет ячеек для уже открытых книг (перебором по листам)
		/// </summary>
		void EnableCalcOfWorkbooks();

		/// <summary>
		/// Интерфейс к математической модели
		/// </summary>
		IMathModel Mm
		{
			get;
		}

		/// <summary>
		/// Интерфейс к объекту планирования
		/// </summary>
		IIdicPlanning Ipl
		{
			get;
		}

		/// <summary>
		/// Создает модель для планирования
		/// </summary>
		void CreatePlanningModel();

		/// <summary>
		/// Хранит лог сообщений работы плагина
		/// </summary>
		String Log { get; }

		/// <summary>
		/// Переводит логгер в трассировку отладочных сообщений
		/// </summary>
		void ToDebugTraceMode();

	}

	/// <summary>
	/// Класс Excel'евской модели
	/// </summary>
	[Guid("7D1FE876-A4DE-44ec-9C97-04E351A19211")]
	[ComVisible(true)]
	public class ExModel : IExModel
	{
		private Microsoft.Office.Interop.Excel.Application excelApp; //Объект приложения
		private String basePath; //Путь к модели

		/// <summary>
		/// Список файлов модели
		/// </summary>
		readonly String[] ModelBooks = new String[] { "Внебюджетные фонды.xls", "Индикаторы.xls",
			"Инфляция и рост.xls", "Капиталовложения.xls", "КБ.xls", "Население.xls",
			"Настройка.xls", "Основной капитал.xls", "Регуляторы.xls", "Сектор населения.xls",
			"Сектор СНУ.xls", "Сектор СПТРУ.xls", "Социальные показатели.xls", "Сценарий.xls",
			"Трудовые ресурсы.xls", "ФБ.xls", "Форма 2п-ОКВЭД.xls", "Макробалансы.xls" }; //"main.xls", к этому времени он уже открыт

		private Int64 timeElapsed;
		
		/// <summary>
		/// Коллекция книг WokrbookOfModel
		/// </summary>
		private List<WorkbookOfModel> lstWorkbook = new List<WorkbookOfModel>();
				
		/// <summary>
		/// Создает Объект Екселя и задает путь к модели.
		/// </summary>
		/// <param name="path">Путь</param>
		/// <param name="show">Показывать Excel</param>
		public void InitExModel(String path, Boolean show)
		{
			excelApp.Visible = show;
			excelApp.DisplayAlerts = false;
			excelApp.DisplayInfoWindow = false;
			basePath = path;
		}

		public void Dispose()
		{
			if (mModel != null)
			{
				try
				{
					mModel.Dispose();
					if (Marshal.IsComObject(mModel))
						Marshal.ReleaseComObject(mModel);
				} catch (Exception e) { }
			}

			if (idicPlan != null)
			{
				try
				{
					idicPlan.Dispose();
					if (Marshal.IsComObject(idicPlan))
						Marshal.ReleaseComObject(idicPlan);
				} catch (Exception e) { }
			}
			
			try
			{
				if (Marshal.IsComObject(excelApp))
					Marshal.ReleaseComObject(excelApp);
			} catch (Exception e) {}
			
			lstWorkbook = null;
			excelApp = null;
			idicPlan = null;
			mModel = null;

			GC.Collect();
		}

		/// <summary>
		/// Открывает модель
		/// </summary>
		/// <returns></returns>
		public Boolean OpenModel()
		{
			Int32 i = 0;
			Stopwatch sw = new Stopwatch();
			sw.Reset();
			sw.Start();
			
			Trace.TraceMes("Start of loading wookbooks.");
			
			foreach (String s in ModelBooks)
			{
				WorkbookOfModel tmp = new WorkbookOfModel(excelApp, basePath/*, pm*/);
				if (tmp.OpenWorkbook(s))
				{
					lstWorkbook.Add(tmp);
					i++;
				}
			}
			sw.Stop();
			timeElapsed = sw.ElapsedMilliseconds;
			
			Trace.TraceMes("End of load. N={0} time elapsed={1}.{2}s.", i, (timeElapsed / 1000), ((timeElapsed % 1000) * 100));
			if (i == ModelBooks.Length) return true;
			else return false;
		}

		/// <summary>
		/// Открывает файл из директории модели и добавляет его к списку открытых
		/// </summary>
		/// <param name="fileName">имя файла</param>
		/// <returns></returns>
		public Boolean OpenFile(String fileName)
		{
			Trace.TraceMes("Start of loading wookbooks.");

			WorkbookOfModel tmp = new WorkbookOfModel(excelApp, basePath);
			if (tmp.OpenWorkbook(fileName))
			{
				lstWorkbook.Add(tmp);
				Trace.TraceMes("End of load. ");
				return true;
			}
			else return false;
		}

		/// <summary>
		/// Закрывает модель, Ексель, уничтожает объекты.
		/// </summary>
		public void CloseModel()
		{
			Stopwatch sw = new Stopwatch();
			sw.Reset();
			sw.Start();
			
			if (lstWorkbook.Count > 0)
			{
				foreach (IWorkbookOfModel workbook in lstWorkbook)
				{
					workbook.CloseWorkbook();
					workbook.Dispose();
				}
				lstWorkbook.Clear();
				GC.Collect();
			}
			
			sw.Stop();
			timeElapsed = sw.ElapsedMilliseconds;
		}

		#region Свойства
		/// <summary>
		/// Путь к модели
		/// </summary>
		public String BasePath
		{
			get { return basePath; }
			set { basePath = value; }
		}

		public Int64 TimeElapsed
		{
			get { return timeElapsed; }
		}

		public String Log
		{
			get
			{
				return Trace.Stat;
			}
		}
		
		public Microsoft.Office.Interop.Excel.Application ExcelApp
		{
			set { excelApp = value; }
		}

		public IMathModel Mm
		{
			get { return mModel; }
		}

		public IIdicPlanning Ipl
		{
			get { return idicPlan; }
		}
		#endregion

		public void ToDebugTraceMode()
		{
			Trace.toDebugMode();
		}

		private MathModel mModel;
		private IdicPlanning idicPlan;

		public void CreatePlanningModel()
		{
			mModel = new MathModel(this);
			idicPlan = new IdicPlanning(mModel);
		}
		
		/// <summary>
		/// Возвращает ссылку на Книгу
		/// </summary>
		/// <param name="name">Имя книги</param>
		/// <returns></returns>
		public IWorkbookOfModel GetWorkBook(String name)
		{
			foreach (WorkbookOfModel workbook in lstWorkbook)
			{
				if (workbook.NameOfFile == name)
					return workbook;
			}
			return null;
		}

		/// <summary>
		/// Вызвает метод пересчета модели CalculateFullRebuild()
		/// </summary>
		public void RecalcAllRebuild()
		{
			excelApp.CalculateFullRebuild();
		}

		/// <summary>
		/// Пересчитывает всю модель
		/// </summary>
		public void RecalcAll()
		{
			excelApp.CalculateFull();
		}

		/// <summary>
		/// Вызывает макрос без параметров
		/// </summary>
		/// <param name="name">Имя макроса</param>
		/// <returns>Возвращаемое макросом значение</returns>
		public Object CallMacros(String name)
		{
			Object o = excelApp.Run(name, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
				Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
				Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			return o;
		}

		/// <summary>
		/// Запрещает расчет ячеек для уже открытых книг (перебором по листам)
		/// </summary>
		public void DisableCalcOfWorkbooks()
		{
			foreach (WorkbookOfModel workbook in lstWorkbook)
			{
				foreach (Worksheet ws in workbook.WorkBook.Worksheets)
				{
					ws.EnableCalculation = false;
				}	
			}
		}

		/// <summary>
		/// Разрешает расчет ячеек для уже открытых книг (перебором по листам)
		/// </summary>
		public void EnableCalcOfWorkbooks()
		{
			foreach (WorkbookOfModel workbook in lstWorkbook)
			{
				foreach (Worksheet ws in workbook.WorkBook.Worksheets)
				{
					ws.EnableCalculation = true;
				}
			}
		}
	}
}