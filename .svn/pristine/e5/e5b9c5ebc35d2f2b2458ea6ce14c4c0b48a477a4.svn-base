using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Common.Validations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.Server.Common;
using Krista.FM.Server.Forecast.ExcelAddin;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.Validations;
using Microsoft.Office.Interop.Excel;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;
using DataTable=System.Data.DataTable;

namespace Krista.FM.Server.Forecast
{
	public class ForecastService : ServerSideObject, IForecastService
	{
		/// <summary>
		/// ID таблицы фактов Прогноз.Параметры
		/// </summary>
		public const String f_S_Forecast_Key = "c865470a-f1b0-4974-a22c-4d81acf6c18f";

		/// <summary>
		/// ID ассоциации мастер-деталь от таблицы фактов к таблице Индикаторы
		/// </summary>
		public const String a_Forecast_IndValuesKey = "82925b29-ee0d-4f58-a762-f7a0e1956392";

		/// <summary>
		/// ID ассоциации мастер-деталь от таблицы фактов к таблице Регуляторы
		/// </summary>
		public const String a_Forecast_AdjValuesKey = "3b2310d9-f09a-4bbd-9915-449af81d5fe6";

		/// <summary>
		/// Ключ таблицы Регуляторы
		/// </summary>
		public const String t_S_Adjusters_Key = "ddade0e2-8964-40a9-9410-891a1c24bcab";
		
		/// <summary>
		/// Ключ таблицы Индикаторы
		/// </summary> 
		public const String t_S_Indicators_Key = "472af329-907c-4bfc-ba6b-e88831356ef2";
						
		
		private static ForecastService instance;
		private IScheme scheme;

		public ForecastService(ServerSideObject owner)
			: base(owner)
		{
		}

		public static ForecastService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ForecastService(null);
				}
				return instance;
			}
		}
		
		public void Initialize(IScheme scheme)
		{
			this.scheme = scheme;
			ExcelModel.RepoDir = this.scheme.BaseDirectory;
		}

		public bool Initialized
		{
			get { return scheme != null; }
		}

		internal IScheme Scheme
		{
			get { return scheme; }
		}
		
		#region IForecastService Members

		public IFactTable Data
		{
			get
			{
				return Instance.Scheme.FactTables[f_S_Forecast_Key];
			}
		}

		public Dictionary<string, IEntityAssociation> Details
		{
			get
			{
				Dictionary<string, IEntityAssociation> tables = new Dictionary<string, IEntityAssociation>();

				foreach (IEntityAssociation item in Data.Associated.Values)
				{
					if (item.AssociationClassType == AssociationClassTypes.MasterDetail)
					{
						tables.Add(item.ObjectKey, item);
					}
				}

				return tables;
			}
		}

		/// <summary>
		/// Метод копирует параметры из базового сецнария в сценарий с id=ToID
		/// </summary>
		/// <param name="toID">ID сценария в который будут копироваться записи</param>
		/// <param name="listener">делегат функции обратного вызова информирования клиента </param>
		public void CopyScenarioDetails(Int32 toID, ForecastListenerDelegate listener)
		{
			Trace.TraceVerbose("Копируем параметры из базового сецнария в сценарий с id={0}",toID);
			///получаем объект для доступа к базе данных
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				//Ищем в таблице фактов строку с базовым сценарием (отличается тем, что UserID = 0 и ReadyToCalc = -1)
				Object o = db.ExecQuery(String.Format("select ID from {0} where (UserID = 0) and (ReadyToCalc = -1)", Data.FullDBName), QueryResultTypes.Scalar);
				if (o != null)
				{
					/// если строка найдена, получаем ID базового сценария
					Int32 baseID = Convert.ToInt32(o);
					if (baseID == toID)
					{
						throw new ForecastException("Невозможно скопировать сценарий сам в себя");
					}

					try
					{
						String copyResults;
						db.BeginTransaction();
						CopyFactTableDetails(Data, db, baseID, toID, listener, out copyResults);
						db.Commit();
					}
					catch (Exception e)
					{
						db.Rollback();
						throw new ForecastException(e.Message, e);
					}
				}
				else
				{
					//если не найдено уничтожаем объект базы данных и вызываем исключение.
					throw new ForecastException("Не найден базовый сценарий в таблице {f_Forecast_Scenario");
				}
			}
		}

		/// <summary>
		/// Метод копирует параметры из сецнария c id=FromID в сценарий с id=ToID
		/// </summary>
		/// <param name="fromID">ID сценария из которого будут копироваться записи</param>
		/// <param name="toID">ID сценария в который будут копироваться записи</param>
		/// <param name="listener">делегат функции обратного вызова информирования клиента </param>
		public void CopyScenarioDetails(Int32 fromID, Int32 toID, ForecastListenerDelegate listener)
		{
			Trace.TraceVerbose("Копируем параметры из сецнария id={0} в сценарий с id={1}", fromID, toID);
			///получаем объект для доступа к базе данных
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				try
				{
					String copyResults;
					db.BeginTransaction();
					CopyFactTableDetails(Data, db, fromID, toID, listener, out copyResults);
					db.Commit();
				}
				catch (Exception e)
				{
					db.Rollback();
					throw new ForecastException(e.Message, e);
				}
			}
		}

		public IValidatorMessageHolder CalcModel(Int32 id)
		{
			ValidationMessages vms = new ValidationMessages();
			
			using (ExcelModel exMod = new ExcelModel())
			{
				using (IDatabase db = this.Scheme.SchemeDWH.DB)
				{
					CalcProccess.MainCalcProc(id, db, exMod);
					SaveOfCalc(id, db, exMod, vms);
					SetScenarioStatus(id, ScenarioStatus.Calculated);
				}
			}
			return vms;
		}
		
		/// <summary>
		/// Индикативное плнирование
		/// </summary>
		/// <param name="ind">перечень индикаторов id,Col</param>
		/// <param name="adj">перечень регуляторов id,Col</param>
		public IValidatorMessageHolder IdicPlanning(Int32 id, Dictionary<Int32, String> ind, Dictionary<Int32, String> adj)
		{
			ValidationMessages vms = new ValidationMessages();

			using (ExcelModel exMod = new ExcelModel())
			{
				using (IDatabase db = this.Scheme.SchemeDWH.DB)
				{
					///exMod.ForecastModel.ToDebugTraceMode();////////
										
					CalcProccess.MainCalcProc(id, db, exMod);
					exMod.ForecastModel.CreatePlanningModel();

					Dictionary<String, Int32> dAdj = new Dictionary<String, Int32>();

					foreach (KeyValuePair<Int32, String> pair in adj)
					{
						String query = String.Format("select t.{0} as value, t.minbound, t.maxbound, d.signat " +
							"from t_forecast_adjvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.id = {1})", pair.Value, pair.Key);
						DataTable dtAdj = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
				
						if ((dtAdj != null) && (dtAdj.Rows.Count > 0))
						{
							DataRow drow = dtAdj.Rows[0];
							String name = drow["SIGNAT"].ToString();
							Decimal min = Convert.ToDecimal(drow["MINBOUND"]);
							Decimal max = Convert.ToDecimal(drow["MAXBOUND"]);
							Decimal value = Convert.ToDecimal(drow["VALUE"]);

							dAdj.Add(name, pair.Key);

							String yearName;
							switch (pair.Value.ToUpper())
							{
								case "VALUEESTIMATE":
									yearName = "estimate";
									break;
								case "VALUEY1":
									yearName = "y.1";
									break;
								case "VALUEY2":
									yearName = "y.2";
									break;
								case "VALUEY3":
									yearName = "y.3";
									break;
								case "VALUEY4":
									yearName = "y.4";
									break;
								case "VALUEY5":
									yearName = "y.5";
									break;
								default:
									throw new ForecastException(String.Format("Неизвестный код: {0}", pair.Value));
							}

							exMod.ForecastModel.Mm.Adj.Add(name, value, min, max, yearName);
							exMod.ForecastModel.Ipl.AdjStart.Add(name, value, min, max, yearName);
						}
						else 
							throw new ForecastException(String.Format("Не найден регулятор {0} помеченный на планирование!",pair.Value)); 
					}


					foreach (KeyValuePair<Int32, String> pair in ind)
					{
						String query = String.Format("select t.{0} as value, t.minbound, t.maxbound, t.leftpenaltycoef, t.rightpenaltucoef,  d.signat " +
							"from t_forecast_indvalues t, d_Forecast_Parametrs d where (d.Id = t.refparams) and (t.id = {1})", pair.Value, pair.Key);
						DataTable dtInd = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);

						if ((dtInd != null) && (dtInd.Rows.Count > 0))
						{
							String name = dtInd.Rows[0]["SIGNAT"].ToString();
							Decimal min = Convert.ToDecimal(dtInd.Rows[0]["MINBOUND"]);
							Decimal max = Convert.ToDecimal(dtInd.Rows[0]["MAXBOUND"]);
							Decimal g_l = Convert.ToDecimal(dtInd.Rows[0]["LEFTPENALTYCOEF"]);
							Decimal g_r = Convert.ToDecimal(dtInd.Rows[0]["RIGHTPENALTUCOEF"]);
							Decimal value = Convert.ToDecimal(dtInd.Rows[0]["VALUE"]);

							String yearName;
							switch (pair.Value.ToUpper())
							{
								case "VALUEESTIMATE":
									yearName = "estimate";
									break;
								case "VALUEY1":
									yearName = "y.1";
									break;
								case "VALUEY2":
									yearName = "y.2";
									break;
								case "VALUEY3":
									yearName = "y.3";
									break;
								case "VALUEY4":
									yearName = "y.4";
									break;
								case "VALUEY5":
									yearName = "y.5";
									break;
								default:
									throw new ForecastException(String.Format("Неизвестный код: {0}", pair.Value));
							}

							exMod.ForecastModel.Mm.Ind.Add(name, value, min, max, g_l, g_r, yearName);
						}
						else
							throw new ForecastException(String.Format("Не найден индикатор {0} помеченный на планирование!", pair.Value)); 
					}

					exMod.ForecastModel.Ipl.CheckBounds = true;
					IAdjusters res = exMod.ForecastModel.Ipl.MakePlanning(10, (Decimal)0.01, false);
					
					IEntityAssociation item = Data.Associated[a_Forecast_AdjValuesKey];
					IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
					parameters[0] = db.CreateParameter(item.FullDBName, id);

					using (IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("{0} = ?", item.FullDBName), null, parameters))
					{

						/// DataTable для детали Индикаторы
						DataTable dtAdjusters = new DataTable();
						du.Fill(ref dtAdjusters);

						for (Int32 indx = 0; indx < res.Count; indx++)
						{
							IFactor fac = res[indx];
							String name = fac.Name;
							Int32 idAdj = dAdj[name];
							String year = adj[idAdj];
							DataRow dr = dtAdjusters.Select(String.Format("ID = {0}", idAdj))[0];

							if (dr != null)
							{
								dr.BeginEdit();
								dr[year] = fac.Value;
								dr.EndEdit();
							}
						}

						du.Update(ref dtAdjusters);
					}

					CalcProccess.MainCalcProc(id, db, exMod); ////???????????/

					SaveOfCalc(id, db, exMod, vms);
					//SetScenarioStatus(id, ScenarioStatus.Calculated);
				}
			}
			return vms;
		}

		private static readonly string[] IND_FIELDS = new String[] { "id", "basescenario", "refscenario", "valuebase", "valueestimate",
			"v_est_b", "valuey1", "v_y1_b", "valuey2", "v_y2_b", "valuey3", "v_y3_b", "valuey4", "v_y4_b", "valuey5", 
			"v_y5_b", "minbound", "maxbound", "leftpenaltycoef", "rightpenaltucoef", "userid", "refparams", "designation", "groupname" };
		private static readonly string[] ADJ_FIELDS = new String[] { "id", "basescenario", "refscenario", "valuebase", "valueestimate", 
			"v_est_b", "valuey1", "v_y1_b", "valuey2", "v_y2_b", "valuey3", "v_y3_b", "valuey4", "v_y4_b", "valuey5",
			"v_y5_b", "minbound", "maxbound", "userid", "refparams", "designation", "groupname" };

		/// <summary>
		/// Возвращает Data Updater для детали t_forecast_adjvalues из вьюшки для варианта расчета
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		public IDataUpdater GetValuationAdjustersUpdater(String filtr)
		{
			DataUpdater du = GetDetailUpdater(filtr, "v_forecast_val_adjusters", ADJ_FIELDS);
			du.OnInsteadUpdate += new InsteadUpdateEventDelegate(DUOnAdjustersUpdate);
			return du;
		}

		/// <summary>
		/// Возвращает Data Updater для детали t_forecast_indvalues из вьюшки для варианта расчета
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		public IDataUpdater GetValuationIndicatorsUpdater(String filtr)
		{
			DataUpdater du = GetDetailUpdater(filtr, "v_forecast_val_indicators", IND_FIELDS);
			du.OnInsteadUpdate += new InsteadUpdateEventDelegate(DUOnIndicatorsUpdate);
			return du;
		}

		/// <summary>
		/// Возвращает Data Updater для деталей сценария с переопределенным запросом select
		/// </summary>
		/// <param name="key">Ключ объекта</param>
		/// <param name="filter">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		public IDataUpdater GetScenarioDetailsUpdater(String key, String filter)
		{
			IEntity activeObj = (IEntity)Scheme.GetObjectByKey(key);
			Database db = (Database)Scheme.SchemeDWH.DB;
			IDbDataAdapter adapter = db.GetDataAdapter();

			DataUpdater du = new DataUpdater(adapter, null, db);
			db.InitDataAdapter(adapter, activeObj.FullDBName, activeObj.Attributes, null, String.Empty, null, null);
						
			//adapter.SelectCommand.CommandText = String.Format("{0} {1}", query, filtr);

			List<string> attributesList = new List<string>();

			foreach (IDataAttribute attr in activeObj.Attributes.Values)
			{
				attributesList.Add("t."+attr.Name.ToUpper());
			}

			String includedQuery2 = String.Empty;

			if (Scheme.SchemeDWH.FactoryName.Contains("Oracle"))
			{
				includedQuery2 = "select(select Name from d_Forecast_Parametrs i where substr(i.Code, 1, 1) = substr(o.code, 1, 1) and substr(i.Code, 2, 2) = substr(o.Code, 2, 2) and substr(i.Code, 5, 2) = '00' ) from d_Forecast_Parametrs o where o.id = t.refparams";
			}
			else
			{
				includedQuery2 = "select(select Name from d_Forecast_Parametrs i where substring(str(i.Code,6,0), 1, 1) = substring(str(o.code,6,0), 1, 1) and substring(str(i.Code,6,0), 2, 2) = substring(str(o.Code,6,0), 2, 2) and substring(str(i.Code,6,0), 5, 2) = '00') from d_Forecast_Parametrs o where o.id = t.refparams";
			};

			String includedQuery1 = "select designation from d_units_okei o where d.refunits = o.id";
			String query = String.Format("select {0}, ({1}) as designation, ({2}) as groupname, d.mask from {3} t, d_forecast_parametrs d where (d.id = t.refparams)", String.Join(", ", attributesList.ToArray()), includedQuery1, includedQuery2, activeObj.FullDBName);
			if (filter != null)
				query = String.Format("{0} and {1}", query, filter);
			adapter.SelectCommand.CommandText = query;

			du.Transaction = db.Transaction;
			return du;
		}

		/// <summary>
		/// Устанавливает статус сценария с ID
		/// </summary>
		/// <param name="status"></param>
		/// <param name="id"></param>
		public void SetScenarioStatus(Int32 id, ScenarioStatus status)
		{
			Trace.TraceVerbose("Устанавливаем статус для сценария с id={0}",id);
			///получаем объект для доступа к базе данных

			IDataUpdater du = Data.GetDataUpdater();
			DataTable dt = new DataTable();
			du.Fill(ref dt);

			DataRow[] drows = dt.Select(String.Format("ID = {0}", id));
			if (drows.Length > 0)
			{
				DataRow dr = drows[0];
				dr.BeginEdit();
				dr["READYTOCALC"] = status;
				dr.EndEdit();
			}

			du.Update(ref dt);
		}

		public String GetParentScenarioName(Int32 id)
		{
			Trace.TraceVerbose("Получаем наименование для родительского сценария с id={0}", id);
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				String query = String.Format("select name from f_forecast_scenario where id = {0}", id);
				Object o = db.ExecQuery(query, QueryResultTypes.Scalar);
				if (o != null)
				{
					return o.ToString();
				}
				else
					return "Родительский сценарий не существует";
			}
		}
				
		public void GetPercentOfComplete(Int32 id)
		{
			Trace.TraceVerbose("Получаем процент готовности для сценария с id={0}", id);
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				String query = String.Format("select (select count(ta.id) from t_forecast_adjvalues ta where (ta.refscenario=f.id))+"+
					"(select count(ts.id) from t_forecast_staticvalues ts where (ts.refscenario=f.id))+"+
					"(select count(tu.id) from t_forecast_unregadj tu where (tu.refscenario=f.id)) as fullcoount,"+
					"(select count(ta.id) from t_forecast_adjvalues ta where (ta.refscenario=f.id) and (ta.finished=1))+"+
					"(select count(ts.id) from t_forecast_staticvalues ts where (ts.refscenario=f.id) and (ts.finished=1))+"+
					"(select count(tu.id) from t_forecast_unregadj tu where (tu.refscenario=f.id) and (tu.finished=1)) as readycount "+
					"from f_forecast_scenario f where f.id={0}", id);
				DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);

				Int32 perc = -1;
				try
				{
					if (dt != null)
					{
						Int32 full = Convert.ToInt32(dt.Rows[0]["fullcoount"]);
						Int32 ready = Convert.ToInt32(dt.Rows[0]["readycount"]);
						perc = (100 * ready / full);
						if ((perc == 100) && (ready != full))
							perc = 99;
					}
				}
				catch (DivideByZeroException e)
				{
					Trace.TraceError("Деление на ноль при определении количества параметров для сценария с  id={0}", id);
				}

				query = "update f_forecast_scenario set percofcomplete=? where id=?";
				db.ExecQuery(query, QueryResultTypes.NonQuery, db.CreateParameter("perc",perc,DbType.Int32),
					db.CreateParameter("id", id, DbType.Int32));
			}
		}


		#endregion
				
		/// <summary>
		/// Копирует детали таблицы фактов.
		/// </summary>
		/// <param name="factTable">Таблица фактов.</param>
		/// <param name="db">Объект доступа к базе данных.</param>
		/// <param name="fromID">старый ID копируемых записей таблицы фактов.</param>
		/// <param name="toID">новый ID копируемых записей таблицы фактов.</param>
		/// <param name="listener">делегат функции обратного вызова информирования клиента </param>
		/// <param name="copyResults">результат копирования</param>
		private void CopyFactTableDetails(IFactTable factTable, IDatabase db, Int32 fromID, Int32 toID, ForecastListenerDelegate listener, out String copyResults)
		{
			StringBuilder sbResults = new StringBuilder();
			// Перебор всех ассоциаций таблицы фактов
			foreach (IEntityAssociation item in factTable.Associated.Values)
			{
				///проверка ассоциации на соответсвие классу Таблица
				if (item.RoleData.ClassType == ClassTypes.Table)
				{
					IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
					parameters[0] = db.CreateParameter(item.FullDBName, fromID);
					IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("{0} = ?", item.FullDBName), null, parameters);
					DataTable dt = new DataTable();
					du.Fill(ref dt);
					foreach (DataRow row in dt.Rows)
					{
						row.SetAdded();
						row["ID"] = item.RoleData.GetGeneratorNextValue;
						row[item.FullDBName] = toID;
					}
					Int32 affectedRows = du.Update(ref dt);
					sbResults.AppendLine(String.Format("\tТаблица \"{0}\", скопировано строк: {1} ", item.RoleData.OlapName, affectedRows));
					if (listener != null)
						listener(String.Format("\tТаблица \"{0}\", скопировано строк: {1}", item.RoleData.OlapName, affectedRows));

				}
			}
			copyResults = sbResults.ToString();
		}

		/// <summary>
		/// Копирует детали таблицы фактов заполняя значением DBNULL
		/// все поля кроме "refparams" и "refscenario" (EntityAssociation.FullDBName)
		/// </summary>
		/// <param name="factTable">Таблица фактов.</param>
		/// <param name="db">Объект доступа к базе данных.</param>
		/// <param name="fromID">ID источника копируемых записей таблицы фактов.</param>
		/// <param name="toID">ID приемника копируемых записей таблицы фактов.</param>
		/// <param name="listener">делегат функции обратного вызова информирования клиента </param>
		/// <param name="copyResults">результат копирования</param>
		private void CopyFactTableDetailsNulls(IFactTable factTable, IDatabase db, Int32 fromID, Int32 toID, ForecastListenerDelegate listener, out String copyResults)
		{
			StringBuilder sbResults = new StringBuilder();
			// Перебор всех ассоциаций таблицы фактов
			foreach (IEntityAssociation item in factTable.Associated.Values)
			{
				///проверка ассоциации на соответсвие классу Таблица
				if (item.RoleData.ClassType == ClassTypes.Table)
				{
					IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
					parameters[0] = db.CreateParameter(item.FullDBName, fromID);
					IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("{0} = ?", item.FullDBName), null, parameters);
					DataTable dt = new DataTable();
					du.Fill(ref dt);
					foreach (DataRow row in dt.Rows)
					{
						row.SetAdded();
						foreach (DataColumn column in dt.Columns)
						{
							if (column.ColumnName == item.FullDBName) row[item.FullDBName] = toID;
							else
								if (column.ColumnName != "refparams") row[column.ColumnName] = DBNull.Value;
						}
					}
					Int32 affectedRows = du.Update(ref dt);
					sbResults.AppendLine(String.Format("\tТаблица \"{0}\", скопировано строк: {1} ", item.RoleData.OlapName, affectedRows));
					if (listener != null)
						listener(String.Format("\tТаблица \"{0}\", скопировано строк: {1}", item.RoleData.OlapName, affectedRows));

				}
			}
			copyResults = sbResults.ToString();
		}
				
		/// <summary>
		/// Возвращает Data Updater для детали из вьюшки для варианта расчета
		/// из вьюшки 
		/// </summary>
		/// <param name="filtr"></param>
		/// <param name="table"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		private DataUpdater GetDetailUpdater(String filtr, String table, String[] param)
		{
			Database db = (Database)Scheme.SchemeDWH.DB;
			IDbDataAdapter adapter = db.GetDataAdapter();

			#region Select Command
			adapter.SelectCommand = db.InitCommand(null);

			List<string> attributesList = new List<string>();

			foreach (String p in param)
			{
				attributesList.Add("t." + p.ToUpper());
			}
						
			adapter.SelectCommand.CommandText = String.Format("Select {0}, d.mask from {1} t, d_forecast_parametrs d where (d.id = t.refparams) and {2}", String.Join(", ", attributesList.ToArray()),  table, filtr);
			
			#endregion

			DataUpdater du = new DataUpdater(adapter, null, db);
			du.Transaction = db.Transaction;
			return du;
		}

		/// <summary>
		/// Метод сохранения данных
		/// </summary>
		/// <param name="id"></param>
		/// <param name="db"></param>
		/// <param name="exMod"></param>
		/// <param name="vms"></param>
		private void SaveOfCalc(Int32 id, IDatabase db, ExcelModel exMod, ValidationMessages vms)
		{
			Trace.TraceVerbose("Сохранение данных...");
			IEntityAssociation item = Data.Associated[a_Forecast_IndValuesKey];
			IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
			parameters[0] = db.CreateParameter(item.FullDBName, id);

			///DataTable для данных получаемых из excel модели
			System.Data.DataTable dt_Ind;
			try
			{
				IWorkbookOfModel wbIndicators = exMod.ForecastModel.GetWorkBook("Индикаторы.xls");
				Worksheet wsIndicators = (Worksheet)wbIndicators.WorkBook.Worksheets.get_Item("Индикаторы");
				dt_Ind = wbIndicators.GetDataFromSheetY6(wsIndicators);
				ReleaseBookAndSheet(ref wbIndicators, ref wsIndicators);
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка при получении данных из модели: " + e.Message, e);
			}

			if (dt_Ind == null) 
				throw new ForecastException("Запрос индикаторов вернул пустую таблицу");

			String queryDictornary = String.Format("select ID, SIGNAT from d_Forecast_Parametrs where Code like '1_____'");
			DataTable dt = (DataTable)db.ExecQuery(queryDictornary, QueryResultTypes.DataTable);
			Dictionary<String, Int32> dic = new Dictionary<String, Int32>();
			if (dt != null)
			{
				foreach (DataRow datarow in dt.Rows)
				{
					dic.Add(Convert.ToString(datarow["SIGNAT"]), Convert.ToInt32(datarow["ID"]));
				}
			}
			else
				throw new ForecastException("Запрос сигнатур вернул пустую таблицу");


			/// DataTable для детали Индикаторы
			DataTable dtIndicators = new DataTable();
			using (IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("{0} = ?", item.FullDBName), null, parameters))
			{
				du.Fill(ref dtIndicators);

				try
				{
					int rowsProcessed = 0;
					foreach (DataRow row in dt_Ind.Rows)
					{
						String curParam = String.Empty;
						DataRow sdr;
						try
						{
							curParam = Convert.ToString(row["SIGNAT"]);
							sdr = (dtIndicators.Select("REFPARAMS = " + dic[curParam]))[0];
						}
						catch (Exception e)
						{
							throw new ForecastException("В словаре сигнатур индикаторов не найден параметр: " + curParam, e);
						}

						Boolean rowModified = false;

						sdr.BeginEdit();

						Int32 compareRes;

						compareRes = Decimal.Compare((Decimal)sdr["VALUEESTIMATE"], (Decimal)row["VALUEESTIMATE"]);
						if (compareRes != 0)
						{
							sdr["VALUEESTIMATE"] = row["VALUEESTIMATE"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";
							msg.Message = String.Format("{0}_{1}_{2}_{3}", Convert.ToInt32(sdr["ID"]), "VALUEESTIMATE", direction, curParam);
							vms.Add(msg);
						}

						compareRes = Decimal.Compare((Decimal)sdr["VALUEY1"], (Decimal)row["VALUEY1"]);
						if (compareRes != 0)
						{
							sdr["VALUEY1"] = row["VALUEY1"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";

                            var curid = Convert.ToInt32(sdr["ID"]);
                            var s = String.Format("{0}_{1}_{2}_{3}", curid, "VALUEY1", direction, curParam);

                            msg.Message =  s;
							vms.Add(msg);
						}

						compareRes = Decimal.Compare((Decimal)sdr["VALUEY2"], (Decimal)row["VALUEY2"]);
						if (compareRes != 0)
						{
							sdr["VALUEY2"] = row["VALUEY2"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";
							msg.Message = String.Format("{0}_{1}_{2}_{3}", Convert.ToInt32(sdr["ID"]), "VALUEY2", direction, curParam);
							vms.Add(msg);
						}

						compareRes = Decimal.Compare((Decimal)sdr["VALUEY3"], (Decimal)row["VALUEY3"]);
						if (compareRes != 0)
						{
							sdr["VALUEY3"] = row["VALUEY3"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";
							msg.Message = String.Format("{0}_{1}_{2}_{3}", Convert.ToInt32(sdr["ID"]), "VALUEY3", direction, curParam);
							vms.Add(msg);
						}

						compareRes = Decimal.Compare((Decimal)sdr["VALUEY4"], (Decimal)row["VALUEY4"]);
						if (compareRes != 0)
						{
							sdr["VALUEY4"] = row["VALUEY4"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";
							msg.Message = String.Format("{0}_{1}_{2}_{3}", Convert.ToInt32(sdr["ID"]), "VALUEY4", direction, curParam);
							vms.Add(msg);
						}

						compareRes = Decimal.Compare((Decimal)sdr["VALUEY5"], (Decimal)row["VALUEY5"]);
						if (compareRes != 0)
						{
							sdr["VALUEY5"] = row["VALUEY5"];
							rowModified = true;

							ValidationMessage msg = new ValidationMessage();
							String direction;
							if (compareRes == -1) // в базе меньше рассичтанного
								direction = "inc";
							else
								direction = "dec";
							msg.Message = String.Format("{0}_{1}_{2}_{3}", Convert.ToInt32(sdr["ID"]), "VALUEY5", direction, curParam);
							vms.Add(msg);
						}

						if (rowModified)
						{
							rowsProcessed++;
							sdr.EndEdit();
						}
						else
							sdr.CancelEdit();

					}
					Trace.TraceVerbose("Обработанно {0} из {1} строк", rowsProcessed, dt_Ind.Rows.Count);
				}
				catch (Exception e)
				{
					throw new ForecastException("Ошибка при обновлении DataRow для dtIndicators: " + e.Message, e);
				}

				du.Update(ref dtIndicators);
			}
			Trace.TraceVerbose("Данные сохранены");
		}

		/// <summary>
		/// Обработчик срабатывающий при обновлении записей из вьюшки для варианта расчета
		/// для регуляторов
		/// </summary>
		/// <param name="db"></param>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private bool DUOnAdjustersUpdate(IDatabase db, DataRow dataRow)
		{
			if (dataRow["V_EST_B"] == DBNull.Value)
				dataRow["V_EST_B"] = dataRow["ValueEstimate"];
			if (dataRow["V_Y1_B"] == DBNull.Value)
				dataRow["V_Y1_B"] = dataRow["ValueY1"];
			if (dataRow["V_Y2_B"] == DBNull.Value)
				dataRow["V_Y2_B"] = dataRow["ValueY2"];
			if (dataRow["V_Y3_B"] == DBNull.Value)
				dataRow["V_Y3_B"] = dataRow["ValueY3"];
			if (dataRow["V_Y4_B"] == DBNull.Value)
				dataRow["V_Y4_B"] = dataRow["ValueY4"];
			if (dataRow["V_Y5_B"] == DBNull.Value)
				dataRow["V_Y5_B"] = dataRow["ValueY5"];
			
			String queryUpdate = String.Format("update {0} set valueestimate = ?, valuey1 = ?, valuey2 = ?, valuey3 = ?, valuey4 = ?, valuey5 = ? where ID = ?", "t_forecast_adjvalues");
			Object o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
				db.CreateParameter("p0", dataRow["V_EST_B"]),
				db.CreateParameter("p1", dataRow["V_Y1_B"]),
				db.CreateParameter("p2", dataRow["V_Y2_B"]),
				db.CreateParameter("p3", dataRow["V_Y3_B"]),
				db.CreateParameter("p4", dataRow["V_Y4_B"]),
				db.CreateParameter("p5", dataRow["V_Y5_B"]),
				db.CreateParameter("p6", dataRow["ID"]));

			return Convert.ToInt32(o) > 0;
		}

		/// <summary>
		/// Обработчик срабатывающий при обновлении записей из вьюшки для варианта расчета
		/// для индикаторов
		/// </summary>
		/// <param name="db"></param>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private bool DUOnIndicatorsUpdate(IDatabase db, DataRow dataRow)
		{
            if (dataRow["V_EST_B"] == DBNull.Value)
				dataRow["V_EST_B"] = dataRow["ValueEstimate"];
			if (dataRow["V_Y1_B"] == DBNull.Value)
				dataRow["V_Y1_B"] = dataRow["ValueY1"];
			if (dataRow["V_Y2_B"] == DBNull.Value)
				dataRow["V_Y2_B"] = dataRow["ValueY2"];
			if (dataRow["V_Y3_B"] == DBNull.Value)
				dataRow["V_Y3_B"] = dataRow["ValueY3"];
			if (dataRow["V_Y4_B"] == DBNull.Value)
				dataRow["V_Y4_B"] = dataRow["ValueY4"];
			if (dataRow["V_Y5_B"] == DBNull.Value)
				dataRow["V_Y5_B"] = dataRow["ValueY5"];

			String queryUpdate = String.Format("update {0} set valueestimate = ?, valuey1 = ?, valuey2 = ?, valuey3 = ?, valuey4 = ?, valuey5 = ?, minbound = ?, maxbound = ?, leftpenaltycoef = ?, rightpenaltucoef = ? where ID = ?", "t_forecast_indvalues");
			Object o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
				db.CreateParameter("p0", dataRow["V_EST_B"]),
				db.CreateParameter("p1", dataRow["V_Y1_B"]),
				db.CreateParameter("p2", dataRow["V_Y2_B"]),
				db.CreateParameter("p3", dataRow["V_Y3_B"]),
				db.CreateParameter("p4", dataRow["V_Y4_B"]),
				db.CreateParameter("p5", dataRow["V_Y5_B"]),
				db.CreateParameter("p6", dataRow["MINBOUND"]),
				db.CreateParameter("p7", dataRow["MAXBOUND"]),
				db.CreateParameter("p8", dataRow["LEFTPENALTYCOEF"]),
				db.CreateParameter("p9", dataRow["RIGHTPENALTUCOEF"]),
				db.CreateParameter("p10", dataRow["ID"]));

			return true;
		}


		public static void ReleaseBookAndSheet(ref IWorkbookOfModel wb, ref Worksheet ws)
		{
			try
			{
				if (Marshal.IsComObject(ws))
					Marshal.ReleaseComObject(ws);
			} catch (Exception e) { }
			ws = null;

			try
			{
				if (Marshal.IsComObject(wb))
					Marshal.ReleaseComObject(wb);
			} catch (Exception e) { }
			wb = null;
		}

		public static void ReleaseBook(ref IWorkbookOfModel wb)
		{
			try
			{
				if (Marshal.IsComObject(wb))
					Marshal.ReleaseComObject(wb);
			}
			catch (Exception e) { }
			wb = null;
		}

	}
}
