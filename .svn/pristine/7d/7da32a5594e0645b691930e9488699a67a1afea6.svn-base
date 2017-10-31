unit uEvents;

interface

uses
  SysUtils, Classes, MSXML2_TLB, uGlobalPlaningConst, uXMLUtils,
  uFMAddinGeneralUtils, WordXP, ExcelXP, OfficeXP, PlaningTools_TLB, Windows,
  Dialogs, uOfficeUtils, uExcelUtils, uWordUtils;

type
  TEventName = (enBeforeRefresh, enAfterRefresh, enBeforeWriteBack,
    enAfterWriteBack);

  TEvent = class
  private
    FEnabled: boolean;
    FProcessName: string;
    FFulfillingMacrosName: string; //им€ выполн€ющего макроса
  public
    property Enabled: boolean read FEnabled write FEnabled;
    property ProcessName: string read FProcessName write FProcessName;
    property FulfillingMacrosName: string read FFulfillingMacrosName
      write FFulfillingMacrosName;
  end;

  TEvents = class
  private
    FProcessForm: IProcessForm;
    FEvents: array[TEventName] of TEvent;
    function GetEvent(EventName: TEventName): TEvent;
    procedure SetEvent(EventName: TEventName; const Value: TEvent);
    procedure LoadEvent(EventNode: IXMLDOMNode; Eventname: TEventName);
    procedure SaveEvent(EventNode: IXMLDOMNode; EventName: TEventName);
    function GetProcessName(EventName: TEventName): string;
  protected
    constructor Create;
    procedure Load(var Node: IXMLDOMNode);
    procedure Save(var Node: IXMLDOMNode);
  public
    destructor Destroy; override;
    procedure Clear;
    procedure SetExternalLinks(ProcessForm_: IProcessForm);
    property Items[EventName: TEventName]: TEvent read GetEvent
      write SetEvent;
  end;

  TWordEvents = class(TEvents)
  public
    constructor Create;
    destructor Destroy; override;  
    procedure LoadEvents(Document: WordDocument);
    procedure SaveEvents(Document: WordDocument);
    function Execute(Document: WordDocument; EventName: TEventName): boolean;
    function GetMacrosList(Document: WordDocument;
      out MacrosList: TStringList): boolean;
    function IsExistMacros(Document: WordDocument; MacrosName: string): boolean;
  end;

  TExcelEvents = class(TEvents)
  public
    constructor Create;
    destructor Destroy; override;
    procedure LoadEvents(ExcelSheet: ExcelWorkSheet);
    procedure SaveEvents(ExcelSheet: ExcelWorkSheet);
    function Execute(ExcelSheet: ExcelWorkSheet; EventName: TEventName): boolean;
    function GetMacrosList(ExcelBook: ExcelWorkBook;
      out MacrosList: TStringList): boolean;
    function IsExistMacros(ExcelBook: ExcelWorkBook; MacrosName: string): boolean;
  end;

implementation

{ TSheetEvents }

procedure TEvents.Clear;
var
  Cnt: TEventName;
begin
  for Cnt := Low(TEventName) to High(TEventName) do
    with FEvents[Cnt] do
    begin
      Enabled := false;
      FulfillingMacrosName := '';
    end;
end;

function TEvents.GetProcessName(EventName: TEventName): string;
const
  PrefixName = 'ѕользовательское событие';
begin
  case EventName of
    enBeforeRefresh: result := PrefixName + ' до обновлени€';
    enAfterRefresh: result := PrefixName + ' после обновлени€';
    enBeforeWriteBack: result := PrefixName + ' до обратной записи';
    enAfterWriteBack: result := PrefixName + ' после обратной записи';
  else
    result := '';
  end;
end;

constructor TEvents.Create;
var
  Cnt: TEventName;
begin
  Inherited Create;
  for Cnt := Low(TEventName) to High(TEventName) do
  begin
    FEvents[Cnt] := TEvent.Create;
    FEvents[Cnt].ProcessName :=  GetProcessName(Cnt);
  end;
end;

destructor TEvents.Destroy;
var
  Cnt: TEventName;
begin
  FProcessForm := nil;
  for Cnt := Low(TEventName) to High(TEventName) do
    FEvents[Cnt].Free;
  inherited;
end;

function TEvents.GetEvent(EventName: TEventName): TEvent;
begin
  result := FEvents[EventName];
end;

procedure TEvents.SetEvent(EventName: TEventName;
  const Value: TEvent);
begin
  FEvents[EventName] := Value;
end;

procedure TEvents.LoadEvent(EventNode: IXMLDOMNode; Eventname: TEventName);
begin
  if not Assigned(EventNode) then
    exit;

  FEvents[Eventname].Enabled := GetBoolAttr(EventNode, attrEnabled, false);
  FEvents[Eventname].FulfillingMacrosName := GetStrAttr(EventNode,
    attrMacrosName, '');
end;

procedure TEvents.Load(var Node: IXMLDOMNode);
var
  Event: IXMLDOMNode;
begin
  Self.Clear;
  if not Assigned(Node) then
    exit;

  Event := Node.selectSingleNode(xpBeforeRefresh);
  LoadEvent(Event, enBeforeRefresh);

  Event := Node.selectSingleNode(xpAfterRefresh);
  LoadEvent(Event, enAfterRefresh);

  Event := Node.selectSingleNode(xpBeforeWriteBack);
  LoadEvent(Event, enBeforeWriteBack);

  Event := Node.selectSingleNode(xpAfterWriteBack);
  LoadEvent(Event, enAfterWriteBack);
end;

procedure TEvents.SaveEvent(EventNode: IXMLDOMNode; EventName: TEventName);
begin
  if not Assigned(EventNode) then
    exit;
  with (EventNode as IXMLDOMElement) do
  begin
    setAttribute(attrEnabled, BoolToStr(FEvents[EventName].Enabled));
    setAttribute(attrMacrosName, FEvents[EventName].FulfillingMacrosName);
  end;
end;

procedure TEvents.Save(var Node: IXMLDOMNode);
var
  Event: IXMLDOMNode;
begin
  if not Assigned(Node) then
    exit;

  Event := Node.selectSingleNode(xpBeforeRefresh);
  SaveEvent(Event, enBeforeRefresh);

  Event := Node.selectSingleNode(xpAfterRefresh);
  SaveEvent(Event, enAfterRefresh);

  Event := Node.selectSingleNode(xpBeforeWriteBack);
  SaveEvent(Event, enBeforeWriteBack);

  Event := Node.selectSingleNode(xpAfterWriteBack);
  SaveEvent(Event, enAfterWriteBack);
end;

procedure TEvents.SetExternalLinks(ProcessForm_: IProcessForm);
begin
  FProcessForm := nil;
  FProcessForm := ProcessForm_;
end;

constructor TExcelEvents.Create;
begin
  Inherited Create;
end;

destructor TExcelEvents.Destroy;
begin
  inherited;
end;

function TExcelEvents.Execute(ExcelSheet: ExcelWorkSheet;
  EventName: TEventName): boolean;
var
  IsViewProcess: boolean;
  Event: TEvent;
  ExcelBook: ExcelWorkbook;
  ExcelAppl: ExcelApplication;
begin
  result := true;
  if not Assigned(ExcelSheet) then
    exit;
  Self.LoadEvents(ExcelSheet);
  Event := FEvents[EventName];
  if not(Event.Enabled and (Event.FulfillingMacrosName <> '')) then
    exit;
  IsViewProcess := false;
  result := false;
  ExcelBook := (ExcelSheet.Parent as ExcelWorkbook);
  ExcelAppl := ExcelSheet.Application;

  try
    IsViewProcess := (Assigned(ExcelSheet) and Assigned(FProcessForm) and FProcessForm.Showing);
    if IsViewProcess then
      FProcessForm.OpenOperation(Event.ProcessName, false, false, otUser);
    try
      if not IsExistMacros(ExcelBook, Event.FulfillingMacrosName) then
      begin
        if IsViewProcess then
          FProcessForm.PostWarning('ћакрос с именем "' + Event.FulfillingMacrosName +
            '" не существует');
        exit;
      end;
      ExcelAppl.Run('''' + ExcelBook.Name + '''!' + Event.FulfillingMacrosName,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam);
      result := true;
    except
      if IsViewProcess then
        FProcessForm.PostWarning('¬о врем€ выполнени€ событи€ произошла ошибка');
    end;
  finally
    if IsViewProcess then
      FProcessForm.CloseOperation;
  end;
end;

function TExcelEvents.GetMacrosList(ExcelBook: ExcelWorkBook;
  out MacrosList: TStringList): boolean;
begin
  result := false;
  MacrosList := nil;
  if not Assigned(ExcelBook) then
    exit;
  GetExcelMacrosList(ExcelBook, MacrosList);
  result := Assigned(MacrosList);
  if not result then
    MacrosList := TStringList.Create;
end;

function IsExistMacros_(MacrosList: TStringList; MacrosName: string): boolean;
var
  i: integer;
begin
  result := false;
  if (Trim(MacrosName) = '') then
    exit;
  if not Assigned(MacrosList) then
  begin
    result := true;
    exit;
  end;
  for i := 0 to MacrosList.Count - 1 do
    if UpperCase(MacrosList.Strings[i]) = UpperCase(MacrosName) then
    begin
      result := true;
      exit;
    end;
end;

function TExcelEvents.IsExistMacros(ExcelBook: ExcelWorkBook;
  MacrosName: string): boolean;
var
  MacrosList: TStringList;
begin
  result := false;
  if (not Assigned(ExcelBook)) and (Trim(MacrosName) = '') then
    exit;
  try
    MacrosList := nil;
    GetExcelMacrosList(ExcelBook, MacrosList);
    result := IsExistMacros_(MacrosList, MacrosName);
  finally
    FreeStringList(MacrosList);
  end;
end;

function GetEventsNode(Var EventsDOM: IXMLDOMDocument2): IXMLDOMNode;
begin
  result := nil;
  if not Assigned(EventsDOM) then
    exit;
  result := EventsDOM.selectSingleNode('events');
  if not Assigned(result) then
  begin
    EventsDOM.documentElement := EventsDOM.createElement('events');
    result := EventsDOM.selectSingleNode('events');
    InsertElement(result, xpAfterWriteBack, 0);
    InsertElement(result, xpBeforeWriteBack, 0);
    InsertElement(result, xpAfterRefresh, 0);
    InsertElement(result, xpBeforeRefresh, 0);
  end;
end;

procedure TExcelEvents.LoadEvents(ExcelSheet: ExcelWorkSheet);
var
  DOM: IXMLDOMDocument2;
  CP: CustomProperty;
  Node: IXMLDOMNode;
begin
  Self.Clear;
  if not Assigned(ExcelSheet) then
    exit;
  CP := GetCPByName(ExcelSheet, cpUserEvents, false);
  if not Assigned(CP) then
     exit;
  try
    GetDOMDocument(DOM);
    DOM.loadXML(CP.Value);
    Node := GetEventsNode(DOM);
    Load(Node);
  finally
    KillDOMDocument(DOM);
  end;
end;

procedure TExcelEvents.SaveEvents(ExcelSheet: ExcelWorkSheet);
var
  DOM: IXMLDOMDocument2;
  CP: CustomProperty;
  Node: IXMLDOMNode;
begin
  if not Assigned(ExcelSheet) then
    exit;
  CP := GetCPByName(ExcelSheet, cpUserEvents, true);
  if not Assigned(CP) then
     exit;
  try
    GetDOMDocument(DOM);
    Node := GetEventsNode(DOM);
    Save(Node);
    CP.Value := DOM.xml;
  finally
    KillDOMDocument(DOM);
  end;
end;

{TWordEvents}

constructor TWordEvents.Create;
begin
  Inherited Create;
end;

destructor TWordEvents.Destroy;
begin
  inherited;
end;

function TWordEvents.Execute(Document: WordDocument;
  EventName: TEventName): boolean;
var
  IsViewProcess: boolean;
  Event: TEvent;
  WordAppl: WordApplication;
begin
  result := true;
  if not Assigned(Document) then
    exit;
  Self.LoadEvents(Document);
  Event := FEvents[EventName];
  if not(Event.Enabled and (Event.FulfillingMacrosName <> '')) then
    exit;
  IsViewProcess := false;
  result := false;
  WordAppl := Document.Application;

  try
    IsViewProcess := (Assigned(Document) and Assigned(FProcessForm) and FProcessForm.Showing);
    if IsViewProcess then
      FProcessForm.OpenOperation(Event.ProcessName, false, false, otUser);
    try
      if not IsExistMacros(Document, Event.FulfillingMacrosName) then
      begin
        if IsViewProcess then
          FProcessForm.PostWarning('ћакрос с именем "' + Event.FulfillingMacrosName +
            '" не существует');
        exit;
      end;
      WordAppl.Run(Event.FulfillingMacrosName,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam, EmptyParam);
      result := true;
    except
      if IsViewProcess then
        FProcessForm.PostWarning('¬о врем€ выполнени€ событи€ произошла ошибка');
    end;
  finally
    if IsViewProcess then
      FProcessForm.CloseOperation;
  end;
end;

function TWordEvents.GetMacrosList(Document: WordDocument;
  out MacrosList: TStringList): boolean;
begin
  result := false;
  MacrosList := nil;
  if not Assigned(Document) then
    exit;
  GetWordMacrosList(Document, MacrosList);
  result := Assigned(MacrosList);
  if not result then
    MacrosList := TStringList.Create;
end;

function TWordEvents.IsExistMacros(Document: WordDocument;
  MacrosName: string): boolean;
var
  MacrosList: TStringList;
begin
  result := false;
  if (not Assigned(Document)) and (Trim(MacrosName) = '') then
    exit;
  try
    MacrosList := nil;
    GetWordMacrosList(Document, MacrosList);
    result := IsExistMacros_(MacrosList, MacrosName);
  finally
    FreeStringList(MacrosList);
  end;
end;

procedure TWordEvents.LoadEvents(Document: WordDocument);
var
  DOM: IXMLDOMDocument2;
  Script_: Script;
  Node: IXMLDOMNode;
begin
  Self.Clear;
  if not Assigned(Document) then
    exit;
  Script_ := GetHideScriptByName(Document, cpUserEvents, false);
  if not Assigned(Script_) then
     exit;
  try
    GetDOMDocument(DOM);
    DOM.loadXML(Script_.ScriptText);
    Node := GetEventsNode(DOM);
    Load(Node);
  finally
    KillDOMDocument(DOM);
  end;
end;

procedure TWordEvents.SaveEvents(Document: WordDocument);
var
  DOM: IXMLDOMDocument2;
  Script_: Script;
  Node: IXMLDOMNode;
begin
  if not Assigned(Document) then
    exit;
  Script_ := GetHideScriptByName(Document, cpUserEvents, true);
  if not Assigned(Script_) then
     exit;
  try
    GetDOMDocument(DOM);
    Node := GetEventsNode(DOM);
    Save(Node);
    Script_.ScriptText := DOM.xml;
  finally
    KillDOMDocument(DOM);
  end;
end;

end.
