'------------------------------------------------------------------------------
' <auto-generated>
'     Este código se generó a partir de una plantilla.
'
'     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
'     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class Resp_DetalleSalesOrder
    Public Property idRespDetalleSalesOrder As Integer
    Public Property idRespSalesOrder As Nullable(Of Integer)
    Public Property idProducto As Nullable(Of Integer)
    Public Property Cantidad As Nullable(Of Decimal)
    Public Property UbicacionAlmacen As String
    Public Property Importe As Nullable(Of Decimal)
    Public Property Total As Nullable(Of Decimal)
    Public Property NumLote As String
    Public Property IssuesInventory As String
    Public Property CantidadEntregada As Nullable(Of Decimal)
    Public Property idEstatus As Nullable(Of Integer)

    Public Overridable Property Catalogo_Productos As Catalogo_Productos
    Public Overridable Property Estatus As Estatus
    Public Overridable Property Resp_SalesOrder As Resp_SalesOrder

End Class