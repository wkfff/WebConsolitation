{
  Общие метдоды работы с офисными приложениями, как таковыми.
  Словом все, что крутится вокруг модуля OfficeXP
}
unit uOfficeUtils;

interface
uses
  OfficeXP, ExcelXP, Classes, SysUtils, Windows, Registry, ComAddInUtils,
  VBIDEXP, WordXP, Dialogs;

const
  {Ключи сохранения состояния тулбара}
  regToolBar = 'ToolBar';
  regStatusBar = 'StatusBar';
  regCBLeft = 'Left';
  regCBTop = 'Top';
  regCBRowIndex = 'RowIndex';
  regCBPosition = 'Position';
  regCBVisible = 'Visible';

  {Сохраняет положение тулбара в реестре.
   RegRoot - узел в реестре, куда будут сохранены настройки}
  procedure SaveToolBarPosition(AToolBar: CommandBar; RegRoot: string);
  procedure SaveStatusBarPosition(AStatusBar: CommandBar; RegRoot: string);

  {Восстанавливает положение тулбара из реестра
   RegRoot - узел, откуда будут читаться настройки}
  procedure LoadToolBarPosition(AToolBar: CommandBar; RegRoot: string);
  procedure LoadStatusBarPosition(AStatusBar: CommandBar; RegRoot: string);

  {возвращает значение СР активной книги по его имени}
  function GetWBCustomPropertyValue(CustomDocumentProperties: OleVariant; AName: string): string;
  function GetWBCustomPropertyValue2(CustomDocumentProperties: OleVariant; AName: string): widestring;

  {присваивает СР значение, при необходимости создает СР}
  procedure SetWBCustomPropertyValue(CustomDocumentProperties: OleVariant; AName, AValue: string);
  procedure DeleteWBCustomProperty(CustomDocumentProperties: OleVariant; AName: string);

  {Cоздаем кнопку в меню}
  function AppendInMenuButton(Menu: CommandBarControl; ParentControlTag: string;
    Tag: string; Caption: string; Face: integer; Style: MsoButtonStyle;
    IsGroupStarted: boolean; var EventSink: TCommandButtonEventSink;
    Handler: TOnCommandButtonClick; AdditionalInfo: widestring): CommandBarButton;
  {Cоздаем кнопку в тулбарe}
  function AppendInToolBarButton(ToolBar: CommandBar; ParentControlTag: string;
    Tag: string; Caption: string; Face: integer; Style: MsoButtonStyle;
    IsGroupStarted: boolean; var EventSink: TCommandButtonEventSink;
    Handler: TOnCommandButtonClick; AdditionalInfo: widestring): CommandBarButton;
  {Создаем едит на тулбаре}
  function AppendEditInToolBar(ToolBar: CommandBar; Tag: string; Caption: string;
    Text: string; Width: integer; Style: MsoComboStyle; Enabled: boolean;
    IsGroupStarted: boolean): CommandBarComboBox;
  {Поменять текст в ComboBox-е}
  procedure SetTextInComboBox(ComboBox: CommandBarComboBox; Text: string);
  function GetTextInComboBox(ComboBox: CommandBarComboBox): string;
  {Cоздаем подменю в меню}
  function AppendInMenuSubMenu(Menu: CommandBarControl; ParentControlTag: string;
    Tag: string; NameSubMenu: string; Face: integer;
    IsGroupStarted: boolean): CommandBarPopup;
  {Cоздаем подменю в тулбаре}
  function AppendInToolBarSubMenu(ToolBar: CommandBar; ParentControlTag: string;
    Tag: string; NameSubMenu: string; Face: integer;
    IsGroupStarted: boolean): CommandBarPopup;

  procedure GetMacrosOfVBComponent(VBComponent_: VBComponent; out List: TStringList);
  procedure GetMacrosOfVBComponents(VBComponents_: VBComponents; out List: TStringList);
  {Достает список макросов из екселевской книги}
  procedure GetExcelMacrosList(ExcelWBook: ExcelWorkbook; out List: TStringList);
  {Достает список макросов из вордовского документа}
  procedure GetWordMacrosList(WordDocum: WordDocument; out List: TStringList);
  
  function FindControl(CommandBar_: CommandBar; Tag: string): CommandBarControl;
  {Возвращает комбобокс}
  function GetCommandBarComboBox(CommandBar_: CommandBar; ComboBoxTag: string): CommandBarComboBox;

implementation

///!!!!!!!!!!!! таких процедур уже с десяток наверное - прибираться
function ReadStrRegSetting(RegPath: string; ValueName: string; DefaultValue: String): String;
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  result := DefaultValue;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(RegPath)) then
    begin
      Reg.OpenKey(RegPath, false);
      if Reg.ValueExists(ValueName) then
        result := Reg.ReadString(ValueName)
      else
        Reg.WriteString(ValueName, DefaultValue);

      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;

end;

///!!!!!!!!!!!! таких процедур уже с десяток наверное - прибираться
procedure WriteStrRegSetting(RegPath: string; ValueName: string; Value: String);
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(RegPath, true) then
      Reg.WriteString(ValueName, Value);
  finally
    Reg.Free;
  end;
end;

procedure SaveCommandBarPosition(ACommandBar: CommandBar; RegRoot, BarName: string);
begin
  if Assigned(ACommandBar) then
  try
    BarName := BarName + '_';
    WriteStrRegSetting(RegRoot, BarName + regCBLeft, IntToStr(ACommandBar.Left));
    WriteStrRegSetting(RegRoot, BarName + regCBTop, IntToStr(ACommandBar.Top));
    WriteStrRegSetting(RegRoot, BarName + regCBRowIndex, IntToStr(ACommandBar.RowIndex));
    WriteStrRegSetting(RegRoot, BarName + regCBPosition, IntToStr(ACommandBar.Position));

    if ACommandBar.Visible then
      WriteStrRegSetting(RegRoot, BarName + regCBVisible, 'true')
    else
      WriteStrRegSetting(RegRoot, BarName + regCBVisible, 'false')
  except
  end;
end;

procedure SaveToolBarPosition(AToolBar: CommandBar; RegRoot: string);
begin
  SaveCommandBarPosition(AToolBar, RegRoot, regToolBar);
end;

procedure SaveStatusBarPosition(AStatusBar: CommandBar; RegRoot: string);
begin
  SaveCommandBarPosition(AStatusBar, RegRoot, regStatusBar);
end;

procedure LoadCommandBarPosition(ACommandBar: CommandBar; RegRoot, BarName: string);
var
  CBLeft, CBTop, CBRowIndex, CBPosition, CBVisible: string;
begin
  if Assigned(ACommandBar) then
  begin
    ACommandBar.Set_Visible(false); //прячем, что бы не мелькало.
    BarName := BarName + '_';
    {Считываем параметры}
    CBPosition := ReadStrRegSetting(RegRoot, BarName + regCBPosition, '');
    CBRowIndex := ReadStrRegSetting(RegRoot, BarName + regCBRowIndex, '');
    CBTop := ReadStrRegSetting(RegRoot, BarName + regCBTop, '');
    CBLeft := ReadStrRegSetting(RegRoot, BarName + regCBLeft, '');
    CBVisible := ReadStrRegSetting(RegRoot, BarName + regCBVisible, 'true');

    {Порядок восстановления свойств имеет значение!}
    if CBPosition <> '' then
      ACommandBar.Set_Position(StrToInt(CBPosition));

    if CBRowIndex <> '' then
      ACommandBar.Set_RowIndex(StrToInt(CBRowIndex));

    if CBTop <> '' then
      ACommandBar.Set_Top(StrToInt(CBTop));

    if CBLeft <> '' then
      ACommandBar.Set_Left(StrToInt(CBLeft));

    ACommandBar.Set_Visible(CBVisible = 'true');
  end;
end;

procedure LoadToolBarPosition(AToolBar: CommandBar; RegRoot: string);
begin
  LoadCommandBarPosition(AToolBar, RegRoot, regToolBar);
end;

procedure LoadStatusBarPosition(AStatusBar: CommandBar; RegRoot: string);
begin
  LoadCommandBarPosition(AStatusBar, RegRoot, regStatusBar);
end;

procedure DeleteWBCustomProperty(CustomDocumentProperties: OleVariant; AName: string);
var
  i: integer;
begin
  try
    for i := 1 to CustomDocumentProperties.Count do
      if (CustomDocumentProperties.Item[i].Name = AName) then
      begin
        CustomDocumentProperties.Item[i].Delete;
        exit;
      end;
  except
  end;
end;

function GetWBCustomPropertyValue(CustomDocumentProperties: OleVariant; AName: string): string;
var
  i, k: integer;
begin
  result := '';
  try
    for i := 1 to CustomDocumentProperties.Count do
      if (CustomDocumentProperties.Item[i].Name = AName) then
      begin
        k := VarType(CustomDocumentProperties.Item[i].Value);
        if k and varBoolean = varBoolean then
          if boolean(CustomDocumentProperties.Item[i].Value) then
            result := 'true'
          else
            result := 'false'
        else
          result := CustomDocumentProperties.Item[i].Value;
        break;
      end;
  except
  end;
end;

function GetWBCustomPropertyValue2(CustomDocumentProperties: OleVariant; AName: string): widestring;
var
  i: integer;
begin
  result := '';
  try
    for i := 1 to CustomDocumentProperties.Count do
      if (CustomDocumentProperties.Item[i].Name = AName) then
      begin
        result := CustomDocumentProperties.Item[i].Value;
        break;
      end;
  except
  end;
end;

procedure SetWBCustomPropertyValue(CustomDocumentProperties: OleVariant; AName, AValue: string);
var
  i: integer;
begin
  try
    for i := 1 to CustomDocumentProperties.Count do
      if (CustomDocumentProperties.Item[i].Name = AName) then
      begin
        CustomDocumentProperties.Item[i].Value := AValue;
        exit;
      end;
  except
  end;
  //если свойства нет, то создадим его
  CustomDocumentProperties.Add(AName, false, msoPropertyTypeString, AValue, EmptyParam);
end;

{Cоздаем кнопку в меню}
function AppendInMenuButton(Menu: CommandBarControl; ParentControlTag: string;
  Tag: string; Caption: string; Face: integer; Style: MsoButtonStyle;
  IsGroupStarted: boolean; var EventSink: TCommandButtonEventSink;
  Handler: TOnCommandButtonClick; AdditionalInfo: widestring): CommandBarButton;
var
  PlaceInsert: CommandBarControl;
  Button: CommandBarButton;
begin
  result := nil;
  if not Assigned(Menu) then
    exit;

  Button := Menu.Parent.FindControl(EmptyParam, EmptyParam, Tag, EmptyParam,
    true) as CommandBarButton;

  if Assigned(Button) then
    exit;

  if (Trim(ParentControlTag) <> '') then
    PlaceInsert := Menu.Parent.FindControl(EmptyParam, EmptyParam, ParentControlTag,
      EmptyParam, true)
  else
    PlaceInsert := Menu;

  if Assigned(PlaceInsert) then
    Button := (PlaceInsert as CommandBarPopup).Controls.Add(msoControlButton,
      EmptyParam, Tag, 1, EmptyParam) as CommandBarButton;
  if not Assigned(Button) then
    exit;

  Button.Set_Tag(Tag);
  Button.Set_BeginGroup(IsGroupStarted);
  // надпись
  Button.Set_Caption(Caption);
  // иконка (временный вариант - используем встроенные)
  Button.Set_FaceId(Face);
  // стиль отображения
  Button.Set_Style(Style);  
  Button.Set_HelpFile(AdditionalInfo);
  // вешаем обработчик
  EventSink := TCommandButtonEventSink.Create;
  EventSink.OnClick := Handler;
  EventSink.Connect(Button);
  result := Button;
end;

{Cоздаем кнопку в тулбарe}
function AppendInToolBarButton(ToolBar: CommandBar; ParentControlTag: string;
  Tag: string; Caption: string; Face: integer; Style: MsoButtonStyle;
  IsGroupStarted: boolean; var EventSink: TCommandButtonEventSink;
  Handler: TOnCommandButtonClick; AdditionalInfo: widestring): CommandBarButton;
var
  PlaceInsert: CommandBarControl;
  Button: CommandBarButton;
begin
  result := nil;
  if not Assigned(ToolBar) then
    exit;

  Button := ToolBar.FindControl(EmptyParam, EmptyParam, Tag, EmptyParam,
    true) as CommandBarButton;

  if Assigned(Button) then
    exit;

  if (Trim(ParentControlTag) <> '') then
    PlaceInsert := ToolBar.FindControl(EmptyParam, EmptyParam, ParentControlTag,
      EmptyParam, true);

  if Assigned(PlaceInsert) then
    Button := (PlaceInsert as CommandBarPopup).Controls.Add(msoControlButton,
      EmptyParam, Tag, 1, EmptyParam) as CommandBarButton
  else
    Button := ToolBar.Controls.Add(Style, EmptyParam, Tag, 1,
      EmptyParam) as CommandBarButton;
  if not Assigned(Button) then
    exit;

  Button.Set_Tag(Tag);
  Button.Set_BeginGroup(IsGroupStarted);
  // надпись
  Button.Set_Caption(Caption);
  // иконка (временный вариант - используем встроенные)
  Button.Set_FaceId(Face);
  // стиль отображения
  Button.Set_Style(Style);
  Button.Set_HelpFile(AdditionalInfo);
  // вешаем обработчик
  EventSink := TCommandButtonEventSink.Create;
  EventSink.OnClick := Handler;
  EventSink.Connect(Button);
  result := Button;
end;

function AppendEditInToolBar(ToolBar: CommandBar; Tag: string; Caption: string;
  Text: string; Width: integer; Style: MsoComboStyle; Enabled: boolean;
  IsGroupStarted: boolean): CommandBarComboBox;
var
  Edit: CommandBarComboBox;
begin
  result := nil;
  if not Assigned(ToolBar) then
    exit;

  Edit := ToolBar.FindControl(EmptyParam, EmptyParam, Tag, EmptyParam, true)
    as CommandBarComboBox;
  if Assigned(Edit) then
    exit;
  Edit := ToolBar.Controls.Add(msoControlEdit, EmptyParam, Tag, 1, true)
    as CommandBarComboBox;
  if not Assigned(Edit) then
    exit;
  Edit.Set_Style(Style);
  Edit.Set_Tag(Tag);
  Edit.Set_BeginGroup(IsGroupStarted);
  Edit.Set_Caption(Caption);
  Edit.Set_Width(Width);
  Edit.Set_Text(Text);
  Edit.Set_Enabled(Enabled);
  result := Edit;
end;

{Поменять текст в ComboBox-е}
procedure SetTextInComboBox(ComboBox: CommandBarComboBox; Text: string);
var
  Enabled: boolean;
begin
  if not Assigned(ComboBox) then
    exit;
  try
    Enabled := ComboBox.Enabled;
    if not Enabled then
      ComboBox.Set_Enabled(true);
    ComboBox.Set_Text(Text);
    ComboBox.Set_Enabled(Enabled);
  except
  end;
end;

function GetTextInComboBox(ComboBox: CommandBarComboBox): string;
var
  Enabled: boolean;
begin
  result := '';
  if not Assigned(ComboBox) then
    exit;
  try
    Enabled := ComboBox.Enabled;
    if not Enabled then
      ComboBox.Set_Enabled(true);
    result := ComboBox.Get_Text;
    ComboBox.Set_Enabled(Enabled);
  except
  end;
end;


{Cоздаем подменю в меню}
function AppendInMenuSubMenu(Menu: CommandBarControl; ParentControlTag: string;
  Tag: string; NameSubMenu: string; Face: integer;
  IsGroupStarted: boolean): CommandBarPopup;
var
  PlaceInsert: CommandBarControl;
  SubMenu: CommandBarPopup;
begin
  result := nil;
  if not Assigned(Menu) then
    exit;

  SubMenu := Menu.Parent.FindControl(EmptyParam, EmptyParam, Tag,
    EmptyParam, true) as CommandBarPopup;

  if Assigned(SubMenu) then
    exit;

  if (Trim(ParentControlTag) <> '') then
    PlaceInsert := Menu.Parent.FindControl(EmptyParam, EmptyParam, ParentControlTag,
      EmptyParam, true)
  else
    PlaceInsert := Menu;

  if Assigned(PlaceInsert) then
    SubMenu := (PlaceInsert as CommandBarPopup).Controls.Add(msoControlPopup, EmptyParam, Tag, 1,
      EmptyParam) as CommandBarPopup;
  if not Assigned(SubMenu) then
    exit;

  SubMenu.Set_Tag(Tag);
  SubMenu.Set_BeginGroup(IsGroupStarted);
  SubMenu.Set_Caption(NameSubMenu);

  result := SubMenu;
end;

{Cоздаем подменю в тулбаре}
function AppendInToolBarSubMenu(ToolBar: CommandBar; ParentControlTag: string;
  Tag: string; NameSubMenu: string; Face: integer;
  IsGroupStarted: boolean): CommandBarPopup;
var
  PlaceInsert: CommandBarControl;
  SubMenu: CommandBarPopup;
begin
  result := nil;
  if not Assigned(ToolBar) then
    exit;

  SubMenu := ToolBar.FindControl(EmptyParam, EmptyParam, Tag,
    EmptyParam, true) as CommandBarPopup;

  if Assigned(SubMenu) then
    exit;

  if (Trim(ParentControlTag) <> '') then
    PlaceInsert := ToolBar.FindControl(EmptyParam, EmptyParam, ParentControlTag,
      EmptyParam, true);

  if Assigned(PlaceInsert) then
    SubMenu := (PlaceInsert as CommandBarPopup).Controls.Add(msoControlPopup, EmptyParam, Tag, 1,
      EmptyParam) as CommandBarPopup
  else
    SubMenu := ToolBar.Controls.Add(msoControlPopup, EmptyParam, Tag, 1,
      EmptyParam) as CommandBarPopup;
  if not Assigned(SubMenu) then
    exit;      

  SubMenu.Set_Tag(Tag);
  SubMenu.Set_BeginGroup(IsGroupStarted);
  SubMenu.Set_Caption(NameSubMenu);

  result := SubMenu;
end;

procedure GetMacrosOfVBComponent(VBComponent_: VBComponent; out List: TStringList);
var
  i: integer;
  MacrosName: string;
  Prockind: vbext_ProcKind;
begin
  if not Assigned(List) then
    List := TStringList.Create;
  if not Assigned(VBComponent_) then
    exit;
  i := 1;
  while i <= VBComponent_.CodeModule.CountOfLines do
  begin
    MacrosName := VBComponent_.CodeModule.ProcOfLine[i, Prockind];
    if MacrosName = '' then
    begin
      inc(i);
      continue;
    end
    else
      i := i + VBComponent_.CodeModule.ProcCountLines[MacrosName, Prockind];
    {Если тип компонента vbext_ct_Document, значит к имени макроса дописываем
    имя компонента, иначе работать не будет. К ворду это не отностится}
    if (VBComponent_.type_ = vbext_ct_Document) and (VBComponent_.Name <> 'ThisDocument') then
      MacrosName := VBComponent_.Name + '.' + MacrosName;
    List.Add(MacrosName);
  end;
end;

procedure GetMacrosOfVBComponents(VBComponents_: VBComponents; out List: TStringList);
var
  i: integer;
begin
  if not Assigned(List) then
    List := TStringList.Create;
  if not Assigned(VBComponents_) then
    exit;
  for i := 1 to VBComponents_.Count do
    if (VBComponents_.Item(i).Name <> 'KristaPlaningVBModule') then
      GetMacrosOfVBComponent(VBComponents_.Item(i), List);
end;

procedure GetExcelMacrosList(ExcelWBook: ExcelWorkbook; out List: TStringList);
begin
  if not Assigned(List) then
    List := TStringList.Create;
  try
    if not(Assigned(ExcelWBook) and Assigned(ExcelWBook.VBProject)) then
      exit;
    GetMacrosOfVBComponents(ExcelWBook.VBProject.VBComponents, List);
  except
    List.Free;
    List := nil;
  end;
end;

procedure GetWordMacrosList(WordDocum: WordDocument; out List: TStringList);
begin
  if not Assigned(List) then
    List := TStringList.Create;
  try
    if not(Assigned(WordDocum) and Assigned(WordDocum.VBProject)) then
      exit;
    GetMacrosOfVBComponents(WordDocum.VBProject.VBComponents, List);
  except
    List.Free;
    List := nil
  end;
end;

function FindControl(CommandBar_: CommandBar; Tag: string): CommandBarControl;
begin
  result := nil;
  if not Assigned(CommandBar_) then
    exit;
  try
    result := CommandBar_.FindControl(EmptyParam, EmptyParam, Tag, EmptyParam, true);
  except
    result := nil;
  end;
end;

{Возвращает комбобокс}
function GetCommandBarComboBox(CommandBar_: CommandBar; ComboBoxTag: string): CommandBarComboBox;
begin
  if not Assigned(CommandBar_) then
    exit;
  try
    result := FindControl(CommandBar_, ComboBoxTag) as CommandBarComboBox;
  except
    result := nil;
  end;
end;

end.
