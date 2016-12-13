using DropshipCommon.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Dropship
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DropshipWebContext.Instance.Initialize();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //run schedule task
            //TaskManager.Instance.Initialize();
            //TaskManager.Instance.Start();
        }
    }
}
