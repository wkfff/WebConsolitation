unit uDsoXmlConverter;

interface

uses
  Windows, Classes, ComObj, ActiveX, SysUtils, DSO80_TLB, MSXML2_TLB, CommonTools_TLB,
  uDsoXmlSchema;

const
  DsoXmlConverterVersion = '1.0';
  MAX_DSOPATH = 10;

type
  TScriptingOptions = (
    SelectedObjectOnly,
    SelectedObjectAndChildrens,
    SelectedObjectAndSiblings
  );

  TConversionOptions = (
    ConvertFromXML,
    ConvertFromDSOAdd,
    ConvertFromDSOUpdate,
    ConvertFromDSOReplace,
    ConvertFromDSORemove
  );

  // Элемент пути к DSO объекту
  TDSOObjectPathNode = record
    PathLen: Integer;   // Длина последующего пути включая текущий элемент
    dsoClass: Integer;  // Класс DSO объекта
    dsoName: string;    // Имя DSO объекта
  end;

  // Путь к DSO объекту
  TDSOObjectPath = array[1..MAX_DSOPATH] of TDSOObjectPathNode;

  TDSOXMLConverterProcessEvent = procedure (const stringMsg: string; const ProcessedDSOObject: ICommon; const ProcessedMetaObject: TSchemaObject) of object;

  TDsoXmlConverter = class
  private
    _server: Server;
    _ErrLog: TStringList;
    _xmlSchema: TXMLSchema;
    _Progress: IProgress;
    fOnProcessEvent: TDSOXMLConverterProcessEvent;
    fOnErrorEvent: TDSOXMLConverterProcessEvent;
    _dsoDB: DataBase;
    fMacrosList: TStrings;
    fReplaceList: TStrings;
    procedure LogError(const ErrMsg : string);
    procedure MetaLogError(const ErrMsg: string);
    procedure LoadDatabaseMacros(const conversionObject: ICommon; xmlNode: IXMLDOMNode);
    procedure SaveDSOAncestors(const conversionObject: ICommon; var xmlNode: IXMLDOMElement; var MetaObject: TSchemaObject);
    procedure SaveDSOObjectProperties(const conversionObject: ICommon; var xmlNode: IXMLDOMElement; var MetaObject: TSchemaObject; const Method: string; const SaveAllProperties: boolean; PadStr: string);
    procedure SaveCustomProperties(const conversionObject: ICommon; var xmlNode: IXMLDOMElement; var MetaObject: TSchemaObject; const Method: string; const SaveAllProperties: boolean);
    procedure SaveDSOObjects(const conversionObject: ICommon; var xmlNode: IXMLDOMElement; var MetaObject: TSchemaObject; const Method: string; PadStr: string; const SaveConnString: Boolean);
    procedure RestoreObjectProperties(const conversionObject: ICommon; const xmlNode: IXMLDOMNode; const MetaObject: TSchemaObject; const IsSecondPass: Boolean);
    procedure RestoreObjectCustomProperties(const conversionObject: ICommon; const xmlNode: IXMLDOMNode; const MetaObject: TSchemaObject);
    procedure RestoreDSOObject(const conversionObject: ICommon; const xmlNode: IXMLDOMNode; const MetaObject: TSchemaObject; const ParentInterface: TGUID);
    function ApplyReplaces(const InString: string): string;
  public
    constructor Create;
    destructor Destroy; override;
    procedure RemoveMacroValues(const InputString: string; var OutputString: string);
    procedure ProcessEvent(const stringMsg: string; const ProcessedDSOObject: ICommon; const ProcessedMetaObject: TSchemaObject);
    procedure ErrorEvent(const stringMsg: string; const ProcessedDSOObject: ICommon; const ProcessedMetaObject: TSchemaObject);
    procedure FindDSOMetaObject(const conversionObject: ICommon; var MetaObject: TSchemaObject);
    function FindDSOObject(const srv: Server; const Path: string): ICommon;
    function GetDSOObjectPath(const dsoObject: ICommon; var Path: TDSOObjectPath): Boolean;
    function ConvertDSO2XML(const conversionObject: ICommon;
      var xmlDocoment: IXMLDomDocument2;
      const ConversionOptions: TConversionOptions;
      const ScriptingOptions: TScriptingOptions): HResult;
    function ConvertDSO2XMLFile(serverNameOlap: string; fs: TFileStream;
      DSOPath: string; ConversionOptions: TConversionOptions;
      const ScriptingOptions: TScriptingOptions): HResult;
    function ConvertXMLFile2DSO(filePathXml, serverNameOlap: string): HResult;
    function ConvertXML2DSO(const srv: Server; const xmlNode: IXMLDOMElement): HResult;
    procedure ProcessXMLPackage(const filePathXml, serverNameOlap: string);
    procedure LoadMacrosList(const filePathXml: string);
    procedure LoadReplaceList(const filePathXml: string);
  published
    property Schema: TXMLSchema read _xmlSchema write _xmlSchema;
    property ErrorLog: TStringList read _ErrLog write _ErrLog;
    property Progress: IProgress read _Progress write _Progress;
    property MacrosList: TStrings read fMacrosList;
    property ReplaceList: TStrings read fMacrosList;
    property OnProcessEvent: TDSOXMLConverterProcessEvent read fOnProcessEvent write fOnProcessEvent;
    property OnErrorEvent: TDSOXMLConverterProcessEvent read fOnErrorEvent write fOnErrorEvent;
  end;

  TGetMacrosListProc = procedure(var MacrosList: TStrings) of object;

function GetCollectionByName(
  const conversionObject: ICommon; const CollectionName: string;
  const InterfaceGUID: TGUID): OlapCollection;

procedure UpdateDSOObject(
  const DSOObject: ICommon; const MetaObject: TSchemaObject);

{!!procedure ConvertValueStringToMacroString(
  const OrignString: string; var MacroString: string; GetMacrosProc: TGetMacrosListProc);
}

function ApplyMacros(const InputString: string; var OutputString: string; const MacrosList: TStrings): Integer;

function GetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; MacrosList: TStrings): string; overload;

function GetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; GetMacrosProc: TGetMacrosListProc): string; overload;

function SetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; MacrosList: TStrings;
  const MacroString: string): string; overload;

function SetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; GetMacrosProc :TGetMacrosListProc;
  const MacroString: string): string; overload;

implementation

uses
  uDispatchInvoke, uXMLUtils, VBA_TLB;

function GetCollectionByName(const conversionObject: ICommon;
  const CollectionName: string; const InterfaceGUID: TGUID): OlapCollection;
var
  DispIDs: array[0..MaxDispArgs - 1] of Integer;
  Results: Variant;
  CallDesc: TCallDesc;
  Params: pDispParams;
  tmpd: IDispatch;
begin
  CallDesc.CallType := DISPATCH_PROPERTYGET;
  CallDesc.ArgCount := 0;
  CallDesc.NamedArgCount := 0;
  IUnknown(conversionObject).QueryInterface(InterfaceGUID, tmpd);
  if tmpd = nil then
    raise Exception.CreateFmt(
      'GetCollectionByName: Невозможно получить интерфейс %s для объекта [%s]',
      [GUIDToString(InterfaceGUID), conversionObject.Name]);
  GetIDsOfNames(IDispatch(tmpd), PChar(CollectionName), 1, @DispIDs);
  if DispIDs[0] < 0 then
    raise Exception.CreateFmt(
      'GetCollectionByName: Невозможно получить коллекцию %s для объекта [%s]',
      [CollectionName, conversionObject.Name]);
  DispatchInvoke(IDispatch(tmpd), @CallDesc, @DispIDs, @Params, @Results);
  Result := IDispatch(TVarData(Results).VDispatch) as olapCollection;
end;

procedure UpdateDSOObject(const DSOObject: ICommon; const MetaObject: TSchemaObject);
var
  Index: Integer;
  tmpMetaObject: TSchemaObject;
  DsoCommon: ICommon;
begin
  tmpMetaObject := MetaObject;
  DsoCommon := DSOObject;
  while not tmpMetaObject.Methods.Find('Update', Index) do begin
    tmpMetaObject := tmpMetaObject.Parent;
    DsoCommon := DsoCommon.Get_Parent;
  end;
  InvokeMetod(DsoCommon as IDispatch, 'Update', OleVarArrayOf([]), OleVarArrayOf([]));
end;
{!!
function ConvertValueStringToMacroString(
  const OrignString: string; var MacroString:
  string; GetMacrosProc: TGetMacrosListProc): Boolean;
var
  MacrosList: TStrings;
begin
  GetMacrosProc(MacrosList);



  MacroString := OrignString;
end;
}
function CheckValueStringAndMacroString(const OrignString, MacroString: string): Boolean;
begin
//!!  Result := (Length(OrignString) = Length(MacroString));
  Result := True
end;

// Функция возвращает строковое значение свойства DSO-объекта
// с мета-определениями, если они определены или обычное значение
function GetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; MacrosList: TStrings): string;
var
  i: Integer;
  Prop: _Property;
  Disp: IDispatch;
  CustomPropName: string;
  MacroString: string;
  OrignString: string;
begin
  // Если у объекта в коллекции CustomProperties есть свойство
  // с именем "@PropName", то возвращаем строку с мета-определениями,
  // иначе просто вернём значение свойства

  // Находим свойства в CustomProperties
  CustomPropName := '@' + PropName;
  for i := 1 to DsoObject.CustomProperties.Count do
    if DsoObject.CustomProperties.Item[i].Name = CustomPropName then
      Prop := DsoObject.CustomProperties.Item[i];

  // Получаем значение свойства
  DsoObject.QueryInterface(MetaObject.InterfaceGUID, Disp);
  OrignString := GetDispatchProperty(Disp, PropName);

  if not (Assigned(Prop) and Assigned(MacrosList)) then
    // Если не нашли, то возвращаем оригиналиное значение свойства
    Result := OrignString
  else begin
    // А если нашли, то возвращаем теневое значение свойства
    // в котором все значения макросов заменены на определеня макросов
    MacroString := Prop.Value;
    // Проверим на соответствие значение свойства теневому значению свойства
    if not CheckValueStringAndMacroString(OrignString, MacroString) then
      raise Exception.CreateFmt(
        'Значение свойства не соответствует теневому значению. Текущее:(%s) Теневое:(%s)',
        [OrignString, MacroString]);
    // Если все нормально, то возвращаем значение с макросами
    Result := MacroString
  end;
end;

// Функция возвращает строковое значение свойства DSO-объекта
// с мета-определениями, если они определены или обычное значение
function GetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; GetMacrosProc: TGetMacrosListProc): string;
var
  MacrosList: TStrings;
begin
  GetMacrosProc(MacrosList);
  Result := GetDSOObjectPropMetaValue(DsoObject, PropName, MetaObject, MacrosList);
end;

// Синхранизируем значение теневого свойства:
procedure SynchronizeCustomProp(const PropName, MacroString: string;
  const DsoObject: ICommon; const IsPresentMacros: Boolean);
var
  Prop: _Property;
  CustomPropName: string;
  i: Integer;
begin
  // 1. Находим теневое свойство в CustomProperties
  CustomPropName := '@' + PropName;
  for i := 1 to DsoObject.CustomProperties.Count do begin
    if DsoObject.CustomProperties.Item[i].Name = CustomPropName then
      Prop := DsoObject.CustomProperties.Item[i];
  end;

  // 2. Если в значении свойства были макро-определения,
  // то обновляем (добавляем новое) теневое свойство
  if IsPresentMacros then begin
    if Assigned(Prop) then begin
      // Если нашли, то обновляем существующее свойство
      Prop.Value := MacroString;
    end else begin
      // Если не нашли, то добавляем новое свойство
      DsoObject.CustomProperties.Add(MacroString, CustomPropName, vbString);
    end
  end else
    // А если макро-определений не было, но существует теневое свойство,
    // то удаляем его
    if Assigned(Prop) then
      DsoObject.CustomProperties.Remove(Prop.Name)
end;

// Функция устанавливает строковое значение свойства DSO-объекта
// с мета-определениями, заменяя мета-определения на их значения,
// а в теневую копию записявает значение без замены мета-определений на их значения
function SetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; MacrosList: TStrings;
  const MacroString: string): string; overload;
var
  Disp: IDispatch;
  OrignString: string;
  IsPresentMacros: Boolean;
begin
  // Делаем подстановку макро-определений
  IsPresentMacros := (ApplyMacros(MacroString, OrignString, MacrosList) > 0);

  // Сохраняем значение свойства с подстановленными макро-определениями
  IUnknown(DsoObject).QueryInterface(MetaObject.InterfaceGUID, Disp);
  PutDispatchProperty(Disp, PropName, OrignString);

  // Синхранизируем значение теневого свойства:
  SynchronizeCustomProp(PropName, MacroString, DsoObject, IsPresentMacros);
end;

// Функция устанавливает строковое значение свойства DSO-объекта
// с мета-определениями, заменяя мета-определения на их значения,
// а в теневую копию записявает значение без замены мета-определений на их значения
function SetDSOObjectPropMetaValue(
  const DsoObject: ICommon; const PropName: string;
  const MetaObject: TSchemaObject; GetMacrosProc :TGetMacrosListProc;
  const MacroString: string): string;
var
  MacrosList: TStrings;
begin
  GetMacrosProc(MacrosList);
  SetDSOObjectPropMetaValue(DsoObject, PropName, MetaObject, MacrosList, MacroString);
end;

const
  START_MACRO = '/*#';
  END_MACRO = '#*/';
  MACRO_LEN = Length(START_MACRO);

function FindMacro(const InputString: string; const StartPos: Integer; var MacroName: string; var Len: Integer): Integer;
var
  InString: string;
  StartMacro, EndMacro: Integer;
begin
  Result := -1;
  InString := Copy(InputString, StartPos, Length(InputString) - StartPos + 1);

  // Находим начало макроса
  StartMacro := Pos(START_MACRO, InString);
  if StartMacro = 0 then exit;

  // Находим конец макроса
  InString := Copy(InputString, StartMacro + MACRO_LEN, Length(InputString) - StartMacro - MACRO_LEN + 1);
  EndMacro := Pos(END_MACRO, InString);
  if EndMacro = 0 then begin Result := -1; exit end;

  // Извлекаем имя макроса
  MacroName := Copy(InString, 1, EndMacro - 1);

  // Вычисляем длину макроса
  Len := EndMacro + MACRO_LEN*2 - 1;

  Result := StartMacro;

  // Пытаемся найти пустой макрос,
  // который определяет конец подстановленного значения
  InString := Copy(InString, EndMacro + 3, Length(InString) - EndMacro - 2);
  StartMacro := Pos(START_MACRO, InString);
  if StartMacro = 0 then exit;
  EndMacro := Pos(END_MACRO, InString);
  if EndMacro = 0 then begin Result := -1; exit end;
  if (EndMacro - StartMacro) = Length(START_MACRO) then
    Len := Len + EndMacro + 2;
end;

// Заменяет в строке макро-определения на их значения
function ApplyMacros(const InputString: string;
  var OutputString: string; const MacrosList: TStrings): Integer;
var
  MacroName, InString, MacroValue: string;
  MacroStart, MacroLen: Integer;
  MacrosCount: Integer;
begin
  MacrosCount := 0;
  OutputString := '';
  // Формируем входную строку
  InString := InputString;
  while Length(InString) > 0 do begin
    // Находим положение макро-определения
    MacroStart := FindMacro(InString, 1, MacroName, MacroLen);
    // Если макро-определение не найдено, то завершаем цикл
    if MacroStart <= 0 then break;
    // Копируем в выходную строку подстроку до макро-определения
    OutputString := OutputString + Copy(InString, 1, MacroStart - 1) {+ START_MACRO + MacroName + END_MACRO};
    // Получаем значение макроса
    if MacrosList.IndexOfName(MacroName) = -1 then
      raise Exception.CreateFmt('Не найдено значение макроса #%s', [MacroName]);
    MacroValue := MacrosList.Values[MacroName];
    // Копируем в выходную строку значение макро-определения
    OutputString := OutputString + MacroValue;
    // Формируем входную строку
    InString := Copy(InString, MacroStart + MacroLen, Length(InString) - MacroStart - MacroLen + 1);
    // Увеличиваем количество обработанных макро-определений
    Inc(MacrosCount)
  end;
  // Копируем в выходную строку подстроку идущую за макро-определением
  OutputString := OutputString + InString;
  Result := MacrosCount;
end;

procedure TDsoXmlConverter.RemoveMacroValues(const InputString: string;
  var OutputString: string);
var
  MacroName, InString: string;
  MacroStart, MacroLen: Integer;
begin
  OutputString := '';
  InString := InputString;
  while Length(InString) > 0 do begin
    MacroStart := FindMacro(InString, 1, MacroName, MacroLen);
    if MacroStart <= 0 then break;
    OutputString := OutputString + Copy(InString, 1, MacroStart + Length(MacroName) + MACRO_LEN*2 - 1);
    InString := Copy(InString, MacroStart + MacroLen, Length(InString) - MacroStart - MacroLen + 1);
  end;
  OutputString := OutputString + InString;
end;

{ TDsoXmlConverter }

constructor TDsoXmlConverter.Create;
begin
  inherited;
  _server := CreateComObject(CLASS_Server) as Server;
  _xmlSchema := TXMLSchema.Create;
  _xmlSchema.LogError := MetaLogError;
  fMacrosList := TStringList.Create;
  fReplaceList := TStringList.Create;
end;

destructor TDsoXmlConverter.Destroy;
begin
  fReplaceList.Free;
  fMacrosList.Free;
  _xmlSchema.Destroy;
  _server := nil;
  inherited;
end;

procedure TDsoXmlConverter.ErrorEvent(const stringMsg: string;
  const ProcessedDSOObject: ICommon;
  const ProcessedMetaObject: TSchemaObject);
begin
  if Assigned(fOnErrorEvent) then
    fOnErrorEvent(stringMsg, ProcessedDSOObject, ProcessedMetaObject)
end;

procedure TDsoXmlConverter.ProcessEvent(const stringMsg: string;
  const ProcessedDSOObject: ICommon;
  const ProcessedMetaObject: TSchemaObject);
begin
  if Assigned(Progress) then
    Progress.ProgressMsg := stringMsg;
  if Assigned(fOnProcessEvent) then
    fOnProcessEvent(stringMsg, ProcessedDSOObject, ProcessedMetaObject)
end;

procedure TDsoXmlConverter.LoadDatabaseMacros(
  const conversionObject: ICommon; xmlNode: IXMLDOMNode);
var
{  xmlNodeList: IXMLDOMNodeList;}
  i: Integer;
  PropName: string;
  dsoProp: _Property;
begin
  // Считываем макро-определеня для базы данных из XML элемента
{  xmlNodeList := xmlNode.selectNodes('CustomProperties/Property');
  for i := 0 to xmlNodeList.length - 1 do begin
    PropName := xmlNodeList.item[i].attributes.getNamedItem('name').text;
    if PropName[1] = '#' then begin
      PropName := Copy(PropName, 2, Length(PropName) - 1);
      if fMacrosList.IndexOfName(PropName) = -1 then begin
        fMacrosList.Values[PropName] := xmlNodeList.item[i].text;
      end
    end
  end;}
  // Считываем макро-определеня для базы данных из CustomProperties
  for i := 1 to conversionObject.CustomProperties.Count do begin
    dsoProp := conversionObject.CustomProperties.Item[i];
    if dsoProp.Name[1] = '#' then begin
      PropName := Copy(dsoProp.Name, 2, Length(dsoProp.Name) - 1);
      if fMacrosList.IndexOfName(PropName) = -1 then
        fMacrosList.Values[PropName] := dsoProp.Value;
    end
  end;

  for i := 0 to fMacrosList.Count - 1 do begin
    PropName := fMacrosList.Names[i];
try
    conversionObject.CustomProperties.Add(
        fMacrosList.Values[PropName], '#' + PropName, vbString);
except
end;
  end
end;

// Сохраняет свойства DSO объекта conversionObject, в XML узел
procedure TDsoXmlConverter.SaveDSOObjectProperties(
  const conversionObject: ICommon; var xmlNode: IXMLDOMElement;
  var MetaObject: TSchemaObject; const Method: string;
  const SaveAllProperties: boolean; PadStr: string);
var
  i: integer;
  Prop: TProperty;
  PropValue: Variant;
  Disp: IDispatch;
  xmlChildNode: IXMLDOMElement;
  xmlCDATANode: IXMLDOMNode;
  PermitedAccess: integer;
  str: string;
begin
  // Выводим сообщение о ходе выполнения
  ProcessEvent(PadStr + MetaObject.Name + ': '
    + conversionObject.Name, conversionObject, MetaObject);

  // Сохраняем общие свойства для всех DSO объектов
  xmlNode.setAttribute('method', Method);
  xmlNode.setAttribute('name', GetDSOObjectPropMetaValue(conversionObject, 'name', MetaObject, fMacrosList));
  xmlNode.setAttribute('ClassType', conversionObject.ClassType);
  xmlNode.setAttribute('SubClassType', conversionObject.SubClassType);

  if not SaveAllProperties then exit;

  // Запрашиваем конкретный интерфейс для обращения к специфическим
  // свойствам объекта
  IUnknown(conversionObject).QueryInterface(MetaObject.InterfaceGUID, Disp);

  // Перебираем все возможные свойства у объекта
  for i := 0 to MetaObject.Properties.Count - 1 do begin
    Prop := TProperty(MetaObject.Properties.Objects[i]);
{    if Prop.DataType = dtObject then begin
			PermitedAccess := 0;
    end;}
    if Prop = nil then continue; // по идее этого не должно быть, но все таки происходит - ГЛЮК!!!

    // Проверяем доступность свойства
    PermitedAccess := Prop.CheckAccess(Disp, accessRead);
    if (PermitedAccess = 0) and not Prop.IsSaved then
      continue;
    if Prop.Name = 'Name' then
      continue;

    // Сохраняем только те свойства, которые потом можно будет восстановить
    if (PermitedAccess = 2) or Prop.IsSaved then begin
      try
        // Получаем значение свойства
        // Если свойство имеет строковыи тип, то получаем значение с макро-определениями
        if (Prop.DataType = dtString) or (Prop.DataType = dtCDATA) then
          PropValue := GetDSOObjectPropMetaValue(Disp as ICommon, Prop.Name, MetaObject, fMacrosList)
        else
          PropValue := GetDispatchProperty(Disp, Prop.Name);
      except
        ErrorEvent(Format('Свойство не найдено: %s. для %s  ClassType=%d SubClassType=%d',
          [MetaObject.Name, Prop.Name, conversionObject.Name,
          conversionObject.ClassType, conversionObject.SubClassType]),
          conversionObject, MetaObject);
        continue;
      end;

      if Prop.DataType = dtBoolean then
        // Булевское значение
      	if PropValue then
          xmlNode.setAttribute(Prop.Name, 'true')
      	else
          xmlNode.setAttribute(Prop.Name, 'false')
      else if Prop.DataType = dtObject then begin
        // Объект. Сохраняем имя объекта
      	try
          str := PropValue.Name;
        except
          on E: Exception do begin
            ErrorEvent(Format('Свойство %s не доступно для объекта %s:[%s] (%s)',
              [Prop.Name, MetaObject.Name, conversionObject.Name, E.Message]),
              conversionObject, MetaObject);
            continue
          end
        end;
        xmlNode.setAttribute(Prop.Name, str)
      end else
        // Сохраняем элемент в секции CDATA
        if Prop.DataType = dtCDATA then begin
          xmlChildNode := xmlNode.ownerDocument.createElement('property');
          xmlNode.appendChild(xmlChildNode);
          xmlChildNode.setAttribute('name', Prop.Name);
          xmlCDATANode := xmlNode.ownerDocument.createCDATASection(PropValue);
          xmlChildNode.appendChild(xmlCDATANode);
        end else
          xmlNode.setAttribute(Prop.Name, PropValue)
    end;
  end; // for
end;

procedure TDsoXmlConverter.SaveCustomProperties(
  const conversionObject: ICommon; var xmlNode: IXMLDOMElement;
  var MetaObject: TSchemaObject; const Method: string;
  const SaveAllProperties: boolean);
var
  i, PropCount: Integer;
  xmlChildNode: IXMLDOMElement;
  xmlChildNode2: IXMLDOMElement;
  xmlCDATANode: IXMLDOMNode;
  Prop: _Property;
begin
  if not SaveAllProperties then exit;
  PropCount := conversionObject.CustomProperties.Count;
  if PropCount = 0 then exit;
  xmlChildNode := xmlNode.ownerDocument.createElement('CustomProperties');
  xmlNode.appendChild(xmlChildNode);
  for i := 1 to PropCount do begin
    Prop := conversionObject.CustomProperties.Item[I];
    // Теневые значения свойств не сохраняем
    if Prop.Name[1] = '@' then continue;
    xmlChildNode2 := xmlNode.ownerDocument.createElement('Property');
    xmlChildNode.appendChild(xmlChildNode2);
    xmlChildNode2.setAttribute('name', Prop.Name);
    xmlChildNode2.setAttribute('datatype', Prop.DataType);
    xmlCDATANode := xmlNode.ownerDocument.createCDATASection(Prop.Value);
    xmlChildNode2.appendChild(xmlCDATANode);
    Prop := nil;
  end
end;

procedure TDsoXmlConverter.FindDSOMetaObject(const conversionObject: ICommon; var MetaObject: TSchemaObject);
var
  Ancestor: ICommon;
  indx: integer;
begin
  // Если наткнулись на сервер, то уууууу......
  if conversionObject.ClassType = clsServer then begin
    // Возвращаем реального предка от фиктивного
    MetaObject := _xmlSchema.Root.Parent;
    exit;
  end;
  // Получаем предка для текущего объекта
  Ancestor := conversionObject.Get_Parent;
  // если имеем предка, то обработаем его
  if Ancestor <> nil then begin
    if Ancestor.ClassType <> clsServer then begin
      // Входим в рекурсию
      FindDSOMetaObject(Ancestor, MetaObject);

      indx := MetaObject.Collections.IndexOf(ObjectNameByClassType[conversionObject.ClassType, 1]);
      MetaObject := TSchemaObject(MetaObject.Collections.Objects[indx])
    end else begin
      // А сюды мы попадём, если добрались до прорадителя всех DSO объектов, т.е. до DSO.IServer.
      // Далее выходим из рекурсии и попутно находим соответствующий мета-объект
      MetaObject := _xmlSchema.Root
    end
  end
end;

function TDsoXmlConverter.GetDSOObjectPath(const dsoObject: ICommon;
  var Path: TDSOObjectPath): Boolean;
var
  Ancestor: ICommon;
  Indx: Integer;
begin
  Result := True;

  if dsoObject.ClassType <> clsServer then begin
    // Получаем предка для текущего объекта
    Ancestor := dsoObject.Get_Parent;

    // Входим в рекурсию
    GetDSOObjectPath(Ancestor, Path);

    // Заполняем элемент пути для текущего объекта
    Indx := Path[1].PathLen + 1;
    Path[1].PathLen := Indx;
    Path[Indx].PathLen := 1;
    Path[Indx].dsoClass := dsoObject.ClassType;
    Path[Indx].dsoName := dsoObject.Name;
  end else begin
    // Заполняем элемент пути для корневого объекта
    Path[1].PathLen := 1;
    Path[1].dsoClass := dsoObject.ClassType;
    Path[1].dsoName := dsoObject.Name;
  end
end;

procedure TDsoXmlConverter.SaveDSOAncestors(const conversionObject: ICommon; var xmlNode: IXMLDOMElement; var MetaObject: TSchemaObject);
var
  Ancestor: ICommon;
  xmlChildNode: IXMLDOMElement;
  ObjectName: string;
  indx: integer;
begin
  // Получаем предка для текущего объекта
  Ancestor := conversionObject.Get_Parent;

  // если имеем предка, то обработаем его
  if Ancestor <> nil then begin
    if Ancestor.ClassType <> clsServer then begin
      // Входим в рекурсию
      SaveDSOAncestors(Ancestor, xmlNode, MetaObject);

      // Сохраняем описание объекта в XML
      ObjectName := GetObjectNameByClassType(Ancestor.ClassType);
      xmlChildNode := xmlNode.ownerDocument.createElement(ObjectName + 's');
      xmlNode.appendChild(xmlChildNode);
      xmlNode := xmlChildNode;
      xmlChildNode := xmlNode.ownerDocument.createElement(ObjectName);
      SaveDSOObjectProperties(Ancestor, xmlChildNode, MetaObject, 'none', False, '');
      SaveCustomProperties(Ancestor, xmlChildNode, MetaObject, 'none', MetaObject.ObjectClass = clsDatabase);
      xmlNode.appendChild(xmlChildNode);
      xmlNode := xmlChildNode;
      indx := MetaObject.Collections.IndexOf(ObjectNameByClassType[conversionObject.ClassType, 1]);
      MetaObject := TSchemaObject(MetaObject.Collections.Objects[indx]);
    end else begin
      // А сюды мы попадём, если добрались до прорадителя всех DSO объектов, т.е. до DSO.IServer.
      // Далее выходим из рекурсии и попутно сохраняем обработанные объекты в XML
      MetaObject := _xmlSchema.Root
    end
  end
end;

procedure TDsoXmlConverter.SaveDSOObjects(
  const conversionObject: ICommon; var xmlNode: IXMLDOMElement;
  var MetaObject: TSchemaObject;  const Method: string; PadStr: string;
  const SaveConnString: Boolean);
var
  xmlChildNode: IXMLDOMElement;
  xmlChildNode2: IXMLDOMElement;
  i, j: integer;
  CollObject: TSchemaObject;
  olapColl: OlapCollection;
  tmp: ICommon;
  MyMethod, MethodForChildrens: string;
  PermitedAccess: integer;
  IsSaveAll: boolean;
begin
  // Для объекта CubeLevel можно использовать только метод Update
  if MetaObject.Name = 'CubeLevel' then
    MyMethod := 'Update'
  else
    MyMethod := Method;

  // Сохраняем свойства объекта
  // Небольшая заплатка: для DataSource сохранять все свойства можно только для базы данных
  IsSaveAll := True;
  if conversionObject.ClassType = clsDatasource then begin
    IsSaveAll := SaveConnString
  end;
  SaveDSOObjectProperties(conversionObject, xmlNode, MetaObject, MyMethod, not (Method = 'Remove') and IsSaveAll, PadStr);

  // Сохраняем дополнительные свойства объекта
  SaveCustomProperties(conversionObject, xmlNode, MetaObject, MyMethod, not (Method = 'Remove'));

  // Определяем метод сохранения для дочерних объектов
  //   Если сохраняем объект с методом Remove или Update,
  //   то дочерние объекты не сохраняем
  if (MyMethod = 'Remove') or (MyMethod = 'Update') then
    exit;

  if MyMethod = 'Replace' then
    // Если объект заменяется, то дочерние необходимо добавлять
    MethodForChildrens := 'Add'
  else
    MethodForChildrens := Method;

  // 1. Перебираем все доступные коллекции текущего объекта conversionObject
  // 1.1. И для каждой коллекции объекта conversionObject перебираем все
  //      элементы коллекции и рекурсивно сохраняем их в XML
  for i := 0 to MetaObject.Collections.Count - 1 do begin
    CollObject := TSchemaObject(MetaObject.Collections.Objects[i]);
    try
      PermitedAccess := CollObject.CheckAccess(conversionObject, accessRead);
      if PermitedAccess < 2 then
        continue;
      olapColl := GetCollectionByName(conversionObject, MetaObject.Collections[i], MetaObject.InterfaceGUID);
    except
      LogError('Коллекция "' + MetaObject.Collections[i] + '" для объекта ' + MetaObject.Name + ':"' + conversionObject.Name + '" не доступна');
    	continue
    end;
    xmlChildNode := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(olapColl.ContainedClassType) + 's');
    xmlNode.appendChild(xmlChildNode);
    for j := 1 to olapColl.Count do begin
      xmlChildNode2 := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(olapColl.ContainedClassType));
      xmlChildNode.appendChild(xmlChildNode2);
      IUnknown(olapColl.Item(j)).QueryInterface(ICommon , tmp);
      SaveDSOObjects(tmp, xmlChildNode2, CollObject, MethodForChildrens, PadStr + '  ', (MetaObject.ObjectClass = clsDatabase));
      tmp := nil;
    end;
  end;
end;

// Сохраняет DSO объект conversionObject в MXL документ
function TDsoXmlConverter.ConvertDSO2XML(
  const conversionObject: ICommon; var xmlDocoment: IXMLDomDocument2;
  const ConversionOptions: TConversionOptions;
  const ScriptingOptions: TScriptingOptions): HResult;
var
  MetaObject: TSchemaObject;
  xmlNode: IXMLDOMElement;
  xmlChildNode: IXMLDOMElement;
  Method: string;
  olapColl: olapCollection;
  DSOObject: ICommon;
  i: integer;
begin
  // Добавляем шапку в XML документ и получаем указатель на элемент документа
  xmlDocoment.appendChild(xmlDocoment.CreateProcessingInstruction('xml', 'version="1.0" encoding="windows-1251"'));
  xmlNode := xmlDocoment.createElement('XMLDSOConverter');
  xmlNode.setAttribute('version', DsoXmlConverterVersion);
  xmlDocoment.documentElement := xmlNode;

  // Сохраняем серверные мокро-определения (CustomProperties)
  SaveCustomProperties(conversionObject.Server as ICommon, xmlNode, MetaObject, '', True);

  // Определяем метод восстановления для объекта
  case ConversionOptions of
  ConvertFromDSOAdd: Method := 'Add';
  ConvertFromDSOUpdate: Method := 'Update';
  ConvertFromDSOReplace: Method := 'Replace';
  ConvertFromDSORemove: Method := 'Remove';
  end;

  // Рекурсивно поднимаемся вверх по иерархии объектов DSO от текущего
  // объекта conversionObject до объекта Server, а при спуске
  // (возврате из рекурсии) сохраняем каждый объект в XML документе
  MetaObject := nil;
  SaveDSOAncestors(conversionObject, xmlNode, MetaObject);

  // Сохраняем DSO объект в XML документ
  //	Добавляем обрамляющий тег
  xmlChildNode := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(conversionObject.ClassType) + 's');
  xmlNode.appendChild(xmlChildNode);
  xmlNode := xmlChildNode;

  // Сохраняем текущий объект и всех его потомков
  if ScriptingOptions = SelectedObjectAndChildrens then begin
    // Добавляем тег объекта
    xmlChildNode := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(conversionObject.ClassType));
    xmlNode.appendChild(xmlChildNode);
    xmlNode := xmlChildNode;
    try
      SaveDSOObjects(conversionObject, xmlNode, MetaObject, Method, '', (MetaObject.ObjectClass = clsDatasource));
    except
      on E: Exception do
        raise Exception.Create('Ошибка при сохранении объекта '
          + conversionObject.Name	+ #13#10 + E.Message)
    end;
  end else
    // Сохраняем текущий объект и всех братьев объекта
    if ScriptingOptions = SelectedObjectAndSiblings then begin
      olapColl := GetCollectionByName(conversionObject.Get_Parent,
        ObjectNameByClassType[conversionObject.ClassType, 1],
        MetaObject.Parent.InterfaceGUID);
      for i := 1 to olapColl.Count do begin
        // Добавляем тег объекта
        xmlChildNode := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(conversionObject.ClassType));
        xmlNode.appendChild(xmlChildNode);
        try
          IUnknown(olapColl.Item(i)).QueryInterface(ICommon, DSOObject);
          SaveDSOObjects(DSOObject, xmlChildNode, MetaObject, Method, '', (MetaObject.ObjectClass = clsDatasource));
        except
          on E: Exception do
            raise Exception.Create('Ошибка при сохранении объекта '
              + conversionObject.Name	+ #13#10 + E.Message)
        end;
      end;
    end else
      // Сохраняем только текущий объект
      if ScriptingOptions = SelectedObjectOnly then begin
        // Добавляем тег объекта
        xmlChildNode := xmlNode.ownerDocument.createElement(GetObjectNameByClassType(conversionObject.ClassType));
        xmlNode.appendChild(xmlChildNode);
        xmlNode := xmlChildNode;
        // Сохраняем свойства объекта
        try
          SaveDSOObjectProperties(conversionObject, xmlNode, MetaObject, Method, not (Method = 'Remove'), '');
        except
          on E: Exception do
            raise Exception.Create('Ошибка при сохранении объекта '
              + conversionObject.Name + #13#10 + E.Message)
        end;
      end;
  Result := 0;
end;

// Функция находит DSO объект по указанному пути на OLAP сервере srv
// Синтаксис строки пути:
//  <PATH> := <PATH> '/' <PATH_NODE> | <PATH_NODE>
//  <PATH_NODE> := <NODE_CLASS> ':' <NODE_NAME>
//  <NODE_CLASS> := 'db' | 'cub' | 'prt' | 'dim' | 'ds' | 'cmd' | 'rol' | 'msr' | 'lev' | 'mbr'
//  <NODE_NAME> := "Имя объекта"
function TDsoXmlConverter.FindDSOObject(const srv: Server; const Path: string): ICommon;
const
  MAXPATHNODES = 10; // Максимальное кол-во элементов в пути
type
  PPathNodes = ^TPathNodes;
  TPathNode = record		// Отдельный элемент пути
    ClassType: string;	//	класс элемента
    Name: string;				//	имя элемента
  end;
  TPathNodes = array[1..MAXPATHNODES] of TPathNode;

  // Разбор пути к DSO объекту на отдельные элементы
  function ParseDSOPath(StringPath: string; var Path: TPathNodes): integer;
  type
    TState = (sClassLexem, sNameLexem, sError);
  var
    i, Node: integer; state: TState;
  begin
    state := sClassLexem;
    Node := 1;
    for i := 1 to Length(StringPath) do begin
      case StringPath[i] of
      ':':
        if state = sClassLexem then begin
          state := sNameLexem;
          if Path[Node].ClassType = 'cub' then Path[Node].ClassType := 'MDStores';
          if Path[Node].ClassType = 'prt' then Path[Node].ClassType := 'MDStores';
          if Path[Node].ClassType = 'dim' then Path[Node].ClassType := 'Dimensions';
          if Path[Node].ClassType = 'ds'  then Path[Node].ClassType := 'DataSources';
          if Path[Node].ClassType = 'cmd' then Path[Node].ClassType := 'Commands';
          if Path[Node].ClassType = 'rol' then Path[Node].ClassType := 'Roles';
          if Path[Node].ClassType = 'msr' then Path[Node].ClassType := 'Measures';
          if Path[Node].ClassType = 'lev' then Path[Node].ClassType := 'Levels';
          if Path[Node].ClassType = 'mbr' then Path[Node].ClassType := 'MemberProperties';
        end else state := sError;
      '/':
        if state = sNameLexem then begin
          state := sClassLexem;
          Node := Node + 1;
        end else state := sError;
      else
      	if state = sNameLexem then
          Path[Node].Name := Path[Node].Name + StringPath[i]
        else if state = sClassLexem then
          Path[Node].ClassType := Path[Node].ClassType + StringPath[i]
        else state := sError;
      end;
      if state = sError then begin Result := 0; exit; end;
    end;
    if state <> sNameLexem then
      Result := 0
    else
      Result := Node;
  end;

var
  Count, i, j: integer;
  PathNodes: TPathNodes;
  conversionObject: ICommon;
  olapColl: olapCollection;
  tmp: ICommon;
  MetaObject: TSchemaObject;
  VarName: Variant;
  CollIndx: integer;
  Finded: boolean;
begin
  Result := nil;
  MetaObject := _xmlSchema.Root;
  Count := ParseDSOPath(Path, PathNodes);
  if Count = 0 then Exit;
  conversionObject := srv as ICommon;

  // Находим базу
  Finded := False;
  olapColl := GetCollectionByName(conversionObject, 'MDStores', IID__Server);
  for j := 1 to olapColl.Count do begin
    IUnknown(olapColl.Item(j)).QueryInterface(ICommon , tmp);
    VarName := GetDispatchProperty(tmp, 'Name');
    if VarName = PathNodes[1].Name then begin
      conversionObject := tmp;
      Finded := true;
      break
    end
  end;

  if not Finded then
    raise Exception.Create('Object not found: ' + PathNodes[1].Name);

  for i := 2 to Count do begin
    CollIndx := MetaObject.Collections.IndexOf(PathNodes[i].ClassType);
    olapColl := GetCollectionByName(conversionObject, MetaObject.Collections[CollIndx], MetaObject.InterfaceGUID);
    for j := 1 to olapColl.Count do begin
      IUnknown(olapColl.Item(j)).QueryInterface(ICommon , tmp);
      VarName := GetDispatchProperty(tmp, 'Name');
      if VarName = PathNodes[i].Name then begin
      	conversionObject := tmp;
        MetaObject := TSchemaObject(MetaObject.Collections.Objects[CollIndx]);
        break
      end;
    end;
  end;
  Result := conversionObject;
end;

// Сохраняет DSO объект conversionObject в файл filePathXml
function TDsoXmlConverter.ConvertDSO2XMLFile(serverNameOlap: string;
  fs: TFileStream; DSOPath: string;
  ConversionOptions: TConversionOptions;
  const ScriptingOptions: TScriptingOptions): HResult;
var
  xmlDocoment: IXMLDomDocument2;
  conversionObject: ICommon;
begin
  // Подключаемся к серверу
  ProcessEvent('Подключение к серверу ' + serverNameOlap + '...', nil, nil);

  if _server.State <> StateConnected then _server.Connect(serverNameOlap);
  if _server.State <> StateConnected then begin
    LogError('Невозможно подключиться к серверу ' + serverNameOlap); result := -1;
    exit
  end;

  // По пути к объекту находим DSO объект
  conversionObject := FindDSOObject(_server, DSOPath);

  // Создаём новый XML документ и получаем указатель на элемент документа
  xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;

  ConvertDSO2XML(conversionObject, xmlDocoment, ConversionOptions, ScriptingOptions);

  // Сохраняем XML документ
  ProcessEvent('Сохранение документа на диск...', nil, nil);
  SaveFormattedXMLDocumentToStream(xmlDocoment, fs);

  xmlDocoment := nil;

  // Отключаемся от сервера
  if _server.State = StateConnected  then
    _server.CloseServer;
  result := 0;
end;

procedure TDsoXmlConverter.RestoreObjectProperties(
  const conversionObject: ICommon; const xmlNode: IXMLDOMNode;
  const MetaObject: TSchemaObject; const IsSecondPass: Boolean);
var
  i, j: integer;
  Prop: TProperty;
  xmlAttrNode: IXMLDOMNode;
  xmlPropNode: IXMLDOMNode;
  PropValue: OleVariant;
  tmp: IDispatch;
  PermitedAccess: integer;
  dsoDataSource: DataSource;
  dsoDimension: Dimension;
  s, PropStrValue: string;
  Indx: Integer;
begin
  if conversionObject.ClassType = clsDatabase then begin
    _dsoDB := conversionObject as DataBase;
  end;

  for i := 0 to MetaObject.Properties.Count - 1 do begin
    xmlAttrNode := nil;
    xmlPropNode := nil;
    Prop := TProperty(MetaObject.Properties.Objects[i]);

    if not IsSecondPass then
      if Prop.IsSecondPass then
        continue;

    PermitedAccess := Prop.CheckAccess(conversionObject, accessWrite);
    if PermitedAccess < 2 then
      continue;
    if Prop.DataType > 10 then begin
      for j := 0 to xmlNode.childNodes.length - 1 do begin
      	xmlAttrNode := xmlNode.childNodes.item[j].attributes.getNamedItem('name');
        if xmlAttrNode <> nil then
          if xmlAttrNode.text = Prop.Name then begin
            xmlPropNode := xmlNode.childNodes.item[j];
            break
          end;
      end;
    end else
      xmlPropNode := xmlNode.attributes.getNamedItem(Prop.Name);
    if Assigned(xmlPropNode) then begin
      PropValue := xmlPropNode.text;

      case Prop.DataType of
      dtInteger: PropValue := VarAsType(PropValue, varInteger);
      dtBoolean:
        if PropValue = 'true' then PropValue := True
        else PropValue := False;
      dtDate: PropValue := VarAsType(PropValue, varDate);
      dtDouble:
        try
          Indx := Pos('.', PropValue);
          if Indx = 0 then
            Indx := Pos(',', PropValue);
          if Indx <> 0 then begin
            s := PropValue;
            s[Indx] := DecimalSeparator;
            PropValue := s
          end;
        except
          on E: Exception do begin
            PropValue := PropValue;
          end
        end;
      dtObject:
        if Prop.Name = 'DataSource' then begin
          dsoDimension := conversionObject as Dimension;
          dsoDataSource := IUnknown(_dsoDB.DataSources.item(PropValue)) as DataSource;
          dsoDimension.DataSource := dsoDataSource;
          continue;
        end;
      end; //case

      try
        if (Prop.DataType = dtString) or (Prop.DataType = dtCDATA) then begin
          PropStrValue := ApplyReplaces(VarToStr(PropValue));
          SetDSOObjectPropMetaValue(conversionObject, Prop.Name, MetaObject, fMacrosList, PropStrValue)
        end else begin
          IUnknown(conversionObject).QueryInterface(MetaObject.InterfaceGUID , tmp);
          PutDispatchProperty(tmp, Prop.Name, PropValue);
        end
      except
        on E: Exception do begin
          if not Prop.IsSilent then
            raise Exception.CreateFmt('Ошибка утановки значения свойства "%s" в %s: %s', [Prop.Name, PropValue, E.Message]);
          continue
        end
      end;
    end
  end
end;

procedure TDsoXmlConverter.RestoreObjectCustomProperties(
  const conversionObject: ICommon; const xmlNode: IXMLDOMNode;
  const MetaObject: TSchemaObject);
var
  i: Integer;
  xmlPropertiesNodeList: IXMLDOMNodeList;
  Name, DataType: string;
  Value: OleVariant;
  Prop: _Property;
begin
  xmlPropertiesNodeList := xmlNode.selectNodes('CustomProperties/Property');
  for i := 0 to xmlPropertiesNodeList.length - 1 do begin
    Name := xmlPropertiesNodeList.item[i].attributes.getNamedItem('name').text;
    DataType := xmlPropertiesNodeList.item[i].attributes.getNamedItem('datatype').text;
    Value := xmlPropertiesNodeList.item[i].text;
    // Если свойство не найдено
    if conversionObject.CustomProperties.Item[Name]<> nil then begin
        ProcessEvent('Обновление CustomProperty (' + Name + ', ' + DataType + ', ' + Value + ')', nil, nil);
        conversionObject.CustomProperties.Item[Name].Value := Value;
    end else begin
      // Добавляем свойство
      try
        Prop := conversionObject.CustomProperties.Item[Name];
        if Prop = nil then
          conversionObject.CustomProperties.Add(Value, Name, StrToInt(DataType));
      except
        on E: Exception do begin
          LogError('Error on add CustomProperty: ' + Name + ' for ' + conversionObject.Name + ' SubClassType=' + IntToStr(conversionObject.SubClassType));
        end;
       end;
      end;
   end;
end;

procedure TDsoXmlConverter.RestoreDSOObject(
  const conversionObject: ICommon; const xmlNode: IXMLDOMNode;
  const MetaObject: TSchemaObject; const ParentInterface: TGUID);
var
  i, j, ObjIndx: integer;
  Method, MacroName, ObjName, CollName, OrignString: string;
  olapColl: olapCollection;
  convObject: ICommon;
  SubClassType: SubClassTypes;
  CollObject: TSchemaObject;
  xmlGroupNode: IXMLDOMNode;
  tmp: IDispatch;
  PermitedAccess: integer;
  IsPresentMacros: Boolean;
begin
  if conversionObject.ClassType = clsDatabase then begin
    _dsoDB := conversionObject as DataBase;
  end;
  MacroName := xmlNode.attributes.getNamedItem('name').text;

  // Делаем подстановку макро-определений
  IsPresentMacros := (ApplyMacros(MacroName, OrignString, fMacrosList) > 0);
  ObjName := OrignString;

  // Делаем замены
  ObjName := ApplyReplaces(ObjName);

  SubClassType := StrToInt(xmlNode.attributes.getNamedItem('SubClassType').text);
  Method := xmlNode.attributes.getNamedItem('method').text;

  CollName := GetCollectionByObjectName(xmlNode.nodeName);
  olapColl := GetCollectionByName(conversionObject, CollName, ParentInterface);
  ObjIndx := 0;

  ProcessEvent(Method + ': ' + xmlNode.nodeName + ' ' + ObjName, conversionObject, MetaObject);

  // Находим элемент в коллекции по имени
  for i := 1 to olapColl.Count do
    if olapColl.Item(i).Name = ObjName then begin
      IUnknown(olapColl.Item(i)).QueryInterface(ICommon , convObject);
      ObjIndx := i;
      break
    end;

  if (Method <> 'Add') and (convObject = nil) then begin
    raise Exception.CreateFmt('Объект не найден: %s.[%s]', [xmlNode.nodeName, ObjName]);
  end;
  if Method = 'Add' then begin
    if convObject <> nil then begin
      if (convObject.ClassType <> clsPartition)
         and (convObject.ClassType <> clsDatasource) then begin
        raise Exception.CreateFmt('Добавляемый объект уже существует: %s.[%s] SubClassType=%d', [xmlNode.nodeName, ObjName, SubClassType]);
      end
    end else
      try
        convObject := olapColl.AddNew(ObjName, SubClassType) as ICommon;
      except
        on E: Exception do
          raise Exception.CreateFmt('Ошибка при добавлении объекта: %s.[%s] SubClassType=%d', [xmlNode.nodeName, ObjName, SubClassType]);
      end;
  end else if Method = 'Replace' then begin
    olapColl.Remove(ObjIndx);
    convObject := olapColl.AddNew(ObjName, SubClassType) as ICommon;
  end else if Method = 'Remove' then begin
    olapColl.Remove(ObjIndx);
    exit
  end;

  if convObject.ClassType = clsDatabase then
    LoadDatabaseMacros(convObject, xmlNode);
  // Восстанавливаем свойства объекта
  if Method <> 'none' then begin
    SynchronizeCustomProp('name', MacroName, convObject, IsPresentMacros);
    RestoreObjectCustomProperties(convObject, xmlNode, MetaObject);
    RestoreObjectProperties(convObject, xmlNode, MetaObject, False);
  end;

  if (Method <> 'none') and (MetaObject.Methods.Find('Update', i)) then begin
    if (MetaObject.Parent <> nil) and (MetaObject.Parent.Name = 'Cube') and (convObject.ClassType = clsDatasource) then
    else
    begin
      IUnknown(convObject).QueryInterface(MetaObject.InterfaceGUID , tmp);
      try
        ProcessEvent('Update object ' + ' ' + convObject.Name, convObject, MetaObject);
        InvokeMetod(tmp, 'Update', OleVarArrayOf([]), OleVarArrayOf([]));
      except
        on E: Exception do begin
          raise Exception.CreateFmt('Ошибка при вызове метода Update: %s.[%s] ClassType=%d SubClassType=%d: %s',
            [MetaObject.Name, convObject.Name, convObject.ClassType, convObject.SubClassType, E.Message]);
        end
      end
    end;
  end;

  // Обрабатываем дочерние объекты
  for i := 0 to MetaObject.Collections.Count - 1 do begin
    CollObject := TSchemaObject(MetaObject.Collections.Objects[i]);
    PermitedAccess := CollObject.CheckAccess(convObject, accessWrite);
    if PermitedAccess < 2 then
      continue;
    xmlGroupNode := xmlNode.selectSingleNode(CollObject.Name + 's');
    if xmlGroupNode = nil then continue;
    for j := 0 to xmlGroupNode.childNodes.length - 1 do begin
      RestoreDSOObject(convObject, xmlGroupNode.childNodes.item[j], CollObject, MetaObject.InterfaceGUID);
    end;
  end;

  // Повторно восстанавливаем свойства объекта
  if Method <> 'none' then begin
    try
      SynchronizeCustomProp('name', MacroName, convObject, IsPresentMacros);
      RestoreObjectProperties(convObject, xmlNode, MetaObject, True);
      RestoreObjectCustomProperties(convObject, xmlNode, MetaObject);
    except
      on E: Exception do begin
        ProcessEvent('Error: restore properties on pas 2 for object ' + conversionObject.Name + ' (' + E.Message + ')', conversionObject, MetaObject);
      end
    end
  end;

  if MetaObject.Methods.Find('Update', i) then begin
    if (MetaObject.Parent <> nil) and (MetaObject.Parent.Name = 'Cube') and (convObject.ClassType = clsDatasource) then
    else begin
      try
        ProcessEvent('Update2 object ' + ' ' + convObject.Name, convObject, MetaObject);
        IUnknown(convObject).QueryInterface(MetaObject.InterfaceGUID , tmp);
        InvokeMetod(tmp, 'Update', OleVarArrayOf([]), OleVarArrayOf([]));
      except
    	on E: Exception do begin
          raise Exception.CreateFmt('Ошибка при вызове метода Update: %s.[%s] ClassType=%d SubClassType=%d: %s',
            [MetaObject.Name, convObject.Name, convObject.ClassType, convObject.SubClassType, E.Message]);
        end
      end;
    end
  end;
end;

// Восстанавливает DSO-объекты из XML описания
function TDsoXmlConverter.ConvertXML2DSO(const srv: Server;
  const xmlNode: IXMLDOMElement): HResult;
var
  i: integer;
  MetaObject: TSchemaObject;
  xmlNodeList: IXMLDOMNodeList;
  tmp: IDispatch;
  Prop: _Property;
  PropName: string;
begin
  // Считываем макро-определеня из XML файла
{  fMacrosList.Clear;
  xmlNodeList := xmlNode.selectNodes('CustomProperties/Property');
  for i := 0 to xmlNodeList.length - 1 do begin
    PropName := xmlNodeList.item[i].attributes.getNamedItem('name').text;
    if PropName[1] = '#' then begin
      PropName := Copy(PropName, 2, Length(PropName) - 1);
      fMacrosList.Values[PropName] := xmlNodeList.item[i].text;
    end
  end;}
  // Считываем макро-определеня для сервера из CustomProperties
  // уже существующие не перезаписываются
  for i :=  1 to srv.CustomProperties.Count do begin
    Prop := srv.CustomProperties.Item[i];
    if Prop.Name[1] = '#' then begin
      PropName := Copy(Prop.Name, 2, Length(Prop.Name) - 1);
      if fMacrosList.IndexOfName(PropName) = -1 then
        fMacrosList.Values[PropName] := Prop.Value;
    end
  end;

  MetaObject := _xmlSchema.Root;
  xmlNodeList := xmlNode.selectSingleNode('Databases').childNodes;
  for i := 0 to xmlNodeList.length - 1 do begin
    RestoreDSOObject(srv as ICommon, xmlNodeList.item[i], MetaObject, IID__Server);
  end;
  IUnknown(srv).QueryInterface(IID__Server, tmp);
  InvokeMetod(tmp, 'Update', OleVarArrayOf([]), OleVarArrayOf([]));
  Result := 0;
end;

function TDsoXmlConverter.ConvertXMLFile2DSO(filePathXml,
  serverNameOlap: string): HResult;
var
  xmlDocoment: IXMLDomDocument2;
  xmlElement: IXMLDOMElement;
begin
  ProcessEvent('Загружаем XML документ ' + filePathXml + '...', nil, nil);
  // Загружаем XML документ
  xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
  xmlDocoment.async := false;
  if not xmlDocoment.Load(filePathXml) then begin
    LogError('Не удалось открыть файл ' + filePathXml + ': ' + xmlDocoment.parseError.reason);
    if _server.State = StateConnected then _server.CloseServer;
    result := 0;
    exit;
  end;

  // Подключаемся к серверу
  ProcessEvent('Подключение к серверу ' + serverNameOlap + '...', nil, nil);
  if _server.State <> StateConnected then _server.Connect(serverNameOlap);
  if _server.State <> StateConnected then begin
    ErrorEvent('Невозможно подключиться к серверу ' + serverNameOlap, nil, nil);
    result := -1;
    exit
  end;

  // Получаем элемент документа
  xmlElement := xmlDocoment.documentElement;

  // Восстанавливаем
  ConvertXML2DSO(_server, xmlElement);

  // Отключаемся от сервера
  if _server.State = StateConnected  then begin
    _server.CloseServer;
  end;
  ProcessEvent('Восстановление завершено.', nil, nil);
  result := 0;
end;

procedure TDsoXmlConverter.LogError(const ErrMsg: string);
begin
  if Assigned(_ErrLog) then _ErrLog.Insert(0, ErrMsg);
  if Assigned(fOnErrorEvent) then fOnErrorEvent(ErrMsg, nil, nil);
end;

procedure TDsoXmlConverter.MetaLogError(const ErrMsg: string);
begin
  if Assigned(_ErrLog) then _ErrLog.Insert(0, ErrMsg);
  if Assigned(fOnErrorEvent) then fOnErrorEvent(ErrMsg, nil, nil);
end;

procedure TDsoXmlConverter.ProcessXMLPackage(const filePathXml,
  serverNameOlap: string);
var
  i: Integer;
  xmlDocoment: IXMLDomDocument2;
  xmlNode: IXMLDOMNode;
  PacketFileName: string;
  IsError: Boolean;
begin
  IsError := False;
  try
    ProcessEvent('Загружаем XML пакет ' + filePathXml + '...', nil, nil);

    // Загружаем XML пакет-сет
    xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
    xmlDocoment.async := false;
    if not xmlDocoment.Load(filePathXml) then begin
      LogError('Не удалось открыть файл ' + filePathXml + ': ' + xmlDocoment.parseError.reason);
      if _server.State = StateConnected then _server.CloseServer;
      exit;
    end;

    // Получаем список макросов
    ProcessEvent('Загружаем список макросов...', nil, nil);
    try
      xmlNode := xmlDocoment.selectSingleNode('//package/macros');
      PacketFileName := xmlNode.attributes.getNamedItem('file').text;
    except
      on E: Exception do begin
        IsError := True;
        ErrorEvent('Невозможно определить имя файла определяющий список макросов.', nil, nil);
        exit
      end
    end;
    try
      LoadMacrosList(PacketFileName);
    except
      on E: Exception do begin
        IsError := True;
        ErrorEvent(Format('Ошибка загрузки файла содержащего списоко макросов: %s', [E.Message]), nil, nil);
        exit
      end
    end;

    // Получаем список замен
    ProcessEvent('Загружаем список замен...', nil, nil);
    try
      xmlNode := xmlDocoment.selectSingleNode('//package/replaces');
      PacketFileName := xmlNode.attributes.getNamedItem('file').text;
    except
      on E: Exception do begin
        IsError := True;
        ErrorEvent('Невозможно определить имя файла определяющий список замен.', nil, nil);
        exit
      end
    end;
    try
      LoadReplaceList(PacketFileName);
    except
      on E: Exception do begin
        IsError := True;
        ErrorEvent(Format('Ошибка загрузки файла содержащего список замен: %s', [E.Message]), nil, nil);
        exit
      end
    end;

    // Получаем список пакетов
    // Восстанавливаем каждый пакет
    ProcessEvent('Обрабатываем список пакетов...', nil, nil);
    try
      for i := 0 to xmlDocoment.selectNodes('//packets/packet').length - 1 do begin
        xmlNode := xmlDocoment.selectSingleNode('//packets/packet[@order = ''' + IntToStr(i + 1) + ''']');
        if not Assigned(xmlNode) then
          raise Exception.Create('Не указан пакет.');
        try
          PacketFileName := xmlNode.attributes.getNamedItem('file').text;
        except
          raise Exception.Create('Не указано имя файла пакета.');
        end;
        ProcessEvent('Восстанавление XML пакета ' + PacketFileName + '...', nil, nil);

        // Восстанавливаем пакет
        ConvertXMLFile2DSO(PacketFileName, serverNameOlap);
        ProcessEvent('Восстанавление XML пакета завершено.', nil, nil);
      end;
    except
      on E: Exception do begin
        IsError := True;
        ErrorEvent(Format('Ошибка восстановления пакета: %s', [E.Message]), nil, nil);
        exit
      end
    end;
  finally
    if IsError then
      ProcessEvent('Восстановление прервано из-за ошибки.', nil, nil)
    else
      ProcessEvent('Восстановление завершено.', nil, nil)
  end
end;

procedure TDsoXmlConverter.LoadReplaceList(const filePathXml: string);
var
  i: Integer;
  xmlDocoment: IXMLDomDocument2;
  xmlNodeList: IXMLDOMNodeList;
  Name, Value: string;
begin
  // Загружаем XML документ
  xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
  xmlDocoment.async := false;
  if not xmlDocoment.Load(filePathXml) then
    raise Exception.CreateFmt('Не удалось открыть файл: %s : %s', [filePathXml, xmlDocoment.parseError.reason]);

  try
    // Получаем список замен
    try
      xmlNodeList := xmlDocoment.selectNodes('//replacelist/replace');
      fReplaceList.Clear;
      for i := 0 to xmlNodeList.length - 1 do begin
        Name := xmlNodeList.item[i].attributes.getNamedItem('find').text;
        if xmlNodeList.item[i].attributes.getNamedItem('case').text = '1' then
          Name := '1' + Name
        else if xmlNodeList.item[i].attributes.getNamedItem('case').text = '0' then
          Name := '0' + Name
        else
          raise Exception.Create('неверно указано знасение атрибута case.');

        if fReplaceList.IndexOfName(Name) = -1 then begin
          Value := xmlNodeList.item[i].attributes.getNamedItem('replace').text;
          if Value = '' then begin
            fReplaceList.Values[Name] := ' ';
          end else
            fReplaceList.Values[Name] := Value;
        end;
      end;
    except
      on E: Exception do begin
        raise Exception.CreateFmt('Ошибка загрузки списка замен: %s', [E.Message]);
      end
    end;
  finally
  end;
end;

procedure TDsoXmlConverter.LoadMacrosList(const filePathXml: string);
var
  i: Integer;
  xmlDocoment: IXMLDomDocument2;
  xmlNodeList: IXMLDOMNodeList;
  Name: string;
begin
  // Загружаем XML документ
  xmlDocoment := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
  xmlDocoment.async := false;
  if not xmlDocoment.Load(filePathXml) then
    raise Exception.CreateFmt('Не удалось открыть файл: %s : %s', [filePathXml, xmlDocoment.parseError.reason]);

  try
    // Получаем список макросов
    try
      xmlNodeList := xmlDocoment.selectNodes('//macroslist/macros');
      fMacrosList.Clear;
      for i := 0 to xmlNodeList.length - 1 do begin
        Name := xmlNodeList.item[i].attributes.getNamedItem('name').text;
        if fMacrosList.IndexOfName(Name) = -1 then
          fMacrosList.Values[Name] := xmlNodeList.item[i].text;
      end;
    except
      on E: Exception do begin
        raise Exception.Create('Ошибка загрузки списка макросов.');
      end
    end;
  finally
  end;
end;

function StringReplaceWholeWord(const S, OldPattern, NewPattern: string;
  Flags: TReplaceFlags): string;
const
  Delimiters: string = '.,:; *-+|=~`?!^%"$''<>/\(){}[]';
var
  SearchStr, Patt, NewStr: string;
  Offset: Integer;
begin
  if rfIgnoreCase in Flags then
  begin
    SearchStr := AnsiUpperCase(S);
    Patt := AnsiUpperCase(OldPattern);
  end else
  begin
    SearchStr := S;
    Patt := OldPattern;
  end;
  NewStr := S;
  Result := '';
  while SearchStr <> '' do
  begin
    Offset := AnsiPos(Patt, SearchStr);
    if Offset = 0 then
    begin
      Result := Result + NewStr;
      Break;
    end;
    // Делаем замену, если найденная подстрока является словом
    if (
         IsDelimiter(Delimiters, SearchStr, Offset - 1)
         and IsDelimiter(Delimiters, SearchStr, Offset + Length(Patt)))
    or (
         (Offset = 1)
         and IsDelimiter(Delimiters, SearchStr, Offset + Length(Patt))
         and (Result = '')
       )
    or (
         IsDelimiter(Delimiters, SearchStr, Offset - 1)
         and (Offset + Length(Patt) = Length(SearchStr) + 1)
       )
    or (
         (Offset = 1)
         and (Offset + Length(Patt) = Length(SearchStr) + 1)
         and (Result = '')
       )
    then begin
      Result := Result + Copy(NewStr, 1, Offset - 1) + NewPattern
    end else begin
      Result := Result + Copy(NewStr, 1, Offset - 1) + OldPattern;
    end;

    NewStr := Copy(NewStr, Offset + Length(OldPattern), MaxInt);
    if not (rfReplaceAll in Flags) then
    begin
      Result := Result + NewStr;
      Break;
    end;
    SearchStr := Copy(SearchStr, Offset + Length(Patt), MaxInt);
  end;
end;

function TDsoXmlConverter.ApplyReplaces(const InString: string): string;
var
  i: Integer;
  ReplFlag: TReplaceFlags;
  FindStr: string;
  ReplStr, ReplStrVal: string;
begin
  try
    Result := InString;
    if Result = '' then exit;
    if Assigned(fReplaceList) then begin
      for i := 0 to fReplaceList.Count - 1 do begin
        FindStr := fReplaceList.Names[i];
        ReplStr := FindStr;
        if FindStr[1] = '0' then
          ReplFlag := [rfReplaceAll, rfIgnoreCase]
        else
          ReplFlag := [rfReplaceAll];
        FindStr := Copy(FindStr, 2, length(FindStr) - 1);
        ReplStrVal := fReplaceList.Values[ReplStr];
        Result := StringReplaceWholeWord(Result, FindStr, ReplStrVal, ReplFlag)
      end
    end
  except
    on E: Exception do raise Exception.CreateFmt(
        'Ошибка выполнения замены [%s] на [%s]: %s',
        [FindStr, ReplStrVal, E.Message])
  end
end;

end.
