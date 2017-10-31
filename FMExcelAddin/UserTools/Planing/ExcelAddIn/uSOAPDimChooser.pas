{
  ����� ������ ��������� ����� ���-������.
  ��������� � ���� ������� �����, � ���-�� ������� ��������,
  �������������� COM-������
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
  {����� ������ ���������}
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
    {�����}
    FCatalog: TXMLCatalog;
    {SOAP- ���������}
    FDataProvider: IPlaningProvider;
    {������������ ����}
    FParentWnd: HWND;

    FURL: string;
    FSchemeName: string;

    {���������� �� ��������������, ���������������� �� ������}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

    {��������� ��� ������ �������}
    FRefreshOnShow: boolean;
    {��������� �� ������}
    FLastError: string;

    {�������������� ��������� ��� ������ ����,
     ������������ ������� �� ������������ ������������ XMLCatalog.}
    procedure SetUpCatalog(URL, SchemeName: string);
    {�������� ��������}
    procedure CreateObjects;
    {������������ ��������}
    procedure ReleaseObjects;
    {��������� ���������}
    function GetSelectedDim: string;
    procedure LookUpSelection(DimensionName: string);
  protected
    {������������ ����� � ������������ �����}
    procedure CreateParams(var Params : TCreateParams); override;
  public
    {����� ��������� �������������.
     ������� ���������� ������� ����, ��� �� ���������� �����
     ��� �� ���� "������"}
    function SelectDimension(parentWnd: integer; URL, SchemeName: string;
      var DimensionName: string): boolean;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login,
      PwdHash: WideString);
    {��������� ������ ��������� ��� ������ ������ �����}
    property RefreshOnShow: boolean read FRefreshOnShow write FRefreshOnShow;
    {��������� �� ������}
    property LastError: string read FLastError;
    {������������ ����}
    property ParentWnd: HWND read FParentWnd write FParentWnd;
  end;



  {COM-������, ����� ������� ����� �������� ������ � ����� ������ ���������}
  TSOAPDimChooser = class(TAutoObject, ISOAPDimChooser)
  private
    {�����}
    FChooser: TfrmSOAPDimChooser;
    {��������� ������}
    FLastError: WideString;
    {��������� ��� ������ ������}
    FRefreshOnShow: boolean;

    {���������� �� ��������������, ���������������� �� ������}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

  protected
    {��������� � ��������� ������������ ������}
    function Get_LastError: WideString; safecall;
    {���������� ����� ���������}
    function SelectDimension(parentWnd: SYSINT; const URL,
      SchemeName: WideString; var DimensionName: WideString): WordBool;
      safecall;

    {���� - "��������� ��� ������ ������"}
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

{������ ����� �����}
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
    result := Format('�� ������� ��������� ������ ��������� � ������ ' +
      '"%s".' +  'C����: "%s".',
      [URL, SchemeName]);
  end;
const
  PorgressCaption = '�������� ���������...';
var
  Op: IOperation;
begin
  result := false;
  FLastError := '';

  try
    Op := CreateComObject(CLASS_Operation) as IOperation;
    {��������� ������, �� ������ ��� ���������� ����������� ��������,
      ������� ���� ������� ��������� �����. ����� ���� �����������.}
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

  {���� �� ���� ������, �� ����� �������� ������}
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

  {���� �� ������� ����, "��������� ��� ������ �������" �
   ��� �������� ��������� ������, ����� ����� � ������ ������ �� ����}
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
  DimList.ToolBarVisible := false; //������ ����� �� �����
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


{����� ������ ��� COM-��������}
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
    {���� ��� ����� ����� ��� ��������� ��������� ��� ������ ������,
    ����� � ����� �������� �� �����. ������� �� �� ���������� ������.}
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
