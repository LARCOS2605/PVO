@Imports PortalGeneral
@Imports MvcFlash.Core.Extensions
@code

    If IsNothing(System.Web.HttpContext.Current.Session("Nombre")) Then

    End If

    If Request.IsAuthenticated Then
        If User.Identity.IsAuthenticated And System.Web.HttpContext.Current.Session("Nombre") Is Nothing Then
            @Html.Action("SetCurrentUser", "Account")
        End If
    End If

    Dim db As New PVO_NetsuiteEntities
    Dim Select_Ubicacion As New List(Of SelectListItem)
    Dim l_Mostradores As New List(Of Ubicaciones)
    Dim Usuario As New Usuarios

    If WebSecurity.IsAuthenticated Then
        If User.Identity.IsAuthenticated Then
            Usuario = (From n In db.Usuarios Where n.idUsuario = WebSecurity.CurrentUserId).FirstOrDefault()
        End If
    End If

    If System.Web.HttpContext.Current.Session("L_Mostradores") Is Nothing Then

        If User.IsInRole("SuperAdmin") Or User.IsInRole("CxC") Then
            l_Mostradores = (From n In db.Ubicaciones).ToList()
        ElseIf User.IsInRole("Cajero") Or User.IsInRole("Vendedor") Or User.IsInRole("Regional") Then
            l_Mostradores = (From n In db.UsuarioUbicaciones Where n.UserId = WebSecurity.CurrentUserId Select n.Ubicaciones).ToList()
        End If

        System.Web.HttpContext.Current.Session("L_Mostradores") = l_Mostradores
        ViewBag.MostradoresDisponibles = System.Web.HttpContext.Current.Session("L_Mostradores")
    Else
        ViewBag.MostradoresDisponibles = System.Web.HttpContext.Current.Session("L_Mostradores")
    End If

    If System.Web.HttpContext.Current.Session("Validado_User") Is Nothing Then
        If IsNothing(Usuario.MostradorPref) Then
            ViewBag.ValidaMostrador = "False"
        Else
            ViewBag.ValidaMostrador = "True"
            System.Web.HttpContext.Current.Session("Validado_User") = "True"
        End If
    End If

    If System.Web.HttpContext.Current.Session("ValidaNombreMostrador") Is Nothing Then

        If Usuario.MostradorPref Is Nothing Then
            System.Web.HttpContext.Current.Session("ValidaNombreMostrador") = "No Localizado"
        Else
            Dim ValidaMostNombre = (From n In db.Ubicaciones Where n.idUbicacion = Usuario.MostradorPref Select n.DescripcionAlmacen).FirstOrDefault()

            If Not IsNothing(ValidaMostNombre) Then
                System.Web.HttpContext.Current.Session("ValidaNombreMostrador") = ValidaMostNombre
            Else
                System.Web.HttpContext.Current.Session("ValidaNombreMostrador") = "No Localizado"
            End If
        End If

    End If

    If System.Web.HttpContext.Current.Session("Ambiente") Is Nothing Then

        Dim Ambiente = (From n In db.Parametros Where n.Clave = "Ambiente").FirstOrDefault()

        If Ambiente Is Nothing Then
            System.Web.HttpContext.Current.Session("Ambiente") = "No Localizado"
        Else

            If Ambiente.Valor = "QA" Then
                System.Web.HttpContext.Current.Session("Ambiente") = "Ambiente de Pruebas - SandBox"
            End If
        End If

    End If

End Code

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="Cache-Control" content="no-store, no-cache, must-revalidate" />
    <link rel="shortcut icon" type="image/png" href="~/assets/images/favicon.ico">
    <link rel="stylesheet" href="~/assets/css/bootstrap.min.css">
    <link rel="stylesheet" href="~/assets/css/font-awesome.min.css">
    <link rel="stylesheet" href="~/assets/css/themify-icons.css">
    <link rel="stylesheet" href="~/assets/css/metisMenu.css">
    <link rel="stylesheet" href="~/assets/css/owl.carousel.min.css">
    <link rel="stylesheet" href="~/assets/css/slicknav.min.css">
    <link rel="stylesheet" href="~/Content/Loading.css">
    <!-- amchart css -->
    <link rel="stylesheet" href="https://www.amcharts.com/lib/3/plugins/export/export.css" type="text/css" media="all" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.css" type="text/css" media="all" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css" type="text/css" media="all" />
    <!-- Start datatable css -->
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css">
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.18/css/dataTables.bootstrap4.min.css">
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/responsive/2.2.3/css/responsive.bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/responsive/2.2.3/css/responsive.jqueryui.min.css">
    <!-- others css -->
    <link rel="stylesheet" href="~/assets/css/typography.css">
    <link rel="stylesheet" href="~/assets/css/default-css.css">
    <link rel="stylesheet" href="~/assets/css/styles.css">
    <link rel="stylesheet" href="~/assets/css/responsive.css">
    <link rel="stylesheet" href="~/Content/chosen.css">
    <link rel="stylesheet" href="~/Content/chosen.min.css">
    <!-- modernizr css -->
    <script src="~/assets/js/vendor/modernizr-2.8.3.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/css/select2.min.css" rel="stylesheet" />
</head>


<body>
    <div id="preloader">
        <div class="loader"></div>
    </div>

    <div class="page-container">

        <!-- sidebar menu area start -->
        <div class="sidebar-menu">
            <div class="sidebar-header">
                <div class="logo">
                    <a href="https://www.muellesmaf.com.mx/"><img src="~/Assets/images/logo.png" alt="logo"></a>
                </div>
            </div>
            <div class="main-menu">
                <div class="menu-inner">
                    <nav>
                        <ul class="metismenu" id="menu">

                            @If User.IsInRole("SuperAdmin") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-user"></i><span>@Idiomas.My.Resources.Resource.Mn_Usuarios</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_CU")><a href="@Url.Action("CrearUsuario", "Usuarios")">@Idiomas.My.Resources.Resource.Smn_CrearUsuario</a></li>
                                        <li @ViewData("I_ConUs")><a href="@Url.Action("ConsultarUsuarios", "Usuarios")">@Idiomas.My.Resources.Resource.Smn_ConUsuario</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Vendedor") Or User.IsInRole("Regional") Or User.IsInRole("CxC") Or User.IsInRole("Cajero") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-agenda"></i><span>@Idiomas.My.Resources.Resource.Mn_Clientes</span></a>
                                    <ul class="collapse">

                                        <li @ViewData("I_CC")><a href="@Url.Action("CrearVistaCliente", "Clientes")">@Idiomas.My.Resources.Resource.Smn_crearCliente</a></li>
                                        @*<li @ViewData("I_CC")><a href="@Url.Action("CrearCliente", "Clientes")">@Idiomas.My.Resources.Resource.Smn_crearCliente (Formulario web)</a></li>*@


                                        <li @ViewData("I_CoC")><a href="@Url.Action("ConsultarClientes", "Clientes")">@Idiomas.My.Resources.Resource.Smn_ConCliente</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Vendedor") Or User.IsInRole("Regional") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-shopping-cart"></i><span>@Idiomas.My.Resources.Resource.Mn_OrdenVenta</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_SO")><a href="@Url.Action("CrearOrdenDeVenta", "OrdenesVenta")">@Idiomas.My.Resources.Resource.Smn_CrearOV</a></li>
                                        <li @ViewData("I_SOCon")><a href="@Url.Action("ConsultarOrdenVenta", "OrdenesVenta")">@Idiomas.My.Resources.Resource.Smn_ConOV</a></li>
                                        @*<li @ViewData("I_INVCon")><a href="@Url.Action("ConsultarOrdenDeVenta", "OrdenesVenta")">Gestionar Facturas</a></li>*@
                                    </ul>
                                </li>
                            End If
                            @If User.IsInRole("SuperAdmin")  Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-file"></i><span>Estimaciones</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_Est")><a href="@Url.Action("CrearEstimacion", "Estimaciones")">Crear Estimación</a></li>
                                        <li @ViewData("I_EstCon")><a href="@Url.Action("ConsultarEstimacion", "Estimaciones")">Consultar Estimación</a></li>
                                        @*<li @ViewData("I_INVCon")><a href="@Url.Action("ConsultarOrdenDeVenta", "OrdenesVenta")">Gestionar Facturas</a></li>*@
                                    </ul>
                                </li>
                            End If
                            @If User.IsInRole("SuperAdmin") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-close"></i><span>Negadas</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_Neg")><a href="@Url.Action("CrearNegadas", "Negadas")">Crear Negada</a></li>
                                        <li @ViewData("I_NegCon")><a href="@Url.Action("ConsultarNegadas", "Negadas")">Consultar Negadas</a></li>
                                        @*<li @ViewData("I_INVCon")><a href="@Url.Action("ConsultarOrdenDeVenta", "OrdenesVenta")">Gestionar Facturas</a></li>*@
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("CxC") Or User.IsInRole("Vendedor") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-bag"></i><span>Reportes</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_RIG")><a href="@Url.Action("GenerarReporteIngresosGeneral", "CorteCaja")">Reporte de Ingresos Generales</a></li>
                                        <li @ViewData("I_RCC")><a href="@Url.Action("GenerarReporteCorte", "CorteCaja")">Reporte de Corte de Caja</a></li>
                                        @*<li @ViewData("I_RKA")><a href="@Url.Action("GenerarReporteKardexArticulo", "CorteCaja")">Reporte de Historial de Articulo</a></li>*@
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("Vendedor") Or User.IsInRole("CxC") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-layers-alt"></i><span>PVO vs Netsuite</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_PNCF")><a href="@Url.Action("ConsultarFacturasNetsuite", "PVONetSuite")">Consultar Facturas Netsuite</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("Vendedor") Or User.IsInRole("CxC") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-list"></i><span>Inventario</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_CID")><a href="@Url.Action("ConsultarInventarioDisponible", "Inventario")">Consultar Inventario Disponible</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("Vendedor") Or User.IsInRole("CxC") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-truck"></i><span>Solicitud MRP</span></a>
                                    <ul class="collapse">
                                        @*<li @ViewData("I_GST")><a href="@Url.Action("GenerarSolicitudMRP", "Traslados")">Crear Solicitud MRP</a></li>*@
                                        <li @ViewData("I_GST")><a href="@Url.Action("GenerarAprobacionMRP", "Traslados")">Generar Recepcion de MRP</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("CxC") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-money"></i><span>@Idiomas.My.Resources.Resource.Mn_Pagos</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_PGP")><a href="@Url.Action("GenerarPagos", "Pagos")">@Idiomas.My.Resources.Resource.Smn_ApliPago</a></li>
                                        <li @ViewData("I_CPA")><a href="@Url.Action("ConsultarPagosAplicados", "Pagos")">Consultar Pagos Aplicados</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-settings"></i><span>@Idiomas.My.Resources.Resource.Mn_Configuraciones</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_CP")><a href="@Url.Action("ConsultarParametros", "Parametros")">@Idiomas.My.Resources.Resource.Smn_ConParametros</a></li>
                                        <li @ViewData("I_EN")><a href="@Url.Action("ConsultarEstatusNetsuite", "Parametros")">@Idiomas.My.Resources.Resource.Smn_ConNetEstatus</a></li>
                                    </ul>
                                </li>
                            End If

                            @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Or User.IsInRole("CxC") Or User.IsInRole("Vendedor") Then
                                @<li>
                                    <a href="javascript:void(0)" aria-expanded="true"><i class="ti-info"></i><span>Manuales de Usuario</span></a>
                                    <ul class="collapse">
                                        <li @ViewData("I_RIG")><a href="@Url.Action("DescargarManualAltaClientes", "Parametros")">Manual Alta de Clientes</a></li>
                                    </ul>
                                </li>
                            End If

                        </ul>
                    </nav>
                </div>
            </div>
        </div>
        <!-- sidebar menu area end -->
        <!-- main content area start -->
        <div class="main-content">
            <!-- header area start -->
            <div class="header-area">
                <div class="container">
                    @Html.Flash()
                </div>
                <div class="row align-items-center">
                    <!-- nav and search button -->
                    <div class="col-md-6 col-sm-8 clearfix">
                        <div class="nav-btn pull-left">
                            <span></span>
                            <span></span>
                            <span></span>
                        </div>
                        @*<div class="search-box pull-left">
                                <form action="#">
                                    <input type="text" name="search" placeholder="Search..." required>
                                    <i class="ti-search"></i>
                                </form>
                            </div>*@
                    </div>
                    <!-- profile info & task notification -->
                    <div class="col-md-6 col-sm-4 clearfix">
                        <ul class="notification-area pull-right">
                            <li id="full-view"><i class="ti-fullscreen"></i></li>
                            <li id="full-view-exit"><i class="ti-zoom-out"></i></li>
                            @*<li class="" id="full-view"><i class="ti-home"></i></li>
                                <li class="full-view-exit"><i class="ti-home"></i></li>*@
                            <li class="home-btn"><i class="ti-home"></i></li>
                            <li class="settings-btn"><i class="ti-settings"></i></li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- header area end -->
            <!-- page title area start -->
            <div class="page-title-area">
                <div class="row align-items-center">
                    <div class="col-sm-6">
                        <div class="breadcrumbs-area clearfix">
                            @If Not IsNothing(System.Web.HttpContext.Current.Session("Ambiente")) Then
                                @<h5>@System.Web.HttpContext.Current.Session("Ambiente")</h5>
                            End If
                            <h4 id="title_M" class="page-title pull-left">Punto de Ventas : Mostrador @System.Web.HttpContext.Current.Session("ValidaNombreMostrador")</h4>
                            <ul class="breadcrumbs pull-left">
                                <li><a href="@Url.Action("index", "home")">@ViewData("SubPage")</a></li>
                                <li><span>@ViewData("Title")</span></li>
                            </ul>
                        </div>
                    </div>
                    <div class="col-sm-6 clearfix">
                        <div class="user-profile pull-right">
                            <img class="avatar user-thumb" src="~/assets/images/author/avatar.png" alt="avatar">
                            <h4 class="user-name dropdown-toggle" data-toggle="dropdown">@Idiomas.My.Resources.Resource.lbl_Prueba! @System.Web.HttpContext.Current.Session("Nombre")<i class="fa fa-angle-down"></i></h4>
                            <div class="dropdown-menu">
                                <a class="dropdown-item" href="@Url.Action("UserStats", "Account")">Mi Información</a>
                                <a class="dropdown-item" href="@Url.Action("CerrarSesion", "Account")">Cerrar Sesión</a>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
            <!-- page title area end -->
            <div class="main-content-inner">
                <!-- sales report area start -->
                <div class="col-12 mt-5">
                    @RenderBody()
                    <hr />
                </div>
            </div>
        </div>
        <!-- main content area end -->
        <!-- footer area start-->
        <footer>
            <div class="footer-area">
                <p>&copy; @DateTime.Now.Year - Alvarez Automotriz SA de CV</p>
            </div>
        </footer>
        <!-- footer area end-->
    </div>

    <!-- Validar Si el usuario ya seleccionó un mostrador -->
    <div class="modal fade" id="ModalSelection">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Seleccionar Un Mostrador</h5>
                    <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                </div>
                <div class="modal-body">
                    <form>
                        <p>Para poder usar algunas de las opciones del portal, es necesario seleccionar un mostrador.</p>
                        <br />
                        <h5>Cambiar Mostrador</h5>
                        <select id="Mostradores_Select" class="form-control">
                            <option> --- Seleccione un Mostrador --- </option>
                            @For Each RegMost As Ubicaciones In ViewBag.MostradoresDisponibles
                                @<option id="@RegMost.idUbicacion">@RegMost.DescripcionAlmacen</option>
                            Next
                        </select>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    <button type="button" onclick="AlmacenarMostradorInicial()" class="btn btn-primary">Guardar Mostrador</button>
                </div>
            </div>
        </div>
    </div>

    <div class="offset-area">
        <div class="offset-close"><i class="ti-close"></i></div>
        <ul class="nav offset-menu-tab">
            <li><a class="active" data-toggle="tab" href="#activity">Configuraciones</a></li>
            <li><a data-toggle="tab" href="#settings">Preferencias</a></li>
        </ul>
        <div class="offset-content tab-content">
            <div id="activity" class="tab-pane fade in show active">
                <div class="offset-settings">
                    <h4>Configuraciones Generales</h4>
                    <div class="settings-list">
                        <div class="s-settings">
                            <div class="s-sw-title">
                                <div class="form-group">
                                    <h5>Cambiar Mostrador</h5>
                                    <select id="Mostradores_Select_config" class="form-control">
                                        <option> --- Seleccione un Mostrador --- </option>
                                        @For Each RegMost As Ubicaciones In ViewBag.MostradoresDisponibles
                                            @<option value="@RegMost.idUbicacion">@RegMost.DescripcionAlmacen</option>
                                        Next
                                    </select>
                                    <br />
                                    <button type="button" onclick="ChangeMostradorConfig()" class="btn btn-outline-primary mb-3">Cambiar Mostrador</button>

                                    <p>Si usted cuenta con más de un mostrador, puede cambiar su localización desde este apartado.</p>

                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="settings" Class="tab-pane fade">

            </div>
        </div>
    </div>

    <div style="display:none;">
        @Using Html.BeginForm("SeleccionarMostradorPred", "Account", FormMethod.Post, New With {.id = "ActualizarMostrador"})
            @Html.AntiForgeryToken()

            @<input type="hidden" name="id" id="create-id" />

        End Using
    </div>

    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/TemplateJS")
    <!-- Start datatable js -->
    <script src="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js"></script>
    <script src="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.18/js/dataTables.bootstrap4.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.2.3/js/dataTables.responsive.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.2.3/js/responsive.bootstrap.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.2.3/js/dataTables.buttons.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.2.3/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.2.3/js/buttons.print.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/js/select2.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/inputmask/4.0.9/jquery.inputmask.bundle.min.js"></script>
    @*<script src="https://cdnjs.cloudflare.com/ajax/libs/inputmask/4.0.9/jquery.inputmask.bundle.min.js"></script>*@
    @Scripts.Render("~/bundles/BundSJ")
    @Scripts.Render("~/bundles/modernizr")

    @*@Scripts.Render("~/bundles/bootstrap")*@
    @RenderSection("scripts", required:=False)
</body>
</html>

@Section Scripts

    <script>
        var TipoConsulta = "@Html.Raw(ViewBag.ValidaMostrador)";

        $(document).ready(function () {

            if (TipoConsulta == "False") {
                $('#ModalSelection').modal('toggle');
            }

        });

        $(document).ready(function () {
            setTimeout(function () {
                $('#successMessage').fadeOut('fast');
            }, 10000);
        });

        //document.addEventListener('contextmenu', (e) => e.preventDefault());

        //function ctrlShiftKey(e, keyCode) {
        //    return e.ctrlKey && e.shiftKey && e.keyCode === keyCode.charCodeAt(0);
        //}

        //document.onkeydown = (e) => {
        //    // Disable F12, Ctrl + Shift + I, Ctrl + Shift + J, Ctrl + U
        //    if (
        //        event.keyCode === 123 ||
        //        ctrlShiftKey(e, 'I') ||
        //        ctrlShiftKey(e, 'J') ||
        //        ctrlShiftKey(e, 'C') ||
        //        (e.ctrlKey && e.keyCode === 'U'.charCodeAt(0))
        //    )
        //        return false;
        //};

    </script>
End Section
