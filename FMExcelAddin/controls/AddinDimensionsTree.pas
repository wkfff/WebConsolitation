unit AddinDimensionsTree;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, ComCtrls, StdCtrls, ImgList, uXMLCatalog, uSheetObjectModel,
  Buttons, uFMAddinGeneralUtils, uFMExcelAddInConst, uCheckTV2,
  uGlobalPlaningConst;

type

  {режимы поведения дерева измерений:
    dtmSingleCube - работа с измерениями только одного куба
    dtmSorted - измерения рассортированы по кубам, разрешено дублирование
    dtmFiltered - работа только с теми измерениями, по которым можно разложить
      показатели из коллекции Totals
    dtmParams - работа с параметрами задачи}
  TDimTreeMode = (dtmSingleCube, dtmSorted, dtmFiltered, dtmParams);
  TDimTreeState = set of TDimTreeMode;

  {варианты принадлежности выбранного в дереве узла}
  TDimTreeNodeType = (ntNone, ntCube, ntSemantics, ntDimension, ntHierarchy,
    ntParam, ntParamDetail, ntMeasure, ntCalcMeasure);

  TDimTreeImageIndexes = array[TDimTreeNodeType] of integer;

        (*
  TDimTree = class(TCustomPanel)
  private
    {дерево с выводом чекбоксов}
    FTreeView: TBasicCheckTreeView;
    {панель для управляющих кнопок}
    FBtnPanel: TPanel;
    btnSort, btnFilter, btnParams: TSpeedButton;
    {ссылка на список изображений}
    FImages: TCustomImageList;
    {индексы изображений для узлов соответствующих типов}
    FImageIndexes: TDimTreeImageIndexes;
    {признак состояния загрузки дерева}
    FLoading: boolean;
    FOnChange: TTVChangedEvent;
    FOnChanging: TTVChangingEvent;
    {нужно ли показывать узел куба}
    FNeedShowCube: boolean;
    FState: TDimTreeState;
    FCube: TCube;
  protected
    function GetImageIndex(Selection: TDimTreeSelectionType): integer;
    procedure SetImageIndex(Selection: TDimTreeSelectionType; AValue: integer);
    function GetCube: TCube;
    procedure SetCube(ACube: TCube);
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    function Load: boolean;

    property ImageIndex[Selection: TDimTreeSelectionType]: integer
      read GetImageIndex write SetImageIndex;
    property Cube: TCube read GetCube write SetCube;
  published
    property Align;
    property Anchors;
    property BevelInner;
    property BevelOuter;
    property BevelWidth;
    property BorderWidth;
    property BorderStyle;
    property Color;
    property Enabled;
    property Visible;
    property OnCanResize;
    property OnResize;
    property OnChange: TTVChangedEvent read FOnChange write FOnChange;
    property OnChanging: TTVChangingEvent read FOnChanging write FOnChanging;
  end;*)

  {компонент - дерево измерений одного куба}
  TSingleCubeDimTree = class(TCustomPanel)
  private
    { Private declarations }
    //дерево измерений
    FTreeView: TBasicCheckTreeView;//TTreeView;
    FImages: TCustomImageList;
    FImageIndexes: array[TDimTreeNodeType] of integer;
    FCatalog: TXMLCatalog;
    FOnChange: TTVChangedEvent;
    FOnChanging: TTVChangingEvent;
    FOnNodeCheck: TNotifyEvent;
    //признак состояния загрузки дерева
    FLoading: boolean;
    FCube: TCube;
    {нужно ли показывать узел куба}
    FNeedShowCube: boolean;
    {нужно ли показывать чекбоксы на соответствующих уровнях}
    FShowCheckBoxes: array[TDimTreeNodeType] of boolean;
    //загружает в дерево одно измерение
    procedure LoadDimension(CubeNode: TTreeNode; Dimension: TDimension);
    function GetImageIndexes(Index: TDimTreeNodeType): integer;
    procedure SetImageIndexes(Index: TDimTreeNodeType; const Value: integer);
    function GetShowCheckboxes(Index: TDimTreeNodeType): boolean;
    procedure SetShowCheckBoxes(Index: TDimTreeNodeType;
      Value: boolean);
  protected
    { Protected declarations }
    function CanResize(var NewWidth, NewHeight: integer): boolean; override;
    function GetImages: TCustomImageList;
    procedure SetImages(Value: TCustomImageList);
    function GetCube: TCube;
    function GetDimension: TDimension;
    function GetHierarchy: THierarchy;
    procedure Change(Sender: TObject; Node: TTreeNode);
    procedure Changing(Sender: TObject; Node: TTreeNode;
      var AllowChange: Boolean);
    function GetSelectionType: TDimTreeNodeType; virtual;
    function GetSelectedNode: TBasicCheckTreeNode;
    function GetSelectedNodeAtLevel(Index: TDimTreeNodeType): TBasicCheckTreeNode;
    procedure DoNodeCheck(Node: TTreeNode);
    procedure OnTreeNodeCheck(Sender: Tobject);
    function GetHideSelection: boolean;
    procedure SetHideSelection(AValue: boolean);
    //function GetNodeType(Index: TDimTreeSelectionType): TNodeType;
    procedure ParseDimensionName(DimensionName: string; out Semantics, Dimension: string);
    function GetOutputDimensionName: string;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    //загрузка дерева
    function Load(ACube: TCube; NeedShowCube: boolean): boolean;
    function IsEmpty: boolean;
    function SetSelection(Dim: TDimension; Hier: THierarchy): boolean;
    procedure Clear;
    {находит узел по его кэпшену,
      если таких много, то возвращает первый попавшийся}
    function FindNodeByName(AName: string): TBasicCheckTreeNode; overload;
    function FindNodeByName(AName: string; AtLevel: integer): TBasicCheckTreeNode; overload;
    {находит узел по его кэпшену и кэпшену его родителя,
      если таких много, то возвращает первый попавшийся}
    function FindNodeBy2Names(AParentName, AName: string): TBasicCheckTreeNode;
    {ищет узел среди потомков указанного родителя}
    function FindNodeWithinParent(AParent: TBasicCheckTreeNode; AName: string): TBasicCheckTreeNode;
    property Images: TCustomImageList read GetImages write SetImages;
    property Catalog: TXMLCatalog read FCatalog write FCatalog;
    property Cube: TCube read GetCube;
    property Dimension: TDimension read GetDimension;
    property Hierarchy: THierarchy read GetHierarchy;
    property SelectionType: TDimTreeNodeType read GetSelectionType;
    property ImageIndexes[Index: TDimTreeNodeType]: integer read GetImageIndexes
      write SetImageIndexes;
    property ShowCheckBoxes[Index: TDimTreeNodeType]: boolean read
      GetShowCheckBoxes write SetShowCheckBoxes;
    {возвращает выбранный в данный момент узел, аналогично TTreeView.Selected}
    property SelectedNode: TBasicCheckTreeNode read GetSelectedNode;
    property SelectedNodeAtLevel[Index: TDimTreeNodeType]:
      TBasicCheckTreeNode read GetSelectedNodeAtLevel;
    property HideSelection: boolean read GetHideSelection write SetHideSelection;
    property OutputDimensionName: string read GetOutputDimensionName;
  published
    { Published declarations }
    property Align;
    property Anchors;
    property BevelInner;
    property BevelOuter;
    property BevelWidth;
    property BorderWidth;
    property BorderStyle;
    property Color;
    property Enabled;
    property Visible;
    property OnCanResize;
    property OnResize;
    property OnEnter;
    property OnChange: TTVChangedEvent read FOnChange write FOnChange;
    property OnChanging: TTVChangingEvent read FOnChanging write FOnChanging;
    property OnNodeCheck: TNotifyEvent read FOnNodeCheck
      write FOnNodeCheck;
    property TabOrder;
  end;

  TCommonDimTree = class(TSingleCubeDimTree)
  protected
    //режим представления: иерархическое по кубам или плоское
    FSorted: boolean;
    //фильтрация по мерам
    FilteredByMeasures: boolean;
    {ранее выбранные элементы}
    FOldCube: TCube;
    FOldDim: TDimension;
    FOldHier: THierarchy;

    {загрузка в режиме сортировки измерений по кубам}
    procedure LoadInCubesMode;
    {обычная загрузка}
    procedure LoadInNormalMode;
    procedure LoadDimension(CubeNode: TTreeNode; Dimension: TDimension); virtual;
    //выделяет в дереве ранее запомненную позицию
    procedure SetOldSelection;

    function IsFilterPassed(ADimension: TDimension; AHierarchy: Thierarchy): boolean; virtual;

    function GetCube: TCube;
    function GetDimension: TDimension;
    function GetHierarchy: THierarchy;
  public
    function Load: boolean;
    function IsEmpty: boolean;
    function SetSelection(Dim: TDimension; Hier: THierarchy): boolean;

    property Cube: TCube read GetCube;
    property Dimension: TDimension read GetDimension;
    property Hierarchy: THierarchy read GetHierarchy;
  end;


  TCubeDimTree = class(TSingleCubeDimTree)
  private
  protected
    function LoadCubes: boolean;
    function LoadCubeMeasures(Cube: TCube; CubeNode: TBasicCheckTreeNode): boolean;
    function LoadCubeDimensions(Cube: TCube; CubeNode: TBasicCheckTreeNode): boolean;
    function GetOnStartDrag: TStartDragEvent;
    procedure SetOnStartDrag(Event: TStartDragEvent);
    //function GetOnDragOver
  public
    //constructor Create(AOwner: TComponent); override;
    //destructor Destroy; override;
    function Load: boolean;
    function GetNodeAtCursor: TBasicCheckTreeNode;
  published
    property OnStartDrag: TStartDragEvent read GetOnStartDrag write SetOnStartDrag;
  end;


  {стандартное дерево измерений с возможностью фильтрации
    по выбранным в листе показателям}
  TAddinDimTree = class(TCommonDimTree)
  private
  protected
    procedure SetSorted(Value: boolean);
    procedure SetFiltered(Value: boolean);
  public
    function Load: boolean;
    property Sorted: boolean read FSorted write SetSorted;
    property Filtered: boolean read FilteredByMeasures write SetFiltered;
  end;


  TAddinDimensionsTree = class(TCommonDimTree)
  private
    { Private declarations }
    FSheetInterface: TSheetInterface;
    FilteredByParams: boolean;
    FToolBarPanel: TPanel; //панель с кнопками
    FToolBarVisible: boolean; //видимость панели с кнопками
    procedure BtnSortClick(Sender: TObject);
    procedure BtnFilterClick(Sender: TObject);
    {загружает коллекцию параметров листа, если выбран режим параметров}
    procedure LoadInParamsMode;
    procedure EnableButtons;
  protected
    function GetDimension: TDimension;
    function GetHierarchy: THierarchy;
    function GetSelectionType: TDimTreeNodeType; override;
    function GetParameter: TTaskParam;
    function GetSheetParam: TParamInterface;
    procedure SetToolBarVisible(AValue: boolean);
    {количество дочерних уровней у узла}
    function ChildrenLevelCount(Node: TTreeNode): integer;
    function IsFilterPassed(ADimension: TDimension; AHierarchy: Thierarchy): boolean; override;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    //загрузка дерева
    function Load: boolean;
    function IsEmpty: boolean;
    property Images: TCustomImageList read GetImages write SetImages;
    property Catalog: TXMLCatalog read FCatalog write FCatalog;
    property SheetInterface: TSheetInterface read FSheetInterface
      write FSheetInterface;
    property Cube: TCube read GetCube;
    property Dimension: TDimension read GetDimension;
    property Hierarchy: THierarchy read GetHierarchy;
    property Parameter: TTaskParam read GetParameter;
    property SheetParam: TParamInterface read GetSheetParam;
    property ToolBarVisible: boolean read FToolBarVisible write SetToolBarVisible;
  published
  end;

procedure Register;

{$R AddinDimensionsTree.dcr}

implementation


const
  HSpace = 8;
  VSpace = 16;
  LargeButtonWidth = 120;
  ButtonHeight = 20;
  capSortOn = 'Простой список';
  capSortOff = 'Список по кубам';
  capFilterOn = 'Все измерения';
  capFilterOff = 'Фильтр по мерам';

          (*
constructor TDimTree.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FState := [];
end;    *)




constructor TAddinDimensionsTree.Create(AOwner: TComponent);
var
  SpeedBtn: TSpeedButton;
begin
  inherited Create(AOwner);
  Parent := AOwner as TWinControl;
  FSorted := false;
  FilteredByMeasures := false;
  FSheetInterface := nil;

  {панель для управляющих кнопок}
  //Изначально тулбар видим - потом надо будет вынести в паблишед свойства
  FToolBarVisible := true;
  FToolBarPanel := TPanel.Create(Self);
  FToolBarPanel.Name := 'ButtonPanel';
  FToolBarPanel.Parent := Self;
  FToolBarPanel.Align := alRight;
  FToolBarPanel.Caption := '';
  FToolBarPanel.Width := 2 * HSpace + ButtonHeight;
  FToolBarPanel.BevelInner := bvNone;
  FToolBarPanel.BevelOuter := bvNone;

  {кнопки}
  SpeedBtn := TSpeedButton.Create(Self);
  SpeedBtn.Parent := FToolBarPanel;
  SpeedBtn.Name := 'btnSort';
  SpeedBtn.Anchors := [akRight, akTop];
  SpeedBtn.Width := ButtonHeight;
  SpeedBtn.Height := ButtonHeight;
  SpeedBtn.Top := 0;
  SpeedBtn.Left := HSpace;
  SpeedBtn.Glyph.LoadFromResourceName(HInstance, 'CUBE');
  SpeedBtn.GroupIndex := 1;
  SpeedBtn.AllowAllUp := true;
  SpeedBtn.Hint := 'Сортировка по кубам';
  SpeedBtn.ShowHint := true;
  SpeedBtn.OnClick := BtnSortClick;

  SpeedBtn := TSpeedButton.Create(Self);
  SpeedBtn.Parent := FToolBarPanel;
  SpeedBtn.Name := 'btnFilter';
  SpeedBtn.Anchors := [akRight, akTop];
  SpeedBtn.Width := ButtonHeight;
  SpeedBtn.Height := ButtonHeight;
  SpeedBtn.Top := ButtonHeight + 4;
  SpeedBtn.Left := HSpace;
  SpeedBtn.Glyph.LoadFromResourceName(HInstance, 'CONE');;
  SpeedBtn.GroupIndex := 2;
  SpeedBtn.AllowAllUp := true;
  SpeedBtn.Hint := 'Фильтрация по мерам, размещенным на листе';
  SpeedBtn.ShowHint := true;
  SpeedBtn.OnClick := BtnFilterClick;

  SpeedBtn := TSpeedButton.Create(Self);
  SpeedBtn.Parent := FToolBarPanel;
  SpeedBtn.Name := 'btnParams';
  SpeedBtn.Anchors := [akRight, akTop];
  SpeedBtn.Width := ButtonHeight;
  SpeedBtn.Height := ButtonHeight;
  SpeedBtn.Top := 2* ButtonHeight + 8;
  SpeedBtn.Left := HSpace;
  SpeedBtn.Glyph.LoadFromResourceName(HInstance, 'PARAM');;
  SpeedBtn.GroupIndex := 3;
  SpeedBtn.AllowAllUp := true;
  SpeedBtn.Hint := 'Фильтрация по параметрам задачи';
  SpeedBtn.ShowHint := true;
  SpeedBtn.OnClick := BtnFilterClick;

  EnableButtons;
end;

destructor TAddinDimensionsTree.Destroy;
begin
  inherited Destroy;
  FSheetInterface := nil;
end;

(*function TAddinDimensionsTree.GetCube: TCube;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if (not FSorted) or (FilteredByParams) then
    exit;
  if not Assigned(FTreeView.Selected) then
    exit;
  case FTreeView.Selected.Level of
    0: P := FTreeView.Selected.Data;
    1: P := FTreeView.Selected.Parent.Data;
    2: P := FTreeView.Selected.Parent.Parent.Data;
  end;
  if Assigned(P) then
    result := TCube(P);
end;*)

function TAddinDimensionsTree.ChildrenLevelCount(Node: TTreeNode): integer;
var
  tempNode: TTreeNode;
begin
  result := 0;
  if not Assigned(Node) then
    exit;
  tempNode := Node.getFirstChild;
  while Assigned(tempNode) do
  begin
    inc(result);
    tempNode := tempNode.getFirstChild;
  end;
end;


function TAddinDimensionsTree.GetDimension: TDimension;
var
  P: Pointer;
  tmpStr: string;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) or FLoading then
    exit;
  if FilteredByParams then
  begin
    if FTreeView.Selected.Level = 0 then
      tmpStr := FTreeView.Selected.Item[0].Text
    else
      tmpStr := FTreeView.Selected.Parent.Item[0].Text;
    CutPart(tmpStr, ': ');
    tmpStr := CutPart(tmpStr, '.');
    result := Catalog.Dimensions.Find(tmpStr, Catalog.PrimaryProvider);
    exit;
  end;

  case TDimTreeNodeType((FTreeView.Selected as TBasicCheckTreeNode).NodeType) of
    ntCube: P := nil;
    ntSemantics: begin
      if FTreeView.Selected.HasChildren then
        P := FTreeView.Selected.getFirstChild.Data;
    end;
    ntDimension: P := FTreeView.Selected.Data;
    ntHierarchy: P := FTreeView.Selected.Parent.Data;
  end;

  if Assigned(P) then
    result := TDimension(P);
end;

function TAddinDimensionsTree.GetHierarchy: THierarchy;
var
  P: Pointer;
  tmpStr: string;
  tempDimension: TDimension;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) or FLoading then
    exit;

  if FilteredByParams then
  begin
    if FTreeView.Selected.Level = 0 then
      tmpStr := FTreeView.Selected.Item[0].Text
    else
      tmpStr := FTreeView.Selected.Parent.Item[0].Text;
    CutPart(tmpStr, '.');
    result := Dimension.Hierarchies.Find(tmpStr) as THierarchy;
    exit;
  end;

  case GetSelectionType of
    ntCube: P := nil;
    ntSemantics: begin
      if FTreeView.Selected.HasChildren then
        if FTreeView.Selected.getFirstChild.HasChildren then
          P := FTreeView.Selected.getFirstChild.getFirstChild.Data;
    end;
    ntDimension: begin
      if FTreeView.Selected.HasChildren then
        P := FTreeView.Selected.getFirstChild.Data;
    end;
    ntHierarchy: P := FTreeView.Selected.Data;
  end;

  if Assigned(P) then
    result := THierarchy(P)
  else
  begin
    tempDimension := GetDimension;
    if Assigned(tempDimension) and (tempDimension.Hierarchies.Count > 0) then
      result := tempDimension.Hierarchies[0];
  end;
end;

procedure TAddinDimensionsTree.BtnSortClick(Sender: TObject);
begin
  FOldCube := Cube;
  FOldDim := Dimension;
  FOldHier := Hierarchy;
  FSorted := TSpeedButton(Sender).Down;
  Load;
  FTreeView.SetFocus;
end;

procedure TAddinDimensionsTree.BtnFilterClick(Sender: TObject);
begin
  FOldCube := Cube;
  FOldDim := Dimension;
  FOldHier := Hierarchy;
  with TSpeedButton(Sender) do
    if Name = 'btnFilter' then
      FilteredByMeasures := Down
    else
    begin
      FilteredByParams := Down;
      EnableButtons;
    end;
  Load;
end;

function TAddinDimensionsTree.Load: boolean;
begin
  result := false;
  if not Assigned(FCatalog) then
    exit;
  FLoading := true;
  FTreeView.Items.BeginUpdate;
  FTreeView.Items.Clear;
  try
    if FilteredByParams then
    begin
      LoadInParamsMode; //режим отображения параметров
      exit;
    end;
    if FSorted then //измерения отсортированы по кубам
      LoadInCubesMode
    else
      LoadInNormalMode;
    FTreeView.AlphaSort;
    SetOldSelection;
  finally
    FTreeView.Items.EndUpdate;
    FLoading := false;
    Change(nil, nil);
    EnableButtons;
    result := not IsEmpty;
  end;
end;

procedure TAddinDimensionsTree.EnableButtons;
var
  Panel: TPanel;
begin
  Panel := TPanel(FindChildControl('ButtonPanel'));
  if not Assigned(Panel) then
    exit;
  TSpeedButton(Panel.Controls[2]).Enabled :=
    Assigned(SheetInterface);(* and Assigned(SheetInterface.TaskContext)*)
  if not TSpeedButton(Panel.Controls[2]).Enabled then
    TSpeedButton(Panel.Controls[2]).Down := false;

  TSpeedButton(Panel.Controls[0]).Enabled :=
    not TSpeedButton(Panel.Controls[2]).Down;
  TSpeedButton(Panel.Controls[1]).Enabled :=
    not TSpeedButton(Panel.Controls[2]).Down;
end;

function TAddinDimensionsTree.IsEmpty: boolean;
begin
  result := FTreeView.Items.Count = 0;
end;

function TCommonDimTree.SetSelection(Dim: TDimension;
  Hier: THierarchy): boolean;
begin
  result := true;
  try
    FOldCube := nil;
    FOldDim := Dim;
    FOldHier := Hier;
    SetOldSelection;
  except
    result := false;
  end;
end;

procedure Register;
begin
  RegisterComponents('FM Controls', [TAddinDimensionsTree]);
  RegisterComponents('FM Controls', [TSingleCubeDimTree]);
  RegisterComponents('FM Controls', [TCommonDimTree]);
  RegisterComponents('FM Controls', [TCubeDimTree]);
end;

{ TSingleCubeDimTree }

function TSingleCubeDimTree.CanResize(var NewWidth,
  NewHeight: integer): boolean;
begin
  if NewWidth < 240 then
    NewWidth := 240;
  if NewHeight < 160 then
    NewHeight := 160;
  result := true;
end;

procedure TSingleCubeDimTree.Change(Sender: TObject; Node: TTreeNode);
begin
  //во время загрузки не надо реагировать на обновление дерева
  if Assigned(FOnChange) and not FLoading then
    FOnChange(Sender, Node);
end;

procedure TSingleCubeDimTree.Changing(Sender: TObject; Node: TTreeNode;
  var AllowChange: Boolean);
begin
  if Assigned(FOnChanging) then
    FOnChanging(Sender, Node, AllowChange);
end;

procedure TSingleCubeDimTree.Clear;
begin
  FTreeView.Items.BeginUpdate;
  FTreeView.Items.Clear;
  FTreeView.Items.EndUpdate;
end;

constructor TSingleCubeDimTree.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  Parent := AOwner as TWinControl;
  Height := 160;
  Width := 400;
  BevelInner := bvNone;
  BevelOuter := bvNone;
  FCatalog := nil;

  {дерево измерений}
  FTreeView := TBasicCheckTreeView.Create(Self);//TTreeView.Create(Self);
  FTreeView.Parent := Self;
  FTreeView.Align := alClient;
  FTreeView.ShowButtons := true;
  FTreeView.ShowLines := true;
  FTreeView.ReadOnly := true;
  FTreeView.OnChange := Change;
  FTreeView.OnChanging := Changing;
  FTreeView.OnNodeCheck := OnTreeNodeCheck;
  FTreeView.HideSelection := false;
  FTreeView.ShowHint := true;
  FLoading := false;
  FShowCheckboxes[ntCube] := false;
  FShowCheckboxes[ntSemantics] := false;
  FShowCheckboxes[ntDimension] := false;
  FShowCheckboxes[ntHierarchy] := false;

  FillChar(FImageIndexes, SizeOf(FImageIndexes), -1);
end;

destructor TSingleCubeDimTree.Destroy;
begin
  FCatalog := nil;
  FTreeView.Free;
  inherited Destroy;
end;

procedure TSingleCubeDimTree.DoNodeCheck(Node: TTreeNode);
begin
  if Assigned(FOnNodeCheck) then
    FOnNodeCheck(Node);
end;

function TSingleCubeDimTree.FindNodeBy2Names(AParentName,
  AName: string): TBasicCheckTreeNode;
var
  Semantics, Dimension: string;
  NodeIndex, ParentLevel, i: integer;
begin
  result := nil;
  ParseDimensionName(AParentName, Semantics, Dimension);
  ParentLevel := IIF(FNeedShowCube, 1, 0);
  for NodeIndex := 0 to FTreeView.Items.Count - 1 do
    if FTreeView.Items[NodeIndex].Level = ParentLevel then
      if (FTreeView.Items[NodeIndex].Text = IIF((Semantics <> ''), Semantics, Dimension)) then
      begin
        result := TBasicCheckTreeNode(FTreeView.Items[NodeIndex]);
        if Assigned(result) and (Semantics <> '') then
          for i := 0 to result.Count do
            if (result.Item[i].Text = Dimension) then
            begin
              result := TBasicCheckTreeNode(result.Item[i]);
              break;
            end;
        break;
      end;


  if Assigned(result) then
    if (result.HasChildren) then
    begin
      for NodeIndex := 0 to result.Count - 1 do
      if result.Item[NodeIndex].Text = AName then
      begin
        result := TBasicCheckTreeNode(result.Item[NodeIndex]);
        exit;
      end;
      result := nil;
    end
end;

function TSingleCubeDimTree.FindNodeByName(AName: string): TBasicCheckTreeNode;
var
  NodeIndex: integer;
begin
  result := nil;
  for NodeIndex := 0 to FTreeView.Items.Count - 1 do
    if FTreeView.Items[NodeIndex].Text = AName then
    begin
      result := TBasicCheckTreeNode(FTreeView.Items[NodeIndex]);
      exit;
    end;
end;

function TSingleCubeDimTree.FindNodeByName(AName: string;
  AtLevel: integer): TBasicCheckTreeNode;
var
  NodeIndex: integer;
begin
  result := nil;
  for NodeIndex := 0 to FTreeView.Items.Count - 1 do
    if (FTreeView.Items[NodeIndex].Level = AtLevel) then
      if (FTreeView.Items[NodeIndex].Text = AName) then
      begin
        result := TBasicCheckTreeNode(FTreeView.Items[NodeIndex]);
        exit;
      end;
end;

function TSingleCubeDimTree.FindNodeWithinParent(
  AParent: TBasicCheckTreeNode; AName: string): TBasicCheckTreeNode;
var
  i: integer;
begin
  result := nil;
  if not Assigned(AParent) then
    exit;
  for i := 0 to AParent.Count - 1 do
    if AParent.Item[i].Text = AName then
    begin
      result := TBasicCheckTreeNode(AParent.Item[i]);
      exit;
    end;
end;

function TSingleCubeDimTree.GetCube: TCube;
begin
  result := FCube;
end;

function TSingleCubeDimTree.GetDimension: TDimension;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) then
    exit;
  case GetSelectionType of
    ntCube: P := nil;
    ntSemantics: begin
      if FTreeView.Selected.HasChildren then
        P := FTreeView.Selected.getFirstChild.Data;
    end;
    ntDimension: P := FTreeView.Selected.Data;
    ntHierarchy: P := FTreeView.Selected.Parent.Data;
  end;

  if Assigned(P) then
    result := TDimension(P);
end;

function TSingleCubeDimTree.GetHideSelection: boolean;
begin
  result := FTreeView.HideSelection;
end;

function TSingleCubeDimTree.GetHierarchy: THierarchy;
var
  P: Pointer;
  tempDimension: TDimension;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) then
    exit;
  case GetSelectionType of
    ntCube: P := nil;
    ntSemantics: begin
      if FTreeView.Selected.HasChildren then
        if FTreeView.Selected.getFirstChild.HasChildren then
          P := FTreeView.Selected.getFirstChild.getFirstChild.Data;
    end;
    ntDimension: begin
      if FTreeView.Selected.HasChildren then
        P := FTreeView.Selected.getFirstChild.Data;
    end;
    ntHierarchy: P := FTreeView.Selected.Data;
  end;

  if Assigned(P) then
    result := THierarchy(P)
  else
  begin
    tempDimension := GetDimension;
    if Assigned(tempDimension) and (tempDimension.Hierarchies.Count > 0) then
      result := tempDimension.Hierarchies[0];
  end;
end;

function TSingleCubeDimTree.GetImageIndexes(Index: TDimTreeNodeType): integer;
begin
  result := FImageIndexes[Index];
end;

function TSingleCubeDimTree.GetImages: TCustomImageList;
begin
  result := FImages;
end;

function TSingleCubeDimTree.GetOutputDimensionName: string;
var
  SName, DName: string;
begin
  result := '';
  if not (Assigned(Dimension) and Assigned(Hierarchy))then
    exit;
  ParseDimensionName(Dimension.Name, SName, DName);
  if (SName <> '') then
    result := Format('[%s].[%s].[%s]', [SName, DName, IIF(Hierarchy.Name = '',
      DefaultHierarchyName, Hierarchy.Name)])
  else
    result := Format('[%s].[%s]', [DName, IIF(Hierarchy.Name = '',
      DefaultHierarchyName, Hierarchy.Name)])
end;

function TSingleCubeDimTree.GetSelectedNode: TBasicCheckTreeNode;
begin
  if Assigned(FTreeView.Selected) then
    result := TBasicCheckTreeNode(FTreeView.Selected)
  else
    result := nil;
end;

function TSingleCubeDimTree.GetSelectedNodeAtLevel(Index: TDimTreeNodeType): TBasicCheckTreeNode;
var
  tmpInt: integer;
begin
  result := nil;
  if not Assigned(SelectedNode) then
    exit;

  tmpInt := Ord(Index) - Ord(SelectionType);
  case tmpInt of
    -3: begin
          result := TBasicCheckTreeNode(SelectedNode.Parent);
          if Assigned(result) then
            result := TBasicCheckTreeNode(result.Parent);
          if Assigned(result) and (result.NodeType <> Ord(SelectionType)) then
            result := TBasicCheckTreeNode(result.Parent);
        end;
    -2: begin
          result := TBasicCheckTreeNode(SelectedNode.Parent);
          if Assigned(result) and (result.NodeType <> Ord(SelectionType)) then
            result := TBasicCheckTreeNode(result.Parent);
        end;
    -1: result := TBasicCheckTreeNode(SelectedNode.Parent);
    0: result := SelectedNode;
    1:  begin
          result := SelectedNode;
          if result.HasChildren then
            result := TBasicCheckTreeNode(SelectedNode.getFirstChild);
        end;
    2:  begin
          result := TBasicCheckTreeNode(SelectedNode.getFirstChild);
          if Assigned(result) and (result.NodeType <> Ord(SelectionType)) then
            if result.HasChildren then
              result := TBasicCheckTreeNode(result.getFirstChild);
        end;
    3:  begin
          result := SelectedNode;
          if result.HasChildren then
          begin
            result := TBasicCheckTreeNode(SelectedNode.getFirstChild);
            if (Assigned(result) and result.HasChildren) then
            begin
              result := TBasicCheckTreeNode(result.getFirstChild);
              if Assigned(result) and (result.NodeType <> Ord(SelectionType)) and result.HasChildren then
                result := TBasicCheckTreeNode(result.getFirstChild);
            end;
          end;
        end;
  end;
end;

function TSingleCubeDimTree.GetSelectionType: TDimTreeNodeType;
begin
  result := ntNone;
  if Assigned(FTreeView.Selected) then
    result :=  TDimTreeNodeType((FTreeView.Selected as TBasicCheckTreeNode).NodeType);
end;

function TSingleCubeDimTree.GetShowCheckboxes(
  Index: TDimTreeNodeType): boolean;
begin
  result := FShowCheckBoxes[Index];
end;

function TSingleCubeDimTree.IsEmpty: boolean;
begin
  {если куб показывается, то один узел есть всегда}
  result := FTreeView.Items.Count < IIF(FNeedShowCube, 2, 1)
end;

function TSingleCubeDimTree.Load(ACube: TCube; NeedShowCube: boolean): boolean;
var
  DimIndex: integer;
  Dimension: TDimension;
  CubeNode: TBasicCheckTreeNode;
begin
  result := false;
  if not Assigned(FCatalog) then
    exit;
  FNeedShowCube := NeedShowCube;
  FLoading := true;
  FTreeView.Items.BeginUpdate;
  FTreeView.Items.Clear;
  FCube := ACube;
  if NeedShowCube then
  begin
    CubeNode := FTreeView.Items.AddChild(nil, Cube.Name) as TBasicCheckTreeNode;
    CubeNode.ImageIndex := ImageIndexes[ntCube];
    CubeNode.SelectedIndex := ImageIndexes[ntCube];
    CubeNode.StateIndex := IIF(ShowCheckboxes[ntCube], 1, -1);
    CubeNode.Data := Pointer(FCube);
    CubeNode.NodeType := Ord(ntCube);
  end
  else
    CubeNode := nil;
  for DimIndex := 0 to Cube.Dimensions.Count - 1 do
  begin
    Dimension := Cube.Dimensions[DimIndex];
    LoadDimension(CubeNode, Dimension);
  end;
  FTreeView.AlphaSort;
  FTreeView.Items.EndUpdate;
  FLoading := false;
  Change(nil, nil);
  result := not IsEmpty;
  if not result then
    exit;
  if NeedShowCube then
    FTreeView.Items[0].Expand(false);
  FTreeView.Items[0].Selected := true;
end;

procedure TSingleCubeDimTree.LoadDimension(CubeNode: TTreeNode;
  Dimension: TDimension);
var
  HierIndex: integer;
  Hierarchy: THierarchy;
  SemanticsNode, DimNode, HierNode: TBasicCheckTreeNode;
  SemantName, DimName, HierName: string;
begin
  ParseDimensionName(Dimension.Name, SemantName, DimName);
  SemanticsNode := nil;
  if (SemantName <> '') then
  begin
    SemanticsNode := FindNodeByName(SemantName);
    if not Assigned(SemanticsNode) then
      SemanticsNode := FTreeView.Items.AddChild(CubeNode, SemantName) as TBasicCheckTreeNode;
    SemanticsNode.ImageIndex := ImageIndexes[ntDimension];
    SemanticsNode.SelectedIndex := ImageIndexes[ntDimension];
      SemanticsNode.StateIndex := IIF(ShowCheckboxes[ntDimension], 1, -1);
    SemanticsNode.NodeType := Ord(ntSemantics);
  end;

  if Assigned(SemanticsNode) then
    DimNode := FTreeView.Items.AddChild(SemanticsNode, DimName) as TBasicCheckTreeNode
  else
    DimNode := FTreeView.Items.AddChild(CubeNode, DimName) as TBasicCheckTreeNode;

  DimNode.ImageIndex := ImageIndexes[ntDimension];
  DimNode.SelectedIndex := ImageIndexes[ntDimension];
  DimNode.Data := Pointer(Dimension);
  DimNode.NodeType := Ord(ntDimension);
  for HierIndex := 0 to Dimension.Hierarchies.Count - 1 do
  begin
    Hierarchy := Dimension.Hierarchies[HierIndex];
    HierName := IIF(Hierarchy.Name = '', 'Иерархия по умолчанию', Hierarchy.Name);
    HierNode := FTreeView.Items.AddChild(DimNode, HierName) as TBasicCheckTreeNode;
    HierNode.ImageIndex := ImageIndexes[ntHierarchy];
    HierNode.SelectedIndex := ImageIndexes[ntHierarchy];
    HierNode.StateIndex := IIF(ShowCheckboxes[ntHierarchy], 1, -1);
    HierNode.Data := Pointer(Hierarchy);
    HierNode.NodeType := Ord(ntHierarchy);
  end;
  if Assigned(SemanticsNode) then
    SemanticsNode.StateIndex := IIF(SemanticsNode.HasChildren, -1, 1);
  if Assigned(DimNode) then
    DimNode.StateIndex := IIF(DimNode.HasChildren, -1, 1);
end;

procedure TSingleCubeDimTree.OnTreeNodeCheck(Sender: Tobject);
begin
  DoNodeCheck(TTreeNode(Sender));
end;

procedure TSingleCubeDimTree.ParseDimensionName(DimensionName: string;
  out Semantics, Dimension: string);
var
  SepPos: integer;
begin
  Semantics := '';
  SepPos := Pos(snSemanticsSeparator, DimensionName);
  if (SepPos > 0) then
    Semantics := CutPart(DimensionName, snSemanticsSeparator);
  Dimension := DimensionName;
end;

procedure TSingleCubeDimTree.SetHideSelection(AValue: boolean);
begin
  FTreeView.HideSelection := AValue;
end;

procedure TSingleCubeDimTree.SetImageIndexes(Index: TDimTreeNodeType;
  const Value: integer);
begin
  FImageIndexes[Index] := Value;
end;

procedure TSingleCubeDimTree.SetImages(Value: TCustomImageList);
begin
  FImages := Value;
  if Assigned(FTreeView) then
    FTreeView.Images := Value;
end;

function TSingleCubeDimTree.SetSelection(Dim: TDimension;
  Hier: THierarchy): boolean;
begin
  result := false;
end;

function TAddinDimensionsTree.GetSelectionType: TDimTreeNodeType;
begin
  result := ntNone;
  if FilteredByParams then
    exit;

  if Assigned(FTreeView.Selected) then
    result := TDimTreeNodeType((FTreeView.Selected as TBasicCheckTreeNode).NodeType);
end;

procedure TSingleCubeDimTree.SetShowCheckBoxes(
  Index: TDimTreeNodeType; Value: boolean);
begin
  FShowCheckBoxes[Index] := Value;
end;

procedure TAddinDimensionsTree.LoadInParamsMode;

  procedure AddParam(AName, Dim, Comment: string; Inherit, MultiSelect: boolean);
  var
    Node: TTreeNode;
  begin
    Node := FTreeView.Items.AddChild(nil, AName);
    Node.ImageIndex := ImageIndexes[ntDimension];
    Node.SelectedIndex := ImageIndexes[ntDimension];
    FTreeView.Items.AddChild(Node, 'Измерение: ' + Dim);
    Node.Item[0].ImageIndex := 5;
    Node.Item[0].SelectedIndex := 5;
    FTreeView.Items.AddChild(Node, IIF(Inherit,
      'От родительской задачи', 'Локальный параметр'));
    Node.Item[1].ImageIndex := 5;
    Node.Item[1].SelectedIndex := 5;
    FTreeView.Items.AddChild(Node, 'Множественный выбор' +
      IIF(MultiSelect, ' разрешен' , ' запрещен'));
    Node.Item[2].ImageIndex := 5;
    Node.Item[2].SelectedIndex := 5;
    if Comment <> '' then
    begin
      FTreeView.Items.AddChild(Node, 'Комментарий: ' + Comment);
      Node.Item[3].ImageIndex := 5;
      Node.Item[3].SelectedIndex := 5;
    end;
  end;

var
  i: integer;
  TaskParams: TTaskParamsCollection;
  TaskParam: TTaskParam;
  SheetParams: TParamCollectionInterface;
  SheetParam: TParamInterface;
  DimName, HierName: string;
  Dim: TDimension;
  ParamNames: TStringList;
begin
  if Assigned(SheetInterface.TaskContext) then
  begin
    TaskParams := SheetInterface.TaskContext.GetTaskParams;
    if not Assigned(TaskParams) then
      exit;
    for i := 0 to TaskParams.Count - 1 do
    begin
      TaskParam := TaskParams[i];
      {если такое измерение отсутствует в каталоге, то пропускаем}
      HierName := TaskParam.Dimension;
      DimName := CutPart(HierName, '.');
      Dim := Catalog.Dimensions.Find(DimName, Catalog.PrimaryProvider);
      if not Assigned(Dim) then
        Continue;
      if (HierName <> '') then
        if not Assigned(Dim.Hierarchies.Find(HierName)) then
          Continue;
      with TaskParam do
        AddParam(Name, Dimension, Comment, IsInherited, AllowMultiSelect);
    end;
  end
  else
  begin
    SheetParams := SheetInterface.Params;
    ParamNames := TStringList.Create;
    try
      for i := 0 to SheetParams.Count - 1 do
      begin
        SheetParam := SheetParams[i];
        if (ParamNames.IndexOf(SheetParam.Name) <> - 1) then
          continue;
        ParamNames.Add(SheetParam.Name);
        with SheetParam do
          AddParam(Name, Dimension, Comment, IsInherited, MultiSelect);
      end;
    finally
      FreeAndNil(ParamNames);
    end;
  end;
end;

function TAddinDimensionsTree.GetParameter: TTaskParam;
var
  AName: string;
begin
  result := nil;
  if not Assigned(SheetInterface.TaskContext) then
    exit;
  if not FilteredByParams then
    exit;
  if not Assigned(FTreeView.Selected) then
    exit;
  if FTreeView.Selected.Level = 0 then
    AName := FTreeView.Selected.Text
  else
    AName := FTreeView.Selected.Parent.Text;
  result := SheetInterface.TaskContext.GetTaskParams.ParamByName(AName);
end;

function TAddinDimensionsTree.GetSheetParam: TParamInterface;
var
  AName: string;
begin
  result := nil;
  if not FilteredByParams then
    exit;
  if not Assigned(FTreeView.Selected) then
    exit;
  if (FTreeView.Selected.Level = 0) then
    AName := FTreeView.Selected.Text
  else
    AName := FTreeView.Selected.Parent.Text;
  result := SheetInterface.Params.ParamByName(AName);
end;

procedure TAddinDimensionsTree.SetToolBarVisible(AValue: boolean);
begin
  FToolBarPanel.Visible := AValue;
  Invalidate;
end;

{ TCommonDimTree }

function TCommonDimTree.GetCube: TCube;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) then
    exit;
  if FSorted then
    case FTreeView.Selected.Level of
      0: P := FTreeView.Selected.Data;
      1: P := FTreeView.Selected.Parent.Data;
      2: P := FTreeView.Selected.Parent.Parent.Data;
    end;
  if Assigned(P) then
    result := TCube(P);
end;

function TCommonDimTree.GetDimension: TDimension;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) or FLoading then
    exit;
  if FSorted then
    case FTreeView.Selected.Level of
      0: P := nil;
      1: P := FTreeView.Selected.Data;
      2: P := FTreeView.Selected.Parent.Data;
    end
  else
    case FTreeView.Selected.Level of
      0: P := FTreeView.Selected.Data;
      1: P := FTreeView.Selected.Parent.Data;
    end;
  if Assigned(P) then
    result := TDimension(P);
end;

function TCommonDimTree.GetHierarchy: THierarchy;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(FTreeView.Selected) or FLoading then
    exit;

  if FSorted then
    case FTreeView.Selected.Level of
      0: P := nil;
      //для измерения берем первую иерархию
      1: if FTreeView.Selected.HasChildren then
        P := FTreeView.Selected.getFirstChild.Data;
      2: P := FTreeView.Selected.Data;
    end
  else
    case FTreeView.Selected.Level of
      0: P := FTreeView.Selected.getFirstChild.Data;{P := nil;}
      1: P := FTreeView.Selected.Data;
    end;
  if Assigned(P) then
    result := THierarchy(P);
end;

function TCommonDimTree.IsEmpty: boolean;
begin
  result := FTreeView.Items.Count = 0;
end;

function TCommonDimTree.Load: boolean;
begin
  result := false;
  if not Assigned(FCatalog) then
    exit;
  FLoading := true;
  FTreeView.Items.BeginUpdate;
  FTreeView.Items.Clear;
  try
    if FSorted then //измерения отсортированы по кубам
      LoadInCubesMode
    else
      LoadInNormalMode;
    FTreeView.AlphaSort;
//    SetOldSelection;
  finally
    FTreeView.Items.EndUpdate;
    FLoading := false;
  end;
  Change(nil, nil);
  result := not IsEmpty;
end;

procedure TCommonDimTree.LoadInNormalMode;
var
  (*CubeIndex, *)DimIndex: integer;
  Dimension: TDimension;
begin
  with FCatalog do
  begin
    {сперва загрузим все общие измерения...}
    for DimIndex := 0 to Dimensions.Count - 1 do
    begin
      Dimension := Dimensions[DimIndex];
      LoadDimension(nil, Dimension);
    end;
(*    {...а затем оставшиеся частные из кубов}
    for CubeIndex := 0 to Cubes.Count - 1 do
      for DimIndex := 0 to Cubes[CubeIndex].Dimensions.Count - 1 do
        if not Cubes[CubeIndex].Dimensions[DimIndex].IsShared then
          LoadDimension(nil, Cubes[CubeIndex].Dimensions[DimIndex]);*)
  end;
end;

procedure TCommonDimTree.LoadInCubesMode;
var
  CubeIndex, DimIndex: integer;
  Dimension: TDimension;
  Cube: TCube;
  CubeNode: TBasicCheckTreeNode;
  Caption: string;
begin
  for CubeIndex := 0 to FCatalog.Cubes.Count - 1 do
  begin
    Cube := FCatalog.Cubes[CubeIndex];
    Caption := Cube.Name;
    if FCatalog.InMultibaseMode then
      Caption := Format('[P_ID = %s] %s)', [Cube.ProviderId, Caption]);
    CubeNode := FTreeView.Items.AddChild(nil, Caption) as TBasicCheckTreeNode;
    CubeNode.ImageIndex := ImageIndexes[ntCube];
    CubeNode.SelectedIndex := ImageIndexes[ntCube];
    CubeNode.StateIndex := IIF(ShowCheckboxes[ntCube], 1, -1);
    CubeNode.Data := Pointer(Cube);
    CubeNode.NodeType := Ord(ntCube);

    for DimIndex := 0 to Cube.Dimensions.Count - 1 do
    begin
      Dimension := Cube.Dimensions[DimIndex];
      LoadDimension(CubeNode, Dimension);
    end;
    {если в результате фильтрации у куба не осталось измерений, то выкинем его}
    if not CubeNode.HasChildren then
      FTreeView.Items.Delete(CubeNode);
  end;
end;


procedure TCommonDimTree.LoadDimension(CubeNode: TTreeNode;
  Dimension: TDimension);
var
  HierIndex: integer;
  Hierarchy: THierarchy;
  SemNode, DimNode, HierNode: TBasicCheckTreeNode;
  SemName, DimName, HierName: string;
begin
  SemNode := nil;
  {для начала выясним, присутстсвует ли в имени измерения семантика}
  ParseDimensionName(Dimension.Name, SemName, DimName);
  if FCatalog.InMultibaseMode and not FSorted then
    DimName := Format('[P_ID = %s] %s)', [Dimension.ProviderId, DimName]);
  if SemName <> '' then
  begin
    if Assigned(CubeNode) then
      SemNode := FindNodeWithinParent(TBasicCheckTreeNode(CubeNode), SemName)
    else
      SemNode := FindNodeByName(SemName);
    if not Assigned(SemNode) then
      SemNode := FTreeView.Items.AddChild(CubeNode, SemName) as TBasicCheckTreeNode;
    with SemNode do
    begin
      ImageIndex := ImageIndexes[ntDimension];
      SelectedIndex := ImageIndexes[ntDimension];
      StateIndex := IIF(ShowCheckboxes[ntDimension], 1, -1);
      NodeType := Ord(ntSemantics);
    end;
    DimNode := FTreeView.Items.AddChild(SemNode, DimName) as TBasicCheckTreeNode
  end
  else
    DimNode := FTreeView.Items.AddChild(CubeNode, DimName) as TBasicCheckTreeNode;

  with DimNode do
  begin
    ImageIndex := ImageIndexes[ntDimension];
    SelectedIndex := ImageIndexes[ntDimension];
    StateIndex := IIF(ShowCheckboxes[ntDimension], 1, -1);
    Data := Pointer(Dimension);
    NodeType := Ord(ntDimension);
  end;

  {и подгрузим коллекцию его иерархий}
  for HierIndex := 0 to Dimension.Hierarchies.Count - 1 do
  begin
    Hierarchy := Dimension.Hierarchies[HierIndex];

    if not IsFilterPassed(Dimension, Hierarchy) then
      continue;

    HierName := IIF(Hierarchy.Name = '', 'Иерархия по умолчанию', Hierarchy.Name);
    HierNode := FTreeView.Items.AddChild(DimNode, HierName) as TBasicCheckTreeNode;
    with HierNode do begin
      ImageIndex := ImageIndexes[ntHierarchy];
      SelectedIndex := ImageIndexes[ntHierarchy];
      StateIndex := IIF(ShowCheckboxes[ntHierarchy], 1, -1);
      Data := Pointer(Hierarchy);
      NodeType := Ord(ntHierarchy);
    end;
  end;

  {напоследок выкинем измерения, оставшиеся без иерархий}
  if not DimNode.HasChildren then
  begin
    DimNode.Data := nil;
    FTreeView.Items.Delete(DimNode);
    if (Assigned(SemNode) and (not SemNode.HasChildren)) then
      FTreeView.Items.Delete(SemNode);
  end;
end;

procedure TCommonDimTree.SetOldSelection;

  procedure FindNode(var Node: TTreeNode; P: Pointer);
  begin
    while Assigned(Node) do
    begin
      if Node.Data = P then
        Exit;
      Node := Node.getNextSibling;
    end;
    //докрутили цикл до конца и ничего не нашли
    Node := nil;
  end;

var
  i: integer;
  CubeNode, DimNode, HierNode, ResultNode : TTreeNode;
begin
  if IsEmpty then
    exit;

  CubeNode := nil;
  DimNode := nil;
  HierNode := nil;
  //если возможно, то выберем куб
  if FSorted then
  begin
    if Assigned(FOldCube) then
    begin
      CubeNode := FTreeView.Items.GetFirstNode;
      FindNode(CubeNode, Pointer(FOldCube));
    end;
    if Assigned(CubeNode) then //Куб найден, теперь отыщем измерение
    begin
      DimNode := CubeNode.getFirstChild;
      FindNode(DimNode, Pointer(FOldDim));
    end
  end;
  if not FSorted or not Assigned(DimNode) then
    //куб не задан, ищем первое вхождение измерения
    for i := 0 to FTreeView.Items.Count - 1 do
      if FTreeView.Items[i].Data = Pointer(FOldDim) then
      begin
        DimNode := FTreeView.Items[i];
        Break;
      end;
  if Assigned(DimNode) then //осталось найти иерархию (если есть)
    if Assigned(FOldHier) then
    begin
      HierNode := DimNode.getFirstChild;
      FindNode(HierNode, Pointer(FOldHier));
    end;
  if Assigned(HierNode) then
    ResultNode := HierNode
  else
    if Assigned(DimNode) then
      ResultNode := DimNode
    else
      if Assigned(CubeNode) then
        ResultNode := CubeNode
      else
        ResultNode := FTreeView.Items.GetFirstNode;
  if Assigned(ResultNode.Parent) then
    FTreeView.TopItem := ResultNode.Parent
  else
    FTreeView.TopItem := ResultNode;
  ResultNode.MakeVisible;
  ResultNode.Selected := true;
end;

function TCommonDimTree.IsFilterPassed(ADimension: TDimension;
  AHierarchy: Thierarchy): boolean;
begin
  result := true;
end;

{ TAddinDimTree }

function TAddinDimTree.Load: boolean;
begin
  result := true;
end;

procedure TAddinDimTree.SetFiltered(Value: boolean);
begin
  FilteredByMeasures := Value;
  Load;
end;

procedure TAddinDimTree.SetSorted(Value: boolean);
begin
  FSorted := Value;
  Load;
end;

function TAddinDimensionsTree.IsFilterPassed(ADimension: TDimension;
  AHierarchy: Thierarchy): boolean;
var
  MustSkip: boolean;
  TotalIndex: integer;
begin
  if FilteredByMeasures then
    with FSheetInterface do
    begin
      MustSkip := true;
      for TotalIndex := 0 to Totals.Count - 1 do
        if (Totals[TotalIndex].TotalType in [wtMeasure, wtResult]) then
          {если удовлетворяет хоть одной мере - скипать не надо}
          if Totals[TotalIndex].Cube.DimAndHierInCube(ADimension.Name, AHierarchy.Name) then
          begin
            MustSkip := false;
            Break;
          end;
      result := not MustSkip;
  end
  else
    result := true;
end;

function TCubeDimTree.GetNodeAtCursor: TBasicCheckTreeNode;
var
  Point: TPoint;
begin
  Point := FTreeView.ScreenToClient(Mouse.CursorPos);
  result := TBasicCheckTreeNode(FTreeView.GetNodeAt(Point.X, Point.Y));
end;


function TCubeDimTree.GetOnStartDrag: TStartDragEvent;
begin
  result := FTreeView.OnStartDrag;
end;

function TCubeDimTree.Load: boolean;
begin
  result := false;
  if not Assigned(FCatalog) then
    exit;
  FLoading := true;
  FTreeView.Items.BeginUpdate;
  FTreeView.Items.Clear;
  try
    LoadCubes;
    //FTreeView.AlphaSort;
  finally
    FTreeView.Items.EndUpdate;
    FLoading := false;
  end;
  Change(nil, nil);
  result := not IsEmpty;
end;

function TCubeDimTree.LoadCubeDimensions(Cube: TCube;
  CubeNode: TBasicCheckTreeNode): boolean;
var
  DimIndex: integer;
  Dimension: TDimension;
begin
  for DimIndex := 0 to Cube.Dimensions.Count - 1 do
  begin
    Dimension := Cube.Dimensions[DimIndex];
    LoadDimension(CubeNode, Dimension);
  end;
  result := true; // просто так
end;

function TCubeDimTree.LoadCubeMeasures(Cube: TCube; CubeNode: TBasicCheckTreeNode): boolean;
var
  MeasureIndex: integer;
  Measure: TMeasure;
  MeasureNodeCaption: string;
  MeasureNode: TBasicCheckTreeNode;
begin
  for MeasureIndex := 0 to Cube.Measures.Count - 1 do
  begin
    Measure := Cube.Measures[MeasureIndex];

    case Measure.Format of
      fmtCurrency: MeasureNodeCaption := Measure.Name + ' ($)';
      fmtPercent: MeasureNodeCaption := Measure.Name + ' (%)';
      fmtText: MeasureNodeCaption := Measure.Name + ' (@)';
      else
        MeasureNodeCaption := Measure.Name;
    end;
    MeasureNode := TBasicCheckTreeNode(FTreeView.Items.AddChild(CubeNode, MeasureNodeCaption));

    if Measure.IsCalculated then
    begin
      MeasureNode.NodeType := Ord(ntCalcMeasure);
      MeasureNode.ImageIndex := ImageIndexes[ntCalcMeasure];
      MeasureNode.SelectedIndex := ImageIndexes[ntCalcMeasure];
    end
    else
    begin
      MeasureNode.NodeType := Ord(ntMeasure);
      MeasureNode.ImageIndex := ImageIndexes[ntMeasure];
      MeasureNode.SelectedIndex := ImageIndexes[ntMeasure];
    end;
    MeasureNode.Data := Pointer(Measure);
  end;
  result := true;// просто так
end;

function TCubeDimTree.LoadCubes: boolean;
var
  CubeIndex: integer;
  Cube: TCube;
  CubeNode: TBasicCheckTreeNode;
begin
  for CubeIndex := 0 to FCatalog.Cubes.Count - 1 do
  begin
    Cube := FCatalog.Cubes[CubeIndex];
    CubeNode := FTreeView.Items.AddChild(nil, Cube.Name) as TBasicCheckTreeNode;
    CubeNode.ImageIndex := ImageIndexes[ntCube];
    CubeNode.SelectedIndex := ImageIndexes[ntCube];
    CubeNode.StateIndex := IIF(ShowCheckboxes[ntCube], 1, -1);
    CubeNode.Data := Pointer(Cube);
    CubeNode.NodeType := Ord(ntCube);

    LoadCubeMeasures(Cube, CubeNode);
    LoadCubeDimensions(Cube, CubeNode);

    if not CubeNode.HasChildren then
      FTreeView.Items.Delete(CubeNode);
  end;
  result := not IsEmpty;
end;

procedure TCubeDimTree.SetOnStartDrag(Event: TStartDragEvent);
begin
  FTreeView.OnStartDrag := Event;
  FTreeView.DragKind := dkDrag;
  FTreeView.DragMode := dmAutomatic;
end;

end.



