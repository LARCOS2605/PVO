@modeltype PortalGeneral.EstimacionesViewModel
@Code
    ViewData("Title") = Idiomas.My.Resources.Resource.Title_Estimaciones
    ViewData("SubPage") = Idiomas.My.Resources.Resource.SubPage_Estimacion
    ViewData("I_Est") = "class = active"

    'Dim RecuperarCarrito = Session("Carrito")
End Code

<style>
    * {
        margin: 0;
        padding: 0;
    }

    label[for="Customer"]:before {
        content: "*";
        color: red;
    }

    label[for="MetodoPagoSAT"]:before {
        content: "*";
        color: red;
    }

    label[for="FormaPagoSAT"]:before {
        content: "*";
        color: red;
    }

    label[for="UsoCFDI"]:before {
        content: "*";
        color: red;
    }

    .autocomplete {
        /*the container must be positioned relative:*/
        position: relative;
        display: inline-block;
    }

    .autocomplete-items {
        position: absolute;
        border: 1px solid #d4d4d4;
        border-bottom: none;
        border-top: none;
        z-index: 99;
        /*position the autocomplete items to be the same width as the container:*/
        top: 100%;
        left: 0;
        right: 0;
    }

        .autocomplete-items div {
            padding: 10px;
            cursor: pointer;
            background-color: #fff;
            border-bottom: 1px solid #d4d4d4;
        }

            .autocomplete-items div:hover {
                /*when hovering an item:*/
                background-color: #e9e9e9;
            }

    .autocomplete-active {
        /*when navigating through the items using the arrow keys:*/
        background-color: DodgerBlue !important;
        color: #ffffff;
    }

    /*form styles*/
    #msform {
        text-align: center;
        position: relative;
        margin-top: 20px;
    }

        #msform fieldset .form-card {
            background: white;
            border: 0 none;
            border-radius: 0px;
            box-shadow: 0 2px 2px 2px rgba(0, 0, 0, 0.2);
            padding: 20px 40px 30px 40px;
            box-sizing: border-box;
            width: 94%;
            margin: 0 3% 20px 3%;
            /*stacking fieldsets above each other*/
            position: relative;
        }

        #msform fieldset {
            background: white;
            border: 0 none;
            border-radius: 0.5rem;
            box-sizing: border-box;
            width: 100%;
            margin: 0;
            padding-bottom: 20px;
            /*stacking fieldsets above each other*/
            position: relative;
        }

            /*Hide all except first fieldset*/
            #msform fieldset:not(:first-of-type) {
                display: none;
            }

            #msform fieldset .form-card {
                text-align: left;
                color: #9E9E9E;
            }


        /*Blue Buttons*/
        #msform .action-button {
            width: 100px;
            background: skyblue;
            font-weight: bold;
            color: white;
            border: 0 none;
            border-radius: 0px;
            cursor: pointer;
            padding: 10px 5px;
            margin: 10px 5px;
        }

            #msform .action-button:hover, #msform .action-button:focus {
                box-shadow: 0 0 0 2px white, 0 0 0 3px skyblue;
            }

        /*Previous Buttons*/
        #msform .action-button-previous {
            width: 100px;
            background: #616161;
            font-weight: bold;
            color: white;
            border: 0 none;
            border-radius: 0px;
            cursor: pointer;
            padding: 10px 5px;
            margin: 10px 5px;
        }

            #msform .action-button-previous:hover, #msform .action-button-previous:focus {
                box-shadow: 0 0 0 2px white, 0 0 0 3px #616161;
            }

    /*Dropdown List Exp Date*/
    select.list-dt {
        border: none;
        outline: 0;
        border-bottom: 1px solid #ccc;
        padding: 2px 5px 3px 5px;
        margin: 2px;
    }

        select.list-dt:focus {
            border-bottom: 2px solid skyblue;
        }

    /*The background card*/
    .card {
        z-index: 0;
        border: none;
        border-radius: 0.5rem;
        position: relative;
    }

    /*FieldSet headings*/
    .fs-title {
        font-size: 25px;
        color: #2C3E50;
        margin-bottom: 10px;
        font-weight: bold;
        text-align: left;
    }

    /*progressbar*/
    #progressbar {
        margin-bottom: 30px;
        overflow: hidden;
        color: lightgrey;
    }

        #progressbar .active {
            color: #000000;
        }

        #progressbar li {
            list-style-type: none;
            font-size: 12px;
            width: 50%;
            float: left;
            position: relative;
        }

        /*Icons in the ProgressBar*/
        #progressbar #account:before {
            font-family: FontAwesome;
            content: "\f0c0";
        }

        #progressbar #personal:before {
            font-family: FontAwesome;
            content: "\f02e";
        }

        #progressbar #payment:before {
            font-family: FontAwesome;
            content: "\f07a";
        }

        #progressbar #confirm:before {
            font-family: FontAwesome;
            content: "\f00c";
        }

        /*ProgressBar before any progress*/
        #progressbar li:before {
            width: 50px;
            height: 50px;
            line-height: 45px;
            display: block;
            font-size: 18px;
            color: #ffffff;
            background: lightgray;
            border-radius: 50%;
            margin: 0 auto 10px auto;
            padding: 2px;
        }

        /*ProgressBar connectors*/
        #progressbar li:after {
            content: '';
            width: 100%;
            height: 2px;
            background: lightgray;
            position: absolute;
            left: 0;
            top: 25px;
            z-index: -1;
        }

        /*Color number of the step and the connector before it*/
        #progressbar li.active:before, #progressbar li.active:after {
            background: skyblue;
        }

    /*Imaged Radio Buttons*/
    .radio-group {
        position: relative;
        margin-bottom: 25px;
    }

    .radio {
        display: inline-block;
        width: 204;
        height: 104;
        border-radius: 0;
        background: lightblue;
        box-shadow: 0 2px 2px 2px rgba(0, 0, 0, 0.2);
        box-sizing: border-box;
        cursor: pointer;
        margin: 8px 2px;
    }

        .radio:hover {
            box-shadow: 2px 2px 2px 2px rgba(0, 0, 0, 0.3);
        }

        .radio.selected {
            box-shadow: 1px 1px 2px 2px rgba(0, 0, 0, 0.1);
        }

    /*Fit image in bootstrap div*/
    .fit-image {
        width: 100%;
        object-fit: cover;
    }
</style>

<div class="card">
    <div class="card-body">
        <h4 class="header-title">Generar Estimación</h4>
        <div class="col-lg-12 mt-5">
            @Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "msform"})
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True)



                @<ul id="progressbar">
                    @*<li class="active" id="account"><strong>Información General</strong></li>*@
                    <li class="active" id="payment"><strong>@Idiomas.My.Resources.Resource.WiS_Articulos</strong></li>
                    <li id="confirm"><strong>@Idiomas.My.Resources.Resource.WiS_ConfirmPedido</strong></li>
                </ul>
                @<fieldset>
                    <input style="display:none" type="text" class="form-control" id="Token">
                    <div class="card" style="text-align: left;">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">
                                    @Html.LabelFor(Function(model) model.Customer, Idiomas.My.Resources.Resource.lbl_Clientes, New With {.class = "col-form-label"})
                                    @Html.DropDownList("Customer", Nothing, Idiomas.My.Resources.Resource.cbb_Clientes, New With {.class = "form-control "})
                                    @Html.ValidationMessageFor(Function(model) model.Customer)
                                </div>
                            </div>
                        </div>
                        <br />
                        <h4 class="header-title">@Idiomas.My.Resources.Resource.WiS_Articulos</h4>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">
                                    <label class="col-form-label">@Idiomas.My.Resources.Resource.lbl_BusquedaArticulo</label>
                                    <div class="input-group-append">
                                        <input class="form-control" placeholder="@Idiomas.My.Resources.Resource.ph_ClaveProducto" type="text" id="ClaveProducto">
                                        <input class="form-control" placeholder="@Idiomas.My.Resources.Resource.ph_NombreProducto" type="text" id="NombreProducto">
                                        <button type="button" onclick="FindProducto()" class="btn btn-success" data-toggle="tooltip">@Idiomas.My.Resources.Resource.btn_BuscarProducto</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_CodigoProducto</label>
                                    <input type="text" id="Clave" readonly class="form-control">
                                </div>
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_NoLote</label>
                                    <input type="text" id="NumeroLote" readonly class="form-control">
                                </div>
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">Existencia Mostrador:</label>
                                    <input type="text" id="StockDisponible" readonly class="form-control">
                                </div>
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">Existencia Planta:</label>
                                    <input type="text" id="StockDisponiblePlanta" readonly class="form-control">
                                </div>
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_Precio</label>
                                    <input type="text" id="PrecioProducto" readonly class="form-control">
                                </div>
                                <div class="col-lg-2">
                                    <label for="disabledTextInput">Precio Lista:</label>
                                    <input type="text" id="PrecioProductoLista" readonly class="form-control">
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-6">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_Descripcion</label>
                                    <input type="text" id="Descripcion" readonly class="form-control">
                                </div>
                                <div class="col-lg-3">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_DescuentoAsignado</label>
                                    <input type="text" id="TipoDescuento" readonly class="form-control">
                                </div>
                                <div class="col-lg-3">
                                    <label for="disabledTextInput">@Idiomas.My.Resources.Resource.lbl_Cantidad</label>
                                    <input type="number" id="Cantidad" class="form-control">
                                </div>
                            </div>
                        </div>


                        <input style="display:none" type="text" class="form-control" id="IssueInventory">
                        <input style="display:none" type="text" class="form-control" id="ID_NS">
                        <input style="display:none" type="text" class="form-control" id="idPr">
                        <input style="display:none" type="text" class="form-control" id="Prex">
                        <input style="display:none" type="text" class="form-control" id="TioMat">

                        <div class="form-group">
                            <button type="button" onclick="AddProducto()" class="btn btn-primary mb-3">@Idiomas.My.Resources.Resource.btn_AgregarProducto</button>
                        </div>

                        <h4 class="header-title">Cotización.</h4>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">

                                    <div class="single-table">
                                        <div class="table-responsive">
                                            <table id="TableMaterialInventario" class="table text-center">
                                                <thead class="text-uppercase bg-primary">
                                                    <tr class="text-white">
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Codigo</th>
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Descripcion</th>
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Lote</th>
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Precio</th>
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Cantidad</th>
                                                        <th scope="col">@Idiomas.My.Resources.Resource.lblt_Subtotal</th>
                                                        <th scope="col"><i class="fa fa-gears"></i></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    @* En caso de que el pedido marque error, no se perdera por completo *@
                                                    @*@If Not IsNothing(RecuperarCarrito) Then
                                                    For Each item In RecuperarCarrito
                                                        Dim currentItem As CarritoViewModel = item
                                                        @<tr>
                                                            <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.ClaveProducto)</td>
                                                            <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Descripcion)</td>
                                                            <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.NumLote)</td>
                                                            <td scope="row">@Convert.ToDecimal(currentItem.PrecioU).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                                            <td scope="row">@Html.DisplayFor(Function(modelItem) currentItem.Cantidad)</td>
                                                            <td scope="row">@Convert.ToDecimal(currentItem.Total).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                                            <td scope="row"><ul class='d-flex justify-content-center'><li onclick='EliminarArticulo(@currentItem.Vlender)' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-trash'></i></a></li></ul></td>
                                                        </tr>
                                                    Next
                                                End If*@
                                                </tbody>
                                                <tfoot>
                                                    <tr style="text-align:right">
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td><strong>SUBTOTAL :     </strong></td>
                                                        <td id="DataPSubtotal"></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr style="text-align:right">
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td><strong>IVA :     </strong></td>
                                                        <td id="DataPTotalIVA"></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr style="text-align:right">
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td><strong>@Idiomas.My.Resources.Resource.lbl_Total</strong></td>
                                                        <td id="DataPTotal"></td>
                                                        <td></td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">
                                    @Html.LabelFor(Function(model) model.Memo, "Nota:", New With {.class = "col-form-label"})
                                    @Html.TextBoxFor(Function(model) model.Memo, New With {.class = "form-control"})
                                    @Html.ValidationMessageFor(Function(model) model.Memo)
                                </div>
                            </div>
                        </div>
                    </div>
                    @*<button type="button" name="previous" class="previous btn btn-secondary mb-3">Anterior</button>*@
                    <button type="button" class="btn btn-flat btn-primary mb-3 next">Siguiente</button>
                </fieldset>
                @<fieldset>
                    <div class="card" style="text-align: left;">
                        <h4 class="header-title">Información Facturación.</h4>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @Html.LabelFor(Function(model) model.MetodoPagoSAT, "Método de Pago:", New With {.class = "col-form-label"})
                                                @Html.DropDownList("MetodoPagoSAT", Nothing, "--- Seleccione un Metodo de Pago ---", New With {.class = "form-control"})
                                                @Html.ValidationMessageFor(Function(model) model.MetodoPagoSAT)
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @Html.LabelFor(Function(model) model.FormaPagoSAT, "Forma de Pago:", New With {.class = "col-form-label"})
                                                @Html.DropDownList("FormaPagoSAT", Nothing, "--- Seleccione una Forma de Pago ---", New With {.class = "form-control"})
                                                @Html.ValidationMessageFor(Function(model) model.FormaPagoSAT)
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @Html.LabelFor(Function(model) model.UsoCFDI, "Uso de CFDI:", New With {.class = "col-form-label"})
                                                @Html.DropDownList("UsoCFDI", Nothing, "--- Seleccione un Uso de CFDI ---", New With {.class = "form-control"})
                                                @Html.ValidationMessageFor(Function(model) model.UsoCFDI)
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                    </div>
                    <button type="button" name="previous" class="previous btn btn-secondary mb-3">Anterior</button>
                    <button type="button" onclick="CrearOrdenVenta()" Class="btn btn-flat btn-primary mb-3">Almacenar Estimación</button>
                    @*<button type="button" onclick="CrearFacturaVenta()" Class="btn btn-flat btn-warning mb-3">Crear Factura Pedido con entrega pendiente </button>*@
                    @*<button type="button" onclick="EntregarVentaFacturar()" Class="btn btn-flat btn-success mb-3"> Entregar y Facturar Pedido</button>*@
                </fieldset>
                @<input type="hidden" name="GenerarFacturaDirecta" id="GenerarFacturaDirecta" />
                @<input type="hidden" name="GenerarVentaDirecta" id="GenerarVentaDirecta" />
                @<div Class="col-lg-12" id="ProductoshiddenDivDetalles">
                </div>
            End Using
        </div>
    </div>
</div>

<script type="text/template" id='ProductoListHidde'>
    @Html.Partial("ProductoListHidden")
</script>

<div style="display:none;">
    @Using Html.BeginForm("CrearListaProductos", "Estimaciones", FormMethod.Post, New With {.id = "AgregarProducto"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="ClaveProducto" id="create-ClaveProducto" />
        @<input type="hidden" name="Descripcion" id="create-Descripcion" />
        @<input type="hidden" name="Cantidad" id="create-Cantidad" />
        @<input type="hidden" name="NumLote" id="create-NumLote" />
        @<input type="hidden" name="IssueInventory" id="create-IssueInventory" />
        @<input type="hidden" name="ID_NS" id="create-ID_NS" />
        @<input type="hidden" name="idPr" id="create-idPr" />
        @<input type="hidden" name="PrecioU" id="create-PrecioU" />
        @<input type="hidden" name="Vlender" id="create-Vlender" />
        @<input type="hidden" name="TipoMaterial" id="create-TipoMaterial" />
        @<input type="hidden" name="Token" id="create-Token" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("ConsultarInventario", "Estimaciones", FormMethod.Post, New With {.id = "ConsultarProducto"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="id" id="create-id" />
        @<input type="hidden" name="NombreProducto" id="create-NombreProducto" />
        @<input type="hidden" name="Customer" id="create-Customer" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("EliminarArticuloCarrito", "Estimaciones", FormMethod.Post, New With {.id = "EliminarProducto"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="Vlender" id="create-vlender" />
        @<input type="hidden" name="token" id="create-token" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("VaciarCarrito", "Estimaciones", FormMethod.Post, New With {.id = "VaciarCarrito"})
        @Html.AntiForgeryToken()
        @<input type="hidden" name="token" id="create-token" />
    End Using
</div>

@Section Scripts
    @Scripts.Render("~/bundles/Carrito")

<script type="text/javascript">

    var tablaMercancia = null;
    var Carrito = [];
    var contador = 0;
    var Token = "@Html.Raw(ViewBag.tokenGenCarrito)";
    $('#Token').val("");
    $('#Token').val(Token);

    $('#GenerarFacturaDirecta').val(false);
    $('#GenerarVentaDirecta').val(false);

    window.onpageshow = function (event) {
        if (event.persisted) {
            window.location.reload()
        }
    };

    $('#Customer').change(function (n) {
            $('#Descripcion').val("");
            $('#StockDisponible').val("");
            $('#Cantidad').val("");
            $('#Clave').val("");
            $('#NumeroLote').val("");
            $('#IssueInventory').val("");
            $('#PrecioProducto').val("");
            $('#TipoDescuento').val("");
            $('#ID_NS').val("");
            $('#idPr').val("");
            $('#Prex').val("");
            $('#TioMat').val("");


            Carrito.length = 0;
            Vlender = 0;
            tablaMercancia.clear().draw();

            var Token = $('#Token').val();
            var form = $("#VaciarCarrito");
            form.children("[name='token']").val(Token);
            var dataForm = form.serialize();
            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) { });
            CalcularDetalle();
        });

    $(document).ready(function () {

            // Initialize select2
            $("#Customer").select2();

            $("#NombreProducto").keyup(function (e) {

                var descripcion = $(this).val();

                var url = "@Url.Action("GetArticulo", "Estimaciones", New With {.id = "__ID__"})"

                var url_RES = url.replace("__ID__", descripcion)
                $.ajax({
                    type: "POST",
                    url: url_RES,
                    data: $(this).val(),
                    beforeSend: function () {
                        $("#NombreProducto").css("background", "#FFF url(/Assets/images/LoaderIcon.gif) no-repeat 165px");
                    },
                    success: function (data) {
                        $("#NombreProducto").css("background", "#FFF");
                        var arr = [];

                        $.each(data.Valor, function (index, value) {
                            arr.push(value.Descripcion);
                        });

                        autocomplete(document.getElementById("NombreProducto"),arr);
                    }
                });
            });

            /*CalcularDetalle();*/

            @*var statesAvailable = @Html.Raw(RecuperarCarrito);*@
            //console.log(statesAvailable);

            // Read selected option
            //$('#Customer').click(function () {
            //    var username = $('#selUser option:selected').text();
            //    var userid = $('#selUser').val();

            //    $('#result').html("id : " + userid + ", name : " + username);

            //});

            /*showLoading("Pruebas");*/
            tablaMercancia = $("#TableMaterialInventario").DataTable({
                "bPaginate": false, "ordering": false, "searching": false, "bInfo": false, "language": {
                    "emptyTable": "No hay Inventario disponible."
                }
            });
            var current_fs, next_fs, previous_fs; //fieldsets
            var opacity;

            //$('#smartcart').smartCart();

            $(".next").click(function () {

                current_fs = $(this).parent();
                next_fs = $(this).parent().next();

                //Add Class Active
                $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

                //show the next fieldset
                next_fs.show();
                //hide the current fieldset with style
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
                        // for making fielset appear animation
                        opacity = 1 - now;

                        current_fs.css({
                            'display': 'none',
                            'position': 'relative'
                        });
                        next_fs.css({ 'opacity': opacity });
                    },
                    duration: 600
                });
            });

            $(".previous").click(function () {

                current_fs = $(this).parent();
                previous_fs = $(this).parent().prev();

                //Remove class active
                $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

                //show the previous fieldset
                previous_fs.show();

                //hide the current fieldset with style
                current_fs.animate({ opacity: 0 }, {
                    step: function (now) {
                        // for making fielset appear animation
                        opacity = 1 - now;

                        current_fs.css({
                            'display': 'none',
                            'position': 'relative'
                        });
                        previous_fs.css({ 'opacity': opacity });
                    },
                    duration: 600
                });
            });

            $('.radio-group .radio').click(function () {
                $(this).parent().find('.radio').removeClass('selected');
                $(this).addClass('selected');
            });

            $(".submit").click(function () {
                return false;
            })

    });

    //$(document).load(function () {
    //    $('#Token').val("");
    //    $('#Token').val(Token);

    //    $('#Descripcion').val("");
    //    $('#StockDisponible').val("");
    //    $('#Cantidad').val("");
    //    $('#Clave').val("");
    //    $('#NumeroLote').val("");
    //    $('#IssueInventory').val("");
    //    $('#PrecioProducto').val("");
    //    $('#TipoDescuento').val("");
    //    $('#ID_NS').val("");
    //    $('#idPr').val("");
    //    $('#Prex').val("");
    //    $('#TioMat').val("");
    //    $('#PrecioProductoLista').val("");
    //    $('#StockDisponiblePlanta').val("");
    //    $('#Customer').val('');
    //    var s = $('#Customer').val();
    //});

    function autocomplete(inp, arr) {
        var currentFocus;
        inp.addEventListener("input", function (e) {
            var a, b, i, val = this.value;
            closeAllLists();
            if (!val) { return false; }
            currentFocus = -1;
            a = document.createElement("DIV");
            a.setAttribute("id", this.id + "autocomplete-list");
            a.setAttribute("class", "autocomplete-items");
            this.parentNode.appendChild(a);
            for (i = 0; i < arr.length; i++) {
              if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                    b = document.createElement("DIV");
                    b.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
                    b.innerHTML += arr[i].substr(val.length);
                    b.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
                    b.addEventListener("click", function (e) {
                        inp.value = this.getElementsByTagName("input")[0].value;
                        closeAllLists();
                    });
                    a.appendChild(b);
                }
            }
        });
        inp.addEventListener("keydown", function (e) {
            var x = document.getElementById(this.id + "autocomplete-list");
            if (x) x = x.getElementsByTagName("div");
            if (e.keyCode == 40) {
                /*If the arrow DOWN key is pressed,
                increase the currentFocus variable:*/
                currentFocus++;
                /*and and make the current item more visible:*/
                addActive(x);
            } else if (e.keyCode == 38) { //up
                /*If the arrow UP key is pressed,
                decrease the currentFocus variable:*/
                currentFocus--;
                /*and and make the current item more visible:*/
                addActive(x);
            } else if (e.keyCode == 13) {
                /*If the ENTER key is pressed, prevent the form from being submitted,*/
                e.preventDefault();
                if (currentFocus > -1) {
                    /*and simulate a click on the "active" item:*/
                    if (x) x[currentFocus].click();
                }
            }
        });
        function addActive(x) {
            /*a function to classify an item as "active":*/
            if (!x) return false;
            /*start by removing the "active" class on all items:*/
            removeActive(x);
            if (currentFocus >= x.length) currentFocus = 0;
            if (currentFocus < 0) currentFocus = (x.length - 1);
            /*add class "autocomplete-active":*/
            x[currentFocus].classList.add("autocomplete-active");
        }
        function removeActive(x) {
            /*a function to remove the "active" class from all autocomplete items:*/
            for (var i = 0; i < x.length; i++) {
                x[i].classList.remove("autocomplete-active");
            }
        }
        function closeAllLists(elmnt) {
            /*close all autocomplete lists in the document,
            except the one passed as an argument:*/
            var x = document.getElementsByClassName("autocomplete-items");
            for (var i = 0; i < x.length; i++) {
                if (elmnt != x[i] && elmnt != inp) {
                    x[i].parentNode.removeChild(x[i]);
                }
            }
        }
    }


        $('#ClaveProducto').bind("enterKey", function (e) {
            FindProducto();
        });
        $('#ClaveProducto').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).trigger("enterKey");
            }
        });


        $('#NombreProducto').bind("enterKey", function (e) {
            FindProducto();
        });
        $('#NombreProducto').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).trigger("enterKey");
            }
        });

        function FindProducto() {

            var Customer = $('#Customer').val();

            if (Customer == "") {
                toastr.error("Para buscar el producto, es necesario seleccionar un cliente");
                return false;
            }

            if ($('#ClaveProducto').val() == "" && ($('#NombreProducto').val() == "")) {

                $('#Descripcion').val("");
                $('#StockDisponible').val("");
                $('#Cantidad').val("");
                $('#Clave').val("");
                $('#NumeroLote').val("");
                $('#IssueInventory').val("");
                $('#PrecioProducto').val("");
                $('#TipoDescuento').val("");
                $('#ID_NS').val("");
                $('#idPr').val("");
                $('#Prex').val("");
                $('#TioMat').val("");
                $('#PrecioProductoLista').val("");
                $('#StockDisponiblePlanta').val("");

            } else {
                toastr.info('Obteniendo Inventario...');
                var form = $("#ConsultarProducto");

                $('#Descripcion').val("");
                $('#StockDisponible').val("");
                $('#Cantidad').val("");
                $('#Clave').val("");
                $('#NumeroLote').val("");
                $('#IssueInventory').val("");
                $('#PrecioProducto').val("");
                $('#TipoDescuento').val("");
                $('#ID_NS').val("");
                $('#idPr').val("");
                $('#Prex').val("");
                $('#TioMat').val("");
                $('#PrecioProductoLista').val("");
                $('#StockDisponiblePlanta').val("");

                var id = $('#ClaveProducto').val();
                var nombreProd = $('#NombreProducto').val();
                var Customer = $('#Customer').val();

                form.children("[name='Customer']").val(Customer);
                form.children("[name='id']").val(id);

                if (nombreProd == "") {
                    form.children("[name='NombreProducto']").val("");
                } else {
                    form.children("[name='NombreProducto']").val(nombreProd);
                }

                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {

                        if (data.Valor.length != 0) {
                            $.each(data.Valor, function (index, value) {

                                if (value.Precio != 0) {
                                    $('#Descripcion').val(value.Producto);
                                    if (value.TipoInventario != "Service") {
                                        $('#StockDisponible').val(value.StockDisponible1);
                                    } else {
                                        $('#StockDisponible').val("");
                                    }
                                    $('#Clave').val(value.Clave);
                                    $('#NumeroLote').val(value.NumLote);
                                    $('#IssueInventory').val(value.IssueInventory);
                                    $('#PrecioProducto').val(accounting.formatMoney(value.Precio));
                                    $('#TipoDescuento').val(value.Descuento);
                                    $('#ID_NS').val(value.ID_NS);
                                    $('#idPr').val(value.idProducto);
                                    $('#Prex').val(value.Precio);
                                    $('#TioMat').val(value.TipoInventario);
                                    $('#StockDisponiblePlanta').val(value.StockPlanta);
                                    $('#PrecioProductoLista').val(accounting.formatMoney(value.PrecioLista));
                                    toastr.info('Articulo localizado...');
                                } else {
                                    toastr.warning('El articulo no tiene un precio asignado...');
                                    return false;
                                }
                            });
                        } else {
                            toastr.error('Articulo no localizado...');
                        }


                    }
                    else {
                        toastr.error('No se pudo obtener el articulo, revise los parametros de busqueda...');
                    }
                });
            }
        }

        function AddProducto() {

            //Validacion Acciones
            //if ($('#ClaveProducto').val() == "") || ($('#Descripcion').val() == "") {
            //    toastr.error('La Clave de Producto Ingresada es Invalida...');
            //    return false
            //}
            if ($('#Cantidad').val() == "") {
                toastr.error('La Cantidad Ingresada es Invalida...');
                return false
            }

            if ($('#TioMat').val() != "Service") {
                if ($('#Clave').val() == "" || $('#Descripcion').val() == "" || $('#StockDisponible').val() == "") {
                    toastr.error('No se ha podido localizar un producto valido...');
                    return false
                }

                //if ($('#StockDisponible').val() == "0") {
                //    toastr.error('No se puede agregar este articulo debido a que no tiene inventario disponible...');
                //    return false
                //}

                if (($('#Cantidad').val() == "0") || ($('#Cantidad').val() < "0")) {
                    toastr.error('No se puede agregar una cantidad menor a 1...');
                    return false
                }
            }


            //Validar Cantidades Ingresadas
            var StockDisponible = parseInt($('#StockDisponible').val());
            var Cantidad = parseInt($('#Cantidad').val());

           /* if (Cantidad > StockDisponible) {*/
                //toastr.error('No se puede agregar este producto, debido a que excede el stock disponible...');
          /*  } else {*/

                var form = $("#AgregarProducto");

                var Clave = $('#Clave').val();
                var Descripcion = $('#Descripcion').val();
                var Cantidad = $('#Cantidad').val();
                var NumLote = $('#NumeroLote').val();
                var IssueInventory = $('#IssueInventory').val();
                var ID_NS = $('#ID_NS').val();
                var idPr = $('#idPr').val();
                var PrecioProducto = $('#Prex').val();
                var TipoMat = $('#TioMat').val();
                var Token = $('#Token').val();

                form.children("[name='ClaveProducto']").val(Clave);
                form.children("[name='Descripcion']").val(Descripcion);
                form.children("[name='Cantidad']").val(Cantidad);
                form.children("[name='NumLote']").val(NumLote);
                form.children("[name='IssueInventory']").val(IssueInventory);
                form.children("[name='ID_NS']").val(ID_NS);
                form.children("[name='idPr']").val(idPr);
                form.children("[name='PrecioU']").val(PrecioProducto);
                form.children("[name='TipoMaterial']").val(TipoMat);
                form.children("[name='Token']").val(Token);

                var dataForm = form.serialize();
                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {

                        tablaMercancia.clear().draw();
                        Carrito.length = 0;

                        $.each(data.Valor, function (index, value) {

                            tablaMercancia.row.add([
                                value.ClaveProducto,
                                value.Descripcion,
                                value.NumLote,
                                accounting.formatMoney(value.PrecioU),
                                value.Cantidad,
                                accounting.formatMoney(value.Total),
                                "<ul class='d-flex justify-content-center'><li onclick='EliminarArticulo(" + value.Vlender + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-trash'></i></a></li></ul>",
                            ]).node().id = value.Vlender;
                            tablaMercancia.draw(false);

                            Carrito.push({ "id": value.Vlender, "ClaveProducto": value.ClaveProducto, "Descripcion": value.Descripcion, "Cantidad": value.Cantidad, "NumLote": value.NumLote, "IssueInventory": value.IssueInventory, "ID_NS": value.ID_NS, "idPr": value.idPr, "Total": value.Total, "TipoMaterial": value.TipoMaterial, "PrecioU": value.PrecioU })

                        });

                        CalcularDetalle();

                        $('#ClaveProducto').val("");
                        $('#Clave').val("");
                        $('#Descripcion').val("");
                        $('#StockDisponible').val("");
                        $('#Cantidad').val("");
                        $('#TipoDescuento').val("");
                        $('#NumeroLote').val("");
                        $('#idPr').val("");
                        $('#PrecioProducto').val("");
                        $('#TioMat').val("");
                        $('#PrecioProductoLista').val("");
                        $('#StockDisponiblePlanta').val("");

                        toastr.success('El Producto Fue Agregado Correctamente...');

                    }
                    else {
                        toastr.error(data.Mensaje);
                    }
                });
            }


           /* }*/

        function EliminarArticulo(id) {
            var form = $("#EliminarProducto");
            var Token = $('#Token').val();
            form.children("[name='Vlender']").val(id);
            form.children("[name='token']").val(Token);

            var dataForm = form.serialize();

            $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                tablaMercancia.clear().draw();
                Carrito.length = 0;

                $.each(data.Valor, function (index, value) {

                    tablaMercancia.row.add([
                        value.ClaveProducto,
                        value.Descripcion,
                        value.NumLote,
                        accounting.formatMoney(value.PrecioU),
                        value.Cantidad,
                        accounting.formatMoney(value.Total),
                        "<ul class='d-flex justify-content-center'><li onclick='EliminarArticulo(" + value.Vlender + ")' class='mr-3'><a href='#' class='text-secondary'><i class='fa fa-trash'></i></a></li></ul>",
                    ]).node().id = value.Vlender;
                    tablaMercancia.draw(false);

                    Carrito.push({ "id": value.Vlender, "ClaveProducto": value.ClaveProducto, "Descripcion": value.Descripcion, "Cantidad": value.Cantidad, "NumLote": value.NumLote, "IssueInventory": value.IssueInventory, "ID_NS": value.ID_NS, "idPr": value.idPr, "Total": value.Total, "PrecioU": value.PrecioU })

                });

                toastr.success('El Producto Se Eliminó Correctamente...');
                CalcularDetalle();
            });



    }

        function CrearOrdenVenta() {

            var Campos = ValidaCamposVacios();

            if (Campos == false) {
                toastr.error("Existen Campos Vacios o no se han seleccionado articulos...");
                return false
            }

            showLoading("Creando Estimación...");

            var templateProducto = _.template(document.getElementById("ProductoListHidde").textContent);
            $("#ProductoListHidden").html("");

            _.each(Carrito, function (item) {
                var dataItem = item
                var html = templateProducto({ Carrito: dataItem, contador: contador })
                $("#ProductoshiddenDivDetalles").append(html);
                contador++;

            }, this);

            $('#FormaPagoSAT').attr('disabled', false);
            $('#UsoCFDI').attr('disabled', false);

            $('#msform').submit();
        }

        function CrearFacturaVenta() {

            var Campos = ValidaCamposVacios();

            if (Campos == false) {
                toastr.error("Existen Campos Vacios o no se han seleccionado articulos...");
                return false
            }

            showLoading("Creando Factura de Venta...");

            var templateProducto = _.template(document.getElementById("ProductoListHidde").textContent);
            $("#ProductoListHidden").html("");

            _.each(Carrito, function (item) {
                var dataItem = item
                var html = templateProducto({ Carrito: dataItem, contador: contador })
                $("#ProductoshiddenDivDetalles").append(html);
                contador++;

            }, this);

            $('#GenerarFacturaDirecta').val(true);
            $('#GenerarVentaDirecta').val(false);

            $('#FormaPagoSAT').attr('disabled', false);
            $('#UsoCFDI').attr('disabled', false);

            $('#msform').submit();
        }

        function CalcularDetalle() {
            var total = 0;
            var totalImpuestos = 0;
            var subtotal = 0;
            $.each(Carrito, function (index, value) {
                subtotal = subtotal + parseFloat(value.Total);
                totalImpuestos = totalImpuestos + parseFloat(((0.16) * value.Total));
            });

            total = subtotal + totalImpuestos;

            $('#DataPSubtotal').text(accounting.formatMoney(subtotal));
            $('#DataPTotal').text(accounting.formatMoney(total));
            $('#DataPTotalIVA').text(accounting.formatMoney(totalImpuestos));
        }

    function ValidaCamposVacios() {
        if ($('#Customer').val() == "") {
            return false
        }

        if ($('#MetodoPagoSAT').val() == "") {
            return false
        }

        if ($('#UsoCFDI').val() == "") {
            return false
        }

        if (Carrito.length === 0) {
            return false
        }
        return true
    }

    //jQuery(document).ready(function ($) {

    //    if (window.history && window.history.pushState) {

    //        window.history.pushState('forward', null, './#forward');

    //        $(window).on('popstate', function () {
    //            //alert('Back button was pressed.');
    //        });

    //    }
    //});

    $('#MetodoPagoSAT').change(function () {
        var id = $('#MetodoPagoSAT').val();

        if (id == "4") {
            $('#FormaPagoSAT').val("28");
            $('#FormaPagoSAT').attr('disabled', true);
        } else {
            $('#FormaPagoSAT').val("");
            $('#FormaPagoSAT').attr('disabled', false);
        }

    });

    function EntregarVentaFacturar() {

        var Campos = ValidaCamposVacios();

        if (Campos == false) {
            toastr.error("Existen Campos Vacios o no se han seleccionado articulos...");
            return false
        }

        showLoading("Creando Factura de Venta...");

        var templateProducto = _.template(document.getElementById("ProductoListHidde").textContent);
        $("#ProductoListHidden").html("");

        _.each(Carrito, function (item) {
            var dataItem = item
            var html = templateProducto({ Carrito: dataItem, contador: contador })
            $("#ProductoshiddenDivDetalles").append(html);
            contador++;

        }, this);

        $('#GenerarFacturaDirecta').val(false);
        $('#GenerarVentaDirecta').val(true);


        $('#FormaPagoSAT').attr('disabled', false);
        $('#UsoCFDI').attr('disabled', false);

        $('#msform').submit();
    }

</script>
End Section
