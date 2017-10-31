(*
  ------------------------------------------------------------------------------
  MDX Эксперт, АС "Финансовый Анализ", НПО "Криста, 2004г.
  ------------------------------------------------------------------------------
  Диалоговая форма запроса параметров подключения.
  Кроме того этот класс используется для хранения некоторых параметоров
  и настроек (в том числе и не связанных непосредственно с подключение к БД)
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
  // Путь к сохраненным в реестре параметрам
  RegistryPath = '\SOFTWARE\krista\FM\OLAPClient';
  // Ключ с именем сервера
  ServerKey = 'Server';
  // Ключ с именем базы
  CatalogKey = 'Catalog';
  // Ключ с именем провайдера
  ProviderKey = 'Provider';

type
  //варианты подключения
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
    { подключено? }
    FConnected: Boolean;
    { Имя сервера }
    FServerName: String;
    { Имя базы на сервере}
    FCatalogName: String;
    { Имя куба в базе}
    FCubeName: String;
    //имя сервера изменено юзером
    FServerChanged:boolean;
    //имя провайдера изменено юзером
    FProviderChanged:boolean;
    { Флаг - показывать сообщения об ошибках}
    FShowErrorMessages: boolean;
    { включить тип отображения уникальных имен элементов - именами}
    FUNameByNameStyleEnabled: boolean;

    { Загрузка настроек из реестра }
    procedure LoadRegistryData;
    { Сохранение настроек в реестере }
    procedure SaveRegistryData;
    { Проверка соединения }
    function TestConnection: Boolean;
    { Строка подключения (ADO) }
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
    { подключено? }
    property Connected: Boolean read FConnected write SetConnected;
    { Имя сервера }
    property ServerName: String read GetServerName;
    { Имя базы на сервере }
    property CatalogName: String read GetCatalogName;
    { Имя куба в базе }
    property CubeName: String read FCubeName write FCubeName;
    { Строка подключения (ADO)}
    property ConnectionString: String read GetConnectionString;
    { Подключиться}
    procedure Connect(Request:TConnectionRequest);
    { Разорвать подключение}
    procedure DisConnect;
    { Показывать сообщения об ошибках}
    property ShowErrorMessages: boolean read FShowErrorMessages
      write SetShowErrorMessages default true;
    { Включить стиль отображения уникальных имен именами, если выключено - кодами}
    property UNameByNameStyleEnabled: boolean read FUNameByNameStyleEnabled
      write SetUNameByNameStyleEnabled default true;
    { конструктор }
    constructor Create(AOwner: TComponent); override;
  end;

var
  ConnectionInfo: TConnectionInfo;

implementation
uses
  Registry, ComObj, ADOMD_TLB;

{$R *.DFM}

{
  В конструкторе инициализируем свойства.
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
    MessageWindow('Ошибка получения данных из реестра','',mtError,[mbOK]);
  end;
end;

{
  Загрузка параметров подключения из реестра
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
  Сохранение параметров подключения в реестре
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
  Возвращает имя сервера
}
function TConnectionInfo.GetServerName: String;
begin
  if Connected then result := FServerName else result := '';
end;

{
  Возвращает имя базы
}
function TConnectionInfo.GetCatalogName: String;
begin
  if Connected then result := FCatalogName else result := '';
end;

{
  Возвращает строку подключения
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
  Собираем строку подключения по текущим свойствам
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
  Проверяем подключение с текущими свойствами
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
          MessageWindow('Не удается подключиться к серверу '#39+edServerName.Text +
            #39' база '#39+cbCatalogName.Text+#39+'.'+#13#10+
              'Проверьте, правильно ли указано имя сервера и базы и попробуйте еще раз',
                 e.Message, mtError,[mbOK]);

 //         MessageWindow(ermConnectingFailed, e.Message, mtError,[mbOK]);
      end;
    end;
  finally
    CloseCatalog(Cat);
  end;
end;

{
  Подключение.
  Параметр- флаг, можно ли нажать "отмена" и подконектится потом
  Стрижаков: уже не совсем так
}
procedure TConnectionInfo.Connect(Request:TConnectionRequest);
begin
//  LoadRegistryData;
  if Request=crRegistry
    then TestConnection;
  while Request in [crImmediate,crMaybeLater] do//нужен диалог
    if ShowModal = mrOK then
    begin
      if TestConnection then
      begin
{       if HasAdminRights
        then} SaveRegistryData;
        exit;//<-- точка выхода (подключились)
      end;
    end
    else
      if Request = crMaybeLater then
        Exit;//<-- точка выхода (отложили)

   //старый вариант
 { while true do begin //будем крутиться пока чего нибудь не добьемся
    LoadRegistryData;
    if ShowModal = mrOK then begin
      if TestConnection then begin
        SaveRegistryData;
        exit; //<-- точка выхода (подключились)
      end
    end
    else
      if MayLater then exit; //<-- точка выхода (отложили)
  end;                                          }
end;

{
  Отключение
}
procedure TConnectionInfo.DisConnect;
begin
  FConnected := false;
end;


{
  Подключение/Отключение
}
procedure TConnectionInfo.SetConnected(Value: Boolean);
begin
  FConnected := false;
  if not Value then exit;
  Connect(crImmediate);
end;

{
  Показываем диалог запроса параметров
  инициализируем поля
}
procedure TConnectionInfo.FormShow(Sender: TObject);
begin
  edServerName.Text := FServerName;
  cbCatalogName.Text := FCatalogName;
end;

{
  закрыли форму - перепишем поля
}
procedure TConnectionInfo.FormHide(Sender: TObject);
begin
  if ModalResult = mrOk then begin
    FServerName := edServerName.Text;
    FCatalogName := cbCatalogName.Text;
  end;
end;

//Получение списка хранилищ на сервере
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
//      если Неопознанная ошибка, то заменяем на свое сообщение
      if Conn.Errors[0].NativeError = -2147467259 then
        s := 'Ошибка OLEDB-провайдера'
      else
        s := Conn.Errors[0].Description;

      MessageWindow('Не удается подключиться к серверу '#39+edServerName.Text +#39'.'+#13#10+
        'Проверьте, правильно ли указаны имя и версия сервера и попробуйте еще раз',
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
    btParams.Caption := 'Параметры >>';
  end
  else
  begin
    pProvider.Visible := true;
    btParams.Caption := 'Параметры <<';
  end;
end;

end.
