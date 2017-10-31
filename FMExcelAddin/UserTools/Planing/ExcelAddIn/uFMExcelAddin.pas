{
  ��������� ������ �������.
  ��������� COM-��������� IFMPlanningExtension, ������� ���������� � ���������
  ���������������� DLL-�� ��� ������ � ������.
  �� ����, ������ �������� ����������� �������� ������ ���� �������� �������.
  ����� ��� ���������, ����������������, ����� ��������� ������������
  ���������������� ���������, ����� ����������� ������� ������.
  (!) ������ ������ ��������� ������� ������-������ ������ ����� ������������.
}

{$A+,B-,C+,D+,E-,F-,G+,H+,I+,J+,K-,L+,M-,N+,O-,P+,Q-,R-,S-,T-,U-,V+,W-,X+,Y+,Z1}
{$MINSTACKSIZE $00004000}
{$MAXSTACKSIZE $00100000}
{$IMAGEBASE $00400000}
{$APPTYPE GUI}
unit uFMExcelAddin;

interface

uses
  ComObj, ActiveX, FMExcelAddin_TLB, Forms, OfficeXP, ExcelXP, ComAddInUtils,
  uConstructorWizard, uSheetCollector, PlaningProvider_TLB,
  controls, uXMLCatalog, FileCtrl, Registry, classes, MSXML2_TLB, uXmlUtils,
  uPropertiesForm, uParamControl, uConstControl, uConstProperties,
  PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinExcelUtils, uFMAddinRegistryUtils, uFMAddinXMLUtils, uSplitterPad,
  uSheetHistory, uSheetObjectModel, uCopyForm,
  uSingleCellEditor, uSingleCellManager, uExcelUtils, ExcelComAddinEvents,
  Planing_TLB, uGlobalPlaningConst, uOfficeUtils, uSheetComponentEditor, uEvents,
  uAppendEmptyRowToSheet, uOfflineFiltersEditor, uDataCollectionFormWizard,
  uNewConnection, ExtCtrls, uWritebackOptions, 
  uOptions, uTaskContextProvider, uProcessFormSink, uReplicationWizard,
  uFMExcelAddinInterface, uSheetInfo;


{ ������ ������ �������� � �������� Office XP � ����.
  �������� TLB-��� �� XP }

type
  //������ MS Excel
  TMicrosoftExcelVersion = (evXP, ev2003, evUnknown);

  {}
  TRebuildCycleState = (rcsWait, rcsLeave, rcsReturn);

  TFMExcelAddIn = class(TFMExcelAddininterface, IFMPlanningExtension,
                        IFMPlanningAncillary, IFMPlanningVBProgramming,
                        IFMPlaningInteraction)
  private
    FExcelVersion: TMicrosoftExcelVersion; // ������ ������
    FSplitterPad: TfrmSplitterPad;
    FXMLCatalog: TXMLCatalog;
    FfrmConstructorWizard: TfrmConstructorWizard; // ������
    FfrmProperties: TfrmProperties;               //���� �������
    FfrmSheetHistory: TSheetHistory;              //���� ������� �����
    FUserEvents: TExcelEvents;                    //���������������� �������
    NeedEvidentAuthentication: boolean; // � ������ ��� �� ����� ���� �������� ����
    NeedForceRefresh: boolean; //����� �� �������������� ���������� ���������
    VersionOK: boolean;

    Timer: TTimer;

    {���������� �� ��������������, ���������������� �� ������}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;


    // ����� ��������� ��������
    FProcess: IProcessForm;
    FProcessFormSink: TProcessFormSink;
    // ��������� �����
    FTaskContextProvider: TTaskContextProvider;

    // ���� ��������� ������� �������� �������
    FIsMassCall: boolean;

    { ���������� �� �������� �����, ��������� ��� ������.
      �������� ������ ���������, ��������� � TSheetInterface.
      ���������������� ��������������� ����� ������������ ����� �
      ��� ������������ ��������� ����������.}
    FWritablesInfo: TWritablesInfo;

    SingleCellCanBePlacedHere: boolean;
    {������ ���� ������ ������� �����, ����������� �� ���� �� ���������.
      ����������� ��� ���������������� �� ������ ������� ��� ������ ��������.}
    SheetsOpened: TStringList;

    RebuildCycleState: TRebuildCycleState;

    function GetCommentForHistory(WizardRunMode: TWizardRunMode; ID: string): string;
    {���������� true ���� �������� ����������� ���������� ����������  ���
    ���������� �������}
    function EditableTotalSelected(ExcelSheet: ExcelWorksheet;
      Target: ExcelRange): boolean;

    procedure StoreSheetConnectionToRegistry;
    {������������ � �������}
    function TryConnection(Force: boolean): boolean;

    {������������� �����������, ��������� ������� � ���������� � ��������� ������}
    function PrepareRebuild(LoadMode: TLoadMode; out ExlSheet: ExcelWorkSheet): boolean;
    {����������� ������� � ��������������� ��������}
    procedure RebuildSheet(WizardRunMode: TWizardRunMode; ID: string); overload;
    {����������� ������� ��� �������������� (������ ������)}
    function RebuildSheet: boolean; overload;
    {���������������� ����������� �����. �������� ��� ������ ���� ������������}
    function DoRebuildSheet(ExcelSheet: ExcelWorksheet): boolean;
    {�������� ����� ���������� �����}
    procedure AfterRebuild(ExcelSheet: ExcelWorksheet; IsSuccess: boolean);

    { ���������� ���� ������ � �����( Quest 7309). ���������������,
      ��� ������� ������ ��� �������� ��������� ���������, ���������
      ������������� ��� ��������������, ������� ���������� ��������� ���������,
      ������� ������ �� ��, ��� ����� ������� ���� �� �������� ���������.}
    procedure CollectDeferredTotalsData;

    {���������� true ���� Target ������������ c ����� ����������� �����}
    function SheetComponentIntersect(ExcelSheet: ExcelWorkSheet;
      Target: ExcelRange): boolean;
    {������������ �������� � �������� �� �����(���������� �������� �����
    ����������� �� ������ ����������)}
    procedure NewRangeToBreaks(ExcelSheet: ExcelWorksheet; Target: ExcelRange);
    {��������� ������ �������� � ����������� � ������� ���������� �����}
    procedure RefreshSplitterPad;
    {����������� �������� ��������� ������ �� ����}
    procedure ProtectionAnalisis(const ExcelSheet: ExcelWorksheet;
      const Target: ExcelRange);
    {�������� ������ ������������ � ����� ��������}
    function GetErrorList: string;
    {��������� ����� ����������� � Excel-e}
    procedure DisableCopyMode;
    {���/���� ����� ���� "�������� ��������� ������"}
    procedure EnableSingleCellAppend;

    {�������������� �������������, ����� ��� ���������� ������ ����� ���������}
    procedure AncillaryInit;
    {���������� ������ Excel. ������������� ���� FExcelVerison}
    procedure RecognizeExcelVersion;
    { ��� ����� �������� ������ ������ � ������ ("���������������" ���
      "������ � �������") ��������� ��������� ������ � ������� ����}
    procedure UpdateWorkModeButton(AButton: CommandBarButton);
    { ������� �� "������ ������������", �������� ����� UpdateWorkModeButton}
    procedure UpdateStatusBar(ExcelSheet: ExcelWorkSheet);
    {��������� ������� � ��������� �����... ����� ����� �������� ��� ��
    ��������������� ��������, ����� ���� � ������� ���������� ������ �� ����
    VB ������, ��� ������ �� ����� �����������}
    procedure CalculateSheet(ExcelSheet: ExcelWorksheet);
    function TrySwitchWorkbookOffline: boolean;
    procedure OnTimerHandler(Sender: TObject);

    function GetProcessLogFileName: string;
    procedure UpdateProcessLogging;

    {��������� ��������� ������ �� ������� ����������� ������� ����}
    function CheckTotalsByType(CheckType: TSheetTotalType): boolean;
    function CheckTotalsForWriteback: boolean;
    function CheckTotalsForCollect: boolean;

    procedure SetAuthenticationInfoNew;
    procedure SetConnectionStrNew;
    procedure SetTaskConnectionLoadNew;

    function GetEnvironment: TTaskEnvironment;
    function GetStatic: boolean;
    function GetContext: TTaskContext;
    function GetActiveWorkBookName: string;

    {�������� ���������� � ����� ���� ����� �������� ����� ���� �����
      ��������� IFMPlaningInteraction. ����� ��������� ��� ���� ���������
      �� ���� ������ �����. ���� ����� ������������� ��������� *������*
      ���������� SetTaskContext}
    procedure ApplyTaskContext;
    procedure DoMassAction;
    procedure LoadContextFromTask(ShowGears: boolean);

    function CheckAxesSaved(ExcelSheet: ExcelWorkSheet): boolean;

    // ���������� �������� � ������ ��� �������� ����� ���������
    procedure OnProcessFormReturn;
    procedure OnProcessFormClose;
    procedure OnProcessFormShow;

    function LoadCatalogAndMetadata(LoadMode: TLoadMode; ESheet: ExcelWorkSheet): boolean;

    // �������� �����
    property Context: TTaskContext read GetContext;
    // ��� �������� �����
    property ActiveWorkBookName: string read GetActiveWorkBookName;
    property Environment: TTaskEnvironment read GetEnvironment;
    {����������� ����� �����������}
    property Static: boolean read GetStatic;

  protected

    {IDTExtensibility2}
    procedure BeginShutdown(var custom: PSafeArray); override; safecall;
    procedure OnAddInsUpdate(var custom: PSafeArray); override; safecall;
    procedure OnConnection(const HostApp: IDispatch; ext_ConnectMode: integer;
      const AddInInst: IDispatch; var custom: PSafeArray); override; safecall;
    procedure OnDisconnection(ext_DisconnectMode: integer;
      var custom: PSafeArray); override; safecall;
    procedure OnStartupComplete(var custom: PSafeArray); override; safecall;

    {����������� �����}
    procedure SheetActivate(const Sh: IDispatch); override;
    procedure BeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
      var Cancel: WordBool); override;
    procedure OnSheetChange(const Sh: IDispatch; const Target: ExcelRange); override;
    procedure OnSheetSelectionChange(const Sh: IDispatch; const Target: ExcelRange); override;
    {����������� �����}
    procedure WorkbookActivate(const Wb: ExcelWorkbook); override;
    procedure WorkbookDeactivate(const Wb: ExcelWorkbook); override;
    procedure WorkbookOpen(const Wb: ExcelWorkbook); override;
    procedure WorkbookBeforeSave(const Wb: ExcelWorkbook; SaveAsUI: WordBool; var Cancel: WordBool); override;
    procedure WorkbookBeforeClose(const Wb: ExcelWorkbook; var Cancel: WordBool); override;

    { ����������� }

    { �������� ������}
    procedure SendDataHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SendDataOptionalHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { ���������� ����������� � �����������}
    procedure ShowParamsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ShowConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure AddConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure EditConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure RefreshConstHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { ������� � ���������}
    procedure ConstructorWizardHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ComponentEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SingleCellEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SingleCellsManagerHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure OfflineFiltersEditorHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure DataCollectionFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure ReplicationHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    { ���������� �����}
    procedure RefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure RefreshHandler2(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure OneTotalRefreshHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    {��������������� �����}
    procedure ConnectionHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure EditStyleHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetHistoryHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SplitterPadHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure CopyFormHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetPropertiesHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure CommonOptionsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SheetInfoHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;


    {������}
    procedure MoveElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure DeleteElementHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure InsertNewLineHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure MarkEmptyHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SetTypeFormulaHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure WorkModeHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure SwitchTotalTypeHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure FreezePanesHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;
    procedure HideTotalColumnsHandler(Button: CommandBarButton; var CancelDefault: WordBool); override;

    {������ �� ������������� ������ �� ����� ���������}
    procedure CommandBarsUpdate; override;



    { IFMPlanningExtension }
    {�������� �������� ��������}
    function GetPropValueByName(const PropName: WideString): WideString; safecall;
    {���������� �������� ��������}
    procedure SetPropValueByName(const PropName: WideString; const PropValue: WideString); safecall;
    {��������� ������ ���������� � ��-������ ������������� ����� � ���������
    �����������}
    function SetConnectionStr(const URL, Scheme: WideString): HResult; safecall;
    {������� ������������\����������� ��������� � �������}
    procedure OnTaskConnection(IsConnected: WordBool); safecall;
    {OBSOLETE
      ��������� ��������� ���������� � ����������� �����}
    procedure SetTaskContext(const taskContext: IDispatch); overload; safecall;
    function  Get_IsSilentMode: WordBool; safecall;
    procedure Set_IsSilentMode(Value: WordBool); safecall;
    function  Get_ProcessForm: IProcessForm; safecall;
    procedure Set_ProcessForm(const Value: IProcessForm); safecall;
    function  Get_IsLoadingFromTask: WordBool; safecall;
    procedure Set_IsLoadingFromTask(Value: WordBool); safecall;
    property IsLoadingFromTask: WordBool read Get_IsLoadingFromTask write Set_IsLoadingFromTask;

    function RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; safecall;
    function RefreshActiveSheet: WordBool; safecall;
    function RefreshActiveBook: WordBool; safecall;

    function WritebackActiveSheet: WordBool; safecall;
    function WritebackActiveBook: WordBool; safecall;
    function WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;
    function WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;

    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString;
      const PwdHash: WideString); safecall;
    procedure AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType; 
                                   const Comment: WideString; Success: WordBool); safecall;
    { IFMAncillary }
    function QueryRange(const ACaption: WideString;
      const APrompt: WideString; AllowMultiArea: WordBool): IDispatch; safecall;
    { IFMPlanningVBProgramming }
    // ��������\������ �������� �����
    function VBGetPropertyByName(const PropertyName: WideString): WideString; safecall;
    procedure VBSetPropertyByName(const PropertyName: WideString; const PropertyValue: WideString); safecall;
    // ��������\������ ���������
    function VBGetConstValueByName(const ConstName: WideString): WideString; safecall;
    function VBSetConstValueByName(const ConstName: WideString;
      const ConstValue: WideString): WordBool; safecall;
    // ��������
    function VBRefresh: WordBool; safecall;
    // �������� ������
    function VBWriteback: WordBool; safecall;
    // �������� ������� ����������
    function VBGetCurrentConnection(var URL: WideString; var SchemeName: WideString): WordBool; safecall;
    // �������� ��������� �������� ���������
    // ���������� ��������� ������ [[UniqueName, Name]..[UniqueName, Name]]
    function VBGetMembers(const DimensionName: WideString): OleVariant; safecall;
    // ������ ��������� �������� ���������
    // ����� ��������� �������� ���������� �������� ���������� ����
    procedure VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant); safecall;
    // �������� �������� ���������
    // ���������� ��������� ������ [[UniqueName, Name]..[UniqueName, Name]]
    function VBGetParamMembers(const ParamName: WideString): OleVariant; safecall;
    // ������ �������� ���������
    // ����� ��������� �������� ���������� �������� ���������� ����
    procedure VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant); safecall;
    // ������������� �������� ��������� ����� �����
    function VBEditMembers(const DimensionName: WideString): WordBool; safecall;
    // �������� �������� �������� ���������, ������� �������� ���������� ������
    function VBGetMemberProperty(const DimensionName: WideString; const UniqueName: WideString;
                                 const MemberPropertyName: WideString): WideString; safecall;
    // �������� �������� ���������� � ������
    // ������ ������������ ������ ���������� � ��������� �������� [[Dimension, UniqueName]..[Dimension, UniqueName]]
    function VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString; safecall;
    // �������� �������� ���������� ����������
    function VBGetSingleCellValue(const SingleCellName: WideString): WideString; safecall;

    {}
    procedure SetTaskContext(const TaskContextXml: Widestring; IsPacked: WordBool); overload; safecall;

  end;

var
  {�� �� ��������?. ������������� �������� ����� ������� � ��� ���, ��� ��������
  �������������� ��������� ������� ���������� ���������� BeforeClose (���
  ������������ ��������), � ����� BeforeSave (��� �������� �����������)}
  IsCloseWorkbook: boolean;
  AssignedContext: boolean;

implementation
uses
  ComServ, SysUtils, Windows, uSheetStyles, uFMExcelAddInConst, uConverter;

{���������� ��������� ����������� ����� � ������}
procedure TFMExcelAddIn.StoreSheetConnectionToRegistry;
var
  Reg: TRegistry;
  OldUrl, OldSchemeName: string;
  ExlSheet: ExcelWorkSheet;
begin
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if not PlaningSheet.Load(ExlSheet, Context, [lmInner])then
    exit;
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if not Reg.OpenKey(RegBasePath + RegConnSection + regConnWebServiceSection, true) then
      exit;

    {���� ������ ���� � ������������, �������� �� ��������, �� ���������������
      ����� ����������� � �������. ���� �� ��� ����� ����, ��� ���
      �������� �����������, �� �������� ��� �������.}
    OldUrl := Reg.ReadString(regURLKey);
    if (PlaningSheet.URL <> '') then
      Reg.WriteString(regURLKey, PlaningSheet.URL)
    else
      if not Static then
        PlaningSheet.URL := OldUrl;
    OldSchemeName := Reg.ReadString(regWebServiceSchemeKey);
    if (PlaningSheet.SchemeName <> '') then
      Reg.WriteString(regWebServiceSchemeKey, PlaningSheet.SchemeName)
    else
      if not Static then
        PlaningSheet.SchemeName := OldSchemeName;

    if (PlaningSheet.URL <> '') and (PlaningSheet.SchemeName <> '') then
      if (OldUrl <> PlaningSheet.URL) or (OldSchemeName <> PlaningSheet.SchemeName) then
      begin
        DataProvider.ClearCache;
        DataProvider.Disconnect;
      end;
    Reg.CloseKey;
  finally
    Reg.Free;
    PlaningSheet.Clear;
  end;
end;

{ ������������ � �������
  ���������:
  Force - ���� ���� ������ �����������, ��� ����������� � ����� ������
  ��������� - ��� ���� ������������ ��� ���. }
function TFMExcelAddIn.TryConnection(Force: boolean): boolean;

var
  Silent, Enforced: boolean;
  URL, SchemeName, ErrorMsg: string;
  Op: IOperation;
  ExlSheet: ExcelWorkSheet;
begin
  result := false;

  {���� ��� ���������� - ��� ���-�� ������������}
  if not Assigned(DataProvider) then
  begin
    PlaningSheet.ShowMessageEX(ermUnknown, msgError);
    exit;
  end;

  StoreSheetConnectionToRegistry;

  {������������: ���� ���� ������ � ��������� ����� - ���������� �������� �����������}
  Silent := PlaningSheet.IsSilentMode or PlaningSheet.Process.Showing;

  {���� �������� � ��������� �����, �� ����� �������������� �� �����.}
  Enforced := Force or (NeedEvidentAuthentication and not Static);
  if not Static then
    Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    if not Static then
    begin
      Op.StartOperation(Host.Hwnd);
      Op.Caption := pcapConnect;
    end;

    result := Connect(DataProvider, Enforced, Silent, Static,
      FTaskAuthType, FTaskLogin, FTaskPwdHash, URL, SchemeName, ErrorMsg);
    // ���� ���� �� �������� ���������������, ���� �������� ��������� � �������
    NeedForceRefresh := result;

    { ���� ������������ � ����� ������ ���, �� �������� ������, �����
      ������������ ���������� ���-������� � ����, ����� ������ �� ��������
      �� �������.}
    if NeedEvidentAuthentication and DataProvider.Connected then
    begin
      Timer := TTimer.Create(nil);
      Timer.Interval := 180000; // 3 ������
      Timer.OnTimer := OnTimerHandler;
      NeedEvidentAuthentication := false;
    end;

  finally
    Application.ProcessMessages;
    if not Static then
      Op.StopOperation;
    SetActiveWindow(Host.Hwnd);
    Op := nil;
  end;

  {��� ����� �����������, ����� �������� ��� � ��������� ���������}
  if result then
  begin
    {����� ����������� ���� ��� ����� ������������ �����}
    PlaningSheet.URL := URL;
    PlaningSheet.SchemeName := SchemeName;
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if Assigned(ExlSheet) then
      if IsPlaningSheet(ExlSheet) or PlaningSheet.IsTaskConnectionLoad then
        if PlaningSheet.Load(ExlSheet, Context, [lmInner]) then
        try
          PlaningSheet.URL := URL;
          PlaningSheet.SchemeName := SchemeName;
          PlaningSheet.Save;
        finally
          PlaningSheet.Clear;
        end;

    {�������� ������� ������������� �����}
    DataProvider.ClearCache;
    if Assigned(FXMLCatalog) then
      FXMLCatalog.Clear;
    if Assigned(FfrmConstructorWizard) then
      FfrmConstructorWizard.Clear;
  end;

  result := DataProvider.Connected;
end;

procedure TFMExcelAddIn.BeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
  var Cancel: WordBool);
var
  WSheet: ExcelWorkSheet;
begin
  WSheet := GetWorkSheet(Sh);
  if not Assigned(WSheet) then
    exit;
  if VersionRelation <> svModern then
    exit;
  if not PlaningSheet.Load(WSheet, Context, lmNoFreeData + [lmNoMembers]) then
    exit;
  try
    inherited BeforeRightClick(Sh, Target, Cancel);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.BeginShutdown(var custom: PSafeArray);
begin
  inherited BeginShutdown(custom);
end;

procedure TFMExcelAddIn.OnAddInsUpdate(var custom: PSafeArray);
begin
  inherited OnAddInsUpdate(custom);
end;

procedure TFMExcelAddIn.OnConnection(const HostApp: IDispatch;
  ext_ConnectMode: Integer; const AddInInst: IDispatch;
  var custom: PSafeArray);
var
  Op: IOperation;
  InitialDelay, i: integer;
begin
  InitialDelay := ReadIntegerRegSetting(regInitialDelay, 0);
  for i := 1 to InitialDelay * 10 do
  begin
    sleep(100);
    Application.ProcessMessages;
  end;
  
  inherited OnConnection(HostApp, ext_ConnectMode, AddInInst, custom);

  Op := CreateComObject(CLASS_OPERATION) as IOperation;
  try
    Op.StartOperation(Host.Hwnd);
    Op.Caption := '������������� ����������';

    RecognizeExcelVersion;
    Application.Handle := Host.Get_Hwnd;
    // ����� ��������� ��������� � �� �������
    FProcess := CreateComObject(CLASS_ProcessForm) as IProcessForm;
    UpdateProcessLogging;
    FProcessFormSink := TProcessFormSink.Create;
    FProcessFormSink.OnReturn := OnProcessFormReturn;
    FProcessFormSink.OnClose := OnProcessFormClose;
    FProcessFormSink.OnShow := OnProcessFormShow;
    try
    FProcessFormSink.Connect(FProcess);
    except
      on e: exception do showerror(e.message);
    end;

    FfrmSheetHistory := TSheetHistory.Create(nil);
    FfrmConstructorWizard := nil;
    FSplitterPad := nil;
    FXMLCatalog := TXMLCatalog.Create;
    PlaningSheet := TPlaningSheet.Create;
    FTaskContextProvider := TTaskContextProvider.Create;
    FUserEvents := TExcelEvents.Create;
    PlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    PlaningSheet.Events := FUserEvents;
    FUserEvents.SetExternalLinks(FProcess);
    PlaningSheet.IsSilentMode := true;
    IsLoadingFromTask := false;
    AssignedContext := false;
    VersionRelation := svModern;
    // �������� ���� � ����������� �� ����� �����, ���� ��� � ������� ����
    CopyToHKCU;
    DestroyUserInterface;
    InitUserInterface;
    LoadToolBarPosition(ToolBar, regBasePath + regToolBarSettingsSection);
    LoadStatusBarPosition(StatusBar, regBasePath + regToolBarSettingsSection);
    FWritablesInfo := TWritablesInfo.Create;
    SheetsOpened := TStringList.Create;
    RebuildCycleState := rcsLeave;
  finally
    Application.ProcessMessages;
    Op.StopOperation;
    Op := nil;
  end;
end;

procedure TFMExcelAddIn.OnDisconnection(ext_DisconnectMode: integer;
  var custom: PSafeArray);
begin

  if not PlaningSheet.IsSilentMode then
  begin
    SaveToolBarPosition(ToolBar, regBasePath + regToolBarSettingsSection);
    SaveStatusBarPosition(StatusBar, regBasePath + regToolBarSettingsSection);
  end;
(*
  // ���� ������ �� ������������ AddIn �����������
  if Assigned(fHostAddIn) then begin
    // .. ����������� ��� ���������� ������ �� ��� ������
    try
      FHostAddIn.Object_ := nil;
    except
    end;
    // .. ����������� ������ �� ����
    FHostAddIn := nil;
    // ������ ����������� ������ ���
  end;*)
  DestroyUserInterface;
  FreeAndNil(FXMLCatalog);
  FreeAndNil(FfrmConstructorWizard);
  FreeAndNil(FSplitterPad);
  FreeAndNil(PlaningSheet);
  FreeAndNil(FfrmSheetHistory);
  FreeAndNil(FUserEvents);
//  FStatusBarRefreshDate := nil;
//  FStatusBarTaskID := nil;
//  Menu := nil;
//  FEmptyMenu := nil;
//  ToolBar := nil;
//  StatusBar := nil;
  FProcessFormSink.Disconnect;
  FProcessFormSink.Free;
  FProcess := nil;
//  DataProvider.FreeProvider;
//  DataProvider := nil;
  //FreeList(FTaskContextCollection);
  FTaskContextProvider.Free;
  FreeAndNil(FWritablesInfo);
  FreeAndNil(Timer);
  FreeStringList(SheetsOpened);

  inherited OnDisconnection(ext_DisconnectMode, custom);
end;

procedure TFMExcelAddIn.OnStartupComplete(var custom: PSafeArray);
begin
  inherited OnStartupComplete(custom); 
  {������ "�������� � ���������"}
  if Assigned(Host.ActiveWorkbook) then
  begin
    FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
    PlaningSheet.Environment := Environment;
  end;
  UpdateStatusBar(GetWorkSheet(Host.ActiveSheet));
  NeedEvidentAuthentication := true;
end;

{ ����������� }
{ ����������� � ������� }
procedure TFMExcelAddIn.ConnectionHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  if not TryConnection(true) then
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

procedure TFMExcelAddIn.CopyFormHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  IsNewBook: boolean;
begin
  DisableCopyMode;
  try
    CopySheet(Host, PlaningSheet, IsNewBook);
  finally
    UpdateNeed := IsNeedUpdateSheet(GetWorkSheet(Host.ActiveSheet), VersionRelation);
    { ��� ����������� ��������� ������ ����� ���������� ������� ����,
      �� ����, ��� � ���� ����� ���������� ��. ����� �������, �� ��������������
      �������� ��� ������, "�� ���" ����, � ��������������� �������� �������
      � ����. ��� ��� ��� � �������� �������, �� ���������� ��������� ���������.}
    PlaningSheet.SpecialFlagForTaskParamCopy := true;
    if IsNewBook then
      WorkbookOpen(Host.ActiveWorkbook);
    SheetActivate(Host.ActiveSheet);
    PlaningSheet.SpecialFlagForTaskParamCopy := false;
  end;
end;

procedure TFMExcelAddIn.SheetPropertiesHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
begin
  DisableCopyMode;
  FfrmProperties := TfrmProperties.Create(nil);

  try
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    FfrmProperties.ShowProperties(ExcelSheet, FXmlCatalog, DataProvider);
  finally
    SetActiveWindow(Host.Hwnd);
    FreeAndNil(FfrmProperties);
    UpdateProcessLogging;
  end;
end;

procedure TFMExcelAddIn.CalculateSheet(ExcelSheet: ExcelWorksheet);
const
  Automatic = -4105;
  Semiautomatic = 2;
var
  Calculation: integer;
begin
  if not IsPlaningSheet(ExcelSheet) then
    exit;
  PlaningSheet.OpenOperationEx(Host.Hwnd, pfoCalculateSheet, false, true, otUpdate);
  try
    Calculation := Host.Get_Calculation(LCID);

    if (Calculation = Automatic) or (Calculation = Semiautomatic) then
      {��� �������������� ������ ���������� ������, ����� �� ������� �� ��������
      ����� ���� ������� ������. ExcelSheet.Calculate(FLCID) - �� ��������, �.�.
      ��� ��� ������ Excel ������� ��� ������� � ���� ���������� �� ���������}
      ExcelSheet.Cells.Item[ExcelSheet.Rows.Count, ExcelSheet.Columns.Count].Insert(xlDown, EmptyParam)
    else
      ExcelSheet.Calculate(LCID);
  except
  end;
  PlaningSheet.CloseOperationEx(true, '');
end;

procedure TFMExcelAddIn.AfterRebuild(ExcelSheet: ExcelWorksheet; IsSuccess: boolean);
begin
  try
    if not Assigned(ExcelSheet) then
      exit;
    if not IsPlaningSheet(ExcelSheet) then
    begin
      PlaningSheet.Clear;
      exit;
    end;
    if IsSuccess then
    begin
      //����������. ���������� ����������� ������ ������������.
      RecreateCp(ExcelSheet);
      PlaningSheet.ClearCPGarbage(true);
      FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
    end;
    (* !!! ���������������� � ������ 14963  - ������� � ������ ��� ��������� ����������*)
    if RebuildCycleState = rcsLeave then
      PlaningSheet.Clear;
    //����� ������ �� ����� �����
    Application.ProcessMessages;
    //�������� ������
    if IsPlaningSheet(ExcelSheet)then
      ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  finally
    ExcelSheet.Application.ScreenUpdating[LCID] := true;
    if Assigned(FProcess) then
      if not FProcess.Showing then
        ExcelSheet.Application.Set_Interactive(LCID, true);
    ExcelSheet.Activate(LCID);
    SetActiveWindow(Host.Hwnd);
  end;
end;

function TFMExcelAddIn.GetCommentForHistory(WizardRunMode: TWizardRunMode; ID: string): string;
begin
  result := '';
  if (not Assigned(PlaningSheet)) or (Id = '') then
    exit;
  case WizardRunMode of
    wrmEditColumn: result := '������� �������� "' +
      PlaningSheet.Columns[PlaningSheet.Columns.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditRow: result := '������� ����� "' +
      PlaningSheet.Rows[PlaningSheet.Rows.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditFilter: result := '������� �������� "' +
      PlaningSheet.Filters[PlaningSheet.Filters.FindByID(ID)].FullDimensionName2 + '"';
    wrmEditTotal: begin
      with PlaningSheet.Totals[PlaningSheet.Totals.FindByID(ID)] do
        case TotalType of                               
          wtFree: result := '���������� "' + Caption + '"' + ' ���: ���������';
          wtConst: result := '���������� "' + Caption + '"' + ' ���: ���������';
          wtMeasure: result := '���������� "' + Caption + '"' + ' ���: ����,' +
            ' ���: "' + CubeName + '",' + ' ����: "' + MeasureName + '"';
          wtResult: result := '���������� "' + Caption + '"' + ' ���: ���������,' +
            ' ���: "' + CubeName + '",' + ' ����: "' + MeasureName + '"';
        end;
    end;
  end;
end;

function TFMExcelAddIn.GetErrorList: string;
begin
  result := FProcess.ErrorList;
end;

procedure TFMExcelAddIn.RebuildSheet(WizardRunMode: TWizardRunMode; ID: string);
var
  ExlSheet: ExcelWorkSheet;
  IsSuccess: Boolean;
  EventForHistory: string;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExlSheet) then
  begin
    repeat
      if RebuildCycleState = rcsReturn then
        LoadCatalogAndMetadata([lmCatalogOnly], ExlSheet);
      RebuildCycleState := rcsWait;
      try
        {����������� ������ - ���������� ��� ���� ��� �� �����}
        if not Assigned(FfrmConstructorWizard) then
        begin
          FfrmConstructorWizard := TfrmConstructorWizard.CreateEx(nil, PlaningSheet);
          FfrmConstructorWizard.XMLCatalog := FXMLCatalog;
          FfrmConstructorWizard.DataProvider := DataProvider;
        end;

          if FfrmConstructorWizard.RunWizard(WizardRunMode, ID) then
          begin
            AncillaryInit;
            FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, true);
            FProcess.NewProcessClear := false;

            FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"',
              not CriticalNode, NoteTime, otProcess);

            CollectDeferredTotalsData;

            PlaningSheet.NeedMapSingleCells := false;
            IsSuccess := DoRebuildSheet(ExlSheet);

            EventForHistory := MergeCommaText(FfrmConstructorWizard.EventList,
              GetErrorList);
            PlaningSheet.AddEventInSheetHistory(evtEdit, EventForHistory,
              IsSuccess);
          end
          else
            RebuildCycleState := rcsLeave;

      finally
        AfterRebuild(ExlSheet, IsSuccess);

        CalculateSheet(ExlSheet);
        if IsSuccess then
        begin
          FUserEvents.Execute(ExlSheet, enAfterRefresh);
          FProcess.CloseOperation; //pfoRebuild
        end;
        FProcess.CloseProcess;
      end;

      while RebuildCycleState = rcsWait do
      begin
        Application.ProcessMessages;
        Sleep(10);
      end;

    until RebuildCycleState = rcsLeave;
  end;
  PlaningSheet.Clear;
end;

function TFMExcelAddIn.RebuildSheet: boolean;
var
  ExlSheet: ExcelWorkSheet;
  IsSuccess: Boolean;
  HistoryText: string;
begin
  result := false;
  HistoryText := '';
  DisableCopyMode;
  IsSuccess := false;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if TryConnection(false) then
  try
    AncillaryInit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    try
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"', not CriticalNode, NoteTime, otProcess);
      if not IsPlaningSheet(ExlSheet) then
      begin
        result := true;
        exit;
      end;
      FUserEvents.Execute(ExlSheet, enBeforeRefresh);
      FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
      FXMLCatalog.SetUp(DataProvider);
      LoadContextFromTask(false);
      PlaningSheet.Clear;
      if not PlaningSheet.Load(ExlSheet, Context, lmAll) then
      begin
        if Assigned(Context) and (VersionRelation = svFuture) then
          FProcess.PostWarning('������ ����� ���� ������ ����������');
        FProcess.CloseOperation;
        exit;
      end;
      if PlaningSheet.Empty then
      begin
        FProcess.CloseOperation;
        result := true;
        exit;
      end;
      if not PlaningSheet.Online then
      begin
        FProcess.PostWarning('���� �������� ���������� ������ ����� � �� �������� ����������');
        result := true;
        FProcess.CloseOperation;
        exit;
      end;
    finally
      //����� ������ �� ����� �����
      Application.ProcessMessages;
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    IsSuccess := DoRebuildSheet(ExlSheet);
    result := IsSuccess;
                          
    if FIsMassCall then
      HistoryText := ConvertStringToCommaText('�������� �����');
    HistoryText := MergeCommaText(HistoryText, FProcess.ErrorList);
    if (HistoryText = '') and not PlaningSheet.NeedMapSingleCells then
      HistoryText := MergeCommaText(HistoryText, ConvertStringToCommaText('���������� �����'));
    PlaningSheet.AddEventInSheetHistory(evtRefresh, HistoryText, result);
  finally
    AfterRebuild(ExlSheet, result);
    CalculateSheet(ExlSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end
  else
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

function TFMExcelAddin.DoRebuildSheet(ExcelSheet: ExcelWorksheet): boolean;
var
  CheckFormulas, Alerts: boolean;
begin
  result := false;

  if not PlaningSheet.Refresh(NeedForceRefresh) then
    exit;
  if not SetSheetProtection(ExcelSheet, false) then
  begin
    FProcess.PostError(ermWorksheetProtectionFault);
    exit;
  end;
  if PlaningSheet.Validate then
  begin
    {������� ������, ��������� ���������������}
    ExcelSheet.Application.ScreenUpdating[LCID] := false;
    ExcelSheet.Application.Set_Interactive(LCID, false);

    if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
    begin
      FProcess.PostError(ermWorkbookProtectionFault);
      exit;
    end;
    PlaningSheet.ValidateStyles;
    MayHook := false;

    {� �������� ��������� ����� ������� ����� ��������� �������������,
    �������, ��� �� ��� ���� �� �������� ����������� ����, �������� ���������
    �������� ������������ ������}
    CheckFormulas := Host.ErrorCheckingOptions.EvaluateToError;
    Host.ErrorCheckingOptions.EvaluateToError := false;
    Alerts := Host.DisplayAlerts[LCID];
    Host.DisplayAlerts[LCID] := false;
    try
      PlaningSheet.MapAll;                          
    finally
      //��������������� �������� ������
      Host.ErrorCheckingOptions.EvaluateToError := CheckFormulas;
      Host.DisplayAlerts[LCID] := Alerts;
      //��������� �����������
      MayHook := true;
    end;

    if (FProcess.LastError = '') then
    begin
      PlaningSheet.LastRefreshDate := DateTimeToStr(Now);
      SetStatusBarInfo(sbiRefreshDate, PlaningSheet.LastRefreshDate);
      PlaningSheet.IsMarkupNew := true;
      PlaningSheet.Save;
      result := true;
    end;
  end;

  RefreshSplitterPad;
end;

{ ������ ��������� }
procedure TFMExcelAddIn.ConstructorWizardHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  WizardRunMode: TWizardRunMode;
  ID, DummyStr: string;
begin
  PlaningSheet.NeedMapSingleCells := true;
  if Button.Get_HelpFile = '' then
    RebuildSheet(wrmStandart, '')
  else
  begin
    ParseAdditionalInfo(Button.Get_HelpFile, WizardRunMode, ID, DummyStr);
    RebuildSheet(WizardRunMode, ID);
  end;
end;

{ ��������� ������ �� ������}
procedure TFMExcelAddIn.SendDataHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  WritebackActiveSheet;
end;

procedure TFMExcelAddIn.SendDataOptionalHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  NeedRewrite, NeedProcess: boolean;
begin
  if not OptionalWriteback(NeedRewrite, NeedProcess) then
    exit;
  WritebackActiveSheetEx(NeedRewrite, NeedProcess);
end;

function TFMExcelAddIn.RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; safecall;
var
  Sheet: ExcelWorksheet;
  IsVisible: XlSheetVisibility;
begin
  result := false;
  try
    Sheet := GetWorkSheet(Host.Sheets.Item[Index]);
    if not Assigned(Sheet) then
      exit;
    IsVisible := Sheet.Visible[LCID];
    try
      {$WARNINGS OFF}
      Sheet.Visible[LCID] := xlSheetVisible;
      {$WARNINGS ON}
      Sheet.Activate(LCID);
      PlaningSheet.NeedMapSingleCells := true;
      result := RebuildSheet;
    finally
      Sheet.Visible[LCID] := IsVisible;
    end;
  except
    result := false;
    IsAccessVioletion := true;
  end;
end;

function TFMExcelAddIn.RefreshActiveSheet: WordBool;
var
  IsAV: WordBool;
  Sheet: ExcelWorksheet;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  result := RefreshSheet(Sheet.Name, IsAV);
end;

function TFMExcelAddIn.RefreshActiveBook: WordBool;
var
  Sheet: ExcelWorksheet;
  Book: ExcelWorkbook;
  i: integer;
  IsAV: WordBool;
  ActiveSheet:ExcelWorksheet;
begin
  FIsMassCall := true;
  Book := Host.ActiveWorkbook;
  result := Assigned(Book);
  if result then
  begin
    ActiveSheet := GetWorkSheet(Book.ActiveSheet);
    for i := 1 to Book.Sheets.Count do
    begin
      FProcess.ClearErrors;
      Sheet := GetWorkSheet(Book.Sheets.Item[i]);
      if not Assigned(Sheet) then
        continue;
      result := RefreshSheet(i, IsAV) and result;
    end;
    if Assigned(ActiveSheet) then
      ActiveSheet.Activate(LCID);
  end;
  FIsMassCall := false;
end;

function TFMExcelAddIn.WritebackActiveBook: WordBool;
begin
  result := WritebackActiveBookEx(false, true);
end;

function TFMExcelAddIn.WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool;
var
  Sheet: ExcelWorksheet;
  Book: ExcelWorkbook;
  i: integer;
  IsVisible: XlSheetVisibility;
  ActiveSheet: ExcelWorksheet;
begin
  FIsMassCall := true;
  Book := Host.ActiveWorkbook;
  result := Assigned(Book);
  if result then
  begin
    ActiveSheet := GetWorkSheet(Book.ActiveSheet);
    for i := 1 to Book.Sheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Sheets.Item[i]);
      if not Assigned(Sheet) then
        continue;
      IsVisible := Sheet.Visible[LCID];
      try
        {$WARNINGS OFF}
        Sheet.Visible[LCID] := xlSheetVisible;
        {$WARNINGS ON}
        Sheet.Activate(LCID);
        result := WritebackActiveSheetEx(EraseEmptyCells, ProcessCube) and result;
      finally
        Sheet.Visible[LCID] := IsVisible;
      end;
    end;
    if Assigned(ActiveSheet) then
      ActiveSheet.Activate(LCID);
  end;
  FIsMassCall := false;
end;

function TFMExcelAddIn.WritebackActiveSheet: WordBool;
begin
  WritebackActiveSheetEx(false, true);
end;

function TFMExcelAddIn.WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool;
var
  TotalsDataDocument: IXMLDOMDocument2;
  ExlSheet: ExcelWorkSheet;
  InnerXml, FilterName, ErrorContent, SingleCellName: string;
  Error, TaskId: widestring;
  ErrorList, HistoryList: TStringList;
  IsSuccess: boolean;
  BatchNode: IXMLDOMNode;

  // �������� �� �������������� ��������� ��������
  function CheckFilters(var FilterName, Error, SingleCellName: string): boolean;
  var
    i, j: integer;
    Filters: TSheetFilterCollectionInterface;

    function CheckFilter(Filter: TSheetFilterInterface): boolean;
    begin
      CutAllInvisible(Filter.Members, true);  // ������������� ��-�� 13918
      FilterName := Filter.FullDimensionName;
      result := Filter.CheckForWriteback(Error);
    end;

  begin
    result := false;
    Filters := PlaningSheet.Filters;
    for i := 0 to Filters.Count - 1 do
    begin
      // ������� �� ��������� ���� �� �������������
      if not Filters[i].OfPrimaryProvider then
        continue;
      // ������ ������ ����������� �� ������� ���� ���� ���������� - ���������
      if (Filters[i].TotalTypesAffected and ttResult) <> ttResult then
        continue;
      if not CheckFilter(Filters[i]) then
        exit;
    end;
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      if not PlaningSheet.SingleCells[i].OfPrimaryProvider then
        continue;
      if (PlaningSheet.SingleCells[i].TotalType <> wtResult) then
        continue;
      Filters := PlaningSheet.SingleCells[i].Filters;
      SingleCellName := PlaningSheet.SingleCells[i].Name;
      for j := 0 to Filters.Count - 1 do
        if not CheckFilter(Filters[j]) then
          exit;
    end;
    result := true;
  end;

  function CheckDimensions(var Error: string): boolean;
  var
    i: integer;
  begin
    Error := '';
    with PlaningSheet do
    begin
      for i := 0 to Rows.Count - 1 do
        if not Rows[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Rows[i].FullDimensionName + '"';
        end;
      for i := 0 to Columns.Count - 1 do
        if not Columns[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Columns[i].FullDimensionName + '"';
        end;
      for i := 0 to Filters.Count - 1 do
        if not Filters[i].OfPrimaryProvider then
        begin
          AddTail(Error, ', ');
          Error := Error + '"' + Filters[i].FullDimensionName + '"';
        end;
    end;
    result := Error = '';
    if not result then
      Error := Format(ermWritebackBadDimensions, [Error]);
  end;

  // �������� ������ � ������� ����� ������
  procedure GetErrorMessage(TotalsDataDocument: IXmlDomDocument2;
                            var Error: widestring; var ErrorList: TStringList);
  var
    XPath: string;
    DomElement: IXmlDomNode;
    StackTrace, Source, ErrorId: string;
    Caption: string;
    Index: integer;
  begin
    XPath := 'Exception/RequestID';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      ErrorId := DomElement.text;
      Index := PlaningSheet.Totals.FindById(ErrorId);
      if (Index <> -1) then
        Caption := '���������� "' + PlaningSheet.Totals[Index].Caption + '"'
      else
      begin
        Index := PlaningSheet.SingleCells.FindById(ErrorId);
        if (Index <> -1) then
          Caption := '��������� ���������� "' + PlaningSheet.SingleCells[Index].Name + '"';
      end;
      ErrorList.Add('��������� �������, �� ������� ��������� ������:');
      ErrorList.Add(Caption);
    end;
    XPath := 'Exception/Message';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      Error := DomElement.text;
      ErrorList.Add(Error);
    end;
    XPath := 'Exception/Source';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      Source := DomElement.text;
      ErrorList.Add('������, ��������� ������:');
      ErrorList.Add(Source);
    end;
    XPath := 'Exception/StackTrace';
    DomElement := TotalsDataDocument.selectSingleNode(XPath);
    if Assigned(DomElement) then
    begin
      StackTrace := DomElement.text;
      ErrorList.Add('������ ��������� �� ������:');
      ErrorList.Add(StackTrace);
    end;  
  end;

  // �������� ��������� ����� "�������" - �������� �� �� �������
  procedure ClearErasedCells;
  var
    ExcelSheet: ExcelWorkSheet;
    i, k, j: integer;
    TotalRange, CellRange: ExcelRange;

    // �������� ����� ������
    procedure ChangeStyle(CellRange: ExcelRange; NewStyle: string);
    var
      Style: variant;
      NumberFormat: variant;
    begin
      Style := CellRange.Style;
      // ���������� ��������� ������ "���������"
      if (string(Style) = snDataFreeErased) then
      begin
        NumberFormat := CellRange.NumberFormat;
        CellRange.Style := NewStyle;
        CellRange.NumberFormat := NumberFormat;
        CellRange.Value2 := fmEmptyCell;
        CellRange.Formula := '';
      end;
    end;

  begin
    ExcelSheet := PlaningSheet.ExcelSheet;
    // ������� ������ �����������
    for i := 0 to PlaningSheet.Totals.Count - 1 do
    begin
      if (PlaningSheet.Totals[i].TotalType <> wtResult) then
        continue;
      for k := 0 to PlaningSheet.Totals[i].SectionCount - 1 do
      begin
        TotalRange := PlaningSheet.Totals[i].GetTotalRange(k);
        for j := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
        begin
          with ExcelSheet do
            CellRange := Range[Cells.Item[j, TotalRange.Column],
                               Cells.Item[j, TotalRange.Column]];
          ChangeStyle(CellRange, snDataFree);
        end;
      end;
    end;
    // ������� ��������� ������
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      if (PlaningSheet.SingleCells[i].TotalType <> wtResult) then
        continue;
      CellRange := GetRangeByName(ExcelSheet, PlaningSheet.SingleCells[i].ExcelName);
      ChangeStyle(CellRange, snResultSingleCells);
    end;
  end;
var
  BatchID: string;
begin
  result := false;
  IsSuccess := false;
  DisableCopyMode;
  MayHook := false;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if not Assigned(Context) then
    if not TryConnection(false) then
    begin
      PlaningSheet.ShowDetailedError(ermNoneConnection,
        DataProvider.LastError, ermNoneConnection);
      exit;
    end;
  AncillaryInit;
  FProcess.OpenProcess(Host.Hwnd, ftWriteback, mWritebackSuccess, mWritebackError, false);
  FProcess.NewProcessClear := false;
  HistoryList := TStringList.Create;
  try
    FProcess.OpenOperation(pfoWriteback + ' "' + ExlSheet.Name +'"',
      not CriticalNode, NoteTime, otProcess);
    if FIsMassCall then
      HistoryList.Add('�������� �����');
    // ���������� �� ���� �����
    if not IsPlaningSheet(ExlSheet) then
    begin
      result := true;
      FProcess.CloseOperation;  //pfoWriteback
      exit;
    end;

    { ���� ���� �� �� ����� - ��������� �������� ������}
    //TaskId := GetPropValueByName(pspTaskId);
    TaskId := Environment.TaskId;
    if (TaskId = '') then
    begin
      FProcess.PostError(ermWritebackNoTaskId);
      HistoryList.Add(ermWritebackNoTaskId);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;


    {���������� �����������}
    if not DataProvider.Connected then
      TryConnection(false);
    if not DataProvider.Connected then
    begin
      FProcess.PostError(ermNoneConnection + ': ' + DataProvider.LastError);
      exit;
    end;

    { �� ������ ���� "�����" �������� ������ ��������� �� ����}
    if (GetPropValueByName(pspSheetType) = '2') then
    begin
      result := true;
      FProcess.PostInfo('��� ��������� ���� "�����" �������� ������ �� �����������');
      FProcess.CloseOperation;  //pfoWriteback
      exit;
    end;

    { �������� ����������}
    FUserEvents.Execute(ExlSheet, enBeforeWriteBack);
    FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
    try
      FXMLCatalog.SetUp(DataProvider);
      if not PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
      begin
        FProcess.CloseOperation;
        exit;
      end;
      if PlaningSheet.Empty then
      begin
        result := true;
        FProcess.CloseOperation;
        exit;
      end;
    finally
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    // ������� �������� �� ��������� �� ���������� ����
    if not CheckDimensions(ErrorContent) then
    begin
      FProcess.PostError(ErrorContent);
      HistoryList.Add(ErrorContent);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;

    // ��������� �� ������� ����������� ���� ���������
    if (not CheckTotalsForWriteback) then
    begin
      FProcess.PostError(ermWritebackNoResultTotals);
      HistoryList.Add(ermWritebackNoResultTotals);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;

    // ��������� �� �������������� �������� ��������
    if (not CheckFilters(FilterName, ErrorContent, SingleCellName)) then
    begin
      if (SingleCellName <> '') then
        ErrorContent := '��������� ������ "' + SingleCellName + '": ' + ErrorContent;
      ErrorContent := Format(ErrorContent, ['"' + FilterName + '"']);
      FProcess.PostError(ErrorContent);
      HistoryList.Add(ErrorContent);
      PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
      exit;
    end;
    // �������� ������ ������
    FProcess.OpenOperation(pfoCollectWritebackData, CriticalNode, NoteTime, otProcess);
    try
      // �������� �������� � �������
      TotalsDataDocument := PlaningSheet.GetWritebackData(TaskId, EraseEmptyCells, ProcessCube);
      if AddinLogEnable then
        if (not WriteDocumentLog(TotalsDataDocument, '������ ��� �������� ������.xml')) then
          FProcess.PostWarning(ermDocumentLogSaveFault);
    finally
      FProcess.CloseOperation;
    end;

    try
      if (TotalsDataDocument = nil) then
        exit;
      // �������� �� ����������� ������
      if (TotalsDataDocument.documentElement.childNodes.length = 0) then
      begin
        FProcess.PostError(ermWritebackNoData);
        HistoryList.Add(ermWritebackNoData);
        PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, false);
        exit;
      end;
      // ���������� �� ������
      InnerXml := TotalsDataDocument.xml;
       (*��� ����������� ������ ����� �� ����� - ��������� ����������� ����
          Xml � ������ CDATA*)
      //XmlEncode(InnerXml);
      if not Assigned(DataProvider) then
        exit;
      FProcess.OpenOperation(pfoWriteback, CriticalNode, NoteTime, otProcess);
      try
        InnerXml := DataProvider.Writeback(InnerXml);
      finally
        FProcess.CloseOperation;
      end;

      TotalsDataDocument.loadXML(InnerXml);
      result := not Assigned(TotalsDataDocument.selectSingleNode('//Exception'));
      if result then
      begin
        // �������� ������ ������ �������
        // ���������� ��������� ����� "�������" - �������� �� �� �������
        BatchNode := TotalsDataDocument.selectSingleNode('//BatchID');
        if Assigned(BatchNode) then
          BatchID := BatchNode.text
        else
          BatchID := '�/�';
        FProcess.OpenOperation(pfoWritebackComplete + '. �����: ' + BatchID, CriticalNode, NoteTime, otProcess);
        try
          ClearErasedCells;
        finally
          FProcess.CloseOperation;
          HistoryList.Add('�����: ' + BatchID);
          HistoryList.Add(Format('������ ���� %s��������; ������ ����� %s',
            [IIF(EraseEmptyCells, '����', '��'), IIF(ProcessCube, '��������.', '�� ��������.')]));
          PlaningSheet.AddEventInSheetHistory(evtWriteBack, HistoryList.CommaText, true);
          {������������� ��� ����, ����� ��������� ������� MultiplicationFlag}
          PlaningSheet.Save([lmInner]);
        end;
        IsSuccess := true;
      end
      else
      begin
        // ��� �������� ������ ��������� ������
        ErrorList := TStringList.Create;
        try
          if FIsMassCall then
            ErrorList.Add('�������� �����');
          GetErrorMessage(TotalsDataDocument, Error, ErrorList);
          PlaningSheet.AddEventInSheetHistory(evtWriteBack, ErrorList.CommaText, false);
          FProcess.PostError(Error);
        finally
          FreeStringList(ErrorList);
        end;
      end;
    finally
      KillDomDocument(TotalsDataDocument);
    end;
  finally
    MayHook := true;
    AfterRebuild(ExlSheet, result);
    FreeStringList(HistoryList);
    CalculateSheet(ExlSheet);
    //���� �������� ������ ������ ������
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterWriteBack);
      FProcess.CloseOperation;  //pfoWriteback
    end;
    FProcess.CloseProcess;
  end;
end;

{ �������� ������ }
procedure TFMExcelAddIn.RefreshHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  PlaningSheet.NeedMapSingleCells := true;
  RebuildSheet;
end;

procedure TFMExcelAddIn.RefreshHandler2(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  PlaningSheet.NeedMapSingleCells := false;
  RebuildSheet;
end;

procedure TFMExcelAddin.WorkbookActivate(const Wb: ExcelWorkbook);
var
  ExcelSheet: ExcelWorkSheet;
  tmpStr: string;
  Editable: boolean;
  Range: ExcelRange;
  SheetName: string;
begin
  { ��� ����� �������� ����� ������ ����� ��������. ������� � 6975.}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      FSplitterPad.Hide;
  UpdateStatusBar(GetWorksheet(Host.ActiveSheet));
  SetButtonsState(true, false);
  IsCloseWorkbook := false;
  if not VersionOK then
    exit;
  FWritablesInfo.Clear;
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  {������ "�������� � ���������"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;
  if IsPlaningSheet(ExcelSheet)then
  begin
    PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData);
    try
      FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
      UpdateWorkModeButton(nil);
    finally
      PlaningSheet.Clear;
    end;
    {���� ���� ��������� � ������ ���, �� �������� � ������ �������}
    SheetName := Wb.FullName[LCID] + ExcelSheet.Name;
    if SheetsOpened.IndexOf(SheetName) < 0 then
    begin
      SheetsOpened.Add(SheetName);
      Range := GetRangeByName(ExcelSheet, BuildExcelName(sntTable));
      if Assigned(Range) then
        Range.Cells.Item[1, 1].Activate;
    end;
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  end
  else
  begin
    PlaningSheet.SetDefaultPermissions;
    SingleCellCanBePlacedHere := IsSingleCellAllowed(ExcelSheet, ExcelSheet.UsedRange[LCID],
      FWritablesInfo, tmpStr, Editable);
    if not PlaningSheet.InConstructionMode then
      SingleCellCanBePlacedHere := SingleCellCanBePlacedHere and Editable;
    UpdateWorkModeButton(nil);
  end;
  StoreSheetConnectionToRegistry;
  UpdateStatusBar(GetWorksheet(Host.ActiveSheet));
end;

procedure TFMExcelAddin.WorkbookDeactivate(const Wb: ExcelWorkbook);
begin
  if Wb.IsAddin then
    exit;
  if IsCloseWorkBook then
  begin
    SetButtonsState(false, false);
    SetStatusBarInfo(sbiRefreshDate, '');
    SetStatusBarInfo(sbiTaskID, '');

    AssignedContext := not wb.Saved[LCID] and Assigned(Context);
    FTaskContextProvider.OnWorkbookClose(Wb);
  end;
end;

procedure TFMExcelAddIn.WorkbookOpen(const Wb: ExcelWorkbook);
var
  ExlSheet: ExcelWorkSheet;
  CurrBookCode, NormalBookCode: Longword;
  i: integer;
begin
  {�� ����� ����������� �������� � ����� �� ��������� �������
    1 - ����� �������� �����������
    2 - ���������� ������������ � ������
    3 - ���������� �����
    4 - ����������� ����� "������" ����}
  if Wb.IsAddin then
    exit;
  SheetsOpened.Clear;
  IsCloseWorkbook := false;

  {������ "�������� � ���������"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;

  {�������� ������� ����� ������� ������ (IsAddin), �.�. ��� ��������� � ��������
  FileFormat ����������� ���������� DoWindowDeactive ��� ������ � ������ ��������
  � ������ 6164}
  NormalBookCode := xlWorkbookNormal;
  CurrBookCode := Wb.FileFormat[LCID];

  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  PlaningSheet.ExcelSheet := ExlSheet;


  {���������� ������ ������ �������������� ���� ��������� ���������.
    � ����� ������ ��������� ��� ����� �����, � ���������, ������ �������}
  VersionOk := true;
  if PlaningSheet.IsSilentMode then
    for i := 1 to wb.Worksheets.Count do
      VersionOK := CheckSheetVersion(GetWorkSheet(wb.Worksheets[i]), DataProvider, false, FProcess)
  else
    VersionOK := CheckSheetVersion(GetWorkSheet(Host.ActiveSheet), DataProvider, true, FProcess);


  if IsLoadingFromTask then
  begin
    SetPropValueByName(pspDocPath, ActiveWorkBookName);
    SetAuthenticationInfoNew;
    SetTaskConnectionLoadNew;
    if not PlaningSheet.IsSilentMode then
    begin
      Set_IsSilentMode(true);
      SetConnectionStrNew;
      Set_IsSilentMode(false);
    end
    else
      SetConnectionStrNew;
    ApplyTaskContext;
  end;

  if IsLoadingFromTask then
    DoMassAction;

  if IsLoadingFromTask or PlaningSheet.IsSilentMode or not
    ((CurrBookCode = NormalBookCode) or
    (CurrBookCode = 56)) then  // ��������������� ��������� � ��� �� �� ���
    exit;

  UpdateNeed := IsNeedUpdateSheet(ExlSheet, VersionRelation);
end;

procedure TFMExcelAddin.WorkbookBeforeClose(const Wb: ExcelWorkbook; var Cancel: WordBool);
var
  i: integer;
  SheetName: string;
  Sheet: ExcelWorkSheet;
  Index: integer;
begin
  if Wb.IsAddin then
    exit;
  IsCloseWorkbook := true;

  for i := 1 to Wb.Sheets.Count do
  begin
    Sheet := GetWorkSheet(Wb.Sheets[i]);
    if not Assigned(Sheet) then
      continue;
    SheetName := Wb.FullName[LCID] + Sheet.Name;
    Index := SheetsOpened.IndexOf(SheetName);
    if Index > -1 then
      SheetsOpened.Delete(Index);
  end;
end;

function TFMExcelAddin.SheetComponentIntersect(ExcelSheet: ExcelWorkSheet;
  Target: ExcelRange): boolean;
begin
  result := false;
  if not Assigned(ExcelSheet) then
   exit;
   result :=
    (IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntColumnsAndMPropsArea))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntColumnTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntRows))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntRowTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntTotals))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntTotalTitles))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntUnitMarker))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntFilterArea))) or
    IsRangesIntersect(ExcelSheet, Target, GetRangeByName(ExcelSheet,
      BuildExcelName(sntSheetId))));
end;

procedure TFMExcelAddin.NewRangeToBreaks(ExcelSheet: ExcelWorksheet; Target: ExcelRange);

  function AddRangeToName(Range: ExcelRange; Name: string): boolean;
  var
    NameRange: ExcelRange;
    TopIndexBreak, BottomIndexBreak: integer;
    TopIndexRange, BottomIndexRange: integer;
  begin
    result := false;
    if (not Assigned(Range)) or (Name = '') then
      exit;
    NameRange := GetRangeByName(ExcelSheet, BuildExcelName(Name));
    if not Assigned(NameRange) then
      exit;
    TopIndexBreak := NameRange.Row;
    BottomIndexBreak := NameRange.Row + NameRange.Rows.Count;

    TopIndexRange := Range.Row;
    BottomIndexRange := Range.Row + Range.Rows.Count;

    if TopIndexBreak =  BottomIndexRange then
    begin
      MayHook := false;
      Range.ClearFormats;
      NameRange := GetRange(ExcelSheet, TopIndexRange, 1, BottomIndexBreak - 1, ExcelSheet.Columns.Count);
      MarkObject(ExcelSheet, NameRange, Name, false);
      MayHook := true;
      result := true;
    end;
  end;

begin
  if not Assigned(ExcelSheet) then
    exit;
  //������� ����� �� ������������ ����� ����������� �������� � �������� �� �����
  if ((IntersectTableAndRange(ExcelSheet, Target)) and
    (not BreakPointSelected(ExcelSheet, Target)) and
    (not SheetComponentIntersect(ExcelSheet, Target))) then
  begin
    if (not AddRangeToName(Target, sntFiltersBreak)) then
      if (not AddRangeToName(Target, sntUnitMarkerBreak)) then
        if (not AddRangeToName(Target, sntColumnTitlesBreak)) then
          if (not AddRangeToName(Target, sntColumnsBreak)) then
            if (not AddRangeToName(Target, sntRowTitlesBreak)) then
              AddRangeToName(Target, sntRowsBreak);
  end;
end;

//��������� ������ �������� � ����������� � ������� ���������� �����.
procedure TFMExcelAddin.RefreshSplitterPad;
begin
  {������ �������� ����� ������ �� ���� ��� ��� ����� ���� ������}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      if Assigned(PlaningSheet) then
        FSplitterPad.Init(PlaningSheet);
end;

procedure TFMExcelAddin.OnSheetChange(const Sh: IDispatch;
  const Target: ExcelRange);
var
  tmpIndex: integer;
  Name_: ExcelXP.Name;
  ObjType: string;
  CellValue: widestring;
  ExcelSheet: ExcelWorksheet;
  RangeName: widestring;
  i: integer;
  NameFound: boolean;
  Params: TStringList;
  Formula: string;
begin
  // ���� ��� ���������� - �������
  if not MayHook then
    exit;
  ExcelSheet := GetWorkSheet(Sh);
  if not Assigned(ExcelSheet) then
    exit;
  NewRangeToBreaks(ExcelSheet, Target);
  NameFound := false;
  {���������, � ������ ������ ��������� ����������� ����.
  ��� ���������� ��������� ����������� � �������� "��������"}
  for i := 1 to ExcelSheet.Names.Count - 1 do
  begin
    Name_ := ExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
    if NameWithoutRange(Name_) then
      continue;
    if IsNestedRanges(Target, Name_.RefersToRange) then
    begin
      RangeName := Name_.Name_;
      tmpIndex := Pos('!', RangeName);
      RangeName := Copy(RangeName, tmpIndex + 1, Length(RangeName) - tmpIndex);
      // �������� ��� �� ����
      if not ParseExcelName(RangeName, Params) then
        continue;
      try
        try
          ObjType := Params[0];
          // ������������ ������ ���������� � ��������� ������
          if not ((ObjType = sntTotalResult) or (ObjType = sntSingleCellMeasure) or
                  (ObjType = sntSingleCellResult)) then
            continue;
          // ����� ������ ���
          NameFound := true;
          break;
        finally
          FreeAndNil(Params);
        end;
      except
      end;
    end;
  end;
  if not NameFound then
    exit;
  Formula := Target.Cells.Item[1, 1].Formula;
  if Pos('=', Formula) = 1 then
    exit; //������� ������� �� ����
  // �������� �������� ������
  CellValue := Target.Cells.Item[1, 1].Value;
  // ���� �� ������� � �� ������ ������, ���������, ����� �� ���?
  if (CellValue <> fmEmptyCell) then
  begin
    try
      StrToFloat(CellValue);
    except
      // �� ����� - ��������
      ShowError(ermIncorrectCellValue + ' (' + CellValue + ')');
      Target.Cells.Item[1, 1].Value[EmptyParam] := fmEmptyCell;
      exit;
    end;
  end;
end;

// ���������� ���������� IFMPlanningExtension
// ��������� �������� �������� �� �����
function  TFMExcelAddin.GetPropValueByName(const PropName: WideString): WideString;
begin
  // ����� ����� �������� � ������� ��� ��������
  // ���� �������� ��� - ������ ������
  try
    result := GetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, PropName);
  except
    result := '';
  end;
  if (PropName = pspSheetType) and (result = '') then
    result := 'null';
end;

// ��������� ��������
procedure TFMExcelAddin.SetPropValueByName(const PropName: WideString;
  const PropValue: WideString);
begin
  // ����� �������� �� �����
  // ���� ������ ��� - �������
  // ���������� ��������
  try
    SetWBCustomPropertyValue(Host.ActiveWorkbook.CustomDocumentProperties, PropName, PropValue);
  except
  end;
end;

procedure TFMExcelAddin.CommandBarsUpdate;
var
  cmdBars: CommandBars;
  cmdBarStandard: CommandBar;
  btnOpen, btnNew: CommandBarControl;
  Enabled: boolean;
begin
  cmdBars := Host.CommandBars;
  try
    cmdBarStandard := cmdBars['standard'];
    btnNew := cmdBarStandard.FindControl(msoControlButton, 18, EmptyParam,
      EmptyParam, EmptyParam);
    btnOpen := cmdBarStandard.FindControl(msoControlButton, 23, EmptyParam,
      EmptyParam, EmptyParam);
  except
  end;
  {��������� ��� ������ ��� ��������������� ������� ����, ��� �� ����� �����}
  if (Host.Workbooks.Count = 0) then
    Enabled := false
  else
    if Assigned(btnOpen) then
      Enabled := btnOpen.Enabled
    else
      if Assigned(btnNew) then
        Enabled := btnNew.Enabled
      else
        Enabled := true;
  SetButtonsState(Enabled, true);
end;

procedure TFMExcelAddIn.SheetActivate(const Sh: IDispatch);
var
  ExlSheet: ExcelWorkSheet;
  Range: ExcelRange;
  SheetName: string;
begin
  if PlaningSheet.NeedHostEventsDisabled then
    exit;
  FWritablesInfo.Clear;
  { ��� ����� ��������� ����� ������ ����� ��������. ������� � 6975.}
  if Assigned(FSplitterPad) then
    if FSplitterPad.Visible then
      FSplitterPad.Hide;
  IsCloseWorkbook := false;
  ExlSheet := GetWorkSheet(Sh);
  UpdateStatusBar(ExlSheet);
  if not Assigned(ExlSheet) then
    exit;

  {������ "�������� � ���������"}
  FTaskContextProvider.OnWorkbookOpen(Host.ActiveWorkbook);
  PlaningSheet.Environment := Environment;


  if CheckSheetVersion(ExlSheet, DataProvider, (not PlaningSheet.IsSilentMode),
    FProcess) then
      if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
        try
          FWritablesInfo.CopyFrom(PlaningSheet.WritablesInfo);
          UpdateWorkModeButton(nil);
          if PlaningSheet.SpecialFlagForTaskParamCopy then
            PlaningSheet.Save;
        finally
          PlaningSheet.Clear;
        end
      else
      begin
        PlaningSheet.SetDefaultPermissions;
        UpdateWorkModeButton(nil);
      end;
  UpdateStatusBar(ExlSheet);
  StoreSheetConnectionToRegistry;
  if IsPlaningSheet(GetWorkSheet(ExlSheet))then
  begin
    {���� ���� ��������� � ������ ���, �� �������� � ������ �������}
    SheetName := (ExlSheet.Parent as ExcelWorkbook).FullName[LCID] + ExlSheet.Name;
    if SheetsOpened.IndexOf(SheetName) < 0 then
    begin
      SheetsOpened.Add(SheetName);
      Range := GetRangeByName(ExlSheet, BuildExcelName(sntTable));
      if Assigned(Range) then
        Range.Cells.Item[1, 1].Activate;
    end;
    ProtectionAnalisis(GetWorkSheet(ExlSheet), Host.ActiveCell);
  end;
end;

procedure TFMExcelAddIn.SplitterPadHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExlSheet: ExcelWorkSheet;
begin
  {���� �������� ��� �� ������� (������ �����), ����� �������}
  if not Assigned(FSplitterPad) then
  begin
    FSplitterPad := TfrmSplitterPad.Create(nil);
    FSplitterPad.ParentWindow := Host.Hwnd;

    {���������������� �� ����}
    FSplitterPad.Top := 200;
    FSplitterPad.Left := round(Host.ActiveWindow.Width) - 100;
  end;

  {���� ��� ����� - ������, ���� �� ����� - ����������}
  if FSplitterPad.Visible then
    FSplitterPad.Hide
  else
  begin
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExlSheet) then
      exit;
    if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
    try
      FSplitterPad.Init(PlaningSheet);
      if not FSplitterPad.Visible then
        FSplitterPad.Show;
    finally
      PlaningSheet.Clear;
    end;
  end;
end;

procedure TFMExcelAddIn.OnSheetSelectionChange(const Sh: IDispatch;
  const Target: ExcelRange);
var
  ExcelSheet: ExcelWorkSheet;
begin
  IsCloseWorkbook := false;
  ExcelSheet := GetWorkSheet(Sh);
  if not Assigned(ExcelSheet) then
    exit;
  if IsPlaningSheet(ExcelSheet) then
    ProtectionAnalisis(ExcelSheet, Target);
end;

function TFMExcelAddIn.SetConnectionStr(const URL, Scheme: WideString): HResult;
var
  Reg: TRegistry;
  DirectPath: string;
  OldUrl, OldScheme: string;
  IsSameConnection: boolean;
  ExlSheet: ExcelWorkSheet;
  ExcelBook: ExcelWorkbook;
  i: integer;
  tmpPlaningSheet: TPlaningSheet;
begin
  result := -1;
  OldUrl := '';
  OldScheme := '';
  if (URL = '') or (Scheme = '') then
    exit;
  Reg := TRegistry.Create;
  DirectPath := RegBasePath + RegConnSection + regConnWebServiceSection;
  //IsSameConnection := false;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(DirectPath, true) then
    begin
      OldUrl := Reg.ReadString(regURLKey);
      OldScheme := Reg.ReadString(regWebServiceSchemeKey);
      IsSameConnection := ((OldUrl + OldScheme) = (URL + Scheme));
      {������� �����������}
      if not IsSameConnection then
      begin
        Reg.WriteString(regURLKey, URL);
        Reg.WriteString(regWebServiceSchemeKey, Scheme);
        DataProvider.ClearCache;
        DataProvider.Disconnect;
      end;
      Reg.CloseKey;
    end;
    ExcelBook := Host.ActiveWorkbook;
    if not Assigned(ExcelBook) then
      exit;
    {������������ ��������� ����������� ���� ������ � �����. ��� �����
      ���������� ��������� ��������� ��������� ������. ������������� ���������
      ����� ������� �������� ��� � ������ 10304.}
    tmpPlaningSheet := TPLaningSheet.CreateInherited(PlaningSheet);
    tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    for i := 1 to ExcelBook.Sheets.Count do
    begin
      ExlSheet := GetWorkSheet(ExcelBook.Sheets.Item[i]);
      if not IsPlaningSheet(ExlSheet) then
        continue;
      if tmpPlaningSheet.Load(ExlSheet, Context, [lmInner]) then
      try
        tmpPlaningSheet.URL := URL;
        tmpPlaningSheet.SchemeName := Scheme;
        tmpPlaningSheet.Save;
      finally
        tmpPlaningSheet.Clear;
      end;
    end;
    tmpPlaningSheet.SetExternalLinks(nil, nil, nil, nil);
    FreeAndNil(tmpPlaningSheet);
  finally
    Reg.Free;
    //if not IsSameConnection then
      TryConnection(false);
    result := IIF(DataProvider.Connected, 0, -1);
  end;
end;

procedure TFMExcelAddIn.ApplyTaskContext;
var
  i: integer;
  ExlSheet: ExcelWorkSheet;
  TaskID: string;
  tmpPlaningSheet: TPLaningSheet;
  Op: IOperation;
begin
  if Host.Visible[LCID] then
    Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    if Host.Visible[LCID] then
    begin
      Op.Caption := '��������� ��������� ������...';
      Op.StartOperation(Host.Hwnd);
    end;
    if not VersionOK then
      exit;

    TaskId := '';
    if Assigned(Environment) then
      TaskId := Environment.TaskId;

    {�������������� ��� �����. ���������� ��� ����� ��������� ���������
      ��������� ������ �� ��������� 10304}
    tmpPlaningSheet := TPlaningSheet.CreateInherited(PlaningSheet);//TPLaningSheet.Create;
    tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    tmpPlaningSheet.Environment := Environment;
    for i := 1 to Host.ActiveWorkbook.Worksheets.Count do
    begin
      ExlSheet := GetWorkSheet(Host.ActiveWorkbook.Worksheets[i]);
      if not Assigned(ExlSheet) then
        continue;
      if not IsPlaningSheet(ExlSheet) then
        continue;
      tmpPlaningSheet.IsTaskContextLoad := true;
      {������������� ���������� ��� �������� ��������� ���������.}
      if CheckSheetVersion(ExlSheet, DataProvider, false, FProcess) then
        if tmpPlaningSheet.Load(ExlSheet, Context, [lmInner]) then
        begin
          tmpPlaningSheet.Clear;
          tmpPlaningSheet.SpecialFlagForTaskParamCopy := tmpPlaningSheet.TaskId <> TaskId;
          if tmpPlaningSheet.Load(ExlSheet, Context, [lmCollections]) then
          try
            tmpPlaningSheet.TaskId := TaskId;
            tmpPlaningSheet.Save;
          finally
            tmpPlaningSheet.Clear;
          end;
        end;
      tmpPlaningSheet.IsTaskContextLoad := false;
      tmpPlaningSheet.SpecialFlagForTaskParamCopy := false;

      LoadContextFromTask(false);
    end;
    tmpPlaningSheet.SetExternalLinks(nil, nil, nil, nil);
    FreeAndNil(tmpPlaningSheet);

    { ������ �������� ����� ��� ������������� �������� Online �������� �����}
    ExlSheet := GetWorkSheet(Host.ActiveSheet);
    if Assigned(ExlSheet) then
    begin
      PlaningSheet.Load(ExlSheet, Context, [lmInner]);
      PlaningSheet.Clear;
    end;

    PlaningSheet.IsTaskConnectionLoad := false;
    SetStatusBarInfo(sbiTaskID, IIF((TaskID = ''), '���', TaskID));
  finally
    if Host.Visible[LCID] then
    begin
      Op.StopOperation;
      Op := nil;
      SetActiveWindow(Host.Hwnd);
    end;
  end;
end;

{��������� ����� ������� ��������� ����� (����� 14009). ������������ �� ���������
���������� IFMPlaningExtension}
procedure TFMExcelAddIn.SetTaskContext(const taskContext: IDispatch); safecall;
begin
  ;
end;

procedure TFMExcelAddIn.MoveElementHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

  function GetElementType(ObjType: string): string;
  begin
    if ObjType = sntColumnDimension then
      result := '��������.'
    else
      if ObjType = sntRowDimension then
        result := '�����.'
      else
        result := '��������.';
  end;

  function GetComment(Mode: TWizardRunMode; NewObjType, Id: string): string;
  var
    StrTemp: string;
  begin
    try
      StrTemp := GetCommentForHistory(Mode, Id) + ' ��������� � ������� ' +
        GetElementType(NewObjType);
    except
      StrTemp := '��� ��������� ���������� ��� ������� ��������� ������.';
    end;
    //���� ����������� ���������� � ��������� �����, �������� �(�����) � �������� ��������
    StrTemp[1] := AnsiUpperCase(StrTemp[1])[1];
    result := ConvertStringToCommaText(StrTemp);
  end;

var
  Mode: TWizardRunMode;
  ObjType, NewObjType, Id: string;
  ExcelSheet: ExcelWorksheet;
  IsSuccess: boolean;
  CommentForHistory: string;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.Get_HelpFile, Mode, Id, NewObjType);
  case Mode of
    wrmEditColumn: ObjType := sntColumnDimension;
    wrmEditRow: ObjType := sntRowDimension;
    wrmEditFilter: ObjType := sntFilter;
    else exit;
  end;
  IsSuccess := false;
  if TryConnection(false) then
  try
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExcelSheet.Name + '"',
      not CriticalNode, NoteTime, otProcess);
    FUserEvents.Execute(ExcelSheet, enBeforeRefresh);

    try
      FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
      FXMLCatalog.SetUp(DataProvider);
      if not PlaningSheet.Load(ExcelSheet, Context, lmAll) then
        exit;
    finally
      //����� ������ �� ����� �����
      Application.ProcessMessages;
      FProcess.CloseOperation; //pfoSMDLoad
    end;

    CommentForHistory := GetComment(Mode, NewObjType, Id);

    if PlaningSheet.MoveElement(ObjType, NewObjType, Id) then
    begin
      if NewObjType = sntFilter then
        PlaningSheet.Filters.Refresh(true);
      PlaningSheet.SetUpMeasuresPosition;
      IsSuccess := DoRebuildSheet(ExcelSheet);

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    end;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
    CalculateSheet(ExcelSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExcelSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end
  else
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
end;

procedure TFMExcelAddIn.InsertNewLineHandler(Button: CommandBarButton; var CancelDefault: WordBool);
begin
  AppendEmptyRowToSheet(GetWorkSheet(Host.ActiveSheet), nil, PlaningSheet);
end;

procedure TFMExcelAddIn.DeleteElementHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

  function GetComment(Mode: TWizardRunMode; Id: string): string;
  begin
    try
      result := '������ ' + GetCommentForHistory(Mode, Id) + '.';
      result := ConvertStringToCommaText(result);
    except
      result := '��� ��������� ���������� ��� ������� ��������� ������.';
    end;
  end;

var
  Mode: TWizardRunMode;
  Id, Question, tmpStr, CellAlias: string;
  Index: integer;
  IsSuccess: boolean;
  CommentForHistory: string;
  NeedRebuild: boolean;
  ESheet: ExcelWorkSheet;
  Cell: TSheetSingleCellInterface;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Question);
  NeedRebuild := false;
  FXMLCatalog.SetUp(DataProvider);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    with PlaningSheet do
    begin
      if Empty then
        exit;
      CommentForHistory := GetComment(Mode, ID);

      case Mode of
        wrmEditColumn:
          begin
            Index := Columns.FindByID(Id);
            if Index < 0 then
              exit;
            if Columns[Index].MayBeDeleted then
            begin
              Columns.Delete(Index);
              SetUpMeasuresPosition;
              NeedRebuild := true;
            end;
          end;
        wrmEditRow:
          begin
            Index := Rows.FindByID(Id);
            if Index < 0 then
              exit;
            if Rows[Index].MayBeDeleted then
            begin
              Rows.Delete(Index);
              SetUpMeasuresPosition;
              NeedRebuild := true;
            end;
          end;
        wrmEditFilter:
          begin
            Index := Filters.FindByID(Id);
            if Index < 0 then
              exit;
            if Filters[Index].MayBeDeleted then
            begin
              Filters.Delete(Index);
              NeedRebuild := true;
            end;
          end;
        wrmEditTotal:
          begin
            Index := Totals.FindById(Id);
            if Index < 0 then
              exit;
            if Totals[Index].MayBeDeleted then
            begin
              Totals.Delete(Index);
              NeedRebuild := true;
            end;
          end;
        wrmNone: {!! ������ �������� ����������: ����� "�������" - ������
          ���� ���� �� ��������� �������}
          begin
            Index := SingleCells.FindById(Id);
            if Index < 0 then
              exit;
            if SingleCells[Index].MayBeDeleted then
            begin
              IsSuccess := false;
              try
                Cell := SingleCells[Index];
                CellAlias := Cell.Alias;
                Host.ScreenUpdating[LCID] := false;
                {��������� ���������}
                PlaningSheet.StartOperation(Host.Hwnd, pfoSingleResultDeletion);
                {�������� �������� ������� ������ ����������� �� ��������� ����� ����������}

                SetSheetProtection(ESheet, false);


                //Cell.ClearLinkedTypeFormulasValues;
                {���������� �������� ������� ������ ����������� �� ��������� ����� ����������}
                //Cell.MapLinkedTypeFormulasValues;
                Cell.Suicide(smImmediate, tmpStr);
                tmpStr := ConvertStringToCommaText(tmpStr);
                AddEventInSheetHistory(evtEdit, tmpStr, true);
                FProcess.DirectWriteLogString(tmpStr);
                NeedRebuild := false;
                IsSuccess := true;
              finally
                if IsSuccess then
                begin
                  PlaningSheet.Save;
                  SingleCellCanBePlacedHere := true;
                  EnableSingleCellAppend;
                end;
                PlaningSheet.StopOperation;
                Host.ScreenUpdating[LCID] := true;
              end;
            end;
          end;
      end;
    end;
    Application.ProcessMessages;
    IsSuccess := false;
    if NeedRebuild then
    try
      CollectDeferredTotalsData;
      //PlaningSheet.Backup;
      //PlaningSheet.Save;
      //PlaningSheet.Clear;
      FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
      FProcess.NewProcessClear := false;
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ESheet.Name + '"',
        not CriticalNode, NoteTime, otProcess);
      FUserEvents.Execute(ESheet, enBeforeRefresh);
      //FXMLCatalog.SetUp(DataProvider);
//      if not PlaningSheet.Load(ESheet, Context, lmAll) then
  //      exit;
      IsSuccess := DoRebuildSheet(ESheet);
     (* if not IsSuccess then
      begin
        PlaningSheet.Restore;
        PlaningSheet.Save;
      end;*)

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    finally
      AfterRebuild(ESheet, IsSuccess);
      CalculateSheet(ESheet);
      if IsSuccess then
      begin
        FUserEvents.Execute(ESheet, enAfterRefresh);
        FProcess.CloseOperation; //pfoRebuild
      end;
      FProcess.CloseProcess;
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.MarkEmptyHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  Selection, CurSelection, CellRange: ExcelRange;
  ExlSheet: ExcelWorkSheet;
  i, j, k: integer;
  Address: string;
  Style: variant;
  NumberFormat: variant;
  FirstRow, LastRow: integer;
  Columns: TStringList;

  // �������� ���������� �����������
  // ������ � ��������� ������, �������
  function GetTotalRanges(ExlSheet: ExcelWorkSheet; Totals: TSheetTotalCollectionInterface;
                          var FirstRow, LastRow: integer): TStringList;
  var
    Range: ExcelRange;
    i: integer;
    SectionIndex: integer;
    TotalRange: ExcelRange;
  begin
    result := TStringList.Create;
    // �������� ������ � ��������� ������
    Range := GetRangeByName(ExlSheet, snNamePrefix + snSeparator + sntTotals);
    if not Assigned(Range) then
      exit;
    FirstRow := Range.Row;
    LastRow := Range.Row + Range.Rows.Count - 1;
    // �������� ������� �����������
    for i := 0 to Totals.Count - 1 do
    begin
      // ���������� �� ����������
      if (Totals[i].TotalType <> wtResult) then
        continue;
      if not Totals[i].MayBeEdited then
        continue;
      SectionIndex := 0;
      TotalRange := Totals[i].GetTotalRange(SectionIndex);
      while (TotalRange <> nil) do
      begin
        result.Add(IntToStr(TotalRange.Column));
        // ��������� � ��������� ������
        Inc(SectionIndex);
        TotalRange := Totals[i].GetTotalRange(SectionIndex);
      end;
    end;
  end;

  function IsSingleCell(SingleCells: TSheetSingleCellCollectionInterface; Address: string): boolean;
  var
    i: integer;
  begin
    result := false;
    for i := 0 to SingleCells.Count - 1 do
    begin
      if not (SingleCells[i].TotalType = wtResult) then
        continue;
      if not SingleCells[i].MayBeEdited then
        continue;
      if SingleCells[i].Address = Address then
      begin
        result := true;
        exit;
      end;
    end;
  end;

begin
  // ���� ��� �� ���� ������������ - �������
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not IsPlaningSheet(ExlSheet) then
    exit;
  DisableCopyMode;
  // ������ ����������
  if PlaningSheet.Load(ExlSheet, Context, lmNoFreeData) then
  try
    // ������� �����...
    // ����� �������� ����� ��� ���������� �� ������ ������
    if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.ValidateStyles;
    // �������� ������� ���������� �����������
    Columns := GetTotalRanges(ExlSheet, PlaningSheet.Totals, FirstRow, LastRow);
    try
      // �������� ����������
      ExlSheet.Application.ScreenUpdating[LCID] := false;
      ExlSheet.Application.Set_Interactive(LCID, false);
      // �������� ���������� ������
      Selection := (ExlSheet.Application.Selection[LCID] as ExcelRange);
      for k := 1 to Selection.Areas.Count do
      begin
        CurSelection := Selection.Areas[k];
        for i := CurSelection.Row to CurSelection.Row + CurSelection.Rows.Count - 1 do
          for j := CurSelection.Column to CurSelection.Column + CurSelection.Columns.Count - 1 do
          begin
            CellRange := ExlSheet.Range[ExlSheet.Cells.Item[i, j], ExlSheet.Cells.Item[i, j]];
            Address := CellRange.AddressLocal[false, false, xlA1, false, varNull];
            NumberFormat := ExlSheet.Cells.Item[i, j].NumberFormat;
            Style := ExlSheet.Cells.Item[i, j].Style;
            if IsSingleCell(PlaningSheet.SingleCells, Address) then
            begin
              if (string(Style) = snDataFreeErased) then
                ExlSheet.Cells.Item[i, j].Style := snResultSingleCells
              else
                if (string(Style) = snResultSingleCells) then
                  ExlSheet.Cells.Item[i, j].Style := snDataFreeErased;
            end
            else
            begin
              // ���������� ������ ��� ���������� �����������
              if (i < FirstRow) or (i > LastRow) or
                 (Columns.IndexOf(IntToStr(j)) = -1) then
                continue;
              // ����������� ����� � ����������� ���������
              // "���������" ������ / �������
              if (string(Style) = snDataFreeErased) then
                ExlSheet.Cells.Item[i, j].Style := snDataFree
              else
                if (string(Style) = snDataFree) then
                  ExlSheet.Cells.Item[i, j].Style := snDataFreeErased;
            end;
            ExlSheet.Cells.Item[i, j].NumberFormat := NumberFormat;
          end;
      end;
    finally
      FreeAndNil(Columns);
      ExlSheet.Application.ScreenUpdating[LCID] := true;
      ExlSheet.Application.Set_Interactive(LCID, true);
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.SheetHistoryHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  if not Assigned(FfrmSheetHistory)then
    FfrmSheetHistory := TSheetHistory.Create(nil);
  DisableCopyMode;
  try
    FfrmSheetHistory.ShowSheetHistory(Host);
  finally
    SetActiveWindow(Host.Hwnd);
  end;
end;

procedure TFMExcelAddIn.EditStyleHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  BarButton: CommandBarButton;
  ExcelSheet: ExcelWorksheet;
begin
  try
    DisableCopyMode;
    if not Assigned(Host) then
      exit;
    ExcelSheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(ExcelSheet) then
      exit;
    SetBookProtection(Host.ActiveWorkbook, false);
    //���� ������ "�����" � ���� Excel-�
    BarButton := (Host.CommandBars.FindControl(EmptyParam, 254, EmptyParam,
      EmptyParam) as CommandBarButton);
    //�������� �
    BarButton.Execute;
    //���� ���� ������������� ������
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  except
  end;
end;

procedure TFMExcelAddIn.WorkbookBeforeSave(const Wb: ExcelWorkbook; SaveAsUI: WordBool; var Cancel: WordBool);
var
  TaskId: string;
  ContextAssigned: boolean;
begin
  DisableCopyMode;
  ProtectPlaningSheets(Wb, true);
  // ���� ����� - �� �����, �� ��������� ����� ���, ������ ��������������
  if PlaningSheet.IsSilentMode then
    exit;
  TaskId := Environment.TaskId;
  ContextAssigned := AssignedContext or Assigned(Context);
  if SaveAsUI and (TaskId <> '') and ContextAssigned then
    Cancel := not ShowWarningEx(wrnTaskSave);
end;

function TFMExcelAddin.EditableTotalSelected(ExcelSheet: ExcelWorksheet;
  Target: ExcelRange): boolean;
var
  Editable: boolean;
begin
  result := FWritablesInfo.CheckForWritableRange(ExcelSheet, Target, Editable);
  { ���� ���� ��������� � ������ "������ � �������", �� ��� ������ ������
    ���� ���� �����, ��� ���������� ������� ����, ���������� ��� ����������
    �� ��� �������������� � ����� ������.}
  if not PlaningSheet.InConstructionMode then
    result := result and Editable;
end;

procedure TFMExcelAddIn.ProtectionAnalisis(const ExcelSheet: ExcelWorksheet;
  const Target: ExcelRange);
var
  tmpStr: string;
  IsWritableCell, Editable: boolean;
begin
  if not Assigned(ExcelSheet) then
    exit;

  {��������� ��� ��������� �����}
  if FWritablesInfo.IsSingleCellSelected(ExcelSheet, Target,
    IsWritableCell, Editable) then
  begin
    SingleCellCanBePlacedHere := false;
    EnableSingleCellAppend;
    if PlaningSheet.InConstructionMode then
      SetSheetProtection(ExcelSheet, not IsWritableCell)
    else
      SetSheetProtection(ExcelSheet, not (IsWritableCell and Editable));
    exit;
  end;

  SingleCellCanBePlacedHere := IsSingleCellAllowed(ExcelSheet, Target,
    FWritablesInfo, tmpStr, Editable);
  if not PlaningSheet.InConstructionMode then
    SingleCellCanBePlacedHere := SingleCellCanBePlacedHere and Editable;
  if not IsPlaningSheet(ExcelSheet) then
  begin
    EnableSingleCellAppend;
    exit;
  end;
  if TableSelected(ExcelSheet, Target) then
  begin
    if (BreakPointSelected(ExcelSheet, Target) or EditableTotalSelected(ExcelSheet, Target)) then
      SetSheetProtection(ExcelSheet, false)
    else
      SetSheetProtection(ExcelSheet, true);
    EnableSingleCellAppend;
  end
  else
    if BreakPointSelected(ExcelSheet, Target) then
    begin
      SetSheetProtection(ExcelSheet, false);
      EnableSingleCellAppend;
    end
    else
      if IntersectTableAndRange(ExcelSheet, Target) then
      begin
        SetSheetProtection(ExcelSheet, true);
        SingleCellCanBePlacedHere := false;
        EnableSingleCellAppend;
      end
      else
      begin
        SetSheetProtection(ExcelSheet, false);
        if IsNestedRanges(Target, GetExtendedTableRange(ExcelSheet)) then
          SingleCellCanBePlacedHere := false;
        EnableSingleCellAppend
      end;
end;

procedure TFMExcelAddIn.DisableCopyMode;
begin
  {��������� ����� ����������� � Excel-�}
  Host.CutCopyMode[LCID] := Integer(false);
end;

procedure TFMExcelAddIn.ShowParamsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Sheet: ExcelWorkSheet;

  // ��������� ����������� ��������� ����� 
  procedure ModifyComments(Sheet: ExcelWorkSheet);
  var
    i, j: integer;
    Range: ExcelRange;

    procedure CommentAxis(AxisCollection: TSheetAxisCollectionInterface);
    var
      i, j: integer;
      Range: ExcelRange;
    begin
      if AxisCollection.Broken then
      begin
        Range := GetRangeByName(Sheet, snNamePrefix + snSeparator + sntRowTitles);
        if Assigned(Range) then
          CommentCell(Sheet, Range.Row, Range.Column, AxisCollection.CommentText);
      end
      else
        for i := 0 to AxisCollection.Count - 1 do
          for j := 0 to AxisCollection[i].Levels.Count - 1 do
          begin
            Range := GetRangeByName(Sheet, AxisCollection[i].Levels[j].TitleExcelName);
            if Assigned(Range) then
              CommentCell(Sheet, Range.Row, Range.Column, AxisCollection[i].CommentText);
          end;
    end;

  begin
    CommentAxis(PlaningSheet.Rows);
    CommentAxis(PlaningSheet.Columns);
    for i := 0 to PlaningSheet.Filters.Count - 1 do
    begin
      if PlaningSheet.Filters[i].IsPartial then
        continue;
      Range := GetRangeByName(Sheet, PlaningSheet.Filters[i].ExcelName);
      if Assigned(Range) then
        CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.Filters[i].CommentText);
    end;
    for i := 0 to PlaningSheet.Totals.Count - 1 do
      for j := 0 to PlaningSheet.Totals[i].SectionCount - 1 do
      begin
        Range := GetRangeByName(Sheet, PlaningSheet.Totals[i].TitleExcelName +
                                snSeparator + IntToStr(j));
        if Assigned(Range) then
          CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.Totals[i].CommentText);
      end;
    for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    begin
      Range := GetRangeByName(Sheet, PlaningSheet.SingleCells[i].ExcelName);
      if Assigned(Range) then
        CommentCell(Sheet, Range.Row, Range.Column, PlaningSheet.SingleCells[i].CommentText);
    end;
  end;

begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
    exit;
  end;
  {���������� ��������� ����� ��������� ����������, ��� ������������� �� ������ � ����}
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    if EditParams(PlaningSheet) then
    begin
      PlaningSheet.Save;
      {���������� ��������� ����� ���������� ����������, ��� ������������� �� ����� � ������}
      LoadContextFromTask(true);
      ApplyTaskContext;
      SetSheetProtection(Sheet, false);
      ModifyComments(Sheet);
    end;
  finally
    //FreeAndNil(frmParamControl);
    PlaningSheet.Clear;
    ProtectionAnalisis(Sheet, Host.ActiveCell);
    SetActiveWindow(Host.Hwnd);
  end;
end;

procedure TFMExcelAddIn.ShowConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  frmConstControl: TfrmConstControl;
  Sheet: ExcelWorkSheet;
  EventType: TSheetEventType;
  TmpConst: TConstInterface;
  NewName, OldName: string;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
  begin
    PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
    exit;
  end;
  frmConstControl := TfrmConstControl.Create(nil);
  frmConstControl.ISConstChoiseMode := false;
  {���������� ��������� ����� ��������� ����������, ��� ������������� �� ������ � ����}
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.ValidateStyles;
    frmConstControl.SheetInterface := PlaningSheet;
    frmConstControl.ShowModal;
    if (frmConstControl.ModalResult = mrOK) then
    begin
      EventType := evtConstsEdit;
      if (frmConstControl.HistoryList.Count <> 0) then
        PlaningSheet.AddEventInSheetHistory(EventType, frmConstControl.HistoryList.CommaText, true);

      {���� �����-�� ��������� ���� ������������� ������� ���������� ���������� ��������� � ������ ������}
      if not PlaningSheet.Consts.Empty then
      begin
        TmpConst := PlaningSheet.Consts[0];
        while frmConstControl.RenamingList.Count > 0 do
        begin
          OldName := frmConstControl.RenamingList.Names[0];
          NewName := frmConstControl.RenamingList.Values[OldName];
          if Assigned(TmpConst) then
            TmpConst.SyncSheetConsts(OldName, NewName);
          frmConstControl.RenamingList.Delete(0);
        end;
      end;

      if frmConstControl.NeedUpdate then
      begin
        if not SetSheetProtection(Sheet, false) then
        begin
          PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
          exit;
        end;
        PlaningSheet.MapConsts;
      end;
      PlaningSheet.Save;
      {���������� ��������� ����� ���������� ����������, ��� ������������� �� ����� � ������}
      LoadContextFromTask(false);
    end;
  finally
    MayHook := true;
    FreeAndNil(frmConstControl);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.AddConstsHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  frmConstControl: TfrmConstControl;
  Sheet: ExcelWorkSheet;
  Cell: TSheetSingleCellInterface;
  EventType: TSheetEventType;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
  begin
    PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
    exit;
  end;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.ValidateStyles;
    frmConstControl := TfrmConstControl.Create(nil);
    frmConstControl.ISConstChoiseMode := true;

    frmConstControl.SheetInterface := PlaningSheet;
    frmConstControl.ShowModal;
    if not (frmConstControl.ModalResult = mrOK) then
      exit;
    Cell := PlaningSheet.SingleCells.Append;
    Cell.TotalType := wtConst;
    Cell.Name := frmConstControl.Constant.Name;
    Cell.Caption := frmConstControl.Constant.Name;
    PlaningSheet.Consts.ConstByName(Cell.Name).IsSheetConst := true;
    MayHook := false;
    MarkObject(Sheet, Sheet.Application.ActiveCell, Cell.ExcelName, false);
    frmConstControl.HistoryList.Add(Format('%s ��������� ������ - ��������� "%s" , �� ������ %s',
      ['���������', Cell.Name, Cell.Address]));
    EventType := evtEdit;
    if (frmConstControl.HistoryList.Count <> 0) then
      PlaningSheet.AddEventInSheetHistory(EventType, frmConstControl.HistoryList.CommaText, true);
    if not SetSheetProtection(Sheet, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.MapConsts;
    PlaningSheet.Save;
    LoadContextFromTask(false);
  finally
    MayHook := true;
    FreeAndNil(frmConstControl);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.EditConstHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Mode: TWizardRunMode;
  Id, Query, EditingConstName, tmpStr: string;
  Cell: TSheetSingleCellInterface;
  Sheet: ExcelWorkSheet;
  Index: integer;
  Constant: TConstInterface;
  frmConstProperties: TfrmConstProperties;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    frmConstProperties := TfrmConstProperties.Create(nil);
    Index := PlaningSheet.SingleCells.FindById(Id);
    Cell := PlaningSheet.SingleCells[Index];
    Constant := PlaningSheet.Consts.ConstByName(Cell.Name);
    {��������� �������� - ��������� �������� � ��������� ��������? }
    if not Assigned(Constant) then
      if ShowQuestion('������ �������������� ���������� ����������.' +
        #13#10'���������� � ��������� ����������� � �����.' +
        #13#10'�������� ��������� � ���� (� ������ �������������� ������ ���������� ����� ������)?') then
      begin
        Constant := Cell.RepairConst;
      end
      else
      begin
        Cell.Suicide(smImmediate, tmpStr);
        PlaningSheet.Save;
        if tmpStr <> '' then
          PlaningSheet.AddEventInSheetHistory(evtConstsEdit, tmpStr, false);
        exit;
      end;
    EditingConstName := Constant.Name;
    frmConstProperties.Constant := Constant;
    frmConstProperties.Consts := PlaningSheet.Consts;
    frmConstProperties.TaskContext := PlaningSheet.TaskContext;
    frmConstProperties.ShowModal;
    if not frmConstProperties.Apply then
      exit;
    Constant.SyncSheetConsts(EditingConstName, Constant.Name);
    MayHook := false;
    if not SetSheetProtection(Sheet, false) then
    begin
      PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      exit;
    end;
    PlaningSheet.MapConsts;
    PlaningSheet.Save;
    LoadContextFromTask(false);
  finally
    MayHook := true;
    FreeAndNil(frmConstProperties);
    AfterRebuild(Sheet, true);
  end;
end;

procedure TFMExcelAddIn.RefreshConstHandler(Button: CommandBarButton; var CancelDefault: WordBool);
var
  Id, Query, MsgText: string;
  tmpStringList: TStringList;
  Mode: TWizardRunMode;
  Sheet: ExcelWorkSheet;
begin
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  LoadContextFromTask(true);
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    SetSheetProtection(Sheet, false); //�������� ������� ������
    tmpStringList := TStringList.Create;
    tmpStringList.Add(IntToStr(PlaningSheet.SingleCells.FindById(Id)));
    PlaningSheet.MapSingleCells(tmpStringList, MsgText, true, nil, nil);
    PlaningSheet.Save;
  finally
    SetSheetProtection(Sheet, true); //������ �� �����
    FreeStringList(tmpStringList);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.EnableSingleCellAppend;
var
  State: boolean;
begin
  State := SingleCellCanBePlacedHere;
  if not PlaningSheet.InConstructionMode then
    State := State and PlaningSheet.SingleCells.MayBeEdited;
  UpdateAppendSubmenuItem(true, tagToolButtonSingleCell, State);
  UpdateAppendSubmenuItem(true, tagToolButtonConst, State);
  UpdateAppendSubmenuItem(false, tagMenuButtonSingleCell, State);
  UpdateAppendSubmenuItem(false, tagMenuButtonConst, State);
end;

procedure TFMExcelAddIn.SingleCellEditorHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  CellIndex, ErrCode: integer;
  Cell: TSheetSingleCellInterface;
  Success, IsAdding, IsRefresh: boolean;
  ExcelSheet: ExcelWorkSheet;
  Mode: TWizardRunMode;
  Id, Param2, MsgText: string;
  HList, AddedCells, tmpStringList: TStringList;

  procedure RefreshCell(Cell: TSheetSingleCellInterface);
  var
    Operation: IOperation;
  begin
    Operation := CreateComObject(CLASS_Operation) as IOperation;
    try
      Operation.StartOperation(Host.Hwnd);
      Operation.Caption := pfoFiltersRefresh;
      Cell.Refresh(false);
    finally
      Application.ProcessMessages;
      Operation.StopOperation;
      SetActiveWindow(Host.Hwnd);
      Operation := nil;
    end;
  end;

begin
  Success := false;
  IsRefresh := false;
  if PrepareRebuild(lmNoFreeData, ExcelSheet) then
  try
    PlaningSheet.CollectTotalsData(true);
    ParseAdditionalInfo(Button.HelpFile, Mode, Id, Param2);
    CellIndex := PlaningSheet.SingleCells.FindById(Id);
    IsAdding := CellIndex = -1;
    IsRefresh := Param2 = 'refresh';
    HList := TStringList.Create;
    tmpStringList := TStringList.Create;
    MsgText := '';

    if IsRefresh then
      Success := true
    else
      Success := EditSingleCell(PlaningSheet, CellIndex, Host.Application.Hwnd);
    if Success then
    begin
      Success := false;
      //������� ������ �� ����� ����������
      if not SetSheetProtection(ExcelSheet, false) then
      begin
        MsgText := ermWorkbookProtectionFault;
        HList.Add(MsgText);
        exit;
      end;
      ExcelSheet.Application.ScreenUpdating[LCID] := false;
      ExcelSheet.Application.Set_Interactive(LCID, false);
      if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
      begin
        MsgText := ermWorkbookProtectionFault;
        HList.Add(MsgText);
        exit;
      end;
      PlaningSheet.ValidateStyles;
      Cell := PlaningSheet.SingleCells[CellIndex];

      // ����� �������� �������
      RefreshCell(Cell);
      if not Cell.Validate(MsgText, ErrCode) then
      begin
        HList.Add(Format('��������� ���������� "%s" (���: "%s", ����: "%s"), �� ������ %s: ',
          [Cell.Name, Cell.CubeName, Cell.MeasureName, Cell.Address]) + MsgText);
        exit;
      end;
      MayHook := false;

      if IsAdding then
      begin
        MarkObject(ExcelSheet, ExcelSheet.Application.ActiveCell, Cell.ExcelName,
          false);
        AddedCells := TStringList.Create;
        AddedCells.Add(Cell.ExcelName);
      end;

      tmpStringList.Add(IntToStr(CellIndex));
      Success := PlaningSheet.MapSingleCells(tmpStringList, MsgText, true, nil, nil);
      HList.Add(Format('%s ��������� ���������� "%s" (���: "%s", ����: "%s"), �� ������ %s',
        [IIF(IsRefresh, '��������', IIF(IsAdding, '��������', '�������')),
        Cell.Name, Cell.CubeName, Cell.MeasureName, Cell.Address]));
      if Success then
      begin
        PlaningSheet.Save;
        if IsAdding then
        begin
          SingleCellCanBePlacedHere := false;
          EnableSingleCellAppend;
        end;
      end
      else
        if IsAdding then
        begin
          ClearAddedCellsNames(ExcelSheet, AddedCells);
          FreeStringList(AddedCells);
        end;
    end;
  finally
    if HList.Count > 0 then
      if IsRefresh then
        PlaningSheet.AddEventInSheetHistory(evtRefresh, HList.CommaText, Success)
      else
        PlaningSheet.AddEventInSheetHistory(evtEdit, HList.CommaText, Success);
    FProcess.DirectWriteLogString(CommaTextToString(HList.CommaText));
    MayHook := true;
    FreeStringList(HList);
    FreeStringList(tmpStringList);
    AfterRebuild(ExcelSheet, Success);
  end;
end;

procedure TFMExcelAddIn.SingleCellsManagerHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  RefreshList, HList, AddedCells: TStringList;
  MsgText: string;
begin
  if PrepareRebuild(lmNoFreeData, ExcelSheet) then
  try
    PlaningSheet.CollectTotalsData(true);
    {���� �������� ���������� ������ ��������� ��������� �����, �� ����� ��}
    HList := TStringList.Create;
    RefreshList := TStringList.Create;
    AddedCells := TStringList.Create;
    if ManageSingleCells(PlaningSheet, HList, AddedCells, RefreshList) then
    begin
      if (RefreshList.Count > 0) then
      try
        Application.ProcessMessages;
        FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
        FProcess.OpenOperation(pfoSingleCellsRefresh, not CriticalNode, NoteTime, otProcess);
        if not PlaningSheet.SingleCells.Validate then
        begin
          PlaningSheet.PostMessage(ermValidationFault, msgError);
          exit;
        end;

        //������� ������ �� ����� ����������
        if not SetSheetProtection(ExcelSheet, false) then
        begin
          FProcess.PostError(ermWorksheetProtectionFault);
          exit;
        end;
        ExcelSheet.Application.ScreenUpdating[LCID] := false;
        ExcelSheet.Application.Set_Interactive(LCID, false);
        if not InitWorkbookStyles(Host.ActiveWorkbook, false) then
        begin
          FProcess.PostError(ermWorkbookProtectionFault);
          exit;
        end;
        PlaningSheet.ValidateStyles;
        if not PlaningSheet.MapSingleCells(RefreshList, MsgText, true, nil, nil) then
        begin
          ClearAddedCellsNames(ExcelSheet, AddedCells);
          exit;
        end;
        FProcess.CloseOperation;
      finally
        FProcess.CloseProcess;
      end;
      PlaningSheet.Save;
      PlaningSheet.AddEventInSheetHistory(evtEdit, HList.CommaText, true);
    end;
  finally
    FreeStringList(HList);
    FreeStringList(AddedCells);
    FreeStringList(RefreshList);
    AfterRebuild(ExcelSheet, true);
    ProtectionAnalisis(ExcelSheet, Host.ActiveCell);
  end;
end;

function TFMExcelAddIn.GetContext: TTaskContext;
begin
  result := FTaskContextProvider.TaskContext[Host.ActiveWorkbook];
end;


function TFMExcelAddIn.GetActiveWorkBookName: string;
var
  ActiveBook: ExcelWorkBook;
begin
  result := '';
  ActiveBook := Host.ActiveWorkbook;
  if Assigned(ActiveBook) then
    result := ActiveBook.FullName[LCID];
end;

function TFMExcelAddIn.Get_IsSilentMode: WordBool;
begin
  result := PlaningSheet.IsSilentMode;
end;

procedure TFMExcelAddIn.Set_IsSilentMode(Value: WordBool);
begin
  PlaningSheet.IsSilentMode := Value;
end;

function TFMExcelAddIn.Get_ProcessForm: IProcessForm;
begin
  if Assigned(FProcess) then
    result := FProcess;
end;

procedure TFMExcelAddIn.Set_ProcessForm(const Value: IProcessForm);
begin
  FProcess := nil;
  FProcess := Value;
  PlaningSheet.Process := FProcess;
  FUserEvents.SetExternalLinks(FProcess);
  UpdateProcessLogging;
end;
                                            
procedure TFMExcelAddIn.OnTaskConnection(IsConnected: WordBool);
begin
  PlaningSheet.OnTaskConnection(IsConnected);
  PlaningSheet.IsTaskConnectionLoad := true;
end;

function TFMExcelAddIn.Get_IsLoadingFromTask: WordBool;
begin
  result := Environment.IsLoadingFromTask;
end;

procedure TFMExcelAddIn.Set_IsLoadingFromTask(Value: WordBool);
begin
  PlaningSheet.IsLoadingFromTask := Value;
end;

procedure TFMExcelAddIn.AncillaryInit;
begin
  if (not(Assigned(Host) and IsExcelVisible(Host)))then
    exit;
  try
    Host.ScreenUpdating[LCID] := false;
  finally
    Application.ProcessMessages;
  end;
end;

function TFMExcelAddIn.QueryRange(const ACaption: WideString;
  const APrompt: WideString; AllowMultiArea: WordBool): IDispatch;
var
  Visibility: boolean;
  v: Variant;
begin
  result := nil;
  Visibility := Host.Visible[LCID];
  if not Visibility then
    Host.Set_Visible(LCID, true);
  try
    while true do
    begin
      v := Host.InputBox(APrompt, ACaption, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, 8, LCID);
      Application.ProcessMessages;
      if not Assigned(TVarData(v).VDispatch) then
        exit;
      result := IDispatch(TVarData(v).VDispatch);
      if AllowMultiArea or (not AllowMultiArea and ((result as ExcelRange).Areas.Count = 1)) then
        exit;
      ShowError('��������� �������� �������� ��������� ����������� ��������.');
    end;
  finally
    if not Visibility then
      Host.Set_Visible(LCID, false);
  end;
end;

procedure TFMExcelAddIn.RecognizeExcelVersion;
begin
  FExcelVersion := evUnknown;
  if Assigned(Host) then
  begin
    if (Pos('11.', Host.Version[LCID]) > 0) then
      FExcelVersion := ev2003
    else
      if (Pos('10.', Host.Version[LCID]) > 0) then
        FExcelVersion := evXP;
  end;
end;

procedure TFMExcelAddIn.ComponentEditorHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExlSheet: ExcelWorkSheet;
  HistoryString, ID, DummyStr: string;
  StylesToDefault, IsSuccess, NeedSwitchOffline, NeedClearTaskInfo: boolean;
  WizardRunMode: TWizardRunMode;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData, ExlSheet) then
  try
    HistoryString := '';
    ParseAdditionalInfo(Button.Get_HelpFile, WizardRunMode, ID, DummyStr);
    if EditSheetComponents(PlaningSheet, FUserEvents, HistoryString,
      StylesToDefault, NeedSwitchOffline, NeedClearTaskInfo, Id) then
    begin
      Application.ProcessMessages;
      FUserEvents.SaveEvents(ExlSheet);
      if NeedClearTaskInfo then
      begin
        PlaningSheet.OnTaskConnection(false);
        UpdateStatusBar(ExlSheet);
      end;
      if StylesToDefault then
      begin
        Host.ScreenUpdating[GetUserDefaultLCID] := false;
        if not InitWorkbookStyles((ExlSheet.Parent as ExcelWorkbook), true) then
          PlaningSheet.ShowMessageEX(ermWorkbookProtectionFault, msgError);
      end;

      AncillaryInit;
      if PlaningSheet.Online then
      begin
        FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh,mErrorRefresh, false);
        FProcess.NewProcessClear := false;
        FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExlSheet.Name + '"',
          not CriticalNode, NoteTime, otProcess);

        CollectDeferredTotalsData;

        PlaningSheet.NeedMapSingleCells := false;
        if NeedSwitchOffline then
          PlaningSheet.PrepareToSwitchOffline;
        IsSuccess := DoRebuildSheet(ExlSheet);
      end;
      PlaningSheet.AddEventInSheetHistory(evtEdit,
        MergeCommaText(HistoryString, GetErrorList), IsSuccess);
      if NeedSwitchOffline then
      begin
        PlaningSheet.SwitchOffline;
        PlaningSheet.Save;
      end;
    end;
  finally
    AfterRebuild(ExlSheet, IsSuccess);
    CalculateSheet(ExlSheet);
    if IsSuccess then
    begin
      FUserEvents.Execute(ExlSheet, enAfterRefresh);
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
  end;
end;

{ IFMPlanningVBProgramming }

function TFMExcelAddIn.VBGetPropertyByName(const PropertyName: WideString): WideString;
begin
  result := GetPropValueByName(PropertyName);
end;

procedure TFMExcelAddIn.VBSetPropertyByName(const PropertyName, PropertyValue: WideString);
begin
  SetPropValueByName(PropertyName, PropertyValue);
end;

function TFMExcelAddIn.VBGetConstValueByName(const ConstName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  Constant: TConstInterface;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Constant := PlaningSheet.Consts.ConstByName(ConstName);
    if not Assigned(Constant) then
      exit;
    result := Constant.Value;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBSetConstValueByName(const ConstName,
  ConstValue: WideString): WordBool;
var
  Sheet: ExcelWorkSheet;
  Constant: TConstInterface;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Constant := PlaningSheet.Consts.ConstByName(ConstName);
    if not Assigned(Constant) then
      exit;
    Constant.Value := ConstValue;
    PlaningSheet.Save;
    result := true;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBRefresh: WordBool;
begin
  result := RefreshActiveSheet;
end;

function TFMExcelAddIn.VBWriteback: WordBool;
begin
  result := WritebackActiveSheet;
end;

function TFMExcelAddIn.VBGetCurrentConnection(var URL, SchemeName: WideString): WordBool;

  function GetSheetConnection(var URL, SchemeName: WideString): boolean;
  var
    Sheet: ExcelWorkSheet;
  begin
    result := false;
    Sheet := GetWorkSheet(Host.ActiveSheet);
    if not Assigned(Sheet) then
      exit;
    if PlaningSheet.Load(Sheet, Context, [lmInner]) then
    try
      URL := PlaningSheet.URL;
      SchemeName := PlaningSheet.SchemeName;
      result := (URL <> '') and (SchemeName <> '');
    finally
      PlaningSheet.Clear;
    end;
  end;

  function GetRegistryConnection(var URL, SchemeName: WideString): boolean;
  var
    Reg: TRegistry;
  begin
    result := false;
    Reg := TRegistry.Create;
    try
      Reg.RootKey := HKEY_CURRENT_USER;
      if not Reg.OpenKey(RegBasePath + RegConnSection + regConnWebServiceSection, true) then
        exit;
      URL := Reg.ReadString(regURLKey);
      SchemeName := Reg.ReadString(regWebServiceSchemeKey);
      result := (URL <> '') and (SchemeName <> '');
      Reg.CloseKey;
    finally
      Reg.Free;
    end;
  end;

begin
  result := false;
  if not GetSheetConnection(URL, SchemeName) then
    if not GetRegistryConnection(URL, SchemeName) then
      exit;
  result := true;    
end;

function TFMExcelAddIn.VBGetMembers(const DimensionName: WideString): OleVariant;
var
  Sheet: ExcelWorkSheet;
begin
  result := Null;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    result := PlaningSheet.GetMembersArray(DimensionName);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant);
var
  Sheet: ExcelWorkSheet;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    PlaningSheet.SetMembersByArray(DimensionName, UniqueNames);
    PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetParamMembers(const ParamName: WideString): OleVariant;
var
  Sheet: ExcelWorkSheet;
  Param: TParamInterface;
begin
  result := Null;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Param := PlaningSheet.Params.ParamByName(ParamName);
    if not Assigned(Param) then
      exit;
    result := PlaningSheet.GetMembersArray(Param.Dimension);
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant);
var
  Sheet: ExcelWorkSheet;
  Param: TParamInterface;
begin
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Param := PlaningSheet.Params.ParamByName(ParamName);
    if not Assigned(Param) then
      exit;
    PlaningSheet.SetMembersByArray(Param.Dimension, UniqueNames);
    PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBEditMembers(const DimensionName: WideString): WordBool;
var
  Sheet: ExcelWorkSheet;
  Dimension: TSheetDimension;
  WizardRunMode: TWizardRunMode;
begin
  result := false;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if not TryConnection(false) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    if not Assigned(FfrmConstructorWizard) then
    begin
      FfrmConstructorWizard := TfrmConstructorWizard.CreateEx(nil, PlaningSheet);
      FfrmConstructorWizard.XMLCatalog := FXMLCatalog;
      FfrmConstructorWizard.DataProvider := DataProvider;
    end;
    FXMLCatalog.SetUp(DataProvider);
    if not FXMLCatalog.Loaded then
      exit;

    Dimension := PlaningSheet.GetDimension(DimensionName);
    if not Assigned(Dimension) then
      exit;
    case Dimension.GetObjectType of
      wsoRow: WizardRunMode := wrmEditRow;
      wsoColumn: WizardRunMode := wrmEditColumn;
      wsoFilter: WizardRunMode := wrmEditFilter;
    else
      WizardRunMode := wrmNone;
    end;
    result := FfrmConstructorWizard.RunWizard(WizardRunMode, Dimension.UniqueID);
    if result then
      PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetMemberProperty(const DimensionName, UniqueName,
  MemberPropertyName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  Dimension: TSheetDimension;
  MembersDom: IXmlDomDocument2;
  IsLeaf: boolean;
  PropertyName: string;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    Dimension := PlaningSheet.GetDimension(DimensionName);
    if not Assigned(Dimension) then
      exit;
    MembersDom := Dimension.Members;
    if not Assigned(MembersDom) then
      exit;
    PropertyName := MemberPropertyName;
    EncodeMemberPropertyName(PropertyName);
    result := GetMemberAttrByKey(MembersDom, 'unique_name', UniqueName, PropertyName, IsLeaf);
  finally
    PlaningSheet.Clear;
  end;  
end;

function TFMExcelAddIn.VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString;
var
  Sheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  AxisCondition: string;

  function GetAxisCondition: string;
  var
    i: integer;
    DimName: string;
    ElementAxis: TSheetAxisElementInterface;
  begin
    result := '';
    if VarIsNull(Coordinates) then
      exit;
    result := '[';
    for i := VarArrayLowBound(Coordinates, 1) to VarArrayHighBound(Coordinates, 1) do
    begin
      DimName := VarToStr(Coordinates[i, 0]);
      ElementAxis := PlaningSheet.Columns.FindByDimension(DimName);
      if not Assigned(ElementAxis) then
      begin
        ElementAxis := PlaningSheet.Rows.FindByDimension(DimName);
        if not Assigned(ElementAxis) then
          continue;
      end;
      result := result + '(@' + ElementAxis.Alias + '="' + VarToStr(Coordinates[i, 1]) + '")';
    end;
    result := result + ']';
  end;

  function GetNodeValue(Dom: IXmlDomDocument2; TotalAlias, AxisCondition: string): string;
  var
    XPath: string;
    Node: IXmlDomNode;
  begin
    result := '';
    XPath := 'function_result/data/row' + AxisCondition;
    Node := Dom.selectSingleNode(XPath);
    if not Assigned(Node) then
      exit;
    result := GetStrAttr(Node, TotalAlias, '');
  end;

begin
  result := '';
  exit;
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmAll) then
  try
    Total := PlaningSheet.Totals.FindByCaption(TotalName);
    if not Assigned(Total) then
      exit;
    AxisCondition := GetAxisCondition;

    if Assigned(PlaningSheet.FreeTotalsData) then
      result := GetNodeValue(PlaningSheet.FreeTotalsData, Total.Alias, AxisCondition);
    if (result <> '') then
      exit;
    if Assigned(PlaningSheet.FreeTotalsDataIgnored) then
      result := GetNodeValue(PlaningSheet.FreeTotalsDataIgnored, Total.Alias, AxisCondition);
  finally
    PlaningSheet.Clear;
  end;
end;

function TFMExcelAddIn.VBGetSingleCellValue(const SingleCellName: WideString): WideString;
var
  Sheet: ExcelWorkSheet;
  SingleCell: TSheetSingleCellInterface;
begin
  result := '';
  Sheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(Sheet) then
    exit;
  if PlaningSheet.Load(Sheet, Context, lmNoFreedata) then
  try
    SingleCell := PlaningSheet.SingleCells.FindByName(SingleCellName);
    if not Assigned(SingleCell) then
      exit;
    result := SingleCell.Value;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.UpdateStatusBar(ExcelSheet: ExcelWorkSheet);
var
  ExcelBook: ExcelWorkbook;
  TaskID, StatusText: string;
begin
  if not Assigned(ExcelSheet) then
  begin
    SetStatusBarInfo(sbiTaskID, '���');
    SetStatusBarInfo(sbiRefreshDate, fmNoRefreshDate);
    SetStatusBarInfo(sbiOnlineMode, '�������');
    SetStatusBarInfo(sbiContext, '���');
    exit;
  end;
  try
    ExcelBook := (ExcelSheet.Parent as ExcelWorkbook);
    TaskId := '���';
    if Assigned(Environment) then
      TaskId := Environment.TaskId;
    SetStatusBarInfo(sbiTaskID, TaskId);
    if PlaningSheet.Load(ExcelSheet, GetContext, [lmInner]) then
    try
      SetStatusBarInfo(sbiRefreshDate, IIF((PlaningSheet.LastRefreshDate <> ''),
        PlaningSheet.LastRefreshDate, fmNoRefreshDate));
      if PlaningSheet.Online then
        if PlaningSheet.InConstructionMode then
          StatusText := '���������������'
        else
          StatusText := '������ � �������'
      else
        StatusText := '����������';
      SetStatusBarInfo(sbiOnlineMode, StatusText);
      {��� �������� ����� �� ����� �������� ���������� ������� ������,
        � �� ����� ��� ������� �������� �� ������ ������������ ����� ��.}
      SetStatusBarInfo(sbiContext, IIF(IsLoadingFromTask or Assigned(Context), '��', '���'));
    finally
      PlaningSheet.Clear;
    end
  except
  end;
end;

function TFMExcelAddIn.PrepareRebuild(LoadMode: TLoadMode; out ExlSheet: ExcelWorkSheet): boolean;
begin
  result := false;
  DisableCopyMode;
  ExlSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExlSheet) then
    exit;
  if TryConnection(false) then
    result := LoadCatalogAndMetadata(LoadMode, ExlSheet)
  else
    PlaningSheet.ShowDetailedError(ermNoneConnection,
      DataProvider.LastError, ermNoneConnection);
end;

function TFMExcelAddin.LoadCatalogAndMetadata(LoadMode: TLoadMode; ESheet: ExcelWorkSheet): boolean;
var
  Op: IOperation;
begin
  result := false;
  Op := CreateComObject(CLASS_Operation) as IOperation;
  try
    {�������� ��������}
    Op.StartOperation(Host.Hwnd);
    Op.Caption := pcapLoadMetadata;
    FXMLCatalog.SetUp(DataProvider);
    if not FXMLCatalog.Loaded then
    begin
      Op.StopOperation;
      ShowDetailError(ermMetaDataLoadFault, DataProvider.LastError,
        ermMetaDataLoadFault);
      exit;
    end;
    if LoadMode = [lmCatalogOnly] then
    begin
      result := true;
      exit;
    end;
    {�������� ����������}
    Op.Caption := pcapCollectMetadata;
    LoadContextFromTask(false);
    if not PlaningSheet.Load(ESheet, Context, LoadMode) then
      exit;
    result := true;
  finally
    //����� ������ �� ����� �����
    Application.ProcessMessages;
    Op.StopOperation;
  end;
end;

procedure TFMExcelAddIn.SetTypeFormulaHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorksheet;
  Mode: TWizardRunMode;
  Id, Query, ErrorText: string;
  Index: integer;
  Total: TSheetTotalInterface;
  TypeFormula: TTypeFormula;
  Selection: ExcelRange;
begin
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  if PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData) then
  try
    MayHook := false;

    Index := PlaningSheet.Totals.FindById(Id);
    Total := PlaningSheet.Totals[Index];
    if not Assigned(Total) then
      exit;

    Selection := Host.ActiveCell;
    if not IsTypeFormulaAllowed(Selection, ErrorText) then
    begin
      ShowError(ErrorText);
      exit;
    end;
    TypeFormula := PlaningSheet.GetTypeFormula(Total, Selection.Row, Selection.Column);
    if not Assigned(TypeFormula) or not TypeFormula.IsValid(ErrorText) then
    begin
      ShowError(ermTypeFormulaFault + ErrorText);
      exit;
    end;

    {��������� ���������}
    PlaningSheet.StartOperation(Host.Hwnd, pfoTypeFormulaRefresh);
    Host.ScreenUpdating[LCID] := false;

    {������� �������� ���������� ������ ������� �������}
    Total.ClearTypeFormulaValues;
    SetCellFormula(ExcelSheet, Selection, '');
    Total.TypeFormula := TypeFormula;
    Total.TypeFormula.Enabled := true;
    PlaningSheet.MapTypeFormula(Total);
    PlaningSheet.Save;
  finally
    MayHook := true;
    PlaningSheet.StopOperation;
    PlaningSheet.Clear;
    Host.ScreenUpdating[LCID] := true;
  end;
end;

procedure TFMExcelAddIn.WorkModeHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  NeedSave: boolean;
begin
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ExcelSheet) then
    exit;
  NeedSave := PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData);
  try
    PlaningSheet.InConstructionMode := not PlaningSheet.InConstructionMode;
    UpdateWorkModeButton(Button);
    PlaningSheet.UpdateAllowEditRanges;
    if NeedSave then
      PlaningSheet.Save;
  finally
    PlaningSheet.Clear;
  end;
  UpdateStatusBar(ExcelSheet);
end;

procedure TFMExcelAddIn.UpdateWorkModeButton(AButton: CommandBarButton);
var
  i, cnt: integer;
  Button: CommandBarButton;
begin
  { ���� ������ ����� ������ �� ���� �������� ����������, �� ������ �� ����}
  if not Assigned(AButton) then
  begin
    if not Assigned(ToolBar) then
      exit;
    cnt := ToolBar.Controls.Count;
    for i := 1 to cnt do
      if (ToolBar.Controls[i].Tag = tagToolButtonWorkMode) then
      begin
        Button := CommandBarButton(ToolBar.Controls[i]);
        break;
      end;
  end
  else
    Button := AButton;

  { ��������� ��� � ����}
  if PlaningSheet.InConstructionMode then
  begin
    {$WARNINGS OFF}
    Button.Set_State(msoButtonDown);
    Button.Set_TooltipText(kriInDataMode);
    {$WARNINGS ON}
  end
  else
  begin
    Button.Set_State(msoButtonUp);
    Button.Set_TooltipText(kriInConstructionMode);
  end;

  { ������� ��������� ������� ������� "��������"}
  with PlaningSheet do
  begin
    UpdateAppendSubmenuItem(true, tagToolButtonTotals, Totals.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonTotals, Totals.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonRows, Rows.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonRows, Rows.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonColumns, Columns.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonColumns, Columns.MayBeEdited);
    UpdateAppendSubmenuItem(true, tagToolButtonFilters, Filters.MayBeEdited);
    UpdateAppendSubmenuItem(false, tagMenuButtonFilters, Filters.MayBeEdited);
    EnableSingleCellAppend;
  end;
  
  RefreshSplitterPad;
end;

procedure TFMExcelAddIn.OfflineFiltersEditorHandler(
  Button: CommandBarButton; var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  IsSuccess: boolean;
  ID, DummyStr, HistoryString: string;
  DummyWRM: TWizardRunMode;
begin
  // ���� ��� �� ���� ������������ - �������
  ExcelSheet := GetWorkSheet(Host.ActiveSheet);
  if not IsPlaningSheet(ExcelSheet) then
    exit;
  DisableCopyMode;
  IsSuccess := false;
  // ������ ����������
  if PlaningSheet.Load(ExcelSheet, Context, lmNoFreeData) then
  try
    SetSheetProtection(ExcelSheet, false);
    Id := '';
    ParseAdditionalInfo(Button.Get_HelpFile, DummyWRM, ID, DummyStr);
    IsSuccess := EditFiltersOffline(PlaningSheet, Id, HistoryString);
    if IsSuccess then
    begin
      PlaningSheet.AddEventInSheetHistory(evtEdit, HistoryString, true);
      PlaningSheet.Save;
    end;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
  end;
end;

procedure TFMExcelAddIn.DataCollectionFormHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
  IsSuccess: boolean;
  HistoryString: string;
begin
  IsSuccess := false;
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExcelSheet) then
  try
    SetSheetProtection(ExcelSheet, false);
    IsSuccess := MakeDataCollectionForm(PlaningSheet, HistoryString);
    Application.ProcessMessages;
    if not IsSuccess then
      exit;

    AncillaryInit;
    FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh,mErrorRefresh, false);
    FProcess.NewProcessClear := false;
    FProcess.OpenOperation(pfoRebuildSheet + ' "' + ExcelSheet.Name + '"',
      not CriticalNode, NoteTime, otProcess);
    CollectDeferredTotalsData;
    PlaningSheet.NeedMapSingleCells := false;
    PlaningSheet.PrepareToSwitchOffline;
    IsSuccess := DoRebuildSheet(ExcelSheet);
    PlaningSheet.AddEventInSheetHistory(evtRefresh,
        {MergeCommaText(HistoryString,} GetErrorList{)}, IsSuccess);

    if not IsSuccess then
    begin
      PlaningSheet.UpdateFiltersText;
      PlaningSheet.UpdateTotalsComments;
    end;

    { ������� ����� � ���������� �����}
    PlaningSheet.SwitchOffline;
    FProcess.OpenOperation(pfoSwitchSheetOffline, not CriticalNode, not NoteTime, otProcess);
    FProcess.CloseOperation;
    PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
      MergeCommaText(ConvertStringToCommaText('���� ��������� � ���������� �����.'),
      HistoryString), true);
    PlaningSheet.Save;
  finally
    AfterRebuild(ExcelSheet, IsSuccess);
    if IsSuccess then
    begin
      if TrySwitchWorkbookOffline then
        PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
          ConvertStringToCommaText('��� ��������� ������� �� "����� ����� ������"'), true);
        // ���� ������, ���� ����� ���-������ ������ ��� ���...
        //FProcess.PostInfo('��� ��������� ������� �� "����� ����� ������"');
        (*PlaningSheet.AddEventInSheetHistory(evtPropertyEdit,
          ConvertStringToCommaText('��� ��������� ������� �� "����� ����� ������"', true);*)
      FProcess.CloseOperation; //pfoRebuild
    end;
    FProcess.CloseProcess;
    UpdateWorkModeButton(nil);
  end;
end;

procedure TFMExcelAddIn.CollectDeferredTotalsData;
var
  tmpCollector: TPlaningSheet;
begin
  if not CheckTotalsForCollect then
    exit;
  tmpCollector := TPlaningSheet.CreateInherited(PlaningSheet);
  tmpCollector.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
  tmpCollector.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
    FfrmSheetHistory);
  tmpCollector.Load(PlaningSheet.ExcelSheet, Context, lmAll);
  PlaningSheet.FreeTotalsData := tmpCollector.FreeTotalsData;
  PlaningSheet.FreeTotalsDataIgnored := tmpCollector.FreeTotalsDataIgnored;
  PlaningSheet.FormulaArrays := tmpCollector.FormulaArrays;
  PlaningSheet.SingleCellsData := tmpCollector.SingleCellsData;
  PlaningSheet.LoadMode := PlaningSheet.LoadMode + [lmFreeData];
  tmpCollector.Free;
end;

function TFMExcelAddIn.TrySwitchWorkbookOffline: boolean;
var
  i: integer;
  Book: ExcelWorkbook;
  Sheet: ExcelWorkSheet;
  tmpPlaningSheet: TPlaningSheet;
  Ok: boolean;
begin
  result := false;
  Book := Host.ActiveWorkbook;
  if not Assigned(Book) then
    exit;
  Ok := true;
  tmpPlaningSheet := TPlaningSheet.CreateInherited(PlaningSheet);
  tmpPlaningSheet.IsTaskConnectionLoad := PlaningSheet.IsTaskConnectionLoad;
  try
    tmpPlaningSheet.SetExternalLinks(FXMLCatalog, FProcess, DataProvider,
      FfrmSheetHistory);
    for i := 1 to Book.Sheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Sheets[i]);
      if not Assigned(Sheet) then
        continue;
      if not IsPlaningSheet(Sheet) then
        continue;
      tmpPlaningSheet.Load(Sheet, Context, lmNoFreeData);
      Ok := Ok and (not tmpPlaningSheet.Online or
        tmpPlaningSheet.Online and tmpPlaningSheet.Empty);
      tmpPlaningSheet.Clear;
    end;
  finally
    tmpPlaningSheet.Free;
  end;
  if Ok then
  begin
    SetPropValueByName(pspSheetType, '3');
    UpdateStatusBar(GetWorkSheet(Host.ActiveSheet));
    result := true;
  end;
end;

procedure TFMExcelAddIn.OnTimerHandler(Sender: TObject);
begin
  {��� � ��� ������ ���������� ���-������, ��� ��� ��� ����}
  DataProvider.ClientSessionIsAlive;
end;

procedure TFMExcelAddIn.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

function TFMExcelAddIn.GetProcessLogFileName: string;
begin
  result := AddinLogPath + 'ProcessLog.txt';
end;

procedure TFMExcelAddIn.UpdateProcessLogging;
begin
  if not Assigned(FProcess) then
    exit;
  if AddinLogEnable then
    FProcess.EnableLogging(GetProcessLogFileName)
  else
    FProcess.DisableLogging;
end;

procedure TFMExcelAddIn.SwitchTotalTypeHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);

var
  Total: TSheetBasicTotal;
  Collection: TSheetCollection;
  ESheet: ExcelWorkSheet;
  IsSuccess: boolean;
  Index: integer;
  Mode: TWizardRunMode;
  Id, tmpString, CommentForHistory: string;
  tmpStringList: TStringList;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;
  DisableCopyMode;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, tmpString);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    with PlaningSheet do
    begin
      if Empty then
        exit;
      if Mode = wrmNone then
        Collection := SingleCells
      else
        Collection := Totals;
      Index := Collection.FindById(Id);
      if Index < 0 then
        exit;
      Total := Collection[Index];
      {���� �� ������� ������ ����� ��� �� � �������� �� ����������� ����� ���� �� �����}
      if Total.TotalType = wtResult then
        tmpString := '��� ���������� %s ����� ������� � ���������� �� ����.' +
          ' ��� ��������� �������� � ������� ����� �������� ������� �� ����. ����������?'
      else
        tmpString := '��� ���������� %s ����� ������� � ���� �� ���������. ����������?';
      if not ShowQuestion(Format(tmpString, [Total.GetElementCaption])) then
        exit;
    end;

    Application.ProcessMessages;
    IsSuccess := false;
    try
      FProcess.OpenProcess(Host.Hwnd, ftRefresh, mSuccessRefresh, mErrorRefresh, false);
      FProcess.NewProcessClear := false;
      FProcess.OpenOperation(pfoRebuildSheet + ' "' + ESheet.Name + '"',
        not CriticalNode, NoteTime, otProcess);
      FUserEvents.Execute(ESheet, enBeforeRefresh);
      FXMLCatalog.SetUp(DataProvider);
      PlaningSheet.Clear;
      if not PlaningSheet.Load(ESheet, Context, lmAll) then
        exit;

      Total := Collection[Index];
      if Mode = wrmNone then
      begin
        TSheetSingleCellInterface(Total).SwitchType(CommentForHistory);
        tmpStringList := TStringList.Create;
        tmpStringList.Add(IntToStr(Index));
        IsSuccess := PlaningSheet.MapSingleCells(tmpStringList, tmpString, true, nil, nil);
        if IsSuccess then
          PlaningSheet.Save;
      end
      else
      begin
        Total.SwitchType(CommentForHistory);
        IsSuccess := DoRebuildSheet(ESheet);
      end;

      CommentForHistory := MergeCommaText(CommentForHistory, GetErrorList);
      PlaningSheet.AddEventInSheetHistory(evtEdit, CommentForHistory,
        IsSuccess);
    finally
      AfterRebuild(ESheet, IsSuccess);
      CalculateSheet(ESheet);
      if IsSuccess then
      begin
        FUserEvents.Execute(ESheet, enAfterRefresh);
        FProcess.CloseOperation; //pfoRebuild
      end;
      FProcess.CloseProcess;
    end;
  finally
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType;
  const Comment: WideString; Success: WordBool);
var
  Book: ExcelWorkBook;
  ESheet: ExcelWorkSheet;
  EventType: TSheetEventType;
  i: integer;
begin
  EventType := TSheetEventType(EType);
  try
    Book := Doc as ExcelWorkBook;
  except
    exit;
  end;

  for i := 1 to Book.Sheets.Count do
  begin
    ESheet := GetWorkSheet(Book.Sheets.Item[i]);
    if not IsPlaningSheet(ESheet) then
      continue;
    FfrmSheetHistory.AddEvent(ESheet, EventType, ConvertStringToCommaText(Comment), Success);
  end
end;

function TFMExcelAddIn.CheckTotalsByType(CheckType: TSheetTotalType): boolean;
var
  i: integer;
begin
  result := true;
  for i := 0 to PlaningSheet.Totals.Count - 1 do
    if (PlaningSheet.Totals[i].TotalType = CheckType) then
      exit;
  for i := 0 to PlaningSheet.SingleCells.Count - 1 do
    if (PlaningSheet.SingleCells[i].TotalType = CheckType) then
      exit;
  result := false;
end;

function TFMExcelAddIn.CheckTotalsForCollect: boolean;
begin
  result := CheckTotalsByType(wtResult) or CheckTotalsByType(wtFree);
end;

function TFMExcelAddIn.CheckTotalsForWriteback: boolean;
begin
  result := CheckTotalsByType(wtResult);
end;

procedure TFMExcelAddIn.CommonOptionsHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  EditCommonOptions(PlaningSheet);
end;

procedure TFMExcelAddIn.OneTotalRefreshHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ESheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  Mode: TWizardRunMode;
  Id, Query: string;
  Index: integer;
  CheckFormulas, Alerts, RefreshResult: boolean;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;

  if not TryConnection(false) then
  begin
    PlaningSheet.ShowDetailedError(ermConnectionFault,
      DataProvider.LastError, ermConnectionFault);
    exit;
  end;

  AncillaryInit;
  FProcess.OpenProcess(Host.Hwnd, '���������� ����������', mSuccessRefresh, mErrorRefresh, false);
  FProcess.NewProcessClear := false;

  FProcess.OpenOperation('���������� ����������', not CriticalNode, NoteTime, otProcess);

  FProcess.OpenOperation(pfoSMDLoad, CriticalNode, NoteTime, otProcess);
  FXMLCatalog.SetUp(DataProvider);
  if not PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
    exit;
  FProcess.CloseOperation;

  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Query);
  Index := PlaningSheet.Totals.FindById(Id);
  if Index < 0 then
    exit;
  try
    if not CheckAxesSaved(ESheet) then
      exit;

    Total := PlaningSheet.Totals[Index];
    FProcess.OpenOperation('���������� ����������' + ' "' + Total.GetElementCaption + '"', not CriticalNode, NoteTime, otProcess);

    if not SetSheetProtection(ESheet, false) then
    begin
      FProcess.PostError(ermWorksheetProtectionFault);
      exit;
    end;
    if not PlaningSheet.Validate(Total) then
    begin
      FProcess.CloseOperation;
      exit;
    end;

    ESheet.Application.ScreenUpdating[LCID] := false;
    ESheet.Application.Set_Interactive(LCID, false);

    {� �������� ��������� ����� ������� ����� ��������� �������������,
    �������, ��� �� ��� ���� �� �������� ����������� ����, �������� ���������
    �������� ������������ ������}
    CheckFormulas := Host.ErrorCheckingOptions.EvaluateToError;
    Host.ErrorCheckingOptions.EvaluateToError := false;
    Alerts := Host.DisplayAlerts[LCID];
    Host.DisplayAlerts[LCID] := false;
    RefreshResult := false;
    try
      RefreshResult := PlaningSheet.MapOneTotal(Total);
    finally
      PlaningSheet.AddEventInSheetHistory(evtRefresh,
        ConvertStringToCommaText(
        Format('���������� ���������� "%s"', [Total.GetElementCaption])),
        RefreshResult);
      //��������������� �������� ������
      Host.ErrorCheckingOptions.EvaluateToError := CheckFormulas;
      Host.DisplayAlerts[LCID] := Alerts;
      FProcess.CloseOperation;
    end;
    FProcess.CloseOperation;
  finally
    AfterRebuild(ESheet, false);
    PlaningSheet.Clear;
    FProcess.CloseProcess;
  end;
end;

procedure TFMExcelAddIn.SetAuthenticationInfoNew;
begin
  if Assigned(Environment) then
    SetAuthenticationInfo(Environment.AuthType, Environment.Login, Environment.PwdHash);
end;

procedure TFMExcelAddIn.SetConnectionStrNew;
begin
  if not Assigned(Environment) then
    exit;
  if SetConnectionStr(Environment.ConnectionStr, Environment.SchemeName) = 0 then
    exit;
  SetConnectionStr(Environment.AlterConnection, Environment.SchemeName);
end;

function TFMExcelAddIn.GetEnvironment: TTaskEnvironment;
begin
  result := FTaskContextProvider.TaskEnvironment[Host.ActiveWorkbook];
end;

procedure TFMExcelAddIn.SetTaskConnectionLoadNew;
begin
  if Assigned(Environment) then
    if Environment.IsTaskConnect then
      OnTaskConnection(true);
end;

procedure TFMExcelAddIn.DoMassAction;
var
  NeedRewrite, NeedProcess, Ok: boolean;
  Msg: string;
begin
  Ok := false;
  Msg := '';
  try
    try
      if Environment.Action and datRefresh = datRefresh then
        RefreshActiveBook;

      if Environment.Action and datWriteback = datWriteback then
      begin
        NeedRewrite := Environment.Action and datRewrite = datRewrite;
        NeedProcess := Environment.Action and datProcessCube = datProcessCube;
        WritebackActiveBookEx(NeedRewrite, NeedProcess);
      end;

      if Environment.Action and datRefreshAfter = datRefreshAfter then
        RefreshActiveBook;

      Ok := FProcess.LastError = '';
      if not Ok then
        Msg := FProcess.LastError;
    except
    end;
  finally
    Environment.SetActionResult(Host.ActiveWorkbook, Ok, Msg);
  end;
end;

function TFMExcelAddIn.GetStatic: boolean;
begin
  if Assigned(PlaningSheet.Environment) then
    result := PlaningSheet.Environment.IsLoadingFromTask
  else
    result := false;
end;

procedure TFMExcelAddIn.SetTaskContext(const TaskContextXml: Widestring;
  IsPacked: WordBool);
begin
;
end;

function TFMExcelAddIn.CheckAxesSaved(ExcelSheet: ExcelWorkSheet): boolean;
var
  RowsCPSaved, ColumnsCPSaved: boolean;
begin
  RowsCPSaved := Assigned(GetCPByName(ExcelSheet, cpRowsAxis, false));
  ColumnsCPSaved := Assigned(GetCPByName(ExcelSheet, cpColumnsAxis, false));
  result := (not PlaningSheet.Rows.Empty and RowsCPSaved) or
    (not PlaningSheet.Columns.Empty and ColumnsCPSaved) or
    (PlaningSheet.Rows.Empty and PlaningSheet.Columns.Empty);
  if not Result then
    PlaningSheet.PostMessage('���������� ������ ���������� �����', msgError);
end;

procedure TFMExcelAddIn.LoadContextFromTask(ShowGears: boolean);
var
  TaskId: integer;
  ContextXml: string;
  Op: IOperation;
begin
  if not Environment.IsLoadingFromTask then
    exit;
  try
    try
      if ShowGears then
      begin
        Op := CreateComObject(CLASS_OPERATION) as IOperation;
        Op.StartOperation(Host.Hwnd);
        Op.Caption := '���������� ���������';
      end;

      TaskId := StrToInt(Environment.TaskId);
      ContextXml := DataProvider.GetTaskContext(TaskId);
    except
      ContextXml := '';
    end;
    FTaskContextProvider.UpdateContext(Host.ActiveWorkbook, ContextXml);
  finally
    if ShowGears then
    begin
      Op.StopOperation;
      Op := nil;
    end;
  end;
end;

procedure TFMExcelAddIn.OnProcessFormReturn;
begin
  RebuildCycleState := rcsReturn;
end;

procedure TFMExcelAddIn.OnProcessFormClose;
begin
  if RebuildCycleState <> rcsReturn then
    RebuildCycleState := rcsLeave;
  if Host.Visible[LCID] then
  try
    Host.Application.Set_Interactive(LCID, true);
  except
    on e: Exception do ShowError(e.message);
  end;
end;

procedure TFMExcelAddIn.OnProcessFormShow;
begin
  if Host.Visible[LCID] then
    Host.Application.Set_Interactive(LCID, false);
end;

procedure TFMExcelAddIn.FreezePanesHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
begin
  Host.ActiveWindow.FreezePanes := not Host.ActiveWindow.FreezePanes;
end;

procedure TFMExcelAddIn.ReplicationHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ExcelSheet: ExcelWorkSheet;
begin
  if PrepareRebuild(lmNoFreeData + [lmWithWizard], ExcelSheet) then
  begin
    RunReplicationWizard(PlaningSheet, FfrmSheetHistory);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.SheetInfoHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  ESheet: ExcelWorksheet;
begin
  if PrepareRebuild(lmNoFreeData, ESheet) then
  begin
    ViewSheetInfo(PlaningSheet);
    PlaningSheet.Clear;
  end;
end;

procedure TFMExcelAddIn.HideTotalColumnsHandler(Button: CommandBarButton;
  var CancelDefault: WordBool);
var
  Mode: TWizardRunMode;
  Id, Question: string;
  Index, i: integer;
  ESheet: ExcelWorkSheet;
  Total: TSheetTotalInterface;
  Section: ExcelRange;
begin
  ESheet := GetWorkSheet(Host.ActiveSheet);
  if not Assigned(ESheet) then
    exit;
  if (Button.HelpFile = '') then
    exit;
  ParseAdditionalInfo(Button.HelpFile, Mode, Id, Question);

  if PlaningSheet.Load(ESheet, Context, lmNoFreeData) then
  try
    Index := PlaningSheet.Totals.FindById(Id);
    if Index < 0 then
      exit;
    Total := PlaningSheet.Totals[Index];
    Total.IsHidden := true;
    PlaningSheet.Save;
    SetSheetProtection(ESheet, false);
    for i := 0 to Total.SectionCount - 1 do
    begin
      Section := Total.GetTotalRange(i);
      if not Assigned(Section) then
        continue;
      Section.EntireColumn.Hidden := true;
    end;
    SetSheetProtection(ESheet, false);
  finally
    PlaningSheet.Clear;
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TFMExcelAddIn, Class_DTExtensibility2,
    ciMultiInstance, tmApartment);
end.


