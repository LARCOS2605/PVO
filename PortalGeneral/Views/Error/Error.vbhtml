@ModelType PortalGeneral.LoginModel
@Code
    Layout = "~/Views/Shared/_LayoutLogin.vbhtml"
End Code

<div class="error-area ptb--100 text-center">
    <div class="container">
        <div class="error-content">
            <h2>404</h2>
            <p>Hubo un problema al procesar su solicitud, intentelo más tarde.</p>
            <a href="@Url.Action("index", "Home")">Regresar a la pagina principal</a>
        </div>
    </div>
</div>

