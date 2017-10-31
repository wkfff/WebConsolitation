using System.Drawing;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	public interface IHeadered
	{
		string Title { set; get; }
		Font Font { set; get; }
	}
}
