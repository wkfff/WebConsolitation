unit uXMLUtils;

interface

uses sysutils, MSXML2_TLB, comobj, classes, windows;

function  GetDomDocument(out ipDomDocument : IXMLDomDocument2) : boolean;
procedure  KillDOMDocument(var DOM: IXMLDomDocument2);

// добавление заголовка
procedure AddProcInstruction(var XmlDoc: IXmlDomDocument2);
// создание документа с заголовком
function InitXmlDocument: IXmlDomDocument2;

function GetAttr       (Node: IXMLDOMNode; Name: widestring): widestring;
function CheckAttr     (Node: IXMLDOMNode; Name: widestring; var wideStr: widestring): boolean;
function GetStrAttr    (Node: IXMLDOMNode; Name: widestring; Def: widestring): widestring;
function GetBoolAttr   (Node: IXMLDOMNode; Name: widestring; Def: boolean): boolean;
function GetIntAttr    (Node: IXMLDOMNode; Name: widestring; Def: Integer): Integer;
procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: widestring); overload;
procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: integer); overload;
procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: boolean); overload;
procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: extended); overload;
{То же что GetStrAttr, только с перекодировкой спецсимволов XMLDencode}
function GetDecodedStrAttr(Node: IXMLDOMNode; Name: widestring; Def: widestring): widestring;
{Устанавливает аттрибут с перекодировкой}
procedure SetEncodedStrAttr(Node: IXMLDOMNode; Attr, Value: string);



{Две станные процедурки, надо-бы их упразднить, привести к стандартным.
 (используются в плагине)}
function GetNodeStrAttr(Node: IXMLDOMNode; Attr: string): string;
procedure SetNodeStrAttr(Node: IXMLDOMNode; Attr, Value: string);


function AppendElement (Node: IXMLDOMNode; ElemName: WideString): IXMLDOMNode;
function InsertElement (Node: IXMLDOMNode; ElemName: WideString; ind: integer): IXMLDOMNode;
function AppendText    (Node: IXMLDOMNode; ElemText: WideString): IXMLDOMNode;
function AppendCDATA   (Node: IXMLDOMNode; ElemText: WideString): IXMLDOMNode;
function AppendNamedItem(ipNode : IXMLDOMNode; ItemName : widestring; ItemText : widestring) : IXMLDOMNode;
function AppendBoolNamedItem(ipNode : IXMLDOMNode; ItemName : widestring; ItemVal : boolean) : IXMLDOMNode;
function SaveFormattedXMLDocumentToStream(const DomDoc: IXMLdomDocument2; tmpStream: TFileStream): boolean;
function SaveFormattedXMLDocument(const DomDoc: IXMLdomDocument2; const FileName : string): boolean;
{Принудительно, но реально форматирует перед сохранением}
procedure FormatXMLDocument(const XML: IXMLDOMDocument2);
function AppendElemWithAttr(const Node : IXMLDOMNode; const ElemName : WideString;
  const AttrName : WideString; const AttrVal : OleVariant): IXMLDOMNode;

procedure InsertText(elem : IXMLDOMElement);

procedure WaitForDOMLoad(const ipDOM : IXMLDomDocument2);

{Заменяем спецсимволы сущностями}
procedure XMLEncode(var Src: String);
{Заменяет сущности символами}
procedure XMLDecode(var Src: String);
// клонировать документ
function CloneDocument(SourceDom: IXmlDomDocument2): IXmlDomDocument2;
{Уровень вложенности узла}
function GetNodeDepth(Node: IXMLDOMNode): integer;

implementation

function GetDomDocument(out ipDomDocument : IXMLDomDocument2) : boolean;
begin
 ipDomDocument := CreateComObject(CLASS_FreeThreadedDOMDocument) as IXMLDOMDocument2;
// ipDomDocument := CreateComObject(Class_DomDocument) as IXMLDOMDocument;
 result := Assigned(ipDomDocument)
end; // GetDomDocument

procedure  KillDOMDocument(var DOM: IXMLDomDocument2);
var
  tmpDOM: IXMLDOMDocument2;
begin
  if not Assigned(DOM) then
    exit;
  tmpDOM := DOM;
  DOM := nil;
  tmpDOM.loadXML('');
  tmpDOM := nil;
end;

// Бежим по списку потомков и перед каждым вставляем новую строку.
// После последнего элемента в списке детей тоже вставляем новую строку.
procedure InsertText(elem : IXMLDOMElement);

  function GetTextNewLine: IXMLdomText;
  begin
    if elem.OwnerDocument <> nil then
      Result := elem.OwnerDocument.CreateTextNode(#13#10);
  end;

var
  elem2, temp: IXMLdomNode;
begin
  try
    try
      // Значит ненужно если (один потомок и он текст) или потомков 0
      if elem.ChildNodes.Length > 0 then
      begin
        if not ((elem.ChildNodes.Length = 1) and (elem.FirstChild.nodeType = Node_Text)) then
        begin
          elem2 := elem.FirstChild;
          while elem2 <> nil do
          begin
            if elem2.QueryInterface(IXMLDOMElement, temp) = 0 then
            begin
              InsertText(elem2 as IXMLDOMElement);
              elem.InsertBefore(GetTextNewLine, elem2);
            end;
            elem2 := elem2.NextSibling;
          end;
          elem.AppendChild(GetTextNewLine);
        end;
      end;
    except
      on e: Exception do
        raise Exception.Create('InsertText: Ошибка вставки текста новой строки. ' + e.Message);
    end;
  finally
    elem2 := nil;
    temp := nil;
  end;
end;

// Отформатировать документ (создать отступы и символы перевода строки)
function SaveFormattedXMLDocumentToStream(const DomDoc: IXMLdomDocument2; tmpStream: TFileStream): boolean;
var ReplyDoc: IXMLDomDocument2;
    ProcInstruction: IXMLDOMProcessingInstruction;
    tmpStr : string;
begin
  result := false;
  try
    if domDoc.DocumentElement <> nil then begin
      // Вставили символы перевода строки, где надо
      InsertText(domDoc.DocumentElement);
      GetDomDocument(ReplyDoc);
      // Перечитали,только теперь будет форматирование при сохранении
      ReplyDoc.loadXML(domDoc.DocumentElement.xml);
      // Эту интсрукцию нужно создать и добавить самим т.к. encoding не передаётся через xml
      ProcInstruction :=
        ReplyDoc.createProcessingInstruction('xml', 'version="1.0" encoding="windows-1251"');
      ReplyDoc.insertBefore(ProcInstruction, ReplyDoc.FirstChild);
      ReplyDoc.PreserveWhiteSpace := true;
      tmpStr := '<?xml version="1.0" encoding="windows-1251"?>'+#13#10 +
        string(ReplyDoc.DocumentElement.xml);
      tmpStream.Position := 0;
      tmpStream.WriteBuffer(tmpStr[1], length(tmpStr));
      result := true;
    end
  finally
    ReplyDoc := nil;
    ProcInstruction := nil;
  end;
end;

// Отформатировать документ (создать отступы и символы перевода строки)
function SaveFormattedXMLDocument(const DomDoc: IXMLdomDocument2; const FileName : string): boolean;
var
	tmpStream : TFileStream;
begin
  tmpStream := TFileStream.Create(FileName, fmCreate or fmShareDenyRead);
  try
  	result := SaveFormattedXMLDocumentToStream(DomDoc, tmpStream);
  finally
    tmpStream.Free;
  end;
end;

procedure FormatXMLDocument(const XML: IXMLDOMDocument2);
  // бежим по списку потомков и перед каждым вставляем новую строку
  // после последнего элемента в списке детей тоже вставляем новую строку.
  procedure InsertNL(const elem : IXMLdomElement);
    function GetTextNewLine: IXMLdomText;
    begin
      if (elem.OwnerDocument <> nil) then
        Result := elem.OwnerDocument.CreateTextNode(#13#10);
    end;

  var elem2 : IXMLdomNode;
      temp : IXMLDOMElement;
  begin
    try
      // Значит ненужно если (один потомок и он текст) или потомков 0
      if (elem.ChildNodes.Length > 0) then
        begin
          if not ((elem.ChildNodes.Length = 1) and (elem.FirstChild.nodeType = Node_Text)) then
            begin
              elem2 := elem.FirstChild;
              while (elem2 <> nil) do
                begin
                  if (elem2.QueryInterface(IXMLdomElement, temp) = S_OK) then
                    begin
                      InsertNL(temp);
                      elem.InsertBefore(GetTextNewLine, elem2);
                    end;
                  elem2 := elem2.NextSibling;
                end;
              elem.AppendChild(GetTextNewLine);
            end;
        end;
    except
      on e: Exception do
        raise Exception.Create('Форматирование XML.' + #13#10 + e.Message);
    end;
  end;
begin
  if (XML.documentElement <> nil) then
    begin
      XML.PreserveWhiteSpace := false;
      InsertNL(XML.documentElement);
      XML.loadXML(XML.documentElement.xml);
//      AddCaptionXML(XML);
      XML.PreserveWhiteSpace := true;
      XML.insertBefore(XML.createProcessingInstruction('xml',
        'version="1.0" encoding="windows-1251"'),
        XML.documentElement);

    end;
end;


function GetAttr(Node: IXMLDOMNode; Name: widestring): widestring;
var tmpNode : IXMLDOMNode;
begin
  result := '';
  if Assigned(Node) and Assigned(Node.Attributes) then
  begin
    tmpNode := Node.Attributes.getNamedItem(Name);
    if Assigned(tmpNode) then
      result := tmpNode.NodeValue;//Trim(tmpNode.NodeValue);
  end;
end; //function GetAttr(Node: IXMLDOMNode; Name: string): string;

function CheckAttr(Node: IXMLDOMNode; Name: widestring; var wideStr: widestring): boolean;
begin
  wideStr := GetAttr(Node, Name);
  Result := wideStr <> '';
end; //function CheckAttr(Node: IXMLDOMNode; Name: string; var Str: string): boolean;

function GetStrAttr(Node: IXMLDOMNode; Name: widestring; Def: widestring): widestring;
begin
  if not CheckAttr(Node, Name, Result)
    then Result := Def;
end; //function GetStrAttr(Node: IXMLDOMNode; Name: string; Def: string): string;

function GetBoolAttr(Node: IXMLDOMNode; Name: widestring; Def: boolean): boolean;
var tmpStr: widestring;
begin
  if not CheckAttr(Node, Name, tmpStr)
    then Result := Def
    else Result := tmpStr = 'true';
end; //function GetBoolAttr(Node: IXMLDOMNode; Name: string; Def: boolean): boolean;

function GetIntAttr(Node: IXMLDOMNode; Name: widestring; Def: Integer): Integer;
var tmpStr: widestring;
begin
  tmpStr := GetAttr(Node, Name);
  Result := StrToIntDef(tmpStr, Def);
end; //function GetIntAttr(Node: IXMLDOMNode; Name: string; Def: Integer): Integer;

function GetDecodedStrAttr(Node: IXMLDOMNode; Name: widestring; Def: widestring): widestring;
var
  tmpStr: string;
begin
  tmpStr := GetStrAttr(Node, Name, Def);
  XMLDecode(tmpStr);
  result := tmpStr;
end;

procedure SetEncodedStrAttr(Node: IXMLDOMNode; Attr, Value: string);
var
  tmpStr: string;
begin
  if Assigned(Node) and (Attr <> '') then
  begin
    tmpStr := Value;
    XMLEncode(tmpStr);
    (Node as IXMLDOMElement).setAttribute(Attr, tmpStr);
  end;
end;

procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: widestring);
begin
  if Assigned(Node) then
    (Node  as IXMLDOMElement).setAttribute(Name, Value);
end;

procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: integer);
begin
  if Assigned(Node) then
    (Node  as IXMLDOMElement).setAttribute(Name, IntToStr(Value));
end;

procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: boolean);
begin
  if Assigned(Node) then
  begin
    if Value then
      (Node  as IXMLDOMElement).setAttribute(Name, 'true')
    else
      (Node  as IXMLDOMElement).setAttribute(Name, 'false')
  end;
end;

procedure SetAttr(Node: IXMLDOMNode; Name: widestring; Value: extended);
begin
  if Assigned(Node) then
    (Node  as IXMLDOMElement).setAttribute(Name, FloatToStr(Value));
end;


function GetNodeStrAttr(Node: IXMLDOMNode; Attr: string): string;
var
  tmpNode: IXMLDOMNode;
begin
  result := '';
  if not Assigned(Node) then
    exit;
  tmpNode := Node.selectSingleNode(Attr);
  if Assigned(tmpNode) then
    result := tmpNode.Text;
  XMLDecode(result);
end;

procedure SetNodeStrAttr(Node: IXMLDOMNode; Attr, Value: string);
var
  tmpNode: IXMLDOMNode;
begin
  if not Assigned(Node) then
    exit;
  tmpNode := Node.selectSingleNode(Attr);
  if Assigned(tmpNode) then
    tmpNode.text := Value
  else
  begin
    tmpNode := Node.ownerDocument.createNode(1, Attr, '');
    tmpNode.text := Value;
    Node.appendChild(tmpNode);
  end
end;


{ Добавление элемента с именем ElemName как последнего потомка элемента Node
  В качестве результата возвращается добавленный элемент }
function AppendElement(Node: IXMLDOMNode; ElemName: WideString): IXMLDOMNode;
var Elem : IXMLDOMElement;
begin
  Elem := Node.ownerDocument.createElement(ElemName);
  result := Node.appendChild(Elem);
end; //AppendElement

{ Добавление элемента с именем ElemName как ind-1-ого потомка элемента Node
  В качестве результата возвращается добавленный элемент }
function InsertElement(Node: IXMLDOMNode; ElemName: WideString; ind: integer): IXMLDOMNode;
var Elem: IXMLDOMElement;
begin
  Elem := Node.ownerDocument.createElement(ElemName);
  result := Node.insertBefore(Elem, Node.childNodes.item[ind]);
end; //InsertElement

{ Добавление текстового узла к Node. Возвращается новый узел }
function AppendText(Node: IXMLDOMNode; ElemText: WideString): IXMLDOMNode;
var Elem: IXMLDOMText;
begin
  Elem := Node.ownerDocument.CreateTextNode(ElemText);
  result := Node.appendChild(Elem);
end; //AppendText

{ Добавление секции CDATA к Node. Возвращается новый узел }
function AppendCDATA(Node: IXMLDOMNode; ElemText: WideString): IXMLDOMNode;
var Elem: IXMLDOMText;
begin
  Elem := Node.ownerDocument.CreateCDATASection(ElemText);
  result := Node.appendChild(Elem);
end; //AppendCDATA

function AppendNamedItem(ipNode : IXMLDOMNode; ItemName : widestring; ItemText : widestring) : IXMLDOMNode;
begin
  result := ipNode.ownerDocument.createNode(NODE_ATTRIBUTE, ItemName, '');
  result.text := ItemText;
  ipNode.attributes.setNamedItem(result);
end; //AppendNamedItem

function AppendBoolNamedItem(ipNode : IXMLDOMNode; ItemName : widestring; ItemVal : boolean) : IXMLDOMNode;
begin
  case ItemVal of
    true  : AppendNamedItem(ipNode, ItemName, 'true');
    false : AppendNamedItem(ipNode, ItemName, 'false');
  end
end;

function AppendElemWithAttr(const Node : IXMLDOMNode; const ElemName : WideString;
  const AttrName : WideString; const AttrVal : OleVariant): IXMLDOMNode;
var tmpElem : IXMLDOMElement;
begin
  tmpElem := Node.OwnerDocument.createElement(ElemName);
  tmpElem.setAttribute(AttrName, AttrVal);
  result:=Node.appendChild(tmpElem)
end;

procedure WaitForDOMLoad(const ipDOM : IXMLDomDocument2);
begin
  repeat
    sleep(50)
  until ipDOM.readyState = 4
end;

{Заменяем спецсимволы сущностями}
procedure XMLEncode(var Src: String);
begin
  Src := StringReplace(Src, '<', '&lt;', [rfReplaceAll]);
  Src := StringReplace(Src, '>', '&gt;', [rfReplaceAll]);
  Src := StringReplace(Src, '&', '&amp;', [rfReplaceAll]);
  Src := StringReplace(Src, '''', '&apos;', [rfReplaceAll]);
  Src := StringReplace(Src, '"', '&quot;', [rfReplaceAll]);
end;

{Заменяет сущности символами}
procedure XMLDecode(var Src: String);
begin
  Src := StringReplace(Src, '&lt;', '<', [rfReplaceAll]);
  Src := StringReplace(Src, '&gt;', '>', [rfReplaceAll]);
  Src := StringReplace(Src, '&amp;', '&', [rfReplaceAll]);
  Src := StringReplace(Src, '&apos;', '''', [rfReplaceAll]);
  Src := StringReplace(Src, '&quot;', '"', [rfReplaceAll]);
end;

procedure AddProcInstruction(var XmlDoc: IXmlDomDocument2);
const
  sCaptionValue: string = 'version="1.0" encoding="windows-1251"';
  sCaptionElement: string = 'xml';
begin
  if not Assigned(XmlDoc) then
    exit;
  if (XmlDoc.documentElement = nil)
  then XmlDoc.appendChild(XmlDoc.createProcessingInstruction(sCaptionElement, sCaptionValue))
  else XmlDoc.insertBefore(XmlDoc.createProcessingInstruction(sCaptionElement, sCaptionValue), XmlDoc.documentElement);
end;

function InitXmlDocument: IXmlDomDocument2;
begin
  try
    if (not GetDomDocument(result)) then abort;
  except
    result := nil;
    Raise Exception.Create('Невозможно создать XML документ');
  end;
  AddProcInstruction(result);
end;

function CloneDocument(SourceDom: IXmlDomDocument2): IXmlDomDocument2;
begin
  GetDomDocument(result);
  result.loadXML(SourceDom.xml);
end;

function GetNodeDepth(Node: IXMLDOMNode): integer;
begin
  result := -1;
  if not Assigned(Node) then
    exit;
  while Assigned(Node.parentNode) do
  begin
    Node := Node.parentNode;
    inc(result);
  end;
end;

end.
