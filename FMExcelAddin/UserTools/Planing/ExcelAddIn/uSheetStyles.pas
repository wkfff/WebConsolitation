{
  Описание стилей (MSExcel Workbook Styles),
  которые используются для "рисования" таблицы.
}

unit uSheetStyles;

interface

uses
  ComObj, Dialogs, ExcelXP, uFMExcelAddInConst, uFMAddinExcelUtils, Windows,
  uGlobalPlaningConst;

{создание стилей, если уже есть и выставлен флаг IsReset, вернет стилям вид
по умолчанию}
function InitWorkbookStyles(Workbook:_Workbook; IsReset: boolean): boolean;
{является ли стиль одним из наших "дефолтных"}
function IsOurStyle(StyleName: string): boolean;
procedure LockStyle(AStyle: OleVariant); overload;
procedure LockStyle(Book: ExcelWorkBook; StyleName: string); overload;
procedure LockStyles(Wb: ExcelWorkbook);
{получаем стиль по имени}
function GetStyleByName(ExcelBook: ExcelWorkbook; StyleName: string): Style;

implementation

const
  AllOurStyles: array [1..23] of string = (snFieldTitle, snFieldTitlePrint,
    snFieldPosition, snFieldPositionPrint, snTotalMeasureTitle, snTotalConstTitle,
    snTotalFreeTitle, snTotalResultTitle, snTotalTitlePrint, snData, snDataFree,
    snDataFreeErased, snFilterValue, snFilterValuePrint, snSheetId,
    snMemberProperties, snMemberPropertiesPrint, snSingleCells, snSingleCellsPrint,
    snResultSingleCells, snResultSingleCellsPrint, snSingleCellConst,
    snSingleCellConstPrint);

procedure LockStyle(AStyle: OleVariant);
var
  xpStyle: ExcelXP.Style;
begin
  try
    xpStyle := ExcelXP.Style(TVarData(AStyle).VDispatch);
    if not xpStyle.Locked then
      xpStyle.Locked := true;
  except
  end;
end;

procedure LockStyle(Book: ExcelWorkBook; StyleName: string); overload;
begin
  try
    LockStyle(Book.Styles.Item[StyleName]);
  except
  end;
end;

procedure LockStyles(Wb: ExcelWorkbook);
var
  i: integer;
begin
  for i := 1 to Wb.Styles.Count do
    if IsOurStyle(Wb.Styles[i].Name) then
      LockStyle(Wb.Styles[i]);
end;

{получить стиль по имени}
function GetStyleByName(ExcelBook: ExcelWorkbook; StyleName: string): Style;
var
  i: integer;
begin
  result := nil;
  for i := 1 to ExcelBook.Styles.Count do
    if (ExcelBook.Styles.Item[i].Name = StyleName) then
    begin
      result := ExcelBook.Styles.Item[i];
      exit;
    end;
end;

function IsExistAllStyles(ExcelBook: ExcelWorkBook): boolean;
var
  i: integer;
begin
  result := false;
  for i := Low(AllOurStyles) to High(AllOurStyles) do
    if not Assigned(GetStyleByName(ExcelBook, AllOurStyles[i])) then
      exit;
  result := true;
end;
     
// проверка на существование стиля
function ExistStyle(ExcelBook: ExcelWorkbook; StyleName: string): boolean;
begin
  result := Assigned(GetStyleByName(ExcelBook, StyleName));
end;

{создание стилей, если уже есть и выставлен флаг IsReset, вернет стилям вид
по умолчанию}
function InitWorkbookStyles(Workbook: ExcelWorkbook; IsReset: boolean): boolean;

  procedure RecreateStyle(var CurStyle: ExcelXP.Style; StyleName: string);
  begin
    if Assigned(CurStyle) then
      try
        CurStyle.Delete;
        CurStyle := nil;
      except
      end;
    if not Assigned(CurStyle) then
      CurStyle := Workbook.Styles.Add(StyleName, null);
  end;

var
  CurStyle: Style;
begin
  {$WARNINGS OFF}
  result := false;
  try
    {Если все требуемые стили уже есть, и не требуется их перегружать тогда выходим}
    if IsExistAllStyles(WorkBook) and (not IsReset) then
    begin
      result := true;
      exit;
    end;
    if not SetBookProtection(Workbook, false) then
      exit;

    CurStyle := GetStyleByName(Workbook, snFieldTitle);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFieldTitle);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFieldTitle, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 15;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        NumberFormat := '@';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    CurStyle := GetStyleByName(Workbook, snFieldTitlePrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFieldTitlePrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFieldTitlePrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        Font.Bold := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        NumberFormat := '@';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    CurStyle := GetStyleByName(Workbook, snFieldPosition);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFieldPosition);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFieldPosition, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 34;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex :=xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
       end;
    end;

    CurStyle := GetStyleByName(Workbook, snFieldPositionPrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFieldPositionPrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFieldPositionPrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex :=xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
       end;
    end;

    // описание стиля TotalMeasureTitle
    CurStyle := GetStyleByName(Workbook, snTotalMeasureTitle);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snTotalMeasureTitle);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snTotalMeasureTitle, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 44;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle:= xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля TotalConstTitle
    CurStyle := GetStyleByName(Workbook, snTotalConstTitle);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snTotalConstTitle);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snTotalConstTitle, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 24;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle:= xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля TotalFreeTitle
    CurStyle := GetStyleByName(Workbook, snTotalFreeTitle);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snTotalFreeTitle);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snTotalFreeTitle, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 6;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля TotalResultTitle
    CurStyle := GetStyleByName(Workbook, snTotalResultTitle);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snTotalResultTitle);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snTotalResultTitle, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 8;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля snTotalTitlePrint
    CurStyle := GetStyleByName(Workbook, snTotalTitlePrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snTotalTitlePrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snTotalTitlePrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        Font.Bold := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := True;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle:= xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля Data
    CurStyle := GetStyleByName(Workbook, snData);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snData);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snData, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        NumberFormat := '';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;
  
    // описание стиля DataFree
    CurStyle := GetStyleByName(Workbook, snDataFree);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snDataFree);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snDataFree, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex:= xlNone;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        NumberFormat := '';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля DataFreeErased
    CurStyle := GetStyleByName(Workbook, snDataFreeErased);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snDataFreeErased);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snDataFreeErased, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex:= xlNone;
        Interior.Pattern := xlDown;
        Interior.PatternColorIndex := 3;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        NumberFormat := '';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля FilterValue
    CurStyle := GetStyleByName(Workbook, snFilterValue);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFilterValue);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFilterValue, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 34;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := true;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex :=xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
       end;
    end;

    CurStyle := GetStyleByName(Workbook, snFilterValuePrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snFilterValuePrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snFilterValuePrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := true;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex :=xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
       end;
    end;

    CurStyle := GetStyleByName(Workbook, snSheetId);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snSheetId);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snSheetId, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := false;
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex :=xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlNone;
        Borders[xlLeft].LineStyle := xlNone;
        Borders[xlRight].LineStyle := xlNone;
        Borders[xlTop].LineStyle := xlNone;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
        Font.ColorIndex := 16;
        Font.Italic := true;
        Font.Size := 8;
       end;
    end;

    // описание стиля MemberProperties
    CurStyle := GetStyleByName(Workbook, snMemberProperties);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snMemberProperties);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snMemberProperties, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := 36;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := true;
        Font.ColorIndex := 55;
        NumberFormat := '@';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля MemberPropertiesPrint
    CurStyle := GetStyleByName(Workbook, snMemberPropertiesPrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snMemberPropertiesPrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snMemberPropertiesPrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlLeft;
        VerticalAlignment := xlTop;
        WrapText := true;
        Font.ColorIndex := xlAutomatic;
        NumberFormat := '@';
        // бордюры
        Borders.LineStyle := xlContinuous;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle:= xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля SingleCells
    CurStyle := GetStyleByName(Workbook, snSingleCells);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snSingleCells);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snSingleCells, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := 44;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders[xlBottom].LineStyle := xlDash;
        Borders[xlLeft].LineStyle := xlDash;
        Borders[xlRight].LineStyle := xlDash;
        Borders[xlTop].LineStyle := xlDash;
        Borders.ColorIndex := 5;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля SingleCellsPrint
    CurStyle := GetStyleByName(Workbook, snSingleCellsPrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snSingleCellsPrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snSingleCellsPrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля ResultSingleCells
    CurStyle := GetStyleByName(Workbook, snResultSingleCells);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snResultSingleCells);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snResultSingleCells, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := 8;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders[xlBottom].LineStyle := xlDash;
        Borders[xlLeft].LineStyle := xlDash;
        Borders[xlRight].LineStyle := xlDash;
        Borders[xlTop].LineStyle := xlDash;
        Borders.ColorIndex := 5;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля ResultSingleCellsPrint
    CurStyle := GetStyleByName(Workbook, snResultSingleCellsPrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snResultSingleCellsPrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snResultSingleCellsPrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля SingleCellConst
    CurStyle := GetStyleByName(Workbook, snSingleCellConst);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snSingleCellConst);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snSingleCellConst, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := 24;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders[xlBottom].LineStyle := xlDash;
        Borders[xlLeft].LineStyle := xlDash;
        Borders[xlRight].LineStyle := xlDash;
        Borders[xlTop].LineStyle := xlDash;
        Borders.ColorIndex := 5;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;

    // описание стиля snSingleCellConstPrint
    CurStyle := GetStyleByName(Workbook, snSingleCellConstPrint);
    if (not Assigned(CurStyle) or IsReset) then
    begin
      RecreateStyle(CurStyle, snSingleCellConstPrint);
      (*if not Assigned(CurStyle) then
        CurStyle := Workbook.Styles.Add(snSingleCellConstPrint, null);*)
      with CurStyle do
      begin
        Locked := true;
        IncludeNumber := false;
        Interior.ColorIndex := xlNone;
        HorizontalAlignment := xlRight;
        VerticalAlignment := xlTop;
        // бордюры
        Borders.Weight := xlThin;
        Borders.ColorIndex := xlAutomatic;
        Borders[xlInsideHorizontal].LineStyle := xlNone;
        Borders[xlInsideVertical].LineStyle := xlNone;
        Borders[xlBottom].LineStyle := xlOn;
        Borders[xlLeft].LineStyle := xlOn;
        Borders[xlRight].LineStyle := xlOn;
        Borders[xlTop].LineStyle := xlOn;
        Borders[xlDiagonalDown].LineStyle := xlNone;
        Borders[xlDiagonalUp].LineStyle := xlNone;
      end;
    end;
    result := true;
  except
    result := false;
  end;
  {$WARNINGS ON}
end;

function IsOurStyle(StyleName: string): boolean;
begin
  result := (StyleName = snFieldTitle) or (StyleName = snFieldTitlePrint) or
    (StyleName = snFieldPosition) or (StyleName = snFieldPositionPrint) or
    (StyleName = snTotalMeasureTitle) or (StyleName = snTotalConstTitle) or
    (StyleName = snTotalFreeTitle) or (StyleName = snTotalResultTitle) or
    (StyleName = snTotalTitlePrint) or (StyleName = snData) or
    (StyleName = snDataFree) or (StyleName = snDataFreeErased) or
    (StyleName = snFilterValue) or (StyleName = snFilterValuePrint) or
    (StyleName = snSheetId) or (StyleName = snMemberProperties) or
    (StyleName = snMemberPropertiesPrint) or (StyleName = snSingleCells) or
    (StyleName = snSingleCellsPrint) or (StyleName = snResultSingleCells) or
    (StyleName = snResultSingleCellsPrint) or (StyleName = snSingleCellConst) or
    (StyleName = snSingleCellConstPrint);
end;

end.
