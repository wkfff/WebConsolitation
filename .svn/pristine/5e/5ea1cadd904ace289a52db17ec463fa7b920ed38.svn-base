{ Компонент TAddinMembersTree - редактор атрибутов XML-дерева мемберсов
  с дополнительной возможностью поиска по дереву.}


unit AddinMembersTree;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, StdCtrls, uCheckTV2, ExtCtrls, MSXML2_TLB, ImgList, uXMLUtils,
  uStringSimilarity, Buttons, uFMExcelAddInConst, ComObj,
  uFMAddinGeneralUtils, uFMAddinXMLUtils, Spin, PlaningTools_TLB,
  uLevelSelector, Menus, fmPopUpMenu, uRusDlg, uGlobalPlaningConst;

type

  TAddinMembersTree = class(TCustomPanel)
  private
    FPageControl: TPageControl;
    //Дерево измерения
    FTreeView: TCheckTreeView2;
    //заголовок вкладки дерева
    FTreePageCaption: string;
    //заголовок вкладки поиска
    FSearchPageCaption: string;
    FSearchLine: TEdit;
    FSearchButton: TButton;
    FSearchButtonCaption: string;
    //дерево результатов поиска
    FSearchResult: TCheckTreeView2;
    //XML измерения
    FMembersDOM: IXMLDOMDocument2;
    FImages: TCustomImageList;
    //Переключатель нечеткого поиска
    FUseFuzzy: TCheckBox;
    //Строка ввода коэффициента релевантности
    FRelevantnessLine: TSpinEdit;
    {редактор уровней}
    FSelector: TLevelSelector;
    {контекстное меню элемента}
    FPopup: TPopupMenu;
    {разрешено ли показывать селектор уровней}
    FMayShowSelector: boolean;
    FMembersReadOnly: boolean;
    FOnNodeCheck: TNotifyEvent;
    {признак наличия в измерении МП "Код", которое надо выводить}
    //FNeedShowCode: boolean;
    FCodeToShow: string;
    { Позволяет ли отключение уровня дизэйблить его элементы.
      Да - штатное поведение для измерений осей;
      Нет - для фильтров по требованию задачи 5634.}
    FMayDisableLevels: boolean;
    { Из числа выбранных элементов можно указать значение по умолчанию.
      Применяется только при настройке форм сбора данных, когда в качестве
      значения фильтра задается множество элементов, а в лист должно быть
      выведено какое-то одно из них.}
    FWithDefaultValue: boolean;
    FDefaultNode: TCheckTreeNode2;
    FIsFilterBehavior: boolean;
    //создает компоненты на странице дерева
    procedure ConstructTreePage;
    //создает компоненты на странице поиска
    procedure ConstructSearchPage;
    //загрузка узла дерева
    procedure LoadMemberNode(XmlNode: IXMLDOMNode; TreeNode: TTreeNode);
    procedure SyncronizePages(TreeControl: TCheckTreeView2);
    //делает кнопки доступными, если дерево не пусто
    procedure EnableSelectionButtons;
    //выводит количество найденных элементов
    procedure ShowSearchResult(Count: integer);
    procedure CreatePopup;
    procedure ContextPopup(Sender: TObject; MousePos: TPoint;
      var Handled: boolean);
    {обработчик контекстного меню элемента}
    procedure PopupHandler(Sender: TObject);
    {обработчик смены состояния уровня}
    procedure DoLevelCheck(LevelIndex: integer; LevelState: TLevelState);
    {обработчик выделения всех элементов уровня}
    procedure DoMembersCheck(LevelIndex: integer; Checked_: boolean);
    {обработчик раскрытия уровня}
    procedure DoLevelExpand(LevelIndex: integer);
    {обработка снятия выделения с элемента красного уровня}
    procedure BeforeUncheckQuery(LevelIndex: integer; var MayUncheck: boolean);
    function GetTabStyle: TTabStyle;
    procedure SetTabStyle(const Value: TTabStyle);
    procedure ReAlignButtons;
    function GetWasEdited: boolean;
    procedure SetMembersReadOnly(const Value: boolean);
    procedure SetDomLevelState(LevelIndex: integer; LevelState: TLevelState);
    procedure SetWithDefaultValue(const Value: boolean);
    { После перегрузки дерева пытается установить дефолтное значение.}
    procedure RestoreDefaultValue;
    function GetNeedShowCode: boolean;
    procedure SetIsFilterBehavior(const Value: boolean);
  protected
    { Protected declarations }
    function GetTreePageCaption: string;
    procedure SetTreePageCaption(Value: string);
    function GetSearchPageCaption: string;
    procedure SetSearchPageCaption(Value: string);
    function GetImageList: TCustomImageList;
    procedure SetImageList(Value: TCustomImageList);
    function GetSearchButtonCaption: string;
    procedure SetSearchButtonCaption(Value: string);
    procedure Resize; override;
    procedure DoSearch(Sender: TObject);
    function Search: integer; //возвращает кол-во найденных элементов
    function CanResize(var NewWidth, NewHeight: integer): boolean; override;
    //обработчик нажатия клавиш в строке поиска
    procedure SearchLineKeyDown(Sender: TObject;
      var Key: Word; Shift: TShiftState);
    procedure CustomDrawItem(Sender: TCustomTreeView; Node: TTreeNode;
      State: TCustomDrawState; var DefaultDraw: Boolean);
    procedure SetDefaultNode(NewDefaultNode: TCheckTreeNode2);
    function GetCheckedCount: integer;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    //загрузка дерева измерения
    function Load(DOM: IXMLDOMDocument2; AllLevels: string; CodeToShow: string): boolean;
    procedure Clear;
    {если мы ушли из редактирования, находясь на странице поиска,
    то эта процедура выполнит дополнительную синхронизацию}
    procedure SyncronizeSearchWithTree;
    procedure SetBounds(ALeft, ATop, AWidth, AHeight: Integer); override;
    procedure AfterConstruction; override;
    function GetMemberName(Node: IXMLDOMNode): string;
    function GetFullMemberName(Node: IXMLDOMNode): string;
    function GetCodeString(Node: IXMLDOMNode): string;

    {Фильтрует DOM-измерения по  состоянию уровней:
       -Выбрасывает мэмберы с выключенных уровней
       -Выбрасывает сами узлы выключенных уровней.
    Параметром передается обрабатываемый дом и компонет-дерево, где
    он был отредактирован.}
    procedure FilterMembersDom(Dom: IXMLDOMDocument2);

    //обработчик смены вкладки
    procedure TabChange(Sender: TObject);
    {обработчики кнопок выделения}
    procedure InvertButtonClick(Sender: TObject);
    procedure DeselectAll(Sender: TObject);
    procedure SelectAll(Sender: TObject);
    procedure ExpandButtonClick(Sender: TObject);
    procedure CollapseButtonClick(Sender: TObject);
    procedure ExpandCheckedButtonClick(Sender: TObject);
    procedure PrevCheckedButtonClick(Senfer: Tobject);
    procedure NextCheckedButtonClick(Senfer: Tobject);
    //обработчик переключателя нечеткого поиска
    procedure FuzzyClick(Sender: TObject);
    procedure RelevantnessLineChange(Sender: TObject);
    procedure Loaded; override;
    function GetPageIndex: integer;
    procedure SetPageIndex(Value: integer);
    function IsSelectorVisible: boolean;
    function GetLevelsReadOnly: boolean;
    procedure SetLevelsReadOnly(Value: boolean);
    function GetLevelNames(Index: integer): string;
    function GetLevelStates(Index: integer): TLevelState;
    procedure SetLevelStates(Index: integer; Value: TLevelState);
    function GetLevelCount: integer;
    procedure SetMayShowSelector(Value: boolean);
    procedure DoNodeCheck(Node: TTreeNode);
    procedure OnTreeNodeCheck(Sender: Tobject);
    function GetDefaultValue: string;

    {доступ к основным частям компонента}
    property PageControl: TPageControl read FPageControl write FPageControl;
    property TreeView: TCheckTreeView2 read FTreeView write FTreeView;
    property SearchResult: TCheckTreeView2 read FSearchResult write FSearchResult;
    property Selector: TLevelSelector read FSelector write FSelector;

    property MembersDOM: IXMLDOMDocument2 read FMembersDOM write FMembersDOM;
    property LevelNames[Index: integer]: string read GetLevelNames;
    property LevelStates[Index: integer]: TLevelState read GetLevelStates
      write SetLevelStates;
    property LevelCount: integer read GetLevelCount;
    {изменилось ли состояние дерева по сравнению с изначальным:
      true, если сброшена хоть одна галка или выставлен хоть один шарик}
    property WasEdited: boolean read GetWasEdited;
    property MembersReadOnly: boolean read FMembersReadOnly write
      SetMembersReadOnly;
    property NeedShowCode: boolean read GetNeedShowCode;

    property MayDisableLevels: boolean read FMayDisableLevels
      write FMayDisableLevels;
    property WithDefaultValue: boolean read FWithDefaultValue
      write SetWithDefaultValue;
    property CheckedCount: integer read GetCheckedCount;

  published
    property Align;
    property Anchors;
    property BevelInner;
    property BevelOuter;
    property Images: TCustomImageList read GetImageList write SetImageList stored true;
    property TreePageCaption: string read GetTreePageCaption
      write SetTreePageCaption stored true;
    property SearchPageCaption: string read GetSearchPageCaption
      write SetSearchPageCaption;
    property SearchButtonCaption: string read GetSearchButtonCaption
      write SetSearchButtonCaption;
    property OnResize;
    property OnCanResize;
    property PageIndex: integer read GetPageIndex write SetPageIndex;
    property LevelsReadOnly: boolean read GetLevelsReadOnly
      write SetLevelsReadOnly;
    property MayShowSelector: boolean read FMayShowSelector
      write SetMayShowSelector;
    property TabStyle: TTabStyle read GetTabStyle write SetTabStyle;
    property OnNodeCheck: TNotifyEvent read FOnNodeCheck
      write FOnNodeCheck;
    property TabOrder;
    property IsFilterBehavior: boolean read FIsFilterBehavior write SetIsFilterBehavior;
  end;

  {Еще один маленький специализированный компонент. Дерево мемберов с
    радиовыбором, никакого лишнего функционала.}
  TAddinMembersRadioTree = class(TCustomPanel)
  private
    FTreeView: TBasicCheckTreeView;
    FMembersDOM: IXMLDOMDocument2;
    FCheckedNode: TBasicCheckTreeNode;

    {Картинки и индексы для простого и королевского шариков}
    FImages: TCustomImageList;
    FSimpleIndex, FRoyalIndex: integer;
    {Те же индексы для узлов с запретом на выделение}
    FForbiddenSimpleIndex, FForbiddenRoyalIndex: integer;

    FLoading: boolean;

    //FOnNodeCheck: TNotifyEvent;
    function GetImageList: TCustomImageList;
    procedure SetImageList(const Value: TCustomImageList);
    procedure LoadMemberNode(XmlNode: IXMLDOMNode; TreeNode: TTreeNode);
    procedure UpdateNodeImages;
    function GetMayCheck: TTVMayCheck;
    procedure SetMayCheck(const Value: TTVMayCheck);
  protected
//    procedure DoNodeCheck(Node: TTreeNode);
    procedure OnTreeNodeCheck(Sender: Tobject);
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function Load(Dom: IXMLDOMDocument2): boolean;
    procedure SetImageIndices(Simple, Royal, ForbiddenSimple, ForbiddenRoyal: integer);

    property MembersDOM: IXMLDOMDocument2 read FMembersDOM write FMembersDOM;
  published
    property Images: TCustomImageList read GetImageList write SetImageList stored true;
    //property OnNodeCheck: TNotifyEvent read FOnNodeCheck write FOnNodeCheck;
    property MayCheck: TTVMayCheck read GetMayCheck write SetMayCheck;
  end;

procedure Register;


{$R AddinMembersTree.DCR}

implementation


const
  HSpace = 8;
  VSpace = 20;
  ButtonHeight = 20;
  xpAllMembers = 'function_result/Members//' + ntMember + '[@name]';



constructor TAddinMembersTree.Create(AOwner: TComponent);

  procedure CreateSelectionButton(Index: integer; ResourceName, AHint: string;
    ClickHandler: TNotifyEvent; AParent: TWinControl);
  var
    Button: TSpeedButton;
  begin
    Button := TSpeedButton.Create(Self);
//    Button.Anchors := [akRight, akTop];
    Button.Width := ButtonHeight;
    Button.Height := ButtonHeight;
    Button.Top := VSpace + Index * (ButtonHeight + 4);
    Button.Left := HSpace;
    Button.Caption := '';
    Button.OnClick := ClickHandler;
    Button.Parent := AParent;
    Button.Glyph.LoadFromResourceName(HInstance, ResourceName);
    Button.Hint := AHint;
    Button.ShowHint := true;
  end;

var
  Panel: TPanel;
  NewPage1, NewPage2: TTabSheet;
begin
  inherited Create(AOwner);
  Parent := AOwner as TWinControl;
  Height := 200;
  Width := 445;
  BevelInner := bvNone;
  BevelOuter := bvNone;
  FMembersDOM := nil;
  {панель для кнопок выделения - размещается справа от пейджконтрола}
  Panel := TPanel.Create(Self);
  Panel.Name := 'SelectionPanel';
  Panel.Align := alRight;
  Panel.Caption := '';
  Panel.Width := 2 * HSpace + ButtonHeight;
  Panel.BevelInner := bvNone;
  Panel.BevelOuter := bvNone;
  Panel.Parent := Self;
  {кнопки выделения}
  CreateSelectionButton(0, 'PLUS', 'Выделить все', SelectAll, Panel);
  CreateSelectionButton(1, 'MINUS', 'Снять выделение', DeselectAll, Panel);
  CreateSelectionButton(2, 'STAR',
    'Обратить выделение', InvertButtonClick, Panel);
  CreateSelectionButton(3, 'EXPAND',
    'Развернуть все уровни', ExpandButtonClick, Panel);
  CreateSelectionButton(4, 'COLLAPSE',
    'Свернуть все уровни', CollapseButtonClick, Panel);
  CreateSelectionButton(5, 'EXPANDCHECKED',
    'Развернуть до выделенных элементов', ExpandCheckedButtonClick, Panel);
  CreateSelectionButton(6, 'PREVCHECKED',
    'Перейти к предыдущему выделенному элементу', PrevCheckedButtonClick, Panel);
  CreateSelectionButton(7, 'NEXTCHECKED',
    'Перейти к следующему выделенному элементу', NextCheckedButtonClick, Panel);

  FPageControl := TPageControl.Create(Self);
  FPageControl.Align := alClient;
  FPageControl.OnChange := TabChange;
  FPageControl.Parent := Self;
  NewPage1 := TTabSheet.Create(Self);
  NewPage1.PageControl := FPageControl;
  NewPage1.Caption := 'Иерархия';
  NewPage2 := TTabSheet.Create(Self);
  NewPage2.PageControl := FPageControl;
  NewPage2.Caption := 'Поиск';
  //создание элементов страницы дерева
  ConstructTreePage;
  //создание элементов страницы поиска
  ConstructSearchPage;
  //обновим состояние кнопок
  EnableSelectionButtons;
  FMayDisableLevels := true;
  FWithDefaultValue := false;
  FDefaultNode := nil;
end;

destructor TAddinMembersTree.Destroy;
begin
  FPopup.Free;
  FMembersDOM := nil;
  FTreeView.Items.Clear;
  FTreeView.Free;
  FSearchLine.Free;
  FSearchButton.Free;
  FSearchResult.Items.Clear;
  FSearchResult.Free;
  FPageControl.Free;
  inherited Destroy;
end;

procedure TAddinMembersTree.ConstructTreePage;
begin
  FTreeView := TCheckTreeView2.Create(Self);
  FTreeView.Align := alClient;
  FTreeView.ShowButtons := true;
  FTreeView.ReadOnly := true;
  FTreeView.Images := FImages;
  FTreeView.Parent := FPageControl.Pages[0];
  FTreeView.HideSelection := false;
  FTreeView.OnContextPopup := ContextPopup;
  FTreeView.RightClickSelect := true;
  FTreeView.OnBeforeUncheck := BeforeUncheckQuery;
  FTreeView.OnNodeCheck := OnTreeNodeCheck;

  FSelector := TLevelSelector.Create(Self);
  FSelector.Align := alTop;
  FSelector.Visible := IsSelectorVisible;
  FSelector.Parent := FPageControl.Pages[0];
  FSelector.Indent := 19;
  FSelector.LeftMargin := 23;
  FSelector.BevelOuter := bvLowered;
  FSelector.OnLevelStateChange := DoLevelCheck;
  FSelector.OnLevelMembersCheck := DoMembersCheck;
  FSelector.OnLevelExpand := DoLevelExpand;
  FMayShowSelector := true;
  FTreeView.OnCustomDrawItem := CustomDrawItem;
end;

procedure TAddinMembersTree.ConstructSearchPage;
var
  Lbl: TLabel;
begin
  {кнопка поиска}
  FSearchButton := TButton.Create(Self);
  FSearchButton.Caption := 'Найти!';
  FSearchButton.Top := VSpace;
  FSearchButton.Left := FPageControl.Pages[1].Width - HSpace -
    FSearchButton.Width;
  FSearchButton.Height := ButtonHeight;
  FSearchButton.OnClick := DoSearch;
  FSearchButton.Parent := FPageControl.Pages[1];
  FSearchButton.Anchors := [akTop, akRight];
  {строка ввода шаблона}
  FSearchLine := TEdit.Create(Self);
  FSearchLine.Anchors := [akLeft, akTop, akRight];
  FSearchLine.Left := HSpace;
  FSearchLine.Top := VSpace;
  FSearchLine.Width := FSearchButton.Left - 2 * HSpace;
  FSearchLine.OnKeyDown := SearchLineKeyDown;
  FSearchLine.Parent := FPageControl.Pages[1];
  {Переключатель нечеткого поиска}
  FUseFuzzy := TCheckBox.Create(Self);
  FUseFuzzy.Caption := 'Нечеткий поиск с критерием похожести (%):';
  FUseFuzzy.Left := 2 * HSpace;
  FUseFuzzy.Top := FSearchLine.Top + (3 * VSpace div 2);
  FUseFuzzy.Width := 240;
  FUseFuzzy.Checked := false;
  FUseFuzzy.OnClick := FuzzyClick;
  FUseFuzzy.Parent := FPageControl.Pages[1];
  {Строка ввода коэффициента релевантности}
  FRelevantnessLine := TSpinEdit.Create(Self);
  FRelevantnessLine.Left := 3 * HSpace + FUseFuzzy.Width;
  FRelevantnessLine.Top := FUseFuzzy.Top;
  FRelevantnessLine.Width := 48;
  FRelevantnessLine.Value := 80;
  FRelevantnessLine.MinValue := 0;
  FRelevantnessLine.MaxValue := 100;
  FRelevantnessLine.Increment := 5;
  FRelevantnessLine.OnChange := RelevantnessLineChange;
  FuzzyClick(nil);
  FRelevantnessLine.Parent := FPageControl.Pages[1];
  FRelevantnessLine.Anchors := [akTop, akLeft];
  {лист с результатами поиска}
  FSearchResult := TCheckTreeView2.Create(Self);
  FSearchResult.Left := HSpace;
  FSearchResult.Top := FRelevantnessLine.Top + FRelevantnessLine.Height +
     VSpace;
  FSearchResult.Width := FSearchLine.Width + HSpace + FSearchButton.Width;
  FSearchResult.Height := FPageControl.Height -
    FSearchResult.Top - HSpace - 28;
  FSearchResult.ReadOnly := true;
  FSearchResult.ShowLines := false;
  FSearchResult.ReadOnly := true;
  FSearchResult.Images := FImages;
  FSearchResult.Parent :=  FPageControl.Pages[1];
  FSearchResult.Anchors := [akLeft, akTop, akBottom, akRight];
  {метки}
  Lbl := TLabel.Create(Self);
  Lbl.Parent := FPageControl.Pages[1];
  Lbl.Caption := 'Искать текст:';
  Lbl.Top := FSearchLine.Top - Lbl.Height;
  Lbl.Left := FSearchLine.Left;
  Lbl := TLabel.Create(Self);
  Lbl.Parent := FPageControl.Pages[1];
  Lbl.Caption := 'Результаты поиска:';
  Lbl.Top := FSearchResult.Top - Lbl.Height;
  Lbl.Left := FSearchResult.Left;
end;

function TAddinMembersTree.GetTreePageCaption: string;
begin
  result := FTreePageCaption;
  if Assigned(FPageControl) then
    if Assigned(FPageControl.Pages[0]) then
      result := FPageControl.Pages[0].Caption;
end;

function TAddinMembersTree.GetSearchPageCaption: string;
begin
  result := FSearchPageCaption;
  if Assigned(FPageControl) then
    if Assigned(FPageControl.Pages[1]) then
      result := FPageControl.Pages[1].Caption;
end;

procedure TAddinMembersTree.SetTreePageCaption(Value: string);
begin
  if Value = '' then
    exit;
  FTreePageCaption := Value;
  if not Assigned(FPageControl) then
    exit;
  if Assigned(FPageControl.Pages[0]) then
    FPageControl.Pages[0].Caption := Value;
end;

procedure TAddinMembersTree.SetSearchPageCaption(Value: string);
begin
  if Value = '' then
    exit;
  FSearchPageCaption := Value;
  if not Assigned(FPageControl) then
    exit;
  if Assigned(FPageControl.Pages[1]) then
    FPageControl.Pages[1].Caption := Value;
end;

function TAddinMembersTree.GetImageList: TCustomImageList;
begin
  result := FImages;
end;

procedure TAddinMembersTree.SetImageList(Value: TCustomImageList);
begin
  if not Assigned(Value) then
    exit;
  FImages := Value;
  if Assigned(FTreeView) then
    FTreeView.Images := Value;
  if Assigned(FSearchResult) then
    FSearchResult.Images := Value;
end;

procedure TAddinMembersTree.LoadMemberNode(XmlNode: IXMLDOMNode;
  TreeNode: TTreeNode);
var
  NewTreeNode: TCheckTreeNode2;
  NL: IXMLDOMNodeList;
  j: integer;
begin
  if Assigned(XmlNode) then
  begin
    NewTreeNode := FTreeView.Items.AddChild(TreeNode, GetMemberName(XMLNode)) as TCheckTreeNode2;
    with NewTreeNode do
    begin
      DomNode := XmlNode;
      Checked := GetBoolAttr(XmlNode, 'checked', true);
      Influence := TNodeinfluence(GetIntAttr(XmlNode, 'influence', 0));
      if IsFilterBehavior and (Influence in [neChildren, neDescendants]) then
        Influence := neNone;
      InCheckedLevel := (LevelStates[Level] in [lsEnabled, lsForced]) or not MayDisableLevels;
      NewUnderInfluence;
      UpdateStateImage;
    end;
    NewTreeNode.HasChildren := XmlNode.hasChildNodes;
    {!!!29.11.05 Временный вариант до исправления глюков.
    Полная загрузка всего дерева мемберов сразу.
    Большие измерения на медленных машинах будут изрядно тормозить.}
    if NewTreeNode.HasChildren then
    begin
      NL := XmlNode.childNodes;
      for j := 0 to NL.length - 1 do
        LoadMemberNode(NL[j], NewTreeNode);
    end;
  end;
end;

function TAddinMembersTree.Load(DOM: IXMLDOMDocument2;
  AllLevels: string; CodeToShow: string): boolean;

  procedure LoadLevels;
  var
    tmpState: integer;
    AName, XPath: string;
    AState: TLevelState;
    Node: IXMLDOMNode;
  begin
    FSelector.Clear;
    AName := CutPart(AllLevels, snBucks);
    while AName <> '' do
    begin
      AState := lsDisabled;
      XPath := 'function_result/Levels/Level[@name="' + AName + '"]';
      Node := DOM.selectSingleNode(XPath);
      if Assigned(Node) then
      begin
        tmpState := GetIntAttr(Node, attrLevelState, -1);
        if tmpState = -1 then
        begin
          AState := lsEnabled;
          (Node as IXMLDOMElement).setAttribute(attrLevelState, AState);
        end
        else
          AState := TLevelState(tmpState);
      end;
      FSelector.AddLevel(AName, AState);
      AName := CutPart(AllLevels, snBucks);
    end;
  end;

var
  NL: IXMLDOMNodeList;
  i: integer;
begin
  Resize;
  FCodeToShow := CodeToShow;
  FTreeView.Loading := true;
  FTreeView.Items.Clear;
  FSearchResult.Items.Clear;
  LoadLevels;
  result := true;
  try
    FMembersDOM := DOM;
    NL := FMembersDOM.selectNodes('function_result/Members/' + ntMember + '[@name]');
    for i := 0 to NL.length - 1 do
      LoadMemberNode(NL[i], nil);
    if WithDefaultValue then
      RestoreDefaultValue;
    {после окончания загрузки обновим иконки для всех узлов}
    FTreeView.UpdateNodeImages;
    //раскроем первый уровень
    FTreeView.Items[0].Expand(false);
    EnableSelectionButtons;
    FTreeView.Loading := false;
  except
    result := false;
  end;
end;

function TAddinMembersTree.GetSearchButtonCaption: string;
begin
  if Assigned(FSearchButton) then
    result := FSearchButton.Caption
  else result := FSearchButtonCaption;
end;

procedure TAddinMembersTree.SetSearchButtonCaption(Value: string);
begin
  FSearchButtonCaption := Value;
  if Assigned(FSearchButton) then
    FSearchButton.Caption := Value;
end;

procedure TAddinMembersTree.Resize;
begin
  inherited Resize;
(*  if not Assigned(FPageControl) then
    exit;
  if FPageControl.PageCount <> 2 then
    exit;
  FSearchButton.Left := FPageControl.Pages[1].Width - HSpace -
    FSearchButton.Width;
  FSearchLine.Width := FSearchButton.Left - 2 * HSpace;
  FSearchResult.Width := FSearchLine.Width + HSpace + FSearchButton.Width;
  FSearchResult.Height := FPageControl.Pages[1].Height -
    FSearchResult.Top - HSpace;*)
end;

procedure TAddinMembersTree.ShowSearchResult(Count: integer);
var
  i: integer;
  s: string;
begin
  for i := 0 to ComponentCount - 1 do
    if Components[i] is TLabel then
      with (Components[i] as TLabel) do
        if Pos('Результаты поиска:', Caption) > 0 then
        begin
          if (Count mod 10 = 1) and (Count <> 11) then
            s := ' элемент'
          else
            if (Count mod 10 in [2, 3, 4]) and not (Count in [12, 13, 14]) then
              s := ' элемента'
            else
              s := ' элементов';
          Caption := 'Результаты поиска: ' + IntToStr(Count) + s;
          Visible := true;
          Application.ProcessMessages;
        end;
end;

procedure TAddinMembersTree.DoSearch(Sender: TObject);
var
  Op: IOperation;
  i: integer;
begin
  Op := CreateComObject(CLASS_Operation) as IOperation;
  for i := 0 to ComponentCount - 1 do
    if Components[i] is TLabel then
      (Components[i] as TLabel).Visible := false;
  Application.ProcessMessages;
  try
    Op.StartOperation(Parent.Handle);
    Op.Caption := 'Поиск элементов';
    ShowSearchResult(Search);
  finally
    //чтобы эксель не терял фокус
    Application.ProcessMessages;
    Op.StopOperation;
    Op := nil;
  end;
end;

function TAddinMembersTree.GetFullMemberName(Node: IXMLDOMNode): string;
var
  NodeName, Code: string;
begin
  result := '';
  NodeName := GetStrAttr(Node, 'name', '');
  if NeedShowCode then
    Code := GetCodeString(Node);
  while NodeName <> '' do
  begin
    if result <> '' then
      result := MemberBrackets(NodeName) + '.' + result
    else
      result := MemberBrackets(NodeName);
    Node := Node.parentNode;
    NodeName := GetStrAttr(Node, 'name', '');
  end;
  if NeedShowCode then
    result := Code + result;
end;

function TAddinMembersTree.Search: integer;

  //удовлетворяет ли строка Str2 шаблону Str1
  function Fit(Str1, Str2: string): boolean;
  begin
    if FUseFuzzy.Checked then
      result := Similarity(Str1, Str2, 4) > StrToInt(FRelevantnessLine.Text)
    else
      result := Pos(AnsiUpperCase(Str1), AnsiUpperCase(Str2)) > 0;
  end;

var
  i: integer;
  NL: IXMLDOMNodeList;
  MemberName: string;
  Item: TCheckTreeNode2;
begin
  FSearchResult.Items.BeginUpdate;
  try
    FSearchResult.Items.Clear;
    result := 0;
    if not Assigned(FMembersDOM) then
      exit;
    NL := FMembersDOM.selectNodes(xpAllMembers);
    for i := 0 to NL.length - 1 do
    begin
      MemberName := GetMemberName(NL[i]);
      if Fit(FSearchLine.Text, MemberName) then
      begin
        Item := FSearchResult.Items.AddChild(nil, GetFullMemberName(NL[i])) as
          TCheckTreeNode2;
        Item.DomNode := NL[i];
        Item.Checked := GetBoolAttr(NL[i], 'checked', true);
        Item.Influence := TNodeInfluence(GetIntAttr(NL[i], attrInfluence, 0));
        Item.InCheckedLevel := true;
        Item.UpdateNodeImage; //лишний, страховочный вызов
        Item.UpdateStateImage;
      end;
    end;
    result := FSearchResult.Items.Count;
  finally
    FSearchResult.Items.EndUpdate;
  end;
end;

function TAddinMembersTree.CanResize(var NewWidth, NewHeight: integer): boolean;
begin
  result := ((Width < 364) or (NewWidth >= 364)) and ((Height < 160) or (NewHeight >= 160));
end;

procedure TAddinMembersTree.SearchLineKeyDown(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if Key = VK_RETURN then
    FSearchButton.Click;
end;

procedure TAddinMembersTree.SyncronizePages(TreeControl: TCheckTreeView2);
var
  i: integer;
begin
  for i := 0 to TreeControl.Items.Count - 1 do
    with TCheckTreeNode2(TreeControl.Items[i]) do
    begin
      Checked := GetBoolAttr(DomNode, 'checked', true);
      UpdateNodeImage;
    end;
end;

procedure TAddinMembersTree.TabChange(Sender: TObject);
begin
  {Событие срабатывает _после_ переключения страницы,
  следовательно, пэйджиндекс - это индекс страницы на которую переключились.}
  if Sender = FPageControl then
    if FPageControl.ActivePageIndex = 0 then
      SyncronizePages(FTreeView)
    else
    begin
      SyncronizePages(FSearchResult);
      if FSearchResult.Items.Count = 0 then
        ShowSearchResult(0);
    end;
end;

procedure TAddinMembersTree.InvertButtonClick(Sender: TObject);
var
  i: integer;
  OldCursor: TCursor;
begin
  if not Assigned(FMembersDOM)then
    exit;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourGlass;
  FTreeView.Loading := true;
  for i := 0 to FTreeView.Items.Count - 1 do
    with TCheckTreeNode2(FTreeView.Items[i]) do
    begin
      if Influence <> neExclude then
        Influence := neNone;
      ToggleCheck;
    end;
  SyncronizePages(FSearchResult);
  for i := 0 to LevelCount - 1 do
    if LevelStates[i] = lsForced then
      LevelStates[i] := lsEnabled;
  FTreeView.UpdateNodeImages;
  FTreeView.Loading := false;
  DoNodeCheck(FDefaultNode);//(nil);
  Screen.Cursor := OldCursor;
  FTreeView.Invalidate;
end;

procedure TAddinMembersTree.DeselectAll(Sender: TObject);
var
  i: integer;
  OldCursor: TCursor;
begin
  if not Assigned(FMembersDOM) then
    exit;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourGlass;
  FTreeView.Loading := true;
  for i := 0 to FTreeView.Items.Count - 1 do
    with TCheckTreeNode2(FTreeView.Items[i]) do
    begin
      Checked := false;
      if Influence <> neExclude then
        Influence := neNone;
    end;
  SyncronizePages(FSearchResult);
  for i := 0 to LevelCount - 1 do
    if LevelStates[i] = lsForced then
      LevelStates[i] := lsEnabled;
  FTreeView.UpdateNodeImages;
  FTreeView.Loading := false;
  DoNodeCheck(FDefaultNode);//(nil);
  Screen.Cursor := OldCursor;
  FTreeView.Invalidate;
end;

procedure TAddinMembersTree.SelectAll(Sender: TObject);
var
  OldCursor: TCursor;
  i: integer;
begin
  if not Assigned(FMembersDOM) then
    exit;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourGlass;
  FTreeView.Loading := true;
  for i := 0 to FTreeView.Items.Count - 1 do
    with TCheckTreeNode2(FTreeView.Items[i]) do
    begin
      Checked := true;
      if Influence = neExclude then
        Influence := neNone;
    end;
  SyncronizePages(FSearchResult);
  FTreeView.UpdateNodeImages;
  FTreeView.Loading := false;
  DoNodeCheck(nil);
  Screen.Cursor := OldCursor;
end;

procedure TAddinMembersTree.FuzzyClick(Sender: TObject);
begin
  FRelevantnessLine.Enabled := FUseFuzzy.Checked;
  FSearchButton.Enabled := FUseFuzzy.Checked and (FRelevantnessLine.Text <> '')
    or not FUseFuzzy.Checked;
end;

procedure TAddinMembersTree.RelevantnessLineChange(Sender: TObject);
begin
  FSearchButton.Enabled := FRelevantnessLine.Text <> '';
end;

procedure TAddinMembersTree.EnableSelectionButtons;
begin
  with Controls[0] as TPanel do
  begin
    (Controls[0] as TSpeedButton).Enabled := (FTreeView.Items.Count > 0)
      and not MembersReadOnly;
    (Controls[1] as TSpeedButton).Enabled := (FTreeView.Items.Count > 0)
      and not MembersReadOnly;
    (Controls[2] as TSpeedButton).Enabled := (FTreeView.Items.Count > 0)
      and not MembersReadOnly;
    (Controls[3] as TSpeedButton).Enabled := FTreeView.Items.Count > 0;
    (Controls[4] as TSpeedButton).Enabled := FTreeView.Items.Count > 0;
    (Controls[5] as TSpeedButton).Enabled := FTreeView.Items.Count > 0;
    (Controls[6] as TSpeedButton).Enabled := FTreeView.Items.Count > 0;
    (Controls[7] as TSpeedButton).Enabled := FTreeView.Items.Count > 0;
  end;
end;

procedure TAddinMembersTree.Clear;
begin
  FTreeView.Items.Clear;
  FSearchResult.Items.Clear;
  EnableSelectionButtons;
end;

procedure TAddinMembersTree.Loaded;
begin
  Resize;
end;

function TAddinMembersTree.GetPageIndex: integer;
begin
  result := FPageControl.ActivePageIndex;
end;

procedure TAddinMembersTree.SetPageIndex(Value: integer);
begin
  try
    if Value in [0, 1] then
      FPageControl.ActivePage := FPageControl.Pages[Value];
    FPageControl.Pages[Value].Repaint;
    if Value = 0 then
      FTreeView.Repaint;
  finally
    Application.ProcessMessages;
  end;
end;

procedure TAddinMembersTree.SyncronizeSearchWithTree;
begin
  if PageIndex = 1 then
    SyncronizePages(FTreeView);
end;

function TAddinMembersTree.IsSelectorVisible: boolean;
begin
  result := (FPageControl.Height > 340) and FMayShowSelector;
end;

procedure TAddinMembersTree.SetBounds(ALeft, ATop, AWidth, AHeight: Integer);
begin
  inherited;
  if Assigned(FSelector) then
    FSelector.Visible := IsSelectorVisible;
end;

procedure TAddinMembersTree.CreatePopup;
var
  PopupItems: array of TMenuItem;
begin
  SetLength(PopupItems, IIF(WithDefaultValue, 11, 9));
  PopupItems[0] := NewItem('Не влияет на подчиненные элементы', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState0');
  PopupItems[1] := NewItem('Автоматически включает дочерние элементы', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState1');
  PopupItems[2] := NewItem('Автоматически включает все подчиненные элементы', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState2');
  PopupItems[3] := NewItem('Элемент всегда игнорируется', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState7');

  PopupItems[4] := NewItem('-', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState3');

  PopupItems[5] := NewItem('Выделить дочерние элементы', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState4');
  PopupItems[6] := NewItem('Выделить все подчиненные элементы', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState5');
  PopupItems[7] := NewItem('Снять выделение с дочерних элементов', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState4');
  PopupItems[8] := NewItem('Снять выделение со всех подчиненных элементов', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState5');

  if WithDefaultValue then
  begin
    PopupItems[9] := NewItem('-', TextToShortcut(''), false, true,
      PopupHandler, 0, 'pmiState9');
    PopupItems[10] := NewItem('Выбрать значением по умолчанию',
      TextToShortcut(''), false, true, PopupHandler, 0, 'pmiSetDefault');
  end;

  FPopup := NewPopupMenu(Self, 'pmNodePopup', paLeft, true, PopupItems);
  FTreeView.PopupMenu := FPopup;
end;

procedure TAddinMembersTree.ContextPopup(Sender: TObject; MousePos: TPoint;
      var Handled: boolean);
var
  Node: TCheckTreeNode2;
  i: integer;
begin
  Node := TCheckTreeNode2(FTreeView.GetNodeAt(MousePos.x, MousePos.y));
  if not Assigned(Node) or MembersReadOnly then
  begin
    Handled := true;
    exit;
  end;
  for i := 0 to FPopup.Items.Count - 1 do
  begin
    FPopup.Items[i].Checked := false;
    FPopup.Items[i].Enabled := (not FTreeView.Locked or (i in [0, 3]))
      and not (Node.UnderInfluence = neExclude)
      and not (((Node.UnderInfluence = neDescendants) or IsFilterBehavior) and (i in [1, 2]));
    if (FPopup.Items[i].Name = 'pmiSetDefault') and FPopup.Items[i].Enabled then
      FPopup.Items[i].Enabled := Node.Checked;
  end;
  case Node.Influence of
    neNone: FPopup.Items[0].Checked := true;
    neChildren: FPopup.Items[1].Checked := true;
    neDescendants: FPopup.Items[2].Checked := true;
    neExclude: FPopup.Items[3].Checked := true;
  end;
  Node.Selected := true;
end;

procedure TAddinMembersTree.AfterConstruction;
begin
  CreatePopup;
end;

procedure TAddinMembersTree.PopupHandler(Sender: TObject);

  function InfluenceByIndex(Index: integer): TNodeInfluence;
  begin
    case Index of
      1: result := neChildren;
      2: result := neDescendants;
      3: result := neExclude;
      else result := neNone;
    end;
  end;

var
  Index: integer;
  Node: TCheckTreeNode2;
  OldCursor: TCursor;
begin
  if not Assigned(FTreeView.Selected) then
    exit;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourGlass;
  Index := TMenuItem(Sender).MenuIndex;
  Node := TCheckTreeNode2(FTreeView.Selected);
  case Index of
    0, 1, 2, 3:
      begin
        Node.Influence := InfluenceByIndex(Index);
      end;
    5: Node.CheckChildren(true, false);
    6: Node.CheckChildren(true, true);
    7: Node.CheckChildren(false, false);
    8: Node.CheckChildren(false, true);
    10:
      begin
        SetDefaultNode(Node);
        FTreeView.Invalidate;
      end;
  end;
  DoNodeCheck(Node);
  Screen.Cursor := OldCursor;
end;

function TAddinMembersTree.GetLevelsReadOnly: boolean;
begin
  result := FSelector.ReadOnly;
end;

procedure TAddinMembersTree.SetLevelsReadOnly(Value: boolean);
begin
  FSelector.ReadOnly := Value or FMembersReadOnly;
end;

function TAddinMembersTree.GetLevelNames(Index: integer): string;
begin
  result := FSelector.Names[Index];
end;

function TAddinMembersTree.GetLevelStates(Index: integer): TLevelState;
begin
  result := FSelector.States[Index];
end;

procedure Register;
begin
  RegisterComponents('FM Controls', [TAddinMembersTree]);
end;

function TAddinMembersTree.GetLevelCount: integer;
begin
  result := FSelector.Count;
end;

procedure TAddinMembersTree.DoLevelCheck(LevelIndex: integer;
  LevelState: TLevelState);
var
  i: integer;
begin
  FTreeView.Items.BeginUpdate;
  for i := 0 to FTreeView.Items.Count - 1 do
    if FTreeView.Items[i].Level = LevelIndex then
      with TCheckTreeNode2(FTreeView.Items[i]) do
      begin
        InCheckedLevel := (LevelState <> lsDisabled) or not MayDisableLevels;
        if LevelState <> lsForced then
          UpdateNodeImage;
      end;
  if LevelState = lsForced then
    DoMembersCheck(LevelIndex, true);
  SetDomLevelState(LevelIndex, LevelState);
  FTreeView.Items.EndUpdate;
end;

procedure TAddinMembersTree.DoMembersCheck(LevelIndex: integer;
  Checked_: boolean);
var
  i: integer;
begin
  FTreeView.Loading := true;
  {если снимаем выделение с элементов "красного" уровня, то красную галку
    заменяем на обычную, черную;
    если на верхних уровнях есть влиятельные шарики, то их надо сбросить}
  if (LevelStates[LevelIndex] = lsForced) and not Checked_ then
    LevelStates[LevelIndex] := lsEnabled;
  SetDomLevelState(LevelIndex, lsEnabled);
  for i := 0 to FTreeView.Items.Count - 1 do
    with TCheckTreeNode2(FTreeView.Items[i]) do
      if Level = LevelIndex then
        Checked := Checked_
      else
        if ((Level < LevelIndex) and (Influence = neDescendants)) or
          ((Level - LevelIndex = -1) and (Influence = neChildren)) then
          Influence := neNone;
  FTreeView.UpdateNodeImages;
  FTreeView.Loading := false;
end;

procedure TAddinMembersTree.BeforeUncheckQuery(LevelIndex: integer; var MayUncheck: boolean);
begin
  if LevelStates[LevelIndex] = lsForced then
  begin
    MayUncheck := RusMessageDlg('Снятие выделения с данного элемента ' +
      'приведет к сбросу безусловного выделения уровня. Продолжить?',
      mtWarning, [mbYes, mbNo], 0) = mrYes;
  if MayUncheck then
    LevelStates[LevelIndex] := lsEnabled;
  end
  else
    MayUncheck := true;
end;

procedure TAddinMembersTree.SetLevelStates(Index: integer;
  Value: TLevelState);
begin
  FSelector.States[Index] := Value;
end;

procedure TAddinMembersTree.CollapseButtonClick(Sender: TObject);
var
  OldCursor: TCursor;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  FTreeView.Items.BeginUpdate;
  FTreeView.FullCollapse;
  FTreeView.Items[0].Expand(false);
  FTreeView.Items.EndUpdate;
  Screen.Cursor := OldCursor;
end;

procedure TAddinMembersTree.ExpandButtonClick(Sender: TObject);
var
  OldCursor: TCursor;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  FTreeView.Items.BeginUpdate;
  FTreeView.FullExpand;
  FTreeView.Items.EndUpdate;
  Screen.Cursor := OldCursor;
end;

procedure TAddinMembersTree.ExpandCheckedButtonClick(Sender: TObject);
var
  OldCursor: TCursor;
  i: integer;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  FTreeView.Items.BeginUpdate;
  for i := 0 to FTreeView.Items.Count - 1 do
  begin
    if TCheckTreeNode2(FTreeView.Items[i]).HasCheckedChildren[true] then
      FTreeView.Items[i].Expand(false);
  end;
  FTreeView.Items.EndUpdate;
  Screen.Cursor := OldCursor;
end;

procedure TAddinMembersTree.NextCheckedButtonClick(Senfer: Tobject);
var
  TreeNode: TTreeNode;
begin
  if CheckedCount = 0 then
    exit;
  if Assigned(FTreeView.Selected) then
    TreeNode := FTreeView.Selected
  else
    TreeNode := FTreeView.TopItem;
  if not Assigned(TreeNode) then
    exit;
  TreeNode := TreeNode.GetNext;
  repeat
    if Assigned(TreeNode) then
    begin
      if TCheckTreeNode2(TreeNode).Checked then
      begin
        TreeNode.Selected := true;
        exit;
      end;
      TreeNode := TreeNode.GetNext;
    end;
    if not Assigned(TreeNode) then
      TreeNode := FTreeView.Items[0];
  until false;
end;

procedure TAddinMembersTree.PrevCheckedButtonClick(Senfer: Tobject);
var
  TreeNode: TTreeNode;
begin
  if CheckedCount = 0 then
    exit;
  if Assigned(FTreeView.Selected) then
    TreeNode := FTreeView.Selected
  else
    TreeNode := FTreeView.TopItem;
  if not Assigned(TreeNode) then
    exit;
  TreeNode := TreeNode.GetPrev;
  repeat
    if Assigned(TreeNode) then
    begin
      if TCheckTreeNode2(TreeNode).Checked then
      begin
        TreeNode.Selected := true;
        exit;
      end;
      TreeNode := TreeNode.GetPrev;
    end;
    if not Assigned(TreeNode) then
      TreeNode := FTreeView.Items[FTreeView.Items.Count - 1];
  until false;
end;

procedure TAddinMembersTree.DoLevelExpand(LevelIndex: integer);
var
  OldCursor: TCursor;
  i: integer;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  FTreeView.Items.BeginUpdate;
  for i := 0 to FTreeView.Items.Count - 1 do
    if FTreeView.Items[i].Level < LevelIndex then
      FTreeView.Items[i].Expand(false);
  FTreeView.Items.EndUpdate;
  Screen.Cursor := OldCursor;
end;

procedure TAddinMembersTree.SetMayShowSelector(Value: boolean);
begin
  if Value <> FMayShowSelector then
  begin
    FMayShowSelector := Value;
    if Assigned(FSelector) then
      FSelector.Visible := IsSelectorVisible;
    Repaint;
  end;
end;

function TAddinMembersTree.GetTabStyle: TTabStyle;
begin
  result := FPageControl.Style;
end;

procedure TAddinMembersTree.SetTabStyle(const Value: TTabStyle);
begin
  if Value <> FPageControl.Style then
  begin
    FPageControl.Style := Value;
    ReAlignButtons;
  end;
end;

procedure TAddinMembersTree.ReAlignButtons;
var
  Panel: TPanel;
  i: integer;
begin
  Panel := TPanel(FindChildControl('SelectionPanel'));
  if not Assigned(Panel) then
    exit;
  for i := 0 to Panel.ControlCount - 1 do
    TSpeedButton(Panel.Controls[i]).Top :=
      VSpace + IIF(TabStyle = tsTabs, 0, 6)
      + i * (ButtonHeight + 4);
end;

function TAddinMembersTree.GetWasEdited: boolean;
begin
  result := false;
  if Assigned(FMembersDom) then
    result := FMembersDom.selectNodes('function_result/Members//Member' +
      '[(@checked="false") or (@influence!="0")]').length > 0;
end;

procedure TAddinMembersTree.SetMembersReadOnly(const Value: boolean);
begin
  FMembersReadOnly := Value;
  FTreeView.Locked := Value;
  FSearchResult.Locked := Value;
  if Value then
    LevelsReadOnly := Value;
  EnableSelectionButtons;
end;

procedure TAddinMembersTree.DoNodeCheck(Node: TTreeNode);
begin
  if Assigned(FDefaultNode) then
    if Node = FDefaultNode then
      if not TCheckTreeNode2(Node).Checked then
        SetDefaultNode(nil);
  if Assigned(FOnNodeCheck) then
    FOnNodeCheck(Node);
end;

procedure TAddinMembersTree.OnTreeNodeCheck(Sender: Tobject);
begin
  DoNodeCheck(TTreeNode(Sender));
end;

procedure TAddinMembersTree.FilterMembersDom(Dom: IXMLDOMDocument2);

  procedure FilterMembers(OldRoot, NewRoot: IXMLDOMNode; LevelIndex: integer);
  var
    ChildrenNL, GrandChildrenNL: IXMLDOMNodeList;
    i, j: integer;
    Node: IXMLDOMNode;
  begin
    if not Assigned(OldRoot) then
      exit;
    if not OldRoot.hasChildNodes then
      exit;
    ChildrenNL := OldRoot.childNodes;
//    if not (LevelIndex in CheckedLevels) then
    if self.LevelStates[LevelIndex] = lsDisabled then
      for i := 0 to ChildrenNL.length - 1 do
      begin
        FilterMembers(ChildrenNL[0], NewRoot , LevelIndex + 1);
        if ChildrenNL[0].hasChildNodes then
        begin
          GrandChildrenNL := ChildrenNL[0].childNodes;
          for j := 0 to GrandChildrenNL.length - 1 do
          begin
            Node := GrandChildrenNL[j].cloneNode(true);
            NewRoot.appendChild(Node);
          end;
        end;
        //  удаляем
        Node := OldRoot.removeChild(ChildrenNL[0]);
        Node := nil;
      end
    else
      if OldRoot.hasChildNodes then
        for i := 0 to ChildrenNL.length - 1 do
          FilterMembers(ChildrenNL[i], ChildrenNL[i], LevelIndex + 1);
  end;

  procedure FilterLevels(Root: IXMLDOMNode);
  var
    ChildrenNL: IXMLDOMNodeList;
    i, LevelState: integer;
  begin
    if not Assigned(Root) then
      exit;
    ChildrenNL := Root.childNodes;
    for i := ChildrenNL.length - 1 downto 0 do
      if self.LevelStates[i] = lsDisabled then
        Root.removeChild(ChildrenNL[i])
      else
      begin
        LevelState := Ord(self.LevelStates[i]);
        (ChildrenNL[i] as IXMLDOMElement).setAttribute(attrLevelState, LevelState);
      end;
  end;

var
  Root: IXMLDOMNode;
begin
  if not Assigned(DOM) then
    exit;
  {Т.к. все элементы, не предназначенные для размещения, удаляются из XML, а значит
  и признаки присущие элементу тоже удаляются, было решено сохранять некоторые из
  них в отдельной секции. Делалось в рамках задачи 3231, перед удалением элементов
  сохраним их зависимости в отдельном узле, что бы при синхронизации хранящейся
  в листе XML-ки с полученной с сервера, проставить influence}
  CopyInfluences(DOM);
  Root := DOM.selectSingleNode('function_result/Members');
  FilterMembers(Root, Root, 0);
  Root := DOM.selectSingleNode('function_result/Levels');
  FilterLevels(Root);
end;

procedure TAddinMembersTree.SetDomLevelState(LevelIndex: integer;
  LevelState: TLevelState);
var
  LevelNode: IXMLDOMNode;
begin
  LevelNode := GetLevelNodeByIndex(FMembersDom, LevelIndex);
  if Assigned(LevelNode) then
    (LevelNode as IXMLDOMElement).setAttribute(attrLevelState, LevelState);
end;

function TAddinMembersTree.GetMemberName(Node: IXMLDOMNode): string;
begin
  result := GetStrAttr(Node, 'name', 'не задано');
  if NeedShowCode then
    result := GetCodeString(Node) + result;
end;

function TAddinMembersTree.GetCodeString(Node: IXMLDOMNode): string;
var
  tmpFloat: double;
begin
  result := '';
  if not NeedShowCode then
    exit;
  result := GetStrAttr(Node, FCodeToShow, '');
  if IsNumber(result) then
  try
    tmpFloat := StrToFloat(result);
    result := MemberBrackets(result);
    if tmpFloat < 0 then
      result := '';
  except
    result := '';
  end
  else
    result := '';
  AddTail(result, ' ');
end;

function TAddinMembersTree.GetDefaultValue: string;
begin
  result := '';
  if not WithDefaultValue then
    exit;
  if Assigned(FDefaultNode) then
    if Assigned(FDefaultNode.DomNode) then
      result := GetStrAttr(FDefaultNode.DomNode, attrUniqueName, '');
end;

procedure TAddinMembersTree.CustomDrawItem(Sender: TCustomTreeView;
  Node: TTreeNode; State: TCustomDrawState; var DefaultDraw: Boolean);
begin
  if FWithDefaultValue and Assigned(FDefaultNode) then
    if FDefaultNode = Node then
      Sender.Canvas.Font.Style := Sender.Canvas.Font.Style + [fsUnderline]
    else
      Sender.Canvas.Font.Style := Sender.Canvas.Font.Style - [fsUnderline];
end;

procedure TAddinMembersTree.SetWithDefaultValue(const Value: boolean);
begin
  FWithDefaultValue := Value;
  FPopup.Free;
  CreatePopup;
end;

procedure TAddinMembersTree.RestoreDefaultValue;
var
  i: integer;
  DomNode: IXMLDOMNode;
begin
  if not WithDefaultValue then
    exit;
  { Ищем во вновь загруженном документе дефолтный узел. Если такого нет,
    то берем самый первый узел в дереве.}
  DomNode := MembersDOM.selectSingleNode(
    Format('function_result/Members//Member[@%s="true"]', [attrDefaultValue]));
  if not Assigned(DomNode) then
  begin
    FDefaultNode := TCheckTreeNode2(FTreeView.Items[0]);
    FDefaultNode.DomElement.setAttribute(attrDefaultValue, 'true');
    exit;
  end;
  { Если есть, то ищем узел дерева, содержащий ссылку на этот элемент.}
  for i := 0 to FTreeView.Items.Count - 1 do
    if TCheckTreeNode2(FTreeView.Items[i]).DomNode.xml = DomNode.xml then
    begin
      FDefaultNode := TCheckTreeNode2(FTreeView.Items[i]);
      break;
    end;
end;

procedure TAddinMembersTree.SetDefaultNode(NewDefaultNode: TCheckTreeNode2);
begin
  if Assigned(FDefaultNode) then
    FDefaultNode.DomNode.attributes.removeNamedItem(attrDefaultValue);
  FDefaultNode := NewDefaultNode;
  if Assigned(FDefaultNode) then
    FDefaultNode.DomElement.setAttribute(attrDefaultValue, 'true');
end;

function TAddinMembersTree.GetCheckedCount: integer;
begin
  try
    result := FMembersDom.selectNodes(
      Format('function_result/Members//Member[@%s="true"]', [attrChecked])).length
  except
    result := 0;
  end;
end;

{ TAddinMembersRadioTree }

constructor TAddinMembersRadioTree.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  Parent := AOwner as TWinControl;
  Height := 200;
  Width := 445;
  BevelInner := bvNone;
  BevelOuter := bvNone;
  FMembersDOM := nil;

  FTreeView := TBasicCheckTreeView.Create(Self);
  FTreeView.Parent := Self;
  FTreeView.Align := alClient;
  FTreeView.ReadOnly := true;
  FTreeView.Images := FImages;
  FTreeView.ShowButtons := true;
  FTreeView.OnNodeCheck := OnTreeNodeCheck;
  FTreeView.IsRadio := true;
  FCheckedNode := nil;
end;

function TAddinMembersRadioTree.GetImageList: TCustomImageList;
begin
  result := FImages;
end;

procedure TAddinMembersRadioTree.SetImageList(const Value: TCustomImageList);
begin
  if not Assigned(Value) then
    exit;
  FImages := Value;
  if Assigned(FTreeView) then
    FTreeView.Images := Value;
end;

function TAddinMembersRadioTree.Load(Dom: IXMLDOMDocument2): boolean;
var
  NL: IXMLDOMNodeList;
  i: integer;
begin
  FLoading := true;
  FTreeView.Items.Clear;
  FCheckedNode := nil;
  result := true;
  try
    FMembersDOM := DOM;
    NL := FMembersDOM.selectNodes('function_result/Members/Member[@name]');
    for i := 0 to NL.length - 1 do
      LoadMemberNode(NL[i], nil);
    {после окончания загрузки обновим иконки для всех узлов}
    //UpdateNodeImages;
    FTreeView.UpdateStateImages;
    //раскроем первый уровень
    FTreeView.Items[0].Expand(false);
    FLoading := false;
  except
    result := false;
  end;

end;

procedure TAddinMembersRadioTree.LoadMemberNode(XmlNode: IXMLDOMNode;
  TreeNode: TTreeNode);
var
  NewTreeNode: TBasicCheckTreeNode;
  NL: IXMLDOMNodeList;
  j: integer;
  Forbidden: boolean;
begin
  if Assigned(XmlNode) then
  begin
    NewTreeNode := FTreeView.Items.AddChild(TreeNode,
      GetStrAttr(XmlNode, attrName, 'не задано')) as TBasicCheckTreeNode;
    with NewTreeNode do
    begin
      RelatedDomNode := XmlNode;
      Checked := GetBoolAttr(XmlNode, attrChecked, true);
      if Checked then
        if not Assigned(FCheckedNode) then
          FCheckedNode := NewTreeNode
        else
        begin
          Checked := false;
          SetAttr(RelatedDomNode, attrChecked, 'false');
        end;
      Forbidden := GetBoolAttr(XmlNode, attrForbidCheck, false);
    end;
    NewTreeNode.HasChildren := XmlNode.hasChildNodes;
    if NewTreeNode.HasChildren then
    begin
      NewTreeNode.ImageIndex := IIF(Forbidden, FForbiddenRoyalIndex, FRoyalIndex);
      NL := XmlNode.childNodes;
      for j := 0 to NL.length - 1 do
        LoadMemberNode(NL[j], NewTreeNode);
    end
    else
      NewTreeNode.ImageIndex := IIF(Forbidden, FForbiddenSimpleIndex, FSimpleIndex);
    NewTreeNode.SelectedIndex := NewTreeNode.ImageIndex;
  end;
end;

destructor TAddinMembersRadioTree.Destroy;
begin
  FMembersDOM := nil;
  FTreeView.Items.Clear;
  FTreeView.Free;
  inherited Destroy;
end;

procedure TAddinMembersRadioTree.SetImageIndices(Simple, Royal, ForbiddenSimple, ForbiddenRoyal: integer);
begin
  if not Assigned(Images) then
    exit;
  if (Simple >= 0) and (Simple < Images.Count) then
    FSimpleIndex := Simple;
  if (Royal >= 0) and (Royal < Images.Count) then
    FRoyalIndex := Royal;
  if (ForbiddenSimple >= 0) and (ForbiddenSimple < Images.Count) then
    FForbiddenSimpleIndex := ForbiddenSimple;
  if (ForbiddenRoyal >= 0) and (ForbiddenRoyal < Images.Count) then
    FForbiddenRoyalIndex := ForbiddenRoyal;
end;

procedure TAddinMembersRadioTree.UpdateNodeImages;
var
  i: integer;
  Node: TBasicCheckTreeNode;
begin
  for i := 0 to FTreeView.Items.Count - 1 do
  begin
    FTreeView.Items[i].ImageIndex := FSimpleIndex;
    FTreeView.Items[i].SelectedIndex := FSimpleIndex;
  end;

  {Заведомо некорректная ситуация..}
  if not Assigned(FCheckedNode) then
    exit;

  Node := TBasicCheckTreeNode(FCheckedNode.Parent);
  while Assigned(Node) do
  begin
    Node.ImageIndex := FRoyalIndex;
    Node.SelectedIndex := FRoyalIndex;
    Node := TBasicCheckTreeNode(Node.Parent);
  end;
end;

procedure TAddinMembersRadioTree.OnTreeNodeCheck(Sender: Tobject);
begin
  if FLoading then
    exit;
  SetAttr(FCheckedNode.RelatedDomNode, attrChecked, 'false');
  FCheckedNode := TBasicCheckTreeNode(Sender);
  SetAttr(FCheckedNode.RelatedDomNode, attrChecked, 'true');
  (*UpdateNodeImages;*) // ЗАЧЕМ ТУТ ЭТО??
end;

function TAddinMembersTree.GetNeedShowCode: boolean;
begin
  result := FCodeToShow <> '';
end;

procedure TAddinMembersTree.SetIsFilterBehavior(const Value: boolean);
begin
  FIsFilterBehavior := Value;
  FTreeView.IsFilterBehavior := Value;
end;

function TAddinMembersRadioTree.GetMayCheck: TTVMayCheck;
begin
  result := nil;
  if Assigned(FTreeView) then
    result := FTreeView.MayCheck;
end;

procedure TAddinMembersRadioTree.SetMayCheck(const Value: TTVMayCheck);
begin
  if not Assigned(FTreeView) then
    exit;
  FTreeView.MayCheck := Value;
end;

end.

