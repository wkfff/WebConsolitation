unit AddinMeasuresTree;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, uXMLCatalog, ImgList;

type

  TAddinMeasuresTree = class(TTreeView)
  private
    { Private declarations }
    FCatalog: TXMLCatalog;
    FCubeImg: integer;
    FMeasureImg: integer;
    FCalcMeasureImg: integer;
  protected
    { Protected declarations }
    function GetCube: TCube;
    function GetMeasure: TMeasure;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function Load(IsResult: boolean): boolean;
    function IsEmpty: boolean;
    {Проверка того, что выбрана именно мерa, а не куб.}
    function IsMeasureSelected: boolean;
    {пытается выделить указанную меру, возвращает успех/неуспех}
    function SetSelection(CubeName, MeasureName: string): boolean;
    property Cube: TCube read GetCube;
    property Measure: TMeasure read GetMeasure;
    property CubeImg: integer read FCubeImg write FCubeImg;
    property MeasureImg: integer read FMeasureImg write FMeasureImg;
    property CalcMeasureImg: integer read FCalcMeasureImg write FCalcMeasureImg;
    property Catalog: TXMLCatalog read FCatalog write FCatalog;
  published
    { Published declarations }
  end;

procedure Register;

{$R AddinMeasuresTree.dcr}

implementation

constructor TAddinMeasuresTree.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FCatalog := nil;
  FCubeImg := 0;
  FMeasureImg := 1;
  FCalcMeasureImg := 23;
  ReadOnly := true;
  HideSelection := false;
end;

destructor TAddinMeasuresTree.Destroy;
begin
  FCatalog := nil;
  inherited Destroy;
end;

function TAddinMeasuresTree.Load(IsResult: boolean): boolean;
var
  i, j: integer;
  Cube: TCube;
  Measure: TMeasure;
  CubeNode, MeasureNode: TTreeNode;
  CubeNodeCaption, MeasureNodeCaption: string;
begin
  result := false;
  Items.Clear;
  Items.BeginUpdate;
  if not Assigned(Catalog) then
    exit;
  for i := 0 to Catalog.Cubes.Count - 1 do
  begin
    Cube := Catalog.Cubes[i];
    //кубы без мер не включаем
    if Cube.Measures.Count = 0 then
      continue;
    { если добавляем результат, то не включаем кубы, не предназначенные для записи
     (fullName = '')  или (subClass <> вводу или сбору)
     запись возможна только в основную базу}
    if IsResult and (not Cube.WritebackPossible or (Cube.ProviderId <> Catalog.PrimaryProvider)) then
      continue;
    CubeNodeCaption := Cube.Name;
    if Catalog.InMultibaseMode then
      CubeNodeCaption := Format('[P_ID = %s] %s)', [Cube.ProviderId, CubeNodeCaption]);
    CubeNode := Items.AddChild(nil, CubeNodeCaption);
    CubeNode.ImageIndex := FCubeImg;
    CubeNode.SelectedIndex := FCubeImg;
    CubeNode.Data := Pointer(Cube);
    for j := 0 to Cube.Measures.Count - 1 do
    begin
      Measure := Cube.Measures[j];
      //для показателей - результатов можно использовать только хранимые меры
      if IsResult and Measure.IsCalculated then
        continue;
      case Measure.Format of
        fmtCurrency: MeasureNodeCaption := Measure.Name + ' ($)';
        fmtPercent: MeasureNodeCaption := Measure.Name + ' (%)';
        fmtText: MeasureNodeCaption := Measure.Name + ' (@)';
        else
          MeasureNodeCaption := Measure.Name;
      end;
      MeasureNode := Items.AddChild(CubeNode, MeasureNodeCaption);
      if Measure.IsCalculated then
      begin
        MeasureNode.ImageIndex := FCalcMeasureImg;
        MeasureNode.SelectedIndex := FCalcMeasureImg;
      end
      else
      begin
        MeasureNode.ImageIndex := FMeasureImg;
        MeasureNode.SelectedIndex := FMeasureImg;
      end;
      MeasureNode.Data := Pointer(Measure);
    end;
  end;
  Items.EndUpdate;
  result := Items.Count > 0;
end;

function TAddinMeasuresTree.GetCube: TCube;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(Selected) then
    exit;
  case Selected.Level of
    0: P := Selected.Data;
    1: P := Selected.Parent.Data;
  end;
  if Assigned(P) then
    result := TCube(P);
end;

function TAddinMeasuresTree.GetMeasure: TMeasure;
var
  P: Pointer;
begin
  result := nil;
  P := nil;
  if not Assigned(Selected) then
    exit;
  case Selected.Level of
    0: P := nil;
    1: P := Selected.Data;
  end;
  if Assigned(P) then
    result := TMeasure(P);
end;

function TAddinMeasuresTree.IsEmpty: boolean;
begin
  result := Items.Count = 0;
end;

function TAddinMeasuresTree.IsMeasureSelected: boolean;
begin
  result := Assigned(Measure);
end;

function TAddinMeasuresTree.SetSelection(CubeName,
  MeasureName: string): boolean;
var
  Node: TTreeNode;
begin
  result := false;
  Node := Items[0];
  while Assigned(Node) do
  begin
    if TCube(Node.Data).Name = CubeName then
    begin
      Node := Node.getFirstChild;
      while Assigned(Node) do
      begin
        if TMeasure(Node.Data).Name = MeasureName then
        begin
          Node.Selected := true;
          Node.MakeVisible;
          result := true;
          exit;
        end;
        Node := Node.getNextSibling;
      end;
    end;
    Node := Node.getNextSibling;
  end;
end;

procedure Register;
begin
  RegisterComponents('FM Controls', [TAddinMeasuresTree]);
end;

end.
