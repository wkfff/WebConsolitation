using System;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI;
using Krista.FM.ServerLibrary;
using System.Data;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    class DebtorBookVariantUI : VariantClsUI
    {
        public DebtorBookVariantUI(IEntity entity)
            : base(entity)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            UltraGridExComponent._ugData.CellChange += _ugData_CellChange;
        }

        void _ugData_CellChange(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "CurrentVariant", true) == 0)
            {
                bool currentVariant = false;
                try
                {
                    currentVariant = Convert.ToBoolean(e.Cell.Text);
                }
                catch
                {
                    currentVariant = Convert.ToBoolean(Convert.ToInt32(e.Cell.Text));
                }
                //int currentID = Convert.ToInt32(e.Cell.Row.Cells["ID"].Value);
                if (currentVariant)
                {
                    foreach (DataRow row in dsObjData.Tables[0].Rows)
                    {
                        if (Convert.ToBoolean(row["CurrentVariant"])/* && Convert.ToInt32(row["ID"]) != currentID*/)
                            row["CurrentVariant"] = false;
                    }
                }
            }
        }
    }
}
