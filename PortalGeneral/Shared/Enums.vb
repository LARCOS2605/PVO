Public Module Enums
    Public Enum EnumTipoRespuesta As Integer
        Exito = 1
        Fracaso = 0
        ErrorComunicacion = -1
        Advertencia = 2
    End Enum

    Public Class EnumInterfaz
        Public Const NOMINA As String = "NOMINA"
        Public Const PROVEEDORES As String = "PROVEEDORES"
        Public Const GASTOS As String = "GASTOS DE VIAJE"
    End Class

    Public Class EnumcuentaCorreo
        Public Const NAVIERAS As String = "NAVIERAS"
        Public Const FACTURACION As String = "FACTURACION"
        Public Const COMPLEMENTOS As String = "COMPLEMENTOS"
        Public Const RECEPCOMPLEMENTOS As String = "RECEPCOMPLEMENTOS"
    End Class

    Public Class TipoLog
        Public Const RECIBIDO As String = "Recibido en portal" 'Cuando el usaurio inicial carga la factura al servidor es primer estatus por el que pasan las facturas que son cargadas
        Public Const APROBADO As String = "Aprobado"  'Cuando uno de los administradores asigna una provisión a la factura y además es aprobada
        Public Const PAGADO As String = "Pagado"  'Cuando la provisión asociada al documento aprobado cambia a a estatus P
        Public Const RECHAZADO As String = "Rechazado" 'La factura se puede rechazar solo cuando no esta en estatus aprobado, es decir solo en estatus recibida en portal
        Public Const REACTIVADO As String = "Reactivado"  'Cuando una factura rechazada es regresada a su estatus de recibida en portal
        Public Const BORRADO As String = "Borrado por usuario"  ' Cuando el usuario realiza el borrado la factura que ha subido al portal esto solo se permite cuando esta como recibida
        Public Const DELETEPROVISION As String = "Quitar asignación de provisión"  'Cuando por algúna cancelación se tiene que desasignar la provisóin
        Public Const RECORDATORIO As String = "Recordatorio" ' Cuando se envia un correo de recordatorio al responsable de aprobar una factura
        Public Const CANCELADO As String = "Folio cancelado en SAT" ' Cuando una factura es detectada que ha sido anulada en el SAT
        Public Function toArray()
            Dim campos = (New TipoLog).GetType.GetFields(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static Or System.Reflection.BindingFlags.FlattenHierarchy)
            Return (From c In campos Where c.IsLiteral AndAlso Not c.IsInitOnly Select Name = c.GetValue(Nothing), Value = c.GetValue(Nothing)).ToArray
        End Function
    End Class

    Public Enum EnumTipoXML As Integer
        FACTURA = 1
        BALANZA = 2
        DIOT = 3
    End Enum

    Public Class EnumTipoMovimiento

        Public Const Timbrando As String = "Proceso Timbrado"
        Public Const Resultado As String = "Resultado Timbrado"
        Public Const IniciaProceso As String = "Inicio de Proceso"
        Public Const FinProceso As String = "Fin de Proceso"
        Public Const Cancelacion As String = "Resultado Cancelación"
        Public Const EnProceso As String = "En Proceso"
        Public Const ErrorProceso As String = "Existio errores en el Proceso"

        Public Const IniciaEnvio As String = "Inicio de Envío Mail"
        Public Const ErrorEnvio As String = "Error de Envío Mail"
        Public Const EnvioExitoso As String = "Envío Exitoso Mail"
        Public Const FinEnvio As String = "Fin de Envío Mail"

        Public Const IniciaRecepcion As String = "Inicia Recepción Facturas Intercompañías"
        Public Const ErrorRecepcion As String = "Error en la Recepción Facturas Intercompañías"
        Public Const ResultadoRecepcion As String = "Recepción Exitosa"
        Public Const FinRecepcion As String = "Termina Recepción Facturas Intercompañías"

    End Class

End Module
