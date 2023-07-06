
@Code
    ViewData("Title") = "Consultar Inventario Disponible"
    ViewData("I_CID") = "class=active"
    ViewData("SubPage") = "Inventario"
End Code

<div class="col-lg-12 mt-5">
    <div class="card">
        <div class="card-body">
            <header><h4 class="header-title">@ViewData("Title")</h4></header>

            <div class="single-table">
                <div class="table-responsive">
                    <table id="TableInventario" class="table table-hover progress-table text-center">
                        <thead class="text-uppercase">
                            <tr>
                                <th scope="col">Clave Producto</th>
                                <th scope="col">Descripción</th>
                                <th scope="col">Stock Disponible Mostrador</th>
                                <th scope="col">Stock Disponible CEDIS</th>
                                <th scope="col">Stock Disponible MUELLES</th>
                                <th scope="col">Precio Base</th>
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.Stock
                                Dim currentItem As StockArticulosViewModel = item
                                @<tr>
                                    <td scope="row">@currentItem.ClaveProducto</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Descripcion)</td>
                                    <td scope="row">@Convert.ToDecimal(currentItem.stock_Mostrador).ToString("###")</td>
                                    <td scope="row">@Convert.ToDecimal(currentItem.stock_Cedis).ToString("###")</td>
                                    <td scope="row">@Convert.ToDecimal(currentItem.stock_Muelles).ToString("###")</td>
                                    <td scope="row">@Convert.ToDecimal(currentItem.precioBase).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
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
    @Using Html.BeginForm("SincronizarInventarioDisponible", "Inventario", FormMethod.Post, New With {.id = "SincronizarInventario"})
        @Html.AntiForgeryToken()
    End Using
</div>

@Section Scripts

    <script>
        var tablaDetalle = null;

        $(document).ready(function () {


            tablaDetalle = $("#TableInventario").DataTable({
                dom: 'Bfrtip',
                "info": false,
                responsive: false,
                "ordering": false,
                paging: false,
                buttons: [{
                    text: '<i class="fa fa-file-excel-o"></i>',
                    extend: 'excelHtml5',
                    title: 'Inventario Disponible',
                    className: 'btn btn-flat btn-info mb-3'
                }]
                //},
                //    {
                //        text: '<i class="fa fa-repeat"></i>',
                //        className: 'btn btn-flat btn-primary mb-3',
                //        action: function (e, dt, node, config) {
                //            SincronizarInventarios();
                //        }
                //    }]
            });
            changeExcelButton();
        });

        function changeExcelButton() {
            //Modificar boton de Exportar a Excel
            $("div.dt-buttons").appendTo("#TableInventario_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TableOrdenesVenta_filter").css("margin-left", "10px");
        }

        function SincronizarInventarios() {
            if (confirm("¿Desea sincronizar el stock disponible? esta acción puede demorar en procesar.")) {

                try {
                    var form = $("#SincronizarInventario");

                    showLoading("Sincronizando Stock Con Netsuite");

                    form.submit()
                } catch (error) {
                    hideLoading();
                }



            }
        }

    </script>
End Section
