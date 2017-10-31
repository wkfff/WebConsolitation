using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Множество областей грида
    /// </summary>
    public struct AreaSet
    {
        /// <summary>
        /// Все области
        /// </summary>
        static public GridArea[] All = new GridArea[] 
        {
            GridArea.FiltersCaption,
            GridArea.ColumnsCaption,
            GridArea.RowsCaption,
            GridArea.MeasuresCaption,
            GridArea.Columns,
            GridArea.Rows,
            GridArea.MeasuresData
        };

        /// <summary>
        /// Область всех заголовков
        /// </summary>
        static public GridArea[] AllCaptions = new GridArea[]
        { 
            GridArea.FiltersCaption,
            GridArea.ColumnsCaption,
            GridArea.RowsCaption,
            GridArea.MeasuresCaption
        };

        /// <summary>
        /// Область осей
        /// </summary>
        static public GridArea[] AllAxis = new GridArea[]
        { 
            GridArea.Columns,
            GridArea.Rows
        };

        /// <summary>
        /// Если в areas содержиться area, вернет true
        /// </summary>
        /// <param name="areas">Множество областей</param>
        /// <param name="area">Объект поиска</param>
        /// <returns>результат</returns>
        static public bool IsIntersect(GridArea[] areas, GridArea area)
        {
            foreach (GridArea currentArea in areas)
            {
                if (currentArea == area)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
