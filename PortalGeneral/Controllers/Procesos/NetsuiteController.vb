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

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class NetsuiteController
        Inherits System.Web.Mvc.Controller

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_Payment As New PaymentHelper
        Public H_SalesOrder As New SalesOrderHelper
        Public H_Item As New ItemHelper
        Public H_Customer As New CustomerHelper
        Public H_Search As New SearchHelper
        Public H_Location As New DimensionHelper
#End Region

#Region "Proceso"

#Region "Sincronizar Vistas"
        Public Function SincronizarCliente(ByVal id As String) As RespuestaControlGeneral
            Dim costumerHelp As New CustomerHelper
            Dim ClienteEspecifico As New Customer

            ClienteEspecifico = H_Customer.GetCustomerByName(id)

            Try
                If IsNothing(ClienteEspecifico) Then

                    Dim validaCliente = (From n In db.Customers Where n.NS_InternalID = ClienteEspecifico.internalId).FirstOrDefault()

                    If IsNothing(validaCliente) Then
                        Dim nuevoCustomer As New Customers

                        nuevoCustomer.NS_InternalID = ClienteEspecifico.internalId
                        nuevoCustomer.NS_ExternalID = ClienteEspecifico.entityId
                        nuevoCustomer.Nombre = ClienteEspecifico.altName
                        nuevoCustomer.Dias_Atraso = Convert.ToDecimal(ClienteEspecifico.daysOverdue)

                        If Not IsNothing(ClienteEspecifico.customFieldList) Then
                            For Each GetRFC In ClienteEspecifico.customFieldList
                                If GetRFC.scriptId = "custentity_mx_rfc" Then
                                    Dim n As StringCustomFieldRef
                                    n = GetRFC
                                    nuevoCustomer.RFC = n.value
                                End If
                            Next
                        End If

                        If Not IsNothing(ClienteEspecifico.creditLimit) Then
                            Dim s = ClienteEspecifico.creditLimit

                            nuevoCustomer.Limite_Credito = s
                        End If

                        If Not IsNothing(ClienteEspecifico.terms) Then
                            Dim s = ClienteEspecifico.terms.internalId

                            nuevoCustomer.idCatalogoTerminos = (From n In db.Catalogo_Terminos Where n.NS_InternalID = s Select n.idCatalogoTerminos).FirstOrDefault()
                        End If

                        If Not IsNothing(ClienteEspecifico.priceLevel) Then
                            Dim s = ClienteEspecifico.priceLevel.internalId

                            nuevoCustomer.idCategoriaPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = s Select n.idCategoriaPrecio).FirstOrDefault()
                        End If

                        If Not IsNothing(ClienteEspecifico.entityStatus) Then
                            Dim EstatusCliente = ClienteEspecifico.entityStatus.internalId

                            nuevoCustomer.idCatalogoTipoCliente = (From n In db.Catalogo_TipoCliente Where n.NS_InternalID = EstatusCliente Select n.idCatalogoTipoCliente).FirstOrDefault()
                        End If

                        db.Customers.Add(nuevoCustomer)
                        db.SaveChanges()
                    Else

                        validaCliente.NS_ExternalID = ClienteEspecifico.entityId
                        validaCliente.Nombre = ClienteEspecifico.altName
                        validaCliente.Dias_Atraso = Convert.ToDecimal(ClienteEspecifico.daysOverdue)

                        If Not IsNothing(ClienteEspecifico.customFieldList) Then
                            For Each GetRFC In ClienteEspecifico.customFieldList
                                If GetRFC.scriptId = "custentity_mx_rfc" Then
                                    Dim n As StringCustomFieldRef
                                    n = GetRFC
                                    validaCliente.RFC = n.value
                                End If
                            Next
                        End If

                        If Not IsNothing(ClienteEspecifico.priceLevel) Then
                            Dim s = ClienteEspecifico.priceLevel.internalId

                            validaCliente.idCategoriaPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = s Select n.idCategoriaPrecio).FirstOrDefault()
                        End If

                        If Not IsNothing(ClienteEspecifico.entityStatus) Then
                            Dim EstatusCliente = ClienteEspecifico.entityStatus.internalId

                            validaCliente.idCatalogoTipoCliente = (From n In db.Catalogo_TipoCliente Where n.NS_InternalID = EstatusCliente Select n.idCatalogoTipoCliente).FirstOrDefault()
                        End If

                        If Not IsNothing(ClienteEspecifico.creditLimit) Then
                            Dim s = ClienteEspecifico.creditLimit

                            validaCliente.Limite_Credito = s
                        End If

                        If Not IsNothing(ClienteEspecifico.terms) Then
                            Dim s = ClienteEspecifico.terms.internalId

                            validaCliente.idCatalogoTerminos = (From n In db.Catalogo_Terminos Where n.NS_InternalID = s Select n.idCatalogoTerminos).FirstOrDefault()
                        End If

                        db.SaveChanges()
                    End If

                    Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "")
                End If

            Catch ex As Exception
                Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message)
            End Try

        End Function
#End Region

#Region "Sincronizacion General Operacion"
        <AllowAnonymous>
        Public Function SincronizarDatos() As String
            Dim NivelPrecio = SincronizarNivelPrecio()
            Dim ProductosVentas = SincronizarProductosVenta()
            Dim PreciosVenta = SincronizarPrecioProductos()
            Dim StockAlmacenes = SincronizarStockAlmacenes()
            Dim Clientes = SincronizarClientesExistentes()
            Dim FacturasClientes = SincronizarFacturasPorClienteJob()
            'Dim x = SincronizarMetodosPago()
        End Function

#End Region

#Region "Metodos Ejecucion"
        Public Async Function SincronizarStockAlmacenes() As Task(Of String)

#Region "Stock Lotes Ensamblados"

            Dim Busqueda_NS = (From n In db.Parametros Where n.Clave = "BG_NumInventarioLoc").FirstOrDefault()
            Dim Resultado = H_Search.GetListLotAvaible(Busqueda_NS.Valor)

            Dim lmostradores As New List(Of String)
            'lmostradores.Add("21")
            'lmostradores.Add("19")
            If Resultado.Estatus = "Exito" Then

                Dim l_ResultadoInventario As List(Of ActualizaStock)

                l_ResultadoInventario = Resultado.Valor

                'l_ResultadoInventario = (From n In l_ResultadoInventario Where lmostradores.Contains(n.location)).ToList()

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
                        Await db.SaveChangesAsync()
                    Else

                        If validaStock.StockDisponible1 <> Convert.ToDecimal(RegistrarInventario.quantityavailable) Then
                            validaStock.StockDisponible1 = Convert.ToDecimal(RegistrarInventario.quantityavailable)
                            Await db.SaveChangesAsync()
                        End If


                    End If

                Next

            End If

#End Region

#Region "Stock Articulos Inventariados"
            Dim Busqueda_NS_inv = (From n In db.Parametros Where n.Clave = "BG_StockInventario").FirstOrDefault()

            Dim StockInventariados = H_Search.GetInventoryItemStock(Busqueda_NS_inv.Valor)

            Dim l_precios As List(Of Sinc_StockInventarios) = StockInventariados.Valor

            'Dim lprecios = (From n In l_precios Where lmostradores.Contains(n.s_locationinternalId)).ToList()

            For Each SincronizarArticulos As Sinc_StockInventarios In l_precios

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    Await db.SaveChangesAsync()
                Else

                    If validaStock.StockDisponible1 <> Convert.ToDecimal(SincronizarArticulos.s_StockDisponible) Then
                        validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)
                        Await db.SaveChangesAsync()
                    End If

                End If
            Next
#End Region

#Region "Stock Articulos Ensamble sin Lote"

            Dim Busqueda_NS_ensamblessinlote = (From n In db.Parametros Where n.Clave = "BG_Stockensamble").FirstOrDefault()

            Dim s = H_Search.GetInventoryItemStock(Busqueda_NS_ensamblessinlote.Valor)

            Dim l_precios_s As List(Of Sinc_StockInventarios) = s.Valor

            'Dim lprecios_s = (From n In l_precios_s Where lmostradores.Contains(n.s_locationinternalId)).ToList()

            For Each SincronizarArticulos As Sinc_StockInventarios In l_precios_s

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    Await db.SaveChangesAsync()
                Else

                    If validaStock.StockDisponible1 <> Convert.ToDecimal(SincronizarArticulos.s_StockDisponible) Then
                        validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)
                        Await db.SaveChangesAsync()
                    End If


                End If
            Next
#End Region

            Return "Proceso actualizado con exito!"
        End Function

        Public Function SincronizarClientesExistentes()
            Dim costumerHelp As New CustomerHelper
            Dim l_costumer As New List(Of Customer)

            l_costumer = H_Customer.GetAllCustomers()
            'l_costumer = (From n In l_costumer Where n.internalId = "2430").ToList()

            For Each RegistrarCustomer In l_costumer

                Try

                    Dim validaCliente = (From n In db.Customers Where n.NS_InternalID = RegistrarCustomer.internalId).FirstOrDefault()

                    If IsNothing(validaCliente) Then
                        Dim nuevoCustomer As New Customers

                        nuevoCustomer.NS_InternalID = RegistrarCustomer.internalId
                        nuevoCustomer.NS_ExternalID = RegistrarCustomer.entityId
                        nuevoCustomer.Nombre = RegistrarCustomer.altName
                        nuevoCustomer.Dias_Atraso = Convert.ToDecimal(RegistrarCustomer.daysOverdue)

                        If Not IsNothing(RegistrarCustomer.customFieldList) Then
                            For Each GetRFC In RegistrarCustomer.customFieldList
                                If GetRFC.scriptId = "custentity_mx_rfc" Then
                                    Dim n As StringCustomFieldRef
                                    n = GetRFC
                                    nuevoCustomer.RFC = n.value
                                End If
                            Next
                        End If

                        If Not IsNothing(RegistrarCustomer.creditLimit) Then
                            Dim s = RegistrarCustomer.creditLimit

                            nuevoCustomer.Limite_Credito = s
                        End If

                        If Not IsNothing(RegistrarCustomer.terms) Then
                            Dim s = RegistrarCustomer.terms.internalId

                            nuevoCustomer.idCatalogoTerminos = (From n In db.Catalogo_Terminos Where n.NS_InternalID = s Select n.idCatalogoTerminos).FirstOrDefault()
                        End If

                        If Not IsNothing(RegistrarCustomer.priceLevel) Then
                            Dim s = RegistrarCustomer.priceLevel.internalId

                            nuevoCustomer.idCategoriaPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = s Select n.idCategoriaPrecio).FirstOrDefault()
                        End If

                        If Not IsNothing(RegistrarCustomer.entityStatus) Then
                            Dim EstatusCliente = RegistrarCustomer.entityStatus.internalId

                            nuevoCustomer.idCatalogoTipoCliente = (From n In db.Catalogo_TipoCliente Where n.NS_InternalID = EstatusCliente Select n.idCatalogoTipoCliente).FirstOrDefault()
                        End If

                        db.Customers.Add(nuevoCustomer)
                        db.SaveChanges()
                    Else

                        validaCliente.NS_ExternalID = RegistrarCustomer.entityId
                        validaCliente.Nombre = RegistrarCustomer.altName
                        validaCliente.Dias_Atraso = Convert.ToDecimal(RegistrarCustomer.daysOverdue)

                        If Not IsNothing(RegistrarCustomer.customFieldList) Then
                            For Each GetRFC In RegistrarCustomer.customFieldList
                                If GetRFC.scriptId = "custentity_mx_rfc" Then
                                    Dim n As StringCustomFieldRef
                                    n = GetRFC
                                    validaCliente.RFC = n.value
                                End If
                            Next
                        End If

                        If Not IsNothing(RegistrarCustomer.priceLevel) Then
                            Dim s = RegistrarCustomer.priceLevel.internalId

                            validaCliente.idCategoriaPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = s Select n.idCategoriaPrecio).FirstOrDefault()
                        End If

                        If Not IsNothing(RegistrarCustomer.entityStatus) Then
                            Dim EstatusCliente = RegistrarCustomer.entityStatus.internalId

                            validaCliente.idCatalogoTipoCliente = (From n In db.Catalogo_TipoCliente Where n.NS_InternalID = EstatusCliente Select n.idCatalogoTipoCliente).FirstOrDefault()
                        End If

                        If Not IsNothing(RegistrarCustomer.creditLimit) Then
                            Dim s = RegistrarCustomer.creditLimit

                            validaCliente.Limite_Credito = s
                        End If

                        If Not IsNothing(RegistrarCustomer.terms) Then
                            Dim s = RegistrarCustomer.terms.internalId

                            validaCliente.idCatalogoTerminos = (From n In db.Catalogo_Terminos Where n.NS_InternalID = s Select n.idCatalogoTerminos).FirstOrDefault()
                        End If

                        db.SaveChanges()
                    End If
                Catch ex As Exception
                    Continue For
                End Try

            Next

            Return "Proceso actualizado con exito!"
        End Function

        Public Async Function SincronizarProductosVenta() As Task(Of String)
            Try
                Dim Connection As New ConnectionServer
                Dim l_clases As New List(Of String)
                Dim l_erroresEnsamble As New List(Of String)
                Dim l_erroresInventario As New List(Of String)

                ''Clases validas para agregar un producto
                l_clases.Add("MUELLES COMERCIALIZACION")
                l_clases.Add("MUELLES MAF")
                l_clases.Add("REFACCIONES COMERCIAL")
                l_clases.Add("REFACCIONES MAF")

                ''************** Proceso para alta de Servicios ************** 
                Dim l_SRItem = H_Item.GetAllServicesItems()

                For Each RegistrarInventario As ServiceResaleItem In l_SRItem
                    Try
                        If Not IsNothing(RegistrarInventario.parent) Then

                            'If Not IsNothing(RegistrarInventario.class) Then

                            Dim validaInventario = (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Service").FirstOrDefault()

                            If IsNothing(validaInventario) Then
                                Dim RegMercancia As New Catalogo_Productos

                                RegMercancia.Categoria = "Service"
                                RegMercancia.NS_InternalID = RegistrarInventario.internalId
                                RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                                RegMercancia.Descripcion = RegistrarInventario.displayName

                                db.Catalogo_Productos.Add(RegMercancia)
                                Await db.SaveChangesAsync()
                            End If
                            'End If

                        End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

                ''************** Proceso para alta de Ensambles ************** 
                Dim l_AssemblyItem As List(Of LotNumberedAssemblyItem) = H_Item.GetAllItemsLotNumberedAssemblyItem()

                For Each RegistrarInventario As LotNumberedAssemblyItem In l_AssemblyItem
                    Try
                        'If Not IsNothing(RegistrarInventario.class) Then

                        'If l_clases.Contains(RegistrarInventario.class.name) Then

                        Dim validaInventario = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Assembly").FirstOrDefaultAsync()

                                If IsNothing(validaInventario) Then
                                    Dim RegMercancia As New Catalogo_Productos

                                    RegMercancia.Categoria = "Assembly"
                                    RegMercancia.NS_InternalID = RegistrarInventario.internalId
                                    RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                                    RegMercancia.Descripcion = RegistrarInventario.displayName

                                    db.Catalogo_Productos.Add(RegMercancia)
                                    Await db.SaveChangesAsync()
                                End If
                        'End If

                        'End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

                ''************** Proceso para alta de Inventario ************** 
                Dim l_InventoryItem As List(Of InventoryItem) = H_Item.GetAllInventoryItems()

                For Each RegistrarInventario As InventoryItem In l_InventoryItem
                    Try
                        ''Validacion de Clase
                        'If Not IsNothing(RegistrarInventario.class) Then

                        '    If l_clases.Contains(RegistrarInventario.class.name) Then

                        Dim validaInventario = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Inventory").FirstOrDefaultAsync()

                                If IsNothing(validaInventario) Then
                                    Dim RegMercancia As New Catalogo_Productos

                                    RegMercancia.Categoria = "Inventory"
                                    RegMercancia.NS_InternalID = RegistrarInventario.internalId
                                    RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                                    RegMercancia.Descripcion = RegistrarInventario.displayName

                                    db.Catalogo_Productos.Add(RegMercancia)
                                    Await db.SaveChangesAsync()
                                End If

                        '    End If

                        'End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

                ''************** Proceso para alta de Ensambles sin Lote ************** 
                Dim l_AssemblynlItem As List(Of AssemblyItem) = H_Item.GetAllItemsAssemblyItem()

                For Each RegistrarAssemblynl As AssemblyItem In l_AssemblynlItem
                    Try
                        'If Not IsNothing(RegistrarInventario.class) Then

                        '    If l_clases.Contains(RegistrarInventario.class.name) Then

                        If RegistrarAssemblynl.internalId = "23287" Then
                            Dim n = ""
                        End If

                        Dim validaInventario = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarAssemblynl.internalId And n.Categoria = "Assembly_nL").FirstOrDefaultAsync()

                        If IsNothing(validaInventario) Then
                            Dim RegMercancia As New Catalogo_Productos

                            RegMercancia.Categoria = "Assembly_nL"
                            RegMercancia.NS_InternalID = RegistrarAssemblynl.internalId
                            RegMercancia.NS_ExternalID = RegistrarAssemblynl.itemId
                            RegMercancia.Descripcion = RegistrarAssemblynl.displayName

                            db.Catalogo_Productos.Add(RegMercancia)
                            Await db.SaveChangesAsync()
                        End If
                        '    End If

                        'End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next



                Return "Proceso actualizado con exito!"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Public Async Function SincronizarPrecioProductos() As Task(Of String)

            ''************** Proceso para alta de Precios articulos inventariados **************
            Dim RegistrosTranscurridos = 0
            Dim RegistrosValidos = 0
            Dim datos_inventariados = H_Search.GetListInventoryItemsSearchAdvanced()
            Dim RespValues_inventariados As Sinc_JobItems = datos_inventariados.Valor
            Dim l_Precios_inventariados As List(Of Sinc_PricingItems) = RespValues_inventariados.l_Pricing

            For Each RegPrecios_inv As Sinc_PricingItems In l_Precios_inventariados

                RegistrosTranscurridos = RegistrosTranscurridos + 1
                Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios_inv.s_internalIdpricing).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaNivel) Then
                    Continue For
                End If

                Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios_inv.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaarticulo) Then
                    Continue For
                End If

                Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

                If IsNothing(ValidarRegistroNivelPrecio) Then

                    Dim RegistrarPrecio As New NivelesPrecioProducto
                    RegistrarPrecio.idProducto = ValidaExistenciaarticulo.idProducto
                    RegistrarPrecio.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio
                    RegistrarPrecio.Precio = Convert.ToDecimal(RegPrecios_inv.s_value)

                    db.NivelesPrecioProducto.Add(RegistrarPrecio)
                    RegistrosValidos += Await db.SaveChangesAsync()

                Else
                    If ValidaExistenciaarticulo.Precio <> Convert.ToDecimal(RegPrecios_inv.s_value) Then
                        ValidarRegistroNivelPrecio.Precio = Convert.ToDecimal(RegPrecios_inv.s_value)
                        RegistrosValidos += Await db.SaveChangesAsync()
                    End If

                End If

            Next

            ''************** Proceso para alta de Precios articulos Servicio **************
            Dim datos_Servicios = H_Search.GetListServicesItemsSearchAdvanced()
            Dim RespValues_servicios As Sinc_JobItems = datos_Servicios.Valor
            Dim l_Precios_servicios As List(Of Sinc_PricingItems) = RespValues_servicios.l_Pricing
            For Each RegPrecios_serv As Sinc_PricingItems In l_Precios_servicios

                Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios_serv.s_internalIdpricing).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaNivel) Then
                    Continue For
                End If

                Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios_serv.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaarticulo) Then
                    Continue For
                End If

                Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

                If IsNothing(ValidarRegistroNivelPrecio) Then

                    Dim RegistrarPrecio As New NivelesPrecioProducto
                    RegistrarPrecio.idProducto = ValidaExistenciaarticulo.idProducto
                    RegistrarPrecio.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio
                    RegistrarPrecio.Precio = Convert.ToDecimal(RegPrecios_serv.s_value)

                    db.NivelesPrecioProducto.Add(RegistrarPrecio)
                    Await db.SaveChangesAsync()

                Else
                    ValidarRegistroNivelPrecio.Precio = Convert.ToDecimal(RegPrecios_serv.s_value)
                    db.SaveChanges()
                End If

            Next

            ''************** Proceso para alta de Precios Ensambles con lote **************
            Dim dat = H_Search.GetListAssemblyItemsSearchAdvanced()
            Dim RespValues As Sinc_JobItems = dat.Valor
            Dim l_Precios As List(Of Sinc_PricingItems) = RespValues.l_Pricing
            For Each RegPrecios In l_Precios

                Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios.s_internalIdpricing).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaNivel) Then
                    Continue For
                End If

                Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaarticulo) Then
                    Continue For
                End If

                Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

                If IsNothing(ValidarRegistroNivelPrecio) Then

                    Dim RegistrarPrecio As New NivelesPrecioProducto
                    RegistrarPrecio.idProducto = ValidaExistenciaarticulo.idProducto
                    RegistrarPrecio.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio
                    RegistrarPrecio.Precio = Convert.ToDecimal(RegPrecios.s_value)

                    db.NivelesPrecioProducto.Add(RegistrarPrecio)
                    Await db.SaveChangesAsync()

                Else
                    If ValidarRegistroNivelPrecio.Precio <> Convert.ToDecimal(RegPrecios.s_value) Then
                        ValidarRegistroNivelPrecio.Precio = Convert.ToDecimal(RegPrecios.s_value)
                        Await db.SaveChangesAsync()
                    End If
                End If

            Next

            ''************** Proceso para alta de Precios Ensambles sin lote **************
            Dim datos_NotLot = H_Search.GetListNotLotAssemblyItemsSearchAdvanced()
            Dim RespValues_NotLotAssembly As Sinc_JobItems = datos_NotLot.Valor
            Dim l_Precios_notLot As List(Of Sinc_PricingItems) = RespValues_NotLotAssembly.l_Pricing
            For Each RegPrecios_notlot As Sinc_PricingItems In l_Precios_notLot

                Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios_notlot.s_internalIdpricing).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaNivel) Then
                    Continue For
                End If

                Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios_notlot.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaExistenciaarticulo) Then
                    Continue For
                End If

                Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

                If IsNothing(ValidarRegistroNivelPrecio) Then

                    Dim RegistrarPrecio As New NivelesPrecioProducto
                    RegistrarPrecio.idProducto = ValidaExistenciaarticulo.idProducto
                    RegistrarPrecio.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio
                    RegistrarPrecio.Precio = Convert.ToDecimal(RegPrecios_notlot.s_value)

                    db.NivelesPrecioProducto.Add(RegistrarPrecio)
                    Await db.SaveChangesAsync()

                Else
                    If ValidarRegistroNivelPrecio.Precio <> Convert.ToDecimal(RegPrecios_notlot.s_value) Then
                        ValidarRegistroNivelPrecio.Precio = Convert.ToDecimal(RegPrecios_notlot.s_value)
                        Await db.SaveChangesAsync()
                    End If
                End If

            Next

            Return "Job ejecutado exito"
        End Function

        <AllowAnonymous>
        Public Async Function SincronizarNivelPrecio() As Task(Of String)

            Dim Resultado = H_Search.GetLevelPricing("2650")

            If Resultado.Estatus = "Exito" Then

                Dim l_ResultadoNivelPrecio As List(Of Sinc_NivelPrecio)

                l_ResultadoNivelPrecio = Resultado.Valor

                For Each RegistroNivelPrecio In l_ResultadoNivelPrecio
                    Try
                        Dim ValidaExistente = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistroNivelPrecio.s_internalId).FirstOrDefaultAsync()

                        If IsNothing(ValidaExistente) Then
                            Dim RegistrarNivelPrecio As New Catalogo_NivelesPrecio
                            RegistrarNivelPrecio.NS_InternalID = RegistroNivelPrecio.s_internalId
                            RegistrarNivelPrecio.Descripcion = RegistroNivelPrecio.s_descripcion

                            db.Catalogo_NivelesPrecio.Add(RegistrarNivelPrecio)
                            Await db.SaveChangesAsync()
                        Else
                            ValidaExistente.Descripcion = RegistroNivelPrecio.s_descripcion
                            Await db.SaveChangesAsync()
                        End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

            End If

            Return "Metodo ejecutado con exito"
        End Function

        <AllowAnonymous>
        Public Function SincronizarFacturasPorClienteJob()

            Dim l_Clientes As New List(Of Customers)
            Dim Item As New InvoiceHelper
            Dim l_Invoice As New List(Of Invoice)

            l_Clientes = (From n In db.Customers).ToList()

            For Each Cliente In l_Clientes

                l_Invoice = Item.GetListInvoiceFromCustomerID(Cliente.NS_InternalID)

                For Each RegistrarFactura In l_Invoice

                    Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarFactura.internalId).FirstOrDefault()

                    If IsNothing(ValidaFactura) Then
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
                        End If

                        db.Invoice_SO.Add(RegInvoice)
                        db.SaveChanges()
                    Else
                        'Dim SaldoPendiente = Item.GetBalancePaidInvoice(ValidaFactura.NS_InternalID)
                        'ValidaFactura.ImporteAdeudado = SaldoPendiente

                        'If SaldoPendiente = 0.00 Then
                        '    ValidaFactura.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                        'End If
                        'db.SaveChanges()
                    End If


                Next
            Next


            Dim resultado = "Exito"
        End Function

        <AllowAnonymous>
        Public Function SincronizarSalesOrderPorClienteJob()

            Dim l_Clientes As New List(Of Customers)
            Dim Item As New InvoiceHelper
            Dim l_SalesOrder As New List(Of NetsuiteLibrary.SuiteTalk.SalesOrder)

            l_Clientes = (From n In db.Customers).ToList()

            For Each Cliente In l_Clientes

                l_SalesOrder = Item.GetListSalesOrderFromCustomerID(Cliente.NS_InternalID)

                For Each RegistrarSalesOrder In l_SalesOrder

                    Dim detalleOrden As New List(Of DetalleSalesOrder)
                    Dim ValidaFactura = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarSalesOrder.internalId).FirstOrDefault()

                    If IsNothing(ValidaFactura) Then

                        If Not IsNothing(RegistrarSalesOrder.itemList) Then
                            Dim RegSO As New SalesOrder

                            RegSO.SubTotal = RegistrarSalesOrder.subTotal
                            RegSO.Total = RegistrarSalesOrder.total
                            RegSO.TotalImpuestos = RegistrarSalesOrder.taxTotal
                            RegSO.NS_ExternalID = RegistrarSalesOrder.tranId
                            RegSO.idCustomer = (From n In db.Customers Where n.NS_InternalID = RegistrarSalesOrder.entity.internalId Select n.idCustomer).FirstOrDefault()
                            RegSO.idUbicacion = (From n In db.Ubicaciones Where n.NS_InternalID = RegistrarSalesOrder.location.internalId Select n.idUbicacion).FirstOrDefault()
                            RegSO.Nota = RegistrarSalesOrder.memo
                            RegSO.NS_InternalID = RegistrarSalesOrder.internalId
                            RegSO.FechaCreacion = Date.Now

                            If RegistrarSalesOrder.status = "Facturado" Then
                                RegSO.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "SO_Facturada" Select n.idEstatus).FirstOrDefault()

                            ElseIf RegistrarSalesOrder.status = "Facturación pendiente" Then


                            ElseIf RegistrarSalesOrder.status = "Ejecución de la orden pendiente" Then
                                RegSO.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "SO_Creada" Select n.idEstatus).FirstOrDefault()

                            End If

                            For Each detalle As SalesOrderItem In RegistrarSalesOrder.itemList.item
                                Dim RegDetalleOrden As New DetalleSalesOrder

                                RegDetalleOrden.idProducto = (From n In db.Catalogo_Productos Where detalle.item.internalId Select n.idProducto).FirstOrDefault()
                                RegDetalleOrden.Cantidad = detalle.quantity
                                RegDetalleOrden.UbicacionAlmacen = (From n In db.Ubicaciones Where n.NS_InternalID = RegistrarSalesOrder.location.internalId Select n.idUbicacion).FirstOrDefault()
                                RegDetalleOrden.Importe = detalle.rate
                                RegDetalleOrden.Total = detalle.amount
                                RegDetalleOrden.CantidadEntregada = detalle.quantityFulfilled
                                detalleOrden.Add(RegDetalleOrden)
                            Next

                            RegSO.DetalleSalesOrder = detalleOrden
                            db.SalesOrder.Add(RegSO)
                            db.SaveChanges()
                        End If

                    Else
                        'Dim SaldoPendiente = Item.GetBalancePaidInvoice(ValidaFactura.NS_InternalID)
                        'ValidaFactura.ImporteAdeudado = SaldoPendiente

                        'If SaldoPendiente = 0.00 Then
                        '    ValidaFactura.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                        'End If
                        'db.SaveChanges()
                    End If


                Next
            Next


            Dim resultado = "Exito"
        End Function
#End Region

#Region "Metodos Individuales"
        Public Async Function SincronizarStockAlmacenesINV() As Task(Of String)
            Dim l As New SearchHelper()

            Dim Busqueda_NS = (From n In db.Parametros Where n.Clave = "BG_StockInventario").FirstOrDefault()

            Dim s = l.GetInventoryItemStock(Busqueda_NS.Valor)

            For Each SincronizarArticulos As Sinc_StockInventarios In s.Valor

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If SincronizarArticulos.s_iteminternalId = "2740" Then
                    Dim gl = ""
                End If

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    db.SaveChanges()
                Else
                    validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.SaveChanges()

                End If
            Next

            Return "Proceso actualizado con exito!"
        End Function

        Public Async Function SincronizarStockAlmacenesAssembly() As Task(Of String)
            Dim l As New SearchHelper()

            'Dim dat = l.GetListPaymentForLocationAdvancedVer("4")
            Dim Busqueda_NS = (From n In db.Parametros Where n.Clave = "BG_Stockensamble").FirstOrDefault()

            Dim s = l.GetInventoryItemStock(Busqueda_NS.Valor)

            For Each SincronizarArticulos As Sinc_StockInventarios In s.Valor

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    db.SaveChanges()
                Else
                    validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.SaveChanges()

                End If
            Next

            Return "Proceso actualizado con exito!"
        End Function


#End Region

#Region "Metodos Sync"
        Public Async Function SincronizarStockPorUbicaciones(ByVal NS_ID_Cedis As String, ByVal NS_ID_Ubicacion As String) As Task(Of String)

            Dim l_Mostradores As New List(Of String)
            l_Mostradores.Add(NS_ID_Cedis)
            l_Mostradores.Add(NS_ID_Ubicacion)

#Region "Stock Lotes Ensamblados"

            Dim Busqueda_NS = (From n In db.Parametros Where n.Clave = "BG_NumInventarioLoc").FirstOrDefault()
            Dim Resultado = H_Search.GetListLotAvaible(Busqueda_NS.Valor)

            If Resultado.Estatus = "Exito" Then

                Dim l_ResultadoInventario As List(Of ActualizaStock) = Resultado.Valor
                l_ResultadoInventario = (From n In l_ResultadoInventario Where l_Mostradores.Contains(n.location)).ToList()
                l_ResultadoInventario = (From n In l_ResultadoInventario Where n.item = "1106").ToList()
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
                        Await db.SaveChangesAsync()
                    Else

                        If validaStock.StockDisponible1 <> Convert.ToDecimal(RegistrarInventario.quantityavailable) Then
                            validaStock.StockDisponible1 = Convert.ToDecimal(RegistrarInventario.quantityavailable)
                            Await db.SaveChangesAsync()
                        End If


                    End If

                Next

            End If

#End Region

#Region "Stock Articulos Inventariados"
            Dim Busqueda_NS_inv = (From n In db.Parametros Where n.Clave = "BG_StockInventario").FirstOrDefault()
            Dim StockInventariados = H_Search.GetInventoryItemStock(Busqueda_NS_inv.Valor)

            Dim l_precios As List(Of Sinc_StockInventarios) = StockInventariados.Valor
            l_precios = (From n In l_precios Where l_Mostradores.Contains(n.s_locationinternalId)).ToList()
            l_precios = (From n In l_precios Where n.s_iteminternalId = "1106").ToList()
            For Each SincronizarArticulos As Sinc_StockInventarios In l_precios

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    Await db.SaveChangesAsync()
                Else

                    If validaStock.StockDisponible1 <> Convert.ToDecimal(SincronizarArticulos.s_StockDisponible) Then
                        validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)
                        Await db.SaveChangesAsync()
                    End If

                End If
            Next
#End Region

#Region "Stock Articulos Ensamble sin Lote"

            Dim Busqueda_NS_ensamblessinlote = (From n In db.Parametros Where n.Clave = "BG_Stockensamble").FirstOrDefault()
            Dim s = H_Search.GetInventoryItemStock(Busqueda_NS_ensamblessinlote.Valor)

            Dim l_precios_s As List(Of Sinc_StockInventarios) = s.Valor
            l_precios_s = (From n In l_precios_s Where l_Mostradores.Contains(n.s_locationinternalId)).ToList()
            'l_precios_s = (From n In l_precios_s Where n.s_iteminternalId = "1106").ToList()
            For Each SincronizarArticulos As Sinc_StockInventarios In l_precios_s

                Dim ValidaUbicacion = Await (From n In db.Ubicaciones Where n.NS_InternalID = SincronizarArticulos.s_locationinternalId).FirstOrDefaultAsync()
                If IsNothing(ValidaUbicacion) Then
                    Continue For
                End If

                Dim validaArticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = SincronizarArticulos.s_iteminternalId).FirstOrDefaultAsync()
                If IsNothing(validaArticulo) Then
                    Continue For
                End If

                Dim validaStock = Await (From n In db.StockDisponible Where n.idProducto = validaArticulo.idProducto And n.idUbicacion = ValidaUbicacion.idUbicacion).FirstOrDefaultAsync()

                If IsNothing(validaStock) Then
                    Dim RegistrarStock As New StockDisponible
                    RegistrarStock.idProducto = validaArticulo.idProducto
                    RegistrarStock.idUbicacion = ValidaUbicacion.idUbicacion
                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)

                    db.StockDisponible.Add(RegistrarStock)
                    Await db.SaveChangesAsync()
                Else

                    If validaStock.StockDisponible1 <> Convert.ToDecimal(SincronizarArticulos.s_StockDisponible) Then
                        validaStock.StockDisponible1 = Convert.ToDecimal(SincronizarArticulos.s_StockDisponible)
                        Await db.SaveChangesAsync()
                    End If


                End If
            Next
#End Region

            Return "True"
        End Function
#End Region

        Public Function SincronizarMetodosPago()

            Dim l_metodosPago = H_Payment.GetAllPaymenthMethod()

            For Each RegistrarMetodoPago As RecordRef In l_metodosPago

                Try

                    Dim validaMetodoPago = (From n In db.Catalogo_MetodosPago Where n.NS_InternalID = RegistrarMetodoPago.internalId).FirstOrDefault()

                    If IsNothing(validaMetodoPago) Then

                        Dim nuevoMetodoPago As New Catalogo_MetodosPago

                        nuevoMetodoPago.NS_InternalID = RegistrarMetodoPago.internalId
                        nuevoMetodoPago.Descripcion = RegistrarMetodoPago.name
                        nuevoMetodoPago.Activo = True

                        db.Catalogo_MetodosPago.Add(nuevoMetodoPago)
                        db.SaveChanges()
                    Else

                        validaMetodoPago.NS_ExternalID = RegistrarMetodoPago.internalId
                        validaMetodoPago.Descripcion = RegistrarMetodoPago.name

                        db.SaveChanges()
                    End If
                Catch ex As Exception
                    Continue For
                End Try

            Next

            Return "Proceso actualizado con exito!"
        End Function

        Public Async Function SincronizarPreciosService() As Task(Of String)

            Dim dat = H_Search.GetListServiceItemsSearchAdvanced()
            Dim RespValues As Sinc_JobItems = dat.Valor
            Dim l_items As List(Of Sinc_AssemblyItems) = RespValues.l_Items
            Dim l_Precios As List(Of Sinc_PricingItems) = RespValues.l_Pricing

            Dim GetItems = l_items.GroupBy(Function(i) i.s_internalId).[Select](Function(g) g.First()).ToList()

            For Each RegistrarProductos In GetItems

                Dim ActualizaPrecios = (From n In l_Precios Where n.s_iteminternalId = RegistrarProductos.s_internalId).ToList()

                For Each RegPrecios In ActualizaPrecios

                    Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios.s_internalIdpricing).FirstOrDefaultAsync()
                    If IsNothing(ValidaExistenciaNivel) Then
                        Continue For
                    End If

                    Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios.s_iteminternalId).FirstOrDefaultAsync()
                    If IsNothing(ValidaExistenciaarticulo) Then
                        Continue For
                    End If

                    Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

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

        Public Async Function SincronizarPreciosArticulosInventariados() As Task(Of String)

            Dim dat = H_Search.GetListInventoryItemsSearchAdvanced()
            Dim RespValues As Sinc_JobItems = dat.Valor
            Dim l_items As List(Of Sinc_AssemblyItems) = RespValues.l_Items
            Dim l_Precios As List(Of Sinc_PricingItems) = RespValues.l_Pricing

            Dim GetItems = l_items.GroupBy(Function(i) i.s_internalId).[Select](Function(g) g.First()).ToList()

            For Each RegistrarProductos In GetItems

                Dim ActualizaPrecios = (From n In l_Precios Where n.s_iteminternalId = RegistrarProductos.s_internalId).ToList()

                For Each RegPrecios In ActualizaPrecios

                    Dim ValidaExistenciaNivel = Await (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegPrecios.s_internalIdpricing).FirstOrDefaultAsync()
                    If IsNothing(ValidaExistenciaNivel) Then
                        Continue For
                    End If

                    Dim ValidaExistenciaarticulo = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegPrecios.s_iteminternalId).FirstOrDefaultAsync()
                    If IsNothing(ValidaExistenciaarticulo) Then
                        Continue For
                    End If

                    Dim ValidarRegistroNivelPrecio = Await (From n In db.NivelesPrecioProducto Where n.idProducto = ValidaExistenciaarticulo.idProducto And n.idCategoriaPrecio = ValidaExistenciaNivel.idCategoriaPrecio).FirstOrDefaultAsync()

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



        Public Async Function SincronizarProductosVentaEnsamblesSinLote() As Task(Of String)
            Try
                Dim Connection As New ConnectionServer
                Dim l_clases As New List(Of String)
                Dim l_erroresEnsamble As New List(Of String)
                Dim l_erroresInventario As New List(Of String)

                ''Clases validas para agregar un producto
                l_clases.Add("MUELLES COMERCIALIZACION")
                l_clases.Add("MUELLES MAF")
                l_clases.Add("REFACCIONES COMERCIAL")
                l_clases.Add("REFACCIONES MAF")

                ''************** Proceso para alta de Ensambles ************** 
                Dim l_AssemblyItem As List(Of AssemblyItem) = H_Item.GetAllItemsAssemblyItem()

                For Each RegistrarInventario As AssemblyItem In l_AssemblyItem
                    Try
                        'If Not IsNothing(RegistrarInventario.class) Then

                        'If l_clases.Contains(RegistrarInventario.class.name) Then

                        Dim validaInventario = Await (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Assembly_nL").FirstOrDefaultAsync()

                                If IsNothing(validaInventario) Then
                                    Dim RegMercancia As New Catalogo_Productos

                                    RegMercancia.Categoria = "Assembly_nL"
                                    RegMercancia.NS_InternalID = RegistrarInventario.internalId
                                    RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                                    RegMercancia.Descripcion = RegistrarInventario.displayName

                                    db.Catalogo_Productos.Add(RegMercancia)
                                    db.SaveChanges()
                                End If
                        '    End If

                        'End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

                Return "Proceso actualizado con exito!"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Public Async Function SincronizarCuentasBancarias() As Task(Of String)
            Try

                Dim Busqueda_NS = (From n In db.Parametros Where n.Clave = "BG_CuentasBanc").FirstOrDefault()

                Dim s = H_Search.GetListAccountSearchAdvanced(Busqueda_NS.Valor)

                If s.Estatus = "Fracaso" Then
                    Return "Hubo un problema al obtener la información. Verifique la existencia del job o que los datos de conexión sean correctos."
                Else
                    For Each SincronizarCuentaBancaria As Sinc_Account In s.Valor

                        Dim validaCuenta = Await (From n In db.Catalogo_Cuentas Where n.NS_InternalID = SincronizarCuentaBancaria.s_iteminternalId).FirstOrDefaultAsync()

                        If IsNothing(validaCuenta) Then
                            Dim RegistrarCuenta As New Catalogo_Cuentas

                            RegistrarCuenta.NS_InternalID = SincronizarCuentaBancaria.s_iteminternalId
                            RegistrarCuenta.Descripcion = SincronizarCuentaBancaria.s_Descripcion

                            db.Catalogo_Cuentas.Add(RegistrarCuenta)
                            db.SaveChanges()
                        End If
                    Next

                    Return "Proceso actualizado con exito!"
                End If


            Catch ex As Exception

            End Try
        End Function

        Public Function SincronizarUbicacionesExistentes()

            Dim l_location As New List(Of Location)

            l_location = H_Location.GetAllLocation()

            For Each RegistrarLocalizacion In l_location

                Try

                    Dim validaLocalización = (From n In db.Ubicaciones Where n.NS_InternalID = RegistrarLocalizacion.internalId).FirstOrDefault()

                    If IsNothing(validaLocalización) Then
                        Dim nuevaUbicacion As New Ubicaciones

                        nuevaUbicacion.DescripcionAlmacen = RegistrarLocalizacion.name
                        nuevaUbicacion.NS_InternalID = RegistrarLocalizacion.internalId

                        db.Ubicaciones.Add(nuevaUbicacion)
                        db.SaveChanges()
                    Else
                        validaLocalización.DescripcionAlmacen = RegistrarLocalizacion.name
                        db.SaveChanges()
                    End If
                Catch ex As Exception
                    Continue For
                End Try

            Next

            Return "Proceso actualizado con exito!"
        End Function

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

        ''Falta por estructurar






        Public Function SincronizarEstadoArticulosInventariados() As String
            Dim Helpers As New ItemHelper

            Dim l_Ensambles = (From n In db.Catalogo_Productos Where n.Categoria = "Assembly_nL").ToList()

            For Each PrecioEnsambles In l_Ensambles

                Try
                    Dim RegistrarNivelPrecios = Helpers.GetItemAssemblyitemsByID(PrecioEnsambles.NS_InternalID)

                    If Not IsNothing(RegistrarNivelPrecios.locationsList) Then
                        For Each localizacionInventario In RegistrarNivelPrecios.locationsList.locations

                            If localizacionInventario.quantityAvailable <> 0 Then

                                Dim RegistrarStockArticuloInventariado As New StockDisponible

                                Dim GetLocalizacion = (From n In db.Ubicaciones Where n.NS_InternalID = localizacionInventario.locationId.internalId).FirstOrDefault()

                                If IsNothing(GetLocalizacion) Then
                                    Continue For
                                End If

                                RegistrarStockArticuloInventariado.StockDisponible1 = localizacionInventario.quantityAvailable
                                RegistrarStockArticuloInventariado.idUbicacion = GetLocalizacion.idUbicacion
                                RegistrarStockArticuloInventariado.idProducto = PrecioEnsambles.idProducto

                                db.StockDisponible.Add(RegistrarStockArticuloInventariado)
                                db.SaveChanges()

                            End If



                        Next
                    End If

                    If Not IsNothing(RegistrarNivelPrecios.pricingMatrix) Then

                        For Each RegistrarPrecio In RegistrarNivelPrecios.pricingMatrix.pricing
                            Dim Precio As New NivelesPrecioProducto

                            ''VALIDAMOS EXISTENCIA DE NIVEL DE PRECIO

                            Dim ValidarNivelPrecioValido = (From n In db.NivelesPrecioProducto Where n.idProducto = PrecioEnsambles.idProducto And n.Catalogo_NivelesPrecio.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

                            If IsNothing(ValidarNivelPrecioValido) Then
                                Dim GetNivelPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

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

            Return "Proceso actualizado con exito!"
        End Function

        Public Function SincronizarPreciosInventariados() As String
            Dim Helpers As New ItemHelper

            Dim l_Ensambles = (From n In db.Catalogo_Productos Where n.Categoria = "Inventory").ToList()

            For Each PrecioEnsambles In l_Ensambles

                Try
                    Dim RegistrarNivelPrecios = Helpers.GetInventoryItemByID(PrecioEnsambles.NS_InternalID)

                    If Not IsNothing(RegistrarNivelPrecios.locationsList) Then
                        For Each localizacionInventario In RegistrarNivelPrecios.locationsList.locations

                            If localizacionInventario.quantityAvailable <> 0 Then

                                Dim RegistrarStockArticuloInventariado As New StockDisponible

                                Dim GetLocalizacion = (From n In db.Ubicaciones Where n.NS_InternalID = localizacionInventario.locationId.internalId).FirstOrDefault()

                                If IsNothing(GetLocalizacion) Then
                                    Continue For
                                End If

                                RegistrarStockArticuloInventariado.StockDisponible1 = localizacionInventario.quantityAvailable
                                RegistrarStockArticuloInventariado.idUbicacion = GetLocalizacion.idUbicacion
                                RegistrarStockArticuloInventariado.idProducto = PrecioEnsambles.idProducto
                                RegistrarStockArticuloInventariado.IssueInventory = ""
                                RegistrarStockArticuloInventariado.NumLote = ""

                                db.StockDisponible.Add(RegistrarStockArticuloInventariado)
                                db.SaveChanges()

                            End If
                        Next
                    End If

                    'If Not IsNothing(RegistrarNivelPrecios.pricingMatrix) Then

                    '    For Each RegistrarPrecio In RegistrarNivelPrecios.pricingMatrix.pricing
                    '        Dim Precio As New NivelesPrecioProducto

                    '        ''VALIDAMOS EXISTENCIA DE NIVEL DE PRECIO

                    '        Dim ValidarNivelPrecioValido = (From n In db.NivelesPrecioProducto Where n.idProducto = PrecioEnsambles.idProducto And n.Catalogo_NivelesPrecio.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

                    '        If IsNothing(ValidarNivelPrecioValido) Then
                    '            Dim GetNivelPrecio = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

                    '            Precio.idProducto = PrecioEnsambles.idProducto
                    '            Precio.idCategoriaPrecio = GetNivelPrecio.idCategoriaPrecio
                    '            Precio.Precio = RegistrarPrecio.priceList.First.value

                    '            db.NivelesPrecioProducto.Add(Precio)
                    '            db.SaveChanges()
                    '        Else
                    '            ''Solo actualizamos el precio, si es que es diferente

                    '            If ValidarNivelPrecioValido.Precio <> RegistrarPrecio.priceList.First.value Then
                    '                ValidarNivelPrecioValido.Precio = RegistrarPrecio.priceList.First.value
                    '                db.SaveChanges()
                    '            End If
                    '        End If

                    '    Next

                    'End If
                Catch ex As Exception
                    Continue For
                End Try
            Next

            Return "Proceso actualizado con exito!"
        End Function


        'Public Function SincronizarStockPorAlmacen()

        '    Dim Connection As New ConnectionServer
        '    Dim s As New SalesOrderHelper
        '    Dim l_UbicacionLote = (From n In db.Ubicaciones).ToList()
        '    Dim l_MaterialesAssembly = (From n In db.Catalogo_Productos).ToList()

        '    Dim Helpers As New ItemFulFillmentHelper

        '    For Each Materiales In l_MaterialesAssembly

        '        'Dim AltaLote As New PruebaStockDisponible

        '        Dim actualizarStock As New RespuestaServicios

        '        actualizarStock = Helpers.GetInventoryNumber(Ubicacion.DescripcionAlmacen, Materiales.NS_InternalID)

        '        If actualizarStock.Estatus = "Fracaso" Then
        '            Continue For
        '        Else
        '            Dim l_Stock As New List(Of ActualizaStock)
        '            l_Stock = actualizarStock.Valor
        '            For Each MaterialesStock In l_Stock
        '                Dim id = MaterialesStock.InventoryID
        '                Dim validaStock = (From n In db.StockDisponible Where n.idProducto = Materiales.idProducto And n.idUbicacion = Ubicacion.idUbicacion And n.NumLote = id).FirstOrDefault()

        '                If IsNothing(validaStock) Then
        '                    Dim RegistrarStock As New StockDisponible
        '                    RegistrarStock.idProducto = Materiales.idProducto
        '                    RegistrarStock.idUbicacion = Ubicacion.idUbicacion
        '                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)
        '                    RegistrarStock.NumLote = MaterialesStock.InventoryID
        '                    RegistrarStock.IssueInventory = MaterialesStock.IssuesID

        '                    db.StockDisponible.Add(RegistrarStock)
        '                    db.SaveChanges()
        '                Else
        '                    validaStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)

        '                    db.SaveChanges()

        '                End If
        '            Next
        '        End If

        '    Next

        '    Return "Proceso actualizado con exito!"
        'End Function

        Public Function SincronizarStockPorUbicacion()

            Dim Connection As New ConnectionServer
            Dim s As New SalesOrderHelper
            Dim l_MaterialesAssembly = (From n In db.Catalogo_Productos).ToList()

            Dim Helpers As New ItemFulFillmentHelper

            For Each Materiales In l_MaterialesAssembly

                'Dim AltaLote As New PruebaStockDisponible

                Dim actualizarStock As New RespuestaServicios

                actualizarStock = Helpers.GetInventoryNumber("10", Materiales.NS_InternalID)

                If actualizarStock.Estatus = "Fracaso" Then
                    Continue For
                Else
                    Dim l_Stock As New List(Of ActualizaStock)
                    l_Stock = actualizarStock.Valor
                    For Each MaterialesStock In l_Stock
                        Dim id = MaterialesStock.InventoryID
                        Dim validaStock = (From n In db.StockDisponible Where n.idProducto = Materiales.idProducto And n.idUbicacion = 8 And n.NumLote = id).FirstOrDefault()

                        If IsNothing(validaStock) Then
                            Dim RegistrarStock As New StockDisponible
                            RegistrarStock.idProducto = Materiales.idProducto
                            RegistrarStock.idUbicacion = 8
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

            Return "Proceso actualizado con exito!"
        End Function

        Public Function SincronizarProductosDisponibles()

            Dim Connection As New ConnectionServer
            Dim Inventory As New InventaryHelper

            Dim l_InventoryItem As New List(Of InventoryItem)

            l_InventoryItem = Inventory.GetAllInventary()

            For Each RegistrarInventario As InventoryItem In l_InventoryItem

                Dim validaInventario = (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Inventory").FirstOrDefault()

                If IsNothing(validaInventario) Then
                    Dim RegMercancia As New Catalogo_Productos

                    RegMercancia.Categoria = "Inventory"
                    RegMercancia.NS_InternalID = RegistrarInventario.internalId
                    RegMercancia.NS_ExternalID = RegistrarInventario.externalId
                    RegMercancia.Descripcion = RegistrarInventario.displayName
                    'RegMercancia.Precio = RegistrarInventario.
                    db.Catalogo_Productos.Add(RegMercancia)
                    db.SaveChanges()
                End If

            Next

            'For Each Ubicacion In l_UbicacionLote
            '    For Each Materiales In l_MaterialesAssembly

            '        'Dim AltaLote As New PruebaStockDisponible

            '        Dim actualizarStock As New RespuestaServicios

            '        actualizarStock = Helpers.GetInventoryNumber(Ubicacion.DescripcionAlmacen, Materiales.NS_InternalID)

            '        If actualizarStock.Estatus = "Fracaso" Then
            '            Continue For
            '        Else
            '            Dim l_Stock As New List(Of ActualizaStock)
            '            l_Stock = actualizarStock.Valor
            '            For Each MaterialesStock In l_Stock
            '                Dim id = MaterialesStock.InventoryID
            '                Dim validaStock = (From n In db.StockDisponible Where n.idProducto = Materiales.idProducto And n.idUbicacion = Ubicacion.idUbicacion And n.NumLote = id).FirstOrDefault()

            '                If IsNothing(validaStock) Then
            '                    Dim RegistrarStock As New StockDisponible
            '                    RegistrarStock.idProducto = Materiales.idProducto
            '                    RegistrarStock.idUbicacion = Ubicacion.idUbicacion
            '                    RegistrarStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)
            '                    RegistrarStock.NumLote = MaterialesStock.InventoryID
            '                    RegistrarStock.IssueInventory = MaterialesStock.IssuesID

            '                    db.StockDisponible.Add(RegistrarStock)
            '                    db.SaveChanges()
            '                Else
            '                    validaStock.StockDisponible1 = Convert.ToDecimal(MaterialesStock.quantityavailable)

            '                    db.SaveChanges()

            '                End If
            '            Next
            '        End If

            '    Next
            'Next

            Return "Proceso actualizado con exito!"
        End Function



        Public Function SincronizarAssenblyItems()
            Dim Connection As New ConnectionServer
            Dim Inventory As New ItemHelper
            Dim l_clases As New List(Of String)

            l_clases.Add("MUELLES COMERCIALIZACION")
            l_clases.Add("MUELLES MAF")
            l_clases.Add("REFACCIONES COMERCIAL")
            l_clases.Add("REFACCIONES MAF")

            Dim l_InventoryItem As New List(Of LotNumberedAssemblyItem)

            l_InventoryItem = Inventory.GetAllItemsLotNumberedAssemblyItem()

            For Each RegistrarInventario As LotNumberedAssemblyItem In l_InventoryItem

                ''Validacion de Clase
                If Not IsNothing(RegistrarInventario.class) Then

                    If l_clases.Contains(RegistrarInventario.class.name) Then
                        Dim validaInventario = (From n In db.Catalogo_Productos Where n.NS_InternalID = RegistrarInventario.internalId And n.Categoria = "Assembly").FirstOrDefault()

                        If IsNothing(validaInventario) Then
                            Dim RegMercancia As New Catalogo_Productos

                            RegMercancia.Categoria = "Assembly"
                            RegMercancia.NS_InternalID = RegistrarInventario.internalId
                            RegMercancia.NS_ExternalID = RegistrarInventario.itemId
                            RegMercancia.Descripcion = RegistrarInventario.displayName

                            If Not IsNothing(RegistrarInventario.pricingMatrix) Then

                                For Each RegistrarPrecio In RegistrarInventario.pricingMatrix.pricing
                                    Dim Precio As New NivelesPrecioProducto

                                    Dim ValidarNivelPrecioValido = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

                                    If Not IsNothing(ValidarNivelPrecioValido) Then
                                        Precio.idCategoriaPrecio = ValidarNivelPrecioValido.idCategoriaPrecio
                                        Precio.Precio = RegistrarPrecio.priceList.First.value

                                        If ValidarNivelPrecioValido.Descripcion = "Base Price" Then
                                            RegMercancia.Precio = RegistrarPrecio.priceList.First.value
                                        End If

                                        RegMercancia.NivelesPrecioProducto.Add(Precio)
                                    End If
                                Next
                            End If

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

                End If
            Next
        End Function

        Public Function SincronizarFacturasPorCliente(ByVal NS_InternalID As String)
            Dim Item As New InvoiceHelper
            Dim l_Invoice As New List(Of Invoice)

            l_Invoice = Item.GetListInvoiceFromCustomerID(NS_InternalID)

            For Each RegistrarFactura In l_Invoice

                Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarFactura.internalId).FirstOrDefault()

                If IsNothing(ValidaFactura) Then
                    Dim RegInvoice As New Invoice_SO
                    Dim SaldoPendiente = Item.GetBalancePaidInvoice(ValidaFactura.NS_InternalID)

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



        'Public Async Function SincronizarPrecios() As Threading.Tasks.Task
        '    Dim Inventory As New ItemHelper
        '    Dim l_AssemblyItems = Await (From n In db.Catalogo_Productos).ToListAsync()

        '    For Each AssemblyPrice In l_AssemblyItems
        '        Dim GetAssembly = Inventory.GetLotNumberedAssemblyItemByID(AssemblyPrice.NS_InternalID)

        '        If Not IsNothing(GetAssembly.pricingMatrix) Then

        '            Dim ValidarProducto = (From n In db.Catalogo_Productos Where n.NS_InternalID = GetAssembly.internalId).FirstOrDefault()

        '            If ValidarProducto.NivelesPrecioProducto.Count = 0 Then
        '                For Each RegistrarPrecio In GetAssembly.pricingMatrix.pricing
        '                    Dim Precio As New NivelesPrecioProducto

        '                    Dim ValidarNivelPrecioValido = (From n In db.Catalogo_NivelesPrecio Where n.NS_InternalID = RegistrarPrecio.priceLevel.internalId).FirstOrDefault()

        '                    If Not IsNothing(ValidarNivelPrecioValido) Then
        '                        Precio.idProducto = ValidarProducto.idProducto
        '                        Precio.idCategoriaPrecio = ValidarNivelPrecioValido.idCategoriaPrecio
        '                        Precio.Precio = RegistrarPrecio.priceList.First.value

        '                        If ValidarNivelPrecioValido.Descripcion = "Base Price" Then
        '                            ValidarProducto.Precio = RegistrarPrecio.priceList.First.value
        '                        End If

        '                        db.NivelesPrecioProducto.Add(Precio)
        '                        db.SaveChanges()
        '                    End If

        '                Next
        '            End If


        '        End If
        '    Next
        'End Function


        <AllowAnonymous>
        Public Function Pruebasmateria()
            Dim Item As New InvoiceHelper

            Dim r = Item.GetListInvoiceFromCustomerID("334")
            'Dim r = Item.GetListInvoice()

            Dim resultado = r
        End Function

        '<AllowAnonymous>
        'Public Function GetBalance()
        '    Dim Item As New PaymentHelper

        '    Dim r = Item.InsertCustomerPayment()
        '    'Dim r = Item.GetListInvoice()

        '    Dim resultado = r
        'End Function

        <AllowAnonymous>
        Public Function GetBalanceForInvoice()
            Dim Item As New InvoiceHelper

            Dim l_invoice = (From n In db.Invoice_SO).ToList()

            For Each fac In l_invoice
                Dim MontoPagado = Item.GetBalancePaidInvoice(fac.NS_InternalID)

                fac.ImporteAdeudado = MontoPagado
                db.SaveChanges()
            Next

            'Dim r = Item.GetBalancePaidInvoice("11460")
            'Dim r = Item.GetListInvoice()

        End Function

        <AllowAnonymous>
        Public Function GetFacturasPruebas()
            Dim Item As New InvoiceHelper
            Dim l_Invoice As New List(Of Invoice)

            l_Invoice = Item.GetListInvoiceFromCustomerID("334")
            'Dim r = Item.GetListInvoice()

            For Each RegistrarFactura In l_Invoice

                Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarFactura.internalId).FirstOrDefault()

                If IsNothing(ValidaFactura) Then
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
