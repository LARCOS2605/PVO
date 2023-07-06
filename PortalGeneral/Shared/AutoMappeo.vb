Imports System.ComponentModel
Imports System.Data.Entity
Imports AutoMapper
Imports NetsuiteLibrary.Clases

Public Class AutoMappeo
    Public db As New PVO_NetsuiteEntities
    Public Function AutoMapperUsuarios(ByVal Model As UsuariosViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of UsuariosViewModel, Usuarios)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        Dim Ob = mapper.Map(Of UsuariosViewModel, Usuarios)(Model)

        Return Ob

    End Function

    Public Function AutoMapperUserToViewModel(ByVal Model As Usuarios, Optional ByVal Regla As String = "")

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Usuarios, UsuariosViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        Dim Ob = mapper.Map(Of Usuarios, UsuariosViewModel)(Model)

        Return Ob

    End Function

    Public Function AutoMapperListaUsuarios(ByVal Model As List(Of Usuarios))

        ''Esta puta mierda no funciona con las listas
        ''Se tendrá que hacer un ciclo manual
        Dim l_Usuarios As New List(Of UsuariosViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Usuarios, UsuariosViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseUser In Model

            Dim Ob = mapper.Map(Of Usuarios, UsuariosViewModel)(ParseUser)

            Ob.perfil = ParseUser.webpages_Roles.First.Description

            l_Usuarios.Add(Ob)
        Next


        Return l_Usuarios
    End Function

    Public Function AutoMapperParametros(ByVal Model As ParametrosViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of ParametrosViewModel, Parametros)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        Dim Ob = mapper.Map(Of ParametrosViewModel, Parametros)(Model)

        Return Ob
    End Function

    Public Function AutoMapperListaParametros(ByVal Model As List(Of Parametros))

        Dim l_Parametros As New List(Of ParametrosViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Parametros, ParametrosViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseParametros In Model

            Dim Ob = mapper.Map(Of Parametros, ParametrosViewModel)(ParseParametros)

            l_Parametros.Add(Ob)
        Next


        Return l_Parametros
    End Function

    Public Function AutoMapperProductosAlmacen(ByVal Model As List(Of StockDisponible), Optional ByVal Customer As String = "")

        Dim l_Productos As New List(Of AlmacenProductoViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of StockDisponible, AlmacenProductoViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of StockDisponible, AlmacenProductoViewModel)(ParseProducto)

            Ob.Producto = ParseProducto.Catalogo_Productos.Descripcion
            Ob.Ubicacion = ParseProducto.Ubicaciones.DescripcionAlmacen
            Ob.ID_NS = ParseProducto.Catalogo_Productos.NS_InternalID
            Ob.TipoInventario = ParseProducto.Catalogo_Productos.Categoria
            Ob.Clave = ParseProducto.Catalogo_Productos.NS_ExternalID

            Dim ValidarNivelPrecio = (From n In db.Customers Where n.NS_InternalID = Customer Select n.idCategoriaPrecio).FirstOrDefault()

            If Not IsNothing(ValidarNivelPrecio) Then
                Dim NivelPrecio = (From n In db.NivelesPrecioProducto Where n.idProducto = ParseProducto.Catalogo_Productos.idProducto And n.idCategoriaPrecio = ValidarNivelPrecio).FirstOrDefault()

                If Not IsNothing(NivelPrecio) Then
                    Ob.Precio = NivelPrecio.Precio
                    Ob.Descuento = NivelPrecio.Catalogo_NivelesPrecio.Descripcion
                Else
                    Dim NivelPrecioBase = (From n In db.NivelesPrecioProducto Where n.idProducto = ParseProducto.Catalogo_Productos.idProducto And n.Catalogo_NivelesPrecio.Descripcion = "Precio Base").FirstOrDefault()
                    If Not IsNothing(NivelPrecioBase) Then
                        Ob.Precio = NivelPrecioBase.Precio
                        Ob.Descuento = "Sin Descuento"
                    Else
                        Ob.Precio = 0
                        Ob.Descuento = "Sin Descuento"
                    End If
                End If
            Else
                Dim NivelPrecioBase = (From n In db.NivelesPrecioProducto Where n.idProducto = ParseProducto.Catalogo_Productos.idProducto And n.Catalogo_NivelesPrecio.Descripcion = "Precio Base").FirstOrDefault()
                If Not IsNothing(NivelPrecioBase) Then
                    Ob.Precio = NivelPrecioBase.Precio
                    Ob.Descuento = "Sin Descuento"
                End If
            End If


            l_Productos.Add(Ob)
        Next


        Return l_Productos
    End Function
    Public Function AutoMapperInformacionColonias(ByVal Model As List(Of Catalogo_Colonias))


        Dim l_clave As New List(Of DomicilioColoniasDomViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Colonias, DomicilioColoniasDomViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Colonias, DomicilioColoniasDomViewModel)(ParseProducto)

            Ob.Cp = ParseProducto.CodigoPostal

            If Not IsNothing(Ob.Cp) Then

                Ob.idColonia = ParseProducto.Clave
                Ob.Colonia = ParseProducto.Descripcion

            End If
            l_clave.Add(Ob)
        Next

        Return l_clave
    End Function
    Public Function AutoMapperInformacionColoniasFis(ByVal Model As List(Of Catalogo_Colonias))


        Dim l_clave As New List(Of DomicilioColoniasFisViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Colonias, DomicilioColoniasFisViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Colonias, DomicilioColoniasFisViewModel)(ParseProducto)

            Ob.Cp = ParseProducto.CodigoPostal

            If Not IsNothing(Ob.Cp) Then

                Ob.idColonia = ParseProducto.Clave
                Ob.Colonia = ParseProducto.Descripcion.ToUpper

            End If
            l_clave.Add(Ob)
        Next

        Return l_clave
    End Function
    Public Function AutoMapperDomicilioNuevo(ByVal Model As List(Of Catalogo_Municipios))


        Dim l_clave As New List(Of InfoNuevoClienteViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Municipios, InfoNuevoClienteViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Municipios, InfoNuevoClienteViewModel)(ParseProducto)

            Ob.idMunicipio = ParseProducto.idCatalogoMunicipio

            If Not IsNothing(Ob.idMunicipio) Then

                Ob.idMunicipio = ParseProducto.idCatalogoMunicipio
                Ob.Municipio = ParseProducto.Descripcion
                Ob.Clave = ParseProducto.Clave

            End If
            l_clave.Add(Ob)
        Next

        Return l_clave
    End Function
    Public Function AutoMapperInformacionDomicilio(ByVal Model As List(Of Catalogo_Cp))


        Dim l_clave As New List(Of DomicilioClienteViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Cp, DomicilioClienteViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Cp, DomicilioClienteViewModel)(ParseProducto)

            Ob.Cp = ParseProducto.Cp
            Ob.idEstado = ParseProducto.idCatalogoEstados
            Ob.idMunicipio = ParseProducto.idCatalogoMunicipio
            Ob.idLocalidad = ParseProducto.idCatalogoLocalidad

            If Not IsNothing(Ob.Cp) Then
                Dim B_Estado = (From n In db.Catalogo_Estados Where n.idCatalogoEstados = ParseProducto.idCatalogoEstados).FirstOrDefault()
                Dim clave_pais = B_Estado.Clave

                Ob.Estado = B_Estado.Descripcion 'Descripcion del estado
                Ob.NsInternalEstado = B_Estado.NS_ExternalID 'Ns_ExternalId del estado
                Ob.idPais = B_Estado.Clave 'Clave del País

                Dim B_Pais = (From n In db.Catalogo_Paises Where n.NS_ExternalID = clave_pais).FirstOrDefault()

                Ob.Pais = B_Pais.Descripcion 'Descripción del País

                Dim B_Municipio = (From n In db.Catalogo_Municipios Where n.Clave = ParseProducto.idCatalogoMunicipio And n.idCatalogoEstados = ParseProducto.idCatalogoEstados).FirstOrDefault()

                Ob.Municipio = B_Municipio.Descripcion 'Descripción del Municipio

                Dim B_Localidades = (From n In db.Catalogo_Localidades Where n.idCatalogoEstados = ParseProducto.idCatalogoEstados And n.Clave = ParseProducto.idCatalogoLocalidad).FirstOrDefault()

                Ob.Localidad = B_Localidades.Descripcion 'Descripción de la Localidad

                Dim B_Colonias = (From n In db.Catalogo_Colonias Where n.CodigoPostal = ParseProducto.Cp).ToListAsync()


            End If
            l_clave.Add(Ob)
        Next

        Return l_clave
    End Function

    Public Function AutoMapperInformacionDomicilioFiscal(ByVal Model As List(Of Catalogo_Cp))

        Dim l_claves As New List(Of DomicilioFiscalViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Cp, DomicilioFiscalViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Cp, DomicilioFiscalViewModel)(ParseProducto)

            Ob.Cp = ParseProducto.Cp
            Ob.idEstado = ParseProducto.idCatalogoEstados
            Ob.idMunicipio = ParseProducto.idCatalogoMunicipio

            If (ParseProducto.idCatalogoLocalidad = "") Then
                Ob.idLocalidad = ParseProducto.idCatalogoMunicipio
            Else
                Ob.idLocalidad = ParseProducto.idCatalogoLocalidad
            End If


            If Not IsNothing(Ob.Cp) Then
                Dim B_Estado = (From n In db.Catalogo_Estados Where n.idCatalogoEstados = ParseProducto.idCatalogoEstados).FirstOrDefault()
                Dim clave_pais = B_Estado.Clave

                Ob.Estado = B_Estado.Descripcion 'Descripcion del estado
                Ob.NsInternalEstado = B_Estado.NS_ExternalID 'Ns_ExternalId del estado
                Ob.idPais = B_Estado.Clave 'Clave del País

                Dim B_Pais = (From n In db.Catalogo_Paises Where n.NS_ExternalID = clave_pais).FirstOrDefault()

                Ob.Pais = B_Pais.Descripcion 'Descripción del País

                Dim B_Municipio = (From n In db.Catalogo_Municipios Where n.Clave = ParseProducto.idCatalogoMunicipio And n.idCatalogoEstados = ParseProducto.idCatalogoEstados).FirstOrDefault()

                Ob.Municipio = B_Municipio.Descripcion 'Descripción del Municipio

                If (ParseProducto.idCatalogoLocalidad = "") Then
                    Ob.Localidad = B_Municipio.Descripcion 'Descripción de la Localidad
                Else
                    Dim B_Localidades = (From n In db.Catalogo_Localidades Where n.idCatalogoEstados = ParseProducto.idCatalogoEstados And n.Clave = ParseProducto.idCatalogoLocalidad).FirstOrDefault()

                    Ob.Localidad = B_Localidades.Descripcion 'Descripción de la Localidad
                End If

            End If
            l_claves.Add(Ob)
        Next


        Return l_claves
    End Function

    Public Function AutoMapperProductosServicio(ByVal Model As List(Of Catalogo_Productos))

        Dim l_Productos As New List(Of AlmacenProductoViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Catalogo_Productos, AlmacenProductoViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of Catalogo_Productos, AlmacenProductoViewModel)(ParseProducto)

            Ob.Producto = ParseProducto.Descripcion
            Ob.ID_NS = ParseProducto.NS_InternalID
            Ob.TipoInventario = ParseProducto.Categoria
            Ob.Clave = ParseProducto.NS_ExternalID
            Ob.StockDisponible1 = 1

            Dim NivelPrecioBase = (From n In db.NivelesPrecioProducto Where n.idProducto = ParseProducto.idProducto And n.Catalogo_NivelesPrecio.Descripcion = "Precio Base").FirstOrDefault()

            If Not IsNothing(NivelPrecioBase) Then
                Ob.Precio = NivelPrecioBase.Precio
                Ob.Descuento = "Sin Descuento"
            Else
                Ob.Precio = 0
                Ob.Descuento = "Sin Descuento"
            End If

            l_Productos.Add(Ob)
        Next


        Return l_Productos
    End Function

    Public Function AutoMapperSalesOrder(ByVal Model As SalesOrder)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of SalesOrder, SalesOrderMapViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        Dim Ob = mapper.Map(Of SalesOrder, SalesOrderMapViewModel)(Model)

        Ob.Nombre_Mostrador = Model.Ubicaciones.DescripcionAlmacen
        Ob.Nombre_Cliente = Model.Customers.Nombre
        Ob.ClaveCliente = Model.Customers.NS_ExternalID
        Ob.Estatus = Model.Estatus.ClaveInterna
        Ob.Fecha = Convert.ToDateTime(Model.FechaCreacion).ToString("dd/MM/yyyy")

        If Model.Invoice_SO.Count <> 0 Then
            Ob.NS_Val_INV = Model.Invoice_SO.First.NS_InternalID
            Ob.NS_ID_Invoice = Model.Invoice_SO.First.idInvoice
            Ob.NoFactura = Model.Invoice_SO.First.NS_ExternalID

            'If IsNothing(Model.Invoice_SO.First.UUID) Then

            'End If

        End If
        'Ob.UsoCFDI = Model.idUsoCFDI
        'Ob.MetodoPagoSAT = Model.idMetodoPagoSAT
        'Ob.FormaPagoSAT = Model.idMetodoPagoSAT

        Return Ob
    End Function

    Public Function AutoMapperListaSalesOrder(ByVal Model As List(Of SalesOrder))

        Dim l_SO As New List(Of SalesOrderMapViewModel)
        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of SalesOrder, SalesOrderMapViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of SalesOrder, SalesOrderMapViewModel)(ParseProducto)

            Ob.Nombre_Mostrador = ParseProducto.Ubicaciones.DescripcionAlmacen
            Ob.Nombre_Cliente = ParseProducto.Customers.Nombre
            Ob.Estatus = ParseProducto.Estatus.ClaveInterna
            Ob.Fecha = Convert.ToDateTime(ParseProducto.FechaCreacion).ToString("")
            Ob.ClaveCliente = ParseProducto.Customers.NS_ExternalID

            If ParseProducto.Invoice_SO.Count = 0 Then
                Ob.ValInvoice = "Sin Factura"
            Else
                Ob.ValInvoice = ParseProducto.Invoice_SO.First.NS_ExternalID
            End If

            If ParseProducto.Estatus.ClaveExterna = "SO_Facturada" And ParseProducto.Invoice_SO.Count = 0 Then
                Ob.TieneFactura = "False"
            Else
                Ob.TieneFactura = "True"
            End If

            l_SO.Add(Ob)
        Next

        Return l_SO
    End Function

    Public Function AutoMapperDetallePedido(ByVal Model As List(Of DetalleSalesOrder))

        Dim l_Productos As New List(Of DetalleSalesOrderMapViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of DetalleSalesOrder, DetalleSalesOrderMapViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseProducto In Model

            Dim Ob = mapper.Map(Of DetalleSalesOrder, DetalleSalesOrderMapViewModel)(ParseProducto)

            Ob.DescripcionProducto = ParseProducto.Catalogo_Productos.Descripcion
            Ob.EstatusEntrega = ParseProducto.Estatus.ClaveExterna
            Ob.ClaveProducto = ParseProducto.Catalogo_Productos.NS_ExternalID
            l_Productos.Add(Ob)
        Next


        Return l_Productos
    End Function

    Public Function AutoMapperCustomer(ByVal Model As List(Of Customers))

        Dim l_Clientes As New List(Of CustomerViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Customers, CustomerViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseCliente In Model

            Dim Ob = mapper.Map(Of Customers, CustomerViewModel)(ParseCliente)

            If IsNothing(ParseCliente.RFC) Then
                Ob.RFC = "Sin RFC"
            End If

            If IsNothing(ParseCliente.idCategoriaPrecio) Then
                Ob.NombrePrecio = "Precio Base"
            Else
                Ob.NombrePrecio = ParseCliente.Catalogo_NivelesPrecio.Descripcion
            End If

            If IsNothing(ParseCliente.idCatalogoTipoCliente) Then
                Ob.TipoCliente = "Sin Categoria Cliente"
            Else
                Ob.TipoCliente = ParseCliente.Catalogo_TipoCliente.Descripcion
            End If

            Ob.Limite_Credito = Convert.ToDecimal(ParseCliente.Limite_Credito).ToString("C", New System.Globalization.CultureInfo("es-MX"))

            If IsNothing(ParseCliente.idCatalogoTerminos) Then
                Ob.Terminos = "Sin Terminos"
            Else
                Ob.Terminos = ParseCliente.Catalogo_Terminos.Descripcion
            End If

            If IsNothing(ParseCliente.Dias_Atraso) Then
                Ob.Dias_Atraso = 0
            Else
                Ob.Dias_Atraso = Math.Floor(Convert.ToDecimal(ParseCliente.Dias_Atraso))
            End If

            l_Clientes.Add(Ob)
        Next


        Return l_Clientes
    End Function

    Public Function AutoMapperInvoice(ByVal Model As List(Of Invoice_SO))

        Dim l_Invoice As New List(Of InvoiceViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of Invoice_SO, InvoiceViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseInvoice In Model

            Dim Ob = mapper.Map(Of Invoice_SO, InvoiceViewModel)(ParseInvoice)

            Ob.NombreCliente = ParseInvoice.Customers.Nombre
            Ob.Fecha = Convert.ToDateTime(ParseInvoice.FechaCreacion).ToString("dd/MM/yyyy")

            If Not IsNothing(ParseInvoice.Catalogo_MetodoPagoSAT) Then
                If ParseInvoice.Catalogo_MetodoPagoSAT.NS_InternalID = 4 Then
                    Ob.MetodoPagoSAT = "PPD"
                Else
                    Ob.MetodoPagoSAT = "PUE"
                End If
            Else
                Ob.MetodoPagoSAT = "S/M"
            End If


            l_Invoice.Add(Ob)
        Next


        Return l_Invoice
    End Function

    Public Function AutoMapperStockDisponible(ByVal Model As List(Of StockDisponible))

        Dim l_stockDisponible As New List(Of StockDisponibleViewModel)

        Dim MapperConfiguration = New MapperConfiguration(Sub(config)
                                                              config.CreateMap(Of StockDisponible, StockDisponibleViewModel)()
                                                          End Sub)
        Dim mapper = MapperConfiguration.CreateMapper()

        For Each ParseArticulo In Model

            Dim Ob = mapper.Map(Of StockDisponible, StockDisponibleViewModel)(ParseArticulo)

            Ob.NombreArticulo = ParseArticulo.Catalogo_Productos.Descripcion
            Ob.NS_Externalid = ParseArticulo.Catalogo_Productos.NS_ExternalID
            Ob.StockDisponible = ParseArticulo.StockDisponible1

            l_stockDisponible.Add(Ob)
        Next


        Return l_stockDisponible
    End Function
End Class
