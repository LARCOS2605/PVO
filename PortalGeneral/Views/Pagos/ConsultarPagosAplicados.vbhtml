
@Code
    ViewData("Title") = "Consultar Pagos Aplicados"
    ViewData("I_CPA") = "class=active"
    ViewData("SubPage") = "Pagos"
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Pagos Aplicados
                        </a>
                    </div>
                    <div id="accordion21" class="collapse" data-parent="#accordion2">
                        <div class="card-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label for="example-datetime-local-input" class="col-form-label">Fecha de Carga Desde:</label>
                                        <input class="form-control" type="date" id="FechaIni">
                                    </div>
                                    <div class="col-lg-6">
                                        <label for="example-datetime-local-input" class="col-form-label">Fecha de Carga Hasta:</label>
                                        <input class="form-control" type="date" id="FechaFin">
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <label for="Cliente_Select" class="col-form-label">Cliente:</label>
                                        <br />
                                        <select id="Cliente_Select" class="form-control">
                                            <option value="">--- Seleccione un Cliente a Consultar ---</option>
                                            @For Each RegMost As Customers In ViewBag.ClientesDisponibles
                                                @<option value="@RegMost.NS_InternalID">@RegMost.NS_ExternalID - @RegMost.Nombre</option>
                                            Next
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <Button type="button" onclick="ConsultarPagosAplicados()" Class="btn btn-primary"> Consultar Pagos</Button>
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
                    <table id="TablaPagos" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Folio de Pago</th>
                                <th scope="col">Cliente</th>
                                <th scope="col">Fecha Aplicación</th>
                                <th scope="col">Metodo de Pago</th>
                                <th scope="col">Total Pagado</th>
                                <th scope="col">Estatus</th>
                                <th scope="col"><i class="fa fa-gears"></i></th>
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

<div class="modal fade bd-example-modal-lg modal-sl" id="ModalEdicion">
    <div class="modal-dialog modal-lg modal-sl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Detalles del Pedido</h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body" id="areaDetalles">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarPagosAplicados", "Pagos", FormMethod.Post, New With {.id = "ConsultarDatosPagos"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaInicio" id="create-FechaInicio" />
        @<input type="hidden" name="FechaFin" id="create-FechaFin" />
        @<input type="hidden" name="Customers" id="create-Customers" />

    End Using
</div>


@Section Scripts

    <script type="text/template" id='DetalleTemplate'>

        <p><strong>Datos Principales.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Folio de Pago:</label>
                    <input class="form-control" type="text" value="<%= FolioTransaccion %>" readonly id="Show_NoPedido">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Fecha De Creacion:</label>
                    <input class="form-control" type="text" readonly value="<%= FechaCreacion %>" id="Show_FechaCreacion">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Cliente:</label>
                    <input class="form-control" type="text" readonly value="<%= Cliente %>" id="Show_Cliente">
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <label for="example-text-input" class="col-form-label">Nota:</label>
                    <input class="form-control" type="text" readonly value="<%= NotaPago %>" id="Show_NotaPago">
                </div>
            </div>
        </div>
        <%
        if(Timbrado == 'True'){
        %>
        <p><strong>Datos Fiscales.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Serie:</label>
                    <input class="form-control" type="text" value="<%= Serie_Timbrado %>" readonly id="Show_NoPedido">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Folio:</label>
                    <input class="form-control" type="text" readonly value="<%= Folio_Timbrado %>" id="Show_FechaCreacion">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Fecha Timbrado:</label>
                    <input class="form-control" type="text" readonly value="<%= FechaTimbrado %>" id="Show_Mostrador">
                </div>
            </div>
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">UUID</label>
                    <div class="input-group-append">
                        <input class="form-control" type="text" value="<%= UUID %>" readonly id="UUID_INV">
                        <button type="button" id="GetPDF_Timbrado" data-id="<%= NS_ID_pdf_pago %>" class="btn btn-info"><i class="fa fa-file-pdf-o"></i> PDF</button>
                        <button type="button" id="GetXML_Timbrado" data-id="<%= NS_ID_xml_pago %>" class="btn btn-info"><i class="fa fa-file"></i> XML</button>
                    </div>
                </div>
            </div>
        </div>
        <%
        }
        %>
        <br />
        <p><strong>Facturas Aplicadas.</strong></p>
        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Folio Factura</th>
                            <th scope="col">Importe Original</th>
                            <th scope="col">Importe a Pagar</th>
                            <th scope="col">Pago Aplicado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_DetallePago, function(factura){ %>
                        <tr>
                            <td nowrap><%= factura.FolioFactura %></td>
                            <td nowrap><%= accounting.formatMoney(factura.ImporteOriginal) %></td>
                            <td nowrap><%= accounting.formatMoney(factura.ImportePagar) %></td>
                            <td nowrap><%= accounting.formatMoney(factura.PagoAplicado) %></td>
                        </tr>
                        <% }); %>
                    </tbody>
                </table>
            </div>
        </div>

        <script>

            $('#GetPDF_Timbrado').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando PDF Complemento de Pago...");

                window.location.replace(gets);
            });

            $('#GetXML_Timbrado').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando XML Complemento de Pago...");

                window.location.replace(gets);
            });
        </script>

    </script>

    <script>
        var tablaDetalle = null;
        var EjecucionPedido = [];
        var Detalle = [];
        var contador = 0;

        $(document).ready(function () {

            tablaDetalle = $("#TablaPagos").DataTable({
                dom: 'Bfrtip',
                "info": false,
                responsive: false,
                "ordering": false,
                paging: true,
                buttons: [{
                    text: '<i class="fa fa-file-excel-o"></i>',
                    extend: 'excelHtml5',
                    title: 'Sales Order',
                    className: 'btn btn-flat btn-info mb-3'
                }]
            });
            changeExcelButton();
        });

        function changeExcelButton() {
            //Modificar boton de Exportar a Excel
            $("div.dt-buttons").appendTo("#TablaPagos_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TablaPagos_filter").css("margin-left", "10px");
            $("#Cliente_Select").select2();
        }

        function ConsultarPagosAplicados() {

            var form = $("#ConsultarDatosPagos");

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();
            var Customer = $('#Cliente_Select').val();
            
            form.children("[name='FechaInicio']").val(FechaIni);
            form.children("[name='FechaFin']").val(FechaFin);
            form.children("[name='Customers']").val(Customer);

            var dataForm = form.serialize();

            toastr.info("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        var control = ""
                        var Estatus = ""
                        control = "<ul class='d-flex justify-content-center'>  <li onclick='DescargarComprobantePago(" + value.NS_InternalID + ")' class='mr-3' title='Imprimir Comprobante de Pago'><a href='#' class='text-secondary'><i class='fa fa-print'></i></a></li> <li onclick='DetallesPago(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles Pago'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li></ul>"

                        if (value.Timbrado != null) {
                            Estatus = "<span class='badge badge-success'>Timbrado</span>";
                        } else {
                            Estatus = "<span class='badge badge-danger'>Sin Timbrar</span>";
                        }

                        tablaDetalle.row.add([
                            value.FolioPago,
                            value.Nombre_Cliente,
                            value.fecha,
                            value.MetodoPago,
                            accounting.formatMoney(value.MontoPago),
                            Estatus,
                            control,
                        ]).node().id = value.NS_InternalID;

                        tablaDetalle.draw(false);

                    });

                    toastr.success("Consulta realizada con exito...");

                }
                else {
                    toastr.error(data.Mensaje);
                }
            });

        }

        function DescargarComprobantePago(datos) {
            var id = datos;
            var url = "@Url.Action("ImprimirComprobantePago", "Pagos", New With {.id = "__ID__"})";
            var gets = url.replace("__ID__", id);

            toastr.info("Descargando Comprobante de Pago...");

            window.location.replace(gets);
        }

        function DetallesPago(id) {

            var Url_Datos = "@Url.Action("DetallePagos", "Pagos", New With {.ns_id = "__ID__"})"

            var link = Url_Datos.replace("__ID__", id);

            toastr.info("Obteniendo Detalles de Pedido...");

            jQuery.ajax({
                url: link,
                cache: false,
                contentType: false,
                processData: false,
                type: 'POST',
                success: function (data) {
                    if (data.Tipo == 1) {

                        Detalle.length = 0;

                        _.each(data.Valor.l_detalle, function (item) {
                            Detalle.push({ "id": item.idDetalleSalesOrder });
                        }, this);

                        var template = _.template(document.getElementById("DetalleTemplate").textContent);
                        $("#areaDetalles").html("");
                        var html = template(data.Valor);
                        $("#areaDetalles").html(html);
                        $('#ModalEdicion').modal('toggle');
                    } else {

                    }
                }
            });


        }
    </script>
End Section
