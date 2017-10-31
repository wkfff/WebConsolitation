unit uAppendEmptyRowToSheet;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, AddinMembersTree, OfficeXP, ExcelXP, MSXML2_TLB, uXmlUtils,
  uSheetObjectModel, uSheetLogic, uFMAddinGeneralUtils, uGlobalPlaningConst,
  ImgList, ComCtrls, uCheckTV2, uFMAddinXMLUtils, uExcelUtils,
  uFMAddinExcelUtils, uFMExcelAddInConst, Buttons, ToolWin, Tabs,
  PlaningTools_TLB, ComObj, uSheetMaper, Math, uXMLCatalog;

type
  {Вариант дерева для данной формы:
    Никаких сервисных наворотов - только дерево
    Еще кое какая специфика}
  TReducedMemberTree = class(TAddinMembersTree)
  private
    procedure SetButtonsVisible(const Value: boolean);
  public
    constructor Create(AOwner: TComponent); override;
    procedure DisableNonLeaves;
    property ButtonsVisible: boolean write SetButtonsVisible;
  end;

  TfrmAppendEmptyRowToSheet = class(TForm)
    p3: TPanel;
    ImgList: TImageList;
    Panel1: TPanel;
    Panel2: TPanel;
    pTreePanel: TPanel;
    tsDimChooser: TTabSet;
    Splitter1: TSplitter;
    Panel3: TPanel;
    Button1: TButton;
    Button2: TButton;
    Panel4: TPanel;
    Bevel: TBevel;
    Label1: TLabel;
    memResume: TMemo;
    procedure btCancelClick(Sender: TObject);
    procedure FormResize(Sender: TObject);
    procedure btOKClick(Sender: TObject);
    procedure tsDimChooserChange(Sender: TObject; NewTab: Integer;
      var AllowChange: Boolean);
  private
    FTree: TReducedMemberTree;
    FPlaningSheet: TSheetLogic;
    FSheet: ExcelWorkSheet;
    FApply: boolean;

    {Инициализирует модель}
    procedure InitModel;
    {Инициализация контролов с модели}
    procedure InitControls;
    {Проверка готовности - все позиции установлены}
    function CheckReady: boolean;

  public
    {Редактирование}
    function Edit: boolean;
    {Обработка нажатия на флажок в дереве}
    procedure OnNodeCheckHandler(Sender: TObject);
    property PlaningSheet: TSheetLogic read FPlaningSheet write FPlaningSheet;
    property Tree: TReducedMemberTree read FTree write FTree;
  end;

  TBottomAxisMembers = array of string;

  {Функция добавления, которая вызывается снаружи}
  function AppendEmptyRowToSheet(AExcelSheet: ExcelWorkSheet;
    AContext: TTaskContext; APlaningSheet: TSheetLogic): boolean;


implementation
const
//  cPointUnassigned = '<Не указано>';
  cPointUnassigned = '     ???';
  attrWasChecked = 'waschecked';

{$R *.DFM}

function GetChainNL(Model: TSheetLogic): IXMLDOMNodeList; forward;
function InsertNewRows(Sheet: ExcelWorkSheet; Model: TSheetLogic;
  ChainNL: IXMLDOMNodeList): ExcelRange; forward;

function AppendEmptyRowToSheet(AExcelSheet: ExcelWorkSheet;
  AContext: TTaskContext; APlaningSheet: TSheetLogic): boolean;
var
  EditForm: TfrmAppendEmptyRowToSheet;
  NewLine: ExcelRange;
  ChainNL: IXMLDOMNodeList;
begin
  result := false;
  NewLine := nil;

  if not (Assigned(AExcelSheet) and Assigned(APlaningSheet)) then
    exit;

  //создаем редактор
  EditForm := TfrmAppendEmptyRowToSheet.Create(nil);
  //грузим модель
  if not APlaningSheet.Load(AExcelSheet, AContext, lmNoFreeData) then
    exit;

  //снимаем защиту
  if not SetSheetProtection(AExcelSheet, false) then
  begin
    ShowError(ermWorksheetProtectionFault);
    exit;
  end;

  try
    with EditForm do
    begin
      PlaningSheet := APlaningSheet;
      FSheet := AExcelSheet;
      if Edit then
      try
        ChainNL := GetChainNL(APlaningSheet);
        if Assigned(ChainNL) then
          NewLine := InsertNewRows(AExcelSheet, APlaningSheet, ChainNL);
      except
        ShowError(ermUnknown);
      end;
    end;

  finally
    FreeAndNil(EditForm);
    APlaningSheet.Clear;
    SetSheetProtection(AExcelSheet, true);
    if Assigned(NewLine) then
      NewLine.Select;
  end;
end;

constructor TReducedMemberTree.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);

  //уровни не редактируются
  MayShowSelector := false;

  {Прячем панель с кнопками}
  ButtonsVisible := false;

  {Прячем странцицу поиска, убираем язычки вкладок}
  TabStyle := tsFlatButtons;
  PageControl.Pages[0].TabVisible := false;
  PageControl.Pages[1].TabVisible := false;
  PageControl.Pages[0].Visible := true;
  PageControl.ActivePageIndex := 0;
end;

procedure TReducedMemberTree.DisableNonLeaves;
var
  i: integer;
  Node: TCheckTreeNode2;
begin
  for i := 0 to TreeView.Items.Count - 1 do
  begin
    Node := TreeView.Items[i] as TCheckTreeNode2;
    Node.InCheckedLevel := not Node.HasChildren;
  end;
end;

procedure TReducedMemberTree.SetButtonsVisible(const Value: boolean);
var
  ButtonPanel: TPanel;
begin
  ButtonPanel := TPanel(FindChildControl('SelectionPanel'));
  if Assigned(ButtonPanel) then
    ButtonPanel.Visible := Value;
end;

{выбираем отмеченные элементы, не имеющих потомков}
function GetCheckedLeafMembers(DOM: IXMLDOMDocument2): IXMLDOMNodeList;
var
  XPath: string;
begin
  result := nil;
  if not Assigned(DOM) then
    exit;

  XPath := './/*[(not(Member)) and (not(Summary)) and (@checked="true")]';
  result := DOM.selectNodes(XPath);
end;

{Получаем информацию о выбранных элементах измерений}
function GetResumeText(Model: TSheetLogic):TStringList;
var
  i, j: integer;
  CheckNodeList: IXMLDOMNodeList;
  NodeName: string;
begin
  result := TStringList.Create;
  if not Assigned(Model) then
    exit;
  for i := 0 to Model.Rows.Count - 1 do
  begin
    result.Add(Model.Rows[i].FullDimensionName + ':');
    CheckNodeList := GetCheckedLeafMembers(Model.Rows[i].Members);
    {Обновляем строчку в описании}
    if Assigned(CheckNodeList) and (CheckNodeList.length > 0) then
    begin
      for j := 0 to CheckNodeList.length - 1 do
      begin
        NodeName := GetStrAttr(CheckNodeList[j], 'name', cPointUnassigned);
        result.Add( '     ' + NodeName);
      end;
    end
    else
      result.Add(cPointUnassigned);
    result.Add('');
  end;
end;

{Делает специальную обводку (пунктир), для заданного рэнджа.}
procedure DrawDottedFrame(Range: ExcelRange);
  procedure SetBorders(BRDS: OleVariant);
  begin
    BRDS.LineStyle := xlDash;
    BRDS.Weight := xlThin;
    BRDS.ColorIndex := xlAutomatic;
  end;
begin
  {$WARNINGS OFF}
  try
    if Assigned(Range) then
    begin
      Range.Borders[xlDiagonalDown].LineStyle := xlNone;
      Range.Borders[xlDiagonalUp].LineStyle := xlNone;

      SetBorders(Range.Borders[xlEdgeLeft]);
      SetBorders(Range.Borders[xlEdgeTop]);
      SetBorders(Range.Borders[xlEdgeBottom]);
      SetBorders(Range.Borders[xlEdgeRight]);
      SetBorders(Range.Borders[xlInsideVertical]);
      SetBorders(Range.Borders[xlInsideHorizontal]);
    end;
  except
  end;
  {$WARNINGS ON}
end;

{Возвращает диапазон, соответствующий строке показателей с переданным номером}
function GetTotalsRow(Sheet: ExcelWorkSheet; RowNum: integer): ExcelRange;
var
  TotalsRange: ExcelRange;
begin
  result := nil;
  try
    TotalsRange := GetRangeByName(Sheet, BuildExcelName(sntTotals));
    if Assigned(TotalsRange) then
      result := GetRange(Sheet, RowNum, TotalsRange.Column,
        RowNum, TotalsRange.Column + TotalsRange.Columns.Count - 1);
  except
  end;
end;

{Может такое случиться (при в ставке в пустую таблицу), вставленная строка
с показателями, не объединиться с уже существующим диапазоном показателей,
проверяем такой случай, если надо объедениям}
procedure UpdateTotalsRange(InsertedTotalsRange: ExcelRange);
var
  ExcelSheet: ExcelWorksheet;
  TotalsRange: ExcelRange;
begin
  if not Assigned(InsertedTotalsRange) then
    exit;
  ExcelSheet := InsertedTotalsRange.Worksheet;

  TotalsRange := GetRangeByName(ExcelSheet, BuildExcelNAme(sntTotals));
  TotalsRange := GetUnionRange(TotalsRange, InsertedTotalsRange);
  MarkObject(ExcelSheet, TotalsRange, sntTotals, false);
end;

{Может такое случиться (при в ставке в пустую таблицу), вставленная строка
показателя, не объединиться с уже существующим диапазоном показателя, проверяем
такой случай, если надо объедениям}
procedure UpdateTotalRange(Total: TSheetTotalInterface; InsertedRange: ExcelRange;
  SectionIndex: integer);
var
  ExcelSheet: ExcelWorkSheet;
  ExcelName: string;
  TotalRange: ExcelRange;
begin
  if not(Assigned(Total) and Assigned(InsertedRange) and (SectionIndex > -1)) then
    exit;
  ExcelSheet := InsertedRange.Worksheet;
  ExcelName := Total.GetFullExcelName(SectionIndex);
  TotalRange := GetRangeByName(ExcelSheet, ExcelName);
  TotalRange := GetUnionRange(TotalRange, InsertedRange);
  MarkObject(ExcelSheet, TotalRange, ExcelName, false);
end;

function GetTotalsStartRow(ExcelSheet: ExcelWorksheet): integer;
var
  TotalsRange: ExcelRange;
begin
  result := 0;
  TotalsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntTotals));
  if not Assigned(TotalsRange) then
    exit;
  result := TotalsRange.Row;
end;

procedure SetTotalStyle(Total: TSheetTotalInterface; TotalRange: ExcelRange);
begin
  if not(Assigned(Total) and Assigned(TotalRange)) then
    exit;

  if Total.SheetInterface.PrintableStyle then
    TotalRange.Style := Total.Styles.Name[esValuePrint]
  else
  begin
    TotalRange.Style := Total.Styles.Name[esValue];
    {если добавленая ячейка - итог, пометим ее серым}
    if Total.TotalType in [wtFree, wtResult] then
      if IsSummaryCell(TotalRange.Worksheet, TotalRange.Row, TotalRange.Column) then
        TotalRange.Interior.ColorIndex := 15;
  end;
end;

procedure DrawTotals(Model: TSheetLogic; RInd: integer);
var
  ExcelSheet: ExcelWorksheet;
  InsertedTotalsRange, InsertedTotalRange: ExcelRange;
  CurTotal: TSheetTotalInterface;
  CInd, ColumnCount, SectionIndex, GrandSummaryRow: integer;
begin
  if not Assigned(Model) then
    exit;
  ExcelSheet := Model.ExcelSheet;

  {получим вставленный диапазон показателей}
  InsertedTotalsRange := GetTotalsRow(ExcelSheet, RInd);
  if not Assigned(InsertedTotalsRange) then
    exit;

  GrandSummaryRow := GetGrandSummaryRow(ExcelSheet);

  ColumnCount := InsertedTotalsRange.Column + InsertedTotalsRange.Columns.Count - 1;
  for CInd := InsertedTotalsRange.Column to ColumnCount do
  begin
    CurTotal := Model.Totals.FindByColumn(CInd, SectionIndex);
    {диапазон добавленной ячейки показателя}
    InsertedTotalRange := GetRange(ExcelSheet, RInd, CInd, Rind, CInd);
    if not Assigned(CurTotal) then
      continue;

    {устанавливаме символ пустого значения показателя}
    InsertedTotalRange.Value2 := CurTotal.EmptyValueSymbol;
    {устанавливем числовой формат показателя}
    InsertedTotalRange.NumberFormat := CurTotal.NumberFormat;

    if CurTotal.TotalType in [wtFree, wtResult] then
    begin
      {если включена типовая формула, добавленая ячейка находится не в итогах или
      включен режим подсчета итогов - "типовая формула", то размещаем ее}
      if CurTotal.TypeFormula.Enabled then
        if not IsSummaryCell(ExcelSheet, RInd, CInd) or
        ((CurTotal.CountMode = mcmTypeFormula) and CurTotal.SummariesByVisible) then
          SetCellFormula(ExcelSheet, Rind, CInd,
          Model.TypeFormulaToString(CurTotal, RInd, SectionIndex, GrandSummaryRow));
    end;
    {назначает стиль, добавленной ячейке показателя}
    SetTotalStyle(CurTotal, InsertedTotalRange);
    UpdateTotalRange(CurTotal, InsertedTotalRange, SectionIndex);
  end;
  UpdateTotalsRange(InsertedTotalsRange);
end;

function IsEmptyRows(ExcelSheet: ExcelWorksheet): boolean;
var
  RowsRange, SummaryRange: ExcelRange;
begin
  result := false;
  if not Assigned(ExcelSheet) then
    exit;
  RowsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
  SummaryRange := GetRangeByName(ExcelSheet, BuildExcelName(snNamePrefix +
    snSeparator + gsRow));

  if not Assigned(SummaryRange) and Assigned(RowsRange) and (RowsRange.Rows.Count = 1) then
    result := true;
end;

function GetColumn(Level: TSheetLevelInterface): integer;
var
  Axis: TSheetAxisCollectionInterface;
  i, j: integer;
  ExcelSheet: ExcelWorksheet;
  RowsRange: ExcelRange;
begin
  result := 1;
  if not Assigned(Level) then
    exit;
  Axis := Level.SheetInterface.Rows;
  ExcelSheet := Level.SheetInterface.ExcelSheet;
  RowsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
  if Assigned(RowsRange) then
    result := RowsRange.Column - 1;
  {если у всей оси нет иерархии, то и вычислять нечего (все размещается в первой
  колонке строк)}
  if Axis.Broken then
  begin
    inc(result);
    exit;
  end;
  for i := 0 to Axis.Count - 1 do
    for j := 0 to Axis[i].Levels.Count - 1 do
    begin
      if not(Axis[i].IgnoreHierarchy) or (j = 0)then
        inc(result);
      if (Level = Axis[i].Levels[j]) then
        exit;
    end;
end;

{Присоединяем вновь добавленные строки к уже существуещему имени}
procedure ConnectToRows(ExcelSheet: ExcelWorksheet; RInd: integer);
var
  InsertedRowsRange, RowsRange: ExcelRange;
begin
  if not Assigned(ExcelSheet) then
    exit;
  RowsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
  if not Assigned(RowsRange) then
    exit;
  InsertedRowsRange := GetRange(ExcelSheet, RInd, RowsRange.Column, RInd,
    RowsRange.Column + RowsRange.Columns.Count - 1);
  RowsRange := GetUnionRange(RowsRange, InsertedRowsRange);
  MarkObject(ExcelSheet, RowsRange, sntRows, false);
end;

{Покрасим вновь вставленный диапазон строк}
procedure PaintRows(ExcelSheet: ExcelWorksheet; IsPrintableStyle: boolean;
  Rind: integer);
var
  InsertedRowsRange, RowsRange: ExcelRange;
begin
  if not Assigned(ExcelSheet) then
    exit;
  RowsRange := GetRangeByName(ExcelSheet, BuildExcelName(sntRows));
  if not Assigned(RowsRange) then
    exit;
  InsertedRowsRange := GetRange(ExcelSheet, RInd, RowsRange.Column, RInd,
    RowsRange.Column + RowsRange.Columns.Count - 1);
  SetRangeStyle(InsertedRowsRange, IIF(IsPrintableStyle, snFieldPositionPrint,
    snFieldPosition));
end;

{Накладывает на ячейку стиль}
procedure SetCellStyle(Model: TSheetLogic; Node: IXMLDOMNode; Range: ExcelRange);
var
  ElemIndex, LevelIndex: integer;
  Level: TSheetLevelInterface;
begin
  if not (Assigned(Node) and Assigned(Range)) then
    exit;
  ElemIndex := GetIntAttr(Node, attrAxisIndex, - 1);
  LevelIndex := GetIntAttr(Node, attrLevelIndex, - 1);
  if (ElemIndex > -1) and (LevelIndex > -1) then
  begin
    Level := Model.Rows[ElemIndex].Levels[LevelIndex];
    if Assigned(Level) then
      SetRangeStyle(Range, IIF(Model.PrintableStyle, Level.Styles.Name[esValuePrint],
        Level.Styles.Name[esValue]));
  end;
end;

{Рисует ячейку по узлу и координатам}
procedure DrawCell(Model: TSheetLogic; Node: IXMLDOMNode; RInd, CInd: integer);
var
  CR: ExcelRange;
begin
  try
    CR := GetRange(Model.ExcelSheet, RInd, CInd, RInd, CInd);
    if Assigned(CR) then
    begin
      SetCellStyle(Model, Node, CR);
      MarkObject(Model.ExcelSheet, CR, GetStrAttr(Node, attrRangeName, 'error'), false);
      DrawDottedFrame(CR);
    end;

    Model.ExcelSheet.Cells.Item[RInd, CInd].Value := GetStrAttr(Node, attrName, '');
  except
  end;
end;

{Вставляет пустую строку в таблицу}
procedure InsertNewLine(Sheet: ExcelWorksheet; RowNumber: integer);
begin
  try
    Sheet.Cells.Item[RowNumber, Sheet.Columns.Count].EntireRow.Insert(xlShiftDown, 0);
    Sheet.Cells.Item[RowNumber, Sheet.Columns.Count].EntireRow.Style := 'Normal';
  except
  end;
end;

{Возвращает номер столбца, откда начинать размещение}
function InitCInd(Sheet: ExcelWorksheet): integer;
var
  ExRange: ExcelRange;
begin
  result := 1;
  {Берем первый столбцец всей таблицы}
  ExRange := GetRangeByName(Sheet, BuildExcelName(sntRows));
  if Assigned(ExRange) then
    result := ExRange.Column;
end;

{Возвращает первоначальный номер новой строки, этот номер будет использован,
если строка "абсолютно новая", т.е в листе нет еще ни одного компонента этой
строки.}
function InitRInd(Sheet: ExcelWorksheet): integer;
var
  ExRange: ExcelRange;
begin
  result := 1; //на всякий пожарный

  {Если в таблице есть гранд-тотал, тогда вставлять будем до него}
  ExRange := GetRangeByName(Sheet, BuildExcelName(snNamePrefix + snSeparator +
    gsRow));
  if Assigned(ExRange) then
  begin
    result := ExRange.Row - 1;
    exit;
  end;

  ExRange := GetRangeByName(Sheet, BuildExcelName(sntRows));
  if Assigned(ExRange) then
    result := ExRange.Row + ExRange.Rows.Count - 1;
end;

{Чтобы найти родителя, откусываем у имени последний LocalID}
function GetParentRangeName(ChildRangeName: string): string;
var
  RangeName: TStringList;
  i: integer;
begin
  result := '';
  RangeName := nil;
  if not ParseExcelName(ChildRangeName, RangeName) then
    exit;
  try
    for i := RangeName.Count - 2 downto 0 do
      if not((RangeName[i] = fpDummy) and (result = '')) then
        result :=  RangeName[i] + snSeparator + result;
    if (result <> '') and (result[Length(result)] = snSeparator) then
      result := Copy(result, 0, Length(result) - 1);
  finally
    FreeStringList(RangeName);
  end;
end;

{По цепочке LocalID содержащейся в имене дочернего узла, находим родителя}
function GetParentNodeByLocalID(SheetInterface: TSheetInterface;
  Node: IXMLDOMNode): IXMLDOMNode;
var
  NodeRangeName, ParentRangeName: string;
  ParentRange: ExcelRange;
  AxisIndex: integer;
  RangeName: TStringList;
begin
  result := nil;
  if not(Assigned(SheetInterface) and Assigned(Node)) then
    exit;
  RangeName := nil;
  ParentRange := nil;
  NodeRangeName := GetStrAttr(Node, attrRangeName, '');
  AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
  repeat
    ParentRangeName := GetParentRangeName(BuildExcelName(NodeRangeName));
    {В имене родителя меняем признак, того что с этого элемента возможна запись}
    ParentRangeName := StringReplace(ParentRangeName, '_true_', '_false_', [rfReplaceAll]);
    ParentRange := GetRangeByName(SheetInterface.ExcelSheet,
      BuildExcelName(ParentRangeName));
    {Проверяем случай когда родитель из другого измерения, подменяем в имене
    соответсвующий параметр}
    if not Assigned(ParentRange) then
      if (AxisIndex > 0) and ParseExcelName(BuildExcelName(ParentRangeName), RangeName) then
      begin
        RangeName[1] := SheetInterface.Rows[AxisIndex - 1].UniqueID;
        NodeRangeName := ConstructNameByParams(RangeName);
        ParentRange := GetRangeByName(SheetInterface.ExcelSheet, NodeRangeName);
        if Assigned(ParentRange) then
          ParentRangeName := NodeRangeName;
      end;
    {Родного родителя может и не быть, ищем приемного}
    NodeRangeName := ParentRangeName;
  until Assigned(ParentRange) or (ParentRangeName = '');
  if Assigned(ParentRange) then
    result := Node.ownerDocument.selectSingleNode('//Member[@rangename="' +
      BuildExcelName(ParentRangeName) + '"]');
  FreeStringList(RangeName);
end;

{Ищем родителя по ParentUN, такой поиск возможен если не игнорируется иерархия
у всей оси целиком}
function GetParentNodeByParentUN(ExcelSheet: ExcelWorkSheet;
  Node: IXMLDOMNode): IXMLDOMNode;
var
  ParentUN: string;
begin
  result := nil;
  if not(Assigned(ExcelSheet) and Assigned(Node) and Assigned(Node.parentNode))then
    exit;
  ParentUN := GetStrAttr(Node, attrParentUN, '');
  if ParentUN = '' then
    exit;
  result := Node.parentNode.selectSingleNode('//Member[@unique_name="' + ParentUN + '"]');
end;

{Ищем реального родителя даного узла}
function GetRealParentNode(SheetInterface: TSheetInterface;
  ChildNode: IXMLDOMNode): IXMLDOMNode;
begin
  result := nil;
  if not(Assigned(SheetInterface) and Assigned(ChildNode)) then
    exit;

  if not SheetInterface.Rows.Broken then
    result := GetParentNodeByParentUN(SheetInterface.ExcelSheet, ChildNode);
  if not Assigned(result) then
    result := GetParentNodeByLocalID(SheetInterface, ChildNode);
end;

{Рисует MP в строке для всей цепочки}
procedure DrawMP(Model: TSheetLogic; RInd: integer; Node: IXMLDOMNode);
var
  CInd, AxisIndex: integer;
  tNode: IXMLDomNode;
  i, j: integer;
  sName, sValue: string;
  AxEl: TSheetAxisElementInterface;
  Mask: string;
  Sheet: ExcelWorksheet;
begin
  Sheet := Model.ExcelSheet;
  //Колонка куда пишем текущий MP
  CInd := IIF(
                Model.Rows.MPBefore,
                InitCInd(Sheet) - Model.Rows.MPCheckedCount,
                InitCInd(Sheet) + Model.Rows.FieldCount
              );

  if (RInd < 0) or (CInd < 0) then
    exit;

  for i := 0 to Model.Rows.Count - 1 do
  begin
    AxEl := Model.Rows[i];
    tNode := Node;
    AxisIndex := GetIntAttr(tNode, attrAxisIndex, -1);
    {если вставляемый узел из другой оси, ищем нужный в родителях}
    while Assigned(tNode) and (AxisIndex <> i) do
    begin
      tNode := GetRealParentNode(Model, tNode);
      AxisIndex := GetIntAttr(tNode, attrAxisIndex, -1);
    end;

    for j := 0 to AxEl.MemberProperties.Count - 1 do
      if AxEl.MemberProperties[j].Checked then
      begin
        try
          Sheet.Cells.Item[RInd, CInd].Style :=
            IIF(Model.PrintableStyle, snMemberPropertiesPrint, snMemberProperties);
        except
        end;
        sName := AxEl.MemberProperties[j].Name;
        EncodeMemberPropertyName(sName);
        sValue := GetStrAttr(tNode, sName, fmEmptyCell);
        if sValue <> 'null' then
        begin
          Mask := AxEl.MemberProperties[j].Mask;
          if (Mask <> '') and (Length(sValue) <= 15) then
            Sheet.Cells.Item[RInd, CInd].NumberFormat := Mask;
          if sValue <> '' then
            Sheet.Cells.Item[RInd, CInd].Value := sValue;
        end;
        inc(CInd);
      end;
  end;
end;

{Вставленный диапазон может входить в состав нескольких имен, проверяем это,
если надо присоединяем его к этому сотаву}
procedure ConnectToOtherNames(ExcelSheet: ExcelWorksheet;
  InsertedRange: ExcelRange);
var
  RangeRow, RangeColumn, i: integer;
  StartCell: ExcelRange; // начальная ячейка стартового диапазона
  Name: ExcelXP.Name;
  NameRange: ExcelRange;
begin
  if not(Assigned(ExcelSheet) and Assigned(InsertedRange)) then
    exit;
  RangeRow := InsertedRange.Row;
  RangeColumn := InsertedRange.Column;
  {получаем начальную ячейку вставленного диапазона}
  StartCell := GetRange(ExcelSheet, RangeRow, RangeColumn, RangeRow, RangeColumn);
  for i := 1 to ExcelSheet.Names.Count do
  begin
    Name := ExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
    if not(IsNameOurs(Name.Name_) and Assigned(Name.RefersToRange)) then
      continue;
    NameRange := Name.RefersToRange;
    if IsNestedRanges(StartCell, NameRange) then
    begin
      {Если начальна ячейка, принадлежит имени, присоединим весь вставленный
      диапазон к этому имени}
      NameRange := GetUnionRange(Name.RefersToRange, InsertedRange);
      MarkObject(ExcelSheet, NameRange, Name.Name_, false);
    end;
  end;
end;

{Начиная с указанных координат, и до начала таблицы, у строк будет объеденять
ячейки с выше лежащими}
procedure MergeCells(ExcelSheet: ExcelWorksheet; StartRow, StartColumn: integer);
type
  TNames = array of string;

  {На один и тот же диапазон, может ссылаться несколько имен, получаем их всех}
  function ExtractNames(ExcelSheet: ExcelWorksheet; Range: ExcelRange): TNames;
  var
    CountName: integer;
    Name: string;
  begin
    CountName := 0;
    SetLength(result, CountName);
    if not(Assigned(ExcelSheet) and Assigned(Range)) then
      exit;

    Name := GetNameByRange(ExcelSheet, Range);
    while Name <> '' do
    begin
       Inc(CountName);
       SetLength(result, CountName);
       result[CountName - 1] := Name;
       GetNameObject(ExcelSheet, Name).Delete;
       Name := GetNameByRange(ExcelSheet, Range);
    end;
  end;

var
  AxisStartColumn: integer; //Стартовая колонка у оси строк
  i, j: integer;
  Range, InsertedCell: ExcelRange;
  RangeName: string;
  Names: TNames;
begin
  AxisStartColumn := InitCInd(ExcelSheet);
  for i := AxisStartColumn to StartColumn do
  begin
    InsertedCell := GetRange(ExcelSheet, StartRow, i, StartRow, i);
    if not TVarData(InsertedCell.MergeCells).VBoolean then
    begin
      SetLength(Names, 0);
      RangeName := GetNameByRC(ExcelSheet, StartRow - 1, i);
      if RangeName <> '' then
      begin
        {Получаем диапазон с которым объедением нашу свеже вставленую ячейку}
        Range := GetRangeByName(ExcelSheet, RangeName);
        {Пулучаем все имена данного диапазона}
        Names := ExtractNames(ExcelSheet, Range);
      end
      else
        {Если пустышка}
        Range := GetRange(ExcelSheet, StartRow - 1, i, StartRow - 1, i);

      InsertedCell := GetUnionRange(InsertedCell, Range);
      InsertedCell.Merge(false);
      for j := 0 to Length(Names) - 1 do
        MarkObject(ExcelSheet, InsertedCell, Names[j], false);
      DrawDottedFrame(InsertedCell);
      {Полезная, но трудоемкая операция, пока выключми ее}
      (*
      {Вставленный диапазон может входить в состав нескольких имен, проверяем это,
      если надо присоединяем его к этому сотаву}
      ConnectToOtherNames(ExcelSheet, InsertedRange);
      *)
    end;
  end;
end;

function GetRange(ExcelSheet: ExcelWorksheet; Node: IXMLDOMNode): ExcelRange;
var
  RangeName: string;
begin
  result := nil;
  if not(Assigned(ExcelSheet) and Assigned(Node)) then
    exit;
  RangeName := GetStrAttr(Node, attrRangeName, '');
  result := GetRangeByName(ExcelSheet, BuildExcelName(RangeName));
end;

{Требуется ли вставка новой строки}
function IsNeedInsert(ExcelSheet: ExcelWorksheet; ChainNL: IXMLDomNodeList;
  CurItem: integer): boolean;
var
  CurDepth, PriorDepth: integer;
  ParentRange: ExcelRange;
begin
  result := false;
  if not Assigned(ChainNL) then
    exit;
  if (CurItem = 0) then
  begin
    result := true;
    exit;
  end;
  {Смотрим вложенность текущешо и предыдущего узла}
  CurDepth := GetNodeDepth(ChainNL[CurItem]);
  PriorDepth := GetNodeDepth(ChainNL[CurItem - 1]);
  ParentRange := GetRange(ExcelSheet, ChainNL[CurItem - 1]);
  {Если она меньше или равна предыдущему, значит требуется вставка новой строки,
  вставка требуется так же если высота родительского диапазона больше 1, то
  есть у него уже есть размещеные дети}
  result := (CurDepth <= PriorDepth) or (GetRangeHeight(ParentRange) > 1);
end;

function GetSummary(SheetInterface: TSheetInterface; Node: IXMLDOMNode): ExcelRange;
var
  RangeName, SummaryName: string;
  Axisindex, LevelIndex, i, j, DummyCount: integer;
  ExcelSheet: ExcelWorksheet;
  Axis: TSheetAxisCollectionInterface;
begin
  result := nil;
  if not(Assigned(SheetInterface) and Assigned(Node)) then
    exit;

  ExcelSheet := SheetInterface.ExcelSheet;
  RangeName := GetStrAttr(Node, attrRangeName, '');
  if RangeName = '' then
    exit;

  AxisIndex := GetIntAttr(Node, attrAxisIndex, 0);
  LevelIndex := GetIntAttr(Node, attrlevelIndex, 0);
  Axis := SheetInterface.Rows;

  DummyCount := 0;
  if not SheetInterface.Rows.Broken then
    {Считаем количество пустых ячеек содержащихся в имене итога}
    for i := AxisIndex to Axis.Count - 1 do
      for j := 0 to Axis[i].Levels.Count - 1 do
        if (i = AxisIndex) and (j <= LevelIndex) then
          continue
        else
          if (not(Axis[i].IgnoreHierarchy) or (j = 0)) then
            inc(DummyCount);

  for i := 1 to DummyCount - 1 do
    RangeName := RangeName + snSeparator + fpDummy;

  {Составляем имя итога}
  SummaryName := RangeName + snSeparator + fpEnd;
  result := GetRangeByName(ExcelSheet, BuildExcelName(SummaryName));
end;

function GetParentSummary(SheetInterafce: TSheetInterface; Node: IXMLDOMNode): ExcelRange;
begin
  result := nil;
  if not(Assigned(SheetInterafce) and Assigned(Node)) then
    exit;
  result := GetSummary(SheetInterafce, Node.parentNode);
end;

function IsParentHasSummary(SheetInterafce: TSheetInterface; Node: IXMLDOMNode): boolean;
begin
  result := Assigned(GetParentSummary(SheetInterafce, Node));
end;

function GetRow(SheetInterface: TSheetInterface; Var BottomAxisMembers, NoParentMembers,
  NoParentBottomAxisMembers: TBottomAxisMembers; Node: IXMLDOMNode;
  IsInsert: boolean): integer;
var
  RealParentNode: IXMLDOMNode;
  ParentName, CellName: string;
  ParentSummary, ParentRange, BottomAxisMember: ExcelRange;
  ExcelSheet: ExcelWorksheet;
  AxisIndex, i: integer;
  IsLocationToEndTable: boolean;
begin
  result := 1;
  if not(Assigned(SheetInterface) and Assigned(Node)) then
    exit;
  ExcelSheet := SheetInterface.ExcelSheet;

  CellName := GetStrAttr(Node, attrRangeName, '');
  AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
  RealParentNode := GetRealParentNode(SheetInterface, Node);
  ParentName := GetStrAttr(RealParentNode, attrRangeName, '');
  ParentRange := GetRangeByName(ExcelSheet, ParentName);

  {Концепция размещения новых мемберов, такова:
    На вход процедуре подается три списка.
      BottomAxisMembers - его размер равен количеству осей, для каждой оси хранится
    имя последне размещеного элемента.
      NoParentMembers - в списке хрянятся имена мемберов у которых нет родителя,
    а так же имена их детей.
      NoParentBottomAxisMembers - аналог BottomAxisMembers, только хранятся
    имена элементов, родители которых содержаться в списке NoParentMembers

      1) Если у мембера нет родителя, он размещается в конце таблицы, его имя
    запоминается в списке мемберов не имеющих родителя, то есть размещенных в
    конце таблицы
      2) Если у мембера есть родитель, и иерархия не  разрушена, здесь все просто,
    помещаем его внизу родителя
      3) Если иерархия разрушена, то мембер принадлежащий родителю размещеному в
    конце таблицы, должен размещаться не выше уже добавленных в таблицу нами
    мемберов из списка NoParentBottomAxisMembers.
      4) Если иерархия разрушена, то мембер не принадлежаций родителю из списка
    NoParentMembers, должен размещаться не выше уже добавленных в таблицу нами
    мемберов из списка BottomAxisMembers.}

  if Assigned(ParentRange) then
  begin
    result := ParentRange.Row + ParentRange.Rows.Count - 1;
    {Элемент размещающийся в оси игнорирующей иерархию, должен быть
    расположен ниже уже размещеных нами мемберов}
    if SheetInterface.Rows.Broken or SheetInterface.Rows[AxisIndex].IgnoreHierarchy then
    begin
      IsLocationToEndTable := false;
      for i := 0 to Length(NoParentMembers) - 1 do
        if NoParentMembers[i] = ParentName then
        begin
          IsLocationToEndTable := true;
          SetLength(NoParentMembers, Length(NoParentMembers) + 1);
          NoParentMembers[Length(NoParentMembers) - 1] := CellName;
          break;
        end;
      if SheetInterface.Rows.Broken then
        for i := SheetInterface.Rows.Count - 1 downto 0 do
        begin
          BottomAxisMember := GetRangeByName(ExcelSheet,
            IIF(IsLocationToEndTable, NoParentBottomAxisMembers[i], BottomAxisMembers[i]));
          if Assigned(BottomAxisMember) then
            break;
        end
      else
        BottomAxisMember := GetRangeByName(ExcelSheet,
          IIF(IsLocationToEndTable, NoParentBottomAxisMembers[AxisIndex], BottomAxisMembers[AxisIndex]));
      if Assigned(BottomAxisMember) then
        result := Max(result, (BottomAxisMember.Row + BottomAxisMember.Rows.Count - 1));
      if IsLocationToEndTable then
        NoParentBottomAxisMembers[AxisIndex] := CellName
      else
        BottomAxisMembers[AxisIndex] := CellName;
    end;
  end
  else
  begin
    //размещаем элемент в конец таблицы, и запоминаем имя мембера
    SetLength(NoParentMembers, Length(NoParentMembers) + 1);
    NoParentMembers[Length(NoParentMembers) - 1] := CellName;
    NoParentBottomAxisMembers[AxisIndex] := CellName;
    result := InitRInd(ExcelSheet);
  end;

  {если требуется вставка, немножко подкоректируем координату строки}
  if IsInsert then
  begin
    ParentSummary := GetParentSummary(SheetInterface, Node);
    {Если высота родителя вставляемого элемента больше 1, и у него есть итог,
    и он расположен на месте вставляемой строки, то оставляем координату
    вставки строки без изменений, для того чтобы строка вставилась перед
    итогом, иначе увеличиваем координату, чтобы строка добавилась внизу
    родительского элемента}
    if not(Assigned(ParentSummary) and (ParentSummary.Row = result) and
    (GetRangeHeight(ParentRange) > 1)) then
      inc(result);
  end;
end;

{Вставляет новые строки по подготовленной модели}
function InsertNewRows(Sheet: ExcelWorkSheet; Model: TSheetLogic;
  ChainNL: IXMLDOMNodeList): ExcelRange;
var
  i: integer;
  {Флаг, говорящий о том, что нужна вставка новой строки}
  IsInsert: boolean;
  CellName: string;
  CurrRange: ExcelRange;
  RInd, CInd, AxisIndex, LevelIndex: Integer;
  Op: IOperation;
  BottomAxisMembers: TBottomAxisMembers; //имена нижних мемберов осей
  {имена элементов, а так же их потомков у которых при размещении не оказалось
  родителей, то есть эти элементы были размещены в конец таблицы}
  NoParentMembers: TBottomAxisMembers;
  {имена нижних мемберов, родители которых размещены в конце таблицы}
  NoParentBottomAxisMembers: TBottomAxisMembers;
begin
  Application.ProcessMessages;
  result := nil;

  if not Assigned(ChainNL) or (ChainNL.length = 0) then
    exit;
  try
    Sheet.Application.ScreenUpdating[GetUserDefaultLCID] := false;
    Op := CreateComObject(CLASS_Operation) as IOperation;
    Op.StartOperation(Sheet.Application.Hwnd);
    Op.Caption := 'Добавление строк';

    SetLength(BottomAxisMembers, Model.Rows.Count);
    SetLength(NoParentBottomAxisMembers, Model.Rows.Count);
    IsInsert := not IsEmptyRows(Sheet);
    {Идем по цепочке и ищем ячейки по именам, как только не нашли, всталяем
     свою строку и рисуем недостающие ячейки}
    for i := 0 to ChainNL.length - 1 do
    begin
      CellName := GetStrAttr(ChainNL[i], attrRangeName, '');
      {пустышки пропускаем}
      if CellName = '' then
        continue;
      CurrRange := GetRangeByName(Sheet, CellName);

      if not Assigned(CurrRange) then
      begin
        {если строку уже вставили, смотрим требуется ли вставка срток еще, на первой
        итерации пропускаем, признак и так инициализирован}
        if (not IsInsert) and (i > 0) then
          IsInsert := IsNeedInsert(Sheet, ChainNL, i);

        AxisIndex := GetIntAttr(ChainNL[i], attrAxisIndex, -1);
        LevelIndex := GetIntAttr(ChainNL[i], attrLevelIndex, -1);
        {Вычисляем колонку для размещения}
        CInd := GetColumn(Model.Rows[AxisIndex].Levels[LevelIndex]);
        {Вычисляем строку для размещения}
        RInd := GetRow(Model, BottomAxisMembers, NoParentMembers,
          NoParentBottomAxisMembers, ChainNL[i], IsInsert);
        if IsInsert then
        begin
          InsertNewLine(Sheet, RInd);
          //во вновь вставленном диапазоне, покрасим строки
          PaintRows(Sheet, Model.PrintableStyle, Rind);
          MergeCells(Sheet, RInd, CInd - 1);
          ConnectToRows(Sheet, RInd);
          IsInsert := false;
        end;
        DrawMP(Model, RInd, ChainNL[i]); //рисуем MP
        DrawCell(Model, ChainNL[i], RInd, CInd);
        DrawTotals(Model, RInd); //рисуем показатели
      end;
    end;
  finally
    Sheet.Application.ScreenUpdating[GetUserDefaultLCID] := true;
    SetLength(BottomAxisMembers, 0);
    SetLength(NoParentBottomAxisMembers, 0);
    SetLength(NoParentMembers, 0);
    Application.ProcessMessages;
    Op.StopOperation;
    Op := nil;
  end;
end;

{Получаем цепочку мемберов, определяющую вставляемые строки}
function GetChainNL(Model: TSheetLogic): IXMLDOMNodeList;
var
  FullDOM: IXMLDOMDocument2;
  i: integer;
begin
  result := nil;
  {Получаем специальную ось по подготовленным ранее узлам.
   По сути, эта ось будет представлять из себя одну цепочку мемберов,
   определяющую вставляемую строку}
  FullDOM := Model.Rows.GetFullAxisDOM;

  if not Assigned(FullDOM) then
  begin
    ShowError(ermUnknown);
    exit;
  end;

  {Удаляем все саммари}
  result := FullDOM.selectNodes('//' + ntSummary);
  for i := 0 to result.length - 1 do
    result[i].parentNode.removeChild(result[i]);
  result := FullDOM.selectNodes('//Member');
  //FullDOM.Save('C:\FullDOM.xml');
end;

function TfrmAppendEmptyRowToSheet.Edit: boolean;
begin
  result := true;
  FApply := false;
  if not Assigned(FPlaningSheet) then
    exit;

  {Если компонет - дерево еще не создали, тогда создаем}
  if not Assigned(Tree) then
  begin
    Tree := TReducedMemberTree.CreateParented(pTreePanel.handle);
    Tree.Parent := pTreePanel;
    Tree.Align := alClient;
    //вешаем обработчик
    Tree.OnNodeCheck := OnNodeCheckHandler;
  end;

  Tree.Images := ImgList;
  InitModel;
  InitControls;
  ShowModal;
  result := FApply;
end;

procedure TfrmAppendEmptyRowToSheet.InitModel;
var
  i, j: integer;
  MemNL: IXMLDOMNodeList;
  MemNode: IXMLDOMNode;
  Selection: ExcelRange;
  SelName, LocalID, XPath: string;
  SelParams: TStringList;
  ParamIndex: integer;
  WasChecked: boolean;
begin
  for i := 0 to PlaningSheet.Rows.Count - 1 do
  begin
    {Пометим элементы измерений у которых признак checked - false, что бы в
    дальнейшем при составлении цепочки размещаемых мемберов, не включать эти
    элементы}
    if PlaningSheet.Rows.Broken or PlaningSheet.Rows[i].IgnoreHierarchy then
      MemNL := PlaningSheet.Rows[i].Members.selectNodes('//Member[(@checked="false") or not(@checked)]')
    else
      MemNL := PlaningSheet.Rows[i].Members.selectNodes(
      'function_result/Members//Member[((@checked="false") or not(@checked)) and not (.//Member[@checked="true"])]');
    FillNodeListAttribute(MemNL, attrWasChecked, 'false');

    {У всех элементов строк сбрасываем checked}
    MemNL := PlaningSheet.Rows[i].Members.selectNodes('//Member');
    FillNodeListAttribute(MemNL, attrChecked, 'false');
  end;

  {На основе анализа текущцй ячейки в сетке, выполняем инициализацию
   значений в измерениях (какие есть)}
  Selection := (FSheet.Application.Selection[GetUserDefaultLCID] as ExcelRange);
  if Assigned(Selection) then
  try
    SelName := GetNameByRange(FSheet, Selection);

    if ParseExcelName(SelName, SelParams) then
    begin
      ParamIndex := 3; //локал-индексы начинаются с третьего
      {Пытаемся инициализировать все кроме последнего}
      for i := 0 to PlaningSheet.Rows.Count - 2 do
        for j := 0 to PlaningSheet.Rows[i].Levels.Count - 1 do
        begin
          if ParamIndex < SelParams.Count then
          begin
            LocalID := SelParams[ParamIndex];
            XPath := Format('//Member[@local_id="%s"]', [LocalID]);
            MemNode := PlaningSheet.Rows[i].Members.selectSingleNode(XPath);
            {Если нашли такой мембер, и он листовой, проставляем}
            if Assigned(MemNode) and (not MemNode.hasChildNodes) then
            begin
              (MemNode as IXMLDOMElement).setAttribute(attrChecked, 'true');
              while Assigned(MemNode.parentNode) do
              begin
                MemNode := MemNode.parentNode;
                {Если мембер небыл отмечен в исходном дереве, то и в цепочку
                размещаемых элементов включать его не будем}
                WasChecked := GetBoolAttr(MemNode, attrWasChecked, true);
                if (MemNode.nodeName = 'Member') and WasChecked then
                  (MemNode as IXMLDOMElement).setAttribute(attrChecked, 'true');
              end;
            end;
          end
          else
            break; //параметры кончились - выходим
          {Если иерархия не сломана или сломана целиком, тогда сдвигаемся на следующий уровень}
          if (not PlaningSheet.Rows[i].IgnoreHierarchy) or PlaningSheet.Rows.Broken then
            Inc(ParamIndex);
        end;
    end;
  finally
    FreeAndNil(SelParams);
  end;
end;

procedure TfrmAppendEmptyRowToSheet.InitControls;
var
  i, TabIndex: integer;
  DimList, ResumeList: TStringList;
  NodeList: IXMLDOMNodeList;
begin
  memResume.Clear;
  tsDimChooser.Tabs.Clear;

  DimList := TStringList.Create;
  try
    ResumeList := GetResumeText(FPlaningSheet);
    memResume.Lines := ResumeList;
    TabIndex := -1; //инициализируем индекс вкладки

    for i := 0 to FPlaningSheet.Rows.Count - 1 do
    begin
      DimList.Add(FPlaningSheet.Rows[i].FullDimensionName);
      NodeList := GetCheckedLeafMembers(FPlaningSheet.Rows[i].Members);
      {Если у измерения нет отмеченых элементов, значит при показе форме
      фокусировать будем на него}
      if (not Assigned(NodeList) or (NodeList.Length = 0)) and (TabIndex = -1) then
        TabIndex := i;
    end;
    tsDimChooser.Tabs := DimList;
    if (TabIndex > -1) and (TabIndex < tsDimChooser.Tabs.Count) then
      tsDimChooser.TabIndex := TabIndex;
  finally
    FreeAndNil(ResumeList);
    FreeAndNil(DimList);
  end;
end;

function TfrmAppendEmptyRowToSheet.CheckReady: boolean;
const
  WarningMSG = 'Не указано значение измерения "%s".' + #13
    + 'Для выполнения операции следует указать значения для всех элементов строк.';
var
  i: integer;
  NodeList: IXMLDOMNodeList;
begin
  result := true;
  with PlaningSheet do
    for i := 0 to Rows.Count - 1 do
    begin
      NodeList := GetCheckedLeafMembers(FPlaningSheet.Rows[i].Members);
      if not Assigned(NodeList) or (NodeList.Length = 0) then
      begin
        result := false;
        ShowWarning(Format(WarningMSG, [Rows[i].FullDimensionName]));
        tsDimChooser.TabIndex := i;
        exit;
      end;
    end;
end;

procedure TfrmAppendEmptyRowToSheet.btCancelClick(Sender: TObject);
begin
  FApply := false;
  Close;
end;

procedure TfrmAppendEmptyRowToSheet.FormResize(Sender: TObject);
begin
  Invalidate;
end;

procedure TfrmAppendEmptyRowToSheet.OnNodeCheckHandler(Sender: TObject);
var
  CurNode, ParentNode: TCheckTreeNode2;
  ResumeText: TStringList;
  WasChecked: boolean;
  NodeData: IXMLDOMNode;
begin
  Tree.OnNodeCheck := nil;
  try
    if Visible then
    begin
      CurNode := Tree.TreeView.Selected as TCheckTreeNode2;
      if Assigned(CurNode) then
      begin
        if CurNode.Checked  then
          CurNode.Checked := true
        else
          CurNode.Checked := false;

        ParentNode := CurNode.Parent as TCheckTreeNode2;
        if CurNode.Checked then
          while Assigned(ParentNode) do
          begin
            NodeData := ParentNode.DomNode;
            {Если мембер небыл отмечен в исходном дереве, то и в цепочку
            размещаемых элементов включать его не будем}
            WasChecked := GetBoolAttr(NodeData, attrWasChecked, true);
            if WasChecked then
              ParentNode.Checked := true;
            ParentNode := ParentNode.Parent as TCheckTreeNode2;
          end
        else
          while Assigned(ParentNode) do
          begin
            if not ParentNode.HasCheckedChildren[true] then
              ParentNode.Checked := false;
            ParentNode := ParentNode.Parent as TCheckTreeNode2;
          end;
      end;

      {Обновим комментарий о выбранных элементах измерений}
      ResumeText := GetResumeText(FPlaningSheet);
      memResume.Lines := ResumeText;
      FreeStringList(ResumeText);
    end;
  finally
    Tree.OnNodeCheck := OnNodeCheckHandler;
  end;
end;

procedure TfrmAppendEmptyRowToSheet.btOKClick(Sender: TObject);
begin
  FApply := CheckReady;
  if FApply then
    close;
end;

procedure TfrmAppendEmptyRowToSheet.tsDimChooserChange(Sender: TObject;
  NewTab: Integer; var AllowChange: Boolean);
var
  Elem: TSheetAxisElementInterface;
  Hier: THierarchy;
  CodeToShow: string;
begin
  Tree.Clear;
  if Assigned(PlaningSheet) then
    try
      Elem := PlaningSheet.Rows[NewTab];
      Hier := Elem.CatalogHierarchy;
      if Assigned(Hier) then
        CodeToShow := Hier.CodeToShow
      else
        CodeToShow := '';
      Tree.Load(Elem.Members, Elem.Levels.ToString, CodeToShow);
      Tree.DisableNonLeaves;
      Tree.ExpandCheckedButtonClick(self);
    except
      ShowError(ermUnknown)
    end;
end;

end.

