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
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class ParametrosController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
#End Region

#Region "Vistas"
        Public Async Function ConsultarParametros() As Task(Of ActionResult)

            ViewBag.Parametros = Await Consulta.ConsultarParametros()

            Return View()
        End Function

        Public Function ConsultarEstatusNetsuite() As ActionResult

            Return View()
        End Function

#End Region

#Region "Guias de Ayuda"
        Public Function DescargarManualAltaClientes()

            Dim Path_Guia = Server.MapPath("~\Guias\") + "REGISTRO DE CLIENTES NUEVO PVO.pdf"

            If System.IO.File.Exists(Path_Guia) Then
                Dim fileStream = New FileStream(Path_Guia,
   FileMode.Open,
   FileAccess.Read)
                Dim fsResult = New FileStreamResult(fileStream, "application/pdf")
                fsResult.FileDownloadName = Path.GetFileName(Path_Guia)
                Return fsResult
            Else
                Flash.Instance.Error("La guia no se encuentra disponible para descargar.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function
#End Region

#Region "Procesos"
        <HttpPost>
        Public Async Function EditarParametros(ByVal model As ParametrosViewModel) As Task(Of JsonResult)
            Try
                Dim l_Parametros As New List(Of Parametros)
                Dim l_ParametrosView As New List(Of ParametrosViewModel)
                Dim Parametro As Parametros = Await (From n In db.Parametros Where n.idParametro = model.idParametro).FirstOrDefaultAsync()

                Parametro.Valor = model.Valor
                Parametro.Descripcion = model.Descripcion

                db.SaveChanges()

                ''Utilizaremos el metodo de muestra en la vista parcial
                l_Parametros = Await Consulta.ConsultarParametros

                l_ParametrosView = AutoMap.AutoMapperListaParametros(l_Parametros)


                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_ParametrosView))
            Catch ex As Exception

            End Try
        End Function

        <HttpPost>
        Public Async Function ConsultarParametro(ByVal id As String) As Task(Of JsonResult)
            Try
                Dim Parametro As Parametros = Await Consulta.ConsultarParametroEspecifico(id)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", New With {.Valor = Parametro.Valor, .Descripcion = Parametro.Descripcion}))
            Catch ex As Exception

            End Try
        End Function
#End Region

#Region "Pruebas"

        Public Function contactoprueba()
            Dim n As New SearchHelper()

            Dim busqueda As New BusquedaCorteCaja

            busqueda.Location_ID = "4"

            Dim s = n.PruebasProton(busqueda)
            'Dim s = n.UpdateCustomerTest()
        End Function
        Public Function getarticulo()
            Dim n As New CustomerHelper()

            Dim s = n.GetCustomerById("12334")
            'Dim s = n.UpdateCustomerTest()
        End Function

        Public Function Getmostradoritems()

            Dim n As New SearchHelper()

            Dim s = n.GetTranIDInvoice("113093")

            Dim a = ""
            'Dim s = n.UpdateCustomerTest()
        End Function

        <AllowAnonymous>
        Public Function levelpricing()
            Dim MostradorUsuario = "4"
            Dim MostradorCedis = "2"
            Dim s As New SearchHelper()
            Dim l_MapStock As New List(Of StockArticulosViewModel)

            ''Obtener todos los articulos
            Dim l_productos = (From n In db.Catalogo_Productos).ToList()

            Dim l_ArticulosPrecio As RespuestaServicios = s.GetListAllItemsMRP("2678")
            Dim l_StockDisponible As RespuestaServicios = s.GetListAllStockMRP("2679")

            Dim l_Precio As List(Of MapListaArticulos) = l_ArticulosPrecio.Valor
            Dim l_stock As List(Of MapListaArticulos) = l_StockDisponible.Valor

            Dim l_MostradorUsuario = (From n In l_stock Where MostradorUsuario.Contains(n.InternalID_Location)).ToList()
            Dim l_MostradorCedis = (From n In l_stock Where MostradorCedis.Contains(n.InternalID_Location)).ToList()

            For Each RegProductos In l_productos
                Dim RegistrarDatos As New StockArticulosViewModel

                Dim Stock_Mostrador = (From n In l_MostradorUsuario Where n.InternalID = RegProductos.NS_InternalID).FirstOrDefault()
                Dim Stock_Cedis = (From n In l_MostradorCedis Where n.InternalID = RegProductos.NS_InternalID).FirstOrDefault()
                Dim PrecioBase = (From n In l_Precio Where n.InternalID = RegProductos.NS_InternalID).FirstOrDefault()

                RegistrarDatos.ClaveProducto = RegProductos.NS_ExternalID
                RegistrarDatos.Descripcion = RegProductos.Descripcion

                If IsNothing(Stock_Mostrador) Then
                    RegistrarDatos.stock_Mostrador = 0
                Else
                    RegistrarDatos.stock_Mostrador = Stock_Mostrador.StockDisponible
                End If

                If IsNothing(Stock_Cedis) Then
                    RegistrarDatos.stock_Cedis = 0
                Else
                    RegistrarDatos.stock_Cedis = Stock_Cedis.StockDisponible
                End If

                If IsNothing(PrecioBase) Then
                    RegistrarDatos.precioBase = 0
                Else
                    RegistrarDatos.precioBase = PrecioBase.PrecioBase
                End If

                l_MapStock.Add(RegistrarDatos)
            Next

            ViewBag.datos = l_MapStock
        End Function

        <AllowAnonymous>
        Public Function modinv()
            Dim s As New CustomerHelper()

            Dim l As NetsuiteLibrary.SuiteTalk.Customer = s.GetCustomerByName("VENTA AL PÚBLICO MOSTRADOR LEÓN")
        End Function
        <AllowAnonymous>
        Public Function Crearcliente()
            Dim s As New CustomerHelper

            Dim l = s.UpdateCustomerTest()
        End Function

        <AllowAnonymous>
        Public Function Getuuid()
            Dim s As New SalesOrderHelper

            Dim re = s.TransformSalesOrderToReturnAutorization("111555")
        End Function

        <AllowAnonymous>
        Public Function gettOrderTraslado()
            Dim search As New TransferOrderHelper

            Dim s = search.GetTransferOrderByID("106969")

        End Function

        <AllowAnonymous>
        Public Function gettitemreceipt()
            Dim search As New TransferOrderHelper

            Dim s = search.GetItemReceiptByID("107177")

        End Function

        <AllowAnonymous>
        Public Function insertOrderTraslado()
            Dim search As New TransferOrderHelper

            Dim s = search.InsertTransferOrder()

        End Function

        <AllowAnonymous>
        Public Function ejecutarTraslado()
            Dim n As New ConnectionServer
            Dim datosejecucion As New EntregaMercancia
            Dim regdetalle As New DetalleEntregaMercancia
            Dim l_detalle As New List(Of DetalleEntregaMercancia)

            datosejecucion.NS_InternalID_Doc = "106969"
            datosejecucion.Memo = "Ejecucion de Traslado PVO MAF 20 JG"

            regdetalle.Cantidad = 1
            regdetalle.NS_InternalID_Mercancia = 12953
            regdetalle.TipoMaterial = "Assembly_nL"
            l_detalle.Add(regdetalle)
            datosejecucion.l_Detalle = l_detalle


            Dim pruebas = n.ConnectionServiceServletTO(datosejecucion)
        End Function

        <AllowAnonymous>
        Public Function TransformarordenTrasladoRecepcion()

            Dim h_Customer As New CustomerHelper()

            Dim s = h_Customer.GetCustomerById("2674")
        End Function

        <AllowAnonymous>
        Public Function PruebaGetPagosPorUbicacion()
            Dim search As New SearchHelper
            Dim datos As New BusquedaCorteCaja

            datos.FechaDesde = Convert.ToDateTime("06/10/2022")
            datos.FechaHasta = Convert.ToDateTime("07/10/2022")
            datos.Location_ID = "4"

            Dim s = search.GetListPaymentForLocation(datos)

        End Function

        <AllowAnonymous>
        Public Function Getsc()
            Dim Helpers As New ItemFulFillmentHelper

            Dim actualizarStock As New RespuestaServicios

            actualizarStock = Helpers.GetInventoryNumber("10", "7493")

            Dim l_Stock As New List(Of ActualizaStock)
            l_Stock = actualizarStock.Valor
            For Each MaterialesStock In l_Stock
                Dim id = MaterialesStock.InventoryID
                Dim validaStock = (From n In db.StockDisponible Where n.Catalogo_Productos.NS_InternalID = "13731" And n.Ubicaciones.NS_InternalID = "10" And n.NumLote = id).FirstOrDefault()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = (From n In db.Catalogo_Productos Where n.NS_InternalID = "13731" Select n.idProducto).FirstOrDefault()
                    RegistrarStock.idUbicacion = (From n In db.Ubicaciones Where n.NS_InternalID = "10" Select n.idUbicacion).FirstOrDefault()
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)
                    RegistrarStock.NumLote = MaterialesStock.InventoryID
                    RegistrarStock.IssueInventory = MaterialesStock.IssuesID

                    db.StockDisponible.Add(RegistrarStock)
                    db.SaveChanges()
                Else
                    validaStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)

                    db.SaveChanges()

                End If
            Next

        End Function

        <AllowAnonymous>
        Public Async Function ActualizarListaPrecios() As Task(Of ActionResult)
            Dim Helpers As New ItemHelper

            Dim l_Ensambles = Await (From n In db.Catalogo_Productos Where n.Categoria = "Assembly").ToListAsync()

            For Each PrecioEnsambles In l_Ensambles

                Try
                    Dim RegistrarNivelPrecios = Helpers.GetLotNumberedAssemblyItemByID(PrecioEnsambles.NS_InternalID)

                    If Not IsNothing(RegistrarNivelPrecios.pricingMatrix) Then

                        For Each RegistrarPrecio In RegistrarNivelPrecios.pricingMatrix.pricing
                            Dim Precio As New NivelesPrecioProducto

                            ''VALIDAMOS EXISTENCIA DE NIVEL DE PRECIO

                            Dim ValidarNivelPrecioValido = Await (From n In db.NivelesPrecioProducto Where n.idProducto = PrecioEnsambles.idProducto And n.Catalogo_NivelesPrecio.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefaultAsync()

                            If IsNothing(ValidarNivelPrecioValido) Then
                                Dim GetNivelPrecio = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefaultAsync()

                                Precio.idProducto = PrecioEnsambles.idProducto
                                Precio.idCategoriaPrecio = GetNivelPrecio.idCategoriaPrecio
                                Precio.Precio = RegistrarPrecio.priceList.First.value

                                db.NivelesPrecioProducto.Add(Precio)
                                db.SaveChanges()
                            Else
                                ''Solo actualizamos el precio, si es que es diferente

                                If ValidarNivelPrecioValido.Precio <> RegistrarPrecio.priceList.First.value Then
                                    ValidarNivelPrecioValido.Precio = RegistrarPrecio.priceList.First.value
                                    db.SaveChanges()
                                End If
                            End If

                        Next

                    End If
                Catch ex As Exception
                    Continue For
                End Try
            Next

        End Function

        <AllowAnonymous>
        Public Function PruebasAlmacenMateire()
            Dim Helpers As New ItemFulFillmentHelper

            Dim actualizarStock As RespuestaServicios = Helpers.GetInventoryNumber("Acapulco", "1170")
        End Function

        <AllowAnonymous>
        Public Function MetodoPAgoUpdate()
            Dim Helpers As New InvoiceHelper

            Dim actualizarStock = Helpers.UpdateInvoice("18918")
        End Function

        <AllowAnonymous>
        Public Function GetEtiquetas()
            Dim l As New PaymentHelper
            Dim dat = l.GetListPaymentFromLocationID("10")


            Return "Proceso"
        End Function

        <AllowAnonymous>
        Public Function useCustomClient()
            Dim l As New SearchHelper

            Dim dat = l.GetListCustomerAdvancedVer()


            Return "Proceso"
        End Function

        <AllowAnonymous>
        Public Function exampleSearchItems()
            Dim l As New SearchHelper()

            Dim dat = l.GetListAssemblyItemsSearchAdvanced()
            Dim RespValues As Sinc_JobItems = dat.Valor
            Dim l_items As List(Of Sinc_AssemblyItems) = RespValues.l_Items
            Dim l_Precios As List(Of Sinc_PricingItems) = RespValues.l_Pricing

            Dim GetItems = l_items.GroupBy(Function(i) i.s_internalId).[Select](Function(g) g.First()).ToList()

            For Each RegistrarProductos In GetItems

                Dim ActualizaPrecios = (From n In l_Precios Where n.s_iteminternalId = RegistrarProductos.s_internalId).ToList()

                For Each RegPrecios In ActualizaPrecios

                    Dim ValidaExistenciaNivel = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios.s_internalIdpricing).FirstOrDefault()
                    If IsNothing(ValidaExistenciaNivel) Then
                        Continue For
                    End If

                    Dim ValidaExistenciaarticulo = (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios.s_iteminternalId).FirstOrDefault()
                    If IsNothing(ValidaExistenciaarticulo) Then
                        Continue For
                    End If

                    Dim ValidarRegistroNivelPrecio = (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefault()

                    If IsNothing(ValidarRegistroNivelPrecio) Then

                        Dim RegistrarPrecio As New NivelesPrecioProducto
                        RegistrarPrecio.idProducto = ValidaExistenciaarticulo.idProducto
                        RegistrarPrecio.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio
                        RegistrarPrecio.Precio = Convert.ToDecimal(RegPrecios.s_value)

                        db.NivelesPrecioProducto.Add(RegistrarPrecio)
                        db.SaveChanges()

                    Else
                        ValidarRegistroNivelPrecio.Precio = Convert.ToDecimal(RegPrecios.s_value)
                        db.SaveChanges()
                    End If

                Next

            Next

            Return "Proceso"
        End Function

        <AllowAnonymous>
        Public Function GetInvoiceTimbrado()
            Dim l As New InvoiceHelper
            Dim dat = l.GetInvoiceByID("19142")


            Return "Proceso"
        End Function

        <AllowAnonymous>
        Public Function Getpaymentforidpayments()
            Dim l As New SearchHelper()

            'Dim dat = l.GetListPaymentForLocationAdvancedVer("4")

            Dim s = l.GetListPaymentsForCustomSearchAdvanced()


            Return "Proceso"
        End Function


        <AllowAnonymous>
        Public Function GetDataInventario()
            Dim Helpers As New ItemFulFillmentHelper

            Dim actualizarStock As New RespuestaServicios

            actualizarStock = Helpers.GetInventoryNumber("ACAPULCO", "1170")
        End Function

        <AllowAnonymous>
        Public Function Pruebas()

            Dim Connection As New ConnectionServer
            Dim s As New SalesOrderHelper

            'Dim n = s.GetLotNumberedAssemblyItemByID("1170")

            Dim a = s.DeleteSalesOrder("103531")


            Dim l_UbicacionLote = (From n In db.Ubicaciones).ToList()
            Dim l_MaterialesAssembly = (From n In db.Catalogo_Productos).ToList()

            Dim Helpers As New ItemFulFillmentHelper

            For Each Ubicacion In l_UbicacionLote
                For Each Materiales In l_MaterialesAssembly

                    'Dim AltaLote As New PruebaStockDisponible

                    Dim actualizarStock As New RespuestaServicios

                    actualizarStock = Helpers.GetInventoryNumber(Ubicacion.DescripcionAlmacen, Materiales.NS_InternalID)

                    If actualizarStock.Estatus = "Fracaso" Then
                        Continue For
                    Else
                        Dim l_Stock As New List(Of ActualizaStock)
                        l_Stock = actualizarStock.Valor
                        For Each MaterialesStock In l_Stock
                            Dim id = MaterialesStock.InventoryID
                            Dim validaStock = (From n In db.StockDisponible Where n.idProducto = Materiales.idProducto And n.idUbicacion = Ubicacion.idUbicacion And n.NumLote = id).FirstOrDefault()

                            If IsNothing(validaStock) Then
                                Dim RegistrarStock As New StockDisponible
                                RegistrarStock.idProducto = Materiales.idProducto
                                RegistrarStock.idUbicacion = Ubicacion.idUbicacion
                                RegistrarStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)
                                RegistrarStock.NumLote = MaterialesStock.InventoryID
                                RegistrarStock.IssueInventory = MaterialesStock.IssuesID

                                db.StockDisponible.Add(RegistrarStock)
                                db.SaveChanges()
                            Else
                                validaStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)

                                db.SaveChanges()

                            End If
                        Next

                    End If

                Next
            Next



            Return "Proceso actualizado con exito!"
        End Function

        <AllowAnonymous>
        Public Function Pruebasmateria()
            Dim Item As New SearchHelper

            Dim r = Item.GetListPaymentsForCustomSearchAdvanced()
            'Dim r = Item.GetListInvoice()

            Dim resultado = r
        End Function

        <AllowAnonymous>
        Public Function invPruebas()
            Dim Item As New InvoiceHelper()

            Dim r = Item.GetListInvoiceFromSalesOrder("19916")



            Dim resultado = r
        End Function

        <AllowAnonymous>
        Public Function PruebaArticuloInventariadostock()
            Dim Item As New SalesOrderHelper

            Dim r = Item.UpdateSalesOrder("143746")
            'Dim r = Item.GetListInvoice()

            Dim resultado = r
        End Function

        <AllowAnonymous>
        Public Function GetINV()
            'Dim Item As New InvoiceHelper

            'Dim r = Item.GetInvoiceFile("93283")
            'Dim webClient As New WebClient
            'webClient.DownloadFile(r, "C:/Temp/d-ksajdnjas.pdf")

            'Dim resultado = r
        End Function

        <AllowAnonymous>
        Public Function GetINVsselvlet()

            'Dim baseFile = r.Valor

            'Dim bytes = System.Convert.FromBase64String(baseFile)
            'Dim writer As New System.IO.BinaryWriter(IO.File.Open("C:\temp\tembase641.pdf", IO.FileMode.Create))
            'writer.Write(bytes)
            'writer.Close()


        End Function

        <AllowAnonymous>
        Public Async Function GetBusquedaAvanzada() As Task(Of ActionResult)
            Dim Item As New SearchHelper

            Dim Resultado = Item.GetListLotAvaible("2055")

            If Resultado.Estatus = "Exito" Then

                Dim l_ResultadoInventario As List(Of ActualizaStock)

                l_ResultadoInventario = Resultado.Valor

                For Each RegistrarInventario In l_ResultadoInventario

                    Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = RegistrarInventario.location).FirstOrDefaultAsync()
                    If IsNothing(ValidaUbicacion) Then
                        Continue For
                    End If

                    Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.item).FirstOrDefaultAsync()
                    If IsNothing(validaArticulo) Then
                        Continue For
                    End If

                    Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion And n.NumLote = RegistrarInventario.InventoryID).FirstOrDefaultAsync()

                    If IsNothing(validaStock) Then
                        Dim RegistrarStock As New StockDisponible
                        RegistrarStock.idProducto = validaArticulo.idProducto
                        RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                        RegistrarStock.StockDisponible1 = Convert.ToDecimal(RegistrarInventario.quantityavailable)
                        RegistrarStock.NumLote = RegistrarInventario.InventoryID
                        RegistrarStock.IssueInventory = RegistrarInventario.IssuesID

                        db.StockDisponible.Add(RegistrarStock)
                        db.SaveChanges()
                    Else
                        validaStock.StockDisponible1 = Convert.ToDecimal(RegistrarInventario.quantityavailable)

                        db.SaveChanges()

                    End If

                Next

            End If

        End Function

        <AllowAnonymous>
        Public Function GetBalanceForInvoice()
            Dim Item As New InvoiceHelper

            Dim l_invoice = (From n In db.Invoice_SO).ToList()

            For Each fac In l_invoice
                Dim MontoPagado = Item.GetBalancePaidInvoice(fac.NS_InternalID)

                fac.ImporteAdeudado = MontoPagado
                db.SaveChanges()
            Next

        End Function

        <AllowAnonymous>
        Public Function GetServiceItem()
            Try
                Dim Connection As New ConnectionServer
                Dim l_clases As New List(Of String)
                Dim l_erroresEnsamble As New List(Of String)
                Dim l_erroresInventario As New List(Of String)
                Dim Item As New ItemHelper
                Dim l_SRItem As New List(Of ServiceResaleItem)

                ''Clases validas para agregar un producto
                l_clases.Add("MUELLES COMERCIALIZACION")
                l_clases.Add("MUELLES MAF")
                l_clases.Add("REFACCIONES COMERCIAL")
                l_clases.Add("REFACCIONES MAF")

                ''************** Proceso para alta de Ensambles ************** 
                l_SRItem = Item.GetAllServicesItems()

                For Each RegistrarInventario As ServiceResaleItem In l_SRItem

                    ''Validacion de Clase
                    If Not IsNothing(RegistrarInventario.parent) Then

                        'If l_clases.Contains(RegistrarInventario.class.name) Then

                        'If Not IsNothing(RegistrarInventario.pricingMatrix) Then

                        Dim validaInventario = (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Service").FirstOrDefault()

                        If IsNothing(validaInventario) Then
                            Dim RegMercancia As New Catalogo_Productos

                            RegMercancia.Categoria = "Service"
                            RegMercancia.NS_InternalID = RegistrarInventario.internalId
                            RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                            RegMercancia.Descripcion = RegistrarInventario.displayName

                            'For Each RegistrarPrecio In RegistrarInventario.pricingMatrix.pricing
                            '    Dim Precio As New NivelesPrecioProducto

                            '    Dim ValidarNivelPrecioValido = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

                            '    If Not IsNothing(ValidarNivelPrecioValido) Then
                            '        Precio.idCategoriaPrecio = ValidarNivelPrecioValido.idCategoriaPrecio
                            '        Precio.Precio = RegistrarPrecio.priceList.First.value

                            '        If ValidarNivelPrecioValido.Descripcion = "Precio Base" Then
                            '            RegMercancia.Precio = RegistrarPrecio.priceList.First.value
                            '        End If

                            '        RegMercancia.NivelesPrecioProducto.Add(Precio)
                            '    End If
                            'Next

                            'RegMercancia.Precio = RegistrarInventario.
                            db.Catalogo_Productos.Add(RegMercancia)
                            db.SaveChanges()

                        Else
                            validaInventario.NS_ExternalID = RegistrarInventario.itemId

                            If Not IsNothing(RegistrarInventario.pricingMatrix) Then
                                Dim Precio = RegistrarInventario.pricingMatrix.pricing.First.priceList.First.value

                                validaInventario.Precio = Precio
                            End If

                            db.SaveChanges()
                        End If
                    End If

                    'End If


                    'End If
                Next

                Return "Proceso actualizado con exito!"
            Catch ex As Exception

            End Try
        End Function

        <AllowAnonymous>
        Public Function GetFacturasPruebas()
            Dim Item As New InvoiceHelper
            Dim l_Invoice As New List(Of Invoice)

            l_Invoice = Item.GetListInvoiceFromCustomerID("2572")
            'Dim r = Item.GetListInvoice()

            For Each RegistrarFactura In l_Invoice

                Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarFactura.internalId).FirstOrDefault()

                If IsNothing(ValidaFactura) Then
                    Dim RegInvoice As New Invoice_SO
                    Dim SaldoPendiente = Item.GetBalancePaidInvoice(RegistrarFactura.internalId)

                    RegInvoice.NS_InternalID = RegistrarFactura.internalId
                    RegInvoice.NS_ExternalID = RegistrarFactura.tranId
                    RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                    RegInvoice.FechaCreacion = RegistrarFactura.tranDate
                    RegInvoice.idCustomer = (From n In db.Customers Where n.NS_InternalID = RegistrarFactura.entity.internalId Select n.idCustomer).FirstOrDefault()
                    RegInvoice.Subtotal = RegistrarFactura.subTotal
                    RegInvoice.Total_Impuestos = RegistrarFactura.taxTotal
                    RegInvoice.Total = RegistrarFactura.total
                    RegInvoice.ImporteAdeudado = SaldoPendiente

                    Dim validaDatos = 0

                    If Not IsNothing(RegistrarFactura.createdFrom) Then
                        validaDatos = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
                    End If

                    If Not validaDatos = 0 Then
                        RegInvoice.idSalesOrder = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
                    End If

                    db.Invoice_SO.Add(RegInvoice)
                    db.SaveChanges()
                Else
                    Dim SaldoPendiente = Item.GetBalancePaidInvoice(ValidaFactura.NS_InternalID)
                    ValidaFactura.ImporteAdeudado = SaldoPendiente

                    If SaldoPendiente = 0.00 Then
                        ValidaFactura.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                    End If
                    db.SaveChanges()
                End If


            Next

            Dim resultado = "Exito"
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
