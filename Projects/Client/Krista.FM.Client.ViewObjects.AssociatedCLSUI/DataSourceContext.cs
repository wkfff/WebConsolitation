using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    /// <summary>
    /// Контекст сохраняющий информацию о текущем выбранном источнике данных для всего приложения.
    /// </summary>
    public static class DataSourceContext
    {
        private static int currentDataSourceID = -1;
        private static int currentDataSourceYear;
        private static int currentDataSourceDataCode;
        private static string currentDataSourceSuplierCode;

        public static int CurrentDataSourceID
        {
            get { return currentDataSourceID; }
            set { currentDataSourceID = value; }
        }

        public static int CurrentDataSourceYear
        {
            get { return currentDataSourceYear; }
            set { currentDataSourceYear = value; }
        }

        public static int CurrentDataSourceDataCode
        {
            get { return currentDataSourceDataCode; }
            set { currentDataSourceDataCode = value; }
        }

        public static string CurrentDataSourceSuplierCode
        {
            get { return currentDataSourceSuplierCode; }
            set { currentDataSourceSuplierCode = value; }
        }
    }
}
