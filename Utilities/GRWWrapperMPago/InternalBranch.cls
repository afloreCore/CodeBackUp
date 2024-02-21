VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InternalBranch"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
'id = item.id,
'category = item.category,
'external_id = item.external_id,
'date_created = item.date_created,
'external_store_id = item.external_store_id,
'date_last_updated = item.date_last_updated,
'fixed_amount = item.fixed_amount,
'linkImageQr = (item.qr == null ? string.Empty : item.qr.image.ToString()),
'name = item.name,
'site = Item.site
Public id As Long
Public linkImageQr As String
Public status As String
Public date_created As Date
Public date_last_updated As Date
Public name As String
Public fixed_amount As Boolean
Public store_id As Long
Public external_store_id As String
Public external_id As String
Public site As String
Public category As InterfaceComNetCore.BranchCategories
Public itemdesc As String
Public itemunit As String
Public TimeOut As Double
