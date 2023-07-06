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
Imports Rotativa

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class TrasladosController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_Search As New SearchHelper
        Public H_Transfer As New TransferOrderHelper
        Public H_Servlet As New ConnectionServer
#End Region

#Region "Vistas"
        Public Function GenerarAprobacionMRP() As ActionResult
            Return View()
        End Function

        Public Function ConsultarEstatusNetsuite() As ActionResult
            Return View()
        End Function

        Public Function GenerarSolicitudMRP() As ActionResult
            Return View()
        End Function

        Public Function GenerarRecepcionArticulos(ByVal id As String) As ActionResult
            Try
                Dim GetOrdenTraslado As NetsuiteLibrary.SuiteTalk.TransferOrder = H_Transfer.GetTransferOrderByID(id)
                Dim l_Detalle As New List(Of DetalleOrdenTrasladoViewModel)
                Dim l_entrega As New List(Of DetalleOrdenTrasladoViewModel)

                If IsNothing(GetOrdenTraslado) Then
                    Flash.Instance.Error("No se pudo localizar esta orden de traslado.")
                    Return RedirectToAction("GenerarAprobacionMRP")

                ElseIf IsNothing(GetOrdenTraslado.itemList) Then
                    Flash.Instance.Error("La orden de traslado, no posee conceptos disponibles.")
                    Return RedirectToAction("GenerarAprobacionMRP")

                Else

                    Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                    If IsNothing(ValidaUsuario.MostradorPref) Then
                        Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                        Return RedirectToAction("GenerarAprobacionMRP")

                    ElseIf ValidaUsuario.Ubicaciones.NS_InternalID <> GetOrdenTraslado.transferLocation.internalId Then
                        Flash.Instance.Error("Esta orden de traslado pertenece a un mostrador diferente al seleccionado. Verifique su mostrador.")
                        Return RedirectToAction("GenerarAprobacionMRP")
                    End If

                    For Each RegistrarItems As TransferOrderItem In GetOrdenTraslado.itemList.item

                        '' Para validar que articulos hacen falta por registrar
                        If RegistrarItems.quantityReceived <> RegistrarItems.quantityFulfilled Then
                            Dim RegConceptosTraslado As New DetalleOrdenTrasladoViewModel
                            Dim RegEntrega As New DetalleOrdenTrasladoViewModel

                            RegConceptosTraslado.NS_InternalID = RegistrarItems.item.internalId
                            RegConceptosTraslado.NombreArticulo = RegistrarItems.description
                            RegConceptosTraslado.CantidadDisponible = RegistrarItems.quantity
                            RegConceptosTraslado.CantidadRecibida = RegistrarItems.quantityReceived
                            RegConceptosTraslado.CantidadEnviada = RegistrarItems.quantityFulfilled - RegistrarItems.quantityReceived
                            RegConceptosTraslado.ClassArticulo = RegistrarItems.item.name
                            'RegConceptosTraslado.NS_Externalid = RegistrarItems.item

                            RegEntrega.NS_InternalID = RegistrarItems.item.internalId
                            RegEntrega.CantidadDisponible = RegistrarItems.quantityFulfilled - RegistrarItems.quantityReceived

                            l_Detalle.Add(RegConceptosTraslado)
                            l_entrega.Add(RegEntrega)
                        End If

                    Next

                    ViewBag.DetalleConceptos = l_Detalle
                    ViewBag.Detalle = JsonConvert.SerializeObject(l_entrega)
                    ViewBag.NS = id
                    Return View()
                End If

            Catch ex As Exception
                Return RedirectToAction("GenerarAprobacionMRP")
            End Try

        End Function
#End Region

#Region "Procesos"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarRecepcionesPendientes(ByVal model As ConsultaSalesOrderViewModel) As JsonResult

            Dim l_SalesOrder As New List(Of SalesOrder)
            Dim l_Ventas As New List(Of MapTransferenciaOrden)
            Dim fromDate = ""
            Dim toDate = ""
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

                Dim BuscarDatosMRP As New BusquedaCorteCaja

                BuscarDatosMRP.FechaDesde = model.FechaInicio
                BuscarDatosMRP.FechaHasta = model.FechaFin
                BuscarDatosMRP.Location_ID = ValidaUsuario.Ubicaciones.NS_InternalID

                Dim Datos As RespuestaServicios = H_Transfer.GetListTransferForLocationAdvancedVer(BuscarDatosMRP)

                If Datos.Estatus = "Exito" Then
                    Dim l_respuesta As List(Of MapTransferenciaOrden) = Datos.Valor

                    For Each RegVentas In l_respuesta

                        Dim Venta As New MapTransferenciaOrden
                        Venta.NS_InternalID = RegVentas.NS_InternalID
                        Venta.Num_Transaccion = RegVentas.Num_Transaccion
                        Venta.NS_InternalID_Destino = (From n In db.Ubicaciones Where n.NS_InternalID = RegVentas.NS_InternalID_Origen Select n.DescripcionAlmacen).FirstOrDefault()
                        Venta.Estatus = RegVentas.Estatus

                        l_Ventas.Add(Venta)

                    Next

                Else

                End If

            End If

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Ventas))

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function GenerarRecepcionMercancia(ByVal NS_ID As String) As JsonResult

            Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

            If IsNothing(ValidaUsuario.MostradorPref) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador."))
            Else

                'Dim Datos As RespuestaServicios = H_Transfer.TransformTransferOrderToReceipt(NS_ID)

                'If Datos.Estatus = "Exito" Then

                '    Dim NS = Convert.ToString(Datos.Valor.internalId)
                '    Dim GenerarImpresion As NetsuiteLibrary.SuiteTalk.ItemReceipt = H_Transfer.GetItemReceiptByID(NS)

                '    Dim RegistrarRecepcion As New RecepcionMercancia

                '    RegistrarRecepcion.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "IR_Generado" Select n.idEstatus).FirstOrDefault()
                '    RegistrarRecepcion.NS_InternalID = GenerarImpresion.internalId
                '    RegistrarRecepcion.NS_ExternalID = GenerarImpresion.tranId 
                '    RegistrarRecepcion.UbicacionDestino = ValidaUsuario.MostradorPref

                '    db.RecepcionMercancia.Add(RegistrarRecepcion)
                '    db.SaveChanges()

                '    'Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Ventas))
                '    'RegistrarRecepcion.NS_InternalID =
                'Else

                'End If

            End If

            'Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Ventas))

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function GenerarRecepcionArticulos(ByVal model As GeneraRecepcionViewModel, ByVal entrega As List(Of DetalleRecepcionArticulos)) As ActionResult
            Try
                Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaUsuario.MostradorPref) Then

                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("GenerarAprobacionMRP")
                Else

                    Dim GenerarEntrega As New MapRecepcionMercancia
                    Dim l_Detalle As New List(Of MapDetalleRecepcionMercancia)

                    GenerarEntrega.Internal_ID = model.NS_Traslado
                    GenerarEntrega.Nota = model.Nota

                    For Each RegistrarDetalle In entrega

                        Dim Detalle As New MapDetalleRecepcionMercancia
                        Detalle.Cantidad = RegistrarDetalle.CantidadDisponible
                        Detalle.InternalID = RegistrarDetalle.NS_InternalID
                        l_Detalle.Add(Detalle)


                    Next

                    GenerarEntrega.l_Detalle = l_Detalle

                    Dim Datos As RespuestaServicios = H_Transfer.TransformTransferOrderToReceipt(GenerarEntrega)

                    If Datos.Estatus = "Exito" Then

                        Dim NS = Convert.ToString(Datos.Valor.internalId)
                        Dim GenerarImpresion As NetsuiteLibrary.SuiteTalk.ItemReceipt = H_Transfer.GetItemReceiptByID(NS)

                        Dim RegistrarRecepcion As New RecepcionMercancia

                        RegistrarRecepcion.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "IR_Generado" Select n.idEstatus).FirstOrDefault()
                        RegistrarRecepcion.NS_InternalID = GenerarImpresion.internalId
                        RegistrarRecepcion.NS_ExternalID = GenerarImpresion.tranId
                        RegistrarRecepcion.UbicacionDestino = ValidaUsuario.MostradorPref
                        RegistrarRecepcion.idUsuario = ValidaUsuario.idUsuario
                        RegistrarRecepcion.Nota = model.Nota
                        RegistrarRecepcion.FechaRecepcion = Date.Now

                        db.RecepcionMercancia.Add(RegistrarRecepcion)
                        db.SaveChanges()

                        Flash.Instance.Success("Se ha generado la recepcion de mercancia con exito! Folio de transacción: " + GenerarImpresion.tranId)
                        Return RedirectToAction("GenerarAprobacionMRP")
                    Else
                        Flash.Instance.Error(Datos.Mensaje)
                        Return RedirectToAction("GenerarAprobacionMRP")
                    End If

                End If

            Catch ex As Exception
                Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                Return RedirectToAction("GenerarAprobacionMRP")
            End Try

        End Function

        <HttpPost>
        Public Function DetalleOrdenTraslado(ByVal ns_id As String) As JsonResult
            Try

                Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaUsuario.MostradorPref) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.", ""))
                Else
                    Dim GetOrdenTraslado As NetsuiteLibrary.SuiteTalk.TransferOrder = H_Transfer.GetTransferOrderByID(ns_id)

                    If IsNothing(GetOrdenTraslado) Then
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar esta orden de traslado.", ""))

                    ElseIf IsNothing(GetOrdenTraslado.itemList) Then
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La orden de traslado, no posee conceptos disponibles.", ""))

                    Else

                        If ValidaUsuario.Ubicaciones.NS_InternalID <> GetOrdenTraslado.transferLocation.internalId Then
                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Esta orden de traslado pertenece a un mostrador diferente al seleccionado. Verifique su mostrador.", ""))
                        End If

                        Dim MapTrasladoDatos As New DatosSolicitudTrasladoViewModel
                        Dim l_DetalleTraslado As New List(Of DetalleSolicitudTrasladoViewModel)

                        MapTrasladoDatos.NombreTransaccion = GetOrdenTraslado.tranId
                        MapTrasladoDatos.FechaCreacion = GetOrdenTraslado.createdDate.ToString("dd/MM/yyyy")
                        MapTrasladoDatos.AlmacenOrigen = (From n In db.Ubicaciones Where n.NS_InternalID = GetOrdenTraslado.location.internalId Select n.DescripcionAlmacen).FirstOrDefault()
                        MapTrasladoDatos.AlmacenDestino = (From n In db.Ubicaciones Where n.NS_InternalID = GetOrdenTraslado.transferLocation.internalId Select n.DescripcionAlmacen).FirstOrDefault()

                        For Each RegistrarItems As TransferOrderItem In GetOrdenTraslado.itemList.item

                            Dim RegConceptosTraslado As New DetalleSolicitudTrasladoViewModel

                            RegConceptosTraslado.Descripcion = RegistrarItems.description
                            RegConceptosTraslado.CantidadEnviada = RegistrarItems.quantityFulfilled
                            RegConceptosTraslado.CantidadRecibida = RegistrarItems.quantityReceived
                            RegConceptosTraslado.CantidadSolicitud = RegistrarItems.quantity
                            RegConceptosTraslado.Articulo = RegistrarItems.item.name
                            'RegConceptosTraslado.NS_Externalid = RegistrarItems.item

                            l_DetalleTraslado.Add(RegConceptosTraslado)

                        Next

                        MapTrasladoDatos.l_Detalle = l_DetalleTraslado

                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", MapTrasladoDatos))
                    End If

                End If

            Catch ex As Exception

            End Try
        End Function
        Function ImprimirComprobanteRecepcion(ByVal id As String)

            'If WebSecurity.IsAuthenticated Then

            '    Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

            '    If IsNothing(ValidaMostrador) Then
            '        Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
            '        Return RedirectToAction("Index", "Home")
            '    End If

            '    Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

            '    If GenerarImpresion.Estatus = "Exito" Then
            '        Dim NombreArchivo = String.Format("RecepcionMRP_{0}.pdf", id)
            '        Dim Path_Impresion = "C:\temp\" + NombreArchivo
            '        Dim baseFile = GenerarImpresion.Valor

            '        Dim bytes = System.Convert.FromBase64String(baseFile)
            '        Dim writer As New System.IO.BinaryWriter(IO.File.Open(Path_Impresion, IO.FileMode.Create))
            '        writer.Write(bytes)
            '        writer.Close()

            '        Dim fileStream = New FileStream(Path_Impresion,
            '       FileMode.Open,
            '       FileAccess.Read)
            '        Dim fsResult = New FileStreamResult(fileStream, "application/pdf")
            '        fsResult.FileDownloadName = Path.GetFileName(Path_Impresion)
            '        Return fsResult

            '    Else
            '        Flash.Instance.Error("Hubo un problema al generar el PDF.")
            '        Return RedirectToAction("GenerarAprobacionMRP")
            '    End If

            'Else
            '    Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
            '    Return RedirectToAction("Index", "Home")
            'End If

            Try
                Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaUsuario.MostradorPref) Then
                    Flash.Instance.Warning("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("GenerarAprobacionMRP")
                Else
                    Dim GetOrdenTraslado As NetsuiteLibrary.SuiteTalk.TransferOrder = H_Transfer.GetTransferOrderByID(id)

                    If IsNothing(GetOrdenTraslado) Then
                        Flash.Instance.Warning("No se pudo localizar esta orden de traslado.")
                        Return RedirectToAction("GenerarAprobacionMRP")

                    ElseIf IsNothing(GetOrdenTraslado.itemList) Then
                        Flash.Instance.Warning("La orden de traslado, no posee conceptos disponibles.")
                        Return RedirectToAction("GenerarAprobacionMRP")

                    Else

                        If ValidaUsuario.Ubicaciones.NS_InternalID <> GetOrdenTraslado.transferLocation.internalId Then
                            Flash.Instance.Warning("Esta orden de traslado pertenece a un mostrador diferente al seleccionado. Verifique su mostrador.")
                            Return RedirectToAction("GenerarAprobacionMRP")
                        End If

                        Dim nombrePDF = String.Format("Orden_Traslado_No_{0}_{1}.pdf", id, Date.Now.ToString("ddMMyyyy_HHmmss"))
                        Return New ActionAsPdf("GetImpresionOrdenesTraslado", New With {.id = id}) With {.FileName = nombrePDF, .PageOrientation = Rotativa.Options.Orientation.Landscape, .PageSize = Rotativa.Options.Size.A4}

                    End If

                End If
            Catch ex As Exception

            End Try
        End Function

        Function GetImpresionOrdenesTraslado(ByVal id As String) As ActionResult

            Dim GetOrdenTraslado As NetsuiteLibrary.SuiteTalk.TransferOrder = H_Transfer.GetTransferOrderByID(id)
            Dim MapTrasladoDatos As New DatosSolicitudTrasladoViewModel
            Dim l_DetalleTraslado As New List(Of DetalleSolicitudTrasladoViewModel)

            MapTrasladoDatos.NombreTransaccion = GetOrdenTraslado.tranId
            MapTrasladoDatos.FechaCreacion = GetOrdenTraslado.createdDate.ToString("dd/MM/yyyy")
            MapTrasladoDatos.AlmacenOrigen = (From n In db.Ubicaciones Where n.NS_InternalID = GetOrdenTraslado.location.internalId Select n.DescripcionAlmacen).FirstOrDefault()
            MapTrasladoDatos.AlmacenDestino = (From n In db.Ubicaciones Where n.NS_InternalID = GetOrdenTraslado.transferLocation.internalId Select n.DescripcionAlmacen).FirstOrDefault()
            MapTrasladoDatos.Nota = GetOrdenTraslado.memo

            For Each RegistrarItems As TransferOrderItem In GetOrdenTraslado.itemList.item

                Dim RegConceptosTraslado As New DetalleSolicitudTrasladoViewModel

                RegConceptosTraslado.Descripcion = RegistrarItems.description
                RegConceptosTraslado.CantidadEnviada = RegistrarItems.quantityFulfilled
                RegConceptosTraslado.CantidadRecibida = RegistrarItems.quantityReceived
                RegConceptosTraslado.CantidadSolicitud = RegistrarItems.quantity
                RegConceptosTraslado.Articulo = RegistrarItems.item.name
                'RegConceptosTraslado.NS_Externalid = RegistrarItems.item

                l_DetalleTraslado.Add(RegConceptosTraslado)

            Next

            MapTrasladoDatos.l_Detalle = l_DetalleTraslado

            Return View(MapTrasladoDatos)
        End Function


        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarVentasGeneradas(ByVal model As ConsultaSalesOrderViewModel) As JsonResult
            Dim l_respuesta As List(Of MapMRP)
            Dim l_SalesOrder As New List(Of SalesOrder)
            Dim fromDate = ""
            Dim toDate = ""
            If model.FechaInicio IsNot Nothing And model.FechaFin IsNot Nothing Then
                Dim dif As Integer = DateDiff(("d"), model.FechaInicio, model.FechaFin)
                fromDate = model.FechaInicio
                model.FechaFin = model.FechaFin.Value.AddDays(1)
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

                Dim BuscarDatosMRP As New BusquedaCorteCaja

                BuscarDatosMRP.FechaDesde = model.FechaInicio
                BuscarDatosMRP.FechaHasta = model.FechaFin
                BuscarDatosMRP.Location_ID = ValidaUsuario.Ubicaciones.NS_InternalID

                Dim Datos As RespuestaServicios = H_Search.GetListItemsForInvoice(BuscarDatosMRP)

                l_respuesta = Datos.Valor
            End If

            'If Not IsNothing(model.) Then
            '    l_SalesOrder = (From n In l_SalesOrder Where model.NoPedido.Contains(n.NS_ExternalID)).ToList()
            'End If

            l_SalesOrder = (From n In l_SalesOrder Order By n.idSalesOrder Descending).ToList()

            Dim l_SalesOrderViewModel As List(Of SalesOrderMapViewModel) = AutoMap.AutoMapperListaSalesOrder(l_SalesOrder)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_respuesta))

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
