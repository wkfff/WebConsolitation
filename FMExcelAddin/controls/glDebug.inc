{$I glDEF.INC}

{$IFDEF glDEBUG}

function DeleteObject(p1: HGDIOBJ): BOOL; stdcall;
begin
  Result := glUtils.DeleteObject(p1);
end;

{$ENDIF}
