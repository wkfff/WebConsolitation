using System;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using BudServer;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    /// <summary>
    /// Модуль программы закачки. В отличие от основных закачек, основная логика вынесена во внутреннюю
    /// объектную модель. перекрыты только основные методв базовой модели
    /// </summary>
    public class BudgetLayersDataPumpModule : DataPumpModuleBase
    {
        // Название каталога с программами закачки
        private const string PUMP_PROGRAMS_FOLDER = "PumpPrograms";
        // Название XSD-схемы для проверки корректности XML настроек
        private const string PUMP_PRORAM_XSD_FILE_NAME = "BudgetLayersDataPump.xsd";

        private PumpProgram pumpProgram;

        /// <summary>
        /// Перекрытый метод инициализации модуля закачки. Вызывается при загрузке модуля
        /// </summary>
        /// <param name="scheme">Схема</param>
        /// <param name="programIdentifier">Идентицикатор модуля закачки</param>
        public override void Initialize(IScheme scheme, string programIdentifier, string userParams)
        {
            base.Initialize(scheme, programIdentifier, userParams);
            // формируем имена файлов программы и схемы 
            DirectoryInfo dir = GetCurrentDir();
            string xsdFileName = dir.FullName + "\\" + PUMP_PROGRAMS_FOLDER + "\\" + PUMP_PRORAM_XSD_FILE_NAME;
            string pumpProgramFileName = dir.FullName + "\\" + PUMP_PROGRAMS_FOLDER + "\\" + PumpRegistryElement.PumpProgram;
            // проверяем программы н соответсвие схеме и загружаем в XmlDocument
            string msg = String.Format("Выполняется проверка программы закачки '{0}' по схеме '{1}'", 
                pumpProgramFileName, xsdFileName);
            WriteToTrace(msg, TraceMessageKind.Information);
            XmlDocument pumpProgramXml;
            string errors;
            if (!XmlLoader.LoadXmlValidated(xsdFileName, pumpProgramFileName, out pumpProgramXml, out errors))
            {
                msg = String.Format("Проверка завершена с ошибками: {0}", errors);
                WriteToTrace(errors, TraceMessageKind.CriticalError);
                throw new PumpDataFailedException(msg);
            }
            // инициализируем программу закачки
            WriteToTrace("Загрузка конфигурации программы закачки", TraceMessageKind.Information);
            pumpProgram = new PumpProgram(this);
            pumpProgram.LoadFromXml(pumpProgramXml.DocumentElement);
        }

        /// <summary>
        /// Инициализация используемых объектов
        /// </summary>
        protected override void InitDBObjects()
        {
            WriteToTrace("Валидация программы закачки", TraceMessageKind.Information);
            string errors;
            pumpProgram.Validate(out errors);
            if (!String.IsNullOrEmpty(errors))
                throw new PumpDataFailedException("Ошибка валидации программы закачки: " + errors);
        }

        /// <summary>
        /// Запрос данных используемых объектов
        /// </summary>
        protected override void QueryData()
        {
            pumpProgram.QueryData();
        }

        /// <summary>
        /// Обновление (сохранение) данных используемых объектов
        /// </summary>
        protected override void UpdateData()
        {
            pumpProgram.UpdateData();
        }
       
        protected override void DeleteEarlierPumpedData()
        {
            if (!this.DeleteEarlierData)
                return;
            // удаляем только факты (на одинаковые клс ссылаются разные факты)
            foreach (FMObject obj in pumpProgram.fmObjects)
            {
                if (obj.objectType != FmObjectsTypes.factTable)
                    continue;
                int objSourceId = this.SourceID;
                if (obj.dataSource != null)
                    objSourceId = obj.dataSource.id;
                DirectDeleteFactData(new IFactTable[] { this.Scheme.FactTables[obj.name] },
                    -1, objSourceId, string.Empty);
            }
        }

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            // если клс качаются по своему заданному источнику, отдельно для них вызываем установку иерархии 
            foreach (FMObject obj in pumpProgram.fmObjects)
            {
                if (obj.objectType != FmObjectsTypes.cls)
                    continue;
                int objSourceId = this.SourceID;
                if (obj.dataSource != null)
                    objSourceId = obj.dataSource.id;

                IClassifier cls = Scheme.Classifiers[obj.name];
                WriteToTrace(string.Format("Установка иерархии {0} по источнику {1}", cls.FullCaption, objSourceId), TraceMessageKind.Information);
                DataSet ds = null;
                cls.DivideAndFormHierarchy(objSourceId, this.PumpID, ref ds);
                WriteToTrace(string.Format("Иерархия {0} по источнику {1} установлена", cls.FullCaption, objSourceId), TraceMessageKind.Information);
            }
        }

        /// <summary>
        /// Определяет правила обработки источника данных (?)
        /// </summary>
        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }
        
        /// <summary>
        /// Обработка одного источника данных (каталога)
        /// </summary>
        /// <param name="dir"></param>
        protected override void PumpDataSource(DirectoryInfo dir)
        {
            // *** заглушка ***
            // если в каталоге источника нет ни одного файла - родительский метод будет ругаться
            // создадим на время работы закачки один временный файл
            // ****************
            string tmpFileName = dir.FullName + "\\" + Guid.NewGuid() + ".tmp";
            try
            {
                File.Create(tmpFileName).Close();
                base.PumpDataSource(dir);
            }
            finally
            {
                if (File.Exists(tmpFileName))
                    File.Delete(tmpFileName);
            }
        }


        /// <summary>
        /// Завершение программы закачки (очистка ресурсов)
        /// </summary>
        protected override void PumpFinalizing()
        {
            pumpProgram.Clear();
        }

        /// <summary>
        /// Обработка одного каталога с данными
        /// </summary>
        /// <param name="dir"></param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            // если нужно подключиться к серверу слоев - берем данные оттуда
            string prmValue = GetParamValueByName(ProgramConfig, "ucbUseBudServerConnection", String.Empty);
            bool useBudgetServerConnection = Convert.ToBoolean(prmValue);
            if (useBudgetServerConnection)
            {
                WriteToTrace("Выполняется создание экземпляра сервера слоев", TraceMessageKind.Information);
                ServerClass srv = new ServerClass();
                WriteToTrace("Выполняется подключение к серверу слоев", TraceMessageKind.Information);
                BudServer.ISession session = srv.Connect("sysdba", "masterkey");
                WriteToTrace("Выполняется запрос объекта 'BudServer.Layers'", TraceMessageKind.Information);
                ILayers layers = (ILayers)srv.GetObject("BudServer.Layers", session);
                WriteToTrace("Выполняется запрос списка слоев", TraceMessageKind.Information);
                string allLayers = layers.GetLayersList(String.Empty).ToUpper();
                List<string> allLayersList = new List<string> (allLayers.Split(','));
                WriteToTrace("Выполняется проверка наличия необходимых слоев", TraceMessageKind.Information);
                foreach (Layer lay in pumpProgram.usedLayers)
                {
                    if (!allLayersList.Contains(lay.name.ToUpper()))
                        throw new PumpDataFailedException(String.Format("Сервер слоев не содержит слоя с именем '{0}'", lay.name));
                }
                // если все нужные слои есть - сгружаем их в нашу папку
                foreach (Layer lay in pumpProgram.usedLayers)
                {
                    WriteToTrace(string.Format("начало загрузки данных слоя '{0}'", lay.name), TraceMessageKind.Information);
                    string layData = layers.GetData(lay.name, String.Empty);
                    // временная заплата - в заголовке не прописывается кодировка
                    layData = layData.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"windows-1251\"?>");
                    byte[] dataBuff = Encoding.GetEncoding(1251).GetBytes(layData);
                    string fileName = dir.FullName + "\\" + lay.name + ".xml";
                    using (FileStream fs = File.Create(fileName))
                    {
                        fs.Write(dataBuff, 0, dataBuff.Length);
                        fs.Flush();
                    }
                }
            }
            // загружаемся в нормальном режиме (из папки)
            pumpProgram.LoadLayersFromDir(dir.FullName);
            pumpProgram.PumpData();
            UpdateData();
        }

    }
}
