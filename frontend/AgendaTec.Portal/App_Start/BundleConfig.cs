﻿using System.Web.Optimization;

namespace AgendaTec.Portal
{
    public class BundleConfig
    {        
        public static void RegisterBundles(BundleCollection bundles)
        {
            /* -- SCRIPTS -- */
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  //"~/Content/jquery/jquery-2.2.4.min.js",                  
                  "~/Content/jquery/jquery-1.12.4.min.js",
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

            bundles.Add(new ScriptBundle("~/bundles/menu-script").Include(
                     "~/Content/js/menu.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Content/modernizr/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Content/angular/angular-1.2.19.js"));

            //fineUploader css
            bundles.Add(new StyleBundle("~/bundles/fineuploader/css").Include(
                      "~/Content/fineUploader/fine-uploader.min.css", new CssRewriteUrlTransform()).Include(
                      "~/Content/fineUploader/fine-uploader-new.min.css"));

            //fineUploader js
            bundles.Add(new ScriptBundle("~/bundles/fineuploader/js").Include(
                      "~/Content/fineUploader/fine-uploader.core.min.js",
                      "~/Content/fineUploader/fine-uploader.min.js"));

            /* -- CSS -- */
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/kendo-ui/styles/web/kendo.common.min.css",
                "~/Content/kendo-ui/styles/web/kendo.blueopal.min.css",
                "~/Content/kendo-ui/styles/web/kendo.blueopal.mobile.all.css",
                "~/Content/bootstrap/css/bootstrap.css",
                "~/Content/bootstrap/css/bootstrap-switch.css",
                "~/Content/bootstrap/css/agendatec-custom-bootstrap.css",                 
                "~/Content/agendatec-default.css"));       
        }
    }
}