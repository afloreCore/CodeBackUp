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
Private oGrwString As GRWStrings.SqlUtilities
Public Function QueryGetStoreConfig(StoreID As String, posID As String, CodEmp As String) As String
    Dim sSql As String
     With oGrwString
        sSql = "Select s.GRTMPH_STORID, s.GRTMPH_STNAME, s.GRTMPH_FECCRE, s.GRTMPH_EXTRID, s.GRTMPH_USERID, s.GRTMPH_BTOKEN"
        sSql = sSql & ", p.GRTMPI_BRANID, p.GRTMPI_EXTPID, p.GRTMPI_FIXAMN, p.GRTMPI_QRIMGN, p.GRTMPI_TIPPRO, p.GRTMPI_ARTCOD, p.GRTMPI_TIMOUT, p.GRTMPI_FECCRE"
        sSql = sSql & ", p.GRTMPI_PSNAME, IsNull(STMPDH_DESCRP,'') As STMPDH_DESCRP, IsNull(STTUNI_DESCRP, '') As STTUNI_DESCRP"
        sSql = sSql & " From GRTMPH s"
        sSql = sSql & " Inner Join GRTMPI p On(GRTMPH_STORID = GRTMPI_STORID)"
        sSql = sSql & " Left Join STMPDH On(STMPDH_TIPPRO = GRTMPI_TIPPRO And STMPDH_ARTCOD = GRTMPI_ARTCOD)"
        sSql = sSql & " Left Join STTUNI On(STTUNI_UNIDAD = STMPDH_UNIMED)"
        sSql = sSql & " Where s.GRTMPH_STORID = " & .SQLValue(dtLong, StoreID)
        sSql = sSql & " And p.GRTMPI_BRANID = " & .SQLValue(dtLong, posID)
        If Len(CodEmp) > 0 Then
            sSql = sSql & " And s.GRTMPH_CODEMP = " & .SQLValue(dtString, CodEmp)
            sSql = sSql & " And p.GRTMPI_CODEMP = s.GRTMPH_CODEMP"
        End If
    End With
    QueryGetStoreConfig = sSql
        
End Function
Public Function QueryStoreAndBranchFromSucurs(Sucurs As String, CodEmp As String) As String
    Dim sSql As String
    With oGrwString
'        sSql = ";WITH T1(CODEMP, CODFOR, STORID, STNAME, USERMP, BTOKEN, ST_EXTRID, ST_FECCRE, BRANID, BRNAME, BR_EXTRID, FIXAMN, BR_FECCRE, DISBLD, QRIMGN, TIMOUT, ARTCOD_DES, UNIMED) AS"
'        sSql = sSql & " ("
'        sSql = sSql & " SELECT DISTINCT GRCFOR_CODEMP, GRCFOR_CODFOR, GRTMPH_STORID, GRTMPH_STNAME, GRTMPH_USERMP, GRTMPH_BTOKEN, GRTMPH_EXTRID, GRTMPH_FECCRE, GRTMPI_BRANID,"
'        sSql = sSql & " GRTMPI_PSNAME , GRTMPI_EXTPID, GRTMPI_FIXAMN, GRTMPI_FECCRE, GRTMPI_DISBLD, GRTMPI_QRIMGN, GRTMPI_TIMOUT, ISNULL(STMPDH_DESCRP,'') ARTCOD_DES, ISNULL(STMPDH_UNIMED,'') UNIMED"
'        sSql = sSql & " From GRTSUC"
'        sSql = sSql & " INNER JOIN GRCFOR ON(GRCFOR_CODEMP = GRTSUC_EMPSUC AND GRCFOR_SUCURS = GRTSUC_SUCURS)"
'        sSql = sSql & " INNER JOIN GRCCBF ON(GRCCBF_CODEMP = GRCFOR_CODEMP AND GRCCBF_MODFOR = GRCFOR_MODFOR AND GRCCBF_CODFOR = GRCFOR_CODFOR)"
'        sSql = sSql & " INNER JOIN GRCCBH V ON(V.GRCCBH_MODCOM = GRCCBF_MODCOM AND V.GRCCBH_CODCOM = GRCCBF_CODCOM)"
'        sSql = sSql & " INNER JOIN GRTMPH ON(GRTMPH_CODEMP = GRTSUC_EMPSUC AND GRTMPH_STORID = GRTSUC_STORID)"
'        sSql = sSql & " INNER JOIN GRTMPI ON(GRTMPI_CODEMP = GRTMPH_CODEMP AND GRTMPI_STORID = GRTMPH_STORID AND GRTMPI_BRANID = GRTSUC_BRANID)"
'        sSql = sSql & " LEFT JOIN STMPDH ON(STMPDH_TIPPRO = GRTMPI_TIPPRO AND STMPDH_ARTCOD = GRTMPI_ARTCOD)"
'        sSql = sSql & " WHERE GRTSUC_SUCURS = " & .SQLValue(dtString, Sucurs)
'        sSql = sSql & " AND GRCFOR_MODFOR = " & .SQLValue(dtString, ModFor)
'        sSql = sSql & " AND GRCFOR_CODFOR = " & .SQLValue(dtString, CodFor)
        sSql = "SELECT GRTSUC_EMPSUC CODEMP, GRTMPH_STORID STORID, GRTMPH_STNAME STNAME, GRTMPH_USERMP USERMP, GRTMPH_BTOKEN BTOKEN, GRTMPH_EXTRID ST_EXTRID, GRTMPH_FECCRE ST_FECCRE, GRTMPH_EPOINT ENDPOINT, GRTMPI_BRANID BRANID,"
        sSql = sSql & " GRTMPI_PSNAME BRNAME, GRTMPI_EXTPID BR_EXTRID, GRTMPI_FIXAMN FIXAMN, GRTMPI_FECCRE BR_FECCRE, GRTMPI_QRIMGN QRIMGN, GRTMPI_TIMOUT TIMOUT, ISNULL(STMPDH_DESCRP,'') ARTCOD_DES,  ISNULL(STMPDH_UNIMED,'') UNIMED"
        sSql = sSql & " FROM GRTSUC"
        sSql = sSql & " INNER JOIN GRTMPH ON(GRTMPH_CODEMP = GRTSUC_EMPSUC AND GRTMPH_STORID = GRTSUC_STORID)"
        sSql = sSql & " INNER JOIN GRTMPI ON(GRTMPI_CODEMP = GRTMPH_CODEMP AND GRTMPI_STORID = GRTMPH_STORID AND GRTMPI_BRANID = GRTSUC_BRANID)"
        sSql = sSql & " LEFT JOIN STMPDH ON(STMPDH_TIPPRO = GRTMPI_TIPPRO AND STMPDH_ARTCOD = GRTMPI_ARTCOD)"
        sSql = sSql & " WHERE GRTSUC_SUCURS = " & .SQLValue(dtString, Sucurs)
        If Len(CodEmp) > 0 Then
            sSql = sSql & " AND GRTSUC_EMPSUC = " & .SQLValue(dtString, CodEmp)
        End If
    End With
    QueryStoreAndBranchFromSucurs = sSql
End Function
Public Function QueryGetConceptoMP(ModCpt As String, TipCpt As String, CodCpt As String) As String
    Dim sSql As String
    With oGrwString
        sSql = "SELECT 1 FROM CJTCTA"
        sSql = sSql & " INNER JOIN GRCCOH ON(GRCCOH_CJCTACTE = CJTCTA_CTACTE)"
        sSql = sSql & " WHERE  CJTCTA_BILLET = " & .SQLValue(dtString, "MP")
        sSql = sSql & " AND  GRCCOH_MODCPT = " & .SQLValue(dtString, ModCpt)
        sSql = sSql & " AND GRCCOH_TIPCPT = " & .SQLValue(dtString, TipCpt)
        sSql = sSql & " AND GRCCOH_CODCPT = " & .SQLValue(dtString, CodCpt)
    End With
    QueryGetConceptoMP = sSql
End Function

Private Sub Class_Initialize()
    Set oGrwString = New GRWStrings.SqlUtilities
End Sub

Private Sub Class_Terminate()
    Set oGrwString = Nothing
End Sub
