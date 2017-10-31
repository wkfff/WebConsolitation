unit uOfflineFiltersEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, ExtCtrls, StdCtrls, ImgList, uAppendEmptyRowToSheet,
  uSheetObjectModel, uFMAddinXMLUtils, uFMAddinGeneralUtils, MSXML2_TLB,
  uXMLUtils, uGlobalPlaningConst, ExcelXP, uFMAddinExcelUtils, uExcelUtils,
  uCheckTV2, AddinMembersTree;

type
  TfmOfflineFiltersEditor = class(TForm)
    btnOK: TButton;
    btnCancel: TButton;
    Panel1: TPanel;
    tcFilters: TTabControl;
    ImgList: TImageList;
    procedure tcFiltersChange(Sender: TObject);
  private
    Tree: TAddinMembersRadioTree;
    SheetInterface: TSheetInterface;
    FiltersIndices: TIntegerArray;
    FiltersCount: integer;
    FiltersTexts: TStringList;
    FiltersOk: array of boolean;  // единственность выбора
    procedure InitTabControl(Id: string);
    function MayCheck(Sender: tObject): boolean;
  public
    function MakeHistoryString: string;
  end;


  function EditFiltersOffline(SheetInterface: TSheetInterface; Id: string;
    out HString: string): boolean;

implementation

{$R *.DFM}

function EditFiltersOffline(SheetInterface: TSheetInterface; Id: string;
  out HString: string): boolean;
var
  fmEditor: TfmOfflineFiltersEditor;
begin
  fmEditor := TfmOfflineFiltersEditor.Create(nil);
  HString := '';
  try
    fmEditor.SheetInterface := SheetInterface;
    fmEditor.InitTabControl(Id);
    result := fmEditor.ShowModal = mrOK;
    if result then
    begin
      SheetInterface.UpdateFiltersText;
      SheetInterface.UpdateTotalsComments;
      HString := fmEditor.MakeHistoryString;
    end;
  finally
    fmEditor.Free;
  end;
end;

procedure TfmOfflineFiltersEditor.InitTabControl(Id: string);
var
  i, FilterIndex: integer;
  Filter: TSheetFilterInterface;
begin
  tcFilters.Tabs.Clear;
  FiltersCount := 0;
  SetLength(FiltersIndices, FiltersCount);
  SetLength(FiltersOk, FiltersCount);
  if Assigned(FiltersTexts) then
    FiltersTexts.Clear
  else
    FiltersTexts := TStringList.Create;
  for i := 0 to SheetInterface.Filters.Count - 1 do
  begin
    Filter := SheetInterface.Filters[i];
    if not Filter.MayBeEdited then
      continue;
    tcFilters.Tabs.Add(Filter.GetElementCaption);
    inc(FiltersCount);
    SetLength(FiltersIndices, FiltersCount);
    FiltersIndices[FiltersCount - 1] := i;
    FiltersTexts.Add(Filter.GetFilterDescription(true));
    { Вообще говоря, сам механизм создания формы сбора обеспечивает
      единственность выделенного в дереве элемента, так что эта инициализация,
      по большому счету, лишняя.}
    SetLength(FiltersOk, FiltersCount);
    FiltersOk[FiltersCount - 1] := Pos(FSD, FiltersTexts[FiltersCount - 1]) = 0;
  end;
  if not Assigned(Tree) then
  begin
    Tree := TAddinMembersRadioTree.Create(Self);
    Tree.MayCheck := Self.MayCheck;
    Tree.Parent := tcFilters;
    Tree.Align := alClient;
    Tree.Images := imgList;
    Tree.SetImageIndices(34, 21, 37, 31);
  end;
  if FiltersCount > 0 then
  begin
    FilterIndex := SheetInterface.Filters.FindById(Id);
    if FilterIndex > -1 then
      for i := 0 to FiltersCount - 1 do
        if FiltersIndices[i] = FilterIndex then
        begin
          tcFilters.TabIndex := i;
          break;
        end;
    tcFiltersChange(tcFilters);
  end;
end;

function TfmOfflineFiltersEditor.MakeHistoryString: string;
var
  List: TStringList;
  i: integer;
  Filter: TSheetFilterInterface;
  NewText: string;
begin
  List := TStringList.Create;
  result := '';
  try
    for i := 0 to FiltersCount - 1 do
    begin
      Filter := SheetInterface.Filters[FiltersIndices[i]];
      NewText := Filter.GetFilterDescription(true);
      if NewText <> FiltersTexts[i] then
        List.Add(Format('Значение фильтра %s изменено на "%s"',
          [Filter.GetElementCaption, NewText]));
    end;
    result := List.CommaText;
  finally
    FreeStringList(List);
  end;
end;

function TfmOfflineFiltersEditor.MayCheck(Sender: TObject): boolean;
var
  Node: TBasicCheckTreeNode;
  XmlNode: IXMLDOMNode;
begin
  Node := TBasicCheckTreeNode(Sender);
  XmlNode := Node.RelatedDomNode;
  {Такая инверсная конструкция используется для сохранения возможности
    редактировать фильтры в ранее созданных формах сбора, в которых
    атрибут attrForbidCheck отсутствует.}
  result := not GetBoolAttr(XmlNode, attrForbidCheck, false);
end;

procedure TfmOfflineFiltersEditor.tcFiltersChange(Sender: TObject);
var
  FilterIndex, i: integer;
  Filter: TSheetFilterInterface;
  List: TStringList;
  LevelsNL: IXMLDOMNodeList;
begin
  FilterIndex := FiltersIndices[tcFilters.TabIndex];
  Filter := SheetInterface.Filters[FilterIndex];
  List := TStringList.Create;
  try
    LevelsNL := Filter.Members.selectNodes('function_result/Levels/Level');
    for i := 0 to LevelsNL.length - 1 do
      List.Add(GetStrAttr(LevelsNL[i], attrName, ''));
    Tree.Load(Filter.Members);
  finally
    FreeStringList(List);
  end;
end;

end.

