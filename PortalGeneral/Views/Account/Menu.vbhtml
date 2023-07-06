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

                @If User.IsInRole("SuperAdmin") Or User.IsInRole("Vendedor") Or User.IsInRole("Regional") Or User.IsInRole("CxC") Then
                    @<li>
                        <a href="javascript:void(0)" aria-expanded="true"><i class="ti-agenda"></i><span>@Idiomas.My.Resources.Resource.Mn_Clientes</span></a>
                        <ul class="collapse">
                            <li @ViewData("I_CC")><a href="@Url.Action("CrearCliente", "Clientes")">@Idiomas.My.Resources.Resource.Smn_crearCliente</a></li>
                            <li @ViewData("I_CoC")><a href="@Url.Action("ConsultarClientes", "Clientes")">@Idiomas.My.Resources.Resource.Smn_ConCliente</a></li>
                        </ul>
                    </li>
                End If

                @If User.IsInRole("SuperAdmin") Or User.IsInRole("Vendedor") Or User.IsInRole("Regional") Then
                    @<li>
                        <a href="javascript:void(0)" aria-expanded="true"><i class="ti-shopping-cart"></i><span>@Idiomas.My.Resources.Resource.Mn_OrdenVenta</span></a>
                        <ul class="collapse">
                            <li @ViewData("I_SO")><a href="@Url.Action("CrearOrdenDeVenta", "OrdenesVenta")">@Idiomas.My.Resources.Resource.Smn_CrearOV</a></li>
                            <li @ViewData("I_SOCon")><a href="@Url.Action("ConsultarOrdenDeVenta", "OrdenesVenta")">@Idiomas.My.Resources.Resource.Smn_ConOV</a></li>
                        </ul>
                    </li>
                End If

                @If User.IsInRole("SuperAdmin") Or User.IsInRole("Cajero") Or User.IsInRole("Regional") Then
                    @<li>
                        <a href="javascript:void(0)" aria-expanded="true"><i class="ti-money"></i><span>@Idiomas.My.Resources.Resource.Mn_Pagos</span></a>
                        <ul class="collapse">
                            <li @ViewData("I_PGP")><a href="@Url.Action("GenerarPagos", "Pagos")">@Idiomas.My.Resources.Resource.Smn_ApliPago</a></li>
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

            </ul>
        </nav>
    </div>
</div>