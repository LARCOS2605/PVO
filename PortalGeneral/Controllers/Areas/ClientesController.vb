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
Imports AutoMapper
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports WebMatrix.WebData
Imports PortalGeneral.Controllers.Areas

'Namespace Controllers.Areas
<Authorize()>
<HandleError>
Public Class ClientesController
    Inherits AppBaseController

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
    Public Consulta As New ConsultasController
    Public Netsuite As New NetsuiteController
    Public H_Customer As New CustomerHelper
    Public H_Search As New SearchHelper
#End Region

#Region "Vistas"
    Public Async Function ConsultarClientes() As Task(Of ActionResult)

        Dim l_costumer As New List(Of MapClientes)
        Dim l_costumer_alt As New List(Of Customers_ALT)
        Dim l_tipoClientes As List(Of Catalogo_TipoCliente) = db.Catalogo_TipoCliente.ToList()
        Dim l_niveles As List(Of Catalogo_NivelesPrecio) = db.Catalogo_NivelesPrecio.ToList()
        Dim l_Terminos As List(Of Catalogo_Terminos) = db.Catalogo_Terminos.ToList()

        l_costumer = H_Search.GetListCustomersAdvancedVER()

        For Each RegClientes In l_costumer
            Dim Cliente As New Customers_ALT

            With Cliente
                .ClaveCliente = RegClientes.ExternalID
                .NombreCliente = RegClientes.Nombre
                .RFC = RegClientes.RFC
                .Limite_Credito = RegClientes.limite_Credito

                If RegClientes.Terminos = "ST" Then
                    .Terminos = "Sin Terminos"
                Else
                    .Terminos = (From n In l_Terminos Where n.NS_InternalID = RegClientes.Terminos Select n.Descripcion).FirstOrDefault()
                End If

                If RegClientes.Nivel_Precios = "SNP" Then
                    .Nivel_Descuento = "Sin Precio Asignado"
                Else
                    .Nivel_Descuento = (From n In l_niveles Where n.NS_InternalID = RegClientes.Nivel_Precios Select n.Descripcion).FirstOrDefault()
                End If

                If RegClientes.Estado = "Sin Estatus" Then
                    .tipo_cliente = RegClientes.Estado
                Else
                    .tipo_cliente = (From n In l_tipoClientes Where n.NS_InternalID = RegClientes.Estado Select n.Descripcion).FirstOrDefault()
                End If

                .dias_atraso = RegClientes.DiasVencimiento
            End With

            l_costumer_alt.Add(Cliente)

        Next

        ViewBag.Clientes = l_costumer_alt

        Return View()
    End Function
    Public Function CrearCliente() As ActionResult
        Return View()
    End Function
    Public Async Function CrearVistaCliente() As Task(Of ActionResult)
        Await GeneraViewbags()
        Return View()
    End Function

#End Region

#Region "Crear Cliente"
    <HttpPost()>
    Public Async Function CrearVistaCliente(ByVal Model As ClientesViewModel) As Task(Of ActionResult)
        Try
            Await GeneraViewbags()

            'If ModelState.IsValid Then

            'Dim respuesta As RespuestaControlGeneral
            Dim account As New AccountController
            Dim altacustom As New AltaCustomer

            Dim AltaCliente As New Customers

            AltaCliente.RFC = Model.Rfc.ToUpper

            'Dim Nombre = Model.Nombre
            'Dim Apellido = Model.Apellido
            'Dim NombreSat = Model.NombreSat
            'Dim RegimenFiscal = Model.RegimenFiscal
            'Dim Correo = Model.Correo
            'Dim PaisDom = Model.PaisDom
            'Dim EstadoDom = Model.EstadoDom
            'Dim MunicipioDom = Model.MunicipioDom
            'Dim ColoniaDom = Model.ColoniaDom
            'Dim CalleDom = Model.CalleDom
            'Dim NumExtDom = Model.NumExtDom
            'Dim CpDom = Model.CpDom
            'Dim Ubicacion = Model.Ubicacion
            'Dim Comentarios = Model.Comentarios
            'Dim PaisFis = Model.PaisFis
            'Dim Atencion = Model.Atencion
            'Dim Destinatario = Model.Destinatario
            'Dim CalleFis = Model.CalleFis
            'Dim NumExtFis = Model.NumExtFis
            'Dim Piso = Model.Piso
            'Dim NumIntFis = Model.NumIntFis
            'Dim ColoniaFis = Model.ColoniaFis
            'Dim MunicipioFis = Model.MunicipioFis
            'Dim CpFis = Model.CpFis
            'Dim CiudadFis = Model.CiudadFis
            'Dim EstadoFis = Model.EstadoFis
            'Dim Direc = Model.Direc

            'Mandar datos a netsuite

            Dim tipo = Model.PersonaFisica

            If (tipo = "true") Then
                altacustom.Nombre_Empresa = Model.Empresa
                AltaCliente.Nombre = Model.Empresa.ToUpper
                altacustom.Nombre_Sat = Model.Empresa
            ElseIf (tipo = "false") Then
                altacustom.Nombre = Model.Nombre
                altacustom.Apellido = Model.Apellido
                AltaCliente.Nombre = Model.Nombre.ToUpper + " " + Model.Apellido.ToUpper
                altacustom.Nombre_Sat = Model.Nombre + " " + Model.Apellido
                altacustom.Celular = Model.Celular
            End If

            Dim pago = Model.TipoPago
            If (pago = "true") Then
                'Contado
                altacustom.InternalidPago = "13"
            ElseIf (pago = "false") Then
                'Credito
                altacustom.InternalidPago = "6"
            End If

            altacustom.Rfc = Model.Rfc
            altacustom.InternalID_RegimenFiscal = Model.RegimenFiscal
            altacustom.Correo = Model.Correo
            altacustom.InternalId_Mostrador = Model.Ubicacion
            altacustom.Comentarios = Model.Comentarios
            altacustom.EstadoFis = Model.EstadoFis
            altacustom.CiudadFis = Model.CiudadFis
            altacustom.CpFis = Model.CpFis
            altacustom.Direc = Model.Direc
            altacustom.Telefono = Model.Telefono
            altacustom.Destinatario = Model.Destinatario
            altacustom.NumExtFis = Model.NumExtFis
            altacustom.CalleFis = Model.CalleFis
            altacustom.NumIntFis = Model.NumIntFis
            altacustom.Internalid_estado = Model.InternalId_Estado
            altacustom.PersonaFisica = Model.PersonaFisica
            altacustom.ColoniaFis = Model.ColoniaFis
            altacustom.MunicipioFis = Model.MunicipioFis

            Dim s As New CustomerHelper()

            Dim l = s.InsertCustomer(altacustom)

            If l.Estatus = "Exito" Then
                If (tipo = "true") Then 'Empresa
                    altacustom.Nombre_Empresa = Model.Empresa.ToUpper
                    Dim nuevo_contacto = s.InsertContact(l.Valor.internalId, Model.Empresa.ToUpper, Model.Correo)

                    If nuevo_contacto.Estatus = "Exito" Then
                        Dim Connection As New ConnectionServer

                        Dim r = Connection.ConnectionServiceServletCustList(l.Valor.internalId, nuevo_contacto.Valor.internalId)
                    Else
                        Flash.Instance.Error(String.Format("Hubo un problema al generar el contacto. Detalles {0}", nuevo_contacto.Mensaje))
                    End If
                End If
                Dim clienteInternal = s.GetCustomerById(l.Valor.internalId)
                AltaCliente.NS_InternalID = clienteInternal.internalId
                AltaCliente.NS_ExternalID = clienteInternal.entityId
                'Asignar el tipo de cliente a la tabla customer
                If Not IsNothing(clienteInternal.entityStatus) Then
                    Dim EstatusCliente = clienteInternal.entityStatus.internalId

                    AltaCliente.idCatalogoTipoCliente = (From n In db.Catalogo_TipoCliente Where n.NS_InternalID = EstatusCliente Select n.idCatalogoTipoCliente).FirstOrDefault()
                End If

                If (tipo = "true") Then
                    Flash.Instance.Success(String.Format("El Cliente {0} ha sido registrado correctamente.", Model.Empresa.ToUpper))
                Else
                    Flash.Instance.Success(String.Format("El Cliente {0} ha sido registrado correctamente.", Model.Nombre.ToUpper + " " + Model.Apellido.ToUpper))
                End If

                db.Customers.Add(AltaCliente)
                db.SaveChanges()
                Return RedirectToAction("ConsultarClientes")
            Else
                Flash.Instance.Error(String.Format("Hubo un problema al validar la estructura de los datos proporcionados. Detalles {0}", l.Mensaje))
                Return RedirectToAction("ConsultarClientes")
            End If

            'Else
            '        Flash.Instance.Error("Hubo un problema al validar la estructura de los datos proporcionados, intentelo nuevamente mas tarde.")
            '    Return View()
            'End If
        Catch ex As Exception
            Flash.Instance.Error("Hubo un problema al registrar el cliente. Detalles: " + ex.Message)
            Return View()
        End Try
    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionNuevoDomicilio(ByVal Model As GuardarDomicilioNuevoModel) As Task(Of JsonResult)
        Try
            Await GeneraViewbags()

            Dim AltaCp As New Catalogo_Cp
            Dim AltaColonia As New Catalogo_Colonias

            'Buscamos que no exista el codigo postal a ingresar
            Dim buscar_cp = (From n In db.Catalogo_Cp Where n.Cp = Model.CodigoPostal Select n.Cp).FirstOrDefault()

            If IsNothing(buscar_cp) Then
                'Datos Guardar Cp
                AltaCp.Cp = Model.CodigoPostal
                AltaCp.idCatalogoEstados = Model.Estado
                AltaCp.idCatalogoMunicipio = Model.Municipio

                db.Catalogo_Cp.Add(AltaCp)
                db.SaveChanges()
            End If

            'Buscamos que no exista la colonia a ingresar
            Dim buscar_colonia = (From n In db.Catalogo_Colonias Where n.CodigoPostal = Model.CodigoPostal And n.Descripcion = Model.Colonia Select n.Descripcion).FirstOrDefault()

            If IsNothing(buscar_colonia) Then
                'Datos Guardar Colonia
                AltaColonia.Clave = "NR"
                AltaColonia.CodigoPostal = Model.CodigoPostal
                AltaColonia.Descripcion = Model.Colonia.ToUpper

                db.Catalogo_Colonias.Add(AltaColonia)
                db.SaveChanges()
            End If
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al registrar el domicilio"))
        End Try
    End Function
#End Region

#Region "Procesos"
    <HttpPost>
    Public Async Function SincronizarClientesNetsuite() As Task(Of JsonResult)
        Try
            Dim Respuesta = Netsuite.SincronizarClientesExistentes()

            Dim l_Customers = Await Consulta.ConsultarListaCustomers()

            Dim l_CustomerViewModel As List(Of CustomerViewModel) = AutoMap.AutoMapperCustomer(l_Customers)

            l_CustomerViewModel = (From n In l_CustomerViewModel Order By n.idCustomer Descending).ToList()

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_CustomerViewModel))

        Catch ex As Exception
            Dim errors = ex.Message
        End Try
    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionCliente(ByVal CodigoPostal As String) As Task(Of JsonResult)

        Try

            Dim l_BuscarCp = Await Consulta.ConsultarCp(CodigoPostal)

            Dim l_BuscarCpViewModel As List(Of DomicilioClienteViewModel) = AutoMap.AutoMapperInformacionDomicilio(l_BuscarCp)


            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_BuscarCpViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar el Código Postal. Detalles: " + ex.Message))
        End Try

    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionFiscal(ByVal CodigoPostalFis As String) As Task(Of JsonResult)

        Try
            Dim l_BuscarCp = Await Consulta.ConsultarCpFiscal(CodigoPostalFis)

            Dim l_BuscarCpViewModel As List(Of DomicilioFiscalViewModel) = AutoMap.AutoMapperInformacionDomicilioFiscal(l_BuscarCp)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_BuscarCpViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar el Código Postal. Detalles: " + ex.Message))
        End Try

    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionColonia(ByVal CodigoPostalCol As String) As Task(Of JsonResult)

        Try
            Dim l_BuscarColonia = Await Consulta.ConsultarColonias(CodigoPostalCol)

            Dim l_BuscarCpViewModel As List(Of DomicilioColoniasDomViewModel) = AutoMap.AutoMapperInformacionColonias(l_BuscarColonia)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_BuscarCpViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar las colonias. Detalles: " + ex.Message))
        End Try

    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionNueva(ByVal idEstado As String) As Task(Of JsonResult)

        Try
            Dim l_BuscarMunicipio = Await Consulta.ConsultarMunicipio(idEstado)

            Dim l_BuscarCpViewModel As List(Of InfoNuevoClienteViewModel) = AutoMap.AutoMapperDomicilioNuevo(l_BuscarMunicipio)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_BuscarCpViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar los municipios. Detalles: " + ex.Message))
        End Try

    End Function
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function BuscarInformacionColoniaFis(ByVal CodigoPostalColFis As String) As Task(Of JsonResult)

        Try
            Dim l_BuscarColonia = Await Consulta.ConsultarColoniasFis(CodigoPostalColFis)

            Dim l_BuscarCpViewModel As List(Of DomicilioColoniasFisViewModel) = AutoMap.AutoMapperInformacionColoniasFis(l_BuscarColonia)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_BuscarCpViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar las colonias. Detalles: " + ex.Message))
        End Try

    End Function
    <HttpPost>
        Public Async Function SincronizarClientesEspecifico(ByVal idCliente As String) As Task(Of JsonResult)
            Try
                Dim Respuesta = Netsuite.SincronizarCliente(idCliente.Trim().ToUpper())

                Dim l_Customers = Await Consulta.ConsultarListaCustomers()

                Dim l_CustomerViewModel As List(Of CustomerViewModel) = AutoMap.AutoMapperCustomer(l_Customers)

                l_CustomerViewModel = (From n In l_CustomerViewModel Order By n.idCustomer Descending).ToList()

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_CustomerViewModel))

            Catch ex As Exception
                Dim errors = ex.Message
            End Try
        End Function

#End Region

#Region "Funciones auxiliares"
    Public Async Function GeneraViewbags() As Threading.Tasks.Task
        Dim Select_Idiomas As New List(Of SelectListItem)
        Dim Select_RegimenFiscal As New List(Of SelectListItem)
        Dim Select_Ubicaciones As New List(Of SelectListItem)
        Dim Select_Paises As New List(Of SelectListItem)
        Dim Select_Estados As New List(Of SelectListItem)
        Dim Select_Colonias As New List(Of SelectListItem)


        Dim l_Idiomas = Await Consulta.ConsultarIdiomas()

        For Each RegistroIdiomas In l_Idiomas
            Select_Idiomas.Add(New SelectListItem With {.Text = RegistroIdiomas.Descripcion, .Value = RegistroIdiomas.Clave})
        Next

        Dim l_Ubicaciones = Await Consulta.ConsultarUbicacionesMostrador()

        For Each RegistroUbicaciones In l_Ubicaciones
            Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.Descripcion, .Value = RegistroUbicaciones.NS_InternalID})
        Next

        Dim l_regimenfiscal = Await Consulta.ConsultarRegimenFiscal()

        For Each RegistroFiscal In l_regimenfiscal
            Select_RegimenFiscal.Add(New SelectListItem With {.Text = RegistroFiscal.Descripcion, .Value = RegistroFiscal.NS_InternalID})
        Next

        Dim l_paises = Await Consulta.ConsultarPaises()

        For Each Paises In l_paises
            Select_Paises.Add(New SelectListItem With {.Text = Paises.Descripcion, .Value = Paises.NS_ExternalID})
        Next

        Dim l_estados = Await Consulta.ConsultarEstados()

        For Each Estados In l_estados
            Select_Estados.Add(New SelectListItem With {.Text = Estados.Descripcion, .Value = Estados.idCatalogoEstados})
        Next

        Dim l_colonias = Await Consulta.ConsultarColonias()

        For Each Colonias In l_colonias
            Select_Colonias.Add(New SelectListItem With {.Text = Colonias.Clave + " - " + Colonias.Descripcion, .Value = Colonias.idCatalogoColonia})
        Next

        ViewBag.Lenguaje = Select_Idiomas
        ViewBag.RegimenFiscal = Select_RegimenFiscal
        ViewBag.Ubicacion = Select_Ubicaciones
        ViewBag.Paises = Select_Paises
        ViewBag.Estados = Select_Estados
        ViewBag.ColoniaDom = Select_Colonias
        ViewBag.ColoniaFis = Select_Colonias
        ViewBag.l_estados = l_estados
        ViewBag.Roles = Await Consulta.ConsultarRoles()
    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If (disposing) Then
            db.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
#End Region
End Class
'End Namespace

