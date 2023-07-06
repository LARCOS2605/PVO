@ModelType PortalGeneral.LocalPasswordModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
    ViewData("Title") = "Cambiar Password"
End Code

<div class="login-area login-bg">
    <div class="container">
        <div class="login-box ptb--100">
            @Using Html.BeginForm("ResetPasswordRequest", "Account", FormMethod.Post, New With {.enctype = "multipart/form-data", .id = "formReset"})
                @Html.AntiForgeryToken()
                @<form>
                    <div class="login-form-head">
                        <h4>Restablecer Contraseña.</h4>
                        <p>Para terminar el proceso, capture su nueva contraseña.</p>
                    </div>
                    <div class="login-form-body">
                        <div class="form-gp">
                            @Html.LabelFor(Function(m) m.NewPassword)
                            @Html.PasswordFor(Function(m) m.NewPassword, New With {.type = "password"})
                            @Html.ValidationMessageFor(Function(m) m.NewPassword)
                            <i class="ti-lock"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="form-gp">
                            @Html.LabelFor(Function(m) m.ConfirmPassword)
                            @Html.PasswordFor(Function(m) m.ConfirmPassword, New With {.type = "password"})
                            @Html.ValidationMessageFor(Function(m) m.ConfirmPassword)
                            <i class="ti-lock"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="submit-btn-area">
                            <button id="form_submit" type="submit">Restaurar Contraseña <i class="ti-arrow-right"></i></button>
                        </div>
                        <div class="form-footer text-center mt-5">
                            @Html.ActionLink("Regresar a Iniciar Sesión", "Login", "Account")
                        </div>
                    </div>
                </form>
            End Using
        </div>
    </div>
</div>


@Section Scripts
    @*<script src="http://code.jquery.com/jquery-1.11.0.min.js"></script>*@
    <script>

        $("#form_submit").on("click", function () {

            var pass = $("#NewPassword").val();
            var npass = $("#ConfirmPassword").val();

            if (pass == "") {
                toastr.error("Uno de los campos de contraseña no puede ir vacio...")
                return false;
            } else if (npass == "") {
                toastr.error("Uno de los campos de contraseña no puede ir vacio...")
                return false;
            }

            toastr.info("Cambiando su Contraseña...");
            $('#formReset').submit();
        });

    </script>
End Section