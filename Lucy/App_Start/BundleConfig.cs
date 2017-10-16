using System.Web;
using System.Web.Optimization;

namespace Lucy
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-select.min.css",
                      "~/Content/site.css"));

            // bootstrap-datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker").Include(
                      "~/Scripts/moment-with-locales.min.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-datetimepicker").Include(
                      "~/Content/bootstrap-datetimepicker-build.less",
                      "~/Content/bootstrap-datetimepicker.min.css"));

            // Mis script
            bundles.Add(new ScriptBundle("~/bundles/myScript").Include(
                      "~/Scripts/slideRigth.js",
                      "~/Scripts/menuScroll.js",
                      "~/Scripts/customDropdown.js",
                      "~/Scripts/sidebarMenuHi.js"));

            // Tablas
            bundles.Add(new ScriptBundle("~/bundles/Tablas").Include(
                "~/Scripts/jquery.sortelements.js",
                "~/Scripts/jquery.bdt.min.js"));

            // Slides
            bundles.Add(new ScriptBundle("~/bundles/Slides").Include(
                "~/Scripts/jquery-ui-{version}.min.js",
                "~/Scripts/slick.js",
                "~/Scripts/SlickSlider.js"));

            // Slick
            bundles.Add(new StyleBundle("~/Content/Slick").Include(
                      "~/Content/slick-theme.css",
                      "~/Content/slick.css"));
        }
    }
}
