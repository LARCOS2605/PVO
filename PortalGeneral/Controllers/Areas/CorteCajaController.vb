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
Imports Rotativa
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class CorteCajaController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_Search As New SearchHelper
#End Region

#Region "Vistas"
        Public Function GenerarReporteCorte() As ActionResult

            'Await GeneraViewBags()

            Return View()
        End Function
        Public Function GenerarReporteIngresosGeneral() As ActionResult

            'Await GeneraViewBags()

            Return View()
        End Function
        Public Function ConsultarEstatusNetsuite() As ActionResult

            Return View()
        End Function
        Public Function GenerarReporteKardexArticulo() As ActionResult
            Return View()
        End Function
#End Region

#Region "Procesos"
        <HttpPost>
        Public Function GenerarReporteKardexArticulo(ByVal model As ConsultaKardexArticulo) As ActionResult

            Dim l_Ventas As New List(Of MapTransferenciaOrden)
            Dim fromDate = ""
            Dim toDate = ""
            Dim l_respuesta As New List(Of Consulta_Pagos)
            If model.FechaInicio IsNot Nothing And model.FechaFin IsNot Nothing Then
                Dim dif As Integer = DateDiff(("d"), model.FechaInicio, model.FechaFin)
                fromDate = model.FechaInicio
                model.FechaFin = model.FechaFin.Value.AddDays(1).AddMinutes(-1)
                If dif < 0 Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
                End If
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

            Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

            If IsNothing(ValidaUsuario.MostradorPref) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador."))
            Else

                Dim BuscarDatosMRP As New BusquedaKardex

                BuscarDatosMRP.FechaDesde = model.FechaInicio
                BuscarDatosMRP.FechaHasta = model.FechaFin
                BuscarDatosMRP.Location_ID = ValidaUsuario.Ubicaciones.NS_InternalID
                BuscarDatosMRP.Articulo = model.ClaveArticulo

                Dim Datos As RespuestaServicios = H_Search.GetListKardexForLocation(BuscarDatosMRP)

                If Datos.Estatus = "Exito" Then
                    l_respuesta = Datos.Valor
                    Dim l_metodoPagos As List(Of Catalogo_FormasPagoSAT) = db.Catalogo_FormasPagoSAT.ToList()

                    For Each RegVentas In l_respuesta

                        RegVentas.MetodoPago = (From n In l_metodoPagos Where n.NS_InternalID = RegVentas.MetodoPago Select n.Descripcion).FirstOrDefault()
                        RegVentas.fecha = RegVentas.FechaEmision.ToString("dd/MM/yyyy")

                    Next

                End If
                'GetListKardexForLocation
                'BusquedaKardex
            End If
            Return View()
        End Function

        <HttpPost>
        Public Async Function ConsultarCorteCajaRep(ByVal model As BusquedaCorteCaja) As Task(Of ActionResult)

            'Await GeneraViewBags()

            Dim fromDate = ""
            Dim toDate = ""

            fromDate = model.FechaDesde
            model.FechaHasta = model.FechaHasta.AddDays(1)

            Dim dif As Integer = DateDiff(("d"), model.FechaDesde, model.FechaHasta)

            If dif < 0 Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

            Dim GetMostradorUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Ubicaciones.NS_InternalID).FirstOrDefaultAsync()

            If IsNothing(GetMostradorUsuario) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El usuario no cuenta con un mostrador seleccionado."))
            End If

            Dim RegCaja As New BusquedaCorteCaja

            RegCaja.FechaDesde = model.FechaDesde
            RegCaja.FechaHasta = model.FechaHasta
            RegCaja.Location_ID = GetMostradorUsuario

            Dim Resultado = H_Search.GetListInvoiceForLocationAdvancedVer(RegCaja)

            For Each ValidarFormaPago As MapCorteCajaRep In Resultado.Valor
                Dim DescripcionFormaPago = (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = ValidarFormaPago.FormaPago).FirstOrDefault()

                ValidarFormaPago.FormaPago = DescripcionFormaPago.Descripcion
                ValidarFormaPago.Fechas = ValidarFormaPago.Fecha.ToString()
            Next

            If Resultado.Estatus = "Exito" Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", Resultado.Valor))
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al realizar su peticion, Detalles: " + Resultado.Mensaje))
            End If

        End Function

        <HttpPost>
        Public Async Function ConsultarCorteCaja(ByVal model As BusquedaCorteCaja) As Task(Of ActionResult)

            'Await GeneraViewBags()
            ''Cambio linea codigo ejemplo
            ''Se agrego otro comentario de prueba
            Dim fromDate = ""
            Dim toDate = ""

            fromDate = model.FechaDesde.ToString("dd/MM/yyyy") + " 00:00:00"
            Dim dateConvert = Convert.ToDateTime(fromDate)
            model.FechaHasta = model.FechaHasta.AddDays(1).AddMinutes(-1)

            Dim dif As Integer = DateDiff(("d"), model.FechaDesde, model.FechaHasta)

            If dif < 0 Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

            Dim GetMostradorUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Ubicaciones.NS_InternalID).FirstOrDefaultAsync()

            If IsNothing(GetMostradorUsuario) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El usuario no cuenta con un mostrador seleccionado."))
            End If

            Dim RegCaja As New BusquedaCorteCaja

            RegCaja.FechaDesde = model.FechaDesde.AddMinutes(-1)
            RegCaja.FechaHasta = model.FechaHasta
            RegCaja.Location_ID = GetMostradorUsuario

            ''DEBIDO A LAS LIMITACIONES DE NETSUITE, SE TENDRA QUE PROCESAR DOS LISTAS PARA COMPLETAR LOS DATOS DEL REPORTE
            Dim Resultado = H_Search.GetListPaymentForLocationAdvancedVer(RegCaja)

            If IsNothing(Resultado.Valor) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al realizar su peticion, Detalles: " + "La busqueda no arrojo datos."))
            End If

            'Dim l_p_lista As List(Of MapCorteCaja) = ResultadoVer2.Valor

            Dim l_Reporte As New List(Of MapCorteCaja)

            For Each ConsultaResultado As MapCorteCaja In Resultado.Valor

                'Dim s = (From n In l_p_lista Where ConsultaResultado.FolioPago = n.FolioPago).FirstOrDefault()

                Dim RegMapeo As New MapCorteCaja

                RegMapeo.ClaveCliente = ConsultaResultado.ClaveCliente
                RegMapeo.NombreCliente = ConsultaResultado.NombreCliente
                RegMapeo.FolioFactura = ConsultaResultado.FolioFactura
                RegMapeo.DiasCredito = ""
                RegMapeo.FechaEmision = ConsultaResultado.FechaEmision
                RegMapeo.FechaVencimiento = ConsultaResultado.FechaVencimiento
                RegMapeo.MontoFactura = ConsultaResultado.MontoFactura
                RegMapeo.MontoPago = ConsultaResultado.MontoPago
                RegMapeo.FolioPago = ConsultaResultado.FolioPago
                RegMapeo.MetodoPago = (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = ConsultaResultado.MetodoPago Select n.Descripcion).FirstOrDefault()
                RegMapeo.CuentaBancaria = (From n In db.Catalogo_Cuentas Where n.NS_InternalID = ConsultaResultado.CuentaBancaria Select n.Descripcion).FirstOrDefault()

                If (ConsultaResultado.metodopagoFactura <> "SNM") Then
                    RegMapeo.metodopagoFactura = (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = ConsultaResultado.metodopagoFactura Select n.Descripcion).FirstOrDefault()
                Else
                    RegMapeo.metodopagoFactura = "Sin Formas de Pago"
                End If

                RegMapeo.fecha = ConsultaResultado.FechaEmision.ToString("dd/MM/yyyy")
                RegMapeo.fechacre = ConsultaResultado.FechaCreacion.ToString("dd/MM/yyyy")

                l_Reporte.Add(RegMapeo)
            Next

            If Resultado.Estatus = "Exito" Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Reporte))
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al realizar su peticion, Detalles: " + Resultado.Mensaje))
            End If

        End Function

        Public Async Function GenerarReportePDF(ByVal model As BusquedaCorteCaja) As Task(Of ActionResult)

            Try
                If model.ReporteDia <> "" Then
                    Dim FechaHoy = Date.Now
                    model.FechaDesde = FechaHoy
                    model.FechaHasta = FechaHoy.AddDays(1)
                Else
                    model.FechaHasta = model.FechaHasta.AddDays(1)
                End If


                Dim dif As Integer = DateDiff(("d"), model.FechaDesde, model.FechaHasta)

                If dif < 0 Then
                    Flash.Instance.Error("Debe definir una rango de fechas válido para poder hacer la consulta")
                    Return RedirectToAction("GenerarReporteIngresosGeneral", "CorteCaja")
                End If

                Dim GetMostradorUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Ubicaciones.NS_InternalID).FirstOrDefaultAsync()

                If IsNothing(GetMostradorUsuario) Then
                    Flash.Instance.Error("El usuario no cuenta con un mostrador seleccionado.")
                    Return RedirectToAction("GenerarReporteIngresosGeneral", "CorteCaja")
                End If


                Dim nombrePDF = String.Format("Reporte_Ingresos_{0}.pdf", Date.Now.ToString("ddMMyyyy_HHmmss"))
                Return New ActionAsPdf("GetReporteIngresos", New With {.fechaInicio = model.FechaDesde, .fechafin = model.FechaHasta, .mostrador = GetMostradorUsuario, .vista = False}) With {.FileName = nombrePDF, .PageOrientation = Rotativa.Options.Orientation.Landscape, .PageSize = Rotativa.Options.Size.A4}

            Catch ex As Exception
                Return RedirectToAction("GenerarReporteIngresosGeneral", "CorteCaja")
            End Try

        End Function

        Public Async Function GenerarReporteCorteCajaPDF(ByVal model As BusquedaCorteCaja) As Task(Of ActionResult)

            Try
                If model.ReporteDia <> "" Then
                    Dim FechaHoy = Date.Now
                    model.FechaDesde = FechaHoy
                    model.FechaHasta = FechaHoy.AddDays(1)
                Else
                    model.FechaHasta = model.FechaHasta.AddDays(1)
                End If


                Dim dif As Integer = DateDiff(("d"), model.FechaDesde, model.FechaHasta)

                If dif < 0 Then
                    Flash.Instance.Error("Debe definir un rango de fechas válido para poder hacer la consulta")
                    Return RedirectToAction("GenerarReporteCorte", "CorteCaja")
                End If

                'HttpContext

                Dim GetMostradorUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Ubicaciones.NS_InternalID).FirstOrDefaultAsync()

                If IsNothing(GetMostradorUsuario) Then
                    Flash.Instance.Error("El usuario no cuenta con un mostrador seleccionado.")
                    Return RedirectToAction("GenerarReporteCorte", "CorteCaja")
                End If


                Dim nombrePDF = String.Format("Reporte_CorteCaja_{0}.pdf", Date.Now.ToString("ddMMyyyy_HHmmss"))
                Return New ActionAsPdf("GetReporteCorteCaja", New With {.fechaInicio = model.FechaDesde, .fechafin = model.FechaHasta, .mostrador = GetMostradorUsuario, .vista = False}) With {.FileName = nombrePDF, .PageOrientation = Rotativa.Options.Orientation.Landscape, .PageSize = Rotativa.Options.Size.A4}

            Catch ex As Exception
                Return RedirectToAction("GenerarReporteCorte", "CorteCaja")
            End Try


        End Function
#End Region

#Region "Funciones Auxiliares"
        Public Async Function GeneraViewBags() As Threading.Tasks.Task
            Dim Select_Ubicaciones As New List(Of SelectListItem)

            Dim l_Ubicaciones = Await Consulta.ConsultarUbicacionesAlmacenActivos()

            For Each RegistroUbicaciones In l_Ubicaciones
                Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.NS_InternalID})
            Next

            ViewBag.Mostrador = Select_Ubicaciones

        End Function
#End Region

#Region "Reportes Impresos"
        Function GetReporteIngresos(ByVal fechaInicio As Date, ByVal fechafin As Date, ByVal mostrador As String) As ActionResult
            Dim RegCaja As New BusquedaCorteCaja

            RegCaja.FechaDesde = fechaInicio
            RegCaja.FechaHasta = fechafin
            RegCaja.Location_ID = mostrador

            ''DEBIDO A LAS LIMITACIONES DE NETSUITE, SE TENDRA QUE PROCESAR DOS LISTAS PARA COMPLETAR LOS DATOS DEL REPORTE
            Dim Resultado = H_Search.GetListPaymentForLocationAdvancedVer(RegCaja)

            If IsNothing(Resultado.Valor) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al realizar su peticion, Detalles: " + "La busqueda no arrojo datos."))
            End If

            Dim l_Reporte As New List(Of MapCorteCaja)

            For Each ConsultaResultado As MapCorteCaja In Resultado.Valor

                Dim RegMapeo As New MapCorteCaja

                RegMapeo.ClaveCliente = ConsultaResultado.ClaveCliente
                RegMapeo.NombreCliente = ConsultaResultado.NombreCliente
                RegMapeo.FolioFactura = ConsultaResultado.FolioFactura
                RegMapeo.DiasCredito = ""
                RegMapeo.FechaEmision = ConsultaResultado.FechaEmision
                RegMapeo.FechaVencimiento = ConsultaResultado.FechaVencimiento
                RegMapeo.MontoFactura = ConsultaResultado.MontoFactura
                RegMapeo.MontoPago = ConsultaResultado.MontoPago
                RegMapeo.FolioPago = ConsultaResultado.FolioPago
                RegMapeo.MetodoPago = (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = ConsultaResultado.MetodoPago Select n.Descripcion).FirstOrDefault()
                RegMapeo.CuentaBancaria = (From n In db.Catalogo_Cuentas Where n.NS_InternalID = ConsultaResultado.CuentaBancaria Select n.Descripcion).FirstOrDefault()

                l_Reporte.Add(RegMapeo)
            Next

            Dim reporteCorteCaja As New GeneraReporteIngresosViewModel

            reporteCorteCaja.FechaInicio = fechaInicio
            reporteCorteCaja.FechaFin = fechafin
            reporteCorteCaja.l_Detalle = l_Reporte
            reporteCorteCaja.totalFechaPago = Math.Round(l_Reporte.Sum(Function(x) x.MontoPago), 2)
            reporteCorteCaja.totalMoneda = Math.Round(l_Reporte.Sum(Function(x) x.MontoPago), 2)
            reporteCorteCaja.totalPagos = l_Reporte.Count
            Return View(reporteCorteCaja)
        End Function

        Function GetReporteCorteCaja(ByVal fechaInicio As Date, ByVal fechafin As Date, ByVal mostrador As String) As ActionResult
            Dim RegCaja As New BusquedaCorteCaja
            Dim l_detalle As New List(Of MapCorteCajaRep)
            Dim l_datos As New List(Of MapCorteCajaRep)

            RegCaja.FechaDesde = fechaInicio
            RegCaja.FechaHasta = fechafin
            RegCaja.Location_ID = mostrador

            Dim Resultado = H_Search.GetListInvoiceForLocationAdvancedVer(RegCaja)
            l_detalle = Resultado.Valor

            For Each MapeoDatos In l_detalle
                MapeoDatos.Descripcion_MetodoPago = (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = MapeoDatos.FormaPago Select n.Descripcion).FirstOrDefault()

                If MapeoDatos.NS_ID_OS <> "Sin SO" Then
                    Dim NombreEmpleado = (From n In db.SalesOrder Where n.NS_InternalID = MapeoDatos.NS_ID_OS Select n.Usuarios.Nombre).FirstOrDefault()

                    If IsNothing(NombreEmpleado) Then
                        MapeoDatos.Usuario = "No Localizado"
                    Else
                        MapeoDatos.Usuario = NombreEmpleado
                    End If
                Else
                    MapeoDatos.Usuario = "No Localizado"
                End If


                'MapeoDatos.Fecha = MapeoDatos.Fecha.ToString("dd/MM/yyyy")

                l_datos.Add(MapeoDatos)
            Next

            Dim reporteCorteCaja As New GeneraReporteIngresosViewModel

            reporteCorteCaja.FechaInicio = fechaInicio
            reporteCorteCaja.FechaFin = fechafin
            reporteCorteCaja.l_CorteCaja = Resultado.Valor

            reporteCorteCaja.totalFechaPago = Math.Round(l_datos.Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.Efectivo = Math.Round(l_datos.Where(Function(z) z.FormaPago = 1).Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.Transferencia = Math.Round(l_datos.Where(Function(z) z.FormaPago = 3).Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.TarjetaCredito = Math.Round(l_datos.Where(Function(z) z.FormaPago = 4).Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.TarjetaDebito = Math.Round(l_datos.Where(Function(z) z.FormaPago = 23).Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.Credito = Math.Round(l_datos.Where(Function(z) z.FormaPago = 28).Sum(Function(x) x.TotalFactura), 2)
            reporteCorteCaja.Cheque = Math.Round(l_datos.Where(Function(z) z.FormaPago = 2).Sum(Function(x) x.TotalFactura), 2)
            'l_ConsultaExcel.Sum(Function(x) x.TotalMagna)
            Return View(reporteCorteCaja)
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
