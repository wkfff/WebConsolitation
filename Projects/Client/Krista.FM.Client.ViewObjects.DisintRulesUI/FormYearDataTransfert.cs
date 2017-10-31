using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;

namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    public partial class FormYearDataTransfert : RequestForm
    {
        public static bool ShowRequestForm(IWin32Window parent, ref int oldYear, ref int newYear, ref bool transfertForYear)
        {
            FormYearDataTransfert frmRequest = new FormYearDataTransfert();
            frmRequest.FormCaption = "Копирование нормативов с года на год";
            if (frmRequest.ShowDialog(parent) == DialogResult.OK)
            {
                oldYear = frmRequest.YearDataTransfertParams.OldYear;
                newYear = frmRequest.YearDataTransfertParams.NewYear;

                transfertForYear = frmRequest.YearDataTransfertParams.IsTransfertSelectedRows;
                return true;
            }
            return false;
        }


        public FormYearDataTransfert()
        {
            InitializeComponent();
            YearDataTransfertParams.Parent = WorkPanel;
            YearDataTransfertParams.Dock = DockStyle.Fill;
        }

        internal YearDataTransfertParams YearDataTransfertParams
        {
            get { return yearDataTransfertParams1; }
        }
    }
}