VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Utilities"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
Private General As New GRW_GENERAL
Private oGrwStrings As GRWStrings.SqlUtilities
Private oIntSite As InternalSite
Private oCollSite As InternalSites
Private oIntTja As InternalTarjeta
Private oCollTja As InternalTarjetas
Private oQuerys As Querys
Private oGrwLan As GRWTranslate.GRWTraducciones
Private m_sUser As String
'#Parte->CL-41034->
'Se lleva a módulo Consts
'Private m_bSincronizado As Boolean
'#Parte->CL-41034<-
Private m_sTipoPidPad As String
Private m_bIsPtoVta As Boolean
Private m_bAnulacion As Boolean
Private m_ErrorParameters As String
Private m_ErrorCodeWithParams As String
Private m_ErrorCode As String
Private m_Module As String
Private m_ComputeName As String
Private m_IsModeTest As Boolean
Private m_SetErrorNroCta As Boolean
Private m_bUsaLectora As Boolean
'#Parte->AFLORE-42---->
Private m_Mudule As String
Private m_bIsOnline As Boolean
Private m_bTraMedPag As Boolean
Private oIndexMedioPago As Object
Private m_DeviceType As String
Private Const C_MODULE = "FC"
'#Parte->AFLORE-42----<
Private Const LAPOS_ID = ""
Private Type tpPending
    Comp As String
    Fecha As String
    Hora As String
    Importe As String
End Type
'
Private Type tpDataFile
    UltNro As Long
    Cupon As String
    CodAut As String
    CodNsu As String
End Type
'
Private Type tpLastTrans
    UltNro As Long
    Cupon As String
    CodAut As String
    CodNsu As String
    CheCkd As String
    '#Parte->AFLORE-43->
    'Fchmov As Date
    Fchmov As String
    '#Parte->AFLORE-43<-
    Import As Double
    Status As String
    Textos As String
End Type

Private Type tpCompSitef
    Fisvid As Long
    Comp As String
    Fecha As String
    Hora As String
    Textos As String
End Type

Private Enum eTrans
    Ninguno = 0
    APROBADA = 1
    NO_ENVIADA = 2
    RECHAZADA = 3
    CANCELADA = 4
    PENDIENTE = 5
End Enum
Private Enum eDateFormat
    DDMMYYYY
    MMDDYYYY
    YYYYMMDD
End Enum
Private Enum eTipoPinPad
    No_Lectora = 0
    PinPad = 1
    LaPos = 2
    Clover = 5
End Enum

Implements IReturnValue
Private m_oPinPad As GrwPinPadNet.PinPad
Public Event ReturnStringValue(ByVal TipoCampo As Long, ByVal Message As String)
Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)

'Private oLocalWho As cwTManBZ.ObjectInstance

Private Sub Class_Initialize()
    Set oQuerys = New Querys
    Set oCollSite = New InternalSites
    Set oCollTja = New InternalTarjetas
    Set oGrwStrings = New GRWStrings.SqlUtilities
    Set oGrwLan = New GRWTranslate.GRWTraducciones
    '#Parte->AFLORE-42->
    m_DeviceType = No_Lectora
    Set oIndexMedioPago = CreateObject("GRWIndexKey.IndexKey")
    oIndexMedioPago.AutoSort = True
    '#Parte->AF1905 ->
    #If VB5 = 1 Then
        LogVersion
    #End If
    ' <- AF1905
End Sub
 
Private Sub Class_Terminate()
    Set oQuerys = Nothing
    Set oCollSite = Nothing
    Set oCollTja = Nothing
    Set oIntSite = Nothing
    Set oIntTja = Nothing
    Set oGrwStrings = Nothing
    Set m_oPinPad = Nothing
    Set oGrwLan = Nothing
    LogEndOfObject "Utilities"    '#Parte->AF1905
    '#Parte->AFLORE-42->
    Set oIndexMedioPago = Nothing
End Sub
 
'#Parte->AFLORE-42<-
Public Property Let Anulacion(vValue As Boolean)
    m_bAnulacion = vValue
End Property
 
Public Property Get ErrorCode() As String
    ErrorCode = m_ErrorCode
End Property
Public Property Get ErrorParameters() As String
    ErrorParameters = m_ErrorParameters
End Property
 
Public Property Get ErrorCodeWithParam() As String
    ErrorCodeWithParam = m_ErrorCodeWithParams
End Property
'#Parte->AFLORE-42
Public Property Get ExistPinPand() As Boolean
    ExistPinPand = m_bUsaLectora
End Property
'#Parte->AFLORE-42
Public Property Let DeviceType(vValue As String)
    m_DeviceType = vValue
End Property
'#Parte->AFLORE-42--->
'Se hace public
Public Function Initialize(Who As Object, Optional Module As String, Optional bPtoVta As Boolean, Optional bAnulacion As Boolean = False, Optional bTraMedPag As Boolean = False) As Integer  '#NOERRORHANDLING
    Static bPaso As Boolean
    Dim bIsAnulc As Boolean
    Dim bIsAnuTot As Boolean
    On Error GoTo EH
    If Not bPaso Then
        m_Module = Module
        Initialize = InitializeCnn(Who)
        If Initialize <> STOP_IT Then
            If m_bUsaLectora Then
                m_bIsPtoVta = bPtoVta
                m_bAnulacion = bAnulacion
                m_bTraMedPag = bTraMedPag
            End If
            'FillIndexCard Who
            bPaso = True
        End If
    End If
Exit Function
EH:
    Initialize = STOP_IT
    InitializeError "GR000068", Err.Description
End Function
Public Function OnEvent(iEvent As evEvents, Who As cwTManBZ.ObjectInstance) As Integer
    '#Parte->AF1905->
    Static loggedToSTA As Boolean
 
    If Not loggedToSTA Then
        loggedToSTA = True
        LogObjectAndVersion Who, "Utilities"
    End If
    ' <- AF1905
    OnEvent = InitializeCnn(Who)
    '#Parte->AFLORE-42->
    If OnEvent <> STOP_IT And m_bUsaLectora Then
    '#Parte->AFLORE-42<-
        Select Case iEvent
            Case VALIDATE_FIELD
                OnEvent = ValidaCampos(Who)
            Case PRE_VALIDATE_SAVE
                OnEvent = PreValidateSave(Who)
        End Select
    End If '#Parte->AFLORE-42<-
End Function
'#Parte->AFLORE-42
Public Function HabilitoCamposAUTPOS(Who As Object)   '#NOERRORHANDLING
    Dim oRow As cwTManBZ.Row
    Dim bIsAnulc As Boolean
    Dim bIsAnuTot As Boolean
    Dim lIndice As Long
    Dim bTraeMedPag As Boolean
 
    On Error GoTo EH
 
    With Who.Table.Rows(1).Tables(sSlpAsientoCjD)
        For Each oRow In .Rows
            With oRow
                lIndice = IndirectAndTrack(oIndexMedioPago, "GRWIndCtasTarjCredito", "AFLORE-42").Item(.Fields("CJRMVI_CTACTE").Value)
 
                ManejoCampoAUTPOS Who, oRow, lIndice
 
                HabilitoCamposPOS Who, lIndice, oRow
 
                HabilitoCodBco oRow
            End With
        Next
    End With
Exit Function
EH:
 
End Function
Public Function ValidaConfigPinPad(Who As Object) As Integer
'    Dim oInstance As cwTManBZ.IObjectHead
'    Set oInstance = Who
'    Dim sSql As String
'    Dim sModulo As String
'    Dim sTable As String
'    Dim sPassord As String
'    Dim sArgs As String
'    Dim Rs As rdoResultset
'    Dim eTipoPinPad As GrwPinPadNet.TipoPinPad
'    Static bPaso As Boolean
'    Dim sTipoPidPad As String
'
    On Error GoTo EH
    '#Parte->AFLORE-42
    'Initialize Who
    '#Parte->AF-1905->
    ValidaConfigPinPad = InternalValidaConfiguracion(Who)
    '#Parte->AF-1905<-
Exit Function
EH:
    ValidaConfigPinPad = STOP_IT
    TraceAndRaiseError Err.Number, Err.Description, Err.Source, Erl, App.EXEName, "Utilities.cls", "Private Function ValidaConfigPinPad"
 
End Function
 
Public Function ImprimirCupon(ParamArray vParam() As Variant)
    Dim eTF As TipoFuncion
    Dim eTP As TipoFormaPago
    Dim bTrue As Boolean
    Dim sFecha As String
    Dim sComercio As String
    On Error GoTo EH
 
    If Not oIntSite Is Nothing Then '#1
        With oIntSite
            If Not IsVariantEmpty(vParam) Then
                eTF = TipoFuncion_ReimpresionUltimo
                eTP = TipoFormaPago_ReimprimirUltimo
                sFecha = CStr(Format(Now, "dd/MM/yyyy"))
                sComercio = .Comercio
            Else
                eTF = TipoFuncion_ReimpresionConID
                eTP = TipoFormaPago_ReimpresionOtros
                sComercio = GetParam("COMERCIO", SPLIT_KEY_VALUE, vParam)
                If Len(Trim(sComercio)) = 0 Then sComercio = .Comercio
            End If
            'Inicializar variables de conexión
            oPinPad.TimeOutSec = .TimeOut
            oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, sComercio, .CUIT, .CUITISV, "" '.Args
            'Inicializar transacción
            '#Parte->SR-571 31/10/22
            'Print oPinPad.ConfigTransaction; eTF, TipoOperacion_General, .TipoPinPad, .Puerto, .IPClover
            oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, IIf(.TipoPinPad = TipoPinPad_Clover, .HTTPPinpad, .Puerto), .IPClover
            '<----------------------
            oPinPad.Imprimir = True
            oPinPad.CodSupervisor = m_sUser
            oPinPad.DebugPrint = True
            Set oPinPad.ReturnValue = Me
            bTrue = oPinPad.Begin("0", CDbl(0.01), eTP, TipoTarjeta_TCreditoDIG, EmpTarjeta_Otro, sFecha, 0, m_sUser)
 
            ImprimirCupon = IIf(bTrue, GO_ON, STOP_IT)
            If ImprimirCupon = STOP_IT Then
                InitializeError oPinPad.ErrorLogCode, oPinPad.ErrorLogDescrp
            End If
        End With
    Else
        'GR000354      No se encuentran datos del puerto COM para este equipo (~1)
        InitializeError "GR000354"
        ImprimirCupon = STOP_IT
    End If
    oPinPad.Dispose
Exit Function
EH:
    ImprimirCupon = STOP_IT
    InitializeError "GR000068", Err.Description
    oPinPad.Dispose
End Function
Public Sub DisposePinPad()
    On Error GoTo EH
        oPinPad.Dispose
    Exit Sub
EH:
    Set oPinPad = Nothing
End Sub
 
Public Function GetPending(ListDelimiter As String, FieldsDelimiter As String, ParamArray vParam() As Variant) As String
    Dim eTF As TipoFuncion
    Dim eTP As TipoFormaPago
    Dim bTrue As Boolean
    Dim sFecha As String
    Dim sComercio As String
    Dim sAux As String
    Dim Cadena As String
    Dim ComprobanteData As String
    Dim Last As Boolean
    Dim Pending As tpPending
    On Error GoTo EH
 
    If Not oIntSite Is Nothing Then '#1
        With oIntSite
            eTF = TipoFuncion_PendientesTodos
            eTP = TipoFormaPago_TcreditoTodas
            sFecha = CStr(Format(Now, "dd/MM/yyyy"))
            '
            If Not IsVariantEmpty(vParam) Then
                sComercio = GetParam("COMERCIO", SPLIT_KEY_VALUE, vParam)
                If Len(Trim(sComercio)) = 0 Then sComercio = .Comercio
                sAux = GetParam("LAST", SPLIT_KEY_VALUE, vParam)
                Last = (sAux = "TRUE")
            End If
 
            'Inicializar variables de conexión
            oPinPad.TimeOutSec = .TimeOut
            oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, sComercio, .CUIT, .CUITISV, "" '.Args
            'Inicializar transacción
            '#Parte->SR-571 31/10/22
            'oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, .Puerto, .IPClover
            oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, IIf(.TipoPinPad = TipoPinPad_Clover, .HTTPPinpad, .Puerto), .IPClover
            '<----------------------
            oPinPad.DebugPrint = True
            Set oPinPad.ReturnValue = Me
            bTrue = oPinPad.Begin("0", CDbl(0.01), eTP, TipoTarjeta_TCreditoDIG, EmpTarjeta_Otro, sFecha, 0, m_sUser)
            If bTrue Then
                If Last Then
                'public void GetLastTrPending(string ListDelimiter, string FieldsDelimiter)
'                string DateLastTrPending();
'                string HourLastTrPending();
'                string CompLastTrPending();
'                string ImportLastTrPending();
                    oPinPad.GetLastTrPending ListDelimiter, FieldsDelimiter
                    Pending.Comp = oPinPad.CompLastTrPending
                    Pending.Fecha = oPinPad.DateLastTrPending
                    Pending.Hora = oPinPad.HourLastTrPending
                    Pending.Importe = oPinPad.ImportLastTrPending
                    'Armar cadena
                    Cadena = Cadena & "Date=" & GRW_GENERAL.Fechas.ConvertirStringAFecha(Pending.Fecha)
                    Cadena = Cadena & FieldsDelimiter
                    Cadena = Cadena & "Comprobante=" & Pending.Comp
                    Cadena = Cadena & FieldsDelimiter
                    Cadena = Cadena & "Import=" & Str(Pending.Importe)
                    Cadena = Cadena & ListDelimiter
                Else
                    Cadena = oPinPad.GetListPending(ListDelimiter, FieldsDelimiter)
                End If
            End If
        End With
    End If
    GetPending = Cadena
    oPinPad.Dispose
    Exit Function
EH:
    InitializeError "GR000068", Err.Description
    Set oPinPad = Nothing
End Function
Public Function SetCancelPending(Cadena As String, ListDelimiter As String, FieldsDelimiter As String, ParamArray vParam() As Variant) As Integer
    Dim eTF As TipoFuncion
    Dim eTP As TipoFormaPago
    Dim bTrue As Boolean
    Dim sFecha As String
    Dim sComercio As String
    On Error GoTo EH
 
    If Not oIntSite Is Nothing Then '#1
        With oIntSite
            eTF = TipoFuncion_PendienteID
            eTP = TipoFormaPago_TcreditoTodas
            sFecha = CStr(Format(Now, "dd/MM/yyyy"))
            sComercio = GetParam("COMERCIO", SPLIT_KEY_VALUE, vParam)
            If Len(Trim(sComercio)) = 0 Then sComercio = .Comercio
            'Inicializar variables de conexión
            oPinPad.TimeOutSec = .TimeOut
            oPinPad.Imprimir = (.Imprimir = "S")
            oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, sComercio, .CUIT, .CUITISV, "" '.Args
            'Inicializar transacción
            '#Parte->SR-571 31/10/22
            'oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, .Puerto, .IPClover
            oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, IIf(.TipoPinPad = TipoPinPad_Clover, .HTTPPinpad, .Puerto), .IPClover
            '<----------------------
            oPinPad.DebugPrint = True
            Set oPinPad.ReturnValue = Me
            oPinPad.CancelListPending Cadena, ListDelimiter, FieldsDelimiter
            bTrue = oPinPad.Begin("0", CDbl(0.01), eTP, TipoTarjeta_TCreditoDIG, EmpTarjeta_Otro, sFecha, 0, m_sUser)
            If Not bTrue Then
                InitializeError oPinPad.ErrorLogCode, oPinPad.ErrorLogDescrp
                SetCancelPending = STOP_IT
            End If
        End With
    End If
    Exit Function
EH:
    InitializeError "GR000068", Err.Description
    Set oPinPad = Nothing
End Function
 
'#Parte->AF1905
Public Function StringVersion() As String        '#NOERRORHANDLING
     StringVersion = App.Major & Right("000" & App.Minor, 3) & Right("000" & App.Revision, 3)
End Function
 
 
'**************************************************************************
'                               Private
'**************************************************************************
Private Function GetConfigConnection(Who As Object) As Integer
    Dim sSql As String
    Dim sArgs As String
    Dim TipoPinPad As String
    Dim sTerminalID As String
    Dim sPassord As String
    Dim eTipo As eTipoPinPad
    Dim Rs As rdoResultset
    Dim oTableManger As Object
    On Error GoTo EH
    sSql = oQuerys.QueryConfigGRTCOM(m_ComputeName, m_sUser, cLectorTarj)
    If Len(Trim(sSql)) > 0 Then '#1
        '#Parte->AFLORE-42->
        'Set Rs = Who.DataAccess.OpenResultSet(sSql)
        Set Rs = OPenResultSet(Who, sSql)
        '#Parte->AFLORE-42<-
    End If '#1
    If Not Rs.EOF Then
        sArgs = Trim(RetornaValorDeCampoMemo(Rs("GRTCOM_PARAMS")))
        sPassord = Rs("PADMIN").Value & vbNullString
        '#Parte->AF-1905->
        'TipoPinPad = Rs("GRTCOM_TIPPIN").Value & vbNullString
        'eTipo = IIf(m_sTipoPidPad = LECTORA_POSNET, eTipoPinPad.PinPad, eTipoPinPad.Clover)
        sTerminalID = Rs("GRTCOM_TERMID").Value & vbNullString
        m_sTipoPidPad = Rs("GRTCOM_TIPPIN").Value & vbNullString
        m_IsModeTest = (Rs("GRTCOM_AMBIEN").Value & vbNullString = "S")
        Select Case m_sTipoPidPad
            Case LECTORA_POSNET
                eTipo = PinPad
            Case LECTORA_CLOVER
                eTipo = Clover
            Case LECTORA_LAPOS
                sTerminalID = ObtieneID(8)
                eTipo = LaPos
            Case Else
                eTipo = No_Lectora
                GetConfigConnection = STOP_IT
                'GR000478      No existe ningún dispositivo POS configurado
                InitializeError "GR000478"
        End Select
        If GetConfigConnection <> STOP_IT Then
        '#Parte->AF-1905<-
            Set oIntSite = oCollSite.Add(Rs("GRTCOM_DIREIP").Value, sTerminalID, Rs("GRTCOM_COMERC").Value & vbNullString, Rs("GRTCOM_NRODOC").Value, "", sArgs, eTipo, CStr(Rs("GRTCOM_PUERTO").Value), Rs("GRTCOM_NOMCOM").Value, Rs("GRTCOM_IPCLOV").Value & vbNullString, 0, SuperVal(Rs("GRTCOM_TMPCON")), Rs("GRTCOM_PINPUE").Value, sPassord, Rs("GRTCOM_PRTCUP").Value & vbNullString, Rs("GRTCOM_AMBIEN").Value & vbNullString)
            m_bUsaLectora = True
        End If  '#Parte->AF-1905<-
        Rs.Close
    End If
    Set Rs = Nothing
    Set oTableManger = Nothing
Exit Function
EH:
    Set Rs = Nothing
    Set oTableManger = Nothing
    GetConfigConnection = STOP_IT
    InitializeError "GR000068", Err.Description
End Function
'#Parte->AF-1905
Private Function InternalValidaConfiguracion(Who As Object) As Integer
    Dim eTipo As eTipoPinPad
    Dim sPassword As String
    Dim sArgs As String
    Dim ePinPadAux As eTipoPinPad
 
    '#Parte->AFLORE-42<-
    If Not oIntSite Is Nothing Then '#000
        With oIntSite
            eTipo = .TipoPinPad
            '#Parte->AFLORE-42->
            'Viene de Operaciones con Fiserv
            If IsObjectWizard(Who) Then
                ePinPadAux = GetTipoPinPad(m_DeviceType)
 
                If eTipo <> ePinPadAux Then
                    'GR000069      El campo ~1 Tiene un valor inválido
                    InitializeError "GR000069", "Tipo de Lectora"
                    InternalValidaConfiguracion = STOP_IT
                End If
            End If
            '#Parte->AFLORE-42<-
    'oCollSite.Add(Rs("GRTCOM_DIREIP").Value, Rs("GRTCOM_TERMID").Value, Rs("GRTCOM_COMERC").Value & vbNullString, Rs("GRTCOM_NRODOC").Value, "", sArgs, eTipo, CStr(Rs("GRTCOM_PUERTO").Value), Rs("GRTCOM_NOMCOM").Value, Rs("GRTCOM_IPCLOV").Value & vbNullString, 0, SuperVal(Rs("GRTCOM_TMPCON")), Rs("GRTCOM_PINPUE").Value, sPassord, Rs("GRTCOM_PRTCUP").Value & vbNullString, Rs("GRTCOM_AMBIEN").Value & vbNullString)
    'Add(IPServer As String, Terminal_ID As String, Comercio As String, CUIT As String, CUITISV As String, Args As String, TipoPinPad As TipoPinPad, Puerto As String, Nombre As String, Optional IPClover As String = "", Optional TiempoTmpCopia As Integer = 0, Optional TimeOut As Integer = 0, Optional HTTPPinpad As String = "", Optional PassWord As String = "", Optional Imprimir As String = "S", Optional AmbTest As String = "N") As InternalSite
            '#Parte->AFLORE-42->
            If InternalValidaConfiguracion <> STOP_IT And (eTipo = eTipoPinPad.Clover Or eTipo = eTipoPinPad.PinPad) Then
            '#Parte->AFLORE-42<-
                If Len(Trim(.CUIT)) = 0 Then '#3
                    'ERROR: Debe parametrizar el CUIT de la empresa
                    Who.SetError "GR000349"
                    InternalValidaConfiguracion = STOP_IT
                ElseIf Len(Trim(.Nombre)) = 0 Then
                    'ERROR: Debe parametrizar el nombre de la empresa
                    Who.SetError "GR000350"
                    InternalValidaConfiguracion = STOP_IT
                ElseIf eTipo = eTipoPinPad.Clover Or eTipo = eTipoPinPad.PinPad Then
                    sPassword = .PassWord
                    '0.0.0.0
                    If Len(Trim(.IPServer)) < 7 Then '#4
                        'GR000069      El campo ~1 Tiene un valor inválido
                        Who.SetError "GR000069", "Dirección de IP del porveedor"
                        InternalValidaConfiguracion = STOP_IT
                    'Si bien los conceptos de tarjeta pueden tener asociado un comercio, este es obligatorio para la inicialización de la conexión
                    ElseIf Len(Trim(.Comercio)) = 0 Then
                         'GR000069      El campo ~1 Tiene un valor inválido
                        Who.SetError "GR000069", "Código de comercio"
                        InternalValidaConfiguracion = STOP_IT
                    ElseIf Len(Trim(.Terminal_ID)) = 0 Then
                        'GR000069      El campo ~1 Tiene un valor inválido
                        Who.SetError "GR000069", "Terminal ID"
                        InternalValidaConfiguracion = STOP_IT
                    ElseIf eTipo = eTipoPinPad.Clover Then
                        If Len(Trim(.IPClover)) < 7 Then '#5
                            'GR000069      El campo ~1 Tiene un valor inválido
                            Who.SetError "GR000069", "Dirección IP Clover"
                            InternalValidaConfiguracion = STOP_IT
                        ElseIf Len(Trim(.HTTPPinpad)) = 0 Then
                            'GR000069      El campo ~1 Tiene un valor inválido
                            Who.SetError "GR000069", "Puerto HTTP pinpad"
                        ElseIf Len(Trim(sPassword)) = 0 Then
                            'GR000069      El campo ~1 Tiene un valor inválido
                            Who.SetError "GR000069", "Password Clover"
                        End If '#5
                    Else
                        sArgs = Trim(.Args)
                        If Len(sArgs) = 0 And .AmbTest <> "S" Then '#6
                         'ERROR: Debe completar el campo ~1
                            Who.SetError "GR000196", "Parámetros adicionales"
                            InternalValidaConfiguracion = STOP_IT
                        End If '#6
                    End If '#4
                End If '#3
            End If '#Parte->AFLORE-42<-
            'Validar Puerto COM
            If InternalValidaConfiguracion <> STOP_IT Then '#10
                If eTipo = eTipoPinPad.LaPos Or eTipo = eTipoPinPad.PinPad Then  '#11
                    If Not IsNumeric(.Puerto) Then  '#12
                        Who.SetError "GR000069", "Puerto COM"
                        InternalValidaConfiguracion = STOP_IT
                    End If '#12
                End If '#11
            End If '#10
        End With
    Else
        'GR000354      No se encuentran datos del puerto COM para este equipo (~1)
        Who.SetError "GR000354", "Lectora de Tarjeta"
        InternalValidaConfiguracion = STOP_IT
        InitializeError "GR000354", "Lectora de Tarjeta"
    End If '#2
 
End Function
Private Property Get oPinPad() As GrwPinPadNet.PinPad
    If m_oPinPad Is Nothing Then
        Set m_oPinPad = New GrwPinPadNet.PinPad
    End If
    Set oPinPad = m_oPinPad
End Property
'#Parte->AFLORE-42
Private Function GetTipoPinPad(sType As String) As eTipoPinPad
    Select Case sType
        Case LECTORA_CLOVER
            GetTipoPinPad = Clover
        Case LECTORA_POSNET
            GetTipoPinPad = PinPad
        Case LECTORA_LAPOS
            GetTipoPinPad = LaPos
        Case Else
            GetTipoPinPad = No_Lectora
    End Select
 
End Function
'#Parte->AFLORE-42
Private Function GetNamePinPad(eType As eTipoPinPad) As String
    Select Case eType
        Case Clover
            GetNamePinPad = LECTORA_CLOVER
        Case PinPad
            GetNamePinPad = LECTORA_POSNET
        Case LaPos
            GetNamePinPad = LECTORA_LAPOS
        Case Else
            GetNamePinPad = ""
    End Select
 
End Function
Private Function ValidaCampos(Who As cwTManBZ.ObjectInstance) As Integer
    Select Case Who.CurrentField
    '#Parte->AFLORE-42---->
'        Case "CJRMVI_AUTPOS"
'            ValidarAUTPOS Who
        ''#Parte->AF-1905->
        'Case "CJRMVI_ONLINE",
        '   ValidarONLINE Who
        Case "CJRMVI_ONLINE", "CJRMVI_CUOTAS", "CJRMVI_AUTPOS"
            SetPagado Who
        '#Parte->AF-1905<-
'        Case "CJRMVI_IMPORT", "CJRMVI_IMPUSS", "CJRMVG_IMPORT", "CJRMVI_CUENTA"
'            If Who.CurrentTable = sSlpAsientoCjD Then
'                ManejoCampoAUTPOS Who
'                HabilitoCamposPOS Who
'            End If
'        Case "CJRMVI_TIPCPT", "CJRMVI_CODCPT"
'            If m_bIsPtoVta And Not (Who.Saving And m_bAnulacion) Then
'                ManejoCampoAUTPOS Who
'                HabilitoCamposPOS Who
'            End If
    '#Parte->AFLORE-42----<
        Case "CJRMVI_PAGADO", "VIRT_ANULAR", "CJRMVI_CUOTAS"
            '#Parte->AF-1905->
            'ValidaCampos = EnviarPagoPinPad(Who)
            ValidaCampos = ValidarPAGADO(Who)
            '#Parte->AFLORE-42->
            'If ValidaCampos <> STOP_IT Then
            If ValidaCampos <> STOP_IT And m_bIsOnline Then
            '#Parte->AFLORE-42<-
                ValidaCampos = EnviarPagoPinPad(Who)
            End If
            '#Parte->AF-1905<-
        Case "VIRT_SINCRO"
            ValidaCampos = InicializarPinPad(Who)
        Case "FCRMVH_NROCTA"
            ValidaCampos = ValidarUltimaTransaccion(Who)
    End Select
 End Function
 
'#Parte->AFLORE-42---->
''#Parte->AF-1905->
'Private Function ValidarONLINE(Who As cwTManBZ.ObjectInstance) As Integer
'    Dim oRow As cwTManBZ.Row
'    Dim sTable As String
'
'    sTable = Who.CurrentTable
'    If sTable = sSlpAsientoCjD Then
'        Set oRow = GetCurrentRow(Who.Table.Rows(1), sTable)
'        With oRow
'            If .Fields("CJRMVI_ONLINE").Value <> "S" Then
'                .Fields("CJRMVI_PAGADO").Value = "N"
'                .Fields("CJRMVI_PAGADO").ReadOnly = True
'                '#Parte->AFLORE-42->
'                m_bIsOnline = False
'            Else
'                '#Parte->AFLORE-42->
'                m_bIsOnline = True
'                '#Parte->AFLORE-42<-
'                If Val(.Fields("CJRMVI_CUOTAS").Value) > 0 Or Val(.Fields("CJRMVI_IMPORT").Value) <> 0 Then
'                    .Fields("CJRMVI_PAGADO").ReadOnly = False
'                End If
'            End If
'        End With
'    End If
'    Set oRow = Nothing
'End Function
'#Parte->AF-1905->
Private Function ValidarPAGADO(Who As Object) As Integer '#NOERRORHANDLING
    Dim oRow As cwTManBZ.Row
    Dim sTable As String
    '#Parte->AFLORE-43->
    Dim bIsOnline As Boolean
    Dim FieldName As String
    On Error GoTo EH
    FieldName = Who.CurrentField
    '#Parte->AFLORE-43<-
    sTable = Who.CurrentTable
    If (sTable = sSlpAsientoCjD Or sTable = sSlpAsientoCjH) And (FieldName = "CJRMVI_PAGADO" Or FieldName = "VIRT_ANULAR") Then
        Set oRow = GetCurrentRow(Who.Table.Rows(1), sTable)
        bIsOnline = True
        With oRow
            '#Parte->AFLORE-43-> Por '#Parte->AFLORE-42
            If (sTable = sSlpAsientoCjD) Then
                If .Fields("CJRMVI_INDICA").Value = "T" Then
                    If FieldName = "CJRMVI_PAGADO" Then
                        If .Fields("CJRMVI_PAGADO").Value = "S" Then
                            If .Fields("CJRMVI_CUOTAS").Value <= 0 Then '#2
                                ValidarPAGADO = STOP_IT
                                'ERROR: Debe completar el campo ~1
                                'Who.SetError "GR000196", .Fields("CJRMVI_CUOTAS").ColumnDef.Caption
                                .Fields("CJRMVI_CUOTAS").SetError "GR000196", .Fields("CJRMVI_CUOTAS").ColumnDef.Caption
                                bIsOnline = False
                            ElseIf .Fields("CJRMVI_IMPORT").Value <= 0 Then
                                ValidarPAGADO = STOP_IT
                                 'ERROR: Debe completar el campo ~1
                                'Who.SetError "GR000196", .Fields("CJRMVI_IMPORT").ColumnDef.Caption
                                .Fields("CJRMVI_IMPORT").SetError "GR000196", .Fields("CJRMVI_IMPORT").ColumnDef.Caption
                                bIsOnline = False
                            End If
                        End If
                    ElseIf FieldName = "CJRMVI_CUOTAS" Then
                        If .Fields("CJRMVI_CUOTAS").Value <= 0 Then '#2
                            bIsOnline = False
                        Else
                            .Fields("CJRMVI_CUOTAS").SetError ""
                        End If
                    End If
                Else
                    bIsOnline = False
                End If
            ElseIf (m_bAnulacion And sTable = sSlpAsientoCjH) Then
                    'toque aflore
                If .Fields("CJRMVI_PAGADO").Value <> "S" Then
                    ValidarPAGADO = STOP_IT
                    bIsOnline = False
                End If
            End If
            If FieldName = "CJRMVI_PAGADO" And Not bIsOnline Then
                .Fields("CJRMVI_PAGADO").Value = "N"
            End If
        End With
    End If
    Set oRow = Nothing
    m_bIsOnline = bIsOnline
Exit Function
EH:
    Set oRow = Nothing
    m_bIsOnline = bIsOnline
    ValidarPAGADO = STOP_IT
    InitializeError "GR000068", Err.Description
End Function
Private Function ValidateSINCRO(Who As Object) As Integer
 
    With Who.Table.Rows(1)
        If .Fields("VIRT_PASADM").Value = vbNullString Then
            'GR000069      El campo ~1 Tiene un valor inválido
            Who.SetError "GR000069", "Clave del dispositivo"
            ValidateSINCRO = STOP_IT
        End If
    End With
 
 End Function
 
 Private Function PreValidateSave(Who As cwTManBZ.ObjectInstance) As Integer
    Dim oRow As cwTManBZ.Row
    Dim sModulo As String
    Dim sTabla As String
    If m_bIsPtoVta Then
        sModulo = Left(Who.Name, 2)
        sTabla = IIf(m_bAnulacion, sSlpAsientoCjH, sSlpAsientoCjD)
        If sModulo = "FC" Then
            For Each oRow In Who.Table.Rows(1).Tables(sTabla).Rows
                With oRow
                    If .Fields("CJRMVI_ONLINE").Value = "S" Then
                        If .Fields("CJRMVI_IMPORT").Value > 0 Then
                            If Not (.Fields("CJRMVI_PAGADO").Value = "S") Then
                                PreValidateSave = STOP_IT
                                'ERROR: Debe completar el campo ~1
                            Who.SetError "GR000196", .Fields("CJRMVI_PAGADO").ColumnDef.Caption
                            Exit For
                            End If
                        End If
                    End If
                End With
            Next
        End If
    End If
 End Function
 Private Function ValidarUltimaTransaccion(Who As cwTManBZ.ObjectInstance) As Integer
    Dim bTrue As Boolean
    Dim eTR As eTrans
    Dim bPending As Boolean
    Dim sFecha As String
    Dim Cadena As String
    Dim CadAux As String
    Dim Status As String
    Dim CompPending As Long
    Dim dImport As Double
    Dim eTF As TipoFuncion
    Dim eTP As TipoFormaPago
    Dim Data As tpDataFile
    Dim LastTrans As tpLastTrans
    Dim Pendings As tpPending
    On Error GoTo EH
    If Not m_SetErrorNroCta Then '#0
        ValidarUltimaTransaccion = ValidaConfigPinPad(Who)
        If ValidarUltimaTransaccion <> STOP_IT Then '#1
            If Not oIntSite Is Nothing Then '#2
                If oIntSite.TipoPinPad <> TipoPinPad_Clover And oIntSite.TipoPinPad <> TipoPinPad_PinPad Then
                'Solo se aplica la validación para Fiserv
                    Exit Function
                End If
                eTR = APROBADA
                With oIntSite
                    eTF = TipoFuncion_PendientesTodos
                    eTP = TipoFormaPago_TcreditoTodas
                    sFecha = CStr(Format(Now, "dd/MM/yyyy"))
 
                    'Inicializar variables de conexión
                    oPinPad.TimeOutSec = .TimeOut
                    oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, .Comercio, .CUIT, .CUITISV, "" '.Args
                    'Inicializar transacción
                    '#Parte->SR-571 31/10/22
                    'oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, .Puerto, .IPClover
                    oPinPad.ConfigTransaction eTF, TipoOperacion_General, .TipoPinPad, IIf(.TipoPinPad = TipoPinPad_Clover, .HTTPPinpad, .Puerto), .IPClover
                    '<----------------------
                    oPinPad.DebugPrint = True
                    Set oPinPad.ReturnValue = Me
                End With
                bTrue = oPinPad.Begin("0", CDbl(0.01), eTP, TipoTarjeta_TCreditoDIG, EmpTarjeta_Otro, sFecha, 0, m_sUser)
                If bTrue Then '#3
                    oPinPad.GetLastTrPending LIST_DELIMITER, FIELD_DELIMITER
                    Pendings.Comp = oPinPad.CompLastTrPending
                    'Convertir a Long el comprobante pendiente
                    If IsNumeric(Pendings.Comp) Then CompPending = CLng(Pendings.Comp)
                    Pendings.Fecha = oPinPad.DateLastTrPending
                    Pendings.Hora = oPinPad.HourLastTrPending
                    Pendings.Importe = oPinPad.ImportLastTrPending
                    'Armar cadena
                    Cadena = Cadena & "Date=" & Pendings.Fecha
                    Cadena = Cadena & FIELD_DELIMITER
                    Cadena = Cadena & "Comprobante=" & Pendings.Comp
                    Cadena = Cadena & FIELD_DELIMITER
                    Cadena = Cadena & "Import=" & Pendings.Importe
                    Cadena = Cadena & LIST_DELIMITER
                End If '#3
            End If '#2
            'Obtener el último nro desde la tabla FCRPOS
            LastTrans = GetLastTrans(Who)
            ' Obtiene alguna de las claves del último comprobante envíado
            ' ----------------------------------------------------------->
            'Primero hay que cargar una colección desde el archivo .dat
            'GetKeysValuesFromDataFile(string ListDelimiter = "", string NexoDelimiter = "")
            oPinPad.GetKeysValuesFromDataFile
            ' KEY: Nro. de comprobante envíado
            ' TR: Código de TR aprobada
            ' CUPON: Nro. de Cupón
            ' DATE: Fecha del comprobante
            'GetValueFromDataFile(string Key, string ListDelimiter = "", string NexoDelimiter = "")
            CadAux = oPinPad.GetValueFromDataFile("FISVID")
            If IsNumeric(CadAux) Then Data.UltNro = CDbl(CadAux)
            Data.CodAut = oPinPad.GetValueFromDataFile("TR")
            Data.Cupon = oPinPad.GetValueFromDataFile("CUPON")
            ' <-----------------------------------------------------------
            'Hay pendientes
            If CompPending > 0 Then '#4
                If CompPending = LastTrans.UltNro Then  '#5
                    bPending = True
                'Comparar con el dato del archivo .dat
                ElseIf CompPending = Data.UltNro Then  '#5
                    bPending = True
                End If '#5
            'No se encotraron pendientes, se debe evaluar los datos de FCRPOD y el archivo .dat
            Else '#4
                If (LastTrans.CheCkd <> "S") Then '#7
                    If Data.UltNro = LastTrans.UltNro Then '#8
                    'Sí tiene valor <> a 0 ya fue chequeada-> no corresponde a una TR sin evaluar
                    'Sino, hay que evaluar el CodAut: Si no es vacío->está aprobada
                        If LastTrans.Status <> "A" And Len(Trim(Data.CodAut)) = 0 Then '#9
                            eTR = NO_ENVIADA
                        Else
                            eTR = APROBADA
                        End If '#9
                    ElseIf Data.UltNro > LastTrans.UltNro Then '#8
                        If Len(Trim(Data.CodAut)) = 0 Then '#10
                            eTR = NO_ENVIADA
                        Else
                            eTR = APROBADA
                        End If '#10
                    ElseIf Len(LastTrans.CodAut) = 0 Then '#8
                        eTR = NO_ENVIADA
                    Else
                        eTR = APROBADA
                    End If '#18
                Else
                    eTR = Ninguno
                End If '#7
            End If '#4
            'Update a FCRPOS-> FCRPOS_CHECKD = 1, porque ya fué evaluada
            If IsNumeric(Pendings.Importe) Then
                dImport = CDbl(Pendings.Importe)
            Else
                dImport = 0
            End If
        '
            If bPending Then '#9
                eTR = PENDIENTE
                ValidarUltimaTransaccion = SetCancelPending(Cadena, LIST_DELIMITER, FIELD_DELIMITER)
                If ValidarUltimaTransaccion <> STOP_IT Then '#10
                    eTR = CANCELADA
                    'GR000471      La operación anterior con Fiserv no se encontraba confirmada y fue cancelada con éxito. Comprobante ~1, Fecha ~2
                    Who.SetError "GR000471", Pendings.Comp, General.Fechas.ConvertirStringAFecha(Pendings.Fecha)
                    'Who.Table.Rows(1).Fields("FCRMVH_NROCTA").SetError "GR000471", Pendings.Comp, Format(Pendings.Fecha, "dd/MM/yyyy"), CDbl(Pendings.Importe)
                End If '#10
            Else '#9 'Analizar estados de eTR
                If eTR = NO_ENVIADA Then '#10
                    'GR000472      La operación anterior con Fiserv no pudo ser enviada para su aprobación
                    Who.SetError "GR000472"
                ElseIf eTR = APROBADA Then
                    'GR000473      La operación anterior con Fiserv se encuentra aprobada
                    Who.SetError "GR000473"
                End If '#10
 
            End If '#9
            m_SetErrorNroCta = True
            'Asignar el Status según el valor de eTR
            Select Case eTR
                Case eTrans.CANCELADA:
                    Status = "C"
Case eTrans.PENDIENTE:
                    Status = "P"
Case eTrans.NO_ENVIADA:
                    Status = "F"
                Case eTrans.RECHAZADA
                    Status = "R"
                Case eTrans.APROBADA
                    Status = "A"
Case Else:
                    Status = "N"
            End Select
            UpdateTableTrans Who, LastTrans.UltNro, LastTrans.CodAut, LastTrans.Cupon, LastTrans.CodNsu, IIf(eTR <> PENDIENTE, "S", ""), "", Status, dImport
        End If '#1
    Else '#0
    'Ya pasó por acá. No por qu'e, pero a veces pasa dos veces. Entonces debo ver el último registro si está como pendiente
        LastTrans = GetLastTrans(Who)
        With LastTrans
            If .Status = "P" Then
                eTR = PENDIENTE
                ValidarUltimaTransaccion = SetCancelPending(Cadena, LIST_DELIMITER, FIELD_DELIMITER)
                If ValidarUltimaTransaccion <> STOP_IT Then '#10
                    eTR = CANCELADA
                    UpdateTableTrans Who, .UltNro, CheCkd:="S", Status:="C"
                End If
            ElseIf .Status = "C" Then
                eTR = CANCELADA
            ElseIf .Status = "A" Then
                eTR = APROBADA
            End If
            '
            If eTR = CANCELADA Then
                'GR000471      La operación anterior con Fiserv no se encontraba confirmada y fue cancelada con éxito. Comprobante ~1, Fecha ~2
                Who.SetError "GR000471", .UltNro, .Fchmov
            ElseIf (eTR <> APROBADA And eTR <> Ninguno) Then
                'GR000472      La operación anterior con Fiserv no pudo ser enviada para su aprobación
                'Who.SetError "GR000472"
                Who.SetError "GR000472"
            ElseIf eTR = APROBADA Then
                'GR000473      La operación anterior con Fiserv se encuentra aprobada
                Who.SetError "GR000473"
            End If
        End With
 
    End If '#0
Exit Function
EH:
     ValidarUltimaTransaccion = STOP_IT
     Who.SetError "GR000068", Err.Description
 
End Function
Private Function GetParam(sField As String, sSplitChar As String, ParamArray vParams() As Variant)
Dim i As Integer
Dim iPos As Integer
Dim sText As String
Dim vVar As Variant
Dim FieldAux As String
On Error GoTo EH
 
    If IsArray(vParams(0)) Then
        vVar = vParams(0)
    Else
        vVar = vParams
    End If
    '
    For i = 0 To UBound(vVar)
        sText = CStr(vVar(i))
        sField = Trim(UCase(sField))
        iPos = InStr(1, sText, sSplitChar)
        If iPos > 0 Then
            FieldAux = Mid(sText, 1, iPos - 1)
            If Trim(UCase(FieldAux)) = sField Then
                sText = Mid(sText, iPos + 1, Len(sText) - 1)
                Exit For
            End If
        Else
            Exit For
        End If
    Next i
    GetParam = sText
Exit Function
EH:
    GetParam = ""
End Function
Private Function IsVariantEmpty(ParamArray vParam() As Variant) As Boolean
    Dim vVar As Variant
    Dim vValue As Variant
    On Error GoTo EH
    If IsArray(vParam(0)) Then
        vVar = vParam(0)
    Else
        vVar = vParam
    End If
 
    vValue = vVar
    Exit Function
EH:
    IsVariantEmpty = True
 
End Function
Private Function InicializarPinPad(Who As Object) As Integer
    Dim oRow As cwTManBZ.Row
    Dim bRespuesta As Boolean
    Dim sPassword As String
    On Err GoTo EH
    If Not m_bSincronizado Then
        'Set oRow = Who.Table.Rows(Who.CurrentRow)
        InicializarPinPad = ValidaConfigPinPad(Who)
        If InicializarPinPad <> FIELD_NOT_OK Then
            With oIntSite
                If Left(Who.Name, 2) = "GR" Then
                    sPassword = Who.Table.Rows(1).Fields("PASSWORD").Value
                Else
                    sPassword = CWFC.DeCrypt(.PassWord)
                End If
 
                oPinPad.CodSupervisor = sPassword
                oPinPad.CodOperador = .CodAdmin
                oPinPad.TimeOutSec = .TimeOut
                'Para validar existencia de dispositivo-> se utilizará el comercio por default en GRTCOM
                oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, .Comercio, .CUIT, .CUITISV, .Args
                oPinPad.ConfigTransaction TipoFuncion_ConfigClover, TipoOperacion_General, TipoPinPad_Clover, .HTTPPinpad, .IPClover
                Set oPinPad.ReturnValue = Me
                'Set oLocalWho = Who
                bRespuesta = oPinPad.Begin("100000", 50, TipoFormaPago_TcreditoTodas, TipoTarjeta_TDebitoMAG, EmpTarjeta_Visa, CStr(Format(Now, "dd/MM/yyyy")), 0, Usuario:=m_sUser)
                If Not bRespuesta Then
                    'GR000460      El POS respondió: Dispositivo no sincronizado. ~1
                    Who.SetError "GR000460", oPinPad.ErrorLogDescrp
                    InicializarPinPad = FIELD_NOT_OK
                Else
                    m_bSincronizado = True
                End If
                'Set oLocalWho = Nothing
            End With
        End If
    End If
    Exit Function
EH:
    InicializarPinPad = FIELD_NOT_OK
    Who.SetError "GR000068", Err.Description
    'Set oLocalWho = Nothing
End Function
Private Function EnviarPagoPinPad(Who As cwTManBZ.ObjectInstance) As Integer
 
    Dim bPortOpened As Boolean
    Dim oRow As cwTManBZ.Row
    Dim oTable As cwTManBZ.Table
    Dim sTable As String
    Dim bTrue As Boolean
    Dim sFecha As String
    Dim sComercio As String
    Dim sComprobante As String
    Dim sTextos As String
    Dim iCuotas As Integer
    Dim NroInt As Long
    Static bPaso As Boolean
    Dim oField As cwTManBZ.Field
    Dim TipoFunc As GrwPinPadNet.TipoFuncion
    Dim TipoOpe As GrwPinPadNet.TipoOperacion
    Dim TipoTja As GrwPinPadNet.TipoTarjeta
    Dim EmpTja As GrwPinPadNet.EmpTarjeta
    Dim oLaPos As LaPos
    Dim tpCompDev As tpCompSitef
    Dim LasTrans As tpLastTrans
    '#Parte->SR-571 31/10/22
    Dim Puerto As String
 
    On Error GoTo EH
    '#Parte->AFLORE-42->
    If m_bUsaLectora Then
    '#Parte->AFLORE-42<-
       If Not Who.Saving Then
           sTable = Who.CurrentTable
           If m_bIsPtoVta And (sTable = sSlpAsientoCjD Or sTable = sSlpAsientoCjH) Then '#0
               'Limpiar error de Cliente
               If m_SetErrorNroCta Then
                   Who.Table.Rows(1).Fields("FCRMVH_NROCTA").SetError ""
                   m_SetErrorNroCta = False
               End If
               '
               Set oTable = Who.Table.Rows(1).Tables(sTable)
               Set oRow = oTable.Rows(Who.CurrentRow)
               Set oField = oRow.Fields(Who.CurrentField)
               '#Parte->AF-1905->
               'EnviarPagoPinPad = ValidaCODTAR(Who, oRow, sTable)
               EnviarPagoPinPad = ValidaCODTAR(Who, oRow, sTable, oIntSite)
               '#Parte->AF-1905<-
               If EnviarPagoPinPad <> STOP_IT Then  '#2
                   EnviarPagoPinPad = ValidaConfigPinPad(Who)
                   If EnviarPagoPinPad <> STOP_IT Then '#3
                       If m_sTipoPidPad = LECTORA_LAPOS Then '#4
                           Set oLaPos = New LaPos
                           EnviarPagoPinPad = oLaPos.EnviarPagoPOS(Who, oRow, oIntSite, sTable)
                           Set oLaPos = Nothing
                       Else
                           If Not oIntSite Is Nothing Then '#5
                              If oIntSite.TipoPinPad = TipoPinPad_Clover Then
                                 EnviarPagoPinPad = InicializarPinPad(Who)
                                 '#Parte->SR-571 31/10/22
                                 Puerto = oIntSite.HTTPPinpad
                              Else
                                 Puerto = oIntSite.Puerto
                               '<---------------------
                              End If
                               If EnviarPagoPinPad <> FIELD_NOT_OK Then '#6
                                   If Not oIntTja Is Nothing Then  '#7
                                       TipoOpe = GetTipoOperacion(oTable, oField)
                                       TipoTja = oIntTja.TipTja
                                       TipoFunc = GetTipoFuncion(oIntTja.TipTja, TipoOpe, oIntSite.TipoPinPad)
                                       EmpTja = oIntTja.EmpTja
                                       'El comercio de la tarjeta es mandatario
                                       sComercio = oIntTja.CodCio
                                   Else
                                       'ERROR: El código de tarjeta es obligatorio
                                       Who.SetError "GR000351"
                                       EnviarPagoPinPad = STOP_IT
                                       GoTo EH
                                   End If '#7
                                   If EnviarPagoPinPad <> STOP_IT Then '#8
                                       iCuotas = oRow.Fields("CJRMVI_CUOTAS").Value
                                       With oIntSite
                                           'Si la TJ no tiene comercio-> se asigna el de la conf. de dispositivos (GRTCOM)
                                           If Len(Trim(sComercio)) = 0 Then sComercio = .Comercio
                                           'Inicializar variables de conexión
                                           oPinPad.ConfigSiteSitef .IPServer, .Terminal_ID, sComercio, .CUIT, .CUITISV, "" '.Args
                                           'Inicialar medio de pago, importe
                                           '#Parte->SR-571 31/10/22
                                           'oPinPad.ConfigTransaction TipoFunc, TipoOpe, .TipoPinPad, .Puerto, .IPClover
                                           oPinPad.ConfigTransaction TipoFunc, TipoOpe, .TipoPinPad, Puerto, .IPClover
                                           '<----------------------
                                           oPinPad.PlandDeCuotas = IIf(iCuotas > 1, 2, 1)
                                           oPinPad.Imprimir = (.Imprimir = "S")
                                           oPinPad.TiempoEsperaCopia = .TiempoTmpCopia
                                           oPinPad.TimeOutSec = .TimeOut
                                           oPinPad.CodSupervisor = m_sUser
                                           oPinPad.FinishAuto = Not (.AmbTest = "S")
                                       End With
                                       With oRow
                                           'Iniciar transacción, Fecha en formato dd/MM/yyyy
                                            If TipoOpe = TipoOperacion_Devolucion Then '#9
                                                '#Parte->AFLORE-43->
                                                'LasTrans = GetLastTrans(who)
                                                'sFecha = LasTrans.Fchmov
                                                'sTextos = LasTrans.Textos
                                                '#Parte->CL-41034->
                                                'NroInt = .Fields("CJRMVI_INTORI").Value
                                                NroInt = IIf(sTable = sSlpAsientoCjH, .Fields("CJRMVI_INTORI").Value, .Fields("CJRMVI_FISVID").Value)
                                                '#Parte->CL-41034<-
                                                tpCompDev = GetPairKeyValueFromTrans(Who, NroInt)
                                                sTextos = tpCompDev.Textos
                                                If IsDate(tpCompDev.Fecha) Then sFecha = tpCompDev.Fecha
                                                '#Parte->AFLORE-43<-
                                           Else
                                               sFecha = ""
                                           End If '#9
                                           If Not bPaso Then '#10
                                               oPinPad.Inicializado = False
                                               bPaso = True
                                           End If '#10
                                           'Confirmar el ID Identity de la tabla insertando un registro
                                           EnviarPagoPinPad = InsertTableTrans(Who, NroInt, General.Fechas.ConvertirFechaAString(Now), m_ComputeName, oPinPad.Terminal_ID, sComercio, m_sUser)
                                           If EnviarPagoPinPad <> STOP_IT Then '#11
                                               'Asigno el valor de indentificación de la transacción
                                               '.Fields("VIRT_FISVID").Value = NroInt
                                               'Se almacenará en este campo en la tabla CJRMVI para luego poder hacer el Join con FCRPOS_NROINT
                                               .Fields("CJRMVI_FISVID").Value = NroInt
                                               'Devuelve los mensajes de la aplicación para mostrar en pantalla
                                               Set oPinPad.ReturnValue = Me
                                               oPinPad.DebugPrint = True
                                               'Enviar un string con el formato Key=Value, con un carater separador
                                               oPinPad.StringKeyPairValue sTextos, "#"
                                               '
                                               bTrue = oPinPad.Begin(NroInt, CDbl(.Fields("CJRMVI_IMPORT").Value), GetTipoDePago(TipoTja, iCuotas), TipoTja, EmpTja, sFecha, iCuotas, m_sUser, .Fields("CJRMVI_CHECOM").Value)
                                               EnviarPagoPinPad = IIf(bTrue, GO_ON, STOP_IT)
                                               If EnviarPagoPinPad <> STOP_IT Then
                                                   ProcesarRespuestaPurchase Who, oIntSite.Terminal_ID, oRow, oPinPad
                                               Else
                                                   Who.SetError oPinPad.ErrorLogCode, oPinPad.ErrorLogDescrp
                                               End If
 
                                           End If '#11
                                       End With
                                       UpdateTableTrans Who, NroInt, oRow.Fields("CJRMVI_AUTPOS").Value, oRow.Fields("CJRMVI_CHEQUE").Value, oRow.Fields("CJRMVI_CHECOM").Value, "", CStr(oPinPad.GetStringKeyValue), IIf(bTrue, "A", "R"), oRow.Fields("CJRMVI_IMPORT").Value
 
                                       SetCJRMVIFields Who, oRow, oField.Name, oTable.Name, EnviarPagoPinPad
                                   End If '#8
                               End If '#6
                           End If '#5
                       End If '#4
                   End If '#3
               End If '#2
           End If '#0
       End If
    End If '#Parte->AFLORE-42<-
    Set oTable = Nothing
    Set oRow = Nothing
    Set oField = Nothing
 
Exit Function
EH:
    EnviarPagoPinPad = STOP_IT
    Who.SetError "GR000068", Err.Description
    Set oTable = Nothing
    Set oRow = Nothing
    Set oField = Nothing
    On Error Resume Next
    oPinPad.Dispose
    Set m_oPinPad = Nothing
End Function
 
Private Function InsertTableTrans(Who As Object, ByRef Fisvid As Long, Fecha As String, ComputerName As String, terminalID As String, Comercio As String, UserID As String) As Integer
    Dim sSql As String
    Dim Rs As rdoResultset
    On Error GoTo EH
    sSql = oQuerys.QueryInsTableTrans(Fisvid, Comercio, terminalID, ComputerName, UserID, Fecha)
    '#Parte->AFLORE-42->
    'Set Rs = Who.DataAccess.OPenResultSet(sSql, rdOpenKeyset)
    Set Rs = OPenResultSet(Who, sSql)
    '#Parte->AFLORE-42<-
    If Not Rs.EOF Then
        Fisvid = SuperVal(Rs(0))
        Rs.Close
    End If
    Set Rs = Nothing
Exit Function
EH:
    InsertTableTrans = STOP_IT
    Who.SetError "GR000068", Err.Description
    Resume Next
    '#Parte->AFLORE-42
    'Who.DataAccess.RollbackTrans
    RollBackTrans Who
End Function
Private Function UpdateTableTrans(Who As Object, Fisvid As Long, Optional CodAut As String = "", Optional Cupon As String = "", Optional CodNsu As String = "", Optional CheCkd As String = "", Optional Textos As String = "", Optional Status As String = "", Optional Import As Double = 0) As Integer
    Dim sSql As String
    On Error GoTo EH
 
    sSql = oQuerys.QueryUpdTableTrans(Fisvid, CodAut, Cupon, CodNsu, CheCkd, Textos, Status, Import)
    UpdateTableTrans = ExecWithTrans(Who, sSql)
 
Exit Function
EH:
    UpdateTableTrans = STOP_IT
    'Who.SetError "GR000068", Err.Description
    '#Parte->AFLORE-42
    'Who.DataAccess.RollBackTrans
    RollBackTrans Who
End Function
Private Function GetPairKeyValueFromTrans(Who As Object, NroInt As Long) As tpCompSitef
    Dim sSql As String
    Dim Rs As rdoResultset
    Dim tpComp As tpCompSitef
 
    On Error GoTo EH
    sSql = oQuerys.QueryGetFieldsFromTrans(NroInt)
    '#Parte->AFLORE-42->
    'Set Rs = Who.DataAccess.OPenResultSet(sSql, rdOpenKeyset)
    Set Rs = OPenResultSet(Who, sSql)
    '#Parte->AFLORE-42<-
    If Not Rs.EOF Then
        tpComp.Textos = Rs("TEXTOS").Value & vbNullString
        tpComp.Fecha = Rs("FCHMOV").Value & vbNullString
        Rs.Close
    End If
    GetPairKeyValueFromTrans = tpComp
    Set Rs = Nothing
Exit Function
EH:
    Set Rs = Nothing
End Function
 
Private Function SetLastNumber(Who As Object) As Integer
    Dim sTable As String
    Dim bTrue As Boolean
    Dim sFieldName As String
    Dim LastTrans As tpLastTrans
    Dim oRow As cwTManBZ.Row
    Dim UltNro As Long
    On Error GoTo EH
    UltNro = -1
    sTable = Who.CurrentTable
    sFieldName = Who.CurrentField
    If m_bIsPtoVta And (sTable = sSlpAsientoCjD Or sTable = sSlpAsientoCjH) Then '#1
        Set oRow = GetCurrentRow(Who.Table.Rows(1), sTable)
        If sFieldName = "CJRMVI_CUOTAS" Then '#2
            If oRow.Fields("CJRMVI_CUOTAS").Value > 0 Then '#3
                bTrue = True
            End If '#3
        ElseIf sFieldName = "CJRMVI_IMPORT" Then '#2
            If oRow.Fields("CJRMVI_IMPORT").Value <> 0 Then '#21
                bTrue = True
            End If '#21
        Else
            bTrue = True
        End If '#2
        If bTrue Then '#4
            SetLastNumber = ValidaConfigPinPad(Who)
            If SetLastNumber <> STOP_IT Then '#5
                If Not oIntSite Is Nothing Then '#6
                    LastTrans = GetLastTrans(Who)
                    'Se suma uno al Nro. obtenido ya que es el próximo a usar para el comercio y terminal asociada
                    UltNro = LastTrans.UltNro + 1
                End If '#6
            End If '#5
        Else
            UltNro = 0
        End If '#4
    End If '#1
 
    oRow.Fields("CJRMVI_FISVID").Value = UltNro
    If UltNro < 0 Then
        SetLastNumber = STOP_IT
        'GR000470      No se pudo obtener el número de identificación para la transacción de Fiserv
        Who.SetError "GR000470"
    End If
    Set oRow = Nothing
Exit Function
 
EH:
    Set oRow = Nothing
    SetLastNumber = STOP_IT
    'GR000470      No se pudo obtener el número de identificación para la transacción de Fiserv
    Who.SetError "GR000470"
End Function
Private Function GetLastTrans(Who As Object) As tpLastTrans
    Dim sSql As String
    Dim NroId As Long
    Dim LasTrans As tpLastTrans
    Dim Rs As rdoResultset
    On Error GoTo EH
 
    sSql = oQuerys.QueryGetLastTrans()
    '#Parte->AFLORE-42->
    'Set Rs = Who.DataAccess.OPenResultSet(sSql, rdOpenKeyset)
    Set Rs = OPenResultSet(Who, sSql)
    '#Parte->AFLORE-42<-
    If Not Rs.EOF Then '#1
        With LasTrans
            .UltNro = SuperVal(Rs("ULTNRO"))
            .Cupon = Rs("NCUPON").Value & vbNullString
            .CodAut = Rs("CODAUT").Value & vbNullString
            .CodNsu = Rs("CODNSU").Value & vbNullString
            .Fchmov = Rs("FCHMOV").Value & vbNullString
            .Import = SuperVal(Rs("IMPORT"))
            .CheCkd = Rs("CHECKD").Value & vbNullString
            .Status = Rs("STATUS").Value & vbNullString
        End With
        Rs.Close
    End If '#1
    GetLastTrans = LasTrans
    Set Rs = Nothing
 
Exit Function
 
EH:
    Set Rs = Nothing
End Function
 
Private Function GetFECALT(Who As Object, sTable As String, sModfor As String, sCodfor As String, lNrofor As Long) As String
    Dim sSql As String
    Dim Rs As rdoResultset
    sSql = oQuerys.QueryGetFECALT(sTable, sModfor, sCodfor, lNrofor)
    '#Parte->AFLORE-42->
    'Set Rs = Who.DataAccess.OPenResultSet(sSql, rdOpenKeyset)
    Set Rs = OPenResultSet(Who, sSql)
    '#Parte->AFLORE-42<-
    If Not Rs.EOF Then
        GetFECALT = Rs("FECHA").Value & vbNullString
        Rs.Close
    End If
 
End Function
Private Function ProcesarRespuestaPurchase(Who As Object, terminalID As String, oRow As cwTManBZ.Row, oPinPad As PinPad) As Integer
    Dim sLote As String
    Dim lLote As Long
'    Código de autorización tarjeta (CJRMVI_AUTPOS)           ->            Nro de autorizacion que devuelve lapos
'
'    Número de cheque/doc (CJRMVI_CHEQUE)                     ->            numero de cupon que devuelve lapos
'
'    Firmante del documento (CJRMVI_DOCFIR) -                 ->            DNI de la persona
'
'    Número de lote (CJRMVI_NROLOT)                           ->            lapos indica cual es el lote abierto en el momento en que se presenta el cupon
'
'    Primeros 6 dígitos de Tarjeta (CJRMVI_PRITAR)            ->            lo dice el propio nombre de campo
'
'    Terminal ID (CJRMVI_TERMID)                              ->            lo dice el propio nombre de campo
'
'    Últimos 4 dígitos de Tarjeta (CJRMVI_ULTTAR)             ->            lo dice el propio nombre de campo
    Dim Digitos As Long
 
    With oRow
        If IsThisNumeric(Trim(oPinPad.Lote)) Then
            lLote = CInt(oPinPad.Lote)
        Else
            lLote = 0
        End If
 
        .Fields("CJRMVI_AUTPOS").Value = Trim(oPinPad.CodAutorizacion)
        '#Parte->SR-571 31/10/22
        '.Fields("CJRMVI_CHEQUE").Value = IIf(IsnUmeric(Trim(oPinPad.NroCupon)), CLng(oPinPad.NroCupon), 0)
        If IsThisNumeric(Trim(oPinPad.NroCupon)) Then
            .Fields("CJRMVI_CHEQUE").Value = CLng(oPinPad.NroCupon)
        Else
            .Fields("CJRMVI_CHEQUE").Value = 0
        End If
 
        .Fields("CJRMVI_CHECOM").Value = Trim(oPinPad.CodTransSitef)    'Se utiliza para los Posnet para las devoluciones como el comprobante original. Clover usa otros campos
        .Fields("CJRMVI_NROLOT").Value = lLote
        .Fields("CJRMVI_DOCFIR").Value = .Fields("CJRMVI_NRODOC").Value
        'Vuelven cruzados, el Last4 es First6 y el First6 es Last4
        If IsNumeric(Trim(oPinPad.PrimerosDigitos)) Then
            Digitos = CLng(Trim(oPinPad.PrimerosDigitos))
        Else
            Digitos = 0
        End If
        .Fields("CJRMVI_PRITAR").Value = Digitos                           'First6
        If IsNumeric(Trim(oPinPad.UltimosDigitos)) Then
            Digitos = CLng(Trim(oPinPad.UltimosDigitos))
        Else
            Digitos = 0
        End If
        .Fields("CJRMVI_ULTTAR").Value = Digitos                           'Last4
        .Fields("CJRMVI_TERMID").Value = terminalID
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
        .Fields("CJRMVI_PAGADO").ValidationState = "Y"
        .Fields("VIRT_ANULAR").ReadOnly = False
        .Fields("VIRT_ANULAR").Value = "N"
    End With
 
 End Function
'#Parte->SR-571 31/10/22
Private Function IsThisNumeric(var As Variant) As Boolean
On Error GoTo EH
    IsThisNumeric = IsNumeric(var)
 
    Exit Function
EH:
    IsThisNumeric = False
 
End Function
'#Parte->AF-1905 por parte SR-571
'Private Function ValidaCODTAR(Who As cwTManBZ.ObjectInstance, oRow As cwTManBZ.Row, sTable As String) As Integer
Private Function ValidaCODTAR(Who As Object, oRow As cwTManBZ.Row, sTable As String, oInternalSite As InternalSite) As Integer
    Dim sSql As String
    Dim rdSelect As rdoResultset
    Dim sCodPos As String
    Dim sDebCre As String
    Dim sCodTar As String
 
    On Error GoTo EH
    '#Parte->AF-1905 por parte SR-571
    If Not oInternalSite Is Nothing Then
       If sTable = sSlpAsientoCjD Then
           sCodTar = Trim(oRow.Fields("CJRMVI_CODTJA").Value)
           If Len(sCodTar) = 0 Then
               oRow.Fields("CJRMVI_CODTAR").Value = vbNullString
           End If
       Else
           sCodTar = SetCodTarjeta(Who, oRow)
       End If
 
       With oGrwStrings
           sSql = "Select CJTTJA_CODPOS, CJTTJA_CODTJA, CJTTJA_DEBCRE, CJTTJA_CODCIO From CJTTJA Where CJTTJA_CODTJA = " & .SQLValue(dtString, sCodTar)
       End With
       Set rdSelect = OPenResultSet(Who, sSql)
 
       If Not rdSelect.EOF Then
           sCodPos = rdSelect("CJTTJA_CODPOS").Value & vbNullString
           oRow.Fields("CJRMVI_CODTAR").Value = sCodPos
           sDebCre = Trim(rdSelect("CJTTJA_DEBCRE").Value & vbNullString)
           '#Parte->AF-1905 por parte SR-571
           'If sDebCre <> "D" And sDebCre <> "C" Then
           If (sDebCre <> "D" And sDebCre <> "C") And (oInternalSite.TipoPinPad = TipoPinPad_Clover Or oInternalSite.TipoPinPad = TipoPinPad_PinPad) Then
           '#Parte->AF-1905<-
               'GR000069      El campo ~1 Tiene un valor inválido
               Who.SetError "GR000069", "El campo Tipo de tarjeta"
               ValidaCODTAR = STOP_IT
           ElseIf Len(Trim(oRow.Fields("CJRMVI_CODTAR").Value)) = 0 Then
               'ERROR: El código de tarjeta es obligatorio
               Who.SetError "GR000351"
               ValidaCODTAR = STOP_IT
           End If
 
           If ValidaCODTAR <> STOP_IT And (Trim(sCodPos)) > 0 Then
               Set oIntTja = oCollTja.Add(sCodPos, rdSelect("CJTTJA_CODTJA").Value & vbNullString, sDebCre, IIf(sDebCre = "D", GrwPinPadNet.TipoTarjeta_TDebitoMAG, GrwPinPadNet.TipoTarjeta_TCreditoMAG), GetEmpTarjeta(sCodPos), rdSelect("CJTTJA_CODCIO").Value & vbNullString)
           End If
           rdSelect.Close
       End If
    '#Parte->AF-1905 por parte SR-571
    Else
        ValidaCODTAR = STOP_IT
        'GR000478      No existe ningún dispositivo POS configurado
        InitializeError "GR000478"
    End If
    Set rdSelect = Nothing
 Exit Function
EH:
 ValidaCODTAR = STOP_IT
 Set rdSelect = Nothing
End Function
 
 Public Function RetornaValorDeCampoMemo(RdField As rdoColumn) As String
 
    RetornaValorDeCampoMemo = oGrwStrings.GetValueFromMemo(RdField)
 
End Function
Private Function GetCurrentRow(oRow As Object, sTbl As String) As cwTManBZ.Row
    With oRow.Tables(sTbl)
        If .CurrentRow <> 0 Then
            Set GetCurrentRow = .Rows(CStr(.CurrentRow))
        End If
    End With
End Function
 
 Private Function ComputerName() As String
    Dim oSystem As CWFC.System
    Set oSystem = New CWFC.System
    ComputerName = oSystem.ComputerName
    Set oSystem = Nothing
End Function
Private Function GetEmpTarjeta(CodTarjeta As String) As GrwPinPadNet.EmpTarjeta
    Select Case CodTarjeta
        Case "100"
            GetEmpTarjeta = EmpTarjeta_MasterCard
        Case "200"
            GetEmpTarjeta = EmpTarjeta_Visa
        Case "300"
            GetEmpTarjeta = EmpTarjeta_Amex
        Case Else
            GetEmpTarjeta = EmpTarjeta_Otro
 
    End Select
End Function
 
Private Function GetTipoFuncion(TipoTja As GrwPinPadNet.TipoTarjeta, TOpe As GrwPinPadNet.TipoOperacion, TPinPad As GrwPinPadNet.TipoPinPad) As GrwPinPadNet.TipoFuncion
    If TOpe = TipoOperacion_General Then '#1
        If TPinPad = TipoPinPad_Clover Then
            GetTipoFuncion = TipoFuncion_PGeneral '#2
        Else
            If TipoTja = TipoTarjeta_TCreditoMAG Then '#3
                GetTipoFuncion = TipoFuncion_Credito
            ElseIf TipoTja = TipoTarjeta_TDebitoMAG Then
                GetTipoFuncion = TipoFuncion_Debito
            Else
                GetTipoFuncion = TipoFuncion_PGeneral
            End If '#3
        End If '#2
    ElseIf TOpe = TipoOperacion_Devolucion Then
        If TPinPad = TipoPinPad_Clover Then '#4
            '#Parte->CL-40497->
            'GetTipoFuncion = TipoFuncion_DevolucionGral
            GetTipoFuncion = TipoFuncion_DevolucionClover
            '#Parte->CL-40497<-
 
        Else
            If TipoTja = TipoTarjeta_TCreditoMAG Then '#5
                    GetTipoFuncion = TipoFuncion_DevolucionTCMAG
            ElseIf TipoTja = TipoTarjeta_TDebitoMAG Then
                GetTipoFuncion = TipoFuncion_DevolucionTDMAG
            Else
                GetTipoFuncion = TipoFuncion_DevolucionGral
            End If '#5
        End If '#4
    End If '#1
 
End Function
 
Private Function GetTipoDePago(TipoTja As GrwPinPadNet.TipoTarjeta, cuotas As Integer) As GrwPinPadNet.TipoFormaPago
    Select Case TipoTja
        Case GrwPinPadNet.TipoTarjeta_TCreditoMAG, GrwPinPadNet.TipoTarjeta_TCreditoDIG
            If cuotas > 1 Then
                GetTipoDePago = TipoFormaPago_TcreditoTodas
            Else
                GetTipoDePago = TipoFormaPago_TCreditoEFT
            End If
        Case GrwPinPadNet.TipoTarjeta_TDebitoMAG, GrwPinPadNet.TipoTarjeta_ClaveDebitoDIG
            If cuotas > 1 Then
                GetTipoDePago = TipoFormaPago_TDebitoTodas
            Else
                GetTipoDePago = TipoFormaPago_TDebitoEFT
            End If
        End Select
End Function
 
Private Function GetTipoOperacion(oTable As cwTManBZ.Table, oField As cwTManBZ.Field) As GrwPinPadNet.TipoOperacion
    If oField.Name = "VIRT_ANULAR" Or oTable.Name = sSlpAsientoCjH Then
    'DEVOLUCION
        GetTipoOperacion = TipoOperacion_Devolucion
    ElseIf oField.Name = "CJRMVI_PAGADO" Or oTable.Name = sSlpAsientoCjD Then
    'COMPRA
        GetTipoOperacion = TipoOperacion_General
    Else
        GetTipoOperacion = TipoOperacion_Ninguno
    End If
End Function
 
Private Sub SetCJRMVIFields(Who As Object, oRow As cwTManBZ.Row, sField As String, sTable As String, OnStop As Integer)
    Dim Fisvid As Long
    With oRow
        If OnStop <> STOP_IT Then '#0
        'Camino resultados OK
            If sTable = sSlpAsientoCjD Then '#1
                'PAGOS
                If sField = "CJRMVI_PAGADO" Then '#2
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
                'ANULACIONES
                ElseIf sField = "VIRT_ANULAR" Then
                    .Fields("CJRMVI_PAGADO").Value = "N"
                    .Fields("VIRT_ANULAR").Value = "S"
                    'Para que no boore el cupón cuando se devuelve en la misma operación
                    '.Fields("CJRMVI_AUTPOS").Value = ""
                    '.Fields("CJRMVI_CHEQUE").Value = ""
                    .Fields("VIRT_ANULAR").ReadOnly = True
                    .Fields("CJRMVI_TIPCPT").ReadOnly = False
                    .Fields("CJRMVI_CODCPT").ReadOnly = False
                    .Fields("CJRMVI_CHEQUE").ReadOnly = False
                    .Fields("CJRMVI_IMPORT").ReadOnly = False
                    .Fields("CJRMVI_AUTPOS").ReadOnly = False
                    .Fields("CJRMVI_CUOTAS").ReadOnly = False
                    .Fields("CJRMVI_ONLINE").ReadOnly = False
                    .Fields("CJRMVI_PAGADO").ReadOnly = False
                    Fisvid = .Fields("CJRMVI_FISVID").Value
                    Who.Validating = True
                    .Fields("CJRMVI_CUOTAS").Value = 0
                    .Fields("CJRMVI_FISVID").Value = Fisvid
 
                    .Fields("CJRMVI_CUOTAS").ReadOnly = False
                End If '#2
            Else
            'DEVOLUCIONES
                .Fields("CJRMVI_AUTPOS").ReadOnly = True
                .Fields("CJRMVI_PAGADO").ReadOnly = True
                .Fields("CJRMVI_PAGADO").Value = "S"
            End If '#1
        Else
        'Camino resultado NO OK
           If sTable = sSlpAsientoCjD Then '#3
                'PAGOS
                If sField = "CJRMVI_PAGADO" Then '#4
                    .Fields("CJRMVI_PAGADO").Value = "N"
                ElseIf sField = "VIRT_ANULAR" Then
                'ANULACIONES
                    .Fields("VIRT_ANULAR").Value = "N"
                End If '#4
            Else
            'DEVOLUCIONES
                .Fields("CJRMVI_PAGADO").Value = "N"
            End If '#3
        End If '#0
    End With
End Sub
Private Function SetCodTarjeta(Who As Object, oRow As cwTManBZ.Row) As String
    Dim rdSelect As rdoResultset
    Dim sSql As String
    On Error GoTo EH
    With oRow
        If .Fields("CJRMVI_INDICA").Value = "T" Then
            sSql = "SELECT CJTTJA_CODTJA, CJTTJA_DESCRP FROM CJTTJA"
            sSql = sSql & " INNER JOIN CJTCTA ON(CJTCTA_CODTJA = CJTTJA_CODTJA AND CJTCTA_INDICA = 'T')"
            sSql = sSql & " INNER JOIN GRCCOH ON(GRCCOH_CJCTACTE = CJTCTA_CTACTE)"
            sSql = sSql & " WHERE GRCCOH_MODCPT = " & .Fields("CJRMVI_MODCPT").SQLValue & " AND GRCCOH_TIPCPT = " & .Fields("CJRMVI_TIPCPT").SQLValue & " AND GRCCOH_CODCPT = " & .Fields("CJRMVI_CODCPT").SQLValue
            '#Parte->AFLORE-42->
            'Set rdSelect = Who.DataAccess.OPenResultSet(sSql, rdOpenKeyset)
            Set rdSelect = OPenResultSet(Who, sSql)
            '#Parte->AFLORE-42<-
            If Not rdSelect.EOF Then
                SetCodTarjeta = Trim(rdSelect("CJTTJA_CODTJA").Value & vbNullString)
                rdSelect.Close
            End If
        End If
    End With
 
    Set rdSelect = Nothing
Exit Function
EH:
    Set rdSelect = Nothing
End Function
Private Sub InitializeError(sErrorCode As String, Optional sParam As String = "") '#NOERRORHANDLING
 'por ahora no se implementa los parámetros de la descripción
    On Error Resume Next
    m_ErrorParameters = sParam
    m_ErrorCode = sErrorCode
'
'    If Len(Trim(sParam)) = 0 Then
'        m_ErrorCodeWithParams = sErrorCode
'    Else
'         m_ErrorCodeWithParams = oGrwLan.SolveParams(sErrorCode, sParam)
'    End If
 
End Sub
 
Private Function IsObjectWizard(Who As Object) As Boolean '#NOERRORHANDLING
 
'Se definio por preguntar steps(1).table.name si tiene esta proepidad es wizard. Porque algunos procesos los que tienen bajada abm como el de produccion (no tuerquita)
'En el CLASS trae el nombre de la tabla. Se vio con JS, MI, JN.
    Dim sSiSeteaNoRompe As String
 
    On Error GoTo EH
    sSiSeteaNoRompe = Who.Steps(1).Table.Name
    IsObjectWizard = True
    Exit Function
 
EH:
 
    IsObjectWizard = False
End Function
Private Function DateTO_FORMAT(dFecha As Date, eFormat As eDateFormat, Optional Delimiter As String = "")
    Dim sYYYY, sMM, sDD, sHH, sMI, sSS
    sYYYY = LZeroPad(Year(dFecha), 4)
    sMM = LZeroPad(Month(dFecha), 2)
    sDD = LZeroPad(Day(dFecha), 2)
 
    Select Case eFormat
        Case eDateFormat.DDMMYYYY
            DateTO_FORMAT = sDD & Delimiter & sMM & Delimiter & sYYYY
        Case eDateFormat.MMDDYYYY
            DateTO_FORMAT = sMM & Delimiter & sDD & Delimiter & sYYYY
        Case eDateFormat.YYYYMMDD
            DateTO_FORMAT = sYYYY & Delimiter & sMM & Delimiter & sDD
    End Select
 
End Function
Private Function LZeroPad(vValue, nPos)
    LZeroPad = Right(String(nPos, "0") & Trim(vValue), nPos)
End Function
'Private Sub IReturnValue_ReturnStringValue(ByVal Message As String)
'    RaiseEvent ReturnStringValue(Message)
''    If Left(oLocalWho.Name, 2) = "GR" Then
''        With oLocalWho.Table.Rows(1)
''            .Fields("VIRT_CODSIN").Value = Message
''        End With
''    End If
'    'Debug.Print Message
'End Sub
'Private Sub IReturnValue_ReturnStringValue(ByVal TipoCampo As Long, ByVal Message As String)
'    RaiseEvent ReturnStringValue(TipoCampo, Message)
'End Sub
'Private Sub IReturnValue_ReturnStringValue(ByVal TipoCampo As Long, ByVal Message As String)
''     Select Case TipoCampo
''        'Código de sincronización cpara Clover
''        Case 5081
''             RaiseEvent ReturnStringValue(TipoCampo, Message)
''        Case Else
''
''            cwfc.MsgBox Message, vbInformation
''        End Select
'    If m_Module = "FC" Then
'        'Finalizar de Forma manual?
'        If TipoCampo = 9999 Then
'            'Ambiente de prueba
'            If m_IsModeTest Then
'                CWFC.MsgBox Message
'            End If
'        Else
'            CWFC.MsgBox Message, vbInformation
'        End If
'    Else
'        RaiseEvent ReturnStringValue(TipoCampo, Message)
'    End If
'End Sub
Private Sub IReturnValue_ReturnStringValue(ByVal TipoCampo As Long, ByVal Message As String, YesNot As Boolean)
    Dim rpt As VbMsgBoxResult
    If m_Module = "FC" Then
        'Finalizar de Forma manual?
        If TipoCampo = 9999 Then
            'Ambiente de prueba
            If m_IsModeTest Then
                rpt = CWFC.MsgBox(Message, vbYesNo)
                YesNot = (rpt = vbOK) Or (rpt = vbYes)
            End If
        Else
            CWFC.MsgBox Message, vbInformation
        End If
    Else
        RaiseEvent ReturnStringValue(TipoCampo, Message)
    End If
End Sub
 
Public Function InternalCreateObject(oObj As Object, sObjectName As String) As Integer '#NOERRORHANDLING
 
    On Error GoTo EH
 
    Set oObj = CreateObject(sObjectName)
 
    Exit Function
EH:
 
    InternalCreateObject = STOP_IT
 
End Function
'*******************************************************************************
'                                   PRIVATE
'*******************************************************************************
'#Parte->AFLORE-42
Private Function InitializeCnn(Who As Object) As Integer '#NOERRORHANDLING
    Static bPaso As Boolean
    On Error GoTo EH
    If Not bPaso Then
        m_sUser = Who.Company.Core.User.ID
        m_Module = Left(Who.Name, 2)
        m_ComputeName = ComputerName
        InitializeCnn = GetConfigConnection(Who)
        bPaso = True
    End If
Exit Function
EH:
    InitializeCnn = STOP_IT
End Function
'#Parte->AFLORE-42
Private Function ManejoCampoAUTPOS(Who As cwTManBZ.ObjectInstance, Optional oRow As cwTManBZ.Row = Nothing, Optional lInd As Long = 0)
    Dim sTable As String
    If m_bUsaLectora Then
        If ConfigPOS(Who) Then
           If oRow Is Nothing Then
               sTable = Who.CurrentTable
               Set oRow = GetCurrentRow(Who.Table.Rows(1), sTable)
               lInd = IndirectAndTrack(oIndexMedioPago, "GRWIndCtasTarjCredito", "AFLORE-42").Item(oRow.Fields("CJRMVI_CTACTE").Value)
           End If
 
           With oRow
               If lInd > 0 Then
                   'Cuando la cuenta es de tipo Tarjeta de Crédito, habilito el campo AUTPOS
                   .Fields("CJRMVI_AUTPOS").ReadOnly = False
                   If .Fields("CJRMVI_IMPORT").Value > 0 Then
                       'Si el importe es mayor a 0 requiero el código de autorización de la tarjeta
                       .Fields("CJRMVI_AUTPOS").Required = True
                   Else
                       .Fields("CJRMVI_AUTPOS").Required = False
                   End If
               Else
                   .Fields("CJRMVI_AUTPOS").ReadOnly = True
               End If
           End With
        End If
    End If
 
 End Function
'#Parte->AFLORE-42
 Private Function HabilitoCamposPOS(Who As cwTManBZ.ObjectInstance, Optional lInd As Long = 0, Optional oRow As cwTManBZ.Row = Nothing)
    If m_bUsaLectora Then
        If ConfigPOS(Who) Then
            If oRow Is Nothing Then
                With Who.Table.Rows(1).Tables(sSlpAsientoCjD)
                    For Each oRow In .Rows
                        lInd = IndirectAndTrack(oIndexMedioPago, "GRWIndCtasTarjCredito", "AFLORE-42").Item(oRow.Fields("CJRMVI_CTACTE").Value)
                       SetCamposPOS oRow, lInd
                    Next
                End With
            Else
                SetCamposPOS oRow, lInd
            End If
        End If
    End If
End Function
'#Parte->AFLORE-42
Private Sub SetCamposPOS(oRow As cwTManBZ.Row, lInd As Long)
    With oRow
       If lInd > 0 Then
           'Cuando la cuenta es de tipo Tarjeta de Crédito, habilito los campos para el POS
           .Fields("CJRMVI_CUOTAS").ReadOnly = False
           .Fields("CJRMVI_ONLINE").ReadOnly = False
           .Fields("CJRMVI_ONLINE").Value = "S"
           If .Fields("CJRMVI_IMPORT").Value > 0 Then
               .Fields("CJRMVI_PAGADO").ReadOnly = False
           Else
               .Fields("CJRMVI_PAGADO").ReadOnly = True
           End If
           .Fields("VIRT_ANULAR").ReadOnly = True
       Else
           If .Fields("CJRMVI_INDICA").Value = "D" And m_bIsPtoVta Then
               .Fields("CJRMVI_CUOTAS").ReadOnly = False
               .Fields("CJRMVI_CUOTAS").Value = 0
               .Fields("CJRMVI_ONLINE").ReadOnly = True
               .Fields("CJRMVI_ONLINE").Value = "N"
               .Fields("CJRMVI_PAGADO").ReadOnly = True
               .Fields("VIRT_ANULAR").ReadOnly = True
           Else
               .Fields("CJRMVI_CUOTAS").ReadOnly = True
               .Fields("CJRMVI_CUOTAS").Value = 0
               .Fields("CJRMVI_ONLINE").ReadOnly = True
               .Fields("CJRMVI_ONLINE").Value = "N"
               .Fields("CJRMVI_PAGADO").ReadOnly = True
               .Fields("VIRT_ANULAR").ReadOnly = True
           End If
       End If
 
       If Not m_bUsaLectora Then
           If m_bTraMedPag Then
               .Fields("CJRMVI_ONLINE").Value = "N"
               .Fields("CJRMVI_ONLINE").ReadOnly = True
           End If
           .Fields("CJRMVI_PAGADO").Value = "N"
           .Fields("CJRMVI_PAGADO").ReadOnly = True
       End If
 
   End With
End Sub
'#Parte->AFLORE-42
Private Function HabilitoCodBco(oRow As cwTManBZ.Row)
 
    With oRow
        If .Fields("CJRMVI_INDICA").Value = "T" Or .Fields("CJRMVI_INDICA").Value = "C" Then
            .Fields("CJRMVI_CODBCO").ReadOnly = False
        Else
            .Fields("CJRMVI_CODBCO").ReadOnly = True
        End If
    End With
 
End Function
''#Parte->AFLORE-42
'Private Sub SetPagado(who As cwTManBZ.ObjectInstance)
'    Dim sTable As String
'    Dim oRowCJ As cwTManBZ.Row
'    Dim lenAutPos As Integer
'    Dim bAutPosReadOnly As Boolean
'
'   sTable = who.CurrentTable
'    If sTable = sSlpAsientoCjD Then
'
'        Set oRowCJ = GetCurrentRow(who.Table.Rows(1), sTable)
'        With oRowCJ
'            bAutPosReadOnly = .Fields("CJRMVI_AUTPOS").ReadOnly
'            If .Fields("CJRMVI_ONLINE").Value = "N" Then
'                lenAutPos = Len(Trim(.Fields("CJRMVI_AUTPOS").Value))
'                If (Not bAutPosReadOnly And lenAutPos > 0) Or bAutPosReadOnly Then
'                        .Fields("CJRMVI_PAGADO").ReadOnly = True
'                        .Fields("CJRMVI_PAGADO").Value = "N"
'                        m_bIsOnline = False
'                Else
'                    .Fields("CJRMVI_PAGADO").ReadOnly = False
'                    .Fields("CJRMVI_ONLINE").ReadOnly = False
'                    .Fields("CJRMVI_ONLINE").Value = "S"
'                End If
'                '#Parte->AFLORE-43-> Comportamiento encontrado por Testing
'                If Not bAutPosReadOnly And (.Fields("CJRMVI_CUOTAS").Value > 0 Or .Fields("CJRMVI_IMPORT").Value > 0) Then
'                    .Fields("CJRMVI_AUTPOS").Required = True
'                Else
'                    .Fields("CJRMVI_AUTPOS").Required = False
'                End If
'
'               '#Parte->AFLORE-43<-
'            Else
'               If (.Fields("CJRMVI_CUOTAS").Value > 0 Or .Fields("CJRMVI_IMPORT").Value > 0) And m_bUsaLectora Then
'                   .Fields("CJRMVI_PAGADO").ReadOnly = False
'                   '#Parte->AFLORE-43->
'                   If .Fields("CJRMVI_AUTPOS").ReadOnly = False Then
'                       .Fields("CJRMVI_AUTPOS").Required = True
'                   Else
'                       .Fields("CJRMVI_AUTPOS").Required = False
'                   End If
'                   '#Parte->AFLORE-43<-
'               End If
'               m_bIsOnline = True
'            End If
'
'           If .Fields("CJRMVI_INDICA").Value = "T" Or .Fields("CJRMVI_INDICA").Value = "D" Then
'               If Not m_bUsaLectora Then
'                   .Fields("CJRMVI_PAGADO").Value = "N"
'                   .Fields("CJRMVI_PAGADO").ReadOnly = True
'               End If
'           End If
'        End With
'    End If
'End Sub
'#Parte->AFLORE-43
Private Sub SetPagado(Who As cwTManBZ.ObjectInstance)
    Dim sTable As String
    Dim oRowCJ As cwTManBZ.Row
    Dim lenAutPos As Integer
    Dim bAutPosReadOnly As Boolean
    Dim CurrentFienld As String
 
   sTable = Who.CurrentTable
    If sTable = sSlpAsientoCjD Then
        '#Parte->CL-40497
        CurrentFienld = Who.CurrentField
 
        Set oRowCJ = GetCurrentRow(Who.Table.Rows(1), sTable)
        With oRowCJ
            If .Fields("CJRMVI_INDICA").Value = "T" Then '#1
                If (.Fields("CJRMVI_CUOTAS").Value > 0 Or .Fields("CJRMVI_IMPORT").Value > 0) Then '#2
                    lenAutPos = Len(Trim(.Fields("CJRMVI_AUTPOS").Value))
                    '
                    .Fields("CJRMVI_AUTPOS").Required = True
                    .Fields("CJRMVI_CHEQUE").Required = True
                    If lenAutPos = 0 And CurrentFienld <> "CJRMVI_ONLINE" Then '#3
                        .Fields("CJRMVI_ONLINE").ReadOnly = False
                        .Fields("CJRMVI_ONLINE").Value = "S"
                        .Fields("CJRMVI_PAGADO").ReadOnly = False
                        .Fields("CJRMVI_PAGADO").ValidationState = ""
                    '#Parte->CL-40497->
'                    Else
'                        .Fields("CJRMVI_ONLINE").Value = "N"
'                        .Fields("CJRMVI_ONLINE").ReadOnly = True
'                        .Fields("CJRMVI_PAGADO").Value = "N"
'                        .Fields("CJRMVI_PAGADO").ReadOnly = False
'                        .Fields("CJRMVI_PAGADO").ValidationState = "Y"
                    ElseIf CurrentFienld = "CJRMVI_ONLINE" Then
                        .Fields("CJRMVI_PAGADO").ReadOnly = False
                        .Fields("CJRMVI_PAGADO").Value = "N"
                        .Fields("CJRMVI_PAGADO").ValidationState = ""
                    '#Parte->CL-40497<-
                    End If '#3
                Else
                    .Fields("CJRMVI_AUTPOS").Required = False
                    .Fields("CJRMVI_CHEQUE").Required = False
                End If '#2
            ElseIf .Fields("CJRMVI_INDICA").Value = "D" Then
                .Fields("CJRMVI_CHEQUE").Required = True
                .Fields("CJRMVI_ONLINE").ReadOnly = True
                .Fields("CJRMVI_PAGADO").Value = "N"
                .Fields("CJRMVI_PAGADO").ReadOnly = True
                .Fields("CJRMVI_PAGADO").ValidationState = "Y"
 
            End If '#1
        End With
    End If
End Sub
'#Parte->AFLORE-42
Private Function ValidarAUTPOS(Who As cwTManBZ.ObjectInstance) As Integer
    Dim oRow As cwTManBZ.Row
    If ConfigPOS(Who) Then
       With oRow
           If Len(Trim(.Fields("CJRMVI_AUTPOS").Value)) > 0 Then
               .Fields("CJRMVI_AUTPOS").Required = False
               .Fields("CJRMVI_PAGADO").Value = "N"
               .Fields("CJRMVI_PAGADO").ReadOnly = True
               If oRow.Table.Name = sSlpAsientoCjD Then
                   .Fields("CJRMVI_ONLINE").Value = "N"
                   .Fields("CJRMVI_ONLINE").ReadOnly = True
               End If
           Else
               If oRow.Table.Name = sSlpAsientoCjD Then 'Solapa Debe
                   .Fields("CJRMVI_PAGADO").ReadOnly = False
                   .Fields("CJRMVI_ONLINE").Value = "S"
                   .Fields("CJRMVI_ONLINE").ReadOnly = False
                   .Fields("CJRMVI_CUOTAS").Value = 1
                   .Fields("CJRMVI_CUOTAS").ReadOnly = False
               Else 'Solapa Haber
                   .Fields("CJRMVI_PAGADO").ReadOnly = (Len(Trim(.Fields("CJRMVI_CODTAR").Value)) = 0)
               End If
 
               If .Fields("CJRMVI_IMPORT").Value > 0 Then
                   'Si el importe es mayor a 0 requiero el código de autorización de la tarjeta
                   .Fields("CJRMVI_AUTPOS").Required = True
                   'ERROR: Debe completar el campo ~1
                   Who.SetError "GR000196", "Código de autorización tarjeta"
                   ValidarAUTPOS = STOP_IT
               Else
                   .Fields("CJRMVI_AUTPOS").Required = False
               End If
           End If
           If Not m_bUsaLectora Then
               .Fields("CJRMVI_ONLINE").Value = "N"
               .Fields("CJRMVI_ONLINE").ReadOnly = True
               .Fields("CJRMVI_PAGADO").Value = "N"
               .Fields("CJRMVI_PAGADO").ReadOnly = True
           End If
 
       End With
    End If
 
End Function
Private Sub FillIndexCard(Who As Object)
    Dim sSql As String
    Dim lIndTarjCred As Long
    Dim rdSelect As rdoResultset
 
    On Error GoTo EH
 
    sSql = "SELECT CJTCTA_CTACTE FROM CJTCTA WHERE CJTCTA_INDICA = 'T' "
    Set rdSelect = OPenResultSet(Who, sSql)
 
    If Not rdSelect.EOF Then
        While Not rdSelect.EOF
            'Cuentas tipo Tarjeta de crédito: cargo el índice
            lIndTarjCred = lIndTarjCred + 1
            oIndexMedioPago.Add rdSelect("CJTCTA_CTACTE").Value & vbNullString, lIndTarjCred
            rdSelect.MoveNext
        Wend
 
        rdSelect.Close
    End If
 
    Set rdSelect = Nothing
Exit Sub
EH:
    Set rdSelect = Nothing
End Sub
Private Function ObtieneID(cntdig As Integer) As String
    Dim oHash As Object
 
    InternalCreateObject oHash, "CWFC.MD5Hash"
    ObtieneID = Left(oHash.HashBytes(CWFC.GetRandomKey), cntdig)
 
    Set oHash = Nothing
End Function
Private Function ConfigPOS(Who As Object) As Boolean '#NOERRORHANDLING
    On Error GoTo EH
    If m_bIsPtoVta And Not Who.Saving Then
        ConfigPOS = True
    End If
Exit Function
EH:
    ConfigPOS = False
End Function
