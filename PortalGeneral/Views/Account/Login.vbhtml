@ModelType PortalGeneral.LoginModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
    ViewData("Title") = "Login"
End Code


<div id="Loggg" class="login-area login-bg">
    <div class="container">
        <div class="login-box ptb--100">
            @Using Html.BeginForm(New With {.ReturnUrl = ViewData("ReturnUrl"), .id = "FormLogin"})
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True)

                @If not IsNothing(TempData("Error")) Then
                    @<div class="alert-items">
                        <div class="alert alert-danger" role="alert"><strong>Hubo un problema al iniciar su sesión.</strong> @Html.ValidationMessageFor(Function(m) m.ValidationMessage)</div>
                    </div>
                End If

                @<form>
                     <div class="login-form-head">
                         <div class="form-group">
                             <div class="row">
                                 <div class="col-lg-12">
                                     <div class="logo">
                                         <a href="https://www.muellesmaf.com.mx/"><img src="~/Assets/images/logo.png" alt="logo"></a>
                                     </div>
                                 </div>
                             </div>
                         </div>
                         <br />
                         <h4>Portal PVO 2.0</h4>
                         <p>Iniciar Sesión</p>
                     </div>
                    <div class="login-form-body">
                        <div class="form-gp">
                            <label>Correo Electronico:</label>
                            @Html.TextBoxFor(Function(m) m.UserName, New With {.type = "text"})
                            @Html.ValidationMessageFor(Function(m) m.UserName)
                            <i class="ti-user"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="form-gp">
                            <label>Contraseña:</label>
                            @Html.TextBoxFor(Function(m) m.Password, New With {.type = "password", .onkeyup = "Encript()"})
                            @Html.ValidationMessageFor(Function(m) m.Password)
                            <i class="ti-lock"></i>
                            <div class="text-danger"></div>
                        </div>
                        <div class="row mb-4 rmber-area">
                            <div class="col-6">
                                <div class="custom-control custom-checkbox mr-sm-2">
                                    @Html.CheckBoxFor(Function(m) m.customControlAutosizing, New With {.Class = "custom-control-input"})
                                    <label class="custom-control-label" id="customControlAutosizing" for="customControlAutosizing">Recordarme</label>
                                </div>
                            </div>
                            @*<div class="col-6 text-right">
                                @Html.ActionLink("¿Olvidó su contraseña?", "Reset", "Account")
                            </div>*@
                        </div>
                        <div class="submit-btn-area">
                            <button id="form_submit" type="submit">Iniciar Sesión <i class="ti-arrow-right"></i></button>
                        </div>
                    </div>
                </form>
            End Using
        </div>
    </div>
</div>

@Section Scripts
    <script src="~/assets/js/jquery.backstretch.min.js"></script>
    <script>

        function Encript() {
            var password = $('#Password').val();
            var enPass = hex_sha1(password);
            console.log(enPass);
        }

        $("#form_submit").on("click", function () {

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

        $(function () {
            jQuery(document).ready(function () {
                $('#Loggg').backstretch([
                    "/Assets/images/home-bg-slideshow1.jpg",
                    "/Assets/images/home-bg-slideshow2.jpg",
                    "/Assets/images/home-bg-slideshow5.jpg",
                ], { duration: 2000, fade: 750 });
            });
        })

    </script>
End Section

