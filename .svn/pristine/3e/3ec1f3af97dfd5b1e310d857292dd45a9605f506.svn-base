unit uFormulasUtils;

interface
uses
  SysUtils, ExcelXP, MSXML2_TLB, Classes, uXMLCatalog, uFMExcelAddInConst,
  uXMLUtils, uFMAddinGeneralUtils, uFMAddinXMLUtils, uFMAddinExcelUtils,
  uSheetObjectModel, uExcelUtils, uGlobalPlaningConst, uOfficeUtils;

  {получить границы показателей}
  procedure GetOffsets(ExcelSheet: ExcelWorkSheet; out FirstRow, LastRow, FirstColumn,
    LastColumn, RowsLeaf, ColumnsLeaf: integer);
  function AddAxisFreeData(AxisCollection: TSheetAxisCollectionInterface;
    Row, Column: integer; var Element: IXMLDOMNode): boolean;
  function GetAxisFreeData(AxisCollection: TSheetAxisCollectionInterface;
    Row, Column: integer; var AxisData: IXMLDOMNode): boolean;
  {кодирование типовой формулы}
 (* procedure AddEncodedTypeFormula(SheetInterface: TSheetInterface; Formula: string;
    var FormulaElement: IXMLDOMNode; CurRow, CurColumn: integer);*)
  {раскодировать TypeFormula}
  function DecodeTypeFormula(SheetInterface: TSheetInterface; TypeFormula: TTypeFormula;
    FirstRow, CurRow, CurSectionIndex: integer): string;
  {кодирование формулы}
 (* procedure AddEncodedFormula(SheetInterface: TSheetInterface; Formula: string;
    var FormulaElement: IXMLDOMNode; CurRow: integer);*)
  {по указаным координатам получаем типовую формулу}
(*  procedure GetTypeFormula(Total: TSheetTotalInterface; Row,
    Column: integer; var FormulaNode: IXMLDOMNode); overload;
  {по указаным координатам получаем типовую формулу}
  function GetTypeFormula(Total: TSheetTotalInterface; Row,
    Column: integer): TTypeFormula; overload;*)
  {если исключение из типовой формулы, вернет true}
(*  function IsTypeFormulaException(Total: TSheetTotalInterface; Row,
    Column: integer): boolean; overload;
  {если исключение из типовой формулы, вернет true}
  function IsTypeFormulaException(Total: TSheetTotalInterface; Row,
    Column, Section: integer): boolean; overload;*)
  {мапит типовую формулу конкретному показателю}
(*  procedure MapTypeFormula(Total: TSheetTotalInterface);*)
  {размещает TypeFormula на лист}
  (*procedure MapTypeFormulas(SheetInterface: TSheetInterface);*)
  {у показателя очищает во всех столбцах значения типовой формулы}
 // procedure ClearTypeFormulaValue(Total: TSheetTotalInterface);
  {очистить значения типовых формул ссылающихся на указанный алиас показателя}
  (*procedure ClearLinkedTypeFormulasValue(Model: TSheetInterface; TotalAlias: string);*)
  {разместить значения типовых формул ссылающихся на указанный алиас показателя}
  //procedure MapLinkedTypeFormulaValue(Model: TSheetInterface; TotalAlias: string);
  {Если на указанный показатель имеются ссылки из типовых формул других показателей,
  вернет предупреждение с перечислением ссылающихся показателей}
  //function GetWarningText(Total: TSheetBasicTotal): string;
  //function ReplaceCellRef(Template, WhatPattern, WithPattern: string): string;
  (*procedure ParseCellRef(ExcelSheet: ExcelWorkSheet; CellRef: string;
    out ReplacedCellRef, Column, Row: string;
    out ColumnNumber, RowNumber: integer; out IsOtherSheet, IsAbsolute: boolean);*)

implementation

{ Обязательна к применению вместо StringReplace в процедурах кодировки формул,
  иначе замена адреса Cxxxx испортит все адреса CCxxxx}
function ReplaceCellRef(Template, WhatPattern, WithPattern: string): string;
var
  Index, Len: integer;
begin
  result := '';
  Len := Length(WhatPattern);
  if Template[1] = '=' then
  begin
    result := '=';
    Delete(Template, 1, 1);
  end;
  Index := Pos(WhatPattern, Template);
  while Index > 0 do
  begin
    if Template[Index - 1] in ['A'..'Z'] then
      result := result + Copy(Template, 1, Index + Len - 1)
    else
      result := result + Copy(Template, 1, Index - 1) + WithPattern;
    Delete(Template, 1, Index + Len - 1);
    Index := Pos(WhatPattern, Template);
  end;
  result := result + Template;
end;

procedure GetOffsets(ExcelSheet: ExcelWorkSheet; out FirstRow, LastRow, FirstColumn,
  LastColumn, RowsLeaf, ColumnsLeaf: integer);
var
  Range: ExcelRange;
begin
  FirstRow := 0;
  LastRow := 0;
  FirstColumn := 0;
  LastColumn := 0;
  RowsLeaf := 0;
  Columnsleaf := 0;
  if not Assigned(ExcelSheet) then
    exit;
  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntTotals);
  if Assigned(Range) then
  begin
    FirstRow := Range.Row;
    LastRow := Range.Row + Range.Rows.Count - 1;
    FirstColumn := Range.Column;
    LastColumn := Range.Column + Range.Columns.Count - 1;
  end;
  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntRows);
  if Assigned(Range) then
    RowsLeaf := Range.Column + Range.Columns.Count - 1;
  Range := GetRangeByName(ExcelSheet, snNamePrefix + snSeparator + sntColumns);
  if Assigned(Range) then
    ColumnsLeaf := Range.Row + Range.Rows.Count - 1;
end;

function AddAxisFreeData(AxisCollection: TSheetAxisCollectionInterface;
  Row, Column: integer; var Element: IXMLDOMNode): boolean;
var
  RangeName, UniqueName: string;
  Params: TStringList;
  i, j, CurAxisElementIndex, CurLevelIndex: integer;
  IsLeaf, IsleafMember: boolean;
begin
  result := true;
  if (AxisCollection.Count = 0) then
  begin
    if AxisCollection.AxisType = axRow then
      SetAttr(Element, mnIsRowLeaf, 'true')
    else
      SetAttr(Element, mnIsColumnLeaf, 'true');
    exit;
  end;
  if (Row = 0) or (Column = 0) then
    exit;
  RangeName := GetNameByRC(AxisCollection.Owner.ExcelSheet, Row, Column);
  if (RangeName = '') then
  begin
    SetAttr(Element, mnIsColumnLeaf, true);
    exit;
  end;
  // общий итог
  if (RangeName = snNamePrefix + snSeparator + gsRow) or
     (RangeName = snNamePrefix + snSeparator + gsColumn) then
  begin
    for j := 0 to AxisCollection.Count - 1 do
      SetAttr(Element, AxisCollection[j].Alias, AxisCollection[j].AllMember);
    if (AxisCollection.AxisType = axRow) then
      SetAttr(Element, mnIsRowLeaf, false)
    else
      SetAttr(Element, mnIsColumnLeaf, false);
    exit;
  end;
  if not ParseExcelName(RangeName, Params) then
  begin
    result := false;
    exit;
  end;
  try
    CurAxisElementIndex := 0;
    CurLevelIndex := -1;
    IsLeaf := (Params.IndexOf(fpEnd) = -1);

    // локал ай ди начинается с 4 параметра
    for i := 3 to Params.Count - 1 do
    begin
      // из за гребучего контекстного меню надо проверять
      if CurAxisElementIndex > AxisCollection.Count - 1 then
        exit;
      // пропускаем фиктивный параметр
      if (Params.Strings[i] = fpLeafEnd) or (Params.Strings[i] = fpDummy) then
        continue;
      inc(CurLevelIndex);
      // если итог, заполняем остальные алиасы элементов осей общими (All) мемберами
      if (Params.Strings[i] = fpEnd) then
      begin
        if (CurLevelIndex > 0) then
          inc(CurAxisElementIndex);
        for j := CurAxisElementIndex to AxisCollection.Count - 1 do
          SetAttr(Element, AxisCollection[j].Alias, AxisCollection[j].AllMember);
        break;
      end;
      UniqueName := GetMemberAttrByKey(AxisCollection[CurAxisElementIndex].Members,
        attrLocalId, Params.Strings[i], attrUniqueName, IsleafMember);
      // если UniqueName неопределено - выходим
      result := (UniqueName <> '');
      if not result then
        exit;
      SetAttr(Element, AxisCollection[CurAxisElementIndex].Alias, UniqueName);
      // если игнорирует иерархию или следующий - пустышка - то переходим на следующий элемент строки
      if (AxisCollection[CurAxisElementIndex].IgnoreHierarchy and not AxisCollection.Broken) or
         ((i <> Params.Count - 1) and (Params.Strings[i + 1] = fpDummy)) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
        continue;
      end;
      // переход на следующий элемент строк
      if (CurLevelIndex = AxisCollection[CurAxisElementIndex].Levels.Count - 1) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
      end;
    end;
    if (AxisCollection.AxisType = axRow) then
      SetAttr(Element, mnIsRowLeaf, IsLeaf)
    else
      SetAttr(Element, mnIsColumnLeaf, IsLeaf);
  finally
    FreeStringList(Params);
  end;
end;

function GetAxisFreeData(AxisCollection: TSheetAxisCollectionInterface;
  Row, Column: integer; var AxisData: IXMLDOMNode): boolean;
var
  RangeName, UniqueName: string;
  Params: TStringList;
  i, j, CurAxisElementIndex, CurLevelIndex: integer;
  IsLeaf, IsleafMember: boolean;
begin
  result := true;
  if AxisCollection.Empty or (Row = 0) or (Column = 0) then
    exit;

  RangeName := GetNameByRC(AxisCollection.Owner.ExcelSheet, Row, Column);
  if (RangeName = '') then
  begin
    SetAttr(AxisData, mnIsColumnLeaf, 'true');
    exit;
  end;

  // общий итог
  if (RangeName = snNamePrefix + snSeparator + gsRow) or
     (RangeName = snNamePrefix + snSeparator + gsColumn) then
  begin
    for j := 0 to AxisCollection.Count - 1 do
      SetAttr(AxisData, AxisCollection[j].Alias, AxisCollection[j].AllMember);
    SetAttr(AxisData, IIF(AxisCollection.AxisType = axRow, mnIsRowLeaf, mnIsColumnLeaf), 'false');
    exit;
  end;

  if not ParseExcelName(RangeName, Params) then
  begin
    result := false;
    exit;
  end;

  try
    CurAxisElementIndex := 0;
    CurLevelIndex := -1;
    IsLeaf := (Params.IndexOf(fpEnd) = -1);

    // локал ай ди начинается с 4 параметра
    for i := 3 to Params.Count - 1 do
    begin
      // из за гребучего контекстного меню надо проверять
      if CurAxisElementIndex > AxisCollection.Count - 1 then
        exit;
      // пропускаем фиктивный параметр
      if (Params.Strings[i] = fpLeafEnd) or (Params.Strings[i] = fpDummy) then
        continue;
      inc(CurLevelIndex);
      // если итог, заполняем остальные алиасы элементов осей общими (All) мемберами
      if (Params.Strings[i] = fpEnd) then
      begin
        if (CurLevelIndex > 0) then
          inc(CurAxisElementIndex);
        for j := CurAxisElementIndex to AxisCollection.Count - 1 do
          SetAttr(AxisData, AxisCollection[j].Alias, AxisCollection[j].AllMember);
        break;
      end;
      UniqueName := GetMemberAttrByKey(AxisCollection[CurAxisElementIndex].Members,
        attrLocalId, Params.Strings[i], attrUniqueName, IsleafMember);
      // если UniqueName неопределено - выходим
      result := (UniqueName <> '');
      if not result then
        exit;
      SetAttr(AxisData, AxisCollection[CurAxisElementIndex].Alias, UniqueName);
      // если игнорирует иерархию или следующий - пустышка - то переходим на следующий элемент строки
      if (AxisCollection[CurAxisElementIndex].IgnoreHierarchy and not AxisCollection.Broken) or
         ((i <> Params.Count - 1) and (Params.Strings[i + 1] = fpDummy)) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
        continue;
      end;
      // переход на следующий элемент строк
      if (CurLevelIndex = AxisCollection[CurAxisElementIndex].Levels.Count - 1) then
      begin
        inc(CurAxisElementIndex);
        CurLevelIndex := -1;
      end;
    end;
    SetAttr(AxisData,IIF(AxisCollection.AxisType = axRow,
      mnIsRowLeaf, mnIsColumnLeaf), BoolToStr(IsLeaf));
  finally
    FreeStringList(Params);
  end;
end;

(*procedure ParseCellRef(ExcelSheet: ExcelWorkSheet; CellRef: string;
  out ReplacedCellRef, Column, Row: string;
  out ColumnNumber, RowNumber: integer;
  out IsOtherSheet, IsAbsolute: boolean);
begin
  ReplacedCellRef := '';
  IsOtherSheet := (CellRef[1] = '!');
  if IsOtherSheet then
    Delete(CellRef, 1, 1);
  ReplacedCellRef := StringReplace(CellRef, '|', '', [rfReplaceAll]);
  IsAbsolute := false;
  while Pos('$', CellRef) > 0 do
  begin
    IsAbsolute := true;
    Delete(CellRef, Pos('$', CellRef), 1);
  end;
  Column := copy(CellRef, 1, Pos('|', CellRef) - 1);
  ColumnNumber := GetColumnIndex(Column);
  if (not IsOtherSheet) and
    (ColumnNumber = GetGrandSummaryColumn(ExcelSheet)) then
    Column := gsColumn;
  Row := copy(CellRef, Pos('|', CellRef) + 1, Length(CellRef));
  RowNumber := StrToInt(Row);
end;*)

(*procedure AddEncodedTypeFormula(SheetInterface: TSheetInterface; Formula: string;
  var FormulaElement: IXMLDOMNode; CurRow, CurColumn: integer);

  procedure SetParamAttributes(Node: IXMLDOMNode; ParamType: TFormulaParamType;
    ParamValue, Offset: string; IsOtherSheet: boolean);
  begin
    SetAttr(Node, attrParamType, Ord(ParamType));
    SetAttr(Node, attrParamValue, ParamValue);
    if Offset <> '' then
      SetAttr(Node, attrOffset, Offset);
    SetAttr(Node, attrIsOtherSheet, BoolToStr(IsOtherSheet));
  end;

  function CheckMPReference(ExcelSheet: ExcelWorkSheet; AxisType: TAxisType;
    RowNumber, ColumnNumber: integer; out MPName: string): boolean;
  var
    MPRange: ExcelRange;
  begin
    result := false;
    MPRange := GetRangeByName(ExcelSheet,
      BuildExcelName(IIF(AxisType = axRow, sntRowsMPArea, sntColumnsMPArea)));
    if Assigned(MPRange) then
      if IsPointInRange(MPRange, RowNumber, ColumnNumber) then
      begin
        if AxisType = axRow then
          MPRange := GetRange(ExcelSheet, MPRange.Row, ColumnNumber,
            MPRange.Row + MPRange.Rows.Count - 1, ColumnNumber)
        else
          MPRange := GetRange(ExcelSheet, RowNumber, MPRange.Column,
            RowNumber, MPRange.Column + MPRange.Columns.Count - 1);
        MPName := GetNameByRange(ExcelSheet, MPRange);
        result := MPName <> '';
      end;
  end;

  {}
  function ReplaceRangeNames(var Template: string): boolean;
  var
    i, FirstIndex, LastIndex: integer;
    CellName, RefName, CellRef: string;
  begin
    result := false;
    {пользовательское имя диапазона отдельного показателя имеет вид
      Криста_отдельный_тип_номер, см. TSheetSingleCell.GetUserExcelName}
    repeat
      FirstIndex := Pos('Криста_Отдельн', Template);
      if FirstIndex = 0 then
        exit;
      LastIndex := FirstIndex + 15;
      repeat
        inc(LastIndex);
      until (Template[LastIndex] in ['0'..'9']);
      repeat
        inc(LastIndex);
        if LastIndex > Length(Template) then
        begin
          break;
        end;
      until not (Template[LastIndex] in ['0'..'9']);
          dec(LastIndex);
      RefName := Copy(Template, FirstIndex, LastIndex - FirstIndex + 1);

      for i := 0 to SheetInterface.SingleCells.Count - 1 do
      begin
        CellName := SheetInterface.SingleCells[i].GetUserExcelName;
        if CellName = RefName then
        begin
          CellRef := SheetInterface.SingleCells[i].Address;
          Delete(Template, FirstIndex, LastIndex - FirstIndex + 1);
          Insert(CellRef, Template, FirstIndex);
          result := true;
          break;
        end;
      end;
    until false;
  end;

var
  ParamElement: IXMLDOMNode;
  FormulaCellRefs: TStringList;
  ReplacedCellRef, Column, Row: string;
  i, RowNumber, ColumnNumber, SectionIndex, GrandSummaryRow: integer;
  Offset, ParamName, Template, SingleCellID, TotalAlias: string;
  Total: TSheetBasicTotal;
  IsOtherSheet, IsAbsolute: boolean;
  FirstRow, LastRow, FirstColumn, LastColumn, RowsLeaf, ColumnsLeaf: integer;
  ExcelSheet: ExcelWorksheet;
  MPName: string;
begin
  ExcelSheet := SheetInterface.ExcelSheet;
  GetOffsets(ExcelSheet, FirstRow, LastRow, FirstColumn,
    LastColumn, RowsLeaf, ColumnsLeaf);
  Template := Formula;
  if ReplaceRangeNames(Template) then
  begin
    Formula := Template;
    SetCellFormula(ExcelSheet, CurRow, CurColumn, Formula);
  end;
  FormulaCellRefs := GetFormulaCellRefs(Formula);
  GrandSummaryRow := GetGrandSummaryRow(ExcelSheet);

  {Все ссылки в формуле пытаемся перевести в параметры.
    Допустимы ссылки на ячейки табличных и отдельных показателей.
    В 2.2.7 добавляем ссылки на мембер пропертиз.}
  for i := 0 to FormulaCellRefs.Count - 1 do
  begin
    ParseCellRef(ExcelSheet, FormulaCellRefs[i], ReplacedCellRef,
      Column, Row, ColumnNumber, RowNumber, IsOtherSheet, IsAbsolute);
    ParamName := IIF((i < 10), 'param0', 'param') + IntToStr(i);

    ParamElement := CreateAndAddChild(FormulaElement, ParamName);

    {Обработка ссылки на отдельный показатель}
    if SheetInterface.WritablesInfo.IsSingleCellSelected(ExcelSheet,
      RowNumber, ColumnNumber, SingleCellID) then
    begin
      SetParamAttributes(ParamElement, fptSingleCell, 'S_' + SingleCellID, '', IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;

    Offset := Row;
    if not IsOtherSheet then
      if StrToInt(Row) = GrandSummaryRow then
        Offset := gsRow
      else
        {В случае абсолютной ссылки на ячейку показателя смещение берем
          относительно первой ячейки, а не текущей. Чтобы ссылка действительно
          вела себя как абсолютная и не смещалась по столбцам надо указывать
          секцию показателя. Будем запоминать ее здесь же, в Offset.}
        if IsAbsolute then
          Offset := IntToStr(StrToInt(Row) - FirstRow)
        else
          Offset := IntToStr(StrToInt(Row) - CurRow);

    {обработка ссылки на МР}
    if CheckMPReference(ExcelSheet, axRow, RowNumber, ColumnNumber, MPName) then
    begin
      SetParamAttributes(ParamElement, fptRowMP,
        MPName, Offset, IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;
    if CheckMPReference(ExcelSheet, axColumn, RowNumber, ColumnNumber, MPName) then
    begin
      Offset := IntToStr(ColumnNumber - CurColumn);
      SetParamAttributes(ParamElement, fptColumnMP,
        MPName, Offset, IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
      continue;
    end;

    {Ссылка на ячейку вне таблицы}
    if (StrToInt(Row) < FirstRow) or (StrToInt(Row) > LastRow) or
      (ColumnNumber < FirstColumn) or (ColumnNumber > LastColumn) then
    begin
      SetParamAttributes(ParamElement, fptFreeCell, ReplacedCellRef, '', IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
    end
    else
    begin
      {Обработка ссылки на табличный показатель}
      TotalAlias := '';
      Total := SheetInterface.Totals.FindByColumn(ColumnNumber, SectionIndex);
      { если показатель удалили - заменяем его на #ССЫЛКА! = fmIncorrectRef}
      if not Assigned(Total) then
      begin
        Template := ReplaceCellRef(Template, ReplacedCellRef, fmIncorrectRef);
        continue;
      end;
      if IsAbsolute then
      begin
        Offset := Offset + '_' + IntToStr(SectionIndex);
        SetParamAttributes(ParamElement, fptTotalAbsolute, Total.Alias, Offset, IsOtherSheet);
      end
      else
        SetParamAttributes(ParamElement, fptTotal, Total.Alias, Offset, IsOtherSheet);
      Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
    end;

  end;
  SetAttr(FormulaElement, attrTemplate, Template);
end; *)

{раскодировать TypeFormula}
function DecodeTypeFormula(SheetInterface: TSheetInterface; TypeFormula: TTypeFormula;
  FirstRow, CurRow, CurSectionIndex: integer): string;

  function NotAssignedTotal(Total: TSheetBasicTotal; ParamName: string;
    var Template: string): boolean;
  begin
    result := not Assigned(Total);
    if result then
      Template := StringReplace(Template, ParamName, fmIncorrectRef, [rfReplaceAll]);
  end;

  function NotAssignedTargetRange(Range: ExcelRange; ParamName: string;
    var Template: string): boolean;
  begin
    result := not Assigned(Range);
    if result then
      Template := StringReplace(Template, ParamName, fmIncorrectRef, [rfReplaceAll]);
  end;

var
  Template, ParamName, DecodedParam: string;
  FormulaParam: TFormulaParam;
  i, SectionIndex: integer;
  Offset, ColumnName, RowName, tmpStr: string;
  SummaryRange: ExcelRange;
  Total: TSheetBasicTotal;
  TargetRange: ExcelRange;
begin
  result := '';
  if not(Assigned(SheetInterface) and Assigned(TypeFormula)) then
    exit;
  Template := TypeFormula.Template;
  // проходим по всем закодированным параметрам формулы
  for i := 0 to TypeFormula.FormulaParams.Count - 1 do
  begin
    FormulaParam := TypeFormula.FormulaParams[i];
    ParamName := FormulaParam.Name;
    Offset := FormulaParam.OffSet;
    if (Offset <> '') and (OffSet[1] = '!') then
      Delete(Offset, 1, 1);
    (*Bucks := '';
    if Pos('p', ParamName) > 1 then
      Bucks := Copy(ParamName, 1, Pos('p', ParamName) - 1);*)

    if FormulaParam.ParamType = fptFreeCell then
      DecodedParam := FormulaParam.ParamValue
    else
    begin
      case FormulaParam.ParamType of
        fptTotal, fptTotalAbsolute:
          begin
            Total := SheetInterface.Totals.FindByAlias(FormulaParam.ParamValue);
            if NotAssignedTotal(Total, ParamName, Template) then
              continue;

            {В случае абсолютной ссылки Offset содержит еще и номер секции}
            if FormulaParam.ParamType = fptTotalAbsolute then
            begin
              tmpStr := Offset;
              Offset := CutPart(tmpStr, '_');
              SectionIndex := StrToInt(tmpStr);
            end
            else
              SectionIndex := IIF((Total as TSheetTotalInterface).IsIgnoredColumnAxis, 0, CurSectionIndex);

            TargetRange := (Total as TSheetTotalInterface).GetTotalRangeWithoutGrandSummary(SectionIndex);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;

            if FormulaParam.IsOtherSheet  then
              RowName := IntToStr(StrToInt(Offset) + (CurRow - FirstRow))
            else
              if Offset = gsRow then
              begin
                SummaryRange := GetGrandSummaryCellRange(SheetInterface.ExcelSheet, axRow);
                if NotAssignedTargetRange(SummaryRange, ParamName, Template) then
                  continue;
                RowName := IntToStr(SummaryRange.Row)
              end
              else
                if FormulaParam.ParamType = fptTotal then
                  RowName := IntToStr(StrToInt(Offset) + CurRow)
                else
                  RowName := '$' + IntToStr(StrToInt(Offset) + FirstRow);
          end;
        fptSingleCell:
          begin
            Total := SheetInterface.SingleCells.FindByAlias(FormulaParam.ParamValue);
            if NotAssignedTotal(Total, ParamName, Template) then
              continue;
            TargetRange := (Total as TSheetSingleCellInterface).GetExcelRange;
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            RowName := IntToStr(TargetRange.Row)
          end;
        fptRowMP:
          begin
            TargetRange := GetRangeByName(SheetInterface.ExcelSheet, FormulaParam.ParamValue);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            if Offset = gsRow then
            begin
              SummaryRange := GetGrandSummaryCellRange(SheetInterface.ExcelSheet, axRow);
              if NotAssignedTargetRange(SummaryRange, ParamName, Template) then
                continue;
              RowName := IntToStr(SummaryRange.Row)
            end
            else
              RowName := IntToStr(StrToInt(Offset) + CurRow);
          end;
        fptColumnMP:
          begin
            TargetRange := GetRangeByName(SheetInterface.ExcelSheet, FormulaParam.ParamValue);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            RowName := IntToStr(TargetRange.Row);
            TargetRange := (TypeFormula.Owner as TSheetTotalInterface).GetTotalRange(CurSectionIndex);
            if NotAssignedTargetRange(TargetRange, ParamName, Template) then
              continue;
            ColumnName := GetColumnName(StrToInt(Offset) + TargetRange.Column);
          end;
      end;

      if FormulaParam.ParamType <> fptColumnMP then
      begin
        ColumnName := GetColumnName(TargetRange.Column);
        if FormulaParam.ParamType = fptTotalAbsolute then
          ColumnName := '$' + ColumnName;
      end;

      DecodedParam := ColumnName + RowName;
    end;
    // заменяем параметр на раскодированный
    Template := StringReplace(Template, ParamName, DecodedParam, [rfReplaceAll]);
  end;
  result := Template;
end;

(*procedure AddEncodedFormula(SheetInterface: TSheetInterface; Formula: string;
  var FormulaElement: IXMLDOMNode; CurRow: integer);
var
  ParamElement: IXMLDOMNode;
  FormulaCellRefs: TStringList;
  CellRef, ReplacedCellRef, Column, Row: string;
  i, ColumnNumber, tempInt: integer;
  Total: TSheetTotalInterface;
  Offset, ParamName, Template: string;
  FirstRow, LastRow, FirstColumn, LastColumn, RowsLeaf, ColumnsLeaf: integer;
  ExcelSheet: ExcelWorksheet;
begin
  ExcelSheet := SheetInterface.ExcelSheet;
  GetOffsets(ExcelSheet, FirstRow, LastRow, FirstColumn,
    LastColumn, RowsLeaf, ColumnsLeaf);
  Template := Formula;
  FormulaCellRefs := GetFormulaCellRefs(Formula);
  for i := 0 to FormulaCellRefs.Count - 1 do
  begin
    ReplacedCellRef := '';
    if (i < 10) then
      ParamName := 'param0' + IntToStr(i)
    else
      ParamName := 'param' + IntToStr(i);
    CellRef := FormulaCellRefs.Strings[i];
    if CellRef[1] = '!' then
      continue;
    ReplacedCellRef := StringReplace(CellRef, '|', '', [rfReplaceAll]);
    Column := copy(CellRef, 1, Pos('|', CellRef) - 1);
    ColumnNumber := GetColumnIndex(Column);
    ParamName := ParamName;
    Row := copy(CellRef, Pos('|', CellRef) + 1, Length(CellRef));

    Offset := Row;
    if StrToInt(Row) = GetGrandSummaryRow(ExcelSheet) then
      Offset := gsRow
    else
      Offset := IntToStr(StrToInt(Row) - CurRow);

    if (ColumnNumber < FirstColumn) or (ColumnNumber > LastColumn) or
       (StrToInt(Row) < FirstRow) or (StrToInt(Row) > LastRow) then
      continue;
    Total := SheetInterface.Totals.FindByColumn(ColumnNumber, tempInt);

    // показатель удалили
    // заменяем его на #ССЫЛКА! = fmIncorrectRef
    if (Total = nil) then
    begin
      Template := ReplaceCellRef(Template, ReplacedCellRef, fmIncorrectRef);
      continue;
    end;
    ParamElement := CreateAndAddChild(FormulaElement, ParamName);
    FillNodeAttributes(ParamElement, [attrOffset, Offset, 'total', Total.UniqueID]);
    if (not Total.IsIgnoredColumnAxis) and (SheetInterface.Columns.Count <> 0) then
      if not AddAxisFreeData(SheetInterface.Columns, ColumnsLeaf, ColumnNumber,
        ParamElement) then
        continue;
    if (SheetInterface.Rows.Count <> 0) then
      if not AddAxisFreeData(SheetInterface.Rows, StrToInt(Row), RowsLeaf, ParamElement) then
        continue;
    Template := ReplaceCellRef(Template, ReplacedCellRef, ParamName);
  end;
  SetAttr(FormulaElement, attrTemplate, Template);
end;*)

(*procedure GetTypeFormula(Total: TSheetTotalInterface; Row, Column: integer;
  var FormulaNode: IXMLDOMNode);
var
  LocalFormula: string;
  ExcelSheet: ExcelWorksheet;
begin
  if not(Assigned(Total) and Assigned(FormulaNode)) then
    exit;
  ExcelSheet := Total.SheetInterface.ExcelSheet;
  if not GetCellFormula(ExcelSheet, Row, Column, LocalFormula) then
    exit;
  AddEncodedTypeFormula(Total.SheetInterface, LocalFormula, FormulaNode, Row, Column);
end;

function GetTypeFormula(Total: TSheetTotalInterface; Row,
  Column: integer): TTypeFormula;
var
  DOM: IXMLDOMDocument2;
  FormulaNode: IXMLDOMNode;
begin
  result := nil;
  if not Assigned(Total) then
    exit;
  result := TTypeFormula.Create(Total);
  DOM := InitXmlDocument;
  try
    FormulaNode := DOM.CreateNode(1, attrTypeFormula, '');
    GetTypeFormula(Total, Row, Column, FormulaNode);
    result.ReadFromXML(FormulaNode);
  finally
    FormulaNode := nil;
    KillDOMDocument(DOM);
  end;
end;

{если исключение из типовой формулы, вернет true}
function IsTypeFormulaException(Total: TSheetTotalInterface; Row, Column,
  Section: integer): boolean;
var
  TotalRange: ExcelRange;
  Model: TSheetInterface;
  EncodedCellFormula: TTypeFormula;
begin
  result := false;
  if not Assigned(Total) then
    exit;
  if not Total.TypeFormula.Enabled then
    exit;
  Model := Total.SheetInterface;

  {итог является исключением если в нем не типовая формула}
  if IsSummaryCell(Total.SheetInterface.ExcelSheet, Row, Column) then
  begin
    if not(Total.CountMode in [mcmTypeFormula]) then
      result := true;
    exit;
  end;
  {в области показателя может быть отдельная ячейка}
  if Model.WritablesInfo.IsSingleCellSelected(Model.ExcelSheet, Row, Column) then
  begin
    result := true;
    exit;
  end;
  
  TotalRange := Total.GetTotalRange(Section);

  EncodedCellFormula := GetTypeFormula(Total, Row, Column);
  result := not Total.TypeFormula.IsEqual(EncodedCellFormula, true);
end;

{если исключение из типовой формулы, вернет true}
function IsTypeFormulaException(Total: TSheetTotalInterface; Row, Column: integer): boolean;
var
  TotalRange: ExcelRange;
  i, SectionIndex: integer;
begin
  result := false;
  if not Assigned(Total) then
    exit;
  if not Total.TypeFormula.Enabled then
    exit;

  SectionIndex := 0;
  for i := 0 to Total.SectionCount - 1 do
  begin
    SectionIndex := i;
    TotalRange := Total.GetTotalRange(i);
    if TotalRange.Column = Column then
      break;
  end;
  result := IsTypeFormulaException(Total, Row, Column, SectionIndex);
end;

{мапит типовую формулу конкретному показателю}
procedure MapTypeFormula(Total: TSheetTotalInterface);
var
  s, r, c: integer;
  TotalRange, CellRange: ExcelRange;
  CellValue, CellFormula, CellStyle, DecodedTypeFormula: string;
  IsItSummary: boolean;
  SheetInterface: TSheetInterface;
  ExcelSheet: ExcelWorksheet;
begin
  if not Assigned(Total) then
    exit;
  if not(Total.TypeFormula.Enabled and (Total.TypeFormula.Template <> '')) then
    exit;
  SheetInterface := Total.SheetInterface;
  ExcelSheet := SheetInterface.ExcelSheet;

  for s := 0 to Total.SectionCount - 1 do
  begin
    TotalRange := Total.GetTotalRange(s);
    if not Assigned(TotalRange) then
      continue;
    c := TotalRange.Column;
    for r := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
    begin
      {отдельные ячейки не трогаем}
      if SheetInterface.WritablesInfo.IsSingleCellSelected(ExcelSheet, r, c) then
        continue;
      IsItSummary := IsSummaryCell(Total.SheetInterface.ExcelSheet, r, c);
      if not IsItSummary or ((Total.CountMode = mcmTypeFormula) and Total.SummariesByVisible) then
      begin
        CellRange := GetRange(ExcelSheet, r, c, r, c);
        GetCellValue(ExcelSheet, r, c, Total.EmptyValueSymbol,
          CellValue, CellFormula, CellStyle);
        {Размещаем типовую формулу в ячейку если:
        1)(Нет значения в ячейке или показатель "результат") и нет формулы в ячейке
        2) Если формула в ячейке равна символу пустого значения показателя
        3) Если это итог}
        if (((CellValue = '') or (Total.TotalType in [wtResult])) and
          not IsExistsFormula(CellRange)) or (CellFormula = Total.EmptyValueSymbol) or IsItSummary then
        begin
          DecodedTypeFormula := DecodeTypeFormula(SheetInterface,
            Total.TypeFormula, TotalRange.Row, r, s);
          SetCellFormula(ExcelSheet, r, c, DecodedTypeFormula);
          CellRange.Interior.Pattern := xlSolid;
        end
        else
        begin
          if IsTypeFormulaException(Total, r, c, s) then
          begin
            {В ячейке исключение, помечаем соответствующим образом}
            CellRange.Interior.PatternColorIndex := 32;
            CellRange.Interior.Pattern := xlGray16;
          end
          else
            {Типовые формулы совпадают, значит у ячейки снимаем ярлык иключения}
            CellRange.Interior.Pattern := xlSolid;
        end;
     end;
   end;
  end;
end;   *)

{размещает TypeFormula на лист}
(*procedure MapTypeFormulas(SheetInterface: TSheetInterface);
var
  i, r, s, c: integer;
  TotalRange: ExcelRange;
  Total:  TSheetTotalInterface;
  TypeFormula: TTypeFormula;
begin
  if not Assigned(SheetInterface) then
    exit;
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    Total := SheetInterface.Totals[i];
    TypeFormula := Total.TypeFormula;
    if not(TypeFormula.Enabled and (TypeFormula.Template <> '')) then
      continue;

    for s := 0 to Total.SectionCount - 1 do
    begin
      TotalRange := Total.GetTotalRange(s);
      if not Assigned(TotalRange) then
        continue;

      c := TotalRange.Column;
      for r := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
      begin
        if SheetInterface.WritablesInfo.IsSingleCellSelected(SheetInterface.ExcelSheet,
        r, c) then
          continue;

        if not IsSummaryCell(Total.SheetInterface.ExcelSheet, r, c) or
        ((Total.CountMode = mcmTypeFormula) and Total.SummariesByVisible) then
          SetCellFormula(SheetInterface.ExcelSheet, r, c,
            DecodeTypeFormula(SheetInterface, TypeFormula, TotalRange.Row, r, s));
      end;
    end;
  end;
end; *)

(*procedure ClearTypeFormulaValue(Total: TSheetTotalInterface);
var
  s, r, c: integer;
  TotalRange: ExcelRange;
begin
  if not(Assigned(Total) and Assigned(Total.TypeFormula)) then
    exit;
  if not Total.TypeFormula.Enabled then
    exit;
  for s := 0 to Total.SectionCount - 1 do
  begin
    TotalRange := Total.GetTotalRange(s);
    if not Assigned(TotalRange) then
      continue;
    c := TotalRange.Column;
    for r := TotalRange.Row to TotalRange.Row + TotalRange.Rows.Count - 1 do
      if not IsTypeFormulaException(Total, r, c, s) then
        GetRange(Total.SheetInterface.ExcelSheet, r, c).ClearContents;
        //SetCellFormula(TotalRange.Worksheet, r, c, '');
  end;
end;  *)

{очистить значения типовых формул ссылающихся на указанный алиас показателя}
(*procedure ClearLinkedTypeFormulasValue(Model: TSheetInterface; TotalAlias: string);
var
  CurTotal: TSheetTotalInterface;
  i: integer;
begin
  if not Assigned(Model) then
    exit;

  Model.WritablesInfo.UpdateSingleCellsPoints(Model.ExcelSheet);
  for i := 0 to Model.Totals.Count - 1 do
  begin
    CurTotal := Model.Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(TotalAlias) then
      {очищаем значения типовой формулы}
      ClearTypeFormulaValue(CurTotal);
  end;
end;*)

{разместить значения типовых формул ссылающихся на указанный алиас показателя}
(*procedure MapLinkedTypeFormulaValue(Model: TSheetInterface; TotalAlias: string);
var
  CurTotal: TSheetTotalInterface;
  i: integer;
begin
  if not Assigned(Model) then
    exit;

  Model.WritablesInfo.UpdateSingleCellsPoints(Model.ExcelSheet);
  for i := 0 to Model.Totals.Count - 1 do
  begin
    CurTotal := Model.Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(TotalAlias) then
      {размещаем значения типовой формулы}
      MapTypeFormula(CurTotal);
  end;
end;*)

{получить список показателей, у которых типовая формула содержит ссылку на Total}
(*function GetReferTotalsList(Total: TSheetBasicTotal): TStringList;
var
  CurTotal: TSheetTotalInterface;
  i: integer;
  Totals: TSheetTotalCollectionInterface;
begin
  result := TStringList.Create;
  if not Assigned(Total) then
    exit;
  Totals := Total.SheetInterface.Totals;
  for i := 0 to Totals.Count - 1 do
  begin
    CurTotal := Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(Total.Alias) then
      result.Add(CurTotal.Caption);
  end;
end;

{Если на указанный показатель имеются ссылки из типовых формул других показателей,
вернет предупреждение с перечислением ссылающихся показателей}
function GetWarningText(Total: TSheetBasicTotal): string;
var
  TotalList: TStringList;
  i: integer;
begin
  result := '';
  if not Assigned(Total) then
    exit;
  TotalList := GetReferTotalsList(Total);
  try
    if TotalList.Count = 0 then
      exit;
    result := 'ВНИМАНИЕ. На удаляемый элемент ссылаются типовые формулы следующих показателей:' + #10;
    for i := 0 to TotalList.Count - 1 do
      result := result + '"'+ TotalList.Strings[i] + '", ';
    result[Length(result) - 1] := '.';
    result := result + #10 + 'При удалении типовые формулы указанных показателей станут некорректны.';
  finally
    FreeStringList(TotalList);
  end;
end;*)

end.


