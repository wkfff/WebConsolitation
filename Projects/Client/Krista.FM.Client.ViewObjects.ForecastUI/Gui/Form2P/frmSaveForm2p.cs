using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Gui.Form2P
{
	public partial class frmSaveForm2p : Form
	{
		private Form2pUI content;
		private Int32 id1 = -1;
		private Int32 id2 = -1;
		private Int32 year1;
		private Int32 year2;
		private Int32 reg1;
		private Int32 reg2;

		public frmSaveForm2p()
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			InitializeComponent();
		}

		public Form2pUI Content
		{
			set { content = value; }
		}

		public Int32 Id1
		{
			get { return id1; }
		}

		public Int32 Id2
		{
			get { return id2; }
		}

		public Int32 Year
		{
			get
			{
				if (year1 == year2) return year1;
				else throw new ForecastException("В выбранных варианах различаются годы");
			}
		}

		public void SetVariant1(String name, Int32 id)
		{
			id1 = id;
			if (id != -1)
			{
				String s;
				content.Service.GetNameYearRegionForm2pByID(id, out s, out year1, out reg1);
				tbVar1.Text = s;
			}
		}

		private void btnSelect1_Click(object sender, EventArgs e)
		{
			Form2pModal fm = new Form2pModal();
			id1 = fm.ShowForm2pModal();
			if ((id1 != -1) && (content != null))
			{
				String s;
				content.Service.GetNameYearRegionForm2pByID(id1, out s, out year1, out reg1);
				tbVar1.Text = s;
			}

			if ((id2 != -1) && (content != null))
			{
				if ((year1 != year2) || (reg1 != reg2))
				{
					id2 = -1;
					tbVar2.Text = "";
				}
			}
		}

		private void btnSelect2_Click(object sender, EventArgs e)
		{
			SaveForm2pModal fm = new SaveForm2pModal();

			if (id1 != -1) 
			{
				fm.Reg = reg1;
				fm.Year = year1;
			}

			id2 = fm.ShowForm2pModal();
			if ((id2 != -1) && (content != null))
			{
				//tbVar2.Text = content.Service.GetNameForm2pByID(id2);
				String s;
				content.Service.GetNameYearRegionForm2pByID(id2, out s, out year2, out reg2);
				tbVar2.Text = s;
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}
	}

	class SaveForm2pModal : Form2pModal
	{
		private Int32 reg = -1;
		private Int32 year = -1;

		public int Reg
		{
			set { reg = value; }
		}

		public int Year
		{
			set { year = value; }
		}

		public override void AdditionsFilters(Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls.BaseClsUI clsUI)
		{
			base.AdditionsFilters(clsUI);
			if ((reg != -1) && (year != -1))
			{
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters.ClearAllFilters();
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["REFYEAR"].FilterConditions.Add(FilterComparisionOperator.Equals, year);
				clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["REFTERRITORY"].FilterConditions.Add(FilterComparisionOperator.Equals, reg);
			}
		}
	}
}