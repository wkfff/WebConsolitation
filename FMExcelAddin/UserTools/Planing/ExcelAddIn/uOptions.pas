unit uOptions;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, uFMAddinGeneralUtils, uSheetObjectModel, ExtCtrls, ShellApi,
  uFMAddinRegistryUtils, uGlobalPlaningConst, FileCtrl, ComCtrls;

type
  TfmOptions = class(TForm)
    cbLogEnable: TCheckBox;
    eLogDir: TEdit;
    btDirDialog: TButton;
    cbCloseProcessForm: TCheckBox;
    btnClearCache: TButton;
    btnExplorer: TButton;
    btnOK: TButton;
    btnCancel: TButton;
    Bevel1: TBevel;
    edInitialDelay: TEdit;
    Label1: TLabel;
    udInitialDelay: TUpDown;
    procedure btDirDialogClick(Sender: TObject);
    procedure btnClearCacheClick(Sender: TObject);
    procedure cbLogEnableClick(Sender: TObject);
    procedure btnExplorerClick(Sender: TObject);
  private
    { Private declarations }
    SheetInterface: TSheetInterface;
    procedure LoadOptions;
    procedure SaveOptions;
    function DirectoryFromLogExists: boolean;
  public
    { Public declarations }
  end;

function EditCommonOptions(SI: TSheetInterface): boolean;

implementation

{$R *.DFM}
var
  fmOptions: TfmOptions;

function EditCommonOptions(SI: TSheetInterface): boolean;
begin
  fmOptions := TfmOptions.Create(nil);
  with fmOptions do
  try
    SheetInterface := SI;
    LoadOptions;
    result := ShowModal = mrOK;
    if result then
      SaveOptions;
  finally
    SheetInterface := nil;
    Free;
  end;
end;

procedure TfmOptions.cbLogEnableClick(Sender: TObject);
begin
  eLogDir.Enabled := cbLogEnable.Checked;
  btDirDialog.Enabled := cbLogEnable.Checked;
  btnExplorer.Enabled := cbLogEnable.Checked;
end;

procedure TfmOptions.btDirDialogClick(Sender: TObject);
var
 sLogDir: string;
begin
  sLogDir := SelectDir(Self.Handle);
  if (sLogDir <> '') then
    eLogDir.Text := IncludeTrailingBackSlash(sLogDir);
end;

procedure TfmOptions.btnClearCacheClick(Sender: TObject);
begin
  try
    SheetInterface.DataProvider.ClearCache;
  except
    on e: Exception do
      ShowError('Ошибка при попытке очистки локального кэша. '  + e.Message);
  end;
end;


procedure TfmOptions.btnExplorerClick(Sender: TObject);
begin
  ShellExecute(Handle, nil, PChar(eLogDir.Text), nil, nil, SW_SHOWNORMAL);
end;

procedure TfmOptions.LoadOptions;
begin
  cbLogEnable.Checked := AddinLogEnable;
  eLogDir.Enabled := cbLogEnable.Checked;
  btDirDialog.Enabled := cbLogEnable.Checked;
  btnExplorer.Enabled := cbLogEnable.Checked;
  eLogDir.Text := ReadStrRegSetting(regLogPathKey, ermRegistryFault);
  cbCloseProcessForm.Checked := ReadBoolRegSetting(regCloseFormProcess, true);
  btnClearCache.Enabled := SheetInterface.DataProvider.Connected;
  udInitialDelay.Position := ReadIntegerRegSetting(regInitialDelay, 0);
end;

procedure TfmOptions.SaveOptions;
begin
  if cbLogEnable.Checked then
    if DirectoryFromLogExists then
      WriteStrRegSetting(regLogPathKey, eLogDir.Text);
  WriteBoolRegSetting(regLogEnableKey, cbLogEnable.Checked);
  WriteBoolRegSetting(regCloseFormProcess, cbCloseProcessForm.Checked);
  WriteIntegerRegSetting(regInitialDelay, udInitialDelay.Position);
end;

function TfmOptions.DirectoryFromLogExists: boolean;
begin
  eLogDir.Text := IncludeTrailingBackslash(eLogDir.Text);
  result := DirectoryExists(eLogDir.Text);
end;

end.
