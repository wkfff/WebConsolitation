using System;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	
	/// <summary>
	/// Интерфейс IFactor определяет свойства для факторов. Используется в IFactors
	/// </summary>
	[Guid("08D9002A-8403-4f44-96CD-152CD713B691")]
	[ComVisible(true)]
	public interface IFactor
	{
		/// <summary>
		/// Имя фактора
		/// </summary>
		String Name { get; set; }

		/// <summary>
		/// Описание фактора
		/// </summary>
		String Description { get; set; }

		/// <summary>
		/// Значение
		/// </summary>
		Decimal Value { get; set; }

		/// <summary>
		/// Минимальное значение
		/// </summary>
		Decimal MinVal { get; set; }

		/// <summary>
		/// Максимальное значение
		/// </summary>
		Decimal MaxVal { get; set; }

		/// <summary>
		/// Левая граница
		/// </summary>
		Decimal G_left { get; set; }

		/// <summary>
		/// Правая граница
		/// </summary>
		Decimal G_right { get; set; }

		/// <summary>
		/// Наименование года
		/// </summary>
		String YearName { get; set; }
		
	}

	/// <summary>
	/// Класс реализующий интерфейс IFactor. Содержит конструкторы и поля для типа Factor,
	/// а так же реализации свойств интерфейса.
	/// </summary>
	[Guid("D01F6D9A-F2EE-466e-9E12-F5E35DD7BC7C")]
	[ComVisible(true)]
	public class Factor : IFactor
	{
		private String f_name;
		private String f_desc;
		private Decimal f_value;
		private Decimal minval;
		private Decimal maxval;
		private Decimal g_left;
		private Decimal g_right;
		private String yearName;

		#region Constructors
		public Factor()
		{
		}

		public Factor(String name, String desc, Decimal value)
		{
			this.f_name = name;
			this.f_desc = desc;
			this.f_value = value;
		}

		public Factor(String name, String comments, Decimal value, Decimal min, Decimal max)
		{
			this.f_name = name;
			this.f_desc = comments;
			this.f_value = value;
			this.minval = min;
			this.maxval = max;
		}

		public Factor(String name, String comments, Decimal value, Decimal min, Decimal max, Decimal g_l, Decimal g_r)
		{
			this.f_name = name;
			this.f_desc = comments;
			this.f_value = value;
			this.minval = min;
			this.maxval = max;
			this.g_left = g_l;
			this.g_right = g_r;
		}

		public Factor(String name, String desc, Decimal value, String yearName)
		{
			this.f_name = name;
			this.f_desc = desc;
			this.f_value = value;
			this.yearName = yearName;
		}

		public Factor(String name, String comments, Decimal value, Decimal min, Decimal max, String yearName)
		{
			this.f_name = name;
			this.f_desc = comments;
			this.f_value = value;
			this.minval = min;
			this.maxval = max;
			this.yearName = yearName;
		}

		public Factor(String name, String comments, Decimal value, Decimal min, Decimal max, Decimal g_l, Decimal g_r, String yearName)
		{
			this.f_name = name;
			this.f_desc = comments;
			this.f_value = value;
			this.minval = min;
			this.maxval = max;
			this.g_left = g_l;
			this.g_right = g_r;
			this.yearName = yearName;
		}
		#endregion

		#region Свойства
		public String Name
		{
			get { return f_name; }
			set { f_name = value; }
		}

		public String Description
		{
			get { return f_desc; }
			set { f_desc = value; }
		}

		public Decimal Value
		{
			get { return f_value; }
			set { this.f_value = value; }
		}

		public Decimal MinVal
		{
			get { return minval; }
			set { minval = value; }
		}

		public Decimal MaxVal
		{
			get { return maxval; }
			set { maxval = value; }
		}

		public Decimal G_left
		{
			get { return g_left; }
			set { g_left = value; }
		}

		public Decimal G_right
		{
			get { return g_right; }
			set { g_right = value; }
		}

		public string YearName
		{
			get { return yearName; }
			set { yearName = value; }
		}

		#endregion
	}
}