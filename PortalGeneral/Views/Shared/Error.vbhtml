@ModelType PortalGeneral.LoginModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
End Code

<div id="preloader">
    <div class="loader"></div>
</div>
<!-- preloader area end -->
<!-- error area start -->
<div class="error-area ptb--100 text-center">
    <div class="container">
        <div class="error-content">
            <h2>500</h2>
            <p>Hubo un problema interno con su solicitud, intentelo más tarde.</p>
            <a href="@Url.Action("Index", "Home")">Regresar Al Sitio.</a>
        </div>
    </div>
</div>