using System;
using System.Data;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI;
using Krista.FM.Domain;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands
{
    public class SelectVariantCommand : AbstractCommand
    {
        public SelectVariantCommand()
        {
            key = "SelectVariantCommand";
            caption = "Выбрать вариант";
        }

        public override void Run()
        {
            object refVariant = null;
            DataTable dtSelectVariant = new DataTable();
            if (DebtBookNavigation.Instance.Workplace.ClsManager.ShowClsModal(
                DomainObjectsKeys.d_Variant_Schuldbuch, DebtBookNavigation.Instance.CurrentVariantID, -1, -1, ref refVariant, ref dtSelectVariant))
            {
                if (refVariant != null && DebtBookNavigation.Instance.CurrentVariantID != Convert.ToInt32(refVariant))
                {
                    DebtBookNavigation.Instance.SetCurrentVariant(Convert.ToInt32(refVariant), dtSelectVariant.Rows[0], true);
                }
            }
        }
    }
}
