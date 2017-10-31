using System;

namespace Krista.FM.Server.Users
{
	#region Базовый класс для объектов системы
	/// <summary>
	/// Базовый класс для объектов системы.
	/// </summary>
	public abstract class SysBaseObject
	{
		/// <summary>
		/// ID объекта
		/// </summary>
		public int ID = -1;
		/// <summary>
		/// Внутреннее имя объекта
		/// </summary>
		public string Name = String.Empty;
		/// <summary>
		/// Отображаемое имя объекта
		/// </summary>
		public string Caption = String.Empty;
		/// <summary>
		/// Описание объекта
		/// </summary>
		public string Description = String.Empty;
	}
	#endregion

	#region Базовый класс для пользователей и групп
	/// <summary>
	/// Базовый класс для пользователей и групп
	/// </summary>
	public abstract class SysBaseWinObject : SysBaseObject
	{
		/// <summary>
		/// Имя объекта в формате DNS
		/// </summary>
		public string DNSName;
		/// <summary>
		/// Признак блокирования объекта
		/// </summary>
		public bool Blocked;
	}
	#endregion
}