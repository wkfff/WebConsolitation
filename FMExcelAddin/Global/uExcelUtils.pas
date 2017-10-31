{Работа с Excel}
unit uExcelUtils;

interface
uses
  ExcelXP, OfficeXP, Classes, SysUtils, Windows, dialogs,
  comObj;

  {Создает невидимый эксель}
  function GetExcel(out Excel : ExcelApplication) : boolean;
  {по указному пути получить книгу}
  function GetExcelBook(ExcelAppl: ExcelApplication; Path: string): ExcelWorkbook;
  {получить адрес диапазона}
  function GetAddressLocal(Range: ExcelRange): string;
  {возвращает объект ExcelXP.Name по его имени}
  function GetNameObject(ExcelSheet: ExcelWorksheet; AName: string): ExcelXP.Name;
  {Вернет диапазон по имени}
  function GetRangeByName(ExcelSheet: ExcelWorksheet; AName: string): ExcelRange;
  {получить имя по диапазону}
  function GetNameByRange(ExcelSheet: ExcelWorksheet; ElementRange: ExcelRange): string;
  {получить имя, соответствующее ячейке}
  function GetNameByRC(ExcelSheet: ExcelWorksheet; Row, Column: integer): string;
  {по координатам диапазонов определяем входит ли Range1 в Range2}
  function IsNestedRanges(Range1, Range2: ExcelRange): boolean;
  function IsNestedRanges_(Range1, Range2: ExcelRange): boolean;
  {если диапазоны пересекаются возвращает true}
  function IsRangesIntersect(ExcelSheet: ExcelWorksheet; Range1, Range2: ExcelRange): boolean;
  { Вернет результат пересечения диапазонов}
  function GetIntersection(ExcelSheet: ExcelWorksheet; Range1, Range2: ExcelRange): ExcelRange;
  {вернет объединеный диапазон}
  function GetUnionRange(const Range1, Range2: ExcelRange): ExcelRange;
  {если имя не имеет диапазона возвращает true}
  function NameWithoutRange(Name: ExcelXP.Name): boolean;
  {возвращает диапазон по координатам}
  function GetRange(ExcelSheet: ExcelWorksheet;
    x1, y1, x2, y2: integer): ExcelRange; overload;
  {Возвращает диапазон по двум угловым точкам}
  function GetRange(ExcelSheet: ExcelWorksheet; P1, P2: TPoint): ExcelRange; overload;
  {Как долго нехватало этого варианта получения диапазона}
  function GetRange(ExcelSheet: ExcelWorksheet; Row, Column: integer): ExcelRange; overload;
  {Возвращает диапазон по адресу(xlR1C1)}
  function GetRange(ExcelSheet: ExcelWorksheet; AddressLocal: string): ExcelRange; overload;
  {объеденият диапазон WorkSheet в одну ячейку}
  function MergeCells(ExcelSheet: ExcelWorksheet; x1, y1, x2, y2: integer): ExcelRange;
  {получить высоту диапазона}
  function GetRangeHeight(Range: ExcelRange): integer;
  {возвращает СР по имени; если не находит, то может создать.}
  function GetCPByName(ExcelSheet: ExcelWorksheet;
    CPName: string; ForceCreate: boolean): CustomProperty;
  { получить индекс колонки - то есть A - 1,  B - 2  и. т. д.}
  function GetColumnIndex(Column: string): integer;
  {получить литерное имя колонки - 1 - A, 2 - B}
  function GetColumnName(Column: integer): string;
  {Достает книгу из диспача}
  function GetWorkSheet(Disp: IDispatch): ExcelWorkSheet;
  {Безопасное сохранение книги по указанному адресу}
  function SaveBook(ExcelBook: ExcelWorkbook; Path: string): boolean;
  // получить Formula ячейки
  function GetCellFormula(ExcelSheet: ExcelWorkSheet; Row, Column: integer; var Formula: string): boolean; overload;
  function GetCellFormula(CellRange: ExcelRange; var Formula: string): boolean; overload;
  // установить Formula ячейки
  procedure SetCellFormula(ExcelSheet: ExcelWorkSheet; Row, Column: integer; Formula: string); overload;
  procedure SetCellFormula(ExcelSheet: ExcelWorkSheet; CellRange: ExcelRange; Formula: string); overload;
  {добавляет комментарий к ячейке
  text может содержать символы #10 для разбиения на строки}
  procedure CommentCell(ExcelSheet: ExcelWorksheet;
    Row, Column: integer; Text: string);
  {Добовляет комментарий к уже сужествующему комментарию в ячейке}
  procedure AddCellComment(ExcelSheet: ExcelWorksheet; Row, Column: integer;
    Text: string); overload;
  procedure AddCellComment(Cell: OleVariant{ExcelRange}; Text: string); overload;
  {возвращает имя без ссылки на лист}
  function GetShortSheetName(AName: string): string;
  function IsExcelVisible(Excel: ExcelApplication): boolean;
  function RenameCP(ExcelSheet: ExcelWorkSheet; OldName, NewName: string): boolean;
  procedure SetCalculation(ExcelAppl: ExcelApplication; Calculation: XlCalculation);
  {безопасная установка стиля, диапазону}
  function SetRangeStyle(Range: ExcelRange; StyleName: string): boolean;
  function IsPointInRange(Range: ExcelRange; Row, Column: integer): boolean;
implementation

function GetExcel(out Excel : ExcelApplication) : boolean;
var LCID: integer;
begin
  result := true;
  try
    Excel := CreateComObject(CLASS_ExcelApplication) as ExcelApplication;
    LCID := GetUserDefaultLCID;
    Excel.Visible[LCID] := false;
    Excel.DisplayAlerts[LCID] := false;
  except
    result := false;
  end;
end;

{По указному пути получить книгу}
function GetExcelBook(ExcelAppl: ExcelApplication;
  Path: string): ExcelWorkbook;
begin
  result := nil;
  if ((not Assigned(ExcelAppl)) or (Path = '')) then
    exit;
  try
    result := ExcelAppl.Workbooks.Add(Path, GetUserDefaultLCID);
  except
    result := nil;
  end;
end;

function GetAddressLocal(Range: ExcelRange): string;
begin
  result := '';
  if not Assigned(Range) then
    exit;
  try
    {$WARNINGS OFF}
    result := Range.AddressLocal[true, true, xlR1C1, false, varNull];
    {$WARNINGS ON}
  except
    result := '';
  end;
end;

function IsExcelVisible(Excel: ExcelApplication): boolean;
begin
  result := (Assigned(Excel) and(Excel.Visible[GetUserDefaultLCID]));
end;

function GetNameObject(ExcelSheet: ExcelWorksheet; AName: string): ExcelXP.Name;
var
  SheetName: string;
begin
  result := nil;
  if not Assigned(ExcelSheet) or (AName = '') then
    exit;
  // получаем полное имя имени (формат: Листа!Имя)
  SheetName := ExcelSheet.Name;
  if Pos('!', AName) < 1 then
  begin
    while SheetName[Length(SheetName)] = '!' do
      delete(SheetName, Length(SheetName), 1);
    AName := SheetName + '!' + AName;
  end;
  // получаем объект "имя"
  try
    result := ExcelSheet.Names.Item(0, AName, 0);
  except
  end;
end;

function GetRangeByName(ExcelSheet: ExcelWorksheet; AName: string): ExcelRange;
var
  ExcelName: ExcelXP.Name;
begin
  try
    // получаем объект "имя"
    ExcelName := GetNameObject(ExcelSheet, AName);
    // возвращаем соответствующий имени диапазон
    result := ExcelName.RefersToRange;
  except
    // имени может не быть
    result := nil;
  end;
end;

function GetNameByRange(ExcelSheet: ExcelWorksheet; ElementRange: ExcelRange): string;
var
  tmpIndex: integer;
  Name_: ExcelXP.Name;
  RangeName: string;
begin
  try
    // ищем имя, соответствующее элементу
    try
      RangeName := ElementRange.MergeArea.Name;
      Name_ := ExcelSheet.Names.Item(EmptyParam, EmptyParam, RangeName);
      result := Name_.Name_;
    except
      result := ElementRange.Name.Name;
    end;

    tmpIndex := Pos('!', result);
    result := Copy(result, tmpIndex + 1, Length(result) - tmpIndex);
  except
    result := '';
  end;
end;

function GetNameByRC(ExcelSheet: ExcelWorksheet; Row, Column: integer): string;
var
  CellRange: ExcelRange;
begin
  result := '';
  if (Row < 1) or (Row > ExcelSheet.Rows.Count) or
    (Column < 1) or (Column > ExcelSheet.Columns.Count) then
    exit;
  with ExcelSheet do
    CellRange := Range[Cells.Item[Row, Column],
                       Cells.Item[Row, Column]];
  result := GetNameByRange(ExcelSheet, CellRange);
end;

function IsNestedRanges(Range1, Range2: ExcelRange): boolean;
var
  Application: ExcelApplication;
  LCID: integer;
  ProductSpace: ExcelRange; //область пересечения
begin
  result := false;
  if not(Assigned(Range1) and Assigned(Range2)) then
    exit;
  Application := Range1.Application;
  LCID := GetUserDefaultLCID;
  try
    ProductSpace := Application.Intersect(Range1 ,Range2 ,EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, LCID);
    {$WARNINGS OFF}
    if Assigned(ProductSpace) then
      result := (ProductSpace.AddressLocal[true, true, xlR1C1, false, varNull] =
        Range1.AddressLocal[true, true, xlR1C1, false, varNull]);
    {$WARNINGS ON}
  except
    result := false;
  end;
end;

function IsNestedRanges_(Range1, Range2: ExcelRange): boolean;
var
  Range1Start, Range2Start, Range1End, Range2End: TPoint;
begin
  result := false;
  if not(Assigned(Range1) and Assigned(Range2)) then
    exit;
  Range1Start := Point(Range1.Row, Range1.Column);
  Range2Start := Point(Range2.Row, Range2.Column);
  Range1End := Point(Range1.Row + Range1.Rows.Count - 1,
    Range1.Column + Range1.Columns.Count - 1);
  Range2End := Point(Range2.Row + Range2.Rows.Count - 1,
    Range2.Column + Range2.Columns.Count - 1);
  result := ((Range1Start.x >= Range2Start.x) and (Range1Start.x <= Range2End.x)
    and (Range1Start.y >= Range2Start.y) and (Range1Start.y <= Range2End.y)
    and (Range1End.x >= Range2Start.x) and (Range1End.x <= Range2End.x)
    and (Range1End.y >= Range2Start.y) and (Range1End.y <= Range2End.y));
end;

function NameWithoutRange(Name: ExcelXP.Name): boolean;
begin
  try
    result := not Assigned(Name.RefersToRange);
  except
    result := true;
  end;
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

function GetRange(ExcelSheet: ExcelWorksheet; x1, y1, x2, y2: integer): ExcelRange;
begin
  try
    with ExcelSheet do
      result := Range[Cells.Item[x1, y1], Cells.Item[x2, y2]];
  except
    result := nil;
  end;
end;

function GetRange(ExcelSheet: ExcelWorksheet; P1, P2: TPoint): ExcelRange;
begin
  result := nil;
  if (P1.x > 0) and (P1.y > 0) and (P2.x > 0) and (P2.y > 0) then
    result := GetRange(ExcelSheet, P1.x, P1.y, P2.x, P2.y);
end;

function GetRange(ExcelSheet: ExcelWorksheet; Row, Column: integer): ExcelRange;
begin
  result := GetRange(ExcelSheet, Row, Column, Row, Column);
end;

function GetRange(ExcelSheet: ExcelWorksheet; AddressLocal: string): ExcelRange; overload;
var
  R1, C1, R2, C2: integer;
begin
  result := nil;
  if not Assigned(ExcelSheet) then
    exit;
  try
    CutPart(AddressLocal, 'R');
    R1 := StrToInt(CutPart(AddressLocal, 'C'));
    C1 := StrToInt(CutPart(AddressLocal, ':'));
    CutPart(AddressLocal, 'R');
    R2 := StrToInt(CutPart(AddressLocal, 'C'));
    C2 := StrToInt(AddressLocal);
    result := GetRange(ExcelSheet, R1, C1, R2, C2);
  except
    result := nil;
  end;
end;

function MergeCells(ExcelSheet: ExcelWorksheet;
  x1, y1, x2, y2: integer): ExcelRange;
begin
  result := GetRange(ExcelSheet, x1, y1, x2, y2);
  result.Merge(false);
end;

function GetRangeHeight(Range: ExcelRange): integer;
begin
  result := 0;
  if not Assigned(Range) then
    exit;
  result := Range.Rows.Count;
end;

function GetCPByName(ExcelSheet: ExcelWorksheet;
  CPName: string; ForceCreate: boolean): CustomProperty;
var
  i: integer;
  sName: string;
  CPCount: integer;
begin
  result := nil;
  if not Assigned(ExcelSheet) then
    exit;
  CPCount := ExcelSheet.CustomProperties.Count;
  for i := 1 to  CPCount do
  begin
    sName := ExcelSheet.CustomProperties.Item[i].Name;
    if sName = CPName then
    begin
      result := ExcelSheet.CustomProperties.Item[i];
      exit;
    end;
  end;
  if not Assigned(result) and ForceCreate then
    result := ExcelSheet.CustomProperties.Add(CPName, 'eprst');
end;

function GetIntersection(ExcelSheet: ExcelWorksheet; Range1, Range2: ExcelRange): ExcelRange;
var
  FLCID: integer;
begin
  result := nil;
  if not Assigned(ExcelSheet) then
    exit;
  FLCID := GetUserDefaultLCID;
  try
    result := ExcelSheet.Application.Intersect(Range1 ,Range2 ,EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, EmptyParam, FLCID);
  except
    result := nil;
  end;
end;

function IsRangesIntersect(ExcelSheet: ExcelWorksheet; Range1, Range2: ExcelRange): boolean;
var
  Range: ExcelRange;
begin
  result := false;
  if not Assigned(ExcelSheet) then
    exit;
  Range := GetIntersection(ExcelSheet, Range1, Range2);
  result := Assigned(Range);
end;

function GetUnionRange(const Range1, Range2: ExcelRange): ExcelRange;
var
  Application: ExcelApplication;
  LCID: integer;
begin
  result := nil;
  if not(Assigned(Range1) or Assigned(Range2)) then
    exit;

  try
    LCID := GetUserDefaultLCID;
    if Assigned(Range1) then
      Application := Range1.Application
    else
      Application := Range2.Application;

    if (Assigned(Range1) and Assigned(Range2)) then
      result := Application.Union(Range1, Range2, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, LCID)
    else
      if Assigned(Range1) then
        result := Range1
      else
        result := Range2;
  except
    result := nil;
  end;
end;

function GetColumnIndex(Column: string): integer;
var
  i: integer;
  Digit: integer;
begin
  result := 0;
  Digit := -1;
  for i := Length(Column) downto 1 do
  begin
    Inc(Digit);
    // букв 26...
    if (Digit = 0) then
      result := (ord(Column[i]) - ord('A') + 1);
    result := (Digit * 26) * (ord(Column[i]) - ord('A') + 1) + result;
  end;
end;

function GetColumnName(Column: integer): string;
var
  Rest: integer;
begin
  result := '';
  Rest := (Column div 26);
  if (Column mod 26 = 0) then
    Dec(Rest);
  if (Rest > 0) then
    result := result + Chr(ord('A') - 1 + Rest);
  if (Column mod 26 = 0) then
    Rest := 26
  else Rest := Column mod 26;
  if (Rest > 0) then
    result := result + Chr(ord('A') - 1 + Rest);
end;


function GetWorkSheet(Disp: IDispatch): ExcelWorkSheet;
begin
  try
    result := Disp as ExcelWorkSheet;
    if Cardinal(result.type_[0]) <> Cardinal(xlWorkSheet) then
      result := nil;
  except
    result := nil;
  end;
end;

function SaveBook(ExcelBook: ExcelWorkbook; Path: string): boolean;
var
  LCID: integer;
begin
  result := false;
  if not (Assigned(ExcelBook) and (Path <> '')) then
    exit;
  LCID := GetUserDefaultLCID;
  try
    ExcelBook.SaveCopyAs(Path, LCID);
    result := true;
  except
    result := false;
  end;
end;

function GetCellFormula(ExcelSheet: ExcelWorkSheet; Row, Column: integer; var Formula: string): boolean;
var
  CellRange: ExcelRange;
begin
  with ExcelSheet do
    CellRange := Range[Cells.Item[Row, Column], Cells.Item[Row, Column]];
  result := GetCellFormula(CellRange, Formula);
end;

function GetCellFormula(CellRange: ExcelRange; var Formula: string): boolean;
begin
  Formula := CellRange.Formula;
  result := (Formula <> '') and (copy(Formula, 1, 1) = '=');
end;

procedure SetCellFormula(ExcelSheet: ExcelWorkSheet; CellRange: ExcelRange; Formula: string);
var
  NumberFormat: OleVariant;
begin
  NumberFormat := CellRange.NumberFormat;
  if NumberFormat = '@' then
    NumberFormat := '';
  try
    CellRange.Formula := Formula;
    CellRange.NumberFormat := NumberFormat;
  except
    //on e: exception do showmessage(e.message);
  end;
end;

procedure SetCellFormula(ExcelSheet: ExcelWorkSheet; Row, Column: integer; Formula: string);
var
  CellRange: ExcelRange;
begin
  CellRange := GetRange(ExcelSheet, Row, Column);
  SetCellFormula(ExcelSheet, CellRange, Formula);
end;

procedure CommentCell(ExcelSheet: ExcelWorksheet; Row, Column: integer; Text: string);
var
  Comm: variant;
begin
  if not Assigned(ExcelSheet) or (Text = '')then
    exit;
  try
    ExcelSheet.Cells.Item[Row, Column].ClearComments;
    ExcelSheet.Cells.Item[Row, Column].AddComment(Text);
    Comm := ExcelSheet.Cells.Item[Row, Column].Comment;
    Comm.Shape.TextFrame.Autosize := true;
    Comm.Shape.Placement := xlMove;
  except
  end;
end;

procedure AddCellComment(ExcelSheet: ExcelWorksheet; Row, Column: integer;
  Text: string);
var
  OldComment: string;
  Cell: OleVariant;
begin
  if not Assigned(ExcelSheet) or (Text = '') then
    exit;
  try
    Cell := ExcelSheet.Cells.Item[Row, Column];
    try
      OldComment := Cell.Comment.Text;
    except
      OldComment := '';
    end;
    Cell.ClearComments;
    if OldComment <> '' then
      Text := OldComment + #10#10 + Text;
    Cell.AddComment(Text);
    Cell.Comment.Shape.TextFrame.Autosize := true;
    Cell.Comment.Shape.Placement := xlMove;
  except
  end;
end;

procedure AddCellComment(Cell: OleVariant{ExcelRange}; Text: string); overload;
var
  OldComment: string;
begin
  try
    try
      OldComment := Cell.Comment.Text;
    except
      OldComment := '';
    end;
    Cell.ClearComments;
    if OldComment <> '' then
      Text := OldComment + #10#10 + Text;
    Cell.AddComment(Text);
    Cell.Comment.Shape.TextFrame.Autosize := true;
    Cell.Comment.Shape.Placement := xlMove;
  except
  end;
end;

function GetShortSheetName(AName: string): string;
begin
  result := Copy(AName, Pos('!', AName) + 1, Length(AName) - Pos('!', AName));
end;

function RenameCP(ExcelSheet: ExcelWorkSheet; OldName, NewName: string): boolean;
var
  CP: CustomProperty;
begin
  result := false;
  CP := GetCPByName(ExcelSheet, OldName, false);
  try
    CP.Name := NewName;
  except
    result := false;
  end;
end;

procedure SetCalculation(ExcelAppl: ExcelApplication; Calculation: XlCalculation);
begin
  if not Assigned(ExcelAppl) then
    exit;
  try
    ExcelAppl.Set_Calculation(GetUserDefaultLCID, Calculation);
  except
  end;
end;

{безопасная установка стиля, диапазону}
function SetRangeStyle(Range: ExcelRange; StyleName: string): boolean;
begin
  result := true;
  try
    Range.Style := StyleName;
  except
    result := false;
  end;
end;

function IsPointInRange(Range: ExcelRange; Row, Column: integer): boolean;
begin
  result := ((Row >= Range.Row) and (Row <= Range.Row + Range.Rows.Count - 1) and
          (Column >= Range.Column) and (Column <= Range.Column + Range.Columns.Count - 1))
end;


end.
