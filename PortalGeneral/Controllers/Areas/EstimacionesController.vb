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
    Public Class EstimacionesController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_SalesOrder As New EstimacionHelper
        Public H_Search As New SearchHelper
        Public H_Transfer As New TransferOrderHelper
        Public H_Invoice As New InvoiceHelper
        Public H_Servlet As New ConnectionServer
        Public H_Customer As New CustomerHelper
        Public H_ItemFillment As New ItemFulFillmentHelper
#End Region

#Region "Vistas"
        Public Async Function CrearEstimacion() As Task(Of ActionResult)
            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                Else
                    Dim Gentocken As Guid = Guid.NewGuid()
                    Dim TokenCarrito As String = Gentocken.ToString()

                    ViewBag.tokenGenCarrito = TokenCarrito
                    Await GeneraViewbags()
                    Return View()
                End If
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If
        End Function
        Public Function ConsultarEstimacion() As ActionResult

            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaMostPredef.MostradorPref) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe tener un mostrador asignado.")
                    Return RedirectToAction("Index", "Home")

                Else
                    Dim GetEstimaciones As New BusquedaCorteCaja

                    GetEstimaciones.FechaDesde = Date.Now.AddDays(-2).ToString("dd/MM/yyyy")
                    GetEstimaciones.FechaHasta = Date.Now.AddDays(1).AddMinutes(-1).ToString("dd/MM/yyyy")
                    GetEstimaciones.Location_ID = ValidaMostPredef.Ubicaciones.NS_InternalID

                    Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListEstimacionForLocation(GetEstimaciones)
                    Dim l_Busqueda As List(Of MapEstimaciones) = ValidaRespuesta.Valor
                    ViewBag.Estimacion = l_Busqueda.OrderByDescending(Function(x) x.FechaCreacion).ToList()
                    Return View()
                End If

            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarEstimacionesDisponiblesVer2(ByVal model As ConsultaEstimacionViewModel) As JsonResult

            'Dim l_SalesOrder As New List(Of SalesOrder)
            Dim l_salesorder As New List(Of MapEstimaciones)
            Dim fromDate = ""
            Dim toDate = ""
            If model.FechaInicio IsNot Nothing And model.FechaFin IsNot Nothing Then
                Dim dif As Integer = DateDiff(("d"), model.FechaInicio, model.FechaFin)
                fromDate = model.FechaInicio
                toDate = model.FechaFin.Value.AddDays(1)
                If dif < 0 Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
                End If
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Debe definir una rango de fechas válido para poder hacer la consulta"))
            End If

            If User.IsInRole("SuperAdmin") Or User.IsInRole("CxC") Then

                Dim GetSalesOrder As New BusquedaCorteCaja

                GetSalesOrder.FechaDesde = fromDate
                GetSalesOrder.FechaHasta = toDate
                GetSalesOrder.Location_ID = ""

                Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListEstimacionForLocationFilters(GetSalesOrder)

                l_salesorder = ValidaRespuesta.Valor

            ElseIf User.IsInRole("Vendedor") Or User.IsInRole("Regional") Then

                Dim validaPerfil = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(validaPerfil.MostradorPref) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador."))
                Else

                    Dim GetSalesOrder As New BusquedaCorteCaja

                    GetSalesOrder.FechaDesde = fromDate
                    GetSalesOrder.FechaHasta = toDate
                    GetSalesOrder.Location_ID = validaPerfil.Ubicaciones.NS_InternalID

                    Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListEstimacionForLocationFilters(GetSalesOrder)

                    l_salesorder = ValidaRespuesta.Valor

                End If

            End If


            If Not IsNothing(model.NoPedido) Then
                l_salesorder = (From n In l_salesorder Where model.NoPedido.Contains(n.NS_ExternalID)).ToList()
            End If

            l_salesorder = l_salesorder.OrderByDescending(Function(x) x.FechaCreacion).ToList()

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_salesorder))

        End Function
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function CancelarEstimacion2(ByVal NS_ID As String, ByVal Folio As String) As Task(Of JsonResult)
            Try
                'Dim ValidarOrdenVenta = (From n In db.SalesOrder Where n.idSalesOrder = NS_ID And n.Estatus.ClaveExterna = "SO_Creada").FirstOrDefault()

                'If IsNothing(ValidarOrdenVenta) Then
                '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta valida para este proceso.", ""))
                'End If

                Dim Respuesta = H_SalesOrder.CancelEstimacion(NS_ID)

                If Respuesta.Estatus = "Exito" Then

                    Dim RegistrarCancelacion As New LogCancelaciones
                    RegistrarCancelacion.Usuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Nombre).FirstOrDefault()
                    RegistrarCancelacion.FolioSalesOrder = Folio
                    db.LogCancelaciones.Add(RegistrarCancelacion)

                    db.SaveChanges()

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "Se ha generado la cancelación con exito.", New With {.Estatus = "Cancelado", .idSO = NS_ID}))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al cancelar esta orden de venta. Detalles: " + Respuesta.Mensaje, ""))
                End If
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message, ""))
            End Try
        End Function
        Function ImprimireEstimacionVer2(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("Estimacion_{0}.pdf", id)
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
        <HttpPost()>
        Public Async Function GetEstimacion(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim ConsultaEstimacion As New EstimacionesMapViewModel
                Dim l_Detalle As New List(Of DetalleEstimacionMapViewModel)

                Dim Estimacion As NetsuiteLibrary.SuiteTalk.Estimate = H_SalesOrder.GetEstimateByID(id)

                If IsNothing(Estimacion) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No fue posible localizar la estimación..."))
                End If

                For Each RegistrarItems As EstimateItem In Estimacion.itemList.item
                    Dim RegMercancia As New DetalleEstimacionMapViewModel

                    RegMercancia.DescripcionProducto = RegistrarItems.description
                    RegMercancia.Cantidad = RegistrarItems.quantity
                    RegMercancia.NumLote = ""

                    'If RegistrarItems.quantity = RegistrarItems.quantityFulfilled Then
                    '    RegMercancia.EstatusEntrega = "Merc_Complete"

                    'ElseIf RegistrarItems.quantityFulfilled <> 0 Then
                    '    RegMercancia.EstatusEntrega = "Merc_Parcial"

                    'ElseIf RegistrarItems.quantityFulfilled = 0 Then
                    '    RegMercancia.EstatusEntrega = "Merc_Creada"
                    'End If

                    l_Detalle.Add(RegMercancia)
                Next

                With ConsultaEstimacion
                    .NS_ExternalID = Estimacion.tranId
                    .NS_InternalID = Estimacion.internalId
                    .Nombre_Mostrador = Estimacion.location.name
                    .Fecha = Estimacion.createdDate.ToString("dd/MM/yyyy")
                    .Nombre_Cliente = Estimacion.entity.name
                    .SubTotal = Estimacion.subTotal
                    .Descuento = Estimacion.discountTotal
                    .TotalImpuestos = Estimacion.taxTotal
                    .Total = Estimacion.total
                    .Estatus = Estimacion.status
                    .l_detalle = l_Detalle
                End With

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", ConsultaEstimacion))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar la estimación, contacte con el administrador..."))
            End Try

        End Function
#End Region

#Region "Procesos"
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function ConsultarInventario(ByVal id As String, Optional ByVal NombreProducto As String = "", Optional ByVal Customer As String = "") As Task(Of JsonResult)

            Try
                Dim l_Articulos As New List(Of Catalogo_Productos)
                Dim l_InventarioViewModel As New List(Of EstimacionProductoViewModel)

                Dim GetUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                Dim TipoMaterial = Await (From n In db.Catalogo_Productos Where n.NS_ExternalID = id).FirstOrDefaultAsync()

                If Not IsNothing(TipoMaterial) Then

                    Dim Validarcliente = (From n In db.Customers Where n.NS_InternalID = Customer).FirstOrDefault()

                    Dim ValidarNivelPrecio = ""
                    Dim DatosClientes As Customer = H_Customer.GetCustomerById(Validarcliente.NS_InternalID)

                    If IsNothing(DatosClientes.priceLevel) Then
                        ValidarNivelPrecio = "1"
                    Else
                        ValidarNivelPrecio = DatosClientes.priceLevel.internalId
                    End If

                    Dim InventarioDisponible = H_ItemFillment.GetInventoryAvaibleforLocation(GetUsuario.Ubicaciones.NS_InternalID, TipoMaterial.NS_InternalID, TipoMaterial.Categoria, ValidarNivelPrecio)

                    If InventarioDisponible.Estatus = "Exito" Then

                        Dim StockEncontrado As ActualizaStock = InventarioDisponible.Valor
                        Dim MapDatos As New EstimacionProductoViewModel

                        With MapDatos
                            .Producto = TipoMaterial.Descripcion
                            .Ubicacion = GetUsuario.Ubicaciones.DescripcionAlmacen
                            .ID_NS = TipoMaterial.NS_InternalID
                            .TipoInventario = TipoMaterial.Categoria
                            .Clave = TipoMaterial.NS_ExternalID
                            .StockDisponible1 = StockEncontrado.quantityavailable
                            .idProducto = TipoMaterial.idProducto
                            .idUbicacion = GetUsuario.Ubicaciones.idUbicacion
                            .Precio = StockEncontrado.Precio
                            .Descuento = StockEncontrado.DescripcionPrecio
                            .StockPlanta = StockEncontrado.StockPlanta
                            .PrecioLista = StockEncontrado.PrecioLista
                        End With

                        l_InventarioViewModel.Add(MapDatos)
                    Else

                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar el articulo"))
                    End If


                    'Dim l_InventarioAlmacen = Await Consulta.ConsultarListaMateriales(id, GetUsuario.MostradorPref, NombreProducto)


                    'Dim l_InventarioViewModel As List(Of AlmacenProductoViewModel) = AutoMap.AutoMapperProductosAlmacen(l_InventarioAlmacen, Customer)

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_InventarioViewModel))
                Else

                    '' Si no existe, se hace un proceso para reconsiderarlo
                    Dim respuestaitem = H_Search.GetItemForSalesOrders(id)

                    If respuestaitem.Estatus = "Exito" Then

                        Dim RegistrarArticulo As Sinc_Item = respuestaitem.Valor

                        Dim registrarProducto As New Catalogo_Productos
                        With registrarProducto
                            .Descripcion = RegistrarArticulo.s_displayname
                            .NS_InternalID = RegistrarArticulo.s_iteminternalId
                            .NS_ExternalID = RegistrarArticulo.s_claveArticulo

                            If RegistrarArticulo.s_Tipo = "_assembly" Then
                                .Categoria = "Assembly_nL"
                            ElseIf RegistrarArticulo.s_Tipo = "_service" Then
                                .Categoria = "Service"
                            ElseIf RegistrarArticulo.s_Tipo = "_inventoryItem" Then
                                .Categoria = "Inventory"
                            End If
                        End With

                        db.Catalogo_Productos.Add(registrarProducto)
                        db.SaveChanges()

                        Dim RecTipoMaterial = Await (From n In db.Catalogo_Productos Where n.NS_ExternalID = id).FirstOrDefaultAsync()

                        Dim Validarcliente = (From n In db.Customers Where n.NS_InternalID = Customer).FirstOrDefault()

                        Dim ValidarNivelPrecio = ""
                        Dim DatosClientes As Customer = H_Customer.GetCustomerById(Validarcliente.NS_InternalID)

                        If IsNothing(DatosClientes.priceLevel) Then
                            ValidarNivelPrecio = "1"
                        Else
                            ValidarNivelPrecio = DatosClientes.priceLevel.internalId
                        End If

                        Dim InventarioDisponible = H_ItemFillment.GetInventoryAvaibleforLocation(GetUsuario.Ubicaciones.NS_InternalID, RecTipoMaterial.NS_InternalID, RecTipoMaterial.Categoria, ValidarNivelPrecio)

                        If InventarioDisponible.Estatus = "Exito" Then

                            Dim StockEncontrado As ActualizaStock = InventarioDisponible.Valor
                            Dim MapDatos As New EstimacionProductoViewModel

                            With MapDatos
                                .Producto = RecTipoMaterial.Descripcion
                                .Ubicacion = GetUsuario.Ubicaciones.DescripcionAlmacen
                                .ID_NS = RecTipoMaterial.NS_InternalID
                                .TipoInventario = RecTipoMaterial.Categoria
                                .Clave = RecTipoMaterial.NS_ExternalID
                                .StockDisponible1 = StockEncontrado.quantityavailable
                                .idProducto = RecTipoMaterial.idProducto
                                .idUbicacion = GetUsuario.Ubicaciones.idUbicacion
                                .Precio = StockEncontrado.Precio
                                .Descuento = StockEncontrado.DescripcionPrecio
                                .StockPlanta = StockEncontrado.StockPlanta
                                .PrecioLista = StockEncontrado.PrecioLista
                            End With

                            l_InventarioViewModel.Add(MapDatos)

                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_InventarioViewModel))
                        Else
                            ''el articulo esta mal escrito o esta incompleto
                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar el articulo"))
                        End If

                    End If
                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar el producto. Detalles: " + ex.Message))
            End Try

        End Function
        <HttpPost()>
        Public Function GetArticulo(ByVal id As String) As JsonResult

            Try
                Dim l_select As New List(Of AutollenadoViewModel)

                Dim l_Incidencias = db.JG_ConsultarProductoCoincidencia(id).Take(10).ToList()

                For Each RegCoincidencias As JG_ConsultarProductoCoincidencia_Result In l_Incidencias
                    Dim inDatos As New AutollenadoViewModel

                    inDatos.Descripcion = RegCoincidencias.DescripcionProducto
                    inDatos.id = RegCoincidencias.InternalID

                    l_select.Add(inDatos)
                Next

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_select))

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la factura. Detalles: " + ex.Message, ""))
            End Try


        End Function
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function CrearEstimacion(ByVal model As EstimacionesViewModel, ByVal Carrito As List(Of EstimacionCarritoViewModel)) As Task(Of ActionResult)

            Dim AltaOrden As New AltaEstimacion
            Dim l_Detalle As New List(Of DetalleEstimacion)

#Region "Generar Combos"
            Dim Gentocken As Guid = Guid.NewGuid()
            Dim TokenCarrito As String = Gentocken.ToString()

            ViewBag.tokenGenCarrito = TokenCarrito
            Await GeneraViewbags()
#End Region

            Try

#Region "Validar Selección Mostrador y Validación de Ubicación"
                Dim UbicacionUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefaultAsync()
                If IsNothing(UbicacionUsuario.MostradorPref) Then
                    Flash.Instance.Error("Para poder generar una estimación
, es necesario seleccionar un mostrador.")
                    Return View()
                End If

                Dim GetCustomer = Await (From n In db.Customers Where n.NS_InternalID = model.Customer).FirstOrDefaultAsync()
                If IsNothing(GetCustomer) Then
                    Flash.Instance.Error("Hubo un problema al localizar al cliente.")
                    Return View()
                End If
#End Region

#Region "Mapeo de Campos"
                AltaOrden.InternalID_Customer = model.Customer
                AltaOrden.Ubicacion_Venta = UbicacionUsuario.Ubicaciones.NS_InternalID
                AltaOrden.memo = model.Memo
                AltaOrden.CantidadProductos = l_Detalle.Count
                AltaOrden.DetalleInventario = l_Detalle
                AltaOrden.InternalID_MetodoPagoSAT = model.MetodoPagoSAT
                AltaOrden.InternalID_UsoCFDI = model.UsoCFDI
                AltaOrden.InternalID_FormaPagoSAT = model.FormaPagoSAT

                For Each RegistrarProd In Carrito
                    Dim Detalle As New DetalleEstimacion

                    Detalle.Cantidad = RegistrarProd.Cantidad
                    Detalle.IssuesInventoryID = RegistrarProd.IssueInventory
                    Detalle.UbicacionInventario = UbicacionUsuario.Ubicaciones.NS_InternalID
                    Detalle.InternalID = RegistrarProd.ID_NS
                    If RegistrarProd.TipoMaterial = "Inventory" Or RegistrarProd.TipoMaterial = "Assembly_nL" Or RegistrarProd.TipoMaterial = "Service" Then
                        Detalle.NumLote = ""
                    Else
                        Detalle.NumLote = RegistrarProd.NumLote.Trim()
                    End If

                    Detalle.idProducto = RegistrarProd.idPr
                    Detalle.TipoMaterial = RegistrarProd.TipoMaterial
                    Detalle.PrecioU = RegistrarProd.PrecioU
                    Detalle.Total = RegistrarProd.Total
                    l_Detalle.Add(Detalle)
                Next
#End Region

#Region "Envio Datos a NetSuite"
                Dim Resultado = H_SalesOrder.InsertEstimacion(AltaOrden)
#End Region

#Region "Validacion de Respuesta y alta en base de datos"
                If Resultado.Estatus = "Exito" Then

                    Dim RegistrarVenta As New Estimaciones
                    'Dim l_DetalleReg As New List(Of DetalleSalesOrder)

                    Dim ValidaSO = H_SalesOrder.GetEstimateByID(Resultado.Valor.internalId)

                    Dim l_conceptos = ValidaSO.itemList.item

                    RegistrarVenta.idCustomer = GetCustomer.idCustomer
                    RegistrarVenta.idUbicacion = UbicacionUsuario.Ubicaciones.idUbicacion
                    RegistrarVenta.NS_ExternalID = ValidaSO.tranId
                    RegistrarVenta.SubTotal = ValidaSO.subTotal
                    RegistrarVenta.Descuento = ValidaSO.discountTotal
                    RegistrarVenta.TotalImpuestos = ValidaSO.taxTotal
                    RegistrarVenta.Total = ValidaSO.total
                    RegistrarVenta.NS_InternalID = ValidaSO.internalId
                    RegistrarVenta.Nota = ValidaSO.memo
                    RegistrarVenta.FechaCreacion = Date.Now
                    RegistrarVenta.idUsuario = WebSecurity.CurrentUserId
                    RegistrarVenta.idMetodoPagoSAT = Await (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = model.MetodoPagoSAT Select n.idMetodoPagoSAT).FirstOrDefaultAsync()
                    RegistrarVenta.idFormaPago = Await (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = model.FormaPagoSAT Select n.idFormaPago).FirstOrDefaultAsync()
                    RegistrarVenta.idUsoCFDI = Await (From n In db.Catalogo_UsoCFDI Where n.NS_InternalID = model.UsoCFDI Select n.idUsoCFDI).FirstOrDefaultAsync()
                    RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "EST_Creada" Select n.idEstatus).FirstOrDefaultAsync()

                    db.Database.CommandTimeout = 200000
                    db.Estimaciones.Add(RegistrarVenta)
                    db.SaveChanges()
                    Flash.Instance.Success("Se ha creado la estimación con éxito! " + ValidaSO.tranId)
                    Return RedirectToAction("ConsultarEstimacion")
                End If

#End Region

            Catch ex As Exception
                model.GenerarFacturaDirecta = False
                model.GenerarVentaDirecta = False

                Dim mensaje = ex.Message
                Dim detallesExtras = ex.InnerException.Message + ex.InnerException.StackTrace
                'Dim guardarlog As New LogSalesOrder


                Flash.Instance.Error("Hubo un problema al procesar esta solicitud. Detalles: " + ex.Message)
                Return RedirectToAction("Index", "Home")
            End Try

        End Function
#End Region

#Region "Extras"
        Public Async Function GeneraViewbags() As Threading.Tasks.Task
            Dim Select_Customer As New List(Of SelectListItem)
            Dim Select_UbicacionVentas As New List(Of SelectListItem)
            Dim Select_UbicacionesAlmacen As New List(Of SelectListItem)
            Dim Select_MetodosPagoSAT As New List(Of SelectListItem)
            Dim Select_FormasPagoSAT As New List(Of SelectListItem)
            Dim Select_UsoCFDI As New List(Of SelectListItem)
            Dim l_Articulos As New List(Of String)

            'Dim l_UbicacionesAlmacen = Await Consulta.ConsultarUbicacionesAlmacen()

            'For Each RegistroCustomer In l_UbicacionesAlmacen
            '    Select_UbicacionesAlmacen.Add(New SelectListItem With {.Text = RegistroCustomer.DescripcionAlmacen, .Value = RegistroCustomer.idUbicacion})
            'Next

            Dim l_Customers = Await Consulta.ConsultarListaCustomers()

            For Each RegistroCustomer In l_Customers
                Dim texto = RegistroCustomer.NS_ExternalID + " - " + RegistroCustomer.Nombre
                Select_Customer.Add(New SelectListItem With {.Text = texto, .Value = RegistroCustomer.NS_InternalID})
            Next

            'Dim l_Ubicaciones = Await Consulta.ConsultarUbicacionesVentas()

            'For Each RegistroUbicaciones In l_Ubicaciones
            '    Select_UbicacionVentas.Add(New SelectListItem With {.Text = RegistroUbicaciones.Descripcion, .Value = RegistroUbicaciones.NS_InternalID})
            'Next

            Dim l_MetodoPagoSAT = Await Consulta.ConsultaListaMetodosPagoSAT()

            For Each RegistroMetodoPagoSAT In l_MetodoPagoSAT
                Select_MetodosPagoSAT.Add(New SelectListItem With {.Text = RegistroMetodoPagoSAT.Descripcion, .Value = RegistroMetodoPagoSAT.NS_InternalID})
            Next

            Dim l_FormasPagoSAT = Await Consulta.ConsultaListaFormaPagoSAT()

            For Each RegistroMetodoPagoSAT In l_FormasPagoSAT
                Select_FormasPagoSAT.Add(New SelectListItem With {.Text = RegistroMetodoPagoSAT.Descripcion, .Value = RegistroMetodoPagoSAT.NS_InternalID})
            Next

            Dim l_UsoCFDI = Await Consulta.ConsultaListaUsoCFDI()

            For Each RegistroUsoCFDI In l_UsoCFDI
                Select_UsoCFDI.Add(New SelectListItem With {.Text = RegistroUsoCFDI.Descripcion, .Value = RegistroUsoCFDI.NS_InternalID})
            Next

            l_Articulos = Await (From n In db.Catalogo_Productos Select n.Descripcion).ToListAsync()

            'ViewBag.ProductosDisponibles = Await Consulta.ConsultarListaMateriales()
            ViewBag.ProductosDisponibles = Nothing
            ViewBag.Customer = Select_Customer
            ViewBag.UbicacionVenta = Select_UbicacionVentas
            ViewBag.UbicacionesAlmacen = Select_UbicacionesAlmacen
            ViewBag.MetodoPagoSAT = Select_MetodosPagoSAT
            ViewBag.FormaPagoSAT = Select_FormasPagoSAT
            ViewBag.UsoCFDI = Select_UsoCFDI

            ViewBag.idMetodoPagoSAT = Select_MetodosPagoSAT
            ViewBag.idFormaPago = Select_FormasPagoSAT
            ViewBag.idUsoCFDI = Select_UsoCFDI
            ViewBag.BusquedaArticulos = l_Articulos.ToArray()
        End Function
#End Region
#Region "Carrito"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function CrearListaProductos(ByVal model As EstimacionCarritoViewModel) As JsonResult

            Dim tempDataDictionary = Session(model.Token)
            Dim Vlender = Session("Vlender")

            Session("Vlender") = Session("Vlender") + 1

            If IsNothing(tempDataDictionary) Then

                Dim l_CarritoRender As New List(Of EstimacionCarritoViewModel)

                ''Generar numero de producto unico
                Dim random As New Random()

                model.Total = Math.Round(Convert.ToDecimal(model.Cantidad) * Convert.ToDecimal(model.PrecioU), 2)
                model.Vlender = Session("Vlender")

                l_CarritoRender.Add(model)

                Session(model.Token) = l_CarritoRender
            Else
                Dim l_CarritoRender As New List(Of EstimacionCarritoViewModel)

                l_CarritoRender = Session(model.Token)

                ''Validamos que el producto agregado, ya exista en la lista

                Dim valDato = (From n In l_CarritoRender Where n.NumLote = model.NumLote And n.ClaveProducto = model.ClaveProducto).FirstOrDefault()

                If IsNothing(valDato) Then
                    model.Total = Math.Round(Convert.ToDecimal(model.Cantidad) * Convert.ToDecimal(model.PrecioU), 2)
                    model.Vlender = Session("Vlender")
                    l_CarritoRender.Add(model)
                Else
                    valDato.Cantidad = Math.Round(Convert.ToDecimal(valDato.Cantidad) + Convert.ToDecimal(model.Cantidad), 0)
                    valDato.Total = Math.Round(Convert.ToDecimal(valDato.Cantidad) * Convert.ToDecimal(model.PrecioU), 2)
                End If

                Session(model.Token) = l_CarritoRender

            End If

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", Session(model.Token)))
        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function EliminarArticuloCarrito(ByVal Vlender As String, ByVal token As String) As JsonResult
            Dim l_CarritoRender As New List(Of EstimacionCarritoViewModel)

            l_CarritoRender = Session(token)

            ''Validamos que el producto agregado, ya exista en la lista

            Dim valDato = (From n In l_CarritoRender Where n.Vlender = Vlender).FirstOrDefault()

            If Not IsNothing(valDato) Then
                l_CarritoRender.Remove(valDato)
            End If

            Session(token) = l_CarritoRender

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", Session(token)))
        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function VaciarCarrito(ByVal token As String) As JsonResult

            Session(token) = Nothing
            'Session("Vlender") = Nothing

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
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
