Imports System.ComponentModel
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports PortalGeneral.Controllers.Areas

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WS_Jobs
    Inherits System.Web.Services.WebService
    Private SincronizarInformacionGeneral As Thread
    Private SincronizarClientesAutomaticamente As Thread
    Private Sincronizarstock As Thread


    <AllowAnonymous()>
    <WebMethod()>
    Public Function SincronizacionGeneral() As String
        Dim MetodosNetsuite As New NetsuiteController

        SincronizarInformacionGeneral = New Thread(AddressOf MetodosNetsuite.SincronizarDatos)
        SincronizarInformacionGeneral.Start()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Async Function SincronizarStockAlmacenesAsync() As Task(Of String)
        Dim MetodosNetsuite As New NetsuiteController

        Await MetodosNetsuite.SincronizarStockAlmacenes()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Function SincronizarClientes() As String
        Dim MetodosNetsuite As New NetsuiteController

        SincronizarClientesAutomaticamente = New Thread(AddressOf MetodosNetsuite.SincronizarClientesExistentes)
        SincronizarClientesAutomaticamente.Start()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Async Function SincronizarPreciosAsync() As Task(Of String)
        Dim MetodosNetsuite As New NetsuiteController

        Await MetodosNetsuite.SincronizarNivelPrecio()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Async Function SincronizarProductosVentaAsync() As Task(Of String)
        Dim MetodosNetsuite As New NetsuiteController

        Await MetodosNetsuite.SincronizarProductosVenta()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Async Function SincronizarNivelesPrecioAsync() As Task(Of String)
        Dim MetodosNetsuite As New NetsuiteController

        Await MetodosNetsuite.SincronizarNivelPrecio()

        Return "Se ha ejecutado el proceso con exito!"
    End Function

    <AllowAnonymous()>
    <WebMethod()>
    Public Async Function SincronizarFacturasClientesAsync() As Task(Of String)
        Dim MetodosNetsuite As New NetsuiteController

        Await MetodosNetsuite.SincronizarFacturasPorClienteJob()

        Return "Se ha ejecutado el proceso con exito!"
    End Function


End Class