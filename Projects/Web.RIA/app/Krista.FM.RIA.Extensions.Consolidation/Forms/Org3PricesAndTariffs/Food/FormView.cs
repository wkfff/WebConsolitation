namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Food
{
    public class FormView : PricesAndTariffsView
    {
        public FormView(ITaskService taskService, IUserSessionState userSessionState)
            : base(GoodType.Food, taskService, userSessionState)
        {
        }
    }
}