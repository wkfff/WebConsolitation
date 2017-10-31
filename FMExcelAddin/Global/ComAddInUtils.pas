{
  ������� ������ ��� ��������� �������� � Excel
}

unit ComAddInUtils;

interface

uses
  Windows, ActiveX, OfficeXP, ComObj;

type
  TBaseSink = class(TObject, IUnknown, IDispatch)
  protected
    { ������ IUnknown }
    function QueryInterface(const IID: TGUID; out Obj): HResult; stdcall;
    function _AddRef: integer; stdcall;
    function _Release: integer; stdcall;
    { ������ IDispatch }
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


  // ���������� ������� ������� �� ������
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

  //���������� ������� ��������� ������� ��������
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
  // �� ��������� �������� ��������� � �������� �������
  for i := 0 to DPs.cArgs - 1 do
    PDispIDs^ [i] := DPs.cArgs - 1 - i;
  // ��������� ����������� ���������
  if (DPs.cNamedArgs <= 0) then
    Exit;
  // ������������  ����������� ���������
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
  // �������� ������������ ������
  if (Flags and DISPATCH_METHOD = 0) then
    raise Exception.Create(
      Format('%s only supports sinking of method calls!', [ClassName] ));
  { ���������� ������� pDispIDs. ��� ����� ��������� ��������� ������
    �� ��������� ������������ ������ � ������������ �����������,
    �����, ��� AppEvents �� Excel � �.�. }
  PDispIDs := nil;
  DispIDsSize := 0;
  HasParams := (DPs.cArgs > 0);
  if HasParams then
  begin
    DispIDsSize := DPs.cArgs * SizeOf(TDispID);
    GetMem(PDispIDs, DispIDsSize);
  end;
  try
    // ������������� DispIDs
    if HasParams then
      BuildPositionalDispIDs(PDispIDs, DPs);
    result := DoInvoke(DispId, IID, LocaleID, Flags, DPs, PDispIDs,
      VarResult, ExcepInfo, ArgErr);
  finally
    // ����������� ������
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
  // ���� ������������� ��������� SinkIID - ���������� ���� IDispatch
  if not Succeeded(result) then
    if (IsEqualIID(IID, FSinkIID)) then
      if (GetInterface(IDispatch, Obj)) then
        result := S_OK;
end;

{ ��������� ������ - �������� }

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
    // ����������� ��������� IConnectionPointContainer
    OleCheck(PSource.QueryInterface(IConnectionPointContainer, PointCont));
    // ����������� ��������� IConnectionPoint
    OleCheck(PointCont.FindConnectionPoint(FSinkIID, FCP));
    // ������������ � ����������� �������
    OleCheck(FCP.Advise(Self, FCookie));
    // ��� ������ ������� - ������������� �������� Source
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
  // ������������� ���������� ������������� ��������� �������
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

{  ���� ����� ����� ���������� ��� ������������� �������
   � ���� ����� ���� �������� DispId, ��������������� ���������������
   ������� ����������, � �������� �� ���������� }
//  _CommandBarButtonEvents = dispinterface
//    ['{000C0351-0000-0000-C000-000000000046}']
{    procedure Click(const Ctrl: CommandBarButton; var CancelDefault: WordBool); dispid 1;
  end;
  ����� ������� ���� ����������� ����� � DispId = 1
  � ����� ����������� ���� CommandBarButton � WordBool}

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

