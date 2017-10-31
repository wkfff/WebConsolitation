using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.FNS28nDataPump
{
    // фнс 0001 - 28н (65н)
    public class FNS28nDataPumpModule : CorrectedPumpModuleBase
    {
        #region Поля

        #region классификаторы

        // КД.28н (d_KD_A28N)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        // ОКАТО.28н (d_OKATO_A28N)
        private IDbDataAdapter daOKATO;
        private DataSet dsOKATO;
        private IClassifier clsOKATO;
        private Dictionary<string, int> okatoCache = null;
        private int nullOkato = -1;
        // ОКТМО.65н (d_OKTMO_A65N)
        private IDbDataAdapter daOktmo;
        private DataSet dsOktmo;
        private IClassifier clsOktmo;
        private Dictionary<string, int> oktmoCache = null;
        private int nullOktmo = -1;
        // ОКВЭД.28н (d_OKVED_A28N)
        private IDbDataAdapter daOKVED;
        private DataSet dsOKVED;
        private IClassifier clsOKVED;
        private Dictionary<string, int> okvedCache = null;
        // Фиксированный.ФНС_28н_Показатели (fx_FX_DataMarks28n)
        private IDbDataAdapter daDataMarks28n;
        private DataSet dsDataMarks28n;
        private Dictionary<int, int> dataMarksCache = null;
        private IClassifier fxcDataMarks28n;
        // Фиксированный.ФНС_65н_Показатели (fx_FX_DataMarks65n)
        private IDbDataAdapter da65nMarks;
        private DataSet ds65nMarks;
        private IClassifier cls65nMarks;
        private Dictionary<string, int> cache65nMarks = null;

        #endregion классификаторы

        #region факты

        // Факт.28н_без расщепления (f_F_DirtyUMNS28n)
        private IDbDataAdapter daDirtyUMNS28n;
        private DataSet dsDirtyUMNS28n;
        private IFactTable fctDirtyUMNS28n;
        // Факт.28н_с расщеплением (f_F_UMNS28n)
        private IFactTable fctUMNS28n;

        #endregion факты

        #region протокол

        // признак, выполнять сверку с протоколом;
        private bool isCheckProtocol = false;

        // Количество выгруженных файлов
        private int totalUnloadedFiles;
        // Фактическое количество закачанных файлов
        private int totalLoadedFiles;

        // Количество выгруженных документов
        private int totalUnloadedDocs;
        // Фактическое количество загруженных документов
        private int totalLoadedDocs;

        // Конторольная сумма поступившего налога
        private decimal controlIncomeTax;
        // Итоговая сумма поступившего налога
        private decimal totalIncomeTax;

        // Конторольная сумма начисленного налога
        private decimal controlAccrualTax;
        // Итоговая сумма начисленного налога
        private decimal totalAccrualTax;

        // Список файлов и количество загруженных в них документов
        private Dictionary<string, int> logs = new Dictionary<string, int>();

        #endregion протокол

        #region протокол выгрузки

        private struct ProtocolRow
        {
            public string fileName;
            public string fileId;
            public string infoType;
            public long fileSize;
            public int countDocs;
            public int countPumpedDocs;
            public decimal totalIncomeTax;
            public decimal totalAccrualTax;
        };

        private ArrayList protocolRows = new ArrayList();

        // Наименование отправителя
        private string senderName = string.Empty;

        // Телефон отправителя
        private string senderPhone = string.Empty;
        
        // Должность ответственного лица
        private string senderEmployment = string.Empty;
        
        // Ответственное лицо
        private string senderResponsible = string.Empty;
        
        // Имя получателя
        private string destineeName = string.Empty;

        #endregion протокол выгрузки

        #region прочие переменные

        private Dictionary<string, string> okvedMaskMapping = new Dictionary<string, string>();
        private Dictionary<string, DataRow> dirtyUMNS28nCache = new Dictionary<string, DataRow>();

        private int processedDocs = 0;
        private int droppedDocs = 0;
        private int emptyDocs = 0;
        private int filesCount = 0;
        private int totalFiles;
        private CultureInfo culture;

        private bool disintAll = false;

        // Коллекция пропущенных 
        // Ключ - код налоголпательщика, Значение - список ИД документов, содержащих этот код
        private Dictionary<string, List<string>> badTaxpayerCodesCache = new Dictionary<string, List<string>>();

        // признак контингента (нужен при формировании классификатора KD)
        int mark = 0;
        // код кд - признак кд (нужно для того, чтобы не качать данные, если уже были закачаны данные по кд с большим признаком)
        // то есть если закачаны данные по субъекту, по МО не качать
        private Dictionary<string, int> kdMarkCache = null;

        // 28н only
        // общее количество документов в файлк
        private int totalIdDocCount = 0;
        // количество некорректных документов в файле
        private int incorrectIdDocCount = 0;

        // 65н - встретился показатель OKTMO
        private bool oktmoPresence = false;
        private int incorrectDocCount = 0;

        private Dictionary<string, decimal[]> fact65nCache = null;
        private ExcelHelper excelHelper;

        #endregion прочие переменные

        #endregion Поля

        #region Константы

        // Сведения служебной части.
		// Идентификатор файла
		private const string constAttrFileID = "ИДФАЙЛ";
		// Тип информации
		private const string constAttrInfoType = "ТИПИНФ";
		// Идентификатор получателя
		private const string constAttrDestID = "ИДПОЛ";
		// Код налогового органа
		private const string constAttrTaxCode = "НАИМОТПР";
		// Телефон отправителя
		private const string constAttrSenderTel = "ТЕЛОТПР";
		// Должность отправителя
		private const string constAttrSenderPlace = "ДОЛЖНОТПР";
		// Фамилия, Имя, Отчество отправителя
		private const string constAttrSenderName = "ФИООТПР";
		// Версия передающей программы
		private const string constAttrProgVersion = "ВЕРСПРОГ";
		// Количество документов
		private const string constAttrDocsAmount = "КОЛДОК";
		// Конец фрагмента
		private const string constAttrEndOfFragment = "@@@";
		// Идентификатор документа
		private const string constAttrDocID = "ИДДОК";
		// Конец файла
		private const string constAttrEndOfFile = "===";

        /// <summary>
        /// Разрядность целой части значений поля суммы
        /// </summary>
        private const int constSumFieldCapacity = 13;

        #endregion Константы

        #region Инициализация

        public FNS28nDataPumpModule() : base()
		{
            culture = new CultureInfo("ru-RU");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = string.Empty;
        }

        #endregion Инициализация

        #region Закачка данных

        #region работа с базой и кэшами

        private void FillCache()
        {
            FillRowsCache(ref dataMarksCache, dsDataMarks28n.Tables[0], "CODE");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref kdMarkCache, dsKD.Tables[0], "CODESTR", "RefSignKD28n");
            FillRowsCache(ref okatoCache, dsOKATO.Tables[0], "CODE");
            FillRowsCache(ref okvedCache, dsOKVED.Tables[0], "CODESTR");
            FillRowsCache(ref oktmoCache, dsOktmo.Tables[0], "Code");
            FillRowsCache(ref cache65nMarks, ds65nMarks.Tables[0], "Id");
        }

        private void InitUpdatedFixedRows()
        {
            nullOktmo = clsOktmo.UpdateFixedRows(this.DB, this.SourceID);
        }

        protected override void QueryData()
        {
            InitDataSet(ref daDataMarks28n, ref dsDataMarks28n, fxcDataMarks28n, true, string.Empty, string.Empty);
            InitDataSet(ref da65nMarks, ref ds65nMarks, cls65nMarks, true, string.Empty, string.Empty);
            if (dsDataMarks28n.Tables[0].Rows.Count == 0)
                throw new Exception("Отсутствуют показатели");
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO, false, string.Empty);
            InitClsDataSet(ref daOKVED, ref dsOKVED, clsOKVED, false, string.Empty);
            InitClsDataSet(ref daOktmo, ref dsOktmo, clsOktmo, false, string.Empty);

            InitFactDataSet(ref daDirtyUMNS28n, ref dsDirtyUMNS28n, fctDirtyUMNS28n);
            //InitDataSet(ref daDirtyUMNS28n, ref dsDirtyUMNS28n, fctDirtyUMNS28n, false, "1 = 0", string.Empty, false);

            InitUpdatedFixedRows();
            FillCache();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daOKVED, dsOKVED, clsOKVED);
            UpdateDataSet(daOktmo, dsOktmo, clsOktmo);
            UpdateDataSet(daDirtyUMNS28n, dsDirtyUMNS28n, fctDirtyUMNS28n);
        }

        #region guid
        private const string FX_FX_DATA_MARKS_28N_GUID = "b79484b1-b50a-4a0e-bb73-2e6187d6bf82";
        private const string FX_65N_MARKS_GUID = "88c43768-4b4f-4ac0-a706-6d24f2654780";
        private const string D_KD_A28N_GUID = "218ecabf-9f86-41f3-9dce-6bcb7e2c5dc8";
        private const string D_OKATO_A28N_GUID = "ff6b0307-058d-412d-b2d2-733edaad489b";
        private const string D_OKVED_A28N_GUID = "c19b0c79-8c68-4d60-9c2b-efc2712e3658";
        private const string D_OKTMO_GUID = "f1b8c0ce-86ac-4c84-811f-dcfaa93b3aac";
        private const string F_F_DIRTY_UMNS_28N_GUID = "88cbf43e-9e95-4db6-8cac-f36b4263aaed";
        private const string F_F_UMNS_28N_GUID = "96cc00e1-78b2-40c2-b19c-c0e57197649a";
        #endregion guid
        protected override void InitDBObjects()
        {
            fxcDataMarks28n = this.Scheme.Classifiers[FX_FX_DATA_MARKS_28N_GUID];
            cls65nMarks = this.Scheme.Classifiers[FX_65N_MARKS_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_A28N_GUID],
                clsOKATO = this.Scheme.Classifiers[D_OKATO_A28N_GUID],
                clsOKVED = this.Scheme.Classifiers[D_OKVED_A28N_GUID],
                clsOktmo = this.Scheme.Classifiers[D_OKTMO_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctDirtyUMNS28n = this.Scheme.FactTables[F_F_DIRTY_UMNS_28N_GUID],
                fctUMNS28n = this.Scheme.FactTables[F_F_UMNS_28N_GUID] };

            this.CubeFacts = new IFactTable[] { fctDirtyUMNS28n, fctUMNS28n };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsDirtyUMNS28n);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOKATO);
            ClearDataSet(ref dsOKVED);
            ClearDataSet(ref dsOktmo);
            ClearDataSet(ref dsDataMarks28n);
            ClearDataSet(ref ds65nMarks);
        }

        #endregion работа с базой и кэшами

        #region закачка 28n

        /// <summary>
		/// Проверка названия файла
		/// </summary>
		/// <param name="fileName">Файл</param>
		/// <param name="fileNo">Номер файла в каталоге</param>
        /// <param name="prevFile">Предыдущий файл</param>
		/// <returns>Результат проверки</returns>
		private string CheckFileName(FileInfo fileName, int fileNo, FileInfo prevFile)
		{
			// Выполняем проверку - имя файла должно соответствовать правилам:
			// 8 символов имя, 3 расширение.
			if (fileName.Name.Length - fileName.Extension.Length != 8)
				return string.Format("Некорректный формат названия файла: {0}.", fileName.Name);
			
			if (fileName.Extension.Length != 4)
				return string.Format("Некорректный формат расширения файла: {0}.", fileName.Name);
			
			// Первые три символы имени - префикс (значение "TAX").
			if (!fileName.Name.ToUpper().StartsWith("TAX"))
				return string.Format("Некорректный формат названия файла (отсутствует префикс 'TAX'): {0}.", fileName.Name);
			
			// Выполняем проверку части файла.
			// Для первого файла проверяем – если номер части (символы 10, 11 имени файла) не «01», 
			// то пишем ошибку (не критическую), что нет первой части.
			if (fileNo == 0 && Convert.ToInt32(fileName.Name.Substring(10, 2)) != 1)
			{
				WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Отсутствует первая часть данных.");
			}

			// Для всех последующих проверяем – если номер части не следующие за предыдущей, 
			// то пишем ошибку (не критическую).
			if (Convert.ToInt32(fileName.Name.Substring(10, 2)) -
                Convert.ToInt32(prevFile.Name.Substring(10, 2)) > 1)
			{
				WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
					string.Format("Номер части данных не является следующим: {0}.", fileName.Name));
			}
			return string.Empty;
		}

		/// <summary>
		/// Возвращает фрагмент данных (строки, находящиеся между разделителями фрагментов)
		/// </summary>
		/// <param name="sr">Данные</param>
		/// <returns>Фрагмент</returns>
		private List<string> GetFragment(string[] strList, ref int startIndex, string startFragmentAttr)
		{
            List<string> result = new List<string>(0);

            if (startIndex >= strList.GetLength(0))
            {
                return result;
            }

            while (startIndex < strList.GetLength(0) &&
                !strList[startIndex].Trim().ToUpper().StartsWith(startFragmentAttr)) 
            {
                if (strList[startIndex].Contains(constAttrEndOfFile))
                {
                    break;
                }
                startIndex++;
            }

            int i = 0;
            int count = strList.GetLength(0);
            for (i = startIndex; i < count; i++)
            {
                if (strList[i].Contains(constAttrEndOfFragment))
                {
                    break;
                }
                result.Add(strList[i]);
            }
            startIndex = i + 1;

			return result;
		}

		/// <summary>
		/// Ищет значение атрибута в текущем фрагменте
		/// </summary>
		/// <param name="fragment">Данные</param>
		/// <param name="attrName">Название стрибута</param>
		/// <returns>Значение стрибута</returns>
		private string FindAttrValue(List<string> fragment, string attrName)
		{
			string result = string.Empty;
			string str;
			string attr = attrName.ToUpper();
			for (int i = 0; i < fragment.Count; i++)
			{
				str = fragment[i].ToUpper();
				if (str.StartsWith(attr))
				{
					result = str.Substring(attr.Length + 1);
					break;
				}
			}
			return result.Trim();
		}

        /// <summary>
        /// Добавляет запись в датасет пропущенных кодов налогоплательщика
        /// </summary>
        /// <param name="docID">ИД документа</param>
        /// <param name="code">Код налогоплательщика</param>
        private void WriteToBadCodesCache(string docID, string code)
        {
            List<string> value = null;

            if (!badTaxpayerCodesCache.ContainsKey(code))
            {
                value = new List<string>(100);
                badTaxpayerCodesCache.Add(code, value);
            }
            else
            {
                value = badTaxpayerCodesCache[code];
            }

            value.Add(docID);
        }

        /// <summary>
        /// Записывает в лог данные из датасета пропущенных кодов налогоплательщика
        /// </summary>
        private void WriteBadTaxpayerCodesCacheToBD()
        {
            foreach (KeyValuePair<string, List<string>> kvp in badTaxpayerCodesCache)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format(
                        "Пропущено {0} документов с некорректным кодом налогоплательщика {1}: \n{2}.",
                        kvp.Value.Count, kvp.Key, string.Join(", ", kvp.Value.ToArray())));
            }
        }

        private DataRow GetKDRowByID(int kdID)
        {
            foreach (DataRow row in dsKD.Tables[0].Rows)
                if (Convert.ToInt32(row["ID"]) == kdID)
                    return row;
            return null;
        }

        private void DeleteKDData(int kdID, int mark)
        {
            DeleteData(string.Format("REFKDA28N = {0}", kdID), string.Empty);
            DataRow kdRow = GetKDRowByID(kdID);
            kdRow["RefSignKD28n"] = mark;
        }

        /// <summary>
        /// Закачивает данные фактов
        /// </summary>
        /// <param name="fileDate">Дата файла</param>
        /// <param name="kd">Код КД</param>
        /// <param name="okato">Код ОКАТО</param>
        /// <param name="okved">Код ОКВЭД</param>
        /// <param name="statusCode">Код статуса</param>
        /// <param name="taxpayerCode">Код налогоплательщика</param>
        /// <param name="docID">ИД документа</param>
        /// <param name="strNum">Номер строки</param>
        /// <param name="attrCode">Код атрибута</param>
        /// <param name="attrValue">Значение атрибута</param>
        /// <param name="docIsEmpty">Признак пустого документа</param>
        /// <param name="addedRowsCount">Счетчик добаленных документов</param>
        /// <param name="iterativeRowsCount">Счетчик повторяющих документов</param>
        private void PumpFactData(string fileDate, string kd, string okato, string okved, int statusCode,
            int taxpayerCode, string docID, int strNum, int attrCode, string attrValue, ref bool docIsEmpty)
        {
            double sum = double.Parse(attrValue.PadLeft(1, '0'), culture);
            if (sum == 0)
            {
                return;
            }

            // Проверка соответствия разрядности суммы отчета полю в базе
            if (sum.ToString("F").Split(new string[] { culture.NumberFormat.CurrencyDecimalSeparator }, StringSplitOptions.None)[0].Length > constSumFieldCapacity)
            {
                // Если размер числа превышает размерность поля, то пропускать документ и выдавать сообщение в протокол.
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Разрядность суммы {0:F} документа {1} не соответствует формату.", sum, docID));
                return;
            }

            // Ищем код в таблице показателей
            int markID = -1;
            if (!dataMarksCache.ContainsKey(attrCode))
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeError, string.Format(
                        "Пропущена сумма (строка {0}, ИДДОК:{1}): в таблице показателей не найден код {2}, сумма {3}.",
                        strNum, docID, attrCode, attrValue));
                return;
            }
            else
            {
                markID = dataMarksCache[attrCode];
            }
            docIsEmpty = false;

            // Для каждого показателя делаем:

            // Добавляем код дохода в классификатор данных (если еще нет по этой закачке PumpID такого кода)
            bool kdPresence = (kdMarkCache.ContainsKey(kd));
            if (!kdPresence)
                kdMarkCache.Add(kd, mark);
            int kdID = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kd, new object[] { "CODESTR", kd, "RefSignKD28n", mark });
            bool toPumpFact = true;
            // если данные по этому кд уже есть,
            // причем признак меньше, чем закачиваемый, удаляем соответственные записи факта и меняем признак у кд
            if ((kdPresence) && (mark != 0))
            {
                int kdMark = kdMarkCache[kd];
                // если данные уже есть по большему признаку, в факт записи не качаем
                toPumpFact = (mark <= kdMark);
                if (kdMark > mark)
                {
                    DeleteKDData(kdID, mark);
                    kdMarkCache[kd] = mark;
                }
            }

            // Добавляем ОКАТО в классификатор данных (если еще нет по этой закачке PumpID такого кода)
            okato = okato.TrimStart('0').PadLeft(1, '0');
            int okatoID = PumpCachedRow(okatoCache, dsOKATO.Tables[0], clsOKATO, okato, "CODE", null);

            // Добавляем ОКВЭД в классификатор данных (если еще нет по этой закачке PumpID такого кода)
            int okvedID;
            if (okved.Trim('0') == string.Empty)
            {
                okvedID = PumpCachedRow(okvedCache, dsOKVED.Tables[0], clsOKVED, "0",
                    new object[] { "CODESTR", okved });
            }
            else
            {
                okvedID = PumpCachedRow(okvedCache, dsOKVED.Tables[0], clsOKVED, okved,
                    new object[] { "CODESTR", okved });
            }

            // Получаем значение ключа для поиска по кэшу фактов
            string key = GetComplexCacheKey(new object[] { kdID, okatoID, okvedID, markID, taxpayerCode, statusCode });
            
            // Добавляем строку в таблицу фактов (или добавляем сумму к существующей строке 
            // и увеличиваем количество)
            if (!toPumpFact)
                return;
            DataRow row;
            if (dirtyUMNS28nCache.ContainsKey(key))
            {
                row = dirtyUMNS28nCache[key];
                row["SUMME"] = sum + GetDoubleCellValue(row, "SUMME", 0);
                row["DOCCOUNT"] = GetIntCellValue(row, "DOCCOUNT", 0) + 1;
            }
            else
            {
                row = dsDirtyUMNS28n.Tables[0].NewRow();

                row["PUMPID"] = this.PumpID;
                row["SOURCEID"] = this.SourceID;
                row["REFDATAMARKS28N"] = markID;
                row["REFKDA28N"] = kdID;
                row["REFOKATOA28N"] = okatoID;
                row["REFOKVEDA28N"] = okvedID;
                row["REFTAXPAYERCODES"] = taxpayerCode;
                row["REFTAXPAYERSTATUSCODES"] = statusCode;
                row["RefYearDayUNV"] = fileDate;
                row["REFISDISINT"] = 0;
                row["SUMME"] = sum;
                row["DOCCOUNT"] = 1;
                
                dsDirtyUMNS28n.Tables[0].Rows.Add(row);
                dirtyUMNS28nCache.Add(key, row);
            }

            // Если накопилось много закачанных документов, то сбрасываем в базу
            if (dsDirtyUMNS28n.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 4)
            {
                UpdateData();
                ClearDataSet(daDirtyUMNS28n, ref dsDirtyUMNS28n);
                if (dirtyUMNS28nCache != null) dirtyUMNS28nCache.Clear();
            }
        }

        /// <summary>
        /// Разбивает атрибут на код и значение
        /// </summary>
        /// <param name="attr">Атрибут</param>
        /// <param name="attrCode">Код</param>
        /// <param name="attrValue">Значение</param>
        private void ProcessAttribute(string attr, ref int attrCode, ref string attrValue)
        {
            int ind = attr.IndexOf(":");
            if (ind < 0)
                return;
            attrCode = Convert.ToInt32(attr.Substring(0, ind));
            attrValue = attr.Substring(ind + 1);

        }

        /// <summary>
        /// Обрабатывает значение ОКВЭД
        /// </summary>
        /// <param name="attrValue">Значение из файла отчета</param>
        /// <returns>Результат обработки</returns>
        private string SetOkvedMask(string attrValue)
        {
            string result = attrValue;

            if (attrValue != string.Empty)
            {
                if (!okvedMaskMapping.ContainsKey(attrValue))
                {
                    string[] sections = new string[3];

                    // Если нет разделяющих подкоды точек, то формируем массив подкодов вручную
                    if (!attrValue.Contains("."))
                    {
                        result = attrValue;
                    }
                    else
                    {
                        sections = attrValue.Split('.');

                        // Догоняем до маски ХХ.ХХ.ХХ
                        int count = sections.GetLength(0);
                        for (int k = 1; k < count; k++)
                        {
                            sections[k] = sections[k].PadRight(2, '0');
                        }
                        result = string.Concat(sections).PadRight(sections[0].Length + 4, '0');
                    }

                    if (result.Trim('0') == string.Empty)
                    {
                        okvedMaskMapping.Add(attrValue, "0");
                    }
                    else
                    {
                        okvedMaskMapping.Add(attrValue, result);
                    }
                }
                else
                {
                    result = okvedMaskMapping[attrValue];
                }
            }

            return result;
        }

        /// <summary>
        /// Сверяет наименование файла с данными из самого файла
        /// </summary>
        /// <param name="taxCode">ИД отправителя</param>
        /// <param name="fileID">ИД файла</param>
        /// <param name="fileName">Наименование файла</param>
        private void CheckFileID(string taxCode, string fileID, string fileName)
        {
            // Сверяем идентификатор отправителя из имени  файла, реквизит "НаимОтпр" и отправителя из реквизита
            // "Идентификатор файла".
            if (taxCode != fileID.Substring(0, 4) || taxCode != fileName.Substring(3, 4) ||
                fileID.Substring(0, 4) != fileName.Substring(3, 4))
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format(
                        "В файле {0} реквизит 'Наименование налогового органа' не совпадает во всех вхождениях", fileName));
            }
        }

        /// <summary>
        /// Проверяет, есть ли во фрагменте строки с суммами (показателями)
        /// </summary>
        /// <param name="fragment">Фрагмент</param>
        /// <returns>true если документ не пустой</returns>
        private bool CheckEmptyDoc(List<string> fragment)
        {
            int attrCode = -1;
            string attrValue = string.Empty;

            for (int i = fragment.Count - 1; i > 0; i--)
            {
                ProcessAttribute(fragment[i], ref attrCode, ref attrValue);
                if (attrCode >= 20701 && attrCode <= 50000) return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет текущий фрагмент
        /// </summary>
        /// <param name="fragment">Фрагмент</param>
        /// <param name="docID">ИД документа</param>
        /// <param name="firstDoc">Первый документ файла</param>
        /// <param name="lastDoc">Последний документ файла</param>
        /// <param name="docNum">Номер документа</param>
        /// <param name="processedDocsCount">Счетчик пропущенных документов</param>
        /// <param name="strNum">Номер строки в файле</param>
        /// <returns>Пропускать документ или нет</returns>
        private bool CheckCurrentFragment(List<string> fragment, ref string docID, ref string firstDoc, 
            ref string lastDoc, ref int docNum, int processedDocsCount, int strNum, bool isFirstFile,
            string fileName)
        {
            totalIdDocCount++;
            docID = FindAttrValue(fragment, constAttrDocID);
            if (docID == string.Empty)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format("Пропущен разорванный документ (строка {0})", strNum));
                return false;
            }

            if (docID.Length < 24)
                return false;

            if (docID.Substring(24, 1) != fileName.Substring(7, 1))
            {
                incorrectIdDocCount++;
                return false;
            }

            // Запоминаем первый документ, чтобы потом записать его в лог
            if (firstDoc == string.Empty)
            {
                firstDoc = docID;
            }
            // Запоминаем последний документ, чтобы потом записать его в лог
            lastDoc = docID;

            // Запоминаем номер первого документа для проверки последовательности документов
            int tmpInt = Convert.ToInt32(docID.Substring(26));
            if (processedDocsCount == 0)
            {
                // Если это первый документ в обработке, а номер у него не первый - пишем ошибку
                if (isFirstFile && tmpInt != 1)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Отсутствует первый фрагмент данных.");
                }

                docNum = tmpInt;
            }
            else
            {
                // Если это не первый документ, то номер документа должен быть следующим за предыдущим
                if (tmpInt != docNum)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeWarning, string.Format(
                            "Текущий фрагмент данных не является следующим (строка {0}, ИДДОК:{1})", strNum, docID));
                    docNum = tmpInt;
                }
            }
            docNum++;

            return true;
        }

        /// <summary>
        /// Разбирает версию программы
        /// </summary>
        /// <param name="version">Версия</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        private bool ParseProgVersion(string version, out int year, out int month)
        {
            year = -1;
            month = -1;
            int day = -1;

            if (!version.Contains("#")) return false;

            string date = version.Split('#')[1];

            CommonRoutines.DecodeShortDate("00." + date, out year, out month, out day);

            return true;
        }

        /// <summary>
        /// Проверяет дату отчета на соответствие источнику
        /// </summary>
        /// <param name="fragment">Фрагмент заголовка отчета</param>
        private void CheckReportDate(List<string> fragment)
        {
            int year = -1;
            int month = -1;
            if (ParseProgVersion(FindAttrValue(fragment, constAttrProgVersion), out year, out month))
            {
                if (year != this.DataSource.Year)
                    throw new Exception(string.Format("Год отчета {0} не соответствует параметрам источника", year));
                if (month != this.DataSource.Month)
                    throw new Exception(string.Format("Месяц {0} отчета не соответствует параметрам источника", 
                        CommonRoutines.MonthByNumber[month - 1]));
            }
        }

        /// <summary>
        /// Обрабатывает фрагмент заголовка файла
        /// </summary>
        /// <param name="fragment">Фрагмент</param>
        /// <param name="docsAmount">Количество документов в файле</param>
        /// <param name="progressText">Текст для прогресса</param>
        /// <param name="taxCode">Код налогоплательщика</param>
        /// <param name="fileID">ИД файла</param>
        private void CheckHeaderFragment(List<string> fragment, ref int docsAmount, 
            ref string progressText, out string taxCode, out string fileID)
        {
            // Проверяем дату файла на соответствие источнику, если она указана
            CheckReportDate(fragment);
            // Запоминаем «ИдФайл».
            fileID = FindAttrValue(fragment, constAttrFileID);
            // Читаем все сведения служебной части и выполняем проверку – наш ли файл
            // (pеквизит «ТипИнф» должен иметь значение «РАСЧЕТЫ С БЮДЖЕТОМ»)
            string str = FindAttrValue(fragment, constAttrInfoType);
            if (str.ToUpper() != "РАСЧЕТЫ С БЮДЖЕТОМ")
                throw new PumpDataFailedException("Некорректный тип информации.");
            // Запоминаем «НаимОтпр», он понадобится для записи в данные.
            taxCode = FindAttrValue(fragment, constAttrTaxCode);
            // Запоминаем реквизит количество документов «КолДок», он нам потом понадобится для проверки.
            str = FindAttrValue(fragment, constAttrDocsAmount);
            if (str != string.Empty)
            {
                docsAmount = Convert.ToInt32(str);
                progressText = string.Format(" из {0}", docsAmount);
            }
            progressText = "Документ {0}" + progressText;
        }

        /// <summary>
        /// Записывает данные списка в протокол. 
        /// Только для отладки.
        /// </summary>
        /// <param name="message">Сообщение, предваряющее вывод данных списка</param>
        /// <param name="list">Списка</param>
        private void WriteStatListInProtocol(string message, List<string> list)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, 
                string.Format("{0}: {1}", message, CommonRoutines.ListToString(list)));
        }

        /// <summary>
        /// Функция закачки списка файлов
        /// </summary>
        /// <param name="dir">Каталог с файлами для закачки</param>
        private void ProcessDir28n(DirectoryInfo dir)
        {
            List<string> fragment;
            string str;
            string taxCode;
            int docsAmount = -1;
            string fileID;
            string docID = string.Empty;
            // Номер строки
            int strNum = 0;
            // Первый документ в файле
            string firstDoc = string.Empty;
            // Последний документ в файле
            string lastDoc = string.Empty;
            // Номер документа
            int docNum = 0;
            string progressText = string.Empty;
            // Счетчик показателей в файле
            int marksCount = 0;
            int currentRow = 0;

            if (dirtyUMNS28nCache != null) dirtyUMNS28nCache.Clear();

            processedDocs = 0;
            droppedDocs = 0;
            emptyDocs = 0;

            string fileDate = string.Format("{0}{1:00}00", this.DataSource.Year, this.DataSource.Month);
            if (fileDate.Length > 8)
            {
                fileDate = fileDate.Remove(4, 1);
            }

            FileInfo[] filesList = dir.GetFiles("*.*");
            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                //if (filesList[i].Extension.Substring(0, 2).ToUpper() == ".A")
                //    continue;

                filesCount++;
                 
                // 65n не закачиваем
                if (filesList[i].Name.Length >= 15)
                    continue;

                string fileName = string.Format("{0}\\{1}", GetShortSourcePathBySourceID(this.SourceID), filesList[i].Name);
                SetProgress(totalFiles, filesCount,
                    string.Format("Обработка файла {0}...", fileName),
                    string.Format("Файл {0} из {1}", filesCount, totalFiles), true);

                if (!filesList[i].Exists || filesList[i].Extension.ToUpper() == ".PRT" || 
                    filesList[i].Extension.ToUpper() == ".XLS")
                    continue;

                badTaxpayerCodesCache.Clear();

                firstDoc = string.Empty;
                strNum = 0;

                // Счетчик пропущенных документов
                int droppedDocsCount = 0;
                // Счетчик пустых документов
                int emptyDocsCount = 0;
                // Счетчик пустых кодов налогоплательцика
                int emptyTaxpayerCodeCount = 0;
                // Счетчик пустых ОКВЭД
                int emptyOkvedCount = 0;
                // Всего документов
                int processedDocsCount = 0;
                // Счетчик некорректных кодов налогоплательцика
                int badTaxpayerCodeCount = 0;

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeStartFilePumping,
                    string.Format("Закачка файла {0}.", filesList[i].FullName));

                try
                {
                    // Проверка корректности названия файла
                    if (i == 0)
                    {
                        str = CheckFileName(filesList[0], i, filesList[0]);
                    }
                    else
                    {
                        str = CheckFileName(filesList[i], i, filesList[i - 1]);
                    }
                    if (str != string.Empty)
                    {
                        throw new PumpDataFailedException(str);
                    }

                    string[] fileContent = CommonRoutines.GetFileContent(filesList[i], Encoding.GetEncoding(866));

                    fragment = GetFragment(fileContent, ref strNum, constAttrFileID);

                    CheckHeaderFragment(fragment, ref docsAmount, ref progressText, out taxCode, out fileID);

                    // Сверим наименование файла с данными из самого файла
                    CheckFileID(taxCode, fileID, filesList[i].Name);

                    // Закачиваем данные
                    int fileLength = fileContent.GetLength(0);
                    incorrectIdDocCount = 0;
                    totalIdDocCount = 0;

                    while (fragment.Count > 0)
                    {
                        fragment = GetFragment(fileContent, ref strNum, constAttrDocID);

                        // Если достигнут конец файла, то выходим
                        if (fragment.Count == 0 || fragment[0] == constAttrEndOfFile)
                        {
                            break;
                        }

                        string kd = "0";
                        string okato = "0";
                        string okved = string.Empty;
                        int statusCode = 0;
                        int taxpayerCode = 0;
                        bool docIsEmpty = true;

                        int attrCode = 0;
                        string attrValue = string.Empty;

                        if (!CheckCurrentFragment(fragment, ref docID, ref firstDoc, ref lastDoc, ref docNum,
                            processedDocsCount, currentRow, i == 0, filesList[i].Name))
                        {
                            continue;
                        }

                        if (!CheckEmptyDoc(fragment))
                        {
                            emptyDocsCount++;
                            processedDocsCount++;
                            continue;
                        }

                        bool isDropped = false;

                        // Качаем данные секции
                        for (int j = 1; j < fragment.Count; j++)
                        {
                            currentRow = strNum - fragment.Count + j;

                            ProcessAttribute(fragment[j], ref attrCode, ref attrValue);

                            switch (attrCode)
                            {
                                // формат не поддерживается 
                                case 10100:
                                case 10101:
                                case 10200:
                                case 10300:
                                case 10301:
                                case 10900:
                                case 11000:
                                case 11100:
                                case 11200:
                                case 10600:
                                case 10700:
                                    throw new Exception("Формат не поддерживается. Это формат обмена между налоговыми органами (по Приказу ФНС от 12 мая 2005 г. N ШС-3-10/201@" +
                                        "'Об утверждении Рекомендаций по порядку ведения в налоговых органах базы данных 'Расчеты с бюджетом'");
                                // ОКВЭД
                                case 10400:
                                    okved = SetOkvedMask(attrValue);
                                    break;

                                // Код налогоплательщика
                                case 10500:
                                    if (attrValue != string.Empty)
                                    {
                                        taxpayerCode = Convert.ToInt32(attrValue);

                                        // Код имеет значение от 0 до 9
                                        if (!(taxpayerCode >= 0 && taxpayerCode <= 9))
                                        {
                                            WriteToBadCodesCache(docID, attrValue);

                                            badTaxpayerCodeCount++;
                                            isDropped = true;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        emptyTaxpayerCodeCount++;
                                        taxpayerCode = 0;
                                    }
                                    break;

                                // КБК
                                case 20300:
                                    kd = attrValue;
                                    // КБК не может быть пустым
                                    if (kd == string.Empty)
                                    {
                                        WriteEventIntoDataPumpProtocol(
                                            DataPumpEventKind.dpeError, string.Format(
                                                "Пропущен документ (строка {0}, ИДДОК:{1}): некорректный КБК ({2})",
                                                currentRow, docID, kd));
                                        isDropped = true;
                                        break;
                                    }
                                    break;

                                // ОКАТО
                                case 20400:
                                    okato = attrValue;
                                    // ОКАТО тоже не может быть пустым
                                    if (okato == string.Empty)
                                    {
                                        WriteEventIntoDataPumpProtocol(
                                            DataPumpEventKind.dpeError, string.Format(
                                                "Пропущен документ (строка {0}, ИДДОК:{1}): некорректный ОКАТО ({2})",
                                                currentRow, docID, okato));
                                        isDropped = true;
                                        break;
                                    }
                                    break;

                                // Код статуса
                                case 20500:
                                    if (attrValue != string.Empty)
                                    {
                                        statusCode = Convert.ToInt32(attrValue);
                                    }
                                    else
                                    {
                                        statusCode = 0;
                                    }
                                    // Код статуса может быть 0, 1, 2
                                    if (statusCode > 2)
                                    {
                                        WriteEventIntoDataPumpProtocol(
                                            DataPumpEventKind.dpeError, string.Format(
                                                "Пропущен документ (строка {0}, ИДДОК:{1}): некорректный код статуса ({2})",
                                                currentRow, docID, statusCode));
                                        isDropped = true;
                                        break;
                                    }
                                    break;

                                // Показатели
                                default:
                                    // Если такого показателя нет среди зарегистрированных, то пишем ошибку
                                    if (attrCode < 20701 || attrCode > 50000)
                                    {
                                        WriteEventIntoDataPumpProtocol(
                                            DataPumpEventKind.dpeError, string.Format(
                                                "Пропущена строка {0} (ИДДОК:{1}): неопознанный код реквизита {2}, сумма {3}.",
                                                currentRow, docID, attrCode, attrValue));
                                        break;
                                    }

                                    if (okved == string.Empty)
                                    {
                                        // ОКВЭД может быть пуст только если Код налогоплательщика равен "02" или "03"
                                        if (!(taxpayerCode >= 2 && taxpayerCode <= 3))
                                        {
                                            emptyOkvedCount++;
                                        }
                                        okved = "0";
                                    }

                                    // Без лишних проволочек обрабатываем все показатели документа
                                    while (attrCode >= 20701 && attrCode <= 50000 && j < fragment.Count)
                                    {
                                        currentRow = strNum - fragment.Count + j;

                                        marksCount++;

                                        PumpFactData(fileDate, kd, okato, okved, statusCode, taxpayerCode,
                                            docID, currentRow, attrCode, attrValue, ref docIsEmpty);

                                        j++;
                                        if (j < fragment.Count)
                                        {
                                            ProcessAttribute(fragment[j], ref attrCode, ref attrValue);
                                        }
                                        else break;
                                    }
                                    j--;

                                    break;
                            }

                            if (isDropped)
                            {
                                droppedDocsCount++;
                                break;
                            }
                        }

                        // Счетчик пустых документов
                        if (docIsEmpty) emptyDocsCount++;
                        // Счетчик всех документов
                        processedDocsCount++;
                        processedDocs++;
                    }

                    if (incorrectIdDocCount > 0)
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeError, string.Format(
                            "Ошибка при закачке файла {0}: год отчета не совпадает во всех вхождениях " +
                            "(количество необработанных документов: {1}, всего документов: {2})",
                            filesList[i].Name, incorrectIdDocCount, totalIdDocCount));

                    // Если в файле не нашли признака конца файла, то пишем в протокол ошибку (не критическую)
                    if (fragment.Count == 0)
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Не найден признак конца файла.");

                    WriteBadTaxpayerCodesCacheToBD();

                    // Запись в протокол итогов закачки файла
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                         "Закачка из файла {0} успешно завершена. \nПервый документ: {1}, последний документ: {2}. " +
                         "Обработано документов: {3} (из них пропущено {4}, пустых {5}). " +
                         "Найдено пустых кодов налогоплательщика: {6}. Пустые коды заменены на 0. " +
                         "Найдено пустых ОКВЭД: {7}. Пустые ОКВЭД заменены на 0. " +
                         "Некорректных кодов налогоплательщика: {8}.",
                         filesList[i].Name, firstDoc, lastDoc, processedDocsCount, droppedDocsCount, emptyDocsCount,
                         emptyTaxpayerCodeCount, emptyOkvedCount, badTaxpayerCodeCount));
                }
                catch (ThreadAbortException)
                {
                    WriteBadTaxpayerCodesCacheToBD();

                    // Запись в протокол итогов закачки файла
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, string.Format(
                        "Закачка из файла {0} прервана пользователем. \nНа момент прерывания достигнуты следующие результаты. " +
                        "Первый документ: {1}, последний документ: {2}. " +
                        "Обработано документов: {3} (из них пропущено {4}, пустых {5}). " +
                        "Найдено пустых кодов налогоплательщика: {6}. Пустые коды заменены на 0. " +
                        "Найдено пустых ОКВЭД: {7}. Пустые ОКВЭД заменены на 0. " +
                        "Некорректных кодов налогоплательщика: {8}. " +
                        "Закачка прервана при обработке документа {9}, строка {10}.",
                        filesList[i].Name, firstDoc, lastDoc, processedDocsCount, droppedDocsCount, emptyDocsCount,
                        emptyTaxpayerCodeCount, emptyOkvedCount, badTaxpayerCodeCount, docID, currentRow));

                    throw;
                }
                catch (Exception ex)
                {
                    WriteBadTaxpayerCodesCacheToBD();

                    // Запись в протокол итогов закачки файла
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                        "Закачка из файла {0} завершена с ошибками. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Первый документ: {1}, последний документ: {2}. " +
                        "Обработано документов: {3} (из них пропущено {4}, пустых {5}). " +
                        "Найдено пустых кодов налогоплательщика: {6}. Пустые коды заменены на 0. " +
                        "Найдено пустых ОКВЭД: {7}. " +
                        "Некорректных кодов налогоплательщика: {8}. Данные не сохранены.",
                        filesList[i].Name, firstDoc, lastDoc, processedDocsCount, droppedDocsCount,
                        emptyDocsCount, emptyTaxpayerCodeCount, emptyOkvedCount, badTaxpayerCodeCount), ex);

                    throw;
                }
                finally
                {
                    droppedDocs += droppedDocsCount;
                    emptyDocs += emptyDocsCount;

                    CollectGarbage();
                }
            }
        }

        #endregion закачка 28n

        #region закачка 65n

        private bool CheckFileName(string fileName)
        {
            int month = Convert.ToInt32(fileName.Substring(15, 2));
            int year = 2000 + Convert.ToInt32(fileName.Substring(17, 2));
            int date = year * 10000 + month * 100;
            bool result = CheckDataSourceByDate(date, false);
            if (!result)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Файл {0} не соответствует параметрам источника, его данные закачаны не будут.", fileName));
            return result;
        }

        private int GetRefStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return 0;
                case 1:
                    return 99;
                case 2:
                    return 2;
                case 3:
                    return 3;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 6:
                    return 6;
                case 7:
                    return 7;
                case 8:
                    return 8;
                case 9:
                    return 9;
                case 10:
                    return 10;
                case 11:
                    return 11;
                case 12:
                    return 12;
                case 13:
                    return 13;
                case 14:
                    return 14;
                case 15:
                    return 15;
                case 16:
                    return 16;
                case 17:
                    return 17;
                case 18:
                    return 18;
                case 19:
                    return 19;
                case 20:
                    return 20;
                case 21:
                    return 21;
                case 22:
                    return 22;
                default:
                    return -1;
            }
        }

        private int GetRefCode(int code)
        {
            switch (code)
            {
                case 0:
                    return 0;
                case 16:
                    return 16;
                case 17:
                    return 7;
                case 18:
                    return 8;
                case 19:
                    return 9;
                default:
                    return -1;
            }
        }

        private void Pump65nRow(object[] mapping)
        {
            // ключ синтетический - все ссылки на показатели
            // RefOKVEDA28N + RefTaxPayerStatusCodes + RefTaxPayerCodes + RefKDA28N + RefOKTMO + RefOKATOA28N + RefDataMarks65n
            string cacheKey = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", 
                mapping[1], mapping[3], mapping[5], mapping[9], mapping[11], mapping[13], mapping[15] );
            // если записи в такой разрезности нет - добавляем в кэш, если есть - суммируем
            if (fact65nCache.ContainsKey(cacheKey))
            {
                fact65nCache[cacheKey][0] += Convert.ToDecimal(mapping[17]);
                fact65nCache[cacheKey][1] += 1;
            }
            else
            {
                fact65nCache.Add(cacheKey, new decimal[] { Convert.ToDecimal(mapping[17]), Convert.ToDecimal(mapping[19]) });
            }
        }

        private void Update65nData()
        {
            foreach (KeyValuePair<string, decimal[]> cacheItem in fact65nCache)
            {
                string[] clsRefs = cacheItem.Key.Split('|');
                decimal[] sumInfo = cacheItem.Value;
                int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
                object[] mapping = new object[] { "RefOKVEDA28N", clsRefs[0], "RefTaxPayerStatusCodes", clsRefs[1], 
                            "RefTaxPayerCodes", clsRefs[2], "RefYearDayUNV", refDate, "RefKDA28N", clsRefs[3], 
                            "RefOKTMO", clsRefs[4], "RefOKATOA28N", clsRefs[5], "RefDataMarks65n", clsRefs[6], 
                            "Summe", sumInfo[0], "DocCount", sumInfo[1], "RefIsDisint", 0, "RefDataMarks28n", 0 };
                PumpRow(dsDirtyUMNS28n.Tables[0], mapping);
                if (dsDirtyUMNS28n.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
                {
                    UpdateData();
                    ClearDataSet(daDirtyUMNS28n, ref dsDirtyUMNS28n);
                }
            }
            UpdateData();
        }

        // загружаем лист протокола
        private void PumpProtocol(object sheet)
        {
            isCheckProtocol = true;
            totalUnloadedFiles = 0;
            totalLoadedFiles = 0;
            totalUnloadedDocs = 0;
            totalLoadedDocs = 0;
            controlIncomeTax = 0.0M;
            totalIncomeTax = 0.0M;
            controlAccrualTax = 0.0M;
            totalAccrualTax = 0.0M;
            logs.Clear();

            bool isStart = false;
            for (int curRow = 1; curRow < 65536; curRow++)
            {
                string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                if (cellValue == string.Empty)
                    continue;

                if (cellValue.ToUpper().StartsWith("ВСЕГО ВЫГРУЖЕНО ДОКУМЕНТОВ ДЛЯ ФИНАНСОВЫХ ОРГАНОВ"))
                {
                    totalUnloadedDocs += Convert.ToInt32(cellValue.Split(':')[1].Trim());
                    isStart = false;
                }

                if (isStart)
                {
                    curRow++;
                    string docsCount = excelHelper.GetCell(sheet, curRow, 1).Value.Split(':')[1].Trim();
                    logs.Add(cellValue, Convert.ToInt32(docsCount));
                    continue;
                }

                if (cellValue.ToUpper().StartsWith("КОЛИЧЕСТВО ВЫГРУЖЕННЫХ ФАЙЛОВ ДЛЯ ФИНАНСОВЫХ ОРГАНОВ"))
                {
                    totalUnloadedFiles += Convert.ToInt32(cellValue.Split(':')[1].Trim());
                    isStart = true;
                }

                if (cellValue.ToUpper().StartsWith("ПРОВЕДЕНА СВЕРКА РЕКВИЗИТОВ"))
                {
                    curRow += 3;
                    string temp = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    controlAccrualTax = Convert.ToDecimal(temp);
                    temp = excelHelper.GetCell(sheet, curRow, 3).Value.Trim();
                    controlIncomeTax = Convert.ToDecimal(temp);
                    return;
                }
            }
        }

        // открываем Excel-файл с протоколом
        private void PumpProtocolFile(FileInfo file)
        {
            // Если имя файла начинается со слова "ПРОТОКОЛ",
            // то закачивать его не надо, т.к. это выгружаемый протокол
            if (file.Name.ToUpper().StartsWith("ПРОТОКОЛ"))
                return;

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            object excelObj = excelHelper.OpenExcel(false);
            try
            {
                object workbook = excelHelper.GetWorkbook(excelObj, file.FullName, true);
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpProtocol(sheet);
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.SetDisplayAlert(excelObj, false);
                    excelHelper.CloseWorkBooks(excelObj);
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Close();
                }
                GC.GetTotalMemory(true);
            }
        }

        #region константы
        private const string INF_TYPE_MARK = "ТИПИНФ";
        private const string INFORM_MARK = "ИНФОРМ_МАССИВ";
        private const string VERSION_MARK = "ВЕРСФОРМ";
        private const string DOC_START_MARK = "###";
        private const string DOC_END_MARK = "@@@";
        private const string OKVED_MARK = "10100";
        private const string PLAT_STAT_MARK = "10200";
        private const string PLAT_CODE_MARK = "10300";
        private const string REPORT_END_MARK = "===";
        private const string OKATO_MARK = "20100";
        private const string OKTMO_MARK = "20200";
        private const string KD_MARK = "20300";
        private const string ID_DOC_MARK = "ИДДОК";
        private const string SENDER_MARK = "НАИМОТПР";
        private const string SENDER_RESP_MARK = "ФИООТПР";
        private const string SENDER_EMPL_MARK = "ДОЛЖНОТПР";
        private const string SENDER_PHONE_MARK = "ТЕЛОТПР";
        private const string DESTINEE_MARK = "ИДПОЛ";
        private const string FILE_ID_MARK = "ИДФАЙЛ";
        private const string KOL_DOC_MARK = "КОЛДОК";
        #endregion константы
        private void Pump65nFile(FileInfo file)
        {
            if (!CheckFileName(file.Name))
                return;

            ProtocolRow protocolRow = new ProtocolRow();
            protocolRow.fileName = file.Name.Substring(0, file.Name.LastIndexOf('.'));
            protocolRow.fileSize = file.Length;
            protocolRow.countDocs = 0;
            // пока пишем 0, если закачка файла пройдёт нормально,
            // сюда поместим значение поля protocolRow.countDocs
            protocolRow.countPumpedDocs = 0;
            
            bool toPumpRow = false;
            int rowIndex = 0;
            object[] mapping = null;
            string okved = "-1";
            int refOkved = -1;
            int status = 0;
            int code = 0;
            int refStatus = 0;
            int refCode = 0;
            int refOkato = nullOkato;
            int refOktmo = nullOktmo;
            int refKd = -1;
            int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            bool isIncorrectDocument = false;
            string docDescription = string.Empty;
            bool toPumpFact = true;

            totalLoadedFiles++;
            int countDocsForCurFile = 0;

            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            foreach (string row in reportData)
            {
                rowIndex++;
                string[] data = row.Split(':');
                string value = data[0].Trim().ToUpper();

                if (value == REPORT_END_MARK)
                    break;

                switch (value)
                {
                    case ID_DOC_MARK:
                        docDescription = data[1];
                        isIncorrectDocument = false;
                        totalLoadedDocs++;
                        countDocsForCurFile++;
                        continue;
                    case INF_TYPE_MARK:
                        if (data[1].Trim().ToUpper() != INFORM_MARK)
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("Реквизит «ТипИнф» должен иметь значение «ИНФОРМ_МАССИВ», файл {0} не будет закачан.", file.FullName));
                            return;
                        }
                        protocolRow.infoType = data[1].Trim();
                        continue;
                    case VERSION_MARK:
                        string version = data[1].Trim();
                        if (version != "3.02")
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("Версия файла: {0}", version));
                        continue;
                    case OKVED_MARK:
                        okved = data[1].Trim();
                        continue;
                    case PLAT_STAT_MARK:
                        status = Convert.ToInt32(data[1].Trim());
                        continue;
                    case PLAT_CODE_MARK:
                        code = Convert.ToInt32(data[1].Trim());
                        continue;
                    case DOC_END_MARK:
                        toPumpRow = false;
                        continue;
                    case DOC_START_MARK:
                        if (toPumpRow)
                            continue;

                        refStatus = GetRefStatus(status);
                        if (refStatus == -1)
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("Статус плательщика имеет значение: {0}", status));
                            isIncorrectDocument = true;
                        }

                        refCode = GetRefCode(code);
                        if (refCode == -1)
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("Код налогоплательщиков имеет значение: {0}", code));
                            isIncorrectDocument = true;
                        }

                        if ((okved == "-1") && !(new int[] { 0, 3, 4, 5, 6, 7, 8, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22 }.Contains(status)))
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "ОКВЭД не определен.");
                            isIncorrectDocument = true;
                        }

                        if (okved == "-1")
                            okved = "0";
                        okved = okved.Replace(".", string.Empty).PadRight(6, '0');
                        refOkved = PumpCachedRow(okvedCache, dsOKVED.Tables[0], clsOKVED,
                            okved, new object[] { "CodeStr", okved });

                        if (isIncorrectDocument)
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("Документ ({0}) является неккоректным. Закачан не будет.", docDescription));
                            incorrectDocCount++;
                        }

                        refOkato = nullOkato;
                        refOktmo = nullOktmo;
                        okved = "-1";
                        continue;
                    case OKATO_MARK:
                        string okato = data[1].Trim().TrimStart('0').PadLeft(1, '0');
                        refOkato = PumpCachedRow(okatoCache, dsOKATO.Tables[0], clsOKATO, okato, new object[] { "Code", okato });
                        toPumpRow = false;
                        continue;
                    case OKTMO_MARK:
                        string oktmo = data[1].Trim().TrimStart('0').PadLeft(1, '0');
                        refOktmo = PumpCachedRow(oktmoCache, dsOktmo.Tables[0], clsOktmo, oktmo, new object[] { "Code", oktmo });
                        toPumpRow = false;
                        oktmoPresence = true;
                        continue;
                    case KD_MARK:
                        string kd = data[1].Trim().TrimStart('0').PadLeft(1, '0');
                        // Добавляем код дохода в классификатор данных (если еще не добавлен)
                        bool kdPresence = (kdMarkCache.ContainsKey(kd));
                        if (!kdPresence)
                            kdMarkCache.Add(kd, mark);
                        refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kd, new object[] { "CodeStr", kd, "RefSignKD28n", mark });
                        toPumpFact = true;
                        // если данные по этому кд уже есть,
                        // причем признак меньше, чем закачиваемый, удаляем соответственные записи факта и меняем признак у кд
                        if ((kdPresence) && (mark != 0))
                        {
                            int kdMark = kdMarkCache[kd];
                            // если данные уже есть по меньшему признаку, в факт записи не качаем
                            toPumpFact = (mark <= kdMark);
                            // если данные закачиваются с меньшим признаком, чем признак уже закачанных данных - удаляем существующие
                            if (kdMark > mark)
                            {
                                DeleteKDData(refKd, mark);
                                kdMarkCache[kd] = mark;
                            }
                        }
                        // со следующей строки начинаем закачку факта
                        toPumpRow = true;
                        continue;
                    case "30110":
                        totalAccrualTax += Convert.ToDecimal(data[1].Trim().PadLeft(1, '0').Replace('.', ','));
                        break;
                    case "30210":
                        totalIncomeTax += Convert.ToDecimal(data[1].Trim().PadLeft(1, '0').Replace('.', ','));
                        break;
                    case SENDER_MARK:
                        if (data[1].Trim() == "3500")
                            senderName = "Управление ФНС России по области";
                        else
                            senderName = data[1].Trim();
                        continue;
                    case SENDER_RESP_MARK:
                        senderResponsible = data[1].Trim();
                        continue;
                    case SENDER_EMPL_MARK:
                        senderEmployment = data[1].Trim();
                        continue;
                    case SENDER_PHONE_MARK:
                        senderPhone = data[1].Trim();
                        continue;
                    case DESTINEE_MARK:
                        if (data[1].Trim().ToUpper() == "ВОЛОГОДСКАЯ ОБЛАСТЬ")
                            destineeName = "Департамент финансов Вологодской области";
                        else
                            destineeName = data[1].Trim();
                        continue;
                    case FILE_ID_MARK:
                        protocolRow.fileId = data[1].Trim();
                        continue;
                    case KOL_DOC_MARK:
                        protocolRow.countDocs = Convert.ToInt32(data[1].Trim().PadLeft(1, '0'));
                        continue;
                    case "30100":
                    case "30200":
                    case "30300":
                    case "40100":
                    case "40600":
                    case "40700":
                    case "40800":
                    case "50000":
                        continue;
                }
                // закачиваем только те показатели, которые есть в фиксированном классификаторе
                if (!cache65nMarks.ContainsKey(value))
                    continue;

                if ((toPumpRow) && (!isIncorrectDocument) && (toPumpFact))
                {
                    decimal sum = Convert.ToDecimal(data[1].Trim().PadLeft(1, '0').Replace('.', ','));
                    mapping = new object[] { "RefOKVEDA28N", refOkved, "RefTaxPayerStatusCodes", refStatus, 
                        "RefTaxPayerCodes", refCode, "RefYearDayUNV", refDate, "RefKDA28N", refKd, 
                        "RefOKTMO", refOktmo, "RefOKATOA28N", refOkato, "RefDataMarks65n", value, 
                        "Summe", sum, "DocCount", 1, "RefIsDisint", 0, "RefDataMarks28n", 0 };
                    Pump65nRow(mapping);
                }
            }

            protocolRow.totalAccrualTax = totalAccrualTax;
            protocolRow.totalIncomeTax = totalIncomeTax;
            protocolRow.countPumpedDocs = protocolRow.countDocs;
            protocolRows.Add(protocolRow);

            if (isCheckProtocol && (logs[file.Name] != countDocsForCurFile))
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Фактическое количество документов {0} в файле \"{1}\" не соответствует количеству в протоколе {2}.",
                    countDocsForCurFile, file.Name, logs[file.Name]));

            UpdateData();
        }

        private void DoCheckProtocol()
        {
            if (totalLoadedFiles != totalUnloadedFiles)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Фактическое количество закачанных файлов {0} не соответствует количеству в протоколе {1}.",
                    totalLoadedFiles, totalUnloadedFiles));
            if (totalLoadedDocs != totalUnloadedDocs)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Общее количество закачанных документов {0} не соответствует количеству документов в протоколе {1}.",
                    totalLoadedDocs, totalUnloadedDocs));
            if (totalAccrualTax != controlAccrualTax)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Сумма начисленного налога {0} для финансовых органов субъекта РФ не сходится с контрольной суммой {1} в протоколе.",
                    totalAccrualTax, controlAccrualTax));
            if (totalIncomeTax != controlIncomeTax)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Сумма поступившего налога {0} для финансовых органов субъекта РФ не сходится с контрольной суммой {1} в протоколе.",
                    totalIncomeTax, controlIncomeTax));
        }

        private void Process65nFiles(DirectoryInfo dir)
        {
            oktmoPresence = false;
            fact65nCache = new Dictionary<string, decimal[]>();
            try
            {
                // если ранее закачанные данные не удаляются, то сверку делать не надо,
                // соответственно не надо загружать и xls-файлы протоколов
                if (this.DeleteEarlierData)
                    ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpProtocolFile), false);
                ProcessFilesTemplate(dir, "*.txt", new ProcessFileDelegate(Pump65nFile), false);
                if (isCheckProtocol)
                    DoCheckProtocol();
                Update65nData();
                
                // формирование протокола выгрузки
                if (this.Region == RegionName.Vologda)
                {
                    BuildProtocol(dir);
                }
            }
            finally
            {
                fact65nCache.Clear();
            }
            if (oktmoPresence)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "В отчетах встретился реквизит ОКТМО «20200». Расщепление по этому ОКТМО произведено не будет.");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                string.Format("Обработано {0} документов, из них {1} являются некорректными.", totalLoadedDocs, incorrectDocCount));
        }

        #endregion закачка 65n

        #region перекрытые методы закачек

        private void ProcessDirectories(DirectoryInfo dir)
        {
            // Если есть архивы, то сначала распаковываем их во временную директорию
            DirectoryInfo tempDir = CommonRoutines.GetTempDir();

            FileInfo[] files = dir.GetFiles("*.arj");
            foreach (FileInfo archFile in files)
                CommonRoutines.ExtractArchiveFile(archFile.FullName, tempDir.FullName,
                    ArchivatorName.Arj, FilesExtractingOption.SingleDirectory);

            files = dir.GetFiles("*.a01");
            foreach (FileInfo archFile in files)
                CommonRoutines.ExtractArchiveFile(archFile.FullName, tempDir.FullName,
                    ArchivatorName.Arj, FilesExtractingOption.SingleDirectory);

            files = dir.GetFiles("*.rar");
            foreach (FileInfo rarFile in files)
                CommonRoutines.ExtractArchiveFile(rarFile.FullName, tempDir.FullName,
                    ArchivatorName.Rar, FilesExtractingOption.SingleDirectory);
            // Если есть разархивированные файлы, копируем их во временную директорию, чтобы там находился полный
            // список обрабатываемых файлов
            files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if ((file.Extension.Substring(0, 2).ToUpper() == ".A") || (file.Extension.ToUpper() == ".RAR"))
                    continue;
                file.CopyTo(string.Format("{0}\\{1}", tempDir.FullName, file.Name), true);
            }

            try
            {
                ProcessDir28n(tempDir);
                Process65nFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private const string SUBJ_DIR_NAME = "1_Субъект";
        private const string MR_DIR_NAME = "2_МР и ГО";
        private const string SETTLE_DIR_NAME = "3_Поселения";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            // если подпапок нет, качаем с признаком 0
            if (dir.GetDirectories().GetLength(0) == 0)
            {
                mark = 0;
                ProcessDirectories(dir);
                return;
            }
            // обходим подпапки субъекта, МР и поселений
            DirectoryInfo[] subDirs = dir.GetDirectories(SUBJ_DIR_NAME);
            if (subDirs.GetLength(0) != 0)
            {
                mark = 1;
                ProcessDirectories(subDirs[0]);
            }
            subDirs = dir.GetDirectories(MR_DIR_NAME);
            if (subDirs.GetLength(0) != 0)
            {
                mark = 2;
                ProcessDirectories(subDirs[0]);
            }
            subDirs = dir.GetDirectories(SETTLE_DIR_NAME);
            if (subDirs.GetLength(0) != 0)
            {
                mark = 3;
                ProcessDirectories(subDirs[0]);
            }
        }

        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.*", SearchOption.AllDirectories).GetLength(0);
            PumpDataYMTemplate();
        }

        #endregion перекрытые методы закачек

        #region Выгрузка протокола

        private string GetFioFromFullname(string[] fullname)
        {
            string fio = fullname[0];
            if (fullname[1].Length > 0)
                fio = string.Format("{0} {1}.", fio, fullname[1].Substring(0, 1));
            if (fullname[2].Length > 0)
                fio = string.Format("{0} {1}.", fio, fullname[2].Substring(0, 1));
            return fio;
        }

        private void GenerateProtocol(ExcelHelper protocol, DirectoryInfo dir)
        {
            object excelObj = protocol.OpenExcel(false);
            object workbook = protocol.CreateWorkbook(excelObj);

            try
            {
                object sheet = protocol.GetSheet(workbook, 1);

                int month = this.DataSource.Month;
                int year = this.DataSource.Year;

                protocol.SetCell(sheet, 1, 1, "Протокол форматно-логического контроля");

                protocol.SetCell(sheet, 3, 1, string.Format("Протокол №{0:00} от {1:dd.MM.yyyy}", month, DateTime.Now));

                // инфа об отправителе
                protocol.SetCell(sheet, 5, 1, string.Format("Отправитель: {0}", senderName));
                protocol.SetCell(sheet, 6, 1, string.Format("ФИО ответственного лица: {0}", senderResponsible));
                protocol.SetCell(sheet, 7, 1, string.Format("Должность ответственного лица: {0}", senderEmployment));
                protocol.SetCell(sheet, 8, 1, string.Format("Телефон ответственного лица: {0}", senderPhone));

                // берём информацию о получателе
                // string userName = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
                string queryText = string.Format("select LASTNAME, FIRSTNAME, PATRONYMIC, JOBTITLE from USERS where NAME = '{0}'", this.userName);
                DataTable dt = (DataTable)this.DB.ExecQuery(queryText, QueryResultTypes.DataTable);

                // инфа о получателе
                if (dt.Rows.Count > 0)
                {
                    DataRow user = dt.Rows[0];
                    protocol.SetCell(sheet, 10, 1, string.Format("Получатель: {0}", destineeName));
                    protocol.SetCell(sheet, 11, 1, string.Format("ФИО ответственного лица: {0} {1} {2}", user[0], user[1], user[2]));
                    protocol.SetCell(sheet, 12, 1, string.Format("Должность ответственного лица: {0}", user[3]));
                    protocol.SetCell(sheet, 13, 1, "Телефон ответственного лица:");

                }
                else
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Сведения о получателе не найдены");
                    protocol.SetCell(sheet, 10, 1, string.Format("Получатель: {0}", destineeName));
                    protocol.SetCell(sheet, 11, 1, "ФИО ответственного лица:");
                    protocol.SetCell(sheet, 12, 1, "Должность ответственного лица:");
                    protocol.SetCell(sheet, 13, 1, "Телефон ответственного лица:");
                }
                
                // основная часть протокола
                protocol.SetCell(sheet, 15, 1, "Имя файла");
                protocol.SetCell(sheet, 15, 2, "Идентификатор файла");
                protocol.SetCell(sheet, 15, 3, "Тип информации");
                protocol.SetCell(sheet, 15, 4, "Размер принимаемого файла, Кб");
                protocol.SetCell(sheet, 15, 5, "Количество документов, заявленных отправителем");
                protocol.SetCell(sheet, 15, 6, "Количество обработанных документов при форматном контроле");
                protocol.SetCell(sheet, 15, 7, "Количество документов, прошедших форматный контроль");
                protocol.SetCell(sheet, 15, 8, "Количество документов, не прошедших форматный контроль");
                protocol.SetCell(sheet, 15, 9, "Количество документов, готовых к приему");
                protocol.SetCell(sheet, 15, 10, "Результат ФЛК");

                decimal totalAccrual = 0.0M;
                decimal totalIncome = 0.0M;

                int numRow = 16;
                foreach (ProtocolRow row in protocolRows)
                {
                    protocol.SetCell(sheet, numRow, 1, row.fileName);
                    protocol.SetCell(sheet, numRow, 2, row.fileId);
                    protocol.SetCell(sheet, numRow, 3, row.infoType);
                    protocol.SetCell(sheet, numRow, 4, (row.fileSize / 1000).ToString());
                    protocol.SetCell(sheet, numRow, 5, row.countDocs.ToString());
                    protocol.SetCell(sheet, numRow, 6, row.countDocs.ToString());
                    protocol.SetCell(sheet, numRow, 7, row.countPumpedDocs.ToString());
                    protocol.SetCell(sheet, numRow, 8, (row.countDocs - row.countPumpedDocs).ToString());
                    protocol.SetCell(sheet, numRow, 9, row.countPumpedDocs.ToString());
                    if (row.countPumpedDocs > 0)
                        protocol.SetCell(sheet, numRow, 10, "ошибок в документах нет");
                    else
                        protocol.SetCell(sheet, numRow, 10, "обнаружены ошибки при форматно-логическом контроле");

                    totalAccrual += row.totalAccrualTax;
                    totalIncome += row.totalIncomeTax;
                    numRow++;
                }

                // вывод итоговых сумм
                protocol.SetCell(sheet, numRow + 2, 1, string.Format("Всего начислено налога: {0}", totalAccrual));
                protocol.SetCell(sheet, numRow + 3, 1, string.Format("Всего поступило налога: {0}", totalIncome));

                // подписи получателя и отправителя
                protocol.SetCell(sheet, numRow + 6, 2, "Ответственное лицо получателя");
                protocol.SetCell(sheet, numRow + 6, 7, GetFioFromFullname(senderResponsible.Split(',')));
                protocol.SetCell(sheet, numRow + 7, 5, "(подпись)");
                protocol.SetCell(sheet, numRow + 7, 7, "(ФИО)");

                protocol.SetCell(sheet, numRow + 9, 2, "Ответственное лицо отправителя");
                if (dt.Rows.Count > 0)
                {
                    DataRow user = dt.Rows[0];
                    protocol.SetCell(sheet, numRow + 9, 7, GetFioFromFullname(new string[] { user[0].ToString(), user[1].ToString(), user[2].ToString() }));
                }
                protocol.SetCell(sheet, numRow + 10, 5, "(подпись)");
                protocol.SetCell(sheet, numRow + 10, 7, "(ФИО)");

                string fileName = string.Format("Протокол_{0:00}_{1}.xls", month, year);
                fileName = string.Format("{0}\\{1}\\{2:00}\\{3}", this.RootDir, year, month, fileName);

                protocol.SaveChanges(workbook, fileName);
            }
            finally
            {
                protocol.CloseExcel(ref excelObj);
            }
        }

        private void BuildProtocol(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            ExcelHelper protocol = new ExcelHelper();
            try
            {
                GenerateProtocol(protocol, dir);
            }
            finally
            {
                if (protocol != null)
                    protocol.Close();
            }
        }

        #endregion Выгрузка протокола

        #endregion Закачка данных

        #region Обработка данных

        #region расчет итоговых сумм - для 28 н

        private string GetCacheKey(DataRow row, string[] refsCls)
        {
            string key = string.Empty;
            foreach (string clsRef in refsCls)
            {
                string refValue = row[clsRef].ToString();
                key += string.Format("{0}|", refValue);
            }
            key = key.Remove(key.Length - 1);
            return key;
        }

        // получить кэш факта, ключ - синтетический, состоит из всех ссылок на классификаторы
        // значение - список пар "показатель 28 н - сумма по нему"
        private Dictionary<string, Dictionary<int, decimal>> GetFactCache(IFactTable fct, int totalRecs)
        {
            Dictionary<string, Dictionary<int, decimal>> factCache = new Dictionary<string,Dictionary<int,decimal>>();
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}  and RefDataMarks28n <> 0",
                fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2} and RefDataMarks28n <> 0",
                    firstID, lastID, this.SourceID);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    string cacheKey = GetCacheKey(row, new string[] { "RefOKVEDA28N", "RefTaxPayerStatusCodes", 
                        "RefTaxPayerCodes", "RefYearDayUNV", "RefKDA28N", "RefOKATOA28N" });
                    int refMarks28n = Convert.ToInt32(row["RefDataMarks28n"]);
                    decimal sum = Convert.ToDecimal(row["Summe"]);
                    if (!factCache.ContainsKey(cacheKey))
                    {
                        Dictionary<int, decimal> factCacheValue = new Dictionary<int, decimal>();
                        factCache.Add(cacheKey, factCacheValue);
                    }
                    if (factCache[cacheKey].ContainsKey(refMarks28n))
                        factCache[cacheKey][refMarks28n] += sum;
                    else
                        factCache[cacheKey].Add(refMarks28n, sum);
                }
                UpdateDataSet(da, ds, fct);
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            return factCache;
        }

        private decimal GetCacheSum(Dictionary<int, decimal> cache, int key)
        {
            if (cache.ContainsKey(key))
                return cache[key];
            else 
                return 0;
        }

        private void Add28nTotalSum(DataTable dt, string[] refCls, int refMarks28n, decimal sum)
        {
            DataRow row = dt.NewRow();
            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["RefOKVEDA28N"] = refCls[0];
            row["RefTaxPayerStatusCodes"] = refCls[1];
            row["RefTaxPayerCodes"] = refCls[2];
            row["RefYearDayUNV"] = refCls[3];
            row["RefKDA28N"] = refCls[4];
            row["RefOKATOA28N"] = refCls[5];
            row["RefDataMarks28n"] = refMarks28n;
            row["REFISDISINT"] = 0;
            row["DocCount"] = 0;
            row["SUMME"] = sum;
            dt.Rows.Add(row);
        }

        private void DeleteTotalSums(string dbName)
        {
            this.DB.ExecQuery(string.Format("delete from {0} where SOURCEID = {1} and " +
                "(RefDataMarks28n = 910001 or RefDataMarks28n = 910002 or RefDataMarks28n = 910003 or RefDataMarks28n = 910004 or" +
                " RefDataMarks28n = 930001 or RefDataMarks28n = 930002 or RefDataMarks28n = 930003)",
                dbName, this.SourceID), QueryResultTypes.Scalar);
        }

        private void Add28nTotalSums()
        {
            // итоговые суммы удаляем
            DeleteTotalSums(fctDirtyUMNS28n.FullDBName);
            DeleteTotalSums(fctUMNS28n.FullDBName);

            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1} and RefDataMarks28n <> 0",
                fctDirtyUMNS28n.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return;
            string semantic = fctDirtyUMNS28n.FullCaption;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                string.Format("Старт расчета итоговых сумм у {0}.", semantic));
            Dictionary<string, Dictionary<int, decimal>> factCache = GetFactCache(fctDirtyUMNS28n, totalRecs);
            try
            {
                foreach (KeyValuePair<string, Dictionary<int, decimal>> cacheItem in factCache)
                {
                    string[] refCls = cacheItem.Key.Split('|');

                    if (cacheItem.Value.ContainsKey(20701) || cacheItem.Value.ContainsKey(20801) ||
                        cacheItem.Value.ContainsKey(20901))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 910001, GetCacheSum(cacheItem.Value, 20701) +
                            GetCacheSum(cacheItem.Value, 20801) - GetCacheSum(cacheItem.Value, 20901));

                    if (cacheItem.Value.ContainsKey(20702) || cacheItem.Value.ContainsKey(20802) ||
                        cacheItem.Value.ContainsKey(20902))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 910002, GetCacheSum(cacheItem.Value, 20702) +
                            GetCacheSum(cacheItem.Value, 20802) - GetCacheSum(cacheItem.Value, 20902));

                    if (cacheItem.Value.ContainsKey(20703) || cacheItem.Value.ContainsKey(20803) ||
                        cacheItem.Value.ContainsKey(20903))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 910003, GetCacheSum(cacheItem.Value, 20703) +
                            GetCacheSum(cacheItem.Value, 20803) - GetCacheSum(cacheItem.Value, 20903));

                    if (cacheItem.Value.ContainsKey(20704) || cacheItem.Value.ContainsKey(20804))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 910004, GetCacheSum(cacheItem.Value, 20704) +
                            GetCacheSum(cacheItem.Value, 20804));

                    if (cacheItem.Value.ContainsKey(30700) || cacheItem.Value.ContainsKey(31201) ||
                        cacheItem.Value.ContainsKey(31301) || cacheItem.Value.ContainsKey(31601) ||
                        cacheItem.Value.ContainsKey(31701) || cacheItem.Value.ContainsKey(31801) ||
                        cacheItem.Value.ContainsKey(31901) || cacheItem.Value.ContainsKey(32001) ||
                        cacheItem.Value.ContainsKey(32101))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 930001, GetCacheSum(cacheItem.Value, 30700) +
                            GetCacheSum(cacheItem.Value, 31201) + GetCacheSum(cacheItem.Value, 31301) +
                            GetCacheSum(cacheItem.Value, 31601) + GetCacheSum(cacheItem.Value, 31701) +
                            GetCacheSum(cacheItem.Value, 31801) + GetCacheSum(cacheItem.Value, 31901) +
                            GetCacheSum(cacheItem.Value, 32001) + GetCacheSum(cacheItem.Value, 32101));

                    if (cacheItem.Value.ContainsKey(31000) || cacheItem.Value.ContainsKey(31100) ||
                        cacheItem.Value.ContainsKey(31202) || cacheItem.Value.ContainsKey(31302) ||
                        cacheItem.Value.ContainsKey(31602) || cacheItem.Value.ContainsKey(31603) ||
                        cacheItem.Value.ContainsKey(31702) || cacheItem.Value.ContainsKey(31703) ||
                        cacheItem.Value.ContainsKey(31802) || cacheItem.Value.ContainsKey(31803) ||
                        cacheItem.Value.ContainsKey(31902) || cacheItem.Value.ContainsKey(31903) ||
                        cacheItem.Value.ContainsKey(32002) || cacheItem.Value.ContainsKey(32003) ||
                        cacheItem.Value.ContainsKey(32102) || cacheItem.Value.ContainsKey(32103))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 930002, GetCacheSum(cacheItem.Value, 31000) +
                            GetCacheSum(cacheItem.Value, 31100) + GetCacheSum(cacheItem.Value, 31202) +
                            GetCacheSum(cacheItem.Value, 31302) + GetCacheSum(cacheItem.Value, 31602) +
                            GetCacheSum(cacheItem.Value, 31603) + GetCacheSum(cacheItem.Value, 31702) +
                            GetCacheSum(cacheItem.Value, 31703) + GetCacheSum(cacheItem.Value, 31802) +
                            GetCacheSum(cacheItem.Value, 31803) + GetCacheSum(cacheItem.Value, 31902) +
                            GetCacheSum(cacheItem.Value, 31903) + GetCacheSum(cacheItem.Value, 32002) +
                            GetCacheSum(cacheItem.Value, 32003) + GetCacheSum(cacheItem.Value, 32102) +
                            GetCacheSum(cacheItem.Value, 32103));

                    if (cacheItem.Value.ContainsKey(30900) || cacheItem.Value.ContainsKey(31203) ||
                        cacheItem.Value.ContainsKey(31303) || cacheItem.Value.ContainsKey(31401) ||
                        cacheItem.Value.ContainsKey(31501) || cacheItem.Value.ContainsKey(31604))
                        Add28nTotalSum(dsDirtyUMNS28n.Tables[0], refCls, 930003, GetCacheSum(cacheItem.Value, 30900) +
                            GetCacheSum(cacheItem.Value, 31203) + GetCacheSum(cacheItem.Value, 31303) +
                            GetCacheSum(cacheItem.Value, 31401) + GetCacheSum(cacheItem.Value, 31501) +
                            GetCacheSum(cacheItem.Value, 31604));

                    if (dsDirtyUMNS28n.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
                    {
                        UpdateDataSet(daDirtyUMNS28n, dsDirtyUMNS28n, fctDirtyUMNS28n);
                        ClearDataSet(daDirtyUMNS28n, ref dsDirtyUMNS28n);
                    }
                }
                UpdateDataSet(daDirtyUMNS28n, dsDirtyUMNS28n, fctDirtyUMNS28n);
                ClearDataSet(daDirtyUMNS28n, ref dsDirtyUMNS28n);
            }
            finally
            {
                factCache.Clear();
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                string.Format("Завершение расчета итоговых сумм у {0}.", semantic));
        }

        #endregion расчет итоговых сумм - для 28 н

        #region Соответствие показателей 28н и 65н

        private int Get65nBy28nMark(int mark28n)
        {
            switch (mark28n)
            {
                case 910001:
                    return 30110;
                case 910002:
                    return 30120;
                case 910003:
                    return 30130;
                case 910004:
                    return 30140;
                case 21001:
                case 21101:
                case 21201:
                    return 30210;
                case 21002:
                case 21102:
                case 21202:
                    return 30220;
                case 21003:
                case 21103:
                case 21203:
                    return 30230;
                case 21004:
                case 21104:
                case 21204:
                    return 30240;
                case 21301:
                    return 30310;
                case 21302:
                    return 30340;
                case 930001:
                    return 40110;
                case 930002:
                    return 40150;
                case 930003:
                    return 40140;
                case 30700:
                    return 40200;
                case 31201:
                case 31301:
                    return 40610;
                case 31202:
                case 31302:
                    return 40620;
                case 31203:
                case 31303:
                    return 40640;
                case 31601:
                    return 40710;
                case 31602:
                    return 40720;
                case 31603:
                    return 40730;
                case 31701:
                case 31801:
                case 31901:
                case 32001:
                case 32101:
                    return 40810;
                case 31702:
                case 31802:
                case 31902:
                case 32002:
                case 32102:
                    return 40820;
                case 31703:
                case 31803:
                case 31903:
                case 32003:
                case 32103:
                    return 40830;
                case 30800:
                    return 50500;
                default:
                    return 0;
            }
        }

        private void Fill65nMarks(IFactTable fct)
        {
            string semantic = fct.FullCaption;
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1} and RefDataMarks65n = 0", 
                fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart,
                string.Format("Старт заполнения ссылки на показатели 65н у {0}.", semantic));
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}  and RefDataMarks65n = 0", 
                fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2} and RefDataMarks65n = 0", 
                    firstID, lastID, this.SourceID);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    row["RefDataMarks65n"] = Get65nBy28nMark(Convert.ToInt32(row["RefDataMarks28n"]));
                }
                UpdateDataSet(da, ds, fct);
                ClearDataSet(da, ref ds);
            }
            while (processedRecCount < totalRecs);
            UpdateDataSet(da, ds, fct);
            ClearDataSet(da, ref ds);
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                string.Format("Завершение заполнения ссылки на показатели 65н у {0}.", semantic));
        }

        #endregion Соответствие показателей 28н и 65н

        protected override void ProcessDataSource()
        {
            this.CubeFacts = new IFactTable[] { fctDirtyUMNS28n, fctUMNS28n };
            // расчет итоговых сумм для 28н
            Add28nTotalSums();
            // расщепление
            CheckDisintRulesCache();
            PrepareMessagesDS();
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();
            DisintegrateData(fctDirtyUMNS28n, fctUMNS28n, clsKD, clsOKATO, new string[] { "SUMME" },
                "RefYearDayUNV", "REFKDA28N", "REFOKATOA28N", disintAll);
            // Соответствие показателей 28н и 65н
            Fill65nMarks(fctDirtyUMNS28n);
            Fill65nMarks(fctUMNS28n);
        }

        protected override void UpdateProcessedData()
        {
            UpdateOkatoData();
            UpdateData();
        }

        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
        }

        protected override void AfterProcessDataAction()
        {
            UpdateMessagesDS();
            WriteBadOkatoCodesCacheToBD();
        }

        protected override void DirectProcessData()
        {
            // Заполняем кэши соответствия операционных дней
            FillDisintRulesCache();
            int year = -1;
            int month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
            ProcessDataSourcesTemplate(year, month, "Расщепление сумм по нормативам отчисления доходов");
        }

		#endregion Обработка данных

        #region Сопоставление

        protected override void DirectAssociateData()
        {
            Dictionary<int, string> dataSources = null;

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted)
            {
                dataSources = this.PumpedSources;
            }
            else
            {
                dataSources = GetAllPumpedDataSources();
            }

            if (dataSources.Count == 0)
            {
                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeError, "Нет данных", "Нет данных",
                    "Ошибка при установке иерархии данных: Нет источников, закачанные данные из которых " +
                    "могли бы быть обработаны.", this.PumpID, -1);
                WriteToTrace("ERROR: Нет источников, закачанные данные из которых " +
                    "могли бы быть обработаны.", TraceMessageKind.Error);
            }
            else
            {
                int i = 1;
                // сопоставляем классификаторы, формирующиеся по источникам закачки
                foreach (KeyValuePair<int, string> dataSource in dataSources)
                {
                    SetDataSource(dataSource.Key);
                    DoBridgeCls(dataSource.Key,
                        string.Format("источник {0} из {1}", i, dataSources.Count), this.AssociateClassifiers, false, false);
                    i++;
                }
            }
        }

        #endregion Сопоставление

        #region Проверка данных

        private void CheckClassifier(IClassifier cls, string clsName, string fieldName, int curSourceID, int prevSourceID)
        {
            string queryCurrentSource = string.Format("select t2.{0} from {1} t2 where t2.SourceId = {2}",
                fieldName, cls.FullDBName, curSourceID);
            queryCurrentSource = string.Format("select t1.{2} from {0} t1 where t1.SourceId = {1} AND NOT t1.{2} IN ({3})",
                    cls.FullDBName, prevSourceID, fieldName, queryCurrentSource);
            DataSet ds = (DataSet)this.DB.ExecQuery(queryCurrentSource, QueryResultTypes.DataSet);

            string rowCodes = string.Empty;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowCodes = string.Format("{0}, {1}", rowCodes, row[fieldName].ToString());
            }

            if (rowCodes != string.Empty)
            {
                rowCodes = rowCodes.Substring(1);
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning,
                    string.Format("В классификаторе {0} есть коды, которые отсутствуют в текущем источнике, но были закачаны в предыдущем: {1}", clsName, rowCodes),
                    this.PumpID, curSourceID);
            }
        }

        private void CheckData(int year, int month)
        {
            // вычисление предыдущего месяца
            int prevYear = month == 1 ? year - 1 : year;
            int prevMonth = month == 1 ? 12 : month - 1;
            
            IDataSource curDs = FindDataSource(ParamKindTypes.YearMonth, "ФНС", "0001", string.Empty, year, month, string.Empty, 0, string.Empty);
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeStart,
                string.Format("Старт проверки по данным источника {0}-{1} (SourceID = {2}).",
                    year, month.ToString().PadLeft(2, '0'), curDs.ID), this.PumpID, curDs.ID);
            if (curDs == null)
            {
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning,
                    "Данные за указанный месяц отсутствуют", this.PumpID, curDs.ID);
                return;
            }

            IDataSource prevDs = FindDataSource(ParamKindTypes.YearMonth, "ФНС", "0001", string.Empty, prevYear, prevMonth, string.Empty, 0, string.Empty);
            if (prevDs == null)
            {
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning,
                    "Данные за предыдущий месяц отсутствуют", this.PumpID, curDs.ID);
                return;
            }

            CheckClassifier(clsKD, "КД.28н", "CodeStr", curDs.ID, prevDs.ID);
            CheckClassifier(clsOKATO, "ОКАТО.28н", "Code", curDs.ID, prevDs.ID);
            CheckClassifier(clsOKVED, "ОКВЭД.28н", "CodeStr", curDs.ID, prevDs.ID);
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeSuccefullFinished,
                string.Format("Проверка по данным источника {0}-{1} (SourceID = {2}) успешно завершена.",
                    year, month.ToString().PadLeft(2, '0'), curDs.ID), this.PumpID, curDs.ID);
        }

        protected override void DirectCheckData()
        {
            int year = -1;
            int month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                GetDisintParams(ref year, ref month, ref disintAll);
            }
            else
            {
                year = this.DataSource.Year;
                month = this.DataSource.Month;
            }
            CheckData(year, month);
        }

        #endregion Проверка данных

    }
}