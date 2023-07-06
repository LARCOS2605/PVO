@ModelType PortalGeneral.LoginModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
    ViewData("Title") = "Login"
End Code


<div class="login-area login-bg">
    <div class="container">
        <div class="login-box ptb--100">
            @Using Html.BeginForm("Reset", "Account", FormMethod.Post, New With {.enctype = "multipart/form-data"})
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True)
                @<form>
                     <div class="login-form-head">
                         <h4> Recuperar Contraseña</h4>
                         <p>Podemos ayudarle a restablecer su contraseña, escriba su cuenta de correo para poder realizar esta acción.</p>
                     </div>
                    <div class="login-form-body">
                        <div class="form-gp">
                            <label for="exampleInputEmail1">Correo Electronico:</label>
                            @Html.TextBoxFor(Function(m) m.UserName, New With {.type = "email"})
                            @Html.ValidationMessageFor(Function(m) m.UserName)
                            <i class="ti-email"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="submit-btn-area">
                            <button id="form_submit" type="submit">Recuperar Contraseña <i class="ti-arrow-right"></i></button>
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

            var userName = $("#UserName").val();

            if (userName == "") {
                toastr.error("El Campo Correo Electronico, no puede ir vacio.")
                return false;
            }

            toastr.info("Generando solicitud de cambio de contraseña...");
        });

    </script>
End Section

