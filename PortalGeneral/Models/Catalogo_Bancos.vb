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

Partial Public Class Catalogo_Bancos
    Public Property idCatalogoBanco As Integer
    Public Property NumIdentificacion As String
    Public Property Nombre As String
    Public Property RFC As String
    Public Property NS_InternalID As Nullable(Of Integer)

    Public Overridable Property AplicacionPago As ICollection(Of AplicacionPago) = New HashSet(Of AplicacionPago)
    Public Overridable Property Catalogo_CuentasBancarias As ICollection(Of Catalogo_CuentasBancarias) = New HashSet(Of Catalogo_CuentasBancarias)

End Class
