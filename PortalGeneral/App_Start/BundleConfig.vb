Imports System.Web.Optimization

Public Module BundleConfig
    ' Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
    Public Sub RegisterBundles(ByVal bundles As BundleCollection)

        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"))

        ' Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
        ' para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"))

        bundles.Add(New ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/site.css"))

        bundles.Add(New StyleBundle("~/Content/chosenSelect").Include(
                    "~/Content/chosen.css",
                    "~/Content/chosen.min.css"))

        bundles.Add(New StyleBundle("~/Content/Templates").Include(
                  "~/Assets/css/bootstrap.min.css",
                  "~/Assets/css/default-css.css",
                  "~/Assets/css/font-awesome.min.css",
                  "~/Assets/css/metisMenu.css",
                  "~/Assets/css/owl.carousel.min.css",
                  "~/Assets/css/responsive.css",
                  "~/Assets/css/slicknav.min.css",
                  "~/Assets/css/styles.css",
                  "~/Assets/css/themify-icons.css",
                  "~/Assets/css/typography.css"))

        bundles.Add(New ScriptBundle("~/bundles/TemplateJS").Include(
                  "~/Scripts/jquery-{version}.js",
                  "~/Scripts/toastr.js",
                  "~/Assets/js/popper.min.js",
                  "~/Assets/js/bootstrap.min.js",
                  "~/Assets/js/owl.carousel.min.js",
                  "~/Assets/js/metisMenu.min.js",
                  "~/Assets/js/jquery.slimscroll.min.js",
                  "~/Assets/js/jquery.slicknav.min.js",
                  "~/Assets/js/line-chart.js",
                  "~/Assets/js/pie-chart.js",
                  "~/Assets/js/bootstrapWizard.js",
                  "~/Assets/js/bootstrapWizard.min.js",
                  "~/Scripts/underscore.js",
                  "~/Scripts/backbone.js",
                  "~/Scripts/Util.js",
                  "~/Scripts/chosen.jquery.js",
                  "~/Scripts/chosen.jquery.min.js",
                  "~/Scripts/accounting.js"))

        bundles.Add(New ScriptBundle("~/bundles/BundSJ").Include(
                  "~/Assets/js/plugins.js",
                  "~/Assets/js/scripts.js"))

        bundles.Add(New ScriptBundle("~/bundles/Security").Include(
                  "~/Assets/js/sha1.js",
                  "~/Assets/js/sha1-min.js"))

        bundles.Add(New ScriptBundle("~/bundles/Carrito").Include(
                 "~/Assets/js/jquery.smartCart.js"))

        bundles.Add(New ScriptBundle("~/bundles/until").Include(
          "~/Scripts/Util.js"))

    End Sub
End Module

