﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Este código fue generado por una herramienta.
'     Versión de runtime:4.0.30319.42000
'
'     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
'     se vuelve a generar el código.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'StronglyTypedResourceBuilder generó automáticamente esta clase
    'a través de una herramienta como ResGen o Visual Studio.
    'Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    'con la opción /str o recompile su proyecto de VS.
    '''<summary>
    '''  Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Public Class Resource
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Idiomas.Resource", GetType(Resource).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        '''  búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a ¿Confirma que desea sincronizar los clientes? El proceso puede tardar un poco..
        '''</summary>
        Public Shared ReadOnly Property Alert_ConfirmSyncCustom() As String
            Get
                Return ResourceManager.GetString("Alert_ConfirmSyncCustom", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Para porder visualizar el cliente, es necesario ir al apartado &quot;Consultar Clientes&quot; y dar clic en el boton de sincronización..
        '''</summary>
        Public Shared ReadOnly Property Alert_CrearCliente() As String
            Get
                Return ResourceManager.GetString("Alert_CrearCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Recordatorio:.
        '''</summary>
        Public Shared ReadOnly Property Alert_Recordatorio() As String
            Get
                Return ResourceManager.GetString("Alert_Recordatorio", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Se han sincronizado los clientes con exito....
        '''</summary>
        Public Shared ReadOnly Property Alert_SusSyncCustomer() As String
            Get
                Return ResourceManager.GetString("Alert_SusSyncCustomer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Agregar Producto.
        '''</summary>
        Public Shared ReadOnly Property btn_AgregarProducto() As String
            Get
                Return ResourceManager.GetString("btn_AgregarProducto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Buscar.
        '''</summary>
        Public Shared ReadOnly Property btn_BuscarCp() As String
            Get
                Return ResourceManager.GetString("btn_BuscarCp", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Buscar Producto.
        '''</summary>
        Public Shared ReadOnly Property btn_BuscarProducto() As String
            Get
                Return ResourceManager.GetString("btn_BuscarProducto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Almacenar Cliente.
        '''</summary>
        Public Shared ReadOnly Property btn_GuardarCliente() As String
            Get
                Return ResourceManager.GetString("btn_GuardarCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Almacenar Usuario.
        '''</summary>
        Public Shared ReadOnly Property btn_GuardarUsuario() As String
            Get
                Return ResourceManager.GetString("btn_GuardarUsuario", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Confirmados.
        '''</summary>
        Public Shared ReadOnly Property btn_UsuarioActivo() As String
            Get
                Return ResourceManager.GetString("btn_UsuarioActivo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Desactivados.
        '''</summary>
        Public Shared ReadOnly Property btn_UsuarioDesac() As String
            Get
                Return ResourceManager.GetString("btn_UsuarioDesac", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Sin Confirmar.
        '''</summary>
        Public Shared ReadOnly Property btn_UsuarioSinConfir() As String
            Get
                Return ResourceManager.GetString("btn_UsuarioSinConfir", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Todos Los Usuarios.
        '''</summary>
        Public Shared ReadOnly Property btn_UsuarioTodos() As String
            Get
                Return ResourceManager.GetString("btn_UsuarioTodos", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione Un Cliente ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_Clientes() As String
            Get
                Return ResourceManager.GetString("cbb_Clientes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione la Colonia  ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_Colonias() As String
            Get
                Return ResourceManager.GetString("cbb_Colonias", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione el Estado ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_Estado() As String
            Get
                Return ResourceManager.GetString("cbb_Estado", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione el País ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_Pais() As String
            Get
                Return ResourceManager.GetString("cbb_Pais", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a - Seleccione Régimen Fiscal -.
        '''</summary>
        Public Shared ReadOnly Property cbb_Regimen() As String
            Get
                Return ResourceManager.GetString("cbb_Regimen", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione Tipo de Calle ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_TipoCalle() As String
            Get
                Return ResourceManager.GetString("cbb_TipoCalle", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a --- Seleccione su Ubicación ---.
        '''</summary>
        Public Shared ReadOnly Property cbb_Ubicacion() As String
            Get
                Return ResourceManager.GetString("cbb_Ubicacion", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Busqueda de Producto:.
        '''</summary>
        Public Shared ReadOnly Property lbl_BusquedaArticulo() As String
            Get
                Return ResourceManager.GetString("lbl_BusquedaArticulo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Cantidad:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Cantidad() As String
            Get
                Return ResourceManager.GetString("lbl_Cantidad", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Cliente:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Clientes() As String
            Get
                Return ResourceManager.GetString("lbl_Clientes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Código de Producto:.
        '''</summary>
        Public Shared ReadOnly Property lbl_CodigoProducto() As String
            Get
                Return ResourceManager.GetString("lbl_CodigoProducto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Correo Electrónico:.
        '''</summary>
        Public Shared ReadOnly Property lbl_CorreoElectronico() As String
            Get
                Return ResourceManager.GetString("lbl_CorreoElectronico", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Dirección del Cliente.
        '''</summary>
        Public Shared ReadOnly Property lbl_DatosCliente() As String
            Get
                Return ResourceManager.GetString("lbl_DatosCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Datos Fiscales del Cliente.
        '''</summary>
        Public Shared ReadOnly Property lbl_DatosClienteFis() As String
            Get
                Return ResourceManager.GetString("lbl_DatosClienteFis", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a * Datos Obligatorios.
        '''</summary>
        Public Shared ReadOnly Property lbl_DatosObligatorios() As String
            Get
                Return ResourceManager.GetString("lbl_DatosObligatorios", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Descripción:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Descripcion() As String
            Get
                Return ResourceManager.GetString("lbl_Descripcion", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Descuento Asignado:.
        '''</summary>
        Public Shared ReadOnly Property lbl_DescuentoAsignado() As String
            Get
                Return ResourceManager.GetString("lbl_DescuentoAsignado", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Existencia:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Existencia() As String
            Get
                Return ResourceManager.GetString("lbl_Existencia", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Idiomas:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Idioma() As String
            Get
                Return ResourceManager.GetString("lbl_Idioma", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Información de Contacto:.
        '''</summary>
        Public Shared ReadOnly Property lbl_InfoContacto() As String
            Get
                Return ResourceManager.GetString("lbl_InfoContacto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Mostrador:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Mostrador() As String
            Get
                Return ResourceManager.GetString("lbl_Mostrador", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a No. De Lote:.
        '''</summary>
        Public Shared ReadOnly Property lbl_NoLote() As String
            Get
                Return ResourceManager.GetString("lbl_NoLote", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Nombre:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Nombre() As String
            Get
                Return ResourceManager.GetString("lbl_Nombre", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Precio:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Precio() As String
            Get
                Return ResourceManager.GetString("lbl_Precio", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Hola.
        '''</summary>
        Public Shared ReadOnly Property lbl_Prueba() As String
            Get
                Return ResourceManager.GetString("lbl_Prueba", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Roles de Usuario:.
        '''</summary>
        Public Shared ReadOnly Property lbl_Roles() As String
            Get
                Return ResourceManager.GetString("lbl_Roles", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a *Tipo de Cliente.
        '''</summary>
        Public Shared ReadOnly Property lbl_TipoCliente() As String
            Get
                Return ResourceManager.GetString("lbl_TipoCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a *Forma de Pago.
        '''</summary>
        Public Shared ReadOnly Property lbl_TipoPago() As String
            Get
                Return ResourceManager.GetString("lbl_TipoPago", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a TOTAL A PAGAR.
        '''</summary>
        Public Shared ReadOnly Property lbl_Total() As String
            Get
                Return ResourceManager.GetString("lbl_Total", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Acciones.
        '''</summary>
        Public Shared ReadOnly Property lblt_Acciones() As String
            Get
                Return ResourceManager.GetString("lblt_Acciones", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Cantidad.
        '''</summary>
        Public Shared ReadOnly Property lblt_Cantidad() As String
            Get
                Return ResourceManager.GetString("lblt_Cantidad", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Clave de Cliente.
        '''</summary>
        Public Shared ReadOnly Property lblt_ClaveCliente() As String
            Get
                Return ResourceManager.GetString("lblt_ClaveCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Código.
        '''</summary>
        Public Shared ReadOnly Property lblt_Codigo() As String
            Get
                Return ResourceManager.GetString("lblt_Codigo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Correo Electronico.
        '''</summary>
        Public Shared ReadOnly Property lblt_Correo() As String
            Get
                Return ResourceManager.GetString("lblt_Correo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Descripción.
        '''</summary>
        Public Shared ReadOnly Property lblt_Descripcion() As String
            Get
                Return ResourceManager.GetString("lblt_Descripcion", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Estatus.
        '''</summary>
        Public Shared ReadOnly Property lblt_Estatus() As String
            Get
                Return ResourceManager.GetString("lblt_Estatus", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Lote.
        '''</summary>
        Public Shared ReadOnly Property lblt_Lote() As String
            Get
                Return ResourceManager.GetString("lblt_Lote", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Nivel de Descuento.
        '''</summary>
        Public Shared ReadOnly Property lblt_NivelDescuento() As String
            Get
                Return ResourceManager.GetString("lblt_NivelDescuento", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Nombre.
        '''</summary>
        Public Shared ReadOnly Property lblt_Nombre() As String
            Get
                Return ResourceManager.GetString("lblt_Nombre", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Nombre de Cliente.
        '''</summary>
        Public Shared ReadOnly Property lblt_NombreCliente() As String
            Get
                Return ResourceManager.GetString("lblt_NombreCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Precio.
        '''</summary>
        Public Shared ReadOnly Property lblt_Precio() As String
            Get
                Return ResourceManager.GetString("lblt_Precio", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a RFC.
        '''</summary>
        Public Shared ReadOnly Property lblt_RFC() As String
            Get
                Return ResourceManager.GetString("lblt_RFC", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Subtotal.
        '''</summary>
        Public Shared ReadOnly Property lblt_Subtotal() As String
            Get
                Return ResourceManager.GetString("lblt_Subtotal", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Tipo Cliente.
        '''</summary>
        Public Shared ReadOnly Property lblt_TipoCliente() As String
            Get
                Return ResourceManager.GetString("lblt_TipoCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Clientes.
        '''</summary>
        Public Shared ReadOnly Property Mn_Clientes() As String
            Get
                Return ResourceManager.GetString("Mn_Clientes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Configuración.
        '''</summary>
        Public Shared ReadOnly Property Mn_Configuraciones() As String
            Get
                Return ResourceManager.GetString("Mn_Configuraciones", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Ordenes de Venta.
        '''</summary>
        Public Shared ReadOnly Property Mn_OrdenVenta() As String
            Get
                Return ResourceManager.GetString("Mn_OrdenVenta", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Aplicaciones de Pagos.
        '''</summary>
        Public Shared ReadOnly Property Mn_Pagos() As String
            Get
                Return ResourceManager.GetString("Mn_Pagos", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios.
        '''</summary>
        Public Shared ReadOnly Property Mn_Usuarios() As String
            Get
                Return ResourceManager.GetString("Mn_Usuarios", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Activos....
        '''</summary>
        Public Shared ReadOnly Property msg_Usuarios_Activos() As String
            Get
                Return ResourceManager.GetString("msg_Usuarios_Activos", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Desactivados....
        '''</summary>
        Public Shared ReadOnly Property msg_Usuarios_Desactivados() As String
            Get
                Return ResourceManager.GetString("msg_Usuarios_Desactivados", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios Sin Confirmar....
        '''</summary>
        Public Shared ReadOnly Property msg_Usuarios_SinCon() As String
            Get
                Return ResourceManager.GetString("msg_Usuarios_SinCon", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Todos los Usuarios....
        '''</summary>
        Public Shared ReadOnly Property msg_Usuarios_Todos() As String
            Get
                Return ResourceManager.GetString("msg_Usuarios_Todos", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a -- Clave de Producto --.
        '''</summary>
        Public Shared ReadOnly Property ph_ClaveProducto() As String
            Get
                Return ResourceManager.GetString("ph_ClaveProducto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a -- Nombre del Producto --.
        '''</summary>
        Public Shared ReadOnly Property ph_NombreProducto() As String
            Get
                Return ResourceManager.GetString("ph_NombreProducto", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Sincronizando Clientes con Netsuite....
        '''</summary>
        Public Shared ReadOnly Property SL_SyncCustomer() As String
            Get
                Return ResourceManager.GetString("SL_SyncCustomer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Registrar Pago Por Cliente.
        '''</summary>
        Public Shared ReadOnly Property Smn_ApliPago() As String
            Get
                Return ResourceManager.GetString("Smn_ApliPago", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consultar Clientes.
        '''</summary>
        Public Shared ReadOnly Property Smn_ConCliente() As String
            Get
                Return ResourceManager.GetString("Smn_ConCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consultar Estatus Netsuite.
        '''</summary>
        Public Shared ReadOnly Property Smn_ConNetEstatus() As String
            Get
                Return ResourceManager.GetString("Smn_ConNetEstatus", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consultar Ordenes de Venta.
        '''</summary>
        Public Shared ReadOnly Property Smn_ConOV() As String
            Get
                Return ResourceManager.GetString("Smn_ConOV", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consultar Parametros.
        '''</summary>
        Public Shared ReadOnly Property Smn_ConParametros() As String
            Get
                Return ResourceManager.GetString("Smn_ConParametros", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consultar Usuarios.
        '''</summary>
        Public Shared ReadOnly Property Smn_ConUsuario() As String
            Get
                Return ResourceManager.GetString("Smn_ConUsuario", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Registrar Cliente.
        '''</summary>
        Public Shared ReadOnly Property Smn_crearCliente() As String
            Get
                Return ResourceManager.GetString("Smn_crearCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Ordenes de Venta.
        '''</summary>
        Public Shared ReadOnly Property Smn_CrearOV() As String
            Get
                Return ResourceManager.GetString("Smn_CrearOV", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Nuevo Usuario.
        '''</summary>
        Public Shared ReadOnly Property Smn_CrearUsuario() As String
            Get
                Return ResourceManager.GetString("Smn_CrearUsuario", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Activo.
        '''</summary>
        Public Shared ReadOnly Property sts_Usuario_Activo() As String
            Get
                Return ResourceManager.GetString("sts_Usuario_Activo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Inactivo.
        '''</summary>
        Public Shared ReadOnly Property sts_Usuario_Desactivados() As String
            Get
                Return ResourceManager.GetString("sts_Usuario_Desactivados", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Clientes.
        '''</summary>
        Public Shared ReadOnly Property SubPage_Cliente() As String
            Get
                Return ResourceManager.GetString("SubPage_Cliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Estimación.
        '''</summary>
        Public Shared ReadOnly Property SubPage_Estimacion() As String
            Get
                Return ResourceManager.GetString("SubPage_Estimacion", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Negadas.
        '''</summary>
        Public Shared ReadOnly Property SubPage_Negadas() As String
            Get
                Return ResourceManager.GetString("SubPage_Negadas", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Orden de Venta.
        '''</summary>
        Public Shared ReadOnly Property SubPage_OV() As String
            Get
                Return ResourceManager.GetString("SubPage_OV", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Usuarios.
        '''</summary>
        Public Shared ReadOnly Property SubPage_Users() As String
            Get
                Return ResourceManager.GetString("SubPage_Users", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Orden de Venta.
        '''</summary>
        Public Shared ReadOnly Property Title_CrearOV() As String
            Get
                Return ResourceManager.GetString("Title_CrearOV", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Estimaciones.
        '''</summary>
        Public Shared ReadOnly Property Title_Estimaciones() As String
            Get
                Return ResourceManager.GetString("Title_Estimaciones", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Negadas.
        '''</summary>
        Public Shared ReadOnly Property Title_Negadas() As String
            Get
                Return ResourceManager.GetString("Title_Negadas", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consulta de Clientes.
        '''</summary>
        Public Shared ReadOnly Property Tittle_ConsultaCliente() As String
            Get
                Return ResourceManager.GetString("Tittle_ConsultaCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Cliente.
        '''</summary>
        Public Shared ReadOnly Property Tittle_CreateCliente() As String
            Get
                Return ResourceManager.GetString("Tittle_CreateCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Crear Usuarios.
        '''</summary>
        Public Shared ReadOnly Property Tittle_CreateUser() As String
            Get
                Return ResourceManager.GetString("Tittle_CreateUser", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Datos Principales.
        '''</summary>
        Public Shared ReadOnly Property Tittle_RegCliente() As String
            Get
                Return ResourceManager.GetString("Tittle_RegCliente", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Datos Principales.
        '''</summary>
        Public Shared ReadOnly Property Tittle_RegUser() As String
            Get
                Return ResourceManager.GetString("Tittle_RegUser", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Consulta de Usuarios.
        '''</summary>
        Public Shared ReadOnly Property Tittle_SearchUsers() As String
            Get
                Return ResourceManager.GetString("Tittle_SearchUsers", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Articulos.
        '''</summary>
        Public Shared ReadOnly Property WiS_Articulos() As String
            Get
                Return ResourceManager.GetString("WiS_Articulos", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Busca una cadena traducida similar a Confirmación de Pedido.
        '''</summary>
        Public Shared ReadOnly Property WiS_ConfirmPedido() As String
            Get
                Return ResourceManager.GetString("WiS_ConfirmPedido", resourceCulture)
            End Get
        End Property
    End Class
End Namespace