using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	
	/// <summary>
	/// Интерфейс индикаторов. Наследуется от IFactors. Содержит метод для клонирования
	/// объекта Adjusters по объекту поддерживающему интерфейс IAdjusters.
	/// </summary>
	[Guid("C0BD851B-35C7-43ee-9138-1310CC39728D")]
	[ComVisible(true)]
	public interface IAdjusters : IEnumerable<Factor>, IFactors
	{
		/// <summary>
		/// Клонирует объект поддерживающий интерфейс IAdjusters
		/// </summary>
		/// <param name="fromAdj">объект поддерживающий интерфейс IAdjusters</param>
		/// <returns>клон объекта</returns>
		Adjusters Copy(IAdjusters fromAdj);
	}

	[Guid("35026861-0A63-4bec-AEE4-4DFCBB4A015E")]
	[ComVisible(true)]
	public class Adjusters : Factors, IAdjusters
	{
		/// <summary>
		/// Проверяет границы регуляторов
		/// </summary>
		/// <returns></returns>
		internal Boolean BoundsIsOk()
		{
			Trace.DebugTraceMes("Процедура проверки границ");
			foreach (Factor item in Items)
			{
				if ((item.Value > item.MaxVal) || (item.Value < item.MinVal))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Проверяет содержит ли объект Adjusters все Factor содрежащиеся в this.
		/// </summary>
		/// <param name="curadj">Проверяемый объект</param>
		/// <returns>True если содержит</returns>
		internal bool checkAllNeeded(Adjusters curadj)
		{
			Boolean allIn = true;
			foreach (Factor item in Items)
				allIn &= (curadj.ContainsByName(item));

			return allIn;
		}

		/// <summary>
		/// Клонирует объект Adjusters
		/// </summary>
		/// <param name="fromAdj">объект Adjusters</param>
		/// <returns>клон объекта Adjusters</returns>
		internal static Adjusters Copy(Adjusters fromAdj)
		{
			Adjusters newadj = new Adjusters();
			foreach (Factor adj in fromAdj)
				newadj.Add(adj);
			return newadj;
		}

		/// <summary>
		/// Метод реализует интерфейсный метод Copy. Клонирует объект 
		/// поддерживающий интерфейс IAdjusters
		/// </summary>
		/// <param name="fromAdj">объект поддерживающий интерфейс IAdjusters</param>
		/// <returns>клон объекта</returns>
		public Adjusters Copy(IAdjusters fromAdj)
		{
			Adjusters newadj = new Adjusters();
			foreach (Factor adj in fromAdj)
				newadj.Add(adj);
			return newadj;
		}

		/// <summary>
		/// Корректирует значение регуляторов при выход за границы
		/// </summary>
		internal void CorrectBounds()
		{
			foreach (Factor item in Items)
			{
				if (item.Value > item.MaxVal)
					item.Value = item.MaxVal;

				if (item.Value < item.MinVal)
					item.Value = item.MinVal;
			}
		}

		/// <summary>
		/// Корректирует значение регуляторов при выход за границы
		/// </summary>
		/// <param name="boundsOut">True если что-то было скорректированно</param>
		internal void CorrectBounds(out Boolean boundsOut)
		{
			Trace.DebugTraceMes("Процедура корректировки ");
			Boolean tmpChk = false;
			foreach (Factor item in Items)
			{
				if (item.Value > item.MaxVal)
				{
					item.Value = item.MaxVal;
					tmpChk = true;
					Trace.DebugTraceMes("Скорректировали превышение maxvalue");
				}

				if (item.Value < item.MinVal)
				{
					item.Value = item.MinVal;
					tmpChk = true;
					Trace.DebugTraceMes("Скорректировали превышение minvalue");
				}
			}
			boundsOut = tmpChk;
		}
	}
}