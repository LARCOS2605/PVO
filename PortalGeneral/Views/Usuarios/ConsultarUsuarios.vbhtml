@Code
    ViewData("Title") = Idiomas.My.Resources.Resource.Tittle_SearchUsers
    ViewData("I_ConUs") = "class=active"
    ViewData("SubPage") = Idiomas.My.Resources.Resource.SubPage_Users
End Code


<div class="card">
    <div class="card-body">
        <header><h4 class="header-title">@ViewData("Title")</h4></header>
        <div class="col-lg-12 mt-5">
            <button type="button" onclick="ConsultarUsuariosActivos()" class="btn btn-rounded btn-success mb-3"><i class="fa fa-user"></i> @Idiomas.My.Resources.Resource.btn_UsuarioActivo</button>
            <button type="button" onclick="ConsultarTodosDesactivados()" class="btn btn-rounded btn-danger mb-3"><i class="fa fa-user-times"></i> @Idiomas.My.Resources.Resource.btn_UsuarioDesac</button>
            <button type="button" onclick="ConsultarSinConfirmar()" class="btn btn-rounded btn-warning mb-3"><i class="fa fa-user"></i> @Idiomas.My.Resources.Resource.btn_UsuarioSinConfir</button>
            <button type="button" onclick="ConsultarTodosUsuarios()" class="btn btn-rounded btn-info mb-3"><i class="fa fa-users"></i> @Idiomas.My.Resources.Resource.btn_UsuarioTodos</button>
        </div>
        <div class="single-table">
            <div class="table-responsive">
                <table id="TableUsuarios" class="table table-hover progress-table text-center">
                    <thead class="text-uppercase">
                        <tr>
                            <th scope="col">@Idiomas.My.Resources.Resource.lblt_Nombre</th>
                            <th scope="col">@Idiomas.My.Resources.Resource.lblt_Correo</th>
                            <th scope="col">Rol de Usuario</th>
                            <th scope="col">@Idiomas.My.Resources.Resource.lblt_Estatus</th>
                            <th scope="col">@Idiomas.My.Resources.Resource.lblt_Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        @For Each item In ViewBag.valor
                            Dim currentItem As Usuarios = item
                            @<tr>
                                <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Nombre)</td>
                                <td>@Html.DisplayFor(Function(modelItem) currentItem.Correo)</td>
                                <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.webpages_Roles.First.Description)</td>
                                <td>
                                    @If Not currentItem.CuentaActiva Then
                                        @<span class="status-p bg-warning">@Idiomas.My.Resources.Resource.sts_Usuario_Desactivados</span>
                                    Else
                                        @<span class="status-p bg-primary">@Idiomas.My.Resources.Resource.sts_Usuario_Activo</span>
                                    End If
                                </td>
                                <td>
                                    <ul class="d-flex justify-content-center">
                                        <li class="mr-3"><a href="@Url.Action("EditarUsuario", Nothing, New With {.id = currentItem.idUsuario}) " class="text-secondary"><i class="fa fa-edit"></i></a></li>
                                        <li onclick="ShowContrasena(@currentItem.idUsuario)" class="mr-3"><a href="#" class="text-secondary"><i class="fa fa-key"></i></a></li>

                                        @If currentItem.CuentaActiva = True Then
                                            @<li onclick="DesactivarUsuario(@currentItem.idUsuario)"><a href="#" class="text-danger"><i class="ti-lock"></i></a></li>
                                        Else
                                            @<li onclick="ActivarUsuario(@currentItem.idUsuario)"><a href="#" class="text-info"><i class="ti-unlock"></i></a></li>
                                        End If
                                    </ul>
                                </td>
                            </tr>
                        Next
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- Modal Editar Usuario -->
    <div class="modal fade" id="ModalEdicion">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Editar Usuario</h5>
                    <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                </div>
                <div class="modal-body">
                    @Using Html.BeginForm("EditarParametros", "Parametros", Nothing, FormMethod.Post, New With {.id = "formUsuario"})
                        @Html.AntiForgeryToken()
                        @Html.ValidationSummary(True)
                        @<form>
                            <div class="form-group">
                                <label for="exampleInputEmail1">Nombre:</label>
                                <input type="text" class="form-control" id="Nombre" aria-describedby="emailHelp">
                            </div>
                            <div class="form-group">
                                <label for="exampleInputPassword1">Correo Electronico:</label>
                                <input type="text" class="form-control" id="Correo">
                                <input style="display:none" type="text" class="form-control" id="idUsuario">
                            </div>
                            <div class="form-group">
                                <label for="disabledSelect">Idioma:</label>
                                <select id="IdiomaSelect" class="form-control">
                                    <option>--- Seleccionar Idioma ---</option>
                                    @For Each Idioma As Catalogo_Idiomas In ViewBag.Lenguaje
                                        @<option value="@Idioma.Clave">@Idioma.Descripcion</option>
                                    Next
                                </select>
                            </div>
                            <div Class="form-group">
                                <Label for="example-email-input" class="col-form-label">Información de Contacto:</Label>
                                <textarea Class="form-control" type="a" id="InfoContacto"></textarea>
                            </div>
                            <div Class="form-group">
                                <Label for="disabledSelect">Rol de Usuario:</Label>
                                <select id="RolSelect" class="form-control">
                                    <option>--- Seleccionar Rol ---</option>
                                    @For Each Rol As webpages_Roles In ViewBag.Roles
                                        @<option value="@Rol.RoleName">@Rol.Description</option>
                                    Next
                                </select>
                            </div>
                        </form>

                    end using
                </div>
                <div Class="modal-footer">
                    <Button type="button" Class="btn btn-secondary" data-dismiss="modal">Cerrar</Button>
                    <Button type="button" id="AlmacenarCambios()" Class="btn btn-primary">Guardar Cambios</Button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="ModalResetPassword">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Cambiar Contraseña</h5>
                    <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                </div>
                <div class="modal-body">
                    <form>
                        <div class="form-group">
                            <label for="exampleInputEmail1">Nueva Contraseña:</label>
                            <input type="password" class="form-control" id="NuevaContra" aria-describedby="emailHelp">
                        </div>
                        <div class="form-group">
                            <label for="exampleInputEmail1">Confirmar Contraseña:</label>
                            <input type="password" class="form-control" id="ConfirmContra" aria-describedby="emailHelp">
                        </div>
                    </form>
                </div>
                <input style="display:none" type="text" class="form-control" id="idUsuario">
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    <button type="button" onclick="CambiarContrasena()" class="btn btn-primary">Guardar Cambios</button>
                </div>
                <div style="display:none;">
                    @Using Html.BeginForm("ResetPasswordAdmin", "Account", FormMethod.Post, New With {.id = "CambiarContrasena"})
                        @Html.AntiForgeryToken()

                        @<input type="hidden" name="NewPassword" id="create-NewPassword" />
                        @<input type="hidden" name="ConfirmPassword" id="create-ConfirmPassword" />
                        @<input type="hidden" name="id" id="create-id" />

                    End Using
                </div>
            </div>
        </div>
    </div>

</div>

@Section Scripts

    <script>
        var tablaDetalle = null;

        $(document).ready(function () {
            tablaDetalle = $("#TableUsuarios").DataTable();
        });

        function ShowContrasena(id) {
            $('#NuevaContra').val("");
            $('#ConfirmContra').val("");
            $('#idUsuario').val(id);
            $('#ModalResetPassword').modal('toggle');
        }

        function CambiarContrasena() {

            var form = $("#CambiarContrasena");

            if ($('#NuevaContra').val() == "") {
                toastr.error('El Campo "Nueva Contraseña" no puede ir vacio.');
                return false
            }

            if ($('#ConfirmContra').val() == "") {
                toastr.error('El Campo "Confirmar Contraseña" no puede ir vacio.');
                return false
            }

            if ($('#NuevaContra').val() !== $('#ConfirmContra').val()) {
                toastr.error('Las contraseñas no coindicen en ambas casillas por favor vuelva a intentarlo.');
                return false
            }

            var NuevaContraseña = $('#NuevaContra').val();
            var ConfirmarContraseña = $('#ConfirmContra').val();
            var id = $('#idUsuario').val();

            form.children("[name='id']").val(id);
            form.children("[name='NewPassword']").val(NuevaContraseña);
            form.children("[name='ConfirmPassword']").val(ConfirmarContraseña);

            var dataForm = form.serialize();
            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                console.log(data);
                if (data.Tipo == 1) {

                    $('#ContraActual').val("");
                    $('#NuevaContra').val("");
                    $('#ConfirmContra').val("");
                    $('#ModalResetPassword').modal('toggle');

                    toastr.success('La Contraseña fue cambiada con exito...');

                }
                else {
                    toastr.error('Hubo un problema al cambiar su contraseña, intentelo más tarde...');
                }
            });
        }

        function ConsultarUsuariosActivos() {

            jQuery.ajax({
                    url: "@Url.Action("ConsultarUsuariosActivos", "Usuarios", Nothing)",
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    "<span class='status-p bg-primary'>Activo</span>",
                                    //"<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario +")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>",
                                    "<ul class='d-flex justify-content-center'><li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>",
                                ]).draw(false);
                            });

                            toastr.success('@Idiomas.My.Resources.Resource.msg_Usuarios_Activos');
                        } else {

                        }
                    }
            });
        }

        function ConsultarTodosUsuarios() {

            jQuery.ajax({
                    url: "@Url.Action("ConsultarUsuariosTotales", "Usuarios", Nothing)",
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                var VarianteEstatus
                                var VarianteAcciones

                                if (value.CuentaActiva == false) {
                                    VarianteEstatus = "<span class='status-p bg-warning'>Inactivo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"
                                    /*VarianteAcciones = "<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"*/
                                } else {
                                    VarianteEstatus = "<span class='status-p bg-primary'>Activo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"

                                    //VarianteAcciones = "<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"
                                }

                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    VarianteEstatus,
                                    VarianteAcciones,
                                ]).draw(false);
                            });

                            toastr.info('@Idiomas.My.Resources.Resource.msg_Usuarios_Todos');
                        } else {

                        }
                    }
                });
        }

        function ConsultarTodosDesactivados() {

            jQuery.ajax({
                    url: "@Url.Action("ConsultarUsuariosDesactivados", "Usuarios", Nothing)",
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    "<span class='status-p bg-warning'>Inactivo</span>",
                                    "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>",
                                    //"<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario +")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>",
                                ]).draw(false);
                            });

                            toastr.error('@Idiomas.My.Resources.Resource.msg_Usuarios_Desactivados');
                        } else {

                        }
                    }
            });
        }

        function ConsultarSinConfirmar() {

            jQuery.ajax({
                    url: "@Url.Action("ConsultarUsuariosSinConfirmar", "Usuarios", Nothing)",
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                var VarianteEstatus
                                var VarianteAcciones

                                if (value.CuentaActiva == false) {
                                    VarianteEstatus = "<span class='status-p bg-warning'>Inactivo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"
                                    //VarianteAcciones = "<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"
                                } else {
                                    VarianteEstatus = "<span class='status-p bg-primary'>Activo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"
                                    //VarianteAcciones = "<ul class='d-flex justify-content-center'><li onclick='EditarUsuario(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"

                                }

                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    VarianteEstatus,
                                    VarianteAcciones,
                                ]).draw(false);
                            });

                            toastr.warning('@Idiomas.My.Resources.Resource.msg_Usuarios_SinCon');
                        } else {

                        }
                    }
                });
        }

        function DesactivarUsuario(id) {
            var Url_Parametros = "@Url.Action("GestionarUsuarios", "Usuarios", New With {.id = "__ID__", .Accion = "__a__"})"

            var link = Url_Parametros.replace("__ID__", id)
            var ulink = ""

            ulink = link.replace("__a__", "Des")

            jQuery.ajax({
                url: ulink,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                var VarianteEstatus
                                var VarianteAcciones

                                if (value.CuentaActiva == false) {
                                    VarianteEstatus = "<span class='status-p bg-warning'>Inactivo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li>  <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"
                                } else {
                                    VarianteEstatus = "<span class='status-p bg-primary'>Activo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li>  <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"
                                }


                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    VarianteEstatus,
                                    VarianteAcciones,
                                ]).draw(false);
                            });

                            toastr.success(data.Mensaje);
                        } else {
                            toastr.error(data.Mensaje);
                        }
                    }
             });

        }

        function ActivarUsuario(id) {
            var Url_Parametros = "@Url.Action("GestionarUsuarios", "Usuarios", New With {.id = "__ID__", .Accion = "__a__"})"

            var link = Url_Parametros.replace("__ID__", id)
            var ulink = ""
                ulink = link.replace("__a__", "Act")

            jQuery.ajax({
                url: ulink,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {

                                var VarianteEstatus
                                var VarianteAcciones

                                if (value.CuentaActiva == false) {
                                    VarianteEstatus = "<span class='status-p bg-warning'>Inactivo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li>  <li onclick='ActivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-info'><i class='ti-unlock'></i></a></li> </ul>"
                                } else {
                                    VarianteEstatus = "<span class='status-p bg-primary'>Activo</span>"
                                    VarianteAcciones = "<ul class='d-flex justify-content-center'> <li class='mr-3'><a href='/Usuarios/EditarUsuario/" + value.idUsuario + "' class='text-secondary'><i class='fa fa-edit'></i></a></li> <li onclick='ShowContrasena(" + value.idUsuario + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-key'></i></a></li>  <li onclick='DesactivarUsuario(" + value.idUsuario + ")'><a href='#' class='text-danger'><i class='ti-lock'></i></a></li> </ul>"
                                }


                                tablaDetalle.row.add([
                                    value.Nombre,
                                    value.Correo,
                                    value.perfil,
                                    VarianteEstatus,
                                    VarianteAcciones,
                                ]).draw(false);
                            });

                            toastr.success(data.Mensaje);
                        } else {
                            toastr.error(data.Mensaje);
                        }
                    }
             });

        }

        function EliminarElemento(id) {
            alert(id);
        }

        function EditarUsuario(id) {
            var URL_Usuarios = "@Url.Action("ConsultarUsuario", "Usuarios", New With {.id = "__ID__"})"

            var link = URL_Usuarios.replace("__ID__",id)

            jQuery.ajax({
                    url: link,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            console.log(data.Valor);
                            $('#Nombre').val(data.Valor.Nombre);
                            $('#Correo').val(data.Valor.Correo);
                            $('#IdiomaSelect').val(data.Valor.Lenguaje);
                            $('#InfoContacto').val(data.Valor.Contacto);
                            $('#RolSelect').val(data.Valor.Rol);
                            $('#ModalEdicion').modal('toggle');
                        } else {

                        }
                    }
                });

        }

    </script>
End Section