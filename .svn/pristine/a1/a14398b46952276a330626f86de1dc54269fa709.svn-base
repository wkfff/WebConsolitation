unit uDsoXmlSchema;

interface

uses Classes, ComObj, SysUtils, DSO80_TLB, MSXML2_TLB;

type
  TDSONames = record
    mane: string;
    collection: string;
  end;

const
  ObjectNameByClassType: array[1..35, 0..1] of string = (
    ('Server', ''),
    ('Database', 'MDStores'),
    ('DatabaseRole', 'Roles'),
    ('DatabaseCommand', 'Commands'),
    ('Unknown', ''),
    ('DataSource', 'DataSources'),
    ('DatabaseDimension', 'Dimensions'),
    ('DatabaseLevel', 'Levels'),
    ('Cube', 'MDStores'),
    ('CubeMeasure', 'Measures'),
    ('CubeDimension', 'Dimensions'),
    ('CubeLevel', 'Levels'),
    ('CubeCommand', 'Commands'),
    ('CubeRole', 'Roles'),
    ('Unknown', ''),
    ('Unknown', ''),
    ('Unknown', ''),
    ('Unknown', ''),
    ('Partition', 'MDStores'),
    ('PartitionMeasure', 'Measures'),
    ('PartitionDimension', 'Dimensions'),
    ('PartitionLevel', 'Levels'),
    ('Aggregation', 'MDStores'),
    ('AggregationMeasure', 'Measures'),
    ('AggregationDimension', 'Dimensions'),
    ('AggregationLevel', 'Levels'),
    ('Unknown', ''),
    ('CubeAnalyzer', ''),
    ('PartitionAnalyzer', ''),
    ('Collection', ''),
    ('MemberProperty', 'MemberProperties'),
    ('RoleCommand', 'Commands'),
    ('MiningModel', 'MiningModels'),
    ('Column', 'Columns'),
    ('MiningModelRole', 'Roles')
  );

// Тип доступа
  AccessDeny = 0;
  AccessReadOnly	= 1;
  AccessReadWrite	= 2;
  AccessRW_IF = 3;
  AccessR_IF = 4;
  AccessNA_IF = 5;

// Типы данных
  dtInteger = 0;
  dtString = 1;
  dtBoolean = 2;
  dtDate = 3;
  dtDouble = 4;
  dtObject = 5;
  dtCDATA = 11;

type
  AccessType = (AccessRead, AccessWrite);

type
  TXMLSchema = class;
  TSchemaObject = class;

  TCheck = class
  private
    _propertyName: string;	// Имя свойства значение которого нужно проверить
    _propertyValue: OleVariant;	// Значение свойства. Если свойство равно этому значению,
															//	то возвращаем истину
    _checkKind: integer;	// Вид проверки: 0 - "=", 1 - "<>", 2 - "<" 3 - ">" 4 - "<=" 5 - ">="
  public
    destructor Destroy; override;
  end;

  PEnumItem = ^TEnumItem;
  TEnumItem = record
    id: integer;
    Name: string;
    Caption: string;
  end;

  PDSOEnum = ^TDSOEnum;
  TDSOEnum = class(TList)
  private
    FName: string;
    FMin: Integer;
    FMax: Integer;
  public
    destructor Destroy; override;
    function GetEnumItem(AIndex: Integer): PEnumItem;
    procedure Clear; override;
    procedure Add(EnumItem: PEnumItem);
    procedure DeleteItem(Index: Integer);
    function FindItemName(const ItemID: string): Integer;
    function FindItemCaption(const ItemID: string): Integer;
    function FindItemIDByName(const ItemName: string): Integer;
    function IndexOfItemID(const ItemID: integer;
      StartIndex: Integer): Integer;
    property Name: string read FName;
    property MaxVal: Integer read FMax;
    property MinVal: Integer read FMin;
  end;

  TDSOEnumList = class(TList)
  private
    FOwner: TXMLSchema;
    function GetEnum(AIndex: Integer): TDSOEnum;
  public
    constructor Create(AOwner: TXMLSchema);
    destructor Destroy; override;
    procedure Clear; override;
    procedure Add(DSOEnum: TDSOEnum);
    procedure DeleteEnum(Index: Integer);
    function IndexOfEnumName(const EnumName: string;
      StartIndex: Integer): Integer;
    function FindEnumName(const EnumName: string): Integer;
    property Enums[AIndex: Integer]: TDSOEnum read GetEnum; default;
  end;

  TProperty = class	    // заполняется из тега <property ... />
  private
    _name: WideString;	    // Имя свойства
    _type: integer;	    // Тип данных свойства
    _access: integer;	    // Способ доступа к свойству
    _reduceAccess: integer; // Понижение уровня доступа к свойству
    _checks: TList;	    // Набор проверок, которые необходимо выполнить
    _IsSaved: boolean;      // Должно ли свойство притудительно сохраняться
                            //	для определения доступности свойства
    _caption: WideString;   // Описание свойства
    _IsSecondPass: Boolean; // Значение свойства может быть установленно
                            // только после обработки всех предков
    _IsSilent: Boolean;     // Очередная затычка
    _enum: integer;
    _parent: TXMLSchema;
    _owner: TSchemaObject;
    _object: TSchemaObject;
  public
    Constructor Create;
    destructor Destroy; override;
    function CheckAccess(const conversionObject: IDispatch; const RequestAccess: AccessType): integer;
  published
    property Name: WideString read _name;
    property DataType: Integer read _type;
    property Access: Integer read _access;
    property IsSaved: Boolean read _IsSaved;
    property IsSecondPass: Boolean read _IsSecondPass;
    property IsSilent: Boolean read _IsSilent;
    property Caption: WideString read _caption;
    property Owner: TSchemaObject read _owner write _owner;
    property Enum: Integer read _enum write _enum;
    property Parent: TXMLSchema read _parent write _parent;
    property PropObject: TSchemaObject read _object;
  end;

  TSchemaObject = class       	// заполняется из тега <object ... />
  private
    _name: WideString;         	// Имя объекта (Database, Cube, CubeLevel и т.д.)
    _class: Integer;            // Ксласс объекта
    _interface: WideString;	// Имя интерфейса объекта (MDStore, Dimension, Level и т.д.)
    _parent: TSchemaObject;     // Объект предок
    _access: integer;		// Способ доступа к объекту
    _reduceAccess: integer;	// Понижение уровня доступа к объекту
    _collections: TStringList;	// Набор доступных коллекций объекта
    _methods: TStringList;      // Список доступных методов объекта
    _properties: TStringList;   // Список доступных свойств объекта
    _guid: TGUID;
    _checks: TList;		// Набор проверок, которые необходимо выполнить
                                //  для определения доступности свойства
  public
    constructor Create(pParent: TSchemaObject);
    destructor Destroy; override;
    function CheckAccess(const conversionObject: ICommon; const RequestAccess: AccessType): integer;
  published
    property Name: WideString read _name;
    property ObjectClass: Integer read _class;
    property InterfaceName: WideString read _interface;
    property InterfaceGUID: TGUID read _guid;
    property Parent: TSchemaObject read _parent;
    property Access: integer read _access;
    property Methods: TStringList read _methods;
    property Collections: TStringList read _collections;
    property Properties: TStringList read _properties;
  end;

  TXMLSchemaErrorEvent = procedure (const stringMsg: string) of object;

  TXMLSchema = class
  private
    _root: TSchemaObject;	// Корневой объект каталога, должен быть Database
    _logError: TXMLSchemaErrorEvent;
    FEnums: TDSOEnumList;
    FObjectList: TStringList;
    procedure LoadProperty(const xmlNode: IXMLDOMNode; var Properties: TStringList; const SchemaObject: TSchemaObject);
    procedure LoadCollection(const xmlNode: IXMLDOMNode; MetaObject: TSchemaObject);
    procedure LoadCollection2(const xmlNode: IXMLDOMNode; MetaObject: TSchemaObject);
    procedure LoadObjectProperties(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
    procedure LoadObjectProperties2(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
    procedure LoadObjectServerProperties(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
    procedure LoadObjects(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
    procedure LoadObject2(const xmlNode: IXMLDOMNode);
    procedure LoadObjectsTree(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
    procedure LoadDSOEnums(const xmlNodes: IXMLDOMNodeList);
    procedure LoadServerNode(const xmlNode: IXMLDOMNode; var SchemaObject: TSchemaObject);
  public
    constructor Create;
    destructor Destroy; override;
    function LoadFromXML(const filePathXml: string): boolean;
    function FindMetaObject(ObjectName: string): TSchemaObject;
    function FindMetaObjectByClass(ObjectClass: Integer): TSchemaObject;
  published
    property Root: TSchemaObject read _root;
    property LogError: TXMLSchemaErrorEvent read _logError write _logError;
    property Enums: TDSOEnumList read FEnums write FEnums;
  end;

function GetObjectNameByClassType(clsType: ClassTypes): WideString;
function GetCollectionByClassType(clsType: ClassTypes): WideString;
function GetCollectionByObjectName(const name: string): WideString;

implementation

uses uDispatchInvoke;

function GetObjectNameByClassType(clsType: ClassTypes): WideString;
begin
  Result := ObjectNameByClassType[clsType, 0];
end;

function GetCollectionByClassType(clsType: ClassTypes): WideString;
begin
  Result := ObjectNameByClassType[clsType, 1];
end;

function GetCollectionByObjectName(const name: string): WideString;
var
  i, indx: integer;
begin
  indx := 0;
  for i := 1 to 35 do
    if ObjectNameByClassType[i, 0] = name then begin
      indx := i; break
    end;
  if indx > 0 then
    Result := ObjectNameByClassType[indx, 1]
  else
    Result := ''
end;

{ TCheck }

destructor TCheck.Destroy;
begin
end;

{ TProperty }

constructor TProperty.Create;
begin
  inherited;
  _checks := TList.Create
end;

destructor TProperty.Destroy;
var
  i: Integer;
begin
  for i := 0 to _checks.Count - 1 do
    TCheck(_checks.Items[i]).Destroy;
  _checks.Clear;
  _checks.Destroy;
  inherited
end;

// Функция проверяет можно ли получить доступ к свойству объекта conversionObject
// с запрощенным уровнем доступа RequestAccess. Если доступ возможен, то возвращает True
// Значением уровня доступа может быть AccessRead или AccessWrite
function TProperty.CheckAccess(const conversionObject: IDispatch; const RequestAccess: AccessType): integer;
var
  i: integer;
  check: TCheck;
  Val: Variant;
  ChecksResult: boolean;
//  raccess: integer;
  permitaccess: integer;
  propertyValue: Variant;
  dsoDim: Dimension;
  dsoDBDim: DbDimension;
  dsoObj: ICommon;
  Disp: IDispatch;
begin
  try
    dsoObj := conversionObject as ICommon;
    if dsoObj.ClassType = clsDatabaseDimension then begin
      dsoDim := conversionObject as Dimension;
      dsoDBDim := conversionObject as DbDimension;
    end;
  except
  end;

  if _reduceAccess = 0 then
    permitaccess := _access
  else begin
    ChecksResult := False;
    for i := 0 to _checks.Count - 1 do begin
      check := _checks.Items[i];
      try
      	// Получаем значение свойства
        IUnknown(conversionObject).QueryInterface(_owner.InterfaceGUID, Disp);
        Val := GetDispatchProperty(Disp, check._propertyName);
      except
      	if Assigned(_parent) then
          if Assigned(_parent._logError) then
            _parent._logError('Can''t check property:' + check._propertyName + ' for ' + Name);
            continue;// по каким-то причинам проверку не удалось выполнить
      end;
      // Если условие проверки выполняется, то возвращаем True
      propertyValue := VarAsType(check._propertyValue, TVarData(Val).VType);
      case check._checkKind of
      0:	if Val = propertyValue then begin
      			ChecksResult := True; break
      		end;
      1:	if Val <> propertyValue then begin
      			ChecksResult := True; break
      		end;
      2:	if Val < propertyValue then begin
      			ChecksResult := True; break
      		end;
      3:	if Val > propertyValue then begin
      			ChecksResult := True; break
      		end
      end
		end;

    // Если проверки выполнены успешно, то уровень доступа остаётся прежним
    if ChecksResult then
      permitaccess := _access
    else
      // иначе понижаем уроветь доступа к свойству
      permitaccess := _access - _reduceAccess
  end;

  result := permitaccess
end;

{ TSchemaObject }

constructor TSchemaObject.Create(pParent: TSchemaObject);
begin
  _Parent := pParent;

  _collections := TStringList.Create;
  _methods := TStringList.Create;
  _properties := TStringList.Create;
  _checks := TList.Create;

  _collections.Sorted := False;
  _methods.Sorted := True;
  _properties.Sorted := False;
end;

destructor TSchemaObject.Destroy;
var
  i: integer;
begin
  for i:=0 to _checks.Count - 1 do
    if Assigned(_checks.Items[i]) then
      TCheck(_checks.Items[i]).Destroy;

  for i:=0 to _properties.Count - 1 do
    if Assigned(_properties.Objects[i]) then
      _properties.Objects[i].Free;

  for i:=0 to _collections.Count - 1 do
    if Assigned(_collections.Objects[i]) then
      _collections.Objects[i].Free;

  _checks.Destroy;
  _properties.Destroy;
  _methods.Destroy;
  _collections.Destroy;
  inherited
end;

function TSchemaObject.CheckAccess(const conversionObject: ICommon;
  const RequestAccess: AccessType): integer;
var
	i: integer;
	check: TCheck;
	Val: Variant;
  {raccess, }permitaccess: integer;
  ChecksResult: boolean;
begin
{  if RequestAccess = AccessRead then
  	raccess := 1
  else
  	raccess := 2;}

  if _reduceAccess = 0 then
    permitaccess := _access
  else begin
    ChecksResult := False;
    for i := 0 to _checks.Count - 1 do begin
      check := _checks.Items[i];
      try
      	// Получаем значение свойства
        Val := GetDispatchProperty(conversionObject, check._propertyName);
      except
        continue;// по каким-то причинам проверку не удалось выполнить
      end;
      // Если условие проверки выполняется, то возвращаем True
      case check._checkKind of
      0: if Val = check._propertyValue then begin
           ChecksResult := True; break
         end;
      1: if Val <> check._propertyValue then begin
      	   ChecksResult := True; break
      	 end;
      2: if Val < check._propertyValue then begin
      	   ChecksResult := True; break
      	 end;
      3: if Val > check._propertyValue then begin
      	   ChecksResult := True; break
      	 end
      end
    end;

    // Если проверки выполнены успешно, то уровень доступа остаётся прежним
    if ChecksResult then
      permitaccess := _access
    else
      // иначе понижаем уроветь доступа к свойству
      permitaccess := _access - _reduceAccess
  end;

  Result := permitaccess
{  if raccess <= permitaccess then
  	Result := True
  else
  	Result := False;}
end;

{ TXMLSchema }

constructor TXMLSchema.Create;
begin
  inherited;
//!!  FObjectList := TStringList.Create;
  _root := TSchemaObject.Create(nil);
  FEnums := TDSOEnumList.Create(self);
end;

destructor TXMLSchema.Destroy;
var
  i: Integer;
begin
  FEnums.Destroy;

  _root.Parent.Free;
  if Assigned(FObjectList) then begin
    for i := 0 to FObjectList.Count - 1 do begin
      FObjectList.Objects[i].Free;
      FObjectList.Objects[i] := nil
    end;
    FObjectList.Free
  end;
  inherited
end;

procedure TXMLSchema.LoadObjectsTree(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  i: integer;
  tmpObject: TSchemaObject;
  coll_name: string;
begin
  SchemaObject._name := xmlNode.attributes.getNamedItem('name').text;
  for i := 0 to xmlNode.childNodes.length - 1 do begin
    coll_name := xmlNode.childNodes.Item[i].attributes.getNamedItem('collection').text;
    SchemaObject._collections.Add(coll_name);
    tmpObject := TSchemaObject.Create(SchemaObject);
    SchemaObject._collections.Objects[SchemaObject._collections.IndexOf(coll_name)] := tmpObject;
    LoadObjectsTree(xmlNode.childNodes.Item[i], tmpObject)
  end
end;

procedure TXMLSchema.LoadCollection(const xmlNode: IXMLDOMNode; MetaObject: TSchemaObject);
var
  CollectionName: string;
  tmpObject: TSchemaObject;
  check: TCheck;
  i: integer;
  reduceNode: IXMLDOMNode;
begin
  CollectionName := xmlNode.attributes.getNamedItem('name').text;
  tmpObject := TSchemaObject(MetaObject.Collections.Objects[MetaObject.Collections.IndexOf(CollectionName)]);
  tmpObject._access := StrToInt(xmlNode.attributes.getNamedItem('access').text);

  try
    reduceNode := xmlNode.attributes.getNamedItem('reduce');
    if reduceNode <> nil then
      tmpObject._reduceAccess := StrToInt(reduceNode.text)
    else
      tmpObject._reduceAccess := 0
  except
    tmpObject._reduceAccess := 0
  end;

  if tmpObject._reduceAccess = 0 then exit;

  // Инициализируем проверки доступа к коллекции
  for i := 0 to xmlNode.childNodes.length - 1 do
    if xmlNode.childNodes.item[i].nodeName = 'checkvalue' then begin
      check := TCheck.Create;
      check._propertyName := xmlNode.childNodes.item[i].attributes.getNamedItem('property').text;
      check._propertyValue := xmlNode.childNodes.item[i].attributes.getNamedItem('value').text;
      try
      	check._checkKind := StrToInt(xmlNode.childNodes.item[i].attributes.getNamedItem('kind').text)
      except
      	check._checkKind := 0
      end;
      tmpObject._checks.Add(check);
    end
end;

// Загружает писание коллекции для объекта из XML
procedure TXMLSchema.LoadCollection2(const xmlNode: IXMLDOMNode; MetaObject: TSchemaObject);
var
  CollectionName, ObjectName: string;
  tmpObject: TSchemaObject;
  check: TCheck;
  i: integer;
  reduceNode, xmlObjectNode: IXMLDOMNode;
  xmlNodeList: IXMLDOMNodeList;
begin
  CollectionName := xmlNode.attributes.getNamedItem('name').text;
  ObjectName := xmlNode.attributes.getNamedItem('object').text;
  xmlObjectNode := xmlNode.selectSingleNode('../../../object[@name = ''' + ObjectName + ''']');
  LoadObject2(xmlObjectNode);
  tmpObject := TSchemaObject(FObjectList.Objects[FObjectList.IndexOf(ObjectName)]);
  tmpObject._parent := MetaObject;
  tmpObject._access := StrToInt(xmlNode.attributes.getNamedItem('access').text);

  try
    reduceNode := xmlNode.attributes.getNamedItem('reduce');
    if reduceNode <> nil then
      tmpObject._reduceAccess := StrToInt(reduceNode.text)
    else
      tmpObject._reduceAccess := 0
  except
    tmpObject._reduceAccess := 0
  end;

  if tmpObject._reduceAccess = 0 then exit;

  // Инициализируем проверки доступа к коллекции
  xmlNodeList := xmlNode.selectNodes('checkvalue');
  for i := 0 to xmlNodeList.length - 1 do begin
    check := TCheck.Create;
    check._propertyName := xmlNodeList.item[i].attributes.getNamedItem('property').text;
    check._propertyValue := xmlNodeList.item[i].attributes.getNamedItem('value').text;
    try
      check._checkKind := StrToInt(xmlNodeList.item[i].attributes.getNamedItem('kind').text)
    except
      check._checkKind := 0
    end;
    tmpObject._checks.Add(check);
  end
end;

procedure TXMLSchema.LoadProperty(const xmlNode: IXMLDOMNode;
  var Properties: TStringList; const SchemaObject: TSchemaObject);
var
  PropertyName: string;
  tmpProperty: TProperty;
  check: TCheck;
  i, enumIndx: integer;
  IsSaveNode, reduceNode, kindNode, enumNode, captionNode: IXMLDOMNode;
begin
  enumIndx := -1;
  PropertyName := xmlNode.attributes.getNamedItem('name').text;
  Properties.Add(PropertyName);
  tmpProperty := TProperty.Create;
  tmpProperty._name := xmlNode.attributes.getNamedItem('name').text;
  tmpProperty._type := StrToInt(xmlNode.attributes.getNamedItem('type').text);
  tmpProperty._access := StrToInt(xmlNode.attributes.getNamedItem('access').text);
  tmpProperty.Owner := SchemaObject;
  tmpProperty._parent := self;

  try
    reduceNode := xmlNode.attributes.getNamedItem('reduce');
    if reduceNode <> nil then
      tmpProperty._reduceAccess := StrToInt(reduceNode.text)
    else
      tmpProperty._reduceAccess := 0
  except
    tmpProperty._reduceAccess := 0
  end;

  try
    IsSaveNode := xmlNode.attributes.getNamedItem('saved');
    if IsSaveNode <> nil then
      tmpProperty._IsSaved := (IsSaveNode.text = '1')
    else
      tmpProperty._IsSaved := False
  except
    tmpProperty._IsSaved := False
  end;

  try
    enumNode := xmlNode.attributes.getNamedItem('enum');
    if enumNode <> nil then
      enumIndx := tmpProperty._parent.FEnums.FindEnumName(enumNode.text);
      tmpProperty.enum := enumIndx
  except
    tmpProperty.enum := -1
  end;

  captionNode := xmlNode.attributes.getNamedItem('caption');
  if Assigned(captionNode) then
    tmpProperty._caption := captionNode.text;

  captionNode := xmlNode.attributes.getNamedItem('secondpass');
  if captionNode <> nil then
    tmpProperty._IsSecondPass := (captionNode.text = '1')
  else
    tmpProperty._IsSecondPass := False;

  captionNode := xmlNode.attributes.getNamedItem('silent');
  if captionNode <> nil then
    tmpProperty._IsSilent := (captionNode.text = '1')
  else
    tmpProperty._IsSilent := False;

  if tmpProperty._type = dtObject then begin
    try
      enumNode := xmlNode.attributes.getNamedItem('object');
      if enumNode <> nil then
        tmpProperty._object := tmpProperty._parent.FindMetaObject(enumNode.text);
    except
      tmpProperty._object := nil
    end;
  end;

  Properties.Objects[Properties.IndexOf(PropertyName)] := tmpProperty;
  if tmpProperty._reduceAccess = 0 then exit;

  // Инициализируем проверки доступа к свойству
  for i := 0 to xmlNode.childNodes.length - 1 do
    if xmlNode.childNodes.item[i].nodeName = 'checkvalue' then begin
      check := TCheck.Create;
      check._propertyName := xmlNode.childNodes.item[i].attributes.getNamedItem('property').text;
      check._propertyValue := xmlNode.childNodes.item[i].attributes.getNamedItem('value').text;
      try
        kindNode := xmlNode.childNodes.item[i].attributes.getNamedItem('kind');
        if kindNode <> nil then
      	  check._checkKind := StrToInt(kindNode.text)
        else
          check._checkKind := 0
      except
      	check._checkKind := 0
      end;
      tmpProperty._checks.Add(check);
    end
end;

procedure TXMLSchema.LoadObjectProperties(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  i: Integer;
  xmlMethodsNode: IXMLDOMNode;
  xmlNodeList: IXMLDOMNodeList;
begin
  SchemaObject._interface := xmlNode.attributes.getNamedItem('interface').text;
  SchemaObject._guid := StringToGUID(xmlNode.attributes.getNamedItem('guid').text);
  SchemaObject._class := StrToInt(xmlNode.attributes.getNamedItem('class').text);

  // Загружаем коллекции
  xmlNodeList := xmlNode.selectNodes('collections/collection');
  for i := 0 to xmlNodeList.length - 1 do
    LoadCollection(xmlNodeList.item[i], SchemaObject);

  // Загружаем методы
  xmlMethodsNode := xmlNode.selectSingleNode('methods');
  if Assigned(xmlMethodsNode) then
    for i := 0 to xmlMethodsNode.attributes.length - 1 do
      SchemaObject._methods.Add(xmlMethodsNode.attributes.item[i].nodeName);

  // Загружаем свойства
  xmlNodeList := xmlNode.selectNodes('properties/property');
  for i := 0 to xmlNodeList.length - 1 do
    LoadProperty(xmlNodeList.item[i], SchemaObject._properties, SchemaObject);

end;

procedure TXMLSchema.LoadObjectProperties2(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  i: Integer;
  xmlMethodsNode: IXMLDOMNode;
  xmlNodeList: IXMLDOMNodeList;
begin
  // Считываем атрибуты объекта
  SchemaObject._name := xmlNode.attributes.getNamedItem('name').text;
  SchemaObject._interface := xmlNode.attributes.getNamedItem('interface').text;
  SchemaObject._guid := StringToGUID(xmlNode.attributes.getNamedItem('guid').text);
  SchemaObject._class := StrToInt(xmlNode.attributes.getNamedItem('class').text);

  // Загружаем коллекции
  xmlNodeList := xmlNode.selectNodes('collections/collection');
  for i := 0 to xmlNodeList.length - 1 do
    LoadCollection2(xmlNodeList.item[i], SchemaObject);

  // Загружаем методы
  xmlMethodsNode := xmlNode.selectSingleNode('methods');
  if Assigned(xmlMethodsNode) then
    for i := 0 to xmlMethodsNode.attributes.length - 1 do
      SchemaObject._methods.Add(xmlMethodsNode.attributes.item[i].nodeName);

  // Загружаем свойства
  xmlNodeList := xmlNode.selectNodes('properties/property');
  for i := 0 to xmlNodeList.length - 1 do
    LoadProperty(xmlNodeList.item[i], SchemaObject._properties, SchemaObject);
end;

procedure TXMLSchema.LoadDSOEnums(const xmlNodes: IXMLDOMNodeList);
var
  i, j: integer;
  xmlNode: IXMLDOMNode;
  xmlItemNodes: IXMLDOMNodeList;
  Enum: TDSOEnum;
  EnumItem: PEnumItem;
  MinID, MaxID: integer;
begin
  if xmlNodes = nil then exit;
  for i := 0 to xmlNodes.length - 1 do begin
    Enum := TDSOEnum.Create;
    xmlNode := xmlNodes.item[i];
    Enum.FName := xmlNode.attributes.getNamedItem('name').text;
    xmlItemNodes := xmlNode.selectNodes('enumitem');
    if xmlItemNodes = nil then Continue;
    MinID := 0; MaxID := 0;
    for j := 0 to xmlItemNodes.length - 1 do begin
      New(EnumItem);
      EnumItem.id := StrToInt('$' + xmlItemNodes.item[j].attributes.getNamedItem('id').text);
      if EnumItem.ID > MaxID then MaxID := EnumItem.ID
      else if EnumItem.ID < MinID then MinID := EnumItem.ID;
      EnumItem.Name := xmlItemNodes.item[j].attributes.getNamedItem('name').text;
      EnumItem.Caption := xmlItemNodes.item[j].attributes.getNamedItem('caption').text;
      Enum.Add(EnumItem);
    end;
    Enum.FMin := MinID;
    Enum.FMax := MaxID;
    FEnums.Add(Enum);
  end;
end;

procedure TXMLSchema.LoadObjects(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  i: integer;
  tmpObject: TSchemaObject;
begin
  for i := 0 to xmlNode.childNodes.length - 1 do begin
    if xmlNode.childNodes.item[i].nodeName <> 'object' then continue;
    if xmlNode.childNodes.item[i].attributes.getNamedItem('name').text = SchemaObject._name then begin
      LoadObjectProperties(xmlNode.childNodes.item[i], SchemaObject);
      break;
    end
  end;

  for i := 0 to SchemaObject._collections.Count - 1 do begin
    tmpObject := TSchemaObject(SchemaObject._collections.Objects[i]);
    LoadObjects(xmlNode, tmpObject)
  end
end;

procedure TXMLSchema.LoadObject2(const xmlNode: IXMLDOMNode);
var
  SchemaObject: TSchemaObject;
  num: string;
begin
  num := xmlNode.attributes.getNamedItem('name').text;
  if FObjectList.IndexOf(num) <> -1 then
    exit;
  // Создаем новый мета-объект
  SchemaObject := TSchemaObject.Create(nil);

  // Загружаем свойства, методы и коллекции объекта
  LoadObjectProperties2(xmlNode, SchemaObject);

  // Добавляем объект в глобальный список объектов
  FObjectList.AddObject(SchemaObject.Name, SchemaObject);
end;

procedure TXMLSchema.LoadObjectServerProperties(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  i: Integer;
  xmlMethodsNode: IXMLDOMNode;
  xmlNodeList: IXMLDOMNodeList;
begin
  // Считываем атрибуты объекта
  SchemaObject._name := xmlNode.attributes.getNamedItem('name').text;
  SchemaObject._interface := xmlNode.attributes.getNamedItem('interface').text;
  SchemaObject._guid := StringToGUID(xmlNode.attributes.getNamedItem('guid').text);
  SchemaObject._class := StrToInt(xmlNode.attributes.getNamedItem('class').text);

  // Загружаем методы
  xmlMethodsNode := xmlNode.selectSingleNode('methods');
  if Assigned(xmlMethodsNode) then
    for i := 0 to xmlMethodsNode.attributes.length - 1 do
      SchemaObject._methods.Add(xmlMethodsNode.attributes.item[i].nodeName);

  // Загружаем свойства
  xmlNodeList := xmlNode.selectNodes('properties/property');
  for i := 0 to xmlNodeList.length - 1 do
    LoadProperty(xmlNodeList.item[i], SchemaObject._properties, SchemaObject);

  // Загружаем коллекции
//  xmlNodeList := xmlNode.selectNodes('collections/collection');
//  for i := 0 to xmlNodeList.length - 1 do
//    LoadCollection2(xmlNodeList.item[i], SchemaObject);
end;

procedure TXMLSchema.LoadServerNode(const xmlNode: IXMLDOMNode;
  var SchemaObject: TSchemaObject);
var
  xmlServerNode: IXMLDOMNode;
  ServerObject: TSchemaObject;
  Indx: Integer;
begin
  xmlServerNode := xmlNode.selectSingleNode('object[@name = ''Server'']');
  if not Assigned(xmlServerNode) then
    raise Exception.Create('Не найден элемент <object name="Server"...');

  // Создаем мета-объект для сервера
  ServerObject := TSchemaObject.Create(nil);

  // Загружаем свойства, методы и коллекции объекта
  LoadObjectServerProperties(xmlServerNode, ServerObject);

  // Делаем сервер родителем всех объектов, хотя фиктивно
  // кокневым элементом будет база данных
  SchemaObject._parent := ServerObject;
  Indx := ServerObject.Collections.Add('MDStores');
  SchemaObject._access := 2;
  ServerObject.Collections.Objects[Indx] := SchemaObject;
end;

function TXMLSchema.LoadFromXML(const filePathXml: string): boolean;
var
  xmlDocoment: IXMLDOMDocument2;
  xmlElement: IXMLDOMElement;
  xmlNode: IXMLDOMNode;
  i: Integer;
  StrFileName: OleVariant;
begin
  xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
  StrFileName := filePathXml;
  xmlDocoment.load(StrFileName);

  xmlElement := xmlDocoment.documentElement;
  if xmlElement.tagName <> 'dsoxmlschema' then ;

  LoadDSOEnums(xmlElement.selectNodes('/dsoxmlschema/enums/enum'));

{  xmlNodeList := xmlElement.selectNodes('objects/object');
  for i := 0 to xmlNodeList.length - 1 do
    LoadObject2(xmlNodeList.item[i]);
}

  xmlNode := xmlElement.selectSingleNode('//treenodes');
  if xmlNode.nodeName <> 'treenodes' then ;

  for i := 0 to xmlNode.childNodes.length - 1 do begin
    LoadObjectsTree(xmlNode.childNodes.Item[i], _root);
  end;


  xmlNode := xmlElement.selectSingleNode('//objects');
  xmlElement := nil;

  LoadObjects(xmlNode, _root);

  LoadServerNode(xmlNode, _root);

  Result := true
end;

function TXMLSchema.FindMetaObjectByClass(ObjectClass: Integer): TSchemaObject;
var
  FindedObject: TSchemaObject;
  function FindMetaObject2(RootObject: TSchemaObject): boolean;
  var
    I: Integer;
  begin
    Result := False;
    if RootObject.ObjectClass = ObjectClass then begin
      FindedObject := RootObject;
      Result := true;
      exit
    end;
    for I := 0 to RootObject._collections.Count - 1 do
      if FindMetaObject2(TSchemaObject(RootObject._collections.Objects[I])) then begin
        Result := true;
        exit
      end;
  end;
begin
  if _root._parent = nil then begin
    Result := nil;
    exit;
  end;
  if FindMetaObject2(_root._parent) then
    Result := FindedObject
  else
    Result := nil
end;

function TXMLSchema.FindMetaObject(ObjectName: string): TSchemaObject;
var
  FindedObject: TSchemaObject;
  function FindMetaObject2(RootObject: TSchemaObject): boolean;
  var
    I: Integer;
  begin
    Result := False;
    if RootObject._name = ObjectName then begin
      FindedObject := RootObject;
      Result := true;
      exit
    end;
    for I := 0 to RootObject._collections.Count - 1 do
      if FindMetaObject2(TSchemaObject(RootObject._collections.Objects[I])) then begin
        Result := true;
        exit
      end;
  end;
begin
  if _root._parent = nil then begin
    Result := nil;
    exit;
  end;
  if FindMetaObject2(_root._parent) then
    Result := FindedObject
  else
    Result := nil
end;

{ TDSOEnum }

destructor TDSOEnum.Destroy;
begin
  Clear;
  inherited Destroy;
end;

procedure TDSOEnum.Add(EnumItem: PEnumItem);
begin
  inherited Add(EnumItem);
end;

procedure TDSOEnum.Clear;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do DeleteItem(I);
  inherited;
end;

procedure TDSOEnum.DeleteItem(Index: Integer);
var
  E: PEnumItem;
begin
  E := Items[Index];
  E^.Name := '';
  E^.Caption := '';
  FreeMem(E);
end;

function TDSOEnum.FindItemCaption(const ItemID: string): Integer;
var
  S, EnumItem: string;
  I: Integer;
begin
  Result := -1;
  S := ItemID;
  while S <> '' do        // Expand subproperties
  begin
    I := Pos('\', S);
    if I > 0 then
    begin
      EnumItem := Copy(S, 1, I - 1);
      System.Delete(S, 1, I);
    end
    else
    begin
      EnumItem := S;
      S := '';
    end;

    I := IndexOfItemID(StrToInt(EnumItem), Succ(Result));
    if I <= Result then Exit;
    Result := I;
  end;
end;

function TDSOEnum.FindItemIDByName(const ItemName: string): Integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 0 to Count - 1 do
    if TEnumItem(Items[I]^).Name = ItemName then
    begin
      Result := I;
      Exit;
    end;
end;

function TDSOEnum.FindItemName(const ItemID: string): Integer;
begin
  Result := -1
end;

function TDSOEnum.GetEnumItem(AIndex: Integer): PEnumItem;
begin
  Result := Items[AIndex];
end;

function TDSOEnum.IndexOfItemID(const ItemID: integer;
  StartIndex: Integer): Integer;
var
  I: Integer;
begin
  if StartIndex < Count then
  begin
    Result := 0;
    for I := StartIndex to Count - 1 do
      if TEnumItem(Items[I]^).ID = ItemID then
      begin
        Result := I;
        Exit;
      end;
  end
  else
    Result := -1;
end;

{ TDSOEnumList }

constructor TDSOEnumList.Create(AOwner: TXMLSchema);
begin
  inherited Create;
  FOwner := AOwner;
end;

destructor TDSOEnumList.Destroy;
begin
  Clear;
  inherited Destroy;
end;

procedure TDSOEnumList.Add(DSOEnum: TDSOEnum);
begin
  inherited Add(DSOEnum);
end;

procedure TDSOEnumList.Clear;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
    DeleteEnum(I);
  inherited;
end;

procedure TDSOEnumList.DeleteEnum(Index: Integer);
var
  E: TDSOEnum;
begin
  E := Enums[Index];
  E.Destroy;
end;

function TDSOEnumList.FindEnumName(const EnumName: string): Integer;
var
  S, Enum: string;
  I: Integer;
begin
  Result := -1;
  S := EnumName;
  while S <> '' do        // Expand subproperties
  begin
    I := Pos('\', S);
    if I > 0 then
    begin
      Enum := Copy(S, 1, I - 1);
      System.Delete(S, 1, I);
    end
    else
    begin
      Enum := S;
      S := '';
    end;

    I := IndexOfEnumName(Enum, Succ(Result));
    if I <= Result then Exit;
    Result := I;
  end;
end;

function TDSOEnumList.GetEnum(AIndex: Integer): TDSOEnum;
begin
  Result := Items[AIndex];
end;

function TDSOEnumList.IndexOfEnumName(const EnumName: string;
  StartIndex: Integer): Integer;
var
  I: Integer;
begin
  if StartIndex < Count then
  begin
    Result := 0;
    for I := StartIndex to Count - 1 do
      if Enums[I].Name = EnumName then
      begin
        Result := I;
        Exit;
      end;
  end
  else
    Result := -1;
end;

end.
