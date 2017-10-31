using System;
using System.Collections.Generic;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

namespace Krista.FM.Client.Components
{
    #region StateButtonsManager
    internal class StateButtonsManager
    {
        private List<StateButtonTool> Buttons;
        private PopupMenuTool ParentMenu;

        internal StateButtonsManager(PopupMenuTool parentMenu)
        {
            if (parentMenu == null) throw new Exception("Родительский элемент не может быть пуст");
            Buttons = new List<StateButtonTool>();
            clmnsType = new List<UltraGridEx.ColumnType>();
            ParentMenu = parentMenu;
        }

        internal void ClearButtons()
        {
            if (Buttons.Count == 0) return;
            ParentMenu.ToolbarsManager.BeginUpdate();
            try
            {
                foreach (StateButtonTool pmt in Buttons)
                {
                    ParentMenu.ToolbarsManager.Tools.Remove(pmt);
                }
                Buttons.Clear();
                clmnsType.Clear();
            }
            finally
            {
                ParentMenu.ToolbarsManager.EndUpdate();
            }
        }

        internal void AttachButtons()
        {
            ParentMenu.ToolbarsManager.BeginUpdate();
            try
            {
                for (int i = 0; i <= Buttons.Count - 1; i++)
                {
                    switch (clmnsType[i])
                    {
                        case UltraGridEx.ColumnType.unknown:
                            ParentMenu.ToolbarsManager.Tools.Add(Buttons[i]);
                            ParentMenu.Tools.AddTool(Buttons[i].Key);
                            break;
                        case UltraGridEx.ColumnType.Standart:
                            ParentMenu.ToolbarsManager.Tools.Add(Buttons[i]);
                            ParentMenu.Tools.AddTool(Buttons[i].Key);
                            break;
                        case UltraGridEx.ColumnType.Service:
                            PopupMenuTool ServiceMenu;
                            if (ParentMenu.Tools.Exists("ServiceMenu"))
                                ServiceMenu = (PopupMenuTool)ParentMenu.Tools["ServiceMenu"];
                            else
                            {
                                ServiceMenu = new PopupMenuTool("ServiceMenu");
                                ServiceMenu.SharedProps.Caption = "Служебные поля";
                                ServiceMenu.DropDownArrowStyle = DropDownArrowStyle.SegmentedStateButton;
                                ServiceMenu.SharedProps.AppearancesSmall.PressedAppearance.Image = Properties.Resources.CheckMenu;
                                ParentMenu.ToolbarsManager.Tools.Add(ServiceMenu);
                                ParentMenu.Tools.AddTool("ServiceMenu");
                            }
                            ServiceMenu.ToolbarsManager.Tools.Add(Buttons[i]);
                            ServiceMenu.Tools.AddTool(Buttons[i].Key);
                            break;
                        case UltraGridEx.ColumnType.System:
                            PopupMenuTool SystemMenu;
                            if (ParentMenu.Tools.Exists("SystemMenu"))
                                SystemMenu = (PopupMenuTool)ParentMenu.Tools["SystemMenu"];
                            else
                            {
                                SystemMenu = new PopupMenuTool("SystemMenu");
                                SystemMenu.SharedProps.Caption = "Системные поля";
                                SystemMenu.DropDownArrowStyle = DropDownArrowStyle.SegmentedStateButton;

                                SystemMenu.SharedProps.AppearancesSmall.PressedAppearance.Image = Properties.Resources.CheckMenu;
                                ParentMenu.ToolbarsManager.Tools.Add(SystemMenu);
                                ParentMenu.Tools.AddTool("SystemMenu");
                            }
                            SystemMenu.ToolbarsManager.Tools.Add(Buttons[i]);
                            SystemMenu.Tools.AddTool(Buttons[i].Key);
                            break;
                    }
                }
            }
            finally
            {
                ParentMenu.ToolbarsManager.EndUpdate();
            }
        }

        private List<UltraGridEx.ColumnType> clmnsType;

        internal void AddButton(string Key, string Text, bool Checked, bool Visible, UltraGridEx.ColumnType Type)
        {
            StateButtonTool btn = new StateButtonTool(Key);
            btn.SharedProps.Caption = Text;
            btn.Checked = Checked;
            btn.SharedProps.Visible = Visible;
            btn.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
            Buttons.Add(btn);
            clmnsType.Add(Type);
        }
    }
    #endregion
}