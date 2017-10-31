{
  Константы общие для обеих надстроек планирования
}

unit uGlobalPlaningConst;
interface

type
  {отношение версии листа к версии плагина}
  TVersionRelation = (svAncient, svModern, svFuture);

const
  FSD = '  '#10;//Filter String Delimiter

  fmMajorVersion = '2';
  fmMinorVersion = '3';
  fmRelease = '9';
  IsTestVersion = false;

  // пароль для защиты книги
  fmPassword = 'krista.fm.planing';
  { режимы отладки версии}
  dcvTest = 'Test';
  dcvRelease = 'Release';

  fmNoRefreshDate = 'неизвестно';

  //имена свойств листа или документа
  pspSheetType = 'PlanningSheetType';
  pspTaskName = 'PlanningSheetTaskName';
  pspTaskId = 'PlanningSheetTaskId';
  pspDocumentName = 'PlanningSheetDocumentName';
  pspDocumentId = 'PlanningSheetDocumentId';
  pspOwner = 'PlanningSheetOwner';
  pspCreationDate = 'PlanningSheetCreationDate';
  pspDocPath = 'DocPath';

  {Новые имена свойств документа}

  {Статические - остаются в листе по окончании работы}
  fmDocumentName = 'fm.DocumentName';
  fmDocumentId = 'fm.DocumentId';
  fmTaskName = 'fm.TaskName';
  fmTaskId = 'fm.TaskId';
  fmOwner = 'fm.Owner';
  fmDocPath = 'fm.DocPath';
  fmConnectionStr = 'fm.ConnectionStr';
  fmAlterConnection = 'fm.AlterConnection';
  fmSchemeName = 'fm.SchemeName';
  fmDocType = 'fm.DocType';
  fmResultSuccess = 'fm.Result.Success';
  fmResultMessage = 'fm.Result.Message';

  {Контекстные - удаляются при закрытии}
  fmtcSilentMode = 'fm.tc.SilentMode';
  fmtcContextType = 'fm.tc.ContextType';
  fmtcData = 'fm.tc.Data';
  fmtcAuthType = 'fm.tc.AuthType';
  fmtcLogin = 'fm.tc.Login';
  fmtcPwdHash = 'fm.tc.PwdHash';
  fmtcIsTaskConnect ='fm.tc.IsTaskConnect';
  fmtcAction = 'fm.tc.Action';
  fmtcMutexName = 'fm.tc.Action.MutexName';
  fmtcLoadingFromTask = 'fm.tc.LoadingFromTask';


  {enum DocumentActionType}
   datNone = 0;
   datRefresh = 1;
   datWriteBack = 2;
   datRewrite = 4;
   datProcessCube = 8;
   datRefreshAfter = 16;
   datRewriteAndProcess = datRewrite + datProcessCube;
   datRefreshWritebackRefresh = datRefresh + datWriteBack + datRefreshAfter;

  { имена и значения CustomProperties
  cpnDocumentType = 'dtype';
  cpnDocumentVersion = 'dversion';
  cpvDocumentTypeConstruct = 'construct';
  cpvDocumentTypeProtected = 'protected';}

  { константы для именования атрибутов узла }
  attrName = 'name';
  attrChecked = 'checked';
  attrForbidCheck = 'forbidcheck';
  attrInfluence = 'influence';
  attrUniqueName = 'unique_name';
  attrMask = 'mask';
  attrLocalId = 'local_id';
  attrPKID = 'pk_id';

  //константы для сборки XPath
  xpRoot = 'metadata';
  xpTotals = xpRoot + '/totals';
  xpTotal = 'total';
  xpRows = xpRoot + '/rows';
  xpRow = 'row';
  xpColumns = xpRoot + '/columns';
  xpColumn = 'column';
  xpFilters = xpRoot + '/filters';
  xpFilter = 'filter';
  xpScope = 'scope';
  xpInnerData = xpRoot + '/innerdata';
  xpNames = xpRoot + '/names';
  xpBreaks = xpRoot + '/breaks';
  xpBreak = 'break';
  xpSingleCells = xpRoot + '/singlecells';
  xpConsts = xpRoot + '/consts';
  xpParams = xpRoot + '/params';

  mnIsRowLeaf = 'IsRowLeaf';
  mnIsColumnLeaf ='IsColumnLeaf';

  //константы для именования атрибутов XML
  attrID = 'id';
  attrMP = 'member_props';
  attrAllMP = 'all_member_props';
  attrIgnoreHierarchy = 'ignorehierarchy';
  attrReverseOrder = 'reverseorder';
  attrHideDataMembers = 'HideDataMembers';
  attrSummariesAtBeginning = 'summaries_at_beginning';
  attrPlaceRowMPBefore = 'rmp_before';
  attrPlaceColumnMPBefore = 'cmp_before';
  attrTotalType = 'totaltype';
  attrCaption = 'caption';
  attrCube = 'cubename';
  attrMeasure = 'measurename';
  attrIgnoreColumns = 'ignorecolumns';
  attrIgnoreRows = 'ignorerows';
  attrGrandTotalOnly = 'grandtotalonly';
  attrColumnWidth = 'columnwidth';
  attrDimension = 'dimension';
  attrHierarchy = 'hierarchy';
  attrPartial = 'ispartial';
  attrCounter = 'counter';
  attrPidCounter = 'pidcounter';
  attrRowSummaryEnable = 'rowsummaryenable';
  attrPrintableStyle = 'printablestyle';
  attrRowGrandSummaryEnable = 'rowgrandsummaryenable';
  attrColumnSummaryEnable = 'columnsummaryenable';
  attrColumnGrandSummaryEnable = 'columngrandsummaryenable';
  attrTotalsInThousand = 'totalsinthousand';
  attrTotalMultiplier = 'TotalMultiplier';
  attrIsMarkerOnTheRight = 'ismarkerontheright';
  attrMarkerPosition = 'markerposition';
  attrFilterCellsLength = 'filtercellslength';
  attrIsMergeFilterCellsByTable = 'ismergefiltercellsbytable';
  attrMultiplicationFlag = 'multiplicationflag';
  attrHideEmptyRows = 'hideemptyrows';
  attrHideEmptyColumns = 'hideemptycolumns';
  attrFormat = 'format';
  attrDigits = 'digits';
  attrAllMember = 'all_member';
  attrSheetVersion = 'sheet_version';
  attrDisplaySheetInfo = 'displaysheetinfo';
  attrDisplayCommentStructuralCell = 'displaycommentstructuralcell';
  attrDisplayCommentDataCell = 'displaycommentdatacell';
  attrSummariesByVisible = 'SummariesByVisible';
  attrDisplayColumnsTitles = 'displaycolumnstitles';
  attrDisplayFilters = 'displayfilters';
  attrDisplayRowsTitles = 'displayrowstitles';
  attrDisplayTotalsTitles = 'displaytotalstitles';
  attrDisplayColumns = 'displaycolumns';
  attrDisplayFullFilterText = 'displayfullfiltertext';
  attrURL = 'URL';
  attrSchemeName = 'schemeName';
  attrLastRefreshDate = 'lastrefreshdate';
  attrNeedRound = 'needround';
  attrCountMode = 'countmode';
  attrAllChildrenChecked = 'allchildrenchecked';
  attrAllDescendantsChecked = 'alldescendantschecked';
  attrLevelState = 'levelstate';
  attrEmptyValueSymbol = 'emptyvaluesymbol';
  attrSummaryOptimization = 'summaryoptimization';
  attrRowsBroken = 'rowsbroken';
  attrColumnsBroken = 'columnsbroken';
  attrValueStyle = 'valuestyle';
  attrValueStylePrint = 'valuestyleprint';
  attrTitleStyle = 'titlestyle';
  attrTitleStylePrint = 'titlestyleprint';
  attrTableProcessingMode = 'tableprocessingmode';
  attrSparseMatrixMode = 'sparsematrixmode';
  attrSummaryOptions = 'summaryoptions';
  attrGrandSummaryOptions = 'grandsummaryoptions';
  attrMPBefore = 'mpbefore';
  attrBroken = 'broken';
  attrHideEmpty = 'hideempty';
  attrUseSummariesForElements = 'usesummariesforelements';
  attrUseSummariesForLevels = 'usesummariesforlevels';
  attrStyleByLevels = 'stylebylevels';
  attrAllCapitals = 'allcapitals';
  attrFormatTotalsArea = 'formattotalsarea';
  attrUseIndents = 'useindents';
  attrAllowNECJ = 'allownonemptycrossjoin';
  attrHeadingType = 'headingtype';
  attrHeadingAddress = 'headingaddress';
  attrHeadingEnd = 'headingend';
  attrRangeName = 'rangename';
  attrType = 'type';
  attrIsOtherSheet = 'isothersheet';
  attrTotalID = 'totalid';
  attrDefaultValue = 'defaultvalue';
  attrUserExcelName = 'userexcelname';
  attrDeployment = 'deployment';
  attrIsMarkupNew = 'ismarkupnew';
  attrHidden = 'hidden';

  //это - сквозной порядковый номер уровня в оси
  attrLevelNumber = 'levelnumber';
  attrLevelsFormatting = 'levelsformatting';
  attrFormatByRows = 'formatbyrows';
  attrUseFormat = 'useformat';
  attrParentUN = 'parentun';
  attrHiddenTuple = 'hidden';
  attrInitialIndex = 'initialindex';
  attrUseCustomDMTitle = 'usecustomdmtitle';
  attrCustomDMTitle = 'customdmtitle';
  attrDataMembersMode = 'datamembersmode';

  {атрибуты шрифта}
  attrSubscript = 'subscript';
  attrItalic = 'italic';
  attrBold = 'bold';
  attrUnderline = 'underline';
  attrStrikethrough = 'strikethrough';
  attrSuperscript = 'superscript';
  attrColorIndex = 'colorindex';
  attrSize = 'size';

  {имена узлов пользовательских событий}
  xpBeforeRefresh = 'beforerefresh';
  xpAfterRefresh = 'afterrefresh';
  xpAfterWriteBack = 'afterwriteback';
  xpBeforeWriteBack = 'beforewriteback';
  {атрибуты пользовательского события}
  attrEnabled = 'enabled';
  attrMacrosName = 'macrosname';
  {атрибуты типовой формулы}
  attrTypeFormula = 'typeformula';
  attrTemplate = 'template';
  attrTotalAlias = 'total';
  attrOffset = 'offset';
  attrIsEnabled = 'isenabled';
  attrParamType = 'paramtype';
  attrParamValue = 'paramvalue';
  attrSection = 'section';

  { Константы для разъименовки объектов базы в имя диапазона (Worksheet.Names) }
  { Имена зарезервированных диапазонов листа планирования строятся следующим образом }
  { <общий префикс><разделитель><тип объекта>[<разделитель><параметр1>][<разделитель><параметр1>]...}
  { В роли параметров выступают имена объектов базы: измерения, уровни, меры и пр... }
  { Нужно помнить, что имя диапазона это неболее 255 символов только цифр и букв (русские можно) }
  snNamePrefix = 'krista'; // общий префикс для всех имен
  snUserNamePrefix = 'Криста';
  // !!!разделитель (по сечашней технологии перекодировки это значение лучше не менять!!!)
  //  и еще это должен быть обязательно один символ (временный вариант)
  snSeparator = '_';
  // разделитель семантической части в имени измерения
  snSemanticsSeparator = '__';
  // разделитель элементов фильтра
  snBucks = '$$$';
  snSingleCell = 'sc';


  { сообщения об ошибках и предупреждения }
  ermUnknown = 'Неизвестная ошибка';
  ermMetadataCacheSaveFault = 'При сохранении метаданных в клиентский кэш произошла ошибка.';
  ermMetaDataCacheLoadFault = 'При получении метаданных из клиентского кэша произошла ошибка.';
  ermDirectoryNotExists ='Указанный каталог не существует.';
  ermDocumentLogSaveFault ='Не удалось сохранить документ в журнал.';
  ermCacheClearFault = 'При очищении кэша произошла ошибка.';
  ermMDCacheClearFault = 'Не удается очистить метаданные.';
  ermMemberListCachClearFault = 'При удалении устаревшего измерения из клиентского кэша произошла ошибка.';
  ermMemberListCacheSaveFault = 'При сохранении измерения "%s" в клиентский кэш произошла ошибка.';
  ermMemberListCacheLoadFault = 'При получении измерения "%s" из клиентского кэша произошла ошибка.';
  ermMemberListCacheClearAllFault = 'Не удалось удалить все измерения.';
  ermMemberListCacheGetLoaded = 'Не удалось получить список загруженных измерений.';
  ermRegistryFault = 'Ошибка при работе с реестром';
  ermMDCacheWriteFault = 'Не удается обновить кэш метаданных.';
  ermConnParamFault = 'Некорректные параметры подключения';
  ermMetaDataLoadFault = 'При получении метаданных возникла ошибка.';
  ermMembersLoadFault = 'При получении элементов измерения возникла ошибка.';
  ermMeasuresLoadFault = 'Не удалось получить список мер.';
  ermNoWriteBackCubes = ermMeasuresLoadFault + ' Нет кубов, предназначенных для обратной записи.';
  ermDimensionsLoadFault = 'Не удалось получить список гиперкубов.';
  ermLevelsLoadFault = 'Не удалось получить список уровней измерения.';
  ermDataProviderUnknown = 'Не определен поставщик данных.';
  ermConnectionFault = 'Не удалось подключиться к серверу.';
  ermNoneConnection = 'Не установлено соединение с сервером.';
  ermSheetOverSize = 'Невозможно разместить данные на листе MS Excel. ' +
    'Лист не может содержать больше %d столбцов или больше %d строк.';
  ermWorkbookProtectionFault = 'Невозможно снять защиту с книги, вероятно один из листов'
      + ' защищен паролем пользователя.';
  ermWorksheetProtectionFault = 'Не удается снять защиту с листа, возможно он'
    + ' защищен паролем пользователя.';
  ermValidationFault = 'Некорректное состояние метаданных';
  ermDuplicateFilterForTotal = 'Фильтр дублируется для показателя "%s".' + #13 +
    'На этот показатель уже наложен общий или частный фильтр, построенный по этому же измерению.';
  ermUpdateSheetVersionFail = 'Не удалось обновить лист до текущей версии надстройки.';
  ermUpdateDocumentVersionFail = 'Не удалось обновить документ до текущей версии надстройки.';
  ermIOperationNotFound = 'Интерфейс IOperation не загеристрирован.';

  // сообщения при обратной записи
  ermWritebackError = 'Во время обратной записи данных произошла ошибка.';
  ermWritebackNoResultTotals = 'На листе отсутствуют показатели типа "результат".';
  ermWritebackNoTaskId = 'Отсутствует идентификатор задачи, обратную запись произвести невозможно.';
  ermWritebackNoData = 'Не удалось осуществить обратную запись. Данные отсутствуют или относятся к нелистовым элементам.';
  ermWritebackMultipleFilter = 'В фильтре %s выбрано несколько элементов. Обратная запись невозможна.';
  ermWritebackNoDataMember = 'В фильтре %s выбран нелистовой элемент, у которого отсутствует элемент Data Member. Обратная запись невозможна.';
  ermWritebackSuccess = 'Данные были успешно записаны.';
  ermTotalsMarkupDamaged = 'Повреждена разметка области показателей, обратная запись невозможна. Попробуйте обновить лист.';
  ermWritebackBadDimensions = 'Обратная запись невозможна, т.к на лист вынесены измерения из неосновной базы: %s.';

  {ошибки типовых формул}
  ermTypeFormulaFault = 'Формула из ячейки не может быть типовой, так как ';

  wrmDimInUseAlrady = 'Выбранное измерение уже входит в состав %s таблицы.' + #13 +
    'Использование одного и того же измерения в разных компонентах таблицы недопустимо.';
  wrmImpossibleMultiplySelection = 'Для параметра "%s" запрещен множественный выбор элементов.' + #10#13 +
                                   'Выберите один элемент или измените свойства параметра.';
  wrmMeasureInUseAlrady = 'Выбранная мера уже использована в структуре таблицы.';
  wrmFreeTotalInUseAlrady = 'Свободный показатель с таким именем уже существует.';
  wrmSelectExistsElement = 'Выберите из списка объект, который необходимо изменить.';
  qumDelElementColumns = 'Удалить элемент столбцов ';
  qumDelElementRows = 'Удалить элемент строк ';
  qumDelTotal = 'Удалить показатель ';
  qumDelFilter = 'Удалить фильтр ';
  qumDelSingleCell = ' Удалить отдельный показатель ';
  qumDelConstCell = ' Удалить константу ';
  qumDontComplite = 'Текущая операция редактирования компонента таблицы не завершена.' + #13 +
    'Закончить работу мастера с потерей незавершенной операции?';
  qumDelSheetHistory = 'Удалить историю выбранного листа?';
  ermEditNotPossible = 'Редактирование невозможно.'#13#10 +
          'Элемент отсутствует в базе. '#13#10 +
          'Возможно, лист устарел.';
  ermIncorrectCellValue = 'Значение ячейки является некорректным. ';
  ermNoDimensionSelected = 'Не выбрано измерение';

  ermMapRowsMPFailed = 'Ошибка при размещении свойств оси строк';
  ermMapColumnsMPFailed = 'Ошибка при размещении свойств оси столбцов';
  // !!!
  ermBreakInsertFault = 'Не удалось вставить разрыв.' + #13 + 'Возможно включён режим копирования.';
  wrmDropInfluence = 'Снятие выделения с данного элемента приведет к сбросу ' +
    'атрибута влияния у его предков. Вы уверены, что хотите продолжить?';

  wrnNoTaskConnection = 'Лист принадлежит задаче, но контекст задач отсутствует.';
  wrnClearTaskPropertiesSave = 'Документ прикреплен к задаче, но был сохранен по адресу, отличному от оригинального. Документ будет откреплен от задачи.';
  wrnClearTaskPropertiesLoad = 'Документ прикреплен к задаче, но его адрес отличается от оригинального. Документ будет откреплен от задачи.';
  wrnTaskSave = 'Сохранение документа под другим именем или по другому адресу приведет к тому, что изменения в нем не будут сохраняться в базу данных.';

  { заголовки для индикатора процесса }
  pcapConnect = 'Подключение к серверу...';
  pcapMaping =  'Запрос и размещение данных...';
  pcapRefreshMembers = 'Обновление измерений...';
  pcapLoadMembers = 'Загрузка дерева измерения...';
  pcapLoadMetadata = 'Загрузка метаданных...';
  pcapCollectMetadata = 'Сбор метаданных таблицы';
  pcapCollectWritebackData = 'Сбор данных для обратной записи';
  pcapWriteback = 'Обратная запись';


  {заголовки формы индикации, сообщения об (не)успехе}
  ftRefresh = 'Обновление листа';
  mSuccessRefresh = 'Обновление прошло успешно';
  mErrorRefresh = 'Ошибка при обновлении';
  mSheetVersionUpdate = 'Обновление версии листа';
  mDocumentVersionUpdate = 'Обновление версии документа';

  ftWriteback = 'Обратная запись';
  mWritebackSuccess = 'Обратная запись прошла успешно';
  mWritebackError = 'При обратной записи произошла ошибка';

  {описание операций для формы индикации процесса}
  pfoOperationFailed = 'В процессе операции произошла ошибка';

  pfoWriteback = 'Обратная запись';
  pfoCollectWritebackData = 'Сбор данных для обратной записи';
  pfoWritebackComplete = 'Завершение операции';

  pfoRebuildSheet = 'Обновление листа';

  pfoSMDLoad = 'Загрузка метаданных';
  pfoSMDRefresh = 'Обновление метаданных';
  pfoRowsRefresh = 'Обновление строк';
  pfoColumnsRefresh = 'Обновление столбцов';
  pfoFiltersRefresh = 'Обновление фильтров';
  pfoDimensionRefresh = 'Обновление измерения ';
  pfoSingleCellsRefresh = 'Обновление отдельных показателей';
  pfoTypeFormulaRefresh = 'Обновление типовой формулы';

  pfoSingleResultDeletion = 'Удаление отдельного показателя';

  pfoSingleResultMove = 'Перемещение показателя';

  pfoCollectFreeTotalsData = 'Сбор данных для размещения';
  pfoCollectSingleCellsData = 'Сбор данных отдельных показателей для размещения';

  pfoSMDValidate = 'Проверка корректности таблицы';
  pfoRowsValidate = 'Проверка корректности строк';
  pfoColumnsValidate = 'Проверка корректности столбцов';
  pfoFiltersValidate = 'Проверка корректности фильтров';
  pfoTotalsValidate = 'Проверка корректности показателей';
  pfoSingleCellsValidate = 'Проверка корректности отдельных показателей';

  pfoMapTable = 'Размещение таблицы';
  pfoGetFullRowAxis = 'Сборка оси строк';
  pfoGetFullColumnAxis = 'Сборка оси столбцов';

  pfoLogSave = 'Сохранение результата в лог-файл ';
  pfoLogSaveFailed = 'Не удалось сохранить лог-файл ';

  pfoCalcSheetSize = 'Проверка размеров таблицы';
  pfoSheetOversize = 'Невозможно разместить данные на листе MS Excel. ' +
    'Лист не может содержать больше 256 столбцов или больше 65536 строк.';
  pfoQueryMdx = 'Выполнение MDX-запроса для показателей ';
  pfoQuerySingleCellMdx = 'Выполнение MDX-запроса для отдельного показателя';
  pfoQuerySingleCellMdxShort = 'Выполнение MDX-запроса';
  pfoQueryFailed=  'Ошибка получения данных. ';
  pfoMapFilters = 'Размещение фильтров';
  pfoMapRows = 'Размещение строк';
  pfoMapColumns = 'Размещение столбцов';
  pfoMapTotals = 'Размещение показателей';
  pfoProcessTotalData = 'Размещение данных для показателей ';
  pfoLargeMDXQuery = 'Текст точного запроса превышает предельные 64Кб. ' +
      'Исполняется неоптимизированный запрос.';
  pfoMapRowsMP = 'Размещение свойств оси строк';
  pfoMapColumnsMP = 'Размещение свойств оси столбцов';
  pfoCalculateSheet = 'Вычисление формул в листе';
  pfoSwitchSheetOffline = 'Перевод в автономный режим';

  {имена xml в CustomProperty}
  cpSheetHistory = snNamePrefix + snSeparator + 'SheetHistory';
  cpMDName = snNamePrefix + snSeparator + 'fm_metadataXML';
  cpConstsName = snNamePrefix + snSeparator + 'fm_consts';
  cpUserEvents = snNamePrefix + snSeparator + 'fm_Events';
  cpSequelName = snNamePrefix + snSeparator + 'fm_sequel';
  cpRowsMarkup = snNamePrefix + snSeparator + 'fm_rowsmarkup';
  cpColumnsMarkup = snNamePrefix + snSeparator + 'fm_columnsmarkup';
  cpCellsMarkup = snNamePrefix + snSeparator + 'fm_cellsmarkup';
  cpRowsAxis = snNamePrefix + snSeparator + 'fm_rowsaxis';
  cpColumnsAxis = snNamePrefix + snSeparator + 'fm_columnaxis';

  {имена стилей на листе}
  snFieldTitle = 'Заголовки полей';
  snFieldTitlePrint = 'Заголовки полей [печать]';

  snFieldPosition = 'Элементы осей';
  snFieldPositionPrint = 'Элементы осей [печать]';

  snTotalMeasureTitle = 'Заголовок меры';
  snTotalConstTitle = 'Заголовок показателя константы';
  snTotalFreeTitle = 'Заголовок свободного показателя';
  snTotalResultTitle = 'Заголовок результата расчета';
  snTotalTitlePrint = 'Заголовок показателя [печать]';

  snData = 'Данные (только для чтения)';
  snDataFree = 'Данные (редактируемые)';
  snDataFreeErased = 'Данные для удаления';

  snFilterValue = 'Значение фильтра';
  snFilterValuePrint = 'Значение фильтра [печать]';

  snSheetId = 'Информация о задаче';

  snMemberProperties = 'Свойства элементов измерения';
  snMemberPropertiesPrint = 'Свойства элементов измерения [печать]';

  snSingleCells = 'Отдельная ячейка';
  snSingleCellsPrint = 'Отдельная ячейка [печать]';
  snResultSingleCells = 'Отдельная ячейка-результат';
  snResultSingleCellsPrint = 'Отдельная ячейка-результат [печать]';
  snSingleCellConst = 'Отдельная ячейка - константа';
  snSingleCellConstPrint = 'Отдельная ячейка - константа [печать]';

  FunnyStr = '/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*';

  DefaultHierarchyName = 'Иерархия по умолчанию';

  CriticalNode = true;
  NoteTime = true;

  { типы объектов }
  sntTable = 'table'; // вся таблица
  sntTableWitoutID = 'tablewitoutid';
  sntUserTable = 'Таблица'; // имя таблицы видимое пользователю
  sntImpotrArea = 'ОбластьИмпорта';
  sntTableHeading = 'Заголовок_Таблицы';
  sntColumns = 'c'; // вся ось столбцов
  sntColumnTitles = 'ct'; //все заголовки оси столбцов
  sntColumnDimension = 'cd'; // одно измерение по столбцам, включает несколько уровней. 2 параметра - измерение и иерархия.
  sntColumnLevel = 'cl'; // уровень измерения по столбцам. 2 параметра - порядковый номер и уровень.
  sntColumnLevelTitle = 'clt'; // заголовок уровня измерения по столбцам. 2 параметра - порядковый номер и уровень.
  sntRows = 'r'; // вся ось строк
  sntRowTitles = 'rt'; //все заголовки оси строк
  sntRowDimension = 'rd'; // измерение по строкам.
  sntRowLevel = 'rl'; // уровень по строкам
  sntRowLevelTitle = 'rlt'; // заголовок уровня по строкам
  sntTotals = 't'; // все колонки показателей.
  sntTotalTitles = 'tt'; //все заголовки показателей
  sntTotalMeasure = 'tm'; // показатель из базы. 3 параметра - порядковый номер столбца, куб и мера в нем
  sntTotalMeasureTitle = 'tmt'; // параметры такие же как и у основной колонки
  sntTotalFree = 'tf'; // свободный показатель. 1 параметр - порядковый номер столбца, имя
  sntTotalFreeTitle = 'tft'; // параметры такие же как и у основной колонки
  sntTotalResult = 'tr'; // показатель-результат. 1 параметр - порядковый номер столбца, имя.
  sntTotalResultTitle = 'trt'; // параметры такие же как и у основной колонки
  sntTotalConst = 'tc'; // показатель-константа. 1 параметр - порядковый номер столбца, имя.
  sntTotalConstTitle = 'tct'; // параметры такие же как и у основной колонки
  sntFilter = 'f'; // ?? фильтр. 2 параметра - измерение и иерархия
  sntFilterArea = 'fa'; // ?? вся область фильтров. Без параметров.
  sntMemberOld = 'member';
  sntMember = 'amember';
  sntSheetId = 'sid';
  sntTestMark = 'test_mark';
  sntUnitMarker = 'um';
  sntSingleCellOld = 'sc';
  sntSingleCellMeasure = 'scm';
  sntSingleCellResult = 'scr';
  sntSingleCellConst = 'scc';


  { пункты контекстного меню}
  pmnEditStyle = 'Изменить стиль...';
  pmnRowEdit = 'Редактировать элемент строк...';
  pmnRowDel = 'Удалить элемент строк';
  pmnInsertNewLine = 'Добавить строки...';
  pmnColumnEdit = 'Редактировать элемент столбцов...';
  pmnColumnDel = 'Удалить элемент столбцов';
  pmnTotalMeasureEdit = 'Редактировать показатель...';
  pmnTotalMeasureRefresh = 'Обновить показатель';
  pmnTotalMeasureDel = 'Удалить показатель';
  pmnTotalFreeEdit = 'Редактировать свободный показатель...';
  pmnTotalFreeDel = 'Удалить свободный показатель';
  pmnTotalResultEdit = 'Редактировать результат...';
  pmnTotalResultDel = 'Удалить результат';
  pmnTotalResultMarkEmpty = 'Очистить ячейку';
  pmnTotalConstDel = 'Удалить константу';
  pmnTotalConstEdit = 'Редактировать константу...';
  pmnTotalRefresh = 'Обновить показатель';
  pmnFilterEdit = 'Редактировать фильтр...';
  pmnFilterDel = 'Удалить фильтр';
  pmnMoveToColumns = 'Переместить в область столбцов';
  pmnMoveToRows = 'Переместить в область строк';
  pmnMoveToFilters = 'Переместить в область фильтров';
  pmnSingleCellNew = 'Добавить отдельный показатель...';
  pmnSingleCellEdit = 'Редактировать отдельный показатель...';
  pmnSingleCellRefresh = 'Обновить отдельный показатель';
  pmnSingleCellDel = 'Удалить отдельный показатель';
  pmnCellConstNew = 'Добавить константу...';
  pmnCellConstEdit = 'Редактировать константу...';
  pmnCellConstRefresh = 'Обновить константу';
  pmnCellConstDel = 'Удалить константу';
  pmnTableRefresh = 'Обновить таблицу';
  pmpSetTypeFormula = 'Установить как типовую формулу';
  pmnElementProperties = 'Свойства элемента...';
  pmnSwitchToMeasure = 'Преобразовать в меру...';
  pmnSwitchToResult = 'Преобразовать в результат...';
  pmnHideTotalColumns = 'Скрыть столбцы показателя';

  { ключи реестра }
  regBasePath = '\SOFTWARE\Krista\FM\ExcelAddIn';
  regConnSection = '\Connection';
  regConnDirectSection = '\Direct';
  regConnWebServiceSection = '\WebService';
  regToolBarSettingsSection = '\ToolBarSettings';
  regDirectServerKey = 'Server';
  regDirectCatalogKey = 'Catalog';
  regURLKey = 'URL';
  regServiceListKey = 'ServiceList';
  regWebServiceSchemeKey = 'Scheme';
  regCachePathKey = 'CachePath';
  regLogEnableKey = 'LogEnable';
  regLogPathKey = 'LogPath';
  regCloseFormProcess = 'AutoCloseProcessForm';
  regWindowsAuthentication = 'WindowsAuthentication';
  regLogin = 'Login';
  regInitialDelay = 'InitialDelay';

  { заголовки интерфейса плагина }
  kriAdd ='Добавить';

  {пункты статусбара}
  kriRefreshDate = 'Последнее обновление';
  kriTaskID = 'Задача';
  kriOnlineMode = 'Режим';
  kriStatusBarContext = 'Контекст';

implementation

end.


