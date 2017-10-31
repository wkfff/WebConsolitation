unit brs_Operation;

interface

uses windows, messages, brs_OperationForm, comobj, PlaningTools_TLB, StdVcl;

type
  TOperation = class(TAutoObject, IOperation)
  protected
    procedure StartOperation(ParentWnd: Integer); safecall;
    procedure StopOperation; safecall;
    procedure Set_Caption(const Value: WideString); safecall;
    property Caption: WideString write Set_Caption;
  public
    procedure Initialize; override;
  end;

implementation

uses ComServ;

var
  OperationForm : TfrmOperation;
  msg : TMSG;

function ThreadProc(Param : pointer): integer; stdcall;
begin
  OperationForm := TFrmOperation.Create(nil);
  OperationForm.Refresh;
  while GetMessage(msg, 0, 0, 0) do begin
    if Msg.Message = WM_QUIT then break;
    TranslateMessage(msg);
    DispatchMessage(msg);
  end;
  result := 0;
  OperationForm := nil;
  ExitThread(0)
end;

procedure CreateOperationThread;
var ThreadID : cardinal;
begin
  CloseHandle(CreateThread(nil, 0, @ThreadProc, nil, 0, ThreadID));
  repeat
    sleep(50)
  until Assigned(OperationForm);
end;

procedure CloseOperationThread;
begin
  if Assigned(OperationForm) then
    SendMessage(OperationForm.Handle, WM_DESTROY, 0, 0);
  repeat
    sleep(50)
  until not Assigned(OperationForm);
end;

procedure TOperation.Initialize;
begin
  inherited;
  if not Assigned(OperationForm) then
    CreateOperationThread;  
end;

procedure TOperation.StartOperation(ParentWnd: Integer);
begin
  if Assigned(OperationForm) then
    SendMessage(OperationForm.Handle, WM_STARTOPERATION, ParentWnd, 0)
end;

procedure TOperation.StopOperation;
begin
  if Assigned(OperationForm) then
    SendMessage(OperationForm.Handle, WM_STOPOPERATION, 0, 0)
end;

procedure TOperation.Set_Caption(const Value: WideString);
var tmpStr : string;
begin
  if Assigned(OperationForm) then
  begin
    tmpStr := '    ' + Value;
    SendMessage(OperationForm.Handle, WM_SETCAPTION, integer(PChar(tmpStr)), 0)
  end
end;

initialization
  TAutoObjectFactory.Create(ComServer, TOperation, CLASS_Operation,
    ciSingleInstance, tmSingle);
finalization
  CloseOperationThread
end.
