Imports System.Net.Mail
Imports System.Globalization
Imports System.IO
Imports System.Threading.Tasks
Imports System.Data.Entity

Public Class Mail

    Public Shared Async Function SendMail(ByVal asunto As String, ByVal cuerpo As String(), ByVal lista As String(), ByVal archivosAdjuntos As String(), Optional ByVal isHtml As Boolean = False) As Task(Of RespuestaControlGeneral)
        Dim response As RespuestaControlGeneral
        Dim db As New PVO_NetsuiteEntities

        Dim configMail As ConfiguracionCorreo

        configMail = Await (From p In db.ConfiguracionCorreo Where p.idCorreo = 1).SingleAsync

        Dim ipCorreo = configMail.ServidorSaliente
        Dim puertoCorreo = configMail.Puerto.ToString

        Dim msg As MailMessage = New MailMessage()
        For Each Mail As String In lista
            If IsValidEmail(Mail) Then
                msg.To.Add(Mail)
            End If
        Next

        Dim context = HttpContext.Current
        msg.From = New MailAddress(configMail.DireccionCorreo, If(context.Session("nameSenderMail") Is Nothing, "PVO MAF", context.Session("nameSenderMail")), System.Text.Encoding.UTF8)
        msg.Subject = asunto
        msg.SubjectEncoding = System.Text.Encoding.UTF8

        For Each s As String In cuerpo
            msg.Body += s + Environment.NewLine
        Next
        msg.BodyEncoding = System.Text.Encoding.UTF8
        msg.IsBodyHtml = isHtml

        Dim client As SmtpClient = New SmtpClient(configMail.ServidorEntrante)
        client.Port = Integer.Parse(puertoCorreo)
        client.UseDefaultCredentials = False
        client.Credentials = New System.Net.NetworkCredential(configMail.DireccionCorreo, configMail.PasswordCuenta)
        client.DeliveryMethod = SmtpDeliveryMethod.Network
        client.EnableSsl = configMail.SSL

        For Each archivo As String In archivosAdjuntos
            Try
                msg.Attachments.Add(New Net.Mail.Attachment(archivo))
            Catch ex As Exception

                If ex IsNot Nothing Then

                End If
            End Try
        Next

        Try
            client.Timeout() = 100000

            'If parametros("Ambiente") = "PRD" Then
            client.Send(msg)
            response = New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "")

            'Else
            '    response = New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "No se envía mensaje, por ser ambiente de pruebas")
            'End If
        Catch ex As Exception
            client.Dispose()
            client = Nothing

            msg.Attachments.Dispose()
            msg.Dispose()
            msg = Nothing
            response = New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, ex.Message +
                                                   If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""))
        End Try
        Return response
    End Function

    Private Shared Function RedirectionUrlValidationCallback1(ByVal redirectionUrl As String) As Boolean
        Dim result As Boolean = False
        Dim redirectionUri As Uri = New Uri(redirectionUrl)

        If redirectionUri.Scheme = "https" Then
            result = True
        End If

        Return result
    End Function

    Public Shared Function IsValidEmail(strIn As String) As Boolean
        Try
            Dim a As New System.Net.Mail.MailAddress(strIn)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Shared Function DomainMapper(match As Match) As String
        Dim invalid As Boolean
        ' IdnMapping class with default property values.
        Dim idn As New IdnMapping()

        Dim domainName As String = match.Groups(2).Value
        Try
            domainName = idn.GetAscii(domainName)
        Catch e As ArgumentException
            invalid = True
        End Try
        Return match.Groups(1).Value + domainName
    End Function

End Class
