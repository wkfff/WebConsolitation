{
  ������������ ������.
  ������������ ������ � ��������� ������� MSExcel ������������� � �����
  ������������.
  (!) � ���� ������ �������� ����� �������� ������� ������ ��� ����������
  �������.
}

unit uFMAddinExcelUtils;

interface

uses
  uFMExcelAddinConst, ExcelXP, OfficeXP, Classes, uFMAddinGeneralUtils, SysUtils,
  Windows, uXMLUtils, MSXML2_TLB, uFMAddinXmlUtils, uExcelUtils,
  uGlobalPlaningConst, ZLibEx;

type
  TByteSet = set of byte;

  {���������� true, ���� ��� �������� ��� �������}
  function IsNameOurs(AName: string): boolean;
  {���������� true, ���� ��� �������� ��� ���������������� �������}
  function IsUserNameOurs(AName: string): boolean;
  {���������� true, ���� � CP ����� ���� fm_metadataXML}
  function IsPlaningSheet(ExcelSheet: ExcelWorksheet): boolean;
  {��������� ���������� �����, � ����� � ���������� �������.
   � ������, ���� ���� �� ���, �� ���� ������ false}
  function IsNeedUpdateSheet(ExcelSheet: ExcelWorksheet;
    out Relation: TVersionRelation): boolean;
  {������������ ObjType � Id �� ������������ �����}
  procedure ExtractParamsOfExcelName(AName: string; out ObjType, Id: string);
  {������ �����}
  function ParseExcelName(AName: string; var Params: TStringList): boolean;
  {������ ��� ������� ��� ��� ��������� Worksheet.Names
  � ��������� ����� ������ ��������� ������� "krista" (���� ��� ���)}
  function BuildExcelName(BasicName: string): string;
  {������ ��� ������� ���������������� ��� ��� ��������� Worksheet.Names}
  function BuildUserExcelName(BasicName: string): string;
  {�������� ���� �� �����, ���������� ������ �����, ������� ��� ������� ��������}
  function MarkObject(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
    AName: string; IsVisible: boolean): integer;
  {�������� ���������������� ���� �� �����, ���������� ������ �����, �������
  ��� ������� ��������, ���� IsBuildName = true ��������� �����������������
  ����� ������� ������_}
  function MarkUserObject(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
    AName: string; IsBuildName: boolean): integer;
  {�������� ��� �� ������}
  function ConstructName(ObjType, Id, Param1, Param2: string): string;
  {�������� ��� �� ������ ����������}
  function ConstructNameByParams(Params: TStringList): string;
  {���� �������� ����������� ���������� ����� ����� true}
  function IsRangeBelongName(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
     Name: string): boolean;
  {���������� ���, ����������� � ��}
  function GetDataFromCP(ExcelSheet: ExcelWorksheet; CPName: string): IXMLDOMDocument2;
  procedure PutDataInCP(ExcelSheet: ExcelWorksheet; CPName: string;
    Data: IXMLDOMDocument2); overload;
  procedure PutDataInCP(ExcelSheet: ExcelWorksheet; CPName: string;
    Data: IXMLDOMDocument2; ForceCreate: boolean); overload;
  function IsSheetProtected(const Sheet: ExcelWorksheet): boolean;
  {������������� ��� ������� ������ � �����}
  function SetSheetProtection(ExcelSheet: ExcelWorksheet; State: boolean): boolean;
  {������������� ��� ������� ������ �� ���� ������ � �����}
  function SetBookProtection(Book: ExcelWorkbook; State: boolean): boolean;
  {������������� ��� ������� ������ � ����� ������ � ������ ������������}
  function ProtectPlaningSheets(Book: ExcelWorkbook; State: boolean): boolean;
  { �������� �� ������� ������ �� ������ (RC)
   ������� ������ ���� �������, �� ���� ����� �� ������!!!}
  function GetFormulaCellRefs(Formula: string): TStringList;
  {��������� �� FormulaArray}
  function CheckFormulaArray(Range: ExcelRange; var FormulaArray: string): boolean;
  function IsExistsFormula(Cell: ExcelRange): boolean;
  {�������� �������� � ������� ������; ��������� - ������ ������� � ���������}
  function GetCellValue(ExcelSheet:ExcelWorkSheet; Row, Column: integer;
    EmptyValue: string; out CellValue, Formula, Style: string;
    EraseEmptyCells: boolean): boolean; overload;
  function GetCellValue(CellRange: ExcelRange;
    EmptyValue: string; out CellValue, Formula, Style: string;
    EraseEmptyCells: boolean): boolean; overload;
  {������� �� �� ������ �����}
  function GetExcelSheetVersion(Sheet: ExcelWorksheet): string;
  {����������� ��� CustomProperties �����. ��� ������ ������� - �����
   ������������� �������� (�p ������ ����������� ��� ��������������),
   ������������ ��� ������� � � �������.}
  procedure RecreateCp(ExcelSheet: ExcelWorkSheet);
  {������� � ����� ��� ������������ �����}
  procedure DeleteExcelNames(Sheet: ExcelWorksheet);
  procedure DeleteAllowEditRanges(Sheet: ExcelWorkSheet);
  {�������� ������ ������ �����}
  function GetGrandSummaryCellRange(ExcelSheet: ExcelWorksheet;
    AxisType: TAxisType): ExcelRange;
  {�������� ����� ������ �������� �����, ���� ��� ������ 0}
  function GetGrandSummaryRow(ExcelSheet: ExcelWorksheet): integer;
  {�������� ����� ������� �������� �����, ���� ��� ������ 0}
  function GetGrandSummaryColumn(ExcelSheet: ExcelWorksheet): integer;
  {���������� ��������� �������� ����� ������������, ������� �����������
    ������ ��������}
  function GetRangeOwners(Range: ExcelRange): TPSObjects;
  {���������� true ���� �������� ����������� �������}
  function TableSelected(ExcelSheet: ExcelWorksheet; Target: ExcelRange): boolean;
  {���������� true ���� �������� ����������� �������� �������}
  function BreakPointSelected(ExcelSheet: ExcelWorksheet;
    Target: ExcelRange): boolean;
  {���� �������� ���������� ������� ������� ���������� true}
  function IntersectTableAndRange(ExcelSheet: ExcelWorksheet;
    Target: ExcelRange): boolean;
  {���������� �������� �������, ����������� �� ������ ������ �����}
  function GetExtendedTableRange(ExcelSheet: ExcelWorkSheet): ExcelRange;
  {����� �� ������� ������� �� ���������� ���������}
  function IsTypeFormulaAllowed(Target: ExcelRange; out ErrorText: string): boolean;
  function ByteSetToString(Value: TByteSet): string;
  function StringToByteSet(Value: string): TByteSet;
  {�� ����������� ���������� ���������� �������� �� ������ ������}
  function IsSummaryCell(ExcelSheet: ExcelWorksheet; Row, Column: integer): boolean;
  function IsSummaryColumn(ExcelSheet: ExcelWorkSheet; Column: integer): boolean;
  function IsSummaryRow(ExcelSheet: ExcelWorkSheet; Row: integer): boolean;
  function GetRowsLeafMemberName(ExcelSheet: ExcelWorksheet; Row: integer): string;
  function GetColumnsLeafMemberName(ExcelSheet: ExcelWorksheet; Column: integer): string;
  function IsSequelSheet(ExcelSheet: ExcelWorkSheet): boolean;
  { ����������� � ���������� ������ StringReplace � ���������� ��������� ������,
    ����� ������ ������ Cxxxx �������� ��� ������ CCxxxx}
  function ReplaceCellRef(Template, WhatPattern, WithPattern: string): string;
  {���������� ������ ��� ������������ �����}
  procedure AutoFitMerged(MergedRange: ExcelRange);
  function IsSingleCellName(AName: string): boolean;
  
implementation

function ByteSetToString(Value: TByteSet): string;
var
  b: byte;
begin
  result := '';
  for b := 0 to 255 do
    if (b in Value) then
    begin
      AddTail(result, ',');
      result := result + IntToStr(b);
    end;
end;

function StringToByteSet(Value: string): TByteSet;
var
  Part: string;
begin
  result := [];
  while Value <> '' do
  begin
    Part := CutPart(Value, ',');
    Include(result, Lo(StrToInt(Part)));
  end;
end;

{������� � ����� ��� ������������ �����}
procedure DeleteExcelNames(Sheet: ExcelWorksheet);
var
  Countname, i, j: integer;
  Name: ExcelXP.Name;
begin
  if not Assigned(Sheet) then
    exit;
  try
    {������� ����}
    CountName := Sheet.Names.Count;
    j := 1;
    for i := 1 to CountName do
    begin
      Name := Sheet.Names.Item(j, 0, 0);
      if (IsNameOurs(Name.Name_) and not IsSingleCellName(Name.Name_))
        or IsUserNameOurs(Name.Name_) then
        Name.Delete
      else
        inc(j);
    end;
  except
  end;
end;

function IsSingleCellName(AName: string): boolean;
begin
  result := false;
  if not IsNameOurs(AName) then
    exit;
  result := (Pos(snSeparator + sntSingleCellMeasure + snSeparator, AName) > 0) or
    (Pos(snSeparator + sntSingleCellResult + snSeparator, AName) > 0) or
    (Pos(snSeparator + sntSingleCellConst + snSeparator, AName) > 0);
end;

procedure DeleteAllowEditRanges(Sheet: ExcelWorkSheet);
var
  i: integer;
begin
  if not Assigned(Sheet) then
    exit;
  for i := Sheet.Protection.AllowEditRanges.Count downto 1 do
    if IsNameOurs(Sheet.Protection.AllowEditRanges[i].Title) then
      Sheet.Protection.AllowEditRanges[1].Delete;

end;

function IsNameOurs(AName: string): boolean;
var
  ExclamIndex, PrefixIndex: integer;
begin
  PrefixIndex := pos(snNamePrefix + snSeparator, AName);
  ExclamIndex := pos('!', AName);
  result := (PrefixIndex = 1) or (PrefixIndex = ExclamIndex + 1);
end;

function IsUserNameOurs(AName: string): boolean;
var
  ExclamIndex, PrefixIndex: integer;
begin
  PrefixIndex := pos(snUserNamePrefix + snSeparator, AName);
  ExclamIndex := pos('!', AName);
  result := (PrefixIndex = 1) or (PrefixIndex = ExclamIndex + 1) or
    (AName = sntImpotrArea);
end;

procedure ExtractParamsOfExcelName(AName: string; out ObjType, Id: string);
begin
  // ���� ��� �� ����, �� �������
  if not IsNameOurs(AName) then
    exit;
  // �������� ������� "Krista"
  CutPart(AName, snNamePrefix + snSeparator);
  // �������� Id
  ObjType := CutPart(AName, snSeparator);
  Id := CutPart(AName, snSeparator);
end;

function ParseExcelName(AName: string; var Params: TStringList): boolean;
var
  Part: string;
begin
  // ���� ��� �� ����, �� �������
  result := IsNameOurs(AName);
  if not result then
    exit;
  if Assigned(Params) then
    Params.Clear
  else
    Params := TStringList.Create;
  // �������� ������� "Krista"
  CutPart(AName, snNamePrefix + snSeparator);
  // �������� ��������� ���������
  Part := CutPart(AName, snSeparator);
  while (Part <> '') do
  begin
    Params.Add(Part);
    Part := CutPart(AName, snSeparator);
  end;
end;

function BuildExcelName(BasicName: string): string;
begin
  if (Trim(BasicName) = '') then
  begin // ������ ���� �� ������.
    result := '';
    exit;
  end;
  result := IIF(IsNameOurs(BasicName), '', snNamePrefix + snSeparator) +
    BasicName;
end;

function MarkObject(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
  AName: string; IsVisible: boolean): integer;
begin
  result := -1;
  if Assigned(Range) and (AName <> '') then
  begin
    AName := BuildExcelName(AName);
    {��� ����� ������������ ����������� ������ ����������,
    ������� � ������������� ������������� ������}
    ExcelSheet.Names.Add(EmptyParam, Range, IsVisible, EmptyParam, EmptyParam,
      EmptyParam, AName, EmptyParam, EmptyParam, EmptyParam, EmptyParam);
    result := ExcelSheet.Names.Count;
  end;
end;

function BuildUserExcelName(BasicName: string): string;
begin
  if (Trim(BasicName) = '') then
  begin // ������ ���� �� ������.
    result := '';
    exit;
  end;
  result := IIF(IsUserNameOurs(BasicName), '', snUserNamePrefix + snSeparator) +
    BasicName;
end;

function MarkUserObject(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
  AName: string; IsBuildName: boolean): integer;
begin
  result := -1;
  if Assigned(Range) and (AName <> '') then
  begin
    if IsBuildName then
      AName := BuildUserExcelName(AName);
    {��� ����� ������������ ����������� ������ ����������,
    ������� � ������������� ������������� ������}
    ExcelSheet.Names.Add(EmptyParam, Range, true, EmptyParam, EmptyParam,
      EmptyParam, AName, EmptyParam, EmptyParam, EmptyParam, EmptyParam);
    result := ExcelSheet.Names.Count;
  end;
end;

function ConstructName(ObjType, Id, Param1, Param2: string): string;
begin
  result := ObjType + snSeparator + Id;
  if Param1 <> '' then
    result := result + snSeparator + Param1;
  if Param2 <> '' then
    result := result + snSeparator + Param2;
  result := BuildExcelName(result);
end;

function ConstructNameByParams(Params: TStringList): string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Params.Count - 1 do
  begin
    AddTail(result, snSeparator);
    result := result + Params.Strings[i];
  end;
  result := BuildExcelName(result);
end;

function IsRangeBelongName(ExcelSheet: ExcelWorksheet; Range: ExcelRange;
  Name: string): boolean;
var
  NameRange: ExcelRange;
begin
  try
    Name := BuildExcelName(Name);
    NameRange := GetRangeByName(ExcelSheet, Name);
    if not Assigned(NameRange) then
      result := false
    else
      result := IsNestedRanges(Range, NameRange);
  except
    result := false;
  end;
end;

function GetDataFromCP(ExcelSheet: ExcelWorksheet; CPName: string): IXMLDOMDocument2;
var
  CP: CustomProperty;
  Value: string;
begin
  result := nil;
  if not Assigned(ExcelSheet) then
    exit;
  CP := GetCPByName(ExcelSheet, CPName, false);
  if Assigned(CP) then
  begin
    try
      Value := ZDecompressStr(CP.Value);
    except
      Value := CP.Value;
    end;
    GetDOMDocument(result);
    try
      result.loadXML(Value);
    except
    end;
  end;
end;

procedure PutDataInCP(ExcelSheet: ExcelWorksheet; CPName: string;
  Data: IXMLDOMDocument2);
begin
  PutDataInCP(ExcelSheet, CPName, Data, false);
end;

procedure PutDataInCP(ExcelSheet: ExcelWorksheet; CPName: string;
  Data: IXMLDOMDocument2; ForceCreate: boolean);
var
  CP: CustomProperty;
  Value: string;
begin
  if not Assigned(ExcelSheet) then
    exit;
  CP := GetCPByName(ExcelSheet, CPName, ForceCreate);
  if Assigned(CP) then
  begin
    Value := ZCompressStr(Data.xml, zcMax);
    CP.Value := Value;
  end;
end;

function IsSheetProtected(const Sheet: ExcelWorksheet): boolean;
begin
  result := Sheet.ProtectContents[GetUserDefaultLCID];
end;

function SetSheetProtection(ExcelSheet: ExcelWorksheet; State: Boolean): boolean;
var
  LCID{, CopyMode}: integer;
  OldState: boolean;
  //CopyRange: ExcelRange;
begin
  try
    result := false;
    if not Assigned(ExcelSheet) then
      exit;
    LCID := GetUserDefaultLCID;

    OldState := IsSheetProtected(ExcelSheet);
    if OldState = State then
    begin
      result := true;
      exit;
    end;

(*   CopyMode := ExcelSheet.Application.CutCopyMode[LCID];
   if ((CopyMode = xlCopy) or (CopyMode = xlCut)) then
   begin
    CopyRange := ExcelRange(ExcelSheet.Application.Selection[LCID]);
   end;*)

    if State then
    begin
      ExcelSheet.EnableSelection := xlNoRestrictions;
      ExcelSheet.Protect(fmPassword, true, true, true, false, true, true,
        true, false, false, false, true, true, false, false, false);
    end
    else
      ExcelSheet.Unprotect(fmPassword, LCID);

    result := true;
  except
    result := false;
  end;  
end;

function SetBookProtection(Book: ExcelWorkbook; State: boolean): boolean;
var
  i: integer;
  tempSheet: ExcelWorkSheet;
begin
  if not Assigned(Book) then
  begin
    result := false;
    exit;
  end;
  {!!! ��� ������ ������ � �����(������ ������ �� ���� � ������), �����
  ��������� ������ ������� ������������ - �������� �������������}
  result := true;
  for i := 1 to Book.Sheets.Count do
  begin
    tempSheet := GetWorkSheet(Book.Sheets[i]);
    if not Assigned(tempSheet) then
      continue;
    if not SetSheetProtection(tempSheet, State) then
    begin
      result := false;
      exit;
    end;
  end;
end;

function ProtectPlaningSheets(Book: ExcelWorkbook; State: boolean): boolean;
var
  tempSheet: ExcelWorksheet;
  i: integer;
begin
  result := false;
  if not Assigned(Book) then
    exit;
  for i := 1 to Book.Sheets.Count do
  begin
    tempSheet := GetWorkSheet(Book.Sheets[i]);
    if IsPlaningSheet(tempSheet) then
      if not SetSheetProtection(tempSheet, State) then
        exit;
  end;
  result := true;
end;

// �������� �� ������� ������ �� ������ (RC)
// �������� ������ - [$]C[C]|[$]N[N][N][N][N]
// �� ���� ����� �������� � ��������� �������� �����������... �������� G|6, $AB|67 � � �...
// ������� ������ ���� �������, �� ���� ����� �� ������!!!
function GetFormulaCellRefs(Formula: string): TStringList;
var
  i: integer;
  CurChar, CurLexema: string;
  QuoteFlag, ApostropheFlag, IsOtherSheet: boolean;

  // ����������, �������� �� ������� ������� �� ������
  // ������ ������ �� ������: [$]C[C][$]N[N][N][N][N]
  // �- ������, n - �����, [] - ����������������
  function IsCellRef(var Lexema: string): boolean;
  var
    CurIndex: integer;
  begin
    try
      // ����������� ��������� �������� - 2
      if (Length(Lexema) < 2) then
        abort;
      CurIndex := 1;
      // ������ ������ ����� ���� ������ ����� �������
      if not (Lexema[CurIndex] in ['A'..'Z', '$']) then
        abort;
      if (Lexema[CurIndex] in ['$']) then
        inc(CurIndex);
      if (Lexema[CurIndex] in ['A'..'Z']) then
        inc(CurIndex)
      else abort;
      // ��������� �� ������ ������� ������, ������ ���� ��� ������� ��� ������ ��� ������
      if not (Lexema[CurIndex] in ['A'..'Z', '$', '0'..'9']) then
        abort;
      if (Lexema[CurIndex] in ['A'..'Z']) then
        inc(CurIndex);
      Lexema := copy(Lexema, 1, CurIndex - 1) + '|' + copy(Lexema, CurIndex, Length(Lexema));
      inc(CurIndex);
      if (Lexema[CurIndex] in ['$']) then
        inc(CurIndex);
      if (Lexema[CurIndex] in ['0'..'9']) then
        inc(CurIndex)
      else abort;
      while (CurIndex <= Length(Lexema)) do
        if (Lexema[CurIndex] in ['0'..'9']) then
          inc(CurIndex)
        else abort;
      result := true;
    except
      result := false;
    end;
  end;

  // �������� �� ������� � ���������
  function CheckQuotes(var QuoteFlag, ApostropheFlag: boolean; Char: char): boolean;
  begin
    if (Char = '"') then
      QuoteFlag := not QuoteFlag;
    if (Char = '''') then
      ApostropheFlag := not ApostropheFlag;
    result := (QuoteFlag) or (ApostropheFlag);
  end;

begin
  result := TStringList.Create;
  // ��������� �����������
  // ���������� ������ ������ - "="
  QuoteFlag := false;
  ApostropheFlag := false;
  IsOtherSheet := false;
  for i := 2 to Length(Formula) do
  begin
    // �������� ��������� ������
    CurChar := Formula[i];
    // ��������� �� ������� � ���������
    if CheckQuotes(QuoteFlag, ApostropheFlag, CurChar[1]) then
    begin
      CurLexema := '';
      continue;
    end;
    if (CurChar[1] = '"') or (CurChar[1] = '''') then
      continue;
    if (CurChar[1] = '!') then
    begin
      IsOtherSheet := true;
      CurLexema := '';
      continue;
    end;  
    // ����������� ������
    case CurChar[1] of
      // �����������
      '+', '-', '*', '/', '=', '>', '<', ')', ';', ' ', ',', ':', '%':
        begin
          if IsCellRef(CurLexema) then
            if (result.IndexOf(CurLexema) = -1) then
            begin
              if IsOtherSheet then
              begin
                CurLexema := '!' + CurLexema;
                IsOtherSheet := false;
              end;
              result.Add(CurLexema);
            end;
          CurLexema := '';
        end;
      // ����������� ������
      '(': CurLexema := '';
      else
        CurLexema := CurLexema + CurChar;
    end;
  end;
  // ������� ��������� �������
  if IsCellRef(CurLexema) then                
    if (result.IndexOf(CurLexema) = -1) then
    begin
      if IsOtherSheet then
        CurLexema := '!' + CurLexema;
      result.Add(CurLexema);
    end;
end;


function CheckFormulaArray(Range: ExcelRange; var FormulaArray: string): boolean;
begin
  result := false;
  // � ������������� ��������� - FormulaArray ��������� � Formula - �������
  if (Range.Rows.Count = 1) then
    exit;
  if not VarIsNull(Range.HasArray) then
    if not Range.HasArray then
      exit;
  if VarIsNull(Range.FormulaArray) then
    exit;
  FormulaArray := string(Range.FormulaArray);
  if not (pos('=', FormulaArray) = 1) then
    exit;
  result := true;
end;

function IsPlaningSheet(ExcelSheet: ExcelWorksheet): boolean;
var
  CP: CustomProperty;
begin
  try
    CP := GetCPByName(ExcelSheet, cpMDName, false);
    result := Assigned(CP);
  except
    result := false;
  end;
end;

function IsExistsFormula(Cell: ExcelRange): boolean;
begin
  result := false;
  if not Assigned(Cell) then
    exit;
  try
    result := (Pos('=', Cell.Formula) > 0);
  except
    result := false;
  end;
end;

function GetCellValue(ExcelSheet: ExcelWorkSheet; Row, Column: integer;
  EmptyValue: string; out CellValue, Formula, Style: string;
  EraseEmptyCells: boolean): boolean;
var
  CellRange: ExcelRange;
begin
  with ExcelSheet do
    CellRange := Range[Cells.Item[Row, Column], Cells.Item[Row, Column]];
  result := GetCellValue(CellRange, EmptyValue, CellValue, Formula, Style,
    EraseEmptyCells);
end;

function GetCellValue(CellRange: ExcelRange;
    EmptyValue: string; out CellValue, Formula, Style: string;
    EraseEmptyCells: boolean): boolean;
var
  Value: Variant;
  IsError: wordbool;
  ValueInt: integer;
  HexValue: string;
begin
  Style := string(CellRange.Style);
  Value := CellRange.Value2;
  IsError := CellRange.Application.WorksheetFunction.IsError(Value);
  if IsError then
  begin
    ValueInt := TVarData(Value).VInteger;
    HexValue := IntToHex(ValueInt, 0);
    HexValue := Copy(HexValue, Length(HexValue) - 3, 4);
    if (HexValue = '07D7') then
      CellValue := fmCellZeroDivision
    else
      if (HexValue = '07E7') then
        CellValue := fmIncorrectRef
      else
        if (HexValue = '07DF') then
          CellValue := fmCellWrongValue;
  end
  else
    CellValue := string(Value);
  try
    Formula := CellRange.Formula;
  except
    Formula := '';
  end;
  if (CellValue = fmEmptyCell) and EraseEmptyCells then
  begin
    result := true;
    Style := snDataFreeErased;
  end
  else
    result := not IsError and (CellValue <> fmEmptyCell) and (CellValue <> EmptyValue) and (Formula <> EmptyValue);
end;

{������� �� �� ������ �����}
function GetExcelSheetVersion(Sheet: ExcelWorksheet): string;
var
  DOM: IXMLDOMDocument2;
  Node: IXMLDOMNode;
begin
  result := '0.0.0';
  if not Assigned(Sheet) then
    exit;
  try
    DOM := GetDataFromCP(Sheet, cpMDName);
    if not Assigned(DOM) then
      exit;
    Node := DOM.selectSingleNode('metadata/innerdata');
    result := GetStrAttr(Node, attrSheetVersion, '0.0.0');
  finally
    KillDOMDocument(DOM);
  end;
end;

function IsNeedUpdateSheet(ExcelSheet: ExcelWorksheet;
  out Relation: TVersionRelation): boolean;
const
  PartCount = 3;
type
  TVersion = array [1 .. PartCount] of integer;

  function IsNewSheet(Version: TVersion): boolean;
  var
    i: integer;
  begin
    result := true;
    i := 1;
    while (result and (i <= PartCount)) do
    begin
      if Version[i] <> 0 then
        result := false;
      inc(i);
    end;
  end;

  function IsNeedUpdate(SVersion, AVersion : TVersion): boolean;
  var
    i: integer;
  begin
    result := false;
    i := 1;
    while (not result) and (i <= PartCount) do
    begin
      if (SVersion[i] < Aversion[i]) then
      begin
        result := true;
        Relation := svAncient;
      end
      else
        if (SVersion[i] > Aversion[i]) then
        begin
          Relation := svFuture;
          exit;
        end
        else
          Relation := svModern;
      inc(i);  
    end;
  end;

var
  SheetVersion, AddinVersion: TVersion;
  sSheetVersion, sAddinVersion: string;
  i: integer;
begin
  result := false;
  Relation := svModern;

  if not Assigned(ExcelSheet) then
    exit;
  if not IsPlaningSheet(ExcelSheet) then
    exit;

  sSheetVersion := GetExcelSheetVersion(ExcelSheet);
  sAddinVersion := GetAddinVersion;

  for i := 1 to PartCount do
  begin
    SheetVersion[i] := StrToInt(CutPart(sSheetVersion, '.'));
    AddinVersion[i] := StrToInt(CutPart(sAddinVersion, '.'));
  end;
  {���� ��� ����� ����, �� ����������� ��� �� � ����}
  if IsNewSheet(SheetVersion) then
    result := false
  else
    result := IsNeedUpdate(SheetVersion, AddinVersion);
end;

procedure RecreateCp(ExcelSheet: ExcelWorkSheet);
var
  i, Cnt, DupCount: integer;
  NList, VList: TStringList;
begin
  NList := TStringList.Create;
  VList := TStringList.Create;
  try
    Cnt := ExcelSheet.CustomProperties.Count;
    DupCount := 0;
    for i := 1 to Cnt do
      if NList.IndexOf(ExcelSheet.CustomProperties[i].Name) < 0 then
      begin
        NList.Add(ExcelSheet.CustomProperties[i].Name);
        VList.Add(ExcelSheet.CustomProperties[i].Value);
      end
      else
        inc(DupCount);
    while ExcelSheet.CustomProperties.Count > 0 do
      ExcelSheet.CustomProperties[1].Delete;
    for i := 0 to Cnt - DupCount - 1 do
      ExcelSheet.CustomProperties.Add(NList.Strings[i], VList.Strings[i]);
  finally
    FreeStringList(NList);
    FreeStringList(VList);
  end;
end;

function GetGrandSummaryCellRange(ExcelSheet: ExcelWorksheet; AxisType: TAxisType): ExcelRange;
var
  RangeName: string;
begin
  case AxisType of
    axRow: RangeName := snNamePrefix + snSeparator + gsRow;
    axColumn: RangeName := snNamePrefix + snSeparator + gsColumn;
  end;
  result := GetRangeByName(ExcelSheet, RangeName);
end;

function GetGrandSummaryRow(ExcelSheet: ExcelWorksheet): integer;
var
  SummaryRange: ExcelRange;
begin
  result := 0;
  SummaryRange := GetGrandSummaryCellRange(ExcelSheet, axRow);
  if Assigned(SummaryRange) then
    result := SummaryRange.Row;
end;

function GetGrandSummaryColumn(ExcelSheet: ExcelWorksheet): integer;
var
  SummaryRange: ExcelRange;
begin
  result := 0;
  SummaryRange := GetGrandSummaryCellRange(ExcelSheet, axColumn);
  if Assigned(SummaryRange) then
    result := SummaryRange.Column;
end;

function GetRangeOwners(Range: ExcelRange): TPSObjects;

var
  tmpResult: TPSObjects;

  function TestRange(Obj: TPSObject): boolean;
  begin
    result := IsRangeBelongName(Range.WorkSheet, Range, PSObjectNames[Obj]);
    if result then
      Include(tmpResult, Obj);
  end;

begin
  tmpResult := [psoNone];
  try
    TestRange(psoTable);

    {�������� �� ��������� � ������� ��������}
    if TestRange(psoFilterArea) then
    begin
      TestRange(psoFilter);
      exit;
    end;

    {�������� �� ��������� � ������� ��������}
    if TestRange(psoColumnsArea) then
    begin
      if TestRange(psoColumnsAndMPropsArea) then
        TestRange(psoColumns);
      if TestRange(psoColumnTitles) then
        TestRange(psoColumnLevelTitle)
      else
        TestRange(psoColumnDimension);
      TestRange(psoColumnTitlesBreak);
      exit;
    end;

    {�������� �� ��������� � ������� �����/�����������}
    if TestRange(psoRowsTotalsArea) then
    begin
      TestRange(psoRows);
      if TestRange(psoRowTitles) then
        TestRange(psoRowLevelTitle)
      else
        TestRange(psoRowDimension);

      TestRange(psoRowTitlesBreak);

      if TestRange(psoTotals) then
      begin
        TestRange(psoTotalMeasure);
        TestRange(psoTotalFree);
        TestRange(psoTotalResult);
      end;
      if TestRange(psoTotalTitles) then
      begin
        TestRange(psoTotalMeasureTitle);
        TestRange(psoTotalFreeTitle);
        TestRange(psoTotalResultTitle);
      end;

      exit;
    end;

    if TestRange(psoSheetId) then
      exit;
    if TestRange(psoTestMark) then
      exit;
    if TestRange(psoUnitMarker) then
      exit;

    {�������� �� ��������� � ������� ��������}
    if TestRange(psoFiltersBreak) then
      exit;
    if TestRange(psoUnitMarkerBreak) then
      exit;
    if TestRange(psoColumnTitlesBreak) then
      exit;
    if TestRange(psoColumnsBreak) then
      exit;
    if TestRange(psoRowTitlesBreak) then
      exit;
    if TestRange(psoRowsBreak) then
      exit;

  finally
    TestRange(psoSingleCellMeasure);
    TestRange(psoSingleCellResult);
    TestRange(psoSingleCellConst);
    result := tmpResult;
  end;
end;


function TableSelected(ExcelSheet: ExcelWorksheet; Target: ExcelRange): boolean;
var
  Range: ExcelRange;
begin
  result := false;
  Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));
  if not Assigned(Range) then
    exit;
  result := IsNestedRanges(Target, Range);
end;

function BreakPointSelected(ExcelSheet: ExcelWorksheet;
  Target: ExcelRange): boolean;
begin
  result :=
    (IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntFiltersBreak)) or
    IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntColumnsBreak)) or
    IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntRowsBreak)) or
    IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntUnitMarkerBreak))or
    IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntColumnTitlesBreak))or
    IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntRowTitlesBreak)));
end;

function IntersectTableAndRange(ExcelSheet: ExcelWorksheet;
  Target: ExcelRange): boolean;
var
  Range, TableRange: ExcelRange;
begin
  result := false;
  TableRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));
  if not Assigned(TableRange) then
    exit;
  try
    Range := ExcelSheet.Application.Intersect(Target, TableRange, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, 0);
    result := Assigned(Range);
  except
    result := false;
  end;
end;

function GetExtendedTableRange(ExcelSheet: ExcelWorkSheet): ExcelRange;
var
  TableRange: ExcelRange;
  StartRow, EndRow: integer;
begin
  result := nil;
  TableRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));
  if Assigned(TableRange) then
  begin
    StartRow := TableRange.Row;
    EndRow := StartRow + TableRange.Rows.Count - 1;
    result := GetRange(ExcelSheet, StartRow, 1, EndRow, ExcelSheet.Columns.Count);
  end;
end;

function IsTypeFormulaAllowed(Target: ExcelRange; out ErrorText: string): boolean;
var
  ExcelSheet: ExcelWorksheet;
  IsSelRowOrCol: boolean; //������� ��� ������� ������ ������ ��� ������ �������
  AddressTarget: string;
begin
  result := false;
  ErrorText := '';
  if not Assigned(Target) then
  begin
    ErrorText := '�������� �� �������';
    exit;
  end;
  ExcelSheet := Target.Worksheet;

  AddressTarget := GetAddressLocal(Target);
  IsSelRowOrCol := not((Pos('R', AddressTarget) > 0) and (Pos('C', AddressTarget) > 0));
  if IsSelRowOrCol then
  begin
    ErrorText := '��������� �������� �������� ������������ ������.';
    exit;
  end;

  if TVarData(Target.MergeCells).VBoolean then
  begin
    ErrorText := '��������� �������� �������� ������������ ������.';
    exit;
  end;

  if (Target.Columns.Count > 1) or (Target.Rows.Count > 1) then
  begin
    ErrorText := '��������� �������� �������� ��������� �����.';
    exit;
  end;

  if TableSelected(ExcelSheet, Target) then
  begin
    if IsRangeBelongName(ExcelSheet, Target, BuildExcelName(sntTotals)) then
      result := true
    else
      ErrorText := '��������� �������� ���������� �� �������� �����������.';
  end
  else
    ErrorText := '��������� �������� ���������� �� �������� �����������.';
end;

{�������� ��������� ������� ��� �����}
function GetRowsLeafColumn(ExcelSheet: ExcelWorksheet): integer;
var
  RowsRange: ExcelRange;
begin
  result := 0;
  RowsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
  if not Assigned(RowsRange) then
    exit;
  result := RowsRange.Column + RowsRange.Columns.Count - 1;
end;

{�������� ��������� ������ ��� ��������}
function GetColumnsLeafRow(ExcelSheet: ExcelWorksheet): integer;
var
  ColumnsRange: ExcelRange;
begin
  result := 0;
  ColumnsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntColumns));
  if not Assigned(ColumnsRange) then
    exit;
  result := ColumnsRange.Row + ColumnsRange.Rows.Count - 1;
end;

{���������� �� ��� �����, � ����� ��� �������� ��������}
function GetRowsLeafMemberName(ExcelSheet: ExcelWorksheet; Row: integer): string;
var
  RowsLeafColumn: integer;
  CellRange: ExcelRange;
begin
  RowsLeafColumn := GetRowsLeafColumn(ExcelSheet);
  CellRange := GetRange(ExcelSheet, Row, RowsLeafColumn, Row, RowsLeafColumn);
  result := GetNameByRange(ExcelSheet, CellRange);
end;

{���������� �� ��� ��������, � ����� ��� �������� ��������}
function GetColumnsLeafMemberName(ExcelSheet: ExcelWorksheet;
  Column: integer): string;
var
  ColumnsLeafRow: integer;
  CellRange: ExcelRange;
begin
  ColumnsLeafRow := GetColumnsLeafRow(ExcelSheet);
  CellRange := GetRange(ExcelSheet, ColumnsLeafRow, Column, ColumnsLeafRow, Column);
  result := GetNameByRange(ExcelSheet, CellRange);
end;

{�� ����������� ���������� ���������� �������� �� ������ ������}
function IsSummaryCell(ExcelSheet: ExcelWorksheet; Row, Column: integer): boolean;
begin
  result := IsSummaryRow(ExcelSheet, Row) or
    IsSummaryColumn(ExcelSheet, Column)
end;

function IsSummaryColumn(ExcelSheet: ExcelWorkSheet; Column: integer): boolean;
var
  LeafMemberColumns: string;
begin
  LeafMemberColumns := GetColumnsLeafMemberName(ExcelSheet, Column);
  if LeafMemberColumns <> '' then
    result := IsWritebackSenseless(LeafMemberColumns)
  else
    result := false;
end;

function IsSummaryRow(ExcelSheet: ExcelWorkSheet; Row: integer): boolean;
var
  LeafMemberRows: string;
begin
  LeafMemberRows := GetRowsLeafMemberName(ExcelSheet, Row);
  if LeafMemberRows <> '' then
    result := IsWritebackSenseless(LeafMemberRows)
  else
    result := false;
end;

function IsSequelSheet(ExcelSheet: ExcelWorkSheet): boolean;
begin
  result := Assigned(GetCPByName(ExcelSheet, cpSequelName, false));
end;

function ReplaceCellRef(Template, WhatPattern, WithPattern: string): string;
var
  Index, Len: integer;
begin
  result := '';
  Len := Length(WhatPattern);
  if Template[1] = '=' then
  begin
    result := '=';
    Delete(Template, 1, 1);
  end;
  Index := Pos(WhatPattern, Template);
  while Index > 0 do
  begin
    if Template[Index - 1] in ['A'..'Z'] then
      result := result + Copy(Template, 1, Index + Len - 1)
    else
      result := result + Copy(Template, 1, Index - 1) + WithPattern;
    Delete(Template, 1, Index + Len - 1);
    Index := Pos(WhatPattern, Template);
  end;
  result := result + Template;
end;

procedure AutoFitMerged(MergedRange: ExcelRange);
var
  StartCell, tmpCell: ExcelRange;
  OldStartCellHeight, StartCellWidth, MergedWidth,
  OldMergedHeight, NewMergedHeight: double;
  MultipleRows: boolean;
  i: integer;
begin
  if not Assigned(MergedRange) or not boolean(MergedRange.MergeCells) then
    exit;
  StartCell := ExcelRange(TVarData(MergedRange.Item[1, 1]).VDispatch);
  MergedRange := StartCell.MergeArea;
  if MergedRange.Columns.Count < 2 then
    exit;

  OldStartCellHeight := StartCell.RowHeight;
  StartCellWidth := StartCell.ColumnWidth;

  {������ ������ ������������ �������}
  MergedWidth := 0;
  for i := 1 to MergedRange.Columns.Count do
  begin
    tmpCell := ExcelRange(TVarData(MergedRange.Item[1, i]).VDispatch);
    MergedWidth := MergedWidth + tmpCell.ColumnWidth;
  end;
  if MergedWidth > 255 then
    MergedWidth := 255;

  OldMergedHeight := MergedRange.Height;

  MergedRange.MergeCells := false;
  StartCell.ColumnWidth := MergedWidth;
  if not StartCell.WrapText then
    StartCell.WrapText := true;
  StartCell.EntireRow.AutoFit;
  NewMergedHeight := StartCell.RowHeight;
  StartCell.ColumnWidth := StartCellWidth;
  MergedRange.MergeCells := true;

  MultipleRows := MergedRange.Rows.Count > 1;
  if MultipleRows then
    StartCell.RowHeight := OldStartCellHeight
  else
    StartCell.RowHeight := NewMergedHeight;

  if MultipleRows and (NewMergedHeight > OldMergedHeight) then
  begin
    tmpCell := ExcelRange(TVarData(MergedRange.Item[MergedRange.Rows.Count, 1]).VDispatch);
    tmpCell.RowHeight := tmpCell.RowHeight + NewMergedHeight - OldMergedHeight;
  end;
end;

end.


