unit uStringSimilarity;

interface

// Сравнение строк Str1 и Str2 без учёта регистра, обрамляющийх пробелов и
// служебных символов. В результате возвращается процент подобия строк 0..100%.
// MaxSubStrLen - максимальный размер подстрок, используемых при сравнении.
// Рекомендуемая максимальная длина подстрок MaxSubStrLen 3 или 4.
function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;



implementation

uses SysUtils;


function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;
// Сравнение строк Str1 и Str2 без учёта регистра, обрамляющийх пробелов и
// служебных символов. В результате возвращается процент подобия строк 0..100%.
// MaxSubStrLen - максимальный размер подстрок, используемых при сравнении.
// Рекомендуемая максимальная длина подстрок MaxSubStrLen 3 или 4.
var
  CurLen: Integer;      // текущая длина подстроки
  MatchCount: Integer;  // cчётчик совпадающих подстрок.
  SubStrCount: Integer; // счётчик подстрок в строках.

  procedure Matching(StrA, StrB: String; Len: Integer);
  // Сравнение подстрок длины Len из строк StrA и StrB.
  var
    PosStrA, PosStrB, CurrentSubStrCount: Integer;
    SubStr: String;
  begin
    CurrentSubStrCount := Length(StrA) - Len + 1; // Количество подстрок в строке
    if CurrentSubStrCount > 0 then
      SubStrCount := SubStrCount + CurrentSubStrCount; // Общее количество подстрок.
    for PosStrA := 1 to CurrentSubStrCount do begin
      SubStr := Copy(StrA, PosStrA, Len);
      PosStrB := Pos(SubStr, StrB);  // Ищем построку в другой строке.
      if PosStrB > 0 then inc(MatchCount); // Считаем совпадающие подстроки.
    end; // for PosStrA := 1 to CurrentSubStrCount
  end; // procedure Matching(StrA, StrB: String; Len: Integer);

begin // function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;
  Result := 0;
  // если не передан какой-либо параметр, то выход
  if (MaxMatching = 0) or (Str1 = '') or (Str2 = '') then exit;
  // Инициализируем счётчики.
  SubStrCount := 0;
  MatchCount := 0;
  // Сравнивать будем без учёта регистра, обрамляющих пробелов и служебных символов.
  Str1 := AnsiUpperCase(Trim(Str1));
  Str2 := AnsiUpperCase(Trim(Str2));
  // Если строки совпадают, но нет смысла сравнивать через подстроки.
  if Str1 = Str2 then begin
    Result := 100;
    exit;
  end; // if Str1 = Str2

  // Цикл прохода по длине сравниваемой фразы
  for CurLen := 1 to MaxMatching do begin
    Matching(Str1, Str2, CurLen);  // Сравниваем строку 1 со строкой 2
    Matching(Str2, Str1, CurLen);  // Сравниваем строку 2 со строкой 1
  end; // for CurLen := 1 to MaxMatching

  if SubStrCount = 0 then exit;

  Result := Trunc((MatchCount / SubStrCount) * 100);
end;  // function Similarity(Str1, Str2: String; MaxMatching: Integer): Integer;


end.
