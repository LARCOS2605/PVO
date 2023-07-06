
Public Class RespuestaControlGeneral
#Region "Variables miembro"
    Private m_tipo As EnumTipoRespuesta
    Private m_mensaje As String
    Private m_valor As Object

#End Region

#Region "Constructores"
    Public Sub New(ByVal tipo As EnumTipoRespuesta, ByVal mensaje As String)
        m_tipo = tipo
        m_mensaje = mensaje
        m_valor = Nothing
    End Sub

    Public Sub New(ByVal tipo As EnumTipoRespuesta, ByVal mensaje As String, ByVal valor As Object)
        m_tipo = tipo
        m_mensaje = mensaje
        m_valor = valor
        'm_aux = Nothing
    End Sub


#End Region

#Region "Propiedades"

    Public ReadOnly Property Tipo() As EnumTipoRespuesta
        Get
            Return m_tipo
        End Get
    End Property

    Public ReadOnly Property Mensaje() As String
        Get
            Return m_mensaje
        End Get
    End Property

    Public ReadOnly Property Valor() As Object
        Get
            Return m_valor
        End Get
    End Property

#End Region

End Class
