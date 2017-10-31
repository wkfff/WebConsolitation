using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Workplace.Services;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
	/// <summary>
	/// Журнал ставок рефинансирования ЦБ.
	/// </summary>
    internal class JournalCBDataClsUI : ReferenceUI
    {
        public JournalCBDataClsUI(IEntity entity)
            : base(entity)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            #region Инициализация панели инструментов
            UltraToolbar utbFinSourcePlanning = new UltraToolbar("FinSourcePlanning");
            utbFinSourcePlanning.DockedColumn = 0;
            utbFinSourcePlanning.DockedRow = 0;
            utbFinSourcePlanning.Text = "FinSourcePlanning";
            utbFinSourcePlanning.Visible = true;
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbFinSourcePlanning });

			CommandService.AttachToolbarTool(new CalcPercentsForCurrentRateCommand(), utbFinSourcePlanning);
			CommandService.AttachToolbarTool(new CalcPercentsForCreditIncomesCommand(), utbFinSourcePlanning);
			CommandService.AttachToolbarTool(new RequestRefinancingRateCommand(), utbFinSourcePlanning);
			
            #endregion Инициализация панели инструментов
        }
    }
}
