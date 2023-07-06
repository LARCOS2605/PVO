Public Class NegadasMapViewModel
    Public Property idEstimacion As Integer
    Public Property idCustomer As Nullable(Of Integer)
    Public Property idUbicacion As Nullable(Of Integer)
    Public Property Nota As String
    Public Property FormaPago As String
    Public Property MetodoDePago As String
    Public Property UsoCFDI As String
    Public Property FechaCreacion As Nullable(Of Date)
    Public Property idUsuario As Nullable(Of Integer)
    Public Property SubTotal As Nullable(Of Decimal)
    Public Property Descuento As Nullable(Of Decimal)
    Public Property TotalImpuestos As Nullable(Of Decimal)
    Public Property VAT_MX As Nullable(Of Decimal)
    Public Property Total As Nullable(Of Decimal)
    Public Property NS_InternalID As Nullable(Of Integer)
    Public Property NS_ExternalID As String
    Public Property Nombre_Mostrador As String
    Public Property Nombre_Cliente As String
    Public Property Fecha As String
    Public Property Estatus As String
    Public Property MetodoPagoSAT As String
    Public Property FormaPagoSAT As String
    Public Property ClaveCliente As String
    Public Property idMetodoPagoSAT As Nullable(Of Integer)
    Public Property idUsoCFDI As Nullable(Of Integer)
    Public Property idFormaPago As Nullable(Of Integer)
    Public Property l_detalle As New List(Of DetalleNegadasMapViewModel)
    Public Property NoFactura As String
    Public Property NS_ID_Invoice As String
    Public Property ValInvoice As String

    Public Property NS_Val_INV As String
    Public Property UUID As String
    Public Property NS_ID_pdf As String
    Public Property NS_ID_xml As String
    Public Property TieneFactura As String
End Class
