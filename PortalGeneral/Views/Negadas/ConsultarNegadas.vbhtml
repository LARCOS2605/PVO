
@Code
    ViewData("Title") = "Consultar Negadas"
    ViewData("I_EstCon") = "class=active"
    ViewData("SubPage") = "Estimate"

    Dim db As New PVO_NetsuiteEntities
    Dim Ambiente = (From n In db.Parametros Where n.Clave = "Ambiente").FirstOrDefault()
    Dim TipoAmbiente As String = ""

    If Not IsNothing(Ambiente) Then
        TipoAmbiente = Ambiente.Valor
    End If

    Dim URL_NS = ""

    @If TipoAmbiente = "QA" Then
        URL_NS = "https://6236630-sb1.app.netsuite.com/app/accounting/transactions/estimate.nl?id="
    ElseIf TipoAmbiente = "PRD" Then
        URL_NS = "https://6236630.app.netsuite.com/app/accounting/transactions/estimate.nl?id="
    End If
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Avanzada Negadas
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
                                    <div class="col-lg-6">
                                        <label for="example-text-input" class="col-form-label">No. De Negada:</label>
                                        <input class="form-control" type="text" id="NoPedido">
                                    </div>
                                    <div class="col-lg-6">
                                        <label for="example-text-input" class="col-form-label">Cliente:</label>
                                        <input class="form-control" type="text" id="example-text-input">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
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
                                <th scope="col">No. Negada</th>
                                <th scope="col">Fecha Creación</th>
                                <th scope="col">Mostrador</th>
                                <th scope="col"><i class="fa fa-gears"></i></th>
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.Negadas
                                Dim currentItem As NetsuiteLibrary.Clases.MapNegadas = item
                                @<tr id="@currentItem.NS_InternalID">
                                    <td scope="row">@currentItem.ClaveCliente - @currentItem.NombreCliente</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.NS_ExternalID)</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.FechaCreacion)</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Mostrador)</td>

                                <td scope="row">
                                    <ul class="d-flex justify-content-center">
                                        @If currentItem.Estatus = "closed" Then
                                            @<li onclick="CancelarOrdenVenta(@currentItem.NS_InternalID)" Class="mr-3" title="Cancelar Estimación"><a href="#" Class="text-secondary"><i Class="fa fa-ban"></i></a></li>
                                        End If
                                        <li onclick="DetallesPedido(@currentItem.NS_InternalID)" class="mr-3" title="Consultar Detalles"><a href="#" class="text-secondary"><i class="fa fa-search"></i></a></li>
                                        @*<li onclick="PrintPedido()" class="mr-3" title="Imprimir Pedido"><a href="@Url.Action("ImprimireEstimacionVer2", "Estimaciones", New With {.id = currentItem.NS_InternalID})" class="text-secondary"><i class="fa fa-print"></i></a></li>*@
                                        @*@If TipoAmbiente = "QA" Then
                                            @<li class="mr-3" title="Consultar En Netsuite (requiere usuario)"><a href="@String.Format("https://6236630-sb1.app.netsuite.com/app/accounting/transactions/salesord.nl?id={0}&whence=", currentItem.NS_InternalID)" class="text-secondary" target="_blank"><i class="fa fa-share"></i></a></li>
                                        ElseIf TipoAmbiente = "PRD" Then
                                            @<li class="mr-3" title="Consultar En Netsuite (requiere usuario)"><a href="@String.Format("https://6236630.app.netsuite.com/app/accounting/transactions/salesord.nl?id={0}&whence=", currentItem.NS_InternalID)" class="text-secondary" target="_blank"><i class="fa fa-share"></i></a></li>
                                        End If*@
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
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarNegadasDisponiblesVer2", "Negadas", FormMethod.Post, New With {.id = "ConsultarOrdenes"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaInicio" id="create-FechaInicio" />
        @<input type="hidden" name="FechaFin" id="create-FechaFin" />
        @<input type="hidden" name="Cliente" id="create-Cliente" />
        @<input type="hidden" name="NoPedido" id="create-NoPedido" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("CancelarNegada2", "Negadas", FormMethod.Post, New With {.id = "CancelarOrdenVenta"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="NS_ID" id="create-NS_ID" />
        @*@<input type="hidden" name="Folio" id="create-Folio" />*@

    End Using
</div>

<div class="modal fade bd-example-modal-lg modal-sl" id="ModalEdicion">
    <div class="modal-dialog modal-lg modal-sl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Detalles de la Negada</h5>
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

@*<script type="text/template" id='EjecucionListHidde'>
    @Html.Partial("EjecucionPedidoListHidden")
</script>*@

@Section Scripts

    <script type="text/template" id='DetalleTemplate'>

        <p><strong>Datos Principales.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">No. De Negada</label>
                    <input class="form-control" type="text" value="<%= NS_ExternalID %>" readonly id="Show_NoPedido">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Mostrador</label>
                    <input class="form-control" type="text" readonly value="<%= Nombre_Mostrador %>" id="Show_Mostrador">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Fecha De Creación:</label>
                    <input class="form-control" type="text" readonly value="<%= Fecha %>" id="Show_FechaCreacion">
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-12">
                    <label for="example-text-input" class="col-form-label">Cliente</label>
                    <input class="form-control" type="text" value="<%= Nombre_Cliente %>" readonly id="Show_Cliente">
                </div>
            </div>
        </div>
    
        <br />
        <p><strong>Productos.</strong></p>

        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Cantidad</th>
                            <th scope="col">Descripción</th>
                            <th scope="col">Mercancía</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_detalle, function(producto){ %>
                        <tr id="Detalle-<%= producto.idDetalleEstimacion %>">
                            <td nowrap><%= producto.Cantidad %></td>
                            <td nowrap><%= producto.DescripcionProducto %></td>
                            <td id="p<%= producto.idDetalleEstimacion %>"><%= producto.CantidadEntregada %></td>
                        </tr>
                        <% }); %>
                    </tbody>
                </table>
            </div>
        </div>
      
        <br />
        <p><strong>Resumen.</strong></p>
        <div class="row align-items-center">
            <div class="col-md-2 text-md-left">
                <p>SubTotal:</p>
                <p>Descuentos:</p>
                <p>Impuestos:</p>
                <p>Total:</p>
            </div>
            <div class="col-md-4 text-md-right">
                <p><%= accounting.formatMoney(SubTotal) %></p>
                <p><%= accounting.formatMoney(Descuento) %></p>
                <p><%= accounting.formatMoney(TotalImpuestos) %></p>
                <p><%= accounting.formatMoney(Total) %></p>
            </div>
            <div class="col-md-6 text-md-right">
            </div>
        </div>
        <br />
        <script>

            var DetalleConsulta = [];
            $('#GenerarFacturaSO').click(function () {
                try {
                    var id = $(this).attr("data-id");
                    var form = $("#CrearFactura");

                    form.children("[name='id']").val(id);
                    var dataForm = form.serialize();

                    $('#ModalEdicion').modal('toggle');
                    showLoading("Generando Factura...");
                    $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                        if (data.Tipo == 1) {

                            hideLoading();
                            toastr.success('Se Genero la Factura Con Exito...');
                            if (data.Valor.Estatus == "SO_Facturada") {
                                $('#' + id).find('td:eq(5)').html("<span class='badge badge-success'>Factura Creada</span>");
                                $('#' + id).find('td:eq(4)').html(data.Valor.Factura);
                            } else if (data.Valor.Estatus == "SO_Entrega") {
                                $('#' + id).find('td:eq(5)').html("<span class='badge badge-warning'>Mercancia Entregada</span>");
                            } else if (data.Valor.Estatus == "SO_Creada") {
                                $('#' + id).find('td:eq(5)').html("<span class='badge badge-info'>Orden Creada</span>");
                            }


                        }
                        else {
                            hideLoading();
                            toastr.error(data.Mensaje);
                        }
                    });
                }
                catch (err) {
                    alert("err");
                }
                finally {

                }

            });


            function ValidaMercancia(id) {

                var CantidadOriginal = parseFloat($('#c' + id).text());
                var CantidadIngresada = parseFloat($('#l' + id).val());

                if (CantidadIngresada > CantidadOriginal) {
                    $('#l' + id).val("");
                    toastr.error('El monto es superior o erroneo.');
                }
            }

        </script>

    </script>

    <script>
        var tablaDetalle = null;
        var EjecucionPedido = [];
        var Detalle = [];
        var contador = 0;
        var URL_DEC = "@URL_NS";

        $(document).ready(function () {

            tablaDetalle = $("#TableOrdenesVenta").DataTable({
                dom: 'Bfrtip',
                "info": false,
                responsive: false,
                "ordering": false,
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
            $("div.dt-buttons").appendTo("#TableOrdenesVenta_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TableOrdenesVenta_filter").css("margin-left", "10px");
        }

        function DetallesPedido(id) {

            var Url_Datos = "@Url.Action("GetNegadas", "Negadas", New With {.id = "__ID__"})"

            var link = Url_Datos.replace("__ID__", id);

            toastr.info("Obteniendo Detalles de la Negada...");

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
                            Detalle.push({ "id": item.idDetalleEstimacion });
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


        function CancelarOrdenVenta(id) {
            try {
                if (confirm("¿Desea Cancelar esta transacción? El proceso generara la cancelación de la negada.")) {
                    if (confirm("Confirmar Cancelación.")) {

                        var form = $("#CancelarOrdenVenta");

                        form.children("[name='NS_ID']").val(id);
                   
                        var dataForm = form.serialize();
                        showLoading("Obteniendo Negada...");
                        $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                            console.log(data);
                            if (data.Tipo == 1) {
                                hideLoading();
                                toastr.success('Se cancelo la estimación con éxito...');
                                var Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + id + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/Estimaciones/ImprimireEstimacionVer2/" + id + "' class='text-secondary'><i class='fa fa-print'></i></a></li></ul>"
                                var Estatus = "<span class='badge badge-danger'>Cancelado</span >"
                                $('#' + id).find('td:eq(4)').html(Details);

                                toastr.success(data.Mensaje);
                            }
                            else {
                                hideLoading();
                                toastr.error(data.Mensaje);
                            }
                        });

                    }
                }

            }

            catch (err) {
                hideLoading();
                toastr.error("Hubo un problema al procesar su solicitud... Intentelo más tarde.");
            }
        }

        function ConsultarOrdenesVentas() {

            var form = $("#ConsultarOrdenes");

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();
            var NoPedido = $('#NoPedido').val();

            form.children("[name='FechaInicio']").val(FechaIni);
            form.children("[name='FechaFin']").val(FechaFin);
            form.children("[name='NoPedido']").val(NoPedido);

            var dataForm = form.serialize();

            toastr.info("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        var Nombre = value.ClaveCliente + " - " + value.NombreCliente
                        var Estatus = value.Estatus
                        var Details = ""
                        var Invoices = ""

                        if (Estatus == "voided") {
                            Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/Estimaciones/ImprimireEstimacionVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li></ul>"
                        } else {
                            Details = "<ul class='d-flex justify-content-center'><li onclick='CancelarOrdenVenta(" + value.NS_InternalID + ', ' + "\"" + value.NS_ExternalID + "\")' class='mr-3' title='Cancelar Orden de Venta'><a href='#' class='text-secondary'><i class='fa fa-ban'></i></a></li><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/Estimaciones/ImprimireEstimacionVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li> </ul>"
                        }

                        tablaDetalle.row.add([
                            Nombre,
                            value.NS_ExternalID,
                            value.Fecha,
                            value.Mostrador,
                            Details,
                        ]).node().id = value.NS_InternalID;

                        tablaDetalle.draw(false);

                    });

                    toastr.success("Consulta realizada con éxito...");

                }
                else {
                    toastr.error(data.Mensaje);
                }
            });

        }
    </script>
End Section
