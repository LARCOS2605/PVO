
@Code
    ViewData("Title") = "Consultar Recepción MRP"
    ViewData("I_GST") = "class=active"
    ViewData("SubPage") = "Solicitud MRP"
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Solicitud MRP
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
                        </div>
                        <div class="card-footer">
                            <Button type="button" onclick="ConsultarSolicitudMRP()" Class="btn btn-primary">Consultar</Button>
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
                    <table id="TablaMRP_Solicitud" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Numero de Operación</th>
                                <th scope="col">Mostrador destino</th>
                                <th scope="col">Estatus Orden Traslado</th>
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


@Section Scripts

    <script type="text/template" id='DetalleTemplate'>

        <p><strong>Datos Principales.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">No. De Pedido</label>
                    <input class="form-control" type="text" value="<%= NombreTransaccion %>" readonly id="Show_NoPedido">
                </div>
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Fecha De Creacion:</label>
                    <input class="form-control" type="text" readonly value="<%= FechaCreacion %>" id="Show_FechaCreacion">
                </div>
            </div>
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Mostrador Origen</label>
                    <input class="form-control" type="text" readonly value="<%= AlmacenOrigen %>" id="Show_Mostrador">
                </div>
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Mostrador Destino</label>
                    <input class="form-control" type="text" readonly value="<%= AlmacenDestino %>" id="Show_Mostrador">
                </div>
            </div>
        </div>
        <br />
        <p><strong>Articulos MRP.</strong></p>
        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Articulo</th>
                            <th scope="col">Descripcion</th>
                            <th scope="col">Cantidad Solicitud</th>
                            <th scope="col">Cantidad Enviada</th>
                            <th scope="col">Cantidad Recibida</th>
                            <th scope="col">Estatus</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_Detalle, function(producto){ %>
                        <tr>
                            <td nowrap><%= producto.Articulo %></td>
                            <td nowrap><%= producto.Descripcion %></td>
                            <td nowrap><%= producto.CantidadSolicitud %></td>
                            <td nowrap><%= producto.CantidadEnviada %></td>
                            <td nowrap><%= producto.CantidadRecibida %></td>
                            <td>
                                <%
                                if(producto.CantidadRecibida == 0){
                                %>
                                <span class="badge badge-info">
                                    Pendiente de Entrega
                                </span>
                                <%
                                }else if(producto.CantidadRecibida == producto.CantidadEnviada){
                                %>
                                <span Class="badge badge-success">
                                    Entregado
                                </span>
                                <%
                                }else if(producto.CantidadRecibida != producto.CantidadEnviada){
                                %>
                                <span class="badge badge-warning">
                                    Entrga Parcial
                                </span>
                                <%
                                }
                                %>
                            </td>
                        </tr>
                        <% }); %>
                    </tbody>
                </table>
            </div>
        </div>


    </script>

    <script>
        var tablaDetalle = null;
        var EjecucionPedido = [];
        var Detalle = [];
        var contador = 0;

        $(document).ready(function () {

            tablaDetalle = $("#TablaMRP_Solicitud").DataTable({
                dom: 'Bfrtip',
                "info": false,
                responsive: false,
                "ordering": false,
                paging: false,
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
            $("div.dt-buttons").appendTo("#TablaMRP_Solicitud_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TablaMRP_Solicitud_filter").css("margin-left", "10px");
        }

        function ConsultarSolicitudMRP() {

            var form = $("#ConsultarDatosMRP");

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();

            form.children("[name='FechaInicio']").val(FechaIni);
            form.children("[name='FechaFin']").val(FechaFin);

            var dataForm = form.serialize();

            toastr.info("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        var control = ""
                        var Estatus = ""

                        if (value.Estatus == "received") {

                            control = "<ul class='d-flex justify-content-center'>  <li onclick='DescargarComprobanteRecepcion(" + value.NS_InternalID + ")' class='mr-3' title='Imprimir Comprobante Recepcion'><a href='#' class='text-secondary'><i class='fa fa-print'></i></a></li> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li></ul>"
                            Estatus = "<span class='badge badge-success'>Mercancia Recibida</span>"
                            
                        } else if (value.Estatus == "pendingReceiptPartFulfilled") {

                            control = "<ul class='d-flex justify-content-center'> <li onclick='PrintPedido()' class='mr-3' title='Recibir Mercancia'><a href='/Traslados/GenerarRecepcionArticulos/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-list-alt'></i></a></li> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li> </ul>"
                            Estatus = "<span class='badge badge-warning'>Envio Parcial/Pendiente Recepción</span>"

                        } else if (value.Estatus == "partiallyFulfilled") {

                            control = "<ul class='d-flex justify-content-center'>  <li onclick='DescargarComprobanteRecepcion(" + value.NS_InternalID + ")' class='mr-3' title='Imprimir Comprobante Recepcion'><a href='#' class='text-secondary'><i class='fa fa-print'></i></a></li> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li></ul>"
                            Estatus = "<span class='badge badge-secondary'>Parcialmente Recibido</span>"

                        } else if (value.Estatus == "pendingFulfillment") {

                            control = "<ul class='d-flex justify-content-center'>  <li onclick='DescargarComprobanteRecepcion(" + value.NS_InternalID + ")' class='mr-3' title='Imprimir Comprobante Recepcion'><a href='#' class='text-secondary'><i class='fa fa-print'></i></a></li> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li></ul>"
                            Estatus = "<span class='badge badge-warning'>Pendiente de Envio</span>"

                        }  else {

                            control = "<ul class='d-flex justify-content-center'> <li onclick='PrintPedido()' class='mr-3' title='Recibir Mercancia'><a href='/Traslados/GenerarRecepcionArticulos/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-list-alt'></i></a></li> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li> </ul>"
                            Estatus = "<span class='badge badge-warning'>Pendiente de Recepción</span>"
                        }

                        tablaDetalle.row.add([
                            value.Num_Transaccion,
                            value.NS_InternalID_Destino,
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

        function CancelarOrdenVenta(id) {

            //Modificar boton de Exportar a Excel
            var form = $("#GenerarRecepcion");

            form.children("[name='NS_ID']").val(id);

            var dataForm = form.serialize();
            toastr.info("Generando Recepción Mercancia...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    toastr.success("Consulta realizada con exito...");

                }
                else {
                    toastr.error(data.Mensaje);
                }
            });
        }

        function DescargarComprobanteRecepcion(datos) {
            var id = datos;
            var url = "@Url.Action("ImprimirComprobanteRecepcion", "Traslados", New With {.id = "__ID__"})";
            var gets = url.replace("__ID__", id);

            toastr.info("Descargando Factura...");

            window.location.replace(gets);
        }

        function DetallesPedido(id) {

            var Url_Datos = "@Url.Action("DetalleOrdenTraslado", "Traslados", New With {.ns_id = "__ID__"})"

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
