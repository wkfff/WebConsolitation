unit uPropertiesForm;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, StdCtrls, OfficeXP, ExcelXP, uSheetCollector, ComObj,
  ExtCtrls, Registry, uFMExcelAddInConst, FileCtrl, PlaningTools_TLB,
  uFMAddinGeneralUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils,
  uGlobalPlaningConst, uXmlCatalog, PlaningProvider_TLB;

type
  TfrmProperties = class(TForm)
    btCancel: TButton;
    Panel2: TPanel;
    Label1: TLabel;
    Bevel21: TBevel;
    Label3: TLabel;
    Label4: TLabel;
    Panel1: TPanel;
    lExcelAddinVersion: TLabel;
    lSheetVersion: TLabel;
    stExcelAddinVersion: TStaticText;
    stSheetVersion: TStaticText;
    stDebuggingConditions: TStaticText;
    pMarkerOutdated: TPanel;
    Label6: TLabel;
    Panel3: TPanel;
    Label2: TLabel;
    Bevel1: TBevel;
    Label5: TLabel;
    Label7: TLabel;
    edPrimaryServer: TEdit;
    edPrimaryBase: TEdit;
    edSecondaryServer: TEdit;
    edSecondaryBase: TEdit;
  private
    FExcelSheet: ExcelWorksheet;
    // режим отладки версии
    function GetVersionDebugCondit: string;

    procedure Init(Catalog: TXmlCatalog);
  public
    {Выброс формы}
    procedure ShowProperties(ExcelSheet: ExcelWorkSheet; Catalog: TXMLCatalog; Provider: IPlaningProvider);
  end;

var
  frmProperties: TfrmProperties;

implementation
{$R *.DFM}


function TfrmProperties.GetVersionDebugCondit: string;
begin
  if IsTestVersion then
    result := dcvTest
  else
    result := dcvRelease;
end;

{Общие}
procedure TfrmProperties.Init(Catalog: TXmlCatalog);
begin
  stSheetVersion.Caption := GetExcelSheetVersion(FExcelSheet);
  stExcelAddinVersion.Caption := GetAddinVersion;
  if IsTestVersion then
  begin
    stDebuggingConditions.Visible := true;
    stDebuggingConditions.Caption := GetVersionDebugCondit;
  end;

  edPrimaryServer.Text := ' Нет данных, подключение не установлено';
  edPrimaryBase.Text := ' Нет данных, подключение не установлено';
  edSecondaryServer.Text := ' Нет данных, подключение не установлено';
  edSecondaryBase.Text := ' Нет данных, подключение не установлено';

  if not Catalog.Loaded then
    exit;
  edPrimaryServer.Text := ' ' + Catalog.Providers[0].Datasource;
  edPrimaryBase.Text := ' ' + Catalog.Providers[0].Catalog;
  if Catalog.InMultibaseMode then
  begin
    edSecondaryServer.Text := ' ' + Catalog.Providers[1].Datasource;
    edSecondaryBase.Text := ' ' + Catalog.Providers[1].Catalog;
  end;
end;


procedure TfrmProperties.ShowProperties(ExcelSheet: ExcelWorksheet; Catalog: TXMLCatalog;
  Provider: IPlaningProvider);
var
  NeedUpdate: boolean;
  VersionRelation: TVersionRelation;
  Op: IOperation;
begin
  if not Assigned(ExcelSheet) then
    exit;
  FExcelSheet := ExcelSheet;
  if not Catalog.Loaded then
  begin
    Op := CreateComObject(CLASS_Operation) as IOperation;
    try
      Op.StartOperation(ExcelSheet.Application.Hwnd);
      Op.Caption := pcapLoadMetadata;
      Catalog.SetUp(Provider);
    finally
      Op.StopOperation;
      Op := nil;
      Application.ProcessMessages;
    end;
  end;
  Init(Catalog);
  NeedUpdate := IsNeedUpdateSheet(ExcelSheet, VersionRelation);
  if NeedUpdate or ((not NeedUpdate) and (VersionRelation = svFuture)) then
  begin
    pMarkerOutdated.Caption := IIF(VersionRelation = svAncient,
      'Устаревшая версия листа. Выполните операцию обновления.',
      'Версия листа выше версии надстройки.');
    pMarkerOutdated.Visible := true;
  end
  else
  begin
    Panel1.Height := Panel1.Height - 17;
    Height := Height - 17;
  end;

  ShowModal;
end;

end.
