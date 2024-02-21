VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalBranchs"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Private m_Col As Collection
Public Function Add(id As Long, linkImageQr As String, status As String, date_created As Date, date_last_updated As Date, name As String, _
    fixed_amount As Boolean, store_id As Long, external_store_id As String, external_id As String, site As String, _
    category As InterfaceComNetCore.BranchCategories, itemdesc As String, itemunit As String, TimeOut As Double) As InternalBranch
    Dim oInternal As InternalBranch
    Set oInternal = New InternalBranch
    With oInternal
        .id = id
        .name = name
        .external_id = external_id
        .external_store_id = external_store_id
        .fixed_amount = fixed_amount
        .store_id = store_id
        .linkImageQr = linkImageQr
        .date_created = date_created
        .date_last_updated = date_last_updated
        .category = category
        .status = status
        .itemdesc = itemdesc
        .itemunit = itemunit
        .TimeOut = TimeOut
        .site = site
    End With
    m_Col.Add oInternal, CStr(id)
    Set Add = oInternal
    Set oInternal = Nothing

End Function

Public Function Item(index) As InternalBranch
     If m_Col.Count > 0 Then
        Set Item = m_Col(index)
    End If
End Function
Public Property Get NewEnum() As IUnknown
    Set NewEnum = m_Col.[_NewEnum]
End Property
Public Function Count() As Long
    Count = m_Col.Count
End Function
Private Sub Class_Initialize()
    Set m_Col = New Collection
End Sub

Private Sub Class_Terminate()
    Set m_Col = Nothing
End Sub
