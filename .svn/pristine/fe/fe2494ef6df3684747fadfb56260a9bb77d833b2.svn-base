{
  Библиотечный модуль.
  Подпрограммы вывода "руссифицированных" формочек с сообщениями.
}

unit uRusDlg;

interface

uses StdCtrls, Dialogs, Controls;

type
  TButtonsCaptions = array of String;

// обычный диалог
function RusMessageDlg(const Msg: string; DlgType: TMsgDlgType;
                        Buttons: TMsgDlgButtons; HelpCtx: Longint;
                        DefModalResult: integer = -1): integer;
                        
// диалог в котором при добавлении кнопок можно указывать их имена
function RusMessageDlg_(const Msg: string; DlgType: TMsgDlgType;
                       Buttons: TMsgDlgButtons; ButtonsCaptions: TButtonsCaptions;
                       HelpCtx: Longint; DefModalResult: integer = -1): integer;

// диалог + координаты вывода формы                        
function RusMessageDlgPos(const Msg: string; DlgType: TMsgDlgType;
                           Buttons: TMsgDlgButtons; HelpCtx: Longint;
                           X, Y: integer; DefModalResult: integer = -1): integer;

// диалог + координаты вывода формы + кнопка файла справки                       
function RusMessageDlgPosHelp(const Msg: string; DlgType: TMsgDlgType;
                               Buttons: TMsgDlgButtons; HelpCtx: Longint;
                               X, Y: integer; const HelpFileName: string;
                               DefModalResult: integer = -1): integer;
function RusMessageDlgPosHelp_(const Msg: string; DlgType: TMsgDlgType;
                               Buttons: TMsgDlgButtons;
                               ButtonsCaptions: TButtonsCaptions; HelpCtx: Longint;
                               X, Y: integer; const HelpFileName: string;
                               DefModalResult: integer = -1): integer;

implementation

var
  Captions: array[TMsgDlgType] of string = ('Предупреждение', 'Ошибка',
                                            'Информация', 'Подтверждение', '');

function RusMessageDlg(const Msg: string; DlgType: TMsgDlgType;
                       Buttons: TMsgDlgButtons; HelpCtx: Longint;
                       DefModalResult: integer = -1): integer;
begin
  Result := RusMessageDlgPosHelp(Msg, DlgType, Buttons, HelpCtx, -1, -1, '', DefModalResult);
end;

function RusMessageDlg_(const Msg: string; DlgType: TMsgDlgType;
                       Buttons: TMsgDlgButtons; ButtonsCaptions: TButtonsCaptions;
                       HelpCtx: Longint; DefModalResult: integer = -1): integer;
begin
  Result := RusMessageDlgPosHelp_(Msg, DlgType, Buttons, ButtonsCaptions,
    HelpCtx, -1, -1, '', DefModalResult);
end;

function RusMessageDlgPos(const Msg: string; DlgType: TMsgDlgType;
                          Buttons: TMsgDlgButtons; HelpCtx: Longint;
                          X, Y: integer; DefModalResult: integer = -1): integer;
begin
  Result := RusMessageDlgPosHelp(Msg, DlgType, Buttons, HelpCtx, X, Y, '', DefModalResult);
end;

function RusMessageDlgPosHelp(const Msg: string; DlgType: TMsgDlgType;
                              Buttons: TMsgDlgButtons; HelpCtx: Longint;
                              X, Y: integer; const HelpFileName: string;
                              DefModalResult: integer = -1): integer;
var
  i: integer;
  Button: TButton;
begin
  with CreateMessageDialog(Msg, DlgType, Buttons) do
  try
    HelpContext := HelpCtx;
    HelpFile := HelpFileName;
    if (X >= 0) then
      Left := X;
    if (Y >= 0) then
      Top := Y;
    if (DlgType <> mtCustom) then
      Caption := Captions[DlgType];
    for i := 0 to ControlCount - 1 do
      if (Controls[i] is TButton) then
      begin
        Button := Controls[i] as TButton;
        case Button.ModalResult of
          mrYes: Button.Caption := '&Да';
          mrNo: Button.Caption := '&Нет';
          mrOk: Button.Caption := '&ОК';
          mrCancel: Button.Caption := 'О&тмена';
          mrAbort: Button.Caption := '&Прервать';
          mrRetry: Button.Caption := 'Повто&рить';
          mrIgnore: Button.Caption := 'Пропу&стить';
          mrAll: Button.Caption := 'Для &всех';
          mrNoToAll: Button.Caption := 'Нет для всех';
          mrYesToAll: begin
            Button.Caption := 'Да для всех';
            Button.Width := 70;
          end;
          0: Button.Caption := 'Справ&ка';
        end;
        // выделяем нужную кнопку
        if (Button.ModalResult = DefModalResult) then
          ActiveControl := TWinControl(Controls[i]);
      end;
    Result := ShowModal;
  finally
    Free;
  end;
end;

function RusMessageDlgPosHelp_(const Msg: string; DlgType: TMsgDlgType;
                              Buttons: TMsgDlgButtons;
                              ButtonsCaptions: TButtonsCaptions; HelpCtx: Longint;
                              X, Y: integer; const HelpFileName: string;
                              DefModalResult: integer = -1): integer;
var
  i, j: integer;
  Button: TButton;
begin
  with CreateMessageDialog(Msg, DlgType, Buttons) do
  try
    HelpContext := HelpCtx;
    HelpFile := HelpFileName;
    if (X >= 0) then
      Left := X;
    if (Y >= 0) then
      Top := Y;
    if (DlgType <> mtCustom) then
      Caption := Captions[DlgType];
    j := 0;
    for i := 0 to ControlCount - 1 do
      if (Controls[i] is TButton) then
      begin
        Button := Controls[i] as TButton;
        Button.Caption := ButtonsCaptions[j];
        inc(j);
        // выделяем нужную кнопку
        if (Button.ModalResult = DefModalResult) then
          ActiveControl := TWinControl(Controls[i]);
      end;
    Result := ShowModal;
  finally
    Free;
  end;
end;

end.
