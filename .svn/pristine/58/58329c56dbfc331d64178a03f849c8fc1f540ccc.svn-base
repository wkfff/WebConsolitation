{
  Уровни измерений в объектной модели листа (объект-уровень + коллекция).
}

unit uSheetLevels;

interface
uses
  Classes, SysUtils, MSXML2_TLB, uXMLUtils, uFMExcelAddinConst,
  uFMAddinGeneralUtils, uFMAddinExcelUtils, uSheetObjectModel,
  uGlobalPlaningConst, uXMLCatalog, uFMAddinXMLUtils;

type
  TSheetLevelCollection = class;

  TSheetLevel = class(TSheetLevelInterface)
  private
  protected
    function GetExcelName: string; override;
    function GetOrientation: TAxisType; override;
    function GetTitleExcelName: string; override;
    function GetSheetInterface: TSheetInterface; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    function GetAlias: string; override;
  public
    constructor Create(AOwner: TSheetLevelCollectionInterface);
    destructor Destroy; override;

    //считывает атрибуты элемента из узла
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает атрибуты элемента в узел
    procedure WriteToXML(Node: IXMLDOMNode); override;

    {!!!Заглушка, что бы прикрыть абстрактный метод}
    function Validate(out MsgText: string; out ErrorCode: integer): boolean; override;
    {!!!Заглушка, что бы прикрыть абстрактный метод}
    function Refresh(Force: boolean): boolean; override;
    // получить тип элемента листа
    function GetObjectType: TSheetObjectType; override;
    // получить строковое описание типа элемента листа
    function GetObjectTypeStr: string; override;
    function GetElementCaption: string; override;
    {!!!Заглушка, что бы прикрыть абстрактный метод}
    procedure ApplyStyles; override;
    function GetDepth: integer; override;
    function GetOnDeleteWarning: string; override;
  end;


  TSheetLevelCollection = class(TSheetLevelCollectionInterface)
  private
  protected
    function GetItem(Index: integer): TSheetLevelInterface; override;
    procedure SetItem(Index: integer; Value: TSheetLevelInterface); override;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    destructor Destroy; override;
    function Append: TSheetLevelInterface; override;
    procedure Delete(Index: integer); override;
    //загружает коллекцию из DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает коллекцию в XML
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //Экспортируется в список имен уровней в виде TStringList
    function GetToString: string; override;
    function  GetNamesToString: string; override;
    //возвращает индекс элемента в коллекции
    function FindByID(ID: string): integer; override;
    //возвращает имя коллекции в соответствии с типом ее элементов
    function GetCollectionName: string; override;
    {!!!Заглушка, что бы прикрыть абстрактный метод}
    function Validate: boolean; override;
    {!!!Заглушка, что бы прикрыть абстрактный метод}
    function Refresh(Force: boolean): boolean; override;
    function FindByInitialIndex(Index: integer): TSheetLevelInterface; override;

    property Items[Index: integer]: TSheetLevelInterface
      read GetItem write SetItem; default;
  end;


implementation

{************ TSheetLevel ***************}

constructor TSheetLevel.Create(AOwner: TSheetLevelCollectionInterface);
begin
  FOwner := AOwner;
  inherited Create(TSheetCollection(AOwner));
  ColumnWidth := 0; //по умолчанию. означает - на усмотрение Excel
  SummaryOptions := TSummaryOptions.Create;
  FontOptions := TFontOptions.Create;
  InitialIndex := -1;
  DMDeployment := idTop;
end;

destructor TSheetLevel.Destroy;
begin
  Owner := nil;
  SummaryOptions.Free;
  FontOptions.Free;
  inherited Destroy;
end;

function TSheetLevel.GetExcelName: string;
begin
  result := snSeparator + UniqueID;
  if Orientation = axRow then
    result := sntRowLevel + result
  else
    result := sntColumnLevel + result;
  result := BuildExcelName(result);
end;

function TSheetLevel.GetTitleExcelName: string;
begin
  result := ParentCollection.Owner.ExcelName;
  CutPart(result, snSeparator);
  result := snSeparator + UniqueID + snSeparator + result;
  if Orientation = axRow then
    result := sntRowLevelTitle + result
  else
    result := sntColumnLevelTitle + result;
  result := BuildExcelName(result);
end;

function TSheetLevel.GetOrientation: TAxisType;
begin
  result := ParentCollection.Owner.Orientation;
end;

procedure TSheetLevel.ReadFromXML(Node: IXMLDOMNode);
begin
  inherited ReadFromXML(Node);
  if not Assigned(Node) then
    exit;
  if Assigned(SheetInterface) then
    if not SheetInterface.InCopyMode then
      UniqueID := GetStrAttr(Node, attrID, '');
  Name := GetNodeStrAttr(Node, attrName);
  ColumnWidth := StrToFloat(GetAttr(Node, attrColumnWidth));
  SummaryOptions.ReadFromXML(Node.selectSingleNode(attrSummaryOptions));
  FontOptions.ReadFromXml(Node.selectSingleNode('fontoptions'));
  AllCapitals := GetBoolAttr(Node, attrAllCapitals, false);
  UseFormat := GetBoolAttr(Node, attrUseFormat, false);
  UseCustomDMTitle := GetBoolAttr(Node, attrUseCustomDMtitle, false);
  CustomDMTitle := GetStrAttr(Node, attrCustomDMtitle, '');
  DMDeployment := TItemDeployment(GetIntAttr(Node, attrDeployment, 0));
end;

procedure TSheetLevel.WriteToXML(Node: IXMLDOMNode);
var
  SummaryNode, FontNode: IXMLDOMNode;
begin
  inherited WriteToXml(Node);
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    SetNodeStrAttr(Node, attrName, Name);
    setAttribute(attrColumnWidth, FloatToStr(ColumnWidth));
    setAttribute(attrAllCapitals, BoolToStr(AllCapitals));
    setAttribute(attrUseFormat, BoolToStr(UseFormat));
    setAttribute(attrUseCustomDMTitle, BoolToStr(UseCustomDMTitle));
    setAttribute(attrCustomDMTitle, CustomDMTitle);
    setAttribute(attrDeployment, Ord(DMDeployment));
  end;
  {настройки итогов}
  SummaryNode := Node.ownerDocument.createNode(1, attrSummaryoptions, '');
  SummaryOptions.WriteToXML(SummaryNode);
  Node.appendChild(SummaryNode);
  {настройки шрифта}
  FontNode := Node.ownerDocument.createNode(1, 'fontoptions', '');
  FontOptions.WriteToXml(FontNode);
  Node.appendChild(FontNode);
end;

{!!!Заглушка, что бы прикрыть абстрактный метод}
function TSheetLevel.Validate(out MsgText: string; out ErrorCode: integer): boolean;
begin
  result := true;
end;

{!!!Заглушка, что бы прикрыть абстрактный метод}
function TSheetLevel.Refresh(Force: boolean): boolean;
begin
  result := true;
end;


{************ TSheetLevelCollection ***************}



destructor TSheetLevelCollection.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TSheetLevelCollection.GetItem(Index: integer): TSheetLevelInterface;
begin
  result := Get(Index);
end;

procedure TSheetLevelCollection.SetItem(Index: integer; Value: TSheetLevelInterface);
begin
  Put(Index, Value);
end;

function TSheetLevelCollection.Append: TSheetLevelInterface;
begin
  result := TSheetLevel.Create(Self);
  inherited Add(result);
  result.SummaryOptions.Copy(Owner.SummaryOptions);
end;

procedure TSheetLevelCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TSheetLevelCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Level: TSheetLevelInterface;
begin
  inherited;
  if not Assigned(Node) then
    exit;
  NL := Node.selectNodes('level');
  for i := 0 to NL.length - 1 do
  begin
    Level := Append;
    Level.ReadFromXML(NL[i]);
  end;
  NL := nil;
end;

procedure TSheetLevelCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  inherited;
  if not Assigned(Node) then
    exit;

  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'level', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;
end;

function TSheetLevelCollection.GetToString: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Count - 1 do
  begin
    AddTail(result, snBucks);
    result := result + Items[i].Name;
  end;
end;

function TSheetLevelCollection.FindByID(ID: string): integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Count - 1 do
    if Items[i].UniqueID = ID then
    begin
      result := i;
      break;
    end;
end;

{возвращает имя коллекции в соответствии с типом ее элементов}
function TSheetLevelCollection.GetCollectionName: string;
begin
  result := 'levels';
end;

{!!!Заглушка, что бы прикрыть абстрактный метод}
function TSheetLevelCollection.Validate: boolean;
begin
  result := true;
end;

{!!!Заглушка, что бы прикрыть абстрактный метод}
function TSheetLevelCollection.Refresh(Force: boolean): boolean;
begin
  result := true;
end;

function TSheetLevel.GetObjectType: TSheetObjectType;
begin
  result := wsoLevel;
end;

function TSheetLevel.GetObjectTypeStr: string;
begin
  result := 'Уровень';
end;

function TSheetLevel.GetSheetInterface: TSheetInterface;
begin
  try
    result := ParentCollection.Owner.SheetInterface;
  except
    result := nil;
  end;
end;

function TSheetLevel.GetElementCaption: string;
begin
  result := Name;
end;

procedure TSheetLevel.ApplyStyles;
begin
  //заглушка
end;

function TSheetLevel.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFieldPosition;
    esValueprint: result := snFieldPositionPrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

function TSheetLevelCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue: result := 'Элементы';
    esValuePrint: result := 'Элементы (печать)';
    esTitle: result := 'Заголовок';
    esTitlePrint: result := 'Заголовок (печать)';
  end;
end;

function TSheetLevelCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFieldPosition;
    esValueprint: result := snFieldPositionPrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

function TSheetLevel.GetDepth: integer;
var
  Dim: TDimension;
  Hier: THierarchy;
  Level: TLevel;
begin
  result := 0;
  Dim := SheetInterface.XMLCatalog.Dimensions.Find(AxisElement.Dimension, AxisElement.ProviderId);
  if not Assigned(Dim) then
    exit;
  Hier := Dim.GetHierarchy(AxisElement.Hierarchy);
  if not Assigned(Hier) then
    exit;
  Level := Hier.Levels.Find(Name) as TLevel;
  if not Assigned(Level) then
    exit;
  result := Level.Depth;
end;

function TSheetLevel.GetAlias: string;
begin
  result := '';
end;

function TSheetLevelCollection.FindByInitialIndex(
  Index: integer): TSheetLevelInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].InitialIndex = Index then
    begin
      result := Items[i];
      exit;
    end;
end;

function TSheetLevel.GetOnDeleteWarning: string;
begin
  ;
end;

function TSheetLevelCollection.GetNamesToString: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Count - 1 do
  begin
    AddTail(result, ', ');
    result := result + Items[i].Name;
  end;
end;

end.
