unit uTaskParams;

interface

uses Classes, SysUtils, uSheetObjectModel, uXMLUtils, uFMAddinXMLUtils, MSXML2_TLB;

type

  TTaskParamLocal = class(TTaskParam)
  private
    FOwner: TTaskParamsCollection;
  public
    constructor Create(AOwner: TTaskParamsCollection); override;
    procedure ReadFromXml(Node: IXMLDOMNode); override;
  end;

  TTaskConstLocal = class(TTaskConst)
  private
    Fowner: TTaskConstsCollection;
  public
    constructor Create(AOwner: TTaskConstsCollection); override;
    procedure ReadFromXml(Node: IXMLDOMNode); override;
  end;

  {Коллекция параметров "от задачи"}
  TTaskParamsCollectionLocal = class(TTaskParamsCollection)
  private
    FIsReadOnly: boolean;
  protected
    function GetItem(Index: integer): TTaskParam; override;
    procedure SetItem(Index: integer; Item: TTaskParam); override;
    function Append: TTaskParam; override;
    function  GetIsReadOnly: boolean; override;
  public
    procedure ReadFromXml(Node: IXMLDOMNode); override;
    procedure Delete(Index: integer); virtual;

    function  ParamByID(ID: integer): TTaskParam; override;
    function  ParamByName(Name: string): TTaskParam; override;

    property IsReadOnly: boolean read FIsReadOnly;
  end;


  {Коллекция констант "от задачи"}
  TTaskConstsCollectionLocal = class(TTaskConstsCollection)
  private
    FIsReadOnly: boolean;
  protected
    function GetItem(Index: integer): TTaskConst; override;
    procedure SetItem(Index: integer; Item: TTaskConst); override;
    function Append: TTaskConst; override;
    function  GetIsReadOnly: boolean; override;
  public
    procedure ReadFromXml(Node: IXMLDOMNode); override;
    procedure Delete(Index: integer); virtual;

    function  ConstByID(ID: integer): TTaskConst; override;
    function  ConstByName(Name: string): TTaskConst; override;

    property IsReadOnly: boolean read FIsReadOnly;
  end;

  TTaskContextLocal = class(TTaskContext)
  private
    FTaskParams: TTaskParamsCollectionLocal;
    FTaskConsts: TTaskConstsCollectionLocal;

  protected
    function GetParamCount: integer; override;
    function GetConstCount: integer; override;
  public
    constructor Create; override;
    destructor Destroy; override;
    procedure ReadFromXml(Dom: IXMLDOMDocument2); override;
    procedure Clear; override;
    function GetTaskParams: TTaskParamsCollection; override;
    function GetTaskConsts: TTaskConstsCollection; override;
  end;


implementation

{ TTaskContextLocal }

procedure TTaskContextLocal.Clear;
begin
  FTaskParams.Clear;
  FTaskConsts.Clear;
end;

constructor TTaskContextLocal.Create;
begin
  FTaskParams := TTaskParamsCollectionLocal.Create;
  FTaskConsts := TTaskConstsCollectionLocal.Create;
end;

destructor TTaskContextLocal.Destroy;
begin
  FTaskParams.Free;
  FTaskConsts.Free;
  inherited;
end;

function TTaskContextLocal.GetConstCount: integer;
begin
  result := -1;
  if Assigned(FTaskConsts) then
    result := FTaskConsts.Count;
end;

function TTaskContextLocal.GetParamCount: integer;
begin
  result := -1;
  if Assigned(FTaskParams) then
    result := FTaskParams.Count;
end;

function TTaskContextLocal.GetTaskConsts: TTaskConstsCollection;
begin
  result := FTaskConsts;
end;

function TTaskContextLocal.GetTaskParams: TTaskParamsCollection;
begin
  result := FTaskParams;
end;

procedure TTaskContextLocal.ReadFromXml(Dom: IXMLDOMDocument2);
var
  Node: IXMLDOMNode;
begin
  FTaskParams.Clear;
  FTaskConsts.Clear;

  if not Assigned(Dom) then
    exit;

  Node := Dom.selectSingleNode('TaskContext');
  FTaskParams.ReadFromXml(Node);
  FTaskConsts.ReadFromXml(Node);
end;

{ TTaskParamsCollectionLocal }

function TTaskParamsCollectionLocal.Append: TTaskParam;
begin
  result := TTaskParamLocal.Create(Self);
  inherited Add(result);
end;

procedure TTaskParamsCollectionLocal.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TTaskParamsCollectionLocal.GetIsReadOnly: boolean;
begin
  result := FIsReadOnly;
end;

function TTaskParamsCollectionLocal.GetItem(Index: integer): TTaskParam;
begin
  result := Get(Index);
end;

function TTaskParamsCollectionLocal.ParamByID(ID: integer): TTaskParam;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Id = Id then
    begin
      result := Items[i];
      exit;
    end;
end;

function TTaskParamsCollectionLocal.ParamByName(Name: string): TTaskParam;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if AnsiUpperCase(Items[i].Name) = AnsiUpperCase(Name) then
    begin
      result := Items[i];
      exit;
    end;
end;

procedure TTaskParamsCollectionLocal.ReadFromXml(Node: IXMLDOMNode);
var
  i: integer;
  NL: IXMLDOMNodeList;
  TaskParam: TTaskParam;
begin
  Clear;
  if not Assigned(Node) then
    exit;
  FIsReadOnly := GetBoolAttr(Node, 'isreadonly', false);
  NL := Node.selectNodes('Parameter');
  for i := 0 to NL.length - 1 do
  begin
    TaskParam := Append;
    TaskParam.ReadFromXml(NL[i]);
  end;
end;

procedure TTaskParamsCollectionLocal.SetItem(Index: integer; Item: TTaskParam);
begin
  Put(Index, Item);
end;

{ TTaskConstLocal }

constructor TTaskConstLocal.Create(AOwner: TTaskConstsCollection);
begin
  FOwner := AOwner;
end;

procedure TTaskConstLocal.ReadFromXml(Node: IXMLDOMNode);
begin
  inherited;
end;

{ TTaskParamLocal }

constructor TTaskParamLocal.Create(AOwner: TTaskParamsCollection);
begin
  FOwner := AOwner;
end;

procedure TTaskParamLocal.ReadFromXml(Node: IXMLDOMNode);
var
  str: string;
begin
  inherited ReadFromXml(Node);
  Dimension := ReadTaskContextAttr(Node, 'DIMENSION', 'Dimension');
  str := ReadTaskContextAttr(Node, 'ALLOWMULTISELECT', 'AllowMultiSelect');
  try
    AllowMultiSelect := boolean(StrToInt(str));
  except
    AllowMultiSelect := false;
  end;
end;

{ TTaskConstsCollectionLocal }

function TTaskConstsCollectionLocal.Append: TTaskConst;
begin
  result := TTaskConstLocal.Create(Self);
  inherited Add(result);
end;

function TTaskConstsCollectionLocal.ConstByID(ID: integer): TTaskConst;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Id = Id then
    begin
      result := Items[i];
      exit;
    end;
end;


function TTaskConstsCollectionLocal.ConstByName(Name: string): TTaskConst;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if AnsiUpperCase(Items[i].Name) = AnsiUpperCase(Name) then
    begin
      result := Items[i];
      exit;
    end;
end;

procedure TTaskConstsCollectionLocal.Delete(Index: integer);
begin
  Items[Index].Free;
  inherited Delete(Index);
end;

function TTaskConstsCollectionLocal.GetIsReadOnly: boolean;
begin
  result := FIsReadOnly;
end;

function TTaskConstsCollectionLocal.GetItem(Index: integer): TTaskConst;
begin
  result := Get(Index);
end;

procedure TTaskConstsCollectionLocal.ReadFromXml(Node: IXMLDOMNode);
var
  NL: IXMLDOMNodeList;
  i: integer;
  TaskConst: TTaskConst;
begin
  Clear;
  if not Assigned(Node) then
    exit;
  FIsReadOnly := GetBoolAttr(Node, 'isreadonly', false);
  NL := Node.selectNodes('Constant');
  for i := 0 to NL.length - 1 do
  begin
    TaskConst := Append;
    TaskConst.ReadFromXml(NL[i]);
  end;
end;

procedure TTaskConstsCollectionLocal.SetItem(Index: integer; Item: TTaskConst);
begin
  Put(Index, Item);
end;

end.

