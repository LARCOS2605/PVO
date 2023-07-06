Public Class DetallesPagoViewMap
    Public Property FolioTransaccion As String
    Public Property FechaCreacion As String
    Public Property Cliente As String
    Public Property NotaPago As String
    Public Property Timbrado As String

    '' Timbrado Datos
    Public Property Serie_Timbrado As String
    Public Property Folio_Timbrado As String
    Public Property FechaTimbrado As String
    Public Property UUID As String
    Public Property NS_ID_pdf_pago As String
    Public Property NS_ID_xml_pago As String
    Public Property l_DetallePago As List(Of DetallesConceptoPagoViewMap)
End Class

Public Class DetallesConceptoPagoViewMap
    Public Property FolioFactura As String
    'Public Property Internal_ID As String
    Public Property ImporteOriginal As Decimal
    Public Property ImportePagar As Decimal
    Public Property PagoAplicado As Decimal
End Class
