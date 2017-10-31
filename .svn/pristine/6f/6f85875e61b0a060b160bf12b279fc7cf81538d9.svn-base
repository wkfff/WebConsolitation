using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
	/// <summary>
	/// Добавляет отступ в соответствии с иерархией
	/// </summary>
    public class PaddingRule : IndicatorRule
    {
        private int baseColumnIndex = -1;
		private string baseColumnName = String.Empty;
		private int levelColumnIndex = -1;
        private string levelColumnName = String.Empty;


        public int BasePadding
        {
            get; set;
        }

		/// <summary>
		/// Добавляет отступ в соответствии с иерархией
		/// </summary>
		/// <param name="baseColumnIndex">номер колонки с показателем</param>
		/// <param name="levelColumnIndex">номер колонки с уровнем иерархии</param>
		/// <param name="basePadding">шаг отступа (в пикселях)</param>
        public PaddingRule(int baseColumnIndex, int levelColumnIndex, int basePadding)
        {
            this.baseColumnIndex = baseColumnIndex;
            this.levelColumnIndex = levelColumnIndex;
        	BasePadding = basePadding;
        }

		/// <summary>
		/// Добавляет отступ в соответствии с иерархией
		/// </summary>
		/// <param name="baseColumnIndex">номер колонки с показателем</param>
		/// <param name="levelColumnName">название колонки с уровнем иерархии</param>
		/// <param name="basePadding">шаг отступа (в пикселях)</param>
		public PaddingRule(int baseColumnIndex, string levelColumnName, int basePadding)
		{
			this.baseColumnIndex = baseColumnIndex;
			this.levelColumnName = levelColumnName;
			BasePadding = basePadding;
		}

		/// <summary>
		/// Добавляет отступ в соответствии с иерархией
		/// </summary>
		/// <param name="baseColumnName">название колонки с показателем</param>
		/// <param name="levelColumnName">название колонки с уровнем иерархии</param>
		/// <param name="basePadding">шаг отступа (в пикселях)</param>
        public PaddingRule(string baseColumnName, string levelColumnName, int basePadding)
        {
            this.baseColumnName = baseColumnName;
            this.levelColumnName = levelColumnName;
			BasePadding = basePadding;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
			// если не все параметры найдены - выходим
			if (baseColumnIndex == -2 || levelColumnIndex == -2)
				return;
        	
			int i;

			// находим номер колонки с показателем
			i = 0;
			while ((i < row.Cells.Count) && (baseColumnIndex == -1))
			{
				if (row.Band.Grid.Columns[i].Header.Caption.Contains(baseColumnName))
				{
					baseColumnIndex = i;
				}
				i++;
			}

			// находим номер колонки с уровнем иерархии
			i = 0;
			while ((i < row.Cells.Count) && (levelColumnIndex == -1))
			{
				if (row.Band.Grid.Columns[i].Header.Caption.Contains(levelColumnName))
				{
					levelColumnIndex = i;
				}
				i++;
			}
			
			// если не нашли чего-то нужного - выход
			if (baseColumnIndex == -1 || levelColumnIndex == -1)
			{
				baseColumnIndex = -2;
				levelColumnIndex = -2;
				return;
			}

			// все необходимые параметры найдены
			UltraGridCell cell = row.Cells[baseColumnIndex];
			int level = GetIntDtValue(row, levelColumnIndex);
			if (level != Int32.MinValue)
			{
				if (level < 0)
				{
				    level = 0;
				}

			    cell.Style.CustomRules = String.Format("padding-left: {0}px;", 3 + BasePadding*level);
			}
        	
        }
    }
}
