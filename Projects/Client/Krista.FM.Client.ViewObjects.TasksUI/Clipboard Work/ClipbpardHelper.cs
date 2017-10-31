using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    internal class ClipbpardHelper
    {
        /// <summary>
        ///  копировать задачу в буфер обмена
        /// </summary>
        internal void CopyTask()
        {
            
        }

        /// <summary>
        /// вырезать задачу в буфер обмена
        /// </summary>
        internal void CutTask()
        {
            
        }

        /// <summary>
        /// вставить задачу из буфера обмена
        /// </summary>
        internal void PastTask()
        {
            
        }

        internal static List<int> CuttedTasksIDs
        {
            get { return cuttedTasksIDs; }
        }

        private static List<int> cuttedTasksIDs = new List<int>();
        internal static void ResetCuttedTasksIDs(UltraGrid grid)
        {
            if ((cuttedTasksIDs == null) || (cuttedTasksIDs.Count == 0))
            {
                return;
            }
            UltraGridHelper.SetRowsTransparent(grid, cuttedTasksIDs, false);
            cuttedTasksIDs.Clear();
        }

        internal static List<int> CuttedDocumentsIDs
        {
            get { return cuttedDocumentsIDs; }
        }

        private static List<int> cuttedDocumentsIDs = new List<int>();
        internal static void ResetCuttedDocumentsIDs(UltraGrid documentsGrid)
        {
            if ((cuttedDocumentsIDs == null) || (cuttedDocumentsIDs.Count == 0))
            {
                return;
            }
            UltraGridHelper.SetRowsTransparent(documentsGrid, cuttedDocumentsIDs, false);
            cuttedDocumentsIDs.Clear();
        }

    }
}
