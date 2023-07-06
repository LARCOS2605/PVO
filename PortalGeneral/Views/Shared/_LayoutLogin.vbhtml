@Imports MvcFlash.Core.Extensions

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="x-ua-compatible" content="ie=edge">
    <title>Login - Alvarez Automotriz SA de CV</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="shortcut icon" type="image/png" href="~/assets/images/favicon.ico">
    <link rel="stylesheet" href="~/assets/css/bootstrap.min.css">
    <link rel="stylesheet" href="~/assets/css/font-awesome.min.css">
    <link rel="stylesheet" href="~/assets/css/themify-icons.css">
    <link rel="stylesheet" href="~/assets/css/metisMenu.css">
    <link rel="stylesheet" href="~/assets/css/owl.carousel.min.css">
    <link rel="stylesheet" href="~/assets/css/slicknav.min.css">
    <!-- amchart css -->
    <link rel="stylesheet" href="https://www.amcharts.com/lib/3/plugins/export/export.css" type="text/css" media="all" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.css" type="text/css" media="all" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css" type="text/css" media="all" />
    <!-- others css -->
    <link rel="stylesheet" href="~/assets/css/typography.css">
    <link rel="stylesheet" href="~/assets/css/default-css.css">
    <link rel="stylesheet" href="~/assets/css/styles.css">
    <link rel="stylesheet" href="~/assets/css/responsive.css">
    <!-- modernizr css -->
    <script src="~/assets/js/vendor/modernizr-2.8.3.min.js"></script>
</head>


<body>
    <div id="preloader">
        <div class="loader"></div>
    </div>

    <div class="container">
        @Html.Flash()
    </div>
    @RenderBody()

    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/TemplateJS")
    <!-- Start datatable js -->
    <script src="https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js"></script>
    <script src="https://cdn.datatables.net/1.10.18/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.18/js/dataTables.bootstrap4.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.2.3/js/dataTables.responsive.min.js"></script>
    <script src="https://cdn.datatables.net/responsive/2.2.3/js/responsive.bootstrap.min.js"></script>
    @Scripts.Render("~/bundles/BundSJ")
    @Scripts.Render("~/bundles/modernizr")
    @Scripts.Render("~/bundles/Security")

    @*@Scripts.Render("~/bundles/bootstrap")*@
    @RenderSection("scripts", required:=False)
</body>
</html>
