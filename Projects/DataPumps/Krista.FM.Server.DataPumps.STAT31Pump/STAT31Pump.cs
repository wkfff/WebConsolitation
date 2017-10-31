using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.STAT31Pump
{

    // СТАТ - 0031 - Прибыль убыток организаций
    public class STAT31PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОКВЭД.Прибыль убыток организаций (d_OKVED_Org)
        private IDbDataAdapter daOkved;
        private DataSet dsOkved;
        private IClassifier clsOkved;
        private Dictionary<string, int> cacheOkved = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.СТАТ_Прибыль убыток организаций (f_Marks_BenefitDam)
        private IDbDataAdapter daMarksFact;
        private DataSet dsMarksFact;
        private IFactTable fctMarksFact;

        #endregion Факты

        private int clsSourceId = -1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            clsSourceId = this.AddDataSource("СТАТ", "0031", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        #region GUIDs

        private const string D_OKVED_ORG_GUID = "5f29ec7e-78b9-4809-b369-cf1517766c52";
        private const string F_MARKS_BENEFIT_DAM_GUID = "46c62e7d-f92a-42c5-b492-9331268b508c";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOkved = this.Scheme.Classifiers[D_OKVED_ORG_GUID];
            fctMarksFact = this.Scheme.FactTables[F_MARKS_BENEFIT_DAM_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctMarksFact };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOkved, dsOkved, clsOkved);
            UpdateDataSet(daMarksFact, dsMarksFact, fctMarksFact);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarksFact);
            ClearDataSet(ref dsOkved);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private void PumpXlsSheet(ExcelHelper excelDoc)
        {
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                PumpXlsSheet(excelDoc);
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        protected override void PumpDataSource(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
