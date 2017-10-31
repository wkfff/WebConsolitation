{
  Страшненькая формочка для вывода больших сообщений. Т.е таких сообщений,
  к которым прилагается относительно объемная детальная информация
}

unit uDetailForm;

interface

uses
  Windows, Messages, SysUtils, Classes, Controls, Forms, Dialogs, Buttons, ExtCtrls, StdCtrls;

type
  TDetailForm = class(TForm)
    Panel1: TPanel;
    mError: TMemo;
    bDetail: TButton;
    mDetailError: TMemo;
    Button1: TButton;
    procedure FormCreate(Sender: TObject);
    procedure bDetailClick(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    FDetails: boolean;
    FError: widestring;
    FDetailError: widestring;
  public
    { Название ошибки }
    property Error: widestring read FError write FError;
    { Детальное описание ошибки }
    property DetailError: widestring read FDetailError write FDetailError;
  end;

implementation

{$R *.DFM}

procedure TDetailForm.FormCreate(Sender: TObject);
begin
  FDetails := true;
  bDetail.OnClick(Self);
end;

procedure TDetailForm.bDetailClick(Sender: TObject);
const
  Captions: array[boolean] of string[20] = ('Детально >>','Детально <<');
  Size: array[boolean] of integer = (24, 200);
begin
  FDetails := not FDetails;
  mDetailError.Visible := FDetails;
  bDetail.Caption := Captions[FDetails];
  Height := Panel1.Height + Size[FDetails];
end;

procedure TDetailForm.FormShow(Sender: TObject);
begin
  mError.Text := FError;
  mDetailError.Text := FDetailError;
end;

procedure TDetailForm.Button1Click(Sender: TObject);
begin
  Close;
end;

end.
