using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using DropshipCommon.Infrastructure;
using DropshipCommon;

namespace DropshipCommon.Infrastructure
{
    public class DropshipWebContext
    {
        private static DropshipWebContext _instance;
        private ContainerManager _containerManager;
        private ICacheManager _cacheManager;
        private DropshipWebContext()
        {

        }

        public static DropshipWebContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DropshipWebContext();
                }
                return _instance;
            }
        }

        public void Initialize()
        {

            //dependency injection
            _containerManager = new ContainerManager(new ContainerBuilder().Build());
            _containerManager.RegisterDependency();

            //set dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_containerManager.Container));

            //InitializeCache();

            //Run startup task
            RunStartupTasks();

        }

        private void InitializeCache()
        {
            _cacheManager = Resolve<ICacheManager>();
            //_cacheManager.Get(DropshipCacheKey.ItemStatusList, CacheFunc.GetOrderStatusList);
            //_cacheManager.Get(DropshipCacheKey.ListingStatusList, CacheFunc.GetOrderTypeList);
            //_cacheManager.Get(DropshipCacheKey.SupplierList, CacheFunc.GetPaymentStatusList);
            //_cacheManager.Get(DropshipCacheKey.ListingChannelList, CacheFunc.GetPaymentStatusList);
        }

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        //public T Resolve<T>() where T : class
        //{
        //    return _containerManager.Resolve<T>();
        //}

        public object Resolve(Type type)
        {
            return _containerManager.Resolve(type);
        }

        public object ResolveOptional(Type serviceType)
        {
            return _containerManager.ResolveOptional(serviceType);
        }

        //public T Resolve<T>(string key = "") where T : class
        //{
        //    try
        //    {
        //        return _containerManager.Resolve<T>(key);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = _containerManager.Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>();
            }
            return scope.ResolveKeyed<T>(key);
        }

        public T[] ResolveAll<T>(string key = "")
        {
            return _containerManager.Resolve<IEnumerable<T>>(key).ToArray();
        }

        public object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = _containerManager.Scope();
            }
            return scope.Resolve(type);
        }

        public bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = _containerManager.Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }

        public T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }

        public object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope =_containerManager.Scope();
            }
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new Exception("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Exception)
                {

                }
            }
            throw new Exception("No contructor was found that had all the dependencies satisfied.");
        }

        //public ILifetimeScope BeginLifetimeScope()
        //{
        //    return _containerManager.Scope();
        //}
        
    }
}