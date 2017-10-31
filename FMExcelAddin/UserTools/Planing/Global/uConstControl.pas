unit uConstControl;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, ImgList, StdCtrls, Buttons, ComCtrls, uSheetObjectModel, uSheetParam,
  uConstProperties, uFMAddinGeneralUtils, uSheetLogic, uSheetHistory;

type
  TfrmConstControl = class(TForm)
    Panel3: TPanel;
    lvConsts: TListView;
    btOK: TBitBtn;
    btCancel: TBitBtn;
    btConstProperties: TBitBtn;
    btDeleteConst: TBitBtn;
    btAddConst: TBitBtn;
    ImageList: TImageList;
    procedure FormShow(Sender: TObject);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
    procedure lvConstsDblClick(Sender: TObject);
    procedure btConstPropertiesClick(Sender: TObject);
    procedure btOKClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure bAddConstClick(Sender: TObject);
    procedure bDeleteConstClick(Sender: TObject);
    procedure lvConstsClick(Sender: TObject);
    procedure lvConstsInfoTip(Sender: TObject; Item: TListItem; var InfoTip: String);
  private
    FSheetInterface: TSheetInterface;
    FConst: TConstInterface;
    FfrmConstProperties: TfrmConstProperties;
    FHistoryList: TStringList;
    FDeletedConsts: TStringList;
    FISConstChoiseMode: boolean;
    FNeedUpdate: boolean;
    FRenamingList: TStringList;
    procedure Init;
    function AddElement(ConstName, Value: string; UniqueId: integer; IsInherited: boolean): TListItem;
    // отображение свойств параметра
    procedure ShowConstProperties;
    // удалить константы
    procedure DeleteConsts;
  public
    // метаданные листа
    property SheetInterface: TSheetInterface read FSheetInterface write FSheetInterface;
    // истори€ изменений параметров
    property HistoryList: TStringList read FHistoryList;
    // режим - чисто выбор константы из списка
    property ISConstChoiseMode: boolean read FISConstChoiseMode write FISConstChoiseMode;
    // выбранна€ константа
    property Constant: TConstInterface read FConst;
    // нужно ли обновление листа
    property NeedUpdate: boolean read FNeedUpdate;
    // —писок переименований констант
    property RenamingList: TStringList read FRenamingList;
  end;

implementation

{$R *.DFM}

procedure TfrmConstControl.FormShow(Sender: TObject);
begin
  Init;
  FHistoryList := TStringList.Create;
  FDeletedConsts := TStringList.Create;
  FNeedUpdate := false;
  btOk.Enabled := not FISConstChoiseMode;
end;

procedure TfrmConstControl.Init;
var
  i: integer;
begin
  for i := 0 to SheetInterface.Consts.Count - 1 do
    with SheetInterface.Consts.Items[i] do
      AddElement(Name, Value, UniqueId, IsInherited);
  FRenamingList := TStringList.Create;
end;

function TfrmConstControl.AddElement(ConstName, Value: string; UniqueId: integer; IsInherited: boolean): TListItem;
begin
  result := lvConsts.Items.Add;
  with result do
  begin
    Caption := ConstName;
    SubItems.Append(Value);
    Data := Pointer(UniqueId);
    if IsInherited then
      StateIndex := 0;
  end;
end;

procedure TfrmConstControl.ShowConstProperties;
var
  EditingConstName: string;
  i: integer;
begin
  if (lvConsts.Selected = nil) then
    exit;
  FConst := SheetInterface.Consts.ConstByID(integer(lvConsts.Selected.Data));
  EditingConstName := FConst.Name;
  FfrmConstProperties := TfrmConstProperties.Create(nil);
  try
    FfrmConstProperties.Constant := FConst;
    FfrmConstProperties.Consts := SheetInterface.Consts;
    FfrmConstProperties.TaskContext := SheetInterface.TaskContext;
    FfrmConstProperties.ShowModal;
    if FfrmConstProperties.Apply then
    begin
      //переименование констант-показателей
      for i := 0 to SheetInterface.Totals.Count - 1 do
      begin
        if (SheetInterface.Totals[i].TotalType <> wtConst) then
          continue;
        if (SheetInterface.Totals[i].Caption = EditingConstName) then
        begin
          SheetInterface.Totals[i].Caption := FConst.Name;
          FNeedUpdate := true;
        end;
      end;
      //переименование констант-отдельных
      for i := 0 to SheetInterface.SingleCells.Count - 1 do
      begin
        if (SheetInterface.SingleCells[i].TotalType <> wtConst) then
          continue;
        if (SheetInterface.SingleCells[i].Name = EditingConstName) then
        begin
          SheetInterface.SingleCells[i].Name := FConst.Name;
          FNeedUpdate := true;
        end;
      end;
      FRenamingList.Add(EditingConstName + '=' + FConst.Name);
      lvConsts.Selected.Caption := FConst.Name;
      lvConsts.Selected.SubItems[0] := FConst.Value;
      HistoryList.Add('»зменены свойства константы "' + FConst.Name + '"');
    end;
  finally
    FreeAndNil(FfrmConstProperties);
  end;
end;

procedure TfrmConstControl.FormKeyPress(Sender: TObject; var Key: Char);
begin
  if (Key = chr(27)) then
    Close;
end;

procedure TfrmConstControl.lvConstsDblClick(Sender: TObject);
begin
  ShowConstProperties;
end;

procedure TfrmConstControl.btConstPropertiesClick(Sender: TObject);
begin
  ShowConstProperties;
end;

procedure TfrmConstControl.btOKClick(Sender: TObject);
begin
  DeleteConsts;
end;

procedure TfrmConstControl.FormDestroy(Sender: TObject);
begin
  FreeStringList(FHistoryList);
  FreeStringList(FDeletedConsts);
  FreeStringList(FRenamingList);
end;

procedure TfrmConstControl.bAddConstClick(Sender: TObject);
begin
  FConst := SheetInterface.Consts.Append;
  FConst.IsSheetConst := true;
  FConst.IsInherited := false;
  FConst.ID := -1;
  FfrmConstProperties := TfrmConstProperties.Create(Self);
  try
    FfrmConstProperties.Constant := FConst;
    FfrmConstProperties.Consts := SheetInterface.Consts;
    FfrmConstProperties.TaskContext := SheetInterface.TaskContext;
    FfrmConstProperties.ShowModal;
    if not FfrmConstProperties.Apply then
      SheetInterface.Consts.Delete(SheetInterface.Consts.FindByID(IntToStr(FConst.UniqueId)))
    else
    begin
      with FConst do
        AddElement(Name, Value, UniqueId, IsInherited);
      HistoryList.Add('ƒобавлена константа "' + FConst.Name + '"');
    end;
  finally
    FreeAndNil(FfrmConstProperties);
  end;
end;

procedure TfrmConstControl.bDeleteConstClick(Sender: TObject);

  function IsSheetElement(ConstName: string): boolean;
  var
    i: integer;
  begin
    result := true;
    for i := 0 to SheetInterface.Totals.Count - 1 do
      if (SheetInterface.Totals[i].TotalType = wtConst) and
         (SheetInterface.Totals[i].Caption = ConstName) then
        exit;
    for i := 0 to SheetInterface.SingleCells.Count - 1 do
      if (SheetInterface.SingleCells[i].TotalType = wtConst) and
         (SheetInterface.SingleCells[i].Name = ConstName) then
        exit;
    result := false;
  end;

begin
  if not Assigned(lvConsts.Selected) then
    exit;
  FConst := SheetInterface.Consts.ConstByID(integer(lvConsts.Selected.Data));
  if IsSheetElement(FConst.Name) then
  begin
    ShowError('Ќевозможно удалить размещенную константу');
    exit;
  end;
  if not ShowQuestion('”далить константу?') then
    exit;
  FDeletedConsts.Add(IntToStr(integer(lvConsts.Selected.Data)));
  lvConsts.Selected.Delete;
  if FISConstChoiseMode then
    btOk.Enabled := (lvConsts.Selected <> nil);
end;

procedure TfrmConstControl.DeleteConsts;
var
  i: integer;
  TaskConst: TTaskConst;
  ConstName: string;
  Constant: TConstInterface;
begin
  for i := 0 to FDeletedConsts.Count - 1 do
  begin
    Constant := SheetInterface.Consts.ConstByID(StrToInt(FDeletedConsts[i]));
    // мочим из коллекции констант
    ConstName := Constant.Name;
    SheetInterface.Consts.Delete(SheetInterface.Consts.FindByID(IntToStr(Constant.UniqueId)));
    HistoryList.Add('удалена константа "' + ConstName + '"');
    // мочим из задач
    if not Assigned(SheetInterface.TaskContext) then
      continue;
    TaskConst := SheetInterface.TaskContext.GetTaskConsts.ConstByName(ConstName);
    if not Assigned(TaskConst) then
      continue;
    SheetInterface.TaskContext.GetTaskConsts.Remove(TaskConst);
  end;
  FDeletedConsts.Clear;
end;

procedure TfrmConstControl.lvConstsClick(Sender: TObject);
begin
  if FISConstChoiseMode then
    btOK.Enabled := (lvConsts.Selected <> nil);
  if (lvConsts.Selected = nil) then
    exit;
  FConst := SheetInterface.Consts.ConstByID(integer(lvConsts.Selected.Data));
  btDeleteConst.Enabled := not FConst.IsInherited;
end;

procedure TfrmConstControl.lvConstsInfoTip(Sender: TObject; Item: TListItem; var InfoTip: string);
begin
  FConst := SheetInterface.Consts.ConstByID(integer(Item.Data));
  if (FConst.Comment = '') then
    InfoTip := ' омментарий: отсутствует'
  else
    InfoTip := ' омментарий: ' + FConst.Comment;
end;

end.

