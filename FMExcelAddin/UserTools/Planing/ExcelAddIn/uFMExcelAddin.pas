{
  Заглавный модуль плагина.
  Реализует COM-интерфейс IFMPlanningExtension, который собственно и позволяет
  зарегистрировать DLL-ку как плагин к экселю.
  По сути, модуль является диспетчером верхнего уровня всех объектов системы.
  Здесь они создаются, инициализируются, здесь создается встраиваемый
  пользовательский интерфейс, здесь обработчики событий экселя.
  (!) Модуль должен содержать минимум бизнес-логики работы листа планирования.
}

{$A+,B-,C+,D+,E-,F-,G+,H+,I+,J+,K-,L+,M-,N+,O-,P+,Q-,R-,S-,T-,U-,V+,W-,X+,Y+,Z1}
{$MINSTACKSIZE $00004000}
{$MAXSTACKSIZE $00100000}
{$IMAGEBASE $00400000}
{$APPTYPE GUI}
unit uFMExcelAddin;

interface

uses
  ComObj, ActiveX, FMExcelAddin_TLB, Forms, OfficeXP, ExcelXP, ComAddInUtils,
  uConstructorWizard, uSheetCollector, PlaningProvider_TLB,
  controls, uXMLCatalog, FileCtrl, Registry, classes, MSXML2_TLB, uXmlUtils,
  uPropertiesForm, uParamControl, uConstControl, uConstProperties,
  PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinExcelUtils, uFMAddinRegistryUtils, uFMAddinXMLUtils, uSplitterPad,
  uSheetHistory, uSheetObjectModel, uCopyForm,
  uSingleCellEditor, uSingleCellManager, uExcelUtils, ExcelComAddinEvents,
  Planing_TLB, uGlobalPlaningConst, uOfficeUtils, uSheetComponentEditor, uEvents,
  uAppendEmptyRowToSheet, uOfflineFiltersEditor, uDataCollectionFormWizard,
  uNewConnection, ExtCtrls, uWritebackOptions, 
  uOptions, uTaskContextProvider, uProcessFormSink, uReplicationWizard,
  uFMExcelAddinInterface, uSheetInfo;


{ Плагин должен работать с версиями Office XP и выше.
  Работаем TLB-хой от XP }

type
  //Версии MS Excel
  TMicrosoftExcelVersion = (evXP, ev2003, evUnknown);

  {}
  TRebuildCycleState = (rcsWait, rcsLeave, rcsReturn);

  TFMExcelAddIn = class(TFMExcelAddininterface, IFMPlanningExtension,
                        IFMPlanningAncillary, IFMPlanningVBProgramming,
                        IFMPlaningInteraction)
  private
    FExcelVersion: TMicrosoftExcelVersion; // версия экселя
    FSplitterPad: TfrmSplitterPad;
    FXMLCatalog: TXMLCatalog;
    FfrmConstructorWizard: TfrmConstructorWizard; // мастер
    FfrmProperties: TfrmProperties;               //окно свойств
    FfrmSheetHistory: TSheetHistory;              //окно истории листа
    FUserEvents: TExcelEvents;                    //пользовательские события
    NeedEvidentAuthentication: boolean; // в первый раз за сеанс надо спросить явно
    NeedForceRefresh: boolean; //нужно ли принудительное обновление измерений
    VersionOK: boolean;

    Timer: TTimer;

    {Информация об аутентификации, инициализируется из задачи}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;


    // форма индикации процесса
    FProcess: IProcessForm;
    FProcessFormSink: TProcessFormSink;
    // контексты задач
    FTaskContextProvider: TTaskContextProvider;

    // флаг массового запуска процедур плагина
    FIsMassCall: boolean;

    { Информация об областях листа, пригоднях для записи.
      Является копией оригинала, хранимого в TSheetInterface.
      Синхронизируется непосредственно после перестроения листа и
      при переключении контекста метаданных.}
    FWritablesInfo: TWritablesInfo;

    SingleCellCanBePlacedHere: boolean;
    {Список имен листов текущей книги, пополняемый по мере их активации.
      Использется для позиционирования на начало таблицы при первом открытии.}
    SheetsOpened: TStringList;

    RebuildCycleState: TRebuildCycleState;

    function GetCommentForHistory(WizardRunMode: TWizardRunMode; ID: string): string;
    {Возвращает true если диапазон принадлежит свободному показателю  или
    результату расчёта}
    function EditableTotalSelected(ExcelSheet: ExcelWorksheet;
      Target: ExcelRange): boolean;

    procedure StoreSheetConnectionToRegistry;
    {Подключаемся к серверу}
    function TryConnection(Force: boolean): boolean;

    {устанавливает подключение, загружает каталог и метаданные в указанном режиме}
    function PrepareRebuild(LoadMode: TLoadMode; out ExlSheet: ExcelWorkSheet): boolean;
    {Перестроить таблицу с редактированием мастером}
    procedure RebuildSheet(WizardRunMode: TWizardRunMode; ID: string); overload;
    {Перестроить таблицу без редактирования (просто рефреш)}
    function RebuildSheet: boolean; overload;
    {Непосредственная перестройка листа. Метадата уже должна быть подготовлена}
    function DoRebuildSheet(ExcelSheet: ExcelWorksheet): boolean;
    {Действия после обновления листа}
    procedure AfterRebuild(ExcelSheet: ExcelWorksheet; IsSuccess: boolean);

    { Отложенный сбор данных с листа( Quest 7309). Подразумевается,
      что текущая модель уже содержит изменения структуры, внесенные
      пользователем при редактировании, поэтому используем временный экземпляр,
      который грузим из СР, тем самым получая лист до внесения изменений.}
    procedure CollectDeferredTotalsData;

    {Возвращает true если Target пересекается c любым компонентом листа}
    function SheetComponentIntersect(ExcelSheet: ExcelWorkSheet;
      Target: ExcelRange): boolean;
    {Присоединяет диапазон к разрывам на листе(определяет нужность этого
    объединения по набору параметров)}
    procedure NewRangeToBreaks(ExcelSheet: ExcelWorksheet; Target: ExcelRange);
    {Обновляет панель разрывов в соответсвии с текущим состоянием листа}
    procedure RefreshSplitterPad;
    {Анализирует нужность установки зашиты на лист}
    procedure ProtectionAnalisis(const ExcelSheet: ExcelWorksheet;
      const Target: ExcelRange);
    {Получаем ошибки отображенные в форме процесса}
    function GetErrorList: string;
    {Отключаем режим копирования в Excel-e}
    procedure DisableCopyMode;
    {Вкл/выкл пункт меню "добавить отдельную ячейку"}
    procedure EnableSingleCellAppend;

    {дополнительная инициализация, нужна для корректной работы формы процессов}
    procedure AncillaryInit;
    {Считывание версии Excel. Инициализация поля FExcelVerison}
    procedure RecognizeExcelVersion;
    { При смене текущего режима работы с листом ("конструирование" или
      "работа с данными") обновляет состояние кнопок и пунктов меню}
    procedure UpdateWorkModeButton(AButton: CommandBarButton);
    { Зависит от "режима конструктора", вызывать после UpdateWorkModeButton}
    procedure UpdateStatusBar(ExcelSheet: ExcelWorkSheet);
    {Обновляет формулы в указанном листе... Имеет смысл вызывать при не
    заблокированной метадате, иначе если в формуле содержится ссылка на наши
    VB методы, они просто не будут выполняться}
    procedure CalculateSheet(ExcelSheet: ExcelWorksheet);
    function TrySwitchWorkbookOffline: boolean;
    procedure OnTimerHandler(Sender: TObject);

    function GetProcessLogFileName: string;
    procedure UpdateProcessLogging;

    {Проверяет объектную модель на наличие показателей нужного типа}
    function CheckTotalsByType(CheckType: TSheetTotalType): boolean;
    function CheckTotalsForWriteback: boolean;
    function CheckTotalsForCollect: boolean;

    procedure SetAuthenticationInfoNew;
    procedure SetConnectionStrNew;
    procedure SetTaskConnectionLoadNew;

    function GetEnvironment: TTaskEnvironment;
    function GetStatic: boolean;
    function GetContext: TTaskContext;
    function GetActiveWorkBookName: string;

    {Контекст передается в книгу либо через свойства файла либо через
      интерфейс IFMPlaningInteraction. После получения его надо применить
      ко всем листам книги. Этот метод функционально повторяет *старую*
      реализацию SetTaskContext}
    procedure ApplyTaskContext;
    procedure DoMassAction;
    procedure LoadContextFromTask(ShowGears: boolean);

    function CheckAxesSaved(ExcelSheet: ExcelWorkSheet): boolean;

    // обработчик возврата в мастер при закрытии формы индикации
    procedure OnProcessFormReturn;
    procedure OnProcessFormClose;
    procedure OnProcessFormShow;

    function LoadCatalogAndMetadata(LoadMode: TLoadMode; ESheet: ExcelWorkSheet): boolean;

    // контекст задач
    property Context: TTaskContext read GetContext;
    // имя активной книги
    property ActiveWorkBookName: string read GetActiveWorkBookName;
    property Environment: TTaskEnvironment read GetEnvironment;
    {возможность смены подключения}
    property Static: boolean read GetStatic;

  protected

    {IDTExtensibility2}
    procedure BeginShutdown(var custom: PSafeArray); override; safecall;
    procedure OnAddInsUpdate(var custom: PSafeArray); override; safecall;
    procedure OnConnection(const HostApp: IDispatch; ext_ConnectMode: integer;
      const AddInInst: IDispatch; var custom: PSafeArray); override; safecall;
    procedure OnDisconnection(ext_DisconnectMode: integer;
      var custom: PSafeArray); override; safecall;
    procedure OnStartupComplete(var custom: PSafeArray); override; safecall;

    {Обработчики листа}
    procedure SheetActivate(const Sh: IDispatch); override;
    procedure BeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
      var Cancel: WordBool); override;
    procedure OnSheetChange(const Sh: IDispatch; const Target: ExcelRange); override;
    procedure OnSheetSelectionChange(const Sh: IDispatch; const Target: ExcelRange); override;
    {Обработчики книги}
    procedure WorkbookActivate(const Wb: ExcelWorkbook); override;
    procedure WorkbookDeactivate(const Wb: ExcelWorkbook); override;
    procedure WorkbookOpen(const Wb: ExcelWorkbook); override;
    procedure WorkbookBeforeSave(const Wb: ExcelWorkbook; SaveAsUI: WordBool; var Cancel: WordBool); override;
    procedure WorkbookBeforeClose(const Wb: ExcelWorkbook; var Cancel: WordBool); override;

    { обработчики }

    { Обратная запись}
    procedure SendDataHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SendDataOptionalHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { Управление параметрами и константами}
    procedure ShowParamsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ShowConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure AddConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure EditConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure RefreshConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { Мастера и редакторы}
    procedure ConstructorWizardHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ComponentEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SingleCellEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SingleCellsManagerHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure OfflineFiltersEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure DataCollectionFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ReplicationHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { Обновление листа}
    procedure RefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure RefreshHandler2(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure OneTotalRefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    {Вспомогательные формы}
    procedure ConnectionHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure EditStyleHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetHistoryHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SplitterPadHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure CopyFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetPropertiesHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure CommonOptionsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetInfoHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    {Прочие}
    procedure MoveElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure DeleteElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure InsertNewLineHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure MarkEmptyHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SetTypeFormulaHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure WorkModeHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SwitchTotalTypeHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure FreezePanesHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure HideTotalColumnsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;

    {Следят за включенностью кнопок на наших контролах}
    procedure CommandBarsUpdate; override;



    { IFMPlanningExtension }
    {Получить значение свойства}
    function GetPropValueByName(const PropName: WideString): WideString; safecall;
    {Установить значение свойства}
    procedure SetPropValueByName(const PropName: WideString; const PropValue: WideString); safecall;
    {Разрывает старое соединение и по-тихому установливает новое с заданными
    параметрами}
    function SetConnectionStr(const URL, Scheme: WideString): HResult; safecall;
    {событие прикрепление\открепление документа к задачам}
    procedure OnTaskConnection(IsConnected: WordBool); safecall;
    {OBSOLETE
      Установка контекста параметров с интерфейсом задач}
    procedure SetTaskContext(const taskContext: IDispatch); overload; safecall;
    function  Get_IsSilentMode: WordBool; safecall;
    procedure Set_IsSilentMode(Value: WordBool); safecall;
    function  Get_ProcessForm: IProcessForm; safecall;
    procedure Set_ProcessForm(const Value: IProcessForm); safecall;
    function  Get_IsLoadingFromTask: WordBool; safecall;
    procedure Set_IsLoadingFromTask(Value: WordBool); safecall;
    property IsLoadingFromTask: WordBool read Get_IsLoadingFromTask write Set_IsLoadingFromTask;

    function RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; safecall;
    function RefreshActiveSheet: WordBool; safecall;
    function RefreshActiveBook: WordBool; safecall;

    function WritebackActiveSheet: WordBool; safecall;
    function WritebackActiveBook: WordBool; safecall;
    function WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;
    function WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;

    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString;
      const PwdHash: WideString); safecall;
    procedure AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType; 
                                   const Comment: WideString; Success: WordBool); safecall;
    { IFMAncillary }
    function QueryRange(const ACaption: WideString;
      const APrompt: WideString; AllowMultiArea: WordBool): IDispatch; safecall;
    { IFMPlanningVBProgramming }
    // получить\задать свойство книги
    function VBGetPropertyByName(const PropertyName: WideString): WideString; safecall;
    procedure VBSetPropertyByName(const PropertyName: WideString; const PropertyValue: WideString); safecall;
    // получить\задать константу
    function VBGetConstValueByName(const ConstName: WideString): WideString; safecall;
    function VBSetConstValueByName(const ConstName: WideString;
      const ConstValue: WideString): WordBool; safecall;
    // обновить
    function VBRefresh: WordBool; safecall;
    // обратная запись
    function VBWriteback: WordBool; safecall;
    // получить текущее соединение
    function VBGetCurrentConnection(var URL: WideString; var SchemeName: WideString): WordBool; safecall;
    // получить выбранные элементы измерения
    // возвращает двумерный массив [[UniqueName, Name]..[UniqueName, Name]]
    function VBGetMembers(const DimensionName: WideString): OleVariant; safecall;
    // задать выбранные элементы измерения
    // набор элементов задается одномерным массивом уникальных имен
    procedure VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant); safecall;
    // получить значение параметра
    // возвращает двумерный массив [[UniqueName, Name]..[UniqueName, Name]]
    function VBGetParamMembers(const ParamName: WideString): OleVariant; safecall;
    // задать значение параметра
    // набор элементов задается одномерным массивом уникальных имен
    procedure VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant); safecall;
    // редактировать элементы измерения через форму
    function VBEditMembers(const DimensionName: WideString): WordBool; safecall;
    // получить свойство элемента измерения, элемент задается уникальным именем
    function VBGetMemberProperty(const DimensionName: WideString; const UniqueName: WideString;
                                 const MemberPropertyName: WideString): WideString; safecall;
    // получить значение показателя в ячейке
    // ячейка определяется именем показателя и двумерным массивом [[Dimension, UniqueName]..[Dimension, UniqueName]]
    function VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString; safecall;
    // получить значение отдельного показателя
    function VBGetSingleCellValue(const SingleCellName: WideString): WideString; safecall;

    {}
    procedure SetTaskContext(const TaskContextXml: Widestring; IsPacked: WordBool); overload; safecall;

  end;

var
  {Бы ли контекст?. Необходимость введения флага связана с тем что, при закрытии
  несохраненного документа сначала вызывается обработчик BeforeClose (где
  заниливается контекст), а потом BeforeSave (где контекст проверяется)}
  IsCloseWorkbook: boolean;
  AssignedContext: boolean;

implementation
uses
  ComServ, SysUtils, Windows, uSheetStyles, uFMExcelAddInConst, uConverter;

{Записывает параметры подключения листа в реестр}
procedure TFMExcelAddIn.StoreSheetConnectionToRegistry;
var
  Reg: TRegistry;
  OldUrl, OldSchemeName: string;
  ExlSheet: ExcelWorkSheet;
begin
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if not PlaningSheet.Load(ExlSheet, Context, [lmInner])then
    exit;
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if not Reg.OpenKey(RegBasePath + RegConnSection + regConnWebServiceSection, true) then
      exit;

    {Если открыт лист с подключением, отличным от прежнего, то перепрописываем
      новое подключение в реестре. Если же это новый лист, еще без
      указания подключения, то присвоим ему текущее.}
    OldUrl := Reg.ReadString(regURLKey);
    if (PlaningSheet.URL <> '') then
      Reg.WriteString(regURLKey, PlaningSheet.URL)
    else
      if not Static then
        PlaningSheet.URL := OldUrl;
    OldSchemeName := Reg.ReadString(regWebServiceSchemeKey);
    if (PlaningSheet.SchemeName <> '') then
      Reg.WriteString(regWebServiceSchemeKey, PlaningSheet.SchemeName)
    else
      if not Static then
        PlaningSheet.SchemeName := OldSchemeName;

    if (PlaningSheet.URL <> '') and (PlaningSheet.SchemeName <> '') then
      if (OldUrl <> PlaningSheet.URL) or (OldSchemeName <> PlaningSheet.SchemeName) then
      begin
        DataProvider.ClearCache;
        DataProvider.Disconnect;
      end;
    Reg.CloseKey;
  finally
    Reg.Free;
    PlaningSheet.Clear;
  end;
end;

{ Подключаемся к серверу
  Параметры:
  Force - если было старое подключение, оно разрывается в любом случае
  результат - все таки подключились или нет. }
function TFMExcelAddIn.TryConnection(Force: boolean): boolean;

var
  Silent, Enforced: boolean;
  URL, SchemeName, ErrorMsg: string;
  Op: IOperation;
  ExlSheet: ExcelWorkSheet;
begin
  result := false;

  {Если нет провайдера - это что-то ненормальное}
  if not Assigned(DataProvider) then
  begin
    PlaningSheet.ShowMessageEX(ermUnknown, msgError);
    exit;
  end;

  StoreSheetConnectionToRegistry;

  {Подключаемся: если лист открыт в контексте задач - невозможно поменять подключение}
  Silent := PlaningSheet.IsSilentMode or PlaningSheet.Process.Showing;

  {Если работаем в контексте задач, то явная аутентификация не нужна.}
  Enforced := Force or (NeedEvidentAuthentication and not Static);
  if not Static then
    Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    if not Static then
    begin
      Op.StartOperation(Host.Hwnd);
      Op.Caption := pcapConnect;
    end;

    result := Connect(DataProvider, Enforced, Silent, Static,
      FTaskAuthType, FTaskLogin, FTaskPwdHash, URL, SchemeName, ErrorMsg);
    // если было ли реальное переподключение, надо обновить измерения с сервера
    NeedForceRefresh := result;

    { Если подключились в самый первый раз, то создадим таймер, чтобы
      периодически напоминать веб-сервису о себе, чтобы сессию по таймауту
      не грохнул.}
    if NeedEvidentAuthentication and DataProvider.Connected then
    begin
      Timer := TTimer.Create(nil);
      Timer.Interval := 180000; // 3 минуты
      Timer.OnTimer := OnTimerHandler;
      NeedEvidentAuthentication := false;
    end;

  finally
    Application.ProcessMessages;
    if not Static then
      Op.StopOperation;
    SetActiveWindow(Host.Hwnd);
    Op := nil;
  end;

  {Это смена подключения, нужно очистить кэш и некоторые настройки}
  if result then
  begin
    {чтобы прописалось даже для вновь создаваемого листа}
    PlaningSheet.URL := URL;
    PlaningSheet.SchemeName := SchemeName;
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if Assigned(ExlSheet) then
      if IsPlaningSheet(ExlSheet) or PlaningSheet.IsTaskConnectionLoad then
        if PlaningSheet.Load(ExlSheet, Context, [lmInner]) then
        try
          PlaningSheet.URL := URL;
          PlaningSheet.SchemeName := SchemeName;
          PlaningSheet.Save;
        finally
          PlaningSheet.Clear;
        end;

    {Удаление ставших неактуальными вещей}
    DataProvider.ClearCache;
    if Assigned(FXMLCatalog) then
      FXMLCatalog.Clear;
    if Assigned(FfrmConstructorWizard) then
      FfrmConstructorWizard.Clear;
  end;

  result := DataProvider.Connected;
end;

procedure TFMExcelAddIn.BeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
  var Cancel: WordBool);
var
  WSheet: ExcelWorkSheet;
begin
  WSheet := GetWorkSheet(Sh);
  if not Assigned(WSheet) then
    exit;
  if VersionRelation <> svModern then
    exit;
  if not PlaningSheet.Load(WSheet, Context, lmNoFreeData + [lmNoMembers]) then
    exit;
  try
    inherited BeforeRightClick(Sh, Target, Cancel);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.BeginShutdown(var custom: PSafeArray);
begin
  inherited BeginShutdown(custom);
end;

procedure TFMExcelAddIn.OnAddInsUpdate(var custom: PSafeArray);
begin
  inherited OnAddInsUpdate(custom);
end;

procedure TFMExcelAddIn.OnConnection(const HostApp: IDispatch;
  ext_ConnectMode: Integer; const AddInInst: IDispatch;
  var custom: PSafeArray);
var
  Op: IOperation;
  InitialDelay, i: integer;
begin
  InitialDelay := ReadIntegerRegSetting(regInitialDelay, 0);
  for i := 1 to InitialDelay * 10 do
  begin
    sleep(100);
    Application.ProcessMessages;
  end;
  
  inherited OnConnection(HostApp, ext_ConnectMode, AddInInst, custom);

  Op := CreateComObject(CLASS_OPERATION) as IOperation;
  try
    Op.StartOperation(Host.Hwnd);
    Op.Caption := 'Инициализация надстройки';

    RecognizeExcelVersion;
    Application.Handle := Host.Get_Hwnd;
    // Форма индикации процессов и ее события
    FProcess := CreateComObject(CLASS_ProcessForm) as IProcessForm;
    UpdateProcessLogging;
    FProcessFormSink := TProcessFormSink.Create;
    FProcessFormSink.OnReturn := OnProcessFormReturn;
    FProcessFormSink.OnClose := OnProcessFormClose;
    FProcessFormSink.OnShow := OnProcessFormShow;
    try
    FProcessFormSink.Connect(FProcess);
    except
      on e: exception do showerror(e.message);
    end;

    FfrmSheetHistory := TSheetHistory.Create(nil);
    FfrmConstructorWizard := nil;
    FSplitterPad := nil;
    FXMLCatalog := TXMLCatalog.Create;
    PlaningSheet := TPlaningSheet.Create;
    FTaskContextProvider := TTaskContextProvider.Create;
    FUserEvents := TExcelEvents.Create;
    PlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    PlaningSheet.Events := FUserEvents;
    FUserEvents.SetExternalLinks(FProcess);
    PlaningSheet.IsSilentMode := true;
    IsLoadingFromTask := false;
    AssignedContext := false;
    VersionRelation := svModern;
    // копируем узел с настройками из локал машин, если нет в каррент юзер
    CopyToHKCU;
    DestroyUserInterface;
    InitUserInterface;
    LoadToolBarPosition(ToolBar, regBasePath + regToolBarSettingsSection);
    LoadStatusBarPosition(StatusBar, regBasePath + regToolBarSettingsSection);
    FWritablesInfo := TWritablesInfo.Create;
    SheetsOpened := TStringList.Create;
    RebuildCycleState := rcsLeave;
  finally
    Application.ProcessMessages;
    Op.StopOperation;
    Op := nil;
  end;
end;

procedure TFMExcelAddIn.OnDisconnection(ext_DisconnectMode: integer;
  var custom: PSafeArray);
begin

  if not PlaningSheet.IsSilentMode then
  begin
    SaveToolBarPosition(ToolBar, regBasePath + regToolBarSettingsSection);
    SaveStatusBarPosition(StatusBar, regBasePath + regToolBarSettingsSection);
  end;
(*
  // если ссылка на родительский AddIn установлена
  if Assigned(fHostAddIn) then begin
    // .. освобождаем его внутреннюю ссылку на наш объект
    try
      FHostAddIn.Object_ := nil;
    except
    end;
    // .. освобождаем ссылку на него
    FHostAddIn := nil;
    // теперь циклических ссылок нет
  end;*)
  DestroyUserInterface;
  FreeAndNil(FXMLCatalog);
  FreeAndNil(FfrmConstructorWizard);
  FreeAndNil(FSplitterPad);
  FreeAndNil(PlaningSheet);
  FreeAndNil(FfrmSheetHistory);
  FreeAndNil(FUserEvents);
//  FStatusBarRefreshDate := nil;
//  FStatusBarTaskID := nil;
//  Menu := nil;
//  FEmptyMenu := nil;
//  ToolBar := nil;
//  StatusBar := nil;
  FProcessFormSink.Disconnect;
  FProcessFormSink.Free;
  FProcess := nil;
//  DataProvider.FreeProvider;
//  DataProvider := nil;
  //FreeList(FTaskContextCollection);
  FTaskContextProvider.Free;
  FreeAndNil(FWritablesInfo);
  FreeAndNil(Timer);
  FreeStringList(SheetsOpened);

  inherited OnDisconnection(ext_DisconnectMode, custom);
end;

procedure TFMExcelAddIn.OnStartupComplete(var custom: PSafeArray);
begin
  inherited OnStartupComplete(custom); 
  {Читаем "контекст и окружение"}
  if Assigned(Host.ActiveWorkbook) then
  begin
    FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
    PlaningSheet.Environment := Environment;
  end;
  UpdateStatusBar(GetWorkSheet(Host.ActiveSheet));
  NeedEvidentAuthentication := true;
end;

{ обработчики }
{ подключение к серверу }
procedure TFMExcelAddIn.ConnectionHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  if not TryConnection(true) then
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

procedure TFMExcelAddIn.CopyFormHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  IsNewBook: boolean;
begin
  DisableCopyMode;
  try
    CopySheet(Host, PlaningSheet, IsNewBook);
  finally
    UpdateNeed := IsNeedUpdateSheet(GetWorkSheet(Host.ActiveSheet), VersionRelation);
    { При копировании активация нового листа происходит слишком рано,
      ДО того, как в него будут переписаны СР. Таким образом, он воспринимается
      плагином как чистый, "не наш" лист, с соответствующей реакцией тулбара
      и меню. Так как это в принципе неверно, то необходима повторная активация.}
    PlaningSheet.SpecialFlagForTaskParamCopy := true;
    if IsNewBook then
      WorkbookOpen(Host.ActiveWorkbook);
    SheetActivate(Host.ActiveSheet);
    PlaningSheet.SpecialFlagForTaskParamCopy := false;
  end;
end;

procedure TFMExcelAddIn.SheetPropertiesHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
begin
  DisableCopyMode;
  FfrmProperties := TfrmProperties.Create(nil);

  try
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    FfrmProperties.ShowProperties(ExcelSheet, FXmlCatalog, DataProvider);
  finally
    SetActiveWindow(Host.Hwnd);
    FreeAndNil(FfrmProperties);
    UpdateProcessLogging;
  end;
end;

procedure TFMExcelAddIn.CalculateSheet(ExcelSheet: ExcelWorksheet);
const
  Automatic = -4105;
  Semiautomatic = 2;
var
  Calculation: integer;
begin
  if not IsPlaningSheet(ExcelSheet) then
    exit;
  PlaningSheet.OpenOperationEx(Host.Hwnd, pfoCalculateSheet, false, true, otUpdate);
  try
    Calculation := Host.Get_Calculation(LCID);

    if (Calculation = Automatic) or (Calculation = Semiautomatic) then
      {При автоматическом режиме обновления формул, одним из пособов их обновить
      может быть вставка ячейки. ExcelSheet.Calculate(FLCID) - не работает, т.к.
      при его вызове Excel считает что формулы в этом обновлении не нуждаются}
      ExcelSheet.Cells.Item[ExcelSheet.Rows.Count, ExcelSheet.Columns.Count].Insert(xlDown, EmptyParam)
    else
      ExcelSheet.Calculate(LCID);
  except
  end;
  PlaningSheet.CloseOperationEx(true, '');
end;

procedure TFMExcelAddIn.AfterRebuild(ExcelSheet: ExcelWorksheet; IsSuccess: boolean);
begin
  try
    if not Assigned(ExcelSheet) then
      exit;
    if not IsPlaningSheet(ExcelSheet) then
    begin
      PlaningSheet.Clear;
      exit;
    end;
    if IsSuccess then
    begin
      //вынужденно. непонятное размножение иногда проскакивает.
      RecreateCp(ExcelSheet);
      PlaningSheet.ClearCPGarbage(true);
      FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
    end;
    (* !!! Закомментировано в рамках 14963  - возврат в мастер при неудачном обновлении*)
    if RebuildCycleState = rcsLeave then
      PlaningSheet.Clear;
    //чтобы эксель не терял фокус
    Application.ProcessMessages;
    //включаем защиту
    if IsPlaningSheet(ExcelSheet)then
      ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  finally
    ExcelSheet.Application.ScreenUpdating[LCID] := true;
    if Assigned(FProcess) then
      if not FProcess.Showing then
        ExcelSheet.Application.Set_Interactive(LCID, true);
    ExcelSheet.Activate(LCID);
    SetActiveWindow(Host.Hwnd);
  end;
end;

function TFMExcelAddIn.GetCommentForHistory(WizardRunMode: TWizardRunMode; ID: string): string;
begin
  result := '';
  if (not Assigned(PlaningSheet)) or (Id = '') then
    exit;
  case WizardRunMode of
    wrmEditColumn: result := 'элемент столбцов "' +
      PlaningSheet.Columns[PlaningSheet.Columns.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditRow: result := 'элемент строк "' +
      PlaningSheet.Rows[PlaningSheet.Rows.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditFilter: result := 'элемент фильтров "' +
      PlaningSheet.Filters[PlaningSheet.Filters.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditTotal: begin
      with PlaningSheet.Totals[PlaningSheet.Totals.FindByID(ID)] do
        case TotalType of                               
          wtFree: result := 'показатель "' + Caption + '"' + ' тип: Свободный';
          wtConst: result := 'показатель "' + Caption + '"' + ' тип: Константа';
          wtMeasure: result := 'показатель "' + Caption + '"' + ' тип: Мера,' +
            ' куб: "' + CubeName + '",' + ' мера: "' + MeasureName + '"';
          wtResult: result := 'показатель "' + Caption + '"' + ' тип: Результат,' +
            ' куб: "' + CubeName + '",' + ' мера: "' + MeasureName + '"';
        end;
    end;
  end;
end;

function TFMExcelAddIn.GetErrorList: string;
begin
  result := FProcess.ErrorList;
end;

procedure TFMExcelAddIn.RebuildSheet(WizardRunMode: TWizardRunMode; ID: string);
var
  ExlSheet: ExcelWorkSheet;
  IsSuccess: Boolean;
  EventForHistory: string;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExlSheet) then
  begin
    repeat
      if RebuildCycleState = rcsReturn then
        LoadCatalogAndMetadata([lmCatalogOnly], ExlSheet);
      RebuildCycleState := rcsWait;
      try
        {потребуется мастер - подготовим его если еще не готов}
        if not Assigned(FfrmConstructorWizard) then
        begin
          FfrmConstructorWizard := TfrmConstructorWizard.CreateEx(nil, PlaningSheet);
          FfrmConstructorWizard.XMLCatalog := FXMLCatalog;
          FfrmConstructorWizard.DataProvider := DataProvider;
        end;

          if FfrmConstructorWizard.RunWizard(WizardRunMode, ID) then
          begin
            AncillaryInit;
            FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, true);
            FProcess.NewProcessClear := false;

            FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"',
              not CriticalNode, NoteTime, otProcess);

            CollectDeferredTotalsData;

            PlaningSheet.NeedMapSingleCells := false;
            IsSuccess := DoRebuildSheet(ExlSheet);

            EventForHistory := MergeCommaText(FfrmConstructorWizard.EventList,
              GetErrorList);
            PlaningSheet.AddEventInSheetHistory(evtEdit, EventForHistory,
              IsSuccess);
          end
          else
            RebuildCycleState := rcsLeave;

      finally
        AfterRebuild(ExlSheet, IsSuccess);

        CalculateSheet(ExlSheet);
        if IsSuccess then
        begin
          FUserEvents.Execute(ExlSheet, enAfterRefresh);
          FProcess.CloseOperation; //pfoRebuild
        end;
        FProcess.CloseProcess;
      end;

      while RebuildCycleState = rcsWait do
      begin
        Application.ProcessMessages;
        Sleep(10);
      end;

    until RebuildCycleState = rcsLeave;
  end;
  PlaningSheet.Clear;
end;

function TFMExcelAddIn.RebuildSheet: boolean;
var
  ExlSheet: ExcelWorkSheet;
  IsSuccess: Boolean;
  HistoryText: string;
begin
  result := false;
  HistoryText := '';
  DisableCopyMode;
  IsSuccess := false;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if TryConnection(false) then
  try
    AncillaryInit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    try
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"', not CriticalNode, NoteTime, otProcess);
      if not IsPlaningSheet(ExlSheet) then
      begin
        result := true;
        exit;
      end;
      FUserEvents.Execute(ExlSheet, enBeforeRefresh);
      FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
      FXMLCatalog.SetUp(DataProvider);
      LoadContextFromTask(false);
      PlaningSheet.Clear;
      if not PlaningSheet.Load(ExlSheet, Context, lmAll) then
      begin
        if Assigned(Context) and (VersionRelation = svFuture) then
          FProcess.PostWarning('Версия листа выше версии надстройки');
        FProcess.CloseOperation;
        exit;
      end;
      if PlaningSheet.Empty then
      begin
        FProcess.CloseOperation;
        result := true;
        exit;
      end;
      if not PlaningSheet.Online then
      begin
        FProcess.PostWarning('Лист является автономной формой сбора и не подлежит обновлению');
        result := true;
        FProcess.CloseOperation;
        exit;
      end;
    finally
      //чтобы эксель не терял фокус
      Application.ProcessMessages;
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    IsSuccess := DoRebuildSheet(ExlSheet);
    result := IsSuccess;
                          
    if FIsMassCall then
      HistoryText := ConvertStringToCommaText('Массовый вызов');
    HistoryText := MergeCommaText(HistoryText, FProcess.ErrorList);
    if (HistoryText = '') and not PlaningSheet.NeedMapSingleCells then
      HistoryText := MergeCommaText(HistoryText, ConvertStringToCommaText('Обновление листа'));
    PlaningSheet.AddEventInSheetHistory(evtRefresh, HistoryText, result);
  finally
    AfterRebuild(ExlSheet, result);
    CalculateSheet(ExlSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end
  else
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

function TFMExcelAddin.DoRebuildSheet(ExcelSheet: ExcelWorksheet): boolean;
var
  CheckFormulas, Alerts: boolean;
begin
  result := false;

  if not PlaningSheet.Refresh(NeedForceRefresh) then
    exit;
  if not SetSheetProtection(ExcelSheet, false) then
  begin
    FProcess.PostError(ermWorksheetProtectionFault);
    exit;
  end;
  if PlaningSheet.Validate then
  begin
    {снимаем защиту, отключаем интерактивность}
    ExcelSheet.Application.ScreenUpdating[LCID] := false;
    ExcelSheet.Application.Set_Interactive(LCID, false);

    if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
    begin
      FProcess.PostError(ermWorkbookProtectionFault);
      exit;
    end;
    PlaningSheet.ValidateStyles;
    MayHook := false;

    {В процессе изменения листа формулы могут оказаться некорректными,
    поэтому, что бы при этом не вылезали экселевские окна, временно отключаем
    проверку корректности формул}
    CheckFormulas := Host.ErrorCheckingOptions.EvaluateToError;
    Host.ErrorCheckingOptions.EvaluateToError := false;
    Alerts := Host.DisplayAlerts[LCID];
    Host.DisplayAlerts[LCID] := false;
    try
      PlaningSheet.MapAll;                          
    finally
      //восстанавливаем проверку формул
      Host.ErrorCheckingOptions.EvaluateToError := CheckFormulas;
      Host.DisplayAlerts[LCID] := Alerts;
      //разрешаем обработчики
      MayHook := true;
    end;

    if (FProcess.LastError = '') then
    begin
      PlaningSheet.LastRefreshDate := DateTimeToStr(Now);
      SetStatusBarInfo(sbiRefreshDate, PlaningSheet.LastRefreshDate);
      PlaningSheet.IsMarkupNew := true;
      PlaningSheet.Save;
      result := true;
    end;
  end;

  RefreshSplitterPad;
end;

{ мастер структуры }
procedure TFMExcelAddIn.ConstructorWizardHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  WizardRunMode: TWizardRunMode;
  ID, DummyStr: string;
begin
  PlaningSheet.NeedMapSingleCells := true;
  if Button.Get_HelpFile = '' then
    RebuildSheet(wrmStandart, '')
  else
  begin
    ParseAdditionalInfo(Button.Get_HelpFile, WizardRunMode, ID, DummyStr);
    RebuildSheet(WizardRunMode, ID);
  end;
end;

{ отправить данные на сервер}
procedure TFMExcelAddIn.SendDataHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  WritebackActiveSheet;
end;

procedure TFMExcelAddIn.SendDataOptionalHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  NeedRewrite, NeedProcess: boolean;
begin
  if not OptionalWriteback(NeedRewrite, NeedProcess) then
    exit;
  WritebackActiveSheetEx(NeedRewrite, NeedProcess);
end;

function TFMExcelAddIn.RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; safecall;
var
  Sheet: ExcelWorksheet;
  IsVisible: XlSheetVisibility;
begin
  result := false;
  try
    Sheet := GetWorkSheet(Host.Sheets.Item[Index]);
    if not Assigned(Sheet) then
      exit;
    IsVisible := Sheet.Visible[LCID];
    try
      {$WARNINGS OFF}
      Sheet.Visible[LCID] := xlSheetVisible;
      {$WARNINGS ON}
      Sheet.Activate(LCID);
      PlaningSheet.NeedMapSingleCells := true;
      result := RebuildSheet;
    finally
      Sheet.Visible[LCID] := IsVisible;
    end;
  except
    result := false;
    IsAccessVioletion := true;
  end;
end;

function TFMExcelAddIn.RefreshActiveSheet: WordBool;
var
  IsAV: WordBool;
  Sheet: ExcelWorksheet;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  result := RefreshSheet(Sheet.Name, IsAV);
end;

function TFMExcelAddIn.RefreshActiveBook: WordBool;
var
  Sheet: ExcelWorksheet;
  Book: ExcelWorkbook;
  i: integer;
  IsAV: WordBool;
  ActiveSheet:ExcelWorksheet;
begin
  FIsMassCall := true;
  Book := Host.ActiveWorkbook;
  result := Assigned(Book);
  if result then
  begin
    ActiveSheet := GetWorkSheet(Book.ActiveSheet);
    for i := 1 to Book.Sheets.Count do
    begin
      FProcess.ClearErrors;
      Sheet := GetWorkSheet(Book.Sheets.Item[i]);
      if not Assigned(Sheet) then
        continue;
      result := RefreshSheet(i, IsAV) and result;
    end;
    if Assigned(ActiveSheet) then
      ActiveSheet.Activate(LCID);
  end;
  FIsMassCall := false;
end;

function TFMExcelAddIn.WritebackActiveBook: WordBool;
begin
  result := WritebackActiveBookEx(false, true);
end;

function TFMExcelAddIn.WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool;
var
  Sheet: ExcelWorksheet;
  Book: ExcelWorkbook;
  i: integer;
  IsVisible: XlSheetVisibility;
  ActiveSheet: ExcelWorksheet;
begin
  FIsMassCall := true;
  Book := Host.ActiveWorkbook;
  result := Assigned(Book);
  if result then
  begin
    ActiveSheet := GetWorkSheet(Book.ActiveSheet);
    for i := 1 to Book.Sheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Sheets.Item[i]);
      if not Assigned(Sheet) then
        continue;
      IsVisible := Sheet.Visible[LCID];
      try
        {$WARNINGS OFF}
        Sheet.Visible[LCID] := xlSheetVisible;
        {$WARNINGS ON}
        Sheet.Activate(LCID);
        result := WritebackActiveSheetEx(EraseEmptyCells, ProcessCube) and result;
      finally
        Sheet.Visible[LCID] := IsVisible;
      end;
    end;
    if Assigned(ActiveSheet) then
      ActiveSheet.Activate(LCID);
  end;
  FIsMassCall := false;
end;

function TFMExcelAddIn.WritebackActiveSheet: WordBool;
begin
  WritebackActiveSheetEx(false, true);
end;

function TFMExcelAddIn.WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool;
var
  TotalsDataDocument: IXMLDOMDocument2;
  ExlSheet: ExcelWorkSheet;
  InnerXml, FilterName, ErrorContent, SingleCellName: string;
  Error, TaskId: widestring;
  ErrorList, HistoryList: TStringList;
  IsSuccess: boolean;
  BatchNode: IXMLDOMNode;

  // проверка на единственность элементов фильтров
  function CheckFilters(var FilterName, Error, SingleCellName: string): boolean;
  var
    i, j: integer;
    Filters: TSheetFilterCollectionInterface;

    function CheckFilter(Filter: TSheetFilterInterface): boolean;
    begin
      CutAllInvisible(Filter.Members, true);  // перестраховка из-за 13918
      FilterName := Filter.FullDimensionName;
      result := Filter.CheckForWriteback(Error);
    end;

  begin
    result := false;
    Filters := PlaningSheet.Filters;
    for i := 0 to Filters.Count - 1 do
    begin
      // фильтры из вторичной базы не рассматриваем
      if not Filters[i].OfPrimaryProvider then
        continue;
      // фильтр должен затрагивать по крайней мере один показатель - результат
      if (Filters[i].TotalTypesAffected and ttResult) <> ttResult then
        continue;
      if not CheckFilter(Filters[i]) then
        exit;
    end;
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      if not PlaningSheet.SingleCells[i].OfPrimaryProvider then
        continue;
      if (PlaningSheet.SingleCells[i].TotalType <> wtResult) then
        continue;
      Filters := PlaningSheet.SingleCells[i].Filters;
      SingleCellName := PlaningSheet.SingleCells[i].Name;
      for j := 0 to Filters.Count - 1 do
        if not CheckFilter(Filters[j]) then
          exit;
    end;
    result := true;
  end;

  function CheckDimensions(var Error: string): boolean;
  var
    i: integer;
  begin
    Error := '';
    with PlaningSheet do
    begin
      for i := 0 to Rows.Count - 1 do
        if not Rows[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Rows[i].FullDimensionName + '"';
        end;
      for i := 0 to Columns.Count - 1 do
        if not Columns[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Columns[i].FullDimensionName + '"';
        end;
      for i := 0 to Filters.Count - 1 do
        if not Filters[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Filters[i].FullDimensionName + '"';
        end;
    end;
    result := Error = '';
    if not result then
      Error := Format(ermWritebackBadDimensions, [Error]);
  end;

  // получить полный и краткий текст ошибки
  procedure GetErrorMessage(TotalsDataDocument: IXmlDomDocument2;
                            var Error: widestring; var ErrorList: TStringList);
  var
    XPath: string;
    DomElement: IXmlDomNode;
    StackTrace, Source, ErrorId: string;
    Caption: string;
    Index: integer;
  begin
    XPath := 'Exception/RequestID';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      ErrorId := DomElement.text;
      Index := PlaningSheet.Totals.FindById(ErrorId);
      if (Index <> -1) then
        Caption := 'Показатель "' + PlaningSheet.Totals[Index].Caption + '"'
      else
      begin
        Index := PlaningSheet.SingleCells.FindById(ErrorId);
        if (Index <> -1) then
          Caption := 'Отдельный показатель "' + PlaningSheet.SingleCells[Index].Name + '"';
      end;
      ErrorList.Add('Компонент таблицы, на котором произошла ошибка:');
      ErrorList.Add(Caption);
    end;
    XPath := 'Exception/Message';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      Error := DomElement.text;
      ErrorList.Add(Error);
    end;
    XPath := 'Exception/Source';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      Source := DomElement.text;
      ErrorList.Add('Объект, вызвавший ошибку:');
      ErrorList.Add(Source);
    end;
    XPath := 'Exception/StackTrace';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      StackTrace := DomElement.text;
      ErrorList.Add('Полное сообщение об ошибке:');
      ErrorList.Add(StackTrace);
    end;  
  end;

  // сбросить состояние ячеек "удалить" - заменить их на пустоту
  procedure ClearErasedCells;
  var
    ExcelSheet: ExcelWorkSheet;
    i, k, j: integer;
    TotalRange, CellRange: ExcelRange;

    // изменить стиль ячейки
    procedure ChangeStyle(CellRange: ExcelRange; NewStyle: string);
    var
      Style: variant;
      NumberFormat: variant;
    begin
      Style := CellRange.Style;
      // сбрасываем состояние ячейки "очищенная"
      if (string(Style) = snDataFreeErased) then
      begin
        NumberFormat := CellRange.NumberFormat;
        CellRange.Style := NewStyle;
        CellRange.NumberFormat := NumberFormat;
        CellRange.Value2 := fmEmptyCell;
        CellRange.Formula := '';
      end;
    end;

  begin
    ExcelSheet := PlaningSheet.ExcelSheet;
    // очищаем ячейки показателей
    for i := 0 to PlaningSheet.Totals.Count - 1 do
    begin
      if (PlaningSheet.Totals[i].TotalType <> wtResult) then
        continue;
      for k := 0 to PlaningSheet.Totals[i].SectionCount - 1 do
      begin
        TotalRange := PlaningSheet.Totals[i].GetTotalRange(k);
        for j := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
        begin
          with ExcelSheet do
            CellRange := Range[Cells.Item[j, TotalRange.Column],
                               Cells.Item[j, TotalRange.Column]];
          ChangeStyle(CellRange, snDataFree);
        end;
      end;
    end;
    // очищаем отдельные ячейки
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      if (PlaningSheet.SingleCells[i].TotalType <> wtResult) then
        continue;
      CellRange := GetRangeByName(ExcelSheet, PlaningSheet.SingleCells[i].ExcelName);
      ChangeStyle(CellRange, snResultSingleCells);
    end;
  end;
var
  BatchID: string;
begin
  result := false;
  IsSuccess := false;
  DisableCopyMode;
  MayHook := false;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if not Assigned(Context) then
    if not TryConnection(false) then
    begin
      PlaningSheet.ShowDetailedError(ermNoneConnection,
        DataProvider.LastError, ermNoneConnection);
      exit;
    end;
  AncillaryInit;
  FProcess.OpenProcess(Host.Hwnd, ftWriteback, mWritebackSuccess, mWritebackError, false);
  FProcess.NewProcessClear := false;
  HistoryList := TStringList.Create;
  try
    FProcess.OpenOperation(pfoWriteback + ' "' + ExlSheet.Name +'"',
      not CriticalNode, NoteTime, otProcess);
    if FIsMassCall then
      HistoryList.Add('Массовый вызов');
    // пропускаем не наши листы
    if not IsPlaningSheet(ExlSheet) then
    begin
      result := true;
      FProcess.CloseOperation;  //pfoWriteback
      exit;
    end;

    { если лист не от задач - запрещаем обратную запись}
    //TaskId := GetPropValueByName(pspTaskId);
    TaskId := Environment.TaskId;
    if (TaskId = '') then
    begin
      FProcess.PostError(ermWritebackNoTaskId);
      HistoryList.Add(ermWritebackNoTaskId);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;


    {необходимо подключение}
    if not DataProvider.Connected then
      TryConnection(false);
    if not DataProvider.Connected then
    begin
      FProcess.PostError(ermNoneConnection + ': ' + DataProvider.LastError);
      exit;
    end;

    { на листах типа "отчет" обратную запись выполнять не надо}
    if (GetPropValueByName(pspSheetType) = '2') then
    begin
      result := true;
      FProcess.PostInfo('Для документа типа "Отчет" обратная запись не выполняется');
      FProcess.CloseOperation;  //pfoWriteback
      exit;
    end;

    { загрузка метаданных}
    FUserEvents.Execute(ExlSheet, enBeforeWriteBack);
    FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
    try
      FXMLCatalog.SetUp(DataProvider);
      if not PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
      begin
        FProcess.CloseOperation;
        exit;
      end;
      if PlaningSheet.Empty then
      begin
        result := true;
        FProcess.CloseOperation;
        exit;
      end;
    finally
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    // сделаем проверку на измерения из неосновной базы
    if not CheckDimensions(ErrorContent) then
    begin
      FProcess.PostError(ErrorContent);
      HistoryList.Add(ErrorContent);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;

    // проверяем на наличие показателей типа результат
    if (not CheckTotalsForWriteback) then
    begin
      FProcess.PostError(ermWritebackNoResultTotals);
      HistoryList.Add(ermWritebackNoResultTotals);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;

    // проверяем на единственность элемента фильтров
    if (not CheckFilters(FilterName, ErrorContent, SingleCellName)) then
    begin
      if (SingleCellName <> '') then
        ErrorContent := 'Отдельная ячейка "' + SingleCellName + '": ' + ErrorContent;
      ErrorContent := Format(ErrorContent, ['"' + FilterName + '"']);
      FProcess.PostError(ErrorContent);
      HistoryList.Add(ErrorContent);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;
    // получаем данные итогов
    FProcess.OpenOperation(pfoCollectWritebackData, CriticalNode, NoteTime, otProcess);
    try
      // получаем документ с данными
      TotalsDataDocument := PlaningSheet.GetWritebackData(TaskId, EraseEmptyCells, ProcessCube);
      if AddinLogEnable then
        if (not WriteDocumentLog(TotalsDataDocument, 'Данные для обратной записи.xml')) then
          FProcess.PostWarning(ermDocumentLogSaveFault);
    finally
      FProcess.CloseOperation;
    end;

    try
      if (TotalsDataDocument = nil) then
        exit;
      // проверка на присутствие данных
      if (TotalsDataDocument.documentElement.childNodes.length = 0) then
      begin
        FProcess.PostError(ermWritebackNoData);
        HistoryList.Add(ermWritebackNoData);
        PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
        exit;
      end;
      // отправляем на запись
      InnerXml := TotalsDataDocument.xml;
       (*Это кодирование вообще нафиг не нужно - провайдер оборачивает этот
          Xml в секцию CDATA*)
      //XmlEncode(InnerXml);
      if not Assigned(DataProvider) then
        exit;
      FProcess.OpenOperation(pfoWriteback, CriticalNode, NoteTime, otProcess);
      try
        InnerXml := DataProvider.Writeback(InnerXml);
      finally
        FProcess.CloseOperation;
      end;

      TotalsDataDocument.loadXML(InnerXml);
      result := not Assigned(TotalsDataDocument.selectSingleNode('//Exception'));
      if result then
      begin
        // обратная запись прошла успешно
        // сбрасываем состояния ячеек "удалить" - заменяем их на пустоту
        BatchNode := TotalsDataDocument.selectSingleNode('//BatchID');
        if Assigned(BatchNode) then
          BatchID := BatchNode.text
        else
          BatchID := 'н/д';
        FProcess.OpenOperation(pfoWritebackComplete + '. Пакет: ' + BatchID, CriticalNode, NoteTime, otProcess);
        try
          ClearErasedCells;
        finally
          FProcess.CloseOperation;
          HistoryList.Add('Пакет: ' + BatchID);
          HistoryList.Add(Format('Данные были %sзаписаны; расчет кубов %s',
            [IIF(EraseEmptyCells, 'пере', 'до'), IIF(ProcessCube, 'выполнен.', 'не выполнен.')]));
          PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, true);
          {Исключительно для того, чтобы сохранить признак MultiplicationFlag}
          PlaningSheet.Save([lmInner]);
        end;
        IsSuccess := true;
      end
      else
      begin
        // при обратной записи произошла ошибка
        ErrorList := TStringList.Create;
        try
          if FIsMassCall then
            ErrorList.Add('Массовый вызов');
          GetErrorMessage(TotalsDataDocument, Error, ErrorList);
          PlaningSheet.AddEventInSheetHistory(evtWriteBack, ErrorList.CommaText, false);
          FProcess.PostError(Error);
        finally
          FreeStringList(ErrorList);
        end;
      end;
    finally
      KillDomDocument(TotalsDataDocument);
    end;
  finally
    MayHook := true;
    AfterRebuild(ExlSheet, result);
    FreeStringList(HistoryList);
    CalculateSheet(ExlSheet);
    //Если обратная запись прошла удачно
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterWriteBack);
      FProcess.CloseOperation;  //pfoWriteback
    end;
    FProcess.CloseProcess;
  end;
end;

{ обновить данные }
procedure TFMExcelAddIn.RefreshHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  PlaningSheet.NeedMapSingleCells := true;
  RebuildSheet;
end;

procedure TFMExcelAddIn.RefreshHandler2(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  PlaningSheet.NeedMapSingleCells := false;
  RebuildSheet;
end;

procedure TFMExcelAddin.WorkbookActivate(const Wb: ExcelWorkbook);
var
  ExcelSheet: ExcelWorkSheet;
  tmpStr: string;
  Editable: boolean;
  Range: ExcelRange;
  SheetName: string;
begin
  { При смене активной книги прячем форму разрывов. Связано с 6975.}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      FSplitterPad.Hide;
  UpdateStatusBar(GetWorksheet(Host.ActiveSheet));
  SetButtonsState(true, false);
  IsCloseWorkbook := false;
  if not VersionOK then
    exit;
  FWritablesInfo.Clear;
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  {Читаем "контекст и окружение"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;
  if IsPlaningSheet(ExcelSheet)then
  begin
    PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData);
    try
      FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
      UpdateWorkModeButton(nil);
    finally
      PlaningSheet.Clear;
    end;
    {Если лист открываем в первый раз, то перейдем к началу таблицы}
    SheetName := Wb.FullName[LCID] + ExcelSheet.Name;
    if SheetsOpened.IndexOf(SheetName) < 0 then
    begin
      SheetsOpened.Add(SheetName);
      Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));
      if Assigned(Range) then
        Range.Cells.Item[1, 1].Activate;
    end;
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  end
  else
  begin
    PlaningSheet.SetDefaultPermissions;
    SingleCellCanBePlacedHere := IsSingleCellAllowed(ExcelSheet, ExcelSheet.UsedRange[LCID],
      FWritablesInfo, tmpStr, Editable);
    if not PlaningSheet.InConstructionMode then
      SingleCellCanBePlacedHere := SingleCellCanBePlacedHere and Editable;
    UpdateWorkModeButton(nil);
  end;
  StoreSheetConnectionToRegistry;
  UpdateStatusBar(GetWorksheet(Host.ActiveSheet));
end;

procedure TFMExcelAddin.WorkbookDeactivate(const Wb: ExcelWorkbook);
begin
  if Wb.IsAddin then
    exit;
  if IsCloseWorkBook then
  begin
    SetButtonsState(false, false);
    SetStatusBarInfo(sbiRefreshDate, '');
    SetStatusBarInfo(sbiTaskID, '');

    AssignedContext := not wb.Saved[LCID] and Assigned(Context);
    FTaskContextProvider.OnWorkbookClose(Wb);
  end;
end;

procedure TFMExcelAddIn.WorkbookOpen(const Wb: ExcelWorkbook);
var
  ExlSheet: ExcelWorkSheet;
  CurrBookCode, NormalBookCode: Longword;
  i: integer;
begin
  {Не будем производить проверку в одном из следующих случаев
    1 - кника является надстройкой
    2 - Происходит прикрепление к задаче
    3 - молчаливый режим
    4 - открывается книга "левого" типа}
  if Wb.IsAddin then
    exit;
  SheetsOpened.Clear;
  IsCloseWorkbook := false;

  {Читаем "контекст и окружение"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;

  {Проверка формата книги сделана раньше (IsAddin), т.к. при обращение к свойству
  FileFormat срабатывает обработчик DoWindowDeactive что влекло к ошибке описаннй
  в задаче 6164}
  NormalBookCode := xlWorkbookNormal;
  CurrBookCode := Wb.FileFormat[LCID];

  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  PlaningSheet.ExcelSheet := ExlSheet;


  {Обновление версии должно предшествовать всем остальным операциям.
    В тихом режиме обновляем все листы книги, в нормально, только текущий}
  VersionOk := true;
  if PlaningSheet.IsSilentMode then
    for i := 1 to wb.Worksheets.Count do
      VersionOK := CheckSheetVersion(GetWorkSheet(wb.Worksheets[i]), DataProvider, false, FProcess)
  else
    VersionOK := CheckSheetVersion(GetWorkSheet(Host.ActiveSheet), DataProvider, true, FProcess);


  if IsLoadingFromTask then
  begin
    SetPropValueByName(pspDocPath, ActiveWorkBookName);
    SetAuthenticationInfoNew;
    SetTaskConnectionLoadNew;
    if not PlaningSheet.IsSilentMode then
    begin
      Set_IsSilentMode(true);
      SetConnectionStrNew;
      Set_IsSilentMode(false);
    end
    else
      SetConnectionStrNew;
    ApplyTaskContext;
  end;

  if IsLoadingFromTask then
    DoMassAction;

  if IsLoadingFromTask or PlaningSheet.IsSilentMode or not
    ((CurrBookCode = NormalBookCode) or
    (CurrBookCode = 56)) then  // соответствующей константы в тлб от ХР нет
    exit;

  UpdateNeed := IsNeedUpdateSheet(ExlSheet, VersionRelation);
end;

procedure TFMExcelAddin.WorkbookBeforeClose(const Wb: ExcelWorkbook; var Cancel: WordBool);
var
  i: integer;
  SheetName: string;
  Sheet: ExcelWorkSheet;
  Index: integer;
begin
  if Wb.IsAddin then
    exit;
  IsCloseWorkbook := true;

  for i := 1 to Wb.Sheets.Count do
  begin
    Sheet := GetWorkSheet(Wb.Sheets[i]);
    if not Assigned(Sheet) then
      continue;
    SheetName := Wb.FullName[LCID] + Sheet.Name;
    Index := SheetsOpened.IndexOf(SheetName);
    if Index > -1 then
      SheetsOpened.Delete(Index);
  end;
end;

function TFMExcelAddin.SheetComponentIntersect(ExcelSheet: ExcelWorkSheet;
  Target: ExcelRange): boolean;
begin
  result := false;
  if not Assigned(ExcelSheet) then
   exit;
   result :=
    (IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntColumnsAndMPropsArea))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntColumnTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntRows))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntRowTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntTotals))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntTotalTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntUnitMarker))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntFilterArea))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntSheetId))));
end;

procedure TFMExcelAddin.NewRangeToBreaks(ExcelSheet: ExcelWorksheet; Target: ExcelRange);

  function AddRangeToName(Range: ExcelRange; Name: string): boolean;
  var
    NameRange: ExcelRange;
    TopIndexBreak, BottomIndexBreak: integer;
    TopIndexRange, BottomIndexRange: integer;
  begin
    result := false;
    if (not Assigned(Range)) or (Name = '') then
      exit;
    NameRange := GetRangeByName(ExcelSheet, BuildExcelName(Name));
    if not Assigned(NameRange) then
      exit;
    TopIndexBreak := NameRange.Row;
    BottomIndexBreak := NameRange.Row + NameRange.Rows.Count;

    TopIndexRange := Range.Row;
    BottomIndexRange := Range.Row + Range.Rows.Count;

    if TopIndexBreak =  BottomIndexRange then
    begin
      MayHook := false;
      Range.ClearFormats;
      NameRange := GetRange(ExcelSheet, TopIndexRange, 1, BottomIndexBreak - 1, ExcelSheet.Columns.Count);
      MarkObject(ExcelSheet, NameRange, Name, false);
      MayHook := true;
      result := true;
    end;
  end;

begin
  if not Assigned(ExcelSheet) then
    exit;
  //смотрим нужно ли присоединить вновь добавляемый диапазон к разрывам на листе
  if ((IntersectTableAndRange(ExcelSheet, Target)) and
    (not BreakPointSelected(ExcelSheet, Target)) and
    (not SheetComponentIntersect(ExcelSheet, Target))) then
  begin
    if (not AddRangeToName(Target, sntFiltersBreak)) then
      if (not AddRangeToName(Target, sntUnitMarkerBreak)) then
        if (not AddRangeToName(Target, sntColumnTitlesBreak)) then
          if (not AddRangeToName(Target, sntColumnsBreak)) then
            if (not AddRangeToName(Target, sntRowTitlesBreak)) then
              AddRangeToName(Target, sntRowsBreak);
  end;
end;

//обновляет панель разрывов в соответсвии с текущим состоянием листа.
procedure TFMExcelAddin.RefreshSplitterPad;
begin
  {Панели разрывов может вообще не быть или она может быть скрыта}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      if Assigned(PlaningSheet) then
        FSplitterPad.Init(PlaningSheet);
end;

procedure TFMExcelAddin.OnSheetChange(const Sh: IDispatch;
  const Target: ExcelRange);
var
  tmpIndex: integer;
  Name_: ExcelXP.Name;
  ObjType: string;
  CellValue: widestring;
  ExcelSheet: ExcelWorksheet;
  RangeName: widestring;
  i: integer;
  NameFound: boolean;
  Params: TStringList;
  Formula: string;
begin
  // ввод уже перехвачен - выходим
  if not MayHook then
    exit;
  ExcelSheet := GetWorkSheet(Sh);
  if not Assigned(ExcelSheet) then
    exit;
  NewRangeToBreaks(ExcelSheet, Target);
  NameFound := false;
  {Определим, в ячейку какого диапазона осуществлен ввод.
  Нас интересуют диапазоны показателей с форматом "Денежный"}
  for i := 1 to ExcelSheet.Names.Count - 1 do
  begin
    Name_ := ExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
    if NameWithoutRange(Name_) then
      continue;
    if IsNestedRanges(Target, Name_.RefersToRange) then
    begin
      RangeName := Name_.Name_;
      tmpIndex := Pos('!', RangeName);
      RangeName := Copy(RangeName, tmpIndex + 1, Length(RangeName) - tmpIndex);
      // возможно имя не наше
      if not ParseExcelName(RangeName, Params) then
        continue;
      try
        try
          ObjType := Params[0];
          // обрабатываем только показатели и отдельные ячейки
          if not ((ObjType = sntTotalResult) or (ObjType = sntSingleCellMeasure) or
                  (ObjType = sntSingleCellResult)) then
            continue;
          // нашли нужное имя
          NameFound := true;
          break;
        finally
          FreeAndNil(Params);
        end;
      except
      end;
    end;
  end;
  if not NameFound then
    exit;
  Formula := Target.Cells.Item[1, 1].Formula;
  if Pos('=', Formula) = 1 then
    exit; //формулы трогать не надо
  // получаем значение ячейки
  CellValue := Target.Cells.Item[1, 1].Value;
  // если не формула и не пустая ячейка, проверяем, число ли это?
  if (CellValue <> fmEmptyCell) then
  begin
    try
      StrToFloat(CellValue);
    except
      // не число - ругаемся
      ShowError(ermIncorrectCellValue + ' (' + CellValue + ')');
      Target.Cells.Item[1, 1].Value[EmptyParam] := fmEmptyCell;
      exit;
    end;
  end;
end;

// Реализация интерфейса IFMPlanningExtension
// Получение значения свойства по имени
function  TFMExcelAddin.GetPropValueByName(const PropName: WideString): WideString;
begin
  // Нужно найти свойство и вернуть его значение
  // Если свойства нет - пустая строка
  try
    result := GetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, PropName);
  except
    result := '';
  end;
  if (PropName = pspSheetType) and (result = '') then
    result := 'null';
end;

// Установка свойства
procedure TFMExcelAddin.SetPropValueByName(const PropName: WideString;
  const PropValue: WideString);
begin
  // Найти свойство по имени
  // Если такого нет - создать
  // Установить значение
  try
    SetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, PropName, PropValue);
  except
  end;
end;

procedure TFMExcelAddin.CommandBarsUpdate;
var
  cmdBars: CommandBars;
  cmdBarStandard: CommandBar;
  btnOpen, btnNew: CommandBarControl;
  Enabled: boolean;
begin
  cmdBars := Host.CommandBars;
  try
    cmdBarStandard := cmdBars['standard'];
    btnNew := cmdBarStandard.FindControl(msoControlButton, 18, EmptyParam,
      EmptyParam, EmptyParam);
    btnOpen := cmdBarStandard.FindControl(msoControlButton, 23, EmptyParam,
      EmptyParam, EmptyParam);
  except
  end;
  {Отключаем все кнопки без дополнительного анализа если, нет ни одной книги}
  if (Host.Workbooks.Count = 0) then
    Enabled := false
  else
    if Assigned(btnOpen) then
      Enabled := btnOpen.Enabled
    else
      if Assigned(btnNew) then
        Enabled := btnNew.Enabled
      else
        Enabled := true;
  SetButtonsState(Enabled, true);
end;

procedure TFMExcelAddIn.SheetActivate(const Sh: IDispatch);
var
  ExlSheet: ExcelWorkSheet;
  Range: ExcelRange;
  SheetName: string;
begin
  if PlaningSheet.NeedHostEventsDisabled then
    exit;
  FWritablesInfo.Clear;
  { При смене активного листа прячем форму разрывов. Связано с 6975.}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      FSplitterPad.Hide;
  IsCloseWorkbook := false;
  ExlSheet := GetWorkSheet(Sh);
  UpdateStatusBar(ExlSheet);
  if not Assigned(ExlSheet) then
    exit;

  {Читаем "контекст и окружение"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;


  if CheckSheetVersion(ExlSheet, DataProvider, (not PlaningSheet.IsSilentMode),
    FProcess) then
      if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
        try
          FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
          UpdateWorkModeButton(nil);
          if PlaningSheet.SpecialFlagForTaskParamCopy then
            PlaningSheet.Save;
        finally
          PlaningSheet.Clear;
        end
      else
      begin
        PlaningSheet.SetDefaultPermissions;
        UpdateWorkModeButton(nil);
      end;
  UpdateStatusBar(ExlSheet);
  StoreSheetConnectionToRegistry;
  if IsPlaningSheet(GetWorkSheet(ExlSheet))then
  begin
    {Если лист открываем в первый раз, то перейдем к началу таблицы}
    SheetName := (ExlSheet.Parent as ExcelWorkbook).FullName[LCID] + ExlSheet.Name;
    if SheetsOpened.IndexOf(SheetName) < 0 then
    begin
      SheetsOpened.Add(SheetName);
      Range := GetRangeByName(ExlSheet, BuildExcelName(sntTable));
      if Assigned(Range) then
        Range.Cells.Item[1, 1].Activate;
    end;
    ProtectionAnalisis(GetWorkSheet(ExlSheet), Host.ActiveCell);
  end;
end;

procedure TFMExcelAddIn.SplitterPadHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExlSheet: ExcelWorkSheet;
begin
  {Если формочка еще не создана (первый вызов), тогда создаем}
  if not Assigned(FSplitterPad) then
  begin
    FSplitterPad := TfrmSplitterPad.Create(nil);
    FSplitterPad.ParentWindow := Host.Hwnd;

    {Позиционирование на глаз}
    FSplitterPad.Top := 200;
    FSplitterPad.Left := round(Host.ActiveWindow.Width) - 100;
  end;

  {Если уже видна - прячем, если не видна - показываем}
  if FSplitterPad.Visible then
    FSplitterPad.Hide
  else
  begin
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExlSheet) then
      exit;
    if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
    try
      FSplitterPad.Init(PlaningSheet);
      if not FSplitterPad.Visible then
        FSplitterPad.Show;
    finally
      PlaningSheet.Clear;
    end;
  end;
end;

procedure TFMExcelAddIn.OnSheetSelectionChange(const Sh: IDispatch;
  const Target: ExcelRange);
var
  ExcelSheet: ExcelWorkSheet;
begin
  IsCloseWorkbook := false;
  ExcelSheet := GetWorkSheet(Sh);
  if not Assigned(ExcelSheet) then
    exit;
  if IsPlaningSheet(ExcelSheet) then
    ProtectionAnalisis(ExcelSheet, Target);
end;

function TFMExcelAddIn.SetConnectionStr(const URL, Scheme: WideString): HResult;
var
  Reg: TRegistry;
  DirectPath: string;
  OldUrl, OldScheme: string;
  IsSameConnection: boolean;
  ExlSheet: ExcelWorkSheet;
  ExcelBook: ExcelWorkbook;
  i: integer;
  tmpPlaningSheet: TPlaningSheet;
begin
  result := -1;
  OldUrl := '';
  OldScheme := '';
  if (URL = '') or (Scheme = '') then
    exit;
  Reg := TRegistry.Create;
  DirectPath := RegBasePath + RegConnSection + regConnWebServiceSection;
  //IsSameConnection := false;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(DirectPath, true) then
    begin
      OldUrl := Reg.ReadString(regURLKey);
      OldScheme := Reg.ReadString(regWebServiceSchemeKey);
      IsSameConnection := ((OldUrl + OldScheme) = (URL + Scheme));
      {Сменили подключение}
      if not IsSameConnection then
      begin
        Reg.WriteString(regURLKey, URL);
        Reg.WriteString(regWebServiceSchemeKey, Scheme);
        DataProvider.ClearCache;
        DataProvider.Disconnect;
      end;
      Reg.CloseKey;
    end;
    ExcelBook := Host.ActiveWorkbook;
    if not Assigned(ExcelBook) then
      exit;
    {Перепропишем параметры подключения всем листам в книге. Для этого
      используем временный экземпляр объектной модели. Использование основного
      может вызвать проблемы как в задаче 10304.}
    tmpPlaningSheet := TPLaningSheet.CreateInherited(PlaningSheet);
    tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    for i := 1 to ExcelBook.Sheets.Count do
    begin
      ExlSheet := GetWorkSheet(ExcelBook.Sheets.Item[i]);
      if not IsPlaningSheet(ExlSheet) then
        continue;
      if tmpPlaningSheet.Load(ExlSheet, Context, [lmInner]) then
      try
        tmpPlaningSheet.URL := URL;
        tmpPlaningSheet.SchemeName := Scheme;
        tmpPlaningSheet.Save;
      finally
        tmpPlaningSheet.Clear;
      end;
    end;
    tmpPlaningSheet.SetExternalLinks(nil, nil, nil, nil);
    FreeAndNil(tmpPlaningSheet);
  finally
    Reg.Free;
    //if not IsSameConnection then
      TryConnection(false);
    result := IIF(DataProvider.Connected, 0, -1);
  end;
end;

procedure TFMExcelAddIn.ApplyTaskContext;
var
  i: integer;
  ExlSheet: ExcelWorkSheet;
  TaskID: string;
  tmpPlaningSheet: TPLaningSheet;
  Op: IOperation;
begin
  if Host.Visible[LCID] then
    Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    if Host.Visible[LCID] then
    begin
      Op.Caption := 'Установка контекста задачи...';
      Op.StartOperation(Host.Hwnd);
    end;
    if not VersionOK then
      exit;

    TaskId := '';
    if Assigned(Environment) then
      TaskId := Environment.TaskId;

    {Синхронизируем все листы. Используем для этого временный экземпляр
      объектной модели во избежание 10304}
    tmpPlaningSheet := TPlaningSheet.CreateInherited(PlaningSheet);//TPLaningSheet.Create;
    tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    tmpPlaningSheet.Environment := Environment;
    for i := 1 to Host.ActiveWorkbook.Worksheets.Count do
    begin
      ExlSheet := GetWorkSheet(Host.ActiveWorkbook.Worksheets[i]);
      if not Assigned(ExlSheet) then
        continue;
      if not IsPlaningSheet(ExlSheet) then
        continue;
      tmpPlaningSheet.IsTaskContextLoad := true;
      {синхронизация происходит при загрузке элементов коллекций.}
      if CheckSheetVersion(ExlSheet, DataProvider, false, FProcess) then
        if tmpPlaningSheet.Load(ExlSheet, Context, [lmInner]) then
        begin
          tmpPlaningSheet.Clear;
          tmpPlaningSheet.SpecialFlagForTaskParamCopy := tmpPlaningSheet.TaskId <> TaskId;
          if tmpPlaningSheet.Load(ExlSheet, Context, [lmCollections]) then
          try
            tmpPlaningSheet.TaskId := TaskId;
            tmpPlaningSheet.Save;
          finally
            tmpPlaningSheet.Clear;
          end;
        end;
      tmpPlaningSheet.IsTaskContextLoad := false;
      tmpPlaningSheet.SpecialFlagForTaskParamCopy := false;

      LoadContextFromTask(false);
    end;
    tmpPlaningSheet.SetExternalLinks(nil, nil, nil, nil);
    FreeAndNil(tmpPlaningSheet);

    { Лишняя загрузка нужна для инициализации свойства Online текущего листа}
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if Assigned(ExlSheet) then
    begin
      PlaningSheet.Load(ExlSheet, Context, [lmInner]);
      PlaningSheet.Clear;
    end;

    PlaningSheet.IsTaskConnectionLoad := false;
    SetStatusBarInfo(sbiTaskID, IIF((TaskID = ''), 'нет', TaskID));
  finally
    if Host.Visible[LCID] then
    begin
      Op.StopOperation;
      Op := nil;
      SetActiveWindow(Host.Hwnd);
    end;
  end;
end;

{Отключено после реформы контекста задач (квест 14009). Отказываемся от поддержки
интерфейса IFMPlaningExtension}
procedure TFMExcelAddIn.SetTaskContext(const taskContext: IDispatch); safecall;
begin
  ;
end;

procedure TFMExcelAddIn.MoveElementHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

  function GetElementType(ObjType: string): string;
  begin
    if ObjType = sntColumnDimension then
      result := 'столбцов.'
    else
      if ObjType = sntRowDimension then
        result := 'строк.'
      else
        result := 'фильтров.';
  end;

  function GetComment(Mode: TWizardRunMode; NewObjType, Id: string): string;
  var
    StrTemp: string;
  begin
    try
      StrTemp := GetCommentForHistory(Mode, Id) + ' перемещен в область ' +
        GetElementType(NewObjType);
    except
      StrTemp := 'При получении информации для истории произошла ошибка.';
    end;
    //если предложение начинается с маленькой буквы, приводим её(букву) к верхнему регистру
    StrTemp[1] := AnsiUpperCase(StrTemp[1])[1];
    result := ConvertStringToCommaText(StrTemp);
  end;

var
  Mode: TWizardRunMode;
  ObjType, NewObjType, Id: string;
  ExcelSheet: ExcelWorksheet;
  IsSuccess: boolean;
  CommentForHistory: string;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.Get_HelpFile, Mode, Id, NewObjType);
  case Mode of
    wrmEditColumn: ObjType := sntColumnDimension;
    wrmEditRow: ObjType := sntRowDimension;
    wrmEditFilter: ObjType := sntFilter;
    else exit;
  end;
  IsSuccess := false;
  if TryConnection(false) then
  try
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExcelSheet.Name + '"',
      not CriticalNode, NoteTime, otProcess);
    FUserEvents.Execute(ExcelSheet, enBeforeRefresh);

    try
      FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
      FXMLCatalog.SetUp(DataProvider);
      if not PlaningSheet.Load(ExcelSheet, Context, lmAll) then
        exit;
    finally
      //чтобы эксель не терял фокус
      Application.ProcessMessages;
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    CommentForHistory := GetComment(Mode, NewObjType, Id);

    if PlaningSheet.MoveElement(ObjType, NewObjType, Id) then
    begin
      if NewObjType = sntFilter then
        PlaningSheet.Filters.Refresh(true);
      PlaningSheet.SetUpMeasuresPosition;
      IsSuccess := DoRebuildSheet(ExcelSheet);

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    end;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
    CalculateSheet(ExcelSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExcelSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end
  else
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
end;

procedure TFMExcelAddIn.InsertNewLineHandler(Button: CommandBarButton; var CancelDefault: WordBool);
begin
  AppendEmptyRowToSheet(GetWorkSheet(Host.ActiveSheet), nil, PlaningSheet);
end;

procedure TFMExcelAddIn.DeleteElementHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

  function GetComment(Mode: TWizardRunMode; Id: string): string;
  begin
    try
      result := 'Удален ' + GetCommentForHistory(Mode, Id) + '.';
      result := ConvertStringToCommaText(result);
    except
      result := 'При получения информации для истории произошла ошибка.';
    end;
  end;

var
  Mode: TWizardRunMode;
  Id, Question, tmpStr, CellAlias: string;
  Index: integer;
  IsSuccess: boolean;
  CommentForHistory: string;
  NeedRebuild: boolean;
  ESheet: ExcelWorkSheet;
  Cell: TSheetSingleCellInterface;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Question);
  NeedRebuild := false;
  FXMLCatalog.SetUp(DataProvider);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    with PlaningSheet do
    begin
      if Empty then
        exit;
      CommentForHistory := GetComment(Mode, ID);

      case Mode of
        wrmEditColumn:
          begin
            Index := Columns.FindByID(Id);
            if Index < 0 then
              exit;
            if Columns[Index].MayBeDeleted then
            begin
              Columns.Delete(Index);
              SetUpMeasuresPosition;
              NeedRebuild := true;
            end;
          end;
        wrmEditRow:
          begin
            Index := Rows.FindByID(Id);
            if Index < 0 then
              exit;
            if Rows[Index].MayBeDeleted then
            begin
              Rows.Delete(Index);
              SetUpMeasuresPosition;
              NeedRebuild := true;
            end;
          end;
        wrmEditFilter:
          begin
            Index := Filters.FindByID(Id);
            if Index < 0 then
              exit;
            if Filters[Index].MayBeDeleted then
            begin
              Filters.Delete(Index);
              NeedRebuild := true;
            end;
          end;
        wrmEditTotal:
          begin
            Index := Totals.FindById(Id);
            if Index < 0 then
              exit;
            if Totals[Index].MayBeDeleted then
            begin
              Totals.Delete(Index);
              NeedRebuild := true;
            end;
          end;
        wrmNone: {!! Весьма условная условность: режим "никакой" - значит
          речь идет об отдельных ячейках}
          begin
            Index := SingleCells.FindById(Id);
            if Index < 0 then
              exit;
            if SingleCells[Index].MayBeDeleted then
            begin
              IsSuccess := false;
              try
                Cell := SingleCells[Index];
                CellAlias := Cell.Alias;
                Host.ScreenUpdating[LCID] := false;
                {Запускаем индикатор}
                PlaningSheet.StartOperation(Host.Hwnd, pfoSingleResultDeletion);
                {очистить значения типовых формул ссылающихся на указанный алиас показателя}

                SetSheetProtection(ESheet, false);


                //Cell.ClearLinkedTypeFormulasValues;
                {разместить значения типовых формул ссылающихся на указанный алиас показателя}
                //Cell.MapLinkedTypeFormulasValues;
                Cell.Suicide(smImmediate, tmpStr);
                tmpStr := ConvertStringToCommaText(tmpStr);
                AddEventInSheetHistory(evtEdit, tmpStr, true);
                FProcess.DirectWriteLogString(tmpStr);
                NeedRebuild := false;
                IsSuccess := true;
              finally
                if IsSuccess then
                begin
                  PlaningSheet.Save;
                  SingleCellCanBePlacedHere := true;
                  EnableSingleCellAppend;
                end;
                PlaningSheet.StopOperation;
                Host.ScreenUpdating[LCID] := true;
              end;
            end;
          end;
      end;
    end;
    Application.ProcessMessages;
    IsSuccess := false;
    if NeedRebuild then
    try
      CollectDeferredTotalsData;
      //PlaningSheet.Backup;
      //PlaningSheet.Save;
      //PlaningSheet.Clear;
      FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
      FProcess.NewProcessClear := false;
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ESheet.Name + '"',
        not CriticalNode, NoteTime, otProcess);
      FUserEvents.Execute(ESheet, enBeforeRefresh);
      //FXMLCatalog.SetUp(DataProvider);
//      if not PlaningSheet.Load(ESheet, Context, lmAll) then
  //      exit;
      IsSuccess := DoRebuildSheet(ESheet);
     (* if not IsSuccess then
      begin
        PlaningSheet.Restore;
        PlaningSheet.Save;
      end;*)

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    finally
      AfterRebuild(ESheet, IsSuccess);
      CalculateSheet(ESheet);
      if IsSuccess then
      begin
        FUserEvents.Execute(ESheet, enAfterRefresh);
        FProcess.CloseOperation; //pfoRebuild
      end;
      FProcess.CloseProcess;
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.MarkEmptyHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  Selection, CurSelection, CellRange: ExcelRange;
  ExlSheet: ExcelWorkSheet;
  i, j, k: integer;
  Address: string;
  Style: variant;
  NumberFormat: variant;
  FirstRow, LastRow: integer;
  Columns: TStringList;

  // получить координаты показателей
  // первую и последнюю строку, столбцы
  function GetTotalRanges(ExlSheet: ExcelWorkSheet; Totals: TSheetTotalCollectionInterface;
                          var FirstRow, LastRow: integer): TStringList;
  var
    Range: ExcelRange;
    i: integer;
    SectionIndex: integer;
    TotalRange: ExcelRange;
  begin
    result := TStringList.Create;
    // получаем первую и последнюю строки
    Range := GetRangeByName(ExlSheet, snNamePrefix + snSeparator + sntTotals);
    if not Assigned(Range) then
      exit;
    FirstRow := Range.Row;
    LastRow := Range.Row + Range.Rows.Count - 1;
    // получаем столбцы показателей
    for i := 0 to Totals.Count - 1 do
    begin
      // пропускаем не результаты
      if (Totals[i].TotalType <> wtResult) then
        continue;
      if not Totals[i].MayBeEdited then
        continue;
      SectionIndex := 0;
      TotalRange := Totals[i].GetTotalRange(SectionIndex);
      while (TotalRange <> nil) do
      begin
        result.Add(IntToStr(TotalRange.Column));
        // переходим в следующую секцию
        Inc(SectionIndex);
        TotalRange := Totals[i].GetTotalRange(SectionIndex);
      end;
    end;
  end;

  function IsSingleCell(SingleCells: TSheetSingleCellCollectionInterface; Address: string): boolean;
  var
    i: integer;
  begin
    result := false;
    for i := 0 to SingleCells.Count - 1 do
    begin
      if not (SingleCells[i].TotalType = wtResult) then
        continue;
      if not SingleCells[i].MayBeEdited then
        continue;
      if SingleCells[i].Address = Address then
      begin
        result := true;
        exit;
      end;
    end;
  end;

begin
  // если это не лист планирования - выходим
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not IsPlaningSheet(ExlSheet) then
    exit;
  DisableCopyMode;
  // грузим метаданные
  if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
  try
    // создаем стили...
    // чтобы работало сразу без обновления на старых листах
    if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.ValidateStyles;
    // получаем границы диапазонов показателей
    Columns := GetTotalRanges(ExlSheet, PlaningSheet.Totals, FirstRow, LastRow);
    try
      // вырубаем прорисовку
      ExlSheet.Application.ScreenUpdating[LCID] := false;
      ExlSheet.Application.Set_Interactive(LCID, false);
      // получаем выделенные ячейки
      Selection := (ExlSheet.Application.Selection[LCID] as ExcelRange);
      for k := 1 to Selection.Areas.Count do
      begin
        CurSelection := Selection.Areas[k];
        for i := CurSelection.Row to CurSelection.Row + CurSelection.Rows.Count - 1 do
          for j := CurSelection.Column to CurSelection.Column + CurSelection.Columns.Count - 1 do
          begin
            CellRange := ExlSheet.Range[ExlSheet.Cells.Item[i, j], ExlSheet.Cells.Item[i, j]];
            Address := CellRange.AddressLocal[false, false, xlA1, false, varNull];
            NumberFormat := ExlSheet.Cells.Item[i, j].NumberFormat;
            Style := ExlSheet.Cells.Item[i, j].Style;
            if IsSingleCell(PlaningSheet.SingleCells, Address) then
            begin
              if (string(Style) = snDataFreeErased) then
                ExlSheet.Cells.Item[i, j].Style := snResultSingleCells
              else
                if (string(Style) = snResultSingleCells) then
                  ExlSheet.Cells.Item[i, j].Style := snDataFreeErased;
            end
            else
            begin
              // пропускаем ячейки вне диапазонов показателей
              if (i < FirstRow) or (i > LastRow) or
                 (Columns.IndexOf(IntToStr(j)) = -1) then
                continue;
              // инвертируем стиль у показателей результат
              // "очищенная" ячейка / обычная
              if (string(Style) = snDataFreeErased) then
                ExlSheet.Cells.Item[i, j].Style := snDataFree
              else
                if (string(Style) = snDataFree) then
                  ExlSheet.Cells.Item[i, j].Style := snDataFreeErased;
            end;
            ExlSheet.Cells.Item[i, j].NumberFormat := NumberFormat;
          end;
      end;
    finally
      FreeAndNil(Columns);
      ExlSheet.Application.ScreenUpdating[LCID] := true;
      ExlSheet.Application.Set_Interactive(LCID, true);
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.SheetHistoryHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  if not Assigned(FfrmSheetHistory)then
    FfrmSheetHistory := TSheetHistory.Create(nil);
  DisableCopyMode;
  try
    FfrmSheetHistory.ShowSheetHistory(Host);
  finally
    SetActiveWindow(Host.Hwnd);
  end;
end;

procedure TFMExcelAddIn.EditStyleHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  BarButton: CommandBarButton;
  ExcelSheet: ExcelWorksheet;
begin
  try
    DisableCopyMode;
    if not Assigned(Host) then
      exit;
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    SetBookProtection(Host.ActiveWorkbook, false);
    //ищем кнопку "Стиль" в меню Excel-я
    BarButton := (Host.CommandBars.FindControl(EmptyParam, 254, EmptyParam,
      EmptyParam) as CommandBarButton);
    //нажимаем её
    BarButton.Execute;
    //если надо устанавливаем защиту
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  except
  end;
end;

procedure TFMExcelAddIn.WorkbookBeforeSave(const Wb: ExcelWorkbook; SaveAsUI: WordBool; var Cancel: WordBool);
var
  TaskId: string;
  ContextAssigned: boolean;
begin
  DisableCopyMode;
  ProtectPlaningSheets(Wb, true);
  // если книга - от задач, но контекста задач нет, выдаем предупреждение
  if PlaningSheet.IsSilentMode then
    exit;
  TaskId := Environment.TaskId;
  ContextAssigned := AssignedContext or Assigned(Context);
  if SaveAsUI and (TaskId <> '') and ContextAssigned then
    Cancel := not ShowWarningEx(wrnTaskSave);
end;

function TFMExcelAddin.EditableTotalSelected(ExcelSheet: ExcelWorksheet;
  Target: ExcelRange): boolean;
var
  Editable: boolean;
begin
  result := FWritablesInfo.CheckForWritableRange(ExcelSheet, Target, Editable);
  { Если лист находится в режиме "работы с данными", то для снятия защиты
    мало того факта, что показатель нужного типа, необходимо еще разрешение
    на его редактирование в таком режиме.}
  if not PlaningSheet.InConstructionMode then
    result := result and Editable;
end;

procedure TFMExcelAddIn.ProtectionAnalisis(const ExcelSheet: ExcelWorksheet;
  const Target: ExcelRange);
var
  tmpStr: string;
  IsWritableCell, Editable: boolean;
begin
  if not Assigned(ExcelSheet) then
    exit;

  {обработка для отдельных ячеек}
  if FWritablesInfo.IsSingleCellSelected(ExcelSheet, Target,
    IsWritableCell, Editable) then
  begin
    SingleCellCanBePlacedHere := false;
    EnableSingleCellAppend;
    if PlaningSheet.InConstructionMode then
      SetSheetProtection(ExcelSheet, not IsWritableCell)
    else
      SetSheetProtection(ExcelSheet, not (IsWritableCell and Editable));
    exit;
  end;

  SingleCellCanBePlacedHere := IsSingleCellAllowed(ExcelSheet, Target,
    FWritablesInfo, tmpStr, Editable);
  if not PlaningSheet.InConstructionMode then
    SingleCellCanBePlacedHere := SingleCellCanBePlacedHere and Editable;
  if not IsPlaningSheet(ExcelSheet) then
  begin
    EnableSingleCellAppend;
    exit;
  end;
  if TableSelected(ExcelSheet, Target) then
  begin
    if (BreakPointSelected(ExcelSheet, Target) or EditableTotalSelected(ExcelSheet, Target)) then
      SetSheetProtection(ExcelSheet, false)
    else
      SetSheetProtection(ExcelSheet, true);
    EnableSingleCellAppend;
  end
  else
    if BreakPointSelected(ExcelSheet, Target) then
    begin
      SetSheetProtection(ExcelSheet, false);
      EnableSingleCellAppend;
    end
    else
      if IntersectTableAndRange(ExcelSheet, Target) then
      begin
        SetSheetProtection(ExcelSheet, true);
        SingleCellCanBePlacedHere := false;
        EnableSingleCellAppend;
      end
      else
      begin
        SetSheetProtection(ExcelSheet, false);
        if IsNestedRanges(Target, GetExtendedTableRange(ExcelSheet)) then
          SingleCellCanBePlacedHere := false;
        EnableSingleCellAppend
      end;
end;

procedure TFMExcelAddIn.DisableCopyMode;
begin
  {отключаем режим копирования в Excel-е}
  Host.CutCopyMode[LCID] := Integer(false);
end;

procedure TFMExcelAddIn.ShowParamsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Sheet: ExcelWorkSheet;

  // обновляем комментарии элементов листа 
  procedure ModifyComments(Sheet: ExcelWorkSheet);
  var
    i, j: integer;
    Range: ExcelRange;

    procedure CommentAxis(AxisCollection: TSheetAxisCollectionInterface);
    var
      i, j: integer;
      Range: ExcelRange;
    begin
      if AxisCollection.Broken then
      begin
        Range := GetRangeByName(Sheet, snNamePrefix + snSeparator + sntRowTitles);
        if Assigned(Range) then
          CommentCell(Sheet, Range.Row, Range.Column, AxisCollection.CommentText);
      end
      else
        for i := 0 to AxisCollection.Count - 1 do
          for j := 0 to AxisCollection[i].Levels.Count - 1 do
          begin
            Range := GetRangeByName(Sheet, AxisCollection[i].Levels[j].TitleExcelName);
            if Assigned(Range) then
              CommentCell(Sheet, Range.Row, Range.Column, AxisCollection[i].CommentText);
          end;
    end;

  begin
    CommentAxis(PlaningSheet.Rows);
    CommentAxis(PlaningSheet.Columns);
    for i := 0 to PlaningSheet.Filters.Count - 1 do
    begin
      if PlaningSheet.Filters[i].IsPartial then
        continue;
      Range := GetRangeByName(Sheet, PlaningSheet.Filters[i].ExcelName);
      if Assigned(Range) then
        CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.Filters[i].CommentText);
    end;
    for i := 0 to PlaningSheet.Totals.Count - 1 do
      for j := 0 to PlaningSheet.Totals[i].SectionCount - 1 do
      begin
        Range := GetRangeByName(Sheet, PlaningSheet.Totals[i].TitleExcelName +
                                snSeparator + IntToStr(j));
        if Assigned(Range) then
          CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.Totals[i].CommentText);
      end;
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      Range := GetRangeByName(Sheet, PlaningSheet.SingleCells[i].ExcelName);
      if Assigned(Range) then
        CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.SingleCells[i].CommentText);
    end;
  end;

begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
    exit;
  end;
  {Обновление контекста перед загрузкой метаданных, для синхронизации из задачи в лист}
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    if EditParams(PlaningSheet) then
    begin
      PlaningSheet.Save;
      {Обновление контекста после сохранения метаданных, для синхронизации из листа в задачу}
      LoadContextFromTask(true);
      ApplyTaskContext;
      SetSheetProtection(Sheet, false);
      ModifyComments(Sheet);
    end;
  finally
    //FreeAndNil(frmParamControl);
    PlaningSheet.Clear;
    ProtectionAnalisis(Sheet, Host.ActiveCell);
    SetActiveWindow(Host.Hwnd);
  end;
end;

procedure TFMExcelAddIn.ShowConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  frmConstControl: TfrmConstControl;
  Sheet: ExcelWorkSheet;
  EventType: TSheetEventType;
  TmpConst: TConstInterface;
  NewName, OldName: string;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
  begin
    PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
    exit;
  end;
  frmConstControl := TfrmConstControl.Create(nil);
  frmConstControl.ISConstChoiseMode := false;
  {Обновление контекста перед загрузкой метаданных, для синхронизации из задачи в лист}
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.ValidateStyles;
    frmConstControl.SheetInterface := PlaningSheet;
    frmConstControl.ShowModal;
    if (frmConstControl.ModalResult = mrOK) then
    begin
      EventType := evtConstsEdit;
      if (frmConstControl.HistoryList.Count <> 0) then
        PlaningSheet.AddEventInSheetHistory(EventType, frmConstControl.HistoryList.CommaText, true);

      {если какие-то константы были переименованы следует обеспечить синхронные изменения в других листах}
      if not PlaningSheet.Consts.Empty then
      begin
        TmpConst := PlaningSheet.Consts[0];
        while frmConstControl.RenamingList.Count > 0 do
        begin
          OldName := frmConstControl.RenamingList.Names[0];
          NewName := frmConstControl.RenamingList.Values[OldName];
          if Assigned(TmpConst) then
            TmpConst.SyncSheetConsts(OldName, NewName);
          frmConstControl.RenamingList.Delete(0);
        end;
      end;

      if frmConstControl.NeedUpdate then
      begin
        if not SetSheetProtection(Sheet, false) then
        begin
          PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
          exit;
        end;
        PlaningSheet.MapConsts;
      end;
      PlaningSheet.Save;
      {Обновление контекста после сохранения метаданных, для синхронизации из листа в задачу}
      LoadContextFromTask(false);
    end;
  finally
    MayHook := true;
    FreeAndNil(frmConstControl);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.AddConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  frmConstControl: TfrmConstControl;
  Sheet: ExcelWorkSheet;
  Cell: TSheetSingleCellInterface;
  EventType: TSheetEventType;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
  begin
    PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
    exit;
  end;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.ValidateStyles;
    frmConstControl := TfrmConstControl.Create(nil);
    frmConstControl.ISConstChoiseMode := true;

    frmConstControl.SheetInterface := PlaningSheet;
    frmConstControl.ShowModal;
    if not (frmConstControl.ModalResult = mrOK) then
      exit;
    Cell := PlaningSheet.SingleCells.Append;
    Cell.TotalType := wtConst;
    Cell.Name := frmConstControl.Constant.Name;
    Cell.Caption := frmConstControl.Constant.Name;
    PlaningSheet.Consts.ConstByName(Cell.Name).IsSheetConst := true;
    MayHook := false;
    MarkObject(Sheet, Sheet.Application.ActiveCell, Cell.ExcelName, false);
    frmConstControl.HistoryList.Add(Format('%s отдельная ячейка - константа "%s" , по адресу %s',
      ['Добавлена', Cell.Name, Cell.Address]));
    EventType := evtEdit;
    if (frmConstControl.HistoryList.Count <> 0) then
      PlaningSheet.AddEventInSheetHistory(EventType, frmConstControl.HistoryList.CommaText, true);
    if not SetSheetProtection(Sheet, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.MapConsts;
    PlaningSheet.Save;
    LoadContextFromTask(false);
  finally
    MayHook := true;
    FreeAndNil(frmConstControl);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.EditConstHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Mode: TWizardRunMode;
  Id, Query, EditingConstName, tmpStr: string;
  Cell: TSheetSingleCellInterface;
  Sheet: ExcelWorkSheet;
  Index: integer;
  Constant: TConstInterface;
  frmConstProperties: TfrmConstProperties;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    frmConstProperties := TfrmConstProperties.Create(nil);
    Index := PlaningSheet.SingleCells.FindById(Id);
    Cell := PlaningSheet.SingleCells[Index];
    Constant := PlaningSheet.Consts.ConstByName(Cell.Name);
    {Аварийная ситуация - поврежден документ с описанием констант? }
    if not Assigned(Constant) then
      if ShowQuestion('Ошибка редактирования отдельного показателя.' +
        #13#10'Информация о константе отсутствует в листе.' +
        #13#10'Добавить константу в лист (в случае отрицательного ответа показатель будет удален)?') then
      begin
        Constant := Cell.RepairConst;
      end
      else
      begin
        Cell.Suicide(smImmediate, tmpStr);
        PlaningSheet.Save;
        if tmpStr <> '' then
          PlaningSheet.AddEventInSheetHistory(evtConstsEdit, tmpStr, false);
        exit;
      end;
    EditingConstName := Constant.Name;
    frmConstProperties.Constant := Constant;
    frmConstProperties.Consts := PlaningSheet.Consts;
    frmConstProperties.TaskContext := PlaningSheet.TaskContext;
    frmConstProperties.ShowModal;
    if not frmConstProperties.Apply then
      exit;
    Constant.SyncSheetConsts(EditingConstName, Constant.Name);
    MayHook := false;
    if not SetSheetProtection(Sheet, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.MapConsts;
    PlaningSheet.Save;
    LoadContextFromTask(false);
  finally
    MayHook := true;
    FreeAndNil(frmConstProperties);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.RefreshConstHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Id, Query, MsgText: string;
  tmpStringList: TStringList;
  Mode: TWizardRunMode;
  Sheet: ExcelWorkSheet;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    SetSheetProtection(Sheet, false); //временно снимаем защиту
    tmpStringList := TStringList.Create;
    tmpStringList.Add(IntToStr(PlaningSheet.SingleCells.FindById(Id)));
    PlaningSheet.MapSingleCells(tmpStringList, MsgText, true, nil, nil);
    PlaningSheet.Save;
  finally
    SetSheetProtection(Sheet, true); //защиту на место
    FreeStringList(tmpStringList);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.EnableSingleCellAppend;
var
  State: boolean;
begin
  State := SingleCellCanBePlacedHere;
  if not PlaningSheet.InConstructionMode then
    State := State and PlaningSheet.SingleCells.MayBeEdited;
  UpdateAppendSubmenuItem(true, tagToolButtonSingleCell, State);
  UpdateAppendSubmenuItem(true, tagToolButtonConst, State);
  UpdateAppendSubmenuItem(false, tagMenuButtonSingleCell, State);
  UpdateAppendSubmenuItem(false, tagMenuButtonConst, State);
end;

procedure TFMExcelAddIn.SingleCellEditorHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  CellIndex, ErrCode: integer;
  Cell: TSheetSingleCellInterface;
  Success, IsAdding, IsRefresh: boolean;
  ExcelSheet: ExcelWorkSheet;
  Mode: TWizardRunMode;
  Id, Param2, MsgText: string;
  HList, AddedCells, tmpStringList: TStringList;

  procedure RefreshCell(Cell: TSheetSingleCellInterface);
  var
    Operation: IOperation;
  begin
    Operation := CreateComObject(CLASS_Operation) as IOperation;
    try
      Operation.StartOperation(Host.Hwnd);
      Operation.Caption := pfoFiltersRefresh;
      Cell.Refresh(false);
    finally
      Application.ProcessMessages;
      Operation.StopOperation;
      SetActiveWindow(Host.Hwnd);
      Operation := nil;
    end;
  end;

begin
  Success := false;
  IsRefresh := false;
  if PrepareRebuild(lmNoFreeData, ExcelSheet) then
  try
    PlaningSheet.CollectTotalsData(true);
    ParseAdditionalInfo(Button.HelpFile, Mode, Id, Param2);
    CellIndex := PlaningSheet.SingleCells.FindById(Id);
    IsAdding := CellIndex = -1;
    IsRefresh := Param2 = 'refresh';
    HList := TStringList.Create;
    tmpStringList := TStringList.Create;
    MsgText := '';

    if IsRefresh then
      Success := true
    else
      Success := EditSingleCell(PlaningSheet, CellIndex, Host.Application.Hwnd);
    if Success then
    begin
      Success := false;
      //снимаем защиту на время построения
      if not SetSheetProtection(ExcelSheet, false) then
      begin
        MsgText := ermWorkbookProtectionFault;
        HList.Add(MsgText);
        exit;
      end;
      ExcelSheet.Application.ScreenUpdating[LCID] := false;
      ExcelSheet.Application.Set_Interactive(LCID, false);
      if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
      begin
        MsgText := ermWorkbookProtectionFault;
        HList.Add(MsgText);
        exit;
      end;
      PlaningSheet.ValidateStyles;
      Cell := PlaningSheet.SingleCells[CellIndex];

      // нужно обновить фильтры
      RefreshCell(Cell);
      if not Cell.Validate(MsgText, ErrCode) then
      begin
        HList.Add(Format('Отдельный показатель "%s" (куб: "%s", мера: "%s"), по адресу %s: ',
          [Cell.Name, Cell.CubeName, Cell.MeasureName, Cell.Address]) + MsgText);
        exit;
      end;
      MayHook := false;

      if IsAdding then
      begin
        MarkObject(ExcelSheet, ExcelSheet.Application.ActiveCell, Cell.ExcelName,
          false);
        AddedCells := TStringList.Create;
        AddedCells.Add(Cell.ExcelName);
      end;

      tmpStringList.Add(IntToStr(CellIndex));
      Success := PlaningSheet.MapSingleCells(tmpStringList, MsgText, true, nil, nil);
      HList.Add(Format('%s отдельный показатель "%s" (куб: "%s", мера: "%s"), по адресу %s',
        [IIF(IsRefresh, 'Обновлен', IIF(IsAdding, 'Добавлен', 'Изменен')),
        Cell.Name, Cell.CubeName, Cell.MeasureName, Cell.Address]));
      if Success then
      begin
        PlaningSheet.Save;
        if IsAdding then
        begin
          SingleCellCanBePlacedHere := false;
          EnableSingleCellAppend;
        end;
      end
      else
        if IsAdding then
        begin
          ClearAddedCellsNames(ExcelSheet, AddedCells);
          FreeStringList(AddedCells);
        end;
    end;
  finally
    if HList.Count > 0 then
      if IsRefresh then
        PlaningSheet.AddEventInSheetHistory(evtRefresh, HList.CommaText, Success)
      else
        PlaningSheet.AddEventInSheetHistory(evtEdit, HList.CommaText, Success);
    FProcess.DirectWriteLogString(CommaTextToString(HList.CommaText));
    MayHook := true;
    FreeStringList(HList);
    FreeStringList(tmpStringList);
    AfterRebuild(ExcelSheet, Success);
  end;
end;

procedure TFMExcelAddIn.SingleCellsManagerHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  RefreshList, HList, AddedCells: TStringList;
  MsgText: string;
begin
  if PrepareRebuild(lmNoFreeData, ExcelSheet) then
  try
    PlaningSheet.CollectTotalsData(true);
    {если получили результаты работы менеджера отдельных ячеек, то мапим их}
    HList := TStringList.Create;
    RefreshList := TStringList.Create;
    AddedCells := TStringList.Create;
    if ManageSingleCells(PlaningSheet, HList, AddedCells, RefreshList) then
    begin
      if (RefreshList.Count > 0) then
      try
        Application.ProcessMessages;
        FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
        FProcess.OpenOperation(pfoSingleCellsRefresh, not CriticalNode, NoteTime, otProcess);
        if not PlaningSheet.SingleCells.Validate then
        begin
          PlaningSheet.PostMessage(ermValidationFault, msgError);
          exit;
        end;

        //снимаем защиту на время построения
        if not SetSheetProtection(ExcelSheet, false) then
        begin
          FProcess.PostError(ermWorksheetProtectionFault);
          exit;
        end;
        ExcelSheet.Application.ScreenUpdating[LCID] := false;
        ExcelSheet.Application.Set_Interactive(LCID, false);
        if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
        begin
          FProcess.PostError(ermWorkbookProtectionFault);
          exit;
        end;
        PlaningSheet.ValidateStyles;
        if not PlaningSheet.MapSingleCells(RefreshList, MsgText, true, nil, nil) then
        begin
          ClearAddedCellsNames(ExcelSheet, AddedCells);
          exit;
        end;
        FProcess.CloseOperation;
      finally
        FProcess.CloseProcess;
      end;
      PlaningSheet.Save;
      PlaningSheet.AddEventInSheetHistory(evtEdit, HList.CommaText, true);
    end;
  finally
    FreeStringList(HList);
    FreeStringList(AddedCells);
    FreeStringList(RefreshList);
    AfterRebuild(ExcelSheet, true);
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  end;
end;

function TFMExcelAddIn.GetContext: TTaskContext;
begin
  result := FTaskContextProvider.TaskContext[Host.ActiveWorkbook];
end;


function TFMExcelAddIn.GetActiveWorkBookName: string;
var
  ActiveBook: ExcelWorkBook;
begin
  result := '';
  ActiveBook := Host.ActiveWorkbook;
  if Assigned(ActiveBook) then
    result := ActiveBook.FullName[LCID];
end;

function TFMExcelAddIn.Get_IsSilentMode: WordBool;
begin
  result := PlaningSheet.IsSilentMode;
end;

procedure TFMExcelAddIn.Set_IsSilentMode(Value: WordBool);
begin
  PlaningSheet.IsSilentMode := Value;
end;

function TFMExcelAddIn.Get_ProcessForm: IProcessForm;
begin
  if Assigned(FProcess) then
    result := FProcess;
end;

procedure TFMExcelAddIn.Set_ProcessForm(const Value: IProcessForm);
begin
  FProcess := nil;
  FProcess := Value;
  PlaningSheet.Process := FProcess;
  FUserEvents.SetExternalLinks(FProcess);
  UpdateProcessLogging;
end;
                                            
procedure TFMExcelAddIn.OnTaskConnection(IsConnected: WordBool);
begin
  PlaningSheet.OnTaskConnection(IsConnected);
  PlaningSheet.IsTaskConnectionLoad := true;
end;

function TFMExcelAddIn.Get_IsLoadingFromTask: WordBool;
begin
  result := Environment.IsLoadingFromTask;
end;

procedure TFMExcelAddIn.Set_IsLoadingFromTask(Value: WordBool);
begin
  PlaningSheet.IsLoadingFromTask := Value;
end;

procedure TFMExcelAddIn.AncillaryInit;
begin
  if (not(Assigned(Host) and IsExcelVisible(Host)))then
    exit;
  try
    Host.ScreenUpdating[LCID] := false;
  finally
    Application.ProcessMessages;
  end;
end;

function TFMExcelAddIn.QueryRange(const ACaption: WideString;
  const APrompt: WideString; AllowMultiArea: WordBool): IDispatch;
var
  Visibility: boolean;
  v: Variant;
begin
  result := nil;
  Visibility := Host.Visible[LCID];
  if not Visibility then
    Host.Set_Visible(LCID, true);
  try
    while true do
    begin
      v := Host.InputBox(APrompt, ACaption, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, 8, LCID);
      Application.ProcessMessages;
      if not Assigned(TVarData(v).VDispatch) then
        exit;
      result := IDispatch(TVarData(v).VDispatch);
      if AllowMultiArea or (not AllowMultiArea and ((result as ExcelRange).Areas.Count = 1)) then
        exit;
      ShowError('Выбранный диапазон содержит несколько несвязанных областей.');
    end;
  finally
    if not Visibility then
      Host.Set_Visible(LCID, false);
  end;
end;

procedure TFMExcelAddIn.RecognizeExcelVersion;
begin
  FExcelVersion := evUnknown;
  if Assigned(Host) then
  begin
    if (Pos('11.', Host.Version[LCID]) > 0) then
      FExcelVersion := ev2003
    else
      if (Pos('10.', Host.Version[LCID]) > 0) then
        FExcelVersion := evXP;
  end;
end;

procedure TFMExcelAddIn.ComponentEditorHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExlSheet: ExcelWorkSheet;
  HistoryString, ID, DummyStr: string;
  StylesToDefault, IsSuccess, NeedSwitchOffline, NeedClearTaskInfo: boolean;
  WizardRunMode: TWizardRunMode;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData, ExlSheet) then
  try
    HistoryString := '';
    ParseAdditionalInfo(Button.Get_HelpFile, WizardRunMode, ID, DummyStr);
    if EditSheetComponents(PlaningSheet, FUserEvents, HistoryString,
      StylesToDefault, NeedSwitchOffline, NeedClearTaskInfo, Id) then
    begin
      Application.ProcessMessages;
      FUserEvents.SaveEvents(ExlSheet);
      if NeedClearTaskInfo then
      begin
        PlaningSheet.OnTaskConnection(false);
        UpdateStatusBar(ExlSheet);
      end;
      if StylesToDefault then
      begin
        Host.ScreenUpdating[GetUserDefaultLCID] := false;
        if not InitWorkbookStyles((ExlSheet.Parent as ExcelWorkbook), true) then
          PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      end;

      AncillaryInit;
      if PlaningSheet.Online then
      begin
        FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh,mErrorRefresh, false);
        FProcess.NewProcessClear := false;
        FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"',
          not CriticalNode, NoteTime, otProcess);

        CollectDeferredTotalsData;

        PlaningSheet.NeedMapSingleCells := false;
        if NeedSwitchOffline then
          PlaningSheet.PrepareToSwitchOffline;
        IsSuccess := DoRebuildSheet(ExlSheet);
      end;
      PlaningSheet.AddEventInSheetHistory(evtEdit,
        MergeCommaText(HistoryString, GetErrorList), IsSuccess);
      if NeedSwitchOffline then
      begin
        PlaningSheet.SwitchOffline;
        PlaningSheet.Save;
      end;
    end;
  finally
    AfterRebuild(ExlSheet, IsSuccess);
    CalculateSheet(ExlSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end;
end;

{ IFMPlanningVBProgramming }

function TFMExcelAddIn.VBGetPropertyByName(const PropertyName: WideString): WideString;
begin
  result := GetPropValueByName(PropertyName);
end;

procedure TFMExcelAddIn.VBSetPropertyByName(const PropertyName, PropertyValue: WideString);
begin
  SetPropValueByName(PropertyName, PropertyValue);
end;

function TFMExcelAddIn.VBGetConstValueByName(const ConstName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  Constant: TConstInterface;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Constant := PlaningSheet.Consts.ConstByName(ConstName);
    if not Assigned(Constant) then
      exit;
    result := Constant.Value;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBSetConstValueByName(const ConstName,
  ConstValue: WideString): WordBool;
var
  Sheet: ExcelWorkSheet;
  Constant: TConstInterface;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Constant := PlaningSheet.Consts.ConstByName(ConstName);
    if not Assigned(Constant) then
      exit;
    Constant.Value := ConstValue;
    PlaningSheet.Save;
    result := true;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBRefresh: WordBool;
begin
  result := RefreshActiveSheet;
end;

function TFMExcelAddIn.VBWriteback: WordBool;
begin
  result := WritebackActiveSheet;
end;

function TFMExcelAddIn.VBGetCurrentConnection(var URL, SchemeName: WideString): WordBool;

  function GetSheetConnection(var URL, SchemeName: WideString): boolean;
  var
    Sheet: ExcelWorkSheet;
  begin
    result := false;
    Sheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(Sheet) then
      exit;
    if PlaningSheet.Load(Sheet, Context, [lmInner]) then
    try
      URL := PlaningSheet.URL;
      SchemeName := PlaningSheet.SchemeName;
      result := (URL <> '') and (SchemeName <> '');
    finally
      PlaningSheet.Clear;
    end;
  end;

  function GetRegistryConnection(var URL, SchemeName: WideString): boolean;
  var
    Reg: TRegistry;
  begin
    result := false;
    Reg := TRegistry.Create;
    try
      Reg.RootKey := HKEY_CURRENT_USER;
      if not Reg.OpenKey(RegBasePath + RegConnSection + regConnWebServiceSection, true) then
        exit;
      URL := Reg.ReadString(regURLKey);
      SchemeName := Reg.ReadString(regWebServiceSchemeKey);
      result := (URL <> '') and (SchemeName <> '');
      Reg.CloseKey;
    finally
      Reg.Free;
    end;
  end;

begin
  result := false;
  if not GetSheetConnection(URL, SchemeName) then
    if not GetRegistryConnection(URL, SchemeName) then
      exit;
  result := true;    
end;

function TFMExcelAddIn.VBGetMembers(const DimensionName: WideString): OleVariant;
var
  Sheet: ExcelWorkSheet;
begin
  result := Null;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    result := PlaningSheet.GetMembersArray(DimensionName);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant);
var
  Sheet: ExcelWorkSheet;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.SetMembersByArray(DimensionName, UniqueNames);
    PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetParamMembers(const ParamName: WideString): OleVariant;
var
  Sheet: ExcelWorkSheet;
  Param: TParamInterface;
begin
  result := Null;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Param := PlaningSheet.Params.ParamByName(ParamName);
    if not Assigned(Param) then
      exit;
    result := PlaningSheet.GetMembersArray(Param.Dimension);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant);
var
  Sheet: ExcelWorkSheet;
  Param: TParamInterface;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Param := PlaningSheet.Params.ParamByName(ParamName);
    if not Assigned(Param) then
      exit;
    PlaningSheet.SetMembersByArray(Param.Dimension, UniqueNames);
    PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBEditMembers(const DimensionName: WideString): WordBool;
var
  Sheet: ExcelWorkSheet;
  Dimension: TSheetDimension;
  WizardRunMode: TWizardRunMode;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    if not Assigned(FfrmConstructorWizard) then
    begin
      FfrmConstructorWizard := TfrmConstructorWizard.CreateEx(nil, PlaningSheet);
      FfrmConstructorWizard.XMLCatalog := FXMLCatalog;
      FfrmConstructorWizard.DataProvider := DataProvider;
    end;
    FXMLCatalog.SetUp(DataProvider);
    if not FXMLCatalog.Loaded then
      exit;

    Dimension := PlaningSheet.GetDimension(DimensionName);
    if not Assigned(Dimension) then
      exit;
    case Dimension.GetObjectType of
      wsoRow: WizardRunMode := wrmEditRow;
      wsoColumn: WizardRunMode := wrmEditColumn;
      wsoFilter: WizardRunMode := wrmEditFilter;
    else
      WizardRunMode := wrmNone;
    end;
    result := FfrmConstructorWizard.RunWizard(WizardRunMode, Dimension.UniqueID);
    if result then
      PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetMemberProperty(const DimensionName, UniqueName,
  MemberPropertyName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  Dimension: TSheetDimension;
  MembersDom: IXmlDomDocument2;
  IsLeaf: boolean;
  PropertyName: string;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Dimension := PlaningSheet.GetDimension(DimensionName);
    if not Assigned(Dimension) then
      exit;
    MembersDom := Dimension.Members;
    if not Assigned(MembersDom) then
      exit;
    PropertyName := MemberPropertyName;
    EncodeMemberPropertyName(PropertyName);
    result := GetMemberAttrByKey(MembersDom, 'unique_name', UniqueName, PropertyName, IsLeaf);
  finally
    PlaningSheet.Clear;
  end;  
end;

function TFMExcelAddIn.VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString;
var
  Sheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  AxisCondition: string;

  function GetAxisCondition: string;
  var
    i: integer;
    DimName: string;
    ElementAxis: TSheetAxisElementInterface;
  begin
    result := '';
    if VarIsNull(Coordinates) then
      exit;
    result := '[';
    for i := VarArrayLowBound(Coordinates, 1) to VarArrayHighBound(Coordinates, 1) do
    begin
      DimName := VarToStr(Coordinates[i, 0]);
      ElementAxis := PlaningSheet.Columns.FindByDimension(DimName);
      if not Assigned(ElementAxis) then
      begin
        ElementAxis := PlaningSheet.Rows.FindByDimension(DimName);
        if not Assigned(ElementAxis) then
          continue;
      end;
      result := result + '(@' + ElementAxis.Alias + '="' + VarToStr(Coordinates[i, 1]) + '")';
    end;
    result := result + ']';
  end;

  function GetNodeValue(Dom: IXmlDomDocument2; TotalAlias, AxisCondition: string): string;
  var
    XPath: string;
    Node: IXmlDomNode;
  begin
    result := '';
    XPath := 'function_result/data/row' + AxisCondition;
    Node := Dom.selectSingleNode(XPath);
    if not Assigned(Node) then
      exit;
    result := GetStrAttr(Node, TotalAlias, '');
  end;

begin
  result := '';
  exit;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmAll) then
  try
    Total := PlaningSheet.Totals.FindByCaption(TotalName);
    if not Assigned(Total) then
      exit;
    AxisCondition := GetAxisCondition;

    if Assigned(PlaningSheet.FreeTotalsData) then
      result := GetNodeValue(PlaningSheet.FreeTotalsData, Total.Alias, AxisCondition);
    if (result <> '') then
      exit;
    if Assigned(PlaningSheet.FreeTotalsDataIgnored) then
      result := GetNodeValue(PlaningSheet.FreeTotalsDataIgnored, Total.Alias, AxisCondition);
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetSingleCellValue(const SingleCellName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  SingleCell: TSheetSingleCellInterface;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    SingleCell := PlaningSheet.SingleCells.FindByName(SingleCellName);
    if not Assigned(SingleCell) then
      exit;
    result := SingleCell.Value;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.UpdateStatusBar(ExcelSheet: ExcelWorkSheet);
var
  ExcelBook: ExcelWorkbook;
  TaskID, StatusText: string;
begin
  if not Assigned(ExcelSheet) then
  begin
    SetStatusBarInfo(sbiTaskID, 'нет');
    SetStatusBarInfo(sbiRefreshDate, fmNoRefreshDate);
    SetStatusBarInfo(sbiOnlineMode, 'Обычный');
    SetStatusBarInfo(sbiContext, 'нет');
    exit;
  end;
  try
    ExcelBook := (ExcelSheet.Parent as ExcelWorkbook);
    TaskId := 'нет';
    if Assigned(Environment) then
      TaskId := Environment.TaskId;
    SetStatusBarInfo(sbiTaskID, TaskId);
    if PlaningSheet.Load(ExcelSheet, GetContext, [lmInner]) then
    try
      SetStatusBarInfo(sbiRefreshDate, IIF((PlaningSheet.LastRefreshDate <> ''),
        PlaningSheet.LastRefreshDate, fmNoRefreshDate));
      if PlaningSheet.Online then
        if PlaningSheet.InConstructionMode then
          StatusText := 'конструирования'
        else
          StatusText := 'работы с данными'
      else
        StatusText := 'автономный';
      SetStatusBarInfo(sbiOnlineMode, StatusText);
      {При открытии книги из задач контекст передается слишком поздно,
        в то время как признак загрузки из задачи выставляется сразу же.}
      SetStatusBarInfo(sbiContext, IIF(IsLoadingFromTask or Assigned(Context), 'да', 'нет'));
    finally
      PlaningSheet.Clear;
    end
  except
  end;
end;

function TFMExcelAddIn.PrepareRebuild(LoadMode: TLoadMode; out ExlSheet: ExcelWorkSheet): boolean;
begin
  result := false;
  DisableCopyMode;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if TryConnection(false) then
    result := LoadCatalogAndMetadata(LoadMode, ExlSheet)
  else
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

function TFMExcelAddin.LoadCatalogAndMetadata(LoadMode: TLoadMode; ESheet: ExcelWorkSheet): boolean;
var
  Op: IOperation;
begin
  result := false;
  Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    {загрузка каталога}
    Op.StartOperation(Host.Hwnd);
    Op.Caption := pcapLoadMetadata;
    FXMLCatalog.SetUp(DataProvider);
    if not FXMLCatalog.Loaded then
    begin
      Op.StopOperation;
      ShowDetailError(ermMetaDataLoadFault, DataProvider.LastError,
        ermMetaDataLoadFault);
      exit;
    end;
    if LoadMode = [lmCatalogOnly] then
    begin
      result := true;
      exit;
    end;
    {загрузка метаданных}
    Op.Caption := pcapCollectMetadata;
    LoadContextFromTask(false);
    if not PlaningSheet.Load(ESheet, Context, LoadMode) then
      exit;
    result := true;
  finally
    //чтобы эксель не терял фокус
    Application.ProcessMessages;
    Op.StopOperation;
  end;
end;

procedure TFMExcelAddIn.SetTypeFormulaHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorksheet;
  Mode: TWizardRunMode;
  Id, Query, ErrorText: string;
  Index: integer;
  Total: TSheetTotalInterface;
  TypeFormula: TTypeFormula;
  Selection: ExcelRange;
begin
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  if PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData) then
  try
    MayHook := false;

    Index := PlaningSheet.Totals.FindById(Id);
    Total := PlaningSheet.Totals[Index];
    if not Assigned(Total) then
      exit;

    Selection := Host.ActiveCell;
    if not IsTypeFormulaAllowed(Selection, ErrorText) then
    begin
      ShowError(ErrorText);
      exit;
    end;
    TypeFormula := PlaningSheet.GetTypeFormula(Total, Selection.Row, Selection.Column);
    if not Assigned(TypeFormula) or not TypeFormula.IsValid(ErrorText) then
    begin
      ShowError(ermTypeFormulaFault + ErrorText);
      exit;
    end;

    {Запускаем индикатор}
    PlaningSheet.StartOperation(Host.Hwnd, pfoTypeFormulaRefresh);
    Host.ScreenUpdating[LCID] := false;

    {очищаем значения показателя старой типовой формулы}
    Total.ClearTypeFormulaValues;
    SetCellFormula(ExcelSheet, Selection, '');
    Total.TypeFormula := TypeFormula;
    Total.TypeFormula.Enabled := true;
    PlaningSheet.MapTypeFormula(Total);
    PlaningSheet.Save;
  finally
    MayHook := true;
    PlaningSheet.StopOperation;
    PlaningSheet.Clear;
    Host.ScreenUpdating[LCID] := true;
  end;
end;

procedure TFMExcelAddIn.WorkModeHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  NeedSave: boolean;
begin
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  NeedSave := PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData);
  try
    PlaningSheet.InConstructionMode := not PlaningSheet.InConstructionMode;
    UpdateWorkModeButton(Button);
    PlaningSheet.UpdateAllowEditRanges;
    if NeedSave then
      PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
  UpdateStatusBar(ExcelSheet);
end;

procedure TFMExcelAddIn.UpdateWorkModeButton(AButton: CommandBarButton);
var
  i, cnt: integer;
  Button: CommandBarButton;
begin
  { Если кнопка смены режима не была передана параметром, то найдем ее сами}
  if not Assigned(AButton) then
  begin
    if not Assigned(ToolBar) then
      exit;
    cnt := ToolBar.Controls.Count;
    for i := 1 to cnt do
      if (ToolBar.Controls[i].Tag = tagToolButtonWorkMode) then
      begin
        Button := CommandBarButton(ToolBar.Controls[i]);
        break;
      end;
  end
  else
    Button := AButton;

  { Установим вид и хинт}
  if PlaningSheet.InConstructionMode then
  begin
    {$WARNINGS OFF}
    Button.Set_State(msoButtonDown);
    Button.Set_TooltipText(kriInDataMode);
    {$WARNINGS ON}
  end
  else
  begin
    Button.Set_State(msoButtonUp);
    Button.Set_TooltipText(kriInConstructionMode);
  end;

  { Обновим состояние пунктов подменю "Добавить"}
  with PlaningSheet do
  begin
    UpdateAppendSubmenuItem(true, tagToolButtonTotals, Totals.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonTotals, Totals.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonRows, Rows.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonRows, Rows.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonColumns, Columns.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonColumns, Columns.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonFilters, Filters.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonFilters, Filters.MayBeEdited);
    EnableSingleCellAppend;
  end;
  
  RefreshSplitterPad;
end;

procedure TFMExcelAddIn.OfflineFiltersEditorHandler(
  Button: CommandBarButton; var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  IsSuccess: boolean;
  ID, DummyStr, HistoryString: string;
  DummyWRM: TWizardRunMode;
begin
  // если это не лист планирования - выходим
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not IsPlaningSheet(ExcelSheet) then
    exit;
  DisableCopyMode;
  IsSuccess := false;
  // грузим метаданные
  if PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData) then
  try
    SetSheetProtection(ExcelSheet, false);
    Id := '';
    ParseAdditionalInfo(Button.Get_HelpFile, DummyWRM, ID, DummyStr);
    IsSuccess := EditFiltersOffline(PlaningSheet, Id, HistoryString);
    if IsSuccess then
    begin
      PlaningSheet.AddEventInSheetHistory(evtEdit, HistoryString, true);
      PlaningSheet.Save;
    end;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
  end;
end;

procedure TFMExcelAddIn.DataCollectionFormHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  IsSuccess: boolean;
  HistoryString: string;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExcelSheet) then
  try
    SetSheetProtection(ExcelSheet, false);
    IsSuccess := MakeDataCollectionForm(PlaningSheet, HistoryString);
    Application.ProcessMessages;
    if not IsSuccess then
      exit;

    AncillaryInit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh,mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExcelSheet.Name + '"',
      not CriticalNode, NoteTime, otProcess);
    CollectDeferredTotalsData;
    PlaningSheet.NeedMapSingleCells := false;
    PlaningSheet.PrepareToSwitchOffline;
    IsSuccess := DoRebuildSheet(ExcelSheet);
    PlaningSheet.AddEventInSheetHistory(evtRefresh,
        {MergeCommaText(HistoryString,} GetErrorList{)}, IsSuccess);

    if not IsSuccess then
    begin
      PlaningSheet.UpdateFiltersText;
      PlaningSheet.UpdateTotalsComments;
    end;

    { Перевод листа в автономный режим}
    PlaningSheet.SwitchOffline;
    FProcess.OpenOperation(pfoSwitchSheetOffline, not CriticalNode, not NoteTime, otProcess);
    FProcess.CloseOperation;
    PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
      MergeCommaText(ConvertStringToCommaText('Лист переведен в автономный режим.'),
      HistoryString), true);
    PlaningSheet.Save;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
    if IsSuccess then
    begin
      if TrySwitchWorkbookOffline then
        PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
          ConvertStringToCommaText('Тип документа изменен на "форма сбора данных"'), true);
        // Пока неясно, надо здесь что-нибудь писать или нет...
        //FProcess.PostInfo('Тип документа изменен на "форма сбора данных"');
        (*PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
          ConvertStringToCommaText('Тип документа изменен на "форма сбора данных"', true);*)
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
    UpdateWorkModeButton(nil);
  end;
end;

procedure TFMExcelAddIn.CollectDeferredTotalsData;
var
  tmpCollector: TPlaningSheet;
begin
  if not CheckTotalsForCollect then
    exit;
  tmpCollector := TPlaningSheet.CreateInherited(PlaningSheet);
  tmpCollector.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
  tmpCollector.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
    FfrmSheetHistory);
  tmpCollector.Load(PlaningSheet.ExcelSheet, Context, lmAll);
  PlaningSheet.FreeTotalsData := tmpCollector.FreeTotalsData;
  PlaningSheet.FreeTotalsDataIgnored := tmpCollector.FreeTotalsDataIgnored;
  PlaningSheet.FormulaArrays := tmpCollector.FormulaArrays;
  PlaningSheet.SingleCellsData := tmpCollector.SingleCellsData;
  PlaningSheet.LoadMode := PlaningSheet.LoadMode + [lmFreeData];
  tmpCollector.Free;
end;

function TFMExcelAddIn.TrySwitchWorkbookOffline: boolean;
var
  i: integer;
  Book: ExcelWorkbook;
  Sheet: ExcelWorkSheet;
  tmpPlaningSheet: TPlaningSheet;
  Ok: boolean;
begin
  result := false;
  Book := Host.ActiveWorkbook;
  if not Assigned(Book) then
    exit;
  Ok := true;
  tmpPlaningSheet := TPlaningSheet.CreateInherited(PlaningSheet);
  tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
  try
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    for i := 1 to Book.Sheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Sheets[i]);
      if not Assigned(Sheet) then
        continue;
      if not IsPlaningSheet(Sheet) then
        continue;
      tmpPlaningSheet.Load(Sheet, Context, lmNoFreeData);
      Ok := Ok and (not tmpPlaningSheet.Online or
        tmpPlaningSheet.Online and tmpPlaningSheet.Empty);
      tmpPlaningSheet.Clear;
    end;
  finally
    tmpPlaningSheet.Free;
  end;
  if Ok then
  begin
    SetPropValueByName(pspSheetType, '3');
    UpdateStatusBar(GetWorkSheet(Host.ActiveSheet));
    result := true;
  end;
end;

procedure TFMExcelAddIn.OnTimerHandler(Sender: TObject);
begin
  {Раз в три минуты уведомляем веб-сервис, что все еще живы}
  DataProvider.ClientSessionIsAlive;
end;

procedure TFMExcelAddIn.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

function TFMExcelAddIn.GetProcessLogFileName: string;
begin
  result := AddinLogPath + 'ProcessLog.txt';
end;

procedure TFMExcelAddIn.UpdateProcessLogging;
begin
  if not Assigned(FProcess) then
    exit;
  if AddinLogEnable then
    FProcess.EnableLogging(GetProcessLogFileName)
  else
    FProcess.DisableLogging;
end;

procedure TFMExcelAddIn.SwitchTotalTypeHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

var
  Total: TSheetBasicTotal;
  Collection: TSheetCollection;
  ESheet: ExcelWorkSheet;
  IsSuccess: boolean;
  Index: integer;
  Mode: TWizardRunMode;
  Id, tmpString, CommentForHistory: string;
  tmpStringList: TStringList;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, tmpString);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    with PlaningSheet do
    begin
      if Empty then
        exit;
      if Mode = wrmNone then
        Collection := SingleCells
      else
        Collection := Totals;
      Index := Collection.FindById(Id);
      if Index < 0 then
        exit;
      Total := Collection[Index];
      {сюда мы попадем только когда все ОК и проверки на возможность смены типа не нужны}
      if Total.TotalType = wtResult then
        tmpString := 'Тип показателя %s будет изменен с результата на меру.' +
          ' Все введенные значения и формулы будут заменены данными из базы. Продолжить?'
      else
        tmpString := 'Тип показателя %s будет изменен с меры на результат. Продолжить?';
      if not ShowQuestion(Format(tmpString, [Total.GetElementCaption])) then
        exit;
    end;

    Application.ProcessMessages;
    IsSuccess := false;
    try
      FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
      FProcess.NewProcessClear := false;
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ESheet.Name + '"',
        not CriticalNode, NoteTime, otProcess);
      FUserEvents.Execute(ESheet, enBeforeRefresh);
      FXMLCatalog.SetUp(DataProvider);
      PlaningSheet.Clear;
      if not PlaningSheet.Load(ESheet, Context, lmAll) then
        exit;

      Total := Collection[Index];
      if Mode = wrmNone then
      begin
        TSheetSingleCellInterface(Total).SwitchType(CommentForHistory);
        tmpStringList := TStringList.Create;
        tmpStringList.Add(IntToStr(Index));
        IsSuccess := PlaningSheet.MapSingleCells(tmpStringList, tmpString, true, nil, nil);
        if IsSuccess then
          PlaningSheet.Save;
      end
      else
      begin
        Total.SwitchType(CommentForHistory);
        IsSuccess := DoRebuildSheet(ESheet);
      end;

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    finally
      AfterRebuild(ESheet, IsSuccess);
      CalculateSheet(ESheet);
      if IsSuccess then
      begin
        FUserEvents.Execute(ESheet, enAfterRefresh);
        FProcess.CloseOperation; //pfoRebuild
      end;
      FProcess.CloseProcess;
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType;
  const Comment: WideString; Success: WordBool);
var
  Book: ExcelWorkBook;
  ESheet: ExcelWorkSheet;
  EventType: TSheetEventType;
  i: integer;
begin
  EventType := TSheetEventType(EType);
  try
    Book := Doc as ExcelWorkBook;
  except
    exit;
  end;

  for i := 1 to Book.Sheets.Count do
  begin
    ESheet := GetWorkSheet(Book.Sheets.Item[i]);
    if not IsPlaningSheet(ESheet) then
      continue;
    FfrmSheetHistory.AddEvent(ESheet, EventType, ConvertStringToCommaText(Comment), Success);
  end
end;

function TFMExcelAddIn.CheckTotalsByType(CheckType: TSheetTotalType): boolean;
var
  i: integer;
begin
  result := true;
  for i := 0 to PlaningSheet.Totals.Count - 1 do
    if (PlaningSheet.Totals[i].TotalType = CheckType) then
      exit;
  for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    if (PlaningSheet.SingleCells[i].TotalType = CheckType) then
      exit;
  result := false;
end;

function TFMExcelAddIn.CheckTotalsForCollect: boolean;
begin
  result := CheckTotalsByType(wtResult) or CheckTotalsByType(wtFree);
end;

function TFMExcelAddIn.CheckTotalsForWriteback: boolean;
begin
  result := CheckTotalsByType(wtResult);
end;

procedure TFMExcelAddIn.CommonOptionsHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  EditCommonOptions(PlaningSheet);
end;

procedure TFMExcelAddIn.OneTotalRefreshHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ESheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  Mode: TWizardRunMode;
  Id, Query: string;
  Index: integer;
  CheckFormulas, Alerts, RefreshResult: boolean;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;

  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;

  AncillaryInit;
  FProcess.OpenProcess(Host.Hwnd, 'Обновление показателя', mSuccessRefresh, mErrorRefresh, false);
  FProcess.NewProcessClear := false;

  FProcess.OpenOperation('Обновление показателя', not CriticalNode, NoteTime, otProcess);

  FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
  FXMLCatalog.SetUp(DataProvider);
  if not PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
    exit;
  FProcess.CloseOperation;

  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Index := PlaningSheet.Totals.FindById(Id);
  if Index < 0 then
    exit;
  try
    if not CheckAxesSaved(ESheet) then
      exit;

    Total := PlaningSheet.Totals[Index];
    FProcess.OpenOperation('Обновление показателя' + ' "' + Total.GetElementCaption + '"', not CriticalNode, NoteTime, otProcess);

    if not SetSheetProtection(ESheet, false) then
    begin
      FProcess.PostError(ermWorksheetProtectionFault);
      exit;
    end;
    if not PlaningSheet.Validate(Total) then
    begin
      FProcess.CloseOperation;
      exit;
    end;

    ESheet.Application.ScreenUpdating[LCID] := false;
    ESheet.Application.Set_Interactive(LCID, false);

    {В процессе изменения листа формулы могут оказаться некорректными,
    поэтому, что бы при этом не вылезали экселевские окна, временно отключаем
    проверку корректности формул}
    CheckFormulas := Host.ErrorCheckingOptions.EvaluateToError;
    Host.ErrorCheckingOptions.EvaluateToError := false;
    Alerts := Host.DisplayAlerts[LCID];
    Host.DisplayAlerts[LCID] := false;
    RefreshResult := false;
    try
      RefreshResult := PlaningSheet.MapOneTotal(Total);
    finally
      PlaningSheet.AddEventInSheetHistory(evtRefresh,
        ConvertStringToCommaText(
        Format('Обновление показателя "%s"', [Total.GetElementCaption])),
        RefreshResult);
      //восстанавливаем проверку формул
      Host.ErrorCheckingOptions.EvaluateToError := CheckFormulas;
      Host.DisplayAlerts[LCID] := Alerts;
      FProcess.CloseOperation;
    end;
    FProcess.CloseOperation;
  finally
    AfterRebuild(ESheet, false);
    PlaningSheet.Clear;
    FProcess.CloseProcess;
  end;
end;

procedure TFMExcelAddIn.SetAuthenticationInfoNew;
begin
  if Assigned(Environment) then
    SetAuthenticationInfo(Environment.AuthType, Environment.Login, Environment.PwdHash);
end;

procedure TFMExcelAddIn.SetConnectionStrNew;
begin
  if not Assigned(Environment) then
    exit;
  if SetConnectionStr(Environment.ConnectionStr, Environment.SchemeName) = 0 then
    exit;
  SetConnectionStr(Environment.AlterConnection, Environment.SchemeName);
end;

function TFMExcelAddIn.GetEnvironment: TTaskEnvironment;
begin
  result := FTaskContextProvider.TaskEnvironment[Host.ActiveWorkbook];
end;

procedure TFMExcelAddIn.SetTaskConnectionLoadNew;
begin
  if Assigned(Environment) then
    if Environment.IsTaskConnect then
      OnTaskConnection(true);
end;

procedure TFMExcelAddIn.DoMassAction;
var
  NeedRewrite, NeedProcess, Ok: boolean;
  Msg: string;
begin
  Ok := false;
  Msg := '';
  try
    try
      if Environment.Action and datRefresh = datRefresh then
        RefreshActiveBook;

      if Environment.Action and datWriteback = datWriteback then
      begin
        NeedRewrite := Environment.Action and datRewrite = datRewrite;
        NeedProcess := Environment.Action and datProcessCube = datProcessCube;
        WritebackActiveBookEx(NeedRewrite, NeedProcess);
      end;

      if Environment.Action and datRefreshAfter = datRefreshAfter then
        RefreshActiveBook;

      Ok := FProcess.LastError = '';
      if not Ok then
        Msg := FProcess.LastError;
    except
    end;
  finally
    Environment.SetActionResult(Host.ActiveWorkbook, Ok, Msg);
  end;
end;

function TFMExcelAddIn.GetStatic: boolean;
begin
  if Assigned(PlaningSheet.Environment) then
    result := PlaningSheet.Environment.IsLoadingFromTask
  else
    result := false;
end;

procedure TFMExcelAddIn.SetTaskContext(const TaskContextXml: Widestring;
  IsPacked: WordBool);
begin
;
end;

function TFMExcelAddIn.CheckAxesSaved(ExcelSheet: ExcelWorkSheet): boolean;
var
  RowsCPSaved, ColumnsCPSaved: boolean;
begin
  RowsCPSaved := Assigned(GetCPByName(ExcelSheet, cpRowsAxis, false));
  ColumnsCPSaved := Assigned(GetCPByName(ExcelSheet, cpColumnsAxis, false));
  result := (not PlaningSheet.Rows.Empty and RowsCPSaved) or
    (not PlaningSheet.Columns.Empty and ColumnsCPSaved) or
    (PlaningSheet.Rows.Empty and PlaningSheet.Columns.Empty);
  if not Result then
    PlaningSheet.PostMessage('Необходимо полное обновление листа', msgError);
end;

procedure TFMExcelAddIn.LoadContextFromTask(ShowGears: boolean);
var
  TaskId: integer;
  ContextXml: string;
  Op: IOperation;
begin
  if not Environment.IsLoadingFromTask then
    exit;
  try
    try
      if ShowGears then
      begin
        Op := CreateComObject(CLASS_OPERATION) as IOperation;
        Op.StartOperation(Host.Hwnd);
        Op.Caption := 'Обновление контекста';
      end;

      TaskId := StrToInt(Environment.TaskId);
      ContextXml := DataProvider.GetTaskContext(TaskId);
    except
      ContextXml := '';
    end;
    FTaskContextProvider.UpdateContext(Host.ActiveWorkbook, ContextXml);
  finally
    if ShowGears then
    begin
      Op.StopOperation;
      Op := nil;
    end;
  end;
end;

procedure TFMExcelAddIn.OnProcessFormReturn;
begin
  RebuildCycleState := rcsReturn;
end;

procedure TFMExcelAddIn.OnProcessFormClose;
begin
  if RebuildCycleState <> rcsReturn then
    RebuildCycleState := rcsLeave;
  if Host.Visible[LCID] then
  try
    Host.Application.Set_Interactive(LCID, true);
  except
    on e: Exception do ShowError(e.message);
  end;
end;

procedure TFMExcelAddIn.OnProcessFormShow;
begin
  if Host.Visible[LCID] then
    Host.Application.Set_Interactive(LCID, false);
end;

procedure TFMExcelAddIn.FreezePanesHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  Host.ActiveWindow.FreezePanes := not Host.ActiveWindow.FreezePanes;
end;

procedure TFMExcelAddIn.ReplicationHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
begin
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExcelSheet) then
  begin
    RunReplicationWizard(PlaningSheet, FfrmSheetHistory);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.SheetInfoHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ESheet: ExcelWorksheet;
begin
  if PrepareRebuild(lmNoFreeData, ESheet) then
  begin
    ViewSheetInfo(PlaningSheet);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.HideTotalColumnsHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  Mode: TWizardRunMode;
  Id, Question: string;
  Index, i: integer;
  ESheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  Section: ExcelRange;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Question);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    Index := PlaningSheet.Totals.FindById(Id);
    if Index < 0 then
      exit;
    Total := PlaningSheet.Totals[Index];
    Total.IsHidden := true;
    PlaningSheet.Save;
    SetSheetProtection(ESheet, false);
    for i := 0 to Total.SectionCount - 1 do
    begin
      Section := Total.GetTotalRange(i);
      if not Assigned(Section) then
        continue;
      Section.EntireColumn.Hidden := true;
    end;
    SetSheetProtection(ESheet, false);
  finally
    PlaningSheet.Clear;
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TFMExcelAddIn, Class_DTExtensibility2,
    ciMultiInstance, tmApartment);
end.


