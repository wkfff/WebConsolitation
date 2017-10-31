using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	public delegate void PrintMes(String s);

	[Guid("21EA085E-1EFE-407e-91E0-120FBCFF4920")]
	[ComVisible(true)]
	public interface IPrintMes
	{
		void PrintMess(String s);
	}
	
	public static class Trace
	{
		public static PrintMes pm;

		private static Boolean debug = false;

		private static String stat = String.Empty;

		public static void toDebugMode()
		{
			debug = true;
		}

		public static string Stat
		{
			get { return stat; }
		}

		public static void TraceMes(String s)
		{
			stat = stat + (s + Environment.NewLine);
			if (pm != null) pm(s);
		}

		public static void TraceMes(String s, params Object[] o)
		{
			s = String.Format(s, o);
			stat = stat + (s + Environment.NewLine);
			if (pm != null) pm(s);
		}

		public static void TraceFactor(String s, Factor f)
		{
			String tmpS = s;
			//tmpS += f.Name + "= " + f.Value.ToString("G17") + "; ";
			tmpS = String.Format("{0} {1}={2}; ", tmpS, f.Name, f.Value.ToString("G17"));
			stat = stat + (tmpS + Environment.NewLine);
			if (pm != null) pm(tmpS);
		}

		public static void TraceFactors(String s, Factors f)
		{
			String tmpS = s;
			foreach (Factor factor in f)
			{
				//tmpS += factor.Name + "= " + factor.Value.ToString("G17") + "; ";
				tmpS = String.Format("{0} {1}={2}; ", tmpS, factor.Name, factor.Value.ToString("G17"));
			}
			stat = stat + (tmpS + Environment.NewLine);
			if (pm != null) pm(tmpS);
		}

		public static void TraceFactors(String s, Factors f, Boolean withRegion)
		{
			String tmpS = s;
			foreach (Factor factor in f)
			{
				tmpS = String.Format("{0} {1} = {2}; ", tmpS, factor.Name, factor.Value);
				if (withRegion) //tmpS += "minVal=" + factor.MinVal + " MaxVal=" + factor.MaxVal + " G_left=" + factor.G_left + " G_right=" + factor.G_right + Environment.NewLine;
					tmpS = String.Format("{0} minVal={1} MaxVal={2} G_left={3} G_right={4}", tmpS, factor.MinVal, factor.MaxVal, factor.G_left, factor.G_right);
			}
			stat = stat + (tmpS + Environment.NewLine);
			if (pm != null) pm(tmpS);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s">Строка выводимая на экран</param>
		/// <param name="d">массив коэфициентов</param>
		/// <param name="n">их количество</param>
		public static void TraceKReg(String s, Decimal[] d, Int32 n)
		{
			String tmpS = s;
			for (Int32 i = 0; i < n; i++)
			{
				//tmpS += "Коэфф. регр. b" + (i + 1).ToString() + "=" + d[i].ToString() + " ";
				tmpS = String.Format("{0} Коэфф. регр. b[{1}]={2} ", tmpS, i, d[i]);
			}
			stat = stat + (tmpS + Environment.NewLine);
			if (pm != null) pm(tmpS);
		}

		public static void DebugTraceMes(String s)
		{
			if (debug)
			{
				stat = stat + (s + Environment.NewLine);
				if (pm != null) pm(s);
			}
		}

		public static void DebugTraceMes(String s, params Object[] o)
		{
			if (debug)
			{
				s = String.Format(s, o);
				stat = stat + (s + Environment.NewLine);
				if (pm != null) pm(s);
			}
		}

		public static void DebugTraceFactor(String s, Factor f)
		{
			if (debug)
			{
				String tmpS = s;
				//tmpS += f.Name + "= " + f.Value.ToString("G17") + "; ";
				tmpS = String.Format("{0} {1}={2}; ", tmpS, f.Name, f.Value.ToString("G17"));
				stat = stat + (tmpS + Environment.NewLine);
				if (pm != null) pm(tmpS);
			}
		}

		public static void DebugTraceFactors(String s, Factors f)
		{
			if (debug)
			{
				String tmpS = s;
				foreach (Factor factor in f)
				{
					//tmpS += factor.Name + "= " + factor.Value.ToString("G17") + "; ";
					tmpS = String.Format("{0} {1}={2}; ", tmpS, factor.Name, factor.Value.ToString("G17"));
				}
				stat = stat + (tmpS + Environment.NewLine);
				if (pm != null) pm(tmpS);
			}
		}

		public static void DebugTraceFactors(String s, Factors f, Boolean withRegion)
		{
			if (debug)
			{
				String tmpS = s;
				foreach (Factor factor in f)
				{
					tmpS = String.Format("{0} {1} = {2}; ", tmpS, factor.Name, factor.Value);
					if (withRegion) //tmpS += "minVal=" + factor.MinVal + " MaxVal=" + factor.MaxVal + " G_left=" + factor.G_left + " G_right=" + factor.G_right + Environment.NewLine;
						tmpS = String.Format("{0} minVal={1} MaxVal={2} G_left={3} G_right={4}", tmpS, factor.MinVal, factor.MaxVal, factor.G_left, factor.G_right);
				}
				stat = stat + (tmpS + Environment.NewLine);
				if (pm != null) pm(tmpS);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s">Строка выводимая на экран</param>
		/// <param name="d">массив коэфициентов</param>
		/// <param name="n">их количество</param>
		public static void DebugTraceKReg(String s, Decimal[] d, Int32 n)
		{
			if (debug)
			{
				String tmpS = s;
				for (Int32 i = 0; i < n; i++)
				{
					//tmpS += "Коэфф. регр. b" + (i + 1).ToString() + "=" + d[i].ToString() + " ";
					tmpS = String.Format("{0} Коэфф. регр. b[{1}]={2} ", tmpS, i, d[i]);
				}
				stat = stat + (tmpS + Environment.NewLine);
				if (pm != null) pm(tmpS);
			}
		}
	}
}
