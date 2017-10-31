unit uTableStructure;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, AddinDimensionsTree, uSheetObjectModel, ImgList,
  uCheckTV2, Buttons, fmSpeedButton, uFMAddinGeneralUtils, uXmlCatalog;

type


  TSiteType = (stFilters, stRows, stColumns, stTotals);

  TColorButton = class(TButton)
  published
    property Color;
  end;

  TElementSite = class;

  TElement = class
  private
    FOwner: TElementSite;
    {ссылка на узел в дереве элементов}
    FTreeNode: TBasicCheckTreeNode;
    FButton: TColorButton;
    function GetCaption: string;
  public
    constructor Create(AOwner: TElementSite);
    destructor Destroy; override;
    function GetSelfIndex: integer;
    function GetButtonTop: integer;
    procedure ButtonClick(Sender: TObject);

    property Owner: TElementSite read FOwner;
    property Caption: string read GetCaption;
    property TreeNode: TBasicCheckTreeNode read FTreeNode write FTreeNode;
    property Button: TColorButton read FButton write FButton;
  end;

  TElementSite = class(TList)
  private
    FSiteType: TSiteType;
    FSiteHolder: TScrollBox;
  protected
    function GetItem(Index: integer): TElement;
    procedure SetItem(Index: integer; Value: TElement);
    {возвращает верхнюю координату кнопки элемента}
    function GetButtonTop(Index: integer): integer;
  public
    constructor Create(SiteType: TSiteType; SiteHolder: TScrollBox);
    destructor Destroy; override;
    procedure Clear; override;
    procedure Delete(Index: integer); virtual;
    function Add(Node: TBasicCheckTreeNode): TElement;
    //function IndexOf(Name: string): integer;
    //function Find(Name: string): TBaseElement;

    property SiteType: TSiteType read FSiteType;
    property Items[Index: integer]: TElement read GetItem write SetItem; default;
    property SiteHolder: TScrollBox read FSiteHolder;
  end;

  TfmTableStructure = class(TForm)
    btnCancel: TButton;
    btnOk: TButton;
    CubeDimTree: TCubeDimTree;
    ilImages: TImageList;
    Panel1: TPanel;
    sbFilters: TScrollBox;
    sbColumns: TScrollBox;
    sbRows: TScrollBox;
    sbTotals: TScrollBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    ListBox1: TListBox;
    procedure sbFiltersDragOver(Sender, Source: TObject; X, Y: Integer;
      State: TDragState; var Accept: Boolean);
    procedure sbFiltersDragDrop(Sender, Source: TObject; X, Y: Integer);
    procedure ListBox1DragDrop(Sender, Source: TObject; X, Y: Integer);
    procedure ListBox1DragOver(Sender, Source: TObject; X, Y: Integer;
      State: TDragState; var Accept: Boolean);
  private
    { Private declarations }
    FSheetInterface: TSheetInterface;
    DraggingNode: TBasicCheckTreeNode;
    FiltersSite, RowsSite, ColumnsSite, TotalsSite: TElementSite;

    procedure CubeDimTreeStartDrag(Sender: TObject; var DragObject: TDragObject);
    {пригоден ли сайт для дропа измерений}
    function IsSiteDimensional(Site: TControl): boolean;
  public
    { Public declarations }
    procedure Init;

    property SheetInterface: TSheetInterface read FSheetInterface
      write FSheetInterface;
  end;

function EditTableStructure(SheetInterface: TSheetInterface): boolean;

implementation

{$R *.DFM}

const
  ButtonHeight = 22;
  ButtonSplit = 8;

{возвращает обобщенный тип - мера или измерение}
function GetNodeCommonType(Node: TBasicCheckTreeNode): TDimTreeNodeType;
begin
  if (Node.NodeType = Ord(ntDimension)) or
    (Node.NodeType = Ord(ntHierarchy)) then
    result := ntDimension
  else
    if (Node.NodeType = Ord(ntMeasure)) or
      (Node.NodeType = Ord(ntCalcMeasure)) then
      result := ntMeasure
    else
      result := ntNone;
end;

{}
function GetMeasureCaption(MeasureNode: TBasicCheckTreeNode): string;
var
  Measure: TMeasure;
  Cube: TCube;
begin
  Measure := TMeasure(MeasureNode.Data);
  Cube := TCube(MeasureNode.Parent.Data);
  result := Cube.Name + '.' + Measure.Name;
end;



function EditTableStructure(SheetInterface: TSheetInterface): boolean;
var
  fmTableStructure: TfmTableStructure;
begin
  fmTableStructure := TfmTableStructure.Create(nil);
  fmTableStructure.SheetInterface := SheetInterface;
  fmTableStructure.Init;
  result := fmTableStructure.ShowModal = mrOk;
  FreeAndNil(fmTableStructure);
end;

{ TfmTableStructure }

procedure TfmTableStructure.CubeDimTreeStartDrag(Sender: TObject;
  var DragObject: TDragObject);
var
  Point: TPoint;
  NodeType: TDimTreeNodeType;
begin
  Point := CubeDimTree.ScreenToClient(Mouse.CursorPos);
  DraggingNode := CubeDimTree.GetNodeAtCursor;
  if not Assigned(DraggingNode) then
    exit;
  NodeType := TDimTreeNodeType(DraggingNode.NodeType);
  if (NodeType in [ntMeasure, ntCalcMeasure, ntDimension, ntHierarchy]) then
    exit;
  CancelDrag;
  DraggingNode := nil;
end;

procedure TfmTableStructure.Init;
begin
  with CubeDimTree do
  begin
    OnStartDrag := CubeDimTreeStartDrag;
    Catalog := SheetInterface.XMLCatalog;
    Images := ilImages;
    ImageIndexes[ntCube] := 0;
    ImageIndexes[ntDimension] := 2;
    ImageIndexes[ntHierarchy] := 3;
    ImageIndexes[ntSemantics] := 0;
    ImageIndexes[ntMeasure] := 1;
    ImageIndexes[ntCalcMeasure] := 23;
    Load;
  end;
  FiltersSite := TElementSite.Create(stFilters, sbFilters);
  RowsSite := TElementSite.Create(stRows, sbRows);
  ColumnsSite := TElementSite.Create(stColumns, sbColumns);
  TotalsSite := TElementSite.Create(stTotals, sbTotals);
end;

procedure TfmTableStructure.sbFiltersDragOver(Sender, Source: TObject; X,
  Y: Integer; State: TDragState; var Accept: Boolean);
begin
  if IsSiteDimensional(TControl(Sender)) then
    Accept := (GetNodeCommonType(DraggingNode) = ntDimension)
  else
    Accept := (GetNodeCommonType(DraggingNode) = ntMeasure);
end;

procedure TfmTableStructure.sbFiltersDragDrop(Sender, Source: TObject; X,
  Y: Integer);
var
  Site: TElementSite;
begin
  Site := TElementSite((Sender as TScrollBox).Tag);
  Site.Add(DraggingNode);
  DraggingNode := nil;
end;

function TfmTableStructure.IsSiteDimensional(Site: TControl): boolean;
begin
  result := (Site.Name = 'sbFilters') or (Site.Name = 'sbRows') or
    (Site.Name = 'sbColumns');
end;

{ TElement }

procedure TElement.ButtonClick(Sender: TObject);
begin
  ShowInfo(Caption);
end;

constructor TElement.Create(AOwner: TElementSite);
begin
  FOwner := AOwner;
end;

destructor TElement.Destroy;
begin
  FOwner := nil;
  FreeAndNil(FButton);
  inherited;
end;

function TElement.GetButtonTop: integer;
begin
  result := ButtonSplit + (ButtonHeight + ButtonSplit) * GetSelfIndex;
end;

function TElement.GetCaption: string;
begin
  result := '';
  result := TreeNode.Text;
  if GetNodeCommonType(TreeNode) = ntMeasure then
    result := GetMeasureCaption(TreeNode)
  else
    if GetNodeCommonType(TreeNode) = ntDimension then
      result := TDimension(TreeNode.Data).UniqueName
end;

function TElement.GetSelfIndex: integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to FOwner.Count - 1 do
    if Owner[i] = Self then
    begin
      result := i;
      exit;
    end;
end;

{ TElementSite }

function TElementSite.Add(Node: TBasicCheckTreeNode): TElement;
begin
  result := TElement.Create(Self);
  result.TreeNode := Node;
  inherited Add(result);
  result.Button := TColorButton.Create(nil);
  with result.Button do
  begin
    Color := clRed;

    Top := GetButtonTop(Count - 1);
    Left := ButtonSplit;
    Width := SiteHolder.Width - 5 * ButtonSplit;
    Parent := SiteHolder;
    Caption := result.Caption;
    Tag := integer(result);
    OnClick := result.ButtonClick;
  end;
end;

procedure TElementSite.Clear;
begin
  while Count > 0 do
    Delete(0);
end;

constructor TElementSite.Create(SiteType: TSiteType; SiteHolder: TScrollBox);
begin
  inherited Create;
  FSiteType := SiteType;
  FSiteHolder := SiteHolder;
  SiteHolder.Tag := longint(Self);
end;

procedure TElementSite.Delete(Index: integer);
begin
  inherited Delete(Index);
end;

destructor TElementSite.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TElementSite.GetButtonTop(Index: integer): integer;
begin
  result := ButtonSplit + (ButtonHeight + ButtonSplit) * Index;
end;

function TElementSite.GetItem(Index: integer): TElement;
begin
  result := Get(Index);
end;

procedure TElementSite.SetItem(Index: integer; Value: TElement);
begin
  Put(Index, Value);
end;

procedure TfmTableStructure.ListBox1DragDrop(Sender, Source: TObject; X,
  Y: Integer);
begin
  ListBox1.Items.Add(DraggingNode.Text);
  DraggingNode := nil;
end;

procedure TfmTableStructure.ListBox1DragOver(Sender, Source: TObject; X,
  Y: Integer; State: TDragState; var Accept: Boolean);
begin
  Accept := true;
end;

end.
