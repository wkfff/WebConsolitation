Attribute VB_Name = "KristaPlaningVBModule"
Private Function GetPlaningInterface() As IFMPlanningVBProgramming
    Set GetPlaningInterface = Nothing
    For i = 1 To Application.COMAddIns.Count
        If Application.COMAddIns.Item(i).progID = "FMExcelAddIn.DTExtensibility2" Then
            Set GetPlaningInterface = Application.COMAddIns.Item(i).Object
            Exit Function
        End If
    Next
End Function

Public Function ѕолучить—войство(»м€—войства As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучить—войство = Obj.VBGetPropertyByName(»м€—войства)
End Function

Public Sub «аписать—войство(»м€—войства As String, «начение—войства As String)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetPropertyByName(»м€—войства, «начение—войства)
End Sub

Public Function ѕолучить онстанту(»м€ онстанты As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучить онстанту = Obj.VBGetConstValueByName(»м€ онстанты)
End Function

Public Sub «аписать онстанту(»м€ онстанты As String, «начение онстанты As String)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetConstValueByName(»м€ онстанты, «начение онстанты)
End Sub

Public Function ќбновитьЋист() As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ќбновитьЋист = Obj.VBRefresh
End Function

Public Function ќбратна€«апись() As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ќбратна€«апись = Obj.VBWriteback
End Function

Public Function “екущее—оединение(URL As String, »м€—хемы As String) As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    “екущее—оединение = Obj.VBGetCurrentConnection(URL, »м€—хемы)
End Function

Public Function ѕолучитьЁлементы»змерени€(»м€»змерени€ As String) As Variant
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучитьЁлементы»змерени€ = Obj.VBGetMembers(»м€»змерени€)
End Function

Public Sub «аписатьЁлементы»змерени€(»м€»змерени€ As String, Ёлементы»змерени€ As Variant)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetMembers(»м€»змерени€, Ёлементы»змерени€)
End Sub

Public Function ѕолучитьЁлементыѕараметра(»м€ѕараметра As String) As Variant
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучитьЁлементыѕараметра = Obj.VBGetParamMembers(»м€ѕараметра)
End Function

Public Sub «аписатьЁлементыѕараметра(»м€ѕараметра As String, Ёлементыѕараметра As Variant)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetParamMembers(»м€ѕараметра, Ёлементыѕараметра)
End Sub

Public Function –едактироватьЁлементы»змерени€(»м€»змерени€ As String) As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    –едактироватьЁлементы»змерени€ = Obj.VBEditMembers(»м€»змерени€)
End Function

Public Function ѕолучить—войствоЁлемента(»м€»змерени€ As String, ”никальное»м€ As String, »м€—войстваЁлемента As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучить—войствоЁлемента = Obj.VBGetMemberProperty(»м€»змерени€, ”никальное»м€, »м€—войстваЁлемента)
End Function

Public Function ѕолучить«начениеќтдельногоѕоказател€(»м€ќтдельногоѕоказател€ As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучить«начениеќтдельногоѕоказател€ = Obj.VBGetSingleCellValue(»м€ќтдельногоѕоказател€)
End Function

Public Function ѕолучить«начениеѕоказател€(»м€ѕоказател€ As String,  оординаты As Variant) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ѕолучить«начениеѕоказател€ = Obj.VBGetTotalValue(»м€ѕоказател€,  оординаты)
End Function












