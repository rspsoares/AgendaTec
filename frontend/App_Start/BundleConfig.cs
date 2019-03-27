using System.Web.Optimization;

namespace AgendaTech.View
{
    public class BundleConfig
    {        
        public static void RegisterBundles(BundleCollection bundles)
        {
            /* -- SCRIPTS -- */
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Content/jquery/jquery-2.2.4.min.js",
                  "~/Content/jquery/jquery-treeview.min.js",
                  "~/Content/jquery/jquery.PrintArea.min.js",
                  "~/Content/jquery/jquery.maskedinput.min.js",
                  "~/Content/jquery/jquery.cookie.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                   "~/Content/kendo-ui/js/kendo.all.min.js",
                   "~/Content/kendo-ui/js/cultures/kendo.culture.pt-BR.min.js",
                   "~/Content/kendo-ui/js/cultures/kendo.pt-BR.min.js",
                   "~/Content/kendo-ui/js/jszip.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Content/bootstrap/js/bootstrap.js",
                    "~/Content/bootstrap/js/bootstrap-switch.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui/js").Include(
                      "~/Content/plugins/jquery-ui/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryValidate").Include(
                    "~/Content/jqueryvalidate/jquery.validate.js",
                    "~/Content/jqueryvalidate/messages_pt_BR.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-mosaic").Include(
                     "~/Content/js/mosaic-script.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Content/modernizr/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Content/angular/angular-1.2.19.js"));

            bundles.Add(new ScriptBundle("~/plugins/flot").Include(
                    "~/Content/plugins/flot/jquery.flot.js",
                    "~/Content/plugins/flot/jquery.flot.tooltip.min.js",                      
                    "~/Content/plugins/flot/jquery.flot.pie.js",
                    "~/Content/plugins/flot/jquery.flot.time.js",
                    "~/Content/plugins/flot/jquery.flot.spline.js",
                    "~/Content/plugins/flot/jquery.flot.categories.js",
                    "~/Content/plugins/flot/jquery.flot.tickrotor.js",
                    "~/Content/plugins/flot/jquery.flot.axislabels.js",                      
                    "~/Content/plugins/flot/jquery.colorhelpers.js",
                    "~/Content/plugins/flot/jquery.flot.canvas.js",                    
                    "~/Content/plugins/flot/base64.js",
                    "~/Content/plugins/flot/canvas2image.js",
                    "~/Content/plugins/flot/jquery.flot.saveAsImage.js"));

            bundles.Add(new ScriptBundle("~/plugins/flotWaterfall").Include(
                    "~/Content/plugins/flot/jquery.flot.js",
                    "~/Content/plugins/flot/jquery.flot.time.js",
                    "~/Content/plugins/flot/jquery.flot.stack.js",
                    "~/Content/plugins/flot/jquery.flot.JUMlib.js",
                    "~/Content/plugins/flot/jquery.flot.tickrotor.js",
                    "~/Content/plugins/flot/jquery.flot.canvas.js",
                    "~/Content/plugins/flot/jquery.flot.valuelables.js",
                    "~/Content/plugins/flot/base64.js",
                    "~/Content/plugins/flot/canvas2image.js",
                    "~/Content/plugins/flot/jquery.flot.saveAsImage.js"
                    ));
            /* -- CSS -- */
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/kendo-ui/styles/web/kendo.common.min.css",
                "~/Content/kendo-ui/styles/web/kendo.metro.min.css",
                "~/Content/kendo-ui/styles/web/kendo.mobile.all.css",
                "~/Content/bootstrap/css/bootstrap.css",
                "~/Content/bootstrap/css/bootstrap-switch.css",
                "~/Content/bootstrap/css/mosaic-custom-bootstrap.css",
                "~/Content/mosaic-default.css"));           
        }
    }
}