VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "COMDLG32.OCX"
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   7065
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   7710
   LinkTopic       =   "Form1"
   ScaleHeight     =   7065
   ScaleWidth      =   7710
   StartUpPosition =   3  'Windows Default
   Begin MSComDlg.CommonDialog CommonDialog1 
      Left            =   720
      Top             =   6480
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.CommandButton Command2 
      Caption         =   "..."
      Height          =   375
      Left            =   7080
      TabIndex        =   11
      Top             =   1680
      Width           =   495
   End
   Begin VB.TextBox Text1 
      Height          =   375
      Index           =   3
      Left            =   1560
      TabIndex        =   9
      Top             =   1680
      Width           =   5535
   End
   Begin VB.Frame Frame1 
      Caption         =   "Mensajes desde Server"
      Height          =   3375
      Left            =   120
      TabIndex        =   7
      Top             =   2640
      Width           =   7575
      Begin VB.TextBox Text2 
         Height          =   3015
         Left            =   120
         MultiLine       =   -1  'True
         TabIndex        =   8
         Top             =   240
         Width           =   3855
      End
      Begin VB.Image Image1 
         Height          =   3135
         Left            =   4320
         Top             =   120
         Width           =   3135
      End
   End
   Begin VB.TextBox Text1 
      Height          =   375
      Index           =   2
      Left            =   1560
      TabIndex        =   6
      Top             =   720
      Width           =   5895
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Conectar"
      Height          =   375
      Left            =   2760
      TabIndex        =   4
      Top             =   6120
      Width           =   1815
   End
   Begin VB.TextBox Text1 
      Height          =   375
      Index           =   1
      Left            =   1560
      TabIndex        =   3
      Top             =   1200
      Width           =   2895
   End
   Begin VB.TextBox Text1 
      Height          =   375
      Index           =   0
      Left            =   1560
      TabIndex        =   0
      Top             =   240
      Width           =   2895
   End
   Begin VB.Label Label2 
      Caption         =   "Path imagen QR"
      Height          =   375
      Index           =   2
      Left            =   120
      TabIndex        =   10
      Top             =   1800
      Width           =   1695
   End
   Begin VB.Label Label2 
      Caption         =   "EndPoint"
      Height          =   375
      Index           =   1
      Left            =   120
      TabIndex        =   5
      Top             =   840
      Width           =   1575
   End
   Begin VB.Label Label2 
      Caption         =   "ConnectionId"
      Height          =   375
      Index           =   0
      Left            =   120
      TabIndex        =   2
      Top             =   1320
      Width           =   1695
   End
   Begin VB.Label Label1 
      Caption         =   "Client ID"
      Height          =   375
      Left            =   120
      TabIndex        =   1
      Top             =   240
      Width           =   1455
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Private oMPCallback As Object
Const StoreID As String = "PDV0001"

Private Sub Command1_Click()
    Dim result As Boolean
    If Len(Text1(0).Text) > 0 And Len(Text1(2).Text) > 0 Then
        result = oMPCallback.Initialize(Text1(2).Text, "clientId=" & Text1(0).Text)
        If result Then
            cnnId = oMPCallback.GetConnectionId(Text1(0).Text)
            Text1(1).Text = cnnId
        End If
    
    End If
End Sub

Private Sub Command2_Click()
Dim path As String
CommonDialog1.Filter = "images (*.bmp)|*.bmp|All files (*.*)|*.*"
CommonDialog1.DefaultExt = "bmp"
CommonDialog1.DialogTitle = "Select File"
CommonDialog1.ShowOpen
Text1(3).Text = CommonDialog1.filename
Me.Image1.Picture = LoadPicture(Text1(3).Text)

End Sub

Private Sub Form_Load()
Dim result As Boolean
Dim cnnId As String
Set oMPCallback = CreateObject("WrapperMercadoPagoAPI.ClassInterop")
Text1(0).Text = StoreID
Text1(2).Text = "https://7723-200-105-93-58.ngrok-free.app"
cnnId = FindValueFromFile("C:\ProgramData\Softland\Logs\Log_202403\TempMPagoLog.log", "merchant_order", "id")


End Sub
Private Function GetTopicId(ByVal topic As String, ByVal keyName As String, ByVal sKeyNames As String, listSep As String, valueSep As String) As String '#NOERRORHANDLING
    Dim keyValue As String
    Dim topicId As Currency
    Dim topicMaxId As Currency
    Dim bSeguir As Boolean
    Dim vec() As Variant
    Dim varItem As Variant
    On Error GoTo EH
    sKeyValues = ""
    bSeguir = True
    Split sKeyNames, "|", 255, vec
    For Each varItem In vec()
        If InStr(varItem, topic) > 0 Then
            sKeyNames = Left(Mid(varItem, InStr(varItem, keyName)), InStr(varItem, listSep))
            eachField = Left(sKeyNames, InStr(sKeyNames, valueSep) - 1)
            If eachField = keyName Then '#1
                keyValue = Mid(sKeyNames, InStr(sKeyNames, valueSep) + 1)
                If IsNumeric(keyValue) Then '#2
                    topicId = CCur(keyValue)
                    If topicId > topicMaxId Then '#3
                        topicMaxId = topicId
                    End If '#3
                End If '#2
            End If '#1
        End If
    Next
'    Do While Len(sKeyNames) And bSeguir
'        sKeyNames = Left(Mid(sKeyNames, InStr(sKeyNames, keyName)), InStr(sKeyNames, listSep))
'        eachField = Left(sKeyNames, InStr(sKeyNames, valueSep) - 1)
'        If eachField = keyName Then
'            sKeyValues = Mid(sKeyNames, InStr(sKeyNames, valueSep) + 1)
'            bSeguir = False
'        End If
'    Loop
'
    GetKeyValues = CStr(topicMaxId)
 
    Exit Function
EH:

End Function


Private Sub Form_Unload(Cancel As Integer)
    Set oMPCallback = Nothing
End Sub
Private Function FindValueFromFile(fullPath As String, keyTopic As String, keyValue As String) As String '#NOERRORHANDLING
    Dim textLine As String
    On Error GoTo EH
    

    If ReadEntireTextFile(fullPath, textLine) <> 1 Then
        If InStr(textLine, keyTopic) > 0 Then
            'FindValueFromFile = GetKeyValues(keyValue, textLine, LIST_SEP, VAL_SEP)
            FindValueFromFile = GetTopicId("merchant_order", keyValue, textLine, "#", "=")
        End If
    End If
    
  Exit Function
EH:
    On Error Resume Next
End Function
Private Function ReadEntireTextFile(fullPathFile As String, ByRef textFile As String) As Integer
    On Error GoTo EH
    Dim iFile As Integer
    iFile = FreeFile
    Open fullPathFile For Input As #iFile
    textFile = StrConv(InputB(LOF(iFile), iFile), vbUnicode)
    Close #iFile
Exit Function
EH:
    ReadEntireTextFile = STOP_IT
    On Error Resume Next
    Close #iFile
End Function
Private Function Split(ByVal sText As String, sSplitChar As String, iMaxLen As Integer, ByRef vec() As Variant)
 
    On Error GoTo EH
    Dim iPos As Integer
    Dim x As Integer
    Dim i As Integer
    Dim iMaxLenArray As Integer
 
 
        If Len(sSplitChar) > 0 Then
            Do
                i = i + 1
                iPos = InStr(1, sText, sSplitChar)
                If iPos > 0 Then
                    ReDim Preserve vec(x)
                    vec(x) = Mid(sText, 1, iPos - 1)
                    sText = Mid(sText, iPos + 1, Len(sText) - 1)
                    x = x + 1
                End If
            Loop Until (Len(sText) = 0 Or iPos = 0)
             
        End If

Exit Function
 
EH:
 
End Function
