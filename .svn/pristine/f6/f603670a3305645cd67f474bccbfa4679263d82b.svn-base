using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    public partial class SearchServiceControl : UserControl
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public SearchServiceControl()
        {
            InitializeComponent();
            TypeObjList.SetItemChecked(0, true);
            TypeObjList.SetItemChecked(1, true);
            TypeObjList.SetItemChecked(2, true);
            TypeObjList.SetItemChecked(3, true);
            TypeObjList.SetItemChecked(4, true);
            TypeObjList.SetItemChecked(5, true);
            TypeObjList.SetItemChecked(6, true);
            TypeObjList.SetItemChecked(7, true);
        }

        /// <summary>
        /// Обработка нажатия на кнопку поиска.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            // Инициализация параметров поиска.
            ServerLibrary.SearchParam searchParam = new ServerLibrary.SearchParam();
            searchParam.UseCaseSense = SearchParams.GetItemChecked(0);
            searchParam.UseRegExp = SearchParams.GetItemChecked(1);
            searchParam.UseWholeWord = SearchParams.GetItemChecked(2);
            searchParam.FindPackages = TypeObjList.GetItemChecked(0);
            searchParam.FindClassifiers = TypeObjList.GetItemChecked(1);
            searchParam.FindAssocitions = TypeObjList.GetItemChecked(2);
            searchParam.FindDocuments = TypeObjList.GetItemChecked(3);
            searchParam.FindAttributes = TypeObjList.GetItemChecked(4);
            searchParam.FindAssociateRule = TypeObjList.GetItemChecked(5);
            searchParam.FindAssociateMapping = TypeObjList.GetItemChecked(6);
            searchParam.FindHierarchyLevel = TypeObjList.GetItemChecked(7); 
            searchParam.pattern = FindEdit.Text;

            // Создаем таблицу результатов.
            DataTable dataTable = new DataTable();

            // Добавляем в таблицу колонки.
            dataTable.Columns.Add("key");
            dataTable.Columns.Add("caption");
            dataTable.Columns.Add("path");
            dataTable.Columns.Add("type");
            dataTable.Columns.Add("property");

            bool result = false;
            if (searchParam.pattern != "")
            {
                // Запуск поиска.
                try
                {
                    SchemeEditor.Instance.Operation.Text = "Поиск...";
                    SchemeEditor.Instance.Operation.StartOperation();
                    result = SchemeEditor.Instance.Scheme.RootPackage.Search(searchParam, ref dataTable);
                    SchemeEditor.Instance.Operation.StopOperation();
                }
                catch (Exception exc)
                {
                    SchemeEditor.Instance.Operation.StopOperation();
                    if (exc.Message == "parsing \"*\" - Quantifier {x,y} following nothing.")
                    {
                        MessageBox.Show("Некорректное использование регулярных выражений.", "Ошибка!",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        throw new Exception(exc.Message, exc);
                    }
                    return;
                }

                // Если что-нибудь нашли
                if (result)
                {
                    // Создаем вкладку на форме результатов.
                    SearchGridControl searchGrid = SchemeEditor.Instance.SearchTabControl.AddTabPage(searchParam.pattern);

                    if (SchemeEditor.Instance.DockManager.ControlPanes["searchTabControl"].Closed)
                    {
                        ((Infragistics.Win.UltraWinToolbars.StateButtonTool)
                        SchemeEditor.Instance.ToolbarManager.Tools["searchTabControl"]).Checked = true;
                        SchemeEditor.Instance.DockManager.ControlPanes["searchTabControl"].Closed = false;
                    }

                    // то выводим кол-во найденных элементов
                    searchGrid.DataSource = dataTable;
                }
                else
                {
                    //searchGrid.Enabled = false;
                    MessageBox.Show("По заданным параметрам объектов в схеме не обнаружено.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Обработка нажатия клавиш Enter и Esc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == '\r')
            {
                FindButton_Click(sender, e);
            }
        }
    }
}
