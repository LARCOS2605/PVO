@imports RecepcionFacturas
@Code
    ViewData("Title") = "Información General del Perfil"
    Dim usuario As Usuarios = ViewBag.usuario
    ViewData("SubPage") = "Usuarios"

    Dim has As New OC.Core.Crypto.Hash

    Dim texto As String = "usuarios-" + usuario.idUsuario.ToString()
    Dim clave = has.Sha256(texto).ToLower
End Code

<!-- Modal Reset -->
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
                        <label for="exampleInputEmail1">Contraseña Actual:</label>
                        <input type="password" class="form-control" id="ContraActual" aria-describedby="emailHelp">
                    </div>
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
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                <button type="button" onclick="CambiarContrasena()" class="btn btn-primary">Guardar Cambios</button>
            </div>
            <div style="display:none;">
                @Using Html.BeginForm("ResetPassword", "Account", FormMethod.Post, New With {.id = "CambiarContrasena"})
                    @Html.AntiForgeryToken()

                    @<input type="hidden" name="OldPassword" id="create-OldPassword" />
                    @<input type="hidden" name="NewPassword" id="create-NewPassword" />
                    @<input type="hidden" name="ConfirmPassword" id="create-ConfirmPassword" />

                End Using
            </div>
        </div>
    </div>
</div>

<!-- Modal  -->
<div class="modal fade" id="ModalInfo">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Actualizar Información.</h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body">
                <form>
                    <div class="form-group">
                        <label for="exampleInputEmail1">Nombre:</label>
                        <input type="text" class="form-control" id="Consulta-Nombre" aria-describedby="emailHelp">
                    </div>
                    <div class="form-group">
                        <label for="exampleInputEmail1">Información de Contacto:</label>
                        <textarea class="form-control" type="a" id="Consulta-Contacto"></textarea>
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
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                <button type="button" onclick="GuardarInformacion('@clave')" class="btn btn-primary">Guardar Cambios</button>
            </div>
            <div style="display:none;">
                @Using Html.BeginForm("CambiarInformacionPrincipal", "Account", FormMethod.Post, New With {.id = "Cambiarinformacion"})
                    @Html.AntiForgeryToken()

                    @<input type="hidden" name="Nombre" id="create-Nombre" />
                    @<input type="hidden" name="InformacionContacto" id="create-InformacionContacto" />
                    @<input type="hidden" name="Lenguaje" id="create-Lenguaje" />

                End Using
            </div>
        </div>
    </div>
</div>

<div class="card">
    <div class="card-body">
        <header><h4 class="header-title">@ViewData("Title")</h4></header>
        <div class="table-responsive">
            <table class="table align-items-center table-flush nowrap">
                <tr>
                    <td style="width:350px">Nombre de Usuario:</td>
                    <td><strong> @usuario.Nombre </strong></td>
                </tr>
                <tr>
                    <td style="width:350px">Correo Electronico:</td>
                    <td><strong> @usuario.Correo </strong></td>
                </tr>
                <tr>
                    <td style="width:350px">Información de Contacto:</td>
                    <td><strong> @usuario.InformacionContacto </strong></td>
                </tr>
                <tr>
                    <td style="width:350px">Fecha de Registro: <strong> @WebSecurity.GetCreateDate(usuario.Correo).ToShortDateString</strong> </td>
                    <td>Fecha Ultima Modificación: <strong> @WebSecurity.GetPasswordChangedDate(usuario.Correo).ToShortDateString </strong></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="card-footer">
        <a onclick="EditarInformacion('@clave')" class="btn btn-info">
            <span class="fa fa-edit btn_descarga"></span> &nbsp Actualizar datos
        </a>
        <a onclick="ShowContrasena()" class="btn btn-info">
            <span class="fa fa-key btn_descarga"></span> &nbsp Cambiar Contraseña
        </a>
    </div>
</div>


@Section Scripts

    <script>

        function ShowContrasena() {
            $('#ContraActual').val("");
            $('#NuevaContra').val("");
            $('#ConfirmContra').val("");
            $('#ModalResetPassword').modal('toggle');
        }

        function CambiarContrasena() {

            var form = $("#CambiarContrasena");

            if ($('#ContraActual').val() == "") {
                toastr.error('El Campo "Contraseña Actual" no puede ir vacio.');
                return false
            }

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

            var ContraseñaActual = $('#ContraActual').val();
            var NuevaContraseña = $('#NuevaContra').val();
            var ConfirmarContraseña = $('#ConfirmContra').val();

            form.children("[name='OldPassword']").val(ContraseñaActual);
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

        function EditarInformacion(id) {
            var Url_Datos = "@Url.Action("ConsultarInfoUsuario", "Account", New With {.id = "__ID__"})"

            var link = Url_Datos.replace("__ID__",id)

            jQuery.ajax({
                url: link,
                cache: false,
                contentType: false,
                processData: false,
                type: 'POST',
                success: function (data) {
                    if (data.Tipo == 1) {
                        $('#Consulta-Nombre').val(data.Valor.Nombre);
                        $('#Consulta-Contacto').val(data.Valor.InformacionContacto);
                        $('#IdiomaSelect').val(data.Valor.Lenguaje);
                        $('#ModalInfo').modal('toggle');
                    } else {
                        toastr.error("Hubo un problema al consultar la información. " + data.Mensaje);
                    }
                }
            });

        }

        function GuardarInformacion() {

            var form = $("#Cambiarinformacion");

            if ($('#Consulta-Nombre').val() == "") {
                toastr.error('El Campo "Nombre" no puede ir vacio.');
                return false
            }

            if ($('#IdiomaSelect').val() == "") {
                toastr.error('El Combo "Idioma" no puede ir vacio.');
                return false
            }

            var Nombre = $('#Consulta-Nombre').val();
            var InfoContacto = $('#Consulta-Contacto').val();
            var Idiomas = $('#IdiomaSelect').val();

            form.children("[name='Nombre']").val(Nombre);
            form.children("[name='InformacionContacto']").val(InfoContacto);
            form.children("[name='Lenguaje']").val(Idiomas);

            var dataForm = form.serialize();
            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                //console.log(data);
                //if (data.Tipo == 1) {

                //    $('#ContraActual').val("");
                //    $('#NuevaContra').val("");
                //    $('#ConfirmContra').val("");
                //    $('#ModalResetPassword').modal('toggle');

                //    toastr.success('La Contraseña fue cambiada con exito...');

                //}
                //else {
                //    toastr.error('Hubo un problema al cambiar su contraseña, intentelo más tarde...');
                //}
            });
        }

    </script>
End Section