using System;
using System.Data;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.Wizards;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    /// <summary>
    /// команда для заполнения плана привлечения в гарантиях
    /// </summary>
    internal class FillAttractPlanCommand : AbstractCommand
    {
        public FillAttractPlanCommand()
        {
            key = "CalcGuaranteeAttractPlanCommand";
            caption = "План привлечения";
        }

        public override void Run()
        {
            GuaranteeUI content = (GuaranteeUI)WorkplaceSingleton.Workplace.ActiveContent;

            // 
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow == null)
                return;
            if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(guaranteeRow["ID"])))
            {
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                    "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Заполнение плана привлечения";
            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(guaranteeRow);
                GuaranteeServer grntSvr = GuaranteeServer.GetGuaranteeServer(Convert.ToInt32(principalContractRow["RefOKV"]), FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                grntSvr.FillAttractPlan(new Guarantee(content.GetActiveDataRow()),
                                        FinSourcePlanningNavigation.BaseYear,
                                        FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                                "План привлечения успешно заполнен", "Гарантии", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PlanAttractPrGrnt_Key);
            }
            catch(Exception e)
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, e.Message, "Гарантии",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    class CalcObligationExecutionPlanCommand : AbstractCommand
    {
        public CalcObligationExecutionPlanCommand()
        {
            key = "ObligationExecutionPlanCommand";
            caption = "План исполнения обязательств";
        }

        public override void Run()
        {
            GuaranteeUI content = (GuaranteeUI)WorkplaceSingleton.Workplace.ActiveContent;
            try
            {
                DataRow guaranteeRow = content.GetActiveDataRow();
                if (guaranteeRow != null)
                {
                    GuaranteeServer grntSvr = GuaranteeServer.GetGuaranteeServer(Convert.ToInt32(guaranteeRow["RefOKV"]), FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                    grntSvr.CalcObligationExecutionPlan(new Guarantee(content.GetActiveDataRow()), FinSourcePlanningNavigation.BaseYear);
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "План исполнения по гарантии успешно заполнен", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PlanAttractGrnt_Key);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, e.Message, "Гарантии",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class FillPlanDebtCommand : AbstractCommand
    {
        public FillPlanDebtCommand()
        {
            key = "FillPlanDebtCommand";
            caption = "План погашения основного долга";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow != null)
            {
                if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(activeRow["ID"])))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GuaranteeMainPlanWizard wizard = new GuaranteeMainPlanWizard(activeRow);
                wizard.ShowDialog();
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PlanDebtPrGrnt_Key);
            }
        }
    }

    public class FillPlanServiceCommand : AbstractCommand
    {
        public FillPlanServiceCommand()
        {
            key = "FillPlanServiceCommand";
            caption = "План обслуживания долга";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow != null)
            {
                int activeId = Convert.ToInt32(activeRow["ID"]);
                if (!GuaranteeServer.PrincipalContractFilled(activeId))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GuaranteePercentsWizard wizard = new GuaranteePercentsWizard(PayPeriodicity.Other);
                wizard.ShowDialog();
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PlanServicePrGrnt_Key);
            }
        }
    }

    public class CalcGuaranteePercentCommand : AbstractCommand
    {
        public CalcGuaranteePercentCommand()
        {
            key = "btnCalcPercent";
            caption = "Расчет ставок процентов";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow != null)
            {
                try
                {
                    if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(activeRow["ID"])))
                    {
                        MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                            "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Расчет ставок процентов";

                    DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(activeRow);
                    if (principalContractRow != null)
                    {
                        DataRow[] dataRows = new DataRow[1];
                        dataRows[0] = principalContractRow.Table.NewRow();
                        dataRows[0]["ID"] = principalContractRow["ID"];
                        dataRows[0]["StartDate"] = principalContractRow["StartDate"];
                        dataRows[0]["EndDate"] = principalContractRow["EndDate"];
                        dataRows[0]["PenaltyDebtRate"] = principalContractRow["PenaltyDebtRate"];
                        dataRows[0]["PenaltyPercentRate"] = principalContractRow["PenaltyPercentRate"];
                        dataRows[0]["CreditPercent"] = principalContractRow["CreditPercent"];

                        GuaranteeServer server = GuaranteeServer.GetGuaranteeServer(Convert.ToInt32(principalContractRow["RefOKV"]), FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);

                        server.CalcPercents(dataRows, Convert.ToInt32(activeRow["ID"]), -1, true);
                    }
                    content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_JournalPercentGrnt_Key);
                }
                catch (FinSourcePlanningException ex)
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                }
            }
        }
    }

    public class CalculatePercentPenaltyCommand : AbstractCommand
    {
        public CalculatePercentPenaltyCommand()
        {
            key = "CalculatePercentPenaltyCommand";
            caption = "Расчет пени по процентам";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(guaranteeRow["ID"])))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GuaranteeServer server = GuaranteeServer.GetGuaranteeServer(-1, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                server.CalculatePenaltiesPlanService(new DataRow[] {guaranteeRow}, FinSourcePlanningNavigation.BaseYear);
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PrGrntChargePenaltyPercent_Key);
            }
        }
    }

    public class CalculateDebtPenaltyCommand : AbstractCommand
    {
        public CalculateDebtPenaltyCommand()
        {
            key = "CalculateDebtPenaltyCommand";
            caption = "Расчет пени по основному долгу";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                GuaranteeServer server = GuaranteeServer.GetGuaranteeServer(-1, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(guaranteeRow["ID"])))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                server.CalculatePenaltiesMainDebt(new DataRow[] { guaranteeRow }, FinSourcePlanningNavigation.BaseYear);
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_ChargePenaltyDebtPrGrnt_Key);
            }
        }
    }

    public class GuaranteeDebtPenaltyWizardCommand : AbstractCommand
    {
        public GuaranteeDebtPenaltyWizardCommand()
        {
            key = "GuaranteeDebtPenaltyWizardCommand";
            caption = "Мастер расчета пени по основному долгу";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(guaranteeRow["ID"])))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GuaranteePenaltyWizard wizard = new GuaranteeDebtPenaltyWizard(guaranteeRow);
                wizard.ShowDialog();
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_ChargePenaltyDebtPrGrnt_Key);
            }
        }
    }

    public class GuaranteePercentPenaltyWizardCommand : AbstractCommand
    {
        public GuaranteePercentPenaltyWizardCommand()
        {
            key = "GuaranteePercentPenaltyWizardCommand";
            caption = "Мастер расчета пени по процентам";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                if (!GuaranteeServer.PrincipalContractFilled(Convert.ToInt32(guaranteeRow["ID"])))
                {
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Деталь 'Гарантируемый договор' не заполнена", "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GuaranteePenaltyWizard wizard = new GuaranteePercentPenaltyWizard(guaranteeRow);
                wizard.ShowDialog();
                content.RefreshDetail(GuaranteeIssuedObjectKeys.t_S_PrGrntChargePenaltyPercent_Key);
            }
        }
    }

    public class GuaranteeVariantTransferCommand : AbstractCommand
    {
        public GuaranteeVariantTransferCommand()
        {
            key = "VariantTransfer";
            caption = "Перевод договора из одного варианта в другой";
        }

        public override void  Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                // спрашиваем, пер
                int variantID = Convert.ToInt32(guaranteeRow["RefVariant"]);
                string variantCaption = "действующие договора";
                string question = "Договор будет перенесен в действующие договора. Подолжить?";
                if (variantID == 0)
                {
                    variantCaption = "архив";
                    question = "Договор будет перенесен в архив. Подолжить?";
                }

                if (MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                   question, "Гарантии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                // переводим в вариант в зависимости от текущего состояния и варианта
                IEntity entity =
                    WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Guarantissued_Key);
                Utils.SetNewVariant(guaranteeRow, entity, WorkplaceSingleton.Workplace.ActiveScheme);
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                    string.Format("Договор переведен в {0}", variantCaption), "Гарантии", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
