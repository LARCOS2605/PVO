Public Class DetalleEstimacionMapViewModel
    Public Property idDetalleEstimacion As Integer
    Public Property idEstimacion As Nullable(Of Integer)
    Public Property idProducto As Nullable(Of Integer)
    Public Property Cantidad As Nullable(Of Decimal)
    Public Property EstatusEntrega As String
    Public Property UbicacionAlmacen As String
    Public Property Importe As Nullable(Of Decimal)
    Public Property Total As Nullable(Of Decimal)
    Public Property NumLote As String
    Public Property IssuesInventory As String
    Public Property DescripcionProducto As String
    Public Property ClaveProducto As String
    Public Property CantidadEntregada As Nullable(Of Decimal)
End Class
