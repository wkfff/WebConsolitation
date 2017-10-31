unit uSheetParam;

interface

uses
  Messages, SysUtils, Classes, Controls, Dialogs, uSheetObjectModel, MSXML2_TLB,
  uSheetHistory, uFMAddinGeneralUtils, uXMLUtils, uFMExcelAddInConst, uExcelUtils,
  uXmlCatalog, uFMAddinXmlUtils;

type

  TParam = class(TParamInterface)
  private
    FDimension: string;

    procedure PrepareTaskParamUpdate;
    function RefreshBeforeTaskConnection: boolean;
  protected
    function GetSheetDimension: TSheetDimension; override;
    function GetDimension: string; override;
  public
    procedure WriteToXML(Node: IXMLDOMNode); override;
    function ReadFromXML(Node: IXMLDOMNode): boolean; override;
    function GetPresenceInTask(out TaskParam: TTaskParam): TParamPresence; override;
    function GetModifiedName: string; override;
    procedure SetLink(SheetDimension: TSheetDimension); override;
    procedure RemoveLink(SheetDimension: TSheetDimension); override;
    procedure Delete; override;
  end;

  TParamCollection = class(TParamCollectionInterface)
  private
    FParamsText: string;
    procedure UpdateTaskParams;
  protected
    function GetItem(Index: integer): TParamInterface; override;
    procedure SetItem(Index: integer; Item: TParamInterface); override;
    function Append: TParaminterface; override;
    {заглушки}
    function GetStyleCaption(ElementStyle: TElementStyle): string; override;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; override;
  public
    constructor Create(AOwner: TSheetInterface);
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXml(Node: IXMLDOMNode); override;
    procedure Delete(Index: integer); override;
    function AddParam(SheetDimension: TSheetDimension): TParamInterface; override;
    function ParamByName(Name: string): TParamInterface; override;
    function ParamByPid(Pid: integer): TParamInterface; override;
    function ParamById(Id: integer): TParamInterface; override;
    function GetCollectionName: string; override;
    function Validate: boolean; override;
    function FindByID(ID: string): integer; override;
    function Refresh(Force: boolean): boolean; override;
  end;

implementation

{ TParamCollection }

constructor TParamCollection.Create(AOwner: TSheetInterface);
begin
  inherited Create(AOwner);
end;

function TParamCollection.GetItem(Index: integer): TParamInterface;
begin
  result := Get(Index);
end;

procedure TParamCollection.SetItem(Index: integer; Item: TParamInterface);
begin
  Put(Index, Item);
end;

function TParamCollection.Append: TParaminterface;
begin
  result := TParam.Create(Self);
  result.UniqueId := GetUniqueID;
  result.Owner := Self;
  inherited Add(result);
end;

procedure TParamCollection.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TParamCollection.AddParam(SheetDimension: TSheetDimension): TParamInterface;
begin
  result := Append;
  result.SetLink(SheetDimension);
end;

function TParamCollection.ParamByName(Name: string): TParamInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Name = Name then
    begin
      result := Items[i];
      exit;
    end;
end;

function TParamCollection.ParamByPid(Pid: integer): TParamInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].PID = Pid then
    begin
      result := Items[i];
      exit;
    end;
end;

function TParamCollection.ParamById(Id: integer): TParamInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].ID = Id then
    begin
      result := Items[i];
      exit;
    end;
end;

procedure TParamCollection.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  Param: TParamInterface;
begin
  FParamsText := '';
  if not Assigned(Node) then
    exit;
  NL := Node.selectNodes('param');
  for i := 0 to NL.length - 1 do
  begin
    Param := Append;
    if not Param.ReadFromXML(NL[i]) then
      Param.Delete;
  end;

  UpdateTaskParams;
  FParamsText := '';
end;

procedure TParamCollection.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
  ItemNode: IXMLDOMNode;
begin
  FParamsText := '';
  SetAttr(Node, 'counter', Counter);
  for i := 0 to Count - 1 do
  begin
    ItemNode := Node.ownerDocument.createNode(1, 'param', '');
    Items[i].WriteToXML(ItemNode);
    Node.appendChild(ItemNode);
  end;

  UpdateTaskParams;
  FParamsText := '';
end;



(*function TParamCollection.FindObjectByUniqueId(UniqueId: string): TSheetDimension;
var
  Index: integer;
  i: integer;
begin
  result := nil;
  Index := Owner.Rows.FindById(UniqueId);
  if (Index <> - 1) then
  begin
    result := Owner.Rows[Index];
    exit;
  end;
  Index := Owner.Columns.FindById(UniqueId);
  if (Index <> - 1) then
  begin
    result := Owner.Columns[Index];
    exit;
  end;
  Index := Owner.Filters.FindById(UniqueId);
  if (Index <> - 1) then
  begin
    result := Owner.Filters[Index];
    exit;
  end;
  for i := 0 to Owner.SingleCells.Count - 1 do
  begin
    Index := Owner.SingleCells[i].Filters.FindById(UniqueId);
    if (Index <> - 1) then
    begin
      result := Owner.SingleCells[i].Filters[Index];
      exit;
    end;
  end;
end;

function TParamCollection.GetItem(UniqueId: string): TParamInterface;
begin
  result := nil;
  FSheetDimension := FindObjectByUniqueId(UniqueId);
  if not Assigned(FSheetDimension) then
    exit;
  result := FSheetDimension.Param;
end;

function TParamCollection.GetUniqueId(Index: integer): string;
begin
  result := string(Get(Index));
end;

procedure TParamCollection.SetItem(UniqueId: string; Value: TParamInterface);
begin
  if not Assigned(Value) then
    exit;
  FSheetDimension := FindObjectByUniqueId(UniqueId);
  if not Assigned(FSheetDimension) then
    exit;
  FSheetDimension.Param := Value;
end;

procedure TParamCollection.SetUniqueId(Index: integer; Value: string);
begin
  Put(Index, Pointer(Value));
end;

function TParamCollection.ParamByName(Name: string): TParamInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
  begin
    FSheetDimension := FindObjectByUniqueId(UniqueIDs[i]);
    if FSheetDimension.Param.Name = Name then
    begin
      result := FSheetDimension.Param;
      exit;
    end
  end;
end;

function TParamCollection.AddParam(UniqueId: string): TParamInterface;
begin
  result := nil;
  FSheetDimension := FindObjectByUniqueId(UniqueId);
  if not Assigned(FSheetDimension) then
    exit;
  if FSheetDimension.IsParam then
    result := FSheetDimension.Param
  else
  begin
    result := TParam.Create;
    FSheetDimension.Param := result;
  end;  
  result.Owner := FSheetDimension;
  inherited Add(Pointer(UniqueId));
end;

function TParamCollection.AddParam(SheetDimension: TSheetDimension): TParamInterface;
begin
  result := TParam.Create;
  SheetDimension.Param := result;
  result.Owner := SheetDimension;
  inherited Add(Pointer(SheetDimension.UniqueId));
end;

procedure TParamCollection.DeleteParam(UniqueId: string);
var
  Index: integer;
begin
  FSheetDimension := FindObjectByUniqueId(UniqueId);
  if not Assigned(FSheetDimension) then
    exit;
  FSheetDimension.Param := nil;
  Index := IndexByUniqueId(UniqueId);
  if (Index <> - 1) then
    inherited Delete(Index);
end;

function TParamCollection.IndexByUniqueId(UniqueId: string): integer;
var
  i: integer;
begin
  result := - 1;
  for i := 0 to Count - 1 do
    if UniqueIds[i] = UniqueId then
    begin
      result := i;
      exit;
    end;
end;

function TParamCollection.ParamByIndex(Index: integer): TParamInterface;
begin
  result := Items[UniqueIDs[Index]];
end;  *)

function TParamCollection.GetDefaultStyleName(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

function TParamCollection.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := '';
end;

function TParamCollection.GetCollectionName: string;
begin
  result := 'params';
end;

function TParamCollection.Validate: boolean;
begin
  result := true;
end;

function TParamCollection.FindByID(ID: string): integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Count - 1 do
    if IntToStr(Items[i].UniqueId) = ID then
    begin
      result := i;
      exit;
    end;
end;

function TParamCollection.Refresh(Force: boolean): boolean;
begin
  result := true;
end;

procedure TParamCollection.UpdateTaskParams;
var
  TaskId, OldId, NewId: integer;
  UpdateResult, OldPart, NewPart: string;
  Param: TParamInterface;
begin
  if FParamsText = '' then
    exit;
  if not Assigned(Owner.TaskContext) then
    exit;
  try
    TaskId := StrToInt(Owner.Environment.TaskId);
  except
    TaskId := -1;
  end;

  UpdateResult := Owner.DataProvider.UpdateTaskParams(TaskId, FParamsText, ParamsDivider, ValuesDivider);
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
      Param := ParamById(OldId);
      if Assigned(Param) then
        Param.Id := NewId;
    except
      continue;
    end;

  end;
end;

{ TParam }

function TParam.GetModifiedName: string;
var
  TaskContext: TTaskContext;
  i: integer;
begin
  TaskContext := SheetInterface.TaskContext;
  if not Assigned(TaskContext) then
    exit;
  i := 0;
  repeat
    inc(i);
    result := Name + ' (' + IntToStr(i) + ')';
  until not Assigned(TaskContext.GetTaskParams.ParamByName(result));
end;

function CheckDimensionName(Local, FromTask: string): boolean;
begin
  result := (Local = FromTask) or
    (Local = Copy(FromTask, 1, Pos('.', FromTask) - 1));
end;

function TParam.GetPresenceInTask(out TaskParam: TTaskParam): TParamPresence;
var
  TaskContext: TTaskContext;
begin
  TaskParam := nil;
  TaskContext := SheetInterface.TaskContext;
  {Без контекста ничего не выясним}
  if not Assigned(TaskContext) then
  begin
    result := ppNoContext;
    exit;
  end;

  {Сперва ищем по имени. Имя параметра - главный критерий. Если параметр с таким именем есть, то
    измерение - второй фактор. Имя и измерение совпадают - параметр тот же что в листе.
    Если измерения не совпадают - нужна разыменовка.}
  TaskParam := TaskContext.GetTaskParams.ParamByName(Name);
  if Assigned(TaskParam) then
  begin
    if CheckDimensionName(Dimension, TaskParam.Dimension) then
      result := ppSameNameAndDimension
    else
      result := ppMustBeRenamed;
    exit;
  end;

  {Напоследок глянем по айди - может параметр переименовали в листе.
    Если измерения совпадают - параметр наш.}
  TaskParam := TaskContext.GetTaskParams.ParamByID(Id);
  if Assigned(TaskParam) then
    if CheckDimensionName(Dimension, TaskParam.Dimension) then
    begin
      result := ppSameId;
      exit;
    end;

  if not SheetInterface.SpecialFlagForTaskParamCopy then
    if Id > -1 then
    begin
      result := ppMustBeKilled;
      exit;
    end;

  result := ppNew;
end;

function TParam.ReadFromXML(Node: IXMLDOMNode): boolean;
var
  TaskContext: TTaskContext;
  TaskParam: TTaskParam;
  HistoryString, tmpString: string;
  TaskId: integer;
begin
  result := true;
  TaskContext := SheetInterface.TaskContext;
  // первоначальные значения вытаскиваем из кастом пропертис
  ID := GetIntAttr(Node, 'id', -1);
  PID := GetIntAttr(Node, 'pid', -1);

  // !!!
  if SheetInterface.IsTaskConnectionLoad then
    ID := -PID;

  Name := GetStrAttr(Node, 'name', '');
  Comment := GetStrAttr(Node, 'comment', '');
  MultiSelect := boolean(GetIntAttr(Node, 'multiSelect', 0));
  FDimension := GetStrAttr(Node, 'dimension', '');
  tmpString := GetStrAttr(Node, 'links', '');
  Links.CommaText := tmpString;

  if not Assigned(FMembers) then
    GetDomDocument(FMembers);
  FMembers := SheetInterface.GetData(CPName);

  if Assigned(TaskContext) then
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
    {Если нет контекста задач, то сбрасываем флаг наследования параметров}
    IsInherited := false;
    Id := -1;
    exit;
  end; //дальше делать нечего

  {Синхронизация из задачи в лист. На каждой загрузке проверяем параметр в задаче}
  case GetPresenceInTask(TaskParam) of

    ppSameId:  
      begin  // записываем значения параметра задачи
        Name := TaskParam.Name;
        Comment := TaskParam.Comment;
        MultiSelect := TaskParam.AllowMultiSelect;
        IsInherited := TaskParam.IsInherited;
        SetMembers(TaskParam.Values);
        exit;
      end;

    ppMustBeKilled:
      begin  //  параметр замочили в задаче - мочим его и тут
        result := false;
        HistoryString := 'удален параметр "' + FullName + '" (синхронизация с задачей)';
        SheetInterface.AddEventInSheetHistory(evtParamsEdit,
          ConvertStringToCommaText(HistoryString), true);
        exit;
      end;

    ppNew:
      begin  // параметр новый - добавляем в контекст задач
        if TaskContext.GetTaskParams.IsReadOnly then
          exit;
        if TaskId <> -1 then
        begin
          if SheetInterface.IsTaskConnectionLoad then
            RefreshBeforeTaskConnection;
          PrepareTaskParamUpdate;
        end;
        exit;
      end;

    ppSameNameAndDimension:
      begin  // записываем значения параметра задачи
        ID := TaskParam.ID;
        Name := TaskParam.Name;
        Comment := TaskParam.Comment;
        MultiSelect := TaskParam.AllowMultiSelect;
        IsInherited := TaskParam.IsInherited;
        SetMembers(TaskParam.Values);
        exit;
      end;

    ppMustBeRenamed:
      begin // модифицируем имя параметра и добавляем его в задачу как новый
        if TaskContext.GetTaskParams.IsReadOnly then
          exit;
        if TaskId <> -1 then
        begin
          Name := GetModifiedName;
          if SheetInterface.IsTaskConnectionLoad then
            RefreshBeforeTaskConnection;
          PrepareTaskParamUpdate;
        end;
      end;
  end;
end;

procedure TParam.WriteToXML(Node: IXMLDOMNode);
var
  tmpString: string;
  TaskId: integer;
  TaskParam: TTaskParam;
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    if SheetInterface.TaskContext = nil then
      ID := -1 ;
    setAttribute('id', ID);
    setAttribute('pid', PID);
    setAttribute('name', Name);
    setAttribute('comment', Comment);
    setAttribute('dimension', Dimension);
    setAttribute('multiSelect', IntToSTr(IIF(MultiSelect, 1, 0)));
    setAttribute('isInherited', IntToSTr(IIF(IsInherited, 1, 0)));
    tmpString := CommaTextToString(Links.CommaText);
    setAttribute('links', tmpString);
  end;
  SheetInterface.PutData(Members, CPName);

  {синхронизация из листа в задачу}
  if not Assigned(SheetInterface.TaskContext) then
    exit;
  try
    TaskId := StrToInt(SheetInterface.Environment.TaskId);
  except
    TaskId := -1;
  end;

  case GetPresenceInTask(TaskParam) of

    ppSameId:
      begin  // записываем значения параметра задачи в лист
        if (TaskId <> -1) and not IsInherited then
          PrepareTaskParamUpdate;
        exit;
      end;

    ppNew:
      begin  // параметр новый - добавляем в контекст задач
        if SheetInterface.TaskContext.GetTaskParams.IsReadOnly then
          exit;
        if TaskId <> -1 then
          PrepareTaskParamUpdate;
        exit;
      end;

    ppSameNameAndDimension:
      begin  // аналогично ppSameId
        if (TaskId <> -1) and not IsInherited then
          PrepareTaskParamUpdate;
        exit;
      end;

    ppMustBeRenamed:
      begin // модифицируем имя параметра и добавляем его в задачу как новый
        if SheetInterface.TaskContext.GetTaskParams.IsReadOnly then
          exit;
        if TaskId <> -1 then
        begin
          Name := GetModifiedName;
          PrepareTaskParamUpdate;
        end;
      end;
  end;
end;

procedure TParam.SetLink(SheetDimension: TSheetDimension);
begin
  // Если параметр свежесозданный, копируем в него элементы из измерения
  if Links.Count = 0 then
    if Assigned(SheetDimension.Members) then
      Members.load(SheetDimension.Members);

  Links.Add(SheetDimension.UniqueId);
  if FDimension = '' then
    FDimension := SheetDimension.FullDimensionName;
  SheetDimension.Param := Self;
end;

procedure TParam.RemoveLink(SheetDimension: TSheetDimension);
var
  Index: integer;
begin
  SheetDimension.Param := nil;
  Index := Links.IndexOf(SheetDimension.UniqueId);
  if Index > -1 then
  begin
    Links.Delete(Index);
    // Измерению, открепленному от параметра, отдаем и его элементы
    SheetDimension.Members := Members;
  end;
  if (Links.Count = 0) and SheetInterface.KillZeroLinkedParams then
    Delete;
end;

procedure TParam.Delete;
var
  Index: integer;
begin
  for Index := 0 to Owner.Count - 1 do
    if Owner[Index] = Self then
    begin
      Owner.Delete(Index);
      exit;
    end;
  if Assigned(FMembers) then
    KillDomDocument(FMembers);
end;

function TParam.GetSheetDimension: TSheetDimension;
var
  UID: string;
begin
  UID := Links[0]; // не может не быть, поэтому берем смело
  result := SheetInterface.FindDimensionByUniqueId(UID);
end;

function TParam.GetDimension: string;
begin
  result := FDimension;
end;

procedure TParam.PrepareTaskParamUpdate;
var
  ParamsText: string;
begin
  ParamsText := (Owner as TParamCollection).FParamsText;
  AddTail(ParamsText, ParamsDivider);
  ParamsText := ParamsText + IntToStr(Id);

  AddTail(ParamsText, ValuesDivider);
  ParamsText := ParamsText + Name;

  AddTail(ParamsText, ValuesDivider);
  ParamsText := ParamsText + Members.xml;

  AddTail(ParamsText, ValuesDivider);
  ParamsText := ParamsText + Dimension;

  AddTail(ParamsText, ValuesDivider);
  ParamsText := ParamsText + Comment;

  AddTail(ParamsText, ValuesDivider);
  ParamsText := ParamsText + BoolToStr(MultiSelect);

  (Owner as TParamCollection).FParamsText := ParamsText;
end;

function TParam.RefreshBeforeTaskConnection: boolean;
var
  OldMembers, NewMembers: IXMLDOMDocument2;
  DelimPos: integer;
  DimName, HierName, AllMemberProperties: string;
  Dim: TDimension;
  Hier: THierarchy;
begin
  result := false;
  if not SheetInterface.CheckConnection then
    exit;
  { Получим раздельные имя измерения и иерархии}
  DelimPos := Pos('.', Dimension);
  if DelimPos > 0 then
  begin
    DimName := Copy(Dimension, 1, DelimPos - 1);
    HierName := Copy(Dimension, DelimPos + 1, length(Dimension));
  end
  else
    DimName := Dimension;

  {}
  Dim := SheetInterface.XMLCatalog.Dimensions.Find(DimName, SheetInterface.XMLCatalog.PrimaryProvider);
  Hier := nil;
  AllMemberProperties := '';
  if Assigned(Dim) then
    Hier := Dim.GetHierarchy(HierName);
  if Assigned(Hier) then
    AllMemberProperties := Hier.MemberProperties.GetCommaList;

  try
    OldMembers := Members;

    if not Assigned(OldMembers) then
      exit;

    try
      begin
        NewMembers := SheetInterface.DataProvider.GetMemberList(
          SheetInterface.XMLCatalog.PrimaryProvider, '', DimName, HierName,
          '', AllMemberProperties);
        if (SheetInterface.DataProvider.LastWarning <> '') then
          SheetInterface.PostMessage(SheetInterface.DataProvider.LastWarning, msgWarning);
      end;

      if (SheetInterface.DataProvider.LastError <> '') then
      begin
        SheetInterface.PostMessage(SheetInterface.DataProvider.LastError, msgWarning);
        exit;
      end;
    except
      exit;
    end;

    if not Assigned(NewMembers) then
      exit;

    CopyMembersState(OldMembers, NewMembers, SheetInterface.SetPBarPosition);
    SetCheckedIndication(NewMembers);
    FilterMembersDomEx(NewMembers);
    CutAllInvisible(NewMembers, true);

    //подменяем обновленные данные
    setMembers(NewMembers);

    OldMembers := nil;
    result := true;
  finally
  end;
end;

end.

