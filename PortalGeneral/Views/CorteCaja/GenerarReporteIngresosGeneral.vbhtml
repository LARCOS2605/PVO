
@Code
    ViewData("Title") = "Generar Reporte Ingresos Generales"
    ViewData("I_RIG") = "class=active"
    ViewData("SubPage") = "Reportes"
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Ingresos Generales
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
                            </div>
                        </div>
                        <div class="card-footer">
                            <Button type="button" onclick="GenerarReporteOrdenesVentas()" Class="btn btn-success">Imprimir Reporte</Button>
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
                                <th scope="col">Folio Pago</th>
                                <th scope="col">Monto</th>
                                <th scope="col">Formas de Pago</th>
                                <th scope="col">Factura Aplicada</th>
                                <th scope="col">Fecha Aplicación</th>
                                <th scope="col">Fecha Real Pago</th>
                                <th scope="col">Metodo Pago</th>
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
    @Using Html.BeginForm("ConsultarCorteCaja", "CorteCaja", FormMethod.Post, New With {.id = "ConsultarCorteCaja"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaDesde" id="create-FechaDesde" />
        @<input type="hidden" name="FechaHasta" id="create-FechaHasta" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("GenerarReportePDF", "CorteCaja", FormMethod.Post, New With {.id = "ConsultarReporteCorteCaja"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaDesde" id="create-FechaDesde" />
        @<input type="hidden" name="FechaHasta" id="create-FechaHasta" />
        @<input type="hidden" name="ReporteDia" id="create-ReporteDia" />

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
                    title: 'Reporte de Ingresos',
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
                            value.ClaveCliente + " - " + value.NombreCliente,
                            value.FolioPago,
                            accounting.formatMoney(value.MontoPago),
                            value.MetodoPago,
                            value.FolioFactura,
                            value.fechacre,
                            value.fecha,
                            value.metodopagoFactura
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

        function GenerarReporteOrdenesVentas() {

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();

            var form = $("#ConsultarReporteCorteCaja");

            if ((!Date.parse(FechaIni)) || (!Date.parse(FechaFin))) {
                if (confirm("No se ha definido un rango de fechas, por defecto se generará con el dia de hoy. ¿Desea continuar?")) {
                    form.children("[name='ReporteDia']").val("True");
                    form.submit();
                }
            } else {

                //var NumLote = $('#NumeroLote').val();

                form.children("[name='FechaDesde']").val(FechaIni);
                form.children("[name='FechaHasta']").val(FechaFin);
                form.children("[name='ReporteDia']").val("");
                //form.children("[name='NumLote']").val(NumLote);

                //toa("Generando Consulta...");
                form.submit();
            }




        }
    </script>
End Section
