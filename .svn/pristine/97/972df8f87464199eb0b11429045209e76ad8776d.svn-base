(*
  ------------------------------------------------------------------------------
  MDX Эксперт, АС "Финансовый Анализ", НПО "Криста, 2004г.
  ------------------------------------------------------------------------------
  Компонент взят из пакета FlatStyle
  и постепенно будет перерабатываться под нас.
  ------------------------------------------------------------------------------
*)
unit fmFlatPanel;

interface

{$I DFS.inc}

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, FlatUtilitys;

type
  TfmSide		= ( fsdLeft, fsdTop, fsdRight, fsdBottom );
  TfmSides		= set of TfmSide;

  TfmFlatPanel = class(TCustomPanel)
  private
    FTransparent: Boolean;
    FBorderColorTop: TColor;
    FBorderColorRight: TColor;
    FBorderColorBottom: TColor;
    FBorderColorLeft: TColor;
    FBorderWidth: integer;
    FPanelSides: TfmSides;
    procedure SetColors (Index: Integer; Value: TColor);
    procedure CMEnabledChanged (var Message: TMessage); message CM_ENABLEDCHANGED;
    procedure CMTextChanged (var Message: TWmNoParams); message CM_TEXTCHANGED;
    procedure SetTransparent (const Value: Boolean);
    procedure SetPanelSides(Value: TfmSides);
    procedure DrowBorders(Canvas: TCanvas);
  protected
    procedure Paint; override;
  public
    constructor Create (AOwner: TComponent); override;
  published
    property PanelSides: TfmSides read FPanelSides write SetPanelSides
      default [ fsdLeft, fsdTop, fsdRight, fsdBottom ];
    property Transparent: Boolean read FTransparent write SetTransparent default false;
    property Caption;
    property Font;
    property Color;
    property ParentColor;
    property Enabled;
    property Visible;
    property BorderColorTop: TColor index 0 read FBorderColorTop write SetColors default $008396A0;
    property BorderColorRight: TColor index 1 read FBorderColorRight write SetColors default $008396A0;
    property BorderColorBottom: TColor index 2 read FBorderColorBottom write SetColors default $008396A0;
    property BorderColorLeft: TColor index 3 read FBorderColorLeft write SetColors default $008396A0;
    property BorderWidth: integer read FBorderWidth write FBorderWidth default 3;
    property Align;
    property Alignment;
    property Cursor;
    property Hint;
    property ParentShowHint;
    property ShowHint;
    property PopupMenu;
    property TabOrder;
    property TabStop;
   {$IFDEF DFS_DELPHI_4_UP}
    property AutoSize;
    property UseDockManager;
    property Anchors;
    property BiDiMode;
    property Constraints;
    property DragKind;
    property DragMode;
    property DragCursor;
    property ParentBiDiMode;
    property DockSite;
    property OnEndDock;
    property OnStartDock;
    property OnCanResize;
    property OnConstrainedResize;
    property OnDockDrop;
    property OnDockOver;
    property OnGetSiteInfo;
    property OnUnDock;
   {$ENDIF}
   {$IFDEF DFS_DELPHI_5_UP}
    property OnContextPopup;
   {$ENDIF}
    property OnClick;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnResize;
    property OnStartDrag;
  end;

procedure Register;

implementation

procedure Register;
begin
  RegisterComponents('FM Controls', [TfmFlatPanel]);
end;

constructor TfmFlatPanel.Create (AOwner: TComponent);
begin
  inherited Create(AOwner);
  ParentFont := True;

  FBorderColorTop := clGray;
  FBorderColorRight := clGray;
  FBorderColorBottom := clGray;
  FBorderColorLeft := clGray;

  BorderWidth := 1;

  ParentColor := True;
  ControlStyle := ControlStyle + [csAcceptsControls, csOpaque];
  SetBounds(0, 0, 185, 41);
  PanelSides := [ fsdLeft, fsdTop, fsdRight, fsdBottom ];
end;

procedure TfmFlatPanel.SetColors (Index: Integer; Value: TColor);
begin
  case Index of
    0: FBorderColorTop := Value;
    1: FBorderColorRight := Value;
    2: FBorderColorBottom := Value;
    3: FBorderColorLeft := Value;
  end;
  Invalidate;
end;

procedure TfmFlatPanel.Paint;
var
  memoryBitmap: TBitmap;
  textBounds: TRect;
  Format: UINT;
begin
  textBounds := ClientRect;
  Format := DT_SINGLELINE or DT_VCENTER;
  case Alignment of
    taLeftJustify:
      Format := Format or DT_LEFT;
    taCenter:
      Format := Format or DT_CENTER;
    taRightJustify:
      Format := Format or DT_RIGHT;
  end;

  memoryBitmap := TBitmap.Create; // create memory-bitmap to draw flicker-free
  try
    memoryBitmap.Height := ClientRect.Bottom;
    memoryBitmap.Width := ClientRect.Right;

    // Draw Background
    if FTransparent then
      DrawParentImage(Self, memoryBitmap.Canvas)
    else
    begin
      memoryBitmap.Canvas.Brush.Color := Self.Color;
      memoryBitmap.Canvas.FillRect(ClientRect);
    end;

    // Draw Border
//    Frame3DBorder(memoryBitmap.Canvas, ClientRect, FColorHighlight, FColorShadow, 1);
    DrowBorders(memoryBitmap.Canvas);

    // Draw Text
    memoryBitmap.Canvas.Font := Self.Font;
    memoryBitmap.Canvas.Brush.Style := bsClear;
    if not Enabled then
    begin
      OffsetRect(textBounds, 1, 1);
      memoryBitmap.Canvas.Font.Color := clBtnHighlight;
      DrawText(memoryBitmap.Canvas.Handle, PChar(Caption), Length(Caption), textBounds, Format);
      OffsetRect(textBounds, -1, -1);
      memoryBitmap.Canvas.Font.Color := clBtnShadow;
      DrawText(memoryBitmap.Canvas.Handle, PChar(Caption), Length(Caption), textBounds, Format);
    end
    else
      DrawText(memoryBitmap.Canvas.Handle, PChar(Caption), Length(Caption), textBounds, Format);

    // Copy memoryBitmap to screen
    canvas.CopyRect(ClientRect, memoryBitmap.canvas, ClientRect);

  finally
    memoryBitmap.free; // delete the bitmap
  end;
end;

procedure TfmFlatPanel.CMEnabledChanged(var Message: TMessage);
begin
  inherited;
  Invalidate;
end;

procedure TfmFlatPanel.CMTextChanged(var Message: TWmNoParams);
begin
  inherited;
  Invalidate;
end;

procedure TfmFlatPanel.SetTransparent(const Value: Boolean);
begin
  FTransparent := Value;
  Invalidate;
end;

procedure TfmFlatPanel.SetPanelSides(Value: TfmSides);
begin
  if FPanelSides=Value then exit;
  FPanelSides:=Value;
  Invalidate;
end;

procedure TfmFlatPanel.DrowBorders(Canvas: TCanvas);
var
  Rect: TRect;

  procedure DoRect;
  begin
    with Canvas, Rect do
    begin
      Pen.Width := BorderWidth;

      MoveTo(Left, Top);

      Pen.Color := FBorderColorTop;
      if fsdTop in FPanelSides then
        LineTo(Right, Top)
      else
        MoveTo(Right, Top);

      Pen.Color := FBorderColorRight;
      if fsdRight in FPanelSides then
        LineTo(Right, Bottom)
      else
        MoveTo(Right, Bottom);

      Pen.Color := FBorderColorBottom;
      if fsdBottom in FPanelSides then
        LineTo(Left, Bottom)
      else
        MoveTo(Left, Bottom);

      Pen.Color := FBorderColorLeft;
      if fsdLeft in FPanelSides then
        LineTo(Left, Top -1)
      else
        MoveTo(Left, Top);

{
      TopRight.X := Right;
      TopRight.Y := Top;
      BottomLeft.X := Left;
      BottomLeft.Y := Bottom;
      Pen.Color := FColorHighlight;
      PolyLine([BottomLeft, TopLeft, TopRight]);
      Pen.Color := FColorShadow;
      Dec(BottomLeft.X);
      PolyLine([TopRight, BottomRight, BottomLeft]);
}
    end;
  end;

begin
  Rect := ClientRect;
  Canvas.Pen.Width := 1;
  Dec(Rect.Bottom); Dec(Rect.Right);

//  Dec(Width);
  DoRect;
  InflateRect(Rect, -1, -1);

  Inc(Rect.Bottom); Inc(Rect.Right);
end;

end.
