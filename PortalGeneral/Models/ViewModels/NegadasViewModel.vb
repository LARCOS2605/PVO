Public Class NegadasViewModel
    Public Property Customer As String
    Public Property UbicacionesAlmacen As String
    Public Property UbicacionVenta As String
    Public Property MetodoPagoSAT As String
    Public Property FormaPagoSAT As String
    Public Property UsoCFDI As String
    Public Property Memo As String
    Public Property FechaAlta As DateTime = Date.Now
    Public Property GenerarFacturaDirecta As Boolean = False
    Public Property GenerarVentaDirecta As Boolean = False
    Public Property TokenCarrito As String

End Class
