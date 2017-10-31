(*
  ------------------------------------------------------------------------------
  MDX Эксперт, АС "Финансовый Анализ", НПО "Криста, 2004г.
  ------------------------------------------------------------------------------
  Компонент взят из пакета Globus Delphi VCL Extensions Library
  и постепенно будет перерабатываться под нас.
  ------------------------------------------------------------------------------
*)
unit fmSpeedButton;

interface
{$I glDEF.INC}
uses
  Windows, Messages, Classes, Controls, Graphics, glTypes, glCommCl,
  glUtils, ExtCtrls, buttons, stdctrls, Forms, ActnList,
  menus;
type

  TfmSpeedButton = class(TSpeedButton)
  private
    FCanvas: TCanvas;
    fMouseEnter: boolean;
    IsDown: Boolean;
    FControl: TControl;
    FFrame: boolean;
    FCaptionLabel: TLabel;
    FDefaultStyle: boolean;
    FModalResult: TModalResult;
    FFrameColorActive: TColor;
    FFrameColor: TColor;
    procedure CMMouseEnter(var Message: TMessage); message CM_MOUSEENTER;
    procedure CMMouseLeave(var Message: TMessage); message CM_MOUSELEAVE;
    procedure SetControl(const Value: TControl);
    procedure SetFrame(const Value: boolean);
    procedure SetCaptionLabel(const Value: TLabel);
    procedure SetDefaultStyle(const Value: boolean);
    procedure SetEnabled(const Value: boolean); reintroduce;
    function GetEnabled: boolean; reintroduce;
    procedure SetColor(const Value: TColor);
    procedure SetFrameColorActive(const Value: TColor);
    procedure SetFrameColor(const Value: TColor);
  protected
    FColor: TColor;
    FActiveColor: TColor;
    procedure Paint; override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState; X, Y: Integer); override;
    procedure SetPushed(Value: Boolean);
  public
    constructor Create( AOwner : TComponent ); override;
    destructor Destroy; override;
    procedure Click; override;
    property Pushed: Boolean read IsDown write SetPushed;
  published
    property Color: TColor read FColor write SetColor default clBlack;
    property ActiveColor: TColor read FActiveColor write FActiveColor default clBlack;
    property Control: TControl read FControl write SetControl;
    property CaptionLabel: TLabel read FCaptionLabel write SetCaptionLabel;
    property Frame: boolean read FFrame write SetFrame default true;
    property FrameColorActive: TColor read FFrameColorActive write SetFrameColorActive;
    property FrameColor: TColor read FFrameColor write SetFrameColor;
    property DefaultStyle: boolean read FDefaultStyle write SetDefaultStyle;
    property Enabled: boolean read GetEnabled write SetEnabled;
    property ModalResult: TModalResult read FModalResult write FModalResult;
  end;

  TfmExtSpeedButton = class(TfmSpeedButton)
  private
    FStyle: TglSpeedButtonStyle;
    FStyleActive: TglSpeedButtonStyle;
    FStylePushed: TglSpeedButtonStyle;
    FFMStyle: boolean;
    FDefault:boolean;
    FCancel:boolean;
    procedure CMDialogKey(var Message: TCMDialogKey); message CM_DIALOGKEY;
    procedure CMMouseEnter(var Message: TMessage); message CM_MOUSEENTER;
    procedure CMMouseLeave(var Message: TMessage); message CM_MOUSELEAVE;
    procedure SetColor(const Value: TColor);
    procedure SetActiveColor(const Value: TColor);
    function GetFont: TFont;
    procedure SetFont(const Value: TFont);
    function GetActiveColor: TColor;
    function GetColor: TColor;
    procedure SetFMStyle(const Value: boolean);
  protected
    procedure Paint; override;
    procedure OnChanged(Sender: TObject);
  public
    constructor Create( AOwner : TComponent ); override;
    destructor Destroy; override;
  published
    property Style: TglSpeedButtonStyle read FStyle write FStyle;
    property StyleActive: TglSpeedButtonStyle read FStyleActive write FStyleActive;
    property StylePushed: TglSpeedButtonStyle read FStylePushed write FStylePushed;
    property Font: TFont read GetFont write SetFont;
    property Color: TColor read GetColor write SetColor stored false;
    property ActiveColor: TColor read GetActiveColor write SetActiveColor stored false;
    property FMStyle: boolean read FFMStyle write SetFMStyle default false;
    property Cancel: boolean read FCancel write FCancel default false;
    property Default: boolean read FDefault write FDefault default false;
  end;

procedure Register;

implementation
{~~~~~~~~~~~~~~~~~~~~~~~~~}
procedure Register;
begin
  RegisterComponents('FM Controls', [TfmExtSpeedButton]);
end;
{~~~~~~~~~~~~~~~~~~~~~~~~~}

//________________________________________________________ Methods _

{ TfmSpeedButton }
constructor TfmSpeedButton.Create( AOwner : TComponent );
begin
  inherited;
  FCanvas := TControlCanvas.Create;
  TControlCanvas(FCanvas).Control := Self;//...i can draw now! :)
  //..defaults
  FColor :=  IncColor(GetSysColor(COLOR_BTNFACE), 30);
  FActiveColor :=  IncColor(FColor, 10);
  FFrame := true;
end;

destructor TfmSpeedButton.Destroy;
begin
  FCanvas.Free;
  inherited;
end;

procedure TfmSpeedButton.SetPushed(Value: Boolean);
begin
  IsDown := Value;
  Down := Value;
  Invalidate;
end;

procedure TfmSpeedButton.Paint;
var
  R: TRect;
  BevelOuter:TPanelBevel;
begin
  if DefaultStyle then
  begin
    inherited Paint;
    exit;
  end;
  if SystemColorDepth < 16 then FColor := GetNearestColor(Canvas.handle, FColor);

  R := ClientRect;

  if IsDown and fMouseEnter then BevelOuter := bvLowered else BevelOuter := bvRaised;
  if Flat and not IsDown then
    BevelOuter := bvNone;

  if FFrame then InflateRect(R, -1, -1);
  dec(R.Right); dec(R.Bottom);
  DrawBoxEx(Canvas.handle, R, ALLGLSIDES, bvNone, BevelOuter, false, iif(fMouseEnter, ActiveColor, Color), false);

  SetBkMode(Canvas.handle, integer(TRANSPARENT));

  Canvas.Font.Assign(Font);
  if not Enabled then Canvas.Font.Color := clGrayText;
  if Assigned(Glyph) then inc(R.Left, Glyph.Width);

  if IsDown then OffsetRect(R, 1, 1);
  DrawText(Canvas.handle, PChar(Caption), length(Caption), R, DT_SINGLELINE or DT_CENTER or DT_VCENTER);

  R := ClientRect;
  Canvas.Brush.Color := 0;
  if FFrame then
  begin
    Canvas.Font.Color := FFrameColorActive;
    Canvas.FrameRect(R);
  end;

  if Assigned(Glyph) then
    CreateBitmapExt( Canvas.Handle, Glyph, ClientRect, (Width - Glyph.Width - Canvas.TextWidth(Caption))div 2 + integer(IsDown) - 1-Spacing, 1+(Height - Glyph.Height)div 2 + integer(IsDown),
		     fwoNone, fdsDefault,
		     true, GetTransparentColor(Glyph, ftcLeftBottomPixel), 0 );

end;

procedure TfmSpeedButton.CMMouseEnter(var Message: TMessage);
begin
  inherited;
  if not Enabled then exit;
  fMouseEnter := true;
  if IsDown or (Color <> ActiveColor) then Invalidate;
end;

procedure TfmSpeedButton.CMMouseLeave(var Message: TMessage);
begin
  inherited;
  if not Enabled then exit;
  fMouseEnter := false;
  if IsDown or (Color <> ActiveColor) then Invalidate;
end;

procedure TfmSpeedButton.MouseDown(Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  inherited;
  if not Enabled then exit;
  IsDown := true;
  Invalidate;
end;

procedure TfmSpeedButton.MouseUp(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);
begin
  inherited MouseUp(Button, Shift,X,Y);
  if not Enabled then exit;
  IsDown := false;
  Invalidate;
end;

procedure TfmSpeedButton.Click;
var
  Form: TCustomForm;
begin
  inherited;
  if not Enabled then exit;
  if ModalResult = mrNone then exit;
  Form := GetParentForm(Self);
  if Form <> nil then Form.ModalResult := ModalResult;
end;

procedure TfmSpeedButton.SetControl(const Value: TControl);
begin
  FControl := Value;
end;

procedure TfmSpeedButton.SetFrame(const Value: boolean);
begin
  FFrame := Value;
  Invalidate;
end;


procedure TfmSpeedButton.SetCaptionLabel(const Value: TLabel);
begin
  FCaptionLabel := Value;
  Invalidate;
end;

procedure TfmSpeedButton.SetDefaultStyle(const Value: boolean);
begin
  FDefaultStyle := Value;
  Invalidate;
end;

procedure TfmSpeedButton.SetEnabled(const Value: boolean);
begin
  if not Value then
    FMouseEnter := false;
  inherited Enabled := Value;
  if Assigned(FControl) then
    FControl.Enabled := Value
end;

function TfmSpeedButton.GetEnabled: boolean;
begin
  Result := inherited Enabled;
end;

procedure TfmSpeedButton.SetColor(const Value: TColor);
begin
  FColor := Value;
  Invalidate;
end;

procedure TfmSpeedButton.SetFrameColorActive(const Value: TColor);
begin
  FFrameColorActive := Value;
  Invalidate;
end;

procedure TfmSpeedButton.SetFrameColor(const Value: TColor);
begin
  FFrameColor := Value;
  Invalidate;
end;


{ TfmExtSpeedButton }

constructor TfmExtSpeedButton.Create( AOwner : TComponent );
begin
  inherited;
  FStyle := TglSpeedButtonStyle.Create;
  FStyleActive := TglSpeedButtonStyle.Create;
  FStylePushed := TglSpeedButtonStyle.Create;

  FStyle.OnChanged := OnChanged;
  FStyleActive.OnChanged := OnChanged;
  FStylePushed.OnChanged := OnChanged;
  //..defaults
  FStyle.Color := incColor(clBtnFace, 30);
  FStyleActive.Color := IncColor(FStyle.Color, 10);
  FStylePushed.Color := DecColor(FStyle.Color, 10);
  FStyle.Bevel.Inner := bvRaised;
  FStyleActive.Bevel.Inner := bvRaised;
  FStylePushed.Bevel.Inner := bvLowered;
end;

destructor TfmExtSpeedButton.Destroy;
begin
  FStyle.Free;
  FStyleActive.Free;
  FStylePushed.Free;
  inherited;
end;

procedure TfmExtSpeedButton.Paint;
var
  R: TRect;
  offset: integer;
  _Style: TglSpeedButtonStyle;
  _Caption: string;
  _Glyph: TBitmap;
  _Action: TAction;
  function TextStyle: TglTextStyle;
  begin
    if Enabled then Result := _Style.TextStyle else Result := fstPushed;
  end;
begin

  {!!! заплата. Если кнопка связана с TAction, то выбрасываем Caption) }
  if Assigned(Action) then
    _Caption := ''
  else
    _Caption := Caption;

//  if Name = 'btSave' then
//    _Glyph := Glyph;

  _Glyph := Glyph;
  {Вытаскиваем картинку из ActionList-a}
  _Action := (Action as TAction);

  if (_Action <> nil) and (not _Action.Enabled) then
    fMouseEnter := false;

  if _Action <> nil then
    with _Action do
      ActionList.Images.GetBitmap(ImageIndex, _Glyph);

//showmessage(intTostr(_Action.ImageIndex))
//  _Action.ActionList.Images.GetBitmap(_Action.ImageIndex, _Glyph);

  if DefaultStyle then
  begin
    inherited Paint;
    exit;
  end;
  R := ClientRect;


  if IsDown and fMouseEnter then
    _Style := StylePushed
  else
    if Enabled and (fMouseEnter or Down) then
    _Style := StyleActive
  else
    _Style := Style;

  if FFrame then InflateRect(R, -1, -1);
  dec(R.Right); dec(R.Bottom);

  with _Style do
  begin
    R := DrawBoxEx( Canvas.Handle, R, Bevel.Sides, Bevel.Inner, Bevel.Outer, Bevel.Bold, Color, Gradient.Active );
    if Gradient.Active then
    begin
      inc(R.Right); inc(R.Bottom);
      Gradient.Draw(Canvas.Handle, R, integer(psSolid), 1);
      dec(R.Right); dec(R.Bottom);
    end;
  end;

  if not _Glyph.Empty then
    inc(R.Left, _Glyph.Width);

  Canvas.Font.Assign(_Style.Font);
  if IsDown then offset := 1 else offset := 0;
  ExtTextOutExt( Canvas.Handle, R.Left+offset+(R.Right - R.Left -Canvas.TextWidth(_Caption)) div 2, R.Top+offset+(R.Bottom - R.Top - Canvas.TextHeight(_Caption)) div 2, R, _Caption,
  		 TextStyle, false{ fcoDelineatedText in Options},
		 false, _Style.Font.Color, _Style.DelineateColor,
		 _Style.HighlightColor, _Style.ShadowColor,
		 nil, _Style.TextGradient, _Style.Font );

  R := ClientRect;
  Canvas.Brush.Color := 0;

  if FFrame then begin
    if ((IsDown and fMouseEnter) or fMouseEnter or Down) and Enabled then
      Canvas.Brush.Color := FFrameColorActive
    else
      Canvas.Brush.Color := FFrameColor;
    Canvas.FrameRect(R);
  end;


  //Зверев. Очень временная заплата. Надо разбираться.
  //Суть в том что если кэпшэна нету, то картинка рисуется не по центру (первый вариант родной)

  if _Caption <> '' then begin
    if Assigned(_Glyph) then
      CreateBitmapExt( Canvas.Handle, _Glyph, ClientRect, (Width - _Glyph.Width - Canvas.TextWidth(_Caption))div 2 + integer(IsDown) - 1-Spacing, 1+(Height - _Glyph.Height)div 2 + integer(IsDown),
           fwoNone, fdsDefault,
           true, GetTransparentColor(_Glyph, ftcLeftBottomPixel), 0 );
  end else begin
    if Assigned(_Glyph) then
      CreateBitmapExt(
        Canvas.Handle,
        _Glyph,
        ClientRect,
        (Width - _Glyph.Width) div 2 + integer(IsDown),
        (Height - _Glyph.Height) div 2 + integer(IsDown) ,
        fwoNone,
        fdsDefault,
        true,
        GetTransparentColor(_Glyph, ftcLeftBottomPixel),
        0 );
  end;



end;

procedure TfmExtSpeedButton.CMMouseEnter(var Message: TMessage);
begin
  inherited;
  if not Enabled then exit;
  fMouseEnter := true;
  Paint;
end;

procedure TfmExtSpeedButton.CMMouseLeave(var Message: TMessage);
begin
  inherited;
  if not Enabled then exit;
  fMouseEnter := false;
  Paint;
end;


procedure TfmExtSpeedButton.OnChanged(Sender: TObject);
begin
  if csLoading in ComponentState then exit;
  Invalidate;
end;

procedure TfmExtSpeedButton.SetColor(const Value: TColor);
begin
  Style.Color := Value;
  Invalidate;
end;

procedure TfmExtSpeedButton.SetActiveColor(const Value: TColor);
begin
  StyleActive.Color := Value;
  Invalidate;
end;

function TfmExtSpeedButton.GetActiveColor: TColor;
begin
  Result := StyleActive.Color;
end;

function TfmExtSpeedButton.GetColor: TColor;
begin
  Result := Style.Color;
end;

function TfmExtSpeedButton.GetFont: TFont;
begin
  Result := inherited Font;
end;

procedure TfmExtSpeedButton.SetFont(const Value: TFont);
begin
  inherited Font.Assign(Font);
  Style.Font.Assign(Font);
end;

procedure TfmExtSpeedButton.SetFMStyle(const Value: boolean);
begin
  FFMStyle := Value;
  if Value then begin
    Frame := true;
    Color := clBtnFace;
    ActiveColor := $00D2BDB6;
    FrameColor := clGray;
    FrameColorActive := $006B2408;


    with Style do begin
      Bevel.Inner := bvNone;
      Bevel.Outer := bvNone;
//      Color := clBtnFace; //clWindow;
    end;

    with StyleActive do begin
      Bevel.Inner := bvNone;
      Bevel.Outer := bvNone;
      Color := $00D2BDB6;
    end;

    with StylePushed do begin
      Bevel.Inner := bvNone;
      Bevel.Outer := bvNone;
      Color := $00B59285;
      Font.Color := clWhite;
    end;
  end;

  Invalidate;
end;
// реакция на Enter & Escape
procedure TfmExtSpeedButton.CMDialogKey(var Message: TCMDialogKey);
begin
  with Message do
    if  (((CharCode = VK_RETURN) and FDefault) or
      ((CharCode = VK_ESCAPE) and FCancel)) and
      (KeyDataToShiftState(Message.KeyData) = []) and Visible and Enabled then
    begin
      Click;
      Result := 1;
    end else
      inherited;
end;

end.
