using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// Интерфейс IFactors определяет методы добавления и доступа к членам
	/// списка факторов Factor. Наследуется в IIndicators, IAdjusters
	/// </summary>
	[Guid("10E6FF25-F48E-4196-A8FF-0020231F02B4")]
	[ComVisible(true)]
	public interface IFactors
	{
		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="desc">Описание</param>
		/// <param name="value">Значение</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, String desc, Decimal value);
		
		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="value">Значение</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, Decimal value);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="value">Значение</param>
		/// <param name="minVal">минимальное значение</param>
		/// <param name="maxVal">максимальное значение</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="value">Значение</param>
		/// <param name="minVal">минимальное значение</param>
		/// <param name="maxVal">максимальное значение</param>
		/// <param name="g_l">граница слева</param>
		/// <param name="g_r">граница справа</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="value">Значение</param>
		/// <param name="minVal">минимальное значение</param>
		/// <param name="maxVal">максимальное значение</param>
		/// <param name="g_l">граница слева</param>
		/// <param name="g_r">граница справа</param>
		/// <param name="yearName">имя года</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r, String yearName);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <param name="value">Значение</param>
		/// <param name="minVal">минимальное значение</param>
		/// <param name="maxVal">максимальное значение</param>
		/// <param name="yearName">имя года</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, String yearName);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(String name);

		/// <summary>
		/// Создает новый фактор и добавлет его в список факторов.
		/// </summary>
		/// <param name="fac">Фактор</param>
		/// <returns>Успешно ли добавлен</returns>
		Boolean Add(Factor fac);

		/// <summary>
		/// Количестов факторов в списке
		/// </summary>
		Int32 Count { get; }

		/// <summary>
		/// Получает значение фактор по имени
		/// </summary>
		/// <param name="name">Имя фактора</param>
		/// <returns>Значение</returns>
		Decimal this[String name]
		{
			get;
			set;
		}

		/// <summary>
		/// Получает интерфейс на фактор с индексом
		/// </summary>
		/// <param name="index">индекс</param>
		/// <returns>Инерфейс</returns>
		IFactor this[Int32 index]
		{
			get; //set { Items[index].Value = value; }
		}
	}

	/// <summary>
	/// Класс реализующий интерфейс IFactors. Содержит список факторов.
	/// </summary>
	[Guid("B2FF3FCD-F7B1-4138-81AF-6824374CF73B")]
	[ComVisible(true)]
	public class Factors : IFactors, IEnumerable<Factor>
	{
		protected List<Factor> Items = new List<Factor>();

		#region ADDs
		public Boolean Add(String name, String desc, Decimal value)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, desc, value));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name, Decimal value)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, value));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, value, minVal, maxVal));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, String yearName)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, value, minVal, maxVal, yearName));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, value, minVal, maxVal, g_l, g_r));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r, String yearName)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, value, minVal, maxVal, g_l, g_r, yearName));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(String name)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(name, String.Empty, default(Decimal)));
			if (c < Items.Count) return true;
			else return false;
		}

		public Boolean Add(Factor fac)
		{
			Int32 c = Items.Count;
			Items.Add(new Factor(fac.Name, fac.Description, fac.Value, fac.MinVal, fac.MaxVal, fac.G_left, fac.G_right, fac.YearName));
			if (c < Items.Count) return true;
			else return false;
		}
		#endregion

		private Decimal GetValue(String name)
		{
			foreach (Factor item in Items)
			{
				if (item.Name == name)
					return item.Value;
			}
			return 0;  ///must be exeption
		}

		private Boolean ReplaceValue(String name, Decimal newvalue)
		{
			foreach (Factor item in Items)
			{
				if (item.Name == name)
				{
					item.Value = newvalue;
					return true;
				}
			}
			return false;
		}

		protected Boolean ContainsByName(Factor f)
		{
			foreach (Factor item in Items)
			{
				if (item.Name == f.Name)
					return true;
			}
			return false;
		}

		#region Свойства
		public Decimal this[String name]
		{
			get { return GetValue(name); }
			set { ReplaceValue(name, value); }
		}

		/// <summary>
		/// реализует свойство возвращающее интерфейс на фактор с индексом
		/// описанное в интерфейсе IFactor
		/// </summary>
		/// <param name="index">индекс</param>
		/// <returns>значение</returns>
		IFactor IFactors.this[Int32 index]
		{
			get { return Items[index]; }
			//set { Items[index].Value = value; }
		}

		/// <summary>
		/// Возвращает объект типа Factor из списка факторов по индексу
		/// </summary>
		/// <param name="index">индекс</param>
		/// <returns>Объект</returns>
		public Factor this[Int32 index]
		{
			get { return Items[index]; }
			//set { Items[index].Value = value; }
		}
		
		public Int32 Count
		{
			get { return Items.Count; }
		}
		#endregion

		#region IEnumerator
		public IEnumerable<Factor> BottomToTop
		{
			get
			{
				for (Int32 i = 0; i < Items.Count; i++)
					yield return Items[i];
			}
		}

		IEnumerator<Factor> IEnumerable<Factor>.GetEnumerator()
		{
			return BottomToTop.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<Factor>)this).GetEnumerator();
		}
		#endregion
	}
}