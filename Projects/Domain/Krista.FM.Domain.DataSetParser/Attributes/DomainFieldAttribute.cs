using System;

namespace Krista.FM.Domain.DataSetParser.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DomainFieldAttribute : DomainAttribute
	{
		public DomainFieldAttribute(string name)
			: base(name)
		{
			Name = name;
		}

		private bool isPrimaryKey;
		/// <summary>
		/// Указывает, что данное поле является первичным ключом в базе данных.
		/// </summary>
		public bool IsPrimaryKey
		{
			get { return isPrimaryKey; }
			set { isPrimaryKey = value; }
		}

		private object defaultValue;
		/// <summary>
		/// Значение по умолчанию.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}
	}
}
