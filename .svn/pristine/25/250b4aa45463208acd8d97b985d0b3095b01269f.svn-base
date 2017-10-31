using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.FO99Pump
{
    // закачка данных данных фм
    public partial class FO99PumpModule : CorrectedPumpModuleBase
    {

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            toSetHierarchy = false;
            FileInfo[] fi = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            foreach (FileInfo f in fi)
            {
                XmlImporter importer = new XmlImporter(this);
                importer.LoadFromXml(f);
                importer.QueryData();
                importer.PumpData();
                importer.UpdateData();
                importer.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            string variant = "Импорт данных ФМ";
            SetDataSource(ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty);
            PumpDataSource(this.RootDir);
        }

        #endregion Перекрытые методы закачки

    }

}
