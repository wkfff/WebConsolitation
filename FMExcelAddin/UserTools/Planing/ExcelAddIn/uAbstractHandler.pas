{
  Модуль обработки "абстрактного исключения" исключения.
  В дельфях когда происходит вызов абстрактного метода сообщение об ошибке
  очень не информативное. Присутствие этого модуля в проекте, немного исправляет
  ситуацию.
}

unit uAbstractHandler;
{ версия 2.0 - добавлена поддержка классов в DLL
 version 1.01b - исправлена реализация GetVMTEnd}
interface
type
  // информация о виртуальном методе
  TMethodInfoRec = record
    ClassType: TClass; // класс, в котором декларирован метод
    VMTIndex: cardinal; // номер слота в VMT
  end;
  TMIRArray = array of TMethodInfoRec;

  TAbstractHandler = class
  private
    // замена стандартному обработчику абстрактных вызовов
    procedure HandleAbstract;
    // используем для получения адреса, хранящегося в VMT
    class procedure AbstractProc; virtual;abstract;
    // поиск класса-предка, в котором был декларирован заданный абстрактный метод
    class function GetFirstDeclarator(AClass: TClass; VMTIndex: integer): TClass;
  protected
    // вспомогательная функция для подготовки текста исключения
    // при форматировании для каждого метода в FormatStr подставляются:
    // - строка: имя класса
    // - число: номер слота VMT
    // полученные строки конкатенируются.
    class function FormatAbstractInfos(const Abstracts: Array of TMethodInfoRec;
      const FormatStr: String = 'Introduced in: %s; VMT: %d'#10#13): String;
  public
    // возвращает имя пакета, в котором декларирован указанный класс
    class function GetClassPackageName(AClass: TClass): String;
    // возвращает имя юнита, в котором декларирован указанный класс
    // возвращает Unknown, если в классе нет RTTI информации (см. доку по $M)
    class function GetClassUnitName(AClass: TClass): String;
    // возвращает истинный адрес функции, импортированной из DLL/BPL
    // Если Addr указывает на код, а не на импорт, то возвращаем Addr без изменений
    class function UnThunkImport(Addr: pointer): pointer;
    // возвращает адрес, записываемый в VMT для абстрактных методов
    class function AbsProcAddress: Pointer;
    // проверяет, является ли указанный метод абстрактным
    class function IsMethodAbstract(Method: Pointer): Boolean;
    // ищет абстрактные методы в VMT заданного класса. Возвращает истину,
    // если хоть один найден. Список найденных методов записывается в Abstracts
    class function DetectAbstracts(AClass: TClass; out Abstracts: TMIRArray): boolean;
    // бросает исключение, если в классе есть абстрактные методы
    class procedure AssertNonAbstract(AClass: TClass);
  end;


implementation
uses SysUtils, TypInfo, Windows;
var
  AbsProc: Pointer;
type PPointer = ^Pointer;

class function TAbstractHandler.AbsProcAddress: Pointer;
var
  TAP: procedure of object;
begin
  if not Assigned(AbsProc)
  then begin
    TAP:= self.AbstractProc;
    AbsProc:= UnThunkImport(TMethod(TAP).Code);
  end;
  Result:= AbsProc;
end;

class procedure TAbstractHandler.AssertNonAbstract(AClass: TClass);
var
  Abstracts: TMIRArray;
begin
   if DetectAbstracts(AClass, Abstracts)
    then raise EAbstractError.CreateFMT('Class %s (Unit: %s; package: %s) contains the following abstract methods:'#10#13
     +FormatAbstractInfos(Abstracts), [AClass.ClassName, GetClassUnitName(AClass), GetClassPackageName(AClass)]);
end;

// ищет адрес последней позиции в VMT
function GetVMTEnd(AClass: TClass): Pointer;
var
  VMT, Start, Finish: PPointer;
begin
  TClass(VMT):= AClass;
  Start:= VMT; Inc(Start, vmtIntfTable shr 2);
  Finish:= VMT; Inc(Finish,vmtClassName shr 2);
  Result:= Ptr($7FFFFFFF);

  while Integer(Start) <= Integer(Finish) do
  begin
    if (Integer(Start^)>Integer(VMT)) and (Integer(Start^) < Integer(Result))
      then Result:=Start^;
    Inc(Start);
  end;
end;
// Ищет класс-предок, в котором впервые заполнен заданный слот VMT
class function TAbstractHandler.GetFirstDeclarator(AClass: TClass; VMTIndex: integer): TClass;
var
  VMTEntry: PPointer;
begin
  Result:= AClass;
  while True do
  begin
    TClass(VMTEntry):= Result.ClassParent;
    Inc(VMTEntry, VMTIndex);
    // Если в предке этот слот уже заполнен, то он тоже
    // содержит AbsProcAddress:
    if (VMTEntry^)=AbsProcAddress
      then Result:= Result.ClassParent
      else Exit;
  end;
end;

class function TAbstractHandler.DetectAbstracts(AClass: TClass;
  out Abstracts: TMIRArray): boolean;
var
  VMT: PPointer;
  VMTEnd: Pointer;
begin
  TClass(VMT):= AClass;
  VMTEnd:=GetVMTEnd(AClass);

  SetLength(Abstracts, 0);
  while (VMT<>VMTEnd) // Сканируем VMT
  do begin
    if IsMethodAbstract(VMT^)
    then begin
      SetLength(Abstracts, Length(Abstracts)+1); // Добавляем запись
      with Abstracts[High(Abstracts)] do
      begin
        VMTIndex:= (Integer(VMT)-Integer(AClass)) shr 2; // размер слота - 4 байта
        ClassType:= GetFirstDeclarator(AClass, VMTIndex);
      end;
    end;
    Inc(VMT);
  end;
  Result:= Length(Abstracts)>0; // Сигнализируем, успешен ли поиск.
end;

class function TAbstractHandler.FormatAbstractInfos(
  const Abstracts: array of TMethodInfoRec;
  const FormatStr: String): String;
var
  i: integer;
begin
  Result:='';
  for i:= Low(Abstracts) to High(Abstracts) do
    with Abstracts[i] do
      Result:= Result+Format(FormatStr, [ClassType.ClassName, VMTIndex]);
end;
// наш обработчик выполняет детектирование всех абстрактных методов
procedure TAbstractHandler.HandleAbstract;
begin
  AssertNonAbstract(ClassType);
end;

class function TAbstractHandler.IsMethodAbstract(Method: Pointer): Boolean;
begin
  result:= UnThunkImport(Method)=AbsProcAddress;
end;

class function TAbstractHandler.UnThunkImport(Addr: pointer): pointer;
begin
  Result:=Addr;
  if Word(Addr^) = $25FF // это команда косвенного jmp
    then Result:= PPointer(PPointer(Integer(Addr)+2)^)^;
end;

class function TAbstractHandler.GetClassPackageName(
  AClass: TClass): String;
var
  M: TMemoryBasicInformation;
begin
  // Определяем хэндл DLL, которая владеет классом
  VirtualQuery(AClass, M, sizeof(M));
  SetLength(Result, MAX_PATH+1);
  if HMODULE(M.AllocationBase) <> HInstance // Если это не главная программа
  then begin
    GetModuleFileName(HMODULE(M.AllocationBase), PChar(Result), MAX_PATH);
    SetLength(Result, StrLen(Pchar(Result)));
    Result:= ExtractFileName(Result);
  end
  else
    Result:= 'Main Program';
end;

class function TAbstractHandler.GetClassUnitName(AClass: TClass): String;
var
  C: Pointer;
begin
  Result:= 'Unknown';
  C:= AClass.ClassInfo;
  if Assigned(C)
    then Result:= GetTypeData(C).UnitName;
end;

initialization
  AbsProc:= Nil; // Эта переменная использется для назначительной оптимизации
  // Устанавливаем наш обработчик:
  AbstractErrorProc:= Addr(TAbstractHandler.HandleAbstract);
  // Инициализируем указатель - иначе будет плохо
  TAbstractHandler.AbsProcAddress;
end.


