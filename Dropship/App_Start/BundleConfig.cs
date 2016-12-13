using System.Web;
using System.Web.Optimization;

namespace Dropship
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui-1.10.3.custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").IncludeDirectory("~/Content/bootstrap/js", "*.js"));
            bundles.Add(new StyleBundle("~/bundles/bootstrapcss").IncludeDirectory("~/Content/bootstrap/css", "*.css"));

            bundles.Add(new ScriptBundle("~/bundles/kendojs").Include("~/Content/kendo/js/kendo.web.min.js"));
            bundles.Add(new StyleBundle("~/bundles/kendocss").Include("~/Content/kendo/css/kendo.common.min.css",
                "~/Content/kendo/css/kendo.default.min.css",
                "~/Content/kendo/css/kendo.rtl.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include("~/Scripts/common.js"));
        }
    }
}
