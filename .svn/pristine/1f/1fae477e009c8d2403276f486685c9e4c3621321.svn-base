unit uSheetConst;

interface

uses Classes, SysUtils, uXMLCatalog, uSheetObjectModel, MSXML2_TLB,
    uFMAddinExcelUtils, uFMExcelAddinConst,
    uXMLUtils, uFMAddinGeneralUtils, ExcelXP, PlaningTools_TLB, uExcelUtils,
    uSheetHistory, uGlobalPlaningConst;

type

  TConst = class(TConstInterface)
  private
    procedure PrepareTaskConstUpdate;
  public
    procedure WriteToXML(Node: IXMLDOMNode); override;
    function ReadFromXML(Node: IXMLDOMNode): boolean; override;
    function IsSheetElement: boolean; override;
    procedure SyncSheetConsts(OldName, NewName: string); override;
    function GetPresenceInTask(out TaskConst: TTaskConst): TParamPresence; override;
  end;

  TConstCollection = class(TConstCollectionInterface)
  private
    FConstsText: string;
    procedure UpdateTaskConsts;
  protected
    function GetItem(Index: integer): TConstInterface; override;
    procedure SetItem(Index: integer; Item: TConstInterface); override;
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    function Append: TConstInterface; override;
    procedure Delete(Index: integer); override;
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;
    function GetCollectionName: string; override;
    function ConstByID(ID: integer): TConstInterface; override;
    function ConstByName(Name: string): TConstInterface; override;
    function FindByID(ID: string): integer; override;
    function Validate: boolean; override;
    function Refresh(Force: boolean): boolean; override;
  end;

implementation

{ TConstCollection }

function TConstCollection.Append: TConstInterface;
begin
  result := TConst.Create(Self);
  result.UniqueId := GetUniqueID;
  inherited Add(result);
end;

procedure TConstCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TConstCollection.ConstByID(ID: integer): TConstInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if (Items[i].UniqueID = ID) then
    begin
      result := Items[i];
      break;
    end;
end;

function TConstCollection.GetCollectionName: string;
begin
  result := 'consts';
end;

function TConstCollection.GetItem(Index: integer): TConstInterface;
begin
  result := Get(Index);
end;

procedure TConstCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Constant: TConstInterface;
  TaskConsts: TTaskConstsCollection;
  TaskConst: TTaskConst;
  IdList: TStringList;
  ConstId: string;
begin
  FConstsText := '';
  Clear;
  IdList := TStringList.Create;
  if Assigned(Node) then
  begin
    // загружаем константы, расположенные на листе
    FCounter := GetIntAttr(Node, 'counter', 0);
    NL := Node.selectNodes('const');
    for i := 0 to NL.length - 1 do
    begin
      {профилактика размножения одинаковых констант. По уму надо найти и
        уничтожить все причины такого размножения.}
      ConstId := GetStrAttr(NL[i], attrId, '-1');
      if (ConstId <> '-1') and (IdList.IndexOf(ConstId) > -1) then
        continue;

      Constant := Append;
      Constant.IsSheetConst := true;
      if not Constant.ReadFromXML(NL[i]) then
        Delete(Count - 1);
      IdList.Add(ConstId);
    end;
  end;

  UpdateTaskConsts;
  FConstsText := '';

  // загружаем константы задач
  if not Assigned(Owner.TaskContext) then
    exit;
  TaskConsts := Owner.TaskContext.GetTaskConsts;
  for i := 0 to TaskConsts.Count - 1 do
  begin
    TaskConst := TaskConsts[i];
    if (ConstByName(TaskConst.Name) <> nil) then
      continue;
    Constant := Append;
    Constant.IsSheetConst := false;
    Constant.ID := TaskConst.ID;
    Constant.Name := TaskConst.Name;
    Constant.Comment := TaskConst.Comment;
    Constant.IsInherited := TaskConst.IsInherited;
    if not VarIsEmpty(TaskConst.values) and not VarIsNull(TaskConst.values) then
      Constant.Value := TaskConst.values;
  end;
  FreeStringList(IdList);
end;

function TConstCollection.Refresh(Force: boolean): boolean;
begin
  result := true;
end;

procedure TConstCollection.SetItem(Index: integer; Item: TConstInterface);
begin
  Put(Index, Item);
end;

function TConstCollection.Validate: boolean;
begin
  result := true;
end;

procedure TConstCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  FConstsText := '';
  SetAttr(Node, 'counter', FCounter);
  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'const', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;

  UpdateTaskConsts;
  FConstsText := '';
end;

function TConstCollection.FindByID(ID: string): integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Count - 1 do
    if (Items[i].UniqueID = StrToInt(ID)) then
    begin
      result := i;
      break;
    end;
end;

function TConstCollection.ConstByName(Name: string): TConstInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
  begin
    if (Items[i].Name = Name) then
    begin
      result := Items[i];
      exit;
    end
  end;
end;

function TConstCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

function TConstCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

procedure TConstCollection.UpdateTaskConsts;
var
  TaskId, OldId, NewId: integer;
  UpdateResult, OldPart, NewPart: string;
  Constant: TConstInterface;
begin
  if FConstsText = '' then
    exit;
  if not Assigned(Owner.TaskContext) then
    exit;
  try
    TaskId := StrToInt(Owner.Environment.TaskId);
  except
    TaskId := -1;
  end;

  UpdateResult := Owner.DataProvider.UpdateTaskConsts(TaskId, FConstsText, ParamsDivider, ValuesDivider);
  if Pos('Exception', UpdateResult) > 0 then
  begin
    exit;
  end;

  while UpdateResult <> '' do
  begin
    // получаем пару вида XX=YY, первое число - старый айди параметра, второе - его новое значение
    NewPart := CutPart(UpdateResult, ParamsDivider);
    OldPart := CutPart(NewPart, '=');
    if (NewPart = '') or (OldPart = '') then
      continue; //не должно такого быть, ошибка

    try
      OldId := StrToInt(OldPart);
      NewId := StrToInt(NewPart);
      Constant := ConstById(OldId);
      if Assigned(Constant) then
        Constant.Id := NewId;
    except
      continue;
    end;

  end;
end;

{ TConst }

function TConst.IsSheetElement: boolean;
var
  i: integer;
begin
  result := true;
  with SheetInterface do
  begin
    for i := 0 to Totals.Count - 1 do
      if (Totals[i].TotalType = wtConst) and (Totals[i].Caption = Name) then
        exit;
    for i := 0 to SingleCells.Count - 1 do
      if (SingleCells[i].TotalType = wtConst) and (SingleCells[i].Name = Name) then
        exit;
  end;
  result := false;
end;

function TConst.ReadFromXML(Node: IXMLDOMNode): boolean;
var
  TaskConst: TTaskConst;
  OldName, HistoryString: string;
  TaskId: integer;
begin
  result := true;
  // первоначальные значения вытаскиваем из кастом пропертис
  Id := GetIntAttr(Node, 'id', -1);
  if SheetInterface.IsTaskConnectionLoad then
    ID := -1;
  Name := GetStrAttr(Node, 'name', '');
  Comment := GetStrAttr(Node, 'comment', '');
  Value := GetStrAttr(Node, 'value', '');
  IsSheetConst := GetBoolAttr(Node, 'issheetconst', false);

  if Assigned(SheetInterface.TaskContext) then
  begin
    IsInherited := boolean(GetIntAttr(Node, 'isInherited', 0));
    try
      TaskId := StrToInt(SheetInterface.Environment.TaskId);
    except
      TaskId := -1;
    end;
  end
  else
  begin
    // Если нет контеста задач, то сбрасываем флаг наследования
    IsInherited := false;
    exit; //дальше делать нечего
  end;

  case GetPresenceInTask(TaskConst) of
    ppSameId: 
      begin
        // записываем значения константы задачи в лист
        OldName := Name;
        Name := TaskConst.Name;
        Comment := TaskConst.Comment;
        if not VarIsEmpty(TaskConst.values) and not VarIsNull(TaskConst.values) then
          Value := TaskConst.Values;
        IsInherited := TaskConst.IsInherited;
        // изменяем атрибуты элементов листа - констант
        SyncSheetConsts(OldName, TaskConst.Name);
        exit;
      end;

    ppMustBeKilled:
      begin
        result := false;
        HistoryString := 'удалена константа "' + Name + '"';
        SheetInterface.AddEventInSheetHistory(evtConstsEdit, ConvertStringToCommaText(HistoryString), true);
        exit;
      end;

    ppNew:
      begin
        if SheetInterface.TaskContext.GetTaskConsts.IsReadOnly then
          exit;
        if TaskId <> -1 then
          PrepareTaskConstUpdate;
        exit;
      end;

    ppSameNameAndDimension:
      begin
        // константа с таким именем есть
        // записываем значения константы задачи в лист
        Name := TaskConst.Name;
        ID := TaskConst.ID;
        Comment := TaskConst.Comment;
        if not VarIsEmpty(TaskConst.values) and not VarIsNull(TaskConst.values) then
          Value := TaskConst.Values;
        IsInherited := TaskConst.IsInherited;
      end;

  end;

end;

procedure TConst.WriteToXML(Node: IXMLDOMNode);
var
  TaskId: integer;
  TaskConst: TTaskConst;
begin
  if not Assigned(Node) then
    exit;
  with (Node as IXMLDOMElement) do
  begin
    if (SheetInterface.TaskContext = nil) then
      ID := -1 ;
    setAttribute('id', ID);
    setAttribute('name', Name);
    setAttribute('comment', Comment);
    setAttribute('value', Value);
    setAttribute('isInherited', IntToSTr(IIF(IsInherited, 1, 0)));
    setAttribute('uniqueID', IntToStr(UniqueId));
    setAttribute('issheetconst', BoolToStr(IsSheetConst));
  end;

  if not Assigned(SheetInterface.TaskContext) then
    exit;
  try
    TaskId := StrToInt(SheetInterface.Environment.TaskId);
  except
    TaskId := -1;
  end;

  case GetPresenceInTask(TaskConst) of

    ppSameId:
      begin
        if (TaskId <> -1) and not IsInherited then
          PrepareTaskConstUpdate;
        exit;
      end;

    ppNew:
      begin  // параметр новый - добавляем в контекст задач
        if SheetInterface.TaskContext.GetTaskParams.IsReadOnly then
          exit;
        if TaskId <> -1 then
          PrepareTaskConstUpdate;
        exit;
      end;

    ppSameNameAndDimension:
      begin  // аналогично ppSameId
        if (TaskId <> -1) and not IsInherited then
          PrepareTaskConstUpdate;
        exit;
      end;

  end;
end;

// синхронизация атрибутов элементов листа - констант
procedure TConst.SyncSheetConsts(OldName, NewName: string);
var
  i, SheetIndex: integer;
  Book: ExcelWorkbook;
  ESheet: ExcelWorkSheet;
  Dom: IXMLDOMDocument2;
  NL: IXMLDOMNodeList;
  SubstName: string;
begin
  if not Assigned(SheetInterface.ExcelSheet) then
    exit;
  Book := ExcelWorkbook(SheetInterface.ExcelSheet.Parent);
  for SheetIndex := 1 to Book.Sheets.Count do
  begin
    ESheet := GetWorkSheet(Book.Sheets[SheetIndex]);
    if not Assigned(ESheet) then
      continue;
    if not IsPlaningSheet(ESheet) then
      continue;
    if ESheet.Name = SheetInterface.ExcelSheet.Name then
      continue;

    {!!Нарушение!! Вместо загрузки модели напрямую лезем в xml. Но так проще.
      По-возможности переделать.}
    Dom := GetDataFromCP(ESheet, cpMDName);

    {показатели}
    NL := Dom.selectNodes(Format('metadata/totals/total[@totaltype="%d"]', [Ord(wtConst)]));
    for i := 0 to NL.length - 1 do
    begin
      SubstName := GetStrAttr(NL[i], attrCaption, '');
      if SubstName = OldName then
        SetNodeStrAttr(NL[i], attrCaption, NewName);
    end;

    {отдельные}
    NL := Dom.selectNodes(Format('metadata/singlecells/singlecell[@totaltype="%d"]', [Ord(wtConst)]));
    for i := 0 to NL.length - 1 do
    begin
      SubstName := GetStrAttr(NL[i], attrName, '');
      if SubstName = OldName then
        SetAttr(NL[i], attrName, NewName);
    end;
    PutDataInCP(ESheet, cpMDName, Dom);
  end;

  {обработка для текущего листа - оперируем моделью}
  with SheetInterface do
  begin
    for i := 0 to Totals.Count - 1 do
    begin
      if (Totals[i].TotalType <> wtConst) then
        continue;
      if (Totals[i].Caption = OldName) then
        Totals[i].Caption := NewName;
    end;
    for i := 0 to SingleCells.Count - 1 do
    begin
      if (SingleCells[i].TotalType <> wtConst) then
        continue;
      if (SingleCells[i].Name = OldName) then
        SingleCells[i].Name := NewName;
    end;
  end;
end;

function TConst.GetPresenceInTask(out TaskConst: TTaskConst): TParamPresence;
begin
  TaskConst := nil;
  {Без контекста ничего не выясним}
  if not Assigned(SheetInterface.TaskContext) then
  begin
    result := ppNoContext;
    exit;
  end;

  {сперва ищем по имени, имя - главный атрибут}
  TaskConst := SheetInterface.TaskContext.GetTaskConsts.ConstByName(Name);
  if Assigned(TaskConst) then
  begin
    result := ppSameNameAndDimension;
    exit;
  end;

  {нет такого имени - глянем по айди, быть может константу переименовали в листе}
  TaskConst := SheetInterface.TaskContext.GetTaskConsts.ConstByID(Id);
  if Assigned(TaskConst) then
  begin
    result := ppSameId;
    exit;
  end;

  {Константы с таким айди нет и при этом она не локальная в листе. Видимо
    константа была удалена в задаче.}
  if not SheetInterface.SpecialFlagForTaskParamCopy then
    if Id > -1 then
    begin
      result := ppMustBeKilled;
      exit;
    end;

  result := ppNew;
end;

procedure TConst.PrepareTaskConstUpdate;
var
  ConstsText: string;
begin
  ConstsText := (Owner as TConstCollection).FConstsText;
  AddTail(ConstsText, ParamsDivider);
  ConstsText := ConstsText + IntToStr(Id);

  AddTail(ConstsText, ValuesDivider);
  ConstsText := ConstsText + Name;

  AddTail(ConstsText, ValuesDivider);
  ConstsText := ConstsText + Value;

  AddTail(ConstsText, ValuesDivider);
  ConstsText := ConstsText + Comment;

  (Owner as TConstCollection).FConstsText := ConstsText;
end;

end.

