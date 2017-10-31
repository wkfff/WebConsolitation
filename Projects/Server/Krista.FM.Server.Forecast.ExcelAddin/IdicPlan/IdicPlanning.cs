using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// ��������� ������������ ������������. 
	/// </summary>
	[Guid("847B03B5-D6E8-4767-85AD-15CDDF47C532")]
	[ComVisible(true)]
	public interface IIdicPlanning : IDisposable
	{
		/// <summary>
		/// �������� ��������������� ������������� �������� ������ �����������
		/// ��� ���������� ��������� ������������
		/// </summary>
		bool CheckBounds { get; set; }
		
		/// <summary>
		/// ��������� �� ������ ����������� � ���������� ����������
		/// </summary>
		IAdjusters AdjStart { get; }

		/// <summary>
		/// �������� ���-���������� � ���� ���������� ������
		/// </summary>
		/*String Stat
		{
			get;
		}*/

		/// <summary>
		/// ������ ������ ��� ����������� �����������. ��������! ������ ��������
		/// ����������� � ����������� ������. �� ������������ �������.
		/// </summary>
		/// <param name="adj">������ �����������</param>
		/// <returns>�������� �������</returns>
		Decimal RespQ(Adjusters adj);

		/// <summary>
		/// ������� ������������
		/// </summary>
		/// <param name="adj">�������� �� ������ �����������</param>
		/// <param name="N">���������� ����� ��������� ������� (max-min)/10</param>
		/// <param name="eps">�������� �����</param>
		/// <param name="midUse">������������ �������� ����������� ����������� � ������� ������ adj</param>
		/// <returns>�������� �� ������ � ������������ ������������ (����������� ����������)</returns>
		IAdjusters MakePlanning(IAdjusters adj, Int32 N, Decimal eps, Boolean midUse);

		/// <summary>
		/// ������� ������������ c �������������� ����������� �� AdjStart
		/// </summary>
		/// <param name="N">���������� ����� ��������� ������� (max-min)/10</param>
		/// <param name="eps">�������� �����</param>
		/// <param name="midUse">������������ �������� ����������� ����������� � ������� ������ adj</param>
		/// <returns>�������� �� ������ � ������������ ������������ (����������� ����������)</returns>
		IAdjusters MakePlanning(Int32 N, Decimal eps, Boolean midUse);
	}

		/// <summary>
		/// ����� ������������ ������������
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
			/// �����������
			/// </summary>
			/// <param name="mod">������ �������������� ������</param>
			/// <param name="PM">������ �������</param>
			public IdicPlanning(MathModel mod/*, PrintMes PM*/)
			{
				sm = mod;
			//	pm = PM;
				adjStart = new Adjusters();

			/*	if (pm != null)
					pm("������������� ������������� ������������");*/
				Trace.TraceMes("������������� ������������� ������������");
			}

			#region ��������
			/// <summary>
			/// ����������� �������� ������
			/// </summary>
			public bool CheckBounds
			{
				get { return checkBounds; }
				set { checkBounds = value; }
			}


			/// <summary>
			/// ������ ������ �������
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
			/// ����������� �������� ������������� Q
			/// </summary>
			/// <param name="adj">������ �����������</param>
			/// <returns>�������� Q</returns>
			public Decimal RespQ(Adjusters adj)
			{
				Decimal Q = 0;
				//Trace.TraceMes("�������� ����������� � ������");
				sm.SendAdj(adj);

				//Trace.TraceMes("������������");
				IWorkbookOfModel wbBalance = sm.Mod.GetWorkBook("���������.xls");
				do
				{
					sm.Mod.CallMacros("���������.xls!Module1.BalanceModel");
					sm.Mod.RecalcAll();
				}
				while (Decimal.Compare(Convert.ToDecimal(wbBalance.GetDataFromCell("���������", "disbalance")), (Decimal)0.0001) == 1) ;

				foreach (Factor ind in sm.Ind)
				{
					//Trace.TraceFactor("������� ����������",ind);
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
			/// ��������� ������� �������� ���������� �� min � max ��������
			/// </summary>
			/// <param name="curAdj">������ �����������</param>
			private static void FirstInit(Adjusters curAdj)
			{
				for (Int32 i = 0; i < curAdj.Count; i++)
					curAdj[i].Value = (curAdj[i].MaxVal + curAdj[i].MinVal) / 2;
			}

			/// <summary>
			/// ��������� ���� ��������� ����� �������� �������.
			/// </summary>
			/// <param name="curAdj">������ �����������</param>
			/// <param name="N">���������� �������� ���������</param>
			/// <param name="h">���</param>
			/// <param name="sB">����� �������</param>
			/// <returns>������ ������������� �����������</returns>
			private Decimal[] FindNextPoint(Adjusters curAdj, Int32 N, out Decimal h, out Decimal sB)
			{
				Trace.DebugTraceMes("����� � ��������� ������ ��������� �����");
				Adjusters matrixOfAdj = Adjusters.Copy(curAdj);

				Decimal[] dU = new Decimal[matrixOfAdj.Count];
				Int32[] overflow = new Int32[matrixOfAdj.Count];

				Trace.DebugTraceMes("����� ������");
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
				Trace.DebugTraceMes("����� �� �����");
				Trace.TraceMes("���� ������ ��� matrixOfAdj");
				Decimal Q0 = RespQ(matrixOfAdj);

				Int32 dimOfMatrix = 1 << matrixOfAdj.Count;
				Decimal[] b = new Decimal[matrixOfAdj.Count];

				Trace.DebugTraceMes("����� ������ ������� b[k], ����� � ������� ������������ {0}", dimOfMatrix);
				for (Int32 i = 0; i < dimOfMatrix; i++)
				{
					Adjusters tmpAdj = Adjusters.Copy(matrixOfAdj);

					Trace.TraceMes("���������� ������ Ux,m  m={0}",i);
					for (Int32 j = 0; j < tmpAdj.Count; j++)
					{
						Int32 curIndex = 1 << j;
						if ((i & curIndex) == 0)
							tmpAdj[j].Value = tmpAdj[j].MinVal;
						else
							tmpAdj[j].Value = tmpAdj[j].MaxVal;
						Trace.DebugTraceMes("tmpAdj[{0}].Value={1}", j ,tmpAdj[j].Value);
					}
					Trace.TraceMes("���� ������ ��� tmpAdj ������ i={0}",i);
					Decimal Q = RespQ(tmpAdj) - Q0;
					Trace.TraceMes("���� ������ b[k] ��� i={0}",i);
					for (Int32 k = 0; k < matrixOfAdj.Count; k++)
					{
						if (i == 0)
						{
							Trace.DebugTraceMes("������ ������ �������. �� ������� �������� Q");
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

				Trace.TraceMes("������� sumB");
				Decimal sumB = 0;
				for (Int32 k = 0; k < matrixOfAdj.Count; k++)
				{
					b[k] = b[k] / (dimOfMatrix * dU[k]);
					Trace.DebugTraceMes("��������� b[{0}]={1}", k, b[k]);
					
//					sumB += b[k] * b[k];//// � ����� ����� ���������??????
					
					if (overflow[k] != 0)
						if (Math.Sign(b[k]) * (-1) == overflow[k])
						{
							Trace.TraceMes("��������� ����� b[k]={0}",b[k]);
							b[k] = 0;
						}
					Trace.DebugTraceMes("sumB = {0}",sumB);
					sumB += b[k] * b[k];//// � ����� ����� ���������??????
					Trace.DebugTraceMes("sumB = {0}", sumB);
				}
				sumB = (Decimal)Math.Sqrt((Double)sumB);
				Trace.DebugTraceMes("sumB = {0}", sumB);
				sB = sumB;

				Boolean assigned = false;
				Decimal minDu = 0;

				Trace.TraceMes("������� h. ���� dU[k] � ���������� �������� �� �� ������ 0");
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
						Trace.DebugTraceMes("��������� b[{0}]={1}", k, b[k]);
						b[k] = b[k] / sumB;//minBk; //abs?
					}
				}
				else h = 0;

				return b;
			}

			/// <summary>
			/// ��������� ������ ����������� (������������)
			/// </summary>
			/// <param name="adj">������ �����������</param>
			/// <param name="N">���������� �������� ���������</param>
			/// <param name="eps">���������� ���������� ����� �������</param>
			/// <param name="midUse">��������� ���������� ������� ��������� �����������</param>
			/// <returns>�������� ���������� �����</returns>
			public IAdjusters MakePlanning(IAdjusters adj, Int32 N, Decimal eps, Boolean midUse)
			{
				Trace.TraceMes("���� � ��������� ������������");

				Adjusters curPoint = AdjStart.Copy(adj);

				if (midUse)
				{
					Trace.TraceMes("���������� �������� �������� �����������");
					FirstInit(curPoint);
				}
				Trace.TraceFactors("������� �����", curPoint);

				Adjusters savePoint = Adjusters.Copy(curPoint);
				Decimal saveQ;
				Decimal curQ;

				Trace.DebugTraceMes("��� �� � ���������� save point");

				Decimal Q = RespQ(curPoint);
				saveQ = Q;
				curQ = Q;
				
				Trace.TraceMes("������ Q={0}", Q);

				Trace.DebugTraceMes("���� � ����");

				Boolean exitFrom = false;
				do
				{
					Decimal h;

					Trace.TraceMes("����������� ������� ����������� ������");
					Decimal sumB;
					Decimal[] b = FindNextPoint(curPoint, N, out h, out sumB);
					Trace.TraceKReg("�����. ����. b", b, curPoint.Count);

					Trace.TraceMes("����� ������� ��������� ||B||={0}", sumB);

					if (sumB < eps)
					{
						Trace.TraceMes("�������� ����� ��������� ������ ��������... ������� ������ ������ ������������!");
						exitFrom = true;
					}

					if (Q == 0)
					{
						Trace.TraceMes("Q=0... ������� �������!");
						exitFrom = true;
					}


					if (!exitFrom)
					{
						Boolean nextPoint = false;
						Decimal s = 1; // sumB;
						Int32 c = 0;

						Trace.DebugTraceMes("�������� ����� ������ � Q != 0. ���� ������ ����� �����");

						do
						{
							Adjusters testPoint = Adjusters.Copy(curPoint);
							Trace.DebugTraceFactors("�������������� �����: ", testPoint);

							Trace.TraceMes("��� (h*s): {0}", (h * s));
							for (Int32 i = 0; i < testPoint.Count; i++)
							{
								Trace.DebugTraceMes("b[i]={0}, h={1}, s={2}", b[i], h, s);
								testPoint[i].Value -= b[i] * h * s;
							}

							Trace.DebugTraceFactors("�������������� ����� � ������ ��������: ", testPoint);

							if (CheckBounds) testPoint.CorrectBounds(out nextPoint);
							Trace.TraceFactors("����� �����: ", testPoint);
							Decimal newQ = RespQ(testPoint);
							Trace.TraceMes("�������� ������� � ����� �����: {0}", newQ);

							if (c == 0)
							{
								if ((newQ <= Q))
								{
									Q = newQ;
									curQ = Q;
									curPoint = testPoint;
									c++;
									Trace.TraceMes("�������� ������ ����������� - ������ ����� ����� ������� ����� newQ={0}", Q);
									
								}
								else
								{
									s /= 2;
									Trace.TraceMes("��������� ��������� ���?");
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
									Trace.TraceMes("�������� ������ ����������� - ������ ����� ����� ������� ����� newQ={0}", Q);
								}
								else
								{
									s /= 2;
									Trace.TraceMes("��������� ��������� ���?");
									//needRefresh = true;
								}
							}
							if (s < (eps / 10))
							{
								Trace.TraceMes("��� ������ ������ %( �������");
								nextPoint = true;
							}
							if (Q == 0)
							{
								Trace.TraceMes("Q=0, ������� �� ����� ������ ��������� �����");
								nextPoint = true;
							}
						}
						while (!nextPoint);

						Trace.TraceMes("��������� ����� �� ������� �����������...");
						if (curPoint.BoundsIsOk())
						{
							Trace.TraceMes("��� ��������! ��������� ������� ����� �� ����������...");
							savePoint = curPoint;
							saveQ = curQ;
						}
						else
						{
							Trace.TraceFactors("���-�� ����� �� ��� ", curPoint);
							exitFrom = true;
						}
					}
				}
				while (!exitFrom);
				Trace.TraceMes("����� �� ��������� ������������");
				Trace.TraceFactors("������� �����", savePoint);
				///Q = RespQ(savePoint);
				Trace.TraceMes("������ Q= {0}", saveQ);
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