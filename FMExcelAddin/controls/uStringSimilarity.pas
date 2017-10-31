unit uStringSimilarity;

interface

// ��������� ����� Str1 � Str2 ��� ����� ��������, ������������ �������� �
// ��������� ��������. � ���������� ������������ ������� ������� ����� 0..100%.
// MaxSubStrLen - ������������ ������ ��������, ������������ ��� ���������.
// ������������� ������������ ����� �������� MaxSubStrLen 3 ��� 4.
function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;



implementation

uses SysUtils;


function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;
// ��������� ����� Str1 � Str2 ��� ����� ��������, ������������ �������� �
// ��������� ��������. � ���������� ������������ ������� ������� ����� 0..100%.
// MaxSubStrLen - ������������ ������ ��������, ������������ ��� ���������.
// ������������� ������������ ����� �������� MaxSubStrLen 3 ��� 4.
var
  CurLen: Integer;      // ������� ����� ���������
  MatchCount: Integer;  // c������ ����������� ��������.
  SubStrCount: Integer; // ������� �������� � �������.

  procedure Matching(StrA, StrB: String; Len: Integer);
  // ��������� �������� ����� Len �� ����� StrA � StrB.
  var
    PosStrA, PosStrB, CurrentSubStrCount: Integer;
    SubStr: String;
  begin
    CurrentSubStrCount := Length(StrA) - Len + 1; // ���������� �������� � ������
    if CurrentSubStrCount > 0 then
      SubStrCount := SubStrCount + CurrentSubStrCount; // ����� ���������� ��������.
    for PosStrA := 1 to CurrentSubStrCount do begin
      SubStr := Copy(StrA, PosStrA, Len);
      PosStrB := Pos(SubStr, StrB);  // ���� �������� � ������ ������.
      if PosStrB > 0 then inc(MatchCount); // ������� ����������� ���������.
    end; // for PosStrA := 1 to CurrentSubStrCount
  end; // procedure Matching(StrA, StrB: String; Len: Integer);

begin // function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;
  Result := 0;
  // ���� �� ������� �����-���� ��������, �� �����
  if (MaxMatching = 0) or (Str1 = '') or (Str2 = '') then exit;
  // �������������� ��������.
  SubStrCount := 0;
  MatchCount := 0;
  // ���������� ����� ��� ����� ��������, ����������� �������� � ��������� ��������.
  Str1 := AnsiUpperCase(Trim(Str1));
  Str2 := AnsiUpperCase(Trim(Str2));
  // ���� ������ ���������, �� ��� ������ ���������� ����� ���������.
  if Str1 = Str2 then begin
    Result := 100;
    exit;
  end; // if Str1 = Str2

  // ���� ������� �� ����� ������������ �����
  for CurLen := 1 to MaxMatching do begin
    Matching(Str1, Str2, CurLen);  // ���������� ������ 1 �� ������� 2
    Matching(Str2, Str1, CurLen);  // ���������� ������ 2 �� ������� 1
  end; // for CurLen := 1 to MaxMatching

  if SubStrCount = 0 then exit;

  Result := Trunc((MatchCount / SubStrCount) * 100);
end;  // function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;


end.
