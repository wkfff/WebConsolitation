{
  История листа.
  Формочка которая достает из недр листа его историю и отображает ее.
}

unit uSheetHistory;

interface

uses
  Windows, SysUtils, Forms, MSXML2_TLB, ComObj, ImgList, ComCtrls, StdCtrls,
  ExcelXP, uFMAddinExcelUtils, uXMLUtils, uFMAddinGeneralUtils, Classes,
  uFMExcelAddInConst, Controls, ExtCtrls, uExcelUtils, uGlobalPlaningConst;

const
  //имя функции в XML
  nFunction = 'Get_SheetHistory';

  //константы для сборки XPath
  xpEvent = 'event';

  //атрибуты события
  attrName = 'name';
  attrTypeIndex = 'typeindex';
  attrDate = 'date';
  attrID = 'id';
  attrUser = 'user';
  attrIsSuccess = 'issuccess';
  attrCounter = 'counter';

  //имена событий происходящих с листом
  enSheetRefresh = 'Обновление листа';
  enWriteBack = 'Обратная запись';
  enSheetEdit = 'Изменение структуры листа';
  enPropertyEdit = 'Изменение свойcтв листа';
  enUnknown = 'Изменение листа';
  enVersionUpdate = 'Обновление версии листа';
  enSheetCopy = 'Копирование листа';
  enParamsEdit = 'Изменение параметров';
  enTaskConnectionOn = 'Прикрепление к задаче';
  enTaskConnectionOff = 'Открепление от задачи';
  enConstsEdit = 'Изменение констант';

  //индексы иконок
  iiFault = 4;

type
  { Типы событий, заносимых в историю}
  TSheetEventType = (evtEdit, evtRefresh, evtWriteBack, evtPropertyEdit, evtUnknown, evtVersionUpdate,
                     evtSheetCopy, evtParamsEdit, evtTaskConnectionOn, evtTaskConnectionOff, evtConstsEdit);

  { Форма отображения истории }
  TSheetHistory = class(TForm)
    ImageList1: TImageList;
    Panel1: TPanel;
    Label3: TLabel;
    cbSheetList: TComboBox;
    Label1: TLabel;
    Splitter1: TSplitter;
    lwEventList: TListView;
    pDetailHolder: TPanel;
    Panel3: TPanel;
    btDeleteHistory: TButton;
    btExit: TButton;
    Panel2: TPanel;
    Label2: TLabel;
    mEventSpecification: TMemo;
    procedure cbSheetListChange(Sender: TObject);
    procedure btDeleteHistoryClick(Sender: TObject);
    procedure btExitClick(Sender: TObject);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
    procedure lwEventListSelectItem(Sender: TObject; Item: TListItem;
      Selected: Boolean);
    procedure FormDestroy(Sender: TObject);
  private
    FCounter: int64;
    FExcelApp: ExcelApplication;
    FActiveBook: ExcelWorkbook;
    FActiveExcelSheet: ExcelWorkSheet;
    FLCID: integer;
    FHistoryXML: IXMLDOMDocument2;
    function GetIndexActiveSheet: integer;
    procedure SetActiveExcelSheet(ExcelSheet: ExcelWorksheet);
    property ActiveExcelSheet: ExcelWorksheet read FActiveExcelSheet write SetActiveExcelSheet;
    property IndexActiveSheet: integer read GetIndexActiveSheet;
    // добавляем в ComboBox список листов в книге
    procedure AddItemsInSheetList;
    procedure PropertysInitialization(Application: ExcelApplication);
    procedure ClearForm;
    function GetTypeName(EventType: TSheetEventType): string;
    function GetTypeIndex(EventType: TSheetEventType): integer;
    function GetUniqueID: string;
    //достаем из CP листа события и отображаем
    procedure ViewEventList;
    //по ID достаём из XML дополнительную информацию выбранного события
    procedure ViewEventSpecification(ID: integer);
    function CreateHistoryDocument(ExcelSheet: ExcelWorksheet): IXMLDOMDocument2;
  public
    procedure ShowSheetHistory(Application: ExcelApplication);
    procedure AddEvent(ExcelSheet: ExcelWorkSheet; EventType: TSheetEventType;
      Comment: string; IsSuccess: boolean);
    procedure DeleteHistory(ExcelSheet: ExcelWorksheet);
    procedure SwitchLastEvents(ExcelSheet: ExcelWorkSheet);
  end;

implementation

{$R *.DFM}

{ TSheetHistory }

procedure TSheetHistory.PropertysInitialization(Application: ExcelApplication);
begin
  FExcelApp := Application;
  FActiveBook := FExcelApp.ActiveWorkbook;
  if Assigned(FActiveBook) then
    ActiveExcelSheet := GetWorkSheet(FActiveBook.ActiveSheet);
  FLCID := GetUserDefaultLCID;
end;

procedure TSheetHistory.ShowSheetHistory(Application: ExcelApplication);
begin
  if not Assigned(Application) then
    exit;
  ClearForm;
  PropertysInitialization(Application);
  AddItemsInSheetList;
  cbSheetList.ItemIndex := IndexActiveSheet;

  ShowModal;
end;

procedure TSheetHistory.ClearForm;
begin
  lwEventList.Items.Clear;
  mEventSpecification.Clear;
  cbSheetList.Items.Clear;
end;

procedure TSheetHistory.AddItemsInSheetList;
var
  i: integer;
  SheetCount: integer;
  ExcelSheet: ExcelWorkSheet;
begin
  cbSheetList.Items.Clear;

  if not((Assigned(FActiveBook)) or (FActiveBook.Sheets.Count = 0)) then
  begin
    cbSheetList.Items.Add('Листов не найдено');
    exit;
  end;

  SheetCount := FActiveBook.Sheets.Count;
  for i := 1 to SheetCount do
  begin
    ExcelSheet := GetWorkSheet(FActiveBook.Sheets.Item[i]);
    if Assigned(ExcelSheet) then
      cbSheetList.Items.Add(ExcelSheet.Name);
  end;
end;

function TSheetHistory.GetIndexActiveSheet: integer;
begin
  result := IIF(Assigned(ActiveExcelSheet), ActiveExcelSheet.Index[FLCID] - 1,
    0);
end;

procedure TSheetHistory.SetActiveExcelSheet(ExcelSheet: ExcelWorksheet);
begin
  if not Assigned(ExcelSheet) then
    exit;
  FActiveExcelSheet := ExcelSheet;
  FHistoryXML := GetDataFromCP(ActiveExcelSheet, cpSheetHistory);
  btDeleteHistory.Enabled := Assigned(FHistoryXML);
  ViewEventList;
end;

procedure TSheetHistory.cbSheetListChange(Sender: TObject);
var
  Index: integer;
begin
  if not Assigned(FActiveBook) then
    exit;
  Index := cbSheetList.ItemIndex + 1;
  if ((Index < 1) or (Index > FActiveBook.Sheets.Count)) then
    exit;
  //если выбрали текущий лист - ничего не делаем
  if (Index = ActiveExcelSheet.Index[FLCID]) then
    exit;
  ActiveExcelSheet := GetWorkSheet(FActiveBook.Sheets.item[Index]);
end;

function TSheetHistory.CreateHistoryDocument(ExcelSheet: ExcelWorksheet): IXMLDOMDocument2;
begin
  FCounter := 0;
  result := InitXmlDocument;
  result.documentElement := result.createElement('function_result');
  result.documentElement.setAttribute('function_name', nFunction);
  result.documentElement.setAttribute(attrCounter, IntToStr(FCounter));
end;

procedure TSheetHistory.AddEvent(ExcelSheet: ExcelWorkSheet;
  EventType: TSheetEventType; Comment: string; IsSuccess: boolean);
var
  DOM: IXMLDOMDocument2;
  Root: IXMLDOMNode;
  Node: IXMLDOMNode;
  CP: CustomProperty;
begin
  if not Assigned(ExcelSheet) then
    exit;

  CP := GetCPByName(ExcelSheet, cpSheetHistory, false);
  if not Assigned(CP) then
  begin
    DOM := CreateHistoryDocument(ExcelSheet);
    CP := ExcelSheet.CustomProperties.Add(cpSheetHistory, 'eprst');
  end
  else
    DOM := GetDataFromCP(ExcelSheet, cpSheetHistory);

  Root := DOM.selectSingleNode('function_result');
  FCounter := GetIntAttr(Root, attrCounter, 0);
  Node := InsertElement(Root, xpEvent, 0);

  with Node as IXMLDOMElement do
  begin
    setAttribute(attrName, GetTypeName(EventType));
    setAttribute(attrID, GetUniqueID);
    setAttribute(attrTypeIndex, GetTypeIndex(EventType));
    setAttribute(attrIsSuccess, BoolToStr(IsSuccess));
    setAttribute(attrDate, DateTimeToStr(Now));
    setAttribute(attrUser, ExcelSheet.Application.UserName[FLCID]);
    Comment := IIF(IsTestVersion, (ConvertStringToCommaText(GetAddinVersion +
      ' (тестовая)') + #10#13 + Comment), (ConvertStringToCommaText(GetAddinVersion) +
      #10#13 + Comment));
    AppendCDATA(Node, Comment);
  end;

  (Root as IXMLDOMElement).setAttribute(attrCounter, IntToStr(FCounter));
  CP.Value := DOM.xml;
end;

function TSheetHistory.GetTypeName(EventType: TSheetEventType): string;
begin
  result := '';
  case EventType of
    evtEdit: result := enSheetEdit;
    evtRefresh: result := enSheetRefresh;
    evtWriteBack: result := enWriteBack;
    evtPropertyEdit: result := enPropertyEdit;
    evtUnknown: result := enUnknown;
    evtVersionUpdate: result := enVersionUpdate;
    evtSheetCopy: result := enSheetCopy;
    evtParamsEdit: result := enParamsEdit;
    evtTaskConnectionOn: result := enTaskConnectionOn;
    evtTaskConnectionOff: result := enTaskConnectionOff;
    evtConstsEdit: result := enConstsEdit;
  end;
end;

function TSheetHistory.GetTypeIndex(EventType: TSheetEventType): integer;
begin
  result := Integer(EventType);
end;

function TSheetHistory.GetUniqueID: string;
begin
  inc(FCounter);
  result := IntToStr(FCounter);
end;

procedure TSheetHistory.ViewEventList;
var
  ListItem: TListItem;
  i, NodesCount: integer;
  DOMNodeList: IXMLDOMNodeList;
  IsSuccessEvent: boolean;
begin
  lwEventList.Items.Clear;
  mEventSpecification.Clear;
  if not(Assigned(FActiveBook) and Assigned(FHistoryXML))  then
    exit;
  DOMNodeList := FHistoryXML.selectNodes('function_result/' + xpEvent);
  if not Assigned(DOMNodeList) then
    exit;
  NodesCount := DOMNodeList.length;
  for i := 0 to NodesCount - 1 do
  begin
    //добавляем имя события на форму(lwEventList)
    ListItem := lwEventList.Items.Add;
    ListItem.Caption := GetStrAttr(DOMNodeList[i], attrName, enUnknown);
    IsSuccessEvent := GetBoolAttr(DOMNodeList[i], attrIsSuccess, true);
    if IsSuccessEvent then
      //устанавливаем иконку, её индексом является: индекс типа события в XML
      ListItem.StateIndex := GetIntAttr(DOMNodeList[i], attrTypeIndex, -1)
    else
      //если событие было выполнено некорректно устанавливаем иконку с индексом ошибки
      ListItem.StateIndex := iiFault;
    //отображаем имя пользователя совершившего событие
    ListItem.SubItems.Add(GetStrAttr(DOMNodeList[i], attrUser, 'Неизвестный'));
    //время события
    ListItem.SubItems.Add(GetStrAttr(DOMNodeList[i], attrDate, ''));
  end;
  if (lwEventList.Items.Count > 0) then
    lwEventList.Items.Item[0].Selected := true;
end;

procedure TSheetHistory.ViewEventSpecification(ID: integer);
var
  DOMNode, CDATANode: IXMLDOMNode;
  Comment: TStringList;
  i: integer;
begin
  mEventSpecification.Clear;
  DOMNode := FHistoryXML.selectSingleNode('function_result/' +
    xpEvent + '[@id="' + IntToStr(ID) + '"]');
  CDATANode := DOMNode.firstChild;
  if Assigned(CDATANode) then
    with mEventSpecification.Lines do
    begin
      BeginUpdate;
      Comment := TStringList.Create;
      try
        Comment.CommaText := CDATANode.text;
        for i := 0 to Comment.Count - 1 do
          Add(Comment.Strings[i]);
      finally
        EndUpdate;
        FreeStringList(Comment);
      end;
    end;
end;

procedure TSheetHistory.DeleteHistory(ExcelSheet: ExcelWorksheet);
var
  CP: CustomProperty;
begin
  try
    if not Assigned(ExcelSheet) then
      exit;
    CP := GetCPByName(ExcelSheet, cpSheetHistory, false);
    if not Assigned(CP) then
      exit;
    CP.Delete;
  except
  end;  
end;

procedure TSheetHistory.btDeleteHistoryClick(Sender: TObject);
begin
  if not ShowQuestion(qumDelSheetHistory) then
    exit;
  DeleteHistory(ActiveExcelSheet);
  btDeleteHistory.Enabled := false;
  lwEventList.Items.Clear;
  mEventSpecification.Clear;
  FHistoryXML := nil;  
end;

procedure TSheetHistory.btExitClick(Sender: TObject);
begin
  Close;
end;

procedure TSheetHistory.FormKeyPress(Sender: TObject; var Key: Char);
begin
  if (Key = Chr(27)) or (Key = Chr(vk_return)) then
    close;
end;

procedure TSheetHistory.lwEventListSelectItem(Sender: TObject;
  Item: TListItem; Selected: Boolean);
var
  NodeId: integer;
begin
  if not(Assigned(FHistoryXML) and Selected) then
    exit;
  NodeID := lwEventList.Items.Count - Item.Index;
  ViewEventSpecification(NodeID);
end;

procedure TSheetHistory.FormDestroy(Sender: TObject);
begin
  KillDomDocument(FHistoryXML);
end;

procedure TSheetHistory.SwitchLastEvents(ExcelSheet: ExcelWorkSheet);
var
  CP: CustomProperty;
  Dom: IXMLDOMDocument2;
  Root, FirstNode, SecondNode: IXMLDOMNode;
  Id: integer;
begin
  if not Assigned(ExcelSheet) then
    exit;

  CP := GetCPByName(ExcelSheet, cpSheetHistory, false);
  if not Assigned(CP) then
    exit;
  DOM := GetDataFromCP(ExcelSheet, cpSheetHistory);

  Root := DOM.selectSingleNode('function_result');
  FirstNode := Root.firstChild;
  if not Assigned(FirstNode) then
    exit;
  SecondNode := FirstNode.nextSibling;
  if not Assigned(SecondNode) then
    exit;

  Id := GetIntAttr(FirstNode, attrId, 1);
  SecondNode := Root.removeChild(SecondNode);
  SecondNode := Root.insertBefore(SecondNode, FirstNode);
  (SecondNode as IXMLDOMElement).setAttribute(attrId, Id);
  (FirstNode as IXMLDOMElement).setAttribute(attrId, Id - 1);
  CP.Value := DOM.xml;
end;

end.
