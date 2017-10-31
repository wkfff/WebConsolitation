unit brs_AdoMD_Utils;

interface

uses windows, sysutils, ActiveX, adomd_tlb, brs_GeneralFunctions, classes, brs_AdoDB_Utils,
  {$IFDEF DEBUG}
  dialogs,
  {$ENDIF}
  adodb_tlb;


type
  //  фукции и типы для выборки значений из cellset'a
  pDimsArray = ^TDimsArray;
  TDimsArray = array[0..32767] of integer;
  function InitializeDims(AxesCount : integer; var Dims : pDimsArray; var AccVar : variant) : boolean;
  function FinalizeDims(var Dims : pDimsArray; var AccVar : variant) : boolean;
  function GetCellValue(cst: cellset; const Dims : pDimsArray; AccVar : pvariant;
    AxesCount : integer; const NeedFormatString : boolean; var FormatString : widestring): variant;
  function GetCellFormattedValue(cst: cellset; const Dims : pDimsArray; AccVar : pvariant; AxesCount : integer): widestring;
  //
  function FormattedValueToVariant(const Val : string) : variant;

  procedure CloseCatalog(var Cat : ICatalog);
  procedure GetMDStoragesList(const con : connection; const MDStoragesList : TStrings);

  {$IFDEF DEBUG}
  // Отладочные
  procedure ShowProperties(const ipElem : variant);
  {$ENDIF}

implementation

procedure CloseCatalog(var Cat : ICatalog);
var Con : connection;
begin
  if Assigned(Cat) and Assigned(Cat.Get_ActiveConnection) then begin
    Con := cat.Get_ActiveConnection as Connection;
    if Assigned(Con) and (Con.State = adStateOpen) then Con.Close;
    Cat.Set_ActiveConnection(nil);
  end;
  Cat := nil;
end;

var FormatBufferSec : TRTLCriticalSection;
    FormatBuffer    : array [0..255] of char;
    FormatStr       : string;
    CurCharInd      : integer;

function InitializeDims(AxesCount : integer; var Dims : pDimsArray; var AccVar : variant) : boolean;
begin
  result := AxesCount > 0;
  if result then begin
    GetMem(Dims, AxesCount * SizeOf(integer));
    AccVar := VarArrayCreate([0, AxesCount - 1], VarVariant);
  end
end;

function FinalizeDims(var Dims : pDimsArray; var AccVar : variant) : boolean;
begin
  try
    FreeMem(Dims);
    Dims := nil;
    VarClear(AccVar);
    result := true
  except
    result := false
  end
end;

function GetItemRef(cst: cellset; const Dims : pDimsArray; AccVar : pvariant; AxesCount : integer) : cell;
var i : integer;
begin
  for i := 0 to AxesCount - 1 do
    AccVar^[i] := Dims^[i];
  try
    result := cst.Item[PSafeArray(TVarData(AccVar^).VArray)];
//    ShowProperties(result)
  except
    result := nil
  end;
end;

function GetCellValue(cst: cellset; const Dims : pDimsArray; AccVar : pvariant;
  AxesCount : integer; const NeedFormatString : boolean; var FormatString : widestring): variant;
var item : cell;
    prop : property_;
begin
  result := NULL;
  try
    item := GetItemRef(cst, Dims, AccVar, AxesCount);
    if Assigned(item) then begin
      result := item.Get_Value;
      if NeedFormatString and (FormatString = '') then begin
        try
          prop := item.properties['FORMAT_STRING'];
        except
          prop := nil;
        end;
        if Assigned(prop) then FormatString := widestring(VarToStr(prop.Value))
      end
    end
  finally
    item := nil;
    prop := nil
  end
end;

function GetCellFormattedValue(cst: cellset; const Dims : pDimsArray; AccVar : pvariant; AxesCount : integer): widestring;
begin
  try
    result := GetItemRef(cst, Dims, AccVar, AxesCount).FormattedValue
  except
    // пустые запросы
    result := ''
  end
end;

function FormattedValueToVariant(const Val : string) : variant;
var i : integer;
    IsDouble : boolean;
    CurChar : char;
begin
  EnterCriticalSection(FormatBufferSec);
  try
    result := NULL;
    CurCharInd := 0;
    IsDouble := false;
    for i := 1 to Length(Val) do begin
      CurChar := Val[i];
      if IsNum(CurChar) or IsSign(CurChar) then begin
        FormatBuffer[CurCharInd] := CurChar;
        if CurCharInd < 255 then inc(CurCharInd)
                            else break;
      end else
      if (IsDecimalSeparator(CurChar)) and (not IsDouble) then begin
        FormatBuffer[CurCharInd] := CurChar;
        if CurCharInd < 255 then inc(CurCharInd)
                            else break;
        IsDouble := true;
      end;
    end;
    if (CurCharInd > 0) and (CurCharInd <= 255) then begin
      SetLength(FormatStr, CurCharInd);
      move(FormatBuffer[0], FormatStr[1], CurCharInd);
      case IsDouble of
        false : result := StrToInt(FormatStr);
        true  : result := StrToFloat(FormatStr)
      end
    end
  finally
    LeaveCriticalSection(FormatBufferSec)
  end
end;

{$IFDEF DEBUG}
procedure ShowProperties(const ipElem : variant);
var i : integer;
    tmpStrL : TStringList;
begin
  tmpStrL := TStringList.Create;
  try
    tmpStrl.Add('Properties : ');
    for i := 0 to ipElem.Properties.Count - 1 do
      tmpStrL.Add(format('  %s = %s', [string(ipElem.Properties[i].Name), VarToStr(ipElem.Properties[i].Value)]));
    showmessage(tmpStrL.Text)
  finally
    FreeAndNil(tmpStrL)
  end
end;
{$ENDIF}

procedure GetMDStoragesList(const con : connection; const MDStoragesList : TStrings);
var tmpRS : recordset;
begin
  if (not Assigned(MDStoragesList)) or (not Assigned(con)) or (Con.State <> adStateOpen) then exit;
  MDStoragesList.Clear;
  try
    tmpRs := Con.OpenSchema(adSchemaCatalogs, EmptyParam, EmptyParam);
    if tmpRS.RecordCount > 0 then begin
      tmpRS.MoveFirst;
      while not tmpRS.EOF do begin
        MDStoragesList.Add(VarToStr(tmpRS.Fields['CATALOG_NAME'].Value));
        tmpRS.MoveNext
      end
    end;
   // else ShowErrors(Con.Errors)
  finally
    tmpRS.Close;
  end
end;

initialization
  InitializeCriticalSection(FormatBufferSec);
finalization
  DeleteCriticalSection(FormatBufferSec)
end.
