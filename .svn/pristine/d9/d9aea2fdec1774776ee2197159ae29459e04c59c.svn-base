namespace Krista.FM.Server.Dashboards.Common.Export
{
	public abstract class ParamsMargins : ParamsBase
	{
		public ItemMargins Margins { set; get; }

		protected ParamsMargins()
		{
			Margins = new ItemMargins();
		}

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public void SetMargins(ItemMargins margins)
		{
			Margins = margins;
		}

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public void SetMargins(double all)
		{
			Margins.All = all;
		}

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public void SetMargins(double top, double right, double bottom, double left)
		{
			Margins.SetMargins(top, right, bottom, left);
		}
	}
}
