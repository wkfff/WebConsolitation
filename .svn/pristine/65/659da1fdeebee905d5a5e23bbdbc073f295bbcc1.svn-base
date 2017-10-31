{
  Этот потомок TSheetInterface умеет размещать таблицу по Cellset-у
  Это отдельный режим построения таблицы, когда она обрабатывается одним
  MDX-запросом.
  т.е, на текущий момент, этот класс обслуживает только вариант, когда
  TableProcessingMode = tpmHuge.
  Если это не так, то класс делегирует обработку запроса своему предку

  !!!!! Предупреждение!
  Вещь эта некоторое вермя будет крайне нестабильная. В связи с этим,
  В этом классе будет некоторе количество кода, дублирующегося с предком.
  (Разметка, раскраска и все такое).
  Это сделано осознанно, что бы изолировать этот нестабильный режим от нормального.
  Когда режим гигантских таблиц стабилизируется, то нужно рефакнуть будет кое-чего

}

unit uSheetMaperCellset;

interface

uses
  Windows, Classes, SysUtils, MSXML2_TLB, uXMLUtils, ExcelXP, PlaningProvider_TLB,
  uXMLCatalog, uFMExcelAddinConst, PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils,
  uSheetObjectModel, uSheetMaper, uExcelUtils, uGlobalPlaningConst,
  uSheetSizer, uSheetMDXQueryForCellset;

type
  { лист, который умеет собирать данные с листа }
  TSheetMaperCellset = class(TSheetMaper)
  private
    {Поля под счетчики для прогресса в форме индикации. Здесь это сделано
     отдельными полями, из-за того что алгоритмы рекурсивные.}
    FProgressMaxCount: integer;
    FProgressCurCount: integer;

    {кэш имен мембер пропертиз}
    MPCache: TStringList;

    { Используются в случае сломанной иерархии осей.
      Каждый массив содержит информацию о мемберах, подлежащих размещению
      в виде произведения индекса кортежа на индекс мембера в нем.
      Порядок мемберов соответствует тому, как они будут размещены в листе.}
    RowsProcessRoute, ColumnsProcessRoute: TIntegerArray;

    {Установить верхнюю границу прогресс-бара в полное число картежей
     (в обоих осях), а нижнюю в 0}
    procedure InitPBbyBothAxes(DOM: IXMLDOMDocument2);

    {Инкрементировать прогресс бар}
    procedure IncPB;

    {Маппинг "гигантской" таблицы через селсет}
    function MapCellsetTable: boolean;

    {Запрашивает с сервера, учитвает собранные, возвращает полный набор для таблицы}
    function QueryCellsetData: IXMLDOMDocument2;

    {Рисование таблицы по данным селсета}
    procedure MapCellsetDOM(DOM: IXMLDOMDocument2);

    {Размещение MP оси}
    procedure MapAxisMP(DOM: IXMLDOMDocument2; AxType: TAxisType);

    {Делает иерархию явной.
     Иеарархия в исходном доме находится в неочевидном состоянии.
     Ось представляет из себя плоский список кортежей. В каждом картеже мемберов
     столько, сколько элементов в оси. Одному элементу соответствует ровно один
     мембер (это может быть лист, может быть не лист).
     Есть две вещи:
         1) Информация о уровне в конкретном мембере
         2) Кортежи отсортированы. (Определяется запросом).
     На основании этого, преобразуем исходные картежи в форму, более удобную
     для размещения (но, кстати, не корректную с точки зрения MDX).
     В результате, сделаем такую же структуру, только мембер в каретже будет
     соответствовать на элементу, а уровню элементу.
     (Делается путем добавления недостающих узлов мемберов.)}
    procedure ManifestHierarchy(DOM: IXMLDOMDocument2);
    {Делает явную иерархию для соотв. оси}
    procedure ManifestHierarchyForAxis(DOM: IXMLDOMDocument2; AxType: TAxisType);


    {Картежи, которые требуется скрыть помечаются аттрибутом hidden="true".
     К таким картежам (узлам) относятся узлы, которые детализируют итоги -
     этого делать не надо}
    procedure MarkHiddenTuples(DOM: IXMLDOMDocument2);
    {Помечает "скрываемые" кортежи для соответсвующей оси.}
    procedure MarkHiddenTuplesForAxis(DOM: IXMLDOMDocument2; AxType: TAxisType);
    {Размещение свободных показателей
     Показатели из базы уже должны быть размещены (там проставляются координаты)}
    procedure MapFreeData(DOM: IXMLDOMDocument2);

    {Запись значения в ячейку - проверить на дубликат}
    procedure WriteValue(RInd, CInd: integer; Value: string);

    {Размещение данных}
    procedure MapDataCells(DataDOM: IXMLDOMDocument2;
      StartRInd, StartCInd: integer);

    {Разместить диапазон картежей из набора.
      Параметры:
      TuplesNL - набор картежей
      AxType - тип оси (строка, столбец)
      MemberNum - порядковый номер кортежа
      LBnd, UBnd  - верхняя и нижная граница диапазона (по номерам)
      CellRInd, CellCInd - координаты ячейки для начала размещения в листе
    }
    procedure MapTupleRange(TuplesNL: IXMLDOMNodeList; AxType: TAxisType;
      MemberNum: integer; LBnd, UBnd: integer; CellRInd, CellCInd: integer);

    { Случай полностью разрушенной иерархии оси требует совершенно отдельной
      обработки. Главной особенностью является то, что один и тот же кортеж
      может соответствовать нескольким строкам (столбцам) таблицы.}
    procedure MapBrokenAxis(DOM: IXMLDOMDocument2; AxisType: TAxisType);

    procedure HideDataMemberTuples(DOM: IXMLDOMDocument2; AxisType: TAxisType);

    {В режиме скрытия пустых строк/столбцов запрос может вернуть данные, которые
      не попадут в лист. Как правило, это общие итоги по одной оси в разрезе другой.
      Ячейка в секции данных в таком случае ссылается на нормальный кортеж
      по одной оси и скрытый по другой. Чтобы при этом в лист не выводились
      пустые строки/столбцы (ведь данные-то запрос для них вернул, а в листе их
      нет и не должно быть) надо проверить каждый такой кортеж на наличие у него
      нескрытых "партнеров" и, если таковых нет, скрывать его самого.}
    procedure HideTuplesByCounterparts(Dom: IXMLDOMDocument2; AxisType: TAxisType);

    {просматривает инфо-секцию документа и возвращает используемое имя МР,
      которое может не совпадать с исходным (как правило, если в разных
      измерениях есть МР с одинаковыми именами)}
    function GetAlterMPName(Dom: IXMLDOMDocument2; AxisType: TAxisType;
      AxisElement: TSheetAxisElementInterface; MPName: string): string;

    {при изменении порядка следования кортежей в оси нужно переопределить
      координаты для соответствующих ячеек с данными}
    procedure ShiftDataCells(Dom: IXMLDOMDocument2;AxisType: TAxisType;
      FromIndex, ToIndex: integer);

    {явно прописывает координаты ячейкам данных}
    procedure ExtractCellsCoords(Dom: IXMLDOMDocument2);

    {перемещает кортежи итогов в соответствии с настройками размещения}
    procedure ShakeSummaries(Dom: IXMLDOMDocument2; AxisType: TAxisType);

    (*function GetLevelByMemberIndex(AxisType: TAxisType;
      MemberIndex: integer): TSheetLevelInterface;*)

    function GetLevelByMember(Member: IXMLDOMNode; AxisType: TAxisType): TSheetLevelInterface;

    {}
    procedure HideDisabledSummaries(Dom: IXMLDOMDocument2; AxisType: TAxisType);
    function TupleHasSummary(Tuple: IXMLDOMNode): boolean;
    { Помечает серым цветом ячейки свободных и результатов, которые
      непригодны для обратной записи}
    procedure MarkResultsGrey(Dom: IXMLDOMDocument2);
    function GetTotalByColumnTuple(Tuple: IXMLDOMNode): TSheetTotalInterface;
    procedure MarkMember(ERange: ExcelRange; Member: IXMLDOMNode;
      MemberIndex: integer; AxisType: TAxisType);
    function GetColumnAxisNode(SrcDOM: IXMLDOMDocument2): IXMLDOMNode;
    function GetRowAxisNode(SrcDOM: IXMLDOMDocument2): IXMLDOMNode;
    function IsNodeListSubsetCorrect(NL: IXMLDOMNodeList; LBnd,
      UBnd: integer): boolean;
    procedure ChangeMemberCaption(MemberNode: IXMLDOMNode;
      Condition: boolean; TrueValue, FalseValue: string);
    function CheckForSingleLine(MemberNode: IXMLDOMNode): boolean;
    procedure CloneEmptyMembers(MemberNode: IXMLDOMNode; Count: integer;
      Level: TSheetLevelInterface);
    procedure CopyMemberFromParentTuple(TupleNode: IXMLDOMNode;
      MemberIndex: integer);
    function FullLevelNameMDX(SheetLevel: TSheetLevelInterface): string;
    function GetParentUName(Member: IXMLDOMNode): string;
    function IsItMeasure(Node: IXMLDOMNode): boolean;
    procedure MarkAsSummary(MemberNode: IXMLDOMNode);
    procedure MarkTupleHidden(MemberNode: IXMLDOMNode);
    function MemberByNum(TuplesNL: IXMLDOMNodeList; i,
      j: integer): IXMLDOMNode;
    function MemberForThatLevel(TupleNode: IXMLDOMNode;
      LevelNameMDX: string): IXMLDOMNode;
    procedure UnmarkSingleLineSummaries(Dom: IXMLDOMDocument2);
    {при работе с несколькими многомерками серверный режим невозможен}
    function AllFromSameProvider: boolean;
  protected
    { Приходится перекрывать в связи с возможностью индивидуального скрытия
      колонок показателей.}
    procedure MapTotalTitles(Dom: IXMLDOMDocument2);
    {Перекрытие стандартного мапинга таблицы, определенного в шит-интерфейсе}
    function MapTable: boolean; override;
    { Собирает сведения о диапазонах показателей в таблице, доступных для записи}
    procedure GetWritableTotalsInfo;
  public
  end;

implementation

const
  attrRowIndex = 'RowIndex';
  attrColumnIndex = 'ColumnIndex';
  attrIsGrey = 'IsGrey';
  attrRowCoords = 'RowCoords';
  attrColumnCoords  = 'ColumnCoords';

{=============================Утили============================================}
{Возвращает узел столбцов}
function TSheetMaperCellset.GetColumnAxisNode(SrcDOM: IXMLDOMDocument2): IXMLDOMNode;
begin
  if Assigned(SrcDOM) then
    result := SrcDOM.selectSingleNode('root/Axes/Axis[@name="Axis0"]/Tuples');
end;

{Возвращает узел строк}
function TSheetMaperCellset.GetRowAxisNode(SrcDOM: IXMLDOMDocument2): IXMLDOMNode;
begin
  if Assigned(SrcDOM) then
    result := SrcDOM.selectSingleNode('root/Axes/Axis[@name="Axis1"]/Tuples');
end;

{Передается набор узлов и два индекса в этом наборе, задающие диапазон (подмножество)
 узлов в нем.
 Функция проверяет корректность. Всли возвращает "true" можно смело перебирать
 узлы подмножества по этим индексам без дополнительных проверок. }
function TSheetMaperCellset.IsNodeListSubsetCorrect(NL: IXMLDOMNodeList; LBnd, UBnd: integer): boolean;
var
  TuplesCount: integer;
begin
  result := false;
  TuplesCount := NL.length;

  if Assigned(NL) then //есть набор
    if TuplesCount > 0 then //он не пустой
      if (LBnd >= 0) and (LBnd <= TuplesCount) then //нижний индекс внутри
        if (UBnd >= 0) and (LBnd <= TuplesCount) then //верхний индекс внутри
          if LBnd <= UBnd then //не "мнимый" интервал
            result := true;
end;


{Возвращает j-ый мембер i-ого кортежа из набора}
function TSheetMaperCellset.MemberByNum(TuplesNL: IXMLDOMNodeList; i, j: integer): IXMLDOMNode;
var
  MemsNL: IXMLDOMNodeList;
begin
  result := nil;
  if Assigned(TuplesNL) then
    if TuplesNL.length > i then
    begin
      MemsNL := TuplesNL[i].selectNodes('Member');
      if MemsNL.length > j then
        result := MemsNL[j];
    end;
end;

{помечает кортеж, к которому относится мембер, как скрытый}
procedure TSheetMaperCellset.MarkTupleHidden(MemberNode: IXMLDOMNode);
begin
  (MemberNode.parentNode as IXMLDOMElement).setAttribute(attrHiddenTuple, VarNull)
end;

function IsTupleHidden(Tuple: IXMLDOMNode): boolean;
begin
  result := Assigned(Tuple.attributes.getNamedItem(attrHiddenTuple));
end;

{Устанавливает мембер капшна в зависимости от условия}
procedure TSheetMaperCellset.ChangeMemberCaption(MemberNode: IXMLDOMNode; Condition: boolean;
  TrueValue, FalseValue: string);
var
  CaptionNode: IXMLDOMNode;
begin
  if not Assigned(MemberNode) then
    exit;

  CaptionNode := MemberNode.selectSingleNode('Caption');
  if Assigned(CaptionNode) then
  begin
    if Condition then
      CaptionNode.text := TrueValue
    else
      CaptionNode.text := FalseValue;
  end;
end;

procedure TSheetMaperCellset.MarkAsSummary(MemberNode: IXMLDOMNode);
var
  SiblingNode: IXMLDOMNode;
begin
  SiblingNode := MemberNode.previousSibling;
  while Assigned(SiblingNode) do
  begin
    if IsItSummary(SiblingNode) then
      exit;
    SiblingNode := SiblingNode.previousSibling;
  end;

  (MemberNode as IXMLDOMElement).setAttribute(attrIsItSummary, VarNull);
  {Ну не имеют смысла итоги на самом верхнем уровне...
    Должен же быть собственно элемент, по которому подводится итог!}
  if not Assigned(MemberNode.previousSibling) then
    MarkTupleHidden(MemberNode);
end;

{Проверка на единственность кортежа с таким юникнеймом на заданном уровне.
  Если кортеж и впрямь один, то не надо помечать его мембер как итог -
  ведь подчиненных-то элементов нет вовсе. А если пометить, то при отключении
  итогов на этом уровне пропадет вся строка...}
function TSheetMaperCellset.CheckForSingleLine(MemberNode: IXMLDOMNode): boolean;
var
  MemberIndex: integer;
  NextTupleNode, SiblingNode, SiblingToCompare: IXMLDOMNode;
  UName, UNameToCompare: string;
begin
  result := true;
  NextTupleNode := MemberNode.parentNode;
  repeat
    NextTupleNode := NextTupleNode.nextSibling;
    if not Assigned(NextTupleNode) then
      exit;
  until not IsTupleHidden(NextTupleNode);
  SiblingNode := MemberNode.previousSibling;
  if not Assigned(SiblingNode) then
  begin
    result := false;
    exit;
  end;
  UName := MemberUName(SiblingNode);
  MemberIndex := -1;
  while Assigned(SiblingNode) do
  begin
    inc(MemberIndex);
    SiblingNode := SiblingNode.previousSibling;
  end;
  SiblingToCompare := NextTupleNode.childNodes[MemberIndex];
  UNameToCompare := MemberUName(SiblingToCompare);
  result := UName <> UNameToCompare;
end;

procedure TSheetMaperCellset.UnmarkSingleLineSummaries(Dom: IXMLDOMDocument2);
var
  Summaries: IXMLDOMNodeList;
  i: integer;
  XPath: string;
begin
  XPath := Format('root/Axes/Axis/Tuples/Tuple[not(@%s)]/Member[@%s]',
    [attrHiddenTuple, attrIsItSummary]);
  Summaries := Dom.selectNodes(XPath);
  for i := 0 to Summaries.length - 1 do
    if CheckForSingleLine(Summaries[i]) then
    begin
      //ChangeMemberCaption(Summaries[i], true, '', '');
      Summaries[i].attributes.removeNamedItem(attrIsItSummary);
    end;
end;

{!!!!!!!!!! Нужно вынеси и сделать методом объекта TSheetLevel.
 Кстати, возможно стоит не вычислять а хранить}
function TSheetMaperCellset.FullLevelNameMDX(SheetLevel: TSheetLevelInterface): string;
begin
  try
    result := SheetLevel.ParentCollection.Owner.FullDimensionNameMDX +
      '.' + MemberBrackets(SheetLevel.Name);
  except
    result := '';
  end;
end;

{Параметром передается узел кортежа в документе данных селсета.
 И объект - уровень измерения. Функция возвращает
 мембер соответствующий этому уровню}
function TSheetMaperCellset.MemberForThatLevel(TupleNode: IXMLDOMNode;
  LevelNameMDX: string): IXMLDOMNode;
var
  XPath: string;
begin
  result := nil;
  if not Assigned(TupleNode) then
    exit;

  try
    XPath := 'Member[LName="%s"]';
    XPath := Format(XPath, [LevelNameMDX]);
    result := TupleNode.selectSingleNode(XPath);
  except
  end;
end;

function TSheetMaperCellset.IsItMeasure(Node: IXMLDOMNode): boolean;
begin
  if Assigned(Node) then
    result := MemberLName(Node) = '[Measures].[MeasuresLevel]'
  else
    result := false;
end;

{Передается узел-мембер в кортеже. и колличество требуемых пустышек-клонов.
 Пустышки клоны вставляются после этого переданного мембера и отличаются от
 него отстутсвием кэпшна}
procedure TSheetMaperCellset.CloneEmptyMembers(MemberNode: IXMLDOMNode; Count: integer;
  Level: TSheetLevelInterface);
var
  i: integer;
  Clone, SiblingNode, tmpNode: IXMLDOMNode;
  IsMemberLeaf: boolean;
  SummaryCaption: string;
begin
  if not Assigned(MemberNode) then
    exit;
  IsMemberLeaf := GetBoolAttr(MemberNode, attrMemberLeaf, false);
  SiblingNode := MemberNode.nextSibling;
  for i := 1 to Count do
  begin
    Clone := MemberNode.cloneNode(true);

    { Изменяем капшн.
      В первой пустышке пишем "Итоги", в остальных пусто.
      Если мембер помечен как листовой, то его пустышки должны быть
      просто пустышками, не саммари. Это лист в несбалансированной иерархии.}
    SummaryCaption := Level.SummaryOptions.GetCaption(MemberCaption(MemberNode));

    ChangeMemberCaption(Clone,
      (i = 1) and (MemberCaption(Clone) <> '') and not IsMemberLeaf, SummaryCaption, '');

    {Меняем юник нэйм. Что бы пустышка с нормальным мембером не слилась
     случайно при размещении}
    tmpNode := Clone.selectSingleNode('UName');
    if Assigned(tmpNode) then
      tmpNode.text := 'DUMMY';

    {Вставляем в документ}
    if Assigned(SiblingNode) then
    begin
      MemberNode.parentNode.insertBefore(Clone, SiblingNode);
//        SiblingNode := SiblingNode.nextSibling;
    end
    else
      MemberNode.parentNode.appendChild(Clone);
    if (i = 1) and not IsMemberLeaf then
      MarkAsSummary(Clone);
  end;

    {Смотрим последнюю сестру на которой остановились. Если она не по AllМемберу,
    тогда это лишняя детализация итога. Весь кортеж помечаем как скрытый}
    if Assigned(SiblingNode) and not IsMemberLeaf then
    begin
      if not MemberFromAllLevel(SiblingNode) and not IsItMeasure(SiblingNode) then
        MarkTupleHidden(SiblingNode)
      else
        ChangeMemberCaption(SiblingNode, true, '', ''); //очищаем капшн
    end;
end;

{
  Копируем мембер номер "MemberIndex" в кортеж "TupleNode" из соседнего
  кортежа сверху. Причем старается сделать его таким-же по счету.
  Примичания:
    Если соседей сверху нет - ничего не делает.
    Если невозможно скопировать на то же место (таким-же по счету),
    тогда скопируется последним
}
procedure TSheetMaperCellset.CopyMemberFromParentTuple(TupleNode: IXMLDOMNode; MemberIndex: integer);
var
  Clone, PrevTuple: IXMLDOMNode;
begin
  if not Assigned(TupleNode) or (MemberIndex < 0) then
    exit; //некорректные входные

  PrevTuple := TupleNode.previousSibling;
  if not Assigned(PrevTuple) then
    exit; //неоткуда копировать.

  //берем копию
  Clone := PrevTuple.childNodes[MemberIndex].cloneNode(true);

  {Вставляем в наш кортеж}
  if TupleNode.childNodes.length < MemberIndex then
    TupleNode.appendChild(Clone)
  else
    TupleNode.insertBefore(Clone, TupleNode.childNodes[MemberIndex]);
end;

function TSheetMaperCellset.GetParentUName(Member: IXMLDOMNode): string;
var
  UName: string;
begin
  result := '';
  while Assigned(Member.previousSibling) do
  begin
    UName := MemberUName(Member.previousSibling);
    if UName <> 'DUMMY' then
    begin
      result := UName;
      exit;
    end;
    Member := Member.previousSibling;
  end;
end;

procedure TSheetMaperCellset.InitPBbyBothAxes(DOM: IXMLDOMDocument2);
var
  TuplesNL: IXMLDOMNodeList;
begin
  if Assigned(DOM) then
  begin
    FProgressCurCount := 0;
    TuplesNL := GetAxisVisibleTuples(DOM, axColumn);
    FProgressMaxCount := TuplesNL.length;
    TuplesNL := GetAxisVisibleTuples(DOM, axRow);
    FProgressMaxCount := FProgressMaxCount + TuplesNL.length;
  end;
end;

procedure TSheetMaperCellset.IncPB;
begin
  inc(FProgressCurCount);
  SetPBarPosition(FProgressCurCount, FProgressMaxCount);
end;

{Разместить диапазон картежей из набора.
  Параметры:
  TuplesNL - набор картежей
  AxType - тип оси (строка, столбец)
  MemberNum - порядковый номер элемента в кортеже
  LevelNum - номер размещаемого уровня
  LBnd, UBnd  - верхняя и нижная граница диапазона (по номерам)
  CellRInd, CellCInd - координаты ячейки для начала размещения в листе
}
procedure TSheetMaperCellset.MapTupleRange(TuplesNL: IXMLDOMNodeList;
  AxType: TAxisType; MemberNum: integer; LBnd, UBnd: integer;
  CellRInd, CellCInd: integer);
var
  {Временнаые переменные}
  i: integer;
  Ax: TSheetAxisCollectionInterface; //ось - коллекция
  MemberNode: IXMLDOMNode; //узел для мембера
  FixMemberNode: IXMLDOMNode; //зафиксированный узел
  {поля обрабатываемого элемента}
  CurrUName: string;

  {Различные счетчики для рекурсивного алгоритма.}
  RInd, CInd: integer; //координаты текущей ячейки листа
  FixRInd, FixCInd: integer; //координаты запомненной ячейки в листе (закладка)
  FixLBnd: integer; //нижнаяя граница запомненного интервала картежей (закладка)

  {Номер уровня текущего размещающего узла}
  function CurrLevelNum: integer;
  begin
    result := MemberLNum(FixMemberNode);
  end;

  {Формальная корректность входных параметров главной процедуры.
   Все начальные проверки вынесены сюда.}
  function IsIncomesCorrect: boolean;
  begin
    result := false;
    if IsNodeListSubsetCorrect(TuplesNL, LBnd, UBnd) then //нормальный дипазон
      if (CellRInd > 0) and (CellCInd in [1..255]) then //нормальная стартовая ячейка
        if MemberNum >= 0 then //номер кортежа правильный
          result := true;
  end;

  {Инициализация глобальных счетчиков до начала процедуры.
   Выенесно отдельно так же для разгрузки кода основного алгоритма}
  procedure InitCounters;
  var
    i :integer;
  begin
    {Текущие координаты ячейки устанавливаем в начальные}
    RInd := CellRInd;
    CInd := CellCInd;

    {Фиксируем мембер}
    FixMemberNode := nil;
    for i := LBnd to UBnd do
      if not IsTupleHidden(TuplesNL[i]) then
        begin
          FixMemberNode := MemberByNum(TuplesNL, i, MemberNum);
          break;
        end;

    {Закладка ячейки в листе устанавливается в начальную ячейку}
    FixRInd := RInd;
    FixCInd := CInd;

    FixLBnd := LBnd; //закладку нижней границы интервала в начало.
  end;

  {Обрабатываем ли мы последний мембер в картеже}
  function ItIsLastMemberInTuple: boolean;
  var
    LevelCount: Integer;
  begin
    LevelCount := TuplesNL[0].childNodes.length;
    result := (MemberNum >= LevelCount - 1);
  end;


  {Терминальная процедура -
   Мапинг конкретного мембера, вместе с подчиненными}
  procedure MapTupleSet;
  var
    ChildRInd, ChildCInd: integer; //Начальная ячейка для отрисовки подчиненных
    FinalRInd, FinalCInd: integer; //Последняя ячейка для мембера (для объединения)
    CurCaption: string;
    CurrRange: ExcelRange;
    Level: TSheetLevelInterface;
  begin

    CurCaption := MemberCaption(FixMemberNode);

    {Для строк дети начинаются справа, для столбцов - снизу}
    if not Ax.Broken then
    begin
      ChildRInd := IIF((AxType = axRow), FixRInd, CellRInd + 1);
      ChildCInd := IIF((AxType = axRow), CellCInd + 1, FixCInd);
    end
    else //случай с полностью разрушенной осью
    begin
      ChildRInd := IIF((AxType = axRow), FixRInd(* + 1*), CellRInd);
      ChildCInd := IIF((AxType = axRow), CellCInd, FixCInd + 1);
    end;

    {Последняя ячейка для мембера (для объединения}
    if FixRInd <= RInd then
    begin
      FinalRInd := IIF((AxType = axRow), RInd - 1, FixRInd);
      FinalCInd := IIF((AxType = axRow), FixCInd, CInd - 1);
    end
    else //в несбалансированных и при пустышках такое может быть.
    begin
      FinalRInd := FixRInd;
      FinalCInd := FixCInd;
    end;

    {Не нужно выводить кэпшен "итоги" если элемент не имеет подчиненных}
    if (AxType = axRow) and (FixRInd = FinalRInd) or
      (AxType = axColumn) and (FixCInd = FinalCInd) then
      if Assigned(FixMemberNode.nextSibling) then
        if IsItSummary(FixMemberNode.nextSibling) then
          ChangeMemberCaption(FixMemberNode.nextSibling, true, '', '');

    {Если это не лист, то сперва рекурсивно вызываем обработку интервала
     подчиненных мэмберов}
    if not ItIsLastMemberInTuple then
        MapTupleRange(TuplesNL, AxType, MemberNum + 1,
          FixLBnd, i - 1, ChildRInd, ChildCInd);


    {У нас меры в осях. Их рисовать не будем. Названия показателей рисуются
     отдельной процедурой}
    if not IsItMeasure(FixMemberNode) then
    begin
      {Теперь и сам мэмбер рисум. При желании можно делать это в первую очередь}
      if not Ax.Broken then //при разрушений оси не объединям
        CurrRange := MergeCells(ExcelSheet, FixRInd, FixCInd, FinalRInd, FinalCInd)
      else
        CurrRange := GetRange(ExcelSheet, FixRInd, FixCInd, FixRInd, FixCInd);

      IncPB; //индикация
      WriteValue(FixRInd, FixCInd, CurCaption);

      {применим форматирование по уровням}
      if IsItSummary(FixMemberNode) then
        Level := GetLevelByMember(FixMemberNode.previousSibling, AxType)
      else
        Level := GetLevelByMember(FixMemberNode, AxType);
      if Assigned(Level) then
      begin
        if ((AxType = axRow) and Rows.LevelsFormatting) or
          ((AxType = axColumn) and Columns.LevelsFormatting) then
          ApplyLevelFormatting(CurrRange, Level);
        if not IsItSummary(FixMemberNode) then
          if (AxType = axRow) and Ax.UseIndents then
            CurrRange.IndentLevel := Rows.GetLevelIndent(Level);
      end;

      //помечаем мембер
      MarkMember(CurrRange, FixMemberNode, MemberNum, AxType);
    end;

    {!!!!!!!!!!}
//    if not Assigned(FixMemberNode.nextSibling) or Ax.Broken then //!!! это типа лист - остальные не пишем
    begin
      (FixMemberNode.parentNode as IXMLDOMElement).setAttribute('rind', FixRInd);
      (FixMemberNode.parentNode as IXMLDOMElement).setAttribute('cind', FixCInd);
    end;
  end;

begin
  {контроль входных параметров}
  if not IsIncomesCorrect then
    exit;

  Ax := GetAxis(AxType); //инициализируем ось
  InitCounters; //проинициализируем глобальные счетики

  if not Assigned(FixMemberNode) then
    exit;

  {поехали по интервалу узлов-картежей}
  for i := LBnd to UBnd do
  begin
    //Если у кортежа стоит аттрибут "скрытый", тогда пропускаем его
    if IsTupleHidden(TuplesNL[i]) then
      continue;

    MemberNode := MemberByNum(TuplesNL, i, MemberNum);
    CurrUName := MemberUName(MemberNode);

    {Если пошли кортежи с другими мэмберами, значит пора обрабатывать}
    if (MemberUName(FixMemberNode) <> CurrUName) then
    begin
      //обрабатываем кортеж
      MapTupleSet;

      {обновляем счетчики и поля}
      FixMemberNode := MemberNode;
      FixRInd := RInd;
      FixCInd := CInd;
      FixLBnd := i;
    end;

    {шаг по сетке}
    RInd := RInd + IIF((AxType = axRow), 1, 0);
    CInd := CInd + IIF((AxType = axColumn), 1, 0);

  end;

  MapTupleSet;
end;



{================================ Методы класса ===============================}

{Запись значения в лист}
procedure TSheetMaperCellset.WriteValue(RInd, CInd: integer; Value: string);
begin
  try
    ExcelSheet.Cells.Item[RInd, CInd].Value := Value;
  except
  end;
end;


function TSheetMaperCellset.MapTable: boolean;
begin
  FWideTableSummaryMode := false;
  FWideTableMode := false;
  if TableProcessingMode <> tpmHuge then
    result := inherited MapTable //таблица не является "гагантской" - отдаем предку
  else
    //Размещение большой таблицы
    result := MapCellsetTable;
end;

function TSheetMaperCellset.QueryCellsetData: IXMLDOMDocument2;
var
  MDX, ProviderId: string;
  ErrorMsg: WideString;
begin
  result := nil;

  {Первоначальные условия возможности выполнить сию операцию}
  if ((not CheckConnection) or Totals.Empty or (not Assigned(XMLCatalog))) then
    exit;

  if not AllFromSameProvider then
  begin
    PostMessage('Невозможно обработать лист в серверном режиме - ' +
      'присутствуют компоненты из нескольких многомерных баз.', msgError);
    exit;
  end;

  { пытаемся загрузить метаданные базы }
  XMLCatalog.SetUp(DataProvider);
  if not XMLCatalog.Loaded then
    exit;


  MDX := GetCellsetQuery(self, ProviderId);

  if Pos('SELECT', MDX) = 0 then
  begin
    PostMessage(MDX, msgError);
    exit;
  end;

  UpdateMDXLog(MDX, (ExcelSheet.Parent as ExcelWorkBook).Name, ProviderId);

  OpenOperation(pfoQueryMdx, CriticalNode, NoteTime, otQuery);


  if not DataProvider.GetCellsetData(ProviderId, MDX, result, ErrorMsg) then
  begin
    PostMessage(pfoQueryFailed + ErrorMsg{DataProvider.LastError}, msgError);
    exit;
  end;

  CloseOperation; // pfoQueryMdx

  //Сюдаже надо присобачить и данные свободных
  (*
  ConcatenateData(FTotalsData, FreeTotalsData, BothAxisAliasesList);
  ConcatenateData(FTotalsData, FreeTotalsDataIgnored, BothAxisAliasesList);
  ConcatenateData(FTotalsData, SingleCellsData, BothAxisAliasesList);
  *)

  if AddinLogEnable then
    WriteDocumentLog(result, 'Cellset (первичные данные таблицы).xml');
end;

function TSheetMaperCellset.MapCellsetTable: boolean;

  procedure UniteFreeData;
  var
    BothAxisAliasesList: TStringList;
  begin
    BothAxisAliasesList := GetElementAliasesList([wsoRow, wsoColumn]);
    ConcatenateDataEx(FreeTotalsData, FreeTotalsDataIgnored, BothAxisAliasesList);
    ConcatenateDataEx(FreeTotalsData, SingleCellsData, BothAxisAliasesList);
    if AddinLogEnable then
      WriteDocumentLog(FreeTotalsData, 'Данные всех показателей.xml');
  end;
var
  CellsetData: IXMLDOMDocument2;
  TooWide: boolean;
begin
  result := false;
  {Если в таблице нет ни одного, показателя из кубов (одни свободные или ничего),
  то в таком режиме ее не обработаешь}
  if not Totals.CheckByType([wtMeasure, wtResult]) then
  begin
    PostMessage('В таблице отсутствуют показатели из кубов. Расчет таблицы в таком режиме невозможен.', msgError);
    exit;
  end;

  { Возможно в будущем это будет исправлено, но в 2.2.5 показатели, игнорирующие
    ось столбцов, работу листа в серверном режиме. Ось собирается неправильно,
    данные также размещаются неправильно. Поэтому вводится запрет на такие
    показатели.}
  if Totals.CountWithPlacement(true) > 0 then
  begin
    PostMessage('В таблице присутствуют показатели, которые невозможно разложить по оси столбцов. Расчет таблицы в таком режиме невозможен.', msgError);
    exit;
  end;

  OpenOperation(pfoMapTable, CriticalNode, NoteTime, otMap);
  try
    {получаем данные показателей из базы}
    try
      CellsetData := QueryCellsetData;
    except
      PostMessage('Ошибка при получении данных', msgError);
      exit;
    end;
    if (FProcess.LastError <> '') or not Assigned(CellsetData) then
      exit;

    if GetAxisTuples(CellsetData, axColumn).length = 0 then
    begin
      PostMessage('Запрос не смог вернуть данные показателей. Если включена опция "Использовать быстрое пересечение множеств в MDX", попробуйте отключить её', msgWarning);
      //exit;
    end;

    {Предварительная обработка результата: Восстановление иерархии измерений}
    OpenOperation('Построение иерархии измерений', CriticalNode, NoteTime, otProcess);
    try
      //Обрабатываем документ, делая иерархию явной (для размещения)
      ManifestHierarchy(CellsetData);
    finally
      CloseOperation;
    end;

    {Предварительная обработка результата: "Удаление" ненужных кортежей}
    OpenOperation('Сортировка результата запроса', CriticalNode, NoteTime, otProcess);
    try
      ExtractCellsCoords(CellsetData);
      //Помечаем кортежи, которые требуется скрыть
      MarkHiddenTuples(CellsetData);
    finally
      CloseOperation;
    end;

    OpenOperation(pfoCalcSheetSize, CriticalNode, not NoteTime, otProcess);
    //просчитываем размеры таблицы
    try
      ClearTestVersionWarning;
      Sizer.Init(ExcelSheet, FLCID, self, CellsetData, RowsProcessRoute, ColumnsProcessRoute);
    except
      PostMessage('Ошибка при расчете размеров таблицы', msgError);
      exit;
    end;

    {--------- если таблица влезает в лист - размещаем ---------}
    if CheckSheetSize(TooWide) then
    begin
      CloseOperation; //  pfoCalcSheetSize
      // очищаем существующую таблицу (инициализировать надо до очистки!)
      ClearTableEx;

      //фильтры
       if not MapFilters then
        exit;

      {помечаем
      Чтобы формат, заданный в стилях применялся правильно, накладывать
      стили нужно ДО того, как выводить значения в ячейки}
      try
        MarkAxis(axRow, Sizer.RowsHeight);
        MarkAxis(axColumn, Sizer.ColumnsWidth);

        if Sizer.ColumnsHeight > 0 then
          ShakeSummaries(CellsetData, axColumn);
        if Sizer.RowsWidth > 0 then
          ShakeSummaries(CellsetData, axRow);

        { Мапит заголовки показателей, а также собирает информацию о
          доступных для записи областях, аналогично
          TSheetMaper.GetWritableTotalsInfo}
        MapTotalTitles(CellsetData);
        MapAxisTitles(axRow);
        MapAxisTitlesComments(axRow);
        MapAxisTitles(axColumn);
        MapAxisTitlesComments(axColumn);
      except
        PostMessage('Ошибка при размещении таблицы', msgError);
        exit;
      end;

      //Собственно рисование таблицы
      MapCellsetDOM(CellsetData);
      { Соберем все данные свободных и отдельных в единый ДОМ и разместим его}
      (*UniteFreeData;*)
      MapConsts;
      MapTypeFormulas;
      MapFreeData(CellsetData);

      //заголовки показателей
      {Размещение и маркировка вспомогательных областей}
      if not DoFinalMapping then
        exit;

      CloseOperation;//MapTable
      result := true;
    end
    else
      PostMessage(Format(ermSheetOverSize, [256(*ExcelSheet.Columns.Count*), ExcelSheet.Rows.Count]), msgError);//pfoCalcSheetSize
  finally
    KillDOMDocument(CellsetData);
  end;
end;


{Размещение значений}
procedure TSheetMaperCellset.MapDataCells(DataDOM: IXMLDOMDocument2;
  StartRInd, StartCInd: integer);
var
  DataNL: IXMLDOMNodeList;
  i: integer;
  CellOrd: integer;
  RInd, CInd: integer;
  RowRootNode, ColumnRootNode: IXMLDOMNode;

  {возвращает номера кортежей для ячейки с данными}
  procedure GetTupleIndexes(CellOrd: integer;
    var RowIndex: integer; var ColumnIndex: integer);
  begin
    {Преобразуем порядковый номер ячейки в порядковые номера строк и столбцов}
    if Assigned(ColumnRootNode) then
    begin
      ColumnIndex := CellOrd mod ColumnRootNode.childNodes.length;
      if Assigned(RowRootNode) then
        RowIndex := CellOrd div ColumnRootNode.childNodes.length
      else
        RowIndex := -1;
    end
    else
    begin
      ColumnIndex := -1;
      if Assigned(RowRootNode) then
        RowIndex := CellOrd
      else
        RowIndex := -1
    end;
  end;

  {Вычисляет по порядковым номерам строк и столбцов ячейку в экселе куда надо разместить}
  procedure GetCellAddress(var RowIndex: integer; var ColumnIndex: integer);
  begin
    {По порядковым номерам строк и столбцов ищем координаты ячейки}
    if ColumnIndex >= 0 then
    begin
      ColumnIndex := GetIntAttr(ColumnRootNode.childNodes[ColumnIndex], 'cind', -1);
    end
    else
      ColumnIndex := StartCInd;

    if RowIndex >= 0 then
    begin
      RowIndex := GetIntAttr(RowRootNode.childNodes[RowIndex], 'rind', -1);
    end
    else
      RowIndex := StartRInd;
  end;

  {По номеру ячейки определяет показатель}
  function TotalByCellOrd(CellOrd: integer): TSheetTotalInterface;
  var
    TotalInd: integer;
  begin
    result := nil;

    if Assigned(ColumnRootNode) then
    begin
      {Вчисляем индэкс тотала по номеру ячейки.
       Делаем в два хода:
        Сначала находим номер серии (колонки),  затем номер показателя}
      TotalInd := CellOrd mod ColumnRootNode.childNodes.length;
      TotalInd := TotalInd mod Totals.Count;

      result := Totals[TotalInd];
    end;
  end;

  {Отформатированное значение показателя, которое идет в сетку}
  function FormattedValue(CellNode: IXMLDOMNode; CellOrd: integer;
    out AFormattedValue: extended; out StringValue: string): boolean;
  var
    Total: TSheetTotalInterface;

  begin
    result := false;
    StringValue := '';
    if Assigned(CellNode) then
    begin
      StringValue := CellNode.text;

      Total := TotalByCellOrd(CellOrd);
      if Assigned(Total) then
        if not(StringValue = Total.EmptyValueSymbol) then
        begin
          if not IsNumber(StringValue) then
            UpdatePossibleNumberValue(StringValue);
          if IsNumber(StringValue) then
          begin
            AFormattedValue := StrToFloat(StringValue);
            AFormattedValue := Total.GetDividedValue(AFormattedValue);
            Total.Round(AFormattedValue); ////округлим согласно показателю
            result := true;
          end
          else
            StringValue := CellNode.text;
        end;
    end;
  end;

  function IsGreyCell(RowRoot, ColumnRoot, CellNode: IXMLDOMNode): boolean;
  var
    Index: integer;
    Tuple: IXMLDOMNode;
  begin
    result := false;
    if not Assigned(CellNode) then
      exit;
    if Assigned(RowRoot) then
    begin
      Index := GetIntAttr(CellNode, attrRowIndex, 0);
      Tuple := RowRoot.childNodes[Index];
      result := TupleHasSummary(Tuple);
    end;
    if Assigned(ColumnRoot) then
    begin
      Index := GetIntAttr(CellNode, attrColumnIndex, 0);
      Tuple := ColumnRoot.childNodes[Index];
      result := result or TupleHasSummary(Tuple);
    end;
  end;

  function GetFormattingLevel(RowIndex, ColumnIndex: integer): TSheetLevelInterface;
  var
    Tuple: IXMLDOMNode;
    MemberIndex, i: integer;
    AxisType: TAxisType;
  begin
    result := nil;
    Tuple := nil;
    if not Totals.StyleByLevels then
      exit; 
    AxisType := axRow;
    { по индексу и опциям форматирования найдем нужный кортеж}
    if Totals.FormatByRows and (RowIndex > -1) then
    begin
      Tuple := RowRootNode.childNodes[RowIndex];
      AxisType := axRow;
    end
    else
      if not Totals.FormatByRows and (ColumnIndex > -1) then
      begin
        Tuple := ColumnRootNode.childNodes[ColumnIndex];
        AxisType := axColumn;
      end;
    if not Assigned(Tuple) then
      exit;

    { теперь получим порядковый номер уровня. Если в кортеже есть
      узел-саммари, то искомый уровень предшествует ему. Если саммари нет,
      то нам нужен первый с конца уровень-не-пустышка.}
    MemberIndex := -1;
    for i := 0 to Tuple.childNodes.length - 1 do
      if IsItSummary(Tuple.childNodes[i]) then
      begin
        MemberIndex := i - 1;
        break;
      end;

    if MemberIndex = -1 then
    begin
      MemberIndex := Tuple.childNodes.length - 1;
      while IsItMeasure(Tuple.childNodes[MemberIndex]) or
        (MemberUName(Tuple.childNodes[MemberIndex]) = 'DUMMY') do
        dec(MemberIndex);
    end;

    { получим уровень по его номеру}
    if MemberIndex > -1 then
      (*result := GetLevelByMemberIndex(AxisType, MemberIndex);*)
      result := GetLevelByMember(Tuple.childNodes[MemberIndex], AxisType);
  end;

var
  FormattingLevel: TSheetLevelInterface;
  ECell: ExcelRange;
  RowCoords, ColumnCoords: TIntegerArray;
  Tuple: IXMLDOMNode;
  RowTupleIndex, ColumnTupleIndex, x, y: integer;
  NumericValue: extended;
  StringValue: string;
begin
  if not Assigned(DataDOM) then
    exit;

  {Инициализируем строки}
  RowRootNode := GetRowAxisNode(DataDOM);
  ColumnRootNode := GetColumnAxisNode(DataDOM);


  {В цикле обрабатываем каждую цифру}
  DataNL := DataDOM.selectNodes('root/CellData/Cell[@CellOrdinal]/Value');
  for i := 0 to DataNL.length - 1 do
  begin
    SetPBarPosition(i + 1, DataNL.length);

    CellOrd := GetIntAttr(DataNL[i].parentNode, 'CellOrdinal', -1);
    if CellOrd >= 0 then
    begin

      {получим индексы кортежей по осям}
      GetTupleIndexes(CellOrd, RowTupleIndex, ColumnTupleIndex);
      RInd := RowTupleIndex;
      CInd := ColumnTupleIndex;
      GetCellAddress(RInd, CInd);
      FormattingLevel := GetFormattingLevel(RowTupleIndex, ColumnTupleIndex);

      {В серверном режиме при сломанной оси один и тот же кортеж может
        соответствовать нескольким, поэтому работаем с массивами координат.}
      if GetAxis(axRow).Broken and not GetAxis(axRow).Empty then
      begin
        Tuple := RowRootNode.childNodes[RowTupleIndex];
        GetIntArrayAttr(Tuple, attrRowCoords, RowCoords);
      end
      else
      begin
        SetLength(RowCoords, 1);
        RowCoords[0] := RInd;
      end;
      if GetAxis(axColumn).Broken and not GetAxis(axColumn).Empty then
      begin
        Tuple := ColumnRootNode.childNodes[ColumnTupleIndex];
        GetIntArrayAttr(Tuple, attrColumnCoords, ColumnCoords);
      end
      else
      begin
        SetLength(ColumnCoords, 1);
        ColumnCoords[0] := CInd;
      end;

      for x := 0 to Length(RowCoords) - 1 do
        for y := 0 to Length(ColumnCoords) - 1 do
        begin
          RInd := RowCoords[x];
          CInd := ColumnCoords[y];
          //SetAttr(DataNL[i].parentNode, 'ExcelRow', RInd);
          //SetAttr(DataNL[i].parentNode, 'ExcelColumn', CInd);

          {Размещаем только если нашли координаты.
           А можем и не найти, поскольку некоторые кортежи могут быть скрытыми
           (они не были размещены в листе и координаты у них не проставляются)}
          if (RInd > 0) and (CInd > 0) then
          begin
            ECell := GetRange(ExcelSheet, RInd, CInd, RInd, CInd);
            if Assigned(FormattingLevel) then
              ApplyLevelFormatting(ECell, FormattingLevel);
            if FormattedValue(DataNL[i], CellOrd, NumericValue, StringValue) then
              try
                ExcelSheet.Cells.Item[RInd, CInd].Value2 := NumericValue;
              except
              end
            else
              WriteValue(RInd, CInd, StringValue);
          end;
        end;
    end;
  end;
end;



{Помечает "скрываемые" кортежи для соответствующей оси}
procedure TSheetMaperCellset.MarkHiddenTuplesForAxis(DOM: IXMLDOMDocument2;
  AxType: TAxisType);

  function GetAllMemberSummaryCaption(Member: IXMLDOMNode): string;
  var
    ParentCaption: string;
    Level: TSheetlevelInterface;
  begin
    ParentCaption := '';
    while Assigned(Member.previousSibling) do
    begin
      Member := Member.previousSibling;
      if (ParentCaption = '') and (MemberUName(Member) <> 'DUMMY') then
      begin
        ParentCaption := MemberCaption(Member);
        break;
      end;
    end;
    Level := GetLevelByMember(Member, AxType);
    if Assigned(Level) then
      result := Level.SummaryOptions.GetCaption(ParentCaption)
    else
      result := stUsual;
  end;

var
  Ax: TSheetAxisCollectionInterface;
  i, j: integer;
  XPath, SummaryCaption: string;
  NL: IXMLDOMNodeList;
  MemberNode, CaptionNode: IXMLDOMNode;
begin
  if not Assigned(DOM) then
    exit;

  Ax := GetAxis(AxType);
  for i := 0 to Ax.Count - 1 do
    {Обрабатываем элемент только в том случае, если у него есть алмембер
     и не выбран явно уровень - all. Потому что если у него выбран уровень All,
     тогда это уже и не итог, и его надо детализировать}
    if (Ax[i].AllMember <> '') and (Ax[i].Levels[0].Name <> '(All)') then
    begin
      {Будем искать набор картежей, где есть аллмемер этого элемента}
      XPath := 'root/Axes/Axis[@name="%s"]/Tuples/Tuple[not (@%s)]/Member[UName="%s"]';
      if AxType = axRow then
        XPath := Format(XPath, ['Axis1', attrHiddenTuple, Ax[i].AllMember])
      else
        XPath := Format(XPath, ['Axis0', attrHiddenTuple, Ax[i].AllMember]);

      NL := DOM.selectNodes(XPath);

      {Теперь сканируем каждый узел найденого и проверяем его как подозреваемого
      на скрытие.
      А критерий такой:
      Проходим по последующим мемберам того-же картежа (сестрам), среди них
      могут быть только "алмемберы". Если это не так, тогда кортеж - "хайд"}
      for j := 0 to NL.length - 1 do
      begin
        IncPB;
        MemberNode := NL[j];

        SummaryCaption := GetAllMemberSummaryCaption(MemberNode);

        {!!!!!!!!!! для несбалансированных ??}
        CaptionNode := MemberNode.selectSingleNode('Caption');
        if CaptionNode.text <> '' then
        begin
          CaptionNode.text := SummaryCaption;
          MarkAsSummary(MemberNode);
        end;

        MemberNode := MemberNode.nextSibling;
        while Assigned(MemberNode) do
        begin
          if AxType = axColumn then
            if IsItMeasure(MemberNode) then
              break;
          if MemberFromAllLevel(MemberNode) then
          begin

            //нужно потереть капшн, что бы его в сетке небыло видно
            //бред!!!!!!
            ChangeMemberCaption(
              MemberNode,
              (MemberUName(MemberNode) <> MemberUName(NL[j])),
              '',
              MemberCaption(MemberNode));

          end
          else
          begin
            //помечаем кортеж как скрытый
            MarkTupleHidden(NL[j]);
            break; //дальше нет смысла смотреть сестер - мембер скрыт!
          end;

          //переходим к следующей сестренке-мемберу
          MemberNode := MemberNode.nextSibling;
        end;
      end;
    end;
end;


{Кортежи, которые требуется скрыть помечаются аттрибутом hidden="true".
 К таким кортежам (узлам) относятся узлы, которые детализируют итоги -
 этого делать не надо}
procedure TSheetMaperCellset.MarkHiddenTuples(DOM: IXMLDOMDocument2);
begin
  {Обрабатыаем обе оси}
  //неточная индикация! по максимуму - все кортежи. но иначе тут трудно
  InitPBbyBothAxes(DOM);
  MarkHiddenTuplesForAxis(DOM, axRow);    
  MarkHiddenTuplesForAxis(DOM, axColumn);

  {Если надо, то скрываем элементы "ДАННЫЕ"}
  //ресет индикации. здесь, в виде исключения, на один узел - два пробега прогресса
  InitPBbyBothAxes(DOM);
  HideDataMemberTuples(Dom, axRow);
  HideDataMemberTuples(Dom, axColumn);

  UnmarkSingleLineSummaries(Dom);

  {также скрываем итоги для тех уровней, на которых они отключены}
  InitPBbyBothAxes(DOM); 
  HideDisabledSummaries(Dom, axRow);
  HideDisabledSummaries(Dom, axColumn);

  if Columns.HideEmpty then
    HideTuplesByCounterparts(Dom, axColumn);
  if Rows.HideEmpty then
    HideTuplesByCounterparts(Dom, axRow);

  if AddinLogEnable then
    WriteDocumentLog(DOM, 'Cellset (после маркировки лишних кортежей).xml');
end;

procedure TSheetMaperCellset.ManifestHierarchy(DOM: IXMLDOMDocument2);
begin
  InitPBbyBothAxes(DOM);

  {Обрабатыаем обе оси}
  ManifestHierarchyForAxis(DOM, axRow);
  ManifestHierarchyForAxis(DOM, axColumn);

  if AddinLogEnable then
    WriteDocumentLog(DOM, 'Cellset (после установки иерархии).xml');
end;

{ <<<
    !!!!
     мысли "вслух" перед написанием, может и потом пригодится
     когда станет мусором - удалить.
   >>>
 Что и для чего будем делать см. подробней в секции объявлений.
 Теперь о том, как мы это будем делать.
 Берм ось модели. Берем набор узлов-картежей этой оси из документа с данными.
 Изначально, в каждом кортеже по одному мемберу для каждого элемента оси.
 А нам надо, что было по одному мемберу для каждого измерения элемента оси.
 Порядок перечисления мемберов в кортеже и кортежей в наборе фиксированный.
 (полагаемся на него)

 Алгоритм простой:
 Значится, проходимся по всем картежам и для каждого делаем вот чего:
   Для каждого элемента оси из модели ищем соответствующий ему мембер (узел).
   Этот узел соответсвует какому-то определенному уровню - вытаскиваем, смотрим.
   1)
   Нам нужно вставить ВПЕРЕДИ (!) узлы мемберы предыдущих уровней
      Их берем (клонированием) из предыдущего кортежа. Поскольку порядок следования
      кортежей в наборе четкий, там все уже должно быть построено (идем-то последовательно)
   2)
   ПОСЛЕ (!) нам нужно вставить узлы мемберов последующих уровней.
      Они всегда фиктивные, с пустым капшном. Соответсвуют пустышка итогов.

  ну-с. приступим...
 }
procedure TSheetMaperCellset.ManifestHierarchyForAxis(DOM: IXMLDOMDocument2;
  AxType: TAxisType);

  function MemberIsLeaf(Member: IXMLDOMNode;
    AxisElem: TSheetAxisElementInterface): boolean;
  var
    UName: string;
    Node: IXMLDOMNode;
  begin
    result := false;
    UName := MemberUName(Member);
    EncodeXPathString(UName);
    if Assigned(AxisElem.Members) then
      Node := AxisElem.Members.selectSingleNode(Format(
        'function_result/Members//Member[@%s="%s"]', [attrUniqueName, UName]));
    if Assigned(Node) then
      result := not Node.hasChildNodes;
    if result then
      (Member as IXMLDOMElement).setAttribute(attrMemberLeaf, 'true');
  end;

var
  Ax: TSheetAxisCollectionInterface;
  i, j, k: integer;
  TuplesNL: IXMLDOMNodeList;
  CurrMember, tmpMember: IXMLDOMNode;
  MemberNumber, DummyCount: integer;
  Level: TSheetLevelInterface;
begin
  if not Assigned(DOM) then
    exit;

  Ax := GetAxis(AxType);
  if Ax.Count = 0 then
    exit; //оси нет. обрабатывать нечего.

  TuplesNL := GetAxisTuples(DOM, AxType);
  if not Assigned(TuplesNL) then
    exit; //непонятная ошибка

  for i := 0 to TuplesNL.length - 1 do
  begin
    IncPB;

    {Последовательно будем проверять наличие всех необходимых мемберов в текущем
     кортеже. Инициализируем индекс мембера}
    MemberNumber := 0;

    for j := 0 to Ax.Count - 1 do
    begin

      { Попытаемся определить пригодность кортежа для обратной записи.
        Для этого все его мемберы должны быть листовыми узлами.}
        tmpMember := TuplesNL[i].childNodes[MemberNumber];
        if not MemberIsLeaf(tmpMember, Ax[j]) then
          (TuplesNL[i] as IXMLDOMElement).setAttribute(attrIsGrey, 'true');

        (tmpMember as IXMLDOMElement).setAttribute(attrAxisIndex, j);
        Level := Ax[j].Levels.FindByInitialIndex(MemberLNum(tmpMember));
        if Assigned(Level) then
          (tmpMember as IXMLDOMElement).setAttribute(attrLevelIndex, Level.GetSelfIndex);

      {Если  у элемента иерархия разрушена или у него только один уровень,
       его мемберы обрабатывать не надо}
      if (Ax[j].IgnoreHierarchy) or (Ax[j].Levels.Count = 1) or Ax.Broken then
      begin
        inc(MemberNumber);
        continue;
      end;

      for k := 0 to Ax[j].Levels.Count - 1 do
      begin
        //ищем в кортеже мембер соответсвующего уровня
        CurrMember := MemberForThatLevel(TuplesNL[i],
          FullLevelNameMDX(Ax[j].Levels[k]));
        {если его нет, тогда посмотрим олл-мембер. он тоже подойдет.
         Его может не быть в модели (явно не включен в дереве), но при этом
         все равно может фигурировать для итогов. В этом случае считаем что
         мембер есть и добиваем оставшееся пустышками
         }
        if not Assigned(CurrMember) then
          if Ax[j].Levels[0].Name <> '(All)' then
            CurrMember := MemberForThatLevel(TuplesNL[i],
              Ax[j].FullDimensionNameMDX + '.[(All)]');

        if Assigned(CurrMember) then
        begin
          {Нашли мембер от этого измерения. Остальные добиваем пустышками
           и переходим к следующему элементу оси}
          DummyCount := Ax[j].Levels.Count - k - 1;
          if DummyCount > 0 then
          begin
            CloneEmptyMembers(CurrMember, DummyCount, Ax[j].Levels[k]);

            {Наращивам счетчик на число вставленных пустышек и сразу переходим
            к следующему компонету измерения}
            MemberNumber := MemberNumber + DummyCount;
          end;

          inc(MemberNumber);
          break;
        end
        else
        begin
          CopyMemberFromParentTuple(TuplesNL[i], MemberNumber);
        end;

        inc(MemberNumber); //следующий мембер
      end;

    end;
  end;
end;

procedure TSheetMaperCellset.MapAxisMP(DOM: IXMLDOMDocument2; AxType: TAxisType);
var
  i, j, k: integer;
  Axis: TSheetAxisCollectionInterface;
  TuplesNL, MPNL: IXMLDOMNodeList;
  XPath, FullDimName, AlterMPName, MPValue: string;
  RInd, CInd, MPOffset: integer;
begin
  if not Assigned(DOM) then
    exit;

  MarkMPArea(AxType);
  Axis := GetAxis(AxType);

  XPath := 'root/Axes/Axis[@name="%s"]/Tuples/Tuple[not (@hidden)]';
  XPath := Format(XPath, [IIF((AxType = axRow), 'Axis1', 'Axis0')]);
  TuplesNL := DOM.selectNodes(XPath);
  MPCache := TStringList.Create;

  for i := 0 to TuplesNL.length - 1 do
  begin
    SetPBarPosition(i + 1, TuplesNL.length);
    MPOffset := -1;

    for j := 0 to Axis.Count - 1 do
    begin
      FullDimName := Axis[j].FullDimensionNameMDX;

      for k := 0 to Axis[j].MemberProperties.Count - 1 do
        if Axis[j].MemberProperties[k].Checked then
        begin
          inc(MPOffset);
          AlterMPName := GetAlterMPName(Dom, AxType, Axis[j],
            Axis[j].MemberProperties[k].Name);
          XPath := 'Member[(@Hierarchy="%s") and (UName!="DUMMY")]/%s';
          XPath := Format(XPath, [FullDimName, AlterMPName]);
          MPNL := TuplesNL[i].selectNodes(XPath);

          if MPNL.length = 0 then
            continue;

          MPValue := MPNL[MPNL.length - 1].text;
          if AxType = axRow then
          begin
            RInd := GetIntAttr(TuplesNL[i], 'rind', 0);
            CInd := Sizer.StartRowMProps.y + MPOffset;
          end
          else
          begin
            CInd := GetIntAttr(TuplesNL[i], 'cind', 0);
            RInd := Sizer.StartColumnMProps.x + MPOffset;
          end;

          if (RInd > 0) and (CInd > 0) then
            WriteMaskedValue(RInd, CInd, MPValue, Axis[j].MemberProperties[k].Mask);
        end;
    end;
  end;
  FreeStringList(MPCache);
end;



procedure TSheetMaperCellset.MapCellsetDOM(DOM: IXMLDOMDocument2);

  function MapMP(AxisType: TAxisType): boolean;
  var
    tmpInt: integer;
  begin
    result := true;
    if AxisType = axRow then
      tmpInt := Sizer.RowsWidth
    else
      tmpInt := Sizer.ColumnsHeight;
    if tmpInt > 0 then
    begin
      OpenOperation(IIF(AxisType = axRow, pfoMapRowsMP, pfoMapColumnsMP),
        CriticalNode, not NoteTime, otMap);
      try
        MapAxisMP(DOM, AxisType);
      except
        PostMessage(IIF(AxisType = axRow, ermMapRowsMPFailed, ermMapColumnsMPFailed), msgError);
        result := false;
        exit;
      end;
      CloseOperation;
    end;
  end;

const
  StartTableRInd = 1;
  StartTableCInd = 1;
var
  RowTuplesNL, ColumnTuplesNL: IXMLDOMNodeList;
begin
  if not (Assigned(ExcelSheet) and Assigned(DOM)) then
    exit;

  ColumnTuplesNL := GetAxisTuples(DOM, axColumn);
  RowTuplesNL := GetAxisTuples(DOM, axRow);

  //индикация только при наличии измерений в строках
  if Columns.Count > 0 then
  begin
    OpenOperation(pfoMapColumns, CriticalNode, NoteTime, otMap);
    FProgressMaxCount := ColumnTuplesNL.length;
    FProgressCurCount := 0;
  end;

  {Размещаем столбцы}
  try
    if Sizer.ColumnsHeight > 0 then
      if Columns.Broken then
        MapBrokenAxis(DOM, axColumn)
        //HandleBrokenAxis(DOM, axColumn, Sizer.StartColumns)
      else
      begin
        ColumnTuplesNL := GetAxisTuples(DOM, axColumn);
        MapTupleRange(ColumnTuplesNL, axColumn, 0, 0, ColumnTuplesNL.length - 1,
          Sizer.StartColumns.x, Sizer.StartColumns.y);
      end
    else
      {В этом случае, смысл вызова - проставить координаты у кортежей
      (рисоваться там ничего не будет)}
      MapTupleRange(ColumnTuplesNL, axColumn, 0, 0, ColumnTuplesNL.length - 1,
        Sizer.StartTotalsTitle.x, Sizer.StartTotalsTitle.y);


  except
    PostMessage('Ошибка при размещении оси столбцов', msgError);
    exit;
  end;
    {Размещаем MP cтолбцов}
    if not MapMP(axColumn) then
      exit;
  if Columns.Count > 0 then
    CloseOperation;

  OpenOperation(pfoMapRows, CriticalNode, NoteTime, otMap);
  FProgressMaxCount := RowTuplesNL.length;
  FProgressCurCount := 0;
  {Размещаем строки}
  try
    if Sizer.RowsWidth > 0 then
      if Rows.Broken then
        MapBrokenAxis(DOM, axRow)
      else
      begin
        RowTuplesNL := GetAxisTuples(DOM, axRow);
        MapTupleRange(RowTuplesNL, axRow, 0, 0, RowTuplesNL.length - 1,
          Sizer.StartRows.x, Sizer.StartRows.y);
      end;
  except
    PostMessage('Ошибка при размещении оси строк', msgError);
    exit;
  end;

  {Размещаем MP строк}
  if not MapMP(axRow) then
      exit;
  CloseOperation; //закрытие узла размещения оси

  if AddinLogEnable then
    WriteDocumentLog(DOM, 'Cellset (после размещения осей).xml');


  OpenOperation(pfoMapTotals, CriticalNode, NoteTime, otMap);
  {Размещаем значения}
  try
    MapDataCells(DOM, Sizer.StartTotals.x, Sizer.StartTotals.y);
    if AddinLogEnable then
      WriteDocumentLog(DOM, 'Cellset (после размещения данных).xml');
  except
    PostMessage('Ошибка при размещении показателей', msgError);
    exit;
  end;
  MarkResultsGrey(Dom);
  CloseOperation;
end;

procedure TSheetMaperCellset.MapFreeData(DOM: IXMLDOMDocument2);
var
  i, j, k: integer;
  FreeDataNL: IXMLDOMNodeList;
  Node: IXMLDOMNode;
  RInd, CInd: integer;
  XPath, XPathFilter, UName: string;
  TotalValue(*, TotalFormula*): string;
  AFormattedValue: extended;
begin
  if not (Assigned(ExcelSheet) and Assigned(DOM) and Assigned(FreeTotalsData)) then
    exit;

  RInd := 0;
  CInd := 0;
  FreeDataNL := FreeTotalsData.selectNodes('//data/row');
  for i := 0 to FreeDataNL.length - 1 do
  begin
    SetPBarPosition(i + 1, FreeDataNL.length);

    {RIND}
    if Rows.Count > 0 then
    begin
      XPath := 'root/Axes/Axis[@name="Axis1"]/Tuples/Tuple[%s]';
      XPathFilter := '';

      for j := 0 to Rows.Count - 1 do
      begin
        UName := GetStrAttr(FreeDataNL[i], Rows[j].Alias, '');
        EncodeXPathString(UName);
        if UName <> '' then
        begin
          AddTail(XPathFilter, ' and ');
          //
          XPathFilter := XPathFilter + 'Member[UName="' + UName + '"]';
        end;
      end;

      if XPathFilter <> '' then
      begin
        XPath := Format(XPath, [XPathFilter]);
        Node := DOM.selectSingleNode(XPath);
        if Assigned(Node) then
          RInd := GetIntAttr(Node, 'rind', 0);
      end
      else
        continue;
    end
    else
      RInd := Sizer.StartTotals.x;

    {CIND}
    XPath := 'root/Axes/Axis[@name="Axis0"]/Tuples/Tuple[%s]';
    XPathFilter := '';

    if Columns.Count > 0 then
    begin
      for j := 0 to Columns.Count - 1 do
      begin
        UName := GetStrAttr(FreeDataNL[i], Columns[j].Alias, '');
        EncodeXPathString(UName);
        if UName <> '' then
        begin
          AddTail(XPathFilter, ' and ');
          XPathFilter := XPathFilter + 'Member[UName="' + UName + '"]';
        end;
      end;
    end;

    for k := 0 to Totals.Count - 1 do
      if Totals[k].TotalType in [wtFree, wtResult] then
      begin
        TotalValue := GetStrAttr(FreeDataNL[i], Totals[k].Alias, '');
        (*if Assigned( FreeDataNL[i].selectSingleNode('./' + Totals[k].Alias + snSeparator + 'formula')) then
          TotalFormula :=*)
        if TotalValue <> '' then
        begin
          if Columns.Count > 0 then
          begin
            AddTail(XPathFilter, ' and ');
            XPathFilter := XPathFilter + 'Member[UName="[Measures].[' + Totals[k].Alias + ']"]';

            XPath := Format(XPath, [XPathFilter]);
            Node := DOM.selectSingleNode(XPath);
            if Assigned(Node) then
              CInd := GetIntAttr(Node, 'cind', 0);
          end
          else
            CInd := Sizer.StartTotals.y + k;

          if not IsNumber(TotalValue) then
            UpdatePossibleNumberValue(TotalValue);
          if IsNumber(TotalValue) then
          begin
            AFormattedValue := StrToFloat(TotalValue);
            AFormattedValue := Totals[k].GetDividedValue(AFormattedValue);
            Totals[k].Round(AFormattedValue); //округлим согласно показателю
            TotalValue := FloatToStr(AFormattedValue);
          end
          else
            TotalValue := GetStrAttr(FreeDataNL[i], Totals[k].Alias, '');

          if (RInd > 0) and (CInd > 0) then
            WriteValue(RInd, CInd, TotalValue);

          (*if IsTypeFormulaException(Totals[k], RInd, CInd) then
          with GetRange(ExcelSheet, RInd, CInd) do
          begin
            Interior.PatternColorIndex := 32;
            Interior.Pattern := xlGray16;
          end;*)

        end;
      end;

    if GetBoolAttr(FreeDataNL[i], 'inTable', false) then
    begin
    end;
  end;

end;


procedure TSheetMaperCellset.HideDataMemberTuples(DOM: IXMLDOMDocument2; AxisType: TAxisType);
var
  Axis: TSheetAxisCollectionInterface;
  Level: TSheetLevelInterface;
  i, j: integer;
  MembersNL: IXMLDOMNodeList;
  XPath, UName, FullDimName: string;
begin
  Axis := GetAxis(AxisType);
  if Axis.Empty then
    exit;

  for i := 0 to Axis.Count - 1 do
  begin
    if not Axis[i].HideDataMembers then
      continue;
    FullDimName := Axis[i].FullDimensionNameMDX;
    XPath := 'root/Axes/Axis[@name="%s"]/Tuples/Tuple[not(@%s)]/Member[@Hierarchy="%s"]';
    XPath := Format(XPath, [IIF(AxisType = axColumn, 'Axis0', 'Axis1'),
      attrHiddenTuple, FullDimName]);
    MembersNL := Dom.selectNodes(XPath);
    for j := 0 to MembersNL.length - 1 do
    begin
      IncPB;
      UName := MembersNL[j].selectSingleNode('UName').text;
      if Copy(UName, Length(UName) - 10, 11) = '.DATAMEMBER' then
      begin
        Level := Axis[i].Levels.FindByInitialIndex(MemberLNum(MembersNL[j]));
        if Level.HideDataMembers then
          MarkTupleHidden(MembersNL[j]);
      end;
    end;
  end;
end;

function TSheetMaperCellset.GetAlterMPName(Dom: IXMLDOMDocument2; AxisType: TAxisType;
  AxisElement: TSheetAxisElementInterface; MPName: string): string;

  function GetNode(FullDimName, FullMPName: string): IXMLDOMNode;
  var
    XPath: string;
  begin
    XPath := 'root/OlapInfo/AxesInfo/AxisInfo[@name="%s"]/HierarchyInfo[@name="%s"]/*[@name="%s"]';
    XPath := Format(XPAth, [IIF(AxisType = axColumn, 'Axis0', 'Axis1'),
      FullDimName, FullMPName]);
    result := Dom.selectSingleNode(XPath);
  end;

var
  DimName, tmpStr: string;
  Node: IXMLDOMNode;
  i: integer;
begin
  DimName := AxisElement.FullDimensionNameMDX;
  {сначала поищем в кэше - может уже было}
  tmpStr := DimName + '.[' + MPName + ']';
  result := MPCache.Values[tmpStr];
  if result <> '' then
    exit;
  {если нет, то смотрим инфо-секцию документа}
  Node := GetNode(DimName, tmpStr);
  if not Assigned(Node) then
    for i := 0 to AxisElement.Levels.Count - 1 do
    begin
      tmpStr := Format('%s.[%s].[%s]', [DimName, AxisElement.Levels[i].Name, MPName]);
      Node := GetNode(DimName, tmpStr);
      if Assigned(Node) then
        break;
    end;
  {если нашли, то закэшируем для дальнейшего испорльзования}
  if Assigned(Node) then
  begin
    result := Node.nodeName;
    MPCache.Add(tmpStr + '=' + result);
  end;
end;

procedure TSheetMaperCellset.ShiftDataCells(Dom: IXMLDOMDocument2;
  AxisType: TAxisType; FromIndex, ToIndex: integer);
var
  i: integer;
  Cells: IXMLDOMNodeList;
  AttrName, CounterAttrName, XPath: string;
  AttrValue, CounterAttrValue, CellOrdinal, ColumnAxisSize: integer;
begin
  ColumnAxisSize := GetAxisTuples(Dom, axColumn).length;
  AttrName := IIF(AxisType = axRow, attrRowIndex, attrColumnIndex);
  CounterAttrName := IIF(AxisType = axRow, attrColumnIndex, attrRowIndex);

  XPath := Format('root/CellData/Cell[(number(@%s)>="%d") and (number(@%s)<="%d")]',
    [AttrName, FromIndex, AttrName, ToIndex]);
  Cells := Dom.selectNodes(XPath);

  for i := 0 to Cells.length - 1 do
  begin
    AttrValue := GetIntAttr(Cells[i], AttrName, 0);
    CounterAttrValue := GetIntAttr(Cells[i], CounterAttrName, 0);
    if AttrValue = FromIndex then
      AttrValue := ToIndex
    else
      dec(AttrValue);
    if AxisType = axRow then
      CellOrdinal := ColumnAxisSize * AttrValue + CounterAttrValue
    else
      CellOrdinal := ColumnAxisSize * CounterAttrValue + AttrValue;
    with Cells[i] as IXMLDOMElement do
    begin
      setAttribute('CellOrdinal', CellOrdinal);
      setAttribute(AttrName, AttrValue);
      setAttribute(CounterAttrName, CounterAttrValue);
    end;
  end;
end;

procedure TSheetMaperCellset.ExtractCellsCoords(Dom: IXMLDOMDocument2);
var
  ColumnAxisSize: integer;
  Cells: IXMLDOMNodeList;
  i, CellOrdinal: integer;
begin
  ColumnAxisSize := GetAxisTuples(Dom, axColumn).length;
  Cells := Dom.selectNodes('root/CellData/Cell[@CellOrdinal]');
  for i := 0 to Cells.length - 1 do
    with Cells[i] as IXMLDOMElement do
    begin
      CellOrdinal := GetIntAttr(Cells[i], 'CellOrdinal', 0);
      setAttribute(attrRowIndex, CellOrdinal div ColumnAxisSize);
      setAttribute(attrColumnIndex, CellOrdinal mod ColumnAxisSize);
    end;
end;

procedure TSheetMaperCellset.ShakeSummaries(Dom: IXMLDOMDocument2;
  AxisType: TAxisType);

  { Перемещает кортеж по оси, вставляя его перед кортежем[Index], либо последним}
  procedure MoveSummaryTuple(TupleRoot: IXMLDOMNode; var Tuple: IXMLDOMNode;
    Index: integer; const TuplesCount: integer);
  var
    refNode: IXMLDOMNode;
  begin
    if Index < TuplesCount  then
    begin
      refNode := TupleRoot.childNodes[Index];
      TupleRoot.removeChild(Tuple);
      TupleRoot.insertBefore(Tuple, refNode)
    end
    else
      TupleRoot.appendChild(TupleRoot.removeChild(Tuple));
    (Tuple as IXMLDOMElement).setAttribute('Moved', VarNull);
    Tuple := nil;
  end;

var
  MemberIndex, TupleIndex, FixedIndex,
  LastTupleIndex, TuplesCount, LastMemberIndex, HowMany: integer;
  Tuples: IXMLDOMNodeList;
  MemberNode, FixedTuple, TupleRoot: IXMLDOMNode;
  UName, FixedUName: string;
  ItsaSummary, HasMoved: boolean;
  Level: TSheetLevelInterface;
  i: integer;
begin
  if not Assigned(Dom) then
    exit;
  if GetAxis(AxisType).Broken then
    exit;
  if AxisType = axColumn then
    TupleRoot := DOM.selectSingleNode('root/Axes/Axis[@name="Axis0"]/Tuples')
  else
    TupleRoot := DOM.selectSingleNode('root/Axes/Axis[@name="Axis1"]/Tuples');
  Tuples := TupleRoot.selectNodes('Tuple');
  TuplesCount := Tuples.length;
  if TuplesCount = 0 then
    exit;

  FixedIndex := 0;
  LastTupleIndex := TuplesCount - 1;
  LastMemberIndex := Tuples[0].childNodes.length - IIF(AxisType = axRow, 1, 2);
  HowMany := -1;
  for MemberIndex := 1 to LastMemberIndex do
  begin
    begin
      for TupleIndex := 0 to LastTupleIndex do
      begin
        { Если у кортежа стоит атрибут "скрытый", тогда пропускаем его}
        if IsTupleHidden(Tuples[TupleIndex]) then
          continue;
        MemberNode := MemberByNum(Tuples, TupleIndex, MemberIndex);
        ItsaSummary := IsItSummary(MemberNode);
        { Меры игнорируем}
        if IsItMeasure(MemberNode) then
          continue;

        { Неаккуратно. Если узел изначально принадлежал к явно не выбранному
          уровню, то получить настройки итогов не удастся.
          Через это могут возникнуть нестыковки отображения итогов в листе.}
        Level := GetLevelByMember(MemberNode.previousSibling, AxisType);
        if Assigned(Level) then
          if Level.SummaryOptions.Deployment = idTop then
            continue;

        HasMoved := Assigned(Tuples[TupleIndex].Attributes.getNamedItem('Moved'));
        {есть закладка - надо проверить, не пора ли ее перемещать}
        if Assigned(FixedTuple) then
        begin
          UName := GetParentUName(MemberNode);

          { В столбцах нужно перемещать всю секцию показателей, т.е.
              кол-во перемещаемых кортежей = кол-ву показателей}
          if AxisType = axColumn then
            if CompareTuples(FixedTuple, Tuples[TupleIndex], LastMemberIndex + 1) = -1 then
            begin
              inc(HowMany);
              continue;
            end;

          {если юникнеймы не совпадают, значит мы прошли последний кортеж диапазона
          именно после него и нужно вставить кортеж-закладку}
          if (UName <> FixedUName) or ItsaSummary or HasMoved then
          begin
            for i := 1 to HowMany do
            begin
              MoveSummaryTuple(TupleRoot, FixedTuple, TupleIndex, TuplesCount);
              Tuples := TupleRoot.selectNodes('Tuple');
              ShiftDataCells(Dom, AxisType, FixedIndex, TupleIndex - 1);
              FixedTuple := Tuples[FixedIndex];
            end;
            HowMany := -1;
            FixedTuple := nil;
          end;
        end;

        if HasMoved then
          continue;
        {ставим закладку - этот кортеж нужно будет переместить}
        if ItsaSummary and (HowMany = -1) then
        begin
          FixedTuple := Tuples[TupleIndex];
          FixedIndex := TupleIndex;
          FixedUName := GetParentUName(MemberNode);
          HowMany := 1;
        end;

      end;

      {если пробежали все кортежи, а закладка так и осталась, значит ее нужно
        переместить в самый конец списка}
      if Assigned(FixedTuple) then
      begin
        for i := 1 to HowMany do
        begin
          MoveSummaryTuple(TupleRoot, FixedTuple, LastTupleIndex + 1, TuplesCount);
          Tuples := TupleRoot.selectNodes('Tuple');
          ShiftDataCells(Dom, AxisType, FixedIndex, LastTupleIndex);
          FixedTuple := Tuples[FixedIndex];
        end;
        { Сразу по завершении этой операции мы перейдем к следующему мемберу
          и начнем цикл по кортежам с начала, поэтому надо сбросить закладку.}
        FixedTuple := nil;
        HowMany := -1;
        dec(LastTupleIndex);
      end;
    end;
  end;
  if AddinLogEnable then
    WriteDocumentLog(Dom, 'Cellset (после перемещения итогов).xml');
end;

procedure TSheetMaperCellset.HideDisabledSummaries(Dom: IXMLDOMDocument2; AxisType: TAxisType);
var
  NL: IXMLDOMNodeList;
  AxisIndex, LevelIndex, TupleIndex, MemberIndex: integer;
  MemberNode: IXMLDOMNode;
  Axis: TSheetAxisCollectionInterface;
begin
  Axis := GetAxis(AxisType);
  if Axis.Broken then
    exit;
  for AxisIndex := 0 to Axis.Count - 1 do
    for LevelIndex := 0 to Axis[AxisIndex].Levels.Count - 1 do
      if not Axis[AxisIndex].Levels[LevelIndex].SummaryOptions.Enabled then
      begin
        NL := GetAxisVisibleTuples(Dom, AxisType);
        MemberIndex := Axis.GetLevelNumber(AxisIndex, LevelIndex) + 1;
        for TupleIndex := 0 to NL.length - 1 do
        begin
          IncPB;
          MemberNode := NL[TupleIndex].childNodes[MemberIndex];
          if IsItSummary(MemberNode) then
            MarkTupleHidden(MemberNode);
        end;
      end;
end;

function TSheetMaperCellset.TupleHasSummary(Tuple: IXMLDOMNode): boolean;
var
  i: integer;
begin
  result := false;
  if not Assigned(Tuple) then
    exit;
  for i := 0 to Tuple.childNodes.length - 1 do
    if IsItSummary(Tuple.childNodes[i]) then
    begin
      result := true;
      exit;
    end;
end;

(*function TSheetMaperCellset.GetLevelByMemberIndex(AxisType: TAxisType;
  MemberIndex: integer): TSheetLevelInterface;
var
  AxisIndex: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  result := nil;
  Axis := GetAxis(AxisType);
  for AxisIndex := 0 to Axis.Count - 1 do
    if MemberIndex < Axis[AxisIndex].Levels.Count then
    begin
      result := Axis[AxisIndex].Levels[MemberIndex];
      exit;
    end
    else
      dec(MemberIndex, Axis[AxisIndex].Levels.Count);
end; *)

function TSheetMaperCellset.GetLevelByMember(Member: IXMLDOMNode;
  AxisType: TAxisType): TSheetLevelInterface;
var
  AxisIndex, LevelIndex: integer;
begin
  result := nil;
  if not Assigned(Member) then
    exit;
  AxisIndex := GetIntAttr(Member, attrAxisIndex, -1);
  LevelIndex := GetIntAttr(Member, attrLevelIndex, -1);
  if (AxisIndex = -1) or (LevelIndex = -1) then
    exit;
  try
    result := GetAxis(AxisType)[AxisIndex].Levels[LevelIndex];
  except
  end;
end;

procedure TSheetMaperCellset.MarkResultsGrey(Dom: IXMLDOMDocument2);
var
  RowTuples, ColumnTuples: IXMLDOMNodeList;
  Total: TSheetTotalInterface;
  ColumnIndex, RowIndex, i, j, x, y, StartRInd, EndRInd, StartCInd: integer;
  ColumnGrey, RowGrey: boolean;
  RowCoords, ColumnCoords: TIntegerArray;
begin
  if not Totals.CheckByType([wtResult, wtFree]) then
    exit;
  if PrintableStyle then
    exit;
  ColumnTuples := GetAxisTuples(Dom, axColumn);
  RowTuples := GetAxisTuples(Dom, axRow);
  StartRInd := Sizer.StartTotals.x;
  EndRInd := Sizer.EndTotals.x;
  StartCInd := Sizer.StartColumns.y;

  if ColumnTuples.length > 0 then
    for i := 0 to ColumnTuples.length - 1 do
    begin
      if IsTupleHidden(ColumnTuples[i]) then
        continue;
      Total := GetTotalByColumnTuple(ColumnTuples[i]);
      if not (Total.TotalType in [wtResult, wtFree]) then
        continue;
      ColumnGrey := GetBoolAttr(ColumnTuples[i], attrIsGrey, false);
      GetIntArrayAttr(ColumnTuples[i], attrColumnCoords, ColumnCoords);
      {!! Времянка на пару (!) дней.}
      if Length(ColumnCoords) = 0 then
      begin
        SetLength(ColumnCoords, 1);
        ColumnCoords[0] := GetIntAttr(ColumnTuples[i], 'cind', 0);
      end;
      if RowTuples.length > 0 then
        for j := 0 to RowTuples.length - 1 do
        begin
          if IsTupleHidden(RowTuples[j]) then
            continue;
          RowGrey := GetBoolAttr(RowTuples[j], attrIsGrey, false);
          if RowGrey or ColumnGrey then
          begin
            GetIntArrayAttr(RowTuples[j], attrRowCoords, RowCoords);
            {!! Времянка на пару (!) дней.}
            if Length(RowCoords) = 0 then
            begin
              SetLength(RowCoords, 1);
              RowCoords[0] := GetIntAttr(RowTuples[j], 'rind', 0);
            end;
            for x := 0 to Length(RowCoords) - 1 do
              for y := 0 to Length(ColumnCoords) - 1 do
              begin
                RowIndex := RowCoords[x];
                ColumnIndex := ColumnCoords[y];
                if (RowIndex >0) and (ColumnIndex > 0) then
                  GetRange(ExcelSheet, RowIndex, ColumnIndex).Interior.ColorIndex := 15;
              end;
          end;
        end
        else
          if ColumnGrey then
            for y := 0 to Length(ColumnCoords) - 1 do
            begin
              ColumnIndex := ColumnCoords[y];
              if (StartRInd >0) and (ColumnIndex > 0) then
                GetRange(ExcelSheet, StartRInd, ColumnIndex).Interior.ColorIndex := 15;
            end;
    end
  else
    for i := 0 to Totals.Count - 1 do
      if (Totals[i].TotalType in [wtFree, wtResult]) then
      GetRange(ExcelSheet, StartRInd, StartCInd + i,
        EndRInd, StartCInd + i).Interior.ColorIndex := 15;;
end;

procedure TSheetMaperCellset.GetWritableTotalsInfo;
begin

end;

procedure TSheetMaperCellset.MapTotalTitles(Dom: IXMLDOMDocument2);
var
  i, RHeight: integer;
  Total: TSheetTotalInterface;
  Body: ExcelRange;
  Tuples: IXMLDOMNodeList;
  Sections: array of integer;
  TotalIndex, TupleIndex: integer;
begin

  Tuples := GetAxisVisibleTuples(Dom, axColumn);
  WritablesInfo.ClearColumns;

  if Tuples.length = 0 then
  begin
    inherited MapTotalTitles;
    { Мне кажется, в таком случае не надо собирать WritablesInfo, потому что
      колонки показателей выводятся чисто для сохранения вменяемого вида
      таблицы, без привязки к оси... В обычном режиме ячейки пометятся серым.}
    exit;
  end;

  SetLength(Sections, Totals.Count);
  for i := 0 to Totals.Count - 1 do
    Sections[i] := -1;

  RHeight := Sizer.RowsHeight;
  if RHeight = 0 then
    RHeight := 1; // если нет оси строк, тогда показатель - одно число

  if Columns.Broken and not Columns.Empty then
    for i := 0 to Length(ColumnsProcessRoute) - 1 do
    begin
      TupleIndex := ColumnsProcessRoute[i] div Columns.Count;
      Total := GetTotalByColumnTuple(Tuples[TupleIndex]);
      if Total.TotalType in [wtResult, wtFree] then
        WritablesInfo.Add(Total, Lo(i));
      TotalIndex := Total.GetSelfIndex;
      inc(Sections[TotalIndex]);
      MapTotalTitle(Total, i + 1, Sizer.StartTotalsTitle.x,
        Sizer.StartTotalsTitle.y + i, RHeight, Sections[TotalIndex]);
    end
  else
    for i := 0 to Tuples.length - 1 do
    begin
      Total := GetTotalByColumnTuple(Tuples[i]);
      if Total.TotalType in [wtResult, wtFree] then
        WritablesInfo.Add(Total, Lo(i));
      TotalIndex := Total.GetSelfIndex;
      inc(Sections[TotalIndex]);
      MapTotalTitle(Total, i + 1, Sizer.StartTotalsTitle.x,
        Sizer.StartTotalsTitle.y + i, RHeight, Sections[TotalIndex]);
    end;

  { Помечаем общие области}
  if Totals.Count > 0 then
  begin
    {Все поле данных }
    Body := GetRange(ExcelSheet, Sizer.StartTotals, Sizer.EndTotals);
    MarkObject(ExcelSheet, Body, sntTotals, false);

    {Все заголовки показателей}
    if IsDisplayTotalsTitles then
    begin
      Body := GetRange(ExcelSheet, Sizer.StartTotalsTitle.x, Sizer.StartTotalsTitle.y,
        Sizer.StartTotalsTitle.x, Sizer.EndTotals.y);
      MarkObject(ExcelSheet, Body, sntTotalTitles, false);
    end;
  end;
end;

function TSheetMaperCellset.GetTotalByColumnTuple(Tuple: IXMLDOMNode): TSheetTotalInterface;
var
  Member: IXMLDOMNode;
  UName: string;
begin
  Member := Tuple.lastChild;
  UName := MemberUName(Member);
  CutPart(UName, '].[');
  Delete(UName, Length(UName), 1);
  result := Totals.FindByAlias(UName);
end;

procedure TSheetMaperCellset.MarkMember(ERange: ExcelRange; Member: IXMLDOMNode;
  MemberIndex: integer; AxisType: TAxisType);
var
  ElemID, UniqueIDList, MemberName: string;
  i: integer;
  TupleNode: IXMLDOMNode;
begin
  if not Assigned(Member) then
    exit;

  {Код элемента}
  if (AxisType = axRow) then
    ElemID := Rows[GetIntAttr(Member, attrAxisIndex, -1)].UniqueID
  else
    ElemID := Columns[GetIntAttr(Member, attrAxisIndex, -1)].UniqueID;

  TupleNode := Member.parentNode;
  UniqueIDList := '';

  {Цепочка UniqueID кортежа}
  for i := 0 to MemberIndex do
    UniqueIDList := UniqueIDList + snSeparator +
      MemberOrdinal(TupleNode.childNodes[i]);

  MemberName := sntMember + snSeparator + ElemID + snSeparator +
    BoolToStr(not GetBoolAttr(TupleNode, attrIsGrey, false)) + UniqueIDList;

  MarkObject(ExcelSheet, ERange, MemberName, false);
end;

procedure TSheetMaperCellset.MapBrokenAxis(DOM: IXMLDOMDocument2;
  AxisType: TAxisType);

  function GenerateMemberMarkupName(Tuple: IXMLDOMNode; MemberIndex: integer): string;
  var
    AxisElement: TSheetAxisElementInterface;
    AxisIndex, LevelIndex, i: integer;
    TupleMember, AxisMember: IXMLDOMNode;
    UName, XPath, Head, Tail, Part: string;
  begin
    result := '';
    Head := '';
    Tail := '';
    for AxisIndex := 0 to MemberIndex do
    begin
      if AxisType = axRow then
        AxisElement := Rows[AxisIndex]
      else
        AxisElement := Columns[AxisIndex];

      { По юникнейму мембера найдем его в дереве измерения}
      TupleMember := Tuple.childNodes[AxisIndex];
      UName := MemberUName(TupleMember);
      if UName = '' then
        exit;
      EncodeXPathString(UName);
      XPath := Format('function_result/Members//Member[@%s="%s"]', [attrUniqueName, UName]);
      AxisMember := AxisElement.Members.selectSingleNode(XPath);
      if not Assigned(AxisMember) then
        exit;

      LevelIndex := GetIntAttr(TupleMember, attrLevelIndex, -1);
      if LevelIndex = -1 then
        exit;

      { Идя от найденного мембера к корню, строим цепочку айди}
      Part := '';
      while AxisMember.nodeName <> 'Members' do
      begin
        Part := snSeparator + GetStrAttr(AxisMember, attrLocalId, '') + Part;
        AxisMember := AxisMember.parentNode;
      end;
      Tail := Tail + Part;

      { Для всех измерений, кроме последнего, надо добивать цепочку пустышками}
      if AxisIndex < MemberIndex then
        for i := LevelIndex to AxisElement.Levels.Count - 2 do
          Tail := Tail + snSeparator + fpDummy
      else
      Head := sntMember + snSeparator + AxisElement.UniqueID + snSeparator +
        BoolToStr(not GetBoolAttr(Tuple, attrIsGrey, false));
    end;

    result := BuildExcelName(Head + Tail);
  end;


  procedure MapMember(Tuples: IXMLDOMNodeList;
    TupleIndex, MemberIndex, Offset: integer);
  var
    ERange, OldRange: ExcelRange;
    RInd, CInd, Len: integer;
    Level: TSheetLevelInterface;
    Coords: TIntegerArray;
    Member: IXMLDOMNode;
    MemberName: string;
    tmpBool: boolean;
  begin
    if AxisType = axRow then
    begin
      RInd := Sizer.StartRows.x + Offset;
      CInd := Sizer.StartRows.y;
    end
    else
    begin
      RInd := Sizer.StartColumns.x;
      CInd := Sizer.StartColumns.y + Offset;
    end;
    Member := Tuples[TupleIndex].childNodes[MemberIndex];
    ERange := GetRange(ExcelSheet, RInd, CInd);
    WriteValue(RInd, CInd, MemberCaption(Member));

    { Форматирование по уровням}
    Level := GetLevelByMember(Member, AxisType);
    if Assigned(Level) then
    begin
      if PrintableStyle then
        ERange.Style := Level.AxisElement.Styles.Name[esValuePrint]
      else
        ERange.Style := Level.AxisElement.Styles.Name[esValue];
      if ((AxisType = axRow) and Rows.LevelsFormatting) or
        ((AxisType = axColumn) and Columns.LevelsFormatting) then
        ApplyLevelFormatting(ERange, Level);
      if (AxisType = axRow) and Rows.UseIndents then
        ERange.IndentLevel := Rows.GetLevelIndent(Level);
    end;

    {помечаем мембер}
    MemberName := GenerateMemberMarkupName(Tuples[TupleIndex], MemberIndex);
    OldRange := GetRangeByName(ExcelSheet, MemberName);
    if Assigned(OldRange) then
    begin
      tmpBool := ExcelSheet.Application.DisplayAlerts[FLCID];
      ExcelSheet.Application.DisplayAlerts[FLCID] := false;
      ERange := GetUnionRange(OldRange, ERange);
      ERange.Merge(false);
      ExcelSheet.Application.DisplayAlerts[FLCID] := tmpBool;
    end;
    MarkObject(ExcelSheet, ERange, MemberName, false);

    with (Member.parentNode as IXMLDOMElement) do
    begin
      setAttribute('rind', RInd);
      setAttribute('cind', CInd);

      { В серверном режиме при сломанной оси один и тот же кортеж может
        соответствовать нескольким. Для того, чтобы правильно работала,
        к примеру, раскраска серым, надо учитывать этот факт.
        Каждый раз, обращаясь к кортежу, перезаписываем массивы
        координат "его" ячеек.}
      GetIntArrayAttr(Member.parentNode, attrRowCoords, Coords);
      Len := Length(Coords) + 1;
      SetLength(Coords, Len);
      Coords[Len - 1] := RInd;
      SetIntArrayAttr(Member.parentNode, attrRowCoords, Coords);

      GetIntArrayAttr(Member.parentNode, attrColumnCoords, Coords);
      Len := Length(Coords) + 1;
      SetLength(Coords, Len);
      Coords[Len - 1] := CInd;
      SetIntArrayAttr(Member.parentNode, attrColumnCoords, Coords);
    end;
    SetLength(Coords, 0);
  end;

var
  Tuples: IXMLDOMNodeList;
  ProcessRoute: TIntegerArray;
  MemberIndex, TupleIndex, Offset, AxisSize: integer;
  Axis: TSheetAxisCollectionInterface;
begin
  Tuples := GetAxisVisibleTuples(DOM, AxisType);
  if Tuples.length = 0 then
    exit;

  Axis := GetAxis(AxisType);
  AxisSize := Axis.Count;
  if AxisType = axRow then
    ProcessRoute := RowsProcessRoute
  else
    ProcessRoute := ColumnsProcessRoute;

  for Offset := 0 to Length(ProcessRoute) - 1 do
  begin
    TupleIndex := ProcessRoute[Offset] div AxisSize;
    MemberIndex := ProcessRoute[Offset] mod AxisSize;
    MapMember(Tuples, TupleIndex, MemberIndex, Offset);
  end;
end;

procedure TSheetMaperCellset.HideTuplesByCounterparts(
  Dom: IXMLDOMDocument2; AxisType: TAxisType);
var
  TupleIndex, CounterTupleIndex, CellIndex: integer;
  Tuples, CounterTuples, DataCells: IXMLDOMNodeList;
  Tuple, CounterTuple: IXMLDOMNode;
  CounterAttribute: string;
  NeedHide: boolean;
begin
  Tuples := GetAxisTuples(Dom, AxisType);
  if AxisType = axRow then
    CounterTuples := GetAxisTuples(Dom, axColumn)
  else
    CounterTuples := GetAxisTuples(Dom, axRow);

  for TupleIndex := 0 to Tuples.length - 1 do
  begin
    Tuple := Tuples[TupleIndex];
    if IsTupleHidden(Tuple) then
      continue;

    { Получим список ячеек, ссылающихся на текущий кортеж. Из каждой ячейки
      достаем ссылку на кортеж по другой оси и проверяем его на скрытие.
      Если нет ни одного нескрытого, то скрываем текущий обрабатываемый кортеж.}
    DataCells := Dom.selectNodes(Format('root/CellData/Cell[@%s="%d"]',
      [IIF(AxisType = axRow, attrRowIndex, attrColumnIndex), TupleIndex]));
    CounterAttribute := IIF(AxisType = axRow, attrColumnIndex, attrRowIndex);
    NeedHide := true;
    for CellIndex := 0 to DataCells.length - 1 do
    begin
      CounterTupleIndex := GetIntAttr(DataCells[CellIndex], CounterAttribute, -1);
      if CounterTupleIndex = -1 then
        continue;
      CounterTuple := CounterTuples[CounterTupleIndex];
      NeedHide := IsTupleHidden(CounterTuple);
      if not NeedHide then
        break;
    end;
    if NeedHide then
      SetAttr(Tuple, attrHiddenTuple, VarNull);
  end;
end;

function TSheetMaperCellset.AllFromSameProvider: boolean;
var
  ProviderId: string;
  i: integer;
begin
  // коллекция показателей обязана быть не пустой
  ProviderId := Totals[0].ProviderId;
  result := true;

  for i := 1 to Totals.Count - 1 do
    if Totals[i].ProviderId <> ProviderId then
    begin
      result := false;
      exit;
    end;

  for i := 0 to Rows.Count - 1 do
    if Rows[i].ProviderId <> ProviderId then
    begin
      result := false;
      exit;
    end;

  for i := 0 to Columns.Count - 1 do
    if Columns[i].ProviderId <> ProviderId then
    begin
      result := false;
      exit;
    end;

  for i := 0 to Filters.Count - 1 do
    if Filters[i].ProviderId <> ProviderId then
    begin
      result := false;
      exit;
    end;
end;

end.

