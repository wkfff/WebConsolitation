unit uSingleCellManager;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, Grids, ComCtrls, ExtCtrls, Buttons, uSheetObjectModel,
  uSingleCellEditor, uFMAddinGeneralUtils, uFMExcelAddInConst, ExcelXP,
  uFMAddinExcelUtils, uXMLUtils, MSXML2_TLB, uExcelUtils, uFindForm,
  uGlobalPlaningConst, uSheetLogic;

type

  TSortMode = (smNone, smName, smAddress);

  TfmSingleCellManager = class(TForm)
    Panel1: TPanel;
    btnNew: TSpeedButton;
    btnEdit: TSpeedButton;
    btnDelete: TSpeedButton;
    btnCopy: TSpeedButton;
    btnJump: TSpeedButton;
    Panel2: TPanel;
    btnCancel: TButton;
    btnOK: TButton;
    Panel3: TPanel;
    Panel4: TPanel;
    Panel5: TPanel;
    lvCells: TListView;
    mDetail: TMemo;
    Splitter1: TSplitter;
    btnMove: TSpeedButton;
    btnFind: TSpeedButton;
    procedure btnEditClick(Sender: TObject);
    procedure lvCellsDblClick(Sender: TObject);
    procedure btnDeleteClick(Sender: TObject);
    procedure btnJumpClick(Sender: TObject);
    procedure btnNewClick(Sender: TObject);
    procedure btnCopyClick(Sender: TObject);
    procedure btnCancelClick(Sender: TObject);
    procedure lvCellsChange(Sender: TObject; Item: TListItem;
      Change: TItemChange);
    procedure btnMoveClick(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure lvCellsCompare(Sender: TObject; Item1, Item2: TListItem;
      Data: Integer; var Compare: Integer);
    procedure lvCellsColumnClick(Sender: TObject; Column: TListColumn);
    procedure btnFindClick(Sender: TObject);
  private
    FLCID: integer;
    FPlaningSheet: TSheetInterface;
    RefreshList:  TStringList;
    {список комментариев для истории листа}
    HistoryList: TStringList;

    SortMode: TSortMode;
    {заполняет список уже настроенных в листе отдельных ячеек}
    procedure Enlist;
    procedure EnableButtons;
    function GetExcelSheet: ExcelWorkSheet;
    function GetCellRange: ExcelRange;
    function GetDeletedCellRange(Address: string): ExcelRange;
    function GetCurrentCell: TSheetSingleCellInterface;
    {при открытии формы пытается выделить в списке отдельных ту, которая
      находится в активной ячейке}
    procedure TryPositioning(Target: ExcelRange);

    property CurrentCell: TSheetSingleCellInterface read
      GetCurrentCell;
    property ExcelSheet: ExcelWorkSheet read GetExcelSheet;
  public
    {список имен добавленных ячеек: составляется на случай выхода из
      сеанса редактирования по "отмене", чтобы можно было удалить эти
      имена из листа}
    AddedCellsList: TStringList;
    {список имен удаленных ячеек: составляется на случай выхода из
      сеанса редактирования по "отмене", чтобы можно было вернуть эти
      имена в лист; строки хранятся в форме ExcelName=Address}
    DeletedCellsList: TStringList;
  end;

  {RL - индексы ячеек, требующих обновления;
    если выходили по "отмене", вернет пустой список;
   HList - комментарии для истории листа;
   AddedCells - имена свежедобавленных ячеек;
  }
function ManageSingleCells(PlaningSheet: TSheetInterface;
    var HList, AddedCells: TStringList; out RL: TStringList): boolean;

procedure ClearAddedCellsNames(ExcelSheet: ExcelWorkSheet;
  AddedCellsList: TStringList);

{можно ли разместить отдельную ячейку в указанном диапазоне}
function IsSingleCellAllowed(ExcelSheet: ExcelWorkSheet;
  Target: ExcelRange; WritablesInfo: TWritablesInfo;
  out MsgText: string; out Editable: boolean): boolean;

implementation

{$R *.DFM}


function ManageSingleCells(PlaningSheet: TSheetInterface;
    var HList, AddedCells: TStringList; out RL: TStringList): boolean;
var
  fmSingleCellManager: TfmSingleCellManager;
  eRange: ExcelRange;
begin
  fmSingleCellManager := TfmSingleCellManager.Create(nil);
  try
    with fmSingleCellManager do
    begin
      SortMode := smNone;
      FPlaningSheet := PlaningSheet;
      FLCID := GetUserDefaultLCID;
      eRange := ExcelSheet.Application.ActiveCell;
      RefreshList := TStringList.Create;
      if not Assigned(HList) then
        HList := TStringList.Create;
      HistoryList := HList;
      if not Assigned(AddedCells) then
        AddedCells := TStringList.Create;
      AddedCellsList := AddedCells;
      if not Assigned(DeletedCellsList) then
        DeletedCellsList := TStringList.Create;
      DeletedCellsList.Clear;
      Enlist;
      TryPositioning(eRange);
      result := ShowModal = mrOk;
      if not result then
        RefreshList.Clear;
      RL := RefreshList;
    end;
  finally
    FreeStringList(fmSingleCellManager.DeletedCellsList);
    FreeAndNil(fmSingleCellManager);
  end;
end;

{ TfmSingleCellManager }

procedure TfmSingleCellManager.Enlist;
var
  i, Index: integer;
  Cell: TSheetSingleCellInterface;
begin
  if Assigned(lvCells.Selected) then
    Index := lvCells.Selected.Index
  else
    Index := -1;

  lvCells.Items.BeginUpdate;
  lvCells.OnChange := nil;
  lvCells.Items.Clear;
  mDetail.Clear;
  lvCells.OnChange := lvCellsChange;

  for i := 0 to FPlaningSheet.SingleCells.Count - 1 do
  begin
    Cell := FPlaningSheet.SingleCells[i];
    with lvCells.Items.Add do
    begin
      Caption := Cell.Name;
      SubItems.Append(Cell.Address);
      case Cell.TotalType of
        wtResult: SubItems.Append('Результат расчета');
        wtConst: SubItems.Append('Константа');
        else SubItems.Append('Мера из куба');
      end;
      SubItems.Append(Cell.CubeName);
      SubItems.Append(Cell.MeasureName);
      SubItems.Append(Cell.CommaFilters);
      Data := Cell;
    end;
  end;
  if (Index > -1) and (Index < lvCells.Items.Count) then
  begin
    lvCells.Selected := lvCells.Items[Index];
    lvCells.Items[Index].MakeVisible(false);
  end
  else
    if lvCells.Items.Count > 0 then
      lvCells.Selected := lvCells.Items[0];
  lvCells.Items.EndUpdate;
  EnableButtons;
end;

procedure TfmSingleCellManager.EnableButtons;
var
  State: boolean;
begin
  State := Assigned(CurrentCell);
  btnEdit.Enabled := State and (CurrentCell.TotalType <> wtConst) and
    CurrentCell.MayBeEdited;
  btnDelete.Enabled := State and FPlaningSheet.SingleCells.MayBeEdited;
  btnCopy.Enabled := State and FPlaningSheet.SingleCells.MayBeEdited;
  btnMove.Enabled := State and CurrentCell.MayBeEdited;
  btnJump.Enabled := State and CurrentCell.MayBeEdited;
  btnNew.Enabled := FPlaningSheet.SingleCells.MayBeEdited;
end;

procedure TfmSingleCellManager.btnEditClick(Sender: TObject);
var
  CIndex: integer;
begin
  if Assigned(CurrentCell) then
  begin
    if (CurrentCell.TotalType = wtConst) then
      exit;
    CIndex := CurrentCell.GetSelfIndex;
    if EditSingleCell(FPlaningSheet, CIndex, Self.Handle) then
    begin
      RefreshList.Add(IntToStr(CIndex));
      HistoryList.Add(Format('Изменен отдельный показатель "%s", по адресу %s',
        [CurrentCell.Name, CurrentCell.Address]));
    end;
  end;  
  Enlist;
end;

procedure TfmSingleCellManager.lvCellsDblClick(Sender: TObject);
begin
  if Assigned(lvCells.Selected) then
    btnEditClick(Self);
end;

{Если отдельный показатель размещался в области данных других показателей, то
при его удалении или перемещении остается некрасивая дырка, которую мы и
заделываем}
procedure FormatRange(Model: TSheetInterface; CellRange: ExcelRange);
var
  OwnerTotal: TSheetTotalInterface;
  SectionIndex: integer;
begin
  if not(Assigned(Model) and Assigned(CellRange)) then
    exit;
  {Ищем показатель, в котором размещалась отдельная ячейка}
  OwnerTotal := Model.Totals.FindByColumn(CellRange.Column, SectionIndex);
  if Assigned(OwnerTotal) then
  try
    {латаем дыры сделанные перемещением отдельного показателя}
    CellRange.NumberFormat := OwnerTotal.NumberFormat;
    CellRange.Style := OwnerTotal.Styles.Name[esValue];
  except
  end;
end;

procedure TfmSingleCellManager.btnDeleteClick(Sender: TObject);
var
  tmpStr, TotalAlias: string;
  Index, CIndex: integer;
  PlacedInTotals: boolean;
  SingleCell: TSheetSingleCellInterface;
  CellRange: ExcelRange;
begin
  SingleCell := CurrentCell;
  if not Assigned(SingleCell) then
    exit;
  if not SingleCell.MayBeDeleted then
    exit;
  try
    TotalAlias := CurrentCell.Alias;
    PlacedInTotals := CurrentCell.PlacedInTotals;
    CellRange := SingleCell.GetExcelRange;

    ExcelSheet.Application.ScreenUpdating[FLCID] := false;
    (FPlaningSheet as TSheetLogic).StartOperation(Self.Handle,
      pfoSingleResultDeletion);
    //CurrentCell.ClearLinkedTypeFormulasValues;
    CIndex := CurrentCell.GetSelfIndex;
    Index := AddedCellsList.IndexOf(CurrentCell.Address);
    if Index > -1 then
      AddedCellsList.Delete(Index);
    DeletedCellsList.Add(CurrentCell.ExcelName + '=' + CurrentCell.Address);
    CurrentCell.Suicide(smImmediate, tmpStr);
    HistoryList.Add(tmpStr);
    if RefreshList.IndexOf(IntToStr(CIndex)) > -1 then
      RefreshList.Delete(RefreshList.IndexOf(IntToStr(CIndex)));
    if PlacedinTotals then
      FormatRange(FPlaningSheet, CellRange);
    //CurrentCell.MapLinkedTypeFormulasValues;
    Enlist;
  finally
    ExcelSheet.Application.ScreenUpdating[FLCID] := true;
    (FPlaningSheet as TSheetLogic).StopOperation;
  end;
end;

function TfmSingleCellManager.GetExcelSheet: ExcelWorkSheet;
begin
  if Assigned(FPlaningSheet) then
    result := FPlaningSheet.ExcelSheet
  else
    result := nil;
end;

procedure TfmSingleCellManager.btnJumpClick(Sender: TObject);
var
  eRange: ExcelRange;
begin
  if Assigned(CurrentCell) then
  begin
    ERange := GetRangeByName(ExcelSheet, CurrentCell.ExcelName);
    if Assigned(ERange) then
    begin
      ERange.Show;
      ERange.Select;
      btnCancel.Click;
    end
  end;
end;

procedure TfmSingleCellManager.btnNewClick(Sender: TObject);
var
  CIndex: integer;
  eRange: ExcelRange;
  Cell: TSheetSingleCellInterface;
begin
  Hide;
  try
    eRange := GetCellRange;
    ExcelSheet.Application.Set_Interactive(GetUserDefaultLCID, false);
    if not Assigned(eRange) then
      exit;

    CIndex := -1;
    if EditSingleCell(FPlaningSheet, CIndex, Self.Handle) then
    begin
      RefreshList.Add(IntToStr(CIndex));
//      Include(RefreshSet, CIndex);
      Cell := FPlaningSheet.SingleCells[CIndex];
      MarkObject(ExcelSheet, ERange, Cell.ExcelName, false);
      AddedCellsList.Add(Cell.ExcelName);
      HistoryList.Add(Format('Добавлен отдельный показатель "%s" (куб: "%s", мера: "%s"), по адресу %s',
        [Cell.Name, Cell.CubeName, Cell.MeasureName, Cell.Address]));
      Enlist;
    end;
  finally
    Show;
  end;
end;

procedure TfmSingleCellManager.btnCopyClick(Sender: TObject);
var
  CIndex, i: integer;
  eRange: ExcelRange;
  tmpDom: IXMLDomDocument2;
  tmpNode: IXMLDomNode;
  OldCell, NewCell: TSheetSingleCellInterface;
  Found: boolean;
begin
  OldCell := CurrentCell;
  Hide;
  try
    eRange := GetCellRange;
    ExcelSheet.Application.Set_Interactive(GetUserDefaultLCID, false);
    if not Assigned(eRange) then
      exit;
    NewCell := FPlaningSheet.SingleCells.Append;
    try
      GetDomDocument(tmpDom);
      tmpNode := tmpDom.createNode(1, 'tmpNode', '');
      OldCell.WriteToXML(tmpNode);
      FPlaningSheet.InCopyMode := true;
      NewCell.ReadFromXML(tmpNode);   
      CIndex := FPlaningSheet.SingleCells.Count - 1;
      if NewCell.TotalType <> wtConst then
      repeat
        NewCell.Name := NewCell.Name + '_копия' ;
        Found := false;
        for i := 0 to CIndex - 1 do
          if FPlaningSheet.SingleCells[i].Name = NewCell.Name then
          begin
            Found := true;
            break;
          end;
      until not Found;
      RefreshList.Add(IntToStr(CIndex));
      MarkObject(ExcelSheet, ERange, NewCell.ExcelName, false);
      AddedCellsList.Add(NewCell.ExcelName);
      HistoryList.Add(Format('Добавлен (копированием) отдельный показатель "%s" (куб: "%s", мера: "%s"), по адресу %s',
        [NewCell.Name, NewCell.CubeName, NewCell.MeasureName, NewCell.Address]));
    finally
      FPlaningSheet.InCopyMode := false;
      Enlist;
      KillDomDocument(tmpDom);
    end;
  finally
    Show;
  end;
end;

procedure TfmSingleCellManager.btnMoveClick(Sender: TObject);
var
  OldCell: TSheetSingleCellInterface;
  eRange, OldRange: ExcelRange;
  eName: ExcelXP.Name;
  OldAddress: string;
  PlacedInTotals: boolean;
begin
  OldCell := CurrentCell;
  OldRange := GetRangeByName(ExcelSheet, OldCell.ExcelName);
  Hide;
  try
    eRange := GetCellRange;
    ExcelSheet.Application.Set_Interactive(GetUserDefaultLCID, false);
    if not Assigned(eRange) then
      exit;
    ExcelSheet.Application.ScreenUpdating[FLCID] := false;
    (FPlaningSheet as TSheetLogic).StartOperation(ExcelSheet.Application.Hwnd,
      pfoSingleResultMove);
    {если есть ссылки на данную ячейку в типовых формулах показателей, то до
    перемещения очистим значения типовой формулы, а после заново выставим}
    if not SetSheetProtection(ExcelSheet, false) then
    begin
      FPlaningSheet.PostMessage(ermWorksheetProtectionFault, msgError);
      exit;
    end;
    OldCell.ClearLinkedTypeFormulasValues;

    OldAddress := OldCell.Address;
    eName := GetNameObject(ExcelSheet, OldCell.ExcelName);
    PlacedInTotals := OldCell.PlacedInTotals;

    if Assigned(eName) then
      eName.Delete;
    {$WARNINGS OFF}
    if Assigned(OldRange) then
    begin
      OldRange.Copy(ERange);
      OldRange.Clear;
      if PlacedInTotals then
        FormatRange(FPlaningSheet, OldRange);
    end;
    {$WARNINGS ON}
    MarkObject(ExcelSheet, ERange, OldCell.ExcelName, false);
    MarkUserObject(ExcelSheet, ERange, OldCell.GetUserExcelName, false);
    RefreshList.Add(IntToStr(OldCell.GetSelfIndex));

    {как и обещали, обновляем типовые формулы ссылающиеся на эту отдельную ячейку}
    OldCell.MapLinkedTypeFormulasValues;
    HistoryList.Add(Format('Перемещен отдельный показатель "%s" (куб: "%s", мера: "%s"), старый адрес: %s, новый адрес: %s',
      [OldCell.Name, OldCell.CubeName, OldCell.MeasureName, OldAddress, OldCell.Address]));
    Enlist;
  finally
    ExcelSheet.Application.ScreenUpdating[FLCID] := true;
    (FPlaningSheet as TSheetLogic).StopOperation;
    Show;
  end;
end;

procedure TfmSingleCellManager.btnCancelClick(Sender: TObject);
var
  i: integer;
  AName, Address: string;
  eRange: ExcelRange;
begin
  ClearAddedCellsNames(ExcelSheet, AddedCellsList);
  for i := 0 to DeletedCellsList.Count - 1 do
  begin
    AName := DeletedCellsList.Names[i];
    Address := DeletedCellsList.Values[AName];
    eRange := GetDeletedCellRange(Address);
    MarkObject(ExcelSheet, eRange, AName, false);
  end;
end;

procedure TfmSingleCellManager.lvCellsChange(Sender: TObject;
  Item: TListItem; Change: TItemChange);
var
  Cell: TSheetSingleCellInterface;
  eRange: ExcelRange;
  DetailText: string;
begin
  if Change <> ctState then
    exit;
  mDetail.Clear;
  Cell := TSheetSingleCellInterface(Item.Data);
  if not Assigned(Cell) then
    exit;
  DetailText := Cell.CommentText;
  mDetail.Lines.BeginUpdate;
  while DetailText <> '' do
    mDetail.Lines.Add(CutPart(DetailText, #10));
  mDetail.SelStart := 0;
  SendMessage(mDetail.Handle,EM_SCROLLCARET,0,0);
  mDetail.Lines.EndUpdate;
  ERange := GetRangeByName(ExcelSheet, Cell.ExcelName);
  if Assigned(ERange) then
    ERange.Activate;  //ERange.Select;
  EnableButtons;
end;

procedure ClearAddedCellsNames(ExcelSheet: ExcelWorkSheet;
  AddedCellsList: TStringList);
var
  i: integer;
  eName: ExcelXP.Name;
begin
  for i := 0 to AddedCellsList.Count - 1 do
  begin
    eName := GetNameObject(ExcelSheet, AddedCellsList[i]);
    if Assigned(eName) then
      eName.Delete;
  end;
end;

function TfmSingleCellManager.GetCellRange: ExcelRange;
const
  APrompt = 'Укажите координаты, по которым требуется разместить отдельный показатель';
  ErrorTxt = 'Невозможно разместить отдельный показатель по указанным координатам.'#10;
var
  v: Variant;
  res: hresult;
  MsgText: string;
  Editable: boolean;
begin
  result := nil;
  while true do
  begin
    v := ExcelSheet.Application.InputBox(APrompt, 'Координаты ячейки', EmptyParam,
      EmptyParam, EmptyParam, EmptyParam, EmptyParam, 8, GetUserDefaultLCID);
    if not Assigned(TVarData(v).VDispatch) then
      exit;
    res := IDispatch(TVarData(v).VDispatch).QueryInterface(DIID_ExcelRange, result);
    if res = S_OK then
    begin
      {Проверка на совпадение целевого листа с текущим - разрешаем размещение
        только в пределах активного листа, иначе кирдык. Проверку построим на
        совпадении имен листов и одинаковой ссылке на родительскую книгу.}
      if not ((result.WorkSheet.name = ExcelSheet.name) and
        (result.WorkSheet.parent = ExcelSheet.parent))then
      begin
        ShowError(ErrorTxt + 'Допускается указание ячейки только в пределах активного листа.');
        result := nil;
        exit;
      end;
      if IsSingleCellAllowed(ExcelSheet, result, FPlaningSheet.WritablesInfo,
        MsgText, Editable) then
        exit
      else
      begin
        ShowError(ErrorTxt + MsgText);
        result := nil;
      end;
    end
    else
      exit;
  end;
end;

procedure TfmSingleCellManager.btnOKClick(Sender: TObject);
var
  i: integer;
  AName, Address: string;
  eRange: ExcelRange;
begin
  SetSheetProtection(ExcelSheet, false);
  for i := 0 to DeletedCellsList.Count - 1 do
  begin
    AName := DeletedCellsList.Names[i];
    Address := DeletedCellsList.Values[AName];
    if Address = '' then
      continue;
    eRange := GetDeletedCellRange(Address);
    if not Assigned(eRange) then
      continue;
    eRange.Clear;
  end;
  SetSheetProtection(ExcelSheet, true);
end;

function TfmSingleCellManager.GetDeletedCellRange(Address: string): ExcelRange;
var
  CIndex, RIndex: integer;
begin
  if Address[2] in ['0'..'9'] then
  begin
    CIndex := GetColumnIndex(Address[1]);
    Delete(Address, 1, 1);
  end
  else
  begin
    CIndex := GetColumnIndex(Copy(Address, 1, 2));
    Delete(Address, 1, 2);
  end;
  RIndex := StrToInt(Address);
  result :=  GetRange(ExcelSheet, RIndex, CIndex, RIndex, CIndex);
end;

function TfmSingleCellManager.GetCurrentCell: TSheetSingleCellInterface;
begin
  result := nil;
  if Assigned(lvCells.Selected) then
    result := TSheetSingleCellInterface(lvCells.Selected.Data);
end;

procedure TfmSingleCellManager.lvCellsCompare(Sender: TObject; Item1,
  Item2: TListItem; Data: Integer; var Compare: Integer);
var
  Cell1, Cell2: TSheetSingleCellInterface;
begin
  Cell1 := TSheetSingleCellInterface(Item1.Data);
  Cell2 := TSheetSingleCellInterface(Item2.Data);
  if SortMode = smName then
  begin
    if Cell1.Name < Cell2.Name then
      Compare := -1
    else
      if Cell1.Name = Cell2.Name then
        Compare := 0
      else
        Compare := 1;
  end
  else
  begin
    if Cell1.Address < Cell2.Address then
      Compare := -1
    else
      if Cell1.Address = Cell2.Address then
        Compare := 0
      else
        Compare := 1;
  end;
end;

procedure TfmSingleCellManager.lvCellsColumnClick(Sender: TObject;
  Column: TListColumn);
begin
  if Column.Index = 0 then
    SortMode := smName
  else
    if Column.Index = 1 then
      SortMode := smAddress
    else
      SortMode := smNone;
  if SortMode = smNone then
    lvCells.SortType := stNone
  else
    lvCells.SortType := stData;
  lvCells.AlphaSort;
end;

procedure TfmSingleCellManager.TryPositioning(Target: ExcelRange);
var
  AName, Id: string;
  Params: TStringList;
  i: integer;
begin
  if lvCells.Items.Count = 0 then
    exit;
  if not Assigned(Target) then
    exit;
  AName := GetNameByRange(ExcelSheet, Target);
  if not ParseExcelName(AName, Params) then
    exit;
  try
    try
      Id := Params[1];
    except
    end;
    for i := 0 to lvCells.Items.Count - 1 do
      if TSheetSingleCellInterface(lvCells.Items[i].Data).UniqueID = Id then
      begin
        lvCells.Selected := lvCells.Items[i];
        break;
      end;
  finally
    FreeStringList(Params);
  end;
end;

procedure TfmSingleCellManager.btnFindClick(Sender: TObject);
var
  FindForm: TFindForm;
begin
  FindForm := TFindForm.Create(Self);
  FindForm.Show;
end;

function IsSingleCellAllowed(ExcelSheet: ExcelWorkSheet;
  Target: ExcelRange; WritablesInfo: TWritablesInfo; out MsgText: string;
  out Editable: boolean): boolean;
var
  IsSelRowOrCol: boolean; //признак что выбрана только строка или только столбец
  AddressTarget, RangeName: string;
begin
  result := false;
  MsgText := '';

  { Сперва сразу отсеем диапазоны более чем из одной ячейки}

  AddressTarget := GetAddressLocal(Target);
  { Строка или столбец целиком}
  IsSelRowOrCol := not((Pos('R', AddressTarget) > 0) and (Pos('C', AddressTarget) > 0));
  if IsSelRowOrCol then
  begin
    MsgText := 'Выбранный диапазон содержит несколько ячеек.';
    exit;
  end;
  { Объединение ячеек}
  if TVarData(Target.MergeCells).VBoolean then
  begin
    MsgText := 'Выбранный диапазон содержит объединенные ячейки.';
    exit;
  end;
  { Просто больше, чем одна ячейка}
  if (Target.Columns.Count > 1) or (Target.Rows.Count > 1) then
  begin
    MsgText := 'Выбранный диапазон содержит несколько ячеек.';
    exit;
  end;

  if BreakPointSelected(ExcelSheet, Target)  then
  begin
    if (GetNameByRange(ExcelSheet, Target) = '') then
      result := true
    else
      MsgText := 'Выбранный диапазон уже содержит отдельную ячейку.';
    exit;
  end;

  if TableSelected(ExcelSheet, Target) then
  begin
    if WritablesInfo.CheckForWritableColumn(ExcelSheet, Target, Editable) then
    begin
      RangeName := GetNameByRange(ExcelSheet, Target);
      if (RangeName = '') then
        result := true
      else
        if IsSingleCellName(RangeName) then
        begin
          result := false;
          MsgText := 'Выбранный диапазон уже содержит отдельную ячейку.';
        end
        else
          result := true;
    end
    else
      MsgText := 'Выбранный диапазон содержит защищенные ячейки.';
  end
  else
  begin
    RangeName := GetNameByRange(ExcelSheet, Target);
    if (RangeName = '') then
      if IsNestedRanges(Target, GetExtendedTableRange(ExcelSheet)) then
      begin
        result := false;
        MsgText := 'Выбранный диапазон находится справа от таблицы.';
      end
      else
        result := true
    else
      if IsNameOurs(RangeName) then
      begin
        result := false;
        MsgText := 'Выбранный диапазон уже содержит отдельную ячейку.';
      end
      else
        result := true;
  end;
end;

end.

