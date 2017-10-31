using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabControl;

namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    public partial class SearchTabControl : UserControl
    {
        // Количество вкладок.
        private static int tabCount = 0;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public SearchTabControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавление вкладки с таблицей результатов.
        /// </summary>
        /// <param name="caption">Заголовок вкладки</param>
        /// <returns>Ссылка на созданную таблицу</returns>
        public SearchGridControl AddTabPage(string caption)
        {
            // Создаем новую вкладку
            UltraTab newPage = ultraTabControl.Tabs.Add();
            newPage.Text = '"' + caption + '"';
            newPage.TabControl.TabIndex = tabCount++;

            // Создаем таблицу на этой вкладке.
            SearchGridControl newGridControl = new SearchGridControl();
            newGridControl.Location = new Point(0, 0);
            newGridControl.Size = new Size(newPage.TabPage.Width, newPage.TabPage.Height);
            newGridControl.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            newPage.TabPage.Controls.Add(newGridControl);

            //Делаем активной созданную вкладку.
            ultraTabControl.SelectedTab = newPage;

            return newGridControl;
        }

        /// <summary>
        /// Обработка нажатия на пункт контекстного меню "Закрыть".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolMenuClose_Click(object sender, EventArgs e)
        {
            ultraTabControl.Tabs.Remove(ultraTabControl.SelectedTab);
            tabCount--;
        }

        /// <summary>
        /// Обработка нажатия на пункт контекстного меню "Закрыть все".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolMenuCloseAll_Click(object sender, EventArgs e)
        {
            ultraTabControl.Tabs.Clear();
            tabCount = 0;
        }

        /// <summary>
        /// Обработка нажатия правой кнопки мыши по вкладкам.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            // Если была нажата правая кнопка мыши.
            if (e.Button == MouseButtons.Right)
            {
                // то находим выбранную вкладку и выделяем ее.
                UltraTab selectedTab = ((UltraTabControl)sender).TabFromPoint(new Point(e.X, e.Y));
                if (selectedTab != null)
                {
                    ((UltraTabControl)sender).SelectedTab = selectedTab;
                }

            }
        }

        /// <summary>
        /// Срабатывает при открытии контекстного меню при щелкчке на вкладку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = (ultraTabControl.SelectedTab == null);
        }
    }

}