VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalSites"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Private m_Col As Collection

Public Function Add(IPServer As String, Terminal_ID As String, Comercio As String, CUIT As String, CUITISV As String, Args As String, TipoPinPad As TipoPinPad, Puerto As String, Nombre As String, Optional IPClover As String = "", Optional TiempoTmpCopia As Integer = 0, Optional TimeOut As Integer = 0, Optional HTTPPinpad As String = "", Optional PassWord As String = "", Optional Imprimir As String = "S", Optional AmbTest As String = "N") As InternalSite
 
    Dim oPinPand As InternalSite
    Dim lPos As Long
 
    Set oPinPand = Item(Terminal_ID)
    If oPinPand Is Nothing Then
        Set oPinPand = New InternalSite
        With oPinPand
            .IPServer = IPServer
            .IPClover = IPClover
            .Terminal_ID = Terminal_ID
            .Comercio = Comercio
            .CUIT = CUIT
            .CUITISV = CUITISV
            .Args = Args
            .TipoPinPad = TipoPinPad
            .Puerto = Puerto
            .TiempoTmpCopia = TiempoTmpCopia
            .TimeOut = TimeOut
            .Nombre = Nombre
            .HTTPPinpad = HTTPPinpad
            .PassWord = PassWord
            .Imprimir = Imprimir
            .AmbTest = AmbTest
        End With
        m_Col.Add oPinPand, Terminal_ID
    End If
    Set Add = oPinPand
    Set oPinPand = Nothing
 
End Function
 Public Function Item(Terminal_ID As String) As InternalSite
    If Count > 0 Then
        Set Item = m_Col(Terminal_ID)
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
    '#Parte->AF1905 ->
    #If VB5 = 1 Then
        LogVersion
    #End If
    ' <- AF1905
End Sub
 
Private Sub Class_Terminate()
    Set m_Col = Nothing
End Sub
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
