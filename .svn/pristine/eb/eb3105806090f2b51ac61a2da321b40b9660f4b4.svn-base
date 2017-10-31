unit uSheetInfo;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, uCheckTV2, ExtCtrls, StdCtrls,
  uSheetObjectModel, ImgList;

type
  TfmSheetInfo = class(TForm)
    pnButtons: TPanel;
    btnOK: TButton;
    btnCancel: TButton;
    Bevel1: TBevel;
    PageControl1: TPageControl;
    tsComponents: TTabSheet;
    tsReport: TTabSheet;
    tvSheet: TBasicCheckTreeView;
    imgList: TImageList;
  private
    SheetInterface: TSheetInterface;

    procedure FillCubes(Root: TBasicCheckTreeNode);
    procedure FillCubesFromCollection(List: TStringList; Collection: TSheetCollection);
    procedure FillTotals(Root: TBasicCheckTreeNode; IsForTotals: boolean);
    procedure FillDimensions(Root: TBasicCheckTreeNode);
    procedure FillDimensionsFromCollection(List: TStringList; Collection: TSheetCollection);
    //procedure FillParameters(Root: TBasicCheckTreeNode);
    procedure BuildSheetTree;
  public
  end;


  procedure ViewSheetInfo(SheetIntf: TSheetInterface);

implementation

{$R *.DFM}

procedure ViewSheetInfo;
var
  SheetInfo: TfmSheetInfo;
begin
  SheetInfo := TfmSheetInfo.Create(nil);
  with SheetInfo do
  begin
    SheetInterface := SheetIntf;
    BuildSheetTree;
    PageControl1.ActivePage := tsComponents;
    ShowModal;
  end;
  FreeAndNil(SheetInfo);
end;

{ TfmSheetInfo }

procedure TfmSheetInfo.BuildSheetTree;
var
  RootNode: TBasicCheckTreeNode;
  NodeText: string;
begin
  tvSheet.Items.Clear;

  NodeText := Format('Лист (версия %s)', [SheetInterface.SheetVersion]);
  RootNode := tvSheet.Items.Add(nil, NodeText) as TBasicCheckTreeNode;
  RootNode.ImageIndex := 0;
  RootNode.SelectedIndex := 0;

  FillCubes(RootNode);
  FillTotals(RootNode, true);
  FillTotals(RootNode, false);
  FillDimensions(RootNode);
  RootNode.Expand(false);
end;

procedure TfmSheetInfo.FillCubes(Root: TBasicCheckTreeNode);
var
  List: TStringList;
  i: integer;
  Node: TBasicCheckTreeNode;
begin
  Root := tvSheet.Items.AddChild(Root, 'Кубы') as TBasicCheckTreeNode;
  Root.ImageIndex := 23;
  Root.SelectedIndex := 23;

  List := TStringList.Create;
  FillCubesFromCollection(List, SheetInterface.Totals);
  FillCubesFromCollection(List, SheetInterface.SingleCells);

  for i := 0 to List.Count - 1 do
  begin
    Node := tvSheet.Items.AddChild(Root, List[i]) as TBasicCheckTreeNode;
    Node.ImageIndex := 23;
    Node.SelectedIndex := 23;
  end;
  FreeAndNil(List);

  Root.Text := Format('Кубы (%d)', [Root.Count]);
  Root.AlphaSort;
end;

procedure TfmSheetInfo.FillCubesFromCollection(List: TStringList; Collection: TSheetCollection);
var
  i: integer;
  NodeText: string;
  Total: TSheetBasicTotal;
begin
  for i := 0 to Collection.Count - 1 do
  begin
    Total := Collection[i];
    if not (Total.TotalType in [wtMeasure, wtResult]) then
      continue;
    NodeText := Total.CubeName;
    if not Assigned(Total.Cube) then
      NodeText := NodeText + ' (не рассчитан)';
    if List.IndexOf(NodeText) < 0 then
      List.Add(NodeText);
  end;
end;

procedure TfmSheetInfo.FillDimensions(Root: TBasicCheckTreeNode);
var
  Node: TBasicCheckTreeNode;
  i: integer;
  List: TStringList;
begin
  Root := tvSheet.Items.AddChild(Root, 'Измерения') as TBasicCheckTreeNode;
  Root.ImageIndex := 2;
  Root.SelectedIndex := 2;

  List := TStringList.Create;
  FillDimensionsFromCollection(List, SheetInterface.Rows);
  FillDimensionsFromCollection(List, SheetInterface.Columns);
  FillDimensionsFromCollection(List, SheetInterface.Filters);
  for i := 0 to SheetInterface.SingleCells.Count - 1 do
    FillDimensionsFromCollection(List, SheetInterface.SingleCells[i].Filters);

  for i := 0 to List.Count - 1 do
  begin
    Node := tvSheet.Items.AddChild(Root, List[i]) as TBasicCheckTreeNode;
    Node.ImageIndex := 2;
    Node.SelectedIndex := 2;
  end;

  Root.Text := Format('Измерения (%d)', [Root.Count]);
  Root.AlphaSort;
end;

procedure TfmSheetInfo.FillDimensionsFromCollection(List: TStringList; Collection: TSheetCollection);
var
  i, Index: integer;
  Name: string;
  Dimension: TSheetDimension;
begin
  for i := 0 to Collection.Count - 1 do
  begin
    Dimension := TSheetDimension(Collection[i]);
    Name := Dimension.GetElementCaption;
    if Dimension.IsParam then
      Name := Format('%s (параметр "%s")', [Name, Dimension.Param.FullName]);
    Index := List.IndexOf(Name);
    if Index = -1 then
      List.Add(Name);
  end;
end;

(*procedure TfmSheetInfo.FillParameters(Root: TBasicCheckTreeNode);
begin
  Root := tvSheet.Items.AddChild(Root, 'Измерения') as TBasicCheckTreeNode;
  Root.ImageIndex := 2;
  Root.SelectedIndex := 2;
end;*)

procedure TfmSheetInfo.FillTotals(Root: TBasicCheckTreeNode; IsForTotals: boolean);
var
  Node: TBasicCheckTreeNode;
  NodeText: string;
  Total: TSheetBasicTotal;
  T: TSheetTotalType;
  Roots: array[TSheetTotalType] of TBasicCheckTreeNode;
  i, ImageIndex: integer;
  Collection: TSheetCollection;
begin
  if IsForTotals then
  begin
    Collection := SheetInterface.Totals;
    NodeText := 'Показатели в таблице (%d)';
    ImageIndex := 1;
  end
  else
  begin
    Collection := SheetInterface.SingleCells;
    NodeText := 'Отдельные показатели (%d)';
    ImageIndex := 4;
  end;

  NodeText := Format(NodeText, [Collection.Count]);
  Node := tvSheet.Items.AddChild(Root, NodeText) as TBasicCheckTreeNode;
  Node.ImageIndex := ImageIndex;
  Node.SelectedIndex := ImageIndex;

  if Collection.Count = 0 then
    exit;

  Roots[wtMeasure] := tvSheet.Items.AddChild(Node, 'Меры из кубов (%d)') as TBasicCheckTreeNode;
  Roots[wtResult] := tvSheet.Items.AddChild(Node, 'Результаты расчета (%d)') as TBasicCheckTreeNode;
  if IsForTotals then
    Roots[wtFree] := tvSheet.Items.AddChild(Node, 'Свободные (%d)') as TBasicCheckTreeNode
  else
    Roots[wtFree] := nil;
  Roots[wtConst] := tvSheet.Items.AddChild(Node, 'Константы (%d)') as TBasicCheckTreeNode;

  for i := 0 to Collection.Count - 1 do
  begin
    Total := Collection[i];

    case Total.TotalType of
      wtMeasure, wtResult:
        NodeText := Format('%s ("%s")', [Total.GetElementCaption, Total.CubeName]);

      wtFree, wtConst:
        NodeText := Format('%s', [Total.GetElementCaption]);
    end;

    Node := tvSheet.Items.AddChild(Roots[Total.TotalType], NodeText) as TBasicCheckTreeNode;
    Node.ImageIndex := ImageIndex;
    Node.SelectedIndex := ImageIndex;
    if Assigned(Total.Measure) then
      if Total.Measure.IsCalculated then
      begin
        Node.ImageIndex := 5;
        Node.SelectedIndex := 5;
      end;
  end;

  for T := Low(TSheetTotalType) to High(TSheetTotalType) do
  begin
    Node := Roots[T];
    if not Assigned(Node) then
      continue;
    NodeText := Node.Text;
    NodeText := Format(NodeText, [Node.Count]);
    Node.Text := NodeText;
    if IsForTotals then
    begin
      Node.ImageIndex := 1;
      Node.SelectedIndex := 1;
    end
    else
    begin
      Node.ImageIndex := 4;
      Node.SelectedIndex := 4;
    end;
    Node.AlphaSort;
  end;
end;

end.
