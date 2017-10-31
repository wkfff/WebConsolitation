unit uAbstractWizard;

{ Абстрактный предок мастеров ReplicationWizard и DatacollectionFormWizard.
  Содержит общие элементы интерфейса (Пэйджконтрол, кнопки обхода),
  работу с хранилищем ХМЛ и другие подобные вещи.}

interface

uses
  Windows, SysUtils, Classes, Controls, Forms, 
  ComCtrls, ExtCtrls, StdCtrls, fmWizardHeader, ExcelXP,
  uSheetObjectModel, uFMAddinExcelUtils, uFMAddinGeneralUtils, uOfficeUtils,
  uGlobalPlaningConst, MSXML2_TLB, uXMLUtils,
  uXMLCatalog, uFMAddinXmlUtils;

type
  TfmAbstractWizard = class(TForm)
    pnButtons: TPanel;
    WHeader: TfmWizardHeader;
    pcMain: TPageControl;
    btnDone: TButton;
    btnCancel: TButton;
    btnNext: TButton;
    btnBack: TButton;
    Bevel1: TBevel;

    procedure btnBackClick(Sender: TObject);
    procedure btnNextClick(Sender: TObject);
    procedure btnDoneClick(Sender: TObject);

  private
    FSheetInterface: TSheetInterface;
    FMovingForward: boolean;
    FUseDefaultValueInXml: boolean;

    { Хранилище xml элементов}
    XmlDepot: TStringList;

    function GetCurrentPage: TTabSheet;
    procedure SetCurrentPage(NewPage: TTabSheet);

    function GetExcelSheet: ExcelWorkSheet;

  protected
    procedure Back; virtual;
    procedure Next; virtual;
    procedure Done; virtual; abstract;

    procedure SetupHeader(ACaption, AComment: string);
    procedure SetupButtons(bsBack, bsNext, bsDone: boolean);

    function LoadXML(SheetDimension: TSheetDimension): string;
    procedure SaveXML(SheetDimenion: TSheetDimension; Dom: IXMLDOMDocument2);

    property SheetInterface: TSheetInterface read FSheetInterface;
    property MovingForward: boolean read FMovingForward;
    property ExcelSheet: ExcelWorkSheet read GetExcelSheet;
    property CurrentPage: TTabSheet read GetCurrentPage write SetCurrentPage;
    property UseDefaultValueInXml: boolean read FUseDefaultValueInXml write FUseDefaultValueInXml;
  public
    constructor Create(ASheetInterface: TSheetInterface); reintroduce;
    destructor Destroy; override;
  end;

implementation

{$R *.DFM}

{ TfmAbstractWizard }

constructor TfmAbstractWizard.Create(ASheetInterface: TSheetInterface);
begin
  inherited Create(nil);
  FSheetInterface := ASheetInterface;
  FMovingForward := true;
  XmlDepot := TStringList.Create;
end;

destructor TfmAbstractWizard.Destroy;
begin
  inherited;
  //FreeStringList(History);
  FreeStringList(XmlDepot);
end;

function TfmAbstractWizard.GetCurrentPage: TTabSheet;
begin
  result := pcMain.ActivePage;
end;

procedure TfmAbstractWizard.SetCurrentPage(NewPage: TTabSheet);
begin
  pcMain.ActivePage := NewPage;
end;

function TfmAbstractWizard.GetExcelSheet: ExcelWorkSheet;
begin
  result := SheetInterface.ExcelSheet;
end;


procedure TfmAbstractWizard.SetupButtons(bsBack, bsNext, bsDone: boolean);
begin
  btnBack.Enabled := bsBack;
  btnNext.Enabled := bsNext;
  btnDone.Enabled := bsDone;
end;

procedure TfmAbstractWizard.SetupHeader(ACaption, AComment: string);
begin
  WHeader.Captions[0] := Acaption;
  WHeader.Comments[0] := AComment;
  WHeader.Repaint;
end;

procedure TfmAbstractWizard.btnBackClick(Sender: TObject);
begin
  Back;
end;

procedure TfmAbstractWizard.btnNextClick(Sender: TObject);
begin
  Next;
end;

procedure TfmAbstractWizard.btnDoneClick(Sender: TObject);
begin
  Done;
end;

function TfmAbstractWizard.LoadXML(SheetDimension: TSheetDimension): string;
var
  SheetDom, ProviderDom: IXMLDOMDocument2;
  Index: integer;
begin
  Index := XmlDepot.IndexOfName(SheetDimension.UniqueID);
  if Index < 0 then
  begin
    SheetDom := SheetDimension.Members;
    ProviderDom := SheetInterface.DataProvider.GetMemberList(
      SheetDimension.ProviderId, '', SheetDimension.Dimension, SheetDimension.Hierarchy, '', '');
    if (SheetInterface.DataProvider.LastError <> '') then
    begin
      ShowDetailError(ermMembersLoadFault, SheetInterface.DataProvider.LastError,
        ermMembersLoadFault);
      ProviderDom.load(SheetDom);
    end
    else
      CopyMembersState(SheetDom, ProviderDom, nil);
    FillNodeListAttribute(ProviderDom, 'function_result/Levels/Level', attrLevelState, 1);
    if UseDefaultValueInXml then
      SetAttr(ProviderDom.selectSingleNode(
        Format('function_result/Members//Member[@%s="true"]', [attrChecked])),
        attrDefaultValue, true);
    result := ProviderDom.xml;
    XmlDepot.Add(SheetDimension.UniqueID + '=' + result);
    KillDomDocument(ProviderDom);
  end
  else
    result := XmlDepot.Values[SheetDimension.UniqueID];
end;

procedure TfmAbstractWizard.SaveXML(SheetDimenion: TSheetDimension; Dom: IXMLDOMDocument2);
var
  Index: integer;
begin
  Index := XmlDepot.IndexOfName(SheetDimenion.UniqueID);
  XmlDepot[Index] := SheetDimenion.UniqueID + '=' + Dom.xml;
end;

procedure TfmAbstractWizard.Back;
begin
  FMovingForward := false;
end;

procedure TfmAbstractWizard.Next;
begin
  FMovingForward := true;
end;

end.


