{Модуль содержит описание отдельных ячеек и их коллекции.
Отдельная ячейка представляет собой меру из куба с наложенными на нее
частными фильтрами.}

unit uSheetSingleCells;

interface

uses Classes, SysUtils, uXMLCatalog, uSheetObjectModel, uSheetFilters,
  MSXML2_TLB, uFMAddinExcelUtils, uFMExcelAddinConst, uXMLUtils,
  uFMAddinGeneralUtils, ExcelXP, PlaningTools_TLB, uExcelUtils,
  uGlobalPlaningConst;

type

  TSheetSingleCell = class(TSheetSingleCellInterface)
  private
    FName: string;
  protected
    function GetExcelName: string; override;
    function GetMdxText: string; override;
    function GetWithClause: string;
    function GetSelectClause: string;
    function GetFromClause: string;
    function GetWhereClause: string;
    function GetAlias: string; override;
    function GetCommentText: string; override;
    function GetName: string; override;
    procedure SetName(Value: string); override;
    function GetCommaFilters: string; override;
    function GetAddress: string; override;
    function GetPlacedInTotals: boolean; override;
    procedure SetTotalType(AType: TSheetTotalType); override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    function GetValue: string; override;

    property WithClause: string read GetWithClause;
    property SelectClause: string read GetSelectClause;
    property FromClause: string read GetFromClause;
    property WhereClause: string read GetWhereClause;
  public
    constructor Create(AOwner: TSheetCollection);
    //считывает атрибуты элемента из узла
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает атрибуты элемента в узел
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //проверяет содержимое на соответствие каталогу
    function Validate(out MsgText: string; out ErrorCode: integer): boolean; override;
    function Refresh(Force: boolean): boolean; override;
    // получить тип элемента листа
    function GetObjectType: TSheetObjectType; override;
    // получить строковое описание типа элемента листа
    function GetObjectTypeStr: string; override;
    function GetAddressPoint(out Column, Row: integer): boolean; override;
    function GetExcelRange: ExcelRange; override;
    procedure Suicide(Method: TSuicideMethod; out Msg: string); override;
    function GetElementCaption: string; override;
    procedure ApplyStyles; override;
    function GetUserExcelName: string; override;
    procedure SwitchType(out CommentForHistory: string); override;
    { Если отдельный размещен в области показателей, то возвращает ссылку
      на показатель-"подложку", иначе нил}
    function GetUnderlayingTotal: TSheetTotalInterface; override;
    function GetOnDeleteWarning: string; override;
    function RepairConst: TConstInterface; override;

    function GetMultipliedValue(AValue: extended): extended; overload; override;
    function GetMultipliedValue(AValue: string): string; overload; override;
    function GetDividedValue(AValue: extended): extended; overload; override;
    function GetDividedValue(AValue: string): string; overload; override;

  end;

  TSheetSingleCellCollection = class(TSheetSingleCellCollectionInterface)
  protected
    function GetItem(Index: integer): TSheetSingleCellInterface; override;
    procedure SetItem(Index: integer; Item: TSheetSingleCellInterface); override;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    //добавляет элемент в коллекцию
    function Append: TSheetSingleCellInterface; override;
    //удаляет элемент из коллекции
    procedure Delete(Index: integer); override;
    //загружает коллекцию из DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает коллекцию в XML
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //возвращает имя коллекции в соответствии с типом ее элементов
    function GetCollectionName: string; override;
    //возвращает индекс элемента в коллекции
    function FindByID(ID: string): integer; override;
    function FindByName(AName: string): TSheetSingleCellInterface; override; 
    //проверяет содержимое на соответствие каталогу
    function Validate: boolean; override;
    function Refresh(Force: boolean): boolean; override;
    {проставляет показателям свойство NumberFormat}
    procedure GetNumberFormats; override;
    // присутствует ли в коллекции показатель заданного типа
    function CheckByType(TotalTypes: TTotalTypes): boolean; override;
    // возвращает показатель по его алиасу
    function FindByAlias(aStr: string): TSheetSingleCellInterface; override;
  end;

implementation

{ TSheetSingleCell }

constructor TSheetSingleCell.Create(AOwner: TSheetCollection);
begin
  inherited Create(AOwner);
  {!!
    SheetInterface не является владельцем этой коллекции, но если ее не указать,
    то вызовы методов, содержащих Owner.Owner приведут к AV}
  Filters := TSheetFilterCollection.Create(Self);
end;

function TSheetSingleCell.GetExcelName: string;
begin
  case TotalType of
    wtMeasure: result := sntSingleCellMeasure;
    wtResult: result := sntSingleCellResult;
    wtConst: result := sntSingleCellConst;
  end;
  result := BuildExcelName(result + snSeparator + UniqueID);
end;

procedure TSheetSingleCell.ReadFromXML(Node: IXMLDOMNode);
var
  tmpNode: IXMLDOMNode;
begin
  inherited ReadFromXML(Node);
  if not Assigned(Node) then
    exit;
  TotalMultiplier := TTotalMultiplier(GetIntAttr(Node, attrTotalMultiplier, 0));
  FName := GetStrAttr(Node, attrName, '');
  {загрузка коллекции фильтров}
  tmpNode := Node.selectSingleNode('filters');
  if Assigned(Node) then
    Filters.ReadFromXML(tmpNode);
  if not SheetInterface.InCopyMode then
    FUserExcelName := GetStrAttr(Node, attrUserExcelName, '');
end;

procedure TSheetSingleCell.WriteToXML(Node: IXMLDOMNode);
var
  tmpNode: IXMLDOMNode;
begin
  inherited WriteToXml(Node);
  if not Assigned(Node) then
    exit;
  {запись атрибутов меры}
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrTotalMultiplier, ord(TotalMultiplier));
    setAttribute(attrName, FName);
    if not SheetInterface.InCopyMode then
      setAttribute(attrUserExcelName, FUserExcelName);
  end;
  {запись коллекции фильтров}
  tmpNode := Node.ownerDocument.createNode(1, 'filters', '');
  Node.appendChild(tmpNode);
  Filters.WriteToXML(tmpNode);
end;

function TSheetSingleCell.Refresh(Force: boolean): boolean;
begin
  result := true;
  Filters.Refresh(Force);
end;

function TSheetSingleCell.Validate(out MsgText: string;
  out ErrorCode: integer): boolean;
var
  i: integer;
  NL: IXMLDOMNodeList;
begin
  result := true;
  if (TotalType = wtConst) then
    exit;
  MsgText := '';
  if not Assigned(Cube) then
  begin
    result := false;
    MsgText := ' куб "' + CubeName + '" не существует';
    exit;
  end;
  if not Assigned(Measure) then
  begin
    result := false;
    MsgText := ' мера "' + MeasureName + '" отсутствует в кубе "' +
      CubeName + '"';
    exit;
  end;
  if Filters.Empty then
    exit;
  for i := 0 to Filters.count - 1 do
  begin
    if not CheckDimension(Cube, Filters[i]) then
    begin
      result := false;
      MsgText := ' измерение "' + Filters[i].FullDimensionName2 +
        '" отсутствует в кубе "' + CubeName + '"';
      exit;
    end;
    {проверка на наличие выделенных элементов}
    if result then
    begin
      if not Assigned(Filters[i].Members) then
      begin
        result := false;
        MsgText := 'Не удалось получить элементы измерения "' +
          Filters[i].GetElementCaption + '";';
        exit;
      end;
      NL := Filters[i].Members.selectNodes('//' + ntMember + '[@checked="true"]');
      if Assigned(NL) then
        result := NL.length > 0
      else
        result := false;
      if not result then
      begin
        MsgText := SysUtils.Format('В фильтре "%s" не выбрано ни одного элемента',
          [Filters[i].GetElementCaption]);
        exit;
      end;
    end;
  end;
end;

function TSheetSingleCell.GetMdxText: string;
begin
  result := WithClause + SelectClause + FromClause + WhereClause;
end;

function TSheetSingleCell.GetWithClause: string;
var
  i: integer;
  SlicerName, SlicerValue: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to Filters.Count - 1 do
  begin
    Filter := Filters[i];
    if Filter.IsMultiple then
    begin
      SlicerName := Filter.FullDimensionNameMDX + '.[Slicer] ';
      SlicerValue := TupleBrackets(SetBrackets(Filter.MdxText));
      SlicerValue := StringReplace(SlicerValue, '''', '''' + '''', [rfReplaceAll]);
      result := result + ' MEMBER ' + SlicerName +
        ' AS ' + '''' + 'AGGREGATE' + SlicerValue + '''';
    end;
  end;
  result := result + SysUtils.Format(' MEMBER [MEASURES].[%s] AS ' +
  '''' + '[MEASURES].[%s]' + '''', [Alias, MeasureName]);
  AddHead('WITH ', result);
end;

function TSheetSingleCell.GetSelectClause: string;
begin
  result := SysUtils.Format(' SELECT {[MEASURES].[%s]} on Axis(0)', [Alias]);
end;

function TSheetSingleCell.GetFromClause: string;
begin
  result := ' FROM ' + MemberBrackets(Cube.Name);
end;

function TSheetSingleCell.GetWhereClause: string;
var
  i: integer;
  Slicer: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to Filters.Count - 1 do
  begin
    Filter := Filters[i];
    AddTail(result, ', ');
    Slicer := IIF(Filter.IsMultiple, Filter.FullDimensionNameMDX + '.[Slicer] ',
      Filter.MdxText);
    result := result + Slicer;
  end;
  if result <> '' then
    result := ' WHERE ' + TupleBrackets(result);
end;

function TSheetSingleCell.GetAlias: string;
begin
  result := 'S_' + UniqueId;
end;

function TSheetSingleCell.GetObjectType: TSheetObjectType;
begin
  result := wsoSingleCell;
end;

function TSheetSingleCell.GetObjectTypeStr: string;
begin
  result := 'Отдельный показатель';
  case TotalType of
    wtResult: result := 'Отдельный показатель - результат';
    wtMeasure: result := 'Отдельный показатель - мера';
    wtConst: result := 'Отдельный показатель - мера';
  end;
end;

function TSheetSingleCell.GetCommentText: string;
const
  delimiter = #10;
var
  i: integer;
begin
  case TotalType of
    wtResult: result := 'Результат расчета: "';
    wtMeasure: result := 'Мера из куба: "';
    wtConst: result := 'Константа "';
  end;
  result := result + Name + '"' + #10;
  if (TotalType in [wtResult, wtMeasure]) then
    result := result + 'Куб "' + CubeName + '"; Мера "' + MeasureName + '"';
  {В вордовских комментариях строка адреса не нужна}  
  if Assigned(SheetInterface.ExcelSheet) then
    AddTail(result, #10'Адрес: ' + Address);
  if (Format = fmtCurrency) then
    case TotalMultiplier of
      tmE3: AddTail(result, #10'Выводится в тысячах рублей');
      tmE6: AddTail(result, #10'Выводится в миллионах рублей');
      tmE9: AddTail(result, #10'Выводится в миллиардах рублей');
    end;
  for i := 0 to Filters.Count - 1 do
  begin
    AddTail(result, delimiter);
    result := result + #10'Фильтр "' + Filters[i].FullDimensionName2 + '"';
    result := result + #10 + Filters[i].CommentText;
  end;
end;

function TSheetSingleCell.GetName: string;
begin
  result := FName;
end;

procedure TSheetSingleCell.SetName(Value: string);
begin
  FName := Value;
end;

function TSheetSingleCell.GetCommaFilters: string;
var
  i: integer;
begin
  result := '';
  if Filters.Empty then
    exit;
  for i := 0 to Filters.Count - 1 do
  begin
    AddTail(result, ', ');
    result := result + Filters[i].FullDimensionName2;
  end;
end;

function TSheetSingleCell.GetAddress: string;
var
  ERange: ExcelRange;
begin
  result := '';
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if Assigned(ERange) then
    {$WARNINGS OFF}
    result := ERange.AddressLocal[false, false, xlA1, false, varNull];
    {$WARNINGS ON}
end;

function TSheetSingleCell.GetAddressPoint(out Column,
  Row: integer): boolean;
var
  ERange: ExcelRange;
begin
  result := false;
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if Assigned(ERange) then
  begin
    Column := ERange.Column;
    Row := ERange.Row;
    result := true;
  end;
end;

function TSheetSingleCell.GetPlacedInTotals: boolean;
var
  ERange: ExcelRange;
begin
  result := false;
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if Assigned(ERange) then
    result := IsRangeBelongName(SheetInterface.ExcelSheet, ERange,
      BuildExcelName(sntTotals));
end;

procedure TSheetSingleCell.Suicide(Method: TSuicideMethod; out Msg: string);
var
  eRange: ExcelRange;
  eName, eUserName: ExcelXP.Name;
  WasProtected: boolean;
begin
  Msg := '';
  SheetInterface.WritablesInfo.Delete(ExcelName);
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  eName := GetNameObject(SheetInterface.ExcelSheet, ExcelName);
  eUserName := GetNameObject(SheetInterface.ExcelSheet, GetUserExcelName);
  if (Method = smImmediate) and Assigned(ERange) then
  begin
    WasProtected := IsSheetProtected(SheetInterface.ExcelSheet);
    if WasProtected then
      if not SetSheetProtection(SheetInterface.ExcelSheet, false) then
      begin
        SheetInterface.PostMessage(ermWorksheetProtectionFault, msgError);
        exit;
      end;
    ERange.Clear;
    if WasProtected then
      SetSheetProtection(SheetInterface.ExcelSheet, true);
  end;
  Msg := SysUtils.Format('Удален отдельный показатель "%s" (куб: "%s", мера: "%s"), по адресу %s',
    [Name, CubeName, MeasureName, Address]);
  if Assigned(eName) then
    eName.Delete;
  if Assigned(eUserName) then
    eUserName.Delete;
  MapLinkedTypeFormulasValues;
  Owner.Delete(GetSelfIndex);
end;

procedure TSheetSingleCell.SetTotalType(AType: TSheetTotalType);
begin
  FTotalType := AType;
  SetDefaultStyles;
end;

function TSheetSingleCell.GetElementCaption: string;
begin
  result := Name;
end;

procedure TSheetSingleCell.ApplyStyles;
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
end;

function TSheetSingleCell.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue:
      case TotalType of
        wtMeasure: result := snSingleCells;
        wtResult: result := snResultSingleCells;
        wtConst: result := snSingleCellConst;
      end;
    esValueprint:
      case TotalType of
        wtMeasure: result := snSingleCellsPrint;
        wtResult: result := snResultSingleCellsPrint;
        wtConst: result := snSingleCellConstPrint;
      end;
  end;
end;

function TSheetSingleCell.GetValue: string;
var
  CellRange: ExcelRange;
begin
  result := '';
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  CellRange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if not Assigned(CellRange) then
    exit;
  result := CellRange.value2;
end;

function TSheetSingleCell.GetExcelRange: ExcelRange;
begin
  result := nil;
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  result := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
end;

function TSheetSingleCell.GetUserExcelName: string;
begin
  if FUserExcelName = '' then
  begin
    FUserExcelName := TSheetSingleCellCollectionInterface(Owner).GetTotalCounterValue(TotalType);
    case TotalType of
      wtFree: FUserExcelName := 'Отдельный_Свободный_' + FUserExcelName;
      wtMeasure: FUserExcelName := 'Отдельная_Мера_' + FUserExcelName;
      wtResult: FUserExcelName := 'Отдельный_Результат_' + FUserExcelName;
      wtConst: FUserExcelName := 'Отдельная_Константа_' + FUserExcelName;
    end;
  end;
  result := snUserNamePrefix + snSeparator + FUserExcelName;
end;

procedure TSheetSingleCell.SwitchType(out CommentForHistory: string);
var
  ESheet: ExcelWorkSheet;
  ENameObj: ExcelXP.Name;
  ERange: ExcelRange;
begin
  ESheet := SheetInterface.ExcelSheet;
  ENameObj := GetNameObject(ESheet, ExcelName);
  if not Assigned(ENameObj) then
    exit;
  ERange := GetExcelRange;
  if not Assigned(ERange) then
    exit;
  ENameObj.Delete;
  ENameObj := GetNameObject(ESheet, GetUserExcelName);
  if Assigned(ENameObj) then
    ENameObj.Delete;
  if TotalType = wtResult then
  begin
    TotalType := wtMeasure;
    CommentForHistory := SysUtils.Format('Тип отдельного показателя "%s" изменен с результата на меру.',
      [GetElementCaption]);
  end
  else
  begin
    TotalType := wtResult;
    CommentForHistory := SysUtils.Format('Тип отдельного показателя "%s" изменен с меры на результат.',
      [GetElementCaption]);
  end;
  FUserExcelName := '';
  MarkObject(ESheet, ERange, ExcelName, false);
  MarkObject(ESheet, ERange, GetUserExcelName, false);
  CommentForHistory := ConvertStringToCommaText(CommentForHistory);
end;

function TSheetSingleCell.GetUnderlayingTotal: TSheetTotalInterface;
var
  Row, Column, Shift, TotalIndex: integer;
  TotalsRange: ExcelRange;
  TotalString, TotalId: string;
begin
  result := nil;
  if not PlacedInTotals then
    exit;
  if not GetAddressPoint(Column, Row) then
    exit;
  TotalsRange := GetRangeByName(SheetInterface.ExcelSheet, BuildExcelName(sntTotals));
  if not Assigned(TotalsRange) then
    exit;
  Shift := Column - TotalsRange.Column;
  if (Shift < 0) or (Shift >= SheetInterface.TotalSections.Count) then
    exit;
  TotalString := SheetInterface.TotalSections[Shift];
  TotalId := CutPart(TotalString, snSeparator);
  TotalIndex := SheetInterface.Totals.FindById(TotalId);
  try
    result := SheetInterface.Totals[TotalIndex];
  except
  end;
end;

function TSheetSingleCell.GetOnDeleteWarning: string;
var
  Part1: string;
  i: integer;
  Total: TSheetTotalInterface;
begin
  result := qumDelSingleCell + '"' + GetElementCaption + '"?';

  {ссылающиеся типовые}
  Part1 := '';
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[i];
    if Total.TypeFormula.Enabled and Total.TypeFormula.ContainAlias(Alias) then
    begin
      AddTail(Part1, ', ');
      Part1 := Part1 + '"' + Total.GetElementCaption + '"';
    end;
  end;

  if Part1 <> '' then
  begin
    result := result + #10#10'ВНИМАНИЕ! При удалении данного показателя типовые формулы следующих показателей станут некорректны:' + #10;
    result := result + Part1 + '.';
  end;
end;

function TSheetSingleCell.RepairConst: TConstInterface;
begin
  result := nil;
  if TotalType <> wtConst then
    exit;
  result := SheetInterface.Consts.Append;
  with result do
  begin
    Name := Self.Name;
    Value := Self.Value;
    IsSheetConst := true;
    IsInherited := false;
    Id := -1;
  end;
end;

function TSheetSingleCell.GetDividedValue(AValue: extended): extended;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case TotalMultiplier of
      tmE3: result := result / 1E3;
      tmE6: result := result / 1E6;
      tmE9: result := result / 1E9
    end;
end;

function TSheetSingleCell.GetDividedValue(AValue: string): string;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case TotalMultiplier of
      tmE3: result := FloatToStr(StrToFloat(result) / 1E3);
      tmE6: result := FloatToStr(StrToFloat(result) / 1E6);
      tmE9: result := FloatToStr(StrToFloat(result) / 1E9);
    end;
end;

function TSheetSingleCell.GetMultipliedValue(AValue: extended): extended;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case TotalMultiplier of
      tmE3: result := result * 1E3;
      tmE6: result := result * 1E6;
      tmE9: result := result * 1E9;
    end;
end;

function TSheetSingleCell.GetMultipliedValue(AValue: string): string;
begin
  result := AValue;
  if (Format = fmtCurrency) then
    case TotalMultiplier of
      tmE3: result := FloatToStr(StrToFloat(result) * 1E3);
      tmE6: result := FloatToStr(StrToFloat(result) * 1E6);
      tmE9: result := FloatToStr(StrToFloat(result) * 1E9);
    end;
end;

{ TSheetSingleCellCollection }

function TSheetSingleCellCollection.Append: TSheetSingleCellInterface;
begin
  result := TSheetSingleCell.Create(Self);
  inherited Add(result);
  result.PermitEditing := PermitEditing;
end;

procedure TSheetSingleCellCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TSheetSingleCellCollection.FindByID(ID: string): integer;
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

function TSheetSingleCellCollection.GetCollectionName: string;
begin
  result := 'singlecells'
end;

function TSheetSingleCellCollection.GetItem(
  Index: integer): TSheetSingleCellInterface;
begin
  result := Get(Index);
end;

procedure TSheetSingleCellCollection.SetItem(Index: integer;
  Item: TSheetSingleCellInterface);
begin
  Put(Index, Item);
end;

function TSheetSingleCellCollection.Validate: boolean;
var
  i, ErrorCode: integer;
  DetailText: string;
begin
  result := true;
  if Empty then
    exit;
  Owner.OpenOperation(pfoSingleCellsValidate, not CriticalNode, not NoteTime, otProcess);
  for i := 0 to Count - 1 do
  if not Items[i].Validate(DetailText, ErrorCode) then
    begin
      result := false;
      Owner.PostMessage('- отдельный показатель "' + Items[i].Name +
        '": ' + DetailText + ';', msgWarning);
    end;
  Owner.CloseOperation;
end;

procedure TSheetSingleCellCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  SingleCell: TSheetSingleCellInterface;
begin
  inherited;
  if not Assigned(Node) then
    exit;
  NL := Node.selectNodes('singlecell');
  for i := 0 to NL.length - 1 do
  begin
    SingleCell := Append;
    SingleCell.ReadFromXML(NL[i]);
  end;
  FTotalCounters.ReadFromXml(Node);
end;

procedure TSheetSingleCellCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  inherited;
  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'singlecell', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;
  FTotalCounters.WriteToXml(Node);
end;

function TSheetSingleCellCollection.Refresh(Force: boolean): boolean;
var
  i: integer;
begin
  result := true;
  if Empty then
    exit;
  Owner.OpenOperation(pfoSingleCellsRefresh, not CriticalNode,
    not NoteTime, otUpdate);
  for i := 0 to Count - 1 do
    Items[i].Refresh(Force);
  Owner.CloseOperation;
end;

procedure TSheetSingleCellCollection.GetNumberFormats;
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
      RangeName := ExcelName;
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
        end;
      except
      end;  
    end
    else //генерим маску формата сами
      NumberFormat := BuildFormatMask;
end;

function TSheetSingleCellCollection.FindByName(
  AName: string): TSheetSingleCellInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Name = AName then
    begin
      result := Items[i];
      exit;
    end;
end;

function TSheetSingleCellCollection.CheckByType(TotalTypes: TTotalTypes): boolean;
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

function TSheetSingleCellCollection.FindByAlias(aStr: string): TSheetSingleCellInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].Alias = aStr) then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetSingleCellCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue: result := 'Данные';
    esValuePrint: result := 'Данные (печать)';
  end;
end;

function TSheetSingleCellCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

end.

