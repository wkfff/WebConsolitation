unit uQueryRange;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExcelXP, brs_GeneralFunctions, uFMAddinGeneralUtils,
  uFMAddinExcelUtils, Planing_TLB, OfficeXP, uExcelUtils, ExtCtrls,
  uGlobalPlaningConst;

type

  TfrmQueryRange = class(TForm)
    Label1: TLabel;
    edBook: TEdit;
    btnOpen: TButton;
    cmbSheet: TComboBox;
    Label2: TLabel;
    Label3: TLabel;
    btnCancel: TButton;
    btnOK: TButton;
    btnSelect: TButton;
    cmbRange: TComboBox;
    Bevel1: TBevel;
    dlgOpen: TOpenDialog;
    procedure btnOpenClick(Sender: TObject);
    procedure btnSelectClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure cmbSheetClick(Sender: TObject);
    procedure cmbSheetDropDown(Sender: TObject);
    procedure cmbRangeClick(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure btnCancelClick(Sender: TObject);
  private
    App: ExcelApplication;
    Wb: ExcelWorkbook;
    AddinExtension: IFMPlanningExtension;
    AddinAncillary: IFMPlanningAncillary;

    DroppedDown: boolean;

    procedure LoadWorkbook;
    procedure LoadNames;
    function GetAddinInterfaces: boolean;
    function GetHeadingHeight: integer;
    function CheckSheetHeading(out HeadingAddress, CopyAddress: string): boolean;
  public
  end;


  function QueryRange(out BookPath, RangeAddress: string;
    out HeadingHeight: integer): boolean;
  function EditInsertedWorkbook(WorkbookPath, SheetName, InsertedAddress: string;
    out BookPath, RangeAddress: string; out HeadingHeight: integer): boolean;

{как вариант:

  function QueryRange(out BookPath, RangeAddress, RangeName: string): boolean;
}
implementation

{$R *.DFM}
var
  frmQueryRange: TfrmQueryRange;
  IsApply: boolean;

function QueryRange(out BookPath, RangeAddress: string;
  out HeadingHeight: integer): boolean;
begin
  frmQueryRange := TfrmQueryRange.Create(nil);
  frmQueryRange.btnSelect.Enabled := frmQueryRange.cmbSheet.ItemIndex > -1;
  try
    try
      frmQueryRange.btnOK.Enabled := false;
      IsApply := false;
      frmQueryRange.ShowModal;
      result := IsApply;
      if result then
      begin
        BookPath := frmQueryRange.edBook.Text;
        RangeAddress := frmQueryRange.cmbSheet.Text + '!' +
          frmQueryRange.cmbRange.Text;
        HeadingHeight := frmQueryRange.GetHeadingHeight;
      end;
    except
      result := false;
    end;
  finally
    frmQueryRange.Free;
  end;
end;

{Инициализирует параметры формы при редактировании листа}
procedure InitFormParam(var Form: TfrmQueryRange; WorkbookPath, SheetName,
  InsertedAddress: string);
var
  Index: integer;
begin
  if not Assigned(Form) then
    exit;
  Form.edBook.Text := WorkbookPath;
  Form.LoadWorkbook;
  {Вычислим позицию редактируемого листа в списке имеющихся}
  Index := Form.cmbSheet.Items.IndexOf(SheetName);
  Form.cmbSheet.ItemIndex := Index;
  Form.btnSelect.Enabled := (Form.cmbSheet.ItemIndex > -1);
  Form.LoadNames;
  {Добавляем в список редактируемый диапазон}
  Index := Form.cmbRange.Items.IndexOf(InsertedAddress);
  if (Index > -1) then
    Form.cmbRange.ItemIndex := Index
  else
  begin
    Form.cmbRange.Items.Add(InsertedAddress);
    Form.cmbRange.ItemIndex := Form.cmbRange.Items.Count - 1;
  end;
  Form.btnOK.Enabled := (Form.cmbSheet.ItemIndex > -1) and
    (InsertedAddress <> '');
end;

function EditInsertedWorkbook(WorkbookPath, SheetName, InsertedAddress: string;
  out BookPath, RangeAddress: string; out HeadingHeight: integer): boolean;
begin
  frmQueryRange := TfrmQueryRange.Create(nil);
  frmQueryRange.btnSelect.Enabled := (frmQueryRange.cmbSheet.ItemIndex > -1);
  try
    try
      InitFormParam(frmQueryRange, WorkbookPath, SheetName, InsertedAddress);
      IsApply := false;
      frmQueryRange.ShowModal;
      result := IsApply;
      if result then
      begin
        BookPath := frmQueryRange.edBook.Text;
        RangeAddress := frmQueryRange.cmbSheet.Text + '!' +
          frmQueryRange.cmbRange.Text;
        HeadingHeight := frmQueryRange.GetHeadingHeight;
      end;
    except
      result := false;
    end;
  finally
    frmQueryRange.Free;
  end;
end;

procedure TfrmQueryRange.btnOpenClick(Sender: TObject);
begin
  if not dlgOpen.Execute then
    exit;
  edBook.Text := dlgOpen.FileName;
  Application.ProcessMessages;
  LoadWorkbook;
  if cmbSheet.Items.Count > 0 then
    cmbSheet.ItemIndex := 0;
  LoadNames;
  btnSelect.Enabled := cmbSheet.ItemIndex > -1;
end;

procedure TfrmQueryRange.LoadNames;
var
  i: integer;
  Ws: ExcelWorkSheet;
  eName: ExcelXP.Name;
begin
  if cmbSheet.ItemIndex < 0 then
    exit;
  cmbRange.Items.Clear;
  Ws := Wb.Sheets[cmbSheet.ItemIndex + 1] as ExcelWorkSheet;
  for i := 1 to Ws.Names.Count do
  begin
    eName := Ws.Names.Item(i, 0, 0);
    if IsUserNameOurs(eName.Name_) then
      cmbRange.Items.Add(GetShortSheetName(eName.Name_));
  end;
end;

procedure TfrmQueryRange.LoadWorkbook;
var
  i: integer;
  InterfacesFound: boolean;
  OldCursor: TCursor;
  ExcelSheet: ExcelWorksheet;
begin
  OldCursor := Screen.Cursor;
  try
    Screen.Cursor := crHourglass;
    {создадим свой экземпляр экселя}
    if not Assigned(App) then
    try
      if not GetExcel(App) then
        raise Exception.Create('');
    except
      ShowError('Не удалось создать объект Microsoft Excel');
      exit;
    end;
    {запросим интересующие нас интерфейсы нашего плагина}
    try
      InterfacesFound := GetAddinInterfaces;
      if not InterfacesFound then
        raise Exception.Create('Не найден плагин');
    except
      ShowError('Не найден плагин');
      exit;
    end;
    {загрузим нужную книгу в тихом режиме}
    AddinExtension.IsSilentMode := true;
    try
      Wb := App.Workbooks.Open(edBook.Text, EmptyParam, true, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam, EmptyParam, EmptyParam, false, EmptyParam, EmptyParam,
        EmptyParam, EmptyParam,  EmptyParam, GetUserDefaultLCID);
    except
      ShowError('Не удалось открыть книгу Microsoft Excel');
      exit;
    end;
    cmbSheet.Clear;
    for i := 1 to Wb.Sheets.Count do
    begin
      ExcelSheet := GetWorkSheet(Wb.Sheets[i]);
      if Assigned(ExcelSheet) then
        cmbSheet.Items.Add(ExcelSheet.Name);
    end;
  finally
    Screen.Cursor := OldCursor;
  end;
end;

procedure TfrmQueryRange.btnSelectClick(Sender: TObject);
var
  Range: ExcelRange;
  RangeDisp: IDispatch;
  Address: WideString;
  Index: integer;
begin
  try
    (Wb.Sheets.Item[cmbSheet.text] as ExcelWorkSheet).Activate(GetUserDefaultLCID);
  except
  end;
  try
    RangeDisp := AddinAncillary.QueryRange('Укажите диапазон',
      'Укажите диапазон, который требуется скопировать в документ Word', false);
  except
    on E:Exception do
    begin
      ShowError(E.Message);
      Close;
      exit;
    end;
  end;
  if not Assigned(RangeDisp) then
    exit;
  try
    Range := RangeDisp as ExcelRange;
    {$WARNINGS OFF}
    Address := Range.AddressLocal[true, true, xlR1C1, false, varNull];
    {$WARNINGS ON}
    Index := cmbRange.Items.IndexOf(Address);
    if Index > -1 then
      cmbRange.ItemIndex := Index
    else
    begin
      cmbRange.Items.Add(Address);
      cmbRange.ItemIndex := cmbRange.Items.Count - 1;
      btnOK.Enabled := true;
    end;
  except
  end;
end;

procedure TfrmQueryRange.FormDestroy(Sender: TObject);
begin
  Application.ProcessMessages;
  try
    AddinAncillary := nil;
    AddinExtension := nil;
    if Assigned(App) then
      App.Quit;
  except
  end;
end;

procedure TfrmQueryRange.cmbSheetClick(Sender: TObject);
begin
  if DroppedDown then
    LoadNames;
  btnSelect.Enabled := (cmbSheet.ItemIndex > -1);
  DroppedDown := false;
end;

procedure TfrmQueryRange.cmbSheetDropDown(Sender: TObject);
begin
  DroppedDown := true;
end;

function TfrmQueryRange.GetAddinInterfaces: boolean;
var
  i: integer;
  Index: OleVariant;
  CurAddin: ComAddin;
begin
  result := true;
  for i := 1 to App.COMAddIns.Count do
  begin
    Index := i;
    CurAddin := App.COMAddIns.Item(Index);
    if AnsiUpperCase(CurAddin.Get_ProgId) = AnsiUpperCase('FMExcelAddin.DTExtensibility2') then
    try
      AddinAncillary := CurAddin.Object_ as IFMPlanningAncillary;
      AddinExtension := CurAddin.Object_ as IFMPlanningExtension;
      result := Assigned(AddinAncillary) and Assigned(AddinExtension);
      exit;
    except
      result := false;
    end;
  end;
end;

procedure TfrmQueryRange.cmbRangeClick(Sender: TObject);
begin
  btnOk.Enabled := (cmbRange.ItemIndex > -1);
end;

function TfrmQueryRange.GetHeadingHeight: integer;
var
  TableHeading: ExcelRange;
  HeadingAddress, CopyAddress: string;
  ExcelSheet: ExcelWorkSheet;
begin
  result := 0;
  if not Assigned(Wb) then
    exit;
  if not CheckSheetHeading(HeadingAddress, CopyAddress) then
    exit;
  ExcelSheet := (WB.Sheets.Item[cmbSheet.Text] as ExcelWorkSheet);
  TableHeading := GetRangeByName(ExcelSheet, BuildUserExcelName(sntTableHeading));
  if not Assigned(TableHeading) then
    exit;
  result := TableHeading.Rows.Count;
end;

function TfrmQueryRange.CheckSheetHeading(out HeadingAddress, CopyAddress: string): boolean;
var
  CopyRange, HeadingRange: ExcelRange;
  ExcelSheet: ExcelWorksheet;
begin
  HeadingAddress := '';
  CopyAddress := '';
  result := true;
  ExcelSheet := GetWorkSheet(Wb.Sheets.Item[cmbSheet.Text]);
  {если копируймый диапазон задан именем, достаем его}
  CopyRange := GetRangeByName(ExcelSheet, cmbRange.Text);
  if not Assigned(CopyRange) then
    {значит диапазон задан адресом}
    CopyRange := GetRange(ExcelSheet, cmbRange.Text);
  if not Assigned(CopyRange) then
    exit;
  HeadingRange := GetRangeByName(ExcelSheet, BuildUserExcelName(sntTableHeading));
  if not Assigned(HeadingRange) then
    exit;
  CopyAddress := GetAddressLocal(CopyRange);
  HeadingAddress := GetAddressLocal(HeadingRange);
  result := IsNestedRanges_(HeadingRange, CopyRange) and (HeadingRange.Row = CopyRange.Row);
end;

procedure TfrmQueryRange.btnOKClick(Sender: TObject);
const
  Question = 'Область заголовка листа (%s) не принадлежит выбранному диапазону (%s)' + #10#13
    + 'или расположена не в его начале.' + #10#13
    + 'Это значит, что она не будет считаться заголовком во встраиваемой таблице. Продолжить?';
var
  HeadingAddress, CopyAddress: string;
begin
  if CheckSheetHeading(HeadingAddress, CopyAddress) or
    ShowQuestion(Format(Question, [HeadingAddress, CopyAddress])) then
  begin
    IsApply := true;
    close;
  end;
end;

procedure TfrmQueryRange.btnCancelClick(Sender: TObject);
begin
  IsApply := false;
  Close;
end;

end.




