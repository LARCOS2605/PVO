Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Validation
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Mvc
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports NetsuiteLibrary
Imports NetsuiteLibrary.Clases
Imports NetsuiteLibrary.SuiteServlet_Connection
Imports NetsuiteLibrary.SuiteTalk
Imports NetsuiteLibrary.SuiteTalk_Helpers
Imports PortalGeneral
Imports Renci.SshNet
Imports Renci.SshNet.Sftp
Imports SAT.Services.ConsultaCFDIService
Imports SW.Services.Status
Imports WebMatrix.WebData

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class OrdenesVentaController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_SalesOrder As New SalesOrderHelper
        Public H_ItemFillment As New ItemFulFillmentHelper
        Public H_Invoices As New InvoiceHelper
        Public H_Servlet As New ConnectionServer
        Public H_Search As New SearchHelper
        Public H_Customer As New CustomerHelper
        Private parametros As Dictionary(Of String, String)
#End Region

#Region "Facturas"
        Public Async Function ConsultarFacturas() As Task(Of ActionResult)

            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                Dim l_OrdenesdeVenta = Await Consulta.ConsultarOrdenesVentaPorUbicacion(ValidaMostPredef)

                ViewBag.OrdenesVenta = l_OrdenesdeVenta
                Return View()
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function
#End Region

#Region "Vistas Ordenes Venta"
        '<NoCache()>
        Public Async Function CrearOrdenDeVenta() As Task(Of ActionResult)

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

        Public Async Function EditarOrdenDeVenta(ByVal id As String) As Task(Of ActionResult)

            Dim ValidaMostrador = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefaultAsync()
            Dim ValidaOrdenVenta = Await (From n In db.SalesOrder Where n.NS_InternalID = id).FirstOrDefaultAsync()


            If IsNothing(ValidaOrdenVenta) Then
                Flash.Instance.Error("No fue posible localizar la orden de Venta.")
                Return RedirectToAction("Index", "Home")
            End If

            If Not ValidaOrdenVenta.idUbicacion = ValidaMostrador Then
                Flash.Instance.Error("No se puede modificar esta orden de venta, debido a que la ubicación del usuario, es distinta a la orden generada.")
                Return RedirectToAction("Index", "Home")
            End If

            If ValidaOrdenVenta.Estatus.ClaveExterna <> "SO_Creada" Then
                Flash.Instance.Error("No se puede modificar esta orden de venta, debido a que ya se encuentra comprometida.")
                Return RedirectToAction("Index", "Home")
            End If

            Await GeneraViewbags()

            Dim ModelData As New SalesOrderMapViewModel
            Dim l_Detalle As New List(Of DetalleSalesOrderMapViewModel)

            Dim Detalle = (From n In db.DetalleSalesOrder Where n.idSalesOrder = ValidaOrdenVenta.idSalesOrder).ToList()

            ModelData = AutoMap.AutoMapperSalesOrder(ValidaOrdenVenta)

            l_Detalle = AutoMap.AutoMapperDetallePedido(Detalle)

            ModelData.Nombre_Cliente = ModelData.ClaveCliente + " - " + ModelData.Nombre_Cliente



            ViewBag.Detalle = l_Detalle

            Return View(ModelData)
        End Function

        Public Async Function ConsultarOrdenDeVenta() As Task(Of ActionResult)

            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                Dim l_OrdenesdeVenta = Await Consulta.ConsultarOrdenesVentaPorUbicacion(ValidaMostPredef)

                ViewBag.OrdenesVenta = l_OrdenesdeVenta
                Return View()
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function

        Function ImprimirOrdenVenta(ByVal id As Integer)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim ValidaPedidoCorrespondiente = (From x In db.SalesOrder Where x.idUbicacion = ValidaMostrador And x.idSalesOrder = id).FirstOrDefault

                If IsNothing(ValidaPedidoCorrespondiente) Then
                    Flash.Instance.Error("Este pedido no corresponde al mostrador seleccionado.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim Pedido = (From x In db.SalesOrder Where x.idSalesOrder = id).FirstOrDefault()

                If IsNothing(Pedido) Then
                    Flash.Instance.Error("Este pedido no existe o fue eliminado. Verifique su existencia.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(Pedido.NS_InternalID)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("Pedido_{0}.pdf", Pedido.NS_ExternalID)
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

        Function ImprimirInvoice(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                'Dim ValidaPedidoCorrespondiente = (From x In db.SalesOrder Where x.idUbicacion = ValidaMostrador And x.idSalesOrder = id).FirstOrDefault

                'If IsNothing(ValidaPedidoCorrespondiente) Then
                '    Flash.Instance.Error("Este pedido no corresponde al mostrador seleccionado.")
                '    Return RedirectToAction("Index", "Home")
                'End If

                'Dim Pedido = (From x In db.SalesOrder Where x.idSalesOrder = id).FirstOrDefault()

                'If IsNothing(Pedido) Then
                '    Flash.Instance.Error("Este pedido no existe o fue eliminado. Verifique su existencia.")
                '    Return RedirectToAction("Index", "Home")
                'End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("PreFactura_{0}.pdf", id)
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

        Function GETArchivo_Timbrado(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim GenerarImpresion = H_Invoices.GetInvoiceFile(id)

                If GenerarImpresion.Estatus = "Exito" Then

                    Dim Path_Impresion = String.Format("C:\temp\{0}", GenerarImpresion.Valor)

                    Dim fileStream = New FileStream(Path_Impresion,
                   FileMode.Open,
                   FileAccess.Read)
                    Dim fsResult = New FileStreamResult(fileStream, "application/pdf")
                    fsResult.FileDownloadName = Path.GetFileName(Path_Impresion)
                    Return fsResult

                Else
                    Flash.Instance.Error("Hubo un problema al obtener el PDF.")
                    Return RedirectToAction("Index", "Home")
                End If

            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If
        End Function
#End Region

#Region "Procesos Ordenes Venta"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function ConsultarOrdenesVentaDisponibles(ByVal model As ConsultaSalesOrderViewModel) As Task(Of JsonResult)

            Dim l_SalesOrder As New List(Of SalesOrder)
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

            If User.IsInRole("SuperAdmin") Then

                l_SalesOrder = Await (From n In db.SalesOrder Where n.FechaCreacion >= fromDate And n.FechaCreacion <= toDate).ToListAsync()

            ElseIf User.IsInRole("Vendedor") Or User.IsInRole("Regional") Then

                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador."))
                Else
                    l_SalesOrder = Await (From n In db.SalesOrder Where n.FechaCreacion >= fromDate And n.FechaCreacion <= toDate And n.idUbicacion = ValidaMostPredef).ToListAsync()
                End If

            End If


            If Not IsNothing(model.NoPedido) Then
                l_SalesOrder = (From n In l_SalesOrder Where model.NoPedido.Contains(n.NS_ExternalID)).ToList()
            End If

            'If Not IsNothing(model.) Then
            '    l_SalesOrder = (From n In l_SalesOrder Where model.NoPedido.Contains(n.NS_ExternalID)).ToList()
            'End If

            l_SalesOrder = (From n In l_SalesOrder Order By n.idSalesOrder Descending).ToList()

            Dim l_SalesOrderViewModel As List(Of SalesOrderMapViewModel) = AutoMap.AutoMapperListaSalesOrder(l_SalesOrder)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_SalesOrderViewModel))

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function CrearOrdenDeVenta(ByVal model As SalesOrderViewModel, ByVal Carrito As List(Of CarritoViewModel)) As Task(Of ActionResult)

            Dim AltaOrden As New AltaSalesOrder
            Dim foliotran As String = ""
            Dim l_Detalle As New List(Of DetalleAltaSalesOrder)

            Try

#Region "Generar Combos"
                Dim Gentocken As Guid = Guid.NewGuid()
                Dim TokenCarrito As String = Gentocken.ToString()

                ViewBag.tokenGenCarrito = TokenCarrito
                Await GeneraViewbags()
#End Region

                Dim UbicacionUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefaultAsync()
                If IsNothing(UbicacionUsuario.MostradorPref) Then
                    Flash.Instance.Error("Para poder generar una orden de venta, es necesario seleccionar un mostrador.")
                    Return RedirectToAction("ConsultarOrdenVenta")
                End If

                'Dim GetUbicacion = Await (From n In db.Ubicaciones Where n.idUbicacion = UbicacionUsuario.MostradorPref).FirstOrDefaultAsync()
                'If IsNothing(GetUbicacion) Then
                '    Flash.Instance.Error("No fue posible localizar la ubicación del mostrador.")
                '    Return View()
                'End If

                Dim GetCustomer = Await (From n In db.Customers Where n.NS_InternalID = model.Customer).FirstOrDefaultAsync()
                If IsNothing(GetCustomer) Then
                    Flash.Instance.Error("Hubo un problema al localizar al cliente.")
                    Return RedirectToAction("ConsultarOrdenVenta")
                End If


                AltaOrden.InternalID_Customer = model.Customer
                AltaOrden.Ubicacion_Venta = UbicacionUsuario.Ubicaciones.NS_InternalID
                AltaOrden.memo = model.Memo
                AltaOrden.CantidadProductos = l_Detalle.Count
                AltaOrden.DetalleInventario = l_Detalle
                AltaOrden.InternalID_MetodoPagoSAT = model.MetodoPagoSAT
                AltaOrden.InternalID_UsoCFDI = model.UsoCFDI
                AltaOrden.InternalID_FormaPagoSAT = model.FormaPagoSAT

                For Each RegistrarProd In Carrito
                    Dim Detalle As New DetalleAltaSalesOrder

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


                Dim Resultado = H_SalesOrder.InsertSalesOrder(AltaOrden)

                If Resultado.Estatus = "Exito" Then

                    Dim RegistrarVenta As New SalesOrder
                    Dim l_DetalleReg As New List(Of DetalleSalesOrder)

                    Dim ValidaSO = H_SalesOrder.GetSalesOrderByID(Resultado.Valor.internalId)

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
                    RegistrarVenta.idMetodoPagoSAT = (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = model.MetodoPagoSAT Select n.idMetodoPagoSAT).FirstOrDefault()
                    RegistrarVenta.idFormaPago = (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = model.FormaPagoSAT Select n.idFormaPago).FirstOrDefault()
                    RegistrarVenta.idUsoCFDI = (From n In db.Catalogo_UsoCFDI Where n.NS_InternalID = model.UsoCFDI Select n.idUsoCFDI).FirstOrDefault()
                    RegistrarVenta.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefault()

                    db.SalesOrder.Add(RegistrarVenta)
                    db.SaveChanges()

                    ''En caso de que la factura se genere directamente, se hara este proceso
                    If model.GenerarFacturaDirecta = True Then
                        'Dim Respuesta = H_SalesOrder.TransformSalesOrderToInvoice(Resultado.Valor.internalId)

                        'If Respuesta.Estatus = "Exito" Then

                        '    If l_Detalle.Count = 1 And l_Detalle.First.TipoMaterial = "Service" Then
                        '        RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
                        '    Else
                        '        RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_FacPenEnt" Select n.idEstatus).FirstOrDefaultAsync()
                        '    End If

                        '    Dim RegInvoice As New Invoice_SO
                        '    Dim l_Factura As New List(Of Invoice_SO)
                        '    Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

                        '    Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

                        '    RegInvoice.NS_InternalID = ConvertRespuesta
                        '    RegInvoice.NS_ExternalID = DatosInovice.tranId
                        '    RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                        '    RegInvoice.FechaCreacion = Date.Now
                        '    RegInvoice.idCustomer = GetCustomer.idCustomer
                        '    RegInvoice.Subtotal = DatosInovice.subTotal
                        '    RegInvoice.Total_Impuestos = DatosInovice.taxTotal
                        '    RegInvoice.Total = DatosInovice.total
                        '    RegInvoice.ImporteAdeudado = DatosInovice.amountRemaining
                        '    RegInvoice.idUbicacion = UbicacionUsuario.MostradorPref
                        '    l_Factura.Add(RegInvoice)

                        '    RegistrarVenta.Invoice_SO = l_Factura

                        '    Flash.Instance.Success(String.Format("Se ha creado la orden de venta No. {0} con exito!", ValidaSO.tranId))
                        'Else

                        '    RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefaultAsync()
                        '    Flash.Instance.Warning("Se ha creado la orden de venta No. " + ValidaSO.tranId + " Pero hubo un problema al generar la factura. Detalles: " + Respuesta.Valor)
                        'End If

                    ElseIf model.GenerarVentaDirecta = True Then

                        Dim RegMercancia As New EntregaMercancia
                        Dim EjecucionMercancia As New ConnectionServer
                        Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

                        RegMercancia.NS_InternalID_Doc = ValidaSO.internalId
                        RegMercancia.Memo = "Entrega de Mercancia de la orden " + ValidaSO.tranId

                        For Each reg_Mercancia In l_Detalle

                            Dim NewMerc As New DetalleEntregaMercancia

                            NewMerc.NS_InternalID_Mercancia = reg_Mercancia.InternalID
                            NewMerc.Cantidad = reg_Mercancia.Cantidad

                            If reg_Mercancia.TipoMaterial = "Assembly" Then
                                NewMerc.TipoMaterial = "Assembly"
                                NewMerc.IssuesInventoryID = reg_Mercancia.IssuesInventoryID
                            ElseIf reg_Mercancia.TipoMaterial = "Inventory" Then
                                NewMerc.TipoMaterial = "Inventory"
                            ElseIf reg_Mercancia.TipoMaterial = "Assembly_nL" Then
                                NewMerc.TipoMaterial = "Assembly_nL"
                            ElseIf reg_Mercancia.TipoMaterial = "Service" Then
                                Continue For
                            End If

                            l_Mercancia.Add(NewMerc)
                        Next

                        RegMercancia.l_Detalle = l_Mercancia

                        If l_Mercancia.Count <> 0 Then
                            ''Caso normal en donde hay una venta con articulos y servicios por partes iguales

                            Dim RespuestaEjecucion = EjecucionMercancia.ConnectionServiceServlet(RegMercancia)

                            If RespuestaEjecucion.Estatus = "Exito" Then

                                Dim RespuestaTransformacion = H_SalesOrder.TransformSalesOrderToInvoice(Resultado.Valor.internalId)

                                If RespuestaTransformacion.Estatus = "Exito" Then
                                    Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
                                Else
                                    Flash.Instance.Warning("Se ha creado la orden de venta No. " + ValidaSO.tranId + " Pero hubo un problema al generar la factura. Detalles: " + RespuestaTransformacion.Mensaje)
                                End If
                            Else
                                Flash.Instance.Error(RespuestaEjecucion.Mensaje)
                            End If

                        Else

                            Dim RespuestaTransformacion = H_SalesOrder.TransformSalesOrderToInvoice(Resultado.Valor.internalId)

                            If RespuestaTransformacion.Estatus = "Exito" Then
                                Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
                            Else
                                Flash.Instance.Warning("Se ha creado la orden de venta No. " + ValidaSO.tranId + " Pero hubo un problema al generar la factura. Detalles: " + RespuestaTransformacion.Mensaje)
                            End If

                        End If

                    Else
                        Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
                    End If

                    model.GenerarFacturaDirecta = False
                    model.GenerarVentaDirecta = False
                    Return RedirectToAction("ConsultarOrdenVenta")

                Else

                    model.GenerarFacturaDirecta = False
                    model.GenerarVentaDirecta = False
                    Flash.Instance.Error(Resultado.Mensaje)
                    Return RedirectToAction("ConsultarOrdenVenta")
                End If


            Catch ex As Exception
                model.GenerarFacturaDirecta = False
                model.GenerarVentaDirecta = False

                'Dim mensaje = ex.Message
                'Dim detallesExtras = ex.InnerException.Message
                'Dim constuirMensajeError As String = String.Format("Hubo un problema al almacenar la sales order no. {0}, Detalles: {1}, Extras: {2}", foliotran, mensaje, detallesExtras)
                'Dim guardarlog As New LogSalesOrder
                'guardarlog.Descripcion = constuirMensajeError
                'guardarlog.NS_ExternalID = foliotran
                'db.Database.CommandTimeout = 200000
                'db.LogSalesOrder.Add(guardarlog)
                'db.SaveChanges()

                Flash.Instance.Error("Hubo un problema al procesar esta solicitud. Detalles: " + ex.Message)
                Return RedirectToAction("Index", "Home")
            End Try

        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function ConsultarInventario(ByVal id As String, Optional ByVal NombreProducto As String = "", Optional ByVal Customer As String = "") As Task(Of JsonResult)

            Try
                Dim l_Articulos As New List(Of Catalogo_Productos)
                Dim l_InventarioViewModel As New List(Of AlmacenProductoViewModel)

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
                        Dim MapDatos As New AlmacenProductoViewModel

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
                            .StockMuelles = StockEncontrado.StockMuelles
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
                            Dim MapDatos As New AlmacenProductoViewModel

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
                                .StockMuelles = StockEncontrado.StockMuelles
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
        Public Async Function GetOrdenVenta(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim ConsultaOrdenVenta As New SalesOrderMapViewModel
                Dim l_Detalle As New List(Of DetalleSalesOrderMapViewModel)

                Dim OrdenVenta = Await Consulta.ConsultarOrdenesVentaPorID(id)
                Dim l_DetalleOrdenVenta = Await Consulta.ConsultarDetalleOrden(id)

                If IsNothing(OrdenVenta) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No fue posible localizar la orden de venta..."))
                End If

                ConsultaOrdenVenta = AutoMap.AutoMapperSalesOrder(OrdenVenta)
                l_Detalle = AutoMap.AutoMapperDetallePedido(l_DetalleOrdenVenta)

                ConsultaOrdenVenta.l_detalle = l_Detalle

                If ConsultaOrdenVenta.Estatus = "SO_Facturada" Then

                    Dim ActualizaInfo = Await (From n In db.Invoice_SO Where n.NS_InternalID = ConsultaOrdenVenta.NS_Val_INV).FirstOrDefaultAsync()

                    If IsNothing(ActualizaInfo.UUID) Then
                        ''Validamos si tiene timbre PRUEBA
                        Dim ValidarDatos = H_Invoices.GetSATFieldsForInvoice(ConsultaOrdenVenta.NS_Val_INV)

                        If ValidarDatos.Estatus = "Exito" Then

                            Dim Result As DatosSat = ValidarDatos.Valor


                            ActualizaInfo.UUID = Result.UUID
                            ActualizaInfo.NS_PDF = Result.NS_PDF
                            ActualizaInfo.NS_XML = Result.NS_XML
                            db.SaveChanges()

                            ConsultaOrdenVenta.UUID = Result.UUID
                            ConsultaOrdenVenta.NS_ID_pdf = Result.NS_PDF
                            ConsultaOrdenVenta.NS_ID_xml = Result.NS_XML
                        End If

                    Else
                        ConsultaOrdenVenta.UUID = ActualizaInfo.UUID
                        ConsultaOrdenVenta.NS_ID_pdf = ActualizaInfo.NS_PDF
                        ConsultaOrdenVenta.NS_ID_xml = ActualizaInfo.NS_XML
                    End If
                End If

                'Dim Invoice = H_Invoices.GetInvoiceByID(OrdenVenta.NS_InternalID)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", ConsultaOrdenVenta))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar la orden de venta, contacte con el administrador..."))
            End Try

        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function GenerarEjecucionMercancia(ByVal id As String, ByVal Entrega As List(Of DetalleEjecucionViewModel)) As Task(Of JsonResult)
            Try
                Dim RegMercancia As New EntregaMercancia
                Dim EjecucionMercancia As New ConnectionServer
                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

#Region "Consulta Y Validacion de Orden de Venta"
                Dim GetSalesOrder = Await (From n In db.SalesOrder Where n.idSalesOrder = id).FirstOrDefaultAsync()

                If IsNothing(GetSalesOrder) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If
#End Region

#Region "Mapeo de Datos a Netsuite"
                RegMercancia.NS_InternalID_Doc = GetSalesOrder.NS_InternalID
                RegMercancia.Memo = "Entrega de Mercancia de la orden " + GetSalesOrder.NS_ExternalID

                For Each reg_Mercancia In GetSalesOrder.DetalleSalesOrder

                    Dim ValidaMercancia = (From n In Entrega Where n.id = reg_Mercancia.idDetalleSalesOrder).FirstOrDefault()

                    If Not IsNothing(ValidaMercancia) Then
                        Dim NewMerc As New DetalleEntregaMercancia

                        NewMerc.NS_InternalID_Mercancia = reg_Mercancia.Catalogo_Productos.NS_InternalID
                        NewMerc.Cantidad = ValidaMercancia.Cantidad

                        If reg_Mercancia.Catalogo_Productos.Categoria = "Assembly" Then
                            NewMerc.TipoMaterial = "Assembly"
                            NewMerc.IssuesInventoryID = reg_Mercancia.IssuesInventory
                        Else
                            NewMerc.TipoMaterial = "Inventory"
                        End If

                        l_Mercancia.Add(NewMerc)
                    End If
                Next

                RegMercancia.l_Detalle = l_Mercancia
#End Region

#Region "Envio de Datos Netsuite"
                Dim Respuesta = EjecucionMercancia.ConnectionServiceServlet(RegMercancia)
#End Region

#Region "Validar Respuesta Netsuite"

                If Respuesta.Estatus = "Exito" Then

                    'Actualizamos mercancia entregada
                    For Each reg_Mercancia In GetSalesOrder.DetalleSalesOrder

                        Dim ValidaMercancia = (From n In Entrega Where n.id = reg_Mercancia.idDetalleSalesOrder).FirstOrDefault()

                        If Not IsNothing(ValidaMercancia) Then
                            Dim cantidadEntregada = reg_Mercancia.CantidadEntregada + Convert.ToDecimal(ValidaMercancia.Cantidad)
                            reg_Mercancia.CantidadEntregada = cantidadEntregada

                            If cantidadEntregada = reg_Mercancia.Cantidad Then
                                reg_Mercancia.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Complete" Select n.idEstatus).FirstOrDefaultAsync()
                            Else
                                reg_Mercancia.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Parcial" Select n.idEstatus).FirstOrDefaultAsync()
                            End If
                            db.SaveChanges()
                        End If
                    Next

                    ''Validar Cambio de Estatus SO
                    Dim ValEstatusMercancias As Boolean = False
                    For Each ValidaEstatus In GetSalesOrder.DetalleSalesOrder

                        If ValidaEstatus.Estatus.ClaveInterna = "Merc_Creada" OrElse ValidaEstatus.Estatus.ClaveInterna = "Merc_Parcial" Then
                            ValEstatusMercancias = True
                        End If
                    Next

                    ''Toda la Mercancia de la sales order, ha sido entregada y se debera cambiar el estatos a entregado
                    If ValEstatusMercancias = False Then

                        ''Se detecta como reparto MAF
                        If GetSalesOrder.Estatus.ClaveExterna = "SO_FacPenEnt" Then

                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()
                        Else

                            ''Cambiamos el estatus de todas formas
                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Entrega" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()

                            ''En automatico convertimos la SO a Factura
                            Dim RespuestaConvert = H_SalesOrder.TransformSalesOrderToInvoice(GetSalesOrder.NS_InternalID)

                            If RespuestaConvert.Estatus = "Exito" Then

                                GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()

                                Dim RegInvoice As New Invoice_SO
                                Dim ConvertRespuesta = Convert.ToString(RespuestaConvert.Valor.internalId)

                                Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

                                RegInvoice.NS_InternalID = ConvertRespuesta
                                RegInvoice.NS_ExternalID = DatosInovice.tranId
                                RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                                RegInvoice.FechaCreacion = Date.Now
                                RegInvoice.idCustomer = GetSalesOrder.idCustomer
                                RegInvoice.Subtotal = DatosInovice.subTotal
                                RegInvoice.Total_Impuestos = DatosInovice.taxTotal
                                RegInvoice.Total = DatosInovice.total
                                RegInvoice.ImporteAdeudado = DatosInovice.total
                                RegInvoice.idSalesOrder = GetSalesOrder.idSalesOrder
                                db.Invoice_SO.Add(RegInvoice)
                                db.SaveChanges()

                                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
                            Else
                                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, RespuestaConvert.Mensaje, ""))
                            End If

                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()
                        End If


                    Else

                        GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Entrega" Select n.idEstatus).FirstOrDefaultAsync()
                        db.SaveChanges()
                    End If

                    Dim Estatus = GetSalesOrder.Estatus.ClaveInterna



                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, Respuesta.Mensaje, ""))
                End If
#End Region



            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la entrega de mercancia, Detalles: " + ex.Message))
            End Try

        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function GenerarEjecucionTotalMercancia(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim RegMercancia As New EntregaMercancia
                Dim EjecucionMercancia As New ConnectionServer
                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

#Region "Consulta Y Validacion de Orden de Venta"
                Dim GetSalesOrder = Await (From n In db.SalesOrder Where n.idSalesOrder = id).FirstOrDefaultAsync()

                If IsNothing(GetSalesOrder) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If
#End Region
                Dim SoloServicios = GetSalesOrder.DetalleSalesOrder.Where(Function(x) x.Catalogo_Productos.Categoria = "Service").Count

                ''Caso unicamente si facturan un servicio, en este caso, se factura directamente el pedido
                If (GetSalesOrder.DetalleSalesOrder.Count = 1 And GetSalesOrder.DetalleSalesOrder.First.Catalogo_Productos.Categoria = "Service") Or (GetSalesOrder.DetalleSalesOrder.Count = SoloServicios) Then

#Region "Cambio de Estatus de Sales Order"

                    ''Escenario 1 - Reparto MAF
                    ''En este caso se crea la Sales Order y se Factura, pero no se hace la ejecución de Mercancia
                    ''Como resultado, se omite por completo el convertir nuevamente la SO a Factura y solo se cambia el estatus
                    If GetSalesOrder.Estatus.ClaveExterna = "SO_FacPenEnt" Then

                        GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
                        db.SaveChanges()
                    Else

                        ''Escenario 2 - Entrega o Cotización
                        ''En este caso se crea la Sales Order, pero no se realizo la factura
                        ''Como resultado, se realiza todo el proceso

                        ''Cambiamos el estatus de estatus a entrega de Mercancia como respaldo
                        GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefaultAsync()
                        db.SaveChanges()

                        ''En automatico convertimos la SO a Factura
                        Dim RespuestaConvert = H_SalesOrder.TransformSalesOrderToInvoice(GetSalesOrder.NS_InternalID)

                        If RespuestaConvert.Estatus = "Exito" Then

                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()

                            Dim RegInvoice As New Invoice_SO
                            Dim ConvertRespuesta = Convert.ToString(RespuestaConvert.Valor.internalId)

                            Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

                            RegInvoice.NS_InternalID = ConvertRespuesta
                            RegInvoice.NS_ExternalID = DatosInovice.tranId
                            RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                            RegInvoice.FechaCreacion = Date.Now
                            RegInvoice.idCustomer = GetSalesOrder.idCustomer
                            RegInvoice.Subtotal = DatosInovice.subTotal
                            RegInvoice.Total_Impuestos = DatosInovice.taxTotal
                            RegInvoice.Total = DatosInovice.total
                            RegInvoice.ImporteAdeudado = DatosInovice.total
                            RegInvoice.idSalesOrder = GetSalesOrder.idSalesOrder
                            db.Invoice_SO.Add(RegInvoice)
                            db.SaveChanges()

                        Else
                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, RespuestaConvert.Mensaje, ""))
                        End If
                    End If
#End Region

#Region "Validamos el Ultimo estatus para cambiarlo en la tabla"

                    Dim RespuestaCambioEstatus As New RespSO
                    RespuestaCambioEstatus.Estatus = GetSalesOrder.Estatus.ClaveExterna

                    If GetSalesOrder.Invoice_SO.Count <> 0 Then
                        RespuestaCambioEstatus.Factura = GetSalesOrder.Invoice_SO.First.NS_ExternalID
                    End If
#End Region

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))

                Else

#Region "Mapeo de Datos a Netsuite"
                    RegMercancia.NS_InternalID_Doc = GetSalesOrder.NS_InternalID
                    RegMercancia.Memo = "Entrega de Mercancia de la orden " + GetSalesOrder.NS_ExternalID

                    For Each reg_Mercancia In GetSalesOrder.DetalleSalesOrder

                        Dim NewMerc As New DetalleEntregaMercancia

                        NewMerc.NS_InternalID_Mercancia = reg_Mercancia.Catalogo_Productos.NS_InternalID
                        NewMerc.Cantidad = reg_Mercancia.Cantidad

                        If reg_Mercancia.Catalogo_Productos.Categoria = "Assembly" Then
                            NewMerc.TipoMaterial = "Assembly"
                            NewMerc.IssuesInventoryID = reg_Mercancia.IssuesInventory
                        ElseIf reg_Mercancia.Catalogo_Productos.Categoria = "Inventory" Then
                            NewMerc.TipoMaterial = "Inventory"
                        ElseIf reg_Mercancia.Catalogo_Productos.Categoria = "Assembly_nL" Then
                            NewMerc.TipoMaterial = "Assembly_nL"
                        ElseIf reg_Mercancia.Catalogo_Productos.Categoria = "Service" Then
                            Continue For
                        End If

                        l_Mercancia.Add(NewMerc)
                    Next

                    RegMercancia.l_Detalle = l_Mercancia
#End Region

#Region "Envio de Datos Netsuite"
                    Dim Respuesta = EjecucionMercancia.ConnectionServiceServlet(RegMercancia)
#End Region

#Region "Validar Respuesta Netsuite"
                    If Respuesta.Estatus = "Exito" Then

#Region "Cambio Estatus a Mercancia"
                        For Each reg_Mercancia In GetSalesOrder.DetalleSalesOrder
                            reg_Mercancia.CantidadEntregada = reg_Mercancia.Cantidad
                            reg_Mercancia.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Complete" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()
                        Next
#End Region

#Region "Cambio de Estatus de Sales Order"

                        ''Escenario 1 - Reparto MAF
                        ''En este caso se crea la Sales Order y se Factura, pero no se hace la ejecución de Mercancia
                        ''Como resultado, se omite por completo el convertir nuevamente la SO a Factura y solo se cambia el estatus
                        If GetSalesOrder.Estatus.ClaveExterna = "SO_FacPenEnt" Then

                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()
                        Else

                            ''Escenario 2 - Entrega o Cotización
                            ''En este caso se crea la Sales Order, pero no se realizo la factura
                            ''Como resultado, se realiza todo el proceso

                            ''Cambiamos el estatus de estatus a entrega de Mercancia como respaldo
                            GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Entrega" Select n.idEstatus).FirstOrDefaultAsync()
                            db.SaveChanges()

                            ''En automatico convertimos la SO a Factura
                            Dim RespuestaConvert = H_SalesOrder.TransformSalesOrderToInvoice(GetSalesOrder.NS_InternalID)

                            If RespuestaConvert.Estatus = "Exito" Then

                                GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()

                                Dim RegInvoice As New Invoice_SO
                                Dim ConvertRespuesta = Convert.ToString(RespuestaConvert.Valor.internalId)

                                Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

                                RegInvoice.NS_InternalID = ConvertRespuesta
                                RegInvoice.NS_ExternalID = DatosInovice.tranId
                                RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                                RegInvoice.FechaCreacion = Date.Now
                                RegInvoice.idCustomer = GetSalesOrder.idCustomer
                                RegInvoice.Subtotal = DatosInovice.subTotal
                                RegInvoice.Total_Impuestos = DatosInovice.taxTotal
                                RegInvoice.Total = DatosInovice.total
                                RegInvoice.ImporteAdeudado = DatosInovice.total
                                RegInvoice.idSalesOrder = GetSalesOrder.idSalesOrder
                                db.Invoice_SO.Add(RegInvoice)
                                db.SaveChanges()

                            Else
                                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, RespuestaConvert.Mensaje, ""))
                            End If
                        End If
#End Region

#Region "Validamos el Ultimo estatus para cambiarlo en la tabla"

                        Dim RespuestaCambioEstatus As New RespSO
                        RespuestaCambioEstatus.Estatus = GetSalesOrder.Estatus.ClaveExterna

                        If GetSalesOrder.Invoice_SO.Count <> 0 Then
                            RespuestaCambioEstatus.Factura = GetSalesOrder.Invoice_SO.First.NS_ExternalID
                        End If
#End Region

                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))
                    Else
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, Respuesta.Mensaje, ""))
                    End If
#End Region

                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la entrega de mercancia, Detalles: " + ex.Message))
            End Try

        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function CrearFacturaPorOrdenVenta(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim RegMercancia As New EntregaMercancia
                Dim GenFactura As New SalesOrderHelper
                Dim FacturasHelp As New InvoiceHelper
                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

                Dim GetSalesOrder = Await (From n In db.SalesOrder Where n.idSalesOrder = id).FirstOrDefaultAsync()

                If IsNothing(GetSalesOrder) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If

                Dim Respuesta = GenFactura.TransformSalesOrderToInvoice(GetSalesOrder.NS_InternalID)

                If Respuesta.Estatus = "Exito" Then

                    GetSalesOrder.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()

                    Dim RegInvoice As New Invoice_SO
                    Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

                    Dim DatosInovice = FacturasHelp.GetInvoiceByID(ConvertRespuesta)

                    RegInvoice.NS_InternalID = ConvertRespuesta
                    RegInvoice.NS_ExternalID = DatosInovice.tranId
                    RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                    RegInvoice.FechaCreacion = Date.Now
                    RegInvoice.idCustomer = GetSalesOrder.idCustomer
                    RegInvoice.Subtotal = DatosInovice.subTotal
                    RegInvoice.Total_Impuestos = DatosInovice.taxTotal
                    RegInvoice.Total = DatosInovice.total
                    RegInvoice.ImporteAdeudado = DatosInovice.total
                    RegInvoice.idSalesOrder = GetSalesOrder.idSalesOrder
                    db.Invoice_SO.Add(RegInvoice)
                    db.SaveChanges()

                    Dim RespuestaCambioEstatus As New RespSO
                    RespuestaCambioEstatus.Estatus = "SO_Facturada"
                    RespuestaCambioEstatus.Factura = DatosInovice.tranId

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, Respuesta.Mensaje, ""))
                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la factura. Detalles: " + ex.Message, ""))
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

        <HttpPost()>
        Public Function GetArticuloClave(ByVal id As String) As JsonResult

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

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Async Function RecuperarFactura(ByVal NS_ID As String) As Task(Of JsonResult)
            Try
                Dim RegMercancia As New EntregaMercancia
                Dim GenFactura As New SalesOrderHelper
                Dim FacturasHelp As New InvoiceHelper
                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)
                Dim NS = Convert.ToInt32(NS_ID)

                Dim GetSalesOrder = Await (From n In db.SalesOrder Where n.idSalesOrder = NS).FirstOrDefaultAsync()

                If IsNothing(GetSalesOrder) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If

                Dim Respuesta = H_Invoices.GetListInvoiceFromSalesOrder(GetSalesOrder.NS_InternalID)

                If Respuesta.Count <> 0 Then
                    For Each RegFactura As Invoice In Respuesta

                        Dim ValidarFactura = Await (From n In db.Invoice_SO Where n.NS_InternalID = RegFactura.internalId).FirstOrDefaultAsync()

                        If IsNothing(ValidarFactura) Then
                            Dim RegInvoice As New Invoice_SO

                            RegInvoice.NS_InternalID = RegFactura.internalId
                            RegInvoice.NS_ExternalID = RegFactura.tranId
                            RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                            RegInvoice.FechaCreacion = Date.Now
                            RegInvoice.idCustomer = GetSalesOrder.idCustomer
                            RegInvoice.Subtotal = GetSalesOrder.SubTotal
                            RegInvoice.Total_Impuestos = RegFactura.taxTotal
                            RegInvoice.Total = RegFactura.total
                            RegInvoice.ImporteAdeudado = RegFactura.total
                            RegInvoice.idSalesOrder = GetSalesOrder.idSalesOrder
                            db.Invoice_SO.Add(RegInvoice)
                            db.SaveChanges()

                            Dim RespuestaCambioEstatus As New RespSO
                            RespuestaCambioEstatus.Estatus = "SO_Facturada"
                            RespuestaCambioEstatus.Factura = RegFactura.tranId
                            RespuestaCambioEstatus.NS_ID = RegFactura.internalId

                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "Se ha recuperado la factura con exito...", RespuestaCambioEstatus))

                        Else

                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La factura ya existe..."))
                        End If
                    Next
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la factura. Detalles: " + ex.Message, ""))
            End Try


        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function CancelarOrdenVenta(ByVal NS_ID As String) As Task(Of JsonResult)
            Try
                Dim ValidarOrdenVenta = (From n In db.SalesOrder Where n.idSalesOrder = NS_ID And n.Estatus.ClaveExterna = "SO_Creada").FirstOrDefault()

                If IsNothing(ValidarOrdenVenta) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta valida para este proceso.", ""))
                End If

                Dim Respuesta = H_SalesOrder.CancelSalesOrder(ValidarOrdenVenta.NS_InternalID)

                If Respuesta.Estatus = "Exito" Then

                    Dim RegistrarCancelacion As New LogCancelaciones
                    RegistrarCancelacion.Usuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.Nombre).FirstOrDefault()
                    RegistrarCancelacion.FolioSalesOrder = ValidarOrdenVenta.NS_ExternalID
                    db.LogCancelaciones.Add(RegistrarCancelacion)

                    ValidarOrdenVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveExterna = "SO_Cancelado" Select n.idEstatus).FirstOrDefaultAsync()
                    db.SaveChanges()

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "Se ha generado la cancelación con exito.", New With {.Estatus = "Cancelado", .idSO = ValidarOrdenVenta.NS_InternalID}))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al cancelar esta orden de venta. Detalles: " + Respuesta.Mensaje, ""))
                End If
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message, ""))
            End Try
        End Function

        '<HttpPost()>
        Public Function SincronizarOrdenVenta() As JsonResult
            Try
                Dim id As String = "639079"
                Dim ConsultaOrdenVenta As New SalesOrderMapViewModel
                Dim l_Detalle As New List(Of DetalleSalesOrderMapViewModel)
                Dim OrdenVenta As New SalesOrder
                Dim detalleOrden As New List(Of DetalleSalesOrder)
                'Dim OrdenVenta = (From n In db.SalesOrder Where n.NS_InternalID = id).FirstOrDefault()
                'Dim l_DetalleOrdenVenta = Await Consulta.ConsultarDetalleOrden(id)

                'If IsNothing(OrdenVenta) Then
                '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No fue posible localizar la orden de venta..."))
                'End If

                Dim Invoice = H_SalesOrder.GetSalesOrderByID(id)

                OrdenVenta.SubTotal = Invoice.subTotal
                OrdenVenta.Total = Invoice.total
                OrdenVenta.TotalImpuestos = Invoice.taxTotal
                OrdenVenta.NS_ExternalID = Invoice.tranId
                OrdenVenta.idCustomer = (From n In db.Customers Where n.NS_InternalID = Invoice.entity.internalId Select n.idCustomer).FirstOrDefault()
                OrdenVenta.idUbicacion = (From n In db.Ubicaciones Where n.NS_InternalID = Invoice.location.internalId Select n.idUbicacion).FirstOrDefault()
                OrdenVenta.Nota = Invoice.memo
                OrdenVenta.NS_InternalID = Invoice.internalId
                OrdenVenta.FechaCreacion = Date.Now

                If Invoice.status = "Facturado" Then
                    OrdenVenta.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefault()

                ElseIf Invoice.status = "Facturación pendiente" Then


                ElseIf Invoice.status = "Ejecución de la orden pendiente" Then
                    OrdenVenta.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefault()

                End If

                For Each detalle As SalesOrderItem In Invoice.itemList.item
                    Dim RegDetalleOrden As New DetalleSalesOrder

                    RegDetalleOrden.idProducto = (From n In db.Catalogo_Productos Where detalle.item.internalId = n.NS_InternalID Select n.idProducto).FirstOrDefault()
                    RegDetalleOrden.Cantidad = detalle.quantity
                    RegDetalleOrden.UbicacionAlmacen = (From n In db.Ubicaciones Where n.NS_InternalID = Invoice.location.internalId Select n.idUbicacion).FirstOrDefault()
                    RegDetalleOrden.Importe = detalle.rate
                    RegDetalleOrden.Total = detalle.amount
                    RegDetalleOrden.CantidadEntregada = detalle.quantityFulfilled

                    If detalle.quantity = detalle.quantityFulfilled Then
                        RegDetalleOrden.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "Merc_Complete" Select n.idEstatus).FirstOrDefault()

                    ElseIf detalle.quantityFulfilled = 0 Then
                        RegDetalleOrden.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "Merc_Creada" Select n.idEstatus).FirstOrDefault()

                    Else
                        RegDetalleOrden.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "Merc_Creada" Select n.idEstatus).FirstOrDefault()
                    End If

                    detalleOrden.Add(RegDetalleOrden)
                Next

                OrdenVenta.DetalleSalesOrder = detalleOrden
                db.SalesOrder.Add(OrdenVenta)
                db.SaveChanges()
                'ConsultaOrdenVenta = AutoMap.AutoMapperSalesOrder(OrdenVenta)
                'l_Detalle = AutoMap.AutoMapperDetallePedido(l_DetalleOrdenVenta)

                'ConsultaOrdenVenta.l_detalle = l_Detalle

                'If ConsultaOrdenVenta.Estatus = "SO_Facturada" Then

                '    Dim ActualizaInfo = Await (From n In db.Invoice_SO Where n.NS_InternalID = ConsultaOrdenVenta.NS_Val_INV).FirstOrDefaultAsync()

                '    If IsNothing(ActualizaInfo.UUID) Then
                '        ''Validamos si tiene timbre PRUEBA
                '        Dim ValidarDatos = H_Invoices.GetSATFieldsForInvoice(ConsultaOrdenVenta.NS_Val_INV)

                '        If ValidarDatos.Estatus = "Exito" Then

                '            Dim Result As DatosSat = ValidarDatos.Valor


                '            ActualizaInfo.UUID = Result.UUID
                '            ActualizaInfo.NS_PDF = Result.NS_PDF
                '            ActualizaInfo.NS_XML = Result.NS_XML
                '            db.SaveChanges()

                '            ConsultaOrdenVenta.UUID = Result.UUID
                '            ConsultaOrdenVenta.NS_ID_pdf = Result.NS_PDF
                '            ConsultaOrdenVenta.NS_ID_xml = Result.NS_XML
                '        End If

                '    Else
                '        ConsultaOrdenVenta.UUID = ActualizaInfo.UUID
                '        ConsultaOrdenVenta.NS_ID_pdf = ActualizaInfo.NS_PDF
                '        ConsultaOrdenVenta.NS_ID_xml = ActualizaInfo.NS_XML
                '    End If
                'End If

                'Dim Invoice = H_Invoices.GetInvoiceByID(OrdenVenta.NS_InternalID)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", ConsultaOrdenVenta))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar la orden de venta, contacte con el administrador..."))
            End Try
        End Function
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

#Region "Vista Sales order Ver 2"
        Public Function ConsultarOrdenVenta() As ActionResult

            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaMostPredef.MostradorPref) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe tener un mostrador asignado.")
                    Return RedirectToAction("Index", "Home")

                Else
                    Dim GetSalesOrder As New BusquedaCorteCaja

                    GetSalesOrder.FechaDesde = Date.Now.AddDays(-2).ToString("dd/MM/yyyy")
                    GetSalesOrder.FechaHasta = Date.Now.AddDays(1).AddMinutes(-1).ToString("dd/MM/yyyy")
                    GetSalesOrder.Location_ID = ValidaMostPredef.Ubicaciones.NS_InternalID

                    Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListSalesOrderForLocation(GetSalesOrder)
                    Dim l_Busqueda As List(Of MapSalesOrder) = ValidaRespuesta.Valor
                    ViewBag.SalesOrder = l_Busqueda.OrderByDescending(Function(x) x.FechaCreacion).ToList()
                    Return View()
                End If

            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function

        Function ImprimirOrdenVentaVer2(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                'Dim ValidaPedidoCorrespondiente = (From x In db.SalesOrder Where x.idUbicacion = ValidaMostrador And x.idSalesOrder = id).FirstOrDefault

                'If IsNothing(ValidaPedidoCorrespondiente) Then
                '    Flash.Instance.Error("Este pedido no corresponde al mostrador seleccionado.")
                '    Return RedirectToAction("Index", "Home")
                'End If

                'Dim Pedido = (From x In db.SalesOrder Where x.idSalesOrder = id).FirstOrDefault()

                'If IsNothing(Pedido) Then
                '    Flash.Instance.Error("Este pedido no existe o fue eliminado. Verifique su existencia.")
                '    Return RedirectToAction("Index", "Home")
                'End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("Pedido_{0}.pdf", id)
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
        Public Async Function GetOrdenVentaVer2(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim ConsultaOrdenVenta As New SalesOrderMapViewModel
                Dim l_Detalle As New List(Of DetalleSalesOrderMapViewModel)

                Dim OrdenVenta As NetsuiteLibrary.SuiteTalk.SalesOrder = H_SalesOrder.GetSalesOrderByID(id)

                If IsNothing(OrdenVenta) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No fue posible localizar la orden de venta..."))
                End If

                For Each RegistrarItems As SalesOrderItem In OrdenVenta.itemList.item
                    Dim RegMercancia As New DetalleSalesOrderMapViewModel

                    RegMercancia.DescripcionProducto = RegistrarItems.description
                    RegMercancia.Cantidad = RegistrarItems.quantity
                    RegMercancia.CantidadEntregada = RegistrarItems.quantityFulfilled
                    RegMercancia.NumLote = ""

                    If RegistrarItems.quantity = RegistrarItems.quantityFulfilled Then
                        RegMercancia.EstatusEntrega = "Merc_Complete"

                    ElseIf RegistrarItems.quantityFulfilled <> 0 Then
                        RegMercancia.EstatusEntrega = "Merc_Parcial"

                    ElseIf RegistrarItems.quantityFulfilled = 0 Then
                        RegMercancia.EstatusEntrega = "Merc_Creada"
                    End If

                    l_Detalle.Add(RegMercancia)
                Next

                With ConsultaOrdenVenta
                    .NS_ExternalID = OrdenVenta.tranId
                    .NS_InternalID = OrdenVenta.internalId
                    .Nombre_Mostrador = OrdenVenta.location.name
                    .Fecha = OrdenVenta.createdDate.ToString("dd/MM/yyyy")
                    .Nombre_Cliente = OrdenVenta.entity.name
                    .SubTotal = OrdenVenta.subTotal
                    .Descuento = OrdenVenta.discountTotal
                    .TotalImpuestos = OrdenVenta.taxTotal
                    .Total = OrdenVenta.total
                    .Estatus = OrdenVenta.status
                    .l_detalle = l_Detalle
                End With

                'ConsultaOrdenVenta = AutoMap.AutoMapperSalesOrder(OrdenVenta)
                'l_Detalle = AutoMap.AutoMapperDetallePedido(l_DetalleOrdenVenta)

                'ConsultaOrdenVenta.l_detalle = l_Detalle

                'If ConsultaOrdenVenta.Estatus = "SO_Facturada" Then

                '    Dim ActualizaInfo = Await (From n In db.Invoice_SO Where n.NS_InternalID = ConsultaOrdenVenta.NS_Val_INV).FirstOrDefaultAsync()

                '    If IsNothing(ActualizaInfo.UUID) Then
                '        ''Validamos si tiene timbre PRUEBA
                'Dim ValidarDatos = H_Invoices.GetSATFieldsForInvoice(ConsultaOrdenVenta.NS_Val_INV)

                '        If ValidarDatos.Estatus = "Exito" Then

                '            Dim Result As DatosSat = ValidarDatos.Valor


                '            ActualizaInfo.UUID = Result.UUID
                '            ActualizaInfo.NS_PDF = Result.NS_PDF
                '            ActualizaInfo.NS_XML = Result.NS_XML
                '            db.SaveChanges()

                '            ConsultaOrdenVenta.UUID = Result.UUID
                '            ConsultaOrdenVenta.NS_ID_pdf = Result.NS_PDF
                '            ConsultaOrdenVenta.NS_ID_xml = Result.NS_XML
                '        End If

                '    Else
                '        ConsultaOrdenVenta.UUID = ActualizaInfo.UUID
                '        ConsultaOrdenVenta.NS_ID_pdf = ActualizaInfo.NS_PDF
                '        ConsultaOrdenVenta.NS_ID_xml = ActualizaInfo.NS_XML
                '    End If
                'End If

                'Dim Invoice = H_Invoices.GetInvoiceByID(OrdenVenta.NS_InternalID)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", ConsultaOrdenVenta))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar la orden de venta, contacte con el administrador..."))
            End Try

        End Function

        <HttpPost()>
        Public Function GetDatosFacturacion(ByVal id As String) As JsonResult
            Try
                Dim ConsultaOrdenVenta As New DatosSat

                Dim OrdenVenta As RespuestaServicios = H_Invoices.GetSATInvoice(id)

                If OrdenVenta.Estatus = "Fracaso" Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No fue posible localizar la factura..."))
                End If

                ConsultaOrdenVenta = OrdenVenta.Valor

                ConsultaOrdenVenta.NS_InternalID = id
                If IsNothing(ConsultaOrdenVenta.UUID) Then
                    ConsultaOrdenVenta.Estatus = "No Timbrado"
                Else
                    ConsultaOrdenVenta.Estatus = "Timbrado"
                End If

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", ConsultaOrdenVenta))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar la orden de venta, contacte con el administrador..."))
            End Try

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function CancelarOrdenVenta2(ByVal NS_ID As String, ByVal Folio As String) As Task(Of JsonResult)
            Try
                'Dim ValidarOrdenVenta = (From n In db.SalesOrder Where n.idSalesOrder = NS_ID And n.Estatus.ClaveExterna = "SO_Creada").FirstOrDefault()

                'If IsNothing(ValidarOrdenVenta) Then
                '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta valida para este proceso.", ""))
                'End If

                Dim Respuesta = H_SalesOrder.CancelSalesOrder(NS_ID)

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

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Function GenerarEjecucionTotalMercanciaVer2(ByVal id As String) As JsonResult
            Try
                Dim RegMercancia As New EntregaMercancia
                Dim EjecucionMercancia As New ConnectionServer
                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

#Region "Consulta Y Validacion de Orden de Venta"
                Dim OrdenVenta As NetsuiteLibrary.SuiteTalk.SalesOrder = H_SalesOrder.GetSalesOrderByID(id)

                If IsNothing(OrdenVenta) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If
#End Region

#Region "Validaciones"
                ''Escenario en caso que la Sales order no tenga conceptos
                If (OrdenVenta.itemList.item.Count = 0) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La orden de venta, no contiene conceptos validos.", ""))
                End If
#End Region

#Region "Mapeo de Datos"
                RegMercancia.NS_InternalID_Doc = OrdenVenta.internalId
                RegMercancia.Memo = "Entrega de Mercancia de la orden " + OrdenVenta.tranId

                For Each RegistrarItems As SalesOrderItem In OrdenVenta.itemList.item

                    If Not RegistrarItems.item.name.Contains("SERVICIOS") Then

                        Dim NewMerc As New DetalleEntregaMercancia

                        NewMerc.NS_InternalID_Mercancia = RegistrarItems.item.internalId
                        NewMerc.Cantidad = RegistrarItems.quantity
                        NewMerc.TipoMaterial = "Generic"

                        l_Mercancia.Add(NewMerc)

                    End If

                Next

                RegMercancia.l_Detalle = l_Mercancia
#End Region

#Region "Envio de Datos Netsuite"
                If l_Mercancia.Count <> 0 Then
                    Dim Respuesta = EjecucionMercancia.ConnectionServiceServlet(RegMercancia)

                    If Respuesta.Estatus = "Exito" Then

                        Dim RespuestaConvert = H_SalesOrder.TransformSalesOrderToInvoice(OrdenVenta.internalId)

                        If RespuestaConvert.Estatus = "Exito" Then

                            Dim FolioFactura = H_Search.GetTranIDInvoice(RespuestaConvert.Valor.internalID)
                            Dim RespuestaCambioEstatus As New RespSO
                            RespuestaCambioEstatus.Estatus = "SO_Facturada"
                            RespuestaCambioEstatus.Factura = FolioFactura
                            RespuestaCambioEstatus.NS_ID = RespuestaConvert.Valor.internalID

                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))
                        Else

                            Dim RespuestaCambioEstatus As New RespSO
                            RespuestaCambioEstatus.Estatus = "SO_Entrega"

                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, RespuestaConvert.Mensaje, ""))
                        End If

                    Else
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, Respuesta.Mensaje, ""))
                    End If

                Else
                    ''Caso en el que los articulos sean solo servicios, transformamos directamente la factura
                    Dim RespuestaConvert = H_SalesOrder.TransformSalesOrderToInvoice(OrdenVenta.internalId)

                    Dim FolioFactura = H_Search.GetTranIDInvoice(RespuestaConvert.Valor.internalID)
                    Dim RespuestaCambioEstatus As New RespSO
                    RespuestaCambioEstatus.Estatus = "SO_Facturada"
                    RespuestaCambioEstatus.Factura = FolioFactura
                    RespuestaCambioEstatus.NS_ID = RespuestaConvert.Valor.internalID

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))
                End If

#End Region

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la entrega de mercancia, Detalles: " + ex.Message))
            End Try

        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Public Function CrearFacturaPorOrdenVentaVer2(ByVal id As String) As JsonResult
            Try

                Dim OrdenVenta As NetsuiteLibrary.SuiteTalk.SalesOrder = H_SalesOrder.GetSalesOrderByID(id)

                If IsNothing(OrdenVenta) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar la orden de venta..."))
                End If

                Dim Respuesta = H_SalesOrder.TransformSalesOrderToInvoice(OrdenVenta.internalId)

                If Respuesta.Estatus = "Exito" Then

                    Dim RegInvoice As New Invoice_SO
                    Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

                    Dim DatosInovice = H_Search.GetTranIDInvoice(ConvertRespuesta)

                    Dim RespuestaCambioEstatus As New RespSO
                    RespuestaCambioEstatus.Estatus = "SO_Facturada"
                    RespuestaCambioEstatus.Factura = DatosInovice
                    RespuestaCambioEstatus.NS_ID = Respuesta.Valor.internalId

                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", RespuestaCambioEstatus))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, Respuesta.Mensaje, ""))
                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al generar la factura. Detalles: " + ex.Message, ""))
            End Try


        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarOrdenesVentaDisponiblesVer2(ByVal model As ConsultaSalesOrderViewModel) As JsonResult

            'Dim l_SalesOrder As New List(Of SalesOrder)
            Dim l_salesorder As New List(Of MapSalesOrder)
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

                Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListSalesOrderForLocationFilters(GetSalesOrder)

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

                    Dim ValidaRespuesta As RespuestaServicios = H_Search.GetListSalesOrderForLocationFilters(GetSalesOrder)

                    l_salesorder = ValidaRespuesta.Valor

                End If

            End If


            If Not IsNothing(model.NoPedido) Then
                l_salesorder = (From n In l_salesorder Where model.NoPedido.Contains(n.NS_ExternalID)).ToList()
            End If

            l_salesorder = l_salesorder.OrderByDescending(Function(x) x.FechaCreacion).ToList()

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_salesorder))

        End Function

        '************* Respaldo 
        '        <HttpPost>
        '        <ValidateAntiForgeryToken()>
        '        Public Async Function CrearOrdenDeVenta(ByVal model As SalesOrderViewModel, ByVal Carrito As List(Of CarritoViewModel)) As Task(Of ActionResult)

        '            Dim AltaOrden As New AltaSalesOrder
        '            Dim foliotran As String = ""
        '            Dim l_Detalle As New List(Of DetalleAltaSalesOrder)

        '#Region "Generar Combos"
        '            Dim Gentocken As Guid = Guid.NewGuid()
        '            Dim TokenCarrito As String = Gentocken.ToString()

        '            ViewBag.tokenGenCarrito = TokenCarrito
        '            Await GeneraViewbags()
        '#End Region

        '            Try

        '#Region "Validar Selección Mostrador y Validación de Ubicación"
        '                Dim UbicacionUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefaultAsync()
        '                If IsNothing(UbicacionUsuario.MostradorPref) Then
        '                    Flash.Instance.Error("Para poder generar una orden de venta, es necesario seleccionar un mostrador.")
        '                    Return View()
        '                End If

        '                'Dim GetUbicacion = Await (From n In db.Ubicaciones Where n.idUbicacion = UbicacionUsuario.MostradorPref).FirstOrDefaultAsync()
        '                'If IsNothing(GetUbicacion) Then
        '                '    Flash.Instance.Error("No fue posible localizar la ubicación del mostrador.")
        '                '    Return View()
        '                'End If

        '                Dim GetCustomer = Await (From n In db.Customers Where n.NS_InternalID = model.Customer).FirstOrDefaultAsync()
        '                If IsNothing(GetCustomer) Then
        '                    Flash.Instance.Error("Hubo un problema al localizar al cliente.")
        '                    Return View()
        '                End If
        '#End Region

        '#Region "Mapeo de Campos"
        '                AltaOrden.InternalID_Customer = model.Customer
        '                AltaOrden.Ubicacion_Venta = UbicacionUsuario.Ubicaciones.NS_InternalID
        '                AltaOrden.memo = model.Memo
        '                AltaOrden.CantidadProductos = l_Detalle.Count
        '                AltaOrden.DetalleInventario = l_Detalle
        '                AltaOrden.InternalID_MetodoPagoSAT = model.MetodoPagoSAT
        '                AltaOrden.InternalID_UsoCFDI = model.UsoCFDI
        '                AltaOrden.InternalID_FormaPagoSAT = model.FormaPagoSAT

        '                For Each RegistrarProd In Carrito
        '                    Dim Detalle As New DetalleAltaSalesOrder

        '                    Detalle.Cantidad = RegistrarProd.Cantidad
        '                    Detalle.IssuesInventoryID = RegistrarProd.IssueInventory
        '                    Detalle.UbicacionInventario = UbicacionUsuario.Ubicaciones.NS_InternalID
        '                    Detalle.InternalID = RegistrarProd.ID_NS
        '                    If RegistrarProd.TipoMaterial = "Inventory" Or RegistrarProd.TipoMaterial = "Assembly_nL" Or RegistrarProd.TipoMaterial = "Service" Then
        '                        Detalle.NumLote = ""
        '                    Else
        '                        Detalle.NumLote = RegistrarProd.NumLote.Trim()
        '                    End If

        '                    Detalle.idProducto = RegistrarProd.idPr
        '                    Detalle.TipoMaterial = RegistrarProd.TipoMaterial
        '                    Detalle.PrecioU = RegistrarProd.PrecioU
        '                    Detalle.Total = RegistrarProd.Total
        '                    l_Detalle.Add(Detalle)
        '                Next
        '#End Region

        '#Region "Envio Datos a NetSuite"
        '                Dim Resultado = H_SalesOrder.InsertSalesOrder(AltaOrden)
        '#End Region

        '#Region "Validacion de Respuesta y alta en base de datos"
        '                If Resultado.Estatus = "Exito" Then

        '                    Dim RegistrarVenta As New SalesOrder
        '                    Dim l_DetalleReg As New List(Of DetalleSalesOrder)

        '                    Dim ValidaSO = H_SalesOrder.GetSalesOrderByID(Resultado.Valor.internalId)
        '                    foliotran = ValidaSO.tranId
        '                    Dim l_conceptos = ValidaSO.itemList.item

        '                    RegistrarVenta.idCustomer = GetCustomer.idCustomer
        '                    RegistrarVenta.idUbicacion = UbicacionUsuario.Ubicaciones.idUbicacion
        '                    RegistrarVenta.NS_ExternalID = ValidaSO.tranId
        '                    RegistrarVenta.SubTotal = ValidaSO.subTotal
        '                    RegistrarVenta.Descuento = ValidaSO.discountTotal
        '                    RegistrarVenta.TotalImpuestos = ValidaSO.taxTotal
        '                    RegistrarVenta.Total = ValidaSO.total
        '                    RegistrarVenta.NS_InternalID = ValidaSO.internalId
        '                    RegistrarVenta.Nota = ValidaSO.memo
        '                    RegistrarVenta.FechaCreacion = Date.Now
        '                    RegistrarVenta.idUsuario = WebSecurity.CurrentUserId
        '                    RegistrarVenta.idMetodoPagoSAT = Await (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = model.MetodoPagoSAT Select n.idMetodoPagoSAT).FirstOrDefaultAsync()
        '                    RegistrarVenta.idFormaPago = Await (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = model.FormaPagoSAT Select n.idFormaPago).FirstOrDefaultAsync()
        '                    RegistrarVenta.idUsoCFDI = Await (From n In db.Catalogo_UsoCFDI Where n.NS_InternalID = model.UsoCFDI Select n.idUsoCFDI).FirstOrDefaultAsync()
        '                    RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefaultAsync()

        '                    For Each RegistrarMerc In l_Detalle
        '                        Dim RegMerc As New DetalleSalesOrder

        '                        RegMerc.idProducto = RegistrarMerc.idProducto
        '                        RegMerc.Cantidad = RegistrarMerc.Cantidad
        '                        RegMerc.UbicacionAlmacen = UbicacionUsuario.Ubicaciones.idUbicacion
        '                        RegMerc.NumLote = RegistrarMerc.NumLote
        '                        RegMerc.IssuesInventory = RegistrarMerc.IssuesInventoryID
        '                        RegMerc.Importe = Convert.ToDecimal(RegistrarMerc.PrecioU)
        '                        RegMerc.Total = Convert.ToDecimal(RegistrarMerc.Total)

        '                        If RegistrarMerc.TipoMaterial = "Service" Then
        '                            RegMerc.CantidadEntregada = RegistrarMerc.Cantidad
        '                            RegMerc.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Complete" Select n.idEstatus).FirstOrDefaultAsync()
        '                        Else
        '                            RegMerc.CantidadEntregada = 0
        '                            RegMerc.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Creada" Select n.idEstatus).FirstOrDefaultAsync()
        '                        End If


        '                        l_DetalleReg.Add(RegMerc)
        '                    Next

        '                    RegistrarVenta.DetalleSalesOrder = l_DetalleReg

        '                    ''En caso de que la factura se genere directamente, se hara este proceso
        '                    If model.GenerarFacturaDirecta = True Then
        '                        'Dim Respuesta = H_SalesOrder.TransformSalesOrderToInvoice(Resultado.Valor.internalId)

        '                        'If Respuesta.Estatus = "Exito" Then

        '                        '    If l_Detalle.Count = 1 And l_Detalle.First.TipoMaterial = "Service" Then
        '                        '        RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
        '                        '    Else
        '                        '        RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_FacPenEnt" Select n.idEstatus).FirstOrDefaultAsync()
        '                        '    End If

        '                        '    Dim RegInvoice As New Invoice_SO
        '                        '    Dim l_Factura As New List(Of Invoice_SO)
        '                        '    Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

        '                        '    Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

        '                        '    RegInvoice.NS_InternalID = ConvertRespuesta
        '                        '    RegInvoice.NS_ExternalID = DatosInovice.tranId
        '                        '    RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
        '                        '    RegInvoice.FechaCreacion = Date.Now
        '                        '    RegInvoice.idCustomer = GetCustomer.idCustomer
        '                        '    RegInvoice.Subtotal = DatosInovice.subTotal
        '                        '    RegInvoice.Total_Impuestos = DatosInovice.taxTotal
        '                        '    RegInvoice.Total = DatosInovice.total
        '                        '    RegInvoice.ImporteAdeudado = DatosInovice.amountRemaining
        '                        '    RegInvoice.idUbicacion = UbicacionUsuario.MostradorPref
        '                        '    l_Factura.Add(RegInvoice)

        '                        '    RegistrarVenta.Invoice_SO = l_Factura

        '                        '    Flash.Instance.Success(String.Format("Se ha creado la orden de venta No. {0} con exito!", ValidaSO.tranId))
        '                        'Else

        '                        '    RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefaultAsync()
        '                        '    Flash.Instance.Warning("Se ha creado la orden de venta No. " + ValidaSO.tranId + " Pero hubo un problema al generar la factura. Detalles: " + Respuesta.Valor)
        '                        'End If

        '                    ElseIf model.GenerarVentaDirecta = True Then

        '#Region "Generar Factura"
        '                        Dim Respuesta = H_SalesOrder.TransformSalesOrderToInvoice(Resultado.Valor.internalId)

        '                        If Respuesta.Estatus = "Exito" Then

        '                            If l_Detalle.Count = 1 And l_Detalle.First.TipoMaterial = "Service" Then
        '                                RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
        '                            Else
        '                                RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_FacPenEnt" Select n.idEstatus).FirstOrDefaultAsync()
        '                            End If

        '                            Dim RegInvoice As New Invoice_SO
        '                            Dim l_Factura As New List(Of Invoice_SO)
        '                            Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

        '                            Dim DatosInovice = H_Invoices.GetInvoiceByID(ConvertRespuesta)

        '                            RegInvoice.NS_InternalID = ConvertRespuesta
        '                            RegInvoice.NS_ExternalID = DatosInovice.tranId
        '                            RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
        '                            RegInvoice.FechaCreacion = Date.Now
        '                            RegInvoice.idCustomer = GetCustomer.idCustomer
        '                            RegInvoice.Subtotal = DatosInovice.subTotal
        '                            RegInvoice.Total_Impuestos = DatosInovice.taxTotal
        '                            RegInvoice.Total = DatosInovice.total
        '                            RegInvoice.ImporteAdeudado = DatosInovice.amountRemaining
        '                            RegInvoice.idUbicacion = UbicacionUsuario.MostradorPref
        '                            l_Factura.Add(RegInvoice)

        '                            RegistrarVenta.Invoice_SO = l_Factura

        '                            If l_Detalle.Count = 1 And l_Detalle.First.TipoMaterial = "Service" Then

        '                                Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
        '                            Else

        '#Region "Entrega de Mercancia"
        '                                Dim RegMercancia As New EntregaMercancia
        '                                Dim EjecucionMercancia As New ConnectionServer
        '                                Dim l_Mercancia As New List(Of DetalleEntregaMercancia)

        '                                RegMercancia.NS_InternalID_Doc = ValidaSO.internalId
        '                                RegMercancia.Memo = "Entrega de Mercancia de la orden " + ValidaSO.tranId

        '                                For Each reg_Mercancia In l_Detalle

        '                                    Dim NewMerc As New DetalleEntregaMercancia

        '                                    NewMerc.NS_InternalID_Mercancia = reg_Mercancia.InternalID
        '                                    NewMerc.Cantidad = reg_Mercancia.Cantidad

        '                                    If reg_Mercancia.TipoMaterial = "Assembly" Then
        '                                        NewMerc.TipoMaterial = "Assembly"
        '                                        NewMerc.IssuesInventoryID = reg_Mercancia.IssuesInventoryID
        '                                    ElseIf reg_Mercancia.TipoMaterial = "Inventory" Then
        '                                        NewMerc.TipoMaterial = "Inventory"
        '                                    ElseIf reg_Mercancia.TipoMaterial = "Assembly_nL" Then
        '                                        NewMerc.TipoMaterial = "Assembly_nL"
        '                                    ElseIf reg_Mercancia.TipoMaterial = "Service" Then
        '                                        Continue For
        '                                    End If

        '                                    l_Mercancia.Add(NewMerc)
        '                                Next

        '                                RegMercancia.l_Detalle = l_Mercancia

        '#Region "Envio de Datos Netsuite"
        '                                Dim RespuestaEnvio = EjecucionMercancia.ConnectionServiceServlet(RegMercancia)

        '                                If RespuestaEnvio.Estatus = "Exito" Then
        '                                    For Each reg_Mercancia In RegistrarVenta.DetalleSalesOrder
        '                                        reg_Mercancia.CantidadEntregada = reg_Mercancia.Cantidad
        '                                        reg_Mercancia.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Complete" Select n.idEstatus).FirstOrDefaultAsync()
        '                                    Next

        '                                    RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefaultAsync()
        '                                    Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
        '                                Else
        '                                    Flash.Instance.Warning(Respuesta.Mensaje)
        '                                End If
        '#End Region

        '#End Region
        '                            End If


        '                        Else

        '                            RegistrarVenta.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefaultAsync()
        '                            Flash.Instance.Warning("Se ha creado la orden de venta No. " + ValidaSO.tranId + " Pero hubo un problema al generar la factura. Detalles: " + Respuesta.Valor)
        '                        End If
        '#End Region


        '                    Else
        '                        Flash.Instance.Success("Se ha creado la orden de venta con exito! " + ValidaSO.tranId)
        '                    End If

        '                    Try

        '                        db.Database.CommandTimeout = 200000
        '                        db.SalesOrder.Add(RegistrarVenta)
        '                        db.SaveChanges()

        '                    Catch ex As Exception
        '                        Dim mensaje = ex.Message
        '                        Dim detallesExtras = ex.InnerException.Message
        '                        Dim constuirMensajeError As String = String.Format("Hubo un problema al almacenar la sales order no. {0}, Detalles: {1}, Extras: {2}", ValidaSO.tranId, mensaje, detallesExtras)
        '                        Dim guardarlog As New LogSalesOrder
        '                        guardarlog.Descripcion = constuirMensajeError
        '                        guardarlog.NS_ExternalID = ValidaSO.tranId
        '                        db.Database.CommandTimeout = 200000
        '                        db.LogSalesOrder.Add(guardarlog)
        '                        db.SaveChanges()
        '                        'For Each sa In ex.EntityValidationErrors
        '                        '    For Each bs In sa.ValidationErrors
        '                        '        Dim st1 As String = bs.PropertyName
        '                        '        Dim st2 As String = bs.ErrorMessage


        '                        '        Dim PathLog As String
        '                        '        Dim Registro As String = String.Empty
        '                        '        PathLog = "C:\temp" + "\" + String.Format("LogMailError_{0}.txt", Date.Now.ToString("ddMMyyyy_HHmmss"))
        '                        '        System.IO.File.WriteAllText(PathLog, ex.Message + "Detalles: " + st1 + " - " + st2)
        '                        '    Next
        '                        'Next
        '                    End Try


        '                    ''Proceso para apartar la mercancia

        '                    'For Each ActualizaStockVenta In l_Detalle
        '                    '    If ActualizaStockVenta.TipoMaterial <> "Service" Then
        '                    '        Dim stock

        '                    '        If ActualizaStockVenta.TipoMaterial = "Inventory" Or ActualizaStockVenta.TipoMaterial = "Assembly_nL" Then
        '                    '            stock = Await (From n In db.StockDisponible Where n.idUbicacion = UbicacionUsuario.Ubicaciones.idUbicacion And n.Catalogo_Productos.NS_InternalID = ActualizaStockVenta.InternalID).FirstOrDefaultAsync
        '                    '        Else
        '                    '            stock = Await (From n In db.StockDisponible Where n.IssueInventory = ActualizaStockVenta.IssuesInventoryID And n.idUbicacion = UbicacionUsuario.Ubicaciones.idUbicacion And n.Catalogo_Productos.NS_InternalID = ActualizaStockVenta.InternalID).FirstOrDefaultAsync
        '                    '        End If

        '                    '        Dim stockdisp = stock.StockDisponible1 - Convert.ToDecimal(ActualizaStockVenta.Cantidad)

        '                    '        If stockdisp <= 0 Then
        '                    '            stock.StockDisponible1 = 0
        '                    '        Else
        '                    '            stock.StockDisponible1 = stockdisp
        '                    '        End If

        '                    '        db.SaveChanges()
        '                    '    End If

        '                    'Next
        '                    model.GenerarFacturaDirecta = False
        '                    model.GenerarVentaDirecta = False
        '                    Return RedirectToAction("ConsultarOrdenDeVenta")
        '                Else

        '                    'Dim RegistrarVenta As New Resp_SalesOrder
        '                    'Dim l_DetalleReg As New List(Of Resp_DetalleSalesOrder)

        '                    '''Generamos un respaldo del pedido para 
        '                    'RegistrarVenta.idCustomer = GetCustomer.idCustomer
        '                    'RegistrarVenta.idUbicacion = UbicacionUsuario.Ubicaciones.idUbicacion
        '                    'RegistrarVenta.FechaCreacion = Date.Now
        '                    'RegistrarVenta.idUsuario = WebSecurity.CurrentUserId
        '                    'RegistrarVenta.idMetodoPagoSAT = Await (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = model.MetodoPagoSAT Select n.idMetodoPagoSAT).FirstOrDefaultAsync()
        '                    'RegistrarVenta.idFormaPago = Await (From n In db.Catalogo_FormasPagoSAT Where n.NS_InternalID = model.FormaPagoSAT Select n.idFormaPago).FirstOrDefaultAsync()
        '                    'RegistrarVenta.idUsoCFDI = Await (From n In db.Catalogo_UsoCFDI Where n.NS_InternalID = model.UsoCFDI Select n.idUsoCFDI).FirstOrDefaultAsync()

        '                    'For Each RegistrarMerc In l_Detalle
        '                    '    Dim RegMerc As New Resp_DetalleSalesOrder

        '                    '    RegMerc.idProducto = RegistrarMerc.idProducto
        '                    '    RegMerc.Cantidad = RegistrarMerc.Cantidad
        '                    '    RegMerc.UbicacionAlmacen = UbicacionUsuario.Ubicaciones.idUbicacion
        '                    '    RegMerc.NumLote = RegistrarMerc.NumLote
        '                    '    RegMerc.CantidadEntregada = 0
        '                    '    RegMerc.IssuesInventory = RegistrarMerc.IssuesInventoryID
        '                    '    RegMerc.Importe = Convert.ToDecimal(RegistrarMerc.PrecioU)
        '                    '    RegMerc.Total = Convert.ToDecimal(RegistrarMerc.Total)
        '                    '    RegMerc.idEstatus = Await (From n In db.Estatus Where n.ClaveInterna = "Merc_Creada" Select n.idEstatus).FirstOrDefaultAsync()

        '                    '    l_DetalleReg.Add(RegMerc)
        '                    'Next

        '                    'RegistrarVenta.Resp_DetalleSalesOrder = l_DetalleReg

        '                    'Try
        '                    '    db.Resp_SalesOrder.Add(RegistrarVenta)
        '                    '    db.SaveChanges()
        '                    'Catch ex As Exception

        '                    'End Try

        '                    Dim errormensaje = Resultado.Valor.ToString().ToLower
        '                    ''Vaciamos el carrito
        '                    Session("Carrito") = Nothing
        '                    model.GenerarFacturaDirecta = False
        '                    model.GenerarVentaDirecta = False
        '                    Flash.Instance.Error(errormensaje)
        '                    Return RedirectToAction("ConsultarOrdenDeVenta")
        '                End If
        '#End Region

        '            Catch ex As Exception
        '                model.GenerarFacturaDirecta = False
        '                model.GenerarVentaDirecta = False

        '                Dim mensaje = ex.Message
        '                Dim detallesExtras = ex.InnerException.Message
        '                Dim constuirMensajeError As String = String.Format("Hubo un problema al almacenar la sales order no. {0}, Detalles: {1}, Extras: {2}", foliotran, mensaje, detallesExtras)
        '                Dim guardarlog As New LogSalesOrder
        '                guardarlog.Descripcion = constuirMensajeError
        '                guardarlog.NS_ExternalID = foliotran
        '                db.Database.CommandTimeout = 200000
        '                db.LogSalesOrder.Add(guardarlog)
        '                db.SaveChanges()

        '                Flash.Instance.Error("Hubo un problema al procesar esta solicitud. Detalles: " + ex.Message)
        '                Return RedirectToAction("Index", "Home")
        '            End Try

        '        End Function

#End Region

#Region "Carrito"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function CrearListaProductos(ByVal model As CarritoViewModel) As JsonResult

            Dim tempDataDictionary = Session(model.Token)
            Dim Vlender = Session("Vlender")

            Session("Vlender") = Session("Vlender") + 1

            If IsNothing(tempDataDictionary) Then

                Dim l_CarritoRender As New List(Of CarritoViewModel)

                ''Generar numero de producto unico
                Dim random As New Random()

                model.Total = Math.Round(Convert.ToDecimal(model.Cantidad) * Convert.ToDecimal(model.PrecioU), 2)
                model.Vlender = Session("Vlender")

                l_CarritoRender.Add(model)

                Session(model.Token) = l_CarritoRender
            Else
                Dim l_CarritoRender As New List(Of CarritoViewModel)

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
            Dim l_CarritoRender As New List(Of CarritoViewModel)

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

#Region "DB Dispose"
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal args As EventArgs)
            'If Not IsPostBack Then
            '    ViewState("ViewStateId") = System.Guid.NewGuid().ToString()
            '    Session("SessionId") = ViewState("ViewStateId").ToString()
            'Else

            '    If ViewState("ViewStateId").ToString() <> Session("SessionId").ToString() Then
            '        isPageRefreshed = True
            '    End If

            '    Session("SessionId") = System.Guid.NewGuid().ToString()
            '    ViewState("ViewStateId") = Session("SessionId").ToString()
            'End If
        End Sub
#End Region

    End Class
End Namespace
