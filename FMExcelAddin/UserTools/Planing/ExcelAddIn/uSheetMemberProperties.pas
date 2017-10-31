{
  Свойства измерения (Member Properties)
}

unit uSheetMemberProperties;

interface
uses
  classes, SysUtils,
  uSheetObjectModel, MSXML2_TLB, uXMLUtils, uFMExcelAddinConst, uXMLCatalog,
  uGlobalPlaningConst, uFMAddinGeneralUtils;

type
  TSheetMPCollection = class;

  TSheetMPElement = class(TSheetMPElementInterface)
  private
  protected
    function GetMask: string; override;
    procedure SetMask(Value: string); override;
  public
    procedure ReadFromXML(Node: IXMLDOmNode); override;
    procedure WriteToXML(Node: IXMLDOmNode); override;
  end;

  TSheetMPCollection = class(TSheetMPCollectionInterface)
  private
  protected
    function GetItem(Index: integer): TSheetMPElementInterface; override;
    procedure SetItem(Index: integer; Value: TSheetMPElementInterface); override;
  public
    destructor Destroy; override;
    function Append: TSheetMPElementInterface; override;
    procedure Delete(Index: integer); {override;} //$$$
    procedure Clear; override;
    function GetCheckedCount: integer; override;
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;
    procedure Reload; overload; override;
    function Find(AName: string): TSheetMPElementInterface; override;
    property Items[Index: integer]: TSheetMPElementInterface read GetItem
      write SetItem; default;
  end;

implementation
{ TSheetMPElement }

function TSheetMPElement.GetMask: string;
begin
  result := FMask;
end;

procedure TSheetMPElement.SetMask(Value: string);
begin
  FMask := Value;
end;

procedure TSheetMPElement.ReadFromXML(Node: IXMLDOmNode);
begin
  if not Assigned(Node) then
    exit;
  Name := GetStrAttr(Node, attrName, '');
  Mask := GetStrAttr(Node, attrMask, '');
  Checked := GetIntAttr(Node, attrChecked, 0) <> 0;
end;

procedure TSheetMPElement.WriteToXML(Node: IXMLDOmNode);
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrName, Name);
    setAttribute(attrMask, FMask);
    setAttribute(attrChecked, Checked);
  end;
end;

{ TSheetMPCollection }

function TSheetMPCollection.Append: TSheetMPElementInterface;
begin
  result := TSheetMPElement.Create(Self);
  inherited Add(result);
end;

procedure TSheetMPCollection.Clear;
begin
  while Count > 0 do
    Delete(0);
end;

procedure TSheetMPCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

destructor TSheetMPCollection.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TSheetMPCollection.Find(AName: string): TSheetMPElementInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Name = AName then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetMPCollection.GetCheckedCount: integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to Count - 1 do
    if Items[i].Checked then
      inc(result);
end;

function TSheetMPCollection.GetItem(Index: integer): TSheetMPElementInterface;
begin
  result := Get(Index);
end;

procedure TSheetMPCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  MPElement: TSheetMPElementInterface;
begin
  Clear;
  if not Assigned(Node) then
    exit;
  NL := Node.selectNodes('property');
  for i := 0 to NL.length - 1 do
  begin
    MPElement := Append;
    MPElement.ReadFromXML(NL[i]);
  end;
  NL := nil;
end;

procedure TSheetMPCollection.Reload;
var
  i: integer;
  Hierarchy: THierarchy;
  CatalogMP: TMemberProperty;
  SheetMP: TSheetMPElementInterface;
begin
  {В связи с 18067 - изменяемым порядком вывода свойств - состояние коллекции
    теперь надо сохранять, значит просто очистить и перечитать заново - не выход.
    Сделаем двухпроходную синхронизацию.}
  Hierarchy := Owner.CatalogHierarchy;
  if not Assigned(Hierarchy) then
    exit;

  for i := Count - 1 downto 0 do
  begin
    SheetMP := Items[i];
    CatalogMP := TMemberProperty(Hierarchy.MemberProperties.Find(SheetMP.Name));
    if not Assigned(CatalogMP) then
      Delete(i);
  end;

  for i := 0 to Hierarchy.MemberProperties.Count - 1 do
  begin
    CatalogMP := Hierarchy.MemberProperties[i];
    SheetMP := Find(CatalogMP.Name);
    if Assigned(SheetMP) then
      continue;
    SheetMP := Append;
    SheetMP.Name := CatalogMP.Name;
    SheetMP.Mask := CatalogMP.Mask;
  end;

end;

procedure TSheetMPCollection.SetItem(Index: integer; Value: TSheetMPElementInterface);
begin
  Put(Index, Value);
end;

procedure TSheetMPCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'property', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;
end;

end.
