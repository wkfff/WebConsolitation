unit uWritebackOptions;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls;

type
  TfmWritebackOptions = class(TForm)
    btnOK: TButton;
    btnCancel: TButton;
    Label1: TLabel;
    Bevel1: TBevel;
    Panel1: TPanel;
    rbWrite: TRadioButton;
    rbRewrite: TRadioButton;
    cbProcess: TCheckBox;
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  function OptionalWriteback(out NeedRewrite: boolean; out NeedProcess: boolean): boolean;

implementation

{$R *.DFM}

function OptionalWriteback(out NeedRewrite: boolean; out NeedProcess: boolean): boolean;
var
  fmWritebackOptions: TfmWritebackOptions;
begin
  fmWritebackOptions :=  TfmWritebackOptions.Create(nil);
  with fmWritebackOptions do
  begin
    result := (ShowModal = mrOK);
    NeedRewrite := rbRewrite.Checked;
    NeedProcess := cbProcess.Checked;
  end;
  FreeAndNil(fmWritebackOptions);
end;

end.
