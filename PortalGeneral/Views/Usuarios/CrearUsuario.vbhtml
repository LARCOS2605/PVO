@ModelType PortalGeneral.UsuariosViewModel
@Code
    ViewData("Title") = Idiomas.My.Resources.Resource.Tittle_CreateUser
    ViewData("I_CU") = "class=active"
    ViewData("SubPage") = Idiomas.My.Resources.Resource.SubPage_Users
End Code

@Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "formUsuario"})
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(True)

    @<div class="col-lg-12 col-ml-12">
        <div class="col-12 mt-5">
            <div class="card">
                <div class="card-body">
                    <h4 class="header-title">@Idiomas.My.Resources.Resource.Tittle_RegUser</h4>
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Nombre, Idiomas.My.Resources.Resource.lbl_Nombre, New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Nombre, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Nombre)
                    </div>
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Correo, "Usuario:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Correo, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Correo)
                    </div>
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Contrasena, "Contraseña:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Contrasena, New With {.type = "password", .class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Contrasena)
                    </div>
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Lenguaje, Idiomas.My.Resources.Resource.lbl_Idioma, New With {.class = "col-form-label"})
                        @Html.DropDownList("Lenguaje", Nothing, String.Empty, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Lenguaje)
                    </div>
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.InformacionContacto, Idiomas.My.Resources.Resource.lbl_InfoContacto, New With {.class = "col-form-label"})
                        @Html.TextAreaFor(Function(model) model.InformacionContacto, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.InformacionContacto)
                    </div>
                    <div class="form-group" id="Ubicacion_Mostrador">
                        @Html.LabelFor(Function(model) model.Ubicacion, Idiomas.My.Resources.Resource.lbl_Mostrador, New With {.class = "col-form-label"})
                        @Html.DropDownList("Ubicacion", Nothing, Idiomas.My.Resources.Resource.cbb_Ubicacion, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Ubicacion)
                    </div>
                    <div class="form-group">
                        <label for="Mostradortext" id="Mostradortext">@Idiomas.My.Resources.Resource.lbl_Mostrador</label>
                        @Html.DropDownList("Ubicacion_Reg", Nothing, Idiomas.My.Resources.Resource.cbb_Ubicacion, New With {.class = "form-control chosen-select", .multiple = "multiple"})
                        @Html.ValidationMessageFor(Function(model) model.Ubicacion_Reg)
                    </div>
                    <div class="form-group" style="display:none">
                        @Html.LabelFor(Function(model) model.Ubicacion_Select)
                        @Html.TextBoxFor(Function(model) model.Ubicacion_Select, New With {.class = "form-control", .type = "text"})
                    </div>



                </div>
                <div class="card-footer">
                    <footer>
                        <h4 class="header-title">@Idiomas.My.Resources.Resource.lbl_Roles</h4>

                        <form action="#">
                            @For Each items In ViewBag.Roles
                                Dim dataitem As webpages_Roles = items
                                @<div class="custom-control custom-checkbox custom-control-inline">
                                    <input type="checkbox" name="l_Roles" value="@dataitem.RoleName" class="custom-control-input" data-role="@dataitem.RoleId" id="@String.Format("customCheck{0}", dataitem.RoleId)">
                                    <label class="custom-control-label" for="@String.Format("customCheck{0}", dataitem.RoleId)">@dataitem.Description</label>
                                </div>
                            Next
                        </form>
                    </footer>

                    <div class="form-group">
                        <a onclick="Almacenar()" style="color: white;" class="btn btn-primary mt-4 pl-4 pr-4">@Idiomas.My.Resources.Resource.btn_GuardarUsuario</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

End Using

@Section Scripts

    <script>


        $(document).ready(function () {

            $("#Ubicacion_Reg").chosen({
                width: "100%",
                placeholder_text_multiple: "--- Seleccione una Opción ---"
            });

            $("#Ubicacion_Reg").on('change', function (evt, params) {
                var selected = $(this).val();
                if (selected != null) {
                    if (selected.indexOf('29910406') >= 0) {
                        $(this).val('29910406').trigger("chosen:updated");
                    }
                }
            });

            $("#Ubicacion_Mostrador").hide();
            $("#Ubicacion_Reg_chosen").hide();
            $("#Mostradortext").hide();
        });

        function Almacenar() {

            if ($('#Nombre').val() == "") {
                toastr.error('El Campo "Nombre" no puede ir vacio.');
                return false
            }

            if ($('#Correo').val() == "") {
                toastr.error('El Campo "Correo Electronico" no puede ir vacio.');
                return false
            }

            if ($('#Lenguaje').val() == "") {
                toastr.error('El Campo "Idioma" no puede ir vacio.');
                return false
            }

            if ($('#Contrasena').val() == "") {
                toastr.error('El Campo "Contraseña" no puede ir vacio.');
                return false
            }

            if ($('input[name="l_Roles"][value*="Regional"]:checked').length > 0) {
                var seleccion = $("#Ubicacion_Reg").chosen().val();
                var Mostradores = "";

                if (seleccion.length != 0) {
                    $.each(seleccion, function (index, value) {
                        Mostradores = Mostradores + value + "|"
                    });

                    $('#Ubicacion_Select').val(Mostradores);
                } else {
                    toastr.error('No se puede registrar el usuario, debido a que no se ha seleccionado algun mostrador...');
                    return false
                }
            }

            if ($('input[name="l_Roles"][value*="ajero"]:checked').length > 0) {
                var Mostrador = $('#Ubicacion').val();

                if (Mostrador == "") {
                    toastr.error('No se puede registrar el usuario, debido a que no se ha seleccionado algun mostrador...');
                    return false
                }
            }

            if ($('input[name="l_Roles"][value*="endedor"]:checked').length > 0) {
                var Mostrador = $('#Ubicacion').val();

                if (Mostrador == "") {
                    toastr.error('No se puede registrar el usuario, debido a que no se ha seleccionado algun mostrador...');
                    return false
                }
            }

            toastr.info('Registrando Usuario...');
            $('#formUsuario').submit();
        }

        $('input[name="l_Roles"][value*="SuperAdmin"]').change(function () {
            if ($('input[name="l_Roles"][value*="SuperAdmin"]:checked').length > 0) {
                $('input[name="l_Roles"][value*="ajero"]').prop('checked', false);
                $('input[name="l_Roles"][value*="endedor"]').prop('checked', false);
                $('input[name="l_Roles"][value*="Regional"]').prop('checked', false);
                $('input[name="l_Roles"][value*="CxC"]').prop('checked', false);

                $("#Ubicacion_Mostrador").hide();
                $("#Ubicacion_Reg_chosen").hide();
                $("#Mostradortext").hide();
            }
        });

        $('input[name="l_Roles"][value*="CxC"]').change(function () {
            if ($('input[name="l_Roles"][value*="CxC"]:checked').length > 0) {
                $('input[name="l_Roles"][value*="ajero"]').prop('checked', false);
                $('input[name="l_Roles"][value*="endedor"]').prop('checked', false);
                $('input[name="l_Roles"][value*="Regional"]').prop('checked', false);
                $('input[name="l_Roles"][value*="SuperAdmin"]').prop('checked', false);

                $("#Ubicacion_Mostrador").hide();
                $("#Ubicacion_Reg_chosen").hide();
                $("#Mostradortext").hide();
            }
        });

        $('input[name="l_Roles"][value*="ajero"]').change(function () {
            if ($('input[name="l_Roles"][value*="ajero"]:checked').length > 0) {
                $('input[name="l_Roles"][value*="SuperAdmin"]').prop('checked', false);
                $('input[name="l_Roles"][value*="endedor"]').prop('checked', false);
                $('input[name="l_Roles"][value*="Regional"]').prop('checked', false);
                $('input[name="l_Roles"][value*="CxC"]').prop('checked', false);

                $("#Ubicacion_Mostrador").show();
                $("#Ubicacion_Reg_chosen").hide();
                $("#Mostradortext").hide();
            }
        });

        $('input[name="l_Roles"][value*="endedor"]').change(function () {
            if ($('input[name="l_Roles"][value*="endedor"]:checked').length > 0) {
                $('input[name="l_Roles"][value*="SuperAdmin"]').prop('checked', false);
                $('input[name="l_Roles"][value*="ajero"]').prop('checked', false);
                $('input[name="l_Roles"][value*="Regional"]').prop('checked', false);
                $('input[name="l_Roles"][value*="CxC"]').prop('checked', false);

                $("#Ubicacion_Mostrador").show();
                $("#Ubicacion_Reg_chosen").hide();
                $("#Mostradortext").hide();
            }
        });

        $('input[name="l_Roles"][value*="Regional"]').change(function () {
            if ($('input[name="l_Roles"][value*="Regional"]:checked').length > 0) {
                $('input[name="l_Roles"][value*="SuperAdmin"]').prop('checked', false);
                $('input[name="l_Roles"][value*="ajero"]').prop('checked', false);
                $('input[name="l_Roles"][value*="endedor"]').prop('checked', false);
                $('input[name="l_Roles"][value*="CxC"]').prop('checked', false);

                $("#Ubicacion_Mostrador").hide();
                $("#Ubicacion_Reg_chosen").show();
                $("#Mostradortext").show();
            }
        });


    </script>
End Section



