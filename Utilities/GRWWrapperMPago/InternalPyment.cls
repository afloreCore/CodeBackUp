VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalPayment"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Public TransId As String
Public AthorizationCode As String
Public PaymentTypeID As InterfaceComNetCore.PaymentType
Public PaymentMethodID As InterfaceComNetCore.PaymentMethodID
Public PaymentStatus As InterfaceComNetCore.PaymentStatus
Public ExternalReference As String
Public TotalAmount As Double
Public PayerID As String
Public FirstDigitCard As String
Public LastDigitCard As String
