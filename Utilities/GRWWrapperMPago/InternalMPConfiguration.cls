VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalMPConfig"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Private m_Col As Collection
Private m_UserID As String
Private m_TokenAPP As String
Private oQuerys As Querys


Public Function Add(StoreID As Long, StoreName As String, userMP As String, Token As String, ExtID As String, FecCre As Date, EndPoint As String, oBrachs As InternalBranchs) As InternalMPClass

    Dim oInternal As InternalMPClass
    Dim Key As String
    Key = CStr(StoreID)
    Set oInternal = Item(StoreID)
    If oInternal Is Nothing Then
        Set oInternal = New InternalMPClass
        With oInternal
            .StoreID = StoreID
            .StoreName = StoreName
            .UserID = userMP
            .TokenAPP = Token
            .ExtStoreID = ExtID
            .FecCre = FecCre
            .EndPoint = EndPoint
            Set .Branchs = oBrachs
        End With
        m_Col.Add oInternal, Key
    End If
    Set Add = oInternal
End Function
Public Function Item(index) As InternalMPClass
    Dim oInternal As InternalMPClass
    If Count > 0 Then
        Set oInternal = m_Col(index)
    End If
    Set Item = oInternal
    Set oInternal = Nothing
End Function

Public Sub Initialize(Who As Object)
    m_UserID = Who.GetParameters()
    m_TokenAPP = Who.GetParameters()
    
End Sub

Public Property Get NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
    Set NewEnum = m_Col.[_NewEnum]
End Property
Public Function Count() As Long
    Count = m_Col.Count
End Function
Private Function GetInternalItem(index As Variant) As InternalMPClass '#NOERRORHANDLING
Dim oInternal As InternalMPClass
On Error GoTo EH
    If m_Col.Count > 0 Then
        Set oInternal = m_Col(index)
    End If
    Set GetInternalItem = oInternal
    Set oInternal = Nothing
Exit Function
EH:
Set oInternal = Nothing
End Function
'Private Function GetConfigStoreMP(Who As cwTManBZ.ObjectInstance, StoreId As String, posID As String, CodEmp As String) As InternalMPClass
'On Error GoTo EH
'    Dim TimeOut As Long
'    Dim Rs As rdoResultset
'    Dim oInternal As InternalMPClass
'    Dim sSql As String
''    GRTMPH_CODEMP
''    GRTMPH_STORID
''    GRTMPH_STNAME
''    GRTMPH_EXTRID
''    GRTMPH_USERMP
''    GRTMPH_BTOKEN
''    GRTMPH_FECCRE
''    '*************
''    GRTMPI_STORID
''    GRTMPI_CODEMP
''    GRTMPI_BRANID
''    GRTMPI_PSNAME
''    GRTMPI_EXTPID
''    GRTMPI_FIXAMN
''    GRTMPI_QRIMGN
'    sSql = oQuerys.QueryGetStoreConfig(StoreId, posID, CodEmp)
'    Set Rs = OPenResultSet(Who, sSql)
''    sSql = "Select s.GRTMPH_STORID, s.GRTMPH_STNAME, s.GRTMPH_FECALT, s.GRTMPH_EXTRID, s.GRTMPH_USERID, s.GRTMPH_BTOKEN"
''    sSql = sSql & ", p.GRTMPI_BRANID, p.GRTMPI_EXTPID, p.GRTMPI_FIXAMN, p.GRTMPI_QRIMGN, p.GRTMPI_TIPPRO, p.GRTMPI_ARTCOD, p.GRTMPI_FECALT"
''   STMPDH_DESCRP, IsNull(STTUNI_DESCRP) As STTUNI_DESCRP"
'    If Not Rs.EOF Then
'        Set oInternal = New InternalMPClass
'        TimeOut = SuperVal(Rs("GRTMPI_TIMOUT"))
'        With oInternal
'            .DefaultItemDesc = Rs("STMPDH_DESCRP").Value & vbNullString
'            .DefaultItemUnit = Rs("STTUNI_DESCRP").Value & vbNullString
'            .TokenAPP = Rs("GRTMPH_BTOKEN").Value & vbNullString
'            .TimeOut = IIf(TimeOut < 60, TIMEOUT_ORDER, TimeOut)
'            .UserId = Rs("GRTMPH_USERID").Value & vbNullString
'            Set .BranchConfig = New InternalBranch
'            With .BranchConfig
'                .id = SuperVal(Rs("GRTMPI_BRANID"))
'                .external_id = Rs("GRTMPI_EXTPID").Value & vbNullString
'                .external_store_id = Rs("GRTMPH_EXTRID").Value & vbNullString
'                .store_id = SuperVal(Rs("GRTMPH_STORID"))
'                .itemdesc = Rs("STMPDH_DESCRP").Value & vbNullString
'                .itemunit = Rs("STTUNI_DESCRP").Value & vbNullString
'            End With
'        End With
'    End If
'    Set GetConfigStoreMP = oInternal
'Exit Function
'EH:
'    Set oInternal = Nothing
'
'End Function
Private Function GetKeySotrePos(StoreID As String, posID As String) As String
    GetKeySotrePos = Trim$(StoreID) & Trim$(posID)
End Function

Private Sub Class_Initialize()
    Set oQuerys = New Querys
    Set m_Col = New Collection
End Sub

Private Sub Class_Terminate()
    Set oQuerys = Nothing
    Set m_Col = Nothing
End Sub
