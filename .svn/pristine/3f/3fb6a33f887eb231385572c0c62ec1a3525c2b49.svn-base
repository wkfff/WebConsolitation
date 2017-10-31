{
  Базовые классы для написания примочек к Excel
}

unit ComAddInUtils;

interface

uses
  Windows, ActiveX, OfficeXP, ComObj;

type
  TBaseSink = class(TObject, IUnknown, IDispatch)
  protected
    { методы IUnknown }
    function QueryInterface(const IID: TGUID; out Obj): HResult; stdcall;
    function _AddRef: integer; stdcall;
    function _Release: integer; stdcall;
    { методы IDispatch }
    function GetIDsOfNames(const IID: TGUID; Names: pointer;
      NameCount, LocaleID: integer; DispIDs: pointer): HResult; virtual; stdcall;
    function GetTypeInfo(Index, LocaleID: integer; out TypeInfo): HResult;
      virtual; stdcall;
    function GetTypeInfoCount(out Count: integer): HResult; virtual; stdcall;
    function Invoke(DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var Params; VarResult, ExcepInfo, ArgErr: pointer): HResult;
      virtual; stdcall;
  protected
    FCookie: integer;
    FCP: IConnectionPoint;
    FSinkIID: TGuid;
    FSource: IUnknown;
    function DoInvoke(DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; virtual; abstract;
  public
    destructor Destroy; override;
    procedure Connect(PSource: IUnknown);
    procedure Disconnect;
    property SinkIID: TGuid read FSinkIID;
    property Source: IUnknown read FSource;
  end;


  // обработчик события нажатия на кнопку
  TOnCommandButtonClick = procedure (Button: CommandBarButton;
     var CancelDefault: WordBool) of object;

  TCommandButtonEventSink = class(TBaseSink)
  private
    FOnClick: TOnCommandButtonClick;
  protected
    procedure DoClick(Button: CommandBarButton;
      var CancelDefault: WordBool); virtual;
    function DoInvoke (DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
  public
    constructor Create; virtual;
    property OnClick: TOnCommandButtonClick read FOnClick write FOnClick;
  end;

  //обработчик события изменения офисных тулбаров
  TOnCommandBarsUpdate = procedure of object;
  TCommandBarsEventSink = class(TBaseSink)
  private
    FOnUpdate: TOnCommandBarsupdate;
  protected
    procedure DoUpdate; virtual;
    function DoInvoke(DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
  public
    constructor Create; virtual;
    property OnUpdate: TOnCommandBarsUpdate read FOnUpdate write FOnUpdate;
  end;

implementation
uses
  SysUtils;

{TBaseSink}

procedure BuildPositionalDispIDs(PDispIDs: PDispIDList; const DPs: TDispParams);
var
  i: integer;
begin
  // по умолчанию разносим параметры в обратном порядке
  for i := 0 to DPs.cArgs - 1 do
    PDispIDs^ [i] := DPs.cArgs - 1 - i;
  // проверяем именованные аргументы
  if (DPs.cNamedArgs <= 0) then
    Exit;
  // обрабатываем  именованные аргументы
  for i := 0 to DPs.cNamedArgs - 1 do
    PDispIDs^ [DPs.rgdispidNamedArgs^[i]] := i;
end;

{ TBaseSink }

function TBaseSink.Invoke(DispID: integer; const IID: TGUID; LocaleID: integer;
  Flags: word; var Params; VarResult, ExcepInfo, ArgErr: pointer): HResult;
var
  DPs: TDispParams absolute Params;
  HasParams: boolean;
  PDispIDs: PDispIDList;
  DispIDsSize: integer;
begin
  // проверка корректности вызова
  if (Flags and DISPATCH_METHOD = 0) then
    raise Exception.Create(
      Format('%s only supports sinking of method calls!', [ClassName] ));
  { построение массива pDispIDs. Это может несколько замедлить работу
    но позволяет обрабатывать вызовы с именованными аргументами,
    такие, как AppEvents от Excel и т.п. }
  PDispIDs := nil;
  DispIDsSize := 0;
  HasParams := (DPs.cArgs > 0);
  if HasParams then
  begin
    DispIDsSize := DPs.cArgs * SizeOf(TDispID);
    GetMem(PDispIDs, DispIDsSize);
  end;
  try
    // перестраиваем DispIDs
    if HasParams then
      BuildPositionalDispIDs(PDispIDs, DPs);
    result := DoInvoke(DispId, IID, LocaleID, Flags, DPs, PDispIDs,
      VarResult, ExcepInfo, ArgErr);
  finally
    // освобождаем память
    if HasParams then
      FreeMem(PDispIDs, DispIDsSize);
  end;
end;

function TBaseSink.QueryInterface(const IID: TGUID; out Obj): HResult;
begin
  result := E_NOINTERFACE;
  pointer(Obj) := nil;
  if (GetInterface(IID, Obj)) then
    result := S_OK;
  // если запрашивается интерфейс SinkIID - возвращаем свой IDispatch
  if not Succeeded(result) then
    if (IsEqualIID(IID, FSinkIID)) then
      if (GetInterface(IDispatch, Obj)) then
        result := S_OK;
end;

{ остальные методы - заглушки }

function TBaseSink.GetIDsOfNames(const IID: TGUID; Names: pointer;
  NameCount, LocaleID: integer; DispIDs: pointer): HResult;
begin
  result := E_NOTIMPL;
end;

function TBaseSink.GetTypeInfo(Index, LocaleID: integer; out TypeInfo): HResult;
begin
  result := E_NOTIMPL;
  pointer(TypeInfo) := nil;
end;

function TBaseSink.GetTypeInfoCount(out Count: integer): HResult;
begin
  result := E_NOTIMPL;
  Count := 0;
end;

function TBaseSink._AddRef: integer;
begin
  result := 2;
end;

function TBaseSink._Release: integer;
begin
  result := 1;
end;

destructor TBaseSink.Destroy;
begin
  Disconnect;
  inherited;
end;

procedure TBaseSink.Connect(PSource: IUnknown);
var
  PointCont: IConnectionPointContainer;
begin
  Disconnect;
  try
    // запрашиваем интерфейс IConnectionPointContainer
    OleCheck(PSource.QueryInterface(IConnectionPointContainer, PointCont));
    // запрашиваем интерфейс IConnectionPoint
    OleCheck(PointCont.FindConnectionPoint(FSinkIID, FCP));
    // подключаемся к обработчику событий
    OleCheck(FCP.Advise(Self, FCookie));
    // все прошло успешно - устанавливаем свойство Source
    FSource := PSource;
  except
    raise Exception.Create(Format('Unable to connect %s.'#13'%s',
      [ClassName, Exception(ExceptObject).Message]));
  end;
end;

procedure TBaseSink.Disconnect;
begin
  if (FSource = nil) then
    Exit;
  try
    OleCheck(FCP.Unadvise(FCookie));
    FCP := nil;
    FSource := nil;
  except
    pointer(FCP) := nil;
    pointer(FSource) := nil;
  end;
end;

{ TCommandButtonEventSink }

constructor TCommandButtonEventSink.Create;
begin
  inherited;
  // устанавливаем корректный идентификатор приемника событий
  FSinkIID := _CommandBarButtonEvents;
end;

procedure TCommandButtonEventSink.DoClick(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  if Assigned(FOnClick) then
    FOnClick(Button, CancelDefault);
end;

function TCommandButtonEventSink.DoInvoke(DispID: integer;
  const IID: TGUID; LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin

{  Этот метод будет вызываться при возникновении события
   в него могут быть переданы DispId, соответствующие идентификаторам
   методов интерфейса, к которому мы подключены }
//  _CommandBarButtonEvents = dispinterface
//    ['{000C0351-0000-0000-C000-000000000046}']
{    procedure Click(const Ctrl: CommandBarButton; var CancelDefault: WordBool); dispid 1;
  end;
  Таким образом надо реализовать вызов с DispId = 1
  и двумя параметрами типа CommandBarButton и WordBool}

  result := S_OK;
  case DispID of
    1: DoClick(IUnknown (DPs.rgvarg^ [PDispIDs^ [0]].unkval) as CommandBarButton,
      DPs.rgvarg^ [PDispIDs^ [1]].pbool^);
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

{TCommandBarsEventSink}

constructor TCommandBarsEventSink.Create;
begin
  inherited Create;
  FSinkIID := _CommandBarsEvents;
end;

procedure TCommandBarsEventSink.DoUpdate;
begin
  if Assigned(FOnUpdate) then
    FOnUpdate;
end;

function TCommandBarsEventSink.DoInvoke(DispID: integer;
  const IID: TGUID; LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    1: DoUpdate;
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

end.

