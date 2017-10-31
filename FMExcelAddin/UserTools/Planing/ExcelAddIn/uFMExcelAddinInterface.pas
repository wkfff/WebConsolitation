unit uFMExcelAddinInterface;

{Сюда вынесена основная масса работы с пользовательским интерфейсом Excel}

interface

uses
  ComObj, ActiveX, Windows, SysUtils, Classes,
  ExcelXP, OfficeXP,
  ComAddInUtils, ExcelComAddinEvents,
  FMExcelAddin_TLB, PlaningProvider_TLB,
  uFMExcelAddinConst, uGlobalPlaningConst, uFMAddinGeneralUtils, uOfficeUtils, uFMAddinExcelUtils,
  uConstructorWizard, uFMAddinRegistryUtils, uSheetCollector, uSheetObjectModel, uExcelUtils,
  uConverter;

type

  {Собственно корень объектной модели листа.
   Переобъявлен здесь, потому что имя конкретного класса может часто меняться.
   Должен просто наследоваться от последнего потомка.}
  TPlaningSheet = class(TSheetCollector);

  TStatusBarInformer = (sbiRefreshDate, sbiTaskId, sbiContext, sbiOnlineMode);

  TFMExcelAddinInterface = class(TAutoObject, IDTExtensibility2)
  private
    FHost: ExcelApplication;

    // ссылка на объект ексела (плагин), содержащий наш объект
    FHostAddin: ComAddin;

    FDataProvider: IPlaningProvider;

    //флаг перехвата ввода в ячейку
    FMayHook: boolean;

    {елементы статусбар}
    FStatusBarRefreshDate: CommandBarComboBox;
    FStatusBarTaskID: CommandBarComboBox;
    FStatusBarContext: CommandBarComboBox;
    FStatusBarOnlineMode: CommandBarComboBox;

    { точка подключения к событиям }
    FAppEventSink: TExcelAppEventSink;
    FCommandBarsEventSink: TCommandBarsEventSink;


    { точки подключения }
    //Свойства листа
    FMenuShowSheetPropertiesEventSink: TCommandButtonEventSink;
    FToolBarShowSheetPropertiesEventSink: TCommandButtonEventSink;

    FMenuCommonOptionsEventSink: TCommandButtonEventSink;

    FMenuSheetInfoEventSink: TCommandButtonEventSink;

    //подключение к серверу
    FMenuConnectionEventSink: TCommandButtonEventSink;

    //мастер структуры
    FToolBarConstructorWizardEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить фильтр"
    FToolBarConstructorWizardAddFilterEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddFilterEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить элемент строк"
    FToolBarConstructorWizardAddRowEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddRowEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить элемент столбцов"
    FToolBarConstructorWizardAddColumnEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddColumnEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить показатель-константу"
    FToolBarConstructorWizardAddConstTotalEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddConstTotalEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить показатель-результат"
    FToolBarConstructorWizardAddResultTotalEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddResultTotalEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить показатель-свободный"
    FToolBarConstructorWizardAddFreeTotalEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddFreeTotalEventSink: TCommandButtonEventSink;

    //мастер структуры для подменю "добавить показатель-меру"
    FToolBarConstructorWizardAddMeasureTotalEventSink: TCommandButtonEventSink;
    FMenuConstructorWizardAddMeasureTotalEventSink: TCommandButtonEventSink;

    //обновить данные
    FToolBarRefreshEventSink: TCommandButtonEventSink;
    FMenuRefreshEventSink: TCommandButtonEventSink;

    //панель разрывов
    FMenuBarShowSplitterPadEventSink: TCommandButtonEventSink;

    //обратная запись
    FToolBarSendDataEventSink: TCommandButtonEventSink;
    FMenuSendDataEventSink: TCommandButtonEventSink;

    //пометить ячейку как "пустую"
    FToolBarMarkEmptyEventSink: TCommandButtonEventSink;
    FMenuMarkEmptyEventSink: TCommandButtonEventSink;

    //копирование листов
    FMenuShowCopyFormEventSink: TCommandButtonEventSink;

    // управление параметрами
    FMenuShowParamsEventSink: TCommandButtonEventSink;

    // управление константами
    FMenuShowConstsEventSink: TCommandButtonEventSink;

    // добавление константы на лист
    FToolBarAddConstsEventSink: TCommandButtonEventSink;
    FMenuAddConstsEventSink: TCommandButtonEventSink;

    {редактор отдельных ячеек}
    FToolBarSingleCellEditorEventSink: TCommandButtonEventSink;
    FMenuSingleCellEditorEventSink: TCommandButtonEventSink;

    {менеджер отдельных ячеек}
    FToolbarSingleCellsManagerEventSink: TCommandButtonEventSink;
    FMenuSingleCellsManagerEventSink: TCommandButtonEventSink;

    {редактор свойств элементов листа}
    FToolbarComponentEditorEventSink: TCommandButtonEventSink;
    FMenuComponentEditorEventSink: TCommandButtonEventSink;

    { Режим конструирования/режим работы с данными}
    FToolbarWorkModeEventSink: TCommandButtonEventSink;

    // Мастер форм сбора данных
    FMenuDataCollectionFormEventSink: TCommandButtonEventSink;

    // Мастер тиражирования
    FMenuReplicationEventSink: TCommandButtonEventSink;

    // история листа
    FMenuShowSheetHistoryEventSink: TCommandButtonEventSink;


    function GetFLCID: integer;
    function GetMenu: CommandBarControl;
    function GetMainMenu: CommandBar;
    function GetToolBar: CommandBar;
    function GetStatusBar: CommandBar;

    function GetCommandBar(BarName: string): CommandBar;
    function GetCommandBarControl(OnToolbar: boolean; Tag: string): CommandBarControl;
    procedure AppendButtons;
    function AppendPopupMenuItem(ContextMenu: CommandBar;
      NameCaption, AdditionalInfo: widestring; Handler: TOnCommandButtonClick;
      Tag: widestring; BeginGroup: boolean; StyleCaption: MsoButtonStyle;
      Face: integer): CommandBarButton;

    procedure PopupMenuRow(ContextMenu: CommandBar; Id: string);
    procedure PopupMenuColumn(ContextMenu: CommandBar; Id: string);
    procedure PopupMenuMeasure(ContextMenu: CommandBar; Id, Instance: string);
    procedure PopupMenuFree(ContextMenu: CommandBar; ObjType, Id, Instance: string);
    procedure PopupMenuResult(ContextMenu: CommandBar; ObjType, Id, Instance: string);
    procedure PopupMenuConst(ContextMenu: CommandBar; Id, Instance: string);
    procedure PopupMenuFilter(ContextMenu: CommandBar; Id, Instance: string);
    procedure PopupMenuSingleTotal(ContextMenu: CommandBar; ObjType, Id: string);
    procedure PopupMenuSingleConst(ContextMenu: CommandBar; Id: string);
    procedure PopupMenuSingleNew(ContextMenu: CommandBar);

    procedure DetectName(ContextMenu: CommandBar; ExcelName: ExcelXP.Name;
      var ProperPlace: boolean; var AlreadyPlaced: boolean);

  protected
    PlaningSheet: TPlaningSheet;

    {Создание/разбор дополнительной информации}
    function CreateAdditionalInfo(WizardRunMode: TWizardRunMode; ID, Instance: string): widestring;
    function ParseAdditionalInfo(AdditionalInfo: string;
      out WizardRunMode: TWizardRunMode; out Param1, Param2: string): boolean;

    { Обновляет состояние пунктов подменю "Добавить"}
    procedure UpdateAppendSubmenuItem(OnToolbar: boolean; Tag: string; State: boolean);
    procedure CommandBarsUpdate; virtual; abstract;
    {Если включен дополнительный анализ, то при выставлении состоянии будут
    учитываться признаки: требуется листу обновление, отношение версии}
    procedure SetButtonsState(State, SecondaryAnalysis: boolean);


    { IDTExtensibility2 }
    procedure BeginShutdown(var custom: PSafeArray); virtual; safecall;
    procedure OnAddInsUpdate(var custom: PSafeArray); virtual; safecall;
    procedure OnConnection(const HostApp: IDispatch; ext_ConnectMode: integer;
      const AddInInst: IDispatch; var custom: PSafeArray); virtual; safecall;
    procedure OnDisconnection(ext_DisconnectMode: integer; var custom: PSafeArray); virtual; safecall;
    procedure OnStartupComplete(var custom: PSafeArray); virtual; safecall;

    {Обработчики листа}
    procedure SheetActivate(const Sh: IDispatch); virtual; abstract;
    procedure BeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
      var Cancel: WordBool); virtual;
    procedure OnSheetChange(const Sh: IDispatch; const Target: ExcelRange); virtual; abstract;
    procedure OnSheetSelectionChange(const Sh: IDispatch; const Target: ExcelRange); virtual; abstract;

    {Обработчики книги}
    procedure WorkbookActivate(const Wb: ExcelWorkbook); virtual; abstract;
    procedure WorkbookDeactivate(const Wb: ExcelWorkbook); virtual; abstract;
    procedure WorkbookOpen(const Wb: ExcelWorkbook); virtual; abstract;
    procedure WorkbookBeforeSave(const Wb: ExcelWorkbook; SaveAsUI: WordBool; var Cancel: WordBool); virtual; abstract;
    procedure WorkbookBeforeClose(const Wb: ExcelWorkbook; var Cancel: WordBool); virtual; abstract;

    { обработчики кнопок и пунктов меню}

    { Обратная запись}
    procedure SendDataHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SendDataOptionalHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;


    { Управление параметрами и константами}
    procedure ShowParamsHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure ShowConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure AddConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure EditConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure RefreshConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;


    { Мастера и редакторы}
    procedure ConstructorWizardHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure ComponentEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SingleCellEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SingleCellsManagerHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure OfflineFiltersEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure DataCollectionFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure ReplicationHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;


    { Обновление листа}
    procedure RefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure RefreshHandler2(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure OneTotalRefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;


    {Вспомогательные формы}
    procedure ConnectionHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure EditStyleHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SheetHistoryHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SplitterPadHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure CopyFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SheetPropertiesHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure CommonOptionsHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SheetInfoHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;


    {Прочие}
    procedure MoveElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure DeleteElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure InsertNewLineHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure MarkEmptyHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SetTypeFormulaHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure WorkModeHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure SwitchTotalTypeHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure FreezePanesHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;
    procedure HideTotalColumnsHandler(Button: CommandBarButton; var CancelDefault: WordBool); virtual; abstract;

    procedure InitUserInterface;
    procedure DestroyUserInterface; 

    procedure SetStatusBarInfo(Where: TStatusBarInformer; What: string);

    property LCID: integer read GetFLCID;
    property Host: ExcelApplication read FHost;
    property Menu: CommandBarControl read GetMenu;
    property MainMenu: CommandBar read GetMainMenu;
    property ToolBar: CommandBar read GetToolBar;
    property StatusBar: CommandBar read GetStatusBar;
    property DataProvider: IPlaningProvider read FDataProvider;
    property MayHook: boolean read FMayHook write FMayHook;
  public
  end;

implementation

{ TFMExcelAddinInterface }

procedure TFMExcelAddinInterface.BeginShutdown(var custom: PSafeArray);
begin

end;

procedure TFMExcelAddinInterface.OnAddInsUpdate(var custom: PSafeArray);
begin

end;

procedure TFMExcelAddinInterface.OnConnection(const HostApp: IDispatch;
  ext_ConnectMode: integer; const AddInInst: IDispatch;
  var custom: PSafeArray);
begin
  FHost := HostApp as ExcelApplication;
  FHostAddin := (AddinInst) as ComAddin;
  if Assigned(FHostAddin) then
    FHostAddin.Object_ := Self as IDispatch;
  FDataProvider := CreateComObject(CLASS_PlaningProvider_) as IPlaningProvider;
end;

procedure TFMExcelAddinInterface.OnDisconnection(ext_DisconnectMode: integer; var custom: PSafeArray);
begin
  //FMenu := nil;
  FHost := nil;
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
  end;

  FStatusBarRefreshDate := nil;
  FStatusBarTaskID := nil;

  DataProvider.FreeProvider;
  FDataProvider := nil;
end;

procedure TFMExcelAddinInterface.OnStartupComplete(var custom: PSafeArray);
begin

end;

function TFMExcelAddinInterface.GetFLCID: integer;
begin
  result := GetUserDefaultLCID;
end;



function TFMExcelAddinInterface.GetMenu: CommandBarControl;
begin
  { Ищем ранее созданное}
  result := Host.CommandBars.ActiveMenuBar.FindControl(msoControlPopup,
      EmptyParam, tagMenu, false, true);
  if Assigned(result) then
    exit;

  { Меню еще не создано - создаем}
  result := MainMenu.Controls.Add(msoControlPopup, EmptyParam, EmptyParam,
    MainMenu.Controls.Count, msoFalse);
  result.Set_Tag(tagMenu);
  result.Set_Caption(kriMenuCaption);
end;

function TFMExcelAddinInterface.GetMainMenu: CommandBar;
begin
  result := Host.CommandBars.ActiveMenuBar;
end;

function TFMExcelAddinInterface.GetToolBar: CommandBar;
begin
try
  { Ищем ранее созданный}
  result := GetCommandBar(kriToolbarCaption);
  if Assigned(result) then
    exit;

  { Тулбар еще не создан - создаем.
    ПАРАМЕТРЫ: Название, Положение на экране, Заменяет главное меню?, Временный элемент?}
  result := Host.CommandBars.Add(kriToolbarCaption, msoBarTop, false, false);
except
  on e: exception do
    DebugShowInfo(e.message);
end;
end;

function TFMExcelAddinInterface.GetStatusBar: CommandBar;
begin
  { Ищем ранее созданный}
  result := GetCommandBar(kriStatusbarCaption);
  if Assigned(result) then
    exit;

  { Статусбар еще не создан - создаем.}
  result := Host.CommandBars.Add(kriStatusbarCaption, msoBarFloating, false, false);
end;

function TFMExcelAddinInterface.GetCommandBar(BarName: string): CommandBar;
begin
  try
    result := Host.CommandBars.Get_Item(BarName);
  except
    result := nil;
  end;
end;

procedure TFMExcelAddinInterface.InitUserInterface;
begin
  if Assigned(Menu) then
    Menu.Set_Visible(true);

  AppendButtons;

  { цепляем обработчики событий Application}
  FAppEventSink := TExcelAppEventSink.Create;
  FAppEventSink.OnExcelAppSheetSelectionChange := OnSheetSelectionChange;
  FAppEventSink.OnExcelAppSheetBeforeRightClick := BeforeRightClick;
  FAppEventSink.OnExcelAppWorkbookActivate := WorkbookActivate;
  FAppEventSink.OnExcelAppWorkbookDeactivate := WorkbookDeactivate;
  FAppEventSink.OnExcelAppWorkbookBeforeClose := WorkbookBeforeClose;
  FAppEventSink.OnExcelAppWorkbookOpen := WorkbookOpen;
  FAppEventSink.OnExcelAppSheetChange := OnSheetChange;
  FAppEventSink.OnExcelAppSheetActivate := SheetActivate;
  FAppEventSink.OnExcelAppWorkbookBeforeSave := WorkbookBeforeSave;
  FAppEventSink.Connect(Host);

  FCommandBarsEventSink := TCommandBarsEventSink.Create;
  FCommandBarsEventSink.OnUpdate := CommandBarsUpdate;
  FCommandBarsEventSink.Connect(Host.CommandBars);

  MayHook := true;
  Host.ExtendList := false;
  CopyToHKCU;
end;

procedure TFMExcelAddinInterface.DestroyUserInterface;
begin
  { уничтожаем обработчик событий кнопки }
  FreeAndNil(FMenuShowSheetPropertiesEventSink);
  FreeAndNil(FMenuCommonOptionsEventSink);
  FreeAndNil(FToolBarShowSheetPropertiesEventSink);
  FreeAndNil(FMenuShowSheetHistoryEventSink);
  FreeAndNil(FMenuShowConstsEventSink);
  FreeAndNil(FMenuShowParamsEventSink);
  FreeAndNil(FMenuSingleCellsManagerEventSink);
  FreeAndNil(FToolbarSingleCellsManagerEventSink);
(*  FreeAndNil(FToolBarShowSplitterPadEventSink); *)
  FreeAndNil(FMenuBarShowSplitterPadEventSink);
  FreeAndNil(FMenuConnectionEventSink);
  FreeAndNil(FToolBarConstructorWizardEventSink);
  FreeAndNil(FMenuConstructorWizardEventSink);
  FreeAndNil(FMenuDataCollectionFormEventSink);
  FreeAndNil(FMenuReplicationEventSink);
  FreeAndNil(FMenuSheetInfoEventSink);
  {подменю "добавить" - тулбар}
  FreeAndNil(FToolBarAddConstsEventSink);
  FreeAndNil(FToolBarSingleCellEditorEventSink);
  FreeAndNil(FToolBarConstructorWizardAddFilterEventSink);
  FreeAndNil(FToolBarConstructorWizardAddColumnEventSink);
  FreeAndNil(FToolBarConstructorWizardAddRowEventSink);
  FreeAndNil(FToolBarConstructorWizardAddConstTotalEventSink);
  FreeAndNil(FToolBarConstructorWizardAddResultTotalEventSink);
  FreeAndNil(FToolBarConstructorWizardAddFreeTotalEventSink);
  FreeAndNil(FToolBarConstructorWizardAddMeasureTotalEventSink);
  {подменю "добавить" - меню}
  FreeAndNil(FMenuAddConstsEventSink);
  FreeAndNil(FMenuSingleCellEditorEventSink);
  FreeAndNil(FMenuConstructorWizardAddFilterEventSink);
  FreeAndNil(FMenuConstructorWizardAddColumnEventSink);
  FreeAndNil(FMenuConstructorWizardAddRowEventSink);
  FreeAndNil(FMenuConstructorWizardAddConstTotalEventSink);
  FreeAndNil(FMenuConstructorWizardAddResultTotalEventSink);
  FreeAndNil(FMenuConstructorWizardAddFreeTotalEventSink);
  FreeAndNil(FMenuConstructorWizardAddMeasureTotalEventSink);

  FreeAndNil(FToolBarRefreshEventSink);
  FreeAndNil(FMenuRefreshEventSink);
  FreeAndNil(FToolBarSendDataEventSink);
  FreeAndNil(FMenuSendDataEventSink);
  FreeAndNil(FToolBarMarkEmptyEventSink);
  FreeAndNil(FMenuMarkEmptyEventSink);
  FreeAndNil(FMenuShowCopyFormEventSink);

  FreeAndNil(FToolbarWorkModeEventSink);

    { уничтожаем тулбар}
  if Assigned(ToolBar) then
  try
    while ToolBar.Controls.Count > 0 do
      ToolBar.Controls[1].Delete(msoFalse);
    ToolBar.Delete;
  except
  end;

  { уничтожаем статусбар}
  if Assigned(StatusBar) then
  try
    while StatusBar.Controls.Count > 0 do
      StatusBar.Controls[1].Delete(msoFalse);
    StatusBar.Delete;
  except
  end;

  { уничтожаем меню}
  if Assigned(Menu) then
  try
    while (Menu as CommandBarPopup).Controls.Count > 0 do
      (Menu as CommandBarPopup).Controls[1].Delete(msoFalse);
    Menu.Delete(msoFalse);
  except
  end;

  FreeAndNil(FAppEventSink);
  FreeAndNil(FCommandBarsEventSink);
end;

procedure TFMExcelAddinInterface.AppendButtons;
begin
  if not (Assigned(ToolBar) and Assigned(Menu) and (Assigned(StatusBar))) then
    exit;

(*  // свойства листа - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonSheetProperties,
    kriSheetPropertiesCaption, 855, msoButtonIcon, true,
    FToolBarShowSheetPropertiesEventSink, SheetPropertiesHandler, '');    *)
  // свойства листа - меню
  AppendInMenuButton(Menu, '', tagMenuSheetProperties,
    kriSheetPropertiesCaption, 0, msoButtonCaption, false,
    FMenuShowSheetPropertiesEventSink, SheetPropertiesHandler, '');

  AppendInMenuButton(Menu, '', tagMenuCommonOptions, kriCommonOptionsCaption,
    0, msoButtonCaption, false, FMenuCommonOptionsEventSink, CommonOptionsHandler, '');

  AppendInMenuButton(Menu, '', tagMenuSheetInfo, kriSheetInfo,
    0, msoButtonCaption, false, FMenuSheetInfoEventSink, SheetInfoHandler, '');

  // история листа - меню
  AppendInMenuButton(Menu, '', tagMenuSheetHistory, kriSheetHistory, 0,
    msoButtonIconAndCaption, true, FMenuShowSheetHistoryEventSink,
    SheetHistoryHandler, '');

  // управление константами - меню
  AppendInMenuButton(Menu, '', tagMenuShowConsts, kriShowConstsCaption, 0,
    msoButtonCaption, false, FMenuShowConstsEventSink, ShowConstsHandler, '');

  // управление параметрами - меню
  AppendInMenuButton(Menu, '', tagMenuShowParams, kriShowParamsCaption, 0,
    msoButtonCaption, true, FMenuShowParamsEventSink, ShowParamsHandler, '');

  AppendInMenuButton(Menu, '', tagMenuReplication, kriMenuReplication, 0,
    msoButtonCaption, false, FMenuReplicationEventSink, ReplicationHandler, '');

  AppendInMenuButton(Menu, '', tagMenuDataCollectionForm, kriDataCollectionForm,
    0, msoButtonCaption, false, FMenuDataCollectionFormEventSink, DataCollectionFormHandler, '');

  // показать форму копирования - меню
  AppendInMenuButton(Menu, '', tagMenuButtonShowCopyForm, kriCopyForm,
    0, msoButtonCaption, false, FMenuShowCopyFormEventSink, CopyFormHandler, '');

  // панель разрывов - меню
  AppendInMenuButton(Menu, '', tagMenuSplitterPad, kriBreak, 0, msoButtonCaption,
    true, FMenuBarShowSplitterPadEventSink, SplitterPadHandler, '');

  // Переключение режима работы - тулбар
  {$WARNINGS OFF}
  AppendInToolBarButton(ToolBar, '', tagToolButtonWorkMode,
    kriInDataMode, 212, msoButtonIcon, true, FToolbarWorkModeEventSink,
    WorkModeHandler, '').Set_State(msoButtonDown);
  {$WARNINGS ON}

  // пометить ячейку "пустой" - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonMarkEmpty, kriMarkEmpty,
    47, msoButtonIcon, false, FToolBarMarkEmptyEventSink, MarkEmptyHandler, '');
  // пометить ячейку "пустой" - меню
  AppendInMenuButton(Menu, '', tagMenuMarkEmpty, kriMarkEmpty, 47,
    msoButtonIconAndCaption, false, FMenuMarkEmptyEventSink, MarkEmptyHandler, '');

  // переслать данные - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonSendData, kriToolbarSendData, 527,
    msoButtonIcon, false, FToolBarSendDataEventSink, SendDataHandler, '');
  // переслать данные - меню
  AppendInMenuButton(Menu, '', tagMenuSendData, kriMenuSendData, 527,
    msoButtonIconAndCaption, false, FMenuSendDataEventSink, SendDataOptionalHandler, '');

  // обновить - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonRefresh, kriRefreshCaption,
    1020, msoButtonIcon, true, FToolBarRefreshEventSink, RefreshHandler, '');
  // обновить - меню
  AppendInMenuButton(Menu, '', tagMenuRefresh, kriRefreshCaption, 1020,
    msoButtonIconAndCaption, true, FMenuRefreshEventSink, RefreshHandler , '');

  // менеджер отдельных ячеек - меню
  AppendInMenuButton(Menu, '', tagMenuSingleCellsManager, kriSingleCellsManager,
    162{628}, msoButtonIconAndCaption, false, FMenuSingleCellsManagerEventSink,
    SingleCellsManagerHandler, '');

  // менеджер отдельных ячеек - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonSingleCellsManager,
    kriSingleCellsManager, 162{628}, msoButtonIcon, false,
    FToolbarSingleCellsManagerEventSink, SingleCellsManagerHandler, '');

  {редактор свойств компонентов листа - меню}
  AppendInMenuButton(Menu, '', tagMenuComponentEditor, kriComponentEditor,
    855, msoButtonIconAndCaption, false, FMenuComponentEditorEventSink,
    ComponentEditorHandler, '');

  {редактор свойств компонентов листа - тулбар}
  AppendInToolBarButton(ToolBar, '', tagToolButtonComponentEditor,
    kriComponentEditor, 855, msoButtonIcon, false,
    FToolBarComponentEditorEventSink, ComponentEditorHandler, '');


  // подменю "добавить" - тулбар
  AppendInToolBarSubMenu(ToolBar, '', tagToolButtonAdd, kriAdd, 0, true);
  {----------------------------- Наполнение ----------------------------}

  AppendInToolBarButton(ToolBar, tagToolButtonAdd, tagToolButtonConst, kriConst,
    288, msoButtonIconAndCaption, false, FToolBarAddConstsEventSink,
    AddConstsHandler, '');

  AppendInToolBarButton(ToolBar, tagToolButtonAdd, tagToolButtonSingleCell,
    kriSingleCell, 499, msoButtonIconAndCaption, true, FToolBarSingleCellEditorEventSink,
    SingleCellEditorHandler, '');

  AppendInToolBarButton(ToolBar, tagToolButtonAdd, tagToolButtonFilters,
    kriFilters, 0, msoButtonIconAndCaption, false, FToolBarConstructorWizardAddFilterEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddFilter, '', ''));

  AppendInToolBarButton(ToolBar, tagToolButtonAdd, tagToolButtonColumns,
    kriColumns, 0, msoButtonIconAndCaption, false, FToolBarConstructorWizardAddColumnEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddColumn, '',''));

  AppendInToolBarButton(ToolBar, tagToolButtonAdd, tagToolButtonRows, kriRows,
    0, msoButtonIconAndCaption, false, FToolBarConstructorWizardAddRowEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddRow, '', ''));

  {-------------  под-под меню "добавить" "показатель" (тулбар)----------------}
  AppendInToolBarSubMenu(ToolBar, tagToolButtonAdd, tagToolButtonTotals,
    kriTotals, 0, false);

  AppendInToolBarButton(ToolBar, tagToolButtonTotals, tagToolButtonTotalConst,
    kriTotalConst, 0, msoButtonIconAndCaption, false,
    FToolBarConstructorWizardAddConstTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddConstTotal, '', ''));

  AppendInToolBarButton(ToolBar, tagToolButtonTotals, tagToolButtonResult,
    kriResult, 0, msoButtonIconAndCaption, false,
    FToolBarConstructorWizardAddResultTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddResultTotal, '', ''));

  AppendInToolBarButton(ToolBar, tagToolButtonTotals, tagToolButtonFree,
    kriFree, 0, msoButtonIconAndCaption, false,
    FToolBarConstructorWizardAddFreeTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddFreeTotal, '', ''));

  AppendInToolBarButton(ToolBar, tagToolButtonTotals, tagToolButtonMeasure,
    kriMeasure, 0, msoButtonIconAndCaption, false,
    FToolBarConstructorWizardAddMeasureTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddMeasureTotal, '', ''));
  {------------------- Конец подменю "добавить" (тулбар) ----------------------}

  //Подменю "добавить" (меню)
  AppendInMenuSubMenu(Menu, '', tagMenuButtonAdd, kriAdd, 0, false);
  {----------------------------- Наполнение ----------------------------}

  AppendInMenuButton(Menu, tagMenuButtonAdd, tagMenuButtonConst, kriConst,
    288, msoButtonIconAndCaption, false, FMenuAddConstsEventSink, AddConstsHandler, '');

  AppendInMenuButton(Menu, tagMenuButtonAdd, tagMenuButtonSingleCell,
    kriSingleCell, 499, msoButtonIconAndCaption, true, FMenuSingleCellEditorEventSink,
    SingleCellEditorHandler, '');

  AppendInMenuButton(Menu, tagMenuButtonAdd, tagMenuButtonFilters, kriFilters,
    0, msoButtonIconAndCaption, false, FMenuConstructorWizardAddFilterEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddFilter, '', ''));

  AppendInMenuButton(Menu, tagMenuButtonAdd, tagMenuButtonColumns,
    kriColumns, 0, msoButtonIconAndCaption, false, FMenuConstructorWizardAddColumnEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddColumn, '',''));

  AppendInMenuButton(Menu, tagMenuButtonAdd, tagMenuButtonRows, kriRows, 0,
    msoButtonIconAndCaption, false, FMenuConstructorWizardAddRowEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddRow, '', ''));
  {-------------  под-под меню "добавить" "показатель" (меню)----------------}
  AppendInMenuSubMenu(Menu, tagMenuButtonAdd, tagMenuButtonTotals, kriTotals,
    0, false);

  AppendInMenuButton(Menu, tagMenuButtonTotals, tagMenuButtonTotalConst,
    kriTotalConst, 0, msoButtonIconAndCaption, false,
    FMenuConstructorWizardAddConstTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddConstTotal, '', ''));

  AppendInMenuButton(Menu, tagMenuButtonTotals, tagMenuButtonResult, kriResult,
    0, msoButtonIconAndCaption, false, FMenuConstructorWizardAddResultTotalEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddResultTotal, '', ''));

  AppendInMenuButton(Menu, tagMenuButtonTotals, tagmenuButtonFree, kriFree, 0,
    msoButtonIconAndCaption, false, FMenuConstructorWizardAddFreeTotalEventSink,
    ConstructorWizardHandler, CreateAdditionalInfo(wrmAddFreeTotal, '', ''));

  AppendInMenuButton(Menu, tagMenuButtonTotals, tagMenuButtonMeasure,
    kriMeasure, 0, msoButtonIconAndCaption, false,
    FMenuConstructorWizardAddMeasureTotalEventSink, ConstructorWizardHandler,
    CreateAdditionalInfo(wrmAddMeasureTotal, '', ''));
  {------------------- Конец подменю "добавить" (меню) ----------------------}

  // мастер - тулбар
  AppendInToolBarButton(ToolBar, '', tagToolButtonConstructorWizard,
    kriConstructorWizardCaption, 630, msoButtonIcon, false,
    FToolBarConstructorWizardEventSink, ConstructorWizardHandler, '');
  // мастер - меню
  AppendInMenuButton(Menu, '', tagMenuConstructorWizard,
    kriConstructorWizardCaption, 630, msoButtonIconAndCaption, true,
    FMenuConstructorWizardEventSink, ConstructorWizardHandler, '');

  //Подключение - меню
  AppendInMenuButton(Menu, '', tagMenuConnection, kriConnectionCaption, 0,
    msoButtonCaption, false, FMenuConnectionEventSink, ConnectionHandler, '');

  {------------------------ Наполнение Статус бара --------------------------}
  FStatusBarOnlineMode := AppendEditInToolBar(StatusBar, tagStatusBarOnlineMode,
    kriOnlineMode, '', 172, msoComboLabel, false, true);
  FStatusBarContext := AppendEditInToolBar(StatusBar, tagStatusBarContext,
    kriStatusBarContext, '', 92, msoComboLabel, false, false);
  FStatusBarTaskID := AppendEditInToolBar(StatusBar, tagStatusBarTaskID,
    kriTaskID, '', 92, msoComboLabel, false, true);
  FStatusBarRefreshDate := AppendEditInToolBar(StatusBar, tagStatusBarRefreshDate,
    kriRefreshDate, '', 240, msoComboLabel, false, false);
  {------------------------ Конец Статус бара -------------------------------}
end;

function TFMExcelAddinInterface.CreateAdditionalInfo(
  WizardRunMode: TWizardRunMode; ID, Instance: string): widestring;
begin
  result := snNamePrefix + snSeparator + IntToStr(Ord(WizardRunMode)) +
    snSeparator + ID + snSeparator + Instance;
end;

procedure TFMExcelAddinInterface.SetStatusBarInfo(Where: TStatusBarInformer; What: string);
begin
  case Where of
    sbiRefreshDate: SetTextInComboBox(FStatusBarRefreshDate, What);
    sbiTaskId: SetTextInComboBox(FStatusBarTaskID, What);
    sbiContext: SetTextInComboBox(FStatusBarContext, What);
    sbiOnlineMode: SetTextInComboBox(FStatusBarOnlineMode, What);
  end;
end;

function TFMExcelAddinInterface.ParseAdditionalInfo(AdditionalInfo: string;
  out WizardRunMode: TWizardRunMode; out Param1, Param2: string): boolean;
begin
  result := IsNameOurs(AdditionalInfo);
  if result then
  begin
    CutPart(AdditionalInfo, snSeparator);
    WizardRunMode := TWizardRunMode(StrToInt(CutPart(AdditionalInfo, snSeparator)));
    Param1 := CutPart(AdditionalInfo, snSeparator);
    Param2 := CutPart(AdditionalInfo, snSeparator);
  end;
end;

function TFMExcelAddinInterface.GetCommandBarControl(OnToolbar: boolean;
  Tag: string): CommandBarControl;
begin
  result := nil;
  if OnToolbar then
  begin
    if not Assigned(ToolBar) then
      exit;
    result := ToolBar.FindControl(EmptyParam, EmptyParam,
      tagToolButtonAdd, EmptyParam, msoFalse);
  end
  else
  begin
    if not Assigned(Menu) then
      exit;
    result := (Menu as CommandBarPopup).CommandBar.FindControl(
      EmptyParam, EmptyParam, tagMenuButtonAdd, EmptyParam, EmptyParam);
  end;
end;

procedure TFMExcelAddinInterface.UpdateAppendSubmenuItem(OnToolbar: boolean; Tag: string; State: boolean);
var
  PopupMenuControl: CommandBarPopup;
  ComBtn: CommandBarControl;
begin
  PopupMenuControl := GetCommandBarControl(OnToolbar, Tag) as CommandBarPopup;
  if Assigned(PopupMenuControl) then
  begin
    ComBtn := PopupMenuControl.CommandBar.FindControl(EmptyParam, EmptyParam,
      Tag, EmptyParam, EmptyParam);
    if Assigned(ComBtn) then
      ComBtn.Set_Enabled(State);
  end;
end;

function TFMExcelAddinInterface.AppendPopupMenuItem(ContextMenu: CommandBar;
  NameCaption, AdditionalInfo: widestring; Handler: TOnCommandButtonClick;
  Tag: widestring; BeginGroup: boolean; StyleCaption: MsoButtonStyle;
  Face: integer): CommandBarButton;
var
  Button: CommandBarButton;
  // контекстное меню
  FContextMenuEventSink: TCommandButtonEventSink;
begin
  try
    Button := (ContextMenu.Controls.Add(msoControlButton, EmptyParam, EmptyParam,
      EmptyParam, true) as CommandBarButton);
  except
    result := nil;
  end;
  if not Assigned(Button) then
    exit;
  with Button do
  begin
    Set_Caption(NameCaption);
    Set_Tag(Tag);
    Set_Style(StyleCaption);
    Set_BeginGroup(BeginGroup);
    Set_HelpFile(AdditionalInfo);
    Set_FaceID(Face);
  end;
  FContextMenuEventSink := TCommandButtonEventSink.Create;
  FContextMenuEventSink.OnClick := Handler;
  FContextMenuEventSink.Connect(Button);
  result := Button;
end;

procedure TFMExcelAddinInterface.PopupMenuRow(ContextMenu: CommandBar; Id: string);
var
  AdditionalInfo: string;
  Element: TSheetElement;
begin
  with PlaningSheet.Rows do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditRow, ID, '');

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, true,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {редактирование}
  AppendPopupMenuItem(ContextMenu, pmnRowEdit, AdditionalInfo,
    ConstructorWizardHandler, tagPopupMenuRowEdit, false,
    msoButtonIconAndCaption,
    534).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnRowDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuRowDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Rows.MayBeEdited and PlaningSheet.Online);

  {перемещение в столбцы}
  AdditionalInfo := CreateAdditionalInfo(wrmEditRow,
    ID, sntColumnDimension);
  AppendPopupMenuItem(ContextMenu, pmnMoveToColumns, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToAxis, false,
    msoButtonIconAndCaption,
    297).Set_Enabled(
    PlaningSheet.Rows.MayBeEdited and PlaningSheet.Columns.MayBeEdited
    and PlaningSheet.Online);

  {перемещение в фильтры}
  AdditionalInfo := CreateAdditionalInfo(wrmEditRow, ID, sntFilter);
  AppendPopupMenuItem(ContextMenu, pmnMoveToFilters, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToFilters, false,
    msoButtonIconAndCaption,
    628).Set_Enabled(
    PlaningSheet.Rows.MayBeEdited and PlaningSheet.Filters.MayBeEdited
    and PlaningSheet.Online);
end;

procedure TFMExcelAddinInterface.PopupMenuColumn(ContextMenu: CommandBar; Id: string);
var
  AdditionalInfo: string;
  Element: TSheetElement;
begin
  with PlaningSheet.Columns do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditColumn, ID, '');

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, true,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {редактирование}
  AppendPopupMenuItem(ContextMenu, pmnColumnEdit, AdditionalInfo,
    ConstructorWizardHandler, tagPopupMenuColumnEdit, false,
    msoButtonIconAndCaption,
    534).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnColumnDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuColumnDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Columns.MayBeEdited and PlaningSheet.Online);

  {перемещение в строки}
  AdditionalInfo := CreateAdditionalInfo(wrmEditColumn,
    ID, sntRowDimension);
  AppendPopupMenuItem(ContextMenu, pmnMoveToRows, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToAxis, false,
    msoButtonIconAndCaption,
    296).Set_Enabled(
    PlaningSheet.Rows.MayBeEdited and PlaningSheet.Columns.MayBeEdited
    and PlaningSheet.Online);

  {перемещение в фильтры}
  AdditionalInfo := CreateAdditionalInfo(wrmEditColumn,
    ID, sntFilter);
  AppendPopupMenuItem(ContextMenu, pmnMoveToFilters, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToFilters, false,
    msoButtonIconAndCaption,
    628).Set_Enabled(
    PlaningSheet.Columns.MayBeEdited and PlaningSheet.Filters.MayBeEdited
    and PlaningSheet.Online);
end;

procedure TFMExcelAddinInterface.PopupMenuMeasure(ContextMenu: CommandBar; Id, Instance: string);
var
  AdditionalInfo: string;
  Total: TSheetTotalInterface;
  tmpBool: boolean;
begin
  with PlaningSheet.Totals do
    Total := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);

  {индивидуальное обновление}
  AppendPopupMenuItem(ContextMenu, pmnTotalRefresh, AdditionalInfo,
    OneTotalRefreshHandler, tagPopupMenuRefreshOneTotal, true,
    msoButtonIconAndCaption,
    1977).Set_Enabled(PlaningSheet.Online);

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, false,
    msoButtonIconAndCaption,
    855).Set_Enabled(Total.MayBeEdited and PlaningSheet.Online);

  {Смена типа на результат}
  tmpBool := false;
  if Assigned(Total.Measure) then
    tmpBool := not Total.Measure.IsCalculated and Total.OfPrimaryProvider;

  AppendPopupMenuItem(ContextMenu, pmnSwitchToResult, AdditionalInfo,
    SwitchTotalTypeHandler, tagPopupMenuSwitchTotalType, false,
    msoButtonIconAndCaption,
    0).Set_Enabled(Total.MayBeEdited and PlaningSheet.Online and tmpBool);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnTotalMeasureDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuTotalMeasureDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Totals.MayBeEdited and PlaningSheet.Online);

  {Скрытие}
  AppendPopupMenuItem(ContextMenu, pmnHideTotalColumns, AdditionalInfo,
    HideTotalColumnsHandler, tagPopupMenuHideTotalColumns, false,
    msoButtonIconAndCaption, 0).Set_Enabled(true);
end;

procedure TFMExcelAddinInterface.PopupMenuFree(ContextMenu: CommandBar; ObjType, Id, Instance: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
begin
  with PlaningSheet.Totals do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, true,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnTotalFreeDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuTotalFreeDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Totals.MayBeEdited and PlaningSheet.Online);

  {установить типовую формулу}
  if (ObjType = sntTotalFree) then
  begin
    AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);
    if IsExistsFormula(Host.ActiveCell) then
      AppendPopupMenuItem(ContextMenu, pmpSetTypeFormula, AdditionalInfo,
        SetTypeFormulaHandler, tagPopupMenuSetTypeFormula, false,
        msoButtonIconAndCaption,
        0).Set_Enabled(PlaningSheet.Online);
  end;

  {Скрытие}
  AppendPopupMenuItem(ContextMenu, pmnHideTotalColumns, AdditionalInfo,
    HideTotalColumnsHandler, tagPopupMenuHideTotalColumns, false,
    msoButtonIconAndCaption, 0).Set_Enabled(true);
end;

procedure TFMExcelAddinInterface.PopupMenuResult(ContextMenu: CommandBar; ObjType, Id, Instance: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
begin
  with PlaningSheet.Totals do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);

  {индивидуальное обновление}
  AppendPopupMenuItem(ContextMenu, pmnTotalRefresh, AdditionalInfo,
    OneTotalRefreshHandler, tagPopupMenuRefreshOneTotal, true,
    msoButtonIconAndCaption,
    1977).Set_Enabled(PlaningSheet.Online);

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, false,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {Смена типа на меру}
  AppendPopupMenuItem(ContextMenu, pmnSwitchToMeasure, AdditionalInfo,
    SwitchTotalTypeHandler, tagPopupMenuSwitchTotalType, false,
    msoButtonIconAndCaption,
    0).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnTotalResultDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuTotalResultDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Totals.MayBeEdited and PlaningSheet.Online);

  if ObjType = sntTotalResult then
  begin
    AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);

    {очистка ячейки}
    AppendPopupMenuItem(ContextMenu, pmnTotalResultMarkEmpty, AdditionalInfo,
      MarkEmptyHandler, tagPopupMenuTotalResultMarkEmpty, false,
      msoButtonIconAndCaption, 47);

    {установить типовую формулу}
    if IsExistsFormula(Host.ActiveCell) then
      AppendPopupMenuItem(ContextMenu, pmpSetTypeFormula, AdditionalInfo,
        SetTypeFormulaHandler, tagPopupMenuSetTypeFormula, false,
        msoButtonIconAndCaption,
        0).Set_Enabled(PlaningSheet.Online);
  end;

  {Скрытие}
  AppendPopupMenuItem(ContextMenu, pmnHideTotalColumns, AdditionalInfo,
    HideTotalColumnsHandler, tagPopupMenuHideTotalColumns, false,
    msoButtonIconAndCaption, 0).Set_Enabled(true);
end;

procedure TFMExcelAddinInterface.PopupMenuConst(ContextMenu: CommandBar; Id, Instance: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
begin
  with PlaningSheet.Totals do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditTotal, ID, Instance);

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, true,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnTotalConstDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuTotalConstDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Totals.MayBeEdited and PlaningSheet.Online);

  {Скрытие}
  AppendPopupMenuItem(ContextMenu, pmnHideTotalColumns, AdditionalInfo,
    HideTotalColumnsHandler, tagPopupMenuHideTotalColumns, false,
    msoButtonIconAndCaption, 0).Set_Enabled(true);
end;

procedure TFMExcelAddinInterface.PopupMenuFilter(ContextMenu: CommandBar; Id, Instance: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
begin
  with PlaningSheet.Filters do
    Element := Items[FindById(Id)];
  AdditionalInfo := CreateAdditionalInfo(wrmEditFilter, ID, Instance);

  {свойства}
  AppendPopupMenuItem(ContextMenu, pmnElementProperties, AdditionalInfo,
    ComponentEditorHandler, tagPopupMenuElementProperties, true,
    msoButtonIconAndCaption,
    855).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {редактирование}
  if PlaningSheet.Online then
    AppendPopupMenuItem(ContextMenu, pmnFilterEdit, AdditionalInfo,
      ConstructorWizardHandler, tagPopupMenuFilterEdit, false,
      msoButtonIconAndCaption,
      534).Set_Enabled(Element.MayBeEdited)
  else
    AppendPopupMenuItem(ContextMenu, pmnFilterEdit, AdditionalInfo,
      OfflineFiltersEditorHandler, tagPopupMenuFilterEdit, false,
      msoButtonIconAndCaption,
      534).Set_Enabled(Element.MayBeEdited);

  {удаление}
  AppendPopupMenuItem(ContextMenu, pmnFilterDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuFilterDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.Filters.MayBeEdited and PlaningSheet.Online);

  {перемещение}
  AdditionalInfo := CreateAdditionalInfo(wrmEditFilter,
    ID, sntRowDimension);
  AppendPopupMenuItem(ContextMenu, pmnMoveToRows, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToAxis, false,
    msoButtonIconAndCaption,
    296).Set_Enabled(
    PlaningSheet.Filters.MayBeEdited and PlaningSheet.Rows.MayBeEdited
    and PlaningSheet.Online);

  {перемещение}
  AdditionalInfo := CreateAdditionalInfo(wrmEditFilter,
    ID, sntColumnDimension);
  AppendPopupMenuItem(ContextMenu, pmnMoveToColumns, AdditionalInfo,
    MoveElementHandler, tagPopupMenuMoveToAxis2, false,
    msoButtonIconAndCaption, 
    297).Set_Enabled(
    PlaningSheet.Filters.MayBeEdited and PlaningSheet.Columns.MayBeEdited
    and PlaningSheet.Online);
end;

procedure TFMExcelAddinInterface.PopupMenuSingleTotal(ContextMenu: CommandBar; ObjType, Id: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
  tmpBool: boolean;
begin
  with PlaningSheet.SingleCells do
    Element := Items[FindById(Id)];

  {обновление}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'refresh');
  AppendPopupMenuItem(ContextMenu, pmnSingleCellRefresh, AdditionalInfo,
    SingleCellEditorHandler, tagPopupMenuSingleCellRefresh,
    true, msoButtonIconAndCaption, 1977).Set_Enabled(PlaningSheet.Online);

  {редактирование}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'edit');
  AppendPopupMenuItem(ContextMenu, pmnSingleCellEdit, AdditionalInfo,
    SingleCellEditorHandler, tagPopupMenuSingleCellEdit,
    false, msoButtonIconAndCaption, 499).Set_Enabled(Element.MayBeEdited);

  {Смена типа показателя}
  if ObjType = sntSingleCellMeasure then
  begin
    tmpBool := false;
    if Assigned(TSheetBasicTotal(Element).Measure) then
      tmpBool := not TSheetBasicTotal(Element).Measure.IsCalculated and Element.OfPrimaryProvider;

    AppendPopupMenuItem(ContextMenu, pmnSwitchToResult, AdditionalInfo,
      SwitchTotalTypeHandler, tagPopupMenuSwitchTotalType, false,
      msoButtonIconAndCaption,
      0).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online and tmpBool);
  end
  else
    AppendPopupMenuItem(ContextMenu, pmnSwitchToMeasure, AdditionalInfo,
      SwitchTotalTypeHandler, tagPopupMenuSwitchTotalType, false,
      msoButtonIconAndCaption,
      0).Set_Enabled(Element.MayBeEdited and PlaningSheet.Online);

  {удаление}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'delete');
  AppendPopupMenuItem(ContextMenu, pmnSingleCellDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuSingleCellDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.SingleCells.MayBeEdited and PlaningSheet.Online);
end;

procedure TFMExcelAddinInterface.PopupMenuSingleConst(ContextMenu: CommandBar; Id: string);
var
  Element: TSheetElement;
  AdditionalInfo: string;
begin
  with PlaningSheet.SingleCells do
    Element := Items[FindById(Id)];

  {обновление}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'refresh');
  AppendPopupMenuItem(ContextMenu, pmnCellConstRefresh, AdditionalInfo,
    RefreshConstHandler, tagPopupMenuCellConstRefresh, true,
    msoButtonIconAndCaption,
    1977).Set_Enabled(PlaningSheet.Online);

  {редактирование}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'edit');
  AppendPopupMenuItem(ContextMenu, pmnCellConstEdit, AdditionalInfo,
    EditConstHandler, tagPopupMenuCellConstEdit, false,
    msoButtonIconAndCaption,
    499).Set_Enabled(Element.MayBeEdited);

  {удаление}
  AdditionalInfo := CreateAdditionalInfo(wrmNone, Id, 'delete');
  AppendPopupMenuItem(ContextMenu, pmnCellConstDel, AdditionalInfo,
    DeleteElementHandler, tagPopupMenuCellConstDel, false,
    msoButtonIconAndCaption,
    536).Set_Enabled(PlaningSheet.SingleCells.MayBeEdited and PlaningSheet.Online);
end;

procedure TFMExcelAddinInterface.PopupMenuSingleNew(ContextMenu: CommandBar);
begin
  AppendPopupMenuItem(ContextMenu, pmnSingleCellNew, '', SingleCellEditorHandler,
    tagPopupMenuSingleCellNew, false,
    msoButtonIconAndCaption,
    499).Set_Enabled(PlaningSheet.SingleCells.MayBeEdited);

  AppendPopupMenuItem(ContextMenu, pmnCellConstNew, '', AddConstsHandler,
    tagPopupMenuCellConstNew, false,
    msoButtonIconAndCaption,
    288).Set_Enabled(PlaningSheet.SingleCells.MayBeEdited);
end;

procedure TFMExcelAddinInterface.DetectName(ContextMenu: CommandBar; ExcelName: Name;
  var ProperPlace: boolean; var AlreadyPlaced: boolean);
var
  Name: string;
  ObjType, ID, Instance, Fmt: string;
  Params: TStringList;
begin
  // в Name нужно отсекать имя листа, по восклицательный знак
  Name := ExcelName.Name_;
  Name := GetShortSheetName(Name);

  if not ParseExcelName(Name, Params) then
    exit;
  try
    ObjType := Params[0];
    try
      ID := Params[1];
    except
      ID := '';
    end;
    try
      Instance := Params[2];
    except
      Instance := '';
    end;
    try
      Fmt := Params[3];
    except
      Fmt := '';
    end;

    //обработка клика по заголовку уровня измерения
    if (ObjType = sntRowLevelTitle) or (ObjType = sntColumnLevelTitle) then
    begin
      //здесь добавляем пункты меню непосредственно для уровня
      //...........
      //а здесь подменяем параметры на измерение
      ObjType := Instance;
      ID := Fmt;
    end;

    if ObjType = sntTable then
      AppendPopupMenuItem(ContextMenu, pmnTableRefresh, '',
        RefreshHandler2, tagPopupMenuTableRefresh, true,
        msoButtonIconAndCaption,
        1977).Set_Enabled(PlaningSheet.Online);

    {элемент строк}
    if (ObjType = sntRowDimension) or (ObjType = sntRowLevelTitle) then
      PopupMenuRow(ContextMenu, Id);

    {элемент столбцов}
    if (ObjType = sntColumnDimension) or (ObjType = sntColumnLevelTitle) then
      PopupMenuColumn(ContextMenu, Id);

    {показатель-мера}
    if ((ObjType = sntTotalMeasure) or (ObjType = sntTotalMeasureTitle)) and (Instance <> '') then
      PopupMenuMeasure(ContextMenu, Id, Instance);

    {свободный показатель}
    if ((ObjType = sntTotalFree) or (ObjType = sntTotalFreeTitle)) and (Instance <> '') then
      PopupMenuFree(ContextMenu, ObjType, Id, Instance);

    {показатель-результат}
    if ((ObjType = sntTotalResult) or (ObjType = sntTotalResultTitle)) and (Instance <> '') then
      PopupMenuResult(ContextMenu, ObjType, Id, Instance);

    {показатель-константа}
    if ((ObjType = sntTotalConst) or (ObjType = sntTotalConstTitle)) and (Instance <> '') then
      PopupMenuConst(ContextMenu, Id, Instance);

    if (ObjType = sntFilter) then
      PopupMenuFilter(ContextMenu, Id, Instance);

    {отдельная-мера или результат}
    if (ObjType = sntSingleCellMeasure) or (ObjType = sntSingleCellResult) then
    begin
      AlreadyPlaced := true;
      PopupMenuSingleTotal(ContextMenu, ObjType, Id);
    end;

    {отдельная-константа}
    if (ObjType = sntSingleCellConst) then
    begin
      AlreadyPlaced := true;
      PopupMenuSingleConst(ContextMenu, Id);
    end;

    {Только для области строк (без заголовков)}
    if ObjType = sntRows then
    begin
      {Добавление пустой строки}
      AppendPopupMenuItem(ContextMenu, pmnInsertNewLine, '',
        InsertNewLineHandler, tagPopupMenuInsertNewLine, true,
        msoButtonIconAndCaption,
        0).Set_Enabled((PlaningSheet.Rows.MayBeEdited and PlaningSheet.Online)
          or (not PlaningSheet.Online and PlaningSheet.Rows.HideEmpty));
    end;

    if ((ObjType = sntFiltersBreak) or (ObjType = sntUnitMarkerBreak)
      or (ObjType = sntColumnTitlesBreak) or (ObjType = sntColumnsBreak)
      or (ObjType = sntRowTitlesBreak) or (ObjType = sntRowsBreak)
      or (ObjType = sntTotalFree) or (ObjType = sntTotalResult)
      or (ObjType = sntTotalMeasure)) then
      ProperPlace := true;

  finally
    FreeAndNil(Params);
  end;
end;

procedure TFMExcelAddinInterface.BeforeRightClick(const Sh: IDispatch;
  const Target: ExcelRange; var Cancel: WordBool);
var
  ExcelName: ExcelXP.Name;
  WSheet: ExcelWorkSheet;
  CountNames, i, BarIndex: integer;
  MayPlaceSingleCell, AlreadyPlaced, ProperPlace: boolean;
  ContextMenu: CommandBar;
begin
  WSheet := GetWorkSheet(Sh);
  if not Assigned(WSheet) then
    exit;

  for BarIndex := 1 to Host.CommandBars.Count do
  begin
    if AnsiUpperCase(Host.CommandBars.Item[BarIndex].Name) <> 'CELL' then
      continue;
    ContextMenu := Host.CommandBars.Item[BarIndex];
    // очищаем контекстное меню
    ContextMenu.Reset;

    if IsSequelSheet(WSheet) then
      exit;

    MayPlaceSingleCell := (Target.Rows.Count = 1) and (Target.Columns.Count = 1) and (Target.Areas.Count = 1);

    {В экселе 2007 невозможно включить закрепление областей на защищенном листе, используя меню.
    Дадим пользователям такую возможность.}
    AppendPopupMenuItem(ContextMenu,
      IIF(Host.ActiveWindow.FreezePanes, 'Снять закрепление областей', 'Закрепить области'), '',
      FreezePanesHandler, tagPopupMenuFreezePanes, true, msoButtonIconAndCaption, 0);

    { Лист может содержать какие-то имена, но не быть листом планирования.
      Такие листы расцениваем как пустые и обрабатываем соответственно.}
    if IsPlaningSheet(WSheet) then
      CountNames := WSheet.Names.Count
    else
      CountNames := 0;
        
    if CountNames = 0 then
    begin
      {на пустом листе можно разместить отдельную ячейку}
      if MayPlaceSingleCell and PlaningSheet.Online then
        PopupMenuSingleNew(ContextMenu);
      continue;
    end;

    ProperPlace := false;
    AlreadyPlaced := false;
    for i := 1 to CountNames do
    begin
      ExcelName := WSheet.Names.Item(i, EmptyParam, EmptyParam);

      if NameWithoutRange(ExcelName) then
        continue;

      { если диапазон, по которому кликнули принадлежит нужному имени,
          добавляем в контекстное меню...}
      if IsNestedRanges(Target, ExcelName.RefersToRange) then
        DetectName(ContextMenu, ExcelName, ProperPlace, AlreadyPlaced);
    end;

    {справа от таблицы отдельным показателям не место}
    if Assigned(GetExtendedTableRange(WSheet)) then
      if IsNestedRanges(Target, GetExtendedTableRange(WSheet)) and not TableSelected(WSheet, Target) then
        MayPlaceSingleCell := false;

    if MayPlaceSingleCell and ProperPlace and not AlreadyPlaced and PlaningSheet.Online then
      PopupMenuSingleNew(ContextMenu);

    {добавляем пункт контекстного меню "Изменить стиль"}
    AppendPopupMenuItem(ContextMenu, pmnEditStyle,'', EditStyleHandler, tagPopupMenuStyleEdit,
      false, msoButtonIconAndCaption, 0);
  end;
end;

procedure TFMExcelAddinInterface.SetButtonsState(State, SecondaryAnalysis: boolean);

  function ResolveControlState(ControlTag: string): boolean;
  begin
    { В случае несовпадения версии листа и надстройки
      запрещаем большинство функций...}
    if SecondaryAnalysis and ((State and UpdateNeed) or
      (not UpdateNeed and (VersionRelation = svFuture))) then
    begin
      if (ControlTag = tagToolButtonSheetProperties)
        or ((ControlTag = tagToolButtonRefresh) and (VersionRelation <> svFuture))
        or (ControlTag= tagToolButtonShowCopyForm)
        or (ControlTag = tagMenuConnection)
        or (ControlTag = tagMenuSheetHistory)
        or (ControlTag = tagMenuSheetProperties)
        or ((ControlTag = tagMenuRefresh) and (VersionRelation <> svFuture))
        or (ControlTag = tagMenuButtonShowCopyForm) then
        result := true
      else
        result := false;
    end
    else
      if (ControlTag = tagToolButtonConstructorWizard)
        or (ControlTag = tagMenuConstructorWizard) then
        result := State and PlaningSheet.MayBeEdited
      else
        result := State;

    { ...а в автономном режиме вообще практически все}
    if result and not PlaningSheet.Online then
      if (ControlTag <> tagMenuButtonShowCopyForm)
        and (ControlTag <> tagToolButtonSendData)
        and (ControlTag <> tagMenuSendData)
        and (ControlTag <> tagToolButtonMarkEmpty)
        and (ControlTag <> tagMenuMarkEmpty)
        and (ControlTag <> tagMenuSheetHistory) then
        result := false;
  end;

var
  i, cnt: integer;
  CBar: CommandBarPopup;
  CurSheet: ExcelWorkSheet;
  ResultEnabled, Sequel: boolean;
  Control: CommandBarControl;
begin
  {В данном методе при установлении включенности наших кнопок так же учитывается и
  в каком состояние находится лист(проверяется на предмет устарения), если он
  требует обновления, то оставляем только те кнопки которые непосредственно и
  совершают этот процесс, а так же информационные}

  {Стандартная функция взятия листа GetWorkSheet. И всегда нужно пользоваться ей,
   потому что она четко проверяет тип листа и возвращает лист только в том случае,
   если мы с ним сможем работать (есть еще "диаграммы", "формы", "макросы".
   НО!!!! В случае использования этой функции здесь, под ExcelXP!!! происходит
   зацикливание обработчика обновления тулбара. Вероятнее всего, это ошибка Excel.
   Поэтому, для XP лист будем брать нестандартно, для всего остального GetWorkSheet.
   Внешне эффект такой: Под XP, на листах типа "Макрос" наши кнопки доступны, хоть
   с такими листами работать мы и не можем...}
  try
    CurSheet := Host.ActiveSheet as ExcelWorkSheet;
  except
    CurSheet := nil;
  end;

  if not Assigned(CurSheet) then
    State := false;
  Sequel := IsSequelSheet(CurSheet);
  if Assigned(ToolBar) then
  begin
    cnt := ToolBar.Controls.Count;
    for i := 1 to cnt do
      with ToolBar.Controls[i] do
        Set_Enabled(not Sequel and ResolveControlState(Tag));

    { на листах типа "отчет" запрещаем добавление показателей - результатов}
    if Assigned(Host.ActiveWorkbook) then
    begin
      ResultEnabled := (GetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, pspSheetType) <> '2');
      Control := ToolBar.FindControl(EmptyParam, EmptyParam, tagToolButtonResult, EmptyParam, true);
      if Assigned(Control) then
        Control.Set_Enabled(ResultEnabled);
    end;
  end;

  if Assigned(Menu) then
  begin
    CBar := (Menu as CommandBarPopup);
    cnt := CBar.Controls.Count;
    for i := 1 to cnt do
      with CBar.Controls[i] do
        Set_Enabled(not Sequel and ResolveControlState(Tag));

    { на листах типа "отчет" запрещаем добавление показателей - результатов}
    if Assigned(Host.ActiveWorkbook) then
    begin
      ResultEnabled := (GetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, pspSheetType) <> '2');
      Control := Host.CommandBars.ActiveMenuBar.FindControl(EmptyParam, EmptyParam, tagMenuButtonResult, EmptyParam, true);
      if Assigned(Control) then
        Control.Set_Enabled(ResultEnabled);
    end;  
  end;
end;

end.
