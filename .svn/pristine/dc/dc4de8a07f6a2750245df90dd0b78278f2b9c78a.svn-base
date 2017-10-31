unit brs_DbgAutoObject;

interface

uses comobj, windows, sysutils, classes;

type
  TDecodeKind = (dkWin1251, dkOEM);

  pCodePage = ^TCodePage;
  TCodePage = array [0..255] of byte;

  TDbgAutoObject = class(TAutoObject)
  private
    fDbgStr : string;
    fRememberedTime : TList;
  public
    procedure Initialize; override;
    destructor Destroy; override;
    procedure RememberTime;
    procedure DebugStr(ShowTime : boolean; Value : string);
    procedure DebugStrDec(ShowTime : boolean; Value : string; Kind : TDecodeKind);
  end;

  procedure DecodeString(CodeStr : string; Kind : TDecodeKind);

implementation

const Win1251 : TCodePage =
( $00, $01, $02, $03, $04, $05, $06, $07, $08, $09, $0a, $0b, $0c, $0d, $0e, $0f, $10, $11, $12, $13, $14,
  $15, $16, $17, $18, $19, $1a, $1b, $1c, $1d, $1e, $1f, $20, $21, $22, $23, $24, $25, $26, $27, $28, $29, $2a, $2b, $2c, $2d,
  $2e, $2f, $30, $31, $32, $33, $34, $35, $36, $37, $38, $39, $3a, $3b, $3c, $3d, $3e, $3f, $40, $41, $42, $43, $44, $45, $46,
  $47, $48, $49, $4a, $4b, $4c, $4d, $4e, $4f, $50, $51, $52, $53, $54, $55, $56, $57, $58, $59, $5a, $5b, $5c, $5d, $5e, $5f,
  $60, $61, $62, $63, $64, $65, $66, $67, $68, $69, $6a, $6b, $6c, $6d, $6e, $6f, $70, $71, $72, $73, $74, $75, $76, $77, $78,
  $79, $7a, $7b, $7c, $7d, $7e, $7f, $3f, $3f, $27, $3f, $22, $3a, $c5, $d8, $3f, $25, $3f, $3c, $3f, $3f, $3f, $3f, $3f, $27,
  $27, $22, $22, $07, $2d, $2d, $3f, $54, $3f, $3e, $3f, $3f, $3f, $3f, $ff, $f6, $f7, $3f, $fd, $3f, $b3, $15, $f0, $63, $f2,
  $3c, $bf, $2d, $52, $f4, $f8, $2b, $49, $69, $3f, $e7, $14, $fa, $f1, $fc, $f3, $3e, $3f, $3f, $3f, $f5, $80, $81, $82, $83,
  $84, $85, $86, $87, $88, $89, $8a, $8b, $8c, $8d, $8e, $8f, $90, $91, $92, $93, $94, $95, $96, $97, $98, $99, $9a, $9b, $9c,
  $9d, $9e, $9f, $a0, $a1, $a2, $a3, $a4, $a5, $a6, $a7, $a8, $a9, $aa, $ab, $ac, $ad, $ae, $af, $e0, $e1, $e2, $e3, $e4, $e5,
  $e6, $e7, $e8, $e9, $ea, $eb, $ec, $ed, $ee, $ef  );

function FindChar(sym : byte; Page : TCodePage) : byte;
var i : integer;
begin
  result := 0;
  for i := 0 to 255 do
    if sym = Page[i] then begin
      result := i;
      break
    end
end;

procedure DecodeString(CodeStr : string; Kind : TDecodeKind);
var i : integer;
    ls : integer;
begin
 ls := Length(CodeStr);
 case Kind of
   dkWin1251 : for i := 1 to ls do CodeStr[i] := chr(FindChar(ord(CodeStr[i]), Win1251));
   dkOEM : {OEMToChar}begin
//     CharLowerBuff(PChar(CodeStr), Length(CodeStr));
     CharToOEM(PChar(CodeStr), PChar(CodeStr));
   end;
 end;
end;

procedure TDbgAutoObject.Initialize;
begin
  fRememberedTime := TList.Create;
  inherited
end;

destructor TDbgAutoObject.Destroy;
begin
  fRememberedTime.Free;
  inherited
end;

procedure TDbgAutoObject.RememberTime;
begin
  fRememberedTime.Add(pointer(GetTickCount));
end;

procedure TDbgAutoObject.DebugStr(ShowTime : boolean; Value : string);
begin
  if ShowTime and (fRememberedTime.Count > 0) then begin
    fDbgStr := format('%s[$%0.8x].%s .. TIME=%d ms RC=%d ', [ClassName, integer(Self), Value,
                      GetTickCount - int64(fRememberedTime[fRememberedTime.Count - 1]), RefCount]) + #10#13;
    fRememberedTime.Delete(fRememberedTime.Count - 1);
  end
  else fDbgStr := format('%s[$%0.8x].%s .. RC=%d', [ClassName, integer(Self), Value, RefCount]) + #10#13;
  OutputDebugString(PChar(fDbgStr));
end;

procedure TDbgAutoObject.DebugStrDec(ShowTime : boolean; Value : string; Kind : TDecodeKind);
begin
  if ShowTime and (fRememberedTime.Count > 0) then begin
    fDbgStr := format('%s[$%0.8x].%s .. TIME=%d ms RC=%d ', [ClassName, integer(Self), Value,
                      GetTickCount - int64(fRememberedTime[fRememberedTime.Count - 1]), RefCount]) + #10#13;
    fRememberedTime.Delete(fRememberedTime.Count - 1);
  end
  else fDbgStr := format('%s[$%0.8x].%s .. RC=%d', [ClassName, integer(Self), Value, RefCount]) + #10#13;
  fDbgStr := format('%s[$%0.8x].%s .. RC=%d', [ClassName, integer(Self), Value, RefCount]) + #10#13;
  DecodeString(fDbgStr, Kind);
  OutputDebugString(PChar(fDbgStr));
end;

end.
