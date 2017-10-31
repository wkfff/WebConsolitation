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

Public Function ����������������(����������� As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ���������������� = Obj.VBGetPropertyByName(�����������)
End Function

Public Sub ����������������(����������� As String, ���������������� As String)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetPropertyByName(�����������, ����������������)
End Sub

Public Function �����������������(������������ As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ����������������� = Obj.VBGetConstValueByName(������������)
End Function

Public Sub �����������������(������������ As String, ����������������� As String)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetConstValueByName(������������, �����������������)
End Sub

Public Function ������������() As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������ = Obj.VBRefresh
End Function

Public Function ��������������() As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    �������������� = Obj.VBWriteback
End Function

Public Function �����������������(URL As String, �������� As String) As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ����������������� = Obj.VBGetCurrentConnection(URL, ��������)
End Function

Public Function �������������������������(������������ As String) As Variant
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������������������� = Obj.VBGetMembers(������������)
End Function

Public Sub �������������������������(������������ As String, ����������������� As Variant)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetMembers(������������, �����������������)
End Sub

Public Function �������������������������(������������ As String) As Variant
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������������������� = Obj.VBGetParamMembers(������������)
End Function

Public Sub �������������������������(������������ As String, ����������������� As Variant)
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    Call Obj.VBSetParamMembers(������������, �����������������)
End Sub

Public Function ������������������������������(������������ As String) As Boolean
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������������������������ = Obj.VBEditMembers(������������)
End Function

Public Function ������������������������(������������ As String, ������������� As String, ������������������� As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������������������ = Obj.VBGetMemberProperty(������������, �������������, �������������������)
End Function

Public Function ������������������������������������(����������������������� As String) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    ������������������������������������ = Obj.VBGetSingleCellValue(�����������������������)
End Function

Public Function ��������������������������(������������� As String, ���������� As Variant) As String
    Dim Obj As IFMPlanningVBProgramming
    Set Obj = GetPlaningInterface
    �������������������������� = Obj.VBGetTotalValue(�������������, ����������)
End Function












