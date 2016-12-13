using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using DropshipCommon;
using System.Web;
using Autofac.Core.Lifetime;
using System.Web.Mvc;


namespace DropshipCommon.Infrastructure
{
    public class ContainerManager
    {
        private readonly IContainer _container;

        public ContainerManager(IContainer container)
        {
            _container = container;
        }

        public IContainer Container
        {
            get
            {
                return _container;
            }
        }

        public void RegisterDependency()
        {
            //var builder = new ContainerBuilder();

            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
            ////builder.Register(x => new DropShipSendOutProcess()).As<DropShipSendOutProcess>();
            ////builder.RegisterType<DropShipSendOutProcess>().AsSelf().InstancePerHttpRequest();
            //builder.RegisterType<DropShipSendOutProcess>().As<IDropShipSendOutProcess>().InstancePerHttpRequest();
            //builder.RegisterType<PostageEnquiryService>().As<IPostageEnquiryService>().InstancePerHttpRequest();
            //builder.RegisterType<NewAimWebDbContext>().As<IDbContext>().InstancePerHttpRequest();
            //builder.RegisterGeneric(typeof(NewAimWebRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            ////builder.RegisterControllers(typeof(MvcApplication).Assembly);
            //builder.Update(_container);

            //load assembly to register dependency so that the dependency file can be placed in the highest level of the solution for registering all concrete class

            //type finder
            //AddComponent<ITypeFinder, AppAllDLLTypeFinder>("typeFinder");

            var builder = new ContainerBuilder();
            var typeFinder = new AppAllDLLTypeFinder();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            builder.Update(_container);

            //get type finders after registering
            //var typeFinder = _container.Resolve<ITypeFinder>();

            //register dependencies provided by other assemblies
            UpdateContainer(b =>
            {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = new List<IDependencyRegistrar>();
                foreach (var drType in drTypes)
                    drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(b, typeFinder);
            });


            //set dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            
        }

        public T Resolve<T>(string key = "")
            where T: class
        {
            if (string.IsNullOrEmpty(key))
            {
                if (Scope().IsRegistered(typeof(T)))
                {
                    return Scope().Resolve<T>();
                }
            }
            return Scope().ResolveKeyed<T>(key);
        }

        public ILifetimeScope Scope()
        {
            try
            {
                if (HttpContext.Current != null)
                    return AutofacDependencyResolver.Current.RequestLifetimeScope;

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception exc)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }

        public object Resolve(Type type)
        {
            return Scope().Resolve(type);
        }

        public object ResolveOptional(Type serviceType)
        {
            return Scope().ResolveOptional(serviceType);
        }




        public void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent<TService, TService>(key, lifeStyle);
        }

        public void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent(service, service, key, lifeStyle);
        }

        public void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent(typeof(TService), typeof(TImplementation), key, lifeStyle);
        }

        public void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            UpdateContainer(x =>
            {
                var serviceTypes = new List<Type> { service };

                if (service.IsGenericType)
                {
                    var temp = x.RegisterGeneric(implementation).As(
                        serviceTypes.ToArray()).InstancePerHttpRequest(lifeStyle);
                    if (!string.IsNullOrEmpty(key))
                    {
                        temp.Keyed(key, service);
                    }
                }
                else
                {
                    var temp = x.RegisterType(implementation).As(
                        serviceTypes.ToArray()).PerLifeStyle(lifeStyle);
                    if (!string.IsNullOrEmpty(key))
                    {
                        temp.Keyed(key, service);
                    }
                }
            });
        }

        public void UpdateContainer(Action<ContainerBuilder> action)
        {
            var builder = new ContainerBuilder();
            action.Invoke(builder);
            builder.Update(_container);
        }
    }
}
