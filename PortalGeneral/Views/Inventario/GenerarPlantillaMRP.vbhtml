
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
                                <th scope="col">Stock Disponible Planta</th>
                                <th scope="col">Stock Disponi</th>
                            </tr>
                        </thead>
                        <tbody>
                            @For Each item In ViewBag.Stock
                                Dim currentItem As StockDisponibleViewModel = item
                                @<tr>
                                    <td scope="row">@currentItem.NS_Externalid</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.NombreArticulo)</td>
                                    <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.StockDisponible)</td>
                                    @If currentItem.StockDisponible = "0" Then
                                        @<td scope="row"><span Class="badge badge-warning"> Sin Stock Disponible</span></td>
                                    Else
                                        @<td scope="row"><span Class="badge badge-success"> Disponible </span></td>
                                    End If
                                </tr>
                            Next
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
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
            });
            changeExcelButton();
        });

        function changeExcelButton() {
            //Modificar boton de Exportar a Excel
            $("div.dt-buttons").appendTo("#TableInventario_wrapper div.row div.col-sm-4");
            $("div.dt-buttons").css("float", "left");
            $("#TableOrdenesVenta_filter").css("margin-left", "10px");
        }

    </script>
End Section
