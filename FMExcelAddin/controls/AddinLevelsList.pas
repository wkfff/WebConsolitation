unit AddinLevelsList;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, uXMLCatalog, uFMAddinGeneralUtils, uSheetObjectModel;

type
  TAddinCheckList = class(TListView)
  private
    { Private declarations }
    FCatalog: TXMLCatalog;
    FDimension: TDimension;
    FHierarchy: THierarchy;
  protected
    { Protected declarations }
    procedure OnMouseUpChangeCheckState(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer); virtual;
    function GetItemsAsStrings(CheckedOnly: boolean): TStringList;
   public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    property Catalog: TXMLCatalog read FCatalog write FCatalog;
    property Dimension: TDimension read FDimension;
    property Hierarchy: THierarchy read FHierarchy;
  published
    { Published declarations }
  end;


  (*TAddinLevelsList = class(TAddinCheckList)
  public
    { Public declarations }
    //загрузка уровней для вновь создаваемого элемента
    function Load(Dimension: TDimension;
      Hierarchy: THierarchy): boolean; overload;
    //загрузка уровней для редактируемого элемента
    function Load(Dimension: TDimension; Hierarchy: THierarchy;
      AxisElement: TSheetAxisElementInterface): boolean; overload;
    property LevelsAsStringList[CheckedOnly: boolean]: TStringList read GetItemsAsStrings;
  published
    { Published declarations }
  end; *)


  TAddinMemberPropertiesList = class(TAddinCheckList)
  public
    { Public declarations }
    function Load(Dimension: TDimension; Hierarchy: THierarchy): boolean;
    property PropsAsStringList[CheckedOnly: boolean]: TStringList read GetItemsAsStrings;
  published
    { Published declarations }
  end;

procedure Register;

implementation

{ методы класса TAddinCheckList }
constructor TAddinCheckList.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  CheckBoxes := true;
  HideSelection := false;
  ViewStyle := vsReport;
  Width := 445;
  Height := 200;
  ReadOnly := true;
  ShowColumnHeaders := false;
  ShowWorkAreas := false;
  with Columns.Add do
  begin
    AutoSize := true;
    Caption :='';
  end;
  FCatalog := nil;
  OnMouseUp := OnMouseUpChangeCheckState;
end;

destructor TAddinCheckList.Destroy;
begin
  FCatalog := nil;
  inherited Destroy;
end;

procedure TAddinCheckList.OnMouseUpChangeCheckState(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
end;

function TAddinCheckList.GetItemsAsStrings(CheckedOnly: boolean): TStringList;
var
  i: integer;
begin
  result := TStringList.Create;
  for i := 0 to Items.Count - 1 do
    if (Items[i].Checked or not CheckedOnly) then
      result.Add(Items[i].Caption);
end;


{ методы класса TAddinLevelsList}
(*function TAddinLevelsList.Load(Dimension: TDimension;
  Hierarchy: THierarchy): boolean;
var
  Levels: TLevelCollection;
  i: integer;
begin
  result := false;
  Items.Clear;
  Items.BeginUpdate;
  if not Assigned(Dimension) or not Assigned(Catalog)
    or not Assigned(Hierarchy) then
    exit;
  FDimension := Dimension;
  FHierarchy := Hierarchy;
  Levels := Hierarchy.Levels;
  for i := 0 to Levels.Count - 1 do
    with Items.Add do
    begin
      Caption := Levels[i].Name;
      ImageIndex := 4 + i;
      Checked := true;
      Data := Pointer(Levels[i]);
    end;
  Items.EndUpdate;
  Invalidate;
  result := true;
end;

function TAddinLevelsList.Load(Dimension: TDimension; Hierarchy: THierarchy;
  AxisElement: TSheetAxisElementInterface): boolean;
var
  i, j: integer;
  Levels: TLevelCollection;
begin
  result := false;
  Items.BeginUpdate;
  Items.Clear;
  if not Assigned(Catalog) or not Assigned(Dimension) or
    not Assigned(Hierarchy) or not Assigned(AxisElement) then
    exit;
  FDimension := Dimension;
  FHierarchy := Hierarchy;
  Levels := Hierarchy.Levels;
  for i := 0 to Levels.Count - 1 do
    with Items.Add do
    begin
      Caption := Levels[i].Name;
      ImageIndex := 4 + i;
      Checked := false;
      Data := Pointer(Levels[i]);
      for j := 0 to AxisElement.Levels.Count - 1 do
        if AxisElement.Levels.IndexOf(Levels[i].Name) >= 0 then
        begin
          Checked := true;
          Break;
        end;
    end;
  Items.EndUpdate;
  result := true;
end;
 *)

{ методы класса TAddinMemberPropertiesList }
function TAddinMemberPropertiesList.Load(Dimension: TDimension;
  Hierarchy: THierarchy): boolean;
var
  MProps: TMemberPropertyCollection;
  i: integer;
begin
  result := false;
  Items.Clear;
  Items.BeginUpdate;
  if not Assigned(Dimension) or not Assigned(Catalog)
    or not Assigned(Hierarchy) then
    exit;
  FDimension := Dimension;
  FHierarchy := Hierarchy;
  MProps := Hierarchy.MemberProperties;
  for i := 0 to MProps.Count - 1 do
    with Items.Add do
    begin
      Caption := MProps[i].Name;
//      ImageIndex := 4 + i;
      Checked := false;
//      Data := Pointer(Levels[i]);
    end;
  Items.EndUpdate;
  Invalidate;
  result := true;
end;


procedure Register;
begin
//  RegisterComponents('FM Controls', [TAddinLevelsList]);
  RegisterComponents('FM Controls', [TAddinMemberPropertiesList]);
end;

end.
