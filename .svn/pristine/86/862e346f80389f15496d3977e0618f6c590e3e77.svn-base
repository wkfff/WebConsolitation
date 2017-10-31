using System;
using System.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    public partial class SearchServiceForm : Form
    {
        private static SearchServiceForm singleInstance;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public SearchServiceForm()
        {
            InitializeComponent();
            TypeObjList.SetItemChecked(0, true);
            TypeObjList.SetItemChecked(1, true);
            TypeObjList.SetItemChecked(2, true);
            TypeObjList.SetItemChecked(3, true);
            TypeObjList.SetItemChecked(4, true);

            //Загружаем состояние формы.
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }

        public static SearchServiceForm Instance
        {
            get
            {
                if (singleInstance == null)
                {
                    singleInstance = new SearchServiceForm();
                    singleInstance.Show(SchemeEditor.Instance.Form);
                }
                return singleInstance;
            }
        }

        /// <summary>
        /// Сохранение состояния формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchServiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Krista.FM.Client.Common.DefaultFormState.Save(this);
            singleInstance.Visible = false;
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
                catch
                {
                    SchemeEditor.Instance.Operation.StopOperation();
                    result = false;
                    MessageBox.Show("Некорректное использование регулярных выражений.", "Ошибка!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("По заданным параметрам объектов в схеме не обнаружено!", "Внимание!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Обработка нажатия клавиш Enter и Esc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchServiceForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                FindButton_Click(sender, e);
            }
            else if (e.KeyChar == 27)
            {
                Close();
            }
        }

    }
}