{
  Фильтры листа планирования (объект + коллекция).
}

unit uSheetFilters;

interface
uses
  Classes, SysUtils, MSXML2_TLB, uXMLUtils, uFMExcelAddinConst,
  uFMAddinGeneralUtils, uFMAddinExcelUtils, uSheetObjectModel,
  uXMLCatalog, ExcelXP, PlaningTools_TLB, uFMAddinXMLUtils, uSheetParam,
  uGlobalPlaningConst, uExcelUtils, uSheetMemberProperties;

type
  {фильтр}
  TSheetFilter = class(TSheetFilterInterface)
  private
    {При переносе в фильтры элемента строк/столбцов часть уровней может быть
      отключена. Надо включить}
    (*procedure CheckUncheckedLevels;*)
  protected
    {геттеры и сеттеры (обитатели гетто и их собаки :)}
    function GetIsMultiple: boolean; override;
    function GetText: string; override;
    function GetCommentText: string; override;
    function GetExcelName: string; override;
    function GetScopeText: string; override;
    function GetMdxText: string; override;
    function GetMdx: string;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    procedure SetScope(AValue: TStringList); override;
    function GetAlias: string; override;
    procedure RecreateLevelsByMembers; override;
    function GetOwningCell: TSheetSingleCellInterface; override;
  public
    constructor Create(AOwner: TSheetCollection);
    destructor Destroy; override;
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;
    //действует ли фильтр на показатель
    function IsAffectsTotal(Total: TSheetTotalInterface): boolean; override;
    //на какие _типы_ показателей действует фильтр
    function TotalTypesAffected: integer; override;
    //проверяет содержимое на соответствие каталогу
    function Validate(out MsgText: string; out ErrorCode: integer): boolean; override;
    // получить тип элемента листа
    function GetObjectType: TSheetObjectType; override;
    // получить строковое описание типа элемента листа
    function GetObjectTypeStr: string; override;
    procedure ApplyStyles; override;
    function CheckForWriteback(out Error: string): boolean; override;
    function GetMPStrings: TStringList; override;
    function WithDefaultValue(out DefaultNode: IXMLDOMNode): boolean; override;
    function GetOnDeleteWarning: string; override;
    function GetFilterDescription(AdditionalDetails: boolean): string; override;

    property MdxText: string read GetMdxText;
  end;

  {коллекция фильтров}
  TSheetFilterCollection = class(TSheetFilterCollectionInterface)
  private
    FOwningCell: TSheetSingleCellInterface;
  protected
    function GetItem(Index: integer): TSheetFilterInterface; override;
    procedure SetItem(Index: integer; Item: TSheetFilterInterface); override;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
    function GetOwningCell: TSheetSingleCellInterface; override;
  public
    constructor Create(Cell: TSheetSingleCellInterface); override;
    //обновляет все данные (Members-ы всех фильтров);
    function Refresh(Force: boolean): boolean; override;
    //добавляет элемент в коллекцию
    function Append: TSheetFilterInterface; override;
    //удаляет элемент из коллекции
    procedure Delete(Index: integer); override;
    //загружает коллекцию из DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    //записывает коллекцию в XML
    procedure WriteToXML(Node: IXMLDOMNode); override;

    //возвращает имя коллекции в соответствии с типом ее элементов
    function GetCollectionName: string; override;
    //возвращает индекс элемента в коллекции
    function FindByID(ID: string): integer; override;
    //проверяет содержимое на соответствие каталогу
    function Validate: boolean; override;
    //есть ли фильтр с такими параметрами и не с таким ИД
    function IsThereSuchFilter(DimName, HierName: string;
      Total: TSheetTotalInterface; ExceptId: string): TSheetFilterInterface; override;
    //ищет одинаковые фильтры, действующие на один и тот же показатель
    function IsThereDuplicateFilters(DimName, HierName, ExceptId: string;
      Scope: TStringList; out Msg: string): boolean; override;
    function FindByDimAndHier(DimName, HierName: string): TSheetFilterInterface; override;
    function FindByFullDimensionName(FullDimensionName: string): TSheetFilterInterface; override;
  end;

implementation

uses uCheckTV2;

function GetExcludedChildren(Node: IXMLDOMNode): IXMLDOMNodeList;
begin
  result := nil;
  if not Assigned(Node) then
    exit;
  result := Node.selectNodes(Format('Member[@%s="%d"]', [attrInfluence, ord(neExclude)]));
end;

{************** TSheetFilter implementation ***************}

constructor TSheetFilter.Create(AOwner: TSheetCollection);
begin
  inherited Create(AOwner);
  MemberProperties := TSheetMPCollection.Create(Self);
  SetDefaultStyles;
end;

destructor TSheetFilter.Destroy;
begin
  inherited Destroy;
end;

procedure TSheetFilter.ReadFromXML(Node: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i, Pid: integer;
  Id: string;
  IsParam, IsParamRead: boolean;
begin
  inherited ReadFromXML(Node);
  if not Assigned(Node) then
    exit;
  Id := GetStrAttr(Node, attrID, '');
  if SheetInterface.InCopyMode then
    SheetInterface.CopyCustomProperty(Id, UniqueId)
  else
    UniqueID := Id;
  IsPartial := boolean(abs(GetIntAttr(Node, attrPartial, 0)));
  Dimension := GetNodeStrAttr(Node, attrDimension);
  Hierarchy := GetNodeStrAttr(Node, attrHierarchy);
  if IsPartial then //если частный, то настроим область действия
  begin
    if not Assigned(Scope) then
      Scope := TStringList.Create;
    Scope.Clear;
    NL := Node.selectNodes('scope/total');
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], AttrID, '');
      Scope.Add(Id);
    end;
  end;
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
    end
    else
      if SheetInterface.InCopyMode then
        Param.SetLink(Self);
  end;
  if not (lmNoMembers in SheetInterface.LoadMode) then
    Members := GetMembers;
  if IsParam and not IsParamRead then
  begin
    Param.RemoveLink(Self);
    Param.Free;
    Param := nil;
  end;
end;

procedure TSheetFilter.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ParamNode, ScopeNode, TotalNode, RootNode: IXMLDOMNode;
begin
  inherited WriteToXml(Node);
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrPartial, IsPartial);
    SetNodeStrAttr(Node, attrDimension, Dimension);
    SetNodeStrAttr(Node, attrHierarchy, Hierarchy);
    if IsPartial then
    begin
      ScopeNode := Node.ownerDocument.createNode(1, 'scope', '');
      Node.appendChild(ScopeNode);
      for i := 0 to Scope.Count - 1 do
      begin
        TotalNode := Node.ownerDocument.createNode(1, 'total', '');
        (TotalNode as IXMLDOMElement).setAttribute(attrID, Scope.Strings[i]);
        ScopeNode.appendChild(TotalNode);
      end;
    end;
    {запись коллекции мембер пропертиз}
    RootNode := Node.ownerDocument.createNode(1, 'properties', '');
    MemberProperties.WriteToXML(RootNode);
    Node.appendChild(RootNode);
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

function TSheetFilter.IsAffectsTotal(Total: TSheetTotalInterface): boolean;
{Фильтр действует на показатель типа "мера" или типа "результат"
куба, в котором есть измерение фильтрации; если фильтр частный, то ИД показателя
должен присутствовать в области действия фильтра.}
begin
  result := false;
  if (Total.TotalType in [wtFree, wtConst]) then
    exit;

  if IsPartial then
    result := Scope.IndexOf(Total.UniqueID) >= 0
  else
  begin
    if not Assigned(Total.Cube) then
      exit;//!!заглушка
    if ProviderId <> Total.ProviderId then
      exit;
    result := Total.Cube.DimAndHierInCube(Dimension, Hierarchy);
  end;
end;

function TSheetFilter.TotalTypesAffected: integer;
var
  TotalIndex: integer;
  Total: TSheetTotalInterface;
begin
  result := ttNone;
  for TotalIndex := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[TotalIndex];
    if IsAffectsTotal(Total) then
      case Total.TotalType of
        wtMeasure: result := result or ttMeasure;
        wtFree: result := result or ttFree;
        wtResult: result := result or ttResult;
        wtConst: result := result or ttConst;
      end;
  end;
end;

function TSheetFilter.GetText: string;
begin
  result := GetFilterDescription(false);
  if not SheetInterface.DisplayFullFilterText then
    if Pos(FSD, result) > 0 then
      result := 'Несколько элементов';
end;

function TSheetFilter.GetCommentText: string;
begin
  result := GetFilterDescription(true);
  if IsParam then
  begin
    result := result + #10 + 'Параметр "' + Param.Name + '"';
    if Param.IsInherited then
      result := result + ' (от родительской задачи)';
  end;
end;

function TSheetFilter.GetExcelName: string;
begin
  result := BuildExcelName(sntFilter + snSeparator + UniqueID);
end;

function TSheetFilter.GetScopeText: string;
var
  i: integer;
  TotalId: string;
  TotalIndex: integer;
  Total: TSheetTotalInterface;
begin
  result := '';
  if not IsPartial then
    exit;
  for i := 0 to Scope.Count - 1 do
  try
    TotalId := Scope.Strings[i];
    TotalIndex := SheetInterface.Totals.FindById(TotalId);
    Total := SheetInterface.Totals[TotalIndex];
    AddTail(result, ', ');
    result := result + Total.Caption; //можно и имя меры приписывать
  except
  end;
end;

function TSheetFilter.Validate(out MsgText: string; out ErrorCode: integer): boolean;
var
  Dim: TDimension;
  Hier: THierarchy;
  NL: IXMLDOMNodeList;
  i: integer;
  Total: TSheetTotalInterface;
begin
  result := false;
  Dim := SheetInterface.XMLCatalog.Dimensions.Find(Dimension, ProviderId);
  if not Assigned(Dim) then
  begin
    MsgText := ' измерение "' + Dimension + '" не существует';
    ErrorCode := ecNoDimension;
    exit;
  end;
  Hier := Dim.GetHierarchy(Hierarchy);
  if not Assigned(Hier) then
  begin
    MsgText := ' иерархия "' + Hierarchy + '" отсутствует в измерении "' +
      Dimension + '"';
    ErrorCode := ecNoHierarchy;
    exit;
  end;
  // проверка: если фильтр частный, то куб меры, на которую он наложен должен содержать измерение фильтра
  if IsPartial then
    for i := 0 to Scope.Count - 1 do
    begin
      // показатель помочили - пропускаем
      if (SheetInterface.Totals.FindById(Scope[i]) = -1) then
        continue;
      Total := SheetInterface.Totals[SheetInterface.Totals.FindById(Scope[i])];
      if not Assigned(Total.Cube) then
      begin
        MsgText := ' ошибка показателя "' + Total.MeasureName + '"';
        ErrorCode := ecNoCube;
        exit;
      end;
      if not Total.Cube.DimAndHierInCube(Dimension, Hierarchy) then
      begin
        MsgText := ' фильтр наложен на меру "' + Total.MeasureName + '" куба "' + Total.Cube.Name +
                   '", который не содержит измерение "' + Dimension + '.' + Hierarchy + '"';
        ErrorCode := ecNoMeasure;
        exit;
      end;
    end;
  // проверка на выбор хотя бы одного элемента
  NL := SelectNodes(Members, '//' + ntMember + '[@checked="true"]');
  if not (Assigned(NL) and (NL.length > 0)) then
  begin
    MsgText := ' не выбрано ни одного элемента';
    ErrorCode := ecNoSelection;
    exit;
  end;
  // прошли все проверки успешно
  result := true;
end;

function TSheetFilter.GetMdxText: string;
var
  DefaultNode: IXMLDOMNode;
begin
  if WithDefaultValue(DefaultNode) then
    result :=  GetStrAttr(DefaultNode, attrUniqueName, '')
  else
    result := GetMdx;//GetMDXEnumeration(Members, moFilter);
end;

(*procedure TSheetFilter.CheckUncheckedLevels;
var
  NL: IXMLDOMNodeList;
  i: integer;
begin
  if not Assigned(Members) then
    exit;
  NL := Members.selectNodes('function_result/Levels/Level');
  for i := 0 to NL.length - 1 do
    if GetIntAttr(NL[i], attrLevelState, 0) = 0 then
      (NL[i] as IXMLDOMElement).setAttribute(attrLevelState, 1);
end;  *)

{********** TSheetFilterCollection implementation **********}

function TSheetFilterCollection.GetItem(Index: integer): TSheetFilterInterface;
begin
  result := Get(Index);
end;

procedure TSheetFilterCollection.SetItem(Index: integer; Item: TSheetFilterInterface);
begin
  Put(Index, Item);
end;

function TSheetFilterCollection.Append: TSheetFilterInterface;
begin
  result := TSheetFilter.Create(Self);
  inherited Add(result);
  result.PermitEditing := PermitEditing;
end;

procedure TSheetFilterCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

procedure TSheetFilterCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Filter: TSheetFilterInterface;
begin
  inherited;
  if not Assigned(Node) then
    exit;

  NL := Node.selectNodes('filter');
  for i := 0 to NL.length - 1 do
  begin
    Filter := Append;
    Filter.ReadFromXML(NL[i]);
  end;
  NL := nil;
end;

procedure TSheetFilterCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  inherited;
  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'filter', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;
end;

function TSheetFilterCollection.GetCollectionName: string;
begin
  result := 'filters';
end;

function TSheetFilterCollection.FindByID(ID: string): integer;
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

function TSheetFilterCollection.Validate: boolean;
var
  i, ErrorCode: integer;
  DetailText: string;
begin
  result := true;
  if Empty then
    exit;
  Owner.OpenOperation(pfoFiltersValidate, not CriticalNode, not NoteTime, otProcess);

  for i := 0 to Count - 1 do
    if not Items[i].Validate(DetailText, ErrorCode) then
    begin
      result := false;
      Owner.PostMessage('- фильтр "' + Items[i].FullDimensionName2 + '": ' +
          DetailText + ';', msgWarning);
    end;
  Owner.CloseOperation;
end;

function TSheetFilterCollection.IsThereSuchFilter(DimName,
  HierName: string; Total: TSheetTotalInterface;
  ExceptId: string): TSheetFilterInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
  begin
    if Items[i].UniqueID = ExceptId then
      continue;
    if (Items[i].Dimension = DimName) and (Items[i].Hierarchy = HierName) then
        if Items[i].IsAffectsTotal(Total) then
        begin
          result := Items[i];
          break;
        end;
  end;
end;

function TSheetFilterCollection.IsThereDuplicateFilters(DimName, HierName,
  ExceptId: string; Scope: TStringList; out Msg: string): boolean;
var
  i: integer;
  Total: TSheetTotalInterface;
begin
  result := false;
  Msg := '';

  for i := 0 to Owner.Totals.Count - 1 do
  begin
    Total := Owner.Totals[i];

    if Assigned(Scope) then
      if Scope.IndexOf(Total.UniqueID) < 0 then
        continue;

    if Assigned(IsThereSuchFilter(DimName, HierName, Total, ExceptId)) then
    begin
      result := true;
      Msg := Format(ermDuplicateFilterForTotal, [Total.Caption]);

      break;
    end;
  end;
end;

function TSheetFilterCollection.Refresh(Force: boolean): boolean;
var
  i: integer;
begin
  result := true;
  if Empty then
    exit;

  Owner.OpenOperation(pfoFiltersRefresh, not CriticalNode,
    not NoteTime, otUpdate);
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

function TSheetFilter.GetObjectType: TSheetObjectType;
begin
  result := wsoFilter;
end;

function TSheetFilter.GetObjectTypeStr: string;
var
  i, TotalIndex: integer;
  Total: TSheetTotalInterface;
  IsMultiply: boolean;
begin
  if IsPartial then
  begin
    IsMultiply := Scope.Count > 1;
    if IsMultiply then
      result := 'Частный фильтр показателей: '
    else
      result := 'Частный фильтр показателя ';
    for i := 0 to Scope.Count - 1 do
    begin
      TotalIndex := SheetInterface.Totals.FindById(Scope.Strings[i]);
      if TotalIndex < 0 then
        continue;
      Total := SheetInterface.Totals[TotalIndex];
      result := result + '"' + Total.Caption + '" (Мера "' + Total.MeasureName + '"; Куб "' + Total.CubeName + '")';
      if IsMultiply and (i < Scope.Count - 1) then
        result := result + ', ';
    end;
  end
  else
    result := 'Фильтр таблицы';
end;

function TSheetFilterCollection.FindByDimAndHier(DimName,
  HierName: string): TSheetFilterInterface;
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

procedure TSheetFilter.ApplyStyles;
var
  ERange, ERange2: ExcelRange;
begin
  ERange := GetRangeByName(SheetInterface.ExcelSheet, ExcelName);
  if not Assigned(ERange) then
    exit;
  ERange.Style := 'Normal';
  with ERange do
  begin
    ERange2 := GetRange(SheetInterface.ExcelSheet, Row, Column, Row, Column);
    ERange2.Style := IIF(SheetInterface.PrintableStyle,
      Styles.Name[esTitlePrint], Styles.Name[esTitle]);
    ERange2 := GetRange(SheetInterface.ExcelSheet, Row + 1, Column, Row + 1, Column + Columns.Count - 1);
    ERange2.Style := IIF(SheetInterface.PrintableStyle,
      Styles.Name[esValuePrint], Styles.Name[esValue]);
  end;
end;

function TSheetFilter.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFilterValue;
    esValueprint: result := snFilterValuePrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

procedure TSheetFilter.SetScope(AValue: TStringList);
begin
  if Assigned(AValue) then
  begin
    if not Assigned(FScope) then
      FScope := TStringList.Create;
    FScope.Assign(AValue);
  end
  else
    FreeStringList(FScope);
end;

function TSheetFilterCollection.FindByFullDimensionName(FullDimensionName: string): TSheetFilterInterface;
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

function TSheetFilterCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
  case ElementStyle of
    esValue: result := 'Значение';
    esValuePrint: result := 'Значение (печать)';
    esTitle: result := 'Заголовок';
    esTitlePrint: result := 'Заголовок (печать)';
  end;
end;

function TSheetFilterCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  case ElementStyle of
    esValue: result := snFilterValue;
    esValueprint: result := snFilterValuePrint;
    esTitle: result := snFieldTitle;
    esTitlePrint: result := snFieldTitlePrint;
  end;
end;

function TSheetFilter.GetMPStrings: TStringList;
var
  i, j: integer;
  NL: IXMLDOMNodeList;
  tmpStr, MPValue, MPName, FirstValue: string;
  AllTheSame: boolean;
begin
  result := nil;
  if MemberProperties.CheckedCount = 0 then
    exit;
  NL := Members.selectNodes('function_result/Members//Member[not(*)]');
  result := TStringList.Create;
  for i := 0 to MemberProperties.Count - 1 do
  begin
    if not MemberProperties[i].Checked then
      continue;
    tmpStr := '';
    AllTheSame := true;
    for j := 0 to NL.length - 1 do
    begin
      MPName := MemberProperties[i].Name;
      EncodeMemberPropertyName(MPName);
      MPValue := GetStrAttr(NL[j], MPName, '');
      if MPValue = '' then
        MPValue := 'н/д';
      if j = 0 then
        FirstValue := MPValue
      else
        if AllTheSame then
          AllTheSame := MPValue = FirstValue;
      AddTail(tmpStr, ', ');
      tmpStr := tmpStr + MPValue;
    end;
    if AllTheSame and (NL.length > 1) then
      result.Add(FirstValue)
    else
      result.Add(tmpStr);
  end;
end;

function TSheetFilter.GetAlias: string;
begin
  result := 'F_' + UniqueId;
end;

function TSheetFilter.GetFilterDescription(AdditionalDetails: boolean): string;

  function GetPart(Node, LevelNode: IXMLDOMNode; PartName: string): string;

    function MakeExceptionSet(NL: IXMLDOMNodeList): string;
    var
      i: integer;
    begin
      result := '';
      for i := 0 to NL.length - 1 do
      begin
        AddTail(result, FSD);
        result := result + Format('  %s.[%s]', [PartName, GetStrAttr(NL[i], attrName, '')]);
      end;
    end;

  var
    NL: IXMLDOMNodeList;
    i: integer;
    MemberName, Part, ExceptionSet: string;
    Influence: TNodeInfluence;
    Checked: boolean;
  begin
    result := '';

    {если узел исключен из рассмотрения, то ни он сам, ни его потомки не
    попадают в текст запроса - можно сразу выходить}
    Influence := TNodeInfluence(GetIntAttr(Node, attrInfluence, 0));
    if Influence = neExclude then
      exit;

    Checked := GetBoolAttr(Node, attrChecked, false);
    if not HasCheckedDescendants(Node) and not Checked then
      exit;

    if (GetIntAttr(LevelNode, attrLevelState, 1) <> 0) or Checked then
    begin
      AddTail(PartName, '.');
      MemberName := GetStrAttr(Node, attrName, '');
      PartName := PartName + '[' + MemberName + ']';
      {если элемент фильтра выделен, то нас уже не волнует состояние его потомков}
      if Checked then
      begin
        result := PartName;
        {Особая обработка фильтровых исключений - Redmine Feature #17930}
        NL := GetExcludedChildren(Node);
        if AdditionalDetails then
        begin
          ExceptionSet := MakeExceptionSet(NL);
          if ExceptionSet <> '' then
            result := result + FSD + ' за исключением:' + FSD + ExceptionSet;
        end
        else
          if NL.length > 0 then
            result := result + ' с исключениями';
        exit;
      end;
    end;
    {если нужно, то идем по потомкам}
    NL := Node.childNodes;
    for i := 0 to NL.length - 1 do
    begin
      Part := GetPart(NL[i], LevelNode.nextSibling, PartName);
      if Part <> '' then
      begin
        AddTail(result, FSD);
        result := result + Part;
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
  i: integer;
  Part, PartName: string;
  DefaultNode: IXMLDOMNode;
begin
  result := '';
  if not Assigned(Members) then
    exit;
    
  if WithDefaultValue(DefaultNode) then
  begin
    while DefaultNode.nodeName <> 'Members' do
    begin
      result := '[' + GetStrAttr(DefaultNode, attrName, '') + ']' + result;
      DefaultNode := DefaultNode.parentNode;
    end;
    result := StringReplace(result, '][', '].[', [rfReplaceAll]);
    exit;
  end;

  NL := Members.selectNodes('function_result/Members/Member[@name]');
  if not Assigned(NL) then
    exit;
  for i := 0 to NL.Length - 1 do
  begin
    PartName := '';
    Part := GetPart(NL[i],
      Members.selectSingleNode('function_result/Levels').firstChild, PartName);
    if Part <> '' then
    begin
      AddTail(result, FSD);
      result := result + Part;
    end;
  end;
end;

function TSheetFilter.CheckForWriteback(out Error: string): boolean;
var
  Node: IXMLDOMNode;
begin
  result := false;
  Error := '';

  { проверяем на единственность фильтра}
  if IsMultiple then
  begin
    Error := ermWritebackMultipleFilter;
    exit;
  end;

  Node := Members.selectSingleNode('function_result/Members//Member[@checked="true"]');
  if not Assigned(Node) then
  begin
    Error := 'В фильтре не выбрано ни одного элемента';
    exit;
  end;

  if GetStrAttr(Node, attrPKID, 'null') = 'null' then
  begin
    Error := 'В фильтре %s выбран элемент, не предназначенный для обратной записи (PK_ID = "null").';
    exit;
  end;

  result := true;
end;

function TSheetFilter.WithDefaultValue(out DefaultNode: IXMLDOMNode): boolean; 
begin
  result := false;
  DefaultNode := nil;
  if Assigned(Members) then
  begin
    DefaultNode := Members.selectSingleNode(
      Format('//Member[@%s="true"]', [attrDefaultValue]));
    result := Assigned(DefaultNode);
  end;
end;

procedure TSheetFilter.RecreateLevelsByMembers;
begin
  //заглушка
end;

function TSheetFilter.GetOnDeleteWarning: string;
begin
  result := qumDelFilter + '"' + GetElementCaption + '"?';
end;

constructor TSheetFilterCollection.Create(Cell: TSheetSingleCellInterface);
begin
  Create(Cell.SheetInterface);
  FOwningCell := Cell;
end;

function TSheetFilterCollection.GetOwningCell: TSheetSingleCellInterface;
begin
  result := FOwningCell;
end;

function TSheetFilter.GetOwningCell: TSheetSingleCellInterface;
begin
  result := TSheetFilterCollectionInterface(Owner).OwningCell;
end;

function TSheetFilter.GetIsMultiple: boolean;

  procedure CountChecked(Node: IXMLDOMNode; var Count: integer);
  var
    i: integer;
    NL: IXMLDOMNodeList;
  begin
    if Count > 1 then
      exit;
    if GetBoolAttr(Node, attrChecked, false) then
    begin
      if Assigned(Node.selectSingleNode(Format('//Member[@%s="%d"]', [attrInfluence, Ord(neExclude)]))) then
        Count := 2
      else
        inc(Count);
      exit;
    end;

    NL := Node.childNodes;
    for i := 0 to NL.length - 1 do
    begin
      CountChecked(NL[i], Count);
      if Count > 1 then
        break;
    end;
  end;

var
  Count: integer;
begin
  Count := 0;
  CountChecked(Members.selectSingleNode('function_result/Members'), Count);
  result := Count > 1;
end;

function GetNodeMdx(Node: IXMLDOMNode): string;
var
  i: integer;
  NL, Exclusions: IXMLDOMNodeList;
  ChildMdx, MemberUName, ExceptionSet: string;
  Influence: TNodeInfluence;
  Checked: boolean;
begin
  if not Assigned(Node) then
    exit;

  result := '';
  MemberUName := GetStrAttr(Node, attrUniqueName, '');
  Checked := GetBoolAttr(Node, attrChecked, false);
  Influence := TNodeInfluence(GetIntAttr(Node, attrInfluence, 0));
  if Influence = neExclude then
    exit;


  if Checked then
  begin
    Exclusions := GetExcludedChildren(Node);
    if Assigned(Exclusions) and (Exclusions.length > 0) then
    begin
      ExceptionSet := '';
      for i := 0 to Exclusions.length - 1 do
      begin
        AddTail(ExceptionSet, ', ');
        ExceptionSet := ExceptionSet + GetStrAttr(Exclusions[i], attrUniqueName, '');
      end;
      ExceptionSet := Format('{Except({%s.Children}, {%s})}', [MemberUName, ExceptionSet]);
      AddTail(result, ', ');
      result := result + ExceptionSet;
      exit;
    end;

    AddTail(result, ', ');
    result := result + MemberUName;
    exit;
  end;

  if HasCheckedDescendants(Node) then
  begin
    NL := Node.childNodes;
    for i := 0 to NL.length - 1 do
    begin
      ChildMdx := GetNodeMdx(NL[i]);
      if ChildMdx <> '' then
      begin
        AddTail(result, ',');
        result := result + ChildMdx;
      end;
    end;
  end;
end;

function TSheetFilter.GetMdx: string;
var
  RootNode: IXMLDOMNode;
begin
  RootNode := Members.selectSingleNode('function_result/Members');
  result := GetNodeMdx(RootNode);
end;

end.
