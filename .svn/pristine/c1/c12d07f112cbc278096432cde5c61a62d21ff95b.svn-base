{************************************************************************}
{                       Форма индикации процессов                        }
{ Основные методы формы:                                                 }
{    1)OpenOperation(Name: string; IsCritical, IsNoteTime: boolean;      }
{        OperationType: TOperationType);                                 }
{        Метод предназначен для создания операции на форме. Параметрами  }
{        задаётся: имя операции(Name), её критичность(IsCritical),       }
{                требуется ли засекать время(IsNoteTime) и тип           }
{                операции(OperationType). Сейчас от типа операции        }
{                зависит только иконка у данной операции.                }
{    2)CloseOperation;                                                   }
{        Закрывает последнею начатую операцию. Должен вызываться в том   }
{        случае если операция завершена успешно.                         }
{    3)PostInfo(AText: string);                                          }
{        Создаёт информационное сообщение. Прикрепляется к текущей       }
{        операции.                                                       }
{    4)PostWarning(AText: string);                                       }
{        Создаёт предупреждающие сообщение. Так же прикрепляется к       }
{        текущей операции.                                               }
{    5)PostError(AText: string);                                         }
{        Создаёт сообщение об ошибке. Завершает текущую операцию на      }
{        форме с пометкой что в ней произошла ошибка(изменяется иконка у }
{        узла), есле операция была с признаком критичности рекурсивно    }
{        поднимается выше по дереву(до не критичной операции) так же     }
{        помечая ошибкой. Если поднимится до корня(первой операции) то   }
{        завершает индикацию и выводит сообщение о неудачно выполненом   }
{        процессе.                                                       }
{    6)SetProgressBarPosition(CurrentPosition, MaxPosition: integer);    }
{        Отображает на форме ProgressBar. Параметрами задаём текущее     }
{        и максимальное значение.                                        }
{************************************************************************}

unit brs_ProcessForm;

interface

uses
  Windows, SysUtils, Graphics, Forms, MSXML2_TLB, ComObj, ImgList,
  Controls, StdCtrls, ComCtrls, ExtCtrls, Classes, Messages, 
  Registry, PlaningTools_TLB, uFMAddinGeneralUtils;

const
  //Ключи реестра
  regBasePath = '\SOFTWARE\Krista\FM\ExcelAddIn';
  regAutoCloseProcessForm = 'AutoCloseProcessForm';

  csTime = '  Время: ';
  cStartTime = 'Начало: ';
  cEndTime = 'Конец: ';
  cDuration = 'Длительность: ';

  //индексы иконок для узлов
  iiInfo = 2;
  iiWarning = 3;
  iiError = 5;
  iiTime = 4;
  iiProcessComplete = 7;

  WM_OPENOPERATION     = WM_USER + 1;
  WM_CLOSEOPERATION    = WM_USER + 2;
  WM_POSTINFO          = WM_USER + 3;
  WM_POSTWARNING       = WM_USER + 4;
  WM_POSTERROR         = WM_USER + 5;
  WM_PBARPOSITION      = WM_USER + 6;
  WM_SETTITLE          = WM_USER + 7;

type
  TOperationType = (otQuery, otProcess, otSave, otUpdate, otView, otMap, otUser);

  TPostType = (ptWarning, ptInfo, ptError);

  pProcessInfo = ^TProcessInfo;
  TProcessInfo = record
    OperationName : string;
    Critical : boolean;
    NoteTime : boolean;
    OperType: TOperationType;
    PostText : string;
    PBarCurrentPosition : integer;
    PBarMaxPosition: integer;
    TitleProcess: string;
    SuccessMessage: string;
    ErrorMessage: string;
    OnReturnHandler: procedure of object;
    OnCloseHandler: procedure of object;
    OnShowHandler: procedure of object;
  end;

  TDataNode = class
  private
    {Поля узла операции }
    FCritical: boolean;
    FNoteTime: boolean;
    FOperationType: TOperationType;
    FComplete: boolean;
    FError: boolean;
    {Поля узла времени}
    FStartTime: TTime;
    FEndTime: TTime;
    function GetNodeImageIndex: integer;
  public
    //если операция завершена с ошибкой помечаем здесь
    property Error: boolean read FError write Ferror Default false;
    //признак критичности в случае ошибки
    property Critical: boolean read FCritical write FCritical;
    //состояние операции
    property Complete: boolean read FComplete write FComplete Default false;
    //признак нужности засечки времени при выполнении операции
    property NoteTime: boolean read FNoteTime write FNoteTime;
    //тип операции
    property OperType: TOperationType read FOperationType write FOperationType;
    //зависит от типа опрации и состояния завершености 
    property ImageIndex: integer read  GetNodeImageIndex;
    {Св-ва и методы узла времени}
    property StartTime: TTime read FStartTime write FStartTime;
    property EndTime: TTime read FEndTime write FEndTime;
    //возвращает продолжительность операции, исходя из StartTime и EndTime
    function Duration: TTime;
  end;

  TfrmProcess = class(TForm)
    tvProcess: TTreeView;
    btClose: TButton;     
    ImageList1: TImageList;
    cbAutoCloseForm: TCheckBox;
    pMessageBar: TPanel;
    ProgressBar: TProgressBar;
    btReturn: TButton;
    procedure btCloseClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure btReturnClick(Sender: TObject);
    procedure FormShow(Sender: TObject);
  private
    FTreeRoot: TTreeNode;           //корень дерева
    FCurrentNode: TTreeNode;        //узел текущей операции
    FWithoutErrorComplite: boolean; //результат процесса
    FOnceError: boolean;            //взводится если была хоть одна ошибка
    FOnceWarning: boolean;
    FPBarStep: integer;
    FErrorList: TStringList;
    FParentHWnd: integer;
    FNewProcessClear: boolean;
    FLastError: string;
    FLogFileName: string;
    FReturnOnClose: boolean;

    procedure SetProgressBarStep(Value: integer);
    //исзодя из максимального количества обрабатываемых элементов определяет
    //частоту вывода информации в MessageBar
    property PBarStep: integer read FPBarStep write SetProgressBarStep;
    //узел текущей операции
    property CurrentNode: TTreeNode read FCurrentNode write FCurrentNode;
    //пытамеся показать форму, возвращаем результат
    function ShowProcessForm: boolean;
    //если первая добовляемая операция возвращаем true
    function OperationFirst: boolean;
    //если все начатые операции завершены возвращаем true
    function IsAllProcessComplete: boolean;
    procedure Clear;
    //помечаем ошибкой все родительские узлы, вплоть до не критичного
    procedure MarkInCriticalNodesError;
    procedure CreateTimeNode;
    //устанавливает индекс картинки в узле, заодно делаем ещё кое что
    procedure SetNodeImageIndex(Node: TTreeNode; ImageIndex: integer);
    //создание информационных узлов
    procedure CreatePostNode(PostType: TPostType; Text: string);
    //подытоживающий узел
    procedure CreateSummedNode;
    //возвращает список всех ошибок произошедших во время процесса
    function GetErrorList: string;
    procedure SetErrorList(Value: string);
    procedure ClearBeforeDestroy;
    procedure SaveProcessLog;
    procedure WriteLogString(Node: TTreeNode);
  protected
    procedure WndProc(var Msg : TMessage); override;
    procedure CreateParams(var Params : TCreateParams); override;
  public
    fProcessInfo: pProcessInfo;
    constructor CreatePrnt(AOwner: TComponent; ParentWnd : integer);
    property ErrorList: string read GetErrorList write SetErrorList;
    property LastError: string read FLastError write FLastError;
    procedure SetProgressBarPosition(CurrentPosition, MaxPosition: integer);
    procedure OpenOperation(Name: string; IsCritical, IsNoteTime: boolean;
      OperationType: TOperationType);
    procedure CloseOperation;
    procedure PostInfo(AText: string);
    procedure PostWarning(AText: string);
    procedure PostError(AText: string);

    property NewProcessClear: boolean read FNewProcessClear
      write FNewProcessClear;
    property LogFileName: string read FLogFileName write FLogFileName;
    property ReturnOnClose: boolean read FReturnOnClose;
  end;

var
  frmProcess: TfrmProcess;
implementation

{$R *.DFM}
{255 - Opaqye, 0 - Transparent}

procedure TFrmProcess.CreateParams(var Params : TCreateParams);
begin
 inherited;
 Params.WndParent := FParentHWnd;
 Params.Style := Params.Style or WS_CLIPSIBLINGS or WS_DLGFRAME or WS_OVERLAPPED;
 Params.ExStyle := WS_EX_LTRREADING or WS_EX_DLGMODALFRAME or WS_EX_WINDOWEDGE;
end;

procedure TfrmProcess.WndProc(var Msg : TMessage);
begin
  if Assigned(fProcessInfo) then
  begin
    case Msg.Msg of
      WM_OPENOPERATION: begin
        OpenOperation(fProcessInfo.OperationName, fProcessInfo.Critical,
          fProcessInfo.NoteTime, fProcessInfo.OperType);
      end;
      WM_CLOSEOPERATION: CloseOperation;
      WM_POSTINFO: PostInfo(fProcessInfo.PostText);
      WM_POSTWARNING: PostWarning(fProcessInfo.PostText);
      WM_POSTERROR: PostError(fProcessInfo.PostText);
      WM_PBARPOSITION:
        SetProgressBarPosition(fProcessInfo.PBarCurrentPosition,
          fProcessInfo.PBarMaxPosition);
      WM_SETTITLE:
        if FNewProcessClear or (frmProcess.Caption = '') then
          frmProcess.Caption := fProcessInfo.TitleProcess;
      WM_DESTROY: begin
        inherited WndProc(Msg);
        ClearBeforeDestroy;
        Exit;
      end;
    end;
  end;
  inherited WndProc(Msg);
end;

procedure TfrmProcess.ClearBeforeDestroy;
begin
  PostQuitMessage(0);
  FreeStringList(FErrorList);
  Clear;
  //Close;
end;

function ReadBoolRegSetting(ValueName: string; DefaultValue: Boolean): Boolean;
var
  Reg: TRegistry;
begin
  result := DefaultValue;
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(regBasePath)) then
    begin
      Reg.OpenKey(regBasePath, false);
      if Reg.ValueExists(ValueName) then
        result := (Reg.ReadInteger(ValueName) <> 0)
      else
      begin
        result := DefaultValue;
        if DefaultValue then
          Reg.WriteInteger(ValueName, 1)
        else
          Reg.WriteInteger(ValueName, 0);
      end;
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

procedure WriteBoolRegSetting(ValueName: string; Value: Boolean);
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(regBasePath, true) then
    begin
      if Value then
        Reg.WriteInteger(ValueName, 1)
      else
        Reg.WriteInteger(ValueName, 0);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

{если строка содержит более 255 символов, то обрезаем её и ставим "..."(признак
 продолжения строки)}
function Cut(S: string): string;
begin
  if (S <> '') and (Length(S) > 255) then
  begin
    Insert('...', S, 252);
    Delete(S, 255, Length(S));
  end;
  result := S;
end;

procedure TfrmProcess.FormCreate(Sender: TObject);
begin
  frmProcess := Self;
  if not Assigned(FErrorList) then
    FErrorList := TStringList.Create;
  Clear;
end;

procedure TfrmProcess.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  if Assigned(fProcessInfo.OnCloseHandler) then
    fProcessInfo.OnCloseHandler;
  Clear;
  SetActiveWindow(FParentHWnd);
end;

procedure TfrmProcess.Clear;
var
 i: integer;
 DataNode: TDataNode;
begin
  for i := 0 to tvProcess.Items.Count - 1 do
    if Assigned(tvProcess.Items.Item[i].Data) then
      try
        DataNode := tvProcess.Items.Item[i].Data;
        if Assigned(DataNode) then
          DataNode.Free;
      except
      end;
  pMessageBar.Caption := '';
  tvProcess.Items.Clear;
  FLastError := '';
  CurrentNode := nil;
  FTreeRoot := nil;
end;

procedure TfrmProcess.SetNodeImageIndex(Node: TTreeNode; ImageIndex: integer);
begin
  if (Node = nil) then
    exit;
  if (ImageIndex < 0) then
    exit;
  Node.ImageIndex := ImageIndex;
  Node.SelectedIndex := ImageIndex;
  //заодно сделаем ещё кое-что...
  pMessageBar.Caption := '';
  ProgressBar.Visible := false;
  Node.MakeVisible;
  Refresh;
end;

procedure TfrmProcess.CreateSummedNode;
var
  Node: TTreeNode;
begin
  pMessageBar.Font.Size := 9;
  pMessageBar.Alignment := taCenter;
  pMessageBar.Font.Style := [fsBold];
  if FWithoutErrorComplite then
  begin
    Node := tvProcess.Items.AddChild(FTreeRoot, FTreeRoot.Text);
    SetNodeImageIndex(Node, iiProcessComplete);
    with pMessageBar do
    begin
      if FOnceError then
      begin
        Font.Color := clRed;
        Caption := fProcessInfo.ErrorMessage;
      end
      else
      begin
        Font.Color := clGreen;
        Caption := fProcessInfo.SuccessMessage;
      end;
    end
  end
  else
  begin
    Node := tvProcess.Items.Add(FTreeRoot, Cut(FLastError));
    SetNodeImageIndex(Node, iiError);
    with pMessageBar do
    begin
      Font.Color := clRed;
      Caption := fProcessInfo.ErrorMessage;
    end
  end;
  FTreeRoot := nil;
  CurrentNode := nil;
end;

function TfrmProcess.IsAllProcessComplete: boolean;
var
  DataNode: TDataNode;
begin
  result := (CurrentNode = FTreeRoot);
  if result then
  begin
    if not Assigned(CurrentNode.Data) then
      exit;
    DataNode := CurrentNode.Data;
    FWithoutErrorComplite := not DataNode.Error;
    btClose.Enabled := true;
    // Возврат в мастер разрешаем только при неудачном обновлении.
    btReturn.Enabled := FOnceError;//true;//not FWithoutErrorComplite;
    CreateSummedNode;
    Application.ProcessMessages;
    SaveProcessLog;
    if not (FOnceError or FOnceWarning) and cbAutoCloseForm.Checked then
      btClose.Click;
  end;
end;

procedure TfrmProcess.OpenOperation(Name: string; IsCritical,
  IsNoteTime: boolean; OperationType: TOperationType);
var
  DataNode: TDataNode;
begin
  // отображаем форму, если не можем, продолжать дальше смысла нет...
  if not ShowProcessForm then
    exit;
  Name := Cut(Name);
  CurrentNode := tvProcess.Items.AddChild(CurrentNode, Name);
  if OperationFirst then
    FTreeRoot := CurrentNode;

  DataNode := TDataNode.Create;
  with DataNode do
  begin
    Critical := IsCritical;
    NoteTime := IsNoteTime;
    OperType := OperationType;
  end;
  CurrentNode.Data := Pointer(DataNode);
  SetNodeImageIndex(CurrentNode, DataNode.ImageIndex);

  if IsNoteTime then
    CreateTimeNode;

  WriteLogString(CurrentNode);
end;

procedure TfrmProcess.CloseOperation;
var
  DataNode: TDataNode;
  FirstChildNode: TTreeNode;
begin
  if CurrentNode = nil then
    exit;

  if not Assigned(CurrentNode.Data) then
    exit;
  DataNode := CurrentNode.Data;
  DataNode.Complete := true;
  SetNodeImageIndex(CurrentNode, DataNode.ImageIndex);

  if DataNode.NoteTime then
  begin
    {если у операции засекали время, надо поставить окончание выполнения и
    продолжительность}
    FirstChildNode := CurrentNode.getFirstChild;
    if FirstChildNode = nil then
      exit;
    if not Assigned(FirstChildNode.Data) then
      exit;
    DataNode := FirstChildNode.Data;
    DataNode.EndTime := Now;
    FirstChildNode.Text := cStartTime + TimeToStr(DataNode.StartTime) + ' ' +
      cEndTime + TimeToStr(DataNode.EndTime) + ' ' + cDuration +
      TimeToStr(DataNode.Duration);
  end;

  if not IsAllProcessComplete then
    CurrentNode := CurrentNode.Parent;
end;

function TfrmProcess.ShowProcessForm: boolean;
var
  r : TRect;
begin
  result := false;
  if not Assigned(frmProcess) then
    exit;
  if not frmProcess.Showing then
  begin
    {позиционируем по расположению родительского окна}
    if (FParentHWnd > 0) then
      GetWindowRect(FParentHWnd, r)
    else
      GetWindowRect(GetDesktopWindow, r);
    MoveWindow(frmProcess.Handle,
      round(r.Left + ((r.Right - r.Left) / 2) - (Width / 2)),
      round(r.Top + ((r.Bottom - r.Top) / 2) - (Height / 2)),
      Width, Height, TRUE);

    frmProcess.Show;
    // смотрим в реестре надо ли скрывать форму при завершении
    cbAutoCloseForm.Checked := ReadBoolRegSetting(regAutoCloseProcessForm, true);
  end;
  Refresh;
  {при индицировании нового процесса, не закрывая формы, очишаем деров}
  if ((FTreeRoot = nil) and NewProcessClear) then
    Clear;
  result := frmProcess.Showing;
end;

procedure TfrmProcess.CreateTimeNode;
var
  Node: TTreeNode;
  DataNode: TDataNode;
  Time: TTime;
begin
  if CurrentNode = nil then
    exit;
  Time := Now;
  Node := tvProcess.Items.AddChildFirst(CurrentNode, cStartTime + TimeToStr(Time));

  DataNode := TDataNode.Create;
  DataNode.StartTime := Time;
  Node.Data := Pointer(DataNode);
  SetNodeImageIndex(Node, iiTime);
end;

procedure TfrmProcess.CreatePostNode(PostType: TPostType; Text: string);
var
  Node: TTreeNode;
begin
  if CurrentNode = nil then
    exit;
  Node := tvProcess.Items.AddChild(CurrentNode, Cut(Text));
  case PostType of
    ptInfo:SetNodeImageIndex(Node, iiInfo);
    ptWarning: begin
      FErrorList.Add('  ' + Text);
      SetNodeImageIndex(Node, iiWarning)
    end;
    ptError:
    begin
      SetNodeImageIndex(Node, iiError);
      FErrorList.Add('  ' + Text);
    end;
  end;
  WriteLogString(Node);
end;

procedure TfrmProcess.MarkInCriticalNodesError;
var
  DataNode: TDataNode;
begin
  if CurrentNode = nil then
    exit;
  if not Assigned(CurrentNode.Data) then
    exit;
  DataNode := CurrentNode.Data;
  DataNode.Error := true;
  SetNodeImageIndex(CurrentNode, iiError);
  if IsAllProcessComplete then
    exit
  else
    CurrentNode := CurrentNode.Parent;
  //если ошибка узла критична поднимаемся выше(к родителям)
  if DataNode.Critical then
    MarkInCriticalNodesError;
end;

procedure TfrmProcess.PostError(AText: string);
begin
  //для информативности к ошибке прибавляем время её происхождения
  FLastError := AText;
  FOnceError := true;
  AText := AText + csTime + TimeToStr(Now);
  CreatePostNode(ptError, AText);
  MarkInCriticalNodesError;
end;

procedure TfrmProcess.PostInfo(AText: string);
begin
  CreatePostNode(ptInfo, AText);
end;

procedure TfrmProcess.PostWarning(AText: string);
begin
  FOnceWarning := true;
  CreatePostNode(ptWarning, AText);
end;

function TfrmProcess.OperationFirst: boolean;
begin
  result := (FTreeRoot = nil);
  if result then
  begin
    btClose.Enabled := false;
    btReturn.Enabled := false;
    FLastError := '';
    FErrorList.Clear;
  end;
end;

procedure TfrmProcess.SetProgressBarStep(Value: integer);
begin
  case Value of
    0..999: FPBarStep := 1;
    1000..9999: FPBarStep := 10;
    10000..100000: FPBarStep := 100;
  else
    FPBarStep := 1000;
  end;
end;

procedure TfrmProcess.SetProgressBarPosition(CurrentPosition, MaxPosition: integer);

  function MessageBarView: boolean;
  begin
    result := ((CurrentPosition mod PBarStep) = 0) or
      (MaxPosition < (CurrentPosition + 30)) or (CurrentPosition = 1);
  end;

var
  i: real;
begin
  if not tvProcess.Showing then
    exit;
  if (MaxPosition < CurrentPosition) or (MaxPosition <= 0) then
    exit;
  if not ProgressBar.Visible then
  begin
    ProgressBar.Visible := true;
    PBarStep := MaxPosition;
    with pMessageBar do
    begin
      Caption := '';
      Font.Size := 8;
      Font.Color := clBlack;
      Alignment := taLeftJustify;
      Font.Style := [];
    end;
  end;

  if MessageBarView then
  begin
    pMessageBar.Caption := 'Обрабатывается: ' + IntToStr(CurrentPosition) + '  из '+
      IntToStr(MaxPosition) + ' элементов';
    i := 100 / MaxPosition * CurrentPosition;
    ProgressBar.Position := Round(i);
  end;
  //по завершению скрываем 
  if (CurrentPosition = MaxPosition) then
    ProgressBar.Visible := false;
end;

{  TDataNode}

function TDataNode.Duration: TTime;
var
  Hour, Min, Sec, Msec: word;
begin
  result := 0;
  if ((StartTime <> 0) and (EndTime <> 0)) then
  begin
    // округляем время начала и конца операции до секунд(раньше было до Мсек)
    DecodeTime(StartTime, Hour, Min, Sec, Msec);
    StartTime := EncodeTime(Hour, Min, Sec, 0);
    DecodeTime(EndTime, Hour, Min, Sec, Msec);
    EndTime := EncodeTime(Hour, Min, Sec, 0);
    result := EndTime - StartTime;
  end;
end;

function TDataNode.GetNodeImageIndex: integer;
begin   
  if Complete then
    case OperType of
      otProcess: result := 12;
      otQuery: result := 10;
      otSave: result := 1;
      otUpdate: result := 14;
      otView: result := 8;
      otMap: result := 16;
      otUser: result := 17;
    else
      result := 0;
    end
  else
    case OperType of
      otProcess: result := 11;
      otQuery: result := 9;
      otSave: result := 1;
      otUpdate: result := 13;
      otView: result := 8;
      otMap: result := 15;
      otUser: result := 17;
    else
      result := 0;
    end
end;

procedure CorrectLastString(var ErrorList: TStringList);
var
  LastString: string;
begin
  if not(Assigned(ErrorList) and (ErrorList.Count > 0)) then
    exit;
  try
    LastString := ErrorList.Strings[ErrorList.Count - 1];
    if (LastString <> '') and (LastString[Length(LastString)] = ';') then
      LastString[Length(LastString)] := '.';
    ErrorList.Strings[ErrorList.Count - 1] := LastString;
  except
  end;
end;

function TfrmProcess.GetErrorList: string;
begin
  result := '';
  if Assigned(FErrorList) then
  begin
    if (Trim(FLastError) <> '') and (FErrorList.Count > 0) then
      if FErrorList[0] <> FLastError + ':' then
        FErrorList.Insert(0, FLastError + ':');
    {если в последней строке, в конце сообщения стоит ';' заменяем на '.'}
    CorrectLastString(FErrorList);
    result := FErrorList.CommaText;
  end;
end;

procedure TfrmProcess.SetErrorList(Value: string);
begin
  if Assigned(FErrorList) then
    FErrorList.CommaText := Value;
end;

constructor TfrmProcess.CreatePrnt(AOwner: TComponent; ParentWnd: integer);
begin
  NewProcessClear := true;
  FOnceError := false;
  FParentHWnd := ParentWnd;
  Inherited Create(AOwner);
  FReturnOnClose := false;
end;

procedure TfrmProcess.SaveProcessLog;
var
  LogFile: TextFile;
  i, j: integer;
  s: string;
begin
  if LogFileName = '' then
    exit;
  if tvProcess.Items.Count = 0 then
    exit;

  AssignFile(LogFile, LogFileName);
  try
    if FileExists(LogFileName) then
    begin
      Append(LogFile);
      Writeln(LogFile, #13#10#13#10);
      Writeln(LogFile, 'Копия индикатора процессов');
    end
    else
      Rewrite(LogFile);
  except
    exit;
  end;

  try
    try
      Writeln(LogFile, DateTimeToStr(Now));
      for i := 0 to tvProcess.Items.Count - 1 do
        with tvProcess.Items[i] do
        begin
          s := Text;
          for j := 0 to Level do
            s := '  ' + s;
          Writeln(LogFile, s);
        end;
      Writeln(LogFile, #13#10#13#10);
    except
    end;
  finally
    CloseFile(LogFile);
  end;
end;

procedure TfrmProcess.WriteLogString(Node: TTreeNode);
var
  LogFile: TextFile;
  i: integer;
  s: string;
begin
  if LogFileName = '' then
    exit;
  AssignFile(LogFile, LogFileName);
  try
    if FileExists(LogFileName) then
      Append(LogFile)
    else
      Rewrite(LogFile);
  except
    exit;
  end;

  try
    try
      s := DateTimeToStr(Now) + ': ' + Node.Text;
      for i := 1 to Node.Level do
        s := '  ' + s;
      Writeln(LogFile, s);
    except
    end;
  finally
    CloseFile(LogFile);
  end;
end;

procedure TfrmProcess.btReturnClick(Sender: TObject);
begin
  if Assigned(fProcessInfo.OnReturnHandler) then
    fProcessInfo.OnReturnHandler;
  WriteBoolRegSetting(regAutoCloseProcessForm, cbAutoCloseForm.Checked);
  Close;
end;

procedure TfrmProcess.btCloseClick(Sender: TObject);
begin
  WriteBoolRegSetting(regAutoCloseProcessForm, cbAutoCloseForm.Checked);
  Close;
end;

procedure TfrmProcess.FormShow(Sender: TObject);
begin
  if Assigned(fProcessInfo.OnShowHandler) then
    fProcessInfo.OnShowHandler;
end;

end.
