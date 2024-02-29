VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Utilities"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
'***********************************************************************************************************************
'Fecha   |Us|Proyecto|Programa  |Descripción
'***********************************************************************************************************************
'27/10/2023|AFLORE|AFLORE-46|Integración con mercado pago
Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)
Option Explicit
Implements ICallbackInterop
Private oMPApi As Object
Private oMPCallback As Object
Private oGrwCompany As GRWMultiCompany.Utilities
Private oMPConfig As InternalMPConfig
Private oQuerys As Querys
Private oIndexConcepto As GRWIndexKey.IndexKey
Private Const sTableHeader As String = "FCRMVH"
Private Const sSlpAsientoCjD As String = "FCRMVI08"    'Solapa Bebe de tesoreria FCRMVI08
Private Const sSlpAsientoCjH As String = "FCRMVI09"   'Solapa Haber de tesoreria FCRMVI09
Private Const FIELD_STORE As String = "CJRMVI_STORID"
Private Const FIELD_BRANCH As String = "CJRMVI_BRANID"
Private Const ROUT_CONTROLLER As String = "api/notification"
Private Const SEPARATOR As String = "|"
Private m_CompanyName As String
Private m_Sucurs As String
'
Public Enum eTopic
    None
    order
    payment
End Enum
Private Type tpTopic
    topic As eTopic
    paymentId As String
    orderId As String
End Type
Private Type tpConfigMP
    CodEmp As String
    Sucurs As String
    StoreID As String
    BranID As String
End Type
'
Private m_ConfigMP As tpConfigMP
Private m_tpTopic As tpTopic
'*************************************************************************************************
'                              llamada desde OnEvent
'*************************************************************************************************

Public Function OnEvent(iEvent As evEvents, Who As cwTManBZ.ObjectInstance) As Integer
    Select Case iEvent
        Case VALIDATE_FIELD
            OnEvent = ValidateFields(Who)
    End Select
End Function

'*************************************************************************************************
'                                   Private Methods
'*************************************************************************************************
Private Function ValidateFields(Who As cwTManBZ.ObjectInstance) As Integer
    Dim currentTable As String
    currentTable = Who.currentTable
    '
    Select Case currentTable
        Case sSlpAsientoCjD, sSlpAsientoCjH
            ValidateFields = ValidateFieldsItem(Who)
        Case sTableHeader
            ValidateFields = ValidateHeader(Who)
    End Select
    
End Function
Private Function ValidateHeader(Who As cwTManBZ.ObjectInstance) As Integer
    Dim currentField As String
    currentField = Who.currentField
    
    Select Case currentField
        Case sTableHeader & "_NROCTA", sTableHeader & "_FCHMOV"
            ValidateHeader = Initialize(Who)
    End Select
End Function
Private Function ValidateNROCTA(Who As cwTManBZ.ObjectInstance) As Integer
    Dim ModFor As String
    Dim CodFor As String
    Dim Sucurs As String
    
    With Who.Table.Rows(1)
        ModFor = .Fields(sTableHeader & "_MODFOR").Value
        CodFor = .Fields(sTableHeader & "_CODFOR").Value
        Sucurs = .Fields(sTableHeader & "_SUCURS").Value
    End With

End Function

Private Function ValidateFieldsItem(Who As cwTManBZ.ObjectInstance) As Integer
    Dim currentField As String
    Dim currentTable As String
    Dim oRow As cwTManBZ.Row
    
    currentField = Who.currentField
    currentTable = Who.currentTable
    '
    If currentTable = sSlpAsientoCjD Then
        Set oRow = GetCurrentRow(Who.Table.Rows(1), sSlpAsientoCjD)
    ElseIf currentTable = sSlpAsientoCjH Then
        Set oRow = GetCurrentRow(Who.Table.Rows(1), sSlpAsientoCjH)
    End If
    Select Case currentField
        Case "CJRMVI_IMPORT", "CJRMVI_CUOTAS"
            ValidateIMPORT Who, oRow
        Case "CJRMVI_PAGADO"
            ValidateFieldsItem = ValidatePAGADO(Who, oRow)
    End Select
    
End Function
Private Function ValidateIMPORT(Who As cwTManBZ.ObjectInstance, oRow As cwTManBZ.Row)
    If oRow.Fields("CJRMVI_IMPORT").Value > 0 Then '#1
        If ExistConceptoMP(Who, oRow) Then '#2
            oRow.Fields("CJRMVI_PAGADO").ReadOnly = False
        Else
            oRow.Fields("CJRMVI_PAGADO").ReadOnly = True
        End If '#2
    End If '#1

End Function
Private Function ValidatePAGADO(Who As cwTManBZ.ObjectInstance, oRow As cwTManBZ.Row) As Integer
    Dim oInternal As InternalMPClass
    Dim oBranch As InternalBranch
    Dim oInternalPay As InternalPayment
    Dim oPayment As Object
    Dim oOrder As Object
    Dim reference As String
    Dim FchMov As Date
    Dim FchMovISO8601 As String
    Dim TimeOut As Long
    Dim topicId As String
    Dim result As Boolean
    Dim notificationUrl As String
    On Error GoTo EH

    If oRow.Fields("CJRMVI_PAGADO").Value = "S" Then '#1
        SetValueaAndReadOnlyPOSFields oRow, Nothing
        If ExistConceptoMP(Who, oRow) Then '#2
            With oRow
                Set oInternal = oMPConfig.Item(m_ConfigMP.StoreID)
                If Not oInternal Is Nothing Then '#3
                    'public bool Initialize(string HttpsHost, string queryParameters = "")
                    'queryParameters: Debe ir la clave clientId=(identificador para esta caja). Puede ser en BRANID que es único
                    result = oMPCallback.Initialize(oInternal.EndPoint & "/" & ROUT_CONTROLLER, "clientId=" & CStr(m_ConfigMP.BranID))
                    If result Then '#4
                        With oInternal
                            'Set endpoint para el body de la orden, ya que debe enviar el id de la caja para que SignalR lo pueda direccionar a cada cliente
                            'https://{publicUrl}/api/notification?clientId=xxxxxxx
                            notificationUrl = oInternal.EndPoint & "/" & ROUT_CONTROLLER & "?clientId=" & CStr(m_ConfigMP.BranID)
                            Set oBranch = .Branchs.Item(m_ConfigMP.BranID)
                            TimeOut = IIf(oBranch.TimeOut < TIMEOUT_ORDER, TIMEOUT_ORDER, oBranch.TimeOut)
                            FchMov = DateAdd("s", TimeOut, Now)
                            reference = GetKeyExternalRef(oRow, FchMov)
                            '
                            ValidatePAGADO = SendOrder(.ExtStoreID, oBranch.external_id, CDbl(oRow.Fields("CJRMVI_IMPORT").Value), oBranch.itemdesc, oBranch.itemunit, FchMov, _
                            notificationUrl, ExternalReference:=reference)
                            If ValidatePAGADO <> STOP_IT Then '#5
                                ValidatePAGADO = ValidateOrder(CInt(TimeOut), reference)
                                If ValidatePAGADO = STOP_IT Then '#6
                                    'GR000489      La orden de mercado pago para la caja ~1, no se encuentra generada o ha expirado
                                    Who.SetError "GR000489", m_ConfigMP.BranID
                                End If '#6
                            End If '#5
                            If ValidatePAGADO <> STOP_IT Then '#7
                                ValidatePAGADO = ValidatePayment(Who, oBranch.external_id, reference, CDbl(oRow.Fields("CJRMVI_IMPORT").Value), TimeOut, oInternalPay)
                            End If '#7
                            If Not (ValidatePAGADO = STOP_IT Or oInternalPay Is Nothing) Then '#8
                                SetValueaAndReadOnlyPOSFields oRow, oInternalPay, True
                                'toque aflore para DEMO, luego SACAR
                                Who.SetWarning "Operation ID: " & oRow.Fields("CJRMVI_CHECOM").Value
                                '''''''''''''''''''''''''''''''''''
                            End If '#8
                           
                        End With
                    Else
                        Who.SetError "GR000068", "Error al inicializar callback"
                        ValidatePAGADO = STOP_IT
                    End If '#4
                Else '#3
                    'GR000494      No se encontró la configuración para la tienda ~1 de mercado pago
                    Who.SetError "GR000494", m_ConfigMP.BranID
                    ValidatePAGADO = STOP_IT
                End If '#3
            End With
        Else '#2
            'GR000495      El concepto de tesorería seleccionado o su cuenta relacionada, no posee una caja de mercado pago asignada
            Who.SetError "GR000495"
            ValidatePAGADO = STOP_IT
        End If '#2
    End If '#1
    Set oInternal = Nothing
    Set oInternalPay = Nothing
    Set oBranch = Nothing
    Set oOrder = Nothing
    Set oPayment = Nothing
   'Set oMPApi = Nothing
Exit Function
EH:
    Set oMPApi = Nothing
    Set oInternal = Nothing
    Set oInternalPay = Nothing
    Set oBranch = Nothing
    Set oOrder = Nothing
    Set oPayment = Nothing
    ValidatePAGADO = STOP_IT
    Who.SetError "GR000068", Err.Description
End Function
Private Function GetInternalClass(StoreID As Long) As InternalMPClass
    Set GetInternalClass = oMPConfig.Item(StoreID)
End Function
Private Function ValidateOrder(ByRef TimeOut As Integer, ByVal externalRef As String) As Integer
    Dim oOrder As Object
    Dim topicId As String
    Dim TimeStart As Currency
    Dim TimeStop As Currency
    Dim TimePassed As Currency
    Dim TimeWait As Currency
    Dim continue As Boolean
    TimeWait = TimeOut
    TimeStart = Timer()
    TimePassed = 0
    continue = True
    On Error GoTo EH
    'Inicializar m_tpTopic para que no quede el topic anterior
    Do While continue And (TimePassed < TimeWait)
        m_tpTopic.topic = None
        m_tpTopic.orderId = ""
        'Esperar respuesta desde el servidor de MPago
        ValidateOrder = WaitForTopicId(order, CInt(TimeWait - TimePassed), topicId)
        If ValidateOrder <> STOP_IT Then '#1
            Set oOrder = GetOrderFromId(topicId)
        End If '#1
        If Not oOrder Is Nothing Then '#2
            If oOrder.status = InterfaceComNetCore.PaymentStatus_approved And Trim(CStr(oOrder.external_reference)) = externalRef Then '#3
                continue = False
            End If '#3
        End If '#2
        Sleep 1000
        TimeStop = Timer()
        TimePassed = TimeStop - TimeStart
        DoEvents
    Loop
    TimeOut = TimeOut - TimePassed
    ValidateOrder = IIf(continue, STOP_IT, GO_ON)
    Set oOrder = Nothing
Exit Function
EH:
    Set oOrder = Nothing
    ValidateOrder = STOP_IT
End Function
Private Function ValidatePayment(Who As cwTManBZ.ObjectInstance, ExtBrandID, reference As String, amount As Double, TimeOut As Long, ByRef oInternalPay As InternalPayment) As Integer  '#NOERRORHANDLING
On Error GoTo EH
    Dim topicId As String
    Dim TimeStart As Currency
    Dim TimeStop As Currency
    Dim TimePassed As Currency
    Dim TimeWait As Currency
    Dim continue As Boolean
    Dim FechaReturn As Date
    Dim oPayment As Object
    Dim oOrder As Object
    Dim oInternalPayCol As InternalPayments
    Dim timeOutAux As Long
    Static Paso As Boolean
    'Clase devuelta:PaymentRequestMarshal
'    public double id { get; set; }
'    public string authorization_code { get; set; } = string.Empty;
'    public bool binary_mode { get; set; }
'    public DateTime date_created { get; set; }
'    public DateTime date_approved { get; set; }
'    public DateTime date_last_updated { get; set; }
'    public DateTime money_release_date { get; set; }
'    public string payment_method_id { get; set; } = string.Empty; //master
'    public string payment_type_id { get; set; } = string.Empty;
'    public string status { get; set; } = string.Empty;
'    public string status_detail { get; set; } = string.Empty;
'    public string currency_id { get; set; } = string.Empty;
'    public string description { get; set; } = string.Empty;
'    public double collector_id { get; set; }
'    public double transaction_amount { get; set; }
'    public double transaction_amount_refunded { get; set; }
'    public double coupon_amount { get; set; }
'    public double total_paid_amount { get; set; } //from transaction_details
'    public double net_received_amount { get; set; } //from transaction_details
'    public int installments { get; set; }
'    public string external_reference { get; set; } = string.Empty;
    'Set oOrder = GetOrder(ExtBrandID)
    'If Not oOrder Is Nothing Then '#1
    ' + (TimeOut * PORCENT_TIMEOUT_WAIT_ORDER / 100)
    
    'Buscar payment desde el servidor
     
    TimeWait = TimeOut
    TimeStart = Timer()
    TimePassed = 0
    continue = True
    'Inicializar m_tpTopic para que no quede el topic anterior
    Do While continue And (TimePassed < TimeWait)
        m_tpTopic.topic = ""
        m_tpTopic.paymentId = ""
        'Esperar respuesta desde el servidor de MPago
        ValidatePayment = WaitForTopicId(payment, TimeWait - TimePassed, topicId)
        If ValidatePayment <> STOP_IT Then '#1
            Set oPayment = GetPaymentFromId(topicId)
        End If '#1
        If Not oPayment Is Nothing Then '#2
            If oPayment.PaymentStatus = InterfaceComNetCore.PaymentStatus_approved Then '#3
                continue = False
            End If '#3
        End If '#2
        Sleep 1000
        TimeStop = Timer()
        TimePassed = TimeStop - TimeStart
        DoEvents
    Loop

    If Not oPayment Is Nothing Then  '#3
        Set oInternalPayCol = New InternalPayments
        Set oInternalPay = oInternalPayCol.Add(oPayment)
        With oInternalPay
            If Trim(reference) <> Trim(.ExternalReference) Then '#4
                'GR000490      La referencia externa generada para mercado pago no coincide con el pago encontrado. Referencia encontrada ~1
                Who.SetError "GR000490", Trim(CStr(.ExternalReference))
                ValidatePayment = STOP_IT
            ElseIf amount <> .TotalAmount Then
                'GR000491      El importe total de la orden de mercado pago no coincide con el importe del  pago. Importe del pago ~1
                Who.SetError "GR000491", .TotalAmount
                ValidatePayment = STOP_IT
            ElseIf .PaymentStatus <> InterfaceComNetCore.PaymentStatus.PaymentStatus_approved Then
                'GR000492      El estado del pago de mercado pago no se encuetra aprovado. Estado ~1
                Who.SetError "GR000492", GetPaymentStatusStr(.PaymentStatus)
                ValidatePayment = STOP_IT
            End If '#4
        End With
    Else '#3
        'GR000493      No se enontró el pago de mercado pago para la caja ~1
        Who.SetError "GR000493", ExtBrandID
        ValidatePayment = STOP_IT
    End If '#3

    Set oPayment = Nothing
    Set oInternalPayCol = Nothing
    Set oOrder = Nothing
    Exit Function
EH:
    Set oOrder = Nothing
    Set oPayment = Nothing
    Set oInternalPayCol = Nothing
    ValidatePayment = STOP_IT
    Who.SetError "GR000068", Err.Description
End Function
Private Function WaitAndGetPayment(StoreID As String, BranID As String, externalBranId As String, reference As String, TimeOut As Long) As Object

On Error GoTo EH
    Dim TimeStart As Currency
    Dim TimeStop As Currency
    Dim TimePassed As Currency
    Dim TimeWait As Currency
    Dim timeOutAux As Currency
    Dim VbResult As Integer
    Dim continue As Boolean
    Dim Found As Boolean
    Dim oPayment As Object
    Dim oOrder As Object
    Dim payStatus As InterfaceComNetCore.PaymentStatus
    payStatus = PaymentStatus_None
    continue = True
    
    'timeOutAux = Round(TimeOut * (PORCENT_TIMEOUT_WAIT_ORDER / 100), 0)
    TimeWait = TimeOut
    TimeStart = Timer()
    TimePassed = 0
    Do While (TimePassed < TimeWait) And continue
        Set oOrder = GetOrder(externalBranId)
        If oOrder Is Nothing Then
            Sleep 1000
            Set oPayment = GetPayment(StoreID, BranID, reference)
            If Not oPayment Is Nothing Then
                payStatus = oPayment.status
                If payStatus <> PaymentStatus_None Then
                    Found = True
                    continue = False
                End If
            End If
        Else
             Sleep 1000
        End If
        TimeStop = Timer()
        TimePassed = TimeStop - TimeStart
    Loop
'    Do While (TimePassed < TimeWait) And continue
'        Set oOrder = GetOrder(externalBranId)
'        If oOrder Is Nothing Then
'            continue = False
'        Else
'            Sleep 1000
'        End If
'        TimeStop = Timer()
'        TimePassed = TimeStop - TimeStart
'    Loop
    'Payment
    TimeWait = (TimeOut - TimePassed)
    If TimeWait > 0 And (payStatus <> PaymentStatus_approved And payStatus <> PaymentStatus_None) Then
        continue = True
        timeOutAux = (TimeOut * PORCENT_TIMEOUT_WAIT_ORDER / 100)
        If TimeWait < timeOutAux Then
            TimeWait = timeOutAux
        End If
        '
        TimeStart = Timer()
        TimePassed = 0
        Do While (TimePassed < TimeWait) And continue
            Set oPayment = GetPayment(StoreID, BranID, reference)
            If Not oPayment Is Nothing Then
                If payStatus <> oPayment.status Then
                    Found = True
                    continue = False
                End If
            Else
                 Sleep 1000
            End If
            TimeStop = Timer()
            TimePassed = TimeStop - TimeStart
        Loop
    End If
    '
    If Not Found Then
        VbResult = cwfc.MsgBox("¿Esperar?", vbQuestion + vbYesNo + vbDefaultButton2)
        If VbResult = vbYes Then WaitAndGetPayment StoreID, BranID, externalBranId, reference, TimeOut * (PORCENT_TIMEOUT_WAIT_ORDER / 100)
    End If
    Set WaitAndGetPayment = oPayment
    Set oPayment = Nothing
Exit Function
EH:
    Set oPayment = Nothing
End Function
Private Sub WaitTimer(seconds As Long)
    Dim TimeStart As Currency
    Dim TimeStop As Currency
    Dim TimePassed As Currency
    Dim TimeWait As Currency
    
    'use this block where you need a pause
    TimeWait = seconds
    TimeStart = Timer()
    TimePassed = 0
    Do While TimePassed < TimeWait  'seconds
        TimeStop = Timer()
        TimePassed = TimeStop - TimeStart
        DoEvents
    Loop
End Sub
Private Sub SetValueaAndReadOnlyPOSFields(oRow As cwTManBZ.Row, oInternalPay As InternalPayment, Optional ReadOnly As Boolean = False)
    Dim ePaymentType As InterfaceComNetCore.PaymentType
    oRow.Fields("CJRMVI_CHECOM").Value = ""
    oRow.Fields("CJRMVI_AUTPOS").Value = ""
    oRow.Fields("CJRMVI_PAGADO").ReadOnly = ReadOnly
    oRow.Fields("CJRMVI_AUTPOS").ReadOnly = ReadOnly
    oRow.Fields("CJRMVI_CUOTAS").ReadOnly = ReadOnly
'    Código de autorización tarjeta (CJRMVI_AUTPOS) -> Nro de autorizacion que devuelve lapos
'    Número de cheque/doc (CJRMVI_CHEQUE)           -> numero de cupon que devuelve lapos
'    Firmante del documento (CJRMVI_DOCFIR) -       -> DNI de la persona
'    Número de lote (CJRMVI_NROLOT)                 -> lapos indica cual es el lote abierto en el momento en que se presenta el cupon
'    Primeros 6 dígitos de Tarjeta (CJRMVI_PRITAR)  -> lo dice el propio nombre de campo
'    Terminal ID (CJRMVI_TERMID)                    -> lo dice el propio nombre de campo
'    Últimos 4 dígitos de Tarjeta (CJRMVI_ULTTAR)   -> lo dice el propio nombre de campo
    If Not oInternalPay Is Nothing Then
        With oInternalPay
            ePaymentType = .PaymentTypeID
            oRow.Fields("CJRMVI_DOCFIR").Value = .PayerID
            oRow.Fields("CJRMVI_CHECOM").Value = .TransId
            oRow.Fields("CJRMVI_AUTPOS").Value = .AthorizationCode
            oRow.Fields("CJRMVI_PAGADO").ReadOnly = ReadOnly
            oRow.Fields("CJRMVI_AUTPOS").ReadOnly = ReadOnly
            oRow.Fields("CJRMVI_CUOTAS").ReadOnly = ReadOnly
            oRow.Fields("CJRMVI_IMPING").ReadOnly = ReadOnly
            If ePaymentType = PaymentType_credit_card Or ePaymentType = PaymentType_debit_card Then
                oRow.Fields("CJRMVI_PRITAR").Value = .FirstDigitCard
                oRow.Fields("CJRMVI_ULTTAR").Value = .LastDigitCard
            End If
        End With
    End If
End Sub
Private Function GetPaymentStatusStr(status As InterfaceComNetCore.PaymentStatus) As String
    Select Case status
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_approved
            GetPaymentStatusStr = "approved"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_cancelled
            GetPaymentStatusStr = "cancelled"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_authorized
            GetPaymentStatusStr = "authorized"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_in_process
            GetPaymentStatusStr = "in_process"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_pending
            GetPaymentStatusStr = "pending"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_rejected
            GetPaymentStatusStr = "rejected"
        Case InterfaceComNetCore.PaymentStatus.PaymentStatus_refunded
            GetPaymentStatusStr = "refunded"
        Case Else
            GetPaymentStatusStr = "None"
    End Select
    
End Function
Private Function Initialize(Who As cwTManBZ.ObjectInstance) As Integer  '#NOERRORHANDLING
    Static Paso As Boolean
    On Error GoTo EH
    If Not Paso Then
        With oGrwCompany
            .Init Who
            If .MultiCompany Then
                m_CompanyName = .CoNameForSelect
            End If
        End With
        Initialize = AddConfigMPFromSucurs(Who)
        Paso = True
    End If
Exit Function
EH:
    Initialize = STOP_IT
End Function
Private Function AddConfigMPFromSucurs(Who As cwTManBZ.ObjectInstance) As Integer
    Dim sSql As String
    Dim Rs As rdoResultset
    Dim oBranchs As InternalBranchs
    Dim oInternaBranch As InternalBranch
    Dim oInternalClass As InternalMPClass
    Dim StoreID As Long
    Dim BranID As Long
    On Error GoTo EH
    With Who.Table.Rows(1)
        'Asigno la sucursal a la variable global
        m_Sucurs = .Fields(sTableHeader & "_SUCURS").Value
        sSql = oQuerys.QueryStoreAndBranchFromSucurs(m_Sucurs, m_CompanyName)
    End With
    Set Rs = OPenResultSet(Who, sSql)
    'Add(StoreID As String, StoreName As String, userMP As String, token As String, ExtID As String, FecCre As Date, TimeOut As Double, oBrachs As InternalBranchs) As InternalMPClass
    If Not Rs.EOF Then '#1
        Set oInternalClass = oMPConfig.Item(StoreID)
        StoreID = SuperVal(Rs("STORID"))
        BranID = SuperVal(Rs("BRANID"))
        'Si no existe el Store, cargar todo deSDE cero
        If oInternalClass Is Nothing Then '#2
            'Cargo todos los datos del branch configurado en GRTSUC
            Set oBranchs = New InternalBranchs
            InternalAddBranch Rs, oBranchs
            'Cargo la clase de colecciones de Sucursales o Tiendas
            Set oInternalClass = oMPConfig.Add(StoreID, Rs("STNAME").Value & vbNullString, Rs("USERMP").Value & vbNullString, Rs("BTOKEN").Value & vbNullString, _
            Rs("ST_EXTRID").Value & vbNullString, Rs("ST_FECCRE").Value, Rs("ENDPOINT").Value & vbNullString, oBranchs)
        Else '#2
            Set oBranchs = oInternalClass.Branchs
            If oBranchs Is Nothing Then '#3
                Set oBranchs = New InternalBranchs
                InternalAddBranch Rs, oBranchs
            Else '#3
                Set oInternaBranch = oBranchs.Item(BranID)
                If Not oInternaBranch Is Nothing Then '#4
                    InternalAddBranch Rs, oBranchs
                End If '#4
            End If '#3
        End If '#2
        Rs.Close
        With m_ConfigMP
            .CodEmp = m_CompanyName
            .Sucurs = m_Sucurs
            .StoreID = CStr(StoreID)
            .BranID = CStr(BranID)
        End With
    End If '#1

    Set oInternalClass = Nothing
    Set oBranchs = Nothing
    Set oInternaBranch = Nothing
    Set Rs = Nothing
Exit Function
EH:
    Set oInternalClass = Nothing
    Set oBranchs = Nothing
    Set oInternaBranch = Nothing
    Set Rs = Nothing
    AddConfigMPFromSucurs = STOP_IT
End Function
Private Function InternalAddBranch(Rs As rdoResultset, ByRef oInternalBranchs As InternalBranchs)
    Dim StoreID As Long
    StoreID = SuperVal(Rs("STORID"))
    oInternalBranchs.Add SuperVal(Rs("BRANID")), Rs("QRIMGN").Value & vbNullString, "", Rs("BR_FECCRE").Value, Rs("BR_FECCRE").Value, Rs("BRNAME").Value & vbNullString, (Rs("FIXAMN").Value & vbNullString = "S"), _
    StoreID, Rs("ST_EXTRID").Value & vbNullString, Rs("BR_EXTRID").Value & vbNullString, "", BranchCategories_None, Rs("ARTCOD_DES").Value & vbNullString, Rs("UNIMED").Value & vbNullString, Rs("TIMOUT").Value
   
End Function
Private Function ExistConceptoMP(Who As cwTManBZ.ObjectInstance, oRow As cwTManBZ.Row) As Boolean
    Dim sSql As String
    Dim Rs As rdoResultset
    Dim index As Long
    Dim Key As String
    
    Key = GetKeyIndexConcepto(oRow)
    index = oIndexConcepto.Exists(Key)
    If index <= 0 Then
        With oRow
            sSql = oQuerys.QueryGetConceptoMP(.Fields("CJRMVI_MODCPT").Value, .Fields("CJRMVI_TIPCPT").Value, .Fields("CJRMVI_CODCPT").Value)
            Set Rs = OPenResultSet(Who, sSql)
            If Not Rs.EOF Then
                index = oIndexConcepto.Exists(Key, True)
            End If
        End With
    End If
    Set Rs = Nothing
    ExistConceptoMP = (index > 0)
End Function
Private Function GetKeyIndexConcepto(oRow As cwTManBZ.Row) As String
    Dim Key As String
    With oRow
        Key = Trim(m_Sucurs) & Trim(.Fields("CJRMVI_MODCPT").Value) & Trim(.Fields("CJRMVI_TIPCPT").Value) & Trim(.Fields("CJRMVI_CODCPT").Value)
    End With
    GetKeyIndexConcepto = Key
End Function
Private Function GetCurrentRow(oRow As cwTManBZ.Row, sTbl As String) As cwTManBZ.Row
    With oRow.Tables(sTbl)
        If .CurrentRow <> 0 Then
            Set GetCurrentRow = .Rows(CStr(.CurrentRow))
        End If
    End With
End Function
Private Function GetKeyExternalRef(oRow As cwTManBZ.Row, fecha As Date) As String
    Dim Key As String
    With oRow
        Key = Trim(m_CompanyName) & SEPARATOR & Trim(.Fields("CJRMVI_MODFOR").Value) & SEPARATOR & Trim(.Fields("CJRMVI_CODFOR").Value) & SEPARATOR & Trim(CStr(.Fields("CJRMVI_NROFOR").Value))
        Key = Key & SEPARATOR & Format(fecha, "yyyymmddHHmmss")
    End With
    GetKeyExternalRef = Key
End Function

'*************************************************************************************************
'                                      Llamadas a API Net Core                                   *
'*************************************************************************************************

'******************************************Branchs************************************************
Public Function GetBranchs(StoreID As Long, Optional externalId As String = "", Optional ExtStoreID As String = "", Optional posID As Long = 0) As Variant '#NOERRORHANDLING
On Error GoTo EH
    Dim br As Object
    'public object?[] GetBranchs(long storeId, string externalId, string extStoreid, long posid)
    GetBranchs = oMPApi.GetBranchs(StoreID)
Exit Function
EH:

End Function
Public Function UpdateBrunch(id As Long, name As String, fixed_amount As Boolean, _
    Optional category As BranchCategories = BranchCategories_None) As Object '#NOERRORHANDLING
On Error GoTo EH
    Set UpdateBrunch = oMPApi.UpdateBrunch(id, name, fixed_amount, category)
Exit Function
EH:
End Function
Public Function CreateBranch(StoreID As Long, externalId As String, _
    Optional nameInput As String = "", Optional fixedAmount As Boolean = False, _
        Optional category As BranchCategories = BranchCategories_None) As Object  '#NOERRORHANDLING
On Error GoTo EH
    'public BranchRequestMarshal? CreateBranch(long storeId, string externalId, string nameInput = "", _
        bool fixedAmount = false, BranchCategories inCategory = BranchCategories.None)
    Set CreateBranch = oMPApi.CreateBranch(StoreID, externalId, nameInput, fixedAmount, category)
Exit Function

EH:

End Function
'******************************************Sucrusales/Stores************************************************
Public Function GetStoreFromName(name As String) As Object '#NOERRORHANDLING
On Error GoTo EH
    Set GetStoreFromName = oMPApi.GetSucursalFromName(name)
Exit Function
EH:

End Function
Public Function GetStoreFromId(id As Long) As Object '#NOERRORHANDLING
On Error GoTo EH
    Dim obj As Object
    Set obj = oMPApi.GetSucursalFromId(id)
    Set GetStoreFromId = obj
    Set obj = Nothing
Exit Function
EH:
    Set obj = Nothing
End Function

Public Function UpdateStore(id As Long, name As String, externalId As String) As Object '#NOERRORHANDLING
On Error GoTo EH
'public SucursalRequestMarshal? UpdateSucursal(long id, string name, string external_id)
    Set UpdateStore = oMPApi.UpdateSucursal(id, name, externalId)
Exit Function
EH:

End Function
'*************************************************Orders****************************************************
Public Function SendOrder(externalStoreID As String, externalPosID As String, TotalAmount As Double, itemDescription As String, itemunit As String, _
    FchMov As Date, notificationUrl As String, Optional cashOut As Double = 0, Optional ExternalReference As String = "", Optional title As String = "", Optional desription As String = "") As Integer '#NOERRORHANDLING
    Dim result As Boolean
    On Error GoTo EH
    result = CBool(oMPApi.CreateSingleItemOrder(externalStoreID, externalPosID, TotalAmount, itemDescription, itemunit, _
        FchMov, cashOut, ExternalReference, title, desription))
    'public bool CreateSingleItemOrder(string externalStoreID, string externalPosID, double totalAmount, string itemDescription, string itemUnit,
    'DateTime expidateDateTime, double cashOut = 0, string externalReference = "", string title = "", string desription = "");
    SendOrder = IIf(Not result, STOP_IT, GO_ON)
Exit Function
EH:
    SendOrder = STOP_IT
End Function
Public Function GetOrder(externalPosID As String) As Object '#NOERRORHANDLING
On Error GoTo EH
    Dim order As Object
    'public Order? GetOrder(string externalPosID);
    Set order = oMPApi.GetOrder(externalPosID)
    Set GetOrder = order
    Set order = Nothing
Exit Function
EH:
    Set order = Nothing
End Function
Public Function GetOrderFromId(orderId As String) As Object '#NOERRORHANDLING
On Error GoTo EH
    Dim order As Object
    'public Order? GetOrder(string externalPosID);
    Set order = oMPApi.GetOrderFromId(orderId)
    Set GetOrderFromId = order
    Set order = Nothing
Exit Function
EH:
    Set order = Nothing
End Function
Public Function WaitingForPayOrTimeOutOrder(externalPosID As String, TimeOut As Long) As Object
On Error GoTo EH
    Dim order As Object
    'Order? WaitingForPayOrTimeOutOrder(string externalPosID, int timeOut)
    Set order = oMPApi.WaitingForPayOrTimeOutOrder(externalPosID, TimeOut)
    Set WaitingForPayOrTimeOutOrder = order
    Set order = Nothing
Exit Function
EH:
    Set order = Nothing
End Function
Public Function CancelOrder(externalPosID As String) As Integer '#NOERRORHANDLING
On Error GoTo EH
    Dim result As Boolean
    'public bool CancelOrder(string externalPosID);
    result = CBool(oMPApi.CancelOrder(externalPosID))
    CancelOrder = IIf(Not result, STOP_IT, GO_ON)
Exit Function
EH:
    CancelOrder = STOP_IT
End Function
'**********************************************Payments*****************************************************
Public Function GetAndWaitPayment(StoreID As String, posID As String, ExternalReference As String, TimeOut As Long, Optional PaymentStatus As InterfaceComNetCore.PaymentStatus = PaymentStatus_None) As Object '#NOERRORHANDLING
On Error GoTo EH
    'public PaymentRequestMarshal? GetAndWaitPayment(string storeID, string posID, string externalReference, long timeout, PaymentStatus paystatus = PaymentStatus.None);
    Set GetAndWaitPayment = oMPApi.GetAndWaitPayment(StoreID, posID, ExternalReference, TimeOut, PaymentStatus)
Exit Function
EH:
End Function
Public Function GetPayment(StoreID As String, posID As String, ExternalReference As String) As Object  '#NOERRORHANDLING
On Error GoTo EH
    Dim payment As Object
    'public PaymentRequestMarshal? GetPayment(string storeID, string posID, string externalReference);
    Set payment = oMPApi.GetPayment(StoreID, posID, ExternalReference)
    Set GetPayment = payment
    Set payment = Nothing
Exit Function
EH:
    Set payment = Nothing
End Function
Public Function GetPaymentFromId(paymentId As String) As Object  '#NOERRORHANDLING
On Error GoTo EH
    Dim payment As Object
    'public PaymentRequestMarshal? GetPaymentFromId(string paymentId);
    Set payment = oMPApi.GetPaymentFromId(paymentId)
    Set GetPaymentFromId = payment
    Set payment = Nothing
Exit Function
EH:
    Set payment = Nothing
End Function
'
Public Function GetStrDateISO8601(fecha As Date) As String '#NOERRORHANDLING
    On Error GoTo EH
     GetStrDateISO8601 = CStr(oMPApi.GetStrDateISO8601(fecha))
Exit Function
EH:
    GetStrDateISO8601 = CStr(Now)
End Function
Public Function GetDateTimeFromISO8601(fecha As String) As Date '#NOERRORHANDLING
On Error GoTo EH
    GetDateTimeFromISO8601 = CDate(oMPApi.GetDateTimeFromISO8601(fecha))
Exit Function

EH:
    GetDateTimeFromISO8601 = Now
    
End Function
Private Function WaitForTopicId(topic As eTopic, TimeOut As Integer, ByRef topicId As String) As Integer '#NOERRORHANDLING
    Dim TimeStart As Currency
    Dim TimeStop As Currency
    Dim TimePassed As Currency
    Dim TimeWait As Currency
    Dim continue As Boolean
    TimeWait = TimeOut
    TimeStart = Timer()
    TimePassed = 0
    continue = True
    On Error GoTo EH
    Do While (TimePassed < TimeWait) And continue
        If topic = m_tpTopic.topic Then
            continue = False
            topicId = m_tpTopic.topic
        End If
        Sleep 1000
        TimeStop = Timer()
        TimePassed = TimeStop - TimeStart
        DoEvents
    Loop
    If Not IsNumeric(topicId) Then WaitForTopicId = STOP_IT
Exit Function
EH:
    WaitForTopicId = STOP_IT
End Function


Private Sub Class_Initialize()
    Set oMPConfig = New InternalMPConfig
    Set oGrwCompany = New GRWMultiCompany.Utilities
    Set oIndexConcepto = New GRWIndexKey.IndexKey
    oIndexConcepto.AutoSort = True
    Set oQuerys = New Querys
    'Net core assembly
    Set oMPApi = CreateObject("WrapperMercadoPagoAPI.WrapperMPMarshal")
    Set oMPCallback = CreateObject("WrapperMercadoPagoAPI.ClassInterop")
    'Asigno Me para inicializar el callback
    Set oMPCallback.ReturnValue = Me
End Sub

Private Sub Class_Terminate()
    Set oMPApi = Nothing
    Set oMPCallback = Nothing
    Set oMPConfig = Nothing
    Set oGrwCompany = Nothing
    Set oQuerys = Nothing
    Set oIndexConcepto = Nothing
End Sub

Private Function ICallbackInterop_ReturnValue(ByVal topic As String, ByVal topicId As String) As Boolean
    If topic = "payment" Then
        m_tpTopic.paymentId = topicId
        m_tpTopic.topic = payment
    ElseIf topic = "merchant_order" Then
        m_tpTopic.orderId = topicId
        m_tpTopic.topic = order
    End If
    ICallbackInterop_ReturnValue = True
End Function
