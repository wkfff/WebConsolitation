using System;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CalculateCurrencyDiffCommand : AbstractCommand
    {
        private string currencyDiffDetailKey;

        public CalculateCurrencyDiffCommand(string detailKey)
        {
            key = "CalculateCurrencyDiffCommand";
            caption = "Расчет курсовой разницы";
            currencyDiffDetailKey = detailKey;
        }

        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow != null)
            {
                if (activeRow.IsNull("EndDate"))
                {
                    MessageBox.Show("Поле \"Дата окончательного погашения кредита\" не заполнено. Заполните это поле и повторите попытку", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // проверим, есть ли курсы валюты в справочнике курсов
                using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    int refOKV = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["RefOKV"].Value);
                    string query = String.Format("select ExchangeRate from d_S_ExchangeRate where RefOKV = {0}",
                        refOKV);
                    DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                            string.Format("Справочник «Курсы валют» для валюты '{0}' не заполнен, заполните справочник и повторите попытку", Utils.GetCurrencyName(refOKV)),
                            "Расчет курсовой разницы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                CalculateCurrencyDiffWizard wizard = new CalculateCurrencyDiffWizard(planningServer, vo.ugeCls.ugData.ActiveRow);
                wizard.ShowDialog();
                content.RefreshDetail(currencyDiffDetailKey);
            }
        }
    }
}
