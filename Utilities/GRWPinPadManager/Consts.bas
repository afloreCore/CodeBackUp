Attribute VB_Name = "Consts"
Option Explicit
Public Const sSlpAsientoCjD = "FCRMVI08"   'Solapa Bebe de tesoreria FCRMVI08
Public Const sSlpAsientoCjH = "FCRMVI09"   'Solapa Haber de tesoreria FCRMVI09
Public Const sHeaderCJ = "CJRMVH"
Public Const cLectorTarj = "LT"            'Lectora de tarjetas
Public Const LECTORA_LAPOS = "LP"
Public Const LECTORA_POSNET = "PO"
Public Const LECTORA_CLOVER = "CL"
Public Const SPLIT_KEY_VALUE = "="
Public Const LIST_DELIMITER = "#"
Public Const FIELD_DELIMITER = ";"
'#Parte->CL-41034->
Public m_bSincronizado As Boolean
'#Parte->CL-41034<-
'#Parte->AFLORE-42->
Public Function OPenResultSet(Who As Object, sSql As String) As rdoResultset '#NOERRORHANDLER
    On Error GoTo EH
    Dim oInstance As cwTManBZ.IObjectHead
    Dim Rs As rdoResultset
    #If VB5 = 1 Then
        Set Rs = Who.DataAccess.OPenResultSet(sSql)
    #Else
        Set oInstance = Who
        Set Rs = oInstance.DataAccess.OPenResultSet(sSql)
    #End If
    Set OPenResultSet = Rs
 
    Set Rs = Nothing
Exit Function
 
EH:
    Set Rs = Nothing
End Function
 
Public Function ExecWithTrans(Who As Object, sSql As String) As Integer '#NOERRORHANDLER
    On Error GoTo EH
    Dim oInstance As cwTManBZ.IObjectHead
    #If VB5 = 1 Then
        With Who.DataAccess
        .BeginTrans
        .Execute (sSql)
        .Flush
        .CommitTrans
        End With
    #Else
        Set oInstance = Who
         With oInstance.DataAccess
            .BeginTrans
            .Execute (sSql)
            .Flush
            .CommitTrans
        End With
    #End If
 
Exit Function
 
EH:
    ExecWithTrans = STOP_IT
End Function
 
Public Sub RollBackTrans(Who As Object)  '#NOERRORHANDLER
    On Error GoTo EH
    Dim oInstance As cwTManBZ.IObjectHead
    Dim TransCount As Integer
    Dim i As Integer
 
    #If VB5 = 1 Then
        TransCount = Who.DataAccess.NestedTransactionCount
        While (i < TransCount)
            Who.DataAccess.RollBackTrans
            i = i + 1
        Wend
    #Else
        Set oInstance = Who
         TransCount = oInstance.DataAccess.NestedTransactionCount
            While (i < TransCount)
                oInstance.DataAccess.RollBackTrans
                i = i + 1
            Wend
    #End If
 
Exit Sub
 
EH:
 
End Sub
