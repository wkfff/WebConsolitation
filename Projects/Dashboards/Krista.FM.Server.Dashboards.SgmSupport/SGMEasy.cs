using System;
using System.Data;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;

namespace Krista.FM.Server.Dashboards.SgmSupport
{
	/// <summary>
	/// Получение данных из БД СГМ - заболеваемость
	/// </summary>
	public class SGMEasy
	{

		public int Year { private set; get; }
		public string AreaFM { private set; get; }
		public string Diseases { private set; get; }
		public DataTable DiseasesTable { private set; get; }
		private SGMRegionNamer RegionNamer { set; get; }

		public SGMEasy()
		{
			RegionNamer = new SGMRegionNamer();
			RegionNamer.FillFMtoSGMRegions();
		}

		public SGMEasy(string diseases, int year) 
			: this()
		{
			Year = year;
			Diseases = diseases;
			DiseasesTable = GetDiseaseAreasTable();
		}

		public SGMEasy(string diseases, string areaFM)
			: this()
		{
			AreaFM = areaFM;
			Diseases = diseases;
			DiseasesTable = GetDiseaseYearsTable();
		}

		/// <summary>
		/// Приводит название субъекта ФМ к названию СГМ
		/// </summary>
		private string FMtoSGMAreaName(string areaFM)
		{
			string areaSGM = RegionNamer.GetSGMName(areaFM);
			
			return areaSGM;
		}

		/// <summary>
		/// Приводит название субъекта ФМ к короткому СГМ (касается только РФ, ФО)
		/// </summary>
		private string FMtoSGMShortAreaName(string areaFM)
		{
			// приводим название субъекта к тому, что понимает СГМ
			if (RegionsNamingHelper.IsRF(areaFM))
			{
				areaFM = "РФ";
			}
			else if (RegionsNamingHelper.IsFO(areaFM))
			{
				areaFM = RegionsNamingHelper.ShortName(areaFM)
					.Replace("УрФО", "УФО");
			}
			
			// то, что не заменилось заменяем на имена SGM
			string areaSGM = RegionNamer.GetSGMName(areaFM);
			
			return areaSGM;
		}
		
		/// <summary>
		/// Получает значение из БД СГМ - заболеваемость
		/// </summary>
		public double GetDiseaseDouble(string areaFM)
		{
			return CRHelper.DBValueConvertToDoubleOrZero(GetDiseaseValue(areaFM));
		}

		/// <summary>
		/// Получает значение из БД СГМ - заболеваемость
		/// </summary>
		public decimal GetDiseaseDecimal(string areaFM)
		{
			return CRHelper.DBValueConvertToDecimalOrZero(GetDiseaseValue(areaFM));
		}

		/// <summary>
		/// Получает значение из БД СГМ - заболеваемость
		/// </summary>
		public object GetDiseaseValue(string areaFM)
		{
			return DiseasesTable.FindValue(FMtoSGMShortAreaName(areaFM), 1);
		}

		/// <summary>
		/// Получает таблицу значений из БД СГМ - заболеваемость
		/// </summary>
		private DataTable GetDiseaseAreasTable()
		{
			SGMDataObject dataObject = new SGMDataObject();

			dataObject.InitObject();
			dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
			dataObject.AddColumn(
				SGMDataObject.DependentColumnType.dctAbs,
				Convert.ToString(Year), // year
				String.Empty, // months
				String.Empty, // area
				Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll)),
				Diseases);

			return dataObject.FillData();
		}

		/// <summary>
		/// Получает таблицу значений из БД СГМ - заболеваемость
		/// </summary>
		private DataTable GetDiseaseYearsTable()
		{
			SGMDataObject dataObject = new SGMDataObject();
			SGMDataRotator dataRotator = new SGMDataRotator();
		
			dataObject.InitObject();
			dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
			for (int year = dataRotator.GetFirstYear(); year <= dataRotator.GetLastYear(); year++)
			{
				dataObject.AddColumn(
					SGMDataObject.DependentColumnType.dctAbs,
					year.ToString(), // year
					String.Empty, // months
					FMtoSGMAreaName(AreaFM), // area
					Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll)),
					Diseases);
			}
			DataTable table = dataObject.FillData();
		
			DataTable result = new DataTable();
			result.Columns.Add(new DataColumn("year", typeof(string)));
			result.Columns.Add(new DataColumn("value", typeof(double)));
			for (int i = 1; i < table.Columns.Count; i++)
			{
				DataRow row = result.NewRow();
				row[0] = Convert.ToString(dataRotator.GetFirstYear() + i);
				row[1] = table.FindValue(FMtoSGMShortAreaName(AreaFM), i) ?? DBNull.Value;
				result.Rows.Add(row);
			}

			return result;
		}

	}
}
