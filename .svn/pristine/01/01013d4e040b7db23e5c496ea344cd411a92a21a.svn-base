unit uPlaningProviderAbstract;

interface

uses
  PlaningProvider_TLB, uPlaningCache, MSXML2_TLB, ActiveX, uGlobalPlaningConst,
  classes, uFMAddinGeneralUtils, uXmlUtils;

type
  TPlaningProviderAbstract = class(TObject)
  protected
    FCache: TPlaningCache;

    FURL: string;
    FScheme: string;
    FLastError: string;
    FLastWarning: string;

    function CheckExcept(XmlDocument: IXmlDomDocument2): boolean;
    procedure ClearErrors;
    procedure LoadLevelNamesToList(List: TStringList; Names: string);
    {проверяет совпадение текущего подключения с записанным в доме }
    function IsNeedRequest(Dom: IXMLDOMDocument2): boolean;
    function IsNeedUpdateMembers(MetadataDom, SourceDom: IXMLDOMDocument2;
      const DimensionName, HierarchyName: string): boolean;
  public
    function URL: WideString; virtual;
    function Scheme: WideString; virtual;
    function LastError: WideString; virtual;
    function LastWarning: WideString; virtual;
    function Connected: WordBool; virtual; abstract;
    procedure ClearCache;
    procedure FreeProvider; virtual; abstract;

    {подключение к схеме}
    function Connect(const URL, Login, Password: WideString;
      AuthType: SYSINT; var SchemeName: WideString;
      WithinTaskContext: WordBool): WordBool; virtual; abstract;

    {разрыв подключения}
    procedure Disconnect; virtual; abstract;

    {дата-время генерации файла метаданных}
    function GetMetadataDate: WideString; virtual; abstract;

    {получение метаданных схемы}
    function GetMetaData(var XmlDomDocument: IXMLDOMDocument2): wordbool; virtual; abstract;

    {обратная запись}
    function Writeback(const Data: WideString): WideString; virtual; abstract;

    {в этом провайдере - просто заглушка для совместимости}
    function ClientSessionIsAlive: WordBool; virtual; abstract;

    {получение элементов измерения}
    function GetMemberList(const ProviderId, CubeName, DimensionName,
      HierarchyName, LevelNames,
      PropertiesNamesList: WideString): IXMLDOMDocument2; virtual; abstract;

    {mdx-запрос, обычный режим}
    function GetRecordsetData(const ProviderId, QueryText: WideString;
      var DataDom: IXMLDOMDocument2): WordBool; virtual; abstract;

    {mdx-запрос, серверный режим}
    function GetCellsetData(const ProviderId, QueryText: WideString;
      var Data: IXMLDOMDocument2;  out ErrorMsg: WideString): WordBool; virtual; abstract;

    function UpdateMemberList(const ProviderId: WideString;
      const SourceDom: IXMLDOMDocument2; var DestDom: IXMLDOMDocument2;
      const CubeName, DimensionName, HierarchyName, LevelList,
      PropertiesNamesList: WideString): WordBool; virtual; abstract;

    function GetTaskContext(const TaskId: SYSINT): WideString; virtual; abstract;

    function UpdateTaskParams(TaskId: SYSINT; const ParamsText: WideString;
      const SectionDivider: WideString; const ValuesDivider: WideString): WideString; virtual; abstract;

    function UpdateTaskConsts(TaskId: SYSINT; const ConstsText: WideString;
      const SectionDivider: WideString; const ValuesDivider: WideString): WideString; virtual; abstract;

  end;

implementation

{ TPlaningProviderAbstract }

function TPlaningProviderAbstract.CheckExcept(XmlDocument: IXmlDomDocument2): boolean;
var
  ExceptElement: IXmlDomNode;
begin
  ExceptElement := XmlDocument.selectSingleNode('Exception');
  result := not Assigned(ExceptElement);
  if not result then
    FLastError := ExceptElement.text;
end;

procedure TPlaningProviderAbstract.ClearCache;
begin
  ClearErrors;
  if Assigned(FCache) then
    if not FCache.ClearCache then
      FLastWarning := ermCacheClearFault;
end;

procedure TPlaningProviderAbstract.ClearErrors;
begin
  FLastError := '';
  FLastWarning := '';
end;

function TPlaningProviderAbstract.IsNeedRequest(Dom: IXMLDOMDocument2): boolean;
var
  Node: IXMLDOMNode;
  DocumentUrl: string;
  ServerMetadataDate, ClientMetadataDate: widestring;
begin
  result := true;
  if not Assigned(Dom) then
    exit;
  Node := Dom.selectSingleNode('function_result');
  if not Assigned(Node) then
    exit;
  DocumentUrl := GetStrAttr(Node, attrUrl, '');
  if DocumentUrl <> Url then
    exit;
  ClientMetadataDate := GetAttr(Node, 'date');
  ServerMetadataDate := GetMetadataDate;
  result := (ServerMetadataDate = '') or
    (CompareDates(ClientMetadataDate, ServerMetadataDate) < 0);
end;

function TPlaningProviderAbstract.IsNeedUpdateMembers(MetadataDom, SourceDom: IXMLDOMDocument2;
  const DimensionName, HierarchyName: string): boolean;
var
  XPath, CacheDomDate, SourceDomDate, CacheUrl, SourceUrl: string;
  Node: IXMLDOMNode;
begin
    {Если отличаются урл, откуда брались данные - обновлять обязательно}
    Node := MetadataDom.selectSingleNode('function_result');
    CacheUrl := GetStrAttr(Node, attrUrl, '');
    Node := SourceDom.selectSingleNode('function_result');
    SourceUrl := GetStrAttr(Node, attrUrl, '');
    result := CacheUrl <> SourceUrl;
    if result then
      exit;

    {сравниваем дату расчета измерения, взятую из файла метаданных в кэше...}
    XPath := 'function_result/SharedDimensions/Dimension[@name="' +
             DimensionName + '"]/Hierarchy[@name="' + HierarchyName + '"]';
    Node := MetadataDom.selectSingleNode(XPath);
    CacheDomDate := GetAttr(Node, 'processing_date');

    {... с датой из хмл самого измерения в листе}
    Node := SourceDom.selectSingleNode('function_result/Dimension');
    SourceDomDate := GetAttr(Node, 'processing_date');
    result := (CompareDates(SourceDomDate, CacheDomDate) < 0);
end;

function TPlaningProviderAbstract.LastError: WideString;
begin
  result := FLastError;
end;

function TPlaningProviderAbstract.LastWarning: WideString;
begin
  result := FLastWarning;
end;


procedure TPlaningProviderAbstract.LoadLevelNamesToList(List: TStringList;
  Names: string);
var
  Name: string;
begin
  Name := CutPart(Names, snBucks);
  while Name <> '' do
  begin
    List.Add(Name);
    Name := CutPart(Names, snBucks);
  end;
end;

function TPlaningProviderAbstract.Scheme: WideString;
begin
  result := FScheme;
end;

function TPlaningProviderAbstract.URL: WideString;
begin
  result := FUrl;
end;

end.

