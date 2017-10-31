unit uDataCollectionFormWizard;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, ExtCtrls, StdCtrls, fmWizardHeader, AddinLevelsList, ExcelXP,
  uSheetObjectModel, uFMAddinExcelUtils, uFMAddinGeneralUtils, uOfficeUtils,
  uGlobalPlaningConst, ImgList, AddinMembersTree, MSXML2_TLB, uXMLUtils,
  uXMLCatalog, PlaningTools_TLB, ComObj, uFMAddinXmlUtils, uAbstractWizard;

type
  TfmDataCollectionFormWizard = class(TfmAbstractWizard)
    tsSheetInfo: TTabSheet;
    tsTotals: TTabSheet;
    lvTotals: TListView;
    memSheetInfo: TMemo;
    Label1: TLabel;
    tsFilters: TTabSheet;
    Label2: TLabel;
    lvFilters: TListView;
    Label3: TLabel;
    tsMembers: TTabSheet;
    Panel1: TPanel;
    MembersTree: TAddinMembersTree;
    ImgList: TImageList;
    tsFiltersResume: TTabSheet;
    lvFiltersResume: TListView;
    lvSheetInfo: TListView;
    Label4: TLabel;
    tsDone: TTabSheet;
    memDone: TMemo;
    WarningLabel: TLabel;
    procedure tsSheetInfoShow(Sender: TObject);
    procedure tsTotalsShow(Sender: TObject);
    procedure tsFiltersShow(Sender: TObject);
    procedure tsMembersShow(Sender: TObject);
    procedure tsFiltersResumeShow(Sender: TObject);
    procedure tsDoneShow(Sender: TObject);

    procedure lvTotalsMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure lvFiltersMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormResize(Sender: TObject);
  private
    History: TStringList;
    { ���������� ��������, ���������� ���������}
    FilterPagesCount: integer;
    { ���������� ����� �������������� �������, ���������� � 1}
    CurrentFilterIndex: integer;

    { �������������� �������� �� ����������� ����� }
    function IsSheetSuitable: boolean;
    function GetCurrentFilter: TSheetFilterInterface;
    function CheckFilterSelection(Filter: TSheetFilterInterface;
      out Error: string): boolean;

  protected
    procedure Back; override;
    procedure Next; override;
    procedure Done; override;

  public
    destructor Destroy; override;
  end;

  function MakeDataCollectionForm(ASheetInterface: TSheetInterface;
    out HistoryString: string): boolean;

implementation

{$R *.DFM}

const
  capFirstPage = '������ ��������� �����';
  comFirstPage = '�� ������ ������������ ����� ���� ������ ������� �����������' +
    #13 + '����� ����� ������.';
  capTotals = '����� �����������';
  comTotals = '������� ����������, ��������������� ��� ��������������' +
    #13 + ' � ���������� ������.';
  capFilters = '����� ��������';
  comFilters = '������� �������, ��������������� ��� ��������������' +
    #13 + ' � ���������� ������.';
  capMembers = '��������� ������� "%s" (%d �� %d)';
  comMembers = '������� ������������ ���������, ������� ����� ��������������' +
    #13 + ' ��� �������������� ������� � ���������� ������.';
  capFiltersResume = '������ ������ ��������';
  comFiltersResume = '��������� ��������� ������, ���������� �� ������' +
    #13 + ' ������ ��������� ��������.';
  capDone = '������';
  comDone = '������ ������ ��� ����������� ��� ������ ����������.';

  (*txtRightSheet = '������ ���� ����� ���� ������������ � ����� ����� ������.';
  txtWrongSheet = '������ ���� �� ����� ���� ������ ����� ������ �� ��������� �������:';*)

  txtSheetOrigin = '������������� �����';
  txtPlaningSheet = '���� �������� � ������� ���������� MS Office';
  txtForeignSheet = '��� �� ���� ���������� MS Office.' +
    ' ��� �������������� � ����� ����� ������ �������' +
    ' ������������ ������� ����������� ����.';

  txtSheetType = '����������� �������� ������';//'��� ��������� ��������� �������� ������';
  txtTypeRight = '��, ��� ��������� ��������� �������� ������';
  txtTypeWrong = '���, ��� ��������� �� ������������ �����������' +
      ' ������������� �������� ������';

  txtResults = '������� ����������� ���� "��������� �������"';
  txtResultsYes = '����� ���������� ������������';
  txtResultsNo = '����� ���������� �����������.' +
    ' ��� ����� ����� ������ ���������� ������� ���� �� ������ ������ ����������.';

  txtFilters = '������� ��������';
  txtFiltersYes = '������� ������������';
  txtFiltersNo = '������� �����������.' +
    ' �������� ������ �� ����� �������������� ��� ������� ��������.';

  txtDoneFirstLine = '���������� � �������������� ����� � ����� ����� ������ ���������.';
  txtDoneSecondLine = '���� �� ��������, ��� ������� ������������� ������ �����-���� ' +
    '�����������, �� ������ ��������� ����� � �������� ���� �����. ' +
    '���� �� ��� ������� ���������, �� �� ������� ������ "������" ' +
    '���� ����� ��������� � ���������� ����� ������ � ������������ � ' +
    '����� ����� ������.';
  txtDoneThirdLine = '��������: ������ �������� �������� �����������! ' +
    '������������ �������� ���� �� ����� ����� ������!';


  GreenMark = 35;
  RedMark = 36;

function MakeDataCollectionForm(ASheetInterface: TSheetInterface;
    out HistoryString: string): boolean;
var
  fmWizard: TfmDataCollectionFormWizard;
begin
  fmWizard := TfmDataCollectionFormWizard.Create(ASheetInterface);
  try
    with fmWizard do
    begin
      UseDefaultValueInXml := true;
      pcMain.ActivePage := tsSheetInfo;
      History := TStringList.Create;
      MembersTree.WithDefaultValue := true;
      result := ShowModal = mrOk;
      if result then
        HistoryString := History.CommaText;
    end;
  finally
    FreeAndNil(fmWizard);
  end;
end;

function IsThereCheckedItems(ListView: TListView): boolean;
var
  i: integer;
begin
  result := false;
  for i := 0 to ListView.Items.Count - 1 do
    if ListView.Items[i].Checked then
    begin
      result := true;
      exit;
    end;
end;

function GetCheckedCount(ListView: TListView): integer;
var
  i: integer;
begin
  result := 0;
  for i := 0 to ListView.Items.Count - 1 do
    if ListView.Items[i].Checked then
      inc(result);
end;

{ TfmDataCollectionFormWizard }

function TfmDataCollectionFormWizard.IsSheetSuitable: boolean;
begin
  lvSheetInfo.Items.Clear;

  with lvSheetInfo.Items.Add do
  begin
    Caption := txtSheetOrigin;
    if IsPlaningSheet(ExcelSheet) then
    begin
      ImageIndex := GreenMark;
      SubItems.Append(txtPlaningSheet);
      result := true;
    end
    else
    begin
      ImageIndex := RedMark;
      SubItems.Append(txtForeignSheet);
      result := false;
    end;
  end;

  with lvSheetInfo.Items.Add do
  begin
    Caption := txtSheetType;
    if not result or (GetWBCustomPropertyValue(
      ExcelSheet.Application.ActiveWorkbook.CustomDocumentProperties, pspSheetType) = '2') then
    begin
      ImageIndex := RedMark;
      SubItems.Append(txtTypeWrong);
      result := false;
    end
    else
    begin
      ImageIndex := GreenMark;
      SubItems.Append(txtTypeRight);
      result := result and true;
    end;
  end;

  with lvSheetInfo.Items.Add do
  begin
    Caption := txtResults;
    if SheetInterface.Totals.CheckByType([wtResult]) then
    begin
      ImageIndex := GreenMark;
      SubItems.Append(txtResultsYes);
      result := result and true;
    end
    else
    begin
      ImageIndex := RedMark;
      SubItems.Append(txtResultsNo);
      result := false;
    end;
  end;

  with lvSheetInfo.Items.Add do
  begin
    Caption := txtFilters;
    if SheetInterface.Filters.Empty then
    begin
      ImageIndex := RedMark;
      SubItems.Append(txtFiltersNo);
      result := false;
    end
    else
    begin
      ImageIndex := GreenMark;
      SubItems.Append(txtFiltersYes);
      result := result and true;
    end;
  end;
end;

procedure TfmDataCollectionFormWizard.tsSheetInfoShow(Sender: TObject);
begin
  SetupHeader(capFirstPage, comFirstPage);
  if MovingForward then
    SetupButtons(false, IsSheetSuitable, false)
  else
    SetupButtons(false, true, false);
end;

procedure TfmDataCollectionFormWizard.tsTotalsShow(Sender: TObject);

  procedure AddTotal(Total: TSheetBasicTotal; ImgIndex: integer);
  begin
    with lvTotals.Items.Add do
    begin
      Data := Total;
      Caption := Total.GetElementCaption;
      SubItems.Append(IIF(Total.TotalType = wtResult,
        '���: "' + Total.CubeName + '" ����: "' + Total.MeasureName + '"',
         '��������� ����������'));
      ImageIndex := ImgIndex;
      Checked := Total.PermitEditing;
    end;
  end;

var
  i: integer;
  Total: TSheetBasicTotal;
begin
  SetupHeader(capTotals, comTotals);
  if MovingForward then
  begin
    SetupButtons(true, false, false);
    lvTotals.Items.Clear;

    for i := 0 to SheetInterface.Totals.Count - 1 do
    begin
      Total := SheetInterface.Totals[i];
      if not (Total.TotalType in [wtResult, wtFree]) then
        continue;
      AddTotal(Total, 1);
    end;

    (*//���������������� ���������...
    for i := 0 to SheetInterface.SingleCells.Count - 1 do
    begin
      Total := SheetInterface.SingleCells[i];
      if Total.TotalType <> wtResult then
        continue;
      AddTotal(Total, 39);
    end;*)

    lvTotalsMouseUp(lvTotals, mbLeft, [], 0, 0);
  end
  else
    SetupButtons(true, true, false);
end;

procedure TfmDataCollectionFormWizard.tsFiltersShow(Sender: TObject);

  function AffectsSelectedTotals(Filter: TSheetFilterInterface): boolean;
  var
    i: integer;
  begin
    result := false;
    for i := 0 to lvTotals.Items.Count - 1 do
      if lvTotals.Items[i].Checked then
        if TSheetElement(lvTotals.Items[i].Data).GetObjectType = wsoTotal then
          if Filter.IsAffectsTotal(lvTotals.Items[i].Data) then
          begin
            result := true;
            exit;
          end;
  end;

var
  i{, j}: integer;
  Filter: TSheetFilterInterface;
  //SingleCell: TSheetSingleCellInterface;
  Op: IOperation;
begin
  SetupHeader(capFilters, comFilters);
  if MovingForward then
  begin
    SetupButtons(true, false, false);
    lvFilters.Items.Clear;
    Application.ProcessMessages;
    Op := CreateComObject(CLASS_Operation) as IOperation;
    Op.Caption := '�������� ��������';
    Op.StartOperation(Handle);
    try
      { ������� ��������� �����������}
      for i := 0 to SheetInterface.Filters.Count - 1 do
      begin
        Filter := SheetInterface.Filters[i];
        { �� �������������� ������ ���� ������� ������� �� �������������,
          ��������� � ���������� ������ ��� �� ��������}
        if AffectsSelectedTotals(Filter) and not Filter.IsPartial then
        begin
          with lvFilters.Items.Add do
          begin
            Data := Filter;
            Caption := Filter.GetElementCaption;
            ImageIndex := IIF(Filter.IsPartial, 30, 22);
            Checked := Filter.PermitEditing;
          end;
          LoadXML(Filter);
        end
        else
          Filter.PermitEditing := false;
      end;

      WarningLabel.Visible := lvFilters.Items.Count = 0;
      lvFilters.Enabled := lvFilters.Items.Count > 0;
      
      { ������� ��������� �����������}
      (*for i := 0 to lvTotals.Items.Count - 1 do
        if lvTotals.Items[i].Checked then
          if TSheetElement(lvTotals.Items[i].Data).GetObjectType = wsoSingleCell then
          begin
            SingleCell := TSheetSingleCellInterface(lvTotals.Items[i].Data);
            for j := 0 to SingleCell.Filters.Count - 1 do
              with lvFilters.Items.Add do
              begin
                Data := Filter;
                Caption := Filter.GetElementCaption;
                ImageIndex := IIF(Filter.IsPartial, 3, 2);
                Checked := Filter.PermitEditing;
              end
          end; *)
    finally
      Op.StopOperation;
    end;
    lvFiltersMouseUp(lvFilters, mbLeft, [], 0, 0);
  end
  else
    SetupButtons(true, true, false);

  CurrentFilterIndex := 0;
end;

procedure TfmDataCollectionFormWizard.tsMembersShow(Sender: TObject);
var
  Filter: TSheetFilterInterface;
  Dom: IXMLDOMDocument2;
  Dimension: TDimension;
  Hierarchy: THierarchy;
  Xml: string;
begin
  Filter := GetCurrentFilter;
  SetupHeader(Format(capMembers, [Filter.GetElementCaption,
    CurrentFilterIndex, FilterPagesCount]), comMembers);
  SetupButtons(true, true, false);
  Xml := LoadXml(Filter);
  GetDomDocument(Dom);
  Dom.loadXml(Xml);
  Dimension := SheetInterface.XMLCatalog.Dimensions.Find(Filter.Dimension, Filter.ProviderId);
  Hierarchy := Dimension.GetHierarchy(Filter.Hierarchy);
  MembersTree.Clear;
  MembersTree.Load(Dom, Hierarchy.Levels.ToString, Hierarchy.CodeToShow);
end;

procedure TfmDataCollectionFormWizard.tsFiltersResumeShow(Sender: TObject);
var
  i: integer;
  Filter: TSheetFilterInterface;
  Xml, tmpXml, Error: string;
  Ok: boolean;
begin
  SetupHeader(capFiltersResume, comFiltersResume);
  if MovingForward then
  begin
    Ok := true;
    lvFiltersResume.Items.Clear;
    for i := 0 to lvFilters.Items.Count - 1 do
      if lvFilters.Items[i].Checked then
      begin
        Filter := lvFilters.Items[i].Data;
        tmpXml := Filter.Members.xml;
        Xml := LoadXml(Filter);
        Filter.Members.loadXML(Xml);
        with lvFiltersResume.Items.Add do
        begin
          Caption := Filter.GetElementCaption;
          Data := Filter;
          if CheckFilterSelection(Filter, Error) then
          begin
            ImageIndex := GreenMark;
            SubItems.Append('OK');
          end
          else
          begin
            Ok := false;
            ImageIndex := RedMark;
            SubItems.Append(Format(Error, ['']));
          end;
        end;
        Filter.Members.loadXML(tmpXml);
      end;
    SetupButtons(true, Ok, false);
  end
  else
    SetupButtons(true, true, false);
end;

procedure TfmDataCollectionFormWizard.tsDoneShow(Sender: TObject);
begin
  SetupHeader(capDone, comDone);
  SetupButtons(true, false, true);
end;

destructor TfmDataCollectionFormWizard.Destroy;
begin
  inherited;
  FreeStringList(History);
end;

procedure TfmDataCollectionFormWizard.lvTotalsMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);

  function MayProceedWithCheckedTotals: boolean;
  var
    i: integer;
  begin
    result := false;
    for i := 0 to lvTotals.Items.Count - 1 do
      if lvTotals.Items[i].Checked then
        if TSheetTotalInterface(lvTotals.Items[i].Data).TotalType = wtResult then
        begin
          result := true;
          exit;
        end;
  end;

var
  ListItem: TListItem;
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
  if (Button = mbleft) then
  begin
    btnNext.Enabled := MayProceedWithCheckedTotals;
    ListItem := (Sender as TListView).GetItemAt(X, Y);
    if Assigned(ListItem) then
      TSheetBasicTotal(ListItem.Data).PermitEditing := ListItem.Checked;
  end;
end;

procedure TfmDataCollectionFormWizard.lvFiltersMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  ListItem: TListItem;
begin
  ListViewOnMouseUpChangeCheckState(Sender, Button, Shift, X, Y);
  if (Button = mbleft) then
  begin
    btnNext.Enabled := true;
    ListItem := (Sender as TListView).GetItemAt(X, Y);
    if Assigned(ListItem) then
      TSheetFilterInterface(ListItem.Data).PermitEditing := ListItem.Checked;
  end;
end;

function TfmDataCollectionFormWizard.GetCurrentFilter: TSheetFilterInterface;
var
  i, Cnt: integer;
begin
  result := nil;
  Cnt := 0;
  for i := 0 to lvFilters.Items.Count - 1 do
    if lvFilters.Items[i].Checked then
    begin
      inc(Cnt);
      if Cnt = CurrentFilterIndex then
      begin
        result := lvFilters.Items[i].Data;
        exit;
      end;
    end;
end;

function TfmDataCollectionFormWizard.CheckFilterSelection(
  Filter: TSheetFilterInterface; out Error: string): boolean;
var
  NL: IXMLDOMNodeList;
begin
  result := false;

  NL := Filter.Members.selectNodes('function_result/Members//Member[@checked="true"]');
  case NL.length of
    0:
      begin
        Error := '� ������� �� ������� �� ������ ��������';
        exit;
      end;
    1:
      begin
        Error := '� ������� ������ ������������ �������, ������������ �� ������ ������������� �����';
        exit;
      end;
    else
      begin
      end;
  end;

  result := true;
end;

procedure TfmDataCollectionFormWizard.FormResize(Sender: TObject);
begin
  memDone.Lines.BeginUpdate;
  memDone.Lines.Clear;
  memDone.Lines.Add(txtDoneFirstLine);
  memDone.Lines.Add('');
  memDone.Lines.Add(txtDoneSecondLine);
  memDone.Lines.Add('');
  memDone.Lines.Add(txtDoneThirdLine);
  memDone.Lines.EndUpdate;
end;

procedure TfmDataCollectionFormWizard.Back;
begin
  inherited Back;
  case CurrentPage.PageIndex of
    1: // ����������
      CurrentPage := tsSheetInfo;
    2: // �������
      CurrentPage := tsTotals;
    3: // �������� ��������
      begin
        SaveXML(GetCurrentFilter, MembersTree.MembersDOM);
        if CurrentFilterIndex > 1 then
        begin
          dec(CurrentFilterIndex);
          tsMembersShow(nil);
        end
        else
          CurrentPage := tsFilters;
      end;
    4: // ������ ��������
      CurrentPage := tsMembers;
    5: // ������
      if IsThereCheckedItems(lvFilters) then
        CurrentPage := tsFiltersResume
      else
        CurrentPage := tsFilters;
  end;
end;

procedure TfmDataCollectionFormWizard.Next;
begin
  inherited Next;
  case CurrentPage.PageIndex of
    0:  //  ������ ��������� �����
      CurrentPage := tsTotals;
    1: // ����������
      CurrentPage := tsFilters;
    2: // �������
      if IsThereCheckedItems(lvFilters) then
      begin
        CurrentFilterIndex := 1;
        FilterPagesCount := GetCheckedCount(lvFilters);
        CurrentPage := tsMembers;
      end
      else
        CurrentPage := tsDone;
    3: // �������� �������
      begin
        SaveXML(GetCurrentFilter, MembersTree.MembersDOM);
        if CurrentFilterIndex < FilterPagesCount then
        begin
          inc(CurrentFilterIndex);
          tsMembersShow(nil);
        end
        else
          CurrentPage := tsFiltersResume;
      end;
    4: // ������ ��������
      CurrentPage := tsDone;
  end;
end;

procedure TfmDataCollectionFormWizard.Done;
var
  i: integer;
  Filter: TSheetFilterInterface;
  Xml, tmpString: string;
begin
  { ���������� ��� ����� ������������ ���������� �� ��������������.
    � ����� ����� ����� ����� ������������� ������ �� ��������, �������
    ���� ���� ������� � ���� �������.}
  SheetInterface.SetDefaultPermissions;

  History.Add('��������� ���������� ����� �������� ��� ��������������:');
  for i := 0 to lvTotals.Items.Count - 1 do
    if lvTotals.Items[i].Checked then
      with TSheetTotalInterface(lvTotals.Items[i].Data) do
      begin
        PermitEditing := true;
        History.Add(SysUtils.Format(' - "%s";', [GetElementCaption]));
      end;
  tmpString := History.Strings[History.Count - 1];
  tmpString[Length(tmpString)] := '.';
  History.Strings[History.Count - 1] := tmpString;

  History.Add('��������� ������� ����� �������� ��� ��������������:');
  for i := 0 to lvFiltersResume.Items.Count - 1 do
  begin
    Filter := lvFiltersResume.Items[i].Data;
    if Filter.IsParam then
      Filter.Param.RemoveLink(Filter);
    Xml := LoadXML(Filter);
    Filter.Members.loadXML(Xml);
    Filter.PermitEditing := true;
    History.Add(Format(' - "%s";', [Filter.GetElementCaption]));
  end;
  tmpString := History.Strings[History.Count - 1];
  tmpString[Length(tmpString)] := '.';
  History.Strings[History.Count - 1] := tmpString;

  SheetInterface.PermitEditing := true;
end;

end.
