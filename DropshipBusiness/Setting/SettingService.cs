using DropshipBusiness.eBay;
using DropshipBusiness.Item;
using DropshipCommon.Models;
using DropshipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon;
using eBay.Service.Core.Soap;
using DropshipBusiness.Security;
using System.Web.Security;
using System.Web;
using System.ComponentModel;

namespace DropshipBusiness.Setting
{
    public class SettingService : ISettingService
    {
        private readonly IRepository<T_Setting> _settingRepository;

        public SettingService(IRepository<T_Setting> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public void DeleteSetting(T_Setting setting)
        {
            if (setting != null)
                _settingRepository.Delete(setting);
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettings();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key];

                if (setting != null)
                    return CommonFunc.To<T>(setting.Value);
            }

            return defaultValue;
        }

        protected virtual IDictionary<string, T_Setting> GetAllSettings()
        {
            var query = from s in _settingRepository.Table
                        orderby s.Name
                        select s;
            var settings = query.ToList();

            var dictionary = new Dictionary<string, T_Setting>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                var setting= new T_Setting
                {
                    ID = s.ID,
                    Name = s.Name,
                    Value = s.Value
                };
                if (!dictionary.ContainsKey(resourceName))
                {
                    dictionary.Add(resourceName, setting);
                }
                else
                {
                    dictionary[resourceName]=setting;
                }
            }
            return dictionary;
        }

        public void InsertSetting(T_Setting setting)
        {
            if (setting != null)
                _settingRepository.Insert(setting);
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        public void UpdateSetting(T_Setting setting)
        {
            if (setting != null)
                _settingRepository.Update(setting);
        }
    }
}
