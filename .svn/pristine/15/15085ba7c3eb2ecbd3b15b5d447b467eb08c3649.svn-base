unit ExcelComAddinEvents;

interface

uses
  Windows, ActiveX, ExcelXP, ComAddInUtils;

type
  { прототипы функций-обработчиков событий Excel Workbook }
  TOnWorkbookOpen = procedure of object;// dispid 1923;
  TOnWorkbookActivate = procedure of object;// dispid 304;
  TOnWorkbookDeactivate = procedure of object;// dispid 1530;
  TOnWorkbookBeforeClose = procedure(var Cancel: WordBool) of
    object;// dispid 1546;
  TOnWorkbookBeforeSave = procedure(SaveAsUI: WordBool;
    var Cancel: WordBool) of object;// dispid 1547;
  TOnWorkbookBeforePrint = procedure(var Cancel: WordBool) of
    object;// dispid 1549;
  TOnWorkbookNewSheet = procedure(const Sh: IDispatch) of object;//dispid 1550;
  TOnWorkbookAddinInstall = procedure of object;// dispid 1552;
  TOnWorkbookAddinUninstall = procedure of object;// dispid 1553;
  TOnWorkbookWindowResize = procedure(const Wn: ExcelXP.Window) of object;// dispid 1554;
  TOnWorkbookWindowActivate = procedure(const Wn: ExcelXP.Window) of object;// dispid 1556;
  TOnWorkbookWindowDeactivate = procedure(const Wn: ExcelXP.Window) of object;// dispid 1557;
  TOnWorkbookSheetSelectionChange = procedure(const Sh: IDispatch;
    const Target: ExcelRange) of object;// dispid 1558;
  TOnWorkbookSheetBeforeDoubleClick = procedure(const Sh: IDispatch;
    const Target: ExcelRange; var Cancel: WordBool) of object;// dispid 1559;
  TOnWorkbookSheetBeforeRightClick = procedure(const Sh: IDispatch;
    const Target: ExcelRange; var Cancel: WordBool) of object;// dispid 1560;
  TOnWorkbookSheetActivate = procedure(const Sh: IDispatch) of object;// dispid 1561;
  TOnWorkbookSheetDeactivate = procedure(const Sh: IDispatch) of object;// dispid 1562;
  TOnWorkbookSheetCalculate = procedure(const Sh: IDispatch) of object;// dispid 1563;
  TOnWorkbookSheetChange = procedure(const Sh: IDispatch;
    const Target: ExcelRange) of object;// dispid 1564;
  TOnWorkbookSheetFollowHyperlink = procedure(const Sh: IDispatch;
    const Target: Hyperlink) of object;// dispid 1854;
  TOnWorkbookSheetPivotTableUpdate = procedure(const Sh: IDispatch;
    const Target: PivotTable) of object;// dispid 2157;
  TOnWorkbookPivotTableCloseConnection = procedure
    (const Target: PivotTable) of object;// dispid 2158;
  TOnWorkbookPivotTableOpenConnection = procedure
    (const Target: PivotTable) of object;// dispid 2159;

  { прототипы функций-обработчиков событий Excel Application }
  TOnExcelAppNewWorkbook  = procedure(const Wb: ExcelWorkbook)
      of object;// dispid 1565;
(*  procedure SheetSelectionChange(const Sh: IDispatch; const Target: ExcelRange); dispid 1558;
    procedure SheetBeforeDoubleClick(const Sh: IDispatch; const Target: ExcelRange;
                                    var Cancel: WordBool); dispid 1559;
    procedure SheetBeforeRightClick(const Sh: IDispatch; const Target: ExcelRange;
                                    var Cancel: WordBool); dispid 1560;
    procedure SheetActivate(const Sh: IDispatch); dispid 1561;
    procedure SheetDeactivate(const Sh: IDispatch); dispid 1562;
    procedure SheetCalculate(const Sh: IDispatch); dispid 1563;
    procedure SheetChange(const Sh: IDispatch; const Target: ExcelRange); dispid 1564; *)
  TOnExcelAppWorkbookOpen = procedure(const Wb: ExcelWorkbook)
    of object;// dispid 1567;
  TOnExcelAppWorkbookActivate = procedure(const Wb: ExcelWorkbook)
    of object;// dispid 1568;
  TOnExcelAppWorkbookDeactivate = procedure(const Wb: ExcelWorkbook)
    of object;// dispid 1569;
  TOnExcelAppWorkbookBeforeClose = procedure(const Wb: ExcelWorkbook;
    var Cancel: WordBool) of object;// dispid 1570;
  TOnExcelAppWorkbookBeforeSave = procedure(const Wb: ExcelWorkbook;
    SaveAsUI: WordBool; var Cancel: WordBool) of object;// dispid 1571;
  TOnExcelAppWorkbookBeforePrint = procedure(const Wb: ExcelWorkbook;
    var Cancel: WordBool) of object;// dispid 1572;
  TOnExcelAppWorkbookNewSheet = procedure(const Wb: ExcelWorkbook;
    const Sh: IDispatch) of object;// dispid 1573;
  TOnExcelAppWorkbookAddinInstall = procedure(const Wb: ExcelWorkbook)
    of object;// dispid 1574;
  TOnExcelAppWorkbookAddinUninstall = procedure(const Wb: ExcelWorkbook)
    of object;// dispid 1575;
  TOnExcelAppWindowResize = procedure(const Wb: ExcelWorkbook;
    const Wn: ExcelXP.Window) of object;// dispid 1554;
  TOnExcelAppWindowActivate = procedure(const Wb: ExcelWorkbook;
   const Wn: ExcelXP.Window) of object;// dispid 1556;
  TOnExcelAppWindowDeactivate = procedure(const Wb: ExcelWorkbook;
   const Wn: ExcelXP.Window) of object;// dispid 1557;
(* procedure SheetFollowHyperlink(const Sh: IDispatch; const Target: Hyperlink); dispid 1854;
   procedure SheetPivotTableUpdate(const Sh: IDispatch; const Target: PivotTable); dispid 2157; *)
  TOnExcelAppWorkbookPivotTableCloseConnection = procedure
    (const Wb: ExcelWorkbook; const Target: PivotTable) of object;// dispid 2160;
  TOnExcelAppWorkbookPivotTableOpenConnection = procedure
    (const Wb: ExcelWorkbook; const Target: PivotTable) of object;// dispid 2161;

  {класс - обработчик событий ExcelWorkbook}
  TWorkbookEventSink = class(TBaseSink)
  private
    FOnWorkbookOpen: TOnWorkbookOpen;
    FOnWorkbookActivate: TOnWorkbookActivate;
    FOnWorkbookDeactivate: TOnWorkbookDeactivate;
    FOnWorkbookBeforeClose: TOnWorkbookBeforeClose;
    FOnWorkbookBeforeSave: TOnWorkbookBeforeSave;
    FOnWorkbookBeforePrint: TOnWorkbookBeforePrint;
    FOnWorkbookNewSheet: TOnWorkbookNewSheet;
    FOnWorkbookAddinInstall: TOnWorkbookAddinInstall;
    FOnWorkbookAddinUninstall: TOnWorkbookAddinUninstall;
    FOnWorkbookWindowResize: TOnWorkbookWindowResize;
    FOnWorkbookWindowActivate: TOnWorkbookWindowActivate;
    FOnWorkbookWindowDeactivate: TOnWorkbookWindowDeactivate;
    FOnWorkbookSheetSelectionChange: TOnWorkbookSheetSelectionChange;
    FOnWorkbookSheetBeforeDoubleClick: TOnWorkbookSheetBeforeDoubleClick;
    FOnWorkbookSheetBeforeRightClick: TOnWorkbookSheetBeforeRightClick;
    FOnWorkbookSheetActivate: TOnWorkbookSheetActivate;
    FOnWorkbookSheetDeactivate: TOnWorkbookSheetDeactivate;
    FOnWorkbookSheetCalculate: TOnWorkbookSheetCalculate;
    FOnWorkbookSheetChange: TOnWorkbookSheetChange;
    FOnWorkbookSheetFollowHyperlink: TOnWorkbookSheetFollowHyperlink;
    FOnWorkbookSheetPivotTableUpdate: TOnWorkbookSheetPivotTableUpdate;
    FOnWorkbookPivotTableCloseConnection: TOnWorkbookPivotTableCloseConnection;
    FOnWorkbookPivotTableOpenConnection: TOnWorkbookPivotTableOpenConnection;
  protected
    function DoInvoke (DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
    procedure DoOpen; virtual;
    procedure DoActivate; virtual;
    procedure DoDeactivate; virtual;
    procedure DoBeforeClose(var Cancel: WordBool); virtual;
    procedure DoBeforeSave(SaveAsUI: WordBool; var Cancel: WordBool); virtual;
    procedure DoBeforePrint(var Cancel: WordBool); virtual;
    procedure DoNewSheet(const Sh: IDispatch); virtual;
    procedure DoAddinInstall; virtual;
    procedure DoAddinUninstall; virtual;
    procedure DoWindowResize(const Wn: ExcelXP.Window); virtual;
    procedure DoWindowActivate(const Wn: ExcelXP.Window); virtual;
    procedure DoWindowDeactivate(const Wn: ExcelXP.Window); virtual;
    procedure DoSheetSelectionChange(const Sh: IDispatch;
      const Target: ExcelRange); virtual;
    procedure DoSheetBeforeDoubleClick(const Sh: IDispatch;
      const Target: ExcelRange; var Cancel: WordBool); virtual;
    procedure DoSheetBeforeRightClick(const Sh: IDispatch;
      const Target: ExcelRange; var Cancel: WordBool); virtual;
    procedure DoSheetActivate(const Sh: IDispatch); virtual;
    procedure DoSheetDeactivate(const Sh: IDispatch); virtual;
    procedure DoSheetCalculate(const Sh: IDispatch); virtual;
    procedure DoSheetChange(const Sh: IDispatch;
      const Target: ExcelRange); virtual;
    procedure DoSheetFollowHyperlink(const Sh: IDispatch;
      const Target: Hyperlink); virtual;
    procedure DoSheetPivotTableUpdate(const Sh: IDispatch;
      const Target: PivotTable); virtual;
    procedure DoPivotTableCloseConnection(const Target: PivotTable); virtual;
    procedure DoPivotTableOpenConnection(const Target: PivotTable); virtual;
  public
    constructor Create; virtual;
    property OnWorkbookOpen: TOnWorkbookOpen read FOnWorkbookOpen
      write FOnWorkbookOpen;
    property OnWorkbookActivate: TOnWorkbookActivate read FOnWorkbookActivate
      write FOnWorkbookActivate;
    property OnWorkbookDeactivate: TOnWorkbookDeactivate
      read FOnWorkbookDeactivate write FOnWorkbookDeactivate;
    property OnWorkbookBeforeClose: TOnWorkbookBeforeClose
      read FOnWorkbookBeforeClose write FOnWorkbookBeforeClose;
    property OnWorkbookBeforeSave: TOnWorkbookBeforeSave
      read FOnWorkbookBeforeSave write FOnWorkbookBeforeSave;
    property OnWorkbookBeforePrint: TOnWorkbookBeforePrint
      read FOnWorkbookBeforePrint write FOnWorkbookBeforePrint;
    property OnWorkbookNewSheet: TOnWorkbookNewSheet
      read FOnWorkbookNewSheet write FOnWorkbookNewSheet;
    property OnWorkbookAddinInstall: TOnWorkbookAddinInstall
      read FOnWorkbookAddinInstall write FOnWorkbookAddinInstall;
    property OnWorkbookAddinUninstall: TOnWorkbookAddinUninstall
      read FOnWorkbookAddinUninstall write FOnWorkbookAddinUninstall;
    property OnWorkbookWindowResize: TOnWorkbookWindowResize
      read FOnWorkbookWindowResize write FOnWorkbookWindowResize;
    property OnWorkbookWindowActivate: TOnWorkbookWindowActivate
      read FOnWorkbookWindowActivate write FOnWorkbookWindowActivate;
    property OnWorkbookWindowDeactivate: TOnWorkbookWindowDeactivate
      read FOnWorkbookWindowDeactivate write FOnWorkbookWindowDeactivate;
    property OnWorkbookSheetSelectionChange: TOnWorkbookSheetSelectionChange
      read FOnWorkbookSheetSelectionChange
      write FOnWorkbookSheetSelectionChange;
    property OnWorkbookSheetBeforeDoubleClick: TOnWorkbookSheetBeforeDoubleClick
      read FOnWorkbookSheetBeforeDoubleClick
      write FOnWorkbookSheetBeforeDoubleClick;
    property OnWorkbookSheetBeforeRightClick: TOnWorkbookSheetBeforeRightClick
      read FOnWorkbookSheetBeforeRightClick
      write FOnWorkbookSheetBeforeRightClick;
    property OnWorkbookSheetActivate: TOnWorkbookSheetActivate
      read FOnWorkbookSheetActivate write FOnWorkbookSheetActivate;
    property OnWorkbookSheetDeactivate: TOnWorkbookSheetDeactivate
      read FOnWorkbookSheetDeactivate write FOnWorkbookSheetDeactivate;
    property OnWorkbookSheetCalculate: TOnWorkbookSheetCalculate
      read FOnWorkbookSheetCalculate write FOnWorkbookSheetCalculate;
    property OnWorkbookSheetChange: TOnWorkbookSheetChange
      read FOnWorkbookSheetChange write FOnWorkbookSheetChange;
    property OnWorkbookSheetFollowHyperlink: TOnWorkbookSheetFollowHyperlink
      read FOnWorkbookSheetFollowHyperlink
      write FOnWorkbookSheetFollowHyperlink;
    property OnWorkbookSheetPivotTableUpdate: TOnWorkbookSheetPivotTableUpdate
      read FOnWorkbookSheetPivotTableUpdate
      write FOnWorkbookSheetPivotTableUpdate;
    property OnWorkbookPivotTableCloseConnection:
      TOnWorkbookPivotTableCloseConnection
      read FOnWorkbookPivotTableCloseConnection
      write FOnWorkbookPivotTableCloseConnection;
    property OnWorkbookPivotTableOpenConnection:
      TOnWorkbookPivotTableOpenConnection
      read FOnWorkbookPivotTableOpenConnection
      write FOnWorkbookPivotTableOpenConnection;
  end;

  {класс - обработчик событий ExcelApplication}
  TExcelAppEventSink = class(TBaseSink)
  private
    FOnExcelAppNewWorkbook: TOnExcelAppNewWorkbook;
    FOnExcelAppSheetSelectionChange: TOnWorkbookSheetSelectionChange;
    FOnExcelAppSheetBeforeDoubleClick: TOnWorkbookSheetBeforeDoubleClick;
    FOnExcelAppSheetBeforeRightClick: TOnWorkbookSheetBeforeRightClick;
    FOnExcelAppSheetActivate: TOnWorkbookSheetActivate;
    FOnExcelAppSheetDeactivate: TOnWorkbookSheetDeactivate;
    FOnExcelAppSheetCalculate: TOnWorkbookSheetCalculate;
    FOnExcelAppSheetChange: TOnWorkbookSheetChange;
    FOnExcelAppWorkbookOpen: TOnExcelAppWorkbookOpen;
    FOnExcelAppWorkbookActivate: TOnExcelAppWorkbookActivate;
    FOnExcelAppWorkbookDeactivate: TOnExcelAppWorkbookDeactivate;
    FOnExcelAppWorkbookBeforeClose: TOnExcelAppWorkbookBeforeClose;
    FOnExcelAppWorkbookBeforeSave: TOnExcelAppWorkbookBeforeSave;
    FOnExcelAppWorkbookBeforePrint: TOnExcelAppWorkbookBeforePrint;
    FOnExcelAppWorkbookNewSheet: TOnExcelAppWorkbookNewSheet;
    FOnExcelAppWorkbookAddinInstall: TOnExcelAppWorkbookAddinInstall;
    FOnExcelAppWorkbookAddinUninstall: TOnExcelAppWorkbookAddinUninstall;
    FOnExcelAppWindowResize: TOnExcelAppWindowResize;
    FOnExcelAppWindowActivate: TOnExcelAppWindowActivate;
    FOnExcelAppWindowDeactivate: TOnExcelAppWindowDeactivate;
    FOnExcelAppSheetFollowHyperlink: TOnWorkbookSheetFollowHyperlink;
    FOnExcelAppSheetPivotTableUpdate: TOnWorkbookSheetPivotTableUpdate;
    FOnExcelAppWorkbookPivotTableCloseConnection:
      TOnExcelAppWorkbookPivotTableCloseConnection;
    FOnExcelAppWorkbookPivotTableOpenConnection:
      TOnExcelAppWorkbookPivotTableOpenConnection;
  protected
    function DoInvoke(DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs : TDispParams; PDispIDs : PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
    procedure DoNewWorkbook(const Wb: ExcelWorkbook);
    procedure DoSheetSelectionChange(const Sh: IDispatch;
      const Target: ExcelRange);
    procedure DoSheetBeforeDoubleClick(const Sh: IDispatch;
      const Target: ExcelRange; var Cancel: WordBool);
    procedure DoSheetBeforeRightClick(const Sh: IDispatch;
      const Target: ExcelRange; var Cancel: WordBool);
    procedure DoSheetActivate(const Sh: IDispatch);
    procedure DoSheetDeactivate(const Sh: IDispatch);
    procedure DoSheetCalculate(const Sh: IDispatch);
    procedure DoSheetChange(const Sh: IDispatch;
      const Target: ExcelRange);
    procedure DoWorkbookOpen(const Wb: ExcelWorkbook);
    procedure DoWorkbookActivate(const Wb: ExcelWorkbook);
    procedure DoWorkbookDeactivate(const Wb: ExcelWorkbook);
    procedure DoWorkbookBeforeClose(const Wb: ExcelWorkbook;
      var Cancel: WordBool);
    procedure DoWorkbookBeforeSave(const Wb: ExcelWorkbook;
      SaveAsUI: WordBool; var Cancel: WordBool);
    procedure DoWorkbookBeforePrint(const Wb: ExcelWorkbook;
      var Cancel: WordBool);
    procedure DoWorkbookNewSheet(const Wb: ExcelWorkbook;
      const Sh: IDispatch);
    procedure DoWorkbookAddinInstall(const Wb: ExcelWorkbook);
    procedure DoWorkbookAddinUninstall(const Wb: ExcelWorkbook);
    procedure DoWindowResize(const Wb: ExcelWorkbook; const Wn: ExcelXP.Window);
    procedure DoWindowActivate(const Wb: ExcelWorkbook; const Wn: ExcelXP.Window);
    procedure DoWindowDeactivate(const Wb: ExcelWorkbook; const Wn: ExcelXP.Window);
    procedure DoSheetFollowHyperlink(const Sh: IDispatch;
      const Target: Hyperlink);
    procedure DoSheetPivotTableUpdate(const Sh: IDispatch;
      const Target: PivotTable);
    procedure DoWorkbookPivotTableCloseConnection(const Wb: ExcelWorkbook;
      const Target: PivotTable);
    procedure DoWorkbookPivotTableOpenConnection(const Wb: ExcelWorkbook;
      const Target: PivotTable);
  public
    constructor Create; virtual;
    property OnExcelAppNewWorkbook: TOnExcelAppNewWorkbook
      read FOnExcelAppNewWorkbook write FOnExcelAppNewWorkbook;
    property OnExcelAppSheetSelectionChange: TOnWorkbookSheetSelectionChange
      read FOnExcelAppSheetSelectionChange write FOnExcelAppSheetSelectionChange;
    property OnExcelAppSheetBeforeDoubleClick: TOnWorkbookSheetBeforeDoubleClick
      read FOnExcelAppSheetBeforeDoubleClick
      write FOnExcelAppSheetBeforeDoubleClick;
    property OnExcelAppSheetBeforeRightClick: TOnWorkbookSheetBeforeRightClick
      read FOnExcelAppSheetBeforeRightClick
      write FOnExcelAppSheetBeforeRightClick;
    property OnExcelAppSheetActivate: TOnWorkbookSheetActivate
      read FOnExcelAppSheetActivate write FOnExcelAppSheetActivate;
    property OnExcelAppSheetDeactivate: TOnWorkbookSheetDeactivate
      read FOnExcelAppSheetDeactivate write FOnExcelAppSheetDeactivate;
    property OnExcelAppSheetCalculate: TOnWorkbookSheetCalculate
      read FOnExcelAppSheetCalculate write FOnExcelAppSheetCalculate;
    property OnExcelAppSheetChange: TOnWorkbookSheetChange
      read FOnExcelAppSheetChange write FOnExcelAppSheetChange;
    property OnExcelAppWorkbookOpen: TOnExcelAppWorkbookOpen
      read FOnExcelAppWorkbookOpen write FOnExcelAppWorkbookOpen;
    property OnExcelAppWorkbookActivate: TOnExcelAppWorkbookActivate
      read FOnExcelAppWorkbookActivate write FOnExcelAppWorkbookActivate;
    property OnExcelAppWorkbookDeactivate: TOnExcelAppWorkbookDeactivate
      read FOnExcelAppWorkbookDeactivate write FOnExcelAppWorkbookDeactivate;
    property OnExcelAppWorkbookBeforeClose: TOnExcelAppWorkbookBeforeClose
      read FOnExcelAppWorkbookBeforeClose write FOnExcelAppWorkbookBeforeClose;
    property OnExcelAppWorkbookBeforeSave: TOnExcelAppWorkbookBeforeSave
      read FOnExcelAppWorkbookBeforeSave write FOnExcelAppWorkbookBeforeSave;
    property OnExcelAppWorkbookBeforePrint: TOnExcelAppWorkbookBeforePrint
      read FOnExcelAppWorkbookBeforePrint write FOnExcelAppWorkbookBeforePrint;
    property OnExcelAppWorkbookNewSheet: TOnExcelAppWorkbookNewSheet
      read FOnExcelAppWorkbookNewSheet write FOnExcelAppWorkbookNewSheet;
    property OnExcelAppWorkbookAddinInstall: TOnExcelAppWorkbookAddinInstall
      read FOnExcelAppWorkbookAddinInstall write FOnExcelAppWorkbookAddinInstall;
    property OnExcelAppWorkbookAddinUninstall: TOnExcelAppWorkbookAddinUninstall
      read FOnExcelAppWorkbookAddinUninstall
      write FOnExcelAppWorkbookAddinUninstall;
    property OnExcelAppWindowResize: TOnExcelAppWindowResize
      read FOnExcelAppWindowResize write FOnExcelAppWindowResize;
    property OnExcelAppWindowActivate: TOnExcelAppWindowActivate
      read FOnExcelAppWindowActivate write FOnExcelAppWindowActivate;
    property OnExcelAppWindowDeactivate: TOnExcelAppWindowDeactivate
      read FOnExcelAppWindowDeactivate write FOnExcelAppWindowDeactivate;
    property OnExcelAppSheetFollowHyperlink: TOnWorkbookSheetFollowHyperlink
      read FOnExcelAppSheetFollowHyperlink
      write FOnExcelAppSheetFollowHyperlink;
    property OnExcelAppSheetPivotTableUpdate: TOnWorkbookSheetPivotTableUpdate
      read FOnExcelAppSheetPivotTableUpdate
      write FOnExcelAppSheetPivotTableUpdate;
    property OnExcelAppWorkbookPivotTableCloseConnection:
      TOnExcelAppWorkbookPivotTableCloseConnection
      read FOnExcelAppWorkbookPivotTableCloseConnection
      write FOnExcelAppWorkbookPivotTableCloseConnection;
    property OnExcelAppWorkbookPivotTableOpenConnection:
      TOnExcelAppWorkbookPivotTableOpenConnection
      read FOnExcelAppWorkbookPivotTableOpenConnection
      write FOnExcelAppWorkbookPivotTableOpenConnection;
  end;

implementation
uses
  SysUtils;

{ TWorkbookEventSink }

constructor TWorkbookEventSink.Create;
begin
  inherited;
  FSinkIID := WorkbookEvents;
end;

function TWorkbookEventSink.DoInvoke(DispID: integer;
  const IID: TGUID; LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    1923: DoOpen;
    304: DoActivate;
    1530: DoDeactivate;
    1546: DoBeforeClose(DPs.rgvarg^[PDispIDs^[1]].pbool^);
    1547: DoBeforeSave(DPs.rgvarg^[PDispIDs^[0]].pbool^,
      DPs.rgvarg^[PDispIDs^[1]].pbool^);
    1549: DoBeforePrint(DPs.rgvarg^ [PDispIDs^[0]].pbool^);
    1550: DoNewSheet(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval) as IDispatch);
    1552: DoAddinInstall;
    1553: DoAddinUninstall;
    1554: DoWindowResize(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelXP.Window);
    1556: DoWindowActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelXP.Window);
    1557: DoWindowDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelXP.Window);
    1558: DoSheetSelectionChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange);
    1559: DoSheetBeforeDoubleClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange, dps.rgvarg^ [pDispIds^[2]].pbool^);
    1560: DoSheetBeforeRightClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange, DPs.rgvarg^ [PDispIDs^[2]].pbool^);
    1561: DoSheetActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1562: DoSheetDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1563: DoSheetCalculate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1564: DoSheetChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange);
    1854: DoSheetFollowHyperlink(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as Hyperlink);
    2157: DoSheetPivotTableUpdate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as PivotTable);
    2158: DoPivotTableCloseConnection(IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as PivotTable);
    2159: DoPivotTableOpenConnection(IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as PivotTable);
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

procedure TWorkbookEventSink.DoOpen;
begin
  if Assigned(FOnWorkbookOpen) then
    FOnWorkbookOpen;
end;

procedure TWorkbookEventSink.DoActivate;
begin
  if Assigned(FOnWorkbookActivate) then
    FOnWorkbookActivate;
end;

procedure TWorkbookEventSink.DoDeactivate;
begin
  if Assigned(FOnWorkbookDeactivate) then
    FOnWorkbookDeactivate;
end;

procedure TWorkbookEventSink.DoBeforeClose(var Cancel: WordBool);
begin
  if Assigned(FOnWorkbookBeforeClose) then
    FOnWorkbookBeforeClose(Cancel);
end;

procedure TWorkbookEventSink.DoBeforeSave(SaveAsUI: WordBool;
  var Cancel: WordBool);
begin
  if Assigned(FOnWorkbookBeforeSave) then
    FOnWorkbookBeforeSave(SaveAsUI, Cancel);
end;

procedure TWorkbookEventSink.DoBeforePrint(var Cancel: WordBool);
begin
  if Assigned(FonWorkbookBeforeprint) then
    FonWorkbookBeforePrint(Cancel);
end;

procedure TWorkbookEventSink.DoNewSheet(const Sh: IDispatch);
begin
  if Assigned(FOnWorkbookNewSheet) then
    FOnWorkbookNewSheet(Sh);
end;

procedure TWorkbookEventSink.DoAddinInstall;
begin
  if Assigned(FOnWorkbookAddinInstall) then
    FOnWorkbookAddinInstall;
end;

procedure TWorkbookEventSink.DoAddinUninstall;
begin
  if Assigned(FOnWorkbookAddinUninstall) then
    FOnWorkbookAddinUninstall;
end;

procedure TWorkbookEventSink.DoWindowResize(const Wn: ExcelXP.Window);
begin
  if Assigned(FOnWorkbookWindowResize) then
    FOnWorkbookWindowResize(Wn);
end;

procedure TWorkbookEventSink.DoWindowActivate(const Wn: ExcelXP.Window);
begin
  if Assigned(FOnWorkbookWindowActivate) then
    FOnWorkbookWindowActivate(Wn);
end;

procedure TWorkbookEventSink.DoWindowDeactivate(const Wn: ExcelXP.Window);
begin
  if Assigned(FOnWorkbookWindowDeactivate) then
    FOnWorkbookWindowDeactivate(Wn);
end;

procedure TWorkbookEventSink.DoSheetSelectionChange(const Sh: IDispatch;
  const Target: ExcelRange);
begin
  if Assigned(FOnWorkbookSheetSelectionChange) then
    FOnWorkbookSheetSelectionChange(Sh, Target);
end;


procedure TWorkbookEventSink.DoSheetBeforeDoubleClick(const Sh: IDispatch;
  const Target: ExcelRange; var Cancel: WordBool);
begin
  if Assigned(FOnWorkbookSheetBeforeDoubleClick) then
    FOnWorkbookSheetBeforeDoubleClick(Sh, Target, Cancel);
end;

procedure TWorkbookEventSink.DoSheetBeforeRightClick(const Sh: IDispatch;
  const Target: ExcelRange; var Cancel: WordBool);
begin
  if Assigned(FOnWorkbookSheetBeforeRightClick) then
    FOnWorkbookSheetBeforeRightClick(Sh, Target, Cancel);
end;

procedure TWorkbookEventSink.DoSheetActivate(const Sh: IDispatch);
begin
  if Assigned(FOnWorkbookSheetActivate) then
    FOnWorkbookSheetActivate(Sh);
end;

procedure TWorkbookEventSink.DoSheetDeactivate(const Sh: IDispatch);
begin
  if Assigned(FOnWorkbookSheetDeactivate) then
    FOnWorkbookSheetDeactivate(Sh);
end;

procedure TWorkbookEventSink.DoSheetCalculate(const Sh: IDispatch);
begin
  if Assigned(FOnWorkbookSheetCalculate) then
    FOnWorkbookSheetCalculate(Sh);
end;

procedure TWorkbookEventSink.DoSheetChange(const Sh: IDispatch;
  const Target: ExcelRange);
begin
  if Assigned(FOnWorkbookSheetChange) then
    FOnWorkbookSheetChange(Sh, Target);
end;

procedure TWorkbookEventSink.DoSheetFollowHyperlink(const Sh: IDispatch;
  const Target: Hyperlink);
begin
  if Assigned(FOnWorkbookSheetFollowHyperlink) then
    FOnWorkbookSheetFollowHyperlink(Sh, Target);
end;

procedure TWorkbookEventSink.DoSheetPivotTableUpdate(const Sh: IDispatch;
  const Target: PivotTable);
begin
  if Assigned(FOnWorkbookSheetPivotTableUpdate) then
    FOnWorkbookSheetPivotTableUpdate(Sh, Target);
end;

procedure TWorkbookEventSink.DoPivotTableCloseConnection
 (const Target: PivotTable);
begin
  if Assigned(FOnWorkbookPivotTableCloseConnection) then
    FOnWorkbookPivotTableCloseConnection(Target);
end;

procedure TWorkbookEventSink.DoPivotTableOpenConnection
  (const Target: PivotTable);
begin
  if Assigned(FOnWorkbookPivotTableCloseConnection) then
    FOnWorkbookPivotTableCloseConnection(Target);
end;

{ TExcelAppEventSink }

constructor TExcelAppEventSink.Create;
begin
  inherited;
  FSinkIID := AppEvents;
end;

function TExcelAppEventSink.DoInvoke(DispID: integer; const IID: TGUID;
  LocaleID: integer; Flags: word; var DPs : TDispParams;
  PDispIDs : PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    1565: DoNewWorkbook(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1558: DoSheetSelectionChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange);
    1559: DoSheetBeforeDoubleClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange, DPs.rgvarg^ [PDispIDs^[2]].pbool^);
    1560: DoSheetBeforeRightClick(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange, DPs.rgvarg^ [PDispIDs^[2]].pbool^);
    1561: DoSheetActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1562: DoSheetDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1563: DoSheetCalculate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch);
    1564: DoSheetChange(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelRange);
    1567: DoWorkbookOpen(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1568: DoWorkbookActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1569: DoWorkbookDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1570: DoWorkbookBeforeClose(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    1571: DoWorkbookBeforeSave(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, DPs.rgvarg^ [PDispIDs^[1]].vbool,
      DPs.rgvarg^ [PDispIDs^[2]].pbool^);
    1572: DoWorkbookBeforePrint(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, DPs.rgvarg^ [PDispIDs^[1]].pbool^);
    1573: DoWorkbookNewSheet(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as IDispatch);
    1574: DoWorkbookAddinInstall(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1575: DoWorkbookAddinUninstall(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook);
    1554: DoWindowResize(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelXP.Window);
    1556: DoWindowActivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelXP.Window);
    1557: DoWindowDeactivate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as ExcelWorkbook, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as ExcelXP.Window);
    1854: DoSheetFollowHyperlink(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as Hyperlink);
    2157: DoSheetPivotTableUpdate(IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval)
      as IDispatch, IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval)
      as PivotTable);
    2160: DoWorkbookPivotTableCloseConnection(
      IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval) as ExcelWorkbook,
      IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval) as PivotTable);
    2161: DoWorkbookPivotTableOpenConnection(
      IUnknown(DPs.rgvarg^[PDispIDs^[0]].unkval) as ExcelWorkbook,
      IUnknown(DPs.rgvarg^[PDispIDs^[1]].unkval) as PivotTable);
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

procedure TExcelAppEventSink.DoNewWorkbook(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppNewWorkbook) then
    FOnExcelAppNewWorkbook(Wb)
end;

procedure TExcelAppEventSink.DoSheetSelectionChange(const Sh: IDispatch;
  const Target: ExcelRange);
begin
  if Assigned(FOnExcelAppSheetSelectionChange) then
    FOnExcelAppSheetSelectionChange(Sh, Target);
end;

procedure TExcelAppEventSink.DoSheetBeforeDoubleClick(const Sh: IDispatch;
  const Target: ExcelRange; var Cancel: WordBool);
begin
  if Assigned(FOnExcelAppSheetBeforeDoubleClick) then
    FOnExcelAppSheetBeforeDoubleClick(Sh, Target, Cancel);
end;

procedure TExcelAppEventSink.DoSheetBeforeRightClick(const Sh: IDispatch;
  const Target: ExcelRange; var Cancel: WordBool);
begin
  if Assigned(FOnExcelAppSheetBeforeRightClick) then
    FOnExcelAppSheetBeforeRightClick(Sh, Target, Cancel);
end;

procedure TExcelAppEventSink.DoSheetActivate(const Sh: IDispatch);
begin
  if Assigned(FOnExcelAppSheetActivate) then
    FOnExcelAppSheetActivate(Sh);
end;

procedure TExcelAppEventSink.DoSheetDeactivate(const Sh: IDispatch);
begin
  if Assigned(FOnExcelAppSheetDeactivate) then
    FOnExcelAppSheetDeactivate(Sh);
end;

procedure TExcelAppEventSink.DoSheetCalculate(const Sh: IDispatch);
begin
  if Assigned(FOnExcelAppSheetCalculate) then
    FOnExcelAppSheetCalculate(Sh);
end;

procedure TExcelAppEventSink.DoSheetChange(const Sh: IDispatch;
  const Target: ExcelRange);
begin
  if Assigned(FOnExcelAppSheetChange) then
    FOnExcelAppSheetChange(Sh, Target);
end;

procedure TExcelAppEventSink.DoWorkbookOpen(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppWorkbookOpen) then
    FOnExcelAppWorkbookOpen(Wb);
end;

procedure TExcelAppEventSink.DoWorkbookActivate(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppWorkbookActivate) then
    FOnExcelAppWorkbookActivate(Wb);
end;

procedure TExcelAppEventSink.DoWorkbookDeactivate(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppWorkbookDeactivate) then
    FOnExcelAppWorkbookDeactivate(Wb);
end;

procedure TExcelAppEventSink.DoWorkbookBeforeClose(const Wb: ExcelWorkbook;
  var Cancel: WordBool);
begin
  if Assigned(FOnExcelAppWorkbookBeforeClose) then
    FOnExcelAppWorkbookBeforeClose(Wb, Cancel);
end;

procedure TExcelAppEventSink.DoWorkbookBeforeSave(const Wb: ExcelWorkbook;
  SaveAsUI: WordBool; var Cancel: WordBool);
begin
  if Assigned(FOnExcelAppWorkbookBeforeSave) then
    FOnExcelAppWorkbookBeforeSave(Wb, SaveAsUI, Cancel);
end;

procedure TExcelAppEventSink.DoWorkbookBeforePrint(const Wb: ExcelWorkbook;
  var Cancel: WordBool);
begin
  if Assigned(FOnExcelAppWorkbookBeforePrint) then
    FOnExcelAppWorkbookBeforePrint(Wb, Cancel);
end;

procedure TExcelAppEventSink.DoWorkbookNewSheet(const Wb: ExcelWorkbook;
  const Sh: IDispatch);
begin
  if Assigned(FOnExcelAppWorkbookNewSheet) then
    FOnExcelAppWorkbookNewSheet(Wb, Sh);
end;

procedure TExcelAppEventSink.DoWorkbookAddinInstall(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppWorkbookAddinInstall) then
    FOnExcelAppWorkbookAddinInstall(Wb);
end;

procedure TExcelAppEventSink.DoWorkbookAddinUninstall(const Wb: ExcelWorkbook);
begin
  if Assigned(FOnExcelAppWorkbookAddinUninstall) then
    FOnExcelAppWorkbookAddinUninstall(Wb);
end;

procedure TExcelAppEventSink.DoWindowResize(const Wb: ExcelWorkbook;
  const Wn: ExcelXP.Window);
begin
  if Assigned(FOnExcelAppWindowResize) then
    FOnExcelAppWindowResize(Wb, Wn);
end;

procedure TExcelAppEventSink.DoWindowActivate(const Wb: ExcelWorkbook;
  const Wn: ExcelXP.Window);
begin
  if Assigned(FOnExcelAppWindowActivate) then
    FOnExcelAppWindowActivate(Wb, Wn);
end;

procedure TExcelAppEventSink.DoWindowDeactivate(const Wb: ExcelWorkbook;
  const Wn: ExcelXP.Window);
begin
  if Assigned(FOnExcelAppWindowDeactivate) then
    FOnExcelAppWindowDeactivate(Wb, Wn);
end;

procedure TExcelAppEventSink.DoSheetFollowHyperlink(const Sh: IDispatch;
  const Target: Hyperlink);
begin
  if Assigned(FOnExcelAppSheetFollowHyperlink) then
    FOnExcelAppSheetFollowHyperlink(Sh, Target);
end;

procedure TExcelAppEventSink.DoSheetPivotTableUpdate(const Sh: IDispatch;
  const Target: PivotTable);
begin
  if Assigned(FOnExcelAppSheetPivotTableUpdate) then
    FOnExcelAppSheetPivotTableUpdate(Sh, Target);
end;

procedure TExcelAppEventSink.DoWorkbookPivotTableCloseConnection(
  const Wb: ExcelWorkbook; const Target: PivotTable);
begin
  if Assigned(FOnExcelAppWorkbookPivotTableCloseConnection) then
    FOnExcelAppWorkbookPivotTableCloseConnection(Wb, Target);
end;

procedure TExcelAppEventSink.DoWorkbookPivotTableOpenConnection(
  const Wb: ExcelWorkbook; const Target: PivotTable);
begin
  if Assigned(FOnExcelAppWorkbookPivotTableOpenConnection) then
    FOnExcelAppWorkbookPivotTableOpenConnection(Wb, Target);
end;



end.
