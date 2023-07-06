Imports System.Drawing.Imaging
Imports Gma.QrCodeNet.Encoding
Imports Gma.QrCodeNet.Encoding.Windows.WPF
Imports System.Drawing
Imports System.IO
Imports System.Windows.Media.Imaging
Imports Gma.QrCodeNet.Encoding.Windows.Render


Module ModuleQRCode

#Region "Variables Miembro"
    Private parametro As String = ""
    Private parametros As String()
    Private nombreArchivo As String = ""
    Private texto As String = ""
    Private qrEncoder As QrEncoder
    Private qrCode As QrCode
    Private renderer As WriteableBitmapRenderer

    Private longitudPixels As Integer = 3
    Private pathTarget As String = "C:\temp\QRCode\"
    Private extTarget As String = "bmp"
#End Region

    Function QR(ByVal args As String) As RespuestaControlGeneral
        If args.Length < 1 Then
            Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, "Argumentos insuficientes", "")
        End If

        ''Generar numero de consulta unico
        Dim rnd As New Random()
        Dim numero As Integer = rnd.Next(10000000, 11000000)
        Dim lsNumero As String = Hex(numero)

        nombreArchivo = String.Format("QR_{0}", lsNumero)
        texto = args

        qrEncoder = New QrEncoder(ErrorCorrectionLevel.H)
        qrCode = qrEncoder.Encode(texto)

        renderer = New WriteableBitmapRenderer(New FixedModuleSize(longitudPixels, QuietZoneModules.Four), System.Windows.Media.Colors.Black, System.Windows.Media.Colors.White)

        If Not Directory.Exists(pathTarget) Then
            Directory.CreateDirectory(pathTarget)
        End If

        Dim pathDestino As String = pathTarget + nombreArchivo + "." + extTarget.ToLower
        If File.Exists(pathDestino) Then
            Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "El codigo QR ya existe.", pathDestino)
        Else
            Dim stream As FileStream

            stream = New FileStream(pathDestino, System.IO.FileMode.Create)
            renderer.WriteToStream(qrCode.Matrix, formato, stream)
            stream.Close()
            If File.Exists(pathDestino) Then
                Return New RespuestaControlGeneral(EnumTipoRespuesta.Exito, "El codigo QR fue creado con exito.", pathDestino)
            Else
                Return New RespuestaControlGeneral(EnumTipoRespuesta.Fracaso, String.Format("No se pudocrear el código QR en el path {0}, intente nuevamente.", pathDestino), "")
            End If

        End If
    End Function

    Private Function formato() As ImageFormatEnum
        Dim imgFormat As ImageFormatEnum = ImageFormatEnum.BMP

        Select Case extTarget
            Case "bmp"
                imgFormat = ImageFormatEnum.BMP
            Case "tiff"
                imgFormat = ImageFormatEnum.TIFF
            Case "gif"
                imgFormat = ImageFormatEnum.GIF
            Case "jpeg"
                imgFormat = ImageFormatEnum.JPEG
            Case "png"
                imgFormat = ImageFormatEnum.PNG
        End Select

        Return imgFormat
    End Function

End Module
