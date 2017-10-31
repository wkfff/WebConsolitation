{
  Подключение к веб-сервису.
  Модуль содержит форму подключения, а так-же здесь инкапсулирована логика
  работы с настройками плагина по части подключения.
}

unit uConnection;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, ComCtrls, Registry, Buttons, MSXML2_TLB, ComObj,
  PlaningProvider_TLB, uFMAddinGeneralUtils, uGlobalPlaningConst;

type
  TfrmConnection = class(TForm)
    Panel1: TPanel;
    Bevel1: TBevel;
    pcPages: TPageControl;
    tsMidle: TTabSheet;
    tsWebServices: TTabSheet;
    Edit2: TEdit;
    Label4: TLabel;
    GroupBox1: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    Edit3: TEdit;
    Edit4: TEdit;
    Label7: TLabel;
    Label8: TLabel;
    cbSchemeName: TComboBox;
    btnCancel: TButton;
    btnOk: TButton;
    cbServiceList: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure cbSchemeNameDropDown(Sender: TObject);
    procedure btnCancelClick(Sender: TObject);
    procedure btnOkClick(Sender: TObject);
    procedure FormKeyPress(Sender: TObject; var Key: char);
    procedure FormDestroy(Sender: TObject);
    procedure cbSchemeNameChange(Sender: TObject);
    procedure cbServiceListChange(Sender: TObject);
  private
    { Private declarations }
    // имя сервера (URL веб страницы)
    FServerName: string;
    // имя базы данных (имя схемы)
    FDatabaseName: string;
    // список недавноиспользовавшихся адресов
    FServiceList: string;
    // модал резалт
    FApply: boolean;
    // корректное ли имя сервера
    FServerCorrect: boolean;
    // поменялось ли имя сервера
    FServerChanged: boolean;
    // провайдер
    FDataProvider: IPlaningProvider;
    // загрузка настроек из реестра
    procedure LoadRegistryData;
    // сохранение настроек в реестр
    procedure SaveRegistryData;
    // получение списка схем, доступных на сервере
    procedure GetSchemeList(ShowExcept: boolean);
  public
    { Public declarations }
    // перекрываем конструктор, в котором инициализируем поля
    constructor Create(AOwner: TComponent); override;
    // получение имени сервера (URL веб страницы)
    function GetServerName: string;
    // получение имени базы данных (имя схемы)
    function GetDatabaseName: string;
    // получение модал резалта
    function GetApply: boolean;
    // определение провайдера
    procedure SetProvider(Provider: IPlaningProvider);
    // сделать форму статической - параметры нельзя поменять
    procedure SetStatic;
  end;

implementation

uses
  uFMExcelAddInConst;

{$R *.DFM}

procedure TfrmConnection.FormCreate(Sender: TObject);
begin
  tsMidle.TabVisible := false;
  tsWebServices.TabVisible := false;
  pcPages.ActivePage := tsWebServices;
  // значения параметров по умолчанию получаем из реестра
  // если в реестре их нет, берем пустые строки
  FServerName := '';
  FDatabaseName := '';
  FServiceList := '';
  try
    LoadRegistryData;
  except
    ShowMessage(ErmRegistryFault);
  end;
  cbServiceList.Items.CommaText := FServiceList;
  cbServiceList.Text := FServerName;
  cbSchemeName.Items.Add(FDatabaseName);
  cbSchemeName.ItemIndex := 0;
  if (FServerName <> '') then
  begin
    btnOK.Enabled := true;
    FServerCorrect := true;
    FServerChanged := true;
  end;
end;


constructor TfrmConnection.Create(AOwner: TComponent);
begin
  inherited;
end;

procedure TfrmConnection.LoadRegistryData;
var
  Reg: TRegistry;
  DirectPath: string;
begin
  Reg := TRegistry.Create;
  DirectPath := RegBasePath + RegConnSection + regConnWebServiceSection;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(DirectPath)) then
    begin
      Reg.OpenKey(DirectPath, false);
      FServiceList := Reg.ReadString(regServiceListKey);
      FServerName := Reg.ReadString(regURLKey);
      FDatabaseName := Reg.ReadString(regWebServiceSchemeKey);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
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
      Reg.WriteString(regServiceListKey, FServiceList);
      Reg.WriteString(regURLKey, FServerName);
      Reg.WriteString(regWebServiceSchemeKey, FDatabaseName);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

procedure TfrmConnection.GetSchemeList(ShowExcept: boolean);
var
  SchemeList: OleVariant;
  i: integer;
begin
  FServerChanged := false;
  try
    // получаем данные - ответ на запрос
    FDataProvider.Connect(FServerName, FDatabaseName, SchemeList);
    if not (VarIsNull(SchemeList))
    then cbSchemeName.Items.Clear;
    for i := VarArrayLowBound(SchemeList, 1) to VarArrayHighBound(SchemeList, 1) do
      cbSchemeName.Items.Add(SchemeList[i]);
    if (not FDataProvider.Get_Connected) and (ShowExcept)
    then abort;
    btnOK.Enabled := true;
    FServerCorrect := true;
  except
    ShowError(ermNoneConnection);
    btnOK.Enabled := false;
    FServerCorrect := false;
  end;
end;

procedure TfrmConnection.cbSchemeNameDropDown(Sender: TObject);
begin
  if FServerChanged then
    GetSchemeList(false);
end;

function TfrmConnection.GetServerName: string;
begin
  result := FServerName;
end;

function TfrmConnection.GetDatabaseName: string;
begin
  result := FDatabaseName;
end;

function TfrmConnection.GetApply: boolean;
begin
  result := FApply;
  Application.ProcessMessages;
end;

procedure TfrmConnection.btnCancelClick(Sender: TObject);
begin
  FApply := false;
  Close;
end;

procedure TfrmConnection.btnOkClick(Sender: TObject);
var
  Index: integer;
begin
  FServerName := cbServiceList.Text;
  FDatabaseName := cbSchemeName.Text;
  GetSchemeList(true);

  if FServerCorrect then
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
      if cbServiceList.Items.Count = 5 then
        cbServiceList.Items.Delete(4);
    end;
    FServiceList := cbServiceList.Items.CommaText;
    // сохранение параметров в реестре
    SaveRegistryData;
    FApply := true;
    Close;
  end;
end;

procedure TfrmConnection.FormKeyPress(Sender: TObject; var Key: char);
begin
  if (Key = chr(27)) then
  begin
    ModalResult := mrCancel;
    FApply := false;
    Close;
  end;
end;

procedure TfrmConnection.FormDestroy(Sender: TObject);
begin
  FDataProvider := nil;
end;

procedure TfrmConnection.SetProvider(Provider: IPlaningProvider);
begin
  FDataProvider := Provider;
end;

procedure TfrmConnection.cbSchemeNameChange(Sender: TObject);
begin
  btnOk.Enabled := true;
end;

procedure TfrmConnection.cbServiceListChange(Sender: TObject);
begin
  FServerName := cbServiceList.Text;
  FServerChanged := true;
  cbSchemeName.Clear;
end;

procedure TfrmConnection.SetStatic;
begin
  btnOK.Enabled := false;
  cbServiceList.Enabled := false;
  cbSchemeName.Enabled := false;
end;

end.
