Imports System.Data.Entity
Imports System.Threading.Tasks
Imports AutoMapper
Imports WebMatrix.WebData

<Authorize()>
<InitializeSimpleMembership()>
Public Class SecurityController
    Inherits System.Web.Mvc.Controller

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
    Public Consulta As New ConsultasController
#End Region

#Region "Procesos Login"

    Public Async Function VerificarIntentosFallidos(ByVal model As LoginModel) As Task(Of RespuestaControlGeneral)
        Dim temporalmenteBloqueado As Boolean = False
        Dim intentos As Integer
        Dim minutos As Integer

        'Consultar los parametros de limite y número de intentos máximos configurados , ademas del usuario que se intenta loguear 
        Dim usuarioMemberShip As webpages_Membership = Await Consulta.GetWebPages(model.UserName)
        Dim paramIntentos As String = Await Consulta.ConsultarParametroClave("MAX_NUM_INTENTOS")
        Dim paramMinutosEspera As String = Await Consulta.ConsultarParametroClave("MINUTOS_ESPERA")


        If Integer.TryParse(paramIntentos, intentos) And Integer.TryParse(paramMinutosEspera, minutos) And usuarioMemberShip IsNot Nothing Then
            If (usuarioMemberShip.LastPasswordFailureDate IsNot Nothing) And (usuarioMemberShip.PasswordFailuresSinceLastSuccess >= intentos) Then
                Dim fechaPermitirIntento As DateTime = usuarioMemberShip.LastPasswordFailureDate
                fechaPermitirIntento = fechaPermitirIntento.AddMinutes(minutos)
                If Date.UtcNow < fechaPermitirIntento Then
                    temporalmenteBloqueado = True
                    Dim tiemporestante As Integer = Math.Abs(DateDiff(DateInterval.Minute, Date.UtcNow, fechaPermitirIntento))

                    Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, String.Format("Se ha alcanzado el número máximo de intentos fallidos {0}, vuelva a intentarlo en {1} minutos", intentos.ToString, tiemporestante.ToString))
                End If
            End If
        End If

        Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "")
    End Function

#End Region

#Region "Procesos Autenticación"

    Public Function ValidarSesionActiva()

        ''Caso 1: Existe la posibilidad de que el usuario aparezca como autentificado pero al hacer algunas acciones
        If WebSecurity.IsAuthenticated Then

        End If

    End Function

#End Region

    <HttpPost()>
    <AllowAnonymous()>
    Public Function loginValidate() As JsonResult
        If WebSecurity.CurrentUserId <= 0 Then
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Exito, ""))
        Else
            Return Json(New RespuestaControlGeneral(EnumTipoRespuesta.Advertencia, "No se pueden tener dos sesiones activas en el portal."))
        End If
    End Function
End Class
