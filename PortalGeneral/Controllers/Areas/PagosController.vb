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
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports WebMatrix.WebData
Imports Newtonsoft.Json
Imports System.Data.Entity.Migrations

Namespace Controllers.Areas
    <Authorize()>
    <HandleError>
    Public Class PagosController
        Inherits AppBaseController

#Region "Constructores"
        Public db As New PVO_NetsuiteEntities
        Public AutoMap As New AutoMappeo
        Public Consulta As New ConsultasController
        Public H_Invoice As New InvoiceHelper
        Public H_Search As New SearchHelper
        Public H_Servlet As New ConnectionServer
        Public H_Payment As New PaymentHelper
        Public H_Customer As New CustomerHelper
#End Region

#Region "Vistas"
        Public Async Function GenerarPagos() As Task(Of ActionResult)
            Await GeneraViewbags()
            Return View()
        End Function
        Public Async Function ConsultarPagosAplicados() As Task(Of ActionResult)
            Dim Select_Customer As New List(Of SelectListItem)

            Dim l_Customers = Await Consulta.ConsultarListaCustomers()

            'For Each RegistroCustomer In l_Customers
            '    Dim nombreCliente = RegistroCustomer.NS_ExternalID + " - " + RegistroCustomer.Nombre
            '    Select_Customer.Add(New SelectListItem With {.Text = nombreCliente, .Value = RegistroCustomer.NS_InternalID})
            'Next

            ViewBag.ClientesDisponibles = l_Customers
            Return View()
        End Function

#End Region

#Region "Procesos"
        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarFacturasPorCliente(ByVal id As String) As JsonResult
            Try

                Dim l_Invoice As New List(Of MapFacturasPendientesPago)
                'Dim l_InvoiceView As New List(Of InvoiceViewModel)

                'Dim validaCliente = Await (From n In db.Customers Where n.NS_InternalID = id).FirstOrDefaultAsync()

                'If IsNothing(validaCliente) Then
                '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "", l_Invoice))
                'End If

                'l_Invoice = Await Consulta.ConsultarFacturasPorCliente(validaCliente.idCustomer)

                'GetFacturasClientes()
                'l_Invoice = GetFacturasClientes(validaCliente.NS_InternalID)
                l_Invoice = GetFacturasClientes(id)
                'l_Invoice = ActualizarBalanceFacturas(validaCliente.idCustomer)

                'l_InvoiceView = AutoMap.AutoMapperInvoice(l_Invoice)

                'l_InvoiceView = (From n In l_InvoiceView Order By n.NS_InternalID Descending).ToList()

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_Invoice))
            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message))
            End Try
        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Async Function GenerarPagos(ByVal model As GeneraPagosViewModel, ByVal Pagos As List(Of DetallesPagosRenderViewModel)) As Task(Of ActionResult)

            Dim PagosHelper As New PaymentHelper
            Dim FacturaHelper As New InvoiceHelper
            Dim Respuesta As New RespuestaServicios
            Dim RegistrarPagos As New AltaPagos
            Dim l_DetallePago As New List(Of DetalleAltaPagos)

#Region "Recuperar Datos Netsuite"
            Dim ValidarCliente = (From n In db.Customers Where n.NS_InternalID = model.Customer).FirstOrDefault()

            If IsNothing(ValidarCliente) Then
                Try
                    Using context As New PVO_NetsuiteEntities
                        context.Configuration.AutoDetectChangesEnabled = False
                        context.Configuration.ValidateOnSaveEnabled = False

                        Dim ClienteEspecifico As Customer

                        ClienteEspecifico = H_Customer.GetCustomerById(model.Customer)

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
                    End Using
                Catch ex As Exception

                End Try
            End If

            'For Each Documento In Pagos
            '    Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = Documento.NS_InternalID).FirstOrDefault()

            '    If IsNothing(ValidaFactura) Then
            '        Try
            '            Dim RegistrarFactura = H_Invoice.GetInvoiceByID(Documento.NS_InternalID)

            '            Dim RegInvoice As New Invoice_SO

            '            RegInvoice.NS_InternalID = RegistrarFactura.internalId
            '            RegInvoice.NS_ExternalID = RegistrarFactura.tranId
            '            RegInvoice.idEstatus = (From n In db.Estatus Where n.ClaveInterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
            '            RegInvoice.FechaCreacion = RegistrarFactura.tranDate
            '            RegInvoice.idCustomer = (From n In db.Customers Where n.NS_InternalID = RegistrarFactura.entity.internalId Select n.idCustomer).FirstOrDefault()
            '            RegInvoice.Subtotal = RegistrarFactura.subTotal
            '            RegInvoice.Total_Impuestos = RegistrarFactura.taxTotal
            '            RegInvoice.Total = RegistrarFactura.total
            '            RegInvoice.ImporteAdeudado = RegistrarFactura.amountRemaining

            '            Dim validaDatos = 0

            '            If Not IsNothing(RegistrarFactura.createdFrom) Then
            '                validaDatos = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()
            '            End If

            '            If Not validaDatos = 0 Then
            '                RegInvoice.idSalesOrder = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId Select n.idSalesOrder).FirstOrDefault()

            '                Dim CambioEstatus = (From n In db.SalesOrder Where n.NS_InternalID = RegistrarFactura.createdFrom.internalId).FirstOrDefault()

            '                CambioEstatus.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "SO_Facturada" Select n.idEstatus).FirstOrDefault()
            '            End If

            '            db.Invoice_SO.Add(RegInvoice)
            '            db.SaveChanges()
            '        Catch ex As Exception

            '        End Try

            '    End If
            'Next
#End Region

            ''MetodosPago
            Dim l_TarjetaDebito = db.JG_CoincidenciaMetodoPago("Tarjeta Debito")
            Dim l_TarjetaCredito = db.JG_CoincidenciaMetodoPago("Tarjeta Credito")
            Dim l_Efectivo = db.JG_CoincidenciaMetodoPago("Efectivo")
            Dim l_Transferencia = db.JG_CoincidenciaMetodoPago("Transferencia")
            Dim l_Cheque = db.JG_CoincidenciaMetodoPago("Cheque")

#Region "Generar Combos"
            ''Generar ViewBags de Vistas
            Await GeneraViewbags()
#End Region

#Region "Validar Selección Mostrador y Validación de Ubicación"
            ''Validar mostrador Existencia y Validación de la ubicacion
            Dim UbicacionUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefaultAsync()
            If IsNothing(UbicacionUsuario.MostradorPref) Then
                Flash.Instance.Error("Para poder generar un pago, es necesario seleccionar un mostrador.")
                Return RedirectToAction("GenerarPagos")
            End If

            Dim GetUbicacion = Await (From n In db.Ubicaciones Where n.idUbicacion = UbicacionUsuario.MostradorPref).FirstOrDefaultAsync()
            If IsNothing(GetUbicacion) Then
                Flash.Instance.Error("No fue posible localizar la ubicación del mostrador.")
                Return RedirectToAction("GenerarPagos")
            End If
#End Region

#Region "Mapeo de Campos para Envio a NetSuite"
            ''Mapeo de los Objetos para el proceso de NetSuite
            RegistrarPagos.InternalID_Customer = model.Customer
            RegistrarPagos.InternalID_MetodoPago = model.MetodoPago
            RegistrarPagos.Localizacion = GetUbicacion.DescripcionAlmacen
            RegistrarPagos.memo = model.Nota
            RegistrarPagos.FechaPago = model.FechaPago

            If l_TarjetaDebito.Contains(model.MetodoPago) Then
                Dim MetodoPagoSat = (From n In db.Catalogo_FormasPagoSAT Where n.Descripcion = "28 - Tarjeta de Débito").FirstOrDefault()

                If IsNothing(MetodoPagoSat) Then
                    Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la forma de pago SAT. Verifique su existencia."))
                    Return RedirectToAction("GenerarPagos")
                Else
                    Dim Terminal = (From n In db.Catalogo_Terminales Where n.idUbicacion = GetUbicacion.idUbicacion).FirstOrDefault()

                    If IsNothing(Terminal) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la terminal bancaria, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoOrdenante = (From n In db.Catalogo_Bancos Where n.idCatalogoBanco = model.BancoOrdenante).FirstOrDefault()

                    If IsNothing(BancoOrdenante) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco ordenante, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoBeneficiario = (From n In db.Catalogo_CuentasBancarias Where n.Catalogo_MetodosPago.NS_InternalID = model.MetodoPago).FirstOrDefault()

                    If IsNothing(BancoBeneficiario) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco beneficiario, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    RegistrarPagos.MetodoPagoSAT = MetodoPagoSat.NS_InternalID
                    RegistrarPagos.RFCBanco_Beneficiario = BancoBeneficiario.Catalogo_Bancos.RFC
                    RegistrarPagos.NumCuenta_Beneficiario = BancoBeneficiario.NumCuenta
                    RegistrarPagos.NS_ID_BancoBeneficiario = BancoBeneficiario.Catalogo_Bancos.NS_InternalID
                    RegistrarPagos.RFCBanco_Ordenante = BancoOrdenante.RFC
                    RegistrarPagos.NS_ID_BancoOrdenante = BancoOrdenante.NS_InternalID
                    RegistrarPagos.Terminal = Terminal.Descripcion
                    RegistrarPagos.NumTransaccion = model.NoRefPago
                    RegistrarPagos.TipoMetodoPago = "Tarjeta Debito"

                    Dim DatosCuenta = model.NumCuentaOrdenante.Trim()

                    If DatosCuenta.Length = 4 Then
                        DatosCuenta = String.Format("000000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 5 Then
                        DatosCuenta = String.Format("00000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 6 Then
                        DatosCuenta = String.Format("0000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 7 Then
                        DatosCuenta = String.Format("000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 8 Then
                        DatosCuenta = String.Format("00000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 9 Then
                        DatosCuenta = String.Format("0000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 10 Then
                        DatosCuenta = String.Format("000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 11 Then
                        DatosCuenta = String.Format("00000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 12 Then
                        DatosCuenta = String.Format("0000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 13 Then
                        DatosCuenta = String.Format("000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 14 Then
                        DatosCuenta = String.Format("00{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 15 Then
                        DatosCuenta = String.Format("0{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 16 Then
                        DatosCuenta = String.Format("{0}", DatosCuenta)
                    End If

                    RegistrarPagos.NumCuenta_Ordenante = DatosCuenta

                End If

            ElseIf l_Efectivo.Contains(model.MetodoPago) Then

                Dim MetodoPagoSat = Await (From n In db.Catalogo_FormasPagoSAT Where n.Descripcion = "01 - Efectivo").FirstOrDefaultAsync()

                If IsNothing(MetodoPagoSat) Then
                    Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la forma de pago SAT. Verifique su existencia."))
                    Return RedirectToAction("GenerarPagos")
                Else
                    RegistrarPagos.MetodoPagoSAT = MetodoPagoSat.NS_InternalID
                End If

            ElseIf l_TarjetaCredito.Contains(model.MetodoPago) Then

                Dim MetodoPagoSat = (From n In db.Catalogo_FormasPagoSAT Where n.Descripcion = "04 - Tarjeta de Crédito").FirstOrDefault()

                If IsNothing(MetodoPagoSat) Then
                    Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la forma de pago SAT. Verifique su existencia."))
                    Return RedirectToAction("GenerarPagos")
                Else
                    Dim Terminal = (From n In db.Catalogo_Terminales Where n.idUbicacion = GetUbicacion.idUbicacion).FirstOrDefault()

                    If IsNothing(Terminal) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la terminal bancaria, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoOrdenante = (From n In db.Catalogo_Bancos Where n.idCatalogoBanco = model.BancoOrdenante).FirstOrDefault()

                    If IsNothing(BancoOrdenante) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco ordenante, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoBeneficiario = (From n In db.Catalogo_CuentasBancarias Where n.Catalogo_MetodosPago.NS_InternalID = model.MetodoPago).FirstOrDefault()

                    If IsNothing(BancoBeneficiario) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco beneficiario, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    RegistrarPagos.MetodoPagoSAT = MetodoPagoSat.NS_InternalID
                    RegistrarPagos.RFCBanco_Beneficiario = BancoBeneficiario.Catalogo_Bancos.RFC
                    RegistrarPagos.NumCuenta_Beneficiario = BancoBeneficiario.NumCuenta
                    RegistrarPagos.NS_ID_BancoBeneficiario = BancoBeneficiario.Catalogo_Bancos.NS_InternalID
                    RegistrarPagos.RFCBanco_Ordenante = BancoOrdenante.RFC
                    'RegistrarPagos.NumCuenta_Ordenante = model.NumCuentaOrdenante.Trim()
                    RegistrarPagos.NS_ID_BancoOrdenante = BancoOrdenante.NS_InternalID
                    RegistrarPagos.Terminal = Terminal.Descripcion
                    RegistrarPagos.NumTransaccion = model.NoRefPago
                    RegistrarPagos.TipoMetodoPago = "Tarjeta Credito"

                    Dim DatosCuenta = model.NumCuentaOrdenante.Trim()

                    If DatosCuenta.Length = 4 Then
                        DatosCuenta = String.Format("000000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 5 Then
                        DatosCuenta = String.Format("00000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 6 Then
                        DatosCuenta = String.Format("0000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 7 Then
                        DatosCuenta = String.Format("000000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 8 Then
                        DatosCuenta = String.Format("00000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 9 Then
                        DatosCuenta = String.Format("0000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 10 Then
                        DatosCuenta = String.Format("000000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 11 Then
                        DatosCuenta = String.Format("00000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 12 Then
                        DatosCuenta = String.Format("0000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 13 Then
                        DatosCuenta = String.Format("000{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 14 Then
                        DatosCuenta = String.Format("00{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 15 Then
                        DatosCuenta = String.Format("0{0}", DatosCuenta)
                    ElseIf DatosCuenta.Length = 16 Then
                        DatosCuenta = String.Format("{0}", DatosCuenta)
                    End If

                    RegistrarPagos.NumCuenta_Ordenante = DatosCuenta
                End If

            ElseIf l_Transferencia.Contains(model.MetodoPago) Then

                Dim MetodoPagoSat = Await (From n In db.Catalogo_FormasPagoSAT Where n.Descripcion = "03 - Transferencia Electrónica de Fondos").FirstOrDefaultAsync()

                If IsNothing(MetodoPagoSat) Then
                    Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la forma de pago SAT. Verifique su existencia."))
                    Return RedirectToAction("GenerarPagos")
                Else

                    Dim BancoOrdenante = Await (From n In db.Catalogo_Bancos Where n.idCatalogoBanco = model.BancoOrdenante).FirstOrDefaultAsync()

                    If IsNothing(BancoOrdenante) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco ordenante, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoBeneficiario = Await (From n In db.Catalogo_CuentasBancarias Where n.Catalogo_MetodosPago.NS_InternalID = model.MetodoPago).FirstOrDefaultAsync()

                    If IsNothing(BancoBeneficiario) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco beneficiario, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    RegistrarPagos.MetodoPagoSAT = MetodoPagoSat.NS_InternalID
                    RegistrarPagos.RFCBanco_Beneficiario = BancoBeneficiario.Catalogo_Bancos.RFC
                    RegistrarPagos.NumCuenta_Beneficiario = BancoBeneficiario.NumCuenta
                    RegistrarPagos.NS_ID_BancoBeneficiario = BancoBeneficiario.Catalogo_Bancos.NS_InternalID
                    RegistrarPagos.RFCBanco_Ordenante = BancoOrdenante.RFC
                    RegistrarPagos.NumCuenta_Ordenante = model.NumCuentaOrdenante.Trim()
                    RegistrarPagos.NS_ID_BancoOrdenante = BancoOrdenante.NS_InternalID
                    RegistrarPagos.NumTransaccion = model.NoRefPago
                    RegistrarPagos.TipoMetodoPago = "Transferencia"

                End If

            ElseIf l_Cheque.Contains(model.MetodoPago) Then

                Dim MetodoPagoSat = Await (From n In db.Catalogo_FormasPagoSAT Where n.Descripcion = "02 - Cheque Nominativo").FirstOrDefaultAsync()

                If IsNothing(MetodoPagoSat) Then
                    Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar la forma de pago SAT. Verifique su existencia."))
                    Return RedirectToAction("GenerarPagos")
                Else

                    Dim BancoOrdenante = Await (From n In db.Catalogo_Bancos Where n.idCatalogoBanco = model.BancoOrdenante).FirstOrDefaultAsync()

                    If IsNothing(BancoOrdenante) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco ordenante, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    Dim BancoBeneficiario = Await (From n In db.Catalogo_CuentasBancarias Where n.Catalogo_MetodosPago.NS_InternalID = model.MetodoPago).FirstOrDefaultAsync()

                    If IsNothing(BancoBeneficiario) Then
                        Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", "No se pudo localizar el banco beneficiario, verifique su existencia."))
                        Return RedirectToAction("GenerarPagos")
                    End If

                    RegistrarPagos.MetodoPagoSAT = MetodoPagoSat.NS_InternalID
                    RegistrarPagos.RFCBanco_Beneficiario = BancoBeneficiario.Catalogo_Bancos.RFC
                    RegistrarPagos.NumCuenta_Beneficiario = BancoBeneficiario.NumCuenta
                    RegistrarPagos.NS_ID_BancoBeneficiario = BancoBeneficiario.Catalogo_Bancos.NS_InternalID
                    RegistrarPagos.RFCBanco_Ordenante = BancoOrdenante.RFC
                    RegistrarPagos.NS_ID_BancoOrdenante = BancoOrdenante.NS_InternalID
                    RegistrarPagos.NumTransaccion = model.NoRefPago
                    RegistrarPagos.TipoMetodoPago = "Cheque"

                    'Dim DatosCuenta = model.NumCuentaOrdenante.Trim()

                    'If DatosCuenta.Length = 4 Then
                    '    DatosCuenta = String.Format("000000000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 5 Then
                    '    DatosCuenta = String.Format("00000000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 6 Then
                    '    DatosCuenta = String.Format("0000000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 7 Then
                    '    DatosCuenta = String.Format("000000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 8 Then
                    '    DatosCuenta = String.Format("00000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 9 Then
                    '    DatosCuenta = String.Format("0000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 10 Then
                    '    DatosCuenta = String.Format("000000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 11 Then
                    '    DatosCuenta = String.Format("00000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 12 Then
                    '    DatosCuenta = String.Format("0000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 13 Then
                    '    DatosCuenta = String.Format("000{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 14 Then
                    '    DatosCuenta = String.Format("00{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 15 Then
                    '    DatosCuenta = String.Format("0{0}", DatosCuenta)
                    'ElseIf DatosCuenta.Length = 16 Then
                    '    DatosCuenta = String.Format("{0}", DatosCuenta)
                    'End If

                    RegistrarPagos.NumCuenta_Ordenante = model.NumCuentaOrdenante.Trim()

                End If
            End If

            For Each regPagosDetalle In Pagos
                Dim NuevoDetallePago As New DetalleAltaPagos

                NuevoDetallePago.InternalID_Factura = regPagosDetalle.NS_InternalID
                NuevoDetallePago.Monto = Convert.ToDouble(regPagosDetalle.ImporteAplicado)
                NuevoDetallePago.TipoPago = H_Invoice.GetTermsSAT(regPagosDetalle.NS_InternalID)
                l_DetallePago.Add(NuevoDetallePago)
            Next

            RegistrarPagos.DetallePagos = l_DetallePago

            If l_DetallePago.First.TipoPago = "4" Then
                RegistrarPagos.IndicadorPlantilla = "True"
            Else
                RegistrarPagos.IndicadorPlantilla = "False"
            End If

#End Region

#Region "Envio de Datos a NetSuite"
            ''Mandar Información a NetSuite
            Respuesta = PagosHelper.InsertCustomerPayment(RegistrarPagos)
#End Region

#Region "Validacion de Respuesta y alta en base de datos"
            If Respuesta.Estatus = "Exito" Then

                ''Debido a un error de conversion, este valor no se puede usar directamente, para este caso
                ''es necesario convertirlo en string y posteriormente usarlo para mapear los campos necesarios
                Dim ConvertRespuesta = Convert.ToString(Respuesta.Valor.internalId)

                ''Consultamos el TanID para almacenarlo como el external ID
                Dim ConsultarPago = PagosHelper.GetCustomerPaymentByID(ConvertRespuesta)

                Using context As New PVO_NetsuiteEntities
                    context.Configuration.AutoDetectChangesEnabled = False
                    context.Configuration.ValidateOnSaveEnabled = False

                    Dim RegistrarAplicacionPago As New AplicacionPago
                    'Dim l_DetalleReg As New List(Of DetalleAplicacionPago)

                    RegistrarAplicacionPago.NS_InternalID = ConvertRespuesta
                    RegistrarAplicacionPago.NS_ExternalID = ConsultarPago.tranId
                    RegistrarAplicacionPago.Fecha = Date.Now
                    RegistrarAplicacionPago.MontoTotalPagado = l_DetallePago.Sum(Function(x) x.Monto)
                    RegistrarAplicacionPago.Memo = model.Nota
                    RegistrarAplicacionPago.idUsuario = WebSecurity.CurrentUserId
                    RegistrarAplicacionPago.idMetodoPago = (From n In context.Catalogo_MetodosPago Where n.NS_InternalID = model.MetodoPago Select n.idMetodoPago).FirstOrDefault
                    RegistrarAplicacionPago.idCustomer = (From n In context.Customers Where n.NS_InternalID = model.Customer Select n.idCustomer).FirstOrDefault()

                    'For Each RegistrarMerc In Pagos
                    '    Dim RegMerc As New DetalleAplicacionPago

                    '    ''Obtener Factura Pago
                    '    Dim FacturaDetalle = (From n In db.Invoice_SO Where n.NS_InternalID = RegistrarMerc.NS_InternalID).FirstOrDefault()

                    '    RegMerc.Ref_Pago = FacturaDetalle.NS_ExternalID
                    '    RegMerc.ImporteOriginal = FacturaDetalle.Total
                    '    RegMerc.PagoRealizado = RegistrarMerc.ImporteAplicado
                    '    RegMerc.Moneda = "MXN"
                    '    RegMerc.idInvoice = FacturaDetalle.idInvoice

                    '    l_DetalleReg.Add(RegMerc)
                    'Next

                    'RegistrarAplicacionPago.DetalleAplicacionPago = l_DetalleReg
                    context.AplicacionPago.Add(RegistrarAplicacionPago)
                    context.SaveChanges()

                End Using


#Region "Sincronizar Información Netsuite"
                ''Sincronizamos la información de las facturas con la información en NetSuite
                'For Each ActualizaSaldosFactura In Pagos
                '    Using context As New PVO_NetsuiteEntities
                '        context.Configuration.AutoDetectChangesEnabled = False
                '        context.Configuration.ValidateOnSaveEnabled = False

                '        Dim RegMerc As New DetalleAplicacionPago

                '        ''Obtener Factura Pago
                '        Dim FacturaDetalle = Await (From n In context.Invoice_SO Where n.NS_InternalID = ActualizaSaldosFactura.NS_InternalID).FirstOrDefaultAsync()

                '        Dim SaldoPendiente = FacturaHelper.GetBalancePaidInvoice(ActualizaSaldosFactura.NS_InternalID)
                '        FacturaDetalle.ImporteAdeudado = SaldoPendiente

                '        If SaldoPendiente = 0.00 Then
                '            FacturaDetalle.idEstatus = (From n In context.Estatus Where n.ClaveInterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                '        End If
                '        context.SaveChanges()
                '    End Using

                'Next
#End Region

                If model.MetodoPago = "4" Or model.MetodoPago = "5" Then
                    Flash.Instance.Success(String.Format("El pago se ha registrado, pero se requiere aprobación. Folio de Pago: " + ConsultarPago.tranId))
                Else
                    Flash.Instance.Success(String.Format("Se ha efectuado el pago con exito! Folio de Pago: " + ConsultarPago.tranId))
                End If

                Return RedirectToAction("GenerarPagos")
            Else
                Flash.Instance.Error(String.Format("Hubo un problema al registrar el pago. Detalles: {0}", Respuesta.Mensaje))
                Return RedirectToAction("GenerarPagos")
            End If
#End Region

        End Function

        <HttpPost>
        <ValidateAntiForgeryToken()>
        Public Function ConsultarPagosAplicados(ByVal model As ConsultaSalesOrderViewModel) As JsonResult

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

                Dim BuscarDatosMRP As New BusquedaCorteCaja

                BuscarDatosMRP.FechaDesde = model.FechaInicio
                BuscarDatosMRP.FechaHasta = model.FechaFin
                BuscarDatosMRP.Location_ID = ValidaUsuario.Ubicaciones.NS_InternalID
                BuscarDatosMRP.Customer = model.Customers

                Dim Datos As RespuestaServicios = H_Search.GetListPaymentApplied(BuscarDatosMRP)

                If Datos.Estatus = "Exito" Then
                    l_respuesta = Datos.Valor
                    Dim l_metodoPagos As List(Of Catalogo_FormasPagoSAT) = db.Catalogo_FormasPagoSAT.ToList()

                    For Each RegVentas In l_respuesta

                        RegVentas.MetodoPago = (From n In l_metodoPagos Where n.NS_InternalID = RegVentas.MetodoPago Select n.Descripcion).FirstOrDefault()
                        RegVentas.fecha = RegVentas.FechaEmision.ToString("dd/MM/yyyy")

                    Next

                End If

            End If

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_respuesta))

        End Function

        Function ImprimirComprobantePago(ByVal id As String)

            If WebSecurity.IsAuthenticated Then

                Dim ValidaMostrador = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId Select n.MostradorPref).FirstOrDefault()

                If IsNothing(ValidaMostrador) Then
                    Flash.Instance.Error("Para poder usar esta opción, debe seleccionar un mostrador.")
                    Return RedirectToAction("Index", "Home")
                End If

                Dim GenerarImpresion = H_Servlet.ConnectionServiceServletPDF(id)

                If GenerarImpresion.Estatus = "Exito" Then
                    Dim NombreArchivo = String.Format("Comprobante_Pago_{0}.pdf", id)
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

        Public Function ActualizarBalanceFacturas(ByVal id As String) As List(Of Invoice_SO)

            Dim l_invoice = (From n In db.Invoice_SO Where n.idCustomer = id).ToList()

            For Each ActualizaBalance In l_invoice
                Try
                    Dim SaldoPendiente As BalanceTerms = H_Invoice.GetBalancePaidInvoiceandTerms(ActualizaBalance.NS_InternalID)

                    ActualizaBalance.idMetodoPagoSAT = (From n In db.Catalogo_MetodoPagoSAT Where n.NS_InternalID = SaldoPendiente.ns_terms Select n.idMetodoPagoSAT).FirstOrDefault()

                    If SaldoPendiente.amount <> ActualizaBalance.ImporteAdeudado Then
                        ActualizaBalance.ImporteAdeudado = SaldoPendiente.amount

                        If SaldoPendiente.amount = 0.00 Then
                            ActualizaBalance.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                        Else
                            ActualizaBalance.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "INV_Generada" Select n.idEstatus).FirstOrDefault()
                        End If
                    ElseIf SaldoPendiente.amount = 0.00 Then
                        ActualizaBalance.idEstatus = (From n In db.Estatus Where n.ClaveExterna = "INV_Pagado" Select n.idEstatus).FirstOrDefault()
                    End If

                    db.SaveChanges()
                Catch ex As Exception
                    Continue For
                End Try
            Next

            Return l_invoice
        End Function

        Public Function GetFacturasClientes(ByVal id As String) As List(Of MapFacturasPendientesPago)

            Dim l_Facturas As New List(Of MapFacturasPendientesPago)
            Dim l_MetodosPagoSAT As List(Of Catalogo_MetodoPagoSAT) = db.Catalogo_MetodoPagoSAT.ToList()

            Dim ConsultarFacturasPendientes As RespuestaServicios = H_Search.GetListInvoiceForCustomerPendingPay(id)

            If ConsultarFacturasPendientes.Estatus = "Exito" Then

                For Each VerificarExistenciaFactura As MapFacturasPendientesPago In ConsultarFacturasPendientes.Valor
                    Try
                        'Dim ValidaFactura = (From n In db.Invoice_SO Where n.NS_InternalID = VerificarExistenciaFactura.NS_InternalID).FirstOrDefault()

                        'If IsNothing(ValidaFactura) Then
                        '    Dim result = RecuperarFacturaPendiente(VerificarExistenciaFactura.NS_InternalID)
                        'End If

                        If (VerificarExistenciaFactura.MetodoPagoSAT = "4") Then
                            VerificarExistenciaFactura.MetodoPagoSAT = "PPD"
                        Else
                            VerificarExistenciaFactura.MetodoPagoSAT = "PUE"
                        End If

                        VerificarExistenciaFactura.Fecha = VerificarExistenciaFactura.FechaCreated.ToString("dd/MM/yyyy")
                        'VerificarExistenciaFactura.Fecha_Venci = VerificarExistenciaFactura.Fecha_Vencimiento.ToString("dd/MM/yyyy")
                    Catch ex As Exception
                        Continue For
                    End Try
                Next

            End If

            l_Facturas = ConsultarFacturasPendientes.Valor

            Return l_Facturas.OrderByDescending(Function(x) x.FechaCreated).ToList()
        End Function

        <HttpPost>
        Public Function DetallePagos(ByVal ns_id As String) As JsonResult
            Try

                Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()

                If IsNothing(ValidaUsuario.MostradorPref) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Para poder usar esta opción, es necesario escoger un mostrador. Para escogerlo, puede dar click en el icono del engranaje y seleccionar un mostrador.", ""))
                Else
                    Dim GetCustomerPayment As NetsuiteLibrary.SuiteTalk.CustomerPayment = H_Payment.GetCustomerPaymentByID(ns_id)

                    If IsNothing(GetCustomerPayment) Then
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar este pago.", ""))

                    ElseIf IsNothing(GetCustomerPayment.applyList) Then
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El pago no posee conceptos disponibles.", ""))

                    Else

                        If ValidaUsuario.Ubicaciones.NS_InternalID <> GetCustomerPayment.location.internalId Then
                            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Este pago pertenece a un mostrador diferente al seleccionado. Verifique su mostrador.", ""))
                        End If

                        Dim MapPagosDetalle As New DetallesPagoViewMap
                        Dim l_DetalleConceptos As New List(Of DetallesConceptoPagoViewMap)

                        MapPagosDetalle.FolioTransaccion = GetCustomerPayment.tranId
                        MapPagosDetalle.FechaCreacion = GetCustomerPayment.createdDate.ToString("dd/MM/yyyy")
                        MapPagosDetalle.Cliente = GetCustomerPayment.customer.name
                        MapPagosDetalle.NotaPago = GetCustomerPayment.memo

                        Dim Campocustom As List(Of CustomFieldRef) = (From n In GetCustomerPayment.customFieldList Where n.scriptId = "custbody_mx_cfdi_uuid").ToList()

                        If Campocustom.Count <> 0 Then
                            MapPagosDetalle.Timbrado = "True"

                            For Each CamposFiscales In GetCustomerPayment.customFieldList

                                If CamposFiscales.scriptId = "custbody_mx_cfdi_uuid" Then
                                    Dim ValorRef As StringCustomFieldRef = CType(CamposFiscales, StringCustomFieldRef)
                                    MapPagosDetalle.UUID = ValorRef.value

                                ElseIf CamposFiscales.scriptId = "custbody_mx_cfdi_folio" Then
                                    Dim ValorRef As StringCustomFieldRef = CType(CamposFiscales, StringCustomFieldRef)
                                    MapPagosDetalle.Folio_Timbrado = ValorRef.value

                                ElseIf CamposFiscales.scriptId = "custbody_mx_cfdi_serie" Then
                                    Dim ValorRef As StringCustomFieldRef = CType(CamposFiscales, StringCustomFieldRef)
                                    MapPagosDetalle.Serie_Timbrado = ValorRef.value

                                ElseIf CamposFiscales.scriptId = "custbody_mx_cfdi_certify_timestamp" Then
                                    Dim ValorRef As StringCustomFieldRef = CType(CamposFiscales, StringCustomFieldRef)
                                    MapPagosDetalle.FechaTimbrado = ValorRef.value

                                ElseIf CamposFiscales.scriptId = "custbody_edoc_generated_pdf" Then
                                    Dim ValorRef As SelectCustomFieldRef = CType(CamposFiscales, SelectCustomFieldRef)
                                    MapPagosDetalle.NS_ID_pdf_pago = ValorRef.value.internalId

                                ElseIf CamposFiscales.scriptId = "custbody_psg_ei_certified_edoc" Then
                                    Dim ValorRef As SelectCustomFieldRef = CType(CamposFiscales, SelectCustomFieldRef)
                                    MapPagosDetalle.NS_ID_xml_pago = ValorRef.value.internalId
                                End If
                            Next

                        Else
                            MapPagosDetalle.Timbrado = "False"
                        End If

                        For Each RegistrarPago As CustomerPaymentApply In GetCustomerPayment.applyList.apply

                            If RegistrarPago.apply = True Then
                                Dim RegPagoAplicado As New DetallesConceptoPagoViewMap

                                RegPagoAplicado.FolioFactura = RegistrarPago.refNum
                                RegPagoAplicado.ImporteOriginal = RegistrarPago.total
                                RegPagoAplicado.ImportePagar = RegistrarPago.due
                                RegPagoAplicado.PagoAplicado = RegistrarPago.amount
                                'RegPagoAplicado.ImportePagar = RegistrarPago.

                                l_DetalleConceptos.Add(RegPagoAplicado)
                            End If

                        Next

                        MapPagosDetalle.l_DetallePago = l_DetalleConceptos

                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", MapPagosDetalle))
                    End If

                End If

            Catch ex As Exception
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message, ""))
            End Try
        End Function

        Public Async Function RecuperarFacturaPendiente(ByVal ns_id As String) As Task(Of RespuestaControlGeneral)

            If ns_id = "" Then
                Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La Factura para recuperar, no tiene un identificativo valido.")
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

                Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "La factura ya existe en el PVO.", New With {.NS_ID = ns_id})

            Else
                Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La factura ya existe en el PVO.")
            End If

        End Function

#End Region


        Public Async Function GeneraViewbags() As Threading.Tasks.Task
            Dim Select_Customer As New List(Of SelectListItem)
            Dim Select_MetodoPago As New List(Of SelectListItem)
            Dim Select_Bancos As New List(Of SelectListItem)
            Dim Select_Tarjetas As New List(Of SelectListItem)
            Dim l_MetodosValidos As New List(Of String)

            'Dim l_Customers = Await Consulta.ConsultarListaCustomers()
            Dim l_MetodosPago = Await Consulta.ConsultaListaMetodosPago()
            Dim l_Bancos = Await Consulta.ConsultaBancos()
            Dim l_customers_test As List(Of MapClientes) = H_Search.GetListCustomersViewBag()

            'For Each RegistroCustomer In l_Customers
            '    Dim nombreCliente = RegistroCustomer.NS_ExternalID + " - " + RegistroCustomer.Nombre
            '    Select_Customer.Add(New SelectListItem With {.Text = nombreCliente, .Value = RegistroCustomer.NS_InternalID})
            'Next

            For Each RegistroMetodoPago In l_MetodosPago
                Select_MetodoPago.Add(New SelectListItem With {.Text = RegistroMetodoPago.Descripcion, .Value = RegistroMetodoPago.NS_InternalID})
            Next

            For Each RegistroBanco In l_Bancos
                Select_Bancos.Add(New SelectListItem With {.Text = RegistroBanco.Nombre, .Value = RegistroBanco.idCatalogoBanco})
            Next

            For Each RegistroCustomerTest In l_customers_test
                Dim nombreCliente = RegistroCustomerTest.ExternalID + " - " + RegistroCustomerTest.Nombre
                Select_Customer.Add(New SelectListItem With {.Text = nombreCliente, .Value = RegistroCustomerTest.InternalID})
            Next

            Dim l_TarjetaDebito = db.JG_CoincidenciaMetodoPago("Tarjeta Debito")
            Dim l_TarjetaCredito = db.JG_CoincidenciaMetodoPago("Tarjeta Credito")
            Dim l_Transferencia = db.JG_CoincidenciaMetodoPago("Transferencia")
            Dim l_Cheque = db.JG_CoincidenciaMetodoPago("CHEQUE")

            For Each tarjetaDebito In l_TarjetaDebito
                Dim valor = tarjetaDebito.ToString
                l_MetodosValidos.Add(valor)
            Next

            For Each tarjetaCredito In l_TarjetaCredito
                Dim valor = tarjetaCredito.ToString
                l_MetodosValidos.Add(valor)
            Next

            For Each tranferencia In l_Transferencia
                Dim valor = tranferencia.ToString
                l_MetodosValidos.Add(valor)
            Next

            For Each cheque In l_Cheque
                Dim valor = cheque.ToString
                l_MetodosValidos.Add(valor)
            Next

            ViewBag.Banco = Select_Bancos
            ViewBag.BancoOrdenante = Select_Bancos
            ViewBag.BancoBeneficiario = Select_Bancos
            ViewBag.Customer = Select_Customer
            ViewBag.MetodoPago = Select_MetodoPago
            ViewBag.TarjetasC_beneficiario = Select_Tarjetas
            ViewBag.MetodosValidos = JsonConvert.SerializeObject(l_MetodosValidos)

        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

    End Class
End Namespace
