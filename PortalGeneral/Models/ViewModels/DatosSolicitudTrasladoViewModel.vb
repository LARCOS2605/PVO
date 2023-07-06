Public Class DatosSolicitudTrasladoViewModel
    Public Property FechaCreacion As String
    Public Property NombreTransaccion As String
    Public Property AlmacenOrigen As String
    Public Property AlmacenDestino As String
    Public Property Nota As String

    Public Property l_Detalle As List(Of DetalleSolicitudTrasladoViewModel)
End Class

Public Class DetalleSolicitudTrasladoViewModel
    Public Property Articulo As String
    Public Property Descripcion As String
    Public Property CantidadEnviada As String
    Public Property CantidadSolicitud As String
    Public Property CantidadRecibida As String
End Class