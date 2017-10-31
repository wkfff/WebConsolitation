unit uCheckTV2;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, StdCtrls, MSXML2_TLB, uFMAddinGeneralUtils, uXMLUtils,
  uFMExcelAddInConst, uFMAddinXMLUtils, uRusDlg, CommCtrl, uGlobalPlaningConst;

type
  TTVInfoTipEvent = procedure(Sender: TObject; Item: TTreeNode; var InfoTip: string) of object;

  TBasicCheckTreeNode = class;

  TTVMayCheck = function(Sender: TObject): boolean of object;

  TBasicCheckTreeView = class(TTreeView)
  private
    FStateImages: TImageList;
    FOnNodeCheck: TNotifyEvent;
    FOnInfoTip: TTVInfoTipEvent;
    FIsRadio: boolean;
    FMayCheck: TTVMayCheck;
    procedure WMLButtonDown(var Message: TWMLButtonDown); message WM_LBUTTONDOWN;
    procedure WMCreate(var Message: TMessage); message WM_Create;
    {комментарий смотри в коде}
    procedure WMNotify(var Message: TWMNotify); message WM_NOTIFY;
    {для решения пробемы залипающих хинтов}
    procedure WMMOUSEMOVE(var Message: TWMMouse); message WM_MOUSEMOVE;
  protected
    function CreateNode: TTreeNode; override;
    procedure CreateCheckMarks; dynamic;
    procedure DoNodeCheck(Node: TBasicCheckTreeNode);
    procedure DoInfoTip(Node: TTreeNode; var InfoTip: string); virtual;
    procedure SetIsRadio(const Value: boolean);
    function MayCheckTemplate(Sender: TObject): boolean;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function FindNodeByName(AName: string): TBasicCheckTreeNode;
    procedure UpdateStateImages;
    procedure UncheckAll(ExceptThis: TBasicCheckTreeNode);

    property OnNodeCheck: TNotifyEvent read FOnNodeCheck write FOnNodeCheck;
    property IsRadio: boolean read FIsRadio write SetIsRadio;
  published
    property OnInfoTip: TTVInfoTipEvent read FOnInfoTip write FOnInfoTip;
    property MayCheck: TTVMayCheck read FMayCheck write FMayCheck;
  end;

  TBasicCheckTreeNode = class(TTreeNode)
  private
    {вспомогательная числовая характеристика узла, может использоваться с
      приведением к перечислимым типам}
    FNodeType: integer;
    {вспомогательная булевская характеристика узла}
    FNodeFlag: boolean;
    FRelatedDomNode: IXMLDOMNode;
    function GetTreeView: TBasicCheckTreeView;
  protected
    FChecked: boolean;
    procedure SetChecked(AValue: boolean);
    procedure UpdateStateImage;
  public
    constructor Create(AOwner: TTreeNodes);
    destructor Destroy; override;
    procedure ToggleCheck;
    property Checked: boolean read FChecked write SetChecked;
    property TreeView: TBasicCheckTreeView read GetTreeView;
    {вспомогательная числовая характеристика узла, может использоваться с
      приведением к перечислимым типам}
    property NodeType: integer read FNodeType write FNodeType;
    {вспомогательная булевская характеристика узла}
    property NodeFlag: boolean read FNodeFlag write FNodeFlag;
    property RelatedDomNode: IXMLDOMNode read FRelatedDomNode
      write FRelatedDomNode;
  end;

  TCheckTreeNode2 = class;


  TCTV2BeforeUncheckEvent = procedure(LevelIndex: integer;
    var MayUncheck: boolean) of object;

  TCheckTreeView2 = class(TCustomTreeView)
  private
    FLoading: boolean;
    FOnNodeCheck: TNotifyEvent;
    FStateImages: TImageList;
    FLocked: boolean;
    FOnBeforeUncheck: TCTV2BeforeUncheckEvent;
    FIsFilterBehavior: boolean;
    procedure WMKeyDown(var Message: TWMKeyDown); message WM_KEYDOWN;
    {комментарий смотри в коде}
    procedure WMNotify(var Message: TWMNotify); message WM_NOTIFY;
    {для решения пробемы залипающих хинтов}
    procedure WMMOUSEMOVE(var Message: TWMMouse); message WM_MOUSEMOVE;
  protected
    procedure SetLoading(Value: boolean);
    function CreateNode: TTreeNode; override;
    procedure CreateCheckMarks; dynamic;
    procedure WMCreate(var Message: TMessage); message WM_Create;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y:
      Integer); override;
    procedure DoBeforeUncheck(LevelIndex: integer; var MayUncheck: boolean);
  public
    property Loading: boolean read FLoading write SetLoading;
    procedure DoNodeCheck(Node: TCheckTreeNode2);
    procedure UpdateNodeImages;
    property Locked: boolean read FLocked write FLocked;
    property OnBeforeUncheck: TCTV2BeforeUncheckEvent read FOnBeforeUncheck
      write FOnBeforeUncheck;
    property IsFilterBehavior: boolean read FIsFilterBehavior write FIsFilterBehavior;
  published
    property Align;
    property Anchors;
    property AutoExpand;
    property BorderStyle;
    property BorderWidth;
    property ChangeDelay;
    property Color;
    property Constraints;
    property DragKind;
    property DragCursor;
    property DragMode;
    property Enabled;
    property Font;
    property HideSelection;
    property Images;
    property Indent;
    property Items;
    property ParentColor default False;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ReadOnly default True;
    property RightClickSelect;
    property RowSelect;
    property ShowButtons;
    property ShowHint;
    property ShowLines;
    property ShowRoot;
    property SortType;
    property TabOrder;
    property TabStop default True;
    property ToolTips;
    property Visible;
    property OnChange;
    property OnChanging;
    property OnClick;
    property OnCollapsing;
    property OnCollapsed;
    property OnCompare;
    property OnCustomDraw;
    property OnCustomDrawItem;
    property OnDblClick;
    property OnDeletion;
    property OnDragDrop;
    property OnDragOver;
    property OnEdited;
    property OnEditing;
    property OnEndDock;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnExpanding;
    property OnExpanded;
    property OnGetImageIndex;
    property OnGetSelectedIndex;
    property OnKeyDown;
    property OnKeyPress;
    property OnKeyUp;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnStartDock;
    property OnStartDrag;
    property OnNodeCheck: TNotifyEvent read FOnNodeCheck
      write FOnNodeCheck;
    property OnContextPopup;
  end;

  {влияние узла на потомков (в порядке усиления эффекта):
    синий шарик - не влияет
    зеленый шарик - влияет на непосредственных детей
    красный шарик - влияет на всех потомков
    белый шарик - элемент игнорируется, потомки дизэйблятся}
  TNodeInfluence = (neNone, neChildren, neDescendants, neExclude);

  TCheckTreeNode2 = class(TTreeNode)
  private
    FChecked: boolean;
    FInfluence: TNodeInfluence;
    FReflexParent: boolean;
    FElement: IXMLDOMElement;
    FInCheckedLevel: boolean;
    FUnderInfluence: TNodeInfluence;
  protected
    procedure SetChecked(Value: boolean);
    function GetCheckedCount(Anywhere: boolean): integer;
    function GetUncheckedCount(Anywhere: boolean): integer;
    function GetHasCheckedChildren(Anywhere: boolean): boolean;
    function GetHasUncheckedChildren(Anywhere: boolean): boolean;
    procedure SetInfluence(Value: TNodeInfluence);
    procedure SetReflexParent(Value: boolean);
    procedure DoReflexParent;
    function GetDomNode: IXMLDomNode;
    procedure SetDomNode(Value: IXmlDomNode);
    procedure SetInCheckedLevel(Value: boolean);
    function GetTreeView: TCheckTreeView2;
    function Request: boolean;
  public
    destructor Destroy; override;
    procedure AfterConstruction; override;
    {инвертирует выделение узла}
    procedure ToggleCheck;
    {устанавливает выделение дочерним узлам}
    procedure CheckChildren(CheckValue, CheckAll: boolean);
    {обновляет вид шарика}
    procedure UpdateNodeImage;
    {рисует галку}
    procedure UpdateStateImage;
    {обновляет признак "нахождения под влиянием сверху"}
    procedure NewUnderInfluence;
    {признак выделения - галка}
    property Checked: boolean read FChecked write SetChecked;
    {признак влияния - шарик}
    property Influence: TNodeInfluence read FInfluence write SetInfluence;
    {признак "нахождения под влиянием сверху"}
    property UnderInfluence: TNodeInfluence read FUnderInfluence;
    {1 вариант ссылки на узел XML DOM-а}
    property DomElement: IXMLDOMElement read FElement write FElement;
    {2 вариант ссылки на узел XML DOM-а}
    property DomNode: IXMLDOMNode read GetDomNode write SetDomNode;
    {признак влияния на родителя в случае изменения свойств}
    property ReflexParent: boolean read FReflexParent write SetReflexParent;
    {признак вхождения узла в выделенные уровни измерения}
    property InCheckedLevel: boolean read  FInCheckedLevel
      write SetInCheckedLevel;
    {дерево-хозяин}
    property TreeView: TCheckTreeView2 read GetTreeView;
    {количество подчиненных узлов с галками}
    property CheckedCount[Anywhere: boolean]: integer read GetCheckedCount;
    {количество подчиненных узлов без галок}
    property UncheckedCount[Anywhere: boolean]: integer read GetUncheckedCount;
    {есть ли среди подчиненных узлы с галками}
    property HasCheckedChildren[Anywhere: boolean]: boolean
      read GetHasCheckedChildren;
    {есть ли среди подчиненных узлы без галок}
    property HasUncheckedChildren[Anywhere: boolean]: boolean
      read GetHasUncheckedChildren;
  end;

  procedure Register;

implementation

procedure TCheckTreeNode2.NewUnderInfluence;
var
  ParentInfluence, ParentUnderInfluence: TNodeInfluence;
begin
  {результат всецело зависит от соответствующих атрибутов родительского узла}
  if Assigned (Parent) then
  begin
    ParentInfluence := TCheckTreeNode2(Parent).Influence;
    ParentUnderInfluence := TCheckTreeNode2(Parent).UnderInfluence;
  end
  else
  begin
    ParentInfluence := neNone;
    ParentUnderInfluence := neNone;
  end;
  {здесь все просто - более сильное значение перекрывает остальные}
  case ParentUnderInfluence of
    neNone, neChildren:
      FUnderInfluence := ParentInfluence;
    neDescendants:
      if ParentInfluence = neExclude then
        FUnderInfluence := neExclude
      else
        FUnderInfluence := neDescendants;
    neExclude:
      FUnderInfluence := neExclude;
  end;
  if Assigned(DomElement) then
    DomElement.setAttribute('underinfluence', FUnderInfluence);
end;

procedure TCheckTreeNode2.UpdateNodeImage;
begin
  {узлы (с невыделенных уровней и без зависимостей) или под белым шариком будут серенькими}
  if not(InCheckedLevel or (Influence in [neChildren, neDescendants])) or
    (UnderInfluence = neExclude) then
    ImageIndex := IIF(Influence = neExclude, 38, IIF(HasCheckedChildren[true], 31, 37))
  else
    {если есть выделенные потомки, то шарик будет королевским}
    if HasCheckedChildren[true] then
      case Influence of
        neNone: ImageIndex := 21;
        neChildren: ImageIndex := 32;
        neDescendants: ImageIndex := 33;
        neExclude: ImageIndex := 38;
      end
    else
      case Influence of
        neNone: ImageIndex := 34;
        neChildren: ImageIndex := 35;
        neDescendants: ImageIndex := 36;
        neExclude: ImageIndex := 38;
      end;
  SelectedIndex := ImageIndex;
end;

procedure TCheckTreeNode2.UpdateStateImage;
begin
  {громкое название и тривиальный код :)}
  StateIndex := IIF(Checked, 2, 1);
end;

destructor TCheckTreeNode2.Destroy;
begin
  FElement := nil;
  inherited Destroy;
end;

procedure TCheckTreeNode2.SetChecked(Value: boolean);
begin
  if (Value <> FChecked) then
  begin
    FChecked := Value;
    if Assigned(DomElement) then
      DomElement.setAttribute('checked', BoolToStr(Value));
    UpdateStateImage;
    if not TreeView.Loading then
      TreeView.DoNodeCheck(Self);
  end;
end;

procedure TCheckTreeNode2.CheckChildren(CheckValue, CheckAll: boolean);
var
  Node: TCheckTreeNode2;
  i: integer;
begin
  if not HasChildren then
    exit;
  for i := 0 to Count - 1 do
  begin
    Node := TCheckTreeNode2(Item[i]);
    if not ((Node.Influence = neExclude) and CheckValue) then
      Node.Checked := CheckValue;
    if CheckAll then
      Node.CheckChildren(CheckValue, CheckAll);
    if not TreeView.Loading and (i = Count - 1) then
      Node.DoReflexParent;
  end;
end;

procedure TCheckTreeNode2.SetInfluence(Value: TNodeInfluence);

  procedure InfluenceChildren(Node: TCheckTreeNode2;
    Value, OldValue: TNodeInfluence);
  var
    i: integer;
    ChildNode: TCheckTreeNode2;
  begin
    if not Node.HasChildren then
      exit;
    for i := 0 to Node.Count - 1 do
    begin
      ChildNode := TCheckTreeNode2(Node.Item[i]);
      ChildNode.NewUnderInfluence;
      if (ChildNode.Influence in [neChildren, neDescendants]) and
        (ChildNode.UnderInfluence in [neDescendants, neExclude]) then
        ChildNode.Influence := neNone;
      ChildNode.Checked := (ChildNode.Checked or
        (ChildNode.UnderInfluence in [neChildren, neDescendants])) and
        (ChildNode.Influence <> neExclude) and
        (ChildNode.UnderInfluence <> neExclude);
      if (OldValue in [neDescendants, neExclude]) or
        (Value in [neDescendants, neExclude]) then
      begin
        InfluenceChildren(ChildNode, Value, OldValue);
      end;
      ChildNode.UpdateNodeImage;
      if not TreeView.Loading and (i = Node.Count - 1) then
        ChildNode.DoReflexParent;
    end;
  end;

var
  OldInfluence: TNodeInfluence;
  OldCursor: TCursor;
  OldLoading: boolean;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  try
    OldInfluence := FInfluence;
    FInfluence := Value;
    DomElement.setAttribute('influence', Value);
    {тонкая ситуация: если мы имели белый шарик и делаем из него другой, а
    сверху на него действует зеленый или красный, то необходимо
    поставить галку}
    if Influence = neExclude then
      Checked := false
    else
      if (OldInfluence = neExclude) then
        if UnderInfluence in [neChildren, neDescendants] then
          Checked := true;
    if not TreeView.Loading then
    begin
      OldLoading := true;
      TreeView.Loading := true;
    end
    else
      OldLoading := false;
    InfluenceChildren(Self, Influence, OldInfluence);
    if OldLoading then
      TreeView.Loading := false;
  finally
    if not TreeView.Loading then
    begin
      DoReflexParent;
      UpdateNodeImage;
    end;
    Screen.Cursor := OldCursor;
  end;
end;

procedure TCheckTreeNode2.SetReflexParent(Value: boolean);
begin
  if Value <> FReflexParent then
  begin
    FReflexParent := Value;
    DoReflexParent;
  end;
end;

procedure TCheckTreeNode2.DoReflexParent;
var
  ParentNode: TCheckTreeNode2;
  OldCursor: TCursor;
begin
  if not Assigned(Parent) or not FReflexParent then
    exit;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  try
    ParentNode := TCheckTreeNode2(Parent);
    while Assigned(ParentNode) do
    begin
      {если где-то под зеленым или красным шариком сняли галку,
        то он становится синим}
      if ParentNode.Influence in [neChildren, neDescendants] then
        if ParentNode.HasUncheckedChildren[ParentNode.Influence = neDescendants] then
          ParentNode.Influence := neNone;
      if not TreeView.Loading then
        ParentNode.UpdateNodeImage;
      if TreeView.IsFilterBehavior and (Checked = false) and (Influence <> neExclude) then
        ParentNode.Checked := false;
      ParentNode := TCheckTreeNode2(ParentNode.Parent);
    end;
  finally
    Screen.Cursor := OldCursor;
  end;
end;

function TCheckTreeNode2.GetDomNode: IXmlDomNode;
begin
  if Assigned(FElement) then
    result := FElement as IXmlDomNode
  else
    result := nil;
end;

procedure TCheckTreeNode2.SetDomNode(Value: IXMLDOMNode);
begin
  FElement := nil;
  if not Assigned(Value) then
    exit;
  FElement := Value as IXMLDomElement;
end;

procedure TCheckTreeNode2.SetInCheckedLevel(Value: boolean);
begin
  FInCheckedLevel := Value;
  if not TreeView.Loading then
    UpdateNodeImage;
end;

procedure TCheckTreeNode2.AfterConstruction;
begin
  inherited;
  FReflexParent := true;
  FInfluence := neNone;
  FUnderInfluence := neNone;
  FChecked := false;
  FInCheckedLevel := false;
  FElement := nil;
end;

procedure TCheckTreeNode2.ToggleCheck;
begin
  {белым шарикам, их потомкам и тем кто не с выделенных уровней
    галки трогать не даем}
  if (UnderInfluence <> neExclude) and
    (Influence <> neExclude) and InCheckedLevel then
  begin
    Checked := not Checked;
    if not TreeView.Loading then
    begin
      DoReflexParent;
      if TreeView.IsFilterBehavior then
        CheckChildren(true, true);
    end;
  end;
end;

{ TCheckTreeView2 }

function TCheckTreeView2.CreateNode: TTreeNode;
begin
  result := TCheckTreeNode2.Create(Items);
end;

procedure TCheckTreeView2.DoNodeCheck(Node: TCheckTreeNode2);
begin
  if Assigned(FOnNodeCheck) then
    FOnNodeCheck(Node);
end;

procedure TCheckTreeView2.SetLoading(Value: boolean);
begin
  if Value <> FLoading then
    FLoading := Value;
  if Value then
    Items.BeginUpdate
  else
    Items.EndUpdate;
end;

procedure TCheckTreeView2.WMKeyDown(var Message: TWMKeyDown);
var
  MayUncheck: boolean;
begin
  if Message.CharCode = VK_SPACE then
  begin
    if not (Locked or Loading) then
      with TCheckTreeNode2(Selected) do
        if Request then
        begin
          MayUncheck := true;
          if Checked then
            DoBeforeUncheck(Level, MayUncheck);
          if MayUncheck then
            ToggleCheck;
        end;
    Message.Result := 0;
  end
  else
    inherited;
end;

procedure TCheckTreeView2.CreateCheckMarks;
const
  R: TRect = (Left: 2; Top: 2; Right: 15; Bottom: 15);
var
  Bmp, M: TBitmap;

  procedure Add(MaskColor: TColor = clWhite);
  begin
    FStateImages.AddMasked(Bmp, MaskColor);
  end;

begin
  if FStateImages = nil then
    exit;
  Items.BeginUpdate;
  Bmp := TBitmap.Create;
  M := TBitmap.Create;
  try
    Bmp.Width := 16;
    Bmp.Height := 16;
    M.Width := 16;
    M.Height := 16;

    { Add stub image }
    {0} Add;
    { Add flat images }
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONCHECK or
      DFCS_FLAT);
    {1} Add;
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONCHECK or
      DFCS_FLAT or DFCS_CHECKED);
    {2} Add;
  finally
    M.Free;
    Bmp.Free;
    Items.EndUpdate;
  end;
end;

procedure TCheckTreeView2.WMCreate(var Message: TMessage);
begin
  inherited;
  FStateImages := TImageList.Create(Self);
  StateImages := FStateImages;
  CreateCheckMarks;
end;

procedure TCheckTreeView2.MouseDown(Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
var
  Ht: THitTests;
  Node: TCheckTreeNode2;
  MayUncheck: boolean;
begin
  Ht := GetHitTestInfoAt(X, Y);
  if (Button = mbLeft) and (htOnStateIcon in Ht) then
    if not Locked then
    begin
      Node := TCheckTreeNode2(GetNodeAt(X, Y));
      if Node.Request then
      begin
        MayUncheck := true;
        if Node.Checked then
          DoBeforeUncheck(Node.Level, MayUncheck);
        if MayUncheck then
          Node.ToggleCheck;
      end;
    end;
  inherited MouseDown(Button, Shift, X, Y);
end;

function TCheckTreeNode2.GetTreeView: TCheckTreeView2;
begin
  result := TCheckTreeView2(inherited TreeView);
end;

function TCheckTreeNode2.GetHasCheckedChildren(Anywhere: boolean): boolean;
begin
  result := CheckedCount[Anywhere] > 0;
end;

function TCheckTreeNode2.GetHasUncheckedChildren(Anywhere: boolean): boolean;
begin
  result := UncheckedCount[AnyWhere] > 0;
end;

function TCheckTreeNode2.GetCheckedCount(Anywhere: boolean): integer;
begin
  result := 0;
  if Assigned(DomNode) then
    if Anywhere then
      result := DomNode.selectNodes('.//Member[@checked="true"]').length
    else
      result := DomNode.selectNodes('./Member[@checked="true"]').length;
end;

function TCheckTreeNode2.GetUncheckedCount(Anywhere: boolean): integer;
begin
  result := 0;
  if Assigned(DomNode) then
    if Anywhere then
      result := DomNode.selectNodes('.//Member[(@checked="false") and (@influence!="3") and (@underinfluence!="3")]').length
    else
      result := DomNode.selectNodes('./Member[(@checked="false") and (@influence!="3") and (@underinfluence!="3")]').length
end;

procedure TCheckTreeView2.UpdateNodeImages;
var
  i: integer;
begin
  for i := 0 to Items.Count - 1 do
    TCheckTreeNode2(Items[i]).UpdateNodeImage;
end;

procedure Register;
begin
  RegisterComponents('FM Controls', [TCheckTreeView2]);
  RegisterComponents('FM Controls', [TBasicCheckTreeView]);
end;

function TCheckTreeNode2.Request: boolean;
begin
  {здесь зашита обработка ситуации, когда пользователь снимает галку
  с потомка "красного или зеленого шарика". Чтобы исключить возможность
  случайного, ненамеренного сброса признака влияния, спросим у пользователя,
  уверен ли он в желании снять галку и отключить "шарик"}
  result := true;
  if not TreeView.Loading and (Influence <> neExclude) and Checked then
    if UnderInfluence in [neChildren, neDescendants] then
      result := RusMessageDlg(wrmDropInfluence, mtWarning,
        [mbYes, mbNo], 0) = mrYes;
end;

procedure TCheckTreeView2.DoBeforeUncheck(LevelIndex: integer; var MayUncheck: boolean);
begin
  if Assigned(FOnBeforeUncheck) then
    FOnBeforeUncheck(LevelIndex, MayUncheck);
end;

procedure TCheckTreeView2.WMNotify(var Message: TWMNotify);
var
  Node: TTreeNode;
  Pt: TPoint;
  FWideText: widestring;
begin
  {обработка проблемы виндовских контролов (COMCTL32.dll) с ограничением
    длины хинта 80 символами. Прототип данного обработчика смотри
    в Comctrls.TCustomTreeView, но он просто обрезает текст до 80 букав и
    притом глючит (порождает "залипающие" хинты)}
  with Message do
    if NMHdr^.code = TTN_NEEDTEXTW then
    begin
      GetCursorPos(Pt);
      Pt := ScreenToClient(Pt);
      Node := GetNodeAt(Pt.X, Pt.Y);
      if (Node = nil) or (Node.Text = '') or (PToolTipTextW(NMHdr)^.uFlags and TTF_IDISHWND = 0) then
        exit;
      if (GetComCtlVersion >= ComCtlVersionIE4) and (Length(Node.Text) < 80) then
      begin
        inherited;
        exit;
      end;
      FWideText := Node.Text;
      SendMessage(NMHdr^.hwndFrom, TTM_SETMAXTIPWIDTH, 0, 400);
      PToolTipTextW(NMHdr)^.lpszText := PWideChar(FWideText);
      Result := 1;
    end
    else inherited;
end;

procedure TCheckTreeView2.WMMOUSEMOVE(var Message: TWMMouse);
var
  ToolTip: longint;
  HT: THitTests;
begin
  inherited;
  HT := GetHitTestInfoAt(Message.XPos, Message.Ypos);
  if not (htOnItem in HT) then
  begin
    ToolTip := Perform(TVM_GETTOOLTIPS, 0, 0);
    SendMessage(ToolTip, TTM_POP, 0, 0);
  end;
end;

{ TBasicCheckTreeView }

procedure TBasicCheckTreeView.CreateCheckMarks;
const
  R: TRect = (Left: 2; Top: 2; Right: 15; Bottom: 15);
var
  Bmp, M: TBitmap;

  procedure Add(MaskColor: TColor = clWhite);
  begin
    FStateImages.AddMasked(Bmp, MaskColor);
  end;

begin
  if FStateImages = nil then
    exit;
  Items.BeginUpdate;
  Bmp := TBitmap.Create;
  M := TBitmap.Create;
  try
    Bmp.Width := 16;
    Bmp.Height := 16;
    M.Width := 16;
    M.Height := 16;

    { Add stub image }
    {0} Add;
    { Add flat images }
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONCHECK or
      DFCS_FLAT);
    {1} Add;
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONCHECK or
      DFCS_FLAT or DFCS_CHECKED);
    {2} Add;
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONRADIOIMAGE or
      DFCS_FLAT);
    DrawFrameControl(M.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONRADIOMASK or
      DFCS_FLAT);
    {3} FStateImages.Add(Bmp, M);
    DrawFrameControl(Bmp.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONRADIOIMAGE or
      DFCS_FLAT or DFCS_CHECKED);
    DrawFrameControl(M.Canvas.Handle, R, DFC_BUTTON, DFCS_BUTTONRADIOMASK or
      DFCS_FLAT or DFCS_CHECKED);
    {4} FStateImages.Add(Bmp, M);
  finally
    M.Free;
    Bmp.Free;
    Items.EndUpdate;
  end;
end;

destructor TBasicCheckTreeView.Destroy;
begin
  FreeAndNil(FStateImages);
  inherited;
end;

procedure TBasicCheckTreeView.WMCreate(var Message: TMessage);
begin
  inherited;
  FStateImages := TImageList.Create(Self);
  StateImages := FStateImages;
  CreateCheckMarks;
end;

procedure TBasicCheckTreeView.WMLButtonDown(var Message: TWMLButtonDown);
var
  Node: TBasicCheckTreeNode;
  Ht: THitTests;
begin
    inherited;
    Ht := GetHitTestinfoAt(Message.XPos, Message.YPos);
    if (htOnStateIcon in Ht) then
    begin
      Node := TBasicCheckTreeNode(GetNodeAt(Message.XPos, Message.YPos));
      if Node <> nil then
      begin
        if not MayCheck(Node) then
          exit;
        if not IsRadio then
          Node.ToggleCheck
        else
          if not Node.Checked then
          begin
            Node.Checked := true;
          end;
      end;
    end;
end;

procedure TBasicCheckTreeView.DoNodeCheck(Node: TBasicCheckTreeNode);
begin
  if Assigned(FOnNodeCheck) then
    FOnNodeCheck(Node);
end;

procedure TBasicCheckTreeView.WMNotify(var Message: TWMNotify);
var
  Node: TTreeNode;
  Pt: TPoint;
  FWideText: widestring;
  tmpStr: string;
begin
  {обработка проблемы виндовских контролов (COMCTL32.dll) с ограничением
    длины хинта 80 символами. Прототип данного обработчика смотри
    в Comctrls.TCustomTreeView, но он просто обрезает текст до 80 букав и
    притом глючит (порождает "залипающие" хинты)}
  with Message do
    if NMHdr^.code = TTN_NEEDTEXTW then
    begin
      GetCursorPos(Pt);
      Pt := ScreenToClient(Pt);
      Node := GetNodeAt(Pt.X, Pt.Y);
      if (Node = nil) or (Node.Text = '') or (PToolTipTextW(NMHdr)^.uFlags and TTF_IDISHWND = 0) then
        exit;
(*      if (GetComCtlVersion >= ComCtlVersionIE4) and (Length(Node.Text) < 80) then
      begin
        inherited;
        exit;
      end;*)
//      FWideText := Node.Text;
      tmpStr := Node.Text;
      DoInfoTip(Node, tmpStr);
      FWideText := tmpStr;
      SendMessage(NMHdr^.hwndFrom, TTM_SETMAXTIPWIDTH, 0, 400);
      PToolTipTextW(NMHdr)^.lpszText := PWideChar(FWideText);
      Result := 1;
    end
    else inherited;
end;

procedure TBasicCheckTreeView.DoInfoTip(Node: TTreeNode;
  var InfoTip: string);
begin
  if Assigned(FonInfoTip) then
    FOnInfoTip(Self, Node, InfoTip);
end;

function TBasicCheckTreeView.FindNodeByName(
  AName: string): TBasicCheckTreeNode;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Items.Count - 1 do
    if Items[i].Text = AName then
    begin
      result := TBasicCheckTreeNode(Items[i]);
      exit;
    end;
end;

procedure TBasicCheckTreeView.UpdateStateImages;
var
  i: integer;
begin
  for i := 0 to Items.Count - 1 do
    TBasicCheckTreeNode(Items[i]).UpdateStateImage;
end;

procedure TBasicCheckTreeView.SetIsRadio(const Value: boolean);
begin
  FIsRadio := Value;
end;

procedure TBasicCheckTreeView.UncheckAll(ExceptThis: TBasicCheckTreeNode);
var
  i: integer;
begin
  for i := 0 to Items.Count - 1 do
    if Items[i] <> ExceptThis then
    begin
      TBasicCheckTreeNode(Items[i]).FChecked := false;
      TBasicCheckTreeNode(Items[i]).UpdateStateImage;
    end;
end;

procedure TBasicCheckTreeView.WMMOUSEMOVE(var Message: TWMMouse);
var
  ToolTip: longint;
  HT: THitTests;
begin
  inherited;
  HT := GetHitTestInfoAt(Message.XPos, Message.Ypos);
  if not (htOnItem in HT) then
  begin
    ToolTip := Perform(TVM_GETTOOLTIPS, 0, 0);
    SendMessage(ToolTip, TTM_POP, 0, 0);
  end;
end;

function TBasicCheckTreeView.MayCheckTemplate(Sender: TObject): boolean;
begin
  result := true;
end;

constructor TBasicCheckTreeView.Create(AOwner: TComponent);
begin
  inherited;
  MayCheck := MayCheckTemplate;
end;

{ TBasicCheckTreeNode }

function TBasicCheckTreeView.CreateNode: TTreeNode;
begin
  result := TBasicCheckTreeNode.Create(Items);
end;

constructor TBasicCheckTreeNode.Create(AOwner: TTreeNodes);
begin
  inherited;
  FNodeType := 0;
  FNodeFlag := false;
end;

destructor TBasicCheckTreeNode.Destroy;
begin
  inherited;
  FRelatedDomNode := nil;
end;

function TBasicCheckTreeNode.GetTreeView: TBasicCheckTreeView;
begin
  result := TBasicCheckTreeView(inherited TreeView);
end;

procedure TBasicCheckTreeNode.SetChecked(AValue: boolean);
begin
  if (AValue <> FChecked) then
  begin
    FChecked := AValue;
    if TreeView.IsRadio then
      TreeView.UncheckAll(Self);
    UpdateStateImage;
    TreeView.DoNodeCheck(Self);
  end;
end;

procedure TBasicCheckTreeNode.ToggleCheck;
begin
  Checked := not Checked;
end;

procedure TBasicCheckTreeNode.UpdateStateImage;
begin
(*  if TreeView.IsRadio then
    StateIndex := IIF(Checked, 4, 3)
  else*)
    StateIndex := IIF(Checked, 2, 1);
end;

end.
