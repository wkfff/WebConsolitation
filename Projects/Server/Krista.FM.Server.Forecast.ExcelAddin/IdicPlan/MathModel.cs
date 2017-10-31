using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Krista.FM.Server.Forecast.ExcelAddin.IdicPlan;

namespace Krista.FM.Server.Forecast.ExcelAddin.IdicPlan
{
	/// <summary>
	/// Интерфейс математического взаимодействия с excel'евской моделью
	/// служащий для передачи регуляторов и чтения индикаторов
	/// </summary>
	[Guid("57400284-49F7-4aee-AFE4-21ABE91CF64C")]
	[ComVisible(true)]
	public interface IMathModel : IDisposable
	{
		/// <summary>
		/// Название пронозного года на который подбираются регуляторы
		/// </summary>
		String YearName { set; get; }
		
		/// <summary>
		/// Передает регулятор в модель на год установленный в YearName
		/// </summary>
		/// <param name="curadj">Значение</param>
		void SendAdj(Adjusters curadj);

		/// <summary>
		/// Получает интерфейс на регуляторы модели
		/// </summary>
		IAdjusters Adj { get; }

		/// <summary>
		/// Получает интерфейс на индикаторы модели
		/// </summary>
		IIndicators Ind { get; }

		/// <summary>
		/// Возвращает значение индикатора из модели 
		/// </summary>
		/// <param name="ind">индикатор</param>
		/// <returns>Значение</returns>
		Decimal CalcInd(Factor ind);

		/// <summary>
		/// Возвращает значение индикатора из модели на год установленный в YearName
		/// </summary>
		/// <param name="name">Имя индикатора</param>
		/// <returns>Значение</returns>
		Decimal CalcInd(String name);
	}

		/// <summary> 
		/// Абстрактный класс математической модели используется для построения 
		/// класса MathModel
		/// </summary>
		public abstract class Model
		{
			protected Adjusters adj;
			protected Indicators ind;

			/// <summary>
			/// Возвращает интерфейс к списку регуляторов
			/// </summary>
			public IAdjusters Adj
			{
				get { return adj; }
			}

			/// <summary>
			/// Возвращает интерфейс к списку индикатооров
			/// </summary>
			public IIndicators Ind
			{
				get { return ind; }
			}

			public abstract Decimal CalcInd(String name, Adjusters curadj);
			public abstract void SendAdj(Adjusters curadj);

			protected Model()
			{
				adj = new Adjusters();
				ind = new Indicators();
			}
		}

		/// <summary>
		/// Класс математической модели
		/// </summary>
		[Guid("A0AD51F0-0D34-4188-8184-8F28ACA4752D")]
		[ComVisible(true)]
		public class MathModel : Model, IMathModel
		{
			private ExModel mod;
			private IWorkbookOfModel tmpInd;
			private IWorkbookOfModel tmpAdj;
			private Worksheet shtInd;
			private Worksheet shtAdj;

			/// <summary>
			/// ПРогнозный год, на который извлекаются индикаторы и модифицируются регуляторы
			/// </summary>
			private String yearName = String.Empty;

			/// <summary>
			/// Конструктор класса
			/// </summary>
			/// <param name="model">Объект Excel модели </param>
			public MathModel(ExModel model)
				: base()
			{
				Trace.TraceMes("Конструктор Math Model");
				mod = model;
				tmpInd = Mod.GetWorkBook("Индикаторы.xls");
				shtInd = (Worksheet)tmpInd.WorkBook.Worksheets.get_Item("Индикаторы");
				tmpAdj = Mod.GetWorkBook("Регуляторы.xls");
				shtAdj = (Worksheet)tmpAdj.WorkBook.Worksheets.get_Item("Регуляторы");
			}

			#region Свойства
			/// <summary>
			/// Название пронозного на который подбираются регуляторы
			/// </summary>
			public String YearName
			{
				set { yearName = value; }
				get { return yearName; }
			}

			public ExModel Mod
			{
				get { return mod; }
			}

			#endregion

			/// <summary>
			/// неиспользовать!!!!!!!!!!!!
			/// </summary>
			/// <param name="name"></param>
			/// <param name="curadj"></param>
			/// <returns></returns>
			[ObsoleteAttribute("Осталься наследием от первых версий модели.", true)]
			public override Decimal CalcInd(String name, Adjusters curadj)
			{
				return 0;
			}

			/// <summary>
			/// Возвращает значение индикатора из модели на год установленный в YearName
			/// </summary>
			/// <param name="name">Имя индикатора</param>
			/// <returns>Значение</returns>
			public Decimal CalcInd(String name)
			{
				//String yearName;
				//			return (Double)tmpInd.GetDataFromCell("Индикаторы", name, "y.1");
				return (Decimal)tmpInd.GetDataFromCell(shtInd, name, yearName);
			}

			/// <summary>
			/// Возвращает значение индикатора из модели 
			/// </summary>
			/// <param name="ind">индикатор</param>
			/// <returns>Значение</returns>
			public Decimal CalcInd(Factor ind)
			{
				return Convert.ToDecimal(tmpInd.GetDataFromCell(shtInd, ind.Name, ind.YearName));
			}


			/// <summary>
			/// Передает регулятор в модель на год установленный в YearName
			/// </summary>
			/// <param name="curadj">Значение</param>
			public override void SendAdj(Adjusters curadj)
			{
				foreach (Factor fadj in curadj)
				{
					//tmpAdj.SetDataToCell("Регуляторы", fadj.Name, "y.1", fadj.Value);
					if (fadj.YearName == String.Empty)
						tmpAdj.SetDataToCell(shtAdj, fadj.Name, yearName, fadj.Value);
					else
					{
						//Trace.TraceMes("Передача {0} на {1} в лист {2}",fadj.Name,fadj.YearName,shtAdj.Name);
						tmpAdj.SetDataToCell(shtAdj, fadj.Name, fadj.YearName, fadj.Value);
					}
				}
			}

			public void Dispose()
			{
				try
				{
					if (Marshal.IsComObject(tmpAdj))
						Marshal.ReleaseComObject(tmpAdj);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(tmpInd))
						Marshal.ReleaseComObject(tmpInd);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(shtAdj))
						Marshal.ReleaseComObject(shtAdj);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(shtInd))
						Marshal.ReleaseComObject(shtInd);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(adj))
						Marshal.ReleaseComObject(adj);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(ind))
						Marshal.ReleaseComObject(ind);
				}
				catch (Exception e) { }

				try
				{
					if (Marshal.IsComObject(mod))
						Marshal.ReleaseComObject(mod);
				}
				catch (Exception e) { }

				tmpAdj = null;
				tmpInd = null;
				shtAdj = null;
				shtInd = null;
				adj = null;
				ind = null;
				mod = null;

				GC.Collect();
			}
		}
	
}