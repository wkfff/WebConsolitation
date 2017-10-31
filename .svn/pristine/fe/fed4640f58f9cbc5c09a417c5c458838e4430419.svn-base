unit brs_AdoDB_Utils;

interface

uses windows, ADODB_TLB, ActiveX, comobj, sysutils, brs_GeneralFunctions, FMDTSPumping_TLB,
  classes;

type TProviderType = (ptUnknown, ptInterbase, ptOracle);

function  GetProviderType(const Con : connection) : TProviderType;
function  GetNextGenValue(const Con : connection; const GenName : string; out GenValue : integer) : boolean;

function  GetErrors(const Err : Errors) : string;
procedure ShowErrors(const Err : Errors);
procedure ShowProperties(Props : properties);
procedure ShowParameters(const Cmd : command);

function  CreateADOConnection : _connection;
procedure CloseADOConnection(const Con : _connection);
function  CreateRecordSet(CursorLocation : TOleEnum) : _recordset;
procedure CloseRecordset(const rs : _recordset);
function  CreateSpecialRecordSet(cnt : Connection; CursorLocation : TOleEnum) : _recordset;
function  FindRecord(const rs : _recordset; const Criteria : string) : boolean;
function  FindRecordByFilter(const rs : _recordset; const Filter : string) : boolean;

function AppendBLOBParamFromFile(const cmd : command; const FileName : string) : string;
function AppendTimeParamToCmd(const cmd : command; const Val : TDateTime) : string;
function AppendBlobParamToCmd(const cmd : command; const Val : olevariant) : string;
function FillCMDParametersByRecordsetFields(const cmd : command; const SourceRS : recordset;
const IgnoreIDField : boolean) : string;
function ResyncCMDParametersByRecordsetFields(const cmd : command; const SourceRS : recordset;
const IgnoreIDField : boolean) : string;

function OpenConnection(const Con : connection; const Logger : IUnknown;
  const PumpID : integer; const UDLFilePath : widestring) : string;
function ReopenConnection(const Con : connection; const Logger : IUnknown;
  const PumpID : integer; const ConnectStr : widestring) : string;

implementation

function GetErrors(const Err : Errors) : string;
var i : integer;
    tmpStr : string;
    tmpErr : error;
begin
  result := '';
  if not Assigned(Err) then Exit;
  tmpStr := '';
  for i := 0 to Err.Count - 1 do begin
    tmpErr := Err[i];
    result := result + IntToStr(i) + ') ' + string(tmpErr.Description) + #10#13;
  end;
end;

procedure ShowErrors(const Err : Errors);
var tmpStr : string;
begin
  tmpStr := GetErrors(Err);
  MessageBox(0, PChar(tmpStr), 'ADODB.Errors', MB_OK)
end;

procedure ShowProperties(Props : properties);
var tmpStr : string;
    i : integer;
begin
  tmpStr := '';
  for i := 0 to Props.Count - 1 do
    tmpStr := tmpStr + Props.Item[i].Name + #10#13;
  MessageBox(0, PChar(tmpStr), 'ADODB.Properties', MB_OK)
end;

function GetProviderType(const Con : connection) : TProviderType;
var tmpProp : property_;
    tmpStr : string;
begin
  result := ptUnknown;
  try
    // Вот по этому свойству будем определять тип провайдера
    // (не знаю насколько это правильно)
    tmpProp := Con.Properties.Item['DBMS Name'];
  except
  end;
  if not Assigned(tmpProp) then exit;
  tmpStr := trim(VarToStr(tmpProp.Value));
  if AnsiCompareText(tmpStr, 'Oracle') = 0 then begin
    result := ptOracle;
    exit
  end;
  if AnsiCompareText(tmpStr, 'Interbase') = 0 then begin
    result := ptInterbase;
    exit
  end
end;

function GetNextGenValue(const Con : connection; const GenName : string; out GenValue : integer) : boolean;
const IBGenQueryMask     = 'SELECT GEN_ID(%S, 1) ID FROM RDB$DATABASE';
      OracleGenQueryMask = 'SELECT %S.NEXTVAL ID FROM DUAL';
var RecordsAffected : OleVariant;
    tmpRS : recordset;
    QueryStr : string;
    ErrStr : string;
begin
  GenValue := -1;
  result := false;
  case GetProviderType(Con) of
    ptInterbase : QueryStr := format(IBGenQueryMask, [GenName]);
    ptOracle    : QueryStr := format(OracleGenQueryMask, [GenName])
    else exit
  end;
  try
    {$WARNINGS OFF}
    tmpRS := Con.Execute(QueryStr, RecordsAffected, adOptionUnspecified);
    {$WARNINGS ON}
  except
    ErrStr := GetOleSysErrorMsg;
    raise Exception.Create(ErrStr);
  end;
  result := Assigned(tmpRS) and (tmpRS.State = adStateOpen) and (tmpRS.RecordCount > 0);
  if result then begin
    tmpRS.MoveFirst;
    GenValue := tmpRS.Fields[0].Value;
    CloseRecordSet(tmpRS)
  end;
end;

function CheckRecordSet(const rs : _recordset) : boolean;
begin
  result := (rs.State <> adStateClosed) and (rs.RecordCount > 0);
end;

function FindRecord(const rs : _recordset; const Criteria : string) : boolean;
begin
  result := CheckRecordSet(rs);
  if not result then exit;
  rs.MoveFirst;
  rs.Find(Criteria, 0, adSearchForward, EmptyParam);
  result := not rs.EOF
end;

// подозрительная процедура, тормозить наверное будет не-падецки
function FindRecordByFilter(const rs : _recordset; const Filter : string) : boolean;
var bm : variant;
begin
  result := CheckRecordSet(rs);
  if not result then exit;
  rs.Filter := Filter;
  result := rs.RecordCount <> 0;
  if result then bm := rs.Bookmark;
  rs.Filter := adFilterNone;
  if result then rs.Bookmark := bm
end;

function CreateADOConnection : _connection;
begin
  result := CreateComObject(CLASS_CONNECTION) as _connection;
  result.IsolationLevel := adXactCursorStability;
end;

function CreateRecordSet(CursorLocation : TOleEnum) : _recordset;
begin
  result := CreateComObject(CLASS_RECORDSET) as _recordset;
  result.CursorLocation := CursorLocation
end;

procedure CloseRecordset(const rs : _recordset);
begin
  if Assigned(rs) and (rs.State <> adStateClosed) then rs.Close;
end;

function  CreateSpecialRecordSet(cnt : Connection; CursorLocation : TOleEnum) : _recordset;
begin
  result := CreateComObject(CLASS_RECORDSET) as _recordset;
  result.Set_ActiveConnection(cnt);
  result.CursorType := adOpenForwardOnly;
  result.LockType := adLockBatchOptimistic;
  result.CursorLocation := CursorLocation;
end;

procedure CloseADOConnection(const Con : _connection);
begin
  if Assigned(Con) and (Con.State <> adStateClosed) then Con.Close
end;

function OpenConnection(const Con : connection; const Logger : IUnknown;
  const PumpID : integer; const UDLFilePath : widestring) : string;
begin
  result := '';
  try
    Con.Open('File Name=' + UDLFilePath, '', '', 0);
  except
    result := format('Невозможно установить подключение к базе данных : %s',
      [EOleSysError(ExceptObject).Message]);
    CallWriteEventToPumpReport(Logger, 'File Name=' + UDLFilePath, ekPumpError, PumpID, result);
  end;
end;

function ReopenConnection(const Con : connection; const Logger : IUnknown;
  const PumpID : integer; const ConnectStr : widestring) : string;
begin
  CloseADOConnection(Con);
  result := '';
  try
    Con.Open(ConnectStr, '', '', 0);
  except
    result := format('Невозможно установить подключение к базе данных : %s',
      [EOleSysError(ExceptObject).Message]);
    CallWriteEventToPumpReport(Logger, ConnectStr, ekPumpError, PumpID, result);
  end;
end;


const CMDParamName = 'Param';

function AppendBLOBParamFromFile(const cmd : command; const FileName : string) : string;
var hFile      : THANDLE;
    ParamValue : variant;
    FileSize   : integer;
    pFileData  : pointer;
    BytesRead  : cardinal;
    Prm        : parameter;
    PrmName    : widestring;
begin
  pFileData := nil; FileSize := 0;
  try
    // Проверяем наличие файла
    if not FileExists(FileName) then begin
      result := format('Файл "%s" не найден', [FileName]);
      raise Exception.Create(result)
    end;
    // Пытаемся открыть файл на чтение
    hFile := CreateFile(PChar(FileName), GENERIC_READ, FILE_SHARE_READ, nil, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
    if hFile = INVALID_HANDLE_VALUE then begin
      result := format('Не удалось открыть файл "%s"', [FileName]);
      raise Exception.Create(result)
    end;
    // Получаем размер файла и резервируем под него память
    FileSize := GetFileSize(hFile, nil);
    GetMem(pFileData, FileSize);
    // Пытаемся прочитать файл
    if not ReadFile(hFile, pFileData^, FileSize, BytesRead, nil)
     then result := format('Ошибка чтения из файла "%s"', [FileName]);
    // Закрываем описатель файла
    CloseHandle(hFile);
    // Если чтение не удалось - исключение
    if Result <> '' then raise Exception.Create(result);
    // Создаем вариантный массив необходимого размера
    ParamValue := VarArrayCreate([0, FileSize - 1], varByte);
    VarArrayLock(ParamValue);
    // копируем туда содержимое файла
    move(pFileData^, PVarArray(TVarData(ParamValue).VArray).Data^, FileSize);
    VarArrayUnLock(ParamValue);
    PrmName := CMDParamName + IntToStr(cmd.Parameters.Count);
    try
      // создаем параметр и добавляем его в коллекцию
      Prm := cmd.CreateParameter(PrmName, adBinary, adParamInput, FileSize - 1, ParamValue);
      cmd.Parameters.Append(Prm)
    except
      result := GetOleSysErrorMsg;
      raise Exception.Create(result)
    end
  finally
    // освобождаем память
    if Assigned(pFileData) then FreeMem(pFileData, FileSize);
    if not VarIsEmpty(ParamValue) then VarClear(ParamValue);
  end
end;


function AppendTimeParamToCmd(const cmd : command; const Val : TDateTime) : string;
var tmpVar : OleVariant;
    prm : parameter;
    PrmName : string;
begin
  result := '';
  try
    tmpVar := Val;
    PrmName := CMDParamName + IntToStr(cmd.Parameters.Count);
    prm := cmd.CreateParameter(PrmName, adDBTimeStamp, adParamInput, SizeOf(TTimeStamp), tmpVar);
    cmd.Parameters.Append(prm);
  except
    result := GetOleSysErrorMsg
  end
end;

function AppendBlobParamToCmd(const cmd : command; const Val : olevariant) : string;
var prm : parameter;
    PrmName : string;
begin
  result := '';
  try
    PrmName := CMDParamName + IntToStr(cmd.Parameters.Count);
    prm := cmd.CreateParameter(PrmName, adLongVarBinary, adParamInput,
      VarArrayHighBound(Val, 1) - VarArrayLowBound(Val, 1) + 1, Val);
    cmd.Parameters. Append(prm);
  except
    result := GetOleSysErrorMsg
  end
end;

function FillCMDParametersByRecordsetFields(const cmd : command; const SourceRS : recordset;
  const IgnoreIDField : boolean) : string;
var i : integer;
    prm : parameter;
    CurField : field;
    PrmName : string;
begin
  result := '';
  try
    while cmd.Parameters.Count > 0 do cmd.Parameters.Delete(0);
    for i := 0 to  SourceRS.Fields.Count - 1 do begin
      CurField := SourceRS.Fields[i];
      if not (IgnoreIDField and (AnsiCompareText(string(CurField.Name), 'ID') = 0)) then begin
        PrmName := CMDParamName + '_' + string(CurField.Name);
        prm := cmd.CreateParameter(PrmName, CurField.Type_, adParamInput, CurField.DefinedSize, CurField.Value);
        prm.Precision := CurField.Precision;
        prm.NumericScale := CurField.NumericScale;
        cmd.Parameters.Append(prm)
      end
    end
  except
    result := GetOleSysErrorMsg
  end
end;

function ResyncCMDParametersByRecordsetFields(const cmd : command; const SourceRS : recordset;
  const IgnoreIDField : boolean) : string;
var i : integer;
    PrmName : string;
begin
  result := '';
  try
    for i := 0 to SourceRS.Fields.Count - 1 do
      if not (IgnoreIDField and (AnsiCompareText(string(SourceRS.Fields[i].Name), 'ID') = 0)) then begin
        PrmName := CMDParamName + '_' + string(SourceRS.Fields[i].Name);
        cmd.Parameters[PrmName].Value := SourceRS.Fields[i].Value;
      end;
  except
    result := GetOleSysErrorMsg
  end
end;

procedure ShowParameters(const Cmd : command);
var i : integer;
    tmpStrL : TStringList;
begin
  tmpStrL := TStringList.Create;
  try
    for i := 0 to cmd.Parameters.Count - 1 do
      tmpStrL.Add(format('%s : %s', [string(cmd.Parameters[i].Name), VarToStr(cmd.Parameters[i].Value)]));
    ShowErrorMessage(0, tmpStrL.Text)
  finally
    FreeAndNil(tmpStrL)
  end
end;

end.
