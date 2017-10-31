unit brs_SortLvlList;

interface

uses classes, sysutils, windows, adomd_tlb, adodb_tlb, comobj,
  // модуль с масками бюджетовских классификаторов
  ClsMasks;

type
  pLvlElem = ^TLvlElem;
  TLvlElem = record
    ID          : integer;
    Lvl         : integer;
    LvlName     : string;
    ParentName  : string;
    FullName    : string;
    Name        : string;
    Key         : string;
    Childs      : TList;
    reserved    : integer;
    //
    dsoPropsNames  : TStringList;
    dsoPropsValues : TStringList;
  end;

  TLvlSortList = class(TObject)
  private
    fElems         : TList;
    fSorted        : boolean;
    fElemsData     : pointer;
    fElemsDataSize : integer;
  protected
    procedure SortDimension;
    procedure SortBudgetCLS(const ClsName : string);
    function  InitElems(ElemsCount : integer) : boolean;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    function LoadFromDimension(Con : _Connection; CubeName : widestring;
      DimensionName : widestring; HierarchyName: widestring; LowLevel: widestring) : boolean;
    function LoadFromBudgetCLS(rs : recordset; const ClsName : string) : boolean;
    {$IFDEF DEBUG}
    procedure PrintElems(OutStrings : TStrings);
    {$ENDIF}
    property Elems : TList read fElems;
  end;

  // для манипуляций с бюджетными масками
  pMaskElem = ^TMaskElem;
  TMaskElem = record
    Order      : integer;
    DigitCount : integer;
    Divider    : integer;
    reserved   : integer;
  end;

  TMaskResolver = class
  private
    fElems : TList;
  public
    constructor Create;
    destructor  Destroy; override;
    procedure Clear;
    function InitBudgetClsMask(const ClsName : string) : boolean;
    function GetParentCLS(CLS : integer) : integer;
    function GetClsLvl(CLS : integer) : integer;
    function OrderCount : integer;
  end;

implementation

{TMaskResolver}
constructor TMaskResolver.Create;
begin
  inherited Create;
  fElems := TList.Create;
end;

destructor  TMaskResolver.Destroy;
begin
  Clear;
  FreeAndNil(fElems);
  inherited Destroy;
end;

procedure TMaskResolver.Clear;
var i : integer;
begin
  for i := 0 to fElems.Count - 1 do
    Dispose(pMaskElem(fElems[i]));
  fElems.Clear;
end;

function TMaskResolver.OrderCount : integer;
begin
  result := fElems.Count
end;

function GetBudgetCLSMask(const ClsName : string) : string;
var i : integer;
    tmpIdent : ^TIdentMapEntry;
begin
  result := '';
  tmpIdent := nil;
  for i := low(ClsTypes) to High(ClsTypes) do
    if AnsiCompareText(ClsName, ClsTypes[i].Name) = 0 then begin
      tmpIdent := @ClsTypes[i];
      break
    end;
  if Assigned(tmpIdent) then result := GetEditMask(tmpIdent^.Value)
end;

function TMaskResolver.InitBudgetClsMask(const ClsName : string) : boolean;
var i : integer;
    tmpStr : string;
    pElem : pMaskElem;
    tmpStrL : TStringList;
begin
  Clear;
  tmpStr := GetBudgetCLSMask(ClsName);
  result := tmpStr <> '';
  if result then begin
    tmpStrL := TStringList.Create;
    try
      tmpStr := StringReplace(tmpStr, '.', #13#10, [rfReplaceAll]);
      tmpStrL.Text := tmpStr;
      // создаём элементы и заполняем необходимые поля
      for i := 0 to tmpStrL.Count - 1 do begin
        new(pElem);
        pElem^.DigitCount := length(tmpStrL[i]);
        pElem^.Order      := i;
        if pElem^.DigitCount <> 0
          then pElem^.Divider := round(exp(ln(10) * pElem^.DigitCount))
          else pElem^.Divider := 10;
        fElems.Add(pElem)
      end;
      // расчитываем делители
      for i := fElems.Count - 1 downto 0 do begin
        pElem := fElems[i];
        if i <> fElems.Count - 1 then pElem^.Divider := pElem^.Divider * pMaskElem(fElems[i + 1])^.Divider
      end;
    finally
      FreeAndNil(tmpStrL);
    end;
    result := fElems.Count <> 0
  end
end;

function TMaskResolver.GetParentCLS(CLS : integer) : integer;
var i : integer;
    tmpElem : pMaskElem;
    diff : integer;
begin
  result := 0;
  if fElems.Count > 0 then
    for i := fElems.Count - 1 downto 0 do begin
      tmpElem := fElems[i];
      diff := CLS mod tmpElem^.Divider;
      if diff <> 0 then begin
        result := CLS - diff;
        break;
      end
    end;
end;

function TMaskResolver.GetClsLvl(CLS : integer) : integer;
var i : integer;
begin
  result := 0;
  if Cls <> 0 then
    for i := fElems.Count - 1 downto 0 do
      if CLS mod pMaskElem(fElems[i])^.Divider <> 0 then begin
        result := i + 1;
        break;
      end
end;

//
const SizeOfElem : integer = SizeOf(TLvlElem);

procedure InitFromPointer(p : pLvlElem);
begin
  p^.Childs := TList.Create;
  p^.dsoPropsNames := TStringList.Create;
  p^.dsoPropsValues := TStringList.Create;
end;

procedure DestroyLvlElem(Elem : pLvlElem);
var i : integer;
begin
  with Elem^ do begin
    for i := 0 to Childs.Count - 1 do
      DestroyLvlElem(Childs[i]);
    FreeAndNil(Childs);
    FreeAndNil(dsoPropsNames);
    FreeAndNil(dsoPropsValues);
    SetLength(LvlName, 0);
    SetLength(Name, 0);
    SetLength(ParentName, 0);
    SetLength(FullName, 0);
    SetLength(Key, 0);
  end
end;

{TLvlSortList}
constructor TLvlSortList.Create;
begin
  fElems         := TList.Create;
  fElemsData     := nil;
  fElemsDataSize := 0;
end;

destructor TLvlSortList.Destroy;
begin
  Clear;
  FreeAndNil(fElems);
  inherited;
end;

procedure TLvlSortList.Clear;
var i : integer;
begin
  if fElems.Count <> 0 then
    for i := 0 to fElems.Count - 1 do
      DestroyLvlElem(fElems[i]);
  if fElemsDataSize <> 0 then begin
    FreeMem(fElemsData, fElemsDataSize);
    fElemsData := nil;
    fElemsDataSize := 0
  end;
  fElems.Clear
end;

// сомнительная сортировка, сильно привязана к структуре данных
procedure TLvlSortList.SortDimension;
var CurParentInd  : integer;
    CurInd : integer;

  // рекурсивный обход дерева и проставление инексов в порядке следования элементов
  procedure SetIDs(List : TList; var CurID : integer);
  var i : integer;
  begin
    for i := 0 to List.Count - 1 do
      with pLvlElem(List[i])^ do begin
        ID := CurID;
        inc(CurID);
        if Childs.Count <> 0 then SetIDs(Childs, CurID)
      end
  end;

  function GetNearHiIndex : integer;
  var i : integer;
  begin
    result := -1;
    for i := CurInd downto 0 do
      if pLvlElem(fElems[i])^.Lvl < pLvlElem(fElems[CurInd])^.Lvl then begin
        result := i;
        break
      end;
  end;

begin
  if not fSorted then begin
    if fElems.Count > 0 then begin
      // ищем первый елемент предыдущего уровня
      CurInd := fElems.Count - 1;
      CurParentInd := GetNearHiIndex;
      // переносим дочерние элементы
      while (CurParentInd >= 0) and (CurInd > CurParentInd) and
        (pLvlElem(fElems[CurInd])^.Lvl > 0) do begin

        if AnsiCompareText(pLvlElem(fElems[CurInd])^.ParentName, pLvlElem(fElems[CurParentInd])^.FullName) = 0 then begin
          pLvlElem(fElems[CurParentInd])^.Childs.Insert(0, pLvlElem(fElems[CurInd]));
          fElems[CurInd] := nil;
          dec(CurInd);
          CurParentInd := GetNearHiIndex;
        end
        else
          dec(CurParentInd);

      end;
      // удаляем пустые элементы
      fElems.Pack;
      // проставляем индексы
      CurInd := 0;
      SetIds(fElems, CurInd)
    end;
    fSorted := true;
  end
end;


function SortElemsByID(Item1, Item2: Pointer): Integer;
begin
  if pLvlElem(Item1)^.ID < pLvlElem(Item2)^.ID then result := -1
    else if pLvlElem(Item1)^.ID > pLvlElem(Item2)^.ID then result := 1
      else result := 0
end;

procedure TLvlSortList.SortBudgetCLS(const ClsName : string);
var tmpResolver : TMaskResolver;
    i : integer;
    ParentID : integer;
    tmpElem : pLvlElem;
    CurParent : pLvlElem;

    function FindParent(ChildIndex : integer; List : TList; ParentID : integer) : pLvlElem;
    var i : integer;
    begin
      result := nil;
      for i := ChildIndex downto 0 do
        if pLvlElem(List[i])^.ID = ParentID then begin
          result := List[i];
          break
        end
    end;

begin
  if not fSorted then begin
    // на всякий случай сортируем по ИД
    fElems.Sort(SortElemsByID);
    //
    tmpResolver := TMaskResolver.Create;
    try
      tmpResolver.InitBudgetClsMask(ClsName);
      CurParent := nil;
      for i := fElems.Count - 1 downto 0 do begin
        tmpElem := fElems[i];
        ParentID := tmpResolver.GetParentCLS(tmpElem^.ID);
        tmpElem^.Lvl := tmpResolver.GetClsLvl(tmpElem^.ID);
        // если родитель не найден - пытаемся найти
        if not Assigned(CurParent)
          then CurParent := FindParent(i, fElems, ParentID)
          // если какой-то родительский был найден раньше - смотрим подходит-ли он
          else if CurParent^.ID <> ParentID
                 then CurParent := FindParent(i, fElems, ParentID);
        // если родитель есть - добавляем элемент ему в список
        if Assigned(CurParent) and
          // нулевой ИД оставляем (только он удовлетворяет этому условию)
          (fElems[i] <> CurParent) then begin
          CurParent^.Childs.Insert(0, fElems[i]);
          fElems[i] := nil;
        end
      end;
      // удаляем пустые (перенесенные) элементы
      fElems.Pack
    finally
      FreeAndNil(tmpResolver);
    end;
    fSorted := true;
  end
end;

{$IFDEF DEBUG}
function TabByLvl(Lvl : integer) : string;
begin
  if Lvl > 0 then begin
    SetLength(result, Lvl);
    FillChar(result[1], Lvl, #9);
  end
  else result := ' '
end;

procedure OutElems(OutStr : TStrings; ElemsList : TList);
var i : integer;
begin
  for i := 0 to ElemsList.Count - 1 do
    with pLvlElem(ElemsList[i])^ do begin
      OutStr.Add(format('%d %d%s%s', [ID, Lvl, TabByLvl(Lvl), Name]));
      if Childs.Count <> 0 then OutElems(OutStr, Childs)
    end
end;

procedure TLvlSortList.PrintElems(OutStrings : TStrings);
begin
  if Assigned(OutStrings) then OutElems(OutStrings, fElems)
end;
{$ENDIF}

function TLvlSortList.InitElems(ElemsCount : integer) : boolean;
var CurPtr : pointer;
    i      : integer;
begin
  result := true;
  try
    // выделяем память под элементы
    fElemsDataSize := SizeOfElem * ElemsCount;
    GetMem(fElemsData, fElemsDataSize);
    ZeroMemory(fElemsData, fElemsDataSize);
    // инициализируем элементы и записываем указатели на них в список
    fElems.Capacity := ElemsCount;
    CurPtr := fElemsData;
    for i := 0 to ElemsCount - 1 do begin
      fElems.Add(CurPtr);
      InitFromPointer(CurPtr);
      CurPtr := pointer(integer(CurPtr) + SizeOfElem);
    end;
  except
    FreeMem(fElemsData, fElemsDataSize);
    result := false
  end
end;

function TLvlSortList.LoadFromDimension(Con : _Connection; CubeName : widestring;
  DimensionName : widestring; HierarchyName: widestring; LowLevel: widestring) : boolean;
var j, k, l : integer;
    cat : ICatalog;
    cd  : CubeDef;
    dm  : dimension;
    hr  : Hierarchy;
    lv  : level;
    mb  : member;
    CountinueDetailing: boolean;

    MembersCount : integer;
//    CurPtr : pointer;
    CurElem : integer;
    LvlsList : TList;
    prp : property_;

    {Имя мембера}
    function GetMemberName(memb: member): string;
    begin
      result := string(StringReplace(memb.Caption, #$D#$A, ' ', [rfReplaceAll]))
    end;

    {Элемент является датамембером}
    function IsDataMember(member: member): boolean;
    var
      UName: string;
    begin
      result := false;
      if Assigned(member) then
      begin
        UName := member.UniqueName;
        result := Copy(UName, Length(UName) - 10, 11) = '.DATAMEMBER';
      end;
    end;

    {Полное имя родителя}
    function GetMemberParentUName(memb: member): string;
    begin
      result := '';
      if IsDataMember(memb) then
        result := Copy(mb.UniqueName, 1,
          length(mb.UniqueName) - length('.DATAMEMBER'))
      else
        if Assigned(memb.Parent) then
          result := memb.Parent.UniqueName;
    end;

    function GetParentName(memb : member) : string;
    var tmpMemb : member;
    begin
      result := '';
      if Assigned(memb.Parent) then begin
        tmpMemb := memb;
        while Assigned(tmpMemb.Parent) do begin
          result := GetMemberName(tmpMemb.Parent) + result;
          tmpMemb := tmpMemb.Parent
        end;
      end
    end;

begin
  result := false;
  CountinueDetailing := true;
  try
    Clear;
    LvlsList := TList.Create;
    if Assigned(Con) and (CubeName <> '') and (DimensionName <> '') {and (LowLevel <> '')} then begin
      cat := CreateComObject(CLASS_CATALOG) as ICatalog;
      cat.Set_ActiveConnection(con);
      if cat.CubeDefs.Count > 0 then begin
        try
          cd := Cat.CubeDefs.Item[CubeName];
        except
          // если такого куба нет
        end;
        if Assigned(cd) then begin
          try
            dm := cd.Dimensions.Item[DimensionName];
          except
            // если такого измерения нет
          end;
          if Assigned(dm) then begin
            try
              if HierarchyName <> '' then
                hr := dm.Hierarchies[HierarchyName]
              else
                hr := dm.Hierarchies[0];
            except
              //если нет иерархии
            end;
            if Assigned(hr) then begin
            // кол-во елементов
            MembersCount := 0;
            for j := 0 to hr.Levels.Count - 1 do begin
              lv := hr.Levels[j];
              inc(MembersCount, lv.Members.Count);

              if (lv.Name = LowLevel) then
                break;
            end;
            if MembersCount > 0 then begin

              InitElems(MembersCount);

              CurElem := 0;
              for j := 0 to hr.Levels.Count - 1 do begin
                lv := hr.Levels[j];


                {вычислям, стоит ли детализировать дальше}
                if CountinueDetailing then
                  CountinueDetailing := (lv.Name <> LowLevel)
                else
                  break;



                for k := 0 to lv.Members.Count - 1 do
                  with pLvlElem(fElems[CurElem])^ do begin
                    mb       := lv.Members[k];
                    Name     := GetMemberName(mb);
                    LvlName  := string(lv.Name);

                    {!obsolete}
                    (*
                    ParentName := GetParentName(mb);
                    FullName := ParentName + Name;
                    *)
                    {!new}
                    FullName := mb.UniqueName;
                    ParentName := GetMemberParentUName(mb);


                    // инициализация KEY (возможно устарел)
                    try
                      prp := mb.Properties['MEMBER_KEY'];
                    except
                      prp := nil
                    end;
                    if Assigned(prp)
                      then Key := VarToStr(prp.Value)
                      else Key := '';
                    // копирование всех остальных свойств

                    try
                      if Assigned(mb.Properties) then begin
                        for l := 0 to mb.Properties.Count - 1 do begin
                          dsoPropsNames.Add(AnsiUpperCase(string(mb.Properties[l].Name)));
                          try
                            dsoPropsValues.Add(VarToStr((mb.Properties[l].Value)));
                          except
                            dsoPropsValues.Add('');
                          end
                        end
                      end;
                    except
                    end;

                    Lvl     := j;
                    inc(CurElem);
                  end;

                end
              end;

              SortDimension;
              result := true
            end //if MembersCount > 0 then begin
        end // if Assigned(dm) then begin
      end // if Assigned(cd) then begin
    end; // if cat.CubeDefs.Count > 0 then begin
    Cat.Set_ActiveConnection(nil);
  end // if Assigned (...) ...
  finally
    prp := nil;
    mb  := nil;
    lv  := nil;
    hr  := nil;
    dm  := nil;
    cd  := nil;
    cat := nil;
    FreeAndNil(LvlsList);
  end
end;

function TLvlSortList.LoadFromBudgetCLS(rs : recordset; const ClsName : string) : boolean;
var i : integer;
    FieldID, FieldNAME : field;
begin
  result := false;
  try
    Clear;
    // проверяем правильность входного рекорсета
    if (rs.State = adStateOpen) and (rs.RecordCount > 0) then begin
      FieldID   := rs.Fields.Item['ID'];
      FieldNAME := rs.Fields.Item['NAME'];
      // проверяем наличие необходимых полей
      if Assigned(FieldID) and Assigned(FieldNAME) and
        // и выделяем память под элементы
        InitElems(rs.RecordCount) then begin

        // заполняем необходимые поля элементов
        i := 0;
        rs.MoveFirst;
        while not rs.EOF do begin
          pLvlElem(fElems[i])^.ID   := FieldID.Value;
          pLvlElem(fElems[i])^.Key  := FieldID.Value;
          pLvlElem(fElems[i])^.Name := FieldName.Value;
          pLvlElem(fElems[i])^.LvlName := 'BudgetCLS';
          rs.MoveNext;
          inc(i)
        end;

        //сортируем (строим иерархию)
        SortBudgetCLS(ClsName);

        result := true;
      end;
    end;
  finally

  end
end;

end.
