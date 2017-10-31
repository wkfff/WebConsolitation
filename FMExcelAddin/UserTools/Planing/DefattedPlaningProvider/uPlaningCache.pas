unit uPlaningCache;

interface

uses
  Windows, SysUtils, Registry, MSXML2_TLB,
  uFMExcelAddInConst, uXMLUtils, StdCtrls, Classes, uFMAddinGeneralUtils,
  FileCtrl, uGlobalPlaningConst;

type
  TPlaningCache = class(TObject)
  private
    FCacheDir: widestring;
    // �������� �������������� ��� ����� �� ������ (���������, ��������, ���� ���������), ����������� � ���������
    function GetFileName(const Dom: IXMLDOMDocument2): widestring;
    // �������� �������������� � ����� ����� ����� (���������, ��������, ���� ���������)
    function GetFileKeys(FileName: widestring; var Dimension, Hierarchy, Date: widestring): boolean;
  public
    function GetCacheDir: widestring;
    function IsCacheDirUndefined: boolean;
    function SaveMetaData(DOM: IXMLDOMDocument2): boolean;
    function LoadMetaData(out DOM: IXMLDOMDocument2; var NeedWarning: boolean): boolean;
    function SaveMembersList(DOM: IXMLDOMDocument2; ProviderId: string): boolean;
    function LoadMembersList(out DOM: IXMLDOMDocument2; DimensionName, HierachyName: widestring;
      var NeedWarning: boolean; ProviderId: string): boolean;
    // ���������� �������� ������ ���������� (�������� - ���� ��������� ��������� (processed)) ���������
    function DeleteOldDimensions(Metadata: IXMLDOMDocument2): boolean;
    function ClearCache: boolean;
    // ������� ��������� �� ����
    function DeleteDimension(DimensionName, HierarchyName: string): boolean;
  end;

  function GetFullDimensionName(Dimension, Hierarchy: string): string;
  {��� ������� �������� ����������� ���������� ������ ����� 255 ��� �������������
    ���������� 2000 ������������ �������� � ������ ����� �� ��_��.
    �� � ������������ ��� ���� ������ �� ������}
  procedure UpdateLongNames(Dom: IXMLDOMDocument2);

implementation

function TPlaningCache.GetCacheDir: widestring;
var
  Path: array[0..255] of Char;
begin
  result := '';
  GetEnvironmentVariable('USERPROFILE', Path, 256);
  result := StrPas(Path);
  result := result + '\Application Data\Krista\FM\Office Add-in\Cache';
end;

function TPlaningCache.SaveMetaData(DOM: IXMLDOMDocument2): boolean;
var
  CacheDom: IXmlDomDocument2;
begin
  result := false;
  if not Assigned(DOM) then
    exit;
  if IsCacheDirUndefined then
    exit;
  CacheDom := CloneDocument(DOM);
  try
    try
      result := SaveFormattedXMLDocument(CacheDom, FCacheDir + fnMetaDataFileName);
    except
      result := false;
    end;
  finally
    KillDomDocument(CacheDom);
  end;
end;

function TPlaningCache.LoadMetaData(out DOM: IXMLDOMDocument2; var NeedWarning: boolean): boolean;
begin
  result := false;
  NeedWarning := true;
  if Assigned(Dom) then
    KillDomDocument(DOM);
  if (IsCacheDirUndefined) then
    exit;
  if not GetDomDocument(DOM) then
    exit;
  if FileExists(FCacheDir + fnMetaDataFileName) then
    result := DOM.load(FCacheDir + fnMetaDataFileName)
  else
    NeedWarning := false;
end;

function TPlaningCache.SaveMembersList(DOM: IXMLDOMDocument2; ProviderId: string): boolean;
var
  FileName: string;
  CacheDom: IXmlDomDocument2;
begin
  result := false;
  if not Assigned(DOM) then
    exit;
  if IsCacheDirUndefined then
    exit;
  FileName := GetFileName(DOM);
  FileName := ProviderId + '_' + FileName;
  if (FileName = '') then
    exit;
  CacheDom := CloneDocument(DOM);
  try
    try
      result := SaveFormattedXMLDocument(CacheDom, FCacheDir + FileName + '.xml');
    except
      result := false;
    end;
  finally
    KillDomDocument(CacheDom);
  end;
end;

function TPlaningCache.LoadMembersList(out DOM: IXMLDOMDocument2; DimensionName, HierachyName: widestring;
  var NeedWarning: boolean; ProviderId: string): boolean;
var
  FileName: string;
  sr: TSearchRec;
  ires: integer;
begin
  result := false;
  NeedWarning := true;
  if Assigned(Dom) then
    KillDomDocument(DOM);
  if IsCacheDirUndefined then
    exit;
  if not GetDomDocument(DOM) then
    exit;
  FileName := EncodeFileName(DimensionName) + '@'  + EncodeFileName(HierachyName);
  FileName := ProviderId + '_' + FileName;
  ires := FindFirst(FCacheDir + FileName + '@*.xml', faAnyFile, sr);
  try
    if (ires = 0) then
    begin
      FileName := sr.Name;
      result := DOM.load(FCacheDir + FileName);
    end
    else
      NeedWarning := false;
  finally
    FindClose(sr);
  end;
end;

function TPlaningCache.GetFileName(const DOM: IXMLDOMDocument2): widestring;
var
  DimensionRoot, HierachyRoot: IXMLDOMNode;
  DimensionName, HierarchyName, Date: widestring;
begin
  if not Assigned(DOM) then
    exit;
  DimensionRoot := DOM.selectsingleNode('function_result/Dimension');
  HierachyRoot := DOM.selectsingleNode('function_result/Hierarchy');
  DimensionName := GetAttr(DimensionRoot, 'name');
  HierarchyName := GetAttr(HierachyRoot, 'name');
  Date := GetAttr(DimensionRoot, 'processing_date');
  result := EncodeFileName(DimensionName) + '@' + EncodeFileName(HierarchyName) + '@' + EncodeFileName(Date);
end;

function TPlaningCache.IsCacheDirUndefined: boolean;
begin
  if (FCacheDir = '') then
    FCacheDir := GetCacheDir;
  // ���� ���������� ��� - �������
  if not DirectoryExists(FCacheDir) then
    ForceDirectories(FCacheDir);
  result := (FCacheDir = '');
  if (FCacheDir[Length(FCacheDir)] <> '\') then
    FCacheDir := FCacheDir + '\';
end;

function TPlaningCache.DeleteOldDimensions(Metadata: IXMLDOMDocument2): boolean;
var
  sr: TSearchRec;
  ires: integer;
  Error, IsOurFile: boolean;
  FileName, Dimension, Hierarchy, Date, MetadataDate: widestring;

  // �������� ���� �� ����������
  function GetMetadateDate(Metadata: IXMLDOMDocument2; Dimension, Hierarchy: widestring): widestring;
  var
    HierachyRoot: IXMLDOMNode;
    XPath: widestring;
  begin
    XPath := 'function_result/SharedDimensions/Dimension[@name="' +
             Dimension + '"]/Hierarchy[@name="' + Hierarchy + '"]';
    HierachyRoot := Metadata.selectSingleNode(XPath);
    result := GetAttr(HierachyRoot, 'processing_date');
  end;

begin
  result := true;
  Error := false;
  try
    if IsCacheDirUndefined then
      abort;
    ires := FindFirst(FCacheDir + '*.xml', faAnyFile, sr);
    try
      while (ires = 0) do
      begin
        FileName := sr.Name;
        try
          IsOurFile := GetFileKeys(FileName, Dimension, Hierarchy, Date);
          MetadataDate := GetMetadateDate(Metadata, Dimension, Hierarchy);
        except
          // ���������� ��������� �����
          IsOurFile := false;
        end;
        // ���� ���� ��������� ������ ��������������� ���� � ����������, �� ����� �� ���� ���������
        if IsOurFile and (CompareDates(Date, MetadataDate) < 0) then
          Error := Error or (not DeleteFile(FCacheDir + FileName));
        ires := FindNext(sr);
      end;
    finally
      FindClose(sr);
    end;
    if (Error) then
      abort;
  except
    result := false;
  end;
end;

function TPlaningCache.GetFileKeys(FileName: widestring; var Dimension,
  Hierarchy, Date: widestring): boolean;
var
  DogPos: integer;
  DecryptedParam: widestring;
begin
  result := true;
  try
    // ������� ����������
    FileName := Copy(FileName, 1, Length(FileName) - 4);
    // �������� ���������
    DogPos := Pos('@', FileName);
    if (DogPos = 0) then
      abort;
    DecryptedParam := Copy(FileName, 1, DogPos - 1);
    if (DecryptedParam = '') then
      abort;
    Dimension := DecodeFileName(DecryptedParam);
    Delete(FileName, 1, DogPos);
    // �������� ��������
    DogPos := Pos('@', FileName);
    if (DogPos = 0) then
      abort;
    DecryptedParam := Copy(FileName, 1, DogPos - 1);
    if (DecryptedParam = '') then
      abort;
    Hierarchy := DecodeFileName(DecryptedParam);
    Delete(FileName, 1, DogPos);
    // �������� ����
    DecryptedParam := FileName;
    if (DecryptedParam = '') then
      abort;
    Date := DecodeFileName(DecryptedParam)
  except
    result := false;
  end;
end;

function TPlaningCache.ClearCache: boolean;
var
  ires: integer;
  SearchList: TSearchRec;
  IsFileNotDeleted: boolean;
begin
  result := false;
  IsFileNotDeleted := false;
  if IsCacheDirUndefined then
    exit;
  ires := FindFirst(FCacheDir + '*.xml', faAnyFile, SearchList);
  try
    while (ires = 0) do
    begin
      if not DeleteFile(FCacheDir + SearchList.Name) then
        IsFileNotDeleted := true;
      ires := FindNext(SearchList);
    end;
  finally
    FindClose(SearchList);
  end;  
  result := not IsFileNotDeleted;
end;

function TPlaningCache.DeleteDimension(DimensionName, HierarchyName: string): boolean;
var
  FileName: string;
  sr: TSearchRec;
begin
  result := false;
  if IsCacheDirUndefined then
    exit;
  FileName := EncodeFileName(DimensionName) + '@' + EncodeFileName(HierarchyName) + '@';
  if (FindFirst(FCacheDir + FileName + '*.xml', faAnyFile, sr) = 0) then
    try
      FileName := sr.Name;
      result := DeleteFile(FCacheDir + FileName);
    finally
      FindClose(sr);
    end;
end;

function GetFullDimensionName(Dimension, Hierarchy: string): string;
begin
  result := Dimension;
  if (Hierarchy <> '') then
    result := result + '.' + Hierarchy;
end;

procedure UpdateLongNames(Dom: IXMLDOMDocument2);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Name, PkId: string;
begin
  NL := Dom.selectNodes('function_result/Members//Member');
  for i := 0 to NL.length - 1 do
  begin
    Name := GetStrAttr(NL[i], attrName, '');
    PkId := GetStrAttr(NL[i], attrPKID, '');
    if (Pos(PkId, Name) = 1) and (PkId <> Name) then
    begin
      Delete(Name, 1, Length(PkId));
      SetAttr(NL[i], attrName, Name);
    end;
  end;
end;

end.
