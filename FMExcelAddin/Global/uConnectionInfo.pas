(*
  ------------------------------------------------------------------------------
  MDX �������, �� "���������� ������", ��� "������, 2004�.
  ------------------------------------------------------------------------------
  ���������� ����� ������� ���������� �����������.
  ����� ���� ���� ����� ������������ ��� �������� ��������� �����������
  � �������� (� ��� ����� � �� ��������� ��������������� � ����������� � ��)
  ------------------------------------------------------------------------------
*)
unit uConnectionInfo;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  fmWizardHeader, ExtCtrls, fmFlatPanel, Buttons, fmSpeedButton, StdCtrls,
  {uOptions,} brs_ADOMD_Utils, ADODB_TLB, fmComboBox, fmEdit, uMessage,
  brs_GeneralFunctions;


const
  // ���� � ����������� � ������� ����������
  RegistryPath = '\SOFTWARE\krista\FM\OLAPClient';
  // ���� � ������ �������
  ServerKey = 'Server';
  // ���� � ������ ����
  CatalogKey = 'Catalog';
  // ���� � ������ ����������
  ProviderKey = 'Provider';

type
  //�������� �����������
  TConnectionRequest=(crImmediate,crMaybeLater,crRegistry);

  TConnectionInfo = class(TForm)
    header: TfmWizardHeader;
    Bevel1: TBevel;
    pServer: TPanel;
    Label1: TLabel;
    edServerName: TEdit;
    pProvider: TPanel;
    Label3: TLabel;
    cbProvider: TComboBox;
    pBase: TPanel;
    Label2: TLabel;
    cbCatalogName: TComboBox;
    pButtons: TPanel;
    btOK: TButton;
    btCancel: TButton;
    btParams: TButton;
    procedure FormShow(Sender: TObject);
    procedure FormHide(Sender: TObject);
    procedure cbCatalogNameDropDown(Sender: TObject);
    procedure edServerNameChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure cbProviderDropDown(Sender: TObject);
    procedure btParamsClick(Sender: TObject);
  private
    { ����������? }
    FConnected: Boolean;
    { ��� ������� }
    FServerName: String;
    { ��� ���� �� �������}
    FCatalogName: String;
    { ��� ���� � ����}
    FCubeName: String;
    //��� ������� �������� ������
    FServerChanged:boolean;
    //��� ���������� �������� ������
    FProviderChanged:boolean;
    { ���� - ���������� ��������� �� �������}
    FShowErrorMessages: boolean;
    { �������� ��� ����������� ���������� ���� ��������� - �������}
    FUNameByNameStyleEnabled: boolean;

    { �������� �������� �� ������� }
    procedure LoadRegistryData;
    { ���������� �������� � �������� }
    procedure SaveRegistryData;
    { �������� ���������� }
    function TestConnection: Boolean;
    { ������ ����������� (ADO) }
    function _ConnectionString: String;

    function GetServerName: String;
    function GetCatalogName: String;
    function GetConnectionString: String;
    procedure SetConnected(Value: Boolean);
    function GetCatalogList: boolean;
    function GetProvider: string;
    procedure SetShowErrorMessages(Value: boolean);
    procedure SetUNameByNameStyleEnabled(Value: boolean);
  public
    { ����������? }
    property Connected: Boolean read FConnected write SetConnected;
    { ��� ������� }
    property ServerName: String read GetServerName;
    { ��� ���� �� ������� }
    property CatalogName: String read GetCatalogName;
    { ��� ���� � ���� }
    property CubeName: String read FCubeName write FCubeName;
    { ������ ����������� (ADO)}
    property ConnectionString: String read GetConnectionString;
    { ������������}
    procedure Connect(Request:TConnectionRequest);
    { ��������� �����������}
    procedure DisConnect;
    { ���������� ��������� �� �������}
    property ShowErrorMessages: boolean read FShowErrorMessages
      write SetShowErrorMessages default true;
    { �������� ����� ����������� ���������� ���� �������, ���� ��������� - ������}
    property UNameByNameStyleEnabled: boolean read FUNameByNameStyleEnabled
      write SetUNameByNameStyleEnabled default true;
    { ����������� }
    constructor Create(AOwner: TComponent); override;
  end;

var
  ConnectionInfo: TConnectionInfo;

implementation
uses
  Registry, ComObj, ADOMD_TLB;

{$R *.DFM}

{
  � ������������ �������������� ��������.
}
constructor TConnectionInfo.Create(AOwner: TComponent);
begin
  inherited;
  Connected := false;
  FServerName := '';
  FCatalogName := '';
  FCubeName := '';
  FserverChanged := false;
  FProviderChanged := false;
  try
    LoadRegistryData;
    FProviderChanged := true;
  except
    MessageWindow('������ ��������� ������ �� �������','',mtError,[mbOK]);
  end;
end;

{
  �������� ���������� ����������� �� �������
}
procedure TConnectionInfo.LoadRegistryData;
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
   // Reg.RootKey := HKEY_CURRENT_USER{LOCAL_MACHINE};
    Reg.RootKey := HKEY_CURRENT_USER;
    if (not Reg.KeyExists(RegistryPath)) then
    begin
      Reg.RootKey := HKEY_LOCAL_MACHINE;
      if (Reg.KeyExists(RegistryPath)) then
        CopyKeyToAnotherRoot(RegistryPath, HKEY(HKEY_LOCAL_MACHINE), HKEY(HKEY_CURRENT_USER));
    end;

    Reg.RootKey := HKEY_CURRENT_USER;

    if Reg.KeyExists(RegistryPath) then
    begin
      Reg.OpenKey(RegistryPath, False);
      FServerName := Reg.ReadString(ServerKey);
      FCatalogName := Reg.ReadString(CatalogKey);
      cbProvider.ItemIndex := cbProvider.Items.IndexOf(Reg.ReadString(ProviderKey));
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

{
  ���������� ���������� ����������� � �������
}
procedure TConnectionInfo.SaveRegistryData;
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER{LOCAL_MACHINE};
    if Reg.OpenKey(RegistryPath,true)
      then begin
        Reg.WriteString(ServerKey, FServerName);
        Reg.WriteString(CatalogKey, FCatalogName);
        Reg.WriteString(ProviderKey, cbProvider.Text);
        Reg.CloseKey;
      end;
  finally
    Reg.Free;
  end;
end;

{
  ���������� ��� �������
}
function TConnectionInfo.GetServerName: String;
begin
  if Connected then result := FServerName else result := '';
end;

{
  ���������� ��� ����
}
function TConnectionInfo.GetCatalogName: String;
begin
  if Connected then result := FCatalogName else result := '';
end;

{
  ���������� ������ �����������
}
function TConnectionInfo.GetConnectionString: String;
begin
  if Connected then result := _ConnectionString else result := '';
end;


function TConnectionInfo.GetProvider: string;
begin
  result := 'MSOLAP';
  case cbProvider.ItemIndex of
    0:
      result := 'MSOLAP.2';
    1:
      result := 'MSOLAP.3';
  end;

end;

procedure TConnectionInfo.SetUNameByNameStyleEnabled(Value: boolean);
  var cat: ICatalog;
begin
  if FUNameByNameStyleEnabled = Value then
    exit;
  FUNameByNameStyleEnabled := Value;

  cat := CreateComObject(CLASS_Catalog) as ICatalog;
  cat._Set_ActiveConnection(_ConnectionString);
  FConnected := true;
  CloseCatalog(Cat);
end;
{
  �������� ������ ����������� �� ������� ���������
}
function TConnectionInfo._ConnectionString: String;
var str: String;
begin
  str := 'Provider='+GetProvider+';Data Source=%s;';
  if FUNameByNameStyleEnabled then
    str := str + 'MDX Unique Name Style=2;';

  result := Format(str, [FServerName]);
  if FCatalogName <> '' then begin
    str := 'Initial Catalog=%s';
    result := result + Format(str, [FCatalogName]);
  end;
end;


{
  ��������� ����������� � �������� ����������
}
function TConnectionInfo.TestConnection: Boolean;
var cat: ICatalog;
begin
  try
    try
      result := true;
      cat := CreateComObject(CLASS_Catalog) as ICatalog;
      cat._Set_ActiveConnection(_ConnectionString);
      FConnected := true;
      if FCatalogName = '' then
      begin
        FConnected := false; 
        result := false;
      end;
    except
      on E: Exception do begin
        FConnected := false;
        result := false;
        if ShowErrorMessages then
          MessageWindow('�� ������� ������������ � ������� '#39+edServerName.Text +
            #39' ���� '#39+cbCatalogName.Text+#39+'.'+#13#10+
              '���������, ��������� �� ������� ��� ������� � ���� � ���������� ��� ���',
                 e.Message, mtError,[mbOK]);

 //         MessageWindow(ermConnectingFailed, e.Message, mtError,[mbOK]);
      end;
    end;
  finally
    CloseCatalog(Cat);
  end;
end;

{
  �����������.
  ��������- ����, ����� �� ������ "������" � ������������� �����
  ���������: ��� �� ������ ���
}
procedure TConnectionInfo.Connect(Request:TConnectionRequest);
begin
//  LoadRegistryData;
  if Request=crRegistry
    then TestConnection;
  while Request in [crImmediate,crMaybeLater] do//����� ������
    if ShowModal = mrOK then
    begin
      if TestConnection then
      begin
{       if HasAdminRights
        then} SaveRegistryData;
        exit;//<-- ����� ������ (������������)
      end;
    end
    else
      if Request = crMaybeLater then
        Exit;//<-- ����� ������ (��������)

   //������ �������
 { while true do begin //����� ��������� ���� ���� ������ �� ��������
    LoadRegistryData;
    if ShowModal = mrOK then begin
      if TestConnection then begin
        SaveRegistryData;
        exit; //<-- ����� ������ (������������)
      end
    end
    else
      if MayLater then exit; //<-- ����� ������ (��������)
  end;                                          }
end;

{
  ����������
}
procedure TConnectionInfo.DisConnect;
begin
  FConnected := false;
end;


{
  �����������/����������
}
procedure TConnectionInfo.SetConnected(Value: Boolean);
begin
  FConnected := false;
  if not Value then exit;
  Connect(crImmediate);
end;

{
  ���������� ������ ������� ����������
  �������������� ����
}
procedure TConnectionInfo.FormShow(Sender: TObject);
begin
  edServerName.Text := FServerName;
  cbCatalogName.Text := FCatalogName;
end;

{
  ������� ����� - ��������� ����
}
procedure TConnectionInfo.FormHide(Sender: TObject);
begin
  if ModalResult = mrOk then begin
    FServerName := edServerName.Text;
    FCatalogName := cbCatalogName.Text;
  end;
end;

//��������� ������ �������� �� �������
function TConnectionInfo.GetCatalogList: boolean;
var
  Conn:_Connection;
  s:string;
begin
  Conn := CoConnection.Create;
  s := 'Provider=' + GetProvider + ';Data Source=' + FServerName + ';';
  if FUNameByNameStyleEnabled then
    s := s + 'MDX Unique Name Style=2;';

  try
  {$WARNINGS OFF}
  Conn.Open(s,'','',adConnectUnspecified);
  {$WARNINGS ON}
  except
    on E: Exception do
    begin
      FConnected := false;
      result := false;
//      ���� ������������ ������, �� �������� �� ���� ���������
      if Conn.Errors[0].NativeError = -2147467259 then
        s := '������ OLEDB-����������'
      else
        s := Conn.Errors[0].Description;

      MessageWindow('�� ������� ������������ � ������� '#39+edServerName.Text +#39'.'+#13#10+
        '���������, ��������� �� ������� ��� � ������ ������� � ���������� ��� ���',
          s, mtError,[mbOK]);
      Conn := nil;
      Repaint;
      exit;
    end;
  end;

  GetMDStoragesList(Conn,cbCatalogName.Items);
  Result := (cbCatalogName.Items.Count > 0);
  Conn.Close;
  Conn := nil;
end;

procedure TConnectionInfo.cbCatalogNameDropDown(Sender: TObject);
begin
  if FServerChanged or FProviderChanged then
  begin
    cbCatalogName.Items.Clear;
    GetCatalogList;
    FServerChanged:=false;
    FProviderChanged:=false;
  end;
end;

procedure TConnectionInfo.edServerNameChange(Sender: TObject);
begin
  FServerChanged := true;
  FServerName := edServerName.Text;
end;

procedure TConnectionInfo.SetShowErrorMessages(Value: boolean);
begin
  FShowErrorMessages := Value;
end;

procedure TConnectionInfo.FormCreate(Sender: TObject);
begin
  if cbProvider.ItemIndex = -1 then
    cbProvider.ItemIndex := 0;
  FUNameByNameStyleEnabled := true;  
end;

procedure TConnectionInfo.cbProviderDropDown(Sender: TObject);
begin
  cbCatalogName.Items.Clear;
  FProviderChanged := true;
end;

procedure TConnectionInfo.btParamsClick(Sender: TObject);
begin
  if pProvider.Visible then
  begin
    pProvider.Visible := false;
    btParams.Caption := '��������� >>';
  end
  else
  begin
    pProvider.Visible := true;
    btParams.Caption := '��������� <<';
  end;
end;

end.
