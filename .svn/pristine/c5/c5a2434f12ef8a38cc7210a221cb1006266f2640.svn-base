' Генерирует sql-скрипты системы "Финансовый анализ" для базы данных Oracle.

UDLFile = "GenerateScriptsDVOracle.udl" ' Для второй генерации для FM

FMSchemaName = "DV"
IndexTableSpace = "DVINDX" ' Имя табличного пространства для хранения индексов в Oracle

' Пути к генерируемым файлам
TargetPath = "E:\dotNET\Database\Oracle\Generated\"

' Генерируемые файлы
' создание индексов по внешним ключам в Oracle.
ForeignKeyIndicesFile = TargetPath & "ForeignKeyIndices.sql"
' перенос индексов по первичным ключам и ограничениям уникальности в другое табличное пространство (файл) для Oracle
MoveIndicesFile = TargetPath & "MoveIndices.sql"

'-------------------------------------------------------------------------------
' Подключаем процедуры
Dim FSO
Set FSO = CreateObject("Scripting.FileSystemObject")
Set VBSFile = FSO.OpenTextFile("UtilsGenerate.vbs", 1)
VBSCode = VBSFile.ReadAll
Call ExecuteGlobal(VBSCode)

'-------------------------------------------------------------------------------
' Подключимся к базе данных
Set DataAccess = CreateObject("DataAccess.GetData")
tmpResult =  DataAccess.InitDBConnect("File Name=" & UDLFile)
If not tmpResult Then
	MsgBox DataAccess.LastError
End if

' Определим версию базы данных
tmpResult = DataAccess.GetRecordSetScr("select Name from DatabaseVersions where ID = (select max(ID) from DatabaseVersions)", tmpRecordSet)
Version =  tmpRecordSet(0)
' Закрываем рекордсет
tmpRecordSet.Close
set tmpRecordSet = Nothing

GeneratedFiles = "Сгенерированы файлы:" & vbCrLf ' Список обработанных файлов

' Генерируем скрипты
'-------------------------------------------------------------------------------
' Создание в Оракле индексов по внешним ключам.
CreateText = _
	"/*" & vbCrLf &_
	"	АИС ""Анализ и планирование""" & vbCrLf &_
	"	ВЕРСИЯ	" & Version & vbCrLf &_
	"	МОДУЛЬ" & vbCrLf &_
	"		ForeignKeyIndices.sql - Создание индексов по внешним ключам" & vbCrLf &_
	"	СУБД	Oracle 9.2" & vbCrLf &_
	"*/" & vbCrLf & vbCrLf

' Получим списки внешних ключей
tmpResult = DataAccess.GetRecordSetScr("select RefConstr.Constraint_Name, t.Table_Name " &_
		"from DBA_Tables t, DBA_Constraints RefConstr " &_
		"where t.Owner = '" & FMSchemaName & "' and RefConstr.Owner = '" & FMSchemaName & "' " &_
		"and RefConstr.Table_Name = t.Table_Name and RefConstr.Constraint_Type = 'R' " &_
		"order by t.Table_Name", tmpRecordSet)
While (not tmpRecordSet.EOF)
	' Получим список полей во внешнем ключе
	tmpResult = DataAccess.GetRecordSetScr("select ConsCols.Column_Name " &_
		"from DBA_Cons_Columns ConsCols " &_
		"where ConsCols.Constraint_Name = '" & tmpRecordSet.Fields(0).Value & "' " &_
		"order by ConsCols.Column_Name", tmpRecordSet2)
	CreateText = CreateText & "create index " & TrimName("i_", tmpRecordSet.Fields(0).Value,"", 30) &_
		" on " & tmpRecordSet.Fields(1).Value &	" ("
	While (not tmpRecordSet2.EOF)
	        CreateText = CreateText & tmpRecordSet2.Fields(0).Value
	        tmpRecordSet2.MoveNext
	        If not tmpRecordSet2.EOF Then CreateText = CreateText & ", "
	Wend
	tmpRecordSet2.Close
	set tmpRecordSet2 = Nothing

	CreateText = CreateText & ") tablespace " & IndexTableSpace & " compute statistics;" & vbCrLf
        tmpRecordSet.MoveNext
Wend

' Закрываем рекордсет
tmpRecordSet.Close
set tmpRecordSet = Nothing
CreateText = CreateText & "commit work;" & vbCrLf

Call CreateFile(ForeignKeyIndicesFile, CreateText)

'-------------------------------------------------------------------------------
'Перенос индексов по первичным ключам и ограничениям уникальности в другое табличное пространство (файл) для Oracle
CreateText = _
	"/*" & vbCrLf &_
	"	АИС ""Анализ и планирование""" & vbCrLf &_
	"	ВЕРСИЯ	" & Version & vbCrLf &_
	"	МОДУЛЬ" & vbCrLf &_
	"		ForeignKeyIndices.sql - Перенос автоматически созданных индексов в табличное пространство индексов " & IndexTableSpace & vbCrLf &_
	"	СУБД	Oracle 9.2" & vbCrLf &_
	"*/" & vbCrLf & vbCrLf

tmpResult = DataAccess.GetRecordSetScr("select Index_Name from DBA_Indexes where " &_
		"Owner = '" & FMSchemaName & "' and TableSpace_Name = '" & FMSchemaName & "' and Generated = 'N'", _
		tmpRecordSet)
While (not tmpRecordSet.EOF)
	CreateText = CreateText & "alter index " & tmpRecordSet.Fields(0).Value & " rebuild tablespace " & IndexTableSpace &_
				" compute statistics;" & vbCrLf
        tmpRecordSet.MoveNext
Wend

' Закрываем рекордсет
tmpRecordSet.Close
set tmpRecordSet = Nothing
CreateText = CreateText & "commit work;" & vbCrLf

' Сохраним в файл.
Call CreateFile(MoveIndicesFile, CreateText)

'-------------------------------------------------------------------------------
' Всё закончили. Сообщим о выполнении пользователю.
MsgBox GeneratedFiles

' Отключимся от базы данных
Call DataAccess.ClearAllConnections
Set DataAccess = Nothing
