{
  Класс TSheetSizer. Расчитывает размеры и координаты ключевых точек
  и областей таблицы на листе.
  Расчет производится по уже расчитанной объектной модели.
  Используется TSheetSizer как утилита в модели. 
}

unit uSheetSizer;

interface

uses
  SysUtils, Classes, ExcelXP, Windows, MSXML2_TLB,
  uFMExcelAddInConst, Math, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils,
  uSheetObjectModel, uSheetBreaks, uExcelUtils, uGlobalPlaningConst;


type

  {Класс, предназначенный для расчета размеров областей таблицы}
  TSheetSizer = class(TObject)
  private
    {Полями храниться те координаты, которые расчитываются один раз
    при инциализации с метаданных, остальное расчитывается иcходя из этого}
    FStartTable: TPoint;
    FFiltersHeight: integer;
    FFilterSplit: integer; //зазор между областью фильтров и основной таблицей
    FRowsHeight: integer;
    FRowsWidth: integer;
    FColumnsHeight: integer;
    FColumnsWidth: integer;
    FTotalsCount: integer;
    FIgnoredColumnsTotalsCount: integer;
    FCellSize: integer; //размер одной лиcтовой позиции столбца
    FNeedUnitMarker: boolean; //нужна ли надпись с единицей измерения перед таблицей
    FMarkerPosition: TMarkerPosition;
    FFilterCellsLength: integer; //количество объеденяемых ячеек для фильтра
    FIsMergeFilterCellsByTable: boolean; //объединять ячейки по текущей ширине таблицы
    FSheetIdSplit: integer; //зазор между основной таблицей и идентификаторами листа
    FSheetIdHeight: integer; //высота области идентификаторов листа
    FRowMPropsCount: integer; //кол-во mp в строках
    FColumnMPropsCount: integer; //кол-во mp в столбцах
    FRowPlaceMPBefore: boolean; //помещать область mp вперед строк
    FColumnPlaceMPBefore: boolean; //помещать область mp вперед столбцов

    FFiltersBreakHeight: integer; //Высота разрыва после фильтров
    FUnitMarkerBreakHeight: integer; //Высота разрыва после маркера(тыс. руб.)
    FColumnTitlesBreakHeight: integer; //Высота разрыва после заголовков столбцов
    FColumnsBreakHeight: integer; //Высота разрыва после столбцов
    FRowTitlesBreakHeight: integer; //Высота разрывов после заголовков строк/показателей
    FRowsBreakHeight: integer; //Высота разрыва после строк/показателей

    FIsDisplayFilters: boolean;
    FIsDisplayColumnsTitles: boolean;
    FIsDisplayRowsTitles: boolean;
    FIsDisplayTotalsTitles: boolean;
    FIsDisplayColumns: boolean;

    {Неопределенная точка}
    function UnknownPoint: TPoint;

    function FindTableStartPosition(ExcelSheet: ExcelWorkSheet;
      FLCID: integer): TPoint;

    procedure InitAxisSizeByAxisDOM(Axis: TAxisType; AxisDOM: IXMLDOMDocument2;
      SMD: TSheetInterface);
    procedure InitAxisSizeByCellsetDOM(Axis: TAxisType; CellsetDOM: IXMLDOMDocument2;
      SMD: TSheetInterface);


    {Вычисление базовых размеров таблицы.
     Здесь вычисляются параметры, которые зависят исключительно от SMD
     (не от данных). И только базовые, небазовые вычисляются функциями от них}
    procedure CalculateBaseMetrics(SMD: TSheetInterface);

  protected
    {Формулы "расчета не первичных" координат}
    function GetEndTable_(IsWithId: boolean): TPoint;
    function GetEndTable: TPoint;
    function GetEndTableWithoutId: TPoint;
    function GetStartRowsTitle: TPoint;
    function GetStartRows: TPoint;
    function GetEndRows: TPoint;
    function GetStartColumnsTitle: TPoint;
    function GetStartColumns: TPoint;
    function GetEndColumns: TPoint;
    function GetStartFilters: TPoint;
    function GetEndFilters: TPoint;
    function GetStartTotalsTitle: TPoint;
    function GetStartTotals: TPoint;
    function GetEndTotals: TPoint;
    function GetStartRowMPropsTitle: TPoint;
    function GetStartRowMProps: TPoint;
    function GetEndRowMProps: TPoint;
    function GetStartColumnMPropsTitle: TPoint;
    function GetStartColumnMProps: TPoint;
    function GetEndColumnMProps: TPoint;
    function GetStartSheetId: TPoint;
    function GetEndSheetId: TPoint;

    function GetColumnsAreaStartRow: integer;
    function GetColumnsAreaEndRow: integer;
    function GetRowsTotalsAreaStartRow: integer;
    function GetRowsTotalsAreaEndRow: integer;
    function GetRowsTotalsTitleHeight: integer;

    function GetStartColumnsAndMProps: TPoint;
    function GetEndColumnsAndMProps: TPoint;

    function GetFiltersSplitStartRow: integer;
    function GetUnitMarkerSplitStartRow: integer;
    function GetColumnTitlesSplitStartRow: integer;
    function GetColumnsSplitStartRow: integer;
    function GetRowTitlesSplitStartRow: integer;
    function GetRowsSplitStartRow: integer;
  public
    procedure TestClass;
    {Расчет для нормального режима}
    procedure Init(ExcelSheet: ExcelWorksheet; FLCID: integer; SMD: TSheetInterface;
      RowsDOM, ColumnsDOM: IXMLDOMDocument2); overload;
    {Расчет для "селсетного" режима - большие таблицы}
    procedure Init(ExcelSheet: ExcelWorksheet; FLCID: integer; SMD: TSheetInterface;
      CellSetData: IXMLDOMDocument2;
      out RowsProcessRoute, ColumnsProcessRoute: TIntegerArray); overload;


    {Таблица пуста? (т.е нет ни одного элемента листа)}
    function TableIsEmpty: boolean;

    {Размер листовой позиции столбца}
    function CellSize(Axis: TAxisType): integer;

    { Начало и конец всей таблицы}
    property StartTable: TPoint read FStartTable;
    property EndTable: TPoint read GetEndTable;
    property EndTableWithoutId: TPoint read GetEndTableWithoutId;

    { Начало и конец области фильтров}
    property StartFilters: TPoint read GetStartFilters;
    property EndFilters: TPoint read GetEndFilters;

    {Начало и конец оси строк }
    property StartRowsTitle: TPoint read GetStartRowsTitle;
    property StartRows: TPoint read GetStartRows;
    property EndRows: TPoint read GetEndRows;

    {Начало и конец оси колонок }
    property StartColumnsTitle: TPoint read GetStartColumnsTitle;
    property StartColumns: TPoint read GetStartColumns;
    property EndColumns: TPoint read GetEndColumns;

    {Начало и конец области итогов }
    property StartTotalsTitle: TPoint read GetStartTotalsTitle;
    property StartTotals: TPoint read GetStartTotals;
    property EndTotals: TPoint read GetEndTotals;
    function UnitMarker: TPoint;

    {Начало и конец мембер-пропертис строк }
    property StartRowMPropsTitle: TPoint read GetStartRowMPropsTitle;
    property StartRowMProps: TPoint read GetStartRowMProps;
    property EndRowMProps: TPoint read GetEndRowMProps;

    {Начало и конец мембер-пропертис столбцов }
    property StartColumnMPropsTitle: TPoint read GetStartColumnMPropsTitle;
    property StartColumnMProps: TPoint read GetStartColumnMProps;
    property EndColumnMProps: TPoint read GetEndColumnMProps;

    {Общие параметры таблицы}
    property RowsHeight: integer read FRowsHeight;
    property RowsWidth: integer read FRowsWidth;
    property ColumnsHeight: integer read FColumnsHeight;
    property ColumnsWidth: integer read FColumnsWidth;
    property RowMPropsCount: integer read FRowMPropsCount;
    property ColumnMPropsCount: integer read FColumnMPropsCount;
    property RowPlaceMPBefore: boolean read FRowPlaceMPBefore;
    property ColumnPlaceMPBefore: boolean read FColumnPlaceMPBefore;

    {высота области фильтров}
    property FiltersHeight: integer read FFiltersHeight default 0;
    property FilterSplit: integer read FFilterSplit;

    {начало и конец области иденификации листа}
    property StartSheetId: TPoint read GetStartSheetId;
    property EndSheetId: TPoint read GetEndSheetId;
    property SheetIdSplit: integer read FSheetIdSplit;

    property ColumnsAreaStartRow: integer read GetColumnsAreaStartRow;
    property ColumnsAreaEndRow: integer read GetColumnsAreaEndRow;
    property RowsTotalsAreaStartRow: integer read GetRowsTotalsAreaStartRow;
    property RowsTotalsAreaEndRow: integer read GetRowsTotalsAreaEndRow;
    property RowsTotalsTitleHeight: integer read GetRowsTotalsTitleHeight;

    {координаты области колонок и MProps}
    property StartColumnsAndMProps: TPoint read GetStartColumnsAndMProps;
    property EndColumnsAndMProps: TPoint read GetEndColumnsAndMProps;

    {номера начальных строк разрывов}
    property FiltersSplitStartRow: integer read GetFiltersSplitStartRow;
    property UnitMarkerSplitStartRow: integer read GetUnitMarkerSplitStartRow;
    property ColumnTitlesSplitStartRow: integer read GetColumnTitlesSplitStartRow;
    property ColumnsSplitStartRow: integer read GetColumnsSplitStartRow;
    property RowTitlesSplitStartRow: integer read GetRowTitlesSplitStartRow;
    property RowsSplitStartRow: integer read GetRowsSplitStartRow;

    property FilterSplitHeight: integer read FFiltersBreakHeight;
    property UnitMarkerSplitHeight: integer read FUnitMarkerBreakHeight;
    property ColumnTitlesSplitHeight: integer read FColumnTitlesBreakHeight;
    property ColumnsSplitHeight: integer read FColumnsBreakHeight;
    property RowTitlesSplitHeight: integer read FRowTitlesBreakHeight;
    property RowsSplitHeight: integer read FRowsBreakHeight;
    property NeedUnitMarker: boolean read FNeedUnitMarker;
  end;

  procedure HandleBrokenAxisForCellset(Dom: IXMLDOMDocument2;
    AxisType: TAxisType; AxisSize: integer; out ProcessRoute: TIntegerArray);

implementation

function TSheetSizer.UnknownPoint: TPoint;
begin
  result.x := 0;
  result.y := 0;
end;

function TSheetSizer.TableIsEmpty: boolean;
begin
 {Признаком пустой таблицы считаем совпадение точки конца и начала таблицы
  В одной ячейке ничего помещаться не может}
  result := (FTotalsCount + FRowsWidth + FColumnsHeight + FFiltersHeight = 0);
end;


function TSheetSizer.GetStartRows: TPoint;
begin
  if (RowsWidth > 0) then
  begin
    result.x := StartRowsTitle.x
      + IIF(FIsDisplayRowsTitles, 1, IIF((FTotalsCount > 0)
          and FIsDisplayTotalsTitles, 1, 0))
      + FRowTitlesBreakHeight;
    result.y := StartRowsTitle.y;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartColumns: TPoint;
begin
  if (ColumnsHeight > 0) then
  begin
    result.x := StartColumnsTitle.x + IIF(FIsDisplayColumnsTitles, 1, 0)
      + IIF(((ColumnMPropsCount > 0) and ColumnPlaceMPBefore),ColumnMPropsCount, 0)
      + FColumnTitlesBreakHeight;

    result.y := StartColumnsTitle.y;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartFilters: TPoint;
begin
  if (FiltersHeight > 0) then
    result := StartTable
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartTotals: TPoint;
begin
  if (FTotalsCount > 0) then
  begin
    result.x := StartTotalsTitle.x
      + IIF(FIsDisplayTotalsTitles, 1, IIF((FRowsHeight > 0)
          and FIsDisplayRowsTitles, 1, 0))
      + FRowTitlesBreakHeight;
    result.y := StartTotalsTitle.y;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndTable: TPoint;
begin
  result := GetEndTable_(true);
end;

function TSheetSizer.GetEndTableWithoutId: TPoint;
begin
  result := GetEndTable_(false);
end;

function TSheetSizer.GetEndTable_(IsWithId: boolean): TPoint;
var
  tmpInt: integer;
  FiltersOnly: boolean;
begin
  if TableIsEmpty then
  begin
    result := StartTable;
    exit;
  end;

  FiltersOnly := (FTotalsCount + FRowsWidth + FColumnsHeight = 0);

  {Вычисляем строку}
  tmpInt := StartTable.x
    + FiltersHeight
    + IIF(FNeedUnitMarker, 1, 0)
    + IIF((ColumnsHeight) > 0, ColumnsHeight + IIF(FIsDisplayColumnsTitles, 1, 0), 0)
    + IIF((ColumnMPropsCount) > 0, ColumnMPropsCount, 0)
    + RowsTotalsTitleHeight
    + Max(RowsHeight, IIF(FTotalsCount > 0, 1, 0))
    + FFiltersBreakHeight
    + FUnitMarkerBreakHeight
    + FColumnTitlesBreakHeight
    + FColumnsBreakHeight
    + FRowTitlesBreakHeight
    + FRowsBreakHeight;

  dec(tmpInt); //лишняя единичка
  if IsWithId then
    tmpInt := tmpInt + IIF(FSheetIdHeight > 0, FSheetIdSplit + FSheetIdHeight, 0);
  result.x := tmpInt;

  {Вычисляем колонку}
  tmpInt := StartTable.y;
  if not FiltersOnly then
    tmpInt := tmpInt
      + IIF((RowsWidth > 0), RowsWidth, 0)
      + IIF((RowMPropsCount > 0), RowMPropsCount, 0)
      + IIF((ColumnsWidth > 0), ColumnsWidth + FIgnoredColumnsTotalsCount - 1,
          FTotalsCount - 1);
  {Если длина фильтров больше вычесленной колонки, то за конец таблицы считаем ее}
  if not FIsMergeFilterCellsByTable then
    tmpInt := IIF((tmpInt >= EndFilters.y), tmpInt, EndFilters.y);
  result.y := tmpInt;
{!!! а здесь добавляем ширину мп в строках }
end;

function TSheetSizer.GetEndRows: TPoint;
begin
  if (RowsWidth > 0) then
  begin
    result.x := IIF(RowsHeight > 0, StartRows.x + RowsHeight - 1, StartRows.x);
    result.y := StartRows.y + RowsWidth - 1;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndColumns: TPoint;
begin
  if (ColumnsHeight > 0) then
  begin
    result.x := StartColumns.x + ColumnsHeight - 1;
    result.y := IIF(ColumnsWidth > 0, StartColumns.y + ColumnsWidth - 1, StartColumns.y);
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndFilters: TPoint;
begin
  if (FFiltersHeight > 0) then
  begin
    result.x := StartFilters.x + FiltersHeight - 1;
    if FIsMergeFilterCellsByTable then
      result.y := EndTable.y
    else
      result.y := StartFilters.y + FFilterCellsLength - 1;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndTotals: TPoint;
var tmpInt: integer;
begin
  if (FTotalsCount = 0) then
  begin
    result := UnknownPoint;
    exit;
  end;

  {Вычисляем строку}
  tmpInt := StartTotals.x;
  if (RowsHeight > 0) then
    tmpInt := tmpInt + RowsHeight - 1;
  result.x := tmpInt;

  {Вычисляем колонку}
  tmpInt := StartTotals.y
    + IIF((ColumnsWidth > 0),ColumnsWidth + FIgnoredColumnsTotalsCount - 1,
        FTotalsCount - 1);
  result.y := tmpInt;
end;

function TSheetSizer.GetStartSheetId: TPoint;
begin
  if TableIsEmpty then
  begin
    result := UnknownPoint;
    exit;
  end;

  result.x := RowsTotalsAreaEndRow;
  if (result.x > 0) then //есть строки или показатели
    result.x := result.x + FRowsBreakHeight
  else
  begin
    result.x := ColumnsAreaEndRow;
    if (result.x > 0) then //есть столбцы
      result.x := result.x
        + FColumnsBreakHeight
        + FRowTitlesBreakHeight
        + FRowsBreakHeight
    else //ну уж фильтры-то есть точно, ведь таблица не пустая
      result.x := EndFilters.x
        + FFiltersBreakHeight
        + FUnitMarkerBreakHeight
        + FColumnTitlesBreakHeight
        + FColumnsBreakHeight
        + FRowTitlesBreakHeight
        + FRowsBreakHeight;
  end;
  result.x := result.x + FSheetIdSplit + 1;
  result.y := StartTable.y;
end;

function TSheetSizer.GetEndSheetId: TPoint;
begin
  result.x := GetStartSheetId.x + FSheetIdHeight - 1;
  result.y := GetStartSheetId.y;
end;

function TSheetSizer.GetStartRowsTitle: TPoint;
var
  tmpInt, ColumnsAreaHeight: integer;
begin
  if (RowsWidth > 0) then
  begin
    ColumnsAreaHeight := IIF((ColumnsHeight) > 0, ColumnsHeight +
      IIF(FIsDisplayColumnsTitles, 1, 0), 0)
      + IIF((ColumnMPropsCount) > 0, ColumnMPropsCount, 0);

    tmpInt := StartTable.x
      + FiltersHeight
      + IIF(FNeedUnitMarker, 1, 0)
      + ColumnsAreaHeight
      + FFiltersBreakHeight
      + FUnitMarkerBreakHeight
      + FColumnTitlesBreakHeight
      + FColumnsBreakHeight;

    result.x := tmpInt;
    {Прибавляем еще область мп, если она есть}
    result.y := StartTable.y
      + IIF((RowMPropsCount > 0) and RowPlaceMPBefore, RowMPropsCount, 0);
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartColumnsTitle: TPoint;
var tmpInt: integer;
begin
  if (ColumnsHeight > 0) then
  begin
    tmpInt := StartTable.x
      + FiltersHeight
      + IIF(FNeedUnitMarker, 1, 0)
      + FFiltersBreakHeight
      + FUnitMarkerBreakHeight;

    result.x := tmpInt;
    result.y := StartTable.y
      + RowsWidth
      + IIF((RowMPropsCount > 0), RowMPropsCount, 0);
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartTotalsTitle: TPoint;
begin
  if (FTotalsCount = 0) then
  begin
    result := UnknownPoint;
    exit;
  end;

  {!!! Это вычисление немного по другому сделано.
  Здесь считается от других элментов, а не от первичных значений.
  может это зря}

  if (RowsWidth > 0) then
  begin //есть строки
    result.x := StartRowsTitle.x;
    result.y := StartRowsTitle.y
      + RowsWidth
      + IIF(((RowMPropsCount > 0) and not RowPlaceMPBefore),
          RowMPropsCount, 0);

  end
  else
    if (ColumnsHeight > 0) then
    begin //есть столбцы
      result.x := EndColumns.x + 1
        + IIF(((ColumnMPropsCount > 0) and not ColumnPlaceMPBefore),
            ColumnMPropsCount, 0)
        + FColumnsBreakHeight;
      result.y := StartColumns.y;
    end
    else
      if (FiltersHeight > 0) then
      begin //есть фильтры
        result.x := EndFilters.x
          + IIF(FNeedUnitMarker, 2, 1)
          + FFiltersBreakHeight
          + FUnitMarkerBreakHeight
          + FColumnTitlesBreakHeight
          + FColumnsBreakHeight;
        result.y := StartTable.y;
      end
      else
      begin //нет ничего кроме итогов
        result := StartTable;
        result.x := result.x
          + IIF(FNeedUnitMarker, 1, 0)
          + FFiltersBreakHeight
          + FUnitMarkerBreakHeight
          + FColumnTitlesBreakHeight
          + FColumnsBreakHeight;
      end;
end;

{Ищет начальные координаты всей таблицы}
function TSheetSizer.FindTableStartPosition(ExcelSheet: ExcelWorkSheet;
  FLCID: integer): TPoint;
var
  tmpRange: ExcelRange;
begin
  {Сначала ищем диапазон уже существующей таблицы}
  tmpRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));


  if not Assigned(tmpRange) then
  begin
    tmpRange := ExcelSheet.Application.ActiveCell;
    {нижеследующий код служит для удовлетворения требований задач 3848 и 3886;
      никакой _полезной_ функциональности не содержит}
    (* решено было убрать эту "фичу", чтобы избежать "перескакивания" таблицы
    if Assigned(tmpRange) then
    begin
      x := tmpRange.Row;
      y := tmpRange.Column;
      repeat
        RowOK := true;
        for CInd := 1 to 256 do
        begin
          tmpRange2 := GetRange(ExcelSheet, x, Cind, x, Cind);
          AName := GetNameByRange(ExcelSheet, tmpRange2);
          if AName <> '' then
          begin
            RowOK := false;
            break;
          end;
        end;
        if not RowOk then
        begin
          inc(x);
          tmpRange := GetRange(ExcelSheet, x, y, x, y);
        end;
      until RowOk or (x = 65536);
    end;*)
  end;

  if Assigned(tmpRange) then
  begin
    result.x := tmpRange.Row;
    result.y := tmpRange.Column;
  end
  else //если все равно нет - берем начало координат
  begin
    result.x := 1;
    result.y := 1;
  end;
end;


procedure TSheetSizer.InitAxisSizeByAxisDOM(Axis: TAxisType; AxisDOM: IXMLDOMDocument2;
  SMD: TSheetInterface);

  procedure EmptyAxis;
  begin
    if (Axis = axColumn) then
    begin
      FCellSize := 1;
      FColumnsWidth := 0;
    end
    else
      FRowsHeight := 0;
  end;

var
  Node: IXMLDOMNode;
begin
  if Assigned(AxisDOM) then
  begin
    Node := AxisDOM.selectSingleNode('function_result/Members');
    if Assigned(Node) then
    begin
      if (Axis = axColumn) then
      begin
        FCellSize := SMD.Totals.CountWithPlacement(false);
        if (FCellSize <= 0) then
          FCellSize := 1;
        FColumnsWidth := LeafCount(Node, FCellSize);
      end
      else
        FRowsHeight := LeafCount(Node, 1);
    end
    else
      EmptyAxis;
  end
  else
    EmptyAxis;
end;

procedure TSheetSizer.InitAxisSizeByCellsetDOM(Axis: TAxisType; CellsetDOM: IXMLDOMDocument2;
  SMD: TSheetInterface);
var
  TuplesNL: IXMLDOMNodeList;
  XPath: string;
begin
  if Assigned(CellsetDOM) then
  begin
    //!!!!!!
    XPath := 'root/Axes/Axis[@name="%s"]/Tuples/Tuple[not (@%s)]';
    if (Axis = axColumn) then
      XPath := Format(XPath, ['Axis0', attrHiddenTuple])
    else
      XPath := Format(XPath, ['Axis1', attrHiddenTuple]);

    TuplesNL := CellsetDOM.selectNodes(XPath);


    if TuplesNL.length = 0 then
    begin //empty Axes
      if (Axis = axColumn) then
      begin
        FCellSize := 1;
        FColumnsWidth := 0;
      end
      else
        FRowsHeight := 0;
    end
    else
    begin
      if (Axis = axColumn) then
      begin
        FCellSize := SMD.Totals.CountWithPlacement(false);
        if (FCellSize <= 0) then
          FCellSize := 1;
        //FColumnsWidth := TuplesNL[0].childNodes.length;
        FColumnsWidth := TuplesNL.length;
      end
      else
      begin
        //FRowsHeight := TuplesNL[0].childNodes.length;
        FRowsHeight := TuplesNL.length;
      end;
    end;
  end;
end;


function TSheetSizer.UnitMarker: TPoint;
begin
  result := UnknownPoint;
  if (FNeedUnitMarker and (FTotalsCount > 0)) then
  begin
    result := StartTable;
    if FMarkerPosition = mpRight then
      result.y := EndTable.y;
    result.x := result.x
      + FiltersHeight
      + FFiltersBreakHeight
      + FilterSplit;
  end;
end;

function TSheetSizer.CellSize(Axis: TAxisType): integer;
begin
  result := 0;
  if (Axis = axColumn) then
    result := FCellSize;

  if (result <= 0) then
    result := 1;
end;

procedure TSheetSizer.CalculateBaseMetrics(SMD: TSheetInterface);

  function GetFiltersHeight: integer;
  var
    i: integer;
    FiltersOnly: boolean;
  begin
    result := 0;
    {!!!высота фильтров считается вместе со сплиттером (если он нужен)}
    FFilterSplit := 0;
    FiltersOnly := (FTotalsCount + FRowsWidth + FColumnsHeight = 0);
    if not SMD.IsDisplayFilters then
      exit;
    for i := 0 to SMD.Filters.Count - 1 do
    begin
      if SMD.Filters[i].IsPartial then
        continue;
      result := result + 2 + SMD.Filters[i].MemberProperties.CheckedCount;
    end;
    result := result + IIF((result > 0) and (not FiltersOnly), FFilterSplit, 0);
  end;

begin
  {Нужно инициализировать следующие поля:
    FFiltersHeight: integer;  - Вычисляем по метаданным
    FFilterSplit: integer;  - Задаем константой 1
    FRowsWidth: integer;   - Вычисляем по метаданным
    FColumnsHeight: integer;  - Вычисляем по метаданным
    FTotalsCount: integer;  - Вычисляем по метаданным
    FIgnoredColumnsTotalsCount:  - Вычисляем по метаданным

    FNeedUnitMarker - вычисляем по метаданным
    FSheetIdSplit: integer - Задаем константой 1
    FSheetIdHeight: integer - константой по числу полей идентификаторов
    FRowMPropsCount: integer; //кол-во mp в строках
    FColumnMPPropsCount: integer; //кол-во mp в столбцах
    FRowPlaceMPBefore: boolean; //помещать область mp вперед строк
    FColumnPlaceMPBefore: boolean; //помещать область mp вперед столбцов
    FFiltersBreakHeight // высота разрыва после фильтров
    FColumnsBreakHeight //то же после столбцов
    FRowsBreakHeight //то же после строк/показателей
  }

  FRowsWidth := SMD.Rows.FieldCount;
  FColumnsHeight := SMD.Columns.FieldCount;
  FTotalsCount := SMD.Totals.Count;


  FFiltersHeight := GetFiltersHeight;

  FIgnoredColumnsTotalsCount := SMD.Totals.CountWithPlacement(true);
  FNeedUnitMarker := (SMD.TotalMultiplier > tmE1) and (
    SMD.MarkerPosition <> mpHidden) and (FTotalsCount > 0);
  FMarkerPosition := SMD.MarkerPosition;
  FFilterCellsLength := SMD.FilterCellsLength;
  FIsMergeFilterCellsByTable := SMD.IsMergeFilterCellsByTable;
  FSheetIdSplit := IIF(SMD.IsDisplaySheetInfo, 1, 0);
  FSheetIdHeight := IIF(SMD.IsDisplaySheetInfo, 6, 0);;
  FRowMPropsCount := SMD.Rows.MPCheckedCount;
  FColumnMPropsCount := SMD.Columns.MPCheckedCount;
  FRowPlaceMPBefore := SMD.Rows.MPBefore;
  FColumnPlaceMPBefore := SMD.Columns.MPBefore;

  FFiltersBreakHeight := SMD.Breaks.Items[bpFilters].Height;
  FUnitMarkerBreakHeight := SMD.Breaks.Items[bpUnitMarker].Height;
  FColumnTitlesBreakHeight := SMD.Breaks.Items[bpColumnTitles].Height;
  FColumnsBreakHeight := SMD.Breaks.Items[bpColumns].Height;
  FRowTitlesBreakHeight := SMD.Breaks.Items[bpRowTitles].Height;
  FRowsBreakHeight := SMD.Breaks.Items[bpRows].Height;

  FIsDisplayColumnsTitles := SMD.IsDisplayColumnsTitles;
  FIsDisplayRowsTitles := SMD.IsDisplayRowsTitles;
  FIsDisplayTotalsTitles := SMD.IsDisplayTotalsTitles;
  FIsDisplayColumns := SMD.IsDisplayColumns;
  FIsDisplayFilters := SMD.IsDisplayFilters;

end;

procedure TSheetSizer.Init(ExcelSheet: ExcelWorksheet; FLCID: integer;
  SMD: TSheetInterface; RowsDOM, ColumnsDOM: IXMLDOMDocument2);
begin
  {
    Изначально вычисляются параметры, зависящие от старого листа и от
    данных нового листа
    (не от структуры нового листа)

    FStartTable: TPoint;  - Вычисляем по листу
    CellSize - Вычисляем по метаданным
    FColumnsWidth: integer; - Параметр
    FRowsHeight: integer;  - Параметр
  }

  FStartTable := FindTableStartPosition(ExcelSheet, FLCID);
  InitAxisSizeByAxisDOM(axRow, RowsDOM, SMD);
  InitAxisSizeByAxisDOM(axColumn, ColumnsDOM, SMD);

  {Теперь основные характеристики}
  CalculateBaseMetrics(SMD);
end;

procedure TSheetSizer.Init(ExcelSheet: ExcelWorksheet; FLCID: integer;
  SMD: TSheetInterface; CellSetData: IXMLDOMDocument2;
  out RowsProcessRoute, ColumnsProcessRoute: TIntegerArray);
begin
  {
    Изначально вычисляются параметры, зависящие от старого листа и от
    данных нового листа
    (не от структуры нового листа)

    FStartTable: TPoint;  - Вычисляем по листу
    CellSize - Вычисляем по метаданным
    FColumnsWidth: integer; - Параметр
    FRowsHeight: integer;  - Параметр
  }

  SetLength(RowsProcessRoute, 0);
  SetLength(ColumnsProcessRoute, 0);
  FStartTable := FindTableStartPosition(ExcelSheet, FLCID);
  if SMD.Rows.Broken then
  begin
    HandleBrokenAxisForCellset(CellsetData, axRow, SMD.Rows.Count, RowsProcessRoute);
    FRowsHeight := Length(RowsProcessRoute);
  end
  else
    InitAxisSizeByCellsetDOM(axRow, CellSetData, SMD);
  if SMD.Columns.Broken then
  begin
    HandleBrokenAxisForCellset(CellsetData, axColumn, SMD.Columns.Count, ColumnsProcessRoute);
    FColumnsWidth := Length(ColumnsProcessRoute);
  end
  else
    InitAxisSizeByCellsetDOM(axColumn, CellSetData, SMD);

  {Теперь основные характеристики}
  CalculateBaseMetrics(SMD);
end;


function TSheetSizer.GetStartRowMPropsTitle: TPoint;
begin
  if ((RowMPropsCount > 0) and (RowsWidth > 0)) then
  begin
    result.x := StartRowsTitle.x;
    result.y := IIF(RowPlaceMPBefore, StartTable.y, EndRows.y + 1);
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartRowMProps: TPoint;
begin
  if (RowMPropsCount > 0) then
  begin
    result.x := GetStartRowMPropsTitle.x
      + RowsTotalsTitleHeight
      + FRowTitlesBreakHeight;
    result.y := GetStartRowMPropsTitle.y;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndRowMProps: TPoint;
begin
  if (RowMPropsCount > 0) then
  begin
    result.x := GetStartRowMProps.x + RowsHeight - 1;
    result.y := GetStartRowMProps.y + RowMPropsCount - 1;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartColumnMPropsTitle: TPoint;
begin
  if ((ColumnMPropsCount > 0) and (ColumnsHeight > 0)) then
  begin
    result.x := GetStartColumnsTitle.x ;
    result.y := GetStartColumnsTitle.y + ColumnsHeight;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetStartColumnMProps: TPoint;
begin
  if ((ColumnMPropsCount > 0) and (ColumnsHeight > 0)) then
  begin
    result.x := IIF(ColumnPlaceMPBefore, (StartColumns.x - ColumnMPropsCount),
      (EndColumns.x + 1));
    result.y := StartColumnsTitle.y;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetEndColumnMProps: TPoint;
begin
  if ((ColumnMPropsCount > 0) and (ColumnsHeight > 0)) then
  begin
    result.x := GetStartColumnMProps.x + ColumnMPropsCount - 1;
    result.y := GetStartColumnMProps.y + ColumnsWidth - 1;
  end
  else
    result := UnknownPoint;
end;

function TSheetSizer.GetColumnsAreaStartRow: integer;
begin
  result := 0;
  if (ColumnsHeight = 0) then
    exit;
  if FIsDisplayColumnsTitles then
    result := StartColumnsTitle.x
  else
    if ((ColumnMPropsCount > 0) and FColumnPlaceMPBefore) then
      result := StartColumnMPropsTitle.x
    else
      result := StartColumns.x;
end;

function TSheetSizer.GetColumnsAreaEndRow: integer;
begin
  result := 0;
  if (ColumnsHeight = 0) then
    exit;
  if ((ColumnMPropsCount > 0) and (not FColumnPlaceMPBefore)) then
    result := GetEndColumnMProps.x
  else
    result := EndColumns.x;
end;

function TSheetSizer.GetRowsTotalsAreaStartRow: integer;
var
  RStart, TStart: integer;
begin
  if (RowsHeight > 0) then
    RStart := IIF(FIsDisplayRowsTitles, StartRowsTitle.x, StartRows.x)
  else
    RStart := 0;
  if (FTotalsCount > 0) then
    TStart := IIF(FIsDisplayTotalsTitles, StartTotalsTitle.x, StartTotals.x)
  else
    TStart := 0;
  if (RStart = 0) then
    if (TStart = 0) then
      result := 0 //нет ни строк, ни показателей
    else
      result := Tstart // есть показатели, нет строк
  else
    if (TStart = 0) then
      result := RStart // есть строки, нет показателей
    else
      result := Min(RStart, TStart); // есть и показатели и строки
end;

function TSheetSizer.GetRowsTotalsAreaEndRow: integer;
begin
  result := Max(EndRows.x, EndTotals.x);
end;

function TSheetSizer.GetRowsTotalsTitleHeight: integer;
begin
  result := IIF((FRowsHeight > 0) and FIsDisplayRowsTitles, 1,
    IIF((FTotalsCount > 0) and FIsDisplayTotalsTitles, 1, 0));
end;

function TSheetSizer.GetEndColumnsAndMProps: TPoint;
begin
  result.x := Max(EndColumnMProps.x, EndColumns.x);
  result.y := Max(EndColumnMProps.y, EndColumns.y);
end;

function TSheetSizer.GetStartColumnsAndMProps: TPoint;
begin
  result.x := Min(StartColumnMProps.x, StartColumns.x);
  if (result.x = 0) then
    result.x := StartColumns.x;

  result.y := Min(StartColumnMProps.y, StartColumns.y);
  if (result.y = 0) then
    result.y := StartColumns.y;
end;

function TSheetSizer.GetFiltersSplitStartRow: integer;
begin
  result := IIF((FiltersHeight > 0), (EndFilters.x + 1), StartTable.x);
end;

function TSheetSizer.GetUnitMarkerSplitStartRow: integer;
begin
  if (FNeedUnitMarker and (UnitMarker.x > 0)) then
    result := UnitMarker.x + 1
  else
    result := StartTable.x
      + FiltersHeight
      + FFiltersBreakHeight;
end;

function TSheetSizer.GetColumnTitlesSplitStartRow: integer;
begin
  if ((StartColumnsTitle.x > 0) and FIsDisplayColumnsTitles) then
    result := StartColumnsTitle.x + 1
  else
    result := StartTable.x
      + FiltersHeight
      + FFiltersBreakHeight
      + IIF(FNeedUnitMarker, 1, 0)
      + FUnitMarkerBreakHeight;
end;

function TSheetSizer.GetColumnsSplitStartRow: integer;
begin
  if (ColumnsAreaEndRow > 0) then
    result := ColumnsAreaEndRow + 1
  else
    result := StartTable.x
      + FiltersHeight
      + FFiltersBreakHeight
      + IIF(FNeedUnitMarker, 1, 0)
      + FUnitMarkerBreakHeight
      + FColumnTitlesBreakHeight;
end;

function TSheetSizer.GetRowTitlesSplitStartRow: integer;
begin
  if ((StartRowsTitle.x > 0) and FIsDisplayRowsTitles) then
    result := StartRowsTitle.x + 1
  else
    if ((StartTotalsTitle.x > 0) and FIsDisplayTotalsTitles) then
      result := StartTotalsTitle.x + 1
    else
      result := StartTable.x
        + FiltersHeight
        + FFiltersBreakHeight
        + IIF(FNeedUnitMarker, 1, 0)
        + FUnitMarkerBreakHeight
        + FColumnTitlesBreakHeight
        + IIF((FIsDisplayColumnsTitles and (FColumnsHeight > 0)), 1,0)
        + ColumnsHeight
        + ColumnMPropsCount
        + FColumnsBreakHeight;
end;

function TSheetSizer.GetRowsSplitStartRow: integer;
begin
  if RowsTotalsAreaEndRow > 0 then
    result := RowsTotalsAreaEndRow + 1
  else
    if ColumnsAreaEndRow > 0 then
      result := ColumnsAreaEndRow
        + FColumnsBreakHeight
        + FRowTitlesBreakHeight
        + 1
    else
      result := StartTable.x
        + FiltersHeight
        + FFiltersBreakHeight
        + IIF(FNeedUnitMarker, 1, 0)
        + FUnitMarkerBreakHeight
        + FColumnTitlesBreakHeight
        + IIF((FIsDisplayColumnsTitles and (FColumnsHeight > 0)), 1,0)
        + ColumnsHeight
        + ColumnMPropsCount
        + FColumnsBreakHeight
        + FRowTitlesBreakHeight;
end;

procedure TSheetSizer.TestClass;

  function PointToStr(AP: TPoint): string;
  begin
    result := '  "' + IntToStr(AP.x) + ',  ' + IntToStr(AP.y) + '"';
  end;

var
  Log: TextFile;
begin
  AssignFile(Log, 'D:\Sizer.txt');
  Rewrite(Log);

  WriteLn(Log, 'RowsHeight ' + IntToStr(RowsHeight));
  WriteLn(Log, 'RowsWidth ' + IntToStr(RowsWidth));
  WriteLn(Log, 'ColumnsHeight ' + IntToStr(ColumnsHeight));
  WriteLn(Log, 'ColumnsWidth ' + IntToStr(ColumnsWidth));
  WriteLn(Log, 'FiltersHeight ' + IntToStr(FiltersHeight));


  WriteLn(Log, 'StartTable' +  PointToStr(StartTable));
  WriteLn(Log, 'EndTable' + PointToStr(EndTable));

  WriteLn(Log, 'StartFilters' + PointToStr(StartFilters));
  WriteLn(Log, 'EndFilters' + PointToStr(EndFilters));

  WriteLn(Log, 'StartRowsTitle' + PointToStr(StartRowsTitle));
  WriteLn(Log, 'StartRows' + PointToStr(StartRows));
  WriteLn(Log, 'EndRows' + PointToStr(EndRows));

  WriteLn(Log, 'StartColumnsTitle' + PointToStr(StartColumnsTitle));
  WriteLn(Log, 'StartColumns' + PointToStr(StartColumns));
  WriteLn(Log, 'EndColumns' + PointToStr(EndColumns));


  WriteLn(Log, 'StartTotalsTitle' + PointToStr(StartTotalsTitle));
  WriteLn(Log, 'StartTotals' + PointToStr(StartTotals));
  WriteLn(Log, 'EndTotals' + PointToStr(EndTotals));

  WriteLn(Log, '------------------');
  CloseFile(Log);
end;

{ В серверном режиме при полностью сломанной иерархии оси  ее вид при размещении
  штатным образом очень сильно отличается от обычного режима. Ось выводится
  неправильно. Чтобы этого избежать, приходится нарушать правило
  "один кортеж - одна строка". Один и тот же кортеж вынужденно может отвечать
  более чем за одну строку таблицы, в максимуме - за N-1, где N - число
  измерений в оси. Из-за этого сайзер не способен априорно определить размер оси.
  Данная процедура совершает полный обход кортежей оси, определяет ее размер
  и строит "карту" этого маршрута, по которой мапер будет ее размещать.
  По сути дела, если бы не проблема определения размера оси, эта процедура
  и являлась бы мапингом.}
procedure HandleBrokenAxisForCellset(Dom: IXMLDOMDocument2;
  AxisType: TAxisType; AxisSize: integer; out ProcessRoute: TIntegerArray);

  { Ищет следующий за текущим кортеж, отличающийся в указанном мембере.}
  function FindFirstDifferentTuple(Tuples: IXMLDOMNodeList;
    const TupleIndex, MemberIndex: integer): integer;
  var
    i: integer;
  begin
    result := -1;
    for i := TupleIndex + 1 to Tuples.length - 1 do
      if CompareTuples(Tuples[TupleIndex], Tuples[i], MemberIndex + 1) = MemberIndex then
      begin
        result := i;
        exit;
      end;
  end;

  function ChangeMember(Tuples: IXMLDOMNodeList;
    const TupleIndex, MemberIndex: integer; out LevelNumber: integer): IXMLDOMNode;
  begin
    result := Tuples[TupleIndex].childNodes[MemberIndex];
    LevelNumber := MemberLNum(result);
  end;

var
  Tuples: IXMLDOMNodeList;
  Member, Bookmark: IXMLDOMNode;
  MemberIndex, TupleIndex, Offset,
  LevelNumber, PrevLevelNumber, Correction, NextDifferentIndex: integer;
begin
  SetLength(ProcessRoute, 0);
  Tuples := GetAxisVisibleTuples(DOM, AxisType);
  if Tuples.length = 0 then
    exit;

  TupleIndex := 0; // порядковый номер текущего кортежа
  MemberIndex := 0; // порядковый номер мембера в кортеже
  Offset := 0;
  Correction := 0;
  Member := ChangeMember(Tuples, TupleIndex, MemberIndex, LevelNumber);
  while TupleIndex < Tuples.length do
  begin
    if not IsItSummary(Member) and (MemberCaption(Member) <> '') then
    begin
      inc(Offset);
      SetLength(ProcessRoute, Offset);
      ProcessRoute[Offset - 1] := TupleIndex * AxisSize + MemberIndex;
      if Correction = 0 then
        Bookmark := Tuples[TupleIndex];
      PrevLevelNumber := LevelNumber;

      { Учет одинаковых кортежей в столбцах, различающихся только показателями}
      if (AxisType = axColumn) and (TupleIndex + 1 < Tuples.length) then
      begin
        inc(TupleIndex);
        inc(Correction);
        if CompareTuples(Bookmark, Tuples[TupleIndex], AxisSize) = -1 then
        begin
          continue;
        end
        else
        begin
          if MemberIndex < AxisSize - 1 then
            dec(TupleIndex, Correction)
          else
            dec(TupleIndex);
          Correction := 0;
        end;
      end;

    { Не последний мембер кортежа, есть куда заглубляться}
    if MemberIndex < AxisSize - 1 then
    begin
      { Ищем первый кортеж, отличающийся в текущем мембере}
      NextDifferentIndex := FindFirstDifferentTuple(Tuples, TupleIndex, MemberIndex);
      { Если не нашли, то переходим к следующему мемберу в кортеже}
      if NextDifferentIndex = -1 then
      begin
        inc(MemberIndex);
        Member := ChangeMember(Tuples, TupleIndex, MemberIndex, LevelNumber);
        continue;
      end;
      { Нашли, и уровень мембера ниже текущего - размещаем}
      Member := ChangeMember(Tuples, NextDifferentIndex, MemberIndex, LevelNumber);
      if LevelNumber > PrevLevelNumber then
      begin
        TupleIndex := NextDifferentIndex;
        continue;
      end;
      { Нашли, но это тот же или даже вышестоящий уровень - надо идти вглубь}
      inc(MemberIndex);
      Member := ChangeMember(Tuples, TupleIndex, MemberIndex, LevelNumber);
      continue;
    end;
    end;

    { Мембер последний, заглубляться некуда, переходим к следующему кортежу}
    inc(TupleIndex);
    if TupleIndex = Tuples.length then
      exit;

    MemberIndex := CompareTuples(Tuples[TupleIndex],
      Tuples[TupleIndex - 1], AxisSize);
    Member := ChangeMember(Tuples, TupleIndex, MemberIndex, LevelNumber);
  end;
end;


end.
