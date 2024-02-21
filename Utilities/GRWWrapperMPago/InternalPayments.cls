VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalPayments"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Private m_Col As Collection
Public Function Add(oPayment As Object) As InternalPayment
    On Error GoTo EH_1
    Dim oInternal As InternalPayment
    If Not oPayment Is Nothing Then
        Set oInternal = New InternalPayment
        With oInternal
            .AthorizationCode = CStr(oPayment.authorization_code)
            .TransId = CStr(oPayment.id)
            .ExternalReference = CStr(oPayment.external_reference)
            .TotalAmount = CDbl(oPayment.transaction_amount)
            .PaymentStatus = oPayment.status
            'payment_method_id ->master-visa-account_money-etc
            .PaymentMethodID = oPayment.payment_method_id
            .PaymentTypeID = oPayment.payment_type_id
            On Error Resume Next
            If Not oPayment.Card Is Nothing Then
                .FirstDigitCard = CStr(oPayment.Card.first_six_digits)
                .LastDigitCard = CStr(oPayment.Card.last_four_digits)
            End If
            If Not oPayment.payer Is Nothing Then
                .PayerID = CStr(oPayment.payment_Id)
            End If
        End With
    End If
    On Error GoTo EH_2
    m_Col.Add oInternal, oInternal.TransId
    Set Add = oInternal
    Set oInternal = Nothing
Exit Function
EH_1:
    Set oInternal = Nothing
EH_2:
    Set oInternal = Nothing
End Function

Public Function Item(index) As InternalPayment
     If m_Col.Count > 0 Then
        Set Item = m_Col(index)
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
End Sub

Private Sub Class_Terminate()
    Set m_Col = Nothing
End Sub

