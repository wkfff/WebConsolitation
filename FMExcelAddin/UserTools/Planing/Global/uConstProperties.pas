unit uConstProperties;

interface

uses
  Windows, Messages, SysUtils,  Classes, Graphics, Controls, Forms, Dialogs,
  uSheetconst, StdCtrls, uSheetObjectModel, uFMAddinGeneralUtils, Buttons;

type
  TfrmConstProperties = class(TForm)
    Label4: TLabel;
    btbCancel: TBitBtn;
    bOK: TButton;
    GroupBox2: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    edName: TEdit;
    meComment: TMemo;
    cbIsInherited: TCheckBox;
    eValue: TEdit;
    procedure FormShow(Sender: TObject);
    procedure edNameChange(Sender: TObject);
    procedure bOKClick(Sender: TObject);
    procedure btbCancelClick(Sender: TObject);
  private
    FConst: TConstInterface;
    FConsts: TConstCollectionInterface;
    FTaskContext: TTaskContext;
    FApply: boolean;
    procedure SetStatic;
    function IsDuplicate(AName: string): boolean;
  public
    property Constant: TConstInterface read FConst write FConst;
    property Consts: TConstCollectionInterface read FConsts write FConsts;
    property TaskContext: TTaskContext read FTaskContext write FTaskContext;
    property Apply: boolean read FApply write FApply;
  end;

implementation

{$R *.DFM}

{ TfrmConstProperties }

procedure TfrmConstProperties.FormShow(Sender: TObject);
begin
  edName.Text := FConst.Name;
  meComment.Text := FConst.Comment;
  eValue.Text := FConst.Value;
  cbIsInherited.Checked := FConst.IsInherited;
  if FConst.IsInherited then
    SetStatic;
  bOk.Enabled := (edName.Text <> '');
  FApply := false;  
end;

procedure TfrmConstProperties.edNameChange(Sender: TObject);
begin
  bOk.Enabled := (edName.Text <> '');
end;

procedure TfrmConstProperties.bOKClick(Sender: TObject);
begin
  // проверка на уникальность имени
  if (edName.Text <> FConst.Name) then
  begin
    if IsDuplicate(edName.Text) then
    begin
      ShowError('Константа с таким именем уже существует');
      exit;
    end;
  end;
  if (eValue.Text = '') then
  begin
    ShowError('Значение константы должно быть определено');
    exit;
  end;
  FConst.Name := edName.Text;
  FConst.Comment := meComment.Text;
  FConst.Value := eValue.Text;
  FApply := true;
  Close;
end;

procedure TfrmConstProperties.btbCancelClick(Sender: TObject);
begin
  FApply := false;
end;

procedure TfrmConstProperties.SetStatic;
begin
  edName.Enabled := false;
  meComment.Enabled := false;
  eValue.Enabled := false;
  bOk.Enabled := false;
end;

function TfrmConstProperties.IsDuplicate(AName: string): boolean;
var
  i: integer;
  SheetConst: TConstInterface;
begin
  result := false;
  AName := AnsiUpperCase(AName);
  for i := 0 to FConsts.Count - 1 do
  begin
    SheetConst := FConsts[i];
    if AName = AnsiUpperCase(SheetConst.Name) then
    begin
      result := true;
      exit;
    end;
  end;

  if not Assigned(FTaskContext) then
    exit;
  result := Assigned(FTaskContext.GetTaskConsts.ConstByName(AName));
end;

end.

