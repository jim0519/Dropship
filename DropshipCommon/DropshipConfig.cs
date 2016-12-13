using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Principal;
using System.Xml;

namespace DropshipCommon
{
    public class DropshipConfig
    {
        private static DropshipConfig _instance;
        private DropshipConfig()
        {
        
        }

        public static DropshipConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DropshipConfig();
                }
                return _instance;
            }
        }

        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DropshipContext"].ConnectionString;
            }
        }

        public eBayAPIConfigCollection eBayAPIConfigList
        {
            get
            {
                return (eBayAPIConfigCollection)System.Configuration.ConfigurationManager.GetSection("eBayAPIConfigs");
            }
        }

        public string ImageFilesPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Content/ItemImages/");
            }
        }


    }

    #region eBay API Contexts

    public class eBayAPIConfigHandler : IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, XmlNode section)
        {
            eBayAPIConfigCollection collection = new eBayAPIConfigCollection();
            foreach (XmlNode child in section.ChildNodes)
            {
                var eBayAPIContextConfig = new eBayAPIContextConfig();
                foreach (XmlNode grandChild in child.ChildNodes)
                {
                    eBayAPIContextConfig.APIContextConfigNodes.Add(grandChild.Name, grandChild.InnerText);
                }
                collection.Add(eBayAPIContextConfig);
            }
            return collection;
        }
    }

    public class eBayAPIConfigCollection : List<eBayAPIContextConfig>
    {

    }

    public class eBayAPIContextConfig
    {
        private Dictionary<string, string> _apiContextConfig;
        public eBayAPIContextConfig()
        {
            _apiContextConfig = new Dictionary<string, string>();
        }

        public Dictionary<string, string> APIContextConfigNodes
        {
            get { return _apiContextConfig; }
        }

        public string SellerID { get { return _apiContextConfig["SellerID"].ToString(); } }
        public string ServiceURL { get { return _apiContextConfig["ServiceURL"].ToString(); } }
        public string eBayToken { get { return _apiContextConfig["eBayToken"].ToString(); } }
        public string eBaySiteID { get { return _apiContextConfig["eBaySiteID"].ToString(); } }
        public string DevID { get { return _apiContextConfig["DevID"].ToString(); } }
        public string AppID { get { return _apiContextConfig["AppID"].ToString(); } }
        public string CertID { get { return _apiContextConfig["CertID"].ToString(); } }
    }


    #endregion
}
