unit uPlaningProviderNew;

interface

uses
  Windows, ActiveX, Classes, ComObj, PlaningProvider_TLB, ComServ,
  Krista_FM_PlaningProviderCOMWrapper_TLB, uGlobalPlaningConst,
  uFMAddinGeneralUtils, uXMLUtils, uPlaningCache, MSXML2_TLB, uFMAddinXMLUtils,
  SysUtils, uPlaningProviderAbstract;

type
  TPlaningProviderNew = class(TPlaningProviderAbstract)
  private
    Proxy: PlaningProviderComWrapper;

  public
    constructor Create;
    destructor Destroy; override;

    function Scheme: WideString; override;
    function Connected: WordBool; override;
    procedure FreeProvider; override;

    {подключение к схеме}
    function Connect(const URL, Login, Password: WideString;
      AuthType: SYSINT; var SchemeName: WideString;
      WithinTaskContext: WordBool): WordBool; override;

    {разрыв подключени€}
    procedure Disconnect; override;

    {дата-врем€ генерации файла метаданных}
    function GetMetadataDate: WideString; override;

    {получение метаданных схемы}
    function GetMetaData(var XmlDomDocument: IXMLDOMDocument2): wordbool; override;

    {обратна€ запись}
    function Writeback(const Data: WideString): WideString; override;

    {в этом провайдере - просто заглушка дл€ совместимости}
    function ClientSessionIsAlive: WordBool; override;

    {получение элементов измерени€}
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

{ TPlaningProviderNew }

function TPlaningProviderNew.ClientSessionIsAlive: WordBool;
begin
  result := true;
end;

function TPlaningProviderNew.Connect(const URL, Login,
  Password: WideString; AuthType: SYSINT; var SchemeName: WideString;
  WithinTaskContext: WordBool): WordBool;
var
  errStr: widestring;
begin
  ClearErrors;
  result := Proxy.Connect(URL, Login, Password, AuthType, WithinTaskContext, errStr);
  if result then
  begin
    FURL := URL;
    //FScheme := Proxy.GetSchemeName;
  end
  else
    FLastError := errStr;
end;

function TPlaningProviderNew.Connected: WordBool;
begin
  result := false;
  try
    result := Proxy.Connected;
  except
  end;
end;

constructor TPlaningProviderNew.Create;
begin
  inherited;
  try
    Proxy := CreateComObject(CLASS_PlaningProviderComWrapper) as PlaningProviderComWrapper;
    FCache := TPlaningCache.Create;
  except
    on E: Exception do
    begin
      FLastError := E.Message;
      ShowError(FLastError);
    end;
  end;
end;

destructor TPlaningProviderNew.Destroy;
begin
  if Connected then
    Disconnect;
  if Assigned(Proxy) then
    Proxy.Dispose;
  if Assigned(FCache) then
    FreeAndNil(FCache);
  inherited;
end;

procedure TPlaningProviderNew.Disconnect;
begin
  try
    Proxy.Disconnect;
  except
  end;
  FURL := '';
  FScheme := '';
end;

procedure TPlaningProviderNew.FreeProvider;
begin
  Free;
end;

function TPlaningProviderNew.GetCellsetData(const ProviderId,
  QueryText: WideString; var Data: IXMLDOMDocument2;
  out ErrorMsg: WideString): WordBool;
var
  ResultText: string;
begin
  ClearErrors;
  if Assigned(Data) then
    KillDomDocument(Data);

  ResultText := Proxy.GetCellsetData(ProviderId, QueryText);
  Data := InitXmlDocument;

  if not Data.loadXML(ResultText) then
  begin
    (*if Assigned(Data.firstChild) then
      if Data.firstChild.nodeType <> NODE_PROCESSING_INSTRUCTION then
        AddProcInstruction(Data);*)
    FLastError := ResultText;
    ErrorMsg := ResultText;
    result := false;
  end
  else
    result := CheckExcept(Data);
end;

function TPlaningProviderNew.GetMemberList(const ProviderId, CubeName,
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
  ResultText: string;
  Node: IXMLDOMNode;
begin
  ClearErrors;
  {получаем элементы - сперва пробуем из кэша}
  if not FCache.LoadMembersList(result, DimensionName, HierarchyName, NeedWarning, ProviderId) then
  begin
    if NeedWarning then
      FLastWarning := Format(ermMemberListCacheLoadFault,
        [GetFullDimensionName(DimensionName, HierarchyName)]);

    {с кэшем не получилось - т€нем с сервера}
    ResultText := Proxy.GetMembers(ProviderId, CubeName, DimensionName, HierarchyName,
      LevelNames, PropertiesNamesList);

    result := InitXmlDocument;
    result.loadXML(ResultText);
    Node := result.selectSingleNode('function_result');
    if (Node <> nil) then
    begin
      (Node as IXmlDomElement).setAttribute(attrUrl, URL);
      (Node as IXmlDomElement).setAttribute('scheme', Scheme);
      UpdateLongNames(result);
    end
    else
      if not CheckExcept(result) then
        exit;

    if not FCache.SaveMembersList(result, ProviderId) then
      FLastWarning := Format(ermMemberListCacheSaveFault, [GetFullDimensionName(DimensionName, HierarchyName)]);
  end;
  
  // фильтруем элементы по уровн€м
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

function TPlaningProviderNew.GetMetaData(var XmlDomDocument: IXMLDOMDocument2): wordbool;
var
  ResultText: widestring;
  Node: IXmlDomNode;
  NeedRequest, NeedWarning: boolean;
begin
  result := true;
  ClearErrors;

  {сперва пытаемс€ загрузить из кэша. если удалось, сравниваем даты, определ€€
    необходимость запроса к серверу.}
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
    XmlDomDocument := InitXmlDocument;
    ResultText := Proxy.GetMetaData;

    XmlDomDocument.loadXML(ResultText);
    Node := XmlDomDocument.selectSingleNode('function_result');
    if Assigned(Node) then
    begin
      SetAttr(Node, attrUrl, URL);
      SetAttr(Node, 'scheme', Scheme);
    end;
    if not Assigned(XmlDomDocument.documentElement) then
    begin
      result := false;
      FLastError := ResultText;
      (*result.loadXML('<Exception></Exception>');
      AppendCData(result.documentElement, ResultText);*)
    end;

    if result then
    begin
      if not FCache.SaveMetaData(XmlDomDocument) then
        FLastWarning := ermMetaDataCacheSaveFault;
      if not (FCache.DeleteOldDimensions(XmlDomDocument)) then
        FLastWarning := ermMemberListCachClearFault;
    end;
  end;
end;

function TPlaningProviderNew.GetMetadataDate: WideString;
begin
  ClearErrors;
  result := Proxy.GetMetadataDate;
end;

function TPlaningProviderNew.GetRecordsetData(const ProviderId,
  QueryText: WideString; var DataDom: IXMLDOMDocument2): WordBool;
var
  ResultText: string;
begin
  ClearErrors;
  if Assigned(DataDom) then
    KillDomDocument(DataDom);
  DataDom := InitXmlDocument;

  ResultText := Proxy.GetRecordsetData(ProviderId, QueryText);
  if not DataDom.loadXML(ResultText) then
  begin
    FLastError := ResultText;
    result := false;
  end
  else
    result := CheckExcept(DataDom);
end;

function TPlaningProviderNew.GetTaskContext(const TaskId: SYSINT): WideString;
begin
  ClearErrors;
  //try
  result := Proxy.GetTaskContext(TaskId);
(*  except
    on e: exception do
      showinfo(e.message);
  end;*)
end;

function TPlaningProviderNew.Scheme: WideString;
begin
  result := ''
end;

function TPlaningProviderNew.UpdateMemberList(const ProviderId: WideString;
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

function TPlaningProviderNew.UpdateTaskConsts(TaskId: SYSINT;
  const ConstsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  ClearErrors;
  result := Proxy.UpdateTaskConsts(TaskId, ConstsText, SectionDivider, ValuesDivider);
end;

function TPlaningProviderNew.UpdateTaskParams(TaskId: SYSINT;
  const ParamsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  ClearErrors;
  result := Proxy.UpdateTaskParams(TaskId, ParamsText, SectionDivider, ValuesDivider);
end;

function TPlaningProviderNew.Writeback(const Data: WideString): WideString;
begin
  ClearErrors;
  result := Proxy.Writeback(Data);
end;

end.

