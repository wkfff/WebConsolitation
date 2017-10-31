unit WordComAddinEvents;

interface

uses
  Windows, WordXP, ComAddInUtils, ActiveX;

type
  {прототипы функций-обработчикоы событий Word Document}
  TOnDocumentNew = procedure of object;// dispid 4
  TOnDocumentOpen = procedure of object;// dispid 5
  TOnDocumentClose = procedure of object;// dispid 6

  {прототипы функций-обработчикоы событий Word Application}
  TOnWordAppStartup = procedure of Object;// dispid 1;
  TOnWordAppQuit = procedure of Object;// dispid 2;
  TOnWordAppDocumentChange = procedure of Object;// dispid 3;
  TOnWordAppDocumentOpen = procedure(const Doc: WordDocument) of
    object;// dispid 4;
  TOnWordAppDocumentBeforeClose = procedure(const Doc: WordDocument;
    var Cancel: WordBool) of object;//; dispid 6;
  TOnWordAppDocumentBeforePrint = procedure(const Doc: WordDocument;
    var Cancel: WordBool) of object;// dispid 7;
  TOnWordAppDocumentBeforeSave = procedure(const Doc: WordDocument;
    var SaveAsUI: WordBool; var Cancel: WordBool) of object;// dispid 8;
  TOnWordAppNewDocument = procedure(const Doc: WordDocument) of
    object;// dispid 9;
  TOnWordAppWindowActivate = procedure(const Doc: WordDocument;
    const Wn: Window) of object;// dispid 10;
  TOnWordAppWindowDeactivate = procedure(const Doc: WordDocument;
    const Wn: Window)of object;// dispid 11;
  TOnWordAppWindowSelectionChange = procedure(const Sel: Selection) of
    object;// dispid 12;
  TOnWordAppWindowBeforeRightClick = procedure (const Sel: Selection;
    var Cancel: WordBool) of object;// dispid 13;
  TOnWordAppWindowBeforeDoubleClick = procedure(const Sel: Selection;
    var Cancel: WordBool) of object;// dispid 14;
  TOnWordAppEPostagePropertyDialog = procedure(const Doc: WordDocument) of
    object;// dispid 15;
  TOnWordAppEPostageInsert = procedure(const Doc: WordDocument) of
    object;// dispid 16;
  TOnWordAppMailMergeAfterMerge = procedure(const Doc: WordDocument;
    const DocResult: WordDocument) of object;// dispid 17;
  TOnWordAppMailMergeAfterRecordMerge = procedure(const Doc: WordDocument) of
    object;// dispid 18;
  TOnWordAppMailMergeBeforeMerge = procedure(const Doc: WordDocument;
    StartRecord: Integer; EndRecord: Integer; var Cancel: WordBool) of
    object;// dispid 19;
  TOnWordAppMailMergeBeforeRecordMerge = procedure(const Doc: WordDocument;
    var Cancel: WordBool) of object;// dispid 20;
  TOnWordAppMailMergeDataSourceLoad = procedure(const Doc: WordDocument) of
    object;// dispid 21;
  TOnWordAppMailMergeDataSourceValidate = procedure(const Doc: WordDocument;
    var Handled: WordBool) of object;// dispid 22;
  TOnWordAppMailMergeWizardSendToCustom = procedure(const Doc: WordDocument) of
    object;// dispid 23;
  TOnWordAppMailMergeWizardStateChange = procedure(const Doc: WordDocument;
    var FromState: SYSINT; var ToState: SYSINT; var Handled: WordBool) of
    object;// dispid 24;
  TOnWordAppWindowSize = procedure(const Doc: WordDocument; const Wn: Window) of
    object;// dispid 25;

  TDocumentEventSink = class(TBaseSink)
  private
    FOnDocumentNew: TOnDocumentNew;
    FOnDocumentOpen: TOnDocumentOpen;
    FOnDocumentClose: TOnDocumentClose;
  protected
    function DoInvoke (DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
    procedure DoNew; virtual;
    procedure DoOpen; virtual;
    procedure DoClose; virtual;
  public
    constructor Create; virtual;
    property OnDocumentNew: TOnDocumentNew read FOnDocumentNew
      write FOnDocumentNew;
    property OnDocumentOpen: TOnDocumentOpen read FOnDocumentOpen
      write FOnDocumentOpen;
    property OnDocumentClose: TOnDocumentClose read FOnDocumentClose
      write FOnDocumentClose;
  end;

  TWordAppEventSink = class(TBaseSink)
  private
    FOnWordAppStartup: TOnWordAppStartup;
    FOnWordAppQuit: TOnWordAppQuit;
    FOnWordAppDocumentChange: TOnWordAppDocumentChange;
    FOnWordAppDocumentOpen: TOnWordAppDocumentOpen;
    FOnWordAppDocumentBeforeClose: TOnWordAppDocumentBeforeClose;
    FOnWordAppDocumentBeforePrint: TOnWordAppDocumentBeforePrint;
    FOnWordAppDocumentBeforeSave: TOnWordAppDocumentBeforeSave;
    FOnWordAppNewDocument: TOnWordAppNewDocument;
    FOnWordAppWindowActivate: TOnWordAppWindowActivate;
    FOnWordAppWindowDeactivate: TOnWordAppWindowDeactivate;
    FOnWordAppWindowSelectionChange: TOnWordAppWindowSelectionChange;
    FOnWordAppWindowBeforeRightClick: TOnWordAppWindowBeforeRightClick;
    FOnWordAppWindowBeforeDoubleClick: TOnWordAppWindowBeforeDoubleClick;
    FOnWordAppEPostagePropertyDialog: TOnWordAppEPostagePropertyDialog;
    FOnWordAppEPostageInsert: TOnWordAppEPostageInsert;
    FOnWordAppMailMergeAfterMerge: TOnWordAppMailMergeAfterMerge;
    FOnWordAppMailMergeAfterRecordMerge: TOnWordAppMailMergeAfterRecordMerge;
    FOnWordAppMailMergeBeforeMerge: TOnWordAppMailMergeBeforeMerge;
    FOnWordAppMailMergeBeforeRecordMerge: TOnWordAppMailMergeBeforeRecordMerge;
    FOnWordAppMailMergeDataSourceLoad: TOnWordAppMailMergeDataSourceLoad;
    FOnWordAppMailMergeDataSourceValidate: TOnWordAppMailMergeDataSourceValidate;
    FOnWordAppMailMergeWizardSendToCustom: TOnWordAppMailMergeWizardSendToCustom;
    FOnWordAppMailMergeWizardStateChange: TOnWordAppMailMergeWizardStateChange;
    FOnWordAppWindowSize: TOnWordAppWindowSize;
  protected
    function DoInvoke(DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs : TDispParams; PDispIDs : PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
    procedure DoStartup;
    procedure DoQuit;
    procedure DoDocumentChange;
    procedure DoDocumentOpen(const Doc: WordDocument);
    procedure DoDocumentBeforeClose(const Doc: WordDocument; var Cancel: WordBool);
    procedure DoDocumentBeforePrint(const Doc: WordDocument; var Cancel: WordBool);
    procedure DoDocumentBeforeSave(const Doc: WordDocument; var SaveAsUI: WordBool;
      var Cancel: WordBool);
    procedure DoNewDocument(const Doc: WordDocument);
    procedure DoWindowActivate(const Doc: WordDocument; const Wn: Window);
    procedure DoWindowDeactivate(const Doc: WordDocument; const Wn: Window);
    procedure DoWindowSelectionChange(const Sel: Selection);
    procedure DoWindowBeforeRightClick(const Sel: Selection; var Cancel: WordBool);
    procedure DoWindowBeforeDoubleClick(const Sel: Selection; var Cancel: WordBool);
    procedure DoEPostagePropertyDialog(const Doc: WordDocument);
    procedure DoEPostageInsert(const Doc: WordDocument);
    procedure DoMailMergeAfterMerge(const Doc: WordDocument;
      const DocResult: WordDocument);
    procedure DoMailMergeAfterRecordMerge(const Doc: WordDocument);
    procedure DoMailMergeBeforeMerge(const Doc: WordDocument; StartRecord: Integer;
      EndRecord: Integer; var Cancel: WordBool);
    procedure DoMailMergeBeforeRecordMerge(const Doc: WordDocument;
      var Cancel: WordBool);
    procedure DoMailMergeDataSourceLoad(const Doc: WordDocument);
    procedure DoMailMergeDataSourceValidate(const Doc: WordDocument;
      var Handled: WordBool);
    procedure DoMailMergeWizardSendToCustom(const Doc: WordDocument);
    procedure DoMailMergeWizardStateChange(const Doc: WordDocument;
      var FromState: SYSINT; var ToState: SYSINT; var Handled: WordBool);
    procedure DoWindowSize(const Doc: WordDocument; const Wn: Window);
  public
    constructor Create; virtual;
    property OnWordAppStartup: TOnWordAppStartup read FOnWordAppStartup
      write FOnWordAppStartup;
    property OnWordAppQuit: TOnWordAppQuit read FOnWordAppQuit
      write FOnWordAppQuit;
    property OnWordAppDocumentChange: TOnWordAppDocumentChange
      read FOnWordAppDocumentChange
      write FOnWordAppDocumentChange;
    property OnWordAppDocumentOpen: TOnWordAppDocumentOpen
      read FOnWordAppDocumentOpen
      write FOnWordAppDocumentOpen;
    property OnWordAppDocumentBeforeClose: TOnWordAppDocumentBeforeClose
      read FOnWordAppDocumentBeforeClose
      write FOnWordAppDocumentBeforeClose;
    property OnWordAppDocumentBeforePrint: TOnWordAppDocumentBeforePrint
      read FOnWordAppDocumentBeforePrint
      write FOnWordAppDocumentBeforePrint;
    property OnWordAppDocumentBeforeSave: TOnWordAppDocumentBeforeSave
      read FOnWordAppDocumentBeforeSave
      write FOnWordAppDocumentBeforeSave;
    property OnWordAppNewDocument: TOnWordAppNewDocument
      read FOnWordAppNewDocument
      write FOnWordAppNewDocument;
    property OnWordAppWindowActivate: TOnWordAppWindowActivate
      read FOnWordAppWindowActivate
      write FOnWordAppWindowActivate;
    property OnWordAppWindowDeactivate: TOnWordAppWindowDeactivate
      read FOnWordAppWindowDeactivate
      write FOnWordAppWindowDeactivate;
    property OnWordAppWindowSelectionChange: TOnWordAppWindowSelectionChange
      read FOnWordAppWindowSelectionChange
      write FOnWordAppWindowSelectionChange;
    property OnWordAppWindowBeforeRightClick: TOnWordAppWindowBeforeRightClick
      read FOnWordAppWindowBeforeRightClick
      write FOnWordAppWindowBeforeRightClick;
    property OnWordAppWindowBeforeDoubleClick: TOnWordAppWindowBeforeDoubleClick
      read FOnWordAppWindowBeforeDoubleClick
      write FOnWordAppWindowBeforeDoubleClick;
    property OnWordAppEPostagePropertyDialog: TOnWordAppEPostagePropertyDialog
      read FOnWordAppEPostagePropertyDialog
      write FOnWordAppEPostagePropertyDialog;
    property OnWordAppEPostageInsert: TOnWordAppEPostageInsert
      read FOnWordAppEPostageInsert
      write FOnWordAppEPostageInsert;
    property OnWordAppMailMergeAfterMerge: TOnWordAppMailMergeAfterMerge
      read FOnWordAppMailMergeAfterMerge
      write FOnWordAppMailMergeAfterMerge;
    property OnWordAppMailMergeAfterRecordMerge: TOnWordAppMailMergeAfterRecordMerge
      read FOnWordAppMailMergeAfterRecordMerge
      write FOnWordAppMailMergeAfterRecordMerge;
    property OnWordAppMailMergeBeforeMerge: TOnWordAppMailMergeBeforeMerge
      read FOnWordAppMailMergeBeforeMerge
      write FOnWordAppMailMergeBeforeMerge;
    property OnWordAppMailMergeBeforeRecordMerge: TOnWordAppMailMergeBeforeRecordMerge
      read FOnWordAppMailMergeBeforeRecordMerge
      write FOnWordAppMailMergeBeforeRecordMerge;
    property OnWordAppMailMergeDataSourceLoad: TOnWordAppMailMergeDataSourceLoad
      read FOnWordAppMailMergeDataSourceLoad
      write FOnWordAppMailMergeDataSourceLoad;
    property OnWordAppMailMergeDataSourceValidate: TOnWordAppMailMergeDataSourceValidate
      read FOnWordAppMailMergeDataSourceValidate
      write FOnWordAppMailMergeDataSourceValidate;
    property OnWordAppMailMergeWizardSendToCustom: TOnWordAppMailMergeWizardSendToCustom
      read FOnWordAppMailMergeWizardSendToCustom
      write FOnWordAppMailMergeWizardSendToCustom;
    property OnWordAppMailMergeWizardStateChange: TOnWordAppMailMergeWizardStateChange
      read FOnWordAppMailMergeWizardStateChange
      write FOnWordAppMailMergeWizardStateChange;
    property OnWordAppWindowSize: TOnWordAppWindowSize
      read FOnWordAppWindowSize
      write FOnWordAppWindowSize;
  end;

implementation
uses
  SysUtils;
{ TDocumentEventSink }

constructor TDocumentEventSink.Create;
begin
  inherited Create;
  FSinkIID := DocumentEvents;
end;

function TDocumentEventSink.DoInvoke(DispID: integer; const IID: TGUID;
  LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    4: DoNew;
    5: DoOpen;
    6: DoClose;
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

procedure TDocumentEventSink.DoNew;
begin
  if Assigned(FOnDocumentNew) then
    FOnDocumentNew;
end;

procedure TDocumentEventSink.DoOpen;
begin
  if Assigned(FOnDocumentOpen) then
    FOnDocumentOpen;
end;

procedure TDocumentEventSink.DoClose;
begin
  if Assigned(FOnDocumentClose) then
    FOnDocumentClose;
end;

{ TWordAppEventSink }

constructor TWordAppEventSink.Create;
begin
  inherited;
  FSinkIID := ApplicationEvents3;
end;

function TWordAppEventSink.DoInvoke(DispID: integer; const IID: TGUID;
  LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    1: DoStartup;
    2: DoQuit;
    3: DoDocumentChange;
    4: DoDocumentOpen(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
         as WordDocument);
    6: DoDocumentBeforeClose(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
         as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    7: DoDocumentBeforePrint(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
         as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    8: DoDocumentBeforeSave(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
         as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pbool^,
         DPs.rgvarg^ [PDispIDs^[2]].pbool^);
    9: DoNewDocument(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
         as WordDocument);
    10: DoWindowActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
          as Window);
    11: DoWindowDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
          as Window);
    12: DoWindowSelectionChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as Selection);
    13: DoWindowBeforeRightClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as Selection, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    14: DoWindowBeforeDoubleClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as Selection, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    15: DoEPostagePropertyDialog(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument);
    16: DoEPostageInsert(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument);
    17: DoMailMergeAfterMerge(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
          as WordDocument);
    18: DoMailMergeAfterRecordMerge(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument);
    19: DoMailMergeBeforeMerge(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pintVal^,
          DPs.rgvarg^ [PDispIDs^[2]].pintVal^, DPs.rgvarg^ [PDispIDs^[3]].pbool^);
    20: DoMailMergeBeforeRecordMerge(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    21: DoMailMergeDataSourceLoad(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument);
    22: DoMailMergeDataSourceValidate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    23: DoMailMergeWizardSendToCustom(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument);
    24: DoMailMergeWizardStateChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, DPs.rgvarg^ [PDispIDs^[1]].pintVal^,
          DPs.rgvarg^ [PDispIDs^[2]].pintVal^, DPs.rgvarg^ [PDispIDs^[3]].pbool^);
    25: DoWindowSize(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
          as WordDocument, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
          as Window);
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

procedure TWordAppEventSink.DoDocumentBeforeClose(const Doc: WordDocument;
  var Cancel: WordBool);
begin
  if Assigned(FOnWordAppDocumentBeforeClose) then
    FOnWordAppDocumentBeforeClose(Doc, Cancel);
end;

procedure TWordAppEventSink.DoDocumentBeforePrint(const Doc: WordDocument;
  var Cancel: WordBool);
begin
  if Assigned(FOnWordAppDocumentBeforePrint) then
    FOnWordAppDocumentBeforePrint(Doc, Cancel);
end;

procedure TWordAppEventSink.DoDocumentBeforeSave(const Doc: WordDocument;
  var SaveAsUI, Cancel: WordBool);
begin
  if Assigned(FOnWordAppDocumentBeforeSave) then
    FOnWordAppDocumentBeforeSave(Doc, SaveAsUI, Cancel);
end;

procedure TWordAppEventSink.DoDocumentChange;
begin
  if Assigned(FOnWordAppDocumentChange) then
    FOnWordAppDocumentChange;
end;

procedure TWordAppEventSink.DoDocumentOpen(const Doc: WordDocument);
begin
  if Assigned(FOnWordAppDocumentOpen) then
    FOnWordAppDocumentOpen(Doc);
end;

procedure TWordAppEventSink.DoEPostageInsert(const Doc: WordDocument);
begin
  if Assigned(FOnWordAppEPostageInsert) then
    FOnWordAppEPostageInsert(Doc);
end;

procedure TWordAppEventSink.DoEPostagePropertyDialog(
  const Doc: WordDocument);
begin
  if Assigned(FOnWordAppEPostagePropertyDialog) then
    FOnWordAppEPostagePropertyDialog(Doc);
end;

procedure TWordAppEventSink.DoMailMergeAfterMerge(const Doc,
  DocResult: WordDocument);
begin
  if Assigned(FOnWordAppMailMergeAfterMerge) then
    FOnWordAppMailMergeAfterMerge(Doc, DocResult);
end;

procedure TWordAppEventSink.DoMailMergeAfterRecordMerge(
  const Doc: WordDocument);
begin
  if Assigned(FOnWordAppMailMergeAfterRecordMerge) then
    FOnWordAppMailMergeAfterRecordMerge(Doc);
end;

procedure TWordAppEventSink.DoMailMergeBeforeMerge(const Doc: WordDocument;
  StartRecord, EndRecord: Integer; var Cancel: WordBool);
begin
  if Assigned(FOnWordAppMailMergeBeforeMerge) then
    FOnWordAppMailMergeBeforeMerge(Doc, StartRecord, EndRecord, Cancel);
end;

procedure TWordAppEventSink.DoMailMergeBeforeRecordMerge(
  const Doc: WordDocument; var Cancel: WordBool);
begin
  if Assigned(FOnWordAppMailMergeBeforeRecordMerge) then
    FOnWordAppMailMergeBeforeRecordMerge(Doc, Cancel);
end;

procedure TWordAppEventSink.DoMailMergeDataSourceLoad(
  const Doc: WordDocument);
begin
  if Assigned(FOnWordAppMailMergeDataSourceLoad) then
    FOnWordAppMailMergeDataSourceLoad(Doc);
end;

procedure TWordAppEventSink.DoMailMergeDataSourceValidate(
  const Doc: WordDocument; var Handled: WordBool);
begin
  if Assigned(FOnWordAppMailMergeDataSourceValidate) then
    FOnWordAppMailMergeDataSourceValidate(Doc, Handled);
end;

procedure TWordAppEventSink.DoMailMergeWizardSendToCustom(
  const Doc: WordDocument);
begin
  if Assigned(FOnWordAppMailMergeWizardSendToCustom) then
    FOnWordAppMailMergeWizardSendToCustom(Doc);
end;

procedure TWordAppEventSink.DoMailMergeWizardStateChange(
  const Doc: WordDocument; var FromState, ToState: SYSINT;
  var Handled: WordBool);
begin
  if Assigned(FOnWordAppMailMergeWizardStateChange) then
    FOnWordAppMailMergeWizardStateChange(Doc, FromState, ToState, Handled);
end;

procedure TWordAppEventSink.DoNewDocument(const Doc: WordDocument);
begin
  if Assigned(FOnWordAppNewDocument) then
    FOnWordAppNewDocument(Doc);
end;

procedure TWordAppEventSink.DoQuit;
begin
  if Assigned(FOnWordAppQuit) then
    FOnWordAppQuit;
end;

procedure TWordAppEventSink.DoStartup;
begin
  if Assigned(FOnWordAppStartup) then
    FOnWordAppStartup;
end;

procedure TWordAppEventSink.DoWindowActivate(const Doc: WordDocument;
  const Wn: Window);
begin
  if Assigned(FOnWordAppWindowActivate) then
    FOnWordAppWindowActivate(Doc, Wn);
end;

procedure TWordAppEventSink.DoWindowBeforeDoubleClick(const Sel: Selection;
  var Cancel: WordBool);
begin
  if Assigned(FOnWordAppWindowBeforeDoubleClick) then
    FOnWordAppWindowBeforeDoubleClick(Sel, Cancel);
end;

procedure TWordAppEventSink.DoWindowBeforeRightClick(const Sel: Selection;
  var Cancel: WordBool);
begin
  if Assigned(FOnWordAppWindowBeforeRightClick) then
    FOnWordAppWindowBeforeRightClick(Sel, Cancel);
end;

procedure TWordAppEventSink.DoWindowDeactivate(const Doc: WordDocument;
  const Wn: Window);
begin
  if Assigned(FOnWordAppWindowDeactivate) then
    FOnWordAppWindowDeactivate(Doc, Wn);
end;

procedure TWordAppEventSink.DoWindowSelectionChange(const Sel: Selection);
begin
  if Assigned(FOnWordAppWindowSelectionChange) then
    FOnWordAppWindowSelectionChange(Sel);
end;

procedure TWordAppEventSink.DoWindowSize(const Doc: WordDocument;
  const Wn: Window);
begin
  if Assigned(FOnWordAppWindowSize) then
    FOnWordAppWindowSize(Doc, Wn);
end;


end.
