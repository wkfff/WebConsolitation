using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls
{
	public partial class HandAssociateMaster : Form
	{
		private IAssociation masterAssociation = null;

		private UltraGridRow masterGridRow = null;

		private IWorkplace masterWorkplace = null;

		//private IConversionTable masterConversionTable = null;
		// ��������� ��� ��������� ������� �� 
		Dictionary<int, DataRow> FoundRows = new Dictionary<int,DataRow>();

		Dictionary<int, string> ss = new Dictionary<int, string>();

		int currentPage = 0;

		int prevPage = 0;

		int relevPercent = 100;

		public HandAssociateMaster(IAssociation CurentAssociation, UltraGridRow CurrentRow, IWorkplace curentWorkplace)
		{
			InitializeComponent();
			masterGridRow = CurrentRow;
			masterAssociation = CurentAssociation;
			masterWorkplace = curentWorkplace;
			btnApply.Enabled = false;
			GetSources();
			// �������� ������� �������������, ��������������� ������� ����������
			//masterConversionTable = masterWorkplace.ActiveScheme.ConversionTables[masterAssociation.FullName];
			//
		}

		private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void utcWizardMain_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
		{

		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			//masterAssociation.Dispose();
			//masterAssociation = null;
			masterGridRow = null;
			masterWorkplace = null;
			this.Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			//masterAssociation.Dispose();
			//masterAssociation = null;
			masterGridRow = null;
			masterWorkplace = null;
			this.Close();
		}

		/// <summary>
		///  ���������� �������� ������ �� ��������� ���������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnNextPage_Click(object sender, EventArgs e)
		{
			//*****************************************************************************************
			// ��������� ������ ����, �� ����� ����� ������, �������� �������� � �������� �������������
			//*****************************************************************************************
			// � ����������� �� ������� �������� ��������� �� ������
			// ����������, ��� ������ �� �������� �� ����� ���������
			prevPage = currentPage;
			switch (currentPage)
			{
				case 0:
					currentPage = 1;
					break;
				case 1:
					currentPage = 2;
					break;
				case 2:
					currentPage = 3;
					break;
				case 3:
					currentPage = 3;
					break;
				case 4:
					currentPage = 5;
					break;
				case 5:
					currentPage = 6;
					break;
				case 6:
					currentPage = 6;
					break;
			}
			// ��������� �������� ��� �������� �� ����� ��������� (������ �������, ��������� � (���) ����������� ������ � �.�.)
			NextPageAction(currentPage);
			utcWizardMain.SelectedTab = utcWizardMain.Tabs[currentPage];
		}

		private void rbNoRelevant_CheckedChanged(object sender, EventArgs e)
		{
			if (rbNoRelevant.Checked)
				nudRelevPercent.Enabled = false;
			else
				nudRelevPercent.Enabled = true;
		}

		/// <summary>
		///  ����� �������, ������� ������������� ��������� ������
		/// </summary>
		/// <returns>���������� ��������� �������</returns>
		private int SearchRowsFromClsBridge(string Code, string Name, int relevantPercent)
		{
			// ��� �� ��������, ����� �������� ����� �����. �������� ����� �������� �� ������
			int index = 0;
			// �������� ������� � �������
			DataTable dt = ((IInplaceClsView)masterAssociation.RoleBridge).GetClsDataSet().Tables[0];
			// ���� ���� �� ������� ������������, �� ���� �� �������
			if (relevantPercent == 100)
			{
				string filter = string.Format("Code = {0} and Name + {1}", Code, Name);
				DataRow[] rows = dt.Select(filter);
				// ���� ���� ����� ������, �� ������� � ��������� ID ���� ������, ����� ����� ����� ���� ����� �����
				if (rows.Length > 0)
					FoundRows.Add(0, rows[0]);
			}
			// ���� �� �� ������� ������������
			else
			{
				// ��������� �� ���� ������� � ������� � ���� ������ ������������ ���� � �� ������ ������������ ������������
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					// ���� ���� ����� ������, �� ������� � ��������� ID ���� ������, ����� ����� ����� ���� ����� �����
					if (Similarity(Name, dt.Rows[i]["Name"].ToString(), 3) >= relevantPercent)
					{
						FoundRows.Add(index, dt.Rows[i]);
						index++;
					}
				}
			}
			return FoundRows.Count;
		}

		/// <summary>
		///  �������� ��������� ������ �� ����� �������������� ������, ������� ���������� ������� ������
		/// </summary>
		/// <param name="Code"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		private int SearchSameRowsFromClsData(string Code, string Name)
		{
			// ����� ���������� ����� ���������� �� ����� ������ ������������� ��� ������ ������ �������������
			string query = string.Empty;
			IDataUpdater du = ((IEntity)masterAssociation.RoleData).GetDataUpdater(query, null);
			DataTable dt = null;
			du.Fill(ref dt);
			du.Dispose();
			du = null;
			return dt.Rows.Count;
		}

		private void NextPageAction(int NexpPageIndex)
		{
			// ��������� ���� ������
			btnPrevPage.Text = "< �����";
			btnNextPage.Text = "����� >";
			btnDoubleBack.Visible = false;
			switch (NexpPageIndex)
			{
				// �������� �������� � ������� ������, �� ������� ����� ������ ������, � ������� ����� ������������
				// ��������� ������ �� �������������� ������
				case 0:
					
				case 1:
					// ���� ������ ����� ����������� ������, �� �������� ������� �������������
					if (rbRelevant.Checked)
						relevPercent = Convert.ToInt32(nudRelevPercent.Value);
					// ��� ����� �������� � ����� ������ �������������, �� ���� �� ����� ����

					// ����� ���� ������� � ������������, ������� ������������� �������� ������
					// �������� ������ ����� ���������� � ������������ �� ������ ������������� 
					int rowsCount = SearchRowsFromClsBridge(masterGridRow.Cells["Code"].Text, masterGridRow.Cells["Name"].Text, relevPercent);
					if (rowsCount > 0)
					{
						// ���� ����� ������, �� ���� �� ��� "�������� ���������� ������"
					}
					else
					{
						// ���� ������ �� �����, �� ���� �� ��� "���������� ������ � ������������"
					}
					btnPrevPage.Text = "�� ��������";
					btnNextPage.Text = "��������";
					btnDoubleBack.Visible = true;
					break;
				case 2:
					// ���� ������� �������� � �������������, �� ������ ���
					if (cbAddToConversionTable.Checked)
					{

					}
					// ���� ������� ��������� ������������� �� ���� ����� �� ������� � ������ ���������� ������,
					// �� ������������ ��� ��������� ������
					if (cbApplyToAllSameRecords.Checked)
					{
						// ������� � ����� �� �������, ��� ��� ����� "�������" � ��������, 
						// ������ �� ���� ��������� ������������ ���
					}
					else
					{
						// ������������ ������ ���� ������, ������� �������
					}
					break;
				case 3:
					// ���������� ���������� ������������� � ������� ����� �����
					btnPrevPage.Enabled = false;
					btnNextPage.Enabled = false;
					btnCancel.Enabled = false;
					btnApply.Enabled = true;
					break;
				// ������� �� �����, ��������� � ������������ ������� ������ �� �������������� ������
				case 4:
					break;
				// ��� ���� �����, ���������� ��� ������ ���� ����������, ��� ����� � ��������, ����� �� ��� ������������
				case 5:
					break;
				case 6:
					// ���������� ���������� ������������� � ������� ����� �����
					btnPrevPage.Enabled = false;
					btnNextPage.Enabled = false;
					btnCancel.Enabled = false;
					btnApply.Enabled = true;
					break;
				case 7:
					break;
			}
		}

		/// <summary>
		///  ���������� �������� �� ���������� ���������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPrevPage_Click(object sender, EventArgs e)
		{
			currentPage = prevPage;
			// ��������� ���� ������
			btnPrevPage.Text = "< �����";
			btnNextPage.Text = "����� >";
			btnDoubleBack.Visible = false;
			switch (prevPage)
			{
				// ������� �� ������ ���
				case 0:
					if ((Button)sender == btnPrevPage)
					{
						currentPage = 4;
						prevPage = 1;
					}
					else
						prevPage = 0;
					break;
				// ������� �� ��� � ������� ���������� ������
				case 1:
					btnPrevPage.Text = "�� ��������";
					btnNextPage.Text = "��������";
					btnDoubleBack.Visible = true;
					prevPage = 0;
					break;
				// ������� �� ��� ������ ���������� ������������� � �������� ������
				case 2:
					prevPage = 1;
					break;
				// ���������� ������������� (������� ���� ���� �� ��������)
				case 3:
					prevPage = 3;
					break;
				// ������� �� ��� ��� �� ��������� ������ �������������
				case 4:;
					prevPage = 0;
					break;
				// ������� �� ��� ������ ���������� �������������
				case 5:
					prevPage = 4;
					break;
				// ���������� ������������� (������� ���� ���� �� ��������)
				case 6:
					prevPage = 6;
					break;
			}
			utcWizardMain.SelectedTab = utcWizardMain.Tabs[currentPage];
		}

		private int Similarity(string str1, string str2, int maxMatching)
		{
			// ������� ����� ���������
			int curLen = 1;
			// c������ ����������� ��������.
			int matchCount = 0;  
			// ������� ���� ��������
			int subStrCount = 0;
			// ���� �� ������ ���� �� ���� ��������, �� �������
			if ((maxMatching == 0) || (str1 == string.Empty) || (str2 == string.Empty)) 
				return 0;

			// ����������� ������, �������� � �������� ��������, ������� ��������� � �������� ������� 
			str1 = str1.ToUpper();
			str2 = str2.ToUpper();
			str1 = str1.Trim();
			str2 = str2.Trim();
			// ���� ������ �����, �� ������ ���������� ���� ������
			if (str1 == str2)
				return 100;
			// ��������� ���� �����
			for (curLen = 1; curLen <= maxMatching; curLen++)
			{
				// ���������� ������ ������ �� ������
				Matching(str1, str2, curLen, ref matchCount, ref subStrCount);
				// ���������� ������ ������ � ������
				Matching(str1, str2, curLen, ref matchCount, ref subStrCount);
			}
			// ���� ����� ���������� �������� ����� ����, �� �������
			if (subStrCount == 0)
				return 0;
			// ������� ��������� ��������� �����, ����� � ���������
			int result = Convert.ToInt32(matchCount / subStrCount) * 100;
			return result;
		}

		/// <summary>
		///  ��������� �������� ������ � � ������ �
		/// </summary>
		/// <param name="strA">������ �</param>
		/// <param name="strB">������ �</param>
		/// <param name="len">����� ���������</param>
		/// <param name="MatchCount">���������� ���������� �������� ����� ������ � ������</param>
		/// <param name="subStrCount">����� ���������� ��������</param>
		private void Matching(string strA, string strB, int len, ref int MatchCount, ref int subStrCount)
		{
			int currentSubStrCount = 0;
			int posStrA = 0;
			string subStr = string.Empty;

			currentSubStrCount = strA.Length - len + 1;
			if (currentSubStrCount > 0)
				subStrCount = subStrCount + currentSubStrCount;
			for (posStrA = 0; posStrA <= currentSubStrCount - 1; posStrA++)
			{
				// �������� ���������
				subStr = strA.Substring(posStrA, len);
				// ������� ����������� ���������
				if (strB.Contains(subStr))
					MatchCount++;
			}
		}

		private void GetSources()
		{
			ss = ((IClassifier)masterAssociation.RoleData).GetDataSourcesNames();
		}
	}
}