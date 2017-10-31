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
        /// ����� �� ��������� ������ � �����, ���������� ����� �� �������
        /// </summary>
        /// <param name="grid">����</param>
        /// <param name="findAttrClmn">�������, �� ������� ���� ������</param>
        /// <param name="sortColumnName">�������, �� ������� ���������</param>
        /// <param name="moduleParams">�������� �� �������� ���� ������</param>
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
        /// ����������� ������������� �������������� �������
        /// </summary>
        /// <param name="cmnObj">������</param>
        /// <returns>������� �������� ������������� ��������������</returns>
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
            // ������ ��� �������� ������ ��������
            toolBarManager.Toolbars["utbImportExport"].Visible = false;
            foreach (ToolBase tool in toolBarManager.Tools)
            {
                if (tool.Key != "Refresh" && tool.Key != "ColumnsVisible")
                    tool.SharedProps.Visible = false;
            }
            // ���������� ��� ������, ������� ���� ��������
            PopupMenuTool menu = (PopupMenuTool)toolBarManager.Tools["ColumnsVisible"];
            foreach (ToolBase tool in menu.Tools)
            {
                if (tool.SharedProps.Caption != string.Empty)
                    tool.SharedProps.Visible = true;
            }
            toolBarManager.Tools["Refresh"].SharedProps.ToolTipText = "�������� ������";

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
