using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinMaskedEdit;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public struct CommonMethods
    {
        /// <summary>
        /// поиск по параметру записи в гриде, сортировка грида по колонке
        /// </summary>
        /// <param name="grid">грид</param>
        /// <param name="findAttrClmn">колонка, по которой ищем запись</param>
        /// <param name="sortColumnName">колонка, по которой сортируем</param>
        /// <param name="moduleParams">параметр по которому ищем запись</param>
        public static bool SelectRow(UltraGrid grid, string findAttrClmn, string sortColumnName, params object[] moduleParams)
        {
            grid.DisplayLayout.Bands[0].Columns[sortColumnName].SortIndicator = SortIndicator.Ascending;
            if (moduleParams != null)
            {
                UltraGridRow row = UltraGridHelper.FindGridRow(grid, findAttrClmn, moduleParams[0].ToString());
                if (row != null)
                {
                    row.Activate();
                    //row.Selected = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// –азыменовка семантической принадлежности объекта
        /// </summary>
        /// <param name="cmnObj">объект</param>
        /// <returns>русское название семантической принадлежности</returns>
        public static string GetDataObjSemanticRus(ISemanticsCollection semantics, IEntity cmnObj)
        {
            string semanticText;
            if (semantics.TryGetValue(cmnObj.Semantic, out semanticText))
                return semanticText;
            else
                return cmnObj.Semantic;
        }

        public static void HideUnusedButtons(UltraToolbarsManager toolBarManager)
        {
            // пр€чем все ненужные кнопки тулбаров
            toolBarManager.Toolbars["utbImportExport"].Visible = false;
            foreach (ToolBase tool in toolBarManager.Tools)
            {
                if (tool.Key != "Refresh" && tool.Key != "ColumnsVisible")
                    tool.SharedProps.Visible = false;
            }
            // показываем все нужные, которые были спр€таны
            PopupMenuTool menu = (PopupMenuTool)toolBarManager.Tools["ColumnsVisible"];
            foreach (ToolBase tool in menu.Tools)
            {
                if (tool.SharedProps.Caption != string.Empty)
                    tool.SharedProps.Visible = true;
            }
            toolBarManager.Tools["Refresh"].SharedProps.ToolTipText = "ќбновить список";

            toolBarManager.ToolbarSettings.AllowFloating = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.AllowHiding = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.AllowDockBottom = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.AllowDockLeft = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.AllowDockRight = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.AllowDockTop = DefaultableBoolean.False;
            toolBarManager.ToolbarSettings.GrabHandleStyle = GrabHandleStyle.None;
        }
    }
}
