unit uStyleManager;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls, uCheckTV2, uSheetObjectModel, ExcelXP, OfficeXP,
  uFMExcelAddinConst, uFMAddinGeneralUtils, CommCtrl, uExcelUtils, Buttons,
  ExtCtrls, uGlobalPlaningConst, uSheetStyles, uFMAddinExcelUtils;

type
  TStyleManager = class(TForm)
    btnOK: TButton;
    btnCancel: TButton;
    Bevel1: TBevel;
    Panel3: TPanel;
    Panel4: TPanel;
    Splitter1: TSplitter;
    Panel5: TPanel;
    Panel2: TPanel;
    tvElements: TBasicCheckTreeView;
    Label1: TLabel;
    Panel1: TPanel;
    tvStyles: TBasicCheckTreeView;
    Label2: TLabel;
    btnDefault: TButton;
    procedure tvElementsChange(Sender: TObject; Node: TTreeNode);
    procedure tvStylesChange(Sender: TObject; Node: TTreeNode);
    procedure tvElementsCustomDrawItem(Sender: TCustomTreeView;
      Node: TTreeNode; State: TCustomDrawState; var DefaultDraw: Boolean);
    procedure tvStylesCustomDrawItem(Sender: TCustomTreeView;
      Node: TTreeNode; State: TCustomDrawState; var DefaultDraw: Boolean);
    procedure btnDefaultClick(Sender: TObject);
  private
    SheetInterface: TSheetInterface;
    ExcelSheet: ExcelWorkSheet;
    {флаг допустимости реакции на событие - смену стиля}
    StyleChangeAllowed: boolean;
    FRestyleList: TStringList;
    procedure LoadElement(Element: TSheetElement; Node: TTreeNode);
    {загрузка дерева элементов}
    procedure LoadElements;
    {загрузка списка стилей текущей книги}
    function LoadStyles: boolean;
    {проверка на то, что узел сопоставлен со стилем элемента}
    function IsStyleNode(Node: TTreeNode): boolean;
    {аналогично, но для стиля всей коллекции}
    function IsFakeStyleNode(Node: TTreeNode): boolean;
    function GetElementStyleName(Node: TTreeNode): string;
    {ссылка на коллекцию стилей активной книги}
    function GetBookStyles: Styles;

    property BookStyles: Styles read GetBookStyles;
  public
  end;

  {RestyleList содержит строки вида UniqueId=TSheetElementType}
  function ShowStyleManager(SheetIntf: TSheetInterface;
    var RestyleList: TStringList): boolean;


implementation

{$R *.DFM}

function ShowStyleManager(SheetIntf: TSheetInterface;
    var RestyleList: TStringList): boolean;
var
  StyleManager: TStyleManager;
begin
  result := false;
  StyleManager := TStyleManager.Create(nil);
  try
    with StyleManager do
    begin
      SheetInterface := SheetIntf;
      ExcelSheet := SheetInterface.ExcelSheet;
      LoadElements;
      if not LoadStyles then
        exit;
      tvElementsChange(nil, nil);
      if not Assigned(RestyleList) then
        RestyleList := TStringList.Create;
      RestyleList.Sorted := true;
      RestyleList.Duplicates := dupIgnore;
      FRestyleList := RestyleList;
      result := ShowModal = mrOK;
    end;
  finally
    FreeAndNil(StyleManager);
  end;
end;

{ TStyleManager }

procedure TStyleManager.LoadElement(Element: TSheetElement; Node: TTreeNode);
var
  ElementNode: TTreeNode;
  ElemCaption: string;
  ObjType: TSheetObjectType;
begin
  ObjType := Element.Owner.ObjectType;
  ElementNode := tvElements.Items.AddChild(Node, Element.GetElementCaption);
  ElementNode.Data := Element;
  case ObjType of
    wsoTotal, wsoSingleCell: ElemCaption := 'Данные';
    wsoRow, wsoColumn: ElemCaption := 'Элементы';
    wsoFilter: ElemCaption := 'Значение';
  end;
  tvElements.Items.AddChild(ElementNode, ElemCaption);
  case ObjType of
    wsoTotal, wsoSingleCell: ElemCaption := 'Данные (печать)';
    wsoRow, wsoColumn: ElemCaption := 'Элементы (печать)';
    wsoFilter: ElemCaption := 'Значение (печать)';
  end;
  tvElements.Items.AddChild(ElementNode, ElemCaption);
  if ObjType = wsoSingleCell then
    exit;
  tvElements.Items.AddChild(ElementNode, 'Заголовок');
  tvElements.Items.AddChild(ElementNode, 'Заголовок (печать)');
end;

procedure TStyleManager.LoadElements;
var
  i: integer;
  Node: TTreeNode;
begin
  tvElements.Items.Clear;

  Node := tvElements.Items.Add(nil, 'Показатели в таблице');
  Node.Data := SheetInterface.Totals;
  tvElements.Items.AddChild(Node, 'Все данные');
  tvElements.Items.AddChild(Node, 'Все данные (печать)');
  tvElements.Items.AddChild(Node, 'Все заголовки');
  tvElements.Items.AddChild(Node, 'Все заголовки (печать)');
  for i := 0 to SheetInterface.Totals.Count - 1 do
    LoadElement(SheetInterface.Totals[i], Node);

  Node := tvElements.Items.Add(nil, 'Строки');
  Node.Data := SheetInterface.Rows;
  tvElements.Items.AddChild(Node, 'Все элементы');
  tvElements.Items.AddChild(Node, 'Все элементы (печать)');
  tvElements.Items.AddChild(Node, 'Все заголовки');
  tvElements.Items.AddChild(Node, 'Все заголовки (печать)');
  for i := 0 to SheetInterface.Rows.Count - 1 do
    LoadElement(SheetInterface.Rows[i], Node);

  Node := tvElements.Items.Add(nil, 'Столбцы');
  Node.Data := SheetInterface.Columns;
  tvElements.Items.AddChild(Node, 'Все элементы');
  tvElements.Items.AddChild(Node, 'Все элементы (печать)');
  tvElements.Items.AddChild(Node, 'Все заголовки');
  tvElements.Items.AddChild(Node, 'Все заголовки (печать)');
  for i := 0 to SheetInterface.Columns.Count - 1 do
    LoadElement(SheetInterface.Columns[i], Node);

  {грузим только общие фильтры}
  Node := tvElements.Items.Add(nil, 'Фильтры');
  Node.Data := SheetInterface.Filters;
  tvElements.Items.AddChild(Node, 'Все значения');
  tvElements.Items.AddChild(Node, 'Все значения (печать)');
  tvElements.Items.AddChild(Node, 'Все заголовки');
  tvElements.Items.AddChild(Node, 'Все заголовки (печать)');
  for i := 0 to SheetInterface.Filters.Count - 1 do
    if not SheetInterface.Filters[i].IsPartial then
      LoadElement(SheetInterface.Filters[i], Node);

  Node := tvElements.Items.Add(nil, 'Отдельные показатели');
  Node.Data := SheetInterface.SingleCells;
  tvElements.Items.AddChild(Node, 'Все данные');
  tvElements.Items.AddChild(Node, 'Все данные (печать)');
  for i := 0 to SheetInterface.SingleCells.Count - 1 do
    LoadElement(SheetInterface.SingleCells[i], Node);
end;

function TStyleManager.LoadStyles: boolean;
var
  i: integer;
  Book: ExcelWorkBook;
begin
//  result := false;
  tvStyles.IsRadio := true;
  tvStyles.Items.BeginUpdate;
  try
    tvStyles.Items.Clear;
    Book := ExcelSheet.Parent as ExcelWorkBook;
    for i := 1 to Book.Styles.Count do
      tvStyles.Items.Add(nil, Book.Styles[i].Name);
  finally
    tvStyles.UpdateStateImages;
    tvStyles.Items.EndUpdate;
  end;
    result := true;
end;

procedure TStyleManager.tvElementsChange(Sender: TObject; Node: TTreeNode);
var
  StyleName: string;
  StyleNode: TBasicCheckTreeNode;
begin
  if IsStyleNode(Node) then
  begin
    tvStyles.Visible := true;
    StyleName := GetElementStyleName(Node);
    if StyleName = '' then
    begin
      tvStyles.Enabled := false;
      exit;
    end;
    tvStyles.Enabled := true;
    StyleChangeAllowed := false;
    StyleNode := tvStyles.FindNodeByName(StyleName);
    StyleNode.Checked := true;
    StyleNode.Selected := true;
    StyleChangeAllowed := true;
    exit;
  end;
  if IsFakeStyleNode(Node) then
  begin
    tvStyles.Enabled := true;
    tvStyles.UncheckAll(nil);
    tvStyles.Visible := true;
    StyleChangeAllowed := true;
    exit;
  end;
  tvStyles.Visible := false;
end;

procedure TStyleManager.tvStylesChange(Sender: TObject; Node: TTreeNode);
var
  StyleNode: TTreeNode;
  Element: TSheetElement;
  i: integer;
  OldStyle, NewStyle: Style;
begin
  if not StyleChangeAllowed then
    exit;
  StyleNode := tvElements.Selected;
  if IsStyleNode(StyleNode) then
  begin
    Element := TSheetElement(StyleNode.Parent.Data);
    OldStyle := BookStyles[Element.Styles.Name[TElementStyle(StyleNode.Index)]];
    NewStyle := BookStyles[Node.Text];
    NewStyle.Locked := true;
(*    if OldStyle.Locked and not NewStyle.Locked then
    begin
      ShowError('Вы пытаетесь применить незащищаемый стиль к области листа, ' +
        'редактирование которой запрещено');
      exit;
    end; *)
    Element.Styles.Name[TElementStyle(StyleNode.Index)] := Node.Text;
    Element.ApplyStyles;
    FRestyleList.Add(Element.UniqueID + '=' + IntToStr(Ord(Element.Owner.ObjectType)));
    TBasicCheckTreeNode(Node).Checked := true;
    tvElements.Refresh;
    exit;
  end;
  if IsFakeStyleNode(StyleNode) then
  begin
    with TSheetCollection(StyleNode.Parent.Data) do
      for i := 0 to Count - 1 do
      begin
        Element := TSheetElement(Items[i]);
        NewStyle := BookStyles[Node.Text];
        NewStyle.Locked := true;
(*        if not NewStyle.Locked and ((StyleNode.Parent.Text = 'Строки') or
          (StyleNode.Parent.Text = 'Столбцы') or (StyleNode.Parent.Text = 'Фильтры')) then
        begin
          ShowError('Вы пытаетесь применить незащищаемый стиль к области листа, ' +
            'редактирование которой запрещено');
          exit;
        end;*)
        Element.Styles.Name[TElementStyle(StyleNode.Index)] := Node.Text;
        Element.ApplyStyles;
        FRestyleList.Add(Element.UniqueID + '=' + IntToStr(Ord(Element.Owner.ObjectType)));
      end;
    TBasicCheckTreeNode(Node).Checked := true;
    tvElements.Refresh;
    exit;
  end;
end;

function TStyleManager.IsStyleNode(Node: TTreeNode): boolean;
begin
  result := false;
  if not Assigned(Node) then
    exit;
  if Node.HasChildren then
    exit;
  if Assigned(Node.Parent) then
    result := Assigned(Node.Parent.Data) and (TObject(Node.Parent.Data) is TSheetElement)
  else
    result := false;
end;


function TStyleManager.GetElementStyleName(Node: TTreeNode): string;
var
  ElementNode: TTreeNode;
  Element: TSheetElement;
begin
  result := '';
  if not IsStyleNode(Node) then
    exit;
  ElementNode := Node.Parent;
  Element := TSheetElement(ElementNode.Data);
  {предполагается, что у узла-элемента нет других потомков, кроме узлов-стилей}
  result := Element.Styles.Name[TElementStyle(Node.Index)];
end;


procedure TStyleManager.tvElementsCustomDrawItem(Sender: TCustomTreeView;
  Node: TTreeNode; State: TCustomDrawState; var DefaultDraw: Boolean);
var
  Element: TSheetElement;
begin
  if IsStyleNode(Node) then
  begin
     Element := TSheetElement(Node.Parent.Data);
     if Element.Styles.Name[TElementStyle(Node.Index)] <>
      Element.DefaultStyleName[TElementStyle(Node.Index)] then
        tvElements.Canvas.Font.Style := [fsUnderline, fsBold]
      else
        tvElements.Canvas.Font.Style := [fsUnderline];
    exit;
  end;
  if IsFakeStyleNode(Node) then
  begin
    tvElements.Canvas.Font.Style := [fsUnderline];
    exit;
  end;
  tvElements.Canvas.Font.Style := [];
end;

procedure TStyleManager.tvStylesCustomDrawItem(Sender: TCustomTreeView;
  Node: TTreeNode; State: TCustomDrawState; var DefaultDraw: Boolean);
begin
(*  with tvStyles.Canvas do
    if IsOurStyle(Node.Text) then
      if Node.Selected then
        Font.Color := clHighlightText
      else
        Font.Color := clBlue//clNavy
    else
      DefaultDraw := true;*)
end;

procedure TStyleManager.btnDefaultClick(Sender: TObject);

  procedure UpdateCollection(Collection: TSheetCollection);
  var
    i: integer;
    Element: TSheetElement;
  begin
    for i := 0 to Collection.Count - 1 do
    begin
      Element := TSheetElement(Collection[i]);
      Element.SetDefaultStyles;
      Element.ApplyStyles;
    end;
  end;

  procedure RemoveOurStyles;
  var
    i: integer;
  begin
    with SheetInterface.ExcelSheet.Application.ActiveWorkbook do
      for i := Styles.Count downto 1 do
        if IsOurStyle(Styles[i].Name) then
          Styles[i].Delete;
    CreateWorkbookStyles(SheetInterface.ExcelSheet.Application.ActiveWorkbook);
  end;

var
  ERange: ExcelRange;
begin
  with SheetInterface do
  try
    ExcelSheet.Application.ScreenUpdating[GetUserDefaultLCID] := false;
    SetBookProtection(ExcelSheet.Parent as ExcelWorkbook, false);
    RemoveOurStyles;
    UpdateCollection(Totals);
    UpdateCollection(Rows);
    UpdateCollection(Columns);
    UpdateCollection(Filters);
    UpdateCollection(SingleCells);
    ERange := GetRangeByName(ExcelSheet, BuildExcelName(sntSheetId));
    if Assigned(ERange) then
    begin
      ERange.Style := 'Normal';
      ERange.Style := snSheetId;
    end;
  finally
    ExcelSheet.Application.ScreenUpdating[GetUserDefaultLCID] := true;
    tvElements.Refresh;
  end;
end;

function TStyleManager.IsFakeStyleNode(Node: TTreeNode): boolean;
begin
  result := false;
  if not Assigned(Node) then
    exit;
  if Node.HasChildren then
    exit;
  if Assigned(Node.Parent) then
    result := Assigned(Node.Parent.Data) and
      (TObject(Node.Parent.Data) is TSheetCollection)
  else
    result := false;
end;

function TStyleManager.GetBookStyles: Styles;
begin
  result := ExcelSheet.Application.ActiveWorkbook.Styles;
end;

end.

