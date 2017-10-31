{
  Реализация логического уровня корневого объекта модели листа.
  Под логическим уровнем подразумеваются такие функции объекта как
  сохранение, воосстановление, выборка и обработка данных на уровне XML,
  управление коллекциями и подобные.
  Класс TSheetLogic по прежнему остался абстрактным и не может непосредственно
  использоваться в плагине. Использоваться могут только его потомки.
}

unit uSheetLogic;

interface
uses
  Windows, Classes, SysUtils, MSXML2_TLB, uXMLUtils,
  OfficeXP, ExcelXP, PlaningProvider_TLB,
  uXMLCatalog, uFMExcelAddinConst, PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils, uSheetAxes,
  uSheetObjectModel, uSheetBreaks, uSheetTotals, uSheetFilters, uSheetMDXQueries,
  uSheetHistory, uSheetSingleCells, uSheetParam, uSheetConst, uOfficeUtils,
  uExcelUtils, uGlobalPlaningConst, uEvents;

type

  //класс - оболочка на DOM с метаданными листа
  TSheetLogic = class(TSheetInterface)
  private
    //коллекции
    FTotals: TSheetTotalCollection;
    FRows: TSheetAxisCollection;
    FColumns: TSheetAxisCollection;
    FFilters: TSheetFilterCollection;
    FSingleCells: TSheetSingleCellCollection;
    FConsts: TConstCollection;
    FParams: TParamCollection;
    //счетчик для генерации UniqueID
    FCounter: int64;
    {счетчик для генерации ParamID}
    FPidCounter: integer;
    {форма индикации}
    FOperation: IOperation;
    //версия плагина, зашитая в лист
    FSheetVersion: string;
    {резервная копия метаданных}
    FBackupXml: IXMLDOMDocument2;
    FNeedMapSingleCells: boolean;
    FEvents: TExcelEvents;
    {В версии 2.3.3 введен новый механизм сбора данных на основе хранимой в СР
      информации о разметке взамен разметки именованными диапазонами. Данный
      признак показывает, можно ли использовать новый механизм или
      задействовать старый.}
    FIsMarkupNew: boolean;
(*    //максимальное количество шагов отката
    FMaxUndoSteps: integer;
    //счетчик отката, указывает на последний бэкап
    FUndoCounter: integer;
    //массив имен забэкапленных листов, его размер = FMaxUndoSteps
    FSheetNames: array of string;   *)
    //загружает вспомогательную информацию
    //загружает значения разделителей разрядов и дробной части чисел
    procedure LoadSeparators;
    procedure ProcessMdxQuery(Query: TSheetMDXQuery; var DataDom: IXMLDOMDocument2);
  protected
    // данные отдельных ячеек
    FSingleCellsData: IXMLDOMDocument2;
    // данные FormulaArrays
    FFormulaArrays: IXMLDOMDocument2;
    // данные свободных и результатов, укладывающихся в ось строк
    FFreeTotalsData: IXMLDOMDocument2;
    // данные свободных и результатов, игнорирующих ось строк
    FFreeTotalsDataIgnored: IXMLDOMDocument2;

    //создает заготовку для метаданных
    function NewMetaDataDOM: IXMLDOMDocument2;

    procedure SetDataDom(var DestDom, Value: IXMLDOMDocument2);
    procedure SetTotalsData(Value: IXMLDOMDocument2); override;
    procedure SetFreeTotalsData(Value: IXMLDOMDocument2);
    procedure SetFreeTotalsDataIgnored(Value: IXMLDOMDocument2);
    procedure SetFormulaArrays(Value: IXMLDOMDocument2);
    procedure SetSingleCellsData(Value: IXMLDOMDocument2);
    function GetSheetVersion: string; override;
    function GetEmpty: boolean;

    function Get_Totals: TSheetTotalCollectionInterface; override;
    function Get_Rows: TSheetAxisCollectionInterface; override;
    function Get_Columns: TSheetAxisCollectionInterface; override;
    function Get_Filters: TSheetFilterCollectionInterface; override;
    function Get_SingleCells: TSheetSingleCellCollectionInterface; override;
    function Get_Consts: TConstCollectionInterface; override;
    function Get_Params: TParamCollectionInterface; override;

    {генерит новый идентификатор параметра}
    function GetParamID: integer; override;
    {Возвращает список алиасов листа.
     Какие типы элементов включать задется параметром}
    function GetElementAliasesList(ATypes: TSheetObjectTypes): TStringList;
    procedure LoadInnerData(Node: IXMLDOMNode);
    procedure SaveInnerData(Node: IXMLDOMNode);
    procedure ClearInnerData;
    procedure GetOffsets; virtual; abstract;

  public
    constructor Create;
    destructor Destroy; override;
    {загружает данные с листа}
    function Load(ExcelSheet: ExcelWorksheet; Context: TTaskContext;
      LoadMode: TLoadMode): boolean;
    procedure LoadFromXml(MetaDataDom: IXMLDOMDocument2);
    procedure LoadConsts;
    {собирает и записывает метаданные в лист}
    procedure Save; override;
    procedure SaveConsts;
    procedure ExportXml(var Dom: IXMLDOMDocument2); override;
    {очищает коллекции}
    procedure Clear; virtual;
    {генерит уникальный идентификатор элемента - генерация сквозная для всех}
    function GetUniqueID: string; override;
    {заполняет данные коллекции показателей}
    procedure QueryTotalsData;
    {запрос для режима одного показателя}
    procedure QueryOneTotalData(Total: TSheetTotalInterface);
    {Кол-во фильтров определенной области видимости}
    function GetFilterCountWithScope(IsPartial: boolean): integer; override;
    {Настраивает разрезность показателей-мер.
     Т.е на основании структуры оси столбцов, определяет, может ли
     показатель размещаться по столбцам.
     Фактически, выставляет показателю свойство - IsIgnoredColumnAxis}
    procedure SetUpMeasuresPosition; override;
    {Очистка мусора в СР - удаляет записи, с которыми не сопоставлены объекты}
    procedure ClearCPGarbage(GarbageOnly: boolean);
    {проверяет содержимое на соответствие XMLCatalog-у}
    function Validate: boolean; overload;
    function Validate(Total: TSheetTotalInterface): boolean; overload;
    {обновляет метаданные и измерения
    если Force = true, то обновляет обязательно, иначе - если данные изменились}
    function Refresh(Force: boolean): boolean;
    {перемещает элемент из одной области листа(строки/столбцы/фильтры) в другую}
    function MoveElement(ObjType, NewObjType, UID: string): boolean; override;
    {проверяет, заюзано ли уже измерение в листе}
    function IsDimensionUsed(DimName, HierName: string): TSheetObjectType; override;
    {при проверке на заюзаность, не учитываем сам проверяемый элемент}
    function IsDimensionUsedEX(SheetElement: TSheetDimension): TSheetObjectType;
    function GetDimension(DimensionName: widestring): TSheetDimension; override;
    {сохраняет резервную копию метаданных}
    procedure Backup;
    {восстанавливает метаданные из резервной копии}
    procedure Restore;
(*    {делает копию активного листа для поддержки операций Undo/Redo}
    procedure MakeBackup;
    procedure Undo;   *)
    {данные свободных/результатов в разрезе оси столбцов}

    {определяет "раскладываемость" показателей по каждому элементу оси.
    List1 содержит раскладываемые показатели, List2 - нераскладываемые.
    Порядковый номер строки в обоих списках соответствует индексу элемента оси.
    Строка GrandTotalsOnly содержит показатели, вообще игнорирующие ось строк и
    выводимые только в главный итог.}
    procedure GetFactorization(Axis: TSheetAxisCollectionInterface;
      FromBaseOnly: boolean;
      out List1, List2: TStringList; out GrandTotalsOnly: string); override;
    {Ишет требуемое измерение во всех коллекциях - строках, столбцах, фильтрах и
      фильтрах отдельных показателей}
    function FindDimensionByUniqueId(UniqueId: string): TSheetDimension; override;

    property FreeTotalsData: IXMLDOMDocument2 read FFreeTotalsData write SetFreeTotalsData;
    {данные свободных/результатов за осью столбцов}
    property FreeTotalsDataIgnored: IXMLDOMDocument2 read FFreeTotalsDataIgnored write SetFreeTotalsDataIgnored;
    {данные FormulaArray}
    property FormulaArrays: IXMLDOMDocument2 read FFormulaArrays write SetFormulaArrays;
    {данные отдельных ячеек}
    property SingleCellsData: IXMLDOMDocument2 read FSingleCellsData write SetSingleCellsData;
    {метадата пуста, если пусты все коллекции, т.е. на листе ничего нет}
    property Empty: boolean read GetEmpty;
    property NeedMapSingleCells: boolean read FNeedMapSingleCells write FNeedMapSingleCells;
    {коллекция событий}
    property Events: TExcelEvents read FEvents write FEvents;
    // проверка текущего адреса книги с оригинальным
    (* Реформа по FMQ 8192 - обратная запись без контекста задач
    procedure CheckPath;*)
    // событие - прикрепление, открепление от задачи
    procedure OnTaskConnection(IsConnected: boolean);
    procedure OpenOperationEx(Hwnd: integer; Msg: string; IsCritical,
      IsTimed: boolean; OperationType: integer);
    procedure CloseOperationEx(IsSuccess: boolean; ErrorMsg: string);
    procedure StartOperation(Hwnd: integer; Msg: string);
    procedure StopOperation;
    {удаляет из модели показатели и отдельные, построенные на основе указанной константы}
    procedure DeleteConstElements(Constant: TConstInterface); override;

    {Признак, отвечающий за модель механизма разметки для сбора и ОЗ.
      Старый - основан на именованных диапазонов экселя.
      Новый - на хранении разметочной информации в СР листа.
      Смена значения данного признака возможна только один раз и в одном
      направлении - при операции обновления листа предыдущей версии.}
    property IsMarkupNew: boolean read FIsMarkupNew write FIsMarkupNew;
  end;

implementation

uses
  uConverter;


{************ TSheetLogic implementaion *************}

constructor TSheetLogic.Create;
begin
  {Коллекции}
  FTotals := TSheetTotalCollection.Create(Self);
  FTotals.ObjectType := wsoTotal;
  FRows := TSheetAxisCollection.Create(Self);
  FRows.AxisType := axRow;
  FRows.ObjectType := wsoRow;
  FColumns := TSheetAxisCollection.Create(Self);
  FColumns.AxisType := axColumn;
  FColumns.ObjectType := wsoColumn;
  FFilters := TSheetFilterCollection.Create(Self);
  FFilters.ObjectType := wsoFilter;
  FBreaks := TSheetBreakCollection.Create;
  FSingleCells := TSheetSingleCellCollection.Create(Self);
  FSingleCells.ObjectType := wsoSingleCell;
  FParams := TParamCollection.Create(Self);
  FConsts := TConstCollection.Create(Self);
  FIsLock := false;
(*  FMaxUndoSteps := 5;
  FUndoCounter := -1;
  SetLength(FSheetNames, FMaxUndoSteps);*)

  FWritablesInfo := TWritablesInfo.Create;
  FInConstructionMode := true;
  FOnline := true;
  FTotalSections := TStringList.Create;
  SpecialFlagForTaskParamCopy := false;
  FIsMarkupNew := true;
  FKillZeroLinkedParams := true;
end;

destructor TSheetLogic.Destroy;
begin
  Clear;
  FTotals.Free;
  FRows.Free;
  FColumns.Free;
  Filters.Free;
  FBreaks.Free;
  FSingleCells.Free;
  FParams.Destroy;
  FConsts.Destroy;
  FProcess := nil;
  FTaskContext := nil;
  KillDomDocument(FBackupXml);
  FWritablesInfo.Free;
  FreeStringList(FTotalSections);
end;

procedure TSheetLogic.LoadInnerData(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  FCounter := GetIntAttr(Node, attrCounter, 0);
  FPidCounter := GetIntAttr(Node, attrPidCounter, 0);

  {Свойства листа - итоги }
  FPrintableStyle := GetBoolAttr(Node, attrPrintableStyle, false);
  FTotalMultiplier := TTotalMultiplier(GetIntAttr(Node, attrTotalMultiplier, 0));
  FMarkerPosition := TMarkerPosition(GetIntAttr(Node, attrMarkerPosition, 0));
  FFilterCellsLength := GetIntAttr(Node, attrFilterCellsLength, 7);
  FIsMergeFilterCellsByTable := GetBoolAttr(Node, attrIsMergeFilterCellsByTable, false);
  FIsDisplaySheetInfo := GetBoolAttr(Node, attrDisplaySheetInfo, true);
  FIsDisplayCommentStructuralCell := GetBoolAttr(Node, attrDisplayCommentStructuralCell,
    true);
  FIsDisplayCommentDataCell := GetBoolAttr(Node, attrDisplayCommentDataCell, false);
  FSheetVersion := GetStrAttr(Node, attrSheetVersion, '');
  FIsDisplayFilters := GetBoolAttr(Node, attrDisplayFilters, true);
  FIsDisplayColumnsTitles := GetBoolAttr(Node, attrDisplayColumnsTitles, true);
  FIsDisplayRowsTitles := GetBoolAttr(Node, attrDisplayRowsTitles, true);
  FIsDisplayTotalsTitles := GetBoolAttr(Node, attrDisplayTotalsTitles, true);
  FIsDisplayColumns := GetBoolAttr(Node, attrDisplayColumns, true);
  FDisplayFullFilterText := GetBoolAttr(Node, attrDisplayFullFilterText, false);
  FURL := GetStrAttr(Node, attrURL, '');
  FSchemeName := GetStrAttr(Node, attrSchemeName, '');
  FLastRefreshDate := GetStrAttr(Node, attrLastRefreshDate, fmNoRefreshDate);
  FNeedRound := GetBoolAttr(Node, attrNeedRound, false);
  FTableProcessingMode := TTableProcessingMode(GetIntAttr(Node, attrTableProcessingMode, 0));
  FAllowNECJ := GetBoolAttr(Node, attrAllowNECJ, true);
  {заголовок листа}
  with FSheetHeading do
  begin
    Type_ := THeadingType(GetIntAttr(Node, attrHeadingtype, 0));
    End_ := TPSObject(GetIntAttr(Node, attrHeadingEnd, 0));
    Address := GetStrAttr(Node, attrHeadingAddress, '');
  end;

  WritablesInfo.ReadFromXml(Node);

  FInConstructionMode := GetBoolAttr(Node, attrInConstructionMode, true);
  FPermitEditing := GetBoolAttr(Node, attrPermitEditing, false);
  FOnline := GetBoolAttr(Node, 'online', true);
  FTotalSections.CommaText := GetStrAttr(Node, attrTotalSections, '');

  FIsMarkupNew := GetBoolAttr(Node, attrIsMarkupNew, false);
  FTaskId := GetStrAttr(Node, 'taskid', '');

(*  FMaxUndoSteps := GetIntAttr(Node, 'maxundosteps', 5);
  FUndoCounter := GetIntAttr(Node, 'undocounter', -1);
  tmpNode := Node.selectSingleNode('./sheetnames');
  if Assigned(tmpNode) then
    for i := 0 to tmpNode.childNodes.length - 1 do
      FSheetNames[i] := GetNodeStrAttr(tmpNode, 'undostep');*)
end;

procedure TSheetLogic.SaveInnerData(Node: IXMLDOMNode);

  procedure SyncURL;
  begin
    if not FDataProvider.Connected then
      exit;
    if (FDataProvider.URL = '') or (FDataProvider.Scheme = '') then
      exit;
    if (FDataProvider.URL + FDataProvider.Scheme) <> (URL + SchemeName) then
    begin
      URL := FDataProvider.URL;
      SchemeName := FDataProvider.Scheme;
    end;
  end;

(*var
  ProvidersNode: IXMLDOMNode;*)
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrCounter, IntToStr(FCounter));
    setAttribute(attrPidCounter, IntToStr(FPidCounter));

    {Свойства листа - итоги}
    setAttribute(attrPrintableStyle, BoolToStr(FPrintableStyle));
    setAttribute(attrTotalMultiplier, Ord(TotalMultiplier));
    setAttribute(attrMarkerPosition, Ord(MarkerPosition));
    setAttribute(attrFilterCellsLength, IntToStr(FFilterCellsLength));
    setAttribute(attrIsMergeFilterCellsByTable, BoolToStr(FIsMergeFilterCellsByTable));
    setAttribute(attrDisplaySheetInfo, BoolToStr(FIsDisplaySheetInfo));
    setAttribute(attrDisplayCommentStructuralCell,
      BoolToStr(FIsDisplayCommentStructuralCell));
    setAttribute(attrDisplayCommentDataCell, BoolToStr(FIsDisplayCommentDataCell));
    if FSheetVersion = '' then
      FSheetVersion := GetAddinVersion;
    setAttribute(attrSheetVersion, FSheetVersion);
    setAttribute(attrDisplayFilters, BoolToStr(FIsDisplayFilters));
    setAttribute(attrDisplayColumnsTitles, BoolToStr(FIsDisplayColumnsTitles));
    setAttribute(attrDisplayRowsTitles, BoolToStr(FIsDisplayRowsTitles));
    setAttribute(attrDisplayTotalsTitles, BoolToStr(FIsDisplayTotalsTitles));
    setAttribute(attrDisplayColumns, BoolToStr(FIsDisplayColumns));
    setAttribute(attrDisplayFullFilterText, BoolToStr(FDisplayFullFilterText));
    SyncURL;
    SetAttribute(attrLastRefreshDate, FLastRefreshDate);
    setAttribute(attrURL, FURL);
    setAttribute(attrSchemeName, FSchemeName);
    setAttribute(attrNeedRound, BoolToStr(FNeedRound));
    setAttribute(attrTableProcessingMode, Ord(FTableProcessingMode));
    setAttribute(attrAllowNECJ, BoolToStr(FAllowNECJ));
    setAttribute(attrHeadingType, Ord(FSheetHeading.Type_));
    setAttribute(attrHeadingEnd, Ord(FSheetHeading.End_));
    setAttribute(attrHeadingAddress, FSheetHeading.Address);
    setAttribute(attrInConstructionMode, BoolToStr(FInConstructionMode));
    setAttribute(attrPermitEditing, BoolToStr(PermitEditing));
    setAttribute('online', BoolToStr(Online));

    WritablesInfo.WriteToXml(Node);
    setAttribute(attrTotalSections, FTotalSections.CommaText);

    setAttribute(attrIsMarkupNew, BoolToStr(IsMarkupNew));
    setAttribute('taskid', TaskId);

    (*if XMLCatalog.Loaded then
    begin
      ProvidersNode := Node.ownerDocument.createNode(1, 'Providers', '');
      XMLCatalog.Providers.WriteToXml(ProvidersNode);
      Node.appendChild(ProvidersNode);
    end; *)

(*
    setAttribute('maxundosteps', FMaxUndoSteps);
    setAttribute('undocounter', FUndoCounter);
    tmpNode := Node.ownerDocument.createNode(1, 'sheetnames', '');
    Node.appendChild(tmpNode);
    for i := 0 to FUndoCounter do
      SetNodeStrAttr(tmpNode, 'undostep', FSheetNames[i]);*)
  end;
end;

procedure TSheetLogic.ClearInnerData;
begin
  FPrintableStyle := false;
  FMarkerPosition := mpLeft;
  FFilterCellsLength := 7;
  FIsMergeFilterCellsByTable := false;
  FIsDisplaySheetInfo := true;
  FIsDisplayCommentStructuralCell := true;
  FIsDisplayCommentDataCell := false;
  FIsDisplayFilters := true;
  FIsDisplayColumnsTitles := true;
  FIsDisplayRowsTitles := true;
  FIsDisplayTotalsTitles := true;
  FIsDisplayColumns := true;
  FDisplayFullFilterText := false;
  FLastRefreshDate := fmNoRefreshDate;
  FURL := '';
  FSchemeName := '';
  FSheetVersion := '';
  FNeedRound := false;
  FTableProcessingMode := tpmNormal;
  FAllowNECJ := true;
  FTotalMultiplier := tmE1;
  with FSheetHeading do
  begin
    Type_ := htNoDefine;
    End_ := psoNone;
    Address := '';
  end;
  WritablesInfo.Clear;
end;

procedure TSheetLogic.SetDataDom(var DestDom, Value: IXMLDOMDocument2);
begin
  if not Assigned(Value) then
  begin
    KillDOMDocument(DestDom);
    exit;
  end;
  if not Assigned(DestDom) then
    GetDOMDocument(DestDom);
  DestDom.loadXML(Value.XML);
end;

procedure TSheetLogic.SetTotalsData(Value: IXMLDOMDocument2);
begin
  SetDataDom(FTotalsData, Value);
end;

procedure TSheetLogic.SetFreeTotalsData(Value: IXMLDOMDocument2);
begin
  SetDataDom(FFreeTotalsData, Value);
end;

procedure TSheetLogic.SetFreeTotalsDataIgnored(Value: IXMLDOMDocument2);
begin
  SetDataDom(FFreeTotalsDataIgnored, Value);
end;

procedure TSheetLogic.SetFormulaArrays(Value: IXMLDOMDocument2);
begin
  SetDataDom(FFormulaArrays, Value);
end;

procedure TSheetLogic.SetSingleCellsData(Value: IXMLDOMDocument2);
begin
  SetDataDom(FSingleCellsData, Value);
end;

procedure TSheetLogic.StartOperation(Hwnd: integer; Msg: string);
begin
  if not Assigned(FOperation) then
    FOperation := CoOperation.Create;
  FOperation.StartOperation(Hwnd);
  FOperation.Caption := Msg;
end;

procedure TSheetLogic.StopOperation;
begin
  if Assigned(FOperation) then
  begin
    FOperation.StopOperation;
    FOperation := nil;
  end;
end;

procedure TSheetLogic.OpenOperationEx(Hwnd: integer; Msg: string; IsCritical,
  IsTimed: boolean; OperationType: integer);
begin
  if IsProcessShowing then
    OpenOperation(Msg, IsCritical, IsTimed, OperationType)
  else
  begin                  
    FOperation := CoOperation.Create;
    FOperation.StartOperation(Hwnd);
    FOperation.Caption := Msg;
  end;
end;

procedure TSheetLogic.CloseOperationEx(IsSuccess: boolean; ErrorMsg: string);
begin
  if IsProcessShowing then
  begin
    if IsSuccess then
      CloseOperation
    else
      PostMessage(ErrorMsg, msgError);
  end
  else
    if Assigned(FOperation) then
    begin
      FOperation.StopOperation;
      FOperation := nil;
      if not IsSuccess then
        ShowError(ErrorMsg);
    end;
end;

function TSheetLogic.Load(ExcelSheet: ExcelWorksheet; Context: TTaskContext; LoadMode: TLoadMode): boolean;

  function DrawUpQuestion: string;
  begin
    result := 'Версия листа устарела (' + GetExcelSheetVersion(ExcelSheet)
      + '). Обновить до текущей версии надстройки (' + GetAddinVersion + ') ?';
  end;

var
  MetaDataDOM: IXMLDOMDocument2;
  OldSheetVersion: string;
  IsSuccessUpdate: boolean;
begin
  result := false;
  if not Assigned(ExcelSheet) then
    exit;
  if IsLock then
    exit;
  FExcelSheet := ExcelSheet;
  TaskContext := Context;
  (* Реформа по FMQ 8192 - обратная запись без контекста задач
  if (LoadMode <> [lmInner]) then
    CheckPath;*)
  loadSeparators;
  FLoadMode := LoadMode;
  MetaDataDOM := GetDataFromCP(FExcelSheet, cpMDName);
  if not Assigned(MetaDataDOM) then
  begin
    result := true;
    (*IsLock := true;*)
    FOnline := true;
    { Перешли на чистый лист в книге, в которой уже есть константы,
      а метаданных-то в нем и нет. Так коллекцию констант подгрузить
      всё же надо, иначе может случиться задача 4435.}
    if (LoadMode <> [lmInner]) then
      LoadConsts;
    exit;
  end;
  {GetDataFromCP безусловно создает ДОМ при наличии искомого СР. При этом возможна
    неприятная ситуация, что в результате какой-то ошибки документ не загрузился.}
  if not Assigned(MetaDataDOM.documentElement) then
  begin
    result := false;
    exit;
  end;
  if (not IsTaskContextLoad) and (LoadMode <> [lmInner]) and Assigned(DataProvider) then
  begin
    OldSheetVersion := GetExcelSheetVersion(ExcelSheet);
    IsSuccessUpdate := CheckSheetVersion(ExcelSheet, DataProvider, false, FProcess);

    if IsSuccessUpdate then
      MetaDataDOM := GetDataFromCP(ExcelSheet, cpMDName)
    else
      exit;
  end;
  LoadFromXml(MetaDataDom);
  KillDOMDocument(MetaDataDOM);

  GetOffsets;

  KillDOMDocument(FFreeTotalsData);
  if (lmFreeData in LoadMode) then
  begin
    {На защищеном листе будут большие проблемы со сбором, если в экселе
      установлена галка "скрыть формулы"}
    if IsSheetProtected(ExcelSheet) then
      SetSheetProtection(ExcelSheet, false);
    CollectTotalsData(false);
  end;

  { получаем формат показателей и отдельных ячеек}
  if (lmCollections in LoadMode) then
  begin
    Totals.GetNumberFormats;
    SingleCells.GetNumberFormats;
  end;

  FBreaks.Load(FExcelSheet);
  {Константы необходимо грузить после показателей - чтобы сработала синхронизация}
  if (LoadMode <> [lmInner]) then
    LoadConsts;
  result := true;
  IsLock := true;
end;

procedure TSheetLogic.Save;

  procedure SaveCollection(DOM: IXMLDOMDocument2; Collection: TSheetCollection);
  var
    Node: IXMLDOMNode;
    CollectionName: string;
  begin
    CollectionName := Collection.GetCollectionName;
    Node := DOM.createNode(1, CollectionName, '');
    Collection.WriteToXML(Node);
    DOM.documentElement.appendChild(Node);
  end;

  procedure InitializeSheetType;
  var
    CurType: string;
  begin
    if not IsPlaningSheet(ExcelSheet) then
    try
      CurType := GetWBCustomPropertyValue(ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType);
      if CurType = '' then
        SetWBCustomPropertyValue(ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType, '0');
    except
    end;
  end;

var
  MetaDataDOM: IXMLDOMDocument2;
  InnerDataNode: IXMLDOMNode;
begin
  if FLoadMode <> [lmInner] then
  begin
    MetaDataDOM := NewMetaDataDOM;
    InnerDataNode := MetaDataDOM.createNode(1, 'innerdata', '');
  end
  else
  begin
    MetaDataDOM := GetDataFromCP(FExcelSheet, cpMDName);
    if Assigned(MetaDataDOM) then
      InnerDataNode := MetaDataDOM.selectSingleNode(xpInnerData)
    else
    begin
      MetaDataDOM := NewMetaDataDOM;
      InnerDataNode := MetaDataDOM.createNode(1, 'innerdata', '');
    end;
  end;
  {сохраним в XML вспомогательную информацию}
  MetaDataDOM.documentElement.appendChild(InnerDataNode);
  SaveInnerData(InnerDataNode);

  if (FLoadMode <> [lmInner]) then
  begin
    //удаление XML-ок мемберов
    ClearCPGarbage(false);
    {сохраним коллекции в XML}
    SaveCollection(MetaDataDOM, FTotals);
    SaveCollection(MetaDataDOM, FRows);
    SaveCollection(MetaDataDOM, FColumns);
    SaveCollection(MetaDataDOM, FFilters);
    SaveCollection(MetaDataDOM, FSingleCells);
    SaveCollection(MetaDataDOM, FParams);
    SaveConsts;
  end;
  {если тип листа не установлен, то выставим значение "расчетный лист"}
  InitializeSheetType;
  {запишем XML в СР}
  GetCPByName(ExcelSheet, cpMDName, true).Value := MetaDataDOM.xml;

  if AddinLogEnable and (FLoadMode <> [lmInner])then
    WriteDocumentLog(MetaDataDOM, 'sheetmetadata.xml');

  KillDOMDocument(MetaDataDOM);
end;

procedure TSheetLogic.ExportXml(var Dom: IXMLDOMDocument2);

  procedure SaveCollection(DOM: IXMLDOMDocument2; Collection: TSheetCollection);
  var
    Node: IXMLDOMNode;
    CollectionName: string;
  begin
    CollectionName := Collection.GetCollectionName;
    Node := DOM.createNode(1, CollectionName, '');
    Collection.WriteToXML(Node);
    DOM.documentElement.appendChild(Node);
  end;

var
  InnerDataNode: IXMLDOMNode;
begin
  if not Assigned(Dom) then
    Dom := NewMetaDataDom;
  InnerDataNode := Dom.createNode(1, 'innerdata', '');
  Dom.documentElement.appendChild(InnerDataNode);
  SaveInnerData(InnerDataNode);
  SaveCollection(Dom, FTotals);
  SaveCollection(Dom, FRows);
  SaveCollection(Dom, FColumns);
  SaveCollection(Dom, FFilters);
  SaveCollection(Dom, FSingleCells);
  SaveCollection(Dom, FParams);
  SaveConsts;
end;

procedure TSheetLogic.Clear;
begin
  IsLock := false;
  ClearInnerData;
  FCounter := 0;
  FPidCounter := 0;
  FTotals.Clear;
  FRows.Clear;
  FColumns.Clear;
  FFilters.Clear;
  FSingleCells.Clear;
  FParams.Clear;
  FConsts.Clear;
  FBreaks.Clear;
  KillDomDocument(FTotalsData);
  KillDomDocument(FFreeTotalsData);
  KillDomDocument(FFreeTotalsDataIgnored);
  KillDomDocument(FFormulaArrays);
  KillDomDocument(FSingleCellsData);
end;

function TSheetLogic.GetUniqueID: string;
begin
  inc(FCounter);
  result := IntToStr(FCounter);
end;

function TSheetLogic.GetParamID: integer;
begin
  inc(FPidCounter);
  result := FPidCounter;
end;

function TSheetLogic.NewMetaDataDOM: IXMLDOMDocument2;
const
  sCaptionValue: string = 'version="1.0" encoding="windows-1251"';
begin
  GetDOMDocument(result);
  result.documentElement := result.createElement(xpRoot);
  result.insertBefore(result.createProcessingInstruction('xml', sCaptionValue),
    result.documentElement);
end;

procedure TSheetLogic.QueryTotalsData;
var
  i: integer;
  Queries: TSheetMDXQueries;
  DataDOM: IXMLDOMDocument2;
  BothAxisAliasesList: TStringList;
begin
  {Первоначальные условия возможности выполнить сию операцию}
  if ((not CheckConnection) or Totals.Empty or (not Assigned(XMLCatalog))) then
    exit;
  { пытаемся загрузить метаданные базы }
  XMLCatalog.SetUp(DataProvider);
  if not XMLCatalog.Loaded then
    exit;
  KillDomDocument(FTotalsData);
  GetDomDocument(FTotalsData);
  FTotalsData.documentElement := FTotalsData.createElement('function_result');
  FTotalsData.documentElement.appendChild(FTotalsData.createElement('data'));

  BothAxisAliasesList := GetElementAliasesList([wsoRow, wsoColumn]);
  Queries := TSheetMDXQueries.Create(Self);
  try
    for i := 0 to Queries.Count - 1 do
    try
      ProcessMdxQuery(Queries[i], DataDom);
      {Сохраняем результат}
      if i = 0 then
        FTotalsData.load(DataDOM)
      else
        ConcatenateDataEx(FTotalsData, DataDOM, BothAxisAliasesList);
    finally
      KillDOMDocument(DataDOM);
    end;
    ConcatenateDataEx(FTotalsData, FreeTotalsData, BothAxisAliasesList);
    ConcatenateDataEx(FTotalsData, FreeTotalsDataIgnored, BothAxisAliasesList);
    ConcatenateDataEx(FTotalsData, SingleCellsData, BothAxisAliasesList);

    if AddinLogEnable then
      WriteDocumentLog(FTotalsData, 'Данные всех показателей.xml');
  finally
    FreeAndNil(Queries);
    FreeStringList(BothAxisAliasesList);
  end;
end;

function TSheetLogic.GetElementAliasesList(ATypes: TSheetObjectTypes): TStringList;
var
  i: integer;
begin
  result := TStringList.Create;

  {Дублирование, но маленькое и локальное :)}
  if wsoTotal in ATypes then
    for i := 0 to Totals.Count - 1 do
      result.Add(Totals[i].Alias);

  if wsoRow in ATypes then
    for i := 0 to Rows.Count - 1 do
      result.Add(Rows[i].Alias);

  if wsoColumn in ATypes then
    for i := 0 to Columns.Count - 1 do
      result.Add(Columns[i].Alias);
end;

function TSheetLogic.GetFilterCountWithScope(IsPartial: boolean): integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to Filters.Count - 1 do
    if FFilters[i].IsPartial = IsPartial then
      inc(result);
end;

procedure TSheetLogic.SetUpMeasuresPosition;
var
  i: integer;
begin
  if not Assigned(XMLCatalog) then
    exit; //невозможно заниматься
  if Totals.Empty then
    exit; //нечего делать
  {разрезность по столбцам}
  if not Columns.Empty then
  begin
    for i := 0 to Totals.Count - 1 do
      if (FTotals[i].TotalType in [wtMeasure, wtResult]) then
      begin
        if not Assigned(FTotals[i].Cube) then
          continue;
        FTotals[i].IsIgnoredColumnAxis := FTotals[i].IsIgnoredColumnAxis or (FTotals[i].FitInAxis(axColumn) <> tfFull);
      end
  end
  else //Оси нет. Считаем что все итоги подходят для оси.
    for i := 0 to Totals.Count - 1 do
      if (FTotals[i].TotalType in [wtMeasure, wtResult]) then
        FTotals[i].IsIgnoredColumnAxis := false;
  {разрезность по строкам}
  if not Rows.Empty then
  begin
    for i := 0 to Totals.Count - 1 do
    begin
      if (FTotals[i].TotalType in [wtMeasure, wtResult]) then
      begin
        if not Assigned(FTotals[i].Cube) then
          continue;
        FTotals[i].IsIgnoredRowAxis := FTotals[i].FitInAxis(axRow) = tfNone;
      end
    end;
  end
  else
    for i := 0 to Totals.Count - 1 do
      if (FTotals[i].TotalType in [wtMeasure, wtResult]) then
        FTotals[i].IsIgnoredRowAxis := false;
end;

procedure TSheetLogic.ClearCPGarbage(GarbageOnly: boolean);
var
  i, j, Index, CPCount: integer;
  CPList : TStringList;
  CP: CustomProperty;
  CPName: string;
begin
  if not Assigned(FExcelSheet) then
    exit;
  {заполнение списка записей CP}
  CPList := TStringList.Create;
  CPCount := FExcelSheet.CustomProperties.Count;
  if CPCount = 0 then
    exit;
  for i := 1 to CPCount do
    CPList.Add(FExcelSheet.CustomProperties[i].Name);

  {удаление из списка "правильных" записей (для которых есть объекты)}
  Index := CPList.IndexOf(cpMDName);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpSheetHistory);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpConstsName);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpUserEvents);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpRowsMarkup);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpColumnsMarkup);
  if Index >= 0 then
    CPList.Delete(Index);
  Index := CPList.IndexOf(cpCellsMarkup);
  if Index >= 0 then
    CPList.Delete(Index);
  if not Rows.Empty then
  begin
    Index := CPList.IndexOf(cpRowsAxis);
    if Index >= 0 then
      CPList.Delete(Index);
  end;
  if not Columns.Empty then
  begin
    Index := CPList.IndexOf(cpColumnsAxis);
    if Index >= 0 then
      CPList.Delete(Index);
  end;

  if GarbageOnly then
  begin
    for i := 0 to FRows.Count - 1 do
    begin
      Index := CPList.IndexOf(FRows[i].CPName);
      if Index >= 0 then
        CPList.Delete(Index);
    end;
    for i := 0 to FColumns.Count - 1 do
    begin
      Index := CPList.IndexOf(FColumns[i].CPName);
      if Index >= 0 then
        CPList.Delete(Index);
    end;
    for i := 0 to FFilters.Count - 1 do
    begin
      Index := CPList.IndexOf(FFilters[i].CPName);
      if Index >= 0 then
        CPList.Delete(Index);
    end;
    for i := 0 to FSingleCells.Count - 1 do
      for j := 0 to FSingleCells[i].Filters.Count - 1 do
      begin
        Index := CPList.IndexOf(FSingleCells[i].Filters[j].CPName);
        if Index >= 0 then
          CPList.Delete(Index);
      end;
  end;

  {оставшиеся в списке записи - это мусор, который и нужно удалить из СР}
  for i := 0 to CPList.Count - 1 do
  try
    CPName := CPList[i];
    CP := GetCPByName(FExcelSheet, CPName, false);
    if Assigned(CP) then
      CP.Delete;
  except
  end;
  CPList.Clear;
  CPList.Free;
end;

function TSheetLogic.Validate: boolean;
begin
  OpenOperation(pfoSMDValidate, true, false, 1);
  result := Rows.Validate;
  result := Columns.Validate and result;
  result := Filters.Validate and result;
  result := Totals.Validate and result;
(*  if NeedMapSingleCells then*)
    result := SingleCells.Validate and result;
  if Empty then
    Breaks.Clear;
  if Result then
    CloseOperation
  else
    PostMessage(ermValidationFault, msgError);
end;

function TSheetLogic.Validate(Total: TSheetTotalInterface): boolean;
var
  Msg: string;
  ErrorCode: integer;
begin
  OpenOperation(pfoSMDValidate, true, false, 1);
  result := Total.Validate(Msg, ErrorCode);
  if result  then
    CloseOperation
  else
    PostMessage('- показатель "' + Total.CubeName + '.' +
      Total.MeasureName + '": ' + Msg + ';', msgError);
end;

function TSheetLogic.Refresh(Force: boolean): boolean;
begin
  OpenOperation(pfoSMDRefresh, CriticalNode, NoteTime, otUpdate);
  result := Rows.Refresh(Force);
  result := Columns.Refresh(Force) and result;
  result := Filters.Refresh(Force) and result;
  result := SingleCells.Refresh(Force) and result;
  if Result then
    CloseOperation
  else
    PostMessage(ermValidationFault, msgError);
end;

function TSheetLogic.MoveElement(ObjType, NewObjType, UID: string): boolean;
var
  tmpDOM: IXMLDOMDocument2;
  tmpNode: IXMLDOMNode;
  Index: integer;
  Elem, NewElem: TSheetDimension;
  CheckResult: TSheetObjectType;
begin
  result := false;
  if ObjType = sntRowDimension then
  begin
    Index := Rows.FindById(UID);
    Elem := FRows[Index];
  end
  else
    if ObjType = sntColumnDimension then
    begin
      Index := Columns.FindById(UID);
      Elem := Columns[Index];
    end
    else
      if ObjType = sntFilter then
      begin
        Index := Filters.FindById(UID);
        CheckResult :=
          IsDimensionUsedEX(FFilters[Index]);
        case CheckResult of
          wsoRow:
            begin
              ShowMessageEX(Format(wrmDimInUseAlrady, ['элементов строк']), msgError);
              exit;
            end;
          wsoColumn:
            begin
              ShowMessageEX(Format(wrmDimInUseAlrady, ['элементов столбцов']), msgError);
              exit;
            end;
          wsoFilter:
            begin
              ShowMessageEX(Format(wrmDimInUseAlrady, ['элементов фильтров']), msgError);
              exit;
            end;
          end;
        Elem := Filters[Index];
      end
      else
        exit;
  GetDOMDocument(tmpDOM);
  try
    tmpNode := tmpDOM.createNode(1, 'tmpnode', '');
    tmpDOM.appendChild(tmpNode);
    Elem.WriteToXML(tmpNode);

    if NewObjType = sntRowDimension then
      NewElem := FRows.Append
    else
      if NewObjType = sntColumnDimension then
        NewElem := FColumns.Append
      else
        NewElem:= FFilters.Append;

    {у старого измерения искусственно разрушим связь с параметром. Если этого
      не сделать, то при удалении этого элемента может быть удален и параметр -
      и новый элемент окажется непараметризованным}
    Elem.Param := nil;
    NewElem.ReadFromXML(tmpNode);

    if ObjType = sntRowDimension then
      Rows.Delete(Index)
    else
      if ObjType = sntColumnDimension then
        Columns.Delete(Index)
      else
        Filters.Delete(Index);

    if ObjType = sntFilter then
    begin
      TSheetAxisElement(NewElem).RecreateLevelsByMembers;
      NewElem.TurnAllLevelsOn;
    end;
    result := true;
  finally
    KillDOMDocument(tmpDOM);
  end;
end;

function TSheetLogic.GetSheetVersion: string;
begin
  if FSheetVersion <> '' then
    result := FSheetVersion
  else
    result := '0.0.0';
end;

procedure TSheetLogic.LoadSeparators;
begin
  FDSeparator := ',';
  FTSeparator := ' ';
  FListSeparator := ';';
  if not Assigned(ExcelSheet) then
    exit;
  if ExcelSheet.Application.UseSystemSeparators then
  begin
    FDSeparator := SysUtils.DecimalSeparator;
    FTSeparator := SysUtils.ThousandSeparator;
    FListSeparator := SysUtils.ListSeparator;
  end
  else
  begin
    FDSeparator := ExcelSheet.Application.DecimalSeparator;
    FTSeparator := ExcelSheet.Application.ThousandsSeparator;
  end;
  try
    FCountrySetting := ExcelSheet.Application.International[xlCountrySetting, 0];
  except
    FCountrySetting := 0;
  end;
end;

function TSheetLogic.GetEmpty: boolean;
begin
  result := Filters.Empty and Columns.Empty and Rows.Empty and Totals.Empty
    and SingleCells.Empty;
end;

function TSheetLogic.Get_Totals: TSheetTotalCollectionInterface;
begin
  result := FTotals;
end;

function TSheetLogic.Get_Rows: TSheetAxisCollectionInterface;
begin
  result := FRows;
end;

function TSheetLogic.Get_Columns: TSheetAxisCollectionInterface;
begin
  result := FColumns;
end;

function TSheetLogic.Get_Filters: TSheetFilterCollectionInterface;
begin
  result := FFilters;
end;

function TSheetLogic.Get_SingleCells: TSheetSingleCellCollectionInterface;
begin
  result := FSingleCells;
end;

function TSheetLogic.Get_Consts: TConstCollectionInterface;
begin
  result := FConsts;
end;

function TSheetLogic.Get_Params: TParamCollectionInterface;
begin
  result := FParams;
end;

function TSheetLogic.IsDimensionUsed(DimName,
  HierName: string): TSheetObjectType;
var
  i: integer;
begin
  result := wsoTotal; //используется в качестве отрицательного результата
  for i := 0 to Rows.Count - 1 do
    if (FRows[i].Dimension = DimName) and (FRows[i].Hierarchy = HierName) then
    begin
      result := wsoRow;
      exit;
    end;
  for i := 0 to Columns.Count - 1 do
    if (FColumns[i].Dimension = DimName) and (FColumns[i].Hierarchy = HierName) then
    begin
      result := wsoColumn;
      exit;
    end;
  for i := 0 to Filters.Count - 1 do
    if (FFilters[i].Dimension = DimName) and (FFilters[i].Hierarchy = HierName) then
    begin
      result := wsoFilter;
      exit;
    end;
end;

function TSheetLogic.IsDimensionUsedEX(SheetElement: TSheetDimension): TSheetObjectType;
var
  i: integer;
begin
  result := wsoTotal; //используется в качестве отрицательного результата
  for i := 0 to Rows.Count - 1 do
    if (SheetElement.UniqueID <> FRows[i].UniqueID) and
    (FRows[i].Dimension = SheetElement.Dimension) and
    (FRows[i].Hierarchy = SheetElement.Hierarchy) then
    begin
      result := wsoRow;
      exit;
    end;
  for i := 0 to Columns.Count - 1 do
    if (SheetElement.UniqueID <> FColumns[i].UniqueID) and
    (FColumns[i].Dimension = SheetElement.Dimension) and
    (FColumns[i].Hierarchy = SheetElement.Hierarchy) then
    begin
      result := wsoColumn;
      exit;
    end;
  for i := 0 to Filters.Count - 1 do
    if (SheetElement.UniqueID <> FFilters[i].UniqueID) and
    (FFilters[i].Dimension = SheetElement.Dimension) and
    (FFilters[i].Hierarchy = SheetElement.Hierarchy) then
    begin
      result := wsoFilter;
      exit;
    end;
end;

function TSheetLogic.GetDimension(DimensionName: widestring): TSheetDimension;
var
  i: integer;
begin
  result := Rows.FindByFullDimensionName(DimensionName);
  if Assigned(result) then
    exit;
  result := Columns.FindByFullDimensionName(DimensionName);
  if Assigned(result) then
    exit;
  result := Filters.FindByFullDimensionName(DimensionName);
  if Assigned(result) then
    exit;
  for i := 0 to SingleCells.Count - 1 do
  begin
    result := SingleCells[i].Filters.FindByFullDimensionName(DimensionName);
    if Assigned(result) then
      exit;
  end;
end;

(*
procedure TSheetLogic.MakeBackup;
var
  Book: ExcelWorkbook;
  NewSheet: ExcelWorksheet;
  OldSheetName, NewSheetName: string;
begin
  if FUndoCounter + 1 < FMaxUndoSteps then
  begin
    {Если это только что созданный лист, то бэкапить не надо}
    if not IsPlaningSheet(ExcelSheet) then
      exit;
    Book := ExcelSheet.Application.ActiveWorkbook;
    ExcelSheet.Copy(EmptyParam, ExcelSheet, 0);
    OldSheetName := ExcelSheet.Name;
    NewSheetName := OldSheetName + '_backup' + IntToStr(FUndoCounter + 1);
    ExcelSheet.Name := NewSheetName;
    NewSheet := Book.Sheets[ExcelSheet.Index[0] + 1] as ExcelWorksheet;
    NewSheet.Name := OldSheetName;
    FExcelSheet.Visible[0] := xlSheetVeryHidden;
    FExcelSheet := NewSheet;
    FExcelSheet.Activate(0);
    inc(FUndoCounter);
    FSheetNames[FUndoCounter] := NewSheetName;
  end
  else
  begin
  end;
end;

procedure TSheetLogic.Undo;
var
  Book: ExcelWorkbook;
  LastSheet, BackupSheet, OldSheet: ExcelWorksheet;
  SheetsCount: integer;
  SheetName: string;
begin
  if FUndoCounter < 0 then
    exit;
  FExcelSheet.Visible[0] := xlSheetVeryHidden;
//  dec(FUndoCounter);

  SheetName := FSheetNames[FUndoCounter];
  Book := ExcelSheet.Application.ActiveWorkbook;
  BackupSheet := Book.Sheets[SheetName] as ExcelWorkSheet;
  FExcelSheet := BackupSheet;
  BackupSheet.Activate(0);
end;
*)

procedure TSheetLogic.Backup;
begin
  FBackupXml := GetDataFromCP(FExcelSheet, cpMDName);
end;

procedure TSheetLogic.Restore;
begin
  if not Assigned(FBackupXml) then
    exit;
  // при ресторе собирать данные никак нельзя
  Exclude(FLoadMode, lmFreeData);
  LoadFromXml(FBackupXml);
end;

procedure TSheetLogic.LoadFromXml(MetaDataDom: IXMLDOMDocument2);
begin
  if not Assigned(MetaDataDom) then
    exit;

  LoadInnerData(MetaDataDOM.selectSingleNode(xpInnerData));

  if (lmCollections in LoadMode) then
  begin
    FTotals.ReadFromXML(MetaDataDOM.selectSingleNode(xpTotals));
    {Коллекцию параметров надо загрузить перед измерениями}
    FParams.ReadFromXML(MetaDataDOM.selectSingleNode(xpParams));
    FRows.ReadFromXML(MetaDataDOM.selectSingleNode(xpRows));
    FColumns.ReadFromXML(MetaDataDOM.selectSingleNode(xpColumns));
    FFilters.ReadFromXML(MetaDataDOM.selectSingleNode(xpFilters));
    FSingleCells.ReadFromXML(MetaDataDOM.selectSingleNode(xpSingleCells));
    FWritablesInfo.UpdateSingleCellsPoints(ExcelSheet);
  end;

end;

procedure TSheetLogic.OnTaskConnection(IsConnected: boolean);
var
  HistoryList: TStringList;
  i: integer;
  DefaultExlSheet, ExlSheet: ExcelWorkSheet;
begin
  HistoryList := TStringList.Create;
  DefaultExlSheet := ExcelSheet;
  try
    if IsConnected then
      HistoryList.Add('Документ прикреплен к задаче')
    else
      HistoryList.Add('Документ откреплен от задачи');
    if Assigned(Environment) then
    begin
      HistoryList.Add('Наименование задачи: ' + Environment.TaskName);
      HistoryList.Add('Идентификатор задачи: ' + Environment.TaskId);
      HistoryList.Add('Наименование документа: ' + Environment.DocumentName);
      HistoryList.Add('Идентификатор документа: ' + Environment.DocumentId);
      HistoryList.Add('Исполнитель: ' + Environment.Owner);
      HistoryList.Add('Оригинальный путь к документу: ' + Environment.DocPath);
    end;

    // прикрепление\открепление к задачам - событие всех листов книги
    for i := 1 to ExcelSheet.Application.ActiveWorkbook.Worksheets.Count do
    begin
      ExlSheet := GetWorkSheet(ExcelSheet.Application.ActiveWorkbook.Worksheets[i]);
      if not Assigned(ExlSheet) then
        continue;
      if not IsPlaningSheet(ExlSheet) then
        continue;
      ExcelSheet := ExlSheet;
      if IsConnected then
        AddEventInSheetHistory(evtTaskConnectionOn, HistoryList.CommaText, true)
      else
        AddEventInSheetHistory(evtTaskConnectionOff, HistoryList.CommaText, true);
    end;

    if not IsConnected then
    begin
      Environment.DocPath := '';
      Environment.TaskName := '';
      Environment.TaskId := '';
      Environment.DocumentName := '';
      Environment.DocumentId := '';
      Environment.Owner := '';
    end;
  finally
    FreeStringList(HistoryList);
    ExcelSheet := DefaultExlSheet;
  end;
end;

procedure TSheetLogic.GetFactorization(Axis: TSheetAxisCollectionInterface;
  FromBaseOnly: boolean;
  out List1, List2: TStringList; out GrandTotalsOnly: string);
var
  i, j: integer;
  Total: TSheetTotalInterface;
  tmpStr1, tmpStr2: string;
begin
  List1.Clear;
  List2.Clear;
  GrandTotalsOnly := '';
  for i := 0 to Axis.Count - 1 do
  begin
    tmpStr1 := '';
    tmpStr2 := '';
    for j := 0 to Totals.Count - 1 do
    begin
      Total := Totals[j];
      if FromBaseOnly and Total.SummariesByVisible then
        continue;
      if (Axis.AxisType = axColumn) and Total.IsIgnoredColumnAxis then
        continue;

      if not Total.FactorizedBy(Axis[i]) then
        begin
          AddTail(tmpStr2, ' or ');
          tmpStr2 := tmpStr2 + Format('(@%s)', [Total.Alias]);
        end
        else
        begin
          AddTail(tmpStr1, ' or ');
          tmpStr1 := tmpStr1 + Format('(@%s)', [Total.Alias]);
        end;
      if i = 0 then
        if Total.IsIgnoredRowAxis and Total.IsGrandTotalDataOnly then
        begin
          AddTail(GrandTotalsOnly, ' or ');
          GrandTotalsOnly := GrandTotalsOnly + '@' + Total.Alias;
        end;
    end;
    if tmpStr1 <> '' then
      tmpStr1 := '(' + tmpStr1 + ')';
    List1.Add(tmpStr1);
    if tmpStr2 <> '' then
      tmpStr2 := '(' + tmpStr2 + ')';
    List2.Add(tmpStr2);
    if GrandTotalsOnly <> '' then
      GrandTotalsOnly := TupleBrackets(GrandTotalsOnly);
  end;
end;

procedure TSheetLogic.LoadConsts;
var
  ConstsDom: IXmlDomDocument2;
  i: integer;
  Sheet: ExcelWorksheet;
begin
  // считываем из первого найденного листа с коллекцией констант
  for i := 1 to ExcelSheet.Application.ActiveWorkbook.Worksheets.Count do
  begin
    Sheet := GetWorkSheet(ExcelSheet.Application.ActiveWorkbook.Worksheets[i]);
    if not Assigned(Sheet) then
      continue;
    ConstsDom := GetDataFromCP(Sheet, cpConstsName);
    if Assigned(ConstsDom) then
      break;
  end;
  try
    if Assigned(ConstsDom) then
      FConsts.ReadFromXML(ConstsDom.documentElement)
    else
      FConsts.ReadFromXML(nil);
  finally
    KillDomDocument(ConstsDom);
  end;
end;

procedure TSheetLogic.SaveConsts;
var
  ConstsDom: IXmlDomDocument2;
  i: integer;
  Sheet: ExcelWorksheet;
begin
  // сохраняем во все листы книги
  ConstsDom := InitXmlDocument;
  try
    ConstsDom.documentElement := (ConstsDom.createNode(1, FConsts.GetCollectionName, '') as IXmlDomElement);
    FConsts.WriteToXML(ConstsDom.documentElement);
    for i := 1 to ExcelSheet.Application.ActiveWorkbook.Worksheets.Count do
    begin
      Sheet := GetWorkSheet(ExcelSheet.Application.ActiveWorkbook.Worksheets[i]);
      if not Assigned(Sheet) then
        continue;
      GetCPByName(Sheet, cpConstsName, true).Value := ConstsDom.xml;
    end;
  finally
    KillDomDocument(ConstsDom);
  end;
end;

procedure TSheetLogic.DeleteConstElements(Constant: TConstInterface);
var
  i: integer;
  ERange: ExcelRange;
  EName: ExcelXP.Name;
begin
  for i := Totals.Count - 1 downto 0 do
    if (Totals[i].TotalType = wtConst) and (Totals[i].Caption = Constant.Name) then
    begin
      Totals.Delete(i);
    end;

  for i := SingleCells.Count - 1 downto 0 do
    if (SingleCells[i].TotalType = wtConst) and (SingleCells[i].Name = Constant.Name) then
    begin
      ERange := GetRangeByName(ExcelSheet, SingleCells[i].ExcelName);
      if Assigned(ERange) then
        ERange.Clear;
      EName := GetNameObject(ExcelSheet, SingleCells[i].ExcelName);
      if Assigned(EName) then
        EName.Delete;
      SingleCells.Delete(i);
    end;
end;

function TSheetLogic.FindDimensionByUniqueId(
  UniqueId: string): TSheetDimension;
var
  Index, i: integer;
begin
  result := nil;
  Index := Rows.FindById(UniqueId);
  if Index > -1 then
  begin
    result := Rows[Index];
    exit;
  end;

  Index := Columns.FindById(UniqueId);
  if Index > -1 then
  begin
    result := Columns[Index];
    exit;
  end;

  Index := Filters.FindById(UniqueId);
  if Index > -1 then
  begin
    result := Filters[Index];
    exit;
  end;

  for i := 0 to SingleCells.Count - 1 do
  begin
    Index := SingleCells[i].Filters.FindById(UniqueId);
    if Index > -1 then
    begin
      result := SingleCells[i].Filters[Index];
      exit;
    end;

  end;
end;

procedure TSheetLogic.QueryOneTotalData(Total: TSheetTotalInterface);
var
  Query: TSheetMDXQuery;
  BothAxisAliasesList: TStringList;
begin
  KillDomDocument(FTotalsData);
  GetDomDocument(FTotalsData);

  BothAxisAliasesList := GetElementAliasesList([wsoRow, wsoColumn]);
  Query := TSheetMDXQuery.Create(Total);
  try
    ProcessMdxQuery(Query, FTotalsData);
    {Сохраняем результат}
    ConcatenateDataEx(FTotalsData, FreeTotalsData, BothAxisAliasesList);
    ConcatenateDataEx(FTotalsData, FreeTotalsDataIgnored, BothAxisAliasesList);
    ConcatenateDataEx(FTotalsData, SingleCellsData, BothAxisAliasesList);

    if AddinLogEnable then
      WriteDocumentLog(FTotalsData, 'Данные всех показателей.xml');
  finally
    FreeAndNil(Query);
    FreeStringList(BothAxisAliasesList);
  end;
end;


function TranslateAdoProviderError(Msg: string): string;
var
  StartIndex, EndIndex: integer;
begin
  result := Msg;
  if Pos('cannot find dimension member', result) <= 0 then
    exit;
  StartIndex := Pos('("', result) + 1;
  EndIndex := Pos('")', result) + 1;
  result := 'Элемент измерения ' + Copy(result, StartIndex, EndIndex - StartIndex) + ' не найден.';
end;

procedure TSheetLogic.ProcessMdxQuery(Query: TSheetMDXQuery; var DataDom: IXMLDOMDocument2);
var
  MdxQuery: string;
begin
  MdxQuery := Query.Text;
  UpdateMDXLog(MDXQuery, (ExcelSheet.Parent as ExcelWorkBook).Name, Query.ProviderId);
  {Выполнение запроса}
  OpenOperation(pfoQueryMdx + Query.TotalCaptions,
    CriticalNode, NoteTime, otQuery);

  {Если запрос "большой" предупредим, что-бы это всегда видно было}
  if Query.IsLargeQuery then
    PostMessage(pfoLargeMDXQuery, msgInfo);

  if not DataProvider.GetRecordsetData(Query.ProviderId, MDXQuery, DataDom) then
  begin
    PostMessage(pfoQueryFailed + TranslateAdoProviderError(DataProvider.LastError), msgError);
    exit;
  end;
  CloseOperation; // pfoQueryMdx
end;

end.


