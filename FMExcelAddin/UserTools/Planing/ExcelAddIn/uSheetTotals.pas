{
  Показатели листа планирования (объект + коллекция).
  Сейчас объект один для всех типов показателей. Что опять пораждает кейсы.
  В будущем, скорее всего, разнесем наследованием.
}

unit uSheetTotals;

interface
uses
  Classes, SysUtils, MSXML2_TLB, uXMLUtils, uFMExcelAddinConst,
  uFMAddinGeneralUtils, uFMAddinExcelUtils, uSheetObjectModel,
  uXMLCatalog, ExcelXP, PlaningTools_TLB, uSheetHistory, uExcelUtils,
  uGlobalPlaningConst, uFMAddinXMLUtils;


type
  //показатель листа
  TSheetTotal = class(TSheetTotalInterface)
  private
    {Вернет фильтр, который дублируется на этом показателе, что автоматически
     означает ошибку (неправильная конфигурация листа). Здесь неважно какой
     именно экземпляр дублирующей пары вернется (все равно ошибка).
     Кроме того, таких пар может быть несколько. Но нам главное знать есть
     они или нет. Результат нужен для вывода сообщения об ошибке.
     Если все в порядке вернет ничего.}
    function DuplicatedFilter: TSheetFilterInterface;
  protected
    function GetCommentText: string; override;
    function GetExcelName: string; override;
    function GetTitleExcelName: string; override;
    function GetAlias: string; override;
    //возвращает порядковый номер показателя из числа определенного типа размещения
    function GetOrdinal: integer; override;
    //список действующих на показатель фильтров (индексы через запятую)
    function GetFiltersCommaText: string; override;

    function GetIsIgnoredColumnAxis: boolean; override;
    function GetFullName: string; override;
    function GetSectionCount: integer; override;
    procedure SetTotalType(AType: TSheetTotalType); override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    {возвращает смещение от начала секции ("блока")}
    function GetShift: integer; override;
  public
    destructor Destroy; override;

    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;

    //действует ли на показатель данный фильтр
    function IsFilteredBy(Filter: TSheetFilterInterface): boolean; override;

    function GetFullExcelName(Instance: integer): string; override;
    {Имя для пользовательской разметки}
    function GetUserExcelName(Instance: integer): string; override;
    function GetElementCaption: string; override;

    // получение диапазона показателя
    function GetTotalRange(SectionIndex: integer): ExcelRange; override;
    function GetTotalRangeByColumn(Column: integer): ExcelRange; override;
    // получение диапазона показателя без общего итога
    function GetTotalRangeWithoutGrandSummary(SectionIndex: integer): ExcelRange; override;
    function GetTotalRangeWithoutGrandSummaryByColumn(Column: integer): ExcelRange; override;

    // получение столбца показателя по индексу секции
    function GetTotalColumn(SectionIndex: integer): integer; override;

    //проверяет содержимое на соответствие каталогу
    function Validate(out MsgText: string; out ErrorCode: integer): boolean; override;

    {можно ли разложить по измерению}
    function FactorizedBy(AxisElem: TSheetAxisElementInterface): boolean; override;

    {насколько показатель раскладывается по оси}
    function FitInAxis(AxisType: TAxisType): integer; override;
    function FitInAxis(AxisType: TAxisType; out UnfitDimensions: string): integer; override;

    function Refresh(Force: boolean): boolean; override;

    // получить тип элемента листа
    function GetObjectType: TSheetObjectType; override;
    // получить строковое описание типа элемента листа
    function GetObjectTypeStr: string; override;

    function AddTypeFormula(Row, Column: integer): TTypeFormula; override;
    function IsTypeFormulaException(Row, Column: integer): boolean; override;
    procedure ClearTypeFormulaValues; override;

    procedure ApplyStyles; override;
    function GetOnDeleteWarning: string; override;
    procedure SwitchType(out CommentForHistory: string); override;
    function RepairConst: TConstInterface; override;

    function GetMultipliedValue(AValue: extended): extended; overload; override;
    function GetMultipliedValue(AValue: string): string; overload; override;
    function GetDividedValue(AValue: extended): extended; overload; override;
    function GetDividedValue(AValue: string): string; overload; override;

  end;

  //коллекция показателей
  //в DOM-e соответствует ветке '/Totals'
  TSheetTotalCollection = class(TSheetTotalCollectionInterface)
  private
  protected
    function GetItem(Index: integer): TSheetTotalInterface; override;
    procedure SetItem(Index: integer; Value: TSheetTotalInterface); override;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    //добавляет элемент в коллекцию
    function Append: TSheetTotalInterface; override;
    //удаляет элемент из коллекции
    procedure Delete(Index: integer); override;
    //загружает коллекцию из DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает коллекцию в XML
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //возвращает имя коллекции в соответствии с типом ее элементов
    function GetCollectionName: string; override;
    // возвращает показатель-меру по кубу и мере
    function FindTotal(CubeName, MeasureName: string): TSheetTotalInterface; override;
    //возвращает индекс элемента в коллекции
    function FindByID(ID: string): integer; override;
    //возвращает показатель по его алиасу
    function FindByAlias(aStr: string): TSheetTotalInterface; override;
    //возвращает показатель по его заголовку
    function FindByCaption(Caption: string): TSheetTotalInterface; override;
    //проверяет содержимое на соответствие каталогу
    function Validate: boolean; override;
    // проверка на возможностьсовпадения MDX для показателей
    function MayTotalsHaveSameMDX(Index1, Index2: integer): boolean; override;
    {Кол-во показателей определенной области видимости}
    function CountWithPlacement(IgnoredColumns: boolean): integer; override;
    { доступ к показателям определенного типа размещения}
    function GetWithPlacement(IgnoredColumns: boolean;
      ind: integer): TSheetTotalInterface; override;
    function GetWithPlacementInd(IgnoredColumns: boolean;
      ind: integer): integer; override;
    {Обновление - здесь пока фиктивное, просто что бы закрыть абстрактрый метод}
    function Refresh(Force: boolean): boolean; override;
    // возвращает показатель по столбцу
    function FindByColumn(Column: integer; out SectionIndex: integer): TSheetTotalInterface; override;
    // присутствует ли в коллекции показатель заданного типа
    function CheckByType(TotalTypes: TTotalTypes): boolean; override;
    // получить списки индексов показателей заданного типа
    // возвращает false - заданные показатели отсутствуют
    function GetTotalLists(var TotalsList, IgnoredTotalsList: TStringList;
                           FTotalTypes: TTotalTypes): boolean; override;
    // обязательны ли промежуточные итоги
    function AreSummariesImperative: boolean; override;
    {проставляет показателям свойство NumberFormat}
    procedure GetNumberFormats; override;
    {возвращает XPath-условие на вариативность расчета итогов - учитывать ли
      записи данных, не относящиеся к листьям}
    function GetLeafCondition(NeedRow, NeedColumn: boolean): string; override;
    procedure Clear; override;
  end;


implementation

{*********** TSheetTotal implementation ***********}

destructor TSheetTotal.Destroy;

  //удаляем ИД показателя из области действия частных фильтров
  procedure RemoveFromFilters;
  var
    i, ScopeIndex: integer;
    tmpList: TStringList;
  begin
    tmpList := TStringList.Create;
    try
      for i := SheetInterface.Filters.Count - 1 downto 0 do
        if IsFilteredBy(SheetInterface.Filters[i]) and
          SheetInterface.Filters[i].IsPartial then
        begin
          ScopeIndex := SheetInterface.Filters[i].Scope.IndexOf(UniqueId);
          if ScopeIndex > -1 then
            SheetInterface.Filters[i].Scope.Delete(ScopeIndex);
          //если область действия пуста, то можно и сам фильтр грохнуть
          if SheetInterface.Filters[i].Scope.Count = 0 then
          begin
            if not Owner.Updating then
              tmpList.Add('Удален частный фильтр ' +
                SheetInterface.Filters[i].FullDimensionName2);
            SheetInterface.Filters.Delete(i);
          end;
        end;
      if tmpList.Count > 0 then
        SheetInterface.AddEventInSheetHistory(evtEdit, tmpList.CommaText, true);
    finally
      FreeStringList(tmpList);
    end;
  end;

begin
  RemoveFromFilters;
  inherited Destroy;
end;

function TSheetTotal.GetCommentText: string;
const
  delimiter = #10;
var
  i: integer;
begin
  result := '';

  case TotalType of
    wtConst: begin
      result := 'Константа "' + Caption + '"';
      exit;
    end;
    wtFree: result := 'Свободный "' + Caption + '"';
    wtMeasure, wtResult: result := 'Куб "' + CubeName + '"; Мера "' + MeasureName + '"';
  end;

  if SummariesByVisible then
    result := result + #10 + 'Итоги подводятся только по видимым элементам (' +
      MeasureCountModeStr[CountMode] + ').'
  else
    result := result + #10 + 'Итоги подводятся по всем элементам.';

  {у "свободных" и "результатов" выводим информацию о типовой формуле}
  if (TotalType in [wtFree, wtResult]) and TypeFormula.Enabled then
    result := result + #10#10'Включена типовая формула:' + #10 + TypeFormula.UserFormula;

  with SheetInterface do
    for i := 0 to Filters.Count - 1 do
      if IsFilteredBy(Filters[i]) then
      begin
        AddTail(result, delimiter);
        if Filters[i].IsPartial then
        begin
          result := result + #10'Частный фильтр "' + Filters[i].FullDimensionName2 + '"';
          result := result + #10 + Filters[i].CommentText;
        end
        else
        begin
          result := result + #10'Общий фильтр "' + Filters[i].FullDimensionName2 + '"';
          if Filters[i].IsParam then
          begin
            result := result + #10'Параметр "' + Filters[i].Param.Name + '"';
            if Filters[i].Param.IsInherited then
              result := result + ' (от родительской задачи)';
          end;
        end;
      end;
end;

function TSheetTotal.GetExcelName: string;
begin
  case TotalType of
    wtMeasure: result := sntTotalMeasure;
    wtFree: result := sntTotalFree;
    wtResult: result := sntTotalResult;
    wtConst: result := sntTotalConst;
  end;
  result := BuildExcelName(result + snSeparator + UniqueID);
end;

function TSheetTotal.GetTitleExcelName: string;
begin
  case TotalType of
    wtMeasure: result := sntTotalMeasureTitle;
    wtFree: result := sntTotalFreeTitle;
    wtResult: result := sntTotalResultTitle;
    wtConst: result := sntTotalConstTitle;
  end;
  result := BuildExcelName(result + snSeparator + UniqueID);
end;

function TSheetTotalCollection.GetWithPlacementInd(IgnoredColumns: boolean;
  ind: integer): Integer;
var
  i, k: integer;
  x, y: boolean;
begin
  result := -1;
  k := 0;
  for i := 0 to Count - 1 do
  begin
    x := (Items[i].IsIgnoredColumnAxis);
    y := (IgnoredColumns);
{ TODO -oСтрижаков -cхорошо бы :
Монструозное условие равенства булевских значений.
При случае переделать }
    if ((x and y) or (not x and  not y)) then
    begin
      if k = ind then
      begin
        result := i;
        exit;
      end
      else
        inc(k);
    end;
  end;
end;

{Обновление - здесь пока фиктивное, просто что бы закрыть абстрактрый метод}
function TSheetTotalCollection.Refresh(Force: boolean): boolean;
begin
  result := true;
end;


function TSheetTotalCollection.CountWithPlacement(IgnoredColumns: boolean): integer;
var
  i: integer;
  x, y: boolean;
begin
  result := 0;
  for i := 0 to Count - 1 do
  begin
    x := (Items[i].IsIgnoredColumnAxis);
    y := (IgnoredColumns);
    if ((x and y) or (not x and  not y)) then
      inc(result);
  end;
end;

function TSheetTotalCollection.GetWithPlacement(IgnoredColumns: boolean;
  ind: integer): TSheetTotalInterface;
var GlobalInd: integer;
begin
  result := nil;
  GlobalInd := GetWithPlacementInd(IgnoredColumns, ind);
  if GlobalInd >= 0 then
    result := Items[GlobalInd];
end;


function  TSheetTotal.GetAlias: string;
begin
  result := 'T_' + UniqueId;
end;

function TSheetTotal.GetOrdinal: integer;
var
  i: integer;
  Total: TSheetTotalInterface;
begin
  result := -1;
  for i := 0 to Owner.Count - 1 do
  begin
    Total := (Owner as TSheetTotalCollection)[i];
    if (IsIgnoredColumnAxis and Total.IsIgnoredColumnAxis) or
      (not IsIgnoredColumnAxis and not Total.IsIgnoredColumnAxis) then
      inc(result);
    if Self = Total then
      break;
  end;
end;

function TSheetTotal.GetFiltersCommaText: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
    if IsFilteredBy(SheetInterface.Filters[i]) then
    begin
      AddTail(result, ',');
      result := result + IntToStr(i);
    end;
end;

function TSheetTotal.GetFullExcelName(Instance: integer): string;
begin
  result := ExcelName + snSeparator + IntToStr(Instance) +
    snSeparator + IntToStr(Ord(Format));
end;


procedure TSheetTotal.ReadFromXML(Node: IXMLDOMNode);
var
  TmpInt: integer;
begin
  inherited ReadFromXML(Node);
  if not Assigned(Node) then
    exit;
  IsIgnoredColumnAxis := boolean(GetIntAttr(Node, attrIgnoreColumns, 0));
  IsIgnoredRowAxis := boolean(GetIntAttr(Node, attrIgnoreRows, 0));
  IsGrandTotalDataOnly := boolean(GetIntAttr(Node, attrGrandTotalOnly, 0));
  if (TotalType in [wtFree, wtConst]) then
    SummariesByVisible := true
  else
    SummariesByVisible := GetBoolAttr(Node, attrSummariesByVisible, true);
  ColumnWidth := StrToFloat(GetAttr(Node, attrColumnWidth));
  EmptyValueSymbol := GetStrAttr(Node, attrEmptyValueSymbol, '');
  // вынужденная страховка. квест 15597. Каким-то образом значение в хмл оказалось -1
  // в результате аут оф баундз
  TmpInt := GetIntAttr(Node, attrCountMode, 0);
  if TmpInt < 0 then
    TmpInt := 0;
  CountMode := TMeasureCountMode(TmpInt);
  if not SheetInterface.InCopyMode then
    FUserExcelName := GetStrAttr(Node, attrUserExcelName, '');
  IsHidden := GetBoolAttr(Node, attrHidden, false);
end;

procedure TSheetTotal.WriteToXML(Node: IXMLDOMNode);
begin
  inherited WriteToXml(Node);
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrIgnoreColumns, IsIgnoredColumnAxis);
    setAttribute(attrIgnoreRows, IsIgnoredRowAxis);
    setAttribute(attrGrandTotalOnly, IsGrandTotalDataOnly);
    setAttribute(attrColumnWidth, FloatToStr(ColumnWidth));
    setAttribute(attrSummariesByVisible, BoolToStr(SummariesByVisible));
    setAttribute(attrEmptyValueSymbol, EmptyValueSymbol);
    setAttribute(attrCountMode, Ord(CountMode));
    if not SheetInterface.InCopyMode then
      setAttribute(attrUserExcelName, FUserExcelName);
    setAttribute(attrHidden, BoolToStr(IsHidden));
  end;
end;

function TSheetTotal.IsFilteredBy(Filter: TSheetFilterInterface): boolean;
begin
  result := false;
  if (TotalType in [wtMeasure, wtResult]) then
    result := Filter.IsAffectsTotal(Self);
end;

function TSheetTotal.DuplicatedFilter: TSheetFilterInterface;
var
  i: integer;
  Filter: TSheetFilterInterface;
  FilterKey: string; //то по чему ищем дублирование
  KeysList: TStringList;
begin
  result := nil;
  KeysList := TStringList.Create;
  try
    for i := 0 to SheetInterface.Filters.Count - 1 do
    begin
      Filter := SheetInterface.Filters[i];
      if IsFilteredBy(Filter) then
      begin
        FilterKey := Filter.FullDimensionNameMDX;
        if KeysList.IndexOf(FilterKey) >= 0 then
        begin //нашли дубликат
          result := Filter;
          break;
        end
        else
          KeysList.Add(FilterKey);
      end;
    end;
  finally
    FreeStringList(KeysList);
  end;
end;

function TSheetTotal.Validate(out MsgText:string; out ErrorCode: integer): boolean;
var
  AMeasure: TMeasure;
  DFilter: TSheetFilterInterface;
  UnfitDimensions: string;
begin
  result := true;
  MsgText := '';

  if (TotalType in [wtFree, wtConst]) then
    exit; //здесь проверять нечего

  if not Assigned(Cube) then
  begin
    result := false;
    MsgText := ' куб "' + CubeName + '" не существует';
    ErrorCode := ecNoCube;
    exit;
  end;

  AMeasure := Cube.Measures.Find(MeasureName) as TMeasure;
  if not Assigned(AMeasure) then
  begin
    result := false;
    MsgText := ' мера "' + MeasureName + '" отсутствует в кубе "' +
      CubeName + '"';
    ErrorCode := ecNoMeasure;
    exit;
  end;

  DFilter := DuplicatedFilter;
  if Assigned(DFilter) then //нашли дублирующийся фильтр
  begin
    result := false;
    MsgText := ' наложены два фильтра по измерению ' + DFilter.FullDimensionName2;
    ErrorCode := ecDuplicateFilters;
    exit;
  end;

  if (TotalType = wtResult) and not SheetInterface.Rows.Empty then
    if FitInAxis(axRow, UnfitDimensions) <> tfFull then
    begin
      result := false;
      MsgText := ' не раскладывается по измерениям ' + UnfitDimensions;
      ErrorCode := ecUnfitDimensions;
      exit;
    end;
end;

function TSheetTotal.FactorizedBy(AxisElem: TSheetAxisElementInterface): boolean;
begin
  result := false;
  if (TotalType in [wtFree, wtConst]) then
  begin
    result := true;
    exit;
  end;
  if ProviderId <> AxisElem.ProviderId then
    exit;
  if not Assigned(Cube) then
    exit;
  result := Cube.DimAndHierInCube(AxisElem.Dimension, AxisElem.Hierarchy);
end;

function TSheetTotal.FitInAxis(AxisType: TAxisType): integer;
var
  i, Cnt: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  if AxisType = axRow then
    Axis := SheetInterface.Rows
  else
    Axis := SheetInterface.Columns;
  Cnt := 0;
  for i := 0 to Axis.Count - 1 do
    if FactorizedBy(Axis[i]) then
      inc(Cnt);
  if Cnt = 0 then
    result := tfNone
  else
    if Cnt = Axis.Count then
      result := tfFull
    else result := tfPartial;
end;

function TSheetTotal.FitInAxis(AxisType: TAxisType;
  out UnfitDimensions: string): integer;
var
  i, Cnt: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  UnfitDimensions := '';
  if AxisType = axRow then
    Axis := SheetInterface.Rows
  else
    Axis := SheetInterface.Columns;
  Cnt := 0;
  for i := 0 to Axis.Count - 1 do
    if FactorizedBy(Axis[i]) then
      inc(Cnt)
    else
    begin
      AddTail(UnfitDimensions, ', ');
      UnfitDimensions := UnfitDimensions + '"' + Axis[i].FullDimensionName + '"';
    end;
  if Cnt = 0 then
    result := tfNone
  else
    if Cnt = Axis.Count then
      result := tfFull
    else result := tfPartial;
end;

function TSheetTotal.GetIsIgnoredColumnAxis: boolean;
begin
  if (TotalType = wtFree) and SheetInterface.Columns.Empty then
    FIsIgnoredColumnAxis := false;
  result := FIsIgnoredColumnAxis;
end;

function TSheetTotal.Refresh(Force: boolean): boolean;
begin
  result := false;
end;

function TSheetTotal.GetObjectType: TSheetObjectType;
begin
  result := wsoTotal;
end;

function TSheetTotal.GetObjectTypeStr: string;
begin
  result := 'Показатель';
  case TotalType of
    wtMeasure: result := 'Мера из куба';
    wtFree: result := 'Свободный показатель';
    wtResult: result := 'Результат расчета';
    wtConst: result := 'Константа';
  end;
end;

function TSheetTotal.GetTotalRange(SectionIndex: integer): ExcelRange;
var
  ExcelName: string;
begin
  result := nil;
  if IsIgnoredColumnAxis and (SectionIndex <> 0) then
    exit;
  ExcelName := GetFullExcelName(SectionIndex);
  Result := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
end;

function TSheetTotal.GetTotalRangeWithoutGrandSummary(
  SectionIndex: integer): ExcelRange;
begin
  result := GetTotalRange(SectionIndex);
  if not SheetInterface.Rows.GrandSummaryOptions.Enabled then
    exit;
  if result = nil then
    exit;
  if GetRangeByName(SheetInterface.ExcelSheet, snNamePrefix + snSeparator + gsRow) = nil then
    exit;
  {случай, когда весь диапазон показателей состоит из одной строчки общего итога}
  if result.Rows.Count = 1 then
  begin
    result := nil;
    exit;
  end;
  result := GetRange(SheetInterface.ExcelSheet, result.Row, result.Column,
                     result.Row + result.Rows.count - 2, result.Column);
end;

function TSheetTotalCollection.GetItem(Index: integer): TSheetTotalInterface;
begin
  result := Get(Index);
end;

procedure TSheetTotalCollection.SetItem(Index: integer; Value: TSheetTotalInterface);
begin
  Put(Index, Value);
end;

function TSheetTotalCollection.Append: TSheetTotalInterface;
begin
  result := TSheetTotal.Create(Self);
  inherited Add(result);
  result.PermitEditing := PermitEditing;
end;

procedure TSheetTotalCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TSheetTotalCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Total: TSheetTotalInterface;
begin
  inherited;
  if not Assigned(Node) then
    exit;

  StyleByLevels := GetBoolAttr(Node, attrStyleByLevels, false);
  FormatByRows := GetBoolAttr(Node, attrFormatByRows, true);

  NL := Node.selectNodes(xpTotal);
  for i := 0 to NL.length - 1 do
  begin
    Total := Append;
    Total.ReadFromXML(NL[i]);
  end;

  FTotalCounters.ReadFromXml(Node);
end;

procedure TSheetTotalCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  inherited;

  (Node as IXMLDOMElement).setAttribute(attrStyleByLevels, BoolToStr(StyleByLevels));
  (Node as IXMLDOMElement).setAttribute(attrFormatByRows, BoolToStr(FormatByRows));

  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'total', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;

  FTotalCounters.WriteToXml(Node);
end;

function TSheetTotalCollection.GetCollectionName: string;
begin
  result := 'totals';
end;

function TSheetTotalCollection.FindTotal(CubeName, MeasureName: string): TSheetTotalInterface;
var
  i: integer;
begin
  result := nil;
  if (CubeName = '') or (MeasureName = '') then
    exit;
  for i := 0 to Count - 1 do
    if (Items[i].TotalType in [wtMeasure, wtResult]) then
      if (Items[i].CubeName = CubeName) and
        (Items[i].MeasureName = MeasureName) then
        begin
          result := Items[i];
          break;
        end
end;

function TSheetTotalCollection.FindByID(ID: string): integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Count - 1 do
    if Items[i].UniqueID = ID then
    begin
      result := i;
      break;
    end;
end;

function TSheetTotalCollection.FindByAlias(aStr: string): TSheetTotalInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Alias = aStr then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetTotalCollection.FindByCaption(Caption: string): TSheetTotalInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].Caption = Caption) then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetTotalCollection.Validate: boolean;
var
  i, ErrorCode: integer;
  DetailText: string;
begin
  result := true;
  if Empty then
    exit;
  Owner.OpenOperation(pfoTotalsValidate, not CriticalNode, not NoteTime, otProcess);
  for i := 0 to Count - 1 do
  if not Items[i].Validate(DetailText, ErrorCode) then
    begin
      result := false;
      Owner.PostMessage('- показатель "' + Items[i].CubeName + '.' +
          Items[i].MeasureName + '": ' + DetailText + ';', msgWarning);
    end;
  Owner.CloseOperation;
end;

function TSheetTotalCollection.MayTotalsHaveSameMDX(Index1, Index2: integer): boolean;
begin
  result := false;
  //оба показателя должны быть из базы (не свободными)...
  if not ((Items[Index1].TotalType in [wtMeasure, wtResult]) and (Items[Index2].TotalType in [wtMeasure, wtResult])) then
    exit;
  //...из одного и того же куба...
  if Items[Index1].CubeName <> Items[Index2].CubeName then
    exit;
  //...и если на них дейтвуют частные фильтры, то они (фильтры) должны совпадать
  if Items[Index1].FiltersCommaText <> Items[Index2].FiltersCommaText then
    exit;

  result := true;
end;

function TSheetTotal.GetFullName: string;
var
  Cube: TCube;
begin
  result := 'null';
  Cube := SheetInterface.XMLCatalog.Cubes.Find(CubeName, ProviderId);
  if (Cube = nil) then
    exit;
  result := Cube.FullName;
end;

function TSheetTotalCollection.FindByColumn(Column: integer; out SectionIndex: integer): TSheetTotalInterface;
var
  AllTotals: ExcelRange;
  tmpStr, Id: string;
  Index: integer;
begin
  result := nil;
  AllTotals := GetRangeByName(Owner.ExcelSheet, BuildExcelName(sntTotals));
  if not Assigned(AllTotals) then
    exit;
  try
    tmpStr := Owner.TotalSections[Column - AllTotals.column];
  except
    exit;
  end;
  // Некорректная ситуация
  if tmpStr = '' then
    exit;
  Id := CutPart(tmpStr, '_');
  Index := FindById(Id);
  if Index > -1 then
    result := Items[Index];
  SectionIndex := StrToInt(tmpStr);
end;

function TSheetTotalCollection.CheckByType(TotalTypes: TTotalTypes): boolean;
var
  i: integer;
begin
  result := false;
  for i := 0 to Count - 1 do
    if (Items[i].TotalType in TotalTypes) then
    begin
      result := true;
      exit;
    end;
end;

function TSheetTotalCollection.GetTotalLists(var TotalsList, IgnoredTotalsList: TStringList;
                                             FTotalTypes: TTotalTypes): boolean;
var
  i: integer;
begin
  TotalsList := TStringList.Create;
  IgnoredTotalsList := TStringList.Create;
  for i := 0 to Count - 1 do
  begin
    if not (Items[i].TotalType in FTotalTypes) then
      continue;
    if (Items[i].IsIgnoredColumnAxis) then
      IgnoredTotalsList.Add(IntToStr(i))
    else TotalsList.Add(IntToStr(i));
  end;
  result := (TotalsList.Count <> 0) or (IgnoredTotalsList.Count <> 0);
  if not result then
  begin
    FreeStringList(TotalsList);
    FreeStringList(IgnoredTotalsList);
  end;  
end;

function TSheetTotal.GetSectionCount: integer;
var
  TotalRange: ExcelRange;
begin
  result := 0;
  TotalRange := GetTotalRange(result);
  while TotalRange <> nil do
  begin
    inc(result);
    TotalRange := GetTotalRange(result);
  end;
end;

function TSheetTotal.GetTotalColumn(SectionIndex: integer): integer;
var
  TotalRange: ExcelRange;
begin
  result := 0;
  TotalRange := GetTotalRange(SectionIndex);
  if TotalRange <> nil then
   result := TotalRange.Column;
end;

function TSheetTotalCollection.AreSummariesImperative: boolean;
var
  i: integer;
begin
  {если хотя бы у одного показателя итоги берутся из базы,
    то промежуточные итоги необходимы даже элементам с одним потомком}
  result := false;
  if Empty then
    exit;
  for i := 0 to Count - 1 do
    if not Items[i].SummariesByVisible then
      begin
        result := true;
        exit;
      end;
end;

procedure TSheetTotalCollection.GetNumberFormats;
var
  i, x ,y: integer;
  RangeName: string;
  Range: ExcelRange;
  ESheet: ExcelWorkSheet;
begin
  ESheet := Owner.ExcelSheet;
  for i := 0 to Count - 1 do
  with Items[i] do
    if (Format = fmtStandard) then
    begin //берем формат пользователя
      //ориентируемся на первое вхождение показателя
      RangeName := GetFullExcelName(0);
      Range := GetRangeByName(ESheet, RangeName);
      if Assigned(Range) then
      try
        if not VarIsNull(Range.NumberFormat) then //формат один для всей колонки
          NumberFormat := Range.NumberFormat
        else //разные форматы в ячейках; берем по первой
        begin
          x := Range.Row;
          y := Range.Column;
          Range := ESheet.Range[ESheet.Cells.Item[x, y], ESheet.Cells.Item[x, y]];
          if not VarIsNull(Range.NumberFormat) then
            NumberFormat := Range.NumberFormat;
          {формулы в текстовом формате имеют мало смысла... ставим общий}
        end;
        if Items[i].TypeFormula.Enabled and (NumberFormat = '@') then
          NumberFormat := '';
      except
      end;
    end
    else //генерим маску формата сами
      NumberFormat := BuildFormatMask;
end;

function TSheetTotal.GetTotalRangeByColumn(Column: integer): ExcelRange;
var
  TotalsRange: ExcelRange;
begin
  result := nil;
  TotalsRange := GetRangeByName(SheetInterface.ExcelSheet, BuildExcelName(sntTotals));
  if not Assigned(TotalsRange) then
    exit;
  result := GetIntersection(SheetInterface.ExcelSheet, TotalsRange,
    GetRange(SheetInterface.ExcelSheet, 1, Column, SheetInterface.ExcelSheet.rows.count, Column));
end;

function TSheetTotal.GetTotalRangeWithoutGrandSummaryByColumn(Column: integer): ExcelRange;
begin
  result := GetTotalRangeByColumn(Column);
  if not SheetInterface.Rows.GrandSummaryOptions.Enabled then
    exit;
  if not Assigned(result) then
    exit;
  if GetRangeByName(SheetInterface.ExcelSheet, snNamePrefix + snSeparator + gsRow) = nil then
    exit;
  {случай, когда весь диапазон показателей состоит из одной строчки общего итога}
  if result.Rows.Count = 1 then
  begin
    result := nil;
    exit;
  end;
  result := GetRange(SheetInterface.ExcelSheet, result.Row, result.Column,
    result.Row + result.Rows.count - 2, result.Column);
end;

procedure TSheetTotal.SetTotalType(AType: TSheetTotalType);
begin
  FTotalType := AType;
  SetDefaultStyles;
end;

function TSheetTotal.GetElementCaption: string;
begin
  result := Caption;
end;

procedure TSheetTotal.ApplyStyles;
var
  ERange: ExcelRange;
begin
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if Assigned(ERange) then
  begin
    ERange.Style := 'Normal';
    ERange.Style := IIF(SheetInterface.PrintableStyle,
      Styles.Name[esValuePrint], Styles.Name[esValue]);
  end;
  ERange := GetRangeByName(SheetInterface.ExcelSheet, TitleExcelName);
  if Assigned(ERange) then
  begin
    ERange.Style := 'Normal';
    ERange.Style := IIF(SheetInterface.PrintableStyle,
      Styles.Name[esTitlePrint], Styles.Name[esTitle]);
  end;
end;

function TSheetTotal.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue, esValueprint:
      result := IIF(TotalType in [wtMeasure, wtConst], snData, snDataFree);
    esTitle:
      case TotalType of
        wtMeasure: result := snTotalMeasureTitle;
        wtFree: result := snTotalFreeTitle;
        wtResult: result := snTotalResultTitle;
        wtConst: result := snTotalConstTitle;
      end;
    esTitlePrint: result := snTotalTitlePrint;
  end;
end;

function TSheetTotalCollection.GetLeafCondition(NeedRow, NeedColumn: boolean): string;
var
  i: integer;
  ByVisible, FromBase: string;
begin
  result := '';
  ByVisible := '';
  FromBase := '';
  for i := 0 to Count - 1 do
    if Items[i].SummariesByVisible then
    begin
      AddTail(ByVisible, ' or ');
      ByVisible := ByVisible + Format('(@%s)', [Items[i].Alias]);
    end
    else
    begin
      AddTail(FromBase, ' or ');
      FromBase := FromBase + Format('(@%s)', [Items[i].Alias]);
    end;
  if pos(' or ', ByVisible) > 0 then
    ByVisible := TupleBrackets(ByVisible);
  if pos(' or ', FromBase) > 0 then
    FromBase := TupleBrackets(FromBase);

  if ByVisible <> '' then
      result := '(' + ByVisible +
        IIF(NeedRow, ' and (@IsRowLeaf="true")', '') +
        IIF(NeedColumn, ' and (@IsColumnLeaf="true")', '') + ')';

  if FromBase <> '' then
  begin
    AddTail(result, ' or ');
    result := TupleBrackets(result + FromBase);
  end;
end;

function TSheetTotalCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue: result := 'Данные';
    esValuePrint: result := 'Данные (печать)';
    esTitle: result := 'Заголовок';
    esTitlePrint: result := 'Заголовок (печать)';
  end;
end;

function TSheetTotalCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

procedure TSheetTotalCollection.Clear;
begin
  inherited;
  FStyleByLevels := false;
end;

function TSheetTotal.AddTypeFormula(Row, Column: integer): TTypeFormula;
var
  FormulaEnabled: boolean;
begin
  FormulaEnabled := TypeFormula.Enabled;
  TypeFormula := SheetInterface.GetTypeFormula(Self, Row, Column);
  TypeFormula.Enabled := FormulaEnabled;
  result := TypeFormula;
end;

function TSheetTotal.IsTypeFormulaException(Row, Column: integer): boolean;
var
  SampleTypeFormula: TTypeFormula;
begin
  result := false;
  if not TypeFormula.Enabled then
    exit;

  {итог является исключением если в нем не типовая формула}
  if SheetInterface.IsSummaryCell(Row, Column) then
  begin
    if not(CountMode in [mcmTypeFormula]) then
      result := true;
    exit;
  end;
  {в области показателя может быть отдельная ячейка}
  if SheetInterface.WritablesInfo.IsSingleCellSelected(SheetInterface.ExcelSheet, Row, Column) then
  begin
    result := true;
    exit;
  end;

  SampleTypeFormula := SheetInterface.GetTypeFormula(Self, Row, Column);
  result := not TypeFormula.IsEqual(SampleTypeFormula, true);
end;

function TSheetTotal.GetUserExcelName(Instance: integer): string;
begin
  if FUserExcelName = '' then
  begin
    FUserExcelName := TSheetTotalCollectionInterface(Owner).GetTotalCounterValue(TotalType);
    case TotalType of
      wtFree: FUserExcelName := 'Свободный_' + FUserExcelName;
      wtMeasure: FUserExcelName := 'Мера_' + FUserExcelName;
      wtResult: FUserExcelName := 'Результат_' + FUserExcelName;
      wtConst: FUserExcelName := 'Константа_' + FUserExcelName;
    end;
  end;
  result := snUserNamePrefix + snSeparator + FUserExcelName + snSeparator + IntToStr(Instance);
end;

function TSheetTotal.GetOnDeleteWarning: string;
var
  i: integer;
  Cell: TSheetSingleCellInterface;
  Total: TSheetTotalInterface;
  Part1, Part2: string;
begin
  result := '';

  case TotalType of
    wtFree: result := SysUtils.Format('%s"%s"?'#13'тип: %s',
      [qumDelTotal, Caption, 'Свободный']);
    wtConst: result := SysUtils.Format('%s"%s"?'#13'тип: %s',
      [qumDelTotal, Caption, 'Константа']);
    wtMeasure: result := SysUtils.Format('%s"%s"?'#13'тип: %s, куб: "%s", мера: "%s"',
      [qumDelTotal, Caption, 'Мера', CubeName, MeasureName]);
    wtResult: result := SysUtils.Format('%s"%s"?'#13'тип: %s, куб: "%s", мера: "%s"',
      [qumDelTotal, Caption, 'Результат', CubeName, MeasureName]);
  end;

  {ссылающиеся типовые}
  Part1 := '';
  for i := 0 to Owner.Count - 1 do
  begin
    Total := Owner[i];
    if Total = Self then
      continue;
    if Total.TypeFormula.Enabled and Total.TypeFormula.ContainAlias(Alias) then
    begin
      AddTail(Part1, ', ');
      Part1 := Part1 + '"' + Total.GetElementCaption + '"';
    end;
  end;

  { Размещенные отдельные}
  Part2 := '';
  for i := 0 to SheetInterface.SingleCells.Count - 1 do
  begin
    Cell := SheetInterface.SingleCells[i];
    if Cell.GetUnderlayingTotal = Self then
    begin
      AddTail(Part2, ', ');
      Part2 := Part2 + '"' + Cell.GetElementCaption + '"';
    end;
  end;

  if Part1 + Part2 = '' then
    exit;

  if (Part1 <> '') and (Part2 <> '') then
  begin
    result := result + #10#10'ВНИМАНИЕ! При удалении данного показателя также будут удалены следующие отдельные показатели: ' + #10;
    result := result + Part2 + '.'#10#10;
    result := result + 'Типовые формулы следующих показателей станут некорректны: ' + #10;
    result := result+ Part1 + '.';
    exit;
  end;

  if Part1 <> '' then
  begin
    result := result + #10#10'ВНИМАНИЕ! При удалении данного показателя типовые формулы следующих показателей станут некорректны:' + #10;
    result := result + Part1 + '.';
  end
  else
  begin
    result := result + #10#10'ВНИМАНИЕ! При удалении данного показателя также будут удалены следующие отдельные показатели: ' + #10;
    result := result + Part2 + '.';
  end;
end;

procedure TSheetTotal.ClearTypeFormulaValues;
var
  Section, Column, Row: integer;
  TotalRange: ExcelRange;
begin
  if not Assigned(TypeFormula) then
    exit;
  if not TypeFormula.Enabled then
    exit;
  for Section := 0 to SectionCount - 1 do
  begin
    TotalRange := GetTotalRange(Section);
    if not Assigned(TotalRange) then
      continue;
    Column := TotalRange.Column;
    for Row := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
      if not IsTypeFormulaException(Row, Column) then
        GetRange(SheetInterface.ExcelSheet, Row, Column).ClearContents;
  end;
end;

procedure TSheetTotal.SwitchType(out CommentForHistory: string);
begin
  if TotalType = wtResult then
  begin
    TotalType := wtMeasure;
    TypeFormula.Clear;
    CommentForHistory := SysUtils.Format('Тип показателя "%s.%s" изменен с результата на меру.',
      [CubeName, GetElementCaption]);
  end
  else
  begin
    TotalType := wtResult;
    CommentForHistory := SysUtils.Format('Тип показателя "%s.%s" изменен с меры на результат.',
      [CubeName, GetElementCaption]);
  end;
  FUserExcelName := '';
  CommentForHistory := ConvertStringToCommaText(CommentForHistory);
end;

function TSheetTotal.RepairConst: TConstInterface;
begin
  result := nil;
  if TotalType <> wtConst then
    exit;
  result := SheetInterface.Consts.Append;
  with result do
  begin
    Name := Self.Caption;
    //Value := Self.Value;
    IsSheetConst := true;
    IsInherited := false;
    Id := -1;
  end;
end;

function TSheetTotal.GetDividedValue(AValue: extended): extended;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case SheetInterface.TotalMultiplier of
      tmE3: result := result / 1E3;
      tmE6: result := result / 1E6;
      tmE9: result := result / 1E9
    end;
end;

function TSheetTotal.GetDividedValue(AValue: string): string;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case SheetInterface.TotalMultiplier of
      tmE3: result := FloatToStr(StrToFloat(result) / 1E3);
      tmE6: result := FloatToStr(StrToFloat(result) / 1E6);
      tmE9: result := FloatToStr(StrToFloat(result) / 1E9);
    end;
end;

function TSheetTotal.GetMultipliedValue(AValue: extended): extended;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case SheetInterface.TotalMultiplier of
      tmE3: result := result * 1E3;
      tmE6: result := result * 1E6;
      tmE9: result := result * 1E9;
    end;
end;

function TSheetTotal.GetMultipliedValue(AValue: string): string;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case SheetInterface.TotalMultiplier of
      tmE3: result := FloatToStr(StrToFloat(result) * 1E3);
      tmE6: result := FloatToStr(StrToFloat(result) * 1E6);
      tmE9: result := FloatToStr(StrToFloat(result) * 1E9);
    end;
end;

function TSheetTotal.GetShift: integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    if SheetInterface.Totals[i] = Self then
      exit;
    if (SheetInterface.Totals[i].IsIgnoredColumnAxis and IsIgnoredColumnAxis) or
      (not SheetInterface.Totals[i].IsIgnoredColumnAxis and not IsIgnoredColumnAxis) then
      inc(result);
  end;
end;


end.

