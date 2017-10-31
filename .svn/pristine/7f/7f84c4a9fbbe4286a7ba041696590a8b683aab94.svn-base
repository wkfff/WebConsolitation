using System;
using System.Drawing.Printing;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Некоторые глобальный настройки по-умолчанию
    /// </summary>
    public class DefaultSettings
    {
        private PageSettings pageSettings;

        #region Параметры печати

        public DefaultSettings()
        {
            pageSettings = new PageSettings();

            // Формат страницы по-умолчанию - А4
            if (pageSettings.PrinterSettings.IsValid)
            {
                pageSettings.PaperSize = pageSettings.PrinterSettings.PaperSizes[8];
            }
            else
            {
                // если принтер не установлен
                pageSettings.PaperSize = null;
            }

            // расположение
            pageSettings.Landscape = true;

            // отступы  в сотых долях дюйма!
            pageSettings.Margins.Left = 0;
            pageSettings.Margins.Right = 0;
            pageSettings.Margins.Bottom = 0;
            pageSettings.Margins.Top = 0;
        }

        public event EventHandler PageSettingsChange;

        public PageSettings DafaultPageSettings
        {
            get
            {
                return pageSettings;
            }

            set
            {
                pageSettings = value;
                ChangePageSettings();
            }
        }

        private void OnPageSettingsChange(EventArgs args)
        {
            if (PageSettingsChange != null)
            {
                PageSettingsChange(this, args);
            }
        }

        private void ChangePageSettings()
        {
            EventArgs args = new EventArgs();
            OnPageSettingsChange(args);
        }

        #endregion
    }
}
