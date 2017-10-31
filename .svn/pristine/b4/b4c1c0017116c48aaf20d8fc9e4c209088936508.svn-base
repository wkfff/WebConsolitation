using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.DataWarning;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations
{
    public class BaseCapitalOperationsUI : BaseViewObj
    {
        public BaseCapitalOperationsUI(string key)
            : base(key)
        {
        }

        protected override void SetViewCtrl()
        {

        }

        protected CapitalOperationsServer CapitalOperationsServer
        {
            get;
            set;
        }

        protected string CurrentCalculationCaption
        {
            get;
            set;
        }

        protected Dictionary<string, DataWarningNotifier> WarningList
        {
            get;
            set;
        }

        protected List<string> InvalidDataEditors
        {
            get; set;
        }

        public override void Initialize()
        {
            base.Initialize();
            CapitalOperationsServer = new CapitalOperationsServer(Workplace.ActiveScheme);
            WarningList = new Dictionary<string, DataWarningNotifier>();
            InvalidDataEditors = new List<string>();
        }

        public bool CheckCalculatePermission()
        {
            bool calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning",
                (int)FinSourcePlaningOperations.Calculate, false);
            if (!calcVisible)
                calcVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("CapitalOperations",
                (int)FinSourcePlaningCalculateUIModuleOperations.Calculate, false);
            return calcVisible;
        }

        protected void BurnEditor(bool burn, UltraNumericEditor editor)
        {
            if (!burn)
            {
                editor.Appearance.ResetBackColor();
                editor.Appearance.ResetBackColor2();
                editor.Appearance.BackGradientStyle = GradientStyle.None;
            }
            else
            {
                editor.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
                editor.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Orange);
                editor.Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
        }

        protected void BurnGridCellWarning(bool burn, UltraGridCell cell)
        {
            if (!burn)
            {
                cell.Appearance.ResetBackColor();
                cell.Appearance.ResetBackColor2();
                cell.Appearance.BackGradientStyle = GradientStyle.None;
            }
            else
            {
                cell.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
                cell.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Orange);
                cell.Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
        }

        protected void GetCalculationUniqueParams(string dateName, ref DateTime date, ref string name)
        {
            int dateIndex = dateName.Split('_').Length;
            string dateStr = dateName.Split('_')[dateIndex - 1];
            name = dateName.Replace(dateStr, string.Empty).TrimEnd('_');
            date = Convert.ToDateTime(dateStr);
        }

        protected void CheckMaxEditorValue(UltraNumericEditor numericEditor)
        {
            CheckMaxEditorValue(numericEditor, Convert.ToDecimal(999999999999));
        }

        protected void CheckMaxEditorValue(UltraNumericEditor numericEditor, decimal maxValue)
        {
            if (WarningList.ContainsKey(numericEditor.Name))
                return;
            if (numericEditor.ReadOnly)
                return;

            if (numericEditor.Value != null)
            {
                decimal editorValue = Convert.ToDecimal(numericEditor.Value);
                if (editorValue > Convert.ToDecimal(maxValue))
                    WarningList.Add(numericEditor.Name, new DataWarningNotifier(numericEditor, "Исходные параметры введены некорректно. Рассчитанные на их основе показатели выходят за рамки допустимых значений"));
            }
        }

        protected void CleanWarnings()
        {
            foreach (var warning in WarningList.Values)
            {
                warning.Hide();
            }
            WarningList.Clear();
        }

        private ToolTip toolTipValue = null;

        public ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new ToolTip(fViewCtrl);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        protected void Grid_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        protected void Grid_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            decimal maxValue = editor.Tag != null ? Convert.ToDecimal(editor.Tag) : Convert.ToDecimal(999999999999);

            if (editor.Value != null)
            {
                decimal editorValue = Math.Abs(Convert.ToDecimal(editor.Value));
                if (editorValue <= maxValue)
                {
                    return;
                }
            }

            ToolTip.ToolTipText = "Исходные параметры введены некорректно. Рассчитанные на их основе показатели выходят за рамки допустимых значений";
            var tooltipPos = new Point(editor.UIElement.Rect.Left, editor.UIElement.Rect.Bottom);
            ToolTip.Show(editor.PointToScreen(tooltipPos));
        }

        protected void Editor_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        protected void Editor_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            decimal maxValue = editor.Tag != null ? Convert.ToDecimal(editor.Tag) : Convert.ToDecimal(999999999999);

            if (editor.Value != null)
            {
                decimal editorValue = Math.Abs(Convert.ToDecimal(editor.Value));
                if (editorValue <= maxValue)
                {
                    return;
                }
            }
            
            ToolTip.ToolTipText = "Исходные параметры введены некорректно. Рассчитанные на их основе показатели выходят за рамки допустимых значений";
            var tooltipPos = new Point(editor.UIElement.Rect.Left, editor.UIElement.Rect.Bottom);
            ToolTip.Show(editor.PointToScreen(tooltipPos));
        }

        protected void Editor_ValueChanged(object sender, EventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;

            if (editor.Value == null || editor.Value == DBNull.Value)
                editor.Value = 0;

            decimal maxValue = editor.Tag != null ? Convert.ToDecimal(editor.Tag) : Convert.ToDecimal(999999999999);

            if (editor.Value != null)
            {
                decimal editorValue = Math.Abs(Convert.ToDecimal(editor.Value));
                if (editorValue <= maxValue)
                {
                    BurnEditor(false, editor);
                    if (InvalidDataEditors.Contains(editor.Name))
                        InvalidDataEditors.Remove(editor.Name);
                    return;
                }
                if (!InvalidDataEditors.Contains(editor.Name))
                    InvalidDataEditors.Add(editor.Name);
                BurnEditor(true, editor);
            }
        }

        protected void SetEditorCheck(UltraNumericEditor editor)
        {
            editor.ValueChanged += Editor_ValueChanged;
            editor.MouseEnterElement += Editor_MouseEnterElement;
            editor.MouseLeave += Editor_MouseLeave;
        }

        protected int DaysInAYear(int year)
        {
            int days = 0;
            for (int i = 1; i <= 12; i++)
            {
                days += DateTime.DaysInMonth(year, i);
            }
            return days;
        }
    }
}
