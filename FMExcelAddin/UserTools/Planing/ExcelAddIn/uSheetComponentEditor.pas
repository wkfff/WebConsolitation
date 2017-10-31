unit uSheetComponentEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  uSheetObjectModel, StdCtrls, ExtCtrls, ComCtrls, ExcelXP, FileCtrl,
  uCheckTV2, uFMAddinGeneralUtils, Spin, uFMAddinExcelUtils, uExcelUtils,
  uXMLCatalog, Buttons, uSheetStyles, uGlobalPlaningConst, uFMExcelAddinConst,
  uEvents, uOfficeUtils, ImgList, uSheetBreaks, uMSWordUtils, uFMAddinXMLUtils,
  uMPSelector, uStringsEditor, uFMAddinRegistryUtils;

type

  TfmSheetComponentEditor = class(TForm)
    Bevel1: TBevel;
    Panel1: TPanel;
    tvSheet: TBasicCheckTreeView;
    imgList: TImageList;
    Panel4: TPanel;
    pcEditor: TPageControl;
    tsSheetProperties: TTabSheet;
    Bevel8: TBevel;
    Label13: TLabel;
    Label14: TLabel;
    Bevel9: TBevel;
    cbShowFilters: TCheckBox;
    cbShowColumns: TCheckBox;
    cbShowColumnsTitles: TCheckBox;
    cbShowRowsTitles: TCheckBox;
    cbShowTotalsTitles: TCheckBox;
    cbShowSheetInfo: TCheckBox;
    cbPrintable: TCheckBox;
    cbCommentStructuralCell: TCheckBox;
    tsAxisCollectionProperties: TTabSheet;
    cbHideEmpty: TCheckBox;
    cbBroken: TCheckBox;
    cbMPBefore: TCheckBox;
    cbLevelsFormatting: TCheckBox;
    tsAxisElementProperties: TTabSheet;
    cbIgnoreHierarchy: TCheckBox;
    cbHideDataMembers: TCheckBox;
    tsTotalProperties: TTabSheet;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Bevel4: TBevel;
    Label10: TLabel;
    lblTotalFunction: TLabel;
    lbEmptyValueSymbol: TLabel;
    Label11: TLabel;
    Bevel5: TBevel;
    edCaption: TEdit;
    cmbTotalFormat: TComboBox;
    seDigits: TSpinEdit;
    cbTotalIgnoreColumns: TCheckBox;
    cbTotalGrandSummaryDataOnly: TCheckBox;
    cbSummariesByVisible: TCheckBox;
    cmbTotalFunction: TComboBox;
    edEmptyValueSymbol: TEdit;
    tsSingleCellProperties: TTabSheet;
    Bevel3: TBevel;
    Label6: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label7: TLabel;
    edSingleCellCaption: TEdit;
    cmbSingleCellFormat: TComboBox;
    seSingleCellDigits: TSpinEdit;
    tsTotalCollectionProperties: TTabSheet;
    Label12: TLabel;
    Bevel7: TBevel;
    cbNeedRound: TCheckBox;
    cbFormatTotalsArea: TCheckBox;
    pnMarkerPlace: TPanel;
    rbMarkerToRight: TRadioButton;
    rbMarkerToLeft: TRadioButton;
    Panel3: TPanel;
    rbFormatByColumns: TRadioButton;
    rbFormatByRows: TRadioButton;
    tsFilterCollectionProperties: TTabSheet;
    Label15: TLabel;
    Bevel10: TBevel;
    cbFullFilterText: TCheckBox;
    rbFilterCellsByUser: TRadioButton;
    rbFilterCellsByTable: TRadioButton;
    seLengthFilterCells: TSpinEdit;
    tsSummaries: TTabSheet;
    Label1: TLabel;
    Bevel6: TBevel;
    cbUseSummaryOptionsForChildren: TCheckBox;
    cbSummaryOptimization: TCheckBox;
    tsFilterScope: TTabSheet;
    rbCommonFilter: TRadioButton;
    rbPartialFilter: TRadioButton;
    lvFilterScope: TListView;
    tsStyles: TTabSheet;
    lblValueStyle: TLabel;
    lblValuePrintStyle: TLabel;
    lblTitleStyle: TLabel;
    lblTitlePrintStyle: TLabel;
    Bevel2: TBevel;
    Label5: TLabel;
    cmbValueStyle: TComboBox;
    cmbValuePrintStyle: TComboBox;
    cmbTitleStyle: TComboBox;
    cmbTitlePrintStyle: TComboBox;
    rbThisElement: TRadioButton;
    rbAllElements: TRadioButton;
    btnDefaultStyles: TButton;
    cbUseStylesForChildren: TCheckBox;
    tsFormat: TTabSheet;
    Label21: TLabel;
    Bevel11: TBevel;
    pnFontSample: TPanel;
    btnFont: TButton;
    cbAllCapitals: TCheckBox;
    cbUseFormat: TCheckBox;
    tsTask: TTabSheet;
    Label16: TLabel;
    Label17: TLabel;
    cmbSheetType: TComboBox;
    memProperties: TMemo;
    tsEvents: TTabSheet;
    cbBeforeRefresh: TCheckBox;
    cmbBeforeRefresh: TComboBox;
    cbAfterRefresh: TCheckBox;
    cmbAfterRefresh: TComboBox;
    cbBeforeWriteBack: TCheckBox;
    cmbBeforeWriteBack: TComboBox;
    cbAfterWriteBack: TCheckBox;
    cmbAfterWriteBack: TComboBox;
    mMarkerImpossiblyMacros: TMemo;
    tsProcessing: TTabSheet;
    Label18: TLabel;
    Label19: TLabel;
    Label20: TLabel;
    rbNormal: TRadioButton;
    rbLarge: TRadioButton;
    rbHuge: TRadioButton;
    tsHistory: TTabSheet;
    meHistory: TMemo;
    cbCommentDataCell: TCheckBox;
    mCellDetail: TMemo;
    Label22: TLabel;
    tsFilterProperties: TTabSheet;
    Label23: TLabel;
    Bevel12: TBevel;
    Label24: TLabel;
    Bevel13: TBevel;
    cbAllowNECJ: TCheckBox;
    tsMSWord: TTabSheet;
    Label25: TLabel;
    Bevel14: TBevel;
    rbHeadingThisTableArea: TRadioButton;
    rbHeadingThisCustomArea: TRadioButton;
    Label29: TLabel;
    rbHeadingNoDefine: TRadioButton;
    cbHeadingEnd: TComboBox;
    lbStart: TLabel;
    lbEnd: TLabel;
    edHeadingAddress: TEdit;
    lbAddress: TLabel;
    btDefineHeadingAddress: TButton;
    tsTotalFormulas: TTabSheet;
    Bevel15: TBevel;
    Label26: TLabel;
    cbIsUseTypeFormula: TCheckBox;
    btAssignTypeFormula: TButton;
    mUserTypeFormula: TMemo;
    Label27: TLabel;
    cbSheetPermitEditing: TCheckBox;
    cbAxisCollectionPermitEditing: TCheckBox;
    cbAxisElementPermitEditing: TCheckBox;
    cbTotalPermitEditing: TCheckBox;
    cbSingleCellPermitEditing: TCheckBox;
    cbTotalCollectionPermitEditing: TCheckBox;
    cbFilterCollectionPermitEditing: TCheckBox;
    cbFilterPermitEditing: TCheckBox;
    tsSingleCellsCollectionProperties: TTabSheet;
    cbSingleCellCollectionPermitEditing: TCheckBox;
    cbUseIndents: TCheckBox;
    pnButtons: TPanel;
    btnOK: TButton;
    btnCancel: TButton;
    btnTaskDisconnect: TButton;
    rbMarkerHidden: TRadioButton;
    FMPSelector: TMPSelector;
    AMPSelector: TMPSelector;
    tsGrandSummary: TTabSheet;
    pnGSDeployment: TPanel;
    Label28: TLabel;
    rbGSNone: TRadioButton;
    rbGSTop: TRadioButton;
    rbGSBottom: TRadioButton;
    Bevel16: TBevel;
    pnGSProperties: TPanel;
    Label30: TLabel;
    Bevel17: TBevel;
    Label31: TLabel;
    edGSTitle: TEdit;
    btnGSFont: TButton;
    pnGSFontSample: TPanel;
    cbGSAllCapitals: TCheckBox;
    Label32: TLabel;
    Bevel18: TBevel;
    rbSummaryNone: TRadioButton;
    rbSummaryTop: TRadioButton;
    rbSummaryBottom: TRadioButton;
    Label33: TLabel;
    edSummaryTitle: TEdit;
    cbSummaryAllCapitals: TCheckBox;
    btnSummaryFont: TButton;
    pnSummaryFontSample: TPanel;
    tsDataMembers: TTabSheet;
    pnDMDeployment: TPanel;
    Label34: TLabel;
    Bevel19: TBevel;
    rbDMNone: TRadioButton;
    rbDMTop: TRadioButton;
    rbDMBottom: TRadioButton;
    cbUseDMTitle: TCheckBox;
    edDMTitle: TEdit;
    Label35: TLabel;
    Label36: TLabel;
    cmbCellMultiplier: TComboBox;
    cmbTotalMultiplier: TComboBox;
    cbReverseOrder: TCheckBox;
    cbAxisReverseOrder: TCheckBox;
    procedure tvSheetChange(Sender: TObject; Node: TTreeNode);
    procedure tvSheetChanging(Sender: TObject; Node: TTreeNode;
      var AllowChange: Boolean);
    procedure lvFilterScopeSelectItem(Sender: TObject; Item: TListItem;
      Selected: Boolean);
    procedure rbCommonFilterClick(Sender: TObject);
    procedure lvFilterScopeMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure cbSummariesByVisibleClick(Sender: TObject);
    procedure pcEditorChange(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure btnDefaultStylesClick(Sender: TObject);
    procedure btnFontClick(Sender: TObject);
    procedure cbAllCapitalsClick(Sender: TObject);
    procedure tvSheetStartDrag(Sender: TObject;
      var DragObject: TDragObject);
    procedure tvSheetDragOver(Sender, Source: TObject; X, Y: Integer;
      State: TDragState; var Accept: Boolean);
    procedure tvSheetDragDrop(Sender, Source: TObject; X, Y: Integer);
    procedure rbHeadingNoDefineClick(Sender: TObject);
    procedure rbHeadingThisTableAreaClick(Sender: TObject);
    procedure rbHeadingThisCustomAreaClick(Sender: TObject);
    procedure btDefineHeadingAddressClick(Sender: TObject);
    procedure btAssignTypeFormulaClick(Sender: TObject);
    procedure cbFormatTotalsAreaClick(Sender: TObject);
    procedure cmbTotalFormatClick(Sender: TObject);
    procedure cmbSheetTypeClick(Sender: TObject);
    procedure tvSheetKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure btnTaskDisconnectClick(Sender: TObject);
    procedure GSRadioClick(Sender: TObject);
    procedure SummaryRadioClick(Sender: TObject);
    procedure btnGSFontClick(Sender: TObject);
    procedure btnSummaryFontClick(Sender: TObject);
    procedure cbSummaryAllCapitalsClick(Sender: TObject);
    procedure cbGSAllCapitalsClick(Sender: TObject);
    procedure cbUseDMTitleClick(Sender: TObject);
    procedure DMRadioClick(Sender: TObject);
    procedure cmbTotalMultiplierClick(Sender: TObject);
    procedure cbIgnoreHierarchyClick(Sender: TObject);
    procedure cbBrokenClick(Sender: TObject);
  private
    SheetInterface: TSheetInterface;
    ExcelSheet: ExcelWorkSheet;
    FHistory: TStringList;
    FNeedStylesToDefault: boolean;
    Events: TExcelEvents;
    FOldSheetType: integer;
    RegularFontOptions, SummaryFontOptions, GSFontOptions: TFontOptions;
    FPreviousTab: TTabSheet;
    DraggingNode: TBasicCheckTreeNode;
    MayBeEdited: boolean;
    FNeedSwitchOffline, FNeedClearTaskInfo: boolean;
    DeletedCellsNames: TStringList;
    SummaryDeployment, GSDeployment, DataMembersDeployment: TItemDeployment;

    {строит дерево объектов листа}
    procedure BuildSheetTree;
    {загрузка списка стилей текущей книги}
    procedure LoadStyles;

    procedure SetupStyles(Element: TSheetElement); overload;
    procedure SetupStyles(Collection: TSheetCollection); overload;
    procedure SetupTaskInfo;
    procedure SetupEvents;
    procedure SetupFontOptions(RegularFont, SummaryFont, GSFont: TFontOptions);

    {следующие процедуры инициализируют редактор свойствами объектов}
    procedure EditSheetInterface;
    procedure EditTotal(Total: TSheetTotalInterface);
    procedure EditTotalCollection(TotalCollection: TSheetTotalCollectionInterface);
    procedure EditAxisElement(AxisElement: TSheetAxisElementInterface);
    procedure EditAxisCollection(AxisCollection: TSheetAxisCollectionInterface);
    procedure EditFilter(Filter: TSheetFilterInterface);
    procedure EditFilterCollection(FilterCollection: TSheetFilterCollectionInterface);
    procedure EditSingleCell(SingleCell: TSheetSingleCellInterface);
    procedure EditSingleCellCollection(SingleCellCollection: TSheetSingleCellCollectionInterface);
    procedure EditLevel(Level: TSheetLevelInterface);

    {а эти процедуры применяют настройки редактора к объектам,
      результат - были ли внесены изменения}
    function ApplySheetInterfaceChanges: boolean;
    function ApplyTotalChanges(Total: TSheetTotalInterface): boolean;
    function ApplyTotalCollectionChanges(TotalCollection: TSheetTotalCollectionInterface): boolean;
    function ApplyAxisElementChanges(AxisElement: TSheetAxisElementInterface): boolean;
    function ApplyAxisCollectionChanges(AxisCollection: TSheetAxisCollectionInterface): boolean;
    function ApplyFilterChanges(Filter: TSheetFilterInterface): boolean;
    function ApplyFilterCollectionChanges(FilterCollection: TSheetFilterCollectionInterface): boolean;
    function ApplySingleCellChanges(SingleCell: TSheetSingleCellInterface): boolean;
    function ApplySingleCellCollectionChanges(
      SingleCellCollection: TSheetSingleCellCollectionInterface): boolean;
    function ApplyLevelChanges(Level: TSheetLevelInterface): boolean;
    function ApplySummaryChanges(SummaryOptions: TSummaryOptions; IsGrandSummary: boolean): boolean;

    function GetFilterScope: TStringList;
    procedure SetFilterScope(Filter: TSheetFilterInterface);
    {применяет изменения к стилям элемента листа,
      результат - были ли внесены изменения}
    function ApplyElementStylesChanges(Element: TSheetElement): boolean;
    {применяет изменения к стилям коллекции листа,
      результат - были ли внесены изменения}
    function ApplyCollectionStylesChanges(Collection: TSheetCollection): boolean;
    function ApplyEventsChanges: boolean;
    function GetHeadingRange: ExcelRange;
    function GetTotalCell(Total: TSheetTotalInterface): ExcelRange;
    procedure EnablePages(Value: boolean);
    function FindElementNode(Id: string): TTreeNode;
    procedure DeleteElementHandler;
    function ConvertBuiltinStyleName(AName: string): string;
    {Очистим в листе ячейки, принадлежавшие удаленным отдельным показателям.}
    procedure ClearDeletedCells;
    procedure EditFont(var FontOptions: TFontOptions; SamplePanel: TPanel);
    procedure AllCapitalsClick(CheckBox: TCheckBox; Panel: TPanel); 
  public
    destructor Destroy; override;

    property History: TStringList read FHistory write FHistory;
    property NeedStylesToDefault: boolean read FNeedStylesToDefault
      write FNeedStylesToDefault;
  end;

  function EditSheetComponents(SheetIntf: TSheetInterface; var UserEvents: TExcelEvents;
    out HString: string; out StylesToDefault, NeedSwitchOffline, NeedClearTaskInfo: boolean;
    ElementId: string): boolean;

implementation

{$R *.DFM}

type
  TComponentNodeType = (cntNone, cntSheetInterface, cntTotal, cntTotalCollection,
    cntRow, cntColumn, cntRowCollection, cntColumnCollection, cntFilter, cntFilterCollection,
    cntSingleCell, cntSingleCellCollection, cntLevel, cntCommon);

const
  ElementTypes = [cntTotal, cntRow, cntColumn, cntFilter, cntSingleCell];

var
  SourceFormulaTemplate: string;

function EditSheetComponents(SheetIntf: TSheetInterface; var UserEvents: TExcelEvents;
  out HString: string; out StylesToDefault, NeedSwitchOffline, NeedClearTaskInfo: boolean;
  ElementId: string): boolean;
var
  SheetComponentEditor: TfmSheetComponentEditor;
  Node: TTreeNode;
begin
  SheetComponentEditor := TfmSheetComponentEditor.Create(nil);
  try
    with SheetComponentEditor do
    begin
      SheetInterface := SheetIntf;
      ExcelSheet := SheetInterface.ExcelSheet;
      Events := UserEvents;
      SetupEvents;
      BuildSheetTree;
      LoadStyles;
      SetupTaskInfo;
      FPreviousTab := tsSheetProperties;
      pcEditor.ActivePage := tsSheetProperties;
      Node := FindElementNode(ElementId);
      tvSheet.Selected := Node;
      tvSheetChange(nil, Node);
      History := TStringList.Create;
      NeedStylesToDefault := false;
      NeedSwitchOffline := false;
      DeletedCellsNames := TStringList.Create;
      result := ShowModal = mrOK;
      if result then
      begin
        HString := History.CommaText;
        StylesToDefault := NeedStylesToDefault;
        NeedSwitchOffline := FNeedSwitchOffline;
        NeedClearTaskInfo := FNeedClearTaskInfo;
        ClearDeletedCells;
      end;
    end;
  finally
    FreeAndNil(SheetComponentEditor);
  end;
end;

{ TfmSheetComponentEditor }

procedure TfmSheetComponentEditor.BuildSheetTree;
var
  i, j: integer;
  Node, RootNode, ElemNode, LevelNode: TBasicCheckTreeNode;
  ApplyExpansion: boolean;
  ExpansionInfo: array of boolean;
  Total: TSheetBasicTotal;
  Description: string;
begin
  ApplyExpansion := tvSheet.Items.Count > 0;
  if ApplyExpansion then
  begin
    SetLength(ExpansionInfo, tvSheet.Items[0].Count);
    for i := 0 to tvSheet.Items[0].Count - 1 do
      ExpansionInfo[i] := tvSheet.Items[0].Item[i].Expanded;
  end;
  tvSheet.Items.Clear;

  RootNode := tvSheet.Items.Add(nil, 'Лист') as TBasicCheckTreeNode;
  RootNode.Data := SheetInterface;
  RootNode.NodeType := ord(cntSheetInterface);
  RootNode.ImageIndex := 0;
  RootNode.SelectedIndex := 0;

  Node := tvSheet.Items.AddChild(RootNode, 'Показатели в таблице') as TBasicCheckTreeNode;
  Node.Data := SheetInterface.Totals;
  Node.NodeType := Ord(cntTotalCollection);
  Node.ImageIndex := 1;
  Node.SelectedIndex := 1;
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[i];
    case Total.TotalType of
      wtMeasure: Description := Format('%s (%s "%s")',
        [Total.GetElementCaption, Total.GetObjectTypeStr, Total.CubeName]);
      wtResult: Description := Format('%s (%s из куба "%s")',
        [Total.GetElementCaption, Total.GetObjectTypeStr, Total.CubeName]);
      else
        Description := Format('%s (%s)', [Total.GetElementCaption, Total.GetObjectTypeStr]);
    end;
    ElemNode := tvSheet.Items.AddChild(Node, Description) as TBasicCheckTreeNode;
    ElemNode.Data := SheetInterface.Totals[i];
    ElemNode.NodeType := Ord(cntTotal);
    ElemNode.ImageIndex := 1;
    ElemNode.SelectedIndex := 1;
    if Assigned(SheetInterface.Totals[i].Measure) then
      if SheetInterface.Totals[i].Measure.IsCalculated then
      begin
        ElemNode.ImageIndex := 5;
        ElemNode.SelectedIndex := 5;
      end;
  end;

  Node := tvSheet.Items.AddChild(RootNode, 'Строки') as TBasicCheckTreeNode;
  Node.Data := SheetInterface.Rows;
  Node.NodeType := Ord(cntRowCollection);
  Node.ImageIndex := 2;
  Node.SelectedIndex := 2;
  for i := 0 to SheetInterface.Rows.Count - 1 do
  begin
    ElemNode := tvSheet.Items.AddChild(Node,
      SheetInterface.Rows[i].GetElementCaption) as TBasicCheckTreeNode;
    ElemNode.Data := SheetInterface.Rows[i];
    ElemNode.NodeType := Ord(cntRow);
    ElemNode.ImageIndex := 2;
    ElemNode.SelectedIndex := 2;
    for j := 0 to SheetInterface.Rows[i].Levels.Count - 1 do
    begin
      LevelNode := tvSheet.Items.AddChild(ElemNode,
        SheetInterface.Rows[i].Levels[j].GetElementCaption) as TBasicCheckTreeNode;
      LevelNode.Data := SheetInterface.Rows[i].Levels[j];
      LevelNode.NodeType := Ord(cntLevel);
      LevelNode.ImageIndex := 6 + SheetInterface.Rows[i].Levels[j].GetDepth;
      LevelNode.SelectedIndex := LevelNode.ImageIndex;
    end;
  end;

  Node := tvSheet.Items.AddChild(RootNode, 'Столбцы') as TBasicCheckTreeNode;
  Node.Data := SheetInterface.Columns;
  Node.NodeType := Ord(cntColumnCollection);
  Node.ImageIndex := 2;
  Node.SelectedIndex := 2;
  for i := 0 to SheetInterface.Columns.Count - 1 do
  begin
    ElemNode := tvSheet.Items.AddChild(Node,
      SheetInterface.Columns[i].GetElementCaption) as TBasicCheckTreeNode;
    ElemNode.Data := SheetInterface.Columns[i];
    ElemNode.NodeType := Ord(cntColumn);
    ElemNode.ImageIndex := 2;
    ElemNode.SelectedIndex := 2;
    for j := 0 to SheetInterface.Columns[i].Levels.Count - 1 do
    begin
      LevelNode := tvSheet.Items.AddChild(ElemNode,
        SheetInterface.Columns[i].Levels[j].GetElementCaption) as TBasicCheckTreeNode;
      LevelNode.Data := SheetInterface.Columns[i].Levels[j];
      LevelNode.NodeType := Ord(cntLevel);
      LevelNode.ImageIndex := 6 + SheetInterface.Columns[i].Levels[j].GetDepth;
      LevelNode.SelectedIndex := LevelNode.ImageIndex;
    end;
  end;

  Node := tvSheet.Items.AddChild(RootNode, 'Фильтры') as TBasicCheckTreeNode;
  Node.Data := SheetInterface.Filters;
  Node.NodeType := Ord(cntFilterCollection);
  Node.ImageIndex := 3;
  Node.SelectedIndex := 3;
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    ElemNode := tvSheet.Items.AddChild(Node,
      SheetInterface.Filters[i].GetElementCaption) as TBasicCheckTreeNode;
    ElemNode.Data := SheetInterface.Filters[i];
    ElemNode.NodeType := Ord(cntFilter);
    ElemNode.ImageIndex := 3;
    ElemNode.SelectedIndex := 3;
  end;

  Node := tvSheet.Items.AddChild(RootNode, 'Отдельные показатели') as TBasicCheckTreeNode;
  Node.Data := SheetInterface.SingleCells;
  Node.NodeType := Ord(cntSingleCellCollection);
  Node.ImageIndex := 4;
  Node.SelectedIndex := 4;
  for i := 0 to SheetInterface.SingleCells.Count - 1 do
  begin
    Total := SheetInterface.SingleCells[i];
    if Total.TotalType <> wtConst then
      Description := Format('%s (%s из куба "%s")', [Total.GetElementCaption, Total.GetObjectTypeStr,
        Total.CubeName])
      else
        Description := Format('%s (%s)', [Total.GetElementCaption, Total.GetObjectTypeStr]);
    ElemNode := tvSheet.Items.AddChild(Node, Description) as TBasicCheckTreeNode;
    ElemNode.Data := SheetInterface.SingleCells[i];
    ElemNode.NodeType := Ord(cntSingleCell);
    ElemNode.ImageIndex := 4;
    ElemNode.SelectedIndex := 4;
  end;

  RootNode.Expand(false);
  if ApplyExpansion then
    for i := 0 to tvSheet.Items[0].Count - 1 do
      tvSheet.Items[0].Item[i].Expanded := ExpansionInfo[i];
end;

procedure TfmSheetComponentEditor.EditAxisElement(AxisElement: TSheetAxisElementInterface);

begin
  tsAxisElementProperties.TabVisible := true;
  tsSummaries.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsAxisElementProperties;

  MayBeEdited := AxisElement.MayBeEdited;
  EnablePages(MayBeEdited);
  cbAxisElementPermitEditing.Checked := AxisElement.PermitEditing;
  cbAxisElementPermitEditing.Enabled := SheetInterface.InConstructionMode and MayBeEdited;

  {итоги}
  with AxisElement.SummaryOptions do
  begin
    edSummaryTitle.Enabled := true;
    edSummaryTitle.Text := Title;
    case Deployment of
      idNone: rbSummaryNone.Checked := true;
      idTop: rbSummaryTop.Checked := true;
      idBottom: rbSummaryBottom.Checked := true;
    end;
    cbSummaryAllCapitals.Checked := AllCapitals;
    SummaryRadioClick(nil);
  end;

  cbUseSummaryOptionsForChildren.Visible := true;
  cbUseSummaryOptionsForChildren.Checked := AxisElement.UseSummariesForLevels;
  cbSummaryOptimization.Visible := false;

  SetupStyles(AxisElement);
  cbUseStylesForChildren.Visible := false;
  SetupFontOptions(nil, AxisElement.SummaryOptions.FontOptions, nil);

  cbIgnoreHierarchy.Checked := AxisElement.IgnoreHierarchy;
  cbIgnoreHierarchy.Enabled := MayBeEdited and
    not (AxisElement.Owner as TSheetAxisCollectionInterface).Broken;
  //cbReverseOrder.Checked := AxisElement.ReverseOrder;

  cbHideDataMembers.Checked := AxisElement.HideDataMembers;
  cbHideDataMembers.Enabled := cbIgnoreHierarchy.Enabled;
  {новый редактор МП в 2.3.2}
  AMPSelector.Load(AxisElement.MemberProperties);
end;

procedure TfmSheetComponentEditor.EditAxisCollection(AxisCollection: TSheetAxisCollectionInterface);
begin
  tsAxisCollectionProperties.TabVisible := true;
  tsSummaries.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;
  tsGrandSummary.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsAxisCollectionProperties;

  MayBeEdited := AxisCollection.MayBeEdited;
  EnablePages(MayBeEdited);
  cbAxisCollectionPermitEditing.Checked := AxisCollection.PermitEditing;
  cbAxisCollectionPermitEditing.Enabled := SheetInterface.InConstructionMode and MayBeEdited;

  {итоги}
  with AxisCollection.SummaryOptions do
  begin
    edSummaryTitle.Enabled := true;
    edSummaryTitle.Text := Title;
    case Deployment of
      idNone: rbSummaryNone.Checked := true;
      idTop: rbSummaryTop.Checked := true;
      idBottom: rbSummaryBottom.Checked := true;
    end;
    cbSummaryAllCapitals.Checked := AllCapitals;
    SummaryRadioClick(nil);
  end;
  cbSummaryOptimization.Visible := true;
  cbSummaryOptimization.Checked := AxisCollection.SummaryOptimization;

  {общий итог}
  with AxisCollection.GrandSummaryOptions do
  begin
    edGSTitle.Enabled := true;
    edGSTitle.Text := Title;

    case Deployment of
      idNone: rbGSNone.Checked := true;
      idTop: rbGSTop.Checked := true;
      idBottom: rbGSBottom.Checked := true;
    end;
    cbGSAllCapitals.Checked := AllCapitals;
    GSRadioClick(nil);
  end;

  cbUseSummaryOptionsForChildren.Visible := true;
  cbUseSummaryOptionsForChildren.Checked := AxisCollection.UseSummariesForElements;

  {стили}
  cbUseStylesForChildren.Visible := true;
  cbUseStylesForChildren.Checked := AxisCollection.UseStylesForChildren;
  SetupStyles(AxisCollection);

  {свойства}
  cbHideEmpty.Checked := AxisCollection.HideEmpty;
  cbBroken.Checked := AxisCollection.Broken;
  //cbAxisReverseOrder.Checked := AxisCollection.ReverseOrder;
  //cbAxisReverseOrder.Enabled := cbBroken.Checked;
  cbMPBefore.Checked := AxisCollection.MPBefore;
  cbLevelsFormatting.Checked := AxisCollection.LevelsFormatting;
  cbUseIndents.Enabled := (AxisCollection.AxisType = axRow) and MayBeEdited;
  cbUseIndents.Checked := AxisCollection.UseIndents;

  with AxisCollection do
    SetupFontOptions(nil, SummaryOptions.FontOptions, GrandSummaryOptions.FontOptions);
end;

procedure TfmSheetComponentEditor.EditFilter(Filter: TSheetFilterInterface);
begin
  tsFilterProperties.TabVisible := true;
  tsFilterScope.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsFilterScope;

  MayBeEdited := Filter.MayBeEdited;
  EnablePages(MayBeEdited);
  cbFilterPermitEditing.Checked := Filter.PermitEditing;
  cbFilterPermitEditing.Enabled := SheetInterface.InConstructionMode and MayBeEdited;

  rbPartialFilter.Checked := Filter.IsPartial;
  SetFilterScope(Filter);

  cbUseStylesForChildren.Visible := false;
  SetupStyles(Filter);

  FMPSelector.Load(Filter.MemberProperties);
end;

procedure TfmSheetComponentEditor.EditFilterCollection(
  FilterCollection: TSheetFilterCollectionInterface);
begin
  tsFilterCollectionProperties.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsFilterCollectionProperties;

  MayBeEdited := FilterCollection.MayBeEdited;
  EnablePages(MayBeEdited);
  cbFilterCollectionPermitEditing.Checked := FilterCollection.PermitEditing;
  cbFilterCollectionPermitEditing.Enabled := SheetInterface.InConstructionMode
    and MaybeEdited;

  cbFullFilterText.Checked := SheetInterface.DisplayFullFilterText;
  seLengthFilterCells.Value := SheetInterface.FilterCellsLength;
  rbFilterCellsByTable.Checked := SheetInterface.IsMergeFilterCellsByTable;


  cbUseStylesForChildren.Visible := true;
  cbUseStylesForChildren.Checked := FilterCollection.UseStylesForChildren;
  SetupStyles(FilterCollection);

end;

procedure TfmSheetComponentEditor.EditTotal(Total: TSheetTotalInterface);
var
  CurTotalType: TSheetTotalType;
begin
  tsTotalProperties.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;
  tsTotalFormulas.TabVisible := (Total.TotalType = wtFree) or (Total.TotalType = wtResult);

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsTotalProperties;

  MayBeEdited := Total.MayBeEdited;
  EnablePages(MayBeEdited);
  cbTotalPermitEditing.Checked := Total.PermitEditing;
  cbTotalPermitEditing.Enabled := SheetInterface.InConstructionMode and MayBeEdited;

  CurTotalType := Total.TotalType;

  {Заголовок показателя - не даем редактировать у констант}
  edCaption.Text := Total.Caption;
  edCaption.Enabled := (CurTotalType in [wtMeasure, wtResult, wtFree]) and MayBeEdited;


  {Формат и разрядность. Разрядность - только для числовых форматов}
  cmbTotalFormat.ItemIndex := Ord(Total.Format);
  seDigits.Value := Total.Digits;
  seDigits.Enabled := (Total.Format in [fmtCurrency, fmtPercent, fmtNumber]) and MayBeEdited;

  {"Размещать только в разрезе строк"}
  cbTotalIgnoreColumns.Checked := Total.IsIgnoredColumnAxis;
  cbTotalIgnoreColumns.Enabled := (CurTotalType in [wtFree, wtConst]) and
    not SheetInterface.Columns.Empty and MayBeEdited;

  {"Выводить значение только в главный итог"}
  cbTotalGrandSummaryDataOnly.Checked := Total.IsGrandTotalDataOnly;
  cbTotalGrandSummaryDataOnly.Enabled := (CurTotalType in [wtMeasure, wtResult]) and
    (Total.FitInAxis(axRow) = tfNone) and MayBeEdited;

  {"Итоги только по видимым"}
  cbSummariesByVisible.Checked := Total.SummariesByVisible;
  cbSummariesByVisible.Enabled := (CurTotalType in [wtMeasure, wtResult]) and
    Total.MayBeEdited;

  {Функция вычисления итогов.
    Если тип показателя "свободный" или "результат" позволяем выбирать тип
    подсчета итогов - "Типовая формула", иначе запрещаем.
    Разрешения для cmbTotalFunction взяты из обработчика cbSummariesByVisibleClick,
    здесь продублированы в явном виде для большей читабельности}
  if CurTotalType in [wtFree, wtResult] then
  begin
    if (cmbTotalFunction.Items.IndexOf(dmTypeFormula) < 0) then
      cmbTotalFunction.Items.Add(dmTypeFormula);
  end
  else
    if (cmbTotalFunction.Items.IndexOf(dmTypeFormula) > -1) then
      cmbTotalFunction.Items.Delete(cmbTotalFunction.Items.IndexOf(dmTypeFormula));
  cmbTotalFunction.ItemIndex := Ord(Total.CountMode);
  cmbTotalFunction.Enabled := cbSummariesByVisible.Checked and
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtStandard, fmtCurrency, fmtPercent, fmtNumber]) and MayBeEdited;
  lblTotalFunction.Enabled := cmbTotalFunction.Enabled;

  {Символ пустого значения}
  edEmptyValueSymbol.Text := Total.EmptyValueSymbol;
  edEmptyValueSymbol.Enabled := (CurTotalType in [wtMeasure, wtResult, wtFree])
    and MayBeEdited;


  cbUseStylesForChildren.Visible := false;
  SetupStyles(Total);

  {Формулы}
  if CurTotalType in [wtFree, wtResult] then
  begin
    cbIsUseTypeFormula.Checked := Total.TypeFormula.Enabled;
    mUserTypeFormula.Text := Total.TypeFormula.UserFormula;
    SourceFormulaTemplate := Total.TypeFormula.UserFormula;
  end;

end;

procedure TfmSheetComponentEditor.EditTotalCollection(
  TotalCollection: TSheetTotalCollectionInterface);
begin
  tsTotalCollectionProperties.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsStyles.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsTotalCollectionProperties;

  MayBeEdited := TotalCollection.MayBeEdited;
  EnablePages(MayBeEdited);
  cbTotalCollectionPermitEditing.Checked := TotalCollection.PermitEditing;
  cbTotalCollectionPermitEditing.Enabled := SheetInterface.InConstructionMode
    and MayBeEdited;

  cmbTotalMultiplier.ItemIndex := ord(SheetInterface.TotalMultiplier);
  cmbTotalMultiplierClick(nil);
  cbNeedRound.Checked := SheetInterface.NeedRound;
  case SheetInterface.MarkerPosition of
    mpLeft: rbMarkerToLeft.Checked := true;
    mpRight: rbMarkerToRight.Checked := true;
    else rbMarkerHidden.Checked := true;
  end;

  cbFormatTotalsArea.Checked := TotalCollection.StyleByLevels;
  cbFormatTotalsAreaClick(nil);
  rbFormatByColumns.Checked := not TotalCollection.FormatByRows and
    cbFormatTotalsArea.Checked;

end;

procedure TfmSheetComponentEditor.EditSingleCell(
  SingleCell: TSheetSingleCellInterface);
var
  DetailText: string;
begin
  tsSingleCellProperties.TabVisible := true;
  tsStyles.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsSingleCellProperties;

  MayBeEdited := SingleCell.MayBeEdited;
  EnablePages(MayBeEdited);
  cbSingleCellPermitEditing.Checked := SingleCell.PermitEditing;
  cbSingleCellPermitEditing.Enabled := SheetInterface.InConstructionMode and MayBeEdited;
  edSingleCellCaption.Enabled := SingleCell.TotalType <> wtConst;

  {свойства}
  edSingleCellCaption.Text := SingleCell.Name;
  cmbSingleCellFormat.Enabled := (SingleCell.TotalType <> wtResult) and MayBeEdited;
  cmbSingleCellFormat.ItemIndex := Ord(SingleCell.Format);
  seSingleCellDigits.Enabled := MayBeEdited and (cmbSingleCellFormat.ItemIndex in
          [Ord(fmtCurrency), Ord(fmtPercent), Ord(fmtNumber)]);
  seSingleCellDigits.Value := SingleCell.Digits;
  cmbCellMultiplier.ItemIndex := ord(SingleCell.TotalMultiplier);
  cmbCellMultiplier.Enabled := MayBeEdited and (SingleCell.Format = fmtCurrency);

  {стили}
  cbUseStylesForChildren.Visible := false;
  SetupStyles(SingleCell);

  {информация о мере и фильтрах}
  DetailText := SingleCell.CommentText;
  mCellDetail.Lines.BeginUpdate;
  mCellDetail.Lines.Clear;
  while DetailText <> '' do
    mCellDetail.Lines.Add(CutPart(DetailText, #10));
  mCellDetail.SelStart := 0;
  SendMessage(mCellDetail.Handle,EM_SCROLLCARET,0,0);
  mCellDetail.Lines.EndUpdate;

end;

procedure TfmSheetComponentEditor.EditSingleCellCollection(
  SingleCellCollection: TSheetSingleCellCollectionInterface);
begin
  tsSingleCellsCollectionProperties.TabVisible := true;
  tsHistory.TabVisible := true;

  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsStyles.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsSingleCellsCollectionProperties;

  MayBeEdited := SingleCellCollection.MayBeEdited;
  EnablePages(MayBeEdited);
  cbSingleCellCollectionPermitEditing.Checked := SingleCellCollection.PermitEditing;
  cbSingleCellCollectionPermitEditing.Enabled := MayBeEdited and
    SheetInterface.InConstructionMode;
end;

procedure TfmSheetComponentEditor.tvSheetChange(Sender: TObject;
  Node: TTreeNode);
var
  TreeNode: TBasicCheckTreeNode;
begin
  TreeNode := TBasicCheckTreeNode(Node);
  case TreeNode.NodeType of
    Ord(cntSheetInterface): EditSheetInterface;
    Ord(cntTotal): EditTotal(TreeNode.Data);
    Ord(cntTotalCollection): EditTotalCollection(TreeNode.Data);
    Ord(cntRow), Ord(cntColumn): EditAxisElement(TreeNode.Data);
    Ord(cntRowCollection), Ord(cntColumnCollection): EditAxisCollection(TreeNode.Data);
    Ord(cntFilter): EditFilter(TreeNode.Data);
    Ord(cntFilterCollection): EditFilterCollection(TreeNode.Data);
    Ord(cntSingleCell): EditSingleCell(TreeNode.Data);
    Ord(cntSingleCellCollection): EditSingleCellCollection(TreeNode.Data);
    Ord(cntLevel): EditLevel(TreeNode.Data);
  end;
end;

procedure TfmSheetComponentEditor.lvFilterScopeSelectItem(Sender: TObject;
  Item: TListItem; Selected: Boolean);
begin
  rbPartialFilter.Checked := true;
end;

procedure TfmSheetComponentEditor.rbCommonFilterClick(Sender: TObject);
var
  i: integer;
begin
  if rbCommonFilter.Checked then
    for i := 0 to lvFilterScope.Items.Count - 1 do
      lvFilterScope.Items[i].Checked := false;
end;

procedure TfmSheetComponentEditor.lvFilterScopeMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  ListItem: TListItem;
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
  try
    if (Button = mbleft) then
    begin
      ListItem := (Sender as TListView).GetItemAt(X, Y);
      if ListItem.Checked then
        rbPartialFilter.Checked := true;
    end;
  except
  end;
end;

procedure TfmSheetComponentEditor.SetFilterScope(
  Filter: TSheetFilterInterface);
var
  i: integer;
  Total: TSheetTotalInterface;
begin
  lvFilterScope.Items.Clear;
  if Assigned(Filter) then
    if Filter.IsPartial then
      rbPartialFilter.Checked := true
    else
      rbCommonFilter.Checked := true;
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[i];
    if Total.ProviderId <> Filter.ProviderId then
      continue;
    if (Total.TotalType = wtMeasure) or (Total.TotalType = wtResult) then
    begin
      //а вдруг база изменилась? тогда может и не быть такого куба
      if not Assigned(Total.Cube) then
        continue;
      if Total.Cube.DimAndHierInCube(Filter.Dimension, Filter.Hierarchy) then
      begin
        with lvFilterScope.Items.Add do
        begin
          Caption := Total.Caption;
          SubItems.Append('куб: ' + Total.CubeName + ' мера: ' + Total.MeasureName);
          SubItems.Append(Total.UniqueID);
          ImageIndex := 1;
          Checked := Filter.IsPartial and Total.IsFilteredBy(Filter)
        end;
      end;
    end;
  end;
end;

procedure TfmSheetComponentEditor.cbSummariesByVisibleClick(Sender: TObject);
begin
  cmbTotalFunction.Enabled := cbSummariesByVisible.Checked and
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtStandard, fmtCurrency, fmtPercent, fmtNumber]) and MayBeEdited;
  lblTotalFunction.Enabled := cmbTotalFunction.Enabled;
end;

function TfmSheetComponentEditor.ApplyTotalChanges(
  Total: TSheetTotalInterface): boolean;
var
  tmpStr: string;
begin
  result := false;
  History.Add(Format('Показатель "%s":', [Total.Caption]));
  {свойства}
  if Total.Caption <> edCaption.Text then
  begin
    tmpStr := Format(' - заголовок изменен на "%s"', [edCaption.Text]);
    History.Add(tmpStr);
    Total.Caption := edCaption.Text;
    result := true;
  end;
  if Total.EmptyValueSymbol <> edEmptyValueSymbol.Text then
  begin
    tmpStr := Format(' - символ пустого значения изменен на "%s"',
      [edEmptyValueSymbol.Text]);
    History.Add(tmpStr);
    Total.EmptyValueSymbol := edEmptyValueSymbol.Text;
    result := true;
  end;
  if Total.IsIgnoredColumnAxis <> cbTotalIgnoreColumns.Checked then
  begin
    tmpStr := Format(' - размещать только в разрезе строк %s',
      [IIF(cbTotalIgnoreColumns.Checked, 'включено', 'выключено')]);
    History.Add(tmpStr);
    Total.IsIgnoredColumnAxis := cbTotalIgnoreColumns.Checked;
    result := true;
  end;
  if Total.IsGrandTotalDataOnly <> cbTotalGrandSummaryDataOnly.Checked then
  begin
    tmpStr := Format(' - выводить значение только в строку главного итога %s',
      [IIF(cbTotalGrandSummaryDataOnly.Checked, 'включено', 'выключено')]);
    History.Add(tmpStr);
    Total.IsGrandTotalDataOnly := cbTotalGrandSummaryDataOnly.Checked;
    result := true;
  end;
  if Total.SummariesByVisible <> cbSummariesByVisible.Checked then
  begin
    tmpStr := Format(' - рассчитывать итоги только по видимым в листе элементам %s',
      [IIF(cbSummariesByVisible.Checked, 'включено', 'выключено')]);
    History.Add(tmpStr);
    Total.SummariesByVisible := cbSummariesByVisible.Checked;
    result := true;
  end;
  if Total.Format <> TMeasureFormat(cmbTotalFormat.ItemIndex) then
  begin
    tmpStr := Format(' - тип формата изменен на "%s"',
      [cmbTotalFormat.Text]);
    History.Add(tmpStr);
    Total.Format := TMeasureFormat(cmbTotalFormat.ItemIndex);
    result := true;
  end;
  if Total.CountMode <> TMeasureCountMode(cmbTotalFunction.ItemIndex) then
  begin
    tmpStr := Format(' - функция подсчета итогов изменена на "%s"',
      [cmbTotalFunction.Text]);
    History.Add(tmpStr);
    Total.CountMode := TMeasureCountMode(cmbTotalFunction.ItemIndex);
    result := true;
  end;
  if Total.Digits <> seDigits.Value then
  begin
    tmpStr := Format(' - число десятичных знаков изменено на "%d"',
      [seDigits.Value]);
    History.Add(tmpStr);
    Total.Digits := seDigits.Value;
    result := true;
  end;
  {стили}
  result := ApplyElementStylesChanges(Total) or result;

  if (Total.TotalType = wtFree) or (Total.TotalType = wtResult) then
  begin
    {формулы}
    if (Total.TypeFormula.Enabled <> cbIsUseTypeFormula.Checked) then
    begin
      tmpStr := Format(' - типовая формула %s', [IIF(cbIsUseTypeFormula.Checked,
        'включена', 'выключена')]);
      History.Add(tmpStr);
      Total.TypeFormula.Enabled := cbIsUseTypeFormula.Checked;
      result := true;
    end;
    if (Total.TypeFormula.UserFormula <> SourceFormulaTemplate) then
    begin
      tmpStr := ' - шаблон типовой формулы изменен на "' + Total.TypeFormula.UserFormula + '"';
      History.Add(tmpStr);
      SourceFormulaTemplate := Total.TypeFormula.UserFormula;
      result := true;
    end;
  end;

  if Total.PermitEditing <> cbTotalPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbTotalPermitEditing.Checked, 'включено', 'выключено'));
    Total.PermitEditing := cbTotalPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

function TfmSheetComponentEditor.ApplyAxisElementChanges(
  AxisElement: TSheetAxisElementInterface): boolean;
var
  i: integer;
  AxisCollection: TSheetAxisCollectionInterface;
begin
  result := false;
  History.Add(Format('%s "%s":', [AxisElement.GetObjectTypeStr, AxisElement.GetElementCaption]));
  {опции итогов}
  result := result or ApplySummaryChanges(AxisElement.SummaryOptions, false);

  if AxisElement.UseSummariesForLevels <> cbUseSummaryOptionsForChildren.Checked then
  begin
    History.Add(Format(' - использовать настройки итогов для подчиненных элементов %s',
      [IIF(cbUseSummaryOptionsForChildren.Checked, 'включено', 'выключено')]));
    AxisElement.UseSummariesForLevels := cbUseSummaryOptionsForChildren.Checked;
    result := true;
  end;
  if AxisElement.UseSummariesForLevels then
    for i := 0 to AxisElement.Levels.Count - 1 do
      AxisElement.Levels[i].SummaryOptions.Copy(AxisElement.SummaryOptions);

  AxisCollection := TSheetAxisCollectionInterface(AxisElement.Owner);
  if not AxisElement.SummaryOptions.IsEqualTo(AxisCollection.SummaryOptions) then
    AxisCollection.UseSummariesForElements := false;

  {стили}
  result := ApplyElementStylesChanges(AxisElement) or result;

  if AxisElement.IgnoreHierarchy <> cbIgnoreHierarchy.Checked then
  begin
    History.Add(Format(' - игнорировать иерархию %s',
      [IIF(cbIgnoreHierarchy.Checked, 'включено', 'выключено')]));
    AxisElement.IgnoreHierarchy := cbIgnoreHierarchy.Checked;
    result := true;
  end;
  (*if AxisElement.ReverseOrder <> cbReverseOrder.Checked then
  begin
    History.Add(Format(' - обратный порядок элементов %s',
      [IIF(cbIgnoreHierarchy.Checked, 'включен', 'выключен')]));
    AxisElement.ReverseOrder := cbReverseOrder.Checked;
    result := true;
  end;*)

  if AxisElement.HideDataMembers <> cbHideDataMembers.Checked then
  begin
    History.Add(Format(' - скрывать элементы (ДАННЫЕ) %s',
      [IIF(cbHideDataMembers.Checked, 'включено', 'выключено')]));
    AxisElement.HideDataMembers := cbHideDataMembers.Checked;
    result := true;
    for i := 0 to AxisElement.Levels.Count - 1 do
      if AxisElement.HideDataMembers then
        AxisElement.Levels[i].DMDeployment := idNone
      else
        AxisElement.Levels[i].DMDeployment := idTop;
  end;

  if AMPSelector.Changed then
  begin
    History.Add(' - изменен выбор и/или формат свойств элементов');
    AMPSelector.Save(AxisElement.MemberProperties);
    AMPSelector.UpdateState;
    result := true;
  end;

  if AxisElement.PermitEditing <> cbAxisElementPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbAxisElementPermitEditing.Checked, 'включено', 'выключено'));
    AxisElement.PermitEditing := cbAxisElementPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

function TfmSheetComponentEditor.ApplyFilterChanges(
  Filter: TSheetFilterInterface): boolean;
var
  tmpList: TStringList;
  tmpBool: boolean;
begin
  result := false;
  History.Add(Format('%s "%s":', [Filter.GetObjectTypeStr, Filter.GetElementCaption]));
  if Filter.IsPartial <> rbPartialFilter.Checked then
  begin
    History.Add(Format(' - тип фильтра изменен на "%s"',
      [IIF(rbPartialFilter.Checked, 'частный', 'общий')]));
    Filter.IsPartial := rbPartialFilter.Checked;
    result := true;
  end;

  tmpList := GetFilterScope;
  tmpBool := Assigned(tmpList) xor Assigned(Filter.Scope);
  if tmpBool then  //был частным - стал общим или наоборот
  begin
    Filter.Scope := tmpList;
    result := true;
  end
  else
    if Assigned(tmpList) then
      if Filter.Scope.CommaText <> tmpList.CommaText then
      begin
        History.Add(' - изменена область действия');
        Filter.Scope := tmpList;
        result := true;
      end;

  if FMPSelector.Changed then
  begin
    History.Add(' - изменен выбор и/или формат свойств элементов');
    FMPSelector.Save(Filter.MemberProperties);
    FMPSelector.UpdateState;
    result := true;
  end;

  {стили}
  result := ApplyElementStylesChanges(Filter) or result;

  if Filter.PermitEditing <> cbFilterPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbFilterPermitEditing.Checked, 'включено', 'выключено'));
    Filter.PermitEditing := cbFilterPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

procedure TfmSheetComponentEditor.tvSheetChanging(Sender: TObject;
  Node: TTreeNode; var AllowChange: Boolean);
var
  TreeNode: TBasicCheckTreeNode;
begin
  if not Assigned(tvSheet.Selected) then
    exit;
  TreeNode := TBasicCheckTreeNode(tvSheet.Selected);
  case TreeNode.NodeType of
    {!процедуры ApplyChanges обязательно должны стоять в условии первыми!}
    Ord(cntSheetInterface): TreeNode.NodeFlag :=
      ApplySheetInterfaceChanges or TreeNode.NodeFlag;
    Ord(cntTotal): TreeNode.NodeFlag :=
      ApplyTotalChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntRow), Ord(cntColumn): TreeNode.NodeFlag :=
      ApplyAxisElementChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntFilter): TreeNode.NodeFlag :=
      ApplyFilterChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntSingleCell): TreeNode.NodeFlag :=
      ApplySingleCellChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntSingleCellCollection): TreeNode.NodeFlag :=
      ApplySingleCellCollectionChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntLevel): TreeNode.NodeFlag :=
      ApplyLevelChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntRowCollection), Ord(cntColumnCollection): TreeNode.NodeFlag :=
      ApplyAxisCollectionChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntFilterCollection): TreeNode.NodeFlag :=
      ApplyFilterCollectionChanges(TreeNode.Data) or TreeNode.NodeFlag;
    Ord(cntTotalCollection): TreeNode.NodeFlag :=
      ApplyTotalCollectionChanges(TreeNode.Data) or TreeNode.NodeFlag;
  end;
  FPreviousTab := pcEditor.ActivePage;
end;

procedure TfmSheetComponentEditor.LoadStyles;
var
  i: integer;
  Book: ExcelWorkbook;
begin
  Book := ExcelSheet.Parent as ExcelWorkBook;
  for i := 1 to Book.Styles.Count do
    cmbValueStyle.Items.Add(ConvertBuiltinStyleName(Book.Styles[i].Name));
  cmbValuePrintStyle.Items.Assign(cmbValueStyle.Items);
  cmbTitleStyle.Items.Assign(cmbValueStyle.Items);
  cmbTitlePrintStyle.Items.Assign(cmbValueStyle.Items);
end;

procedure TfmSheetComponentEditor.SetupStyles(Element: TSheetElement);
begin
  lblValueStyle.Caption := Element.StyleCaption[esValue];
  cmbValueStyle.ItemIndex := cmbValueStyle.Items.IndexOf(
    ConvertBuiltinStyleName(Element.Styles.Name[esValue]));

  lblValuePrintStyle.Caption := Element.StyleCaption[esValuePrint];
  cmbValuePrintStyle.ItemIndex := cmbValuePrintStyle.Items.IndexOf(
    ConvertBuiltinStyleName(Element.Styles.Name[esValuePrint]));

  lblTitleStyle.Caption := Element.StyleCaption[esTitle];
  cmbTitleStyle.ItemIndex := cmbTitleStyle.Items.IndexOf(
    ConvertBuiltinStyleName(Element.Styles.Name[esTitle]));

  lblTitlePrintStyle.Caption := Element.StyleCaption[esTitlePrint];
  cmbTitlePrintStyle.ItemIndex := cmbTitlePrintStyle.Items.IndexOf(
    ConvertBuiltinStyleName(Element.Styles.Name[esTitlePrint]));

  {стили заголовков не используются отдельными показателями;
    контролы можно дизэйблить, а можно прятать}
  lblTitleStyle.Visible := not (Element is TSheetSingleCellInterface);
  cmbTitleStyle.Visible := not (Element is TSheetSingleCellInterface);
  lblTitlePrintStyle.Visible := not (Element is TSheetSingleCellInterface);
  cmbTitlePrintStyle.Visible := not (Element is TSheetSingleCellInterface);
end;

procedure TfmSheetComponentEditor.SetupStyles(
  Collection: TSheetCollection);
begin
  lblValueStyle.Caption := Collection.StyleCaption[esValue];
  with cmbValueStyle do
    ItemIndex := Items.IndexOf(ConvertBuiltinStyleName(Collection.Styles.Name[esValue]));

  lblValuePrintStyle.Caption := Collection.StyleCaption[esValuePrint];
  with cmbValuePrintStyle do
    ItemIndex := Items.IndexOf(ConvertBuiltinStyleName(Collection.Styles.Name[esValuePrint]));

  lblTitleStyle.Caption := Collection.StyleCaption[esTitle];
  with cmbTitleStyle do
    ItemIndex := Items.IndexOf(ConvertBuiltinStyleName(Collection.Styles.Name[esTitle]));

  lblTitlePrintStyle.Caption := Collection.StyleCaption[esTitlePrint];
  with cmbTitlePrintStyle do
    ItemIndex := Items.IndexOf(ConvertBuiltinStyleName(Collection.Styles.Name[esTitlePrint]));


end;

procedure TfmSheetComponentEditor.pcEditorChange(Sender: TObject);
var
  tmpBool: boolean;
begin
  tmpBool := true;
  tvSheetChanging(tvSheet, tvSheet.Selected, tmpBool);
  meHistory.Lines.Assign(History);
end;

procedure TfmSheetComponentEditor.btnOKClick(Sender: TObject);
var
  tmpBool: boolean;
begin
  tmpBool := true;
  tvSheetChanging(tvSheet, tvSheet.Selected, tmpBool);
end;

function TfmSheetComponentEditor.ApplyElementStylesChanges(
  Element: TSheetElement): boolean;
var
  Collection: TSheetCollection;
  StyleName: string;
begin
  result := false;
  StyleName := ConvertBuiltinStyleName(cmbValueStyle.Text);
  if StyleName <> '' then
    if Element.Styles.Name[esValue] <> StyleName then
    begin
      Element.Styles.Name[esValue] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;
  StyleName := ConvertBuiltinStyleName(cmbValuePrintStyle.Text);
  if StyleName <> '' then
    if Element.Styles.Name[esValuePrint] <> StyleName then
    begin
      Element.Styles.Name[esValuePrint] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;
  {два оставшихся стиля неприменимы к отдельным показателям}
  if not (Element is TSheetSingleCellInterface) then
  begin
    StyleName := ConvertBuiltinStyleName(cmbTitleStyle.Text);
    if StyleName <> '' then
      if Element.Styles.Name[esTitle] <> StyleName then
      begin
        Element.Styles.Name[esTitle] := StyleName;
        result := true;
        LockStyle((ExcelSheet.Parent as ExcelWorkBook));
      end;
    StyleName := ConvertBuiltinStyleName(cmbTitlePrintStyle.Text);
    if StyleName <> '' then
      if Element.Styles.Name[esTitlePrint] <> StyleName then
      begin
        Element.Styles.Name[esTitlePrint] := StyleName;
        result := true;
        LockStyle((ExcelSheet.Parent as ExcelWorkBook));
      end;
  end;

  if result then
  begin
    History.Add(' - изменены настройки стилей');
    Collection := Element.Owner;
    Collection.UseStylesForChildren := false;
    if Collection is TSheetLevelCollectionInterface then
      TSheetLevelCollectionInterface(Collection).Owner.Owner.UseStylesForChildren := false;
  end;

end;

function TfmSheetComponentEditor.GetFilterScope: TStringList;
var
  i: integer;
begin
  result := nil;
  if rbCommonFilter.Checked then //если фильтр общий, то возвращаем nil
    exit;
  result := TStringList.Create;
  for i := 0 to lvFilterScope.Items.Count - 1 do
    if lvFilterScope.Items[i].Checked then
      result.Add(lvFilterScope.Items[i].SubItems[1]);
end;

function TfmSheetComponentEditor.ApplySingleCellChanges(
  SingleCell: TSheetSingleCellInterface): boolean;
begin
  result := false;
  History.Add(Format('Отдельный показатель "%s":', [SingleCell.GetElementCaption]));
  if SingleCell.Name <> edSingleCellCaption.Text then
  begin
    History.Add(Format(' - заголовок изменен на %s', [edSingleCellCaption.Text]));
    SingleCell.Name := edSingleCellCaption.Text;
    result := true;
  end;
  if Ord(SingleCell.Format) <> cmbSingleCellFormat.ItemIndex then
  begin
    History.Add(Format(' - тип формата изменен на %s', [cmbSingleCellFormat.Text]));
    SingleCell.Format := TMeasureFormat(cmbSingleCellFormat.ItemIndex);
    result := true;
  end;
  if SingleCell.Digits <> seSingleCellDigits.Value then
  begin
    History.Add(Format(' - число десятичных знаков изменено на "%d"',
      [seSingleCellDigits.Value]));
    SingleCell.Digits := seSingleCellDigits.Value;
    result := true;
  end;
  if SingleCell.TotalMultiplier <> TTotalMultiplier(cmbCellMultiplier.ItemIndex) then
  begin
    History.Add(Format(' - выводить значения в %s',
      [cmbCellMultiplier.Items[cmbCellMultiplier.ItemIndex]]));
    SingleCell.TotalMultiplier := TTotalMultiplier(cmbCellMultiplier.ItemIndex);
    result := true;
  end;
  result := ApplyElementStylesChanges(SingleCell) or result;

  if SingleCell.PermitEditing <> cbSingleCellPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbSingleCellPermitEditing.Checked, 'включено', 'выключено'));
    SingleCell.PermitEditing := cbSingleCellPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

function TfmSheetComponentEditor.ApplySingleCellCollectionChanges(
  SingleCellCollection: TSheetSingleCellCollectionInterface): boolean;
begin
  result := false;
  History.Add('Коллекция отдельных показателей: ');

  if SingleCellCollection.PermitEditing <> cbSingleCellCollectionPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbSingleCellCollectionPermitEditing.Checked, 'включено', 'выключено'));
    SingleCellCollection.PermitEditing := cbSingleCellCollectionPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

function TfmSheetComponentEditor.ApplyLevelChanges(Level:
  TSheetLevelInterface): boolean;
var
  tmpStr: string;
begin
  result := false;
  History.Add(Format('Уровень "%s" измерения "%s":',
    [Level.GetElementCaption, Level.AxisElement.GetElementCaption]));

  result := result or ApplySummaryChanges(Level.SummaryOptions, false);

  with Level do
  begin
    if UseCustomDMTitle <> cbUseDMTitle.Checked then
    begin
      History.Add(Format(' - заменять наименование элементов (ДАННЫЕ) %s',
        [IIF(cbUseDMTitle.Checked, 'включено', 'выключено')]));
      UseCustomDMTitle := cbUseDMTitle.Checked;
      result := true;
    end;

    if CustomDMTitle <> edDMTitle.Text then
    begin
      History.Add(Format(' - наименование элементов (ДАННЫЕ) изменено на %s',
        [edDMTitle.Text]));
      CustomDMTitle := edDMTitle.Text;
      result := true;
    end;

    if DataMembersDeployment <> DMDeployment then
    begin
      case DataMembersDeployment of
        idNone: tmpStr := '"не размещать"';
        idTop: tmpStr := '"сверху"';
        idBottom: tmpStr := '"снизу"';
      end;
      History.Add(Format('- размещение элементов (ДАННЫЕ) изменено на %s', [tmpStr]));
      DMDeployment := DataMembersDeployment;
      if DMDeployment <> idNone then
        Level.AxisElement.HideDataMembers := false;
      result := true;
    end;
  end;

  if not Level.SummaryOptions.IsEqualTo(Level.AxisElement.SummaryOptions) then
  begin
    Level.AxisElement.UseSummariesForLevels := false;
    (Level.AxisElement.Owner as TSheetAxisCollectionInterface).UseSummariesForElements := false;
  end;

  if Level.UseFormat <> cbUseFormat.Checked then
  begin
    History.Add(Format(' - использовать дополнительный формат %s',
      [IIF(cbUseFormat.Checked, 'включено', 'выключено')]));
    Level.UseFormat := cbUseFormat.Checked;
    result := true;
  end;

  if not Level.FontOptions.IsEqualTo(RegularFontOptions) or
    (Level.AllCapitals <> cbAllCapitals.Checked) then
  begin
    History.Add(' - изменены настройки формата');
    Level.FontOptions.CopyFrom(RegularFontOptions);
    Level.AllCapitals := cbAllCapitals.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

procedure TfmSheetComponentEditor.EditLevel(Level: TSheetLevelInterface);
begin
  tsSummaries.TabVisible := true;
  tsFormat.TabVisible := true;
  tsHistory.TabVisible := true;

  tsStyles.TabVisible := false;
  tsTask.TabVisible := false;
  tsEvents.TabVisible := false;
  tsProcessing.TabVisible := false;
  tsSheetProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsMSWord.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := true;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsSummaries;

  {итоги}
  with Level.SummaryOptions do
  begin
    edSummaryTitle.Enabled := true;
    edSummaryTitle.Text := Title;
    case Deployment of
      idNone: rbSummaryNone.Checked := true;
      idTop: rbSummaryTop.Checked := true;
      idBottom: rbSummaryBottom.Checked := true;
    end;
    cbSummaryAllCapitals.Checked := AllCapitals;
    SummaryRadioClick(nil);
  end;

  cbUseSummaryOptionsForChildren.Visible := false;
  cbSummaryOptimization.Visible := false;

  cbUseStylesForChildren.Visible := false;

  cbAllCapitals.Checked := Level.AllCapitals;
  cbUseFormat.Checked := Level.UseFormat;
  cbUseFormat.Enabled :=
    (Level.AxisElement.Owner as TSheetAxisCollectionInterface).LevelsFormatting;

  {страница датамемберов}
  case Level.DMDeployment of
    idNone: rbDMNone.Checked := true;
    idTop: rbDMTop.Checked := true;
    idBottom: rbDMBottom.Checked := true;
  end;
  cbUseDMTitle.Checked := Level.UseCustomDMTitle;
  edDMTitle.Enabled := true;
  edDMTitle.Text := Level.CustomDMTitle;
  cbUseDMTitleClick(nil);
  DMRadioClick(nil);

  with Level do
    SetupFontOptions(FontOptions, SummaryOptions.FontOptions, nil);
end;

function TfmSheetComponentEditor.ApplyAxisCollectionChanges(
  AxisCollection: TSheetAxisCollectionInterface): boolean;
var
  i, j: integer;
  AxisElement: TSheetAxisElementInterface;
begin
  result := false;
  History.Add(IIF(AxisCollection.AxisType = axRow, 'Ось строк:', 'Ось столбцов:'));
  {опции итогов}
  result := result or ApplySummaryChanges(AxisCollection.SummaryOptions, false);

  {опции общего итога}
  result := result or ApplySummaryChanges(AxisCollection.GrandSummaryOptions, true);

  if AxisCollection.UseSummariesForElements <> cbUseSummaryOptionsForChildren.Checked then
  begin
    History.Add(Format(' - использовать настройки итогов для подчиненных элементов %s',
      [IIF(cbUseSummaryOptionsForChildren.Checked, 'включено', 'выключено')]));
    AxisCollection.UseSummariesForElements := cbUseSummaryOptionsForChildren.Checked;
    result := true;
  end;
  if AxisCollection.UseSummariesForElements then
    for i := 0 to AxisCollection.Count - 1 do
    begin
      AxisElement := AxisCollection.Items[i];
      AxisElement.SummaryOptions.Copy(AxisCollection.SummaryOptions);
      AxisElement.UseSummariesForLevels := true;
      for j := 0 to AxisElement.Levels.Count - 1 do
        AxisElement.Levels[j].SummaryOptions.Copy(AxisElement.SummaryOptions);
    end;

  {стили}
  result := ApplyCollectionStylesChanges(AxisCollection) or result;

  {свойства}
  if AxisCollection.HideEmpty <> cbHideEmpty.Checked then
  begin
    History.Add(Format(' - скрывать элементы, для которых отсутствуют данные %s',
      [IIF(cbHideEmpty.Checked, 'включено', 'выключено')]));
    AxisCollection.HideEmpty := cbHideEmpty.Checked;
    result := true;
  end;
  if AxisCollection.Broken <> cbBroken.Checked then
  begin
    History.Add(Format(' - игнорировать иерархию %s',
      [IIF(cbBroken.Checked, 'включено', 'выключено')]));
    AxisCollection.Broken := cbBroken.Checked;
    result := true;
  end;
  (*if AxisCollection.ReverseOrder <> cbAxisReverseOrder.Checked then
  begin
    History.Add(Format(' - обратный порядок элементов %s',
      [IIF(cbIgnoreHierarchy.Checked, 'включен', 'выключен')]));
    AxisCollection.ReverseOrder := cbAxisReverseOrder.Checked;
    result := true;
  end;*)
  if AxisCollection.MPBefore <> cbMPBefore.Checked then
  begin
    History.Add(Format(' - выносить свойства элементов перед осью %s',
      [IIF(cbMPBefore.Checked, 'включено', 'выключено')]));
    AxisCollection.MPBefore := cbMPBefore.Checked;
    result := true;
  end;
  if AxisCollection.SummaryOptimization <> cbSummaryOptimization.Checked then
  begin
    History.Add(Format(' - использовать оптимизацию итогов %s',
      [IIF(cbSummaryOptimization.Checked, 'включено', 'выключено')]));
    AxisCollection.SummaryOptimization := cbSummaryOptimization.Checked;
    result := true;
  end;

  if AxisCollection.LevelsFormatting <> cbLevelsFormatting.Checked then
  begin
    History.Add(Format(' - дополнительный формат уровней оси %s',
      [IIF(cbLevelsFormatting.Checked, 'включен', 'выключен')]));
    AxisCollection.LevelsFormatting := cbLevelsFormatting.Checked;
    result := true;
  end;
  if AxisCollection.AxisType = axRow then
    if AxisCollection.UseIndents <> cbUseIndents.Checked then
    begin
      History.Add(Format(' - использование отступов %s',
        [IIF(cbUseIndents.Checked, 'включено', 'выключено')]));
      AxisCollection.UseIndents := cbUseIndents.Checked;
      result := true;
    end;

  if AxisCollection.PermitEditing <> cbAxisCollectionPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbAxisCollectionPermitEditing.Checked, 'включено', 'выключено'));
    AxisCollection.PermitEditing := cbAxisCollectionPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

function TfmSheetComponentEditor.ApplyCollectionStylesChanges(
  Collection: TSheetCollection): boolean;
var
  i: integer;
  StyleName: string;
begin
  result := false;
  {к неоднородным коллекциям не даем применять единые стили}
  if Collection is TSheetTotalCollectionInterface then
    exit;
  if Collection is TSheetSingleCellCollectionInterface then
    exit;

  StyleName := ConvertBuiltinStyleName(cmbValueStyle.Text);
  if StyleName <> '' then
    if Collection.Styles.Name[esValue] <> StyleName then
    begin
      Collection.Styles.Name[esValue] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;
  StyleName := ConvertBuiltinStyleName(cmbValuePrintStyle.Text);
  if StyleName <> '' then
    if Collection.Styles.Name[esValuePrint] <> StyleName then
    begin
      Collection.Styles.Name[esValuePrint] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;
  StyleName := ConvertBuiltinStyleName(cmbTitleStyle.Text);
  if StyleName <> '' then
    if Collection.Styles.Name[esTitle] <> StyleName then
    begin
      Collection.Styles.Name[esTitle] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;
  StyleName := ConvertBuiltinStyleName(cmbTitlePrintStyle.Text);
  if StyleName <> '' then
    if Collection.Styles.Name[esTitlePrint] <> StyleName then
    begin
      Collection.Styles.Name[esTitlePrint] := StyleName;
      result := true;
      LockStyle((ExcelSheet.Parent as ExcelWorkBook));
    end;

  if Collection.UseStylesForChildren <> cbUseStylesForChildren.Checked then
  begin
    Collection.UseStylesForChildren := cbUseStylesForChildren.Checked;
    result := true;
  end;
  if Collection.UseStylesForChildren then
  begin
    for i := 0 to Collection.Count - 1 do
      Collection.Items[i].SetOwnerStyles;
  end;
  if result then
    History.Add(' - изменены настройки стилей');
end;

function TfmSheetComponentEditor.ApplyFilterCollectionChanges(
  FilterCollection: TSheetFilterCollectionInterface): boolean;
begin
  result := false;
  History.Add('Коллекция фильтров листа: ');

  if SheetInterface.DisplayFullFilterText <> cbFullFilterText.Checked then
  begin
    History.Add(Format(' - выводить полные описания фильтров %s',
      [IIF(cbFullFilterText.Checked, 'включено', 'выключено')]));
    SheetInterface.DisplayFullFilterText := cbFullFilterText.Checked;
    result := true;
  end;
  if (SheetInterface.IsMergeFilterCellsByTable <> rbFilterCellsByTable.Checked) then
  begin
    History.Add(Format(' - количество объединенных ячеек для фильтров %s',
      [IIF(rbFilterCellsByTable.Checked, 'равно ширине таблицы', 'определяется пользователем')]));
    SheetInterface.IsMergeFilterCellsByTable := rbFilterCellsByTable.Checked;
    result := true;
  end;
  if (SheetInterface.FilterCellsLength <> seLengthFilterCells.Value) then
  begin
    History.Add(Format(' - количество объединенных ячеек %d',
      [seLengthFilterCells.Value]));
    SheetInterface.FilterCellsLength := seLengthFilterCells.Value;
    result := true;
  end;

  result := ApplyCollectionStylesChanges(FilterCollection) or result;

  if FilterCollection.PermitEditing <> cbFilterCollectionPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbFilterCollectionPermitEditing.Checked, 'включено', 'выключено'));
    FilterCollection.PermitEditing := cbFilterCollectionPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

procedure TfmSheetComponentEditor.btnDefaultStylesClick(Sender: TObject);
var
  Element: TSheetElement;
  Collection: TSheetCollection;
begin
  if rbThisElement.Checked then
  begin
    case TComponentNodeType(TBasicCheckTreeNode(tvSheet.Selected).NodeType) of
      cntTotal, cntRow, cntColumn, cntFilter, cntSingleCell, cntLevel:
        begin
          Element := TSheetElement(tvSheet.Selected.Data);
          Element.SetDefaultStyles;
          if Element is TSheetAxisElementInterface then
            TSheetAxisElementInterface(Element).Levels.SetDefaultStyles2All;
          SetupStyles(Element);
        end;
      cntRowCollection, cntColumnCollection,  cntFilterCollection:
        begin
          Collection := TSheetCollection(tvSheet.Selected.Data);
          Collection.SetDefaultStyles;
          SetupStyles(Collection);
        end;
    end;
  end
  else
    with SheetInterface do
    try
      Totals.SetDefaultStyles2All;
      Rows.SetDefaultStyles2All;
      Columns.SetDefaultStyles2All;
      Filters.SetDefaultStyles2All;
      SingleCells.SetDefaultStyles2All;
      History.Add('Настройки стилей листа установлены по умолчанию');
    finally
      case TComponentNodeType(TBasicCheckTreeNode(tvSheet.Selected).NodeType) of
        cntTotal, cntRow, cntColumn, cntFilter, cntSingleCell, cntLevel:
          begin
            Element := TSheetElement(tvSheet.Selected.Data);
            SetupStyles(Element);
          end;
        cntRowCollection, cntColumnCollection,  cntFilterCollection:
          begin
            Collection := TSheetCollection(tvSheet.Selected.Data);
            SetupStyles(Collection);
          end;
      end;
      NeedStylesToDefault := true;
    end;
end;

function TfmSheetComponentEditor.ApplyTotalCollectionChanges(
  TotalCollection: TSheetTotalCollectionInterface): boolean;

  function GetMarkerPosition: TMarkerPosition;
  begin
    if rbMarkerToLeft.Checked then
      result := mpLeft
    else
      if rbMarkerToRight.Checked then
        result := mpRight
      else
        result := mpHidden;
  end;

var
  tmpString: string;
begin
  result := false;
  History.Add('Коллекция показателей: ');
  if ord(SheetInterface.TotalMultiplier) <> cmbTotalMultiplier.ItemIndex then
  begin
    History.Add(Format(' - задан вывод значений показателей в %s',
      [cmbTotalMultiplier.Items[cmbTotalMultiplier.ItemIndex]]));
    SheetInterface.TotalMultiplier := TTotalMultiplier(cmbTotalMultiplier.ItemIndex);
    result := true;
  end;
  if SheetInterface.NeedRound <> cbNeedRound.Checked then
  begin
    History.Add(Format(' - округлять значения показателей при обратной записи %s',
      [IIF(cbNeedRound.Checked, 'включено', 'выключено')]));
    SheetInterface.NeedRound := cbNeedRound.Checked;
    result := true;
  end;
  if SheetInterface.MarkerPosition <> GetMarkerPosition then
  begin
    case GetMarkerPosition of
      mpRight: tmpString := 'справа от таблицы';
      mpLeft: tmpString := 'слева от таблицы';
      else tmpString := 'не показывать';
    end;
    History.Add(Format(' - расположение индикатора (тыс. руб.) изменено на %s', [tmpString]));
    SheetInterface.MarkerPosition := GetMarkerPosition;
    result := true;
  end;
  if (TotalCollection.StyleByLevels <> cbFormatTotalsArea.Checked) or
    ((TotalCollection.FormatByRows <> rbFormatByRows.Checked) and
      rbFormatByRows.Enabled) then
  begin
    History.Add(Format(' - форматирование показателей по уровням оси %s %s',
      [IIF(rbFormatByRows.Checked, 'строк', 'столбцов'),
      IIF(cbFormatTotalsArea.Checked, 'включено', 'выключено')]));
    TotalCollection.StyleByLevels := cbFormatTotalsArea.Checked;
    TotalCollection.FormatByRows := rbFormatByRows.Checked;
    result := true;
  end;

  if TotalCollection.PermitEditing <> cbTotalCollectionPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbTotalCollectionPermitEditing.Checked, 'включено', 'выключено'));
    TotalCollection.PermitEditing := cbTotalCollectionPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

destructor TfmSheetComponentEditor.Destroy;
begin
  inherited;
  FHistory.Clear;
  FreeAndNil(FHistory);
  FreeStringList(DeletedCellsNames);
end;

function TfmSheetComponentEditor.ApplySheetInterfaceChanges: boolean;
var
  tmpInt: integer;
  SheetHeading: TSheetHeading;
begin
  result := false;
  History.Add('Лист:');

  if SheetInterface.IsDisplayFilters <> cbShowFilters.Checked then
  begin
    History.Add(Format(' - отображать элементы фильтров %s',
      [IIF(cbShowFilters.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayFilters := cbShowFilters.Checked;
    result := true;
  end;
  if SheetInterface.IsDisplayColumns <> cbShowColumns.Checked then
  begin
    History.Add(Format(' - отображать элементы оси столбцов %s',
      [IIF(cbShowColumns.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayColumns := cbShowColumns.Checked;
    result := true;
  end;
  if SheetInterface.IsDisplayColumnsTitles <> cbShowColumnsTitles.Checked then
  begin
    History.Add(Format(' - отображать заголовки оси столбцов %s',
      [IIF(cbShowColumnsTitles.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayColumnsTitles := cbShowColumnsTitles.Checked;
    result := true;
  end;
  if SheetInterface.IsDisplayRowsTitles <> cbShowRowsTitles.Checked then
  begin
    History.Add(Format(' - отображать заголовки оси строк %s',
      [IIF(cbShowRowsTitles.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayRowsTitles := cbShowRowsTitles.Checked;
    result := true;
  end;
  if SheetInterface.IsDisplayTotalsTitles <> cbShowTotalsTitles.Checked then
  begin
    History.Add(Format(' - отображать заголовки показателей %s',
      [IIF(cbShowTotalsTitles.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayTotalsTitles := cbShowTotalsTitles.Checked;
    result := true;
  end;
  if SheetInterface.IsDisplaySheetInfo <> cbShowSheetInfo.Checked then
  begin
    History.Add(Format(' - отображать информацию о задаче %s',
      [IIF(cbShowSheetInfo.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplaySheetInfo := cbShowSheetInfo.Checked;
    result := true;
  end;
  if SheetInterface.PrintableStyle <> cbPrintable.Checked then
  begin
    History.Add(Format(' - версия для печати %s',
      [IIF(cbPrintable.Checked, 'включено', 'выключено')]));
    SheetInterface.PrintableStyle := cbPrintable.Checked;
    result := true;
  end;

  if SheetInterface.IsDisplayCommentDataCell <> cbCommentDataCell.Checked then
  begin
    History.Add(Format(' - выводить комментарии к ячейкам с данными %s',
      [IIF(cbCommentDataCell.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayCommentDataCell := cbCommentDataCell.Checked;
    result := true;
  end;

  if SheetInterface.IsDisplayCommentStructuralCell <> cbCommentStructuralCell.Checked then
  begin
    History.Add(Format(' - выводить комментарии к ячейкам структуры таблицы %s',
      [IIF(cbCommentStructuralCell.Checked, 'включено', 'выключено')]));
    SheetInterface.IsDisplayCommentStructuralCell := cbCommentStructuralCell.Checked;
    result := true;
  end;

  result := ApplyEventsChanges or result;

  if FOldSheetType <> cmbSheetType.ItemIndex then
  begin
    tmpInt := cmbSheetType.ItemIndex;
    if (cmbSheetType.Items.Count = 3) and (tmpInt >= 2) then
      inc(tmpInt);
    History.Add(Format(' - вид листа изменен на "%s"', [cmbSheetType.Text]));
    result := true;
    SetWBCustomPropertyValue(
      ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties,
      pspSheetType, IntToStr(tmpInt));
    { Здесь переводим лист в автономную форму}
    if tmpInt = 3 then
    begin
      FNeedSwitchOffline := true;
      //SheetInterface.SwitchOffline;
      History.Add(' - лист переведен в автономный режим');
    end;
  end;

  if ((SheetInterface.TableProcessingMode = tpmNormal) and not rbNormal.Checked) or
    ((SheetInterface.TableProcessingMode = tpmLarge) and not rbLarge.Checked) or
    ((SheetInterface.TableProcessingMode = tpmHuge) and not rbHuge.Checked) then
  begin
    if rbNormal.Checked then
    begin
      History.Add(' - установлен обычный режим  оптимизации');
      SheetInterface.TableProcessingMode := tpmNormal;
    end;
    if rbLarge.Checked then
    begin
      History.Add(' - установлен дополнительный режим оптимизации');
      SheetInterface.TableProcessingMode := tpmLarge;
    end;
    if rbHuge.Checked then
    begin
      History.Add(' - установлен серверный режим оптимизации');
      SheetInterface.TableProcessingMode := tpmHuge;
    end;
    result := true;
  end;

  {Флаг разрешения NonEmptyCrossJoin}
  if (cbAllowNECJ.Checked <> SheetInterface.AllowNECJ) then
  begin
    SheetInterface.AllowNECJ := cbAllowNECJ.Checked;
    result := true;

    if cbAllowNECJ.Checked then
      History.Add(' - разрешено быстрое пересечение множеств в MDX')
    else
      History.Add(' - запрещено быстрое пересечение множеств в MDX');
  end;

  SheetHeading := SheetInterface.SheetHeading;
  if ((SheetInterface.SheetHeading.Type_ = htNoDefine) and not rbHeadingNoDefine.Checked) or
    ((SheetInterface.SheetHeading.Type_ = htTableArea) and not rbHeadingThisTableArea.Checked) or
    ((SheetInterface.SheetHeading.Type_ = htCustomArea) and not rbHeadingThisCustomArea.Checked) then
  begin
    if rbHeadingNoDefine.Checked then
    begin
      History.Add(' - установлено не вычислять заголовок таблицы');
      SheetHeading.Type_ := htNoDefine;
    end;
    if rbHeadingThisTableArea.Checked then
    begin
      History.Add(' - установлено заголовок - область таблицы');
      SheetHeading.Type_ := htTableArea;
    end;
    if rbHeadingThisCustomArea.Checked then
    begin
      History.Add(' - установлен заголовок - пользовательская область');
      SheetHeading.Type_ := htCustomArea;
    end;
    result := true;
  end;

  if (rbHeadingThisTableArea.Checked) and
    (SheetHeading.End_ <> GetPSObjectByCaption(cbHeadingEnd.Text)) then
  begin
    History.Add(' - изменена область заголовка таблицы, До начала: "' + cbHeadingEnd.Text + '"');
    SheetHeading.End_ := GetPSObjectByCaption(cbHeadingEnd.Text);
    result := true;
  end;

  if (SheetHeading.Address <> edHeadingAddress.Text) then
  begin
    History.Add(' - изменен адрес пользовательского заголовка на ' + edHeadingAddress.Text);
    SheetHeading.Address := edHeadingAddress.Text;
    result := true;
  end;

  SheetInterface.SheetHeading := SheetHeading;

  if SheetInterface.PermitEditing <> cbSheetPermitEditing.Checked then
  begin
    History.Add(' - разрешить изменение в режиме работы с данными ' +
      IIF(cbSheetPermitEditing.Checked, 'включено', 'выключено'));
    SheetInterface.PermitEditing := cbSheetPermitEditing.Checked;
    result := true;
  end;

  if not result then
    History.Delete(History.Count - 1);
end;

procedure TfmSheetComponentEditor.EditSheetInterface;
var
  HeadingEndList: TStringList;
begin
  tsSheetProperties.TabVisible := true;
  tsProcessing.TabVisible := true;
  tsEvents.TabVisible := true;
  tsTask.TabVisible := true;
  tsHistory.TabVisible := true;
  tsMSWord.TabVisible := true;

  tsTotalProperties.TabVisible := false;
  tsTotalFormulas.TabVisible := false;
  tsStyles.TabVisible := false;
  tsFilterScope.TabVisible := false;
  tsSummaries.TabVisible := false;
  tsSingleCellProperties.TabVisible := false;
  tsAxisCollectionProperties.TabVisible := false;
  tsAxisElementProperties.TabVisible := false;
  tsTotalCollectionProperties.TabVisible := false;
  tsFormat.TabVisible := false;
  tsFilterProperties.TabVisible := false;
  tsFilterCollectionProperties.TabVisible := false;
  tsSingleCellsCollectionProperties.TabVisible := false;
  tsGrandSummary.TabVisible := false;
  tsDataMembers.TabVisible := false;

  if not FPreviousTab.TabVisible then
    pcEditor.ActivePage := tsSheetProperties;

  MayBeEdited := SheetInterface.MayBeEdited;
  EnablePages(MayBeEdited);

  cbShowFilters.Checked := SheetInterface.IsDisplayFilters;
  cbShowColumns.Checked := SheetInterface.IsDisplayColumns;
  cbShowColumnsTitles.Checked := SheetInterface.IsDisplayColumnsTitles;
  cbShowRowsTitles.Checked := SheetInterface.IsDisplayRowsTitles;
  cbShowTotalsTitles.Checked := SheetInterface.IsDisplayTotalsTitles;
  cbShowSheetInfo.Checked := SheetInterface.IsDisplaySheetInfo;
  cbPrintable.Checked := SheetInterface.PrintableStyle;
  cbCommentStructuralCell.Checked := SheetInterface.IsDisplayCommentStructuralCell;
  cbCommentDataCell.Checked := SheetInterface.IsDisplayCommentDataCell;

  case SheetInterface.TableProcessingMode of
    tpmNormal: rbNormal.Checked := true;
    tpmLarge: rbLarge.Checked := true;
    tpmHuge: rbHuge.Checked := true;
  end;

  {заголовок таблицы}
  case SheetInterface.SheetHeading.Type_ of
    htNoDefine: rbHeadingNoDefine.Checked := true;
    htTableArea: rbHeadingThisTableArea.Checked := true;
    htCustomArea: rbHeadingThisCustomArea.Checked := true;
  end;

  HeadingEndList := GetHeadingEndList(SheetInterface);
  try
    cbHeadingEnd.Items := HeadingEndList;
    cbHeadingEnd.ItemIndex := cbHeadingEnd.Items.IndexOf(
      PSObjectCaption[SheetInterface.SheetHeading.End_]);
    if (cbHeadingEnd.ItemIndex = -1) then
      cbHeadingEnd.ItemIndex := cbHeadingEnd.Items.Count - 1;
    edHeadingAddress.Text := SheetInterface.SheetHeading.Address;
  finally
    FreeStringList(HeadingEndList);
  end;

  {Флаг разрешения NonEmptyCrossJoin}
  cbAllowNECJ.Checked := SheetInterface.AllowNECJ;
  cbSheetPermitEditing.Checked := SheetInterface.PermitEditing;
  cbSheetPermitEditing.Enabled := SheetInterface.InConstructionMode
    and MayBeEdited;
end;

procedure TfmSheetComponentEditor.SetupTaskInfo;
var
  tmpStr: string;
begin
  with cmbSheetType do
  begin
    Items.Clear;
    Items.Add('Расчетный лист');
    Items.Add('Форма ввода');
    try
      FOldSheetType := SheetInterface.Environment.SheetType;
    except
      FOldSheetType := -1;
    end;
    // "отчет" возможен, если нет показателей для записи
    if not SheetInterface.CheckWorkbookForResults then
      Items.Add('Отчет')
    else
      if FOldSheetType >= 2 then
        dec(FOldSheetType);
    Items.Add('Форма сбора данных');
    ItemIndex := FOldSheetType;
  end;
  with memProperties do
  begin
    Lines.Clear;
    try
      tmpStr := SheetInterface.Environment.TaskName;
    except
      tmpStr := ''
    end;
    Lines.Add('Наименование задачи: ' + tmpStr + #13#10);
    try
      tmpStr := SheetInterface.Environment.TaskId;
    except
      tmpStr := ''
    end;
    Lines.Add('Идентификатор задачи: ' + tmpStr + #13#10);
    try
      tmpStr := SheetInterface.Environment.DocumentName;
    except
      tmpStr := ''
    end;
    Lines.Add('Наименование документа: ' + tmpStr);
    try
      tmpStr := SheetInterface.Environment.DocumentId;
    except
      tmpStr := ''
    end;
    Lines.Add('Идентификатор документа: ' + tmpStr + #13#10);
    try
      tmpStr := SheetInterface.Environment.Owner;
    except
      tmpStr := ''
    end;
    Lines.Add('Исполнитель: ' + tmpStr + #13#10);
  end;
end;


procedure TfmSheetComponentEditor.btnFontClick(Sender: TObject);
begin
  EditFont(RegularFontOptions, pnFontSample);
end;

procedure TfmSheetComponentEditor.SetupEvents;
var
  MacrosList: TStringList;
  ExcelBook: ExcelWorkBook;
begin
  ExcelBook := (ExcelSheet.Parent as ExcelWorkBook);
  mMarkerImpossiblyMacros.Visible := not Events.GetMacrosList(ExcelBook,
    MacrosList);
  Events.LoadEvents(ExcelSheet);
  try
    {События до обновления}
    cbBeforeRefresh.Checked := Events.Items[enBeforeRefresh].Enabled;
    cmbBeforeRefresh.Items := MacrosList;
    cmbBeforeRefresh.Text := Events.Items[enBeforeRefresh].FulfillingMacrosName;
    {Событие после обновления}
    cbAfterRefresh.Checked := Events.Items[enAfterRefresh].Enabled;
    cmbAfterRefresh.Items := MacrosList;
    cmbAfterRefresh.Text := Events.Items[enAfterRefresh].FulfillingMacrosName;
    {Событие до обратной записи}
    cbBeforeWriteBack.Checked := Events.Items[enBeforeWriteBack].Enabled;
    cmbBeforeWriteBack.Items := MacrosList;
    cmbBeforeWriteBack.Text := Events.Items[enBeforeWriteBack].FulfillingMacrosName;
    {Событие после обратной записи}
    cbAfterWriteBack.Checked := Events.Items[enAfterWriteBack].Enabled;
    cmbAfterWriteBack.Items := MacrosList;
    cmbAfterWriteBack.Text := Events.Items[enAfterWriteBack].FulfillingMacrosName;
  finally
    FreeStringList(MacrosList);
  end;
end;

function TfmSheetComponentEditor.ApplyEventsChanges: boolean;
begin
  result := false;

  if Events.Items[enBeforeRefresh].Enabled <> cbBeforeRefresh.Checked then
  begin
    History.Add(Format(' - событие "до обновления" %s',
      [IIF(cbBeforeRefresh.Checked, 'включено', 'выключено')]));
    result := true;
    Events.Items[enBeforeRefresh].Enabled := cbBeforeRefresh.Checked;
  end;
  if Events.Items[enBeforeRefresh].FulfillingMacrosName <> cmbBeforeRefresh.Text then
  begin
    History.Add(Format(' - событию "до обновления" назначен обработчик %s',
      [cmbBeforeRefresh.Text]));
    result := true;
    Events.Items[enBeforeRefresh].FulfillingMacrosName := cmbBeforeRefresh.Text;
  end;

  if Events.Items[enAfterRefresh].Enabled <> cbAfterRefresh.Checked then
  begin
    History.Add(Format(' - событие "после обновления" %s',
      [IIF(cbAfterRefresh.Checked, 'включено', 'выключено')]));
    result := true;
    Events.Items[enAfterRefresh].Enabled := cbAfterRefresh.Checked;
  end;
  if Events.Items[enAfterRefresh].FulfillingMacrosName <> cmbAfterRefresh.Text then
  begin
    History.Add(Format(' - событию "после обновления" назначен обработчик %s',
      [cmbAfterRefresh.Text]));
    result := true;
    Events.Items[enAfterRefresh].FulfillingMacrosName := cmbAfterRefresh.Text;
  end;

  if Events.Items[enBeforeWriteBack].Enabled <> cbBeforeWriteBack.Checked then
  begin
    History.Add(Format(' - событие "до обратной записи" %s',
      [IIF(cbBeforeWriteBack.Checked, 'включено', 'выключено')]));
    result := true;
    Events.Items[enBeforeWriteBack].Enabled := cbBeforeWriteBack.Checked;
  end;
  if Events.Items[enBeforeWriteBack].FulfillingMacrosName <> cmbBeforeWriteBack.Text then
  begin
    History.Add(Format(' - событию "до обратной записи" назначен обработчик %s',
      [cmbBeforeWriteBack.Text]));
    result := true;
    Events.Items[enBeforeWriteBack].FulfillingMacrosName := cmbBeforeWriteBack.Text;
  end;

  if Events.Items[enAfterWriteBack].Enabled <> cbAfterWriteBack.Checked then
  begin
    History.Add(Format(' - событие "после обратной записи" %s',
      [IIF(cbAfterWriteBack.Checked, 'включено', 'выключено')]));
    result := true;
    Events.Items[enAfterWriteBack].Enabled := cbAfterWriteBack.Checked;
  end;
  if Events.Items[enAfterWriteBack].FulfillingMacrosName <> cmbAfterWriteBack.Text then
  begin
    History.Add(Format(' - событию "после обратной записи" назначен обработчик %s',
      [cmbAfterWriteBack.Text]));
    result := true;
    Events.Items[enAfterWriteBack].FulfillingMacrosName := cmbAfterWriteBack.Text;
  end;
end;

procedure TfmSheetComponentEditor.cbAllCapitalsClick(Sender: TObject);
begin
  AllCapitalsClick(cbAllCapitals, pnFontSample);
end;

procedure TfmSheetComponentEditor.tvSheetStartDrag(Sender: TObject;
  var DragObject: TDragObject);
var
  Point: TPoint;
  NodeType: TComponentNodeType;
begin
  Point := tvSheet.ScreenToClient(Mouse.CursorPos);
  DraggingNode := tvSheet.GetNodeAt(Point.X, Point.Y) as TBasicCheckTreeNode;
  if not Assigned(DraggingNode) then
    exit;
  NodeType := TComponentNodeType(DraggingNode.NodeType);
  if (NodeType = cntRow) and SheetInterface.Rows.MayBeEdited
    or (NodeType = cntColumn) and SheetInterface.Columns.MayBeEdited
    or (NodeType = cntFilter) and SheetInterface.Filters.MayBeEdited
    or (NodeType = cntTotal) and SheetInterface.Totals.MayBeEdited
    or (NodeType = cntSingleCell) and SheetInterface.SingleCells.MayBeEdited then
    exit;
  CancelDrag;
  DraggingNode := nil;
end;

procedure TfmSheetComponentEditor.tvSheetDragOver(Sender, Source: TObject;
  X, Y: Integer; State: TDragState; var Accept: Boolean);
var
  DestNode: TBasicCheckTreeNode;
  SourceNodeType, DestNodeType: TComponentNodeType;
begin
  DestNode := tvSheet.GetNodeAt(X, Y) as TBasicCheckTreeNode;
  if not Assigned(DestNode) then
    exit;
  SourceNodeType := TComponentNodeType(DraggingNode.NodeType);
  DestNodeType := TComponentNodeType(DestNode.NodeType);

  Accept := (DraggingNode <> DestNode) and
    ((SourceNodeType = cntRow) and
    (DestNodeType in [cntColumnCollection, cntColumn, cntFilterCollection, cntFilter, cntRow])) or
    ((SourceNodeType = cntColumn) and
    (DestNodeType in [cntRowCollection, cntRow, cntFilterCollection, cntFilter, cntColumn])) or
    ((SourceNodeType = cntFilter) and
    (DestNodeType in [cntRowCollection, cntRow, cntColumnCollection, cntColumn, cntFilter])) or
    ((SourceNodeType = cntTotal) and (DestNodeType = cntTotal)) or
    ((SourceNodeType = cntSingleCell) and (DestNodeType = cntSingleCell));

  if Accept then
    if TObject(DestNode.Data) is TSheetCollection then
      Accept := TSheetCollection(DestNode.Data).MayBeEdited
    else
      if TObject(DestNode.Data) is TSheetElement then
        Accept := TSheetElement(DestNode.Data).MayBeEdited;
end;

procedure TfmSheetComponentEditor.tvSheetDragDrop(Sender, Source: TObject;
  X, Y: Integer);
var
  DestNode: TBasicCheckTreeNode;
  SourceNodeType, DestNodeType: TComponentNodeType;
  ObjType, NewObjType, UID, ObjDescription, DestDescription: string;
  i, DestNodeIndex, CollectionNodeIndex: integer;
  IsInternalSwap: boolean;
  Collection: TSheetCollection;
begin
  {получение информации об области назначения}
  DestNode := tvSheet.GetNodeAt(X, Y) as TBasicCheckTreeNode;
  if not Assigned(DestNode) then
    exit;
  DestNodeType := TComponentNodeType(DestNode.NodeType);
  {получение информации о перетаскиваемом объекте}
  SourceNodeType := TComponentNodeType(DraggingNode.NodeType);

  if (DestNodeType = cntRow) and (SourceNodeType in [cntColumn, cntFilter]) then
    DestNodeType := cntRowCollection;
  if (DestNodeType = cntColumn) and (SourceNodeType in [cntRow, cntFilter]) then
    DestNodeType := cntColumnCollection;
  if (DestNodeType = cntFilter) and (SourceNodeType in [cntRow, cntColumn]) then
    DestNodeType := cntFilterCollection;

  IsInternalSwap := false;
  case DestNodeType of
    cntRowCollection:
      begin
        NewObjType := sntRowDimension;
        DestDescription := 'область строк';
      end;
    cntColumnCollection:
      begin
        NewObjType := sntColumnDimension;
        DestDescription := 'область столбцов';
      end;
    cntFilterCollection:
      begin
        NewObjType := sntFilter;
        DestDescription := 'область фильтров';
      end;
    else
      IsInternalSwap := true;
  end;


  if IsInternalSwap then
  begin
    if not ShowQuestion('Порядок следования элементов будет изменен. Продолжить?') then
      exit;
    {запомним положение узла, чтобы потом выделить его}
    DestNodeIndex := DestNode.Index;
    CollectionNodeIndex := DestNode.Parent.AbsoluteIndex;
    {поменяем элементы местами}
    Collection := TSheetElement(DestNode.Data).Owner;
    Collection.Move(DraggingNode.Index, DestNode.Index);
    {перестроим дерево и выделим перемещенный элемент}
    BuildSheetTree;
    tvSheet.Selected := tvSheet.Items[CollectionNodeIndex].Item[DestNodeIndex];
  end
  else
  begin
    case SourceNodeType of
      cntRow: ObjType := sntRowDimension;
      cntColumn: ObjType := sntColumnDimension;
      cntFilter: ObjType := sntFilter;
      else
        exit;
    end;
    with TSheetElement(DraggingNode.Data) do
      ObjDescription := GetObjectTypeStr + ' "' + GetElementCaption + '"';

    if not ShowQuestion(ObjDescription + ' будет перемещен в ' + DestDescription +
      '. Продолжить?') then
      exit;

    {внесение записей в историю и перемещение объекта}
    btnOKClick(Self);
    UID := TSheetElement(DraggingNode.Data).UniqueID;
    if not SheetInterface.MoveElement(ObjType, NewObjType, UID) then
      exit;
    if NewObjType = sntFilter then
      SheetInterface.Filters.Refresh(true);
    SheetInterface.SetUpMeasuresPosition;
    History.Add(ObjDescription + ' перемещен в ' + DestDescription);
    {перестроение дерева и выделение перемешенного объекта}
    BuildSheetTree;
    for i := 0 to tvSheet.Items.Count - 1 do
      if TComponentNodeType(TBasicCheckTreeNode(tvSheet.Items[i]).NodeType) = DestNodeType then
      begin
        DestNode := TBasicCheckTreeNode(tvSheet.Items[i]);
        tvSheet.Selected := DestNode.Item[DestNode.Count - 1];
      end;
  end;

  DraggingNode := nil;
end;

procedure TfmSheetComponentEditor.rbHeadingNoDefineClick(Sender: TObject);
begin
  lbStart.Enabled := false;
  lbEnd.Enabled := false;
  lbAddress.Enabled := false;
  cbHeadingEnd.Enabled := false;
  edHeadingAddress.Enabled := false;
  btDefineHeadingAddress.Enabled := false;
end;

procedure TfmSheetComponentEditor.rbHeadingThisTableAreaClick(
  Sender: TObject);
begin
  lbStart.Enabled := true;
  lbEnd.Enabled := true;
  cbHeadingEnd.Enabled := true;

  lbAddress.Enabled := false;
  edHeadingAddress.Enabled := false;
  btDefineHeadingAddress.Enabled := false;
end;

procedure TfmSheetComponentEditor.rbHeadingThisCustomAreaClick(
  Sender: TObject);
begin
  lbAddress.Enabled := true;
  edHeadingAddress.Enabled := true;
  btDefineHeadingAddress.Enabled := true;

  lbStart.Enabled := false;
  lbEnd.Enabled := false;
  cbHeadingEnd.Enabled := false;
end;

function TfmSheetComponentEditor.GetHeadingRange: ExcelRange;
const
  APrompt = 'Укажите координаты заголовка таблицы';
  ErrorTxt = '';
var
  v: Variant;
  res: hresult;
begin
  result := nil;
  while true do
  begin
    v := ExcelSheet.Application.InputBox(APrompt, 'Координаты заголовка', EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, 8, GetUserDefaultLCID);
    if not Assigned(TVarData(v).VDispatch) then
      exit;
    res := IDispatch(TVarData(v).VDispatch).QueryInterface(DIID_ExcelRange, result);
    if (res = S_OK) then
      exit;
  end;
end;

procedure TfmSheetComponentEditor.btDefineHeadingAddressClick(
  Sender: TObject);
var
  eRange: ExcelRange;
begin
  Hide;
  try
    eRange := GetHeadingRange;
    ExcelSheet.Application.Set_Interactive(GetUserDefaultLCID, false);
    if not Assigned(eRange) then
      exit;
    edHeadingAddress.Text :=  eRange.AddressLocal[true, true, xlA1, false, varNull];
  finally
    Show;
  end;
end;

function TfmSheetComponentEditor.GetTotalCell(Total: TSheetTotalInterface): ExcelRange;
const
  APrompt = 'Укажите ячейку выбранного показателя, содержащую формулу, которая будет считаться типовой';
var
  v: Variant;
  res: hresult;
  ErrorText, Formula: string;
  IsAllowed: boolean;
  SelectedTotal: TSheetTotalInterface;
  SectionIndex: integer;
begin
  result := nil;

  while true do
  begin
    v := ExcelSheet.Application.InputBox(APrompt,
      'Координаты ячейки', EmptyParam, EmptyParam, EmptyParam, EmptyParam,
      EmptyParam, 8, GetUserDefaultLCID);
    if not Assigned(TVarData(v).VDispatch) then
      exit;
    res := IDispatch(TVarData(v).VDispatch).QueryInterface(DIID_ExcelRange, result);
    if (res = S_OK) then
    begin
      IsAllowed := IsTypeFormulaAllowed(result, ErrorText);
      if IsAllowed then
      begin
        SelectedTotal := SheetInterface.Totals.FindByColumn(result.Column, SectionIndex);
        if Assigned(SelectedTotal) then
          if (SelectedTotal.Alias = Total.Alias) then
          begin
            if GetCellFormula(SheetInterface.ExcelSheet, result.Row, result.Column,
              Formula) then
              exit
            else
              ErrorText := 'Некорректная формула.' + #10 + 'Формула: ' + Formula;
          end
          else
            ErrorText := 'Выбрана ячейка другого показателя';
      end;

      if (ErrorText <> '') then
      begin
        ShowError(ErrorText);
        result := nil;
      end;
    end
    else
      exit;
  end;
end;

procedure TfmSheetComponentEditor.btAssignTypeFormulaClick(
  Sender: TObject);
var
  eRange: ExcelRange;
  TreeNode: TBasicCheckTreeNode;
  Total: TSheetTotalInterface;
  TypeFormula: TTypeFormula;
  Enabled: boolean;
  ErrorText: string;
begin
  if not Assigned(tvSheet.Selected) then
    exit;
  TreeNode := TBasicCheckTreeNode(tvSheet.Selected);
  Total := TreeNode.Data;
  if not Assigned(Total) then
    exit;
  Hide;
  try
    eRange := GetTotalCell(Total);
    ExcelSheet.Application.Set_Interactive(GetUserDefaultLCID, false);
    if not Assigned(eRange) then
      exit;
    Enabled := Total.TypeFormula.Enabled;
    TypeFormula := SheetInterface.GetTypeFormula(Total, eRange.Row, eRange.Column);
    if not Assigned(TypeFormula) or not TypeFormula.IsValid(ErrorText) then
    begin
      ShowError(ermTypeFormulaFault + ErrorText);
      FreeAndNil(TypeFormula);
      exit;
    end;
    Total.TypeFormula := TypeFormula;
    Total.TypeFormula.Enabled := Enabled;
    mUserTypeFormula.Text := Total.TypeFormula.UserFormula;
    cbIsUseTypeFormula.Checked := true;
  finally
    Show;
  end;
end;

procedure TfmSheetComponentEditor.cbFormatTotalsAreaClick(Sender: TObject);
begin
  rbFormatByRows.Enabled := cbFormatTotalsArea.Checked and MayBeEdited;
  rbFormatByColumns.Enabled := cbFormatTotalsArea.Checked and MayBeEdited;
end;

procedure TfmSheetComponentEditor.cmbTotalFormatClick(Sender: TObject);
begin
  // Разрядность - только для числовых форматов
  seDigits.Enabled :=
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtCurrency, fmtPercent, fmtNumber]);

  // Для нечисловых показателей жестко задаем итоги не вычислять
  if TMeasureFormat(cmbTotalFormat.ItemIndex) in [fmtText, fmtBoolean] then
    cmbTotalFunction.ItemIndex := 4;

  cbSummariesByVisibleClick(nil);
end;

procedure TfmSheetComponentEditor.EnablePages(Value: boolean);
var
  i: integer;
  tmpStr: string;
begin
  for i := 0 to pcEditor.PageCount - 1 do
  begin
    pcEditor.Pages[i].Enabled := Value;
    EnableChildControls(pcEditor.Pages[i], Value);
  end;
  try
    tmpStr := GetWBCustomPropertyValue(
      ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspTaskID);
  except
    tmpStr := ''
  end;
  btnTaskDisconnect.Enabled := not Assigned(SheetInterface.TaskContext) and (tmpStr <> '');
end;

procedure TfmSheetComponentEditor.cmbSheetTypeClick(Sender: TObject);
begin
  if (cmbSheetType.Items.Count = 3) and (cmbSheetType.ItemIndex = 2) or
    (cmbSheetType.Items.Count > 3) and (cmbSheetType.ItemIndex = 3) then
    ShowWarning('Вы выбрали тип документа "форма сбора данных". После того, ' +
      'как данная настройка вступит в силу, лист будет переведен в автономный ' +
      'режим работы, а большая часть функций отключена. Данная операция ' +
      'является необратимой, поэтому убедитесь, что вся работа по конструированию закончена.');
end;

function TfmSheetComponentEditor.FindElementNode(Id: string): TTreeNode;
var
  i: integer;
  Element: TSheetElement;
  Node: TBasicCheckTreeNode;
begin
  result := tvSheet.Items[0];
  if Id = '' then
    exit;
  for i := 1 to tvSheet.Items.Count - 1 do
  begin
    Node := TBasicCheckTreeNode(tvSheet.Items[i]);
    if not (TComponentNodeType(Node.NodeType) in ElementTypes) then
      continue;
    Element := Node.Data;
    if Element.UniqueID = Id then
    begin
      result := Node;
      exit;
    end;
  end;
end;

procedure TfmSheetComponentEditor.tvSheetKeyUp(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if Key = VK_DELETE then
    DeleteElementHandler;
end;

procedure TfmSheetComponentEditor.DeleteElementHandler;
var
  Element: TSheetElement;
  Collection: TSheetCollection;
  NodeType: TComponentNodeType;
  NodeIndex: integer;
begin
  if not Assigned(tvSheet.Selected) then
    exit;
  NodeType := TComponentNodeType(TBasicCheckTreeNode(tvSheet.Selected).NodeType);
  if not (NodeType in ElementTypes) then
    exit;
  Element := TSheetElement(tvSheet.Selected.Data);

  if not Element.MayBeDeleted then
    exit;

  History.Add(Format('Удален %s "%s"', [AnsiLowerCase(Element.GetObjectTypeStr),
    Element.GetElementCaption]));
  Collection := Element.Owner;
  NodeIndex := tvSheet.Selected.AbsoluteIndex;
  {}
  if Element.GetObjectType = wsoSingleCell then
    DeletedCellsNames.Add(Element.ExcelName);
  Collection.Delete(Element.GetSelfIndex);
  BuildSheetTree;

  if NodeIndex < tvSheet.Items.Count then
    if TComponentNodeType(TBasicCheckTreeNode(tvSheet.Items[NodeIndex]).NodeType) <> NodeType then
      tvSheet.Selected := tvSheet.Items[NodeIndex - 1]
    else
      tvSheet.Selected := tvSheet.Items[NodeIndex]
  else
    tvSheet.Selected := tvSheet.Items[NodeIndex - 1];
end;

function TfmSheetComponentEditor.ConvertBuiltinStyleName(
  AName: string): string;
begin
  result := AName;
  AName := AnsiUpperCase(AName);
  if AName = 'NORMAL' then
  begin
    result := 'Обычный';
    exit;
  end;
  if AName = 'ОБЫЧНЫЙ' then
  begin
    result := 'Normal';
    exit;
  end;

  if AName = 'PERCENT' then
  begin
    result := 'Процентный';
    exit;
  end;
  if AName = 'ПРОЦЕНТНЫЙ' then
  begin
    result := 'Percent';
    exit;
  end;

  if AName = 'COMMA' then
  begin
    result := 'Финансовый';
    exit;
  end;
  if AName = 'ФИНАНСОВЫЙ' then
  begin
    result := 'Comma';
    exit;
  end;

  if AName = 'COMMA [0]' then
  begin
    result := 'Финансовый [0]';
    exit;
  end;
  if AName = 'ФИНАНСОВЫЙ [0]' then
  begin
    result := 'Comma [0]';
    exit;
  end;

  if AName = 'CURRENCY' then
  begin
    result := 'Денежный';
    exit;
  end;
  if AName = 'ДЕНЕЖНЫЙ' then
  begin
    result := 'Currency';
    exit;
  end;

  if AName = 'CURRENCY [0]' then
  begin
    result := 'Денежный [0]';
    exit;
  end;
  if AName = 'ДЕНЕЖНЫЙ [0]' then
  begin
    result := 'Currency [0]';
    exit;
  end;
end;

procedure TfmSheetComponentEditor.ClearDeletedCells;
var
  i: integer;
  ERange: ExcelRange;
  EName: ExcelXP.Name;
  WasProtected: boolean;
begin
  WasProtected := IsSheetProtected(ExcelSheet);
  if WasProtected then
    SetSheetProtection(ExcelSheet, false);
  for i := 0 to DeletedCellsNames.Count - 1 do
  begin
    ERange := GetRangeByName(ExcelSheet, DeletedCellsNames[i]);
    if Assigned(ERange) then
      ERange.Clear;
    EName := GetNameObject(ExcelSheet, DeletedCellsNames[i]);
    if Assigned(EName) then
      EName.Delete;
  end;
  if WasProtected then
    SetSheetProtection(ExcelSheet, true);
end;

procedure TfmSheetComponentEditor.btnTaskDisconnectClick(Sender: TObject);
begin
  if not ShowQuestion('Открепление разорвет связь с задачей ' +
    'и сделает невозможной обратную запись. Продолжить?') then
    exit;
  FNeedClearTaskInfo := true;
  with memProperties do
  begin
    Lines.Clear;
    Lines.Add('Наименование задачи: ' + #13#10);
    Lines.Add('Идентификатор задачи: ' + #13#10);
    Lines.Add('Наименование документа: ');
    Lines.Add('Идентификатор документа: ' + #13#10);
    Lines.Add('Исполнитель: ' + #13#10);
  end;
  btnTaskDisconnect.Enabled := false;
end;

procedure TfmSheetComponentEditor.GSRadioClick(Sender: TObject);
begin
  edGSTitle.Enabled := not rbGSNone.Checked and MayBeEdited;
  cbGSAllCapitals.Enabled := edGSTitle.Enabled;
  btnGSFont.Enabled := edGSTitle.Enabled;
  if rbGSNone.Checked then
    GSDeployment := idNone
  else
    if rbGSTop.Checked then
      GSDeployment := idTop
    else
      GSDeployment := idBottom;
end;

function TfmSheetComponentEditor.ApplySummaryChanges(
  SummaryOptions: TSummaryOptions; IsGrandSummary: boolean): boolean;
var
  tmpStr: string;
  NewDeployment: TItemDeployment;
  NewTitle: string;
begin
  result := false;
  if IsGrandSummary then
    NewDeployment := GSDeployment
  else
    NewDeployment := SummaryDeployment;
  NewTitle := IIF(IsGrandSummary, edGSTitle.Text, edSummaryTitle.Text);

  with SummaryOptions do
  begin
    if Deployment <> NewDeployment then
    begin
      case SummaryDeployment of
        idNone: tmpStr := '"не размещать"';
        idTop: tmpStr := '"сверху"';
        idBottom: tmpStr := '"снизу"';
      end;
      if IsGrandSummary then
        History.Add(Format('- размещение общего итога изменено на %s', [tmpStr]))
      else
        History.Add(Format('- размещение итогов по элементам изменено на %s', [tmpStr]));
      Deployment := NewDeployment;
      result := true;
    end;

    if Title <> NewTitle then
    begin
      if IsGrandSummary then
        History.Add(Format(' - наименование общего итога изменено на %s', [NewTitle]))
      else
        History.Add(Format(' - наименование итогов изменено на "%s"', [NewTitle]));
      Title := NewTitle;
      result := true;
    end;

    if IsGrandSummary then
    begin
      if not FontOptions.IsEqualTo(GSFontOptions) or (AllCapitals <> cbGSAllCapitals.Checked) then
      begin
        History.Add(' - изменены настройки формата общего итога');
        FontOptions.CopyFrom(GSFontOptions);
        AllCapitals := cbGSAllCapitals.Checked;
        result := true;
      end;
    end
    else
    begin
      if not FontOptions.IsEqualTo(SummaryFontOptions) or
        (AllCapitals <> cbSummaryAllCapitals.Checked) then
      begin
        History.Add(' - изменены настройки формата итогов');
        FontOptions.CopyFrom(SummaryFontOptions);
        AllCapitals := cbSummaryAllCapitals.Checked;
        result := true;
      end;
    end;
  end;
end;

procedure TfmSheetComponentEditor.SummaryRadioClick(Sender: TObject);
begin
  edSummaryTitle.Enabled := not rbSummaryNone.Checked and MayBeEdited;
  cbSummaryAllCapitals.Enabled := edSummaryTitle.Enabled;
  btnSummaryFont.Enabled := edSummaryTitle.Enabled;
  cbSummaryOptimization.Enabled := edSummaryTitle.Enabled;
  if rbSummaryNone.Checked then
    SummaryDeployment := idNone
  else
    if rbSummaryTop.Checked then
      SummaryDeployment := idTop
    else
      SummaryDeployment := idBottom;
end;

procedure TfmSheetComponentEditor.EditFont(var FontOptions: TFontOptions; SamplePanel: TPanel);
var
  dlgFont: TFontDialog;
begin
  if not Assigned(tvSheet.Selected) then
    exit;
  dlgFont := TFontDialog.Create(Self);
  dlgFont.Font.Charset := ANSI_CHARSET;

  try
    FontOptions.CopyToFont(dlgFont.Font);

    if dlgFont.Execute then
    with FontOptions do
    begin
      Bold := fsBold in dlgFont.Font.Style;
      Italic := fsItalic in dlgFont.Font.Style;
      Underline := fsUnderline in dlgFont.Font.Style;
      Strikethrough := fsStrikeout in dlgFont.Font.Style;
      Name := dlgFont.Font.Name;
      Size := dlgFont.Font.Size;
      SamplePanel.Font := dlgFont.Font
    end;
  finally
    dlgFont.Free;
  end;
end;

procedure TfmSheetComponentEditor.SetupFontOptions(RegularFont, SummaryFont, GSFont: TFontOptions);
begin
  if Assigned(RegularFont) then
  begin
    if not Assigned(RegularFontOptions) then
      RegularFontOptions := TFontOptions.Create;
    RegularFontOptions.CopyFrom(RegularFont);
    RegularFontOptions.CopyToFont(pnFontSample.Font);
  end;

  if Assigned(SummaryFont) then
  begin
    if not Assigned(SummaryFontOptions) then
      SummaryFontOptions := TFontOptions.Create;
    SummaryFontOptions.CopyFrom(SummaryFont);
    SummaryFontOptions.CopyToFont(pnSummaryFontSample.Font);
  end;

  if Assigned(GSFont) then
  begin
    if not Assigned(GSFontOptions) then
      GSFontOptions := TFontOptions.Create;
    GSFontOptions.CopyFrom(GSFont);
    GSFontOptions.CopyToFont(pnGSFontSample.Font);
  end;
end;

procedure TfmSheetComponentEditor.btnGSFontClick(Sender: TObject);
begin
  EditFont(GSFontOptions, pnGSFontSample);
end;

procedure TfmSheetComponentEditor.btnSummaryFontClick(Sender: TObject);
begin
  EditFont(SummaryFontOptions, pnSummaryFontSample);
end;

procedure TfmSheetComponentEditor.AllCapitalsClick(CheckBox: TCheckBox; Panel: TPanel);
begin
  if CheckBox.Checked then
    Panel.Caption := 'ОБРАЗЕЦ'
  else
    Panel.Caption := 'Образец';
end;

procedure TfmSheetComponentEditor.cbSummaryAllCapitalsClick(
  Sender: TObject);
begin
  AllCapitalsClick(cbSummaryAllCapitals, pnSummaryFontSample);
end;

procedure TfmSheetComponentEditor.cbGSAllCapitalsClick(Sender: TObject);
begin
  AllCapitalsClick(cbGSAllCapitals, pnGSFontSample);
end;

procedure TfmSheetComponentEditor.cbUseDMTitleClick(Sender: TObject);
begin
  edDMTitle.Enabled := cbUseDMTitle.Checked and MayBeEdited;
end;

procedure TfmSheetComponentEditor.DMRadioClick(Sender: TObject);
begin
  if rbDMNone.Checked then
    DataMembersDeployment := idNone
  else
    if rbDMTop.Checked then
      DataMembersDeployment := idTop
    else
      DataMembersDeployment := idBottom;
  cbUseDMTitle.Enabled := (DataMembersDeployment <> idNone) and MayBeEdited;
  edDMTitle.Enabled := cbUseDMTitle.Enabled;
end;

procedure TfmSheetComponentEditor.cmbTotalMultiplierClick(
  Sender: TObject);
begin
  rbMarkerToLeft.Enabled := (cmbTotalMultiplier.ItemIndex > 0) and MayBeEdited;
  rbMarkerToRight.Enabled := (cmbTotalMultiplier.ItemIndex > 0) and MayBeEdited;
  rbMarkerHidden.Enabled := (cmbTotalMultiplier.ItemIndex > 0) and MayBeEdited;
end;

procedure TfmSheetComponentEditor.cbIgnoreHierarchyClick(Sender: TObject);
begin
  //cbReverseOrder.Enabled := cbIgnoreHierarchy.Checked;
end;

procedure TfmSheetComponentEditor.cbBrokenClick(Sender: TObject);
begin
  //cbAxisReverseOrder.Enabled := cbBroken.Checked;
end;

end.

