{
  В этом модуле одна большая функция  - сборка MDX-запроса по модели
  MDX-запрос для вытягивания селсета.
  Режим "гигантских" таблиц (вся таблица одним зпросом).

  !!! Примечание! (Зверев) о происхождении этого модуля....
  Сначала я, естественно, хотел интегрировать сборку такого запроса в
  существующий механизм сборщиков (модуль uSheetMDXQueries).
  Стал делать наследника TSheetMDXQuery. и перекрывать нужные методы.
  Получилось криво, потому что не предназначены стандартные сборщики для
  такого режима. Там предварительно подготавливаются всякие дополнительные
  структуры (3 стринг-листа), которые только мешают и делают код непонятным.
  К тому же, класс "коллеция заросов" в этом случае так же является баластом
  Т.о решил записать сборку отдельно по двум соображениям
    1) Организация сущестующих классов почти непригодна для такой модификации
       (все становится лохматым и непонятным)
    2) Пока что лучше изолировать режим "гигантских" таблиц от нормального режима,
       что бы гарантировано не было влияний. Так же сделано и
       с основной обработкой гиагнского режима - отдельнй наследник TSheetInterface

  Очень желательно, когда "гигантский" режим стабилизируется, переосмыслить,
  интегрировать и отрефачить.
  Задача будет не простая :)
  (понадобиться перетряхнуть некоторые вещи обычного режима)
}
unit uSheetMDXQueryForCellset;

interface
uses
  classes, uSheetObjectModel, uFMAddinGeneralUtils, sysUtils,
  uXMLCatalog, uFMExcelAddInConst, uFMAddinExcelUtils,
  uFMAddinXMLUtils;

  {Вот, собственно, искомая функция. Все остальное утили}
  function GetCellsetQuery(SheetInterface: TSheetInterface; out ProviderId: string): string;
implementation

type

  TCellsetQuery = class
  private
    SheetInterface: TSheetInterface;
    QueryCube: TCube;
    WithClause, SelectClause, FromClause, WhereClause: string;
    OuterTotalPF: TStringList;
    FIsLargeQuery: boolean;

    function GetQueryCube: TCube;
    {Сколько мер листа входят в этот куб}
    function GetServedTotalsCount(Candidate: TCube): integer;
    {В этом кубе есть все измерения листа (частные фильтры игнорируем) }
    function IsFullDimensionality(Candidate: TCube): boolean;
    {проверяет все измерения осей на вхождение в базовый куб запроса}
    function CheckDimensions(out Msg: string): boolean;
    function ForwardCalculationPiece: string;
    (*function AxesDefinitionPiece: string;*)
    function FilterExpressionPiece: string;
    function CommonFiltersPiece: string;
    function DataCalculationPiece: string;
    function GetTotalsAxisSet: string; overload;
    function GetTotalsAxisSet(IgnoringColumns: boolean): string; overload;
    (*function GetAxesSet(Axis: TSheetAxisCollectionInterface): string;*)
    function PartialFiltersPiece: string;
    function QueryCubeMeasurePiece(Total: TSheetTotalInterface): string;
    function GetPartialFilterName(Index: integer): string;
    function LookupStatementForOuterTotal(Total: TSheetTotalInterface): string;
    function GetPFTuples: TStringList;
    function SetJoinClause(SetEnum: string;
      Axis: TSheetAxisCollectionInterface): string;
    function AxesDefinitionPieceNew: string;
    function MPClause(AxType: TAxisType): string;
    (*function AllMeasuresCondition: string;*)
  public
    constructor Create(SheetIntf: TSheetInterface);
    destructor Destroy; override;
    function GetCellsetQuery(out ProviderId: string): string;

    property IsLargeQuery: boolean read FIsLargeQuery write FIsLargeQuery;
  end;

{$B-}


{Нужен для элемента оси Axis под номером Ind вычислимый мембер отвечающий за
итоги}
function NeedSummaryMember(Axis: TSheetAxisCollectionInterface;
  Index: integer): boolean;
var
  AllMember: string;
begin
  result := false;
  if Assigned(Axis) then
    if (Index >= 0) and (Index < Axis.Count) then //нормальный индекс
      if Axis.SummaryOptions.Enabled then //есть у оси
        (* эта строка влияет не только на внутренние итоги по элементу, но
          и на стыковые, поэтому закомментировано.
        if Axis[Index].SummaryOptions.Enabled then //есть у этого элемнета*)
          //result := (Axis[Index].AllMember <> ''); //только, если есть алмембер
        begin
          { Если главный итог не выводится явно, то не нужно его запрашивать,
            т.к. наличие данных по нему влияет на скрытие пустых строк.}
          AllMember := Axis[Index].AllMember;
            if (AllMember <> '') then
              result := Axis.Broken or
                Assigned(Axis[Index].Members.selectSingleNode(
                'function_result/Members/Member[@name="' + AllMember + '"]'));
        end;
end;

{Имя вычислимого мембера для итогов в формате [изм].[хиер].[мем]}
function SummaryMemberName(AxisElem: TSheetAxisElementInterface): string;
begin
  result := '';
  {!могут быть конфликты имен с родными членами измерения}
  if Assigned(AxisElem) then
      result := AxisElem.FullDimensionNameMDX + '.' + MemberBrackets(stUsual);

end;


{MDX-выражение вычислимой меры для реальной меры, находящейся во внешнем
 (не в базовом) кубе
 Выражение примерно такого вида:

    'LookupCube
        (
            "[Внешний_куб]",
            "(" + MemberToStr([ОбщИзм1].CurrentMember) +
            ", " + MemberToStr([ОбщИзм2].CurrentMember) +
            ", [Measures].[ВнешМера])"
        )'

  Собственно это и есть ключевая технология вытаскивания всего листа
  одним запросом.
}
function HiddenTuplesFilterCondition(Axis: TSheetAxisCollectionInterface;
  QueryCube: TCube): string;
  {Текст условия, что мембер является алл-мембером}
  function ItIsAllMember(DimFullName: string; Negation: boolean): string;
  begin
    result := '';
    if DimFullName <> '' then
    begin
      result := DimFullName + '.CurrentMember.Level.Name';
      result := result + IIF(Negation, ' = ', ' <> ') + '"(All)"';
      result := '(' + result + ')';
    end;
  end;

var
  i, j: integer;
  CondForElem: string;
begin
//Все условие имеет такой вид
{
(not А) or (B and C and D)
and
(not B) or (C and D)
and
(not C) or D
 }

  result := '';
  for i := 0 to Axis.Count - 2 do //последний элемент не нужен
    if CheckDimension(QueryCube, Axis[i]) then
    begin
      CondForElem := '';
      { Бессмысленно накладывать условие на то, чего нет}
      if Axis[i].AllMember = '' then
        continue;

      for j := i + 1 to Axis.Count - 1 do
        if CheckDimension(QueryCube, Axis[j]) then
        begin
          if Axis[j].AllMember = '' then
            continue;
          AddTail(CondForElem, ' and ');
          CondForElem := CondForElem +
            ItIsAllMember(Axis[j].FullDimensionNameMDX, true);
        end;

      if CondForElem = '' then
        continue;

      CondForElem := ItIsAllMember(Axis[i].FullDimensionNameMDX, false)
          + ' or ' + TupleBrackets(CondForElem);

      AddTail(result, ' and ');
      result := result + TupleBrackets(CondForElem);
    end;
end;

{Оформление пересечения множеств с учетом настроек.}
function TCellsetQuery.SetJoinClause(SetEnum: string;
  Axis: TSheetAxisCollectionInterface): string;
var
  HTFCondition: string;
begin
  if (SetEnum <> '') and Assigned(Axis) then
  begin
    HTFCondition := HiddenTuplesFilterCondition(Axis, QueryCube);
    if not Axis.HideEmpty then //если пустышки не удаляются,
    begin
      result := SetBrackets(SetEnum); // тогда просто берем сет-констуктор
      if HTFCondition <> '' then
        result := 'Filter(' + result + ', ' + HTFCondition + ')';
    end
    else
    begin //если пустышки удаляются,
      if (Axis.Owner.AllowNECJ) //тогда зависит от настройки NonEmptyCrJ
        //за исключением, пустой оси столбцов (в столбцах только меры)
        and not ((Axis.AxisType = axColumn) and (Axis.Count = 0))
        and (Pos(' * ', SetEnum) > 0) then
      begin
        result := StringReplace(SetEnum, ' * ', ' , ', [rfReplaceAll]);
        result := ' nonemptycrossjoin(' + result + ')';
        if HTFCondition <> '' then
          result := 'Filter(' + result + ', ' + HTFCondition + ')';
      end
      else
      begin //запрещено использовать NECJ, но пустышки удаляются
        result := SetBrackets(SetEnum);
        if HTFCondition <> '' then
          result := 'Filter(' + result + ', ' + HTFCondition + ')';
        result := ' non empty ' + result;
      end
    end;
  end;
end;


{ TCellsetQuery }

{Предвычисления запроса}
function TCellsetQuery.ForwardCalculationPiece: string;
begin
  result := CommonFiltersPiece +
    PartialFiltersPiece + DataCalculationPiece;
end;

{Описание мер и фиктивных свободных}
function TCellsetQuery.DataCalculationPiece: string;
var
  i: integer;
  CurrTotal: TSheetTotalInterface;
begin
  result := '';
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    CurrTotal := SheetInterface.Totals[i];

    case CurrTotal.TotalType of
      wtMeasure, wtResult:
        begin
          if CurrTotal.Cube.Name = QueryCube.Name then //Мера из базового куба
            result := result +  QueryCubeMeasurePiece(CurrTotal)
          else //Мера не из базового куба, нужно делать лукап
          begin
            result := result +
                      ' MEMBER ' +
                      //алиас меры
                      StrAsMeasure(CurrTotal.Alias) +
                      ' AS ' +
                      LookupStatementForOuterTotal(CurrTotal);
          end;
        end;
      wtFree: //делаем заглушку для структуры
        begin
          result := result +
                    ' MEMBER ' + StrAsMeasure(CurrTotal.Alias) + ' AS ''null''';
        end;
      wtConst: //делаем заглушку для структуры
        begin
          result := result +
                    ' MEMBER ' + StrAsMeasure(CurrTotal.Alias) + ' AS ''""''';
        end;
    end;
  end;
end;

{Здесь слайсеры для фильтров.
 Передрано из TSheetMDXQuery}
function TCellsetQuery.CommonFiltersPiece: string;
var
  i: integer;
  SlicerName, SlicerValue: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    Filter := SheetInterface.Filters[i];

    //!!!!!!!!!
    //Здесь пока не проверяется, нужно ли включать данный фильтр (а надо проверять!!!!)
    if (* Filter.IsAffectsTotal(QueryTotals[0]) and *)
      Filter.IsMultiple
      and not Filter.IsPartial then //ЧАСТНЫЕ ФИЛЬТРЫ ЗДЕСЬ НЕ РАБОТАЮТ!
    begin
      SlicerName := Filter.FullDimensionNameMDX + '.[Slicer] ';
      SlicerValue := TupleBrackets(SetBrackets(Filter.MdxText));
      SlicerValue := StringReplace(SlicerValue, '''', '''' + '''', [rfReplaceAll]);
      result := result + ' MEMBER ' + SlicerName +
        ' AS ' + '''' + 'AGGREGATE' + SlicerValue + '''';
    end;
  end;
end;

{Возвращет множество для оси - дублируется (!!!) с TSheetMDXQuery}
(*function TCellsetQuery.GetAxesSet(Axis: TSheetAxisCollectionInterface): string;
var
  i: integer;
  AxisSet: string;
begin
  result := '';
  for i := 0 to Axis.Count - 1 do
    if CheckDimension(QueryCube, Axis[i]) then
    begin
      AxisSet := Axis[i].MdxText;
      if AxisSet = '' then
        continue;

      {Применяем настройки итогов
       !!! На самом деле, тут надо использовать "NeedGrandSummary",
        но, временно, grandsummary исполняют обязанности summary}
      if NeedSummaryMember(Axis, i) then
      begin
          AxisSet := AxisSet + ', ' + Axis[i].AllMember{SummaryMemberName(Axis[i])};
      end;

      AxisSet := SetBrackets(AxisSet);
      if Length(AxisSet) > 32000 then
      begin
        AxisSet := Axis[i].GetMDXLevelsSet;
        //IsLargeQuery := true; //!!!! не реализовано
      end;

      //сортировка
      AxisSet := 'Hierarchize(' + AxisSet + ')';

      //исключение дубликатов
      AxisSet := 'Distinct(' + AxisSet + ')';

      AddTail(result, ' * ');
      result := result + AxisSet;
    end;
end;*)

{Возвращает MDX-сет для показателй}
function TCellsetQuery.GetTotalsAxisSet: string;
var
  i: integer;
begin
  {Просто переписываем все алиасы показателей}
  result := '';
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    AddTail(result, ', ');
    result := result + StrAsMeasure(SheetInterface.Totals[i].Alias);
  end;

  if result <> '' then
    result := SetBrackets(result);
end;

function TCellsetQuery.GetTotalsAxisSet(IgnoringColumns: boolean): string;
var
  i: integer;
begin
  {Просто переписываем все алиасы показателей}
  result := '';
  for i := 0 to SheetInterface.Totals.Count - 1 do
    if (SheetInterface.Totals[i].IsIgnoredColumnAxis and  IgnoringColumns) or
       (not SheetInterface.Totals[i].IsIgnoredColumnAxis and not IgnoringColumns)then
    begin
      AddTail(result, ', ');
      result := result + StrAsMeasure(SheetInterface.Totals[i].Alias);
    end;

  if result <> '' then
    result := SetBrackets(result);
end;

{Запрос MP}
function TCellsetQuery.MPClause(AxType: TAxisType): string;
var
  i, j: integer;
  FullDimName: string;
  Axis: TSheetAxisCollectionInterface;
begin
  result := '';

  Axis := SheetInterface.GetAxis(AxType);
  for i := 0 to Axis.Count - 1 do
  if CheckDimension(QueryCube, Axis[i]) then
    begin
      FullDimName := Axis[i].FullDimensionNameMDX;

      for j := 0 to Axis[i].MemberProperties.Count - 1 do
        if Axis[i].MemberProperties[j].Checked then
        begin
          AddTail(result, ', ');
          result := result + FullDimName + '."' +
            Axis[i].MemberProperties[j].Name + '"';
        end;
    end;

  { начиная с аналайзиса 2005 свойство MEMBER_ORDINAL устарело.
    тем более, что судя по коду, мы его не используем...
  AddTail(result, ', ');
  result := ' properties ' + result + 'MEMBER_ORDINAL ';}
end;

{Определение осей}
(*function TCellsetQuery.AxesDefinitionPiece: string;
  {Множество столбцов таблицы}
  function ColAxDefinition: string;
  begin
    result := GetAxesSet(SheetInterface.Columns);

    if result = '' then //нет столбцов
      result := GetTotalsAxisSet
    else //есть столбцы - пересекаем с показателями
      result := result + ' * ' + GetTotalsAxisSet;

    if result <> '' then
    begin
      result := SetJoinClause(result, SheetInterface.Columns);
      result := result + MPClause(axColumn) +  'on Axis(0)';
    end
  end;

  {Множество строк таблицы}
  function RowAxDefinition: string;
  begin
    result := '';
    result := GetAxesSet(SheetInterface.Rows);
    if result <> '' then
    begin
      result := SetJoinClause(result, SheetInterface.Rows);
      result := result + MPClause(axRow) +  'on Axis(1)';
    end
  end;

begin
  result := RowAxDefinition;
  AddHead(', ', result);
  result := ColAxDefinition + result;
end;*)

{!! Дублирование!! По сути полная калька с TSheetMDXQuery.SetDeclarationPiece.
  Впоследствии крайне желательно переписать оба эти модуля, выделив
  абстрактный класс TBasicMDXQuery.}
function TCellsetQuery.AxesDefinitionPieceNew: string;

  procedure FillAxesList(AxisCollection: TSheetAxisCollectionInterface;
    var List, OrderBySize: TStringList);
  var
    i, j: integer;
    Added: boolean;
  begin
    for i := 0 to AxisCollection.Count - 1 do
    begin
      if CheckDimension(QueryCube, AxisCollection[i]) then
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

  function MakeResult(AxisType: TAxisType; var List: TStringList): string;
  var
    i: integer;
    AxisSet: string;
  begin
    result := '';
    if List.Count = 0 then
      if AxisType = axRow then
        exit;

    for i := 0 to List.Count - 1 do
      if List[i] <> '' then
      begin
        AxisSet := SetBrackets(List[i]);
        AxisSet := 'Hierarchize(' + AxisSet + ')';
        AxisSet := 'Distinct(' + AxisSet + ')';

        AddTail(result, ' * ');
        result := result + AxisSet;
      end;

    if AxisType = axColumn then
      if result = '' then //нет столбцов
        result := GetTotalsAxisSet
      else //есть столбцы - пересекаем с показателями
        result := result + ' * ' + GetTotalsAxisSet;

    result := SetJoinClause(result, SheetInterface.GetAxis(AxisType));
    result := result + MPClause(AxisType);
    result := result + Format(' on Axis(%d)', [IIF(AxisType = axRow, 1, 0)]);
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
    if (RowsBySize.Count > 0) or (ColumnsBySize.Count > 0) then
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

    result := MakeResult(axRow, RowsList);
    AddHead(', ', result);
    result := MakeResult(axColumn, ColumnsList) + result;
  finally
    FreeStringList(RowsList);
    FreeStringList(ColumnsList);
    FreeStringList(RowsBySize);
    FreeStringList(ColumnsBySize);
  end;
end;

{Определение фильтров}
function TCellsetQuery.FilterExpressionPiece: string;
var
  i: integer;
  Slicer: string;
  Filter: TSheetFilterInterface;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    Filter := SheetInterface.Filters[i];

    if not Filter.IsPartial then
      if CheckDimension(QueryCube, Filter) then
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

function TCellsetQuery.CheckDimensions(out Msg: string): boolean;

  procedure CheckAxisDimensions(Axis: TSheetAxisCollectionInterface);
  var
    i: integer;
  begin
    for i := 0 to Axis.Count - 1 do
      if not CheckDimension(QueryCube, Axis[i]) then
      begin
        AddTail(Msg, ', ');
        Msg := Msg + '"' + Axis[i].FullDimensionName + '"';
      end;
  end;

begin
  result := true;
  Msg := '';
  CheckAxisDimensions(SheetInterface.Rows);
  CheckAxisDimensions(SheetInterface.Columns);
  if Msg <> '' then
  begin
    result := false;
    Msg := 'Невозможно обработать запрос в серверном режиме';// + Msg;
  end;
end;

constructor TCellsetQuery.Create(SheetIntf: TSheetInterface);
begin
  SheetInterface := SheetIntf;
end;

function TCellsetQuery.GetCellsetQuery(out ProviderId: string): string;
begin
  result := '';
  if not Assigned(SheetInterface) then
    exit;

  {Получаем куб, по которому будем стрить запрос - базовый куб}
  QueryCube := GetQueryCube;
  if not Assigned(QueryCube) then
    exit;

  if not CheckDimensions(result) then
    exit;

  ProviderId := QueryCube.ProviderId;

  {Абстрактная сборка запроса - делегирование частей}
  {Объявления вычислимых мер}
  WithClause := ForwardCalculationPiece;
  AddHead('WITH ', WithClause);

  {Оси запроса}
  SelectClause := AxesDefinitionPieceNew;
  AddHead(' SELECT ', SelectClause);

  {Куб}
  FromClause := MemberBrackets(QueryCube.Name);
  AddHead(' FROM ', FromClause);

  {Фильтр}
  WhereClause := FilterExpressionPiece;
  AddHead(' WHERE ', WhereClause);

  {altogether}
  result := WithClause + SelectClause + FromClause + WhereClause;
end;

function TCellsetQuery.GetQueryCube: TCube;
var
  i: integer;
  {Два кандидата. Первый плохонький, второй нормальный. "Плохонький" убъет некоторые жлементы осей}
  CandidateByMeasure: TCube;  //Просто куб по которому есть меры
  CandidateByAxes: TCube; //Предыдущее + в этом кубе есть все измерения осей и фильтров
  TotalsRecord: integer; //кол-во мер, которое обслужит текущий кандидат
  CurCube: TCube; //текущий куб
  CurTotalsCount: integer; //кол-во мер, обслуживаемое текущим проверяемым
  CurIsFullDimensionality: boolean; //текущий куб полнораскладываемый
begin
  CandidateByMeasure := nil;
  CandidateByAxes := nil;
  CurCube := nil;
  TotalsRecord := 0;

  for i := 0 to SheetInterface.Totals.Count - 1 do
    if (SheetInterface.Totals[i].TotalType in [wtMeasure, wtResult]) then
    begin //как минимум кандидат первого уровня
      CurCube := SheetInterface.Totals[i].Cube;
      CurTotalsCount := GetServedTotalsCount(CurCube);
      CurIsFullDimensionality := IsFullDimensionality(CurCube);

      {Теперь условия побития рекорда}
      if //если полнораскладной и побил рекорд по мерам
        (CurIsFullDimensionality and (CurTotalsCount > TotalsRecord))
        or //либо
        //если полнораскладной, а до этого таких не было
        (CurIsFullDimensionality and not Assigned(CandidateByAxes))
        or //либо
        //не раскладывается, но побил рекорд по мерам (а раскладываемх еще не было)
        ((CurTotalsCount > TotalsRecord) and not Assigned(CandidateByAxes))
      then
      begin
        //запоминаем как надидата
        if CurIsFullDimensionality then
          CandidateByAxes := CurCube
        else
          CandidateByMeasure := CurCube;

        //запоминаем его счетчик мер
        TotalsRecord := CurTotalsCount;
      end;
    end;


  result := CurCube;

  {подводим итоги поиска оптимального куба запроса}
  if Assigned(CandidateByAxes) then
    result := CandidateByAxes
  else //берем что есть (результат рефреша теперь не гарантирует никто
    if Assigned(CandidateByMeasure) then
      result := CandidateByMeasure;
end;

function TCellsetQuery.GetServedTotalsCount(Candidate: TCube): integer;
var
  i: integer;
begin
  result := 0;
  with SheetInterface do
  begin
    {Смотрим в строках}
    for i := 0 to Rows.Count - 1 do
      if CheckDimension(Candidate, Rows[i]) then
        inc(result);

    {Смотрим в столбцах}
    for i := 0 to Columns.Count - 1 do
      if CheckDimension(Candidate, Columns[i]) then
        inc(result);

    {Смотрим в общих фильтрах}
    for i := 0 to Filters.Count - 1 do
      if not Filters[i].IsPartial then
        if CheckDimension(Candidate, Filters[i]) then
          inc(result);
  end;
end;

{В этом кубе есть все измерения листа (частные фильтры игнорируем) }
function TCellsetQuery.IsFullDimensionality(Candidate: TCube): boolean;
var
  i: integer;
begin
  result := false;
  with SheetInterface do
  begin
    {Смотрим в строках}
    for i := 0 to Rows.Count - 1 do
      if not CheckDimension(Candidate, Rows[i]) then
        exit;

    {Смотрим в столбцах}
    for i := 0 to Columns.Count - 1 do
      if not CheckDimension(Candidate, Columns[i]) then
        exit;

    {Смотрим в общих фильтрах}
    for i := 0 to Filters.Count - 1 do
      if not Filters[i].IsPartial then
        if not CheckDimension(Candidate, Filters[i]) then
          exit;
  end;

  result := true;
end;

function GetCellsetQuery(SheetInterface: TSheetInterface; out ProviderId: string): string;
var
  CellsetQuery: TCellsetQuery;
begin
  CellsetQuery := TCellsetQuery.Create(SheetInterface);
  try
    result := CellsetQuery.GetCellsetQuery(ProviderId);
  finally
    CellsetQuery.Free;
  end;
end;

function TCellsetQuery.PartialFiltersPiece: string;
var
  i, j: integer;
  FilterName, FilterValue: string;
  IsAnyTotalFromQueryCube: boolean;
  TotalIndex: integer;
begin
  result := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
    if SheetInterface.Filters[i].IsPartial then
      if CheckDimension(QueryCube, SheetInterface.Filters[i]) then
      begin
        IsAnyTotalFromQueryCube := false;
        for j := 0 to SheetInterface.Filters[i].Scope.Count - 1 do
        begin
          TotalIndex := SheetInterface.Totals.FindById(SheetInterface.Filters[i].Scope[j]);
          if SheetInterface.Totals[TotalIndex].Cube <> QueryCube then
            continue;
          IsAnyTotalFromQueryCube := true;
          break;
        end;
        if not IsAnyTotalFromQueryCube then
          continue;
        FilterName := GetPartialFilterName(i);
        FilterValue := SetBrackets(SheetInterface.Filters[i].MdxText);
        result := result + Format(' MEMBER %s AS ' + '''AGGREGATE(%s)'''  ,
        [FilterName, FilterValue]);
      end;
end;

function TCellsetQuery.QueryCubeMeasurePiece(
  Total: TSheetTotalInterface): string;
var
  MeasureValue: string;
  i: integer;
begin
  MeasureValue := '';
  for i := 0 to SheetInterface.Filters.Count - 1 do
    if SheetInterface.Filters[i].IsPartial then
      if Total.IsFilteredBy(SheetInterface.Filters[i]) then
      begin
        AddTail(MeasureValue, ', ');
        MeasureValue := MeasureValue + GetPartialFilterName(i);
      end;
  if MeasureValue <> '' then
    MeasureValue := TupleBrackets(StrAsMeasure(Total.MeasureName) + ', ' + MeasureValue)
  else
    MeasureValue := StrAsMeasure(Total.MeasureName);

  result := Format(' MEMBER %s AS ' + ''' %s ''' + '   , SolveOrder = 100',
    [StrAsMeasure(Total.Alias), MeasureValue]);
end;

function TCellsetQuery.GetPartialFilterName(Index: integer): string;
begin
  with SheetInterface.Filters[Index] do
    result := FullDimensionNameMDX + '.[' + Alias + ']';
end;

function TCellsetQuery.LookupStatementForOuterTotal(Total: TSheetTotalInterface): string;

  {Подцепляет выражение для фильтра функции "Lookup", отвечающее за одно общее
   измерение. Параметрами передается собираемая строка и элемент листа,
   измерение которого надо обработать}
  procedure ConcatDimExpr(var Src: string; DimElem: TSheetDimension);
  begin
    {Если измерение входит и в основной и во внешний куб, тогда подцепляем}
    if CheckDimension(QueryCube, DimElem)
      and CheckDimension(Total.Cube, DimElem) then
    begin
      AddTail(Src, ' + ", " + ');
      Src := Src + 'MemberToStr(' + DimElem.FullDimensionNameMDX + '.CurrentMember)';
    end;
  end;
var
  i: integer;
  CommonDimsExpr: string; //Выражение для общих измерений (параметр)
  PFTuples: TStringList;
  tmpResult, Pattern: string;
begin
  result := '';
  CommonDimsExpr := '';

  if not Assigned(OuterTotalPF) then
    OuterTotalPF := TStringList.Create;
  OuterTotalPF.Clear;

  {Собираем MDX- выражение описывающее общие измерения}
  with SheetInterface do
  begin
    //Со строк
    for i := 0 to Rows.Count - 1 do
      ConcatDimExpr(CommonDimsExpr, Rows[i]);
    //Собираем из столбцов
    for i := 0 to Columns.Count - 1 do
      ConcatDimExpr(CommonDimsExpr, Columns[i]);
    //Собираем из фильтров (частные не учитываем, только общие)
    for i := 0 to Filters.Count - 1 do
      if not Filters[i].IsPartial and CheckDimension(QueryCube, Filters[i]) then
        ConcatDimExpr(CommonDimsExpr, Filters[i])
      else
        if Total.IsFilteredBy(SheetInterface.Filters[i]) then
          OuterTotalPF.Add(
            GetPartialFilterName(i) + '=' + Filters[i].MdxText);
  end;

  try
    PFTuples := GetPFTuples;

    //собираем параметры лукапа вместе: <куб>, <общие измерения>, <мера>, <частные фильтры>
    if not Assigned(PFTuples) or (PFTuples.Count = 0) then
      if CommonDimsExpr <> '' then
        result := '"' + MemberBrackets(Total.Cube.Name) + '", ' +
                  '"(" + ' + CommonDimsExpr + ' + ' +
                  '", ' + StrAsMeasure(Total.MeasureName) + ')"'
      else
        result := '"' + MemberBrackets(Total.Cube.Name) + '", ' +
                  '"(' + StrAsMeasure(Total.MeasureName) + ')"'
    else
    begin
      Pattern := StrAsMeasure(Total.MeasureName) + ', %s';
      for i := 0 to PFTuples.Count - 1 do
      begin
        tmpResult := Format(Pattern, [PFTuples[i]]);
        if CommonDimsExpr <> '' then
          tmpResult :='(" + ' + CommonDimsExpr + ' + ' +
                  '", ' + tmpResult +')'
        else
          tmpResult :='(' + tmpResult + ')';
        AddTail(result, ' , ');
        result := result + tmpResult;
      end;
      result := '"AGGREGATE(' + SetBrackets(result) + ')"';
      result := '"' + MemberBrackets(Total.Cube.Name) + '", ' + result;
    end;
  finally
    FreeStringList(PFTuples);
  end;

  //и наконец, берем саму функцию
  result := '''LookupCube(' + result + ')''';
end;

destructor TCellsetQuery.Destroy;
begin
  inherited;
  FreeStringList(OuterTotalPF);
end;

function TCellsetQuery.GetPFTuples: TStringList;

  procedure GetPFList(Index: integer; var List: TStringList);
  var
    tmpString, Part: string;
  begin
    if Index >= OuterTotalPF.Count then
      exit; 
    tmpString := OuterTotalPF.Values[OuterTotalPF.Names[Index]];
    tmpString := StringReplace(tmpString, '],[', ']$$$[', [rfReplaceAll]);
    tmpString := StringReplace(tmpString, 'DATAMEMBER,[', 'DATAMEMBER$$$[', [rfReplaceAll]);
    if not Assigned(List) then
      List := TStringList.Create;
    List.Clear;
    while tmpString <> '' do
    begin
      Part := CutPart(tmpString, '$$$');
      List.Add(Part);
    end;
  end;

var
  PFIndex: integer;
  List1, List2: TStringList;
begin
  List1 := nil;
  List2 := nil;
  GetPFList(0, List1);
  for PFIndex := 1 to OuterTotalPF.Count - 1 do
  begin
    GetPFList(PFIndex, List2);
    List1.Assign(StringListCrossJoin(List1, List2, ', '));
  end;
  result := List1;
end;

(*function TCellsetQuery.AllMeasuresCondition: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to SheetInterface.Totals.Count - 1 do
    if (SheetInterface.Totals[i].TotalType in [wtMeasure, wtResult]) then
    begin
      AddTail(result, ' AND ');
      result := result + Format('IsEmpty(%s)', [StrAsMeasure(SheetInterface.Totals[i].Alias)]);
    end;
  result := Format(' NOT (%s)', [result]);
end;*)

end.



