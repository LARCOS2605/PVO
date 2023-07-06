@ModelType PortalGeneral.LoginModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
    ViewData("Title") = "Login"
End Code

<div class="login-area login-bg">
    <div class="container">
        <div class="login-box ptb--100">
            <form>
                <div class="login-form-head">
                    <h4>Sign In</h4>
                    <p>Hello there, Sign in and start managing your Admin Template</p>
                </div>

                @Using Html.BeginForm(New With {.ReturnUrl = ViewData("ReturnUrl")})
                    @Html.AntiForgeryToken()
                    @Html.ValidationSummary(True)

                    @<div class="login-form-body">
                        <div class="form-gp">
                            <label for="exampleInputEmail1">Correo Electrónico:</label>
                            @Html.TextBoxFor(Function(m) m.UserName, New With {.type = "email"})
                            @Html.ValidationMessageFor(Function(m) m.UserName)
                            <i class="ti-email"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="form-gp">
                            <label for="exampleInputPassword1">Contraseña:</label>
                            @Html.TextBoxFor(Function(m) m.Password, New With {.type = "password"})
                            @Html.ValidationMessageFor(Function(m) m.Password)
                            <i class="ti-lock"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="row mb-4 rmber-area">
                            <div class="col-6">
                                <div class="custom-control custom-checkbox mr-sm-2">
                                    @Html.CheckBoxFor(Function(m) m.RememberMe, New With {.Class = "custom-control-input"})
                                    <label class="custom-control-label" for="customControlAutosizing">Recordarme</label>
                                </div>
                            </div>
                            <div class="col-6 text-right">
                                <a href="#">Forgot Password?</a>
                            </div>
                        </div>
                        <div class="submit-btn-area">
                            <button id="form_submit" type="submit">Submit <i class="ti-arrow-right"></i></button>
                            <button type="button" id="btnAccesar" class="btn btn-success mb-3">Iniciar Sesion <i class="ti-arrow-right"></i></button>
                        </div>
                        <div class="form-footer text-center mt-5">
                            <p class="text-muted">Don't have an account? <a href="register.html">Sign up</a></p>
                        </div>
                    </div>
                End Using
            </form>
        </div>
    </div>
</div>

@Section Scripts
    <script src="http://code.jquery.com/jquery-1.11.0.min.js"></script>
    <script>

        $("#btnAccesar").on("click", function () {

            var userName = $("#UserName").val();
            var password = $("#Password").val();

            if (userName == "") {
                toastr.error("El Campo Correo Electronico, no puede ir vacio.")
                return false;
            }
            if (password == "") {
                toastr.error("El Campo Contraseña, no puede ir vacio.")
                return false;
            }

            var url = "@Url.Action("loginValidate", "Account", Nothing)";
            if (userName != "" && password != "") {
                jQuery.ajax({
                    url: url,
                    data: null,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function (data) {
                        if (data.Tipo == 1) {
                            toastr.info("Iniciando Sesión.");
                            $("#FormLogin").submit();
                        } else {
                            alert(data.Mensaje);
                        }
                    }
                });
            } else {
                $("#FormLogin").submit();
            }
        });

    </script>
End Section