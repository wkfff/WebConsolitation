using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// ��������� IFactors ���������� ������ ���������� � ������� � ������
	/// ������ �������� Factor. ����������� � IIndicators, IAdjusters
	/// </summary>
	[Guid("10E6FF25-F48E-4196-A8FF-0020231F02B4")]
	[ComVisible(true)]
	public interface IFactors
	{
		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="desc">��������</param>
		/// <param name="value">��������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, String desc, Decimal value);
		
		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="value">��������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, Decimal value);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="value">��������</param>
		/// <param name="minVal">����������� ��������</param>
		/// <param name="maxVal">������������ ��������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="value">��������</param>
		/// <param name="minVal">����������� ��������</param>
		/// <param name="maxVal">������������ ��������</param>
		/// <param name="g_l">������� �����</param>
		/// <param name="g_r">������� ������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="value">��������</param>
		/// <param name="minVal">����������� ��������</param>
		/// <param name="maxVal">������������ ��������</param>
		/// <param name="g_l">������� �����</param>
		/// <param name="g_r">������� ������</param>
		/// <param name="yearName">��� ����</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, Decimal g_l, Decimal g_r, String yearName);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="value">��������</param>
		/// <param name="minVal">����������� ��������</param>
		/// <param name="maxVal">������������ ��������</param>
		/// <param name="yearName">��� ����</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name, Decimal value, Decimal minVal, Decimal maxVal, String yearName);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(String name);

		/// <summary>
		/// ������� ����� ������ � �������� ��� � ������ ��������.
		/// </summary>
		/// <param name="fac">������</param>
		/// <returns>������� �� ��������</returns>
		Boolean Add(Factor fac);

		/// <summary>
		/// ���������� �������� � ������
		/// </summary>
		Int32 Count { get; }

		/// <summary>
		/// �������� �������� ������ �� �����
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <returns>��������</returns>
		Decimal this[String name]
		{
			get;
			set;
		}

		/// <summary>
		/// �������� ��������� �� ������ � ��������
		/// </summary>
		/// <param name="index">������</param>
		/// <returns>��������</returns>
		IFactor this[Int32 index]
		{
			get; //set { Items[index].Value = value; }
		}
	}

	/// <summary>
	/// ����� ����������� ��������� IFactors. �������� ������ ��������.
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

		#region ��������
		public Decimal this[String name]
		{
			get { return GetValue(name); }
			set { ReplaceValue(name, value); }
		}

		/// <summary>
		/// ��������� �������� ������������ ��������� �� ������ � ��������
		/// ��������� � ���������� IFactor
		/// </summary>
		/// <param name="index">������</param>
		/// <returns>��������</returns>
		IFactor IFactors.this[Int32 index]
		{
			get { return Items[index]; }
			//set { Items[index].Value = value; }
		}

		/// <summary>
		/// ���������� ������ ���� Factor �� ������ �������� �� �������
		/// </summary>
		/// <param name="index">������</param>
		/// <returns>������</returns>
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