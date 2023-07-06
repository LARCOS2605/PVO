Public Class InvoiceViewModel
    Public Property idInvoice As Integer
    Public Property NS_InternalID As Nullable(Of Integer)
    Public Property NS_ExternalID As String
    Public Property idSalesOrder As Nullable(Of Integer)
    Public Property idEstatus As Nullable(Of Integer)
    Public Property FechaCreacion As Nullable(Of Date)
    Public Property Subtotal As Nullable(Of Decimal)
    Public Property Total_Impuestos As Nullable(Of Decimal)
    Public Property Total As Nullable(Of Decimal)
    Public Property ImporteAdeudado As Nullable(Of Decimal)
    Public Property idCustomer As Nullable(Of Integer)
    Public Property idUsuario As Nullable(Of Integer)
    Public Property NombreCliente As String
    Public Property MetodoPagoSAT As String
    Public Property Fecha As String

End Class
