
@Code
    ViewData("Title") = "Generar Reporte Kardex Articulos"
    ViewData("I_RKA") = "class=active"
    ViewData("SubPage") = "Reportes"
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Kardex Articulos
                        </a>
                    </div>

                    <div id="accordion21" class="collapse" data-parent="#accordion2">
                        <div class="card-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label for="example-datetime-local-input" class="col-form-label">Fecha Desde:</label>
                                        <input class="form-control" type="date" id="FechaIni">
                                    </div>
                                    <div class="col-lg-6">
                                        <label for="example-datetime-local-input" class="col-form-label">Fecha Hasta:</label>
                                        <input class="form-control" type="date" id="FechaFin">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label for="example-datetime-local-input" class="col-form-label">Clave de Articulo:</label>
                                        <input class="form-control" type="text" id="ClaveArticulo">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <Button type="button" onclick="GenerarReporteKardexArticulos()" Class="btn btn-success">Imprimir Reporte</Button>
                            <Button type="button" onclick="ConsultarOrdenesVentas()" Class="btn btn-primary">Consultar</Button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</div>

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <header><h4 class="header-title">@ViewData("Title")</h4></header>

            <div class="single-table">
                <div class="table-responsive">
                    <table id="TableOrdenesVenta" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Cliente</th>
                                <th scope="col">Nombre</th>
                                @*<th scope="col">Nombre</th>*@
                                <th scope="col">Folio Factura</th>
                                <th scope="col">Monto</th>
                                <th scope="col">Metodo de Pago</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarCorteCajaRep", "CorteCaja", FormMethod.Post, New With {.id = "ConsultarCorteCaja"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaDesde" id="create-FechaDesde" />
        @<input type="hidden" name="FechaHasta" id="create-FechaHasta" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("GenerarReporteKardexArticulo", "CorteCaja", FormMethod.Post, New With {.id = "GenerarReporteKardexArticulos"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaInicio" id="create-FechaInicio" />
        @<input type="hidden" name="FechaFin" id="create-FechaFin" />
        @<input type="hidden" name="ClaveArticulo" id="create-ClaveArticulo" />

    End Using
</div>

@Section Scripts


    <script>
        var tablaDetalle = null;
        var Detalle = [];
        var EjecucionPedido = [];
        var contador = 0;

        $(document).ready(function () {


            tablaDetalle = $("#TableOrdenesVenta").DataTable({
                dom: 'Bfrtip',
                "info": false,
                responsive: false,
                "ordering": false,
                buttons: [{
                    text: '<i class="fa fa-file-excel-o"></i>',
                    extend: 'excelHtml5',
                    title: 'Corte Caja',
                    className: 'btn btn-flat btn-info mb-3'
                }]
            });
            changeExcelButton();
        });

        function changeExcelButton() {
            //Modificar boton de Exportar a Excel
            $("div.dt-buttons").appendTo("#TableOrdenesVenta_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TableOrdenesVenta_filter").css("margin-left", "10px");
        }

        function ConsultarOrdenesVentas() {

            var form = $("#ConsultarCorteCaja");

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();
            //var NumLote = $('#NumeroLote').val();

            form.children("[name='FechaDesde']").val(FechaIni);
            form.children("[name='FechaHasta']").val(FechaFin);
            //form.children("[name='NumLote']").val(NumLote);

            var dataForm = form.serialize();

            showLoading("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        tablaDetalle.row.add([
                            value.ClaveCliente,
                            value.NombreCliente,
                            value.FolioFactura,
                            accounting.formatMoney(value.TotalFactura),
                            value.FormaPago,
                        ]).draw(false);

                    });

                    hideLoading();
                    toastr.success("Consulta realizada con exito...");

                }
                else {
                    hideLoading()
                    toastr.error(data.Mensaje);
                }
            });

        }

        function GenerarReporteKardexArticulos() {

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();
            var ClaveArticulo = $('#ClaveArticulo').val();

            var form = $("#GenerarReporteKardexArticulos");

            if ((!Date.parse(FechaIni)) || (!Date.parse(FechaFin))) {
                toastr.error("No se ha definido un rango de fechas calido para realizar el reporte.")
                return false
            //} else if (ClaveArticulo == '') {
            //    toastr.error("No se ha ingresado una clave de articulo valida para realizar el reporte.")
            //    return false
            } else {

                form.children("[name='FechaInicio']").val(FechaIni);
                form.children("[name='FechaFin']").val(FechaFin);
                form.children("[name='ClaveArticulo']").val(ClaveArticulo);

                form.submit();
            }




        }
    </script>
End Section
