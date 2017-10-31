using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Krista.FM.Server.Dashboards.Common
{
	/// <summary>
	/// Упорядоченный список
	/// </summary>
	public class OrderedData
	{
		/// <summary>
		/// Сам список
		/// </summary>
		public List<OrderedValue> Data { private set; get; }

		/// <summary>
		/// Считать ли список обратным
		/// </summary>
		public bool Inversed { set; get; }

		/// <summary>
		/// Точность значений (знаков после запятой)
		/// </summary>
		public int Precision { set; get; }

		/// <summary>
		/// Возвращает худший ранг
		/// </summary>
		public int WorstRank { private set; get; }
		
		/// <summary>
		/// Общее количество элементов в спске
		/// </summary>
		public int Count { get { return Data.Count; } }

		/// <summary>
		/// Количество непустых элементов
		/// </summary>
		public int NonEmptyCount
		{
			get
			{
				int count = 0;
				foreach (OrderedValue value in Data)
				{
					if (!value.IsEmpty)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Возвращает элемент по индексу
		/// </summary>
		public OrderedValue this[int index]
		{
			get { return Data[index]; }
		}

		/// <summary>
		/// Возвращает элемент по ID
		/// </summary>
		public OrderedValue this[string id]
		{
			get { return Data.Find(value => value.ID.Equals(id)); }
		}

		/// <summary>
		/// Инстанцирует класс упорядоченного списка
		/// </summary>
		public OrderedData()
		{
			Data = new List<OrderedValue>();
			Inversed = false;
			WorstRank = 0;
			Precision = 4;
		}

		/// <summary>
		/// Инстанцирует класс упорядоченного списка, используя столбец таблицы
		/// </summary>
		public OrderedData(DataTable dataTable, int columnIndex)
			: this()
		{
			AddFromTable(dataTable, columnIndex);
		}

		/// <summary>
		/// Упорядочивает список
		/// </summary>
		public void Sort()
		{
			Data.Sort();
			if (Inversed)
			{
				Data.Reverse();
			}
			RemoveEmpty();
			CalcRank();
		}

		/// <summary>
		/// Очищает список
		/// </summary>
		public void Clear()
		{
			Data.Clear();
		}

		/// <summary>
		/// Удаляет из списка пустые элементы
		/// </summary>
		public void RemoveEmpty()
		{
			int i = 0;
			while (i < Data.Count)
			{
				OrderedValue value = Data[i];
				if (value.IsEmpty)
				{
					Data.Remove(value);
				}
				else
				{
					i++;
				}
			}
		}

		/// <summary>
		/// Добавляет элемент в список
		/// </summary>
		public void Add(OrderedValue value)
		{
			Data.Add(value);
		}

		/// <summary>
		/// Добавляет элементы из указанного столбца таблицы в список
		/// </summary>
		public void AddFromTable(DataTable dataTable, int columnIndex)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string id = dataTable.Rows[i][0].ToString();
				object value = dataTable.Rows[i][columnIndex];
				if (!CRHelper.DBValueIsEmpty(value))
				{
					Add(new OrderedValue(id, CRHelper.DBValueConvertToDecimalOrZero(value)));
				}
				else
				{
					Add(new OrderedValue(id));
				}
			}
		}

		/// <summary>
		/// Вывод всего списка (для отладки)
		/// </summary>
		public override string ToString()
		{
			StringBuilder text = new StringBuilder();
			int i = 0;
			foreach (OrderedValue value in Data)
			{
				text.AppendFormat(
					"{0} ({1}) {2}: {3}; {4}{5}",
					Convert.ToInt32(i).ToString().PadRight(3),
					Convert.ToInt32(value.Rank).ToString().PadRight(3),
					value.ID.PadRight(50),
					(value.IsEmpty ? "empty" : value.Value.ToString()).PadRight(50),
					value.ExtraValue,
					Environment.NewLine);
				i++;
			}
			return text.ToString();
		}

		/// <summary>
		/// Возвращает ранг элемента по ID
		/// </summary>
		public int GetRank(string id)
		{
			foreach (OrderedValue cur in Data)
			{
				if (cur.ID.Equals(id))
				{
					return cur.Rank;
				}
			}
			return 0;
		}

		/// <summary>
		/// Возвращает ID элемента с указанным индексом
		/// </summary>
		public string GetDataID(int index)
		{
			index = GetIndex(index);
			return Data[index].ID;
		}

		/// <summary>
		/// Возвращает числовое значение элемента с указанным индексом
		/// </summary>
		public decimal GetDataValue(int index)
		{
			index = GetIndex(index);
			return Data[index].Value;
		}

		/// <summary>
		/// Возвращает реальный индекс элемента
		/// </summary>
		private int GetIndex(int index)
		{
			if (index > 0)
			{
				//максимальный по счету элемент
				index--;
			}
			else if (index < 0)
			{
				// минимальный по счету элемент
				index = GetLastNonEmptyIndex() + index;
			}

			if (index >= Data.Count)
				throw new Exception("OrderedData.GetIndex(): index >= Data.Count");
			if (index < 0)
				throw new Exception("OrderedData.GetIndex(): index < 0");

			while (index > 0 && Data[index].IsEmpty)
			{
				index--;
			}

			if (Data[index].IsEmpty)
				throw new Exception("OrderedData.GetIndex(): Data[index].IsEmpty");

			return index;
		}

		/// <summary>
		/// Возвращает индекс последнего непустого элемента
		/// </summary>
		/// <returns></returns>
		private int GetLastNonEmptyIndex()
		{
			int i = Data.Count;
			while (i > 0 && Data[i - 1].IsEmpty)
			{
				i--;
			}
			return i;
		}

		/// <summary>
		/// Вычисляет ранги элементов списка
		/// </summary>
		private void CalcRank()
		{
			// Empty-элементов быть не должно
			// необходимо выполнять ClearEmpty и Sort перед вычислением ранга

			int rank = 1;
			int rankEqual = 0;
			OrderedValue prev = null;
			foreach (OrderedValue cur in Data)
			{
				if (cur.IsEmpty)
				{
					throw new Exception("Empty item in CalcRank() function");
				}

				if (prev != null)
				{
					if ((!Inversed && cur.Value.CompareTo(prev.Value, Precision) < 0)
						|| (Inversed && cur.Value.CompareTo(prev.Value, Precision) > 0))
					{
						rank = rank + rankEqual + 1;
						rankEqual = 0;
					}
					else
					{
						rankEqual++;
					}
				}

				cur.SetRank(rank);

				prev = cur;
			}
			WorstRank = rank;
		}
	}

	/// <summary>
	/// Элемент упорядоченного списка
	/// </summary>
	public class OrderedValue : IComparable<OrderedValue>
	{
		/// <summary>
		/// Идентификатор элемента
		/// </summary>
		public string ID { private set; get; }

		/// <summary>
		/// Является ли элемент пустым
		/// </summary>
		public bool IsEmpty { private set; get; }

		/// <summary>
		/// Числовое значение элемента
		/// </summary>
		public decimal Value { private set; get; }

		/// <summary>
		/// Ранг элемента, доступен после сортировки
		/// </summary>
		public int Rank { private set; get; }

		/// <summary>
		/// Одно дополнительное значение
		/// </summary>
		public decimal ExtraValue { private set; get; }

		/// <summary>
		/// Массив дополнительных значений (на всякий случай)
		/// </summary>
		public object[] ExtraValues { set; get; }
		
		
		public OrderedValue(string id, decimal value)
		{
			ID = id;
			Value = value;
			ExtraValue = 0;
			IsEmpty = false;
			Rank = 0;
		}

		public OrderedValue(string id, double value)
			: this(id, Convert.ToDecimal(value))
		{
		}

		public OrderedValue(string id, int value)
			: this(id, Convert.ToDecimal(value))
		{
		}

		public OrderedValue(string id, decimal value, decimal extraValue)
			: this(id, value)
		{
			ExtraValue = extraValue;
		}

		public OrderedValue(string id, double value, double extraValue)
			: this(id, Convert.ToDecimal(value), Convert.ToDecimal(extraValue))
		{
		}

		public OrderedValue(string id)
		{
			ID = id;
			Value = 0;
			Rank = 0;
			IsEmpty = true;
		}

		public void SetValue(decimal value)
		{
			IsEmpty = false;
			Value = value;
			Rank = 0;
		}

		public void SetExtraValue(decimal value)
		{
			ExtraValue = value;
		}

		public void SetEmpty()
		{
			IsEmpty = true;
			Value = 0;
			Rank = 0;
		}

		public void SetRank(int rank)
		{
			Rank = rank;
		}

		/// <summary>
		/// Сортировка: значение, пустые элементы; при равенстве значений String.Compare по ID
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(OrderedValue other)
		{
			if (IsEmpty && other.IsEmpty)
				return String.Compare(ID, other.ID, true);
			if (IsEmpty)
				return 1;
			if (other.IsEmpty)
				return -1;
			if (Value < other.Value)
				return 1;
			if (Value > other.Value)
				return -1;
			return String.Compare(ID, other.ID, true);
		}

	}
}
