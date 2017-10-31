{
 Объект TXMLCatalog содержит метаданные базы, возвращаемые провайдером
  через метод GetMetaData.
 Остальные модули должны использоваться этим классом для доступа к
 метаданным базы.
}

unit uXMLCatalog;

interface

uses
  SysUtils, Classes, MSXML2_TLB, uXMLUtils, PlaningProvider_TLB,
  uFMAddinGeneralUtils, uGlobalPlaningConst;

type

  {абстрактный класс - элемент метаданных базы}
  TBaseElement = class
  private
    FName: string;
    FProviderId: string;
  public
    property Name: string read FName;
    procedure ReadFromXML(Root: IXMLDOMNode); virtual; abstract;
  end;

  {абстрактный класс - коллекция элементов метаданных базы}
  TBaseCollection = class(TList)
  protected
    function GetItem(Index: integer): TBaseElement;
    procedure SetItem(Index: integer; Value: TBaseElement);
    function GetNamesToString: string;
  public
    destructor Destroy; override;
    procedure Clear; override;
    procedure Delete(Index: integer); virtual;
    function IndexOf(Name: string): integer;
    function Find(Name: string): TBaseElement;
    procedure ReadFromXML(Root: IXMLDOMNode); virtual; abstract;
    procedure SetProviderId(ProvId: string); virtual;

    property Items[Index: integer]: TBaseElement read GetItem write SetItem; default;
    property NamesToString: string read GetNamesToString;
  end;

  TDimElement = class(TBaseElement)
  private
    FCardinality: integer;
  public
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property Cardinality: integer read FCardinality;
  end;

  {уровень измерения}
  TLevel = class(TDimElement)
  private
    FUniqueName: string;
    FDepth: integer;
  protected
  public
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property UniqueName: string read FUniqueName;
    property Depth: integer read FDepth write FDepth;
  end;

  {коллекция уровней}
  TLevelCollection = class(TBaseCollection)
  private
  protected
    function GetItem(Index: integer): TLevel;
    procedure SetItem(Index: integer; Value: TLevel);
    function GetToString: string;
  public
    function Add: TLevel;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property Items[Index: integer]: TLevel read GetItem write SetItem; default;
    property ToString: string read GetToString;
  end;

  TMemberProperty = class(TBaseElement)
  private
    FMask: string;
    {переводит формат инфрагистиков в экселевский}
    function ConvertMask(OldMask: string): string;
  public
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property Mask: string read FMask;
  end;

  TMemberPropertyCollection = class(TBaseCollection)
  protected
    function GetItem(Index: integer): TMemberProperty;
    procedure SetItem(Index: integer; Value: TMemberProperty);
  public
    function Add: TMemberProperty;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    function GetCommaList: string;
    property Items[Index: integer]: TMemberProperty read GetItem write SetItem; default;
  end;

  {иерархия измерения}
  THierarchy = class(TDimElement)
  private
    FUniqueName: string;
    FLevels: TLevelCollection;
    FFullName: string;
    FMemberProperties: TMemberPropertyCollection;
    FAllMember: string;
    FIsShared: boolean;
    FCodeToShow: string;
    FHierarchyId: string;
  protected
  public
    destructor Destroy; override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property UniqueName: string read FUniqueName;
    property Levels: TLevelCollection read FLevels;
    property FullName: string read FFullName write FFullName;
    property MemberProperties: TMemberPropertyCollection read FMemberProperties;
    property AllMember: string read FAllMember write FAllMember;
    property IsShared: boolean read FIsShared;
    property CodeToShow: string read FCodeToShow write FCodeToShow;
    property ProviderId: string read FProviderId;
    property HierarchyId: string read FHierarchyId write FHierarchyId;
  end;

  {коллекция иерархий}
  THierarchyCollection = class(TBaseCollection)
  private
  protected
    function GetItem(Index: integer): THierarchy;
    procedure SetItem(Index: integer; Value: THierarchy);
  public
    function Add: THierarchy;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property Items[Index: integer]: THierarchy
       read GetItem write SetItem; default;
  end;

  TMeasureFormat = (fmtStandard, fmtCurrency, fmtPercent, fmtText, fmtNumber, fmtBoolean);
  {режим подсчета итогов}
  TMeasureCountMode = (mcmSum, mcmMin, mcmMax, mcmAvg, mcmNone, mcmTypeFormula);

const
  MeasureFormatStr: array[TMeasureFormat] of string =
    ('Standard', 'Currency', 'Percent', 'Text', 'Number', 'No such in Excel');

  MeasureCountModeStr: array[TMeasureCountMode] of string=
    ('сумма', 'минимум', 'максимум', 'среднее', 'не вычислять', 'типовая формула');

type
  {мера}
  TMeasure = class(TBaseElement)
  private
    FUniqueName: string;
    FMeasureType: string;
    FFormat: TMeasureFormat;
    FCountMode: TMeasureCountMode;
  protected
    function GetIsCalculated: boolean;
  public
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    property UniqueName: string read FUniqueName;
    property IsCalculated: boolean read GetIsCalculated;
    property Format: TMeasureFormat read FFormat;
    property CountMode: TMeasureCountMode read FCountMode;
    property ProviderId: string read FProviderId;
  end;

  {коллекция мер}
  TMeasureCollection = class(TBaseCollection)
  private
  protected
    function GetItem(Index: integer): TMeasure;
    procedure SetItem(Index: integer; Value: TMeasure);
  public
    function Add: TMeasure;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;

    property Items[Index: integer]: TMeasure
      read GetItem write SetItem; default;
  end;

  {измерение}
  TDimension = class(TDimElement)
  private
    FUniqueName: string;
    FHierarchies: THierarchyCollection;
  protected
  public
    destructor Destroy; override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    function GetHierarchy(Name: string): THierarchy;
    property UniqueName: string read FUniqueName;
    property Hierarchies: THierarchyCollection read FHierarchies;
    property ProviderId: string read FProviderId;
  end;

  {коллекция измерений}
  TDimensionCollection = class(TBaseCollection)
  private
  protected
    function GetItem(Index: integer): TDimension;
    procedure SetItem(Index: integer; Value: TDimension);
  public
    function Add: TDimension;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    function Find(AName: string; AProviderId: string): TDimension;
    procedure SetProviderId(ProvId: string); override;

    property Items[Index: integer]: TDimension
      read GetItem write SetItem; default;
  end;

  {куб}
  TCube = class(TBaseElement)
  private
    FMeasures: TMeasureCollection;
    FDimensions: TDimensionCollection;
    FFullName: string;
    // свойство SubClass куба
    FSubClass: string;
  protected
    function HasStoredMeasuresFunc: boolean;
    function GetWritebackPossible: boolean;
  public
    destructor Destroy; override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    //произвольное вхождение измерения в куб
    function DimInCube(DimName: string): boolean;
    //вхождение конкретной иерархии измерения в куб
    function DimAndHierInCube(DimName, HierName: string): boolean;

    property Measures: TMeasureCollection read FMeasures;
    property Dimensions: TDimensionCollection read FDimensions;
    property FullName: string read FFullName write FFullName;
    property SubClass: string read FSubClass write FSubClass;
    //определяет наличие хранимых мер в кубе
    property HasStoredMeasures: boolean read HasStoredMeasuresFunc;
    property WritebackPossible: boolean read GetWritebackPossible;
    property ProviderId: string read FProviderId;
  end;

  {коллекция кубов}
  TCubeCollection = class(TBaseCollection)
  private
  protected
    function GetItem(Index: integer): TCube;
    procedure SetItem(Index: integer; Value: TCube);
  public
    function Add: TCube;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    function Find(AName: string; AProviderId: string): TCube;

    property Items[Index: integer]: TCube read GetItem write SetItem; default;
  end;

  TProviderInfo = class(TBaseElement)
  private
    FCatalog: string;
    FDatasource: string;
  public
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    procedure WriteToXml(Root: IXMLDOMNode);

    property Datasource: string read FDatasource;
    property Catalog: string read FCatalog;
  end;

  TProvidersCollection = class(TBaseCollection)
  protected
    function GetItem(Index: integer): TProviderInfo;
    procedure SetItem(Index: integer; Value: TProviderInfo);
  public
    function Add: TProviderInfo;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Root: IXMLDOMNode); override;
    procedure WriteToXml(Root: IXMLDOMNode);

    property Items[Index: integer]: TProviderInfo read GetItem write SetItem; default;
  end;

  TXMLCatalog = class
  private
    FCubes: TCubeCollection;
    FDimensions: TDimensionCollection;
    FLoaded: boolean;
    //FPrimaryProvider: string;
    //FInMultibaseMode: boolean;
    FProviders: TProvidersCollection;
    function GetInMultibaseMode: boolean;
    function GetPrimaryProvider: string;
  protected
    function GetLoaded: boolean;
  public
    constructor Create;
    destructor Destroy; override;
    {Загружает или обновляет структуру. Результат - было ли реальное обновление}
    procedure SetUp(Provider: IPlaningProvider);
    procedure Clear;
    property Cubes: TCubeCollection read FCubes;
    property Dimensions: TDimensionCollection read FDimensions;
    property Loaded: boolean read GetLoaded;
    property PrimaryProvider: string read GetPrimaryProvider;
    property InMultibaseMode: boolean read GetInMultibaseMode;
    property Providers: TProvidersCollection read FProviders;
  end;


implementation

const

  {константы для именования атрибутов XML}

  attrName = 'name';
  attrLevel = 'Level';
  attrLevels = 'Levels';
  attrFullName = 'full_name';
  attrHierarchyId = 'hierarchyId';
  attrSubClass = 'subClass';
  attrAllMember = 'all_member';
  attrCube = 'Cube';
  attrCubes = 'Cubes';
  attrDimension = 'Dimension';
  attrDimensions = 'Dimensions';
  attrHierarchy = 'Hierarchy';
  attrHierarchies = 'Hierarchies';
  attrProperty = 'Property';
  attrProperties = 'Properties';
  attrMeasure = 'Measure';
  attrMeasures = 'Measures';
  attrType = 'type';
  attrIsShared = 'IsShared';
  attrFormat = 'format';
  attrMask = 'mask';
  attrCountMode = 'countmode';
  attrCardinality = 'cardinality';
  attrProviderId = 'providerId';


{************* TBaseCollection ****************}

destructor TBaseCollection.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TBaseCollection.GetItem(Index: integer): TBaseElement;
begin
  result := Get(Index);
end;

procedure TBaseCollection.SetItem(Index: integer; Value: TBaseElement);
begin
  Put(Index, Value);
end;

procedure TBaseCollection.Clear;
begin
  while Count > 0 do
    Delete(0);
end;

procedure TBaseCollection.Delete(Index: integer);
begin
  inherited Delete(Index);
end;

function TBaseCollection.IndexOf(Name: string): integer;
begin
  result := 0;
  while (result < Count) and (Items[Result].Name <> Name) do
    Inc(result);
  if result = Count then
    result := -1;
end;

function TBaseCollection.Find(Name: string): TBaseElement;
var
  Index: integer;
begin
  Index := IndexOf(Name);
  if Index >= 0 then
    result := Items[Index]
  else
    result := nil;
end;

{*********** TLevel **************}

procedure TLevel.ReadFromXML(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  inherited ReadFromXml(Root);
end;

{*********** TLevelCollection **************}

function TLevelCollection.GetItem(Index: integer): TLevel;
begin
  result := Get(Index);
end;

procedure TLevelCollection.SetItem(Index: integer; Value: TLevel);
begin
  Put(Index, Value);
end;

function TLevelCollection.Add: TLevel;
begin
  result := TLevel.Create;
  inherited Add(result);
end;

procedure TLevelCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TLevelCollection.ReadFromXML(Root: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Level: TLevel;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrLevel);
  for i := 0 to NL.length - 1 do
  begin
    Level := Add;
    Level.ReadFromXML(NL[i]);
    Level.Depth := i;
  end;
end;

{************** TMemberProperty ************}

function TMemberProperty.ConvertMask(OldMask: string): string;
var
  i: integer;
begin
  result := '';
  if (OldMask = '') then
    exit;
  if (OldMask = 'null') then
  begin
    result := '';
    exit;
  end;
  i := 1;
  repeat
    case OldMask[i] of
      '#': result := result + '0';
      '9': result := result + '#';
      ' ': result := result + '_$';
      '\':
        begin
          result := result + '"' + OldMask[i + 1] + '"';
          inc(i);
        end;
    end;
    inc(i);
  until i > length(OldMask);
end;

procedure TMemberProperty.ReadFromXML(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  FName := GetStrAttr(Root, attrName, '');
  FMask := GetStrAttr(Root, attrMask, '');
  FMask := ConvertMask(FMask);
end;

{************** TMemberPropertyCollection ************}

function TMemberPropertyCollection.GetItem(Index: integer): TMemberProperty;
begin
  result := Get(Index);
end;

procedure TMemberPropertyCollection.SetItem(Index: integer; Value: TMemberProperty);
begin
  Put(Index, Value);
end;

function TMemberPropertyCollection.Add: TMemberProperty;
begin
  result := TMemberProperty.Create;
  inherited Add(result);
end;

procedure TMemberPropertyCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TMemberPropertyCollection.ReadFromXML(Root: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Prop: TMemberProperty;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrProperty);
  for i := 0 to NL.length - 1 do
  begin
    Prop := Add;
    Prop.ReadFromXML(NL[i]);
  end;
end;

function TMemberPropertyCollection.GetCommaList: string;
var i: integer;
begin
  result := '';
  if Count > 0 then
  begin
    result := Items[0].Name;
    for i := 1 to Count - 1 do
    begin
      result := result + ',';
      result := result + Items[i].Name;
    end;
  end;
end;

{************** THierarchy ***************}

destructor THierarchy.Destroy;
begin
  if Assigned(FLevels) then
    FreeAndNil(FLevels);
  if Assigned(FMemberProperties) then
    FreeAndNil(FMemberProperties);
  inherited Destroy;
end;

procedure THierarchy.ReadFromXML(Root: IXMLDOMNode);
var
  DimName, XPath: string;
  LoadingNode: IXMLDOMNode;
begin
  if not Assigned(Root) then
    exit;
  inherited ReadFromXML(Root);
  FIsShared := GetBoolAttr(Root, attrIsShared, true);

  {если грузимся из секции кубов, то иерархии не имеют коллекции уровней.
  необходимо перенаправить загрузку на секцию Shared Dimensions}
  if Root.selectNodes(attrLevels + '/' + attrLevel).length = 0 then
  begin
    DimName := GetStrAttr(Root.parentNode, attrName, '');
    //подменяем Root для загрузки из секции Shared Dimensions
    XPath := 'function_result/SharedDimensions/' + attrDimension +
      '[@' + attrName + '="' + DimName + '"]/' + attrHierarchy +
      '[@' + attrName + '="' + FName + '"]';
    Root := Root.ownerDocument.selectSingleNode(XPath);
  end;
  FFullName := GetStrAttr(Root, attrFullName, '');
  FHierarchyId := GetStrAttr(Root, attrHierarchyId, '');
  FAllMember := GetStrAttr(Root, attrAllMember, '');
  FProviderId := GetStrAttr(Root, attrProviderId, '0');
  {загрузка уровней}
  LoadingNode := Root.selectSingleNode(attrLevels);
  if not Assigned(FLevels) then
    FLevels := TLevelCollection.Create;
  FLevels.ReadFromXML(LoadingNode);
  {загрузка мембер пропертиз}
  LoadingNode := Root.selectSingleNode(attrProperties);
  if not Assigned(FMemberProperties) then
    FMemberProperties := TMemberPropertyCollection.Create;
  FMemberProperties.ReadFromXML(LoadingNode);
  {Имя МР для вывода в компоненте AddinMembersTree в квадратных скобках
    рядом с именем элемента}
  FCodeToShow := '';
  if Assigned(FMemberProperties.Find('Код')) then
    FCodeToShow := 'Код'
  else
    if Assigned(FMemberProperties.Find('ИНН')) then
      FCodeToShow := 'ИНН';
end;

{************** THierarchyCollection ***************}

function THierarchyCollection.GetItem(Index: integer): THierarchy;
begin
  result := Get(Index);
end;

procedure THierarchyCollection.SetItem(Index: integer; Value: THierarchy);
begin
  Put(Index, Value);
end;

function THierarchyCollection.Add: THierarchy;
begin
  result := THierarchy.Create;
  inherited Add(result);
end;

procedure THierarchyCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure THierarchyCollection.ReadFromXML(Root: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Hierarchy: THierarchy;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrHierarchy);
  for i := 0 to NL.length - 1 do
  begin
    Hierarchy := Add;
    Hierarchy.ReadFromXML(NL[i]);
  end;
end;

{*************** TMeasure *******************}

function TMeasure.GetIsCalculated: boolean;
begin
  result := FMeasureType = '127';
end;

procedure TMeasure.ReadFromXML(Root: IXMLDOMNode);
var
  i: TMeasureFormat;
  s: string;
begin
  if not Assigned(Root) then
    exit;
  FName := GetStrAttr(Root, attrName, '');
  FMeasureType := GetStrAttr(Root, attrType, '');
  s := GetStrAttr(Root, attrFormat, '');
  FCountMode := TMeasureCountMode(GetIntAttr(Root, attrCountMode, 0));
  FFormat := fmtStandard;
  for i := Low(TMeasureFormat) to High(TMeasureFormat) do
    if MeasureFormatStr[i] = s then
    begin
      FFormat := i;
      Break;
    end;
end;

{************ TMeasureCollection ****************}

function TMeasureCollection.GetItem(Index: integer): TMeasure;
begin
  result := Get(Index);
end;

procedure TMeasureCollection.SetItem(Index: integer; Value: TMeasure);
begin
  Put(Index, Value);
end;

function TMeasureCollection.Add: TMeasure;
begin
  result := TMeasure.Create;
  inherited Add(result);
end;

procedure TMeasureCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TMeasureCollection.ReadFromXML(Root: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i: integer;
  Measure: TMeasure;
begin
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrMeasure);
  for i := 0 to NL.length - 1 do
  begin
    Measure := Add;
    Measure.ReadFromXML(NL[i]);
  end;
end;

{************** TDimension *****************}

destructor TDimension.Destroy;
begin
  if Assigned(FHierarchies) then
    FreeAndNil(FHierarchies);
  inherited Destroy;
end;

function TDimension.GetHierarchy(Name: string): THierarchy;
begin
  if Name = '' then
    result := Hierarchies[0]
  else
    result := Hierarchies.Find(Name) as THierarchy;
end;

procedure TDimension.ReadFromXML(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  inherited ReadFromXML(Root);
  FProviderId := GetStrAttr(Root, attrProviderId, '0');
  if not Assigned(FHierarchies) then
    FHierarchies := THierarchyCollection.Create;
  FHierarchies.ReadFromXML(Root);
end;

{************ TDimensionCollection **************}

function TDimensionCollection.GetItem(Index: integer): TDimension;
begin
  result := Get(Index);
end;

procedure TDimensionCollection.SetItem(Index: integer; Value: TDimension);
begin
  Put(Index, Value);
end;

function TDimensionCollection.Add: TDimension;
begin
  result := TDimension.Create;
  inherited Add(result);
end;

procedure TDimensionCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TDimensionCollection.ReadFromXML(Root: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i: integer;
  Dimension: TDimension;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrDimension);
  for i := 0 to NL.length - 1 do
  begin
    Dimension := Add;
    Dimension.ReadFromXML(NL[i]);
  end;
end;

{*************** TCube ********************}

destructor TCube.Destroy;
begin
  if Assigned(FMeasures) then
    FreeAndNil(FMeasures);
  if Assigned(FDimensions) then
    FreeAndNil(FDimensions);
  inherited Destroy;
end;

function TCube.HasStoredMeasuresFunc: boolean;
var
  i: integer;
begin
  result := false;
  for i := 0 to FMeasures.Count - 1 do
    if not FMeasures[i].IsCalculated then
    begin
      result := true;
      break;
    end;
end;

procedure TCube.ReadFromXML(Root: IXMLDOMNode);
var
  LoadingNode: IXMLDOMNode;
begin
  if not Assigned(Root) then
    exit;
  FName := GetStrAttr(Root, attrName, '');
  FFullName := GetStrAttr(Root, attrFullName, '');
  FSubClass := GetStrAttr(Root, attrSubClass, '');
  FProviderId := GetStrAttr(Root, attrProviderId, '0');

  {загрузка мер}
  LoadingNode := Root.selectSingleNode(attrMeasures);
  if not Assigned(FMeasures) then
    FMeasures := TMeasureCollection.Create;
  FMeasures.Clear;
  FMeasures.ReadFromXML(LoadingNode);
  Measures.SetProviderId(ProviderId);

  {загрузка измерений}
  LoadingNode := Root.selectSingleNode(attrDimensions);
  if not Assigned(FDimensions) then
    FDimensions := TDimensionCollection.Create;
  FDimensions.Clear;
  FDimensions.ReadFromXML(LoadingNode);
  Dimensions.SetProviderId(ProviderId);
end;

function TCube.DimInCube(DimName: string): boolean;
begin
  result := FDimensions.IndexOf(DimName) >= 0;
end;

function TCube.DimAndHierInCube(DimName, HierName: string): boolean;
var
  DimInd: Integer;
begin
  result := false;
  try
    DimInd := FDimensions.IndexOf(DimName);
    if DimInd >= 0 then
      result := Assigned(FDimensions[DimInd].Hierarchies.Find(HierName));
  except
    result := false;
  end;
end;

{**************** TCubeCollection *****************}

function TCubeCollection.GetItem(Index: integer): TCube;
begin
  result := Get(Index);
end;

procedure TCubeCollection.SetItem(Index: integer; Value: TCube);
begin
  Put(Index, Value);
end;

function TCubeCollection.Add: TCube;
begin
  result := TCube.Create;
  inherited Add(result);
end;

procedure TCubeCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TCubeCollection.ReadFromXML(Root: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i: integer;
  Cube: TCube;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes(attrCube);
  for i := 0 to NL.length - 1 do
  begin
    Cube := Add;
    Cube.ReadFromXML(NL[i]);
  end;
end;

{*********** TXMLCatalog ***************}

constructor TXMLCatalog.Create;
begin
  FDimensions := TDimensionCollection.Create;
  FCubes := TCubeCollection.Create;
  FProviders := TProvidersCollection.Create;
  FLoaded := false;
end;

destructor TXMLCatalog.Destroy;
begin
  FreeAndNil(FCubes);
  FreeAndNil(FDimensions);
  FreeAndNil(FProviders);
  inherited Destroy;
end;

function TXMLCatalog.GetLoaded: boolean;
begin
  result := FLoaded;
end;

procedure TXMLCatalog.SetUp(Provider: IPlaningProvider);
var
  SourceDOM: IXMLDOMDocument2;
  LoadingNode: IXMLDOMNode;
begin
  {полученение метаданных базы}
  if not Assigned(Provider) then
    exit; // нет провайдера - аварийный выход
  if not Provider.Connected then
    exit; // провайдер не подключен - аварийный выход

  try
    {Если загруженные ранее метаданные устарели,
     или мы не были загружены вовсе, тогда произведем очистку и будем загружаться.
     В противном случае, заниливаем SourceDOM, что означает выход из процедуры}
    if Provider.GetMetaData(SourceDOM) or (not FLoaded) then
      Clear
    else
      KillDOMDocument(SourceDOM);
  except
    KillDOMDocument(SourceDOM);
    FLoaded := false;
  end;

  if not Assigned(SourceDOM) then
    exit;

  LoadingNode := SourceDOM.selectSingleNode('function_result/Providers');
  Providers.ReadFromXML(LoadingNode);

  {загрузка shared измерений}
  LoadingNode := SourceDOM.selectSingleNode('function_result/SharedDimensions');
  if not Assigned(LoadingNode) then
    exit; //нет измерений
  FDimensions.ReadFromXML(LoadingNode);

  {загрузка кубов}
  LoadingNode := SourceDOM.selectSingleNode('function_result/Cubes');
  if not Assigned(LoadingNode) then
    exit; // нет кубов
  FCubes.ReadFromXML(LoadingNode);

  FLoaded := true;
  KillDOMDocument(SourceDOM);
end;

procedure TXMLCatalog.Clear;
begin
  FDimensions.Clear;
  FCubes.Clear;
  FLoaded := false;
end;

function TCube.GetWritebackPossible: boolean;
begin
  result := false;
  if ((FullName = 'null') or ((SubClass <> '') and not
    ((SubClass = 'ВВОД') or (SubClass = 'СБОР')))) then
    exit;
  result := HasStoredMeasures;
end;

{ TDimElement }

procedure TDimElement.ReadFromXML(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  FName := GetStrAttr(Root, attrName, '');
  FCardinality := GetIntAttr(Root, attrCardinality, 0);
end;

function TDimensionCollection.Find(AName, AProviderId: string): TDimension;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].Name = AName) and (Items[i].ProviderId = AProviderId) then
    begin
      result := Items[i];
      exit;
    end;
end;

function TCubeCollection.Find(AName, AProviderId: string): TCube;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].Name = AName) and (Items[i].ProviderId = AProviderId) then
    begin
      result := Items[i];
      exit;
    end;
end;

procedure TBaseCollection.SetProviderId(ProvId: string);
var
  i: integer;
begin
  for i := 0 to Count - 1 do
    Items[i].FProviderId := ProvId;
end;

procedure TDimensionCollection.SetProviderId(ProvId: string);
var
  i: integer;
begin
  for i := 0 to Count - 1 do
  begin
    Items[i].FProviderId := ProvId;
    Items[i].Hierarchies.SetProviderId(ProvId);
  end;
end;

{ TProviderInfo }

procedure TProviderInfo.ReadFromXML(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  FProviderId := GetStrAttr(Root, attrProviderId, '0');
  FDatasource := GetStrAttr(Root, 'DataSource', '');
  FCatalog := GetStrAttr(Root, 'Catalog', '');
end;

procedure TProviderInfo.WriteToXml(Root: IXMLDOMNode);
begin
  if not Assigned(Root) then
    exit;
  SetAttr(Root, attrProviderId, FProviderId);
  SetAttr(Root, 'DataSource', FDatasource);
  SetAttr(Root, 'Catalog', FCatalog);
end;

{ TProvidersCollection }

function TProvidersCollection.Add: TProviderInfo;
begin
  result := TProviderInfo.Create;
  inherited Add(result);
end;

procedure TProvidersCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TProvidersCollection.GetItem(Index: integer): TProviderInfo;
begin
  result := Get(Index);
end;

procedure TProvidersCollection.ReadFromXML(Root: IXMLDOMNode);
var
  i: integer;
  ProviderInfo: TProviderInfo;
  NL: IXMLDOMNodeList;
begin
  Clear;
  if not Assigned(Root) then
    exit;
  NL := Root.selectNodes('Provider');
  for i := 0 to NL.length - 1 do
  begin
    ProviderInfo := Add;
    ProviderInfo.ReadFromXML(NL[i]);
  end;
end;

procedure TProvidersCollection.WriteToXml(Root: IXMLDOMNode);
var
  i: integer;
  Node: IXMLDOMNode;
begin
  if not Assigned(Root) then
    exit;
  for i := 0 to Count - 1 do
  begin
    Node := Root.ownerDocument.createNode(1, 'Provider', '');
    Items[i].WriteToXml(Node);
    Root.appendChild(Node);
  end;
end;

function TXMLCatalog.GetInMultibaseMode: boolean;
begin
  result := Providers.Count > 1;
end;

function TXMLCatalog.GetPrimaryProvider: string;
begin
  result := '0';
  try
    result := Providers[0].FProviderId;
  except
  end;
end;

procedure TProvidersCollection.SetItem(Index: integer;
  Value: TProviderInfo);
begin
  Put(Index, Value);
end;

function TLevelCollection.GetToString: string;
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

function TBaseCollection.GetNamesToString: string;
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

