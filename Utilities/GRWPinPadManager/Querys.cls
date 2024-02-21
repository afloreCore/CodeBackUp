VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Querys"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Dim oGrwStrings As GRWStrings.SqlUtilities
Dim sSql As String

Private Sub Class_Initialize()
    Set oGrwStrings = New GRWStrings.SqlUtilities
    '#Parte->AF1905 ->
    #If VB5 = 1 Then
        LogVersion
    #End If
    ' <- AF1905
End Sub
 
Private Sub Class_Terminate()
    Set oGrwStrings = Nothing
End Sub
 
'**************************************************************************
'                           Métodos Públicos
'**************************************************************************
Public Function QueryConfigGRTCOM(ComputerName As String, User As String, TipDip As String) As String
    With oGrwStrings
'    rdSelect("GRTCOM_VELOCI").Value
'        comParams.ByteSize = rdSelect("GRTCOM_BYTESZ").Value
'        comParams.Parity = Asc(rdSelect("GRTCOM_PARITY").Value)
'        comParams.stopBits = rdSelect("GRTCOM_BITSTP").Value
'
        sSql = "SELECT TOP(1) ISNUll(GRTCOM_PUERTO,0) GRTCOM_PUERTO, IsNUll(GRTCOM_DIREIP,'') GRTCOM_DIREIP, GRTCOM_HTTPUE GRTCOM_HTTPUE"
        sSql = sSql & ", GRTCOM_IPCLOV, GRTCOM_TERMID, GRTCOM_COMERC, GRTCOM_NRODOC, GRTCOM_NOMCOM, GRTCOM_TIPPIN, GRTCOM_TMPCON"
        sSql = sSql & ", GRTCOM_PINPUE, GRTCOM_BYTESZ, GRTCOM_PARITY, GRTCOM_BITSTP, GRTCOM_VELOCI, GRTCOM_PARAMS, GRTCOM_AMBIEN, GRTCOM_PRTCUP, GRTCOM_PADMIN PADMIN"
        sSql = sSql & " FROM GRTCOM"
        sSql = sSql & " WHERE GRTCOM_TIPDIP = " & .SQLValue(dtString, TipDip)
        sSql = sSql & " AND GRTCOM_DEBAJA <> " & oGrwStrings.SQLValue(dtString, "S")
        sSql = sSql & " AND GRTCOM_CODIGO =  IsNull((Select TOP (1) GRTCOM_CODIGO FROM GRTCOM WHERE (GRTCOM_CODIGO = " & .SQLValue(dtString, ComputerName) & " OR GRTCOM_CODIGO = " & .SQLValue(dtString, User) & ") AND GRTCOM_TIPDIP = " & .SQLValue(dtString, TipDip) & " AND GRTCOM_DEBAJA <> " & oGrwStrings.SQLValue(dtString, "S") & "), '_DEFAULT_') "
        sSql = sSql & " ORDER BY GRTCOM_APLPUE DESC"
    End With
    QueryConfigGRTCOM = sSql
End Function
 
Public Function QueryGetFECALT(sTable As String, sModfor As String, sCodfor As String, lNrofor As Long) As String
    With oGrwStrings
        sSql = "SELECT CJRMVH_FECALT FECHA FROM " & sTable & " WHERE " & sTable & "_MODFOR = " & .SQLValue(dtString, sModfor)
        sSql = sSql & " AND " & sTable & "_CODFOR = " & .SQLValue(dtString, sCodfor) & " AND " & sTable & "_NROFOR = " & .SQLValue(dtLong, lNrofor)
    End With
    QueryGetFECALT = sSql
End Function
 
Public Function QueryInsTableTrans(TranId As Long, Comercio As String, terminalID As String, PCName As String, UserID As String, Fecha As String, Optional Cupon As String = "", Optional CodAut As String = "", Optional CodNsu As String = "") As String
    With oGrwStrings
'        sSql = "INSERT INTO FCRPOS(FCRPOS_ULTNRO, FCRPOS_COMERC, FCRPOS_TERMID, FCRPOS_USERID, FCRPOS_FCHMOV, FCRPOS_PCNAME, FCRPOS_NCUPON, FCRPOS_CODAUT, FCRPOS_CODNSU, FCRPOS_FECALT)"
'        sSql = sSql & " VALUES(" & .SQLValue(dtLong, TranId) & ", " & .SQLValue(dtString, Comercio) & ", " & .SQLValue(dtString, terminalID) & ", " & .SQLValue(dtString, UserID) & ", " & .SQLValue(dtDate, Fecha) & ", " & .SQLValue(dtString, PCName) & ", " & .SQLValue(dtString, Cupon, True) & ", " & .SQLValue(dtString, CodAut, True) & ", " & .SQLValue(dtString, CodNsu, True) & ", GETDATE())"
 
        'CwSpFCPosFCRPOSIns(@FchMov varchar(10), @Comercio Varchar(10), @Terminal Varchar(10), @User varchar(50) = '', @Computer varchar(30), @Id int Output)
        sSql = "Declare @Id Int"
        sSql = sSql & " Exec CwSpFCPosFCRPOSIns @Comercio =  " & .SQLValue(dtString, Comercio) & ", @Terminal = " & .SQLValue(dtString, terminalID) & ", @User = " & .SQLValue(dtString, UserID) & ", @FchMov = " & .SQLValue(dtDate, Fecha) & ", @Computer = " & .SQLValue(dtString, PCName) & ", @Id = @Id Output"
        sSql = sSql & " Select @Id"
    End With
        QueryInsTableTrans = sSql
End Function
Public Function QueryUpdTableTrans(Fisvid As Long, Optional CodAut As String = "", Optional Cupon As String = "", Optional CodNsu As String = "", Optional CheCkd As String = "", Optional Textos As String = "", Optional Status As String = "", Optional Import As Double = 0) As String
    With oGrwStrings
        sSql = "UPDATE FCRPOS SET FCRPOS_CHECKD = " & .SQLValue(dtString, CheCkd)
        If Len(CodAut) > 0 Then
            sSql = sSql & ", FCRPOS_CODAUT = " & .SQLValue(dtString, CodAut)
        End If
        If Len(Cupon) > 0 Then
            sSql = sSql & ", FCRPOS_NCUPON = " & .SQLValue(dtString, Cupon)
        End If
        If Len(CodNsu) > 0 Then
            sSql = sSql & ", FCRPOS_CODNSU = " & .SQLValue(dtString, CodNsu)
        End If
        If Len(Import) <> 0 Then
            sSql = sSql & ", FCRPOS_IMPORT = " & .SQLValue(dtDouble, Import)
        End If
        If Len(Textos) > 0 Then
            sSql = sSql & ", FCRPOS_TEXTOS = " & .SQLValue(dtString, Textos)
        End If
        sSql = sSql & ", FCRPOS_STATUS = " & .SQLValue(dtString, Status)
        sSql = sSql & " WHERE FCRPOS_NROINT = " & .SQLValue(dtLong, Fisvid)
    End With
    QueryUpdTableTrans = sSql
End Function
Public Function QueryGetLastTrans() As String
    With oGrwStrings
        sSql = "SELECT TOP(1) FCRPOS_NROINT ULTNRO, IsNull(FCRPOS_NCUPON,'') NCUPON, IsNull(FCRPOS_CODAUT,'') CODAUT, IsNull(FCRPOS_CODNSU,'') CODNSU, FCRPOS_CHECKD CHECKD, FCRPOS_STATUS STATUS, FCRPOS_FCHMOV FCHMOV, IsNull(FCRPOS_IMPORT, 0) IMPORT FROM FCRPOS"
        sSql = sSql & " ORDER BY FCRPOS_NROINT DESC"
    End With
    QueryGetLastTrans = sSql
End Function
Public Function QueryGetFieldsFromTrans(NroInt As Long) As String
    With oGrwStrings
        sSql = "SELECT FCRPOS_NROINT ULTNRO, FCRPOS_FCHMOV FCHMOV, IsNull(FCRPOS_NCUPON,'') NCUPON, IsNull(FCRPOS_CODAUT,'') CODAUT, IsNull(FCRPOS_CODNSU,'') CODNSU, FCRPOS_CHECKD CHECKD, FCRPOS_TEXTOS TEXTOS FROM FCRPOS"
        sSql = sSql & " WHERE FCRPOS_NROINT = " & oGrwStrings.SQLValue(dtLong, NroInt)
    End With
    QueryGetFieldsFromTrans = sSql
End Function
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
