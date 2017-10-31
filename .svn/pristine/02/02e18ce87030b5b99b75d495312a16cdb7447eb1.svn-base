using System;
using System.Collections.Generic;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Configuration;
using Itenso.Configuration;

namespace Krista.FM.Client.Common.Configuration
{
    /// <summary>
    /// Опции сохранения/загрузки пользовательских настроек для грида
    /// </summary>
    [Flags]
    public enum SaveOptions
    {
        /// <summary>
        /// Сортировка
        /// </summary>
        SortedColumns = 1, 
        /// <summary>
        /// Фильтры
        /// </summary>
        ColumnFillters = 2,
        /// <summary>
        /// Настройки столбцов
        /// </summary>
        ColumnSettings = 4,
        /// <summary>
        /// Видимость области группировки
        /// </summary>
        GroupHeaderVisible = 8,
        /// <summary>
        /// Видимость кнопок добавления записей
        /// </summary>
        AddButtonVisible = 16,
        /// <summary>
        /// Базовый набор сохранения для грида
        /// </summary>
        Basic = SortedColumns | ColumnFillters | ColumnSettings | GroupHeaderVisible | AddButtonVisible
    }

    /// <summary>
    /// Частичное сохранение настроек грида
    /// </summary>
    public class UltraGridExSettingsPartial : Setting
    {
        private readonly UltraGrid grid;
        private readonly string name;
        private readonly SaveOptions options;

        public UltraGridExSettingsPartial(string name, UltraGrid grid)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            this.name = name;
            this.grid = grid;

            this.options = SaveOptions.Basic;
        }

        public UltraGridExSettingsPartial(string name, UltraGrid grid, SaveOptions options)
            : this(name, grid)
        {
            this.options = options;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        private bool IsSetOptions(SaveOptions singleOption)
        {
            return (options & singleOption) == singleOption;
        }

        /// <summary>
        /// Проверяет, были ли изменены пользовательские настройки грида
        /// </summary>
        public override bool HasChanged
        {
            get 
            {
                UltraGridColumnSetting[] originalColumnSettings = OriginalColumnSettings;
                UltraGridColumnSetting[] columnSettings = ColumnSettings;
                ColumnFilter[] originalColumnFillers = OriginalColumnFillters;
                ColumnFilter[] columnFillers = new ColumnFilter[grid.DisplayLayout.Bands[0].ColumnFilters.Count];
                grid.DisplayLayout.Bands[0].ColumnFilters.CopyTo(columnFillers, 0);
                                               
                if (originalColumnSettings == null || columnSettings == null || OriginalAddButtonVisible == null)
                {
                    return true;
                }

                if (originalColumnSettings.Length != columnSettings.Length)
                {
                    return true;
                }

                for (int i = 0; i < originalColumnSettings.Length; i++)
                {
                    if (!originalColumnSettings[i].Equals(columnSettings[i]))
                    {
                        return true;
                    }
                }

                if (!OriginalGroupHeaderVisible.Equals(grid.DisplayLayout.GroupByBox.Hidden.ToString()))
                    return true;

                if (!OriginalAddButtonVisible.Equals(grid.DisplayLayout.AddNewBox.Hidden.ToString()))
                    return true;
                
                if (originalColumnFillers.Length != columnFillers.Length)
                {
                    return true;
                }

                for (int i = 0; i < originalColumnFillers.Length; i++)
                {
                    if ((columnFillers[i] != null && originalColumnFillers[i] == null) ||
                        (columnFillers[i] == null && originalColumnFillers[i] != null))
                        return true;

                    if (columnFillers[i] != null && originalColumnFillers[i] != null)
                    {
                        if (columnFillers[i].FilterConditions.Count != originalColumnFillers[i].FilterConditions.Count)
                            return true;

                        foreach (FilterCondition filterCondition in columnFillers[i].FilterConditions)
                        {
                            if (!originalColumnFillers[i].FilterConditions.Contains(filterCondition))
                                return true;
                        }
                    }
                }

                return false; 
            }
        } // HasChanged

        private UltraGridColumnSetting[] ColumnSettings
        {
            get
            {
                if (grid.DisplayLayout.Bands[0].Columns.Count == 0)
                {
                    return null;
                }

                List<UltraGridColumnSetting> columns =
                    new List<UltraGridColumnSetting>(grid.DisplayLayout.Bands[0].Columns.Count);
                foreach (UltraGridColumn dataGridViewColumn in grid.DisplayLayout.Bands[0].Columns)
                {
                    columns.Add(new UltraGridColumnSetting(dataGridViewColumn));
                }
                return columns.ToArray();
            }
        } // ColumnSettings

        #region Оригинальные настройки грида

        /// <summary>
        /// 
        /// </summary>
        private UltraGridColumnSetting[] OriginalColumnSettings
        {
            get
            {
                return LoadValue(
                    "ColumnSettings",
                    typeof(UltraGridColumnSetting[]),
                    SettingsSerializeAs.Binary,
                    null) as UltraGridColumnSetting[];
            }
        } // OriginalColumnSettings        

        /// <summary>
        /// Видимость области группировки в пользовательских настройках
        /// </summary>
        private string OriginalGroupHeaderVisible
        {
            get
            {
                return LoadValue(
                   "GroupHeaderVisible",
                   typeof(string),
                   SettingsSerializeAs.Xml,
                   null) as string;
            }
        }

        /// <summary>
        /// Видимость кнопок добавления записей
        /// </summary>
        private string OriginalAddButtonVisible
        {
            get
            {
                return LoadValue("AddButtonVisible",
                                 typeof (string),
                                 SettingsSerializeAs.Xml,
                                 null) as string;
            }
        }

        /// <summary>
        /// Фильтры в пользовательских настройках
        /// </summary>
        private ColumnFilter[] OriginalColumnFillters
        {
            get
            {
                return LoadValue(
                   "ColumnFillers",
                   typeof(ColumnFilter[]),
                   SettingsSerializeAs.Binary,
                   null) as ColumnFilter[];
            }
        } // OriginalColumnFillters

        #endregion


        public override void Load()
        {            
            ColumnFilter[] fillters = OriginalColumnFillters;
            UltraGridColumnSetting[] columnSettings = OriginalColumnSettings;
            
            // Восстанавливаем сохраненные настройки фильтров, сортировки и выделение строк
            grid.BeginUpdate();
            try
            {
                foreach (UltraGridBand ultraGridBand in grid.DisplayLayout.Bands)
                {
                    if (IsSetOptions(SaveOptions.ColumnSettings))
                    {
                        // Восстанавливаем настройки колонок
                        if (columnSettings != null)
                        {
                            ultraGridBand.SortedColumns.Clear();
                            foreach (UltraGridColumnSetting columnSetting in columnSettings)
                            {
                                if (ultraGridBand.Columns.Exists(columnSetting.Name))
                                {
                                    UltraGridColumn ultraGridColumn =
                                        ultraGridBand.Columns[columnSetting.Name];
                                    if (ultraGridColumn == null)
                                    {
                                        continue;
                                    }

                                    ultraGridBand.Columns[columnSetting.Name].Header.VisiblePosition =
                                        columnSetting.DisplayIndex;
                                    ultraGridBand.Columns[columnSetting.Name].Width = columnSetting.Width;
                                    ultraGridBand.Columns[columnSetting.Name].Hidden =
                                        columnSetting.Hidden;
                                    if (columnSetting.IsGroupByColumn)
                                    {
                                        ultraGridBand.SortedColumns.Add(columnSetting.Name, false, true);
                                    }
                                    if (columnSetting.SortIndicator == SortIndicator.Ascending ||
                                        columnSetting.SortIndicator == SortIndicator.Descending)
                                    {
                                        ultraGridBand.SortedColumns.Add(
                                            ultraGridBand.Columns[columnSetting.Name],
                                            columnSetting.SortIndicator == SortIndicator.Descending,
                                            columnSetting.IsGroupByColumn);
                                    }
                                    ultraGridBand.Columns[columnSetting.Name].CellMultiLine =
                                        columnSetting.CellMultiline;

                                    if (ultraGridColumn.DataType == typeof (String))
                                    {
                                        ultraGridColumn.Header.Tag = columnSetting.CellMultiline ==
                                                                     DefaultableBoolean.True
                                                                         ? System.Windows.Forms.CheckState.Checked
                                                                         : System.Windows.Forms.CheckState.Unchecked;
                                    }

                                    UIElement header = ultraGridColumn.Header.GetUIElement();
                                    if (header != null)
                                    {
                                        foreach (UIElement element in header.ChildElements)
                                        {
                                            CheckBoxUIElement cbWrapWordsUIElement = element as CheckBoxUIElement;
                                            if (cbWrapWordsUIElement != null)
                                            {
                                                cbWrapWordsUIElement.CheckState = columnSetting.CellMultiline ==
                                                                                  DefaultableBoolean.True
                                                                                      ? System.Windows.Forms.CheckState.
                                                                                            Checked
                                                                                      : System.Windows.Forms.CheckState.
                                                                                            Unchecked;
                                                ultraGridColumn.Header.Tag = cbWrapWordsUIElement.CheckState;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (IsSetOptions(SaveOptions.ColumnFillters))
                    {
                        // Восстанавливаем фильтры
                        if (fillters != null)
                        {
                            for (int i = 0; i < fillters.GetLength(0); i++)
                            {
                                if (fillters[i] != null)
                                {
                                    // убираем сохраниние фильтра для поля ParentID, мешает при переходе плоский\иерархический режим
                                    if (fillters[i].Column != null && String.Equals(fillters[i].Column.Header.Caption.ToLower(), "parentid"))
                                    {
                                        fillters[i].FilterConditions.Clear();
                                        break;
                                    }

                                    foreach (FilterCondition o in fillters[i].FilterConditions)
                                    {
                                        if (!ultraGridBand.ColumnFilters[i].FilterConditions.Contains(o))
                                            ultraGridBand.ColumnFilters[i].FilterConditions.Add(o);
                                    }
                                }
                            }
                        }
                    }

                    string groupHeaderVisible = OriginalGroupHeaderVisible;

                    if (IsSetOptions(SaveOptions.GroupHeaderVisible))
                    {
                        // восстанавливаем область видимости группировки
                        if (!String.IsNullOrEmpty(groupHeaderVisible))
                            grid.DisplayLayout.GroupByBox.Hidden = Convert.ToBoolean(OriginalGroupHeaderVisible);
                    }

                    string addHeaderVisible = OriginalAddButtonVisible;

                    if (IsSetOptions(SaveOptions.AddButtonVisible))
                    {
                        // восстанавливаем область видимости группировки
                        if (!String.IsNullOrEmpty(addHeaderVisible))
                            grid.DisplayLayout.AddNewBox.Hidden = Convert.ToBoolean(OriginalAddButtonVisible);
                    }
                }
            }
            catch (Exception e)
            {
                if (ThrowOnErrorLoading)
                {
                    throw;
                }
            }
            finally
            {
                grid.EndUpdate();
            }
        } // Load

        // -------------------------------------------------------------------------
        public override void Save()
        {
            // Сохраняем видимость и позицию колонок (только для первого банда)
            try
            {
                if (HasChanged)
                {
                    if (IsSetOptions(SaveOptions.ColumnSettings))
                    {
                        UltraGridColumnSetting[] columnSettings = ColumnSettings;

                        SaveValue(
                            "ColumnSettings",
                            typeof(UltraGridColumnSetting[]),
                            SettingsSerializeAs.Binary,
                            columnSettings,
                            null);
                    }

                    if (IsSetOptions(SaveOptions.ColumnFillters))
                    {
                        ColumnFilter[] columnFillers = new ColumnFilter[grid.DisplayLayout.Bands[0].ColumnFilters.Count];
                        grid.DisplayLayout.Bands[0].ColumnFilters.CopyTo(columnFillers, 0);

                        SaveValue(
                            "ColumnFillers",
                            typeof(ColumnFilter[]),
                            SettingsSerializeAs.Binary,
                            columnFillers,
                            null);
                    }

                    if (IsSetOptions(SaveOptions.GroupHeaderVisible))
                    {
                        SaveValue(
                            "GroupHeaderVisible",
                            typeof(string),
                            SettingsSerializeAs.Xml,
                            grid.DisplayLayout.GroupByBox.Hidden.ToString(),
                            null);
                    }

                    if (IsSetOptions(SaveOptions.AddButtonVisible))
                    {
                        SaveValue(
                            "AddButtonVisible",
                            typeof(string),
                            SettingsSerializeAs.Xml,
                            grid.DisplayLayout.AddNewBox.Hidden.ToString(),
                            null);
                    }
                }
            }
            catch (Exception e)
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
            
        } // Save

        public override string ToString()
        {
            return String.Concat(name, " (UltraGrid)");
        }
    }    
}
