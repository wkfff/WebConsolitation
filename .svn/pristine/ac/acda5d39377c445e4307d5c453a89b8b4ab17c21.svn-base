unit brs_OperationForm;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, ComCtrls,  fmFlatPanel;

const
  WM_STARTOPERATION = WM_USER + 1;
  WM_STOPOPERATION  = WM_USER + 2;
  WM_SETCAPTION     = WM_USER + 3;

  WS_EX_LAYERED = $00080000;
  LWA_ALPHA    = $00000002;
  WS_EX_NOACTIVATE = $08000000;

type
  TSetLayeredWindowAttributes = function(hwnd : HWND; crKey : integer; bAlpha : byte; dwFlags : DWORD): BOOL; stdcall;

  TfrmOperation = class(TForm)
    pnOuther: TfmFlatPanel;
    pnCaption: TfmFlatPanel;
    anAvi: TAnimate;
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormCreate(Sender: TObject);
  private
    FParentWin: integer;
  protected
    procedure WndProc(var Msg : TMessage); override;
  public
    procedure CreateParams(var Params : TCreateParams); override;
  end;


implementation

{$R *.DFM}
{255 - Opaqye, 0 - Transparent}
procedure SetWindowTransparent(Wnd : integer; Alpha : byte);
var slwa  : TSetLayeredWindowAttributes;
begin
  slwa := GetProcAddress(GetModuleHandle(user32), 'SetLayeredWindowAttributes');
  if Assigned(slwa) then
  begin
    SetWindowLong(Wnd, GWL_EXSTYLE, GetWindowLong(Wnd, GWL_EXSTYLE) or WS_EX_LAYERED);
    slwa(Wnd, 0, Alpha, LWA_ALPHA);
  end
end;


procedure TfrmOperation.CreateParams(var Params : TCreateParams);
begin
 inherited;
// Params.WndParent := FParentWin;
 Params.ExStyle := Params.ExStyle or WS_EX_NOACTIVATE
end;

procedure TfrmOperation.WndProc(var Msg : TMessage);
 var r : TRect;
begin
  case Msg.Msg of
    WM_STARTOPERATION : begin
      // если передан хэндл родительского окна  - центрируем
      if (Msg.WParam > 0) then
        GetWindowRect(Msg.WParam, r)
      else
        GetWindowRect(GetDesktopWindow, r);
      MoveWindow(Handle,
        round({r.Left +} ((r.Right - r.Left) / 2) - (Width / 2)),
        round({r.Top +} ((r.Bottom - r.Top) / 2) - (Height / 2)),
        Width, Height, TRUE);
      if (Msg.WParam > 0) then
      begin
        windows.SetParent(Handle, Msg.WParam);
        FParentWin := Msg.WParam;
      end;
      anAVI.Active := true;
      Show;
      SetWindowTransparent(Handle, 220);
    end;
    WM_STOPOPERATION : begin
      Hide;
      anAVI.Active := false;
      if (FParentWin <> 0) then
        SetActiveWindow(FParentWin);
    end;
    WM_SETCAPTION : pnCaption.Caption := string(PChar(Msg.WParam));
    WM_DESTROY  : begin
      inherited WndProc(Msg);
      PostQuitMessage(0);
      if (FParentWin <> 0) then
        SetActiveWindow(FParentWin);
      Exit;
    end
  end;
  inherited WndProc(Msg);
end;

procedure TfrmOperation.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  Action := caNone
end;

procedure TfrmOperation.FormCreate(Sender: TObject);
begin
  anAVI.ResName := 'CH2';
end;
end.
