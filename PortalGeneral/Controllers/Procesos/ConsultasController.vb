Imports System.Threading.Tasks
Imports AutoMapper
Imports System.Data.Entity

Public Class ConsultasController
    Inherits System.Web.Mvc.Controller

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
#End Region

#Region "Usuarios"
    Public Async Function ConsultarUsuarios(Optional ByVal Regla As String = "") As Task(Of List(Of Usuarios))
        Dim l_Usuarios As New List(Of Usuarios)

        If Regla = "" Then

            l_Usuarios = Await (From n In db.Usuarios).ToListAsync()

        ElseIf Regla = "Activos" Then

            l_Usuarios = Await (From n In db.Usuarios Where n.CuentaActiva = True).ToListAsync()

        ElseIf Regla = "Desactivados" Then

            l_Usuarios = Await (From n In db.Usuarios Where n.CuentaActiva = False).ToListAsync()

        ElseIf Regla = "SinConfirmar" Then

            Dim inactivos = Await (From x In db.webpages_Membership Where x.IsConfirmed = False Select x.UserId).ToListAsync()

            l_Usuarios = Await (From x In db.Usuarios Where inactivos.Contains(x.idUsuario) Select x).ToListAsync()

        End If


        Return l_Usuarios
    End Function

    Public Async Function ConsultarUsuarioEspecifico(ByVal id As String, Optional ByVal Caso As String = "") As Task(Of Usuarios)

        Dim Usuario As Usuarios = Nothing

        If Caso = "E" Then

            Usuario = Await (From n In db.Usuarios Where n.idUsuario = id).FirstOrDefaultAsync()

        ElseIf Caso = "C" Then

            Usuario = Await (From n In db.Usuarios Where n.Correo = id).FirstOrDefaultAsync()

        ElseIf Caso = "En" Then

            ''Cambio de Protocolo encryptado Sha256 
            Dim has As New OC.Core.Crypto.Hash
            Dim l_usuarios = Await (From n In db.Usuarios).ToListAsync()

            For Each ValidarUsuario In l_usuarios
                Dim texto As String = "usuarios-" + ValidarUsuario.idUsuario.ToString()
                Dim clave = has.Sha256(texto).ToLower

                If id = clave Then
                    Return ValidarUsuario
                End If
            Next

        End If


        Return Usuario
    End Function
#End Region

#Region "Idiomas"
    Public Async Function ConsultarIdiomas() As Task(Of List(Of Catalogo_Idiomas))

        Dim l_Idiomas As List(Of Catalogo_Idiomas) = Await (From n In db.Catalogo_Idiomas).ToListAsync()

        Return l_Idiomas
    End Function
#End Region

#Region "Roles_Usuario"
    Public Async Function ConsultarRoles() As Task(Of List(Of webpages_Roles))

        Dim l_Roles As List(Of webpages_Roles) = Await (From n In db.webpages_Roles).ToListAsync()

        Return l_Roles
    End Function

#End Region

#Region "Parametros"

    ''Existen dos escenarios en los parametros
    ''Construir un diccionario y consultarlos directamente
    Public Async Function ConstruirDiccionarioParametros() As Task(Of Dictionary(Of String, String))

        Dim parametros As Dictionary(Of String, String) = Await db.Parametros.ToDictionaryAsync(Function(x) x.Clave, Function(x) x.Valor)

        Return parametros
    End Function

    Public Async Function ConsultarParametros() As Task(Of List(Of Parametros))

        Dim l_Parametros As List(Of Parametros) = Await (From n In db.Parametros).ToListAsync()

        Return l_Parametros
    End Function

    Public Async Function ConsultarParametroEspecifico(ByVal id As String) As Task(Of Parametros)

        Dim Parametro As Parametros = Await (From n In db.Parametros Where n.idParametro = id).FirstOrDefaultAsync()

        Return Parametro
    End Function

    Public Async Function ConsultarParametroClave(ByVal Clave As String) As Task(Of String)

        Dim Parametro As Parametros = Await (From n In db.Parametros Where n.Clave = Clave).FirstOrDefaultAsync()

        Return Parametro.Valor
    End Function
#End Region

#Region "webpages_Membership"
    Public Async Function GetWebPages(ByVal Correo As String) As Task(Of webpages_Membership)
        Dim ConsultarUsuario = Await (From s In db.Usuarios Join c In db.webpages_Membership On s.idUsuario Equals c.UserId Where s.Correo = Correo Select c).FirstOrDefaultAsync
        Return ConsultarUsuario
    End Function

    Public Async Function GetMember(ByVal id As String) As Task(Of webpages_Membership)
        Dim Cuenta = Await (From u In db.webpages_Membership Where u.UserId = id).SingleOrDefaultAsync
        Return Cuenta
    End Function
#End Region

#Region "Customes"
    Public Async Function ConsultarListaCustomers(Optional ByVal Take As String = Nothing) As Task(Of List(Of Customers))

        Dim l_Customers As New List(Of Customers)

        If IsNothing(Take) Then
            l_Customers = Await (From n In db.Customers).ToListAsync()
        Else
            l_Customers = Await (From n In db.Customers).Take(Take).ToListAsync()
        End If

        Return l_Customers
    End Function
#End Region

#Region "Items"
    Public Async Function ConsultarListaMateriales(ByVal id As String, ByVal idAlmacen As String, Optional ByVal NombreArchivos As String = "") As Task(Of List(Of StockDisponible))

        Dim l_Items As New List(Of StockDisponible)

        If Not id = "" Then
            l_Items = Await (From n In db.StockDisponible Where n.Catalogo_Productos.NS_ExternalID = id And n.idUbicacion = idAlmacen And n.StockDisponible1 > 0).ToListAsync()
        End If

        If Not NombreArchivos = "" Then
            If l_Items.Count = 0 Then
                l_Items = Await (From n In db.StockDisponible Where n.Catalogo_Productos.Descripcion.Contains(NombreArchivos) And n.idUbicacion = idAlmacen And n.StockDisponible1 > 0).ToListAsync()
            Else
                l_Items = (From n In l_Items Where n.Catalogo_Productos.Descripcion.Contains(NombreArchivos)).ToList()
            End If
        End If

        Return l_Items
    End Function
    Public Async Function ConsultarCp(ByVal CodigoPostal As String) As Task(Of List(Of Catalogo_Cp))

        Dim l_Items = Await (From n In db.Catalogo_Cp Where n.Cp = CodigoPostal).ToListAsync()

        Return l_Items
    End Function
    Public Async Function ConsultarCpFiscal(ByVal CodigoPostalFis As String) As Task(Of List(Of Catalogo_Cp))

        Dim l_Items = Await (From n In db.Catalogo_Cp Where n.Cp = CodigoPostalFis).ToListAsync()

        Return l_Items
    End Function
    Public Async Function ConsultarColonias(ByVal CodigoPostalCol As String) As Task(Of List(Of Catalogo_Colonias))

        Dim l_Items = Await (From n In db.Catalogo_Colonias Where n.CodigoPostal = CodigoPostalCol).ToListAsync()

        Return l_Items
    End Function
    Public Async Function ConsultarColoniasFis(ByVal CodigoPostalColFis As String) As Task(Of List(Of Catalogo_Colonias))

        Dim l_Items = Await (From n In db.Catalogo_Colonias Where n.CodigoPostal = CodigoPostalColFis).ToListAsync()

        Return l_Items
    End Function
    Public Async Function ConsultarMunicipio(ByVal idEstado As String) As Task(Of List(Of Catalogo_Municipios))

        Dim l_Items = Await (From n In db.Catalogo_Municipios Where n.idCatalogoEstados = idEstado).ToListAsync()

        Return l_Items
    End Function
    Public Async Function ConsultarStockPorUbicacion(ByVal id As String) As Task(Of List(Of StockDisponible))
        Dim l_stock = Await (From n In db.StockDisponible Where n.idUbicacion = id).ToListAsync()
        Return l_stock
    End Function
#End Region

#Region "Ubicaciones Venta"
    'Public Async Function ConsultarUbicacionesVentas() As Task(Of List(Of UbicacionesVentas))
    '    Dim l_UbicacionesVenta = Await (From n In db.UbicacionesVentas).ToListAsync()
    '    Return l_UbicacionesVenta
    'End Function
#End Region

#Region "Ubicaciones Almacen"
    Public Async Function ConsultarUbicacionesAlmacen() As Task(Of List(Of Ubicaciones))
        Dim l_Items = Await (From n In db.Ubicaciones).ToListAsync()
        Return l_Items
    End Function
    Public Async Function ConsultarUbicacionesMostrador() As Task(Of List(Of Catalogo_Mostrador))
        Dim l_Items = Await (From n In db.Catalogo_Mostrador).ToListAsync()
        Return l_Items
    End Function

    Public Async Function ConsultarUbicacionesAlmacenActivos() As Task(Of List(Of Ubicaciones))
        Dim l_Items = Await (From n In db.Ubicaciones Where n.EsAlmacen = False).ToListAsync()
        Return l_Items
    End Function
#End Region

#Region "Datos Alta_Clientes"
    Public Async Function ConsultarRegimenFiscal() As Task(Of List(Of Catalogo_RegimenFiscal))
        Dim r_Items = Await (From n In db.Catalogo_RegimenFiscal).ToListAsync()
        Return r_Items
    End Function

    Public Async Function ConsultarPaises() As Task(Of List(Of Catalogo_Paises))
        Dim p_Items = Await (From n In db.Catalogo_Paises).ToListAsync()
        Return p_Items
    End Function

    Public Async Function ConsultarEstados() As Task(Of List(Of Catalogo_Estados))
        Dim e_Items = Await (From n In db.Catalogo_Estados).ToListAsync()
        Return e_Items
    End Function
    Public Async Function ConsultarColonias() As Task(Of List(Of Catalogo_Colonias))
        Dim c_Items = Await (From n In db.Catalogo_Colonias Where n.idCatalogoColonia < 21).ToListAsync()
        Return c_Items
    End Function

#End Region

#Region "Ordenes de Venta"

    Public Async Function ConsultarOrdenesVentaPorUbicacion(ByVal id As String) As Task(Of List(Of SalesOrder))
        Dim l_OrdenVenta = Await (From n In db.SalesOrder Where n.idUbicacion = id Order By n.idSalesOrder Descending).Take(100).ToListAsync()
        Return l_OrdenVenta
    End Function

    Public Async Function ConsultarOrdenesVenta() As Task(Of List(Of SalesOrder))
        Dim l_OrdenVenta = Await (From n In db.SalesOrder Order By n.idSalesOrder Descending).ToListAsync()
        Return l_OrdenVenta
    End Function

    Public Async Function ConsultarOrdenesVentaPorID(ByVal id As String) As Task(Of SalesOrder)
        Dim ordenVenta = Await (From n In db.SalesOrder Where n.idSalesOrder = id).FirstOrDefaultAsync()
        Return ordenVenta
    End Function
#End Region

#Region "Detalle Orden de Venta"
    Public Async Function ConsultarDetalleOrden(ByVal id As String) As Task(Of List(Of DetalleSalesOrder))
        Dim l_ordenVenta = Await (From n In db.DetalleSalesOrder Where n.idSalesOrder = id).ToListAsync()
        Return l_ordenVenta
    End Function
#End Region

#Region "Invoice"
    Public Async Function ConsultarFacturasPorCliente(ByVal id As String) As Task(Of List(Of Invoice_SO))

        Dim l_Invoice As List(Of Invoice_SO) = Await (From n In db.Invoice_SO Where n.idCustomer = id).ToListAsync()

        Return l_Invoice
    End Function
#End Region
#Region "Estimaciones"
    Public Async Function ConsultarEstimacionesPorID(ByVal id As String) As Task(Of Estimaciones)
        Dim estimacion = Await (From n In db.Estimaciones Where n.idEstimacion = id).FirstOrDefaultAsync()
        Return estimacion
    End Function
#End Region

#Region "Metodos de Pago"
    Public Async Function ConsultaListaMetodosPago() As Task(Of List(Of Catalogo_MetodosPago))
        Dim l_MetodosPago = Await (From n In db.Catalogo_MetodosPago Where n.Activo = True).ToListAsync()
        Return l_MetodosPago
    End Function
#End Region

#Region "Metodos Pago SAT"
    Public Async Function ConsultaListaMetodosPagoSAT() As Task(Of List(Of Catalogo_MetodoPagoSAT))
        Dim l_MetodosPago = Await (From n In db.Catalogo_MetodoPagoSAT).ToListAsync()
        Return l_MetodosPago
    End Function
#End Region

#Region "Forma de Pago SAT"
    Public Async Function ConsultaListaFormaPagoSAT() As Task(Of List(Of Catalogo_FormasPagoSAT))
        Dim l_FormasPago = Await (From n In db.Catalogo_FormasPagoSAT).ToListAsync()
        Return l_FormasPago
    End Function
#End Region

#Region "Uso CFDI"
    Public Async Function ConsultaListaUsoCFDI() As Task(Of List(Of Catalogo_UsoCFDI))
        Dim l_UsoCFDI = Await (From n In db.Catalogo_UsoCFDI).ToListAsync()
        Return l_UsoCFDI
    End Function
#End Region

#Region "Bancos"
    Public Async Function ConsultaBancos() As Task(Of List(Of Catalogo_Bancos))
        Dim l_Bancos = Await (From n In db.Catalogo_Bancos Where n.NS_InternalID IsNot Nothing).ToListAsync()
        Return l_Bancos
    End Function

    'Public Async Function ConsultaTarjetaBanco(ByVal metodoPago As String) As Task(Of List(Of Catalogo_Bancos))
    '    Dim l_Tarjeta = Await (From n In db.Catalogo_Bancos Where n.NS_InternalID IsNot Nothing).ToListAsync()
    '    Return l_Bancos
    'End Function
#End Region

End Class
