Imports System.Data.Entity
Imports System.Threading.Tasks
Imports MvcFlash.Core
Imports MvcFlash.Core.Extensions
Imports Microsoft.Web.WebPages.OAuth
Imports WebMatrix.WebData
Imports System.Security.Cryptography

<Authorize()>
<InitializeSimpleMembership()>
Public Class AccountController

#Region "Constructores"
    Inherits AppBaseController
    Private db As New PVO_NetsuiteEntities
    Public Security As New SecurityController
    Public Consultas As New ConsultasController
    Public AutoMap As New AutoMappeo
#End Region

#Region "Vistas"

#Region "Login"
    <AllowAnonymous()>
    Public Function Login(ByVal returnUrl As String) As ActionResult

        If WebSecurity.IsAuthenticated = True Then
            Return RedirectToAction("Index", "Home")
        End If

        ViewData("ReturnUrl") = returnUrl
        Return View()
    End Function

    Public Async Function UserStats() As Task(Of ActionResult)
        Dim nombre = User.Identity.Name
        Dim usuarioSesion As Usuarios = Await (From u In db.Usuarios Where u.idUsuario = WebSecurity.CurrentUserId).SingleOrDefaultAsync()

        ViewBag.Lenguaje = Await Consultas.ConsultarIdiomas()
        ViewBag.usuario = usuarioSesion
        Return View("UserStats")
    End Function

    Public Function Menu() As PartialViewResult
        Return PartialView()
    End Function

#End Region

#Region "Reset"

    <AllowAnonymous()>
    Public Function Reset() As ActionResult
        Return View()
    End Function

    <HttpPost()>
    <AllowAnonymous()>
    Async Function Reset(ByVal model As LoginModel) As Task(Of ActionResult)
        Try
            Dim respuestaEnvio As RespuestaControlGeneral
            If Mail.IsValidEmail(model.UserName.Trim()) Then
                Dim usuarioBuscado As Usuarios = Await Consultas.ConsultarUsuarioEspecifico(model.UserName, "C")
                If usuarioBuscado IsNot Nothing Then
                    If usuarioBuscado.CuentaActiva Then
                        respuestaEnvio = Await EnviarReseteoContrasenia(usuarioBuscado.Nombre, usuarioBuscado.Correo)
                        If respuestaEnvio.Tipo = EnumTipoRespuesta.Exito Then
                            Flash.Instance.Success("", "Se ha enviado un mensaje de correo a " + usuarioBuscado.Correo + " con un enlace para poder completar la solicitud.")
                            Return RedirectToAction("Index", "Home")
                        ElseIf respuestaEnvio.Tipo = EnumTipoRespuesta.Fracaso Then
                            Flash.Instance.Error("", respuestaEnvio.Mensaje)
                        End If
                    Else
                        Flash.Instance.Error("", "La cuenta se encuentra inactiva y no podrá realizar el proceso, consulte con el administrador del sistema.")
                    End If

                Else
                    Flash.Instance.Warning("", "El correo " + model.UserName + " no se encuentra registrado como un usuario.")
                End If
            Else
                Flash.Instance.Info("", "No se ha escrito una dirección de correo con formato válido.")
            End If
            Return View()
        Catch ex As Exception
            Flash.Instance.Error(String.Format("Hubo un error al hacer la petición. Detalles: {0}", ex.Message))
            Return View()
        End Try

    End Function

#End Region

#End Region

#Region "Procesos MVC"
    <HttpPost()>
    <AllowAnonymous()>
    <ValidateAntiForgeryToken()>
    Public Async Function Login(ByVal model As LoginModel, ByVal returnUrl As String) As Task(Of ActionResult)

        TempData("Error") = Nothing

        If WebSecurity.CurrentUserId <= 0 Then

            System.Web.HttpContext.Current.Session("L_Mostradores") = Nothing
            System.Web.HttpContext.Current.Session("Validado_User") = Nothing
            System.Web.HttpContext.Current.Session("ValidaNombreMostrador") = Nothing

            model.Password = model.Password.Trim
            model.UserName = model.UserName.Trim
            Dim usuarioBuscado As Usuarios

            Dim respuesta As RespuestaControlGeneral

            respuesta = Await Security.VerificarIntentosFallidos(model)

            If respuesta.Tipo = EnumTipoRespuesta.Exito Then

                usuarioBuscado = Await Consultas.ConsultarUsuarioEspecifico(model.UserName, "C")

                If usuarioBuscado IsNot Nothing Then

                    Dim cuenta = Await Consultas.GetMember(usuarioBuscado.idUsuario)

                    If Not usuarioBuscado.CuentaActiva Then
                        TempData("Error") = "true"
                        ModelState.AddModelError("ValidationMessage", "La cuenta con la que intenta ingresar ha sido desactivada, realice la solicitud de reactivación.")
                    ElseIf Not cuenta.IsConfirmed Then
                        TempData("Error") = "true"
                        ModelState.AddModelError("ValidationMessage", "La cuenta de correo no ha sido confirmada, verifique su bandeja o solicite un token de verificación.")
                    Else
                        If ModelState.IsValid AndAlso WebSecurity.Login(model.UserName, model.Password, persistCookie:=model.customControlAutosizing) Then
                            SetCurrentUser(model.UserName)
                            Return RedirectToLocal(returnUrl)
                        Else
                            ' Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
                            If Not WebSecurity.UserExists(model.UserName) Then
                                TempData("Error") = "true"
                                ModelState.AddModelError("ValidationMessage", "El nombre de usuario es incorrecto.")
                            Else
                                TempData("Error") = "true"
                                ModelState.AddModelError("ValidationMessage", "La contraseña es incorrecta.")
                            End If
                        End If
                    End If
                Else
                    ModelState.AddModelError("ValidationMessage", "El usuario ingresado, no pudo ser localizádo.")
                End If
            End If
        Else
            ModelState.AddModelError("", "No se permiten dos sesiones en el mismo navegador")
        End If

        Return View(model)
    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Function ResetPassword(ByVal model As LocalPasswordModel) As JsonResult

        Dim hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name))
        ViewData("HasLocalPassword") = hasLocalAccount
        ViewData("ReturnUrl") = Url.Action("Manage")

        If model.NewPassword.Trim <> model.ConfirmPassword.Trim Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La nueva contraseña y la contraseña de confirmación no coinciden."))
        End If

        If model.NewPassword.Length < 6 Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El número de caracteres del nuevo password debe ser de al menos 6."))
        End If

        If hasLocalAccount Then
            If ModelState.IsValid Then
                ' ChangePassword iniciará una excepción en lugar de devolver false en determinados escenarios de error.
                Dim changePasswordSucceeded As Boolean
                Try
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword.Trim, model.NewPassword.Trim)
                    If changePasswordSucceeded Then
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "La contraseña fue cambiada con exito."))
                    Else
                        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La contraseña actual es incorrecta o la nueva contraseña no es válida."))
                    End If
                Catch e As Exception
                    changePasswordSucceeded = False
                End Try
            End If
        Else

        End If
        ' Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al intentar cambiar su contraseña, intenelo más tarde."))

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Function ResetPasswordAdmin(ByVal model As LocalPasswordModel) As JsonResult

        If model.NewPassword.Trim <> model.ConfirmPassword.Trim Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La nueva contraseña y la contraseña de confirmación no coinciden."))
        End If

        If model.NewPassword.Length < 6 Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El número de caracteres del nuevo password debe ser de al menos 6."))
        End If

        Dim ValidaUsuario = (From n In db.Usuarios Where n.idUsuario = model.id).FirstOrDefault()

        If IsNothing(ValidaUsuario) Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "No se pudo localizar al usuario."))
        End If

        Dim TokenReset = WebSecurity.GeneratePasswordResetToken(ValidaUsuario.Correo, 1440)

            Try

                If WebSecurity.ResetPassword(TokenReset, model.NewPassword) Then
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "La contraseña fue cambiada con exito."))
                Else
                    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La contraseña actual es incorrecta o la nueva contraseña no es válida."))
                End If

            Catch e As Exception
                ' Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al intentar cambiar su contraseña, intenelo más tarde. Detalles: " + e.Message))
            End Try




    End Function

    <AllowAnonymous()>
    <HttpPost()>
    Function ResetPasswordRequest(ByVal id As String, ByVal model As LocalPasswordModel) As ActionResult
        If model.NewPassword <> model.ConfirmPassword Then
            Flash.Instance.Warning("", "Las contraseñas no coindicen en ambas casillas por favor vuelva a intentarlo.")
            'Flash.Instance.Warning("", Idiomas.Resource.NoCoincidenContrasenias)
            Return View()
        ElseIf model.NewPassword.Length < 6 Then
            Flash.Instance.Warning("", "La contraseña debe tener una longitud mínima de 6 caracteres.")
            'Flash.Instance.Warning("", Idiomas.Resource.LongitudContrasenia)
            Return View()
        Else
            If WebSecurity.ResetPassword(id, model.NewPassword) Then
                Flash.Instance.Success("", "Se ha cambiado la contraseña exitosamente, ahora puede ingresar usando su nueva contraseña.")
                'Flash.Instance.Success("", Idiomas.Resource.CambioContraseniaExito)
                Return RedirectToAction("Index", "Home")
            Else
                Flash.Instance.Error("", "El enlace no es válido por que es erróneo o el proceso ya ha sido realizado, verifique el enlace recibido e intente nuevamente.")
                'Flash.Instance.Error("", Idiomas.Resource.ErrorContraseniaEnlace)
                Return View()
            End If
        End If

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Function CambiarInformacionPrincipal(ByVal model As UsuariosViewModel) As JsonResult

        'Dim hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name))
        'ViewData("HasLocalPassword") = hasLocalAccount
        'ViewData("ReturnUrl") = Url.Action("Manage")

        'If model.NewPassword.Trim <> model.ConfirmPassword.Trim Then
        '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La nueva contraseña y la contraseña de confirmación no coinciden."))
        'End If

        'If model.NewPassword.Length < 6 Then
        '    Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "El número de caracteres del nuevo password debe ser de al menos 6."))
        'End If

        'If hasLocalAccount Then
        '    If ModelState.IsValid Then
        '        ' ChangePassword iniciará una excepción en lugar de devolver false en determinados escenarios de error.
        '        Dim changePasswordSucceeded As Boolean
        '        Try
        '            changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword.Trim, model.NewPassword.Trim)
        '            If changePasswordSucceeded Then
        '                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "La contraseña fue cambiada con exito."))
        '            Else
        '                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "La contraseña actual es incorrecta o la nueva contraseña no es válida."))
        '            End If
        '        Catch e As Exception
        '            changePasswordSucceeded = False
        '        End Try
        '    End If
        'Else

        'End If
        ' Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
        Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al intentar cambiar su contraseña, intenelo más tarde."))

    End Function


    <HttpPost()>
    Public Async Function ConsultarInfoUsuario(ByVal id As String) As Task(Of JsonResult)
        Try
            Dim Usuario = Await Consultas.ConsultarUsuarioEspecifico(id, "En")

            If Not IsNothing(Usuario) Then

                Dim MapUsuario = AutoMap.AutoMapperUserToViewModel(Usuario)

                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "", MapUsuario))
            Else
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ""))
            End If
        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ""))
        End Try

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken()>
    Public Async Function SeleccionarMostradorPred(ByVal id As String) As Task(Of JsonResult)

        Try

            Dim idvalor = Await (From n In db.Ubicaciones Where n.DescripcionAlmacen = id Select n.idUbicacion).FirstOrDefaultAsync

            Dim ValidaUsuario = Await (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefaultAsync()

            ValidaUsuario.MostradorPref = idvalor
            db.SaveChanges()

            System.Web.HttpContext.Current.Session("ValidaNombreMostrador") = Nothing

            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "El Mostrador ha sido actualizado con exito!"))
        Catch ex As Exception
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Hubo un problema al seleccionar un mostrador."))
        End Try


    End Function
#End Region


    <AllowAnonymous()>
    Function ResetPasswordValidate(Optional ByVal id As String = Nothing) As ActionResult
        Dim member As webpages_Membership = (From w In db.webpages_Membership Where w.PasswordVerificationToken = id).SingleOrDefault

        If member IsNot Nothing Then
            ' La fecha de expiración se almacena en UTC por tanto la validación se hace con esa fecha          
            If member.PasswordVerificationTokenExpirationDate <= Date.UtcNow Then
                'La fecha de expiración del token ha caducado no permitir que el usuario resetee su contraseña
                Flash.Instance.Warning("", "El enlace ha caducado vuelva a solicitar el reseteo de su contraseña.")
                Return RedirectToAction("Reset", "Account")
            Else
                Return RedirectToAction("ResetPasswordRequest", "Account", New With {.id = id})
            End If
        Else
            Flash.Instance.Error("", "El enlace no es válido ya sea porque es erróneo o ya fue usado para realizar el proceso, verifique el enlace recibido e intente nuevamente.")
            Return RedirectToAction("Index", "Home")
        End If
    End Function

    <AllowAnonymous()>
    Function ResetPasswordRequest(ByVal id As String) As ActionResult
        Return View()
    End Function


    Async Function EnviarReseteoContrasenia(ByVal nombreUser As String, ByVal UsuarioMail As String) As Task(Of RespuestaControlGeneral)
        Dim cuerpo As New List(Of String)
        Dim listaDirecciones As New List(Of String)
        Dim archivos As New List(Of String)
        Dim TokenReset As String
        Try
            TokenReset = WebSecurity.GeneratePasswordResetToken(UsuarioMail, 1440)
        Catch ex As Exception
            Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Antes de poder recuperar su cuenta debe confirmar su cuenta de correo mediante el link.")
        End Try

        Dim LinkWithToken As String = Request.Url.Scheme + "://" + Request.Url.Authority + "/Account/ResetPasswordValidate/" + TokenReset
        Dim linkImage As String = Request.Url.Scheme + "://" + Request.Url.Authority _
                                  + If(Session("logo") Is Nothing, "/Images/logo_mail.jpg", Session("logo"))
        Dim fontSize As String = "font-size:10px"

        cuerpo.Add(String.Format("<!DOCTYPE html> <html><body>"))
        ''Cabecera
        cuerpo.Add(String.Format("<body style='width:100%;font-family:Open Sans, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0'> 
  <div class='es-wrapper-color' style='background-color:#EFF2F7'> 
   <!--[if gte mso 9]>
			<v:background xmlns:v='urn:schemas-microsoft-com:vml' fill='t'>
				<v:fill type='tile' color='#eff2f7'></v:fill>
			</v:background>
		<![endif]--> 
   <table class='es-wrapper' width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top'> 
     <tr style='border-collapse:collapse'> 
      <td valign='top' style='padding:0;Margin:0'> 
       <table class='es-header' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:#052358;background-repeat:repeat;background-position:center top'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' bgcolor='transparent' style='padding:0;Margin:0;background-color:transparent'> 
           <table class='es-header-body' cellspacing='0' cellpadding='0' bgcolor='#1f4995' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#1f4995;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:20px;padding-bottom:20px'> 
               <table cellspacing='0' cellpadding='0' width='100%' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                 <tr style='border-collapse:collapse'> 
                  <td class='es-m-p0r' valign='top' align='center' style='padding:0;Margin:0;width:570px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;font-size:0px'><a target='_blank' href='https://www.muellesmaf.com.mx/' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:none;color:#FFFFFF;font-size:12px'><img src='https://i0.wp.com/www.muellesmaf.com.mx/wp-content/uploads/2022/07/cropped-Logo-maf-solo.png?w=513&ssl=1' alt style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic' height='100'></a></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' style='padding:0;Margin:0'> 
           <table class='es-content-body' cellspacing='0' cellpadding='0' bgcolor='#fefefe' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FEFEFE;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:40px;padding-bottom:40px'> 
               <table cellpadding='0' cellspacing='0' width='100%' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                 <tr style='border-collapse:collapse'> 
                  <td align='center' valign='top' style='padding:0;Margin:0;width:570px'> 
                   <table cellpadding='0' cellspacing='0' width='100%' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0'><h1 style='Margin:0;line-height:31px;mso-line-height-rule:exactly;font-family:roboto,helvetica neue, helvetica, arial, sans-serif;font-size:26px;font-style:normal;font-weight:bold;color:#3C4858'>Hola, " + nombreUser + "</h1></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:justify'>Hemos recibido una petición para restablecer la contraseña de su cuenta.<br><br><strong>Para realizar esta acción haga clic en el siguiente enlace:</strong><br><br></p></td>
                     </tr> 
                     
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0'><span class='es-button-border' style='border-style:solid;border-color:#0C66FF;background:#0C66FF;border-width:0px;display:inline-block;border-radius:0px;width:auto'><a href='" + LinkWithToken + "' class='es-button es-button-1' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:14px;border-style:solid;border-color:#0C66FF;border-width:15px 30px;display:inline-block;background:#0C66FF;border-radius:0px;font-family:Open Sans, sans-serif;font-weight:bold;font-style:normal;line-height:17px;width:auto;text-align:center'>Restablecer Contraseña</a></span></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:justify'><br><strong>Si el enlace no funciona al hacer clic en él, puede copiarlo en la ventana de su navegador o teclearlo directamente.</strong></p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#0c66ff;font-size:14px'>" + LinkWithToken + "</p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:left'>Si usted no realizó esta petición puede ignorar este correo.</p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px'><br></p></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table cellpadding='0' cellspacing='0' class='es-footer' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:#141B24;background-repeat:repeat;background-position:center top'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' style='padding:0;Margin:0'> 
           <table class='es-footer-body' cellspacing='0' cellpadding='0' bgcolor='#ffffff' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#273444;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:40px;padding-bottom:40px'> 
               <!--[if mso]><table style='width:570px' cellpadding='0' 
                        cellspacing='0'><tr><td style='width:180px' valign='top'><![endif]--> 
               <table class='es-left' cellspacing='0' cellpadding='0' align='left' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left'> 
                 <tr style='border-collapse:collapse'> 
                  <td class='es-m-p20b' align='left' style='padding:0;Margin:0;width:180px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;font-size:0px'><a target='_blank' href='https://www.muellesmaf.com.mx/' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:none;color:#FFFFFF;font-size:12px'><img src='https://i0.wp.com/www.muellesmaf.com.mx/wp-content/uploads/2022/07/cropped-Logo-maf-solo.png?w=513&ssl=1' alt style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic' width='70'></a></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;padding-top:20px;font-size:0'> 
                       <table cellpadding='0' cellspacing='0' class='es-table-not-adapt es-social' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td align='center' valign='top' style='padding:0;Margin:0;padding-right:10px'><img src='https://rvstos.stripocdn.email/content/assets/img/social-icons/logo-white/facebook-logo-white.png' alt='Fb' title='Facebook' width='32' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic'></td> 
                          <td align='center' valign='top' style='padding:0;Margin:0;padding-right:10px'><img src='https://rvstos.stripocdn.email/content/assets/img/social-icons/logo-white/twitter-logo-white.png' alt='Tw' title='Twitter' width='32' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table> 
               <!--[if mso]></td><td style='width:20px'></td><td style='width:370px' valign='top'><![endif]--> 
               <table class='es-right' cellspacing='0' cellpadding='0' align='right' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right'> 
                 <tr style='border-collapse:collapse'> 
                  <td align='left' style='padding:0;Margin:0;width:370px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='left' class='es-m-txt-c' style='padding:0;Margin:0;padding-top:20px;padding-bottom:20px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:18px;color:#8492A6;font-size:12px'>Contacto - 5565-1580 & 3000-2999
                        <br>Antonio M. Rivera Num. 7 Centro industrial Tlalnepantla Estado de México C.P. 54030</p></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table> 
               <!--[if mso]></td></tr></table><![endif]--></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table></td> 
     </tr> 
   </table> 
  </div>  
 </body>"))

        listaDirecciones.Add(UsuarioMail)
        Dim Html As String = String.Join(vbCrLf, cuerpo)

        Dim Respues As RespuestaControlGeneral

        Respues = Await Mail.SendMail("Reestablecer contraseña.", cuerpo.ToArray(), listaDirecciones.ToArray(), archivos.ToArray(), True)

        Return Respues

        'Return Mail.SendMailExchange(Idiomas.Resource.mail_reestablecerPassword, Html, listaDirecciones, archivos.ToArray, True)
        'Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "")
    End Function


    Function About() As ActionResult
        ViewData("Message") = "Your application description page."

        Return View()
    End Function


    Public Function CerrarSesion() As ActionResult
        WebSecurity.Logout()
        Return RedirectToAction("Index", "Home")
    End Function


    <HttpPost()>
    <AllowAnonymous()>
    Public Function loginValidate() As JsonResult
        Try
            If WebSecurity.CurrentUserId <= 0 Then
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
            Else
                WebSecurity.Logout()
                Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
            End If
        Catch ex As Exception

        End Try
    End Function


    Public Sub SetCurrentUser(Optional ByVal correo As String = Nothing)
        Using db As New PVO_NetsuiteEntities
            Dim usuarioBuscado As Usuarios = (From u In db.Usuarios Where u.Correo = WebSecurity.CurrentUserName Or u.Correo = correo).SingleOrDefault
            If usuarioBuscado IsNot Nothing Then
                System.Web.HttpContext.Current.Session("Nombre") = usuarioBuscado.Nombre
                System.Web.HttpContext.Current.Session("Lenguaje") = usuarioBuscado.Lenguaje
            Else
                System.Web.HttpContext.Current.Session("Nombre") = Nothing
            End If
        End Using
    End Sub

    Private Function RedirectToLocal(ByVal returnUrl As String) As ActionResult
        If Url.IsLocalUrl(returnUrl) Then
            Return Redirect(returnUrl)
        Else
            Return RedirectToAction("Index", "Home")
        End If
    End Function

    <AllowAnonymous()>
    Sub ActivateAccount(Optional ByVal id As String = Nothing)
        Dim db As New PVO_NetsuiteEntities
        Dim diasVigenciaActivacion As Integer
        Dim fechaLimiteActivacion As DateTime
        Dim usuarioActivado As New Usuarios
        Dim paramdiasVigenciaActivacion As String
        If WebSecurity.ConfirmAccount(id) Then

            usuarioActivado = (From s In db.Usuarios Join c In db.webpages_Membership On s.idUsuario Equals c.UserId Where c.ConfirmationToken = id Select s).FirstOrDefault
            paramdiasVigenciaActivacion = (From p In db.Parametros Where p.Clave = "DIAS_ACTIVACION_CUENTA" Select p.Valor).SingleOrDefault

            If Not Integer.TryParse(paramdiasVigenciaActivacion, diasVigenciaActivacion) Then
                diasVigenciaActivacion = 4
            End If
            If usuarioActivado IsNot Nothing Then
                Dim membership As webpages_Membership = (From m In db.webpages_Membership Where usuarioActivado.idUsuario = m.UserId).FirstOrDefault
                fechaLimiteActivacion = membership.CreateDate
                fechaLimiteActivacion = fechaLimiteActivacion.AddDays(diasVigenciaActivacion)
                'Si no esta activado activar la cuenta creada si ya esta activa mostrar el mensaje informativo
                If usuarioActivado.CuentaActiva Then
                    Flash.Instance.Info(String.Format("La cuenta del usuario {0} ya fue activada previamente con el token.", usuarioActivado.Nombre))
                Else
                    If fechaLimiteActivacion < Date.Now Then
                        Flash.Instance.Error(String.Format("La cuenta del usuario {0} no puede ser activada ya que el link ha expirado, consulte con su administrador del sistema.", usuarioActivado.Nombre))
                    Else
                        usuarioActivado.CuentaActiva = True
                        db.Entry(usuarioActivado).State = EntityState.Modified
                        db.SaveChanges()
                        'Flash.Instance.Success(String.Format("Se ha activado la cuenta del usuario {0} ahora puede ingresar con las credenciales que le fueron enviadas.", usuarioActivado.Nombre))
                    End If
                End If
            End If
        Else
            Flash.Instance.Warning("El enlace no es válido, verifique y vuelva a intentar nuevamente.")
        End If
        'Return RedirectToAction("Index", "Home")
    End Sub


End Class
