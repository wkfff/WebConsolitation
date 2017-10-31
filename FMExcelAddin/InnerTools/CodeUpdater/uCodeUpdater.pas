unit uCodeUpdater;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, uFMAddinGeneralUtils, uExcelUtils, ExcelXP, uFMAddinExcelUtils,
  MSXML2_TLB, uGlobalPlaningConst, uFMAddinXMLUtils, uXMLUtils, FileCtrl, IniFiles;

type
  TfmCodeUpdater = class(TForm)
    edWorkPath: TEdit;
    Button1: TButton;
    Label1: TLabel;
    edDim: TEdit;
    Label2: TLabel;
    mList: TMemo;
    Label3: TLabel;
    btnGo: TButton;
    edHier: TEdit;
    Label4: TLabel;
    mLog: TMemo;
    edFullName: TEdit;
    Label5: TLabel;
    procedure btnGoClick(Sender: TObject);
    procedure Button1Click(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Готовый список переименований}
    List: TStringList;
    Excel: ExcelApplication;
    Indent: string;

    function MakeList: boolean;
    function ValidateLine(Line: string): boolean;
    function ResolveBook(Name: string): boolean;
    function ResolveSheet(Sheet: ExcelWorkSheet):boolean;
    function ResolveCP(Sheet: ExcelWorkSheet; Name: string): boolean;
    function ResolveMarkupCP(Sheet: ExcelWorkSheet; CPName, DimFullName: string): boolean;

    procedure WriteLog(Entry: string);
    procedure IndentRight;
    procedure IndentLeft;
    procedure ReadSettings;
    procedure WriteSettings;
  public
    { Public declarations }
  end;

var
  fmCodeUpdater: TfmCodeUpdater;

implementation

{$R *.DFM}

function TfmCodeUpdater.MakeList: boolean;
var
  i: integer;
  Line: string;
begin
  if not Assigned(List) then
    List := TStringlist.Create;
  List.Clear;

  for i := 0 to mList.Lines.Count - 1 do
  begin
    Line := mList.Lines[i];
    if ValidateLine(Line) then
      List.Add(Line);
  end;
  i := mList.Lines.Count - List.Count;
  if i > 0 then
    result := ShowQuestion(Format('Некоторые строки (%d) имеют неверный формат и не могут ' +
    'использоваться для переименования. Продолжить?', [i]))
  else
    result := true;
end;

function TfmCodeUpdater.ValidateLine(Line: string): boolean;
var
  Part1: string;
begin
  {Допустимы только два формата входной строки:
    Число=Число
    Число=null
  }
  result := false;
  Part1 := CutPart(Line, '=');
  if Line = '' then
    exit;
  Part1 := Trim(Part1);
  Line := Trim(Line);
  try
    StrToInt(Part1);
  except
    exit;
  end;
  try
    StrToInt(Line);
  except
    if AnsiUpperCase(Line) <> 'NULL' then
      exit;
  end;
  result := true;
end;

procedure TfmCodeUpdater.btnGoClick(Sender: TObject);
var
  DocPath: string;
  SearchRec: TSearchRec;
begin
  if not MakeList then
    exit;
  Indent := '';
  mlog.Clear;
  if not DirectoryExists(edWorkPath.Text) then
  begin
    WriteLog('Указанный каталог не существует!');
    exit;
  end;
  DocPath := edWorkPath.Text + '\*.xls';
  try
    if FindFirst(DocPath, faAnyFile, SearchRec) = 0 then
    begin
      if not GetExcel(Excel) then
      begin
        ShowError('Не удалось создать экземпляр Excel.');
        exit;
      end;
      ResolveBook(edWorkPath.Text + '\' + SearchRec.Name);
      while FindNext(SearchRec) = 0 do
      begin
        ResolveBook(edWorkPath.Text + '\' + SearchRec.Name);
      end;
    end;
    WriteLog('Процесс завершен.');
  finally
    FindClose(SearchRec);
    if Assigned(Excel) then
      Excel.Quit;
  end;
end;

function TfmCodeUpdater.ResolveBook(Name: string): boolean;
var
  Book: ExcelWorkBook;
  Sheet: ExcelworkSheet;
  i: integer;
begin
  result := false;
  try
    Book := Excel.Workbooks.Open(Name, false, false,
                  EmptyParam, EmptyParam, EmptyParam,
                  EmptyParam, EmptyParam, EmptyParam,
                  EmptyParam, EmptyParam, EmptyParam,
                  EmptyParam, EmptyParam, EmptyParam, GetUserDefaultLCID);

    WriteLog('Книга: ' + Book.Name);
    IndentRight;
    try
      for i := 1 to Book.Sheets.Count do
      begin
        Sheet := GetWorkSheet(Book.Sheets.Item[i]);
        if not Assigned(Sheet) then
          continue;
        if not IsPlaningSheet(Sheet) then
          continue;
        ResolveSheet(Sheet);
      end;
    finally
      IndentLeft;
    end;
  except
    on e: Exception do
      begin
        WriteLog(e.Message);
        exit;
      end;
  end;
  Book.Save(GetUserDefaultLCID);
  Book.Close(false, '', EmptyParam, GetUserDefaultLCID);
  result := true;
end;


function TfmCodeUpdater.ResolveSheet(Sheet: ExcelWorkSheet): boolean;
var
  Md: IXMLDOMDocument2;
  NL: IXMLDOMNodeList;
  XPath, Id, Pid: string;
  i: integer;
  ParamNode: IXMLDOMNode;
begin
  if not IsPlaningSheet(Sheet) then
    exit;
  Md := GetDataFromCP(Sheet, cpMDName);
  { Ищем все вхождения измерения в лист (строки, столбцы, фильтры, фильтры отдельных)}
  XPath := Format('(metadata//*[(dimension="%s") and (hierarchy="%s")])',
    [edDim.Text, edHier.Text]);
  NL := Md.selectNodes(XPath);
  WriteLog('Лист: ' + Sheet.Name);
  if NL.length = 0 then
  begin
    IndentRight;
    WriteLog('Измерение не найдено');
    IndentLeft;
    exit;
  end;
  for i := 0 to NL.length - 1 do
  begin
    {Если измерение параметризовано, то его элементы содержатся в хмл параметра}
    ParamNode := NL[i].selectSingleNode('paramProperties');
    if Assigned(ParamNode) then
    begin
      Pid := GetStrAttr(ParamNode, 'pid', '');
      if Pid = '' then
        continue;
      ResolveCP(Sheet, 'p' + Pid);
    end
    else
    begin
      Id := GetStrAttr(NL[i], attrId, '');
      if Id = '' then
        continue;
      ResolveCP(Sheet, Id);
    end;
  end;
  ResolveMarkupCP(Sheet, cpRowsMarkup, edFullName.Text);
  ResolveMarkupCP(Sheet, cpColumnsMarkup, edFullName.Text);
end;

procedure TfmCodeUpdater.WriteLog(Entry: string);
begin
  mLog.Lines.Add(Indent + Entry);
end;

function TfmCodeUpdater.ResolveCP(Sheet: ExcelWorkSheet; Name: string): boolean;
var
  Dom: IXMLDOMDocument2;
  i, j: integer;
  OldCode, NewCode: string;
  NL: IXMLDOMNodeList;
begin
  Dom := GetDataFromCP(Sheet, Name);
  if not Assigned(Dom) then
    exit;
  for i := 0 to List.Count - 1 do
  begin
    OldCode := List.Names[i];
    NewCode := List.Values[OldCode];
    NL := Dom.selectNodes(Format('function_result/Members//Member[@%s="%s"]',
      [attrPKID, OldCode]));
    IndentRight;
    for j := 0 to NL.length - 1 do
    begin
      SetAttr(NL[j], attrPKID, NewCode);
      WriteLog(OldCode + ' = ' + NewCode);
    end;
    IndentLeft;
  end;
  try
    PutDataInCP(Sheet, Name, Dom);
  except
    IndentRight;
    WriteLog('Ошибка перезаписи CP с Id = ' + Name);
    IndentLeft;
  end;
end;

function TfmCodeUpdater.ResolveMarkupCP(Sheet: ExcelWorkSheet; CPName, DimFullName: string): boolean;
var
  Dom: IXMLDOMDocument2;
  i, j: integer;
  OldCode, NewCode: string;
  NL: IXMLDOMNodeList;
begin
  Dom := GetDataFromCP(Sheet, CPName);
  if not Assigned(Dom) then
    exit;
  for i := 0 to List.Count - 1 do
  begin
    OldCode := List.Names[i];
    NewCode := List.Values[OldCode];
    NL := Dom.selectNodes(Format('markup/row/writeback[@%s="%s"]', [DimFullName, OldCode]));
    IndentRight;
    for j := 0 to NL.length - 1 do
    begin
      SetAttr(NL[j], DimFullName, NewCode);
      WriteLog(OldCode + ' = ' + NewCode);
    end;
    IndentLeft;
  end;
  try
    PutDataInCP(Sheet, CPName, Dom);
  except
    IndentRight;
    WriteLog('Ошибка перезаписи CP с Id = ' + Name);
    IndentLeft;
  end;
end;


procedure TfmCodeUpdater.Button1Click(Sender: TObject);
var
  Path: string;
begin
  //Path := Edit1.Text;
  //SelectDirectory(Path, [], 0);
  if not SelectDirectory('', '', Path) then
    exit;
  edWorkPath.Text :=  Path;
end;

procedure TfmCodeUpdater.IndentLeft;
begin
  if Length(Indent) > 2 then
    Delete(Indent, 1, 2)
  else
    Indent := '';
end;

procedure TfmCodeUpdater.IndentRight;
begin
  Indent := Indent + '  ';
end;

procedure TfmCodeUpdater.FormDestroy(Sender: TObject);
begin
  WriteSettings;
  if Assigned(Excel) then
    Excel.Quit;
end;

procedure TfmCodeUpdater.FormCreate(Sender: TObject);
begin
  ReadSettings;
end;

procedure TfmCodeUpdater.ReadSettings;
var
  Dir: string;
  F: TIniFile;
begin
  Dir := GetCurrentDir;
  F := TIniFile.Create(Dir + '\CodeUpdater.ini');
  edWorkPath.Text := F.ReadString('Options', 'WorkPath', Dir);
  edDim.Text := F.ReadString('Options', 'Dimension', 'Районы');
  edHier.Text := F.ReadString('Options', 'Hierarchy', 'Сопоставимый План');
  edFullName.Text := F.ReadString('Options', 'FullName', 'b.Regions.Bridge');
  mList.Clear;
  F.ReadSectionValues('Replacements', mList.Lines);
  F.Free;
end;

procedure TfmCodeUpdater.WriteSettings;
var
  Dir: string;
  F: TIniFile;
  i: integer;
begin
  Dir := GetCurrentDir;
  F := TIniFile.Create(Dir + '\CodeUpdater.ini');
  F.EraseSection('Options');
  F.EraseSection('Replacements');
  F.WriteString('Options', 'WorkPath', edWorkPath.Text);
  F.WriteString('Options', 'Dimension', edDim.Text);
  F.WriteString('Options', 'Hierarchy', edHier.Text);
  F.WriteString('Options', 'FullName', edFullName.Text);
  for i := 0 to mList.Lines.Count - 1 do
  try
    F.WriteString('Replacements', mList.Lines.Names[i], mList.Lines.Values[mList.Lines.Names[i]]);
  except
  end;
  F.Free;
end;

end.
