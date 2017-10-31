using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using System.Text;
using System;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    /// <summary>
    /// Начисление пени по процентам.
    /// </summary>
    public class CalculatePenaltiesForPercentCommand : AbstractCommand
    {
        protected string detailKey;

        protected CalculatePenaltiesForPercentCommand()
        {
            key = "CalculatePenaltiesForPercentCommand";
            caption = "Начисление пени по процентам";
        }

        public override void Run()
        {
            try
            {
                // проверим, можно ли считать пени по договору
                if (!((BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent).CanCalculatePenalties)
                {
                    MessageBox.Show(
                        "Пени по процентам начисляются только для принятых договоров со статусом «Действующий»",
                        "Расчет пени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";
                BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;

                DataRow row = content.GetActiveDataRow();
                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                Dictionary<string, string> errors = planningServer.CalculatePenaltiesPlanService(new DataRow[] { row }, FinSourcePlanningNavigation.BaseYear);

                if (errors.Count > 0)
                {
                    // TODO: вывести список ошибок
                    StringBuilder errorList = new StringBuilder();
                    foreach (KeyValuePair<string, string> item in errors)
                    {
                        errorList.AppendFormat("№ кредита - {0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                    MessageBox.Show(
                        String.Format("Для {0} кредитов(а) расчет не выполнен:{1}{2}",
                        errors.Count, Environment.NewLine, errorList),
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    content.RefreshDetail(detailKey);
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

    public class CalculatePenaltiesPercentCreditIncomesCommand : CalculatePenaltiesForPercentCommand
    {
        public CalculatePenaltiesPercentCreditIncomesCommand()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key;
        }
    }

    public class CalculatePenaltiesPercentCreditIssuedCommand : CalculatePenaltiesForPercentCommand
    {
        public CalculatePenaltiesPercentCreditIssuedCommand()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO;
        }
    }


    /// <summary>
    /// Начисление пени по процентам по всем договорам.
    /// </summary>
    public class CalculatePenaltiesForPercentForAllCommand : AbstractCommand
    {
        protected string detailKey;

        protected CalculatePenaltiesForPercentForAllCommand()
        {
            key = "CalculatePenaltiesForPercentForAllCommand";
            caption = "Начисление пени по процентам по всем договорам";
        }

        public override void Run()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";

                BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
                BaseClsView vo = (BaseClsView)content.ViewCtrl;
                DataRow[] rows = ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select();

                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                Dictionary<string, string> errors = planningServer.CalculatePenaltiesPlanService(rows, FinSourcePlanningNavigation.BaseYear);

                if (errors.Count > 0)
                {
                    // TODO: вывести список ошибок
                    StringBuilder errorList = new StringBuilder();
                    foreach (KeyValuePair<string, string> item in errors)
                    {
                        errorList.AppendFormat("№ кредита - {0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                    MessageBox.Show(
                        String.Format("Для {0} кредитов(а) расчет не выполнен:{1}{2}",
                        errors.Count, Environment.NewLine, errorList),
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    content.RefreshDetail(detailKey);
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

    public class CalculatePenaltiesPercentForAllCommandCI : CalculatePenaltiesForPercentForAllCommand
    {
        public CalculatePenaltiesPercentForAllCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key;
        }
    }

    public class CalculatePenaltiesPercentForAllCommandCO : CalculatePenaltiesForPercentForAllCommand
    {
        public CalculatePenaltiesPercentForAllCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO;
        }
    }

    /// <summary>
    /// Начисление пени по основному долгу.
    /// </summary>
    public class CalculatePenaltiesMainDebtCommand : AbstractCommand
    {
        protected string detailKey;

        protected CalculatePenaltiesMainDebtCommand()
        {
            key = "CalculatePenaltiesMainDebtCommand";
            caption = "Начисление пени по основному долгу";
        }

        public override void Run()
        {
            try
            {
                // проверим, можно ли считать пени по договору
                if (!((BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent).CanCalculatePenalties)
                {
                    MessageBox.Show(
                        "Пени по процентам начисляются только для принятых договоров со статусом «Действующий»",
                        "Расчет пени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();

                BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
                BaseClsView vo = (BaseClsView)content.ViewCtrl;
                int activeRowID = Convert.ToInt32(vo.ugeCls._ugData.ActiveRow.Cells["ID"].Value);
                DataRow[] rows = ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select(String.Format("ID = {0}", activeRowID));
                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                Dictionary<string, string> errors = planningServer.CalculatePenaltiesMainDebt(rows, FinSourcePlanningNavigation.BaseYear);

                if (errors.Count > 0)
                {
                    // TODO: вывести список ошибок
                    StringBuilder errorList = new StringBuilder();
                    foreach (KeyValuePair<string, string> item in errors)
                    {
                        errorList.AppendFormat("№ кредита - {0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(
                        String.Format("Для {0} кредитов(а) расчет не выполнен:{1}{2}",
                        errors.Count, Environment.NewLine, errorList),
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    content.RefreshDetail(detailKey);
                }
            }
            catch (FinSourcePlanningException ex)
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CalculatePenaltiesDebtCommandCI : CalculatePenaltiesMainDebtCommand
    {
        public CalculatePenaltiesDebtCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key;
        }
    }

    public class CalculatePenaltiesDebtCommandCO : CalculatePenaltiesMainDebtCommand
    {
        public CalculatePenaltiesDebtCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO;
        }
    }


    /// <summary>
    /// Начисление пени по основному долгу по всем договорам.
    /// </summary>
    public class CalculatePenaltiesMainDebtForAllCommand : AbstractCommand
    {
        protected string detailKey;

        protected CalculatePenaltiesMainDebtForAllCommand()
        {
            key = "CalculatePenaltiesMainDebtForAllCommand";
            caption = "Начисление пени по основному долгу по всем договорам";
        }

        public override void Run()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";

                BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
                BaseClsView vo = (BaseClsView)content.ViewCtrl;
                DataRow[] rows = ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select();
                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                Dictionary<string, string> errors = planningServer.CalculatePenaltiesMainDebt(rows, FinSourcePlanningNavigation.BaseYear);

                if (((BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent).GetDetailVisible().ObjectKey == SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key)
                    ((BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent).RefreshDetail();

                if (errors.Count > 0)
                {
                    // TODO: вывести список ошибок
                    StringBuilder errorList = new StringBuilder();
                    foreach (KeyValuePair<string, string> item in errors)
                    {
                        errorList.AppendFormat("№ кредита - {0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(
                        String.Format("Для {0} кредитов(а) расчет не выполнен:{1}{2}",
                        errors.Count, Environment.NewLine, errorList),
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    content.RefreshDetail(detailKey);
                }
            }
            catch (FinSourcePlanningException ex)
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CalculatePenaltiesDebtForAllCommandCI : CalculatePenaltiesMainDebtForAllCommand
    {
        public CalculatePenaltiesDebtForAllCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key;
        }
    }

    public class CalculatePenaltiesDebtForAllCommandCO : CalculatePenaltiesMainDebtForAllCommand
    {
        public CalculatePenaltiesDebtForAllCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO;
        }
    }
}
