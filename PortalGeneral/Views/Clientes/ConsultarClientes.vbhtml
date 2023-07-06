@Code
    ViewData("Title") = Idiomas.My.Resources.Resource.Tittle_ConsultaCliente
    ViewData("I_CoC") = "class=active"
    ViewData("SubPage") = Idiomas.My.Resources.Resource.SubPage_Cliente
End Code


<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Sincronizar Cliente Especifico
                        </a>
                    </div>
                    <div id="accordion21" class="collapse" data-parent="#accordion2">
                        <div class="card-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <label for="example-text-input" class="col-form-label">Clave de Cliente/Nombre del Cliente:</label>
                                        <input class="form-control" type="text" id="sinc_cliente">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <Button type="button" onclick="RecuperarCliente()" Class="btn btn-primary">Sincronizar Cliente</Button>
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
                    <table id="TableClientes" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">@Idiomas.My.Resources.Resource.lblt_ClaveCliente</th>
                                <th scope="col">@Idiomas.My.Resources.Resource.lblt_NombreCliente</th>
                                <th scope="col">@Idiomas.My.Resources.Resource.lblt_RFC</th>
                                <th scope="col">@Idiomas.My.Resources.Resource.lblt_NivelDescuento</th>
                                <th scope="col">Limite de Credito</th>
                                <th scope="col">Terminos</th>
                                <th scope="col">Dias de Atraso</th>
                                <th scope="col">@Idiomas.My.Resources.Resource.lblt_TipoCliente</th>

                                @*<th scope="col">Descripción</th>
                                    <th scope="col">Acciones</th>*@
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.Clientes
                                Dim currentItem As Customers_ALT = item
                                @<tr>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.ClaveCliente)</td>
                                    <td>@Html.DisplayFor(Function(modelItem) currentItem.NombreCliente)</td>
                                    <td>@Html.DisplayFor(Function(modelitem) currentItem.RFC)</td>
                                    <td>@Html.DisplayFor(Function(modelitem) currentItem.Nivel_Descuento)</td>
                                    <td>@Convert.ToDecimal(currentItem.Limite_Credito).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                    <td>@Html.DisplayFor(Function(modelitem) currentItem.Terminos)</td>
                                    <td>@Math.Floor(Convert.ToDecimal(currentItem.Dias_Atraso))</td>
                                    <td>@Html.DisplayFor(Function(modelitem) currentItem.tipo_cliente)</td>
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
    @Using Html.BeginForm("SincronizarClientesNetsuite", "Clientes", FormMethod.Post, New With {.id = "SincronizarClientes"})
        @Html.AntiForgeryToken()
    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("SincronizarClientesEspecifico", "Clientes", FormMethod.Post, New With {.id = "SincronizarClienteEspecifico"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="idCliente" id="create-idCliente" />
    End Using
</div>

@Section Scripts

    <script>
        var tablaDetalle = null;

        $(document).ready(function () {
            tablaDetalle = $("#TableClientes").DataTable({
                dom: 'Bfrtip',
                "info": false,
                buttons: [{
                    text: '<i class="fa fa-file-excel-o"></i>',
                    extend: 'excelHtml5',
                    title: 'Clientes Disponibles',
                    className: 'btn btn-flat btn-info mb-3'
                },
                {
                    text: '<i class="fa fa-file-pdf-o"></i>',
                    extend: 'pdfHtml5',
                    title: 'Almacenes Disponibles',
                    className: 'btn btn-flat btn-success mb-3'
                },
                {
                    text: '<i class="fa fa-repeat"></i>',
                    className: 'btn btn-flat btn-primary mb-3',
                    action: function (e, dt, node, config) {
                        SincronizarDatosNetsuite();
                    }
                }]
            });
            changeExcelButton();
        });

        function changeExcelButton() {
            //Modificar boton de Exportar a Excel
            $("div.dt-buttons").appendTo("#TableAlmacenes_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TableClientes_filter").css("margin-left", "10px");
        }


        function SincronizarDatosNetsuite() {
            if (confirm("@Idiomas.My.Resources.Resource.Alert_ConfirmSyncCustom")) {

                try {
                    var form = $("#SincronizarClientes");
                    var dataForm = form.serialize();
                    showLoading("@Idiomas.My.Resources.Resource.SL_SyncCustomer");

                    $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                        if (data.Tipo == 1) {

                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {


                                tablaDetalle.row.add([
                                    value.NS_ExternalID,
                                    value.Nombre,
                                    value.RFC,
                                    value.NombrePrecio,
                                    accounting.formatMoney(value.Limite_Credito),
                                    value.Terminos,
                                    value.Dias_Atraso,
                                    value.TipoCliente,
                                ]).draw(false);
                            });

                            hideLoading();
                            toastr.success('@Idiomas.My.Resources.Resource.Alert_SusSyncCustomer');
                        } else {
                            hideLoading();
                        }


                    });
                } catch (error) {
                    hideLoading();
                }



            }
        }

        function RecuperarCliente() {
            try {
                var claveDatos = $("#sinc_cliente").val();

                if (claveDatos == '') {
                    toastr.error("Para usar esta función, es necesario ingresar una clave de cliente o nombre de cliente.");
                    return false;
                }

                var form = $("#SincronizarClienteEspecifico");

                form.children("[name='idCliente']").val(claveDatos);

                var dataForm = form.serialize();
                showLoading("Sincronizando Cliente...");
                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {

                            tablaDetalle.clear().draw();

                            $.each(data.Valor, function (index, value) {


                                tablaDetalle.row.add([
                                    value.NS_ExternalID,
                                    value.Nombre,
                                    value.RFC,
                                    value.NombrePrecio,
                                    accounting.formatMoney(value.Limite_Credito),
                                    value.Terminos,
                                    value.Dias_Atraso,
                                    value.TipoCliente,
                                ]).draw(false);
                            });

                            hideLoading();
                            toastr.success('@Idiomas.My.Resources.Resource.Alert_SusSyncCustomer');
                    } else {
                            hideLoading();
                    }
                });
            }

            catch (err) {
                hideLoading();
                toastr.error("Hubo un problema al procesar su solicitud... Intentelo más tarde.");
            }
        }
    </script>
End Section
