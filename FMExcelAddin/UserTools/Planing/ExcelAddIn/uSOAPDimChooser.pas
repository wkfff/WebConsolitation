{
  Форма выбора измерения через веб-сервис.
  Оформлена в виде обычной формы, а так-же имеется оболочка,
  представляющая COM-сервер
}
unit uSOAPDimChooser;

interface

uses
  ComObj, ActiveX, FMExcelAddin_TLB, StdVcl,
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls, AddinDimensionsTree, ImgList, 
  uXMLCatalog, PlaningProvider_TLB, PlaningTools_TLB, ComCtrls,
  uFMExcelAddinConst, uGlobalPlaningConst;

type
  {Форма выбора измерения}
  TfrmSOAPDimChooser = class(TForm)
    pBottom: TPanel;
    btOK: TButton;
    btCancel: TButton;
    Bevel: TBevel;
    ilImages: TImageList;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
  private
    DimList: TAddinDimensionsTree;//TCommonDimTree;
    {Схема}
    FCatalog: TXMLCatalog;
    {SOAP- Поставщик}
    FDataProvider: IPlaningProvider;
    {Родительское окно}
    FParentWnd: HWND;

    FURL: string;
    FSchemeName: string;

    {Информация об аутентификации, инициализируется из задачи}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

    {Обновлять при каждом запросе}
    FRefreshOnShow: boolean;
    {Сообщение об ошибке}
    FLastError: string;

    {Инициализирует компонент для данной базы,
     Иницализация состоит из подсовывания загруженного XMLCatalog.}
    procedure SetUpCatalog(URL, SchemeName: string);
    {Создание объектов}
    procedure CreateObjects;
    {Освобождение объектов}
    procedure ReleaseObjects;
    {Выбранное измерение}
    function GetSelectedDim: string;
    procedure LookUpSelection(DimensionName: string);
  protected
    {Поддерживаем связь с родительским окном}
    procedure CreateParams(var Params : TCreateParams); override;
  public
    {Выбор измерения пользователем.
     Функция возваращет признак того, был ли произведен выбор
     или же была "отмена"}
    function SelectDimension(parentWnd: integer; URL, SchemeName: string;
      var DimensionName: string): boolean;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login,
      PwdHash: WideString);
    {Обновлять список измерений при каждом показе формы}
    property RefreshOnShow: boolean read FRefreshOnShow write FRefreshOnShow;
    {Сообщение об ошибке}
    property LastError: string read FLastError;
    {Родительское окно}
    property ParentWnd: HWND read FParentWnd write FParentWnd;
  end;



  {COM-сервер, через который можно получить доступ к форме выбора измерения}
  TSOAPDimChooser = class(TAutoObject, ISOAPDimChooser)
  private
    {Форма}
    FChooser: TfrmSOAPDimChooser;
    {Последняя ошибка}
    FLastError: WideString;
    {Обновлять при каждом показе}
    FRefreshOnShow: boolean;

    {Информация об аутентификации, инициализируется из задачи}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

  protected
    {Сообщение о последней произошедшей ошибке}
    function Get_LastError: WideString; safecall;
    {Произвести выбор измерения}
    function SelectDimension(parentWnd: SYSINT; const URL,
      SchemeName: WideString; var DimensionName: WideString): WordBool;
      safecall;

    {Флаг - "обновлять при каждом выборе"}
    function Get_RefreshOnShow: WordBool; safecall;
    procedure Set_RefreshOnShow(Value: WordBool); safecall;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login,
      PwdHash: WideString); safecall;
  public
    procedure Initialize; override;
  end;

implementation
{$R *.DFM}

uses ComServ;

{Методы самой формы}
function TfrmSOAPDimChooser.GetSelectedDim: string;
var
  CurDim: TDimension;
  CurHier: THierarchy;
begin
  result := '';
  if Assigned(DimList) then
    if not DimList.IsEmpty then
    begin
      CurDim := DimList.Dimension;
      if Assigned(CurDim) then
      begin
        result := CurDim.Name;

        CurHier := DimList.Hierarchy;
        if Assigned(CurHier) then
          if CurHier.Name <> '' then
            result := result + '.' + CurHier.Name;
      end;

    end;
end;

procedure TfrmSOAPDimChooser.CreateParams(var Params : TCreateParams);
begin
 Params.WndParent := FParentWnd;
 inherited;
end;

procedure TfrmSOAPDimChooser.LookUpSelection(DimensionName: string);
var
  DimName, HierName: string;
  Dim: TDimension;
  Hier: THierarchy;
  DelimPos: integer;
begin
  if not (Assigned(DimList) and Assigned(FCatalog)) then
    exit;
  if not FCatalog.Loaded then
    exit;

  DimName  := '';
  HierName := '';
  Hier := nil;


  DelimPos := Pos('.', DimensionName);
  if DelimPos > 0 then
  begin
    DimName := Copy(DimensionName, 1, DelimPos - 1);
    HierName := Copy(DimensionName, DelimPos + 1, length(DimensionName));
  end
  else
    DimName := DimensionName;

  Dim := FCatalog.Dimensions.Find(DimName, FCatalog.PrimaryProvider);
  if Assigned(Dim) then
  begin
    Hier := Dim.Hierarchies.Find(HierName) as THierarchy;
  end;

  DimList.SetSelection(Dim, Hier);
end;


function TfrmSOAPDimChooser.SelectDimension(parentWnd: integer;
  URL, SchemeName: string; var DimensionName: string): boolean;
  function ErrorMessage: string;
  begin
    result := Format('Не удалось загрузить список измерений с адреса ' +
      '"%s".' +  'Cхема: "%s".',
      [URL, SchemeName]);
  end;
const
  PorgressCaption = 'Загрузка измерений...';
var
  Op: IOperation;
begin
  result := false;
  FLastError := '';

  try
    Op := CreateComObject(CLASS_Operation) as IOperation;
    {Непонятно почему, но первый раз шестеренки неправильно рисуются,
      поэтому пока сделаем фиктивный вызов. Потом надо разбираться.}
    Op.StartOperation(parentWnd);
    Op.StopOperation;
    Application.ProcessMessages;
  except
    FLastError := ermIOperationNotFound;
    exit;
  end;

  try
    try
      Op.StartOperation(parentWnd);
      Op.Caption := PorgressCaption;

      SetUpCatalog(URL, SchemeName);
      LookUpSelection(DimensionName);
    except
      Op.StopOperation;
      Op := nil;
      FLastError := PChar(ErrorMessage);
      exit;
    end;

  finally
    Op.StopOperation;
    Op := nil;
  end;

  {Если не было ошибок, то можно показать список}
  if FLastError = '' then
  begin
    result := (ShowModal = mrOK);
    if result then
      DimensionName := GetSelectedDim;
  end;
end;

procedure TfrmSOAPDimChooser.SetUpCatalog(URL, SchemeName: string);
var
  tmpSchemeName: widestring;
begin
  if not (Assigned(FDataProvider) and Assigned(FCatalog)) then
    exit;

  {Если не взведен флаг, "обновлять при каждом запросе" и
   уже загружен требуемый список, тогда здесь и делать ничего не надо}
  if not RefreshOnShow and
    (FURL = URL) and (SchemeName = FSchemeName) and
    FDataProvider.Connected and FCatalog.Loaded
    and not DimList.IsEmpty then
  else
  begin
    tmpSchemeName := SchemeName;
    FDataProvider.Connect(URL, FTaskLogin, FTaskPwdHash,
      FTaskAuthType, tmpSchemeName, true);
    SchemeName := tmpSchemeName;
    //FDataProvider.Connect(URL, SchemeName, EmptyParam);
    if FDataProvider.Connected then
    begin
      FCatalog.SetUp(FDataProvider);
      DimList.Catalog := FCatalog;
      if not DimList.Load then
        raise Exception.Create('')
      else
      begin
        FURL := URL;
        FSchemeName := SchemeName;
      end;
    end
    else
      FLastError := ermConnectionFault;
  end;
end;

procedure TfrmSOAPDimChooser.CreateObjects;
begin
  FCatalog := TXMLCatalog.Create;
  try
    FDataProvider := CreateComObject(CLASS_PlaningProvider_) as IPlaningProvider;
  except
    FLastError := ermDataProviderUnknown;
  end;
end;

procedure TfrmSOAPDimChooser.ReleaseObjects;
begin
  if Assigned(DimList) then
  begin
    DimList.Clear;
    FreeAndNil(DimList);
  end;

  if Assigned(FCatalog) then
  begin
    FCatalog.Clear;
    FreeAndNil(FCatalog);
  end;

  if Assigned(FDataProvider) then
  begin
    FDataProvider.Disconnect;
    FDataProvider.FreeProvider;
    FDataProvider := nil;
  end;
end;

procedure TfrmSOAPDimChooser.FormCreate(Sender: TObject);
begin
  FRefreshOnShow := false;
  FLastError := '';
  
  DimList := TAddinDimensionsTree.Create(self);
  DimList.ToolBarVisible := false; //кнопки здесь не нужны
  DimList.Parent := self;
  DimList.Align := alClient;


  DimList.Images := ilImages;
  DimList.ImageIndexes[ntCube] := 0;
  DimList.ImageIndexes[ntDimension] := 1;
  DimList.ImageIndexes[ntHierarchy] := 2;
  CreateObjects;
end;

procedure TfrmSOAPDimChooser.FormDestroy(Sender: TObject);
begin
  ReleaseObjects;
end;


{Пошли методы для COM-оболочки}
function TSOAPDimChooser.Get_LastError: WideString;
begin
  result := FLastError;
end;

function TSOAPDimChooser.Get_RefreshOnShow: WordBool;
begin
  if Assigned(FChooser) then
    result := FChooser.RefreshOnShow
  else
    result := FRefreshOnShow;
end;

procedure TSOAPDimChooser.Set_RefreshOnShow(Value: WordBool);
begin
  FRefreshOnShow := Value;
  if Assigned(FChooser) then
    FChooser.RefreshOnShow := Value;
end;


function TSOAPDimChooser.SelectDimension(parentWnd: SYSINT; const URL,
  SchemeName: WideString; var DimensionName: WideString): WordBool;
var
  DimName: string;
begin
  result := true;
  FLastError := '';
  DimName := DimensionName;
  if not Assigned(FChooser) then
  begin
    Application.Handle := parentWnd;
    FChooser := TfrmSOAPDimChooser.Create(nil);
    FChooser.ParentWnd := parentWnd;
    FChooser.RefreshOnShow := FRefreshOnShow;
  end;

  try
    FChooser.SetAuthenticationInfo(FTaskAuthType, FTaskLogin, FTaskPwdHash);
    result := FChooser.SelectDimension(parentWnd, URL, SchemeName, DimName);
    if result then
      DimensionName := DimName
    else
      if FChooser.LastError <> '' then
      begin
        result := false;
        FLastError := FChooser.LastError;
      end;
  finally
    {Если все равно нужно все полностью обновлять при каждом показе,
    тогда и форму деражать не будем. Грохнем ее до сделующего вызова.}
    if FRefreshOnShow then
      FreeAndNil(FChooser);
  end;
end;

procedure TSOAPDimChooser.Initialize;
begin
  FLastError := '';
  FRefreshOnShow := false;
  inherited Initialize;
end;

procedure TSOAPDimChooser.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

procedure TfrmSOAPDimChooser.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TSOAPDimChooser, Class_SOAPDimChooser,
    ciMultiInstance, tmApartment);
end.
