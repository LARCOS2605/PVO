Imports System.Data.Entity
Imports System.DirectoryServices
Imports System.Threading.Tasks
Imports AutoMapper
Imports WebMatrix.WebData

<Authorize()>
<HandleError>
<InitializeSimpleMembership()>
Public Class HomeController
    Inherits AppBaseController

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
#End Region


    Function Index(Optional ByVal soc As String = Nothing) As ActionResult

        If Not WebSecurity.IsAuthenticated Then
            Return RedirectToAction("Login", "Account")
        ElseIf soc IsNot Nothing Then
            Return Redirect("/")
        End If

        Return View()
    End Function

    <HttpPost()>
    <AllowAnonymous()>
    Public Function Login(ByVal model As LoginModel, ByVal returnUrl As String) As ActionResult

        If WebSecurity.CurrentUserId <= 0 Then
            'Quitar los espacios del nombre de usuario o contraseña. Usuarios estan copiando con espacios al inicio de sesión
            model.Password = model.Password.Trim
            model.UserName = model.UserName.Trim
            Dim usuarioBuscado As Usuarios

            'If Not VerificarIntentosFallidos(model) Then
            '    usuarioBuscado = (From u In db.Usuarios Where u.Correo = model.UserName).SingleOrDefault

            '    If usuarioBuscado IsNot Nothing Then
            '        Dim cuenta = (From u In db.webpages_Membership Where u.UserId = usuarioBuscado.idUsuario).SingleOrDefault
            '        If Not usuarioBuscado.CuentaActiva Then
            '            ModelState.AddModelError("", "La cuenta con la que intenta ingresar ha sido desactivada, realice la solicitud de reactivación.")
            '        ElseIf Not cuenta.IsConfirmed Then
            '            ModelState.AddModelError("", "La cuenta de correo no ha sido confirmada, verifique su bandeja o solicite un token de verificación.")
            '        Else
            '            If ModelState.IsValid AndAlso WebSecurity.Login(model.UserName, model.Password, persistCookie:=model.RememberMe) Then
            '                SetCurrentUser(model.UserName)
            '                Return RedirectToLocal(returnUrl)
            '            Else
            '                ' Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            '                If Not WebSecurity.UserExists(model.UserName) Then
            '                    ''ModelState.AddModelError("", "El nombre de usuario es incorrecto.")
            '                    ModelState.AddModelError("", Idiomas.Resource.msg_usuarioIncorrecto)
            '                Else
            '                    ''ModelState.AddModelError("", "La contraseña es incorrecta.")
            '                    ModelState.AddModelError("", Idiomas.Resource.msg_passwordIncorrecto)
            '                End If
            '            End If
            '        End If
            '    End If
            'End If
        Else
            'Flash.Instance.Error("", Idiomas.Resource.alert_NoPermiteDosSesionesMismoNavegador)
        End If
        Return View(model)
    End Function


    'Function Login() As ActionResult
    '    Return View()
    'End Function

    Function About() As ActionResult
        ViewData("Message") = "Your application description page."

        Return View()
    End Function

    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."



        Return View()
    End Function
End Class
