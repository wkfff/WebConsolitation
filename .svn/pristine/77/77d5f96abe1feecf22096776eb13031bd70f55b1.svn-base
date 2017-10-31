unit brs_OWCEventsHandler;

interface

uses windows, activex, classes, owc10_tlb, comobj, oleserver, adodb_tlb;

type
  TConnectionEventsHandler = class (TObject, IUnknown, ConnectionEventsVt)
  private
    fEventSource : IUnknown;
    fConnection : integer;
  protected
    // connection events
    function  InfoMessage(const pError: Error; var adStatus: EventStatusEnum;
                          const pConnection: _Connection): HResult; stdcall;
    function  BeginTransComplete(TransactionLevel: Integer; const pError: Error;
                                 var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;
    function  CommitTransComplete(const pError: Error; var adStatus: EventStatusEnum;
                                  const pConnection: _Connection): HResult; stdcall;
    function  RollbackTransComplete(const pError: Error; var adStatus: EventStatusEnum;
                                    const pConnection: _Connection): HResult; stdcall;
    function  WillExecute(var Source: WideString; var CursorType: CursorTypeEnum;
                          var LockType: LockTypeEnum; var Options: Integer;
                          var adStatus: EventStatusEnum; const pCommand: _Command;
                          const pRecordset: _Recordset; const pConnection: _Connection): HResult; stdcall;
    function  ExecuteComplete(RecordsAffected: Integer; const pError: Error;
                              var adStatus: EventStatusEnum; const pCommand: _Command;
                              const pRecordset: _Recordset; const pConnection: _Connection): HResult; stdcall;
    function  WillConnect(var ConnectionString: WideString; var UserID: WideString;
                          var Password: WideString; var Options: Integer;
                          var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;
    function  ConnectComplete(const pError: Error; var adStatus: EventStatusEnum;
                              const pConnection: _Connection): HResult; stdcall;
    function  Disconnect(var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;

    {IUnknown}
    function QueryInterface(const IID: TGUID; out Obj): HResult; stdcall;
    function _AddRef: Integer; stdcall;
    function _Release: Integer; stdcall;
    //
    procedure fSetEventSource(const Val : IUnknown);
  public
      property EventSource : IUnknown read fEventSource write fSetEventSource;
//    destructor Destroy; override;
  end;


  // предок для обёрток-обработчиков событий OWC, реализует базовую функциональность
  // IUnknown и IDispatch
  TOWCEventsHandler = class (TObject, IUnknown, IDispatch)
  private
    fEventSource : IUnknown;
    fConnection : integer;
  protected
    {IUnknown}
    function QueryInterface(const IID: TGUID; out Obj): HResult; stdcall;
    function _AddRef: Integer; stdcall;
    function _Release: Integer; stdcall;
    {IDispatch}
    function GetTypeInfoCount(out Count: Integer): HResult; stdcall;
    function GetTypeInfo(Index, LocaleID: Integer; out TypeInfo): HResult; stdcall;
    function GetIDsOfNames(const IID: TGUID; Names: Pointer;
      NameCount, LocaleID: Integer; DispIDs: Pointer): HResult; stdcall;
    function Invoke(DispID: Integer; const IID: TGUID; LocaleID: Integer;
      Flags: Word; var Params; VarResult, ExcepInfo, ArgErr: Pointer): HResult; virtual; stdcall;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); virtual;
     // объект, события которого будем обрабатывать, процедура перекрывается в потомках
     // для указания конкретного интерфейса
     procedure fSetEventSource(const Val : IUnknown); virtual;
  published
    destructor Destroy; override;
    property EventSource : IUnknown read fEventSource write fSetEventSource;
  end;

    // прототипы функций-обработчиков событий
    // ... IChartEvents
    TOnDataSetChange        = procedure of object;
    TOnDblClick             = procedure of object;
    TOnClick                = procedure of object;
    TOnKeyDown              = procedure(KeyCode: Integer; Shift: Integer) of object;
    TOnKeyUp                = procedure(KeyCode: Integer; Shift: Integer) of object;
    TOnKeyPress             = procedure(KeyAscii: Integer) of object;
    TOnBeforeKeyDown        = procedure(KeyCode: Integer; Shift: Integer; const Cancel: ByRef) of object;
    TOnBeforeKeyUp          = procedure(KeyCode: Integer; Shift: Integer; const Cancel: ByRef) of object;
    TOnBeforeKeyPress       = procedure(KeyAscii: Integer; const Cancel: ByRef) of object;
    TOnMouseDown            = procedure(Button: Integer; Shift: Integer; x: Integer; y: Integer) of object;
    TOnMouseMove            = procedure(Button: Integer; Shift: Integer; x: Integer; y: Integer) of object;
    TOnMouseUp              = procedure(Button: Integer; Shift: Integer; x: Integer; y: Integer) of object;
    TOnMouseWheel           = procedure(Page: WordBool; Count: Integer) of object;
    TOnSelectionChange      = procedure of object;
    TOnBeforeScreenTip      = procedure(const TipText: ByRef; const ContextObject: IDispatch) of object;
    TOnCommandEnabled       = procedure(Command: OleVariant; const Enabled: ByRef) of object;
    TOnCommandChecked       = procedure(Command: OleVariant; const Checked: ByRef) of object;
    TOnCommandTipText       = procedure(Command: OleVariant; const Caption: ByRef) of object;
    TOnCommandBeforeExecute = procedure(Command: OleVariant; const Cancel: ByRef) of object;
    TOnCommandExecute       = procedure(Command: OleVariant; Succeeded: WordBool) of object;
    TOnBeforeContextMenu    = procedure(x: Integer; y: Integer; const Menu: ByRef; const Cancel: ByRef) of object;
    TOnBeforeRender         = procedure(const drawObject: ChChartDraw; const chartObject: IDispatch; const Cancel: ByRef) of object;
    TOnAfterRender          = procedure(const drawObject: ChChartDraw; const chartObject: IDispatch) of object;
    TOnAfterFinalRender     = procedure(const drawObject: ChChartDraw) of object;
    TOnAfterLayout          = procedure(const drawObject: ChChartDraw) of object;
    TOnViewChange           = procedure of object;
    // ... IPivotControlEvents (здесь и далее только уникальные)
    TOnViewChangePC         = procedure (Reason: PivotViewReasonEnum) of object;
    TOnDataChange           = procedure (Reason: PivotDataReasonEnum) of object;
    TOnPivotTableChange     = procedure (Reason: PivotTableReasonEnum) of object;
    TOnBeforeQuery          = procedure of object;
    TOnQuery                = procedure of object;
    TOnConnect              = procedure of object;
    TOnDisconnect           = procedure of object;
    TOnStartEdit            = procedure (const Selection: IDispatch; const ActiveObject: IDispatch;
                                const InitialValue: ByRef; const ArrowMode: ByRef;
                                const CaretPosition: ByRef; const Cancel: ByRef;
                                const ErrorDescription: ByRef) of object;
    TOnEndEdit              = procedure (Accept: WordBool; const FinalValue: ByRef; const Cancel: ByRef;
                                const ErrorDescription: ByRef) of object;
    // ... ISpreadSheetEventSink
    TOnInitialize           = procedure of object;
    TOnLoadCompleted        = procedure of object;
    TOnMouseOut             = procedure (Button: Integer; Shift: Integer; const Target: _Range) of object;
    TOnMouseOver            = procedure (Button: Integer; Shift: Integer; const Target: _Range) of object;
    TOnSelectionChanging    = procedure (const Range: _Range) of object;
    TOnSheetActivate        = procedure (const Sh: Worksheet) of object;
    TOnSheetCalculate       = procedure (const Sh: Worksheet) of object;
    TOnSheetChange          = procedure (const Sh: Worksheet; const Target: _Range) of object;
    TOnSheetDeactivate      = procedure (const Sh: Worksheet) of object;
    TOnSheetFollowHyperlink = procedure (const Sh: Worksheet; const Target: Hyperlink) of object;
    TOnSpreadsheetViewChange= procedure (const Target: _Range) of object;
    // ... IRangeEvents

    // ... _DataSourceControlEvent
    TOnCurrent               = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeExpand          = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeCollapse        = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeFirstPage       = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforePreviousPage    = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeNextPage        = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeLastPage        = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnDataError             = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnDataPageComplete      = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeInitialBind     = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnRecordsetSaveProgress = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnAfterDelete           = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnAfterInsert           = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnAfterUpdate           = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeDelete          = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeInsert          = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeOverwrite       = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnBeforeUpdate          = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnDirty                 = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnRecordExit            = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnUndo                  = procedure (const DSCEventInfo: DSCEventInfo) of object;
    TOnFocus                 = procedure (const DSCEventInfo: DSCEventInfo) of object;

    // ... _NavigationEvent



    // ADODB
    TOnWillExecute           = procedure (var Source: WideString; var CursorType: CursorTypeEnum;
                          var LockType: LockTypeEnum; var Options: Integer;
                          var adStatus: EventStatusEnum; const pCommand: _Command;
                          const pRecordset: _Recordset; const pConnection: _Connection);


  // обработчик событий для Chart
  TChartEventHandler = class (TOWCEventsHandler)
  private
    fOnDataSetChange        : TOnDataSetChange;
    fOnDblClick             : TOnDBlClick;
    fOnClick                : TOnClick;
    fOnKeyDown              : TOnKeyDown;
    fOnKeyUp                : TOnKeyUp;
    fOnKeyPress             : TOnKeyPress;
    fOnBeforeKeyDown        : TOnBeforeKeyDown;
    fOnBeforeKeyUp          : TOnBeforeKeyUp;
    fOnBeforeKeyPress       : TOnBeforeKeyPress;
    fOnMouseDown            : TOnMouseDown;
    fOnMouseMove            : TOnMouseMove;
    fOnMouseUp              : TOnMouseUp;
    fOnMouseWheel           : TOnMouseWheel;
    fOnSelectionChange      : TOnSelectionChange;
    fOnBeforeScreenTip      : TOnBeforeScreenTip;
    fOnCommandEnabled       : TOnCommandEnabled;
    fOnCommandChecked       : TOnCommandChecked;
    fOnCommandTipText       : TOnCommandTipText;
    fOnCommandBeforeExecute : TOnCommandBeforeExecute;
    fOnCommandExecute       : TOnCommandExecute;
    fOnBeforeContextMenu    : TOnBeforeContextMenu;
    fOnBeforeRender         : TOnBeforeRender;
    fOnAfterRender          : TOnAfterRender;
    fOnAfterFinalRender     : TOnAfterFinalRender;
    fOnAfterLayout          : TOnAfterLayout;
    fOnViewChange           : TOnViewChange;
  protected
     procedure fSetEventSource(const Val : IUnknown); override;
     procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    property OnDataSetChange : TOnDataSetChange read fOnDataSetChange write fOnDataSetChange;
    property OnDblClick : TOnDblClick read fOnDblClick write fOnDblClick;
    property OnClick : TOnClick read fOnClick write fOnClick;
    property OnKeyDown : TOnKeyDown read fOnKeyDown write fOnKeyDown;
    property OnKeyUp : TOnKeyUp read fOnKeyUp write fOnKeyUp;
    property OnKeyPress : TOnKeyPress read fOnKeyPress write fOnKeyPress;
    property OnBeforeKeyDown : TOnBeforeKeyDown read fOnBeforeKeyDown write fOnBeforeKeyDown;
    property OnBeforeKeyUp : TOnBeforeKeyUp read fOnBeforeKeyUp write fOnBeforeKeyUp;
    property OnBeforeKeyPress : TOnBeforeKeyPress read fOnBeforeKeyPress write fOnBeforeKeyPress;
    property OnMouseDown : TOnMouseDown read fOnMouseDown write fOnMouseDown;
    property OnMouseMove : TOnMouseMove read fOnMouseMove write fOnMouseMove;
    property OnMouseUp : TOnMouseUp read fOnMouseUp write fOnMouseUp;
    property OnMouseWheel : TOnMouseWheel read fOnMouseWheel write fOnMouseWheel;
    property OnSelectionChange : TOnSelectionChange read fOnSelectionChange write fOnSelectionChange;
    property OnBeforeScreenTip : TOnBeforeScreenTip read fOnBeforeScreenTip write fOnBeforeScreenTip;
    property OnCommandEnabled : TOnCommandEnabled read fOnCommandEnabled write fOnCommandEnabled;
    property OnCommandChecked : TOnCommandChecked read fOnCommandChecked write fOnCommandChecked;
    property OnCommandTipText : TOnCommandTipText read fOnCommandTipText write fOnCommandTipText;
    property OnCommandBeforeExecute : TOnCommandBeforeExecute read fOnCommandBeforeExecute write fOnCommandBeforeExecute;
    property OnCommandExecute : TOnCommandExecute read fOnCommandExecute write fOnCommandExecute;
    property OnBeforeContextMenu : TOnBeforeContextMenu read fOnBeforeContextMenu write fOnBeforeContextMenu;
    property OnBeforeRender : TOnBeforeRender read fOnBeforeRender write fOnBeforeRender;
    property OnAfterRender : TOnAfterRender read fOnAfterRender write fOnAfterRender;
    property OnAfterFinalRender : TOnAfterFinalRender read fOnAfterFinalRender write fOnAfterFinalRender;
    property OnAfterLayout : TOnAfterLayout read fOnAfterLayout write fOnAfterLayout;
    property OnViewChange : TOnViewChange read fOnViewChange write fOnViewChange;
  end;

  TPivotControlEventhandler = class(TOWCEventsHandler)
  private
    fOnSelectionChange          : TOnSelectionChange;
    fOnViewChange               : TOnViewChangePC;
    fOnDataChange               : TOnDataChange;
    fOnPivotTableChange         : TOnPivotTableChange;
    fOnBeforeQuery              : TOnBeforeQuery;
    fOnQuery                    : TOnQuery;
    fOnOnConnect                : TOnConnect;
    fOnDisconnect               : TOnDisconnect;
    fOnMouseDown                : TOnMouseDown;
    fOnMouseMove                : TOnMouseMove;
    fOnMouseUp                  : TOnMouseUp;
    fOnMouseWheel               : TOnMouseWheel;
    fOnClick                    : TOnClick;
    fOnDblClick                 : TOnDblClick;
    fOnCommandEnabled           : TOnCommandEnabled;
    fOnCommandChecked           : TOnCommandChecked;
    fOnCommandTipText           : TOnCommandTipText;
    fOnCommandBeforeExecute     : TOnCommandBeforeExecute;
    fOnCommandExecute           : TOnCommandExecute;
    fOnKeyDown                  : TOnKeyDown;
    fOnKeyUp                    : TOnKeyUp;
    fOnKeyPress                 : TOnKeyPress;
    fOnBeforeKeyDown            : TOnBeforeKeyDown;
    fOnBeforeKeyUp              : TOnBeforeKeyUp;
    fOnBeforeKeyPress           : TOnBeforeKeyPress;
    fOnBeforeContextMenu        : TOnBeforeContextMenu;
    fOnStartEdit                : TOnStartEdit;
    fOnEndEdit                  : TOnEndEdit;
    fOnBeforeScreenTip          : TOnBeforeScreenTip;
  protected
    procedure fSetEventSource(const Val : IUnknown); override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    property OnSelectionChange : TOnSelectionChange read fOnSelectionChange write fOnSelectionChange;
    property OnViewChange : TOnViewChangePC read fOnViewChange write fOnViewChange;
    property OnDataChange : TOnDataChange read fOnDataChange write fOnDataChange;
    property OnPivotTableChange : TOnPivotTableChange read fOnPivotTableChange write fOnPivotTableChange;
    property OnBeforeQuery : TOnBeforeQuery read fOnBeforeQuery write fOnBeforeQuery;
    property OnQuery : TOnQuery read fOnQuery write fOnQuery;
    property OnOnConnect : TOnConnect read fOnOnConnect write fOnOnConnect;
    property OnDisconnect : TOnDisconnect read fOnDisconnect write fOnDisconnect;
    property OnMouseDown : TOnMouseDown read fOnMouseDown write fOnMouseDown;
    property OnMouseMove : TOnMouseMove read fOnMouseMove write fOnMouseMove;
    property OnMouseUp : TOnMouseUp read fOnMouseUp write fOnMouseUp;
    property OnMouseWheel : TOnMouseWheel read fOnMouseWheel write fOnMouseWheel;
    property OnClick : TOnClick read fOnClick write fOnClick;
    property OnDblClick : TOnDblClick read fOnDblClick write fOnDblClick;
    property OnCommandEnabled : TOnCommandEnabled read fOnCommandEnabled write fOnCommandEnabled;
    property OnCommandChecked : TOnCommandChecked read fOnCommandChecked write fOnCommandChecked;
    property OnCommandTipText : TOnCommandTipText read fOnCommandTipText write fOnCommandTipText;
    property OnCommandBeforeExecute : TOnCommandBeforeExecute read fOnCommandBeforeExecute write fOnCommandBeforeExecute;
    property OnCommandExecute : TOnCommandExecute read fOnCommandExecute write fOnCommandExecute;
    property OnKeyDown : TOnKeyDown read fOnKeyDown write fOnKeyDown;
    property OnKeyUp : TOnKeyUp read fOnKeyUp write fOnKeyUp;
    property OnKeyPress : TOnKeyPress read fOnKeyPress write fOnKeyPress;
    property OnBeforeKeyDown : TOnBeforeKeyDown read fOnBeforeKeyDown write fOnBeforeKeyDown;
    property OnBeforeKeyUp : TOnBeforeKeyUp read fOnBeforeKeyUp write fOnBeforeKeyUp;
    property OnBeforeKeyPress : TOnBeforeKeyPress read fOnBeforeKeyPress write fOnBeforeKeyPress;
    property OnBeforeContextMenu : TOnBeforeContextMenu read fOnBeforeContextMenu write fOnBeforeContextMenu;
    property OnStartEdit : TOnStartEdit read fOnStartEdit write fOnStartEdit;
    property OnEndEdit : TOnEndEdit read fOnEndEdit write fOnEndEdit;
    property OnBeforeScreenTip : TOnBeforeScreenTip read fOnBeforeScreenTip write fOnBeforeScreenTip;
  end;

  TSpreadSheetEventHandler=class(TOwcEventsHandler)
  private
    fOnBeforeContextMenu:     TOnBeforeContextMenu;
    fOnBeforeKeyDown:         TOnBeforeKeyDown;
    fOnBeforeKeyPress:        TOnBeforeKeyPress;
    fOnBeforeKeyUp:           TOnBeforeKeyUp;
    fOnClick:                 TOnClick;
    fOnCommandEnabled:        TOnCommandEnabled;
    fOnCommandChecked:        TOnCommandChecked;
    fOnCommandTipText:        TOnCommandTipText;
    fOnCommandBeforeExecute:  TOnCommandBeforeExecute;
    fOnCommandExecute:        TOnCommandExecute;
    fOnDblClick:              TOnDblClick;
    fOnEndEdit:               TOnEndEdit;
    fOnInitialize:            TOnInitialize;
    fOnKeyDown:               TOnKeyDown;
    fOnKeyPress:              TOnKeyPress;
    fOnKeyUp:                 TOnKeyUp;
    fOnLoadCompleted:         TOnLoadCompleted;
    fOnMouseDown:             TOnMouseDown;
    fOnMouseOut:              TOnMouseOut;
    fOnMouseOver:             TOnMouseOver;
    fOnMouseUp:               TOnMouseUp;
    fOnMouseWheel:            TOnMouseWheel;
    fOnSelectionChange:       TOnSelectionChange;
    fOnSelectionChanging:     TOnSelectionChanging;
    fOnSheetActivate:         TOnSheetActivate;
    fOnSheetCalculate:        TOnSheetCalculate;
    fOnSheetChange:           TOnSheetChange;
    fOnSheetDeactivate:       TOnSheetDeactivate;
    fOnSheetFollowHyperlink:  TOnSheetFollowHyperlink;
    fOnStartEdit:             TOnStartEdit;
    fOnViewChange:            TOnSpreadsheetViewChange;
  protected
    procedure fSetEventSource(const Val : IUnknown); override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    property OnBeforeContextMenu: TOnBeforeContextMenu read FOnBeforeContextMenu write FOnBeforeContextMenu;
    property OnBeforeKeyDown: TOnBeforeKeyDown read FOnBeforeKeyDown write FOnBeforeKeyDown;
    property OnBeforeKeyPress: TOnBeforeKeyPress read FOnBeforeKeyPress write FOnBeforeKeyPress;
    property OnBeforeKeyUp: TOnBeforeKeyUp read FOnBeforeKeyUp write FOnBeforeKeyUp;
    property OnClick: TOnClick read FOnClick write FOnClick;
    property OnCommandEnabled: TOnCommandEnabled read FOnCommandEnabled write FOnCommandEnabled;
    property OnCommandChecked: TOnCommandChecked read FOnCommandChecked write FOnCommandChecked;
    property OnCommandTipText: TOnCommandTipText read FOnCommandTipText write FOnCommandTipText;
    property OnCommandBeforeExecute: TOnCommandBeforeExecute read FOnCommandBeforeExecute write FOnCommandBeforeExecute;
    property OnCommandExecute: TOnCommandExecute read FOnCommandExecute write FOnCommandExecute;
    property OnDblClick: TOnDblClick read FOnDblClick write FOnDblClick;
    property OnEndEdit: TOnEndEdit read FOnEndEdit write FOnEndEdit;
    property OnInitialize: TOnInitialize read FOnInitialize write FOnInitialize;
    property OnKeyDown: TOnKeyDown read FOnKeyDown write FOnKeyDown;
    property OnKeyPress: TOnKeyPress read FOnKeyPress write FOnKeyPress;
    property OnKeyUp: TOnKeyUp read FOnKeyUp write FOnKeyUp;
    property OnLoadCompleted: TOnLoadCompleted read FOnLoadCompleted write FOnLoadCompleted;
    property OnMouseDown: TOnMouseDown read FOnMouseDown write FOnMouseDown;
    property OnMouseOut: TOnMouseOut read FOnMouseOut write FOnMouseOut;
    property OnMouseOver: TOnMouseOver read FOnMouseOver write FOnMouseOver;
    property OnMouseUp: TOnMouseUp read FOnMouseUp write FOnMouseUp;
    property OnMouseWheel: TOnMouseWheel read FOnMouseWheel write FOnMouseWheel;
    property OnSelectionChange: TOnSelectionChange read FOnSelectionChange write FOnSelectionChange;
    property OnSelectionChanging: TOnSelectionChanging read FOnSelectionChanging write FOnSelectionChanging;
    property OnSheetActivate: TOnSheetActivate read FOnSheetActivate write FOnSheetActivate;
    property OnSheetCalculate: TOnSheetCalculate read FOnSheetCalculate write FOnSheetCalculate;
    property OnSheetChange: TOnSheetChange read FOnSheetChange write FOnSheetChange;
    property OnSheetDeactivate: TOnSheetDeactivate read FOnSheetDeactivate write FOnSheetDeactivate;
    property OnSheetFollowHyperlink: TOnSheetFollowHyperlink read FOnSheetFollowHyperlink write FOnSheetFollowHyperlink;
    property OnStartEdit: TOnStartEdit read FOnStartEdit write FOnStartEdit;
    property OnViewChange: TOnSpreadsheetViewChange read FOnViewChange write FOnViewChange;
  end;

  TDataSourceControlEventHandler = class(TOwcEventsHandler)
  private
    fOnCurrent               :    TOnCurrent;
    fOnBeforeExpand          :    TOnBeforeExpand;
    fOnBeforeCollapse        :    TOnBeforeCollapse;
    fOnBeforeFirstPage       :    TOnBeforeFirstPage;
    fOnBeforePreviousPage    :    TOnBeforePreviousPage;
    fOnBeforeNextPage        :    TOnBeforeNextPage;
    fOnBeforeLastPage        :    TOnBeforeLastPage;
    fOnDataError             :    TOnDataError;
    fOnDataPageComplete      :    TOnDataPageComplete;
    fOnBeforeInitialBind     :    TOnBeforeInitialBind;
    fOnRecordsetSaveProgress :    TOnRecordsetSaveProgress;
    fOnAfterDelete           :    TOnAfterDelete;
    fOnAfterInsert           :    TOnAfterInsert;
    fOnAfterUpdate           :    TOnAfterUpdate;
    fOnBeforeDelete          :    TOnBeforeDelete;
    fOnBeforeInsert          :    TOnBeforeInsert;
    fOnBeforeOverwrite       :    TOnBeforeOverwrite;
    fOnBeforeUpdate          :    TOnBeforeUpdate;
    fOnDirty                 :    TOnDirty;
    fOnRecordExit            :    TOnRecordExit;
    fOnUndo                  :    TOnUndo;
    fOnFocus                 :    TOnFocus ;
  protected
    procedure fSetEventSource(const Val : IUnknown); override;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray); override;
  public
    property OnCurrent               : TOnCurrent read fOnCurrent write fOnCurrent;
    property OnBeforeExpand          : TOnBeforeExpand read fOnBeforeExpand write fOnBeforeExpand;
    property OnBeforeCollapse        : TOnBeforeCollapse read fOnBeforeCollapse write fOnBeforeCollapse;
    property OnBeforeFirstPage       : TOnBeforeFirstPage read fOnBeforeFirstPage write fOnBeforeFirstPage;
    property OnBeforePreviousPage    : TOnBeforePreviousPage read fOnBeforePreviousPage write fOnBeforePreviousPage;
    property OnBeforeNextPage        : TOnBeforeNextPage read fOnBeforeNextPage write fOnBeforeNextPage;
    property OnBeforeLastPage        : TOnBeforeLastPage read fOnBeforeLastPage write fOnBeforeLastPage;
    property OnDataError             : TOnDataError read fOnDataError write fOnDataError;
    property OnDataPageComplete      : TOnDataPageComplete read fOnDataPageComplete write fOnDataPageComplete;
    property OnBeforeInitialBind     : TOnBeforeInitialBind read fOnBeforeInitialBind write fOnBeforeInitialBind;
    property OnRecordsetSaveProgress : TOnRecordsetSaveProgress read fOnRecordsetSaveProgress write fOnRecordsetSaveProgress;
    property OnAfterDelete           : TOnAfterDelete read fOnAfterDelete write fOnAfterDelete;
    property OnAfterInsert           : TOnAfterInsert read fOnAfterInsert write fOnAfterInsert;
    property OnAfterUpdate           : TOnAfterUpdate read fOnAfterUpdate write fOnAfterUpdate;
    property OnBeforeDelete          : TOnBeforeDelete read fOnBeforeDelete write fOnBeforeDelete;
    property OnBeforeInsert          : TOnBeforeInsert read fOnBeforeInsert write fOnBeforeInsert;
    property OnBeforeOverwrite       : TOnBeforeOverwrite read fOnBeforeOverwrite write fOnBeforeOverwrite;
    property OnBeforeUpdate          : TOnBeforeUpdate read fOnBeforeUpdate write fOnBeforeUpdate;
    property OnDirty                 : TOnDirty read fOnDirty write fOnDirty;
    property OnRecordExit            : TOnRecordExit read fOnRecordExit write fOnRecordExit;
    property OnUndo                  : TOnUndo read fOnUndo write fOnUndo;
    property OnFocus                 : TOnFocus read fOnFocus write fOnFocus;
  end;


implementation

procedure TConnectionEventsHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(fEventSource) then InterfaceDisconnect(fEventSource, DIID_ConnectionEvents, fConnection);
  if Val <> fEventSource then fEventSource := Val;
  // прицепляемся к новому объекту
  if Assigned(fEventSource) then InterfaceConnect(fEventSource, DIID_ConnectionEvents, Self as IUnknown, fConnection);
end;

function TConnectionEventsHandler.QueryInterface(const IID: TGUID; out Obj): HResult;
var  tmpStr : string;
begin
  Result := S_OK;
  try
    tmpStr := GUIDToString(IID);
  except
  end;
  // пытаемся вернуть указатель на запарошенный интерфейс
  if not GetInterface(IID, Obj)
    then
      tmpStr := 'ffff';
end;

function TConnectionEventsHandler._AddRef: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;

function TConnectionEventsHandler._Release: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;


function  TConnectionEventsHandler.InfoMessage(const pError: Error; var adStatus: EventStatusEnum;
                          const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.BeginTransComplete(TransactionLevel: Integer; const pError: Error;
                                 var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.CommitTransComplete(const pError: Error; var adStatus: EventStatusEnum;
                                  const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.RollbackTransComplete(const pError: Error; var adStatus: EventStatusEnum;
                                    const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.WillExecute(var Source: WideString; var CursorType: CursorTypeEnum;
                          var LockType: LockTypeEnum; var Options: Integer;
                          var adStatus: EventStatusEnum; const pCommand: _Command;
                          const pRecordset: _Recordset; const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.ExecuteComplete(RecordsAffected: Integer; const pError: Error;
                              var adStatus: EventStatusEnum; const pCommand: _Command;
                              const pRecordset: _Recordset; const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.WillConnect(var ConnectionString: WideString; var UserID: WideString;
                          var Password: WideString; var Options: Integer;
                          var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.ConnectComplete(const pError: Error; var adStatus: EventStatusEnum;
                              const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;

function  TConnectionEventsHandler.Disconnect(var adStatus: EventStatusEnum; const pConnection: _Connection): HResult; stdcall;
begin
  result := S_OK;
end;



////////////////////////////////////
procedure TOWCEventsHandler.fSetEventSource(const Val : IUnknown);
begin
  if Val <> fEventSource then fEventSource := Val;
end;

destructor TOWCEventsHandler.Destroy;
begin
  EventSource := nil;
  inherited Destroy;
end;

function TOWCEventsHandler.QueryInterface(const IID: TGUID; out Obj): HResult;
var
  tmpStr : string;
begin
  Result := S_OK;
  try
    tmpStr := GUIDToString(IID);
    //tmpStr := ClassIDToProgID(IID);
  except
  end;
  // пытаемся вернуть указатель на запарошенный интерфейс
  if not GetInterface(IID, Obj)
    // всегда будем возвращать указатель на IDispatch
    then GetInterface(IDispatch, obj);
end;

function TOWCEventsHandler._AddRef: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;

function TOWCEventsHandler._Release: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;


function TOWCEventsHandler.GetTypeInfoCount(out Count: Integer): HResult;
begin
  Count := 0;
  Result:= S_OK;
end;

function TOWCEventsHandler.GetTypeInfo(Index, LocaleID: Integer; out TypeInfo): HResult;
begin
  Pointer(TypeInfo) := nil;
  Result := E_NOTIMPL;
end;

function TOWCEventsHandler.GetIDsOfNames(const IID: TGUID; Names: Pointer;
  NameCount, LocaleID: Integer; DispIDs: Pointer): HResult;
begin
  Result := E_NOTIMPL;
end;

procedure TOWCEventsHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
begin
 {$IFDEF DEBUG}
   // здесь можно вывести какую-нибудь отладочную информацию
 {$ENDIF}
end;

// приводим параметры к более удобному виду... позаимствовано из OleServer.pas
function TOWCEventsHandler.Invoke(DispID: Integer; const IID: TGUID;
  LocaleID: Integer; Flags: Word; var Params;
  VarResult, ExcepInfo, ArgErr: Pointer): HResult;
var
  ParamCount, I: integer;
  VarArray : TVariantArray;
begin
  // Get parameter count
  ParamCount := TDispParams(Params).cArgs;
  // Set our array to appropriate length
  SetLength(VarArray, ParamCount);
  // Copy over data
  for I := Low(VarArray) to High(VarArray) do
    VarArray[High(VarArray)-I] := OleVariant(TDispParams(Params).rgvarg^[I]);
  // передаём на обрбаботку
  InvokeEvent(DispID, VarArray);
  // Clean array
  SetLength(VarArray, 0);
  // Pascal Events return 'void' - so assume success!
  Result := S_OK;
end;

procedure TChartEventHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(EventSource) then InterfaceDisconnect(EventSource, DIID_IChartEvents, fConnection);
  inherited;
  // прицепляемся к новому объекту
  if Assigned(EventSource) then InterfaceConnect(EventSource, DIID_IChartEvents, Self as IUnknown, fConnection);
end;

function VarToUnk(const Val : variant) : IUnknown;
begin
  result := IUnknown(TVarData(Val).VUnknown)
end;

// процедура обработки событий - по переданному DispID вызывает соответсвующий обработчик
// DispID берётся из OWC10_TLB.pas
procedure TChartEventHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
var LowBound : integer;
begin
 inherited;
 LowBound := Low(Params);
 case DispID of
   5101 : if Assigned(OnDataSetChange)
            then OnDataSetChange;
   5102 : if Assigned(OnDblClick)
            then OnDblClick;
   5103 : if Assigned(OnClick)
            then OnClick;
   1009 : if Assigned(OnKeyDown)
            then OnKeyDown(Params[LowBound], Params[LowBound + 1]);
   1008 : if Assigned(OnKeyUp)
            then OnKeyUp(Params[LowBound], Params[LowBound + 1]);
   1010 : if Assigned(OnKeyPress)
            then OnKeyPress(Params[LowBound]);
   1006 : if Assigned(OnBeforeKeyDown)
            then OnBeforeKeyDown(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1005 : if Assigned(OnBeforeKeyUp)
            then OnBeforeKeyUp(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1007 : if Assigned(OnBeforeKeyPress)
            then OnBeforeKeyPress(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   5107 : if Assigned(OnMouseDown)
            then OnMouseDown(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   5108 : if Assigned(OnMouseMove)
            then OnMouseMove(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   5109 : if Assigned(OnMouseUp)
            then OnMouseUp(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   5118 : if Assigned(OnMouseWheel)
            then OnMouseWheel(Params[LowBound], Params[LowBound + 1]);
   5110 : if Assigned(OnSelectionChange)
            then OnSelectionChange;
   5120 : if Assigned(OnBeforeScreenTip)
            then OnBeforeScreenTip(VarToUnk(Params[LowBound]) as ByRef, Params[LowBound + 1]);
   1000 : if Assigned(OnCommandEnabled)
            then OnCommandEnabled(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1001 : if Assigned(OnCommandChecked)
            then OnCommandChecked(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1002 : if Assigned(OnCommandTipText)
            then OnCommandTipText(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1003 : if Assigned(OnCommandBeforeExecute)
            then OnCommandBeforeExecute(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1004 : if Assigned(OnCommandExecute)
            then OnCommandExecute(Params[LowBound], Params[LowBound + 1]);
   1011 : if Assigned(OnBeforeContextMenu)
            then OnBeforeContextMenu(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef, VarToUnk(Params[LowBound + 3]) as ByRef);
   5111 : if Assigned(OnBeforeRender)
            then OnBeforeRender(VarToUnk(Params[LowBound]) as chChartDraw, Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   5112 : if Assigned(OnAfterRender)
            then OnAfterRender(VarToUnk(Params[LowBound]) as chChartDraw, Params[LowBound + 1]);
   5113 : if Assigned(OnAfterFinalRender)
            then OnAfterFinalRender(VarToUnk(Params[LowBound]) as chChartDraw);
   5114 : if Assigned(OnAfterLayout)
            then OnAfterLayout(VarToUnk(Params[LowBound]) as chChartDraw);
   5119 : if Assigned(OnViewChange)
            then OnViewChange;
 end;
end;


procedure TPivotControlEventHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(EventSource) then InterfaceDisconnect(EventSource, DIID_IPivotControlEvents, fConnection);
  inherited;
  // прицепляемся к новому объекту
  if Assigned(EventSource) then InterfaceConnect(EventSource, DIID_IPivotControlEvents, Self as IUnknown, fConnection);
end;

procedure TPivotControlEventHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
var LowBound : integer;
begin
 inherited;
 LowBound := Low(Params);
 case DispID of
   6003: if Assigned(OnSelectionChange)
           then OnSelectionChange;
   6004: if Assigned(OnViewChange)
           then OnViewChange(Params[LowBound]);
   6007: if Assigned(OnDataChange)
           then OnDataChange(Params[LowBound]);
   6021: if Assigned(OnPivotTableChange)
           then OnPivotTableChange(Params[LowBound]);
   6043: if Assigned(OnBeforeQuery)
           then OnBeforeQuery;
   6044: if Assigned(OnQuery)
           then OnQuery;
   6029: if Assigned(OnOnConnect)
           then OnOnConnect;
   6030: if Assigned(OnDisconnect)
           then OnDisconnect;
   6034: if Assigned(OnMouseDown)
            then OnMouseDown(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   6032 : if Assigned(OnMouseMove)
            then OnMouseMove(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   6033 : if Assigned(OnMouseUp)
            then OnMouseUp(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   6035 : if Assigned(OnMouseWheel)
            then OnMouseWheel(Params[LowBound], Params[LowBound + 1]);
   6005: if Assigned(OnClick)
           then OnClick;
   6006: if Assigned(OnDblClick)
           then OnDblClick;
   1000 : if Assigned(OnCommandEnabled)
            then OnCommandEnabled(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1001 : if Assigned(OnCommandChecked)
            then OnCommandChecked(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1002 : if Assigned(OnCommandTipText)
            then OnCommandTipText(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1003 : if Assigned(OnCommandBeforeExecute)
            then OnCommandBeforeExecute(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1004 : if Assigned(OnCommandExecute)
            then OnCommandExecute(Params[LowBound], Params[LowBound + 1]);
   1009 : if Assigned(OnKeyDown)
            then OnKeyDown(Params[LowBound], Params[LowBound + 1]);
   1008 : if Assigned(OnKeyUp)
            then OnKeyUp(Params[LowBound], Params[LowBound + 1]);
   1010 : if Assigned(OnKeyPress)
            then OnKeyPress(Params[LowBound]);
   1006 : if Assigned(OnBeforeKeyDown)
            then OnBeforeKeyDown(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1005 : if Assigned(OnBeforeKeyUp)
            then OnBeforeKeyUp(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1007 : if Assigned(OnBeforeKeyPress)
            then OnBeforeKeyPress(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1011 : if Assigned(OnBeforeContextMenu)
            then OnBeforeContextMenu(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef, VarToUnk(Params[LowBound + 3]) as ByRef);
   6045 : if Assigned(OnStartEdit)
            then OnStartEdit(Params[LowBound], Params[LowBound+1],
            VarToUnk(Params[LowBound+2]) as ByRef, VarToUnk(Params[LowBound+3]) as ByRef,
            VarToUnk(Params[LowBound+4]) as ByRef,VarToUnk(Params[LowBound+5]) as ByRef,
            VarToUnk(Params[LowBound+6]) as ByRef);
   6046 : if Assigned(OnEndEdit)
            then OnEndEdit(Params[LowBound],VarToUnk(Params[LowBound+1]) as ByRef,
            VarToUnk(Params[LowBound+2]) as ByRef,VarToUnk(Params[LowBound+3]) as ByRef);
   6049 : if Assigned(OnBeforeScreenTip)
            then OnBeforeScreenTip(VarToUnk(Params[LowBound]) as ByRef, Params[LowBound + 1]);
 end;
end;

procedure TSpreadSheetEventHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(EventSource) then InterfaceDisconnect(EventSource, DIID_ISpreadsheetEventSink, fConnection);
  inherited;
  // прицепляемся к новому объекту
  if Assigned(EventSource) then InterfaceConnect(EventSource, DIID_ISpreadsheetEventSink, Self as IUnknown, fConnection);
end;

procedure TSpreadSheetEventHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
var LowBound : integer;
begin
 inherited;
 LowBound := Low(Params);
 case DispID of
   1011: if Assigned(OnBeforeContextMenu)
          then OnBeforeContextMenu(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef, VarToUnk(Params[LowBound + 3]) as ByRef);
   1006: if Assigned(OnBeforeKeyDown)
            then OnBeforeKeyDown(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1007 : if Assigned(OnBeforeKeyPress)
            then OnBeforeKeyPress(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1005 : if Assigned(OnBeforeKeyUp)
            then OnBeforeKeyUp(Params[LowBound], Params[LowBound + 1], VarToUnk(Params[LowBound + 2]) as ByRef);
   1502: if Assigned(OnClick)
           then OnClick;
   1000 : if Assigned(OnCommandEnabled)
            then OnCommandEnabled(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1001 : if Assigned(OnCommandChecked)
            then OnCommandChecked(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1002 : if Assigned(OnCommandTipText)
            then OnCommandTipText(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1003 : if Assigned(OnCommandBeforeExecute)
            then OnCommandBeforeExecute(Params[LowBound], VarToUnk(Params[LowBound + 1]) as ByRef);
   1004 : if Assigned(OnCommandExecute)
            then OnCommandExecute(Params[LowBound], Params[LowBound + 1]);
   1503 : if Assigned(OnDblClick)
           then OnDblClick;
   1504 : if Assigned(OnEndEdit)
           then OnEndEdit(Params[LowBound],VarToUnk(Params[LowBound+1]) as ByRef,
            VarToUnk(Params[LowBound+2]) as ByRef,VarToUnk(Params[LowBound+3]) as ByRef);
   1523: if Assigned(OnInitialize)
          then OnInitialize;
   1009 : if Assigned(OnKeyDown)
            then OnKeyDown(Params[LowBound], Params[LowBound + 1]);
   1010 : if Assigned(OnKeyPress)
            then OnKeyPress(Params[LowBound]);
   1008 : if Assigned(OnKeyUp)
            then OnKeyUp(Params[LowBound], Params[LowBound + 1]);
   1522 : if Assigned(OnLoadCompleted)
          then OnLoadCompleted;
   1505 : if Assigned(OnMouseDown)
            then OnMouseDown(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   1506 : if Assigned(OnMouseOut)
            then OnMouseOut(Params[LowBound], Params[LowBound+1], VarToUnk(Params[LowBound+2]) as _Range);
   1507 : if Assigned(OnMouseOver)
            then OnMouseOver(Params[LowBound], Params[LowBound+1], VarToUnk(Params[LowBound+2]) as _Range);
   1508 : if Assigned(OnMouseUp)
            then OnMouseUp(Params[LowBound], Params[LowBound + 1], Params[LowBound + 2], Params[LowBound + 3]);
   1509 : if Assigned(OnMouseWheel)
            then OnMouseWheel(Params[LowBound], Params[LowBound + 1]);
   1511 : if Assigned(OnSelectionChange)
            then OnSelectionChange;
   1512 : if Assigned(OnSelectionChanging)
            then OnSelectionChanging(VarToUnk(Params[LowBound]) as _Range);
   1513 : if Assigned(OnSheetActivate)
            then OnSheetActivate(VarToUnk(Params[LowBound]) as Worksheet);
   1516 : if Assigned(OnSheetCalculate)
            then OnSheetCalculate(VarToUnk(Params[LowBound]) as Worksheet);
   1517 : if Assigned(OnSheetChange)
            then OnSheetChange(VarToUnk(Params[LowBound]) as Worksheet, VarToUnk(Params[LowBound+1]) as _Range);
   1518 : if Assigned(OnSheetDeactivate)
            then OnSheetDeactivate(VarToUnk(Params[LowBound]) as Worksheet);
   1519 : if Assigned(OnSheetFollowHyperlink)
            then OnSheetFollowHyperlink(VarToUnk(Params[LowBound]) as Worksheet, VarToUnk(Params[LowBound+1]) as Hyperlink);
   1520 : if Assigned(OnStartEdit)
            then OnStartEdit(Params[LowBound], Params[LowBound+1],
            VarToUnk(Params[LowBound+2]) as ByRef, VarToUnk(Params[LowBound+3]) as ByRef,
            VarToUnk(Params[LowBound+4]) as ByRef,VarToUnk(Params[LowBound+5]) as ByRef,
            VarToUnk(Params[LowBound+6]) as ByRef);
   1521 : if Assigned(OnViewChange)
           then OnViewChange(VarToUnk(Params[LowBound]) as _Range);
  end;
end;

procedure InterfaceConnect2(const Source: IUnknown; const IID: TIID;
  const Sink: IUnknown; var Connection: Longint);
var
  CPC: IConnectionPointContainer;
  CP: IConnectionPoint;
begin
  Connection := 0;
  if Succeeded(Source.QueryInterface(IConnectionPointContainer, CPC)) then
    if Succeeded(CPC.FindConnectionPoint(IID, CP)) then
      CP.Advise(Sink, Connection);
end;

procedure TDatasourceControlEventHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(EventSource) then InterfaceDisconnect(EventSource, DIID__DataSourceControlEvent, fConnection);
  inherited;
  // прицепляемся к новому объекту
  if Assigned(EventSource) then InterfaceConnect2(EventSource, DIID__DataSourceControlEvent, Self as IUnknown, fConnection);
end;

procedure TDatasourceControlEventHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
var LowBound : integer;
    tmpDSCEventInfo: DSCEventInfo;
begin
  inherited;
  LowBound := Low(Params);
  tmpDSCEventInfo := VarToUnk(Params[LowBound]) as DSCEventInfo;
  case DispID of
    624 : if Assigned(fOnCurrent)               then fOnCurrent(tmpDSCEventInfo);
    626 : if Assigned(fOnBeforeExpand)          then fOnBeforeExpand(tmpDSCEventInfo);
    627 : if Assigned(fOnBeforeCollapse)        then fOnBeforeCollapse(tmpDSCEventInfo);
    628 : if Assigned(fOnBeforeFirstPage)       then fOnBeforeFirstPage(tmpDSCEventInfo);
    629 : if Assigned(fOnBeforePreviousPage)    then fOnBeforePreviousPage(tmpDSCEventInfo);
    630 : if Assigned(fOnBeforeNextPage)        then fOnBeforeNextPage(tmpDSCEventInfo);
    631 : if Assigned(fOnBeforeLastPage)        then fOnBeforeLastPage(tmpDSCEventInfo);
    632 : if Assigned(fOnDataError)             then fOnDataError(tmpDSCEventInfo);
    633 : if Assigned(fOnDataPageComplete)      then fOnDataPageComplete(tmpDSCEventInfo);
    634 : if Assigned(fOnBeforeInitialBind)     then fOnDataPageComplete(tmpDSCEventInfo);
    635 : if Assigned(fOnRecordsetSaveProgress) then fOnRecordsetSaveProgress(tmpDSCEventInfo);
    636 : if Assigned(fOnAfterDelete)           then fOnAfterDelete(tmpDSCEventInfo);
    637 : if Assigned(fOnAfterInsert)           then fOnAfterInsert(tmpDSCEventInfo);
    638 : if Assigned(fOnAfterUpdate)           then fOnAfterUpdate(tmpDSCEventInfo);
    639 : if Assigned(fOnBeforeDelete)          then fOnBeforeDelete(tmpDSCEventInfo);
    640 : if Assigned(fOnBeforeInsert)          then fOnBeforeInsert(tmpDSCEventInfo);
    641 : if Assigned(fOnBeforeOverwrite)       then fOnBeforeOverwrite(tmpDSCEventInfo);
    642 : if Assigned(fOnBeforeUpdate)          then fOnBeforeUpdate(tmpDSCEventInfo);
    643 : if Assigned(fOnDirty)                 then fOnDirty(tmpDSCEventInfo);
    644 : if Assigned(fOnRecordExit)            then fOnRecordExit(tmpDSCEventInfo);
    647 : if Assigned(fOnUndo)                  then fOnUndo(tmpDSCEventInfo);
    648 : if Assigned(fOnFocus)                 then fOnFocus(tmpDSCEventInfo);
  end;
end;

end.
