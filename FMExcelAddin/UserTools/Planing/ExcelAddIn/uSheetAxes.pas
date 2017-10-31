{
  Оси листа планирования.
  Описывается объект - "элемент оси" и объект -коллекция таких элементов.
  Сейчас нет разделения между строками и столбцами. И в том и другом случае
  используются одни и те-же объекты. Естественно, это приводит к некоторому
  кол-ву case-ов по всей программе.
  Когда руки дойдут, нужно бдует отнаследовать от базового элемента два
  соответствующих потомка для строк и столбцов. После этого весь case-подобный
  код будет там.
}

unit uSheetAxes;

interface

uses
  Windows, Classes, SysUtils, MSXML2_TLB, uXMLUtils, ExcelXP, PlaningProvider_TLB,
  uXMLCatalog, uFMExcelAddinConst, PlaningTools_TLB, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uFMAddinExcelUtils, uFMAddinRegistryUtils,
  uSheetObjectModel, uSheetBreaks, uExcelUtils, uSheetMemberProperties,
  uSheetLevels, uSheetParam, uGlobalPlaningConst, uMDXSetGenerator;


type

  {элемент оси строк или столбцов}
  TSheetAxisElement = class(TSheetAxisElementInterface)
  private
    procedure LoadLevelsFromCatalog;
    procedure MoveDM(Dom: IXMLDOMDocument2; NL: IXMLDOMNodeList; Deployment: TItemDeployment);
  protected
    function GetExcelName: string; override;
    function GetOrientation: TAxisType; override;
    function GetAlias: string; override;
    function GetFieldCount: integer; override;
    function GetCommentText: string; override;
    function GetMdxText: string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    constructor Create(AOwner: TSheetCollection);
    destructor Destroy; override;
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;

    {проверяет содержимое на соответствие каталогу}
    function Validate(out MsgText: string; out ErrorCode: integer): boolean; override;
    {Возвращает данные элемента, если их нет, запрашивает с сервера}
    function GetOrQueryMembers: IXMLDOMDocument2; override;
    {MDX-множество, где перечислены все выбранные элементы}
    function GetMDXMembersSet: string; override;
    {Функция альтернативная предыдущей (GetMDXMembersSet),
    иногда прямое перечисление слишком велико по объему.
    Эта функция собирает set по структуре уровней
    (менее оптимальна по обработке MDX-запроса)}
    function GetMDXLevelsSet: string; override;
    // получить тип элемента листа
    function GetObjectType: TSheetObjectType; override;
    // получить строковое описание типа элемента листа
    function GetObjectTypeStr: string; override;
    procedure RecreateLevelsByMembers; override;
    procedure ApplyStyles; override;
    procedure LoadWithIndices(Dom: IXMLDOMDocument2); override;
    procedure RemoveAndRenameDataMembers(Dom: IXMLDOMDocument2); override;
    function GetOnDeleteWarning: string; override;
  end;

  {коллекция строк или столбцов}
  TSheetAxisCollection = class(TSheetAxisCollectionInterface)
  private
    {составляет XPath для выборки узлов данных для построения оси без пустых строк}
    function GetCoordXPath(CoordList, Decomposable, Indecomposable: TStringList;
      LeafCondition: string; Limit: integer): string;
  protected
    function GetItem(Index: integer): TSheetAxisElementInterface; override;
    procedure SetItem(Index: integer; Item: TSheetAxisElementInterface); override;
    function GetMPCheckedCount: integer; override;
    function GetFieldCount: integer; override;
    function GetCommentText: string; override;
    procedure Summaries(Dom: IXmlDomDocument2; ItemIndex: integer;
      ImperativeSummaries: boolean);
    { балансирует дерево, добавляя фиктивные узлы }
    procedure BalanceAxisElement(Dom: IXMLDOMDocument2; AxisIndex: integer);
    { подцепляет второй DOM c мемберсами к первому }
    procedure MergeXML(FirstDOM, SecondDOM: IXMLDOMDocument2; AxisIndex: integer);
    procedure GetDomNormalized(Dom: IXMLDOMDocument2; AxisIndex: integer;
      ImperativeSummaries: boolean);
    function GetRegularAxisDom(ImperativeSummaries: boolean): IXMLDOMDocument2;
    function GetLightenAxisDom(ImperativeSummaries: boolean): IXMLDOMDocument2;
    function GetFullComboAxis(Parts: array of IXMLDOMDocument2): IXMLDOMDocument2;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    function GetMarkupFieldCount: integer; override;
  public
    { собирает все данные оси в один DOM }
    function GetFullAxisDOM: IXMLDOMDocument2; override;
    //добавляет элемент в коллекцию
    function Append: TSheetAxisElementInterface; override;
    //удаляет элемент из коллекции
    procedure Delete(Index: integer); override;
    //загружает коллекцию из DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает коллекцию в XML
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //обновляет все данные (Members-ы всех элементов);
    function Refresh(Force: boolean): boolean; override;
    //возвращает имя коллекции в соответствии с типом ее элементов
    function GetCollectionName: string; override;
    //MP в коллекции
    function MemberPropertiesAsStringList: TStringList; override;
    //возвращает индекс элемента в коллекции
    function FindByID(ID: string): integer; override;
    function FindByDimension(DimName: string): TSheetAxisElementInterface; override;
    function FindByDimAndHier(DimName, HierName: string): TSheetAxisElementInterface; override;
    function FindByFullDimensionName(FullDimensionName: string): TSheetAxisElementInterface; override;
    function FindByAlias(aStr: string): TSheetAxisElementInterface; override;
    //проверяет содержимое на соответствие каталогу
    function Validate: boolean; override;
    {все измерения оси входят в куб}
    function AllFromCube(CubeName: string): boolean; override;
    procedure SetDefaultStyles2All; override;
    procedure Clear; override;
    {получить сквозной порядковый номер уровня в оси}
    function GetLevelNumber(AxisIndex, LevelIndex: integer): integer; override;
    function GetLevelIndent(AxisIndex, LevelIndex: integer): integer; override;
    function GetLevelIndent(Level: TSheetLevelInterface): integer; override;

 (*   function GetLightAxisDom(Parts: TList): IXMLDOMDocument2;*)
  end;

  procedure KillMembersThatMustDie(FieldCount: integer; AxisDom: IXMLDOMDocument2);
  { Возвращает непосредственного родителя для узла в оси}
  function GetParentNode(Node: IXMLDOMNode): IXMLDOMNode;

  { Составляет частное условие для выборки данных из датадома показателей.
    Предназначена для использования в процедурах удаления из оси элементов,
    для которых отсутствуют данные.}
  function GetTotalsDataCondition(
    Alias: string; // Алиас измерения - элемента оси
    UName: string; // Имя мембера в этом измерении
    Decomposable: string; //  Алиасы раскладываемых по измерению показателей
    Indecomposable: string //  Алиасы нераскладываемых по измерению показателей
    ): string;
  { Собирает полное условие выборки данных из датадома показателей.}
  function GetDataSelectionXPath(
    Aliases: TStringList;
    UNames: TStringList;
    Decomposables: TStringList;
    Indecomposables: TStringList;
    LeafCondition: string
  ): string;

implementation

procedure LocalRemoveEmpty(AxisElement: TSheetAxisElementInterface; AxisDom: IXMLDOMDocument2); forward;
function CheckForIndecomposableData(AxisElement: TSheetAxisElementInterface; out NoSuchTotals: boolean): boolean; forward;
{возвращает список алиасов показателей, нераскладываемых по измерению}
function GetIndecomposableTotals(AxisElement: TSheetAxisElementInterface): TStringList; forward;

function GetInternalFieldCount(Elem: TSheetAxisElementInterface): integer; forward;

{********* TSheetAxisElement implemenatation ************}

constructor TSheetAxisElement.Create(AOwner: TSheetCollection);
begin
  inherited Create(AOwner);
  MemberProperties := TSheetMPCollection.Create(Self);
  SetDefaultStyles;
  SummaryOptions := TSummaryOptions.Create;
  UseSummariesForLevels := true;
  {уровни нужно создать после настроек итогов}
  Levels := TSheetLevelCollection.Create(Self);
end;

destructor TSheetAxisElement.Destroy;
begin
  SummaryOptions.Free;
  inherited Destroy;
end;

function TSheetAxisElement.GetExcelName: string;
begin
  result := snSeparator + UniqueID;
  if Orientation = axRow then
    result := sntRowDimension + result
  else
    result := sntColumnDimension + result;
  result := BuildExcelName(result);
end;

function TSheetAxisElement.GetOrientation: TAxisType;
begin
  result := (Owner as TSheetAxisCollection).AxisType;
end;

function TSheetAxisElement.GetAlias: string;
begin
  result := 'A_' + UniqueId;
end;

function TSheetAxisElement.GetFieldCount: integer;
begin
  result := IIF(IgnoreHierarchy, 1, Levels.Count);
end;

procedure TSheetAxisElement.ReadFromXML(Node: IXMLDOMNode);
var
  tmpId: string;
  IsParam, IsParamRead: boolean;
  Pid: integer;
begin
  inherited ReadFromXML(Node);
  if not Assigned(Node) then
    exit;
  tmpId := GetStrAttr(Node, attrID, '');
  if SheetInterface.InCopyMode then
    SheetInterface.CopyCustomProperty(tmpId, UniqueId)
  else
    UniqueID := tmpId;
  Dimension := GetNodeStrAttr(Node, attrDimension);
  Hierarchy := GetNodeStrAttr(Node, attrHierarchy);
  IgnoreHierarchy := GetBoolAttr(Node, attrIgnoreHierarchy, false);
  ReverseOrder := GetBoolAttr(Node, attrReverseOrder, false);
  HideDataMembers := GetBoolAttr(Node, attrHideDataMembers, false);
  {чтение уровней}
  if not Assigned(Levels)
    then Levels := TSheetLevelCollection.Create(Self);
  Levels.Clear;
  Levels.ReadFromXML(Node.selectSingleNode('levels'));
  if Levels.Count = 0 then
    LoadLevelsFromCatalog;
  {чтение мембер пропертиз}
  if not Assigned(MemberProperties)
    then MemberProperties := TSheetMPCollection.Create(Self);
  MemberProperties.Clear;
  MemberProperties.ReadFromXML(Node.selectSingleNode('properties'));
  if Assigned(SheetInterface.XMLCatalog) then
    if SheetInterface.XMLCatalog.Loaded then
      MemberProperties.Reload;

  {чтение параметра.
    Коллекция параметров грузится раньше измерений. Поэтому, если параметр был
    удален в задаче, он уже отсутствует в коллекции. Однако, получение элементов
    измерения завязано на имя СР, которое определяется в зависимости от того,
    является измерение параметром или нет. Для того, чтобы достать хмл измерения
    в случае, когда параметр удален при загрузке создадим фиктивный параметр}
  IsParam := Assigned(Node.selectSingleNode('paramProperties'));
  IsParamRead := true;
  if IsParam then
  begin
    Pid := GetIntAttr(Node.selectSingleNode('paramProperties'), 'pid', -1);
    Param := SheetInterface.Params.ParamByPid(Pid);
    IsParamRead := Assigned(Param);
    if not IsParamRead then
    begin
      Param := TParam.Create(SheetInterface.Params);
      Param.Name := 'Fake param';
      Param.PID := Pid;
      Param.Links.Add(IntToStr(Pid));
    end;
  end;
  if not (lmNoMembers in SheetInterface.LoadMode) then
    Members := GetMembers;
  if IsParam and not IsParamRead then
  begin
    Param.RemoveLink(Self);
    Param.Free;
    Param := nil;
  end;

  SummaryOptions.ReadFromXML(Node.selectSingleNode(attrSummaryOptions));
  UseSummariesForLevels := GetBoolAttr(Node, attrUseSummariesForLevels, true);
end;

procedure TSheetAxisElement.WriteToXML(Node: IXMLDOMNode);
var
  RootNode, ParamNode: IXMLDOMNode;
begin
  inherited WriteToXml(Node);
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    SetNodeStrAttr(Node, attrDimension, Dimension);
    SetNodeStrAttr(Node, attrHierarchy, Hierarchy);
    setAttribute(attrIgnoreHierarchy, BoolToStr(IgnoreHierarchy));
    setAttribute(attrReverseOrder, BoolToStr(ReverseOrder));
    setAttribute(attrHideDataMembers, BoolToStr(HideDataMembers));
    {запись коллекции уровней}
    RootNode := Node.ownerDocument.createNode(1, 'levels', '');
    Levels.WriteToXML(RootNode);
    Node.appendChild(RootNode);
    {запись коллекции мембер пропертиз}
    RootNode := Node.ownerDocument.createNode(1, 'properties', '');
    MemberProperties.WriteToXML(RootNode);
    Node.appendChild(RootNode);
    {настройки итогов}
    RootNode := Node.ownerDocument.createNode(1, attrSummaryoptions, '');
    SummaryOptions.WriteToXML(RootNode);
    Node.appendChild(RootNode);
    setAttribute(attrUseSummariesForLevels, BoolToStr(UseSummariesForLevels));
    {запись параметра}
    if IsParam then
    begin
      ParamNode := Node.ownerDocument.createNode(1, 'paramProperties', '');
      SetAttr(ParamNode, 'pid', Param.PID);
      Node.appendChild(ParamNode);
    end
    else
      {сохранение мемберов в СР}
      if Assigned(Members) and not SheetInterface.InCopyMode then
        StoreData(Members);
  end;
end;

function TSheetAxisElement.Validate(out MsgText: string; out ErrorCode: integer): boolean;
const
  msgDimError = ' измерение "%s" не существует';
  msgHierError = ' иерархия "%s" отсутствует в измерении "%s"';
  msgLevError1 = ' уровни %s не существуют';
  msgLevError2 = ' уровнень %s не существует';
  msgElemError = ' не выбрано ни одного элемента';

var
  Dim: TDimension;
  Hier: THierarchy;
  NL: IXMLDOMNodeList;
  i: integer;
begin
  result := false;
  {проверка на наличие такого измерения}
  Dim := SheetInterface.XMLCatalog.Dimensions.Find(Dimension, ProviderId);
  if not Assigned(Dim) then
  begin
    MsgText := Format(msgDimError, [Dimension]);
    ErrorCode := ecNoDimension;
    exit;
  end;
  {проверка на наличие в этом измерении такой иерархии}
  Hier := Dim.GetHierarchy(Hierarchy);
  result := Assigned(Hier);
  if not result then
  begin
    MsgText := Format(msgHierError, [Hierarchy, Dimension]);
    ErrorCode := ecNoHierarchy;
    exit;
  end;
  {проверка на наличие в этой иерархии таких уровней}
  result := Hier.Levels.Count > 0;
  if result then
  begin
    for i := 0 to Levels.Count - 1 do
      if Hier.Levels.IndexOf(Levels[i].Name) < 0 then
      begin
        AddTail(MsgText, ', ');
        MsgText := MsgText + '"' + Levels[i].Name + '"';
      end;
    if MsgText <> '' then
    begin
      if Pos(', ', MsgText) > 0 then
        MsgText := Format(msgLevError1, [MsgText])
      else
        MsgText := Format(msgLevError2, [MsgText]);
      result := false;
      ErrorCode := ecNoLevel;
      exit;
    end;
  end;
  {проверка на наличие выделенных элементов}
  if result then
  begin
    NL := SelectNodes(Members, '//' + ntMember + '[@checked="true"]');
    if Assigned(NL) then
      result := NL.length > 0
    else
      result := false;
    if not result then
    begin
      MsgText := msgElemError;
      ErrorCode := ecNoSelection;
    end;
  end;    
end;

function TSheetAxisElement.GetOrQueryMembers: IXMLDOMDocument2;
begin
  result := Members;
  if not Assigned(result) then
    try
      result := SheetInterface.DataProvider.GetMemberList(
        ProviderId, '' , Dimension,
        Hierarchy, Levels.ToString, AllMemberProperties);
    except
      result := nil;
    end;
end;

function TSheetAxisElement.GetMDXMembersSet: string;
var
  i: integer;
  NL: IXMLDOMNodeList;
begin
  result := '';
(*  NL := SelectNodes(Members, '//' + ntMember + '[@checked="true"]');*)
  {в запрос должны попасть все те элементы, что будут выведены в лист,
  а не только выделенные галочками. это нужно для правильного отображения
  промежуточных итогов}
  NL := SelectNodes(Members, '//' + ntMember + '[@influence!="3"]');
  if not Assigned(NL) then
    exit;
  if NL.length > 0 then
  begin
    result := AllMember;
    for i := 0 to NL.length - 1 do
    begin
      AddTail(result, ', ');
      result := result + GetStrAttr(NL[i], attrUniqueName, '');
    end;
    result := SetBrackets(result);
  end;
end;

{!!!! Устаревшие методы. Стринг листы какие-то наполяются,
  почему бы просто не пробежаться коллекции уровней в элементе без
  всякого каталога (верефикация должна выполняться не здесь)}
function TSheetAxisElement.GetMDXLevelsSet: string;
var
  i, Index: integer;
  LevelName, AllNames: string;
  LevelsToTake: TStringList;
  CatHier: THierarchy;
  CatLevels: TLevelCollection;
  SheetLevelsNames: TStringList;
begin
  result := '';
  { получим  коллекцию уровней измерения}
  CatHier := CatalogHierarchy;
  CatLevels := CatHier.Levels;

  SheetLevelsNames := TStringList.Create;
  AllNames := Levels.ToString;
  LevelName := CutPart(AllNames, snBucks);
  while LevelName <> '' do
  begin
    SheetLevelsNames.Add(LevelName);
    LevelName := CutPart(AllNames, snBucks);
  end;

  LevelsToTake := TStringList.Create;
  LevelsToTake.Add('0');
  for i := 1 to CatLevels.Count - 1 do
  begin
    LevelName := CatLevels[i].Name;
    if SheetLevelsNames.IndexOf(LevelName) > -1 then
    begin
      LevelsToTake.Add(IntToStr(i));
    end;
  end;
  LevelsToTake.Sort;
  for i := 0 to LevelsToTake.Count - 1 do
  begin
    AddTail(result, ',');
    Index := StrToInt(LevelsToTake.Strings[i]);
    result := result + MemberBrackets(Dimension);
    {Если есть иерархия, то ее надо подцепить}
    if Hierarchy <> '' then
      result := result + '.' + MemberBrackets(Hierarchy)
    else
      if Pos(snSemanticsSeparator, Dimension) > 0 then
        result := result + '.' + MemberBrackets(Dimension);
    result := result + '.'
      + MemberBrackets(CatLevels.Items[Index].Name) + '.AllMembers';
  end;
  if (AllMember <> '') and (result <> '') then
    result := AllMember + ', ' + result;

  result := SetBrackets(result);
  
  FreeStringList(SheetLevelsNames);
  FreeStringList(LevelsToTake)
end;


{************ TSheetAxisColection implementation **********}

function TSheetAxisCollection.GetItem(Index: integer): TSheetAxisElementInterface;
begin
  result := Get(Index);
end;

procedure TSheetAxisCollection.SetItem(Index: integer; Item: TSheetAxisElementInterface);
begin
  Put(Index, Item);
end;

function TSheetAxisCollection.Append: TSheetAxisElementInterface;
begin
  result := TSheetAxisElement.Create(Self);
  inherited Add(result);
  result.PermitEditing := PermitEditing;
  SummaryOptions := TSummaryOptions.Create;
  result.SummaryOptions.Copy(SummaryOptions);
end;

procedure TSheetAxisCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TSheetAxisCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  AxisElement: TSheetAxisElementInterface;
begin
  inherited;
  if not Assigned(Node) then
    exit;

  if Node.nodeName = 'rows' then
  begin
    AxisType := axRow;
    NL := Node.selectNodes('row');
  end;

  if Node.nodeName = 'columns' then
  begin
    AxisType := axColumn;
    NL := Node.selectNodes('column');
  end;

  for i := 0 to NL.length - 1 do
  begin
    AxisElement := Append;
    AxisElement.ReadFromXML(NL[i]);
  end;
  SummaryOptions.ReadFromXML(Node.selectSingleNode(attrSummaryOptions));
  UseSummariesForElements := GetBoolAttr(Node, attrUseSummariesForElements, true);
  GrandSummaryOptions.ReadFromXML(Node.selectSingleNode(attrGrandSummaryOptions));
  Broken := GetBoolAttr(Node, attrBroken, false);
  ReverseOrder := GetBoolAttr(Node, attrReverseOrder, false);
  MPBefore := GetBoolAttr(Node, attrMPBefore, false);
  HideEmpty := GetBoolAttr(Node, attrHideEmpty, false);
  SummaryOptimization := GetBoolAttr(Node, attrSummaryOptimization, true);
  UseIndents := GetBoolAttr(Node, attrUseIndents, false);
  LevelsFormatting := GetBoolAttr(Node, attrLevelsFormatting, false);
end;

procedure TSheetAxisCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode, SummaryNode: IXMLDOMNode;
  sName: string;
begin
  inherited;
  for i := 0 to Count - 1 do
  begin
    if AxisType = axRow then
      sName := 'row'
    else
      sName := 'column';
    ItemNode := Node.ownerDocument.createNode(1, sName, '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;
  {настройки итогов}
  SummaryNode := Node.ownerDocument.createNode(1, attrSummaryoptions, '');
  SummaryOptions.WriteToXML(SummaryNode);
  Node.appendChild(SummaryNode);
  SummaryNode := Node.ownerDocument.createNode(1, attrGrandSummaryoptions, '');
  GrandSummaryOptions.WriteToXML(SummaryNode);
  Node.appendChild(SummaryNode);
  with (Node as IXMLDOMElement) do
  begin
    setAttribute(attrUseSummariesForElements, BoolToStr(UseSummariesForElements));
    setAttribute(attrBroken, BoolToStr(Broken));
    setAttribute(attrReverseOrder, BoolToStr(ReverseOrder));
    setAttribute(attrMPBefore, BoolToStr(MPBefore));
    setAttribute(attrHideEmpty, BoolToStr(HideEmpty));
    setAttribute(attrSummaryOptimization, BoolToStr(SummaryOptimization));
    setAttribute(attrUseIndents, BoolToStr(UseIndents));
    setAttribute(attrLevelsFormatting, BoolToStr(LevelsFormatting));
  end;

end;

function TSheetAxisCollection.GetCollectionName: string;
begin
  if AxisType = axRow then
    result := 'rows'
  else
    result := 'columns';
end;

function TSheetAxisCollection.GetFieldCount: integer;
var i: integer;
begin
  result := 0;
  if Empty then
    exit;
  if Broken then
    result := 1
  else
    for i := 0 to Count - 1 do
      result := result + Items[i].FieldCount;
end;

function TSheetAxisCollection.MemberPropertiesAsStringList: TStringList;
var
  i, j: integer;
begin
  //*** as - ы левые
   result := TStringList.Create;
   for i := 0 to Count - 1 do
     for j := 0 to (Items[i] as TSheetAxisElement).MemberProperties.Count - 1 do
       if (Items[i] as TSheetAxisElement).MemberProperties[j].Checked then
         result.Add((Items[i] as TSheetAxisElement).MemberProperties[j].Name);
end;

function TSheetAxisCollection.FindByID(ID: string): integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Count - 1 do
    if Items[i].UniqueID = ID then
    begin
      result := i;
      break;
    end;
end;

function TSheetAxisCollection.FindByDimension(DimName: string): TSheetAxisElementInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Dimension = DimName then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetAxisCollection.FindByDimAndHier(DimName,
  HierName: string): TSheetAxisElementInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].Dimension = DimName) and (Items[i].Hierarchy = HierName) then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetAxisCollection.Validate: boolean;
var
  i, ErrorCode: integer;
  DetailText, AxisStr: string;
begin
  result := true;
  if Empty then
    exit;
  if AxisType = axRow then
    Owner.OpenOperation(pfoRowsValidate, not CriticalNode, not NoteTime, otProcess)
  else
    Owner.OpenOperation(pfoColumnsValidate, not CriticalNode, not NoteTime, otProcess);
  for i := 0 to Count - 1 do
    if not Items[i].Validate(DetailText, ErrorCode) then
    begin
      result := false;
      if Items[i].Orientation = axRow then
        AxisStr := '- элемент строк "'
      else
        AxisStr := '- элемент столбцов "';
      Owner.PostMessage(AxisStr + Items[i].FullDimensionName2 +
          '": ' + DetailText + ';', msgWarning);
    end;
  Owner.CloseOperation;
end;

function TSheetAxisCollection.AllFromCube(CubeName: string): boolean;
var
  Cube: TCube;
  i: integer;
begin
  result := false;

  {Если провайдеры не совпадают, то результат очевиден}
  for i := 0 to Count - 2 do
    if Items[i].ProviderId <> Items[i + 1].ProviderId then
      exit;

  Cube := Owner.XMLCatalog.Cubes.Find(CubeName, Items[0].ProviderId);
  if not Assigned(Cube) then
    exit;
  for i := 0 to Count - 1 do
    if not Cube.DimAndHierInCube(Items[i].Dimension, Items[i].Hierarchy) then
      exit;
  result := true;
end;


procedure TSheetAxisElement.LoadLevelsFromCatalog;
var
  Hier: THierarchy;
  i: integer;
  Lvl: TSheetLevelInterface;
begin
  if not SheetInterface.XMLCatalog.Loaded then
    exit;
  Hier := CatalogHierarchy;
  if not Assigned(Hier) then
    exit;
  Levels.Clear;
  for i := 0 to Hier.Levels.Count - 1 do
  begin
    Lvl := Levels.Append;
    Lvl.Name := Hier.Levels[i].Name;
  end;
end;

function TSheetAxisCollection.Refresh(Force: boolean): boolean;
var
  i: integer;
begin
  result := true;

  if Empty then
    exit;
  if AxisType = axRow then
    Owner.OpenOperation(pfoRowsRefresh, not CriticalNode, not NoteTime, otUpdate)
  else
    Owner.OpenOperation(pfoColumnsRefresh, not CriticalNode, not NoteTime, otUpdate);
  for i := 0 to Count - 1 do
  begin
    if Items[i].IsParam then
      if not Items[i].IsBaseParam then
        continue;
    if Items[i].Refresh(Force) then
      CutAllInvisible(Items[i].Members, true)
    else
      result := false;  
  end;

  Owner.CloseOperation;
end;

function TSheetAxisCollection.GetMPCheckedCount: integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to Count - 1 do
    result := result + Items[i].MemberProperties.GetCheckedCount;
end;

function TSheetAxisCollection.GetFullAxisDOM: IXMLDOMDocument2;

  {удаляет итоги с пустыми Юникнэймами}
  procedure RemoveBadSummaries(Dom: IXMLDOMDocument2);

      procedure RemoveSummary(SumNode: IXMLDOMNode);
      var
        Node, ParentNode: IXMLDOMNode;
      begin
        if SumNode.hasChildNodes then
          Node := SumNode.selectSingleNode('.//*[not(*)]')
        else
          Node := SumNode;
        repeat
          ParentNode := Node.parentNode;
          Node := ParentNode.removeChild(Node);
          Node := nil;
          Node := ParentNode;
        until (ParentNode.nodeName = 'Members') or (GetNodeType(ParentNode) = ntMember);
      end;
      
  var
    NL: IXMLDOMNodeList;
    i: integer;
    GrandTitle: string;
  begin
    if not Assigned(Dom) then
      exit;
    {удаляем все итоги, у которых где-либо в цепи узлов присутстсвуют пустые юникнеймы}
(*    NL := Dom.selectNodes('function_result/Members//Summary[@unique_name=""]');*)
    NL := Dom.selectNodes('function_result/Members//Summary[@unique_name=""]' +
      ' | function_result/Members//Summary[.//Member[@unique_name=""]]');
    for i := 0 to NL.length - 1 do
      RemoveSummary(NL[i]);
    GrandTitle := GrandSummaryOptions.Title;
    EncodeXPathString(GrandTitle);
    NL := Dom.selectNodes(Format('function_result/Members//Summary[@name!="%s"]',
      [GrandTitle]));
    for i := 0 to NL.length - 1 do
      if (NL[i].parentNode.childNodes.length = 1) and not (Owner.TableProcessingMode = tpmLarge) then
        RemoveSummary(NL[i]);
  end;

    {дописывает к узлу подчиненную коллекцию, содержащую
      юникнеймы - "координаты" для процедуры размещения данных.
      Только для случая с разрушенной иерархией всей оси.}
    procedure CreateUNamesInfo(Node: IXMLDOMNode; AxisIndex: integer);
    var
      ParentalNode, InfoNode: IXMLDOMNode;
      i, CurAxisIndex: integer;
      UName: string;
    begin
      if not Assigned(Node) then
        exit;
      {поскольку после разрушения иерархии не будет ни пустышек ни саммари, то
        для них и делать ничего не надо}
      if (GetNodeType(Node) <> ntMember) and not IsGrandSummary(Node) then
        exit;
      {собственная координата}
      UName := GetStrAttr(Node, attrUniqueName, '');
      InfoNode := Node.ownerDocument.createNode(1, ntAliasInfo, '');
      (InfoNode as IXMLDOMElement).setAttribute(Items[AxisIndex].Alias, UName);
      Node.appendChild(InfoNode);
      {точные координаты вышестоящих узлов}
      ParentalNode := Node.parentNode;
      CurAxisIndex := AxisIndex;
      repeat
        while ParentalNode.nodeName <> 'Members' do
        begin
          if GetIntAttr(ParentalNode, attrAxisIndex, 0) <> CurAxisIndex then
            break;
          ParentalNode := ParentalNode.parentNode;
        end;
        if ParentalNode.nodeName <> 'Members' then
        begin
          CurAxisIndex := GetIntAttr(ParentalNode, attrAxisIndex, 0);
          UName := GetStrAttr(ParentalNode, attrUniqueName, '');
          InfoNode := Node.ownerDocument.createNode(1, ntAliasInfo, '');
          (InfoNode as IXMLDOMElement).setAttribute(Items[CurAxisIndex].Alias, UName);
          Node.appendChild(InfoNode);
        end;
      until ParentalNode.nodeName = 'Members';
      {AllMember-ы в качестве остальных координат}
      for i := AxisIndex + 1 to Count - 1 do
//!!        if Items[i].AllMember <> '' then
        begin
          InfoNode := Node.ownerDocument.createNode(1, ntAliasInfo, '');
          (InfoNode as IXMLDOMElement).setAttribute(Items[i].Alias, Items[i].AllMember);
          Node.appendChild(InfoNode);
        end;
    end;

  //добавление общего итога
  procedure GrandSummary(DOM: IXMLDOMDocument2; ImperativeSummaries: boolean);
  var
    Node, Parent, GSNode: IXMLDOMNode;
    Name, UniqueName: string;
    i, j, LCount: integer;
    EmptyGrandSummary: boolean;
  begin
    if Empty then
      exit;
    EmptyGrandSummary := false;
    for i := 0 to Count - 1 do
    begin
      if Items[i].IgnoreHierarchy then
        LCount := 1
      else
        LCount := Items[i].Levels.Count;
      for j := 0 to LCount - 1 do
      begin
        if (i = 0) and (j = 0) then
        begin
          Parent := DOM.SelectSingleNode('function_result/Members');
          Node := Dom.createNode(1, ntSummary, '');
          Name := GrandSummaryOptions.Title; (*stGrand;*)
          (Node as IXMLDOMElement).setAttribute(attrName, Name);
          GSNode := Node;
        end
        else
          Node := Dom.createNode(1, ntMember, '');
        if j = 0 then //новое измерение - нужно вписать AllMember
          UniqueName := Items[i].AllMember;
        if (UniqueName = '') and not EmptyGrandSummary then
          EmptyGrandSummary := true;
        (Node as IXmlDomElement).setAttribute(attrUniqueName, UniqueName);
        (Node as IXmlDomElement).setAttribute(attrChecked, 'true');

        if (i = 0) and (j = 0) and (GrandSummaryOptions.Deployment = idTop) then
          Parent := Parent.insertBefore(Node, Parent.firstChild)
        else
          Parent := Parent.appendChild(Node);
      end;
    end;
    if Broken then
      CreateUNamesInfo(GSNode, 0);

    {удаление дублирующего промежуточного итога}
    if not EmptyGrandSummary and SummaryOptimization and not HideEmpty then
    begin
      Parent := DOM.SelectSingleNode('function_result/Members');
      if (DOM.selectNodes('function_result/Members/Member').length = 1) and
        not ImperativeSummaries then
      try
        Node := DOM.selectSingleNode('function_result/Members/Member/Summary');
        if Assigned(Node) then
        begin
          Parent := Node.parentNode;
          Node := Parent.removeChild(Node);
          Node := nil;
        end;
      except
      end;
    end;
  end;

  procedure SetIndexes(Dom: IXMLDOMDocument2);
  var
    AxisIndex, ParentAxisIndex, SiblingAxisIndex, LevelIndex, i, LCount: integer;
    NL: IXMLDOMNodeList;
    XPath, Pattern: string;
    Sibling: IXMLDOMNode;
  begin
    Pattern := '';
    for AxisIndex := 0 to Count - 1 do
    begin
      LCount := IIF(Items[AxisIndex].IgnoreHierarchy, 1,
        Items[AxisIndex].Levels.Count);
      for LevelIndex := 0 to LCount - 1 do
      begin
        Pattern := Pattern + '/*';
        XPath := 'function_result/Members' + Pattern + '[not(@axisindex)]';
        NL := Dom.selectNodes(XPath);
        for i := 0 to NL.length - 1 do
        begin
          (NL[i] as IXMLDomElement).setAttribute(attrAxisIndex, AxisIndex);
          (NL[i] as IXMLDomElement).setAttribute(attrLevelIndex, LevelIndex);
        end;
      end;
    end;
    NL := Dom.selectNodes('//Summary[@name="Итоги"]');
    for i := 0 to NL.length - 1 do
    begin
      {найдем соседа на том же уровне}
      Sibling := NL[i].previousSibling;
      if not Assigned(Sibling) then
        Sibling := NL[i].nextSibling;
      ParentAxisIndex := GetIntAttr(NL[i].parentNode, attrAxisIndex, -1);
      {такое может быть в случае скрытия пустых строк и наличия итогов из базы}
      if not Assigned(Sibling) then
        SiblingAxisIndex := ParentAxisIndex
      else
        SiblingAxisIndex := GetIntAttr(Sibling, attrAxisIndex, -1);

      if SiblingAxisIndex = ParentAxisIndex then
      begin //внутриосевой итог
        LevelIndex := GetIntAttr(NL[i].parentNode, attrLevelIndex, -1);
        (NL[i] as IXMLDomElement).setAttribute(attrAxisIndex, ParentAxisIndex);
        (NL[i] as IXMLDomElement).setAttribute(attrLevelIndex, LevelIndex);
      end;
(* итоги на стыке осей добавляются специальной процедурой JointSummaries,
  а потому просто не могут не иметь правильно проставленных индексов.
      else
      begin //итог на стыке осей
        LevelIndex := GetIntAttr(Sibling, attrLevelIndex, -1);
        (NL[i] as IXMLDomElement).setAttribute(attrAxisIndex, SiblingAxisIndex);
        (NL[i] as IXMLDomElement).setAttribute(attrLevelIndex, LevelIndex);
      end;*)
    end;
  end;

  {удаление балансирующих пустышек - требуется в случае разрушенной иерархии
    всей оси}
  procedure RemoveDummies(AxisDom: IXMLDOMDocument2);
  var
    i: integer;
    NL: IXMLDOMNodeList;
    Node: IXMLDOMNode;
  begin
    if not Assigned(AxisDom) then
      exit;
    NL := AxisDom.selectNodes('function_result/Members//Member[not(@name)]');
    for i := 0 to NL.length - 1 do
    begin
      Node := NL[i].parentNode.removeChild(NL[i]);
      Node := nil;
    end;
  end;

  {проставляет мемберам имена для разметки в листе}
  procedure SetRangeNames(AxisDom: IXMLDOMDocument2);

    function GetDummyRangeName(Node: IXMLDOMNode): string;
    var
      RangeName, tmpStr: string;
    begin
      result := '';
      tmpStr := snSeparator + IIF(IsBelongsToSummary(Node), fpEnd, fpLeafEnd);
      repeat
        Node := Node.parentNode;
        if Node.nodeName = 'Members' then
          exit;
        RangeName := GetStrAttr(Node, attrRangeName, '');
        if RangeName = '' then
          tmpStr := snSeparator + fpDummy + tmpStr;
      until RangeName <> '';
      result := RangeName + tmpStr;
    end;

    procedure SetRangeName(Node: IXMLDOMNode; ParentLocalId: string;
      CheckLeafSum: integer; const AxisCount: integer);
    var
      RangeName, AxisUniqueId, LocalId: string;
      AxisIndex, i, LeafIndex: integer;
      NodeType: string;
      IsLeaf: boolean;
    begin
      RangeName := '';
      if not Assigned(Node) then
        exit;
      NodeType := GetNodeType(Node);
      if (NodeType = ntAliasInfo) then
        exit;
      AxisIndex := GetIntAttr(Node, attrAxisIndex, -1);
      if NodeType = ntMember then
      begin //реальный мембер
        if AxisIndex = -1 then
          AxisUniqueId := 'error'
        else
          AxisUniqueId := Items[AxisIndex].UniqueID;
        LocalId := '';
        if (ParentLocalId <> '') then
          LocalId := ParentLocalId + snSeparator;
        LocalId := LocalId + GetStrAttr(Node, attrLocalId, 'error');

        { А вот этот фрагмент отвечает за определение того, подойдет ли
          мембер для обратной записи, т.е. является ли он истинно листовым.}
        LeafIndex := GetIntAttr(Node, attrMemberLeaf, -1);
        if LeafIndex = AxisIndex then
          inc(CheckLeafSum);
        IsLeaf :={ not Node.hasChildNodes and }(CheckLeafSum = AxisCount);

        // получаем имя диапазона
        RangeName := ConstructName(sntMember, AxisUniqueId, BoolToStr(IsLeaf), LocalId);
        if Broken then
          CreateUNamesInfo(Node, AxisIndex);
      end
      else //саммари или пустышка
        // размечаем общие итоги
        if GetStrAttr(Node, 'name', '') = GrandSummaryOptions.Title (*stGrand*) then
          case AxisType of
            axRow: RangeName := snNamePrefix + snSeparator + gsRow;
            axColumn: RangeName := snNamePrefix + snSeparator + gsColumn;
          end
        else
          if (not Node.hasChildNodes) then //отсекли нелистовые элементы
            RangeName := GetDummyRangeName(Node);

      //сохраним имя в узле ДОМа для дальнейшего использования
      (Node as IXMLDOMElement).setAttribute(attrRangeName, RangeName);
      if LocalId = '' then
        LocalId := ParentLocalId + snSeparator + fpDummy;

      if Node.selectNodes('Member').length > 0 then
      begin
        for i := 0 to Node.childNodes.length - 1 do
          SetRangeName(Node.childNodes[i], LocalId, CheckLeafSum, AxisCount);
        if Broken then
          SetAttr(Node, attrWbWorthy, BooltoStr(Pos('_true_', RangeName) > 0));
      end
      else
        SetAttr(Node, attrWbWorthy, BooltoStr(Pos('_true_', RangeName) > 0));
    end;

  var
    i: integer;
    NL: IXMLDOMNodeList;
  begin
    if not Assigned(AxisDom) then
      exit;
    NL := AxisDom.selectNodes('function_result/Members/*');
    for i := 0 to NL.length - 1 do
      SetRangeName(NL[i], '', 0, Count);
  end;

  {Добавляет в собранную ось итоги на стыках измерений}
  procedure JointSummaries(Dom: IXMLDOMDocument2; ImperativeSummaries: boolean);
  var
    i, AxisIndex, LevelIndex, SlashCount: integer;
    ParentNode, Node, Dummy: IXMLDOMNode;
    XPath, UName, SummaryName: string;
    NL, DirectChildren: IXMLDOMNodeList;
    FieldCountArray: array of integer;
    LevelSummaryOptions: TSummaryOptions;
    SummaryAxisIndex, SummaryLevelIndex: integer;
    DummyAxisIndex, DummyLevelIndex: integer;
  begin
    SetLength(FieldCountArray, Count);
    for i := 0 to Count - 1 do
      FieldCountArray[i] := GetInternalFieldCount(Items[i]);

    SlashCount := 0;
    {идем по всем межэлементным стыкам внутри оси}
    for AxisIndex := 0 to Self.Count - 2 do
    begin
      {если измерение иерархическое, то стыковой итог управляется
        настройками его последнего уровня, если же иерархия разрушена,
        то настройки нужно будет брать индивидуально для каждого элемента}
      if not Items[AxisIndex].IgnoreHierarchy then
      begin
        LevelSummaryOptions := Items[AxisIndex].Levels[FieldCountArray[AxisIndex] - 1].SummaryOptions;
        if not LevelSummaryOptions.Enabled then
          continue;
      end;

      SlashCount := SlashCount + FieldCountArray[AxisIndex];
      XPath := Copy(FunnyStr, 1, 2*(SlashCount - 1));
      XPath := Format('function_result/Members%s/Member[.//Member[@name] and not (Summary[@name])]', [XPath]);
      NL := Dom.selectNodes(XPath);
      for i := 0 to NL.length - 1 do
      begin
        {если иерархия разрушена, то настройки итогов берем
          индивидуально для каждого элемента}
        if Items[AxisIndex].IgnoreHierarchy then
        begin
          LevelIndex := GetIntAttr(NL[i], attrLevelIndex, 0);
          LevelSummaryOptions := Items[AxisIndex].Levels[LevelIndex].SummaryOptions;
          if not LevelSummaryOptions.Enabled then
            continue;
        end;

        DirectChildren := NL[i].selectNodes('./Member[@name]');
        if (DirectChildren.length > 1) or (DirectChildren.length = 0) or
          ImperativeSummaries or (not SummaryOptimization) then
        begin
          {если узел, к которому надо клеить, это пустышка, то ищем его прародителя}
          if GetStrAttr(NL[i], attrName, '') = '' then
          begin
            ParentNode := GetRealParent(NL[i]);
            Uname := GetStrAttr(ParentNode, attrUniqueName, '');
            SummaryAxisIndex := GetIntAttr(ParentNode, attrAxisIndex, 0);
            SummaryLevelIndex := GetIntAttr(ParentNode, attrLevelIndex, 0);
            LevelSummaryOptions :=
              Items[SummaryAxisIndex].Levels[SummaryLevelIndex].SummaryOptions;
            DummyAxisIndex := SummaryAxisIndex;
            DummyLevelIndex := SummaryLevelIndex + 1;
          end
          else
          begin
            {самый обычный случай стыкового итога - клеим к реальному мемберу
              последнего уровня, оба индекса берем от него же}
            ParentNode := NL[i];
            UName := Items[AxisIndex + 1].AllMember;
            SummaryAxisIndex := AxisIndex;
            if Items[AxisIndex].IgnoreHierarchy then
              SummaryLevelIndex := GetIntAttr(NL[i], attrLevelIndex, 0)
            else
              SummaryLevelIndex := FieldCountArray[AxisIndex] - 1;
            LevelSummaryOptions :=
              Items[SummaryAxisIndex].Levels[SummaryLevelIndex].SummaryOptions;
            DummyAxisIndex := AxisIndex + 1;
            DummyLevelIndex := 0;
          end;
          if UName = '' then
            continue;
          if not LevelSummaryOptions.Enabled then
            continue;

          Node := Dom.createNode(1, ntSummary, '');

          SummaryName := LevelSummaryOptions.GetCaption(GetStrAttr(ParentNode, attrName, ''));

          (Node as IXMLDOMElement).setAttribute(attrName, SummaryName);
          (Node as IXmlDomElement).setAttribute(attrUniqueName, UName);
          (Node as IXmlDomElement).setAttribute(attrChecked, 'true');
          (Node as IXmlDomElement).setAttribute(attrAxisIndex, SummaryAxisIndex);
          (Node as IXmlDomElement).setAttribute(attrLevelIndex, SummaryLevelIndex);
          if LevelSummaryOptions.Deployment = idTop then
            ParentNode.insertBefore(Node, ParentNode.firstChild)
          else
            ParentNode.appendChild(Node);
          ParentNode := Node;
          {добалансируем пустышками}
          while not ((DummyAxisIndex = Count - 1) and
            (DummyLevelIndex = FieldCountArray[DummyAxisIndex] - 1)) do
          begin
            if DummyLevelIndex = FieldCountArray[DummyAxisIndex] - 1 then
            begin
              inc(DummyAxisIndex);
              DummyLevelIndex := 0;
              UName := Items[DummyAxisIndex].AllMember;
            end
            else
              inc(DummyLevelIndex);
            Dummy := Dom.createNode(1, ntMember, '');
            (Dummy as IXmlDomElement).setAttribute(attrUniqueName, UName);
            (Dummy as IXmlDomElement).setAttribute(attrChecked, 'true');
            (Dummy as IXmlDomElement).setAttribute(attrAxisIndex, DummyAxisIndex);
            (Dummy as IXmlDomElement).setAttribute(attrLevelIndex, DummyLevelIndex);
            ParentNode := ParentNode.appendChild(Dummy);
          end;
        end;
      end;
    end;
  end;

  procedure NumerateNodes(Dom: IXMLDOMDocument2);
  var
    i: integer;
    NL: IXMLDOMNodeList;
  begin
    NL := Dom.selectNodes('function_result/Members//Member | function_result/Members//Summary');
    for i := 0 to NL.length - 1 do
      SetAttr(NL[i], attrNodeId, i);
  end;

var
  AxisDOM: IXMLDOMDocument2;
  FileName: string;
  ImperativeSummaries: boolean;
begin
  result := nil;
  // смотрим не пустая ли она
  if Count <= 0 then
    exit;

  {если хоть какие-то показатели берутся из базы, то промежуточные итоги
    необходимы даже элементам с одним потомком.}
  ImperativeSummaries := Owner.Totals.AreSummariesImperative;

  Owner.OpenOperation(
    IIF(AxisType = axRow, pfoGetFullRowAxis, pfoGetFullColumnAxis),
    CriticalNode, NoteTime, otProcess);
  try
    if ((HideEmpty and (AxisType = axRow)))
      and (Owner.TableProcessingMode = tpmLarge) and Assigned(Owner.TotalsData) then
      AxisDom := GetLightenAxisDom(ImperativeSummaries)
    else
      AxisDom := GetRegularAxisDom(ImperativeSummaries);

    if not Broken then
      JointSummaries(AxisDom, ImperativeSummaries);
    if GrandSummaryOptions.Enabled then
      GrandSummary(AxisDom, ImperativeSummaries);

    SetIndexes(AxisDOM);
    RemoveBadSummaries(AxisDom);
    SetRangeNames(AxisDom);
                      
    {при необходимости разрушим иерархию для всей оси}
    if Broken then
    begin
      BreakHierarchy(AxisDom, ReverseOrder);
      RemoveDummies(AxisDom);
    end;

    NumerateNodes(AxisDOM);

    result := AxisDOM;
    Owner.CloseOperation; //pfoGetFullColumnAxis
  except
    Owner.PostMessage(pfoOperationFailed, msgError);
  end;
  {Если нужно сохраняем в лог}
  if AddinLogEnable then
  begin
    if  AxisType = axRow then
      FileName := 'Ось_строк.xml'
    else
      FileName := 'Ось_столбцов.xml';
    Owner.OpenOperation(pfoLogSave + AddinLogPath + FileName, not CriticalNode,
      not NoteTime, otSave);
    if WriteDocumentLog(result, FileName) then
      Owner.CloseOperation
    else
      begin
        Owner.PostMessage(pfoLogSaveFailed + AddinLogPath + FileName, msgWarning);
        Owner.CloseOperation;
      end;
  end;
end;

function TSheetAxisElement.GetObjectType: TSheetObjectType;
begin
  if (GetOrientation = axRow) then
    result := wsoRow
  else
    result := wsoColumn;
end;

function TSheetAxisElement.GetObjectTypeStr: string;
begin
  if (GetOrientation = axRow) then
    result := 'Элемент строк'
  else
    result := 'Элемент столбцов';
end;

procedure TSheetAxisElement.RecreateLevelsByMembers;
  function GetOldLevelsNode: IXMLDOMNode;
  var
    BuckupDOM: IXMLDOMDocument2;
    DOMElement: IXMLDOMElement;
  begin

    if GetDomDocument(BuckupDOM) then
    try
      {Сохраняем}
      DOMElement := BuckupDOM.createElement('levels');
      result := BuckupDOM.appendChild(DOMElement);
      Levels.WriteToXML(result);
    finally
      //KillDomDocument(BuckupDOM);
    end;
  end;
var
  i: integer;
  DOMNode, OldLevelsNode: IXMLDOMNode;
  Hier: THierarchy;
  SheetLevel: TSheetLevelInterface;
  LevelName, XPath, ShortXPath: string;
  DOM: IXMLDOMDocument2;
begin
  if Assigned(Members) then
  try
    ShortXPath := '//level[@' + attrID + ' and (name="%s")]';
    XPath := 'function_result/Levels/Level[@%s="%s"]';
    OldLevelsNode := GetOldLevelsNode;
    Levels.Clear;

    Hier := CatalogHierarchy;
    if Assigned(Hier) then
      for i := 0 to Hier.Levels.Count -1 do
      begin
        LevelName := Hier.Levels[i].Name;
        {Посмотрим, ести ли такой уровень в данных}
        DOMNode := Members.selectSingleNode(Format(XPath, [attrName, LevelName]));
        if Assigned(DOMNode) then
        begin //нужно добавлять по любому
          SheetLevel := Levels.Append;
          SheetLevel.Name := LevelName;
          {А теперь посмотрим, если такой уровень был и раньше, тогда нужно
            загрузить его данные из бэкапа}
          DOMNode := OldLevelsNode.selectSingleNode(Format(ShortXPath, [LevelName]));
          if Assigned(DOMNode) then
            SheetLevel.ReadFromXML(DomNode);
        end;
      end;
  finally
    DOM := (OldLevelsNode.ownerDocument as IXMLDOMDocument2);
    KillDOMDocument(DOM);
  end;
end;

function TSheetAxisElement.GetCommentText: string;
begin
  result := 'Измерение "' + FullDimensionName2 + '"';
  if IsParam then
  begin
    result := result + #10 + 'Параметр "' + Param.Name + '"';
    if Param.IsInherited then
      result := result + ' (от родительской задачи)';
  end;
end;

function TSheetAxisElement.GetMdxText: string;
var
  Generator: TMDXSetGenerator;
  ForceAllMember, NeedCheckParents: boolean;
begin
  Generator := TMDXSetGenerator.Create;
  ForceAllMember := true;//SheetInterface.TableProcessingMode <> tpmHuge;
  NeedCheckParents := (SheetInterface.TableProcessingMode <> tpmHuge) or
    ((SheetInterface.TableProcessingMode = tpmHuge) and
    not (IgnoreHierarchy or (Owner as TSheetAxisCollectionInterface).Broken));
  Generator.Generate(Self, ForceAllMember{true}, NeedCheckParents, result);
  Generator.Free;
end;

procedure TSheetAxisElement.ApplyStyles;
var
  ERange: ExcelRange;
  i: integer;
begin
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if Assigned(ERange) then
  begin
    ERange.Style := 'Normal';
    ERange.Style := IIF(SheetInterface.PrintableStyle,
      Styles.Name[esValuePrint], Styles.Name[esValue]);
  end;
  for i := 0 to Levels.Count - 1 do
  begin
    ERange := GetRangeByName(SheetInterface.ExcelSheet, Levels[i].TitleExcelName);
    if Assigned(ERange) then
    begin
      ERange.Style := 'Normal';
      ERange.Style := IIF(SheetInterface.PrintableStyle,
        Styles.Name[esTitlePrint], Styles.Name[esTitle]);
    end;
  end;
end;

function TSheetAxisElement.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFieldPosition;
    esValueprint: result := snFieldPositionPrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

function TSheetAxisCollection.GetCommentText: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Count - 1 do
  begin
    AddTail(result, #10#10);
    result := result + Items[i].CommentText;
  end;
end;

function GetIndecomposableTotals(AxisElement: TSheetAxisElementInterface): TStringList;
var
  i: integer;
begin
  result := TStringList.Create;
  for i := 0 to AxisElement.SheetInterface.Totals.Count - 1 do
    if not AxisElement.SheetInterface.Totals[i].FactorizedBy(AxisElement) then
      result.Add(AxisElement.SheetInterface.Totals[i].Alias);
end;

procedure LocalRemoveEmpty(AxisElement: TSheetAxisElementInterface; AxisDom: IXMLDOMDocument2);
var
  i: integer;
  NoIndecomposableTotals, MustDie: boolean;
  NL: IXMLDOMNodeList;
  UName, XPath, Alias: string;
begin
  {Если есть показатели, неразложимые по этому измерению, и для них есть
    данные, то измерение войдет в ось целиком}
  if CheckForIndecomposableData(AxisElement, NoIndecomposableTotals) then
    exit;
  {неразложимых показателей вообще нет, т.е. все раскладываются.
    В этом случае все просто - есть данные по мемберу - он живет, нет - дохнет}
//  if NoIndecomposableTotals then
  begin
    NL := AxisDom.selectNodes('function_result/Members//Member');
    Alias := AxisElement.Alias;
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      EncodeXPathString(UName);
      XPath := Format('function_result/data/row[@%s="%s"]', [Alias, UName]);
      MustDie := not Assigned(AxisElement.SheetInterface.TotalsData.selectSingleNode(XPath));
      (NL[i] as IXMLDOMElement).setAttribute('mustdie', BoolToStr(Mustdie));
    end;
    KillMembersThatMustDie(GetInternalFieldCount(AxisElement), AxisDom);
  end
end;

function CheckForIndecomposableData(AxisElement: TSheetAxisElementInterface; out NoSuchTotals: boolean): boolean;
var
  IndecomposableTotals: TStringList;
  i: integer;
  XPath: string;
  Total: TSheetTotalInterface;
begin
  result := false;
  IndecomposableTotals := GetIndecomposableTotals(AxisElement);
  NoSuchTotals := IndecomposableTotals.Count = 0;
  try
    for i := 0 to IndecomposableTotals.Count - 1 do
    begin
      Total := AxisElement.SheetInterface.Totals.FindByAlias(IndecomposableTotals[i]);
      if Total.IsGrandTotalDataOnly then
        continue;
      XPath := Format('function_result/data/row[@%s]', [IndecomposableTotals[i]]);
      if Assigned(AxisElement.SheetInterface.TotalsData.selectSingleNode(XPath)) then
      begin
        result := true;
        exit;
      end;
    end;
  finally
    FreeStringList(IndecomposableTotals);
  end;
end;

procedure KillMembersThatMustDie(FieldCount: integer; AxisDom: IXMLDOMDocument2);
var
  i, j: integer;
  Node: IXMLDOMNode;
  NL, ChildrenNL: IXMLDOMNodeList;
begin
  {Атрибут MustDie уже должен быть проставлен где нужно.
    Теперь поднимаемся по уровням оси вверх и вычеркиваем элементы,
    у которых все непосредственные потомки "мастдай"}
  for i := FieldCount - 2 downto 0 do
  begin
    NL := AxisDom.selectNodes('function_result/Members' +
      Copy(FunnyStr, 1, 2 * (i + 1)));
    for j := 0 to NL.length - 1 do
    begin
      if not NL[j].hasChildNodes then
        continue;
      ChildrenNL := NL[j].selectNodes('./Member[(@mustdie="false") or not (@mustdie)]' +
        ' | ./Summary[(@mustdie="false") or not (@mustdie)]');
      case ChildrenNL.length of
        0: (NL[j] as IXMLDOMElement).setAttribute('mustdie', 'true');
        1:
          begin
            (NL[j] as IXMLDOMElement).setAttribute('mustdie', 'false');
           Node := NL[j].selectSingleNode('./Summary');
            if Assigned(Node) then
              (Node as IXMLDOMElement).setAttribute('mustdie', 'false');
          end;
      else
        (NL[j] as IXMLDOMElement).setAttribute('mustdie', 'false');
      end;
    end;
  end;       

  {а вот теперь разом убиваем их всех :)}
  NL := AxisDom.selectNodes('function_result/Members//*[@mustdie="true"]');
  for i := NL.length - 1 downto 0 do
  begin
    Node := NL[i].parentNode.removeChild(NL[i]);
    Node := nil;
  end;
end;

{Альтернативная склейка оси с отбраковкой пустых строк "на лету".
  Идея заключается в том, чтобы собирать ось из цепочек узлов,
  описывающих полностью детализированный XPath для листовых элементов ОСИ,
  т.е. содержащих юникнеймы узлов для каждого уровня оси.

  Цепочки формируются итерационно, по пути от самого первого измерения оси
  до самого последнего. На каждом шаге, т.е. когда к цепочке добавляется
  участок от очередного измерения, проводится проверка наличия данных в
  датадоме показателей, которые имеют в адресных атрибутах соответствущие
  цепочке значения. Если данных нет, то дальнейшая детализация пути смысла
  не имеет и цепочка исключается из рассмотрения. Поэтому режим тем эффективнее,
  чем в более ранних измерениях будут встречены элементы без данных.}

function TSheetAxisCollection.GetFullComboAxis(
  Parts: array of IXMLDOMDocument2): IXMLDOMDocument2;

type

  NodeRec = record
    UName: string;
    Node: IXMLDOMNode;
  end;
  ChainArray = array of NodeRec;

  NodeListRec = record
    MembersNL, SummariesNL: IXMLDOMNodeList;
    MembersCount: integer;
  end;
  TNodeListArray = array of NodeListRec;

var
  DecompTotals, IndecompTotals: TStringList;

  {имея координаты листьев в каждом элементе оси,
    соберем полную цепь узлов с первого до последнего уровня}
  procedure GetFullChain(CoordList: TStringList; var FullChain: ChainArray;
    var ChainSize, FirstLevel: integer);
  var
    i, AxisIndex: integer;
    UName, NodeType: string;
    StubMode: boolean;
    Node: IXMLDOMNode;
  begin
    ChainSize := 0;
    for i := 0 to Count - 1 do
      ChainSize := ChainSize + GetInternalFieldCount(Items[i]);
    SetLength(FullChain, ChainSize);

    FirstLevel := 0;
    StubMode := false;
    for AxisIndex := 0 to Count - 1 do
    begin
      Node := IXMLDOMNode(Pointer(CoordList.Objects[AxisIndex]));
      if not StubMode then
      begin
        NodeType := GetNodeType(Node);
        for i := GetInternalFieldCount(Items[AxisIndex]) - 1 downto 0 do
        begin
          UName := GetStrAttr(Node, attrUniqueName, '');
          if UName = '' then
          begin
            ChainSize := -1;
            exit;
          end;
          EncodeXPathString(UName);
          FullChain[FirstLevel + i].UName := UName;
          FullChain[FirstLevel + i].Node := Node;
          Node := Node.parentNode;
        end;
        FirstLevel := FirstLevel + GetInternalFieldCount(Items[AxisIndex]);
        StubMode := (NodeType = ntSummary) or (NodeType = ntSummaryDummy);
      end
      else
      begin
        UName := Items[AxisIndex].AllMember;
        if UName = '' then
        begin
          ChainSize := -1;
          exit;
        end;
        for i := 0 to GetInternalFieldCount(Items[AxisIndex]) - 1 do
        begin
          FullChain[FirstLevel + i].UName := UName;
          FullChain[FirstLevel + i].Node := nil;
        end;
      end;
    end;
  end;

  function GetSummaryParentXPath(FullChain: ChainArray): string;
  var
    i: integer;
    XPath: string;
  begin
    XPath := 'function_result/Members';
    for i := 0 to Length(FullChain) - 1 do
    begin
      if FullChain[i].Node.nodeName = ntSummary then
        break;
      XPath := XPath +Format('/*[@%s="%s"]' ,[attrUniqueName, FullChain[i].UName]);
    end;
    result := XPath;
  end;

  procedure AddResultNode(CoordList: TStringList; ProblemSummaryMode: boolean);
  var
    Node, ParentNode: IXMLDOMNode;
    i, j, LevelIndex, FirstLevel, ChainSize: integer;
    XPath: string;
    FullChain: ChainArray;
    ItsaSummary, SummariesFirst: boolean;
  begin
    GetFullChain(CoordList, FullChain, ChainSize, FirstLevel);
    if ChainSize = -1 then
      exit;

    if ProblemSummaryMode then
    begin
      XPath := GetSummaryParentXPath(FullChain);
      ParentNode := result.selectSingleNode(XPath);
      if not Assigned(ParentNode) then
        exit;
    end;

    {идем по цепочке сверху вниз и ищем место, с которого надо клеить}
    XPath := 'function_result/Members';
    ParentNode := result.selectSingleNode(XPath);
    LevelIndex := -1;
    for i := 0 to ChainSize - 1 do
    begin
      if i = FirstLevel then
        break;
      XPath := XPath +Format('/*[@%s="%s"]' ,[attrUniqueName, FullChain[i].UName]);
      Node := result.selectSingleNode(XPath);
      if not Assigned(Node) then
      begin
        LevelIndex := i;
        break;
      end;
      ParentNode := Node;
    end;

    if LevelIndex = -1 then
      exit;

    ItsaSummary := FullChain[LevelIndex].Node.nodeName = ntSummary;
    SummariesFirst := false;
    if ItsaSummary then
    begin
      i := GetIntAttr(FullChain[LevelIndex].Node, attrAxisIndex, -1);
      j := GetIntAttr(FullChain[LevelIndex].Node, attrLevelIndex, -1);
      if (i <> -1) and (j <> -1) then
        SummariesFirst := Items[i].Levels[j].SummaryOptions.Deployment = idTop;
    end;

    {в этот момент ParentNode - это узел в результирующем ДОМ-е, к которому
      нужно подклеивать все последующие узлы цепочки, начиная с LevelIndex.
      Начиная с FirstLevel клеим пустышки}
    for i := LevelIndex to ChainSize - 1 do
    begin
      if i < FirstLevel then
      begin
        Node := FullChain[i].Node;
        Node := Node.cloneNode(false);
      end
      else
      begin
        Node := ParentNode.ownerDocument.createNode(1, ntMember, '');
        (Node as IXmlDomElement).setAttribute(attrUniqueName, FullChain[i].UName);
        (Node as IXmlDomElement).setAttribute(attrChecked, 'true');
      end;
      if (i = LevelIndex) and ItsaSummary and SummariesFirst then
        ParentNode.insertBefore(Node, ParentNode.firstChild)
      else
        ParentNode.appendChild(Node);
      ParentNode := Node;
    end;
  end;

  procedure ProcessAxisElement(NodeListArray: TNodeListArray; AxisIndex: integer; var CoordList: TStringList;
     LeafCondition: string; TotalMembersCount: integer; var ProgressCounter: integer);
  var
    i: integer;
    NL: IXMLDOMNodeList;
    UName, XPath: string;
    RowNode: IXMLDOMNode;
  begin
    if AxisIndex = Count then
      exit;

    {перебираем все листовые элементы измерения.
      Для каждого элемента составляем условие выборки записей данных
      с учетом их предков из вышестоящих измерений - клеим те самые цепочки.
      Если данных по такому частному условию нет, то глубже можно не копать.
      А если есть, то либо идем вниз, к следующему измерению в оси, либо
      добавляем полученную цепь узлов в результирующий датадом оси.}
    NL := NodeListArray[AxisIndex].MembersNL;
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      if UName = '' then
        continue;
      CoordList[AxisIndex] := UName;
      CoordList.Objects[AxisIndex] := TObject(Pointer(NL[i]));
      XPath := GetCoordXPath(CoordList, DecompTotals, IndecompTotals, LeafCondition, AxisIndex);
      RowNode := Owner.TotalsData.selectSingleNode(XPath);
      if not Assigned(RowNode) then
        continue;
      if AxisIndex < Count - 1 then
        ProcessAxisElement(NodeListArray, AxisIndex + 1, CoordList, LeafCondition, TotalMembersCount, ProgressCounter)
      else
      begin
        AddResultNode(CoordList, false);
        inc(ProgressCounter);
        Owner.SetPBarPosition(ProgressCounter, TotalMembersCount);
      end;
    end;

    NL := NodeListArray[AxisIndex].SummariesNL;
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      if UName = '' then
        continue;
      CoordList[AxisIndex] := UName;
      CoordList.Objects[AxisIndex] := TObject(Pointer(NL[i]));
      XPath := GetCoordXPath(CoordList, DecompTotals, IndecompTotals, LeafCondition, AxisIndex);
      RowNode := Owner.TotalsData.selectSingleNode(XPath);
      if AxisIndex < Count - 1 then
        ProcessAxisElement(NodeListArray, AxisIndex + 1, CoordList, LeafCondition, TotalMembersCount, ProgressCounter)
      else
        AddResultNode(CoordList, not Assigned(RowNode));
    end;
  end;

var
  CoordList: TStringList;
  i, j, TotalMembersCount, ProgressCounter: integer;
  GrandTotalsOnly, LeafCondition: string;
  LevelsNL: IXMLDOMNodeList;
  LevelRootNode: IXMLDOMNode;
  NodeListArray: TNodeListArray;
begin
  DecompTotals := TStringList.Create;
  IndecompTotals := TStringList.Create;

  result := InitXmlDocument;
  result.DocumentElement := result.createElement('function_result');
  LevelRootNode := result.documentElement.appendChild(result.createNode(1, 'Levels', ''));
  result.documentElement.appendChild(result.createNode(1, 'Members', ''));

  SetLength(NodeListArray, Count);
  CoordList := TStringList.Create;
  TotalMembersCount := Owner.TotalsData.selectNodes('function_result/data/row[@IsRowLeaf="true"]').length;

  {Для каждого измерения оси получим списки листовых элементов и саммари.
  Итоговые цепочки будут формироваться из кусков по измерениям, каждый из которых
  будет путем от этих элементов к корню. Юникнеймы этих элементов являются
  значениями адресных атрибутов в записях xml-a данных.}
  for i := 0 to Count - 1 do
  begin
    CoordList.Add('');
    LevelsNL := Parts[i].selectNodes('function_result/Levels/Level[@name]');
    for j := 0 to LevelsNL.length - 1 do
      LevelRootNode.appendChild(LevelsNL[j].cloneNode(true));
    NodeListArray[i].MembersNL :=  Parts[i].selectNodes('function_result/Members//Member[(not (Member)) and (not (Summary)) and (not(@SummaryDummy))]');
    NodeListArray[i].SummariesNL := Parts[i].selectNodes('function_result/Members//Summary[(not (Member)) and (not (Summary))]' +
      ' | function_result/Members//Member[(not (Member)) and (not (Summary)) and (@SummaryDummy)]');
    NodeListArray[i].MembersCount := NodeListArray[i].MembersNL.length;
  end;

  LeafCondition := Owner.Totals.GetLeafCondition(true, true);

  try
    Owner.GetFactorization(Self, false, DecompTotals, IndecompTotals, GrandTotalsOnly);
    ProgressCounter := 0;
    ProcessAxisElement(NodeListArray, 0, CoordList, LeafCondition, TotalMembersCount, ProgressCounter);
  finally
    FreeStringList(DecompTotals);
    FreeStringList(IndecompTotals);
    FreeStringList(CoordList);
  end;
end;

{составляет XPath для выборки узлов данных для построения оси без пустых строк.
  CoordList содержит юникнеймы листовых мемберов и ссылки на соответствующие узлы,
  Decomposable и Indecomposable - алиасы раскладываемых и нераскладываемых показателей}
function TSheetAxisCollection.GetCoordXPath(CoordList, Decomposable,
  Indecomposable: TStringList; LeafCondition: string; Limit: integer): string;
var
  i: integer;
  PartCondition3, Condition, UName: string;
begin
  result := '';
  PartCondition3 := '';
  for i := 0 to Limit do
  begin
    UName := CoordList[i];
    EncodeXPathString(UName);

    Condition := GetTotalsDataCondition(Items[i].Alias, UName,
      Decomposable[i], InDecomposable[i]);

    {Частное условие для отдельных показателей внутри таблицы}
    AddTail(PartCondition3, ' and ');
    PartCondition3 := PartCondition3 + Format('(@%s="%s")',
        [Items[i].Alias, UName]);

    AddTail(result, ' and ');
    result := result + Condition;
  end;
  result := result + Format(' or (%s and @%s and @inTable)', [PartCondition3, attrAlias]);
  result := Format('(%s) and (%s)', [result, LeafCondition]);
  result := Format('function_result/data/row[%s]', [result]);
end;

{ добавляет в дерево элементы-итоги
Итог добавляется к реальным элементам (не пустышкам),
у которых есть не менее 2-х таких же реальных потомков}
procedure TSheetAxisCollection.Summaries(Dom: IXmlDomDocument2; ItemIndex: integer;
  ImperativeSummaries: boolean);
var
  MNL, DirectChildren: IXMLDOMNodeList;
  i, LevelIndex: integer;
  Node: IXMLDOMNode;
  Name, UniqueName: string;
  Level: TSheetLevelInterface;
begin
  MNL := Dom.selectNodes('function_result/Members//Member[(@name) and .//Member[@name] and not (Summary[@name])]');
  for i := 0 to MNL.length - 1 do
  begin
    DirectChildren := MNL[i].selectNodes('./Member[@name]');
    if (DirectChildren.length > 1) or (DirectChildren.length = 0) or
      ImperativeSummaries or (not SummaryOptimization) then
    begin
      LevelIndex := GetIntAttr(MNL[i], attrLevelIndex, -1);
      if LevelIndex = -1 then
        continue;
      Level := Items[ItemIndex].Levels[LevelIndex];
      if not Level.SummaryOptions.Enabled then
        continue;

      UniqueName := GetStrAttr(MNL[i], attrUniqueName, '');
      Node := Dom.createNode(1, ntSummary, '');
      Name := Level.SummaryOptions.GetCaption(GetStrAttr(MNL[i], attrName, ''));
      (Node as IXMLDOMElement).setAttribute(attrName, Name);
      (Node as IXmlDomElement).setAttribute(attrUniqueName, UniqueName);
      (Node as IXmlDomElement).setAttribute(attrChecked, 'true');
      (Node as IXmlDomElement).setAttribute(attrAxisIndex, ItemIndex);
      {Важный момент: итог маркируется тем же уровнем, что и его
        родительский элемент.}
      (Node as IXmlDomElement).setAttribute(attrLevelIndex, LevelIndex);
      if Level.SummaryOptions.Deployment = idTop then
        MNL[i].insertBefore(Node, MNL[i].firstChild)
      else
        MNL[i].appendChild(Node);
    end;
  end;
end;

procedure TSheetAxisCollection.BalanceAxisElement(Dom: IXMLDOMDocument2; AxisIndex: integer);
var
  i, j, LevelIndex, MaxLevel: integer;
  XPath, UniqueName, PkId: string;
  NL: IXMLDOMNodeList;
  Dummy, ParentNode: IXMLDOMNode;
  NodeChecked, IsSummaryDummy: boolean;
  MemberLeaf: integer;
begin
  MaxLevel := Dom.selectSingleNode('function_result/Levels').childNodes.length - 1;
  XPath := 'function_result/Members';
  for LevelIndex := 0 to MaxLevel - 1 do
  begin
    XPath := XPath + '/*';
    NL := Dom.selectNodes(XPath + '[not(*)]');
    for i := 0 to NL.length - 1 do
    begin
      ParentNode := NL[i];
      UniqueName := GetStrAttr(NL[i], attrUniqueName, '');
      NodeChecked := GetBoolAttr(NL[i], attrChecked, false);
      IsSummaryDummy := NL[i].nodeName = ntSummary;
      MemberLeaf := GetIntAttr(NL[i], attrMemberLeaf, -1);
      PkId := GetStrAttr(NL[i], attrPkId, 'null');
      for j := LevelIndex + 1 to MaxLevel do
      begin
        Dummy := Dom.createNode(1, ntMember, '');
        (Dummy as IXmlDomElement).setAttribute(attrUniqueName, UniqueName);
        (Dummy as IXmlDomElement).setAttribute(attrChecked, BoolToStr(NodeChecked));
        (Dummy as IXmlDomElement).setAttribute(attrAxisIndex,  AxisIndex);
        (Dummy as IXmlDomElement).setAttribute(attrLevelIndex,  j);
        if IsSummaryDummy then
          (Dummy as IXmlDomElement).setAttribute(ntSummaryDummy, 1);
        if MemberLeaf > -1 then
          (Dummy as IXmlDomElement).setAttribute(attrMemberLeaf, MemberLeaf);
        if j = MaxLevel then
          (Dummy as IXmlDomElement).setAttribute(attrPkId, PkId);
        ParentNode := ParentNode.appendChild(Dummy);
      end;
    end;
  end;
end;

procedure TSheetAxisCollection.MergeXML(FirstDOM, SecondDOM: IXMLDOMDocument2; AxisIndex: integer);
var
  NL, NL2: IXMLDOMNodeList;
  LevelNode, LevelRootNode, Dummy, ParentNode: IXMLDOMNode;
  i, j, k, LevelsCount: integer;
begin
  // перекинем перечень уровней из второго элемента в первый
  LevelRootNode := FirstDOM.selectSingleNode('function_result/Levels');
  NL := SecondDOM.selectNodes('function_result/Levels/Level[@name]');
  LevelsCount := NL.length;
  if Assigned(NL) and Assigned(LevelRootNode) then
    for i := 0 to LevelsCount - 1 do
    begin
      LevelNode := NL[i].cloneNode(true);
      LevelRootNode.appendChild(LevelNode);
    end;

  NL := FirstDOM.selectNodes('function_result/Members//*[not(*)]');
  NL2 := SecondDOM.selectSingleNode('function_result/Members').childNodes;
  for i := 0 to NL.length - 1 do
    if IsBelongsToSummary(NL[i]) then
    begin
      ParentNode := NL[i];
      for k := 0 to LevelsCount - 1 do
      begin
        Dummy := FirstDom.createNode(1, ntMember, '');
        (Dummy as IXmlDomElement).setAttribute(attrUniqueName, Items[AxisIndex].AllMember);
        (Dummy as IXmlDomElement).setAttribute(attrChecked, 'true');
        (Dummy as IXmlDomElement).setAttribute(attrAxisIndex,  AxisIndex);
        (Dummy as IXmlDomElement).setAttribute(attrLevelIndex,  k);
        (Dummy as IXmlDomElement).setAttribute(ntSummaryDummy, 1);
        ParentNode := ParentNode.appendChild(Dummy);
      end;
    end
    else
      for j := 0 to NL2.length - 1 do
        NL[i].appendChild(NL2[j].cloneNode(true));
end;

procedure TSheetAxisCollection.GetDomNormalized(Dom: IXMLDOMDocument2; AxisIndex: integer;
  ImperativeSummaries: boolean);
var
  XPath, UName: string;
  NL: IXMLDOMNodeList;
  i: integer;
begin
  {выкидываем из дерева исключаемые мемберы (influence = neExclude)}
  CutAllInvisible(DOM, false);

  {проставим специальный признак листовым элементам;
    данный признак будет использоваться при проверке возможности выполнить
    обратную запись на этот элемент.
    При нетронутой иерархии листовые элементы те, что не имеют потомков.
    Если иерархия сломана, то на элемент не должно быть ссылок
    как на родительский через атрибут ParentUN.
    Если сломана вся ось, то работаем все равно по первому варианту.}
  if not Items[AxisIndex].IgnoreHierarchy or Broken then
  begin
    XPath := 'function_result/Members//Member[not(*)]';
    NL := Dom.selectNodes(XPath);
    for i := 0 to NL.length - 1 do
      (NL[i] as IXMLDOMElement).setAttribute(attrMemberLeaf, AxisIndex);
  end
  else
  begin
    NL := Dom.selectNodes('function_result/Members//Member');
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      EncodeXPathString(UName);
      XPath := Format('Member[@%s="%s"]', [attrParentUN, UName]);
      if not Assigned(NL[i].parentNode.selectSingleNode(XPath)) then
        (NL[i] as IXMLDOMElement).setAttribute(attrMemberLeaf, AxisIndex);
    end;
  end;

  if not(Broken or Items[AxisIndex].IgnoreHierarchy) then
    Summaries(DOM, AxisIndex, ImperativeSummaries);
  BalanceAxisElement(DOM, AxisIndex);
end;

function TSheetAxisCollection.GetRegularAxisDom(ImperativeSummaries: boolean): IXMLDOMDocument2;
var
  PartDom: IXMLDOMDocument2;
  i: integer;
begin
  GetDomDocument(result);
  GetDOMDocument(PartDOM);

  Items[0].LoadWithIndices(result);
  Items[0].RemoveAndRenameDataMembers(result);
  //если свойство выставлено в true значит разрушаем иерархию
  if Items[0].IgnoreHierarchy and not Broken then
    BreakHierarchy(result, Items[0].ReverseOrder);

  GetDomNormalized(result, 0, ImperativeSummaries);

  for i := 1 to Count - 1  do
  begin
    Items[i].LoadWithIndices(PartDom);
    Items[i].RemoveAndRenameDataMembers(PartDOM);
    if Items[i].IgnoreHierarchy  and not Broken then
      BreakHierarchy(PartDom, Items[i].ReverseOrder);
    GetDomNormalized(Partdom, i, ImperativeSummaries);
    MergeXML(result, PartDOM, i);
  end;
  KillDOMDocument(PartDOM);
end;

function TSheetAxisCollection.GetLightenAxisDom(
  ImperativeSummaries: boolean): IXMLDOMDocument2;
var
  AxisArray: array of IXMLDOMDocument2;
  i: integer;
  NeedLocalRemove: boolean;
begin
  SetLength(AxisArray, Count);
  {нужно ли включать механизм локального (поэлементного) удаления пустых строк}
  NeedLocalRemove := (Count > 1) and (AxisType = axRow) and HideEmpty;
  try
    for i := 0 to Count - 1 do
    begin
      GetDOMDocument(AxisArray[i]);
      Items[i].LoadWithIndices(AxisArray[i]);
      if Items[i].IgnoreHierarchy  and not Broken then
        BreakHierarchy(AxisArray[i], Items[i].ReverseOrder);
      GetDomNormalized(AxisArray[i], i, ImperativeSummaries);
      {отключено, чтобы не тормозило лишний раз}
      if NeedLocalRemove and false then
        LocalRemoveEmpty(Items[i], AxisArray[i]) ;
    end;
    result := GetFullComboAxis(AxisArray);
  finally
    for i := 0 to Count - 1 do
      KillDomDocument(AxisArray[i]);
    SetLength(AxisArray, 0);
  end;
end;

function GetInternalFieldCount(Elem: TSheetAxisElementInterface): integer;
begin
  if TSheetAxisCollectionInterface(Elem.Owner).Broken then
    result := Elem.Levels.Count
  else
    result := Elem.FieldCount;
end;

function TSheetAxisCollection.FindByFullDimensionName(FullDimensionName: string): TSheetAxisElementInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].FullDimensionName = FullDimensionName) then
    begin
      result := Items[i];
      break;
    end;
end;

function TSheetAxisCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue: result := 'Элементы';
    esValuePrint: result := 'Элементы (печать)';
    esTitle: result := 'Заголовок';
    esTitlePrint: result := 'Заголовок (печать)';
  end;
end;

function TSheetAxisCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFieldPosition;
    esValueprint: result := snFieldPositionPrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

procedure TSheetAxisCollection.SetDefaultStyles2All;
var
  i: integer;
begin
  SetDefaultStyles;
  for i := 0 to Count - 1 do
  begin
    Items[i].SetDefaultStyles;
    Items[i].Levels.SetDefaultStyles2All;
  end;
end;

procedure TSheetAxisCollection.Clear;
begin
  inherited;
  SummaryOptions.Free;
  SummaryOptions := TSummaryOptions.Create;
  GrandSummaryOptions.Free;
  GrandSummaryOptions := TSummaryOptions.Create;
  GrandSummaryOptions.Title := stGrand;
  FUseSummariesForElements := true;
  FBroken := false;
  FMPBefore := false;
  FHideEmpty := false;
  FSummaryOptimization := true;
end;

function TSheetAxisCollection.GetLevelNumber(AxisIndex,
  LevelIndex: integer): integer;
var
  i: integer;
begin
  result := 0;
  if (AxisIndex = -1) or (LevelIndex = -1) then
    exit;
  for i := 0 to AxisIndex - 1 do
    result := result + Items[i].Levels.Count;
  result := result + LevelIndex;
end;

function TSheetAxisCollection.GetLevelIndent(AxisIndex,
  LevelIndex: integer): integer;
var
  i: integer;
begin
  {Отступы используются только при сломанной иерархии
    Если иерархия сломана для всей оси, то отступы сквозные для всех измерений.
    И инкремент отступов общий от 0 до N.
    Если иерархия сломана только для отдельных изменений, то
    отступы используются/не используются для каждого измерения в отдельности.
    И инкремент отступов индивидуальный от 0 до N}
  result := 0;
  if (AxisIndex = -1) or (LevelIndex = -1) then
    exit;
  if not (Broken or Items[AxisIndex].IgnoreHierarchy) then
    exit;
  result := LevelIndex;
  if Broken then
    for i := AxisIndex - 1 downto 0 do
      result := result + Items[i].Levels.Count;
  if result > 15 then
    result := 15;
end;

function TSheetAxisCollection.GetLevelIndent(
  Level: TSheetLevelInterface): integer;
var
  AxisIndex, LevelIndex: integer;
begin
  LevelIndex := Level.GetSelfIndex;
  AxisIndex := Level.AxisElement.GetSelfIndex;
  result := GetLevelIndent(Axisindex, LevelIndex);
end;

function TSheetAxisCollection.GetMarkupFieldCount: integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to Count - 1 do
    if Broken then
      result := result + Items[i].Levels.Count
    else
      result := result + Items[i].FieldCount;
end;

function GetParentNode(Node: IXMLDOMNode): IXMLDOMNode;
var
  XPath, UName: string;
begin
  if Assigned(Node.attributes.getnamedItem(attrParentUN)) then
  begin
    UName := GetStrAttr(Node, attrParentUN, '');
    EncodeXPathString(UName);
    XPath := Format('./Member[@%s="%s"]', [attrUniqueName, UName]);
    result := Node.parentNode.selectSingleNode(XPath);
  end
  else
    result := Node.parentNode;
end;

function GetTotalsDataCondition(
  Alias: string; // Алиас измерения - элемента оси
  UName: string; // Имя мембера в этом измерении
  Decomposable: string; //  Алиасы раскладываемых по измерению показателей
  Indecomposable: string //  Алиасы нераскладываемых по измерению показателей
  ): string;
var
  PartCondition1, PartCondition2: string;
begin
  {частное условие для показателей, раскладываемых по элементу оси}
  if Decomposable <> '' then
    PartCondition1 := Format('((@%s="%s") and %s)',
      [Alias, UName, Decomposable]);

  {частное условие для показателей, НЕ раскладываемых по элементу оси}
  if Indecomposable <> '' then
    PartCondition2 := Format('(%s)', [Indecomposable]);

  if PartCondition1 <> '' then
    if PartCondition2 <> '' then
      result := '(' + PartCondition1 + ' or ' + PartCondition2 + ')'
    else
      result := PartCondition1
  else
    result := PartCondition2;
end;

function GetDataSelectionXPath(
  Aliases: TStringList;
  UNames: TStringList;
  Decomposables: TStringList;
  Indecomposables: TStringList;
  LeafCondition: string
): string;
var
  i: integer;
  TotalsCondition, CellsCondition, PartCondition: string;
begin
  TotalsCondition := '';
  CellsCondition := '';
  for i := 0 to Aliases.Count - 1 do
  begin
    PartCondition := GetTotalsDataCondition(Aliases[i], UNames[i],
      Decomposables[i], Indecomposables[i]);
    if PartCondition <> '' then
    begin
      AddTail(TotalsCondition, ' and ');
      TotalsCondition := TotalsCondition + PartCondition;
    end;

    AddTail(CellsCondition, ' and ');
    CellsCondition := CellsCondition +
      Format('(@%s="%s")', [Aliases[i], UNames[i]]);
  end;
  result := TotalsCondition;
  if LeafCondition <> '' then
  begin
    AddTail(result, ' and ');
    result := TupleBrackets(result + LeafCondition);
  end;
  AddTail(result, ' or ');
  result := result + Format('(%s and @%s and @inTable)', [CellsCondition, attrAlias]);
  result := Format('function_result/data/row[%s]', [result]);
end;

procedure TSheetAxisElement.LoadWithIndices(Dom: IXMLDOMDocument2);
var
  LevelIndex, i, SelfIndex: integer;
  NL: IXMLDOMNodeList;
  XPath: string;
begin
  SelfIndex := GetSelfIndex;
  Dom.load(GetOrQueryMembers);
  XPath := 'function_result/Members';
  for LevelIndex := 0 to Levels.Count - 1 do
  begin
    XPath := XPath + '/*';
    NL := Dom.selectNodes(XPath);
    for i := 0 to NL.length - 1 do
    begin
      (NL[i] as IXMLDomElement).setAttribute(attrAxisIndex, SelfIndex);
      (NL[i] as IXMLDomElement).setAttribute(attrLevelIndex, LevelIndex);
    end;
  end;
end;

function TSheetAxisElement.GetOnDeleteWarning: string;
begin
  result := IIF(Orientation = axRow, qumDelElementRows, qumDelElementColumns)
    + '"' + GetElementCaption + '"?';
end;

procedure TSheetAxisElement.MoveDM(Dom: IXMLDOMDocument2; NL: IXMLDOMNodeList; Deployment: TItemDeployment);
var
  List: TStringList;
  Id: string;
  i: integer;
  Node, ParentNode, TargetNode: IXMLDOMNode;
begin
  List := TStringList.Create;
  try
    for i := 0 to NL.length - 1 do
      if IsDataMember(NL[i]) then
      begin
        Id := GetStrAttr(NL[i], attrLocalId, '');
        if Id = '' then
          continue;
        List.Add(Id);
      end;

    for i := 0 to List.Count - 1 do
    begin
      Id := List[i];
      Node := Dom.selectSingleNode(Format('function_result/Members//*[@%s="%s"]',
        [attrLocalId, Id]));
      ParentNode := Node.parentNode;
      ParentNode.removeChild(Node);
      case Deployment of
        idTop: TargetNode := ParentNode.firstChild;
        idBottom: TargetNode := ParentNode.selectSingleNode('/Summary');
      end;
      if Assigned(TargetNode) then
        ParentNode.insertBefore(Node, TargetNode)
      else
        ParentNode.appendChild(Node);
    end;
  finally
    FreeStringList(List);
  end;
end;


procedure TSheetAxisElement.RemoveAndRenameDataMembers(Dom: IXMLDOMDocument2);
var
  i, j, IndexStar: integer;
  NL: IXMLDOMNodeList;
  Node: IXMLDOMNode;
  XPath: string;
  TempCustomDMTitle, StringWithNewData: widestring;
begin
  XPath := 'function_result/Members';
  for i := 0 to Levels.Count - 1 do
  begin
    XPath := XPath + '/Member';
    NL := Dom.selectNodes(XPath);

    {Удаление датамемберов уровня}
    if Levels[i].HideDataMembers then
    begin
      for j := 0 to NL.length - 1 do
        if IsDataMember(NL[j]) then
        begin
          Node := NL[j].parentNode.removeChild(NL[j]);
          Node := nil;
        end;
      continue;
    end
    else
      MoveDM(Dom, NL, Levels[i].DMDeployment);

    {Переименование}
    if Levels[i].UseCustomDMTitle then
    begin
      for j := 0 to NL.length - 1 do
        if IsDataMember(NL[j]) then
        begin
          // Смотрим: есть ли в введённых ДАННЫХ '*'
          TempCustomDMTitle := Levels[i].CustomDMTitle;
          IndexStar := Pos('*', TempCustomDMTitle);
          // Если есть, то меняем:
          if IndexStar > 0 then
          begin
            { берём имя родительского узла датамембера }
            StringWithNewData := GetStrAttr(NL[j].parentNode, attrName, '');
            // заменяем '*' на новую строку -- имя родителя датамембера
            Delete(TempCustomDMTitle, IndexStar, 1);
            Insert(StringWithNewData, TempCustomDMTitle, IndexStar);
            (NL[j] as IXMLDOMElement).setAttribute(
              attrName, TempCustomDMTitle);
          end
          else (NL[j] as IXMLDOMElement).setAttribute(
                 attrName, Levels[i].CustomDMTitle);
        end;
    end;
  end;
end;

function TSheetAxisCollection.FindByAlias(aStr: string): TSheetAxisElementInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Alias = aStr then
    begin
      result := Items[i];
      break;
    end;
end;

end.
