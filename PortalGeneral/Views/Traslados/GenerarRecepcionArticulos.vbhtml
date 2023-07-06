@modeltype PortalGeneral.GeneraRecepcionViewModel
@Code
    ViewData("Title") = "Generar Recepcion de Mercancia"
    ViewData("I_GST") = "class=active"
    ViewData("SubPage") = "Solicitud MRP"
End Code

@Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "RegistrarRecepcion"})
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(True)

    @<div class="col-lg-12 mt-5">
        <div class="card">
            <div class="card-body">
                <h4 class="header-title">Información de Recepción:</h4>
                <div class="form-group">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-12">
                                @Html.LabelFor(Function(model) model.Nota, "Nota:", New With {.class = "col-form-label"})
                                @Html.TextAreaFor(Function(model) model.Nota, New With {.class = "form-control"})
                                @Html.ValidationMessageFor(Function(model) model.Nota)
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <input type="hidden" name="NS_Traslado" id="NS_Traslado" />
        <div Class="col-lg-12" id="DetallehiddenDivDetalles">
        </div>
    </div>

End Using

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <header><h4 class="header-title">Detalle Mercancia Recibida</h4></header>

            <div class="single-table">
                <div class="table-responsive">
                    <table id="TablaDetalleMRP" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Clase Articulo</th>
                                <th scope="col">Descripcion</th>
                                <th scope="col">Cantidad Orden</th>
                                <th scope="col">Cantidad Enviada</th>
                                <th scope="col">Cantidad Recibida por Confirmar</th>
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.DetalleConceptos
                                Dim currentItem As DetalleOrdenTrasladoViewModel = item
                                @<tr id="@currentItem.NS_InternalID">
                                    <td scope="row">@currentItem.ClassArticulo</td>
                                    <td scope="row">@currentItem.NombreArticulo</td>
                                    <td scope="row">@currentItem.CantidadDisponible</td>
                                    <td scope="row">@currentItem.CantidadEnviada</td>
                                    <td scope="row"><input class='form-control allownumeric' type='number' id='C-@currentItem.NS_InternalID' min='0' onblur='ValidarCantidadIngresada(@currentItem.NS_InternalID)' value="@Math.Round(Convert.ToDouble(currentItem.CantidadEnviada))"></td>
                                </tr>
                            Next
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <button type="button" onclick="AlmacenarRecepcionArticulos()" class="btn btn-success" data-toggle="tooltip">Guardar Recepción</button>
        </div>
    </div>
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarRecepcionesPendientes", "Traslados", FormMethod.Post, New With {.id = "ConsultarDatosMRP"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaInicio" id="create-FechaInicio" />
        @<input type="hidden" name="FechaFin" id="create-FechaFin" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("GenerarRecepcionMercancia", "Traslados", FormMethod.Post, New With {.id = "GenerarRecepcion"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="NS_ID" id="create-NS_ID" />

    End Using
</div>

<script type="text/template" id='ProductoListHidde'>
    @Html.Partial("RecepcionMRPListHidden")
</script>


@Section Scripts

    <script>
        var tablaDetalle = null;
        var EjecucionPedido = [];
        var Detalle = [];
        var contador = 0;
        var Detalle = @Html.Raw(ViewBag.Detalle);
        var Confirmacion = @Html.Raw(ViewBag.Detalle);
        var NS_InternalID_Traslado = "@Html.Raw(ViewBag.NS)";
        $('#NS_Traslado').val(NS_InternalID_Traslado);

        $(document).ready(function () {
            console.log(Confirmacion);
            tablaDetalle = $("#TablaDetalleMRP").DataTable({
                /*dom: 'Bfrtip',*/
                "info": false,
                responsive: false,
                "ordering": false,
                paging: false
            });
        });

        function DescargarComprobanteRecepcion(datos) {
            var id = datos;
            var url = "@Url.Action("ImprimirComprobanteRecepcion", "Traslados", New With {.id = "__ID__"})";
            var gets = url.replace("__ID__", id);

            toastr.info("Descargando Factura...");

            window.location.replace(gets);
        }

        function ValidarCantidadIngresada(datos) {
            $.each(Detalle, function (index, value) {
                if (value.NS_InternalID == datos) {

                    var ValidacionIngresado = $('#C-' + datos).val();
                    var Cantidad = ValidacionIngresado.replace(/[^0-9]/g, "");
                    var CantidadIngresada = parseFloat(Cantidad);

                    if (isNaN(CantidadIngresada)) {
                        $('#C-' + datos).val(value.CantidadDisponible);
                        toastr.error("La cantidad ingresada no es valida.");
                        return false;
                    }

                    //if (CantidadIngresada != "") {

                        if (CantidadIngresada < 0) {
                            $('#C-' + datos).val(value.CantidadDisponible);
                            toastr.error("La cantidad ingresada no es valida.");
                            return false;
                        } else if (CantidadIngresada > value.CantidadDisponible) {
                            $('#C-' + datos).val(value.CantidadDisponible);
                            toastr.error("La cantidad es superior a la enviada.");
                            return false;
                        } else {

                            Confirmacion = $.grep(Confirmacion, function (e) {
                                return e.NS_InternalID != datos;
                            });

                            Confirmacion.push({
                                CantidadDisponible: CantidadIngresada, CantidadRecibida: null, ClassArticulo: null, NS_InternalID: value.NS_InternalID, NombreArticulo: null
                            });
                        }

                    //} else {
                    //    $('#C-' + datos).text(value.CantidadDisponible);
                    //    toastr.error("La cantidad ingresada no es valida.");
                    //    return false;
                    //}
                }
            });
        }

        $(".allownumeric").on("keypress keyup blur", function (event) {
            $(this).val($(this).val().replace(/[^\d].+/, ""));
            if ((event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });

        function AlmacenarRecepcionArticulos() {

            if (Confirmacion.length === 0) {
                toastr.error("No se han encontrado articulos validos.");
                return false
            }

            showLoading("Creando Recepción de Mercancia...");

            var templateProducto = _.template(document.getElementById("ProductoListHidde").textContent);
            $("#RecepcionMRPListHidden").html("");

            _.each(Confirmacion, function (item) {
                var dataItem = item
                var html = templateProducto({ Entrega: dataItem, contador: contador })
                $("#DetallehiddenDivDetalles").append(html);
                contador++;

            }, this);

            $('#RegistrarRecepcion').submit();
        }
    </script>
End Section
