unit uLevelSelector;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls, Menus;

type

  {точки привязки выносной линии}
  TExtensionPoint = (epStart, epMiddle, epEnd);

  {возможные состояния уровня}
  TLevelState = (lsDisabled, lsEnabled, lsForced);

  TLevelItem = record
    Name: string;
    State: TLevelState;
  end;

  TLevelsArray = array of TLevelItem;

  TSelectorHitTest = (shtSomewhere, shtStateImage, shtLabel, shtSelectBtn,
    shtDeselectBtn, shtExpandBtn, shtSecretArea);

  TOnLevelStateChangeEvent =
    procedure(Level: integer; State: TLevelState) of object;

  TOnLevelMembersCheckEvent =
    procedure(Level: integer; Checked: boolean) of object;

  TOnLevelExpand = procedure(Level: integer) of object;

  TLevelSelector = class(TCustomPanel)
  private
    FLevelsArray: TLevelsArray;
    {отступ от края до первого чекбокса}
    FLeftMargin: integer;
    {интервал между чекбоксами}
    FIndent: integer;
    {количество уровней}
    FCount: integer;
    {высота чекбокса - она же и ширина}
    FBoxHeight: integer;
    {высота надписи}
    FLabelHeight: integer;
    FLoading: boolean;
    FOnLevelStateChange: TOnLevelStateChangeEvent;
    FOnLevelMembersCheck: TOnLevelMembersCheckEvent;
    FOnLevelExpand: TOnLevelExpand;
    FOnTopLeftCornerClick: TNotifyEvent;
    FReadOnly: boolean;
    {контекстное меню состояния уровня}
    FLevelMenu: TPopupMenu;
    {индекс уровня полученный с помощью GetHitTestInfo}
    FLevelIndex: integer;
    {картинки с состояниями индикатора}
    StateImages: array[TLevelState] of TBitmap;

    {координаты чекбокса}
    function GetBoxLeft(Index: integer): integer;
    function GetBoxTop: integer;

    {координаты заголовка уровня}
    function GetLabelLeft(Index: integer): integer;
    function GetLabelTop(Index: integer): integer;
    function GetLabelRight(Index: integer): integer;

    {координаты узловых точек выносной линии}
    function GetExtensionLine(Index: integer; PointIndex: TExtensionPoint): TPoint;

    {координаты кнопок выделения [+] и [-]}
    function GetPlusButtonTop(Index: integer): integer;
    function GetPlusButtonLeft(Index: integer): integer;

    {обработчики кликов мышью}
    procedure WMLButtonDblClk(var Message: TWMLButtonUp); message WM_LBUTTONDBLCLK;
    procedure WMLButtonDown(var Message: TWMLButtonDown); message WM_LBUTTONDOWN;
    procedure WMLButtonUp(var Message: TWMLButtonUp); message WM_LBUTTONUP;
    procedure WMRButtonUp(var Message: TWMRButtonUp); message WM_RBUTTONUP;

    {отрисовка}
    procedure DrawLevel(Index: integer);
    {рисует выносную линию и выводит заголовок уровня}
    procedure DrawLevelCaption(Index: integer);
    {рисует чекбокс}
    procedure DrawStateImage(Index: integer);
    {рисует кнопку выделения всех элементов уровня [+]}
    procedure DrawPlusButton(Index: integer; Pressed: boolean);
    {рисует кнопку снятия выделения со всех элементов уровня [-]}
    procedure DrawMinusButton(Index: integer; Pressed: boolean);
    {рисует кнопку раскрытия уровня [>]}
    procedure DrawExpandButton(Index: integer; Pressed: boolean);

    {обработка контекстного меню уровней}
    procedure CreatePopup;
    procedure PopupHandler(Sender: TObject);
    procedure DoPopup(Sender: TObject; MousePos: TPoint;
      var Handled: boolean);

    procedure DoLevelStateChange(Level: integer; State: TLevelState);
    procedure DoLevelMembersCheck(Level: integer; Checked: boolean);
    procedure DoLevelExpand(Level: integer);
  protected
    procedure SetIndent(Value: integer);
    procedure Paint; override;
    procedure SetLeftMargin(Value: integer);
    procedure SetReadOnly(Value: boolean);
    function GetName(Index: integer): string;
    function GetState(Index: integer): TLevelState;
    procedure SetState(Index: integer; Value: TLevelState);
    function GetHitTestInfo(X, Y: integer): TSelectorHitTest;
    property LevelIndex: integer read FLevelIndex;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure AddLevel(AName: string; AState: TLevelState);
    procedure Clear;
    property Indent: integer read FIndent write SetIndent;
    property LeftMargin: integer read FLeftMargin write SetLeftMargin;
    property Names[Index: integer]: string read GetName;
    property States[Index: integer]: TLevelState read GetState write SetState;
    property Count: integer read FCount;
  published
    property Align;
    property BevelInner;
    property BevelOuter;
    property OnLevelStateChange: TOnLevelStateChangeEvent
      read FOnLevelStateChange write FOnLevelStateChange;
    property OnLevelMembersCheck: TOnLevelMembersCheckEvent
      read FOnLevelMembersCheck write FOnLevelMembersCheck;
    property OnTopLeftCornerClick: TNotifyEvent read FOnTopLeftCornerClick
      write FOnTopLeftCornerClick;
    property OnLevelExpand: TOnLevelExpand read FOnLevelExpand
      write FOnLevelExpand;
    property ReadOnly: boolean read FReadOnly write SetReadOnly;
  end;

procedure Register;

{$R LevelSelector.DCR}

implementation

constructor TLevelSelector.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FCount := 0;
  FIndent := 32;
  FLeftMargin := 8;
  FBoxHeight := 17;
  FLabelHeight := 17;
  FLoading := false;
  SetLength(FLevelsArray, 0);
  StateImages[lsDisabled] := TBitmap.Create;
  StateImages[lsDisabled].LoadFromResourceName(HInstance, 'LSDISABLED');
  StateImages[lsEnabled] := TBitmap.Create;
  StateImages[lsEnabled].LoadFromResourceName(HInstance, 'LSENABLED');
  StateImages[lsForced] := TBitmap.Create;
  StateImages[lsForced].LoadFromResourceName(HInstance, 'LSFORCED');
  CreatePopup;
  OnContextPopup := DoPopup;
end;

destructor TLevelSelector.Destroy;
begin
  SetLength(FLevelsArray, 0);
  StateImages[lsDisabled].Free;
  StateImages[lsEnabled].Free;
  StateImages[lsForced].Free;
  FLevelMenu.Items.Clear;
  FLevelMenu.Free;
  inherited Destroy;
end;

procedure TLevelSelector.SetIndent(Value: integer);
begin
  if (Value < 0) or (Value = FIndent) then
    exit;
  FIndent := Value;
  if FCount > 0 then
  begin
    Paint;
  end;
end;

procedure TLevelSelector.Paint;
var
  i: integer;
begin
  inherited Paint;
  for i := 0 to FCount - 1 do
    DrawLevel(i);
end;

procedure TLevelSelector.DrawLevelCaption(Index: integer);
var
  Rect: TRect;
  Flags: Longint;
  Point: TPoint;
begin
  with Canvas do
  begin
    {выносная линия}
    Pen.Color := clInactiveCaption;
    Point := GetExtensionLine(Index, epStart);
    MoveTo(Point.x, Point.y);
    Point := GetExtensionLine(Index, epMiddle);
    LineTo(Point.x, Point.y);
    Point := GetExtensionLine(Index, epEnd);
    LineTo(Point.x, Point.y);
    {надпись}
    Font := Self.Font;
    if States[Index] <> lsDisabled then
      Font.Color := clWindowText
    else
      Font.Color := clInactiveCaption;
    Rect.Left := GetLabelLeft(Index);
    Rect.Top := GetLabelTop(Index);
    Rect.Bottom := GetLabelTop(Index) + FLabelHeight;
    Rect.Right := GetLabelRight(Index);
    Flags := DT_LEFT or DT_BOTTOM or DT_SINGLELINE or DT_PATH_ELLIPSIS;
    DrawText(Handle, PChar(Names[Index]), -1, Rect, Flags);
  end;
end;

function TLevelSelector.GetBoxLeft(Index: integer): integer;
begin
  result := LeftMargin + Index * Indent;
end;

function TLevelSelector.GetBoxTop: integer;
begin
  result := Self.Height - 4 - FBoxHeight
end;

function TLevelSelector.GetLabelLeft(Index: integer): integer;
begin
  result := GetExtensionLine(Index, epEnd).x;
end;

function TLevelSelector.GetLabelTop(Index: integer): integer;
begin
  result := GetBoxTop - 8 - ((FCount - Index) * FLabelHeight);
end;

procedure TLevelSelector.SetLeftMargin(Value: integer);
begin
  if (Value = FLeftMargin) or not(Value in [1..Width])then
    exit;
  FLeftMargin := Value;
  if FCount > 0 then
  begin
    Paint;
  end;
end;

function TLevelSelector.GetExtensionLine(
  Index: integer; PointIndex: TExtensionPoint): TPoint;
begin
  case PointIndex of
    epStart:
      begin
        result.x := GetBoxLeft(Index) + 6;
        result.y := GetBoxTop - 4;
      end;
    epMiddle:
      begin
        result.x := GetBoxLeft(Index) + 6;
        result.y := GetLabelTop(Index) + FLabelHeight;
      end;
    epEnd:
      begin
//        result.x := GetBoxLeft(Index) + FBoxHeight - 4 + Indent;
        result.x := GetBoxLeft(Index) + FBoxHeight + 12  + Indent;
        result.y := GetLabelTop(Index) + FLabelHeight;
      end;
  end;
end;

function TLevelSelector.GetPlusButtonTop(Index: integer): integer;
begin
  result := GetExtensionLine(Index, epMiddle).y - 11;
end;

function TLevelSelector.GetPlusButtonLeft(Index: integer): integer;
begin
  result := GetExtensionLine(Index, epMiddle).x;
end;

procedure TLevelSelector.WMLButtonDblClk(var Message: TWMLButtonUp);
begin
  inherited;
  if GetHitTestInfo(Message.XPos, Message.YPos) = shtSecretArea then
    if Assigned(FOnTopLeftCornerClick) then
      FOnTopLeftCornerClick(Self);
end;

procedure TLevelSelector.SetReadOnly(Value: boolean);
begin
  FReadOnly := Value;
end;

procedure TLevelSelector.AddLevel(AName: string; AState: TLevelState);
begin
  inc(FCount);
  SetLength(FLevelsArray, FCount);
  FLevelsArray[FCount - 1].Name := AName;
  FLevelsArray[FCount - 1].State := AState;
  Self.Height := FCount * FLabelHeight + FBoxHeight + 16;
end;

function TLevelSelector.GetName(Index: integer): string;
begin
  if Index in [0..FCount - 1] then
    result := FLevelsArray[Index].Name
  else
    result := '';
end;

function TLevelSelector.GetState(Index: integer): TLevelState;
begin
  if Index in [0..FCount - 1] then
    result := FLevelsArray[Index].State
  else
    result := lsDisabled;
end;

procedure TLevelSelector.Clear;
begin
  FCount := 0;
  SetLength(FLevelsArray, 0);
end;

procedure TLevelSelector.DrawStateImage(Index: integer);
var
  X, Y: integer;
begin
  X := GetBoxLeft(Index);
  Y := GetBoxTop;
  Canvas.Draw(X, Y, StateImages[States[Index]]);
end;

function TLevelSelector.GetHitTestInfo(X, Y: integer): TSelectorHitTest;
var
  i, TestX, TestY, TestX2, TestY2: integer;
begin
  FLevelIndex := -1;
  result := shtSomewhere;
  if (X in [1..10]) and (Y in [1..10]) then
  begin
    result := shtSecretArea;
    exit;
  end;
  for i := 0 to FCount - 1 do
  begin
    {проверка на попадание по индикатору}
    TestX := GetBoxLeft(i);
    TestY := GetBoxTop;
    TestX2 := TestX + FBoxHeight;
    TestY2 := TestY + FBoxHeight;
    if (X >= TestX) and (X <= TestX2) and (Y >= TestY) and (Y <= TestY2) then
    begin
      result := shtStateImage;
      FLevelIndex := i;
      exit;
    end;
    {проверка на попадание по надписи}
    TestX := GetLabelLeft(i);
    TestY := GetLabelTop(i);
    TestX2 := GetLabelRight(i);
    TestY2 := TestY + FLabelHeight;
    if (X >= TestX) and (X <= TestX2) and (Y >= TestY) and (Y <= TestY2) then
    begin
      result := shtLabel;
      FLevelIndex := i;
      exit;
    end;
    {проверка на попадание по кнопкам выделения [+] или [-]}
    TestX := GetPlusButtonLeft(i);
    TestY := GetPlusButtonTop(i);
    TestX2 := TestX + 10;
    TestY2 := TestY + 10;
    if (X >= TestX) and (X <= TestX2) and (Y >= TestY) and (Y <= TestY2) then
    begin
      result := shtSelectBtn;
      FLevelIndex := i;
      exit;
    end;
    if (X >= TestX + 11) and (X <= TestX2 + 11) and
      (Y >= TestY) and (Y <= TestY2) then
    begin
      result := shtDeselectBtn;
      FLevelIndex := i;
      exit;
    end;
    if (X >= TestX + 22) and (X <= TestX2 + 22) and
      (Y >= TestY) and (Y <= TestY2) then
    begin
      result := shtExpandBtn;
      FLevelIndex := i;
      exit;
    end;
  end;
end;

function TLevelSelector.GetLabelRight(Index: integer): integer;
var
  Dc: Hdc;
  Str: string;
  StrSize: TSize;
begin
  Dc := GetDc(Handle);
  Str := Names[Index];
  try
    if GetTextExtentPoint32(Dc, PChar(Str), Length(Str), StrSize) then
      result := GetLabelLeft(Index) + StrSize.cx
    else
      result := Self.Width - 4;
  finally
    ReleaseDc(Handle, Dc);
  end;
end;

procedure TLevelSelector.WMLButtonDown(var Message: TWMLButtonDown);
var
  HTest: TSelectorHitTest;
begin
  inherited;
  HTest := GetHitTestInfo(Message.XPos, Message.YPos);
  case HTest of
    shtSelectBtn:
      DrawPlusButton(LevelIndex, true);
    shtDeselectBtn:
      DrawMinusButton(LevelIndex, true);
    shtExpandBtn:
      DrawExpandButton(LevelIndex, true);
  end;
end;

procedure TLevelSelector.WMLButtonUp(var Message: TWMLButtonUp);
var
  HTest: TSelectorHitTest;
  OldCursor: TCursor;
begin
  inherited;
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  HTest := GetHitTestInfo(Message.XPos, Message.YPos);
  case HTest of
    shtStateImage:
      if not ReadOnly then
        case States[LevelIndex] of
          lsDisabled: States[LevelIndex] := lsEnabled;
          lsEnabled: States[LevelIndex] := lsDisabled;
          lsForced: States[LevelIndex] := lsEnabled;
        end;
    shtSelectBtn:
      begin
        DrawPlusButton(LevelIndex, false);
        DoLevelMembersCheck(LevelIndex, true);
      end;
    shtDeselectBtn:
      begin
        DrawMinusButton(LevelIndex, false);
        DoLevelMembersCheck(LevelIndex, false);
      end;
    shtExpandBtn:
      begin
        DrawExpandButton(LevelIndex, false);
        DoLevelExpand(LevelIndex);
      end;
  end;
  Screen.Cursor := OldCursor;
end;

procedure TLevelSelector.WMRButtonUp(var Message: TWMRButtonUp);
var
  HTest: TSelectorHitTest;
  Point: TPoint;
begin
  HTest := GetHitTestInfo(Message.XPos, Message.YPos);
  inherited;
  if HTest = shtLabel then
  begin
    Point.x := Message.XPos;
    Point.y := Message.YPos;
    Point := Self.ClientToScreen(Point);
    FLevelMenu.Popup(Point.x, Point.y);
  end;
end;

procedure TLevelSelector.DrawLevel(Index: integer);
begin
  DrawStateImage(Index);
  DrawLevelCaption(Index);
  DrawPlusButton(Index, false);
  DrawMinusButton(Index, false);
  DrawExpandButton(Index, false);
end;

procedure TLevelSelector.SetState(Index: integer; Value: TLevelState);
begin
  FLevelsArray[Index].State := Value;
  DrawLevel(Index);
  DoLevelStateChange(Index, Value);
end;

procedure TLevelSelector.DrawPlusButton(Index: integer; Pressed: boolean);
var
  X, Y: integer;
begin
  X := GetPlusButtonLeft(Index);
  Y := GetPlusButtonTop(Index);
  with Canvas do
  begin
    Brush.Color := clBtnFace;
    FillRect(Rect(X, Y, X + 11, Y + 11));
    if Pressed then
    begin
      Inc(X, 1);
      Inc(Y, 2);
    end;
    Pen.Color := clInactiveCaption;
    MoveTo(X + 2, Y);
    LineTo(X, Y);
    LineTo(X, Y + 8);
    LineTo(X + 3, Y + 8);
    MoveTo(X + 6, Y + 8);
    LineTo(X + 8, Y + 8);
    LineTo(X + 8, Y);
    LineTo(X + 5, Y);

    MoveTo(X + 2, Y + 4);
    LineTo(X + 7, Y + 4);
    MoveTo(X + 4, Y + 2);
    LineTo(X + 4, Y + 7);
  end;
end;

procedure TLevelSelector.DrawMinusButton(Index: integer;
  Pressed: boolean);
var
  X, Y: integer;
begin
  X := GetPlusButtonLeft(Index) + 12;
  Y := GetPlusButtonTop(Index);
  with Canvas do
  begin
    Brush.Color := clBtnFace;
    FillRect(Rect(X, Y, X + 11, Y + 11));
    if Pressed then
    begin
      Inc(X, 1);
      Inc(Y, 2);
    end;
    Pen.Color := clInactiveCaption;
    MoveTo(X + 2, Y);
    LineTo(X, Y);
    LineTo(X, Y + 8);
    LineTo(X + 3, Y + 8);
    MoveTo(X + 6, Y + 8);
    LineTo(X + 8, Y + 8);
    LineTo(X + 8, Y);
    LineTo(X + 5, Y);

    MoveTo(X + 2, Y + 4);
    LineTo(X + 7, Y + 4);
  end;
end;

procedure TLevelSelector.DrawExpandButton(Index: integer;
  Pressed: boolean);
var
  X, Y: integer;
begin
  X := GetPlusButtonLeft(Index) + 24;
  Y := GetPlusButtonTop(Index);
  with Canvas do
  begin
    Brush.Color := clBtnFace;
    FillRect(Rect(X, Y, X + 11, Y + 11));
    if Pressed then
    begin
      Inc(X, 1);
      Inc(Y, 2);
    end;
    Pen.Color := clInactiveCaption;
    MoveTo(X + 2, Y);
    LineTo(X, Y);
    LineTo(X, Y + 8);
    LineTo(X + 3, Y + 8);
    MoveTo(X + 6, Y + 8);
    LineTo(X + 8, Y + 8);
    LineTo(X + 8, Y);
    LineTo(X + 5, Y);

    MoveTo(X + 3, Y + 3);
    LineTo(X + 5, Y + 3);
    LineTo(X + 5, Y + 5);
    LineTo(X + 3, Y + 5);
    LineTo(X + 3, Y + 3);

    MoveTo(X + 2, Y + 2);
    LineTo(X + 3, Y + 3);

    MoveTo(X + 6, Y + 2);
    LineTo(X + 5, Y + 3);

    MoveTo(X + 2, Y + 6);
    LineTo(X + 3, Y + 5);
    MoveTo(X + 6, Y + 6);
    LineTo(X + 5, Y + 5);
  end;
end;

procedure TLevelSelector.CreatePopup;
var
  PopupItems: array of TMenuItem;
begin
  SetLength(PopupItems, 6);
  PopupItems[0] := NewItem('Исключить уровень', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState0');
  PopupItems[1] := NewItem('Включить уровень', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState1');
  PopupItems[2] := NewItem('Включить уровень со всеми элементами', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState2');

  PopupItems[3] := NewItem('-', TextToShortcut(''), false, true,
    PopupHandler, 0, 'pmiState3');

  PopupItems[4] := NewItem('Выделить все элементы уровня', TextToShortcut(''),
    false, true, PopupHandler, 0, 'pmiState4');
  PopupItems[5] := NewItem('Снять выделение со всех элементов уровня',
    TextToShortcut(''), false, true, PopupHandler, 0, 'pmiState5');

  FLevelMenu := NewPopupMenu(Self, 'pmLevelMenu', paLeft, true, PopupItems);
end;

procedure TLevelSelector.PopupHandler(Sender: TObject);
var
  MenuIndex: integer;
  OldCursor: TCursor;
begin
  OldCursor := Screen.Cursor;
  Screen.Cursor := crHourglass;
  MenuIndex := TMenuItem(Sender).MenuIndex;
  case MenuIndex of
    0..2:
      if not ReadOnly then
        States[LevelIndex] := TLevelState(MenuIndex);
    4: DoLevelMembersCheck(LevelIndex, true);
    5: DoLevelMembersCheck(LevelIndex, false);
  end;
  Screen.Cursor := OldCursor;
end;

procedure TLevelSelector.DoPopup(Sender: TObject; MousePos: TPoint;
  var Handled: boolean);
begin
  if LevelIndex = -1 then
  begin
    Handled := true;
    exit;
  end;
  FLevelMenu.Items[0].Checked := false;
  FLevelMenu.Items[0].Enabled := not ReadOnly;
  FLevelMenu.Items[1].Checked := false;
  FLevelMenu.Items[1].Enabled := not ReadOnly;
  FLevelMenu.Items[2].Checked := false;
  FLevelMenu.Items[2].Enabled := not ReadOnly;
  FLevelMenu.Items[Ord(States[LevelIndex])].Checked := true;
end;

procedure TLevelSelector.DoLevelStateChange(Level: integer; State: TLevelState);
begin
  if Assigned(FOnLevelStateChange) then
    FOnLevelStateChange(Level, State);
end;

procedure TLevelSelector.DoLevelMembersCheck(Level: integer; Checked: boolean);
begin
  if Assigned(FOnLevelMembersCheck) then
    FOnLevelMembersCheck(Level, Checked);
end;

procedure Register;
begin
  RegisterComponents('FM Controls', [TLevelSelector]);
end;

procedure TLevelSelector.DoLevelExpand(Level: integer);
begin
  if Assigned(FOnLevelExpand) then
    FOnLevelExpand(Level);
end;

end.
