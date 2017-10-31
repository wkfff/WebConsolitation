{
  Лист планирования, который умеет собирать данные с листа.
  Сборы проводятся для обратной записи, а так же для сохранения введенных
  пользователем данных.
  (!) На сегодняшний момент, это самый последний потомок TSheetInterface
  => Является реально используемым корнем объектной модели.
}

unit uSheetCollector;

interface

uses
  Windows, Classes, SysUtils, MSXML2_TLB, uXMLUtils, ExcelXP, PlaningProvider_TLB,
  uXMLCatalog, uFMExcelAddinConst, PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils,
  uSheetObjectModel, uSheetMaperCellset, uExcelUtils, uGlobalPlaningConst;

type

  { Кэшированные данные, соответствующие ячейке оси.
    Data - ссылка на узел с набором осевых координат (Alias = UniqueName).

    Последующие параметры действительны только для оси столбцов:
    Total - ссылка на показатель, размещенный в столбце;
    TotalRange - ссылка на его диапазон;
    SectionIndex - номер его секции;

    Служебные признаки:
    Stored - показывает, что данные о показателе уже сохранены;
    FormulaArrayProcessed - сложная формула в этом столбце уже была обработана.
    }
  TAxisCellInfo = record
    Data: IXMLDOMNode;
    Total: TSheetTotalInterface;
    TotalRange: ExcelRange;
    SectionIndex: integer;
    Stored: boolean;
    FormulaArrayProcessed: boolean;
  end;
  TAxisInfo = array of TAxisCellInfo;


  { лист, который умеет собирать данные с листа }
  TSheetCollector = class(TSheetMaperCellset)
  private

    FTotalTypes: TTotalTypes;
    FDataElement, FIgnoredDataElement, FFormulaArrayDataElement: IXMLDOMNode;
    FFirstCollectedRow: integer;

    {Кэш осевых координат}
    RowsInfoDom, ColumnsInfoDom: IXMLDOMDocument2;
    RowsInfo, ColumnsInfo: TAxisInfo;

    {список сопоставления фуллнеймов измерений алиасам для обратной записи
      пример строки d.Regions.Plan=c5}
    ComparisonList: TStringList;


    // инициализация документа
    procedure InitDocument(var Document: IXMLDOMDocument2; FunctionName: string; var DataElement: IXMLDOMNode);
    procedure InitDocuments;
    // проверка на корректность числового значения
    function CheckCellValue(CellValue: string; Row, Column: integer): boolean;
    procedure AddSchemaMetadata(Total: TSheetTotalInterface;
      var RequestElement: IXMLDOMNode);
    procedure ParseCellRef(CellRef: string; out ReplacedCellRef, Column,
      Row: string; out ColumnNumber, RowNumber: integer; out IsOtherSheet,
      IsAbsolute: boolean);
    {Сохраняет в дочернем по отношению к DataNode узле кодированную формулу из текущей ячейки}
    procedure AddFormula(Formula: string; var FormulaNode: IXMLDOMNode; CurrentRow: integer);
    {Сохраняет в дочернем по отношению к DataNode узле кодированную типовую формулу из текущей ячейки}
    procedure AddTypeFormula(FormulaTotal: TSheetTotalInterface; Formula: string;
      var FormulaNode: IXMLDOMNode; CurrentRow, CurrentColumn: integer);
    {Переписывает кэшированные значения атрибутов пкид, используя
      таблицу сопоставления фуллмемберов алиасам.}
    procedure CopyCachedData(CacheNode, DataNode: IXMLDOMNode);
    {По метаданным узла запроса строит таблицу сопоставления}
    procedure BuildComparisonList(RequestNode: IXMLDOMNode);
    procedure ReadMarkupDocuments;
    {При новой разметке заполняет осевыми координатами узел параметра формулы}
    procedure AddAxisDataToFormulaParam(AxisType: TAxisType; Shift: integer;
      var ParamNode: IXMLDOMNode);



    // получить данные отдельной ячейки для обратной записи
    function GetWritebackSingleCellData(SingleCell: TSheetSingleCellInterface;
      var RequestElement: IXMLDOMNode; EraseEmptyCells: boolean): boolean;
    {Данные для обратной записи из текущей ячейки}
    procedure AddWritebackTotalData(Total: TSheetBasicTotal;
      ElementValue: widestring; var Element: IXMLDOMNode;
      Style: string; Alias: string); overload;
    {Непосредственное получение осевых координат для обратной записи}
    function GetWritebackAxisData(AxisType: TAxisType; Coord: integer;
      var AxisData: IXMLDOMNode): boolean;
    {Получение данных для записи табличных показателей}
    procedure GetWritebackTotalsData(Root: IXMLDOMNode; EraseEmptyCells: boolean);
    procedure GetWritebackTotalsDataNew(Root: IXMLDOMNode;
      EraseEmptyCells: boolean);


    {Собирает данные отдельных ячеек для размещения}
    procedure GetCollectableCellsData;
    {Прописывает в DataNode значение из текущей ячейки}
    function AddTotalData(RowIndex, ColumnIndex: integer; var DataNode: IXMLDOMNode): boolean;
    {Непосредственное получение осевых координат для размещения}
    function GetAxisData(AxisType: TAxisType; Coord: integer; var AxisData: IXMLDOMNode): boolean;
    {Кэширование данных показателя из указанного столбца}
    function CacheTotalInfo(ColumnIndex: integer): boolean;
    {Получение и кэширование осевых координат.}
    function GetCachedAxisData(AxisType: TAxisType; Coord: integer;
      var DataNode: IXMLDOMNode; IsWriteback: boolean): boolean;
    {получение осевых координат для отдельного показателя в таблице - новая разметка}
    procedure GetCellMarkupData(Cell: TSheetSingleCellInterface; var DataNode: IXMLDOMNode);
    {Собственно сбор данных табличных показателей для размещения}
    procedure GetCollectableTotalsData;
    procedure GetCollectableTotalsDataNew;
    procedure GetOneTotalData(Total: TSheetTotalInterface);

    { получить данные табличных показателей для размещения}
    procedure CollectTableData;
    {получить данные отдельных ячеек для размещения}
    procedure CollectSingleCellsData;
  protected
    // получить нужные для дальнейшей работы коллектора координаты
    procedure GetOffsets; override;
  public
    {получить данные для размещения}
    procedure CollectTotalsData(SingleCellsOnly: boolean); override;
    {сбор данных одного показателя для специального режима обновления одного
      показателя}
    function CollectOneTotalData(Total: TSheetTotalInterface): boolean; override;
    {получить данные для обратной записи}
    function GetWritebackData(TaskId: widestring; EraseEmptyCells, ProcessCube: boolean): IXmlDomDocument2;

    function CheckWorkbookForResults: boolean; override;
    function GetTypeFormula(Total: TSheetTotalInterface; Row, Column: integer): TTypeFormula; override;
    {мапит типовую формулу конкретному показателю}
    procedure MapTypeFormula(Total: TSheetTotalInterface); override;
    {}
    function TypeFormulaToString(FormulaTotal: TSheetTotalInterface; CurrentRow,
      CurrentSectionIndex, GrandSummaryRow: integer): string; override;
    procedure Save; override;
  end;

implementation

procedure TSheetCollector.CollectTotalsData(SingleCellsOnly: boolean);
var
  RowsRootElement, ColumnsRootElement: IXMLDOMNode;
begin
  {Вспомогательные документы для кэширования}
  InitDocument(RowsInfoDom, '', RowsRootElement);
  InitDocument(ColumnsInfoDom, '', ColumnsRootElement);

  SetLength(RowsInfo, FLastRow - FFirstRow + 1);
  SetLength(ColumnsInfo, FLastColumn - FFirstColumn + 1);

  ReadMarkupDocuments;
  {Сбор табличных показателей}
  if not SingleCellsOnly then
    CollectTableData;
  {Сбор отдельных показателей}
  CollectSingleCellsData;

  KillDomDocument(RowsInfoDom);
  KillDomDocument(ColumnsInfoDom);
  try
    SetLength(RowsInfo, 0);
    SetLength(ColumnsInfo, 0);
  except
  end;
end;

function TSheetCollector.CollectOneTotalData(Total: TSheetTotalInterface): boolean;
var
  RowsRootElement, ColumnsRootElement: IXMLDOMNode;
begin
  result := false;
  ReadMarkupDocuments;
  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
  begin
    PostMessage('Для корректного получения данных показателя необходимо произвести полное обновление таблицы.', msgError);
    exit;
  end;
  result := true;
  if Total.TotalType <> wtResult then
    exit;
  {Вспомогательные документы для кэширования}
  InitDocument(RowsInfoDom, '', RowsRootElement);
  InitDocument(ColumnsInfoDom, '', ColumnsRootElement);

  SetLength(RowsInfo, FLastRow - FFirstRow + 1);
  SetLength(ColumnsInfo, FLastColumn - FFirstColumn + 1);

  OpenOperation(pfoCollectFreeTotalsData, CriticalNode, NoteTime, otProcess);
  try
    GetOneTotalData(Total);
    if AddinLogEnable then
    begin
      WriteDocumentLog(FreeTotalsData, 'Данные для размещения.xml');
      WriteDocumentLog(FreeTotalsDataIgnored, 'Данные для размещения_ignored.xml');
      WriteDocumentLog(FormulaArrays, 'Данные для размещения_formulaArrays.xml');
    end;
  finally
    CloseOperation;
  end;

  {Сбор отдельных показателей}
//  CollectSingleCellsData;

  KillDomDocument(RowsInfoDom);
  KillDomDocument(ColumnsInfoDom);
  SetLength(RowsInfo, 0);
  SetLength(ColumnsInfo, 0);
end;

procedure TSheetCollector.CollectSingleCellsData;
begin
  if SingleCells.Empty then
    exit;
  FTotalTypes := [wtResult, wtMeasure, wtConst];
  if not SingleCells.CheckByType(FTotalTypes) then
    exit;
  OpenOperation(pfoCollectSingleCellsData, CriticalNode, NoteTime, otProcess);
  try
    GetCollectableCellsData;
    if AddinLogEnable then
      WriteDocumentLog(SingleCellsData, 'Данные для размещения_singleCells.xml');
  finally
    CloseOperation;
  end;
end;

function TSheetCollector.CheckWorkbookForResults: boolean;
var
  tmpCollector: TSheetCollector;
  i: integer;
  ESheet: ExcelWorkSheet;
  EBook: ExcelWorkbook;
begin
  result := false;
  tmpCollector := TSheetCollector.Create;
  try
    EBook := (ExcelSheet.Parent as ExcelWorkbook);
    for i := 1 to EBook.WorkSheets.Count do
    begin
      ESheet := GetWorkSheet(EBook.Worksheets[i]);
      if not Assigned(ESheet) then
        continue;
      if tmpCollector.Load(ESheet, nil, [lmCollections]) then
      try
        if tmpCollector.Totals.CheckByType([wtResult]) or
          tmpCollector.SingleCells.CheckByType([wtResult]) then
        begin
          result := true;
          exit;
        end;
      finally
        tmpCollector.Clear;
      end;
    end;
  finally
    FreeAndNil(tmpCollector)
  end;
end;

procedure TSheetCollector.CollectTableData;
begin
  if Totals.Empty then
    exit;
  FTotalTypes := [wtFree, wtResult];
  if not Totals.CheckByType(FTotalTypes) then
    exit;
  if WritablesInfo.NoWritableColumns then
    exit;

  OpenOperation(pfoCollectFreeTotalsData, CriticalNode, NoteTime, otProcess);
  try
    if IsMarkupNew then
      GetCollectableTotalsDataNew
    else
      GetCollectableTotalsData;
    if AddinLogEnable then
    begin
      WriteDocumentLog(FreeTotalsData, 'Данные для размещения.xml');
      WriteDocumentLog(FreeTotalsDataIgnored, 'Данные для размещения_ignored.xml');
      WriteDocumentLog(FormulaArrays, 'Данные для размещения_formulaArrays.xml');
    end;
  finally
    CloseOperation;
  end;
end;

procedure TSheetCollector.GetOffsets;
var
  Range: ExcelRange;
  TotalsOk: boolean;
begin
  TotalsOk := false;
  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntTotals);
  if Assigned(Range) then
  begin
    FFirstRow := Range.Row;
    FLastRow := Range.Row + Range.Rows.Count - 1;
    FFirstColumn := Range.Column;
    FLastColumn := Range.Column + Range.Columns.Count - 1;
    TotalsOk := true;
  end;

  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntRows);
  if Assigned(Range) then
  begin
    FRowsLeaf := Range.Column + Range.Columns.Count - 1;
    if not TotalsOk then
    begin
      FFirstRow := Range.Row;
      FLastRow := Range.Row + Range.Rows.Count - 1;
    end;
  end;

  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntColumns);
  if Assigned(Range) then
  begin
    FColumnsLeaf := Range.Row + Range.Rows.Count - 1;
    if not TotalsOk then
    begin
      FFirstColumn := Range.Column;
      FLastColumn := Range.Column + Range.Columns.Count - 1;
    end;
  end;
  
  FGrandSummaryRow := GetGrandSummaryRow(ExcelSheet);
  FGrandSummaryColumn := GetGrandSummaryColumn(ExcelSheet);
end;

procedure TSheetCollector.InitDocument(var Document: IXMLDOMDocument2; FunctionName: string; var DataElement: IXMLDOMNode);

  procedure AddFunctionElement(var Document: IXmlDomDocument2; FunctionName: string);
  begin
    Document.DocumentElement := Document.createElement('function_result');
    Document.DocumentElement.setAttribute('function_name', FunctionName);
  end;

begin
  Document := InitXmlDocument;
  AddFunctionElement(Document, FunctionName);
  DataElement := CreateAndAddChild(Document.documentElement, 'data');
end;

function TSheetCollector.CheckCellValue(CellValue: string; Row, Column: integer): boolean;
var
  WarningText: string;
begin
  result := true;
  try
    StrToFloat(CellValue);
  except
    result := false;
    WarningText := 'Некорректное значение показателя: ' + CellValue + ' (ячейка ' + GetColumnName(Column) + IntToStr(Row) + ')';
    if GetProcessShowing then
      PostMessage(WarningText, msgWarning)
    else
      ShowError(WarningText);
  end;
end;

procedure TSheetCollector.AddWritebackTotalData(Total: TSheetBasicTotal;
  ElementValue: widestring; var Element: IXMLDOMNode; Style: string; Alias: string);
var
  Value: extended;
  tmpStr: string;
begin
  if (Style = snDataFreeErased) then
    ElementValue := 'null'
  else
    if NeedRound and (Total.ClassName = 'TSheetTotal') then
    begin
      // округляем значение
      Value := StrToFloat(ElementValue);
      Value := Total.GetDividedValue(Value);
      Total.Round(Value);
      Value := Total.GetMultipliedValue(Value);
      ElementValue := FloatToStr(Value);
    end;
  try
    SetAttr(Element, Alias, ElementValue);
  except
    on e: exception do
    begin
      tmpStr := e.Message;
      if Alias = '' then
        tmpStr := tmpStr + '. Алиас для обратной записи не может быть пустым.' +
          Format(' Показатель "%s". Проверьте свойство FullName в метаданных куба.', [Total.GetElementCaption]);
      PostMessage(tmpStr, msgError);
    end;
  end;
end;

function AddAxisData(AxisCollection: TSheetAxisCollectionInterface;
  Row, Column: integer; var Element: IXMLDOMNode; AliasIndex: integer): boolean;
var
  Params: TStringList;
  CurAxisElementIndex, i: integer;
  RangeName, PkID, LocalId: string;
  IsMemberLeaf: boolean;
  AxisCollectionCount: integer;
begin
  result := true;
  AxisCollectionCount := AxisCollection.Count;
  if (AxisCollectionCount = 0) or (Row = 0) or (Column = 0) then
    exit;

  RangeName := GetNameByRC(AxisCollection.Owner.ExcelSheet, Row, Column);
  { Благодаря дополнительному признаку в разметке можно прямо сейчас
    определить пригодность элемента для записи}
  if IsWritebackSenseless(RangeName) then
  begin
    result := false;
    exit;
  end;
  Params := nil;
  result := (RangeName <> '') and ParseExcelName(RangeName, Params) and
            (RangeName <> snNamePrefix + snSeparator + gsRow) and
            (RangeName <> snNamePrefix + snSeparator + gsColumn);
  if not result then
    exit;
  try
    // откидываем итоги
    result := (Params.IndexOf(fpEnd) = -1);
    if not result then
      exit;
    CurAxisElementIndex := 0;

    { Парочка проверок, которые возможно уже и не нужны...}
    if ((Params.Count - 3) < AxisCollection.MarkupFieldCount)
      and not AxisCollection.Broken then
    begin
      result := false;
      exit;
    end;
    if AxisCollection.Broken then
      if Params[1] <> AxisCollection[AxisCollectionCount - 1].UniqueID then
      begin
        result := false;
        exit;
      end;

    // локал ай ди начинается с 4 параметра
    for i := 3 to Params.Count - 1 do
    begin
      if Params[i] = fpDummy then
        continue;

      LocalId := Params[i];
      PkID := GetMemberAttrByKey(AxisCollection[CurAxisElementIndex].Members,
        attrLocalId, LocalId, attrPKID, IsMemberLeaf);
      if not IsMemberLeaf then
        continue;

      // если PKID не определен - выходим
      result := (PkId <> '') and (PkId <> 'null');
      if not result then
        exit;
      SetAttr(Element, 'c' + IntToStr(AliasIndex), PkID);
      inc(AliasIndex);
      // переход на следующий элемент строк
      inc(CurAxisElementIndex);
      if CurAxisElementIndex = AxisCollectionCount then
        exit;
    end;
  finally
    FreeStringList(Params);
  end;
end;

function TSheetCollector.GetWritebackAxisData(AxisType: TAxisType;
  Coord: integer; var AxisData: IXMLDOMNode): boolean;
var
  AxisCollection: TSheetAxisCollectionInterface;
  Params: TStringList;
  i, AxisCollectionCount, CurAxisElementIndex: integer;
  RangeName, PkID, LocalId: string;
  IsMemberLeaf: boolean;
begin
  result := true;
  AxisCollection := GetAxis(AxisType);
  AxisCollectionCount := AxisCollection.Count;
  if (AxisCollectionCount = 0) or (Coord = 0) then
    exit;
  if AxisType = axRow then
    RangeName := GetNameByRC(ExcelSheet, Coord, FRowsLeaf)
  else
    RangeName := GetNameByRC(ExcelSheet, FColumnsLeaf, Coord);
  if RangeName = '' then
    exit;

  { Благодаря дополнительному признаку в разметке можно прямо сейчас
    определить пригодность элемента для записи}
  if IsWritebackSenseless(RangeName) then
  begin
    result := false;
    exit;
  end;

  Params := nil;
  result := ParseExcelName(RangeName, Params);
  if not result then
    exit;

  try
    // откидываем итоги
    result := (Params.IndexOf(fpEnd) = -1);
    if not result then
      exit;
    CurAxisElementIndex := 0;

    { Парочка проверок, которые возможно уже и не нужны...}
    if ((Params.Count - 3) < AxisCollection.MarkupFieldCount)
      and not AxisCollection.Broken then
    begin
      result := false;
      exit;
    end;
    if AxisCollection.Broken then
      if Params[1] <> AxisCollection[AxisCollectionCount - 1].UniqueID then
      begin
        result := false;
        exit;
      end;

    // локал айди начинается с 4 параметра
    for i := 3 to Params.Count - 1 do
    begin
      if Params[i] = fpDummy then
        continue;

      LocalId := Params[i];
      PkID := GetMemberAttrByKey(AxisCollection[CurAxisElementIndex].Members,
        attrLocalId, LocalId, attrPKID, IsMemberLeaf);
      if not IsMemberLeaf then
        continue;

      // если PKID не определен - выходим
      result := (PkId <> '') and (PkId <> 'null');
      if not result then
        exit;

      SetAttr(AxisData, AxisCollection[CurAxisElementIndex].FullName, PkID);
      // переход на следующий элемент строк
      inc(CurAxisElementIndex);
      if CurAxisElementIndex = AxisCollectionCount then
        exit;
    end;
  finally
    FreeStringList(Params);
  end;
end;

procedure TSheetCollector.GetWritebackTotalsData(Root: IXMLDOMNode; EraseEmptyCells: boolean);

  function GetRequestNode(Total: TSheetTotalInterface; var DataNode: IXMLDOMNode): IXMLDOMNode;
  begin
    result := Root.selectSingleNode(Format('Request[@id="%s"]', [Total.UniqueId]));
    if not Assigned(result) then
    begin
      result := Root.ownerDocument.createNode(1, 'Request', '');
      SetAttr(result, 'id', Total.UniqueId);
      AddSchemaMetadata(Total, result);
      Root.appendChild(result);
      DataNode := CreateAndAddChild(result, 'Data');
    end
    else
      DataNode := result.selectSingleNode('Data');
  end;

  procedure RemoveEmptyRequests;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    DataNode: IXMLDOMNode;
  begin
    NL := Root.selectNodes('Request');
    for i := NL.length - 1 downto 0 do
    begin
      DataNode := NL[i].selectSingleNode('Data');
      if DataNode.childNodes.length = 0 then
        Root.removeChild(NL[i]);
    end;
  end;

var
  RequestNode, DataNode, TemplateNode, RowNode: IXMLDOMNode;
  i, RowIndex, ColumnIndex, CellCount, CellNumber, RowsCount, ColumnsCount: integer;
  CellValue, tmpStr, Style, PkId, Alias: string;
  tmpBool, ColumnInProcess: boolean;
begin
  RowsCount := FLastRow - FFirstRow + 1;
  ColumnsCount := FLastColumn - FFirstColumn + 1;
  CellCount := RowsCount * ColumnsCount ;
  CellNumber := 0;
  ColumnInProcess := false;

  for ColumnIndex := FFirstColumn to FLastColumn do
  try
    ColumnInProcess := false;
    if IsSummaryColumn(ColumnIndex) then
      continue;
    if not CacheTotalInfo(ColumnIndex) then
      continue;
    with ColumnsInfo[ColumnIndex - FFirstColumn] do
    begin
      if Total.TotalType <> wtResult then
        continue;
      RequestNode := GetRequestNode(Total, DataNode);
      BuildComparisonList(RequestNode);

      TemplateNode := RequestNode.ownerDocument.createNode(1, 'Row', '');

      for i := 0 to Filters.Count - 1 do
      begin
        if not Filters[i].IsAffectsTotal(Total) then
          continue;
        PkID := GetMemberAttrByKey(Filters[i].Members, 'checked', 'true', attrPKID, tmpBool);
        Alias := ComparisonList.Values[Filters[i].FullName];
        SetAttr(TemplateNode, Alias, PkID);
      end;
      if not Total.IsIgnoredColumnAxis then
        if not GetCachedAxisData(axColumn, ColumnIndex, TemplateNode, true) then
          continue;

      ColumnInProcess := true;
      for RowIndex := FFirstRow to FLastRow do
      try
        if IsSummaryRow(RowIndex) then
          continue;
        {Получаем значение из ячейки, пропускаем незаполненные}
        if not GetCellValue(ExcelSheet, RowIndex, ColumnIndex,
          Total.EmptyValueSymbol, CellValue, tmpStr, Style, EraseEmptyCells) then
          continue;
        if Style <> snDataFreeErased then
        begin
          {проверяем на корректность числового значения}
          if not CheckCellValue(CellValue, RowIndex, ColumnIndex) then
            continue;
          CellValue := Total.GetMultipliedValue(CellValue);
        end;

        RowNode := TemplateNode.cloneNode(false);
        if not GetCachedAxisData(axRow, RowIndex, RowNode, true) then
          continue;
        Alias := ComparisonList.Values[Total.FullName];
        AddWritebackTotalData(Total, CellValue, RowNode, Style, Alias);
        DataNode.appendChild(RowNode);
      finally
        inc(CellNumber);
        SetPBarPosition(CellNumber, CellCount);
      end;
    end;
  finally
    if not ColumnInProcess then
      CellNumber := CellNumber + RowsCount;
    SetPBarPosition(CellNumber, CellCount);
  end;

  RemoveEmptyRequests;
  FreeStringList(ComparisonList);
end;

procedure TSheetCollector.GetWritebackTotalsDataNew(Root: IXMLDOMNode; EraseEmptyCells: boolean);

  function GetRequestNode(Total: TSheetTotalInterface; var DataNode: IXMLDOMNode): IXMLDOMNode;
  begin
    result := Root.selectSingleNode(Format('Request[@id="%s"]', [Total.UniqueId]));
    if not Assigned(result) then
    begin
      result := Root.ownerDocument.createNode(1, 'Request', '');
      SetAttr(result, 'id', Total.UniqueId);
      AddSchemaMetadata(Total, result);
      Root.appendChild(result);
      DataNode := CreateAndAddChild(result, 'Data');
    end
    else
      DataNode := result.selectSingleNode('Data');
  end;

  procedure RemoveEmptyRequests;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    DataNode: IXMLDOMNode;
  begin
    NL := Root.selectNodes('Request');
    for i := NL.length - 1 downto 0 do
    begin
      DataNode := NL[i].selectSingleNode('Data');
      if DataNode.childNodes.length = 0 then
        Root.removeChild(NL[i]);
    end;
  end;

  procedure GetFiltersData(var Node: IXMLDOMNode; Total: TSheetTotalInterface);
  var
    j: integer;
    PkId, Alias: string;
    tmpBool: boolean;
  begin
    for j := 0 to Filters.Count - 1 do
    begin
      if not Filters[j].IsAffectsTotal(Total) then
        continue;
      PkID := GetMemberAttrByKey(Filters[j].Members, 'checked', 'true', attrPKID, tmpBool);
      Alias := ComparisonList.Values[Filters[j].FullName];
      SetAttr(Node, Alias, PkID);
    end;
  end;

  procedure GetCellData(RowIndex, ColumnIndex: integer; Total: TSheetTotalInterface;
    TemplateNode, InfoNode: IXMLDOMNode; var DataNode: IXMLDOMNode);
  var
    CellValue, tmpStr, Style, Alias: string;
    RowNode: IXMLDOMNode;
  begin
    {Получаем значение из ячейки, пропускаем незаполненные}
    if not GetCellValue(ExcelSheet, RowIndex, ColumnIndex,
      Total.EmptyValueSymbol, CellValue, tmpStr, Style, EraseEmptyCells) then
      exit;
    if Style <> snDataFreeErased then
    begin
      {проверяем на корректность числового значения}
      if not CheckCellValue(CellValue, RowIndex, ColumnIndex) then
        exit;
      CellValue := Total.GetMultipliedValue(CellValue);
    end;

    RowNode := TemplateNode.cloneNode(false);
    if not Rows.Empty then
      CopyAttrsWithConversion(InfoNode, RowNode, ComparisonList);

    Alias := ComparisonList.Values[Total.FullName];
    AddWritebackTotalData(Total, CellValue, RowNode, Style, Alias);
    DataNode.appendChild(RowNode);
  end;

var
  RowsNL, ColumnsNL: IXMLDOMNodeList;
  RowsCount, ColumnsCount, CellsCount, CellNumber, ColumnIndex, RowIndex, i, j, k,
    ExtraColumnsCount, TotalShift: integer;
  ColumnInProcess: boolean;
  InfoNode, RequestNode, DataNode, TemplateNode: IXMLDOMNode;
  Total: TSheetTotalInterface;


  TotalsNL: IXMLDOMNodeList;
begin

  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
    exit;

  RowsNL := RowsMarkup.selectNodes(Format('markup/row[@%s="true"]', [attrWbWorthy]));
  RowsCount := RowsNL.length;
  ColumnsNL := ColumnsMarkup.selectNodes(Format('markup/column[@%s="true"]', [attrWbWorthy]));
  ColumnsCount := ColumnsNL.length;

  if Columns.Empty then
    ExtraColumnsCount := Totals.Count
  else
    ExtraColumnsCount := Totals.CountWithPlacement(true);



  TotalsNL := ColumnsMarkup.selectNodes('markup/totals/total[@writable="true"]');
  if (TotalsNL.length = 0) and (ExtraColumnsCount = 0) then
    exit;
  CellsCount := RowsCount * (ColumnsCount * TotalsNL.length + ExtraColumnsCount);
  CellNumber := 0;

  if not Columns.Empty then
  begin
    ColumnInProcess := false;
    for i := 0 to TotalsNL.length - 1 do
    begin
      Total := Totals.FindByAlias(GetStrAttr(TotalsNL[i], attrAlias, ''));
      TotalShift := GetIntAttr(TotalsNL[i], attrShift, -BeastNumber);
      RequestNode := GetRequestNode(Total, DataNode);
      BuildComparisonList(RequestNode);
      TemplateNode := RequestNode.ownerDocument.createNode(1, 'Row', '');

      {Пишем данные фильтров}
      GetFiltersData(TemplateNode, Total);

      for j := 0 to ColumnsCount - 1 do
      try
        ColumnInProcess := false;
        InfoNode := ColumnsNL[j].selectSingleNode('writeback');
        if not Assigned(InfoNode) then
          continue;

        ColumnIndex := FFirstColumn + GetIntAttr(ColumnsNL[j], attrShift, -BeastNumber) + TotalShift;
        if not CacheTotalInfo(ColumnIndex) then
          continue;

        {пишем данные столбцов}
        ColumnInProcess := true;
        CopyAttrsWithConversion(InfoNode, TemplateNode, ComparisonList);

        for k := 0 to RowsCount - 1 do
        try
          InfoNode := RowsNL[k].selectSingleNode('writeback');
          if not Assigned(InfoNode) and not Rows.Empty then
            continue;

          RowIndex := FFirstRow + GetIntAttr(RowsNL[k], attrShift, -BeastNumber);

          GetCellData(RowIndex, ColumnIndex, Total, TemplateNode, InfoNode, DataNode);

        finally
          inc(CellNumber);
          SetPBarPosition(CellNumber, CellsCount);
        end;
      finally
        if not ColumnInProcess then
          CellNumber := CellNumber + RowsCount;
        SetPBarPosition(CellNumber, CellsCount);
      end;
    end;
  end;

  {обработка показателей за осью столбцов}
  ColumnInProcess := false;
  for ColumnIndex := FLastColumn - ExtraColumnsCount + 1 to FLastColumn do
  try
    ColumnInProcess := false;
    if not CacheTotalInfo(ColumnIndex) then
      continue;
    Total := ColumnsInfo[ColumnIndex - FFirstColumn].Total;
    if Total.TotalType <> wtResult then
      continue;
    RequestNode := GetRequestNode(Total, DataNode);
    BuildComparisonList(RequestNode);
    TemplateNode := RequestNode.ownerDocument.createNode(1, 'Row', '');
    {Пишем данные фильтров}
    GetFiltersData(TemplateNode, Total);
    ColumnInProcess := true;

    for k := 0 to RowsCount - 1 do
    try
      InfoNode := RowsNL[k].selectSingleNode('writeback');
      if not Assigned(InfoNode) and not Rows.Empty then
        continue;

      RowIndex := FFirstRow + GetIntAttr(RowsNL[k], attrShift, -BeastNumber);

      GetCellData(RowIndex, ColumnIndex, Total, TemplateNode, InfoNode, DataNode);
    finally
      inc(CellNumber);
      SetPBarPosition(CellNumber, CellsCount);
    end;
  finally
    if not ColumnInProcess then
      CellNumber := CellNumber + RowsCount;
    SetPBarPosition(CellNumber, CellsCount);
  end;
  RemoveEmptyRequests;
  FreeStringList(ComparisonList);
end;

function TSheetCollector.GetWritebackSingleCellData(
  SingleCell: TSheetSingleCellInterface; var RequestElement: IXMLDOMNode;
  EraseEmptyCells: boolean): boolean;
var
  RowElement, DataElement, SchemaElement: IXMLDOMNode;
  CellValue, tmpStr, Style, FullName, PkId: string;
  SingleCellRange: ExcelRange;
  AliasIndex, i: integer;
  tmpBool: boolean;
begin
  result := false;
  SingleCellRange := SingleCell.GetExcelRange;
  if not Assigned(SingleCellRange) then
    exit;
  // пропускаем незаполненные ячейки
  if not GetCellValue(ExcelSheet, SingleCellRange.Row, SingleCellRange.Column,
    SingleCell.EmptyValueSymbol, CellValue, tmpStr, Style, EraseEmptyCells) then
    exit;
  // проверяем на корректность числового значения
  if Style <> snDataFreeErased then
    if not CheckCellValue(CellValue, SingleCellRange.Row, SingleCellRange.Column) then
      exit;

  SchemaElement := CreateAndAddChild(RequestElement, 'Schema');
  FullName := 'null';
  if Assigned(SingleCell.Cube) then
    FullName := SingleCell.Cube.FullName;

  DataElement := CreateAndAddChild(RequestElement, 'Data');
  RowElement := RequestElement.ownerDocument.createNode(1, 'Row', '');
  AliasIndex := 0;
  for i := 0 to SingleCell.Filters.Count - 1 do
  begin
    CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
      ['name', 'c' + IntToSTr(AliasIndex), 'type', 'member', 'fullname',
      SingleCell.Filters[i].FullName]);
    PkID := GetMemberAttrByKey(SingleCell.Filters[i].Members, 'checked', 'true', attrPKID, tmpBool);
    SetAttr(RowElement, 'c' + IntToStr(AliasIndex), PkID);
    inc(AliasIndex);
  end;
  CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
    ['name', 'c' + IntToSTr(AliasIndex), 'type', 'total', 'totalName',
    SingleCell.MeasureName, 'fullname', FullName]);

  CellValue := SingleCell.GetMultipliedValue(CellValue);
  AddWritebackTotalData(SingleCell, CellValue, RowElement, Style, 'c' + IntToSTr(AliasIndex));
  DataElement.appendChild(RowElement);
  result := true;
end;

function TSheetCollector.GetWritebackData(TaskId: widestring; EraseEmptyCells, ProcessCube: boolean): IXmlDomDocument2;
var
  i: integer;
  RequestElement, RowsRootElement, ColumnsRootElement: IXMLDOMNode;
begin
  result := InitXmlDocument;
  result.DocumentElement := result.createElement('Requests');
  result.DocumentElement.setAttribute('taskID', TaskId);
  result.DocumentElement.setAttribute('ProcessCube', BoolToStr(ProcessCube));

  {Вспомогательные документы для кэширования}
  InitDocument(RowsInfoDom, '', RowsRootElement);
  InitDocument(ColumnsInfoDom, '', ColumnsRootElement);
  SetLength(RowsInfo, FLastRow - FFirstRow + 1);
  SetLength(ColumnsInfo, FLastColumn - FFirstColumn + 1);

  {В процедуре AddAxisData "листовость" элемента определяется отсутсвием
    дочерних узлов. Возникали ситуации, когда исключенные (Influence = 3)
    элементы засчитывались за дочерние (например FMQ 8278).
    Для предотвращения таких случаев проведем предварительныю подготовку.}
  for i := 0 to Rows.Count - 1 do
  begin
    CutAllInvisible(Rows[i].Members, false);
    Rows[i].RemoveAndRenameDataMembers(Rows[i].Members);
  end;
  for i := 0 to Columns.Count - 1 do
  begin
    CutAllInvisible(Columns[i].Members, false);
    Columns[i].RemoveAndRenameDataMembers(Columns[i].Members);
  end;

  // собираем показатели
  ReadMarkupDocuments;
  if IsMarkupNew then
    GetWritebackTotalsDataNew(result.documentElement, EraseEmptyCells)
  else
    GetWritebackTotalsData(result.documentElement, EraseEmptyCells);
  // собираем отдельные ячейки
  for i := 0 to SingleCells.Count - 1 do
  begin
    if not (SingleCells[i].TotalType = wtResult) then
      continue;
    RequestElement := CreateWithAttributes(result, 'Request', ['id', SingleCells[i].UniqueID]);
    if GetWritebackSingleCellData(SingleCells[i], RequestElement, EraseEmptyCells) then
      result.DocumentElement.appendChild(RequestElement);
  end;
  KillDomDocument(RowsInfoDom);
  KillDomDocument(ColumnsInfoDom);
  SetLength(RowsInfo, 0);
  SetLength(ColumnsInfo, 0);
end;

procedure TSheetCollector.InitDocuments;
begin
  InitDocument(FFreeTotalsData, 'get_free_totals_data', FDataElement);
  InitDocument(FFreeTotalsDataIgnored, 'get_free_totals_data_ignored', FIgnoredDataElement);
  InitDocument(FFormulaArrays, 'get_formula_arrays', FFormulaArrayDataElement);
end;

function TSheetCollector.GetCachedAxisData(AxisType: TAxisType; Coord: integer;
  var DataNode: IXMLDOMNode; IsWriteback: boolean): boolean;
var
  AxisInfo: TAxisInfo;
  AxisInfoDom: IXMLDOMDocument2;
  CellIndex: integer;
  CacheData: IXMLDOMNode;
begin
  if AxisType = axRow then
  begin
    AxisInfoDom := RowsInfoDom;
    AxisInfo := RowsInfo;
    CellIndex := Coord - FFirstRow;
  end
  else
  begin
    AxisInfoDom := ColumnsInfoDom;
    AxisInfo := ColumnsInfo;
    CellIndex := Coord - FFirstColumn;
  end;

  CacheData := AxisInfo[CellIndex].Data;
  if not Assigned(CacheData) then
  begin
    CacheData := AxisInfoDom.createNode(1, 'row', '');
    //SetAttr(CacheData, mnIsRowLeaf, true);
    if IsWriteback then
      result := GetWritebackAxisData(AxisType, Coord, CacheData)
    else
      result := GetAxisData(AxisType, Coord, CacheData);
    if result then
    begin
      AxisInfoDom.documentElement.appendChild(CacheData);
      AxisInfo[CellIndex].Data := CacheData;
    end;
  end
  else
    result := true;

  if result then
    if IsWriteback then
      CopyCachedData(CacheData, DataNode)
    else
      CopyAttrs(CacheData, DataNode);
end;

function TSheetCollector.CacheTotalInfo(ColumnIndex: integer): boolean;
begin
  with ColumnsInfo[ColumnIndex - FFirstColumn] do
  begin
    if Stored then
    begin
      result := Assigned(TotalRange);
      exit;
    end
    else
      result := false;

    Stored := true;
    { Сведения о нужных показателях собраны заранее}
    if not WritablesInfo.IsColumnWritable(ColumnIndex - FFirstColumn) then
      exit;
    { Итоги не собираем вовсе}
    if IsSummaryColumn(ColumnIndex) then
      exit;

  { Проверка показателя}
    Total := Totals.FindByColumn(ColumnIndex, SectionIndex);
    if not Assigned(Total) then
      exit;
    if not (Total.TotalType in [wtFree, wtResult]) then
      exit;
    TotalRange := Total.GetTotalRangeWithoutGrandSummaryByColumn(ColumnIndex);
    result := Assigned(TotalRange);
  end;
end;

procedure TSheetCollector.GetCollectableTotalsData;
var
  DataNode, CurrentRowNode, PreviousDataNode: IXMLDOMNode;
  RowIndex, ColumnIndex, RowsCount: integer;
  PreviousXml, CurrentXml: string;
begin
  //GetOffsets;
  InitDocuments;

  {В итоговом документе значения будут сгрупированы по строкам,
    поэтому и внешний цикл делаем по ним же.}
  RowsCount := FLastRow - FFirstRow + 1;
  for RowIndex := FFirstRow to FLastRow do
  begin
    SetPBarPosition(RowIndex - FFirstRow + 1, RowsCount);
    if IsSummaryRow(RowIndex) then
      continue;

    {Создадим элемент-шаблон с данными текущей строки.}
    CurrentRowNode := FFreeTotalsData.createNode(1, 'row', '');
    if not GetCachedAxisData(axRow, RowIndex, CurrentRowNode, false) then
      continue;

    PreviousDataNode := nil;
    PreviousXml := '';
    for ColumnIndex := FFirstColumn to FLastColumn do
    begin
      if not CacheTotalInfo(ColumnIndex) then
        continue;
      {На основе шаблона создаем реальный элемент и заносим в него данные
        текущего столбца}
      DataNode := CurrentRowNode.cloneNode(false);
      if not GetCachedAxisData(axColumn, ColumnIndex, DataNode, false) then
        continue;

      {Если описание осевых координат различается, нужно создать новый узел,
        иначе же дополнить предыдущий}
      CurrentXml := DataNode.xml;
      if CurrentXml <> PreviousXml then
      begin
        if not AddTotalData(RowIndex, ColumnIndex, DataNode) then
          continue;
        {Заполненный узел цепляем к документу}
        FDataElement.appendChild(DataNode);
        PreviousDataNode := DataNode;
        PreviousXml := CurrentXml;
      end
      else
        AddTotalData(RowIndex, ColumnIndex, PreviousDataNode)
    end;
  end;

end;

procedure TSheetCollector.GetCollectableTotalsDataNew;
var
  CellsCount, RowsCount, ColumnsCount, i, j, k, Counter, RInd, CInd,
  ExtraColumnsCount, TotalsCount: integer;
  RowsNL, ColumnsNL, TotalsNL: IXMLDOMNodeList;
  CurrentRowNode, DataNode, InfoNode: IXMLDOMNode;
  TotalsShift: array of integer;
  Ok: boolean;
begin
  InitDocuments;

  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
    exit;

  RowsNL := RowsMarkup.selectNodes(Format('markup/row[@%s="true"]', [attrWbWorthy]));
  RowsCount := RowsNL.length;

  ColumnsNL := ColumnsMarkup.selectNodes(Format('markup/column[@%s="true"]', [attrWbWorthy]));
  if Columns.Empty then
  begin
    ColumnsCount := 0;
    ExtraColumnsCount := Totals.Count;
  end
  else
  begin
    ColumnsCount := ColumnsNL.length;
    ExtraColumnsCount := Totals.CountWithPlacement(true);
  end;

  TotalsNL := ColumnsMarkup.selectNodes('markup/totals/total');
  TotalsCount := TotalsNL.length;
  if TotalsCount > 0 then
  begin
    SetLength(TotalsShift, TotalsCount);
    for i := 0 to TotalsCount - 1 do
      TotalsShift[i] := GetIntAttr(TotalsNL[i], attrShift, -BeastNumber);
  end
  else
    if ExtraColumnsCount = 0 then
      exit;


  CellsCount := RowsCount * (ColumnsCount + ExtraColumnsCount);
  Counter := 0;

  {В итоговом документе значения будут сгрупированы по строкам,
    поэтому и внешний цикл делаем по ним же.}
  for i := 0 to RowsCount - 1 do
  begin
    {Создадим элемент-шаблон с данными текущей строки.}
    CurrentRowNode := FFreeTotalsData.createNode(1, 'row', '');
    InfoNode := RowsNL[i].selectSingleNode('collect');
    if Assigned(InfoNode) then
      CopyAttrs(InfoNode, CurrentRowNode);
    RInd := FFirstRow + GetIntAttr(RowsNL[i], attrShift, -BeastNumber);
    SetAttr(CurrentRowNode, mnIsRowLeaf, 'true');
    SetAttr(CurrentRowNode, mnIsColumnLeaf, 'true');

    for j := 0 to ColumnsCount - 1 do
    begin
      CInd := FFirstColumn + GetIntAttr(ColumnsNL[j], attrShift, -BeastNumber);
      {На основе шаблона создаем реальный элемент и заносим в него данные
        текущего столбца}
      DataNode := CurrentRowNode.cloneNode(false);
      InfoNode := ColumnsNL[j].selectSingleNode('collect');
      if Assigned(InfoNode) then
        CopyAttrs(InfoNode, DataNode);

      {Пишем данные показателей}
      Ok := false;
      for k := 0 to TotalsCount - 1 do
      begin
        if not CacheTotalInfo(CInd + TotalsShift[k]) then
          continue;
        Ok := AddTotalData(RInd, CInd + TotalsShift[k], DataNode) or Ok;
      end;
      {если что-то добавили, то цепляем узел к документу}
      if Ok then
        FDataElement.appendChild(DataNode);

      inc(Counter);
      SetPBarPosition(Counter, CellsCount);
    end;

    {обработка показателей за осью столбцов}
    if ExtraColumnsCount > 0 then
    begin
      DataNode := CurrentRowNode.cloneNode(false);
      Ok := false;
      for j := 0 to ExtraColumnsCount - 1 do
      begin
        CInd := FLastColumn - ExtraColumnsCount + j + 1;
        if not CacheTotalInfo(CInd) then
          continue;
        Ok := AddTotalData(RInd, CInd, DataNode) or Ok;
        inc(Counter);
        SetPBarPosition(Counter, CellsCount);
      end;
      {если что-то добавили, то цепляем узел к документу}
      if Ok then
        FDataElement.appendChild(DataNode);
    end;

  end;
end;

procedure TSheetCollector.GetOneTotalData(Total: TSheetTotalInterface);
var
  CurrentRowNode, InfoNode, DataNode: IXMLDOMNode;
  TotalShift, i, j, RowsCount, ColumnsCount, CellsCount: integer;
  RelativeTotalCInd, RelativeCInd, RInd, CInd, Counter: integer;
  ColumnsList: TStringList;
  RowsNL{, ColumnsNL}: IXMLDOMNodeList;
begin
  InitDocuments;
  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
    exit;

  {Определим смещение показателя в секции}
  TotalShift := Total.Shift;

  {Выделим строки с нужным показателем. Список содержит индексы столбцов
    относительно начала оси.}
  ColumnsList := TStringList.Create;
  for i := 0 to TotalSections.Count - 1 do
    if Pos(Total.UniqueID + '_', TotalSections[i]) = 1 then
      ColumnsList.Add(IntToStr(i));
  ColumnsCount := ColumnsList.Count;

  RowsNL := RowsMarkup.selectNodes(Format('markup/row[@%s="true"]', [attrWbWorthy]));
  RowsCount := RowsNL.length;
  CellsCount := RowsCount * ColumnsCount;
  Counter := 0;

 // if not Total.IsIgnoredColumnAxis then
   // ColumnsNL := ColumnsMarkup.selectNodes(Format('markup/column[@%s="true"]', [attrWbWorthy]));

  for i := 0 to RowsCount - 1 do
  begin
    {Создадим элемент-шаблон с данными текущей строки.}
    CurrentRowNode := FFreeTotalsData.createNode(1, 'row', '');
    InfoNode := RowsNL[i].selectSingleNode('collect');
    if Assigned(InfoNode) then
      CopyAttrs(InfoNode, CurrentRowNode);
    RInd := FFirstRow + GetIntAttr(RowsNL[i], attrShift, -BeastNumber);
    SetAttr(CurrentRowNode, mnIsRowLeaf, 'true');
    SetAttr(CurrentRowNode, mnIsColumnLeaf, 'true');

    for j := 0 to ColumnsCount - 1 do
    begin
      RelativeTotalCInd := StrToInt(ColumnsList[j]);
      RelativeCInd := RelativeTotalCInd - TotalShift;
      CInd := FFirstColumn + RelativeTotalCInd;
      {На основе шаблона создаем реальный элемент и заносим в него данные
        текущего столбца}
      DataNode := CurrentRowNode.cloneNode(false);
      if not Total.IsIgnoredColumnAxis then
      begin
        InfoNode := ColumnsMarkup.selectSingleNode(
          Format('markup/column[@%s="%d"]/collect', [attrShift, RelativeCInd])); //ColumnsNL[RelativeCInd].selectSingleNode('collect');
        if Assigned(InfoNode) then
          CopyAttrs(InfoNode, DataNode);
      end;

      {Пишем данные показателя}
      if not CacheTotalInfo(CInd) then
        continue;
      if AddTotalData(RInd, CInd, DataNode) then
        FDataElement.appendChild(DataNode);

      inc(Counter);
      SetPBarPosition(Counter, CellsCount);
    end;
  end;
end;

procedure TSheetCollector.GetCollectableCellsData;
var
  i, SectionIndex: integer;
  CellRange: ExcelRange;
  Formula: string;
  RowElement: IXmlDomNode;
  Total: TSheetTotalInterface;
begin
  InitDocument(FSingleCellsData, 'get_single_cells_data', FDataElement);
  //GetOffsets;
  for i := 0 to SingleCells.Count -1 do
  begin
    if not (SingleCells[i].TotalType in FTotalTypes) then
      continue;
    CellRange := SingleCells[i].GetExcelRange;
    if not Assigned(CellRange) then
      continue;
    RowElement := CreateAndAddWithAttributes(FDataElement, 'row', [attrAlias, SingleCells[i].Alias]);
    if SingleCells[i].PlacedInTotals then
    begin
      SetAttr(RowElement, 'inTable', true);
      Total := Totals.FindByColumn(CellRange.Column, SectionIndex);
      if not Assigned(Total) then
        continue;
      SetAttr(RowElement, 'totalalias', Total.Alias);
      if IsMarkupNew then
        GetCellMarkupData(SingleCells[i], RowElement)
      else
      begin
        GetCachedAxisData(axRow, CellRange.Row, RowElement, false);
        GetCachedAxisData(axColumn, CellRange.Column, Rowelement, false);
      end;
    end;
    {!! А может здесь формулу-то все же кодировать надо?}
    if not GetCellFormula(ExcelSheet, CellRange.Row, CellRange.Column, Formula) then
      continue;
    SetAttr(RowElement, 'formula', Formula);
  end;
end;

procedure TSheetCollector.AddSchemaMetadata(Total: TSheetTotalInterface;
  var RequestElement: IXMLDOMNode);
var
  i, AliasIndex: integer;
  SchemaElement: IXMLDOMNode;
begin
  SchemaElement := CreateAndAddChild(RequestElement, 'Schema');
  AliasIndex := 0;
  for i := 0 to Filters.Count - 1 do
  begin
    if not Filters[i].IsAffectsTotal(Total) then
      continue;
    CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
      ['name', 'c' + IntToSTr(AliasIndex), 'type', 'member', 'fullname',
      Filters[i].FullName{, 'hierarchyId', Filters[i].HierarchyId}]);
    inc(AliasIndex);
  end;
  if not Total.IsIgnoredColumnAxis then
    for i := 0 to Columns.Count - 1 do
    begin
      CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
        ['name', 'c' + IntToSTr(AliasIndex), 'type', 'member', 'fullname',
        Columns[i].FullName{, 'hierarchyId', Columns[i].HierarchyId}]);
      inc(AliasIndex);
    end;
  for i := 0 to Rows.Count - 1 do
  begin
    CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
      ['name', 'c' + IntToSTr(AliasIndex), 'type', 'member', 'fullname',
      Rows[i].FullName{, 'hierarchyId', Rows[i].HierarchyId}]);
    inc(AliasIndex);
  end;
  CreateAndAddWithAttributes(SchemaElement, 'AttributeType',
    ['name', 'c' + IntToSTr(AliasIndex), 'type', 'total', 'totalName',
    Total.MeasureName, 'fullname', Total.FullName]);
end;

function TSheetCollector.GetAxisData(AxisType: TAxisType; Coord: integer;
  var AxisData: IXMLDOMNode): boolean;

  procedure WriteAllMembers(AxisCollection: TSheetAxisCollectionInterface; StartIndex: integer);
  var
    i: integer;
  begin
    for i := StartIndex to AxisCollection.Count - 1 do
      SetAttr(AxisData, AxisCollection[i].Alias, AxisCollection[i].AllMember);
  end;

var
  AxisCollection: TSheetAxisCollectionInterface;
  RangeName, UniqueName: string;
  i, CurAxisElementIndex, CurLevelIndex: integer;
  IsLeaf, IsleafMember: boolean;
  Params: TStringList;
begin
  result := true;
  AxisCollection := GetAxis(AxisType);
  if AxisType = axRow then
    RangeName := GetNameByRC(ExcelSheet, Coord, FRowsLeaf)
  else
    RangeName := GetNameByRC(ExcelSheet, FColumnsLeaf, Coord);
  if (RangeName = '') then
  begin
    SetAttr(AxisData, mnIsColumnLeaf, 'true');
    exit;
  end;

  if not ParseExcelName(RangeName, Params) then
  begin
    result := false;
    exit;
  end;

  {Ссылка на общий итог - прописываем в координатах все Allmember-ы}
  if (Params[0] = gsRow) or (Params[0] = gsColumn) then
  begin
    WriteAllMembers(AxisCollection, 0);
    SetAttr(AxisData, IIF(AxisCollection.AxisType = axRow, mnIsRowLeaf, mnIsColumnLeaf), 'false');
    {Для задачи 8311}
    SetAttr(AxisData, IIF(AxisCollection.AxisType = axRow, gsRow, gsColumn), 'true');
    exit;
  end;

  try
    CurAxisElementIndex := 0;
    CurLevelIndex := -1;
    IsLeaf := (Params.IndexOf(fpEnd) = -1);

    {Цепочка локал айди начинается с 4 параметра. Перебираем их и получаем
      осевые координаты - набор юникнеймов элементов оси.}
    for i := 3 to Params.Count - 1 do
    begin
      // из за гребучего контекстного меню надо проверять
      if CurAxisElementIndex > AxisCollection.Count - 1 then
        exit;
      // пропускаем фиктивный параметр
      if (Params[i] = fpLeafEnd) or (Params[i] = fpDummy) then
        continue;
      inc(CurLevelIndex);

      {если итог, заполняем остальные алиасы элементов осей общими (All) мемберами}
      if (Params[i] = fpEnd) then
      begin
        if (CurLevelIndex > 0) then
          inc(CurAxisElementIndex);
        WriteAllMembers(AxisCollection, CurAxisElementIndex);
        break;
      end;

      {По локал айди получим нужный нам юникнейм}
      UniqueName := GetMemberAttrByKey(AxisCollection[CurAxisElementIndex].Members,
        attrLocalId, Params[i], attrUniqueName, IsleafMember);
      // если UniqueName неопределено - выходим
      result := (UniqueName <> '');
      if not result then
        exit;
      SetAttr(AxisData, AxisCollection[CurAxisElementIndex].Alias, UniqueName);

      {Если игнорирует иерархию или следующий - пустышка, то переходим на следующий элемент оси}
      if (AxisCollection[CurAxisElementIndex].IgnoreHierarchy and not AxisCollection.Broken) or
         ((i <> Params.Count - 1) and (Params[i + 1] = fpDummy)) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
        continue;
      end;

      {переходим на следующий элемент строк по достижению последнего уровня в текущем}
      if (CurLevelIndex = AxisCollection[CurAxisElementIndex].Levels.Count - 1) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
      end;
    end;

    SetAttr(AxisData,IIF(AxisType = axRow, mnIsRowLeaf, mnIsColumnLeaf), BoolToStr(IsLeaf));
  finally
    FreeStringList(Params);
  end;
end;

function TSheetCollector.AddTotalData(RowIndex,
  ColumnIndex: integer; var DataNode: IXMLDOMNode): boolean;
var
  Total: TSheetTotalInterface;
  IsValueCorrect, NoFormula: boolean;
  CellValue, CellFormula, CellStyle, TypeFormula: string;
  FormulaNode, ComplexFormulaNode: IXMLDOMNode;
begin
  result := false;
  if (RowIndex <= 0) or (ColumnIndex <= 0) then
    exit;
  {Если в данной ячейке отдельный показатель, то выходим}
  if WritablesInfo.IsSingleCellSelected(ExcelSheet, RowIndex, ColumnIndex) then
    exit;

  Total := ColumnsInfo[ColumnIndex - FFirstColumn].Total;
  IsValueCorrect := GetCellValue(ExcelSheet, RowIndex, ColumnIndex,
    Total.EmptyValueSymbol, CellValue, CellFormula, CellStyle, false);
  NoFormula := not ((CellFormula <> '') and (Copy(CellFormula, 1, 1) = '='));

  {Если формула в ячейке совпадает с символом пустого значения показателя,
    то собирать ее не надо}
  if CellFormula = Total.EmptyValueSymbol then
    exit;

  {если в этой секции показателя сложная формула (FormulaArray или TypeFormula) - выходим
   проверяем FormulaArray}
  with ColumnsInfo[ColumnIndex - FFirstColumn] do
  begin
    {Если сложная формула в этой секции уже была обработана, то выходим}
    (*if (IsArrayFormulaProcessed(Total.Alias, SectionIndex)) and (RowIndex <> FLastRow) then*)
    if FormulaArrayProcessed then
      exit;

    if (RowIndex = FFirstCollectedRow) then //???
      if CheckFormulaArray(TotalRange, CellFormula) then
      begin
        ComplexFormulaNode := CreateAndAddChild(FFormulaArrayDataElement, 'complexFormula');
        GetCachedAxisData(axColumn, ColumnIndex, ComplexFormulaNode, false);
        SetAttr(ComplexFormulaNode, 'totalAlias', Total.Alias);
        SetAttr(ComplexFormulaNode, 'sectionIndex', SectionIndex);
        FormulaNode := CreateAndAddChild(ComplexFormulaNode, 'value');
        AddFormula(CellFormula, FormulaNode, RowIndex);
        FormulaArrayProcessed := true;
        exit;
      end;
  end;


  {провереям TypeFormula}
  if Total.TypeFormula.Enabled then
  begin
    TypeFormula := TypeFormulaToString(Total, RowIndex, ColumnsInfo[ColumnIndex - FFirstColumn].SectionIndex, FGrandSummaryRow);
    if (CellFormula = TypeFormula) or ((Total.TotalType = wtResult) and NoFormula)
      (*or ((CellFormula <> TypeFormula) and (Pos(fmIncorrectRef, TypeFormula) > 0))*) then
    begin
      CreateAndAddChild(DataNode, Total.Alias + '_formula');
      result := true;
      exit;
    end;
  end;

  {С результатов собираем только формулы}
  if (Total.TotalType = wtResult) and NoFormula then
    exit;

  {если некорректное значение и нет формулы - выходим}
  if NoFormula and not IsValueCorrect then
    exit;

  if IsValueCorrect then
  try
    CellValue := Total.GetMultipliedValue(CellValue);
  except
  end;

  if not NoFormula then
  begin
    CellValue := '';
    FormulaNode := CreateAndAddChild(DataNode, Total.Alias + '_formula');
    AddFormula(CellFormula, FormulaNode, RowIndex);
  end;
  SetAttr(DataNode, Total.Alias, CellValue);
  result := true;
end;

procedure TSheetCollector.AddFormula(Formula: string;
  var FormulaNode: IXMLDOMNode; CurrentRow: integer);
var
  Template, CellRef, ReplacedCellRef, ParamName, Offset: string;
  FormulaCellRefs: TStringList;
  i, ColumnNumber, RowNumber, SectionIndex: integer;
  Total: TSheetTotalInterface;
  ParamNode: IXMLDOMNode;
begin
  Template := Formula;
  {Получаем список всех ячеек, на которые ссылается формула и кодируем их}
  FormulaCellRefs := GetFormulaCellRefs(Formula);
  for i := 0 to FormulaCellRefs.Count - 1 do
  begin
    CellRef := FormulaCellRefs[i];
    if CellRef[1] = '!' then
      continue;

    {Больше 30(?) параметров в формуле быть не может}
    ReplacedCellRef := '';
    if (i < 10) then
      ParamName := 'param0' + IntToStr(i)
    else
      ParamName := 'param' + IntToStr(i);

    ReplacedCellRef := StringReplace(CellRef, '|', '', [rfReplaceAll]);

    ColumnNumber := GetColumnIndex(Copy(CellRef, 1, Pos('|', CellRef) - 1));
    RowNumber := StrToInt(Copy(CellRef, Pos('|', CellRef) + 1, Length(CellRef)));
    if (ColumnNumber < FFirstColumn) or (ColumnNumber > FLastColumn) or
       (RowNumber < FFirstRow) or (RowNumber > FLastRow) then
      continue;

    if RowNumber = FGrandSummaryRow then
      Offset := gsRow
    else
      Offset := IntToStr(RowNumber - CurrentRow);

    Total := Totals.FindByColumn(ColumnNumber, SectionIndex);
    if not Assigned(Total) then
    begin
      Template := ReplaceCellRef(Template, ReplacedCellRef, fmIncorrectRef);
      continue;
    end;

    ParamNode := CreateAndAddChild(FormulaNode, ParamName);
    FillNodeAttributes(ParamNode, [attrOffset, Offset, 'total', Total.UniqueID]);
    if (not Total.IsIgnoredColumnAxis) and (not Columns.Empty) then
      if IsMarkupNew then
        AddAxisDataToFormulaParam(axColumn, ColumnNumber - FFirstColumn, ParamNode)
      else
        if not GetCachedAxisData(axColumn, ColumnNumber, ParamNode, false) then
          continue;
    if not Rows.Empty then
      if IsMarkupNew then
        AddAxisDataToFormulaParam(axRow, RowNumber - FFirstRow, ParamNode)
      else
        if not GetCachedAxisData(axRow, RowNumber, ParamNode, false) then
          continue;
    Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
  end;
  SetAttr(FormulaNode, attrTemplate, Template);
end;

function TSheetCollector.TypeFormulaToString(FormulaTotal: TSheetTotalInterface;
  CurrentRow, CurrentSectionIndex, GrandSummaryRow: integer): string;

  function NotAssignedTotal(Total: TSheetBasicTotal; ParamName: string;
    var Template: string): boolean;
  begin
    result := not Assigned(Total);
    if result then
      Template := StringReplace(Template, ParamName, fmIncorrectRef, [rfReplaceAll]);
  end;

  function NotAssignedTargetRange(Range: ExcelRange; ParamName: string;
    var Template: string): boolean;
  begin
    result := not Assigned(Range);
    if result then
      Template := StringReplace(Template, ParamName, fmIncorrectRef, [rfReplaceAll]);
  end;

  function GetColumnByCoords(FormulaParam: TFormulaParam; ParamTotal: TSheetTotalInterface): integer;
  var
    i: integer;
    AttrName, AttrValue, XPath: string;
    MarkupNode: IXMLDOMNode;
    Shift: integer;
  begin
    result := -1;
    XPath := '';

    if ParamTotal.IsIgnoredColumnAxis then
      exit;

    for i := 0 to FormulaParam.Coords.Count - 1 do
    begin
      AddTail(XPath, ' and ');
      AttrName := FormulaParam.Coords.Names[i];
      AttrValue := FormulaParam.Coords.Values[AttrName];
      EncodeXPathString(AttrValue);
      XPath := XPath + Format('(@%s="%s")', [AttrName, AttrValue]);
    end;
    if XPath = '' then
      exit;

    XPath := Format('markup/column[collect[%s]]', [XPath]);
    MarkupNode := ColumnsMarkup.selectSingleNode(XPath);
    if not Assigned(MarkupNode) then
      exit;

    // Получили смещение начала нужной секции показателей
    // Теперь надо добавить смещение нужного показателя внутри нее - и искомый столбец найден
    Shift := GetIntAttr(MarkupNode, attrShift, -BeastNumber);
    result := FFirstColumn + Shift + ParamTotal.Shift;
  end;

var
  Template, ParamName, DecodedParam: string;
  FormulaParam: TFormulaParam;
  i, SectionIndex, ColumnIndex: integer;
  Offset, ColumnName, RowName, tmpStr: string;
  ParamTotal: TSheetBasicTotal;
  TargetRange: ExcelRange;
  TypeFormula: TTypeFormula;
  //SectionsOnly: boolean;
begin
  result := '';
  TypeFormula := FormulaTotal.TypeFormula;
  if not Assigned(TypeFormula) then
    exit;
  Template := TypeFormula.Template;
  //SectionsOnly := false;
  // проходим по всем закодированным параметрам формулы
  for i := 0 to TypeFormula.FormulaParams.Count - 1 do
  begin
    FormulaParam := TypeFormula.FormulaParams[i];
    ParamName := FormulaParam.Name;
    Offset := FormulaParam.OffSet;
    if (Offset <> '') and (OffSet[1] = '!') then
      Delete(Offset, 1, 1);

    if FormulaParam.ParamType = fptFreeCell then
      DecodedParam := FormulaParam.ParamValue
    else
    begin
      case FormulaParam.ParamType of
        fptTotal, fptTotalAbsolute:
          begin
            ParamTotal := Totals.FindByAlias(FormulaParam.ParamValue);
            if NotAssignedTotal(ParamTotal, ParamName, Template) then
              continue;

            SectionIndex := -1;
            ColumnIndex := -1;
            {В случае абсолютной ссылки Offset содержит еще и номер секции}
            if FormulaParam.ParamType = fptTotalAbsolute then
            begin
              tmpStr := Offset;
              Offset := CutPart(tmpStr, '_');
              SectionIndex := StrToInt(tmpStr);
            end
            else
            begin
              if FormulaTotal.IsIgnoredColumnAxis then
              {В версии 2.3.6 в рамках задачи 14964 сделана модернизация типовых формул для показателей
              за осью столбцов, ссылающихся на показатели в таблице. Параметры такой формулы хранят
              двойную привязку к конкретной секции показателя (по мембер-координатам и по номеру).}
              begin
                SectionIndex := 0; // за столбцами у любого показателя секция только одна!
                {Сперва пытаемся использовать привязку по координатам}
               // if not SectionsOnly then
               ColumnIndex := GetColumnByCoords(FormulaParam, TSheetTotalInterface(ParamTotal))
               (* Пока две привязки вместе дают глюки, работать будет только одна, по координатам
                else
                  ColumnIndex := -1;
                {Если не получилось, пробуем тупо по номеру секции}
                if ColumnIndex = -1 then
                begin
                  SectionIndex := FormulaParam.Section;
                  SectionsOnly := true;
                end;*)
              end
              else
                {Старая реализация (до версии 2.3.6) Идея в том, что показатель-параметр в формуле находится
                в той же секции, что и показатель-владелец типовой формулы. Ссылки за пределы своей секции
                недействительны.}
                SectionIndex := IIF((ParamTotal as TSheetTotalInterface).IsIgnoredColumnAxis, 0, CurrentSectionIndex);
            end;

            if ColumnIndex <> -1 then
              TargetRange := (ParamTotal as TSheetTotalInterface).GetTotalRangeWithoutGrandSummaryByColumn(ColumnIndex)
            else
              TargetRange := (ParamTotal as TSheetTotalInterface).GetTotalRangeWithoutGrandSummary(SectionIndex);

            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;

            if FormulaParam.IsOtherSheet  then
              RowName := IntToStr(StrToInt(Offset) + (CurrentRow - FFirstRow))
            else
              if Offset = gsRow then
              begin
                RowName := IntToStr(GrandSummaryRow)
              end
              else
                if FormulaParam.ParamType = fptTotal then
                  RowName := IntToStr(StrToInt(Offset) + CurrentRow)
                else
                  RowName := '$' + IntToStr(StrToInt(Offset) + FFirstRow);
          end;
        fptSingleCell:
          begin
            ParamTotal := SingleCells.FindByAlias(FormulaParam.ParamValue);
            if NotAssignedTotal(ParamTotal, ParamName, Template) then
              continue;
            TargetRange := (ParamTotal as TSheetSingleCellInterface).GetExcelRange;
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            RowName := IntToStr(TargetRange.Row)
          end;
        fptRowMP:
          begin
            TargetRange := GetRangeByName(ExcelSheet, FormulaParam.ParamValue);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            if Offset = gsRow then
            begin
                RowName := IntToStr(FGrandSummaryRow)
            end
            else
              RowName := IntToStr(StrToInt(Offset) + CurrentRow);
          end;
        fptColumnMP:
          begin
            TargetRange := GetRangeByName(ExcelSheet, FormulaParam.ParamValue);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            RowName := IntToStr(TargetRange.Row);
            TargetRange := (TypeFormula.Owner as TSheetTotalInterface).GetTotalRange(CurrentSectionIndex);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            ColumnName := GetColumnName(StrToInt(Offset) + TargetRange.Column);
          end;
      end;

      if FormulaParam.ParamType <> fptColumnMP then
      begin
        ColumnName := GetColumnName(TargetRange.Column);
        if FormulaParam.ParamType = fptTotalAbsolute then
          ColumnName := '$' + ColumnName;
      end;

      DecodedParam := ColumnName + RowName;
    end;
    // заменяем параметр на раскодированный
    Template := StringReplace(Template, ParamName, DecodedParam, [rfReplaceAll]);
  end;
  result := Template;
end;

procedure TSheetCollector.ParseCellRef(CellRef: string;
  out ReplacedCellRef, Column, Row: string;
  out ColumnNumber, RowNumber: integer;
  out IsOtherSheet, IsAbsolute: boolean);
begin
  ReplacedCellRef := '';
  IsOtherSheet := (CellRef[1] = '!');
  if IsOtherSheet then
    Delete(CellRef, 1, 1);
  ReplacedCellRef := StringReplace(CellRef, '|', '', [rfReplaceAll]);
  IsAbsolute := false;
  while Pos('$', CellRef) > 0 do
  begin
    IsAbsolute := true;
    Delete(CellRef, Pos('$', CellRef), 1);
  end;
  Column := copy(CellRef, 1, Pos('|', CellRef) - 1);
  ColumnNumber := GetColumnIndex(Column);
  if (not IsOtherSheet) and
    (ColumnNumber = FGrandSummaryColumn) then
    Column := gsColumn;
  Row := copy(CellRef, Pos('|', CellRef) + 1, Length(CellRef));
  RowNumber := StrToInt(Row);
end;

procedure TSheetCollector.AddTypeFormula(FormulaTotal: TSheetTotalInterface; Formula: string;
  var FormulaNode: IXMLDOMNode; CurrentRow, CurrentColumn: integer);

  {}
  function ReplaceRangeNames(var Template: string): boolean;
  var
    i, FirstIndex, LastIndex: integer;
    CellName, RefName, CellRef: string;
  begin
    result := false;
    {пользовательское имя диапазона отдельного показателя имеет вид
      Криста_отдельный_тип_номер, см. TSheetSingleCell.GetUserExcelName}
    repeat
      FirstIndex := Pos('Криста_Отдельн', Template);
      if FirstIndex = 0 then
        exit;
      LastIndex := FirstIndex + 15;
      repeat
        inc(LastIndex);
      until (Template[LastIndex] in ['0'..'9']);
      repeat
        inc(LastIndex);
        if LastIndex > Length(Template) then
        begin
          break;
        end;
      until not (Template[LastIndex] in ['0'..'9']);
          dec(LastIndex);
      RefName := Copy(Template, FirstIndex, LastIndex - FirstIndex + 1);

      for i := 0 to SingleCells.Count - 1 do
      begin
        CellName := SingleCells[i].GetUserExcelName;
        if CellName = RefName then
        begin
          CellRef := SingleCells[i].Address;
          Delete(Template, FirstIndex, LastIndex - FirstIndex + 1);
          Insert(CellRef, Template, FirstIndex);
          result := true;
          break;
        end;
      end;
    until false;
  end;

  procedure SetParamAttributes(Node: IXMLDOMNode; ParamType: TFormulaParamType;
    ParamValue, Offset: string; IsOtherSheet: boolean);
  begin
    SetAttr(Node, attrParamType, Ord(ParamType));
    SetAttr(Node, attrParamValue, ParamValue);
    if Offset <> '' then
      SetAttr(Node, attrOffset, Offset);
    SetAttr(Node, attrIsOtherSheet, BoolToStr(IsOtherSheet));
  end;

  procedure SetParamCoords(ParamNode: IXMLDOMNode; SectionIndex: integer);
  var
    ColumnNode, Node: IXMLDOMNode;
    RelativeCInd: integer;
  begin
    SetAttr(ParamNode, attrSection, SectionIndex);
    RelativeCInd := SectionIndex * Totals.CountWithPlacement(false);
    ColumnNode := ColumnsMarkup.selectSingleNode(
      Format('markup/column[@%s="%d"]/collect', [attrShift, RelativeCInd]));

    Node := ParamNode.ownerDocument.createNode(1, 'coord', '');
    ParamNode.appendChild(Node);

    if not Assigned(ColumnNode) then
      exit;
    CopyAttrs(ColumnNode, Node);
  end;

  function CheckMPReference(AxisType: TAxisType;
    RowNumber, ColumnNumber: integer; out MPName: string): boolean;
  var
    MPRange: ExcelRange;
  begin
    result := false;
    MPRange := GetRangeByName(ExcelSheet,
      BuildExcelName(IIF(AxisType = axRow, sntRowsMPArea, sntColumnsMPArea)));
    if Assigned(MPRange) then
      if IsPointInRange(MPRange, RowNumber, ColumnNumber) then
      begin
        if AxisType = axRow then
          MPRange := GetRange(ExcelSheet, MPRange.Row, ColumnNumber,
            MPRange.Row + MPRange.Rows.Count - 1, ColumnNumber)
        else
          MPRange := GetRange(ExcelSheet, RowNumber, MPRange.Column,
            RowNumber, MPRange.Column + MPRange.Columns.Count - 1);
        MPName := GetNameByRange(ExcelSheet, MPRange);
        result := MPName <> '';
      end;
  end;

var
  Template, ReplacedCellRef, Column, Row, ParamName, SingleCellID, Offset, MPName: string;
  FormulaCellRefs: TStringList;
  i, RowNumber, ColumnNumber, SectionIndex: integer;
  IsOtherSheet, IsAbsolute: boolean;
  ParamNode: IXMLDOMNode;
  Total: TSheetTotalInterface;
begin
  Template := Formula;
  if ReplaceRangeNames(Template) then
  begin
    Formula := Template;
    SetCellFormula(ExcelSheet, CurrentRow, CurrentColumn, Formula);
  end;
  FormulaCellRefs := GetFormulaCellRefs(Formula);

  {Все ссылки в формуле пытаемся перевести в параметры.
    Допустимы ссылки на ячейки табличных и отдельных показателей.
    В 2.2.7 добавляем ссылки на мембер пропертиз.}
  for i := 0 to FormulaCellRefs.Count - 1 do
  begin
    ParseCellRef(FormulaCellRefs[i], ReplacedCellRef,
      Column, Row, ColumnNumber, RowNumber, IsOtherSheet, IsAbsolute);
    ParamName := IIF((i < 10), 'param0', 'param') + IntToStr(i);
    ParamNode := CreateAndAddChild(FormulaNode, ParamName);

    {Обработка ссылки на отдельный показатель}
    if WritablesInfo.IsSingleCellSelected(ExcelSheet,
      RowNumber, ColumnNumber, SingleCellID) then
    begin
      SetParamAttributes(ParamNode, fptSingleCell, 'S_' + SingleCellID, '', IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;

    Offset := Row;
    if not IsOtherSheet then
      if StrToInt(Row) = FGrandSummaryRow then
        Offset := gsRow
      else
        {В случае абсолютной ссылки на ячейку показателя смещение берем
          относительно первой ячейки, а не текущей. Чтобы ссылка действительно
          вела себя как абсолютная и не смещалась по столбцам надо указывать
          секцию показателя. Будем запоминать ее здесь же, в Offset.}
        if IsAbsolute then
          Offset := IntToStr(StrToInt(Row) - FFirstRow)
        else
          Offset := IntToStr(StrToInt(Row) - CurrentRow);

    {обработка ссылки на МР}
    if CheckMPReference(axRow, RowNumber, ColumnNumber, MPName) then
    begin
      SetParamAttributes(ParamNode, fptRowMP, MPName, Offset, IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;
    if CheckMPReference(axColumn, RowNumber, ColumnNumber, MPName) then
    begin
      Offset := IntToStr(ColumnNumber - CurrentColumn);
      SetParamAttributes(ParamNode, fptColumnMP, MPName, Offset, IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;

    {Ссылка на ячейку вне таблицы}
    if (StrToInt(Row) < FFirstRow) or (StrToInt(Row) > FLastRow) or
      (ColumnNumber < FFirstColumn) or (ColumnNumber > FLastColumn) then
    begin
      SetParamAttributes(ParamNode, fptFreeCell, ReplacedCellRef, '', IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
    end
    else
    begin
      {Обработка ссылки на табличный показатель}
      Total := Totals.FindByColumn(ColumnNumber, SectionIndex);
      { если показатель удалили - заменяем его на #ССЫЛКА! = fmIncorrectRef}
      if not Assigned(Total) then
      begin
        Template := ReplaceCellRef(Template, ReplacedCellRef, fmIncorrectRef);
        continue;
      end;
      if IsAbsolute then
      begin
        Offset := Offset + '_' + IntToStr(SectionIndex);
        SetParamAttributes(ParamNode, fptTotalAbsolute, Total.Alias, Offset, IsOtherSheet);
      end
      else
      begin
        SetParamAttributes(ParamNode, fptTotal, Total.Alias, Offset, IsOtherSheet);
        //здесь должна быть обработка типовой для показателя за столбцами
        if FormulaTotal.IsIgnoredColumnAxis then
          SetParamCoords(ParamNode, SectionIndex);
      end;
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
    end;

  end;
  SetAttr(FormulaNode, attrTemplate, Template);
end;

function TSheetCollector.GetTypeFormula(Total: TSheetTotalInterface; Row,
  Column: integer): TTypeFormula;
var
  DOM: IXMLDOMDocument2;
  FormulaNode: IXMLDOMNode;
  CellFormula: string;
begin
  result := nil;
  if not Assigned(Total) then
    exit;
  result := TTypeFormula.Create(Total);
  DOM := InitXmlDocument;
  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
    ReadMarkupDocuments;
  try
    FormulaNode := DOM.CreateNode(1, attrTypeFormula, '');
    if not GetCellFormula(ExcelSheet, Row, Column, CellFormula) then
      exit;
    AddTypeFormula(Total, CellFormula, FormulaNode, Row, Column);
    result.ReadFromXML(FormulaNode);
  finally
    FormulaNode := nil;
    KillDOMDocument(DOM);
  end;
end;

procedure TSheetCollector.MapTypeFormula(Total: TSheetTotalInterface);
var
  Section, Row, Column, GrandSummaryRow: integer;
  TotalRange, CellRange: ExcelRange;
  IsItSummary: boolean;
  CellValue, CellFormula, CellStyle, DecodedTypeFormula: string;
begin
  if not Assigned(Total) then
    exit;
  if not(Total.TypeFormula.Enabled and (Total.TypeFormula.Template <> '')) then
    exit;
  if not Assigned(RowsMarkup) or not Assigned(ColumnsMarkup) then
    ReadMarkupDocuments;

  GrandSummaryRow := GetGrandSummaryRow(ExcelSheet);

  for Section := 0 to Total.SectionCount - 1 do
  begin
    TotalRange := Total.GetTotalRange(Section);
    if not Assigned(TotalRange) then
      continue;
    Column := TotalRange.Column;
    for Row := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
    begin
      {отдельные ячейки не трогаем}
      if WritablesInfo.IsSingleCellSelected(ExcelSheet, Row, Column) then
        continue;
      IsItSummary := IsSummaryCell(Row, Column);
      if not IsItSummary or ((Total.CountMode = mcmTypeFormula) and Total.SummariesByVisible) then
      begin
        CellRange := GetRange(ExcelSheet, Row, Column, Row, Column);
        GetCellValue(ExcelSheet, Row, Column, Total.EmptyValueSymbol,
          CellValue, CellFormula, CellStyle, false);
        {Размещаем типовую формулу в ячейку если:
        1)(Нет значения в ячейке или показатель "результат") и нет формулы в ячейке
        2) Если формула в ячейке равна символу пустого значения показателя
        3) Если это итог}
        if (((CellValue = '') or (Total.TotalType = wtResult)) and
          not IsExistsFormula(CellRange)) or (CellFormula = Total.EmptyValueSymbol) or IsItSummary then
        begin
          DecodedTypeFormula := TypeFormulaToString(Total, Row, Section, GrandSummaryRow);
          SetCellFormula(ExcelSheet, Row, Column, DecodedTypeFormula);
          CellRange.Interior.Pattern := xlSolid;
        end
        else
        begin
          if Total.IsTypeFormulaException(Row, Column) then
          begin
            {В ячейке исключение, помечаем соответствующим образом}
            CellRange.Interior.PatternColorIndex := 32;
            CellRange.Interior.Pattern := xlGray16;
          end
          else
            {Типовые формулы совпадают, значит у ячейки снимаем ярлык иключения}
            CellRange.Interior.Pattern := xlSolid;
        end;
     end;
   end;
  end;


end;

procedure TSheetCollector.CopyCachedData(CacheNode, DataNode: IXMLDOMNode);
var
  FullName, AttrName, AttrValue: string;
  k: integer;
begin
  for k := 0 to CacheNode.attributes.length - 1 do
  begin
    //fullname, кэшированный в GetWritebackAxisData
    FullName := CacheNode.attributes[k].nodeName;
    //сопоставленный ему алиас для обратной записи
    AttrName := ComparisonList.Values[FullName];
    //pk_id
    AttrValue := CacheNode.attributes[k].text;
    (DataNode as IXmlDomElement).setAttribute(AttrName, AttrValue);
  end;
end;

procedure TSheetCollector.BuildComparisonList(RequestNode: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i: integer;
  AliasName, FullName: string;
begin
  if not Assigned(ComparisonList) then
    ComparisonList := TStringList.Create;
  ComparisonList.Clear;
  NL := RequestNode.selectNodes('Schema/AttributeType');
  for i := 0 to NL.length - 1 do
  begin
    AliasName := NL[i].attributes.GetNamedItem('name').text;
    FullName := NL[i].attributes.GetNamedItem('fullname').text;
    if (AliasName <> '') and (FullName <> '') then
      ComparisonList.Add(FullName + '=' + AliasName);
  end;
end;

procedure TSheetCollector.Save;
begin
  inherited;
  if (FLoadMode = [lmInner]) then
    exit;

  {Для режима обновления одного показателя надо сохранять построенные оси,
    чтобы была возможность считать итоги}
  if Assigned(RowsDom) then
    PutData(RowsDom, cpRowsAxis);
  if Assigned(ColumnsDom) then
    PutData(ColumnsDom, cpColumnsAxis);

  if IsMarkupNew then
  begin
    if Assigned(RowsMarkup) then
      PutData(RowsMarkup, cpRowsMarkup);
    if Assigned(ColumnsMarkup) then
      PutData(ColumnsMarkup, cpColumnsMarkup);
    if Assigned(CellsMarkup) then
      PutData(CellsMarkup, cpCellsMarkup);
  end;
end;

procedure TSheetCollector.ReadMarkupDocuments;
begin
  RowsMarkup := GetData(cpRowsMarkup);
  ColumnsMarkup := GetData(cpColumnsMarkup);
  CellsMarkup := GetData(cpCellsMarkup);
end;

procedure TSheetCollector.GetCellMarkupData(Cell: TSheetSingleCellInterface;
  var DataNode: IXMLDOMNode);
var
  InfoNode: IXMLDOMNode;
  XPath: string;
begin
  XPath := Format('markup/cell[@%s="%s"]/collect', [attrAlias, Cell.Alias]);
  InfoNode := CellsMarkup.selectSingleNode(XPath);
  if not Assigned(InfoNode) then
    exit;
  CopyAttrs(InfoNode, DataNode);
end;

procedure TSheetCollector.AddAxisDataToFormulaParam(AxisType: TAxisType;
  Shift: integer; var ParamNode: IXMLDOMNode);
var
  Node: IXMLDOMNode;
  ColumnShift: integer;
begin
  try
    if AxisType = axRow then
      Node := RowsMarkup.documentElement.childNodes[Shift].selectSingleNode('collect')
    else
    begin
      ColumnShift := Shift - (Shift mod Totals.CountWithPlacement(false));
      Node := ColumnsMarkup.selectSingleNode(
        Format('markup/column[@%s="%d"]/collect', [attrShift, ColumnShift]));
    end;
  except
    exit;
  end;
  if Assigned(Node) then
    CopyAttrs(Node, ParamNode);
end;

end.



