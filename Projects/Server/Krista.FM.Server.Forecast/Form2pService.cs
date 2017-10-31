using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Forecast.ExcelAddin;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Forecast;
using Microsoft.Office.Interop.Excel;
using DataTable=System.Data.DataTable;

namespace Krista.FM.Server.Forecast
{
	public class Form2pService : ServerSideObject, IForm2pService
	{
		/// <summary>
		/// Ключ таблицы фактов формы 2п (варианты формы)
		/// </summary>
		public const string f_S_Form2p_Key = "bfd3c13b-079e-46a3-9133-4e616c7eb018";

		/// <summary>
		/// ID ассоциации мастер-деталь от таблицы фактов вариантов 2п к таблице деталей
		/// </summary>
		public const String a_Form2p_ParamValuesKey = "87433e2c-2187-4c5c-a53d-775deb7270ba";


		private static Form2pService instance;
		private IScheme scheme;

		public Form2pService(ServerSideObject owner)
			: base(owner)
		{
		}

		public static Form2pService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Form2pService(null);
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
		
		#region IForm2pService Members

		public IFactTable Data
		{
			get
			{
				return Instance.Scheme.FactTables[f_S_Form2p_Key];
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
		/// Метод создает набор параметров для деталей формы 2п с id.
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="estYear">оценочный год</param>
		/// <returns></returns>
		public Boolean CreateNewForm2p(Int32 id, Int32 estYear)
		{
			Trace.TraceVerbose("Cоздаем параметры для новой формы 2п с id={0}",id);
			///получаем объект для доступа к базе данных
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				//получаем SourceID по оценочному году
				String query = String.Format("select d.sourceid from d_forecast_forma2p d, hub_datasources t where (d.sourceid = t.id) and (t.year={0})", estYear);
				Object o = db.ExecQuery(query, QueryResultTypes.Scalar);
				if (o != null)
				{
					// если строка найдена, запоминаем SourceID 
					Int32 sourceID = Convert.ToInt32(o);
                    
					// Проверяем есть ли формы 2п на текущий год и по текущему региону,
					// но с другими сценарными условиями. Если есть то два отчетных года
					// мы должны взять из них. Берем с наибольшим кол. записей
					//String qOnThisYear = String.Format("select f.id from f_forecast_varform2p f, f_forecast_varform2p d " +
					//	"where (f.refterritory=d.refterritory) and (f.refyear=d.refyear) and (d.id = {0}) and (f.id <> d.id)", id);
					String qOnThisYear = String.Format("select f.id, (select  count(t.id) from t_forecast_paramvalues t where (t.refvarf2p = f.id) and (t.paramtype = 1)) as cnt " +
						"from f_forecast_varform2p f, f_forecast_varform2p d where (f.refterritory=d.refterritory) and (f.refyear=d.refyear) and (d.id = {0}) and (f.id <> d.id) " +
						"order by cnt desc", id);
					Object oOnThisYear = db.ExecQuery(qOnThisYear, QueryResultTypes.Scalar);

					Boolean foundOnThisYear = false;
					Boolean foundOnPrevYear = false;
					Int32 foundId = -1;

					// если не оказалось
					if (oOnThisYear == null)
					{
						// Проверяем есть ли формы 2п на прошедший год и по текущему региону,
						// но может быть с другими сценарными условиями. Если есть то первый отчетный год
						// мы должны взять из них.
						//						String qOnPrevYear = String.Format("select f.id from f_forecast_varform2p f, f_forecast_varform2p d " +
						//"where (f.refterritory=d.refterritory) and (f.refyear-1=d.refyear) and (d.id = {0})", id);
						String qOnPrevYear = String.Format("select f.id, (select  count(t.id) from t_forecast_paramvalues t where (t.refvarf2p = f.id) and (t.paramtype = 1)) as cnt " +
							"from f_forecast_varform2p f, f_forecast_varform2p d where (f.refterritory=d.refterritory) and (f.refyear-1=d.refyear) and (d.id = {0}) " +
							"order by cnt desc", id);

						Object oOnPrevYear = db.ExecQuery(qOnPrevYear, QueryResultTypes.Scalar);
						if (oOnPrevYear != null)
						{
							foundOnPrevYear = true;
							foundId = Convert.ToInt32(oOnPrevYear);
						}
					}
					else
					{
						foundOnThisYear = true;
						foundId = Convert.ToInt32(oOnThisYear);
					}
                    
					//Получаем набор id всех параметров формы 2п в разрезе sourceID
                    DataTable dt_param = (DataTable)db.ExecQuery(String.Format("select id, signat from d_forecast_forma2p where (sourceid={0}) and (Code <> -1)", sourceID), QueryResultTypes.DataTable);
					if ((dt_param != null) && (dt_param.Rows.Count > 0))
					{
						//						if (Data.Associated.Values.Count == 1)
						foreach (IEntityAssociation item in Data.Associated.Values)
						{
							//проверка ассоциации на соответсвие классу Таблица
							if (item.RoleData.ClassType == ClassTypes.Table)
							{
								//получаем имя поля для Foreign key
								IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
								parameters[0] = db.CreateParameter(item.FullDBName, id);

								//Получаем DataTable для деталей формы 2п 
								IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("{0} = ?", item.FullDBName), null, parameters);
								DataTable dt = new DataTable();
								du.Fill(ref dt);
                                
								foreach (DataRow row in dt_param.Rows)
								{
									//заполняем данные на оценчный год 0 и прогнозные 1,2,3
									for (Int32 i = 0; i <= 3; i++)
									{
										DataRow dr = dt.NewRow();
                                        dr["ID"] = item.RoleData.GetGeneratorNextValue; 
										dr["REFPARAMETRS"] = row["id"];
										dr[item.FullDBName] = id;
										switch (i)
										{
											case 0:
												dr["PARAMTYPE"] = 2;
												break;
											case 1:
											case 2:
											case 3:
												dr["PARAMTYPE"] = 3;
												break;
										}

										dr["YEAROF"] = estYear + i;
										dr["REFFORECASTTYPE"] = -1;
										dt.Rows.Add(dr);

                                        ////Trace.TraceVerbose("Создали данные для {0} года", i + estYear);
									}

									// если найден разрез по двум годам то заполняем
									if (row["signat"] != DBNull.Value)
									{
										if (foundOnThisYear)
										{
											for (Int32 i = 0; i <= 1; i++)
											{
												String qGetData = String.Format("select t.value from t_forecast_paramvalues t left join d_forecast_forma2p d on (d.id = t.refparametrs) " +
													"where (t.paramtype = 1) and (t.yearof = {0}) and (d.signat = '{1}') and (t.refvarf2p = {2})", estYear - 2 + i, row["signat"], foundId);
												Object oData = db.ExecQuery(qGetData, QueryResultTypes.Scalar);
												if (oData != null)
												{
													DataRow dr = dt.NewRow();
                                                    dr["ID"] = item.RoleData.GetGeneratorNextValue; 
													dr["REFPARAMETRS"] = row["id"];
													dr[item.FullDBName] = id;
													dr["PARAMTYPE"] = 1;
													dr["YEAROF"] = estYear - 2 + i;
													dr["REFFORECASTTYPE"] = -1;
													dr["VALUE"] = oData;
													dt.Rows.Add(dr);
												}
                                                
                                                ////Trace.TraceVerbose("Создали данные на основе предыдущих форм для {0} года", estYear - 2 + i);
											}
										}
										else
											if (foundOnPrevYear) // найден только на предыдущий заполняем
											{
												String qGetData = String.Format("select t.value from t_forecast_paramvalues t left join d_forecast_forma2p d on (d.id = t.refparametrs) " +
													"where (t.paramtype = 1) and (t.yearof = {0}) and (d.signat = {1}) and (t.refvarf2p = {2})", estYear - 1, row["signat"], foundId);
												Object oData = db.ExecQuery(qGetData, QueryResultTypes.Scalar);
												if (oData != null)
												{
													DataRow dr = dt.NewRow();
                                                    dr["ID"] = item.RoleData.GetGeneratorNextValue; 
													dr["REFPARAMETRS"] = row["id"];
													dr[item.FullDBName] = id;
													dr["PARAMTYPE"] = 1;
													dr["YEAROF"] = estYear - 1;
													dr["REFFORECASTTYPE"] = -1;
													dr["VALUE"] = oData;
													dt.Rows.Add(dr);
												}
                                                
                                                /////Trace.TraceVerbose("Создали данные на основе предыдущих форм для {0} года", estYear - 1);
											}
									}
								}
                                
                                ////Trace.TraceVerbose("Строк вставленно {0}", dt.GetChanges().Rows.Count);

                                Int32 affectedRows = du.Update(ref dt);

                                Trace.TraceVerbose("Строк созданно {0}", affectedRows);
							}
						}
					}
				}
				else return false;
			}
			return true;
		}

		private static readonly string[] FORM_FIELDS = new String[] { "id", "refparametrs", "refforecasttype", "est", "y1", "y2", "y3", "r1", "r2", "yearof", "refvarf2p" };

		/// <summary>
		/// Возвращает Data Updater для детали из вьюшки формы 2п 
		/// </summary>
		/// <param name="filtr">Дополнительные условия запроса присоеденяемые после where</param>
		/// <returns></returns>
		public IDataUpdater GetForm2pDetailUpdater(String filtr)
		{
			//DataUpdater du = GetDetailUpdater(filtr, "v_forecast_val_indicators", FORM_FIELDS);

			Database db = (Database)Scheme.SchemeDWH.DB;
			IDbDataAdapter adapter = db.GetDataAdapter();

			#region Select Command
			adapter.SelectCommand = db.InitCommand(null);

			List<string> attributesList = new List<string>();

			foreach (String p in FORM_FIELDS)
			{
				attributesList.Add("v."+p.ToUpper());
			}

            String includedQuery1 = "(select o.designation from d_units_okei o where v.refunits=o.id) as units";
            String includedQuery2 = "v.signat";
            String includedQuery3 = "v.groupname";
			/*if (Scheme.SchemeDWH.FactoryName.Contains("Oracle"))
                includedQuery3 = "(select  (select i.name from d_Forecast_forma2p i where (i.code <> -1) and (substr(o.code, 1, length(o.code)-8) = substr(i.code, 1, length(i.code)-8)) and (substr(i.code, length(i.code)-7, 8) = '00000000' )  and (i.sourceid = o.sourceid) ) from d_forecast_forma2p o  where (o.id=v.refparametrs)  and (o.code <> -1)) as groupname";
			else
				includedQuery3 = "(select  (select i.name from d_Forecast_forma2p i where (i.code <> -1) and (substring(CONVERT(VARCHAR(10), o.code), 1, len(CONVERT(VARCHAR(10), o.code))-8) = substring(CONVERT(VARCHAR(10), i.code), 1, len(CONVERT(VARCHAR(10), i.code))-8)) and (substring(CONVERT(VARCHAR(10), i.code), len(CONVERT(VARCHAR(10), i.code))-7, 8) = '00000000' ) and (i.sourceid = o.sourceid) ) from d_forecast_forma2p o  where (o.id=v.refparametrs) and (o.code <> -1)) as groupname";*/
			adapter.SelectCommand.CommandText = String.Format("Select {0}, {1}, {2}, {3} from {4} where {5}", String.Join(", ", attributesList.ToArray()), includedQuery1, includedQuery2, includedQuery3, "v_forecast_val_form2p v", filtr);
			

            /*"Select v.ID, v.REFPARAMETRS, v.REFFORECASTTYPE, v.EST, v.Y1, v.Y2, v.Y3, v.R1, v.R2, v.YEAROF, v.REFVARF2P, 
(select o.designation from d_units_okei o where v.refunits=o.id) as units,
 v.signat, v.groupname
 
 from v_forecast_val_form2p v 
 where (refvarf2p = 6) and (yearof = 2006)"*/
			#endregion

			DataUpdater du = new DataUpdater(adapter, null, db);
			du.Transaction = db.Transaction;

			du.OnInsteadUpdate += new InsteadUpdateEventDelegate(duOnDataUpdate);
			return du;
		}

		/// <summary>
		/// Обработчик срабатывающий при обновлении записей из вьюшки для формы 2п
		/// </summary>
		/// <param name="db"></param>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private bool duOnDataUpdate(IDatabase db, DataRow dataRow)
		{
			Int32 cellAffected = 0;
			Object o;
			Object tmp;
			String queryUpdate;

			tmp = db.ExecQuery("select id from t_forecast_paramvalues where (yearof + 2 = ?) and (refparametrs = ?) and (refvarf2p = ?)", QueryResultTypes.Scalar,
							db.CreateParameter("p0", dataRow["YEAROF"]),
							db.CreateParameter("p1", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p2", dataRow["REFVARF2P"]));
			if (tmp != null)
			{
				queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof + 2 = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
				o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
								db.CreateParameter("p0", dataRow["R1"]),
								db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
								db.CreateParameter("p2", dataRow["YEAROF"]),
								db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
								db.CreateParameter("p4", dataRow["REFVARF2P"]));
				cellAffected += Convert.ToInt32(o);
			}
			else
			{
				String queryInsert = "insert into t_forecast_paramvalues (id, value, paramtype, refparametrs, refforecasttype, refvarf2p, yearof) values(?, ?, ?, ?, ?, ?, ?)";
				o = db.ExecQuery(queryInsert, QueryResultTypes.NonQuery,
                                db.CreateParameter("p0", db.GetGenerator("g_t_forecast_paramvalues")),			
								db.CreateParameter("p1", dataRow["R1"]),
								db.CreateParameter("p2", 1), //отчетное значение
								db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
								db.CreateParameter("p4", dataRow["REFFORECASTTYPE"]),
								db.CreateParameter("p5", dataRow["REFVARF2P"]),
								db.CreateParameter("p6", Convert.ToInt32(dataRow["YEAROF"]) - 2));
				cellAffected += Convert.ToInt32(o);
			}

			tmp = db.ExecQuery("select id from t_forecast_paramvalues where (yearof + 1 = ?) and (refparametrs = ?) and (refvarf2p = ?)", QueryResultTypes.Scalar,
							db.CreateParameter("p0", dataRow["YEAROF"]),
							db.CreateParameter("p1", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p2", dataRow["REFVARF2P"]));
			if (tmp != null)
			{
				queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof + 1 = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
				o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
								db.CreateParameter("p0", dataRow["R2"]),
								db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
								db.CreateParameter("p2", dataRow["YEAROF"]),
								db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
								db.CreateParameter("p4", dataRow["REFVARF2P"]));
				cellAffected += Convert.ToInt32(o);
			}
			else
			{
				String queryInsert = "insert into t_forecast_paramvalues (id, value, paramtype, refparametrs, refforecasttype, refvarf2p, yearof) values(?, ?, ?, ?, ?, ?, ?)";
				o = db.ExecQuery(queryInsert, QueryResultTypes.NonQuery,
								db.CreateParameter("p0", db.GetGenerator("g_t_forecast_paramvalues")),
								db.CreateParameter("p1", dataRow["R2"]),
								db.CreateParameter("p2", 1),
								db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
								db.CreateParameter("p4", dataRow["REFFORECASTTYPE"]),
								db.CreateParameter("p5", dataRow["REFVARF2P"]),
								db.CreateParameter("p6", Convert.ToInt32(dataRow["YEAROF"])-1));
				cellAffected += Convert.ToInt32(o);
			}
			
			queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
			o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
							db.CreateParameter("p0", dataRow["EST"]),
							db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
							db.CreateParameter("p2", dataRow["YEAROF"]),
							db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p4", dataRow["REFVARF2P"]));
			cellAffected += Convert.ToInt32(o);

			queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof -1 = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
			o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
							db.CreateParameter("p0", dataRow["Y1"]),
							db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
							db.CreateParameter("p2", dataRow["YEAROF"]),
							db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p4", dataRow["REFVARF2P"]));
			cellAffected += Convert.ToInt32(o);

			queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof - 2 = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
			o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
							db.CreateParameter("p0", dataRow["Y2"]),
							db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
							db.CreateParameter("p2", dataRow["YEAROF"]),
							db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p4", dataRow["REFVARF2P"]));
			cellAffected += Convert.ToInt32(o);

			queryUpdate = String.Format("update {0} set  value = ? , refforecasttype = ?  where (yearof - 3 = ?) and (refparametrs = ?) and (refvarf2p = ?)", "t_forecast_paramvalues");
			o = db.ExecQuery(queryUpdate, QueryResultTypes.NonQuery,
							db.CreateParameter("p0", dataRow["Y3"]),
							db.CreateParameter("p1", dataRow["REFFORECASTTYPE"]),
							db.CreateParameter("p2", dataRow["YEAROF"]),
							db.CreateParameter("p3", dataRow["REFPARAMETRS"]),
							db.CreateParameter("p4", dataRow["REFVARF2P"]));
			cellAffected += Convert.ToInt32(o);

			return cellAffected > 0;
		}
			
		/// <summary>
		/// Метод рассчитывает модель и заполняет параметры формы 2п на основе этой модели
		/// </summary>
		/// <param name="scenId">id сценария рассчитываемого для формирования формы 2п</param>
		/// <param name="id2p">id из таблицы фактов для формы 2п</param>
		public void FillFromScen(Int32 scenId, Int32 id2p)
		{
			using (ExcelModel exMod = new ExcelModel())
			{
				using (IDatabase db = this.Scheme.SchemeDWH.DB)
				{
					// получение базового года для формы2п с ID=id2p
					String queryBY = String.Format("select refyear from f_forecast_varform2p where id = {0}", id2p);
					Int32 baseYear = Convert.ToInt32(db.ExecQuery(queryBY, QueryResultTypes.Scalar))-1;
					Trace.TraceVerbose("Расчет сценария с id={0}, baseyear={1}", scenId, baseYear);

					// получаем SourceID
					String querySID = String.Format("select d.sourceid from d_forecast_forma2p d, hub_datasources t where (d.sourceid = t.id) and (t.year={0}) group by sourceid", baseYear+1);
					Int32 sid = Convert.ToInt32(db.ExecQuery(querySID, QueryResultTypes.Scalar));

					CalcProccess.MainCalcProc(scenId, db, exMod);

					Trace.TraceVerbose("Передача отчетных данных в форму 2п");

					String queryForm2p;
					if (Scheme.SchemeDWH.FactoryName.Contains("Oracle"))
						queryForm2p = String.Format("select d.signat, t1.value as r1, t2.value as r2 from t_forecast_paramvalues t1 "+
							"left join t_forecast_paramvalues t2 on ((t1.refvarf2p=t2.refvarf2p) and (t1.paramtype = t2.paramtype) and (t1.refparametrs = t2.refparametrs) and (t1.yearof = t2.yearof-1)) "+
							"left join d_forecast_forma2p d on (t1.refparametrs = d.id)"+
							"where (t1.paramtype = 1) and (instr(d.signat,'_I')<>0) and (t1.yearof = {0}) and (t1.refvarf2p={1})", baseYear-1, id2p);
					else
						queryForm2p = String.Format("select d.signat, t1.value as r1, t2.value as r2 from t_forecast_paramvalues t1 "+
							"left join t_forecast_paramvalues t2 on ((t1.refvarf2p=t2.refvarf2p) and (t1.paramtype = t2.paramtype) and (t1.refparametrs = t2.refparametrs) and (t1.yearof = t2.yearof-1)) "+
							"left join d_forecast_forma2p d on (t1.refparametrs = d.id)"+
							"where (t1.paramtype = 1) and (charindex(d.signat,'_I')<>0) and (t1.yearof = {0}) and (t1.refvarf2p={1})", baseYear-1, id2p);
					DataTable dtForm2p = (DataTable)db.ExecQuery(queryForm2p, QueryResultTypes.DataTable);

					if (dtForm2p != null)
					{
						IWorkbookOfModel wbRetData = exMod.ForecastModel.GetWorkBook("Форма 2п-ОКВЭД.xls");
						Worksheet wshRetData = (Worksheet)wbRetData.WorkBook.Worksheets.get_Item("Форма_2п");
						wbRetData.SetDataToCellForm2p_Masked(wshRetData, dtForm2p);
					}

					Trace.TraceVerbose("Сохранение данных...");
					
					SaveCalcedForm2p(id2p, db, baseYear, exMod, sid);
				}
			}
		}


		public String GetNameForm2pByID(Int32 id)
		{
			Trace.TraceVerbose("Получаем наименование для формы 2п с id={0}", id);
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				String query = String.Format("select name from f_forecast_varform2p where id = {0}", id);
				Object o = db.ExecQuery(query, QueryResultTypes.Scalar);
				if (o != null)
				{
					return o.ToString();
				}
				else
					return String.Format("Форма 2П с заданным id={0} не существует", id);
			}
		}

		public void GetNameYearRegionForm2pByID(Int32 id, out String name, out Int32 year, out Int32 reg)
		{
			Trace.TraceVerbose("Получаем наименование для формы 2п с id={0}", id);
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				String query = String.Format("select name, refyear, refterritory from f_forecast_varform2p where id = {0}", id);
				DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
				if (dt != null)
				{
					name = dt.Rows[0]["NAME"].ToString();
					year = Convert.ToInt32(dt.Rows[0]["refyear"]);
					reg = Convert.ToInt32(dt.Rows[0]["refterritory"]);
				}
				else 
				{
					name = "";
					year = -1;
					reg = -1;
				}
			}
		}

		public Byte[] SaveFormToExcel(Int32 v1, Int32 v2, Int32 year)
		{
			FileStream fs = null;
			Byte[] documentData = null;

			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				using (ExcelForm exForm = new ExcelForm(db, year))
				{
					Trace.TraceVerbose("Передача отчетных данных в форму 2п");

					String queryForm2p;
					queryForm2p = String.Format("select v1.refparametrs, v1.signat, v1.r1 as r1, v1.r2 as r2, v1.est as est, v1.y1 as v1_y1, v2.y1 as v2_y1, v1.y2 as v1_y2, v2.y2 as v2_y2, v1.y3  as v1_y3, v2.y3 as v2_y3 from v_forecast_val_form2p v1 " +
						"left join (select y1, y2, y3, refparametrs from v_forecast_val_form2p where refvarf2p = {1}) v2 on (v1.refparametrs = v2.refparametrs) where (v1.refvarf2p = {0})", v1, v2);
					
					DataTable dtForm2p = (DataTable)db.ExecQuery(queryForm2p, QueryResultTypes.DataTable);

					if (dtForm2p != null)
					{
						IWorkbookOfModel wbForm = exForm.ForecastModel.GetWorkBook(ExcelForm.DefFileName);
						Worksheet wshRetData = (Worksheet)wbForm.WorkBook.Worksheets.get_Item("Форма_2п");
						wbForm.SetDataToForm2p(wshRetData, dtForm2p);
						Trace.TraceVerbose("Сохранение временных данных...");
						wbForm.SaveWorkbook();
						wbForm.CloseWorkbook();
					}
					else 
						throw new ForecastException("Запрос отчетных данных формы 2П вернул пустую строку");
				
				
					Trace.TraceVerbose("Передача данных на клиент...");

					try
					{
						FileInfo fi = new FileInfo(exForm.FileName);
						//fs = new FileStream(, FileMode.Open, FileAccess.Read);
						fs = fi.OpenRead();
						if (fi.Length > Int32.MaxValue)
							throw new ForecastException("Ошибка в загрузке файла Формы-2П. Превышен максимальный размер загрузки");
						documentData = new Byte[(Int32)fi.Length];
						fs.Read(documentData, 0, (Int32)fi.Length);
					}
					finally
					{
						if (fs != null)
						{
							fs.Flush();
							fs.Close();
						}
					}
				}
			}
			return documentData;
		}

		/// <summary>
		/// Метод закачивает данные из других таблиц в форму 2п
		/// </summary>
		public void PumpFromAnotherTable(Int32 id, Boolean replace)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			settings.ValidationType = ValidationType.DTD;

			String S = Path.Combine(scheme.BaseDirectory, "Forecast\\pump.xml");
			XmlReader reader = XmlReader.Create(S, settings);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);

			Dictionary<String, String> sqlList = new Dictionary<String, String>();

			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				String query = String.Format("select refyear from f_forecast_varform2p where id = {0}", id);
				Int32 baseyear = Convert.ToInt32(db.ExecQuery(query, QueryResultTypes.Scalar));
				
				try
				{
					XmlNode rootNode = doc.GetElementsByTagName("pump")[0];
					foreach (XmlNode node in rootNode.ChildNodes)
					{
						if (node.Name == "forma2p")
						{
							//String toTable = node.Attributes["name"].Value;
							foreach (XmlNode param in node.ChildNodes)
							{
								if (param.Name == "param")
								{
									if (param.Attributes["type"].Value == "fact")
									{
										String fromTable = param.Attributes["table"].Value;
										String paramName = param.Attributes["name"].Value;
										String dataRow = param["data"].InnerText;
										String yearRow = param["year"].InnerText;
										String k = (Convert.ToDecimal(param["data"].Attributes["format"].Value)/Convert.ToDecimal(param.Attributes["format"].Value)).ToString().Replace(',','.');

										
										String filter = String.Empty;
										XmlNode filtersNode = param["filters"];
										if (filtersNode.HasChildNodes)
											foreach (XmlNode filterNode in filtersNode.ChildNodes)
											{
												filter = String.Format(" {0} and ({1} = {2}) ", filter, filterNode.Attributes["name"].Value, filterNode.InnerText);
											}

										for (Int32 i = baseyear - 2; i < baseyear; i++)
										{
											Int32 year;
											switch (param["year"].Attributes["format"].Value)
											{
												case "unv":
													year = i * 10000 + 0001;
													break;
												case "year":
												default:
													year = i;
													break;
											}

											String selectSQL = String.Format("select {0}*{1} from {2} where ({3}={4}) {5}", dataRow, k, fromTable, yearRow, year, filter);

											Object o = db.ExecQuery(String.Format("select t.id from t_forecast_paramvalues t left join d_forecast_forma2p d on t.refparametrs = d.id where (d.signat = '{0}') and (t.refvarf2p = {1}) and (t.yearof={2})", paramName, id, i),QueryResultTypes.Scalar);
											String updateSQL;
											if ((o != null) && (o != DBNull.Value))
											{
												if (replace)
													updateSQL = String.Format("update t_forecast_paramvalues d set value = :value where d.id={0}", o);
												else updateSQL = String.Empty;
											}
											else
											{
												Int32 sid = Convert.ToInt32(db.ExecQuery(String.Format("select d.sourceid from d_forecast_forma2p d, hub_datasources t where (d.sourceid = t.id) and (t.year={0}) group by d.sourceid", baseyear),QueryResultTypes.Scalar));
												Int32 refparam = Convert.ToInt32(db.ExecQuery(String.Format("select id from d_forecast_forma2p where (sourceid = {0}) and (signat = '{1}')", sid, paramName),QueryResultTypes.Scalar));
												updateSQL = String.Format("insert into t_forecast_paramvalues (value, paramtype, refparametrs, refforecasttype, refvarf2p, yearof) values(:value, 1, {0}, -1, {1}, {2})", refparam, id, i);
											}
											sqlList.Add(selectSQL, updateSQL);
										}
									}
								}
							}
						}
					}
				}
				catch (ForecastException e)
				{
					throw new ForecastException(e.Message, e);
				}

				//List<String> lst =  XmlHelper.GetStringListFromXmlNode(doc.GetElementsByTagName("pump")[0]);

				doc.LoadXml("<xml></xml>");
				doc = null;

				foreach (KeyValuePair<String, String> pair in sqlList)
				{
					Object o = null;
					if (pair.Key != String.Empty)
					{
						try
						{
							o = db.ExecQuery(pair.Key, QueryResultTypes.Scalar);
						}
						catch (ForecastException e)
						{
						}
						if ((o != null) && (o != DBNull.Value))
						{
							Object o2 = null;
							try
							{
								o2 = db.ExecQuery(pair.Value, QueryResultTypes.NonQuery, db.CreateParameter("value", o));
							}
							catch (ForecastException e)
							{
							}
							if ((o2 == null) || (Convert.ToInt32(o2) == 0))
							{
								Trace.TraceVerbose("Ошибка записи параметра, не удалось выполнить запрос: {0}",pair.Value);
							}
						}
						else
							Trace.TraceVerbose("Ошибка получения значения параметра, не удалось выполнить запрос: {0}", pair.Key);
					}
				}
			}
		}

		public void AlternativeForecast(Int32 id)
		{
			using (IDatabase db = this.Scheme.SchemeDWH.DB)
			{
				const Int32 l = 1;
				const Double verySmall = 1E-50;

				///Часть загрузки данных из БД
				DataTable dt = (DataTable)db.ExecQuery(String.Format("select refterritory, refyear from f_forecast_varform2p where id = {0}", id), QueryResultTypes.DataTable);
				if ((dt == null) || dt.Rows.Count == 0)
					return;
				Int32 terr = Convert.ToInt32(dt.Rows[0]["refterritory"]);
				Int32 year = Convert.ToInt32(dt.Rows[0]["refyear"]);

				String querySID = String.Format("select d.sourceid from d_forecast_forma2p d, hub_datasources t where (d.sourceid = t.id) and (t.year={0}) group by sourceid", year);
				Int32 sid = Convert.ToInt32(db.ExecQuery(querySID, QueryResultTypes.Scalar));

				//String queryParam = String.Format("select d.signat, t.value from dv.t_forecast_paramvalues t left join dv.d_forecast_forma2p d on d.id = t.refparametrs where (t.refvarf2p = {0}) and (t.paramtype = 2) and (t.value is not null)", id);
				String queryParam = String.Format("select v.signat, v.r1 as value2, v.r2 as value1, v.est, v.y1, v.y2, v.y3 from v_forecast_val_form2p v where (v.r1 is not null) and (v.r2 is not null) and (v.est is not null) and (v.refvarf2p = {0})", id);
				DataTable valuetable = (DataTable)db.ExecQuery(queryParam, QueryResultTypes.DataTable);
				if ((valuetable == null) || (valuetable.Rows.Count == 0))
					return;

				Int32 k = 0;
				Boolean endLoop = false;
				do
				{
					k++;
					String prevYears = String.Format("select d.signat, t.value from dv.t_forecast_paramvalues t left join dv.d_forecast_forma2p d on d.id = t.refparametrs left join dv.f_forecast_varform2p f on f.id = t.refvarf2p where (t.paramtype = 1) and (t.value is not null) and (f.refterritory = {0}) and (t.yearof = {1}) group by d.signat, t.value", terr, year-k-2);
					dt = (DataTable)db.ExecQuery(prevYears, QueryResultTypes.DataTable);
					Boolean found = false;
					if ((dt != null) && (dt.Rows.Count != 0))
					{
						String colname = String.Format("value{0}", k+2);

						foreach (DataRow dr in valuetable.Rows)
						{
							String sig = dr["signat"].ToString();
							DataRow[] row = dt.Select(String.Format("signat = '{0}'", sig));
							if ((row != null) && (row.Length != 0))
							{
								if (!found)
								{
									valuetable.Columns.Add(colname);
									found = true;
								}
								dr[colname] = row[0]["value"];
							}
						}

					}
					//else endLoop = true;
					if (!found)
						endLoop = true;
				}
				while(!endLoop);
				
				/*String df = String.Empty;
				foreach (DataRow dr in valuetable.Rows)
				{
					String s = String.Format("{0};{1};{2};{3};{4};{5}'", dr["SIGNAT"], dr["value4"], dr["value3"], dr["value2"], dr["value1"], dr["est"]);
					df += s;
				}*/

				// Прогнозная часть
				Int32 paramCount = valuetable.Rows.Count; // he0
				Int32 yearsFound = 3 + k-1; //N_year

				DataTable resultAFData = valuetable.Copy();
				Boolean[] predefData = new Boolean[paramCount];
				
				Double[,] norm0 = new Double[paramCount, yearsFound]; ///Матрица значений по параметрам и годам 
				Double[,] normWork = new Double[paramCount, yearsFound]; ///Рабочая матрица значений по параметрам и годам 
				Boolean[,] haveValue = new Boolean[paramCount, yearsFound]; /// Bin0 Матрица наличия значений на параметры и годы
				Boolean[] rowHaveValues = new Boolean[paramCount];  ///Bin1 существование строки 
				Int32[] nYearsInRow = new Int32[paramCount];
				
				Int32 curRow = 0; //внутренний цикл для строчек
				Int32 predefCount = 0;
				foreach (DataRow dr in valuetable.Rows)
				{
					if ((dr["y1"] != DBNull.Value) || (dr["y2"] != DBNull.Value) || (dr["y3"] != DBNull.Value))
					{
						predefData[curRow] = true;
						predefCount++;
					}
					else
						predefData[curRow] = false;
					
					Int32 foundCount = 0; ///количество найденных лет для текущего параметра
					///Заполняем значения из таблицы запроса в матрицу
					///и заполнение флага наличия значения
					for (Int32 j = 0; j < yearsFound-1; j++)
					{
						Object val = dr[String.Format("value{0}", yearsFound-j-1)];
						if ((val != null) && (val != DBNull.Value))
						{
							norm0[curRow, j] = Convert.ToDouble(val);
							haveValue[curRow, j] = true;
							foundCount++;
						}
						else
						{
							haveValue[curRow, j] = false;
						}
					}
					Object valEst = dr["est"];
					if ((valEst != null) && (valEst != DBNull.Value))
					{
						norm0[curRow, yearsFound-1] = Convert.ToDouble(valEst);
						haveValue[curRow, yearsFound-1] = true;
						foundCount++;
					}
					else
						throw new ForecastException("Не найдено значение на оценочный год!");

					if (foundCount < 3)
					{
						rowHaveValues[curRow] = false;
						throw new ForecastException("Запрос отработал неккоректно, число значений не может быть меньше 3-х!");
					}
					else
					{
						rowHaveValues[curRow] = true;
						nYearsInRow[curRow] = foundCount;
					}
					
					curRow++;
				}

				Int32 paramChecked = 0;
				Double [] aver = new Double[paramCount];

	/*			String df = String.Empty;
				String df1 = String.Empty;*/

				for (Int32 n = 0; n < paramCount; n++)
				{
					/*String s = String.Empty;
					String s1 = String.Empty;*/
					//Double absAver = 0;
					for (Int32 j = 0; j < yearsFound ; j++)
					{
						aver[n] += norm0[n, j];
						// TODO а если aver==0 то далее деление на ноль
					//	absAver += Math.Abs(norm0[n, j]);
					}
										
					if (aver[n] == 0)
					{
						rowHaveValues[n] = false;
						for (Int32 j = 0; j < yearsFound; j++)
						{
							haveValue[n, j] = false;
						}
					}
					else
					{
						for (Int32 j = 0; j < yearsFound; j++)
						{
							normWork[n, j] = norm0[n, j] * nYearsInRow[n] / aver[n];
						}
						paramChecked++;
					}

					/*for (Int32 j = 0; j < yearsFound; j++)
						s1 += String.Format("{0},", normWork[n, j]);

					s = aver[n].ToString();

					df += s + ";";
					df1 += s1 + ";";*/
				}
				
				

				///Cоздаем матрицу корреляции
				//df = String.Empty;
				
				Boolean[,] linkedRows = new Boolean[paramCount, paramCount];
				Double[] vect = new Double[paramCount];
				SortedArray tmpvect = new SortedArray();
				Double[,] matr =  new Double[paramCount, paramCount];
				Int32 countLinked = 0;

				for (Int32 paramRow = 0; paramRow < paramCount ; paramRow++)
				{
					if (!rowHaveValues[paramRow])
						continue; 
					tmpvect.Clear();
					for (Int32 paramCol = 0; paramCol < paramCount ; paramCol++)
					{
						if (!rowHaveValues[paramCol])
							continue; 

						Double sig_x = 0;
						Double sig_y = 0;
						Double sig_xy = 0;
						
						Int32 com_p = 0;

						for (Int32 z = 0; z < yearsFound; z++)
						{
							if (haveValue[paramRow, z] && haveValue[paramCol, z])
							{
								sig_x += Math.Pow(normWork[paramRow, z] - 1, 2);
								sig_y += Math.Pow(normWork[paramCol, z] - 1, 2);
								sig_xy += (normWork[paramRow, z] - 1) * (normWork[paramCol, z] - 1);
								//curVect += Math.Pow(normWork[paramRow, z] - normWork[paramCol, z], 2);
								com_p++;
							}
						}

						Double curVect;
						if (com_p < 2) 
							curVect =1;
						else 
							if ((sig_x + sig_y) == 0)
							curVect = 0;
							else
								curVect = 1 - 2 * sig_xy / (sig_x + sig_y);
						tmpvect.Add(paramCol, curVect);
						vect[paramCol] = curVect;

						/*String s = String.Format("{0},{1}"+Environment.NewLine, paramCol, curVect);
						df += s;*/
					}

					///sort
					
					Int32[] indx = tmpvect.IndexToArray();

					/*df = String.Empty;
					foreach (KeyValuePair<int, double> pair in tmpvect)
					{
						Double v = pair.Value;
						String s1 = Convert.ToString(v);
						String s = String.Format("{0},{1}" + Environment.NewLine, pair.Key, s1);
						df += s;
					}*/
					
					Int32 h = 0;
					while ((vect[indx[h+1]] < 0.1) && (h < 5)) 
						h++;

					if (h < 2)
					{
						h = 0;
						while ((vect[indx[h+1]] < 0.3) && (h < 3)) 
						h++;
					}

					for (Int32 j1 = 0; j1 <= h; j1++)
					{
						if (paramRow != indx[j1])
						{
							linkedRows[paramRow, indx[j1]] = true;
							countLinked++;
						}
						else
							linkedRows[paramRow, indx[j1]] = false;
					}

					for (Int32 j1 = 0; j1 < paramCount; j1++)
						matr[paramRow, j1] = vect[j1];
				}

				/*df = String.Empty;
				for (Int32 i = 0; i < paramCount ; i++)
				{
					String s = String.Empty;
					for (Int32 j = 0; j < paramCount; j++)
					{
						s += String.Format("{0},", linkedRows[i,j] );
					}
					s += Environment.NewLine;
					df += s;
				}*/


				/*for (Int32 j = 0; j < paramCount; j++)
				{
					linkedRows[j, j] = false;
				}

				Int32 he_H = 0;
				for (Int32 i = 0; i < paramCount; i++)
					for (Int32 j = 0; j < paramCount; j++)
						if (linkedRows[i, j])
							he_H++;*/
				
				//df = String.Empty;
				Double[] xp = new Double[paramCount];

				for (Int32 i = 0; i < paramCount; i++)
				{
					Int32 n_X = 0;
					Int32 x2sr = 0;
					Int32 xsr = 0;
					Double yxsr = 0;
					Double ysr = 0;

					if (rowHaveValues[i])
					{
						for (Int32 j = 0; j < yearsFound; j++)
						{
							if ((j >= yearsFound) || (haveValue[i, j]))
							{
								n_X += 1;
								x2sr += j * j;
								xsr += j;
								yxsr += j * normWork[i, j];
								ysr += normWork[i, j];
							}
						}

						if (n_X < 3)
							throw new ForecastException("Ошибка работы алгоритма. По одному или нескольким параметрам статистических данных меньше чем на три года!");

						Double h1_0 = (yxsr - ysr * xsr / n_X) / (x2sr - xsr * xsr / n_X);
						Double h1_1 = (ysr - h1_0 * xsr) / n_X;

						xp[i] = h1_0 * yearsFound + h1_1;
					}
					//df += String.Format("{0},", xp[i]);
				}

				Double[,] h1 = new Double[countLinked, paramCount];
				Double[] alfa_min = new Double[countLinked];
				Int32 lnkEquCount = 0;
				for (Int32 i = 0; i < paramCount; i++)
				{
					for (Int32 j = 0; j < paramCount; j++)
					{
						if (linkedRows[i, j])
						{
							Double sig_x = 0;
							Double sig_y = 0;
							Double sig_xy = 0;
							//Double curVect = 0;
							Double sig2;
							Int32 com_p = 0;

							for (Int32 j1 = 0; j1 < yearsFound; j1++)
								if (haveValue[i, j1] && haveValue[j, j1])
								{
									com_p++;
								}
							Double aver_x = (com_p + xp[i]) / (com_p + 1);
							Double aver_y = (com_p + xp[j]) / (com_p + 1);

							for (Int32 j1 = 0; j1 < yearsFound; j1++)
								if (haveValue[i, j1] && haveValue[j, j1])
								{
									sig_x += Math.Pow(normWork[i, j1] - aver_x, 2);
									sig_y += Math.Pow(normWork[j, j1] - aver_y, 2);
									sig_xy += (normWork[i, j1] - aver_x) * (normWork[j, j1] - aver_y);
								}

							if (sig_x + sig_y == 0)
								sig2 = 0;
							else
								sig2 = 2 * sig_xy / (sig_x + sig_y);
							
							Double Ro;
							if (Math.Abs(sig2).CompareTo(0.4) < 0)
								Ro = 0.4;
							else
								Ro = 1 - matr[i, j];

							h1[lnkEquCount, i] = 1;
							

							if (sig_x > sig_y)
							{
								Double t = (1 / Ro + Math.Sqrt(1 / Ro / Ro - 1));
								h1[lnkEquCount, j] = -t;
								alfa_min[lnkEquCount] = t * aver_y - aver_x;
							}
							else
							{
								Double t = (1 / Ro - Math.Sqrt(1 / Ro / Ro - 1));
								h1[lnkEquCount, j] = -t;
								alfa_min[lnkEquCount] = t * aver_y - aver_x;
							}

							lnkEquCount++;
						}
					}
				}

				/*df = String.Empty;
				df1 = String.Empty;

				for (Int32 i = 0; i < lnkEquCount; i++)
				{
					String s = String.Empty;
					for (Int32 j = 0; j < paramCount; j++)
					{
						s += String.Format("{0}," , h1[i, j]);
					}
					df += s + Environment.NewLine;
					df1 += String.Format("{0}," , alfa_min[i]);
				}*/

				Double[,] H2 = new Double[paramCount, paramCount];
				Double[,] A = new Double[paramCount, paramCount];
				for (Int32 i = 0; i < paramCount; i++)
				{
					if (!rowHaveValues[i])
						continue;

					for (Int32 j1 = 0; j1 < countLinked; j1++)
						if (h1[j1, i] != 0)
							for (Int32 j = 0; j < paramCount; j++)
							{
								if (!rowHaveValues[j])
									continue;

								H2[i, j] += h1[j1, i] * h1[j1, j];
							}
				}

				for (Int32 i = 0; i < paramCount; i++)
					for (Int32 j = 0; j < paramCount; j++)
					{
						A[i, j] = H2[i, j];
					}

				for (Int32 i = 0; i < paramCount; i++)
				{
					if (!rowHaveValues[i])
						continue;
					
					A[i, i] += l;
				}

				/*df = String.Empty;
				for (Int32 i = 0; i < paramCount; i++)
				{
					String s = String.Empty;
					for (Int32 j = 0; j < paramCount; j++)
					{
						s += String.Format("{0},", H2[i, j]);
					}
					df += s + Environment.NewLine;
				}*/

				Double[,] normWorkEx = new Double[paramCount, 3]; ///Рабочая матрица значений по параметрам и годам 

				for (Int32 curYear = 1; curYear <= 3; curYear++)
				{
					for (Int32 i = 0; i < paramCount; i++)
					{
						Int32 n_X = 0;
						Int32 x2sr = 0;
						Int32 xsr = 0;
						Double yxsr = 0;
						Double ysr = 0;

						if (!rowHaveValues[i])
							continue;
						
						for (Int32 j = 0; j < yearsFound + curYear - 1; j++)
						{
							if (j < yearsFound)
							{
								if (haveValue[i, j])
								{
									n_X += 1;
									x2sr += j * j;
									xsr += j;
									yxsr += j * normWork[i, j];
									ysr += normWork[i, j];
								}
							}
							else
							{
								n_X += 1;
								x2sr += j * j;
								xsr += j;
								yxsr += j * normWorkEx[i, j-yearsFound];
								ysr += normWorkEx[i, j-yearsFound];
							}
						}
						Double h1_0 = (yxsr - ysr * xsr / n_X) / (x2sr - xsr * xsr / n_X);
						Double h1_1 = (ysr - h1_0 * xsr) / n_X;

						xp[i] = h1_0 * (yearsFound + curYear - 1) + h1_1;
					
					}


					Double[] b = new Double[paramCount];
					Double[] r = new Double[paramCount];
					for (Int32 i = 0; i < paramCount; i++)
					{
						if (!rowHaveValues[i])
							continue;

						b[i] = l * xp[i];
						for (Int32 j = 0; j < countLinked; j++)
						{
							b[i] -= h1[j, i] * alfa_min[j];
						}
						r[i] = b[i];
					}

/*					df = String.Empty;
					for (Int32 i = 0; i < paramCount; i++)
					{
						df += String.Format("{0}, ", r[i]);
					}*/


					Double[] w = new Double[paramCount];

					Double[] x = new Double[paramCount]; // ?????????????????????????
					Double[] z2 = new Double[paramCount];

					Double alfa = 0;
					Double alfa_1 = 0;

					for (Int32 i = 0; i < paramCount; i++)
					{
						if (!rowHaveValues[i])
							continue;
												
						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							r[i] -= A[i, j] * x[j]; 
						}

						w[i] = -r[i];
					}

					for (Int32 i = 0; i < paramCount; i++)
					{
						z2[i] = 0;
						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							z2[i] += A[i, j] * w[j];
						}
						
						alfa += r[i] * w[i];
						alfa_1 += z2[i] * w[i];
					}

					
					if (Math.Abs(alfa).CompareTo(verySmall) < 0 )
						alfa = 0;
					else
						alfa /= alfa_1;

					for (Int32 j = 0; j < paramCount; j++)
					{
						if (!rowHaveValues[j])
							continue;
						x[j] += alfa * w[j];
					}


					/*df = String.Empty;
					for (Int32 j = 0; j < paramCount; j++)
					{
						df += String.Format("{0}, ", x[j]);
					}*/



					for (Int32 count = 0; count < paramCount; count++)
					{
						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							r[j] -= alfa * z2[j];
						}

						Double beta = 0;
						Double beta_1 = 0;

						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							beta += r[j] * z2[j];
							beta_1 += z2[j] * w[j];
						}

						if (Math.Abs(beta).CompareTo(verySmall) < 0)
							beta = 0;
						else
							beta /= beta_1;

						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;
							w[j] = -r[j] + beta * w[j];
						}

						for (Int32 i = 0; i < paramCount; i++)
						{
							if (!rowHaveValues[i])
								continue;

							z2[i] = 0;
							for (Int32 j = 0; j < paramCount; j++)
							{
								if (!rowHaveValues[j])
									continue;
								z2[i] += A[i, j] * w[j];
							}
						}

						alfa = 0;
						alfa_1 = 0;

						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							alfa += r[j] * w[j];
							alfa_1 += z2[j] * w[j];
						}

						if (Math.Abs(alfa).CompareTo(verySmall) < 0)
							alfa = 0;
						else
							alfa /= alfa_1;

						for (Int32 j = 0; j < paramCount; j++)
						{
							if (!rowHaveValues[j])
								continue;

							x[j] += alfa * w[j];
						}
					} //count


					/*df = String.Empty;
					for (Int32 j = 0; j < paramCount; j++)
					{
						df += String.Format("{0}, ", w[j]);
					}*/

					/* for (Int32 i = 0; i < paramCount; i++)
                        {
                            z2[i] = -b[i];
                            for (Int32 j = 0; j < paramCount; j++)
                            {
                                z2[i] += A[i, j] * x[j];
                            }
                        }*/

						/*df = String.Empty;
						for (Int32 j = 0; j < paramCount; j++)
						{
							df += String.Format("{0}, ", z2[j]);
						}*/

					for (Int32 i = 0; i < paramCount; i++)
					{
						if (!rowHaveValues[i])
							continue;

						normWorkEx[i, curYear - 1] = x[i];
					}
										
					for (Int32 i = 0; i < paramCount; i++)
					{
						if (rowHaveValues[i])
						{
							String colName = String.Format("y{0}", curYear);
							
							resultAFData.Rows[i][colName] = x[i] * aver[i] / yearsFound;
						}
					}
				}

				if (predefCount > 0)
					ModelAdaptation(valuetable, resultAFData, H2, normWork, rowHaveValues, aver, paramCount, predefCount, yearsFound, predefData, nYearsInRow);

				/*df = String.Empty;
				foreach (DataRow dr in valuetable.Rows)
				{
					String s = String.Format("{0};{1};{2};{3}'", dr["SIGNAT"], dr["y1"], dr["y2"], dr["y3"]);
					df += s + Environment.NewLine;
				}*/
				
				//ссылка на детали Вариантов формы 2п (Показатели формы 2п)
				IEntityAssociation item = Data.Associated[a_Form2p_ParamValuesKey];
				IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
				//Параметр с id варианта формы 2п активной строки в мастер гриде 
				parameters[0] = db.CreateParameter(item.FullDBName, id);
				//Параметр для выбора прогнозных значений формы 2п
				//parameters[1] = db.CreateParameter("ParamType", 3);

				IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("({0} = ?)", item.FullDBName), null, parameters); /*and (PARAMTYPE = ?)*/

				/// DataTable для детали 
				DataTable dtParams = new DataTable();
				du.Fill(ref dtParams);

				// Создаем словарь сигнатур для формы 2п на текущий год 
				String queryDictornary = String.Format("select ID, SIGNAT from d_forecast_forma2p where (SOURCEID = {0}) and (CODE <> -1)", sid);
				DataTable dtab = (DataTable)db.ExecQuery(queryDictornary, QueryResultTypes.DataTable);
				Dictionary<String, Int32> dic = new Dictionary<String, Int32>();

				if (dtab != null)
				{
					foreach (DataRow datarow in dtab.Rows)
					{
						dic.Add(Convert.ToString(datarow["SIGNAT"]), Convert.ToInt32(datarow["ID"]));
					}
				}

				try
				{
					int rowsProcessed = 0;
					foreach (DataRow row in resultAFData.Rows)
					{
						String curParam = String.Empty;
						DataRow[] sdr = null;

						try
						{
							curParam = Convert.ToString(row["SIGNAT"]);
							sdr = (dtParams.Select("REFPARAMETRS = " + dic[curParam]));
						}
						catch (Exception e)
						{
							//throw new ForecastException("В словаре сигнатур формы2п не найден параметр: " + curParam, e);
							//давим исключения т.к. сигнатура может быть устарелой и присутствовать только в классификаторе за прошлые годы
							Trace.TraceVerbose("Для сигнатуры {0} не найден id параметра по источнику {1}", curParam, sid);
						}

						if (sdr != null)
						{
							foreach (DataRow dataRow in sdr)
							{
								Int32 curyear = Convert.ToInt32(dataRow["YearOf"]);

								if (curyear > year)
								{
									dataRow.BeginEdit();

									Object value = null;
				/*					if (curyear == year)
										value = row["VALUEESTIMATE"];*/
									if (curyear == year + 1)
										value = row["Y1"];
									if (curyear == year + 2)
										value = row["Y2"];
									if (curyear == year + 3)
										value = row["Y3"];

									dataRow["Value"] = value;
									rowsProcessed++;
									dataRow.EndEdit();
								}
							}
						}
					}
					Trace.TraceVerbose("Обработанно {0} из {1} записей в {2} строках", rowsProcessed, resultAFData.Rows.Count * 3, resultAFData.Rows.Count);
				}
				catch (Exception e)
				{
					throw new ForecastException("Ошибка при обновлении DataRow для dtIndicators: " + e.Message, e);
				}

				du.Update(ref dtParams);
				Trace.TraceVerbose("Данные сохранены");
			}

		}

		#endregion

		private void SaveCalcedForm2p(int id2p, IDatabase db, int baseYear, ExcelModel exMod, int sid)
		{
			//ссылка на детали Вариантов формы 2п (Показатели формы 2п)
			IEntityAssociation item = Data.Associated[a_Form2p_ParamValuesKey];
			IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
			//Параметр с id варианта формы 2п активной строки в мастер гриде 
			parameters[0] = db.CreateParameter(item.FullDBName, id2p);
			//Параметр для выбора прогнозных значений формы 2п
			//parameters[1] = db.CreateParameter("ParamType", 3);

			IDataUpdater du = item.RoleData.GetDataUpdater(String.Format("({0} = ?)", item.FullDBName), null, parameters); /*and (PARAMTYPE = ?)*/

			/// DataTable для детали 
			DataTable dtParams = new DataTable();
			du.Fill(ref dtParams);


			///DataTable для данных получаемых из excel модели
			System.Data.DataTable dt_Form2p;
			try
			{
				IWorkbookOfModel wbForm2p = exMod.ForecastModel.GetWorkBook("Форма 2п-ОКВЭД.xls");
				Worksheet wsForm2p = (Worksheet)wbForm2p.WorkBook.Worksheets.get_Item("Форма_2п");
				dt_Form2p = wbForm2p.GetDataFromSheetF2p(wsForm2p);
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка при получении данных из модели: " + e.Message, e);
			}
			if (dt_Form2p == null)
				throw new ForecastException("Запрос параметров Формы-2п вернул пустую таблицу");

			// Создаем словарь сигнатур для формы 2п на текущий год 
			String queryDictornary = String.Format("select ID, SIGNAT from d_forecast_forma2p where (SOURCEID = {0}) and (CODE <> -1)", sid);
			DataTable dt = (DataTable)db.ExecQuery(queryDictornary, QueryResultTypes.DataTable);
			Dictionary<String, Int32> dic = new Dictionary<String, Int32>();
						
			if (dt != null)
			{
				foreach (DataRow datarow in dt.Rows)
				{
					dic.Add(Convert.ToString(datarow["SIGNAT"]), Convert.ToInt32(datarow["ID"]));
				}
			}

			try
			{
				int rowsProcessed = 0;
				foreach (DataRow row in dt_Form2p.Rows)
				{
					String curParam = String.Empty;
					DataRow[] sdr = null;

					try
					{
						curParam = Convert.ToString(row["SIGNAT"]);
						sdr = (dtParams.Select("REFPARAMETRS = " + dic[curParam]));
					}
					catch (Exception e)
					{
						//throw new ForecastException("В словаре сигнатур формы2п не найден параметр: " + curParam, e);
						//давим исключения т.к. сигнатура может быть устарелой и присутствовать только в классификаторе за прошлые годы
						Trace.TraceVerbose("Для сигнатуры {0} не найден id параметра по источнику {1}", curParam, sid);
					}

					if (sdr != null)
					{
						foreach (DataRow dataRow in sdr)
						{
							Int32 year = Convert.ToInt32(dataRow["YearOf"]);

							if (year > baseYear)
							{
								dataRow.BeginEdit();

								Object value = null;
								if (year == baseYear + 1)
									value = row["VALUEESTIMATE"];
								if (year == baseYear + 2)
									value = row["VALUEY1"];
								if (year == baseYear + 3)
									value = row["VALUEY2"];
								if (year == baseYear + 4)
									value = row["VALUEY3"];

								dataRow["Value"] = value;
								rowsProcessed++;
								dataRow.EndEdit();
							}
						}
					}
				}
				Trace.TraceVerbose("Обработанно {0} из {1} записей в {2} строках", rowsProcessed, dt_Form2p.Rows.Count * 4, dt_Form2p.Rows.Count);
			}
			catch (Exception e)
			{
				throw new ForecastException("Ошибка при обновлении DataRow для dtIndicators: " + e.Message, e);
			}

			du.Update(ref dtParams);
			Trace.TraceVerbose("Данные сохранены");
		}


		private void ModelAdaptation(DataTable valuetable, DataTable resultAFData, Double[,] h2, Double[,] normWork, Boolean[] rowHaveValues, Double[] aver, Int32 paramCount, Int32 predefCount, Int32 yearsFound, Boolean[] predefData, Int32[] nYearsInRow)
		{
			const Double verySmall = 1E-50;
			
			Double[,] h2_cut = new Double[paramCount - predefCount, paramCount - predefCount];

			Int32 i1 = 0;
			for (Int32 i = 0; i < paramCount; i++)
			{
				if (!predefData[i])
				{
					Int32 j1 = 0;

					for (Int32 j = 0; j < paramCount; j++)
					{
						if (!predefData[j])
						{
							h2_cut[i1, j1] = h2[i, j];
							j1++;
						}
					}
					i1++;
				}
			}
			
			Double[] b1_A = new Double[paramCount];
			Double[] b_A = new Double[paramCount - predefCount];
			Double[] r_A = new Double[paramCount - predefCount];
			Double[] x_A = new Double[paramCount - predefCount];
			Double[] w_A = new Double[paramCount - predefCount];
			Double[] z_A = new Double[paramCount - predefCount];

			for (Int32 year = 1; year <= 3; year++)
			{
				i1 = 0;

				for (Int32 i = 0; i < paramCount; i++)
				{
					if (rowHaveValues[i])
					{
						///Заполнение массива b в задаче оптимизации, сводимой к СЛАУ Ax=b
						///Массив есть разность известных нормированных данных и дынных прогнозирования 
						if (predefData[i1]) //здесь valuetable т.к. нам нужны предопределенные данные
							b1_A[i1] = Convert.ToDouble(valuetable.Rows[i][String.Format("y{0}", year)]) / (aver[i] / nYearsInRow[i]) - normWork[i1, yearsFound + year];
						else b1_A[i1] = 0;  
						i1++;
					}
				}
				
				i1 = 0;
				for (Int32 i = 0; i < paramCount; i++)
				{
					if (!predefData[i1])
					{
						b_A[i1] = 0;
						
						///Переход от задачи H2*x=b1_A к задаче h2_cut*x=b_A (учет того, что
						///параметрам, для которых есть Ц данные оптимизация не нужна)
						for (Int32 j = 0; j < paramCount; j++)
							b_A[i1] -= h2[i, j] * b1_A[j];
						i1++;
					}
				}

				for (Int32 i = 0; i < paramCount - predefCount; i++)
				{
					r_A[i] = b_A[i];
					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						r_A[i] -= h2_cut[i,j] * x_A[j];
					}
					w_A[i] = -r_A[i];

					z_A[i] = 0;
					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						z_A[i] += h2_cut[i, j] * w_A[j];
					}
				}

				Double alfa = 0;
				Double alfa1 = 0;
				for (Int32 i = 0; i < paramCount - predefCount; i++)
				{
					alfa += r_A[i] * w_A[i];
					alfa1 += z_A[i] * w_A[i];
				}

				if (Math.Abs(alfa).CompareTo(verySmall) < 0)
					alfa = 0;
				else
					alfa /= alfa1;

				for (Int32 i = 0; i < paramCount - predefCount; i++)
				{
					x_A[i] += alfa * w_A[i]; 
				}

				Double beta = 0;
				Double beta1 = 0;

				for (Int32 count = 1; count <= paramCount - predefCount; count++)
				{
					beta = 0;
					beta1 = 0;
					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						r_A[j] -= alfa * z_A[j];
						beta += r_A[j] * z_A[j];
						beta1 += z_A[j] * w_A[j];
					}

					if (Math.Abs(beta).CompareTo(verySmall) < 0)
						beta = 0;
					else
						beta /= beta1;

					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						w_A[j] = -r_A[j] + beta * w_A[j];
					}

					for (Int32 i = 0; i < paramCount - predefCount; i++)
					{
						z_A[i] = 0;
						for (Int32 j = 0; j < paramCount - predefCount; j++)
						{
							z_A[i] += h2_cut[i, j] * w_A[j];
						}
					}

					alfa = 0;
					alfa1 = 0;

					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						alfa += r_A[j] * w_A[j];
						alfa1 += z_A[j] * w_A[j];
					}

					if (Math.Abs(beta).CompareTo(verySmall) < 0)
						alfa = 0;
					else
						alfa /= alfa1;

					for (Int32 j = 0; j < paramCount - predefCount; j++)
					{
						x_A[j] += alfa * w_A[j];
					}
				} //count

				i1 = 0;
				Int32 j1 = 0;

				for (Int32 i = 0; i < paramCount; i++)
				{
					if (rowHaveValues[i])
					{
						String colName = String.Format("y{0}", year);
						if (!predefData[i1])
						{
							resultAFData.Rows[i][colName] = (x_A[j1] + normWork[i1, yearsFound + year]) * aver[i] / nYearsInRow[i];
						}
						else
						{
							resultAFData.Rows[i][colName] = valuetable.Rows[i][colName];
						}
					}
					i1++;
				}
			}
		}
	}

	class SortedArray: IEnumerable<KeyValuePair<Int32,Double>>
	{
		private ArrayList arrayValue = new ArrayList();
		private ArrayList arrayIndex = new ArrayList();

		public void Add(Int32 index, Double value)
		{
		/*	if (index == 193)
			{
				//int d = 43;
				String s = String.Empty;
				foreach (Double d in arrayValue)
				{
					s += String.Format("{0}, ", d);
				}
			}*/
			if (arrayValue.Count == 0)
			{
				arrayValue.Add(value);
				arrayIndex.Add(index);
			}
			else
			{
				Int32 i = 0;
				foreach (double d in arrayValue)
				{
					if (d.CompareTo(value) < 0)
						i++;
					else break;

				}
				arrayValue.Insert(i, value);
				arrayIndex.Insert(i, index);
			}
		}

		public KeyValuePair<Int32, Double> Get(Int32 index)
		{
			return new KeyValuePair<int, double>(Convert.ToInt32(arrayIndex[index]), Convert.ToDouble(arrayValue[index]));
		}

		public Int32[] IndexToArray()
		{
			Int32[] tmp = new Int32[arrayValue.Count];
			Int32 i = 0;
			foreach (ValueType valueType in arrayIndex)
			{
				tmp[i] = Convert.ToInt32(valueType);
				i++;
			}
			return tmp;
		}

		public void Clear()
		{
			arrayValue.Clear();
			arrayIndex.Clear();
		}

		public IEnumerable<KeyValuePair<Int32, Double>> BottomToTop
		{
			get
			{
				for (Int32 i = 0; i < arrayValue.Count; i++)
					yield return new KeyValuePair<Int32, Double>(Convert.ToInt32(arrayIndex[i]), Convert.ToDouble(arrayValue[i]));
			}
		}

		IEnumerator<KeyValuePair<Int32, Double>> IEnumerable<KeyValuePair<Int32, Double>>.GetEnumerator()
		{
			return BottomToTop.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<int, double>>) this).GetEnumerator();
		}
	}
}
