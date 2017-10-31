unit uDSOPropList;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, StdCtrls, Forms, Dialogs,
  ComObj, DSO80_TLB, uDSOPropEdit, uDsoXmlSchema, uDsoXmlConverter;

type
  TDSOPropType = (ptSimple, ptEllipsis, ptPickList);

  TDSOPopupList = class(TCustomListBox)
  private
    FSearchText: string;
    FSearchTickCount: Integer;
  protected
    procedure CreateWnd; override;
    procedure KeyPress(var Key: Char); override;
  public
    constructor Create(AOwner: TComponent); override;
    procedure CreateParams(var Params: TCreateParams); override;
    procedure Hide;
  end;

  TDSOPropList = class;
  TInplaceEdit = class;

  TDSOListButton = class(TCustomControl)
  private
    FButtonWidth: Integer;
    FPressed: Boolean;
    FArrow: Boolean;
    FTracking: Boolean;
    FActiveList: TDSOPopupList;
    FListVisible: Boolean;
    FEditor: TInplaceEdit;
    FPropList: TDSOPropList;
    FListItemHeight: Integer;
    procedure TrackButton(X, Y: Integer);
    procedure StopTracking;
    procedure DropDown;
    procedure CloseUp(Accept: Boolean);
    procedure ListMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure DoDropDownKeys(var Key: Word; Shift: TShiftState);
    function Editor: TPropertyEditor;
    procedure CMCancelMode(var Message: TCMCancelMode); message CM_CANCELMODE;
    procedure WMKillFocus(var Msg: TWMKillFocus); message WM_KILLFOCUS;
    procedure WMGetDlgCode(var Msg: TWMGetDlgCode); message WM_GETDLGCODE;
    procedure WMCancelMode(var Msg: TWMKillFocus); message WM_CANCELMODE;
    procedure CMFontChanged(var Message: TMessage); message CM_FONTCHANGED;
    procedure MeasureHeight(Control: TWinControl; Index: Integer;
      var Height: Integer);
    procedure DrawItem(Control: TWinControl; Index: Integer; Rect: TRect;
      State: TOwnerDrawState);
  protected
    procedure KeyPress(var Key: Char); override;
    procedure Paint; override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure WndProc(var Message: TMessage); override;
  public
    constructor Create(AOwner: TComponent); override;
    procedure Hide;
  end;


  // Редактор отдельного свойства DSO-объекта
  TInplaceEdit = class(TCustomEdit)
  private
    FPropList: TDSOPropList;
    FClickTime: Integer;
    FListButton: TDSOListButton;
    FAutoUpdate: Boolean;
    FPropType: TDSOPropType;
    FIgnoreChange: Boolean;
    procedure InternalMove(const Loc: TRect; Redraw: Boolean);
    procedure BoundsChanged;
    procedure WMSetFocus(var Msg: TWMSetFocus); message WM_SETFOCUS;
    procedure WMKillFocus(var Msg: TWMKillFocus); message WM_KILLFOCUS;
  protected
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
    procedure KeyPress(var Key: Char); override;
    procedure KeyUp(var Key: Word; Shift: TShiftState); override;
    procedure DblClick; override;
    procedure WndProc(var Message: TMessage); override;
    procedure Change; override;
    procedure AutoComplete(const S: string);
  public
    constructor Create(AOwner: TComponent); override;
    procedure CreateParams(var Params: TCreateParams); override;
    procedure Move(const Loc: TRect);
    procedure UpdateLoc(const Loc: TRect);
    procedure SetFocus; override;
  end;

  // Структура, описывающая редактор свойства объекта,
  // хранится в списоке редакторов свойств
  PDSOEditor = ^TDSOEditor;
  TDSOEditor = record
    peEditor: TPropertyEditor;
    peIdent: Integer;
    peNode: Boolean;
    peExpanded: Boolean;
  end;

  // Список редакторов свойств, хранит указатели на TDSOEditor
  TDSOEditorList = class(TList)
  private
    FPropList: TDSOPropList;
    function GetEditor(AIndex: Integer): PDSOEditor;
  public
    procedure Clear; override;
    procedure Add(Editor: PDSOEditor);
    procedure DeleteEditor(Index: Integer);
    function IndexOfPropName(const PropName: string;
      StartIndex: Integer): Integer;
    function FindPropName(const PropName: string): Integer;
    constructor Create(APropList: TDSOPropList);
    property Editors[AIndex: Integer]: PDSOEditor read GetEditor; default;
  end;

  TNewObjectEvent = procedure (Sender: TDSOPropList; OldObj,
    NewObj: ICommon) of object;
  TChangingEvent = procedure (Sender: TDSOPropList; Prop: TPropertyEditor;
    var CanChange: Boolean) of object;
  THintEvent = procedure (Sender: TDSOPropList; Prop: TPropertyEditor;
    HintInfo: PHintInfo) of object;
  TChangeEvent = procedure (Sender: TDSOPropList; Prop: TPropertyEditor) of object;

  TDSOPropList = class(TCustomControl)
  private
    FOnGetMacrosList: TGetMacrosListProc;
    FMetaObject: TSchemaObject;// Мета-объект описывающий DSO-объект
    FDSOObject: ICommon;       // Сам DSO-объект
    FEditors: TDSOEditorList;  // Список редакторов отдельных свойств DSO-объекта
    FPropCount: Integer;       // Кол-во св-в(редакторов)
    FPropColor: TColor;
    FRowHeight: Integer;
    FHasScrollBar: Boolean;
    FTopRow: Integer;
    FCurrent: Integer;         // Индекс активного редактора
    FDividerHit: Boolean;
    FVertLine: Integer;        // Положение вертикального разделителя
    FHitTest: TPoint;
    FInplaceEdit: TInplaceEdit;// Собственно "окошко" редактора
    FInUpdate: Boolean;
    FDestroying: Boolean;
    FModified: Boolean;
    FCurrentIdent: Integer;
    FCurrentPos: Integer;
    FTracking: Boolean;
    FNewButtons: Boolean;
    FDefFormProc: Pointer;
    FFormHandle: HWND;
    FBorderStyle: TBorderStyle;
    FOnNewObject: TNewObjectEvent;
    FOnHint: THintEvent;
    FHintTimeout: Integer;
    FScrollBarStyle: TScrollBarStyle;
    FOnChanging: TChangingEvent;
    FOnChange: TChangeEvent;
    procedure CMFontChanged(var Message: TMessage); message CM_FONTCHANGED;
    procedure CMShowingChanged(var Message: TMessage); message CM_SHOWINGCHANGED;
    procedure CMHintShow(var Msg: TCMHintShow); message CM_HINTSHOW;
    procedure WMNCHitTest(var Msg: TWMNCHitTest); message WM_NCHITTEST;
    procedure WMSetCursor(var Msg: TWMSetCursor); message WM_SETCURSOR;
    procedure CMCancelMode(var Msg: TMessage); message CM_CANCELMODE;
    procedure WMSize(var Msg: TWMSize); message WM_SIZE;
    procedure WMVScroll(var Msg: TWMVScroll); message WM_VSCROLL;
    procedure WMGetDlgCode(var Msg: TWMGetDlgCode); message WM_GETDLGCODE;
    procedure ModifyScrollBar(ScrollCode: Integer);
    procedure SetDSOObj(const Value: ICommon);
    procedure SetMetaObj(const Value: TSchemaObject);
    procedure UpdateScrollRange;
    procedure MoveTop(NewTop: Integer);
    function MoveCurrent(NewCur: Integer): Boolean;
    procedure InvalidateSelection;
    function VertLineHit(X: Integer): Boolean;
    function YToRow(Y: Integer): Integer;
    procedure SizeColumn(X: Integer);
    function GetValue(Index: Integer): string;
    function GetPrintableValue(Index: Integer): string;
    procedure DoEdit(E: TPropertyEditor; DoEdit: Boolean; const Value: string);
    procedure SetValue(Index: Integer; const Value: string);
    procedure CancelMode;
    function GetEditRect: TRect;
    function UpdateText(Exiting: Boolean): Boolean;
    function ColumnSized(X: Integer): Boolean;
    procedure FreePropList;
    procedure InitPropList;
    procedure PropEnumProc(Prop: TPropertyEditor);
    procedure FormWndProc(var Message: TMessage);
    procedure ChangeCurObj(const Value: ICommon);
    function GetName(Index: Integer): string;
    procedure NodeClicked;
    function ButtonHit(X: Integer): Boolean;
    function GetFullPropName(Index: Integer): string;
  protected
    procedure Paint; override;
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure DblClick; override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    function GetPropType: TDSOPropType;
    procedure Edit;
    function Editor: TPropertyEditor;
    procedure CreateWnd; override;
    procedure DestroyWnd; override;
    function GetNameRect(ARow: Integer): TRect;
    function GetValueRect(ARow: Integer): TRect;
    procedure SetNewButtons(const Value: Boolean);
    procedure SetBorderStyle(Value: TBorderStyle);
    procedure UpdateEditor(CallActivate: Boolean);
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure InitCurrent(const PropName: string);
    procedure CreateParams(var Params: TCreateParams); override;
    function VisibleRowCount: Integer;
    property RowHeight: Integer read FRowHeight;
    property PropCount: Integer read FPropCount;
  published
    property HintTimeout: Integer read FHintTimeout write FHintTimeout default 10000;
    property OnGetMacrosList: TGetMacrosListProc read FOnGetMacrosList write FOnGetMacrosList;
    property NewButtons: Boolean read FNewButtons write SetNewButtons default True;
    property OnHint: THintEvent read FOnHint write FOnHint;
    property OnChanging: TChangingEvent read FOnChanging write FOnChanging;
    property OnChange: TChangeEvent read FOnChange write FOnChange;
    property MetaObject: TSchemaObject read FMetaObject write SetMetaObj;
    property DSOObject: ICommon read FDSOObject write SetDSOObj;
    property Align;
    property BorderStyle: TBorderStyle read FBorderStyle write SetBorderStyle default bsSingle;
    property Color default clBtnFace;
    property Ctl3D;
    property Cursor;
    property Enabled;
    property Font;
    property ParentColor default False;
    property ParentFont;
    property ParentShowHint default False;
    property PopupMenu;
    property ShowHint default True;
    property TabOrder;
    property TabStop default True;
    property Visible;
  end;

procedure GetDSOObjectProperties(DSOObj: ICommon; MetaObject: TSchemaObject; Proc: TGetPropEditProc);

procedure Register;

implementation

uses
  CommCtrl;

const
  MINCOLSIZE   = 32;
  DROPDOWNROWS = 8;

procedure Register;
begin
  RegisterComponents('FM Controls', [TDSOPropList]);
end;

{ Return mimimum of two signed integers }
function EMax(A, B: Integer): Integer;
asm
{     ->EAX     A
        EDX     B
      <-EAX     Min(A, B) }

        CMP     EAX,EDX
        JGE     @@Exit
        MOV     EAX,EDX
  @@Exit:
end;

{ Return maximum of two signed integers }
function EMin(A, B: Integer): Integer;
asm
{     ->EAX     A
        EDX     B
      <-EAX     Max(A, B) }

        CMP     EAX,EDX
        JLE     @@Exit
        MOV     EAX,EDX
  @@Exit:
end;


{ TDSOPopupList }

constructor TDSOPopupList.Create(AOwner: TComponent);
begin
  inherited;
  Parent := AOwner as TWinControl;
  ParentCtl3D := False;
  Ctl3D := False;
  Visible := False;
  TabStop := False;
  Style := lbOwnerDrawVariable;
end;

procedure TDSOPopupList.CreateParams(var Params: TCreateParams);
begin
  inherited;
  with Params do
  begin
    Style := Style or WS_BORDER;
    ExStyle := WS_EX_TOOLWINDOW or WS_EX_TOPMOST;
    X := -200;  // move listbox offscreen
    AddBiDiModeExStyle(ExStyle);
    WindowClass.Style := CS_SAVEBITS;
  end;
end;

procedure TDSOPopupList.CreateWnd;
begin
  inherited;
{  if not (csDesigning in ComponentState) then
  begin}
    Windows.SetParent(Handle, 0);
    CallWindowProc(DefWndProc, Handle, WM_SETFOCUS, 0, 0);
//  end;
end;

procedure TDSOPopupList.Hide;
begin
  if HandleAllocated and IsWindowVisible(Handle) then
  begin
    SetWindowPos(Handle, 0, 0, 0, 0, 0, SWP_NOZORDER or
      SWP_NOMOVE or SWP_NOSIZE or SWP_NOACTIVATE or SWP_HIDEWINDOW);
  end;
end;

procedure TDSOPopupList.KeyPress(var Key: Char);
var
  TickCount: Integer;
begin
  case Key of
    #8, #27: FSearchText := '';
    #32..#255:
      begin
        TickCount := GetTickCount;
        if TickCount - FSearchTickCount > 2000 then FSearchText := '';
        FSearchTickCount := TickCount;
        if Length(FSearchText) < 32 then FSearchText := FSearchText + Key;
        SendMessage(Handle, LB_SELECTSTRING, WORD(-1), Longint(PChar(FSearchText)));
        Key := #0;
      end;
  end;
  inherited Keypress(Key);
end;

{ TDSOListButton }

constructor TDSOListButton.Create(AOwner: TComponent);
begin
  inherited;
  FEditor := AOwner as TInplaceEdit;
  FPropList := FEditor.FPropList;
  FButtonWidth := GetSystemMetrics(SM_CXVSCROLL);
  FActiveList := TDSOPopupList.Create(Self);
  FActiveList.OnMouseUp := ListMouseUp;
  FActiveList.OnMeasureItem := MeasureHeight;
  FActiveList.OnDrawItem := DrawItem;
end;

procedure TDSOListButton.Hide;
begin
  if HandleAllocated and IsWindowVisible(Handle) then
  begin
//    Invalidate;
    SetWindowPos(Handle, 0, 0, 0, 0, 0, SWP_HIDEWINDOW or SWP_NOZORDER or
      SWP_NOREDRAW);
  end;
end;

procedure TDSOListButton.Paint;
var
  R: TRect;
  Flags, X, Y, W: Integer;
begin
  R := ClientRect;
  InflateRect(R, 1, 1);
  Flags := 0;

  with Canvas do
    if FArrow then
    begin
      if FPressed then Flags := DFCS_FLAT or DFCS_PUSHED;
      DrawFrameControl(Handle, R, DFC_SCROLL, Flags or DFCS_SCROLLCOMBOBOX);
    end
    else
    begin
      if FPressed then Flags := BF_FLAT;
      DrawEdge(Handle, R, EDGE_RAISED, BF_RECT or BF_MIDDLE or Flags);
      X := R.Left + ((R.Right - R.Left) shr 1) - 1 + Ord(FPressed);
      Y := R.Top + ((R.Bottom - R.Top) shr 1) - 1 + Ord(FPressed);
      W := FButtonWidth shr 3;
      if W = 0 then W := 1;
      PatBlt(Handle, X, Y, W, W, BLACKNESS);
      PatBlt(Handle, X - (W + W), Y, W, W, BLACKNESS);
      PatBlt(Handle, X + W + W, Y, W, W, BLACKNESS);
    end;
end;

procedure TDSOListButton.TrackButton(X, Y: Integer);
var
  NewState: Boolean;
  R: TRect;
begin
  R := ClientRect;
  NewState := PtInRect(R, Point(X, Y));
  if FPressed <> NewState then
  begin
    FPressed := NewState;
    Invalidate;
  end;
end;

function TDSOListButton.Editor: TPropertyEditor;
begin
  Result := FPropList.Editor;
end;

type
  TGetStrFunc = function(const Value: string): Integer of object;

procedure TDSOListButton.DropDown;
var
  I, M, W: Integer;
  P: TPoint;
  MCanvas: TCanvas;
  AddValue: TGetStrFunc;
begin
  if FEditor.ReadOnly then exit;
  if not FListVisible then
  begin
    FActiveList.Clear;
    with Editor do
    begin
      FActiveList.Sorted := paSortList in GetAttributes;
      AddValue := FActiveList.Items.Add;
      GetValues(TGetStrProc(AddValue));
      SendMessage(FActiveList.Handle, LB_SELECTSTRING, WORD(-1), Longint(PChar(GetValue)));
    end;

    with FActiveList do
    begin
      M := EMax(1, EMin(Items.Count, DROPDOWNROWS));
      I := FListItemHeight;
      MeasureHeight(nil, 0, I);
      Height := M * I + 2;
      width := Self.Width + FEditor.Width + 1;
    end;

    with FActiveList do
    begin
      M := ClientWidth;
      MCanvas := FPropList.Canvas;
      for I := 0 to Items.Count - 1 do
      begin
        W := MCanvas.TextWidth(Items[I]) + 4;
        with Editor do
          ListMeasureWidth(GetName, MCanvas, W);
        if W > M then M := W;
      end;
      ClientWidth := M;
      W := Self.Parent.ClientWidth;
      if Width > W then Width := W;
    end;

    P := Parent.ClientToScreen(Point(Left + Width, Top + Height));
    with FActiveList do
    begin
      if P.Y + Height > Screen.Height then P.Y := P.Y - Self.Height - Height;
      SetWindowPos(Handle, HWND_TOP, P.X - Width, P.Y,
        0, 0, SWP_NOSIZE + SWP_SHOWWINDOW);
      SetActiveWindow(Handle);
    end;
    SetFocus;
    FListVisible := True;
  end;
end;

procedure TDSOListButton.CloseUp(Accept: Boolean);
var
  ListValue: string;
  Ch: Char;
begin
  if FListVisible then
  begin
    with FActiveList do
    begin
      if (ItemIndex >= 0) and (ItemIndex < Items.Count) then
        ListValue := Items[ItemIndex] else Accept := False;
//    Invalidate;
      Hide;
      Ch := #27; // Emulate ESC
      FEditor.KeyPress(Ch);
    end;
    FListVisible := False;
    if Accept then  // Emulate ENTER keypress
    begin
      FEditor.Text := ListValue;
      FEditor.Modified := True;
      Ch := #13;
      FEditor.KeyPress(Ch);
    end;
    if Focused then FEditor.SetFocus;
  end;
end;                    

procedure TDSOListButton.StopTracking;
begin
  if FTracking then
  begin
    TrackButton(-1, -1);
    FTracking := False;
//    MouseCapture := False;
  end;
end;

procedure TDSOListButton.MouseDown(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);
begin
  if Button = mbLeft then
  begin
    if FListVisible then
      CloseUp(False)
    else
    begin
//      MouseCapture := True;
      FTracking := True;
      TrackButton(X, Y);
      if FArrow then DropDown;
    end;
  end;
  inherited;
end;

procedure TDSOListButton.MouseMove(Shift: TShiftState; X, Y: Integer);
var
  ListPos: TPoint;
  MousePos: TSmallPoint;
begin
  if FTracking then
  begin
    TrackButton(X, Y);
    if FListVisible then
    begin
      ListPos := FActiveList.ScreenToClient(ClientToScreen(Point(X, Y)));
      if PtInRect(FActiveList.ClientRect, ListPos) then
      begin
        StopTracking;
        MousePos := PointToSmallPoint(ListPos);
        SendMessage(FActiveList.Handle, WM_LBUTTONDOWN, 0, Integer(MousePos));
        Exit;
      end;
    end;
  end;
  inherited;
end;

procedure TDSOListButton.MouseUp(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);
var
  WasPressed: Boolean;
begin
  WasPressed := FPressed;
  StopTracking;
  if (Button = mbLeft) and not FArrow and WasPressed then FEditor.DblClick;
  inherited;
end;

procedure TDSOListButton.ListMouseUp(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
begin
  if Button = mbLeft then
    CloseUp(PtInRect(FActiveList.ClientRect, Point(X, Y)));
end;

procedure TDSOListButton.DoDropDownKeys(var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_RETURN, VK_ESCAPE:
      if FListVisible and not (ssAlt in Shift) then
      begin
        CloseUp(Key = VK_RETURN);
        Key := 0;
      end;
    else
  end;
end;

procedure TDSOListButton.CMCancelMode(var Message: TCMCancelMode);
begin
  inherited;
  if (Message.Sender <> Self) and (Message.Sender <> FActiveList) then
    CloseUp(False);
end;

procedure TDSOListButton.WMCancelMode(var Msg: TWMKillFocus);
begin
  StopTracking;
  inherited;
end;

procedure TDSOListButton.WMKillFocus(var Msg: TWMKillFocus);
begin
  inherited;
  CloseUp(False);
end;

procedure TDSOListButton.KeyPress(var Key: Char);
begin
  if FListVisible then FActiveList.KeyPress(Key);
end;

procedure TDSOListButton.WndProc(var Message: TMessage);
begin
  case Message.Msg of
    WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR:
      with TWMKey(Message) do
      begin
        DoDropDownKeys(CharCode, KeyDataToShiftState(KeyData));
        if (CharCode <> 0) and FListVisible then
        begin
          with TMessage(Message) do
            SendMessage(FActiveList.Handle, Msg, WParam, LParam);
          Exit;
        end;
      end
  end;
  inherited;
end;

procedure TDSOListButton.WMGetDlgCode(var Msg: TWMGetDlgCode);
begin
  Msg.Result := DLGC_WANTARROWS;
end;

procedure TDSOListButton.MeasureHeight(Control: TWinControl; Index: Integer;
  var Height: Integer);
begin
  Height := FListItemHeight;
  with Editor do
    ListMeasureHeight(GetName, FActiveList.Canvas, Height);
end;

procedure TDSOListButton.DrawItem(Control: TWinControl; Index: Integer;
  Rect: TRect; State: TOwnerDrawState);
begin
  with FActiveList do
    Editor.ListDrawValue(Items[Index], Canvas,
      Rect, odSelected in State);
end;

procedure TDSOListButton.CMFontChanged(var Message: TMessage);
begin
  inherited;
  Canvas.Font := FPropList.Font;
  FListItemHeight := Canvas.TextHeight('Wg') + 2;
end;

{ TZInplaceEdit }

constructor TInplaceEdit.Create(AOwner: TComponent);
begin
  inherited;
  Parent := AOwner as TWinControl;
  FPropList := AOwner as TDSOPropList;
  FListButton := TDSOListButton.Create(Self);
  FListButton.Parent := Parent;
  ParentCtl3D := False;
  Ctl3D := False;
  TabStop := False;
  BorderStyle := bsNone;
  Visible := False;
end;

procedure TInplaceEdit.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
  Params.Style := Params.Style or ES_MULTILINE;
end;

procedure TInplaceEdit.InternalMove(const Loc: TRect; Redraw: Boolean);
var
  W, H: Integer;
  ButtonVisible: Boolean;
begin
  if IsRectEmpty(Loc) then Exit;
  Redraw := Redraw or not IsWindowVisible(Handle);
  with Loc do
  begin
    W := Right - Left;
    H := Bottom - Top;
    FPropType := FPropList.GetPropType;
    ButtonVisible := (FPropType <> ptSimple);

    if ButtonVisible then Dec(W, FListButton.FButtonWidth);
    SetWindowPos(Handle, HWND_TOP, Left, Top, W, H,
      SWP_SHOWWINDOW or SWP_NOREDRAW);
    if ButtonVisible then
    begin
      FListButton.FArrow := FPropType = ptPickList;
      SetWindowPos(FListButton.Handle, HWND_TOP, Left + W, Top,
        FListButton.FButtonWidth, H, SWP_SHOWWINDOW or SWP_NOREDRAW);
    end
    else FListButton.Hide;
  end;
  BoundsChanged;

  if Redraw then
  begin
    Invalidate;
    FListButton.Invalidate;
  end;
  if FPropList.Focused then Windows.SetFocus(Handle);
end;

procedure TInplaceEdit.BoundsChanged;
var
  R: TRect;
begin
  R := Rect(2, 1, Width - 2, Height);
  SendMessage(Handle, EM_SETRECTNP, 0, Integer(@R));
  SendMessage(Handle, EM_SCROLLCARET, 0, 0);
end;

procedure TInplaceEdit.UpdateLoc(const Loc: TRect);
begin
  InternalMove(Loc, False);
end;

procedure TInplaceEdit.Move(const Loc: TRect);
begin
  InternalMove(Loc, True);
end;

procedure TInplaceEdit.KeyDown(var Key: Word; Shift: TShiftState);
begin
//  OutputDebugString('KeyDown');
  if (Key = VK_DOWN) and (ssAlt in Shift) then
    with FListButton do
  begin
    if (FPropType = ptPickList) and not FListVisible then DropDown;
    Key := 0;
  end;
  FIgnoreChange := Key = VK_DELETE;
  FPropList.KeyDown(Key, Shift);
  if Key in [VK_UP, VK_DOWN, VK_PRIOR, VK_NEXT] then Key := 0;
end;

procedure TInplaceEdit.KeyUp(var Key: Word; Shift: TShiftState);
begin
//  OutputDebugString('KeyUp');
  FPropList.KeyUp(Key, Shift);
end;

procedure TInplaceEdit.SetFocus;
begin
  if IsWindowVisible(Handle) then
    Windows.SetFocus(Handle);
end;

procedure TInplaceEdit.WMSetFocus(var Msg: TWMSetFocus);
begin
  inherited;
  DestroyCaret;
  CreateCaret(Handle, 0, 1, FPropList.Canvas.TextHeight('A'));
  ShowCaret(Handle);
end;

procedure TInplaceEdit.KeyPress(var Key: Char);
begin
//  OutputDebugString('KeyPress');
//  FPropList.KeyPress(Key);
  FIgnoreChange := (Key = #8) or (SelText <> '');
  case Key of
    #10: DblClick;  // Ctrl + ENTER;
    #13: if Modified then FPropList.UpdateText(True) else SelectAll;
    #27: with FPropList do
           if paRevertable in Editor.getAttributes then UpdateEditor(False);
    else Exit;
  end;
  Key := #0;
end;

procedure TInplaceEdit.WMKillFocus(var Msg: TWMKillFocus);
begin
  inherited;
  if (Msg.FocusedWnd <> FPropList.Handle) and
    (Msg.FocusedWnd <> FListButton.Handle) then
    if not FPropList.UpdateText(True) then SetFocus;
end;

procedure TInplaceEdit.DblClick;
begin
  FPropList.Edit;
end;

procedure TInplaceEdit.WndProc(var Message: TMessage);
begin
  case Message.Msg of
    WM_LBUTTONDOWN:
      begin
        if UINT(GetMessageTime - FClickTime) < GetDoubleClickTime then
          Message.Msg := WM_LBUTTONDBLCLK;
        FClickTime := 0;
      end;
  end;
  inherited;
end;

procedure TInplaceEdit.AutoComplete(const S: string);
var
  I: Integer;
  Values: TStringList;
  AddValue: TGetStrFunc;
begin
  Values := TStringList.Create;
  try
    AddValue := Values.Add;
    FPropList.Editor.GetValues(TGetStrProc(AddValue));
    for I := 0 to Values.Count - 1 do
      if StrLIComp(PChar(S), PChar(Values[I]), Length(S)) = 0 then
      begin
        SendMessage(Handle, WM_SETTEXT, 0, Integer(Values[I]));
        SendMessage(Handle, EM_SETSEL, Length(S), Length(Values[I]));
        Modified := True;
        Break;
      end;
  finally
    Values.Free;
  end;
end;

procedure TInplaceEdit.Change;
begin
  inherited;
  if Modified then
  begin
//    OutputDebugString(PChar('Change, Text = "' + Text + '"'));
    if (FPropType = ptPickList) and not FIgnoreChange then
      AutoComplete(Text);
    FIgnoreChange := False;
    if FAutoUpdate then FPropList.UpdateText(False);
  end;
end;

{ TDSOEditorList }

constructor TDSOEditorList.Create(APropList: TDSOPropList);
begin
  inherited Create;
  FPropList := APropList;
end;

procedure TDSOEditorList.DeleteEditor(Index: Integer);
var
  P: PDSOEditor;
begin
  P := Editors[Index];
  P.peEditor.Free;
  FreeMem(P);
end;

function TDSOEditorList.IndexOfPropName(const PropName: string;
  StartIndex: Integer): Integer;
var
  I: Integer;
begin
  if StartIndex < Count then
  begin
    Result := 0;
    for I := StartIndex to Count - 1 do
      if Editors[I].peEditor.GetName = PropName then
      begin
        Result := I;
        Exit;
      end;
  end
  else
    Result := -1;
end;

function TDSOEditorList.FindPropName(const PropName: string): Integer;
var
  S, Prop: string;
  I: Integer;
begin
  Result := -1;
  S := PropName;
  while S <> '' do        // Expand subproperties
  begin
    I := Pos('\', S);
    if I > 0 then
    begin
      Prop := Copy(S, 1, I - 1);
      System.Delete(S, 1, I);
    end
    else
    begin
      Prop := S;
      S := '';
    end;

    I := IndexOfPropName(Prop, Succ(Result));
    if I <= Result then Exit;
    Result := I;

    if S <> '' then
      with Editors[Result]^ do
        if peNode then
          if not peExpanded then
          begin
            FPropList.FCurrentIdent := peIdent + 1;
            FPropList.FCurrentPos := Result + 1;
            try
              peEditor.GetProperties(FPropList.PropEnumProc);
            except
            end;
            peExpanded := True;
            FPropList.FPropCount := Count;
          end
        else Exit;
  end;
end;

procedure TDSOEditorList.Add(Editor: PDSOEditor);
begin
  inherited Add(Editor);
end;

procedure TDSOEditorList.Clear;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do DeleteEditor(I);
  inherited;
end;

function TDSOEditorList.GetEditor(AIndex: Integer): PDSOEditor;
begin
  Result := Items[AIndex];
end;

{ TDSOPropList }
constructor TDSOPropList.Create(AOwner: TComponent);
const
  PropListStyle = [csCaptureMouse, csOpaque, csDoubleClicks];
begin
  inherited Create(AOwner);
  FInplaceEdit := TInplaceEdit.Create(Self);
  FPropColor := clNavy;
  FEditors := TDSOEditorList.Create(Self);

  FNewButtons := True;
  FCurrent := -1;
//!!  FFilter := tkProperties;
  FBorderStyle := bsSingle;

  if NewStyleControls then
    ControlStyle := PropListStyle else
    ControlStyle := PropListStyle + [csFramed];
  Color  := clBtnFace;
  ParentColor := False;
  TabStop := True;
  SetBounds(Left, Top, 200, 200);
  FVertLine := 85;
  ShowHint := True;
  ParentShowHint := False;
end;

destructor TDSOPropList.Destroy;
begin
  FDestroying := True;
  FHasScrollBar := False;      // disable UpdateScrollRange
  FEditors.Free;
  FInplaceEdit.Free;
  inherited Destroy
end;

procedure TDSOPropList.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
  with Params do
  begin
    Style := Style + WS_TABSTOP;
    Style := Style + WS_VSCROLL;
    WindowClass.style := CS_DBLCLKS;
    if FBorderStyle = bsSingle then
      if NewStyleControls and Ctl3D then
      begin
        Style := Style and not WS_BORDER;
        ExStyle := ExStyle or WS_EX_CLIENTEDGE;
      end
      else
        Style := Style or WS_BORDER;
  end;
end;

procedure TDSOPropList.CMShowingChanged(var Message: TMessage);
begin
  inherited;
  if Showing then
  begin
    FHasScrollBar := True;
    Perform(CM_FONTCHANGED, 0, 0);
    FInplaceEdit.FListButton.Perform(CM_FONTCHANGED, 0, 0);
    Parent.Realign;
{    UpdateScrollRange;
    InitCurrent;
    UpdateEditor(True);}
  end;
end;

procedure TDSOPropList.WMSetCursor(var Msg: TWMSetCursor);
var
  Cur: HCURSOR;
begin
  Cur := 0;
  if (Msg.HitTest = HTCLIENT) and VertLineHit(FHitTest.X) then
    Cur := Screen.Cursors[crHSplit];
  if Cur <> 0 then SetCursor(Cur) else inherited;
end;

procedure TDSOPropList.CMCancelMode(var Msg: TMessage);
begin
  inherited;
  CancelMode;
end;

procedure TDSOPropList.CancelMode;
begin
  FDividerHit := False;
  FTracking := False;
end;

procedure TDSOPropList.InitCurrent(const PropName: string);
begin
//  FCurrent := FEditors.FindPropName(PropName);
  MoveCurrent(FEditors.FindPropName(PropName));
//  if Assigned(FInplaceEdit) then FInplaceEdit.Move(GetEditRect);
end;

procedure TDSOPropList.FreePropList;
begin
  FEditors.Clear;
  FPropCount := 0;
end;

function GetEditorClass(Prop: TProperty): TPropertyEditorClass;
begin
  case Prop.DataType of
  dtInteger:
    if Prop.Enum = -1 then
      Result := TIntegerProperty
    else
      Result := TEnumProperty;
  dtString: Result := TStringProperty;
  dtBoolean: Result := TBoolProperty;
  dtDate: Result := TDateTimeProperty;
  dtDouble: Result := TPropertyEditor;
  dtObject:
   Result := TDataSourceProperty;
  dtCDATA:
    Result := TMultiLineStrProperty;
  else Result := TPropertyEditor;
  end;
end;

procedure GetDSOObjectProperties(DSOObj: ICommon; MetaObject: TSchemaObject; Proc: TGetPropEditProc);
var
  I: Integer;
  EdClass: TPropertyEditorClass;
  Editor: TPropertyEditor;
  Prop: TProperty;
begin
  for I := 0 to MetaObject.Properties.Count - 1 do begin
    Prop := TProperty(MetaObject.Properties.Objects[I]);
    if Prop.CheckAccess(DSOObj, AccessRead) > 0 then begin
      EdClass := GetEditorClass(Prop);
      if EdClass <> nil then begin
        Editor := EdClass.Create(DSOObj, 1);
        Editor.SetPropEntry(0, TProperty(MetaObject.Properties.Objects[I]));
        Editor.Initialize;
        if Editor.ValueAvailable then
          Proc(Editor)
        else
          Editor.Free
      end
    end
  end
end;

procedure TDSOPropList.InitPropList;
begin
  try
    FCurrentIdent := 0;
    FCurrentPos := 0;
    GetDSOObjectProperties(FDSOObject, FMetaObject, PropEnumProc);
    FPropCount := FEditors.Count;
  finally
  end;
end;

function TDSOPropList.GetFullPropName(Index: Integer): string;
begin
  Result := FEditors[Index].peEditor.GetName;
  while Index > 0 do
  begin
    if FEditors[Pred(Index)].peIdent <> FEditors[Index].peIdent then
      Result := FEditors[Pred(Index)].peEditor.GetName + '\' + Result;
    Dec(Index);
  end;
end;

procedure TDSOPropList.ChangeCurObj(const Value: ICommon);
var
  SavedPropName: string;
begin
  if (FCurrent >= 0) and (FCurrent < FPropCount) then
    SavedPropName := GetFullPropName(FCurrent)
  else SavedPropName := '';

  FDSOObject := Value;
  FreePropList;
  if not FDestroying then
  begin
    InitCurrent('');

    if Assigned(Value) then
    begin
      InitPropList;
      InitCurrent(SavedPropName);
      UpdateEditor(True);
    end;

    Invalidate;
    UpdateScrollRange;
  end;
end;

procedure TDSOPropList.SetMetaObj(const Value: TSchemaObject);
begin
  if FMetaObject <> Value then
  begin
    FMetaObject := Value;
  end;
end;

procedure TDSOPropList.SetDSOObj(const Value: ICommon);
begin
  if FDSOObject <> Value then
  begin
    if not Assigned(FMetaObject) then
      raise Exception.Create('Не указан мета-объект.');
    if Assigned(FOnNewObject) then FOnNewObject(Self, FDSOObject, Value);
    if not FDestroying then
      FInplaceEdit.Modified := False;
    FModified := False;
    ChangeCurObj(Value);
  end;
end;

procedure TDSOPropList.CMFontChanged(var Message: TMessage);
begin
  inherited;
  Canvas.Font := Font;
  FRowHeight := Canvas.TextHeight('Wg') + 3;
  Invalidate;
  UpdateScrollRange;
  FInplaceEdit.Move(GetEditRect);
end;

procedure TDSOPropList.UpdateScrollRange;
var
  si: TScrollInfo;
  diVisibleCount, diCurrentPos: Integer;
begin
  if not FHasScrollBar or not HandleAllocated or not Showing then Exit;

  { Temporarily mark us as not having scroll bars to avoid recursion }
  FHasScrollBar := False;
  try
    with si do
    begin
      cbSize := SizeOf(TScrollInfo);
      fMask := SIF_RANGE + SIF_PAGE + SIF_POS;
      nMin := 0;
      diVisibleCount := VisibleRowCount;
      diCurrentPos := FTopRow;

      if FPropCount <= diVisibleCount then
      begin
        nPage := 0;
        nMax := 0;
      end
      else
      begin
        nPage := diVisibleCount;
        nMax := FPropCount - 1;
      end;

      if diCurrentPos + diVisibleCount > FPropCount then
        diCurrentPos := EMax(0, FPropCount - diVisibleCount);
      nPos := diCurrentPos;
      FlatSB_SetScrollInfo(Handle, SB_VERT, si, True);
      MoveTop(diCurrentPos);
    end;
  finally
    FHasScrollBar := True;
  end;
end;

function TDSOPropList.VisibleRowCount: Integer;
begin
  if FRowHeight > 0 then // avoid division by zero
    Result := EMin(ClientHeight div FRowHeight, FPropCount)
  else
    Result := FPropCount;
end;

procedure TDSOPropList.WMSize(var Msg: TWMSize);
begin
  inherited;
  if FRowHeight <= 0 then Exit;
  ColumnSized(FVertLine);         // move divider if needed
  Invalidate;
  FInplaceEdit.UpdateLoc(GetEditRect);
  UpdateScrollRange;
end;

procedure TDSOPropList.ModifyScrollBar(ScrollCode: Integer);
var
  OldPos, NewPos, MaxPos: Integer;
  si: TScrollInfo;
begin
  OldPos := FTopRow;
  NewPos := OldPos;

  with si do
  begin
    cbSize := SizeOf(TScrollInfo);
    fMask := SIF_ALL;
    FlatSB_GetScrollInfo(Handle, SB_VERT, si);
    MaxPos := nMax - Integer(nPage) + 1;

    case ScrollCode of
      SB_LINEUP: Dec(NewPos);
      SB_LINEDOWN: Inc(NewPos);
      SB_PAGEUP: Dec(NewPos, nPage);
      SB_PAGEDOWN: Inc(NewPos, nPage);
      SB_THUMBPOSITION, SB_THUMBTRACK: NewPos := nTrackPos;
      SB_TOP: NewPos := nMin;
      SB_BOTTOM: NewPos := MaxPos;
      else Exit;
    end;

{    if NewPos < 0 then NewPos := 0;
    if NewPos > MaxPos then NewPos := MaxPos;}
    MoveTop(NewPos);
  end;
end;

procedure TDSOPropList.WMVScroll(var Msg: TWMVScroll);
begin
  ModifyScrollBar(Msg.ScrollCode);
end;

procedure TDSOPropList.MoveTop(NewTop: Integer);
var
  VertCount, ShiftY: Integer;
  ScrollArea: TRect;
begin
  if NewTop < 0 then NewTop := 0;
  VertCount := VisibleRowCount;
  if NewTop + VertCount > FPropCount then
    NewTop := FPropCount - VertCount;

  if NewTop = FTopRow then Exit;

  ShiftY := (FTopRow - NewTop) * FRowHeight;
  FTopRow := NewTop;
  ScrollArea := ClientRect;
  FlatSB_SetScrollPos(Handle, SB_VERT, NewTop, True);
  if Abs(ShiftY) >= VertCount * FRowHeight then
    InvalidateRect(Handle, @ScrollArea, True)
  else
    ScrollWindowEx(Handle, 0, ShiftY,
      @ScrollArea, @ScrollArea, 0, nil, SW_INVALIDATE);

  FInplaceEdit.Move(GetEditRect);
end;

function TDSOPropList.GetNameRect(ARow: Integer): TRect;
var
  RowStart: Integer;
begin
  RowStart := (ARow - FTopRow) * FRowHeight;
  Result := Rect(1, RowStart, FVertLine - 1, RowStart + FRowHeight - 1);
end;

function TDSOPropList.GetValueRect(ARow: Integer): TRect;
var
  RowStart: Integer;
begin
  RowStart := (ARow - FTopRow) * FRowHeight;
  Result := Rect(FVertLine + 1, RowStart, ClientWidth, RowStart + FRowHeight - 1);
end;

function TDSOPropList.GetEditRect: TRect;
begin
  Result := GetValueRect(FCurrent);
end;

procedure TDSOPropList.Paint;
var
  RedrawRect, NameRect, ValueRect, CurRect: TRect;
  FirstRow, LastRow, Y, RowIdx, CW, Offset: Integer;
  NameColor: TColor;
  DrawCurrent: Boolean;

  procedure DrawName(Index: Integer; R: TRect; XOfs: Integer);
  var
    S: string;
    E: PDSOEditor;
    BColor, PColor: TColor;
    YOfs: Integer;
    SavedColor: TColor;
  begin
    if FNewButtons then
    begin
      E := FEditors[Index];
      S := E.peEditor.GetName;
      Inc(XOfs, R.Left + E.peIdent * 10);
      SavedColor := Canvas.Font.Color;
      if E.peEditor.IsReadOnly then begin
        Canvas.Font.Color := clGrayText;
      end;
      ExtTextOut(Canvas.Handle, XOfs + 11, R.Top + 1,
        ETO_CLIPPED or ETO_OPAQUE, @R, PChar(S), Length(S), nil);
      Canvas.Font.Color := SavedColor;

      if E.peNode then
        with Canvas do
      begin
        BColor := Brush.Color;
        PColor := Pen.Color;
        Brush.Color := clWindow;
        Pen.Color := Font.Color;

        YOfs := R.Top + (FRowHeight - 9) shr 1;
        Rectangle(XOfs, YOfs, XOfs + 9, YOfs + 9);
        PolyLine([Point(XOfs + 2, YOfs + 4), Point(XOfs + 7, YOfs + 4)]);
        if not E.peExpanded then
          PolyLine([Point(XOfs + 4, YOfs + 2), Point(XOfs + 4, YOfs + 7)]);

        Brush.Color := BColor;
        Pen.Color := PColor;
      end;
    end
    else
    begin
      Canvas.TextRect(R, R.Left + XOfs, R.Top + 1, GetName(Index));
    end;
  end;

  function GetPenColor(Color: Integer): Integer;
  type
    TRGB = record
      R, G, B, A: Byte;
    end;
  begin
    // produce slightly darker color
    if Color < 0 then Color := GetSysColor(Color and $FFFFFF);
    Dec(TRGB(Color).R, EMin(TRGB(Color).R, $10));
    Dec(TRGB(Color).G, EMin(TRGB(Color).G, $10));
    Dec(TRGB(Color).B, EMin(TRGB(Color).B, $10));
    Result := Color;
  end;

begin
//try
  with Canvas do
  begin
    RedrawRect := ClipRect;
    FirstRow := RedrawRect.Top div FRowHeight;
    LastRow := EMin(FPropCount - FTopRow - 1, (RedrawRect.Bottom - 1) div FRowHeight);
    if LastRow + FTopRow = Pred(FCurrent) then Inc(LastRow); // Selection occupies 2 rows

    NameRect := Bounds(0, FirstRow * FRowHeight, FVertLine, FRowHeight - 1);
    ValueRect := NameRect;
    ValueRect.Left := FVertLine + 2;
    CW := ClientWidth;
    ValueRect.Right := CW;
    Brush.Color := Self.Color;
    Pen.Color := GetPenColor(Self.Color);
    Font := Self.Font;
    NameColor := Font.Color;
    DrawCurrent := False;
    for Y := FirstRow to LastRow do
    begin
      RowIdx := Y + FTopRow;
      Font.Color := NameColor;

      if RowIdx = FCurrent then
      begin
        CurRect := Rect(0, NameRect.Top - 2, CW, NameRect.Bottom + 1);
        DrawCurrent := True;
        Inc(NameRect.Left); // Space for DrawEdge
        DrawName(RowIdx, NameRect, 1);
        Dec(NameRect.Left);
      end
      else
      begin
        if RowIdx <> Pred(FCurrent) then
        begin
          Offset := 0;
          PolyLine([Point(0, NameRect.Bottom), Point(CW, NameRect.Bottom)]);
        end
        else
          Offset := 1;
        Dec(NameRect.Bottom, Offset);
        DrawName(RowIdx, NameRect, 2);
        Inc(NameRect.Bottom, Offset);
        Font.Color := FPropColor;
        FEditors[RowIdx].peEditor.PropDrawValue(Self.Canvas, ValueRect, False);
      end;
      OffsetRect(NameRect, 0, FRowHeight);
      OffsetRect(ValueRect, 0, FRowHeight);
    end;

    Dec(NameRect.Bottom, FRowHeight - 1);
    NameRect.Right := CW;
    ValueRect := Rect(FVertLine, RedrawRect.Top, 10, NameRect.Bottom - 1);
    DrawEdge(Handle, ValueRect, EDGE_ETCHED, BF_LEFT);

    if DrawCurrent then
    begin
      DrawEdge(Handle, CurRect, BDR_SUNKENOUTER, BF_LEFT + BF_BOTTOM + BF_RIGHT);
      DrawEdge(Handle, CurRect, EDGE_SUNKEN, BF_TOP);
    end;

    if NameRect.Bottom < RedrawRect.Bottom then
    begin
      Brush.Color := Self.Color;
      RedrawRect.Top := NameRect.Bottom;
      FillRect(RedrawRect);
    end;
  end;
//except
//  on E: Exception do ShowMessage('TDSOPropList.Paint: ' + E.Message)
//end
end;

procedure TDSOPropList.WMGetDlgCode(var Msg: TWMGetDlgCode);
begin
  Msg.Result := DLGC_WANTARROWS;
end;

procedure TDSOPropList.KeyDown(var Key: Word; Shift: TShiftState);
var
  PageHeight, NewCurrent: Integer;
begin
  inherited KeyDown(Key, Shift);
  NewCurrent := FCurrent;
  PageHeight := VisibleRowCount - 1;
  case Key of
    VK_UP: Dec(NewCurrent);
    VK_DOWN: Inc(NewCurrent);
    VK_NEXT: Inc(NewCurrent, PageHeight);
    VK_PRIOR: Dec(NewCurrent, PageHeight);
    else Exit;
  end;
  MoveCurrent(NewCurrent);
end;

procedure TDSOPropList.SetBorderStyle(Value: TBorderStyle);
begin
  if FBorderStyle <> Value then
  begin
    FBorderStyle := Value;
    RecreateWnd;
  end;
end;

procedure TDSOPropList.SetNewButtons(const Value: Boolean);
begin
  if FNewButtons <> Value then
  begin
    FNewButtons := Value;
    Invalidate;
  end;
end;

const
  Styles: array[TScrollBarStyle] of Integer = (FSB_REGULAR_MODE,
    FSB_ENCARTA_MODE, FSB_FLAT_MODE);

procedure TDSOPropList.CreateWnd;
begin
  inherited;
  ShowScrollBar(Handle, SB_BOTH, False);
  InitializeFlatSB(Handle);
  FlatSB_SetScrollProp(Handle, WSB_PROP_VSTYLE,
    Styles[FScrollBarStyle], False);

  if not (csDesigning in ComponentState) then
  begin
    FFormHandle := GetParentForm(Self).Handle;
    if FFormHandle <> 0 then
      FDefFormProc := Pointer(SetWindowLong(FFormHandle, GWL_WNDPROC,
        Integer(MakeObjectInstance(FormWndProc))));
  end;
end;

procedure TDSOPropList.DestroyWnd;
begin
  if FFormHandle <> 0 then
  begin
    SetWindowLong(FFormHandle, GWL_WNDPROC, Integer(FDefFormProc));
    FFormHandle := 0;
  end;
  inherited;
end;

procedure TDSOPropList.FormWndProc(var Message: TMessage);
begin
  with Message do
  begin
    if (Msg = WM_NCLBUTTONDOWN) or (Msg = WM_LBUTTONDOWN) then
      FInplaceEdit.FListButton.CloseUp(False);
    Result := CallWindowProc(FDefFormProc, FFormHandle, Msg, WParam, LParam);
  end;
end;

procedure TDSOPropList.MouseDown(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  if not (csDesigning in ComponentState) and
    (CanFocus or (GetParentForm(Self) = nil)) then SetFocus;

  if ssDouble in Shift then DblClick
  else
  begin
    FDividerHit := VertLineHit(X) and (Button = mbLeft);
    if not FDividerHit and (Button = mbLeft) then
    begin
      if not MoveCurrent(YToRow(Y)) then Exit;
      if FNewButtons and ButtonHit(X) then NodeClicked
      else
      begin
        FTracking := True;
        FInplaceEdit.FClickTime := GetMessageTime;
      end;
    end;
  end;

  inherited;
end;

procedure TDSOPropList.MouseMove(Shift: TShiftState; X, Y: Integer);
begin
  if FDividerHit then SizeColumn(X)
  else
    if FTracking and (ssLeft in Shift) then MoveCurrent(YToRow(Y));

  inherited;
end;

procedure TDSOPropList.MouseUp(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  FDividerHit := False;
  FTracking := False;
  inherited;
end;

function TDSOPropList.ColumnSized(X: Integer): Boolean;
var
  NewSizingPos: Integer;
begin
  NewSizingPos := EMax(MINCOLSIZE, EMin(X, ClientWidth - MINCOLSIZE));
  Result := NewSizingPos <> FVertLine;
  FVertLine := NewSizingPos
end;

procedure TDSOPropList.DblClick;
begin
  inherited;
  NodeClicked;
end;

procedure TDSOPropList.NodeClicked;
var
  Index, CurIdent, AddedCount, NewTop: Integer;
begin
// Expand|collapse node subproperties
  if (FCurrent >= 0) and (FEditors[FCurrent].peNode) then
    with FEditors[FCurrent]^ do
  begin
    if peExpanded then
    begin
      Index := FCurrent + 1;
      CurIdent := peIdent;
      while (Index < FEditors.Count) and
        (FEditors[Index].peIdent > CurIdent) do
      begin
        FEditors.DeleteEditor(Index);
        FEditors.Delete(Index);
      end
    end
    else
    begin
      FCurrentIdent := peIdent + 1;
      FCurrentPos := FCurrent + 1;
      try
        Editor.GetProperties(PropEnumProc);
      except
      end;
    end;

    peExpanded := not peExpanded;
    AddedCount := FEditors.Count - FPropCount;
    FPropCount := FEditors.Count;
    if AddedCount > 0 then  // Bring expanded properties in view
    begin
      Dec(AddedCount, VisibleRowCount - 1);
      if AddedCount > 0 then AddedCount := 0;
      NewTop := FCurrent + AddedCount;
      if NewTop > FTopRow then MoveTop(NewTop);
    end;
    Invalidate;
    UpdateScrollRange;
  end;
end;

function TDSOPropList.Editor: TPropertyEditor;
begin
  Result := FEditors[FCurrent].peEditor;
end;

procedure TDSOPropList.CMHintShow(var Msg: TCMHintShow);
var
  Row, W: Integer;
  S: string;
  W2: Integer;
begin
  with Msg, HintInfo^ do
  begin
    HideTimeout := FHintTimeout;
    Result := 1;
    Row := YToRow(CursorPos.Y);
    if Row >= FPropCount then exit;
    if (CursorPos.X > FVertLine) and (Row < FPropCount) then
    begin
      S := GetValue(Row);
      CursorRect := GetValueRect(Row);
      if Pos(#10, S) > 0 then         // Multiline string
        W := MaxInt
      else
      begin
        W := Canvas.TextWidth(S);
        W2 := W;
        FEditors[Row].peEditor.ListMeasureWidth(S, Canvas, W2);
        if W2 <> W then W := W2 + 4; // add extra space in case of custom drawing
      end;

      if W >= CursorRect.Right - CursorRect.Left - 1 then
      begin
        Inc(CursorRect.Bottom);
        HintPos := ClientToScreen(
          Point(CursorRect.Left - 1, CursorRect.Top - 2));
        HintStr := S;
        if Assigned(FOnHint) then FOnHint(Self, FEditors[Row].peEditor, HintInfo);
        Result := 0;
      end;
    end else begin
      S := FEditors[Row].peEditor.GetPropInfo.Caption;
      CursorRect := GetNameRect(Row);
      if Pos(#10, S) > 0 then         // Multiline string
        W := MaxInt
      else
      begin
        W := Canvas.TextWidth(S);
        W2 := W;
        FEditors[Row].peEditor.ListMeasureWidth(S, Canvas, W2);
        if W2 <> W then W := W2 + 4; // add extra space in case of custom drawing
      end;

      if W >= CursorRect.Right - CursorRect.Left - 1 then
      begin
        Inc(CursorRect.Bottom);
        HintPos := ClientToScreen(
          Point(CursorRect.Left - 1, CursorRect.Top - 2));
        HintStr := S;
        if Assigned(FOnHint) then FOnHint(Self, FEditors[Row].peEditor, HintInfo);
        Result := 0;
      end;
    end;
  end;
end;

function TDSOPropList.GetName(Index: Integer): string;
var
  Ident: Integer;
begin
  with FEditors[Index]^ do
  begin
    Ident := peIdent shl 1;
    if not peNode then Inc(Ident, 2);
    Result := peEditor.GetName;
    if peNode then
      if peExpanded then Result := '- ' + Result
      else Result := '+' + Result;
    Result := StringOfChar(' ', Ident) + Result;
  end;
end;

function TDSOPropList.GetValue(Index: Integer): string;
begin
  Result := FEditors[Index].peEditor.GetValue;
end;

function TDSOPropList.GetPrintableValue(Index: Integer): string;
var
  I: Integer;
  P: PChar;
begin
  Result := GetValue(Index);
  UniqueString(Result);
  P := Pointer(Result);
  for I := 0 to Length(Result) - 1 do
  begin
    if P^ < #32 then P^ := '.';
    Inc(P);
  end;
end;

procedure TDSOPropList.DoEdit(E: TPropertyEditor; DoEdit: Boolean; const Value: string);
var
  CanChange: Boolean;
//!!  Obj: Integer;
begin
  CanChange := True;
  if Assigned(FOnChanging) then
    FOnChanging(Self, E, CanChange);
  if CanChange then
  begin
//!!    Obj := 0;
//!!    if E is TClassProperty then Obj := THPropEdit(E).GetOrdValue;
    try
      E.LastException := '';
      if DoEdit then
        E.Edit
      else
        E.SetValue(Value);
    except
      on Exc: Exception do
        E.LastException := Exc.Message
    end;
//!!    if (E is TClassProperty) and (Obj <> THPropEdit(E).GetOrdValue)
//!!      and FEditors[FCurrent].peExpanded then NodeClicked; // collapse modified prop
    if Assigned(FOnChange) then FOnChange(Self, E);
  end;
end;

procedure TDSOPropList.SetValue(Index: Integer; const Value: string);
begin
  DoEdit(FEditors[Index].peEditor, False, Value);
end;

function TDSOPropList.GetPropType: TDSOPropType;
var
  Attr: TPropertyAttributes;
begin
  Result := ptSimple;
  if (FCurrent >= 0) and (FCurrent < FPropCount) then
  begin
    Attr := Editor.GetAttributes;
    if paValueList in Attr then Result := ptPickList
    else
      if paDialog in Attr then Result := ptEllipsis;
  end;
end;

procedure TDSOPropList.PropEnumProc(Prop: TPropertyEditor);
var
  P: PDSOEditor;
begin
  Prop.OnGetMacrosList := FOnGetMacrosList;
  New(P);
  P.peEditor := Prop;
  P.peIdent := FCurrentIdent;
  P.peExpanded := False;
  P.peNode := paSubProperties in Prop.GetAttributes;
  FEditors.Insert(FCurrentPos, P);
  Inc(FCurrentPos);
end;

procedure TDSOPropList.Edit;
begin
  if Editor.IsReadOnly then exit;
  DoEdit(Editor, True, '');
  UpdateEditor(False);
  Invalidate;            // repaint all dependent properties
end;

procedure TDSOPropList.UpdateEditor(CallActivate: Boolean);
var
  Attr: TPropertyAttributes;
begin
  if Assigned(FInplaceEdit) and (FCurrent >= 0) then
  with FInplaceEdit, Editor do
  begin
    if CallActivate then Activate;
    MaxLength := GetEditLimit;
    Attr := GetAttributes;
    ReadOnly := (paReadOnly in Attr) or IsReadOnly;
    FAutoUpdate := paAutoUpdate in Attr;
    Text := GetPrintableValue(FCurrent);
    SelectAll;
    Modified := False;
  end;
end;

function TDSOPropList.UpdateText(Exiting: Boolean): Boolean;
begin
  Result := True;
  if not FInUpdate and Assigned(FInplaceEdit) and
    (FCurrent >= 0) and (FInplaceEdit.Modified) then
  begin
    FInUpdate := True;
    try
      SetValue(FCurrent, FInplaceEdit.Text);
    except
      Result := False;
      FTracking := False;
      Application.ShowException(Exception(ExceptObject));
    end;
    if Exiting then UpdateEditor(False);
    Invalidate;            // repaint all dependent properties
    FInUpdate := False;
  end;
end;

procedure TDSOPropList.WMNCHitTest(var Msg: TWMNCHitTest);
begin
  inherited;
  FHitTest := ScreenToClient(SmallPointToPoint(Msg.Pos));
end;

function TDSOPropList.VertLineHit(X: Integer): Boolean;
begin
  Result := Abs(X - FVertLine) < 3;
end;

function TDSOPropList.MoveCurrent(NewCur: Integer): Boolean;
var
  NewTop, VertCount: Integer;
begin
  Result := UpdateText(True);
  if not Result then Exit;

  if NewCur < 0 then NewCur := 0;
  if NewCur >= FPropCount then NewCur := FPropCount - 1;
  if NewCur = FCurrent then Exit;

  InvalidateSelection;
  FCurrent := NewCur;
  InvalidateSelection;

  NewTop := FTopRow;
  VertCount := VisibleRowCount;
  if NewCur < NewTop then NewTop := NewCur;
  if NewCur >= NewTop + VertCount then NewTop := NewCur - VertCount + 1;

  FInplaceEdit.Move(GetEditRect);
  UpdateEditor(True);
  MoveTop(NewTop);
end;

function TDSOPropList.YToRow(Y: Integer): Integer;
begin
  Result := FTopRow + Y div FRowHeight;
end;

function TDSOPropList.ButtonHit(X: Integer): Boolean;
begin
// whether we hit collapse/expand button next to property with subproperties
  if FCurrent >= 0 then
  begin
    Dec(X, FEditors[FCurrent].peIdent * 10);
    Result := (X > 0) and (X < 12);
  end
  else
    Result := False;
end;

procedure TDSOPropList.SizeColumn(X: Integer);
begin
  if ColumnSized(X) then
  begin
    Invalidate;
    FInplaceEdit.UpdateLoc(GetEditRect);
  end;
end;

procedure TDSOPropList.InvalidateSelection;
var
  R: TRect;
  RowStart: Integer;
begin
  RowStart := (FCurrent - FTopRow) * FRowHeight;
  R := Rect(0, RowStart - 2, ClientWidth, RowStart + FRowHeight + 1);
  InvalidateRect(Handle, @R, True);
end;

end.
