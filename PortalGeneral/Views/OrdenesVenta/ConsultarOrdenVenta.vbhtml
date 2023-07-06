
@Code
    ViewData("Title") = "Consultar Ordenes de Venta"
    ViewData("I_SOCon") = "class=active"
    ViewData("SubPage") = "Sales Order"

    Dim db As New PVO_NetsuiteEntities
    Dim Ambiente = (From n In db.Parametros Where n.Clave = "Ambiente").FirstOrDefault()
    Dim TipoAmbiente As String = ""

    If Not IsNothing(Ambiente) Then
        TipoAmbiente = Ambiente.Valor
    End If

    Dim URL_NS = ""

    @If TipoAmbiente = "QA" Then
        URL_NS = "https://6236630-sb1.app.netsuite.com/app/accounting/transactions/salesord.nl?id="
    ElseIf TipoAmbiente = "PRD" Then
        URL_NS = "https://6236630.app.netsuite.com/app/accounting/transactions/salesord.nl?id="
    End If
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <div id="accordion2" class="according accordion-s2">
                <div class="card">
                    <div class="card-header">
                        <a class="card-link" data-toggle="collapse" href="#accordion21">
                            Consulta Avanzada Ordenes Venta
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
                                        <label for="example-text-input" class="col-form-label">No. De Pedido:</label>
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
                                <th scope="col">No. Pedido</th>
                                <th scope="col">Fecha Creacion</th>
                                <th scope="col">Mostrador</th>
                                <th scope="col">Factura</th>
                                <th scope="col">Estatus</th>
                                <th scope="col"><i class="fa fa-gears"></i></th>
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.SalesOrder
                                Dim currentItem As NetsuiteLibrary.Clases.MapSalesOrder = item
                                @<tr id="@currentItem.NS_InternalID">
                                    <td scope="row">@currentItem.ClaveCliente - @currentItem.NombreCliente</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.NS_ExternalID)</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.FechaCreacion)</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Mostrador)</td>

                                    @If (currentItem.Factura = "Sin Factura") Then
                                        @<td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Factura)</td>
                                    Else
                                        @<td scope="row"><button type="button" onclick='ConsultarDatosFactura(@currentItem.NS_InternalID_Factura)' class='btn btn-light btn-xs mb-3'>@Html.DisplayFor(Function(modelItem) currentItem.Factura)</button></td>
                                    End If

                                    <td scope="row">
                                        @if currentItem.Estatus = "pendingFulfillment" And currentItem.Factura = "Sin Factura" Then
                                            @<span class="badge badge-info"> Orden Creada </span>

                                        ElseIf currentItem.Estatus = "pendingBilling" Then
                                            @<span Class="badge badge-warning"> Mercancia Entregada </span>

                                        ElseIf currentItem.Estatus = "fullyBilled" Then
                                            @<span Class="badge badge-success"> Factura Creada </span>

                                        ElseIf currentItem.Estatus = "pendingFulfillment" And currentItem.Factura <> "Sin Factura" Then
                                            @<span Class="badge badge-primary"> Factura Creada/Pendiente Entrega </span>

                                        ElseIf currentItem.Estatus = "closed" Then
                                            @<span Class="badge badge-danger"> Cancelado </span>
                                        End If
                                    </td>
                                    <td scope="row">
                                        <ul class="d-flex justify-content-center">
                                            @If currentItem.Estatus = "pendingFulfillment" And currentItem.Factura = "Sin Factura" Then
                                                @<li onclick="CancelarOrdenVenta(@currentItem.NS_InternalID, '@currentItem.NS_ExternalID')" class="mr-3" title="Cancelar Orden de Venta"><a href="#" class="text-secondary"><i class="fa fa-ban"></i></a></li>
                                                @*@<li Class="mr-3" title="Editar Orden de Venta"><a href="@Url.Action("EditarOrdenDeVenta", "OrdenesVenta", New With {.id = currentItem.NS_InternalID})" class="text-secondary"><i class="fa fa-pencil"></i></a></li>*@
                                            End If
                                            <li onclick="DetallesPedido(@currentItem.NS_InternalID)" class="mr-3" title="Consultar Detalles"><a href="#" class="text-secondary"><i class="fa fa-search"></i></a></li>
                                            <li onclick="PrintPedido()" class="mr-3" title="Imprimir Pedido"><a href="@Url.Action("ImprimirOrdenVentaVer2", "OrdenesVenta", New With {.id = currentItem.NS_InternalID})" class="text-secondary"><i class="fa fa-print"></i></a></li>
                                            @If TipoAmbiente = "QA" Then
                                                @<li class="mr-3" title="Consultar En Netsuite (requiere usuario)"><a href="@String.Format("https://6236630-sb1.app.netsuite.com/app/accounting/transactions/salesord.nl?id={0}&whence=", currentItem.NS_InternalID)" class="text-secondary" target="_blank"><i class="fa fa-share"></i></a></li>
                                            ElseIf TipoAmbiente = "PRD" Then
                                                @<li class="mr-3" title="Consultar En Netsuite (requiere usuario)"><a href="@String.Format("https://6236630.app.netsuite.com/app/accounting/transactions/salesord.nl?id={0}&whence=", currentItem.NS_InternalID)" class="text-secondary" target="_blank"><i class="fa fa-share"></i></a></li>
                                            End If
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
    @Using Html.BeginForm("ConsultarOrdenesVentaDisponiblesVer2", "OrdenesVenta", FormMethod.Post, New With {.id = "ConsultarOrdenes"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="FechaInicio" id="create-FechaInicio" />
        @<input type="hidden" name="FechaFin" id="create-FechaFin" />
        @<input type="hidden" name="Cliente" id="create-Cliente" />
        @<input type="hidden" name="NoPedido" id="create-NoPedido" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("RecuperarFactura", "OrdenesVenta", FormMethod.Post, New With {.id = "RecuperarFacturaPedido"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="NS_ID" id="create-NS_ID" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("CancelarOrdenVenta2", "OrdenesVenta", FormMethod.Post, New With {.id = "CancelarOrdenVenta"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="NS_ID" id="create-NS_ID" />
        @<input type="hidden" name="Folio" id="create-Folio" />

    End Using
</div>
<div style="display:none;">
    @Using Html.BeginForm("EditarOrdenDeVenta", "OrdenesVenta", FormMethod.Post, New With {.id = "EditarOrden"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="NS_ID" id="create-NS_ID" />

    End Using
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

<div class="modal fade bd-example-modal-lg" id="ModalFacturacion">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Detalles de Facturación</h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body" id="areaDetallesFacturacion">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>

<script type="text/template" id='EjecucionListHidde'>
    @ViewData("Title")

</script>

@Section Scripts

    <script type="text/template" id='DetalleTemplate'>

        <p><strong>Datos Principales.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">No. De Pedido</label>
                    <input class="form-control" type="text" value="<%= NS_ExternalID %>" readonly id="Show_NoPedido">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Mostrador</label>
                    <input class="form-control" type="text" readonly value="<%= Nombre_Mostrador %>" id="Show_Mostrador">
                </div>
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">Fecha De Creacion:</label>
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
        <%
        if((Estatus == 'SO_Facturada') || (Estatus == 'SO_FacPenEnt')){
        %>
        <%
        if((NoFactura != null)){
        %>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-4">
                    <label for="example-text-input" class="col-form-label">No. De Factura</label>
                    <div class="input-group-append">
                        <input class="form-control" type="text" value="<%= NoFactura %>" readonly id="Show_NoPedido">
                        <button type="button" id="ImprimirFacturaPedido" data-id="<%= NS_ID_Invoice %>" class="btn btn-info"><i class="fa fa-print"></i> Imprimir Factura </button>
                    </div>
                </div>
            </div>
        </div>
        <%
        }
        %>

        <%
        if((UUID != null)){
        %>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">UUID</label>
                    <div class="input-group-append">
                        <input class="form-control" type="text" value="<%= UUID %>" readonly id="UUID_INV">
                        <button type="button" id="GetPDF_Timbrado" data-id="<%= NS_ID_pdf %>" class="btn btn-info"><i class="fa fa-file-pdf-o"></i> PDF</button>
                        <button type="button" id="GetXML_Timbrado" data-id="<%= NS_ID_xml %>" class="btn btn-info"><i class="fa fa-file"></i> XML</button>
                    </div>
                </div>
            </div>
        </div>
        <%
        }
        %>

        <%
        }
        %>
        <br />
        <p><strong>Productos.</strong></p>

        <%
        if(Estatus == 'Ejecución de la orden pendiente'){
        %>
        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Descripcion</th>
                            <th scope="col">Pendiente de Entrega</th>
                            <th scope="col">Lote</th>
                            <th scope="col">Estatus</th>
                            <th scope="col">Mercancia Entregada</th>
                            <th scope="col">Entregar Mercancia</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_detalle, function(producto){ %>
                        <tr id="Detalle-<%= producto.idDetalleSalesOrder %>">
                            <% var cantidadDisp = parseFloat(producto.Cantidad) - parseFloat(producto.CantidadEntregada) %>
                            <td nowrap><%= producto.DescripcionProducto %></td>
                            <td id="c<%= producto.idDetalleSalesOrder %>"><%= cantidadDisp %></td>
                            <td><%= producto.NumLote %></td>
                            <td>
                                <%
                                if(producto.EstatusEntrega == 'Merc_Creada'){
                                %>
                                <span class="badge badge-info">
                                    Pendiente de Entrega
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Parcial'){
                                %>
                                <span Class="badge badge-warning">
                                    Entregado Parcialmente
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Complete'){
                                %>
                                <span Class="badge badge-success">
                                    Entregado
                                </span>
                                <%
                                }
                                %>
                            </td>
                            <td id="p<%= producto.idDetalleSalesOrder %>"><%= producto.CantidadEntregada %></td>
                            <%
                            if(producto.EstatusEntrega == 'Merc_Complete'){
                            %>
                            <td></td>
                            <%
                            }else{
                            %>
                            <td><input class="form-control" type="number" id="l<%= producto.idDetalleSalesOrder %>" min='0' onblur='ValidaMercancia(<%= producto.idDetalleSalesOrder %>)'></td>
                            <%
                            }
                            %>
                        </tr>
                        <% }); %>
                    </tbody>
                </table>
            </div>
        </div>
        <%
        }else if(Estatus == 'Facturación pendiente'){
        %>
        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Descripcion</th>
                            <th scope="col">Mercancia Entregada</th>
                            <th scope="col">Lote</th>
                            <th scope="col">Estatus</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_detalle, function(producto){ %>
                        <tr id="Detalle-<%= producto.idDetalleSalesOrder %>">
                            <td nowrap><%= producto.DescripcionProducto %></td>
                            <td id="p<%= producto.idDetalleSalesOrder %>"><%= producto.CantidadEntregada %></td>
                            <td><%= producto.NumLote %></td>
                            <td>
                                <%
                                if(producto.EstatusEntrega == 'Merc_Creada'){
                                %>
                                <span class="badge badge-info">
                                    Pendiente de Entrega
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Parcial'){
                                %>
                                <span Class="badge badge-warning">
                                    Entregado Parcialmente
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Complete'){
                                %>
                                <span Class="badge badge-success">
                                    Entregado
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
        <%
        }else if(Estatus == 'Facturado'){
        %>
        <div class="single-table">
            <div class="table-responsive">
                <table class="table text-center">
                    <thead class="text-uppercase bg-dark">
                        <tr class="text-white">
                            <th scope="col">Descripcion</th>
                            <th scope="col">Mercancia Entregada</th>
                            <th scope="col">Lote</th>
                            <th scope="col">Estatus</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% _.each(l_detalle, function(producto){ %>
                        <tr id="Detalle-<%= producto.idDetalleSalesOrder %>">
                            <td nowrap><%= producto.DescripcionProducto %></td>
                            <td id="p<%= producto.idDetalleSalesOrder %>"><%= producto.CantidadEntregada %></td>
                            <td><%= producto.NumLote %></td>
                            <td>
                                <%
                                if(producto.EstatusEntrega == 'Merc_Creada'){
                                %>
                                <span class="badge badge-info">
                                    Pendiente de Entrega
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Parcial'){
                                %>
                                <span Class="badge badge-warning">
                                    Entregado Parcialmente
                                </span>
                                <%
                                }else if(producto.EstatusEntrega == 'Merc_Complete'){
                                %>
                                <span Class="badge badge-success">
                                    Entregado
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
        <%
        }
        %>
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
        <%
        if(Estatus == 'Ejecución de la orden pendiente'){
        %>
        <p><strong>Acciones.</strong></p>
        @*<button type="button" id="GenerarEjecucionMercancia" data-id="<%= idSalesOrder %>" class="btn btn-info">
                Generar Entrega de Mercancia Parcial
            </button>*@
        <button type="button" id="GenerarEjecucionMercanciaTotal" data-id="<%= NS_InternalID %>" class="btn btn-warning">
            Generar Entregar Total de Mercancia
        </button>
        <%
        }else if(Estatus == 'SO_FacPenEnt'){
        %>
        <p><strong>Acciones.</strong></p>
        @*<button type="button" id="GenerarEjecucionMercancia" data-id="<%= idSalesOrder %>" class="btn btn-info">
                Generar Entrega de Mercancia
            </button>*@
        <button type="button" id="GenerarEjecucionMercanciaTotal" data-id="<%= NS_InternalID %>" class="btn btn-warning">
            Generar Entregar Total de Mercancia
        </button>
        <%
        }else if(Estatus == 'Facturación pendiente'){
        %>
        <p><strong>Acciones.</strong></p>
        <button type="button" id="GenerarFacturaSO" data-id="<%= NS_InternalID %>" class="btn btn-warning">
            Generar Factura de Pedido
        </button>
        <%
        }
        %>
        <br />
        <br />
        <p><strong>Estatus.</strong></p>
        <%
        if(Estatus == 'Ejecución de la orden pendiente'){
        %>
        <p>Pendiente Entrega Mercancia.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-warning progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 60%"></div>
        </div>
        <%
        }else if(Estatus == 'Facturado'){
        %>
        <p>Facturado.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-success progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
        </div>
        <%
        }else if(Estatus == 'Facturación pendiente'){
        %>
        <p>Pendiente de Facturación.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-info progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 60%"></div>
        </div>
        <%
        }else if(Estatus == 'Cerrado'){
        %>
        <p>Orden de Venta Cancelada/Cerrada.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-warning progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
        </div>
        <%
        }
        %>

        @*<div style="display:none;">
            @Using Html.BeginForm("GenerarEjecucionMercancia", "OrdenesVenta", FormMethod.Post, New With {.id = "EjecucionMercancia"})
                @Html.AntiForgeryToken()
                @<input type="hidden" name="id" id="create-id" />
                @<div Class="col-lg-12" name="Entrega" id="EjecucionhiddenDivDetalles"></div>
            End Using
        </div>*@

        <div style="display:none;">
            @Using Html.BeginForm("CrearFacturaPorOrdenVentaVer2", "OrdenesVenta", FormMethod.Post, New With {.id = "CrearFactura"})
                @Html.AntiForgeryToken()
                @<input type="hidden" name="id" id="create-id" />
            End Using
        </div>

        <div style="display:none;">
            @Using Html.BeginForm("GenerarEjecucionTotalMercanciaVer2", "OrdenesVenta", FormMethod.Post, New With {.id = "EjecucionTotalMercancia"})
                @Html.AntiForgeryToken()
                @<input type="hidden" name="id" id="create-id" />
            End Using
        </div>

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
                                $('#' + id).find('td:eq(4)').html("<button type='button' onclick='ConsultarDatosFactura(" + data.Valor.NS_ID +")' class='btn btn-light btn-xs mb-3'>" + data.Valor.Factura +"</button>");
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

            $('#ImprimirFacturaPedido').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("ImprimirInvoice", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

            $('#GetPDF_Timbrado').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

            $('#GetXML_Timbrado').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

            //$('#GenerarEjecucionMercancia').click(function () {
            //    try {

            //        var id = $(this).attr("data-id");
            //        var form = $("#EjecucionMercancia");

            //        form.children("[name='id']").val(id);

            //        EjecucionPedido.length = 0;

            //        //Obtenemos la cantidad de stock a ejecutar
            //        _.each(Detalle, function (item) {
            //            var Cantidad = $('#l' + item.id).val();

            //            if (isNaN(Cantidad) || (Cantidad == "") || (Cantidad == "0")) {

            //            } else {
            //                EjecucionPedido.push({ "id": item.id, "Cantidad": Cantidad });
            //            }
            //        }, this);

            //        if (EjecucionPedido.length === 0) {
            //            toastr.error('No se puede hacer la ejecución de pedido, debido a que no se ha ingresado el stock a entregar...');
            //            return false;
            //        } else {
            //            $('#ModalEdicion').modal('toggle');
            //            showLoading("Generando Entrega de Mercancia...");

            //            var templateEjecucionPedido = _.template(document.getElementById("EjecucionListHidde").textContent);
            //            $("#EjecucionPedidoListHidden").html("");

            //            _.each(EjecucionPedido, function (item) {
            //                var dataItem = item
            //                var html = templateEjecucionPedido({ Entrega: dataItem, contador: contador })
            //                form.children("[name='Entrega']").append(html);
            //                contador++;
            //            }, this);

            //            var dataForm = form.serialize();
            //            toastr.info('Generando Entrega de Mercancia...');
            //            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
            //                console.log(data);
            //                if (data.Tipo == 1) {
            //                    hideLoading();
            //                    toastr.success('Se Realizó la entrega de mercancia con exito...');
            //                }
            //                else {
            //                    hideLoading();
            //                    toastr.error(data.Mensaje);
            //                }
            //            });
            //        }
            //    }
            //    catch (err) {
            //        hideLoading();
            //    }

            //});

            $('#GenerarEjecucionMercanciaTotal').click(function () {
                try {
                    if (confirm("Este proceso marcara toda la mercancia como entregada. ¿Desea confirmarlo?")) {
                        var id = $(this).attr("data-id");
                        var form = $("#EjecucionTotalMercancia");

                        form.children("[name='id']").val(id);

                        var dataForm = form.serialize();
                        $('#ModalEdicion').modal('toggle');
                        showLoading("Generando Entrega de Mercancia...");
                        $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                            console.log(data);
                            if (data.Tipo == 1) {
                                hideLoading();
                                toastr.success('Se Realizó la entrega de mercancia con exito...');

                                if (data.Valor.Estatus == "SO_Facturada") {
                                    $('#' + id).find('td:eq(5)').html("<span class='badge badge-success'>Factura Creada</span>");
                                    $('#' + id).find('td:eq(4)').html("<button type='button' onclick='ConsultarDatosFactura(" + data.Valor.NS_ID + ")' class='btn btn-light btn-xs mb-3'>" + data.Valor.Factura + "</button>");
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
                }
                catch (err) {
                    hideLoading();
                    toastr.error("Hubo un problema al procesar su solicitud... Intentelo más tarde.");
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

    <script type="text/template" id='DetalleFacturacionTemplate'>

        <p><strong>Datos de Facturación.</strong></p>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Serie:</label>
                    <input class="form-control" type="text" value="<%= Serie %>" readonly id="Show_Serie">
                </div>
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Folio:</label>
                    <input class="form-control" type="text" readonly value="<%= Folio %>" id="Show_Folio">
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <button type="button" id="DescargarPreFactPDF" data-id="<%= NS_InternalID %>" class="btn btn-info">
                        Descargar PreFactura PDF <i class="fa fa-file-pdf-o"></i>
                    </button>
                </div>
            </div>
        </div>
        <%
        if(Estatus == 'Timbrado'){
        %>
        <br />
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">Fecha de Timbrado:</label>
                    <input class="form-control" type="text" value="<%= FechaTimbrado %>" readonly id="Show_FechaTimbrado">
                </div>
                <div class="col-lg-6">
                    <label for="example-text-input" class="col-form-label">UUID:</label>
                    <input class="form-control" type="text" readonly value="<%= UUID %>" id="Show_UUID">
                </div>
            </div>
        </div>
        <br />
        <div class="form-group">
            <div class="row">
                <div class="col-lg-6">
                    <button type="button" id="DescargarXML" data-id="<%= NS_XML %>" class="btn btn-info">
                        Descargar XML <i class="fa fa-file-archive-o"></i>
                    </button>
                    <button type="button" id="DescargarPDF" data-id="<%= NS_PDF %>" class="btn btn-info">
                        Descargar PDF <i class="fa fa-file-pdf-o"></i>
                    </button>
                </div>
            </div>
        </div>
        <%
        }
        %>

        <br />
        <br />
        <p><strong>Estatus.</strong></p>
        <%
        if(Estatus == 'Timbrado'){
        %>
        <p>Documento Timbrado.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-info progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
        </div>
        <%
        }else{
        %>
        <p>Pendiente de Timbrado.</p>
        <div class="progress">
            <div class="progress-bar progress-bar-striped bg-success progress-bar-animated" role="progressbar" aria-valuenow="30" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
        </div>
        <%
        }
        %>

        <script>

            $('#DescargarPDF').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

            $('#DescargarPreFactPDF').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("ImprimirInvoice", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

            $('#DescargarXML').click(function () {
                var id = $(this).attr("data-id");
                var url = "@Url.Action("GETArchivo_Timbrado", "OrdenesVenta", New With {.id = "__ID__"})";
                var gets = url.replace("__ID__", id);

                toastr.info("Descargando Factura...");

                window.location.replace(gets);
            });

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

            var Url_Datos = "@Url.Action("GetOrdenVentaVer2", "OrdenesVenta", New With {.id = "__ID__"})"

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

        function ConsultarDatosFactura(id) {

            var Url_Datos = "@Url.Action("GetDatosFacturacion", "OrdenesVenta", New With {.id = "__ID__"})"

            var link = Url_Datos.replace("__ID__", id);

            toastr.info("Obteniendo Detalles de Facturación...");

            jQuery.ajax({
                url: link,
                cache: false,
                contentType: false,
                processData: false,
                type: 'POST',
                success: function (data) {
                    if (data.Tipo == 1) {

                        var template = _.template(document.getElementById("DetalleFacturacionTemplate").textContent);
                        $("#areaDetallesFacturacion").html("");
                        var html = template(data.Valor);
                        $("#areaDetallesFacturacion").html(html);
                        $('#ModalFacturacion').modal('toggle');
                    } else {

                    }
                }
            });
        }

        function CancelarOrdenVenta(id, folio) {
            try {
                if (confirm("¿Desea Cancelar esta transacción? El proceso generara la cancelación del pedido y devolvera la mercancia apartada.")) {
                    if (confirm("Confirmar Cancelación.")) {

                        var form = $("#CancelarOrdenVenta");

                        form.children("[name='NS_ID']").val(id);
                        form.children("[name='Folio']").val(folio);

                        var dataForm = form.serialize();
                        showLoading("Obteniendo Factura...");
                        $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                            console.log(data);
                            if (data.Tipo == 1) {
                                hideLoading();
                                toastr.success('Se cancelo el documento con exito...');
                                var Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + id + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVenta/" + id + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + id + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"
                                /*var Estatus = "<span class='badge badge-danger'>Cancelado</span >"*/
                                $('#' + id).find('td:eq(5)').html(Estatus);
                                $('#' + id).find('td:eq(6)').html(Details);

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
        function EditarOrdenVenta(id) {
            var form = $("#EditarOrdenes");

            form.children("[name='NS_ID']").val(id);

            var dataForm = form.serialize();

            toastr.info("Buscando Orden de Venta...");

        }

        function ConsultarOrdenesVentas() {

            var form = $("#ConsultarOrdenes");

            var FechaIni = $('#FechaIni').val();
            var FechaFin = $('#FechaFin').val();
            var NoPedido = $('#NoPedido').val();
            //var NumLote = $('#NumeroLote').val();

            form.children("[name='FechaInicio']").val(FechaIni);
            form.children("[name='FechaFin']").val(FechaFin);
            form.children("[name='NoPedido']").val(NoPedido);
            //form.children("[name='NumLote']").val(NumLote);

            var dataForm = form.serialize();

            toastr.info("Generando Consulta...");

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                if (data.Tipo == 1) {

                    tablaDetalle.clear().draw();

                    $.each(data.Valor, function (index, value) {

                        var Nombre = value.ClaveCliente + " - " + value.NombreCliente
                        var Estatus = ""
                        var Details = ""
                        var Invoices = ""

                        if ((value.Estatus == "pendingFulfillment") && (value.Factura = "Sin Factura")) {
                            var sales = value.NS_InternalID + ', ' + value.NS_ExternalID
                            Estatus = "<span class='badge badge-info'>Orden Creada</span>"
                            //Details = "<ul class='d-flex justify-content-center'><li onclick='EditarOrdenVenta(" + value.NS_InternalID + ")' class='mr-3' title='Editar Orden de Venta'><a href='#' class='text-secondary'><i class='fa fa-pencil'></i></a></li><li onclick='CancelarOrdenVenta(" + value.NS_InternalID + ', ' + "\"" + value.NS_ExternalID + "\")' class='mr-3' title='Cancelar Orden de Venta'><a href='#' class='text-secondary'><i class='fa fa-ban'></i></a></li><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"
                            Details = "<ul class='d-flex justify-content-center'><li onclick='CancelarOrdenVenta(" + value.NS_InternalID + ', ' + "\"" + value.NS_ExternalID + "\")' class='mr-3' title='Cancelar Orden de Venta'><a href='#' class='text-secondary'><i class='fa fa-ban'></i></a></li><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"

                        } else if (value.Estatus == "pendingBilling") {
                            Estatus = "<span class='badge badge-warning'>Mercancia Entregada</span>"
                            Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"

                        } else if (value.Estatus == "fullyBilled") {
                            Estatus = "<span class='badge badge-success'>Factura Creada</span>"
                            Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"

                        } else if ((value.Estatus == "pendingFulfillment") && (value.Factura != "Sin Factura")) {
                            Estatus = "<span class='badge badge-primary'>Factura Creada/Pendiente Entrega</span>"
                            Details = "<ul class='d-flex justify-content-center'><li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"

                        } else if (value.Estatus == "closed") {
                            Estatus = "<span class='badge badge-danger'>Cancelado</span >"
                            Details = "<ul class='d-flex justify-content-center'> <li onclick='DetallesPedido(" + value.NS_InternalID + ")' class='mr-3' title='Consultar Detalles'><a href='#' class='text-secondary'><i class='fa fa-search'></i></a></li><li onclick='PrintPedido()' class='mr-3' title='Imprimir Pedido'><a href='/OrdenesVenta/ImprimirOrdenVentaVer2/" + value.NS_InternalID + "' class='text-secondary'><i class='fa fa-print'></i></a></li><li class='mr-3' title='Consultar En Netsuite (requiere usuario)'><a href='" + URL_DEC + value.NS_InternalID + "&amp;whence=' class='text-secondary' target='_blank'><i class='fa fa-share'></i></a></li></ul>"
                        }

                        if (value.Factura == "Sin Factura") {
                            Invoices = value.Factura
                        } else {
                            Invoices = "<button type='button' onclick='ConsultarDatosFactura(" + value.NS_InternalID_Factura + ")' class='btn btn-light btn-xs mb-3'>" + value.Factura + "</button>"
                        }

                        tablaDetalle.row.add([
                            Nombre,
                            value.NS_ExternalID,
                            value.Fecha,
                            value.Mostrador,
                            Invoices,
                            Estatus,
                            Details,
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
    </script>
End Section
