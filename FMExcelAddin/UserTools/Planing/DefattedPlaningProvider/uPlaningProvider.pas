{$A+,B-,C+,D+,E-,F-,G+,H+,I+,J+,K-,L+,M-,N+,O+,P+,Q-,R-,S-,T-,U-,V+,W-,X+,Y+,Z1}
{$MINSTACKSIZE $00004000}
{$MAXSTACKSIZE $00100000}
{$IMAGEBASE $00400000}
{$APPTYPE GUI}
unit uPlaningProvider;

interface

uses
  Windows, ActiveX, Classes, ComObj, PlaningProvider_TLB, StdVcl, SysUtils,
  MSXML2_TLB, uFMExcelAddInConst, ComServ, uXMLUtils,
  uFMAddinXMLUtils, uGlobalPlaningConst,
  uPlaningProviderOld, uPlaningProviderNew, uPlaningProviderAbstract;

type
  TConnectionType = (ctUnknown, ctWebService, ctNetRemoting);

  TPlaningProvider_ = class(TTypedComObject, IPlaningProvider)
  private
    FConnectionType: TConnectionType;
    FProviderWebService: TPlaningProviderOld;
    FProviderNetRemoting: TPlaningProviderNew;

    function GetProvider: TPlaningProviderAbstract;
    procedure CheckUrl(URL: string);
  public
    procedure AfterConstruction; override;


    function URL: WideString; stdcall;
    function Scheme: WideString; stdcall;
    function LastError: WideString; stdcall;
    function LastWarning: WideString; stdcall;
    function Connected: WordBool; stdcall;
    procedure ClearCache; stdcall;
    procedure FreeProvider; stdcall;


    {подключение к схеме с передачей авторизационной информации.
      ƒавно уже единственный вариант.}
    function Connect(const URL, Login, Password: WideString;
      AuthType: SYSINT; var SchemeName: WideString;
      WithinTaskContext: WordBool): WordBool; stdcall;

    {разрыв подключени€}
    procedure Disconnect; stdcall;

    {дата-врем€ генерации файла метаданных}
    function GetMetadataDate: WideString; stdcall;

    {получение метаданных схемы}
    function GetMetaData(var XmlDomDocument: IXMLDOMDocument2): wordbool; stdcall;

    {обратна€ запись}
    function Writeback(const Data: WideString): WideString; stdcall;

    {уведомление веб-сервису, что пользовательска€ сесси€ еще жива, чтоб не убил}
    function ClientSessionIsAlive: WordBool; stdcall;

    {получение элементов измерени€}
    function GetMemberList(const ProviderId, CubeName, DimensionName,
      HierarchyName, LevelNames,
      PropertiesNamesList: WideString): IXMLDOMDocument2; stdcall;

    {mdx-запрос, обычный режим}
    function GetRecordsetData(const ProviderId, QueryText: WideString;
      var DataDom: IXMLDOMDocument2): WordBool; stdcall;

    {mdx-запрос, серверный режим}
    function GetCellsetData(const ProviderId, QueryText: WideString;
      var Data: IXMLDOMDocument2;  out ErrorMsg: WideString): WordBool; stdcall;

    function UpdateMemberList(const ProviderId: WideString;
      const SourceDom: IXMLDOMDocument2; var DestDom: IXMLDOMDocument2;
      const CubeName, DimensionName, HierarchyName, LevelList,
      PropertiesNamesList: WideString): WordBool; stdcall;

    function GetTaskContext(TaskId: SYSINT): WideString; stdcall;

    function UpdateTaskParams(TaskId: SYSINT; const ParamsText, SectionDivider,
      ValuesDivider: WideString): WideString; stdcall;

    function UpdateTaskConsts(TaskId: SYSINT; const ConstsText, SectionDivider,
      ValuesDivider: WideString): WideString; stdcall;
  end;

implementation

{ TPlaningProvider_ }

procedure TPlaningProvider_.AfterConstruction;
begin
  inherited;
  FConnectionType := ctUnknown;
end;

procedure TPlaningProvider_.CheckUrl(URL: string);
begin
  if Pos(AnsiUpperCase('http:'), AnsiUpperCase(URL)) > 0 then
    FConnectionType := ctWebService
  else
    FConnectionType := ctNetRemoting;
end;

procedure TPlaningProvider_.ClearCache;
var
  Provider: TPlaningProviderAbstract;
begin
  Provider := GetProvider;
  if Assigned(Provider) then
    Provider.ClearCache;
end;

function TPlaningProvider_.ClientSessionIsAlive: WordBool;
begin
  result := GetProvider.ClientSessionIsAlive;
end;

function TPlaningProvider_.Connect(const URL, Login, Password: WideString;
  AuthType: SYSINT; var SchemeName: WideString;
  WithinTaskContext: WordBool): WordBool;
begin
  CheckUrl(URL);
  result := GetProvider.Connect(URL, Login, Password, AuthType, SchemeName,
    WithinTaskContext);
end;

function TPlaningProvider_.Connected: WordBool;
var
  Provider: TPlaningProviderAbstract;
begin
  result := false;
  Provider := GetProvider;
  if Assigned(Provider) then
    result := GetProvider.Connected;
end;

procedure TPlaningProvider_.Disconnect;
var
  Provider: TPlaningProviderAbstract;
begin
  Provider := GetProvider;
  if Assigned(Provider) then
    Provider.Disconnect;
end;

procedure TPlaningProvider_.FreeProvider;
begin
  if Assigned(FProviderWebService) then
    FProviderWebService.FreeProvider;
  if Assigned(FProviderNetRemoting) then
    FProviderNetRemoting.FreeProvider;
end;

function TPlaningProvider_.GetCellsetData(const ProviderId,
  QueryText: WideString; var Data: IXMLDOMDocument2;
  out ErrorMsg: WideString): WordBool;
begin
  result := GetProvider.GetCellsetData(ProviderId, QueryText, Data, ErrorMsg);
end;

function TPlaningProvider_.GetMemberList(const ProviderId, CubeName,
  DimensionName, HierarchyName, LevelNames,
  PropertiesNamesList: WideString): IXMLDOMDocument2;
begin
  result := GetProvider.GetMemberList(ProviderId, CubeName, DimensionName,
    HierarchyName, LevelNames, PropertiesNamesList);
end;

function TPlaningProvider_.GetMetaData(
  var XmlDomDocument: IXMLDOMDocument2): wordbool;
begin
  result := GetProvider.GetMetaData(XmlDomDocument);
end;

function TPlaningProvider_.GetMetadataDate: WideString;
begin
  result := GetProvider.GetMetadataDate;
end;

function TPlaningProvider_.GetProvider: TPlaningProviderAbstract;
begin
  case FConnectionType of
    ctWebService:
      begin
        if not Assigned(FProviderWebService) then
          FProviderWebService := TPlaningProviderOld.Create;
        result := FProviderWebService;
      end;
    ctNetRemoting:
      begin
        if not Assigned(FProviderNetRemoting) then
          FProviderNetRemoting := TPlaningProviderNew.Create;
        result := FProviderNetRemoting;
      end;
    else
      result := nil;
  end;
end;

function TPlaningProvider_.GetRecordsetData(const ProviderId,
  QueryText: WideString; var DataDom: IXMLDOMDocument2): WordBool;
begin
  result := GetProvider.GetRecordsetData(ProviderId, QueryText, DataDom);
end;

function TPlaningProvider_.LastError: WideString;
var
  Provider: TPlaningProviderAbstract;
begin
  result := '';
  Provider := GetProvider;
  if Assigned(Provider) then
    result := Provider.LastError;
end;

function TPlaningProvider_.LastWarning: WideString;
var
  Provider: TPlaningProviderAbstract;
begin
  result := '';
  Provider := GetProvider;
  if Assigned(Provider) then
    result := Provider.LastWarning;
end;

function TPlaningProvider_.Scheme: WideString;
var
  Provider: TPlaningProviderAbstract;
begin
  result := '';
  Provider := GetProvider;
  if Assigned(Provider) then
    result := Provider.Scheme;
end;

function TPlaningProvider_.UpdateMemberList(const ProviderId: WideString;
  const SourceDom: IXMLDOMDocument2; var DestDom: IXMLDOMDocument2;
  const CubeName, DimensionName, HierarchyName, LevelList,
  PropertiesNamesList: WideString): WordBool;
begin
  result := GetProvider.UpdateMemberList(ProviderId, SourceDom, DestDom,
    CubeName, DimensionName, HierarchyName, LevelList, PropertiesNamesList);
end;

function TPlaningProvider_.URL: WideString;
begin
  result := GetProvider.URL;
end;

function TPlaningProvider_.Writeback(const Data: WideString): WideString;
begin
  result := GetProvider.Writeback(Data);
end;

function TPlaningProvider_.GetTaskContext(TaskId: SYSINT): WideString;
begin
  result := GetProvider.GetTaskContext(TaskId);
end;

function TPlaningProvider_.UpdateTaskParams(TaskId: SYSINT;
  const ParamsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  result := GetProvider.UpdateTaskParams(TaskId, ParamsText, SectionDivider, ValuesDivider);
end;

function TPlaningProvider_.UpdateTaskConsts(TaskId: SYSINT;
  const ConstsText, SectionDivider, ValuesDivider: WideString): WideString;
begin
  result := GetProvider.UpdateTaskConsts(TaskId, constsText, SectionDivider, ValuesDivider);
end;

initialization
  TTypedComObjectFactory.Create(ComServer, TPlaningProvider_, Class_PlaningProvider_,
    ciMultiInstance, tmApartment);
end.






