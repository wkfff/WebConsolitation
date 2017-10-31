{
  Реализация уровня рамещения коневого объекта модели.
  На уровне размещения, происходит отображение расчитанной объектной модели
  в рабочий лист Excel.
  Проще говоря, здесь лист планирования учится рисовать себя на рабочем листе
  Экселя.
  Класс uSheetMaper по прежнему остался абстрактным и не может непосредственно
  использоваться в плагине. Использоваться могут только его потомки.

}
unit uSheetMaper;

interface

uses
  SysUtils, ExcelXP, MSXML2_TLB, Classes, Windows,
  PlaningProvider_TLB,  uSheetStyles, uSheetSizer, uXMLCatalog,
  uFMExcelAddInConst, uXMLUtils, Graphics,
  PlaningTools_TLB, uFMAddinGeneralUtils, uSheetHistory, uMSWordUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils,
  uSheetAxes, uSheetLogic, uSheetObjectModel, uSheetBreaks,
  uExcelUtils, uGlobalPlaningConst, uOfficeUtils;

type

  TValuePack = record
    Total: TSheetTotalInterface;
    Value: string;
    CommentOk: boolean;
    FormattedValue: extended;
    IsExistFormula: boolean;
    Formula: string;
    InitialValue: string;
  end;


  { лист, который умеет размещаться }
  TSheetMaper = class(TSheetLogic)
  private
    //кэш запросов для отдельных ячеек
    SSQueryCache: TStringList;
    //список индексов скрытых столбцов
    HiddenColumns: TByteSet;
    HiddenTotalColumns: TByteSet;
    { Вынужденная мера - надо знать, не является ли текущий рефреш листа
      подготовкой к переводу его в автономный режим.}
    FIsLastRefreshBeforeOffline: boolean;
    {Еще более вынужденный трюк - принудительное отключение событий
      ExcelApplication, нужное в частности, при создании копии листа в процессе
      мапинга "широкой" таблицы.}
    FNeedHostEventsDisabled: boolean;
    {В режиме "широкой таблицы" склеенная ось столбцов постепенно режется
      на куски, которые и размещаются на отдельных листах. Для корректного
      подсчета итогов будем на время мапинга сохранять исходную полную ось.}
    FFullColumnAxis: IXMLDOMDocument2;
    {}
    FRowsDom, FColumnsDom, FRowsMarkup, FColumnsMarkup, FCellsMarkup: IXMLDOMDocument2;
    FSizer: TSheetSizer;


    // размещение данных показателей
    procedure MapTotalsData(RowsDOM, ColumnsDOM: IXMLDOMDocument2);
    { Рисуем тело оси}
    procedure MapAxis(DataDOM: IXMLDOMDocument2; Axis: TAxisType);
    //подставляет в лист значения итогов только по видимым элементам
    procedure MapSummaries(RowsDOM, ColumnsDOM: IXMLDOMDocument2; SingleTotal: TSheetTotalInterface);
    {размещает на листе отдельную ячейку}
    function MapSingleCell(CellIndex: integer; var ErrorText: string;
      var BadSingles: TStringList; RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
    // разместить показатели константы на лист
    procedure MapTotalConsts;
    // разместить отдельные константы
    procedure MapCellConsts;
    { Размещает свойства элементов оси}
    procedure MapMemberProperties(Axis: TAxisType; AxisDOM: IXMLDOMDocument2);

    {удаляет из готового ДОМа оси все элементы, которым не сопоставлены
      данные показателей}
    procedure RemoveEmpty(AxisDom: IXMLDOMDocument2; AxisType: TAxisType);
    procedure RemoveEmptyForBrokenAxis(AxisDom: IXMLDOMDocument2; AxisType: TAxisType);

    {если запрос для отдельной был кэширован, то вернет его результат}
    function GetSSQueryCached(Query: string; out AValue: string): boolean;
    function GetTotalByCInd(CInd: integer): TSheetTotalInterface;
    {удаление отдельных ячеек, которые утратили способность к размещению}
    procedure RemoveBadSingles(Indexes: TStringList);
    procedure GetHiddenColumns;
    procedure DoHideColumns;
    procedure SetAxisNodesCoords(Dom: IXMLDOMDocument2; AxisType: TAxisType);

    { применяет к указанной области дополнительное форматирование по уровням оси
      0 - показатели, 1 - МП строк, 2 - МП столбцов}
    procedure StyleAreaByLevels(AxisType: TAxisType; AxisDom: IXMLDOMDocument2; What: integer);
    {красит серым те ячейки области показателей-результатов, для которых
      обратная запись не имеет смысла}
    procedure MarkResultsGrey(RowsDom, ColumnsDom: IXMLDOMDocument2);
    {сохраняет в комментариях к ячейкам размещенные значения из базы}
    procedure CommentBaseValues;
    {Выводит единицу измерения если нужно}
    procedure WriteUnitMarker;
    // делает пометку в листе если он создаётся на тестовой версии
    procedure ShowTestVersionWarning;

    {Помечаем всю таблицу}
    procedure MarkAllTable;
    procedure MarkImportArea;
    {}
    procedure MarkTableHeading;
    //размещение области идентификации листа
    procedure MarkSheetId;
    procedure MarkBreaks;
    procedure MarkAreas;

    {Получение данных показателей для таблицы - обертка для QueryTotalsData}
    function GetTableData: boolean;
    {Сборка осей - обертка для GetFullAxisDOM и RemoveEmpty}
    function BuildAxes: boolean;
    {Обертка для Sizer.Init}
    function InitSizer(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
    function DoMapTable(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
    {Размещение таблицы, не влезающей в лист по ширине - задача 6742}
    function MapWideTable(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
    procedure DeleteAddendSheets(Sheet: ExcelWorkSheet);

    {Подготавливает разметочную информацию для процедур сбора. Данный механизм
      введен в версии 2.3.3 как замена разметки именованными диапазонами -
      на больших листах коллекция из десятков тысяч имен тормозит чудовищно,
      а обращений к ней огромное количество.}
    procedure MakeMarkupDocument(AxisType: TAxisType; AxisDom: IXMLDOMDocument2;
      StartFrom: integer);
    {Сохраняет разметочную информацию для отдельных показателей внутри таблицы}
    procedure MakeCellsMarkup;
    procedure PrepareMarkupDocuments(RowsDom, ColumnsDom: IXMLDOMDocument2);

    function LookUpMemberNodes(DataNode: IXMLDOMNode;
      AxisDOM: IXMLDOMDocument2; AxisType: TAxisType): IXMLDOMNodeList;
    procedure GetBasePointsOld(RowsDOM, ColumnsDOM: IXMLDOMDocument2;
      DataNode: IXMLDOMNode; out BasePoints, BasePointsIgnored: TPoints);
    procedure OneValueForEntireColumn(var BasePoints: TPoints);
    function GetFormula(RowsDom, ColumnsDom: IXMLDOMDocument2; FormulaNode: IXMLDOMNode): string;

    function LookUpMemberNodesNew(DataNode: IXMLDOMNode;
      AxisType: TAxisType): IXMLDOMNodeList;
    procedure GetBasePointsNew(DataNode: IXMLDOMNode; out BasePoints,
      BasePointsIgnored: TPoints; CountIgnored: integer);

    {очистка диапазона одного показателя с оглядкой на отдельные показатели}
    procedure ClearOneTotalsCells(Total: TSheetTotalInterface);
    procedure CommentOneTotalSections(Total: TSheetTotalInterface);

    {размещает в сетке данные показателей}
    procedure MapTotalsDataNodes(RowsDOM, ColumnsDOM: IXMLDOMDocument2; NL: IXMLDOMNodeList; UseMarkup: boolean);

    {вспомогательные методы, выделенные из MapTotalsDataNodes}
    function GetTotalToMap(Attr: IXMLDOMNode; var ValuePack: TValuePack): boolean;
    procedure GetTotalFormulaToMap(Node: IXMLDOMNode; var ValuePack: TValuePack);
    procedure GetRealPoints(var ValuePack: TValuePack; BasePoints, BasePointsIgnored: TPoints; out RealPoints: TPoints);
    procedure CheckTotalValueToMap(var ValuePack: TValuePack);
    procedure DoMapTotalValue(var ValuePack: TValuePack; CurPoint: TPoint);
    procedure CommentMappedValue(ValuePack: TValuePack; DataNode: IXMLDOMNode; CurPoint: TPoint);
    procedure MapTotalValue(var ValuePack: TValuePack; RealPoints: TPoints; DataNode: IXMLDOMNode);

    procedure FormatTotalColumn(Total: TSheetTotalInterface; Range: ExcelRange);
  protected
    // id-интерфейса
    FLCID: integer;
    {индикатор режима мапинга таблицы с продолжением на нескольких листах}
    FWideTableMode: boolean;
    {И наконец, полный ахтунг - ЕЩЕ один флаг для мапинга широких таблиц.}
    FWideTableSummaryMode: boolean;

    {Управляющие параметры коллектора. Перенесены сюда для возможности
      модификации их значениями из сайзера.}
    FFirstRow, FLastRow, FFirstColumn, FLastColumn: integer;
    // столбец, на которой размещены листья на оси строк
    FRowsLeaf: integer;
    // строка, на которой размещены листья на оси столбцов
    FColumnsLeaf: integer;
    FGrandSummaryRow, FGrandSummaryColumn: integer;

    {}
    procedure MarkAxis(Axis: TAxisType; AxLenght: integer);
    {Рисуем заголовки уровней оси}
    procedure MapAxisTitles(Axis: TAxisType);
    {Размещаем комментарии к заголовкам осей}
    procedure MapAxisTitlesComments(Axis: TAxisType);
    {Мапим заголовок одного показателя}
    procedure MapTotalTitle(Total: TSheetTotalInterface;
      ColumnNum, RInd, CInd, RowsWidth, Instance: integer);
    { Мапим заголовки всех показателей }
    procedure MapTotalTitles;
    procedure MapTotalTitlesEx(BeyondColumns: boolean);
    procedure MapTypeFormulas;

    {размещает на листе область фильтров}
    function MapFilters: boolean;
    {Полное размещение оси - обертка для MapAxis и MapMemberProperties}
    function MapFullAxis(AxisType: TAxisType; AxisDom: IXMLDOMDocument2): boolean;
    {Полное размещение данных показателей}
    function MapFullTotals(RowsDom, ColumnsDom: IXMLDOMDocument2; BeyondColumns: boolean): boolean;
    function DoFinalMapping: boolean;
    {Отрисовка таблицы}
    function MapTable: boolean; virtual;

    procedure WriteMaskedValue(RInd, CInd: integer; Value, Mask: string); overload;
    procedure WriteMaskedValue(Range: OleVariant; Value, Mask: string); overload;
    procedure MarkMPArea(AxisType: TAxisType);
    procedure ApplyLevelFormatting(ERange: ExcelRange; Level: TSheetLevelInterface);
    procedure ApplySummaryFormatting(ERange: ExcelRange; SummaryOptions: TSummaryOptions);
    { Собирает сведения о диапазонах показателей в таблице, доступных для записи}
    procedure GetWritableTotalsInfo;
    { Собирает сведения о диапазонах отельных показателей, доступных для записи}
    procedure GetCellsNamesInfo;
    procedure ClearTestVersionWarning;
    procedure UpdatePossibleNumberValue(var Value: string);

    property RowsDom: IXMLDOMDocument2 read FRowsDom write FRowsDom;
    property ColumnsDom: IXMLDOMDocument2 read FColumnsDom write FColumnsDom;
    property RowsMarkup: IXMLDOMDocument2 read FRowsMarkup write FRowsMarkup;
    property ColumnsMarkup: IXMLDOMDocument2 read FColumnsMarkup write FColumnsMarkup;
    property CellsMarkup: IXMLDOMDocument2 read FCellsMarkup write FCellsMarkup;
    property Sizer: TSheetSizer read FSizer write FSizer;
  public
    constructor Create;
    destructor Destroy; override;
    {Влезает ли таблица в лист. Параметром передаются расчитанные размеры}
    function CheckSheetSize(out TooWide: boolean): boolean;
    // размещает данные на листе
    procedure MapAll;
    //очистка таблицы по областям
    procedure ClearTableEx;
    {размещает на листе указанное множество отдельных ячеек}
    function MapSingleCells(Indexes: TStringList; out ErrorText: string;
      Standalone: boolean; RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
    {возвращает объединенный диапазон перезаписываемых отдельных ячеек}
    function GetSingleCellsUnitedRange: ExcelRange;
    {возврашает объединенный диапазон всех отдельных ячеек}
    function GetAllSingleCellsUnitedRange: ExcelRange;
    {применяет к диапазонам элементов их стили
      RestyleList содержит строки вида UniqueId=TSheetElementType}
    procedure ApplyElementStyles(RestyleList: TStringList);
    {разместить константы}
    procedure MapConsts;
    procedure UpdateFiltersText; override;
    procedure UpdateTotalsComments; override;
    procedure PrepareToSwitchOffline;
    procedure UpdateAllowEditRanges;

    constructor CreateInherited(Parent: TSheetMaper);

    function IsSummaryRow(Row: integer): boolean; override;
    function IsSummaryColumn(Column: integer): boolean; override;
    function IsSummaryCell(Row, Column: integer): boolean; override;

    procedure Clear; override;

    {Обновление одного выбранного пользователем показателя, без
      перестроения всей таблицы}
    function MapOneTotal(Total: TSheetTotalInterface): boolean;

    property NeedHostEventsDisabled: boolean read FNeedHostEventsDisabled;
  end;

implementation

const
  attrMustDie = 'mustdie';
  {Для именования дополнительных листов, на которых размещена "широкая" таблица.
    Задача 6742. Так как само слово "продолжение" не является слишком уж редким
    и может быть задано пользователем, то для уникальности все буквы р, о, е
    в нем сделаны английскими.}
  Addend = '_п';//'_пpoдoлжeниe_';


constructor TSheetMaper.Create;
begin
  inherited Create;
  FLCID := GetUserDefaultLCID;
  FNeedHostEventsDisabled := false;
  Sizer := TSheetSizer.Create;
end;

constructor TSheetMaper.CreateInherited(Parent: TSheetMaper);
var
  Dom: IXMLDOMDocument2;
  InnerDataNode: IXMLDOMNode;
begin
  Create;
  Dom := NewMetaDataDom;
  InnerDataNode := Dom.createNode(1, 'innerdata', '');
  Dom.documentElement.appendChild(InnerDataNode);
  Parent.SaveInnerData(InnerDataNode);
  LoadInnerData(InnerDataNode);
  KillDomDocument(Dom);
end;

destructor TSheetMaper.Destroy;
begin
  FProcess := nil;
  inherited Destroy;
  FreeAndNil(FSizer);
end;

{Если у оси строк не размещаются заголовки, то комментарий к нии рисуем на
первой строчке елементов этих измерений.}
procedure TSheetMaper.MapAxisTitlesComments(Axis: TAxisType);
var
  i, j, RInd, CInd, ElementCount, LevelCount: integer;
  AxisElement: TSheetAxisElementInterface;
begin
  if not IsDisplayCommentStructuralCell then
    exit;
  if (Axis = axColumn) and not IsDisplayColumnsTitles then
    exit;

  ElementCount := GetAxis(Axis).Count;

  case Axis of
    axRow: begin
      RInd := Sizer.StartRowsTitle.x;
      CInd := Sizer.StartRowsTitle.y;
    end;
    axColumn: begin
      RInd := Sizer.StartColumnsTitle.x;
      CInd := Sizer.StartColumnsTitle.y;
    end
    else exit;
  end;

  if GetAxis(Axis).Broken then
  begin
    if (Axis = axRow) and not IsDisplayRowsTitles then
      CommentCell(ExcelSheet, Sizer.StartRows.x, CInd, GetAxis(Axis).CommentText)
    else
      CommentCell(ExcelSheet, RInd, CInd, GetAxis(Axis).CommentText);
  end
  else
  begin
    for i := 0 to ElementCount - 1 do
    begin
      AxisElement := GetAxis(Axis)[i];
      if AxisElement.IgnoreHierarchy then
        LevelCount := 1
      else
        LevelCount := AxisElement.Levels.Count;

      for j := 0 to LevelCount - 1 do
      begin
        if (Axis = axRow) and not IsDisplayRowsTitles then
          AddCellComment(ExcelSheet, Sizer.StartRows.x, CInd, AxisElement.CommentText)
        else
          CommentCell(ExcelSheet, RInd, CInd, AxisElement.CommentText);
        inc(CInd);
      end;
    end;
  end;
end;

procedure TSheetMaper.MapAxisTitles(Axis: TAxisType);
var
  i, j, RInd, CInd, ElementCount, LevelCount: integer;
  AxisElement: TSheetAxisElementInterface;
  TitlesMarkName: string;
  StartTitlesCInd: integer;
begin
  {Если у размещаемой оси не стоит признака отображения заголовков, то выходим}
  if not(((Axis = axColumn) and IsDisplayColumnsTitles)
    or ((Axis = axRow) and IsDisplayRowsTitles)) then
    exit;

  ElementCount := GetAxis(Axis).Count;
  if ElementCount = 0 then
    exit;
  case Axis of
    axRow:
      begin
        RInd := Sizer.StartRowsTitle.x;
        CInd := Sizer.StartRowsTitle.y;
        TitlesMarkName := sntRowTitles;
      end;
    axColumn:
      begin
        RInd := Sizer.StartColumnsTitle.x;
        CInd := Sizer.StartColumnsTitle.y;
        TitlesMarkName := sntColumnTitles;
      end
    else exit;
  end;
  StartTitlesCInd := CInd; //сохраняем для разметки всей области заголовков

  if GetAxis(Axis).Broken then
  begin
    with ExcelSheet.Cells do
    begin
      Item[RInd, CInd].Value := IIF(Axis = axRow, 'Строки', 'Столбцы');
      Item[RInd, CInd].Style := IIF(PrintableStyle ,
        GetAxis(Axis)[0].Styles.Name[esTitlePrint],
        GetAxis(Axis)[0].Styles.Name[esTitle]);
    end;
    inc(CInd);
  end
  else
    for i := 0 to ElementCount - 1 do
    begin
      AxisElement := GetAxis(Axis)[i];
      try
        if AxisElement.IgnoreHierarchy then
          LevelCount := 1
        else
          LevelCount := AxisElement.Levels.Count;

        for j := 0 to LevelCount - 1 do
          with ExcelSheet.Cells do
          begin
            if AxisElement.IgnoreHierarchy then
              Item[RInd, CInd].Value := AxisElement.FullDimensionName2
            else
              Item[RInd, CInd].Value := AxisElement.Levels[j].Name;

            { накладываем стиль}
            Item[RInd, CInd].Style := IIF(PrintableStyle ,
              AxisElement.Styles.Name[esTitlePrint],
              AxisElement.Styles.Name[esTitle]);
            (* Эту возможность решили не реализовывать
            {отдельные стили для уровней}
            Item[RInd, CInd].Style := IIF(PrintableStyle ,
              AxisElement.Levels[j].Styles.Name[esTitlePrint],
              AxisElement.Levels[j].Styles.Name[esTitle]);*)

            MarkObject(ExcelSheet, GetRange(ExcelSheet, RInd, CInd, RInd, CInd),
                AxisElement.Levels[j].TitleExcelName, false);
            inc(CInd);
          end;
      finally
      end;
    end;

  {Поменим всю область заголовков оси}
  if (CInd > StartTitlesCInd) then //если вообще были заголовки
  begin
    dec(CInd);
    MarkObject(ExcelSheet, GetRange(ExcelSheet, RInd, StartTitlesCInd, RInd, CInd),
      TitlesMarkName, false);
  end;
end;

procedure TSheetMaper.MapAxis(DataDOM: IXMLDOMDocument2; Axis: TAxisType);


  procedure MapNode(Node: IXMLDOMNode; StartRInd, StartCInd: integer;
                   ParentLocalId: string; var Progress: integer; const MaxProgress: integer);

    function GetRealParentName(Node: IXMLDOMNode): string;
    begin
      result := '';
      repeat
        if not Assigned(Node.parentNode) then
          break;
        Node := Node.parentNode;
        if Node.nodeName = 'Members' then
        begin
          result := '';
          exit;
        end;
        result := GetStrAttr(Node, attrRangeName, '')
      until result <> ''
    end;

    procedure WriteCoords(Node: IXMLDOMNode; RInd, CInd: integer);
    var
      NodeInFullColumns: IXMLDOMNode;
      NodeId: integer;
    begin
      SetAttr(Node, 'rind', RInd);
      SetAttr(Node, 'cind', CInd);
      if not (FWideTableMode and (Axis = axColumn))then
        exit;
      NodeId := GetIntAttr(Node, attrNodeId, 0);
      NodeInFullColumns := FFullColumnAxis.selectSingleNode(
        Format('function_result/Members//*[@%s="%d"]', [attrNodeId, NodeId]));
      SetAttr(NodeInFullColumns, 'rind', RInd);
      SetAttr(NodeInFullColumns, 'cind', CInd);
      SetAttr(NodeInFullColumns, 'sheetname', ExcelSheet.Name);
    end;

  var
    i, ln, RInd, CInd: integer;
    NL: IXMLDOMNodeList;
    NodeName, RangeName, LocalId: string;
    Range: ExcelRange;
    AxisIndex, LevelIndex: integer;
  begin
    RInd := StartRInd;
    CInd := StartCInd;
    NL := Node.childNodes;
    for i := 0 to NL.length - 1 do
    begin
      if not ((NL[i].nodeName = ntMember) or (NL[i].nodeName = ntSummary)) then
        continue;
      RangeName := '';
      NodeName := GetStrAttr(NL[i], attrName, '');
      ln := LeafCount(NL[i], Sizer.CellSize(Axis));
      if ln <= 0 then
        continue;
      ExcelSheet.Cells.Item[RInd, CInd].Value := NodeName;
      inc(Progress);
      SetPBarPosition(Progress, MaxProgress);
      RangeName := GetStrAttr(NL[i], attrRangeName, '');
      WriteCoords(NL[i], RInd,CInd);
      if LocalId = '' then
        LocalId := ParentLocalId + snSeparator + fpDummy;
      case Axis of
        axRow:
          begin
            Range := MergeCells(ExcelSheet, RInd, CInd, RInd + ln - 1, CInd);

            {Старая разметка подразумевает именованные диапазоны для каждого
            мембера, в новой нужно лишь обозначить общие итоги}
            if not IsMarkupNew then
              MarkObject(ExcelSheet, Range, RangeName, false)
            else
              if (NL[i].nodeName = ntSummary) then
                if ((RangeName = BuildExcelName(gsRow)) or (RangeName = BuildExcelName(gsColumn))) then
                  MarkObject(ExcelSheet, Range, RangeName, false);

            if NL[i].hasChildNodes then
              MapNode(NL[i], RInd, CInd + 1, LocalId, Progress, MaxProgress);
            RInd := RInd + ln;

            AxisIndex := GetIntAttr(NL[i], 'axisindex', -1);
            if AxisIndex <> -1 then
            begin
              if Rows.Broken or (NL[i].nodeName = ntSummary) then
                Range.Style := IIF(PrintableStyle,
                Rows[AxisIndex].Styles.Name[esValuePrint],
                Rows[AxisIndex].Styles.Name[esValue]);
              LevelIndex := GetIntAttr(NL[i], 'levelindex', -1);
              if LevelIndex <> -1 then
              begin
                {дополнительное форматирование по уровням оси}
                if Rows.LevelsFormatting then
                  ApplyLevelFormatting(Range, Rows[AxisIndex].Levels[LevelIndex]);

                {индивидуальное форматирование итогов}
                if GetNodeType(NL[i]) = ntSummary then
                  if IsGrandSummary(NL[i]) then
                    ApplySummaryFormatting(Range, Rows.GrandSummaryOptions)
                  else
                    ApplySummaryFormatting(Range, Rows[AxisIndex].Levels[LevelIndex].SummaryOptions);

                {для итогов не надо применять отступы}
                if Rows.UseIndents and (NL[i].nodeName <> ntSummary) then
                  Range.IndentLevel := Rows.GetLevelIndent(AxisIndex, LevelIndex);
              end;
            end;

          end;
        axColumn:
          begin
            Range := MergeCells(ExcelSheet, RInd, CInd, RInd, CInd + ln - 1);
            if not IsMarkupNew then
              MarkObject(ExcelSheet, Range, RangeName, false);
            if NL[i].hasChildNodes then
              MapNode(NL[i], RInd + 1, CInd, LocalId, Progress, MaxProgress);
            CInd := CInd + ln;

            AxisIndex := GetIntAttr(NL[i], 'axisindex', -1);
            if AxisIndex <> -1 then
            begin
              if Columns.Broken or (NL[i].nodeName = ntSummary) then
                Range.Style := IIF(PrintableStyle,
                Columns[AxisIndex].Styles.Name[esValuePrint],
                Columns[AxisIndex].Styles.Name[esValue]);
              LevelIndex := GetIntAttr(NL[i], 'levelindex', -1);
              if LevelIndex <> -1 then
              begin
                {дополнительное форматирование по уровням оси}
                if Columns.LevelsFormatting then
                  ApplyLevelFormatting(Range, Columns[AxisIndex].Levels[LevelIndex]);

                {индивидуальное форматирование итогов}
                if GetNodeType(NL[i]) = ntSummary then
                  if IsGrandSummary(NL[i]) then
                    ApplySummaryFormatting(Range, Columns.GrandSummaryOptions)
                  else
                    ApplySummaryFormatting(Range, Columns[AxisIndex].Levels[LevelIndex].SummaryOptions);
              end;
            end;
          end;
      end;
    end;
  end;

var
  Node: IXMLDOMNode;
  AxSize: integer;
  StartRInd, StartCInd, Progress, MaxProgress: integer;
  FileName: string;
begin
  if not Assigned(dataDOM) then
    exit;

  MapAxisTitles(Axis);
  MapAxisTitlesComments(Axis);

  Node := dataDOM.selectSingleNode('function_result/Members');

  if not Assigned(Node) then
    exit;

  if (Axis = axColumn) then
  begin
    AxSize := Sizer.ColumnsWidth;
    StartRInd := Sizer.StartColumns.x;
    StartCInd := Sizer.StartColumns.y;
    OpenOperation(pfoMapColumns, CriticalNode, not NoteTime, otMap);
  end else
  begin
    AxSize := Sizer.RowsHeight;
    StartRInd := Sizer.StartRows.x;
    StartCInd := Sizer.StartRows.y;
    OpenOperation(pfoMapRows, CriticalNode, not NoteTime, otMap);
  end;

  {помечаем
  Чтобы формат, заданный в стилях применялся правильно, накладывать
  стили нужно ДО того, как выводить значения в ячейки}
  MarkAxis(Axis, AxSize);
  // размещаем элементы
  Progress := 0;
  MaxProgress := Node.ownerDocument.selectNodes('//*[@unique_name]').length;
  MapNode(Node, StartRInd, StartCInd, '', Progress, MaxProgress);
  CloseOperation;
  if AddinLogEnable then
  begin
    if Axis = axRow then
      FileName := 'Ось_строк_после_размещения.xml'
    else
      FileName := 'Ось_столбцов_после_размещения.xml';
    OpenOperation(pfoLogSave + AddinLogPath + FileName, not CriticalNode,
      not NoteTime, otSave);
    if WriteDocumentLog(DataDOM, FileName)then
      CloseOperation
    else
    begin
      PostMessage(pfoLogSaveFailed + AddinLogPath + FileName, msgWarning);
      CloseOperation;
    end;
  end;
end;

procedure TSheetMaper.MarkAxis(Axis: TAxisType; AxLenght: integer);
var
  StartRInd, StartCInd, EndRInd, EndCInd: integer;
  Range: ExcelRange;
  AxisTypeName: string;
  i, CurElemInd, CurLevelInd, ElemOffset, ElemWidth: integer;
  AxElement: TSheetAxisElementInterface;
  FieldStartInd, FieldEndInd: integer;
begin
  //!!! Хуевая, неоптимальная процедура, не учитывает новые веяния

  if GetAxis(Axis).Count <= 0 then
    exit;
  { вычисляем размеры всего }
  case Axis of
    axRow:
      begin
        StartRInd := Sizer.StartRows.x;
        StartCInd := Sizer.StartRows.y;
        EndRInd := Sizer.EndRows.x;
        EndCInd := Sizer.EndRows.y;

        AxisTypeName := sntRows;
        FieldStartInd := StartCInd;
        FieldEndInd := EndCInd;
      end;
    axColumn:
      begin
        StartRInd := Sizer.StartColumns.x;
        StartCInd := Sizer.StartColumns.y;
        EndRInd := Sizer.EndColumns.x;
        EndCInd := Sizer.EndColumns.y;

        AxisTypeName := sntColumns;
        FieldStartInd := StartRInd;
        FieldEndInd := EndRInd;
      end;
    else
      exit;
  end;

  if not GetAxis(Axis).Broken then
  begin
    { размечаем уровни }
    AxElement := GetAxis(Axis)[0];
    CurElemInd := 0;
    CurLevelInd := 0;
    try
      for i := FieldStartInd to FieldEndInd do
      begin
        if CurLevelInd >= AxElement.Levels.Count then
        begin // переходим к следующему компоненту
          inc(CurElemInd);
          AxElement := GetAxis(Axis)[CurElemInd];
          CurLevelInd := 0;
        end;
        case Axis of
          axRow:
            begin
              Range := GetRange(ExcelSheet, StartRInd, i, EndRInd, i);
              { установка параметров диапазона }
              if AxElement.Levels[CurLevelInd].ColumnWidth > 0 then
                Range.ColumnWidth := AxElement.Levels[CurLevelInd].ColumnWidth;
            end;
          axColumn: Range := GetRange(ExcelSheet, i, StartCInd, i, EndCInd);
        end;
        MarkObject(ExcelSheet, Range, AxElement.Levels[CurLevelInd].ExcelName, false);
        inc(CurLevelInd);
      end;
    finally
    end;

    { размечаем компоненты }
    ElemOffset := 0;
    for i := 0 to GetAxis(Axis).Count - 1 do
    begin
      AxElement := GetAxis(Axis)[i];
      if AxElement.IgnoreHierarchy then
        ElemWidth := 1
      else
        ElemWidth := AxElement.Levels.Count;

      (*  Индивидуальные стили уровней были запрещены
      for CurLevelInd := 0 to ElemWidth - 1 do
      begin
        case Axis of
          axRow:
              Range := GetRange(ExcelSheet, StartRInd, StartCInd + ElemOffset + CurLevelInd,
                EndRInd, StartCInd + ElemOffset + CurLevelInd);
          axColumn:
              Range := GetRange(ExcelSheet, StartRInd + ElemOffset + CurLevelInd, StartCInd,
                StartRInd + ElemOffset + CurLevelInd, EndCInd);
        end;
        Range.Style :=
        IIF(PrintableStyle, AxElement.Levels[CurLevelInd].Styles.Name[esValuePrint],
          AxElement.Levels[CurLevelInd].Styles.Name[esValue]);
      end;*)

      case Axis of
        axRow:
            Range := GetRange(ExcelSheet, StartRInd, StartCInd + ElemOffset,
              EndRInd, StartCInd + ElemOffset + ElemWidth - 1);
        axColumn:
            Range := GetRange(ExcelSheet, StartRInd + ElemOffset, StartCInd,
              StartRInd + ElemOffset + ElemWidth - 1, EndCInd);
      end;
      Range.Style := IIF(PrintableStyle, AxElement.Styles.Name[esValuePrint],
        AxElement.Styles.Name[esValue]);
      MarkObject(ExcelSheet, Range, AxElement.ExcelName, false);
      ElemOffset := ElemOffset + ElemWidth;
    end;
  end;

  {размечаем всю ось }
  {!!! Не оптимально - все уже посчитано в сайзере}
  Range := GetRange(ExcelSheet, StartRInd, StartCInd, EndRInd, EndCInd);
  MarkObject(ExcelSheet, Range, AxisTypeName, false);

  if GetAxis(Axis).Broken then
  begin
    AxElement := GetAxis(Axis)[0];
    Range.Style := IIF(PrintableStyle, AxElement.Styles.Name[esValuePrint],
      AxElement.Styles.Name[esValue]);
  end;

  if (Axis = axColumn)  and (not IsDisplayColumns) then
  begin
    Range.Cells.Locked := true;
    exit;
  end;
end;

{Новый вариант использует собранную разметку вместо осей}
function TSheetMaper.LookUpMemberNodesNew(DataNode: IXMLDOMNode;
  AxisType: TAxisType): IXMLDOMNodeList;
var
  MarkupDom: IXMLDOMDocument2;
  i: integer;
  XPath, CurrentAlias, UniqueName: string;
  AxColl: TSheetAxisCollectionInterface;
begin
  result := nil;
  AxColl := GetAxis(AxisType);
  if AxisType = axRow then
    MarkupDom := RowsMarkup
  else
    MarkupDom := ColumnsMarkup;

  XPath := '';
  for i := 0 to AxColl.Count - 1 do
  begin
    CurrentAlias := AxColl[i].Alias;
    if Assigned(DataNode.attributes.getNamedItem(CurrentAlias)) then
    begin
      AddTail(XPath, ' and ');
      UniqueName := GetStrAttr(DataNode, CurrentAlias, '');
      EncodeXPathString(UniqueName);
      XPath := XPath + Format('(@%s="%s")', [CurrentAlias, UniqueName]);
    end;
  end;

  if XPath <> '' then
    XPath := Format('markup/%s[collect[%s]]', [IIF(AxisType = axRow, 'row', 'column'), XPath])
  else
    XPath := Format('markup/%s[collect]', [IIF(AxisType = axRow, 'row', 'column')]);
  result := MarkupDom.selectNodes(XPath);
end;

procedure TSheetMaper.GetBasePointsNew(DataNode: IXMLDOMNode;
  out BasePoints, BasePointsIgnored: TPoints; CountIgnored: integer);
var
  i, j, Index: integer;
  RowsNL, ColumnsNL: IXMLDOMNodeList;
  RPoints, CPoints, CPointsIgnored: array of integer;
begin
  SetLength(RPoints, 1);
  RPoints[0] := 0;
  SetLength(CPoints, 1);
  CPoints[0] := 0;
  SetLength(CPointsIgnored, 1);
  CPointsIgnored[0] := 0;


  //получим индексы строк
  if not Rows.Empty and Assigned(RowsMarkup) then
  begin
    RowsNL := LookUpMemberNodesNew(DataNode, axRow);
    if Assigned(RowsNL) then
      if RowsNL.length > 0 then
      begin
        SetLength(RPoints, RowsNL.length);
        for i := 0 to RowsNL.length - 1 do
          RPoints[i] := GetIntAttr(RowsNL[i], attrShift, 0) + FFirstRow;
      end;
  end
  else
    RPoints[0] := FFirstRow; //Sizer.StartTotals.x;
  //получим индексы столбцов
  if not Columns.Empty and Assigned(ColumnsMarkup) then
  begin
    ColumnsNL := LookUpMemberNodesNew(DataNode, axColumn);
    if Assigned(ColumnsNL) then
      if ColumnsNL.length > 0 then
      begin
        SetLength(CPoints, ColumnsNL.length);
        for i := 0 to ColumnsNL.length - 1 do
          CPoints[i] := GetIntAttr(ColumnsNL[i], attrShift, 0) + FFirstColumn;
      end;
    CPointsIgnored[0] := FLastColumn - CountIgnored + 1;//Sizer.EndColumns.y + 1
  end
  else
  begin
    CPoints[0] := FFirstColumn; //Sizer.StartTotals.y;
    CPointsIgnored[0] := FFirstColumn; //Sizer.StartTotals.y;
  end;

  //результат - декартово произведение
  SetLength(BasePoints, Length(RPoints) * Length(CPoints));
  for i := 0 to Length(RPoints) - 1 do
    for j := 0 to Length(CPoints) - 1 do
    begin
      Index := i * Length(CPoints) + j;
      BasePoints[Index].x := RPoints[i];
      BasePoints[Index].y := CPoints[j];
    end;
  SetLength(BasePointsIgnored, Length(RPoints) * Length(CPointsIgnored));
  for i := 0 to Length(RPoints) - 1 do
    for j := 0 to Length(CPointsIgnored) - 1 do
    begin
      Index := i * Length(CPointsIgnored) + j;
      BasePointsIgnored[Index].x := RPoints[i];
      BasePointsIgnored[Index].y := CPointsIgnored[j];
    end;
end;


function TSheetMaper.LookUpMemberNodes(DataNode: IXMLDOMNode;
  AxisDOM: IXMLDOMDocument2; AxisType: TAxisType): IXMLDOMNodeList;
var
  AxColl: TSheetAxisCollectionInterface;
  XPath, UniqueName, CurrentAlias: string;
  i: integer;
  PlacedToGS: boolean;
begin
  result := nil;
  AxColl := GetAxis(AxisType);
  if not (Assigned(AxisDOM) and Assigned(AxColl)) then
    exit;
  PlacedToGS := (AxisType = axRow) and Assigned(DataNode.attributes.getNamedItem(gsRow)) or
        (AxisType = axColumn) and Assigned(DataNode.attributes.getNamedItem(gsColumn));
  {Собираем XPath для выборки узла}
  if AxColl.Broken then
  begin
    XPath := '';
    for i := 0 to AxColl.Count - 1 do
    begin
      CurrentAlias := AxColl[i].Alias;
      if Assigned(DataNode.attributes.getNamedItem(CurrentAlias)) then
      begin
        AddTail(XPath, ' and ');
        UniqueName := GetStrAttr(DataNode, CurrentAlias, '');
        EncodeXPathString(UniqueName);
        XPath := XPath + Format('(%s[@%s="%s"])', [ntAliasInfo, CurrentAlias, UniqueName]);
      end;
    end;
    if XPath <> '' then
    begin
      {Для задачи 8311}
      XPath := IIF(PlacedToGS,
        Format('function_result/Members/Summary[%s]', [XPath]),
        Format('function_result/Members/*[%s]', [XPath]));
    end
    else
      XPath := 'function_result/Members/*';
  end
  else
  begin
    XPath := '//Members';
    for i := 0 to AxColl.Count - 1 do
    begin
      XPath := XPath + Copy(FunnyStr, 1, 2 * AxColl[i].FieldCount);

      { Непонятно, какое должно быть условие в случае нераскладываемости.
        Из-за этого не работает опция "выводить только в главный итог".}
      if not Assigned(DataNode.attributes.getNamedItem(AxColl[i].Alias)) then
        continue;

      UniqueName := GetStrAttr(DataNode, AxColl[i].Alias,  '');
      EncodeXPathString(UniqueName);
      XPath := XPath + Format('[@%s="%s"]', [attrUniqueName, UniqueName]);
    end;
    {пропускаем узлы с незаполненными UN}
    if (Pos('""', XPath) > 0) then
      exit;

    {если показатель вообще не раскладывается по оси, то пишем его в
    главный итог}
    if PlacedToGS or (Pos('[@unique_name="', XPath) = 0) then
    begin
      if AxColl.GrandSummaryOptions.Enabled then
        XPath := StringReplace(XPath, '/*', '/Summary', []);
    end;
  end;
  result := AxisDOM.selectNodes(XPath);
end;

{Возвращает по узлу данных ячейку в листе, откуда следует
 размещать данную группу показателей. Если позиция не будет
 найдена вернется (0,0)}
procedure TSheetMaper.GetBasePointsOld(RowsDOM, ColumnsDOM: IXMLDOMDocument2;
  DataNode: IXMLDOMNode; out BasePoints, BasePointsIgnored: TPoints);
var
  i, j, Index: integer;
  RowsNL, ColumnsNL: IXMLDOMNodeList;
  RPoints, CPoints, CPointsIgnored: array of integer;
begin
  SetLength(RPoints, 1);
  RPoints[0] := 0;
  SetLength(CPoints, 1);
  CPoints[0] := 0;
  SetLength(CPointsIgnored, 1);
  CPointsIgnored[0] := 0;


  //получим индексы строк
  if Assigned(RowsDOM) then
  begin
    RowsNL := LookUpMemberNodes(DataNode, RowsDOM, axRow);
    if Assigned(RowsNL) then
      if RowsNL.length > 0 then
      begin
        SetLength(RPoints, RowsNL.length);
        for i := 0 to RowsNL.length - 1 do
          RPoints[i] := GetIntAttr(RowsNL[i], 'rind', 0);
      end;
  end
  else
    RPoints[0] := Sizer.StartTotals.x;
  //получим индексы столбцов
  if Assigned(ColumnsDOM) then
  begin
    ColumnsNL := LookUpMemberNodes(DataNode, ColumnsDOM, axColumn);
    if Assigned(ColumnsNL) then
      if ColumnsNL.length > 0 then
      begin
        SetLength(CPoints, ColumnsNL.length);
        for i := 0 to ColumnsNL.length - 1 do
          CPoints[i] := GetIntAttr(ColumnsNL[i], 'cind', 0);
      end;
    CPointsIgnored[0] := Sizer.EndColumns.y + 1
  end
  else
  begin
    CPoints[0] := Sizer.StartTotals.y;
    CPointsIgnored[0] := Sizer.StartTotals.y;
  end;

  //результат - декартово произведение
  SetLength(BasePoints, Length(RPoints) * Length(CPoints));
  for i := 0 to Length(RPoints) - 1 do
    for j := 0 to Length(CPoints) - 1 do
    begin
      Index := i * Length(CPoints) + j;
      BasePoints[Index].x := RPoints[i];
      BasePoints[Index].y := CPoints[j];
    end;
  SetLength(BasePointsIgnored, Length(RPoints) * Length(CPointsIgnored));
  for i := 0 to Length(RPoints) - 1 do
    for j := 0 to Length(CPointsIgnored) - 1 do
    begin
      Index := i * Length(CPointsIgnored) + j;
      BasePointsIgnored[Index].x := RPoints[i];
      BasePointsIgnored[Index].y := CPointsIgnored[j];
    end;
end;

{здесь идут вспомогательные методы, выделенные из TSheetMaper.MapTotalsDataNodes}

function TSheetMaper.GetTotalToMap(Attr: IXMLDOMNode; var ValuePack: TValuePack): boolean;
var
  AName: string;
begin
  result := false;

  AName := Attr.nodeName;
  if AName[1] <> 'T' then
    exit;
  ValuePack.Value := Attr.nodeValue;
  ValuePack.Total := Totals.FindByAlias(AName);
  result := Assigned(ValuePack.Total);
end;

{если ли у показателя формула}
procedure TSheetMaper.GetTotalFormulaToMap(Node: IXMLDOMNode; var ValuePack: TValuePack);
var
  FormulaNode: IXMLDOMNode;
  IsExistTypeFormula: boolean;
begin
  with ValuePack do
  begin
    Formula := '';
    IsExistFormula := false;

    FormulaNode := Node.selectSingleNode('./' + Total.Alias + snSeparator + 'formula');
    IsExistFormula := Assigned(FormulaNode);
    IsExistTypeFormula := Total.TypeFormula.Enabled and (Total.TypeFormula.Template <> '');
    if IsExistFormula then
    begin
      Formula := GetFormula(RowsDom, ColumnsDom, FormulaNode);
      IsExistFormula := (Pos('=', Formula) = 1) or IsExistTypeFormula;
    end;
  end;
end;

procedure TSheetMaper.GetRealPoints(var ValuePack: TValuePack; BasePoints, BasePointsIgnored: TPoints; out RealPoints: TPoints);
begin
  with ValuePack do
  begin
    if Total.IsIgnoredColumnAxis then
      RealPoints := BasePointsIgnored
    else
      RealPoints := BasePoints;

    if Total.IsIgnoredRowAxis and  not(Total.IsGrandTotalDataOnly or IsExistFormula) then
      OneValueForEntireColumn(RealPoints);
  end;
end;

procedure TSheetMaper.CheckTotalValueToMap(var ValuePack: TValuePack);
begin
  with ValuePack do
  begin
    InitialValue := Value;
    if Total.Format = fmtBoolean then
    begin
      if (Value = '') then
        Value := '0';
      if Value <> '0' then
        Value := '1';
      exit;
    end;

    if not IsNumber(Value) then
      UpdatePossibleNumberValue(Value);
    if IsNumber(Value) then
    begin
      FormattedValue := StrToFloat(Value);
      FormattedValue := Total.GetDividedValue(FormattedValue);
      Total.Round(FormattedValue); //округлим согласно показателю
    end
    else
    begin
      Value := InitialValue;
      CommentOk := false;
    end;
  end;
end;

procedure TSheetMaper.DoMapTotalValue(var ValuePack: TValuePack; CurPoint: TPoint);
var
  Cell: ExcelRange;
  CellValue, CellFormula, CellStyle: string;
begin
  Cell := GetRange(ExcelSheet, CurPoint.x, CurPoint.y);
  {если есть - то выводим формулу.
    В случае типовой формулы заход в эту ветку будет фиктивным, ради того, чтобы не зайти в альтернативную}
  with ValuePack do
  begin
    if IsExistFormula then
    begin
      if (Pos('=', Formula) = 1) then
        SetCellFormula(ExcelSheet, Cell, Formula);
    end
    else
    begin
      GetCellValue(Cell, '', CellValue, CellFormula, CellStyle, false);
      {если нет формулы - выводим значение}
      if (Value <> '') then
        if not ((Total.TotalType = wtResult) and (CellValue <> '')
          and (CellValue <> Total.EmptyValueSymbol) and (CellFormula <> '')) then
          if IsNumber(Value) and (Total.Format <> fmtBoolean) then
            Cell.Value2 := FormattedValue
          else
            Cell.Value2 := Value;
    end;

    if not PrintableStyle then
      if not IsSummaryCell(CurPoint.x, CurPoint.y) then
        if Total.IsTypeFormulaException(CurPoint.x, CurPoint.y) then
        begin
          Cell.Interior.PatternColorIndex := 32;
          Cell.Interior.Pattern := xlGray16;
        end;
  end;
end;

procedure TSheetMaper.CommentMappedValue(ValuePack: TValuePack; DataNode: IXMLDOMNode; CurPoint: TPoint);
var
  Comment: string;
begin
  {к показателям типа "результат" добавляем комментарий - значение из базы}
  with ValuePack do
    if (Total.TotalType = wtResult) and CommentOK and IsDisplayCommentDataCell then
    begin
      Comment := Format('Значение%sиз базы%s: ',
        [IIF(GetBoolAttr(DataNode, mnIsRowLeaf, false) and
        GetBoolAttr(DataNode, mnIsColumnLeaf, false), ' показателя ', ' итога '),
        IIF(Total.SummariesByVisible, '', ' (по всем элементам)')]);
      AddCellComment(ExcelSheet, CurPoint.x, CurPoint.y, Comment + InitialValue);
    end;
end;

procedure TSheetMaper.MapTotalValue(var ValuePack: TValuePack; RealPoints: TPoints; DataNode: IXMLDOMNode);
var
  PointIndex: integer;
  CurPoint: TPoint;
begin
  for PointIndex := 0 to Length(RealPoints) - 1 do
  begin
    {Определяем смещение показателя}
    CurPoint := RealPoints[PointIndex];
    if (CurPoint.x <= 0) or (CurPoint.y <= 0) then
      continue;
    inc(CurPoint.y, ValuePack.Total.Ordinal);

    {Если в ячейке располагается отдельный показатель, ничего в ней не размещаем}
    if WritablesInfo.IsSingleCellSelected(ExcelSheet, CurPoint.x, CurPoint.y) then
      continue;

    CheckTotalValueToMap(ValuePack);
    DoMapTotalValue(ValuePack, CurPoint);
    CommentMappedValue(ValuePack, DataNode, CurPoint);
  end;
end;

procedure TSheetMaper.MapTotalsDataNodes(RowsDOM, ColumnsDOM: IXMLDOMDocument2;
  NL: IXMLDOMNodeList; UseMarkup: boolean);
var
  i, j, Len: integer;
  BasePoints, BasePointsIgnored, RealPoints: TPoints;
  Attributes: IXMLDOMNamedNodeMap;
  CountIgnored: integer;
  ValuePack: TValuePack;
begin
  Len := NL.length;
  CountIgnored := Totals.CountWithPlacement(true);
  for i := 0 to Len - 1 do
  begin
    SetPBarPosition(i + 1, Len);

    if UseMarkup then
      GetBasePointsNew(NL[i], BasePoints, BasePointsIgnored, CountIgnored)
    else
      GetBasePointsOld(RowsDom, ColumnsDom, NL[i], BasePoints, BasePointsIgnored);

    Attributes := NL[i].attributes;
    for j := 0 to Attributes.length - 1 do
    begin
      FillChar(ValuePack, SizeOf(ValuePack), 0);
      ValuePack.CommentOk := true;

      {перебираем атрибуты с показателями, прочие пропускаем}
      if not GetTotalToMap(Attributes[j], ValuePack) then
        continue;

      GetTotalFormulaToMap(NL[i], ValuePack);
      GetRealPoints(ValuePack, BasePoints, BasePointsIgnored, RealPoints);
      MapTotalValue(ValuePack, RealPoints, NL[i]);
    end;
  end;
end;

procedure TSheetMaper.OneValueForEntireColumn(var BasePoints: TPoints);
var
  i, j, RowCount, Index: integer;
  NewBasePoints: TPoints;
begin
  RowCount := Sizer.EndTotals.x - Sizer.StartTotals.x + 1;
  SetLength(NewBasePoints, Length(BasePoints) * RowCount);
  for i := 0 to Length(BasePoints) - 1 do
    for j := 0 to RowCount - 1 do
    begin
      Index := RowCount * i + j;
      NewBasePoints[Index].x := Sizer.StartTotals.x + j;
      NewBasePoints[Index].y := BasePoints[i].y;
    end;
  BasePoints := NewBasePoints;
end;

function TSheetMaper.GetFormula(RowsDom, ColumnsDom: IXMLDOMDocument2;
  FormulaNode: IXMLDOMNode): string;
var
  Template, ParamName, DecodedParam, TotalParam: string;
  ParamNode: IXMLDOMNode;
  i, TotalIndex: integer;
  BPoints, BPointsIgnored: TPoints;
  RInd, CInd: integer;
  ColumnName, RowName: string;
  Total: TSheetTotalInterface;
begin
  result := '';
  // если формулы нет - выходим
  if not Assigned(FormulaNode) then
    exit;
  Template := GetStrAttr(FormulaNode, 'template', '');
  // проходим по всем закодированным параметрам формулы
  for i := 0 to FormulaNode.childNodes.length - 1 do
  begin
    ParamNode := FormulaNode.childNodes.item[i];
    ParamName := ParamNode.baseName;

    // получаем показатель
    TotalParam := GetStrAttr(ParamNode, 'total', '');
    TotalIndex := Totals.FindByID(TotalParam);
    if TotalIndex < 0 then
      exit;
    Total := Totals[TotalIndex];
    // получаем координаты закодированнной ячейки
    GetBasePointsOld(RowsDom, ColumnsDom, ParamNode, BPoints, BPointsIgnored);
    if Total.IsIgnoredColumnAxis then
      BPoints := BPointsIgnored;
    // проверка на нахождение точки
    if (BPoints[0].x = 0) or (BPoints[0].y = 0) then
    begin
      Template := StringReplace(Template, ParamName, fmIncorrectRef, [rfReplaceAll]);
      continue;
    end;
    RInd := BPoints[0].x;
    CInd := BPoints[0].y + Total.Ordinal;
    ColumnName := GetColumnName(CInd);
    RowName := IntToStr(RInd);
    // составляем раскодированный параметр
    DecodedParam := ColumnName + RowName;
    // заменяем параметр на раскодированный
    Template := StringReplace(Template, ParamName, DecodedParam, [rfReplaceAll]);
  end;
  result := Template;
end;


procedure TSheetMaper.MapTotalsData(RowsDOM, ColumnsDOM: IXMLDOMDocument2);


  // получить диапазон сложной формулы
  function GetComplexFormulaRange(Node: IXmlDomNode): ExcelRange;
  var
    TotalAlias: string;
    Total: TSheetTotalInterface;
    BPoints, BPointsIgnored: TPoints;
    CInd: integer;
  begin
    result := nil;
    TotalAlias := GetStrAttr(Node, 'totalAlias', '');
    Total := Totals.FindByAlias(TotalAlias);
    if not Assigned(Total) then
      exit;
    GetBasePointsOld(RowsDom, ColumnsDom, Node, BPoints, BPointsIgnored);
    if Total.IsIgnoredColumnAxis then
      BPoints := BPointsIgnored;
    if (BPoints[0].y = 0) then
      exit;
    CInd := BPoints[0].y + Total.Ordinal;
    result := Total.GetTotalRangeWithoutGrandSummaryByColumn(CInd);
  end;

  // размещает FormulaArray на лист
  procedure MapFormulaArrays;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    Formula: string;
    FormulaNode: IXmlDomNode;
    TotalRange: ExcelRange;
  begin
    if not Assigned(FormulaArrays) then
      exit;
    NL := FormulaArrays.selectNodes('function_result/data/complexFormula');
    for i := 0 to NL.length - 1 do
    begin
      TotalRange := GetComplexFormulaRange(NL[i]);
      if not Assigned(TotalRange) then
        continue;
      FormulaNode := NL[i].selectSingleNode('./' + 'value');
      Formula := GetFormula(RowsDom, ColumnsDom, FormulaNode);
      if Assigned(TotalRange) then
        TotalRange.FormulaArray := Formula;
    end;
  end;

  {Перемаркирует отдельные ячейки, размещенные внутри таблицы, c целью
    сохранения их привязки к элементам осей}
  procedure ShiftBindedSingles;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    BP, BPI: TPoints;
    Point: TPoint;
    strAlias: string;
    Cell: TSheetSingleCellInterface;
    eRange: ExcelRange;
    Total: TSheetTotalInterface;
    CellRange: ExcelRange;
  begin
    if not Assigned(SingleCellsData) then
      exit;
    NL := SingleCellsData.selectNodes('function_result/data/row');
    for i := 0 to NL.length - 1 do
    begin
      if not GetBoolAttr(NL[i], 'inTable', false) then
        continue;

      strAlias := GetStrAttr(NL[i], attrAlias, '');
      Cell := SingleCells.FindByAlias(strAlias);
      if not Assigned(Cell) then
        continue;

      GetBasePointsOld(RowsDom, ColumnsDom, NL[i], BP, BPI);

      CellRange := GetRangeByName(ExcelSheet, Cell.ExcelName);
      if Assigned(CellRange) then
        if (CellRange.column > Sizer.EndColumns.y) then
          BP :=  BPI;

      Point := BP[0];
      {сбилась привязка к осям - удаляем отдельную}
      if (Point.x = 0) or (Point.y = 0) then
      begin
        GetNameObject(ExcelSheet, Cell.ExcelName).Delete;
        continue;
      end;

      strAlias := GetStrAttr(NL[i], 'totalalias', '');
      Total := Totals.FindByAlias(strAlias);
      {был удален показатель в столбце которого размещалась отдельная}
      if not Assigned(Total) then
      begin
        GetNameObject(ExcelSheet, Cell.ExcelName).Delete;
        continue;
      end;
      Inc(Point.y, Total.Ordinal);

      eRange := GetRange(ExcelSheet, Point, Point);
      MarkObject(ExcelSheet, ERange, Cell.ExcelName, false);
    end;
    if not NeedMapSingleCells then
      NeedMapSingleCells := true;
  end;

var
  NL: IXMLDOMNodeList;
begin
  // сначала размещаем константы
  MapTotalConsts;
  if not IsSequelSheet(ExcelSheet) then
    ShiftBindedSingles;
  if not Assigned(TotalsData) then
    exit;

  OpenOperation(pfoMapTotals, CriticalNode, NoteTime, otMap);

  MapFormulaArrays;
  {т.к. структура таблицы могла измениться, обновим координаты отдельных ячеек}
  if not IsSequelSheet(ExcelSheet) then
    WritablesInfo.UpdateSingleCellsPoints(ExcelSheet);
  MapTypeFormulas;

  NL := TotalsData.selectNodes('function_result/data/row');
  MapTotalsDataNodes(RowsDom, ColumnsDom, NL, false);
  CloseOperation; //pfoMapTotals
end;

{ Проверяет, убирается таблица в лист }
function TSheetMaper.CheckSheetSize(out TooWide: boolean): boolean;
var
  LastColumn, LastRow: integer;
begin
  LastColumn := SheetColumnsLimitation;
  LastRow := ExcelSheet.Rows.Count;
  with Sizer.EndTable do
  begin
    result := (x < LastRow) and (y <= LastColumn);
    TooWide := (x < LastRow) and (y > LastColumn);
  end;
end;

procedure TSheetMaper.FormatTotalColumn(Total: TSheetTotalInterface; Range: ExcelRange);
var
  ValueList: string;
begin
  try
    Range.Style := IIF(PrintableStyle, Total.Styles.Name[esValuePrint],
      Total.Styles.Name[esValue]);

    if Total.Format = fmtText then
    begin {$WARNINGS OFF}
      Range.WrapText := true;
      Range.HorizontalAlignment := xlLeft;
    end; {$WARNINGS ON}

    if Total.Format = fmtBoolean then
    try
      Range.Validation.Delete;
      ValueList := Format('0%s1%s%s', [ListSeparator, ListSeparator, Total.EmptyValueSymbol]);
      Range.Validation.Add(xlValidateList, EmptyParam, xlBetween, ValueList, EmptyParam);
      with Range.Validation do
      begin
        IgnoreBlank := true;
        InCellDropdown := false;
        ShowInput := true;
        ShowError := true;
      end;
    except
    end;

    {накладываем на диапазон символы пустого значения}
    Range.Value[EmptyParam] := Total.EmptyValueSymbol;
  except
  end;
  {!!! раскомментировано, чтобы не пропадал формат у пустых
  показателей-результатов(когда в базе нет значений). См. структуру цикла
  в процедуре MapTotalsData}
  if not VarIsNull(Total.NumberFormat) then
  try
    Range.NumberFormat := Total.NumberFormat;
  except
  end;

  if Total.ColumnWidth > 0 then
    Range.ColumnWidth := Total.ColumnWidth;
end;

{Мапим заголовок одного показателя}
procedure TSheetMaper.MapTotalTitle(Total: TSheetTotalInterface;
  ColumnNum, RInd, CInd, RowsWidth, Instance: integer);
var
  Caption: string;
  Body, Title, AllSectionsBody, AllSectionsTitle: ExcelRange;
  NameType, NameTypeTitle: string;
begin
  Caption := Total.Caption;
  if not Total.SummariesByVisible then
  begin
    if (Caption[1] <> '=') then
      Caption := Caption + ' *'
    else
      Caption := Caption + '&" *"';
  end;

  case Total.TotalType of
    wtFree:
      begin
        NameType := sntTotalFree;
        NameTypeTitle := sntTotalFreeTitle;
      end;
    wtMeasure:
      begin
        NameType := sntTotalMeasure;
        NameTypeTitle := sntTotalMeasureTitle;
      end;
    wtResult:
      begin
        NameType := sntTotalResult;
        NameTypeTitle := sntTotalResultTitle;
      end;
    wtConst:
      begin
        NameType := sntTotalConst;
        NameTypeTitle := sntTotalConstTitle;
      end;
    else Caption := '';
  end;

  { заголовок }
  if IsDisplayTotalsTitles then
  begin
    Title := GetRange(ExcelSheet, RInd, CInd, RInd, CInd);
    try
      Title.Value2 := Caption;
    except
      Title.Value2 := 'Ошибка в формуле';
      PostMessage('Ошибка в формуле заголовка показателя: ' + Caption, msgWarning);
    end;
    try
      Title.Style := IIF(PrintableStyle,
        Total.Styles.Name[esTitlePrint], Total.Styles.Name[esTitle]);
    except
    end;
    MarkObject(ExcelSheet, Title, Total.TitleExcelName + snSeparator + IntToStr(Instance),
      false);
    if Total.ColumnWidth > 0 then
      Title.ColumnWidth := Total.ColumnWidth;
    {диапазон, объединяющий заголовки всех секций показателя}
    AllSectionsTitle := GetRangeByName(ExcelSheet, Total.TitleExcelName);
    if Assigned(AllSectionsTitle) then
      AllSectionsTitle := GetUnionRange(AllSectionsTitle, Title)
    else
      AllSectionsTitle := Title;
    MarkObject(ExcelSheet, AllSectionsTitle, Total.TitleExcelName, false);
  end;

  if IsDisplayCommentStructuralCell then
    {Если заголовки показателей не отображаются, то комметарий к ним помещаем в
    первую ячйку данных}
    CommentCell(ExcelSheet, IIF(IsDisplayTotalsTitles, RInd, Sizer.StartTotals.x),
      CInd, Total.CommentText);

  { тело }
  Body := GetRange(ExcelSheet, Sizer.StartTotals.x, CInd, Sizer.EndTotals.x, CInd);
  {диапазон, объединяющий заголовки всех секций показателя}
  AllSectionsBody := GetRangeByName(ExcelSheet, Total.ExcelName);
  if Assigned(AllSectionsBody) then
    AllSectionsBody := GetUnionRange(AllSectionsBody, Body)
  else
    AllSectionsBody := Body;
  MarkObject(ExcelSheet, AllSectionsBody, Total.ExcelName, false);

  FormatTotalColumn(Total, Body);

  MarkObject(ExcelSheet, Body, Total.GetFullExcelName(Instance), false);
  MarkUserObject(ExcelSheet, Body, Total.GetUserExcelName(Instance), false);

  if Total.IsHidden then
    Include(HiddenTotalColumns, CInd - 1);

  {При отключенной/отсутствующей надстройке области редактируемых показателей
    не должны быть заблокированы для ввода (ведь лист защищается при сохранении,
    чтобы избежать нежелательных изменений). В режиме конструирования
    разблокируем все свободные и результаты. В режиме работы с данными и при
    подготовке формы сбора - только те, у которых выставлено разрешение на
    редактирование.}
  if not IsSequelSheet(ExcelSheet) then
    if (Total.TotalType in [wtResult, wtFree]) and Total.MayBeEdited then
      ExcelSheet.Protection.AllowEditRanges.Add(Total.GetFullExcelName(Instance), Body, EmptyParam);
end;

procedure TSheetMaper.MapTotalTitlesEx(BeyondColumns: boolean);
var
  i, TotalCount, CWidth, RHeight: integer;
  Total: TSheetTotalInterface;
  Body: ExcelRange;
begin
  TotalCount := Totals.CountWithPlacement(false);

  CWidth := Sizer.ColumnsWidth;
  if CWidth = 0 then // нет оси колонок -> показатели выводятся один раз
    CWidth := TotalCount;

  RHeight := Sizer.RowsHeight;
  if RHeight = 0 then
    RHeight := 1; // если нет оси строк, тогда показатель - одно число

  FTotalSections.Clear;
  {Сначала пробегаем по итогам, которые вписываются в ось}
  if TotalCount > 0 then
    for i := 0 to CWidth - 1 do
    begin
      Total := Totals.GetWithPlacement(false, (i mod TotalCount));
      MapTotalTitle(Total, i + 1, Sizer.StartTotalsTitle.x,
        Sizer.StartTotalsTitle.y + i, RHeight, (i div TotalCount));
      FTotalSections.Add(Total.UniqueID + '_' + IntToStr(i div TotalCount));
    end
  else
    for i := 0 to CWidth - 1 do
      FTotalSections.Add('0_0');

  {Теперь пробегаем по итогам за осью}
  if BeyondColumns then
  begin
    TotalCount := Totals.CountWithPlacement(true);
    if TotalCount > 0 then
      for i := 0 to TotalCount - 1 do
      begin
        Total := Totals.GetWithPlacement(true, i);
        MapTotalTitle(Total, CWidth + i + 1, Sizer.StartTotalsTitle.x,
          Sizer.StartTotalsTitle.y + CWidth + i, RHeight, 0);
        FTotalSections.Add(Total.UniqueID + '_0');
      end;
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

{ Мапим заголовки всех показателей }
procedure TSheetMaper.MapTotalTitles;
begin
  MapTotalTitlesEx(false);
end;

{Помечаем всю таблицу}
procedure TSheetMaper.MarkAllTable;
var tmpRange: ExcelRange;
begin
  if not Sizer.TableIsEmpty then
  begin
    tmpRange := GetRange(ExcelSheet, Sizer.StartTable, Sizer.EndTable);
    MarkObject(ExcelSheet, tmpRange, sntTable, false);
    tmpRange := GetRange(ExcelSheet, Sizer.StartTable, Sizer.EndTableWithoutId);
    MarkUserObject(ExcelSheet, tmpRange, sntUserTable, true);
    MarkObject(ExcelSheet, tmpRange, sntTableWitoutID, false);
  end;
end;

{Выводит единицу измерения если нужно}
procedure TSheetMaper.WriteUnitMarker;
var
  MP: TPoint;
  tmpRange: ExcelRange;
begin
  if MarkerPosition = mpHidden then
    exit;
  if TotalMultiplier > tmE1 then
  begin
    MP := Sizer.UnitMarker;
    if (MP.x > 0) and (MP.y > 0) then //нет итогов - нет единицы измерения
    begin
      tmpRange := GetRange(ExcelSheet, MP.x, MP.y, MP.x, MP.y);
      MarkObject(ExcelSheet, tmpRange, sntUnitMarker, false);
      case TotalMultiplier of
        tmE3: ExcelSheet.Cells.Item[MP.x, MP.y].Value := fmThousandRubles;
        tmE6: ExcelSheet.Cells.Item[MP.x, MP.y].Value := fmMlnRubles;
        tmE9: ExcelSheet.Cells.Item[MP.x, MP.y].Value := fmMlrdRubles;
      end;
      ExcelSheet.Cells.Item[MP.x, MP.y].Font.bold := true;
      ExcelSheet.Cells.Item[MP.x, MP.y].Font.Color := 0;
      If MarkerPosition = mpRight then
        ExcelSheet.Cells.Item[MP.x, MP.y].HorizontalAlignment := xlRight
      else
        ExcelSheet.Cells.Item[MP.x, MP.y].HorizontalAlignment := xlLeft;
      tmpRange.Locked := true;
    end;
  end;
end;

procedure TSheetMaper.MarkImportArea;
var
  StartPoint, EndPoint: TPoint;
  Range: ExcelRange;
begin
  if not((Sizer.RowMPropsCount > 0) or (Totals.Count > 0)) then
    exit;
  if ((not Sizer.RowPlaceMPBefore) and (Sizer.RowMPropsCount > 0)) then
    StartPoint := Sizer.StartRowMProps
  else
    if (Totals.Count > 0) then
      StartPoint := Sizer.StartTotals
    else
      exit;

  if (Totals.Count > 0) then
    EndPoint := Sizer.EndTotals
  else
    if ((not Sizer.RowPlaceMPBefore) and (Sizer.RowMPropsCount > 0)) then
      EndPoint := Sizer.EndRowMProps
    else
      exit;

  if (Columns.GrandSummaryOptions.Enabled and (Columns.Count > 0)) then
    dec(EndPoint.y);

  if (Rows.GrandSummaryOptions.Enabled and (Rows.Count > 0)) then
    dec(EndPoint.x);
  Range := GetRange(ExcelSheet, StartPoint, EndPoint);
  MarkUserObject(ExcelSheet, Range, sntImpotrArea, false);
end;

procedure TSheetMaper.MarkTableHeading;
var
  Range: ExcelRange;
begin
  case SheetHeading.Type_ of
    htNoDefine: exit;
    htTableArea: Range := GetHeadingRange(Sizer, Self);
    htCustomArea: Range := GetCustomHeadingRange(ExcelSheet, SheetHeading.Address);
  end;
  MarkUserObject(ExcelSheet, Range, sntTableHeading, true);
end;

procedure TSheetMaper.MarkAreas;
var
  StartRInd, EndRInd: integer;
  StartPoint, EndPoint: TPoint;
  Range: ExcelRange;
begin
  {пометим ВСЮ область, относящуюся к столбцам}
  StartRInd := Sizer.ColumnsAreaStartRow;
  EndRInd := Sizer.ColumnsAreaEndRow;
  if (StartRInd <> 0) and (EndRInd <> 0) then
  begin
    Range := GetRange(ExcelSheet, StartRInd, 1, EndRInd, ExcelSheet.Columns.Count);
    MarkObject(ExcelSheet, Range, BuildExcelName(sntColumnsArea), false);
    try
      Range.Rows.AutoFit;
    except
    end;
    if not IsDisplayColumns then
      Range.Rows.Hidden := true;
  end;

  {пометим область столбцов с MProp}
  StartPoint := Sizer.StartColumnsAndMProps;
  EndPoint := Sizer.EndColumnsAndMProps;
  if (StartPoint.x <> 0) and (StartPoint.y <> 0) and (EndPoint.x <> 0)
    and (EndPoint.y <> 0)then
  begin
    Range := GetRange(ExcelSheet, StartPoint, EndPoint);
    MarkObject(ExcelSheet, Range, sntColumnsAndMPropsArea, false);
  end;

  {пометим ВСЮ область, относящуюся к строкам/показателям}
  StartRInd := Sizer.RowsTotalsAreaStartRow;
  EndRInd := Sizer.RowsTotalsAreaEndRow;
  if (StartRInd <> 0) and (EndRInd <> 0) then
  begin
    Range := GetRange(ExcelSheet, StartRInd, 1, EndRInd, ExcelSheet.Columns.Count);
    MarkObject(ExcelSheet, Range, BuildExcelName(sntRowsTotalsArea), false);
    try
      Range.Rows.AutoFit;
    except
    end;
  end;
end;

procedure TSheetMaper.MarkBreaks;
var
  Range: ExcelRange;
  Place: TBreakPlacement;
begin
  if Breaks[bpFilters].Height > 0 then
    Breaks[bpFilters].StartRow := Sizer.FiltersSplitStartRow;

  if Breaks[bpUnitMarker].Height > 0 then
    Breaks[bpUnitMarker].StartRow := Sizer.UnitMarkerSplitStartRow;

  if Breaks[bpColumnTitles].Height > 0 then
    Breaks[bpColumnTitles].StartRow := Sizer.ColumnTitlesSplitStartRow;

  if Breaks[bpColumns].Height > 0 then
    Breaks[bpColumns].StartRow := Sizer.ColumnsSplitStartRow;

  if Breaks[bpRowTitles].Height > 0 then
    Breaks[bpRowTitles].StartRow := Sizer.RowTitlesSplitStartRow;

  if Breaks[bpRows].Height > 0 then
    Breaks[bpRows].StartRow := Sizer.RowsSplitStartRow;

  for Place := Low(TBreakPlacement) to High(TBreakPlacement) do
    if Breaks[Place].Height > 0 then
    begin
      Range := GetRange(ExcelSheet, Breaks[Place].StartRow, 1,
        Breaks[Place].StartRow + Breaks[Place].Height - 1, ExcelSheet.Columns.Count);
      MarkObject(ExcelSheet, Range, Breaks[Place].Name, false);
    end;
end;

procedure TSheetMaper.MapMemberProperties(Axis: TAxisType; AxisDOM: IXMLDOMDocument2);

  {!!! кандидат на общие утилиты }
  function GetAncestorNode(Node: IXMLDOMNode; Depth: integer): IXMLDOMNode;
  var i: integer;
  begin
    if Assigned(Node) then
    begin
      result := Node;
      for i := 1 to Depth do
        if Assigned(result) then
          result := result.parentNode;
    end;
  end;

{!!! разновидностей подобных процедур уже великое множество.
  кода имеем некий узел-мембер (чаще всего пустышку) и от него к корню
  ищем реальный мембер. Давно пора обобщить и вынести в утилз.
  Здесь ищется первый предок с именем Member и аттрибутом name,
  При этом не вылезая за границы элемента оси (AxisUiqueID)}
  function GetRealParentMember(Node: IXMLDOMNode; AxisIndex: integer): IXMLDOMNode;
  var
    CurAxIndex: integer;
  begin
    result := Node;
    if Assigned(result) then
      repeat
        CurAxIndex := GetIntAttr(result, attrAxisIndex, -1);
        if ((CurAxIndex <> AxisIndex)) or (result.nodeName = 'Members') then
        begin
          result := nil;
          break;
        end;

        if (GetNodeType(result) = ntMember) then
          break
        else
          result := result.parentNode;
      until not Assigned(result);
  end;

  procedure InitIndexes(Node: IXMLDOMNode; var RInd, CInd: integer; StartMP: TPoint);
  begin
    RInd := 0;
    CInd := 0;
    case Axis of
      axRow:
        begin
          RInd := GetIntAttr(Node, 'rind', 0);
          CInd := StartMP.y - 1;
        end;
      axColumn:
        begin
          RInd := StartMP.x - 1;
          CInd := GetIntAttr(Node, 'cind', 0);
        end
    end;
  end;

  procedure MapNodeMP(MPNode: IXMLDOMNode; AxisItem: TSheetAxisElementInterface;
    var RInd, CInd: integer; TotalsInColumns: integer);
  var
    MPCounter, ColumnCounter: integer;
    sName, sValue, sMask: string;
  begin
    for MPCounter := 0 to AxisItem.MemberProperties.Count - 1 do
    begin
      if not AxisItem.MemberProperties[MPCounter].Checked then
        continue;
      case Axis of
        axRow: inc(CInd);
        axColumn: inc(RInd);
      end;
      sName := AxisItem.MemberProperties[MPCounter].Name;
      EncodeMemberPropertyName(sName);
      sValue := GetStrAttr(MPNode, sName, fmEmptyCell);
      sMask := AxisItem.MemberProperties[MPCounter].Mask;
      if (sValue = 'null') and (sMask = '') then
        continue;
      if ((sValue = 'null') or (sValue = '')) and (sMask <> '') then
        sValue := '0';
      for ColumnCounter := 0 to TotalsInColumns - 1 do
        WriteMaskedValue(RInd, CInd + ColumnCounter, sValue, sMask);
    end;
  end;

  procedure MapGSMP(AxisItem: TSheetAxisElementInterface; var RInd, CInd: integer;
     TotalsInColumns: integer);
  var
    MPCounter, ColumnCounter: integer;
    sMask: string;
  begin
    for MPCounter := 0 to AxisItem.MemberProperties.Count - 1 do
    begin
      if not AxisItem.MemberProperties[MPCounter].Checked then
        continue;
      case Axis of
        axRow: inc(CInd);
        axColumn: inc(RInd);
      end;
      sMask := AxisItem.MemberProperties[MPCounter].Mask;
      if sMask = '' then
        continue;
      for ColumnCounter := 0 to TotalsInColumns - 1 do
        WriteMaskedValue(RInd, CInd + ColumnCounter, '0', sMask);
    end;
  end;

var
  AxCollection: TSheetAxisCollectionInterface;
  StartMP: TPoint;
  i, RInd, CInd, FieldCount: integer;
  LeavesNL: IXMLDOMNodeList;
  Node: IXMLDOMNode;
  TotalsInColumns: integer;
  AxisIndex, AxisCounter: integer;
  MPNode, InfoNode: IXMLDOMNode;
  UName, XPath: string;
begin
  if not Assigned(AxisDOM) then
    exit;
  if (Axis = axColumn) and (not IsDisplayColumns) then
    exit;

  AxCollection := GetAxis(Axis);
  if not Assigned(AxCollection) then
    exit;
  if AxCollection.MPCheckedCount = 0 then
    exit;

  {Настраиваем переменные в зависимости от оси}
  if Axis = axRow then
  begin
    StartMP := Sizer.StartRowMProps;
    TotalsInColumns := 1;
  end
  else //это колонки
  begin
    StartMP := Sizer.StartColumnMProps;
    TotalsInColumns := Totals.CountWithPlacement(false);
    if TotalsInColumns = 0 then
      TotalsInColumns := 1;
  end;

  MarkMPArea(Axis);
    
  if AxCollection.Broken then
  begin
    LeavesNL := AxisDOM.selectNodes('function_result/Members/Member');
    for i := 0 to LeavesNL.length - 1 do
    begin
      Node := LeavesNL[i];
      InitIndexes(Node, RInd, CInd, StartMP);

      AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
      if AxisIndex = -1 then
        continue;
      for AxisCounter := 0 to AxisIndex do
      begin
        MPNode := nil;
        {найдем узел, из которого будем вытаскивать МР}
        if AxisCounter = AxisIndex then
          MPNode := Node
        else
        begin
          InfoNode := Node.selectSingleNode(Format('./AliasInfo[@%s]',
            [AxCollection[AxisCounter].Alias]));
          if not Assigned(InfoNode) then
            continue;
          UName := GetStrAttr(InfoNode, AxCollection[AxisCounter].Alias, '');
          if UName = '' then
            continue;
          EncodeXPathString(UName);
          XPath := Format('function_result/Members/Member[@%s="%s"]',
            [attrUniqueName, UName]);
          MPNode := AxisDom.selectSingleNode(XPath);
        end;

        MapNodeMP(MPNode, AxCollection[AxisCounter], RInd, CInd, TotalsInColumns);
      end;
    end;
    {Для общего итога особая обработка}
    Node := AxisDom.selectSingleNode('function_result/Members/Summary');
    if Assigned(Node) then
    begin
      InitIndexes(Node, RInd, CInd, StartMP);
      for AxisCounter := 0 to AxCollection.Count - 1 do
        MapGSMP(AxCollection[AxisCounter], RInd, CInd, TotalsInColumns);
    end;
  end
  else
  begin
    {Пробегаемся по всем листьям оси}
    LeavesNL := AxisDOM.selectNodes('function_result/Members//Member[(not (Member)) and (not (Summary))]' +
      ' | function_result/Members//Summary[(not (Member)) and (not (Summary))]');
    for i := 0 to LeavesNL.length - 1 do
    begin
      Node := LeavesNL[i];
      {Для начала выясним координаты для свойств}
      InitIndexes(Node, RInd, CInd, StartMP);

      FieldCount := AxCollection.FieldCount;
      for AxisCounter := 0 to AxCollection.Count - 1 do
      begin
        FieldCount := FieldCount -
          IIF(AxCollection[AxisCounter].IgnoreHierarchy, 1, AxCollection[AxisCounter].Levels.Count);
        MPNode := GetRealParentMember(GetAncestorNode(Node, FieldCount), AxisCounter);

        MapNodeMP(MPNode, AxCollection[AxisCounter], RInd, CInd, TotalsInColumns);
      end;
    end;
    {Для общего итога особая обработка}
    Node := AxisDom.selectSingleNode(
      'function_result/Members/Summary | function_result/Members/Summary//Member[(not (Member)) and (not (Summary))]');
    if Assigned(Node) then
    begin
      InitIndexes(Node, RInd, CInd, StartMP);
      for AxisCounter := 0 to AxCollection.Count - 1 do
        MapGSMP(AxCollection[AxisCounter], RInd, CInd, TotalsInColumns);
    end;
  end;
end;

function TSheetMaper.MapFullAxis(AxisType: TAxisType; AxisDom: IXMLDOMDocument2): boolean;
begin
  result := true;
  try
    MapAxis(AxisDOM, AxisType);
    MapMemberProperties(AxisType, AxisDOM);
    if GetAxis(AxisType).LevelsFormatting then
      StyleAreaByLevels(AxisType, AxisDom, IIF(AxisType = axRow, 1, 2));
  except
    PostMessage(IIF(AxisType = axRow, 'Ошибка при размещении оси строк',
      'Ошибка при размещении оси столбцов'), msgError);
    result := false;
  end;
end;

function TSheetMaper.MapFullTotals(RowsDom, ColumnsDom: IXMLDOMDocument2;
  BeyondColumns: boolean): boolean;
var
  i: integer;
  tmpStr:string;
  tmpStringList: TStringList;
begin
  try
    MapTotalTitlesEx(BeyondColumns);
    MapTotalsData(RowsDOM, ColumnsDOM);
    if Totals.StyleByLevels then
    begin
      OpenOperation('Форматирование области показателей', not CriticalNode, NoteTime, otMap);
      if Totals.FormatByRows then
        StyleAreaByLevels(axRow, RowsDom, 0)
      else
        StyleAreaByLevels(axColumn, ColumnsDom, 0);
      CloseOperation;
    end;

    { Соберем сведения о доступных для записи областях. К этому моменту
      показатели уже размещены, а диапазоны отдельных - размечены, хотя
      само размещение отдельных еще впереди.}
    GetWritableTotalsInfo;

    if not IsSequelSheet(ExcelSheet) then
      if not SingleCells.Empty then
      begin
        tmpStringList := TStringList.Create;
        for i := 0 to SingleCells.Count - 1 do
          if NeedMapSingleCells or SingleCells[i].PlacedInTotals then
            tmpStringList.Add(IntToStr(i));
        if tmpStringList.Count > 0 then
          MapSingleCells(tmpStringList, tmpStr, false, RowsDOM, ColumnsDOM);
      end;

    OpenOperation('Подсчет итогов', not CriticalNode, NoteTime, otMap);
    MapSummaries(RowsDOM, ColumnsDOM, nil);
    MarkResultsGrey(RowsDom, ColumnsDom);
    CloseOperation;
    result := true
  except
    PostMessage('Ошибка при размещении показателей', msgError);
    result := false;
  end;
end;

function TSheetMaper.DoFinalMapping: boolean;
begin
  result := false;
  try
    MarkAreas;
    MarkAllTable; //помечаем всю таблицу
    WriteUnitMarker; //выводим единицу измерения
    MarkImportArea;
    MarkTableHeading;
    MarkBreaks;
    MarkSheetId;
    ShowTestVersionWarning;
  except
    PostMessage('Ошибка при завершении операции размещения', msgError);
    exit;
  end;
  result := true;
end;

function TSheetMaper.GetTableData: boolean;
begin
  result := false;
  try
    QueryTotalsData;
  except
    PostMessage('Ошибка при получении данных', msgError);
    exit;
  end;
  if FProcess.LastError <> '' then
    exit;
  result := true;
end;

function TSheetMaper.BuildAxes: boolean;
begin
  result := false;
  try
    RowsDOM := Rows.GetFullAxisDOM; //ось строк
  except
    PostMessage('Ошибка при сборке оси строк', msgError);
    exit;
  end;
  try
    ColumnsDOM := Columns.GetFullAxisDOM; //ось столбцов
  except
    PostMessage('Ошибка при сборке оси столбцов', msgError);
    exit;
  end;

  {Удаляем пустые строки и столбцы}
  {Дополнительный режим (tpmLarge) продемонстрировал свою несовместимость
    с новым, "правильным" подсчетом итогов в сложной оси. В связи с этим
    все равно приходится вызывать процедуру скрытия пустых -
    теряется главная изюмина дополнительного режима - само построение оси
    гарантирует отсутствие в ней пустых строк.}
  if Rows.HideEmpty then
  try
    OpenOperation('Удаление пустых строк', not CriticalNode, not NoteTime, otProcess);
    if Rows.Broken then
      RemoveEmptyForBrokenAxis(RowsDOM, axRow)
    else
      RemoveEmpty(RowsDOM, axRow);
    CloseOperation;
  except
    PostMessage('Ошибка при удалении пустых строк', msgError);
    exit;
  end;
  if Columns.HideEmpty then
  try
    OpenOperation('Удаление пустых столбцов', not CriticalNode, not NoteTime, otProcess);
    if Columns.Broken then
      RemoveEmptyForBrokenAxis(ColumnsDOM, axColumn)
    else
      RemoveEmpty(ColumnsDOM, axColumn);
    CloseOperation;
  except
    PostMessage('Ошибка при удалении пустых столбцов', msgError);
    exit;
  end;
  result := true;
end;

function TSheetMaper.InitSizer(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
begin
  result := false;
  Sizer := TSheetSizer.Create;
  try
    Sizer.Init(ExcelSheet, FLCID, self, RowsDOM, ColumnsDOM);
    {А вот здесь имеет смысл скорректировать величины, управляющие работой
      коллектора - все эти FFirstColumn - чтобы они отражали реальную геометрию
      вновь размещаемой таблицы. Это позволит устранить неверное определение
      исключений из типовых формул.}
    if Totals.Count > 0 then
    begin
      with Sizer.StartTotals do
      begin
        FFirstRow := x;
        FFirstColumn := y;
      end;
      with Sizer.EndTotals do
      begin
        FLastRow := x;
        FLastColumn := y;
      end;
    end;
    if Rows.Count > 0 then
      FRowsLeaf := Sizer.EndRows.y;
    if Columns.Count > 0 then
      FColumnsLeaf := Sizer.EndColumns.x;
  except
    PostMessage('Ошибка при расчете размеров таблицы', msgError);
    exit;
  end;
  result := true;
end;

function TSheetMaper.DoMapTable(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;
begin
  result := false;
  {Размещаем фильтры}
   if not MapFilters then
     exit;
  {Размещаем строки}
  if not MapFullAxis(axRow, RowsDom) then
    exit;
  {Размещаем столбцы}
  if not MapFullAxis(axColumn, ColumnsDom) then
    exit;
  {После окончания размещения осей подготовим разметочную информацию для сбора.
    Это надо сделать до размещения отдельных показателей!}
  PrepareMarkupDocuments(RowsDom, ColumnsDom);
  {Размещаем показатели}
  if not MapFullTotals(RowsDom, ColumnsDom, true) then
    exit;
  {Размещение и маркировка вспомогательных областей}
  if not DoFinalMapping then
    exit;
  result := true;
end;

procedure TSheetMaper.ClearTestVersionWarning;
var
  Range: ExcelRange;
begin
  {$WARNINGS OFF}
  Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTestMark));
  if Assigned(Range) then
    Range.EntireRow.Delete(xlShiftUp);
  {$WARNINGS ON}
end;

function TSheetMaper.MapWideTable(RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;

  function GetSheet(Book: ExcelWorkbook; Index: integer): ExcelWorkSheet;
  begin
    try
      result := ExcelWorkSheet(Book.Sheets[Index]);
    except
      result := nil;
    end;
  end;

  procedure ClearColumnsAndTotals(Sheet: ExcelWorkSheet);
  var
    Range: ExcelRange;
  begin
    Range := GetRangeByName(ExcelSheet, BuildExcelName(sntColumnTitles));
    if Assigned(Range) then
      Range.Clear;
    Range := GetRangeByName(ExcelSheet, BuildExcelName(sntColumnsAndMPropsArea));
    if Assigned(Range) then
      Range.Clear;
    Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTotalTitles));
    if Assigned(Range) then
      Range.Clear;
    Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTotals));
    if Assigned(Range) then
      Range.Clear;
  end;

  {При мапинге широкой таблицы от основного ДОМа оси отрезается кусок, который
    и размещается на очередном листе. И именно в нем прописываются координаты
    размещения узлов, которые нужны для разметки. Поэтому надо их переписать
    в основной ДОМ.}
  procedure CopyCoords(SourceDom, PartDom: IXMLDOMDocument2);
  var
    i: integer;
    NL: IXMLDOMNodeList;
    Node: IXMLDOMNode;
    NodeId, Cind: integer;
  begin
    NL := PartDom.selectNodes(
      Format('function_result/Members//*[@%s]', [attrNodeId]));
    for i := 0 to NL.length - 1 do
    begin
      NodeId := GetIntAttr(NL[i], attrNodeId, -BeastNumber);
      Node := SourceDom.selectSingleNode(
        Format('function_result/Members//*[@%s="%d"]', [attrNodeId, NodeId]));
      if not Assigned(Node) then
        continue;
      CInd := GetIntAttr(NL[i], 'cind', -BeastNumber);
      SetAttr(Node, 'cind', CInd);
    end;
  end;

var
  SourceDom, PartDom, DomToUse: IXMLDOMDocument2;
  LeafLimit, SheetCounter, StartSheetIndex, Divider, SheetLastColumn: integer;
  Book: ExcelWorkBook;
  OriginalName: string;
  NothingToMap: boolean;
  TmpSheet: ExcelWorkSheet;
  MDCopy: IXMLDOMDocument2;
begin
  result := false;
  SheetLastColumn := SheetColumnsLimitation;
  LeafLimit := (SheetLastColumn - Sizer.StartColumns.y - Totals.CountWithPlacement(true));
  Divider := Totals.CountWithPlacement(false);
  if Divider > 0 then
    LeafLimit := LeafLimit div Divider;

  GetDomDocument(SourceDom);
  SourceDom.load(ColumnsDom);
  if not Assigned(FFullColumnAxis) then
    GetDomDocument(FFullColumnAxis);
  FFullColumnAxis.load(ColumnsDom);

  DivideMembersDom(SourceDom, PartDom, LeafLimit, Columns.Broken);
  FNeedHostEventsDisabled := true;
  OriginalName := Copy(ExcelSheet.Name, 1, 27);
  StartSheetIndex := ExcelSheet.Index[FLCID];
  Book := ExcelWorkBook(ExcelSheet.Parent);
  FWideTableSummaryMode := true;
  try
    ClearTestVersionWarning;
    InitSizer(RowsDom, PartDom);
    ClearTableEx;

    {Размещаем фильтры и строки - общие элементы для всех листов широкой таблицы}
    if not MapFilters then
       exit;
    if not MapFullAxis(axRow, RowsDom) then
      exit;
    MarkAllTable;

    {После размещения фильтров и строк сделаем копию
      листа для использования в качестве "основы" для листов-продолжений}
    ExcelSheet.Copy(Book.Sheets[1], EmptyParam, FLCID);
    tmpSheet := ExcelWorkSheet(Book.Sheets[1]);

    {Теперь можно закончить построение "главного" листа.}
    if not MapFullAxis(axColumn, PartDom) then
      exit;

    CopyCoords(ColumnsDom, PartDom);
    PrepareMarkupDocuments(RowsDom, ColumnsDom);

    if not MapFullTotals(RowsDom, PartDom, false) then
      exit;
    if not DoFinalMapping then
      exit;
    ExportXml(MDCopy);


    SheetCounter := 0;
    NothingToMap := false;
    repeat
      tmpSheet.Copy(EmptyParam, ExcelSheet, FLCID);
      inc(SheetCounter);
      ExcelSheet := GetSheet(Book, StartSheetIndex + 1 + SheetCounter);
      ExcelSheet.Name := OriginalName + Addend + IntToStr(SheetCounter);
      GetRange(ExcelSheet, 1, 1).Activate;
      DivideMembersDom(SourceDom, PartDom, LeafLimit, Columns.Broken);
      if Assigned(PartDom) then
        DomToUse := PartDom
      else
      begin
        DomToUse := SourceDom;
        NothingToMap := true;
      end;
      InitSizer(RowsDom, DomToUse);
      ClearColumnsAndTotals(ExcelSheet);
      GetCPByName(ExcelSheet, cpSequelName, true).Value := 'true';

      if not MapFullAxis(axColumn, DomToUse) then
        exit;
      if not MapFullTotals(RowsDom, DomToUse, NothingToMap) then
        exit;
      if not DoFinalMapping then
        exit;
    until NothingToMap;
    FWideTableSummaryMode := false;
    MapSummaries(RowsDom, FFullColumnAxis, nil);

    repeat
      DeleteExcelNames(ExcelWorkSheet(Book.Sheets[StartSheetIndex + 1 + SheetCounter]));
      DeleteAllowEditRanges(ExcelSheet);
      SetSheetProtection(ExcelWorkSheet(Book.Sheets[StartSheetIndex + 1 + SheetCounter]), true);
      dec(SheetCounter);
    until SheetCounter = 0;
  finally
    KillDomDocument(SourceDom);
    KillDomDocument(PartDom);
    KillDomDocument(FFullColumnAxis);
    FNeedHostEventsDisabled := false;
    tmpSheet.Delete(FLCID);
    ExcelSheet := GetSheet(Book, StartSheetIndex);
    LoadFromXml(MDCopy);
  end;
  result := true;
end;

procedure TSheetMaper.DeleteAddendSheets(Sheet: ExcelWorkSheet);
var
  i, j: integer;
  SheetName: string;
  AddendSheet: ExcelWorkSheet;
  Book: ExcelWorkbook;
begin
  Book := ExcelWorkBook(ExcelSheet.Parent);
  for i := 1 to 99 do //ну вряд ли кто сделает лист с сотней продолжений...
  begin
    SheetName := Copy(Sheet.Name, 1, 27) + Addend + IntToStr(i);
    AddendSheet := nil;
    for j := 1 to Book.Sheets.Count do
      if ExcelWorkSheet(Book.Sheets[j]).Name = SheetName then
      begin
        AddendSheet := ExcelWorkSheet(Book.Sheets[j]);
        break;
      end;
    if Assigned(AddendSheet) then
      AddendSheet.Delete(FLCID)
  end;
end;

procedure TSheetMaper.PrepareMarkupDocuments(RowsDom, ColumnsDom: IXMLDOMDocument2);
begin
  FRowsMarkup := InitXmlDocument;
  FColumnsMarkup := InitXmlDocument;
  FRowsMarkup.documentElement := FRowsMarkup.createElement('markup');
  FColumnsMarkup.documentElement := FColumnsMarkup.createElement('markup');
  MakeMarkupDocument(axRow, RowsDom, Sizer.StartRows.x);
  MakeMarkupDocument(axColumn, ColumnsDom, Sizer.StartColumns.y);

  //определяются при инициализации сайзера мапером
  //FFirstRow := Sizer.StartRows.x;
  //FFirstColumn := Sizer.StartColumns.y;

  if AddinLogEnable then
  begin
    WriteDocumentLog(FRowsMarkup, 'rows_markup.xml');
    WriteDocumentLog(FColumnsMarkup, 'columns_markup.xml');
  end;
end;

function TSheetMaper.MapTable: boolean;
begin
  result := false;
  OpenOperation(pfoMapTable, CriticalNode, NoteTime, otMap);
  try
    {Получаем данные показателей из базы. При неудаче дальнейшая работа бессмысленна.}
    if not GetTableData then
      exit;
    {Собираем оси; ошибки - фатальны.}
    if not BuildAxes then
      exit;
    {Просчитываем размеры таблицы}
    OpenOperation(pfoCalcSheetSize, CriticalNode, not NoteTime, otProcess);
    ClearTestVersionWarning;
    if not InitSizer(RowsDom, ColumnsDom) then
      exit;

    DeleteAddendSheets(ExcelSheet);
    {Если таблица полностью влезает в лист - размещаем обычным порядком, если
      выпирает по ширине - извращаемся и раскидываем ее на несколько листов
      (задача 6742)}
    if CheckSheetSize(FWideTableMode) then
    begin
      CloseOperation; //  pfoCalcSheetSize
      {очищаем существующую таблицу (инициализировать надо до очистки!)}
      ClearTableEx;

      if not DoMapTable(RowsDom, ColumnsDom) then
        exit;

      CloseOperation;//MapTable
      result := true;
    end
    else
      if FWideTableMode and not Columns.Empty then
      begin
        CloseOperation; //  pfoCalcSheetSize
        {очищаем существующую таблицу (инициализировать надо до очистки!)}
        //ClearTableEx(Sizer);

        result := MapWideTable(RowsDom, ColumnsDom);
        CloseOperation;//MapTable
      end
      else
        PostMessage(Format(ermSheetOverSize, [SheetColumnsLimitation,
          ExcelSheet.Rows.Count]), msgError);//pfoCalcSheetSize
  finally
  end;
end;

function TSheetMaper.MapFilters: boolean;
var
  i, j, k, FilterCount: integer;
  CurrentFilter: TSheetFilterInterface;
  RInd, CInd, ECInd, MPCount: integer;
  FilterDimension, FilterText: string;
  Range, Range1, Range2, Range3: ExcelRange;
  MPStrings: TStringList;
  NeedPrintableStyle: boolean;
begin
  result := false;
  try
    FilterCount := Filters.Count;
    {фильтров нет или ось скрыта - сразу на выход}
    if (FilterCount <= 0) or (not IsDisplayFilters) then
    begin
      result := true;
      exit;
    end;
    OpenOperation(pfoMapFilters, CriticalNode, not NoteTime, otMap);
    RInd := Sizer.StartFilters.x;
    CInd := Sizer.StartFilters.y;
    ECInd := Sizer.EndFilters.y;
    if ECind = Cind then
      inc(ECind);
    for i := 0 to FilterCount - 1 do
    begin
      CurrentFilter := Filters[i];
      if CurrentFilter.IsPartial then
        continue;
      FilterDimension := CurrentFilter.FullDimensionName2;
      FilterText := CurrentFilter.Text;

      {три диапазона, три составные части фильтра}
      Range1 := GetRange(ExcelSheet, RInd, CInd, RInd, CInd);
      Range2 := MergeCells(ExcelSheet, RInd + 1, CInd, RInd + 1, ECInd);
      MPCount := CurrentFilter.MemberProperties.CheckedCount;
      if MPCount > 0 then
      begin
        Range3 := GetRange(ExcelSheet, RInd + 1 + 1, CInd, RInd + 1 + MPCount, ECInd);
        MPStrings := CurrentFilter.GetMPStrings;
      end
      else
        Range3 := nil;

      {стили}
      NeedPrintableStyle := PrintableStyle or
        (FIsLastRefreshBeforeOffline and not CurrentFilter.MayBeEdited);
      Range1.Style := IIF(NeedPrintableStyle, CurrentFilter.Styles.Name[esTitlePrint],
        CurrentFilter.Styles.Name[esTitle]);
      Range2.Style := IIF(NeedPrintableStyle, CurrentFilter.Styles.Name[esValuePrint],
        CurrentFilter.Styles.Name[esValue]);
      if Assigned(Range3) then
        Range3.Style := IIF(NeedPrintableStyle, snMemberPropertiesPrint, snMemberProperties);

      {значения}
      Range1.Cells.Item[1, 1].Value := FilterDimension;
      Range2.Cells.Item[1, 1].Value := FilterText;
      {подбор высоты}
      AutoFitMerged(Range2);
      k := 0;
      for j := 0 to CurrentFilter.MemberProperties.Count - 1 do
        if CurrentFilter.MemberProperties[j].Checked then
        begin
          inc(k);
          Range3.Cells.Item[k, 1].Value := CurrentFilter.MemberProperties[j].Name;
          WriteMaskedValue(Range3.Cells.Item[k, 2], MPStrings[k - 1],
            CurrentFilter.MemberProperties[j].Mask);
          MergeCells(ExcelSheet, RInd + k + 1, CInd + 1, RInd + k + 1, ECInd);
        end;
      FreeAndNil(MPStrings);

      if IsDisplayCommentStructuralCell then
        CommentCell(ExcelSheet, RInd, CInd, CurrentFilter.CommentText);

      Range := GetRange(ExcelSheet, RInd, CInd, RInd + 1 + MPCount, ECInd);
      MarkObject(ExcelSheet, Range, CurrentFilter.ExcelName, false);
      RInd := RInd + 2 + MPCount;
    end;

    { пометим всю область фильтров }
    if Sizer.FiltersHeight > 0 then  //Есть общие фильтры
      Range := GetRange(ExcelSheet, Sizer.StartFilters, Sizer.EndFilters);
    MarkObject(ExcelSheet, Range, sntFilterArea, false);
    CloseOperation; // pfoMapFilters
    result := true;
  except
    on e: exception do
      PostMessage('Ошибка при размещении фильтров: ' + e.Message, msgError);
  end;
end;

procedure TSheetMaper.MarkSheetId;
var
  IdText: string;
  StartRInd, EndRInd, CInd, i: integer;
  IdRange: ExcelRange;
begin
  if not IsDisplaySheetInfo then
    exit;
  if not Assigned(ExcelSheet) or not Assigned(Sizer) then
    exit;
  if Sizer.TableIsEmpty then
    exit;

  StartRInd := Sizer.StartSheetId.x - Sizer.SheetIdSplit;
  CInd := Sizer.StartSheetId.y;
  EndRInd := Sizer.EndSheetId.x;
  if StartRInd + EndRInd + CInd < 4 then //если хотя бы одно равно 0 - выход
    exit;
  i := StartRInd + Sizer.SheetIdSplit;
  try
    if Assigned(Environment) then
    begin
      IdText := 'Наименование задачи: ' + Environment.TaskName;
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Идентификатор задачи: ' + Environment.TaskId;
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Наименование документа: ' + Environment.DocumentName;
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Идентификатор документа: ' + Environment.DocumentId;
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Исполнитель: ' + Environment.Owner;
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Дата: ' + DateToStr(Date);
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
    end
    else
    begin
      IdText := 'Наименование задачи: ';
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Идентификатор задачи: ';
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Наименование документа: ';
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Идентификатор документа: ';
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Исполнитель: ';
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
      inc(i);
      IdText := 'Дата: ' + DateToStr(Date);
      ExcelSheet.Cells.Item[i, CInd].Value := IdText;
    end;
  finally
    IdRange := GetRange(ExcelSheet, StartRInd, CInd, EndRInd, CInd);
    IdRange.Style := snSheetId;
    MarkObject(ExcelSheet, IdRange, sntSheetId, false);
  end;
end;

{крокодил, конечно, тоже еще тот...}
procedure TSheetMaper.MapSummaries(RowsDOM, ColumnsDOM: IXMLDOMDocument2;
  SingleTotal: TSheetTotalinterface);
var
  FormulaArrayColumns: TByteSet;

  function StrFunction(Str1, Str2: string; Mode: TMeasureCountMode): string;
  begin
    if not IsNumber(Str1) then
      if not IsNumber(Str2) then
        result := ''
      else
        result := Str2
    else
      if not IsNumber(Str2) then
        result := Str1
      else
        try
          case Mode of
            mcmSum, mcmAvg: result := FloatToStr(StrToFloat(Str1) + StrToFloat(Str2));
            mcmMin: result := FloatToStr(Min(StrToFloat(Str1), StrToFloat(Str2)));
            mcmMax: result := FloatToStr(Max(StrToFloat(Str1), StrToFloat(Str2)));
            else result := '';
          end;
        except
          result := '';
        end;
  end;

  // получить диапазон показателя по колонке
  function GetTotalRangeByColumn(Total: TSheetTotalInterface; Column: integer): ExcelRange;
  var
    SectionIndex: integer;
  begin
    SectionIndex := 0;
    result := Total.GetTotalRange(SectionIndex);
    while (result <> nil) do
    begin
     if (result.Column = Column) then
       exit;
     // переходим в следующую секцию показателя
     inc(SectionIndex);
     result := Total.GetTotalRangeWithoutGrandSummary(SectionIndex);
    end;
  end;

  function GetRowIndex(AxisType: TAxisType;
    Index, PairedIndex: integer): integer;
  begin
    result := IIF(AxisType = axRow, PairedIndex, Index);
  end;

  function GetColumnIndex(AxisType: TAxisType;
    Index, PairedIndex: integer): integer;
  begin
    result := IIF(AxisType = axRow, Index, PairedIndex);
  end;

  procedure MapSummaryValue(Node:IXMLDOMNode; AxisType: TAxisType; Index, PairedIndex: integer;
    SumValue: string; Total: TSheetTotalInterface);
  var
    RInd, CInd: integer;
    FormattedSumValue: extended;
    ESheet: ExcelWorkSheet;
  begin
    if FWideTableMode and (AxisType = axColumn) then
    try
      if Node.nodeName <> 'Members' then
        ESheet := GetWorkSheet(ExcelWorkBook(ExcelSheet.Parent).Sheets.Item[GetStrAttr(Node, 'sheetname', '')])
      else
        ESheet := ExcelSheet;
    except
      on E: Exception do
      begin
        ShowError('Ахтунг! Ошибка тестовой версии "широких итогов". '#13#10 +
          'Получение имени листа в MapSummaryValue. Алярмить разработчику!'#13#10 +
          E.Message);
        ESheet := ExcelSheet;
      end;
    end
    else
      ESheet := ExcelSheet;
    RInd := GetRowIndex(AxisType, Index, PairedIndex);
    CInd := GetColumnIndex(AxisType, Index, PairedIndex);
    if (RInd <= 0) or (CInd <= 0) then //перестраховка
      exit;

    if (Total.CountMode in [mcmTypeFormula]) or
      WritablesInfo.IsSingleCellSelected(ESheet, RInd, Cind) then
      exit;

    if Lo(CInd) in FormulaArrayColumns then
      exit;

    {Требование задачи 6994(Функция итогов "Не вычислять". Глюк), при режиме
    подсчета итогов "Не вычислять" значение ячеек =  EmptyValueSymbol}
    if not (Total.CountMode in [mcmNone]) and IsNumber(SumValue) then
    try
      FormattedSumValue := StrToFloat(SumValue);
      Total.Round(FormattedSumValue);
      ESheet.Cells.Item[RInd, CInd].Value := FormattedSumValue;
    except
      on E: Exception do
      begin
        PostMessage('Ошибка при размещении показателя "' + Total.GetElementCaption +
        '". ' + E.Message, msgError);
        raise;
      end;
    end
    else
      if Total.TotalType <> wtConst then  // 7816: Для константы "не вычислять" = ей самой
        try
          ESheet.Cells.Item[RInd, CInd].Value := Total.EmptyValueSymbol;
        except
          PostMessage('Ошибка при размещении показателя "' + Total.GetElementCaption +
            '". Недопустимый символ пустого значения', msgError);
          raise;
        end;
  end;

  {получает значение из ячейки}
  function GetAtomicCellValue(Node: IXMLDOMNode; Total: TSheetTotalInterface; AxisType: TAxisType;
    Index, PairedIndex: integer; var ElementCount: integer; out NaN: boolean): string;
  var
    Cell: OleVariant;
    CellValue, CellFormula, CellStyle: string;
    RowIndex, ColumnIndex: integer;
    ESheet: ExcelWorkSheet;
    SheetName: string;
  begin
    result := Total.EmptyValueSymbol;
    NaN := false;
    try
      inc(ElementCount);
      RowIndex := GetRowIndex(AxisType, Index, PairedIndex);
      ColumnIndex := GetColumnIndex(AxisType, Index, PairedIndex);

      if FWideTableMode and (AxisType = axColumn) then
      begin
        SheetName := GetStrAttr(Node, 'sheetname', '');
        try
          ESheet := GetWorkSheet(ExcelWorkBook(ExcelSheet.Parent).Sheets.Item[SheetName]);
        except
          on E: Exception do
          begin
            ShowError('Ахтунг! Ошибка тестовой версии "широких итогов". '#13#10 +
              'Получение имени листа в GetAtomicCellValue. Алярмить разработчику!'#13#10 +
              E.Message);
            ESheet := ExcelSheet;
          end;
        end;
      end
      else
        ESheet := ExcelSheet;

      Cell := ESheet.Cells.Item[RowIndex, ColumnIndex];
      if GetCellValue(ESheet, RowIndex, ColumnIndex,
        Total.EmptyValueSymbol, CellValue, CellFormula, CellStyle, false) then
        result := CellValue
      else
        exit;
      if not IsNumber(result) then
      begin
        NaN := result <> Total.EmptyValueSymbol;
        if NaN then
          result := Total.EmptyValueSymbol;
      end;
    except
      result := Total.EmptyValueSymbol;
    end;
  end;

  function IsGrandSummaryPresents(Dom: IXMLDOMDocument2): boolean;
  begin
    result := false;
    if not Assigned(Dom) then
      exit;
    result := Assigned(Dom.selectSingleNode('function_result/Members/Summary'));
  end;

  {Общий ход мыслей.
    Главная сложность в том, что если где-то выше текущего узла была
    разрушена иерархия измерения, то реальные данные, детализирующие итог по
    этому узлу НЕ находятся в его потомках. Они расположены в тех "сестрах",
    которые детализируют узел в вышестоящем измерении, у которого сломана иерархия.

    Выделим конкретные случаи различного подсчета итога по узлу.

    1). Самый простой - иерархия у измерения цела, а выше тоже все было в порядке.
      подсчет стандартный - по детям. Давным давно реализовано.

    2). Иерархия сломана, но это впервые, т.е. выше сломанных еще не было.
      Для нелистового узла идем по сестрам до тех пор, пока не встретим сестру
      равного или даже меньшего уровня, чем у текущего узла. Итог детализируется
      сестрами следующего (+1) уровня по отношению к текущему.
      После этого включаем модификатор режима "сверху была сломанная иерархия" и
      идем вглубь оси - по детям текущего узла.

      Если узел был листовым в измерении (признак MemberLeaf), то применяем
      стандартный подсчет по детям, независимо от того, какое там ниже
      измерение - с иерархией или без. При этом модификатор взводить НЕ НУЖНО.

    3). Модификатор режима "сверху была сломанная иерархия" включен.
      Идем по детализирующим итог узлам обычным порядком: есть иерархия - по детям,
      нет - по сестрам, НО! Когда доходим до листьев оси, значения из
      соответствующих ячеек сетки не берем, потому что это "ненастоящие" листья.
      Это тоже итоги. Чтобы найти соответствующие "настоящие" листья, надо
      вернуться к тому элементу, который выставил модификатор и от его
      детализирующих узлов пройти по тем же путям как шли до "ненастоящих листьев".

      На словах немного сумбурно, но на визуальном примере с включенной
      "подсветкой серым" все очень наглядно. 
    }

  function NewMapSummary(Node: IXMLDOMNode;
    Total: TSheetTotalInterface; // показатель, для которого считаем
    AxisType: TAxisType; // тип оси - строки или столбцы
    Index: integer; // координата по противоположной оси
    Ordinal: integer; // смещение по столбцам
    const LastAxisIndex: integer; // индекс последнего измерения оси
    var ElementCount: integer;
    var NaN: boolean;
    ParentLeaf: boolean;
    HierarchyBreaker: IXMLDOMNode;
    TotalFactorization: array of boolean
    ): string;


    function GetComplexCellValue: string;
    var
      XPath, Value, UName, ParentUName, BreakerUName: string;
      tmpNode, ParentNode: IXMLDOMNode;
      i, CurNodeAxisIndex, ParentNodeAxisIndex: integer;
      NL: IXMLDOMNodeList;
    begin
      result := Total.EmptyValueSymbol;
      if Total.CountMode = mcmNone then
        exit;
      {Соберем XPath для адресации к текущему листовому элементу
        относительно узла, сломавшего иерархию. Применение данного XPath
        к детализирующим узлам брейкера даст искомые "реальные" листовые
        элементы.}
      UName := GetStrAttr(Node, attrUniqueName, '');
      EncodeXPathString(UName);
      XPath := Format('Member[@%s="%s"]', [attrUniqueName, UName]);
      CurNodeAxisIndex := GetIntAttr(Node, attrAxisIndex, -1);

      ParentNode := GetParentNode(Node);
      ParentUName := GetStrAttr(ParentNode, attrUniqueName, '');
      ParentNodeAxisIndex := GetIntAttr(ParentNode, attrAxisIndex, -1);
      BreakerUName := GetStrAttr(HierarchyBreaker, attrUniqueName, '');
      while ParentUName <> BreakerUName do
      begin

        if not ((CurNodeAxisIndex = ParentNodeAxisIndex) and
          GetAxis(AxisType)[CurNodeAxisIndex].IgnoreHierarchy) then
        begin
          EncodeXPathString(ParentUName);
          XPath := Format('Member[@%s="%s"]/', [attrUniqueName, ParentUName]) + XPath;
        end;

        CurNodeAxisIndex := ParentNodeAxisIndex;
        ParentNode := GetParentNode(ParentNode);
        ParentUName := GetStrAttr(ParentNode, attrUniqueName, '');
      end;
      XPath := './' + XPath;

      {Теперь идем по детализирующим узлам брейкера, получаем листовые значения
        и суммируем их}
      EncodeXPathString(BreakerUName);
      NL := HierarchyBreaker.parentNode.selectNodes(
        Format('./Member[@%s="%s"]', [attrParentUN, BreakerUName]));
      if NL.length > 0 then
      begin
        for i := 0 to NL.length - 1 do
        begin
          tmpNode := NL[i].selectSingleNode(XPath);
          Value := GetStrAttr(tmpNode, Total.Alias, Total.EmptyValueSymbol);
          result := StrFunction(result, Value, Total.CountMode);
        end;
        if (result <> Total.EmptyValueSymbol) and
          TotalFactorization[LastAxisIndex] and (Total.CountMode = mcmAvg) then
          result := FloatToStr(StrToFloat(result) / NL.length)
      end;
    end;

  const
    NotLeaf = -BeastNumber;
  var
    AxisIndex, LevelIndex: integer;
    PairedIndex, LeafIndex: integer;
    i, Cnt: integer;
    AxisElement: TSheetAxisElementInterface;
    IsTotalFactorizedBy: boolean;
    Value, UName: string;
    NL: IXMLDOMNodeList;
    IsNextAxis, IsNextLevel: boolean;
    SummandAxisIndex, SummandLevelIndex: integer;
  begin
    result := Total.EmptyValueSymbol;
    PairedIndex := IIF(AxisType = axRow, GetIntAttr(Node, 'rind', 0),
      GetIntAttr(Node, 'cind', -BeastNumber) + Ordinal);
    {Реальный листовой элемент должен отвечать трем условиям:
      1) принадлежать последнему измерению в оси
      2) иметь выставленный признак MemberLeaf, равный индексу измерения
      3) его предки в предыдущих измерениях тоже MemberLeaf.
      Для таких элементов мы просто берем значение из ячейки.}
    AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
    LevelIndex := GetIntAttr(Node, attrLevelIndex, -1);
    LeafIndex := GetIntAttr(Node, attrMemberLeaf, NotLeaf);
    if (AxisIndex = LastAxisIndex) and (LeafIndex = AxisIndex) and ParentLeaf then
      if not Assigned(HierarchyBreaker) then
      begin
        result := GetAtomicCellValue(Node, Total, AxisType, Index, PairedIndex, ElementCount, NaN);
        (Node as IXMLDOMElement).setAttribute(Total.Alias, result);
        exit;
      end
      else
      begin
        result := GetComplexCellValue;
        (Node as IXMLDOMElement).setAttribute(Total.Alias, result);
        MapSummaryValue(Node, AxisType, Index, PairedIndex, result, Total);
        exit;
      end;

    if Node.nodeName = 'Members' then
      AxisIndex := 0;
    AxisElement := GetAxis(AxisType)[AxisIndex];
    IsTotalFactorizedBy := TotalFactorization[AxisIndex];

    if not AxisElement.IgnoreHierarchy or
      (AxisElement.IgnoreHierarchy and ((LeafIndex <> NotLeaf))) then
    begin
      NL := Node.selectNodes('./Member');
      for i := 0 to NL.length - 1 do
      begin
        Cnt := ElementCount;
        ElementCount := 0;

        SummandAxisIndex := GetIntAttr(NL[i], attrAxisIndex, -1);
        SummandLevelIndex := GetIntAttr(NL[i], attrLevelIndex, -1);
        IsNextLevel := (SummandAxisIndex = AxisIndex) and (SummandLevelIndex - LevelIndex = 1);
        (*
          (SummandAxisIndex - AxisIndex = 1) and (SummandLevelIndex = 0)
          Такое условие дает сбой, когда во втором измерении сломана иерархия и
          на верхних уровнях нет выбранных элементов - SummandLevelIndex оказывается
          вовсе не 0, хотя элемент непосредственно подклеен к Node.
          Реальным критерием "топовости" элемента в измерении является
          отсутствие атрибута ParentUN: при сломе иерархии он проставляется только
          элементам, имеющим выделенного родителя, а при целой иерархии он
          вообще не проставляется.*)

        IsNextAxis := (SummandAxisIndex - AxisIndex = 1) and
          not Assigned(NL[i].attributes.getNamedItem(attrParentUN)) and
          (Node.nodeName <> 'Members');

        if IsNextAxis then
          ParentLeaf := GetIntAttr(Node, attrMemberLeaf, -1) > -1;

        Value := NewMapSummary(NL[i], Total, AxisType,
          Index, Ordinal, LastAxisIndex, ElementCount,
          NaN, ParentLeaf, HierarchyBreaker, TotalFactorization);
        if NaN then
          exit;
        ElementCount := ElementCount + Cnt;
        {применяем подсчет по дочерним элементам.
          Учитывать слагаемое надо в двух случаях:
          1) когда его аксисиндекс равен, а левелиндекс больше
            родительского на единицу, т.е. это следующий уровень в том же элементе оси;
          2) когда аксисиндекс больше родительского на единицу, а левелиндекс = 0,
            т.е. это узел самого первого (0) уровня в следующем элементе оси}
        if not (IsNextLevel or IsNextAxis) and (Node.nodeName <> 'Members')then
          Value := '';

        IsTotalFactorizedBy := TotalFactorization[IIF(IsNextAxis, AxisIndex + 1, AxisIndex)];

        if Total.CountMode = mcmNone then
          result := ''
        else
          if IsTotalFactorizedBy or (not IsTotalFactorizedBy and Total.TypeFormula.Enabled)then
            result := StrFunction(result, Value, Total.CountMode)
          else
            result := Value;
      end;
      if AxisType = axRow then
        PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'rind', 0)
      else
        PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'cind', -BeastNumber) + Ordinal;
    end
    else
    begin
      UName := GetStrAttr(Node, attrUNiqueName, '');
      EncodeXPathString(UName);
      if (Node.nodeName = 'Members') then
        NL := Node.selectNodes(Format('./Member[not(@%s)]', [attrParentUN]))
      else
        NL := Node.parentNode.selectNodes(Format('./Member[@%s="%s"]', [attrParentUN, UName]));
      for i := 0 to NL.length - 1 do
      begin
        Cnt := ElementCount;
        ElementCount := 0;
        Value := NewMapSummary(NL[i], Total, AxisType,
          Index, Ordinal, LastAxisIndex, ElementCount,
          NaN, ParentLeaf, HierarchyBreaker, TotalFactorization);
        if NaN then
          exit;
        ElementCount := ElementCount + Cnt;

        if Total.CountMode = mcmNone then
          result := ''
        else
          result := IIF(IsTotalFactorizedBy,
            StrFunction(result, Value, Total.CountMode), Value);
      end;

      {теперь надо идти вглубь оси, указав текущий узел в качестве
        "разрушителя иерархии"}
      if not Assigned(HierarchyBreaker) and (Node.nodeName <> 'Members')
        and (AxisIndex < LastAxisIndex) then
      begin
        NL := Node.selectNodes('./Member');
        for i := 0 to NL.length - 1 do
        begin
          Cnt := ElementCount;
          ElementCount := 0;
          NewMapSummary(NL[i], Total, AxisType,
                Index, Ordinal, LastAxisIndex, ElementCount,
                NaN, ParentLeaf, Node, TotalFactorization);
          ElementCount := Cnt;
        end;
      end;
      { На случай стыкового итога}
      if (AxisIndex < LastAxisIndex) or (Node.nodeName = 'Members') then
        if AxisType = axRow then
          PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'rind', 0)
        else
          PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'cind', -BeastNumber) + Ordinal;
    end;


    {в этом месте мы имеем значение итога для Node и можем разместить его}
    if not NaN then
      if IsTotalFactorizedBy and (Total.CountMode = mcmAvg) and
        (ElementCount > 0) and (result <> Total.EmptyValueSymbol) then
        try
          MapSummaryValue(Node, AxisType, Index, PairedIndex,
            FloatToStr(StrToFloat(result) / ElementCount), Total)
        except
          MapSummaryValue(Node, AxisType, Index, PairedIndex, Total.EmptyValueSymbol, Total);
        end
      else
        MapSummaryValue(Node, AxisType, Index, PairedIndex, result, Total);
  end;

  {процедура подсчета итогов для случая полностью разрушенной иерархии оси.
    По применению аналогична MapAxisSummary.
    При первом вызове на вход подается узел function_result/Members}
  function MapSummariesForBrokenAxis(Node: IXMLDOMNode; Total: TSheetTotalInterface;
    AxisType: TAxisType;
    Index, Ordinal: integer;
    const LastAxisIndex: integer;
    var ElementCount: integer;
    var NaN: boolean;
    NodeLevelNumber: integer;
    var Summand: IXMLDOMNode): string;
  var
    {численные метрики очередного слагаемого}
    SummandAxisIndex, SummandLevelIndex, SummandLevelNumber: integer;
    PairedIndex, Cnt: integer;
    Value: string;
    IsTotalFactorizedBy: boolean;
    tmpNode: IXMLDOMNode;
    NodeAxisIndex, NodeLevelIndex: integer;
  begin
    result := '';
    if NaN then
      exit;
    {парный индекс - это индекс по другой оси; например, когда мы считаем
      итоги по строкам, у нас есть фиксированный индекс столбца, а парным ему
      будет переменный индекс строки}
    PairedIndex := IIF(AxisType = axRow, GetIntAttr(Node, 'rind', 0),
      GetIntAttr(Node, 'cind', -BeastNumber) + Ordinal);
    {итак, если это листовой элемент (до слома иерархии),
      то просто забираем значение из ячейки}
    if (GetIntAttr(Node, attrMemberLeaf, BeastNumber) = LastAxisIndex) then
    begin
      result := GetAtomicCellValue(Node, Total, AxisType, Index, PairedIndex, ElementCount, NaN);
      exit;
    end;
    {узнаем порядковый номер уровня слагаемого}
    SummandAxisIndex := GetIntAttr(Summand, attrAxisIndex, -1);
    SummandLevelIndex := GetIntAttr(Summand, attrLevelIndex, -1);
    SummandLevelNumber := GetAxis(AxisType).GetLevelNumber(SummandAxisIndex, SummandLevelIndex);
    {и разкладываемость показателя по измерению обсчитываемого элемента}
    NodeAxisIndex := GetIntAttr(Node, attrAxisIndex, 0);
    NodeLevelIndex := GetIntAttr(Node, attrLevelIndex, 0);

    repeat
      IsTotalFactorizedBy := Total.FactorizedBy(GetAxis(AxisType)[SummandAxisIndex]);
      {нам нужны элементы следующего уровня - каким бы он ни был}
      if SummandLevelNumber > NodeLevelNumber then
      begin
        if not Assigned(Summand) then
          break;
        tmpNode := Summand;
        Summand := Summand.nextSibling;
        Cnt := ElementCount;
        ElementCount := 0;

        Value := MapSummariesForBrokenAxis(tmpNode, Total, AxisType, Index, Ordinal,
          LastAxisIndex, ElementCount, NaN, SummandLevelNumber, Summand);

        {дальше считать нет смысла}
        if NaN then
          break;

        ElementCount := ElementCount + Cnt;

        if Total.CountMode = mcmNone then
          result := ''
        else
          result := IIF(IsTotalFactorizedBy,
            StrFunction(result, Value, Total.CountMode), Value);
      end;

      {проверка на то, что достигли последнего элемента оси}
      if not Assigned(Summand) then
        break;
      if Summand.nodeName = ntSummary then
        break;

      SummandAxisIndex := GetIntAttr(Summand, attrAxisIndex, -1);
      SummandLevelIndex := GetIntAttr(Summand, attrLevelIndex, -1);
      SummandLevelNumber :=
        GetAxis(AxisType).GetLevelNumber(SummandAxisIndex, SummandLevelIndex);

    until SummandLevelNumber <= NodeLevelNumber;

    if (Node.nodeName = 'Members') then
      if AxisType = axRow then
        PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'rind', 0)
      else
        PairedIndex := GetIntAttr(Node.selectSingleNode('./Summary'), 'cind', -BeastNumber) + Ordinal;

    {в этом месте мы имеем значение итога для Node и можем разместить его}
    if GetAxis(AxisType)[NodeAxisIndex].Levels[NodeLevelIndex].SummaryOptions.Enabled then
      if not NaN then
        if IsTotalFactorizedBy and (Total.CountMode = mcmAvg) and
          (ElementCount > 0) and (result <> Total.EmptyValueSymbol) then
          MapSummaryValue(Node, AxisType, Index, PairedIndex,
            FloatToStr(StrToFloat(result) / ElementCount), Total)
        else
          MapSummaryValue(Node, AxisType, Index, PairedIndex, result, Total);
  end;


var
  Node: IXMLDOMNode;
  RInd, CInd, Ordinal, Cnt, (*StartColumnGrandTotal, *)ElementCount, i: integer;
  Total: TSheetTotalInterface;
  NaN: boolean;
  Summand: IXMLDOMNode;
  (*GSValue: string;*)
  TotalFactorization: array of boolean;
  TotalRange: ExcelRange;
  FormulaArray: string;
begin
  if Totals.Empty then
    exit;

  if (FWideTableMode and FWideTableSummaryMode) or not FWideTableMode then
    CommentBaseValues;
  FormulaArrayColumns := [];

  {итоги в разрезе строк}
  if Assigned(RowsDOM) then
  begin
    Node := RowsDOM.selectSingleNode('function_result/Members');
    if Assigned(Node.selectSingleNode('./Member')) then
      for CInd := Sizer.StartTotals.y to Sizer.EndTotals.y do
      begin
        Total := GetTotalByCInd(CInd);
        if Assigned(SingleTotal) and not (Total = SingleTotal) then
          continue;
        if not Assigned(Total) then
          continue;
        if not Total.SummariesByVisible then
          continue;
        ElementCount := 0;
        NaN := false;

        SetLength(TotalFactorization, Rows.Count);
        for i := 0 to Rows.Count - 1 do
          TotalFactorization[i] := Total.FactorizedBy(Rows[i]);

        { Если диапазон показателя содержит FormulaArray, то значения итогов
          выводить не надо.}
        TotalRange := GetTotalRangeByColumn(Total, CInd);
        if Assigned(TotalRange) then
          if CheckFormulaArray(TotalRange, FormulaArray) then
          begin
            Include(FormulaArrayColumns, Lo(CInd));
            continue;
          end;

        {Итоги для полностью разрушенной оси}
        if Rows.Broken then
        begin
          Summand := Node.firstChild;
          MapSummariesForBrokenAxis(Node, Total, axRow, CInd, -255,
            Rows.Count - 1, ElementCount, NaN, -1, Summand);
        end
        else
          {общий случай - ось не порушена}
          NewMapSummary(Node, Total, axRow, CInd, -255, Rows.Count - 1,
            ElementCount, NaN, true, nil, TotalFactorization);
      end;
  end;

  if FWideTableSummaryMode then
    exit;

  {итоги в разрезе столбцов}
  if Assigned(ColumnsDOM) then
  begin
    Node := ColumnsDOM.selectSingleNode('function_result/Members');
    if Assigned(Node.selectSingleNode('./Member')) then
    begin
      Cnt := Totals.CountWithPlacement(false);
      for Ordinal := 0 to Cnt - 1 do
      begin
        Total := Totals.GetWithPlacement(false, Ordinal);
        if not Assigned(Total) then
          continue;
        if not Total.SummariesByVisible then
          continue;

        SetLength(TotalFactorization, Columns.Count);
        for i := 0 to Columns.Count - 1 do
          TotalFactorization[i] := Total.FactorizedBy(Columns[i]);

        for RInd := Sizer.StartTotals.x to Sizer.EndTotals.x do
        begin
          ElementCount := 0;
          NaN := false;
          {Итоги для полностью разрушенной оси}
          if Columns.Broken then
          begin
            Summand := Node.firstChild;
            MapSummariesForBrokenAxis(Node, Total, axColumn, RInd, Ordinal,
              Columns.Count - 1, ElementCount, NaN, -1, Summand);
          end
          else
            NewMapSummary(Node, Total, axColumn, RInd, Ordinal, Columns.Count - 1,
              ElementCount, NaN, true, nil, TotalFactorization);
        end;
      end;
    end;
  end;
end;

procedure TSheetMaper.ShowTestVersionWarning;

  procedure StyleWarning(Range: ExcelRange);
  begin
    if not Assigned(Range) then
      exit;
    {$WARNINGS OFF}
    Range.UnMerge;
    Range.Locked := true;
    Range.Interior.ColorIndex := 44;
    Range.Font.ColorIndex := 11;
    Range.Font.Size := 14;
    Range.Font.Name := 'Times New Roman';
    Range.HorizontalAlignment := xlLeft;
    Range.VerticalAlignment := xlTop;
    Range.WrapText := false;
    // бордюры
    Range.Borders.LineStyle := xlContinuous;
    Range.Borders.ColorIndex :=xlAutomatic;
    Range.Borders[xlInsideHorizontal].LineStyle := xlNone;
    Range.Borders[xlInsideVertical].LineStyle := xlNone;
    Range.Borders[xlBottom].LineStyle := xlOn;
    Range.Borders[xlLeft].LineStyle := xlNone;
    Range.Borders[xlRight].LineStyle := xlNone;
    Range.Borders[xlTop].LineStyle := xlOn;
    Range.Borders[xlDiagonalDown].LineStyle := xlNone;
    Range.Borders[xlDiagonalUp].LineStyle := xlNone;
    Range.Merge(false);
    {$WARNINGS ON}
  end;

var
  Row, TwoRows: ExcelRange;
begin
  if (not IsTestVersion) then
    exit;
  Row := GetRange(ExcelSheet, 1, 1, 1, ExcelSheet.Columns.Count);
  {$WARNINGS OFF}
  Row.Insert(xlShiftDown, 0);
  Row.Insert(xlShiftDown, 0);
  Row := GetRange(ExcelSheet, 1, 1, 1, ExcelSheet.Columns.Count);
  Row.Merge(false);
  Row.Value2 := swTestVersion;
  StyleWarning(Row);
  {$WARNINGS ON}
  TwoRows := GetRange(ExcelSheet, 1, 1, 2, ExcelSheet.Columns.Count);
  MarkObject(ExcelSheet, TwoRows, sntTestMark, false);
end;

procedure TSheetMaper.ClearTableEx;

  procedure InsertRows(CountAddRows, StartRow: integer);
  var
    CountToInsert, AlreadyInserted, ExcelRowLength: integer;
    Row: ExcelRange;
  begin
    ExcelRowLength := ExcelSheet.Columns.Count;
    ExcelSheet.Application.CutCopyMode[FLCID] := xlCopy;
    CountToInsert := 1;
    AlreadyInserted := 0;
    repeat
      with Sizer do
        Row := GetRange(ExcelSheet, StartRow, 1,
          StartRow + CountToInsert - 1, ExcelRowLength);
      {$WARNINGS OFF}
      Row.Insert(xlShiftDown, 0);
      {$WARNINGS ON}
      AlreadyInserted := AlreadyInserted + CountToInsert;
      CountToInsert := CountToInsert shl 1;//удваиваем
    until CountAddRows - AlreadyInserted <= CountToInsert;
    if CountAddRows > AlreadyInserted then
    begin
      with Sizer do
        Row := GetRange(ExcelSheet, StartRow, 1,
          StartRow + CountAddRows - AlreadyInserted - 1, ExcelRowLength);
      {$WARNINGS OFF}
      Row.Insert(xlShiftDown, 0);
      {$WARNINGS ON}
    end;
  end;

  procedure DelRows(DelRange: ExcelRange; NewHeight, StartRow: integer);
  var
    RowsCount, FirstRow, LastRow: integer;
  begin
    if Assigned(DelRange) then
    begin
      RowsCount := DelRange.Rows.Count;
      FirstRow := DelRange.Row;
      LastRow := FirstRow + RowsCount - 1;
      // отключаем скрытые строки и столбцы
      DelRange.Rows.Hidden := false;
      DelRange.Columns.Hidden := false;
      {если новый диапазон больше старого, то добавим разницу}
      if (NewHeight > RowsCount) then
        InsertRows(NewHeight - RowsCount, StartRow);
      {новый диапазон меньше старого - удалим разницу}
      if (NewHeight < RowsCount) then
      begin
        DelRange := GetRange(ExcelSheet, FirstRow + NewHeight, 1, LastRow, ExcelSheet.Columns.Count);
       {$WARNINGS OFF}
        DelRange.EntireRow.Delete(xlShiftUp);
       {$WARNINGS ON}
      end;
      {очистим все строки, попадающие в новый диапазон}
      if NewHeight > 0 then
      try
        DelRange := GetRange(ExcelSheet, FirstRow, 1, FirstRow + NewHeight - 1, ExcelSheet.Columns.Count);
        DelRange.Clear;
      except
      end;
    end else
    begin
      if (NewHeight > 0) then
      begin
        InsertRows(NewHeight, StartRow);
        DelRange := GetRange(ExcelSheet, StartRow, 1, StartRow + NewHeight - 1, ExcelSheet.Columns.Count);
        try
          DelRange.Clear;
        except
        end;
      end;
    end;
  end;

  // мочим FormulaArray (он уже собран) - связано с АВ при перестройке таблицы - "удаление строки - части массива"
  procedure ClearFormulaArrays;
  var
    i, j: integer;
  begin
    for i := 0 to Totals.Count - 1 do
      for j := 0 to Totals[i].SectionCount - 1 do
      begin
        if not (Totals[i].TotalType in [wtFree, wtResult]) then
          continue;
        try
          Totals[i].GetTotalRange(j).FormulaArray := '';
        except
        end;
        try
          Totals[i].GetTotalRangeWithoutGrandSummary(j).FormulaArray := '';
        except
        end;
      end;
  end;

  // отменить объединение ячеек 
  procedure UnMergeCells;
  var
    Range: ExcelRange;
  begin
    Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTotals));
    if Assigned(Range) then
      Range.UnMerge;
  end;

var
  OldRange: ExcelRange;
  NewHeight: integer;
begin
  begin
    UnMergeCells;
    ClearFormulaArrays;
    {очистим область фильтров}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntFilterArea));
    NewHeight := Sizer.FiltersHeight;
    DelRows(OldRange, NewHeight, Sizer.StartFilters.x);

    {строка маркера}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntUnitMarker));
    NewHeight := IIF(Sizer.NeedUnitMarker, 1, 0);
    DelRows(OldRange, NewHeight, Sizer.UnitMarker.x);

    {очистим область заголовков столбцов}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntColumnTitles));
    NewHeight := IIF((IsDisplayColumnsTitles and (Columns.Count > 0)), 1, 0);
    DelRows(OldRange, NewHeight, Sizer.StartColumnsTitle.x);

    {очистим область столбцов}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntColumnsAndMPropsArea));
    //если не нашли старый диапазон, значит либо колонок совсем нет, либо версия листа старая
    if not Assigned(OldRange) then
    begin
      OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntColumns));
      if Assigned(OldRange) then
        //если нашли значит версия старая
        NewHeight := IIF((Columns.Count > 0), Sizer.ColumnsHeight, 0)
      else
        //не нашли, значит их просто не было
        NewHeight := IIF((Columns.Count > 0), Sizer.ColumnsHeight
          + Sizer.ColumnMPropsCount, 0);
    end
    else
      NewHeight := IIF((Columns.Count > 0), Sizer.ColumnsHeight
        + Sizer.ColumnMPropsCount, 0);
    DelRows(OldRange, NewHeight, Sizer.StartColumnsAndMProps.x);

    {очистим область заголовков строк/показателей}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRowTitles));
    if not Assigned(OldRange) then
      OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTotalTitles));
    if ((Rows.Count > 0) and IsDisplayRowsTitles) or
      ((Totals.Count > 0) and IsDisplayTotalsTitles) then
      NewHeight := 1
    else
      NewHeight := 0;
    DelRows(OldRange, NewHeight, IntMax(Sizer.StartRowsTitle.x,
      Sizer.StartTotalsTitle.x));

    {очистим область строк/показателей}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
    if not Assigned(OldRange) then
      OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTotals));
    if (Rows.Count > 0) or (Totals.Count > 0) then
      NewHeight := IntMax(Sizer.RowsHeight, 1)
    else
      NewHeight := 0;
    DelRows(OldRange, NewHeight, IntMax(Sizer.StartRows.x, Sizer.StartTotals.x));

    {и оставшуюся область идентификации листа}
    OldRange := GetRangeByName(ExcelSheet, BuildExcelName(sntSheetId));
    if IsDisplaySheetInfo and (Sizer.StartSheetId.x > 0) then
      NewHeight := Sizer.EndSheetId.x - Sizer.StartSheetId.x + 1 +
        Sizer.SheetIdSplit
    else
      NewHeight := 0;
    DelRows(OldRange, NewHeight, Sizer.StartSheetId.x - Sizer.SheetIdSplit);
  end;

  {Очистка имен}
  DeleteExcelNames(ExcelSheet);
end;

(*procedure TSheetMaper.RemoveEmpty(AxisDom: IXMLDOMDocument2;
  AxisType: TAxisType);

  {Собирает условие для одного элемента оси}
  function GetPartialCondition(List1, List2: TStringList; AxisIndex: integer;
    Alias, UName: string): string;
  var
    PartCondition1, PartCondition2: string;
  begin
    {частное условие для показателей, раскладываемых по элементу оси}
    PartCondition1 := '';
    if List1.Strings[AxisIndex] <> '' then
    begin
      PartCondition1 := Format('(@%s="%s")', [Alias, UName]);
      PartCondition1 := '(' + PartCondition1 + ' and ' +
        List1.Strings[AxisIndex] + ')';
    end;
    {частное условие для показателей, НЕ раскладываемых по элементу оси}
    PartCondition2 := '';
    if List2.Strings[AxisIndex] <> '' then
    begin
      PartCondition2 := Format('((not (@%s)) and %s)',
        [Alias, List2.Strings[AxisIndex]]);
    end;
    if PartCondition1 <> '' then
      if PartCondition2 <> '' then //!!    or (
        result := '(' + PartCondition1 + ' and ( not' + PartCondition2 + '))'
      else
        result := PartCondition1
    else
      result := PartCondition2;
    {дополнительная проверка на наличие данных отдельных показателей в таблице}
    result := result + Format(' or ((@%s="%s") and (@alias and @inTable))', [Alias, UName]);
    result := TupleBrackets(result);
  end;

  function GetAxisCondition(Node: IXMLDOMNode; Axis: TSheetAxisCollectionInterface;
    List1, List2: TStringList): string;
  var
    AxisIndex: integer;
    UName, PartCondition: string;
  begin
    result := '';
    repeat
      AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
      UName := GetStrAttr(Node, attrUniqueName, '');
      EncodeXPathString(UName);
      {соберем и пристыкуем условие для текущего элемента оси}
      PartCondition := GetPartialCondition(List1, List2, AxisIndex,
        Axis[AxisIndex].Alias, UName);
      AddTail(result, ' and ');
      result := result + PartCondition;
      if AxisIndex > 0 then
        while GetIntAttr(Node, attrAxisIndex, -1) >= AxisIndex do
          Node := Node.parentNode;
    until AxisIndex = 0;
  end;

  {условие для полностью нераскладываемых показателей, выводимых во всю колонку}
  function GetEntireColumnCondition(Axis: TSheetAxisCollectionInterface): string;
  var
    i: integer;
    AxisStr, TotalStr: string;
    Total: TSheetTotalInterface;
  begin
    result := '';
    if Axis.AxisType <> axRow then
      exit;
    AxisStr := '';
    for i := 0 to Axis.Count - 1 do
    begin
      AddTail(AxisStr, ' or ');
      AxisStr := AxisStr + '@' + Axis[i].Alias;
    end;
    AxisStr := IIF(AxisStr <> '', ' not ' + TupleBrackets(AxisStr), '');
    TotalStr := '';
    for i := 0 to Totals.Count - 1 do
    begin
      Total := Totals[i];
      if Total.IsIgnoredRowAxis and not Total.IsGrandTotalDataOnly then
      begin
        AddTail(TotalStr, ' or ');
        TotalStr := TotalStr + '@' + Total.Alias;
      end;
    end;
    if TotalStr <> '' then
    begin
      result := AxisStr;
      AddTail(result, ' and ');
      result := TupleBrackets(result + TupleBrackets(TotalStr));
    end;
  end;

var
  Node, RealParent: IXMLDOMNode;
  NL, ChildrenNL: IXMLDOMNodeList;
  i, j: integer;
  Condition, EntireColumnCondition,
  FileName, GrandTotalsOnly: string;
  Axis: TSheetAxisCollectionInterface;
  List1, List2: TStringList;
  tmpStr, TotalsFromBase: string;
begin
  if not Assigned(AxisDom) or
    not Assigned(TotalsData) or Totals.Empty then
    exit;
  Axis := GetAxis(AxisType);

  {!!! 25/05/07 Здесь было условие на "листовость" мемберов.
    Типа, "для показателей с расчетом итогов по видимым не нужно учитывать саммари".
    Но мы же и так делаем выборку только для листовых элементов оси.
    А определение IsRowLeaf/IsColumnLeaf дает сбой при несбалансированной
    иерархии, добитой пустышками.}

  EntireColumnCondition := GetEntireColumnCondition(Axis);
  {проставим видимость всем листовым мемберам, не принадлежащим итогам}
  NL := AxisDom.selectNodes('function_result/Members//Member[(not (Member)) and (not (Summary))]' +
    ' | function_result/Members//Summary[(not (Member)) and (not (Summary))]');
  try
    List1 := TStringList.Create;
    List2 := TStringList.Create;
    //определим раскладываемость показателей по оси
    GetFactorization(Axis, List1, List2, GrandTotalsOnly);
    for i := 0 to NL.length - 1 do
    begin
      Node := NL[i];
      if IsBelongsToSummary(Node) then
        continue;
      Condition := GetAxisCondition(Node, Axis, List1, List2);
      if AxisType = axRow then
      begin
        {и на учет главного итога}
        RealParent := GetRealParent(NL[i]);
        if Assigned(RealParent) then
          if (GetStrAttr(RealParent, 'name', '') <> stGrand) then
            if GrandTotalsOnly <> '' then
              Condition := Condition + ' and (not ' + GrandTotalsOnly + ')';
      end;
      if EntireColumnCondition <> '' then
        Condition := Condition + ' or ' + EntireColumnCondition;
      Condition := Format('function_result/data/row[%s]', [Condition]);
      {если данных нет, мембер обречен}
      if Assigned(TotalsData.selectSingleNode(Condition)) then
        (NL[i] as IXMLDOMElement).setAttribute(attrMustDie, 'false')
      else
        (NL[i] as IXMLDOMElement).setAttribute(attrMustDie, 'true');
    end;
    {теперь обработаем итоги
    если у родительского элемента есть листовые мемберы с данными, то итог
    имеет право на жизнь
    если же таких мемберов нет, то смотрим, есть ли по этому итогу данные
    показателей с "итогами из базы" - если и таких нет, итогу капут}

    TotalsFromBase := '';
    for i := 0 to Totals.Count - 1 do
      if not Totals[i].SummariesByVisible then
      begin
        AddTail(TotalsFromBase, ' or ');
        TotalsFromBase := TotalsFromBase + Format('(@%s)', [Totals[i].Alias]);
      end;
    AddTail(TotalsFromBase, ' or ');
    TotalsFromBase := TotalsFromBase + '(@alias and @inTable)';

    NL := AxisDom.selectNodes('function_result/Members//Summary');
    for i := 0 to NL.length - 1 do
    begin
      if GetStrAttr(NL[i], attrName, '') = stGrand then
        continue;
      Node := NL[i].parentNode;
      Node := Node.selectSingleNode('.//Member[(not (Member)) and (not (Summary)) and (@mustdie="false")]');
      if Assigned(Node) then
      begin
        tmpStr := 'false';
        //tmpStr := IIF(NL[i].parentNode.selectNodes('./Member[(@mustdie="false")]').length > 1,
          //'false', 'true');
        (NL[i] as IXMLDOMElement).setAttribute(attrMustDie, tmpStr);
        ChildrenNL := NL[i].selectNodes('.//Member');
        for j := 0 to ChildrenNL.length - 1 do
          (ChildrenNL[j] as IXMLDOMElement).setAttribute(attrMustDie, tmpStr);
      end
      else
      begin
        if NL[i].hasChildNodes then
          Node := NL[i].selectSingleNode('.//Member[(not (Member)) and (not (Summary))]' +
            ' | .//Summary[(not (Member)) and (not (Summary))]')
        else
          Node := NL[i];
        Condition := GetAxisCondition(Node, Axis, List1, List2);
        AddTail(Condition, ' and ');
        Condition := Condition + TotalsFromBase;
        if (GetStrAttr(NL[i], 'name', '') <> stGrand) then
          if GrandTotalsOnly <> '' then
            Condition := Condition + ' and (not ' + GrandTotalsOnly + ')';
        if EntireColumnCondition <> '' then
          Condition := Condition + ' or ' + EntireColumnCondition;
        Condition := Format('function_result/data/row[%s]', [Condition]);
        {если данных нет, мембер обречен}
        if Assigned(TotalsData.selectSingleNode(Condition)) then
          (Node as IXMLDOMElement).setAttribute(attrMustDie, 'false')
        else
          (Node as IXMLDOMElement).setAttribute(attrMustDie, 'true');
      end;
    end;
  finally
    List1.Clear;
    FreeAndNil(List1);
    List2.Clear;
    FreeAndNil(List2);
  end;
             
  KillMembersThatMustDie(Axis.FieldCount, AxisDom);

  if AddinLogEnable then
  begin
    if AxisType = axRow then
      FileName := 'Ось строк после удаления пустых.xml'
    else
      FileName := 'Ось столбцов после удаления пустых.xml';
    WriteDocumentLog(AxisDom, FileName);
  end;
end;   *)

function TSheetMaper.MapSingleCell(CellIndex: integer; var ErrorText: string;
  var BadSingles: TStringList; RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;

  procedure CommentSingleCell(Cell: TSheetSingleCellInterface;
    ERange: ExcelRange; Comment: string);
  var
    ColumnIndex, RowIndex, FirstEnter: integer;
    ResultComment: string;
  begin
    if not Assigned(ERange) then
      exit;
    RowIndex := ERange.Row;
    ColumnIndex := ERange.Column;
    if IsDisplayCommentStructuralCell then
    begin
      ResultComment := Cell.CommentText;
      if (Comment <> '') then
      begin
        FirstEnter := Pos(#10, ResultComment);
        Insert(#10 + Comment, ResultComment, FirstEnter);
      end;
      CommentCell(ExcelSheet, RowIndex, ColumnIndex, ResultComment);
    end
    else
      try
        ExcelSheet.Cells.Item[RowIndex, ColumnIndex].ClearComments;
      except
      end;
  end;

var
  SingleCell: TSheetSingleCellInterface;
  MdxQuery, MdxQueryForCache, AValue, ProcessCaption: string;
  DataDOM: IXMLDOMDocument2;
  DataNode: IXMLDOMNode;
  ERange: ExcelRange;
  AFormattedValue: Extended;
  Comment, strAddress: string;
  QueryCached: boolean;
  Constant: TConstInterface;
begin
  result := false;
  ErrorText := '';
  if CellIndex < 0 then
    exit;
  SingleCell := SingleCells[CellIndex];
  ERange := GetRangeByName(ExcelSheet, SingleCell.ExcelName);
  if not Assigned(ERange) then
  begin
    BadSingles.Add(IntToStr(CellIndex));
    result := true; //все нормально, ячейка будет удалена после рефреша
    exit;
  end;


  {$WARNINGS OFF}
  strAddress := ERange.AddressLocal[false, false, xlA1, false, varNull];
  {$WARNINGS ON}

  Comment := Format('Размещение отдельного показателя "%s" по адресу %s',
    [SingleCell.Name, strAddress]);
  ProcessCaption := IIF(IsProcessShowing, Comment, pfoQuerySingleCellMdxShort);
  OpenOperationEx(ExcelSheet.Application.Hwnd, ProcessCaption, CriticalNode, NoteTime, otQuery);
  try
    try
      if (SingleCell.TotalType = wtConst) then
      begin
        Comment := '';
        Constant := Consts.ConstByName(SingleCell.Name);
        // не нашли - попытаемся восстановить
        if not Assigned(Constant) then
          Constant := SingleCell.RepairConst;
        // не сумели - выходим с ошибкой
        if not Assigned(Constant) then
        begin
          raise Exception.Create('');
        end;
        AValue := Constant.Value;
        ERange.Value[EmptyParam] := AValue;
      end
      else
      begin
        Comment := '';
        AValue := '';
        MdxQuery := SingleCell.MdxText;
        MdxQueryForCache := StringReplace(MdxQuery, SingleCell.Alias, snBucks, [rfReplaceAll]);
        QueryCached := GetSSQueryCached(MdxQueryForCache, AValue);
        UpdateMdxLog(MdxQuery, (ExcelSheet.Parent as ExcelWorkBook).Name,
          SingleCell.Cube.ProviderId);
        if not QueryCached then
        begin
          if not DataProvider.GetRecordsetData(SingleCell.Cube.ProviderId, MDXQuery, DataDom) then
            exit;
          {Размещаем результат}
          DataNode := DataDom.selectSingleNode('function_result/data/row');
          if Assigned(DataNode) then
            AValue := GetStrAttr(DataNode, SingleCell.Alias, '');
          SSQueryCache.Add(MdxQueryforCache + '=' + AValue);
        end;
      end;

      ERange.Style := IIF(PrintableStyle, SingleCell.Styles.Name[esValuePrint],
        SingleCell.Styles.Name[esValue]);

      ERange.NumberFormat := SingleCell.NumberFormat;
      if not IsNumber(AValue) then
        UpdatePossibleNumberValue(AValue);
      if IsNumber(AValue) then
      begin
        AFormattedValue := StrToFloat(AValue);
        AFormattedValue := SingleCell.GetDividedValue(AFormattedValue);
        SingleCell.Round(AFormattedValue);
        ERange.Value2 := AFormattedValue;
       {к ячейкам типа "результат" добавляем комментарий - значение из базы}
        if (SingleCell.TotalType = wtResult) then
          Comment := 'Значение из базы: ' + FloatToStr(AFormattedValue);
      end
      else
      begin
        if Assigned(DataNode) then
          AValue := GetStrAttr(DataNode, SingleCell.Alias, '');
        ERange.Value2 := AValue;
        if (SingleCell.TotalType = wtResult) then
          Comment := 'Значение из базы: отсутствует';
      end;
      CommentSingleCell(SingleCell, ERange, Comment);
      MarkUserObject(ExcelSheet, ERange, SingleCell.GetUserExcelName, true);
      result := true;
    except
    end;
  finally
    if result then
      ErrorText := ''
    else
    begin
      if Assigned(SingleCell) then
        ErrorText :=
          Format('Ошибка при размещении отдельного показателя "%s" по адресу %s. %s',
          [SingleCell.Name, SingleCell.Address, DataProvider.LastError])
      else
        ErrorText := 'Ошибка при размещении отдельного показателя. ' +
          DataProvider.LastError;
    end;
    CloseOperationEx(result, ErrorText);
    KillDOMDocument(DataDOM);
  end;
end;

function TSheetMaper.MapSingleCells(Indexes: TStringList;
  out ErrorText: string; Standalone: boolean; RowsDom, ColumnsDom: IXMLDOMDocument2): boolean;

  // разместить формулы отдельных ячеек
  procedure MapCellsFormulas;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    CellRange: ExcelRange;
    SingleCell: TSheetSingleCellInterface;
    Alias, Formula: string;
  begin
    if not Assigned(SingleCellsData) then
      exit;
    NL := SingleCellsData.selectNodes('function_result/data/row');
    for i := 0 to NL.length - 1 do
    begin
      Alias := GetStrAttr(NL[i], attrAlias, '');
      SingleCell := SingleCells.FindByAlias(Alias);
      if not Assigned(SingleCell) then
        continue;
      if (SingleCell.TotalType <> wtResult) then
        continue;
      CellRange := GetRangeByName(ExcelSheet, SingleCell.ExcelName);
      Formula := GetStrAttr(NL[i], 'formula', '');
      if (Formula <> '') then
        CellRange.Formula := formula;
    end;
  end;

var
  i: integer;
  BadSingles: TStringList;
  NotSequel: boolean;
begin
  result := true;
  if Indexes.Count = 0 then
    exit;
  NotSequel := not IsSequelSheet(ExcelSheet);
  BadSingles := TStringList.Create;
  SSQueryCache := TStringList.Create;
  try
    for i := 0 to SingleCells.Count - 1 do
      if Indexes.IndexOf(IntToStr(i)) > -1 then
        if not MapSingleCell(i, ErrorText, BadSingles, RowsDom, ColumnsDom) then
        begin
          result := false;
          exit;
        end
        else
        begin
          if NotSequel then
            if (SingleCells[i].TotalType = wtResult) and SingleCells[i].MayBeEdited then
            try
              ExcelSheet.Protection.AllowEditRanges.Add(
                SingleCells[i].ExcelName, SingleCells[i].GetExcelRange, EmptyParam);
            except
            end;
        end;
  finally
    RemoveBadSingles(BadSingles);
    FreeStringList(BadSingles);
    FreeStringList(SSQueryCache);
  end;
  MapCellsFormulas;
  GetCellsNamesInfo;
  MakeCellsMarkup;
  if Standalone then
  begin
    ClearTestVersionWarning;
    ShowTestVersionWarning;
  end;
end;

procedure TSheetMaper.MapAll;
begin
  if not CheckConnection then
   exit;
  DeleteAllowEditRanges(ExcelSheet);
  GetHiddenColumns;
  try
    MapTable;
  finally
    DoHideColumns;
  end;
end;

function TSheetMaper.GetSingleCellsUnitedRange: ExcelRange;
var
  i: integer;
  CellRange: ExcelRange;
begin
  result := nil;
  for i := 0 to SingleCells.Count - 1 do
  if SingleCells[i].TotalType = wtResult then
  begin
    CellRange := GetRangeByName(ExcelSheet, SingleCells[i].ExcelName);
    if not Assigned(CellRange) then
      continue;
    result := GetUnionRange(result, CellRange);
  end;
end;

function TSheetMaper.GetAllSingleCellsUnitedRange: ExcelRange;
var
  i: integer;
  CellRange: ExcelRange;
begin
  result := nil;
  for i := 0 to SingleCells.Count - 1 do
  begin
    CellRange := GetRangeByName(ExcelSheet, SingleCells[i].ExcelName);
    if not Assigned(CellRange) then
      continue;
    result := GetUnionRange(result, CellRange);
  end;
end;

function TSheetMaper.GetSSQueryCached(Query: string;
  out AValue: string): boolean;
begin
  result := false;
  if not Assigned(SSQueryCache) then
    exit;
  if SSQueryCache.IndexOfName(Query) > -1 then
  begin
    AValue := SSQueryCache.Values[Query];
    result := true;
  end;
end;

procedure TSheetMaper.RemoveBadSingles(Indexes: TStringList);
var
  i: integer;
  tmpStr: string;
begin
  if Indexes.Count = 0 then
    exit;
  for i := SingleCells.Count - 1 downto 0 do
    if Indexes.IndexOf(IntToStr(i)) > -1 then
    begin
      tmpStr := Format('Отдельный показатель "%s" автоматически удален ' +
        'ввиду невозможности его размещения', [SingleCells[i].Name]);
      PostMessage(tmpStr, msgWarning);
      SingleCells.Delete(i);
    end;
end;

procedure TSheetMaper.GetHiddenColumns;
var
  i: integer;
  eRange: ExcelRange;
begin
  HiddenColumns := [];
  for i := 1 to SheetColumnsLimitation do
  begin
    eRange := GetRange(ExcelSheet, 1, i, 1, i).EntireColumn;
    if eRange.Hidden then
      Include(HiddenColumns, i - 1);
  end;
  HiddenTotalColumns := [];
end;

procedure TSheetMaper.DoHideColumns;
var
  i: integer;
  eRange: ExcelRange;
begin
  for i := 0 to 255 do
    if (i in HiddenColumns + HiddenTotalColumns) then
    begin
      eRange := GetRange(ExcelSheet, 1, i + 1, 1, i + 1).EntireColumn;
      eRange.Hidden := true;
    end;
end;

procedure TSheetMaper.ApplyElementStyles(RestyleList: TStringList);
var
  i, Index: integer;
  UID: string;
  ObjType: TSheetObjectType;
  Collection: TSheetCollection;
begin
  for i := 0 to RestyleList.Count - 1 do
  begin
    UID := RestyleList.Names[i];
    ObjType := TSheetObjectType(StrToInt(RestyleList.Values[UID]));
    case ObjType of
      wsoTotal: Collection := Totals;
      wsoRow: Collection := Rows;
      wsoColumn: Collection := Columns;
      wsoFilter: Collection := Filters;
      wsoSingleCell: Collection := SingleCells;
      else
        Collection := nil;
    end;
    if not Assigned(Collection) then
      continue;
    Index := Collection.FindById(UID);
    if Index >=0 then
      TSheetElement(Collection[Index]).ApplyStyles;
  end;
end;

procedure TSheetMaper.MapConsts;
begin
  MapTotalConsts;
  MapCellConsts;
end;

procedure TSheetMaper.MapTotalConsts;
var
  i, j: integer;
  TotalRange: ExcelRange;
  ConstValue: string;
  Constant: TConstInterface;
begin
  OpenOperationEx(ExcelSheet.Application.Hwnd, 'Размещение констант', CriticalNode, NoteTime, otQuery);
  try
    for i := 0 to Totals.Count - 1 do
    begin
      if (Totals[i].TotalType <> wtConst) then
        continue;
      for j := 0 to Totals[i].SectionCount - 1 do
      begin
        TotalRange := Totals[i].GetTotalRange(j);
        if not Assigned(TotalRange) then
          continue;
        TotalRange.NumberFormat := Totals[i].NumberFormat;
        Constant := Consts.ConstByName(Totals[i].Caption);
        if not Assigned(Constant) then
          Constant := Totals[i].RepairConst;
        ConstValue := Constant.Value;
        if IsNumber(ConstValue) then
          TotalRange.Value2 :=StrToFloat(ConstValue)
        else
          TotalRange.Value2 :=ConstValue;
      end;
    end;
  finally
    CloseOperationEx(true, '');
  end;  
end;

procedure TSheetMaper.MapCellConsts;
var
  MsgText: string;
  ConstIndicesList: TStringList;

  // получить индексы ячеек - констант
  function GetConstIndices: TStringList;
  var
    i: integer;
  begin
    result := TStringList.Create;
    for i := 0 to SingleCells.Count - 1 do
    begin
      if (SingleCells[i].TotalType <> wtConst) then
        continue;
      result.Add(IntToStr(i));
    end;
  end;

begin
  ConstIndicesList := GetConstIndices;
  try
    MapSingleCells(ConstIndicesList, MsgText, true, nil, nil);
  finally
    FreeStringList(ConstIndicesList);
  end;
end;

procedure TSheetMaper.StyleAreaByLevels(AxisType: TAxisType;
  AxisDom: IXMLDOMDocument2; What: integer);

type
   TOptionsEnum = (foLevel, foSummary, foGrand);

  function GetLevel(Node: IXMLDOMNode; out OptionsToUse: TOptionsEnum): TSheetLevelInterface;
  var
    AxisIndex, LevelIndex: integer;
    NodeType: string;
  begin
    result := nil;
    if not Assigned(Node) then
      exit;

    {если пустышка, то нужно найти реального предка}
    NodeType := GetNodetype(Node);

    OptionsToUse := foLevel;
    if (NodeType = ntSummary) or (NodeType = ntSummaryDummy) then
      OptionsToUse := foSummary;
    if (NodeType = ntMemberDummy) or (NodeType = ntSummaryDummy) then
      Node := GetRealParent(Node);
    if not Assigned(Node) then
      exit;

    if IsGrandSummary(Node) then
      OptionsToUse := foGrand;

    AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
    LevelIndex := GetIntAttr(Node, attrLevelIndex, -1);
    if (AxisIndex = -1) or (LevelIndex = -1) then
      exit;

    result := GetAxis(AxisType)[AxisIndex].Levels[LevelIndex];
  end;

var
  RInd, CInd, StartColumn, EndColumn, StartRow, EndRow: integer;
  ECell: ExcelRange;
  XPath: string;
  Node: IXMLDOMNode;
  Level: TSheetLevelInterface;
  OptionsToUse: TOptionsEnum;
begin
  if not Assigned(AxisDom) then
    exit;
  if ((AxisType = axRow) and Rows.Empty) or ((AxisType = axColumn) and Columns.Empty) then
    exit;
  if Totals.Empty and (What = 0) then
    exit;

  case What of
  0: // показатели
    begin
      StartRow := Sizer.StartTotals.x;
      StartColumn := Sizer.StartTotals.y;
      EndRow := Sizer.EndTotals.x;
      EndColumn := Sizer.EndTotals.y;
    end;
  1: // МП строк
    begin
      StartRow := Sizer.StartRowMProps.x;
      StartColumn := Sizer.StartRowMProps.y;
      EndRow := Sizer.EndRowMProps.x;
      EndColumn := Sizer.EndRowMProps.y;
    end;
  2: // МП столбцов
    begin
      StartRow := Sizer.StartColumnMProps.x;
      StartColumn := Sizer.StartColumnMProps.y;
      EndRow := Sizer.EndColumnMProps.x;
      EndColumn := Sizer.EndColumnMProps.y;
    end;
  else
    exit;
  end;

  if AxisType = axRow then
    for RInd := StartRow to EndRow do
    begin
      {найдем листовой узел с заданными координатами}
      XPath := Format('function_result/Members//*[(@rind="%d") and (@cind="%d")]',
        [RInd, Sizer.EndRows.y]);
      Node := AxisDom.selectSingleNode(XPath);
      Level := GetLevel(Node, OptionsToUse);
      if not Assigned(Level) then
        continue;

      ECell := GetRange(ExcelSheet, Rind, StartColumn, RInd, EndColumn);
      case OptionsToUse of
        foLevel: ApplyLevelFormatting(ECell, Level);
        foSummary: ApplySummaryFormatting(ECell, Level.SummaryOptions);
        foGrand: ApplySummaryFormatting(ECell, Rows.GrandSummaryOptions);
      end;
    end
  else
    for Cind := StartColumn to EndColumn do
    begin
      XPath := Format('function_result/Members//*[(@rind="%d") and (@cind="%d")]',
        [Sizer.EndColumns.x, CInd]);
      Node := AxisDom.selectSingleNode(XPath);
      Level := GetLevel(Node, OptionsToUse);
      if not Assigned(Level) then
        continue;

      ECell := GetRange(ExcelSheet, StartRow, Cind, EndRow, Cind);
      case OptionsToUse of
        foLevel: ApplyLevelFormatting(ECell, Level);
        foSummary: ApplySummaryFormatting(ECell, Level.SummaryOptions);
        foGrand: ApplySummaryFormatting(ECell, Columns.GrandSummaryOptions);
      end;
    end;
end;

procedure TSheetMaper.ApplyLevelFormatting(ERange: ExcelRange;
  Level: TSheetLevelInterface);
var
  CellValue: string;
  i, j, RCount, CCount: integer;
begin
  if not Level.UseFormat then
    exit;
  Level.FontOptions.CopyToFont(ERange.Font{Font(TVarData(ERange.Item[1, 1].Font).VDispatch)});
  if Level.AllCapitals then
  begin
    RCount := ERange.Rows.Count;
    CCount := ERange.Columns.Count;
    for i := 1 to RCount do
      for j := 1 to CCount do
      begin
        try
          CellValue := ERange.Item[i, j].value;
        except
          CellValue := '';
        end;
        if (CellValue <> '') and not IsNumber(CellValue) then
          ERange.Item[i, j].Value := AnsiUpperCase(CellValue);
      end;
  end;
(*  begin
    try
      CellValue := ERange.Item[1, 1].value;
    except
      CellValue := '';
    end;
    if (CellValue <> '') and not IsNumber(CellValue) then
      ERange.Item[1, 1].Value := AnsiUpperCase(CellValue);
  end;*)
end;

procedure TSheetMaper.ApplySummaryFormatting(ERange: ExcelRange;
  SummaryOptions: TSummaryOptions);
var
  CellValue: string;
  i, j, RCount, CCount: integer;
begin
  SummaryOptions.FontOptions.CopyToFont(ERange.Font{Font(TVarData(ERange.Item[1, 1].Font).VDispatch)});
  if SummaryOptions.AllCapitals then
  begin
    RCount := ERange.Rows.Count;
    CCount := ERange.Columns.Count;
    for i := 1 to RCount do
      for j := 1 to CCount do
      begin
        try
          CellValue := ERange.Item[i, j].value;
        except
          CellValue := '';
        end;
        if (CellValue <> '') and not IsNumber(CellValue) then
          ERange.Item[i, j].Value := AnsiUpperCase(CellValue);
      end;
  end;
(*  begin
    try
      CellValue := ERange.Item[1, 1].value;
    except
      CellValue := '';
    end;
    if (CellValue <> '') and not IsNumber(CellValue) then
      ERange.Item[1, 1].Value := AnsiUpperCase(CellValue);
  end;*)
end;

{во многом схожа с процедурой StyleAreaByLevels, но объединить затруднительно}
procedure TSheetMaper.MarkResultsGrey(RowsDom, ColumnsDom: IXMLDOMDocument2);
var
  RInd, CInd, i, StartColumn, EndColumn, StartRow, EndRow, CountWP: integer;
  Node: IXMLDOMNode;
  ECell: ExcelRange;
  XPath: string;
  Senseless: boolean;
begin
  if not Totals.CheckByType([wtResult, wtFree]) then
    exit;
  if PrintableStyle then
    exit;

  StartRow := Sizer.StartTotals.x;
  StartColumn := Sizer.StartTotals.y;
  EndRow := Sizer.EndTotals.x;
  EndColumn := Sizer.EndTotals.y;

  if Assigned(RowsDom) then
    for RInd := StartRow to EndRow do
    begin
      (*{найдем листовой узел с заданными координатами}
      XPath := Format('function_result/Members//*[(@rind="%d") and (@cind="%d")]',
        [RInd, Sizer.EndRows.y]);
      Node := RowsDom.selectSingleNode(XPath);
      Senseless := IsWritebackSenseless(Node);    *)
      Node := RowsMarkup.documentElement.childNodes[RInd - StartRow];
      Senseless := not GetBoolAttr(Node, attrWbWorthy, false);
      if Senseless then
        for CInd := StartColumn to EndColumn do
          if WritablesInfo.IsColumnWritable(CInd - StartColumn) and
            not WritablesInfo.IsSingleCellSelected(ExcelSheet, RInd, Cind) then
          begin
            ECell := ExcelRange(TVarData(ExcelSheet.Cells.Item[Rind, CInd]).VDispatch);
            ECell.Interior.ColorIndex := 15;
          end;
    end;
  if Assigned(ColumnsDom) then
  begin
    CountWP := Totals.CountWithPlacement(false);
    if CountWP = 0 then
      exit;
    CInd := StartColumn;
    EndColumn := Sizer.EndColumns.y;
    while CInd <= EndColumn do
    begin
      XPath := Format('function_result/Members//*[(@rind="%d") and (@cind="%d")]',
        [Sizer.EndColumns.x, CInd]);
      Node := ColumnsDom.selectSingleNode(XPath);
      for i := CInd to (CInd + CountWP - 1) do
        if WritablesInfo.IsColumnWritable(i - StartColumn) then
        begin
          Senseless := IsWritebackSenseless(Node);
          if Senseless then
            for Rind := StartRow to EndRow do
              if not WritablesInfo.IsSingleCellSelected(ExcelSheet, RInd, i) then
              begin
                ECell := ExcelRange(TVarData(ExcelSheet.Cells.Item[Rind, i]).VDispatch);
                ECell.Interior.ColorIndex := 15;
              end;
        end;
      inc(CInd, CountWP);
    end;
  end;
end;

function TSheetMaper.GetTotalByCInd(CInd: integer): TSheetTotalInterface;
var
  wPlacement, Shift: integer;
begin
  //кол-во показателей, укладывающихся в ось
  wPlacement := Totals.CountWithPlacement(false);
  //смещение показателя
  Shift := CInd - Sizer.StartTotals.y;
  if Sizer.ColumnsHeight > 0 then
    if wPlacement > 0 then
      if CInd <= Sizer.EndColumns.y then
        result := Totals.GetWithPlacement(false, Shift mod wPlacement)
      else
        result := Totals.GetWithPlacement(true, CInd - Sizer.EndColumns.y - 1)
    else
      if CInd <= Sizer.EndColumns.y then
        result := nil
      else
        result := Totals[CInd - Sizer.EndColumns.y - 1]
  else //столбцов нет, все показатели в одном экземпляре
    if Shift < wPlacement then
      result := Totals.GetWithPlacement(false, Shift)
    else
      result := Totals.GetWithPlacement(true, Shift - wPlacement);
end;

procedure TSheetMaper.CommentBaseValues;
var
  Row, Column: integer;
  StartPoint, EndPoint: TPoint;
  Formula, BaseValue: string;
  Total: TSheetTotalInterface;
  Cell: OleVariant;
begin
  if not IsDisplayCommentDataCell then
    exit;
  StartPoint := Sizer.StartTotals;
  EndPoint := Sizer.EndTotals;

  for Column := StartPoint.y to EndPoint.y do
  begin
    Total := GetTotalByCInd(Column);
    if not Assigned(Total) then
      continue;
    if not (Total.TotalType in [wtMeasure, wtResult]) then
      continue;
    for Row := StartPoint.x to EndPoint.x do
    begin
      Cell := ExcelSheet.Cells.Item[Row, Column];
      Formula := Cell.Formula;
      if Pos('=', Formula) = 1 then
        continue;
      BaseValue := '';
      if Cell.Text <> '' then
      try
        BaseValue := Cell.Value;
        if (IsNumber(BaseValue) and (BaseValue <> Total.EmptyValueSymbol))then
          AddCellComment(ExcelSheet, Row, Column,
            'Значение из базы (по всем элементам): ' + BaseValue)
      except
      end;
    end;
  end;
end;

procedure TSheetMaper.WriteMaskedValue(RInd, CInd: integer; Value,
  Mask: string);
begin
  WriteMaskedValue(ExcelSheet.Cells.Item[RInd, CInd], Value, Mask);
end;

procedure TSheetMaper.WriteMaskedValue(Range: OleVariant; Value,
  Mask: string);
begin
  try
    if (Mask <> '') and (Length(Value) <= 15) then
      Range.NumberFormat := Mask;
    if Value <> '' then
      Range.Value := Value;
  except
  end;
end;

procedure TSheetMaper.MarkMPArea(AxisType: TAxisType);
var
  TitleStyleName, BodyStyleName, AreaMarkupName, PropertyMarkupName: string;
  Body, MPRange: ExcelRange;
  AxCollection: TSheetAxisCollectionInterface;
  StartMPTitle, StartMP, EndMP: TPoint;
  DisplayTitles: boolean;
  MPChecked: integer;

  AxisCounter, MPCounter: integer;
begin
    AxCollection := GetAxis(AxisType);
    {Настраиваем переменные в зависимости от оси}
    if AxisType = axRow then
    begin
      StartMPTitle := Sizer.StartRowMPropsTitle;
      StartMP := Sizer.StartRowMProps;
      EndMP := Sizer.EndRowMProps;
      DisplayTitles := IsDisplayRowsTitles;
      AreaMarkupName := sntRowsMPArea;
    end
    else //это колонки
    begin
      StartMPTitle := Sizer.StartColumnMPropsTitle;
      StartMP := Sizer.StartColumnMProps;
      EndMP := Sizer.EndColumnMProps;
      DisplayTitles := IsDisplayColumnsTitles;
      AreaMarkupName := sntColumnsMPArea;
    end;

    if not Assigned(AxCollection) then
      exit;
    if AxCollection.MPCheckedCount = 0 then
      exit;

    if PrintableStyle then
    begin
      TitleStyleName := snFieldTitlePrint;
      BodyStyleName := snMemberPropertiesPrint;
    end
    else
    begin
      TitleStyleName := snFieldTitle;
      BodyStyleName := snMemberProperties;
    end;

    {Накладываем стиль и имя на всю область}
    Body := GetRange(ExcelSheet, StartMP, EndMP);
    if Assigned(Body) then
    begin
      Body.Style := BodyStyleName;
      MarkObject(ExcelSheet, Body, AreaMarkupName, false);
    end;

    MPChecked := -1;
    for AxisCounter := 0 to AxCollection.Count - 1 do
      for MPCounter := 0 to AxCollection[AxisCounter].MemberProperties.Count - 1 do
        if AxCollection[AxisCounter].MemberProperties[MPCounter].Checked then
        begin
          inc(MPChecked);
          {выводим заголовок и комментарий}
          with ExcelSheet.Cells do
            if DisplayTitles then
            begin
              Item[StartMPTitle.x, StartMPTitle.y + MPChecked].Value :=
                AxCollection[AxisCounter].MemberProperties[MPCounter].Name;
              Item[StartMPTitle.x, StartMPTitle.y + MPChecked].Style := TitleStyleName;
              if IsDisplayCommentStructuralCell then
                CommentCell(ExcelSheet, StartMPTitle.x, StartMPTitle.y + MPChecked,
                            'Свойство измерения "' + AxCollection[AxisCounter].FullDimensionName2 + '"');
            end;
          {размечаем диапазон МР}
          if AxisType = axRow then
          begin
            MPRange := GetRange(ExcelSheet, StartMP.x, StartMP.y + MPChecked,
              EndMP.x, StartMP.y + MPChecked);
            PropertyMarkupName := ConstructName(sntRowsMP,
              AxCollection[AxisCounter].UniqueID, IntToStr(MPCounter), '');
          end
          else
          begin
            MPRange := GetRange(ExcelSheet, StartMP.x + MPChecked, StartMP.y,
              StartMP.x + MPChecked, EndMP.y);
            PropertyMarkupName := ConstructName(sntColumnsMP,
              AxCollection[AxisCounter].UniqueID, IntToStr(MPCounter), '');
          end;
          MarkObject(ExcelSheet, MPRange, PropertyMarkupName, false);
        end;

end;

type
  { Просто для удобства передачи параметров факторизации показателей}
  TFactorizationInfo = class
    Decomposables, // Список раскладываемых показателей
    Indecomposables, // Список нераскладываемых показателей
    AxisAliases: TStringList; //  Список алиасов измерений оси
    GrandTotalsOnly, LeafCondition: string;
    BaseTotalsPresent: boolean;
    constructor Create;
    destructor Destroy; override;
  end;

  constructor TFactorizationInfo.Create;
  begin
    Decomposables := TStringList.Create;
    Indecomposables := TStringList.Create;
    AxisAliases := TStringList.Create;
  end;

  destructor TFactorizationInfo.Destroy;
  begin
    inherited Destroy;
    FreeStringList(Decomposables);
    FreeStringList(Indecomposables);
    FreeStringList(AxisAliases);
  end;

(* 6945
function CheckConstants(AxisType: TAxisType; Totals: TSheetTotalCollectionInterface): boolean;
var
  List1, List2: TStringList;
begin
  { Показатели-константы в разрезе строк всегда выводятся во весь
    столбец целиком. Таким образом, в строках скрывать просто нечего.}
  if AxisType = axRow then
    result := Totals.CheckByType([wtConst])
  else
  { А для столбцов нужно смотреть не игнорируют ли их}
  try
    result := Totals.GetTotalLists(List1, List2, [wtConst]);
    if result then
      result := List1.Count > 0;
  finally
    FreeStringList(List1);
    FreeStringList(List2);
  end;
end;*)

{ Удаляет из собранной оси элементы, для которых в таблице не будет данных -
  т.е. "пустые строки".
  Предназначена для обычного режима оптимизации и неразрушенной иерархии
  всей оси в целом (иерархии измерений значения не имеют).

  По структуре и принципу работы практически полностью аналогична
  процедуре подсчета итогов. Тот же самый механизм рекурсии по
  детализирующим узел элементам. Поэтому в отдалённой перспективе возможно
  объединение этих процедур в одну - расчет итогов будет выполняться сразу
  после запроса данных и сбора осей, с занесением результатов в датадом
  показателей. Впрочем, трудоемкость такой задачи весьма нехилая.

  - Стрижаков, 11.07.07}
procedure TSheetMaper.RemoveEmpty(AxisDom: IXMLDOMDocument2;
  AxisType: TAxisType);

  { В зависимости от детализирущих элементов проставляет узлу атрибут attrMustDie}
  function CheckNodeEmptiness(
    Node: IXMLDOMNode;  // рассматриваемы узел
    const LastAxisIndex: integer; // индекс последнего измерения оси
    ParentLeaf: boolean;
    HierarchyBreaker: IXMLDOMNode;
    FR, ALterFR: TFactorizationInfo;
    Aliases: TStringList;
    UNamesComma: string
    ): boolean;

    { Проверка на "маздайность" для узла, который формально является листом,
      а на деле - итогом}
    function CheckFakeLeafEmptiness: boolean;
    var
      XPath, UName, ParentUName, BreakerUName: string;
      tmpNode, ParentNode: IXMLDOMNode;
      i, CurNodeAxisIndex, ParentNodeAxisIndex: integer;
      NL: IXMLDOMNodeList;
      Value: boolean;
    begin
      result := true;
      {Соберем XPath для адресации к текущему листовому элементу
        относительно узла, сломавшего иерархию. Применение данного XPath
        к детализирующим узлам брейкера даст искомые "реальные" листовые
        элементы.}
      UName := GetStrAttr(Node, attrUniqueName, '');
      EncodeXPathString(UName);
      XPath := Format('Member[@%s="%s"]', [attrUniqueName, UName]);
      CurNodeAxisIndex := GetIntAttr(Node, attrAxisIndex, -1);

      ParentNode := GetParentNode(Node);
      ParentUName := GetStrAttr(ParentNode, attrUniqueName, '');
      ParentNodeAxisIndex := GetIntAttr(ParentNode, attrAxisIndex, -1);
      BreakerUName := GetStrAttr(HierarchyBreaker, attrUniqueName, '');
      while ParentUName <> BreakerUName do
      begin

        if not ((CurNodeAxisIndex = ParentNodeAxisIndex) and
          GetAxis(AxisType)[CurNodeAxisIndex].IgnoreHierarchy) then
        begin
          EncodeXPathString(ParentUName);
          XPath := Format('Member[@%s="%s"]/', [attrUniqueName, ParentUName]) + XPath;
        end;

        CurNodeAxisIndex := ParentNodeAxisIndex;
        ParentNode := GetParentNode(ParentNode);
        ParentUName := GetStrAttr(ParentNode, attrUniqueName, '');
      end;
      XPath := './' + XPath;

      {Теперь идем по детализирующим узлам брейкера}
      EncodeXPathString(BreakerUName);
      NL := HierarchyBreaker.parentNode.selectNodes(
        Format('./Member[@%s="%s"]', [attrParentUN, BreakerUName]));
      if NL.length > 0 then
      begin
        for i := 0 to NL.length - 1 do
        begin
          tmpNode := NL[i].selectSingleNode(XPath);
          Value := GetBoolAttr(tmpNode, attrMustDie, true{false});
          result := result and Value;
        end;
      end;
    end;

  var
    AxisIndex, LevelIndex, LeafIndex, i: integer;
    UName, XPath: string;
    AxisElement: TSheetAxisElementInterface;
    NL: IXMLDOMNodeList;
    SummandAxisIndex, SummandLevelIndex: integer;
    IsNextLevel, IsNextAxis, Value: boolean;
    SummaryNode: IXMLDOMNode;
    UNames: TStringList;
  begin
    Unames := TStringList.Create;
    UNames.CommaText := UNamesComma;
    try
      result := true;
      AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
      LevelIndex := GetIntAttr(Node, attrLevelIndex, -1);
      LeafIndex := GetIntAttr(Node, attrMemberLeaf, -BeastNumber);
      if Node.nodeName = 'Members' then
        AxisIndex := 0;
      AxisElement := GetAxis(AxisType)[AxisIndex];
      UName := GetStrAttr(Node, attrUniqueName, '');
      EncodeXPathString(UName);
      { На листовых элементах измерений модифицируем условие выборки данных.}
      if LeafIndex <> -BeastNumber then
      begin
        UNames[AxisIndex] := UName;
      end;

      { Проверяем на "маздайность" листовые узлы оси}
      if (AxisIndex = LastAxisIndex) and (LeafIndex = AxisIndex) and ParentLeaf then
      begin
        if not Assigned(HierarchyBreaker) then
        begin //настоящий лист
          XPath := GetDataSelectionXPath(Aliases, UNames,
            FR.Decomposables, FR.Indecomposables, FR.LeafCondition);
          result := not Assigned(TotalsData.selectSingleNode(XPath));
        end
        else  //фиктивный лист
          result := CheckFakeLeafEmptiness;
        (Node as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(result));
        {возможно, что элемент добит пустышками...}
        if Node.hasChildNodes then
        begin
          NL := Node.selectNodes('.//Member');
          for i := 0 to NL.length - 1 do
            (NL[i] as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(result));
        end;
        exit;
      end;

      if not AxisElement.IgnoreHierarchy or
        (AxisElement.IgnoreHierarchy and ((LeafIndex <> -BeastNumber))) then
      begin
        NL := Node.selectNodes('./Member');
        for i := 0 to NL.length - 1 do
        begin
          { Не все дочерние узлы являются детализирующими}
          SummandAxisIndex := GetIntAttr(NL[i], attrAxisIndex, -1);
          SummandLevelIndex := GetIntAttr(NL[i], attrLevelIndex, -1);
          IsNextLevel := (SummandAxisIndex = AxisIndex) and
            (SummandLevelIndex - LevelIndex = 1);
          IsNextAxis := (SummandAxisIndex - AxisIndex = 1) and
            not Assigned(NL[i].attributes.getNamedItem(attrParentUN)) and
            (Node.nodeName <> 'Members');
          if IsNextAxis then
            ParentLeaf := GetIntAttr(Node, attrMemberLeaf, -1) > -1;

          Value := CheckNodeEmptiness(NL[i], LastAxisIndex, ParentLeaf,
            HierarchyBreaker, FR, AlterFR, Aliases, UNames.CommaText);

          if (IsNextLevel or IsNextAxis) then
            result := result and Value;
        end;
      end
      else
      { Иерархия измерения сломана и элемент не является листовым}
      begin
        if (Node.nodeName = 'Members') then
          NL := Node.selectNodes(Format('./Member[not(@%s)]', [attrParentUN]))
        else
          NL := Node.parentNode.selectNodes(Format('./Member[@%s="%s"]', [attrParentUN, UName]));
        { Определим живучесть элемента по детализирующим его узлам.}
        for i := 0 to NL.length - 1 do
        begin
          Value := CheckNodeEmptiness(NL[i], LastAxisIndex, ParentLeaf,
            HierarchyBreaker, FR, AlterFR, Aliases, UNames.CommaText);
          result := result and Value;
        end;
        { В случае если, по этому узлу есть данные показателей, берущихся из базы,
          то скрывать его нельзя. }
        if result and FR.BaseTotalsPresent then
        begin
          if (Node.nodeName <> 'Members') then
            UNames[AxisIndex] := UName;
          XPath := GetDataSelectionXPath(Aliases, UNames,
            AlterFR.Decomposables, AlterFR.Indecomposables, AlterFR.LeafCondition);
          result := not Assigned(TotalsData.selectSingleNode(XPath));
        end;

        {теперь надо идти вглубь оси, указав текущий узел в качестве
          "разрушителя иерархии"}
        if not Assigned(HierarchyBreaker) and (Node.nodeName <> 'Members')
          and (AxisIndex < LastAxisIndex) then
        begin
          NL := Node.selectNodes('./Member');
          UNames[AxisIndex] := UName;
          for i := 0 to NL.length - 1 do
            CheckNodeEmptiness(NL[i], LastAxisIndex, ParentLeaf,
              Node, FR, AlterFR, Aliases, UNames.CommaText);
        end;
      end;

      { Приговор вынесен}
      (Node as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(result));

      { Теперь решим вопрос о маздайности итога по текущему элементу.}
      SummaryNode := Node.selectSingleNode('Summary');
      if Assigned(SummaryNode) then
      begin
        { Если сам элемент не пуст, то итог оставляем}
        if not result then
        begin
          (SummaryNode as IXMLDOMElement).setAttribute(attrMustDie, 'false');
          exit;
        end;

        Value := true;
        if FR.BaseTotalsPresent then
        begin
          if (Node.nodeName <> 'Members') then
            UNames[AxisIndex] := UName;
          XPath := GetDataSelectionXPath(Aliases, UNames,
            AlterFR.Decomposables, AlterFR.Indecomposables, AlterFR.LeafCondition);
          Value := not Assigned(TotalsData.selectSingleNode(XPath));
        end;
        (SummaryNode as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(Value));
        NL := SummaryNode.selectNodes('.//*');
        for i := 0 to NL.length - 1 do
          (NL[i] as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(Value));
      end;
    finally
      FreeStringList(UNames);
    end;
  end;

var
  Axis: TSheetAxisCollectionInterface;
  Node: IXMLDOMNode;
  FileName: string;
  Aliases, UNames: TStringList;
  FR, AlterFR: TFactorizationInfo;
  i: integer;
begin
  if not Assigned(AxisDom) or
    not Assigned(TotalsData) or Totals.Empty then
    exit;
  (*if CheckConstants(AxisType, Totals) then
    exit;*)
  Axis := GetAxis(AxisType);

  { Заполним структуры, описывающие разлагаемость показателей по измерениям оси.}
  FR := TFactorizationInfo.Create;
  ALterFR := TFactorizationInfo.Create;
  Aliases := TStringList.Create;
  UNames := TStringList.Create;
  try
    GetFactorization(Axis, false, FR.Decomposables,
      FR.Indecomposables, FR.GrandTotalsOnly);
    for i := 0 to Axis.Count - 1 do
    begin
      FR.AxisAliases.Add(Axis[i].Alias);
      Aliases.Add(Axis[i].Alias);
      UNames.Add(Axis[i].AllMember);
      if UNames[i] = '' then
        UNames[i] := '$$$'
    end;
    FR.BaseTotalsPresent := Totals.AreSummariesImperative;
    FR.LeafCondition := Totals.GetLeafCondition(true, true);
    if FR.BaseTotalsPresent then
    begin
      GetFactorization(Axis, true, AlterFR.Decomposables,
        AlterFR.Indecomposables, AlterFR.GrandTotalsOnly);
      AlterFR.BaseTotalsPresent := FR.BaseTotalsPresent;
      AlterFR.AxisAliases.Assign(FR.AxisAliases);
      AlterFR.LeafCondition := FR.LeafCondition;
    end;

    Node := AxisDom.selectSingleNode('function_result/Members');
    CheckNodeEmptiness(Node, Axis.Count - 1, true, nil,
      FR, AlterFR, Aliases, UNames.CommaText);

  finally
    FR.Free;
    AlterFR.Free;
  end;

  KillMembersThatMustDie(Axis.FieldCount, AxisDom);

  if AddinLogEnable then
  begin
    if AxisType = axRow then
      FileName := 'Ось строк после удаления пустых.xml'
    else
      FileName := 'Ось столбцов после удаления пустых.xml';
    WriteDocumentLog(AxisDom, FileName);
  end;
end;

procedure TSheetMaper.RemoveEmptyForBrokenAxis(AxisDom: IXMLDOMDocument2;
  AxisType: TAxisType);

  function GetCondition(Node: IXMLDOMNode; FR: TFactorizationInfo): string;
  var
    i, AxisIndex: integer;
    Alias, UName: string;
    AL: IXMLDOMNodeList;
    Aliases, UNames: TStringList;
  begin
    Aliases := TStringList.Create;
    UNames := TStringList.Create;
    AL := Node.selectNodes('AliasInfo');
    for i := 0 to AL.length - 1 do
    begin
      Aliases.Add('');
      UNames.Add('');
    end;
    for i := 0 to AL.length - 1 do
    begin
      {каждый узел AliasInfo содержит один-единственный атрибут - алиас
        элемента оси, а его значение - юникнейм мембера}
      Alias := (AL[i] as IXMLDOMElement).attributes.item[0].nodeName;
      UName := (AL[i] as IXMLDOMElement).attributes.item[0].nodeValue;
      EncodeXPathString(UName);
      AxisIndex := FR.AxisAliases.IndexOf(Alias);
      Aliases[AxisIndex] := Alias;
      UNames[AxisIndex] := UName;
    end;
    result := GetDataSelectionXPath(Aliases, UNames, FR.Decomposables,
      FR.Indecomposables, FR.LeafCondition);
  end;

  function CheckNodeEmptiness(Node: IXMLDOMNode;
    FR, AlterFR: TFactorizationInfo;
    NodeLevelNumber: integer;
    var Summand: IXMLDOMNode
    ): boolean;
  var
    Condition: string;
    Value: boolean;
    {численные метрики очередного слагаемого}
    SummandAxisIndex, SummandLevelIndex, SummandLevelNumber: integer;
    tmpNode: IXMLDOMNode;
  begin
    result := true;

    { Если это листовой элемент последнего измерения в оси}
    if (GetIntAttr(Node, attrMemberLeaf, BeastNumber) = FR.AxisAliases.Count - 1) then
    begin
      Condition := GetCondition(Node, FR);
      result := not Assigned(TotalsData.selectSingleNode(Condition));
    end
    else
    begin
      {узнаем порядковый номер уровня слагаемого}
      SummandAxisIndex := GetIntAttr(Summand, attrAxisIndex, -1);
      SummandLevelIndex := GetIntAttr(Summand, attrLevelIndex, -1);
      SummandLevelNumber := GetAxis(AxisType).GetLevelNumber(SummandAxisIndex, SummandLevelIndex);

      repeat
        {нам нужны элементы следующего уровня - каким бы он ни был}
        if SummandLevelNumber > NodeLevelNumber then
        begin
          if not Assigned(Summand) then
            break;
          tmpNode := Summand;
          Summand := Summand.nextSibling;
          Value := CheckNodeEmptiness(tmpNode, FR, AlterFR,
            SummandLevelNumber, Summand);
          result := result and Value;
        end;

        {проверка на то, что достигли последнего элемента оси}
        if not Assigned(Summand) then
          break;

        SummandAxisIndex := GetIntAttr(Summand, attrAxisIndex, -1);
        SummandLevelIndex := GetIntAttr(Summand, attrLevelIndex, -1);
        SummandLevelNumber :=
          GetAxis(AxisType).GetLevelNumber(SummandAxisIndex, SummandLevelIndex);

      until SummandLevelNumber <= NodeLevelNumber;
      { Если все детализирующие пусты, то у текущего узла последний шанс уцелеть.
        Выясним, есть ли по этому узлу данные показателей, берущихся из базы.
        Если есть, то скрывать его нельзя.}
      if result and FR.BaseTotalsPresent and (Node.nodeName <> 'Members')
        and not IsBelongsToSummary(Node) then
      begin
        Condition := GetCondition(Node, AlterFR);
        result := not Assigned(TotalsData.selectSingleNode(Condition));
      end;

    end;
    if Node.nodeName = 'Members' then
      Node := Node.selectSingleNode('./Summary');
    if Assigned(Node) then
      (Node as IXMLDOMElement).setAttribute(attrMustDie, BoolToStr(result));
  end;

var
  Axis: TSheetAxisCollectionInterface;
  i: integer;
  Node, Summand: IXMLDOMNode;
  NL: IXMLDOMNodeList;
  FR, AlterFR: TFactorizationInfo;
begin
  if not Assigned(AxisDom) or not Assigned(TotalsData) or Totals.Empty then
    exit;
  Axis := GetAxis(AxisType);
  (*if CheckConstants(AxisType, Totals) then
    exit;*)
  if not Axis.Broken then
    exit;

  { Заполним структуры, описывающие разлагаемость показателей по измерениям оси.}
  FR := TFactorizationInfo.Create;
  AlterFR := TFactorizationInfo.Create;
  try
    GetFactorization(Axis, false, FR.Decomposables,
      FR.Indecomposables, FR.GrandTotalsOnly);
    for i := 0 to Axis.Count - 1 do
      FR.AxisAliases.Add(Axis[i].Alias);
    FR.BaseTotalsPresent := Totals.AreSummariesImperative;
    FR.LeafCondition := Totals.GetLeafCondition(true, true);
    if FR.BaseTotalsPresent then
    begin
      GetFactorization(Axis, true, AlterFR.Decomposables,
        AlterFR.Indecomposables, AlterFR.GrandTotalsOnly);
      AlterFR.BaseTotalsPresent := FR.BaseTotalsPresent;
      AlterFR.AxisAliases.Assign(FR.AxisAliases);
    end;

    Node := AxisDom.selectSingleNode('function_result/Members');
    Summand := Node.firstChild;
    CheckNodeEmptiness(Node, FR, AlterFR, -1, Summand);
    {а вот теперь разом убиваем их всех :)}
    NL := AxisDom.selectNodes('function_result/Members/*[@mustdie="true"]');
    for i := NL.length - 1 downto 0 do
    begin
      Node := NL[i].parentNode.removeChild(NL[i]);
      Node := nil;
    end;
    if AddinLogEnable then
      WriteDocumentLog(AxisDom, IIF(AxisType = axRow,
        'Ось строк после удаления пустых.xml',
        'Ось столбцов после удаления пустых.xml'));
  finally
    FR.Free;
    AlterFR.Free;
  end;
end;

procedure TSheetMaper.GetCellsNamesInfo;
var
  i: integer;
begin
  WritablesInfo.ClearCells;
  for i := 0 to SingleCells.Count - 1 do
    WritablesInfo.Add(SingleCells[i]);
end;

procedure TSheetMaper.GetWritableTotalsInfo;
var
  CInd: integer;
  Total: TSheetTotalInterface;
  Offset: integer;
begin
  WritablesInfo.ClearColumns;
  Offset := Sizer.StartTotals.y;
  for CInd := Sizer.StartTotals.y to Sizer.EndTotals.y do
  begin
    Total := GetTotalByCInd(CInd);
    if Assigned(Total) then
      if Total.TotalType in [wtResult, wtFree] then
        WritablesInfo.Add(Total, Lo(CInd - Offset));
  end;
end;

procedure TSheetMaper.UpdateFiltersText;
var
  i: integer;
  ERange: ExcelRange;
  EName: string;
begin
  for i := 0 to Filters.Count - 1 do
  begin
    if Filters[i].IsPartial then
      continue;
    EName := Filters[i].ExcelName;
    ERange := GetRangeByName(ExcelSheet, EName);
    if not Assigned(ERange) then
      continue;
    try
      if IsDisplayCommentStructuralCell then
      begin
        CommentCell(ExcelSheet, ERange.Row, ERange.Column, Filters[i].CommentText);
      end;
      ERange.Item[2, 1].Value := Filters[i].Text;
    except
      {По задаче 9951}
    end;
  end;
end;

procedure TSheetMaper.UpdateTotalsComments;
var
  i, Instance: integer;
  AName, NewText: string;
  ERange: ExcelRange;
begin
  if not IsDisplayCommentStructuralCell then
    exit;
  for i := 0 to Totals.Count - 1 do
  begin
    NewText := Totals[i].CommentText;
    for Instance := 0 to 255 do
    begin
      if IsDisplayTotalsTitles then
        AName := Totals[i].TitleExcelName + snSeparator + IntToStr(Instance)
      else
        AName := Totals[i].GetFullExcelName(Instance);
      ERange := GetRangeByName(ExcelSheet, AName);
      if not Assigned(ERange) then
        break;
      CommentCell(ExcelSheet, ERange.Row, ERange.Column, NewText);
    end;
  end;
end;

procedure TSheetMaper.PrepareToSwitchOffline;
begin
  FIsLastRefreshBeforeOffline := true;
  { Чтобы свойства элементов MayBeEdited возвращали false}
  InConstructionMode := false;
end;

procedure TSheetMaper.UpdateAllowEditRanges;
var
  i, StartColumn, EndColumn, ColumnIndex, TotalSectionIndex: integer;
  AllTotalsRange, TotalRange: ExcelRange;
  TotalInterface: TSheetTotalInterface;
  NeedProtect: boolean;
begin
  if WritablesInfo.NoWritableColumns then
    exit;
  AllTotalsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTotals));
  if not Assigned(AllTotalsRange) then
    exit;

  NeedProtect := IsSheetProtected(ExcelSheet);
  if NeedProtect then
    if not SetSheetProtection(ExcelSheet, false) then
      exit;

  DeleteAllowEditRanges(ExcelSheet);

  StartColumn := AllTotalsRange.Column;
  EndColumn := StartColumn + AllTotalsRange.Columns.Count - 1;
  for ColumnIndex := StartColumn to EndColumn do
  begin
    TotalInterface := Totals.FindByColumn(ColumnIndex, TotalSectionIndex);
    if not Assigned(TotalInterface) then
      continue;
    if (TotalInterface.TotalType in [wtResult, wtFree]) then
      if TotalInterface.MayBeEdited then
      begin
        TotalRange := TotalInterface.GetTotalRangeByColumn(ColumnIndex);
        ExcelSheet.Protection.AllowEditRanges.Add(
          TotalInterface.GetFullExcelName(TotalSectionIndex), TotalRange, EmptyParam);
      end;
  end;

  for i := 0 to SingleCells.Count - 1 do
    if (SingleCells[i].TotalType = wtResult) and SingleCells[i].MayBeEdited then
      ExcelSheet.Protection.AllowEditRanges.Add(
        SingleCells[i].ExcelName, SingleCells[i].GetExcelRange, EmptyParam);

  if NeedProtect then
    SetSheetProtection(ExcelSheet, true);
end;

procedure TSheetMaper.MapTypeFormulas;
var
  i, Section, Row, Column, GrandSummaryRow: integer;
  Total: TSheetTotalInterface;
  TypeFormula: TTypeFormula;
  TotalRange: ExcelRange;
  DecodedTypeFormula: string;
begin
  GrandSummaryRow := GetGrandSummaryRow(ExcelSheet);
  for i := 0 to Totals.Count - 1 do
  begin
    Total := Totals[i];
    TypeFormula := Total.TypeFormula;
    if not(TypeFormula.Enabled and (TypeFormula.Template <> '')) then
      continue;

    for Section := 0 to Total.SectionCount - 1 do
    begin
      TotalRange := Total.GetTotalRange(Section);
      if not Assigned(TotalRange) then
        continue;

      Column := TotalRange.Column;
      for Row := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
      begin
        if WritablesInfo.IsSingleCellSelected(ExcelSheet, Row, Column) then
          continue;

        if not IsSummaryCell(Row, Column) or
        ((Total.CountMode = mcmTypeFormula) and Total.SummariesByVisible) then
        begin
          DecodedTypeFormula := TypeFormulaToString(Total, Row, Section, GrandSummaryRow);
          SetCellFormula(ExcelSheet, Row, Column, DecodedTypeFormula);
          if Total.Format = fmtBoolean then
          begin
          end;
        end;
      end;
    end;
  end;
end;

procedure TSheetMaper.UpdatePossibleNumberValue(var Value: string);
var
  s, c: integer;
begin
  // если раззделителя нет, то и делать нефиг
  s := Pos('.', Value);
  if s = 0 then
    exit;
  c := Length(Value) - s;
  // а если их больше одного, то это уж точно не число
  if Pos('.', Copy(Value, s + 1, c)) > 0 then
    exit;
  Value := StringReplace(Value, '.', DSeparator, []);
end;

procedure KillOrphanedNode(var Node: IXMLDOMNode);
begin
  if Assigned(Node) then
  begin
    Node.text := '';
    Node := nil;
  end;
end;


type
  {вспомогательный класс для создания документов разметки}
  TMarkupHelper = class
  private
    Maper: TSheetMaper;
    AxisType: TAxisType;
    Dom: IXMLDOMDocument2;

    MarkupNode: IXMLDOMNode;
    CollectInfoNode: IXMLDOMNode;
    WritebackInfoNode: IXMLDOMNode;

    ElementName: string;
    IndexName: string;
    LeafAttr: string;
    SkipTheLine: boolean;
    WasNull: boolean;
    TotalsRow, TotalsColumn: integer;

    function GetAxis: TSheetAxisCollectionInterface;
    procedure UpdateCellMarkup(Cell: TSheetSingleCellInterface;
      RowsMarkupDom, ColumnsMarkupDom: IXMLDOMDocument2);
    (*procedure GetCellCoordBroken(MemberNode: IXMLDOMNode);
    procedure GetCellCoordRegular(MemberNode: IXMLDOMNode);*)
    procedure AddCellNode(Alias: string);

  public
    procedure PrepareForTotals(Maper_: TSheetMaper; AxisType_: TAxisType);
    function PrepareForCells(Maper_: TSheetMaper): boolean;
    procedure MakeTotalsSection;
    procedure EmptyAxisMarkup;
    function MakeNodes(MemberNode: IXMLDOMNode; StartFrom: integer): boolean;
    procedure AppendNewMarkupNode;
    procedure HandleMemberNodeBroken(MemberNode: IXMLDOMNode; StartFrom: integer);
    function HandleMemberNodeRegular(MemberNode: IXMLDOMNode; StartFrom: integer): boolean;

    property Axis: TSheetAxisCollectionInterface read GetAxis;
  end;

function TMarkupHelper.GetAxis: TSheetAxisCollectionInterface;
begin
  result := Maper.GetAxis(AxisType);
end;

function TMarkupHelper.PrepareForCells(Maper_: TSheetMaper): boolean;
var
  TotalsRange: ExcelRange;
begin
  Maper := Maper_;
  Dom := Maper.CellsMarkup;
  Dom.documentElement :=  Dom.createElement('markup');
  ElementName := 'cell';
  WasNull := true; // узел writeback нам вообще не нужен в итоговом документе
  {Нет показателей - значит нет и отдельных в таблице, можно закругляться}
  TotalsRange:= GetRangeByName(Maper.ExcelSheet, BuildExcelName(sntTotals));
  result := Assigned(TotalsRange);
  if not result then
    exit;
  TotalsRow := TotalsRange.Row;
  TotalsColumn := TotalsRange.Column;
end;

procedure TMarkupHelper.PrepareForTotals(Maper_: TSheetMaper; AxisType_: TAxisType);
begin
  Maper := Maper_;
  AxisType := AxisType_;
  if AxisType = axRow then
  begin
    Dom := Maper.RowsMarkup;
    ElementName := 'row';
    LeafAttr := mnIsRowLeaf;
    IndexName := 'rind';
  end
  else
  begin
    Dom := Maper.ColumnsMarkup;
    ElementName := 'column';
    LeafAttr := mnIsColumnLeaf;
    IndexName := 'cind';
  end;
  Dom.documentElement := Dom.createElement('markup');
end;

procedure TMarkupHelper.EmptyAxisMarkup;
begin
  MarkupNode := Dom.createNode(1, ElementName, '');
  Dom.documentElement.appendChild(MarkupNode);
  SetAttr(MarkupNode, attrShift, 0);
  SetAttr(MarkupNode, LeafAttr, 'true');
  SetAttr(MarkupNode, attrWbWorthy, 'true');
end;

procedure TMarkupHelper.MakeTotalsSection;
var
  i: integer;
  Total: TSheetTotalInterface;
  Root, TotalNode: IXMLDOMNode;
begin
  if AxisType = axRow then
    exit;
  Root := Dom.createNode(1, 'totals', '');
  for i := 0 to Maper.Totals.CountWithPlacement(false) - 1 do
  begin
    Total := Maper.Totals.GetWithPlacement(false, i);
    if not (Total.TotalType in [wtFree, wtResult]) then
      continue;
    TotalNode := Dom.CreateNode(1, 'total', '');
    SetAttr(TotalNode, attrAlias, Total.Alias);
    SetAttr(TotalNode, attrShift, i);
    if Total.TotalType = wtResult then
      SetAttr(TotalNode, 'writable', 'true');
    Root.appendChild(TotalNode);
  end;
  Dom.documentElement.appendChild(Root);
end;

function TMarkupHelper.MakeNodes(MemberNode: IXMLDOMNode; StartFrom: integer): boolean;
var
  Shift, RCIndex: integer;
  WbWorthy: boolean;
begin
  result := false;
  {создаем узел, отвечающий за элемент разметки,
    Shift - это смещение внутри диапазона оси,
    RCIndex - номер строки/столбца}
  MarkupNode := Dom.createNode(1, ElementName, '');
  if Assigned(MemberNode) then
  begin
    RCIndex := GetIntAttr(MemberNode, IndexName, -BeastNumber);
    if RCIndex = -BeastNumber then
      exit;
    Shift := RCIndex - StartFrom;
    SetAttr(MarkupNode, attrShift, Shift);
    SetAttr(MarkupNode, LeafAttr, 'true');
    WbWorthy := GetBoolAttr(MemberNode, attrWbWorthy, false);
    SetAttr(MarkupNode, attrWbWorthy, WbWorthy);
    WasNull := not WbWorthy;
  end;

  {Информационные узлы для размещения и обратной записи.
    В первом записаны юникнеймы элементов, во втором - их PkId}
  CollectInfoNode := Dom.createNode(1, 'collect', '');
  WritebackInfoNode := Dom.createNode(1, 'writeback', '');
  result := true;
end;

procedure TMarkupHelper.AppendNewMarkupNode;
begin
  if SkipTheLine then
    KillOrphanedNode(CollectInfoNode)
  else
    MarkupNode.appendChild(CollectInfoNode);

  if SkipTheLine or WasNull then
    KillOrphanedNode(WritebackInfoNode)
  else
    MarkupNode.appendChild(WritebackInfoNode);

  if (CollectInfoNode = nil) and (WritebackInfoNode = nil) then
    KillOrphanedNode(MarkupNode)
  else
    Dom.documentElement.appendChild(MarkupNode);
end;

procedure TMarkupHelper.AddCellNode(Alias: string);
var
  Node: IXMLDOMNode;
begin
  Node := Dom.SelectSingleNode(Format('markup/cell[@%s="%s"]', [attrAlias, Alias]));
  if Assigned(Node) then
  begin
    Node.parentNode.removeChild(Node);
    KillOrphanedNode(Node);
  end;
  AppendNewMarkupNode;
end;

{Сбор осевых координат, ось без иерархии}
procedure TMarkupHelper.HandleMemberNodeBroken(MemberNode: IXMLDOMNode;
  StartFrom: integer);
var
  AL: IXMLDOMNodeList;
  i: integer;
  Alias, UName, PkId: string;
  AxisElement: TSheetAxisElementInterface;
  Node: IXMLDOMNode;
begin
  if not MakeNodes(MemberNode, StartFrom) then
    exit;
  AL := MemberNode.selectNodes('AliasInfo');
  for i := 0 to AL.length - 1 do
  begin
    {координаты для сбора}
    Alias := AL[i].attributes.item[0].nodeName;
    UName := AL[i].attributes.item[0].nodeValue;
    SetAttr(CollectInfoNode, Alias, UName);
    {координаты для обратной записи}
    if not WasNull then
    begin
      AxisElement := Axis.FindByAlias(Alias);
      if not Assigned(AxisElement) then
        continue;
      EncodeXPathString(UName);
      Node := AxisElement.Members.selectSingleNode(
        Format('function_result/Members//Member[@%s="%s"]', [attrUniqueName, UName]));
      if not Assigned(Node) then
        continue;
      PkId := GetStrAttr(Node, attrPkId, 'null');
      if (PkId <> 'null') then
        SetAttr(WritebackInfoNode, AxisElement.FullName, PkId)
      else
        WasNull := true;
    end;
  end;
end;

{Сбор осевых координат, нормальная ось}
function TMarkupHelper.HandleMemberNodeRegular(MemberNode: IXMLDOMNode;
  StartFrom: integer): boolean;
var
  PrevAxisIndex, AxisIndex: integer;
  AxisId, UniqueName, PkId: string;
begin
  result := false;
  if not MakeNodes(MemberNode, StartFrom) then
    exit;

  SkipTheLine := false;
  PrevAxisIndex := BeastNumber;
  repeat
    AxisIndex := GetIntAttr(MemberNode, attrAxisIndex, -1);
    if AxisIndex = -1 then
      break;
    { Дополнительное условие закомментировано, чтобы "обновленные" типовые формулы работали с общими итогами}
    if (AxisIndex < PrevAxisIndex) {!!!!!!and (MemberNode.nodeName <> ntSummary)} then
    begin
      AxisId := Axis[AxisIndex].UniqueID;
      UniqueName := GetStrAttr(MemberNode, attrUniqueName, '');
      SetAttr(CollectInfoNode, 'A_' + AxisId, UniqueName);
      if not WasNull then
      begin
        PkId := GetStrAttr(MemberNode, attrPkId, 'null');
        if (PkId <> 'null') then
          SetAttr(WritebackInfoNode, Axis[AxisIndex].FullName, PkId)
        else
          WasNull := true;
      end;
      PrevAxisIndex := AxisIndex;
    end;

    MemberNode := MemberNode.parentNode;
  until false;
  result := true;
end;

procedure TMarkupHelper.UpdateCellMarkup(Cell: TSheetSingleCellInterface;
  RowsMarkupDom, ColumnsMarkupDom: IXMLDOMDocument2);

  function GetTotalsShift(Total: TSheetTotalInterface): integer;
  var
    i: integer;
    CurrTotal: TSheetTotalInterface;
  begin
    result := 0;
    for i := 0 to Maper.Totals.Count - 1 do
    begin
      CurrTotal := Maper.Totals[i];
      if CurrTotal.IsIgnoredColumnAxis then
        continue;
      if CurrTotal = Total then
        break;
      inc(result);
    end;
  end;

const
  FakeStartFrom = 0;
var
  RowIndex, ColumnIndex: integer;
  Node: IXMLDOMNode;
  Total: TSheetTotalInterface;
begin
  {получим координаты }
  if not Cell.GetAddressPoint(ColumnIndex, RowIndex) then
    exit;

  MakeNodes(nil, FakeStartFrom);
  SetAttr(MarkupNode, attrAlias, Cell.Alias);

  RowIndex := RowIndex - TotalsRow;
  ColumnIndex := ColumnIndex - TotalsColumn;

  if Assigned(RowsMarkupDom) then
  begin
    Node := RowsMarkupDom.selectSingleNode(Format(
      'markup/row[@%s="%d"]/collect', [attrShift, RowIndex]));
    if Assigned(Node) then
      CopyAttrs(Node, CollectInfoNode);
  end;

  {Для оси столбцов сложнее. Из-за эффекта "блоков показателей" координаты
    в узлах идут не подряд (cind = 0, 1, 2, 3...), а через интервал, равный
    количеству показателей под столбцами). Поэтому чтобы получить узел
    правильно, надо сперва выяснить его начальную координату.}

  if Assigned(ColumnsMarkupDom) then
  begin
    Total := Cell.GetUnderlayingTotal;
    SetAttr(MarkupNode, 'total', Total.Alias);
    ColumnIndex := ColumnIndex - GetTotalsShift(Total);

    Node := ColumnsMarkupDom.selectSingleNode(Format(
      'markup/column[@%s="%d"]/collect', [attrShift, ColumnIndex]));
    if Assigned(Node) then
      CopyAttrs(Node, CollectInfoNode);
  end;
end;

procedure TSheetMaper.MakeMarkupDocument(AxisType: TAxisType;
  AxisDom: IXMLDOMDocument2; StartFrom: integer);
var
  Axis: TSheetAxisCollectionInterface;
  NL: IXMLDOMNodeList;
  i: integer;
  XPath: string;
  MemberNode: IXMLDOMNode;
  Helper: TMarkupHelper;
begin
  Helper := TMarkupHelper.Create;
  Helper.PrepareForTotals(Self, AxisType);
  if AxisType = axColumn then
    Helper.MakeTotalsSection;

  Axis := GetAxis(AxisType);
  if Axis.Empty then
  begin
    Helper.EmptyAxisMarkup;
    exit;
  end;

  if Axis.Broken then
    XPath := 'function_result/Members/*'
  else
    {Ось уже отбалансирована, поэтому просто берем листовые элементы.
      Важно!!! Берем ВСЕ элементы - и реальные мемберы и саммари и пустышки.
      То есть получаем "линейное отображение" оси - соответствие клеток
      области показателей элементам измерений}
    XPath := 'function_result/Members//*[not(*)]';
  NL := AxisDom.selectNodes(XPath);

  for i := 0 to NL.length - 1 do
  try
    MemberNode := NL[i];
    if Axis.Broken then
      Helper.HandleMemberNodeBroken(MemberNode, StartFrom)
    else
      Helper.HandleMemberNodeRegular(MemberNode, StartFrom);
  finally
    Helper.AppendNewMarkupNode;
  end;
  Helper.Free;
end;

procedure TSheetMaper.Clear;
begin
  inherited;
  if Assigned(FRowsDom) then
    KillDomDocument(FRowsDom);
  if Assigned(FColumnsDom) then
    KillDomDocument(FColumnsDom);
  if Assigned(FRowsMarkup) then
    KillDomDocument(FRowsMarkup);
  if Assigned(FColumnsMarkup) then
    KillDomDocument(FColumnsMarkup);
  if Assigned(FCellsMarkup) then
    KillDomDocument(FCellsMarkup);
end;

{После размещения отдельных показателей процедурой MapSingleCells надо
сохранить разметочную информацию об отдельных внутри таблицы.}
procedure TSheetMaper.MakeCellsMarkup;
var
  Helper: TMarkupHelper;
  i: integer;
  Cell: TSheetSingleCellInterface;
begin
  Helper := TMarkupHelper.Create;
  try
    if not Assigned(FCellsMarkup) then
      FCellsMarkup := InitXmlDocument;
    if not Helper.PrepareForCells(Self) then
      exit;

    {Если было полное обновление листа, то документы разметки загружены,
      иначе надо поднять их из СР}
    if not Assigned(RowsMarkup) then
      RowsMarkup := GetDataFromCP(ExcelSheet, cpRowsMarkup);
    if not Assigned(ColumnsMarkup) then
      ColumnsMarkup := GetDataFromCP(ExcelSheet, cpColumnsMarkup);

    {Разметка осмысленна только для отдельных внутри таблицы.}
    for i := 0 to SingleCells.Count - 1 do
    begin
      Cell := SingleCells[i];
      if not Cell.PlacedInTotals then
        continue;

      Helper.UpdateCellMarkup(Cell, RowsMarkup, ColumnsMarkup);
      Helper.AddCellNode(Cell.Alias);
    end;

  finally
    Helper.Free;
    if AddinLogEnable then
      WriteDocumentLog(CellsMarkup, 'cells_markup.xml');
  end;
end;

function TSheetMaper.IsSummaryCell(Row, Column: integer): boolean;
begin
  result := IsSummaryRow(Row) or IsSummaryColumn(Column);
end;

function TSheetMaper.IsSummaryColumn(Column: integer): boolean;
var
  LeafMemberColumns: string;
  Node: IXMLDOMNode;
  Shift, TotalsWP: integer;
begin
  result := false;
  if IsMarkupNew then
  begin
    Shift := Column - FFirstColumn;
    TotalsWP := Totals.CountWithPlacement(false);
    if TotalsWP = 0 then
      exit;
    Shift := Shift - (Shift mod TotalsWP);
    Node := ColumnsMarkup.selectSingleNode(Format(
      'markup/column[@%s=%d]', [attrShift, Shift]));
    if Assigned(Node) then
      result := not GetBoolAttr(Node, attrWbWorthy, false);
  end
  else
  begin
    LeafMemberColumns := GetColumnsLeafMemberName(ExcelSheet, Column);
    if LeafMemberColumns <> '' then
      result := IsWritebackSenseless(LeafMemberColumns);
  end;
end;

function TSheetMaper.IsSummaryRow(Row: integer): boolean;
var
  LeafMemberRows: string;
  Node: IXMLDOMNode;
begin
  result := false;
  if IsMarkupNew then
  begin
    {Часто вызываемый метод, время выполнения которого должно быть
    минимальным. Вариант с выборкой узла по значению атрибута, через Xpath
    слишком медленный на действительно больших (тысячи элементов) осях.
    Поэтому пользуемся тем, что в хмл "разметки" мы сохраняем ВСЮ "линию" оси,
    следовательно порядковый номер узла совпадает с его смещением.}
    Node := RowsMarkup.documentElement.childNodes[Row - FFirstRow];
    (*Node := RowsMarkup.selectSingleNode(Format(
      'markup/row[@%s=%d]', [attrShift, Row - FFirstRow]));*)
    if Assigned(Node) then
      result := not GetBoolAttr(Node, attrWbWorthy, false);
  end
  else
  begin
    LeafMemberRows := GetRowsLeafMemberName(ExcelSheet, Row);
    if LeafMemberRows <> '' then
      result := IsWritebackSenseless(LeafMemberRows);
  end;
end;

procedure TSheetMaper.ClearOneTotalsCells(Total: TSheetTotalInterface);
var
  i, j, Row, Column, SectionsCount, SCCounter: integer;
  Cell, Section: ExcelRange;
  Coords: array of TPoint;
  ClearByCell, MayClear: boolean;
begin
  SCCounter := 0;
  {Перебираем все отдельные и запоминаем координаты тех, которые
    размещены в выбранном показателе}
  for i := 0 to SingleCells.Count - 1 do
  begin
    if SingleCells[i].GetUnderlayingTotal = Total then
    begin
      if not SingleCells[i].GetAddressPoint(Column, Row) then
        continue;
      inc(SCCounter);
      SetLength(Coords, SCCounter);
      Coords[SCCounter - 1].x := Row;
      Coords[SCCounter - 1].y := Column;
    end;
  end;

  {очистку диапазона показателя ведем по секциям - если в секции нет ни одного
    отдельного, то чистим ее целиком, иначе - поячеечно, что медленнее.}
  SectionsCount := Total.SectionCount;
  for i := 0 to SectionsCount - 1 do
  begin
    ClearByCell := false;
    Section := Total.GetTotalRange(i);
    Column := Section.Column;
    for j := 0 to SCCounter - 1 do
    begin
      if Coords[j].y = Column then
      begin
        ClearByCell := true;
        break;
      end;
    end;

    if ClearByCell then
      for Row := Section.Row to Section.Row + Section.Rows.Count - 1 do
      begin
        MayClear := true;
        for j := 0 to SCCounter - 1 do
          if Coords[j].x = Row then
          begin
            MayClear := false;
            break;
          end;
        if MayClear then
        begin
          Cell := GetRange(Total.SheetInterface.ExcelSheet, Row, Column);
          try
            Cell.ClearContents;
            Cell.ClearComments;
          except
            PostMessage('Не удалось очистить ячейку по адресу ' + GetAddressLocal(Cell), msgWarning);
          end;
        end;
      end
    else
      try
        Section.ClearContents;
        Section.ClearComments;
      except
        PostMessage('Не удалось очистить секцию показателя по адресу ' + GetAddressLocal(Section), msgWarning);
      end;
  end;
end;

function TSheetMaper.MapOneTotal(Total: TSheetTotalInterface): boolean;
var
  NL: IXMLDOMNodeList;
begin
  result := false;
  CheckSheetSize(FWideTableMode);
  if FWideTableMode then
  begin
    PostMessage('Обновление одного показателя в режиме "широких таблиц" не поддерживается.', msgWarning);
    exit;
  end;

  if not CollectOneTotalData(Total) then
    exit;
  QueryOneTotalData(Total);

  OpenOperation(pfoMapTotals, CriticalNode, NoteTime, otMap);
  try
    RowsDom := GetData (cpRowsAxis);
    ColumnsDom := GetData(cpColumnsAxis);

    if not InitSizer(RowsDom, ColumnsDom) then
      exit;

    SetAxisNodesCoords(RowsDom, axRow);
    SetAxisNodesCoords(ColumnsDom, axColumn);

    ClearOneTotalsCells(Total);
    MapTypeFormula(Total);
    NL := TotalsData.selectNodes('function_result/data/row');
    MapTotalsDataNodes(RowsDom, ColumnsDom, NL, false);
    MapSummaries(RowsDom, ColumnsDom, Total);
    {Если заголовки показателей не отображаются, то комментарий к ним помещаем в
    первую ячйку данных}
    CommentOneTotalSections(Total);

    result := true;
  finally
    CloseOperation; //pfoMapTotals
  end;
end;

procedure TSheetMaper.CommentOneTotalSections(Total: TSheetTotalInterface);
var
  i: integer;
  Section: ExcelRange;
begin
  if not (IsDisplayCommentStructuralCell and not IsDisplayTotalsTitles) then
    exit;

  for i := 0 to Total.SectionCount - 1 do
  begin
    Section := Total.GetTotalRange(i);
    if not Assigned(Section) then
      continue;
    CommentCell(ExcelSheet, Section.Row, Section.Column, Total.CommentText);
  end;
end;

procedure TSheetMaper.SetAxisNodesCoords(Dom: IXMLDOMDocument2; AxisType: TAxisType);

  procedure SetCoords(Node: IXMLDOMNode; StartRow, StartColumn: integer);
  var
    i, ln, RInd, CInd: integer;
    NL: IXMLDOMNodeList;
  begin
    RInd := StartRow;
    CInd := StartColumn;
    NL := Node.childNodes;
    for i := 0 to NL.length - 1 do
    begin
      if not ((NL[i].nodeName = ntMember) or (NL[i].nodeName = ntSummary)) then
        continue;
      ln := LeafCount(NL[i], Sizer.CellSize(AxisType));
      if ln <= 0 then
        continue;
      SetAttr(NL[i], 'rind', RInd);
      SetAttr(NL[i], 'cind', CInd);

      if AxisType = axRow then
      begin
        if NL[i].hasChildNodes then
          SetCoords(NL[i], RInd, CInd + 1);
        RInd := RInd + ln;
      end
      else
      begin
        if NL[i].hasChildNodes then
          SetCoords(NL[i], RInd + 1, CInd);
        CInd := CInd + ln;
      end;
    end;
  end;

var
  StartRInd, StartCInd: integer;
  Node: IXMLDOMNode;
begin
  if not Assigned(Dom) then
    exit;
  Node := Dom.selectSingleNode('function_result/Members');
  if (AxisType = axColumn) then
  begin
    StartRInd := Sizer.StartColumns.x;
    StartCInd := Sizer.StartColumns.y;
  end
  else
  begin
    StartRInd := Sizer.StartRows.x;
    StartCInd := Sizer.StartRows.y;
  end;
  SetCoords(Node, StartRInd, StartCInd);
end;

end.








