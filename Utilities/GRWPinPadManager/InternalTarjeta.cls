VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalTarjeta"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Public CodPos As String
Public CodTja As String
Public DebCre As String 'D Débito, C Crédito
Public TipTja As GrwPinPadNet.TipoTarjeta 'Magnética Déb/Cré, Digitada
Public EmpTja As GrwPinPadNet.EmpTarjeta
'Código de Comercio
Public CodCio As String



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
