{
  ������������ ������.
  ����� ��������� ������������ ��������� XML-���������� ����� ������������.
  ���� ���� ������������ �� �������������� XML-���������, �� �� ������ �����
  ������.
  ������������ XML-�������, �� ��������� �� ��������� �����, �� ������ �����
  �������. �� ����� � Glogal\uXMLUtils.xml.
  (!) � ���� ������ �������� ����� �������� ������� ������ ��� ����������
  �������.
}

unit uFMAddinXMLUtils;

interface

uses
  MSXML2_TLB, SysUtils, uXMLUtils, uFMExcelAddinConst, uFMAddinGeneralUtils,
  uFMAddinRegistryUtils, Classes, PlaningTools_TLB, uGlobalPlaningConst;

type
  // ��� ��� ����� ������������
  TAxisType = (axColumn, axRow);

  TSetPBarPositionProc = procedure(CurPosition, MaxPosition: integer) of object;

  TMdxOptimization = (moAxis, moFilter, moServer, moServer2);

  TIntegerArray = array of integer;

  {���������� ������� ���� XML}
  function SelectSingleNode(Root: IXMLDOMNode; XPathQuery: string): IXMLDOMNode;
  {���������� ������� ������ ����� XML}
  function SelectNodes(Root: IXMLDOMNode; XPathQuery: string): IXMLDOMNodeList;
  {������������ ���-�� �������� ��������� � ���� (��� ���������)}
  function LeafCount(Node: IXMLDOMNode; CellSize: integer): integer;
  {�������������� ������� ��������� ������}
  procedure FillNodeListAttribute(NL: IXMLDOMNodeList; AName: string;
    Value: Variant); overload;
  procedure FillNodeListAttribute(Dom: IXMLDOMDocument2; XPath, AName: string;
    Value: Variant); overload;
  {������� �� ���-� �������� � ���������� = 0
  ConsiderInfluence - �����-�� ��������� �� �������� ������� �������}
  procedure CutAllInvisible(DOM: IXMLDOMDocument2; ConsiderInfluence: boolean);
  {���������� ����������� ��������, ������������ � ���������� ���
    ����������� MDX-�������}
  procedure SetCheckedIndication(Dom: IXMLDomDocument2);
  {���������� ��� ���� (ntMember, ntSummary, ntMemberDummy, ntSummaryDummy)}
  function GetNodeType(Node: IXMLDOMNode): string;
  {���� �������� ������ ��� ��� ���������}
  function IsBelongsToSummary(Node: IXMLDOMNode): boolean;
  {������� ����������� ��� ���� ���� }
  procedure KillSummary(Node: IXMLDOMNode);
  // �������� �������
  procedure RemoveChildren(Node: IXMLDOMNode);
  {���������� �������� ��� �������� (������� ��������)}
  procedure BreakHierarchy(Dom: IXMLDOMDocument2; ReverseOrder: boolean);
  {�������� �� ���� ���� ��������}
  function IsDataMember(Node: IXMLDOMNode): boolean;
  {���� �� ������ ��������� �������� � ������, ���������� false}
  function WriteDocumentLog(DOM: IXMLDOMDocument2; NameAndFormat: string): boolean;
  {��������� ������ ��� ������� �� XML ��������� ������� � ����}
  function FilterLevels(const DOM:IXMLDOMDocument2;const FilterLevelsList,
    AllLevels: TStringList):IXMLDOMDocument2;
  {���������� ��� ��������� ���������� ��������� ����� ����������� ��
    ����� �� ��������� ���������.
    Dom1 - ��������, �� �� � �������������� ��������;
    Dom2 - ������������ ��������;
    RootXPath - ���� � ��������� ����
    NodeXPath - ������, �� �������� ���������� ����;
    AttrList - ������ ���� ���������, �� ��������� ������� ������������
    �����������}
  procedure MergeXMLDocuments(Dom1, Dom2: IXMLDOMDocument2;
    RootXPath, NodeXPath: string; AttrList: TStringList);
  {�������������� ������� DataDOM-��}
  (*procedure ConcatenateData(SrcDOM, PartDOM: IXMLDOMDocument2;
    BothAxisAliasesList: TStringList);*)
  {����� ������� �������� � ��������. ������ �������� ����� ������������ ��� ���������� ��������
    ����������� ���������}
  procedure ConcatenateDataEx(SrcDOM, PartDOM: IXMLDOMDocument2; BothAxisAliasesList: TStringList);
  {���� ���������� ������ � ��������� name}
  function GetRealParent(Node: IXMLDOMNode): IXMLDOMNode;
  // �������� ��������� �� ����� � ����
  procedure CopyAttrs(SourceNode: IXmlDomNode; var DestNode: IXmlDomNode);
  //� ������
  function CopyAttrsToString(SourceNode: IXmlDomNode): string;
  // �������� �������� �� ����������� ���������
  procedure CopyAttrsEx(SourceNode: IXmlDomNode; var DestNode: IXmlDomNode;
    ExceptAttrs: array of string);
  // �������� �������� � �� ���������������
  procedure CopyAttrsWithConversion(SourceNode, DestNode: IXMLDOMNode; List: TStringList);

  {������������ �������� checked � influence �� ������� ��������� � �����}
  procedure CopyMembersState(OldMembers, NewMembers: IXMLDOMDocument2;
    Proc: TSetPBarPositionProc);
  {����������, ���� �� �������� ���� � ������������ ��������� Checked
  ���� ������, ��������� ����� ������ �������}
  function HasCheckedDescendants(Node: IXMLDOMNode): boolean;
  // ��������� �������� ������� �� �����
  function GetMemberAttrByKey(Dom: IXmlDomDocument2; KeyName, KeyValue,
                              AttrName: string; var IsLeaf: boolean): string;
  // ��������� ���� ����������
  // ������: [AttrName1, AttrValue1..AttrNameN, AttrValueN]
  procedure FillNodeAttributes(var Node: IXMLDOMNode; Attrs: array of string);
  // ������� � �������� �������� ������� � �������� ������, ���������� �������� �������
  function CreateAndAddChild(ParentElement: IXMLDOMNode; NodeName: string): IXMLDOMNode;
  // ������� ������� � ��������� ��� ����������, ���������� �������� �������
  // �������� � ���� �������: [AttrName1, AttrValue1..AttrNameN, AttrValueN]
  function CreateWithAttributes(ParentDom: IXmlDomDocument2; NodeName: string; Attrs: array of string): IXMLDOMNode;
  // ������� � �������� ������� � ��������� ��� ����������, ���������� �������� �������
  // �������� � ���� �������: [AttrName1, AttrValue1..AttrNameN, AttrValueN]
  function CreateAndAddWithAttributes(ParentElement: IXMLDOMNode; NodeName: string; Attrs: array of string): IXMLDOMNode;
  {������ MDX-�������� ��������. Optimization ��������� �� ��,
   ����� �� ������������ ������� Children � Descendants}
  function GetMDXEnumeration(MembersDOM: IXMLDOMDocument2;
    Optimization: TMdxOptimization): string;
  {�� MDX-�������� ���������� "������������" (������������)}
  function MDXEnumerationToMembersDescription(MDXEnum: string): string;
  {���������� "������������" �������� ��������}
  function GetMembersDescription(MembersDOM: IXMLDOMDocument2;
    Optimization: TMdxOptimization): string;
  {���������� ������ "������������" �������� ��������.
   ����� ����������� �� ������� ���� ������, � ����������������� � ���� ������
   ������� ������. ��������� �������� ������ ���������� ����� � ������}
  function GetMembersDescriptionLikeTree(MembersDOM: IXMLDOMDocument2): string;
  procedure FilterMembersDomEx(Dom: IXMLDOMDocument2);
  function GetLevelNodeByIndex(Dom: IXMLDomDocument2; LevelIndex: integer): IXMLDOMNode;
  // �������� ��� � ���������� � �������������� ��������� - ������� ��� ������� � ������
  procedure MakeDefaultMembersDom(var Members: IXmlDomDocument2);
  { true, ���� �� ������� ������ ������ �������� ������.}
  function IsWritebackSenseless(Node: IXMLDOMNode): boolean; overload;
  function IsWritebackSenseless(RangeName: string): boolean; overload;
  function IsGrandSummary(Node: IXMLDOMNode): boolean;
  {�������������� ������ Influences, ���� ��� ����, �� �������� ��, ���� ���, ��������}
  function InitInfluencesNode(DOM: IXMLDOMDocument2): IXMLDOMNode;
  {��������� ����������� ��������� � ��������� ������}
  procedure CopyInfluences(DOM: IXMLDOMDocument2);
  procedure GetIntArrayAttr(Node: IXMLDOMNode; AttributeName: string;
    var ResultArray: TIntegerArray);
  procedure SetIntArrayAttr(Node: IXMLDOMNode; AttributeName: string;
    ResultArray: TIntegerArray);
  {������� � ��������� ���� �� ��������� �������}
  procedure RemoveNodes(Dom: IXMLDOMDocument2; XPath: string);


  { ������� ��� ������ � ��������� ������� � ��������� ������}

  {���������� ����� ���� � ������ ChildName, ������������ ��������������� Node}
  function GetChildText(Node: IXMLDOMNode; ChildName: string): string;
  {UniqueName ��������}
  function MemberUName(Node: IXMLDOMNode): string;
  {LNum �������� }
  function MemberLNum(Node: IXMLDOMNode): integer;
  {UniqueName ������ �������}
  function MemberLName(Node: IXMLDOMNode): string;
  {������ � ������ "All"}
  function MemberFromAllLevel(Node: IXMLDOMNode): boolean;
  {����� �������}
  function MemberCaption(Node: IXMLDOMNode): string;
  {������� �������}
  function MemberOrdinal(Node: IXMLDOMNode): string;
  function GetAxisTuples(DOM: IXMLDOMDocument2; AxType: TAxisType): IXMLDOMNodeList;
  function GetAxisVisibleTuples(DOM: IXMLDOMDocument2; AxType: TAxisType): IXMLDOMNodeList;
  { ���������� ��� ������� �� ���������� �� "Count" ������ ��������,
    ���������� ������ �������, � ������� ������� ��������;
    -1 �������� ������������ ��������.}
  function CompareTuples(Tuple1, Tuple2: IXMLDOMNode; Count: integer): integer;
  function IsItSummary(Node: IXMLDOMNode): boolean;
  {��� ������ 6742 - ���������� ������� �� ������ �����.
    �������� �� ��������� ���� �����, �� ����������� ������� �� ����� �������.
    ����, � ������� ��� ���� �������� � ����� ���, ��������� �� �������.}
  procedure DivideMembersDom(var SourceDom, PartDom: IXMLDOMDocument2;
    LeafCountLimit: integer; BrokenAxis: boolean);
  {��������� �������� ������ ���������� ��� ����� ��� ��������� ������� ���;
    �� ���� �������� ��������� ���������� GetFullAxisDom.
    Step - ���������� ���������� ��� �������� � ���������� ��������.
    ��� ����� � �������� ������ ����� 1, ��� �������� - ����������
    ����������� � ������� ���}
(*  procedure SetAxisNodesCoords(var Dom: IXMLDOMDocument2; AxisType: TAxisType;
    StartFrom, Step: integer);*)
  {������� ��������, ����� �����������}
(*  procedure EnlightAxisDom(var Dom: IXMLDOMDocument2; AxisType: TAxisType);*)

  {��-�� �������� ����������, ������������
  ������� � ������ �������� ��� �������� ���� �����.
  ������������� ��������� uXMLUtils.GetNodeStrAttr, ������ ��� ����������� ������������� ��������� }
  function ReadTaskContextAttr(Node: IXMLDOMNode; AttrName1, AttrName2: string): string;

  procedure DebugSaveNodeList(NL: IXMLDOMNodeList; Path: string);
  procedure DebugSaveDom(Dom: IXMLDOMDocument2; Path: string);

implementation
uses
  uCheckTV2;


function SelectSingleNode(Root: IXMLDOMNode; XPathQuery: string): IXMLDOMNode;
begin
  result := nil;
  if Assigned(Root) and (XPathQuery <> '') then
    result := Root.selectSingleNode(XPathQuery);
end;

function SelectNodes(Root: IXMLDOMNode; XPathQuery: string): IXMLDOMNodeList;
begin
  result := nil;
  if Assigned(Root) and (XPathQuery <> '') then
    result := Root.selectNodes(XPathQuery);
end;

function LeafCount(Node: IXMLDOMNode; CellSize: integer): integer;
begin
  result := Node.selectNodes('.//*[(not(Member)) and (not(Summary)) and (@checked="true")]').length;
  if result > 0 then
    result := result * CellSize
  else
    result := CellSize;
end;

procedure FillNodeListAttribute(NL: IXMLDOMNodeList; AName: string;
  Value: Variant);
var
  i: integer;
begin
  for i := 0 to NL.length - 1 do
    (NL[i] as IXMLDOMElement).setAttribute(AName, Value);
end;

procedure FillNodeListAttribute(Dom: IXMLDOMDocument2; XPath, AName: string;
  Value: Variant); overload;
var
  NL: IXMLDOMNodeList;
begin
  if not Assigned(Dom) then
    exit;
  NL := Dom.selectNodes(XPath);
  FillNodeListAttribute(NL, AName, Value);
end;

procedure CutAllInvisible(DOM: IXMLDOMDocument2; ConsiderInfluence: boolean);
var
  NL, NL2: IXMLDOMNodeList;
  i, j: integer;
  XPath: string;
  PrntNode: IXMLDOMNode;
begin
  {������ ���� �������� ����������� ���������}
  NL := Dom.selectNodes('function_result/Members//Member[@influence="3"]');
  for i := 0 to NL.length - 1 do
  begin
    NL2 := NL[i].childNodes;
    for j := NL2.length - 1 downto 0 do
      NL[i].removeChild(NL2[j]);
    (NL[i] as IXMLDOMElement).setAttribute(attrChecked, 'false');
  end;

  {��������, ������� �� �������� ���� � �� ����� ���������� ��������,
    ����������� ��������}
  XPath := 'function_result/Members//' + ntMember +
    IIF(ConsiderInfluence,
    '[(@checked="false") and not(.//Member[@checked="true"]) and (@influence="0")]',
    '[(@checked="false") and not(.//Member[@checked="true"])]');
  NL := DOM.selectNodes(XPath);
  for i := 0 to NL.length - 1 do
  begin
    PrntNode := NL[i].parentNode;
    PrntNode.removeChild(NL[i]);
  end;
end;

procedure SetCheckedIndication(Dom: IXMLDomDocument2);
var
  i: integer;
  NL: IXMLDOMNodeList;
begin
  NL := Dom.selectNodes('function_result/Members//Member[@allchidrenchecked]');
  for i := 0 to NL.length - 1 do
    NL[i].attributes.removeNamedItem('allchidrenchecked');
  NL := Dom.selectNodes('function_result/Members//Member[@alldescendantschecked]');
  for i := 0 to NL.length - 1 do
    NL[i].attributes.removeNamedItem('alldescendantschecked');

  NL := Dom.selectNodes('function_result/Members//Member[(*) and' +
    ' not (./Member[@checked="false"])]');
  for i := 0 to NL.length - 1 do
    (NL[i] as IXMLDOMElement).setAttribute(attrAllChildrenChecked, 'true');
  NL := Dom.selectNodes('function_result/Members//Member[(*) and' +
    ' not (.//Member[@checked="false"])]');
  for i := 0 to NL.length - 1 do
    (NL[i] as IXMLDOMElement).setAttribute(attrAllDescendantsChecked, 'true');
end;

function GetNodeType(Node: IXMLDOMNode): string;
begin
  result := '';
  if not Assigned(Node) then
    exit;
  if Node.nodeName = ntSummary then
  begin
    result := ntSummary;
    exit;
  end;
  if Node.nodeName = ntAliasInfo then
  begin
    result := ntAliasInfo;
    exit;
  end;
  
  if Node.nodeName = ntMember then
  begin

    if Assigned(Node.attributes.getNamedItem(attrName)) then
    begin
      result := ntMember;
      exit;
    end;

    repeat
      Node := Node.parentNode;
    until Assigned(Node.attributes.getNamedItem(attrName));
    if Node.nodeName = ntMember then
      result := ntMemberDummy
    else
      result := ntSummaryDummy;
  end;
end;

function IsBelongsToSummary(Node: IXMLDOMNode): boolean;
var
  NodeType: string;
begin
  result := false;
  if Assigned(Node) then
  begin
    NodeType := GetNodeType(Node);
    result := (NodeType = ntSummary) or (NodeType = ntSummaryDummy);
  end;
end;

procedure KillSummary(Node: IXMLDOMNode);
var
  Parent: IXMLDOMNode;
begin
  if not IsBelongsToSummary(Node) then
    exit; //����� �� ������� �������
  while GetNodeType(Node.parentNode) <> ntMember do
    Node := Node.parentNode;
  Parent := Node.parentNode;
  Node := Parent.removeChild(Node);
  Node := nil;
end;

procedure RemoveChildren(Node: IXMLDOMNode);
begin
  while Node.hasChildNodes do
    Node.removeChild(Node.firstChild);
end;

procedure BreakHierarchy(Dom: IXMLDOMDocument2; ReverseOrder: boolean);

  procedure CopyNode(Node, Root: IXMLDomNode);
  var
    Clone: IXMLDOMNode;
    i: integer;
    AliasInfoList: IXMLDOMNodeList;
    ParentUN: string;
    ParentNode: IXMLDOMNode;
  begin
    if ReverseOrder then
      for i := 0 to Node.childNodes.length - 1 do
        CopyNode(Node.childNodes[i], Root);

    if GetBoolAttr(Node, attrChecked, false) then
    begin
      Clone := Node.cloneNode(false);
      AliasInfoList := Node.selectNodes(ntAliasInfo);
      for i := 0 to AliasInfoList.length - 1 do
        Clone.AppendChild(Node.removeChild(AliasInfoList[i]));
      Root.appendChild(Clone);

      ParentUN := '';
      ParentNode := Node.parentNode;
      while ParentNode.nodeName <> 'Members' do
      begin
        if GetBoolAttr(ParentNode, attrChecked, false) then
        begin
          ParentUN := GetStrAttr(ParentNode, attrUniqueName, '');
          break;
        end;
        ParentNode := ParentNode.parentNode;
      end;

      if ParentUN <> '' then
        (Clone as IXMLDOMElement).setAttribute(attrParentUN, ParentUN);
    end;
    if not ReverseOrder then
      for i := 0 to Node.childNodes.length - 1 do
        CopyNode(Node.childNodes[i], Root);
  end;

var
  i: integer;
  NL: IXMLDomNodeList;
  Dom2: IXMLDomDocument2;
  Root: IXMLDOMNode;
begin
  if not Assigned(Dom) then
    exit;
  GetDomDocument(Dom2);
  Dom2.load(Dom);
  Root := Dom.selectSingleNode('function_result/Levels');
  NL := Root.childNodes;
  for i := 1 to NL.length - 1 do
    Root.removeChild(NL[1]);
  Root := Dom.selectSingleNode('function_result/Members');
  while Root.hasChildNodes do
    Root.removeChild(Root.childNodes[0]);
  NL := Dom2.childNodes;
  for i := 0 to NL.length - 1 do
    CopyNode(NL[i], Root);
end;

function IsDataMember(Node: IXMLDOMNode): boolean;
var
  UName: string;
begin
  UName := GetStrAttr(Node, attrUniqueName, '');
  result := Copy(UName, Length(UName) - 10, 11) = '.DATAMEMBER';
end;

function WriteDocumentLog (DOM: IXMLDOMDocument2; NameAndFormat: string): boolean;
begin
  result := Assigned(DOM) and (not(NameAndFormat = ''));
  if not result then
    exit;
  try
    DOM.save(AddinLogPath + NameAndFormat);
  except
    result := false;
  end;
end;

function FilterLevels(const DOM:IXMLDOMDocument2;
  const FilterLevelsList: TStringList; const AllLevels: TStringList):IXMLDOMDocument2;

  function IsRemoving(LevelInd: integer): Boolean;
  var
    sLevelName: string;
    i: integer;
  begin
    result := true;
    sLevelName := AllLevels[LevelInd];

    for i := 0 to FilterLevelsList.Count - 1 do
      if FilterLevelsList[i] = sLevelName then
      begin
        result := false;
        exit;
      end;
  end;

  procedure FilterMembers(RootAlt, RootNew: IXMLDOMNode; NumLevel:longint);
  var
    ChildrenNL, GrandChildrenNL: IXMLDOMNodeList;
    i, j, ChildrenCount: integer;
    Node: IXMLDOMNode;

  begin
    if not Assigned(RootNew) then
      exit;
    if not RootNew.hasChildNodes then
      exit;

    ChildrenNL := RootNew.childNodes;
    ChildrenCount := ChildrenNL.length;
    if IsRemoving(NumLevel) then
      for i := 0 to ChildrenCount - 1 do
      begin
        FilterMembers(RootAlt, ChildrenNL[0], NumLevel + 1);
        if ChildrenNL[0].hasChildNodes then
        begin
          GrandChildrenNL := ChildrenNL[0].childNodes;
          for j := 0 to GrandChildrenNL.length - 1 do
          begin
            Node := GrandChildrenNL[j].cloneNode(true);
            RootAlt.appendChild(Node);
          end;
        end;
        //  �������
        RootNew.removeChild(ChildrenNL[0]);
      end
    else
      if RootNew.hasChildNodes then
        for i := 0 to ChildrenNL.length - 1 do
          FilterMembers(ChildrenNL[i], ChildrenNL[i], NumLevel + 1);
  end;

  procedure FilterLevels_(Root : IXMLDOMNode);
  var
    ChilderNL: IXMLDOMNodeList;
    i, j, ChildrenCount: integer;
  begin
    if not Assigned(Root) then
      exit;
    ChilderNL := Root.childNodes;
    ChildrenCount := Root.childNodes.length;
    j := 0;
    for i := 0 to ChildrenCount - 1 do
      if IsRemoving(i) then
        Root.removeChild(ChilderNL[j])
      else
        inc(j)
  end;

var
  MemberRoot, LevelRoot: IXMLDOMNode;
begin
  Result := nil;
  if not Assigned(DOM) then
    exit;

  MemberRoot := DOM.selectSingleNode('function_result/Members');
  FilterMembers(MemberRoot, MemberRoot, 0);

  LevelRoot := DOM.selectSingleNode('function_result/Levels');
  FilterLevels_(LevelRoot);

  if Assigned(DOM) then
    result := DOM;
end;

procedure ConcatenateData(SrcDOM, PartDOM: IXMLDOMDocument2;
  BothAxisAliasesList: TStringList; OverrideOldValues: boolean);

  function GetCondition(CopiedNode: IXMLDOMNode): string;
  const
    ImpossibleValue = '���������'; //:)
  var
    i: integer;
    AtrName, AtrValue: string;
  begin
    result := '';

    for i := 0 to BothAxisAliasesList.Count - 1 do
    begin
      AtrName := BothAxisAliasesList[i];
      AtrValue := GetStrAttr(CopiedNode, AtrName, ImpossibleValue);

      {���������, ����-�� ����� ������ ��������� � ���������� ����}
      if (AtrValue <> ImpossibleValue) then
      begin {���� - ��������� ���������� ������� �� ��������}
        EncodeXPathString(AtrValue);
        AddTail(result, ' and ');
        result := result + Format('(@%s="%s")', [AtrName, AtrValue]);
      end
      else
      begin {��� - ��������� ������������� ���������� �������}
        AddTail(result, ' and ');
        result := result + Format(' not(@%s)', [AtrName]);
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
  Node, RootNode, ChildNode: IXMLDOMNode;
  i, j: integer;
  Condition, XPath: string;
  Attributes: IXMLDOMNamedNodeMap;
begin
  if not Assigned(SrcDOM) or not Assigned(PartDOM) then
    exit;
  RootNode := SrcDOM.selectSingleNode('function_result/data');
  if not Assigned(RootNode) then
    exit;
  {}
  NL := PartDOM.selectNodes('function_result/data/row');
  {���������� ���� ������������ ���������}
  for i := 0 to NL.length - 1 do
  begin
    {���������� ������� ������� �� ������ ���������}
    Attributes := NL[i].Get_attributes;
    Condition := GetCondition(NL[i]);
    try
      if Condition <> '' then
      begin
        XPath := 'function_result/data/row' + '[' + Condition + ']';
        Node := SrcDOM.selectSingleNode(XPath);
      end;
    except
    end;
    if not Assigned(Node) then
    begin
      Node := NL[i].cloneNode(true);
      RootNode.appendChild(Node);
      continue;
    end;

    {���������� ��� �������� ������������ ����, ���� � ������� ���� ������ �������� ��� - ���������.
      � ���� ����, �� �������� OverrideOldValues ���������� �������������� �� ������ �������� �����.}
    for j := 0 to Attributes.length - 1 do
      if not Assigned(Node.attributes.getNamedItem(Attributes[j].nodeName)) or
        (Assigned(Node.attributes.getNamedItem(Attributes[j].nodeName)) and OverrideOldValues) then
        (Node as IXMLDOMElement).setAttribute(Attributes[j].nodeName,
          Attributes[j].nodeValue);
    ChildNode := NL[i].firstChild;
    while Assigned(ChildNode) do
    begin
      Node.appendChild(ChildNode.cloneNode(true));
      ChildNode := ChildNode.nextSibling;
    end;
  end;
end;

procedure ConcatenateDataEx(SrcDOM, PartDOM: IXMLDOMDocument2; BothAxisAliasesList: TStringList);
begin
  if not Assigned(SrcDOM) or not Assigned(PartDOM) then
    exit;
  if Length(SrcDom.xml) > Length(PartDom.xml) then
    ConcatenateData(SrcDOM, PartDOM, BothAxisAliasesList, false)
  else
  begin
    ConcatenateData(PartDOM, SrcDOM, BothAxisAliasesList, true);
    SrcDom.load(PartDom);
  end;
end;

(*!!! archaic*)
procedure MergeXMLDocuments(Dom1, Dom2: IXMLDOMDocument2;
  RootXPath, NodeXPath: string; AttrList: TStringList);

  function GetCondition(ACollection: IXMLDOMNamedNodeMap): string;
  var
    i, Index: integer;
    AName, AValue, AttrCommaText: string;
  begin
    result := '';
    AttrCommaText := AttrList.CommaText;
    for i := 0 to ACollection.length - 1 do
    begin
      AName := ACollection[i].nodeName;
      Index := AttrList.IndexOf(AName);
      if Index = -1 then
        continue;
      AttrList.Delete(Index);
      AValue := string(ACollection[i].nodeValue);
      EncodeXPathString(AValue);
      AddTail(result, ' and ');
      result := result + Format('(@%s="%s")', [AName, AValue]);
    end;

    for i := 0 to AttrList.Count - 1 do
    begin
      AddTail(result, ' and ');
      result := result + Format(' not(@%s)', [AttrList[i]]);
    end;
    AttrList.CommaText := AttrCommaText;
  end;

var
  NL: IXMLDOMNodeList;
  Node, RootNode, ChildNode: IXMLDOMNode;
  i, j: integer;
  Condition, XPath: string;
  Attributes: IXMLDOMNamedNodeMap;
begin
  if not Assigned(Dom1) or not Assigned(Dom2) then
    exit;
  RootNode := Dom1.selectSingleNode(RootXPath);
  if not Assigned(RootNode) then
    exit;
  {}
  NL := Dom2.selectNodes(NodeXPath);
  {���������� ���� ������������ ���������}
  for i := 0 to NL.length - 1 do
  begin
    {���������� ������� ������� �� ������ ���������}
    Attributes := NL[i].Get_attributes;
    Condition := GetCondition(Attributes);
    try
      if Condition <> '' then
      begin
        XPath := NodeXPath + '[' + Condition + ']';
        Node := Dom1.selectSingleNode(XPath);
      end;
    except
    end;
    if not Assigned(Node) then
    begin
      Node := NL[i].cloneNode(true);
      RootNode.appendChild(Node);
      continue;
    end;
    for j := 0 to Attributes.length - 1 do
      if not Assigned(Node.attributes.getNamedItem(Attributes[j].nodeName)) then
          (Node as IXMLDOMElement).setAttribute(Attributes[j].nodeName,
            Attributes[j].nodeValue);
    ChildNode := NL[i].firstChild;
    while Assigned(ChildNode) do
    begin
      Node.appendChild(ChildNode.cloneNode(true));
      ChildNode := ChildNode.nextSibling;
    end;
  end;
end;

function GetRealParent(Node: IXMLDOMNode): IXMLDOMNode;
begin
  result := nil;
  repeat
    Node := Node.parentNode;
    if GetStrAttr(Node, 'name', '') <> '' then
      result := Node;
  until Assigned(result) or (Node.nodeName = 'Members');
end;

procedure CopyAttrs(SourceNode: IXmlDomNode; var DestNode: IXmlDomNode);
var
  AttrName, AttrValue: string;
  k: integer;
begin
  for k := 0 to SourceNode.attributes.length - 1 do
  begin
    AttrName := SourceNode.attributes[k].nodeName;
    AttrValue := SourceNode.attributes[k].text;
    (DestNode as IXmlDomElement).setAttribute(AttrName, AttrValue);
  end;
end;

function CopyAttrsToString(SourceNode: IXmlDomNode): string;
var
  AttrName, AttrValue: string;
  k: integer;
begin
  result := '';
  for k := 0 to SourceNode.attributes.length - 1 do
  begin
    AttrName := SourceNode.attributes[k].nodeName;
    AttrValue := SourceNode.attributes[k].text;
    result := result + Format(' %s=%s', [AttrName, AttrValue]);
  end;
end;

procedure CopyAttrsEx(SourceNode: IXmlDomNode; var DestNode: IXmlDomNode;
  ExceptAttrs: array of string);
var
  AttrName, AttrValue: string;
  i, j: integer;
  IsExceptable: boolean;
begin
  for i := 0 to SourceNode.attributes.length - 1 do
  begin
    AttrName := SourceNode.attributes[i].nodeName;
    IsExceptable := false;
    for j := 0 to Length(ExceptAttrs) - 1 do
      if AttrName = ExceptAttrs[j] then
      begin
        IsExceptable := true;
        break;
      end;
    if IsExceptable then
      continue;
    AttrValue := SourceNode.attributes[i].text;
    (DestNode as IXmlDomElement).setAttribute(AttrName, AttrValue);
  end;
end;

procedure CopyAttrsWithConversion(SourceNode, DestNode: IXMLDOMNode; List: TStringList);
var
  i, Index: integer;
  AttrName, AttrValue: string;
begin
  for i := 0 to SourceNode.attributes.length - 1 do
  begin
    AttrName := SourceNode.attributes[i].nodeName;
    Index := List.IndexOfName(AttrName);
    if Index >= 0 then
      AttrName := List.Values[AttrName];
    AttrValue := SourceNode.attributes[i].text;
    (DestNode as IXmlDomElement).setAttribute(AttrName, AttrValue);
  end;
end;

function InitInfluencesNode(DOM: IXMLDOMDocument2): IXMLDOMNode;
var
  Root: IXMLDOMNode;
begin
  result := nil;
  if not Assigned(DOM) then
    exit;
  Root := DOM.selectSingleNode('function_result');
  if not Assigned(Root) then
    exit;
  result := Root.selectSingleNode(ntInfluences);
  if Assigned(result) then
    RemoveChildren(result)
  else
    result := CreateAndAddChild(Root, ntInfluences);
end;

procedure CopyInfluences(DOM: IXMLDOMDocument2);
var
  InfluencesNode, InfluenceNode: IXMLDOMNode;
  InfluenceNL: IXMLDOMNodeList;
  i, influence: integer;
  tNode: IXMLDOMNode;
  UN: string;
begin
  InfluencesNode := InitInfluencesNode(DOM);
  if not Assigned(InfluencesNode) then
    exit;

  InfluenceNL := DOM.selectNodes('function_result/Members//Member[(@influence!=0)]');
  if not Assigned(InfluenceNL) then
    exit;
  for i := 0 to InfluenceNL.length - 1 do
  begin
    tNode := InfluenceNL[i];
    influence := GetIntAttr(tNode, attrInfluence, 0);
    UN := GetStrAttr(tNode, attrUniqueName, '');

    InfluenceNode := CreateAndAddChild(InfluencesNode, ntInfluence);
    SetAttr(InfluenceNode, attrType, influence);
    SetAttr(InfluenceNode, attrUniqueName, UN);
  end;
end;

procedure CopyMembersState(OldMembers, NewMembers: IXMLDOMDocument2;
  Proc: TSetPBarPositionProc);
const
  XPathTemplate = 'function_result/Members//Member[@%s="%s"]';

  {���������� ����� �� �������:
    ���� � ������ � ������ ���� ����� LevelState = 2 (������� �����), ��
    � ����� ���� ���� ��������� ����� ������ �������� �����}
  procedure CheckByLevels;
  var
    MembersNL, NewLevelsNL: IXMLDOMNodeList;
    i, LevelState: integer;
    LevelName, XPath: string;
    LevelNode: IXMLDOMNode;
  begin
    NewLevelsNL := NewMembers.selectNodes('function_result/Levels/Level');
    for i := 0 to NewLevelsNL.length - 1 do
    begin
      LevelName := GetStrAttr(NewLevelsNL[i], attrName, '');
      LevelNode := OldMembers.selectSingleNode(
        Format('function_result/Levels/Level[@name="%s"]', [LevelName]));
      LevelState := IIF(Assigned(LevelNode),
        GetIntAttr(LevelNode, attrLevelState, 1), 0);
      (NewLevelsNL[i] as IXMLDOMElement).setAttribute(attrLevelState, LevelState);
      XPath :='function_result/Members' + Copy(FunnyStr, 1, 2 * (i + 1));
      MembersNL := NewMembers.selectNodes(XPath);
      FillNodeListAttribute(MembersNL, attrChecked,
        IIF(LevelState = 2, 'true', 'false'));
    end;
  end;

  {�������� ������������ ����� �� ������� ���� � �����, � �����
    �������������� ������� �������}
  procedure CheckByMembers;
  var
    OldNL, NewNL: IXMLDOMNodeList;
    i, MaxPos: integer;
    UniqueName, XPath: string;
    Node: IXMLDOMNode;
  begin
    OldNL := OldMembers.selectNodes('//Member[@checked="true"]');
    NewNL := NewMembers.selectNodes('//' + ntMember + '[@name]');
    FillNodeListAttribute(NewNL, attrInfluence, neNone);
    MaxPos := OldNL.length;
    for i := 0 to OldNL.length - 1 do
    begin
      if Assigned(Proc) then
        Proc(i + 1, MaxPos);
      UniqueName := GetStrAttr(OldNL[i], attrUniqueName, '');
      if UniqueName <> '' then
      begin
        EncodeXPathString(UniqueName);
        XPath := Format(XPathTemplate, [attrUniqueName, UniqueName]);
        Node := NewMembers.selectSingleNode(XPath);
        if Assigned(Node) then
          (Node as IXMLDOMElement).setAttribute(attrChecked, 'true');
      end;
    end;
    {��� �������� ����� ����� ������ �� ��������� ������������� �������
      "�������� �� ���������". ����� �� �� ������, ��� ���� ���������� �
      ����� ��������.}
    Node := OldMembers.selectSingleNode(Format('//Member[@%s]', [attrDefaultValue]));
    if Assigned(Node) then
    begin
      UniqueName := GetStrAttr(Node, attrUniqueName, '');
      EncodeXPathString(UniqueName);
      Node := NewMembers.selectSingleNode(Format('//Member[@%s="%s"]',
        [attrUniqueName, UniqueName]));
      if Assigned(Node) then
        SetAttr(Node, attrDefaultValue, 'true');
    end;
  end;

  procedure CopyInfluence;
  var
    OldNL, ChildrenNL: IXMLDOMNodeList;
    i, MaxPos: integer;
    UniqueName, XPath: string;
    Node, NewInfluencesNode: IXMLDOMNode;
    Inf: TNodeInfluence;
    BoolStr: string;
  begin
    NewInfluencesNode := InitInfluencesNode(NewMembers);
    {������ ����������� ��������� ������������ �� ��������� ���������, ������ ��
    ��������� ������}
    OldNL := OldMembers.selectNodes('//Influence[@type!="0"]');

    {����� �����������������, �� ������ ���� �������� ����� � ����, ���
    ����������� ������ ����������� � ��������� ������}
    if OldNL.length = 0 then
    begin
      CopyInfluences(OldMembers);
      OldNL := OldMembers.selectNodes('//Influence[@type!="0"]');
    end;

    MaxPos := OldNL.length;
    for i := 0 to OldNL.length - 1 do
    begin
      {������� �� ������� � ����� DOM ����������� ��������}
      NewInfluencesNode.appendChild(OldNL[i].cloneNode(true));
      if Assigned(Proc) then
        Proc(i + 1, MaxPos);
      UniqueName := GetStrAttr(OldNL[i], attrUniqueName, '');
      if UniqueName <> '' then
      begin
        EncodeXPathString(UniqueName);
        XPath := Format(XPathTemplate, [attrUniqueName, UniqueName]);
        Inf := TNodeInfluence(GetIntAttr(OldNL[i], 'type', 0));
        Node := NewMembers.selectSingleNode(XPath);
        if Assigned(Node) then
        begin
          (Node as IXMLDOMElement).setAttribute(attrInfluence, Inf);
          case Inf of
            neNone: continue;
            neChildren:
              begin
                ChildrenNL :=
                  Node.selectNodes('./*[(@checked="false") and (@influence!="3")]');
                BoolStr := 'true';
              end;
            neDescendants:
              begin
                ChildrenNL :=
                  Node.selectNodes('.//*[(@checked="false") and (@influence!="3")]');
                BoolStr := 'true';
              end;
            neExclude:
              begin
                (Node as IXMLDOMElement).setAttribute(attrChecked, 'false');
                ChildrenNL := Node.selectNodes('.//*');
                BoolStr := 'false';
              end;
          end;
          FillNodeListAttribute(ChildrenNL, attrChecked, BoolStr);
        end;
      end;
    end;
  end;

begin
  if (not Assigned(OldMembers)) or (not Assigned(NewMembers)) then
    exit;
  {������ ��������� ������������ �������}
  CheckByLevels;
  CheckByMembers;
  {������ ������� �������}
  CopyInfluence;
end;

function HasCheckedDescendants(Node: IXMLDOMNode): boolean;
begin
  if Assigned(Node) then
    result := Node.selectNodes('.//Member[@checked="true"]').length > 0
  else
    result := false;
end;

function GetMemberAttrByKey(Dom: IXmlDomDocument2; KeyName, KeyValue,
                            AttrName: string; var IsLeaf: boolean): string;
var
  XPath: string;
  Node: IXmlDomNode;
begin
  result := '';
  if not Assigned(Dom) then
    exit;
  XPath := 'function_result/Members//' + ntMember + '[@' + KeyName + '="' + KeyValue + '"]';
  Node := Dom.selectSingleNode(XPath);
  if not Assigned(Node) then
    exit;
  IsLeaf := not Node.hasChildNodes;
  result := IIF(Assigned(Node), GetStrAttr(Node, AttrName, ''), '');
end;

procedure FillNodeAttributes(var Node: IXMLDOMNode; Attrs: array of string);
var
  i: integer;
begin
  i := 0;
  while i <= High(Attrs) do
  begin
    (Node as IXmlDomElement).setAttribute(Attrs[i], Attrs[i + 1]);
    Inc(i, 2);
  end;
end;

function CreateAndAddChild(ParentElement: IXMLDOMNode; NodeName: string): IXMLDOMNode;
begin
  result := ParentElement.ownerDocument.createNode(1, NodeName, '');
  ParentElement.appendChild(result);
end;

function CreateWithAttributes(ParentDom: IXmlDomDocument2; NodeName: string; Attrs: array of string): IXMLDOMNode;
begin
  result := ParentDom.createNode(1, NodeName, '');
  FillNodeAttributes(result, Attrs);
end;

function CreateAndAddWithAttributes(ParentElement: IXMLDOMNode; NodeName: string; Attrs: array of string): IXMLDOMNode;
begin
  result := CreateAndAddChild(ParentElement, NodeName);
  FillNodeAttributes(result, Attrs);
end;

function GetMDXEnumeration(MembersDOM: IXMLDOMDocument2;
  Optimization: TMdxOptimization): string;

  function GetAxisPartialMdx(Node: IXMLDOMNode;
    ParentInfluence: TNodeInfluence): string;
  const
    FuncStr = 'Descendants(%s, %d, %s)';
  var
    NL: IXMLDOMNodeList;
    i: integer;
    MemberName, PartialMdx, Flag: string;
    Influence: TNodeInfluence;
    NeedToDrill, AllChildrenChecked, AllDescendantsChecked, Checked: boolean;
  begin
    result := '';
    Influence := TNodeInfluence(GetIntAttr(Node, attrInfluence, 0));
    {���� ���� �������� �� ������������, �� �� �� ���, �� ��� ������� ��
    �������� � ����� ������� - ����� ����� ��������}
    if Influence = neExclude then
      exit;

    {���� ������� ��� ����/��� �������, �� ������� ������� �������
      ��������������� ������� ��� ����������� �������}
    AllChildrenChecked := GetBoolAttr(Node, attrAllChildrenChecked, false);
    if AllChildrenChecked then
      Influence := neChildren;
    AllDescendantsChecked := GetBoolAttr(Node, attrAllDescendantsChecked, false);
    if AllDescendantsChecked then
      Influence := neDescendants;

    {����� �� ���������� ���� �� �������� -
      ���� ���� � ���������� � ������������ �������}
    NeedToDrill := HasCheckedDescendants(Node) and
      not (AllChildrenChecked and AllDescendantsChecked);
    MemberName := GetStrAttr(Node, attrUniqueName, '');
    Checked := GetBoolAttr(Node, attrChecked, false);
    case Influence of
      neNone:
      {���� ������� ���� ��������� ������� �����, �� ������� �������� ���������
        �� ���� - ��� ��� ������}
        if ParentInfluence <> neChildren then
          if ((Optimization = moServer2) and Checked) or (Optimization <> moServer2) then
        begin
          AddTail(result, ',');
          result := result + MemberName;
        end;
      neChildren:
        begin
        {���� ������� ���� ��������� ������� �����, �� ������� ��������
          ��������� �� ���� - ��� ��� ������}
          if ParentInfluence <> neChildren then
          if ((Optimization = moServer2) and Checked) or (Optimization <> moServer2) then
          begin
            AddTail(result, ',');
            result := result + MemberName;
          end;
          AddTail(result, ',');
          result := result + MemberName + '.Children';
        end;
      neDescendants:
        begin
          AddTail(result, ',');
          if (ParentInfluence = neChildren) or
            ((Optimization = moServer2) and not Checked) then
              Flag := 'AFTER'
          else
            Flag := 'SELF_AND_AFTER';
          (*Flag := IIF(ParentInfluence = neChildren, 'AFTER', 'SELF_AND_AFTER');*)
          result := result + Format(FuncStr, [MemberName, 0, Flag]);
          NeedToDrill := false;
        end;
    end;
    {���� �����, �� ���� �� ��������}
    if NeedToDrill then
    begin
      NL := Node.childNodes;
      for i := 0 to NL.length - 1 do
      begin
        PartialMdx := GetAxisPartialMdx(NL[i], Influence);
        if PartialMdx <> '' then
        begin
          AddTail(result, ',');
          result := result + PartialMdx;
        end;
      end;
    end;
  end;

  function GetFilterPartialMdx(Node: IXMLDOMNode(*; var ExceptionSet: string*)): string;
  var
    NL: IXMLDOMNodeList;
    i: integer;
    MemberName, PartialMdx: string;
    Influence: TNodeInfluence;
    Checked: boolean;
//    ExceptionSet: string;
  begin
    result := '';
    Influence := TNodeInfluence(GetIntAttr(Node, attrInfluence, 0));
    {���� ���� �������� �� ������������, �� �� �� ���, �� ��� ������� ��
    �������� � ����� ������� - ����� ����� ��������}
    if Influence = neExclude then
      exit;
    MemberName := GetStrAttr(Node, attrUniqueName, '');
    {���� ������� ������� �������, �� ��� ��� �� ������� ��������� ��� ��������}
    Checked := (Optimization = moServer) or GetBoolAttr(Node, attrChecked, false);
    if Checked then
    begin
      AddTail(result, ',');
      result := result + MemberName;
      if Optimization = moFilter then
      begin
        {������ ��������� ���������� ���������� - Redmine Feature #17930}
(*        if (Influence = neChildren) or (Influence = neDescendants) then
        begin
          NL := Node.selectNodes(Format('Member[@%s="%d"]', [attrInfluence, Ord(neExclude)]));
          for i := 0 to NL.length - 1 do
          begin
            AddTail(ExceptionSet, ', ');
            ExceptionSet := ExceptionSet + GetStrAttr(Node, attrUniqueName, '');
          end;
          ExceptionSet := Format('{Except({%s.Children)}, {%s}}', [MemberName, ExceptionSet]);
        end;*)
        exit;
      end;
    end;
    {���� �����, �� ���� �� ��������}
    if HasCheckedDescendants(Node) then
    begin
      NL := Node.childNodes;
      for i := 0 to NL.length - 1 do
      begin
        PartialMdx := GetFilterPartialMdx(NL[i]);
        if PartialMdx <> '' then
        begin
          AddTail(result, ',');
          result := result + PartialMdx;
        end;
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
  i: integer;
  PartialMdx: string;
begin
  result := '';
  if not Assigned(MembersDOM) then
    exit;
  NL := MembersDOM.selectNodes('function_result/Members/Member[@name]');
  if not Assigned(NL) then
    exit;
  for i := 0 to NL.Length - 1 do
  begin
    if (Optimization = moAxis) or (Optimization = moServer2) then
      PartialMdx := GetAxisPartialMdx(NL[i], neNone)
    else
      PartialMdx := GetFilterPartialMdx(NL[i]);

    if PartialMdx <> '' then
    begin
      AddTail(result, ',');
      result := result + PartialMdx;
    end;
  end;
end;

function MDXEnumerationToMembersDescription(MDXEnum: string): string;
var
  Index1, Index2: integer;
  OldStr, NewStr: string;
begin
  {�������� �������� ������ �� ��������� ��������� �������}
  result := StringReplace(MDXEnum, '],[', ']' + FSD + '[', [rfReplaceAll]);
  {��� �������� ___].DATAMEMBER,[___ }
  result := StringReplace(result, 'R,[', 'R' + FSD + '[', [rfReplaceAll]);
  result := StringReplace(result, ',Descendants(', ',' + FSD + 'Descendants(', [rfReplaceAll]);
  result := StringReplace(result, '].Children,[', '].Children' + FSD + '[', [rfReplaceAll]);

  {������� ��� ��������� ������� Descendants}
  Index1 := Pos('Descendants(', result);
  while Index1 > 0 do
  begin
    Index2 := Pos('AFTER)', result);
    OldStr := Copy(result, Index1, Index2 + 6 - Index1);
    NewStr := OldStr;
    Delete(NewStr, 1, 12);
    if Pos('SELF_', OldStr) > 0 then
      NewStr := '������� ' + NewStr + ' � ��� ��� �������'
    else
      NewStr := '��� ������� �������� ' + NewStr;
    Index1 := Pos(', 0, ', NewStr);
    if Index1 = 0 then
      Index1 := Pos(', 1, ', NewStr);
    Index2 := Pos('AFTER)', NewStr) + 6;
    Delete(NewStr, Index1, Index2 - Index1);
    result := StringReplace(result, OldStr, NewStr, []);
    Index1 := Pos('Descendants(', result);
  end;

  {������� ��� ��������� ������� Children}
  Index2 := Pos('].Children', result);
  while Index2 > 0 do
  begin
    Index1 := Index2 - 1;
    repeat
      Dec(Index1);
    until (result[Index1] = #10) and (result[Index1 + 3] = '[') or (Index1 = 0);
    Delete(result, Index2 + 1, 9);
    Insert('�������� �������� �������� ', result, Index1 + 1);
    Index2 := Pos('].Children', result);
  end;

  {������� ��� ��������� ��������� DATAMEMBER}
  Index1 := Pos('].DATAMEMBER', result);
  while Index1 > 0 do
  begin
    Index2 := Index1;
    repeat
      Dec(Index2);
    until (result[Index2] + result[Index2 + 1] = '.[') or (Index2 = 1);
    if Index2 <> 1 then
    begin
      result[Index2 + 1] := '(';
      Delete(result, Index1, 12);
      Insert(' ������)', result, Index1);
    end;
    Index1 := Pos('].DATAMEMBER', result);
  end;
  result := StringReplace(result, '.DATAMEMBER', '.������', [rfReplaceAll]);
end;

function GetMembersDescription(MembersDOM: IXMLDOMDocument2; Optimization: TMdxOptimization): string;
begin
  result := MDXEnumerationToMembersDescription(GetMDXEnumeration(MembersDOM, Optimization));
end;

function GetMembersDescriptionLikeTree(MembersDOM: IXMLDOMDocument2): string;
const
  sNewLine = #13#10;
  sTab = '        ';

  {���������� ������ ��������������� �������}
  function GetCurTab(Depth: integer): string;
  var
    i: integer;
  begin
    result := '';
    for i := 1 to Depth do
      result := result + sTab;
  end;

  {���������� �������� �������� �����}
  procedure BuildChildrenDescr(var Src: string; Node: IXMLDOMNode; Depth: integer);
  var
    NL: IXMLDOMNodeList;
    i, CurInfluence: integer;
    MName, CurTab: string;
    CurChacked: boolean;
  begin
    if Assigned(Node) then
    begin
      NL := Node.selectNodes('./Member');
      CurTab := GetCurTab(Depth);
      for i := 0 to NL.length - 1 do
      begin
        MName := GetStrAttr(NL[i], 'name', '');
        CurInfluence := GetIntAttr(NL[i], attrInfluence, 3);
        CurChacked := GetBoolAttr(NL[i], attrChecked, false);
        if MName <> '' then
        begin
          if CurChacked or (CurInfluence = 1) or (CurInfluence = 2) then
          begin
            {���� ���� ���� ��������}
            Src := Src + IIF(Src <> '', sNewLine, '');
            Src := Src + CurTab + MName;

            BuildChildrenDescr(Src, NL[i], Depth + 1);
          end
          else //�������� ����
            BuildChildrenDescr(Src, NL[i], Depth);
        end;
      end;
    end;
  end;
var
  RootNode: IXMLDOMNode;
begin
  result := '';
  if Assigned(MembersDOM) then
  begin
    RootNode := MembersDOM.selectSingleNode('//Members');
    BuildChildrenDescr(result, RootNode, 0);
  end;
end;

procedure FilterMembersDomEx(Dom: IXMLDOMDocument2);

  procedure FilterMembers(OldRoot, NewRoot: IXMLDOMNode; LevelIndex: integer);
  var
    ChildrenNL, GrandChildrenNL: IXMLDOMNodeList;
    i, j: integer;
    Node, LevelNode: IXMLDOMNode;
  begin
    if not Assigned(OldRoot) then
      exit;
    if not OldRoot.hasChildNodes then
      exit;
    ChildrenNL := OldRoot.childNodes;
    LevelNode := GetLevelNodeByIndex(Dom, LevelIndex);
    if GetIntAttr(LevelNode, attrLevelState, 0) = 0 then
      for i := 0 to ChildrenNL.length - 1 do
      begin
        FilterMembers(ChildrenNL[0], NewRoot , LevelIndex + 1);
        if ChildrenNL[0].hasChildNodes then
        begin
          GrandChildrenNL := ChildrenNL[0].childNodes;
          for j := 0 to GrandChildrenNL.length - 1 do
          begin
            Node := GrandChildrenNL[j].cloneNode(true);
            NewRoot.appendChild(Node);
          end;
        end;
        //  �������
        Node := OldRoot.removeChild(ChildrenNL[0]);
        Node := nil;
      end
    else
      if OldRoot.hasChildNodes then
        for i := 0 to ChildrenNL.length - 1 do
          FilterMembers(ChildrenNL[i], ChildrenNL[i], LevelIndex + 1);
  end;

  procedure FilterLevels(Root: IXMLDOMNode);
  var
    ChildrenNL: IXMLDOMNodeList;
    i: integer;
  begin
    if not Assigned(Root) then
      exit;
    ChildrenNL := Root.childNodes;
    for i := ChildrenNL.length - 1 downto 0 do
      if GetIntAttr(ChildrenNL[i], attrLevelState, 0) = 0 then
        Root.removeChild(ChildrenNL[i]);
  end;
var
  Root: IXMLDOMNode;
begin
  if not Assigned(DOM) then
    exit;
  Root := DOM.selectSingleNode('function_result/Members');
  FilterMembers(Root, Root, 0);
  Root := DOM.selectSingleNode('function_result/Levels');
  FilterLevels(Root);
end;

function GetLevelNodeByIndex(Dom: IXMLDomDocument2; LevelIndex: integer): IXMLDOMNode;
begin
  try
    result := Dom.selectSingleNode('function_result/Levels').childNodes[LevelIndex];
  except
    result := nil;
  end;
end;

procedure MakeDefaultMembersDom(var Members: IXmlDomDocument2);
var
  XPath: string;
  i: integer;
  Nodes: IXMLDOMNodeList;
begin
  if not Assigned(Members) then
    exit;
  XPath := 'function_result/Members//';
  Nodes := Members.SelectNodes(XPath);
  for i := 0 to Nodes.length - 1 do
  begin
    (Nodes[i] as IXMLDOMElement).setAttribute('checked', 'false');
    (Nodes[i] as IXMLDOMElement).setAttribute('influence', '0');
    (Nodes[i] as IXMLDOMElement).setAttribute('underinfluence', '0');
  end;
end;

function IsWritebackSenseless(Node: IXMLDOMNode): boolean;
begin
  result := IsWritebackSenseless(GetStrAttr(Node, attrRangeName, '_false_'));
end;

function IsWritebackSenseless(RangeName: string): boolean;
begin
  result := (Pos('_true_', RangeName) = 0);
end;

function IsGrandSummary(Node: IXMLDOMNode): boolean;
begin
  result := (Node.parentNode.nodeName = 'Members') and
    (GetNodeType(Node) = ntSummary);
end;

procedure GetIntArrayAttr(Node: IXMLDOMNode; AttributeName: string;
  var ResultArray: TIntegerArray);
var
  tmpStr, Part: string;
  Len: integer;
begin
  SetLength(ResultArray, 0);
  if not Assigned(Node) then
    exit;
  tmpStr := GetStrAttr(Node, AttributeName, '');
  if tmpStr = '' then
    exit;
  Len := 0;
  Part := CutPart(tmpStr, ',');
  while Part <> '' do
  begin
    inc(Len);
    SetLength(ResultArray, Len);
    ResultArray[Len - 1] := StrToInt(Part);
    Part := CutPart(tmpStr, ',');
  end;
end;

procedure SetIntArrayAttr(Node: IXMLDOMNode; AttributeName: string;
  ResultArray: TIntegerArray);
var
  i: integer;
  tmpStr: string;
begin
  if not Assigned(Node) then
    exit;
  tmpStr := '';
  for i := 0 to Length(ResultArray) - 1 do
  begin
    AddTail(tmpStr, ',');
    tmpStr := tmpStr + IntToStr(ResultArray[i]);
  end;
  (Node as IXMLDOMElement).setAttribute(AttributeName, tmpStr);
end;


{���������� ����� ���� � ������ ChildName, ������������ ��������������� Node}
function GetChildText(Node: IXMLDOMNode; ChildName: string): string;
begin
  result := '';
  try
    result := Node.selectSingleNode(ChildName).text;
  except
  end;
end;

{UniqueName ��������}
function MemberUName(Node: IXMLDOMNode): string;
begin
  result := GetChildText(Node, 'UName');
end;

{LNum �������� }
function MemberLNum(Node: IXMLDOMNode): integer;
var
  LStr: string;
begin
  LStr := GetChildText(Node, 'LNum');
  try
    result := StrToInt(LStr);
  except
    result := -1;
  end;
end;


{UniqueName ������ �������}
function MemberLName(Node: IXMLDOMNode): string;
begin
  result := GetChildText(Node, 'LName');
end;

{������ � ������ "All"}
function MemberFromAllLevel(Node: IXMLDOMNode): boolean;
begin
  result := (Pos('.[(All)]', MemberLName(Node)) > 0);
end;

{����� �������}
function MemberCaption(Node: IXMLDOMNode): string;
begin
  result := GetChildText(Node, 'Caption');
end;

{������� �������}
function MemberOrdinal(Node: IXMLDOMNode): string;
begin
  {!!! ��������� �������.
   ������ �������� Member_Ordinal ��������� �� ���������� ��������� ����
   ��� �� ���������. ����� ����������.
   ��� ����� ����� ���� ������. �������� MEMBER_ORDINAL ��� MEMBER_ORDINAL_13 � �.�
   ��� ��� ����� �� ���� ����� ������� ���������. ��� �� ����� �����, ��������������
   ��� �������� ����, � ����� ����� �� ����, ��� ��� ���������� � "MEMBER_ORDINAL"}
  result := '';
  if Assigned(Node) then
    result := Node.lastChild.text;
end;

function GetAxisTuples(DOM: IXMLDOMDocument2; AxType: TAxisType): IXMLDOMNodeList;
begin
  if Assigned(DOM) then
  begin
    if AxType = axColumn then
      result := DOM.selectNodes('root/Axes/Axis[@name="Axis0"]/Tuples/Tuple')
    else
      result := DOM.selectNodes('root/Axes/Axis[@name="Axis1"]/Tuples/Tuple');
  end;
end;

//!!!! ������������
function GetAxisVisibleTuples(DOM: IXMLDOMDocument2; AxType: TAxisType): IXMLDOMNodeList;
begin
  if Assigned(DOM) then
  begin
    if AxType = axColumn then
      result := DOM.selectNodes('root/Axes/Axis[@name="Axis0"]/Tuples/Tuple[not (@hidden)]')
    else
      result := DOM.selectNodes('root/Axes/Axis[@name="Axis1"]/Tuples/Tuple[not (@hidden)]');
  end;
end;

{ ���������� ��� ������� �� ���������� �� "Count" ������ ��������,
  ���������� ������ �������, � ������� ������� ��������;
  -1 �������� ������������ ��������.}
function CompareTuples(Tuple1, Tuple2: IXMLDOMNode; Count: integer): integer;
var
  i: integer;
  UName1, UName2: string;
begin
  result := -1;
  if Count > Tuple1.childNodes.length then
    Count := Tuple1.childNodes.length;
  for i := 0 to Count - 1 do
  begin
    UName1 := MemberUName(Tuple1.childNodes[i]);
    UName2 := MemberUName(Tuple2.childNodes[i]);
    if UName1 <> UName2 then
    begin
      result := i;
      exit;
    end;
  end;
end;

function IsItSummary(Node: IXMLDOMNode): boolean;
begin
  if Assigned(Node) then
    result := Assigned(Node.Attributes.GetNamedItem(attrIsItSummary))
  else
    result := false;
end;

procedure DivideMembersDom(var SourceDom, PartDom: IXMLDOMDocument2;
  LeafCountLimit: integer; BrokenAxis: boolean);
const
  attrMovable = 'movable';

  function CheckLeafCount(Node: IXMLDOMNode; var LeafCount: integer; const Limit: integer): boolean;
  var
    i: integer;
  begin
    result := false;

    if Node.selectNodes('./Member | ./Summary').length = 0 then
    begin
      inc(LeafCount);
      SetAttr(Node, attrMovable, 'true');
      exit;
    end;

    for i := 0 to Node.childNodes.length - 1 do
    begin
      if LeafCount = Limit then
      begin
        result := true;
        exit;
      end;
      result := result or CheckLeafCount(Node.childNodes[i], LeafCount, Limit);
    end;
    // ���������� �� ���� ����� - �������� ���� �� �������
    if Node.selectNodes('./*[@movable]').length = Node.childNodes.length then
      SetAttr(Node, attrMovable, 'true');
  end;

  procedure ClearMovableAttrs(Dom: IXMLDOMDocument2);
  var
    NL: IXMLDOMNodeList;
    i: integer;
  begin
    NL := Dom.selectNodes('//*[@movable="true"]');
    for i := 0 to NL.length - 1 do
      NL[i].attributes.removeNamedItem('movable');
  end;

  procedure DeleteByCondition(Dom: IXMLDOMDocument2; Condition: string);
  var
    NL: IXMLDOMNodeList;
    i: integer;
    Parent: IXMLDOMNode;
  begin
    NL := Dom.selectNodes(Condition);
    for i := NL.length - 1 downto 0 do
    begin
      Parent := NL[i].parentNode;
      if Assigned(Parent) then
        Parent.removeChild(NL[i]);
    end;
  end;

var
  Root: IXMLDOMNode;
  LeafCount: integer;
begin
  {��� ������ �������, ����� ���� ���� ��������� � ����� ���.}
  Root := SourceDom.selectSingleNode('function_result/Members');
  LeafCount := 0;
  if CheckLeafCount(Root, LeafCount, LeafCountLimit) then
  begin
    if not Assigned(PartDom) then
      GetDomDocument(PartDom);
    PartDom.load(SourceDom);  
    if BrokenAxis then
    begin
      DeleteByCondition(PartDom, 'function_result/Members/*[not(@movable)]');
      DeleteByCondition(SourceDom, 'function_result/Members/*[@movable]');
    end
    else
    begin
      DeleteByCondition(PartDom, 'function_result/Members//*[not(@movable) and not(.//*[@movable])]');
      DeleteByCondition(SourceDom, 'function_result/Members//*[@movable]');
    end;
  end
  else
  begin
    ClearMovableAttrs(SourceDom);
    KillDOMDocument(PartDom);
  end;
end;

(*procedure SetAxisNodesCoords(var Dom: IXMLDOMDocument2; AxisType: TAxisType;
  StartFrom, Step: integer);

  procedure SetAxisNodeCoords(Node: IXMLDOMNode; var x: integer);
  var
    i: integer;
    NL: IXMLDOMNodeList;
  begin
    if AxisType = axRow then
      SetAttr(Node, 'rind', x)
    else
      SetAttr(Node, 'cind', x);

    NL := Node.selectNodes('Member');
    for i := 0 to NL.length - 1 do
    begin
      if i > 0 then
        inc(x, Step);
      SetAxisNodeCoords(NL[i], x);
    end;
  end;

var
  x: integer;
  Root: IXMLDOMNode;
begin
  if not Assigned(Dom) then
    exit;
  x := StartFrom;
  Root := Dom.selectSingleNode('function_result/Members');
  SetAxisNodeCoords(Root, x);
end;  *)

(*procedure EnlightAxisDom(var Dom: IXMLDOMDocument2; AxisType: TAxisType);
var
  i, j: integer;
  MembersNL: IXMLDOMNodeList;
  AttrNode: IXMLDOMNode;
  AttrName: string;
begin
  if not Assigned(Dom) then
    exit;
  MembersNL := Dom.selectNodes('function_result/Members//Member');
  for i := 0 to MembersNL.length - 1 do
    for j := MembersNL[i].attributes.length - 1 downto 0 do
    begin
      AttrName := MembersNL[i].attributes[j].nodeName;
      if (AttrName = attrUniqueName) or (AttrName = attrPkid) or
        (AttrName = IIF(AxisType = axRow, 'rind', 'cind')) then
        continue;
      MembersNL[i].attributes.removeNamedItem(AttrName);
    end;
end;*)

function ReadTaskContextAttr(Node: IXMLDOMNode; AttrName1, AttrName2: string): string;
var
  AttrNode: IXMLDOMNode;
begin
  result := '';
  if not Assigned(Node) then
    exit;
  AttrNode := Node.selectSingleNode(AttrName1);
  if not Assigned(AttrNode) then
    AttrNode := Node.selectSingleNode(AttrName2);
  if Assigned(AttrNode) then
    result := AttrNode.text;
end;

procedure RemoveNodes(Dom: IXMLDOMDocument2; XPath: string);
var
  i: integer;
  NL: IXMLDOMNodeList;
  PrntNode: IXMLDOMNode;
begin
  NL := Dom.selectNodes(XPath);
  for i := 0 to NL.length - 1 do
  begin
    PrntNode := NL[i].parentNode;
    PrntNode.removeChild(NL[i]);
  end;
end;

procedure DebugSaveNodeList(NL: IXMLDOMNodeList; Path: string);
var
  F: TextFile;
  NodeIndex, AttrIndex: integer;
  s: string;
begin
  AssignFile(F, Path);
  Rewrite(F);
  try
    for NodeIndex := 0 to NL.length - 1 do
    begin
      s := NL[NodeIndex].nodeName;
      WriteLn(F, s);
      for AttrIndex := 0 to NL[NodeIndex].attributes.length - 1 do
        WriteLn(F, Format('        %s = %s', [NL[NodeIndex].attributes[AttrIndex].nodeName,
          NL[NodeIndex].attributes[AttrIndex].nodeValue]));
    end;
  finally
    CloseFile(F);
  end;
end;

procedure DebugSaveDom(Dom: IXMLDOMDocument2; Path: string);
begin
  Dom.save(Path);
end;

end.



