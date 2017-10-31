using System;
using System.Collections.Generic;
using Infragistics.Win.UltraWinGrid;


namespace Krista.FM.Client.Components
{
    /// <summary>
    /// Сохранение состояния грида.
    /// </summary>
    public class UltraGridStateSettings
    {
		/// <summary>
		/// Видимость столбцов.
		/// </summary>
		public Dictionary<string, bool> VisibleColumns = new Dictionary<string, bool>();
		
		/// <summary>
		/// Позиции столбцов.
		/// </summary>
		public Dictionary<string, int> ColumnsPositions = new Dictionary<string, int>();

        /// <summary>
        /// Установленные фильтры.
        /// </summary>
        public ColumnFilter[] Filters;

        /// <summary>
        /// Сортировка и группировка колонок.
        /// </summary>
        public UltraGridColumn[] SortColumns;
        
        /// <summary>
        /// Активная строка. 
        /// Предпологается, что строка идентифицируется по полю с именем "ID".
        /// </summary>
        public int? ActiveRowID;

        /// <summary>
        /// Выделенные строки.
        /// Предпологается, что строка идентифицируется по полю с именем "ID".
        /// </summary>
        public int[] SelectedRowsID;

        /// <summary>
        /// Сохраняет пользовательские настройки грида.
        /// </summary>
        /// <param name="grid">Грид, для которого необходимо сохранить настройки.</param>
        /// <returns>Сохраненные настройки грида.</returns>
        public static UltraGridStateSettings SaveUltraGridFilterSettings(UltraGrid grid)
        {
            UltraGridStateSettings settings = new UltraGridStateSettings();

            // Сохраняем установленные фильтры.
            // (Сохранение делается только для одной банды, нужно сделать для всех)
            settings.Filters = new ColumnFilter[grid.DisplayLayout.Bands[0].ColumnFilters.Count];
            grid.DisplayLayout.Bands[0].ColumnFilters.CopyTo(settings.Filters, 0);

            return settings;
        }

        /// <summary>
        /// Сохраняет пользовательские настройки грида.
        /// </summary>
        /// <param name="grid">Грид, для которого необходимо сохранить настройки.</param>
        /// <returns>Сохраненные настройки грида.</returns>
        public static UltraGridStateSettings SaveUltraGridStateSettings(UltraGrid grid)
        {
            // (Сохранение делается только для одной банды, нужно сделать для всех)
            UltraGridStateSettings settings = new UltraGridStateSettings();

			// Сохраняем видимость и позицию колонок (только для первого банда)
        	foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns)
        	{
				settings.VisibleColumns.Add(column.Key, column.Hidden);
				settings.ColumnsPositions.Add(column.Key, column.Header.VisiblePosition);
			}

            // Сохраняем сортировку и группировку колонок грида
            settings.SortColumns = new UltraGridColumn[grid.DisplayLayout.Bands[0].SortedColumns.Count];
            grid.DisplayLayout.Bands[0].SortedColumns.CopyTo(settings.SortColumns, 0);

            // Сохраняем установленные фильтры.
            settings.Filters = new ColumnFilter[grid.DisplayLayout.Bands[0].ColumnFilters.Count];
            grid.DisplayLayout.Bands[0].ColumnFilters.CopyTo(settings.Filters, 0);

            // Запоминаем активную строку
			if (grid.ActiveRow != null && grid.ActiveRow.Cells.IndexOf("ID") != -1)
			{
				settings.ActiveRowID = grid.ActiveRow.IsDataRow ?
					(int?)Convert.ToInt32(grid.ActiveRow.Cells["ID"].Value) :
					null;
			}

            // Сохраняем выделение строк
			if (grid.ActiveRow != null && grid.ActiveRow.Cells.IndexOf("ID") != -1)
			{
				settings.SelectedRowsID = new int[grid.Selected.Rows.Count];
				int idx = 0;
				foreach (UltraGridRow row in grid.Selected.Rows)
				{
					if (row.IsDataRow)
					{
						settings.SelectedRowsID[idx++] = Convert.ToInt32(row.Cells["ID"].Value);
					}
				}
			}

        	return settings;
        }

        /// <summary>
        /// Восстанавливает сохраненные настройки фильтров грида.
        /// </summary>
        /// <param name="grid">Грид, для которого необходимо восстановить состояние.</param>
        public void RestoreUltraGridFilterSettings(UltraGrid grid)
        {
            // Восстанавливаем сохраненные настройки фильтров
            grid.BeginUpdate();
            try
            {
                // Восстанавливаем фильтры
                for (int i = 0; i < Filters.GetLength(0); i++)
                {
                    if (Filters[i] != null)
                    {
                        foreach (FilterCondition o in Filters[i].FilterConditions)
                        {
                            grid.DisplayLayout.Bands[0].ColumnFilters[i].FilterConditions.Add(o);
                        }
                    }
                }
            }
            finally
            {
                grid.EndUpdate();
            }
        }

        /// <summary>
        /// Восстанавливает состояние грида.
        /// </summary>
        /// <param name="grid">Грид, для которого необходимо восстановить состояние.</param>
        public void RestoreUltraGridStateSettings(UltraGrid grid)
        {
            // Восстанавливаем сохраненные настройки фильтров, сортировки и выделение строк
            grid.BeginUpdate();
            try
            {
				// Восстанавливаем позицию колонок
				foreach(KeyValuePair<string, int> item in ColumnsPositions)
				{
					grid.DisplayLayout.Bands[0].Columns[item.Key].Header.VisiblePosition = item.Value;
				}

				// Восстанавливаем видимость колонок
				foreach (KeyValuePair<string, bool> item in VisibleColumns)
            	{
					grid.DisplayLayout.Bands[0].Columns[item.Key].Hidden = item.Value;
            	}

				// Восстанавливаем сортировку 
                for (int i = 0; i < SortColumns.GetLength(0); i++)
                {
                    if (SortColumns[i] != null)
                    {
                        if (SortColumns[i].SortIndicator == SortIndicator.Ascending ||
                            SortColumns[i].SortIndicator == SortIndicator.Descending)
                        {
                            grid.DisplayLayout.Bands[0].SortedColumns.Add(
                                grid.DisplayLayout.Bands[0].Columns[SortColumns[i].Key],
                                SortColumns[i].SortIndicator == SortIndicator.Descending, 
                                SortColumns[i].IsGroupByColumn);
                        }
                    }
                }

                // Восстанавливаем фильтры
                for (int i = 0; i < Filters.GetLength(0); i++)
                {
                    if (Filters[i] != null)
                    {
                        foreach (FilterCondition o in Filters[i].FilterConditions)
                        {
                            grid.DisplayLayout.Bands[0].ColumnFilters[i].FilterConditions.Add(o);
                        }
                    }
                }

                // Выделение строк
				if (SelectedRowsID != null)
				{
				    foreach (int rowID in SelectedRowsID)
				    {
				        UltraGridRow row = UltraGridHelper.FindGridRow(grid, "ID", rowID, false);
				        if (row != null)
				        {
							if (row.IsFilteredOut)
							{
								List<UltraGridRow> path = GetPathToRow(grid, row, new List<UltraGridRow>());
								ExpandGridToRow(path);
							}
							row.Selected = true;
				        }
				    }
				}
            	
				// Устанавливаем активную строку
				if (ActiveRowID != null)
				{
				    UltraGridRow activeRow = UltraGridHelper.FindGridRow(grid, "ID", ActiveRowID, false);
				    if (activeRow != null)
				    {
				        if (activeRow.IsFilteredOut)
				        {
				        	List<UltraGridRow> path = GetPathToRow(grid, activeRow, new List<UltraGridRow>());
				        	ExpandGridToRow(path);
				        }
			            activeRow.Activate();
				    }
				}
            }
            finally
            {
                grid.EndUpdate();
            }
        }

		private static List<UltraGridRow> GetPathToRow(UltraGrid grid, UltraGridRow currentRow, List<UltraGridRow> path)
		{
			path.Add(currentRow);
			
			if (currentRow.Cells["ParentID"].Value is DBNull)
				return path;

			UltraGridRow parentRow = UltraGridHelper.FindGridRow(grid, "ID", currentRow.Cells["ParentID"].Value, false);
			return GetPathToRow(grid, parentRow, path);
		}

		private static UltraGridRow ExpandGridToRow(List<UltraGridRow> path)
		{
			UltraGridRow[] rows = new UltraGridRow[path.Count];
			int i = path.Count - 1;
			foreach (UltraGridRow row in path)
			{
				rows[i--] = row;
			}

			for (i = 0; i < path.Count - 1; i++)
			{
				rows[i].Expanded = true;
			}

			return rows[path.Count - 1];
		}
    }
}
