@modeltype PortalGeneral.DatosSolicitudTrasladoViewModel
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
        <!-- page title area end -->
        <div class="card">
            <div class="main-content-inner">
                <div class="row">
                    <div class="col-lg-12 mt-5">
                        <div class="card">
                            <div class="card-body">
                                <div class="invoice-area">
                                    <div class="invoice-head">
                                        <div class="row">
                                            <div class="iv-left col-4">
                                                <span>Orden de Traslado</span>
                                            </div>
                                            <div class="iv-right col-6 text-md-right">
                                                <span>#@Model.NombreTransaccion</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row align-items-center">
                                        <div class="iv-left col-4">
                                            <div class="invoice-address">
                                                <h3>Datos de Traslado:</h3>
                                                <h5>Mostrador Origen: @Model.AlmacenOrigen</h5>
                                                <h5>Mostrador Destino: @Model.AlmacenDestino</h5>
                                                <h5>Fecha de Creación: @Model.FechaCreacion</h5>
                                                <h5>Nota: @Model.Nota</h5>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="single-table">
                                        <table class="table table-hover progress-table text-center">
                                            <thead>
                                                <tr class="text-capitalize">
                                                    <th nowrap class="text-center" style="width: 5%;">ARTICULO</th>
                                                    <th nowrap class="text-left">DESCRIPCION</th>
                                                    <th nowrap >CANTIDAD SOLICITUD</th>
                                                    <th nowrap >CANTIDAD ENVIADA</th>
                                                    <th nowrap >CANTIDAD RECIBIDA</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                @For Each item In Model.l_Detalle
                                                    Dim currentItem As DetalleSolicitudTrasladoViewModel = item
                                                    @<tr>
                                                        <td nowrap Class="text-center">@currentItem.Articulo</td>
                                                        <td nowrap Class="text-left">@currentItem.Descripcion</td>
                                                        <td nowrap >@currentItem.CantidadEnviada</td>
                                                        <td nowrap >@currentItem.CantidadSolicitud</td>
                                                        <td nowrap>@currentItem.CantidadRecibida</td>
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
        </div>    
    </P>
</body>

</html>

