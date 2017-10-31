unit uTaskContextProvider;

interface

uses
  uSheetObjectModel, uTaskParams, uGlobalPlaningConst,
    uOfficeUtils, ExcelXP, MSXML2_TLB, Classes, SysUtils, uXmlUtils,
    uFMAddinGeneralUtils, uFMAddinRegistryUtils, uFMAddinXMLUtils,
    ZLibEx;

type

  TTaskContextProvider = class
  private
    FContextList: TStringList;
    FEnvironmentList: TStringList;

    function GetTaskContext(Book: ExcelWorkbook): TTaskContext;
    //procedure SetTaskContext(Book: ExcelWorkBook; ContextXml: widestring; IsPacked: boolean);
    function GetTaskEnvironment(Book: ExcelWorkbook): TTaskEnvironment;

    function GetContextByIndex(Index: integer): TTaskContext;
    function GetEnvironmentByIndex(Index: integer): TTaskEnvironment;

    {Получает хмл с контекстом задачи из свойств книги}
    function ReadTaskContextXmlFromProperties(Book: ExcelWorkbook): widestring;
    {Загружает указанный контекст из хмл}
    function ReadTaskContext(ContextXml: string; var Context: TTaskContext): boolean;
    function CheckContextException(ContextDom: IXMLDOMDocument2): boolean;

    function UnPack(Source: string): Ansistring;
  public
    constructor Create;
    destructor Destroy; override;
    procedure OnWorkbookOpen(Book: ExcelWorkbook);
    procedure OnWorkbookClose(Book: ExcelWorkbook);
    procedure UpdateContext(Book: ExcelWorkbook; ContextXml: string);

    property TaskContext[Book: ExcelWorkbook]: TTaskContext read GetTaskContext;
    property TaskEnvironment[Book: ExcelWorkbook]: TTaskEnvironment read GetTaskEnvironment;
  end;

implementation

{ TTaskContextProvider }

function TTaskContextProvider.CheckContextException(ContextDom: IXMLDOMDocument2): boolean;
begin
  result := false;
  if not Assigned(ContextDom.documentElement) then
    exit;
  if ContextDom.documentElement.nodeName = 'Exception' then
  begin
    exit;
  end;
  result := true;
end;

constructor TTaskContextProvider.Create;
begin
  FContextList := TStringList.Create;
  FEnvironmentList := TStringList.Create;
end;

destructor TTaskContextProvider.Destroy;
begin
  inherited;
  FreeStringList(FContextList);
  FreeStringList(FEnvironmentList);
end;

function TTaskContextProvider.GetContextByIndex(Index: integer): TTaskContext;
begin
  result := nil;
  try
    result := TTaskContext(FContextList.Objects[Index]);
  except
  end;
end;

function TTaskContextProvider.GetEnvironmentByIndex(Index: integer): TTaskEnvironment;
begin
  result := TTaskEnvironment(FEnvironmentList.Objects[Index]);
end;

function DecodeBase64(const CinLine: widestring): widestring;
const
  RESULT_ERROR = -2;
var
  inLineIndex: Integer;
  c: WideChar;
  x: SmallInt;
  c4: Word;
  StoredC4: array[0..3] of SmallInt;
  InLineLength: Integer;
begin
  Result := '';
  inLineIndex := 1;
  c4 := 0;
  InLineLength := Length(CinLine);

  while inLineIndex <= InLineLength do
  begin
    while (inLineIndex <= InLineLength) and (c4 < 4) do
    begin
      c := CinLine[inLineIndex];
      case c of
        '+'     : x := 62;
        '/'     : x := 63;
        '0'..'9': x := Ord(c) - (Ord('0')-52);
        '='     : x := -1;
        'A'..'Z': x := Ord(c) - Ord('A');
        'a'..'z': x := Ord(c) - (Ord('a')-26);
      else
        x := RESULT_ERROR;
      end;
      if x <> RESULT_ERROR then
      begin
        StoredC4[c4] := x;
        Inc(c4);
      end;
      Inc(inLineIndex);
    end;

    if c4 = 4 then
    begin
      c4 := 0;
      Result := Result + Char((StoredC4[0] shl 2) or (StoredC4[1] shr 4));
      if StoredC4[2] = -1 then Exit;
      Result := Result + Char((StoredC4[1] shl 4) or (StoredC4[2] shr 2));
      if StoredC4[3] = -1 then Exit;
      Result := Result + Char((StoredC4[2] shl 6) or (StoredC4[3]));
    end;
  end;
end;

function TTaskContextProvider.GetTaskContext(Book: ExcelWorkbook): TTaskContext;
var
  Index: integer;
  BookName: string;
  Environment: TTaskEnvironment;
  //Ok: boolean;
  ContextXml: widestring;
begin
  result := nil;
  Environment := GetTaskEnvironment(Book);
  if not Environment.IsLoadingFromTask then
    exit;

  {Если такой контекст уже есть - возвращаем его}
  BookName := Book.Name;
  Index := FContextList.IndexOf(BookName);
  if Index > -1 then
  begin
    result := GetContextByIndex(Index);
    exit;
  end;

  {Иначе - создаем и пробуем загрузить}
  result := TTaskContextLocal.Create;
  try
    if Environment.ContextType then
    begin {Чтение из альтернативного потока - не реализовано, не ясно понадобится ли}
    end
    else
    begin {Чтение из свойств документа}
      ContextXml := ReadTaskContextXmlFromProperties(Book);
    end;
    {Ok := }ReadTaskContext(ContextXml, result);
  finally
    FContextList.AddObject(BookName, result);
  end;
end;

function TTaskContextProvider.GetTaskEnvironment(Book: ExcelWorkbook): TTaskEnvironment;
var
  Index: integer;
  BookName: string;
begin
  BookName := Book.Name;
  Index := FEnvironmentList.IndexOf(BookName);
  if Index > -1 then
  begin
    result := GetEnvironmentByIndex(Index);
    exit;
  end;

  result := TTaskEnvironment.Create;
  result.ReadTaskProperties(Book);
  FEnvironmentList.AddObject(BookName, result);
end;

procedure TTaskContextProvider.OnWorkbookClose(Book: ExcelWorkbook);
var
  Index: integer;
  Environment: TTaskEnvironment;
  Context: TTaskContext;
begin
  Index := FEnvironmentList.IndexOf(Book.Name);
  if Index > -1 then
  begin
    Environment := GetEnvironmentByIndex(Index);
    FEnvironmentList.Objects[Index] := nil;
    FreeAndNil(Environment);
    FEnvironmentList.Delete(Index);
  end;

  Index := FContextList.IndexOf(Book.Name);
  if Index > -1 then
  begin
    Context := GetContextByIndex(Index);
    FContextList.Objects[Index] := nil;
    FreeAndNil(Context);
    FContextList.Delete(Index);
  end;
end;

procedure TTaskContextProvider.OnWorkbookOpen(Book: ExcelWorkbook);
begin
  GetTaskEnvironment(Book);
  GetTaskContext(Book);
end;

function TTaskContextProvider.ReadTaskContext(ContextXml: string;
  var Context: TTaskContext): boolean;
var
  ContextDom: IXMLDOMDocument2;
begin
  result := false;
  if not Assigned(Context) then
    exit;

  try
    {Создаем и загружаем хмл-документ}
    if ContextXml <> '' then
    begin
      GetDomDocument(ContextDom);
      ContextDom.loadXML(ContextXml);
      result := CheckContextException(ContextDom);
    end;
  finally
    {Если документ прочитан нормально, то загружаем из него контекст}
    if result then
    begin
      Context.ReadFromXml(ContextDom);
      if AddinLogEnable then
        WriteDocumentLog(ContextDom, 'TaskContext.xml');
    end
    else
      Context.Clear;
      //FreeAndNil(Context);
    KillDOMDocument(ContextDom);
  end;
end;

function TTaskContextProvider.ReadTaskContextXmlFromProperties(Book: ExcelWorkbook): widestring;
var
  i, LastI: integer;
  PropName: string;
  Props: IDispatch;
  PropValue: widestring;
begin
  result := '';
  Props := Book.CustomDocumentProperties;
  LastI := -1;
  for i := 0 to 99999 do
  begin
    PropName := Format('%s.%d',  [fmtcData, i]);
    PropValue := GetWBCustomPropertyValue2(Props, PropName);
    if PropValue = '' then
    begin
      LastI := i - 1;
      break;
    end;
    result := result + PropValue;
  end;
  result := UnPack(result);

  for i := LastI downto 0 do
  try
    PropName := Format('%s.%d',  [fmtcData, i]);
    DeleteWBCustomProperty(Props, PropName);
  except
    // глушим все исключения, чтобы случись что не обломать весь процесс
  end;
end;

function TTaskContextProvider.UnPack(Source: string): AnsiString;
begin
  Source := DecodeBase64(Source);
  try
    ZDecompressString(result, Source); 
  except
    on e: exception do
      begin
        ShowError(e.Message);
      end;
  end;
end;

procedure TTaskContextProvider.UpdateContext(Book: ExcelWorkbook; ContextXml: string);
var
  Context: TTaskContext;
begin
  Context := GetTaskContext(Book);
  ReadTaskContext(ContextXml, Context);
end;

end.

