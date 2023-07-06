@Code
    ViewData("Title") = "Consulta de Parametros"
    ViewData("I_CP") = "class=active"
    ViewData("SubPage") = "Configuración"
End Code


<div class="card">
    <div class="card-body">
        <header><h4 class="header-title">@ViewData("Title")</h4></header>

        <!-- Modal -->
        <div class="modal fade" id="ModalEdicion">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Editar Parametro</h5>
                        <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <form>
                            <div class="form-group">
                                <label for="exampleInputEmail1">Valor:</label>
                                <input type="text" class="form-control" id="Valor" aria-describedby="emailHelp">
                            </div>
                            <div class="form-group">
                                <label for="exampleInputPassword1">Descripción:</label>
                                <input type="text" class="form-control" id="Descripcion">
                                <input style="display:none" type="text" class="form-control" id="idParametro">
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                        <button type="button" id="AlmacenarCambios" class="btn btn-primary">Guardar Cambios</button>
                    </div>
                </div>
            </div>
        </div>

        <div style="display:none;">
            @Using Html.BeginForm("EditarParametros", "Parametros", FormMethod.Post, New With {.id = "EditParametros"})
                @Html.AntiForgeryToken()

                @<input type="hidden" name="Descripcion" id="create-Descripcion" />
                @<input type="hidden" name="Valor" id="create-Valor" />
                @<input type="hidden" name="idParametro" id="create-idParametro" />

            End Using
        </div>

        <div class="single-table">
            <div class="table-responsive">
                <table id="TableParametros" class="table table-hover progress-table text-center">
                    <thead class="text-uppercase">
                        <tr>
                            <th scope="col">Clave de Sistema</th>
                            <th scope="col">Valores Ajustables</th>
                            <th scope="col">Descripción</th>
                            <th scope="col">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        @For Each item In ViewBag.Parametros
                            Dim currentItem As Parametros = item
                            @<tr>
                                <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Clave)</td>
                                <td>@Html.DisplayFor(Function(modelItem) currentItem.Valor)</td>
                                <td>@Html.DisplayFor(Function(modelitem) currentItem.Descripcion)</td>
                                <td>
                                    <ul class="d-flex justify-content-center">
                                        <li onclick="EditarParametro(@currentItem.idParametro)" class="mr-3"><a href="#" class="text-secondary"><i class="fa fa-edit"></i></a></li>
                                    </ul>
                                </td>
                            </tr>
                        Next
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

@Section Scripts

    <script>
        var tablaDetalle = null;

        $(document).ready(function () {
            tablaDetalle = $("#TableParametros").DataTable();
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

        $('#AlmacenarCambios').click(function () {

            var form = $("#EditParametros");

            if ($('#Valor').val() == "") {
                toastr.error('El Campo "Valor" no puede ir vacio.');
                return false
            }

            if ($('#Descripcion').val() == "") {
                toastr.error('El Campo "Descripción" no puede ir vacio.');
                return false
            }

            var Valor = $('#Valor').val();
            var Descripcion = $('#Descripcion').val();
            var idParametro = $('#idParametro').val();

            form.children("[name='Valor']").val(Valor);
            form.children("[name='Descripcion']").val(Descripcion);
            form.children("[name='idParametro']").val(idParametro);

            var dataForm = form.serialize();
            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                console.log(data);
                if (data.Tipo == 1) {
                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        tablaDetalle.row.add([
                            value.Clave,
                            value.Valor,
                            value.Descripcion,
                            "<ul class='d-flex justify-content-center'><li onclick='EditarParametro("+value.idParametro+")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-edit'></i></a></li></ul>",
                        ]).draw(false);
                    });

                    $('#ModalEdicion').modal('toggle');
                    toastr.success('El Parametro fue Actualizado Correctamente...');
                    
                }
                else {

                }
            });
        });
    </script>
End Section