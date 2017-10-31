unit uStringsEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, uFMAddinGeneralUtils, stdctrls;

type

  {Представление строки сетки - кортеж строковых значений}
  TStringsEditorRow = class
  private
    FChecked: boolean;
    FValues: TStringList;
  protected
    function GetValue(Index: integer): string;
    procedure SetValue(Index: integer; const Value: string);
    constructor Create(ColumnsCount: integer);
  public
    destructor Destroy; override;
    property Checked: boolean read FChecked write FChecked;
    property Value[Index: integer]: string read GetValue write SetValue;
  end;

  TStringsEditorColumnInfo = record
    Name: string;
    Width: integer;
    ReadOnly: boolean;
  end;

  TStringsEditor = class;

  TCellPos = record
    Row: integer;
    Column: integer;
  end;

  {редактор ячейки.}
  TCellEditor = class(TCustomEdit)
  private
    FGrid: TStringsEditor;
    CellPos: TCellPos;
    procedure CMExit(var Message: TCMExit); message CM_EXIT;
  protected
    property  Grid: TStringsEditor read FGrid;
    procedure KeyPress(var Key: Char); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Setup(AGrid: TStringsEditor; Rect: TRect; Cell: TCellPos);
    procedure UpdateGridText;
  end;

  TImageEnum = (ieChecked, ieUnchecked);
  TStringsEditorHitTest = (sehtSomewhere, sehtCheckBox, sehtCell, sehtGridLine,
    sehtHeader, sehtFrame);

  {}
  TStringsEditor = class(TCustomControl)
  private
    FRows: TList;
    ColumnsCount: integer;
    Columns: array of TStringsEditorColumnInfo;
    Images: array[TImageEnum] of TBitmap;

    FCheckBoxes: boolean;
    FWindowsCheckBoxes: boolean;
    FRadioCheckBoxes: boolean;

    TopVisibleRowIndex: integer;

    FHGridLines: boolean;
    FVGridLines: boolean;

    FInterlaced: boolean;
    FInterlacedColor: TColor;

    SplitterMoving: boolean;
    SplitterIndex: integer;

    CellHit: TCellPos;
    SameCell: boolean;
    CellHasFocus: boolean;

    InEditMode: boolean;
    Editor: TCellEditor;
    FShowHeader: boolean;
    FLinesColor: TColor;
    FOnCheck: TNotifyEvent;

    DraggingRow: integer;
    FEnableDrag: boolean;


    procedure WMMouseMove(var Message: TWMMouseMove); message WM_MOUSEMOVE;
    procedure WMLButtonDown(var Message: TWMLButtonDown); message WM_LBUTTONDOWN;
    procedure WMLButtonUp(var Message: TWMLButtonUp); message WM_LBUTTONUP;
    procedure CMMouseLeave(var Message: TMessage); message CM_MOUSELEAVE;
    procedure WMVScroll(var Message: TWMVScroll); message WM_VSCROLL;
    procedure WMGetDlgCode(var Msg: TWMGetDlgCode); message WM_GETDLGCODE;
    procedure CMExit(var Message: TCMExit); message CM_EXIT;
    procedure CMEnter(var Message: TCMEnter); message CM_ENTER;



    procedure SetCheckBoxes(const Value: boolean);
    procedure SetHGridLines(const Value: boolean);
    procedure SetVGridLines(const Value: boolean);
    procedure SetInterlaced(const Value: boolean);
    procedure SetInterlacedColor(const Value: TColor);
    procedure SetWindowsCheckBoxes(const Value: boolean);
    procedure SetShowHeader(const Value: boolean);
    procedure SetLinesColor(const Value: TColor);

    function GetRowsCount: integer;
    function GetFontHeight: integer;
    function GetRowHeight: integer;
    function GetColumnLeftMargin(Index: integer): integer;
    procedure GetRowRect(Index: integer; var Rect: TRect);
    procedure GetCellRect(RowIndex, ColumnIndex: integer; var Rect: TRect); overload;
    procedure GetCellRect(RowRect: TRect; ColumnIndex: integer; var Rect: TRect); overload;
    procedure GetCheckBoxRect(Index: integer; var Rect: TRect);
    function GetBottomVisibleRowIndex(MaxPossible, FullVisible: boolean): integer;
    function GetFirstVLine: integer;

    procedure UpdateScrollInfo;
    procedure ScrollBarLineUp;
    procedure ScrollBarLineDown;
    procedure ScrollUp;
    procedure ScrollDown;
    procedure ScrollToPos(NewPos: integer);

    procedure ResizeColumn(Index: integer; NewRightMargin: integer);
    procedure UpdateCellHit(NewRow, NewColumn: integer);
    procedure ShowEditor;
    procedure HideEditor(UpdateText: boolean);
    procedure UncheckAll;
  protected
    procedure Paint; override;
    procedure DrawRow(Index: integer);
    procedure DrawCheckBox(Index: integer);
    procedure DrawGridLines(RowRect: TRect; Inversed: boolean);
    procedure DrawHeader;

    procedure CreateParams(var Params: TCreateParams); override;
    function GetRow(Index: integer): TStringsEditorRow;
    function GetRowIndexAt(X, Y: integer): integer;
    function GetHitTestInfo(X, Y: integer; Hit: boolean): TStringsEditorHitTest;
    procedure KeyPress(var Key: Char); override;
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
    procedure Resize; override;
    function DoMouseWheelUp(Shift: TShiftState; MousePos: TPoint): boolean; override;
    function DoMouseWheelDown(Shift: TShiftState; MousePos: TPoint): boolean; override;
    procedure DoCheck(RowIndex: integer);
    procedure DoStartDrag(var DragObject: TDragObject); override;
    procedure DragOver(Source: TObject; X, Y: Integer; State: TDragState; var Accept: Boolean); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    procedure AddColumn(ColumnName: string; ColumnWidth: integer; ReadOnly: boolean);
    procedure RemoveColumn(ColumnName: string);

    function AddRow(Values: array of string): TStringsEditorRow;
    procedure DeleteRow(Index: integer);

    {Очищает сетку. Если параметр равен тру, то убивает и настройку столбцов}
    procedure Clear(All: boolean);
    procedure ResetScrollPosition;

    procedure DragDrop(Source: TObject; X, Y: Integer); override;

    property Rows[Index: integer]: TStringsEditorRow read GetRow;
    property RowsCount: integer read GetRowsCount;
    property RadioCheckBoxes: boolean read FRadioCheckBoxes write FRadioCheckBoxes;
  published
    property Anchors;
    property CheckBoxes: boolean read FCheckBoxes write SetCheckBoxes;
    property WindowsCheckBoxes: boolean read FWindowsCheckBoxes write SetWindowsCheckBoxes;
    property Color default clWindow;
    property Font;
    property HGridLines: boolean read FHGridLines write SetHGridLines;
    property VGridLines: boolean read FVGridLines write SetVGridLines;
    property LinesColor: TColor read FLinesColor write SetLinesColor;
    property Interlaced: boolean read FInterlaced write SetInterlaced;
    property InterlacedColor: TColor read FInterlacedColor write SetInterlacedColor;
    property ShowHeader: boolean read FShowHeader write SetShowHeader;
    property OnCheck: TNotifyEvent read FOnCheck write FOnCheck;
    property EnableDrag: boolean read FEnableDrag write FEnableDrag;
    property TabOrder;
  end;


procedure Register;

{$R StringsEditor.dcr}

implementation

procedure Register;
begin
  RegisterComponents('Standard', [TStringsEditor]);
end;

{ TStringsEditor }

const
  CheckBoxSize = 13;
  Spacing = 4;
  ControlFrameWidth = 2;


procedure TStringsEditor.AddColumn(ColumnName: string; ColumnWidth: integer;
  ReadOnly: boolean);
begin
  //!! need check if column with this name already exists
  inc(ColumnsCount);
  SetLength(Columns, ColumnsCount);
  Columns[ColumnsCount - 1].Name := ColumnName;
  Columns[ColumnsCount - 1].Width := ColumnWidth;
  Columns[ColumnsCount - 1].ReadOnly := ReadOnly;
end;

function TStringsEditor.AddRow(Values: array of string): TStringsEditorRow;
var
  i: integer;
begin
  result := TStringsEditorRow.Create(ColumnsCount);
  FRows.Add(result);
  for i := 0 to ColumnsCount - 1 do
    result.Value[i] := Values[i];
end;

procedure TStringsEditor.Clear(All: boolean);
begin
  {очистка строк}
  while FRows.Count > 0 do
    DeleteRow(0);

  {очистка столбцов}
  if All then
  begin
    ColumnsCount := 0;
    SetLength(Columns, 0);
  end;

  if InEditMode then
  begin
    HideEditor(false);
    InEditMode := false;
  end;
  TopVisibleRowIndex := 0;
  Invalidate;
end;

procedure TStringsEditor.CMMouseLeave(var Message: TMessage);
begin
  SplitterMoving := false;
  Screen.Cursor := crDefault;
end;

constructor TStringsEditor.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  ParentColor := false;
  Color := clWindow;
  Brush.Color := Color;
  Width := 240;
  Height := 160;
  FInterlaced := false;
  FInterlacedColor := clBtnFace;
  FLinesColor := clBtnFace;

  FRows := TList.Create;
  ColumnsCount := 0;
  SetLength(Columns, 0);
  FCheckBoxes := true;
  FShowHeader := true;
  TopVisibleRowIndex := 0;
  TabStop := true;

  Images[ieChecked] := TBitmap.Create;
  Images[ieChecked].LoadFromResourceName(HInstance, 'CB_CHECKED');
  Images[ieUnchecked] := TBitmap.Create;
  Images[ieUnchecked].LoadFromResourceName(HInstance, 'CB_UNCHECKED');

  DoubleBuffered := true;

  UpdateCellHit(-1, -1);
  InEditMode := false;
  ControlStyle := ControlStyle - [csDoubleClicks];
  DragKind := dkDrag;
  DragMode := dmAutomatic;
  EnableDrag := true;
end;

procedure TStringsEditor.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
  with Params do
    Style := Style or WS_VSCROLL or WS_TABSTOP;
end;

procedure TStringsEditor.DeleteRow(Index: integer);
var
  Row: TStringsEditorRow;
begin
  Row := TStringsEditorRow(FRows[Index]);
  Row.Free;
  FRows.Delete(Index);
end;

destructor TStringsEditor.Destroy;
var
  i: TImageEnum;
begin
  Clear(true);
  FRows.Free;
  for i := Low(Images) to High(Images) do
    Images[i].Free;
  inherited;
end;

procedure TStringsEditor.DrawCheckBox(Index: integer);
var
  BoxRect: TRect;
  Style: integer;
  Checked: boolean;
begin
  GetCheckBoxRect(Index, BoxRect);
  Checked := Rows[Index].Checked;
  if WindowsCheckBoxes then
  begin
    Style := IIF(Checked, DFCS_CHECKED, DFCS_BUTTONCHECK);
    DrawFrameControl(Canvas.Handle, BoxRect, DFC_BUTTON, Style);
  end
  else
  begin
    if Checked then
      Canvas.Draw(BoxRect.Left, BoxRect.Top, Images[ieChecked])
    else
      Canvas.Draw(BoxRect.Left, BoxRect.Top, Images[ieUnchecked]);
  end;
end;

procedure TStringsEditor.DrawGridLines(RowRect: TRect; Inversed: boolean);
var
  i, LeftMargin, BottomMargin, LowestLimit: integer;
begin
  Canvas.Pen.Color := LinesColor;//cl3DDkShadow;
  Canvas.Pen.Style := psSolid;

  LowestLimit := Canvas.ClipRect.Bottom - ControlFrameWidth;
  BottomMargin := IIF(RowRect.Bottom > Canvas.ClipRect.Bottom,
    LowestLimit, RowRect.Bottom);

  if HGridLines then
  begin
    Canvas.MoveTo(RowRect.Left, BottomMargin);
    Canvas.LineTo(RowRect.Right + 1, BottomMargin);
  end;

  if LowestLimit - BottomMargin < GetRowHeight then
    BottomMargin := LowestLimit;

  if VGridLines then
  begin
    for i := GetFirstVLine to ColumnsCount - 1 do
    begin
      LeftMargin := GetColumnLeftMargin(i);

      Canvas.MoveTo(LeftMargin, RowRect.Top);
      Canvas.LineTo(LeftMargin, BottomMargin + 1);
    end;
  end;
end;

procedure TStringsEditor.DrawRow(Index: integer);
var
  RowRect, tmpRect: TRect;
  i: integer;
  Inversed: boolean;
begin
  GetRowRect(Index, RowRect);

  Inversed := false;
  Canvas.Brush.Style := bsSolid;
  Canvas.Brush.Color := Color;
  if Interlaced then
  begin
    if Index mod 2 = 1 then
    begin
      Inversed := true;
      Canvas.Brush.Color := InterlacedColor;
      inc(RowRect.Bottom);
      Canvas.FillRect(RowRect);
      dec(RowRect.Bottom);
    end
  end;

//  DrawGridLines(RowRect, Inversed);

  if Index >= RowsCount then
    exit;

  if CheckBoxes then
    DrawCheckBox(Index);

  Canvas.Font := Font;
  for i := 0 to ColumnsCount - 1 do
  begin
    GetCellRect(RowRect, i, tmpRect);
    with tmpRect do
    begin
      if Left > RowRect.Right then
        break;
      DrawText(Canvas.Handle, PChar(Rows[Index].Value[i]), -1, tmpRect,
        DT_LEFT or DT_BOTTOM or DT_NOPREFIX or DT_END_ELLIPSIS or
        DT_SINGLELINE or DT_EXTERNALLEADING);
      if Focused and CellHasFocus and (CellHit.Row = Index) and (CellHit.Column = i) then
        Canvas.DrawFocusRect(tmpRect);
      //Canvas.Brush.Color := clBlack;
      //Canvas.FrameRect(tmpRect);
    end;
  end;
  DrawGridLines(RowRect, Inversed);
end;

function TStringsEditor.GetBottomVisibleRowIndex(MaxPossible, FullVisible: boolean): integer;
var
  ClientRect: TRect;
begin
  ClientRect := GetClientRect;
  inc(ClientRect.Top, ControlFrameWidth);
  dec(ClientRect.Bottom, ControlFrameWidth);
  result := TopVisibleRowIndex + (ClientRect.Bottom - ClientRect.Top) div GetRowHeight;
  if FullVisible then
    dec(result);
  if ShowHeader then
    dec(result);
  if (result >= RowsCount) and not MaxPossible then
    result := RowsCount - 1;
end;

procedure TStringsEditor.GetCellRect(RowIndex, ColumnIndex: integer;
  var Rect: TRect);
var
  RowRect: TRect;
begin
  GetRowRect(RowIndex, RowRect);
  GetCellRect(RowRect, ColumnIndex, Rect);
end;

procedure TStringsEditor.GetCellRect(RowRect: TRect; ColumnIndex: integer;
  var Rect: TRect);
begin
  with Rect do
  begin
    Left := GetColumnLeftMargin(ColumnIndex) + 2;
    if ColumnIndex < ColumnsCount - 1 then
      Right := Left + Columns[ColumnIndex].Width - 2
    else
      Right := RowRect.Right - 2;
    Bottom := RowRect.Bottom;
    Top := Bottom - GetFontHeight;
  end;
end;

procedure TStringsEditor.GetCheckBoxRect(Index: integer; var Rect: TRect);
begin
  GetRowRect(Index, Rect);
  with Rect do
  begin
    inc(Left, Spacing);
    Right := Left + CheckBoxSize;
    dec(Bottom);
    Top := Bottom - CheckBoxSize;
  end;
end;

function TStringsEditor.GetColumnLeftMargin(Index: integer): integer;
var
  i: integer;
begin
  result := ControlFrameWidth + Spacing;
  if CheckBoxes then
    result := result + CheckBoxSize + Spacing;
  for i := 1 to Index do
    result := result + Columns[i - 1].Width;
end;

function TStringsEditor.GetFontHeight: integer;
begin
  result := Canvas.TextHeight('W');
end;

function TStringsEditor.GetHitTestInfo(X, Y: integer; Hit: boolean): TStringsEditorHitTest;
var
  i: integer;
  Rect: TRect;
  RowIndex, Deviation: integer;
begin
  result := sehtSomewhere;
  //DragMode := dmManual;

  if (X < ControlFrameWidth) or (X > Width - ControlFrameWidth) or
    (Y < ControlFrameWidth) or (Y > Height - ControlFrameWidth) then
  begin
    result := sehtFrame;
    exit;
  end;

  if VGridLines or (ShowHeader and (Y < ControlFrameWidth + GetRowHeight)) then
  begin
    for i := 1 to ColumnsCount - 1 do
    begin
      Deviation := Trunc(Abs(X - GetColumnLeftMargin(i)));
      if (Deviation in [0..4]) then
      begin
        result := sehtGridLine;
        SplitterIndex := i;
        exit;
      end;
    end;
  end;

  if not Hit then
    exit;

  RowIndex := GetRowIndexAt(X, Y);
  if ShowHeader and (RowIndex = -1) then
  begin
    result := sehtHeader;
    exit;
  end;

  GetCheckBoxRect(RowIndex, Rect);
  with Rect do
    if (X >= Left) and (X <= Right) and (Y >= Top) and (Y <= Bottom) then
    begin
      result := sehtCheckBox;
      exit;
    end;

  for i := 0 to ColumnsCount - 1 do
  begin
    GetCellRect(RowIndex, i, Rect);
    with Rect do
      if (X >= Left) and (X <= Right) and (Y >= Top) and (Y <= Bottom) then
      begin
        UpdateCellHit(RowIndex, i);
        result := sehtCell;
      //  DragMode := dmAutomatic;
        exit;
      end;
  end;

end;

function TStringsEditor.GetRow(Index: integer): TStringsEditorRow;
begin
  result := FRows[Index];
end;


function TStringsEditor.GetRowHeight: integer;
begin
  result := GetFontHeight;
  inc(result, Spacing);
//  if FHGridLines then
  //  inc(result, 2);
end;

function TStringsEditor.GetRowIndexAt(X, Y: integer): integer;
var
  Rect: TRect;
begin
  result := -1;
  Rect := GetClientRect;
  if (X <= ControlFrameWidth) or (X >= Rect.Right - ControlFrameWidth) then
    exit;
  Y := Y - ControlFrameWidth;
  result := (Y div GetRowHeight) + TopVisibleRowIndex;
  if ShowHeader then
    dec(result);
  if result >= RowsCount then
    result := -1;
end;

procedure TStringsEditor.GetRowRect(Index: integer; var Rect: TRect);
begin
  Rect := GetClientRect;
  with Rect do
  begin
    Left := Rect.Left + 2;
    Right := Rect.Right - 2;
    Top := Rect.Top + 2 + (Index - TopVisibleRowIndex) * GetRowHeight;
    if ShowHeader then
      Top := Top + GetRowHeight;
    Bottom := Top + GetRowHeight - 1;
  end;
end;

function TStringsEditor.GetRowsCount: integer;
begin
  result := FRows.Count;
end;

procedure TStringsEditor.Paint;
var
  Rect: TRect;
  i: integer;
begin
  {фон}
  Rect := GetClientRect;
  with Canvas do
  begin
    Font := Self.Font;
    Brush.Color := Color;
    Brush.Style := bsSolid;
    FillRect(Rect);
  end;

  {заголовки столбцов}
  if ShowHeader then
    DrawHeader;

  {собственно содержимое}
  for i := TopVisibleRowIndex to GetBottomVisibleRowIndex(true, false) do
    DrawRow(i);

  {внешняя граница}
  Canvas.Pen.Style := psSolid;
  Frame3D(Canvas, Rect, clBtnShadow, clBtnHighlight, 1);
  Frame3D(Canvas, Rect, cl3DDkShadow, clBtnFace, 1);
end;

procedure TStringsEditor.RemoveColumn(ColumnName: string);
begin
  ;
end;

procedure TStringsEditor.ResizeColumn(Index, NewRightMargin: integer);
var
  OldRightMargin: integer;
begin
  if Index < 0 then
    exit;
  with Columns[Index] do
  begin
    OldRightMargin := GetColumnLeftMargin(Index) + Width;
    Width := Width + NewRightMargin - OldRightMargin;
    if Width < Spacing then
      Width := Spacing;
  end;
  Invalidate;
end;

procedure TStringsEditor.SetCheckBoxes(const Value: boolean);
begin
  FCheckBoxes := Value;
  Invalidate;
end;

procedure TStringsEditor.SetHGridLines(const Value: boolean);
begin
  FHGridLines := Value;
  Invalidate;
end;

procedure TStringsEditor.SetInterlaced(const Value: boolean);
begin
  FInterlaced := Value;
  Invalidate;
end;

procedure TStringsEditor.SetInterlacedColor(const Value: TColor);
begin
  FInterlacedColor := Value;
  Invalidate;
end;

procedure TStringsEditor.SetVGridLines(const Value: boolean);
begin
  FVGridLines := Value;
  Invalidate;
end;

procedure TStringsEditor.SetWindowsCheckBoxes(const Value: boolean);
begin
  FWindowsCheckBoxes := Value;
  Invalidate;
end;

procedure TStringsEditor.UpdateCellHit(NewRow, NewColumn: integer);
begin
  if (CellHit.Row = NewRow) and (CellHit.Column = NewColumn) then
    SameCell := true
  else
  begin
    SameCell := false;
    CellHit.Row := NewRow;
    CellHit.Column := NewColumn;
  end;
end;

procedure TStringsEditor.ShowEditor;
var
  Rect: TRect;
begin
  Editor := TCellEditor.Create(Self);

  with CellHit do
  begin
    GetCellRect(Row, Column, Rect);
    Editor.Setup(Self, Rect, CellHit);
    if Interlaced and (Row mod 2 = 1) then
      Editor.Color := InterlacedColor;
  end;
  InEditMode := true;
end;

procedure TStringsEditor.UpdateScrollInfo;
var
  Info: SCROLLINFO;
  PossibleVisibleRowsCount: integer;
begin
  PossibleVisibleRowsCount := GetBottomVisibleRowIndex(true, true) - TopVisibleRowIndex + 1;
  Info.cbSize := SizeOf(SCROLLINFO);
  with Info do
  begin
    fMask := SIF_ALL;
    nMin := 0;
    nMax := RowsCount - PossibleVisibleRowsCount;
    if nMax < nMin then
    begin
      nMax := nMin;
      nPos := 0;
    end
    else
      nPos := TopVisibleRowIndex;
    nPage := 1;
    nTrackPos := nPos;
  end;
  if HandleAllocated then
    SetScrollInfo(Self.Handle, SB_VERT, Info, true);
end;

procedure TStringsEditor.WMLButtonDown(var Message: TWMLButtonDown);
var
  RowIndex: integer;
  HitTest: TStringsEditorHitTest;
begin
  if not (csDesigning in ComponentState) and CanFocus then
    SetFocus;
  HitTest := GetHitTestInfo(Message.XPos, Message.YPos, true);

  if HitTest = sehtCheckBox then
  begin
    RowIndex := GetRowIndexAt(Message.XPos, Message.YPos);
    DoCheck(RowIndex);
    Invalidate;
    exit;
  end;

  if HitTest = sehtGridLine then
  begin
    SplitterMoving := true;
    exit;
  end;

  if HitTest = sehtCell then
  begin
    if SameCell then
    begin
      if not InEditMode then
        if not Columns[CellHit.Column].ReadOnly then
        begin
          if (CellHit.Row = GetBottomVisibleRowIndex(false, false)) and
            (CellHit.Row > GetBottomVisibleRowIndex(false, true)) then
            ScrollDown;
          ShowEditor;
        end;
      exit;
    end
    else
    begin
      CellHasFocus := not Columns[CellHit.Column].ReadOnly;
      Invalidate;
    end;
  end;
end;

procedure TStringsEditor.WMLButtonUp(var Message: TWMLButtonUp);
begin
  SplitterMoving := false;
end;

procedure TStringsEditor.WMMouseMove(var Message: TWMMouseMove);
var
  HitTest: TStringsEditorHitTest;
begin
  HitTest := GetHitTestInfo(Message.XPos, Message.YPos, true);

  if HitTest = sehtCell then
    DragMode := dmAutomatic
  else
    DragMode := dmManual;

  if HitTest = sehtFrame then
    SplitterMoving := false;

  if (HitTest = sehtGridLine) or SplitterMoving then
  begin
    Screen.Cursor := crHSplit;
    if (Message.Keys and MK_LBUTTON = MK_LBUTTON) then
      ResizeColumn(SplitterIndex - 1, Message.XPos);
  end
  else
    Screen.Cursor := crDefault;
end;

procedure TStringsEditor.WMVScroll(var Message: TWMVScroll);
begin
  case Message.ScrollCode of
    SB_LINEUP, SB_PAGEUP: ScrollUp;
    SB_LINEDOWN, SB_PAGEDOWN: ScrollDown;
    SB_THUMBTRACK: ScrollToPos(Message.Pos);
  end;
end;

procedure TStringsEditor.HideEditor(UpdateText: boolean);
begin
  if not Assigned(Editor) then
    exit;
  if UpdateText then
    Editor.UpdateGridText;
  Editor.Free;
  Invalidate;
  InEditMode := false;
end;

procedure TStringsEditor.KeyPress(var Key: Char);
begin
  inherited;
  case Key of
    #27:
      if InEditMode then
        HideEditor(false);
    #13:
      if InEditMode then
        HideEditor(true)
      else
        ShowEditor;
  end;
end;

procedure TStringsEditor.KeyDown(var Key: Word; Shift: TShiftState);
begin
  inherited;
  case Key of
    VK_UP:
      if CellHasFocus and not InEditMode and (CellHit.Row > 0) then
      begin
        dec(CellHit.Row);
        if CellHit.Row < TopVisibleRowIndex then
        begin
          dec(TopVisibleRowIndex);
          ScrollBarLineUp;
        end;
        Invalidate;
      end;
    VK_DOWN:
      if CellHasFocus and not InEditMode and (CellHit.Row < RowsCount - 1) then
      begin
        inc(CellHit.Row);
        if CellHit.Row > GetBottomVisibleRowIndex(true, true) then
        begin
          inc(TopVisibleRowIndex);
          ScrollBarLineDown;
        end;
        Invalidate;
      end;
  end;
end;

procedure TStringsEditor.WMGetDlgCode(var Msg: TWMGetDlgCode);
begin
  Msg.Result := DLGC_WANTARROWS;
end;

procedure TStringsEditor.SetShowHeader(const Value: boolean);
begin
  FShowHeader := Value;
  Invalidate;
end;

procedure TStringsEditor.DrawHeader;
var
  Rect, CaptionRect: TRect;
  i, LeftMargin: integer;
begin
  GetRowRect(TopVisibleRowIndex - 1, Rect);
  with Canvas do
  begin
    Brush.Color := clBtnFace;
    FillRect(Rect);
    with Rect do
    begin
      Pen.Color := cl3DDkShadow;
      MoveTo(Left, Bottom);
      LineTo(Right + 1, Bottom);

      Pen.Color := clBtnShadow;
      MoveTo(Left, Bottom - 1);
      LineTo(Right + 1, Bottom - 1);

      Pen.Color := clWindow;
      MoveTo(Left, Bottom - 2);
      LineTo(Left, Top);
      LineTo(Right, Top);

      for i := 0 to ColumnsCount - 1 do
      begin
        LeftMargin := GetColumnLeftMargin(i);
        CaptionRect := Classes.Rect(LeftMargin + 2, Top + 2,
          LeftMargin + Columns[i].Width - 2, Bottom - 2);
        DrawText(Canvas.Handle, PChar(Columns[i].Name), -1, CaptionRect,
          DT_LEFT or DT_BOTTOM or DT_NOPREFIX or DT_END_ELLIPSIS or
          DT_SINGLELINE or DT_EXTERNALLEADING);

        if i < GetFirstVLine then
          continue;

        Pen.Color := cl3DDkShadow;
        MoveTo(LeftMargin, Top);
        LineTo(LeftMargin, Bottom);

        Pen.Color := clWindow;
        MoveTo(LeftMargin + 1, Top + 1);
        LineTo(LeftMargin + 1, Bottom);

        Pen.Color := clBtnShadow;
        MoveTo(LeftMargin - 1, Top + 1);
        LineTo(LeftMargin - 1, Bottom - 1);
      end;
    end;
  end;
end;

function TStringsEditor.GetFirstVLine: integer;
begin
  //result := IIF(CheckBoxes, 0, 1);
  result := 1;
end;

procedure TStringsEditor.CMExit(var Message: TCMExit);
begin
  inherited;
  //UpdateCellHit(-1, -1);
  CellHasFocus := false;
  Invalidate;
end;

procedure TStringsEditor.ScrollBarLineDown;
var
  ScrollPos: integer;
begin
  ScrollPos := GetScrollPos(Handle, SB_VERT);
  inc(ScrollPos);
  SetScrollPos(Handle, SB_VERT, ScrollPos, true);
end;

procedure TStringsEditor.ScrollBarLineUp;
var
  ScrollPos: integer;
begin
  ScrollPos := GetScrollPos(Handle, SB_VERT);
  dec(ScrollPos);
  SetScrollPos(Handle, SB_VERT, ScrollPos, true);
end;

procedure TStringsEditor.Resize;
begin
  inherited;
  UpdateScrollInfo;
end;

procedure TStringsEditor.ScrollDown;
begin
  if GetBottomVisibleRowIndex(false, true) < RowsCount - 1 then
  begin
    inc(TopVisibleRowIndex);
    ScrollBarLineDown;
    Invalidate;
  end;
end;

procedure TStringsEditor.ScrollUp;
begin
  if TopVisibleRowIndex > 0 then
  begin
    dec(TopVisibleRowIndex);
    ScrollBarLineUp;
    Invalidate;
  end;
end;

procedure TStringsEditor.ScrollToPos(NewPos: integer);
begin
  TopVisibleRowIndex := NewPos;
  SetScrollPos(Handle, SB_VERT, NewPos, true);
  Invalidate;
end;

function TStringsEditor.DoMouseWheelDown(Shift: TShiftState;
  MousePos: TPoint): Boolean;
begin
  ScrollDown;
  result := true;
end;

function TStringsEditor.DoMouseWheelUp(Shift: TShiftState;
  MousePos: TPoint): Boolean;
begin
  ScrollUp;
  result := true;
end;

procedure TStringsEditor.CMEnter(var Message: TCMEnter);
begin
  inherited;
  CellHasFocus := true;
  Invalidate;
end;

procedure TStringsEditor.SetLinesColor(const Value: TColor);
begin
  FLinesColor := Value;
  Invalidate;
end;

procedure TStringsEditor.UncheckAll;
var
  i: integer;
begin
  if not CheckBoxes then
    exit;
  for i := 0 to RowsCount - 1 do
    Rows[i].Checked := false;
end;

procedure TStringsEditor.DoCheck(RowIndex: integer);
begin
  if RadioCheckBoxes then
    UncheckAll;
  Rows[RowIndex].Checked := not Rows[RowIndex].Checked;
  if Assigned(FOnCheck) then
    FOnCheck(Rows[RowIndex]);
end;

procedure TStringsEditor.DoStartDrag(var DragObject: TDragObject);
var
  Point: TPoint;
  HitTest: TStringsEditorHitTest;
begin
  if not EnableDrag then
  begin
    CancelDrag;
    exit;
  end;
  Point := Mouse.CursorPos;
  Point := Self.ScreenToClient(Point);
  HitTest := GetHitTestInfo(Point.X, Point.Y, true);
  if HitTest <> sehtCell then
  begin
    CancelDrag;
    exit;
  end;
  inherited DoStartDrag(DragObject);
  DraggingRow := GetRowIndexAt(Point.X, Point.Y);
  if DraggingRow = -1 then
    CancelDrag;
end;

procedure TStringsEditor.DragOver(Source: TObject; X, Y: Integer; State: TDragState; var Accept: Boolean);
var
  DestRow: integer;
begin
  inherited;
  DestRow := GetRowIndexAt(X, Y);
  Accept := DestRow <> -1;
end;

procedure TStringsEditor.DragDrop(Source: TObject; X, Y: Integer);
var
  DestRow: integer;
begin
  inherited;
  DestRow := GetRowIndexAt(X, Y);
  if DestRow = -1 then
    exit;
  FRows.Move(DraggingRow, DestRow);
  Invalidate;
end;

procedure TStringsEditor.ResetScrollPosition;
begin
  ScrollToPos(0);
end;

{ TStringsEditorRow }

constructor TStringsEditorRow.Create(ColumnsCount: integer);
var
  i: integer;
begin
  FValues := TStringList.Create;
  for i := 0 to ColumnsCount - 1 do
    FValues.Add('');
  FChecked := false;
end;

destructor TStringsEditorRow.Destroy;
begin
  FValues.Clear;
  FValues.Free;
  inherited;
end;

function TStringsEditorRow.GetValue(Index: integer): string;
begin
  result := FValues[Index];
end;

procedure TStringsEditorRow.SetValue(Index: integer; const Value: string);
begin
  FValues[Index] := Value;
end;

{ TCellEdit }

procedure TCellEditor.CMExit(var Message: TCMExit);
begin
  {Post}SendMessage(Grid.Handle, WM_CHAR, 13{27}, 0);
end;

constructor TCellEditor.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  ParentCtl3D := False;
  Ctl3D := False;
  TabStop := False;
  BorderStyle := bsNone;
  DoubleBuffered := False;
end;

destructor TCellEditor.Destroy;
begin
  FGrid := nil;
  inherited Destroy;
end;

procedure TCellEditor.KeyPress(var Key: Char);
begin
  inherited;
  case Key of
    #13: PostMessage(Grid.Handle, WM_CHAR, 13, 0);
    #27: PostMessage(Grid.Handle, WM_CHAR, 27, 0);
  end;
end;

procedure TCellEditor.Setup(AGrid: TStringsEditor; Rect: TRect; Cell: TCellPos);
begin
  FGrid := AGrid;
  Parent := AGrid;
  Text := Grid.Rows[Cell.Row].Value[Cell.Column];
  CellPos := Cell;

  CreateHandle;
  Invalidate;
  with Rect do
    SetWindowPos(Handle, HWND_TOP, Left, Top, Right - Left, Bottom - Top,
      SWP_SHOWWINDOW or SWP_NOREDRAW);
  Rect := Classes.Rect(2, 2, Width - 2, Height);
  SendMessage(Handle, EM_SCROLLCARET, 0, 0);
  Invalidate;
  Windows.SetFocus(Handle);
  SelectAll;
end;

procedure TCellEditor.UpdateGridText;
begin
  Grid.Rows[CellPos.Row].Value[CellPos.Column] := Text;
end;

end.


