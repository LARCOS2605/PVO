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

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class InventarioController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public NS_Contact As New NetsuiteController
        Public H_Search As New SearchHelper()
#End Region

#Region "Vistas"
        Public Async Function ConsultarInventarioDisponible() As Task(Of ActionResult)
            'HttpContext
            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                Else

                    Dim GetCedis = Await (From n In db.Ubicaciones Where n.DescripcionAlmacen = "ALMACEN PLANTA (CEDIS)").FirstOrDefaultAsync()
                    Dim GetMuelles = Await (From n In db.Ubicaciones Where n.DescripcionAlmacen = "PLANTA MUELLES").FirstOrDefaultAsync()

                    If IsNothing(GetCedis) Then
                        Flash.Instance.Error("No se pudo localizar el almacen planta. Verifique su existencia o sincronice los datos.")
                        Return RedirectToAction("Index", "Home")
                    End If

                    If IsNothing(GetMuelles) Then
                        Flash.Instance.Error("No se pudo localizar el almacen planta muelles. Verifique su existencia o sincronice los datos.")
                        Return RedirectToAction("Index", "Home")
                    End If

                    Dim Mostrador = Await (From n In db.Ubicaciones Where n.idUbicacion = ValidaMostPredef).FirstOrDefaultAsync()

                    If IsNothing(Mostrador) Then
                        Flash.Instance.Error("No se pudo localizar el almacen. Verifique su existencia o sincronice los datos.")
                        Return RedirectToAction("Index", "Home")
                    End If

                    Dim MostradorUsuario = Mostrador.NS_InternalID.ToString
                    Dim MostradorCedis = GetCedis.NS_InternalID.ToString
                    Dim MostradorMuelles = GetMuelles.NS_InternalID.ToString
                    Dim l_MapStock As New List(Of StockArticulosViewModel)

                    ''Obtener todos los articulos
                    Dim l_productos = (From n In db.Catalogo_Productos).ToList()

                    Dim l_StockDisponible As RespuestaServicios = H_Search.GetListStockForLocation(MostradorUsuario)
                    Dim l_stock As List(Of MapListaArticulos) = l_StockDisponible.Valor

                    Dim l_MostradorUsuario = (From n In l_stock Where n.InternalID_Location = MostradorUsuario).ToList()
                    Dim l_NSM As List(Of String) = (From n In l_stock Where n.InternalID_Location = MostradorCedis Select n.InternalID).ToList()
                    Dim l_MostradorCedis = (From n In l_stock Where n.InternalID_Location = MostradorCedis).ToList()

                    ''Mostrador Muelles
                    Dim l_NSMuelles As List(Of String) = (From n In l_stock Where n.InternalID_Location = MostradorMuelles Select n.InternalID).ToList()
                    Dim l_MostradorMuelles = (From n In l_stock Where n.InternalID_Location = MostradorMuelles).ToList()

                    For Each RegProductos In l_MostradorCedis
                        Dim RegistrarDatos As New StockArticulosViewModel

                        Dim Stock_Mostrador = (From n In l_MostradorUsuario Where n.InternalID = RegProductos.InternalID).FirstOrDefault()
                        Dim Stock_Muelle = (From n In l_MostradorMuelles Where n.InternalID = RegProductos.InternalID).FirstOrDefault()
                        'Dim PrecioBase = (From n In l_Precio Where n.InternalID = RegProductos.NS_InternalID).FirstOrDefault()
                        If IsNothing(Stock_Mostrador) Then
                            RegistrarDatos.stock_Mostrador = 0
                        Else
                            RegistrarDatos.stock_Mostrador = Stock_Mostrador.StockDisponible
                        End If
                        If IsNothing(Stock_Muelle) Then
                            RegistrarDatos.stock_Muelles = 0
                        Else
                            RegistrarDatos.stock_Muelles = Stock_Muelle.StockDisponible
                        End If
                        RegistrarDatos.ClaveProducto = RegProductos.ClaveArticulo
                        RegistrarDatos.Descripcion = RegProductos.NombreArticulo
                        RegistrarDatos.stock_Cedis = RegProductos.StockDisponible

                        RegistrarDatos.precioBase = RegProductos.PrecioBase

                        l_MapStock.Add(RegistrarDatos)
                    Next

                    ''Get list values no cedis

                    Dim DiferenciaCedis = (From n In l_MostradorUsuario Where Not l_NSM.Contains(n.InternalID)).ToList()

                    For Each RegProductosMostrador In DiferenciaCedis
                        Dim RegistrarDatos As New StockArticulosViewModel

                        Dim Stock_Muelle = (From n In l_MostradorMuelles Where n.InternalID = RegProductosMostrador.InternalID).FirstOrDefault()

                        If IsNothing(Stock_Muelle) Then
                            RegistrarDatos.stock_Muelles = 0
                        Else
                            RegistrarDatos.stock_Muelles = Stock_Muelle.StockDisponible
                        End If

                        RegistrarDatos.ClaveProducto = RegProductosMostrador.ClaveArticulo
                        RegistrarDatos.Descripcion = RegProductosMostrador.NombreArticulo
                        RegistrarDatos.stock_Cedis = 0
                        RegistrarDatos.stock_Mostrador = RegProductosMostrador.StockDisponible
                        RegistrarDatos.precioBase = RegProductosMostrador.PrecioBase

                        l_MapStock.Add(RegistrarDatos)
                    Next

                    'Dim DiferenciaMuelles = (From n In l_MostradorUsuario Where Not l_NSMuelles.Contains(n.InternalID)).ToList()

                    'For Each RegProductosMuelles In DiferenciaMuelles
                    '    Dim RegistrarDatos As New StockArticulosViewModel

                    '    RegistrarDatos.ClaveProducto = RegProductosMuelles.ClaveArticulo
                    '    RegistrarDatos.Descripcion = RegProductosMuelles.NombreArticulo
                    '    RegistrarDatos.stock_Cedis = 0
                    '    RegistrarDatos.stock_Mostrador = 0
                    '    RegistrarDatos.stock_Muelles = RegProductosMuelles.StockDisponible
                    '    RegistrarDatos.precioBase = RegProductosMuelles.PrecioBase

                    '    l_MapStock.Add(RegistrarDatos)
                    'Next

                    ViewBag.Stock = l_MapStock

                    Return View()
                End If
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

            Return View()
        End Function
        Public Function GenerarPlantillaMRP() As ActionResult
            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                Else

                    ''Consultamos la existencia de StockDisponible
                    Dim l_PlantillaDatos = db.JG_ConsultarPlantillaMRP("23", ValidaMostPredef.MostradorPref)

                    'Dim detalleVentas = H

                    Return View()
                End If
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function
        Public Function ConsultarInventarioCosto() As ActionResult
            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                Else

                    ''Consultamos la existencia de StockDisponible
                    Dim l_PlantillaDatos = db.JG_ConsultarPlantillaMRP("23", ValidaMostPredef.MostradorPref)

                    'Dim detalleVentas = H

                    Return View()
                End If
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

        End Function
#End Region

#Region "Procesos"
        Public Async Function SincronizarInventarioDisponible() As Task(Of ActionResult)
            'HttpContext
            If WebSecurity.IsAuthenticated Then
                Dim ValidaMostPredef = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostPredef) Then
                    Flash.Instance.Error("Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                Else

                    Dim GetCedis = Await (From n In db.Ubicaciones Where n.DescripcionAlmacen = "ALMACEN PLANTA (CEDIS)").FirstOrDefaultAsync()
                    Dim GetMuelles = Await (From n In db.Ubicaciones Where n.DescripcionAlmacen = "PLANTA MUELLES").FirstOrDefaultAsync()

                    If IsNothing(GetCedis) Then
                        Flash.Instance.Error("No se pudo localizar el almacen planta. Verifique su existencia o sincronice los datos de ubicaciones.")
                        Return RedirectToAction("Index", "Home")
                    End If

                    Dim GetUbicacion = Await (From n In db.Ubicaciones Where n.idUbicacion = ValidaMostPredef).FirstOrDefaultAsync()

                    If IsNothing(GetUbicacion) Then
                        Flash.Instance.Error("No se pudo localizar el almacen seleccionado. Verifique su existencia o sincronice los datos de ubicaciones.")
                        Return RedirectToAction("Index", "Home")
                    End If

                    Dim Respuesta = Await NS_Contact.SincronizarStockPorUbicaciones(GetCedis.NS_InternalID, GetUbicacion.NS_InternalID)

                    'Dim l_stockViewModel As List(Of StockDisponibleViewModel) = AutoMap.AutoMapperStockDisponible(l_stock)

                    Flash.Instance.Success("El Inventario fue sincronizado con exito.")
                    Return RedirectToAction("ConsultarInventarioDisponible")

                    Return View()
                End If
            Else
                Flash.Instance.Error("Para poder usar esta opción, debe iniciar sesión.")
                Return RedirectToAction("Index", "Home")
            End If

            Return View()
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

            Dim actualizarStock As New RespuestaServicios

            actualizarStock = Helpers.GetInventoryNumber("Acapulco", "1170")
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
        Public Function exampleMemoCreditDistar()
            Dim l As New CreditMemoHelper()

            Dim dat = l.TransformInvoiceToCreditMemo("13470")


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

            'Dim a = s.DeleteSalesOrder("16197")


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

        '<AllowAnonymous>
        'Public Function Pruebasalmacenadas()
        '    Dim Item As New SearchHelper

        '    Dim r = Item.GetSearchAdvance("1398")
        '    'Dim r = Item.GetListInvoice()

        '    Dim resultado = r
        'End Function

        <AllowAnonymous>
        Public Function PruebaArticuloInventariadostock()
            Dim Item As New ItemFulFillmentHelper

            Dim r = Item.GetInventoryNumber("23", "1457")
            'Dim r = Item.GetListInvoice()

            Dim resultado = r
        End Function


        <AllowAnonymous>
        Public Function GetINVsselvlet()
            Dim Connection As New ConnectionServer

            Dim r = Connection.ConnectionServiceServletPDF("12036")

            Dim baseFile = r.Valor

            Dim bytes = System.Convert.FromBase64String(baseFile)
            Dim writer As New System.IO.BinaryWriter(IO.File.Open("C:\temp\tembase641.pdf", IO.FileMode.Create))
            writer.Write(bytes)
            writer.Close()

            Dim resultado = r
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

            'For Each RegistrarFactura In l_Invoice

            '    Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarFactura.internalId).FirstOrDefault()

            '    If IsNothing(ValidaFactura) Then
            '        Dim RegInvoice As New Invoice_SO
            '        Dim SaldoPendiente = Item.GetBalancePaidInvoice(RegistrarFactura.internalId)

            '        RegInvoice.NS_InternalID = RegistrarFactura.internalId
            '        RegInvoice.NS_ExternalID = RegistrarFactura.tranId
            '        RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
            '        RegInvoice.FechaCreacion = RegistrarFactura.tranDate
            '        RegInvoice.idCustomer = (From n In db.Customers Where n.NS_InternalID = RegistrarFactura.entity.internalId Select n.idCustomer).FirstOrDefault()
            '        RegInvoice.Subtotal = RegistrarFactura.subTotal
            '        RegInvoice.Total_Impuestos = RegistrarFactura.taxTotal
            '        RegInvoice.Total = RegistrarFactura.total
            '        RegInvoice.ImporteAdeudado = SaldoPendiente

            '        Dim validaDatos = 0

            '        If Not IsNothing(RegistrarFactura.createdFrom) Then
            '            validaDatos = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
            '        End If

            '        If Not validaDatos = 0 Then
            '            RegInvoice.idSalesOrder = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
            '        End If

            '        db.Invoice_SO.Add(RegInvoice)
            '        db.SaveChanges()
            '    Else
            '        Dim SaldoPendiente = Item.GetBalancePaidInvoice(ValidaFactura.NS_InternalID)
            '        ValidaFactura.ImporteAdeudado = SaldoPendiente

            '        If SaldoPendiente = 0.00 Then
            '            ValidaFactura.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
            '        End If
            '        db.SaveChanges()
            '    End If


            'Next

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
