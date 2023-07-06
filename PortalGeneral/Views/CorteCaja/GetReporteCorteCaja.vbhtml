@modeltype PortalGeneral.GeneraReporteIngresosViewModel
@Code
    Layout = Nothing
End Code
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=8">

    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/bootstrap.min.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/default-css.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/font-awesome.min.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/metisMenu.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/owl.carousel.min.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/responsive.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/slicknav.min.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/styles.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/themify-icons.css")">
    <link rel="stylesheet" href="@Server.MapPath("~/Assets/css/typography.css")">

    <script src="@Server.MapPath("~/Assets/js/bootstrap.min.js")"></script>

    <style>
        .page-breaker {
            page-break-after: always
        }
        tr {
            page-break-inside: avoid;
        }
    </style>
</head>

<body>
    <P CLASS="page-breaker">
        <div class="main-content-inner">
            <div class="row">
                <div class="col-lg-12 mt-5">

                    <div class="card">
                        <div class="card-body">
                            <div class="invoice-area">
                                <div class="invoice-head">
                                    <div class="row">
                                        <div class="iv-left col-6">
                                            <span>Resumen de Corte de Caja</span>
                                        </div>
                                        <div class="iv-right col-6 text-md-right">
                                            <span>Desde @Model.FechaInicio.ToString("dd/MM/yyyy") - </span>
                                            <span>Al: @Model.FechaFin.ToString("dd/MM/yyyy")</span>
                                        </div>
                                    </div>
                                </div>
                                <div class="single-table">
                                    <table class="table table-hover progress-table text-center">
                                        <thead>
                                            <tr class="text-capitalize">
                                                <th nowrap class="text-center">Cliente</th>
                                                <th nowrap class="text-center">Nombre</th>
                                                <th nowrap class="text-center">Credito</th>
                                                <th nowrap class="text-center">Vendedor</th>
                                                <th nowrap class="text-center">Fecha</th>
                                                <th nowrap class="text-center">Forma de Pago</th>
                                                <th nowrap class="text-center">Folio Factura</th>
                                                <th nowrap class="text-center">No. Factura</th>
                                                <th nowrap class="text-center">SubTotal </th>
                                                <th nowrap class="text-center">IVA</th>
                                                <th nowrap class="text-center">Total </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            @For Each item In Model.l_CorteCaja
                                                Dim currentItem As NetsuiteLibrary.Clases.MapCorteCajaRep = item
                                                @<tr>
                                                    <td nowrap Class="text-center">@currentItem.ClaveCliente</td>
                                                    <td nowrap Class="text-center">@currentItem.NombreCliente</td>
                                                    <td nowrap Class="text-center"></td>
                                                    <td nowrap Class="text-center">@currentItem.Usuario</td>
                                                    <td nowrap Class="text-center">@currentItem.Fecha.ToString("dd/MM/yyyy")</td>
                                                    <td nowrap Class="text-center">@currentItem.Descripcion_MetodoPago</td>
                                                    <td nowrap Class="text-center">@currentItem.FolioFactura</td>
                                                    <td nowrap Class="text-center">@currentItem.NoFactura</td>
                                                    <td nowrap Class="text-center">@Convert.ToDecimal(currentItem.SubtotalFactura).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                                    <td nowrap Class="text-center">@Convert.ToDecimal(currentItem.IvaFactura).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                                    <td nowrap Class="text-center">@Convert.ToDecimal(currentItem.TotalFactura).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                                </tr>
                                            Next
                                        </tbody>                                     
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </P>
    <P CLASS="page-breaker">
        <div class="main-content-inner">
            <div class="row">
                <div class="col-lg-12 mt-5">

                    <div class="card">
                        <div class="card-body">
                            <div class="invoice-area">
                                <div class="single-table">
                                    <table class="table table-hover progress-table text-center">
                                        <thead>
                                            <tr class="text-capitalize">
                                                <th nowrap class="text-center" colspan="2">Resumen General</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Cheque</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.Cheque).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Efectivo</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.Efectivo).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Credito</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.Credito).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Tarjeta de Debito</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.TarjetaDebito).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Tarjeta de Credito</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.TarjetaCredito).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Transferencia</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.Transferencia).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center"><strong>Total</strong></td>
                                                <td nowrap Class="text-center">@Convert.ToDecimal(Model.totalFechaPago).ToString("C", New System.Globalization.CultureInfo("es-MX"))</td>
                                            </tr>
                                            <tr>
                                                <td nowrap Class="text-center">No. de Facturas</td>
                                                <td nowrap Class="text-center">@Model.l_CorteCaja.Count</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </P>
</body>

</html>

