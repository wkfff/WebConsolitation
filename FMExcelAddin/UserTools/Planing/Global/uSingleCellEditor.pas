{форма - редактор единичного показателя}

unit uSingleCellEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, StdCtrls, ExtCtrls, Spin, uCheckTV2, uOfficeUtils, 
  AddinDimensionsTree, AddinMembersTree, ImgList, uXMLCatalog,
  uFMAddinGeneralUtils, uFMExcelAddInConst, uSheetObjectModel, Buttons,
  PlaningTools_TLB, PlaningProvider_TLB, MSXML2_TLB, uXMLUtils, ComObj,
  uFMAddinXMLUtils, uMeasureSelector, uGlobalPlaningConst, uFMAddinExcelUtils,
  uExcelUtils;

type

  TDimData = class(TObject)
  public
    Members: IXMLDOMDocument2;
    constructor Create;
    destructor Destroy; override;
  end;

  TParamData = class(TDimData)
  public
    Name: string;
    Comment: string;
    Dimension: string;
    Id: integer;
    MultiSelect: boolean;
    IsInherited: boolean;
  end;

  TfmSingleCellEditor = class(TForm)
    btnOK: TButton;
    btnCancel: TButton;
    ImgList: TImageList;
    PageControl: TPageControl;
    tsMeasure: TTabSheet;
    gbTotalFormat: TGroupBox;
    Label8: TLabel;
    Label9: TLabel;
    cmbTotalFormat: TComboBox;
    seDigits: TSpinEdit;
    tsFilters: TTabSheet;
    GroupBox1: TGroupBox;
    btnSelectMeasure: TBitBtn;
    Label2: TLabel;
    Label3: TLabel;
    lblCube: TLabel;
    lblMeasure: TLabel;
    Label1: TLabel;
    eName: TEdit;
    Panel1: TPanel;
    MemTree: TAddinMembersTree;
    sbDimension: TStatusBar;
    cbResult: TCheckBox;
    Panel2: TPanel;
    Splitter1: TSplitter;
    Panel3: TPanel;
    StatusBar1: TStatusBar;
    DimTree: TSingleCubeDimTree;
    Panel4: TPanel;
    StatusBar2: TStatusBar;
    Splitter2: TSplitter;
    ParamTree: TBasicCheckTreeView;
    Label4: TLabel;
    cmbTotalMultiplier: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure PageControlChange(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure cmbTotalFormatChange(Sender: TObject);
    procedure btnSelectMeasureClick(Sender: TObject);
    procedure cbResultClick(Sender: TObject);
    procedure cbResultKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnOKClick(Sender: TObject);
    procedure DimTreeEnter(Sender: TObject);
    procedure ParamTreeInfoTip(Sender: TObject; Item: TTreeNode;
      var InfoTip: String);
    procedure ParamTreeChange(Sender: TObject; Node: TTreeNode);
    procedure ParamTreeChanging(Sender: TObject; Node: TTreeNode;
      var AllowChange: Boolean);
    procedure ParamTreeClick(Sender: TObject);
    procedure ParamTreeEnter(Sender: TObject);
    procedure tsFiltersShow(Sender: TObject);
  private
    {внешние ссылки на объектную модель}
    FCatalog: TXMLCatalog;
    FPlaningSheet: TSheetInterface;

    {выбранные на первой странице куб и мера}
    FSelectedCube: TCube;
    FSelectedMeasure: TMeasure;

    {выбранные на второй странице измерение и иерархия}
    FSelectedDimension: TDimension;
    FSelectedHierarchy: THierarchy;

    {индекс редактируемой ячейки; если создаем новую, то -1}
    FCellIndex: integer;

    {список "указателей" на загруженные деревья измерений -
      по сути локальный кэш;
      строки имеют вид "имя измерения"$$$"имя иерархии";
      значения параметров кэшируются непосредственно в контроле ParamView}
    FDimensionsData: TStringList;

    {признак состояния загрузки, обработчики событий не должны срабатывать}
    FLoading: boolean;

    {дескриптор родительского окна, на фоне которого показывать "шестеренки"}
    FParentWindowHandle: THandle;
    Op: IOperation;

    {список имен фильтров, у которых не выбрано ни одного элемента}
    EmptyFilters: TStringList;
    {коллекция параметров задачи}
    Params: TTaskParamsCollection;

    {помогает узнать, с каким из двух контролов - DimTree или ParamTree -
      в данный момент работает дерево мемберов}
    DimTreeFocused: boolean;

    {разрешает/запрещает контролы на определенной стадии действий юзера
      № 1 - выбрана мера, можно приступать к работе с измерениями фильтров}
    procedure EnableControls(Index: integer; State: boolean);

    {загружает дерево измерений выбранного куба, возвращает успех/неуспех}
    function LoadDimensionsTree: boolean;
    {загружает список параметров задачи (с фильтром по кубу)}
    procedure LoadTaskParams;
    procedure LoadSheetParams;
    procedure LoadParamList;
    {загружает дерево элементов выбранного измерения, возвращает успех/неуспех}
    function LoadMembersTree: boolean;

    {изменился выбор в дереве измерений}
    procedure OnDimensionChange(Sender: TObject; Node: TTreeNode);
    procedure OnDimensionChanging(Sender: TObject; Node: TTreeNode;
      var AllowChange: boolean);
    procedure OnTabChanging(Sender: TObject; var AllowChange: boolean);
    procedure OnMemberNodeCheck(Sender: TObject);
    procedure OnDimNodeCheck(Sender: TObject);

    {добавляет дерево элементов измерения в кэш измерений}
    procedure AddDimensionData(AName: string; Dom: IXMLDOmDocument2);
    {кэширует значение параметра с обновлением}
    procedure CacheTaskParamValue(Param: TTaskParam; var ParamData: TParamData);
    procedure CacheSheetParamValue(Param: TParamInterface; var ParamData: TParamData);

    {очистка кэша}
    procedure ClearDimData;
    procedure ClearParamsData;
    procedure ClearSelections;

    {вернет ранее загруженное дерево мемберов}
    function GetMembersDom(DimName, HierName: string): IXMLDOMDocument2;
    {по значению параметра задачи вернет соответствующие измерение и иерархию
       из каталога}
    function GetDimAndHierByFullName(AName: string; out Dim: TDimension;
      out Hier: THierarchy): boolean;

    {проверяет фильтр на наличие выбранных элементов, если таковых нет, то
      заносит его в список EmptyFilters}
    procedure CheckForEmptiness(AName: string; Checked: boolean);
    {проверяет наличие отмеченных "галкой" фильтров, основанных на одинаковых
      измерениях}
    function CheckForDuplicates(out ErrText: string): boolean;
    function CheckForMultiSelection(out ErrText: string): boolean;

    procedure CreateDimFilters(Cell: TSheetSingleCellInterface);
    procedure CreateParamFilters(Cell: TSheetSingleCellInterface);

    {принятие внесенных изменений}
    procedure ApplyChanges;
    {при редактировании существующей ячейки нужно проинициализировать контролы}
    procedure SetupMeasure(SingleCell: TSheetSingleCellInterface);


  protected
    {добавляем или редактируем}
    function GetIsAdding: boolean;

    function GetSelectedCube: TCube;
    function GetSelectedMeasure: TMeasure;
    function GetSelectedDimension: TDimension;
    function GetSelectedHierarchy: THierarchy;
    procedure SetCatalog(const Value: TXMLCatalog);

    property IsAdding: boolean read GetIsAdding;

    property Catalog: TXMLCatalog read FCatalog write SetCatalog;
    property PlaningSheet: TSheetInterface read FPlaningSheet;

    property SelectedCube: TCube read GetSelectedCube;
    property SelectedMeasure: TMeasure read GetSelectedMeasure;
    property SelectedDimension: TDimension read GetSelectedDimension;
    property SelectedHierarchy: THierarchy read GetSelectedHierarchy;
  public
    { Public declarations }
  end;

  function EditSingleCell(Sheet: TSheetInterface; var Index: integer; ParentWindowHandle: THandle): boolean;

implementation

{$R *.DFM}


function EditSingleCell(Sheet: TSheetInterface;
  var Index: integer; ParentWindowHandle: THandle): boolean;
var
  fmSingleCellEditor: TfmSingleCellEditor;
begin
  fmSingleCellEditor := TfmSingleCellEditor.Create(nil);
  with fmSingleCellEditor do
  try
    FPlaningSheet := Sheet;
    Catalog := FPlaningSheet.XMLCatalog;
    FCellIndex := Index;
    FParentWindowHandle := ParentWindowHandle;
    result := false;
    if IsAdding then
    begin
      cmbTotalMultiplier.ItemIndex := ord(FPlaningSheet.TotalMultiplier);
      // на листах типа "отчет" запрещаем добавление показателей - результатов
      if Assigned(FPlaningSheet.ExcelSheet) then
        cbResult.Enabled := (GetWBCustomPropertyValue(FPlaningSheet.ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType) <> '2')
      else
        cbResult.Enabled := true;
      Caption := 'Добавление показателя';
    end
    else
      SetupMeasure(Sheet.SingleCells[Index]);
    PageControl.ActivePageIndex := 0;
    result := ShowModal = mrOK;
    if result then
    begin
      ApplyChanges;
      Index := FCellIndex;
    end;
  finally
    fmSingleCellEditor.Free;
    Application.ProcessMessages;
  end;
end;

{ TfmSingleCellEditor }

procedure TfmSingleCellEditor.EnableControls(Index: integer; State: boolean);
var
  ResultEnabled: boolean;
begin
  case Index of
    1: // выбрана мера - можно разрешить страницу фильтров и свойства формата
      begin
        PageControl.Pages[1].Enabled := State;
        //cmbTotalFormat.Enabled := State and not cbResult.Checked;
        seDigits.Enabled := State and (cmbTotalFormat.ItemIndex in
          [Ord(fmtCurrency), Ord(fmtPercent), Ord(fmtNumber)]);
        if (Assigned(FPlaningSheet) and Assigned(FPlaningSheet.ExcelSheet))then
          ResultEnabled := (GetWBCustomPropertyValue(FPlaningSheet.ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType) <> '2')
        else
          ResultEnabled := true;
        cbResult.Enabled := ResultEnabled and State and SelectedCube.WritebackPossible and
                            not SelectedMeasure.IsCalculated{ and IsAdding};
        if not cbResult.Enabled {and IsAdding} then
          cbResult.Checked := false;
        cmbTotalMultiplier.Enabled := State and (cmbTotalFormat.ItemIndex = Ord(fmtCurrency));
        btnOK.Enabled := State;
      end;
  end;
end;

procedure TfmSingleCellEditor.FormCreate(Sender: TObject);
begin
  FCatalog := nil;
  FPlaningSheet := nil;
  DimTree.Images := ImgList;
  DimTree.ImageIndexes[ntCube] := 0;
  DimTree.ImageIndexes[ntDimension] := 2;
  DimTree.ImageIndexes[ntHierarchy] := 3;
  DimTree.ShowCheckboxes[ntHierarchy] := true;
  DimTree.HideSelection := false;
  ParamTree.HideSelection := true;
  ClearSelections;
  lblCube.Caption := 'куб не выбран';
  lblMeasure.Caption := 'мера не выбрана';
  eName.Text := '';
  PageControl.OnChanging := OnTabChanging;
  EnableControls(1, false);
  DimTree.OnChange := OnDimensionChange;
  DimTree.OnChanging := OnDimensionChanging;
  FDimensionsData := TStringList.Create;
  EmptyFilters := TStringList.Create;
  EmptyFilters.Sorted := true;
  EmptyFilters.Duplicates := dupIgnore;
  MemTree.Images := ImgList;
  MemTree.PageIndex := 0;
  MemTree.OnNodeCheck := OnMemberNodeCheck;
  MemTree.IsFilterBehavior := false;
  DimTree.OnNodeCheck := OnDimNodeCheck;
  FLoading := false;
  Op := CoOperation.Create;
end;

function TfmSingleCellEditor.LoadDimensionsTree: boolean;
begin
  result := DimTree.Load(SelectedCube, false);
end;

procedure TfmSingleCellEditor.PageControlChange(Sender: TObject);
begin
  if PageControl.ActivePageIndex = 1 then
  begin
    if DimTree.Cube <> SelectedCube then
    begin
      LoadDimensionsTree;
      LoadParamList;
    end;
  end;
end;

procedure TfmSingleCellEditor.SetCatalog(const Value: TXMLCatalog);
begin
  FCatalog := Value;
  DimTree.Catalog := Value;
end;

function TfmSingleCellEditor.GetSelectedCube: TCube;
begin
  result := FSelectedCube;
end;

function TfmSingleCellEditor.GetSelectedMeasure: TMeasure;
begin
  result := FSelectedMeasure;
end;

function TfmSingleCellEditor.GetSelectedDimension: TDimension;
begin
  result := FSelectedDimension;
end;

function TfmSingleCellEditor.GetSelectedHierarchy: THierarchy;
begin
  result := FSelectedHierarchy;
end;

procedure TfmSingleCellEditor.SetupMeasure(SingleCell: TSheetSingleCellInterface);
var
  i: integer;
  DimName, HierName: string;
  Node: TBasicCheckTreeNode;
  NewMembers: IXMLDOMDocument2;
  AParentHandle: Hwnd;
begin
  ClearSelections;
  if not Assigned(SingleCell) then
    exit;
  Floading := true;
  AParentHandle := GetActiveWindow;
  Op.StartOperation(AParentHandle);
  Op.Caption := 'Загрузка';
  try
    cmbTotalFormat.ItemIndex := Ord(SingleCell.Format);
    cmbTotalFormatChange(nil);
    seDigits.Value := SingleCell.Digits;
    cmbTotalMultiplier.ItemIndex := ord(SingleCell.TotalMultiplier);
    eName.Text := SingleCell.Name;

    FSelectedCube := SingleCell.Cube;
    FSelectedMeasure := SingleCell.Measure;
    if not Assigned(FSelectedCube) then
    begin
      ShowError('Куб "' + SingleCell.CubeName + '" не найден.');
      exit;
    end;
    if not Assigned(FSelectedMeasure) then
    begin
      ShowError('Мера "' + SingleCell.MeasureName + '" не найдена.');
      exit;
    end;

    cbResult.Checked := SingleCell.TotalType = wtResult;
    cbResultClick(nil);
    Caption := 'Редактирование показателя - ' +
      IIF(SingleCell.TotalType = wtMeasure, 'меры','результата');

    EnableControls(1, true);
    lblCube.Caption := SelectedCube.Name;
    lblMeasure.Caption := SelectedMeasure.Name;

    ClearDimData;
    LoadDimensionsTree;
    LoadParamList;
    DimTreeFocused := true;
    for i := 0 to SingleCell.Filters.Count - 1 do
    begin
      DimName := SingleCell.Filters[i].Dimension;
      HierName := SingleCell.Filters[i].Hierarchy;

      if SingleCell.Filters[i].IsParam then
      begin
        Node := ParamTree.FindNodeByName(SingleCell.Filters[i].Param.Name);
        if Assigned(Node) then
        begin
          Node.Checked := true;
          Node.Selected := true;
          DimTreeFocused := false;
        end;
      end
      else
      begin
        Node := DimTree.FindNodeBy2Names(DimName,
          IIF(HierName = '', DefaultHierarchyName, HierName));
        if Assigned(Node) then
        begin
          Node.Selected := true;
          DimTreeFocused := true;
          NewMembers := PlaningSheet.DataProvider.GetMemberList(
            SelectedHierarchy.ProviderId,
            SelectedCube.Name,
            SelectedDimension.Name, SelectedHierarchy.Name,
            SelectedHierarchy.Levels.ToString,
            SelectedHierarchy.MemberProperties.GetCommaList);
          // проверка на эксепт
          if Assigned(NewMembers.selectSingleNode('Exception')) then
            continue;
          Node.Checked := true;
          CopyMembersState(SingleCell.Filters[i].Members, NewMembers, nil);
          AddDimensionData(DimName + snBucks + HierName, NewMembers);
        end;
      end;
    end;
    LoadMembersTree;
  finally
    Op.StopOperation;
    Floading := false;
  end;
end;

procedure TfmSingleCellEditor.OnTabChanging(Sender: TObject;
  var AllowChange: boolean);
begin
  if (PageControl.ActivePageIndex = 0) then
    if not Assigned(SelectedMeasure) then
    begin
      AllowChange := false;
      ShowWarning('Не выбрана мера');
    end;
end;

procedure TfmSingleCellEditor.OnDimensionChange(Sender: TObject;
  Node: TTreeNode);
var
  AText: string;
begin
  AText := DimTree.OutputDimensionName;
  sbDimension.Panels[0].Text := IIF(AText = '', '', ' Измерение: ' + AText);
  if DimTree.SelectionType = ntCube then
    exit;
  FSelectedDimension := DimTree.Dimension;
  FSelectedHierarchy := DimTree.Hierarchy;
  if FLoading then
    exit;
  LoadMembersTree;
  DimTreeFocused := true;
end;

procedure TfmSingleCellEditor.OnDimensionChanging(Sender: TObject;
  Node: TTreeNode; var AllowChange: boolean);
begin
  if FLoading then
    exit;
  MemTree.SyncronizeSearchWithTree;
end;

function TfmSingleCellEditor.LoadMembersTree: boolean;
var
  Dom: IXMLDOMDocument2;
begin
  result := false;
  if Assigned(SelectedDimension) and Assigned(SelectedHierarchy) then
  begin
    {возможно, что уже загружали...}
    Dom := GetMembersDom(SelectedDimension.Name, SelectedHierarchy.Name);
    {...нет, грузим впервые}
    if not Assigned(Dom) then
    begin
      try
        if Self.Showing then
        begin
          Op.StartOperation(Self.Handle);
          Op.Caption := pcapLoadMembers;
        end;
        Dom := PlaningSheet.DataProvider.GetMemberList(
          SelectedHierarchy.ProviderId,
          SelectedCube.Name,
          SelectedDimension.Name, SelectedHierarchy.Name,
          SelectedHierarchy.Levels.ToString,
          SelectedHierarchy.MemberProperties.GetCommaList);
      finally
        //чтобы эксель не терял фокус
        if Self.Showing then
        begin
          Application.ProcessMessages;
          Op.StopOperation;
          SetActiveWindow(Self.Handle);
        end;
      end;
      if Assigned(Dom.selectSingleNode('Exception')) then
      begin
        ShowDetailError(ermMembersLoadFault, PlaningSheet.DataProvider.LastError,
          ermMembersLoadFault);
        exit;
      end;
      {загруженное дерево сохраним для дальнейшего использования}
      AddDimensionData(SelectedDimension.Name + snBucks + SelectedHierarchy.Name, Dom);
    end;
    {теперь с ним можно работать}
    MemTree.Load(Dom, SelectedHierarchy.Levels.ToString, SelectedHierarchy.CodeToShow);
    MemTree.MembersReadOnly := false;
  end;
end;

function TfmSingleCellEditor.GetMembersDom(DimName,
  HierName: string): IXMLDOMDocument2;
var
  Index: integer;
begin
  result := nil;
  Index := FDimensionsData.IndexOf(DimName + snBucks + HierName);
  if Index > -1 then
    result := TDimData(FDimensionsData.Objects[Index]).Members;
end;

procedure TfmSingleCellEditor.ApplyChanges;
var
  Cell: TSheetSingleCellInterface;
  CommentForHistory: string;
begin
  if IsAdding then
  begin
    Cell := PlaningSheet.SingleCells.Append;
    FCellIndex := PlaningSheet.SingleCells.Count - 1;
    if cbResult.Checked then
      Cell.TotalType := wtResult
    else
      Cell.TotalType := wtMeasure;
  end
  else
  begin
    Cell := PlaningSheet.SingleCells[FCellIndex];
    if (cbResult.Checked and (Cell.TotalType = wtMeasure)) or
      (not cbResult.Checked and (Cell.TotalType = wtResult)) then
        Cell.SwitchType(CommentForHistory);
  end;

  with Cell do
  begin
    CubeName := SelectedCube.Name;
    MeasureName := SelectedMeasure.Name;
    ProviderId := SelectedCube.ProviderId;
    Format := TMeasureFormat(cmbTotalFormat.ItemIndex);
    Digits := seDigits.Value;
    TotalMultiplier := TTotalMultiplier(cmbTotalMultiplier.ItemIndex);
    Name := eName.Text;
  end;

  PlaningSheet.KillZeroLinkedParams := false;
  Cell.Filters.Clear;
  {создание фильтров-измерений}
  CreateDimFilters(Cell);
  {создание фильтров-параметров}
  CreateParamFilters(Cell);
  PlaningSheet.KillZeroLinkedParams := true;
end;

function TfmSingleCellEditor.GetIsAdding: boolean;
begin
  result := FCellIndex = -1;
end;

procedure TfmSingleCellEditor.AddDimensionData(AName: string;
  Dom: IXMLDOmDocument2);
var
  DimData: TDimData;
begin
  DimData := TDimData.Create;
  DimData.Members := Dom;
  FDimensionsData.AddObject(AName, DimData);
end;

procedure TfmSingleCellEditor.ClearDimData;
begin
  while FDimensionsData.Count > 0 do
  begin
    TDimData(FDimensionsData.Objects[0]).Free;
    FDimensionsData.Delete(0);
  end;
end;

procedure TfmSingleCellEditor.ClearSelections;
begin
  FSelectedCube := nil;
  FSelectedMeasure := nil;
  FSelectedDimension := nil;
  FSelectedHierarchy := nil;
end;

procedure TfmSingleCellEditor.OnMemberNodeCheck(Sender: TObject);
var
  Node: TBasicCheckTreeNode;
begin
  if DimTreeFocused then
  begin
    Node := DimTree.SelectedNodeAtLevel[ntHierarchy];
    if not Node.Checked then
      Node.Checked := true
    else
      OnDimNodeCheck(Node);
  end
  else
  begin
    Node := TBasicCheckTreeNode(ParamTree.Selected);
    if not Node.Checked then
      Node.Checked := true
    else
      CheckForEmptiness(Node.Text + ' (параметр)', true);
  end;
end;

{приводим к привычному для нас формату отображения измерения и его иерархии,
вне зависимости от типа сервера (2000/2005)}
function GetObjDescr(DimName, HierName: string): string;
begin
  if (Pos(snSemanticsSeparator, DimName) > 0) then
    DimName := StringReplace(DimName, snSemanticsSeparator, '.', []);
  if (HierName <> '') then
    HierName := '.' + HierName;
  result := DimName + HierName;
end;

procedure TfmSingleCellEditor.OnDimNodeCheck(Sender: TObject);
var
  AName: string;
begin
  AName := GetObjDescr(SelectedDimension.Name, SelectedHierarchy.Name);
  CheckForEmptiness(AName, TBasicCheckTreeNode(Sender).Checked);
end;

procedure TfmSingleCellEditor.LoadParamList;
begin
  ParamTree.Items.BeginUpdate;
  try
    ClearParamsData;
    if Assigned(FPlaningSheet.TaskContext) then
      LoadTaskParams
    else
      LoadSheetParams;
  finally
    ParamTree.Items.EndUpdate;
    ParamTree.UpdateStateImages;
  end;
end;

function TfmSingleCellEditor.GetDimAndHierByFullName(AName: string;
  out Dim: TDimension; out Hier: THierarchy): boolean;
var
  DimName, HierName: string;
begin
  result := false;
  if AName = '' then
    exit;
  HierName := AName;
  DimName := CutPart(HierName, '.');
  Dim := Catalog.Dimensions.Find(DimName, Catalog.PrimaryProvider);
  if not Assigned(Dim) then
    exit;
  result := true;
  Hier := Dim.GetHierarchy(HierName);
end;

procedure TfmSingleCellEditor.CheckForEmptiness(AName: string;
  Checked: boolean);
var
  Index: integer;
begin
  if Checked then
  begin
    if not Assigned(MemTree.MembersDOM) then
      exit;
    if not Assigned(MemTree.MembersDOM.SelectSingleNode(
      'function_result/Members//Member[@checked="true"]')) then
      EmptyFilters.Add(AName)
    else
    begin
      Index := EmptyFilters.IndexOf(AName);
      if Index > -1 then
        EmptyFilters.Delete(Index);
    end;
  end
  else
  begin
    Index := EmptyFilters.IndexOf(AName);
    if Index > -1 then
      EmptyFilters.Delete(Index);
  end;
end;

procedure TfmSingleCellEditor.CacheTaskParamValue(Param: TTaskParam;
  var ParamData: TParamData);
var
  Dim: TDimension;
  Hier: THierarchy;
  ValueDom: IXMLDomDocument2;
begin
  if not GetDimAndHierByFullName(Param.Dimension, Dim, Hier) then
    exit;
  ParamData := TParamData.Create;
  with ParamData do
  begin
    Name := Param.Name;
    Comment := Param.Comment;
    Dimension := Param.Dimension;
    Id := Param.ID;
    MultiSelect := Param.AllowMultiSelect;
    IsInherited := Param.IsInherited;
    Members := FPlaningSheet.DataProvider.GetMemberList(
      Hier.ProviderId,
      '' , Dim.Name, Hier.Name,
      Hier.Levels.ToString, Hier.MemberProperties.GetCommaList);
    if not Assigned(Members) then
      exit;
    if Assigned(Members.selectSingleNode('Exception')) then
      exit;
    GetDomDocument(ValueDom);
    ValueDom.loadXML(Param.Values);
    CopyMembersState(ValueDom, Members, nil);
  end;
end;

function TfmSingleCellEditor.CheckForDuplicates(
  out ErrText: string): boolean;
var
  i, j: integer;
  ParamData1, ParamData2: TParamData;
  FullName, DimName, HierName: string;
  Node: TBasicCheckTreeNode;
begin
  result := false;
  {проверять имеет смысл только список параметров, т.к. внутри дерева
    измерений дублирование исключено по определению}
  for i := 0 to ParamTree.Items.Count - 1 do
    if TBasicCheckTreeNode(ParamTree.Items[i]).Checked then
    begin
      ParamData1 := TParamData(ParamTree.Items[i].Data);
      for j := i + 1 to ParamTree.Items.Count - 1 do
        if TBasicCheckTreeNode(ParamTree.Items[j]).Checked then
        begin
          ParamData2 := TParamData(ParamTree.Items[j].Data);
          if ParamData1.Dimension = ParamData2.Dimension then
          begin
            if not result then
            begin
              result := true;
              ErrText := 'Несколько выбранных фильтров основаны на одном и ' +
                'том же измерении:'#10' - параметр "' + ParamData1.Name + '";';
            end;
            AddTail(ErrText, #10' - параметр "' + ParamData2.Name + '";');
          end;
        end;
      for j := 0 to FDimensionsData.Count - 1 do
      begin
        FullName := StringReplace(FDimensionsData[j], snBucks, '.', []);
        if Pos('.', FullName) = Length(FullName) then
          Delete(FullName, Length(FullName), 1);
        HierName := FullName;
        DimName := CutPart(HierName, '.');
        Node := DimTree.FindNodeBy2Names(DimName,
          IIF(HierName = '', DefaultHierarchyName, HierName));
        if Assigned(Node) then
          if Node.Checked then
            if ParamData1.Dimension = FullName then
            begin
              if not result then
              begin
                result := true;
                ErrText := 'Несколько выбранных фильтров основаны на одном и ' +
                  'том же измерении:'#10' - параметр "' + ParamData1.Name + '";';
              end;
              AddTail(ErrText, #10' - измерение "' +FullName + '";');
            end;
      end;
    end;
  if result then
  begin
    ErrText[Length(ErrText)] := '.';
    ErrText := ErrText + #10'Отключите все указанные фильтры кроме одного.';
  end;
end;

function TfmSingleCellEditor.CheckForMultiSelection(
  out ErrText: string): boolean;
const
  XPath = 'function_result/Members//Member[@checked="true"]';
var
  i: integer;
  ParamData: TParamData;
begin
  result := false;
  for i := 0 to ParamTree.Items.Count - 1 do
    if TBasicCheckTreeNode(ParamTree.Items[i]).Checked then
    begin
      ParamData := TParamData(ParamTree.Items[i].Data);
      if not ParamData.MultiSelect then
        if ParamData.Members.selectNodes(XPath).length > 1 then
        begin
          if not result then
          begin
            result := true;
            ErrText := 'Следующие фильтры не поддерживают множественный выбор:';
          end;
          AddTail(ErrText, #10' - параметр "' + ParamData.Name + '";');
        end;
    end;
  if result then
  begin
    ErrText[Length(ErrText)] := '.';
  end;
end;

procedure TfmSingleCellEditor.CreateParamFilters(
  Cell: TSheetSingleCellInterface);
var
  Filter: TSheetFilterInterface;
  i, NewPid: integer;
  TaskParam: TTaskParam;
  SheetParam: TParamInterface;
  Dim: TDimension;
  Hier: THierarchy;
  ParamData: TParamData;
  WorkWithinContext: boolean;
begin
  WorkWithinContext := Assigned(FPlaningSheet.TaskContext);
  if WorkWithinContext then
    if FPlaningSheet.TaskContext.GetTaskParams.IsReadOnly then
    begin
      ShowWarning('Лист принадлежит задаче и открыт в режиме "только для чтения".'#10 +
        'Любые возможные изменения коллекции параметров не будут сохранены');
    end;
  for i := 0 to ParamTree.Items.Count - 1 do
    if TBasicCheckTreeNode(ParamTree.Items[i]).Checked then
    begin
      ParamData := TParamData(ParamTree.Items[i].Data);
      GetDimAndHierByFullName(ParamData.Dimension, Dim, Hier);
      Filter := Cell.Filters.Append;
      Filter.ProviderId := Cell.ProviderId;
      Filter.Dimension := Dim.Name;
      if Assigned(Hier) then
        Filter.Hierarchy := Hier.Name
      else
        Filter.Hierarchy := '';

      SheetParam := FPlaningSheet.Params.ParamByName(ParamData.Name);
      if Assigned(SheetParam) then
        SheetParam.SetLink(Filter)
      else
      begin
        NewPid := FPlaningSheet.GetPID(ParamData.Name);
        FPlaningSheet.Params.AddParam(Filter);
        Filter.Param.PID := NewPid;
      end;

      SetCheckedIndication(ParamData.Members);
      FilterMembersDomEx(ParamData.Members);
      CutAllInvisible(ParamData.Members, true);
      Filter.Param.Members.loadXML(ParamData.Members.xml);
      Filter.IsPartial := false;

      Filter.Param.Name := ParamData.Name;
      Filter.Param.Comment := ParamData.Comment;
      Filter.Param.MultiSelect := ParamData.MultiSelect;
      Filter.Param.ID := ParamData.ID;
      Filter.Param.IsInherited := ParamData.IsInherited;

      if WorkWithinContext then
      begin
        if FPlaningSheet.TaskContext.GetTaskParams.IsReadOnly then
          continue;
        TaskParam := Params.ParamByName(ParamTree.Items[i].Text);
        if not Assigned(TaskParam) then
          Continue;
        if not TaskParam.IsInherited then
          TaskParam.Values := Filter.Members.xml;
      end;

      ParamData.Free;
    end;
end;

procedure TfmSingleCellEditor.CreateDimFilters(
  Cell: TSheetSingleCellInterface);
var
  Filter: TSheetFilterInterface;
  tmpStr, DimName, HierName: string;
  Node: TBasicCheckTreeNode;
begin
  while FDimensionsData.Count > 0 do
  begin
    tmpStr := FDimensionsData.Strings[0];
    DimName := CutPart(tmpStr, snBucks);
    HierName := tmpStr;
    Node := DimTree.FindNodeBy2Names(DimName,
      IIF(HierName = '', DefaultHierarchyName, HierName));
    if Assigned(Node) then
      if Node.Checked then
      begin
        Filter := Cell.Filters.Append;
        Filter.Dimension := DimName;
        Filter.Hierarchy := HierName;
        Filter.ProviderId := Cell.ProviderId;
        Filter.Members := TDimData(FDimensionsData.Objects[0]).Members;
        SetCheckedIndication(Filter.Members);
        CopyInfluences(Filter.Members);
        CutAllInvisible(Filter.Members, true);
        Filter.IsPartial := false;
      end;
    TDimData(FDimensionsData.Objects[0]).Free;
    FDimensionsData.Delete(0);
  end;
end;

procedure TfmSingleCellEditor.CacheSheetParamValue(Param: TParamInterface;
  var ParamData: TParamData);
var
  Dim: TDimension;
  Hier: THierarchy;
begin
  if not GetDimAndHierByFullName(Param.Dimension, Dim, Hier) then
    exit;
  ParamData := TParamData.Create;
  with ParamData do
  begin
    Name := Param.Name;
    Comment := Param.Comment;
    Id := Param.ID;
    Dimension := Param.Dimension;
    MultiSelect := Param.MultiSelect;
    IsInherited := Param.IsInherited;
    Members := FPlaningSheet.DataProvider.GetMemberList(
      Hier.ProviderId,
      '' , Dim.Name, Hier.Name,
      Hier.Levels.ToString, Hier.MemberProperties.GetCommaList);
    if not Assigned(Members) then
      exit;
    if Assigned(Members.selectSingleNode('Exception')) then
      exit;
    CopyMembersState(Param.Members, Members, nil);
  end;
end;

procedure TfmSingleCellEditor.LoadSheetParams;
var
  i: integer;
  Param: TParamInterface;
  Dim: TDimension;
  Hier: THierarchy;
  ParamData: TParamData;
begin
  if not Assigned(FPlaningSheet.Params) then
    exit;
  for i := 0 to FPlaningSheet.Params.Count - 1 do
  begin
    Param := FPlaningSheet.Params[i];
    if not GetDimAndHierByFullName(Param.Dimension, Dim, Hier) then
      Continue;
    {если такое измерение отсутствует в каталоге, то пропускаем}
    if not Assigned(Hier) then
      Continue;
    {при неободимости фильтруем по заданному кубу}
    if Assigned(SelectedCube) then
      if not SelectedCube.DimAndHierInCube(Dim.Name, Hier.Name) then
        Continue;
    if not Assigned(ParamTree.FindNodeByName(Param.Name)) then
      with TBasicCheckTreeNode(ParamTree.Items.Add(nil, Param.Name)) do
      begin
        Checked := false;
        {сразу же на загрузке кэшируем значения параметров для дальнейшего
          редактирования}
        CacheSheetParamValue(Param, ParamData);
        Data := ParamData;
      end;
  end;
end;

procedure TfmSingleCellEditor.LoadTaskParams;
var
  i: integer;
  Param: TTaskParam;
  Dim: TDimension;
  Hier: THierarchy;
  ParamData: TParamData;
begin
  Params := FPlaningSheet.TaskParams;
  if not Assigned(Params) then
    exit;
  for i := 0 to Params.Count - 1 do
  begin
    Param := Params[i];
    if not GetDimAndHierByFullName(Param.Dimension, Dim, Hier) then
      Continue;
    {если такое измерение отсутствует в каталоге, то пропускаем}
    if not Assigned(Hier) then
      Continue;
    {при неободимости фильтруем по заданному кубу}
    if Assigned(SelectedCube) then
      if not SelectedCube.DimAndHierInCube(Dim.Name, Hier.Name) then
        Continue;
    with TBasicCheckTreeNode(ParamTree.Items.Add(nil, Param.Name)) do
    begin
      Checked := false;
      {сразу же на загрузке кэшируем значения параметров для дальнейшего
        редактирования}
      CacheTaskParamValue(Param, ParamData);
      Data := ParamData;
    end;
  end;
end;

procedure TfmSingleCellEditor.ClearParamsData;
var
  i: integer;
begin
  for i := ParamTree.Items.Count - 1 downto 0 do
  begin
    TParamData(ParamTree.Items[i].Data).Free;
    ParamTree.Items[i].Delete;
  end;
end;

{ TDimData }

constructor TDimData.Create;
begin
  inherited;
//  GetDomDocument(Members);
end;

destructor TDimData.Destroy;
begin
  KillDomDocument(Members);
  inherited;
end;

procedure TfmSingleCellEditor.FormDestroy(Sender: TObject);
begin
  FLoading := true; // чтобы не срабатывали обработчики
  FreeStringList(FDimensionsData);
  FreeStringList(EmptyFilters);
end;

procedure TfmSingleCellEditor.cmbTotalFormatChange(Sender: TObject);
begin
  seDigits.Enabled :=
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtCurrency, fmtPercent, fmtNumber]);
  cmbTotalMultiplier.Enabled := (TMeasureFormat(cmbTotalFormat.ItemIndex) = fmtCurrency);
end;

procedure TfmSingleCellEditor.btnSelectMeasureClick(Sender: TObject);
var
  Measure: TMeasure;
  Cube: TCube;
  Index: integer;
  NewName: string;
begin
  EmptyFilters.Clear;
  if SelectMeasure(Catalog, cbResult.Checked, Measure, Cube) then
    if (Measure <> SelectedMeasure) or (Cube <> SelectedCube) then
    begin
      if (Cube <> SelectedCube) then
      begin
        FSelectedCube := Cube;
        FLoading := true;
        LoadDimensionsTree;
        LoadParamList;
        FLoading := false;
        DimTreeFocused := true;
        OnDimensionChange(nil, nil);
      end;
      FSelectedMeasure := Measure;
      lblCube.Caption := SelectedCube.Name;
      lblMeasure.Caption := SelectedMeasure.Name;
      if not FLoading then
      begin
        cmbTotalFormat.ItemIndex := Ord(SelectedMeasure.Format);
        cmbTotalFormatChange(Self);
        if SelectedMeasure.Format in [fmtCurrency, fmtPercent, fmtNumber] then
          seDigits.Value := 2;
      end;
      if eName.Text = '' then
      begin
        NewName := SelectedCube.Name + '.' + SelectedMeasure.Name;
        Index := 1;
        while Assigned(fPlaningSheet.SingleCells.FindByName(NewName)) do
        begin
          NewName := SysUtils.Format('%s.%s (%d)',
            [SelectedCube.Name, SelectedMeasure.Name, Index]);
          inc(Index);
        end;
        eName.Text := NewName;
      end;
      EnableControls(1, true);
    end;
end;

procedure TfmSingleCellEditor.cbResultClick(Sender: TObject);
begin
  {Устаревший код. В старых версиях был запрет на смену формата у
    результатов расчета. Позже его убрали.}
(*  if cbResult.Checked then
  begin
    //cmbTotalFormat.Enabled := false;
    if Assigned(SelectedMeasure) then
    begin
      cmbTotalFormat.ItemIndex := Ord(SelectedMeasure.Format);
      cmbTotalFormatChange(nil);
    end;
  end
  else
    cmbTotalFormat.Enabled := true;*)
end;

procedure TfmSingleCellEditor.cbResultKeyUp(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  cbResultClick(nil);
end;

procedure TfmSingleCellEditor.btnOKClick(Sender: TObject);
var
  tmpStr: string;
  i: integer;
begin
  if EmptyFilters.Count > 0 then
  begin
    tmpStr := '';
    for i := 0 to EmptyFilters.Count - 1 do
      tmpStr := tmpStr + #10' - "' + EmptyFilters[i] + '";';
    tmpStr[Length(tmpStr)] := '.';
    ShowError('В одном или нескольких установленных фильтрах не выбрано ни одного элемента:'#10 +
      tmpStr + #10#10'Отключите фильтры или выберите элементы измерений.');
    exit;
  end;
  if CheckForDuplicates(tmpStr) then
  begin
    ShowError(tmpStr);
    exit;
  end;
  if CheckForMultiSelection(tmpStr) then
  begin
    ShowError(tmpStr);
    exit;
  end;
  ModalResult := mrOk;
end;

procedure TfmSingleCellEditor.DimTreeEnter(Sender: TObject);
begin
  DimTreeFocused := true;
  DimTree.HideSelection := false;
  ParamTree.HideSelection := true;
  OnDimensionChange(nil, nil);
end;

procedure TfmSingleCellEditor.ParamTreeInfoTip(Sender: TObject;
  Item: TTreeNode; var InfoTip: String);
var
  ParamData: TParamData;
begin
  ParamData := TParamData(Item.Data);
  if not Assigned(ParamData) then
    exit;
  InfoTip := Format('Наименование: %s;'#10'Измерение: %s;'#10 +
    'От родительской задачи: %s;'#10'Множественный выбор: %s;'#10 +
    'Комментарий: %s.',
    [ParamData.Name, ParamData.Dimension, IIF(ParamData.IsInherited, 'да', 'нет'),
    IIF(ParamData.MultiSelect, 'разрешен', 'запрещен'),
    IIF(ParamData.Comment <> '', ParamData.Comment, 'отсутствует')]);
end;

procedure TfmSingleCellEditor.ParamTreeChange(Sender: TObject;
  Node: TTreeNode);
var
  ParamData: TParamData;
begin
  if FLoading then
    exit;
  if not Assigned(Node) then
    Node := TBasicCheckTreeNode(ParamTree.Selected);
  if not Assigned(Node) then
    exit;
  ParamData := TParamData(Node.Data);
  if not Assigned(ParamData) then
    exit;

  GetDimAndHierByFullName(ParamData.Dimension, FSelectedDimension, FSelectedHierarchy);
  if Assigned(FSelectedDimension) and Assigned(FSelectedHierarchy) then
    sbDimension.Panels[0].Text := Format(' Измерение: [%s].[%s] (параметр "%s")',
      [FSelectedDimension.Name, IIF(FSelectedHierarchy.Name = '',
      DefaultHierarchyName, FSelectedHierarchy.Name), ParamData.Name])
    else
      sbDimension.Panels[0].Text := '';

  MemTree.Load(ParamData.Members, SelectedHierarchy.Levels.ToString, SelectedHierarchy.CodeToShow);
  MemTree.MembersReadOnly := ParamData.IsInherited;
  DimTreeFocused := false;
end;

procedure TfmSingleCellEditor.ParamTreeChanging(Sender: TObject;
  Node: TTreeNode; var AllowChange: Boolean);
begin
  if FLoading then
    exit;
  MemTree.SyncronizeSearchWithTree;
end;

procedure TfmSingleCellEditor.ParamTreeClick(Sender: TObject);
var
  Pt: TPoint;
  Node: TBasicCheckTreeNode;
  AName: string;
begin
  GetCursorPos(Pt);
  Pt := ParamTree.ScreenToClient(Pt);
  Node := TBasicCheckTreeNode(ParamTree.GetNodeAt(Pt.x, Pt.y));
  if not Assigned(Node) then
    exit;
  Node.Selected := true;
  AName := Node.Text +  ' (параметр)';
  CheckForEmptiness(AName, Node.Checked);
end;

procedure TfmSingleCellEditor.ParamTreeEnter(Sender: TObject);
begin
  if ParamTree.Items.Count = 0 then
    exit;
  DimTreeFocused := false;
  DimTree.HideSelection := true;
  ParamTree.HideSelection := false;
  ParamTreeChange(nil, nil);
end;

procedure TfmSingleCellEditor.tsFiltersShow(Sender: TObject);
begin
  DimTree.SetFocus;
end;

end.







