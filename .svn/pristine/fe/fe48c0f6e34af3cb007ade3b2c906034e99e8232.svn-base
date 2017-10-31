{
  Конвертация метаданных листов старых версий в новый формат.
  На вход подаются две вещи:
    1) "Старый дом". Тот что был последний раз сохранен в листе.
    2) Только что загруженная с этого дома объектная модель


  Типичные ситуации изменения метаданных от версии к версии:
    1) Появилось новое свойство объектной модели.
      В этом случае ничего делать не надо. В ранее построенных листах это
      свойство инициализируется значением по умолчанию
    2) Ранее существовашее свойство объектной модели пропало в новой версии.
      Это потенциально опасное изменение. Нужно позаботиться о том, что бы
      со старыми листами все было ОК. Если свойство куда-то переехало, то
      нужно доставать(*) старое свойства инициализировать новым


  (*) - этому модулю в виде исключения позволено лазать по XMLDOM в которую
  была сохранена метадата напрямую, минуя объектную модель
}

unit uConverter;

interface

uses  SysUtils, MSXML2_TLB, uSheetAxes, uXMLUtils, uFmExcelAddinConst,
      OfficeXP, ExcelXP, uFMAddinExcelUtils, classes, uSheetStyles,
      uSheetObjectModel, uFMAddinXMLUtils, PlaningProvider_TLB, uExcelUtils,
      uFMAddinGeneralUtils, Windows, uSheetHistory, PlaningTools_TLB,
      uGlobalPlaningConst, uSheetCollector, uOfficeUtils;

{Проверяет версию листа, с возможным ее обновлением
 Если флаг ShouldAskUser взведен, то в случае необходимости обновления,
 пользователя спросят, хочет ли он это делать.}
function CheckSheetVersion(ExcelSheet: ExcelWorksheet;
  DataProvider: IPlaningProvider; ShouldAskUser: boolean;
  Process: ProcessForm): boolean;

var
  // отношение версии листа к версии надстройки
  VersionRelation: TVersionRelation;
  // нужен ли апдейт версии текущего листа
  UpdateNeed: boolean;

implementation


type
  {Класс-обертка для набора функций, обновляющих версию метаданных до
  текущей версии плагина}
  TConverter = class
  protected
    FOldMDDOM: IXMLDOMDocument2;
    FExcelSheet: ExcelWorkSheet;
    FProvider: IPlaningProvider;

    function UpdateTo108: boolean;
    function UpdateTo109: boolean;

    function UpdateTo100Ex: boolean;
    function UpdateTo101Ex: boolean;
    function UpdateTo107Ex: boolean;
    function UpdateTo1011Ex: boolean;
    function UpdateTo201: boolean;
    function UpdateTo221: boolean;
    function UpdateTo222: boolean;
    function UpdateTo224: boolean;
    function UpdateTo225: boolean;
    function UpdateTo226: boolean;
    function UpdateTo227(NeedUpdateTypeFormulas: boolean): boolean;
    function UpdateTo230: boolean;
    function UpdateTo231: boolean;
    function UpdateTo232: boolean;
    function UpdateTo234: boolean;
  public

    {обновляет версию метаданных листа. результат - успех/неуспех}
    function Update(var Version: string): boolean;
    procedure SetExternalLinks(OldMDDOM: IXMLDOMDocument2;
      ExcelSheet: ExcelWorksheet; DataProvider: IPlaningProvider);
  end;


  {В версии 2.3.2 была проведена реформа итогов. Для работы конвертера
    прежних версий сохраним описание старого класса с минимальным функционалом.}
  TOldSummaryOptions = class
  private
    FEnabled: boolean;
    FAtBeginning: boolean;
    FTitle: string;
    FUseTitle: boolean;
  public
    constructor Create;
    procedure WriteToXML(Node: IXMLDOMNode);
    procedure ReadFromXML(Node: IXMLDOMNode);

    property Enabled: boolean read FEnabled write FEnabled;
    property AtBeginning: boolean read FAtBeginning write FAtBeginning;
    property Title: string read FTitle write FTitle;
    property UseTitle: boolean read FUseTitle write FUseTitle;
  end;

  constructor TOldSummaryOptions.Create;
  begin
    FEnabled := true;
    FAtBeginning := false;
    FUseTitle := false;
    FTitle := 'Итоги';
  end;

  procedure TOldSummaryOptions.ReadFromXML(Node: IXMLDOMNode);
  begin
    if not Assigned(Node) then
      exit;
    FEnabled := GetBoolAttr(Node, 'enabled', true);
    FAtBeginning := GetBoolAttr(Node, 'atbeginning', false);
    FUseTitle := GetBoolAttr(Node, 'usetitle', false);
    FTitle := GetStrAttr(Node, 'title', '');
  end;

  procedure TOldSummaryOptions.WriteToXML(Node: IXMLDOMNode);
  begin
    if not Assigned(Node) then
      exit;
    with Node as IXMLDOMElement do
    begin
      setAttribute('enabled', BoolToStr(Enabled));
      setAttribute('atbeginning', BoolToStr(AtBeginning));
      setAttribute('usetitle', BoolToStr(UseTitle));
      setAttribute('title', Title);
    end;
  end;


procedure RestoreConstants(Book: ExcelWorkbook); forward;

function TConverter.Update(var Version: string): boolean;
var
  Major, Minor, Build: integer;
  InitialVersion: string;
begin
  result := Assigned(FExcelSheet) and Assigned(FOldMDDOM) and Assigned(FProvider);
  if not result then
    exit;
  InitialVersion := Version;
  Major := StrToInt(CutPart(Version, '.'));
  Minor := StrToInt(CutPart(Version, '.'));
  Build := StrToInt(CutPart(Version, '.'));
  try
    {Будем последовательно прогонять все имеющиеся процедуры преобразования}
    if Major = 0 then
    begin
      result := result and UpdateTo100Ex;
      if not result then
        exit;
      Major := 1;
      Minor := 0;
      Build := 0;
    end;

    if (Major = 1) and (Minor = 0) and (Build = 0) then
    begin
      result := result and UpdateTo101Ex;
      if not result then
        exit;
      Build := 1;
    end;

    if (Major = 1) and (Minor = 0) and (Build in [1..6]) then
    begin
      result := result and UpdateTo107Ex;
      if not result then
        exit;
      Build := 7;
    end;

    if (Major = 1) and (Minor = 0) and (Build = 7) then
    begin
      result := result and UpdateTo108;
      if not result then
        exit;
      Build := 8;
    end;

    if (Major = 1) and (Minor = 0) and (Build = 8) then
    begin
      result := result and UpdateTo109;
      if not result then
        exit;
      Build := 9;
    end;

    if (Major = 1) and (Minor = 0) and (Build < 11) then
    begin
      result := result and UpdateTo1011Ex;
      if not result then
        exit;
      Build := 11;
    end;

    if ((Major = 1) and (Minor = 0) and (Build = 11)) or
       ((Major = 2) and (Minor = 0) and (Build = 0)) then
    begin
      result := result and UpdateTo201;
      if not result then
        exit;
      Major := 2;
      Minor := 0;
      Build := 1;
    end;

    if ((Major = 2) and (Minor = 0) and (Build = 1)) or
       ((Major = 2) and (Minor = 2) and (Build = 0)) then
    begin
      result := result and UpdateTo221;
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 1;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 1)) then
    begin
      result := result and UpdateTo222;
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 2;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 2)) or
      ((Major = 2) and (Minor = 2) and (Build = 3)) then
    begin
      result := result and UpdateTo224;
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 4;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 4)) then
    begin
      result := result and UpdateTo225;
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 5;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 5)) then
    begin
      result := result and UpdateTo226;
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 6;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 6)) then
    begin
      result := result and UpdateTo227((InitialVersion = '2.2.5') or (InitialVersion = '2.2.6'));
      if not result then
        exit;
      Major := 2;
      Minor := 2;
      Build := 7;
    end;
    if ((Major = 2) and (Minor = 2) and (Build = 7)) or
       ((Major = 2) and (Minor = 2) and (Build = 8)) or
       ((Major = 2) and (Minor = 2) and (Build = 9)) then
    begin
      result := result and UpdateTo230;
      if not result then
        exit;
      Major := 2;
      Minor := 3;
      Build := 0;
    end;
    if ((Major = 2) and (Minor = 3) and (Build = 0)) then
    begin
      result := result and UpdateTo231;
      if not result then
        exit;
      Major := 2;
      Minor := 3;
      Build := 1;
    end;
    if ((Major = 2) and (Minor = 3) and (Build = 1)) then
    begin
      result := result and UpdateTo232;
      if not result then
        exit;
      Major := 2;
      Minor := 3;
      Build := 2;
    end;
    if ((Major = 2) and (Minor = 3) and (Build = 2)) or
       ((Major = 2) and (Minor = 3) and (Build = 3)) then
    begin
      result := result and UpdateTo234;
      if not result then
        exit;
      Major := 2;
      Minor := 3;
      Build := 4;
    end;
  finally
    Version := Format('%d.%d.%d', [Major, Minor, Build]);
  end;
end;

function TConverter.UpdateTo100Ex: boolean;

  procedure UpdateMembers(Dom: IXMLDOMDocument2);
  var
    NL: IXMLDOMNodeList;
    i: integer;
    UName: string;
  begin
    NL := Dom.selectNodes('function_result/Members//*[@unique_name]');
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      UName := StringReplace(UName, '\', '#5c#', [rfReplaceAll]);
      (NL[i] as IXMLDOMElement).setAttribute(attrUniqueName, UName);
    end;
  end;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i: integer;
    Members: IXMLDOMDocument2;
    Id: string;
  begin
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      if Id = '' then
        continue;
      Members := GetDataFromCP(FExcelSheet, Id);
      if Assigned(Members) then
      begin
        UpdateMembers(Members);
        PutDataInCP(FExcelSheet, Id, Members);
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
begin
  NL := FOldMDDom.selectNodes('metadata/columns/column');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/rows/row');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/filters/filter');
  UpdateCollection(NL);
  result := true;
end;

function TConverter.UpdateTo101Ex: boolean;

  procedure UpdateMembers(Dom: IXMLDOMDocument2);
  var
    NL: IXMLDOMNodeList;
    i: integer;
    UName: string;
  begin
    NL := Dom.selectNodes('function_result/Members//*[@unique_name]');
    for i := 0 to NL.length - 1 do
    begin
      UName := GetStrAttr(NL[i], attrUniqueName, '');
      UName := StringReplace(UName, '#5c#', '\', [rfReplaceAll]);
      (NL[i] as IXMLDOMElement).setAttribute(attrUniqueName, UName);
    end;
  end;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i: integer;
    Members: IXMLDOMDocument2;
    Id: string;
  begin
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      if Id = '' then
        continue;
      Members := GetDataFromCP(FExcelSheet, Id);
      if Assigned(Members) then
      begin
        UpdateMembers(Members);
        PutDataInCP(FExcelSheet, Id, Members);
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
begin
  NL := FOldMDDom.selectNodes('metadata/columns/column');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/rows/row');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/filters/filter');
  UpdateCollection(NL);
  result := true;
end;

{
  Пока переделка одна. Раньше было свойство листа - "считать итоги самим" (или из базы)
  Теперь у листа этого свойства нет. Но появились соотвтетствующие свойства у каждого итога
  Нужно в старых листах проинициализировать старые свойства новыми
}
function TConverter.UpdateTo107Ex: boolean;
var
  Node: IXMLDOMNode;
  OldSummariesByVisible: boolean;
  i: integer;
  NL: IXMLDOMNodeList;
  TotalType: TSheetTotalType;
begin
  result := true;
  Node := FOldMDDOM.selectSingleNode('//metadata/innerdata');
  if Assigned(Node) then
  begin
    OldSummariesByVisible := GetBoolAttr(Node, attrSummariesByVisible, true);
    {Если в старом листе этот флаг был сброшен, сбрасываем сбрасываем его
    у всех итогов. Если true - ничего не делаем - это по умолчанию.
    Причем для всех кроме свободных - у них всегда этот флаг взведен}
    if not OldSummariesByVisible then
    begin
      NL := FOldMdDom.selectNodes('metadata/totals/total');
      for i := 0 to NL.length - 1 do
      begin
        TotalType := TSheetTotalType(GetIntAttr(NL[i], attrTotalType, 0));
        if (TotalType in [wtMeasure, wtResult]) then
          (NL[i] as IXMLDOMElement).setAttribute(attrSummariesByVisible, 'false');
      end;
    end;
  end;
end;

function TConverter.UpdateTo108: boolean;
var
  i: integer;
  Name_: ExcelXP.Name;
  Name: widestring;
  tmpIndex: integer;
  Params: TStringList;
  ObjType: string;
begin
  result := true;
  // меняем имена мемберов
  for i := 1 to FExcelSheet.Names.Count do
  begin
    Name_ := FExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
    Name := Name_.Name_;
    tmpIndex := Pos('!', Name);
    Name := Copy(Name, tmpIndex + 1, Length(Name) - tmpIndex);
    // возможно имя не наше
    if not ParseExcelName(Name, Params) then
      continue;
    try
      ObjType := Params[0];
      // обрабатываем только имена мемберов
      if (ObjType <> sntMemberOld) then
        continue;
      Name_.Name_ := StringReplace(Name_.Name_, sntMemberOld, sntMember, [rfReplaceAll]);
    finally
      FreeAndNil(Params);
    end;
  end;
end;

function TConverter.UpdateTo109: boolean;

const
  {Имена старых стилей, подлежащих удалению}
  osFieldTitle = 'FieldTitle';
  osFieldTitlePrint = 'FieldTitlePrint';
  osFieldPosition = 'FieldPosition';
  osFieldPositionPrint = 'FieldPositionPrint';
  osTotalMeasureTitle = 'TotalMeasureTitle';
  osTotalFreeTitle = 'TotalFreeTitle';
  osTotalResultTitle = 'TotalResultTitle';
  osTotalTitlePrint = 'TotalTitlePrint';
  osData = 'Data';
  osDataFree = 'DataFree';
  osDataFreeErased = 'DataFreeErased';
  osFilterValue = 'FilterValue';
  osFilterValuePrint = 'FilterValuePrint';
  osSheetId = 'SheetId';
  osMemberProperties = 'MemberProperties';
  osMemberPropertiesPrint = 'MemberPropertiesPrint';

  procedure DeleteOldStyle(Workbook: _Workbook);
  var
    i, j: integer;
    Style: ExcelXP.Style;
    s: string;
  begin
    try
      j := 1;
      //пробегаемся по всей коллекции стилей, если находим старый удаляем...
      for i := 1 to Workbook.Styles.Count do
      begin
        Style := Workbook.Styles.Item[j];
        s := Style.Name;
        if (Style.Name = osFieldTitle) or (Style.Name = osFieldTitlePrint)
          or (Style.Name = osFieldPosition) or (Style.Name = osFieldPositionPrint)
          or (Style.Name = osTotalMeasureTitle) or (Style.Name = osTotalFreeTitle)
          or (Style.Name = osTotalResultTitle) or (Style.Name = osTotalTitlePrint)
          or (Style.Name = osData) or (Style.Name = osDataFree)
          or (Style.Name = osDataFreeErased) or (Style.Name = osFilterValue)
          or (Style.Name = osFilterValuePrint) or (Style.Name = osSheetId)
          or (Style.Name = osMemberProperties) or (Style.Name = osMemberPropertiesPrint) then
           Style.Delete
        else
          inc(j);
      end;
    except
    end;
  end;

//var
  //ExcelBook: ExcelWorkbook;
begin
  result := true;
 (* ExcelBook := MetaData.Sheet.Application.ActiveWorkbook;
  SetBookProtection(ExcelBook, false);
  if Assigned(ExcelBook) then
    DeleteOldStyle(ExcelBook);  *)
end;

function TConverter.UpdateTo1011Ex: boolean;

  procedure UpdateMembers(Dom: IXMLDOMDocument2);
  const
    XPathTemplate = 'function_result/Members//Member[@%s="%s"]';
  var
    NL: IXMLDOMNodeList;
    i, CheckState: integer;
  begin
    if not Assigned(Dom) then
      exit;

    NL := Dom.selectNodes('//Member');
    for i := 0 to NL.length - 1 do
    begin
      {замена старого атрибута - CheckState - на новый - Checked}
      CheckState := GetIntAttr(NL[i], 'checkstate', 0);
      NL[i].attributes.removeNamedItem('checkstate');
      if CheckState = 1 then
      begin
        (NL[i] as IXMLDOMElement).setAttribute(attrChecked, 'true');
        if NL[i].hasChildNodes then
          (NL[i] as IXMLDOMElement).setAttribute(attrAllChildrenChecked, 'true');
      end
      else
        (NL[i] as IXMLDOMElement).setAttribute(attrChecked, 'false');
      {новый атрибут - влияние}
      (NL[i] as IXMLDOMElement).setAttribute(attrInfluence, 0);
    end;
  end;

  procedure PreUpdateFilter(Dom: IXMLDOMDocument2);

    procedure DoUpdate(Node: IXMLDOMNode);
    var
      CheckState, i: integer;
    begin
      if not Assigned(Node) then
        exit;
      CheckState := GetIntAttr(Node, 'checkstate', 0);
      case CheckState of
        0: exit;
        1:
          if Node.hasChildNodes then
            for i := 0 to Node.childNodes.length - 1 do
              Node.removeChild(Node.childNodes[0]);
        2:
          if Node.hasChildNodes then
            for i := 0 to Node.childNodes.length - 1 do
              DoUpdate(Node.childNodes[i]);
      end;
    end;

  var
    NL: IXMLDOMNodeList;
    i: integer;
  begin
    if not Assigned(Dom) then
      exit;
    NL := Dom.selectNodes('function_result/Members/Member');
    for i := 0 to NL.length - 1 do
      DoUpdate(NL[i]);
  end;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i: integer;
    Members: IXMLDOMDocument2;
    Name, Id: string;
  begin
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      Name := NL[i].nodeName;
      if Id = '' then
        continue;
      Members := GetDataFromCP(FExcelSheet, Id);
      if Assigned(Members) then
      begin
        if Name = 'filter' then
          PreUpdateFilter(Members);
        UpdateMembers(Members);
        PutDataInCP(FExcelSheet, Id, Members);
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
begin
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  NL := FOldMDDom.selectNodes('metadata/columns/column');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/rows/row');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/filters/filter');
  UpdateCollection(NL);
  result := true;
end;

function TConverter.UpdateTo201: boolean;

  procedure UpdateMembers(Members: IXMLDOMDocument2; OldLevels: IXMLDomNodeList);
  var
    i: integer;
    AName: string;
    NewLevel: IXMLDomNode;
  begin
    for i := 0 to OldLevels.length - 1 do
    begin
      AName := GetNodeStrAttr(OldLevels[i], attrName);
      NewLevel := Members.selectSingleNode(
        'function_result/Levels/Level[@name="' + AName + '"]');
      if Assigned(NewLevel) then
        (NewLevel as IXMLDomElement).setAttribute(attrLevelState, 2);
    end;
  end;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i: integer;
    Members: IXMLDOMDocument2;
    Name, Id: string;
    LevelsOnly: boolean;
    LevelsNL: IXMLDOMNodeList;
  begin
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      Name := NL[i].nodeName;
      if Id = '' then
        continue;
      LevelsOnly := GetBoolAttr(NL[i], 'levelsonly', false);
      NL[i].attributes.removeNamedItem('levelsonly');
      if LevelsOnly then
      begin
        Members := GetDataFromCP(FExcelSheet, Id);
        LevelsNL := NL[i].selectNodes('levels/level');
        if Assigned(Members) then
        begin
          UpdateMembers(Members, LevelsNL);
          PutDataInCP(FExcelSheet, Id, Members);
        end;
      end;
    end;
  end;

var
  NL: IXMLDOMNodeList;
begin
  {раньше у измерения был общий признак "учитывать только выбор уровней",
    теперь есть индивидуальные признаки у каждого уровня. Надо выставлять
    "красные галки" всем уровням, в измерениях которых стоял старый признак}

  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  NL := FOldMdDom.selectNodes('metadata/rows/row');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/columns/column');
  UpdateCollection(NL);
  result := true;
end;

function TConverter.UpdateTo221: boolean;
const
  OldPattern = snSeparator + sntSingleCellOld + snSeparator;
  NewMeasurePattern = snSeparator + sntSingleCellMeasure + snSeparator;
  NewResultPattern = snSeparator + sntSingleCellResult + snSeparator;
var
  i: integer;
  NL: IXMLDOMNodeList;
  CellNode: IXMLDOMNode;
  ExcelNames, Params: TStringList;
  EName: ExcelXP.Name;
  StrName, Id: string;
  TotalType: TSheetTotalType;
begin
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  NL := FOldMDDom.selectNodes('metadata/singlecells/singlecell');
  ExcelNames := TStringList.Create;
  try
    {Коллекция имен листа является "живой" - она динамически сортируется по
      алфавиту. Поэтому нельзя устраивать простой перебор всех имен с их
      изменением - имя, только что бывшее, скажем, вторым в коллекции,
      после переименования мгновенно станет пятисотым, соответственно подвинутся
      вниз все имена в этом промежутке и наша (непрерывная) адресация даст
      сбой - конкретно в этом примере будет пропущено четвертое имя, а то, к
      которому мы адресовались как к четвертому - на самом деле пятое.
      Такие дела...
      Поэтому используется предварительное кэширование интересующих нас имен.}
    for i := 1 to FExcelSheet.Names.Count do
    begin
      EName := FExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
      StrName := EName.Name_;
      if IsNameOurs(StrName) then
        ExcelNames.Add(StrName);
    end;

    for i := 0 to ExcelNames.Count - 1 do
    begin
      StrName := GetShortSheetName(ExcelNames[i]);
      // возможно имя не наше
      if not ParseExcelName(StrName, Params) then
        Continue;
      if Params[0] <> sntSingleCellOld then
        Continue;
      Id := Params[1];
      CellNode := FOldMDDom.selectSingleNode('metadata/singlecells/singlecell[@id="' +
        Id+ '"]');
      if not Assigned(CellNode) then
        Continue;
      TotalType := TSheetTotalType(GetIntAttr(CellNode, attrTotalType, 0));
      case TotalType of
        wtMeasure: StrName := StringReplace(ExcelNames[i], OldPattern,
          NewMeasurePattern, []);
        wtResult: StrName := StringReplace(ExcelNames[i], OldPattern,
          NewResultPattern, []);
        else Continue;
      end;
      EName := GetNameObject(FExcelSheet, ExcelNames[i]);
      EName.Name_ := StrName;
    end;
  finally
    FreeStringList(ExcelNames);
  end;
  result := true;
end;

{ Переводит лист на текущую версию}
function UpdateMetaData(var ExcelSheet: ExcelWorksheet;
  DataProvider: IPlaningProvider; out ResultVersion: string): boolean;

  function GetDebuggingConditions: string;
  begin
    if IsTestVersion then
      result := 'тестовая'
    else
      result := 'релиз';
  end;

var
  Converter: TConverter;
  SheetVersion, Comment: string;
  Node: IXMLDOMNode;
  OldMDDOM: IXMLDOMDocument2;
  SheetHistory: TSheetHistory;
begin
  result := false;

  if not Assigned(ExcelSheet) then
    exit;

  OldMDDOM := GetDataFromCP(ExcelSheet, cpMDName);
  if not Assigned(OldMDDOM) then
    exit;

  Node := OldMdDom.selectSingleNode('metadata/innerdata');
  SheetVersion := GetStrAttr(Node, attrSheetVersion, '0.0.0');
  ResultVersion := SheetVersion;

  Converter := TConverter.Create;
  try

    if SheetVersion = '1.0.8' then
    begin
      result := false;
      SheetHistory := TSheetHistory.Create(nil);
      Comment := Format('Обновление листов версии 1.0.8 не поддерживается.',
        [SheetVersion, GetAddinVersion, GetDebuggingConditions]);
      SheetHistory.AddEvent(ExcelSheet, evtVersionUpdate,
        ConvertStringToCommaText(Comment), result);
      exit;
    end;

    Converter.SetExternalLinks(OldMDDOM, ExcelSheet, DataProvider);
    try
      result := Converter.Update(ResultVersion);
    except
      result := false;
    end;

    if result then
    begin
      ExcelSheet := Converter.FExcelSheet;
      (Node as IXMLDOMElement).setAttribute(attrSheetVersion, GetAddinVersion);
      PutDataInCP(ExcelSheet, cpMDName, OldMdDom);

      {Добавляем запись в журнал}
      SheetHistory := TSheetHistory.Create(nil);
      Comment := Format('Обновление версии листа с %s до %s. Версия надстройки "%s".  ',
        [SheetVersion, GetAddinVersion, GetDebuggingConditions]);
      SheetHistory.AddEvent(ExcelSheet, evtVersionUpdate,
        ConvertStringToCommaText(Comment), result);
    end;
  finally
    FreeAndNil(Converter);
    FreeAndNil(SheetHistory);
  end;
end;


function CheckSheetVersion(ExcelSheet: ExcelWorksheet;
  DataProvider: IPlaningProvider; ShouldAskUser: boolean;
  Process: ProcessForm): boolean;
var
  Operation: IOperation;

  function IsProcessShowing: boolean;
  begin
    result := false;
    if Assigned(Process) then
      result := Process.Showing;
  end;

  procedure StartUpdateIndication;
  begin
    if IsProcessShowing then
      Process.OpenOperation(mSheetVersionUpdate, CriticalNode, NoteTime, otUpdate)
    else
    begin
      Operation := CoOperation.Create;
      Operation.StartOperation(ExcelSheet.Application.Hwnd);
      Operation.Caption := mSheetVersionUpdate;
    end;
  end;

  procedure EndUpdateIndication(IsSuccess: boolean; ErrMessage: string);
  begin
    if IsProcessShowing then
    begin
      if IsSuccess then
        Process.CloseOperation
      else
        Process.PostError(ermUpdateSheetVersionFail);
    end
    else
      if Assigned(Operation) then
      begin
        Operation.StopOperation;
        Operation := nil;
        if not IsSuccess then
          ShowError(ErrMessage);
      end;
  end;

  { Из-за ошибки в процедуре передачи контекста задачи во все листы книги
    прописывалась пустая xml-ка метаданных, что формально превращало лист
    в "наш".}
  function DeleteFakePlaningData: boolean;
  var
    Dom: IXMLDOMDocument2;
    Node: IXMLDOMNode;
    RefreshDate: string;
    CP: CustomProperty;
  begin
    result := false;
    Dom := GetDataFromCP(ExcelSheet, cpMDName);
    if not Assigned(Dom) then
      exit;
    Node := Dom.selectSingleNode('metadata/innerdata');
    RefreshDate := GetStrAttr(Node, attrLastRefreshDate, fmNoRefreshDate);
    if RefreshDate = fmNoRefreshDate then
    begin
      { у старых листов (до 2.2.2) вообще нет свойства "дата обновления"}
      if Dom.selectNodes('metadata/totals/total | metadata/rows/row | ' +
        'metadata/columns/column | metadata/filters/filter | ' +
        'metadata/singlecells/singlecell').length > 0 then
      exit;
      CP := GetCPByName(ExcelSheet, cpMDName, false);
      if Assigned(CP) then
      try
        CP.Delete;
        result := true;
      except
        result := false;
      end;
      CP := GetCPByName(ExcelSheet, cpSheetHistory, false);
      if Assigned(CP) then
      try
        CP.Delete;
      except
      end;
    end;
  end;

  function DrawUpQuestion: string;
  begin
    result := 'Версия листа устарела (' + GetExcelSheetVersion(ExcelSheet)
      + '). Обновить до текущей версии надстройки (' + GetAddinVersion + ') ?';
  end;

var
  SheetVersion, ErrMsg: string;
begin
  if IsSequelSheet(ExcelSheet) then
  begin
    result := false;
    exit;
  end;
  UpdateNeed := IsNeedUpdateSheet(ExcelSheet, VersionRelation);
  if UpdateNeed then
  begin
    if DeleteFakePlaningData then
    begin
      result := true;
      UpdateNeed := false;
      VersionRelation := svModern;
      exit;
    end;
    result := false;
    if ShouldAskUser then
      if not ShowQuestion(DrawUpQuestion) then
        exit; //мы должны были спросить пользователя и он отказался продолжить.

    StartUpdateIndication;
    result := UpdateMetaData(ExcelSheet, DataProvider, SheetVersion);

    {Если было фактически произведено обновление, то пометим лист как "грязны"}
    if result then
    begin
      ExcelSheet.Application.ActiveWorkbook.Saved[0] := false;
      UpdateNeed := false;
      VersionRelation := svModern;
    end
    else
      if (SheetVersion = '1.0.8') then
        ErrMsg := 'Не удалось обновить лист. Обновление листов версии 1.0.8 не поддерживается.'
      else
        ErrMsg := ermUpdateSheetVersionFail;
    EndUpdateIndication(result, ErrMsg);
  end
  else
  begin
    result := VersionRelation <> svFuture;
    if (GetAddinVersion = '2.3.1') and (VersionRelation = svModern) then
      RestoreConstants(ExcelSheet.Parent as ExcelWorkbook);
  end;
end;

procedure TConverter.SetExternalLinks(OldMDDOM: IXMLDOMDocument2;
  ExcelSheet: ExcelWorksheet; DataProvider: IPlaningProvider);
begin
  FOldMDDOM := OldMDDOM;
  FExcelSheet := ExcelSheet;
  FProvider := DataProvider;
end;

function TConverter.UpdateTo222: boolean;
var
  GlobalPIDList: TStringList;
  PIDCounter: integer;

  function GetNewPID: string;
  begin
    inc(PIDCounter);
    result := IntToStr(PIDCounter);
  end;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i, Index: integer;
    ParamNode: IXMLDOMNode;
    ParamName, PID, Id: string;
  begin
    for i := 0 to NL.length - 1 do
    begin
      ParamNode := NL[i].selectSingleNode('paramProperties');
      if not Assigned(ParamNode) then
        continue;
      Id := GetStrAttr(NL[i], attrId, '');
      ParamName := GetStrAttr(ParamNode, 'name', '');
      Index := GlobalPIDList.IndexOfName(ParamName);
      if Index > -1 then
      begin
        PID := GlobalPIDList.Values[ParamName];
        try
          GetCPByName(FExcelSheet, Id, false).Delete;
        except
        end;
      end
      else
      begin
        PID := GetNewPID;
        GlobalPIDList.Add(ParamName + '=' + PID);
        RenameCP(FExcelSheet, Id, 'p' + PID);
      end;
      (ParamNode as IXMLDOMElement).setAttribute('pid', PID);
    end;
  end;

var
  NL: IXMLDOMNodeList;
  j: integer;
begin
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  GlobalPIDList := TStringList.Create;
  PIDCounter := 0;
  try
    NL := FOldMDDom.selectNodes('metadata/rows/row');
    UpdateCollection(NL);
  except
  end;
  try
    NL := FOldMDDom.selectNodes('metadata/columns/column');
    UpdateCollection(NL);
  except
  end;
  try
    NL := FOldMDDom.selectNodes('metadata/filters/filter');
    UpdateCollection(NL);
  except
  end;
  try
    NL := FOldMDDom.selectNodes('metadata/singlecells/singlecell/filters/filter');
    UpdateCollection(NL);
  except
  end;
  try
    (FOldMDDom.selectSingleNode('metadata/innerdata')
      as IXMLDOMElement).setAttribute(attrPIDCounter, PIDCounter);
  except
  end;
  for j := 1 to FExcelSheet.Comments.Count do
    FExcelSheet.Comments[j].Shape.Placement := xlMove;
  result := true;
end;

function TConverter.UpdateTo224: boolean;

  procedure CopyStylesForLevels;
  var
    i, j: integer;
    NL, NL2: IXMLDOMNodeList;
    ParentNode: IXMLDOMNode;
    ValueStyle, ValueStylePrint, TitleStyle, TitleStylePrint: string;
  begin
    NL := FOldMDDom.selectNodes('metadata//levels');
    for i := 0 to NL.length - 1 do
    begin
      ParentNode := NL[i].parentNode;
      ValueStyle := GetStrAttr(ParentNode, attrValueStyle, '');
      ValueStylePrint := GetStrAttr(ParentNode, attrValueStylePrint, '');
      TitleStyle := GetStrAttr(ParentNode, attrTitleStyle, '');
      TitleStylePrint := GetStrAttr(ParentNode, attrTitleStylePrint, '');
      (NL[i] as IXMLDOMElement).setAttribute(attrValueStyle, ValueStyle);
      (NL[i] as IXMLDOMElement).setAttribute(attrValueStylePrint, ValueStylePrint);
      (NL[i] as IXMLDOMElement).setAttribute(attrTitleStyle, TitleStyle);
      (NL[i] as IXMLDOMElement).setAttribute(attrTitleStylePrint, TitleStylePrint);
      NL2 := NL[i].selectNodes('./level');
      for j := 0 to NL2.length - 1 do
        with NL2[j] as IXMLDOMElement do
        begin
          setAttribute(attrValueStyle, ValueStyle);
          setAttribute(attrValueStylePrint, ValueStylePrint);
          setAttribute(attrTitleStyle, TitleStyle);
          setAttribute(attrTitleStylePrint, TitleStylePrint);
        end;
    end;
  end;

  {требуется пройти по всем листам в книге, вытащить из них константы,
    сформировать единую общую xml-ку и записать ее во все листы}
  procedure MoveConstants;
  var
    Book: ExcelWorkbook;
    ESheet: ExcelWorksheet;
    ConstDom, tmpDom: IXMLDOMDocument2;
    Root: IXMLDOMNode;
    i, j, Counter: integer;
    NL: IXMLDOMNodeList;
    AName: string;
  begin
    Book := FExcelSheet.parent as ExcelWorkbook;
    GetDomDocument(ConstDom);
    ConstDom.documentElement := ConstDom.createElement('consts');

    try
      Counter := 0;
      {сбор}
      for i := 1 to Book.Sheets.Count do
      begin
        ESheet := GetWorkSheet(Book.Sheets[1]);
        if not Assigned(ESheet) then
          continue;
        if not IsPlaningSheet(ESheet) then
          continue;
        if ESheet = FExcelSheet then
          tmpDom := FOldMDDom
        else
          tmpDom := GetDataFromCP(ESheet, cpMDName);
        Root := tmpDom.selectSingleNode('metadata/consts');
        if not Assigned(Root) then
          continue;
        NL := Root.selectNodes('./const');
        for j := 0 to NL.length - 1 do
        begin
          AName := GetStrAttr(NL[j], attrName, '');
          if Assigned(ConstDom.selectSingleNode(Format('//const[@name="%s"]', [AName]))) then
            continue;
          inc(Counter);
          ConstDom.documentElement.appendChild(NL[j].cloneNode(true));
        end;
        Root.parentNode.removeChild(Root);
        if ESheet <> FExcelSheet then
        begin
          PutDataInCP(ESheet, cpMDName, tmpDom);
          KillDomDocument(tmpDom);
        end;
      end;

      if Counter = 0 then
        exit;

      ConstDom.documentElement.setAttribute(attrCounter, Counter);
      {запись}
      for i := 1 to Book.Sheets.Count do
      begin
        ESheet := GetWorkSheet(Book.Sheets[i]);
        if not Assigned(ESheet) then
          continue;
        GetCPByName(ESheet, cpConstsName, true);
        PutDataInCP(ESheet, cpConstsName, ConstDom);
      end;
    finally
      KillDomDocument(ConstDom);
    end;
  end;

  procedure CorrectStyle(Style_: Style);
  begin
    if not Assigned(Style_) then
      exit;
    try
      Style_.Locked := false;
    except
    end;
  end;

  procedure CorrectStyles;
  var
    ExcelBook: ExcelWorkbook;
    tmpStyle: Style;
  begin
    ExcelBook := (FExcelSheet.Parent as ExcelWorkbook);
    if not Assigned(ExcelBook) then
      exit;
    if not ProtectPlaningSheets(ExcelBook, false) then
      exit;

    tmpStyle := GetStyleByName(ExcelBook, snDataFree);
    CorrectStyle(tmpStyle);

    tmpStyle := GetStyleByName(ExcelBook, snDataFreeErased);
    CorrectStyle(tmpStyle);

    tmpStyle := GetStyleByName(ExcelBook, snResultSingleCells);
    CorrectStyle(tmpStyle);

    tmpStyle := GetStyleByName(ExcelBook, snResultSingleCellsPrint);
    CorrectStyle(tmpStyle);
  end;

  procedure ReplaceOptions;
  var
    RowSummaryEnabled, ColumnSummaryEnabled,
    RowGrandSummaryEnabled, ColumnGrandSummaryEnabled,
    RowsBroken, ColumnsBroken,
    HideEmptyRows, HideEmptyColumns, SparseMode,
    PlaceRowMPBefore, PlaceColumnMPBefore: boolean;
    InnerDataNode, RowsNode, ColumnsNode, Node: IXMLDOMNode;
    Summary: TOldSummaryOptions;
    DisplayCommentCell: boolean;
  begin
    InnerDataNode := FOldMDDom.selectSingleNode('metadata/innerdata');
    RowsNode := FOldMDDom.selectSingleNode('metadata/rows');
    ColumnsNode := FOldMDDom.selectSingleNode('metadata/columns');

    {комментарии к ячейкам}
    DisplayCommentCell := GetBoolAttr(InnerDataNode, 'displaycommentcell', true);
    setAttr(InnerDataNode, attrDisplayCommentStructuralCell, BoolToStr(DisplayCommentCell));
    setAttr(InnerDataNode, attrDisplayCommentDataCell, BoolToStr(DisplayCommentCell));

    {настройки для оси строк}
    RowsBroken := GetBoolAttr(InnerDataNode, attrRowsBroken, false);
    PlaceRowMPBefore := GetBoolAttr(InnerDataNode, attrPlaceRowMPBefore, false);
    HideEmptyRows := GetBoolAttr(InnerDataNode, attrHideEmptyRows, false);
    with (RowsNode as IXMLDOMElement) do
    begin
      setAttribute(attrBroken, BoolToStr(RowsBroken));
      setAttribute(attrMPBefore, BoolToStr(PlaceRowMPBefore));
      setAttribute(attrHideEmpty, BoolToStr(HideEmptyRows));
      setAttribute(attrUseSummariesForElements, 'true');
    end;
    InnerDataNode.attributes.removeNamedItem(attrRowsBroken);
    InnerDataNode.attributes.removeNamedItem(attrPlaceRowMPBefore);
    InnerDataNode.attributes.removeNamedItem(attrHideEmptyRows);

    {итоги по строкам}
    RowSummaryEnabled := GetBoolAttr(InnerDataNode,  attrRowSummaryEnable, true);
    RowGrandSummaryEnabled := GetBoolAttr(InnerDataNode,  attrRowGrandSummaryEnable, true);
    Node := FOldMDDom.createNode(1, attrSummaryoptions, '');
    Summary := TOldSummaryOptions.Create;
    Summary.Enabled := RowSummaryEnabled;
    Summary.Title := 'Итоги';
    Summary.WriteToXML(Node);
    Node := RowsNode.appendChild(Node);
    Node := FOldMDDom.createNode(1, attrGrandSummaryoptions, '');
    Summary.Enabled := RowGrandSummaryEnabled;
    Summary.Title := 'Общие итоги';
    Summary.WriteToXML(Node);
    Node := RowsNode.appendChild(Node);
    InnerDataNode.attributes.removeNamedItem(attrRowSummaryEnable);
    InnerDataNode.attributes.removeNamedItem(attrRowGrandSummaryEnable);


    {настройки для оси столбцов}
    ColumnsBroken := GetBoolAttr(InnerDataNode, attrColumnsBroken, false);
    HideEmptyColumns := GetBoolAttr(InnerDataNode, attrHideEmptyColumns, false);
    PlaceColumnMPBefore := GetBoolAttr(InnerDataNode, attrPlaceColumnMPBefore, false);
    with (ColumnsNode as IXMLDOMElement) do
    begin
      setAttribute(attrBroken, BoolToStr(ColumnsBroken));
      setAttribute(attrMPBefore, BoolToStr(PlaceColumnMPBefore));
      setAttribute(attrHideEmpty, BoolToStr(HideEmptyColumns));
      setAttribute(attrUseSummariesForElements, 'true');
    end;
    InnerDataNode.attributes.removeNamedItem(attrColumnsBroken);
    InnerDataNode.attributes.removeNamedItem(attrPlaceColumnMPBefore);
    InnerDataNode.attributes.removeNamedItem(attrHideEmptyColumns);

    {итоги по столбцам}
    ColumnSummaryEnabled := GetBoolAttr(InnerDataNode,  attrColumnSummaryEnable, true);
    ColumnGrandSummaryEnabled := GetBoolAttr(InnerDataNode,  attrColumnGrandSummaryEnable, true);
    Node := FOldMDDom.createNode(1, attrSummaryOptions, '');
    Summary.Enabled := ColumnSummaryEnabled;
    Summary.Title := 'Итоги';
    Summary.WriteToXML(Node);
    Node := ColumnsNode.appendChild(Node);
    Node := FOldMDDom.createNode(1, attrGrandSummaryoptions, '');
    Summary.Enabled := ColumnGrandSummaryEnabled;
    Summary.Title := 'Общие итоги';
    Summary.WriteToXML(Node);
    Node := ColumnsNode.appendChild(Node);
    InnerDataNode.attributes.removeNamedItem(attrColumnSummaryEnable);
    InnerDataNode.attributes.removeNamedItem(attrColumnGrandSummaryEnable);

    SparseMode := GetBoolAttr(InnerDataNode, attrSparseMatrixMode, false);
    (InnerDataNode as IXMLDOMElement).setAttribute(
      attrTableProcessingMode, IIF(SparseMode, 1, 0));
    InnerDataNode.attributes.removeNamedItem(attrSparseMatrixMode);

    FreeAndNil(Summary);
  end;

begin
  result := true;

  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);

  ReplaceOptions;

  CorrectStyles;
  CopyStylesForLevels;

  MoveConstants;
end;

function TConverter.UpdateTo225: boolean;

  function IsItLeaf(Model: TSheetCollector; Params: TStringList) : boolean;
  var
    i, AxisIndex, LevelIndex: integer;
    AxisCollection: TSheetAxisCollectionInterface;
    MemberNode: IXMLDOMNode;
    XPath, LocalId: string;
    IsLeaf: boolean;
  begin
    result := false;
    if Pos(fpEnd, Params.Commatext) > 0 then
      exit;
    { Выясним принадлежность данного мембера}
    AxisIndex := Model.Rows.FindById(Params[1]);
    if AxisIndex > -1 then
      AxisCollection := Model.Rows
    else
    begin
      AxisIndex := Model.Columns.FindById(Params[1]);
      if AxisIndex > -1 then
        AxisCollection := Model.Columns
      else
        exit;
    end;

    { Все нижеследующее практически без изменений взято из коллектора.
      Со всеми вытекающими :)}
    if ((Params.Count - 3) < AxisCollection.MarkupFieldCount)
      and not AxisCollection.Broken then
    begin
      result := false;
      exit;
    end;

    if AxisCollection.Broken then
      if Params[1] <> AxisCollection[AxisCollection.Count - 1].UniqueID then
      begin
        result := false;
        exit;
      end;

    AxisIndex := 0;
    LevelIndex := 0;
    for i := 3 to Params.Count - 1 do
    begin
      if Params[i] = fpDummy then
        continue;

      if i < Params.Count - 1 then
      begin
        IsLeaf := (Params[i + 1] = fpDummy);
        if IsLeaf then
          LocalId := Params[i];
      end
      else
        IsLeaf := Params[i] = fpLeafEnd;

      if not IsLeaf then
      begin
        LocalId := Params[i];
        IsLeaf := (LevelIndex = AxisCollection[AxisIndex].Levels.Count - 1) or
          (AxisCollection[AxisIndex].IgnoreHierarchy and not AxisCollection.Broken) or
          (AxisCollection.Broken and (i = Params.Count - 1));
        if not IsLeaf then
        begin
          inc(LevelIndex);
          continue;
        end;
      end;

      if not Assigned(AxisCollection[AxisIndex].Members) then
        exit;
      XPath := Format('function_result/Members//Member[@%s="%s"]', [attrLocalId, LocalId]);
      MemberNode := AxisCollection[AxisIndex].Members.selectSingleNode(XPath);
      if not Assigned(MemberNode) then
        exit;
      if MemberNode.hasChildNodes then
        exit;

      inc(AxisIndex);
      LevelIndex := 0;
      if AxisIndex = AxisCollection.Count then
        break;
    end;
    result := true;
  end;

  function CorrectName(Model: TSheetCollector; RangeName: string): string;
  var
    Params: TStringList;
  begin
    result := RangeName;
    Params := nil;
    if not ParseExcelName(RangeName, Params) then
      exit;
    try
      if Params.Count < 3 then
        exit;
      Params[2] := BoolToStr(IsItLeaf(Model, Params));
      result := ConstructNameByParams(Params);
    finally
      FreeStringList(Params);
    end;
  end;

  procedure CorrectMarkupNames(Model: TSheetCollector);
  var
    ExcelSheet: ExcelWorksheet;
    EName: ExcelXP.Name;
    i: integer;
    ExcelNames: TStringList;
    StrName: string;
    ERange: ExcelRange;
  begin
    ExcelSheet := Model.ExcelSheet;
    { Кэшируем интересующие нас имена (для понимания см. UpdateTo221)}
    ExcelNames := TStringList.Create;
    try
      for i := 1 to FExcelSheet.Names.Count do
      begin
        EName := FExcelSheet.Names.Item(i, EmptyParam, EmptyParam);
        StrName := GetShortSheetName(EName.Name_);
        if IsNameOurs(StrName) then
          if Pos(sntMember, EName.Name_) > 0 then
            ExcelNames.Add(StrName);
      end;
      { Изменяем и записываем. Просто переименовать старое имя нельзя -
        теряется привязка к диапазону. Нужно перемаркировывать.}
      for i := 0 to ExcelNames.Count - 1 do
      begin
        StrName := CorrectName(Model, ExcelNames[i]);
        EName := GetNameObject(ExcelSheet, ExcelNames[i]);
        ERange := GetRangeByName(ExcelSheet, ExcelNames[i]);
        EName.Delete;
        MarkObject(ExcelSheet, ERange, StrName, false);
      end
    finally
      FreeStringList(ExcelNames);
    end;
  end;

  {Если во всем диапазоне показателя одинаковая типовая формула, возвращаем ее}
  function GetTypeFormula_(Total: TSheetTotalInterface; TotalRange: ExcelRange): TTypeFormula;

    function GetParamValue_(SheetInterface: TSheetInterface; Column: integer): string;
    var
      AllTotals, TotalRange: ExcelRange;
      RangeName: string;
      Params: TStringList;
    begin
      result := '';
      AllTotals := GetRangeByName(SheetInterface.ExcelSheet, BuildExcelName(sntTotals));
      TotalRange := GetIntersection(SheetInterface.ExcelSheet, AllTotals,
        GetRange(SheetInterface.ExcelSheet, 1, Column, SheetInterface.ExcelSheet.Rows.count, Column));
      if not Assigned(TotalRange) then
        exit;

      RangeName := GetNameByRange(SheetInterface.ExcelSheet, TotalRange);
      try
        if not ParseExcelName(RangeName, Params) then
          exit;
        result := 'T_' + Params[1];
      finally
        FreeStringList(Params);
      end;
    end;


    {Это переписанная версия uFormulasUtils.AddEncodedTypeFormula из версии 227,
      с выкинутыми упоминаниями о ссылках на МП, которых в 224 просто еще не было.}
    procedure AddEncodedTypeFormula_(Collector: TSheetInterface; Formula: string;
      var FormulaElement: IXMLDOMNode; CurRow: integer);

      procedure SetParamAttributes(Node: IXMLDOMNode; ParamType: TFormulaParamType;
        ParamValue, Offset: string; IsOtherSheet: boolean);
      begin
        SetAttr(Node, attrParamType, Ord(ParamType));
        SetAttr(Node, attrParamValue, ParamValue);
        if Offset <> '' then
          SetAttr(Node, attrOffset, Offset);
        SetAttr(Node, attrIsOtherSheet, BoolToStr(IsOtherSheet));
      end;

      procedure GetOffsets_(ExcelSheet: ExcelWorkSheet; out FirstRow, LastRow, FirstColumn,
        LastColumn, RowsLeaf, ColumnsLeaf: integer);
      var
        Range: ExcelRange;
      begin
        FirstRow := 0;
        LastRow := 0;
        FirstColumn := 0;
        LastColumn := 0;
        RowsLeaf := 0;
        Columnsleaf := 0;
        if not Assigned(ExcelSheet) then
          exit;
        Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntTotals);
        if Assigned(Range) then
        begin
          FirstRow := Range.Row;
          LastRow := Range.Row + Range.Rows.Count - 1;
          FirstColumn := Range.Column;
          LastColumn := Range.Column + Range.Columns.Count - 1;
        end;
        Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntRows);
        if Assigned(Range) then
          RowsLeaf := Range.Column + Range.Columns.Count - 1;
        Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntColumns);
        if Assigned(Range) then
          ColumnsLeaf := Range.Row + Range.Rows.Count - 1;
      end;

      procedure ParseCellRef_(ExcelSheet: ExcelWorkSheet; CellRef: string;
        out ReplacedCellRef, Column, Row: string;
        out ColumnNumber, RowNumber: integer;
        out IsOtherSheet, IsAbsolute: boolean);
      begin
        ReplacedCellRef := '';
        IsOtherSheet := (CellRef[1] = '!');
        if IsOtherSheet then
          Delete(CellRef, 1, 1);
        ReplacedCellRef := StringReplace(CellRef, '|', '', [rfReplaceAll]);
        IsAbsolute := false;
        while Pos('$', CellRef) > 0 do
        begin
          IsAbsolute := true;
          Delete(CellRef, Pos('$', CellRef), 1);
        end;
        Column := copy(CellRef, 1, Pos('|', CellRef) - 1);
        ColumnNumber := GetColumnIndex(Column);
        if (not IsOtherSheet) and
          (ColumnNumber = GetGrandSummaryColumn(ExcelSheet)) then
          Column := gsColumn;
        Row := copy(CellRef, Pos('|', CellRef) + 1, Length(CellRef));
        RowNumber := StrToInt(Row);
      end;

    var
      ParamElement: IXMLDOMNode;
      FormulaCellRefs: TStringList;
      ReplacedCellRef, Column, Row: string;
      i, RowNumber, ColumnNumber, GrandSummaryRow: integer;
      Offset, ParamName, Template, SingleCellID, TotalAlias: string;
      IsOtherSheet, IsAbsolute: boolean;
      FirstRow, LastRow, FirstColumn, LastColumn, RowsLeaf, ColumnsLeaf: integer;
      ExcelSheet: ExcelWorksheet;
    begin
      ExcelSheet := Collector.ExcelSheet;


      GetOffsets_(ExcelSheet, FirstRow, LastRow, FirstColumn,
        LastColumn, RowsLeaf, ColumnsLeaf);
      Template := Formula;
      FormulaCellRefs := GetFormulaCellRefs(Formula);
      GrandSummaryRow := GetGrandSummaryRow(ExcelSheet);

      {Все ссылки в формуле пытаемся перевести в параметры.
        Допустимы ссылки на ячейки табличных и отдельных показателей.}
      for i := 0 to FormulaCellRefs.Count - 1 do
      begin
        ParseCellRef_(ExcelSheet, FormulaCellRefs[i], ReplacedCellRef,
          Column, Row, ColumnNumber, RowNumber, IsOtherSheet, IsAbsolute);
        ParamName := IIF((i < 10), 'param0', 'param') + IntToStr(i);

        ParamElement := CreateAndAddChild(FormulaElement, ParamName);

        {Обработка ссылки на отдельный показатель}
        if Collector.WritablesInfo.IsSingleCellSelected(ExcelSheet,
          RowNumber, ColumnNumber, SingleCellID) then
        begin
          SetParamAttributes(ParamElement, fptSingleCell, 'S_' + SingleCellID, '', IsOtherSheet);
          Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
          continue;
        end;

        Offset := Row;
        if not IsOtherSheet then
          if StrToInt(Row) = GrandSummaryRow then
            Offset := gsRow
          else
            if IsAbsolute then
              Offset := IntToStr(StrToInt(Row) - FirstRow)
            else
              Offset := IntToStr(StrToInt(Row) - CurRow);

        if (StrToInt(Row) < FirstRow) or (StrToInt(Row) > LastRow) or
          (ColumnNumber < FirstColumn) or (ColumnNumber > LastColumn) then
        begin
          SetParamAttributes(ParamElement, fptFreeCell, ReplacedCellRef, '', IsOtherSheet);
          Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
        end
        else
        begin
          {Обработка ссылки на табличный показатель}
          // ссылки допускаем только в пределах таблицы
          TotalAlias := GetParamValue_(Collector, ColumnNumber);
          { если показатель удалили - заменяем его на #ССЫЛКА! = fmIncorrectRef}
          if TotalAlias = '' then
          begin
            Template := ReplaceCellRef(Template, ReplacedCellRef, fmIncorrectRef);
            continue;
          end;
          if IsAbsolute then
            SetParamAttributes(ParamElement, fptTotalAbsolute, Total.Alias, Offset, IsOtherSheet)
          else
            SetParamAttributes(ParamElement, fptTotal, Total.Alias, Offset, IsOtherSheet);
          Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
        end;

      end;
      SetAttr(FormulaElement, attrTemplate, Template);
    end;


    function GetTypeFormula_(Total: TSheetTotalInterface; Row,
      Column: integer): TTypeFormula;
    var
      DOM: IXMLDOMDocument2;
      FormulaNode: IXMLDOMNode;
      LocalFormula: string;
    begin
      result := nil;
      if not Assigned(Total) then
        exit;
      result := TTypeFormula.Create(Total);
      DOM := InitXmlDocument;
      try
        FormulaNode := DOM.CreateNode(1, attrTypeFormula, '');
        if not GetCellFormula(Total.SheetInterface.ExcelSheet, Row, Column, LocalFormula) then
          exit;
        AddEncodedTypeFormula_(Total.SheetInterface, LocalFormula, FormulaNode, Row);
        result.ReadFromXML(FormulaNode);
      finally
        FormulaNode := nil;
        KillDOMDocument(DOM);
      end;
    end;

  var
    ExcelSheet: ExcelWorksheet;
    PreviousTypeFormula, TypeFormula: TTypeFormula;
    r, c: integer;
    ErrorText: string;
  begin
    result := nil;
    if not Assigned(TotalRange) then
      exit;
    ExcelSheet := TotalRange.Worksheet;
    c := TotalRange.Column;
    PreviousTypeFormula := GetTypeFormula_(Total, TotalRange.Row, c);
    if not Assigned(PreviousTypeFormula) then
      exit;
    for r := TotalRange.Row + 1 to TotalRange.Row + TotalRange.Rows.Count - 1 do
    begin
      TypeFormula := GetTypeFormula_(Total, r, c);
      if not Assigned(TypeFormula) then
        exit;
      if not PreviousTypeFormula.IsEqual(TypeFormula, false) then
        exit;
      PreviousTypeFormula := TypeFormula;
    end;
    if not(Assigned(PreviousTypeFormula) and PreviousTypeFormula.IsValid(ErrorText)) then
      exit;
    result := PreviousTypeFormula;
    result.Enabled := true;
  end;

  procedure DetectTypeFormulas(var Model: TSheetCollector);
  var
    ExcelSheet: ExcelWorksheet;
    CurrentTotal: TSheetTotalInterface;
    TotalRange: ExcelRange;
    TotalNode, TypeFormulaNode: IXMLDOMNode;
    TypeFormula: TTypeFormula;
    i, j: integer;
  begin
    ExcelSheet := Model.ExcelSheet;
    for i := 0 to Model.Totals.Count - 1 do
    begin
      CurrentTotal := Model.Totals[i];
      if not(CurrentTotal.TotalType in [wtResult, wtFree]) then
        continue;
      TotalNode := FOldMDDOM.selectSingleNode('metadata/totals/total[@id="' +
        CurrentTotal.UniqueID + '"]');
      if CurrentTotal.TypeFormula.Enabled then
        continue;
      for j := 0 to CurrentTotal.SectionCount - 1 do
      begin
        TotalRange := CurrentTotal.GetTotalRangeWithoutGrandSummary(j);
        TypeFormula := GetTypeFormula_(CurrentTotal, TotalRange);
        if Assigned(TypeFormula) then
        begin
          {как только нашли типовую формулу, применяем ее к показателю}
          TypeFormulaNode := TotalNode.selectSingleNode(attrTypeFormula);
          if not Assigned(TypeFormulaNode) then
            TypeFormulaNode := CreateAndAddChild(TotalNode, attrTypeFormula);
          TypeFormula.WriteToXML(TypeFormulaNode);
            break;
        end;
      end;
    end;
  end;

var
  Collector: TSheetCollector;
begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);

  { В версии произошло изменение имен разметки мемберов. Вместо устаревшего
    параметра, отвечавшего за режим свертки (Vis или Hid, работал только
    до первого рефреша, никем не использовался, практически был фиктивным)
    был введен признак пригодности элемента для обратной записи -
    по сути своей индикатор, является ли элемент истинно листовым. На этот
    признак, в частности, теперь завязана процедура сбора свободных -
    итоги не собираются. Чтобы данные свободных не пропали, необходимо
    обновить разметку.}

  Collector := TSheetCollector.Create;
  try
    Collector.LoadMode := lmNoFreeData;
    Collector.LoadFromXml(FOldMDDOM);
    Collector.ExcelSheet := FExcelSheet;
    CorrectMarkupNames(Collector);
    DetectTypeFormulas(Collector);
  finally
    FreeAndNil(Collector);
  end;
end;

function TConverter.UpdateTo226: boolean;

  procedure UpdateCollection(NL: IXMLDOMNodeList);
  var
    i: integer;
    Members: IXMLDOMDocument2;
    Id: string;
  begin
    for i := 0 to NL.length - 1 do
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      if Id = '' then
        continue;
      Members := GetDataFromCP(FExcelSheet, Id);
      if Assigned(Members) then
      begin
        CopyInfluences(Members);
        PutDataInCP(FExcelSheet, Id, Members);
      end;
    end;
  end;

  { В версии 2.2.6 был изменен механизм хранения информации о редактируемых
    пользователям диапазонах (свободные и результаты). Вместо внесения в лист
    разметки составных диапазонов, имевшей порочные ограничения, сведения о
    перезаписываемых областях теперь хранятся в метаданных листа.}
  procedure GetWritableRangesInfo;
  var
    i: integer;
    AllTotalsRange, TotalRange: ExcelRange;
    WritableColumns: TByteSet;
    RangeName, SingleCellsNames: string;
    List: TStringList;
    NoTotals: boolean;
    InnerDataNode: IXMLDOMNode;
  begin
    WritableColumns := [];
    SingleCellsNames := '';
    AllTotalsRange := GetRangeByName(FExcelSheet, BuildExcelName(sntTotals));
    NoTotals := not Assigned(AllTotalsRange);
    for i := 1 to FExcelSheet.Names.Count do
    begin
      RangeName := FExcelSheet.Names.Item(i, EmptyParam, EmptyParam).Name_;
      if not IsNameOurs(RangeName) then
        continue;
      ParseExcelName(RangeName, List);
      {Показатели}
      if not NoTotals then
        if (List[0] = sntTotalFree) or (List[0] = sntTotalResult) then
        begin
          TotalRange := GetRangeByName(FExcelSheet, RangeName);
          if not Assigned(TotalRange) then
            continue;
          Include(WritableColumns, TotalRange.Column - AllTotalsRange.Column);
          continue;
        end;
      {Отдельные}
      if (List[0] = sntSingleCellMeasure) or
        (List[0] = sntSingleCellResult) or
        (List[0] = sntSingleCellConst) then
      begin
        AddTail(SingleCellsNames, ',');
        SingleCellsNames := SingleCellsNames + RangeName;
      end;
    end;
    InnerDataNode := FOldMDDom.selectSingleNode('metadata/innerdata');
    with (InnerDataNode as IXMLDOMElement) do
    begin
      setAttribute('writablecolumns', ByteSetToString(WritableColumns));
      setAttribute('singlecellsnames', SingleCellsNames);
    end;
    FreeStringList(List);
  end;

var
  NL: IXMLDOMNodeList;
begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);

  {Т.к. все элементы, не предназначенные для размещения, удаляются из XML, а значит
  и признаки присущие элементу тоже удаляются, было решено сохранять некоторые из
  них в отдельной секции. Переносим признак зависимости (influence) у элемента
  в отдельную секцию}
  NL := FOldMDDom.selectNodes('metadata/columns/column');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/rows/row');
  UpdateCollection(NL);
  NL := FOldMDDom.selectNodes('metadata/filters/filter');
  UpdateCollection(NL);
  { У стилей для редактируемых данных признак Locked теперь true.
    Надо пересоздать эти стили, чтобы защита листа отрабатывала правильно.}
  if SetBookProtection(FExcelSheet.Parent as ExcelWorkbook, false) then
    LockStyles(FExcelSheet.Parent as ExcelWorkbook);
  GetWritableRangesInfo;
end;

function TConverter.UpdateTo227(NeedUpdateTypeFormulas: boolean): boolean;

  procedure UpdateTypeFormulas;
  var
    NL: IXMLDOMNodeList;
    i: integer;
    Offset, ParamValue, ParamName, NewParamName, Template: string;
    ParamType: TFormulaParamType;
    FormulaNode, NewNode: IXMLDOMNode;
  begin
    {выбираем узлы параметров типовых формул}
    NL := FOldMDDom.selectNodes('metadata/totals//typeformula/*');
    for i := 0 to NL.length - 1 do
    begin
      Offset := GetStrAttr(NL[i], attrOffset, '');
      if Offset = snSingleCell then
        ParamType := fptSingleCell
      else
        ParamType := fptTotal;

      ParamValue := GetStrAttr(NL[i], attrTotalAlias, '');
      NL[i].attributes.removeNamedItem(attrTotalAlias);
      SetAttr(NL[i], attrParamType, Ord(ParamType));
      SetAttr(NL[i], attrParamValue, ParamValue);

      {В прежних версиях "с баксами могли быть только ссылки на общие итоги".
        Поскольку теперь введены абсолютные ссылки внутри области показателей,
        необходимости в префиксах имен параметров больше нет.}
      if (Offset = gsRow) then
      begin
        ParamName := NL[i].nodeName;
        if Pos('B', ParamName) = 1 then
        begin
          FormulaNode := NL[i].parentNode;
          Template := GetStrAttr(FormulaNode, 'template', '');
          NewParamName := StringReplace(ParamName, 'B', '', [rfReplaceAll]);
          NewNode := NL[i].ownerDocument.createNode(1, NewParamName, '');
          CopyAttrs(NL[i], NewNode);
          FormulaNode.insertBefore(NewNode, NL[i]);
          FormulaNode.removeChild(NL[i]);
          Template := StringReplace(Template, ParamName, NewParamName, [rfReplaceAll]);
          SetAttr(FormulaNode, 'template', Template);
        end;
      end; 

    end;
  end;

  procedure CollectTotalSections;
  var
    i, FirstColumn: integer;
    EName: ExcelXP.Name;
    ERange: ExcelRange;
    AName, AType, Id, Section: string;
    TmpSections: TStringList;
  begin
    {Получим общий дипазон всех показателей - от его начала считаем смещение}
    ERange := GetRangeByName(FExcelSheet, BuildExcelName(sntTotals));
    if not Assigned(ERange) then
      exit;
    TmpSections := TStringList.Create;
    FirstColumn := ERange.Column;
    for i := 1 to ERange.Columns.Count do
      TmpSections.Add('');
    {Выбираем имена секций показателей}
    for i := 1 to FExcelSheet.Names.Count do
    begin
      EName := FExcelSheet.Names.Item(i, VarNull, VarNull);
      AName := GetShortSheetName(EName.Name_);
      if not IsNameOurs(AName) then
        continue;
      CutPart(AName, snSeparator);
      AType := CutPart(AName, snSeparator);
      if not ((AType = sntTotalFree) or (AType = sntTotalMeasure) or
        (AType = sntTotalResult) or (AType = sntTotalConst)) then
        continue;
      {защита от старых имен, объединяющих все секции показателя}
      if Pos(snSeparator, AName) = 0 then
        continue;
      ERange := EName.RefersToRange;
      if not Assigned(ERange) then
        continue;
      {!! Что будет, если не найдет диапазон?}
      Id := CutPart(AName, snSeparator);
      Section := CutPart(AName, snSeparator);
      TmpSections[ERange.Column - FirstColumn] := Id + snSeparator + Section;
    end;
    try
      SetAttr(FoldMDDom.selectSingleNode('metadata/innerdata'), attrTotalSections, TmpSections.CommaText);
    except
    end;
    FreeAndNil(TmpSections);
  end;

begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  CollectTotalSections;
  if NeedUpdateTypeFormulas then
    UpdateTypeFormulas;
end;

function TConverter.UpdateTo230: boolean;
var
  InnerDataNode: IXMLDOMNode;
  MarkerAttr: boolean;
begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  InnerDataNode := FoldMDDom.selectSingleNode('metadata/innerdata');
  MarkerAttr := GetBoolAttr(InnerDataNode, attrIsMarkerOnTheRight, false);
  InnerDataNode.attributes.removeNamedItem(attrIsMarkerOnTheRight);
  SetAttr(InnerDataNode, attrMarkerPosition, IIF(MarkerAttr, Ord(mpRight), Ord(mpLeft)));
end;

function TConverter.UpdateTo231: boolean;
begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  RestoreConstants(FExcelSheet.Parent as ExcelWorkbook);
end;


{В версию 2.3.0 была привнесена ошибка, приводившая к затиранию всего содержимого
xml-ки констант и к проблемам из-за этого. Данная процедура пытается
восстановить эту xml-ку, собрав данные о размещенных в листе константах из
коллекций показателей и отдельных ячеек.}
procedure RestoreConstants(Book: ExcelWorkbook);

  {проверка необходимости восстановления констант.
    Критерий - в xml-ке констант накручен счетчик и отсутствуют сами константы.
    Хотя такое могло быть следствием простого удаления констант штатным путем.}
  function CheckConstXml: boolean;
  var
    i, UIDCounter: integer;
    Dom: IXMLDOMDocument2;
    Sheet: ExcelWorkSheet;
    Root: IXMLDOMNode;
  begin
    result := false;
    Dom := nil;
    for i := 1 to Book.Sheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Worksheets[i]);
      if not Assigned(Sheet) then
        continue;
      Dom := GetDataFromCP(Sheet, cpConstsName);
      break;
    end;
    if not Assigned(Dom) then
      exit;
    Root := Dom.documentElement;
    if not Assigned(Root) then
    begin
      result := true;
      exit;
    end;
    UIDCounter := GetIntAttr(Root, attrCounter, 0);
    result := (UIDCounter > 0) and (Root.childNodes.length = 0)
  end;

  procedure AddConst(Root: IXMLDOMNode; AName, AValue: string);
  var
    UIDCounter: integer;
    Node: IXMLDOMNode;
  begin
    UIDCounter := GetIntAttr(Root, attrCounter, 0);
    inc(UIDCounter);
    Node := Root.ownerDocument.createElement('const');
    Root.appendChild(Node);
    SetAttr(Node, attrId, -1);
    SetAttr(Node, attrName, AName);
    SetAttr(Node, 'comment', '');
    SetAttr(Node, 'value', AValue);
    SetAttr(Node, 'isinherited', '0');
    SetAttr(Node, 'uniqueID', UIDCounter);
    SetAttr(Node, 'issheetconst', 'true');
    SetAttr(Root, attrCounter, UIDCounter);
  end;

  function ConstExists(Root: IXMLDOMNode; AName: string): boolean;
  begin
    result := Assigned(Root.selectSingleNode(Format('const[@name="%s"]', [AName])));
  end;

  procedure RegainConsts(Sheet: ExcelWorkSheet; Root: IXMLDOMNode);

    function GetSingleCellConstValue(Node: IXMLDOMNode): string;
    var
      UniqueId, AName, Formula, Style: string;
      ERange: ExcelRange;
    begin
      result := '';
      {составляем имя отдельной, которым должен быть маркирован ее диапазон}
      UniqueId := GetStrAttr(Node, attrID, '');
      AName := BuildExcelName(sntSingleCellConst + snSeparator + UniqueId);
      ERange := GetRangeByName(Sheet, AName);
      if not Assigned(ERange) then
        exit;
      {если нашли диапазон, достаем значение}
      GetCellValue(ERange, '', result, Formula, Style, false)
    end;

    function GetTotalConstValue(Node: IXMLDOMNode): string;
    var
      UniqueId, AName, Formula, Style, TotalFormat: string;
      ERange: ExcelRange;
    begin
      result := '';
      {составляем имя диапазона первой секции показателя}
      UniqueId := GetStrAttr(Node, attrID, '');
      TotalFormat := GetStrAttr(Node, attrFormat, '0');
      AName := BuildExcelName(sntTotalConst + snSeparator + UniqueId)+
        snSeparator + '0' + snSeparator + TotalFormat;;
      ERange := GetRangeByName(Sheet, AName);
      if not Assigned(ERange) then
        exit;
      {из первой ячейки найденного диапазона достаем значение}
      ERange := ExcelRange(TVarData(ERange.Cells.Item[1, 1]).VDispatch);
      GetCellValue(ERange, '', result, Formula, Style, false);
    end;

  var
    SheetMD: IXMLDOMDocument2;
    NL: IXMLDOMNodeList;
    i: integer;
    Name, Value: string;
  begin
    SheetMD := GetDataFromCP(Sheet, cpMDName);
    {выбираем отдельные-константы}
    NL := SheetMD.selectNodes(Format('metadata/singlecells/singlecell[@totaltype="%d"]',
      [Ord(wtConst)]));
    {в принципе, все что нам надо узнать, это имя константы (берем из заголовка
      отдельной) и значение (достаем из сетки).}
    for i := 0 to NL.length - 1 do
    begin
      Name := GetNodeStrAttr(NL[i], attrCaption);
      if ConstExists(Root, Name) then
        continue;
      Value := GetSingleCellConstValue(NL[i]);
      AddConst(Root, Name, Value);
    end;

    {теперь проверим показатели}
    NL := SheetMD.selectNodes(Format('metadata/totals/total[@totaltype="%d"]',
      [Ord(wtConst)]));
    for i := 0 to NL.length - 1 do
    begin
      Name := GetNodeStrAttr(NL[i], attrCaption);
      if ConstExists(Root, Name) then
        continue;
      Value := GetTotalConstValue(NL[i]);
      AddConst(Root, Name, Value);
    end;
  end;

var
  ConstsDom: IXMLDOMDocument2;
  Sheet: ExcelWorkSheet;
  i: integer;
begin
  if not CheckConstXml then
    exit;
  ConstsDom := InitXmlDocument;
  try
    ConstsDom.documentElement := (ConstsDom.createNode(1, 'consts', '') as IXmlDomElement);

    {первый проход по листам книги - собираем информацию о размещенных константах}
    for i := 1 to Book.Worksheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Worksheets[i]);
      if not Assigned(Sheet) then
        continue;
      if not IsPlaningSheet(Sheet) then
        continue;
      RegainConsts(Sheet, ConstsDom.documentElement);
    end;

    {второй проход - записываем в листы созданную xml-ку}
    for i := 1 to Book.Worksheets.Count do
    begin
      Sheet := GetWorkSheet(Book.Worksheets[i]);
      if not Assigned(Sheet) then
        continue;
      PutDataInCP(Sheet, cpConstsName, ConstsDom);
    end;
  finally
    KillDomDocument(ConstsDom);
  end;
end;

function TConverter.UpdateTo232: boolean;

  {из комбинации старых атрибутов Enabled и AtBeginning получим значение
    нового атрибута Deployment}
  procedure UpdateSummaryOptions;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    Enabled, AtBeginning: boolean;
    Deployment: TItemDeployment;
  begin           
    NL := FOldMDDom.selectNodes('metadata//summaryoptions|metadata//grandsummaryoptions');
    for i := 0 to NL.length - 1 do
    begin
      Enabled := GetBoolAttr(NL[i], attrEnabled, false);
      if not Enabled then
      begin
        Deployment := idNone;
      end
      else
      begin
        AtBeginning := GetBoolAttr(Nl[i], 'atbeginning', false);
        if AtBeginning then
          Deployment := idTop
        else
          Deployment := idBottom;
      end;
      SetAttr(NL[i], 'deployment', Ord(Deployment));
      NL[i].attributes.removeNamedItem(attrEnabled);
      NL[i].attributes.removeNamedItem('atbeginning');
    end;
  end;

  procedure UpdateLevels;
  var
    i: integer;
    NL: IXMLDOMNodeList;
    Deployment: TItemDeployment;
    HideDM, OwnersHideDM: boolean;
  begin
    NL := FOldMDDom.selectNodes('metadata//level');
    for i := 0 to NL.length - 1 do
    begin
      OwnersHideDM := GetBoolAttr(NL[i].parentNode.parentNode, attrHideDataMembers, true);
      HideDM := GetBoolAttr(NL[i], attrHideDataMembers, true);
      if HideDM and OwnersHideDM then
        Deployment := idNone
      else
        Deployment := idTop;
      SetAttr(NL[i], attrDeployment, Ord(Deployment));
      NL[i].attributes.removeNamedItem(attrHideDataMembers);
    end;
  end;

begin
  result := true;
  {эта шляпа здесь для обхода непонятной проблемы с размножением СР}
  RecreateCp(FExcelSheet);
  UpdateSummaryOptions;
  UpdateLevels;
end;

function TConverter.UpdateTo234: boolean;

  procedure UpdateAttrs;
    var
      i: integer;
      NL: IXMLDOMNodeList;
      Node: IXMLDOMNode;
      Bool: boolean;
  begin
    {для табличных показателей, единая опция}
    Node := FOldMDDom.selectSingleNode('metadata/innerdata');
    Bool := GetBoolAttr(Node, attrTotalsInThousand, false);
    SetAttr(Node, attrTotalMultiplier, IIF(Bool, Ord(tmE3), Ord(tmE1)));
    Node.attributes.removeNamedItem(attrTotalsInThousand);
    Node.attributes.removeNamedItem(attrMultiplicationFlag);

    {для отдельных  - индивидуальные настройки}
    NL := FOldMDDom.selectNodes('metadata/singlecells/singlecell');
    for i := 0 to NL.length - 1 do
    begin
      Bool := GetBoolAttr(NL[i], attrTotalsInThousand, false);
      SetAttr(NL[i], attrTotalMultiplier, IIF(Bool, Ord(tmE3), Ord(tmE1)));
      NL[i].attributes.removeNamedItem(attrTotalsInThousand);
      NL[i].attributes.removeNamedItem(attrMultiplicationFlag);
    end;
  end;

  {В 2.3.4 изменился механизм хранения параметров.
    Вместо хранения отдельных копий в узле каждого параметризованного измерения
    будем хранить коллекцию централизованно, в отдельной секции,
    а в измерениях оставим лишь ссылки.}
  procedure UpdateParameters;
  var
    NL: IXMLDOMNodeList;
    i, j: integer;
    NewRoot, ParamNode: IXMLDOMNode;
    Pid, AttrName, Id, tmpStr: string;
    List: TStringList;
  begin
    {выбираем все ссылки на параметры}
    NL := FOldMDDom.selectNodes('//paramProperties');
    if NL.length = 0 then
      exit;
    NewRoot := FOldMdDom.createNode(1, 'params', '');
    FOldMdDom.documentElement.appendChild(NewRoot);
    List := TStringList.Create;

    {действуем в два этапа. На первом заполняем секцию параметров,
      копируя в нее старые узлы лишь по одному разу.}
    for i := 0 to NL.length - 1 do
    begin
      Pid := GetStrAttr(NL[i], 'pid', '');
      {если еше не обрабатывали - копируем узел параметра}
      if List.IndexOf(Pid) = -1 then
      begin
        ParamNode := FOldMdDom.createNode(1, 'param', '');
        NewRoot.appendChild(ParamNode);
        CopyAttrs(NL[i], ParamNode);
        List.Add(Pid);
      end;

      {в старом узле оставим только идентификатор параметра}
      for j := NL[i].attributes.length - 1 downto 0 do
      begin
        AttrName := NL[i].attributes[j].nodeName;
        if AttrName = 'pid' then
          continue;
        NL[i].attributes.removeNamedItem(AttrName);
      end;
    end;

    {на втором этапе для каждого параметра формируем списки ссылающихся
      на него элементов}
    for i := 0 to NewRoot.childNodes.length - 1 do
    begin
      ParamNode := NewRoot.childNodes[i];
      Pid := GetStrAttr(ParamNode, 'pid', '');
      NL := FOldMDDom.selectNodes(Format('//*[paramProperties[@pid=%s]]', [Pid]));
      List.Clear;
      for j := 0 to NL.length - 1 do
      begin
        Id := GetStrAttr(NL[j], attrId, '');
        if Id <> '' then
          List.Add(Id);
      end;
      if List.Count = 0 then
        continue;
      tmpStr := CommaTextToString(List.CommaText);
      SetAttr(ParamNode, 'links', tmpStr);
    end;
  end;

  procedure UpdateTaskId;
  var
    TaskId: string;
    Book: ExcelWorkbook;
    Node: IXMLDOMNode;
  begin
    Book := FExcelSheet.Parent as ExcelWorkbook;
    if Assigned(Book) then
      TaskId := GetWBCustomPropertyValue(Book.CustomDocumentProperties, pspTaskID);
    Node := FOldMdDom.selectSingleNode('metadata/innerdata');
    setAttr(Node, 'taskid', TaskId);
  end;

begin
  result := true;
  RecreateCp(FExcelSheet);
  UpdateAttrs;
  UpdateParameters;
  //UpdateTaskId;
end;

end.
