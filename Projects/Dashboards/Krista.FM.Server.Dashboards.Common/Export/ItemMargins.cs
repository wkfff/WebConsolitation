using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// ѕредставл€ет отступы дл€ элемента (в миллиметрах)
	/// </summary>
	public class ItemMargins
	{
		public double Top { set; get; }
		public double Right { set; get; }
		public double Bottom { set; get; }
		public double Left { set; get; }

		/// <summary>
		/// ”станавливает все отступы одним значением (в миллиметрах)
		/// </summary>
		public double All
		{
			set
			{
				Top = value;
				Right = value;
				Bottom = value;
				Left = value;
			}
		}

		/// <summary>
		/// »нстанцирует класс, задающий отступы (в миллиметрах)
		/// </summary>
		public ItemMargins(double all = 0)
		{
			All = all;
		}

		/// <summary>
		/// »нстанцирует класс, задающий отступы (в миллиметрах)
		/// </summary>
		public ItemMargins(double top, double right, double bottom, double left)
		{
			Top = top;
			Right = right;
			Bottom = bottom;
			Left = left;
		}

		/// <summary>
		/// «адает отступы (в миллиметрах)
		/// </summary>
		public void SetMargins(double top, double right, double bottom, double left)
		{
			Top = top;
			Right = right;
			Bottom = bottom;
			Left = left;
		}
	}
}