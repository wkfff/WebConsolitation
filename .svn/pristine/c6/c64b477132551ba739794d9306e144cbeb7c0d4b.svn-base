unit uFindForm;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls;

type
  TFindForm = class(TForm)
    edText: TEdit;
    btnFind: TButton;
    btnCancel: TButton;
    cbCase: TCheckBox;
    procedure FormCreate(Sender: TObject);
    procedure btnFindClick(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure btnCancelClick(Sender: TObject);
  private
    CurrentIndex: integer;

  //  procedure Find;
  public
    CellsList: TListview;
  end;



implementation

{$R *.DFM}


(*
procedure TFindForm.Find;
var
  InitialIndex: integer;
  WhatToFind, CurrentCaption: string;
  Found: boolean;
begin
  InitialIndex := CurrentIndex;
  if cbCase.Checked then
    WhatToFind := edText.Text
  else
    WhatToFind := AnsiUpperCase(edText.Text);

  repeat
    if CurrentIndex = CellsList.Items.Count - 1 then
      CurrentIndex := 0;
    CurrentCaption := CellsList.Items[CurrentIndex].Caption;
    if not cbCase.Checked then
      CurrentCaption := AnsiUpperCase(CurrentCaption);
    Found := Pos(WhatToFind, CurrentCaption) > 0;
    if Found then
      CellsList.Selected := CellsList.Items[CurrentIndex]
  Until Found or (CurrentIndex = InitialIndex);
end;     *)

procedure TFindForm.FormCreate(Sender: TObject);
begin
  CurrentIndex := 0;
  cbCase.Checked := false;
end;

procedure TFindForm.btnFindClick(Sender: TObject);
begin
  //Find;
end;

procedure TFindForm.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  Action := caFree;
end;

procedure TFindForm.btnCancelClick(Sender: TObject);
begin
  Close;
end;

end.
