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

Partial Public Class Catalogo_CuentasBancarias
    Public Property idCuentaBancaria As Integer
    Public Property idMetodoPago As Nullable(Of Integer)
    Public Property Nombre As String
    Public Property NumCuenta As String
    Public Property idCatalogoBanco As Nullable(Of Integer)

    Public Overridable Property Catalogo_MetodosPago As Catalogo_MetodosPago
    Public Overridable Property Catalogo_Bancos As Catalogo_Bancos

End Class
