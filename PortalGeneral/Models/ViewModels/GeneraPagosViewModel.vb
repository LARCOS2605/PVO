Public Class GeneraPagosViewModel
    Public Property Customer As String
    Public Property Customertest As String
    Public Property MetodoPago As String
    Public Property FechaPago As DateTime
    Public Property Nota As String
    Public Property MontoTotalPago As Double
    Public Property MontoTotalPagoIngresados As Double

    ''tarjeta de credito
    Public Property Banco As String
    Public Property NumBanco As String

    '' Transferencia Electrónica
    Public Property BancoOrdenante As String
    Public Property RFCBancoOrdenante As String
    Public Property NumCuentaOrdenante As String
    Public Property NoRefPago As String
    Public Property BancoBeneficiario As String
    Public Property RFCBancoBeneficiario As String
    Public Property NumCuentaBeneficiario As String

    '' Tarjeta de Credito
    Public Property TarjetasC_beneficiario As String
    Public Property TipoPago As String

End Class

