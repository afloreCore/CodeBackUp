VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "LaPos"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit


'Definición de códigos de resultado
'----------------------------------
Const VPI_OK = 0               ' Operacion exitosa
Const VPI_MORE_REC = 1         ' Operacion exitosa, pero faltan registros

Const VPI_FAIL = 11            ' El comando no pudo ser enviado
Const VPI_TIMEOUT_EXP = 12     ' Tiempo de espera agotado.

Const VPI_INVALID_ISSUER = 101 ' El código de tarjeta no existe.
Const VPI_INVALID_TICKET = 102 ' El número de cupón no existe.
Const VPI_INVALID_PLAN = 103   ' El código de plan no existe.
Const VPI_INVALID_INDEX = 104  ' No existe el indice
Const VPI_EMPTY_BATCH = 105    ' El lote del POS se encuentra vacío.

Const VPI_TRX_CANCELED = 201   ' Transacción cancelada por el usuario.
Const VPI_DIF_CARD = 202       ' La tarjeta deslizada por el usuario no coincide con la pedida.
Const VPI_INVALID_CARD = 203   ' La tarjeta deslizada no es válida.
Const VPI_EXPIRED_CARD = 204   ' La tarjeta deslizada está vencida.
Const VPI_INVALID_TRX = 205    ' La transacción original no existe.

Const VPI_ERR_COM = 301        ' El POS no pudo comunicarse con el host.
Const VPI_ERR_PRINT = 302      ' El POS no pudo imprimir el ticket.

Const VPI_INVALID_IN_CMD = 901     ' Nombre del comando inexistente.
Const VPI_INVALID_IN_PARAM = 902   ' Formato de algún parámetro de entrada no es correcto.
Const VPI_INVALID_OUT_CMD = 903    ' La respuesta enviada por el POS no es válida

Const VPI_GENERAL_FAIL = 909   ' Error general en la operación.


'Codigos de las operaciones
'--------------------------
Const VPI_PURCHASE = 1         ' Venta
Const VPI_VOID = 2             ' Anulación de venta
Const VPI_REFUND = 3           ' Devolución
Const VPI_REFUND_VOID = 4      ' Anulación de devolución
'Timeouts
'--------
Const VPI_TIMEOUT_STD = 3000       ' Timeout mínimo 3 segundos


'Defincion de tipos
'------------------
Private Type vpiComParams_t
    com As String
    BaudRate As Integer
    ByteSize As Integer
    Parity As Byte
    StopBits As Integer
End Type

Private Type vpiPurchaseIn_t
    amount As String          ' Monto *100
    receiptNumber As String   ' Número de factura
    instalmentCount As String ' Cant. de cuotas
    issuerCode As String      ' Código de tarjeta
    planCode As String        ' Código de plan
    tip As String             ' Propina *100
    merchantCode As String    ' Código de comercio
    merchantName As String    ' Razon social del comercio
    CUIT As String            ' CUIT del comercio
    linemode As Byte          ' Offline u Online
End Type

Private Type vpiVoidIn_t
    originalTicket As String  ' Número de cupón de trx. original
    issuerCode As String      ' Código de tarjeta
    merchantName As String    ' Razon social del comercio
    CUIT As String            ' CUIT del comercio
End Type

Private Type vpiRefundIn_t
     amount As String           ' Monto *100
     instalmentCount As String  ' Cant. de cuotas
     issuerCode As String       ' Código de tarjeta
     planCode As String         ' Código de plan
     originalTicket As String   ' Nro. ticket de la trx. original
     originalDate As String     ' Fecha de la trx. original
     receiptNumber As String    ' Número de factura
     merchantCode As String     ' Código de comercio
     merchantName As String     ' Razon social del comercio
     CUIT As String             ' CUIT del comercio
     linemode As Byte         ' Offline u Online
End Type

Private Type vpiTrxOut_t
    hostRespCode As String  ' Código de respuesta del host
    hostMessage As String   ' Mensaje de respuesta del host
    authCode As String      ' Número de autorización
    ticketNumber As String  ' Número de cupón
    batchNumber As String   ' Número de lote
    customerName As String  ' Nombre del tarjeta-habiente
    panLast4 As String      ' Ultimo 4 digitos de la tarjeta
    'MOD ELDAR
    panFirst6 As String     ' Primeros 6 digitos de la tarjeta
    'MOD ELDAR
    date As String          ' Fecha de la transacción
    time As String          ' Hora de la transaccion
    terminalID As String    ' Terminal ID
End Type

Private Type vpiBatchCloseOut_t
    hostRespCode As String  ' Código de respuesta del host
    date As String          ' Fecha de la transacción
    time As String          ' Hora de la transaccion
    terminalID As String    ' Terminal ID
End Type

Private Type vpiBatchCloseDataOut_t
    recIndex As Integer         ' Índice del registro.
    acquirerCode As String      ' Código de procesador.
    batchNumber As String       ' Número de lote.
    issuerCode As String        ' Código de tarjeta
    purchaseCount As String     ' Cantidad de ventas.
    purchaseAmount As String    ' Monto total de ventas.
    voidCount As String         ' Cantidad anulaciones de venta.
    voidAmount As String        ' Monto total de anulaciones.
    refundCount As String       ' Cantidad de devoluciones venta.
    refundAmount As String      ' Monto total de devoluciones.
    refvoidCount As String      ' Cantidad anulaciones devolución.
    refvoidAmount As String     ' Monto total anul. devolución.
    date As String              ' Fecha del ultimo cierre
    time As String              ' Hora del ultimo cierre
    terminalID As String        ' Terminal ID
End Type

Private Type vpiIssuerOut_t
    recIndex  As Integer       ' Índice del registro.
    acquirerCode As String     ' Código de procesador.
    issuerCode As String       ' Código de tarjeta
    issuerName As String       ' Nombre de la tarjeta
    maxInstCount As String     ' Maxima cantidad de cuotas
    terminalID As String       ' Terminal ID
End Type

Private Type vpiPlanOut_t
    recIndex  As Integer       ' Índice del registro.
    issuerCode  As String      ' Código de tarjeta
    planCode As String         ' Código de plan
    planLabel  As String       ' Nombre del plan
    terminalID As String       ' Terminal ID
End Type
'Declaración de funciones
'------------------------
Private Declare Function vpiOpenPort Lib "VpiPc.dll" (params As vpiComParams_t) As Integer

Private Declare Function vpiClosePort Lib "VpiPc.dll" () As Integer

Private Declare Function vpiTestConnection Lib "VpiPc.dll" () As Integer

Private Declare Function vpiPurchase Lib "VpiPc.dll" (paramIn As vpiPurchaseIn_t, paramOut As vpiTrxOut_t, TimeOut As Long) As Integer

Private Declare Function vpiVoid Lib "VpiPc.dll" (paramIn As vpiVoidIn_t, paramOut As vpiTrxOut_t, TimeOut As Long) As Integer

Private Declare Function vpiRefund Lib "VpiPc.dll" (paramIn As vpiRefundIn_t, paramOut As vpiTrxOut_t, TimeOut As Long) As Integer

Private Declare Function vpiRefundVoid Lib "VpiPc.dll" (paramIn As vpiVoidIn_t, paramOut As vpiTrxOut_t, TimeOut As Long) As Integer

Private Declare Function vpiBatchClose Lib "VpiPc.dll" (paramOut As vpiBatchCloseOut_t, TimeOut As Long) As Integer

Private Declare Function vpiGetLastTrxData Lib "VpiPc.dll" (ByRef trxCode As Integer, paramOut As vpiTrxOut_t) As Integer

Private Declare Function vpiGetBatchCloseData Lib "VpiPc.dll" (ByVal recIndex As Integer, paramOut As vpiBatchCloseDataOut_t) As Integer

Private Declare Function vpiPrintTicket Lib "VpiPc.dll" () As Integer

Private Declare Function vpiPrintBatchClose Lib "VpiPc.dll" () As Integer

Private Declare Function vpiGetIssuer Lib "VpiPc.dll" (ByVal recIndex As Integer, paramOut As vpiIssuerOut_t) As Integer

Private Declare Function vpiGetPlan Lib "VpiPc.dll" (ByVal recIndex As Integer, paramOut As vpiPlanOut_t) As Integer

Public Function EnviarPagoPOS(Who As Object, oRow As cwTManBZ.Row, oInternalSite As InternalSite, sTabla As String) As Integer
    If TypeOf who Is cwTManBZ.ObjectInstance Or TypeOf who Is cwTManBZ.IObjectHead Or TypeOf who Is cwTManBZ.Wizard Then LogObjAndVersion who, "LaPos.cls"            '#Parte->CL41034
    LogObjAndVersion Who, "LaPos.cls"            '#Parte->EFM26
    LogObjAndVersion Who, "LaPos.cls"            '#Parte->AF1905
    Dim bPortOpened As Boolean
    If oInternalSite Is Nothing Then '#1
        EnviarPagoPOS = STOP_IT
       'ERROR: El código de tarjeta es obligatorio
        Who.SetError "GR000351"
        Exit Function
    End If '#1
    EnviarPagoPOS = OpenPortPOS(Who, oInternalSite)
    If EnviarPagoPOS <> STOP_IT Then '#2
        bPortOpened = True
        EnviarPagoPOS = TestConnectionPOS(Who)
    End If '#2
    With oRow
        If EnviarPagoPOS <> STOP_IT Then '#3
            If Who.CurrentField = "CJRMVI_PAGADO" Then  '#4
                If sTabla = sSlpAsientoCjD Then '#5
                'COMPRAS --------------------->
                    EnviarPagoPOS = PurchasePOS(Who, oRow, oInternalSite)
                    If EnviarPagoPOS <> STOP_IT Then '#6
                        'Una vez enviado el pago, griso todo para que no pueda hacer mas modificaciones
                        .Fields("CJRMVI_PAGADO").Value = "S"
                        .Fields("CJRMVI_TIPCPT").ReadOnly = True
                        .Fields("CJRMVI_CODCPT").ReadOnly = True
                        .Fields("CJRMVI_CHEQUE").ReadOnly = True
                        .Fields("CJRMVI_IMPORT").ReadOnly = True
                        .Fields("CJRMVI_AUTPOS").ReadOnly = True
                        .Fields("CJRMVI_CUOTAS").ReadOnly = True
                        .Fields("CJRMVI_ONLINE").ReadOnly = True
                        .Fields("CJRMVI_PAGADO").ReadOnly = True
                        .Fields("VIRT_ANULAR").ReadOnly = False
                        .Fields("VIRT_ANULAR").Value = "N"
                    Else
                        .Fields("CJRMVI_PAGADO").Value = "N"
                        .Fields("CJRMVI_CUOTAS").ReadOnly = False
                        .Fields("CJRMVI_ONLINE").ReadOnly = False
                        .Fields("CJRMVI_PAGADO").ReadOnly = False
                        .Fields("VIRT_ANULAR").ReadOnly = True
                    End If '#6
                    'COMPRAS <----------------------
                Else
                    'DEVOLUCION
                    EnviarPagoPOS = DevolucionPOS(Who, oRow, oInternalSite)
                End If '#5
            ElseIf Who.CurrentField = "VIRT_ANULAR" Then
                'ANULACION
                EnviarPagoPOS = AnularPagoPOS(Who, oRow)
            End If '#4
        End If '#3
    End With
    If bPortOpened Then '#7
        EnviarPagoPOS = ClosePortPOS(Who, (EnviarPagoPOS <> STOP_IT))
    End If '#7
 End Function
Private Function OpenPortPOS(Who As Object, oInternalSite As InternalSite) As Integer 'vpiOpenPort
    Dim result As Integer
    Dim comParams As vpiComParams_t
 
On Error GoTo EH:
 
    With oInternalSite
        comParams.com = "COM" & Trim(.Puerto)
        comParams.BaudRate = .BaudRate
        comParams.ByteSize = .ByteSize
        comParams.Parity = Asc(.Parity)
        comParams.StopBits = .Parity
    End With
    result = vpiOpenPort(comParams)
    If result <> VPI_OK Then
        'ERROR: El POS respondió: Error abriendo el puerto
        Who.SetError "GR000352"
        OpenPortPOS = STOP_IT
    End If
Exit Function
EH:
    Who.SetError "GR000068", Err.Description
    OpenPortPOS = STOP_IT
End Function
Private Function PurchasePOS(Who As Object, oRow As cwTManBZ.Row, oInternal As InternalSite) As Integer 'vpiPurchase
'Envía los datos del pago
 
    Dim paramIn As vpiPurchaseIn_t
    Dim paramOut As vpiTrxOut_t
    Dim result As Integer
 
On Error GoTo EH:
        With oRow
            'Cargo los datos
            paramIn.amount = Round(.Fields("CJRMVI_IMPORT").Value * 100, 0)                 ' Importe * 100
            paramIn.receiptNumber = ""                                                      ' Numero de factura (no se envía)
            paramIn.instalmentCount = .Fields("CJRMVI_CUOTAS").Value                        ' Cantidad de cuotas
            paramIn.issuerCode = .Fields("CJRMVI_CODTAR").Value                             ' Código de tarjeta (Ej: 200 = VISA)
            paramIn.planCode = IIf(.Fields("CJRMVI_CUOTAS").Value > 1, 2, " ")              ' Código de plan
            paramIn.tip = "0"                                                               ' Propina (0 = Sin propina)
            paramIn.merchantCode = oInternal.Comercio                                       ' Código de comercio (MAX 15)
            paramIn.merchantName = oInternal.Nombre                                         ' Razon social del comercio (MAX 23)
            paramIn.CUIT = oInternal.CUIT                                                   ' CUIT del comercio (MAX 23)
            paramIn.linemode = IIf(.Fields("CJRMVI_ONLINE").Value = "S", 1, 0)              ' 1 = Online, 0 = Offline
        End With
 
        'Asigno suficiente memoria para la respuesta
        paramOut.hostRespCode = String(3, vbNull)
        paramOut.hostMessage = String(33, vbNull)
        paramOut.authCode = String(6, vbNull)
        paramOut.ticketNumber = String(8, vbNull)
        paramOut.batchNumber = String(4, vbNull)
        paramOut.customerName = String(26, vbNull)
        'Vuelven cruzados, el Last4 es First6 y el First6 es Last4
        paramOut.panLast4 = String(8, vbNull)   'First6
        paramOut.panFirst6 = String(8, vbNull)  'Last4
        paramOut.date = String(11, vbNull)
        paramOut.time = String(9, vbNull)
        paramOut.terminalID = String(9, vbNull)
 
        'Envío el comando
        result = vpiPurchase(paramIn, paramOut, 60)   'Timeout 60 segs
 
        If result = VPI_OK Then
            'Comunicación con POS completada en forma exitosa
            ProcesarRespuestaPurchase Who, oRow, paramOut
        Else
            PurchasePOS = STOP_IT
            Select Case result
                Case VPI_FAIL
                   'ERROR: El POS respondió: El comando no pudo ser enviado
                    Who.SetError "GR000357"
 
                Case VPI_TIMEOUT_EXP
                   'ERROR: El POS respondió: Tiempo de espera agotado. Verifique los datos cargados.
                    Who.SetError "GR000361"
 
                Case VPI_GENERAL_FAIL
                    'ERROR: El POS respondió: Error general en la operación
                    Who.SetError "GR000359"
 
                Case VPI_INVALID_PLAN
                    'ERROR: El POS respondió: El código de plan no existe
                    Who.SetError "GR000362"
 
                Case VPI_TRX_CANCELED
                    'ERROR: El POS respondió: Transacción cancelada por el usuario
                    Who.SetError "GR000363"
 
                Case VPI_DIF_CARD
                    'ERROR: El POS respondió: La tarjeta deslizada por el usuario no coincide con la pedida (Se envió ~1)
                    Who.SetError "GR000364", oRow.Fields("CJRMVI_CODTAR").Value
 
                Case VPI_INVALID_CARD
                    'ERROR: El POS respondió: La tarjeta deslizada no es válida
                    Who.SetError "GR000365"
 
                Case VPI_EXPIRED_CARD
                    'ERROR: El POS respondió: La tarjeta deslizada está vencida
                    Who.SetError "GR000366"
 
                Case VPI_ERR_COM
                    'ERROR: El POS respondió: El POS no pudo comunicarse con el host
                    Who.SetError "GR000367"
 
                Case VPI_ERR_PRINT
                    'ERROR: El POS respondió: El POS no pudo imprimir el ticket
                    Who.SetError "GR000368"
 
                Case VPI_INVALID_IN_CMD
                    'ERROR: El POS respondió: Nombre del comando inexistente
                    Who.SetError "GR000369"
 
                Case VPI_INVALID_IN_PARAM
                    'ERROR: El POS respondió: Formato de algún parámetro de entrada no es correcto
                    Who.SetError "GR000370"
 
                Case VPI_INVALID_OUT_CMD
                    'ERROR: El POS respondió: La respuesta enviada por el POS no es válida
                    Who.SetError "GR000371"
            End Select
        End If
 
    Exit Function
EH:
    'ERROR: Error ralizando el pago: ~1
    PurchasePOS = STOP_IT
    Who.SetError "GR000375", Err.Description
 End Function
Private Function ProcesarRespuestaPurchase(Who As Object, oRow As cwTManBZ.Row, paramOut As vpiTrxOut_t) As Integer
 
    With oRow
        .Fields("CJRMVI_AUTPOS").Value = Trim(paramOut.authCode)
        .Fields("CJRMVI_CHEQUE").Value = Trim(paramOut.ticketNumber)
        .Fields("CJRMVI_NROLOT").Value = Trim(paramOut.batchNumber)
        .Fields("CJRMVI_DOCFIR").Value = Trim(paramOut.customerName)
        'Vuelven cruzados, el Last4 es First6 y el First6 es Last4
        .Fields("CJRMVI_PRITAR").Value = Trim(paramOut.panLast4)      'First6
        .Fields("CJRMVI_ULTTAR").Value = Trim(paramOut.panFirst6)     'Last4
        .Fields("CJRMVI_TERMID").Value = Left(paramOut.terminalID, 8)
    End With
 
 End Function
Private Function ClosePortPOS(Who As Object, Optional bSetError As Boolean = True) As Integer  'vpiClosePort
'Cierra el puerto serial
 
    Dim result As Integer
 
On Error GoTo EH:
 
        result = vpiClosePort()
        If result <> VPI_OK And bSetError Then 'En los casos que ya existe un error previo, no quiero que este setee uno nuevo
            'ERROR: El POS respondió: Error cerrando el puerto
            Who.SetError "GR000355"
            ClosePortPOS = STOP_IT
        End If
    Exit Function
EH:
    'ERROR: Error cerrando el puerto: ~1
    Who.SetError "GR000356", Err.Description
    ClosePortPOS = STOP_IT
End Function
 
Private Function AnularPagoPOS(Who As Object, oRow As cwTManBZ.Row) As Integer
 
    With oRow
        AnularPagoPOS = VoidPOS(Who, oRow)
 
        If AnularPagoPOS <> STOP_IT Then
            .Fields("CJRMVI_PAGADO").Value = "N"
            .Fields("VIRT_ANULAR").Value = "S"
            .Fields("VIRT_ANULAR").ReadOnly = True
            .Fields("CJRMVI_TIPCPT").ReadOnly = False
            .Fields("CJRMVI_CODCPT").ReadOnly = False
            .Fields("CJRMVI_CHEQUE").ReadOnly = False
            .Fields("CJRMVI_IMPORT").ReadOnly = False
            .Fields("CJRMVI_AUTPOS").ReadOnly = False
            .Fields("CJRMVI_CUOTAS").ReadOnly = False
            .Fields("CJRMVI_ONLINE").ReadOnly = False
            .Fields("CJRMVI_PAGADO").ReadOnly = False
        Else
            .Fields("VIRT_ANULAR").Value = "N"
        End If
    End With
End Function
Private Function VoidPOS(Who As Object, oRow As cwTManBZ.Row) As Integer  'vpiVoid
'Anula un pago en función a un nro de ticket
 
    Dim result As Integer
    Dim paramIn As vpiVoidIn_t
    Dim paramOut As vpiTrxOut_t
 
On Error GoTo EH:
    With oRow
        paramIn.originalTicket = .Fields("CJRMVI_CHEQUE").Value
        paramIn.issuerCode = .Fields("CJRMVI_CODTAR").Value
    End With
 
    'Asigno suficiente memoria para la respuesta
    paramOut.hostRespCode = String(3, vbNull)
    paramOut.hostMessage = String(33, vbNull)
    paramOut.authCode = String(6, vbNull)
    paramOut.ticketNumber = String(8, vbNull)
    paramOut.batchNumber = String(4, vbNull)
    paramOut.customerName = String(26, vbNull)
    'Vuelven cruzados, el Last4 es First6 y el First6 es Last4
    paramOut.panLast4 = String(8, vbNull)   'First6
    paramOut.panFirst6 = String(8, vbNull)  'Last4
    paramOut.date = String(11, vbNull)
    paramOut.time = String(9, vbNull)
    paramOut.terminalID = String(9, vbNull)
 
    result = vpiVoid(paramIn, paramOut, 60)
 
    If result = VPI_OK Then
        LimpioCamposPurchase Who, oRow, paramOut
    Else
        VoidPOS = STOP_IT
        Select Case result
            Case VPI_FAIL
                'ERROR: El POS respondió: El comando no pudo ser enviado
                 Who.SetError "GR000357"
 
            Case VPI_TIMEOUT_EXP
                'ERROR: El POS respondió: Tiempo de espera agotado
                Who.SetError "GR000358"
 
            Case VPI_GENERAL_FAIL
                'ERROR: El POS respondió: Error general en la operación
                Who.SetError "GR000359"
 
            Case VPI_TRX_CANCELED
                'ERROR: El POS respondió: Transacción cancelada por el usuario
                Who.SetError "GR000363"
 
            Case VPI_DIF_CARD
                'ERROR: El POS respondió: La tarjeta deslizada por el usuario no coincide con la pedida (Se envió ~1)
                 Who.SetError "GR000364", oRow.Fields("CJRMVI_CODTAR").Value
 
             Case VPI_INVALID_CARD
                 'ERROR: El POS respondió: La tarjeta deslizada no es válida
                 Who.SetError "GR000365"
 
            Case VPI_EXPIRED_CARD
                'ERROR: El POS respondió: La tarjeta deslizada está vencida
                Who.SetError "GR000366"
 
            Case VPI_ERR_COM
                'ERROR: El POS respondió: El POS no pudo comunicarse con el host
                Who.SetError "GR000367"
 
            Case VPI_ERR_PRINT
                'ERROR: El POS respondió: El POS no pudo imprimir el ticket
                Who.SetError "GR000368"
 
            Case VPI_INVALID_IN_CMD
                'ERROR: El POS respondió: Nombre del comando inexistente
                Who.SetError "GR000369"
 
            Case VPI_INVALID_IN_PARAM
                'ERROR: El POS respondió: Formato de algún parámetro de entrada no es correcto
                Who.SetError "GR000370"
 
            Case VPI_INVALID_OUT_CMD
                'ERROR: El POS respondió: La respuesta enviada por el POS no es válida
                Who.SetError "GR000371"
 
            Case VPI_INVALID_TICKET
                'ERROR: El POS respondió: El número de cupón no existe en el lote del POS
                Who.SetError "GR000372"
 
            Case VPI_EMPTY_BATCH
                'ERROR: El POS respondió: El lote se encuentra vacío
                Who.SetError "GR000373"
        End Select
    End If
 
Exit Function
 
EH:
    VoidPOS = STOP_IT
    'ERROR: Error ralizando la anulación: ~1
    Who.SetError "GR000376", Err.Description
 End Function
Private Function LimpioCamposPurchase(Who As Object, oRow As cwTManBZ.Row, paramOut As vpiTrxOut_t) As Integer
 
   With oRow
        'En principio no se realiza la grabación del Log
        'DeleteFromGRLPOS who, .Fields("CJRMVI_TERMID").SQLValue, .Fields("CJRMVI_CHEQUE").SQLValue
 
       .Fields("CJRMVI_AUTPOS").Value = vbNullString
       .Fields("CJRMVI_AUTPOS").Required = True
       .Fields("CJRMVI_AUTPOS").ValidationState = ""
       .Fields("CJRMVI_CHEQUE").Value = vbNullString
       .Fields("CJRMVI_CHEQUE").Required = True
       .Fields("CJRMVI_CHEQUE").ValidationState = ""
       .Fields("CJRMVI_NROLOT").Value = 0
       .Fields("CJRMVI_DOCFIR").Value = vbNullString
       .Fields("CJRMVI_PRITAR").Value = 0
       .Fields("CJRMVI_ULTTAR").Value = 0
       .Fields("CJRMVI_TERMID").Value = vbNullString
       .Fields("CJRMVI_CODTAR").Value = vbNullString
    End With
End Function
Private Function RefundPOS(Who As Object, oRow As cwTManBZ.Row, oInternal As InternalSite) As Integer  'vpiRefund
'Hace una devolución de un pago en función a un nro de ticket (puede no ser del mismo lote)
 
    Dim result As Integer
    Dim paramIn As vpiRefundIn_t
    Dim paramOut As vpiTrxOut_t
 
On Error GoTo EH:
    With oRow
        'Cargo los datos
        paramIn.amount = Round(.Fields("CJRMVI_IMPORT").Value * 100, 0)                     ' Importe * 100
        paramIn.instalmentCount = .Fields("CJRMVI_CUOTAS").Value                            ' Cantidad de cuotas
        paramIn.issuerCode = .Fields("CJRMVI_CODTAR").Value                                 ' Código de tarjeta (Ej: 200 = VISA)
        paramIn.originalDate = Fechas.ConvertirStringAFecha(.Fields("CJRMVI_FCHAUX").Value) ' Fecha de emisión
        paramIn.originalTicket = .Fields("CJRMVI_CHEQUE").Value                             ' Ticket original
        paramIn.planCode = IIf(.Fields("CJRMVI_CUOTAS").Value > 1, 2, " ")                  ' Código de plan
        paramIn.receiptNumber = ""                                                          ' Numero de factura (no se envía)
        paramIn.merchantCode = oInternal.Comercio                                           ' Código de comercio (MAX 15)
        paramIn.merchantName = oInternal.Nombre                                             ' Razon social del comercio (MAX 23)
        paramIn.CUIT = oInternal.CUIT                                                       ' CUIT del comercio (MAX 23)
        paramIn.linemode = IIf(.Fields("CJRMVI_ONLINE").Value = "S", 1, 0)                  ' 1 = Online, 0 = Offline
    End With
 
    'Asigno suficiente memoria para la respuesta
    paramOut.hostRespCode = String(3, vbNull)
    paramOut.hostMessage = String(33, vbNull)
    paramOut.authCode = String(6, vbNull)
    paramOut.ticketNumber = String(8, vbNull)
    paramOut.batchNumber = String(4, vbNull)
    paramOut.customerName = String(26, vbNull)
    'Vuelven cruzados, el Last4 es First6 y el First6 es Last4
    paramOut.panLast4 = String(8, vbNull)   'First6
    paramOut.panFirst6 = String(8, vbNull)  'Last4
    paramOut.date = String(11, vbNull)
    paramOut.time = String(9, vbNull)
    paramOut.terminalID = String(9, vbNull)
 
    result = vpiRefund(paramIn, paramOut, 60)
 
    If result = VPI_OK Then
        'Comunicación con POS completada en forma exitosa
        ProcesarRespuestaPurchase Who, oRow, paramOut
    Else
        RefundPOS = STOP_IT
        Select Case result
            Case VPI_FAIL
                'ERROR: El POS respondió: El comando no pudo ser enviado
                 Who.SetError "GR000357"
 
            Case VPI_TIMEOUT_EXP
                'ERROR: El POS respondió: Tiempo de espera agotado. Verifique los datos cargados.
                Who.SetError "GR000361"
 
            Case VPI_GENERAL_FAIL
                'ERROR: El POS respondió: Error general en la operación
                Who.SetError "GR000359"
 
            Case VPI_TRX_CANCELED
                'ERROR: El POS respondió: Transacción cancelada por el usuario
                Who.SetError "GR000363"
 
            Case VPI_DIF_CARD
                'ERROR: El POS respondió: La tarjeta deslizada por el usuario no coincide con la pedida (Se envió ~1)
                 Who.SetError "GR000364", oRow.Fields("CJRMVI_CODTAR").Value
 
            Case VPI_INVALID_CARD
                 'ERROR: El POS respondió: La tarjeta deslizada no es válida
                 Who.SetError "GR000365"
 
            Case VPI_EXPIRED_CARD
                'ERROR: El POS respondió: La tarjeta deslizada está vencida
                Who.SetError "GR000366"
 
            Case VPI_ERR_COM
                'ERROR: El POS respondió: El POS no pudo comunicarse con el host
                Who.SetError "GR000367"
 
            Case VPI_INVALID_IN_CMD
                'ERROR: El POS respondió: Nombre del comando inexistente
                Who.SetError "GR000369"
 
            Case VPI_INVALID_IN_PARAM
                'ERROR: El POS respondió: Formato de algún parámetro de entrada no es correcto
                Who.SetError "GR000370"
 
            Case VPI_INVALID_OUT_CMD
                'ERROR: El POS respondió: La respuesta enviada por el POS no es válida
                Who.SetError "GR000371"
 
            Case VPI_INVALID_PLAN
                    'ERROR: El POS respondió: El código de plan no existe
                    Who.SetError "GR000362"
 
            Case VPI_INVALID_TRX
                'ERROR: El POS respondió: La transacción original no existe
                Who.SetError "GR000374"
        End Select
    End If
 
Exit Function
 
EH:
    RefundPOS = STOP_IT
    'ERROR: Error ralizando la devolución: ~1
    Who.SetError "GR000377", Err.Description
End Function
Private Function DevolucionPOS(Who As Object, oRow As cwTManBZ.Row, oInternalSite As InternalSite) As Integer
    With oRow
        DevolucionPOS = RefundPOS(Who, oRow, oInternalSite)
        If DevolucionPOS <> STOP_IT Then
            .Fields("CJRMVI_AUTPOS").ReadOnly = True
            .Fields("CJRMVI_PAGADO").ReadOnly = True
            .Fields("CJRMVI_PAGADO").Value = "S"
        Else
            .Fields("CJRMVI_PAGADO").Value = "N"
        End If
    End With
End Function
Private Function TestConnectionPOS(Who As Object) As Integer 'vpiTestConnection
'Verifica la conexión con el POS
 
    Dim result As Integer
 
On Error GoTo EH:
 
        result = vpiTestConnection
        Select Case result 'VPI_OK(0) = OK
            Case VPI_FAIL
                'ERROR: El POS respondió: El comando no pudo ser enviado
                Who.SetError "GR000357"
                TestConnectionPOS = STOP_IT
 
            Case VPI_TIMEOUT_EXP
                'ERROR: El POS respondió: Tiempo de espera agotado
                Who.SetError "GR000358"
                TestConnectionPOS = STOP_IT
 
            Case VPI_GENERAL_FAIL
                'ERROR: El POS respondió: Error general en la operación
                 Who.SetError "GR000359"
                 TestConnectionPOS = STOP_IT
        End Select
 
    Exit Function
EH:
    'ERROR: Error probando la conexión: ~1
    Who.SetError "GR000360", Err.Description
    TestConnectionPOS = STOP_IT
End Function
'#Parte->AF1905 ->
Private Sub Class_Initialize()
    #If VB5 = 1 Then        '#Parte->AF1905
        LogVersion
    #End If        '#Parte->AF1905
End Sub
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
