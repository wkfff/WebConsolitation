{
  Панелька, позволяющая пользователю вставлять разрывы в таблицу.
}

unit uSplitterPad;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  fmFlatPanel, ExtCtrls, fmTitlebar, Buttons, fmSpeedButton, Math, OfficeXP,
  ExcelXP, uFMAddinExcelUtils, uFMExcelAddInConst, uExcelUtils,
  uGlobalPlaningConst, uSheetObjectModel;

const
  ActiveSplitColor: TColor = {clBlue;}$00FF6633;
  DeactiveSplitColor: TColor = $00F4F5F6;
  ActiveSplitFontColor: TColor = clWhite;
  DeactiveSplitFontColor: TColor = clBlack;
  ActiveAreaBorderColor: TColor = (* $006B2408;*)$00444444;//clHighlight;//clRed;
  DeactiveAreaBorderColor: TColor = (* $00A09080;*)clGray;
  ActiveSplitCaption: string = '                           Разрыв...                +';
  DeactiveSplitCaption: string = '                           Разрыв...';

type
  TSheetSplitterType = (sstAfterFilter, sstAfterUnitMarker, sstAfterColumnTitles,
    sstAfterColumn, sstAfterTotalTitles, sstAfterTotals);

  TfrmSplitterPad = class(TForm)
    TitleBar: TfmTitlebar;
    pShaperTop: TPanel;
    pShaperLeft: TPanel;
    pShaperRightInner: TPanel;
    pShaperBottom: TPanel;
    pShaperLeftInner: TPanel;
    pShaperRight: TPanel;
    pShaperTopInner: TPanel;
    Panel2: TPanel;
    Panel3: TPanel;
    pShaperBottomInner: TPanel;
    Panel5: TPanel;
    Panel6: TPanel;
    pMainContainer: TPanel;
    pFilterLayer: TPanel;
    pFilterLabel: TfmFlatPanel;
    pColumnTitlesLayer: TPanel;
    pColumnTitlesLabel: TfmFlatPanel;
    pColumnsTitleLeftDummy: TPanel;
    pColumnsLayer: TPanel;
    pColumnsLabel: TfmFlatPanel;
    pColumnsLeftDummy: TPanel;
    pTotalTitlesLayer: TPanel;
    pRowTitlesLabel: TfmFlatPanel;
    pTotalTitlesLabel: TfmFlatPanel;
    pDataLayer: TPanel;
    pRowLabel: TfmFlatPanel;
    pTotalsLabel: TfmFlatPanel;
    pTaskInfoLayer: TPanel;
    pTaskInfoLabel: TfmFlatPanel;
    pSplit1: TfmFlatPanel;
    pSplit3: TfmFlatPanel;
    pSplit4: TfmFlatPanel;
    pSplit5: TfmFlatPanel;
    pSplit6: TfmFlatPanel;
    btClose: TfmExtSpeedButton;
    PlotBlack1: TPanel;
    PlotBlack2: TPanel;
    pUnitMarkerLayer: TPanel;
    pSplit2: TfmFlatPanel;
    procedure pMainContainerMouseMove(Sender: TObject; Shift: TShiftState;
      X, Y: Integer);
    procedure pSplit1MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pSplit3MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pSplit4MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pSplit5MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pSplit6MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure FormShow(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure btCloseClick(Sender: TObject);
    procedure pSplit2MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pFilterLabelMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pColumnTitlesLabelMouseMove(Sender: TObject;
      Shift: TShiftState; X, Y: Integer);
    procedure pColumnsLabelMouseMove(Sender: TObject; Shift: TShiftState;
      X, Y: Integer);
    procedure pRowTitlesLabelMouseMove(Sender: TObject; Shift: TShiftState;
      X, Y: Integer);
    procedure pTotalTitlesLabelMouseMove(Sender: TObject;
      Shift: TShiftState; X, Y: Integer);
    procedure pRowLabelMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pTotalsLabelMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure pTaskInfoLabelMouseMove(Sender: TObject; Shift: TShiftState;
      X, Y: Integer);
    procedure pFilterLabelClick(Sender: TObject);
    procedure pColumnTitlesLabelClick(Sender: TObject);
    procedure pColumnsLabelClick(Sender: TObject);
    procedure pRowTitlesLabelClick(Sender: TObject);
    procedure pRowLabelClick(Sender: TObject);
    procedure pTotalTitlesLabelClick(Sender: TObject);
    procedure pTotalsLabelClick(Sender: TObject);
    procedure pTaskInfoLabelClick(Sender: TObject);
    procedure pUnitMarkerLayerClick(Sender: TObject);
    procedure btCloseMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure btCloseMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit1MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit2MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit3MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit4MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit5MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure pSplit6MouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormHide(Sender: TObject);
  private
    { Private declarations }
    FFilterAvail: boolean;
    FUnitMarkerAvail: boolean;
    FColumnTitlesAvail: boolean;
    FColumnsAvail: boolean;
    FRowTitlesAvail: boolean;
    FRowsAvail: boolean;
    FTotalTitlesAvail: boolean;
    FTotalsAvail: boolean;
    FSheetIDAvail: boolean;
    FExcelSheet: ExcelWorksheet;

    procedure HighLightSplit(spPanel: TfmFlatPanel);
    procedure Validate;

    procedure SelectName(AName: string);
    function SplitAfterFilterIsPosible: boolean;
    function SplitAfterUnitMarkerIsPosible: boolean;
    function SplitAfterColumnTitlesIsPosible: boolean;
    function SplitAfterColumnsIsPosible: boolean;
    function SplitAfterTotalTitlesIsPosible: boolean;
    function SplitAfterTotalsIsPosible: boolean;

    procedure InsertSplitter(SplitterType: TSheetSplitterType);
    {Возвращает диапазон под место разрыва}
    function GetInsertionPoint(SplitterType: TSheetSplitterType): ExcelRange;
    {Возвращает диапазон самого разрыва}
    function GetSplitterRange(SplitterType: TSheetSplitterType): ExcelRange;
    function GetSplitterName(SplitterType: TSheetSplitterType): string;
    function LaunchToolButton: CommandBarButton;
    function LaunchMenuButton: CommandBarButton;
  protected
    procedure Set_FilterAvail(AValue: boolean);
    procedure Set_UnitMarkerAvail(AValue: boolean);
    procedure Set_ColumnsAvail(AValue: boolean);
    procedure Set_ColumnTitlesAvail(AValue: boolean);
    procedure Set_RowsAvail(AValue: boolean);
    procedure Set_RowTitlesAvail(AValue: boolean);
    procedure Set_TotalsAvail(AValue: boolean);
    procedure Set_TotalTitlesAvail(AValue: boolean);
    procedure Set_SheetIDAvail(AValue: boolean);
    function Get_LaunchButtonState: OleVariant;
    procedure Set_LaunchButtonState(AValue: OleVariant);
  public
    procedure Init(SMD: TSheetInterface);

    property FilterAvail: boolean read FFilterAvail write Set_FilterAvail;
    property UnitMarkerAvail: boolean read FUnitMarkerAvail write Set_UnitMarkerAvail;
    property ColumnsAvail: boolean read FColumnsAvail write Set_ColumnsAvail;
    property ColumnTitlesAvail: boolean read FColumnTitlesAvail write Set_ColumnTitlesAvail;
    property RowsAvail: boolean read FRowsAvail write Set_RowsAvail;
    property RowTitlesAvail: boolean read FRowTitlesAvail write Set_RowTitlesAvail;
    property TotalsAvail: boolean read FTotalsAvail write Set_TotalsAvail;
    property TotalTitlesAvail: boolean read FTotalTitlesAvail write Set_TotalTitlesAvail;
    property SheetIDAvail: boolean read FSheetIDAvail write Set_SheetIDAvail;
    {Состояние тех кнопок на тулбаре и меню экселя, которыми вызывается эта форма.
    Поскольку она стэй-он-топ, приходится это дело обрабатывать внутри самой формы.}
    property LaunchButtonState: OleVariant read Get_LaunchButtonState write Set_LaunchButtonState;
  end;

var
  frmSplitterPad: TfrmSplitterPad;

implementation
uses
  uFMExcelAddin;
{$R *.DFM}


procedure TfrmSplitterPad.Set_FilterAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FFilterAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_UnitMarkerAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FUnitMarkerAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_ColumnsAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FColumnsAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_ColumnTitlesAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FColumnTitlesAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_RowsAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FRowsAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_RowTitlesAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FRowTitlesAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_TotalsAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FTotalsAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_TotalTitlesAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FTotalTitlesAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.Set_SheetIDAvail(AValue: boolean);
begin
  HighLightSplit(nil);
  FSheetIDAvail := AValue;
  Validate;
end;

procedure TfrmSplitterPad.HighLightSplit(spPanel: TfmFlatPanel);

  procedure ActivateArea(spPanel: TfmFlatPanel);
  begin
    spPanel.BorderColorTop := ActiveAreaBorderColor;
    spPanel.BorderColorRight := ActiveAreaBorderColor;
    spPanel.BorderColorBottom := ActiveAreaBorderColor;
    spPanel.BorderColorLeft := ActiveAreaBorderColor;
  end;

  procedure DeactivateArea(spPanel: TfmFlatPanel);
  begin
    spPanel.BorderColorTop := DeactiveAreaBorderColor;
    spPanel.BorderColorRight := DeactiveAreaBorderColor;
    spPanel.BorderColorBottom := DeactiveAreaBorderColor;
    spPanel.BorderColorLeft := DeactiveAreaBorderColor;
  end;

  procedure ActivateSplit(spPanel: TfmFlatPanel);
  begin
    spPanel.Color := ActiveSplitColor;
    spPanel.Font.Color := ActiveSplitFontColor;
    spPanel.Caption :=  ActiveSplitCaption;
  end;

  procedure DeactivateSplit(spPanel: TfmFlatPanel);
  begin
    spPanel.Color := DeactiveSplitColor;
    spPanel.Font.Color := DeactiveSplitFontColor;
    spPanel.Caption :=  DeactiveSplitCaption;
  end;
begin
  DeactivateSplit(pSplit1);
  DeactivateSplit(pSplit2);
  DeactivateSplit(pSplit3);
  DeactivateSplit(pSplit4);
  DeactivateSplit(pSplit5);
  DeactivateSplit(pSplit6);


  DeactivateArea(pFilterLabel);
  DeactivateArea(pColumnTitlesLabel);
  DeactivateArea(pColumnsLabel);
  DeactivateArea(pRowTitlesLabel);
  DeactivateArea(pTotalTitlesLabel);
  DeactivateArea(pRowLabel);
  DeactivateArea(pTotalsLabel);
  DeactivateArea(pTaskInfoLabel);

  if Assigned(spPanel) then
  begin
    if (spPanel.Tag = 1) then
      ActivateArea(spPanel)
    else
      ActivateSplit(spPanel);
  end;
end;

procedure TfrmSplitterPad.Init(SMD: TSheetInterface);
begin
  if Assigned(SMD) then
  begin
    FExcelSheet := SMD.ExcelSheet;
    FilterAvail := (SMD.GetFilterCountWithScope(false) > 0) and
      (SMD.IsDisplayFilters);
    UnitMarkerAvail := (SMD.TotalMultiplier > tmE1) and (SMD.Totals.Count > 0);
    ColumnsAvail := ((SMD.Columns.Count > 0) and SMD.IsDisplayColumns);
    ColumnTitlesAvail := (ColumnsAvail and SMD.IsDisplayColumnsTitles);
    RowsAvail := (SMD.Rows.Count > 0);
    RowTitlesAvail := (RowsAvail and SMD.IsDisplayRowsTitles);
    TotalsAvail := (SMD.Totals.Count > 0);
    TotalTitlesAvail := (TotalsAvail and SMD.IsDisplayTotalsTitles);
    SheetIDAvail := SMD.IsDisplaySheetInfo and
      (FilterAvail or ColumnsAvail or RowsAvail or TotalsAvail);
  end;

  Validate;

  pSplit1.Enabled := SMD.MayBeEdited;
  pSplit2.Enabled := SMD.MayBeEdited;
  pSplit3.Enabled := SMD.MayBeEdited;
  pSplit4.Enabled := SMD.MayBeEdited;
  pSplit5.Enabled := SMD.MayBeEdited;
  pSplit6.Enabled := SMD.MayBeEdited;
end;


procedure TfrmSplitterPad.Validate;
  procedure Arrange;
    procedure SetTop(Layer: TPanel; var ATop: integer);
    begin
      if Layer.Visible then
      begin
        Layer.Top := ATop;
        ATop := ATop + Layer.Height + 1;
      end;
    end;

  var
    CurTop: integer;
  begin
    CurTop := 0;

    SetTop(pFilterLayer, CurTop);
    SetTop(TPanel(pSplit1), CurTop);
    SetTop(pUnitMarkerLayer, CurTop);
    SetTop(TPanel(pSplit2), CurTop);
    SetTop(pColumnTitlesLayer, CurTop);
    SetTop(TPanel(pSplit3), CurTop);
    SetTop(pColumnsLayer, CurTop);
    SetTop(TPanel(pSplit4), CurTop);
    SetTop(pTotalTitlesLayer, CurTop);
    SetTop(TPanel(pSplit5), CurTop);
    SetTop(pDataLayer, CurTop);
    SetTop(TPanel(pSplit6), CurTop);
    SetTop(pTaskInfoLayer, CurTop);
  end;

begin

  pFilterLayer.Visible := FFilterAvail;
  pSplit1.Visible := SplitAfterFilterIsPosible;

  pUnitMarkerLayer.Visible := FUnitMarkerAvail;
  pSplit2.Visible := SplitAfterUnitMarkerIsPosible;

  pColumnTitlesLayer.Visible := FColumnTitlesAvail;
  pSplit3.Visible := SplitAfterColumnTitlesIsPosible;
  pColumnsLayer.Visible := FColumnsAvail;
  pSplit4.Visible := SplitAfterColumnsIsPosible;


  pRowTitlesLabel.Visible := FRowTitlesAvail;
  pRowLabel.Visible := FRowsAvail;
  pColumnsTitleLeftDummy.Visible := FRowsAvail;
  pColumnsLeftDummy.Visible := FRowsAvail;


  pTotalTitlesLayer.Visible := (FRowTitlesAvail or FTotalTitlesAvail);
  pSplit5.Visible := SplitAfterTotalTitlesIsPosible;
  pDataLayer.Visible := (FRowsAvail or FTotalsAvail);
  pSplit6.Visible := SplitAfterTotalsIsPosible;


  pTotalTitlesLabel.Visible := FTotalTitlesAvail;
  pTotalsLabel.Visible := FTotalsAvail;


  if FTotalsAvail then
  begin
    pRowTitlesLabel.PanelSides := [fsdLeft,fsdTop,fsdBottom];
    pRowLabel.PanelSides := [fsdLeft,fsdTop,fsdBottom];
  end
  else
  begin
    pRowTitlesLabel.PanelSides := [fsdRight, fsdLeft,fsdTop,fsdBottom];
    pRowLabel.PanelSides := [fsdRight, fsdLeft,fsdTop,fsdBottom];
  end;

  pTotalTitlesLabel.PanelSides := [fsdRight, fsdLeft,fsdTop,fsdBottom];
  pTotalsLabel.PanelSides := [fsdRight, fsdLeft,fsdTop,fsdBottom];

  pTaskInfoLayer.Visible := FSheetIDAvail;

  Arrange;
  self.Height := max(pMainContainer.Top + pMainContainer.Height + 3, 40);
end;


procedure TfrmSplitterPad.pMainContainerMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(nil);
end;

procedure TfrmSplitterPad.pSplit1MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit1);
end;

procedure TfrmSplitterPad.pSplit2MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit2);
end;


procedure TfrmSplitterPad.pSplit3MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit3);
end;

procedure TfrmSplitterPad.pSplit4MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit4);
end;

procedure TfrmSplitterPad.pSplit5MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit5);
end;

procedure TfrmSplitterPad.pSplit6MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pSplit6);
end;


procedure TfrmSplitterPad.FormCreate(Sender: TObject);
begin
  FFilterAvail := true;
  FUnitMarkerAvail := true;
  FColumnTitlesAvail := true;
  FColumnsAvail := true;
  FRowTitlesAvail := true;
  FRowsAvail := true;
  FTotalTitlesAvail := true;
  FTotalsAvail := true;
  FSheetIDAvail := true;
end;

procedure TfrmSplitterPad.btCloseClick(Sender: TObject);
begin
  close;
end;

procedure TfrmSplitterPad.pFilterLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pFilterLabel);
end;

procedure TfrmSplitterPad.pColumnTitlesLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pColumnTitlesLabel);
end;

procedure TfrmSplitterPad.pColumnsLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pColumnsLabel);
end;

procedure TfrmSplitterPad.pRowTitlesLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  pRowTitlesLabel.PanelSides := [fsdRight, fsdLeft, fsdTop, fsdBottom];
  pTotalTitlesLabel.PanelSides := [fsdRight, fsdTop, fsdBottom];

  HighLightSplit(pRowTitlesLabel);
end;

procedure TfrmSplitterPad.pTotalTitlesLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  pRowTitlesLabel.PanelSides := [fsdLeft, fsdTop, fsdBottom];
  pTotalTitlesLabel.PanelSides := [fsdRight, fsdLeft, fsdTop, fsdBottom];

  HighLightSplit(pTotalTitlesLabel);
end;

procedure TfrmSplitterPad.pRowLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  pRowLabel.PanelSides := [fsdRight, fsdLeft, fsdTop, fsdBottom];
  pTotalsLabel.PanelSides := [fsdRight, fsdTop,fsdBottom];

  HighLightSplit(pRowLabel);
end;

procedure TfrmSplitterPad.pTotalsLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  pRowLabel.PanelSides := [fsdLeft, fsdTop, fsdBottom];
  pTotalsLabel.PanelSides := [fsdRight, fsdLeft, fsdTop, fsdBottom];

  HighLightSplit(pTotalsLabel);
end;

procedure TfrmSplitterPad.pTaskInfoLabelMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  HighLightSplit(pTaskInfoLabel);
end;


function TfrmSplitterPad.SplitAfterFilterIsPosible: boolean;
begin
  result := FFilterAvail and
    (FUnitMarkerAvail or FColumnsAvail or FRowsAvail or
     FTotalsAvail or FSheetIDAvail);
end;

function TfrmSplitterPad.SplitAfterUnitMarkerIsPosible: boolean;
begin
  result := FUnitMarkerAvail and
    (FColumnsAvail or FRowsAvail or FTotalsAvail or FSheetIDAvail);
end;

function TfrmSplitterPad.SplitAfterColumnTitlesIsPosible: boolean;
begin
  result := FColumnTitlesAvail;
end;

function TfrmSplitterPad.SplitAfterColumnsIsPosible: boolean;
begin
  result := FColumnsAvail and (FRowsAvail or FTotalsAvail or FSheetIDAvail);
end;

function TfrmSplitterPad.SplitAfterTotalTitlesIsPosible: boolean;
begin
  result := (FRowTitlesAvail or FTotalTitlesAvail);
end;

function TfrmSplitterPad.SplitAfterTotalsIsPosible: boolean;
begin
  result := (FRowsAvail or FTotalsAvail) and (FSheetIDAvail);
end;


procedure TfrmSplitterPad.SelectName(AName: string);
begin
  if Assigned(FExcelSheet) then
    try
      GetRangeByName(FExcelSheet, BuildExcelName(AName)).Select;
    except
    end;
end;


{выделение фильтров}
procedure TfrmSplitterPad.pFilterLabelClick(Sender: TObject);
begin
  SelectName(sntFilterArea);
end;


{Выделение заголовков столбцов}
procedure TfrmSplitterPad.pColumnTitlesLabelClick(Sender: TObject);
begin
  SelectName(sntColumnTitles);
end;

{выделение столбцов}
procedure TfrmSplitterPad.pColumnsLabelClick(Sender: TObject);
begin
  SelectName(sntColumns);
end;


{Выделяем заголовки строк}
procedure TfrmSplitterPad.pRowTitlesLabelClick(Sender: TObject);
begin
  SelectName(sntRowTitles);
end;


{выделение строк}
procedure TfrmSplitterPad.pRowLabelClick(Sender: TObject);
begin
  SelectName(sntRows);
end;


{Выделяем заголовки показателей}
procedure TfrmSplitterPad.pTotalTitlesLabelClick(Sender: TObject);
begin
  SelectName(sntTotalTitles);
end;


{выделение данных показателя}
procedure TfrmSplitterPad.pTotalsLabelClick(Sender: TObject);
begin
  SelectName(sntTotals);
end;

{описание задачи}
procedure TfrmSplitterPad.pTaskInfoLabelClick(Sender: TObject);
begin
  SelectName(sntSheetId);
end;

{выделение юнит-маркера}
procedure TfrmSplitterPad.pUnitMarkerLayerClick(Sender: TObject);
begin
  SelectName(sntUnitMarker);
end;

procedure TfrmSplitterPad.btCloseMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  btClose.Color := $00B59285;
  btClose.FrameColor := $006B2408;
end;

procedure TfrmSplitterPad.btCloseMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  btClose.Color := clGray;
  btClose.FrameColor := clGray;
end;


{Вставка разрыва после фильтров}
procedure TfrmSplitterPad.pSplit1MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterFilter);
end;

{Вставка разрыва после юнит-маркера}
procedure TfrmSplitterPad.pSplit2MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterUnitMarker);
end;

{Вставка разрыва заголовков столбцов}
procedure TfrmSplitterPad.pSplit3MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterColumnTitles);
end;

{Вставка разрыва после столбцов}
procedure TfrmSplitterPad.pSplit4MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterColumn);
end;

{Вставка разрыва после заголовков строк/показателй}
procedure TfrmSplitterPad.pSplit5MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterTotalTitles);
end;

{Вставка разрыва после строк показателей}
procedure TfrmSplitterPad.pSplit6MouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    InsertSplitter(sstAfterTotals);
end;

function TfrmSplitterPad.GetSplitterName(SplitterType: TSheetSplitterType): string;
begin
  case SplitterType of
    sstAfterFilter: result := sntFiltersBreak;
    sstAfterUnitMarker: result := sntUnitMarkerBreak;
    sstAfterColumnTitles: result := sntColumnTitlesBreak;
    sstAfterColumn: result := sntColumnsBreak;
    sstAfterTotalTitles: result := sntRowTitlesBreak;
    sstAfterTotals: result := sntRowsBreak;
  end;
end;

{Возвращает диапазон самого разрыва}
function TfrmSplitterPad.GetSplitterRange(SplitterType: TSheetSplitterType): ExcelRange;
var
  SplitterName: string;
begin
  if not Assigned(FExcelSheet) then
    exit;
  SplitterName := GetSplitterName(SplitterType);
  result := GetRangeByName(FExcelSheet, BuildExcelName(SplitterName));
end;

{Возвращает диапазон под место разрыва}
function TfrmSplitterPad.GetInsertionPoint(SplitterType: TSheetSplitterType): ExcelRange;
begin
  if not Assigned(FExcelSheet) then
    exit;
  case SplitterType of
    sstAfterFilter: result := GetRangeByName(FExcelSheet, BuildExcelName(sntFilterArea));
    sstAfterUnitMarker: result := GetRangeByName(FExcelSheet, BuildExcelName(sntUnitMarker));
    sstAfterColumnTitles: result := GetRangeByName(FExcelSheet, BuildExcelName(sntColumnTitles));
    sstAfterColumn: result := GetRangeByName(FExcelSheet, BuildExcelName(sntColumnsArea));
    sstAfterTotalTitles: begin
      result := GetRangeByName(FExcelSheet, BuildExcelName(sntRowTitles));
      if not Assigned(result) then
        result := GetRangeByName(FExcelSheet, BuildExcelName(sntTotalTitles));
    end;
    sstAfterTotals: result := GetRangeByName(FExcelSheet, BuildExcelName(sntRowsTotalsArea));
  else
    result := nil;
  end;
end;

procedure TfrmSplitterPad.InsertSplitter(SplitterType: TSheetSplitterType);

  procedure InsertBreak;
  var
    InsertionPoint, RowForInsert, SplitterRange: ExcelRange;
    TopSplitRow, BottomSplitRow: integer;
    AlreadySplitter: boolean;
    SplitterName: string;
  begin
    {проверяем есть ли уже такой разрыв}
    AlreadySplitter := Assigned(GetSplitterRange(SplitterType));
    if AlreadySplitter then
    begin
      {новый диапазон цепляем к старому}
      SplitterRange := GetSplitterRange(SplitterType);
      if not Assigned(SplitterRange) then
        exit;
      TopSplitRow := SplitterRange.Row;
    end
    else
    begin
      InsertionPoint := GetInsertionPoint(SplitterType);
      if not Assigned(InsertionPoint) then
        exit;
      TopSplitRow := InsertionPoint.Row + InsertionPoint.Rows.Count;
    end;

    if not SetSheetProtection(FExcelSheet, false) then
      exit;

    RowForInsert := GetRange(FExcelSheet, TopSplitRow, 1, TopSplitRow, FExcelSheet.Columns.Count);
    {$WARNINGS OFF}
    RowForInsert.Insert(xlShiftDown, 0);
    {$WARNINGS ON}
    RowForInsert := GetRange(FExcelSheet, TopSplitRow, 1, TopSplitRow, FExcelSheet.Columns.Count);
    RowForInsert.Clear;

    SplitterName := GetSplitterName(SplitterType);

    if AlreadySplitter then
    begin
      BottomSplitRow := TopSplitRow + SplitterRange.Rows.Count;
      SplitterRange := GetRange(FExcelSheet, TopSplitRow, 1, BottomSplitRow, FExcelSheet.Columns.Count);
      MarkObject(FExcelSheet, SplitterRange, SplitterName, false);
      SplitterRange.Style.Locked := false;
    end
    else
      MarkObject(FExcelSheet, RowForInsert, SplitterName, false);

    {Для большей наглядности выделим разрыв}
    if AlreadySplitter then
      SplitterRange.Select
    else
      RowForInsert.Select;
  end;

var
  TableRange: ExcelRange;
begin
  if Assigned(FExcelSheet) then
  begin
    TableRange := GetRangeByName(FExcelSheet, BuildExcelName(sntTable));
    if Assigned(TableRange) then
      try
        InsertBreak;
      except
      end;
  end;
end;


function TfrmSplitterPad.LaunchToolButton: CommandBarButton;
var
  ToolBar: CommandBar;
begin
  {$WARNINGS OFF}
  try
    ToolBar := FExcelSheet.Application.CommandBars.Get_Item(kriToolbarCaption);
    result := ToolBar.FindControl(msoControlButton, EmptyParam,
      tagToolButtonSplitterPad, EmptyParam, msoFalse) as CommandBarButton;
  except
    result := nil;
  end;
  {$WARNINGS ON}
end;

function TfrmSplitterPad.LaunchMenuButton: CommandBarButton;
begin
  {$WARNINGS OFF}
  try
    with FExcelSheet.Application.CommandBars.ActiveMenuBar do
      result := FindControl(msoControlButton, EmptyParam, tagMenuSplitterPad,
        EmptyParam, msoTrue) as CommandBarButton;
  except
    result := nil;
  end;
  {$WARNINGS ON}
end;

function TfrmSplitterPad.Get_LaunchButtonState: OleVariant;
var
  LaunchButton: CommandBarButton;
begin
  result := msoButtonUp;
  LaunchButton := LaunchToolButton;
  if Assigned(LaunchButton) then
    result := LaunchButton.State;
end;


procedure TfrmSplitterPad.Set_LaunchButtonState(AValue: OleVariant);
var
  LaunchButton: CommandBarButton;
begin
  LaunchButton := LaunchToolButton;
  if Assigned(LaunchButton) then
    LaunchButton.Set_State(AValue);

  LaunchButton := LaunchMenuButton;
  if Assigned(LaunchButton) then
    LaunchButton.Set_State(AValue);
end;

procedure TfrmSplitterPad.FormShow(Sender: TObject);
begin
  Validate;
  {$WARNINGS OFF}
  LaunchButtonState := msoButtonDown;
  {$WARNINGS ON}
end;

procedure TfrmSplitterPad.FormHide(Sender: TObject);
begin
  {$WARNINGS OFF}
  LaunchButtonState := msoButtonUp;
  {$WARNINGS ON}
end;

end.
