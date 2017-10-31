unit uWordUtils;

interface
uses
  WordXp, ComObj, OfficeXP, Dialogs, MSXML2_TLB, uXMLUtils;

  {������� Word}
  function GetWord(out Word : WordApplication; IsVisible: boolean) : boolean;
  {�������� �������� �������� � �����}
  function GetActiveDocument(WordAppl: WordApplication): WordDocument;
  {��� �������� �����}
  function GetActiveDocumentName(WordAppl: WordApplication): string;
  {���������� ������}
  procedure SetScreenUpdating(WordAppl: WordApplication; State: boolean);
  {����� �������� ������}
  procedure SetShowHiddenText(Window_: Window; State: boolean);
  {����������� ���� ������ ��������������}
  procedure SetShowAll(Window_: Window; State: boolean);
  {����������� ������� ������� ����� ������������� ������������ ��� ������
  ���������}
  procedure SetShowEditableRanges(Document: WordDocument; State: boolean);
  procedure GetCoordinate(Range_: Range; out Start, End_: integer);
  {���������� Script �� �����; ���� �� �������, �� ����� �������.}
  function GetHideScriptByName(Document: WordDocument; Name: string;
    ForceCreate: boolean): Script;
  {���������� Script �� �����; ���� �� �������, �� ����� �������.}
  function GetVisibleScriptByName(Document: WordDocument; Name: string;
    ForceCreate: boolean): Script;
  {���������� �������� Script-� �� �����}
  function GetDataFromScript(Document: WordDocument; Name: string): IXMLDOMDocument2;
  function GetDataFromScriptStr(Document: WordDocument; Name: string): string;
  {����������� Script-� ��������, ��� ������������� ������� Script}
  procedure PutDataInScript(Document: WordDocument; Name: string; Data: IXMLDOMDocument2); overload;
  procedure PutDataInScript(Document: WordDocument; Name: string; Data: string); overload;
implementation

{������� Word}
function GetWord(out Word : WordApplication; IsVisible: boolean) : boolean;
begin
  result := true;
  try
    Word := CreateComObject(CLASS_WordApplication) as WordApplication;
    Word.Visible := IsVisible;
    {$WARNINGS OFF}
    if IsVisible then
      Word.DisplayAlerts := wdAlertsAll
    else
      Word.DisplayAlerts := wdAlertsNone;
    {$WARNINGS ON}
  except
    result := false;
  end;
end;

{�������� �������� �������� � �����}
function GetActiveDocument(WordAppl: WordApplication): WordDocument;
begin
  result := nil;
  if not Assigned(WordAppl) then
    exit;
  try
    result := WordAppl.ActiveDocument;
  except
    result := nil;
  end;
end;

{��� �������� �����}
function GetActiveDocumentName(WordAppl: WordApplication): string;
var
  ActiveDocument: WordDocument;
begin
  result := '';
  ActiveDocument := GetActiveDocument(WordAppl);
  try
    if Assigned(ActiveDocument) then
      result := ActiveDocument.FullName;
  except
  end;
end;

{���������� ������}
procedure SetScreenUpdating(WordAppl: WordApplication; State: boolean);
begin
  if Assigned(WordAppl) then
    WordAppl.ScreenUpdating := State;
end;

{����������� �������� ������}
procedure SetShowHiddenText(Window_: Window; State: boolean);
begin
  if Assigned(Window_) then
    Window_.View.ShowHiddenText := State;
end;

{����������� ���� ������ ��������������}
procedure SetShowAll(Window_: Window; State: boolean);
begin
  if Assigned(Window_) then
    Window_.View.ShowAll := State;
end;

{����������� ������� ������� ����� ������������� ������������ ��� ������
���������}
procedure SetShowEditableRanges(Document: WordDocument; State: boolean);
var
  ODoc: OleVariant;
begin
  if not Assigned(Document) then
    exit;
  try
    ODoc := Document;
    ODoc.ActiveWindow.View.ShadeEditableRanges := State;
  except
  end;
end;

procedure GetCoordinate(Range_: Range; out Start, End_: integer);
begin
  Start := 0;
  End_ := 0;
  if not Assigned(Range_) then
    exit;
  Start := Range_.Start;
  End_ := Range_.End_;
end;

function FindScript(Document: WordDocument; Name: string): Script;
begin
  result := nil;
  if not(Assigned(Document) and (Document.Scripts.Count > 0)) then
    exit;
  try
    result := Document.Scripts.Item(Name);
  except
  end;
end;

{���������� Script �� �����; ���� �� �������, �� ����� �������.}
function GetHideScriptByName(Document: WordDocument; Name: string;
  ForceCreate: boolean): Script;
begin
  result := FindScript(Document, Name);
  if Assigned(result) then
    exit;

  if (not Assigned(result) and ForceCreate) then
    result := Document.Scripts.Add(nil, msoScriptLocationInHead,
      msoScriptLanguageOther, Name, '', '');
end;

{���������� Script �� �����; ���� �� �������, �� ����� �������.}
function GetVisibleScriptByName(Document: WordDocument; Name: string;
  ForceCreate: boolean): Script;
var
  OldStart, OldEnd: integer;
  Sel: Selection;
begin
  result := FindScript(Document, Name);
  if Assigned(result) then
    exit;

  if (not Assigned(result) and ForceCreate) then
  begin
    {��������� ��� ���� ������� � ������ ��������}
    Sel := Document.ActiveWindow.Get_Selection;
    GetCoordinate(Sel.Range, OldStart, OldEnd);

    Sel.SetRange(Document.Scripts.Count, Document.Scripts.Count);

    result := Document.Scripts.Add(nil, msoScriptLocationInBody,
      msoScriptLanguageOther, Name, '', '');

    Sel.SetRange(OldStart, OldEnd);
  end;
end;

function GetDataFromScript(Document: WordDocument; Name: string): IXMLDOMDocument2;
var
  Script_: Script;
begin
  result := nil;
  if not Assigned(Document) then
    exit;
  Script_ := GetHideScriptByName(Document, Name, false);
  if Assigned(Script_) then
  begin
    GetDomDocument(result);
    result.loadXML(Script_.ScriptText);
  end;
end;

function GetDataFromScriptStr(Document: WordDocument; Name: string): string;
var
  Script_: Script;
begin
  result := '';
  if not Assigned(Document) then
    exit;
  Script_ := GetHideScriptByName(Document, Name, false);
  if Assigned(Script_) then
    result := Script_.ScriptText;
end;

procedure PutDataInScript(Document: WordDocument; Name: string; Data: IXMLDOMDocument2);
var
  Script_: Script;
begin
  if not(Assigned(Document) and Assigned(Data))then
    exit;
  Script_ := GetHideScriptByName(Document, Name, false);
  if Assigned(Script_) then
    Script_.ScriptText := Data.xml;
end;

procedure PutDataInScript(Document: WordDocument; Name: string; Data: string);
var
  Script_: Script;
begin
  if not Assigned(Document) and (Data <> '') then
    exit;
  Script_ := GetHideScriptByName(Document, Name, true);
  if Assigned(Script_) then
    Script_.ScriptText := Data;
end;

end.
