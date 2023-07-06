@modeltype PortalGeneral.GeneraPagosViewModel
@Code
    ViewData("Title") = "Generar Pago"
    ViewData("I_PGP") = "class=active"
    ViewData("SubPage") = "Pagos"
End Code

@Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "RegistrarPagos"})
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(True)

    @<div class="col-lg-12 mt-5">
        <div class="card">
            <div class="card-body">
                <h4 class="header-title">Información del Cliente:</h4>
                <div class="form-group">
                    <div class="row">
                        <div class="col-lg-12">
                            @Html.LabelFor(Function(model) model.Customer, "Cliente:", New With {.class = "col-form-label"})
                            <div class="input-group-append">
                                @Html.DropDownList("Customer", Nothing, "--- Seleccione un Cliente ---", New With {.class = "form-control"})
                                @Html.ValidationMessageFor(Function(model) model.Customer)
                                <button type="button" onclick="ConsultarPagosDisponibles()" class="btn btn-success" data-toggle="tooltip">Buscar Facturas</button>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            @Html.LabelFor(Function(model) model.MetodoPago, "MetodoPago:", New With {.class = "col-form-label"})
                            @Html.DropDownList("MetodoPago", Nothing, "--- Seleccione un Metodo de Pago ---", New With {.class = "form-control"})
                            @Html.ValidationMessageFor(Function(model) model.MetodoPago)
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            @Html.LabelFor(Function(model) model.FechaPago, "Fecha de Pago:", New With {.class = "col-form-label"})
                            @Html.TextBoxFor(Function(model) model.FechaPago, New With {.class = "form-control", .type = "date", .max = "hoy", .min = "validar", .onclick = "Fecha()"})
                            @Html.ValidationMessageFor(Function(model) model.FechaPago)
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-6">
                            @Html.LabelFor(Function(model) model.MontoTotalPago, "Monto Total:", New With {.class = "col-form-label"})
                            @Html.TextBoxFor(Function(model) model.MontoTotalPago, New With {.class = "form-control", .type = "number"})
                            @Html.ValidationMessageFor(Function(model) model.MontoTotalPago)
                        </div>
                        <div class="col-lg-6">
                            @Html.LabelFor(Function(model) model.MontoTotalPagoIngresados, "Total Pagos Ingresados:", New With {.class = "col-form-label"})
                            @Html.TextBoxFor(Function(model) model.MontoTotalPagoIngresados, New With {.class = "form-control", .type = "text", .readonly = "readonly"})
                            @Html.ValidationMessageFor(Function(model) model.MontoTotalPagoIngresados)
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-12">
                                @Html.LabelFor(Function(model) model.Nota, "Nota:", New With {.class = "col-form-label"})
                                @Html.TextAreaFor(Function(model) model.Nota, New With {.class = "form-control"})
                                @Html.ValidationMessageFor(Function(model) model.Nota)
                            </div>
                        </div>
                    </div>


                    @* Tarjeta bancaria *@
                    @*<div class="form-group" id="Select_Banco">
            <div class="row">
                <div class="col-lg-12">
                    @Html.LabelFor(Function(model) model.Banco, "Banco:", New With {.class = "col-form-label"})
                    @Html.DropDownList("Banco", Nothing, "--- Seleccionar Banco ---", New With {.class = "form-control"})
                    @Html.ValidationMessageFor(Function(model) model.Banco)
                </div>
            </div>
        </div>
        <div class="form-group" id="Select_NumTarjeta">
            <div class="row">
                <div class="col-lg-12">
                    @Html.LabelFor(Function(model) model.NumBanco, "Numero de Tarjeta:", New With {.class = "col-form-label"})
                    @Html.TextBoxFor(Function(model) model.NumBanco, New With {.class = "form-control", .type = "number", .maxlength = "16", .min = "0"})
                    @Html.ValidationMessageFor(Function(model) model.NumBanco)
                </div>
            </div>
        </div>*@

                    @* Transferencia Electrónica, tarjetas de debito y credito *@
                    <div class="form-group" id="T_BancoOrd">
                        <div class="row">
                            <div class="col-lg-6">
                                @Html.LabelFor(Function(model) model.BancoOrdenante, "Banco Ordenante:", New With {.class = "col-form-label"})
                                @Html.DropDownList("BancoOrdenante", Nothing, "--- Seleccionar Banco ---", New With {.class = "form-control"})
                                @Html.ValidationMessageFor(Function(model) model.BancoOrdenante)
                            </div>
                            <div class="col-lg-6">
                                @Html.LabelFor(Function(model) model.NumCuentaOrdenante, "Numero de Cuenta Ordenante:", New With {.class = "col-form-label"})
                                @Html.TextBoxFor(Function(model) model.NumCuentaOrdenante, New With {.class = "form-control", .type = "text"})
                                @Html.ValidationMessageFor(Function(model) model.NumCuentaOrdenante)
                            </div>
                        </div>
                    </div>
                    <div class="form-group" id="T_NoRefPago">
                        <div class="row">
                            <div class="col-lg-6">
                                @Html.LabelFor(Function(model) model.NoRefPago, "Numero Referencia Pago:", New With {.class = "col-form-label"})
                                @Html.TextBoxFor(Function(model) model.NoRefPago, New With {.class = "form-control", .type = "number", .maxlength = "16", .min = "0"})
                                @Html.ValidationMessageFor(Function(model) model.NoRefPago)
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div Class="col-lg-12" id="PagoshiddenDivDetalles">
        </div>
    </div>

End Using

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <header><h4 class="header-title">Facturas Del Cliente</h4></header>

            <div class="single-table">
                <div class="table-responsive">
                    <table id="TableOrdenesVenta" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Fecha</th>
                                @*<th scope="col">Fecha Vencimiento</th>*@
                                <th scope="col">No. Factura</th>
                                @*<th scope="col">Condicion Pago</th>*@
                                <th scope="col">Importe Original</th>
                                <th scope="col">Importe a Pagar</th>
                                <th scope="col">Estatus</th>
                                <th scope="col">Pago</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                        @*<tfoot>
                            <tr style="text-align:right">
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td><strong>Total :     </strong></td>
                                <td id="DataPtotal">$0.00</td>
                            </tr>
                        </tfoot>*@
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <button type="button" onclick="AlmacenarPagos()" class="btn btn-success" data-toggle="tooltip">Guardar Pagos</button>
        </div>
    </div>
</div>

<script type="text/template" id='PagosListHidde'>
    @Html.Partial("PagosListHidden")
</script>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarFacturasPorCliente", "Pagos", FormMethod.Post, New With {.id = "ConsultarDatos"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="id" id="create-id" />
    End Using
</div>

@Section Scripts

    <script>
        var l_FormasValidas = @Html.Raw(ViewBag.MetodosValidos);
        var l_FacturasCliente = new Array();
        var l_PagosAplicados = new Array();
        var DetallesPagos = null;
        var tablaDetalle = null;
        var tablaPagos = null;
        var contador = 0;
        var Total = 0;

        window.onpageshow = function (event) {
            if (event.persisted) {
                window.location.reload()
            }
        };

        window.addEventListener('DOMContentLoaded', (evento) => {
            /* Obtenemos la fecha de hoy en formato ISO */
            const hoy_fecha = new Date().toISOString().substring(0, 10);
            var hoy = new Date();
            hoy.setMonth(hoy.getMonth() - 12);
            var mins = hoy.getFullYear() + '-' + String(hoy.getMonth() + 1).padStart(2, '0') + '-' + String(hoy.getDate()).padStart(2, '0');
            //console.log(mins);
            /* Buscamos solo las etiquetas que tengan el atributo "max" en "hoy" */
            document.querySelectorAll("input[type='date'][max='hoy']")
                .forEach(elemento => {
                    /* A cada elemento encontrado le asignamos el atributo "max" */
                    elemento.max = hoy_fecha;
                });
            /* Validamos que solo permita un año como mínimo */
            document.querySelectorAll("input[type='date'][min='validar']")
                .forEach(elemento => {
                    /* A cada elemento encontrado le asignamos el atributo "max" */
                    elemento.min = mins;
                });
        });

        function Fecha() {
            window.addEventListener('DOMContentLoaded', (evento) => {
                /* Obtenemos la fecha de hoy en formato ISO */
                const hoy_fecha = new Date().toISOString().substring(0, 10);
                var hoy = new Date();
                //var fecha_hoy = hoy.getFullYear() + '-' + String(hoy.getMonth() + 1).padStart(2, '0') + '-' + String(hoy.getDate()).padStart(2, '0');
                var minimo = hoy.setMonth(hoy.getMonth() - 12);
                /* Buscamos solo las etiquetas que tengan el atributo "max" en "hoy" */
                document.querySelectorAll("input[type='date'][max='hoy']")
                    .forEach(elemento => {
                        /* A cada elemento encontrado le asignamos el atributo "max" */
                        elemento.max = hoy_fecha;
                    });
                /* Validamos que solo permita un año como mínimo */
                document.querySelectorAll("input[type='date'][min='validar']")
                    .forEach(elemento => {
                        /* A cada elemento encontrado le asignamos el atributo "max" */
                        elemento.min = minimo;
                    });
            });
        }

        $(document).ready(function () {
            $("#T_BancoOrd").hide();
            $("#T_NoRefPago").hide();
            //$('#NumCuentaOrdenante').inputmask({
            //    mask: '(3(4|7)99 9{6} 9{5}|3999 9{4} 9{4} 9{4}|9{4} 9{4} 9{4} 9{4})'
            //});
            $('#MontoTotalPagoIngresados').val("$0.00");
            $('#Customer').val("");
            $('#MetodoPago').val("");
            $('#FechaPago').val("");
            $('#Nota').val("");
            $('#BancoOrdenante').val("");
            $('#NumCuentaOrdenante').val("");
            $('#NoRefPago').val("");
            $('#MontoTotalPago').val("");
            
                       
            $("#Customer").select2('');

            tablaDetalle = $("#TableOrdenesVenta").DataTable({
                "bPaginate": false, "ordering": false, "searching": false, "bInfo": false, "language": {
                    "emptyTable": "No hay Pagos disponibles."
                }
            });
        });

        function ConsultarPagosDisponibles() {
            RecalcularTotales();
            l_FacturasCliente.length = 0;
            var id = $("#Customer").val();
            var form = $("#ConsultarDatos");

            if (id == "") {
                toastr.error('El cliente seleccionado, no es valido...');
                return false;
            }

            form.children("[name='id']").val(id);
            var dataForm = form.serialize();

            toastr.info('Obteniendo Facturas...');

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                console.log(data);
                if (data.Tipo == 1) {
                    tablaDetalle.clear().draw();
                    $.each(data.Valor, function (index, value) {

                        var spanDatos = "";
                        var spanPago = "";

                        if (value.idEstatus == 4) {
                            spanDatos = "<span id='E" + value.NS_InternalID + "'  class='status-p bg-success'>Pagado</span>"
                        } else {
                            spanDatos = "<span id='E" + value.NS_InternalID + "'  class='status-p bg-primary'>Pendiente</span>"
                            /*spanPago = "<input class='form-control' type='number' id='l" + value.NS_InternalID + "' min='0' onblur='ValidaPagoIngresado(" + value.NS_InternalID + ")' onclick='ValidaProntoPago(" + value.NS_InternalID + ")'>"*/
                            spanPago = "<input class='form-control' type='number' id='l" + value.NS_InternalID + "' min='0' onblur='ValidaPagoIngresado(" + value.NS_InternalID + ")'>"
                        }

                        if (value.Importe_Adeudado == 0) {
                            l_FacturasCliente.push({ "NS_InternalID": value.NS_InternalID, "NS_ExternalID": value.NS_ExternalID, "ImporteAdeudado": value.Total, "MetodoPagoSAT": value.MetodoPagoSAT });
                        } else {
                            l_FacturasCliente.push({ "NS_InternalID": value.NS_InternalID, "NS_ExternalID": value.NS_ExternalID, "ImporteAdeudado": value.Importe_Adeudado, "MetodoPagoSAT": value.MetodoPagoSAT });
                        }

                        tablaDetalle.row.add([
                            value.Fecha,
                            //value.Fecha_Venci,
                            value.NS_ExternalID + " - " + value.MetodoPagoSAT,
                            //value.MetodoPagoSAT,
                            accounting.formatMoney(value.Total),
                            accounting.formatMoney(value.Importe_Adeudado),
                            spanDatos,
                            spanPago
                        ]).draw(false);
                    });

                    $('#MontoTotalPagoIngresados').val("$0.00");
                    toastr.success('Facturas consultadas con exito...');

                }
                else {
                    toastr.error('Hubo un problema al generar la factura. Detalles: ' + data.Mensaje);
                }
            });


        }
         
    
        function AlmacenarPagos() {

            showLoading("Generando Pagos de Factura...");

            var pagototal = $('#MontoTotalPago').val();
            var fechapago = $('#FechaPago').val();
            var hoy = new Date();
            //var fecha_hoy = hoy.toLocaleDateString('es-MX');
            var fecha_hoy = hoy.getFullYear() + '-' + String(hoy.getMonth() + 1).padStart(2, '0') + '-' + String(hoy.getDate()).padStart(2, '0');
            hoy.setMonth(hoy.getMonth() - 12);
            var mins = hoy.getFullYear() + '-' + String(hoy.getMonth() + 1).padStart(2, '0') + '-' + String(hoy.getDate()).padStart(2, '0');

            //Validacion Acciones
            if (fechapago > fecha_hoy) {
                hideLoading();
                toastr.error('La fecha de pago no puede ser mayor al día que se aplica...');
                return false
            }
            if (fechapago < mins) {
                hideLoading();
                toastr.error('La fecha de pago no puede ser menor a un año al día que se aplica...');
                return false
            }
            if ($('#Customer').val() == "") {
                hideLoading();
                toastr.error('El Cliente Ingresado es Invalida...');
                return false
            }

            if ($('#MetodoPago').val() == "") {
                hideLoading();
                toastr.error('No se ha seleccionado algun metodo de pago...');
                return false
            }

            if ($('#FechaPago').val() == "") {
                hideLoading();
                toastr.error('No se ha seleccionado alguna fecha de pago...');
                return false
            }

            if ($('#MontoTotalPago').val() == "") {
                hideLoading();
                toastr.error('No se ha ingresado un total de pagos...');
                return false
            }

            if (accounting.formatMoney(pagototal) != $('#MontoTotalPagoIngresados').val()) {
                hideLoading();
                toastr.error('Existe una diferencia entre los montos agregados y el total ingresado...');
                return false
            }

            var templatePago = _.template(document.getElementById("PagosListHidde").textContent);
            $("#PagosListHidden").html("");

            $.each(l_FacturasCliente, function (index, value) {
                if (isNaN($('#l' + value.NS_InternalID).val()) || ($('#l' + value.NS_InternalID).val() == "") || ($('#E' + value.NS_InternalID).val() == "Pagado")) {

                } else {

                    var Pago = parseFloat($('#l' + value.NS_InternalID).val());
                    l_PagosAplicados.push({ "NS_InternalID": value.NS_InternalID, "ImporteAplicado": Pago });
                }
            });

            if (l_PagosAplicados.length === 0) {
                hideLoading();
                toastr.error('No se ha registrado ningun pago valido...');
                return false
            }


            _.each(l_PagosAplicados, function (item) {
                var dataItem = item
                var html = templatePago({ Pagos: dataItem, contador: contador })
                $("#PagoshiddenDivDetalles").append(html);
                contador++;

            }, this);

            $('#RegistrarPagos').submit();
        }

        function ValidaProntoPago(id) {
          /*  toastr.success('Calculando descuento pronto pago...');*/
            $('#l' + id).val("");
            $.each(l_FacturasCliente, function (index, value) {
                if (value.NS_InternalID == id) {
                    if (value.MetodoPagoSAT == "PPD") {
                        if (value.ImporteAdeudado > 0) {
                            var Descuento = value.ImporteAdeudado * 0.10;
                            var total_descuento = value.ImporteAdeudado - Descuento;

                            $('#l' + id).val(parseFloat(total_descuento));
                        }
                    }
                }
               
            });
        }

        function ValidaPagoIngresado(id) {
            $.each(l_FacturasCliente, function (index, value) {
                if (value.NS_InternalID == id) {

                    var Pago = parseFloat($('#l' + id).val());
                    var PagoPendiente = parseFloat(value.ImporteAdeudado);
                    console.log(PagoPendiente);
                    if (Pago > PagoPendiente) {
                        $('#l' + id).val("");
                        toastr.error('El monto es superior o erroneo.');
                    } else if (Pago <= 0) {
                        $('#l' + id).val("");
                        toastr.error('El monto es inferior o erroneo.');
                    }
                }
            });

            RecalcularTotales()
        }

        function RecalcularTotales() {
            Total = 0;
            $.each(l_FacturasCliente, function (index, value) {
                if (isNaN($('#l' + value.NS_InternalID).val())) {

                } else {
                    if ($('#l' + value.NS_InternalID).val() != "") {
                        var Pago = parseFloat($('#l' + value.NS_InternalID).val());

                        Total = Total + Pago;
                    }
                }

            });

            $('#MontoTotalPagoIngresados').val(accounting.formatMoney(Total));
        }

        $('#MetodoPago').change(function () {
            var id = $('#MetodoPago').val();

            if (jQuery.inArray(id, l_FormasValidas) != -1) {
                $("#T_BancoOrd").show();
                $("#T_NoRefPago").show();
            } else {
                $("#T_BancoOrd").hide();
                $("#T_NoRefPago").hide();
            }

        });

        $('#MontoTotalPago').change(function () {
            var Pago = $('#MontoTotalPago').val();

            if (Pago <= 0) {
                $('#MontoTotalPago').val("");
                toastr.error('El monto es inferior o erroneo.');
            }

        });

    </script>
End Section
