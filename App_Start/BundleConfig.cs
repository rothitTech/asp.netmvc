using System.Web;
using System.Web.Optimization;

namespace EDS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/EDS.css",
                      "~/Content/site.css",
                      "~/Content/GrideTable.css",
                      "~/Content/webgrid.css"));

            bundles.Add(new ScriptBundle("~/bundles/dataTablesJs").Include(
                      "~/Scripts/dataTables/Extensions/FixedColumns/js/jquery.dataTables.js",
                      "~/Scripts/dataTables/Extensions/FixedColumns/js/dataTables.fixedColumns.js",
                      "~/Scripts/dataTables/Extensions/FixedColumns/js/shCore.js",
                      "~/Scripts/dataTables/Extensions/FixedColumns/js/numeric-null.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/dataTablesCss").Include(
                     "~/Scripts/dataTables/Extensions/FixedColumns/css/jquery.dataTables.css",
                     "~/Scripts/dataTables/Extensions/FixedColumns/css/dataTables.fixedColumns.css",
                     "~/Scripts/dataTables/Extensions/FixedColumns/css/shCore.css"
                     ));
        }
    }
}
