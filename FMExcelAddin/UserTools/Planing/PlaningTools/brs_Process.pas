unit brs_Process;

interface

uses
  ComObj, ActiveX, PlaningTools_TLB, StdVcl, brs_ProcessForm, Windows, Messages,
  Forms, Classes, SysUtils, AxCtrls, dialogs;

type
  TProcessForm = class(TAutoObject, IConnectionPointContainer, IProcessForm)
  private
    FConnectionPoints: TConnectionPoints;
    FConnectionPoint: TConnectionPoint;
    FSinkList: TList;
    FEvents: IProcessFormEvents;
  protected
    procedure CloseOperation; safecall;
    procedure OpenOperation(const Name: WideString; IsCritical,
      IsNoteTime: WordBool; OperationType: Integer); safecall;
    procedure PostError(const AText: WideString); safecall;
    procedure PostInfo(const AText: WideString); safecall;
    procedure PostWarning(const AText: WideString); safecall;
    procedure SetPBarPosition(CurrentPosition, MaxPosition: Integer); safecall;
    function Get_Showing: WordBool; safecall;
    function Get_LastError: WideString; safecall;
    function Get_ErrorList: WideString; safecall;
    function Get_NewProcessClear: WordBool; safecall;
    procedure Set_NewProcessClear(Value: WordBool); safecall;
    procedure ClearErrors; safecall;
    procedure OpenProcess(ParentWnd: Integer; const ProcessTitle,
      SuccessMessage, ErrorMessage: WideString; CanReturn: WordBool);
      safecall;
    procedure CloseProcess; safecall;
    procedure DisableLogging; safecall;
    procedure EnableLogging(const FileName: WideString); safecall;
    procedure DirectWriteLogString(const Msg: WideString); safecall;
    procedure Set_ErrorList(const Value: WideString); safecall;
    { Protected declarations }
    property ConnectionPoints: TConnectionPoints read FConnectionPoints implements IConnectionPointContainer;
    procedure EventSinkChanged(const EventSink: IUnknown); override;


    procedure DoReturn;
    procedure DoClose;
    procedure DoShow;
  public
    procedure Initialize; override;
    procedure AfterConstruction; override;
    destructor Destroy; override;

    property Events: IProcessFormEvents read FEvents;
  end;

implementation

uses ComServ;

var
  ProcessForm: TfrmProcess;
  ProcessInfo : TProcessInfo;
  msg : TMSG;
  ParHwnd: integer;
  FProcessCount: integer;
  FLogFileName: string;
  ClientList: TThreadList = nil;

function ThreadProc(Param : pProcessInfo): integer; stdcall;
begin
  ProcessForm := TfrmProcess.CreatePrnt(nil, ParHwnd);
  ProcessForm.fProcessInfo := Param;
  ProcessForm.LogFileName := FLogFileName;
  ProcessForm.Refresh;
  while GetMessage(msg, 0, 0, 0) do begin
    if (Msg.Message = WM_QUIT) then
      break;
    TranslateMessage(msg);
    DispatchMessage(msg);
  end;
  result := 0;
  ProcessForm := nil;
  ExitThread(0)
end;

procedure CreateProcessThread;
var ThreadID : cardinal;
begin
  CloseHandle(CreateThread(nil, 0, @ThreadProc, @ProcessInfo, 0, ThreadID));
  repeat
    sleep(50)
  until Assigned(ProcessForm);              
end;

procedure CloseProcessThread;
begin
  if Assigned(ProcessForm) then SendMessage(ProcessForm.Handle, WM_DESTROY, 0, 0);
  repeat
    sleep(50)
  until not Assigned(ProcessForm);
end;

procedure TProcessForm.CloseOperation;
begin
  if Assigned(ProcessForm) then
    SendMessage(ProcessForm.Handle, WM_CLOSEOPERATION, 0, 0);
end;

procedure TProcessForm.OpenOperation(const Name: WideString; IsCritical,
  IsNoteTime: WordBool; OperationType: Integer);
begin
  with ProcessInfo do
  begin
    OperationName := string(Name);
    Critical := IsCritical;
    NoteTime := IsNoteTime;
    OperType := TOperationType(OperationType);
  end;
  if Assigned(ProcessForm) then
    SendMessage(ProcessForm.Handle, WM_OPENOPERATION, 0, 0);
end;

procedure TProcessForm.PostError(const AText: WideString);
begin
  ProcessInfo.PostText := string(AText);
  if Assigned(ProcessForm) then
    SendMessage(ProcessForm.Handle, WM_POSTERROR, 0, 0);
end;

procedure TProcessForm.PostInfo(const AText: WideString);
begin
  ProcessInfo.PostText := string(AText);
  if Assigned(ProcessForm) then
    SendMessage(ProcessForm.Handle, WM_POSTINFO, 0, 0);
end;

procedure TProcessForm.PostWarning(const AText: WideString);
begin
  ProcessInfo.PostText := string(AText);
  if Assigned(ProcessForm) then
    SendMessage(ProcessForm.Handle, WM_POSTWARNING, 0, 0);
end;

procedure TProcessForm.SetPBarPosition(CurrentPosition,
  MaxPosition: Integer);
begin
  ProcessInfo.PBarCurrentPosition := CurrentPosition;
  ProcessInfo.PBarMaxPosition := MaxPosition;
  if Assigned(ProcessForm) then
  begin
    SendMessage(ProcessForm.Handle, WM_PBARPOSITION, 0, 0);
    Application.ProcessMessages;
  end;
end;

function TProcessForm.Get_Showing: WordBool;
begin
  result := false;
  if Assigned(ProcessForm) then
    result := ProcessForm.Showing;
end;

function TProcessForm.Get_LastError: WideString;
begin
  if Assigned(ProcessForm) then
    result := ProcessForm.LastError;
end;

function TProcessForm.Get_ErrorList: WideString;
begin
  if Assigned(ProcessForm) then
    result := ProcessForm.ErrorList;
end;

procedure TProcessForm.Initialize;
begin
  inherited;
  FProcessCount := 0;

  FConnectionPoints := TConnectionPoints.Create(Self);
  if AutoFactory.EventTypeInfo <> nil then
    FConnectionPoint := FConnectionPoints.CreateConnectionPoint(
      AutoFactory.EventIID, ckSingle, EventConnect)
  else
    FConnectionPoint := nil;
end;

function TProcessForm.Get_NewProcessClear: WordBool;
begin
  if Assigned(ProcessForm) then
    result := ProcessForm.NewProcessClear;
end;

procedure TProcessForm.Set_NewProcessClear(Value: WordBool);
begin
  if Assigned(ProcessForm) then
    ProcessForm.NewProcessClear := Value;
end;

procedure TProcessForm.ClearErrors;
begin
  if not Assigned(ProcessForm) then
    exit;
  ProcessForm.LastError := '';
  ProcessForm.ErrorList := '';
end;

procedure TProcessForm.OpenProcess(ParentWnd: Integer; const ProcessTitle,
  SuccessMessage, ErrorMessage: WideString; CanReturn: WordBool);
begin
  if (FProcessCount = 0) then
  begin
    if Assigned(ProcessForm) then
      CloseProcessThread;
    ParHwnd := ParentWnd;
    CreateProcessThread;
    ProcessForm.btReturn.Visible := CanReturn;
    ProcessInfo.SuccessMessage := string(SuccessMessage);
    ProcessInfo.ErrorMessage := string(ErrorMessage);
    ProcessInfo.OnReturnHandler := DoReturn;
    ProcessInfo.OnCloseHandler := DoClose;
    ProcessInfo.OnShowHandler := DoShow;
  end;
  inc(FProcessCount);
  if Assigned(ProcessForm) then
  begin
    ProcessInfo.TitleProcess := string(ProcessTitle);
    SendMessage(ProcessForm.Handle, WM_SETTITLE, 0, 0);
  end;
end;

procedure TProcessForm.CloseProcess;
begin
  dec(FProcessCount);
  if (FProcessCount <= 0) then
    FProcessCount := 0;
end;

procedure TProcessForm.DisableLogging;
begin
  FLogFileName := '';
  if Assigned(ProcessForm) then
    ProcessForm.LogFileName := '';
end;

procedure TProcessForm.EnableLogging(const FileName: WideString);
begin
  FLogFileName := FileName;
  if Assigned(ProcessForm) then
    ProcessForm.LogFileName := FileName;
end;

procedure TProcessForm.DirectWriteLogString(const Msg: WideString);
var
  LogFile: TextFile;
  s: string;
begin
  if FLogFileName = '' then
    exit;
  AssignFile(LogFile, FLogFileName);
  try
    if FileExists(FLogFileName) then
      Append(LogFile)
    else
      Rewrite(LogFile);
  except
    exit;
  end;

  try
    try
      s := DateTimeToStr(Now) + ': ' + Msg;
      Writeln(LogFile, s);
    except
    end;
  finally
    CloseFile(LogFile);
  end;
end;

procedure TProcessForm.Set_ErrorList(const Value: WideString);
begin

end;

procedure TProcessForm.AfterConstruction;
var
  List: TList;
begin
  inherited;
  try
    List := ClientList.LockList;
    List.Add(Self);
  finally
    ClientList.UnlockList;
  end;
end;

destructor TProcessForm.Destroy;
var
  List: TList;
  SelfIndex: integer;
begin
  try
    List := ClientList.LockList;
    SelfIndex := List.IndexOf(Self);
    if SelfIndex >= 0 then
      List.Delete(SelfIndex);
  finally
    ClientList.UnlockList;
  end;

  inherited;
end;

procedure TProcessForm.EventSinkChanged(const EventSink: IUnknown);
begin
  FEvents := EventSink as IProcessFormEvents;
  if FConnectionPoint <> nil then
     FSinkList := FConnectionPoint.SinkList;
end;

procedure TProcessForm.DoReturn;
var
  List: TList;
  ProcessForm: TProcessForm;
  i: integer;
begin
  inherited;
  try
    List := ClientList.LockList;
    for i := List.Count - 1 downto 0 do
    begin
      ProcessForm := List[i];
      if Assigned(ProcessForm.Events) then
        ProcessForm.Events.OnReturn;
    end;
  finally
    ClientList.UnlockList;
  end;
end;

procedure TProcessForm.DoClose;
var
  List: TList;
  ProcessForm: TProcessForm;
  i: integer;
begin
  inherited;
  try
    List := ClientList.LockList;
    for i := List.Count - 1 downto 0 do
    begin
      ProcessForm := List[i];
      if Assigned(ProcessForm.Events) then
        ProcessForm.Events.OnClose;
    end;
  finally
    ClientList.UnlockList;
  end;
end;

procedure TProcessForm.DoShow;
var
  List: TList;
  ProcessForm: TProcessForm;
  i: integer;
begin
  inherited;
  try
    List := ClientList.LockList;
    for i := List.Count - 1 downto 0 do
    begin
      ProcessForm := List[i];
      if Assigned(ProcessForm.Events) then
        ProcessForm.Events.OnShow;
    end;
  finally
    ClientList.UnlockList;
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TProcessForm, Class_ProcessForm,
    ciMultiInstance, tmApartment);
  ClientList := TThreadList.Create;
finalization
  CloseProcessThread;
  ClientList.Free;
end.
