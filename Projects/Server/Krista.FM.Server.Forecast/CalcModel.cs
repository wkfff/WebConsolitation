using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.Server.Common;
using Krista.FM.Server.Forecast.ExcelAddin;
using Krista.FM.ServerLibrary;
using Microsoft.Office.Interop.Excel;
using DataTable=System.Data.DataTable;

namespace Krista.FM.Server.Forecast
{
	public static class CalcProccess
	{
		
		public static void MainCalcProc(Int32 id, IDatabase db, ExcelModel exMod)
		{
			Trace.TraceVerbose("������ �������� � id= {0}", id);

			Trace.TraceVerbose("�������� �������� �����������");
			String queryAdj = String.Format("select VALUEESTIMATE, VALUEY1, VALUEY2, VALUEY3, VALUEY4, VALUEY5, SIGNAT " +
											"from t_forecast_adjvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtAdj = (DataTable)db.ExecQuery(queryAdj, QueryResultTypes.DataTable);
			if (dtAdj != null)
			{
				IWorkbookOfModel wbAdj = exMod.ForecastModel.GetWorkBook("����������.xls");
				Worksheet wshAdj = (Worksheet)wbAdj.WorkBook.Worksheets.get_Item("����������");
				wbAdj.SetDataToCellY6_2(wshAdj, dtAdj);

				ForecastService.ReleaseBookAndSheet(ref wbAdj, ref wshAdj);
			}
			else
				throw new ForecastException("������ ������ ����������� ������ ������ �������");

			Trace.TraceVerbose("�������� �������� �������������� ����������");
			String queryUnReg = String.Format("select VALUEESTIMATE, VALUEY1, VALUEY2, VALUEY3, VALUEY4, VALUEY5, SIGNAT " +
											  "from t_forecast_unregadj t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtUnReg = (DataTable)db.ExecQuery(queryUnReg, QueryResultTypes.DataTable);
			if (dtUnReg != null)
			{
				IWorkbookOfModel wbUnReg = exMod.ForecastModel.GetWorkBook("��������.xls");
				Worksheet wshUnReg = (Worksheet)wbUnReg.WorkBook.Worksheets.get_Item("������� ���������");
				wbUnReg.SetDataToCellY6_2(wshUnReg, dtUnReg);

				ForecastService.ReleaseBookAndSheet(ref wbUnReg, ref wshUnReg);
			}
			else
				throw new ForecastException("������ ������ �������������� ���������� ������ ������ �������");

			Trace.TraceVerbose("�������� �������� �������������� ����������");
			String queryStatic = String.Format("select VALUEBASE, VALUEESTIMATE, SIGNAT, d.MASK " +
											   "from t_forecast_staticvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtStatic = (DataTable)db.ExecQuery(queryStatic, QueryResultTypes.DataTable);
			if (dtStatic != null)
			{
				IWorkbookOfModel wbStatic = exMod.ForecastModel.GetWorkBook("��������.xls");
				Worksheet wshStatic = (Worksheet)wbStatic.WorkBook.Worksheets.get_Item("����������");
				wbStatic.SetDataToCellY2_2_Masked(wshStatic, dtStatic);

				ForecastService.ReleaseBookAndSheet(ref wbStatic, ref wshStatic);
			}
			else
				throw new ForecastException("������ ������ �������������� ���������� ������ ������ �������");

			Trace.TraceVerbose("������������ ������...");
			try
			{
				exMod.ForecastModel.RecalcAll();
			}
			catch (Exception e)
			{
				throw new ForecastException("������ ������� ������. �������� �� ��� ��������� ������ ��� ������ �� �����: " + e.Message, e);
			}

			int balanceSteps = 0;

			IWorkbookOfModel wbBalance = exMod.ForecastModel.GetWorkBook("���������.xls");
			try
			{
				do
				{
					exMod.ForecastModel.CallMacros("���������.xls!Module1.BalanceModel");
					exMod.ForecastModel.RecalcAll();
					balanceSteps++;
				}
				while (Decimal.Compare(Convert.ToDecimal(wbBalance.GetDataFromCell("���������", "disbalance")), (Decimal)0.0001) == 1);
			}
			catch (Exception e)
			{
				throw new ForecastException("������ ��� ������������ ������. �������� �� ��� ��������� ������ ��� ������ �� �����: " + e.Message, e);
			}
			finally
			{
				ForecastService.ReleaseBook(ref wbBalance);
			}
			Trace.TraceVerbose("������ �������������� �� {0} �����", balanceSteps);
		}
	}
	
}