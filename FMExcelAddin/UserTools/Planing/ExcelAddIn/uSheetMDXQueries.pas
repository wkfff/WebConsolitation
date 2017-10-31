{
  Формирование MDX-запросов листа планирования.
  В процессе расчета листа, готовой объектной модели, в общем виде,
  соответствет набор MDX-запросов для выборки данных показателей.
  Каждый запрос набора обслуживает 1..n показателей листа.
  Поскольку, связь "запрос - показатель" имеет характер "один ко многим",
  MDX-запрос не может быть аттрибутом какого-то конкретного показателя,
  т.е быть методом объекта TSheetTotal. Запросы формируются здесь.
  ПРИМЕЧАНИЕ: !!!
  Как и во всех других местах, опять реализую примитивный функционал
  коллекции (связи между элементами, добавление и пр.).
  Это явное дублирование кода, которое уже в ближайшее время может стать
  проблемой.
  Исправлять нужно централизовано, во всех наших коллекциях!
  Выделением промежуточного класса.
}
unit uSheetMDXQueries;

interface
uses
  classes, uSheetObjectModel, uFMAddinGeneralUtils, SysUtils,
  uXMLCatalog, uFMExcelAddInConst, uGlobalPlaningConst, uFMAddinXMLUtils;

type
  TSheetMDXQuery = class;

  { Коллекция всех запросов листа планирования.
    Коллекция не подразумевает редактирование (добавление, удаление элементов)
    извне. Подразумевается что все элементы создаются единовременно,
    в момент создания коллекции.}
  TSheetMDXQueries = class(TList)
  private
    FSheet: TSheetInterface;
    {Создание элементов коллекции - групп показателей,
     которые обрабатываются одним запросом}
    procedure CreateQueryGroups;
  protected
    function GetItem(Index: integer): TSheetMDXQuery;
    procedure SetItem(Index: integer; Item: TSheetMDXQuery);
  public
    constructor Create(AOwner: TSheetInterface);
    destructor Destroy; override;
    procedure Clear; override;
    function Append: TSheetMDXQuery;
    procedure Delete(Index: integer);

    property Items[Index: integer]: TSheetMDXQuery
      read GetItem write SetItem; default;

  end;

  {Один конкретный запрос листа планирования.
   Структура данных представляет из себя коллекцию TTotalInterface}
  TSheetMDXQuery = class(TList)
  private
    {Родительская коллекция}
    FOwner: TSheetMDXQueries;
    FAliases: TStringList;
    FAliasValues: TStringList;
    {Является ли запрос "большим". Т.е в нем хотя бы одно измерение не
    влезлов 32кб и мы были вынуждены тащить уровни целиком}
    FIsLargeQuery: boolean;
    FProviderId: string;
    FSheetInterface: TSheetInterface;

    {Ссылка на объектную модель}
    function SheetInterface: TSheetInterface;
    {Заполняем списки алиасов оси}
    procedure CollectAxisAliases(AxType: TAxisType);
    {Заполняем алиасы показателей}
    procedure CollectTotalAliases;
    {Заполняем все списки алиасов, которые содержаться в запросе}
    procedure CollectAliases;

    {Кусок запроса: Фильтровое условие - DataMember}
    function DataMemberFilterPiece(AxType: TAxisType): string;
    {Кусок запроса: Фильтровое условие - меры Non Empty}
    function TotalNonEmptyFilterPiece: string;
    {Кусок запроса: Весь фильтр на данные запроса}
    function NonEmptyFilterPiece: string;
    {Кусок запроса: Ось разрезности. Формируется по осям модели}
    function RowAndColumnAxisPiece: string;
    {Кусок запроса: Ось мер. Формируется по показателям}
    function MeasuresAxisPiece: string;
    {Кусок запроса: Объявления вычислимых меры листовых позиций в запросе}
    function LeavesCalculationPiece: string;
    {Кусок запроса: Объявления фильтров c множественным выбором}
    function FiltersCalculationPiece: string;
    {Кусок запроса: Объявления данных (показателей и юник-неймов) как вычислимых мер}
    function DataCalculationPiece: string;
    {Кусок запроса: Все предвычисления -объявленные меры }
    function ForwardCalculationPiece: string;
    {Кусок запроса: Строка фильтров - срез}
    function FilterExpressionPiece: string;
    {Кусок запроса: объявления множеств для оси измерений}
    function SetDeclarationPiece: string;
    { MDX-описание элемента оси}
    (*function GetAxisSet(AxisElement: TSheetAxisElementInterface;
      var LargeQuery: boolean): string;*)


  protected
    function GetQueryTotal(Index: integer): TSheetTotalInterface;
    procedure SetQueryTotal(Index: integer; Item: TSheetTotalInterface);

    {Возвращает текст MDX запроса  -  функция самого верхнего уровня}
    function GetText: string;
    {Возвращает ссылку в каталоге на объект-куб, который используется в запросе}
    function GetCube: TCube;
    {Возвращает имя куба  запроса}
    function GetCubeName: string;


    {Итоги обслуживаемые запросом}
    property QueryTotals[Index: integer]: TSheetTotalInterface
      read GetQueryTotal write SetQueryTotal; default;
    {Листовой элемент оси для этого запроса}
    function GetAxisLeafElement(AxType: TAxisType): TSheetAxisElementInterface;
    {Листовой элемент строк для этого запроса}
    function GetRowLeafElement: TSheetAxisElementInterface;
    {Листовой элемент столбцов для этого запроса}
    function GetColumnLeafElement: TSheetAxisElementInterface;
    {Куб запроса}
    property Cube: TCube read GetCube;
    {Имя куба запроса}
    property CubeName: string read GetCubeName;
  public
    constructor Create(AOwner: TSheetMDXQueries); overload;
    constructor Create(Total: TSheetTotalInterface); overload;
    destructor Destroy; override;

    {Список заголовков показателей, обрабатываемых запросом. (для индикации)}
    function TotalCaptions: string;

    {Текст MDX запроса}
    property Text: string read GetText;
    property Aliases: TStringList read FAliases;
    {Является ли запрос "большим". Т.е в нем хотя бы одно измерение не
    влезлов 32кб и мы были вынуждены тащить уровни целиком}
    property IsLargeQuery: boolean read FIsLargeQuery write FIsLargeQuery;
    property ProviderId: string read FProviderId write FProviderId;
  end;

implementation

constructor TSheetMDXQueries.Create(AOwner: TSheetInterface);
begin
  if Assigned(AOwner) then
  begin
    FSheet := AOwner;
    CreateQueryGroups;
  end;
end;

destructor TSheetMDXQueries.Destroy;
begin
  {Убить все созданные объекты}
  Clear;
  inherited Destroy;
end;

function TSheetMDXQueries.Append: TSheetMDXQuery;
begin
  result := TSheetMDXQuery.Create(Self);
  inherited Add(result);
end;

procedure TSheetMDXQueries.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TSheetMDXQueries.Clear;
begin
  while Count > 0 do
    Delete(0);
end;

function TSheetMDXQueries.GetItem(Index: integer): TSheetMDXQuery;
begin
  result := Get(Index);
end;

procedure TSheetMDXQueries.SetItem(Index: integer; Item: TSheetMDXQuery);
begin
  Put(Index, Item);
end;

procedure TSheetMDXQueries.CreateQueryGroups;
var
  i, j: integer;
  Query: TSheetMDXQuery;
  TotalSet: set of byte;
  Totals: TSheetTotalCollectionInterface;
begin
  TotalSet := [];
  Totals := FSheet.Totals;
  for i := 0 to Totals.Count - 1 do
    if (Totals[i].TotalType in [wtMeasure, wtResult]) and not (i in TotalSet) then
    begin
      Query := Append; //новый запрос

      Query.Add(Totals[i]);
      Include(TotalSet, i);

      Query.ProviderId :=  Totals[i].Cube.ProviderId;


      {Поиск MDX-братьев}
      for j := i + 1 to Totals.Count - 1 do
        if Totals.MayTotalsHaveSameMDX(i, j) and not (j in TotalSet) then
        begin
          Query.Add(Totals[j]);
          Include(TotalSet, j);
        end;

      //конец формирования структуры запроса - делаем просчет
      Query.CollectAliases;
    end;
end;

constructor TSheetMDXQuery.Create(AOwner: TSheetMDXQueries);
begin
  FOwner := AOwner;
  FSheetInterface := AOwner.FSheet;
  FAliases := TStringList.Create;
  FAliasValues := TStringList.Create;
  FIsLargeQuery := false
end;

constructor TSheetMDXQuery.Create(Total: TSheetTotalInterface);
begin
  FOwner := nil;
  Add(Total);
  FSheetInterface := Total.SheetInterface;
  ProviderId :=  Total.Cube.ProviderId;
  FAliases := TStringList.Create;
  FAliasValues := TStringList.Create;
  FIsLargeQuery := false;
  CollectAliases;
end;

destructor TSheetMDXQuery.Destroy;
begin
  {Внимание!!! Сами элементы на которые ссылается список не уничтожаем.
   Показатели в объектной модели нам еще понадобятся :)}
  FOwner := nil;
  Clear;
  FreeStringList(FAliases);
  FreeStringList(FAliasValues);
  inherited Destroy;
end;

function TSheetMDXQuery.SheetInterface: TSheetInterface;
begin
  result := FSheetInterface;
end;

function TSheetMDXQuery.GetQueryTotal(Index: integer): TSheetTotalInterface;
begin
  result := Get(Index);
end;

procedure TSheetMDXQuery.SetQueryTotal(Index: integer; Item: TSheetTotalInterface);
begin
  Put(Index, Item);
end;

procedure TSheetMDXQuery.CollectAxisAliases(AxType: TAxisType);
var
  i: integer;
  Alias, Value: string;
  Axis: TSheetAxisCollectionInterface;
begin
  Axis := SheetInterface.GetAxis(AxType);

  for i := 0 to Axis.Count - 1 do
  begin
    if Cube.ProviderId <> Axis[i].ProviderId then
      continue;
    if Cube.DimAndHierInCube(Axis[i].Dimension, Axis[i].Hierarchy) then
    begin
      Alias := ' ' + StrAsMeasure(Axis[i].Alias);
      FAliases.Add(Alias);
      Value :=  ' ''' + Axis[i].FullDimensionNameMDX + '.CurrentMember.UniqueName''';
      FAliasValues.Add(Value);
    end;
  end;
end;

procedure TSheetMDXQuery.CollectTotalAliases;
var
  i: integer;
  Alias, Value: string;
begin
  for i := 0 to Count - 1 do
  begin
    Alias := ' ' + StrAsMeasure(QueryTotals[i].Alias);
    FAliases.Add(Alias);
    Value := ' ''' + StrAsMeasure(QueryTotals[i].MeasureName) + '''';
    FAliasValues.Add(Value);
  end;
end;

procedure TSheetMDXQuery.CollectAliases;
begin
  FAliases.Clear;
  FAliasValues.Clear;

  {Собираем промежуточные структуры.
   К моменту этого вызова, коллекция наполнена}
  CollectAxisAliases(axRow);
  CollectAxisAliases(axColumn);
  CollectTotalAliases;
end;

function TSheetMDXQuery.DataMemberFilterPiece(AxType: TAxisType): string;
var
  i: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  result := '';
  Axis := SheetInterface.GetAxis(AxType);

  for i := 0 to Axis.Count - 1 do
  begin
    if Cube.ProviderId <> Axis[i].ProviderId then
      continue;
    if Cube.DimAndHierInCube(Axis[i].Dimension, Axis[i].Hierarchy) then
    begin
      result := Axis[i].FullDimensionNameMDX;
      AddTail(result, '.CurrentMember is ');
      AddTail(result, Axis[i].FullDimensionNameMDX + '.Parent.DataMember');
      result := TupleBrackets(result);
    end;
  end;
end;

function TSheetMDXQuery.TotalNonEmptyFilterPiece: string;
var
  i: integer;
begin
  {
    *** Вот эта процедура просто перенесена. Ее нужно переписать.
    Тоже самое вычисляется через коллекцию QueryTotals, только
    без проверки на T_ (лишняя зависимость)
  }
  result := '';
  for i := 0 to FAliases.Count - 1 do
    if Pos('T_', FAliases.Strings[i]) > 0 then
    begin
      AddTail(result, ' or ');
      result := result + 'not IsEmpty(' + FAliases.Strings[i] + ') ';
    end;
end;

function TSheetMDXQuery.NonEmptyFilterPiece: string;
var
  FilterT, FilterR, FilterC: string;
begin
  FilterT := TotalNonEmptyFilterPiece;
  FilterR := DataMemberFilterPiece(axRow);
  FilterC := DataMemberFilterPiece(axColumn);

  {*** Давно пора написать перечисление через разделитель в общем виде }
  result := FilterT;
  if FilterR <> '' then
    AddTail(result, ' or ');
  result := result + FilterR;
  if FilterC <> '' then
    AddTail(result, ' or ');
  result := result + FilterC;
end;

function TSheetMDXQuery.RowAndColumnAxisPiece: string;

  procedure GetAxesSet(Axis: TSheetAxisCollectionInterface; var ResultStr: string);
  var
    i: integer;
  begin
    for i := 0 to Axis.Count - 1 do
    begin
      if Cube.ProviderId <> Axis[i].ProviderId then
        continue;
      if Cube.DimAndHierInCube(Axis[i].Dimension, Axis[i].Hierarchy) then
      begin
        AddTail(ResultStr, ' * ');
        ResultStr := ResultStr + Axis[i].Alias;
      end;
    end;
  end;
  
begin
  result := '';
  GetAxesSet(SheetInterface.Rows, result);
  GetAxesSet(SheetInterface.Columns, result);
  if result <> '' then
    if Pos(' * ', result) = 0 then
      result := ' non empty Filter(' + result + ', ' + NonEmptyFilterPiece + ') on Axis(1)'
    else
    begin
      {Если стоит соответствующий флаг, то превращаем простой CrossJoin
       в NonEmptyCrossJoin, иначе просто ставим non empty
      }
      if SheetInterface.AllowNECJ then
      begin
        result := StringReplace(result, ' * ', ' , ', [rfReplaceAll]);
        result := ' nonemptycrossjoin(' + result + ')';
      end
      else
      begin
        result := ' non empty ' + SetBrackets(result);
      end;

      result := result + ' on Axis(1)';
    end;
end;

{***
  Я не смог прочесть старую процедуру GetMeasuresSet.
  А именно этот метод выполняет здесь ее роль.
  Если, там действительно все так сложно надо делать,
  тогда я не понимаю, нахера вообще нужны эти вспомогательные стринг-листы!!!
}
function TSheetMDXQuery.MeasuresAxisPiece: string;
var
  i: integer;
begin
  result := '';

  {Перекачиваем все из списка}
  for i := 0 to FAliases.Count - 1 do
  begin
    AddTail(result, ', ');
    result := result +  FAliases.Strings[i];
  end;

  {подцепляем листовые мемберы}
  AddTail(result, ', ');
  result := result + StrAsMeasure(mnIsRowLeaf);
  AddTail(result, ', ');
  result := result + StrAsMeasure(mnIsColumnLeaf);

  {"Заковычиваем"}
  result := SetBrackets(result) + ' on Axis(0) ';
end;

function TSheetMDXQuery.LeavesCalculationPiece: string;
  {Выражение вычислимой меры листа для заданной оси
   CalcName - имя вычислимой меры меры }
  (*function LeafCalculation(AxType: TAxisType; CalcName: string): string;
  var
    Leaf: TSheetAxisElementInterface;
    LevelName: string;
  begin
    result := '';
    Leaf := GetAxisLeafElement(AxType);

    if Assigned(Leaf) then
    begin
      LevelName := Leaf.Levels[Leaf.Levels.Count - 1].Name;
      result := Format('(%s.CurrentMember.level.Name = "%s")',
        [Leaf.FullDimensionNameMDX, LevelName]);
      result := TupleBrackets(result +
        Format(' or IsLeaf(%s.CurrentMember)', [Leaf.FullDimensionNameMDX]));

      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''IIF(' +
        result + ' ,"true", "false")'' ';
    end
    else
      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''"true"'' ';
    result := result + ', SolveOrder = 100';
  end;

  function LeafCalculationEx(AxType: TAxisType; CalcName: string): string;
  var
    AxisCollection: TSheetAxisCollectionInterface;
    LevelName, tmpStr: string;
    i: integer;
  begin
    result := '';
    AxisCollection := Sheet.GetAxis(AxType);
    for i := 0 to AxisCollection.Count - 1 do
      if CheckDimension(Cube, AxisCollection[i]) then
      begin
        AddTail(result, ' and ');
        LevelName := AxisCollection[i].Levels.LastItem.Name;
        tmpStr := Format('(%s.CurrentMember.level.Name = "%s")',
          [AxisCollection[i].FullDimensionNameMDX, LevelName]);
        tmpStr := TupleBrackets(tmpStr + Format(' or IsLeaf(%s.CurrentMember)',
          [AxisCollection[i].FullDimensionNameMDX]));
        result := result + tmpStr;
      end;
    if result <> '' then
      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''IIF(' +
        result + ' ,"true", "false")'' '
    else
      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''"true"'' ';
    result := result + ', SolveOrder = 100';
  end; *)

  function LeafCalculationEx2(AxType: TAxisType; CalcName: string): string;
  var
    AxisCollection: TSheetAxisCollectionInterface;
    i: integer;
  begin
    result := '';
    AxisCollection := SheetInterface.GetAxis(AxType);
    for i := 0 to AxisCollection.Count - 1 do
      if CheckDimension(Cube, AxisCollection[i]) then
      begin
        AddTail(result, ' and ');
        result := result + Format(
          '(Intersect(%s, Descendants(%s.CurrentMember, 0, AFTER)).Count = 0)',
          [AxisCollection[i].Alias, AxisCollection[i].FullDimensionNameMDX]);
      end;
    if result <> '' then
      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''IIF(' +
        result + ' ,"true", "false")'' '
    else
      result := ' MEMBER ' + StrAsMeasure(CalcName) + ' AS ''"true"'' ';
    result := result + ', SolveOrder = 100';
  end;

begin
  result := LeafCalculationEx2(axRow, mnIsRowLeaf) +
            LeafCalculationEx2(axColumn, mnIsColumnLeaf);
end;

function TSheetMDXQuery.FiltersCalculationPiece: string;
var
  i: integer;
  SlicerName, SlicerValue: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    Filter := SheetInterface.Filters[i];
    //для проверки берем первый показатель
    if Filter.IsAffectsTotal(QueryTotals[0]) and Filter.IsMultiple then
    begin
      SlicerName := Filter.FullDimensionNameMDX + '.[Slicer] ';
      SlicerValue := TupleBrackets(SetBrackets(Filter.MdxText));
      SlicerValue := StringReplace(SlicerValue, '''', '''' + '''', [rfReplaceAll]);
      result := result + ' MEMBER ' + SlicerName +
        ' AS ' + '''' + 'AGGREGATE' + SlicerValue + '''';
    end;
  end;
end;

function TSheetMDXQuery.DataCalculationPiece: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to FAliases.Count - 1 do
  begin
    result := result + ' MEMBER ' +  FAliases.Strings[i] +
      ' AS ' + FAliasValues.Strings[i];
    if Pos('T_', FAliases.Strings[i]) = 0 then
      result := result + ', SolveOrder = 100'
  end;
end;

function TSheetMDXQuery.ForwardCalculationPiece: string;
begin
  result := SetDeclarationPiece + 
            LeavesCalculationPiece +
            FiltersCalculationPiece +
            DataCalculationPiece;
end;

function TSheetMDXQuery.FilterExpressionPiece: string;
var
  i: integer;
  Slicer: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    Filter := SheetInterface.Filters[i];

    //для проверки берем первый показатель
    if Filter.IsAffectsTotal(QueryTotals[0]) then
    begin
      AddTail(result, ', ');
      Slicer := IIF(Filter.IsMultiple,
                    Filter.FullDimensionNameMDX + '.[Slicer] ',
                    Filter.MdxText);
      result := result + Slicer;
    end;
  end;
  if result <> '' then
    result := TupleBrackets(result);
end;

function TSheetMDXQuery.GetCube: TCube;
begin
  {Кубы должны быть одниковы у всех показателей группы.
   Берем куб первого показателя.}
  if Count > 0 then
    result := QueryTotals[0].Cube
  else
    result := nil; //аварийное состояние всей коллекции
end;

function TSheetMDXQuery.GetCubeName: string;
begin
  result := MemberBrackets(Cube.Name);
end;

function TSheetMDXQuery.GetAxisLeafElement(AxType: TAxisType): TSheetAxisElementInterface;
var
  i: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  result := nil;
  Axis := SheetInterface.GetAxis(AxType);

  for i := 0 to Axis.Count - 1 do
  begin
    if Cube.ProviderId <> Axis[i].ProviderId then
      continue;
    if Cube.DimAndHierInCube(Axis[i].Dimension, Axis[i].Hierarchy) then
      result := Axis[i];
  end;
end;

function TSheetMDXQuery.GetRowLeafElement: TSheetAxisElementInterface;
begin
  result := GetAxisLeafElement(axRow);
end;

function TSheetMDXQuery.GetColumnLeafElement: TSheetAxisElementInterface;
begin
    result := GetAxisLeafElement(axColumn);
end;

function TSheetMDXQuery.TotalCaptions: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Count - 1 do
  begin
    AddTail(result, ', ');
    result := result + '"' + QueryTotals[i].Caption + '"';
  end;
end;

function TSheetMDXQuery.GetText: string;
var
  WithClause, SelectClause, FromClause, WhereClause: string;
begin
  {Объявления вычислимых мер}
  WithClause := ForwardCalculationPiece;
  AddHead('WITH ', WithClause);

  {Оси запроса}
  SelectClause := RowAndColumnAxisPiece;
  AddHead(', ', SelectClause);
  SelectClause := MeasuresAxisPiece + SelectClause;
  AddHead(' SELECT ', SelectClause);

  {Куб}
  FromClause := CubeName;
  AddHead(' FROM ', FromClause);

  {Фильтр}
  WhereClause := FilterExpressionPiece;
  AddHead(' WHERE ', WhereClause);

  {altogether}
  result := WithClause + SelectClause + FromClause + WhereClause;
end;

function TSheetMDXQuery.SetDeclarationPiece: string;

  procedure FillAxesList(AxisCollection: TSheetAxisCollectionInterface;
    var List, OrderBySize: TStringList);
  var
    i, j: integer;
    Added: boolean;
  begin
    for i := 0 to AxisCollection.Count - 1 do
    begin
      if CheckDimension(Cube, AxisCollection[i]) then
        List.Add(AxisCollection[i].MdxText)
      else
        List.Add('');
      Added := false;
      for j := 0 to i - 1 do
        if Length(List[i]) > Length(List[j]) then
        begin
          OrderBySize.Insert(j, IntToStr(i));
          Added := true;
          break;
        end;
      if not Added then
        OrderBySize.Add(IntToStr(i));
    end;
  end;

  procedure MakeResult(AxisCollection: TSheetAxisCollectionInterface;
    var List: TStringList; var ResultString: string);
  var
    i: integer;
  begin
    for i := 0 to List.Count - 1 do
      if List[i] <> '' then
      begin
        ResultString := ResultString + ' SET ' +  AxisCollection[i].Alias +
          ' AS ' + '''' + List[i] + '''';
      end
  end;

var
  RowsList, ColumnsList, RowsBySize, ColumnsBySize: TStringList;
  RowIndex, ColumnIndex: integer;
begin
  result := '';
  IsLargeQuery := false;
  RowsList := TStringList.Create;
  ColumnsList := TStringList.Create;
  RowsBySize := TStringList.Create;
  ColumnsBySize := TStringList.Create;
  try
    FillAxesList(SheetInterface.Rows, RowsList, RowsBySize);
    FillAxesList(SheetInterface.Columns, ColumnsList, ColumnsBySize);

    { Если обнаружили превышение длины запроса, то пытаемся сократить его,
      заменяя точные описания множеств перечислением уровней.}
    while Length(RowsList.Text + ColumnsList.Text) > 56000 do
    try
      IsLargeQuery := true;

      { Одной из осей может не быть, учтем это
        (если нет обеих, то сюда мы просто не попадем)}
      if RowsBySize.Count > 0 then
        RowIndex := StrToInt(RowsBySize[0])
      else
        RowIndex := -1;
      if ColumnsBySize.Count > 0 then
        ColumnIndex := StrToInt(ColumnsBySize[0])
      else
        ColumnIndex := -1;

      {$B-}  
      if (ColumnIndex = -1) or
        (Length(RowsList[RowIndex]) > Length(ColumnsList[ColumnIndex])) then
      begin
        RowsList[RowIndex] := SheetInterface.Rows[RowIndex].GetMDXLevelsSet;
        RowsBySize.Delete(0);
      end
      else
      begin
        ColumnsList[ColumnIndex] := SheetInterface.Columns[ColumnIndex].GetMDXLevelsSet;
        ColumnsBySize.Delete(0);
      end;
    except
      break; // сократили все множества, а запрос все равно велик. Делать нечего.
    end;

    MakeResult(SheetInterface.Rows, RowsList, result);
    MakeResult(SheetInterface.Columns, ColumnsList, result);
  finally
    FreeStringList(RowsList);
    FreeStringList(ColumnsList);
    FreeStringList(RowsBySize);
    FreeStringList(ColumnsBySize);
  end;
end;

end.
