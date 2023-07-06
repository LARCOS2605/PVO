Public Class ConsultarOrdenesVentaViewModel
    Public Property Customer As String
    Public Property UbicacionesAlmacen As String
    Public Property UbicacionVenta As String
    Public Property MetodoPagoSAT As String
    Public Property Memo As String
    Public Property FechaAlta As DateTime = Date.Now
    Public Property GenerarFacturaDirecta As Boolean = False

End Class
