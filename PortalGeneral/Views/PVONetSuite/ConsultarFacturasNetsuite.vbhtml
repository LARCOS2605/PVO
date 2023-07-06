@modeltype PortalGeneral.FacturasNetsuiteViewModel
@Code
    ViewData("Title") = "Consultar Facturas Netsuite"
    ViewData("I_PNCF") = "class=active"
    ViewData("SubPage") = "PVO vs Netsuite"
End Code

@Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "RegistrarRecepcion"})
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(True)

    @<div class="col-lg-12 mt-5">
        <div class="card">
            <div class="card-header">
                <h4 class="header-title">Información Cliente</h4>
            </div>
            <div class="card-body">
                <div class="form-group">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-12">
                                @Html.LabelFor(Function(model) model.Customer, Idiomas.My.Resources.Resource.lbl_Clientes, New With {.class = "col-form-label"})
                                @Html.DropDownList("Customer", Nothing, Idiomas.My.Resources.Resource.cbb_Clientes, New With {.class = "form-control "})
                                @Html.ValidationMessageFor(Function(model) model.Customer)
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <button type="button" onclick="ConsultarFacturas()" class="btn btn-success" data-toggle="tooltip">Consultar Facturas</button>
            </div>
        </div>
    </div>

End Using

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <header><h4 class="header-title">Facturas Registradas</h4></header>

            <div class="single-table">
                <div class="table-responsive">
                    <table id="TablaDetalleFacturas" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Folio Factura</th>
                                <th scope="col">Forma de Pago</th>
                                <th scope="col">Subtotal</th>
                                <th scope="col">Iva</th>
                                <th scope="col">Total</th>
                                <th scope="col">Saldo Pendiente Pago</th>
                                <th scope="col">Saldo Pagado</th>
                                <th scope="col">Existencia PVO</th>
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
                <h5 class="modal-title">Historial de Pagos</h5>
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
    @Using Html.BeginForm("ConsultarFacturasCliente", "PVONetSuite", FormMethod.Post, New With {.id = "ConsultarFacturasPVO"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="ns_id" id="create-ns_id" />
    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("RecuperarFacturaPendiente", "PVONetSuite", FormMethod.Post, New With {.id = "RecuperarFacturasPVO"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="ns_id" id="create-ns_id" />
    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarHistorialPagos", "PVONetSuite", FormMethod.Post, New With {.id = "ConsultarPagosPVO"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="ns_id" id="create-ns_id" />
    End Using
</div>

@Section Scripts

    <script type="text/template" id='DetalleTemplate'>

        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Folio Pago Asociado Factura</th>
                            <th scope="col">Monto Total Pagos</th>
                            <th scope="col">Fecha</th>
                            <th scope="col"><i class="fa fa-gears"></i></th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_Detalle, function(pagos){ %>
                        <tr>
                            <td nowrap><%= pagos.FolioPago %></td>
                            <td nowrap><%= accounting.formatMoney(pagos.MontoPago) %></td>
                            <td nowrap><%= pagos.fecha %></td>
                            <td nowrap><a title="Imprimir Pago" onclick="printComprobantePago(<%= pagos.InternalID %>)" class="text-secondary"><i class="fa fa-print"></i></a></td>
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
            $("#Customer").select2();
            tablaDetalle = $("#TablaDetalleFacturas").DataTable({
                /*dom: 'Bfrtip',*/
                "info": false,
                responsive: false,
                "ordering": false,
                paging: false
            });
        });

        function ConsultarFacturas() {

            var form = $("#ConsultarFacturasPVO");

            var Cliente = $('#Customer').val();

            if (Cliente == "") {
                toastr.error("No se ha seleccionado un cliente valido");
                return false;
            }

            form.children("[name='ns_id']").val(Cliente);
            var dataForm = form.serialize();
            toastr.info("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    console.log(data.Valor);

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        var control = ""
                        var Estatus = ""

                        if (value.ExistePVO == "True") {
                            Estatus = "<span class='badge badge-success'>En PVO</span>"
                        } else {

                            control = "<ul class='d-flex justify-content-center'> <li onclick='RecuperarFactura(" + value.InternalID + ")' class='mr-3' title='Recuperar Factura'><a href='#' class='text-secondary'><i class='fa fa-exchange'></i></a></li> </ul>"
                            Estatus = "<span class='badge badge-danger'>Faltante</span>"
                        }

                        tablaDetalle.row.add([
                            value.FolioFactura,
                            value.FormaPago,
                            accounting.formatMoney(value.SubtotalFactura),
                            accounting.formatMoney(value.IvaFactura),
                            accounting.formatMoney(value.TotalFactura),
                            accounting.formatMoney(value.SaldoPendiente),
                            "<button type='button' onclick='ConsultarHistorialPagos(" + value.InternalID + ")' class='btn btn-flat btn-info btn-xs mb-3'>" + accounting.formatMoney(value.SaldoPagado) + "</button>",
                            Estatus,
                            control,
                        ]).node().id = value.InternalID;

                        tablaDetalle.draw(false);

                    });

                    toastr.success("Consulta realizada con exito...");

                }
                else {
                    toastr.error(data.Mensaje);
                }
            });

        }

        function ConsultarHistorialPagos(id) {

            var form = $("#ConsultarPagosPVO");

            if (id == "") {
                toastr.error("El Documento no es valido.");
                return false;
            }

            form.children("[name='ns_id']").val(id);
            var dataForm = form.serialize();
            toastr.info("Consultando Historial Pagos...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    var template = _.template(document.getElementById("DetalleTemplate").textContent);
                    $("#areaDetalles").html("");
                    var html = template({ l_Detalle : data.Valor});
                    $("#areaDetalles").html(html);
                    $('#ModalEdicion').modal('toggle');
                }
                else {
                    toastr.error(data.Mensaje);
                }
            });

        }

        function printComprobantePago(id) {

            if (id == "") {
                toastr.error("El Documento no es valido.");
                return false;
            }

           var url = "@Url.Action("ImprimirPago", "PVONetSuite", New With {.id = "__ID__"})";
           var gets = url.replace("__ID__", id);

           toastr.info("Descargando Comprobante Pago...");

           window.location.replace(gets);

        }

        function RecuperarFactura(id) {

            var form = $("#RecuperarFacturasPVO");

            if (id == "") {
                toastr.error("No hay una factura valida para recuperar");
                return false;
            }

            form.children("[name='ns_id']").val(id);
            var dataForm = form.serialize();
            toastr.info("Recuperando Factura...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    $('#' + id).find('td:eq(7)').html("<span class='badge badge-success'>En PVO</span>");
                    $('#' + id).find('td:eq(8)').html("");

                    toastr.success("Se Recupero la factura con exito!...");

                }
                else {
                    toastr.error(data.Mensaje);
                }
            });

        }
    </script>
End Section
