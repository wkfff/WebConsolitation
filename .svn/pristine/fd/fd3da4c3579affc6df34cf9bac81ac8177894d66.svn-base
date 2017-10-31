{
  Функции для работы с шаблоном отчета MDX Эксперт. (файлами EXD)
}
unit uEXDUtils;

interface
uses
  uXMLUtils, MSXML2_TLB;

type
  {Выды элементов эксперта}
  TEXDElementType = (eltTable, eltChart, eltMap, eltNote, eltSpreedsheet);
  TEXDElementTypes = set of TEXDElementType;


  {Передается узел элемента (таблицы или диаграммы).
   Возвращается DOM с сохранкой OWC для этого элемента}
  function GetElementOWCDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
  function GetEXD3ElementDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
  {Передается сохранка OWC.PivotChart, возвращается сохранка внутренней
  таблицы OWC.PivotTable, которая служит для диаграммы источником}
  function ExtractOWCInnerTableDOM(ChartDOM: IXMLDOMDocument2): IXMLDOMDocument2;
  {Из узла элемента вытаскивается OWC-сохранка таблицы.
   В случае если элемент имеет тип "диаграмма", то вытаскивается внутреняя таблица}
  function GetElementOWCSourceDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;

  function GetEXD3ElementPropertiesDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
  {Достает из узла элемента его заголовок (не название элемента,
   а именно заголовок в формате плоского текста.
   Т.е храниться-то там RTF, поэтому достанет прямо с разметкой}
  function GetElemCaption(DataNode: IXMLDOMNode): string;
  {Достает ияз узла элементов его коментарий в формате RTF}
  function GetElemComment(DataNode: IXMLDOMNode): string;
  {Возвращает имя куба отчета}
  function GetEXDCubeName(TemplateDOM: IXMLDOMDocument2): string;
  function GetEXD3CubeName(TemplateDOM: IXMLDOMDocument2): string;
  {Возвращает идентификатор элемента. На вход подается узел элемента}
  function GetElementGUID(DataNode: IXMLDOMNode): string;
  {Достает из шаблона узлы с элементами. Параметром передается шаблон
   и фильтр на тип элементов. Если фильтр пустой, будут выбраны элементы
   всех типов}
  function EXDElementNodes(TamplateDOM: IXMLDOMDocument2;
    ElemTypes: TEXDElementTypes): IXMLDOMNodeList;

  {Достает из шаблона эксперта 3 узлы с элементами.}
  function EXD3ElementNodes(TemplateDOM: IXMLDOMDocument2): IXMLDOMNodeList;

implementation

function GetElementOWCDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
begin
  result := nil;
  if not Assigned(ElemNode) then
    exit;

  GetDOMDocument(result);
  if not result.loadXML(ElemNode.Text) then
    exit;

  if result.selectSingleNode('//oleobj/xml') <> nil then
    result.LoadXML(result.selectSingleNode('//oleobj/xml').xml);
end;

function GetEXD3ElementDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
begin
  result := nil;
  if not Assigned(ElemNode) then
    exit;

  GetDOMDocument(result);
  if not result.loadXML(ElemNode.Text) then
    exit;

  if result.selectSingleNode('//ReportElementNode') <> nil then
    result.LoadXML(result.selectSingleNode('//ReportElementNode').xml);
end;


function ExtractOWCInnerTableDOM(ChartDOM: IXMLDOMDocument2): IXMLDOMDocument2;
var
  InnerNode, TypeNode: IXMLDOMNode;
  SourceType: string;
begin
  result := nil;
  if Assigned(ChartDOM) then
  begin
    InnerNode := ChartDOM.documentElement.selectSingleNode('//x:DataSource');
    TypeNode := InnerNode.selectSingleNode('//x:Type');
    if Assigned(TypeNode) then
    begin
      SourceType := TypeNode.text;
      if (SourceType = 'InternalPivotList') then
      begin
        GetDomDocument(result);
        if not result.LoadXml(InnerNode.selectSingleNode('x:PivotListXML').Text) then
        begin
          KillDOMDocument(result);
          result := nil;
        end;
      end;
    end;
  end;
end;

function GetElementOWCSourceDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
var
  ElemType: string;
  InnerDOM: IXMLDOMDocument2;
  SrcTableID: string;
  SrcTableNode: IXMLDOMNode;
  XPath: string;
begin
  result := GetElementOWCDOM(ElemNode);

  ElemType := GetStrAttr(ElemNode, 'type', '');
  if ElemType='chart' then
  begin
    InnerDOM := ExtractOWCInnerTableDOM(result);
    KillDOMDocument(result);
    if Assigned(InnerDOM) then
      result := InnerDOM
    else
    begin
      {Не смогли вытащить внутреннюю таблицу из даграммы.
       Последний шанс - диаграмма строиться по таблице. Попробуем вытащить}
      SrcTableID := GetStrAttr(ElemNode, 'boundto', '');
      if SrcTableID <> '' then
      begin
        XPath := '//data[@type="crosstab" and @guid="' + SrcTableID + '"]';
        SrcTableNode := ElemNode.ownerDocument.selectSingleNode(XPath);
        if Assigned(SrcTableNode) then
          result := GetElementOWCDOM(SrcTableNode);
      end;
    end;
  end;

end;

function GetEXD3ElementPropertiesDOM(ElemNode: IXMLDOMNode): IXMLDOMDocument2;
begin
  result := nil;
  if not Assigned(ElemNode) then
    exit;

  GetDOMDocument(result);
  result.LoadXML(ElemNode.xml);
end;



function GetElemCaption(DataNode: IXMLDOMNode): string;
var
  tmpDOM: IXMLDOMDocument2;
  DOMNode: IXMLDOMNode;
begin
  if not Assigned(DataNode) then
    exit;

  GetDOMDocument(tmpDOM);
  if not tmpDOM.loadXML(DataNode.Text) then
    exit;

  DOMNode := tmpDOM.selectSingleNode('xml/title[@text]');
  result := GetStrAttr(DOMNode, 'text', '');
  KillDOMDocument(tmpDOM);
end;

function GetElemComment(DataNode: IXMLDOMNode): string;
var
  tmpDOM: IXMLDOMDocument2;
  DOMNode: IXMLDOMNode;
begin
  result := '';
  if not Assigned(DataNode) then
    exit;

  GetDOMDocument(tmpDOM);
  if not tmpDOM.loadXML(DataNode.Text) then
    exit;

  DOMNode := tmpDOM.selectSingleNode('xml/comment[@text]');

  if GetIntAttr(DOMNode, 'visible', 0) > 0 then
    result := GetStrAttr(DOMNode, 'text', '');

  KillDOMDocument(tmpDOM);
end;


function GetEXDCubeName(TemplateDOM: IXMLDOMDocument2): string;
var
  SourceNode: IXMLDOMNode;
begin
  result := '';
  if Assigned(TemplateDOM) then
  begin
    SourceNode := TemplateDOM.selectSingleNode('//root/datasource');
    result := GetStrAttr(SourceNode, 'cube', '');
  end;
end;

function GetEXD3CubeName(TemplateDOM: IXMLDOMDocument2): string;
var
  SourceNode: IXMLDOMNode;
begin
  result := '';
  if Assigned(TemplateDOM) then
  begin
    SourceNode := TemplateDOM.selectSingleNode('//ReportElementNode/Properties');
    result := GetStrAttr(SourceNode, 'cubeName', '');
  end;
end;


function GetElementGUID(DataNode: IXMLDOMNode): string;
begin
  result := '';
  if Assigned(DataNode) then
    result := GetStrAttr(DataNode, 'guid', '');
end;

function EXDElementNodes(TamplateDOM: IXMLDOMDocument2;
  ElemTypes: TEXDElementTypes): IXMLDOMNodeList;
  {!!!!!!!!!!}
  function AddTail(Src, Separator, Tail: string): string;
  begin
    if Src = '' then
      result := Tail
    else
      result := Src + Separator + Tail;
  end;
var
  sFilter: string;
begin
  result := nil;
  sFilter := '';
  if Assigned(TamplateDOM) then
  begin
    if eltTable in ElemTypes then
      sFilter := AddTail(sFilter, ' or ', '@type="crosstab"');

    if eltChart in ElemTypes then
      sFilter := AddTail(sFilter, ' or ', '@type="chart"');

    if eltMap in ElemTypes then
      sFilter := AddTail(sFilter, ' or ', '@type="map"');

    if eltNote in ElemTypes then
      sFilter := AddTail(sFilter, ' or ', '@type="note"');

    if eltSpreedsheet in ElemTypes then
      sFilter := AddTail(sFilter, ' or ', '@type="spreadsheet"');

    if sFilter = '' then
      sFilter := '@type';
                                                                   
    result := TamplateDOM.selectNodes('//data[' + sFilter + ']');
  end;
end;

function EXD3ElementNodes(TemplateDOM: IXMLDOMDocument2): IXMLDOMNodeList;
begin
  result := nil;
  if Assigned(TemplateDOM) then
    result := TemplateDOM.selectNodes('//ReportElementNode');

end;


end.
