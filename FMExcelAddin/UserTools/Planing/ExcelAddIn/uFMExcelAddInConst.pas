{
  Константы листа планирования
}

unit uFMExcelAddInConst;
interface
uses
  uGlobalPlaningConst;


const
  // что ставить в ячейки показателя, если нет данных по этой позиции
  fmEmptyCell = '';
  //недопустимые значения ячеек
  fmCellWrongValue = '#ЗНАЧ!';
  fmCellZeroDivision = '#ДЕЛ/0!';
  fmIncorrectRef = '#REF!';
  fmIncorrectRefRus = '#ССЫЛКА!';
  //Индикатор единицы измерения - "тысячи рублей"
  fmThousandRubles = '(тыс. руб.)';
  fmMlnRubles = '(млн. руб.)';
  fmMlrdRubles = '(млрд. руб.)';
  // имя файла с метаданными в локальном кэше
  fnMetaDataFileName ='METADATA.xml';


  { заголовки интерфейса плагина }
  kriStatusbarCaption = 'Информация о листе (Криста)';
  kriToolbarCaption = 'Данные (Криста)';
  kriMenuCaption = 'Данные (&Криста)';
  kriConnectionCaption = 'Соединение...';
  kriSheetPropertiesCaption = 'О программе...';
  kriCommonOptionsCaption = 'Настройки...';
  kriShowParamsCaption = '&Параметры...';
  kriShowConstsCaption = '&Константы...';
  kriSheetHistory ='&История листа...';
  kriSheetInfo = 'Сводная информация...'; 
  kriConstructorWizardCaption = 'Структура таблицы...';
  kriRefreshCaption = '&Обновить';
  kriToolbarSendData = 'Записать данные';
  kriMenuSendData = 'Записать данные...';
  kriMarkEmpty = 'Очистить ячейку';
  kriCopyForm = 'Скопировать лист...';
  kriSingleCellsManager = 'Отдельные показатели...';
  kriStyleManager = 'Стили...';
  kriComponentEditor = 'Свойства элементов...';
  kriInConstructionMode = 'Режим конструирования';
  kriInDataMode = 'Выйти из режима конструирования';
  kriDataCollectionForm = 'Форма сбора данных...';
  kriMenuReplication = 'Тиражирование...';

  { пункты меню "Добавить"}
  kriTotals = 'Показатель';
  kriRows = 'Элемент строк...';
  kriColumns = 'Элемент столбцов...';
  kriFilters = 'Фильтр...';
  kriSingleCell = 'Отдельный показатель...';
  kriConst = 'Отдельную константу...';

  { пункты меню "Показатели"}
  kriMeasure = 'Мера из куба...';
  kriFree = 'Свободный...';
  kriResult = 'Результат расчёта...';
  kriTotalConst = 'Константа...';

  kriBreak = 'Вставка разрыва...';

  {пункты меню "Разрыв"}
  kriAfterFilters = 'После фильтров';
  kriAfterColumns = 'После столбцов';
  kriAfterRowsOrTotals = 'После строк или показателей';

  { заголовки и пояснения страниц для мастера структуры }
  capElementType = 'Какой компонент таблицы необходимо изменить?';
  dscElementType = 'Выберите одну из четырех составляющих конструктора таблицы,'
    + #13 + 'которую в данный момент требуется изменить.';

  capNewOrEdit = 'Изменение %s';
  dscNewOrEdit = 'Вы можете добавить %s, удалить или изменить свойства уже существующего, а также поменять порядок следования %s.';

  capTotalType = 'Задайте тип итогового поля';
  dscTotalType = 'Каждый показатель принадлежит к одному из трех возможных типов.'
    + #13 + 'Укажите один из них для показателя, который следует добавить.';

  capTotalProperties = 'Свойства показателя';
  dscTotalProperties = 'Определите свойства, влияющие на внешний вид показателя,'
    + #13 + 'а также на его положение относительно осей (разрезность).';

  capConstsChoise = 'Константа';
  dscConstsChoise = 'Выберите константу для последующего ее размещения на листе.';

  capMemberTotal = 'Мера из гиперкуба';
  dscMemberTotal = 'На сервере представлен набор гиперкубов, каждый из которых содержит несколько мер. Выберите одну из них в качестве показателя таблицы.';

  capMemberResult = 'Результат расчета';
  dscMemberResult = 'На сервере представлен набор гиперкубов, каждый из которых содержит несколько мер (отображаются предназначенные для обратной записи кубы' + #10#13 +
                    'и меры). Выберите одну из них в качестве показателя-результата.';

  capDimChoise = 'Выбор измерения';
  dscDimChoise = 'Выберите одно из измерений, имеющихся в многомерной базе на сервере.'
    + #13 + 'Измерение может содержать несколько альтернативных иерархий.'
    + #13 + 'В этом случае укажите одну из них.';
  capMemberPropertiesChoise = 'Выбор свойств элементов';
  dscMemberPropertiesChoise = 'Для указанного ранее измерения выберите одно или несколько свойств.';
  capMemberChoise = 'Выбор элементов измерения';
  dscMemberChoise = 'Выберите, какие из элементов измерения должны участвовать'
    + #13 +  'в построении таблицы.';
  capTotalPlacement = 'Размещение показателя';
  dscTotalPlacement = 'Возможны несколько вариантов размещения показателя на листе.'
    + #13 + 'Выберите один из вариантов, основываясь на структуре осей.';
  capViewAll = 'Операция завершена';
  dscViewAll = 'Вы можете продолжить построение других элементов таблицы, нажав кнопку "Далее" или закончить, нажав "Готово"';
  capFilterScope = 'Укажите область действия фильтра';
  dscFilterScope = 'Общий фильтр действует на все показатели на листе,'#10' частный - только на отмеченные.';
  //режим подсчета итога
  dmTypeFormula = 'Типовая формула';
//  capFilterScope = 'Область действия фильтра';

  {таги для кнопок и пунктов меню }
  tagMenu = '{D1724D12-8E7A-4E1D-9779-5E55C407DB47}';
  tagEmptyMenu = '{B558E17C-5034-4522-B7BE-BD3D732FC742}';
  tagToolButtonConstructorWizard = '{EC6F1194-3C21-41A1-970A-53D5063EFE42}';

  tagToolButtonAdd = '{7EB58042-3711-40D0-8A2A-7FC476FDC8AB}';
  tagToolButtonTotals = '{79B72340-A699-4684-BE6B-0A0421C83D9C}';
  tagToolButtonRows = '{5DEC4E8D-EF5C-47FF-B7B1-F87BE63E8D08}';
  tagToolButtonColumns = '{3377B0CA-C49C-4167-8D7C-EA08721FEA15}';
  tagToolButtonFilters = '{6F3E271B-B9AE-43CC-A500-3B5C14A956B6}';
  tagToolButtonSingleCell = '{DF3F0C68-569F-4007-8A19-2F57A5D78C2A}';
  tagToolButtonConst = '{01D3A3B0-0EAC-4800-A301-3164D5F7BA2C}';

  tagToolButtonMeasure = '{D3E98F07-67B1-4F6A-945A-16CE4CEA3711}';
  tagToolButtonFree = '{3A002227-2DD2-4388-9B9B-3C95571D244B}';
  tagToolButtonResult = '{4D3C3145-41CC-4230-82F6-7D631A429F37}';
  tagToolButtonTotalConst = '{4D466AD6-D4DA-49D4-838A-4F327981B425}';

  tagToolButtonRefresh = '{B5037A63-75FA-47BC-9379-F069960B206A}';
  tagToolButtonSheetProperties = '{3E48B006-6D29-4D83-9F1D-1BD6C5E3095A}';
  tagToolButtonShowParams = '{09731BBB-FF36-4D92-9446-E3C2BBDE4B1B}';
  tagToolButtonShowConsts = '{A2132F5B-1FA7-4CB9-A863-FE982837B59E}';

  tagToolButtonShowCopyForm = '{3D96E772-E0B4-453A-AF35-D4CAA9997974}';
  tagMenuButtonShowCopyForm = '{DFC277A3-7AD4-4EC0-BF2A-6CC3B3CE5F09}';

  tagMenuDataCollectionForm = '{E4EBFEC4-95B2-4C0F-9754-8D53DC803A6B}';

  tagMenuConnection = '{E9852E08-C656-43EA-BDF8-FC9E34ED0D9A}';
  tagMenuConstructorWizard = '{ADA9001E-BAAB-4C75-B2A1-E63A96E532A1}';

  tagToolButtonSplitterPad = '{A0D99BAD-CFA7-41FA-9F0F-3775890C3780}';
  tagMenuSplitterPad = '{45820E8D-BE89-4BA3-8AB3-6DD7B7FFAAD7}';

  tagMenuButtonAdd = '{50F2E3FD-3542-4CDF-A872-8BC59A036666}';
  tagMenuButtonTotals = '{BA0693C1-0BBC-4214-99EE-7598C560D901}';
  tagMenuButtonRows = '{00E34286-ABC9-4588-BE43-070967C4C1BE}';
  tagMenuButtonColumns = '{FC87A688-568E-44C6-9488-E2F5AEEBD023}';
  tagMenuButtonFilters = '{679684BB-1B29-48DB-BDE3-F07703605210}';
  tagMenuButtonSingleCell = '{15A5D99E-07A6-4AC4-AA1E-5F54FD94A714}';
  tagMenuButtonConst = '{85E6437F-3597-47A1-9B7B-1293B36C5634}';

  tagMenuButtonMeasure = '{013FDA03-4128-4E47-BE20-F2BB448E56F1}';
  tagMenuButtonFree = '{C2E07BFC-2754-43E5-A0C1-67909AEEFFBC}';
  tagMenuButtonResult = '{1E44A571-0BAD-4763-A5DC-4C65A3CE5546}';
  tagMenuButtonTotalConst = '{B86E5F3D-5DBB-4C81-A7D8-DB142F365A2D}';

  tagMenuSheetInfo = '{A4C14181-5A5E-4780-B31F-CCD12C339019}';
  tagMenuSheetHistory = '{8AEE24F7-5CBF-41FC-A4D6-27334A4C298A}';
  tagMenuSheetProperties = '{A3D5BAC5-61E5-45F5-A19D-B5C81D441875}';
  tagMenuCommonOptions = '{51EA7A29-1054-4DE4-A7B7-8E6C9529A011}';
  tagMenuShowParams = '{628E1B65-AB0C-407F-9FA0-1F2CA00FD6C0}';
  tagMenuShowConsts = '{4F52D25C-6AB5-4E87-B7BD-3A8EA434E571}';
  tagMenuRefresh = '{72C0B83B-4D7C-4225-8A4B-C083696873B6}';
  tagMenuShowCacheManager = '{POB4B4NW-V3C6-4Q9E-BDF8-FC9E34EZ3A2A}';
  tagToolButtonSendData = '{ED42575F-2BB2-4F89-999C-0DFE6996D7B2}';
  tagMenuSendData = '{6C138EA5-7CB3-47BE-AD02-E5D0D7BCC015}';
  tagToolButtonMarkEmpty = '{BA0AC9F1-2235-4AB7-B4A8-A0E7471C2D2B}';
  tagMenuMarkEmpty = '{57683563-1085-4AA1-BF2C-A96CB02211F6}';
  tagMenuComponentEditor = '{C69F8F40-5B88-4F73-A7BB-862014C6B028}';
  tagToolButtonComponentEditor = '{A3F328B5-311E-434C-8B3A-AD248F53C01F}';
  tagMenuReplication = '{83D018C8-2C3F-4F07-9EDD-24306B37A38D}';

  tagToolButtonSingleCellsManager = '{235914DB-88B1-4D25-8748-493F40E0B6AA}';
  tagMenuSingleCellsManager = '{494643EC-1EB6-4BC7-912C-B4E8FC231A8D}';

  tagToolButtonWorkMode = '{73E6EC36-E50B-47F2-A3CF-A1E5CC1B0486}';
  tagMenuWorkMode = '{53E99146-5989-48B0-B024-447827B1D1D1}';

  {таги для контекстного меню}
  tagPopupMenuStyleEdit = '{B68792EE-7391-433A-8C53-3E5D0A11F8E7}';
  tagPopupMenuRowEdit = '{F82CE1E1-219B-4B1C-8BBA-70B332286167}';
  tagPopupMenuRowDel = '{87C4766C-304C-478F-9794-7D924532024A}';
  tagPopupMenuInsertNewLine = '{5EDDF875-4510-403A-A910-EBF5CBDC3941}';
  tagPopupMenuColumnEdit = '{940F32E0-5BBA-4119-97A5-FD7D1F9C08F5}';
  tagPopupMenuColumnDel = '{4B2CF7DC-590A-461F-96C1-D3F82CB88AEB}';
  tagPopupMenuTotalMeasureEdit = '{4136FE99-8F63-46B1-B975-13D5C5B20A39}';
  tagPopupMenuTotalMeasureDel = '{75DC6F5C-AE5F-4412-9976-9F220D238807}';
  tagPopupMenuTotalFreeEdit = '{71B576C4-24C9-4CCA-837D-A4DE62A64B5E}';
  tagPopupMenuTotalFreeDel = '{FFBDA11D-E25A-4E9D-9394-5A0E5AF94BC0}';
  tagPopupMenuTotalResultEdit = '{81498329-C631-4856-BC38-4476005E4058}';
  tagPopupMenuTotalResultDel = '{A3E326C9-BF7A-4738-BF1C-405FF17CBF1F}';
  tagPopupMenuTotalConstDel = '{2BFB5BB9-4E36-4EF6-843A-E3945C31CA14}';
  tagPopupMenuTotalConstEdit = '{CC3C685B-545F-44AD-885A-84BECA84B7D1}';
  tagPopupMenuTotalResultMarkEmpty = '{AD545C2C-F773-46F5-9470-DFCA7A9A0272}';
  tagPopupMenuFilterEdit = '{A165AAD8-5160-40A6-B3A0-8EC3534E50EF}';
  tagPopupMenuFilterDel = '{818C0FDA-6787-4786-A780-E1E7F65E80EA}';
  tagPopupMenuLevelDel = '{E9085E0A-2187-4A80-BD4A-CD491BC13C07}';
  tagPopupMenuMemberGroup = '{8ACAE1D1-0D0D-40C0-8F13-9E3C9043B1CE}';
  tagPopupMenuMoveToAxis = '{68DF8C34-CB20-4CF8-9AB7-B1A9B3B6AFE0}';
  tagPopupMenuMoveToAxis2 = '{B9AB68C9-1F4C-4450-83BF-40F57699177E}';
  tagPopupMenuMoveToFilters = '{870ECEF1-7508-457A-AC84-903C2442C8DD}';
  tagPopupMenuSingleCellNew = '{F31B0FED-F7C6-435A-BD0B-3AE28EAC8951}';
  tagPopupMenuSingleCellEdit = '{4A893128-AB69-4A2C-8A79-E7047AD669CA}';
  tagPopupMenuSingleCellRefresh = '{81BC4016-CC78-4855-9609-E70E49435909}';
  tagPopupMenuSingleCellDel = '{F43EC511-2419-494E-9959-3462AE3C85D5}';
  tagPopupMenuTableRefresh = '{56F78E4E-2232-4097-AD26-3C0E731522C7}';
  tagPopupMenuCellConstNew = '{92A70E35-7E5C-404E-BCB1-AFA0341738F8}';
  tagPopupMenuCellConstRefresh = '{BF759F3C-7DD2-43AC-9B03-595C7B4FEE8E}';
  tagPopupMenuCellConstEdit = '{46C8024B-A197-4ADC-B0D2-1F0233C12879}';
  tagPopupMenuCellConstDel = '{30FD2595-949C-4317-861C-4B6323A01B2F}';
  tagPopupMenuSetTypeFormula = '{86A278DA-43CC-4972-823E-57F5EEB74DA6}';
  tagPopupMenuElementProperties = '{0FB8988E-11FC-459B-85A1-C2A8D963DBA8}';
  tagPopupMenuSwitchTotalType = 'EF4EDFDE-AA2F-4FCB-AC50-2F59E95A0A58}';
  tagPopupMenuRefreshOneTotal = '{7FA0B41F-3E71-41C8-BF5B-CFD1EA28F08B}';
  tagPopupMenuFreezePanes = '{C68EEDF0-54A3-4933-986E-245B19D6437F}';
  tagPopupMenuHideTotalColumns = '{5F96CE95-7915-4519-9905-29C62B077F2B}';

  {таги статусбара}
  tagStatusBarRefreshDate = '{DC40C219-419B-4862-8BF2-51F36507379F}';
  tagStatusBarIsConnectedToTask = '{C46F4808-4331-41B9-9813-97A856427B66}';
  tagStatusBarTaskID = '{806622FB-B14C-4CDE-A14A-34F73B8B38F0}';
  tagStatusBarOffline = '{0F5DB58A-6B2B-466F-9E7A-25978EEA901F}';
  tagStatusBarOnlineMode = '{3FDCCBA8-7DD1-4289-8CA7-DAA7D80AA87F}';
  tagStatusBarContext = '{7BCA6E26-FCFE-4CB1-B852-D252C4494167}';

  {следующие имена введены для обеспечения "умной" очистки таблицы по ее
  составным частям, а не как раньше - целиком}
  sntColumnsArea = 'ca'; //все, относящееся к оси столбцов (ось, заголовки, МР)
  sntRowsTotalsArea = 'rta';
  sntColumnsAndMPropsArea = 'campa'; //столбцы и MP

  sntFiltersBreak = 'filtersbreak';
  sntUnitMarkerBreak = 'unitmarkerbreak';
  sntColumnTitlesBreak = 'columntitlesbreak';
  sntColumnsBreak = 'columnsbreak';
  sntRowTitlesBreak = 'rowtitlesbreak';
  sntRowsBreak = 'rowsbreak';

  sntRowsMPArea = 'rmpa';
  sntColumnsMPArea = 'cmpa';
  sntRowsMP = 'rmp';
  sntColumnsMP = 'cmp';

type
  {типы объектов, размещаемых на листе планирования}
  TPSObject = (psoNone, psoTable, psoColumns, psoColumnTitles,
    psoColumnDimension, psoColumnLevel, psoColumnLevelTitle, psoRows,
    psoRowTitles, psoRowDimension, psoRowLevel, psoRowLevelTitle,
    psoTotals, psoTotalTitles, psoTotalMeasure, psoTotalMeasureTitle,
    psoTotalFree, psoTotalFreeTitle, psoTotalResult, psoTotalResultTitle,
    psoFilter, psoFilterArea, psoColumnsArea, psoRowsTotalsArea,
    psoColumnsAndMPropsArea, psoMember, psoSheetId, psoTestMark, psoUnitMarker,
    psoSingleCellMeasure, psoSingleCellResult, psoSingleCellConst, psoFiltersBreak,
    psoUnitMarkerBreak, psoColumnTitlesBreak, psoColumnsBreak, psoRowTitlesBreak, psoRowsBreak);
  TPSObjects = set of TPSObject;
const

  PSObjectNames: array[TPSObject] of string = ('', sntTable, sntColumns, sntColumnTitles,
    sntColumnDimension, sntColumnLevel, sntColumnLevelTitle, sntRows,
    sntRowTitles, sntRowDimension, sntRowLevel, sntRowLevelTitle,
    sntTotals, sntTotalTitles, sntTotalMeasure, sntTotalMeasureTitle,
    sntTotalFree, sntTotalFreeTitle, sntTotalResult, sntTotalResultTitle,
    sntFilter, sntFilterArea, sntColumnsArea, sntRowsTotalsArea,
    sntColumnsAndMPropsArea, sntMember, sntSheetId, sntTestMark, sntUnitMarker,
    sntSingleCellMeasure, sntSingleCellResult, sntSingleCellConst, sntFiltersBreak,
    sntUnitMarkerBreak, sntColumnTitlesBreak, sntColumnsBreak, sntRowTitlesBreak, sntRowsBreak);

  { сообщение о пользовании тестовой версией листа планирования}
  swTestVersion = 'Товарищ! Если делаешь нужные листы, пользуйся только стабильными версиями надстройки!';


  //режимы запуска мастера
  mrcwAdd = 'Add';
  mrcwModif = 'Modif';
  mrcwDel = 'Del';

  mrcwNone = 'None';
  //режимы отображения мемберов
  msmHidden = 'Hid';
  msmVisible = 'Vis';

  //фиктивный параметр
  fpEnd = 'end';
  fpLeafEnd = 'leaf';
  fpDummy = 'dummy';
  //имя узла итогов
  ntSummary = 'Summary';
  ntMember = 'Member';
  ntMemberDummy = 'MemberDummy';
  ntSummaryDummy = 'SummaryDummy';
  ntAliasInfo = 'AliasInfo';
  ntParentInfo = 'ParentInfo';
  ntInfluences = 'Influences';
  ntInfluence = 'Influence';
  //подписи итогов
  stGrand = 'Общие итоги';
  stUsual = 'Итоги';
  stDetailedUsual = 'Итого по «%s»';
  // разметка общих итогов
  gsRow = 'gsRow';
  gsColumn = 'gsColumn';


  attrAxisIndex = 'axisindex';
  attrLevelIndex = 'levelindex';
  attrMemberLeaf = 'memberleaf';
  attrIsItSummary = 'IsItSummary';
  attrPermitEditing = 'permitediting';
  attrInConstructionMode = 'inconstructionmode';
  attrIsSequelSheet = 'issequelsheet';
  attrTotalSections = 'totalsections';
  attrNodeId = 'nodeid';
  attrProviderId = 'providerid';
  attrShift = 'shift';
  attrWbWorthy = 'wbworthy';
  attrAlias = 'alias';

  { Ограничение на максимальное количество столбцов, в пределах которых
    размещается таблица. При превышении этого предела таблица становится
    "широкой", размещаемой на нескольких листах. В норме должно быть 256.
    Меньшие значения используются для удобства отладки и тестирования.}
  SheetColumnsLimitation = 256;

  BeastNumber = 666;

  ParamsDivider = '$$$';
  ValuesDivider = '^^^';

  // коды ошибок для функций валидации элементов
  ecNoDimension = 1;
  ecNoHierarchy = 2;
  ecNoLevel = 3;
  ecNoSelection = 4;
  ecNoCube = 5;
  ecNoMeasure = 6;
  ecDuplicateFilters = 7;
  ecUnfitDimensions = 8;
  

implementation

end.

