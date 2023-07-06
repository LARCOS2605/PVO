Imports WebMatrix.WebData
Imports System.Threading

<InitializeSimpleMembership()> _
Public Class AppBaseController
    Inherits System.Web.Mvc.Controller

    Public Shared IdiomasSoportados As String() = {"es", "en"}
    'Protected ambiente As String = Util.GetParameter("Ambiente")

    Protected Overrides Sub Initialize(requestContext As RequestContext)
        MyBase.Initialize(requestContext)
        Dim languaje As String = "en"

        If System.Web.HttpContext.Current.Session("Lenguaje") IsNot Nothing Then
            languaje = System.Web.HttpContext.Current.Session("Lenguaje")
        ElseIf WebSecurity.IsAuthenticated And WebSecurity.Initialized Then
            Using db As New PVO_NetsuiteEntities
                Dim usuarioBuscado As Usuarios = db.Usuarios.Find(WebSecurity.CurrentUserId)
                System.Web.HttpContext.Current.Session("Lenguaje") = usuarioBuscado.Lenguaje
                languaje = usuarioBuscado.Lenguaje
            End Using
        Else
            For Each item In Request.UserLanguages
                Dim temp As String = item.Split("-")(0)
                If IdiomasSoportados.Contains(temp) Then
                    languaje = temp
                    Exit For
                End If
            Next
        End If

        Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(If(languaje = "es", "es-MX", "en"))
        Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(languaje)
    End Sub
End Class