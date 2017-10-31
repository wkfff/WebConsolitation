{
  Редактор элементов измерения (для выбора).
  Оформлен в виде обычной формы, а так-же имеется оболочка,
  представляющая COM-сервер
}
unit uSOAPDimEditor;

interface

uses
  ComObj, ActiveX, FMExcelAddin_TLB, StdVcl,
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls, AddinMembersTree, ImgList,
  uXMLCatalog, PlaningProvider_TLB, PlaningTools_TLB,
  MSXML2_TLB, uXMLUtils, uFMAddinXMLUtils, uLevelSelector, uFMExcelAddInConst,
  uGlobalPlaningConst;

const
  {Сообщения об ошибках}
  errOldValueLoadFail = 'Не удается загрузить значение измерения.';
  errGetXMLDOMFail = 'Не удается создать XMLDOM.';
  errDimNotFound = 'Измерение "%s" отсутствует в базе.';
  errBuildTreeFail = 'Не удалось построить дерево элементов.';
  warnMemberSelectionIsEmpty = 'Не выбрано ни одного элемента измерения.';

type
  {Форма редактора}
  TfrmSOAPDimEditor = class(TForm)
    pBottom: TPanel;
    btOK: TButton;
    btCancel: TButton;
    MembersTree: TAddinMembersTree;
    ilImages: TImageList;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure btOKClick(Sender: TObject);
    procedure btCancelClick(Sender: TObject);
  private
    {Схема}
    FCatalog: TXMLCatalog;
    {SOAP- Поставщик}
    FDataProvider: IPlaningProvider;
    {Сообщение об ошибке}
    FLastError: string;
    {Родительское окно}
    FParentWnd: HWND;
    {Пользователь сделал выбор?}
    FApply: boolean;

    {Информация об аутентификации, инициализируется из задачи}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

    {Создание объектов}
    procedure CreateObjects;
    {Освобождение объектов}
    procedure ReleaseObjects;

    {Принимает имя измерения и иерархии.
    Возвращает фактический объект каталога "иерархия". }
    function GetHierarchy(DimName, HierName: string): THierarchy;
    {Берет тект xml и загружает его в дом. Результат - флаг успеха}
    function GetAndLoadDOM(XMLText: WideString; out ResDOM: IXMLDOMDocument2): boolean;
    {Берет свеженькое измерение с сервера}
    function GetDimFromServer(DimName, HierName: string; Hier: THierarchy): IXMLDOMDocument2;
    {Подготавливает DOM-измерения к сохранению}
    procedure PrepareDOMforSave(DOM: IXMLDOMDocument2);
  protected
    {Поддерживаем связь с родительским окном}
    procedure CreateParams(var Params : TCreateParams); override;
  public
    {Изменение дерева
     Функция возваращет признак того, было призвдено изменение
     или же была "отмена"}
    function EditMemberTree(parentWnd: integer; URL, SchemeName: string;
      DimensionName: string; var Value: WideString): boolean;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login,
      PwdHash: WideString); 
    {Сообщение об ошибке}
    property LastError: string read FLastError;
    {Родительское окно}
    property ParentWnd: HWND read FParentWnd write FParentWnd;
  end;

  {COM-сервер, через который можно получить доступ к редактору}
  TSOAPDimEditor = class(TAutoObject, ISOAPDimEditor)
  private
    {Форма}
    FEditor: TfrmSOAPDimEditor;
    {Последняя ошибка}
    FLastError: WideString;

    {Информация об аутентификации, инициализируется из задачи}
    FTaskAuthType: integer; // 1 = atWindows, 2 = adPwdSHA512
    FTaskLogin: string;
    FTaskPwdHash: string;

  protected
    {Отредактировать измерение}
    function EditMemberTree(parentWnd: SYSINT; const URL, SchemeName,
      DimensionName: WideString; var Value: WideString): WordBool;
      safecall;
    function Get_LastError: WideString; safecall;
    {По XML-тексту измерения, выдать человеко-читаемый список мемберов}
    function GetTextMemberList(const XMLValue: WideString): WideString;
      safecall;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login,
      PwdHash: WideString); safecall;
  public
    procedure Initialize; override;
  end;

implementation
{$R *.DFM}

uses ComServ;

{Методы формы}
{Расщепляет полное имя измерения на имя самого измерения и иерархию}
procedure SplitDimensionName(FullDimName: string; out DimName: string;
  out HierName: string);
var
  DelimPos: integer;
begin
  DimName := '';
  HierName := '';

  DelimPos := Pos('.', FullDimName);
  if DelimPos > 0 then
  begin
    DimName := Copy(FullDimName, 1, DelimPos - 1);
    HierName := Copy(FullDimName, DelimPos + 1, length(FullDimName));
  end
  else
    DimName := FullDimName;
end;

{Берет тект xml и загружает его в дом. Результат - флаг успеха}
function TfrmSOAPDimEditor.GetAndLoadDOM(XMLText: WideString;
  out ResDOM: IXMLDOMDocument2): boolean;
begin
  result := false;

  if GetDOMDocument(ResDOM) then
  begin
    if ResDOM.loadXML(XMLText) then
      result := true
    else
      FLastError := errOldValueLoadFail;
  end
  else
    FLastError := errGetXMLDOMFail;
end;

{Берет свеженькое измерение с сервера}
function TfrmSOAPDimEditor.GetDimFromServer(DimName, HierName: string;
  Hier: THierarchy): IXMLDOMDocument2;
begin
  result := nil;
  try
    result := FDataProvider.GetMemberList('0', '' ,
      DimName, HierName,
      Hier.Levels.ToString,
      Hier.MemberProperties.GetCommaList);

    { проверка резуальтата на эксепт }
    if Assigned(result.selectSingleNode('Exception')) then
    begin
      FLastError := FDataProvider.LastError;
      KillDOMDocument(result);
      result := nil;
      exit;
    end;
  except
    FLastError := ermUnknown; 
  end;
end;

procedure TfrmSOAPDimEditor.CreateParams(var Params : TCreateParams);
begin
 Params.WndParent := FParentWnd;
 inherited;
end;

{Подготавливает DOM-измерения к сохранению}
procedure TfrmSOAPDimEditor.PrepareDOMforSave(DOM: IXMLDOMDocument2);
begin
  {Выкусываем выключенные элементы и обрабатываем уровни}
  SetCheckedIndication(DOM);
  MembersTree.FilterMembersDom(DOM);
  CutAllInvisible(DOM, true);
end;

{Принимает имя измерения и иерархии.
Возвращает фактический объект каталога "иерархия". }
function TfrmSOAPDimEditor.GetHierarchy(DimName, HierName: string): THierarchy;
var
  Dimension: TDimension;
begin
  result := nil;
  if Assigned(FCatalog) then
    if FCatalog.Loaded then
    begin
      Dimension := FCatalog.Dimensions.Find(DimName, FCatalog.PrimaryProvider);
      if Assigned(Dimension) then
        result := Dimension.Hierarchies.Find(HierName) as THierarchy
    end;
end;


function TfrmSOAPDimEditor.EditMemberTree(parentWnd: integer; URL, SchemeName: string;
  DimensionName: string; var Value: WideString): boolean;
var
  DOM, ValueDOM: IXMLDOMDocument2;
  DimName, HierName: string;
  Hierarchy: THierarchy;
  Op: IOperation;
  tmpSchemeName: widestring;
begin
  result := false;
  FLastError := '';
  Caption := ' ' + DimensionName;

  if Value <> '' then
    if not GetAndLoadDOM(Value, ValueDOM) then
      exit;

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

  if Assigned(FDataProvider) then
  begin
    Op.StartOperation(parentWnd);
    Op.Caption := pcapConnect;

    try
      tmpSchemeName := SchemeName;
      FDataProvider.Connect(URL, FTaskLogin, FTaskPwdHash,
        FTaskAuthType, tmpSchemeName, true);
      SchemeName := tmpSchemeName;
      
      if FDataProvider.Connected then
      begin
        FCatalog.SetUp(FDataProvider);
        if FCatalog.Loaded then
        begin
          SplitDimensionName(DimensionName, DimName, HierName);

          Hierarchy := GetHierarchy(DimName, HierName);
          if not Assigned(Hierarchy) then
          begin
            FLastError := Format(errDimNotFound, [DimensionName]);
            exit;
          end;

          Op.Caption := 'Загрузка измерения...';
          DOM := GetDimFromServer(DimName, HierName, Hierarchy);
          if not Assigned(DOM) then
            exit;

          if Assigned(ValueDOM) then
          begin
            Op.Caption := 'Синхронизация измерения...';
            CopyMembersState(ValueDOM, DOM, nil);
          end;

          Op.Caption := 'Построение дерева элементов...';
          if not MembersTree.Load(DOM, Hierarchy.Levels.ToString, Hierarchy.CodeToShow) then
            FLastError := errBuildTreeFail;
          Op.StopOperation;

          {Если не было ошибок, то можно показать форму}
          if FLastError = '' then
          begin
            FApply := false;
            ShowModal;
            if FApply then
            begin
              result := true;
              PrepareDOMforSave(DOM);
              if Assigned(DOM) then
                Value := DOM.XML;
            end;
          end;

        end;
      end
      else
        FLastError := ermNoneConnection; 
    finally
      Op.StopOperation;
      Op := nil;
      KillDOMDocument(DOM);
      KillDOMDocument(ValueDOM);
    end;
  end
  else
    FLastError := ermDataProviderUnknown;
end;


procedure TfrmSOAPDimEditor.CreateObjects;
begin
  FCatalog := TXMLCatalog.Create;
  try
    FDataProvider := CreateComObject(CLASS_PlaningProvider_) as IPlaningProvider;
  except
    FLastError := 'Не удается создать поставщика данных (IPlaningProvider)' //!!!
  end;
end;

procedure TfrmSOAPDimEditor.ReleaseObjects;
begin
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

procedure TfrmSOAPDimEditor.FormCreate(Sender: TObject);
begin
  FLastError := '';
  MembersTree.Images := ilImages;
  CreateObjects;
end;

procedure TfrmSOAPDimEditor.FormDestroy(Sender: TObject);
begin
  ReleaseObjects;
end;

procedure TfrmSOAPDimEditor.FormShow(Sender: TObject);
begin
  MembersTree.PageIndex := 0;
end;


{Ниже идут методы COM-оболочки}
function TSOAPDimEditor.EditMemberTree(parentWnd: SYSINT; const URL,
  SchemeName, DimensionName: WideString; var Value: WideString): WordBool;
var
  DimValue: WideString;
begin
  result := false;
  FLastError := '';
  DimValue := Value;
  if not Assigned(FEditor) then
  begin
    Application.Handle := parentWnd;
    FEditor := TfrmSOAPDimEditor.Create(nil);
    FEditor.ParentWnd := parentWnd;
  end;

  try
    FEditor.SetAuthenticationInfo(FTaskAuthType, FTaskLogin, FTaskPwdHash);
    result := FEditor.EditMemberTree(parentWnd, URL, SchemeName, DimensionName, DimValue);
    if result then
      Value := DimValue
    else
      if FEditor.LastError <> '' then
      begin
        result := false;
        FLastError := FEditor.LastError;
      end;
  finally
    {Форму грохнем, что бы не дрягалась}
    FreeAndNil(FEditor);
  end;
end;

function TSOAPDimEditor.Get_LastError: WideString;
begin
  result := FLastError;
end;

function TSOAPDimEditor.GetTextMemberList(
  const XMLValue: WideString): WideString;
var
  DOM: IXMLDOMDocument2;
begin
  result := '';
  if GetDomDocument(DOM) then
    if DOM.loadXML(XMLValue) then
      result := GetMembersDescriptionLikeTree(DOM); //GetMembersDescription(DOM, true);
end;

procedure TSOAPDimEditor.Initialize;
begin
  FLastError := '';
  inherited Initialize;
end;

procedure TfrmSOAPDimEditor.btOKClick(Sender: TObject);
  {Сделан нормальный выбор}
  function SelectionDone: boolean;
  var
    XPath: string;
    DOM: IXMLDOMDocument2;
  begin
    result := false;
    try
      try
        if GetDOMDocument(DOM) then
          if DOM.loadXML(MembersTree.MembersDOM.xml) then
          begin
            PrepareDOMforSave(DOM);
            XPath := 'function_result/Members//Member[@checked="true"]';
            if Assigned(DOM.selectSingleNode(XPath)) then
              result := true;
          end;
      except
      end;
    finally
      KillDomDocument(DOM);
    end;
  end;
begin
  if SelectionDone then
  begin
    FApply := true;
    Close;
  end
  else
    MessageBox(0, warnMemberSelectionIsEmpty, 'Предупреждение',
      MB_ICONWARNING or MB_OK);

end;

procedure TfrmSOAPDimEditor.btCancelClick(Sender: TObject);
begin
  FApply := false;
  Close;
end;

procedure TSOAPDimEditor.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

procedure TfrmSOAPDimEditor.SetAuthenticationInfo(AuthType: SYSINT;
  const Login, PwdHash: WideString);
begin
  FTaskAuthType := AuthType;
  FTaskLogin := Login;
  FTaskPwdHash := PwdHash;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TSOAPDimEditor, Class_SOAPDimEditor,
    ciMultiInstance, tmApartment);
end.
