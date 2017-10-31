unit uCopyForm;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, ExcelXP, uFMAddinExcelUtils, uFMAddinGeneralUtils,
  uFMExcelAddInConst, uSheetObjectModel, uSheetHistory, uExcelUtils,
  uGlobalPlaningConst, uOfficeUtils, uSheetCollector;

const
  liNewBook = '(новая книга)';
  liMoveToEnd = '(скопировать в конец)';

  ermIncorrectSheetName = 'Введено неправильное имя копии листа.' + #13 +
    'Возможно имя содержит более 31 символа, или один из запрещенных знаков: / \ * ? : [ ] '' ';

  ermIncompatibleSheetType = 'Копирование невозможно, так как вид выбранной книги - "отчет", а копируемый лист содержит показатели для записи.';

type
  TfrmCopyForm = class(TForm)
    Panel1: TPanel;
    Label2: TLabel;
    Label3: TLabel;
    Label1: TLabel;
    cbBookList: TComboBox;
    IsWithoutOrderingInform: TCheckBox;
    lbSheetList: TListBox;
    eNewSheetName: TEdit;
    Panel2: TPanel;
    btAply: TButton;
    btExit: TButton;
    Bevel1: TBevel;
    procedure cbBookListChange(Sender: TObject);
    procedure btExitClick(Sender: TObject);
    procedure btAplyClick(Sender: TObject);
  private
    FExcelApplication: ExcelApplication;
    FForCopySheet: ExcelWorksheet;
    FLCID: integer;
    FMetaData: TSheetCollector;
    FIsNewBook: boolean;

    {возвращает выбранную нами книгу}
    function GetSelectedBook: ExcelWorkbook;
    {возвращает выбранный нами лист}
    function GetSelectedSheet: ExcelWorksheet;
    function CheckSheetName(NameSheet: string): boolean;
    function CheckSheetType: boolean;

    property SelectedBook: ExcelWorkbook read GetSelectedBook;

    procedure FillBookList;
    procedure FillSheetList;

    {Действия после копирования}
    procedure AfterCopy(SheetReal, SheetCopy: ExcelWorksheet);
    procedure SetStateScreenUpdating(State: boolean);
    function CopyInNewBook: boolean;
    function CopyInExistBook: boolean;

    procedure Clear;
    procedure ClearOfOrderingInformSheet(Sheet: ExcelWorksheet);

   // procedure AddEventInSheetHistory(ForCopySheet, SheetCopy: ExcelWorksheet);
  public
    procedure ShowCopyForm(ExcelAppl: ExcelApplication; MetaData: TSheetCollector);
    property IsNewBook: boolean read FIsNewBook;
  end;

procedure CopySheet(ExcelAppl: ExcelApplication; MetaData: TSheetCollector; out IsNewBook: boolean);

implementation

{$R *.DFM}

procedure CopySheet(ExcelAppl: ExcelApplication; MetaData: TSheetCollector; out IsNewBook: boolean);
var
  frmCopyForm: TfrmCopyForm;
begin
  frmCopyForm := TfrmCopyForm.Create(nil);
  try
    frmCopyForm.ShowCopyForm(ExcelAppl, MetaData);
    IsNewBook := frmCopyForm.IsNewBook;
  finally
    FreeAndNil(frmCopyForm);
  end;
end;


{ TfrmCopyForm }

procedure TfrmCopyForm.FillBookList;
var
  i, ActivBookIndex: integer;
begin
  cbBookList.Clear;
  if not Assigned(FExcelApplication) then
   exit;
  cbBookList.Items.Add(liNewBook);
  ActivBookIndex := 0;;

  for i := 1 to FExcelApplication.Workbooks.Count do
  begin
   cbBookList.Items.Add(FExcelApplication.Workbooks.Item[i].Name);
   if (FExcelApplication.Workbooks.Item[i].Name = FExcelApplication.ActiveWorkbook.Name) then
     ActivBookIndex := i;
  end;
  cbBookList.ItemIndex := ActivBookIndex;

  FillSheetList;  
end;

procedure TfrmCopyForm.FillSheetList;
var
  i: integer;
  ExcelSheet: ExcelWorkSheet;
begin
  lbSheetList.Clear;
  if Assigned(SelectedBook) then
  begin
    for i := 1 to SelectedBook.Sheets.Count do
    begin
      ExcelSheet := GetWorkSheet(SelectedBook.Sheets[i]);
      if Assigned(ExcelSheet) then
        lbSheetList.Items.Add(ExcelSheet.Name);
    end;
    lbSheetList.Items.Add(liMoveToEnd);
    lbSheetList.ItemIndex := 0;
  end;
end;

function TfrmCopyForm.GetSelectedBook: ExcelWorkbook;
begin
  if cbBookList.ItemIndex > 0 then
    result := FExcelApplication.Workbooks.Item[cbBookList.ItemIndex]
  else
    result := nil;
end;

function TfrmCopyForm.GetSelectedSheet: ExcelWorksheet;
begin
  {Здесь смотрим что выбрали, если последний элемент в списке, значит это наш
  пункт свидетельствудщий о копирование листа в конец книги}
  if Assigned(SelectedBook) and (lbSheetList.ItemIndex > -1)
    and (lbSheetList.ItemIndex <> lbSheetList.Items.Count -1) then
    result := GetWorkSheet(SelectedBook.Sheets[lbSheetList.ItemIndex + 1])
  else
    result := nil;
end;

procedure TfrmCopyForm.ShowCopyForm(ExcelAppl: ExcelApplication; MetaData: TSheetCollector);
begin
  if not Assigned(ExcelAppl) then
    exit;
  FExcelApplication := ExcelAppl;
  FMetadata := Metadata;
  FForCopySheet := GetWorkSheet(FExcelApplication.ActiveSheet);
  if not Assigned(FForCopySheet) then
    exit;
  eNewSheetName.Text := FForCopySheet.Name;
  FLCID := GetUserDefaultLCID;
  FIsNewBook := false;

  FillBookList;

  ShowModal;
end;

procedure TfrmCopyForm.cbBookListChange(Sender: TObject);
begin
  FillSheetList;
end;

procedure TfrmCopyForm.btExitClick(Sender: TObject);
begin
  Clear;
  Close;
end;

function GetAvailableSheetName(ExcelBook: ExcelWorkbook; SheetName: string): string;
var
  Index, MaxLen: integer;
  ExcelSheet: ExcelWorksheet;
begin
  result := Trim(SheetName);
  if not Assigned(ExcelBook) then
    exit;
  Index := 1;
  try
    ExcelSheet := GetWorkSheet(ExcelBook.Sheets.Item[SheetName]);
    while Assigned(ExcelSheet) do
    begin
      inc(Index);
      MaxLen := 31 - Length(IntToStr(Index)) - 3; 
      if Length(SheetName) > MaxLen then
        SheetName := Copy(SheetName, 1, MaxLen);
      result := Format(SheetName + ' (%d)', [Index]);
      ExcelSheet := GetWorkSheet(ExcelBook.Sheets.Item[result]);
    end;
  except
  end;
end;

procedure TfrmCopyForm.AfterCopy(SheetReal, SheetCopy: ExcelWorksheet);
var
  i: integer;
  BookCopy: ExcelWorkbook;
  SheetType: string;
  CPErrorList: TStringList;
  HForm: TSheetHistory;
  CopyOK: boolean;
begin
  {если присваемое имя листа уже имеется у другого листа этой же книги возникает
  ошибка, ничего страшного нет если затраено, у листа остается прежнее имя,
  далее все идет корректно}
  BookCopy := (SheetCopy.Parent as ExcelWorkbook);


  SheetCopy.Name := 'unisheetname';
  SheetCopy.Name := GetAvailableSheetName(BookCopy, eNewSheetName.Text);

//  AddEventInSheetHistory(SheetReal, SheetCopy);
  CopyOK := true;
  if IsWithoutOrderingInform.Checked then
    ClearOfOrderingInformSheet(SheetCopy)
  else
  begin
    CPErrorList := TStringList.Create;
    for i := 1 to SheetReal.CustomProperties.Count do
    try
      SheetCopy.CustomProperties.Add(SheetReal.CustomProperties[i].Name,
        SheetReal.CustomProperties[i].Value);
    except
      on e: exception do CPErrorList.Add('Ошибка при копировании СР №'  + IntToStr(i));
    end;
    CopyOK := CPErrorList.Count = 0;
    if not CopyOK then
    begin
      HForm := TSheetHistory.Create(nil);
      HForm.AddEvent(SheetCopy, evtSheetCopy, CPErrorList.CommaText, false);
      FreeAndNil(HForm);
      FreeStringList(CPErrorList);
    end;
    if IsPlaningSheet(SheetCopy) then
      SetSheetProtection(SheetCopy, true);
    try
      SheetType := GetWBCustomPropertyValue(BookCopy.CustomDocumentProperties,
        pspSheetType);
      {Если тип листа не установлен ставим по умолчанию "Расчетный лист"}
      if (SheetType = '') or (SheetType = '-1') then
        SetWBCustomPropertyValue(BookCopy.CustomDocumentProperties, pspSheetType,
          '0');
    except
    end;
  end;
  if IsPlaningSheet(SheetReal) then
    SetSheetProtection(SheetReal, true);
  if not CopyOK then
    ShowWarning('Копирование служебной информации завершено с ошибками, см. "историю листа"');
end;

function TfrmCopyForm.CopyInNewBook: boolean;
var
 NewBook: ExcelWorkbook;
 tempSheet: ExcelWorksheet;
begin
  result := true;
  SetStateScreenUpdating(false);
  try
    try
      SetSheetProtection(FForCopySheet, false);
      NewBook := FExcelApplication.Workbooks.Add(EmptyParam, FLCID);
      tempSheet := GetWorkSheet(NewBook.Sheets[1]);

      tempSheet.Name := 'buyaka';
      FForCopySheet.Copy(tempSheet, EmptyParam, FLCID);
      tempSheet := GetWorkSheet(NewBook.Sheets[1]);

      SetStateScreenUpdating(false);
      while NewBook.Sheets.Count > 1 do
        GetWorkSheet(NewBook.Sheets[2]).Delete(FLCID);
      SetStateScreenUpdating(true);

      {попытка обойти "оьбрезание экселем содержимого ячеек сверх
        255 символов - задача 11923}
      FForCopySheet.Cells.Copy(tempSheet.Cells);
      AfterCopy(FForCopySheet, tempSheet);

      NewBook.Activate(FLCID);
    except
      result := false;
    end;
  finally
    SetStateScreenUpdating(true);
  end;
end;

function TfrmCopyForm.CopyInExistBook: boolean;
var
 ForInsertBook: ExcelWorkbook;
 tempSheet: ExcelWorksheet;
 tempSheetIndex: integer;
begin
  result := true;
  SetStateScreenUpdating(false);
  try
    try
      SetSheetProtection(FForCopySheet, false);
      ForInsertBook := GetSelectedBook;
      tempSheet := GetSelectedSheet;

      if Assigned(tempSheet) then
      begin
        tempSheetIndex := tempSheet.Index[FLCID];
        FForCopySheet.Copy(tempSheet, EmptyParam, FLCID);
        tempSheet := GetWorkSheet(ForInsertBook.Sheets[tempSheetIndex]);
      end
      else
      {если лист перед которым нужно вставить копию nil, значит копируем в конец
      книги}
      begin
        tempSheet := (ForInsertBook.Sheets[ForInsertBook.Sheets.Count] as
          ExcelWorksheet);
        tempSheetIndex := tempSheet.Index[FLCID];
        FForCopySheet.Copy(EmptyParam, tempSheet, FLCID);
        tempSheet := GetWorkSheet(ForInsertBook.Sheets[tempSheetIndex + 1]);
      end;

      {попытка обойти "оьбрезание экселем содержимого ячеек сверх
        255 символов - задача 11923.
        К сожалению, по непонятным причинам данная операция может вызвать исключение}
      try
        FForCopySheet.Cells.Copy(tempSheet.Cells);
      except
      end;
      AfterCopy(FForCopySheet, tempSheet);

      ForInsertBook.Activate(FLCID);
    except
      result := false;
    end;
  finally
    SetStateScreenUpdating(true);
  end;
end;

procedure TfrmCopyForm.ClearOfOrderingInformSheet(Sheet: ExcelWorksheet);
var
  tempRange: ExcelRange;
  i: integer;
  AName: string;
begin
  if not Assigned(Sheet) then
    exit;
  try
    SetStateScreenUpdating(false);
    {очишаем таблицу от комментариев}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntTable));
    if Assigned(tempRange) then
    begin
      tempRange.ClearComments;
      tempRange.Interior.ColorIndex := 0;
      tempRange.Font.ColorIndex := 1;
    end;
    {фильтры}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntFilterArea));
    if Assigned(tempRange) then
      {!!!при присваивание диапазону фильтров конкретного стиля происходит
      AcessVioletion из-за не возможности наложения его на объединенные ячейки,
      поэтому просто подменяем фон, в принципе делаем что и требовалось от стиля}
      tempRange.Interior.ColorIndex := 0;
    {заголовки столбцов}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntColumnTitles));
    if Assigned(tempRange) then
      tempRange.Style := snFieldTitlePrint;
    {столбцы}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntColumnsAndMPropsArea));
    if Assigned(tempRange) then
      tempRange.Style := snFieldPositionPrint;
    {строки}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntRows));
    if Assigned(tempRange) then
      tempRange.Style := snFieldPositionPrint;
    {заголовки строк}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntRowTitles));
    if Assigned(tempRange) then
      tempRange.Style := snFieldTitlePrint;
    {заголовки показателей}
    tempRange := GetRangeByName(Sheet, BuildExcelName(sntTotalTitles));
    if Assigned(tempRange) then
      tempRange.Style := snTotalTitlePrint;
    {отдельные ячейки}
    for i := 1 to Sheet.Names.Count do
    begin
      AName := Sheet.Names.Item(i, EmptyParam, EmptyParam).Name_;
      if (Pos(BuildExcelName(sntSingleCellMeasure), AName) = 0) and
        (Pos(BuildExcelName(sntSingleCellResult), AName) = 0) and
        (Pos(BuildExcelName(sntSingleCellConst), AName) = 0) then
        continue;
      tempRange := GetRangeByName(Sheet, AName);
      if Assigned(tempRange) then
      begin
        tempRange.Style := snSingleCellsPrint;
        tempRange.ClearComments;
      end;
    end;
    {очистим лист от наших имен}
    DeleteExcelNames(Sheet);
  finally
    SetStateScreenUpdating(true);
  end;
end;

function TfrmCopyForm.CheckSheetName(NameSheet: string): boolean;
const
  ProhibitedChar = '[]/\?*:''';
var
  i, ProhibitedCharCount: integer;
begin
  result := false;
  if (NameSheet = '') or (Length(NameSheet) > 31) then
    exit;
  ProhibitedCharCount := Length(ProhibitedChar);
  for i := 1 to ProhibitedCharCount do
    if Pos(ProhibitedChar[i], NameSheet) > 0 then
      exit;
  result := true;
end;

function TfrmCopyForm.CheckSheetType: boolean;
begin
  result := true;
  if not Assigned(SelectedBook) then
    exit;
  if (GetWBCustomPropertyValue(SelectedBook.CustomDocumentProperties, pspSheetType) <> '2') then
    exit;

  if FMetaData.Load(FForCopySheet, nil, [lmCollections]) then
  try
    if FMetaData.Totals.CheckByType([wtResult]) or
     FMetaData.SingleCells.CheckByType([wtResult]) then
      result:= false;
  finally
    FMetaData.Clear;
  end;
end;

procedure TfrmCopyForm.btAplyClick(Sender: TObject);
begin
  eNewSheetName.Text := Trim(eNewSheetName.Text);
  if not CheckSheetName(eNewSheetName.Text) then
  begin
    ShowError(ermIncorrectSheetName);
    exit;
  end;
  if not IsWithoutOrderingInform.Checked then
    // нельзя копировать лист с показателями для записи на нем в книгу "отчет"
    if not CheckSheetType then
    begin
      ShowError(ermIncompatibleSheetType);
      exit;
    end;
  try
    if Assigned(SelectedBook) then
      CopyInExistBook
    else
      FIsNewBook := CopyInNewBook;
  finally
    btExit.Click;
  end;  
end;

procedure TfrmCopyForm.Clear;
begin
  eNewSheetName.Text := '';
  FExcelApplication := nil;
  FForCopySheet := nil;
end;

procedure TfrmCopyForm.SetStateScreenUpdating(State: boolean);
begin
  FExcelApplication.DisplayAlerts[FLCID] := State;
  FExcelApplication.ScreenUpdating[FLCID] := State;
  FExcelApplication.Set_Interactive(FLCID, State);
end;
(*
procedure TfrmCopyForm.AddEventInSheetHistory(ForCopySheet,
  SheetCopy: ExcelWorksheet);
var
  Events: TStringList;
  SheetHistory: TSheetHistory;
begin
  if not(Assigned(ForCopySheet) or Assigned(SheetCopy)) then
    exit;
  SheetHistory := TSheetHistory.Create(nil);
  try
    Events := TStringList.Create;
    Events.Add('Лист является копией листа "' + ForCopySheet.Name + '" из книги "'
      + ForCopySheet.Application.ActiveWorkbook.Name + '"');
    SheetHistory.AddEvent(SheetCopy, evtSheetCopy, Events, true);

    Events := TStringList.Create;
    Events.Add('С листа была сделана копия в книгу "'
      + SheetCopy.Application.ActiveWorkbook.Name + '", имя копии листа "'
      + SheetCopy.Name + '"');
    SheetHistory.AddEvent(ForCopySheet, evtSheetCopy, Events, true);
  finally
    FreeAndNil(SheetHistory);
  end;
end;
*)

end.
