Imports NetsuiteLibrary.Clases

Public Class GeneraReporteIngresosViewModel
    Public Property FechaInicio As DateTime
    Public Property FechaFin As DateTime
    Public Property totalFechaPago As Double
    Public Property totalMoneda As Double
    Public Property totalCobrado As Double
    Public Property totalPagos As Double

    Public Property l_Detalle As List(Of MapCorteCaja)
    Public Property l_CorteCaja As List(Of MapCorteCajaRep)

    ''Total Metodo Pago
    Public Property NumTotalFacturas As String
    Public Property Efectivo As Double
    Public Property Cheque As Double
    Public Property Credito As Double
    Public Property TarjetaCredito As Double
    Public Property TarjetaDebito As Double
    Public Property Transferencia As Double


End Class
