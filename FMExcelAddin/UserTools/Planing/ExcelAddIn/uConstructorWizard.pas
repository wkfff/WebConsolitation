{
  Мастер конструирования листа.
}

unit uConstructorWizard;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, ComCtrls, fmWizardHeader, StdCtrls, MSXML2_TLB, uXMLUtils, ImgList,
  uFMExcelAddInConst, StdVCL, AxCtrls,
  PlaningProvider_TLB, PlaningTools_TLB, ComObj,
  uXMLCatalog, AddinMeasuresTree, AddinDimensionsTree,
  AddinMembersTree, Spin, Buttons, uFMAddinGeneralUtils,
  uFMAddinXMLUtils, uLevelSelector, uOfficeUtils,
  uSheetObjectModel, uGlobalPlaningConst, uFMAddinExcelUtils, uStringsEditor, uMPSelector;

type
  {режимы работы мастера - ВСЕ возможные варианты}
  TWizardRunMode = (wrmNone, wrmStandart, wrmAddColumn, wrmAddRow, wrmAddFilter,
    wrmAddMeasureTotal, wrmAddFreeTotal, wrmAddResultTotal, wrmAddConstTotal, 
    wrmEditColumn, wrmEditRow, wrmEditFilter, wrmEditTotal);
  TWizardMovement = (wmNext, wmBack);
  TWizardEvent = (weAdd, weEdit, weDel);

  TfrmConstructorWizard = class(TForm)
    pControlGroup: TPanel;
    bBottomBevel: TBevel;
    whHeader: TfmWizardHeader;
    pcPages: TPageControl;
    btNext: TButton;
    btBack: TButton;
    btDone: TButton;
    btCancel: TButton;
    tsElementType: TTabSheet;
    tsNewOrEdit: TTabSheet;
    tsTotalType: TTabSheet;
    tsTotalProperties: TTabSheet;
    tsMeasureTotal: TTabSheet;
    tsDimChoise: TTabSheet;
    tsLevelChoise: TTabSheet;
    tsMemberChoise: TTabSheet;
    tsViewAll: TTabSheet;
    rbTotals: TRadioButton;
    rbRows: TRadioButton;
    rbColumns: TRadioButton;
    rbFilters: TRadioButton;
    rbNew: TRadioButton;
    rbModify: TRadioButton;
    rbFreeTotal: TRadioButton;
    rbMeasureTotal: TRadioButton;
    Label1: TLabel;
    edCaption: TEdit;
    Label2: TLabel;
    Label3: TLabel;
    l1: TLabel;
    l3: TLabel;
    l4: TLabel;
    l2: TLabel;
    l5: TLabel;
    l6: TLabel;
    rbResultTotal: TRadioButton;
    l7: TLabel;
    l9: TLabel;
    l10: TLabel;
    l11: TLabel;
    l12: TLabel;
    l8: TLabel;
    ilImages: TImageList;
    Label6: TLabel;
    lwViewAll: TListView;
    btShowCurrentObj: TButton;
    btShowAllObj: TButton;
    lvExistsElements: TListView;
    tsFilterScope: TTabSheet;
    rbCommonFilter: TRadioButton;
    rbPartialFilter: TRadioButton;
    lvFilterScope: TListView;
    gbTotalDataOptions: TGroupBox;
    cbTotalIgnoreColumns: TCheckBox;
    cbTotalGrandSummaryDataOnly: TCheckBox;
    MeasuresTree: TAddinMeasuresTree;
    DimensionsTree: TAddinDimensionsTree;
    gbTotalFormat: TGroupBox;
    cmbTotalFormat: TComboBox;
    Label5: TLabel;
    Label7: TLabel;
    seDigits: TSpinEdit;
    btUpElement: TBitBtn;
    btDownElement: TBitBtn;
    btRemoveElement: TBitBtn;
    Label8: TLabel;
    cbBreackHierarchy: TCheckBox;
    cbHideDataMembers: TCheckBox;
    cbSummariesByVisible: TCheckBox;
    pMembers: TPanel;
    MembersTree: TAddinMembersTree;
    labelMaximize: TLabel;
    lblTotalFunction: TLabel;
    cmbTotalFunction: TComboBox;
    lbEmptyValueSymbol: TLabel;
    edEmptyValueSymbol: TEdit;
    rbConstTotal: TRadioButton;
    l14: TLabel;
    btnCopyTotal: TBitBtn;
    tsConstChoise: TTabSheet;
    Label4: TLabel;
    lvConsts: TListView;
    MPSelector: TMPSelector;
    procedure btNextClick(Sender: TObject);
    procedure btBackClick(Sender: TObject);
    procedure tsElementTypeShow(Sender: TObject);
    procedure tsNewOrEditShow(Sender: TObject);
    procedure tsTotalTypeShow(Sender: TObject);
    procedure tsTotalPropertiesShow(Sender: TObject);
    procedure tsConstChoiseShow(Sender: TObject);
    procedure tsMeasureTotalShow(Sender: TObject);
    procedure tsDimChoiseShow(Sender: TObject);
    procedure tsLevelChoiseShow(Sender: TObject);
    procedure tsMemberChoiseShow(Sender: TObject);
    procedure tsViewAllShow(Sender: TObject);
    procedure btCancelClick(Sender: TObject);
    procedure edCaptionChange(Sender: TObject);
    procedure btShowCurrentObjClick(Sender: TObject);
    procedure btShowAllObjClick(Sender: TObject);
    procedure btDoneClick(Sender: TObject);
    procedure btUpElementClick(Sender: TObject);
    procedure btDownElementClick(Sender: TObject);
    procedure btRemoveElementClick(Sender: TObject);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
    procedure lvExistsElementsSelectItem(Sender: TObject; Item: TListItem;
      Selected: Boolean);
    procedure tsFilterScopeShow(Sender: TObject);
    procedure lvFilterScopeSelectItem(Sender: TObject; Item: TListItem;
      Selected: Boolean);
    procedure lwLevelsMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure lvFilterScopeMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure cmbTotalFormatChange(Sender: TObject);
    procedure rbCommonFilterClick(Sender: TObject);
    procedure DimensionsTreeChange(Sender: TObject; Node: TTreeNode);
    procedure labelMaximizeClick(Sender: TObject);
    procedure MeasuresTreeChange(Sender: TObject; Node: TTreeNode);
    procedure cbSummariesByVisibleClick(Sender: TObject);
    procedure lvExistsElementsKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure btnCopyTotalClick(Sender: TObject);
    procedure lvConstsClick(Sender: TObject);
    procedure rbModifyClick(Sender: TObject);
    procedure rbNewClick(Sender: TObject);
  private
    //индекс элемента (используется при запуске мастера в режиме редактирования)
    FIndexElement: integer;
    //режимы запуска мастера
    FWizardRunMode: TWizardRunMode;

    FCurrentPage: TTabSheet;
    FFirstPage: TTabSheet;
    FLastPage: TTabSheet;

    FIsMayDone: Boolean;
    FLap: integer; //круг мастера
    FApply: Boolean; //Значок для результата вызова формы (отмена или готово)
    FMovement: TWizardMovement; //куда движемся, какую кнопку нажимали последней "вперед" или "назад"

    FXMLCatalog: TXMLCatalog;
    FSheetInterface: TSheetInterface; //метаданные листа
    FDataProvider: IPlaningProvider; //поставщик данных
    FCurrentMembersDOM: IXMLDOMDocument2; //редактируемое в данный цикл дерево
    FOldRect: TRect;
    FEventList: TStringList;
    {Ключ последнего редактируемого элемента - нужен для позиционирования
     в списке существующих элементов. Чиста для удобства}
    FLastEditedElementKey: string;

    {Форма развернута?}
    function IsFormMaximized: boolean;
    {Устанавливает надпись на "разворачивалке" мастера}
    procedure SetMaximizeLinkCaption;
    {Экстренное изменение размеров окна (только для страницы мемберсов) }
    procedure ChangeWindowState(AFullScreen: boolean);
    {Инициализация страницы мастера перед показом}
    procedure SetupPage(Caption: String; Description: String);

    {Обрабатывам один цикл мастера}
    procedure CompleteCycle;

    {методы загрузки метаданных }
    function LoadMemberProperties: Boolean;
    function LoadMembers: Boolean;
    {Заполнение списка существующих элементов}
    procedure FillExistsElements;
    {заполняем последнюю страницу}
    procedure FillResume(CurObjOnly: boolean);
    {Настройка мастера при редактировании элементов}
    function CheckSelectedExElem: boolean;

    {Проверка ввода имени}
    function CheckEnteredName: Boolean;

    procedure InitButton;

    procedure SetCurrentPage(Page: TTabSheet);
    function GetLastRequiredPage: TTabSheet;

    { Доступ к редактируемым объектам }
    function GetSelectedDimensionName: String;  //выбранное измерение
    function GetSelectedHierarchyName: String;  //выбранная иерархия
    function GetSelectedCubeName: String;       //выбранный куб
    function GetSelectedMeasureName: String;    //выбранная мера
    function GetTotalCaption: string;         //введенное имя для свободный объектов
    function GetSelectedExistElementIndex: integer; //индекс существуещего элемента, который выбран.
    function IsSelectedDimCorrect: boolean; //Может ли быть выбрано это измерение?
    function CheckMultiplySelection: boolean; // проверка на корректность выбранных элементов параметра
    procedure CheckInheritedParam; // в случае паарметра от родительской задачи - дизэйблим дерево элементов
    function IsCaptionEnteredCorrect: boolean; //Можно ли сделать такой заголовок?
    //возвращает редактируемый элемент листа
    function GetEditingSheetElement: TSheetElement;
    { Возвращает коллекцию с которой работаем}
    function GetCurCollection: TSheetCollection;

    function Get_CurObjectType: TSheetObjectType;
    function Get_CurObjectTypeStr: string;
    function Get_CurTotalType: TSheetTotalType;
    function Get_IsAdding: boolean;
    {Собирает область действия фильтра}
    function GetFilterScope: TStringList;
    procedure SetFilterScope(Filter: TSheetFilterInterface);
    procedure SetXMLCatalog(Value: TXMLCatalog);
    procedure SetSheet(Value: TSheetInterface);
    procedure InitTotalFormatBox;
    procedure InitTotalEmptySymbol;
    {проверка добавляемого/редактируемого фильтра на допустимость}
    function IsFilterCorrect(IsShowWarning: boolean): boolean;
    procedure AddCommentForHistory(EventType: TWizardEvent; SheetElement: TSheetElement);
    function GetEventList: string;
    { В связи с возможным запретом на редактирование метаданных листа в целом
      надо особо обрабатывать состояние кнопки "вперед".}
    procedure SetNextButtonEnabled(Value: boolean);
  public
    {Альтернативный конструктор. Использовать надо именно его.}
    constructor CreateEx(AOwner: TComponent; ASheet: TSheetInterface);
    destructor Destroy; override;
    {Запуск мастера в том или ином режиме}
    function RunWizard(Mode: TWizardRunMode; ID: string): boolean;
    {Редактируемый лист планирования}
    property SheetInterface: TSheetInterface read FSheetInterface write SetSheet;
    {???}
    property EventList: string read GetEventList;

    {Текущая страница мастера. Передвигаться только(!) с помощью этого свойства}
    property CurrentPage: TTabSheet read FCurrentPage write SetCurrentPage;

    {Страница, после которой можно нажимать "готово"}
    property LastRequiredPage: TTabSheet read GetLastRequiredPage;

    {Поставщик данных}
    property DataProvider: IPlaningProvider read FDataProvider write FDataProvider;
    { Можно завершать действие }
    property IsMayDone: Boolean read FIsMayDone write FIsMayDone;
    {куда движемся, какую кнопку нажимали последней "вперед" или "назад"}
    property Movement: TWizardMovement read FMovement write FMovement;
    { Объект текущей операции }
    property CurObjectType: TSheetObjectType read Get_CurObjectType;
    { Объект текущей операции строкой}
    property CurObjectTypeStr: string read Get_CurObjectTypeStr;
    { Тип показателя }
    property CurTotalType: TSheetTotalType read Get_CurTotalType;
    { Модифицируем или добавляем }
    property IsAdding: boolean read Get_IsAdding;
    {Метаданные базы}
    property XMLCatalog: TXMLCatalog read FXMLCatalog write SetXMLCatalog;

    {Очистка состояния мастера}
    procedure Clear;
  end;


implementation

{$R *.DFM}

constructor TfrmConstructorWizard.CreateEx(AOwner: TComponent;
  ASheet: TSheetInterface);
begin
  inherited Create(AOwner);
  SheetInterface := ASheet;
  FLastEditedElementKey := '';
  DimensionsTree.Images := ilImages;
  DimensionsTree.ImageIndexes[ntCube] := 0;
  DimensionsTree.ImageIndexes[ntDimension] := 2;
  DimensionsTree.ImageIndexes[ntHierarchy] := 3;
  MembersTree.Images := ilImages;
  MeasuresTree.Images := ilImages;
  FillChar(FOldRect, SizeOf(FoldRect), 0);
  FEventList := TStringList.Create;
  if (FOldRect.Left +  FOldRect.Top + FOldRect.Right + FOldRect.Bottom) = 0 then
    GetWindowRect(Self.Handle, FOldRect);
  cmbTotalFormat.ItemIndex := 0;
  cmbTotalFunction.ItemIndex := 0;
end;


destructor TfrmConstructorWizard.Destroy;
begin
  If Assigned(FEventList) then
    FreeStringList(FEventList);
  FDataProvider := nil;
  inherited Destroy;
end;

procedure TfrmConstructorWizard.SetCurrentPage(Page: TTabSheet);
begin

  if Page <> tsMemberChoise then
    ChangeWindowState(false);
  FCurrentPage := Page;
  pcPages.ActivePage := Page;

  SetMaximizeLinkCaption;
  labelMaximize.Visible := (Page = tsMemberChoise) or
                           (Page = tsNewOrEdit) or
                           (Page = tsMeasureTotal) or
                           (Page = tsDimChoise) or
                           (Page = tsFilterScope) or
                           (Page = tsViewAll);
end;

function TfrmConstructorWizard.GetLastRequiredPage: TTabSheet;
begin
  result := nil;
  case CurObjectType of
    wsoTotal:
      case CurTotalType of
        wtFree: result := tsTotalProperties;
        wtConst: result := tsConstChoise;
        wtMeasure, wtResult: result := tsMeasureTotal;
      end;
    wsoRow, wsoColumn, wsoFilter: result := tsMemberChoise;
  end;
end;

function TfrmConstructorWizard.Get_CurObjectType: TSheetObjectType;
begin
  if rbTotals.Checked then
    result := wsoTotal
  else
    if rbRows.Checked then
      result := wsoRow
    else
      if rbColumns.Checked then
        result := wsoColumn
      else
        result := wsoFilter;
end;

function TfrmConstructorWizard.Get_CurObjectTypeStr: string;
begin
  case CurObjectType of
    wsoTotal: result := 'Показатель';
    wsoRow: result := 'Компонент строк';
    wsoColumn: result := 'Компонент столбцов';
    wsoFilter: result := 'Фильтр';
  end;
end;

function TfrmConstructorWizard.Get_CurTotalType: TSheetTotalType;
begin
  if rbMeasureTotal.Checked then
    result := wtMeasure
  else
    if rbFreeTotal.Checked then
      result := wtFree
    else
      if rbResultTotal.Checked then
        result := wtResult
      else
        result := wtConst;
end;

function TfrmConstructorWizard.Get_IsAdding: boolean;
begin
  result := rbNew.Checked;
end;

{ Запуск мастера }
function TfrmConstructorWizard.RunWizard(Mode: TWizardRunMode; ID: string): boolean;

  procedure RunStandard;
  begin
    FFirstPage := tsElementType;
    FLastPage := tsViewAll;

    if pcPages.ActivePage = tsElementType then
      tsElementTypeShow(Self);
    CurrentPage := tsElementType;
  end;

  function RunEdit: boolean;
  var
    DetailText: string;
    ErrorCode: integer;
  begin
    result := false;
    if (ID = '') then
      exit;
    rbNew.Checked := false;
    rbModify.Checked := true;

    case Mode of
      wrmEditColumn, wrmEditRow:
        begin
          if Mode = wrmEditColumn then
          begin
            FIndexElement := SheetInterface.Columns.FindByID(ID);
            if FIndexElement = -1 then
              exit;
            if not SheetInterface.Columns[FIndexElement].Validate(DetailText, ErrorCode) then
              if not (ErrorCode in [ecNoLevel, ecNoSelection]) then
                exit;
            rbColumns.Checked := true;
          end
          else
          begin
            FIndexElement := SheetInterface.Rows.FindByID(ID);
            if FIndexElement = -1 then
              exit;
            if not SheetInterface.Rows[FIndexElement].Validate(DetailText, ErrorCode) then
              if not (ErrorCode in [ecNoLevel, ecNoSelection]) then
                exit;
            rbRows.Checked := true;
          end;
          FFirstPage := tsMemberChoise;
          FLastPage := tsLevelChoise;

          if pcPages.ActivePage = tsMemberChoise then
            tsMemberChoiseShow(Self);
          CurrentPage := tsMemberChoise;
          result := true;
        end;
      wrmEditFilter:
        begin
          FIndexElement := SheetInterface.Filters.FindByID(ID);
          if FIndexElement = -1 then
            exit;
          if not SheetInterface.Filters[FIndexElement].Validate(DetailText, ErrorCode) then
            if not (ErrorCode in [ecNoLevel, ecNoSelection]) then
              exit;
          rbFilters.Checked := true;
          MPSelector.Clear;
          FFirstPage := tsMemberChoise;
          FLastPage := tsLevelChoise;
          if pcPages.ActivePage = tsMemberChoise then
            tsMemberChoiseShow(Self);
          SetFilterScope(SheetInterface.Filters[FIndexElement]);
          CurrentPage := tsMemberChoise;
          resuLt := true;
        end;
      wrmEditTotal:
        begin
          FIndexElement := SheetInterface.Totals.FindByID(ID);
          if FIndexElement = -1 then
            exit;
          if not SheetInterface.Totals[FIndexElement].Validate(DetailText, ErrorCode) then
            exit;
          rbTotals.Checked := true;
          edCaption.Text := SheetInterface.Totals[FIndexElement].Caption;
          edEmptyValueSymbol.Text := SheetInterface.Totals[FIndexElement].EmptyValueSymbol;
          cbTotalIgnoreColumns.Checked := SheetInterface.Totals[FIndexElement].IsIgnoredColumnAxis;
          cbTotalGrandSummaryDataOnly.Checked := SheetInterface.Totals[FIndexElement].IsGrandTotalDataOnly;
          cbSummariesByVisible.Checked := SheetInterface.Totals[FIndexElement].SummariesByVisible;

          FFirstPage := tsTotalProperties;
          FLastPage := tsTotalProperties;

          case SheetInterface.Totals[FIndexElement].TotalType of
            wtMeasure: rbMeasureTotal.Checked := true;
            wtFree: rbFreeTotal.Checked := true;
            wtResult: rbResultTotal.Checked := true;
            wtConst: rbConstTotal.Checked := true;
          end;

          if pcPages.ActivePage = tsTotalProperties then
            tsTotalPropertiesShow(Self);
          CurrentPage := tsTotalProperties;
          result := true;
        end;
    end;
  end;

  procedure RunAdd;
  begin
    rbNew.Checked := true;
    rbModify.Checked := false;

    case Mode of
      wrmAddColumn, wrmAddRow:
        begin
          if Mode = wrmAddColumn then
            rbColumns.Checked := true
          else
            rbRows.Checked := true;
          FFirstPage := tsDimChoise;
          FLastPage := tsLevelChoise;
          if pcPages.ActivePage = tsDimChoise then
            tsDimChoiseShow(Self);
          CurrentPage := tsDimChoise;
        end;
      wrmAddFilter:
        begin
          rbFilters.Checked := true;
          rbCommonFilter.Checked := true;
          CurrentPage := tsDimChoise;
          FFirstPage := tsDimChoise;
          FLastPage := tsLevelChoise;

          if pcPages.ActivePage = tsDimChoise then
            tsDimChoiseShow(Self);
          CurrentPage := tsDimChoise;
        end;
      wrmAddMeasureTotal, wrmAddFreeTotal, wrmAddResultTotal, wrmAddConstTotal:
        begin
          rbTotals.Checked := true;
          case Mode of
            wrmAddMeasureTotal, wrmAddResultTotal:
              begin
                FFirstPage := tsMeasureTotal;
                FLastPage  := tsTotalProperties;
                if Mode = wrmAddMeasureTotal then
                  rbMeasureTotal.Checked := true
                else
                  rbResultTotal.Checked := true;
                if pcPages.ActivePage = tsMeasureTotal then
                  tsMeasureTotalShow(Self);
                CurrentPage := tsMeasureTotal;
              end;
            wrmAddFreeTotal:
              begin
                rbFreeTotal.Checked := true;
                FFirstPage := tsTotalProperties;
                FLastPage  := tsTotalProperties;
                if pcPages.ActivePage = tsTotalProperties then
                  tsTotalPropertiesShow(Self);
                CurrentPage := tsTotalProperties;
              end;
            wrmAddConstTotal:
              begin
                rbConstTotal.Checked := true;
                FFirstPage := tsConstChoise;
                FLastPage  := tsTotalProperties;
                CurrentPage := tsConstChoise;
                if (pcPages.ActivePage = tsConstChoise) then
                  tsConstChoiseShow(Self);
              end;
          end;
        end;
    end;
  end;

begin
  result := false;

  FWizardRunMode := Mode;
  IsMayDone := false;
  Movement := wmNext;
  FCurrentMembersDOM := nil;
  MembersTree.PageIndex := 0;

  Clear;
  if not Assigned(FXMLCatalog) then
    exit;

  FApply := false;
  FLap := 1;

  case Mode of
    // стандартный режим запуска мастера
    wrmStandart: RunStandard;
    // запуск мастера в режиме редактирования
    wrmEditColumn, wrmEditRow, wrmEditFilter, wrmEditTotal:
      if not RunEdit then
      begin
        ShowError(ermEditNotPossible);
        exit;
      end;
    // запуск мастера в режиме добавления
    wrmAddColumn, wrmAddRow, wrmAddFilter, wrmAddMeasureTotal,
    wrmAddFreeTotal, wrmAddResultTotal, wrmAddConstTotal: RunAdd;
  end;
  FEventList.Clear;

  Self.ShowModal;
  Application.ProcessMessages;

  //Последний штрих - проставим разрезность показателям-мерам
  if FApply then
    SheetInterface.SetUpMeasuresPosition;

  result := FApply;
end;

{ Инициализация страницы мастера перед показом }
procedure TfrmConstructorWizard.SetupPage(Caption: String; Description: String);
begin
  {(!) Здесь pcPages.ActivePage - страница с которой переходим,
      CurrentPage - страница на которую переходим}
  if (Movement = wmNext) then
  begin
    if not IsMayDone then
      IsMayDone := ((FLap > 1) or (CurrentPage = LastRequiredPage));
  end
  else
    if IsMayDone then
      IsMayDone := ((FLap > 1) or (pcPages.ActivePage <> LastRequiredPage));

  //настраиваем командные кнопки
  case FWizardRunMode of
    wrmStandart: SetNextButtonEnabled(true);
    wrmAddColumn..wrmAddConstTotal:
      SetNextButtonEnabled((CurrentPage <> FLastPage) and (CurrentPage <> tsMeasureTotal));
    wrmEditColumn..wrmEditTotal: SetNextButtonEnabled(CurrentPage <> FLastPage);
  end;

  btBack.Enabled := (CurrentPage <> FFirstPage);

  if ((CurrentPage = tsViewAll) and (FWizardRunMode = wrmStandart)) then
    btDone.Enabled := true
  else
    btDone.Enabled := IsMayDone or (CurTotalType = wtConst);


  //настраиваем заголовки
  whHeader.Captions[0] := Caption;
  whHeader.Comments[0] := Description;
  whHeader.Repaint;
end;

{Обрабатываем один цикл мастера}
procedure TfrmConstructorWizard.CompleteCycle;

  procedure UpdateLevels(AxisElement: TSheetAxisElementInterface);
  var
    i: integer;
    tmpDom: IXMLDOMDocument2;
    LevelNode: IXMLDOMNode;
    LevelName: string;
    SheetLevel: TSheetLevelInterface;
  begin
    {сохраним старые уровни}
    if not IsAdding then
    begin
      GetDomDocument(tmpDom);
      tmpDom.documentElement := tmpDom.createElement('levels');
      AxisElement.Levels.WriteToXML(tmpDom.documentElement);
    end;
    {}
    AxisElement.Levels.Clear;
    for i := 0 to MembersTree.LevelCount - 1 do
      if (MembersTree.LevelStates[i] <> lsDisabled) then
      begin
        SheetLevel := AxisElement.Levels.Append;
        LevelName := MembersTree.LevelNames[i];
        SheetLevel.Name := MembersTree.LevelNames[i];
        if not IsAdding then
        begin
          LevelNode := tmpDom.selectSingleNode('//level[name="' + LevelName + '"]');
          if Assigned(LevelNode) then
            SheetLevel.ReadFromXML(LevelNode);
        end;
        if AxisElement.UseSummariesForLevels then
          SheetLevel.SummaryOptions.Copy(AxisElement.SummaryOptions);
        if AxisElement.HideDataMembers then
          SheetLevel.DMDeployment := idNone
        else
          SheetLevel.DMDeployment := idTop;
      end;
    if not IsAdding then
      killDomDocument(tmpDom);
  end;

  procedure CheckParamAdding(SheetDimension: TSheetDimension);
  var
    TaskParam: TTaskParam;
    SheetParam: TParamInterface;
    NewPID: integer;
  begin
    TaskParam := DimensionsTree.Parameter;
    if Assigned(TaskParam) then
    begin
      SheetParam := SheetDimension.SheetInterface.Params.ParamByName(TaskParam.Name);
      if Assigned(SheetParam) then
        SheetParam.SetLink(SheetDimension)
      else
      begin
        NewPID := SheetInterface.GetPID(TaskParam.Name);
        SheetParam := SheetInterface.Params.AddParam(SheetDimension);
        SheetParam.Name := TaskParam.Name;
        SheetParam.Comment := TaskParam.Comment;
        SheetParam.MultiSelect := TaskParam.AllowMultiSelect;
        SheetParam.ID := TaskParam.ID;
        SheetParam.PID := NewPID;
        SheetParam.IsInherited := TaskParam.IsInherited;
      end;
    end
    else
    begin
      SheetParam := DimensionsTree.SheetParam;
      if Assigned(SheetParam) then
        SheetParam.SetLink(SheetDimension);
    end;
  end;

  procedure CheckParamUpdating(SheetDimension: TSheetDimension);
  var
    TaskParam: TTaskParam;
  begin
    if not SheetDimension.IsParam then
      exit;
    if not Assigned(SheetInterface.TaskContext) then
      exit;
    if SheetInterface.TaskContext.GetTaskParams.IsReadOnly then
      exit;
    TaskParam := SheetInterface.TaskContext.GetTaskParams.ParamByName(SheetDimension.Param.Name);
    if not TaskParam.IsInherited then
      TaskParam.Values := SheetDimension.Members.xml;
  end;

var
  Total: TSheetTotalInterface;
  AxisElement: TSheetAxisElementInterface;
  AxisCollection: TSheetAxisCollectionInterface;
  Filter: TSheetFilterInterface;
  CurInd: integer;
  OldCaption: string;
  Constant: TConstInterface;
begin
  CurInd := GetSelectedExistElementIndex;

  case CurObjectType of
    wsoTotal:
      begin
        if IsAdding then
          Total := SheetInterface.Totals.Append
        else
          Total := SheetInterface.Totals[CurInd];

        OldCaption := Total.Caption;
        Total.Caption := GetTotalCaption;
        Total.IsGrandTotalDataOnly := cbTotalGrandSummaryDataOnly.Checked;
        Total.IsIgnoredColumnAxis := cbTotalIgnoreColumns.Checked;
        Total.SummariesByVisible := cbSummariesByVisible.Checked;
        Total.CountMode := TMeasureCountMode(cmbTotalFunction.ItemIndex);

        Total.Format := TMeasureFormat(cmbTotalFormat.ItemIndex);
        Total.EmptyValueSymbol := edEmptyValueSymbol.Text;
        Total.Digits := seDigits.Value;

        Total.TotalType := CurTotalType;
        case CurTotalType of
          wtFree:;
          wtConst:
            begin
              Constant := SheetInterface.Consts.ConstByName(OldCaption);
              if Assigned(Constant) then
              begin
                // проставляем у константы свойство - расположена на листе
                Constant.IsSheetConst := true;
                Constant.Name := Total.Caption;
                Constant.SyncSheetConsts(OldCaption, Constant.Name);
              end;
            end;
          wtMeasure, wtResult:
            if IsAdding then //при редактировании куб и меру не меняем
            begin
              Total.CubeName := GetSelectedCubeName;
              Total.MeasureName := GetSelectedMeasureName;
              Total.ProviderId := MeasuresTree.Cube.ProviderId;
            end;
        end;
        if IsAdding then
          AddCommentForHistory(weAdd, Total)
        else
          AddCommentForHistory(weEdit, Total);
      end;
    wsoRow, wsoColumn:
      begin
        if CurObjectType = wsoRow then
          AxisCollection := SheetInterface.Rows
        else
          AxisCollection := SheetInterface.Columns;

        if IsAdding then
        begin
          AxisElement := AxisCollection.Append;
          AxisElement.ProviderId := DimensionsTree.Hierarchy.ProviderId;
        end
        else
          AxisElement := AxisCollection[CurInd];

        SetCheckedIndication(FCurrentMembersDOM);
        MembersTree.FilterMembersDom(FCurrentMembersDOM);
        AxisElement.IgnoreHierarchy := cbBreackHierarchy.Checked;
        AxisElement.HideDataMembers := cbHideDataMembers.Checked;

        if not AxisElement.SummaryOptions.IsEqualTo(AxisCollection.SummaryOptions) then
          AxisCollection.UseSummariesForElements := false;

        AxisElement.Dimension := GetSelectedDimensionName;
        AxisElement.Hierarchy := GetSelectedHierarchyName;
        UpdateLevels(AxisElement);
        MPSelector.Save(AxisElement.MemberProperties);
        AxisElement.MemberProperties.Reload;


        MembersTree.SyncronizeSearchWithTree;
        CutAllInvisible(FCurrentMembersDOM, true);
        AxisElement.Members := FCurrentMembersDOM;

          // проверяем на добавление параметра
        if IsAdding then
          CheckParamAdding(AxisElement);
        CheckParamUpdating(AxisElement);

        if IsAdding then
          AddCommentForHistory(weAdd, AxisElement)
        else
          AddCommentForHistory(weEdit, AxisElement);
      end;

    wsoFilter:
      begin
        if IsAdding then
        begin
          Filter := SheetInterface.Filters.Append;
          Filter.ProviderId := DimensionsTree.Hierarchy.ProviderId;
        end
        else
          Filter := SheetInterface.Filters[CurInd];

        MembersTree.SyncronizeSearchWithTree;
        SetCheckedIndication(FCurrentMembersDOM);
        {сохраним зависимости элементов}
        CopyInfluences(FCurrentMembersDOM);
        CutAllInvisible(FCurrentMembersDOM, true);

        with Filter do
        begin
          Dimension := GetSelectedDimensionName;
          Hierarchy := GetSelectedHierarchyName;
          Members := FCurrentMembersDOM;
          IsPartial := rbPartialFilter.Checked;
          Scope := GetFilterScope;
        end;  

        // проверяем на добавление параметра
        if IsAdding then
          CheckParamAdding(Filter);
        CheckParamUpdating(Filter);

        MPSelector.Save(Filter.MemberProperties);
        Filter.MemberProperties.Reload;
        if IsAdding then
          AddCommentForHistory(weAdd, Filter)
        else
          AddCommentForHistory(weEdit, Filter);
      end;
  end;

  FCurrentMembersDOM := nil;

  {Сбрасываем некоторые поля}
  edCaption.Text := '';
  edEmptyValueSymbol.Text := '';
end;


function TfrmConstructorWizard.LoadMemberProperties: boolean;
var
  CurElement: TSheetDimension;
  Hierarchy: THierarchy;
begin
  //если не загружены метаданные или не выделено измерение, то выходим.
  if not FXMLCatalog.Loaded or
    (IsAdding and not Assigned(DimensionsTree.Dimension)) then
  begin
    result := false;
    exit;
  end;
  if IsAdding then
  begin
    Hierarchy := DimensionsTree.Hierarchy;
    MPSelector.Load(Hierarchy.MemberProperties);
  end
  else
  begin
    CurElement := TSheetDimension(GetEditingSheetElement);
    MPSelector.Load(CurElement.MemberProperties);
  end;
  result := true;
end;


function TfrmConstructorWizard.LoadMembers: Boolean;

  {Первичное проставление атрибутов при первой загрузке}
  procedure FirstNormalization(Dom: IXMLDOMDocument2);
  var
    NL: IXMLDOMNodeList;
  begin
    if not Assigned(Dom) then
      exit;
    NL := Dom.selectNodes('//Member');
    FillNodeListAttribute(NL, attrChecked, 'true');
    FillNodeListAttribute(NL, attrInfluence, 0);
  end;

var
  OldMembers: IXMLDOMDocument2;
  CurIndex: integer;
  Filter: TSheetFilterInterface;
  Dimension: TDimension;
  Hierarchy: THierarchy;
  TaskParam: TTaskParam;
  ValuesFromParam: boolean;
  Element: TSheetDimension;
  ParamValues, ProviderId: string;
begin
  result := true;
  MembersTree.Clear;
  Application.ProcessMessages;
  CurIndex := GetSelectedExistElementIndex;
  OldMembers := nil;
  if not IsAdding then
  begin
    case CurObjectType of
      wsoRow:
        begin
          OldMembers := SheetInterface.Rows[CurIndex].Members;
          ProviderId := SheetInterface.Rows[CurIndex].ProviderId;
        end;
      wsoColumn:
        begin
          OldMembers := SheetInterface.Columns[CurIndex].Members;
          ProviderId := SheetInterface.Columns[CurIndex].ProviderId;
        end;
      wsoFilter:
        begin
          { определяем является ли фильтр частным, на случай если пользователь
           закончит его редактирование на этой странице}
          Filter := SheetInterface.Filters[CurIndex];
          rbPartialFilter.Checked := Filter.IsPartial;
          OldMembers := SheetInterface.Filters[CurIndex].Members;
          ProviderId := SheetInterface.Filters[CurIndex].ProviderId;
        end;
    end;
  end
  else
    ProviderId := DimensionsTree.Hierarchy.ProviderId;

  try
    Dimension := FXMLCatalog.Dimensions.Find(GetSelectedDimensionName, ProviderId);
    Hierarchy := Dimension.GetHierarchy(GetSelectedHierarchyName);

    MembersTree.LevelsReadOnly := false;//CurObjectType = wsoFilter;
    MembersTree.MayDisableLevels := CurObjectType <> wsoFilter;

    FCurrentMembersDOM := FDataProvider.GetMemberList(
      ProviderId,
      '' ,
      Dimension.Name, Hierarchy.Name,
      Hierarchy.Levels.ToString,
      Hierarchy.MemberProperties.GetCommaList);


    // проверка на эксепт
    if Assigned(FCurrentMembersDOM.selectSingleNode('Exception')) then
    begin
      result := false;
      exit;
    end;

    // проверяем на добавление параметра  - получаем контекстный параметр из компонента
    //TaskParam := nil;
    ValuesFromParam := false;
    ParamValues := '';
    if IsAdding then
    begin
      if Assigned(DimensionsTree.Parameter) then
        ParamValues := DimensionsTree.Parameter.values
      else
        if Assigned(DimensionsTree.SheetParam) then
          ParamValues := DimensionsTree.SheetParam.Members.xml
    end
    else
    begin
      Element := (GetEditingSheetElement as TSheetDimension);
      if Assigned(Element) and Element.IsParam then
      begin
        if Assigned(Element.SheetInterface.TaskContext) then
        begin
          TaskParam := Element.SheetInterface.TaskContext.GetTaskParams.ParamByName(Element.Param.Name);
          ParamValues := TaskParam.values;
        end
        else
          ParamValues := Element.Members.xml;
      end;
    end;
    if (ParamValues <> '') then
    begin
      if IsAdding then
        GetDomDocument(OldMembers);
      OldMembers.loadXML(ParamValues);
      CopyMembersState(OldMembers, FCurrentMembersDOM, nil);
      ValuesFromParam := true;
    end;

    if not ValuesFromParam then
    begin
      if Assigned(OldMembers) then
        CopyMembersState(OldMembers, FCurrentMembersDOM, nil)
      else
        FirstNormalization(FCurrentMembersDom);
    end;
  except
    FCurrentMembersDOM := nil;
    result := false;
    exit;
  end;
  MembersTree.Load(FCurrentMembersDOM, Hierarchy.Levels.ToString, Hierarchy.CodeToShow);
end;

{ заполняем последнюю страницу. Выводим то что сделали на этом цикле }
procedure TfrmConstructorWizard.FillResume(CurObjOnly: Boolean);

  procedure AddProp(PrName, PrValue: string);
  begin
    with lwViewAll.Items.Add do
    begin
      Caption := PrName;
      SubItems.Append(PrValue);
    end;
  end;

  function TotalTypeStr(tType: TSheetTotalType): string;
  begin
    case CurTotalType of
      wtFree: result := 'Свободный показатель';
      wtMeasure: result := 'Мера из куба';
      wtResult: result := 'Результат расчета';
      wtConst: result := 'Константа';
    end;
  end;

  function GetLevelsStr: string;
  var i: integer;
  begin
    with MembersTree do
      for i := 0 to MembersTree.LevelCount - 1 do
        if MembersTree.LevelStates[i] <> lsDisabled then
        begin
          AddTail(result, ', ');
          result := result + MembersTree.LevelNames[i];
        end
  end;

  procedure FillCurObject;
  var
    tmpStr: string;
    ParamName: string;
  begin
    tmpStr := IIF(IsAdding, 'Добавлен новый объект', 'Изменен существующий объект');
    AddProp('Действие', tmpStr);

    AddProp('Тип объекта', CurObjectTypeStr);
    case CurObjectType of
      wsoTotal:
        begin
          AddProp('Тип показателя', TotalTypeStr(CurTotalType));
          AddProp('Имя', GetTotalCaption);
          case CurTotalType of
            wtFree: ;//!!!AddProp('Имя', GetFreeObjectName);
            wtConst: ;
            wtMeasure, wtResult:
              begin
                AddProp('Куб-источник', GetSelectedCubeName);
                AddProp('Мера', GetSelectedMeasureName);
              end;
          end;
        end;
      wsoRow, wsoColumn, wsoFilter:
        begin
          AddProp('Измерение', GetSelectedDimensionName);

          tmpStr := GetSelectedHierarchyName;
          if tmpStr = '' then
            tmpStr := 'По умолчанию';
          AddProp('Иерархия', tmpStr);

          if CurObjectType = wsoFilter then //фильтры
            AddProp('Значение', '')//!! нет значения
          else
          begin //столбцы и строки
            AddProp('Уровни', GetLevelsStr);
          end;

          if IsAdding then
          begin
            if Assigned(DimensionsTree.Parameter) then
            begin
              ParamName := DimensionsTree.Parameter.Name;
              if DimensionsTree.Parameter.IsInherited then
                ParamName := ParamName + ' (от родительской задачи)';
              AddProp('Параметр', ParamName);
            end
            else
              if Assigned(DimensionsTree.SheetParam) then
                AddProp('Параметр', DimensionsTree.SheetParam.FullName);
          end
          else
            if (GetEditingSheetElement as TSheetDimension).IsParam then
              AddProp('Параметр', (GetEditingSheetElement as TSheetDimension).Param.FullName)
        end;
    end;
  end;

  procedure FillAllObject;
  var
    i: integer;
    tType: TSheetTotalType;
    tmpStr: string;

  begin
    with SheetInterface do
    begin
      AddProp('[ПОКАЗАТЕЛИ]', '');
      for i := 0 to Totals.Count - 1 do
      begin
        tType := Totals[i].TotalType;
        AddProp('Тип показателя', TotalTypeStr(tType));
        case tType of
          wtFree, wtConst: AddProp('Имя', Totals[i].Caption);
          wtMeasure, wtResult:
            begin
              AddProp('Куб-источник', Totals[i].CubeName);
              AddProp('Мера', Totals[i].MeasureName);
            end;
        end;
        AddProp('', ''); //пробел
      end;

      AddProp('[ЭЛЕМЕНТЫ СТОЛБЦОВ]', '');
      for i := 0 to Columns.Count - 1 do
      begin
        AddProp('Измерение', Columns[i].Dimension);
        tmpStr := Columns[i].Hierarchy;
        if tmpStr = '' then
          tmpStr := 'По умолчанию';
        AddProp('Иерархия', tmpStr);
        AddProp('Поля (уровни)', Columns[i].Levels.NamesToString);
        if Columns[i].IsParam then
          AddProp('Параметр', Columns[i].Param.FullName);
        AddProp('', ''); //пробел
      end;

      AddProp('[ЭЛЕМЕНТЫ СТРОК]', '');
      for i := 0 to Rows.Count - 1 do
      begin
        AddProp('Измерение', Rows[i].Dimension);
        tmpStr := Rows[i].Hierarchy;
        if tmpStr = '' then
          tmpStr := 'По умолчанию';
        AddProp('Иерархия', tmpStr);
        AddProp('Поля (уровни)', Rows[i].Levels.NamesToString);
        if Rows[i].IsParam then
          AddProp('Параметр', Rows[i].Param.FullName);
        AddProp('', ''); //пробел
      end;

      AddProp('[ФИЛЬТРЫ]', '');
      for i := 0 to Filters.Count - 1 do
      begin
        AddProp('Измерение', Filters[i].Dimension);
        tmpStr := Filters[i].Hierarchy;
        if tmpStr = '' then
          tmpStr := 'По умолчанию';
        AddProp('Иерархия', tmpStr);
         { DONE -oСтрижаков -cвсенепременно : Вывести текстовое описание фильтра }
        AddProp('Значение', Filters[i].Text);
        if rbCommonFilter.Checked then
          AddProp('Тип', 'Общий')
        else
          AddProp('Тип', 'Частный');
        if Filters[i].IsParam then
          AddProp('Параметр', Filters[i].Param.FullName);
        AddProp('', ''); //пробел
      end;
    end;
  end;

begin
  lwViewAll.Items.Clear;
  if CurObjOnly then
    FillCurObject
  else
    FillAllObject;

  btShowAllObj.Enabled := CurObjOnly;
  btShowCurrentObj.Enabled := not CurObjOnly;
end; //FillResume

function TfrmConstructorWizard.CheckSelectedExElem: boolean;

  {установим нужное измерение}
  procedure SetUpDimension(DName, HName, ProviderId: string);
  var
    Dimension: TDimension;
    Hierarchy: Thierarchy;
  begin
    if (DimensionsTree.IsEmpty) then
      if not DimensionsTree.Load then
      begin
        ShowError(ermDimensionsLoadFault);
        exit;
      end;
    try
      Dimension := FXMLCatalog.Dimensions.Find(DName, ProviderId);
      Hierarchy := Dimension.GetHierarchy(HName);
      DimensionsTree.SetSelection(Dimension, Hierarchy);
    except
    end;
  end;

var
  fTotal: TSheetTotalInterface;
  fAxisElem: TSheetAxisElementInterface;
  fFilter: TSheetFilterInterface;
  DetailText: string;
  ErrorCode: integer;
begin
  result := true;
  if rbModify.Checked then
    if lvExistsElements.SelCount > 0 then
    begin
      {Сделаем закладку, запомним ключ последнего отредактированного элемента,
       Ключом будем считать первое поле, всегда}
      if lvExistsElements.SelCount > 0 then
         FLastEditedElementKey := lvExistsElements.Selected.Caption
      else
        FLastEditedElementKey := '';

      if not GetEditingSheetElement.Validate(DetailText, ErrorCode) then
        if (CurObjectType <> wsoTotal) and not (ErrorCode in [ecNoLevel, ecNoSelection]) then
        begin
          ShowDetailError('Редактирование невозможно', DetailText, 'Редактирование невозможно');
          result := false;
          exit;
        end;
      case CurObjectType of
        wsoTotal:
          begin
            fTotal := SheetInterface.Totals[GetSelectedExistElementIndex];

            {Инициализируем свойства итога}
            edCaption.Text := fTotal.Caption;
            cbTotalIgnoreColumns.Checked := fTotal.IsIgnoredColumnAxis;
            cbTotalGrandSummaryDataOnly.Checked := fTotal.IsGrandTotalDataOnly;
            cbSummariesByVisible.Checked := fTotal.SummariesByVisible;

            case fTotal.TotalType of
              wtFree:
                rbFreeTotal.Checked := true;
              wtMeasure: //!!! можно бы еще и установить на нужную меру (лень)
                rbMeasureTotal.Checked := true;
              wtResult:
                rbResultTotal.Checked := true;
              wtConst:
                rbConstTotal.Checked := true;
            end;
          end;
        wsoRow:
          begin
            fAxisElem := SheetInterface.Rows[GetSelectedExistElementIndex];
            SetUpDimension(fAxisElem.Dimension, fAxisElem.Hierarchy, fAxisElem.ProviderId);
          end;
        wsoColumn:
          begin
            fAxisElem := SheetInterface.Columns[GetSelectedExistElementIndex];
            SetUpDimension(fAxisElem.Dimension, fAxisElem.Hierarchy, fAxisElem.ProviderId);
          end;
        wsoFilter:
          begin
            fFilter := SheetInterface.Filters[GetSelectedExistElementIndex];
            SetUpDimension(fFilter.Dimension, fFilter.Hierarchy, fFilter.ProviderId);
          end;
      end;
    end
    else
    begin
      ShowWarning(wrmSelectExistsElement);
      result := false;
    end;
end;

procedure TfrmConstructorWizard.InitButton;
var
  NextEnabled: boolean;
begin
  if (CurTotalType = wtConst) then
  begin
    SetNextButtonEnabled(true);
    btDone.Enabled := true;
    exit;
  end;
  NextEnabled := MeasuresTree.IsMeasureSelected;
  SetNextButtonEnabled(NextEnabled);
  if (FLap = 1) and (LastRequiredPage = tsMeasureTotal) then
    btDone.Enabled := NextEnabled;
end;

function TfrmConstructorWizard.CheckEnteredName: Boolean;
begin
  result := (Trim(edCaption.Text) <> '');

  if (FWizardRunMode = wrmStandart) then
    SetNextButtonEnabled(result);

  if FLap = 1 then
    btDone.Enabled := result;
end;

function TfrmConstructorWizard.GetSelectedDimensionName: string;
var
  Index: integer;
begin
  result := '';
  try
    case FWizardRunMode of
      wrmEditColumn: result := SheetInterface.Columns[FIndexElement].Dimension;
      wrmEditRow: result := SheetInterface.Rows[FIndexElement].Dimension;
      wrmEditFilter: result := SheetInterface.Filters[FIndexElement].Dimension;
      else
      begin
        if IsAdding then
          result := DimensionsTree.Dimension.Name
        else
        begin
          Index := GetSelectedExistElementIndex;
          if Index >= 0 then
            case CurObjectType of
              wsoColumn: result := SheetInterface.Columns[Index].Dimension;
              wsoRow: result := SheetInterface.Rows[Index].Dimension;
              wsoFilter: result := SheetInterface.Filters[Index].Dimension;
            end;
        end;
      end;
    end;
  except
  end;
end;

function TfrmConstructorWizard.GetSelectedHierarchyName: string;
var
  Index: integer;
begin
  try
    case FWizardRunMode of
      wrmEditColumn: result := SheetInterface.Columns[FIndexElement].Hierarchy;
      wrmEditRow: result :=  SheetInterface.Rows[FIndexElement].Hierarchy;
      wrmEditFilter: result := SheetInterface.Filters[FIndexElement].Hierarchy;
      else
        if IsAdding then
        begin
          if Assigned(DimensionsTree.Hierarchy) then
            result := DimensionsTree.Hierarchy.Name
        end
        else
        begin
          Index := GetSelectedExistElementIndex;
          if (Index >= 0) then
            case CurObjectType of
              wsoColumn: result :=  SheetInterface.Columns[Index].Hierarchy;
              wsoRow: result :=  SheetInterface.Rows[Index].Hierarchy;
              wsoFilter: result := SheetInterface.Filters[Index].Hierarchy;
            end;
        end;
    end;
  except
    result := '';
  end;
end;

function TfrmConstructorWizard.GetSelectedCubeName: String;
var
  ATotal: TSheetTotalInterface;
begin
  case FWizardRunMode of
    wrmEditTotal: result := SheetInterface.Totals[FIndexElement].CubeName;
    else
      begin
        if IsAdding then
          if  Assigned(MeasuresTree.Cube) then
            result := MeasuresTree.Cube.Name
          else
            result := ''
        else
        begin
          ATotal := GetEditingSheetElement as TSheetTotalInterface;
          if Assigned(ATotal) then
            result := ATotal.CubeName
          else
            result := '';
        end;
      end;
  end;
end;

function TfrmConstructorWizard.GetSelectedMeasureName: String;
var
  ATotal: TSheetTotalInterface;
begin
  case FWizardRunMode of
    wrmEditTotal: result := SheetInterface.Totals[FIndexElement].MeasureName;
    else
      begin
        if IsAdding then
        begin
          if MeasuresTree.IsMeasureSelected then
            result := MeasuresTree.Measure.Name
          else
            result := '';
        end
        else
        begin
          ATotal := GetEditingSheetElement as TSheetTotalInterface;
          if Assigned(ATotal) then
            result := ATotal.MeasureName
          else
            result := '';
        end;
      end;
  end;
end;


//введенное имя для свободный объектов
function TfrmConstructorWizard.GetTotalCaption: string;
begin
  result := edCaption.Text;
  if not CheckEnteredName then
    if (CurObjectType = wsoTotal) then
      case CurTotalType of
        wtResult, wtMeasure: result := GetSelectedMeasureName;
        wtConst: result := lvConsts.Selected.Caption;
      end;
end;

function TfrmConstructorWizard.GetSelectedExistElementIndex: integer;
begin
  case FWizardRunMode of
    wrmEditColumn..wrmEditTotal: result := FIndexElement;
    wrmAddColumn..wrmAddResultTotal: result := -1;
    else
      begin
        if lvExistsElements.SelCount > 0 then
         result := lvExistsElements.Selected.Index
        else
          result := -1;
      end;
  end;
end;

{Может ли быть выбрано это измерение?}
function TfrmConstructorWizard.IsSelectedDimCorrect: Boolean;
var
  DimName, HierName: string;
begin
  {проверим нет ли уже какого элемента по этому измерению.
  Причем проверять можно независимо от типа редактируемого компонента.
  В строках, столбцах и фильтрах }
  result := true;
  DimName := GetSelectedDimensionName;
  HierName := GetSelectedHierarchyName;
  case SheetInterface.IsDimensionUsed(DimName, HierName) of
    wsoRow:
      begin
        result := false;
        ShowWarning(Format(wrmDimInUseAlrady, ['элементов строк']));
      end;
    wsoColumn:
      begin
        result := false;
        ShowWarning(Format(wrmDimInUseAlrady, ['элементов столбцов']));
      end;
    wsoFilter:
      begin
        result := CurObjectType = wsoFilter;
        if not result then
          ShowWarning(Format(wrmDimInUseAlrady, ['фильтров']));
      end;
  end;
end;

procedure TfrmConstructorWizard.CheckInheritedParam;
var
  Element: TSheetDimension;
  TaskParam: TTaskParam;
  SheetParam: TParamInterface;
begin
  {Пояснения. Логика поменялась после реформы контекста задач (квест 14009).
    Синхронизация из листа в задачу ушла в прошлое, теперь значения параметров
    можно менять только в задачах. Локальные параметры, таким образом,
    выполняют лишь вспомогательную роль - чтобы настроенные параметры не пропали
    при откреплении от задачи или при копировании в другую. Менять значения
    запрещаем.}
  MembersTree.MembersReadOnly := false;
  if IsAdding then
  begin
    // у наследованного параметра нельзя редактировать элементы
    TaskParam := DimensionsTree.Parameter;
    if Assigned(TaskParam) and TaskParam.IsInherited then
      MembersTree.MembersReadOnly := true
    else
    begin
      SheetParam := DimensionsTree.SheetParam;
      if Assigned(SheetParam) and SheetParam.IsInherited then
        MembersTree.MembersReadOnly := true;
    end;
  end
  else
  begin
    Element := (GetEditingSheetElement as TSheetDimension);
    if not Assigned(Element) then
      exit;
    if not Element.IsParam then
      exit;
    (*// у наследованного параметра нельзя редактировать элементы
    if Element.Param.IsInherited then
      MembersTree.MembersReadOnly := true;*)
    if Element.Param.IsInherited then
    MembersTree.MembersReadOnly := true;
  end;
end;

function TfrmConstructorWizard.CheckMultiplySelection: boolean;
var
  Element: TSheetDimension;
  TaskParam: TTaskParam;
  SheetParam: TParamInterface;
begin
  result := true;
  // исходя из признака параметра запрещаем множественный выбор элементов
  if IsAdding then
  begin
    TaskParam := DimensionsTree.Parameter;
    SheetParam := DimensionsTree.SheetParam;
    if not ((Assigned(TaskParam) and not TaskParam.AllowMultiSelect) or
            (Assigned(SheetParam) and not SheetParam.MultiSelect)) then
      exit;      
    result := MembersTree.MembersDOM.selectNodes('//Member[@checked="true"]').length <= 1;
    if not result then
    begin
      ShowError(Format(wrmImpossibleMultiplySelection, [TaskParam.Name]));
      exit;
    end;
  end
  else
  begin
    Element := (GetEditingSheetElement as TSheetDimension);
    if not Assigned(Element) then
      exit;
    if not Element.IsParam then
      exit;
    if Element.Param.MultiSelect then
      exit;
    result := MembersTree.MembersDOM.selectNodes('//Member[@checked="true"]').length <= 1;
    if not result then
    begin
      ShowError(Format(wrmImpossibleMultiplySelection, [Element.Param.Name]));
      exit;
    end;
  end;
end;

{Можно ли сделать такой свободный показатель?}
function TfrmConstructorWizard.IsCaptionEnteredCorrect: Boolean;
var
  i, CurInd: integer;
  TotalName: string;

begin
  {Т.к для свободного показателя название это имя, оно должно быть уникальным
    на листе. проверим.}
  result := true;

  //если в данный момет работаем не со свободным, уникальность не требуется
  if (CurTotalType in [wtMeasure, wtResult]) then
    exit;

  CurInd := GetSelectedExistElementIndex;

  TotalName := GetTotalCaption;
  for i := 0 to SheetInterface.Totals.Count - 1 do
    if (SheetInterface.Totals[i].TotalType = wtFree) then
      if (SheetInterface.Totals[i].Caption = TotalName) and (i <> CurInd)then
      begin
        ShowWarning(wrmFreeTotalInUseAlrady);
        result := false;
      end;
end;

//begin=========================================================================

{ Корректировать каждый раз при имзенении порядка страниц или
  добавлении номеров страниц. Т.к используется PageIndex. }
procedure TfrmConstructorWizard.btNextClick(Sender: TObject);
begin
  Movement := wmNext;
  with pcPages do
    case CurrentPage.PageIndex of
      0:  //-- страница выбора компонента листа ----------------------------
        CurrentPage := tsNewOrEdit; {1}
      1:  //-- добавлять новый или редактировать имеющийся -----------------
        if CheckSelectedExElem then
          case CurObjectType of
            wsoTotal:
              if IsAdding then
                CurrentPage := tsTotalType {2}
              else
                CurrentPage := tsTotalProperties; {3}
            wsoRow, wsoColumn, wsoFilter:
              if IsAdding then
                CurrentPage := tsDimChoise {5}
              else
                CurrentPage := tsMemberChoise;   {6}
          end;
      2: //-- страница выбора типа показателя -----------------------------------
         //IsAdding - true; CurObject Type - wsoTotal
          case CurTotalType of
            wtFree:
              CurrentPage := tsTotalProperties; {3}
            wtMeasure, wtResult:
              CurrentPage := tsMeasureTotal; {4}
            wtConst:
            begin
              CurrentPage := tsConstChoise;  {10}
              tsConstChoiseShow(Self);
            end;
          end;

      3: //-- страница заголовка показателя ----------------------------------
        if IsCaptionEnteredCorrect then
          CurrentPage := tsViewAll; {9}

      4: //-- страница показателя - меры --------------------------------------
         CurrentPage := tsTotalProperties; {9}

      5: //-- выбор измерения ----------------------------------------------
        begin
          if DimensionsTree.IsEmpty then
            ShowError(ermNoDimensionSelected)
          else
            if IsSelectedDimCorrect then
              CurrentPage := tsMemberChoise; {6}
        end;
      6: //-- выбор членов измерения ---------------------------------------
          // проверка на соответствие атрибута параметра "множественный выбор" 
          // выбранным элементам редактируемого измерения
          if CheckMultiplySelection then
            if CurObjectType = wsoFilter then
              CurrentPage := tsFilterScope {8}
            else
              CurrentPage := tsLevelChoise; {7}
      7: //-- выбор мембер пропертиз измерения ---------------------------------------
        begin
            CurrentPage := tsViewAll; {9}
        end;
      8: //-- область действия фильтра--------------------------------------
        if IsFilterCorrect(true) then
          CurrentPage := tsLevelChoise; {7}
      9: //-- отчет о структуре --------------------------------------------
        begin
          CompleteCycle; //обрабатываем один цикл мастера
          inc(FLap);
          CurrentPage := tsElementType; {0}
        end;
      10: //-- страница показателя - константы --------------------------------------------
        begin
          CurrentPage := tsTotalProperties;
        end;
    end;
end;

{ Корректировать каждый раз при имзенении порядка страниц или
  добавлении номеров страниц. Т.к используется PageIndex. }
procedure TfrmConstructorWizard.btBackClick(Sender: TObject);
begin
  Movement := wmBack;
  with pcPages do
    case CurrentPage.PageIndex of
      1:  //-- добавлять новый или редактировать имеющийся -----------------
        CurrentPage := tsElementType; {0}
      2: //-- страница выбора типа показателя -----------------------------------
        CurrentPage := tsNewOrEdit; {1}
      3: //-- страница свободного показателя ------------------------------------

        case CurTotalType of
          wtFree:
            if IsAdding then
              CurrentPage := tsTotalType {2}
            else
              CurrentPage := tsNewOrEdit; {1}
          wtConst:
            if IsAdding then
              CurrentPage := tsConstChoise
            else
              CurrentPage := tsNewOrEdit; {1}
          wtMeasure, wtResult:
            if IsAdding then
              CurrentPage := tsMeasureTotal {4}
            else
              CurrentPage := tsNewOrEdit; {1}
        end;

      4: //-- страница показателя - меры ----------------------------------------
        if IsAdding then
          CurrentPage := tsTotalType {2}
        else
          CurrentPage := tsNewOrEdit; {1}

      5: //-- выбор измерения ----------------------------------------------
        CurrentPage := tsNewOrEdit; {1}
      6: //-- выбор членов измерения ---------------------------------------
        begin
          KillDomDocument(FCurrentMembersDOM);
          if IsAdding then
            CurrentPage := tsDimChoise
          else
            CurrentPage := tsNewOrEdit;
        end;
      7: //-- выбор мембер пропертиз измерения ---------------------------------------
        begin
          if CurObjectType = wsoFilter then
            CurrentPage := tsFilterScope
          else
            CurrentPage := tsMemberChoise; {6}
        end;
      8: //-- установка области действия фильтров ---------------------------
        CurrentPage := tsMemberChoise;
      9: //-- отчет о структуре --------------------------------------------
        begin
          case CurObjectType of
            wsoTotal:
              CurrentPage := tsTotalProperties; {3}
            wsoRow, wsoColumn:
              CurrentPage := tsLevelChoise; {7}
            wsoFilter:
              CurrentPage := tsLevelChoise; {7}
          end;
        end;
      10: //-- страница показателя - константы --------------------------------------------
        begin
          if IsAdding then
            CurrentPage := tsTotalType {2}
          else
            CurrentPage := tsNewOrEdit; {1}
        end;
    end;
end;


procedure TfrmConstructorWizard.FillExistsElements;

  {настраивает количество, ширину и имена колонок}
  procedure FormatColumns(Names: array of string; Widths: array of integer;
    Cnt: integer);
  var
    i: integer;
  begin
    lvExistsElements.Columns.Clear;
    for i := 0 to Cnt - 1 do
    begin
      lvExistsElements.Columns.Add;
      lvExistsElements.Columns[i].Caption := Names[i];
      lvExistsElements.Columns[i].Width := Widths[i];
    end;
  end;

  procedure DescribeObject(Descriptions: array of string;
    Cnt, ImgIndex: integer; P: Pointer);
  var
    i: integer;
  begin
    with lvExistsElements.Items.Add do
    begin
      Caption := Descriptions[0];
      ImageIndex := ImgIndex;
      Data := P;//указатель на объект - элемент метаданных листа
      for i := 1 to Cnt - 1 do
        SubItems.Append(Descriptions[i]);
    end;
  end;

var
  i, ColCount: integer;
  tmpStr: string;
  Comment: string;
  PrevSelected: integer;
begin
  if lvExistsElements.SelCount > 0 then
    PrevSelected := lvExistsElements.Selected.Index
  else
    PrevSelected := -1;
  lvExistsElements.Items.Clear;
  case CurObjectType of
    wsoTotal:
      begin
        ColCount := 3;
        FormatColumns(['Имя', 'Описание', 'Тип'], [120, 200, 140], ColCount);
        for i := 0 to SheetInterface.Totals.Count - 1 do
          with SheetInterface.Totals[i] do
          begin
            case TotalType of
              wtFree: Comment := '';
              wtMeasure, wtResult: Comment := 'Куб "' + CubeName + '"; Мера "' + MeasureName + '"';
              wtConst:
                try
                  Comment := SheetInterface.Consts.ConstByName(Caption).Comment;
                except
                  Comment := '';
                  ShowError('Константа "' + Caption +
                    '" не найдена. Некорректное состояние метаданных');
                end;
            end;
            DescribeObject([Caption, Comment, GetObjectTypeStr], ColCount,
                           1, Pointer(SheetInterface.Totals[i]));
          end;
      end;
    wsoRow:
      begin
        ColCount := 3;
        FormatColumns(['Измерение', 'Уровни', 'Параметр'], [200, 140, 200], ColCount);
        for i := 0 to SheetInterface.Rows.Count - 1 do
          with SheetInterface.Rows[i] do
          begin
            tmpStr := '';
            if IsParam then
              tmpStr := Param.FullName;
            DescribeObject([FullDimensionName2, Levels.NamesToString,
              tmpStr], ColCount, 2, Pointer(SheetInterface.Rows[i]));
          end;
      end;

    wsoColumn:
      begin
        ColCount := 3;
        FormatColumns(['Измерение', 'Уровни', 'Параметр'], [200, 140, 200], ColCount);
        for i := 0 to SheetInterface.Columns.Count - 1 do
          with SheetInterface.Columns[i] do
          begin
            tmpStr := '';
            if IsParam then
              tmpStr := Param.FullName;
            DescribeObject([FullDimensionName2, Levels.NamesToString,
              tmpStr], ColCount, 2, Pointer(SheetInterface.Columns[i]));
          end;
      end;

    wsoFilter:
      begin
        ColCount := 3;
        FormatColumns(['Измерение', 'Область действия', 'Параметр'], [200, 140, 200], ColCount);
        for i := 0 to SheetInterface.Filters.Count - 1 do
          with SheetInterface.Filters[i] do
          begin
            tmpStr := '';
            if IsParam then
              tmpStr := Param.FullName;
            if IsPartial then
              DescribeObject([FullDimensionName2, ScopeText, tmpStr],
                ColCount, 30, Pointer(SheetInterface.Filters[i]))
            else
              DescribeObject([FullDimensionName2, 'Общий фильтр', tmpStr],
                ColCount, 22, Pointer(SheetInterface.Filters[i]));
          end;
      end;
  end;
  if (PrevSelected > -1) and (PrevSelected < lvExistsElements.Items.Count) then
    lvExistsElements.Items[PrevSelected].Selected := true
  else
    PrevSelected := -1;

  rbNew.Enabled := GetCurCollection.MayBeEdited;
  btRemoveElement.Enabled := (lvExistsElements.Items.Count > 0) and rbNew.Enabled;
  btUpElement.Enabled := (lvExistsElements.Items.Count > 1) and rbNew.Enabled;
  btDownElement.Enabled := (lvExistsElements.Items.Count > 1) and rbNew.Enabled;
  btnCopyTotal.Enabled := rbNew.Enabled;

  rbModify.Enabled := (lvExistsElements.Items.Count > 0);
  if (lvExistsElements.Items.Count > 0) and (lvExistsElements.SelCount > 0)then
    lvExistsElementsSelectItem(lvExistsElements, lvExistsElements.Items[0], true);
  if PrevSelected = -1 then
    rbNew.Checked := true;
end;

//end===========================================================================

procedure TfrmConstructorWizard.tsElementTypeShow(Sender: TObject);
begin
  SetupPage(capElementType, dscElementType);

  with SheetInterface do
  begin
    rbTotals.Enabled := Totals.MayBeEdited or Totals.ElementsMayBeEdited;
    rbRows.Enabled := Rows.MayBeEdited or Rows.ElementsMayBeEdited;
    rbColumns.Enabled := Columns.MayBeEdited or Columns.ElementsMayBeEdited;
    rbFilters.Enabled := Filters.MayBeEdited or Filters.ElementsMayBeEdited;
  end;
  btNext.Enabled := rbTotals.Enabled or rbRows.Enabled or rbColumns.Enabled or
    rbFilters.Enabled;
end;

procedure TfrmConstructorWizard.tsNewOrEditShow(Sender: TObject);
var
  Cap, Descr: string;
  CurItem: TListItem;
begin
  {!!! Времянка }

  case CurObjectType of
    wsoTotal:
      begin
        Cap := Format(capNewOrEdit, ['набора показателей']);
        Descr := Format(dscNewOrEdit, ['новый показатель', 'показателей']);
        rbNew.Caption := 'Добавить новый показатель';
      end;
    wsoRow:
      begin
        Cap := Format(capNewOrEdit, ['структуры строк']);
        Descr := Format(dscNewOrEdit, ['новое поле строк', 'полей']);
        rbNew.Caption := 'Добавить новое поле строк';
      end;
    wsoColumn:
      begin
        Cap := Format(capNewOrEdit, ['структуры столбцов']);
        Descr := Format(dscNewOrEdit, ['поле столбца', 'полей']);
        rbNew.Caption := 'Добавить новое поле столбца';
      end;
    wsoFilter:
      begin
        Cap := Format(capNewOrEdit, ['набора фильтров']);
        Descr := Format(dscNewOrEdit, ['новый фильтр', 'фильтров']);
        rbNew.Caption := 'Добавить новый фильтр';
      end;
  end;
  if Movement = wmNext then
    FillExistsElements;

  rbNew.Checked := true;

  {Если находимся в режиме редактирования, то попытаемся встать на закладку -
   на элемент, который редактировали последним}
  if rbModify.Checked then
  begin
    CurItem := lvExistsElements.FindCaption(0, FLastEditedElementKey, false, true, true);
    if Assigned(CurItem) then
      CurItem.Selected := true
    else
      rbModify.OnClick(Sender);
  end;

  btnCopyTotal.Visible := CurObjectType = wsoTotal;
  SetupPage(Cap, Descr);
end;

procedure TfrmConstructorWizard.tsTotalTypeShow(Sender: TObject);
begin
  SetupPage(capTotalType, dscTotalType);
  // на листах типа "отчет" запрещаем добавление показателей - результатов
  rbResultTotal.Enabled := (GetWBCustomPropertyValue(FSheetInterface.ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType) <> '2');
end;

procedure TfrmConstructorWizard.InitTotalFormatBox;
var
  CurIndex: integer;
  ProviderId: string;

  function GetTotalFormatIndex: integer;
  var
    Cube: TCube;
    Measure: TMeasure;
  begin
    result := 0;
    cmbTotalFunction.ItemIndex := 0;
    if IsAdding then
    begin
      if (CurTotalType in [wtMeasure, wtResult]) then
      begin
        Cube := FXMLCatalog.Cubes.Find(GetSelectedCubeName, ProviderId);
        Measure := Cube.Measures.Find(GetSelectedMeasureName) as TMeasure;
        result := Ord(Measure.Format);
        {здесь же настроим функцию вычисления итогов}
        cmbTotalFunction.ItemIndex := Ord(Measure.CountMode);
      end;
    end
    else
    begin
      result := Ord(SheetInterface.Totals[CurIndex].Format);
      cmbTotalFunction.ItemIndex :=
        Ord(SheetInterface.Totals[CurIndex].CountMode);
    end;
  end;

begin
  if IsAdding then
  begin
    seDigits.Value := 1;
    if CurTotalType in [wtFree, wtConst] then
      ProviderId := XMLCatalog.PrimaryProvider
    else
      ProviderId := MeasuresTree.Measure.ProviderId;
  end
  else
  begin
    CurIndex := GetSelectedExistElementIndex;
    ProviderId := SheetInterface.Totals[CurIndex].ProviderId;
    seDigits.Value := SheetInterface.Totals[CurIndex].Digits;
  end;
  cmbTotalFormat.ItemIndex := GetTotalFormatIndex;
  seDigits.Enabled :=
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtCurrency, fmtPercent, fmtNumber]);
end;

procedure TfrmConstructorWizard.tsTotalPropertiesShow(Sender: TObject);
var
  Cube: TCube;
  i: integer;
  Misfitting: boolean;
  ProviderId: string;
  BoolValue: boolean;
begin
  SetupPage(capTotalProperties, dscTotalProperties);
  //разрезность можно определять в мастере только для свободных показателей
  cbTotalIgnoreColumns.Enabled := (CurTotalType in [wtFree, wtConst]) and
    not SheetInterface.Columns.Empty;

  // для свободных показателей и констант нет смысла в следующей настройке
  cbTotalGrandSummaryDataOnly.Enabled := (CurTotalType in [wtMeasure, wtResult]);
  cbSummariesByVisible.Enabled := (CurTotalType in [wtMeasure, wtResult]);

  edCaption.Enabled := (CurTotalType in [wtMeasure, wtResult, wtFree]);
  seDigits.Enabled := (CurTotalType in [wtMeasure, wtResult, wtFree]);
  edEmptyValueSymbol.Enabled := (CurTotalType in [wtMeasure, wtResult, wtFree]);

  {Если тип показателя "свободный" или "результат" позволяем выбирать тип
  подсчета итогов - "Типовая формула", иначе запрещаем}
  if CurTotalType in [wtFree, wtResult] then
  begin
    if (cmbTotalFunction.Items.IndexOf(dmTypeFormula) < 0) then
      cmbTotalFunction.Items.Add(dmTypeFormula);
  end
  else
    if (cmbTotalFunction.Items.IndexOf(dmTypeFormula) > -1) then
      cmbTotalFunction.Items.Delete(cmbTotalFunction.Items.IndexOf(dmTypeFormula));

  if IsAdding and (Movement = wmNext) then
  begin
    {Значение по умолчанию}
    cbTotalIgnoreColumns.Checked := false;
    cbTotalGrandSummaryDataOnly.Checked := false;
    cbSummariesByVisible.Checked := true;
    edEmptyValueSymbol.Text := '';
    case CurTotalType of
      wtFree:
        edCaption.Text := '';
      wtMeasure, wtResult:
        edCaption.Text := GetSelectedMeasureName;
      wtConst: edCaption.Text := lvConsts.Selected.Caption;
    end;
  end;
  if (Movement = wmNext) and (CurTotalType in [wtMeasure, wtResult]) then
  begin
    Misfitting := true;
    if IsAdding then
      ProviderId := MeasuresTree.Measure.ProviderId
    else
      ProviderId := SheetInterface.Totals[GetSelectedExistElementIndex].ProviderId;
    Cube := FXMLCatalog.Cubes.Find(GetSelectedCubeName, ProviderId);
    if Assigned(Cube) then
    begin
      if SheetInterface.Rows.Empty then
        Misfitting := false
      else
        for i := 0 to SheetInterface.Rows.Count - 1 do
          Misfitting := Misfitting and
            not Cube.DimAndHierInCube(SheetInterface.Rows[i].Dimension,
            SheetInterface.Rows[i].Hierarchy);
      cbTotalGrandSummaryDataOnly.Enabled := Misfitting;
      if IsAdding then
        cbSummariesByVisible.Checked := not Misfitting;
    end;
  end;
  //индицируем свойства формата
  InitTotalFormatBox;
  //инициализируем пустой символ показателя
  InitTotalEmptySymbol;
  if not cbTotalGrandSummaryDataOnly.Enabled then
    cbTotalGrandSummaryDataOnly.Checked := false;
  if (CurTotalType in [wtMeasure, wtResult]) and not cbSummariesByVisible.Enabled then
    cbSummariesByVisible.Checked := false;
  cbSummariesByVisibleClick(nil);

  if (FWizardRunMode = wrmStandart) then
    InitButton;
  CheckEnteredName;

  {Если редактирование элемента запрещено, то отключаются все контролы,
    если же разрешено - то уже отключенные контролы не должны включаться}
  if not IsAdding then
    BoolValue := GetEditingSheetElement.MayBeEdited
  else
    BoolValue := SheetInterface.MayBeEdited;
  if not BoolValue then
    EnableChildControls(tsTotalProperties, BoolValue)
end;

procedure TfrmConstructorWizard.InitTotalEmptySymbol;
var
  TotalIndex: integer;
begin
  if ((not IsAdding) and (FWizardRunMode = wrmStandart) and (Movement = wmNext)) then
  begin
    TotalIndex := GetSelectedExistElementIndex;
    if (TotalIndex = -1) then
      exit;
    edEmptyValueSymbol.Text := SheetInterface.Totals[TotalIndex].EmptyValueSymbol;
  end;
end;

procedure TfrmConstructorWizard.tsMeasureTotalShow(Sender: TObject);
begin
  //это критерий незагруженности мер (если в базе вообще нет кубов - работа невозможна)
  if (Movement = wmNext) {and (MeasuresTree.IsEmpty)} then
    if not MeasuresTree.Load(CurTotalType = wtResult) then
      if CurTotalType = wtResult then
        ShowError(ermNoWriteBackCubes)
      else
        ShowError(ermMeasuresLoadFault);

  if rbMeasureTotal.Checked then
    SetupPage(capMemberTotal, dscMemberTotal)
  else
    SetupPage(capMemberResult, dscMemberResult);
  InitButton;
end;

procedure TfrmConstructorWizard.tsDimChoiseShow(Sender: TObject);
begin
  SetupPage(capDimChoise, dscDimChoise);
  //это критерий незагруженности измерений (если в базе вообще нет измерений - работа невозможна)
  if (Movement = wmNext) and (DimensionsTree.IsEmpty) then
    (*if not *)DimensionsTree.Load (*then
      ShowError(ermDemensionsLoadFault)*);
  DimensionsTreeChange(nil, nil);
end;

procedure TfrmConstructorWizard.tsLevelChoiseShow(Sender: TObject);
begin
  SetupPage(capMemberPropertiesChoise, dscMemberPropertiesChoise);
  if Movement = wmNext then
    MPSelector.ResetScrollPosition;
end;

{приводим к привычному для нас формату отображения измерения и его иерархии,
вне зависимости от типа сервера (2000/2005)}
function GetObjDescr(DimName, HierName: string): string;
begin
  if HierName = DimName then
    HierName := '';
  if (Pos(snSemanticsSeparator, DimName) > 0) then
    DimName := StringReplace(DimName, snSemanticsSeparator, '.', []);
  if (HierName <> '') then
    HierName := '.' + HierName;
  result := DimName + HierName;
end;

procedure TfrmConstructorWizard.tsMemberChoiseShow(Sender: TObject);
var
  Op : IOperation;
  AxisElement: TSheetAxisElementInterface;
  ObjDescr, ParamStr: string;
  DimensionElement: TSheetDimension;
  TaskParam: TTaskParam;
  SheetParam: TParamInterface;
  LoadedOk: boolean;
begin
  ObjDescr := capMemberChoise + ' "' + GetObjDescr(GetSelectedDimensionName,
    GetSelectedHierarchyName) + '"';
  ParamStr := '';
  try
    CheckInheritedParam;
    if IsAdding then
    begin
      TaskParam := DimensionsTree.Parameter;
      if Assigned(TaskParam) then
      begin
        ParamStr := 'Измерение является параметром "' + TaskParam.Name + '"';
        if TaskParam.IsInherited then
          ParamStr := ParamStr + ' (от родительской задачи)';
        ParamStr := ParamStr + #13;
      end
      else
      begin
        SheetParam := DimensionsTree.SheetParam;
        if Assigned(SheetParam) then
        begin
          ParamStr := 'Измерение является параметром "' + SheetParam.FullName + '"';
          if SheetParam.IsInherited then
            ParamStr := ParamStr + ' (от родительской задачи)';
          ParamStr := ParamStr + #13;
        end;
      end;
    end
    else
    begin
      DimensionElement := TSheetDimension(GetEditingSheetElement);
      with DimensionElement do
        if IsParam then
        begin
          ParamStr := 'Измерение является параметром "' + Param.Name + '"';
          if Param.IsInherited then
            ParamStr := ParamStr + ' (от родительской задачи)';
          ParamStr := ParamStr + #13;
        end;
    end;      
  except
  end;
  SetupPage(ObjDescr, ParamStr + dscMemberChoise);
  Op := CreateComObject(CLASS_Operation) as IOperation;
  LoadedOk := false;
  try
    //Критерий загруженности элементов (с измерением без элементов работать не будет)
    if (Movement = wmNext) then
    begin
      {Поскольку порядок обхода страниц изменился после реформы дерева
      мемберов, до страницы с МР мы можем просто не дойти, нажав "готово".
      И список МР, как и две важные галки останутся неинициализированными.
      Поэтому данный фрагмент кода перенесен из tsLevelChoiseShow сюда}
      if CurObjectType <> wsoFilter then
      begin
        cbBreackHierarchy.Visible := true;
        cbHideDataMembers.Visible := true;
        if IsAdding then
        begin
          cbBreackHierarchy.Checked := false;
          cbHideDataMembers.Checked := false;
        end
        else
        begin
          AxisElement := TSheetAxisElementInterface(GetEditingSheetElement);
          cbBreackHierarchy.Checked := AxisElement.IgnoreHierarchy;
          cbHideDataMembers.Checked := AxisElement.HideDataMembers;
        end;
      end
      else
      begin
        cbBreackHierarchy.Visible := false;
        cbHideDataMembers.Visible := false;
      end;
      if (Movement = wmNext) (*and (CurObjectType <> wsoFilter)*)then
        if not LoadMemberProperties then
          ShowError('Не удалось получить свойства элементов измерения');
      if FWizardRunMode in [wrmStandart, wrmAddRow, wrmAddColumn, wrmAddFilter] then
        Op.StartOperation(Handle)
      else
        Op.StartOperation(SheetInterface.ExcelSheet.Application.Hwnd);
      Op.Caption := pcapLoadMembers;  
      btBack.Enabled := false;
      btNext.Enabled := false;
      btDone.Enabled := false;
      LoadedOk := LoadMembers;
      if not LoadedOk then
      begin
        Op.StopOperation;
        ShowDetailError(ermMembersLoadFault, FDataProvider.LastError, ermMembersLoadFault);
      end;
      if (CurObjectType = wsoFilter) then
        if not IsAdding then
          SetFilterScope(SheetInterface.Filters[GetSelectedExistElementIndex])
        else
          rbCommonFilter.Checked := true;
    end;
  finally
    Application.ProcessMessages;
    Op.StopOperation;
    Op := nil;
    btDone.Enabled := LoadedOk;
    btBack.Enabled :=
      not (FWizardRunMode in [wrmEditColumn, wrmEditRow, wrmEditFilter]);
    SetNextButtonEnabled(true);
  end;
end;

procedure TfrmConstructorWizard.tsViewAllShow(Sender: TObject);
begin
  FillResume(true);
  SetupPage(capViewAll, dscViewAll);
end;

procedure TfrmConstructorWizard.btCancelClick(Sender: TObject);
begin
  FApply := false;
  close;
end;

{--------------- условия прохода дальше для конкретных страниц -----------------}
{На странице ввода имени, в поле должна быть непустая строка }
procedure TfrmConstructorWizard.edCaptionChange(Sender: TObject);
begin
  CheckEnteredName;
end;

procedure TfrmConstructorWizard.btShowCurrentObjClick(Sender: TObject);
begin
  FillResume(true);
end;

procedure TfrmConstructorWizard.btShowAllObjClick(Sender: TObject);
begin
  FillResume(false);
end;

procedure TfrmConstructorWizard.Clear;
begin
  MeasuresTree.Items.Clear;
  DimensionsTree.Clear;
  MembersTree.Clear;
  lvFilterScope.Items.Clear;
  lwViewAll.Items.Clear;
  rbTotals.Checked := true;
  rbNew.Checked := true;
  rbMeasureTotal.Checked := true;
end;

procedure TfrmConstructorWizard.btDoneClick(Sender: TObject);

  function IsTotalFreeNameCorrect: boolean;
  begin
    result := true;
    if (CurObjectType = wsoTotal) and (CurtotalType = wtFree) then
      result := (CheckEnteredName and IsCaptionEnteredCorrect);
  end;

var
  IsTotalFreeNameCorrect_: boolean;
begin
  IsTotalFreeNameCorrect_ := IsTotalFreeNameCorrect;
  if ((CurrentPage = tsMeasureTotal) and MeasuresTree.IsMeasureSelected) or (CurrentPage = tsConstChoise) then
     tsTotalPropertiesShow(Self);
  if CurObjectType = wsoFilter then
    if not IsFilterCorrect(true) then
      exit;

  // проверка на соответствие атрибута параметра "множественный выбор" и собственно
  // выбранным элементам редактируемого измерения
  if CurrentPage = tsMemberChoise then
    if not CheckMultiplySelection then
      exit;

  if (CurrentPage = tsViewAll) or
     ((CurrentPage = tsMeasureTotal) and MeasuresTree.IsMeasureSelected) or
     ((CurrentPage = tsTotalProperties) and IsTotalFreeNameCorrect_) or
     (CurrentPage = tsMemberChoise) or
     (CurrentPage = tsFilterScope) or
     (CurrentPage = tsConstChoise) or
     (CurrentPage = tsLevelChoise) then
    CompleteCycle
  else //цикл не закончен надо предупредить.
    if (CurrentPage = tsTotalType) or
      ((CurrentPage = tsMeasureTotal) and (not MeasuresTree.IsMeasureSelected)) or
      ((CurrentPage = tsMeasureTotal) and MeasuresTree.IsMeasureSelected)or
      ((CurrentPage = tsTotalProperties) and (not IsTotalFreeNameCorrect_)) or
      (CurrentPage = tsDimChoise) (*or
      (CurrentPage = tsLevelChoise)*) then
    begin
      if (FWizardRunMode <> wrmStandart) then
        exit;
      if not ShowQuestion(qumDontComplite) then
        exit;
    end;

  FApply := true;
  close;
end;

{Сдвинуть элемент вверх}
procedure TfrmConstructorWizard.btUpElementClick(Sender: TObject);
var
  SelInd: integer;
begin
  SelInd := GetSelectedExistElementIndex;
  if SelInd > 0 then
  begin
    with SheetInterface do
      case CurObjectType of
        wsoTotal: Totals.Exchange(SelInd - 1, SelInd);
        wsoRow: Rows.Exchange(SelInd - 1, SelInd);
        wsoColumn: Columns.Exchange(SelInd - 1, SelInd);
        wsoFilter: Filters.Exchange(SelInd - 1, SelInd);
      end;

    FillExistsElements;
    lvExistsElements.Items[SelInd - 1].Selected := true;
    lvExistsElements.Items[SelInd - 1].MakeVisible(false);

    FIsMayDone := true;
    btDone.Enabled := true;
  end;
end;

{Сдвинуть элемент вниз}
procedure TfrmConstructorWizard.btDownElementClick(Sender: TObject);
var
  SelInd: integer;
begin
  SelInd := GetSelectedExistElementIndex;
  if (SelInd >= 0) and (SelInd < lvExistsElements.Items.Count - 1) then
  begin
    with SheetInterface do
      case CurObjectType of
        wsoTotal: Totals.Exchange(SelInd, SelInd + 1);
        wsoRow: Rows.Exchange(SelInd, SelInd + 1);
        wsoColumn: Columns.Exchange(SelInd, SelInd + 1);
        wsoFilter: Filters.Exchange(SelInd, SelInd + 1);
      end;

    FillExistsElements;
    lvExistsElements.Items[SelInd + 1].Selected := true;
    lvExistsElements.Items[SelInd + 1].MakeVisible(false);

    FIsMayDone := true;
    btDone.Enabled := true;
  end;
end;

{Удалить элемент}
procedure TfrmConstructorWizard.btRemoveElementClick(Sender: TObject);
var
  SelInd, Cnt: integer;
begin
  SelInd := GetSelectedExistElementIndex;
  if SelInd >= 0 then
  begin
    with SheetInterface do
      case CurObjectType of
        wsoTotal: begin
          if not Totals[SelInd].MayBeDeleted then
            exit;
          AddCommentForHistory(weDel, Totals[SelInd]);
          Totals.Delete(SelInd);
        end;
        wsoRow: begin
          if not Rows[SelInd].MayBeDeleted then
            exit;
          AddCommentForHistory(weDel, Rows[SelInd]);
          Rows.Delete(SelInd);
        end;
        wsoColumn: begin
          if not Columns[SelInd].MayBeDeleted then
            exit;
          AddCommentForHistory(weDel, Columns[SelInd]);
          Columns.Delete(SelInd);
        end;
        wsoFilter: begin
          if not Filters[SelInd].MayBeDeleted then
            exit;
          AddCommentForHistory(weDel, Filters[SelInd]);
          Filters.Delete(SelInd);
        end;
      end;

    lvExistsElements.Items.Delete(SelInd);
    Cnt := lvExistsElements.Items.Count;
    if SelInd < Cnt then
      lvExistsElements.Items[SelInd].Selected := true
    else
      if Cnt > 0 then
        lvExistsElements.Items[Cnt - 1].Selected := true;
    FIsMayDone := true;
    btDone.Enabled := true;
    {Если после удаления ни осталось ни одного элемента, выставляем
    автоматически признак на добавление нового}
    rbNew.Checked := (lvExistsElements.Items.Count = 0);
  end;
end;

{Обрабатываем нажатие ESC}
procedure TfrmConstructorWizard.FormKeyPress(Sender: TObject;
  var Key: Char);
begin
  if (Key = Chr(27)) then
  begin
    ModalResult := mrCancel;
    FApply := false;
    Close;
  end;
end;

procedure TfrmConstructorWizard.lvExistsElementsSelectItem(Sender: TObject;
  Item: TListItem; Selected: Boolean);
begin
  if Selected then
  begin
    rbModify.Checked := true;
    rbModify.Enabled := GetEditingSheetElement.MayBeEdited;
    btNext.Enabled := rbModify.Enabled;
  end;
end;

procedure TfrmConstructorWizard.tsFilterScopeShow(Sender: TObject);
var
  Filter: TSheetFilterInterface;
begin
  SetupPage(capFilterScope, dscFilterScope);
  if (Movement = wmNext) then
  begin
    lvFilterScope.Items.Clear;
    rbCommonFilter.Checked := true;

    if not IsAdding then
    begin
      Filter := SheetInterface.Filters[GetSelectedExistElementIndex];
      rbPartialFilter.Checked := Filter.IsPartial;
    end
    else
      Filter := nil;

    SetFilterScope(Filter);
  end;
end;

function TfrmConstructorWizard.GetFilterScope: TStringList;
var
  i: integer;
//  Total: TSheetTotal;
//  CubeName, MeasureName: string;
begin
  result := nil;
  if rbCommonFilter.Checked then //если фильтр общий, то возвращаем nil
    exit;
  result := TStringList.Create;
  for i := 0 to lvFilterScope.Items.Count - 1 do
    if lvFilterScope.Items[i].Checked then
    begin
      result.Add(lvFilterScope.Items[i].SubItems[1]);
//***
(*
      CubeName := lvFilterScope.Items[i].SubItems[0];
      MeasureName := lvFilterScope.Items[i].Caption;
      Total := SheetInterface.Totals.FindTotal(CubeName, MeasureName);
      if Assigned(Total) then
        result.Add(Total.UniqueID);
*)
    end;
end;

procedure TfrmConstructorWizard.SetFilterScope(Filter: TSheetFilterInterface);
var
  i: integer;
  Total: TSheetTotalInterface;
  FilterProviderId: string;
begin
  if Assigned(Filter) then
  begin
    if Filter.IsPartial then
      rbPartialFilter.Checked := true
    else
      rbCommonFilter.Checked := true;
    FilterProviderId := Filter.ProviderId;
  end
  else
  try
    FilterProviderId := DimensionsTree.Hierarchy.ProviderId;
  except
    FilterProviderId := XMLCatalog.PrimaryProvider; // думаю эта фигня лишняя
  end;

  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[i];
    if Total.ProviderId <> FilterProviderId then
      continue;
    if (Total.TotalType = wtMeasure) or (Total.TotalType = wtResult) then
    begin
      //а вдруг база изменилась? тогда может и не быть такого куба
      if not Assigned(Total.Cube) then
        continue;
      if Total.Cube.DimAndHierInCube(GetSelectedDimensionName, GetSelectedHierarchyName) then
      begin
        with lvFilterScope.Items.Add do
        begin
          Caption := Total.Caption;
          SubItems.Append('куб: ' + Total.CubeName + ' мера: ' + Total.MeasureName);
          SubItems.Append(Total.UniqueID);
          ImageIndex := 1;
          if IsAdding then
            Checked := false
          else
            if Assigned(Filter) then
              Checked := Filter.IsPartial and Total.IsFilteredBy(Filter)
            else
              Checked := false;
        end;
      end;
    end;
  end;
end;

procedure TfrmConstructorWizard.lvFilterScopeSelectItem(Sender: TObject;
  Item: TListItem; Selected: Boolean);
begin
  rbPartialFilter.Checked := true;
end;

procedure TfrmConstructorWizard.lwLevelsMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
end;

procedure TfrmConstructorWizard.lvFilterScopeMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  ListItem: TListItem;
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
  try
    if (Button = mbleft) then
    begin
      ListItem := (Sender as TListView).GetItemAt(X, Y);
      if ListItem.Checked then
        rbPartialFilter.Checked := true;
    end;
  except
  end;
end;

procedure TfrmConstructorWizard.SetXMLCatalog(Value: TXMLCatalog);
begin
  FXMLCatalog := Value;
  MeasuresTree.Catalog := Value;
  DimensionsTree.Catalog := Value;
end;

function TfrmConstructorWizard.GetEditingSheetElement: TSheetElement;
var
  Index: integer;
begin
  result := nil;
  Index := GetSelectedExistElementIndex;
  if Index = -1 then
    exit;
  case FWizardRunMode of
    wrmEditTotal: result := SheetInterface.Totals[Index] as TSheetElement;
    wrmEditRow: result := SheetInterface.Rows[Index] as TSheetElement;
    wrmEditColumn: result := SheetInterface.Columns[Index] as TSheetElement;
    wrmEditFilter: result := SheetInterface.Filters[Index] as TSheetElement;
  else
    result := TSheetElement(lvExistsElements.Items[Index].Data);
  end;
end;

procedure TfrmConstructorWizard.SetSheet(Value: TSheetInterface);
begin
  FSheetInterface := Value;
  DimensionsTree.SheetInterface := Value;
end;

procedure TfrmConstructorWizard.cmbTotalFormatChange(Sender: TObject);
begin
  // Разрядность - только для числовых форматов
  seDigits.Enabled :=
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtCurrency, fmtPercent, fmtNumber]);

  cbSummariesByVisible.Enabled := (CurTotalType in [wtMeasure, wtResult]) and
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtStandard, fmtCurrency, fmtPercent, fmtNumber]);
  cbSummariesByVisibleClick(nil);

  // Для нечисловых показателей жестко задаем итоги не вычислять
  if TMeasureFormat(cmbTotalFormat.ItemIndex) in [fmtText, fmtBoolean] then
    cmbTotalFunction.ItemIndex := 4;
end;

procedure TfrmConstructorWizard.rbCommonFilterClick(Sender: TObject);
var
  i: integer;
begin
  if rbCommonFilter.Checked then
    for i := 0 to lvFilterScope.Items.Count - 1 do
      lvFilterScope.Items[i].Checked := false;
end;

procedure TfrmConstructorWizard.DimensionsTreeChange(Sender: TObject;
  Node: TTreeNode);
begin
  SetNextButtonEnabled(Assigned(DimensionsTree.Dimension));
end;

function TfrmConstructorWizard.IsFilterCorrect(IsShowWarning: boolean): boolean;
var
  DimName, HierName, ExceptId: string;
  WarningMsg: string;
  ScopeList: TStringList;
begin
  result := true;
  if CurObjectType <> wsoFilter then
    exit;
  DimName := GetSelectedDimensionName;
  if DimName = '' then
    exit;
  HierName := GetSelectedHierarchyName;

  if IsAdding then
  begin
    ExceptId := '';
    {Если прокручиваеся уже не первый круг,то в этом случае текущим фильтром будем
     считать последний, с ним и будем сравнивать.}
    if FLap > 1 then
      if SheetInterface.Filters.Count > 0 then
        ExceptId := SheetInterface.Filters[SheetInterface.Filters.Count - 1].UniqueID;
  end
  else
    ExceptId := SheetInterface.Filters[GetSelectedExistElementIndex].UniqueID;

  ScopeList := nil;
  try
    if rbPartialFilter.Checked then
      ScopeList :=  GetFilterScope;
    result := not SheetInterface.Filters.IsThereDuplicateFilters(DimName, HierName,
      ExceptId, ScopeList, WarningMsg);
  finally
    FreeAndNil(ScopeList);
  end;
  if (not result) and IsShowWarning and (WarningMsg <> '') then
    ShowWarning(WarningMsg);
end;

function TfrmConstructorWizard.IsFormMaximized: boolean;
begin
  result := (Width > (FOldRect.Right - FOldRect.Left));
end;

procedure TfrmConstructorWizard.labelMaximizeClick(Sender: TObject);
begin
  if IsFormMaximized then
    ChangeWindowState(false)
  else
    ChangeWindowState(true);
end;

procedure TfrmConstructorWizard.SetMaximizeLinkCaption;
begin
  if IsFormMaximized then
    labelMaximize.Caption := 'Свернуть'
  else
    if pcPages.ActivePage = tsMemberChoise then
      labelMaximize.Caption := 'Развернуть и показать уровни'
    else
      labelMaximize.Caption := 'Развернуть';
end;

{Экстренное изменение размеров окна (только для страницы мемберсов) }
procedure TfrmConstructorWizard.ChangeWindowState(AFullScreen: boolean);
var
  Rect, NewRect: TRect;
begin
  GetWindowRect(Self.Handle, NewRect);
  SystemParametersInfo(SPI_GETWORKAREA, 0, @Rect, 0);
  if not((FOldRect.Top = NewRect.Top) and (FOldRect.Left = NewRect.Left)
    and (FOldRect.Right = NewRect.Right) and (FOldRect.Bottom = NewRect.Bottom))
    and not(((NewRect.Left - NewRect.Right) = (Rect.Left - Rect.Right))
    and ((NewRect.Top - NewRect.Bottom) = (Rect.Top - Rect.Bottom))) then
    FOldRect := NewRect;
  if AFullScreen then
  begin
    try
      Top := Rect.Top;
      Left := Rect.Left;
      Height := Rect.Bottom - Top;
      Width := Rect.Right - Left;
    except
    end;
  end
  else
  begin
    try
      Top := FOldRect.Top;
      Left := FOldRect.Left;
      Height := FOldRect.Bottom - Top;
      Width := FOldRect.Right - Left;
    except
    end;
  end;
  SetMaximizeLinkCaption;
end;

procedure TfrmConstructorWizard.AddCommentForHistory(EventType: TWizardEvent;
  SheetElement: TSheetElement);
var
  Total: TSheetTotalInterface;
  Filter: TSheetFilterInterface;
  Row, Column: TSheetAxisElementInterface;
  sComment: string;

  // если параметр - то добавить в комментарий
  procedure CheckParam(SheetDimension: TSheetDimension; var Comment: string);
  begin
    if Assigned(SheetDimension) and SheetDimension.IsParam then
      Comment := Comment + ', параметр "' + SheetDimension.Param.FullName + '"';
  end;

begin
  if not Assigned(FEventList) then
    FEventList := TStringList.Create;

  case CurObjectType of
    wsoColumn: begin
      Column := (SheetElement as TSheetAxisElementInterface);
      sComment := 'элемент столбцов "' + Column.FullDimensionName2 + '"';
      CheckParam((SheetElement as TSheetDimension), sComment);
    end;
    wsoRow: begin
      Row := (SheetElement as TSheetAxisElementInterface);
      sComment := 'элемент строк "' + Row.FullDimensionName2 + '"';
      CheckParam((SheetElement as TSheetDimension), sComment);
    end;
    wsoFilter: begin
      Filter := (SheetElement as TSheetFilterInterface);
      sComment := 'элемент фильтров "' + Filter.FullDimensionName2 + '"';
      CheckParam((SheetElement as TSheetDimension), sComment);
    end;
    wsoTotal: begin
      Total := (SheetElement as TSheetTotalInterface);
      case Total.TotalType of
        wtFree: sComment := 'показатель "' + Total.Caption + '" тип: Свободный.';
        wtConst: sComment := 'показатель "' + Total.Caption + '" тип: Константа.';
        wtMeasure: sComment := 'показатель "' + Total.Caption + '" тип: Мера,' +
          ' куб: "' + Total.CubeName + '", мера: "' + Total.MeasureName + '".';
        wtResult: sComment := 'показатель "' + Total.Caption + '" тип: Результат,' +
          ' куб: "' + Total.CubeName + '", мера: "' + Total.MeasureName + '".';
      end;
    end;
  end;
  case EventType of
    weAdd: sComment := 'Добавлен ' + sComment;
    weEdit: sComment := 'Изменен ' + sComment;
    weDel: sComment := 'Удален ' + sComment;
  end;
  FEventList.Add(sComment);
end;

function TfrmConstructorWizard.GetEventList: string;
begin
  result := FEventList.CommaText;
end;

procedure TfrmConstructorWizard.MeasuresTreeChange(Sender: TObject;
  Node: TTreeNode);
begin
{ На странице выбора меры, не будем пускать дальше до тех пор пока не выберут меру, а не куб }
  InitButton;
end;

procedure TfrmConstructorWizard.cbSummariesByVisibleClick(Sender: TObject);
begin
  cmbTotalFunction.Enabled := cbSummariesByVisible.Checked and
    (TMeasureFormat(cmbTotalFormat.ItemIndex) in
    [fmtStandard, fmtCurrency, fmtPercent, fmtNumber]);
  lblTotalFunction.Enabled := cmbTotalFunction.Enabled;
end;

procedure TfrmConstructorWizard.lvExistsElementsKeyUp(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if Key = VK_DELETE then
    if Assigned(lvExistsElements.Selected) then
      btRemoveElementClick(Self);
end;

procedure TfrmConstructorWizard.btnCopyTotalClick(Sender: TObject);
var
  SelInd, i: integer;
  NewTotal, OldTotal: TSheetTotalInterface;
  tmpDom: IXMLDOMDocument2;
  tmpNode: IXMLDOMNode;
  Found: boolean;
begin
  if CurObjectType <> wsoTotal then
    exit;
  SelInd := GetSelectedExistElementIndex;
  if SelInd < 0 then
    exit;

  try
    GetDomDocument(tmpDom);
    tmpNode := tmpDom.createNode(1, 'tmpNode', '');
    OldTotal := SheetInterface.Totals[SelInd];
    NewTotal := SheetInterface.Totals.Append;
    SheetInterface.InCopyMode := true;
    OldTotal.WriteToXML(tmpNode);
    NewTotal.ReadFromXML(tmpNode);
    NewTotal.NumberFormat := OldTotal.NumberFormat;
    if NewTotal.TotalType <> wtConst then
    repeat
      NewTotal.Caption := NewTotal.Caption + '_копия' ;
      Found := false;
      for i := 0 to SheetInterface.Totals.Count - 2 do
        if SheetInterface.Totals[i].Caption = NewTotal.Caption then
        begin
          Found := true;
          break;
        end;
    until not Found;
    for i := 0 to SheetInterface.Filters.Count - 1 do
      if SheetInterface.Filters[i].IsPartial then
        if SheetInterface.Filters[i].Scope.IndexOf(OldTotal.UniqueID) > -1 then
          SheetInterface.Filters[i].Scope.Add(NewTotal.UniqueID);
    AddCommentForHistory(weAdd, NewTotal);
    FillExistsElements;
    lvExistsElements.Selected := lvExistsElements.Items[SelInd];
    FIsMayDone := true;
    btDone.Enabled := true;
  finally
    SheetInterface.InCopyMode := false;
    tmpNode := nil;
    KillDomDocument(tmpDom);
  end;
end;

procedure TfrmConstructorWizard.tsConstChoiseShow(Sender: TObject);
var
  i: integer;
begin
  SetupPage(capConstsChoise, dscConstsChoise);
  btNext.Enabled := false;
  btDone.Enabled := false;
  lvConsts.Items.Clear;
  for i := 0 to SheetInterface.Consts.Count - 1 do
    with lvConsts.Items.Add do
    begin
      Caption := SheetInterface.Consts.Items[i].Name;
      SubItems.Append(SheetInterface.Consts.Items[i].Value);
      SubItems.Append(SheetInterface.Consts.Items[i].Comment);
      Data := Pointer(SheetInterface.Consts.Items[i].UniqueId);
    end;
end;

procedure TfrmConstructorWizard.lvConstsClick(Sender: TObject);
begin
  btDone.Enabled := (lvConsts.Selected <> nil);
  SetNextButtonEnabled(lvConsts.Selected <> nil);
end;

procedure TfrmConstructorWizard.rbModifyClick(Sender: TObject);
begin
  if (lvExistsElements.Selected = nil) and (lvExistsElements.Items.Count > 0) then
    lvExistsElements.Items.Item[0].Selected := true;
end;

procedure TfrmConstructorWizard.SetNextButtonEnabled(Value: boolean);
begin
  (*if IsAdding and (CurrentPage <> tsElementType) then
    btNext.Enabled := Value and SheetInterface.MayBeEdited
  else *)
    btNext.Enabled := Value;
end;

procedure TfrmConstructorWizard.rbNewClick(Sender: TObject);
begin
  btNext.Enabled := true;
end;

function TfrmConstructorWizard.GetCurCollection: TSheetCollection;
begin
  case Get_CurObjectType of
    wsoTotal: result := FSheetInterface.Totals;
    wsoRow: result := FSheetInterface.Rows;
    wsoColumn: result := FSheetInterface.Columns;
    else
      result := FSheetInterface.Filters;
  end;
end;

end.





