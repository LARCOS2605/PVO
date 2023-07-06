@Code
    ViewData("Title") = "Home Page"
    ViewData("SubPage") = "Inicio"

    ViewBag.MostradoresDisponibles = System.Web.HttpContext.Current.Session("L_Mostradores")
End Code


<div style="display:none;">
    @Using Html.BeginForm("EditarParametros", "Parametros", FormMethod.Post, New With {.id = "EditParametros"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="Descripcion" id="create-Descripcion" />
        @<input type="hidden" name="Valor" id="create-Valor" />
        @<input type="hidden" name="idParametro" id="create-idParametro" />

    End Using
</div>

@Section Scripts

    <script>
        var TipoConsulta = "@Html.Raw(ViewBag.ValidaMostrador)";

        $(document).ready(function () {

            if (TipoConsulta == "False") {
                $('#ModalSelection').modal('toggle');
            }

        });

        function EditarParametro(id) {
            var Url_Parametros = "@Url.Action("ConsultarParametro", "Parametros", New With {.id = "__ID__"})"

            var link = Url_Parametros.replace("__ID__",id)

            jQuery.ajax({
                    url: link,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    success: function(data){
                        if (data.Tipo == 1) {
                            $('#Valor').val(data.Valor.Valor);
                            $('#Descripcion').val(data.Valor.Descripcion);
                            $('#idParametro').val(id);
                            $('#ModalEdicion').modal('toggle');
                        } else {

                        }
                    }
             });

        }
    </script>
End Section