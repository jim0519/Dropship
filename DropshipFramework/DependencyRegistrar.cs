using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DropshipCommon;
using DropshipCommon.Infrastructure;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using DropshipData;
using System.Data.Entity.Core.Metadata.Edm;
//using DropshipBusiness.SalesOrder;
using DropshipBusiness.Item;
using DropshipBusiness.eBay;
using DropshipBusiness.Listing;
using DropshipBusiness.Common;
using System.Web;
using DropshipBusiness.User;
using DropshipBusiness.Security;
using DropshipFramework;
using DropshipBusiness.Events;
//using DropshipBusiness.Task;
//using DropshipBusiness.eBay;
//using DropshipBusiness.ExportImport;

namespace DropshipData
{
    public class DependencyRegistrarData : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {

            //HTTP context and other related stuff
            //builder.Register(c =>
            //    //register FakeHttpContext when HttpContext is not available
            //    HttpContext.Current != null ?
            //    (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
            //    (new FakeHttpContext("~/") as HttpContextBase))
            //    .As<HttpContextBase>()
            //    .InstancePerLifetimeScope();
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //PerHttpRequest
            
            //do not change the order of the registration because autofac will always resolve the one having the latest registration, by default it should resolve the default db. 
            //builder.RegisterType<NewAimWebAPIClientDbContext>().As<IDbContext>().Keyed<IDbContext>(Constants.APIClientDBKey).InstancePerHttpRequest();
            //builder.RegisterType<NewAimWebDMDbContext>().As<IDbContext>().Keyed<IDbContext>(Constants.DeliveryManagementSystemDBKey).InstancePerHttpRequest();
            //builder.RegisterType<NewAimWebWMSDbContext>().As<IDbContext>().Keyed<IDbContext>(Constants.WMSDBKey).InstancePerHttpRequest();
            builder.RegisterType<DropshipDBContext>().As<IDbContext>().Keyed<IDbContext>(Constants.DropshipDBKey).InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(DropshipRepository<>)).As(typeof(IRepository<>)).WithParameter(
                new ResolvedParameter((pi, c) => pi.ParameterType == typeof(IDbContext), (pi, c) => c.ResolveKeyed<IDbContext>(SelectDB(pi.Member.DeclaringType.GetGenericArguments()[0])))).InstancePerLifetimeScope();


            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //Sigleton
            builder.RegisterType<CacheManager>().As<ICacheManager>().SingleInstance();

            //Common
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerLifetimeScope();
            builder.RegisterType<PostageRuleService>().As<IPostageRuleService>().InstancePerLifetimeScope();

            //Item
            builder.RegisterType<ItemService>().As<IItemService>().InstancePerLifetimeScope();
            builder.RegisterType<ImageService>().As<IImageService>().InstancePerLifetimeScope();

            //Listing
            builder.RegisterType<ListingService>().As<IListingService>().InstancePerLifetimeScope();

            //User
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

            //Security
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();
            

            //Schedule Task
            //builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();

            //eBay Service
            builder.RegisterType<eBayAPIContextProvider>().As<IeBayAPIContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<eBayAPICallManager>().As<IeBayAPICallManager>().InstancePerLifetimeScope();

            //Dropship Framework
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //Import/Export Manager
            //builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();

            //Event
            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }

        private string SelectDB(Type type)
        {
            var dbcontext = DropshipWebContext.Instance.Resolve<IDbContext>(Constants.DropshipDBKey);
            if (ExistsInDBContext(type, dbcontext))
            {
                return Constants.DropshipDBKey;
            }

            //dbcontext=DropshipWebContext.Instance.Resolve<IDbContext>(Constants.DeliveryManagementSystemDBKey);
            //if (ExistsInDBContext(type, dbcontext))
            //{
            //    return Constants.DeliveryManagementSystemDBKey;
            //}

            //dbcontext = DropshipWebContext.Instance.Resolve<IDbContext>(Constants.WMSDBKey);
            //if (ExistsInDBContext(type, dbcontext))
            //{
            //    return Constants.WMSDBKey;
            //}

            //dbcontext = DropshipWebContext.Instance.Resolve<IDbContext>(Constants.APIClientDBKey);
            //if (ExistsInDBContext(type, dbcontext))
            //{
            //    return Constants.APIClientDBKey;
            //}

            return Constants.DropshipDBKey;
        }

        private bool ExistsInDBContext(Type type,IDbContext dbcontext)
        {
            string entityName = type.Name;
            var objContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
            MetadataWorkspace workspace = objContext.MetadataWorkspace;
            return workspace.GetItems<EntityType>(DataSpace.CSpace).Any(e => e.Name == entityName);
        }

        #endregion
    }
}
