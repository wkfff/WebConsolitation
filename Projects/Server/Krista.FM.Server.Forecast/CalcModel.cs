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
			Trace.TraceVerbose("Расчет сценария с id= {0}", id);

			Trace.TraceVerbose("Передача значений регуляторов");
			String queryAdj = String.Format("select VALUEESTIMATE, VALUEY1, VALUEY2, VALUEY3, VALUEY4, VALUEY5, SIGNAT " +
											"from t_forecast_adjvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtAdj = (DataTable)db.ExecQuery(queryAdj, QueryResultTypes.DataTable);
			if (dtAdj != null)
			{
				IWorkbookOfModel wbAdj = exMod.ForecastModel.GetWorkBook("Регуляторы.xls");
				Worksheet wshAdj = (Worksheet)wbAdj.WorkBook.Worksheets.get_Item("Регуляторы");
				wbAdj.SetDataToCellY6_2(wshAdj, dtAdj);

				ForecastService.ReleaseBookAndSheet(ref wbAdj, ref wshAdj);
			}
			else
				throw new ForecastException("Запрос набора регуляторов вернул пустую таблицу");

			Trace.TraceVerbose("Передача значений нерегулируемых параметров");
			String queryUnReg = String.Format("select VALUEESTIMATE, VALUEY1, VALUEY2, VALUEY3, VALUEY4, VALUEY5, SIGNAT " +
											  "from t_forecast_unregadj t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtUnReg = (DataTable)db.ExecQuery(queryUnReg, QueryResultTypes.DataTable);
			if (dtUnReg != null)
			{
				IWorkbookOfModel wbUnReg = exMod.ForecastModel.GetWorkBook("Сценарий.xls");
				Worksheet wshUnReg = (Worksheet)wbUnReg.WorkBook.Worksheets.get_Item("Внешние параметры");
				wbUnReg.SetDataToCellY6_2(wshUnReg, dtUnReg);

				ForecastService.ReleaseBookAndSheet(ref wbUnReg, ref wshUnReg);
			}
			else
				throw new ForecastException("Запрос набора нерегулируемых параметров вернул пустую таблицу");

			Trace.TraceVerbose("Передача значений статистических параметров");
			String queryStatic = String.Format("select VALUEBASE, VALUEESTIMATE, SIGNAT, d.MASK " +
											   "from t_forecast_staticvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.RefScenario = {0})", id.ToString());
			DataTable dtStatic = (DataTable)db.ExecQuery(queryStatic, QueryResultTypes.DataTable);
			if (dtStatic != null)
			{
				IWorkbookOfModel wbStatic = exMod.ForecastModel.GetWorkBook("Сценарий.xls");
				Worksheet wshStatic = (Worksheet)wbStatic.WorkBook.Worksheets.get_Item("Статистика");
				wbStatic.SetDataToCellY2_2_Masked(wshStatic, dtStatic);

				ForecastService.ReleaseBookAndSheet(ref wbStatic, ref wshStatic);
			}
			else
				throw new ForecastException("Запрос набора статистических параметров вернул пустую таблицу");

			Trace.TraceVerbose("Балансировка модели...");
			try
			{
				exMod.ForecastModel.RecalcAll();
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка расчета модели. Возможно не все параметры заданы или заданы не верно: " + e.Message, e);
			}

			int balanceSteps = 0;

			IWorkbookOfModel wbBalance = exMod.ForecastModel.GetWorkBook("Настройка.xls");
			try
			{
				do
				{
					exMod.ForecastModel.CallMacros("Настройка.xls!Module1.BalanceModel");
					exMod.ForecastModel.RecalcAll();
					balanceSteps++;
				}
				while (Decimal.Compare(Convert.ToDecimal(wbBalance.GetDataFromCell("Настройка", "disbalance")), (Decimal)0.0001) == 1);
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка при балансировке модели. Возможно не все параметры заданы или заданы не верно: " + e.Message, e);
			}
			finally
			{
				ForecastService.ReleaseBook(ref wbBalance);
			}
			Trace.TraceVerbose("Модель сбалансирована за {0} шагов", balanceSteps);
		}
	}
	
}