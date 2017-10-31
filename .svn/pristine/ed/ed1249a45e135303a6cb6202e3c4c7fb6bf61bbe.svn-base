using System.Windows.Forms;

namespace Krista.FM.Update.Framework.Controls
{
    /// <summary>
    /// Просмотр выкачанных обновлений служюой автоматического обновления
    /// </summary>
    public partial class ReceivedPatchListControl : PatchListControl
    {
        public ReceivedPatchListControl()
        {
            InitializeComponent();
        }

        protected override int ColumnCount()
        {
            return 3;
        }

        protected override void InitializeColumn(DataGridViewColumnCollection dgvcColumns)
        {
            base.InitializeColumn(dgvcColumns);
            dgvcColumns.Add(DataGridViewColumnFactory.BuildTextColumnStyle("App", "Приложение", 100, 100, 100, true));
        }

        protected override bool FillRow(DataGridViewRow row, IUpdatePatch updatePatch)
        {
            string appName = GetAppName(updatePatch.BaseUrl);

            if (string.IsNullOrEmpty(appName))
            {
                return false;
            }

            base.FillRow(row, updatePatch);

            row.Cells.Add(new DataGridViewTextBoxCell());
            row.Cells[2].Value = appName;

            return true;
        }

        private static string GetAppName(string baseUri)
        {
            if (baseUri.ToLower().Contains("olapadmin"))
                return "OlapAdmin";
            if (baseUri.ToLower().Contains("shemedesigner"))
                return "Дизайнер схем";
            if (baseUri.ToLower().Contains("workplace"))
                return "Workplace";
            if (baseUri.ToLower().Contains("officeaddin"))
                return "Надстройка";
            if (baseUri.ToLower().Contains("mdxexpert"))
                return "MDX-Эксперт";

            return string.Empty;
        }
    }
}
