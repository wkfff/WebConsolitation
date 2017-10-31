unit uParamControl;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, ImgList, StdCtrls, Buttons, ComCtrls, uSheetObjectModel, uSheetParam,
  uParamProperties, uFMAddinGeneralUtils, uSheetHistory, uExcelUtils;

const
  // надпись на кнопке, связывающей с параметром
  capLinkToParamCaption = 'Связать с параметром';
  // надпись на кнопке, удаляющей связь с параметром
  capDisLinkToParamCaption = '       Удалить связь      ';
  // нет параметра
  capNoParam = 'нет параметра';

type
  TfrmParamControl = class(TForm)
    Panel3: TPanel;
    lvParams: TListView;
    ImageList: TImageList;
    btApply: TButton;
    btOK: TBitBtn;
    btCancel: TBitBtn;
    bDeclareParam: TBitBtn;
    bParamProperties: TBitBtn;
    procedure FormKeyPress(Sender: TObject; var Key: Char);
    procedure btApplyClick(Sender: TObject);
    procedure lvParamsClick(Sender: TObject);
    procedure lvParamsDblClick(Sender: TObject);
    procedure btParamPropertiesClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure bDeclareParam_Click(Sender: TObject);
    procedure lvParamsCustomDrawItem(Sender: TCustomListView;
      Item: TListItem; State: TCustomDrawState; var DefaultDraw: Boolean);
    procedure lvParamsInfoTip(Sender: TObject; Item: TListItem; var InfoTip: String);
  private
    FSheetInterface: TSheetInterface;
    FParam: TParamInterface;
    FHistoryList: TStringList;
    procedure Init;
    procedure InitButtons(Item: TListItem);
    function AddElement(ParamName: string; IsInherited: boolean;
      SheetDimension: TSheetDimension; ObjectType, Link: string): TListItem;
    // отображение свойств параметра
    procedure ShowParamProperties;
    // объявление параметром
    procedure DeclareParam;
    // отцепить от параметра
    procedure DeleteParam;
  public
    // метаданные листа
    property SheetInterface: TSheetInterface read FSheetInterface write FSheetInterface;
    // история изменений параметров
    property HistoryList: TStringList read FHistoryList;
  end;

function EditParams(SI: TSheetInterface): boolean;

implementation

{$R *.DFM}

function EditParams(SI: TSheetInterface): boolean;
var
  ParamForm: TfrmParamControl;
begin
  ParamForm := TfrmParamControl.Create(nil);
  with ParamForm do
  try
    SheetInterface := SI;
    FHistoryList := TStringList.Create;
    Init;
    with lvParams do
      if Items.Count > 0 then
      begin
        Selected := Items[0];
        InitButtons(Selected);
      end;
    result := ShowModal = mrOK;
    if result then
    begin
      SheetInterface.AddEventInSheetHistory(evtParamsEdit, HistoryList.CommaText, true);
    end;
  finally
    ParamForm.Free;
  end;
end;

procedure TfrmParamControl.Init;
var
  i: integer;

  procedure ScanAxis(Axis: TSheetAxisCollectionInterface);
  var
    i: integer;
    ParamName, Link: string;
    IsInherited: boolean;
  begin
    for i := 0 to Axis.Count - 1 do
      with Axis[i] do
      begin
        if not OfPrimaryProvider then
          continue;
        ParamName := capNoParam;
        IsInherited := false;
        Link := '';
        if IsParam then
        begin
          ParamName := Param.Name;
          IsInherited := Param.IsInherited;
          Link := IIF(Param.ID > -1, 'Есть', 'Нет');
        end;
        AddElement(ParamName, IsInherited, Axis[i], Axis[i].GetObjectTypeStr, Link);
      end;
  end;

  procedure ScanFilters(Filters: TSheetFilterCollectionInterface; SingleCell: TSheetSingleCellInterface);
  var
    i: integer;
    ParamName, ObjectTypeStr, Link: string;
    IsInherited: boolean;
  begin
    for i := 0 to Filters.Count - 1 do
      with Filters[i] do
      begin
        if not OfPrimaryProvider then
          continue;
        ParamName := capNoParam;
        IsInherited := false;
        Link := '';
        if IsParam then
        begin
          ParamName := Param.Name;
          IsInherited := Param.IsInherited;
          Link := IIF(Param.ID <> -1, 'Есть', 'Нет');
        end;
        if (SingleCell = nil) then
          ObjectTypeStr := GetObjectTypeStr
        else
          ObjectTypeStr := 'Фильтр отдельного показателя "' + SingleCell.Name + '" (Мера "' +
            SingleCell.MeasureName + '"; Куб "' + SingleCell.CubeName + '")';
        AddElement(ParamName, IsInherited, Filters[i], ObjectTypeStr, Link);
      end;
  end;

begin
  ScanAxis(FSheetInterface.Rows);
  ScanAxis(FSheetInterface.Columns);
  ScanFilters(FSheetInterface.Filters, nil);
  for i := 0 to FSheetInterface.SingleCells.Count - 1 do
    ScanFilters(FSheetInterface.SingleCells[i].Filters, FSheetInterface.SingleCells[i]);
end;

procedure TfrmParamControl.InitButtons(Item: TListItem);

  {Устанавливает одну из быстрых кнопок иконку из ImageList с указанным номером}
  procedure SetButtonImmage(Button: TBitBtn; ImIndex: integer);
  begin
    Button.Glyph := nil; //Если не занилить, не получится
    ImageList.GetBitMap(ImIndex, Button.Glyph);
    Button.Invalidate;
  end;

begin
  if (Item = nil) then
    exit;
  //lvParams.SetFocus;
  bDeclareParam.Enabled := true;
  if (Item.Caption = capNoParam) then
  begin
    bDeclareParam.Caption := capLinkToParamCaption;
    SetButtonImmage(bDeclareParam, 0);
    bParamProperties.Enabled := false;
  end
  else
  begin
    bDeclareParam.Caption := capDisLinkToParamCaption;
    SetButtonImmage(bDeclareParam, 1);
    bParamProperties.Enabled := true;
  end;
end;

function TfrmParamControl.AddElement(ParamName: string; IsInherited: boolean;
  SheetDimension: TSheetDimension; ObjectType, Link: string): TListItem;
begin
  result := lvParams.Items.Add;
  with result do
  begin
    Caption := ParamName;
    if (Caption = '') then
      Caption := capNoParam;
    if (Caption = capNoParam) then
      StateIndex := 1
    else
      if IsInherited then
        StateIndex := 4
      else
        StateIndex := 0;

    SubItems.Append(SheetDimension.FullDimensionName2);
    SubItems.Append(ObjectType);
    SubItems.Append(Link);
    Data := Pointer(SheetDimension);
  end;
end;

procedure TfrmParamControl.DeclareParam;
var
  SheetDimension: TSheetDimension;
begin
  try
    SheetDimension := TSheetDimension(lvParams.Selected.Data);
    if EditParamProperties(SheetDimension) then
    begin
      FParam := SheetDimension.Param;
      lvParams.Selected.Caption := FParam.Name;
      if FParam.IsInherited then
        lvParams.Selected.StateIndex := 4
      else
        lvParams.Selected.StateIndex := 0;
      HistoryList.Add('Добавлен параметр "' + FParam.FullName + '"');
      btApply.Enabled := true;
    end;
  finally
    InitButtons(lvParams.Selected);
  end;
end;

procedure TfrmParamControl.DeleteParam;
var
  SheetDimension: TSheetDimension;
  ParamName: string;
begin
  if Assigned(lvParams.Selected) then
    if ShowQuestion('Удалить связь с параметром?') then
    begin
      lvParams.Selected.Caption := capNoParam;
      lvParams.Selected.StateIndex := 1;
      SheetDimension := TSheetDimension(lvParams.Selected.Data);
      ParamName := SheetDimension.Param.FullName;
      SheetDimension.Param.RemoveLink(SheetDimension);
      HistoryList.Add('удален параметр "' + ParamName + '" (действие пользователя)');
//      SheetInterface.Params.DeleteParam(TSheetDimension(lvParams.Selected.Data).UniqueID);
      btApply.Enabled := true;
      InitButtons(lvParams.Selected);
    end;
end;

procedure TfrmParamControl.ShowParamProperties;
var
  EditedParamName: string;
  SheetDimension: TSheetDimension;
begin
  if not Assigned(lvParams.Selected) then
    exit;
  if (lvParams.Selected.Caption = capNoParam) then
    exit;

  SheetDimension := TSheetDimension(lvParams.Selected.Data);
  if EditParamProperties(SheetDimension) then
  begin
    FParam := SheetDimension.Param;
    EditedParamName := lvParams.Selected.Caption;
    lvParams.Selected.Caption := FParam.Name;
    if FParam.IsInherited then
      lvParams.Selected.StateIndex := 4
    else
      lvParams.Selected.StateIndex := 0;
    HistoryList.Add('Изменены свойства параметра "' + FParam.FullName + '"');
    btApply.Enabled := true;
  end;
end;

procedure TfrmParamControl.FormKeyPress(Sender: TObject; var Key: Char);
begin
  if (Key = chr(27)) then
    Close;
end;

procedure TfrmParamControl.btApplyClick(Sender: TObject);
begin
  FSheetInterface.Save;
  btApply.Enabled := false;
  SheetInterface.AddEventInSheetHistory(evtParamsEdit, HistoryList.CommaText, true);
  HistoryList.Clear;
end;

procedure TfrmParamControl.lvParamsClick(Sender: TObject);
begin
  InitButtons(lvParams.Selected);
end;

procedure TfrmParamControl.lvParamsDblClick(Sender: TObject);
begin
  ShowParamProperties;
end;

procedure TfrmParamControl.btParamPropertiesClick(Sender: TObject);
begin
  ShowParamProperties;
end;

procedure TfrmParamControl.FormDestroy(Sender: TObject);
begin
  FreeStringList(FHistoryList);
end;

procedure TfrmParamControl.bDeclareParam_Click(Sender: TObject);
begin
  if (lvParams.Selected = nil) then
    exit;
  if (lvParams.Selected.Caption = capNoParam) then
    DeclareParam
  else
    DeleteParam;                             
end;

procedure TfrmParamControl.lvParamsCustomDrawItem(Sender: TCustomListView;
  Item: TListItem; State: TCustomDrawState; var DefaultDraw: Boolean);
begin
  if (Item.Caption = capNoParam) then
    Sender.Canvas.Brush.Color := clWhite
  else
    Sender.Canvas.Brush.Color := $00ACD7AE;
  Sender.Canvas.FillRect(Item.DisplayRect(drBounds));
end;

procedure TfrmParamControl.lvParamsInfoTip(Sender: TObject; Item: TListItem; var InfoTip: string);
begin
  if (Item.Caption = capNoParam) then
    exit;
  FParam := TSheetDimension(Item.Data).Param;
  if (FParam.Comment = '') then
    InfoTip := 'Комментарий: отсутствует'
  else
    InfoTip := 'Комментарий: ' + FParam.Comment;
end;

end.

