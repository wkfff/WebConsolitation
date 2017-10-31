unit uMDXSetGenerator;

interface

uses
  SysUtils, ExcelXP, MSXML2_TLB, Classes, Windows,
  uFMExcelAddInConst, uXMLUtils,
  uFMAddinGeneralUtils, uSheetObjectModel, uCheckTV2,
  uFMAddinXMLUtils, uFMAddinExcelUtils,
  uExcelUtils, uGlobalPlaningConst;

type

  { Вспомогательный класс, составляющий точное MDX-описание множества
    выбранных мемберов элемента оси с использованием механизмов
    оптимизации размеров результируюшего MDX-a.}
  TMDXSetGenerator = class
  private
    Dom: IXMLDOMDocument2;
    AxisElement: TSheetAxisElementInterface;
    WhiteBullets, GreenBullets, RedBullets: TStringList; // кэшированные юникнеймы шариков

    function MakeSet(Str: string): string;
    procedure UncheckRedundantMembers;
    procedure CheckUncheckedParents;
  protected
    function GetMembersEnumeration: string;
    { Добавляет в промежуточный MDX функции для красных и зеленых шариков}
    function ApplyForcedlyIncludedMembers(tmpMDX: string): string;
    { Накладывает на промежуточный MDX исключения от белых шариков}
    function ApplyStronglyExcludedMembers(tmpMDX: string): string;
    { Накладывает на промежуточный MDX  ограничения по выделенным уровням,
      для отсекания лишних мемберов, порождаемых функцией Descendants}
    function IntersectWithLevels(tmpMDX: string; NeedIntersect: boolean): string;
    { Упорядочивает мемберы и исключает дубликаты}
    function OrderMembers(tmpMDX: string): string;

  public
    constructor Create;
    destructor Destroy; override;
    {}
    function Generate(AxisElem: TSheetAxisElementInterface;
      ForceAllMember, NeedCheckParents: boolean; out MDX: string): boolean;
  end;

implementation

const
  DirectEnumerationLimit = 10;

{ TMDXGenerator }

constructor TMDXSetGenerator.Create;
begin
  WhiteBullets := TStringList.Create;
  GreenBullets := TStringList.Create;
  RedBullets := TStringList.Create;
end;

destructor TMDXSetGenerator.Destroy;
begin
  inherited Destroy;
  Dom := nil;
  AxisElement := nil;
  FreeStringList(WhiteBullets);
  FreeStringList(GreenBullets);
  FreeStringList(RedBullets);
end;

function TMDXSetGenerator.Generate(AxisElem: TSheetAxisElementInterface;
  ForceAllMember, NeedCheckParents: boolean; out MDX: string): boolean;
var
  HasDisabledLevels: boolean;
begin
  result := false;
  MDX := '';
  GetDomDocument(Dom);
  try
    { Для построения запроса с использованием функций типа Subset нам
      недостаточно "обрезанного" дерева мемберов, хранящегося в листе -
      нужно все измерение целиком.}
    AxisElement := AxisElem;
    with AxisElement do
    try
      Dom := SheetInterface.DataProvider.GetMemberList(
        ProviderId, '', Dimension,
        Hierarchy, '', AllMemberProperties);
      if (SheetInterface.DataProvider.LastWarning <> '') then
        SheetInterface.PostMessage(
          SheetInterface.DataProvider.LastWarning, msgWarning);
    except
      SheetInterface.PostMessage('Не удалось получить элементы измерения ' +
        FullDimensionName2 + ';', msgError);
      exit;
    end;
    { Перепишем все нужное в новый датадом}
    CopyMembersState(AxisElement.Members, Dom, nil);
    HasDisabledLevels := Dom.selectNodes(
      Format('function_result/Levels/Level[@%s="0"]', [attrLevelState])).length > 0;
    { Снимем галки, совпадающие с действием элементов влияния,
      чтобы исключить двойной их учет. Сделать это надо до удаления уровней.}
    UncheckRedundantMembers;
    { Выкинем отключенные уровни - в обычном режиме они попросту не нужны,
      а в серверном и вовсе вредны.}
    FilterMembersDomEx(Dom);
    if NeedCheckParents then
      CheckUncheckedParents;

    MDX := GetMembersEnumeration;
    MDX := ApplyForcedlyIncludedMembers(MDX);
    MDX := ApplyStronglyExcludedMembers(MDX);
    MDX := IntersectWithLevels(MDX, HasDisabledLevels);
    {чтобы общий итог попадал в запрос даже при отключенном уровне All,
      добавляем его отдельно, уже после пересечения уровней}
    if ForceAllMember and (AxisElement.AllMember <> '') then
    begin
      MDX := MakeSet(MDX);
      AddHead(', ', MDX);
      MDX := AxisElement.AllMember + MDX;
    end;
    MDX := OrderMembers(MDX);
  finally
    KillDomDocument(Dom);
  end;
  result := true;
end;

function TMDXSetGenerator.GetMembersEnumeration: string;
var
  i, Count, LevelIndex: integer;
  XPath, LevelName: string;
  LevelsNL, MembersNL: IXMLDOMNodeList;
  LastIteration: boolean;
begin
  result := '';

  { Перебор уровней}
  XPath := 'function_result/Members';
  LevelsNL := Dom.selectNodes('function_result/Levels/Level');
  for LevelIndex := 0 to LevelsNL.length - 1 do
  begin
    LevelName := GetStrAttr(LevelsNL[LevelIndex], attrName, '');
    XPath := XPath + '/Member';
    MembersNL := Dom.selectNodes(XPath + Format('[@%s="false"]', [attrChecked]));

    { Если уровень выделен красной галкой или выделены все его мемберы,
      то в запрос идет его AllMembers}
    if (GetIntAttr(LevelsNL[LevelIndex], attrLevelState, 1) = 2) or (MembersNL.length = 0)then
    begin
      AddTail(result, ', ');
      result := result + Format('%s.[%s].Allmembers',
        [AxisElement.FullDimensionNameMDX, LevelName]);
      continue;
    end;

    { Выбираем все мемберы данного уровня, анализируем идущие последовательно
      галки и подставляем функцию Subset}
    MembersNL := Dom.selectNodes(XPath);
    Count := 0;
    LastIteration := false;
    for i := 0 to MembersNL.length - 1 do
    begin
      { Встретили поставленную галку - накапливаем серию}
      if GetBoolAttr(MembersNL[i], attrChecked, false) then
      begin
        inc(Count);
        if i < MembersNL.length - 1 then
          continue;
        LastIteration := true;
      end;
      { Встретили снятую галку}
      begin
        { Продолжается серия снятых галок}
        if Count = 0 then
          continue;
        { Отдельно стоящая галка}
        if Count = 1 then
        begin
          AddTail(result, ', ');
          result := result + GetStrAttr(
            MembersNL[IIF(LastIteration, i, i - 1)], attrUniqueName, '');
        end
        { Накопленную серию подставляем в результирующий MDX}
        else
        begin
          AddTail(result, ', ');
          result := result +
            Format('Subset(%s.[%s].Allmembers, %d, %d)',
            [AxisElement.FullDimensionNameMDX, LevelName,
              IIF(LastIteration, i - Count + 1, i - Count), Count]);
        end;
        Count := 0;
      end;
    end;
  end;
end;

procedure TMDXSetGenerator.UncheckRedundantMembers;
var
  LevelsNL, MembersNL, InfluencesNL: IXMLDOMNodeList;
  XPath, UName, EncodedName: string;
  i, j: integer;
  Influence: TNodeInfluence;
  Node: IXMLDOMNode;
  Level: TSheetLevelInterface;
begin

  { Уровни, выделенные красными галками, пойдут в запрос как [Level].AllMembers,
    поэтому снимаем галки со всех их мемберов - не нужны.}
  LevelsNL := Dom.selectNodes('function_result/Levels/Level');
  XPath := 'function_result/Members';
  for i := 0 to LevelsNL.length - 1 do
  begin
    XPath := XPath + '/Member';
    MembersNL := Dom.selectNodes(XPath);
    if GetIntAttr(LevelsNL[i], attrLevelState, 1) = 2 then
    begin
      FillNodeListAttribute(MembersNL, attrChecked, 'false');
      continue;
    end;
    {Здесь же снимем галки с датамемберов по уровням.
      Важно: здесь имеем полный датадом измерения со всеми уровнями и уже
      отредактированный набор уровней в модели.}
    if AxisElement.HideDataMembers then
    begin
      Level := AxisElement.Levels.FindByInitialIndex(i);
      if not Assigned(Level) then
        continue;
      if Level.HideDataMembers then
      begin
        for j := 0 to MembersNL.length - 1 do
          if IsDataMember(MembersNL[j]) then
            (MembersNL[j] as IXMLDOMElement).setAttribute(attrChecked, 'false');
      end;
    end;
  end;

  WhiteBullets.Clear;
  GreenBullets.Clear;
  RedBullets.Clear;

  { Красный и зеленый шарики тоже позволяют убрать лишние галки}
  InfluencesNL := Dom.selectNodes('function_result/Influences/Influence');
  for i := 0 to InfluencesNL.length - 1 do
  begin
    Influence := TNodeInfluence(GetIntAttr(InfluencesNL[i], attrType, 0));
    if Influence = neNone then
      continue;

    {Найдем соответствующий элемент в дереве. Если его нет (например в результате
      переноса листа на другую базу), то и в запросе этот шарик не нужен, т.к.
      приведет к ошибке.}
    UName := GetStrAttr(InfluencesNL[i], attrUniqueName, '');
    EncodedName := UName;
    EncodeXPathString(EncodedName);
    Node := Dom.selectSingleNode(Format('function_result/Members//Member[@%s="%s"]',
      [attrUniqueName, EncodedName]));
    if not Assigned(Node) then
      continue;

    if Influence = neExclude then
    begin
      WhiteBullets.Add(UName);
      continue;
    end;
    if not Node.hasChildNodes then
      continue;
    if Influence = neChildren then
    begin
      MembersNL := Node.childNodes;
      GreenBullets.Add(UName);
    end
    else
    begin
      MembersNL := Node.selectNodes('.//Member');
      RedBullets.Add(UName);
    end;
    FillNodeListAttribute(MembersNL, attrChecked, 'false');
  end;
end;

function TMDXSetGenerator.ApplyStronglyExcludedMembers(
  tmpMDX: string): string;
var
  i: integer;
  tmpStr: string;
begin
  result := '';
  if tmpMDX = '' then
    exit;
  if WhiteBullets.Count = 0 then
  begin
    result := MakeSet(tmpMDX);
    exit;
  end;
  tmpStr := '';
  for i := 0 to WhiteBullets.Count - 1 do
  begin
    tmpStr := Format('Descendants(%s, 0, SELF_AND_AFTER)', [WhiteBullets[i]]);
    AddTail(result, ', ');
    result := result + tmpStr;
  end;

  result := Format('Except(%s, %s)', [MakeSet(tmpMDX), SetBrackets(result)]);
end;

function TMDXSetGenerator.ApplyForcedlyIncludedMembers(
  tmpMDX: string): string;
var
  i: integer;
  tmpStr: string;
begin
  result := tmpMDX;
  
  { Красные шарики}
  for i := 0 to RedBullets.Count - 1 do
  begin
    tmpStr := Format('Descendants(%s, 0, AFTER)', [RedBullets[i]]);
    AddTail(result, ', ');
    result := result + tmpStr;
  end;

  { Зеленые шарики}
  for i := 0 to GreenBullets.Count - 1 do
  begin
    tmpStr := Format('%s.Children', [GreenBullets[i]]);
    AddTail(result, ', ');
    result := result + tmpStr;
  end;
end;

function TMDXSetGenerator.MakeSet(Str: string): string;
begin
//  if (Str[1] = '{') and (Str[Length(Str)] = '}') then
  if (Str[1] = '{') and (Pos('}', Str) = Length(Str)) then
    result := Str
  else
    result := SetBrackets(Str);
end;

function TMDXSetGenerator.OrderMembers(tmpMDX: string): string;
begin
  result := Format('Distinct(Hierarchize(%s))', [MakeSet(tmpMDX)]);
end;

function TMDXSetGenerator.IntersectWithLevels(tmpMDX: string; NeedIntersect: boolean): string;
var
  LevelsNL: IXMLDOMNodeList;
  i: integer;
begin
  { Если нет красных и зеленых шариков, тянущих мемберы с нижних
    уровней, то и никакое пересечение не требуется.}
  if (GreenBullets.Count = 0) and (RedBullets.Count = 0) then
  begin
    result := tmpMDX;
    exit;
  end;
  result := '';
  LevelsNL := Dom.selectNodes('function_result/Levels/Level');
  for i := 0 to LevelsNL.length - 1 do
  begin
    if GetIntAttr(LevelsNL[i], attrLevelState, 0) = 0 then
      continue;
    AddTail(result, ', ');
    result := result + Format('%s.[%s].Allmembers',
      [AxisElement.FullDimensionNameMDX,
      GetStrAttr(LevelsNL[i], attrName, '')]);
   end;
  if NeedIntersect then
    result  := Format('Intersect(%s, %s)', [MakeSet(tmpMDX), MakeSet(result)])
  else
    result := MakeSet(tmpMDX);
end;

procedure TMDXSetGenerator.CheckUncheckedParents;
var
  NL: IXMLDOMNodeList;
  i: integer;
begin
  NL := Dom.selectNodes(Format(
    'function_result/Members//Member[(@%s="false") and (.//Member[@%s="true"])]',
    [attrChecked, attrChecked]));
  for i := 0 to NL. length - 1 do
    (NL[i] as IXMLDOMElement).setAttribute(attrChecked, 'true');
end;

end.
