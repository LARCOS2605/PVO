Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Entity
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Mvc
Imports NetsuiteLibrary.SuiteTalk
Imports NetsuiteLibrary.SuiteTalk_Helpers
Imports NetsuiteLibrary.SuiteServlet_Connection
Imports NetsuiteLibrary.Clases
Imports PortalGeneral
Imports Renci.SshNet
Imports Renci.SshNet.Sftp
Imports SAT.Services.ConsultaCFDIService
Imports SW.Services.Status
Imports NetsuiteLibrary
Imports WebMatrix.WebData
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports Newtonsoft.Json

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class PVONetSuiteController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_Search As New SearchHelper
        Public H_Transfer As New TransferOrderHelper
        Public H_Invoice As New InvoiceHelper
        Public H_Servlet As New ConnectionServer
#End Region

#Region "Vistas"
        Public Async Function ConsultarFacturasNetsuite() As Task(Of ActionResult)
            Await GeneraViewbags()
            Return View()
        End Function
#End Region

#Region "Procesos"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function ConsultarFacturasCliente(ByVal ns_id As String) As Task(Of JsonResult)

            If ns_id = "" Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El Cliente seleccionado no es valido."))
            End If

            Dim Respuesta As RespuestaServicios = H_Search.GetListInvoiceForCustomerAdvancedVer(ns_id)

            If Respuesta.Estatus = "Exito" Then

                Dim l_Facturas As List(Of MapPVONetsuite) = Respuesta.Valor

                For Each ValidaPVO In l_Facturas
                    Dim ExistePVO = Await (From n In db.Invoice_SO Where n.NS_InternalID = ValidaPVO.InternalID).FirstOrDefaultAsync()

                    If IsNothing(ExistePVO) Then
                        ValidaPVO.ExistePVO = "False"
                    Else
                        ValidaPVO.ExistePVO = "True"
                    End If
                Next

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Facturas))
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function RecuperarFacturaPendiente(ByVal ns_id As String) As Task(Of JsonResult)

            If ns_id = "" Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La Factura para recuperar, no tiene un identificativo valido."))
            End If

            Dim ValidarFactura = Await (From n In db.Invoice_SO Where n.NS_InternalID = ns_id).FirstOrDefaultAsync()

            If IsNothing(ValidarFactura) Then

                Dim RegistrarFactura = H_Invoice.GetInvoiceByID(ns_id)

                Dim RegInvoice As New Invoice_SO

                RegInvoice.NS_InternalID = RegistrarFactura.internalId
                RegInvoice.NS_ExternalID = RegistrarFactura.tranId
                RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                RegInvoice.FechaCreacion = RegistrarFactura.tranDate
                RegInvoice.idCustomer = (From n In db.Customers Where n.NS_InternalID = RegistrarFactura.entity.internalId Select n.idCustomer).FirstOrDefault()
                RegInvoice.Subtotal = RegistrarFactura.subTotal
                RegInvoice.Total_Impuestos = RegistrarFactura.taxTotal
                RegInvoice.Total = RegistrarFactura.total
                RegInvoice.ImporteAdeudado = RegistrarFactura.amountRemaining

                Dim validaDatos = 0

                If Not IsNothing(RegistrarFactura.createdFrom) Then
                    validaDatos = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
                End If

                If Not validaDatos = 0 Then
                    RegInvoice.idSalesOrder = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()

                    Dim CambioEstatus = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId).FirstOrDefault()

                    CambioEstatus.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "SO_Facturada" Select n.idEstatus).FirstOrDefault()
                End If

                db.Invoice_SO.Add(RegInvoice)
                db.SaveChanges()

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "La factura ya existe en el PVO.", New With {.NS_ID = ns_id}))

            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La factura ya existe en el PVO."))
            End If

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function ConsultarHistorialPagos(ByVal ns_id As String) As Task(Of JsonResult)

            If ns_id = "" Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El Factura seleccionada no es valida."))
            End If

            Dim Respuesta As RespuestaServicios = H_Search.GetListPaymentForInvoice(ns_id)

            If Respuesta.Estatus = "Exito" Then

                Dim l_Pagos As List(Of MapFacturasPagos) = Respuesta.Valor

                If l_Pagos.Count = 0 Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se han encontrado pagos disponibles para mostrar."))
                Else
                    For Each ValidaPVO In l_Pagos
                        Dim ExistePVO = Await (From n In db.Invoice_SO Where n.NS_InternalID = ValidaPVO.InternalID).FirstOrDefaultAsync()
                        ValidaPVO.fecha = ValidaPVO.FechaEmision.ToString("dd/MM/yyyy")
                    Next

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Pagos))
                End If
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

        End Function

        Function ImprimirPago(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                If id = "" Then
                    Flash.Instance.Error("Hubo un problema al generar el PDF.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("Pago_{0}.pdf", id)
                    Dim Path_Impresion = "C:\temp\" + NombreArchivo
                    Dim baseFile = GenerarImpresion.Valor

                    Dim bytes = System.Convert.FromBase64String(baseFile)
                    Dim writer As New System.IO.BinaryWriter(IO.File.Open(Path_Impresion, IO.FileMode.Create))
                    writer.Write(bytes)
                    writer.Close()

                    Dim fileStream = New FileStream(Path_Impresion,
                   FileMode.Open,
                   FileAccess.Read)
                    Dim fsResult = New FileStreamResult(fileStream, "application/pdf")
                    fsResult.FileDownloadName = Path.GetFileName(Path_Impresion)
                    Return fsResult

                Else
                    Flash.Instance.Error("Hubo un problema al generar el PDF.")
                    Return RedirectToAction("Index", "Home")
                End If

            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If
        End Function
#End Region

#Region "Extras"
        Public Async Function GeneraViewbags() As Threading.Tasks.Task
            Dim Select_Customer As New List(Of SelectListItem)

            Dim l_Customers = Await Consulta.ConsultarListaCustomers()

            For Each RegistroCustomer In l_Customers
                Dim texto = RegistroCustomer.NS_ExternalID + " - " + RegistroCustomer.Nombre
                Select_Customer.Add(New SelectListItem With {.Text = texto, .Value = RegistroCustomer.NS_InternalID})
            Next

            ViewBag.Customer = Select_Customer
        End Function
#End Region

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

    End Class
End Namespace
