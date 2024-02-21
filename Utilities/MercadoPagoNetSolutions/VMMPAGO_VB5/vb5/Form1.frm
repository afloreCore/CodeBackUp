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
Implements ICallbackInterop
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
        Set oMPCallback.ReturnValue = Me
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
Text1(2).Text = "https://b25f-200-105-93-58.ngrok-free.app"

End Sub

Private Sub Form_Unload(Cancel As Integer)
    Set oMPCallback = Nothing
End Sub

Private Function ICallbackInterop_ReturnValue(ByVal topic As String, ByVal topicId As String) As Boolean
    Text2.Text = Text2.Text & "topic: " & topic & "; Id: " & topicId & vbCrLf
End Function
