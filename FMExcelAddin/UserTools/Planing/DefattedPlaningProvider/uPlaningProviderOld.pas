unit uPlaningProviderOld;

interface

uses
  PlaningProvider_TLB, uPlaningCache, uPlaningProviderAbstract, MSXML2_TLB,
  classes, ActiveX, uXmlUtils, uFMAddinGeneralUtils, ComObj, SysUtils,
  uGlobalPlaningConst, uFMAddinXMLUtils, Krista_FM_Update_Framework_TLB;

type
  TPlaningProviderOld = class(TPlaningProviderAbstract)
  private
    FConnected: boolean;
    Params: TStringList;

    {получение метаданных с сервера}
    function GetMetaDataWeb: IXMLDOMDocument2;

    {получение элементов измерения с сервера}
    function GetMemberListWeb(const ProviderId, CubeName, DimensionName, HierarchyName,
      LevelNames, PropertiesNamesList: WideString): IXMLDOMDocument2;

    {запрос к веб-сервису}
    function GetWebResponse(FunctionName: string;
      var Params: TStringList; IsData: boolean): OleVariant;

    function GetResp(FunctionName: string; Params: TStringList; IsData: boolean;
      out ErrorMsg: WideString): IXMLDOMDocument2;

    function GetWebServerName(URL: string): string;

    {формирование текста запроса}
    function GetRequestBody(FunctionName: string; Params: TStringList; IsData: boolean): string;

    procedure SetHeader(var FRequest: IXMLHTTPRequest; WebServerName,
      FunctionName: string; RequestLenght: integer);
    { возвращает сформированный список параметров для вызова GetWebResponce}
    procedure CreateParamsList(ParamsArray: array of string);

  public
    constructor Create;
    destructor Destroy; override;

    //function URL: WideString;
    //function Scheme: WideString;
    //function LastError: WideString;
    //function LastWarning: WideString;
    function Connected: WordBool; override;
    procedure ClearCache;
    procedure FreeProvider; override;

    {подключение к схеме}
    function Connect(const URL, Login, Password: WideString;
      AuthType: SYSINT; var SchemeName: WideString;
      WithinTaskContext: WordBool): WordBool; override;

    {разрыв подключения}
    procedure Disconnect; override;

    {дата-время генерации файла метаданных}
    function GetMetadataDate: WideString; override;

    {получение метаданных схемы}
    function GetMetaData(var XmlDomDocument: IXMLDOMDocument2): wordbool; override;

    {обратная запись}
    function Writeback(const Data: WideString): WideString; override;

    {в этом провайдере - просто заглушка для совместимости}
    function ClientSessionIsAlive: WordBool; override;

    {получение элементов измерения}
    function GetMemberList(const ProviderId, CubeName, DimensionName,
      HierarchyName, LevelNames,
      PropertiesNamesList: WideString): IXMLDOMDocument2; override;

    {mdx-запрос, обычный режим}
    function GetRecordsetData(const ProviderId, QueryText: WideString;
      var DataDom: IXMLDOMDocument2): WordBool; override;

    {mdx-запрос, серверный режим}
    function GetCellsetData(const ProviderId, QueryText: WideString;
      var Data: IXMLDOMDocument2;  out ErrorMsg: WideString): WordBool; override;

    function UpdateMemberList(const ProviderId: WideString;
      const SourceDom: IXMLDOMDocument2; var DestDom: IXMLDOMDocument2;
      const CubeName, DimensionName, HierarchyName, LevelList,
      PropertiesNamesList: WideString): WordBool; override;

    function GetTaskContext(const TaskId: SYSINT): WideString; override;

    function UpdateTaskParams(TaskId: SYSINT; const ParamsText: WideString;
      const SectionDivider: WideString; const ValuesDivider: WideString): WideString; override;

    function UpdateTaskConsts(TaskId: SYSINT; const ConstsText: WideString;
      const SectionDivider: WideString; const ValuesDivider: WideString): WideString; override;

  end;

implementation

{ TPlaningProviderOld }

function TPlaningProviderOld.GetMetaDataWeb: IXMLDOMDocument2;
var
  ResultText: widestring;
  Node: IXmlDOmNode;
begin
  result := InitXmlDocument;
  CreateParamsList([]);
  ResultText := GetWebResponse('GetMetaData2', Params, false);
  result.loadXML(ResultText);
  Node := result.selectSingleNode('function_result');
  if (Node <> nil) then
  begin
    (Node as IXmlDomElement).setAttribute(attrUrl, URL);
    (Node as IXmlDomElement).setAttribute('scheme', Scheme);
  end;
  if not Assigned(result.documentElement) then
  begin
    result.loadXML('<Exception></Exception>');
    AppendCData(result.documentElement, ResultText);
  end;
end;

function TPlaningProviderOld.GetMemberListWeb(const ProviderId, CubeName, DimensionName,
  HierarchyName, LevelNames, PropertiesNamesList: WideString): IXMLDOMDocument2;
var
  ResultText: widestring;
  Node: IXmlDomNode;
begin
  result := InitXmlDocument;
  CreateParamsList([
    'schemeName', FScheme,
    'providerId', ProviderId,
    'cubeName', CubeName,
    'dimensionName', DimensionName,
    'hierarchyName', HierarchyName,
    'levelNames', CommaTextToString(LevelNames),
    'memberPropertiesNames', CommaTextToString(PropertiesNamesList)]);

  ResultText := GetWebResponse('GetMembers2', Params, false);

  result.loadXML(ResultText);
  Node := result.selectSingleNode('function_result');
  if (Node <> nil) then
  begin
    (Node as IXmlDomElement).setAttribute(attrUrl, URL);
    (Node as IXmlDomElement).setAttribute('scheme', Scheme);
    UpdateLongNames(result);
  end;
  if not Assigned(result.documentElement) then
  begin
    result.loadXML('<Exception></Exception>');
    AppendCData(result.documentElement, ResultText);
  end;
end;

function TPlaningProviderOld.GetWebResponse(FunctionName: string;
  var Params: TStringList; IsData: boolean): OleVariant;
var
  FRequest: IXMLHTTPRequest;
  SoapRequest, WebServerName: string;
  ResponseXmlDocument: IXmlDomDocument2;
  ResponseNode, ResultNode: IXmlDomNode;
  tmpStr, ResponseText: string;
  i, IndexOfParam: integer;
begin
  FRequest := CreateOleObject('Microsoft.XMLHTTP') as IXMLHTTPRequest;
  ResponseXmlDocument := InitXmlDocument;
  try
    FRequest.open('POST', FURL, false, EmptyParam, EmptyParam);
    SoapRequest := GetRequestBody(FunctionName, Params, IsData);
    WebServerName := GetWebServerName(FURL);
    SetHeader(FRequest, WebServerName, FunctionName, Length(SoapRequest));
    FRequest.Send(SoapRequest);
    ResponseText := FRequest.responseText;
    ResponseXmlDocument.LoadXml(ResponseText);

    {Если это не нормальный soap-ответ, то делать больше нечего,
      вернем то что есть, без обработки}
    if not ResponseXmlDocument.Parsed then
    begin
      tmpStr := FRequest.responseText;
      result := tmpStr;
      exit;
    end;

    ResponseNode := ResponseXmlDocument.selectSingleNode(
      Format('soap:Envelope/soap:Body/%sResponse', [FunctionName]));
    if Assigned(ResponseNode) then
    begin
      ResultNode := ResponseNode.selectSingleNode(FunctionName + 'Result');
      if not Assigned(ResultNode) then
      begin //абсурдная ситуация, перестраховка
        result := Format('<Exception>%s</Exception>', [ResponseNode.text]);
        exit;
      end;
      {Запрос выполнился корректно, разберем возвращенные им результаты}
      result := ResultNode.text;
      for i := 1 to ResponseNode.childNodes.length -1 do
      begin
        IndexOfParam := Params.IndexOf(ResponseNode.childNodes[i].nodeName);
        if IndexOfParam = -1 then
          continue;
        try
          Params[IndexOfParam + 1] := ResponseNode.childNodes[i].text;
        except
        end;
      end;
    end
    else
    begin
      {запрос отработал неудачно, получим текст ошибки}
      ResultNode := ResponseXmlDocument.selectSingleNode(
        'soap:Envelope/soap:Body/soap:Fault/faultstring');
      if not Assigned(ResultNode) then
      begin
        result := FRequest.responseText;//Format('<Exception>%s</Exception>', [FRequest.responseText]);
        exit;
      end;
      result := Format('<Exception>%s</Exception>', [ResultNode.text]);
    end;
  finally
    FRequest := nil;
    KillDomDocument(ResponseXmlDocument);
  end;
end;

function TPlaningProviderOld.GetResp(FunctionName: string; Params: TStringList;
  IsData: boolean; out ErrorMsg: WideString): IXMLDOMDocument2;
var
  FRequest: IXMLHTTPRequest;
  SoapRequest, WebServerName, ResultText: string;
  ResultNode: IXMLDOMNode;
begin
  FRequest := CreateOleObject('Microsoft.XMLHTTP') as IXMLHTTPRequest;
  result := InitXmlDocument;
  try
    FRequest.open('POST', FURL, false, EmptyParam, EmptyParam);
    SoapRequest := GetRequestBody(FunctionName, Params, IsData);
    WebServerName := GetWebServerName(FURL);
    SetHeader(FRequest, WebServerName, FunctionName, Length(SoapRequest));
    FRequest.Send(SoapRequest);

    { В этом месте возможны два варианта.
      Первый - вернулся XML-документ. Это значит, что запрос отработал штатно.
      Второй - вернулась HTML-страница с сообщением об ошибке выполнения запроса
      на сервере. В таком случае желательно извлечь из тела страницы
      информацию об ошибке.}
    if not result.load(FRequest.responseStream) then
    begin
      ResultText := FRequest.responseText;
      CutPart(ResultText, '<title>');
      ResultText := CutPart(ResultText, '</title>');
      ErrorMsg := ResultText;
      KillDomDocument(result);
    end
    else
    begin
      ResultNode := result.selectSingleNode('//GetCellsetDataNewResult');
      if Assigned(ResultNode) then
        ResultText := ResultNode.text;
      if Pos('Exception', ResultText) > 0 then
      begin
        ErrorMsg := ResultText;
        KillDomDocument(result);
      end;
    end;
  finally
    FRequest := nil;
  end;
end;

function TPlaningProviderOld.GetWebServerName(URL: string): string;
var
  DoubleSlashPos, SlashPos: integer;
begin
  DoubleSlashPos := Pos('//', URL);
  URL := copy(URL, DoubleSlashPos + 2, Length(URL));
  SlashPos := Pos('/', URL);
  result := copy(URL, 1, SlashPos - 1);
end;

function TPlaningProviderOld.GetRequestBody(FunctionName: string;
  Params: TStringList; IsData: boolean): string;
var
  i: integer;
  ParamName, ParamValue: string;
begin
  result := '<?xml version="1.0" encoding="windows-1251"?>' +
            '<soap:Envelope ' +
              'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ' +
              'xmlns:xsd="http://www.w3.org/2001/XMLSchema" ' +
              'xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">' +
              '<soap:Body>' +
                '<' + FunctionName + ' ' +
                  'xmlns="http://tempuri.org/">';
  if Assigned(Params) then
    for i := 0 to ((Params.Count div 2) - 1) do
    begin
      ParamName := Params.Strings[2 * i];
      ParamValue := Params.Strings[2 * i + 1];
      if IsData then
        result := result + '<' + ParamName + '>' + '<![CDATA[' + ParamValue + ']]></' + ParamName + '>'
      else
        result := result + '<' + ParamName + '>' + ParamValue + '</' + ParamName + '>';
    end;
  result := result +
                '</' + FunctionName + '>' +
              '</soap:Body>' +
            '</soap:Envelope>';
end;

procedure TPlaningProviderOld.SetHeader(var FRequest: IXMLHTTPRequest;
  WebServerName, FunctionName: string; RequestLenght: integer);
begin
  FRequest.setRequestHeader('Host', WebServerName);
  FRequest.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
  FRequest.setRequestHeader('Content-Length', IntToStr(RequestLenght));
  FRequest.setRequestHeader('SOAPAction', 'http://tempuri.org/' + FunctionName);
end;

procedure TPlaningProviderOld.CreateParamsList(ParamsArray: array of string);
var
  i: integer;
begin
  if not Assigned(Params) then
    Params := TStringList.Create
  else
    Params.Clear;
  for i := 0 to Length(ParamsArray) - 1 do
    Params.Add(ParamsArray[i]);
end;

function TPlaningProviderOld.Connect(const URL, Login, Password: WideString;
  AuthType: SYSINT; var SchemeName: WideString;
  WithinTaskContext: WordBool): WordBool;
var
  ResultString, ErrStr: string;
  IndexOfParam: integer;
begin
  FConnected := false;
  ClearErrors;
  CreateParamsList([
    'withinTaskContext', BoolToStr(WithinTaskContext),
    'authType', IIF(AuthType = 1, 'atWindows', 'adPwdSHA512'),
    'login', Login,
    'pwd', Password,
    'errStr', ErrStr]);
  try
    FURL := URL;
    try
      ResultString := GetWebResponse('Connect', Params, true);
      FConnected := AnsiUpperCase(ResultString) = 'TRUE';
    except
      on E:Exception do
        ResultString := E.Message;
    end;
    if FConnected then
    begin
      {Получим имя схемы}
      CreateParamsList([]);
      SchemeName := GetWebResponse('GetSchemeName', Params, false);
      FScheme := SchemeName;

      {Автоматическое обновление версии}
    end
    else
    begin
      if AnsiUpperCase(ResultString) = 'FALSE' then
      begin
        IndexOfParam := Params.IndexOf('errStr') + 1;
        FLastError := Params[IndexOfParam];
      end
      else
        FLastError := ResultString;
    end;
  finally
    result := FConnected;
  end;
end;

procedure TPlaningProviderOld.Disconnect;
begin
  try
    CreateParamsList([]);
    if FConnected then
      GetWebResponse('CloseSession', Params, false);
  except
    on E:Exception do
      FLastError := E.Message;
  end;
  FConnected := false;
  FURL := '';
  FScheme := '';
end;

function TPlaningProviderOld.GetMetadataDate: WideString;
begin
  ClearErrors;
  CreateParamsList([]);
  result := GetWebResponse('GetMetaDataDate2', Params, false);
end;

function TPlaningProviderOld.GetMetaData(var XmlDomDocument: IXMLDOMDocument2): WordBool;
var
  NeedRequest, NeedWarning: boolean;
begin
  result := true;
  ClearErrors;
  if FCache.LoadMetaData(XmlDomDocument, NeedWarning) then
  begin
    NeedRequest := IsNeedRequest(XmlDomDocument);
  end
  else
  begin
    if NeedWarning then
      FLastWarning := ermMetaDataCacheLoadFault;
    NeedRequest := true;
  end;
  // получаем метаданные с сервера
  if NeedRequest then
  begin
    XmlDomDocument := GetMetaDataWeb;
    result := CheckExcept(XmlDomDocument);
    if result then
    begin
      if not FCache.SaveMetaData(XmlDomDocument) then
        FLastWarning := ermMetaDataCacheSaveFault;
      if not (FCache.DeleteOldDimensions(XmlDomDocument)) then
        FLastWarning := ermMemberListCachClearFault;
    end;
  end;
end;

function TPlaningProviderOld.Writeback(const Data: WideString): WideString;
begin
  ClearErrors;
  CreateParamsList(['data', Data]);
  result := GetWebResponse('WriteBack2', Params, true);
end;

function TPlaningProviderOld.ClientSessionIsAlive: WordBool;
var
  TmpString: string;
begin
  result := false;
  if not FConnected then
    exit;
  try
    CreateParamsList([]);
    TmpString := GetWebResponse('ClientSessionIsAlive', Params, false);
    result := AnsiUpperCase(TmpString) = 'TRUE';
    FConnected := result;
  except
    result := false;
  end;
end;

function TPlaningProviderOld.GetMemberList(const ProviderId, CubeName,
  DimensionName, HierarchyName, LevelNames,
  PropertiesNamesList: WideString): IXMLDOMDocument2;

  function GetLevelsList(const DOM: IXMLDOMDocument2): TStringList;
  var
    NL: IXMLDOMNodeList;
    i: integer;
  begin
    result := TStringList.Create;
    NL := DOM.selectSingleNode('function_result/Levels').childNodes;
    for i := 0 to NL.length - 1 do
      result.Add(GetAttr(NL[i], 'name'));
  end;

var
  FilteredLevelsList, LevelsList: TStringList;
  NeedWarning: boolean;
begin
  ClearErrors;
  {получаем элементы - сперва пробуем из кэша}
  if not FCache.LoadMembersList(result, DimensionName, HierarchyName, NeedWarning, ProviderId) then
  begin
    if NeedWarning then
      FLastWarning := Format(ermMemberListCacheLoadFault,
        [GetFullDimensionName(DimensionName, HierarchyName)]);

    {с кэшем не получилось - тянем с сервера}
    result := GetMemberListWeb(ProviderId, CubeName, DimensionName,
      HierarchyName, '', PropertiesNamesList);
    if not Assigned(result) then
      exit;
    if not CheckExcept(result) then
      exit;
    if not FCache.SaveMembersList(result, ProviderId) then
      FLastWarning := Format(ermMemberListCacheSaveFault, [GetFullDimensionName(DimensionName, HierarchyName)]);
  end;

  // фильтруем элементы по уровням
  LevelsList := GetLevelsList(result);
  FilteredLevelsList := TStringList.Create;
  try
    if (LevelNames = '') then
      FilteredLevelsList.Assign(LevelsList)
    else
      LoadLevelNamesToList(FilteredLevelsList, LevelNames);
    result := FilterLevels(result, FilteredLevelsList, LevelsList);
  finally
    FreeStringList(LevelsList);
    FreeStringList(FilteredLevelsList);
  end;
end;

function TPlaningProviderOld.GetRecordsetData(const ProviderId,
  QueryText: WideString; var DataDom: IXMLDOMDocument2): WordBool;
var
  ResultText: widestring;
begin
  ClearErrors;
  if Assigned(DataDom) then
    KillDomDocument(DataDom);
  DataDom := InitXmlDocument;
  CreateParamsList([
    'providerId', ProviderId,
    'queryText', QueryText]);

  ResultText := GetWebResponse('GetRecordsetData2', Params, true);
  if not DataDom.loadXML(ResultText) then
  begin
    DataDom.loadXML('<Exception></Exception>');
    AppendCData(DataDom.documentElement, ResultText);
  end;
  result := CheckExcept(DataDom);
end;

function TPlaningProviderOld.GetCellsetData(const ProviderId,
  QueryText: WideString; var Data: IXMLDOMDocument2;
  out ErrorMsg: WideString): WordBool;
begin
  ClearErrors;
  if Assigned(Data) then
    KillDomDocument(Data);
  CreateParamsList([
    'providerId', ProviderId,
    'queryText', QueryText]);

  Data := GetResp('GetCellsetData2', Params, true, ErrorMsg);
  if Assigned(Data) then
  begin
    if Assigned(Data.firstChild) then
      if Data.firstChild.nodeType <> NODE_PROCESSING_INSTRUCTION then
        AddProcInstruction(Data);
    result := CheckExcept(Data);
  end
  else
    result := false;
end;

function TPlaningProviderOld.UpdateMemberList(const ProviderId: WideString;
  const SourceDom: IXMLDOMDocument2; var DestDom: IXMLDOMDocument2;
  const CubeName, DimensionName, HierarchyName, LevelList,
  PropertiesNamesList: WideString): WordBool;
var
  MetadataDom: IXMLDOMDocument2;
  NeedWarning: boolean;
begin
  ClearErrors;
  if FCache.LoadMetaData(MetadataDom, NeedWarning) then
    result := IsNeedUpdateMembers(MetadataDom, SourceDom, DimensionName, HierarchyName)
  else
  begin
    if NeedWarning then
      FLastWarning := ermMetaDataCacheLoadFault;
    result := true;
  end;
  if result then
  begin
    if not FCache.DeleteDimension(DimensionName, HierarchyName) then
      FLastWarning := ermMemberListCachClearFault;
    DestDom := GetMemberList(ProviderId, CubeName, DimensionName, HierarchyName,
      LevelList, PropertiesNamesList);
  end;
end;



procedure TPlaningProviderOld.ClearCache;
begin
  ;
end;

constructor TPlaningProviderOld.Create;
begin
  FCache := TPlaningCache.Create;
end;

destructor TPlaningProviderOld.Destroy;
begin
  if FConnected then
    Disconnect;
  if Assigned(FCache) then
    FreeAndNil(FCache);
  FreeStringList(Params);
  inherited;
end;

function TPlaningProviderOld.Connected: WordBool;
begin
  result := FConnected;
end;

procedure TPlaningProviderOld.FreeProvider;
begin
  Free;
end;

function TPlaningProviderOld.GetTaskContext(const TaskId: SYSINT): WideString;
begin
  ClearErrors;
  CreateParamsList(['taskId', IntToStr(TaskId)]);
  result := GetWebResponse('GetTaskContext', Params, true);
end;


function TPlaningProviderOld.UpdateTaskConsts(TaskId: SYSINT;
  const ConstsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  ClearErrors;
  CreateParamsList([
    'taskId', IntToStr(TaskId),
    'constsText', ConstsText,
    'sectionDivider', SectionDivider,
    'valuesDivider', ValuesDivider]);
  result := GetWebResponse('UpdateTaskConsts', Params, true);
end;

function TPlaningProviderOld.UpdateTaskParams(TaskId: SYSINT;
  const ParamsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  ClearErrors;
  CreateParamsList([
    'taskId', IntToStr(TaskId),
    'paramsText', ParamsText,
    'sectionDivider', SectionDivider,
    'valuesDivider', ValuesDivider]);
  result := GetWebResponse('UpdateTaskParams', Params, true);
end;

end.
