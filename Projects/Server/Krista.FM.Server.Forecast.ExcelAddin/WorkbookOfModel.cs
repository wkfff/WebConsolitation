using System;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using DataTable=Microsoft.Office.Interop.Excel.DataTable;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	[Guid("7305B0B7-EA9C-4331-8AED-CE975C210BBE")]
	[ComVisible(true)]
	public interface IWorkbookOfModel : IDisposable
	{
		/// <summary>
		/// �������� ��� �����
		/// </summary>
		String NameOfFile { get; }

		/// <summary>
		/// �������� ������ ����� Interface Microsoft.Office.Intropt.Excel
		/// </summary>
		Workbook WorkBook { get; }

		/// <summary>
		/// ��������� ����� Excel
		/// </summary>
		/// <param name="Name">��� �����, ��� ����.</param>
		/// <returns></returns>
		Boolean OpenWorkbook(String Name);

		/// <summary>
		/// ��������� ����� 
		/// </summary>
		void CloseWorkbook();

		/// <summary>
		/// ��������� ����� 
		/// </summary>
		void SaveWorkbook();

		/// <summary>
		/// �������� ������ �� ������ �� ����������� ������������ ����� � �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <returns>��������</returns>
		Object GetDataFromCell(String Sheet, String namerow, String namecol);

		/// <summary>
		/// �������� ������ �� ������������� ������ 
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="nameCell">��� ������</param>
		/// <returns>��������</returns>
		Object GetDataFromCell(String Sheet, String nameCell);

		/// <summary>
		/// �������� ������ �� ������ �� ������ ������ � ����� �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="row">����� ������</param>
		/// <param name="namecol">�������� �������</param>
		/// <returns>��������</returns>
		Object GetDataFromCell(String Sheet, Int32 row, String namecol);

		/// <summary>
		/// �������� ������ �� ������ �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">�������� ������</param>
		/// <param name="namecol">�������� �������</param>
		/// <returns>��������</returns>
		Object GetDataFromCell(Worksheet Sheet, String namerow, String namecol);

		/// <summary>
		/// �������� ������ �� ������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="row">����� ������</param>
		/// <param name="count">���������� ����� �������� �� ������</param>
		/// <returns>������ ��������</returns>
		Object[,] GetDataFromRow(String Sheet, Int32 row, Int32 count);

		/// <summary>
		/// �������� ������ �� Worksheet �� 6 ��� (������� Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <returns>DataTable �������� �� 7 �������� � ��������� SIGNAT</returns>
		System.Data.DataTable GetDataFromSheetY6(Worksheet Sheet);

		/// <summary>
		/// �������� ������ �� Worksheet ��� ����� 2� (������� Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <returns>DataTable �������� �� 7 �������� � ��������� SIGNAT</returns>
		System.Data.DataTable GetDataFromSheetF2p(Worksheet Sheet);
		
		/// <summary>
		/// �������� ������ ����������� �� Worksheet �� 6 ��� 
		/// �� ������� (Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <returns>DataTable �������� �� 7 �������� � ��������� SIGNAT</returns>
		System.Data.DataTable GetChangedIndicatorsY6(Worksheet Sheet);

		/// <summary>
		/// ������������� ��������� ������  �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <param name="value">��������������� ��������</param>
		void SetDataToCell(String Sheet, String namerow, String namecol, Decimal value);

		/// <summary>
		/// ������������� �������� � ������  �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <param name="value">��������������� ��������</param>
		void SetDataToCell(Worksheet Sheet, String namerow, String namecol, Decimal value);

		/// <summary>
		/// ������������� �������� � 6 ������� ������������ ������ � ��������
		/// � �������������� estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">��� ������</param>
		void SetDataToCellY6(Worksheet Sheet, String namerow, Decimal estValue, Decimal Y1Value, Decimal Y2Value, Decimal Y3Value, Decimal Y4Value, Decimal Y5Value);

		/// <summary>
		/// ������������� �������� � 5 ������� ������������ ������ � ��������
		/// � �������������� y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">��� ������</param>
		void SetDataToCellY5(Worksheet Sheet, String namerow, Decimal Y1Value, Decimal Y2Value, Decimal Y3Value, Decimal Y4Value, Decimal Y5Value);

		/// <summary>
		/// ������������� �������� � 6 ������� ������������ ������ � ��������
		/// � �������������� estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 7 ��������, � ��������� ��������� ���������</param>
		void SetDataToCellY6(Worksheet Sheet, System.Data.DataTable dtValues);
		
		/// <summary>
		/// ������������� �������� � 6 ������� ������������ ������ � ��������
		/// � �������������� estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 7 ��������, � ��������� ��������� ���������</param>
		void SetDataToCellY6_2(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// ������������� �������� � 2 ������� ������������ ������ � ��������
		/// � �������������� base, estimate
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 3 �������, � ��������� ��������� ���������</param>
		void SetDataToCellY2_2(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// ������������� �������� � 2 ������� ������������ ������ � ��������
		/// � �������������� base, estimate �������� ����� � ������� MASK
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 4 �������, � ������������� ��������� ���������,
		/// ��������� ������� ����� </param>
		void SetDataToCellY2_2_Masked(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// ������������� �������� � 2 ������� ������������ ������ � ��������
		/// � �����2� �������� ������ � ������� SIGNAT
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 3 �������, � ��������� ��������� ���������</param>
		void SetDataToCellForm2p_Masked(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// ������������� �������� � 8 ������� ������������ ������ � ��������
		/// � �����2� �������� ������ � ������� SIGNAT ��� ������ ����� 2� � ������� ���
		/// �������� �������� �� ������.
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 9 ��������, � ��������� ��������� ���������</param>
		void SetDataToForm2p(Worksheet Sheet, System.Data.DataTable dtValues);
	}

	/// <summary>
	/// ����� ��������� �������������� ����� ������. ������������ � ������
	/// ExModel ��� � �������� �������� ��������� List<T>
	/// </summary>
	[Guid("3CAC1C84-AB80-4fe8-9361-D383FD6B1756")]
	[ComVisible(true)]
	public class WorkbookOfModel : IWorkbookOfModel
	{
		private readonly String basePath; //���� � xls ������ ������
		private Microsoft.Office.Interop.Excel.Application excelApp; //������ ������ �� ������ ����������
		private String nameOfFile; //�������� ����� � �������� �������� ������ �����
		private Workbook workBook; //���������� ������ �����
		
		/// <summary>
		///  ����������� ������ ����� ����� ������.
		/// </summary>
		/// <param name="app">������ ���������� Excel</param>
		/// <param name="modelPath">���� � ������ ������</param>
		public WorkbookOfModel(Microsoft.Office.Interop.Excel.Application app, String modelPath)
		{
			basePath = modelPath;
			excelApp = app;
		}
		
		#region ��������
		/// <summary>
		/// �������� ��� �����
		/// </summary>
		public String NameOfFile
		{
			get { return nameOfFile; }
		}

		/// <summary>
		/// �������� ������ �����
		/// </summary>
		public Workbook WorkBook
		{
			get { return workBook; }
		}
		#endregion
		
		/// <summary>
		/// ��������� ����� Excel
		/// </summary>
		/// <param name="Name">��� �����, ��� ����.</param>
		/// <returns></returns>
		public Boolean OpenWorkbook(String Name)
		{
			nameOfFile = Name;
			String S = Path.Combine(basePath, Name);
			try
			{
				workBook = excelApp.Workbooks.Open(S, XlUpdateLinks.xlUpdateLinksAlways, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing);
			}
			catch (Exception e)
			{
				throw new Exception("������ ��� �������� ����� ������: "+S, e);
			}
			if (workBook == null) return false;
			else return true;
		}

		/// <summary>
		/// ��������� ����� 
		/// </summary>
		public void CloseWorkbook()
		{
			if (workBook != null)
				workBook.Close(false, null, false);
		}

		/// <summary>
		/// ��������� ����� 
		/// </summary>
		public void SaveWorkbook()
		{
			if (workBook != null)
				workBook.Save();
		}

		/// <summary>
		/// �������� ������ �� ������ �� ����������� ������������ ����� � �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <returns>��������</returns>
		public Object GetDataFromCell(String Sheet, String namerow, String namecol)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(namerow + " " + namecol, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// �������� ������ �� ������������� ������ 
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="nameCell">��� ������</param>
		/// <returns>��������</returns>
		public Object GetDataFromCell(String Sheet, String nameCell)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(nameCell, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// �������� ������ �� ������ �� ������ ������ � ����� �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="row">����� ������</param>
		/// <param name="namecol">�������� �������</param>
		/// <returns>��������</returns>
		public Object GetDataFromCell(String Sheet, Int32 row, String namecol)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(namecol + row.ToString(), Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// �������� ������ �� ������ �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">�������� ������</param>
		/// <param name="namecol">�������� �������</param>
		/// <returns>��������</returns>
		public Object GetDataFromCell(Worksheet Sheet, String namerow, String namecol)
		{
			try
			{
				Range excelcells = Sheet.get_Range(namerow + " " + namecol, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}


		/// <summary>
		/// �������� ������ �� ������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="row">����� ������</param>
		/// <param name="count">���������� ����� �������� �� ������ �� ����� 26 ('A'..'Z')</param>
		/// <returns>������ ��������</returns>
		public Object[,] GetDataFromRow(String Sheet, Int32 row, Int32 count)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			Char a = 'A';
			for (Int32 i = 1; i < count; i++) a++;
			try
			{
				excelcells = tmp.get_Range("A" + row.ToString(), a + row.ToString());
				return (Object[,])excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		public System.Data.DataTable GetDataFromSheetY6(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("����������");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY4", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY5", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;
			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("A" + row.ToString(), "K" + row.ToString());
					Object[,] o = (Object[,])excelcells.Value2;
					if (o[1, 1] != null)
					{
						DataRow dr = dt.NewRow();
						dr["VALUEESTIMATE"] = Math.Round(Convert.ToDecimal(o[1, 6]), 4);
						dr["VALUEY1"] = Math.Round(Convert.ToDecimal(o[1, 7]), 4);
						dr["VALUEY2"] = Math.Round(Convert.ToDecimal(o[1, 8]), 4);
						dr["VALUEY3"] = Math.Round(Convert.ToDecimal(o[1, 9]), 4);
						dr["VALUEY4"] = Math.Round(Convert.ToDecimal(o[1, 10]), 4);
						dr["VALUEY5"] = Math.Round(Convert.ToDecimal(o[1, 11]), 4);
						dr["SIGNAT"] = o[1, 1];
						dt.Rows.Add(dr);
					}
					else
					{
						if ((o[1, 1] == null) && (o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 5] == null) && (o[1, 6] == null) && (o[1, 7] == null)) 
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("������ ��� ������ �����������");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		public System.Data.DataTable GetChangedIndicatorsY6(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("����������");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY4", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY5", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;

			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("A" + row.ToString(), "R" + row.ToString());

					Object[,] o = (Object[,])excelcells.Value2;
					if (o[1, 1] != null)
					{
						if (Convert.ToBoolean(o[1, 18]))
						{
							DataRow dr = dt.NewRow();
							dr["VALUEESTIMATE"] = o[1, 6];
							dr["VALUEY1"] = o[1, 7];
							dr["VALUEY2"] = o[1, 8];
							dr["VALUEY3"] = o[1, 9];
							dr["VALUEY4"] = o[1, 10];
							dr["VALUEY5"] = o[1, 11];
							dr["SIGNAT"] = o[1, 1];
							dt.Rows.Add(dr);
						}
					}
					else
					{
						if ((o[1, 1] == null) && (o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 5] == null) && (o[1, 6] == null) && (o[1, 7] == null))
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("������ ��� ������ �����������");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		/// <summary>
		/// �������� ������ �� Worksheet ��� ����� 2� (������� Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <returns>DataTable �������� �� 7 �������� � ��������� SIGNAT</returns>
		public System.Data.DataTable GetDataFromSheetF2p(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("�����2�");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;
			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("M" + row.ToString(), "V" + row.ToString());
					Object[,] o = (Object[,])excelcells.Value2;
					if ((o[1, 10] != null) && (o[1, 10].ToString() != "-"))  //������ "V" - ���������
					{
						DataRow dr = dt.NewRow();
						dr["VALUEESTIMATE"] = Math.Round(Convert.ToDecimal(o[1, 2]), 4);//������ "N" - ���������
						dr["VALUEY1"] = Math.Round(Convert.ToDecimal(o[1, 3]), 4);		//������ "O" - 1-��
						dr["VALUEY2"] = Math.Round(Convert.ToDecimal(o[1, 5]), 4);		//������ "Q" - 2-�� 
						dr["VALUEY3"] = Math.Round(Convert.ToDecimal(o[1, 7]), 4);		//������ "S" - 3-�� 
						dr["SIGNAT"] = o[1, 10];
						dt.Rows.Add(dr);
					}
					else
					{
						if ((o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 7] == null) && (o[1, 10] == null))
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("������ ��� ������ �� ����� ����� 2�");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		/// <summary>
		/// ������������� ��������� ������  �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">�������� ��������</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <param name="value">��������������� ��������</param>
		public void SetDataToCell(String Sheet, String namerow, String namecol, Decimal value)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			excelcells = tmp.get_Range(namerow + " " + namecol, Type.Missing);
			excelcells.Value2 = (Decimal)value;
		}

		/// <summary>
		/// ������������� ��������� ������  �� ����������� ������������ ������ � �������
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="namerow">��� ������</param>
		/// <param name="namecol">��� �������</param>
		/// <param name="value">��������������� ��������</param>
		public void SetDataToCell(Worksheet Sheet, String namerow, String namecol, Decimal value)
		{
			try
			{
				Range excelcells = Sheet.get_Range(namerow + " " + namecol, Type.Missing);
				excelcells.Value2 = value;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
					throw new Exception(e.Message, e);
				else
					Trace.TraceMes("�������� {0} �� {1} ������� � �������! ", namerow, namecol);
			}
		}

		public void SetDataToCellY6(Worksheet Sheet, String namerow, Decimal estValue, Decimal Y1Value, Decimal Y2Value,
		                            Decimal Y3Value, Decimal Y4Value, Decimal Y5Value)
		{
			Range excelcells = Sheet.get_Range(namerow + " " + "estimate", Type.Missing);
			excelcells.Value2 = (Decimal)estValue;
			excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
			excelcells.Value2 = (Decimal)Y1Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
			excelcells.Value2 = (Decimal)Y2Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
			excelcells.Value2 = (Decimal)Y3Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
			excelcells.Value2 = (Decimal)Y4Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
			excelcells.Value2 = (Decimal)Y5Value;
		}

		public void SetDataToCellY5(Worksheet Sheet, String namerow, Decimal Y1Value, Decimal Y2Value,
						Decimal Y3Value, Decimal Y4Value, Decimal Y5Value)
		{
			Range excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
			excelcells.Value2 = (Decimal)Y1Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
			excelcells.Value2 = (Decimal)Y2Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
			excelcells.Value2 = (Decimal)Y3Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
			excelcells.Value2 = (Decimal)Y4Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
			excelcells.Value2 = (Decimal)Y5Value;
		}

		[Obsolete("Very slow")]
		public void SetDataToCellY6(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;

				try
				{
					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						excelcells = Sheet.get_Range(namerow + " " + "estimate", Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
					}
					excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY1"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY2"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY3"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY4"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY5"]);
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("������ ��� �������� ���������: {0}", namerow);

					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
			}
		}

		public void SetDataToCellY6_2(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();

				try
				{
					Range excelcells = Sheet.get_Range(namerow, Type.Missing);
					Object[,] o = (Object[,])excelcells.Value2;

					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						o[1, 6] = Convert.ToDecimal(row["VALUEESTIMATE"]);
					}
					o[1, 7] = Convert.ToDecimal(row["VALUEY1"]);
					o[1, 8] = Convert.ToDecimal(row["VALUEY2"]);
					o[1, 9] = Convert.ToDecimal(row["VALUEY3"]);
					o[1, 10] = Convert.ToDecimal(row["VALUEY4"]);
					o[1, 11] = Convert.ToDecimal(row["VALUEY5"]);
					excelcells.Value2 = o;
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("������ ��� �������� ���������: {0}", namerow);

					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
			}

		}

		public void SetDataToCellY2_2(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				
				Range excelcells;
				if (namerow.EndsWith(".base"))
				{
					try
					{
						excelcells = Sheet.get_Range(namerow, Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
					}
					catch (System.Runtime.InteropServices.COMException e)
					{
						Trace.TraceMes("������ ��� �������� ��������� �� ������� ���: {0}", namerow);
						String[] s = e.Message.Split(' ');
						if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
							throw new Exception(e.Message, e);
					}
				}
				else
				{
					if (row["VALUEBASE"] != DBNull.Value)
					{
						try
						{
							excelcells = Sheet.get_Range(namerow, Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ������� ��� ������� � �������! ", namerow);
						}
					}
					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						try
						{
							excelcells = Sheet.get_Range(namerow + ".est", Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ��������� ��� ������� � �������! ", namerow);
						}
					}
				}
			}
		}


		public void SetDataToCellY2_2_Masked(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();

				Range excelcells;

				// ���� ��������� ������������ �� .base ������ �������� ���������� ������ �� ������� ���
				if (namerow.EndsWith(".base"))
				{
					try
					{
						excelcells = Sheet.get_Range(namerow, Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
					}
					catch (System.Runtime.InteropServices.COMException e)
					{
						Trace.TraceMes("������ ��� �������� ��������� �� ������� ���: {0}", namerow);
						String[] s = e.Message.Split(' ');
						if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
							throw new Exception(e.Message, e);
					}
				}
				else
				{
					//����������� �����

					Byte mask = Convert.ToByte(row["MASK"]);
					Boolean baseyear = (mask & 0x02) != 0;
					Boolean estyear = (mask & 0x01) != 0;

					if (baseyear)
					{
						if (row["VALUEBASE"] == DBNull.Value)
						{
							Trace.TraceMes("�������� {0} �� ������� ��� �� �����! ", namerow);
						}
						else
						try
						{
							excelcells = Sheet.get_Range(namerow, Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ������� ��� ������� � �������! ", namerow);
						}
					}
					if (estyear)
					{
						if (row["VALUEESTIMATE"] == DBNull.Value)
						{
							Trace.TraceMes("�������� {0} �� ��������� ��� �� �����! ", namerow);
						}
						else
						try
						{
							excelcells = Sheet.get_Range(namerow + ".est", Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ��������� ��� ������� � �������! ", namerow);
						}
					}
				}
			}
		}

		/// <summary>
		/// ������������� �������� � 2 ������� ������������ ������ � ��������
		/// � �����2� �������� ������ � ������� SIGNAT
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 3 �������, � ��������� ��������� ���������</param>
		public void SetDataToCellForm2p_Masked(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;
				Boolean baseyear1 = false;
				Boolean baseyear2 = false;

				//����������� �����
				if (namerow.Contains("_"))
				{
					String[] parts = namerow.Split('_');
					for (Int32 i = 1; i < parts.Length; i++)
						if (parts[i].StartsWith("I"))
						{
							Int32 k = Convert.ToInt32(parts[i].Substring(1));
							baseyear1 = (k & 0x02) != 0;
							baseyear2 = (k & 0x01) != 0;
						}
				}

				if (baseyear1)
				{
					if (row["r1"] == DBNull.Value)
					{
						Trace.TraceMes("�������� {0} �� ������ �������� ��� �� �����! ", namerow);
					}
					else
						try
						{
							excelcells = Sheet.get_Range(String.Format("_{0}",namerow), Type.Missing);
							excelcells[1, 12] = Convert.ToDecimal(row["r1"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ������ �������� ��� ������� � �������! ", namerow);
						}
				}
				if (baseyear2)
				{
					if (row["r2"] == DBNull.Value)
					{
						Trace.TraceMes("�������� {0} �� ������ �������� ��� �� �����! ", namerow);
					}
					else
						try
						{
							excelcells = Sheet.get_Range(String.Format("_{0}", namerow), Type.Missing);
							excelcells[1, 13] = Convert.ToDecimal(row["r2"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("�������� {0} �� ������ �������� ��� ������� � �������! ", namerow);
						}
				}

			}

		}

		/// <summary>
		/// ������������� �������� � 8 ������� ������������ ������ � ��������
		/// � �����2� �������� ������ � ������� SIGNAT ��� ������ ����� 2� � ������� ���
		/// �������� �������� �� ������.
		/// </summary>
		/// <param name="Sheet">������ �������� Excel.Worksheet</param>
		/// <param name="dtValues">������� � ������� 9 ��������, � ��������� ��������� ���������</param>
		public void SetDataToForm2p(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;
				
				try
				{
					excelcells = Sheet.get_Range(String.Format("_{0}", namerow), Type.Missing);
					if (row["r1"] != DBNull.Value)
						excelcells[1, 12] = Convert.ToDecimal(row["r1"]);
					if (row["r2"] != DBNull.Value)
						excelcells[1, 13] = Convert.ToDecimal(row["r2"]);
					if (row["est"] != DBNull.Value)
						excelcells[1, 14] = Convert.ToDecimal(row["est"]);
					if (row["v1_y1"] != DBNull.Value)
						excelcells[1, 15] = Convert.ToDecimal(row["v1_y1"]);
					if (row["v2_y1"] != DBNull.Value) 
						excelcells[1, 16] = Convert.ToDecimal(row["v2_y1"]);
					if (row["v1_y2"] != DBNull.Value) 
						excelcells[1, 17] = Convert.ToDecimal(row["v1_y2"]);
					if (row["v2_y2"] != DBNull.Value) 
						excelcells[1, 18] = Convert.ToDecimal(row["v2_y2"]);
					if (row["v1_y3"] != DBNull.Value) 
						excelcells[1, 19] = Convert.ToDecimal(row["v1_y3"]);
					if (row["v2_y3"] != DBNull.Value) 
						excelcells[1, 20] = Convert.ToDecimal(row["v2_y3"]);
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
					else
						Trace.TraceMes("�������� {0} ������� � �������! ", namerow);
				}
			}

		}

		public void Dispose()
		{
			try
			{
				if (Marshal.IsComObject(excelApp))
					Marshal.ReleaseComObject(excelApp);
			} catch (Exception e) {}
						
			try
			{
				if (Marshal.IsComObject(workBook))
					Marshal.ReleaseComObject(workBook);
			} catch (Exception e) {}

			excelApp = null;
			workBook = null;

			GC.Collect();
		}
	}
}