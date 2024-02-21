VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalTarjetas"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Private m_Col As Collection

Public Function Add(CodPos As String, CodTja As String, DebCre As String, Optional TipTja As GrwPinPadNet.TipoTarjeta = TipoTarjeta_Niguno, Optional EmpTja As GrwPinPadNet.EmpTarjeta = EmpTarjeta_Otro, Optional Comercio As String = "") As InternalTarjeta
 
    Dim oTja As InternalTarjeta
    Dim lPos As Long
 
    Set oTja = Item(CodTja)
    If oTja Is Nothing Then
        Set oTja = New InternalTarjeta
        With oTja
            .CodPos = CodPos
            .CodTja = CodTja
            .DebCre = DebCre
            .TipTja = TipTja
            .EmpTja = EmpTja
            .CodCio = Comercio
        End With
        m_Col.Add oTja, CodTja
    End If
    Set Add = oTja
    Set oTja = Nothing
 
End Function
 Public Function Item(CodTja As String) As InternalTarjeta
 On Error GoTo EH
    If Count > 0 Then
        Set Item = m_Col(CodTja)
    End If
Exit Function
EH:
 
End Function
 
Public Property Get NewEnum() As IUnknown
    Set NewEnum = m_Col.[_NewEnum]
End Property
Public Function Count() As Long
    Count = m_Col.Count
End Function
 
Private Sub Class_Initialize()
    Set m_Col = New Collection
    #IF VB5 = 1 Then        '#Parte->AF1905
        LogVersion    '#Parte->AF1905
    #End If        '#Parte->AF1905
End Sub
 
Private Sub Class_Terminate()
    Set m_Col = Nothing
End Sub
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
