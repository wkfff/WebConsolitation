using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// Интерфейс индикаторов. Полностью наследуется от IFactors
	/// </summary>
	[Guid("8A1C2C84-2292-4e7a-B316-75EA248D4FB9")]
	[ComVisible(true)]
	public interface IIndicators: IEnumerable<Factor>, IFactors
	{

	}

	/// <summary>
	/// Класс индикаторов реализующий интерфейсы IIndicators и IFactors. 
	/// Полность наследуется от Factors
	/// </summary>
	[Guid("20D95C86-4F9D-4068-9270-BC24EE9F03B0")]
	[ComVisible(true)]
	public class Indicators : Factors, IIndicators
	{

	}
}