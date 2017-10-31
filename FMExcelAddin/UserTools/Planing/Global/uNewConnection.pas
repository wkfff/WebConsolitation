{
  Подключение к веб-сервису.
  Модуль содержит форму подключения, а так-же здесь инкапсулирована логика
  работы с настройками плагина по части подключения.
}

unit uNewConnection;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, ComCtrls, Registry, Buttons, MSXML2_TLB, ComObj,
  PlaningProvider_TLB, uFMAddinGeneralUtils, uGlobalPlaningConst, Mask;

type
  TfrmConnection = class(TForm)
    Panel1: TPanel;
    Bevel1: TBevel;
    btnCancel: TButton;
    btnOk: TButton;
    Label7: TLabel;
    cbServiceList: TComboBox;
    edLogin: TEdit;
    cbWindows: TCheckBox;
    Label2: TLabel;
    Label3: TLabel;
    edPassword: TMaskEdit;
    procedure FormDestroy(Sender: TObject);
    procedure cbWindowsClick(Sender: TObject);
    procedure btnOkClick(Sender: TObject);
  private
    // провайдер
    FDataProvider: IPlaningProvider;
    FWinUserName: string;
    FSchemeName: widestring;
    // загрузка настроек из реестра
    procedure LoadRegistryData;
    // сохранение настроек в реестр
    procedure SaveRegistryData;
    {Попытка установить подключение с текущими параметрами}
    function TryConnect(Static: boolean; PwdHash: string): boolean;
  public
  end;

{ Результат - было ли собственно (пере-)подключение}
function Connect(Provider: IPlaningProvider; Force, Silent, Static: boolean;
  const AuthType: integer; const Login, PwdHash: string;
  out URL, SchemeName, Error: string): boolean;

implementation

uses
  uFMExcelAddInConst;

{$R *.DFM}

function Connect(Provider: IPlaningProvider; Force, Silent, Static: boolean;
  const AuthType: integer; const Login, PwdHash: string;
  out URL, SchemeName, Error: string): boolean;
var
  ConnectionForm: TfrmConnection;
begin
  result := false;
  if not Assigned(Provider) then
    exit;

  if not Force and Provider.Connected then
    exit;

  ConnectionForm := TfrmConnection.Create(nil);
  with ConnectionForm do
  begin
    {Инициализация}
    FDataProvider := Provider;
    cbServiceList.Text := '';
    cbWindows.Checked := true;
    edLogin.Text := '';
    edPassword.Text := '';
    LoadRegistryData;
    FWinUserName := GetCurrentUserName;
    if FWinUserName = '' then
    begin
      cbWindows.Checked := false;
      cbWIndows.Enabled := false;
    end;

    if Static then
    begin
      cbWindows.Checked := AuthType = 1; //atWindows
      edLogin.Text := Login;
    end;
    {Если лист открыт из задачи, то нельзя сменить подключение}
    cbServiceList.Enabled := not Static;
    edLogin.Enabled := not Static;
    edPassword.Enabled := not Static;
    cbWindows.Enabled := not Static;
    //btnCancel.Enabled := not Static;

    if not Static then
      cbWindowsClick(nil)
    else
      edPassword.Text := '********';


    {Пытаемся подключиться по-тихому, без формы}
    if not Force then
      result := TryConnect(Static, PwdHash);
    if not result then
    begin
      if Silent then
        exit;
      if (ShowModal = mrOK) or (Static and not FDataProvider.Connected) then
      begin
        Provider.Disconnect;
        result := TryConnect(Static, PwdHash);
        SaveRegistryData;
      end;
    end;
    if result then
    begin
      URL := cbServiceList.Text;
      SchemeName := FSchemeName;
    end;
    Free;
  end;
end;

function TfrmConnection.TryConnect(Static: boolean; PwdHash: string): boolean;
begin
  result := false;
  try
    result := FDataProvider.Connect(cbServiceList.Text, edLogin.Text,
      IIF(Static, PwdHash, edPassword.Text),
      IIF(cbWindows.Checked, 1, 2), FSchemeName, Static);
  except
  end;
end;

procedure TfrmConnection.LoadRegistryData;
var
  Reg: TRegistry;
  DirectPath: string;
begin
  Reg := TRegistry.Create;
  DirectPath := RegBasePath + RegConnSection + regConnWebServiceSection;
  try
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(DirectPath)) then
    begin
      Reg.OpenKey(DirectPath, false);
      cbServiceList.Items.CommaText := Reg.ReadString(regServiceListKey);
      cbServiceList.Text := Reg.ReadString(regURLKey);
      //FDatabaseName := Reg.ReadString(regWebServiceSchemeKey);
      cbWindows.Checked := Reg.ReadBool(regWindowsAuthentication);
      edLogin.Text := Reg.ReadString(regLogin);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
  except
  end;
end;

procedure TfrmConnection.SaveRegistryData;
var
  Reg: TRegistry;
  DirectPath: string;
begin
  Reg := TRegistry.Create;
  DirectPath := RegBasePath + RegConnSection + regConnWebServiceSection;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(DirectPath, true) then
    begin
      Reg.WriteString(regServiceListKey, cbServiceList.Items.CommaText);
      Reg.WriteString(regURLKey, cbServiceList.Text);
      //Reg.WriteString(regWebServiceSchemeKey, FDatabaseName);
      Reg.WriteBool(regWindowsAuthentication, cbWindows.Checked);
      Reg.WriteString(regLogin, edLogin.Text);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;


procedure TfrmConnection.FormDestroy(Sender: TObject);
begin
  FDataProvider := nil;
end;

(*function TfrmConnection.GetCurrentUserName: string;
const
  cnMaxUserNameLen = 254;
var
  sUserName: string;
  dwUserNameLen: DWORD;
begin
  result := '';
  dwUserNameLen := cnMaxUserNameLen - 1;
  SetLength(sUserName, cnMaxUserNameLen);
  GetUserName(PChar(sUserName), dwUserNameLen);
  SetLength(sUserName, dwUserNameLen);
  Result := sUserName;
end;*)

procedure TfrmConnection.cbWindowsClick(Sender: TObject);
begin
  if cbWindows.Checked then
  begin
    edLogin.Text := FWinUserName;
    edLogin.Enabled := false;
    edPassword.Enabled := false;
  end
  else
  begin
    edLogin.Enabled := true;
    edPassword.Enabled := true;
  end;
end;

procedure TfrmConnection.btnOkClick(Sender: TObject);
var
  Index: integer;
begin
  Index := cbServiceList.Items.IndexOf(cbServiceList.Text);
  if Index > -1 then
  begin
    cbServiceList.Items.Move(Index, 0);
    cbServiceList.ItemIndex := 0;
  end
  else
  begin
    cbServiceList.Items.Insert(0, cbServiceList.Text);
    if cbServiceList.Items.Count = 7 then
      cbServiceList.Items.Delete(6);
  end;
end;

end.

