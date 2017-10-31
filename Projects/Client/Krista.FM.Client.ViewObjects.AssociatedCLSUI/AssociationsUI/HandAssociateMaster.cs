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
		// коллекция для найденных записей по 
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
			// получаем таблицу перекодировки, соответствующую текущей ассоциации
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
		///  Обработчик перехода вперед на следующую страничку
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnNextPage_Click(object sender, EventArgs e)
		{
			//*****************************************************************************************
			// непонятно только пока, по каким полям искать, наверное появится в правилах сопоставления
			//*****************************************************************************************
			// в зависимости от текущей страницы переходим на другую
			// запоминаем, где стояли до перехода на новую страничку
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
			// некоторые действия при переходе на новую страничку (другие надписи, видимость и (или) доступность кнопок и т.п.)
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
		///  Поиск записей, которые соответствуют критериям поиска
		/// </summary>
		/// <returns>Количество найденных записей</returns>
		private int SearchRowsFromClsBridge(string Code, string Name, int relevantPercent)
		{
			// еще не известно, какие названия полей будут. Наверное будем получать из правил
			int index = 0;
			// получаем таблицу с данными
			DataTable dt = ((IInplaceClsView)masterAssociation.RoleBridge).GetClsDataSet().Tables[0];
			// если ищем по полному соответствию, то ищем по полному
			if (relevantPercent == 100)
			{
				string filter = string.Format("Code = {0} and Name + {1}", Code, Name);
				DataRow[] rows = dt.Select(filter);
				// если таки нашли запись, то добавим в коллекцию ID этой записи, чтобы потом можно было легко найти
				if (rows.Length > 0)
					FoundRows.Add(0, rows[0]);
			}
			// если не по полному соответствию
			else
			{
				// пробегаем по всем записям в таблице и ищем полное соответствие кода и не полное соответствие наименования
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					// если таки нашли запись, то добавим в коллекцию ID этой записи, чтобы потом можно было легко найти
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
		///  Получаем некоторые записи из всего классификатора данных, которые аналогичны текущей записи
		/// </summary>
		/// <param name="Code"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		private int SearchSameRowsFromClsData(string Code, string Name)
		{
			// набор параметров может измениться на какой нибудь универсальный для разных правил сопоставления
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
			// настройка вида кнопок
			btnPrevPage.Text = "< Назад";
			btnNextPage.Text = "Далее >";
			btnDoubleBack.Visible = false;
			switch (NexpPageIndex)
			{
				// выбираем критерии и правила поиска, по которым будем искать запись, с которой будем сопоставлять
				// выбранную запись из классификатора данных
				case 0:
					
				case 1:
					// если выбран поиск релевантных данных, то получаем процент релевантности
					if (rbRelevant.Checked)
						relevPercent = Convert.ToInt32(nudRelevPercent.Value);
					// еще будет просмотр и выбор правил сопоставления, но пока их вроде нету

					// поиск всех записей в сопоставимом, которые соответствуют критерию поиска
					// критерий поиска будет выбираться в зависсимости от правил сопоставления 
					int rowsCount = SearchRowsFromClsBridge(masterGridRow.Cells["Code"].Text, masterGridRow.Cells["Name"].Text, relevPercent);
					if (rowsCount > 0)
					{
						// если нашли записи, то идем на шаг "просмотр результата поиска"
					}
					else
					{
						// если запись не нашли, то идем на шаг "добавление записи в сопоставимый"
					}
					btnPrevPage.Text = "Не подходит";
					btnNextPage.Text = "Подходит";
					btnDoubleBack.Visible = true;
					break;
				case 2:
					// если выбрано добавить в перекодировки, то делаем это
					if (cbAddToConversionTable.Checked)
					{

					}
					// если выбрано применить сопоставление ко всем таким же записям в разных источниках данных,
					// то сопоставляем все найденные записи
					if (cbApplyToAllSameRecords.Checked)
					{
						// смотрим в гриде по записям, там где стоит "галочка" в чекбоксе, 
						// записи по тому источнику сопоставляем все
					}
					else
					{
						// сопоставляем только одну запись, которую выбрали
					}
					break;
				case 3:
					// показываем результаты сопоставления и выходим после этого
					btnPrevPage.Enabled = false;
					btnNextPage.Enabled = false;
					btnCancel.Enabled = false;
					btnApply.Enabled = true;
					break;
				// записей не нашли, добавляем в сопоставимый текущую запись из классификатора данных
				case 4:
					break;
				// все тоже самое, показываем все записи всех источников, что нашли и выбираем, будем ли все сопоставлять
				case 5:
					break;
				case 6:
					// показываем результаты сопоставления и выходим после этого
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
		///  Обработчик возврата на предыдущую страничку
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPrevPage_Click(object sender, EventArgs e)
		{
			currentPage = prevPage;
			// настройка вида кнопок
			btnPrevPage.Text = "< Назад";
			btnNextPage.Text = "Далее >";
			btnDoubleBack.Visible = false;
			switch (prevPage)
			{
				// возврат на первый шаг
				case 0:
					if ((Button)sender == btnPrevPage)
					{
						currentPage = 4;
						prevPage = 1;
					}
					else
						prevPage = 0;
					break;
				// возврат на шаг с выбором подходящей записи
				case 1:
					btnPrevPage.Text = "Не подходит";
					btnNextPage.Text = "Подходит";
					btnDoubleBack.Visible = true;
					prevPage = 0;
					break;
				// возврат на шаг выбора параметров сопоставления к найденой записи
				case 2:
					prevPage = 1;
					break;
				// результаты сопоставления (возврат куда либо не возможен)
				case 3:
					prevPage = 3;
					break;
				// возврат на шаг при не найденной записи сопоставления
				case 4:;
					prevPage = 0;
					break;
				// возврат на шаг выбора параметров сопоставления
				case 5:
					prevPage = 4;
					break;
				// результаты сопоставления (возврат куда либо не возможен)
				case 6:
					prevPage = 6;
					break;
			}
			utcWizardMain.SelectedTab = utcWizardMain.Tabs[currentPage];
		}

		private int Similarity(string str1, string str2, int maxMatching)
		{
			// текущая длина подстроки
			int curLen = 1;
			// cчётчик совпадающих подстрок.
			int matchCount = 0;  
			// счетчик всех подстрок
			int subStrCount = 0;
			// если не указан хотя бы один параметр, то выходим
			if ((maxMatching == 0) || (str1 == string.Empty) || (str2 == string.Empty)) 
				return 0;

			// нормализуем строки, приводим к верхнему регистру, убираем начальные и конечные пробелы 
			str1 = str1.ToUpper();
			str2 = str2.ToUpper();
			str1 = str1.Trim();
			str2 = str2.Trim();
			// если строки равны, то дальше сравнивать нету смысла
			if (str1 == str2)
				return 100;
			// сравнение двух строк
			for (curLen = 1; curLen <= maxMatching; curLen++)
			{
				// сравниваем первую строку со второй
				Matching(str1, str2, curLen, ref matchCount, ref subStrCount);
				// сравниваем вторую строку с первой
				Matching(str1, str2, curLen, ref matchCount, ref subStrCount);
			}
			// если вдруг количество подстрок равно нулю, то выходим
			if (subStrCount == 0)
				return 0;
			// выводим результат сравнения строк, будет в процентах
			int result = Convert.ToInt32(matchCount / subStrCount) * 100;
			return result;
		}

		/// <summary>
		///  сравнение подстрок строки А и строки В
		/// </summary>
		/// <param name="strA">строка А</param>
		/// <param name="strB">строка В</param>
		/// <param name="len">длина подстроки</param>
		/// <param name="MatchCount">количество нахождений подстрок одной строки в другой</param>
		/// <param name="subStrCount">всего количество подстрок</param>
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
				// получаем подстроку
				subStr = strA.Substring(posStrA, len);
				// считаем совпадающие подстроки
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