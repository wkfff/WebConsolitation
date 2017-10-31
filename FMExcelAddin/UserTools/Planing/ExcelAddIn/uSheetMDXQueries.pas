{
  ������������ MDX-�������� ����� ������������.
  � �������� ������� �����, ������� ��������� ������, � ����� ����,
  ������������ ����� MDX-�������� ��� ������� ������ �����������.
  ������ ������ ������ ����������� 1..n ����������� �����.
  ���������, ����� "������ - ����������" ����� �������� "���� �� ������",
  MDX-������ �� ����� ���� ���������� ������-�� ����������� ����������,
  �.� ���� ������� ������� TSheetTotal. ������� ����������� �����.
  ����������: !!!
  ��� � �� ���� ������ ������, ����� �������� ����������� ����������
  ��������� (����� ����� ����������, ���������� � ��.).
  ��� ����� ������������ ����, ������� ��� � ��������� ����� ����� �����
  ���������.
  ���������� ����� ��������������, �� ���� ����� ����������!
  ���������� �������������� ������.
}
unit uSheetMDXQueries;

interface
uses
  classes, uSheetObjectModel, uFMAddinGeneralUtils, SysUtils,
  uXMLCatalog, uFMExcelAddInConst, uGlobalPlaningConst, uFMAddinXMLUtils;

type
  TSheetMDXQuery = class;

  { ��������� ���� �������� ����� ������������.
    ��������� �� ������������� �������������� (����������, �������� ���������)
    �����. ��������������� ��� ��� �������� ��������� �������������,
    � ������ �������� ���������.}
  TSheetMDXQueries = class(TList)
  private
    FSheet: TSheetInterface;
    {�������� ��������� ��������� - ����� �����������,
     ������� �������������� ����� ��������}
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

  {���� ���������� ������ ����� ������������.
   ��������� ������ ������������ �� ���� ��������� TTotalInterface}
  TSheetMDXQuery = class(TList)
  private
    {������������ ���������}
    FOwner: TSheetMDXQueries;
    FAliases: TStringList;
    FAliasValues: TStringList;
    {�������� �� ������ "�������". �.� � ��� ���� �� ���� ��������� ��
    ������� 32�� � �� ���� ��������� ������ ������ �������}
    FIsLargeQuery: boolean;
    FProviderId: string;
    FSheetInterface: TSheetInterface;

    {������ �� ��������� ������}
    function SheetInterface: TSheetInterface;
    {��������� ������ ������� ���}
    procedure CollectAxisAliases(AxType: TAxisType);
    {��������� ������ �����������}
    procedure CollectTotalAliases;
    {��������� ��� ������ �������, ������� ����������� � �������}
    procedure CollectAliases;

    {����� �������: ���������� ������� - DataMember}
    function DataMemberFilterPiece(AxType: TAxisType): string;
    {����� �������: ���������� ������� - ���� Non Empty}
    function TotalNonEmptyFilterPiece: string;
    {����� �������: ���� ������ �� ������ �������}
    function NonEmptyFilterPiece: string;
    {����� �������: ��� �����������. ����������� �� ���� ������}
    function RowAndColumnAxisPiece: string;
    {����� �������: ��� ���. ����������� �� �����������}
    function MeasuresAxisPiece: string;
    {����� �������: ���������� ���������� ���� �������� ������� � �������}
    function LeavesCalculationPiece: string;
    {����� �������: ���������� �������� c ������������� �������}
    function FiltersCalculationPiece: string;
    {����� �������: ���������� ������ (����������� � ����-������) ��� ���������� ���}
    function DataCalculationPiece: string;
    {����� �������: ��� �������������� -����������� ���� }
    function ForwardCalculationPiece: string;
    {����� �������: ������ �������� - ����}
    function FilterExpressionPiece: string;
    {����� �������: ���������� �������� ��� ��� ���������}
    function SetDeclarationPiece: string;
    { MDX-�������� �������� ���}
    (*function GetAxisSet(AxisElement: TSheetAxisElementInterface;
      var LargeQuery: boolean): string;*)


  protected
    function GetQueryTotal(Index: integer): TSheetTotalInterface;
    procedure SetQueryTotal(Index: integer; Item: TSheetTotalInterface);

    {���������� ����� MDX �������  -  ������� ������ �������� ������}
    function GetText: string;
    {���������� ������ � �������� �� ������-���, ������� ������������ � �������}
    function GetCube: TCube;
    {���������� ��� ����  �������}
    function GetCubeName: string;


    {����� ������������� ��������}
    property QueryTotals[Index: integer]: TSheetTotalInterface
      read GetQueryTotal write SetQueryTotal; default;
    {�������� ������� ��� ��� ����� �������}
    function GetAxisLeafElement(AxType: TAxisType): TSheetAxisElementInterface;
    {�������� ������� ����� ��� ����� �������}
    function GetRowLeafElement: TSheetAxisElementInterface;
    {�������� ������� �������� ��� ����� �������}
    function GetColumnLeafElement: TSheetAxisElementInterface;
    {��� �������}
    property Cube: TCube read GetCube;
    {��� ���� �������}
    property CubeName: string read GetCubeName;
  public
    constructor Create(AOwner: TSheetMDXQueries); overload;
    constructor Create(Total: TSheetTotalInterface); overload;
    destructor Destroy; override;

    {������ ���������� �����������, �������������� ��������. (��� ���������)}
    function TotalCaptions: string;

    {����� MDX �������}
    property Text: string read GetText;
    property Aliases: TStringList read FAliases;
    {�������� �� ������ "�������". �.� � ��� ���� �� ���� ��������� ��
    ������� 32�� � �� ���� ��������� ������ ������ �������}
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
  {����� ��� ��������� �������}
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
      Query := Append; //����� ������

      Query.Add(Totals[i]);
      Include(TotalSet, i);

      Query.ProviderId :=  Totals[i].Cube.ProviderId;


      {����� MDX-�������}
      for j := i + 1 to Totals.Count - 1 do
        if Totals.MayTotalsHaveSameMDX(i, j) and not (j in TotalSet) then
        begin
          Query.Add(Totals[j]);
          Include(TotalSet, j);
        end;

      //����� ������������ ��������� ������� - ������ �������
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
  {��������!!! ���� �������� �� ������� ��������� ������ �� ����������.
   ���������� � ��������� ������ ��� ��� ����������� :)}
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

  {�������� ������������� ���������.
   � ������� ����� ������, ��������� ���������}
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
    *** ��� ��� ��������� ������ ����������. �� ����� ����������.
    ���� ����� ����������� ����� ��������� QueryTotals, ������
    ��� �������� �� T_ (������ �����������)
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

  {*** ����� ���� �������� ������������ ����� ����������� � ����� ���� }
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
      {���� ����� ��������������� ����, �� ���������� ������� CrossJoin
       � NonEmptyCrossJoin, ����� ������ ������ non empty
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
  � �� ���� �������� ������ ��������� GetMeasuresSet.
  � ������ ���� ����� ��������� ����� �� ����.
  ����, ��� ������������� ��� ��� ������ ���� ������,
  ����� � �� �������, ������ ������ ����� ��� ��������������� ������-�����!!!
}
function TSheetMDXQuery.MeasuresAxisPiece: string;
var
  i: integer;
begin
  result := '';

  {������������ ��� �� ������}
  for i := 0 to FAliases.Count - 1 do
  begin
    AddTail(result, ', ');
    result := result +  FAliases.Strings[i];
  end;

  {���������� �������� �������}
  AddTail(result, ', ');
  result := result + StrAsMeasure(mnIsRowLeaf);
  AddTail(result, ', ');
  result := result + StrAsMeasure(mnIsColumnLeaf);

  {"������������"}
  result := SetBrackets(result) + ' on Axis(0) ';
end;

function TSheetMDXQuery.LeavesCalculationPiece: string;
  {��������� ���������� ���� ����� ��� �������� ���
   CalcName - ��� ���������� ���� ���� }
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
    //��� �������� ����� ������ ����������
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

    //��� �������� ����� ������ ����������
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
  {���� ������ ���� �������� � ���� ����������� ������.
   ����� ��� ������� ����������.}
  if Count > 0 then
    result := QueryTotals[0].Cube
  else
    result := nil; //��������� ��������� ���� ���������
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
  {���������� ���������� ���}
  WithClause := ForwardCalculationPiece;
  AddHead('WITH ', WithClause);

  {��� �������}
  SelectClause := RowAndColumnAxisPiece;
  AddHead(', ', SelectClause);
  SelectClause := MeasuresAxisPiece + SelectClause;
  AddHead(' SELECT ', SelectClause);

  {���}
  FromClause := CubeName;
  AddHead(' FROM ', FromClause);

  {������}
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

    { ���� ���������� ���������� ����� �������, �� �������� ��������� ���,
      ������� ������ �������� �������� ������������� �������.}
    while Length(RowsList.Text + ColumnsList.Text) > 56000 do
    try
      IsLargeQuery := true;

      { ����� �� ���� ����� �� ����, ����� ���
        (���� ��� �����, �� ���� �� ������ �� �������)}
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
      break; // ��������� ��� ���������, � ������ ��� ����� �����. ������ ������.
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
