unit uParamProperties;

interface

uses
  Windows, Messages, SysUtils,  Classes, Graphics, Controls, Forms, Dialogs,
  uSheetParam, StdCtrls, uSheetObjectModel, uFMAddinGeneralUtils, Buttons;

const
  NewParam: string = '<����� ��������>';

type
  TfrmParamProperties = class(TForm)
    GroupBox2: TGroupBox;
    edParamName: TEdit;
    Label1: TLabel;
    Label2: TLabel;
    meParamComment: TMemo;
    cbMultipleSelection: TCheckBox;
    cbParamsList: TComboBox;
    Label4: TLabel;
    lDimensionName: TLabel;
    stDimensionName: TStaticText;
    btbCancel: TBitBtn;
    stParams: TStaticText;
    bOK: TButton;
    cbIsInherited: TCheckBox;
    //procedure FormShow(Sender: TObject);
    procedure cbParamsListChange(Sender: TObject);
    procedure edParamNameChange(Sender: TObject);
    procedure bOKClick(Sender: TObject);
  private
    function GetTaskContext: TTaskContext;
  private
    FDefaultMultiSelect: boolean;
    //FParam: TParamInterface;
    FTaskParam: TTaskParam;
    FSheetDimension: TSheetDimension;
    //FDimension: string;
    FSheetInterface: TSheetInterface;
    procedure ShowParamAttributes(Name, Comment: string; MultiSelect, IsInherited: boolean);
    procedure FillParamsList;
    function GetSheetParams: TParamCollectionInterface;
    function GetSheetParam: TParamInterface;
    function IsDuplicateName: boolean;
    function GetDimensionName: string;

    property SheetParams: TParamCollectionInterface read GetSheetParams;
    property SheetParam: TParamInterface read GetSheetParam;
    property DimensionName: string read GetDimensionName;
    property TaskContext: TTaskContext read GetTaskContext;
    property TaskParam: TTaskParam read FTaskParam write FTaskParam;
    property SheetDimension: TSheetDimension read FSheetDimension write FSheetDimension;
    property SheetInterface: TSheetInterface read FSheetInterface write FSheetInterface;
  end;

function EditParamProperties(SD: TSheetDimension): boolean;


implementation

{$R *.DFM}

function EditParamProperties(SD: TSheetDimension): boolean;
var
  ParamForm: TfrmParamProperties;
begin
  ParamForm := TfrmParamProperties.Create(nil);
  with ParamForm do
  try
    SheetDimension := SD;
    SheetInterface := SheetDimension.SheetInterface;
    FillParamsList;
    if Assigned(SheetParam) then
      with SheetParam do
        ShowParamAttributes(Name, Comment, MultiSelect, IsInherited);
    result := ShowModal = mrOK;
    if result then
    begin
    end;
  finally
    ParamForm.Free;
  end;
end;

{ TfrmParamProperties }

procedure TfrmParamProperties.FillParamsList;
var
  TaskParams: TTaskParamsCollection;
  i: integer;
  TaskParam: TTaskParam;
  Param: TParamInterface;
begin
  cbParamsList.Clear;
(*  if not Assigned(SheetParam) then
  begin*)
  if not Assigned(SheetParam) then
    cbParamsList.Items.Add(NewParam);

    // ������� ��������� ������
    if Assigned(TaskContext) then
    begin
      TaskParams := TaskContext.GetTaskParams;
      for i := 0 to TaskParams.Count - 1 do
      begin
        TaskParam := TaskParams[i];
        if (TaskParam.Dimension = DimensionName) then
          cbParamsList.Items.Add(TaskParam.Name);
      end;
    end;

    // ������� ��������� ���������
    for i := 0 to SheetInterface.Params.Count - 1 do
    begin
      Param := SheetInterface.Params[i];
      if (Param.Dimension = DimensionName) and (Param.Name <> '') and
         (cbParamsList.Items.IndexOf(Param.Name) = - 1) then
        cbParamsList.Items.Add(Param.Name);
    end;
(*  end
  else
  begin
    cbParamsList.Items.Add(SheetParam.Name);
    cbParamsList.Enabled := false;
  end;*)

  if Assigned(SheetParam) and (cbParamsList.Items.IndexOf(SheetParam.Name) <> - 1) then
    cbParamsList.ItemIndex := cbParamsList.Items.IndexOf(SheetParam.Name)
  else
    cbParamsList.ItemIndex := 0;

  {�������� ���������� ������������ � ���������� ������}
(*  if (cbParamsList.Text <> NewParam) and Assigned(TaskContext) then
    try
      TaskParam := TaskContext.GetTaskParams.ParamByName(cbParamsList.Text);
    except
      TaskParam := nil;
    end;*)
end;



procedure TfrmParamProperties.cbParamsListChange(Sender: TObject);
begin
  FTaskParam := nil;

  {��� ������ ��������� - �� �������}
  if (cbParamsList.Text = NewParam) then
  begin
    ShowParamAttributes('', '', FDefaultMultiSelect, false);
    exit;
  end;

  {����������� �������� ��������� �� ��������� �����}
  if Assigned(TaskContext) then
  begin
    TaskParam := TaskContext.GetTaskParams.ParamByName(cbParamsList.Text);
    if (TaskParam <> nil) then
      with TaskParam do
        ShowParamAttributes(Name, Comment, AllowMultiSelect, IsInherited);
  end
  else
    with SheetParams.ParamByName(cbParamsList.Text) do
      ShowParamAttributes(Name, Comment, MultiSelect, IsInherited);
end;

procedure TfrmParamProperties.edParamNameChange(Sender: TObject);
begin
  bOk.Enabled := (edParamName.Text <> '');
end;

procedure TfrmParamProperties.ShowParamAttributes(Name, Comment: string; MultiSelect, IsInherited: boolean);
begin
  edParamName.Text := Name;
  edParamName.Enabled := not IsInherited;
  meParamComment.Text := Comment;
  meParamComment.Enabled := not IsInherited;
  cbMultipleSelection.Checked := MultiSelect;
  cbMultipleSelection.Enabled := not IsInherited;
  cbIsInherited.Checked := IsInherited;
  bOk.Enabled := (edParamName.Text <> '');
end;

procedure TfrmParamProperties.bOKClick(Sender: TObject);
var
  NewParamPID: integer;
  ParamName: string;
  CurrentParam: TParamInterface;
begin
  {�������� �� ������������ �����}
  if IsDuplicateName then
  begin
    ShowError('�������� � ����� ������ ��� ����������');
    exit;
  end;

  {�������� �� ������������� �����}
  if not cbMultipleSelection.Checked and
     (FSheetDimension.Members.selectNodes('//Member[@checked="true"]').length > 1) then
  begin
    ShowError('� ��������������� ��������� ���������� ����� ������� ��������� ��������� ���������.' + #10#13 +
              '�������� ������ ��������� ������������� �����.');
    exit;
  end;

  ParamName := cbParamsList.Text;
  if not Assigned(SheetParam) then
  begin
    if ParamName = NewParam then
      CurrentParam := nil
    else
      CurrentParam := SheetInterface.Params.ParamByName(ParamName);
    if not Assigned(CurrentParam) then
    begin
      NewParamPID := SheetInterface.GetPID(ParamName);
      SheetParams.AddParam(SheetDimension);
      SheetParam.ID := -1;
      SheetParam.PID := NewParamPID;
      SheetParam.IsInherited := false;
    end
    else
      CurrentParam.SetLink(SheetDimension);
  end;

  SheetParam.Name := edParamName.Text;
  SheetParam.Comment := meParamComment.Text;
  SheetParam.MultiSelect := cbMultipleSelection.Checked;
  if Assigned(TaskContext) and Assigned(TaskParam) and (ParamName <> NewParam) then
  begin
    SheetParam.ID := TaskParam.ID;
    SheetParam.IsInherited := TaskParam.IsInherited;
    SheetParam.Members.loadXML(TaskParam.Values);
  end;
  ModalResult := mrOK;
end;

function TfrmParamProperties.GetSheetParams: TParamCollectionInterface;
begin
  result := SheetInterface.Params;
end;

{�������� �� ������������ �����}
function TfrmParamProperties.IsDuplicateName: boolean;
begin
  result := false;
  if ((cbParamsList.Text = NewParam) or (cbParamsList.Text <> edParamName.Text)) and
     (Assigned(SheetParam) and (edParamName.Text <> SheetParam.Name)) then
  begin
    result := (SheetParams.ParamByName(edParamName.Text) <> nil);
    if not result then
      if Assigned(TaskContext) then
        result := (TaskContext.GetTaskParams.ParamByName(edParamName.Text) <> nil);
  end
  else
    if ((cbParamsList.Text = NewParam) or (cbParamsList.Text <> edParamName.Text)) then
      result := (SheetParams.ParamByName(edParamName.Text) <> nil)
end;


function TfrmParamProperties.GetSheetParam: TParamInterface;
begin
  result := SheetDimension.Param;
end;

function TfrmParamProperties.GetDimensionName: string;
begin
  result := SheetDimension.FullDimensionName;
end;

function TfrmParamProperties.GetTaskContext: TTaskContext;
begin
  result := SheetInterface.TaskContext;
end;

end.

