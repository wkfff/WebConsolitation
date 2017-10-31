unit uMSWordUtils;

interface

uses
  uFMExcelAddinConst, uSheetObjectModel, SysUtils, Classes, uSheetBreaks,
  ExcelXP, uFMAddinGeneralUtils, uSheetSizer, Windows, uExcelUtils;

const
  PSObjectCaption: array[TPSObject] of string = ('не выбранно', 'Таблицы',
    'Столбцов', 'Заголовков столбцов', '', '', '', '', '', '', '', '',
    'Строк/данных', 'Заголовков строк/данных', '', '', '', '', '', '',
    'Фильтров', '', '', '', '', '', '', '', 'Маркера (тыс.руб.)', '', '', '',
    'Разрыва после фильтров', 'Разрыва после маркера (тыс.руб.)',
    'Разрыва после заг. столбцов', 'Разрыва после столбцов',
    'Разрыва после заг. строк/данных', 'Разрыва после строк/данных');

  {по названию вернет объект листа}
  function GetPSObjectByCaption(Caption: string): TPSObject;
  {вернет имена областей листа, до которых может продолжаться заголовок таблицы}
  function GetHeadingEndList(SheetInterface: TSheetInterface): TStringList;
  {вернет диапазон по указанаму адресу(xlA1)}
  function GetCustomHeadingRange(ExcelSheet: ExcelWorksheet; Address: string): ExcelRange;
  {возвращает конечный объект заголовка, если его уже нет, берем самый нижний
  достуный объект}
  function GetAllowedEndPSObject(SheetInterface: TSheetInterface): TPSObject;
  function GetHeadingEndPoint(Sizer: TSheetSizer; EndObject: TPSObject): TPoint;
  function GetHeadingRange(Sizer: TSheetSizer; SheetInterface: TSheetInterface): ExcelRange;

implementation

function GetPSObjectByCaption(Caption: string): TPSObject;
var
  PSObject: TPSObject;
begin
  result := psoNone;
  if (Trim(Caption) = '') then
    exit;
  for PSObject := Low(TPSObject) to High(TPSobject) do
    if (PSObjectCaption[PSObject] = Trim(Caption)) then
    begin
      result := PSObject;
      exit;
    end;
end;

function GetHeadingEndList(SheetInterface: TSheetInterface): TStringList;
begin
  result := TStringList.Create;
  if not Assigned(SheetInterface) then
    exit;
  with SheetInterface do
  begin
    {разрыв после фильтров}
    if (Breaks.Items[bpFilters].Height > 0) then
      result.Add(PSObjectCaption[psoFiltersBreak]);

    {маркер (тыс.руб)}
    if (TotalMultiplier > tmE1) and (Totals.Count > 0) then
      result.Add(PSObjectCaption[psoUnitMarker]);

    {разрыв после маркера (тыс.руб)}
    if (Breaks.Items[bpUnitMarker].Height > 0) then
      result.Add(PSObjectCaption[psoUnitMarkerBreak]);

    {заголовки столбцов}
    if (Columns.Count > 0) and (IsDisplayColumnsTitles) then
      result.Add(PSObjectCaption[psoColumnTitles]);

    {разрыв после заголовков столбцов}
    if (Breaks.Items[bpColumnTitles].Height > 0) then
      result.Add(PSObjectCaption[psoColumnTitlesBreak]);

    {столбцы}
    if (Columns.Count > 0) and (IsDisplayColumns) then
      result.Add(PSObjectCaption[psoColumns]);

    {разрыв после столбцов}
    if (Breaks.Items[bpColumns].Height > 0) then
      result.Add(PSObjectCaption[psoColumnsBreak]);

    {заголовки строк/данных}
    if ((Rows.Count > 0) and (IsDisplayRowsTitles)) or
    (Totals.Count > 0) and (IsDisplayTotalsTitles) then
      result.Add(PSObjectCaption[psoTotalTitles]);

    {разрывы после заголовков строк/показателей}
    if (Breaks.Items[bpRowTitles].Height > 0) then
      result.Add(PSObjectCaption[psoRowTitlesBreak]);

    {Строки/Данные}
    if (Totals.Count > 0) or (Rows.Count > 0) then
      result.Add(PSObjectCaption[psoTotals]);
  end;
end;

function GetCustomHeadingRange(ExcelSheet: ExcelWorksheet; Address: string): ExcelRange;
var
  Cell1, Cell2: OleVariant;
begin
  result := nil;
  if not (Assigned(ExcelSheet) and (Address <> '')) then
    exit;
  Cell1 := CutPart(Address, ':');
  Cell2 := Address;
  if (Cell2 = '') then
    Cell2 := EmptyParam;
  try
    result := ExcelSheet.Range[Cell1, Cell2];
  except
    result := nil;
  end;
end;

function GetAllowedEndPSObject(SheetInterface: TSheetInterface): TPSObject;
var
  ObjectList: TStringList;
  EndObjectName: string;
begin
  result := psoNone;
  ObjectList := GetHeadingEndList(SheetInterface);
  EndObjectName := PSObjectCaption[SheetInterface.SheetHeading.End_];
  if (ObjectList.Count = 0) then
    exit;
  {Если в списке доступных конечных объектов нет заявленого, выбираем первый
  доступный}
  if (ObjectList.IndexOf(EndObjectName) = -1) then
  begin
    result := GetPSObjectByCaption(ObjectList.Strings[ObjectList.Count - 1]);
    SheetInterface.ShowMessageEX(' указанного конечного объекта для заголовка' +
      ' таблицы - не существует', msgWarning);
  end
  else
    result := SheetInterface.SheetHeading.End_;
end;

function GetHeadingEndPoint(Sizer: TSheetSizer; EndObject: TPSObject): TPoint;
begin
  result := Classes.Point(0, 0);
  if not(Assigned(Sizer) and (EndObject <> psoNone)) then
    exit;
  case EndObject of
    psoColumns: result := Classes.Point(Sizer.StartColumns.x - 1,
      Sizer.EndTable.y);
    psoColumnTitles: result := Classes.Point(Sizer.StartColumnsTitle.x - 1,
      Sizer.EndTable.y);
    psoTotals: begin
      if (Sizer.StartTotals.x > 0) then
        result := Classes.Point(Sizer.StartTotals.x - 1, Sizer.EndTable.y)
      else
        result := Classes.Point(Sizer.StartRows.x - 1, Sizer.EndTable.y)
    end;
    psoTotalTitles: begin
      if (Sizer.StartTotalsTitle.x > 0) then
        result := Classes.Point(Sizer.StartTotalsTitle.x - 1, Sizer.EndTable.y)
      else
        result := Classes.Point(Sizer.StartRowsTitle.x - 1, Sizer.EndTable.y)
    end;
    psoUnitMarker: result := Classes.Point(Sizer.UnitMarker.x - 1, Sizer.EndTable.y);
    psoFiltersBreak: result := Classes.Point(Sizer.FiltersSplitStartRow - 1,
      Sizer.EndTable.y);
    psoUnitMarkerBreak: result := Classes.Point(Sizer.UnitMarkerSplitStartRow - 1,
      Sizer.EndTable.y);
    psoColumnTitlesBreak: result := Classes.Point(Sizer.ColumnTitlesSplitStartRow - 1,
      Sizer.EndTable.y);
    psoColumnsBreak: result := Classes.Point(Sizer.ColumnsSplitStartRow - 1,
      Sizer.EndTable.y);
    psoRowTitlesBreak: result := Classes.Point(Sizer.RowTitlesSplitStartRow - 1,
      Sizer.EndTable.y);
  end;
end;

function GetHeadingRange(Sizer: TSheetSizer;
  SheetInterface: TSheetInterface): ExcelRange;
var
  EndObject: TPSObject;
  StartPoint, EndPoint: TPoint;
begin
  result := nil;
  if not(Assigned(Sizer) and Assigned(SheetInterface)) then
    exit;

  StartPoint := Sizer.StartTable;
  if (StartPoint.x = 0) or (StartPoint.y = 0) then
    exit;
  EndObject := GetAllowedEndPSObject(SheetInterface);
  if (EndObject = psoNone) then
    exit;
  EndPoint := GetHeadingEndPoint(Sizer, EndObject);
  if (EndPoint.x = 0) or (EndPoint.y = 0) then
    exit;
  if (StartPoint.x > EndPoint.x) or (StartPoint.y > EndPoint.y) then
    exit;
  result := GetRange(SheetInterface.ExcelSheet, StartPoint, EndPoint);
end;

end.
