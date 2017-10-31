using System;

namespace Krista.FM.Domain.DataSetParser.Attributes
{
	public abstract class DomainAttribute : Attribute
	{
		protected DomainAttribute(string name)
		{
			Name = name;
		}

		private string name;
		/// <summary>
		/// Имя в базе данных.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
