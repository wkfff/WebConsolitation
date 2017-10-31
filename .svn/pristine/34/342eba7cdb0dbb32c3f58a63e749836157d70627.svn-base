using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// Интрефейс идикативного планирования. 
	/// </summary>
	[Guid("847B03B5-D6E8-4767-85AD-15CDDF47C532")]
	[ComVisible(true)]
	public interface IIdicPlanning : IDisposable
	{
		/// <summary>
		/// Свойство устанавливающее необходимость проверки границ регуляторов
		/// при проведении процедуры планирования
		/// </summary>
		bool CheckBounds { get; set; }
		
		/// <summary>
		/// Интерфейс на список регуляторов с начальными значениями
		/// </summary>
		IAdjusters AdjStart { get; }

		/// <summary>
		/// Получает лог-статистики в виде ткекстовой строки
		/// </summary>
		/*String Stat
		{
			get;
		}*/

		/// <summary>
		/// Отклик модели при подстановке регуляторов. Внимание! меняет занчение
		/// регуляторов в экселевской модели. Не поддерживает откатов.
		/// </summary>
		/// <param name="adj">Список регуляторов</param>
		/// <returns>Значение отклика</returns>
		Decimal RespQ(Adjusters adj);

		/// <summary>
		/// Функция планирования
		/// </summary>
		/// <param name="adj">Интерфес на список регуляторов</param>
		/// <param name="N">Количестов точек разбиения отрезка (max-min)/10</param>
		/// <param name="eps">значение нормы</param>
		/// <param name="midUse">Использовать значения регуляторов приведенных к средним вместо adj</param>
		/// <returns>Интрефес на вектор с результатами планирования (подобранные регуляторы)</returns>
		IAdjusters MakePlanning(IAdjusters adj, Int32 N, Decimal eps, Boolean midUse);

		/// <summary>
		/// Функция планирования c использованием регуляторов из AdjStart
		/// </summary>
		/// <param name="N">Количестов точек разбиения отрезка (max-min)/10</param>
		/// <param name="eps">значение нормы</param>
		/// <param name="midUse">Использовать значения регуляторов приведенных к средним вместо adj</param>
		/// <returns>Интрефес на вектор с результатами планирования (подобранные регуляторы)</returns>
		IAdjusters MakePlanning(Int32 N, Decimal eps, Boolean midUse);
	}

		/// <summary>
		/// Класс идикативного планирования
		/// </summary>
		[Guid("ADC9FB31-8989-479b-9A5F-63DBCB0A2C16")]
		[ComVisible(true)]
		public class IdicPlanning : IIdicPlanning
		{
			private MathModel sm;
			private Boolean checkBounds;
			//private Boolean correctStep;
			//static public Boolean needRefresh = false;

			/// <summary>
			/// Конструктор
			/// </summary>
			/// <param name="mod">Объект математической модели</param>
			/// <param name="PM">Объект Логгера</param>
			public IdicPlanning(MathModel mod/*, PrintMes PM*/)
			{
				sm = mod;
			//	pm = PM;
				adjStart = new Adjusters();

			/*	if (pm != null)
					pm("Инициализация индикативного планирования");*/
				Trace.TraceMes("Инициализация индикативного планирования");
			}

			#region Свойства
			/// <summary>
			/// Производить проверку границ
			/// </summary>
			public bool CheckBounds
			{
				get { return checkBounds; }
				set { checkBounds = value; }
			}


			/// <summary>
			/// Задает объект логгера
			/// </summary>
			/*public PrintMes Pm
			{
				set { pm = value; }
			}*/

			public IAdjusters AdjStart
			{
				get { return adjStart; }
				//set { adjStart = value; }
			}

			/*public String Stat
			{
				get { return stat; }
			}*/
			#endregion


			/// <summary>
			/// Расчитывает Критерий эффективности Q
			/// </summary>
			/// <param name="adj">Список регуляторов</param>
			/// <returns>Значение Q</returns>
			public Decimal RespQ(Adjusters adj)
			{
				Decimal Q = 0;
				//Trace.TraceMes("Передача регуляторов в модель");
				sm.SendAdj(adj);

				//Trace.TraceMes("Балансировка");
				IWorkbookOfModel wbBalance = sm.Mod.GetWorkBook("Настройка.xls");
				do
				{
					sm.Mod.CallMacros("Настройка.xls!Module1.BalanceModel");
					sm.Mod.RecalcAll();
				}
				while (Decimal.Compare(Convert.ToDecimal(wbBalance.GetDataFromCell("Настройка", "disbalance")), (Decimal)0.0001) == 1) ;

				foreach (Factor ind in sm.Ind)
				{
					//Trace.TraceFactor("Считаем индикаторы",ind);
					Decimal z = sm.CalcInd(ind);
					Decimal q = 0;
					Decimal m = 0;

					if (Decimal.Compare(z, ind.MinVal) == -1)
					{
							m = (ind.MaxVal + ind.MinVal) / 2;
							//q = Math.Pow(ind.MinVal - z, 1) * ind.G_left / m;
							q = (ind.MinVal - z) * ind.G_left / m;
							Q += q;
					}
					else
						if (Decimal.Compare(z, ind.MaxVal) == 1)
						{
							m = (ind.MaxVal + ind.MinVal) / 2;
							//q = Math.Pow(z - ind.MaxVal, 1) * ind.G_right / m;
							q = (z - ind.MaxVal) * ind.G_right / m;
							Q += q;
						}
					Trace.TraceMes("q={0}, m={1}, z={2}, min={3}, max={4}", q, m, z, ind.MinVal, ind.MaxVal);
				}
				return Q;
			}

			/// <summary>
			/// Вычисляет среднне занчение регулятора по min и max значению
			/// </summary>
			/// <param name="curAdj">Список регуляторов</param>
			private static void FirstInit(Adjusters curAdj)
			{
				for (Int32 i = 0; i < curAdj.Count; i++)
					curAdj[i].Value = (curAdj[i].MaxVal + curAdj[i].MinVal) / 2;
			}

			/// <summary>
			/// Процедура ищет следующую точку минимума отклика.
			/// </summary>
			/// <param name="curAdj">Список регуляторов</param>
			/// <param name="N">Количество областей разбиения</param>
			/// <param name="h">Шаг</param>
			/// <param name="sB">НОрма вектора</param>
			/// <returns>Вектор нормированный направления</returns>
			private Decimal[] FindNextPoint(Adjusters curAdj, Int32 N, out Decimal h, out Decimal sB)
			{
				Trace.DebugTraceMes("Зашли в процедуру поиска следующей точки");
				Adjusters matrixOfAdj = Adjusters.Copy(curAdj);

				Decimal[] dU = new Decimal[matrixOfAdj.Count];
				Int32[] overflow = new Int32[matrixOfAdj.Count];

				Trace.DebugTraceMes("Перед циклом");
				for (Int32 i = 0; i < matrixOfAdj.Count; i++)
				{
					overflow[i] = 0;
					dU[i] = (matrixOfAdj[i].MaxVal - matrixOfAdj[i].MinVal) / N;
					
					Trace.TraceMes("du[i]={0}",dU[i]);
					Trace.TraceMes("max={0} min={1} curmax={2} curmin={3}", matrixOfAdj[i].MaxVal, matrixOfAdj[i].MinVal, matrixOfAdj[i].Value + dU[i], matrixOfAdj[i].Value - dU[i]);

					if (Decimal.Compare(matrixOfAdj[i].Value - dU[i], matrixOfAdj[i].MinVal) == -1)
					{
						overflow[i] = -1;
						Trace.TraceMes("-1 min={0} cur={1}", matrixOfAdj[i].MinVal, matrixOfAdj[i].Value - dU[i]);
					}
					matrixOfAdj[i].MinVal = matrixOfAdj[i].Value - dU[i];  //may be here a need use bounds check function/

					if (Decimal.Compare(matrixOfAdj[i].Value + dU[i], matrixOfAdj[i].MaxVal) == 1)
					{
						overflow[i] = +1;
						Trace.TraceMes("+1 max={0} cur={1}", matrixOfAdj[i].MaxVal, matrixOfAdj[i].Value + dU[i]);
					}
					matrixOfAdj[i].MaxVal = matrixOfAdj[i].Value + dU[i];
				}
				Trace.DebugTraceMes("Вышли из цикла");
				Trace.TraceMes("Ищем отклик для matrixOfAdj");
				Decimal Q0 = RespQ(matrixOfAdj);

				Int32 dimOfMatrix = 1 << matrixOfAdj.Count;
				Decimal[] b = new Decimal[matrixOfAdj.Count];

				Trace.DebugTraceMes("Перед циклом расчета b[k], строк в матрице планирования {0}", dimOfMatrix);
				for (Int32 i = 0; i < dimOfMatrix; i++)
				{
					Adjusters tmpAdj = Adjusters.Copy(matrixOfAdj);

					Trace.TraceMes("Составляем строку Ux,m  m={0}",i);
					for (Int32 j = 0; j < tmpAdj.Count; j++)
					{
						Int32 curIndex = 1 << j;
						if ((i & curIndex) == 0)
							tmpAdj[j].Value = tmpAdj[j].MinVal;
						else
							tmpAdj[j].Value = tmpAdj[j].MaxVal;
						Trace.DebugTraceMes("tmpAdj[{0}].Value={1}", j ,tmpAdj[j].Value);
					}
					Trace.TraceMes("Ищем отклик для tmpAdj строка i={0}",i);
					Decimal Q = RespQ(tmpAdj) - Q0;
					Trace.TraceMes("Ищем вектор b[k] для i={0}",i);
					for (Int32 k = 0; k < matrixOfAdj.Count; k++)
					{
						if (i == 0)
						{
							Trace.DebugTraceMes("Первая строка матрицы. До расчета обнуляем Q");
							b[k] = 0;
						}
						Int32 curIndex = 1 << k;
						if ((i & curIndex) == 0)
							b[k] -= Q;
						else
							b[k] += Q;
						Trace.DebugTraceMes("b[{0}]={1}", k,b[k]);
					}
				}

				Trace.TraceMes("Считаем sumB");
				Decimal sumB = 0;
				for (Int32 k = 0; k < matrixOfAdj.Count; k++)
				{
					b[k] = b[k] / (dimOfMatrix * dU[k]);
					Trace.DebugTraceMes("нормируем b[{0}]={1}", k, b[k]);
					
//					sumB += b[k] * b[k];//// а может после обнуления??????
					
					if (overflow[k] != 0)
						if (Math.Sign(b[k]) * (-1) == overflow[k])
						{
							Trace.TraceMes("Обнуление нормы b[k]={0}",b[k]);
							b[k] = 0;
						}
					Trace.DebugTraceMes("sumB = {0}",sumB);
					sumB += b[k] * b[k];//// а может после обнуления??????
					Trace.DebugTraceMes("sumB = {0}", sumB);
				}
				sumB = (Decimal)Math.Sqrt((Double)sumB);
				Trace.DebugTraceMes("sumB = {0}", sumB);
				sB = sumB;

				Boolean assigned = false;
				Decimal minDu = 0;

				Trace.TraceMes("Считаем h. Ищем dU[k] с наименьшим значение но не равным 0");
				for (Int32 k = 0; k < matrixOfAdj.Count; k++)
				{
					Trace.DebugTraceMes("k = {0}", k);
					if (!assigned)
					{
						if (b[k] != 0)
						{
							minDu = dU[k];
							assigned = true;
							Trace.DebugTraceMes("b[k]!=0 minDu = {0} assigned", dU[k]);
						}
					}
					else
						if (dU[k] < minDu)
						{
							minDu = dU[k];
							Trace.DebugTraceMes("dU[k] < minDu minDu = {0}", dU[k]);
						}
				}

				if (assigned)
				{
					h = minDu;// * minBk;
					for (Int32 k = 0; k < matrixOfAdj.Count; k++)
					{
						Trace.DebugTraceMes("нормируем b[{0}]={1}", k, b[k]);
						b[k] = b[k] / sumB;//minBk; //abs?
					}
				}
				else h = 0;

				return b;
			}

			/// <summary>
			/// Выполняет подбор регуляторов (планирование)
			/// </summary>
			/// <param name="adj">Список регуляторов</param>
			/// <param name="N">Количество областей разбиения</param>
			/// <param name="eps">Минимально допустимая норма вектора</param>
			/// <param name="midUse">Проводить усреднение входных згначений регуляторов</param>
			/// <returns>Значение безопасной точки</returns>
			public IAdjusters MakePlanning(IAdjusters adj, Int32 N, Decimal eps, Boolean midUse)
			{
				Trace.TraceMes("Вход в процедуру планирования");

				Adjusters curPoint = AdjStart.Copy(adj);

				if (midUse)
				{
					Trace.TraceMes("Вычисление среднего значения регуляторов");
					FirstInit(curPoint);
				}
				Trace.TraceFactors("Текущая точка", curPoint);

				Adjusters savePoint = Adjusters.Copy(curPoint);
				Decimal saveQ;
				Decimal curQ;

				Trace.DebugTraceMes("Она же и безопасная save point");

				Decimal Q = RespQ(curPoint);
				saveQ = Q;
				curQ = Q;
				
				Trace.TraceMes("Отклик Q={0}", Q);

				Trace.DebugTraceMes("Вход в цикл");

				Boolean exitFrom = false;
				do
				{
					Decimal h;

					Trace.TraceMes("Определение вектора направления поиска");
					Decimal sumB;
					Decimal[] b = FindNextPoint(curPoint, N, out h, out sumB);
					Trace.TraceKReg("Коэфф. регр. b", b, curPoint.Count);

					Trace.TraceMes("Норма вектора градиента ||B||={0}", sumB);

					if (sumB < eps)
					{
						Trace.TraceMes("Значение нормы оказалось меньше заданной... решение дальше искать бессмысленно!");
						exitFrom = true;
					}

					if (Q == 0)
					{
						Trace.TraceMes("Q=0... решение найдено!");
						exitFrom = true;
					}


					if (!exitFrom)
					{
						Boolean nextPoint = false;
						Decimal s = 1; // sumB;
						Int32 c = 0;

						Trace.DebugTraceMes("Значение нормы велико и Q != 0. Цикл поиска новой точки");

						do
						{
							Adjusters testPoint = Adjusters.Copy(curPoint);
							Trace.DebugTraceFactors("Предполагаемая точка: ", testPoint);

							Trace.TraceMes("Шаг (h*s): {0}", (h * s));
							for (Int32 i = 0; i < testPoint.Count; i++)
							{
								Trace.DebugTraceMes("b[i]={0}, h={1}, s={2}", b[i], h, s);
								testPoint[i].Value -= b[i] * h * s;
							}

							Trace.DebugTraceFactors("Предполагаемая точка с учетом смещения: ", testPoint);

							if (CheckBounds) testPoint.CorrectBounds(out nextPoint);
							Trace.TraceFactors("Новая точка: ", testPoint);
							Decimal newQ = RespQ(testPoint);
							Trace.TraceMes("Значение отклика в новой точке: {0}", newQ);

							if (c == 0)
							{
								if ((newQ <= Q))
								{
									Q = newQ;
									curQ = Q;
									curPoint = testPoint;
									c++;
									Trace.TraceMes("Значение меньше предыдущего - значит пусть будет текущая точка newQ={0}", Q);
									
								}
								else
								{
									s /= 2;
									Trace.TraceMes("Попробуем уменьшить шаг?");
									//needRefresh = true;
								}
							}
							else
							{
								if ((newQ < Q))
								{
									Q = newQ;
									curQ = Q;
									curPoint = testPoint;
									Trace.TraceMes("Значение меньше предыдущего - значит пусть будет текущая точка newQ={0}", Q);
								}
								else
								{
									s /= 2;
									Trace.TraceMes("Попробуем уменьшить шаг?");
									//needRefresh = true;
								}
							}
							if (s < (eps / 10))
							{
								Trace.TraceMes("Шаг меньше некуда %( выходим");
								nextPoint = true;
							}
							if (Q == 0)
							{
								Trace.TraceMes("Q=0, выходим из цикла поиска следующей точки");
								nextPoint = true;
							}
						}
						while (!nextPoint);

						Trace.TraceMes("Проверяем выход за границы регуляторов...");
						if (curPoint.BoundsIsOk())
						{
							Trace.TraceMes("Все впорядке! Принимаем текущую точку за безопасную...");
							savePoint = curPoint;
							saveQ = curQ;
						}
						else
						{
							Trace.TraceFactors("Что-то пошло не так ", curPoint);
							exitFrom = true;
						}
					}
				}
				while (!exitFrom);
				Trace.TraceMes("Выход из процедуры планирования");
				Trace.TraceFactors("Текущая точка", savePoint);
				///Q = RespQ(savePoint);
				Trace.TraceMes("Отклик Q= {0}", saveQ);
				return savePoint;
			}

			private Adjusters adjStart;
			
			public IAdjusters MakePlanning(Int32 N, Decimal eps, Boolean midUse)
			{
				return MakePlanning(adjStart, N, eps, midUse);
			}

			public void Dispose()
			{
				try
				{
					if (Marshal.IsComObject(sm))
						Marshal.ReleaseComObject(sm);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(adjStart))
						Marshal.ReleaseComObject(adjStart);
				}
				catch (Exception e) { }
				
				sm = null;
				adjStart = null;

				GC.Collect();
			}
		}
	
}