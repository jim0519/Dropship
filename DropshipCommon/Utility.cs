using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.IO;
using Autofac;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Net;
//using OMSCommon.Models;
using System.Xml;
using System.Diagnostics;
using System.Web.Hosting;

namespace DropshipCommon
{
    #region DropshipWebClient
    public class DropshipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 30 * 60 * 1000;
            return w;
        }
    }
    #endregion

    #region Log Manager
    public class LogManager
    {
        private static ILog _instance;
        private LogManager()
        {

        }

        public static ILog Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = log4net.LogManager.GetLogger("CustomLogger");
                }
                return _instance;
            }
        }

    }
    #endregion

    #region Common Interface

    public interface IObjectParser
    {
        T ParseStringToObject<T>(string objString);
    }

    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute;

        IEnumerable<Assembly> FindAssembliesWithAttribute<T>();

        IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies);

        IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath);
    }

    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }

    #endregion

    #region Implementation class

    public class XMLObjectParser : IObjectParser
    {
        public T ParseStringToObject<T>(string objString)
        {
            try
            {
                if (string.IsNullOrEmpty(objString))
                {
                    return default(T);
                }
                var deserializer = new XmlSerializer(typeof(T));
                using (var strReader = new StringReader(objString))
                {
                    var returnObj = (T)deserializer.Deserialize(strReader);
                    return returnObj;
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
            return default(T);
        }
    }

    public class AppDomainTypeFinder : ITypeFinder
    {

        #region Fields

        private bool loadAppDomainAssemblies = true;
        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha|^Zen.Barcode";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();

        /// <summary>
        /// Caches attributed assembly information so they don't have to be re-read
        /// </summary>
        private readonly List<AttributedAssembly> _attributedAssemblies = new List<AttributedAssembly>();
        /// <summary>
        /// Caches the assembly attributes that have been searched for
        /// </summary>
        private readonly List<Type> _assemblyAttributesSearched = new List<Type>();

        #endregion

        #region Constructor

        public AppDomainTypeFinder()
        {

        }

        #endregion

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>Gets or sets wether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns are applied when loading these assemblies.</summary>
        public bool LoadAppDomainAssemblies
        {
            get { return loadAppDomainAssemblies; }
            set { loadAppDomainAssemblies = value; }
        }

        /// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames
        {
            get { return assemblyNames; }
            set { assemblyNames = value; }
        }

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        /// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies arn't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return assemblyRestrictToLoadingPattern; }
            set { assemblyRestrictToLoadingPattern = value; }
        }

        #endregion

        #region ITypeFinder Members

        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                foreach (var a in assemblies)
                {
                    foreach (var t in a.GetTypes())
                    {
                        if (assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        {
                            if (!t.IsInterface)
                            {
                                if (onlyConcreteClasses)
                                {
                                    if (t.IsClass && !t.IsAbstract)
                                    {
                                        result.Add(t);
                                    }
                                }
                                else
                                {
                                    result.Add(t);
                                }
                            }
                        }
                    }

                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute
        {
            var found = FindAssembliesWithAttribute<TAssemblyAttribute>();
            return FindClassesOfType<T>(found, onlyConcreteClasses);
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>()
        {
            return FindAssembliesWithAttribute<T>(GetAssemblies());
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies)
        {
            //check if we've already searched this assembly);)
            if (!_assemblyAttributesSearched.Contains(typeof(T)))
            {
                var foundAssemblies = (from assembly in assemblies
                                       let customAttributes = assembly.GetCustomAttributes(typeof(T), false)
                                       where customAttributes.Any()
                                       select assembly).ToList();
                //now update the cache
                _assemblyAttributesSearched.Add(typeof(T));
                foreach (var a in foundAssemblies)
                {
                    _attributedAssemblies.Add(new AttributedAssembly { Assembly = a, PluginAttributeType = typeof(T) });
                }
            }

            //We must do a ToList() here because it is required to be serializable when using other app domains.
            return _attributedAssemblies
                .Where(x => x.PluginAttributeType.Equals(typeof(T)))
                .Select(x => x.Assembly)
                .ToList();
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Adds specificly configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string assemblyName in AssemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The name of the assembly to check.
        /// </param>
        /// <returns>
        /// True if the assembly should be loaded into Nop.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly name to match.
        /// </param>
        /// <param name="pattern">
        /// The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        /// True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">
        /// The physical path to a directory containing dlls to load in the app domain.
        /// </param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = new List<string>();
            foreach (Assembly a in this.GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (string dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        App.Load(an);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Does type implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Nested classes

        private class AttributedAssembly
        {
            internal Assembly Assembly { get; set; }
            internal Type PluginAttributeType { get; set; }
        }

        #endregion
    }

    public class AppAllDLLTypeFinder : AppDomainTypeFinder
    {
        private bool _binFolderAssembliesLoaded = false;
        #region Ctor

        public AppAllDLLTypeFinder()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a physical disk path of \Bin directory
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string GetBaseDirectory()
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HttpRuntime.BinDirectory;
            }
            else
            {
                //not hosted. For example, run either in unit tests
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public override IList<Assembly> GetAssemblies()
        {
            if (!_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                string binPath = GetBaseDirectory();
                //binPath = _webHelper.MapPath("~/bin");
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }

        #endregion
    }

    #endregion

    #region Common Extension Method

    public static class ContainerManagerExtensions
    {
        public static Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(this Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder, ComponentLifeStyle lifeStyle)
        {
            switch (lifeStyle)
            {
                case ComponentLifeStyle.LifetimeScope:
                    return builder.InstancePerLifetimeScope();
                case ComponentLifeStyle.Transient:
                    return builder.InstancePerDependency();
                case ComponentLifeStyle.Singleton:
                    return builder.SingleInstance();
                default:
                    return builder.SingleInstance();
            }
        }
    }

    public static class CommonExtension
    {

        public static IEnumerable<T> ToEnumerable<T>(this T entity)
        {
            yield return entity;
        }

        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }

        public static Expression<Func<T>> ToExpression<T>(Func<T> call)
        {
            MethodCallExpression methodCall = call.Target == null
                ? Expression.Call(call.Method)
                : Expression.Call(Expression.Constant(call.Target), call.Method);

            return Expression.Lambda<Func<T>>(methodCall);
        }

        public static T FillOutNull<T>(this T obj) where T: class, new()
        {
            var t = typeof(T);
            foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                var originValue = p.GetValue(obj);
                if (originValue == null)
                {
                    switch (Type.GetTypeCode(p.PropertyType))
                    {
                        case TypeCode.String:
                            p.SetValue(obj, "", null);
                            break;
                        case TypeCode.Boolean:
                            p.SetValue(obj, false, null);
                            break;
                        case TypeCode.Decimal:
                            p.SetValue(obj, Convert.ToDecimal(0), null);
                            break;
                        case TypeCode.Single:
                            p.SetValue(obj, Convert.ToSingle(0), null);
                            break;
                        case TypeCode.Double:
                            p.SetValue(obj, Convert.ToDouble(0), null);
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            p.SetValue(obj, 0, null);
                            break;
                        case TypeCode.Byte:
                        case TypeCode.Char:
                        case TypeCode.DateTime:
                        case TypeCode.DBNull:
                        case TypeCode.Empty:
                        case TypeCode.Object:
                        case TypeCode.SByte:
                        default:
                            break;
                    }
                }
            }

            return obj;
        }


        #region Name of

        public static string nameof<T, TProp>(this T obj, Expression<Func<T, TProp>> expression)
        {
            return nameof((LambdaExpression)expression);
        }

        private static string nameof(LambdaExpression expression)
        {
            MemberExpression memberExp = expression.Body as MemberExpression;
            if (memberExp != null)
                return memberExp.Member.Name;

            MethodCallExpression methodExp = expression.Body as MethodCallExpression;
            if (methodExp != null)
                return methodExp.Method.Name;

            return string.Empty;
        }

        #endregion
    }

#endregion

    #region Common Function

    public class CommonFunc
    {
        private static string[] POBoxLike = { "%P%BOX%", "%CMB%", "%PBX%", "%POST%OFFICE%", "BOX%", "%GPO%", "%LPO%", "%L.P.O%", "%PA%CEL%LOCK%", "%PA%CEL%COLLECT%", "%PMB%" };
        public static string GetOnlyFileNameByFullName(string fileFullName)
        {
            return fileFullName.Substring(fileFullName.LastIndexOf("\\") + 1, fileFullName.Length - (fileFullName.LastIndexOf("\\") + 1));
        }

        public static bool IsPOBox(string str)
        {
            return POBoxLike.Any(ps => str.Like(ps));
        }

        public static string GetImageFileName(string sku, int imgIndex)
        {
            return sku + "-" + imgIndex.ToString().PadLeft(2, '0') + ".jpg";
        }

        public static string ConvertXMLFromUTF8ToUTF16(string utf8XMLStr)
        {
            string content;
            // Create a memoryStream into which the data can be written and readed
            using (var stream = new MemoryStream())
            {
                // Create a XmlTextWriter to write the xml object source, we are going
                // to define the encoding in the constructor
                using (var writer = new XmlTextWriter(stream, Encoding.Unicode))
                {
                    // write the content into the stream
                    writer.WriteString(utf8XMLStr);

                    // Flush the stream
                    writer.Flush();

                    // Read the stream into a string
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        // Set the stream position to the begin
                        stream.Position = 0;

                        // Read the stream into a string
                        content = reader.ReadToEnd();
                    }
                }
            }
            return content;
        }

        public static string ConvertObjectToUTF16XMLString<T>(T obj)
        {
            string content;
            // Create a memoryStream into which the data can be written and readed
            using (var stream = new MemoryStream())
            {

                // Create the xml serializer, the serializer needs to know the type
                // of the object that will be serialized
                var xmlSerializer = new XmlSerializer(typeof(T));

                // Create a XmlTextWriter to write the xml object source, we are going
                // to define the encoding in the constructor
                using (var writer = new XmlTextWriter(stream, Encoding.Unicode))
                {
                    // Save the state of the object into the stream
                    xmlSerializer.Serialize(writer, obj);

                    // Flush the stream
                    writer.Flush();

                    // Read the stream into a string
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        // Set the stream position to the begin
                        stream.Position = 0;

                        // Read the stream into a string
                        content = reader.ReadToEnd();
                    }
                }
            }
            return content;
        }

        //public static T RestfulXMLCall<T>(string restURL) where T : XMLObject
        //{
        //    using (var webClient = new OMSWebClient())
        //    {
        //        var xmlParser = new XMLObjectParser();
        //        var resultString = webClient.DownloadString(restURL);

        //        var returnObj = xmlParser.ParseStringToObject<T>(resultString);

        //        if (returnObj != null)
        //            returnObj.xmlString = resultString;

        //        return returnObj;
        //    }
        //}

        public static string ToCSVFileName(string prefix)
        {
            var sb = new StringBuilder();
            sb.Append(prefix);
            sb.Append("_");
            sb.Append(DateTime.Now.ToString("yyyyMMddHHmmss"));
            sb.Append(".csv");
            return sb.ToString();
        }

        public static object EvaluateFormula( string formula, IList<KeyValuePair<string, object>> paramNameValues=null)
        {
            var e = new NCalc.Expression(formula);
            if (paramNameValues != null)
            {
                foreach (var p in paramNameValues)
                {
                    e.Parameters.Add(p.Key, p.Value);
                }
            }

            return e.Evaluate();
        }

    }

    #endregion

}
