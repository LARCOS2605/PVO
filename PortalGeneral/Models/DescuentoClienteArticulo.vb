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

Partial Public Class DescuentoClienteArticulo
    Public Property idArticuloClienteDescuento As Integer
    Public Property idProducto As Nullable(Of Integer)
    Public Property idCustomer As Nullable(Of Integer)
    Public Property PrecioDescuento As Nullable(Of Decimal)

    Public Overridable Property Catalogo_Productos As Catalogo_Productos
    Public Overridable Property Customers As Customers

End Class