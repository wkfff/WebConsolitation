{Модальная форма с компонентом TMeasureTree для выбора меры}

unit uMeasureSelector;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls, AddinMeasuresTree, uXMLCatalog, ImgList,
  uFMExcelAddInConst, uFMAddinGeneralUtils, uGlobalPlaningConst;

type
  TfmMeasureSelector = class(TForm)
    MeasuresTree: TAddinMeasuresTree;
    btnOk: TButton;
    btnCancel: TButton;
    ImgList: TImageList;
  private
    procedure OnMeasureChange(Sender: TObject; Node: TTreeNode);
    procedure OnDblClick(Sender: TObject);
  public
  end;

  function SelectMeasure(Catalog: TXMLCatalog; ResultsOnly: boolean;
    out Measure: TMeasure; out Cube: TCube): boolean;

implementation


{$R *.DFM}

{ TfmMeasureSelector }

function SelectMeasure(Catalog: TXMLCatalog; ResultsOnly: boolean;
  out Measure: TMeasure; out Cube: TCube): boolean;
var
  fmMeasureSelector: TfmMeasureSelector;
begin
  fmMeasureSelector := TfmMeasureSelector.Create(nil);
  with fmMeasureSelector do
  begin
    result := false;
    Cube := nil;
    Measure := nil;
    MeasuresTree.Catalog := Catalog;
    MeasuresTree.Images := ImgList;
    MeasuresTree.OnChange := OnMeasureChange;
    MeasuresTree.OnDblClick := OnDblClick;
    try
      if not MeasuresTree.Load(ResultsOnly) then
      begin
        ShowError(ermMeasuresLoadFault);
        exit;
      end;
      if fmMeasureSelector.ShowModal = mrOK then
      begin
        Cube := MeasuresTree.Cube;
        Measure := MeasuresTree.Measure;
        result := true;
      end;
    finally
      FreeAndNil(fmMeasureSelector);
      Application.ProcessMessages;
    end;
  end;
end;

{ TfmMeasureSelector }

procedure TfmMeasureSelector.OnDblClick(Sender: TObject);
begin
  if btnOK.Enabled then
    btnOK.Click;
end;

procedure TfmMeasureSelector.OnMeasureChange(Sender: TObject;
  Node: TTreeNode);
begin
  btnOK.Enabled := MeasuresTree.IsMeasureSelected;
end;

end.
