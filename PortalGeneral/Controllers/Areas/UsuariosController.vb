Imports AutoMapper
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports System.Data.Entity
Imports System.Threading.Tasks
Imports WebMatrix.WebData

<Authorize()>
<HandleError>
Public Class UsuariosController
    Inherits AppBaseController

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
    Public Consulta As New ConsultasController
    Public CapaCorreo As New CapaCorreosController

#End Region

#Region "Ventanas Consulta"
    Public Async Function ConsultarUsuarios() As Task(Of ActionResult)

        ViewBag.Lenguaje = Await Consulta.ConsultarIdiomas()
        ViewBag.Roles = Await Consulta.ConsultarRoles()
        ViewBag.valor = Await Consulta.ConsultarUsuarios()


        Return View()
    End Function

    Public Function CrarQR() As ActionResult
        Dim Argumentos As String

        Argumentos = "BEGIN:VCARD" & vbLf
        Argumentos += "VERSION:3.0" & vbLf
        Argumentos += "N:Garcia;Sanchez" & vbLf
        Argumentos += "FN:Jeus García" & vbLf
        Argumentos += "ORG:EVenX" & vbLf
        Argumentos += "URL:Verquet.com" & vbLf
        Argumentos += "EMAIL:SOME@EMAIL.COM" & vbLf
        Argumentos += "TEL;TYPE=Fijo:+49 1234 56788" & vbLf
        Argumentos += "TEL;TYPE=Trabajo:+49 2443 22443" & vbLf
        Argumentos += "ADR;TYPE=intl,work,postal,parcel:;;Wallstr. 1;Tehran;;12345;Iran" & vbLf
        Argumentos += "END:VCARD" & vbLf

        ModuleQRCode.QR(Argumentos)


    End Function

    Public Async Function CrearUsuario() As Task(Of ActionResult)

        Await GeneraViewbags()

        Return View()
    End Function

    Public Async Function EditarUsuario(Optional ByVal id As Integer = Nothing) As Task(Of ActionResult)

        Dim user As Usuarios = (From n In db.Usuarios Where n.idUsuario = id).FirstOrDefault()

        If IsNothing(user) Then
            Flash.Instance.Error("No se pudo localizar el usuario.")
            Return RedirectToAction("ConsultarUsuarios", "Usuarios")
        End If

        Dim model As New UsuariosViewModel
        model.Nombre = user.Nombre
        model.Correo = user.Correo
        model.InformacionContacto = user.InformacionContacto
        model.Lenguaje = user.Lenguaje

        Dim Select_Idiomas As New List(Of SelectListItem)
        Dim Select_Ubicaciones As New List(Of SelectListItem)

        ''Validamos los mostradores preferentes (superadmin, Ventedor Mostrador, Cajero)
        Dim l_Ubicaciones = Await Consulta.ConsultarUbicacionesAlmacen()
        For Each RegistroUbicaciones In l_Ubicaciones
            If Not IsNothing(user.MostradorPref) Then
                If user.MostradorPref = RegistroUbicaciones.idUbicacion Then
                    Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion, .Selected = True})
                Else
                    Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion, .Selected = False})
                End If
            Else
                Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion, .Selected = False})
            End If
        Next

        Dim Sucursales As New List(Of String)
        For Each RegMostrador In user.UsuarioUbicaciones
            Sucursales.Add(RegMostrador.idUbicacion.ToString())
        Next

        ''Validamos los mostradores Regionales
        Dim Select_Ubicaciones_Reg As New List(Of SelectListItem)
        For Each RegistroUbicaciones In l_Ubicaciones
            If Sucursales.Contains(RegistroUbicaciones.idUbicacion.ToString()) Then
                Select_Ubicaciones_Reg.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion, .Selected = True})
            Else
                Select_Ubicaciones_Reg.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion, .Selected = False})
            End If
        Next

        ViewBag.Lenguaje = Select_Idiomas
        ViewBag.Ubicacion = Select_Ubicaciones
        ViewBag.Ubicacion_Reg = Select_Ubicaciones_Reg
        ViewBag.rolesUser = user.webpages_Roles.ToArray
        ViewBag.Sucursales = Sucursales
        ViewBag.RolSeleccionado = user.webpages_Roles.First.RoleName
        ViewBag.Lenguaje = New SelectList(New List(Of SelectListItem)() From {
    New SelectListItem() With {
         .Selected = True,
         .Text = "Español",
         .Value = "es"
    },
    New SelectListItem() With {
         .Selected = False,
         .Text = "English",
         .Value = "en"
    }
}, "Value", "Text", model.Lenguaje)
        ViewBag.Roles = Await Consulta.ConsultarRoles()
        ViewBag.valor = Await Consulta.ConsultarUsuarios()
        Return View(model)

    End Function
#End Region

#Region "Procesos Usuarios"

#Region "Editar Usuario"
    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function EditarUsuario(ByVal id As Integer, ByVal model As UsuariosViewModel, ByVal l_Roles As String()) As Task(Of ActionResult)

        Dim user As Usuarios = (From n In db.Usuarios Where n.idUsuario = id).FirstOrDefault()
        Dim datosValidos As Boolean = True
        Dim actualizado As Integer

        If ModelState.IsValid Then
            If l_Roles Is Nothing OrElse l_Roles.Count = 0 Then
                Flash.Instance.Error("Debe seleccionar por lo menos un perfil.")
                l_Roles = {}
                datosValidos = False
            End If
        End If

        If datosValidos Then
            If user.webpages_Roles.Count > 0 Then
                Roles.RemoveUserFromRoles(user.Correo, user.webpages_Roles.Select(Function(r) r.RoleName).ToArray)
            End If

            Roles.AddUserToRoles(user.Correo, l_Roles)

            ''Removemos las localizaciones previas
            Dim l_datos = Await (From n In db.UsuarioUbicaciones Where user.idUsuario = n.UserId).ToListAsync()

            If l_datos.Count <> 0 Then
                db.UsuarioUbicaciones.RemoveRange(l_datos)
                db.SaveChanges()
            End If



            If l_Roles.First = "Regional" Then
                Dim stringToLines As String() = model.Ubicacion_Select.Split("|")

                For Each addUbicaciones In stringToLines

                    If Not addUbicaciones = "" Then
                        Dim AltaUbicacion As New UsuarioUbicaciones
                        AltaUbicacion.UserId = id
                        AltaUbicacion.idUbicacion = addUbicaciones

                        db.UsuarioUbicaciones.Add(AltaUbicacion)
                        db.SaveChanges()
                    End If

                Next
            ElseIf l_Roles.First = "Cajero" OrElse l_Roles.First = "Vendedor" Then

                Dim UsuarioAltaMostrador = Await (From n In db.Usuarios Where n.idUsuario = id).FirstOrDefaultAsync()
                UsuarioAltaMostrador.MostradorPref = model.Ubicacion

                db.SaveChanges()
            End If

            If user.Correo.ToUpper <> model.Correo.ToUpper And WebSecurity.UserExists(model.Correo) Then
                ModelState.AddModelError("", ErrorCodeToString(MembershipCreateStatus.DuplicateUserName))
            Else
                user.Correo = model.Correo
            End If
            Try
                user.Nombre = model.Nombre
                user.Correo = model.Correo
                user.InformacionContacto = model.InformacionContacto
                user.Lenguaje = model.Lenguaje

                If user.Correo.Trim <> user.Correo.Trim Then
                    user.FechaModificacion = Now
                    user.InfoCambios = String.Format("Cambio de correo {0} por {1} , por el usuario {2}.", user.Correo, model.Correo, WebSecurity.CurrentUserName)

                ElseIf user.Nombre.Trim <> user.Nombre.Trim Then
                    user.FechaModificacion = Now
                    user.InfoCambios = String.Format("Cambio de nombre {0} por {1} , por el usuario {2}.", user.Nombre, model.Nombre, WebSecurity.CurrentUserName)
                End If

                actualizado = db.SaveChanges()
            Catch ex As Exception
                Dim sqlException = TryCast(ex.InnerException.InnerException, SqlClient.SqlException)
                If sqlException.Number = 2627 Then
                    Flash.Instance.Warning(String.Format("Ya existe un nombre de usuario para esa dirección de correo electrónico. Escriba una dirección de correo electrónico diferente a : {0}", model.Correo))
                End If
                Flash.Instance.Warning("No se pudo actualizar la información del usuario {0} , intente nuevamente.".StringFormat(model.Nombre))
            End Try

            If actualizado > 0 Then
                Flash.Instance.Success(String.Format("Se ha actualizado la información del usuario {0}.", model.Nombre))
            End If

            If WebSecurity.CurrentUserId = user.idUsuario Then
                WebSecurity.Logout()
                Return RedirectToAction("Index", "Home")
            Else
                Return RedirectToAction("ConsultarUsuarios")
            End If
        End If

        Return View(model)

    End Function
#End Region

#Region "Crear Usuario"
    <HttpPost()>
    Public Async Function CrearUsuario(ByVal Model As UsuariosViewModel, ByVal l_Roles As String()) As Task(Of ActionResult)
        Try
            Await GeneraViewbags()

            If ModelState.IsValid Then
                Dim Correo As New List(Of String)
                Dim listaDirecciones As New List(Of String)
                Dim archivos As New List(Of String)
                Dim respuesta As RespuestaControlGeneral
                Dim account As New AccountController

                If l_Roles Is Nothing OrElse l_Roles.Count = 0 Then
                    Flash.Instance.Error("", "Debe seleccionar por lo menos un rol de usuario.")
                    'Flash.Instance.Error("", Idiomas.Resource.msg_seleccionePerfil)
                    Return View()
                End If

                Model.Correo = Model.Correo.Trim().ToUpper

                'Dim pass = Guid.NewGuid().ToString("N").Substring(0, 8)
                'Dim pass = "MafAlvarez2022$"
                Dim verificationToken As String = WebSecurity.CreateUserAndAccount(Model.Correo, Model.Contrasena, New With {
                                                     .Nombre = Model.Nombre,
                                                     .InformacionContacto = Model.InformacionContacto,
                                                     .CuentaActiva = 0}, True)

                Roles.AddUserToRoles(Model.Correo, l_Roles)

                Dim idUsuario As Integer = WebSecurity.GetUserId(Model.Correo)

                ''Proceso para agregar las ubicaciones por rol
                If l_Roles.First = "Regional" Then
                    Dim stringToLines As String() = Model.Ubicacion_Select.Split("|")

                    For Each addUbicaciones In stringToLines

                        If Not addUbicaciones = "" Then
                            Dim AltaUbicacion As New UsuarioUbicaciones
                            AltaUbicacion.UserId = idUsuario
                            AltaUbicacion.idUbicacion = addUbicaciones

                            db.UsuarioUbicaciones.Add(AltaUbicacion)
                            db.SaveChanges()
                        End If

                    Next
                ElseIf l_Roles.First = "Cajero" OrElse l_Roles.First = "Vendedor" Then

                    Dim UsuarioAltaMostrador = Await (From n In db.Usuarios Where n.idUsuario = idUsuario).FirstOrDefaultAsync()
                    UsuarioAltaMostrador.MostradorPref = Model.Ubicacion

                    db.SaveChanges()
                End If

                account.ActivateAccount(verificationToken)

                'Correo = Await CapaCorreo.GenerarCorreoInvitacionUsuarios(Model.Nombre, verificationToken, Model.Correo, pass)
                'listaDirecciones.Add(Model.Correo)
                'Dim html As String = String.Join(vbCrLf, Correo.ToArray)
                'respuesta = Await Mail.SendMail("Creación de Usuario " + DateTime.Now.ToString(), Correo.ToArray(), listaDirecciones.ToArray(), archivos.ToArray(), True)

                Flash.Instance.Success(String.Format("El Usuario {0} ha sido registrado correctamente.", Model.Nombre))
                Return RedirectToAction("ConsultarUsuarios")
            Else
                Flash.Instance.Error("Hubo un problema al validar la estructura de los datos proporcionados, intentelo nuevamente mas tarde.")
                Return View()
            End If
        Catch ex As Exception
            Flash.Instance.Error("Hubo un problema al registrar el usuario. Detalles: " + ex.Message)
            Return View()
        End Try


    End Function

#End Region


#Region "Consultar Usuarios"
    <HttpPost()>
    Public Async Function ConsultarUsuariosActivos() As Task(Of JsonResult)

        Try
            Dim l_UsuariosActivos = Await Consulta.ConsultarUsuarios("Activos")

            Dim l_UsuariosViewModel As List(Of UsuariosViewModel) = AutoMap.AutoMapperListaUsuarios(l_UsuariosActivos)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_UsuariosViewModel))

        Catch ex As Exception

        End Try

    End Function

    <HttpPost()>
    Public Async Function ConsultarUsuariosTotales() As Task(Of JsonResult)

        Try
            Dim l_UsuariosActivos = Await Consulta.ConsultarUsuarios()

            Dim l_UsuariosViewModel As List(Of UsuariosViewModel) = AutoMap.AutoMapperListaUsuarios(l_UsuariosActivos)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_UsuariosViewModel))

        Catch ex As Exception

        End Try

    End Function

    <HttpPost()>
    Public Async Function ConsultarUsuariosDesactivados() As Task(Of JsonResult)

        Try
            Dim l_UsuariosActivos = Await Consulta.ConsultarUsuarios("Desactivados")

            Dim l_UsuariosViewModel As List(Of UsuariosViewModel) = AutoMap.AutoMapperListaUsuarios(l_UsuariosActivos)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_UsuariosViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al consultar los usuarios. Intentelo más tarde."))
        End Try

    End Function

    <HttpPost()>
    Public Async Function ConsultarUsuariosSinConfirmar() As Task(Of JsonResult)

        Try
            Dim l_UsuariosActivos = Await Consulta.ConsultarUsuarios("SinConfirmar")

            Dim l_UsuariosViewModel As List(Of UsuariosViewModel) = AutoMap.AutoMapperListaUsuarios(l_UsuariosActivos)

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", l_UsuariosViewModel))

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ""))
        End Try

    End Function

    <HttpPost>
    Public Async Function ConsultarUsuario(ByVal id As String) As Task(Of JsonResult)
        Try
            Dim usuario As Usuarios = Await Consulta.ConsultarUsuarioEspecifico(id, "E")

            If Not IsNothing(usuario) Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", New With {.Nombre = usuario.Nombre, .Correo = usuario.Correo, .Idioma = usuario.Lenguaje, .Contacto = usuario.InformacionContacto, .Rol = usuario.webpages_Roles.First.RoleName}))
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El Usuario no pudo ser localizado."))
            End If

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al localizar al usuario."))
        End Try
    End Function

    <HttpPost>
    Public Async Function GestionarUsuarios(ByVal id As String, ByVal Accion As String) As Task(Of JsonResult)

        Try
            If Not WebSecurity.IsAuthenticated Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No cuentas con los permisos para realizar esta acción."))
            Else
                Dim l_Admin As New List(Of Usuarios)
                Dim Admin = Await (From n In db.Usuarios Where n.idUsuario = id).FirstOrDefaultAsync()

                If IsNothing(Admin) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar el usuario, verifique su existencia o contacte con el administrador."))
                End If

                If Accion = "Act" Then
                    Admin.CuentaActiva = True
                ElseIf Accion = "Des" Then
                    Admin.CuentaActiva = False
                End If

                db.SaveChanges()

                l_Admin = Await Consulta.ConsultarUsuarios()

                Dim l_UsuariosViewModel As List(Of UsuariosViewModel) = AutoMap.AutoMapperListaUsuarios(l_Admin)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "El Usuario se actualizó con exito!", l_UsuariosViewModel))

            End If

        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "Hubo un problema al actualizar el usuario."))
        End Try



    End Function

    <HttpPost()>
    Public Async Function EditarUsuarios(ByVal model As UsuariosViewModel) As Task

    End Function
#End Region

#Region "Administracion de Usuario"

#End Region

#Region "Funciones auxiliares"
    Public Async Function GeneraViewbags() As Threading.Tasks.Task
        Dim Select_Idiomas As New List(Of SelectListItem)
        Dim Select_Ubicaciones As New List(Of SelectListItem)


        Dim l_Idiomas = Await Consulta.ConsultarIdiomas()

        For Each RegistroIdiomas In l_Idiomas
            Select_Idiomas.Add(New SelectListItem With {.Text = RegistroIdiomas.Descripcion, .Value = RegistroIdiomas.Clave})
        Next

        Dim l_Ubicaciones = Await Consulta.ConsultarUbicacionesAlmacen()

        For Each RegistroUbicaciones In l_Ubicaciones
            Select_Ubicaciones.Add(New SelectListItem With {.Text = RegistroUbicaciones.DescripcionAlmacen, .Value = RegistroUbicaciones.idUbicacion})
        Next


        ViewBag.Lenguaje = Select_Idiomas
        ViewBag.Ubicacion = Select_Ubicaciones
        ViewBag.Ubicacion_Reg = Select_Ubicaciones
        'ViewBag.Ubicacion = Await (From n In db.webpages_ub)
        ViewBag.Roles = Await Consulta.ConsultarRoles()
    End Function

    Public Shared Function ErrorCodeToString(ByVal createStatus As MembershipCreateStatus) As String
        ' Vaya a http://go.microsoft.com/fwlink/?LinkID=177550 para
        ' obtener una lista completa de códigos de estado.
        Select Case createStatus
            Case MembershipCreateStatus.DuplicateUserName
                Return "El nombre de usuario ya existe. Escriba un nombre de usuario diferente."
            Case MembershipCreateStatus.DuplicateEmail
                Return "Ya existe un nombre de usuario para esa dirección de correo electrónico. Escriba una dirección de correo electrónico diferente."
            Case MembershipCreateStatus.InvalidPassword
                Return "La contraseña especificada no es válida. Escriba un valor de contraseña válido."
            Case MembershipCreateStatus.InvalidEmail
                Return "La dirección de correo electrónico especificada no es válida. Compruebe el valor e inténtelo de nuevo."
            Case MembershipCreateStatus.InvalidAnswer
                Return "La respuesta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo."
            Case MembershipCreateStatus.InvalidQuestion
                Return "La pregunta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo."
            Case MembershipCreateStatus.InvalidUserName
                Return "El nombre de usuario especificado no es válido. Compruebe el valor e inténtelo de nuevo."
            Case MembershipCreateStatus.ProviderError
                Return "El proveedor de autenticación devolvió un error. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema."
            Case MembershipCreateStatus.UserRejected
                Return "La solicitud de creación de usuario se ha cancelado. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema."
            Case Else
                Return "Error desconocido. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema."
        End Select
    End Function
#End Region

#End Region

End Class
