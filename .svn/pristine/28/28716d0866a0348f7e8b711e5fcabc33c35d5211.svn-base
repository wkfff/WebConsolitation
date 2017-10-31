unit uReplicationWizard;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  uDataCollectionFormWizard, ImgList, AddinMembersTree, ComCtrls, StdCtrls,
  ExtCtrls, fmWizardHeader, uSheetObjectModel, uFMAddinGeneralUtils,
  uAbstractWizard, MSXML2_TLB, uXmlCatalog, uXmlUtils, uGlobalPlaningConst,
  ExcelXP, uStringsEditor, uMPSelector, PlaningTools_TLB, ComObj, uExcelUtils,
  uFMAddinExcelUtils, uSheetHistory;

type
  TfmReplicationWizard = class(TfmAbstractWizard)
    tsFilters: TTabSheet;
    lvFilters: TListView;
    Label3: TLabel;
    tsMembers: TTabSheet;
    Panel1: TPanel;
    MembersTree: TAddinMembersTree;
    ImgList: TImageList;
    tsReplaces: TTabSheet;
    Label4: TLabel;
    tsDone: TTabSheet;
    memDone: TMemo;
    WarningLabel: TLabel;
    memReplaces: TMemo;
    btOpenDlg: TButton;
    dlgOpen: TOpenDialog;
    tsNames: TTabSheet;
    rbtnUseName: TRadioButton;
    rbtnUseProperty: TRadioButton;
    MPRadioSelector: TMPRadioSelector;
    cbPrefix: TCheckBox;
    edPrefix: TEdit;
    edReplacesFileName: TEdit;
    procedure tsFiltersShow(Sender: TObject);
    procedure lvFiltersMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure tsMembersShow(Sender: TObject);
    procedure btOpenDlgClick(Sender: TObject);
    procedure tsReplacesShow(Sender: TObject);
    procedure tsNamesShow(Sender: TObject);
    procedure MPRadioSelectorCheck(Sender: TObject);
  private
    FReplacesFileName: string;
    FLCID: integer;
    FSheetInterface: TSheetInterface;
    FHistoryForm: TSheetHistory;

    function GetCurrentFilter: TSheetFilterInterface;
    procedure SetReplacesFileName(const Value: string);
    procedure ReadReplacesFile;
    procedure ParseReplaceString(var Replace: string);

    procedure Replicate;
    procedure MakeSheetReplica(var AfterSheet: ExcelWorkSheet; FiltersList: TStringList;
      NewName, MDValue: string;  Dom: IXMLDOMDocument2);
    procedure ModyfyMetadata(var MDValue: string);
    procedure MakeSameFilterList(var List: TStringList);
    function AddCP(Sheet: ExcelWorkSheet; CPName, CPValue: string): boolean;

    procedure LoadFilters(From: TSheetFilterCollectionInterface; Cell: TSheetSingleCellInterface);

    function GetNewSheetName(Node: IXMLDOMNode; var Counter: integer; MaxCounter: integer): string;
    function GetPropertyValue(Node: IXMLDOMNode): string;
    procedure ApplyReplaces(var Name: string);
  protected
    procedure Back; override;
    procedure Next; override;
    procedure Done; override;

    property ReplacesFileName: string read FReplacesFileName write SetReplacesFileName;
  public
  end;

function RunReplicationWizard(ASheetInterface: TSheetInterface; HistoryForm: TSheetHistory): boolean;

implementation

{$R *.DFM}

const

  capFilters = 'Выбор фильтра';
  comFilters = 'Копии исходного листа будут различаться значениями ' +
    #13 + 'выбранного фильтра.';
  capMembers = 'Настройка фильтра "%s"';
  comMembers = 'Задайте подмножество элементов, которое будет использоваться' +
    #13 + ' для тиражирования.';
  capNames = 'Наименование листов';
  comNames = 'Выберите, на основе чего будут формироваться имена ' +
    #13 + 'создаваемых листов';
  capReplaces = 'Правила замен';
  comReplaces = 'Укажите путь к файлу замен длинных наименований элементов ' +
    #13 + 'сокращениями или настройте нужные замены вручную.';

  ReplacesComment1 = '// Для сокращения длинных имен настройте правила замен';
  ReplacesComment2 = '// шаблон=замена';

function RunReplicationWizard(ASheetInterface: TSheetInterface; HistoryForm: TSheetHistory): boolean;
var
  fmWizard: TfmReplicationWizard;
begin
  fmWizard := TfmReplicationWizard.Create(ASheetInterface);
  try
    with fmWizard do
    begin
      UseDefaultValueInXml := false;
      FLCID := GetUserDefaultLCID;
      pcMain.ActivePage := tsFilters;
      memReplaces.Clear;
      memReplaces.Lines.Add(ReplacesComment1);
      memReplaces.Lines.Add(ReplacesComment2);
      FSheetInterface := ASheetInterface;
      FHistoryForm := HistoryForm;
      result := ShowModal = mrOk;
    end;
  finally
    FreeAndNil(fmWizard);
  end;
end;

{ TfmReplicationWizard }

procedure TfmReplicationWizard.lvFiltersMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  i: integer;
  Item: TListItem;
begin
  Item := ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
  if Assigned(Item) then
    if not Item.Checked then
      Item.Checked := true;
  for i := 0 to lvFilters.Items.Count - 1 do
    if lvFilters.Items[i] <> Item then
      lvFilters.Items[i].Checked := false;
end;

procedure TfmReplicationWizard.tsFiltersShow(Sender: TObject);
var
  i: integer;
begin
  inherited;
  SetupHeader(capFilters, comFilters);
  if MovingForward then
  begin
    SetupButtons(false, false, false);
    lvFilters.Items.Clear;

    LoadFilters(SheetInterface.Filters, nil);
    for i := 0 to SheetInterface.SingleCells.Count - 1 do
      LoadFilters(SheetInterface.SingleCells[i].Filters, SheetInterface.SingleCells[i]);

    WarningLabel.Visible := lvFilters.Items.Count = 0;
    lvFilters.Enabled := lvFilters.Items.Count > 0;
    if lvFilters.Items.Count > 0 then
      lvFilters.Items[0].Checked := true;
    btnNext.Enabled := lvFilters.Items.Count > 0;
  end
  else
    SetupButtons(false, true, false);
end;

procedure TfmReplicationWizard.tsMembersShow(Sender: TObject);
var
  Filter: TSheetFilterInterface;
  Dom: IXMLDOMDocument2;
  Dimension: TDimension;
  Hierarchy: THierarchy;
  Xml: string;
  Op: IOperation;
begin
  inherited;
  Filter := GetCurrentFilter;
  SetupHeader(Format(capMembers, [Filter.GetElementCaption]), comMembers);
  SetupButtons(true, true, false);
  if MovingForward then
  begin
    Op := CreateComObject(CLASS_Operation) as IOperation;
    Op.StartOperation(Self.Handle);
    Op.Caption := 'Загрузка элементов...';
    try
      Xml := LoadXml(Filter);
      GetDomDocument(Dom);
      Dom.loadXml(Xml);
      Dimension := SheetInterface.XMLCatalog.Dimensions.Find(Filter.Dimension, Filter.ProviderId);
      Hierarchy := Dimension.GetHierarchy(Filter.Hierarchy);
      MembersTree.Clear;
      MembersTree.Load(Dom, Hierarchy.Levels.ToString, Hierarchy.CodeToShow);
    finally
      Application.ProcessMessages;
      Op.StopOperation;
      SetActiveWindow(Self.Handle);
      Op := nil;
    end;
  end;
end;

procedure TfmReplicationWizard.tsNamesShow(Sender: TObject);
begin
  inherited;
  SetupHeader(capNames, comNames);
  SetupButtons(true, true, false);
  if MovingForward then
  begin
    MPRadioSelector.Load(GetCurrentFilter.MemberProperties);
    edPrefix.Text := ExcelSheet.Name;
  end;
end;

procedure TfmReplicationWizard.tsReplacesShow(Sender: TObject);
begin
  inherited;
  SetupHeader(capReplaces, comReplaces);
  SetupButtons(true, false, true);
end;

procedure TfmReplicationWizard.Back;
begin
  inherited Back;
  case CurrentPage.PageIndex of
    1: // элементы - возрващаемся к выбору фильтра
      begin
        SaveXML(GetCurrentFilter, MembersTree.MembersDOM);
        CurrentPage := tsFilters;
      end;
    2: // имена листов - возрващаемся к элементам фильтра
      begin
        MPRadioSelector.Save(GetCurrentFilter.MemberProperties);
        CurrentPage := tsMembers;
      end;
    3: // замены - возвращаемся к правилам именования листов
      begin
        CurrentPage := tsNames;
      end;
  end;

end;

procedure TfmReplicationWizard.Done;
begin
  Replicate;
end;

procedure TfmReplicationWizard.Next;
begin
  inherited Next;
  case CurrentPage.PageIndex of
    0:  // фильтры - переходим к выбору элементов
      CurrentPage := tsMembers;
    1:  // элементы - переходим к правилам именования листов
      begin
        SaveXML(GetCurrentFilter, MembersTree.MembersDOM);
        CurrentPage := tsNames;
      end;
    2:  // имена - переходим к заменам
      CurrentPage := tsReplaces;
  end;
end;

function TfmReplicationWizard.GetCurrentFilter: TSheetFilterInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to lvFilters.Items.Count - 1 do
    if lvFilters.Items[i].Checked then
    begin
      result := lvFilters.Items[i].Data;
      exit;
    end;
end;

procedure TfmReplicationWizard.btOpenDlgClick(Sender: TObject);
begin
  inherited;
  if not dlgOpen.Execute then
    exit;

  ReplacesFileName := dlgOpen.FileName;
  ReadReplacesFile;
end;

procedure TfmReplicationWizard.SetReplacesFileName(const Value: string);
begin
  FReplacesFileName := Value;
  edReplacesFileName.Text := Value;
end;

{Читает файл замен}
procedure TfmReplicationWizard.ReadReplacesFile;
var
  F: TextFile;
  s: string;
begin
  memReplaces.Clear;
  AssignFile(F, ReplacesFileName);
  Reset(F);
  while not EOF(F) do
  begin
    readln(F, s);
    ParseReplaceString(s);
    memReplaces.Lines.Add(s);
  end;
  CloseFile(F);
end;

procedure TfmReplicationWizard.ParseReplaceString(var Replace: string);
begin
  {Здесь должна быть проверка допустимости строки в качестве замены}
  if Pos('=', Replace) = 0 then
    Replace := '//' + Replace;
end;

procedure TfmReplicationWizard.Replicate;
var
  Filter: TSheetFilterInterface;
  Xml, NewSheetName, OldFilterCPName, NewFilterCPName, MDValue, HistoryComment: string;
  Dom: IXMLDOMDocument2;
  NL: IXMLDOMNodeList;
  i, j, Counter, CheckedCount: integer;
  AfterSheet: ExcelWorkSheet;
  SameFilters: TStringList;
begin
  Filter := GetCurrentFilter;
  Xml := LoadXml(Filter);
  OldFilterCPName := Filter.CPName;

  HistoryComment :=  Format('Тиражирование по фильтру "%s".', [Filter.GetElementCaption]);
  if GetCurrentFilter.IsParam then
    HistoryComment :=  HistoryComment + Format(' Связь с параметром "%s" удалена.', [Filter.Param.Name]);
  HistoryComment := ConvertStringToCommaText(HistoryComment);

  GetDomDocument(Dom);
  Dom.loadXML(Xml);
  Counter := 0;
  AfterSheet := ExcelSheet;

  ExcelSheet.Application.DisplayAlerts[FLCID] := false;
  ExcelSheet.Application.ScreenUpdating[FLCID] := false;
  ExcelSheet.Application.Set_Interactive(FLCID, false);
  try

    {Цикл по числу выбранных элементов в фильтре. Сколько элементов - столько будет создано листов}
    NL := Dom.selectNodes(Format('function_result/Members//Member[@%s="true"]', [attrChecked]));
    CheckedCount := NL.length;
    for i := 0 to CheckedCount - 1 do
    begin
      {Для каждого листа надо подготовить собственный хмл фильтра, содержащий только один элемент}
      for j := 0 to CheckedCount - 1 do
        if j <> i then
          SetAttr(NL[j], attrChecked, 'false');

      NewSheetName := GetNewSheetName(NL[i], Counter, NL.length);
      ModyfyMetadata(MDValue);
      MakeSameFilterList(SameFilters);
      NewFilterCPName := Filter.CPName;
      MakeSheetReplica(AfterSheet, SameFilters, NewSheetName, MDValue, Dom);
      FHistoryForm.AddEvent(AfterSheet, evtSheetCopy, HistoryComment, true);

      {После формирования листа загрузим исходный, "неиспорченный" хмл}
      Dom.loadXml(Xml);
      NL := Dom.selectNodes(Format('function_result/Members//Member[@%s="true"]', [attrChecked]));
    end;

  finally
    ExcelSheet.Application.DisplayAlerts[FLCID] := true;
    ExcelSheet.Application.ScreenUpdating[FLCID] := true;
    ExcelSheet.Application.Set_Interactive(FLCID, true);
    ExcelSheet.Activate(FLCID);
    Dom := nil;
    FreeStringList(SameFilters);
  end;
end;

procedure TfmReplicationWizard.MPRadioSelectorCheck(Sender: TObject);
begin
  inherited;
  rbtnUseProperty.Checked := true;
end;

function TfmReplicationWizard.AddCP(Sheet: ExcelWorkSheet; CPName, CPValue: string): boolean;
var
  Msg, ExtraMsg: string;
begin
  result := false;
  try
    Sheet.CustomProperties.Add(CPName, CPValue);
  except
    on e: exception do
    begin
      Msg := Format('При создании листа %s произошла ошибка копирования служебной информации',
        [Sheet.Name]);
      ExtraMsg := Format('"%s": %s', [CPName, e.Message]);
      ShowDetailError(Msg, ExtraMsg, 'Ошибка тиражирования');
      exit;
    end;
  end;
  result := true;
end;

procedure TfmReplicationWizard.MakeSheetReplica(var AfterSheet: ExcelWorkSheet;
  FiltersList: TStringList; NewName, MDValue: string; Dom: IXMLDOMDocument2);
var
  i: integer;
  CPName: string;
  CPValue: string;
begin
    ExcelSheet.Copy(EmptyParam, AfterSheet, FLCID);
    AfterSheet := AfterSheet.Next as ExcelWorkSheet;
    //ExcelSheet.Cells.Copy(AfterSheet.Cells);
    try
      AfterSheet.Name := NewName;
    except
    end;

    for i := 0 to FiltersList.Count - 1 do
    begin
      if not AddCP(AfterSheet, FiltersList[i], Dom.xml) then
        exit;
    end;

    for i := 1 to ExcelSheet.CustomProperties.Count do
    begin
      CPName := ExcelSheet.CustomProperties[i].Name;
      CPValue := ExcelSheet.CustomProperties[i].Value;

      if FiltersList.IndexOf(CPName) > -1 then
        continue;

      if CPName = cpMDName then
        CPValue := MDValue;

      if not AddCP(AfterSheet, CPName, CPValue) then
        exit;
    end;
end;

{Формирует имя листа на основе заданных пользователем установок}
function TfmReplicationWizard.GetNewSheetName(Node: IXMLDOMNode;
  var Counter: integer; MaxCounter: integer): string;
var
  CounterLength: integer;
  CounterStr: string;
begin
  {получаем заготовку}
  if rbtnUseName.Checked then
    result := GetStrAttr(Node, attrName, '')
  else
    result := GetPropertyValue(Node);

  {применяем правила}
  ApplyReplaces(result);
  if cbPrefix.Checked then
    result := edPrefix.Text + result;

  if Length(result) <= 31 then
    exit;

  {если все еще слишком длинное - обрезаем и нумеруем}
  inc(Counter);
  CounterLength := 3 + (MaxCounter div 10);
  result := Copy(result, 1, 31 - CounterLength);
  CounterStr := '(' + IntToStr(Counter) + ')';
  result := result + CounterStr;
end;

{Получает значение выбранного в селекторе свойства}
function TfmReplicationWizard.GetPropertyValue(Node: IXMLDOMNode): string;
var
  PropertyName: string;
begin
  result := '';
  PropertyName := MPRadioSelector.CheckedPropertyName;
  if PropertyName = '' then
    exit;
  EncodeMemberPropertyName(PropertyName);
  result := GetStrAttr(Node, PropertyName, '');
end;

{Применяет к имени правила замен}
procedure TfmReplicationWizard.ApplyReplaces(var Name: string);
var
  i, Index: integer;
  Rule, OldPattern, NewPattern: string;
begin
  for i := 0 to memReplaces.Lines.Count - 1 do
  begin
    Rule := TrimLeft(memReplaces.Lines[i]);
    if Pos('//', Rule) = 1 then
      continue;
    Index := Pos('=', Rule);
    OldPattern := Copy(Rule, 1, Index - 1);
    NewPattern := Copy(Rule, Index + 1, Length(Rule) - Index);
    Name := StringReplace(Name, OldPattern, NewPattern, [rfReplaceAll, rfIgnoreCase]);
  end;
end;

procedure TfmReplicationWizard.LoadFilters(From: TSheetFilterCollectionInterface; Cell: TSheetSingleCellInterface);
var
  i: integer;
  Filter: TSheetFilterInterface;
begin
  for i := 0 to From.Count - 1 do
  begin
    Filter := From[i];
    with lvFilters.Items.Add do
    begin
      Caption := Filter.GetElementCaption;
      ImageIndex := 22;
      Checked := false;
      Data := Filter;
      if Assigned(Cell) then
        SubItems.Add(Cell.GetElementCaption)
      else
        SubItems.Add(IIF(Filter.IsPartial, 'Частный фильтр', 'Общий фильтр'));
    end;
  end;
end;

procedure TfmReplicationWizard.ModyfyMetadata(var MDValue: string);
var
  Filter, CellFilter: TSheetFilterInterface;
  Dom: IXMLDOMDocument2;
  CellIndex, FilterIndex: integer;
  Cell: TSheetSingleCellInterface;
  ParamName: string;
begin
  FSheetInterface.LastRefreshDate := 'Обнови лист!';

  {Связь с параметром разрывать необходимо, при работе в контексте задачи
    параметр испортит растиражированные листы.}
  Filter := GetCurrentFilter;
  if Filter.IsParam then
  begin
    ParamName := Filter.Param.Name;
    Filter.Param.RemoveLink(Filter);
  end;

  {Поскольку на листе могут быть несколько отдельных показателей с одинаковым
    фильтром, по которому и осуществляется тиражирование, то их связи с параметром
    так же нужно разорвать.}
  if Assigned(Filter.OwningCell) then
    for CellIndex := 0 to FSheetInterface.SingleCells.Count - 1 do
    begin
      Cell := FSheetInterface.SingleCells[CellIndex];
      for FilterIndex := 0 to Cell.Filters.Count - 1 do
      begin
        CellFilter := Cell.Filters[FilterIndex];
        if not CellFilter.IsParam then
          continue;
        if CellFilter.Param.Name = ParamName then
          CellFilter.Param.RemoveLink(CellFilter);
      end;
    end;

  FSheetInterface.ExportXml(Dom);
  MDValue := Dom.xml;
end;

procedure TfmReplicationWizard.MakeSameFilterList(var List: TStringList);
var
  CellIndex, FilterIndex: integer;
  CurrentFilter, CellFilter: TSheetFilterInterface;
  Cell: TSheetSingleCellInterface;
begin
  if not Assigned(List) then
    List := TStringList.Create
  else
    List.Clear;

  CurrentFilter := GetCurrentFilter;
  List.Add(CurrentFilter.CPName);

  for CellIndex := 0 to FSheetInterface.SingleCells.Count - 1 do
  begin
    Cell := FSheetInterface.SingleCells[CellIndex];
    if Cell = CurrentFilter.OwningCell then
      continue;
    for FilterIndex := 0 to Cell.Filters.Count - 1 do
    begin
      CellFilter := Cell.Filters[FilterIndex];
      if (CellFilter.Dimension <> CurrentFilter.Dimension) or
        (CellFilter.Hierarchy <> CurrentFilter.Hierarchy) then
        continue;
      if CellFilter.MdxText = CurrentFilter.MdxText then
        List.Add(CellFilter.CPName);
    end;
  end;
end;

end.

