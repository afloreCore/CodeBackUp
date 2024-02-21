VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalSite"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
' void ConfigSiteSitef(string IPSitef, string TerminalID, string Comercio, string Cuit, string CUITisv = "", string args = "");
' void ConfigTransaction(TipoFuncion TipoDeFuncion, TipoOperacion TipoDeOperacion = TipoOperacion.General, TipoPinPad Dispositivo = TipoPinPad.PinPad, int PuerCOM = 0, string IPClover = "");
' bool Begin(string NroComprobante, double Monto, TipoFormaPago TipoPago, TipoTarjeta TipoTarjeta, EmpTarjeta CodTarjeta, string FechaComprobante = "", short NroDeCuotas = 0, string Usuario = "");

Public IPServer As String
Public IPClover As String
Public Terminal_ID As String
Public Comercio As String
Public CUIT As String
Public CUITISV As String
Public Nombre As String
Public Args As String
Public TipoPinPad As TipoPinPad
Public Puerto As String
Public BaudRate As Integer
Public ByteSize As Integer
Public Parity As Byte
Public StopBits As Integer
Public TiempoTmpCopia As Integer
Public TimeOut As Integer
Public CodAdmin As String
Public HTTPPinpad As String
Public PassWord As String
Public Imprimir As String
Public AmbTest As String


'#Parte->AF1905 ->
Private Sub Class_Initialize()
    #If VB5 = 1 Then
        LogVersion
    #End If
End Sub
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
