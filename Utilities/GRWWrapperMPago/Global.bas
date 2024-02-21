Attribute VB_Name = "Global"

Public Const TIMEOUT_ORDER As Double = 60
Public Const PORCENT_TIMEOUT_WAIT_ORDER As Integer = 50
Public Const TIMEOUT_FOR_WAIT As Integer = 3

Public Function OPenResultSet(Who As Object, sSql As String) As rdoResultset '#NOERRORHANDLER
    On Error GoTo EH
    Dim cnn As Connection
    Set cnn = Who.DataAccess

    Set OPenResultSet = cnn.OPenResultSet(sSql)
    Set cnn = Nothing
Exit Function
 
EH:
    Set cnn = Nothing
End Function

