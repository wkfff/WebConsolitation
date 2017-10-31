Sub StartProgress
	On Error Resume Next
	Set Progress = CreateObject("CommonTools.Progress")
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Call Progress.StartProgress(False)
		Progress.ProgressCaption = "Создание базы Бюджета"
	End If
End Sub

Sub StopProgress
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Call Progress.StopProgress
		Set Progress = Nothing
	End If
End Sub

Sub SetProgressPos(Pos)
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Progress.Position = Pos
	End If
End Sub

Sub SetProgressMaxPos(MaxPos)
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Progress.MaxProgress = MaxPos
	End If
End Sub

Sub ProgressCaption(Caption)
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Progress.ProgressCaption = Caption
	End If
End Sub

Sub ProgressMsg(Message)
	If not IsEmpty(Progress) and TypeName(Progress) <> "Nothing" Then
		Progress.ProgressMsg = Message
		If Caption <> "" Then
			Progress.ProgressCaption = Caption
		End If
	End If
End Sub

Sub CreateFile(FileName, Text)
' Создаёт файл с именем FileName и содержимым Text
	Dim ForWriting, FileStream
	' Удалим старый файл, чтобы не мешал.
	On Error Resume Next
	Call Err.Clear
	Call FSO.DeleteFile(FileName, True)
	ForWriting = 2
	'Создаём текстовый файл для записи.
	set FileStream = FSO.OpenTextFile(FileName, 2, ForWriting, TristateFalse)
	'Сохраняем файл.
	call FileStream.Write(Text)
	call FileStream.Close
	GeneratedFiles = GeneratedFiles & FileName & vbCrLf
End Sub 'CreateFile(FileName, Text)

Function TrimName(Prefix, Name, Sufix, MaxLength)
' К имени Name добавляет спереди Prefix, а сзади Sufix.
' Полученное имя втискивается в MaxLength символов, то есть если название превышает
' длину 30 символов, то выкивыдывает из имени "лишние" гласные буквы.
	Dim NewName, NameLen, i
	NameLen = MaxLength - Len(Prefix) - Len(Sufix)
	NewName = Name
	i = Len(NewName)
	' Если не умещается в нужные размеры - выкидываем гласные буквы, двигаясь с конца.
	While i > 1 and Len(NewName) > NameLen
		If InStr(1, "AEIOUYaeiouy", Mid(NewName, i, 1), 1) > 0 Then 'Если это гласная буква
			NewName = Mid(NewName, 1, i - 1) & Mid(NewName, i + 1)
		End If 'InStr(1, "AEIOUYaeiouy", Mid(NewName, i, 1), 1) <> 0
		i = i - 1
        Wend ' i > 1 and Len(NewName) > NameLen
        TrimName = Prefix & NewName & Sufix
End Function 'TrimName
