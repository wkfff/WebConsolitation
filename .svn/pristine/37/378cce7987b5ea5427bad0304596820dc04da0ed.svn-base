{
  Библиотечный модуль.
  Процедуры общего назначения. (не XML, не Excel, не Реестр, вобщем не
  специализированные).
  (!) В этом модуле особенно важно написать дельный камент при объявлении
  функции.
}
unit uFMAddinGeneralUtils;

interface

uses
  uRusDlg, Dialogs, uDetailForm, SysUtils, Controls, Windows, ShlObj,
  uFMExcelAddinConst, Classes, ComCtrls, uFMAddinRegistryUtils,
  uGlobalPlaningConst, Math, ActiveDs_TLB;

  {Условное присваивание целого}
  function IIF(Condition: boolean; ThenRes, ElseRes: integer): integer; overload;
  {Условное присваивание строк}
  function IIF(Condition: boolean; ThenRes, ElseRes: string): string; overload;
  {Откусываем до сепаратора, что откусили возвращаем, исходную строку укорачиваем}
  function CutPart(var Tail: string; Separator: string): string;
  {Лобавляем фрагмент в конец непустой строки}
  procedure AddTail(var Src: string; Tail: string);
  {Добавляем фрагмент в начало непустой строки}
  procedure AddHead(Head: string; var Src: string);
  {Вывод информационных сообщений различного характера}
  procedure ShowError(Msg: string);
  procedure ShowDetailError(Error, DetailError, Caption: widestring);
  procedure ShowWarning(Msg: string);
  function ShowWarningEx(Msg: string): boolean;
  procedure ShowInfo(Msg: string);
  procedure DebugShowInfo(Msg: string);
  function ShowQuestion(Msg: string): boolean;
  function ShowQuestionDelSC(Msg: string): integer;  
  {Булевское в Да/Нет}
  function BoolToYesNo(Value: boolean): string;
  {Да/Нет в булевское}
  function YesNoToBool(Value: string): boolean;
  {Булевское в строку 'true'/'false'}
  function BoolToStr(Flag: Boolean): string;
  {Скобки для мембера (квадратные)}
  function MemberBrackets(Src: string): string;
  {Скобки кoртежа (круглые)}
  function TupleBrackets(Src: string): string;
  {Скобки множества (фигурные)}
  function SetBrackets(Src: string): string;
  {Сравниваем даты
   формат даты "dd.mm.yyyy hh:mm:ss"
   возвращает (-1) - Date1 < Date2
   возвращает (0) - Date1 = Date2
   возвращает (1) - Date1 > Date2}
  function CompareDates(Date1, Date2: string): integer;
  {Попытка преобразования строки к дробному типу}
  function IsNumber(Value: string): boolean;
  {Диалог выбора каталога}
  function SelectDir(ParentWnd: THandle):string;
  {Дозвращает версию плагина по значениям констант}
  function GetAddinVersion: string;
  {Кодирует слэш и кавычки}
  procedure EncodeXPathString(var XPath: string);
  {Кодировать имя свойства элемента}
  procedure EncodeMemberPropertyName(var Name: string);
  {При клике на надпись в CheckBox-e ставится(убирается) галочка}
  function ListViewOnMouseUpChangeCheckState(Sender: TObject; Button: TMouseButton;
    Shift: TShiftState; X, Y: Integer): TListItem;
  {Округление вещественных чисел до заданного числа разрядов после запятой}
  function RoundEx(ANumber: extended; Precision: integer): extended;
  {Замочить StringList}
  procedure FreeStringList(var StringList: TStringList);
  {Замочить List}
  procedure FreeList(var List: TList);
  {Записывает отработанный MDX-запрос в лог.
   При этом проверяются соответствующие реестровые настройки,
   так что снаружи можно просто вызывать. Если лог вести не нужно,
   ничего записываться небудет.}
  procedure UpdateMDXLog(Query: string; BookName, ProviderId: string);
  function MergeStringList(Var SList1, SList2: TStringList): TStringList;
  function MergeCommaText(Text1, Text2: string): string;
  function ConvertStringToCommaText(AString: string): string;
  function CommaTextToString(SourceString: widestring): widestring;
  {Закодировать недопустимые в имени файла символы}
  function EncodeFileName(FileName: string): string;
  {Раскодировать имя файла}
  function DecodeFileName(FileName: string): string;
  {Возвращает строку как MDX-идентификатор меры }
  function StrAsMeasure(Src: string): string;
  {формирует новый список конкатенацией строк исходных списков (все со всеми)}
  function StringListCrossJoin(List1, List2: TStringList; Delimiter: string): TStringList;
  {Операторы с помощью которых конструируются формулы в Excel переводи на русский}
  function EnToRus(const Text: string): string;
  { Не все контролы передают свое свойство Enabled по наследству.
    Данная процедура делает это "вручную".}
  procedure EnableChildControls(WinControl: TWinControl; Value: boolean);
  {Получить /доменное/ имя текущего пользователя}
  function GetCurrentUserName: string;
  {Получить имя компьютера}
  function GetComputerName: string;
  function SimpleRoundTo(const AValue: Extended; const ADigit: integer): Extended;
  function GetLastErrorLoc : string;

  procedure SaveStringToFile(FileName: string; Source: string);
  function  Min(const a, b: double): double; overload;
  function  IntMin(const a, b: integer): integer; overload;
  function  Max(const a, b: double): double; overload;
  function  IntMax(const a, b: integer): integer; overload;

implementation

function IIF(Condition: boolean; ThenRes, ElseRes: integer): integer;
begin
  if Condition then
    result := ThenRes
  else
    result := ElseRes;
end;

function IIF(Condition: boolean; ThenRes, ElseRes: string): string;
begin
  if Condition then
    result := ThenRes
  else
    result := ElseRes;
end;

function CutPart(var Tail: string; Separator: string): string;
var
  p: integer;
begin
  result := '';
  if Tail <> '' then
  begin
    p := Pos(Separator, Tail);
    if p = 0 then // откусываем последнюю часть.
    begin
      result := Tail;
      Tail := '';
    end
    else
    begin
      result := Copy(Tail, 1, p - 1);
      Delete(Tail, 1, p + length(Separator) - 1);
    end;
  end;
end;

procedure AddTail(var Src: string; Tail: string);
begin
  if Src <> '' then
    Src := Src + Tail;
end;

procedure AddHead(Head: string; var Src: string);
begin
  if Src <> '' then
    Src := Head + Src;
end;

procedure ShowError(Msg: string);
begin
  RusMessageDlg(Msg, mtError, [mbOk], 0, -1);
end;

procedure ShowDetailError(Error, DetailError, Caption: widestring);
var
  DetailForm: TDetailForm;
begin
  DetailForm := TDetailForm.Create(nil);
  try
    DetailForm.Error := Error;
    DetailForm.DetailError := DetailError;
    DetailForm.Caption := Caption;
    DetailForm.ShowModal;
  finally
    FreeAndNil(DetailForm);
  end;
end;

procedure ShowWarning(Msg: string);
begin
  RusMessageDlg(Msg, mtWarning, [mbOk], 0, -1);
end;

function ShowWarningEx(Msg: string): boolean;
begin
  result := RusMessageDlg(Msg, mtWarning, [mbOk, mbCancel], 0, -1) = mrOK;
end;

procedure ShowInfo(Msg: string);
begin
  RusMessageDlg(Msg, mtInformation, [mbOk], 0, -1);
end;

procedure DebugShowInfo(Msg: string);
begin
  RusMessageDlg(Msg, mtInformation, [mbOk], 0, -1);
end;

function ShowQuestion(Msg: string): boolean;
begin
  result := (RusMessageDlg(Msg, mtConfirmation, [mbYes, mbNo], 0, -1) = mrYes);
end;

{запрос нужет пока только в ворде, Удалить отдельные ячйки?}
function ShowQuestionDelSC(Msg: string): integer;
var
  BC: TButtonsCaptions;
begin
  SetLength(BC, 3);
  BC[0] := 'Да';
  BC[1] := 'Нет';
  BC[2] := 'Все копии';
  result := RusMessageDlg_(Msg, mtConfirmation, [mbYes, mbNo, mbYesToAll],
    BC, 0, -1);
  SetLength(BC, 0);
end;

function BoolToYesNo(Value: boolean): string;
begin
  if Value then
    result := 'yes'
  else
    result := 'no';
end;

function YesNoToBool(Value: string): boolean;
begin
  result := (Value = 'yes');
end;

function BoolToStr(Flag: Boolean): string;
begin
  if Flag then
    result := 'true'
  else
    result := 'false';
end;

function MemberBrackets(Src: string): string;
begin
  result := '[' + Src + ']';
end;

function TupleBrackets(Src: string): string;
begin
  result := '(' + Src + ')';
end;

function SetBrackets(Src: string): string;
begin
  result := '{' + Src + '}';
end;

function CompareDates(Date1, Date2: string): integer;
var
  Day1, Day2, Month1, Month2, Year1, Year2: string;
  Hour1, Hour2, Minute1, Minute2, Second1, Second2: string;
begin
  result := 0;
  // проверяем ситуации с неопределенными значениями дат
  if (Date1 = 'null') and (Date2 <> 'null') then
  begin
    result := -1;
    exit;
  end;
  if (Date1 = Date2) then
  begin
    result := 0;
    exit;
  end;
  if (Date1 <> 'null') and (Date2 = 'null') then
  begin
    result := 1;
    exit;
  end;
  // получаем параметры даты
  // получаем день
  Day1 := CutPart(Date1, '.');
  Day2 := CutPart(Date2, '.');
  // получаем месяц
  Month1 := CutPart(Date1, '.');
  Month2 := CutPart(Date2, '.');
  // получаем год
  Year1 := CutPart(Date1, ' ');
  Year2 := CutPart(Date2, ' ');
  // получаем час
  Hour1 := CutPart(Date1, ':');
  Hour2 := CutPart(Date2, ':');
  // получаем минуту
  Minute1 := CutPart(Date1, ':');
  Minute2 := CutPart(Date2, ':');
  // получаем секунду
  Second1 := Date1;
  Second2 := Date2;
  // сравниваем параметры даты
  // сравниваем год
  try
    if (StrToInt(Year1) < StrToInt(Year2)) then
      result := -1;
    if (StrToInt(Year1) > StrToInt(Year2)) then
      result := 1;
    if (result <> 0) then
      exit;
    // сравниваем месяц
    if (StrToInt(Month1) < StrToInt(Month2)) then
      result := -1;
    if (StrToInt(Month1) > StrToInt(Month2)) then
      result := 1;
    if (result <> 0) then
      exit;
    // сравниваем день
    if (StrToInt(Day1) < StrToInt(Day2)) then
      result := -1;
    if (StrToInt(Day1) > StrToInt(Day2)) then
      result := 1;
    if (result <> 0) then
      exit;
    // сравниваем час
    if (StrToInt(Hour1) < StrToInt(Hour2)) then
      result := -1;
    if (StrToInt(Hour1) > StrToInt(Hour2)) then
      result := 1;
    if (result <> 0) then
      exit;
    // сравниваем минуту
    if (StrToInt(Minute1) < StrToInt(Minute2)) then
      result := -1;
    if (StrToInt(Minute1) > StrToInt(Minute2)) then
      result := 1;
    if (result <> 0) then
      exit;
    // сравниваем секунду
    if (StrToInt(Second1) < StrToInt(Second2)) then
      result := -1;
    if (StrToInt(Second1) > StrToInt(Second2)) then
      result := 1;
  except
    result := -1;
  end;
end;

function IsNumber(Value: string): boolean;
begin
  result := true;
  try
    StrToFloat(Value);
  except
    result := false;
  end;
end;

function SelectDir(ParentWnd: THandle):string;
var
  TitleName : string;
  lpItemID : PItemIDList;
  BrowseInfo : TBrowseInfo;
  DisplayName : array[0..MAX_PATH] of char;
  TempPath : array[0..MAX_PATH] of char;

begin
  result:='';
  FillChar(BrowseInfo, sizeof(TBrowseInfo), #0);
  BrowseInfo.hwndOwner := ParentWnd;
  BrowseInfo.pszDisplayName := @DisplayName;
  TitleName := 'Укажите каталог';
  BrowseInfo.lpszTitle := PChar(TitleName);
  BrowseInfo.ulFlags := BIF_RETURNONLYFSDIRS;
  lpItemID := SHBrowseForFolder(BrowseInfo);
  if lpItemId <> nil then
  begin
    SHGetPathFromIDList(lpItemID, TempPath);
    result:=TempPath;
    GlobalFreePtr(lpItemID);
  end;
end;

function GetAddinVersion: string;
begin
  result := fmMajorVersion + '.' + fmMinorVersion + '.' + fmRelease;
end;

procedure EncodeXPathString(var XPath: string);
begin
  Xpath := StringReplace(XPath, '\', '\\', [rfReplaceAll]);
  Xpath := StringReplace(XPath, '"', '\"', [rfReplaceAll]);
end;

procedure EncodeMemberPropertyName(var Name: string);
begin
  Name := StringReplace(Name, ' ', 'Char20', [rfReplaceAll]);
  Name := StringReplace(Name, '(', 'Char40', [rfReplaceAll]);
  Name := StringReplace(Name, ')', 'Char41', [rfReplaceAll]);
end;

function ListViewOnMouseUpChangeCheckState(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer): TListItem;
begin
  result := nil;
  try
    if (Button = mbleft) then
    begin
      result := (Sender as TListView).GetItemAt(X, Y);
      result.Checked := not result.Checked;
    end;
  except
  end;
end;

function RoundEx(ANumber: extended; Precision: integer): extended;
var
  Factor: Extended;
begin
  Factor := Int(Exp(Precision * Ln(10)));
  result := Round(Factor * ANumber) / Factor;
end;

procedure FreeStringList(var StringList: TStringList);
begin
  if not Assigned(StringList) then
    exit;
  try
    StringList.Clear;
    FreeAndNil(StringList);
  except
  end;
end;

procedure FreeList(var List: TList);
begin
  if not Assigned(List) then
    exit;
  try
    List.Clear;
    FreeAndNil(List);
  except
  end;
end;

procedure UpdateMDXLog(Query: string; BookName, ProviderId: string);
const
  Delimiter = '---------------------------------------------------'#13#10;
var
  fMDXLog: TextFile;
  LogPath: string;
begin
  if AddinLogEnable then
  try
    LogPath := AddinLogPath + 'MDXLog.txt';
    AssignFile(fMDXLog, LogPath);

    if FileExists(LogPath) then
      Append(fMDXLog)
    else
      Rewrite(fMDXLog);

    WriteLn(fMDXLog, DateTimeToStr(Now));
    WriteLn(fMDXLog, 'Компьютер: ' + GetComputerName);
    WriteLn(fMDXLog, 'Пользователь: ' + GetCurrentUserName);
    WriteLn(fMDXLog, 'Книга: ' + BookName);
    WriteLn(fMDXLog, 'Провайдер: ' + ProviderId);
    WriteLn(fMDXLog, ' ');
    WriteLn(fMDXLog, Query);
    WriteLn(fMDXLog, Delimiter);
  except
  end;

  {$I-}
  CloseFile(fMDXLog);
  {$I+}
end;

function MergeStringList(Var SList1, SList2: TStringList): TStringList;
var
  i: integer;
begin
  if (Assigned(SList1) and Assigned(SList2)) then
  begin
    result := SList1;
    if SList2.Count > 0 then
      result.Add('');
    for i := 0 to SList2.Count - 1 do
      result.Add(SList2.Strings[i]);
    FreeStringList(SList2);
  end
  else
    if (Assigned(SList1) and not Assigned(SList2)) then
      result := SList1
    else
      if (Assigned(SList2) and not Assigned(SList1)) then
        result := SList2
      else
        result := nil;
end;

function MergeCommaText(Text1, Text2: string): string;
begin
  if ((Text1 <> '') and (Text2 <> '')) then
    result := Text1 + ', ,' + Text2
  else
    if (Text1 <> '') then
      result := Text1
    else
      result := Text2;
end;

function ConvertStringToCommaText(AString: string): string;
begin
  if AString = '' then
    result := '""'
  else
    result := AnsiQuotedStr(AString, '"');
end;

function CommaTextToString(SourceString: widestring): widestring;
var
  i: integer;
  TempList: TStrings;
begin
  result := '';
  SourceString := StringReplace(SourceString, ' ', '|', [rfReplaceAll]); 
  TempList := TStringList.Create;
  try
    TempList.CommaText := SourceString;
    for i := 0 to TempList.Count - 1 do
      result := result + TempList.Strings[i] + ',';
    result := copy(result, 1, Length(result) - 1);
    result := StringReplace(result, '|', ' ', [rfReplaceAll]);
  finally
    FreeAndNil(TempList);
  end;
end;

function EncodeFileName(FileName: string): string;
begin
  result := FileName;
  result := StringReplace(result, '/', '&2f;', [rfReplaceAll]);
  result := StringReplace(result, '\', '&5c;', [rfReplaceAll]);
  result := StringReplace(result, '|', '&7c;', [rfReplaceAll]);
  result := StringReplace(result, ':', '&3a;', [rfReplaceAll]);
  result := StringReplace(result, '*', '&2a;', [rfReplaceAll]);
  result := StringReplace(result, '?', '&3f;', [rfReplaceAll]);
  result := StringReplace(result, '"', '&22;', [rfReplaceAll]);
  result := StringReplace(result, '<', '&3c;', [rfReplaceAll]);
  result := StringReplace(result, '>', '&3e;', [rfReplaceAll]);
end;

function DecodeFileName(FileName: string): string;
begin
  result := FileName;
  result := StringReplace(result, '&2f;', '/', [rfReplaceAll]);
  result := StringReplace(result, '&5c;', '\', [rfReplaceAll]);
  result := StringReplace(result, '&7c;', '|', [rfReplaceAll]);
  result := StringReplace(result, '&3a;', ':', [rfReplaceAll]);
  result := StringReplace(result, '&2a;', '*', [rfReplaceAll]);
  result := StringReplace(result, '&3f;', '?', [rfReplaceAll]);
  result := StringReplace(result, '&22;', '"', [rfReplaceAll]);
  result := StringReplace(result, '&3c;', '<', [rfReplaceAll]);
  result := StringReplace(result, '&3e;', '>', [rfReplaceAll]);
end;

function StrAsMeasure(Src: string): string;
begin
  result := '';
  if Src <> '' then
    result := '[MEASURES].' + MemberBrackets(Src);
end;

function StringListCrossJoin(List1, List2: TStringList; Delimiter: string): TStringList;
var
  i, j: integer;
  NoList1, NoList2: boolean;
begin
  NoList1 := not Assigned(List1);
  if not NoList1 then
    NoList1 := List1.Count = 0;

  NoList2 := not Assigned(List2);
  if not NoList2 then
    NoList2 := List2.Count = 0;

  result := TStringList.Create;
  if NoList1 then
    if NoList2 then
      FreeStringList(result)
    else
      result.Assign(List2)
  else
    if NoList2 then
      result.Assign(List1)
    else
      for i := 0 to List1.Count - 1 do
        for j := 0 to List2.Count - 1 do
          result.Add(List1[i] + Delimiter + List2[j]);
end;

function EnToRus(const Text: string): string;
begin
  result := Text;
  result := StringReplace(result, 'ISLOGICAL', 'ЕЛОГИЧ', [rfReplaceAll]);
  result := StringReplace(result, 'ISNONTEXT', 'ЕНЕТЕКСТ', [rfReplaceAll]);
  result := StringReplace(result, 'ISNUMBER',  'ЕЧИСЛО', [rfReplaceAll]);
  result := StringReplace(result, 'ISBLANK',   'ЕПУСТО', [rfReplaceAll]);
  result := StringReplace(result, 'ISERROR',   'ЕОШИБКА', [rfReplaceAll]);
  result := StringReplace(result, 'ISTEXT',    'ЕТЕКСТ', [rfReplaceAll]);
  result := StringReplace(result, 'ROUND',     'ОКРУГЛ', [rfReplaceAll]);
  result := StringReplace(result, 'ISERR',     'ЕОШ', [rfReplaceAll]);
  result := StringReplace(result, 'ISREF',     'ЕССЫЛКА', [rfReplaceAll]);
  result := StringReplace(result, 'ISNA',      'ЕНД', [rfReplaceAll]);

  result := StringReplace(result, 'FALSE', 'ЛОЖЬ', [rfReplaceAll]);
  result := StringReplace(result, 'TRUE',  'ИСТИНА', [rfReplaceAll]);
  result := StringReplace(result, 'AND',   'И', [rfReplaceAll]);
  result := StringReplace(result, 'NOT',   'НЕ', [rfReplaceAll]);
  result := StringReplace(result, 'IF',    'ЕСЛИ', [rfReplaceAll]);
  result := StringReplace(result, 'OR',    'ИЛИ', [rfReplaceAll]);
end;

procedure EnableChildControls(WinControl: TWinControl; Value: boolean);
var
  i: integer;
  Control: TControl;
begin
  for i := 0 to WinControl.ControlCount - 1 do
  begin
    Control := WinControl.Controls[i];
    Control.Enabled := Value;
    if Control is TWinControl then
      EnableChildControls(TWinControl(Control), Value);
  end;
end;

function GetCurrentUserName: string;
var
  Info: IADsWinNTSystemInfo;
  DomainName: string;
begin
  result := '';
  Info := CoWinNTSystemInfo.Create;
  if not Assigned(Info) then
    exit;
  try
    try
      DomainName := Info.DomainName;
      result := Info.UserName;
      if DomainName <> '' then
        result := DomainName + '\' + result;
    except
      ;
    end;
  finally
    Info := nil;
  end;
end;

function GetComputerName: string;
var
  Info: IADsWinNTSystemInfo;
begin
  result := '';
  Info := CoWinNTSystemInfo.Create;
  if not Assigned(Info) then
    exit;
  try
    try
      result := Info.ComputerName;
    except
      ;
    end;
  finally
    Info := nil;
  end;
end;

procedure SaveStringToFile(FileName: string; Source: string);
var
  fs: textFile;
begin
  AssignFile(fs, FileName);
  Rewrite(fs);
  Write(fs, Source);
  CloseFile(fs);
end;

function SimpleRoundTo(const AValue: Extended; const ADigit: integer): Extended;
var
  LFactor, e: Extended;
begin
  { Увы, но без шаманства с очень маленьким допуском обойтись не удалось -
    Trunc пошаливает, да-с...}
  e := 0.5 + 1e-16;
  LFactor := IntPower(10, -ADigit);
  if AValue < 0 then
    Result := Trunc((AValue / LFactor) - e) * LFactor
  else
    Result := Trunc((AValue / LFactor) + e) * LFactor;
end;

function GetErrorLocFromHRESULT(const HR : HRESULT) : string;
const BuffSize = MAX_PATH - 1;
var   Win32ErrBuff : array [0..BuffSize] of char;
begin
  ZeroMemory(@Win32ErrBuff, SizeOf(Win32ErrBuff));
  FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, nil, HR, LANG_NEUTRAL, @Win32ErrBuff, SizeOf(Win32ErrBuff), nil);
  result := StrPas(@Win32ErrBuff)
end;

function GetLastErrorLoc : string;
begin
  result := GetErrorLocFromHRESULT(GetLastError)
end;

function  IntMin(const a, b : integer) : integer;
begin
  if a >= b then result := b else result := a
end;

function  IntMax(const a, b : integer) : integer;
begin
  if a >= b then result := a else result := b
end;

function  Min(const a, b : double) : double;
begin
  if (a - b > MinDouble) then result := b else result := a
end;

function  Max(const a, b : double) : double;
begin
  if (a - b > MinDouble) then result := a else result := b
end;

end.
