using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;
using Microsoft.Office.Interop.Excel;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	/// <summary>
	/// ��������� excel'������ ������
	/// </summary>
	[Guid("2D851755-81AA-4a2a-B954-74BD8040108B")]
	[ComVisible(true)]
	public interface IExModel : IDisposable
	{
		/// <summary>
		/// ��������� ������, ������, ���������� �������.
		/// </summary>
		void CloseModel();

		/// <summary>
		/// �������������� ������
		/// </summary>
		/// <param name="path">���� � ������</param>
		/// <param name="show">���������� Excel</param>
		//void InitExModel(String path, Boolean show);

		/// <summary>
		/// ������������� ���� � ������
		/// </summary>
		String BasePath { get; set; }
				
		/// <summary>
		/// ��������� ������
		/// </summary>
		/// <returns></returns>
		Boolean OpenModel();

		/// <summary>
		/// ��������� ���� �� ���������� ������ � ��������� ��� � ������ ��������
		/// </summary>
		/// <param name="fileName">��� �����</param>
		/// <returns></returns>
		Boolean OpenFile(String fileName);

		/// <summary>
		/// ���������� ��������� ������� � �����
		/// </summary>
		/// <param name="name">��� �����</param>
		/// <returns></returns>
		IWorkbookOfModel GetWorkBook(String name);

		/// <summary>
		/// ������������� ��� ������
		/// </summary>
		void RecalcAll();

		/// <summary>
		/// ������� ����� ��������� ������ CalculateFullRebuild()
		/// </summary>
		void RecalcAllRebuild();

		/// <summary>
		/// �������� ������ ��� ����������
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <returns>������������ �������� ��������</returns>
		object CallMacros(String name);

		/// <summary>
		/// ������� ����������� ��������� ������ � ��� ���������.
		/// </summary>
		//PrintMes Pm { set; }

		/// <summary>
		/// ��������� ������ ����� ��� ��� �������� ���� (��������� �� ������)
		/// </summary>
		void DisableCalcOfWorkbooks();

		/// <summary>
		/// ��������� ������ ����� ��� ��� �������� ���� (��������� �� ������)
		/// </summary>
		void EnableCalcOfWorkbooks();

		/// <summary>
		/// ��������� � �������������� ������
		/// </summary>
		IMathModel Mm
		{
			get;
		}

		/// <summary>
		/// ��������� � ������� ������������
		/// </summary>
		IIdicPlanning Ipl
		{
			get;
		}

		/// <summary>
		/// ������� ������ ��� ������������
		/// </summary>
		void CreatePlanningModel();

		/// <summary>
		/// ������ ��� ��������� ������ �������
		/// </summary>
		String Log { get; }

		/// <summary>
		/// ��������� ������ � ����������� ���������� ���������
		/// </summary>
		void ToDebugTraceMode();

	}

	/// <summary>
	/// ����� Excel'������ ������
	/// </summary>
	[Guid("7D1FE876-A4DE-44ec-9C97-04E351A19211")]
	[ComVisible(true)]
	public class ExModel : IExModel
	{
		private Microsoft.Office.Interop.Excel.Application excelApp; //������ ����������
		private String basePath; //���� � ������

		/// <summary>
		/// ������ ������ ������
		/// </summary>
		readonly String[] ModelBooks = new String[] { "������������ �����.xls", "����������.xls",
			"�������� � ����.xls", "����������������.xls", "��.xls", "���������.xls",
			"���������.xls", "�������� �������.xls", "����������.xls", "������ ���������.xls",
			"������ ���.xls", "������ �����.xls", "���������� ����������.xls", "��������.xls",
			"�������� �������.xls", "��.xls", "����� 2�-�����.xls", "������������.xls" }; //"main.xls", � ����� ������� �� ��� ������

		private Int64 timeElapsed;
		
		/// <summary>
		/// ��������� ���� WokrbookOfModel
		/// </summary>
		private List<WorkbookOfModel> lstWorkbook = new List<WorkbookOfModel>();
				
		/// <summary>
		/// ������� ������ ������ � ������ ���� � ������.
		/// </summary>
		/// <param name="path">����</param>
		/// <param name="show">���������� Excel</param>
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
		/// ��������� ������
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
		/// ��������� ���� �� ���������� ������ � ��������� ��� � ������ ��������
		/// </summary>
		/// <param name="fileName">��� �����</param>
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
		/// ��������� ������, ������, ���������� �������.
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

		#region ��������
		/// <summary>
		/// ���� � ������
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
		/// ���������� ������ �� �����
		/// </summary>
		/// <param name="name">��� �����</param>
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
		/// ������� ����� ��������� ������ CalculateFullRebuild()
		/// </summary>
		public void RecalcAllRebuild()
		{
			excelApp.CalculateFullRebuild();
		}

		/// <summary>
		/// ������������� ��� ������
		/// </summary>
		public void RecalcAll()
		{
			excelApp.CalculateFull();
		}

		/// <summary>
		/// �������� ������ ��� ����������
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <returns>������������ �������� ��������</returns>
		public Object CallMacros(String name)
		{
			Object o = excelApp.Run(name, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
				Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
				Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			return o;
		}

		/// <summary>
		/// ��������� ������ ����� ��� ��� �������� ���� (��������� �� ������)
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
		/// ��������� ������ ����� ��� ��� �������� ���� (��������� �� ������)
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